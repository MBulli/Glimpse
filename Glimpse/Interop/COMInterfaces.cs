using Microsoft.WindowsAPICodePack.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [Guid("8895B1C6-B41F-4C1C-A562-0D564250836F")]
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPreviewHandler
    {
        int SetWindow(IntPtr hwnd, ref RECT rect);
        int SetRect(ref RECT rect);
        int DoPreview();
        int Unload();
        int SetFocus();
        int QueryFocus(out IntPtr phwnd);
        [PreserveSig]
        int TranslateAccelerator(ref Message pmsg);
    }

    [Guid("B7D14566-0509-4CCE-A71F-0A554233BD9B")]
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IInitializeWithFile
    {
        int Initialize([MarshalAs(UnmanagedType.LPWStr)] string pszFilePath, uint grfMode);
    }

    [Guid("B824B49D-22AC-4161-AC8A-9916E8FA3F7F")]
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IInitializeWithStream
    {
        int Initialize(IStream pstream, uint grfMode);
    }
}
