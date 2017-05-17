using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glimpse.Interop
{
    // thanks to:
    // http://www.brad-smith.info/blog/archives/79
    // This is a WinForms control; No WPF!

    class WindowsPreviewControl : Control
    {
        private object currentPreviewHandler;
        private Guid currentPreviewHandlerGUID;
        private Stream currentPreviewHandlerStream;

        public WindowsPreviewControl()
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            if (Marshal.AreComObjectsAvailableForCleanup())
            {
                UnloadPreviewHandler();
            }

            if (currentPreviewHandler != null)
            {
                Marshal.FinalReleaseComObject(currentPreviewHandler);
                currentPreviewHandler = null;
                GC.Collect();
            }
            if (currentPreviewHandlerStream != null)
            {
                currentPreviewHandlerStream.Close();
                currentPreviewHandlerStream = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Unloads the preview handler hosted in this PreviewHandlerHost and closes the file stream.
        /// </summary>
        private void UnloadPreviewHandler()
        {
            if (currentPreviewHandler is IPreviewHandler)
            {
                // explicitly unload the content
                ((IPreviewHandler)currentPreviewHandler).Unload();
            }
            if (currentPreviewHandlerStream != null)
            {
                currentPreviewHandlerStream.Close();
                currentPreviewHandlerStream = null;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Resizes the hosted preview handler when this PreviewHandlerHost is resized.
            if (currentPreviewHandler is IPreviewHandler)
            {
                // update the preview handler's bounds to match the control's
                RECT rect = ClientRECT();
                ((IPreviewHandler)currentPreviewHandler).SetRect(ref rect);
            }
        }

        public bool Open(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                UnloadPreviewHandler();
                return false;
            }

            // stream lifetime is bound to control lifetime 
            Guid previewHandler = GetPreviewHandlerGUID(filename);
            return Open(filename, previewHandler);
        }

        /// <summary>
        /// Opens the specified stream using the preview handler COM type with the provided GUID and displays the result in this PreviewHandlerHost.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="previewHandler"></param>
        /// <returns></returns>
        public bool Open(string filename, Guid previewHandler)
        {
            UnloadPreviewHandler();

            if (string.IsNullOrEmpty(filename))
                return false;
            if (previewHandler == Guid.Empty)
                return false;


            if (previewHandler != currentPreviewHandlerGUID)
            {
                currentPreviewHandlerGUID = previewHandler;

                // need to instantiate a different COM type (file format has changed)
                if (currentPreviewHandler != null)
                    Marshal.FinalReleaseComObject(currentPreviewHandler);

                // use reflection to instantiate the preview handler type
                Type comType = Type.GetTypeFromCLSID(currentPreviewHandlerGUID);
                currentPreviewHandler = Activator.CreateInstance(comType);
            }

            if (currentPreviewHandler is IInitializeWithStream)
            {
                // must wrap the stream to provide compatibility with IStream
                this.currentPreviewHandlerStream = File.OpenRead(filename);
                StreamWrapper wrapped = new StreamWrapper(currentPreviewHandlerStream);
                ThrowOnFailure(((IInitializeWithStream)currentPreviewHandler).Initialize(wrapped, 0));
            }
            else if (currentPreviewHandler is IInitializeWithFile)
            {
                ThrowOnFailure(((IInitializeWithFile)currentPreviewHandler).Initialize(filename, 0));
            }

            if (currentPreviewHandler is IPreviewHandler)
            {
                // bind the preview handler to the control's bounds and preview the content
                RECT rect = ClientRECT();
                ThrowOnFailure(((IPreviewHandler)currentPreviewHandler).SetWindow(Handle, ref rect));
                ThrowOnFailure(((IPreviewHandler)currentPreviewHandler).SetRect(ref rect));
                ThrowOnFailure(((IPreviewHandler)currentPreviewHandler).DoPreview());

                return true;
            }

            return false;
        }

        private RECT ClientRECT()
        {
            return new RECT()
            {
                left = this.ClientRectangle.X,
                right = this.ClientRectangle.X + this.ClientRectangle.Width,
                top = this.ClientRectangle.Y,
                bottom = this.ClientRectangle.Y + this.ClientRectangle.Height
            };
        }

        private Guid GetPreviewHandlerGUID(string filename)
        {
            return Win32.PreviewHandlerGuid(filename);
        }

        private void ThrowOnFailure(int hr)
        {
            if (hr != 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }
        }
    }
}
