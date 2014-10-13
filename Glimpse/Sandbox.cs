using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//using Shell32;
//using SHDocVw;
//using ShellObjects;

namespace Glimpse
{
    class Sandbox
    {
        const int CSIDL_DESKTOP = 0x0000;
        const int SWC_DESKTOP = 0x00000008;
        const int SWFO_NEEDDISPATCH = 0x00000001;

        const int WM_USER = 0x400;

        static Guid SID_STopLevelBrowser = new Guid("4C96BE40-915C-11CF-99D3-00AA004AE837");
        static Guid IID_IShellBrowser = new Guid("000214E2-0000-0000-C000-000000000046");
        static Guid IID_IServiceProvider = new Guid("6d5140c1-7436-11ce-8034-00aa006009fa");
        static Guid IID_IDispatch = new Guid("00020400-0000-0000-C000-000000000046");

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
        internal interface IServiceProvider
        {
            void QueryService([MarshalAs(UnmanagedType.LPStruct)] Guid guidService, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);
        }

        //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214E2-0000-0000-C000-000000000046")]
        //internal interface IShellBrowser
        //{
        //    void _VtblGap0_12(); // skip 12 members
        //    void QueryActiveShellView([MarshalAs(UnmanagedType.IUnknown)] out object ppshv);
        //    // the rest is not defined
        //}


        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("cde725b0-ccc9-4519-917e-325d72fab4ce")]
        internal interface IFolderView
        {
            void _VtblGap0_2(); // skip 2 members
            void GetFolder([MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
            // the rest is not defined
        }

        enum ShellWindowTypeConstants
        {
            SWC_EXPLORER = 0x0,
            SWC_BROWSER = 0x00000001,
            SWC_3RDPARTY = 0x00000002,
            SWC_CALLBACK = 0x00000004,
            SWC_DESKTOP = 0x00000008
        }
        enum ShellWindowFindWindowOptions
        {
            SWFO_NEEDDISPATCH = 0x00000001,
            SWFO_INCLUDEPENDING = 0x00000002,
            SWFO_COOKIEPASSED = 0x00000004
        }

        enum SVGIO : uint
        {
            SVGIO_BACKGROUND = 0x00000000,
            SVGIO_SELECTION = 0x00000001,
            SVGIO_ALLVIEW = 0x00000002,
            SVGIO_CHECKED = 0x00000003,
            SVGIO_TYPE_MASK = 0x0000000F,
            SVGIO_FLAG_VIEWORDER = 0x80000000
        } 

        static void Test()
        {
            // see:
            // http://blogs.msdn.com/b/oldnewthing/archive/2013/04/22/10412906.aspx
            // http://blogs.msdn.com/b/oldnewthing/archive/2004/07/20/188696.aspx

            Type typeShell = Type.GetTypeFromProgID("Shell.Application");
            dynamic shell = Activator.CreateInstance(typeShell);
            dynamic shellWindows = shell.Windows(); // IShellWindows

            for (var i = 0; i < shellWindows.Count; i++)
            {
                dynamic w = shellWindows.Item(i);  // IWebBrowserApp

                IServiceProvider serv = w as IServiceProvider;

                //object sb;
                //serv.QueryService(SID_STopLevelBrowser, typeof(IShellBrowser).GUID, out sb);
                //IShellBrowser shellBrowser = (IShellBrowser)sb;

                //IShellView sv;
                //shellBrowser.QueryActiveShellView(out sv);
                //IFolderView fv = sv as IFolderView;

                //IntPtr some;
                //sv.GetItemObject((uint)SVGIO.SVGIO_BACKGROUND, ref IID_IDispatch, out some);

                //IntPtr sfv;
                //Guid guid = typeof(ShellFolderView).GUID;
                //Marshal.QueryInterface(some, ref guid, out sfv);
                //object sadwq = Marshal.GetObjectForIUnknown(sfv);

                //ShellFolderView folderView = (ShellFolderView)sadwq;
                //folderView.SelectionChanged += folderView_SelectionChanged;
                
                long hwnd = w.HWND;
                //Console.WriteLine(w.LocationName);
                
                dynamic selection = w.Document.SelectedItems();

                for (var j = 0; j < selection.Count; j++)
                {
                    dynamic item = selection.Item(j);
                    //Console.WriteLine(item.Name);
                    Console.WriteLine(item.Path);
                }
            }
        }

        static void folderView_SelectionChanged()
        {
            
        }


        public static void ExportFileClasses()
        {
            StringBuilder csv = new StringBuilder();

            csv.AppendLine("Ext;PerceivedType;ContentType;Previewhandler");

            var subKeys = from name in Registry.ClassesRoot.GetSubKeyNames()
                          where name.StartsWith(".")
                          select name;

            foreach (var subKeyName in subKeys)
            {
                using (RegistryKey subKey = Registry.ClassesRoot.OpenSubKey(subKeyName))
                {
                    string ctype = subKey.GetValue("Content Type", "") as string;

                    var ptype = Glimpse.Interop.Win32.GetPerceivedType(subKeyName);
                    string ptypeStr = ptype == Interop.Win32.PerceivedType.Unspecified ? string.Empty : ptype.ToString();
                    
                    Guid previewGUID = Glimpse.Interop.Win32.PreviewHandlerGuid(subKeyName);
                    string preview = previewGUID == Guid.Empty ? string.Empty : previewGUID.ToString();

                    csv.AppendFormat("{0};{1};{2};{3}", subKeyName, ptypeStr, ctype, preview);
                    csv.AppendLine();
                }
            }

            File.WriteAllText("Extensions.csv", csv.ToString());
        }
    }
}
