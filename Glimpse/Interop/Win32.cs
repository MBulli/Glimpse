using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.Interop
{
    static class Win32
    {
        private const uint S_OK = 0x00;

        public static Guid PreviewHandlerGuid(string file)
        {
            string ext = System.IO.Path.GetExtension(file);

            uint lenght = 255;
            StringBuilder buffer = new StringBuilder(255);

            uint hresult = 
                NativeMethods.AssocQueryString(
                                 NativeMethods.AssocF.None,
                                 NativeMethods.AssocStr.ShellExtension,
                                 ext,
                                 typeof(IPreviewHandler).GUID.ToString("B"),
                                 buffer,
                                 ref lenght);

            if (hresult != S_OK)
            {
                Debug.WriteLine("Inside: Win32.PreviewHandlerGuid(): AssocQueryString failed with hresult 0x{0:X} with file '{1}'",
                                    hresult, file);
                return Guid.Empty;
            }
            else
            {
                Guid result;
                if (Guid.TryParse(buffer.ToString(), out result))
                    return result;
                else
                    return Guid.Empty;
            }
        }

        public static int GetPerceivedType(string filenameOrExtension)
        {
            string ext = Path.GetExtension(filenameOrExtension);
            int result;
            uint flags;

            uint hresult = NativeMethods.AssocGetPerceivedType(ext, out result, out flags, null);

            if (hresult != S_OK)
            {
                Debug.WriteLine("Inside Win32.GetPerceivedType(): AssocGetPerceivedType failed with hresult 0x{0:X} with file or ext '{1}'",
                                  hresult, filenameOrExtension);
                return -2;
            }
            else
            {
                return result;
            }
        }
    }
}
