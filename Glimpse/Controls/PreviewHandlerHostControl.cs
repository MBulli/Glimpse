﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Glimpse.Interop;

namespace Glimpse.Controls
{
    // thanks to:
    // http://www.brad-smith.info/blog/archives/79

    public class PreviewHandlerHostControl : Control
    {
        private object mCurrentPreviewHandler;
        private Guid mCurrentPreviewHandlerGUID;
        private Stream mCurrentPreviewHandlerStream;
        private string mErrorMessage;

        private string ErrorMessage
        {
            get { return mErrorMessage; }
            set
            {
                mErrorMessage = value;
                Invalidate();	// repaint the control
            }
        }

        /// <summary>
        /// Gets or sets the background colour of this PreviewHandlerHost.
        /// </summary>
        [DefaultValue("White")]
        public override System.Drawing.Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        /// <summary>
        /// Initialialises a new instance of the PreviewHandlerHost class.
        /// </summary>
        public PreviewHandlerHostControl()
            : base()
        {
            mCurrentPreviewHandlerGUID = Guid.Empty;
            BackColor = Color.White;
            Size = new Size(320, 240);

            // display default error message (no file)
            ErrorMessage = "No file loaded.";

            // enable transparency
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the PreviewHandlerHost and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (Marshal.AreComObjectsAvailableForCleanup())
            { 
                UnloadPreviewHandler();
            }

            if (mCurrentPreviewHandler != null)
            {
                Marshal.FinalReleaseComObject(mCurrentPreviewHandler);
                mCurrentPreviewHandler = null;
                GC.Collect();
            }

            base.Dispose(disposing);
        }

        private Guid GetPreviewHandlerGUID(string filename)
        {
            return Win32.PreviewHandlerGuid(filename);


            // open the registry key corresponding to the file extension
            //RegistryKey ext = Registry.ClassesRoot.OpenSubKey(Path.GetExtension(filename));
            //if (ext != null)
            //{
            //    // open the key that indicates the GUID of the preview handler type
            //    RegistryKey test = ext.OpenSubKey("shellex\\{8895b1c6-b41f-4c1c-a562-0d564250836f}");
            //    if (test != null) return new Guid(Convert.ToString(test.GetValue(null)));

            //    // sometimes preview handlers are declared on key for the class
            //    string className = Convert.ToString(ext.GetValue(null));
            //    if (className != null)
            //    {
            //        test = Registry.ClassesRoot.OpenSubKey(className + "\\shellex\\{8895b1c6-b41f-4c1c-a562-0d564250836f}");
            //        if (test != null) return new Guid(Convert.ToString(test.GetValue(null)));
            //    }
            //}

            //return Guid.Empty;
        }

        /// <summary>
        /// Paints the error message text on the PreviewHandlerHost control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (mErrorMessage != String.Empty)
            {
                // paint the error message
                TextRenderer.DrawText(
                    e.Graphics,
                    mErrorMessage,
                    Font,
                    ClientRectangle,
                    ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
                );
            }
        }

        /// <summary>
        /// Resizes the hosted preview handler when this PreviewHandlerHost is resized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (mCurrentPreviewHandler is IPreviewHandler)
            {
                // update the preview handler's bounds to match the control's
                Rectangle r = ClientRectangle;
                //((IPreviewHandler)mCurrentPreviewHandler).SetWindow(Handle, ref r);
                ((IPreviewHandler)mCurrentPreviewHandler).SetRect(ref r);
            }
        }

        /// <summary>
        /// Opens the specified file using the appropriate preview handler and displays the result in this PreviewHandlerHost.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Open(string filename)
        {
            UnloadPreviewHandler();

            if (String.IsNullOrEmpty(filename))
            {
                ErrorMessage = "No file loaded.";
                return false;
            }

            // try to get GUID for the preview handler
            Guid guid = GetPreviewHandlerGUID(filename);
            ErrorMessage = "";

            if (guid != Guid.Empty)
            {
                try
                {
                    if (guid != mCurrentPreviewHandlerGUID)
                    {
                        mCurrentPreviewHandlerGUID = guid;

                        // need to instantiate a different COM type (file format has changed)
                        if (mCurrentPreviewHandler != null) Marshal.FinalReleaseComObject(mCurrentPreviewHandler);

                        // use reflection to instantiate the preview handler type
                        Type comType = Type.GetTypeFromCLSID(mCurrentPreviewHandlerGUID);
                        mCurrentPreviewHandler = Activator.CreateInstance(comType);
                    }

                    if (mCurrentPreviewHandler is IInitializeWithFile)
                    {
                        // some handlers accept a filename
                        ((IInitializeWithFile)mCurrentPreviewHandler).Initialize(filename, 0);
                    }
                    else if (mCurrentPreviewHandler is IInitializeWithStream)
                    {
                        if (File.Exists(filename))
                        {
                            // other handlers want an IStream (in this case, a file stream)
                            mCurrentPreviewHandlerStream = File.Open(filename, FileMode.Open);
                            StreamWrapper stream = new StreamWrapper(mCurrentPreviewHandlerStream);
                            ((IInitializeWithStream)mCurrentPreviewHandler).Initialize(stream, 0);
                        }
                        else
                        {
                            ErrorMessage = "File not found.";
                        }
                    }

                    if (mCurrentPreviewHandler is IPreviewHandler)
                    {
                        // bind the preview handler to the control's bounds and preview the content
                        Rectangle r = ClientRectangle;
                        ((IPreviewHandler)mCurrentPreviewHandler).SetWindow(Handle, ref r);
                        ((IPreviewHandler)mCurrentPreviewHandler).DoPreview();

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Preview could not be generated.\n" + ex.Message;
                }
            }
            else
            {
                ErrorMessage = "No preview available.";
            }

            return false;
        }

        /// <summary>
        /// Opens the specified stream using the preview handler COM type with the provided GUID and displays the result in this PreviewHandlerHost.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="previewHandler"></param>
        /// <returns></returns>
        public bool Open(Stream stream, Guid previewHandler)
        {
            UnloadPreviewHandler();

            if (stream == null)
            {
                ErrorMessage = "No file loaded.";
                return false;
            }

            ErrorMessage = "";

            if (previewHandler != Guid.Empty)
            {
                try
                {
                    if (previewHandler != mCurrentPreviewHandlerGUID)
                    {
                        mCurrentPreviewHandlerGUID = previewHandler;

                        // need to instantiate a different COM type (file format has changed)
                        if (mCurrentPreviewHandler != null) Marshal.FinalReleaseComObject(mCurrentPreviewHandler);

                        // use reflection to instantiate the preview handler type
                        Type comType = Type.GetTypeFromCLSID(mCurrentPreviewHandlerGUID);
                        mCurrentPreviewHandler = Activator.CreateInstance(comType);
                    }

                    if (mCurrentPreviewHandler is IInitializeWithStream)
                    {
                        // must wrap the stream to provide compatibility with IStream
                        mCurrentPreviewHandlerStream = stream;
                        StreamWrapper wrapped = new StreamWrapper(mCurrentPreviewHandlerStream);
                        ((IInitializeWithStream)mCurrentPreviewHandler).Initialize(wrapped, 0);
                    }

                    if (mCurrentPreviewHandler is IPreviewHandler)
                    {
                        // bind the preview handler to the control's bounds and preview the content
                        Rectangle r = ClientRectangle;
                        ((IPreviewHandler)mCurrentPreviewHandler).SetWindow(Handle, ref r);
                        ((IPreviewHandler)mCurrentPreviewHandler).DoPreview();

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Preview could not be generated.\n" + ex.Message;
                }
            }
            else
            {
                ErrorMessage = "No preview available.";
            }

            return false;
        }

        /// <summary>
        /// Unloads the preview handler hosted in this PreviewHandlerHost and closes the file stream.
        /// </summary>
        public void UnloadPreviewHandler()
        {
            if (mCurrentPreviewHandler is IPreviewHandler)
            {
                // explicitly unload the content
                ((IPreviewHandler)mCurrentPreviewHandler).Unload();
            }
            if (mCurrentPreviewHandlerStream != null)
            {
                mCurrentPreviewHandlerStream.Close();
                mCurrentPreviewHandlerStream = null;
            }
        }
    }
}
