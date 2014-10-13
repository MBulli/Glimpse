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
                AssocQueryString(AssocF.None,
                                 AssocStr.ShellExtension,
                                 ext,
                                 "{8895B1C6-B41F-4C1C-A562-0D564250836F}",
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

        public static PerceivedType GetPerceivedType(string filenameOrExtension)
        {
            string ext = Path.GetExtension(filenameOrExtension);
            PerceivedType result;
            uint flags;

            uint hresult = AssocGetPerceivedType(ext, out result, out flags, null);

            if (hresult != S_OK)
            {
                Debug.WriteLine("Inside Win32.GetPerceivedType(): AssocGetPerceivedType failed with hresult 0x{0:X} with file or ext '{1}'",
                                  hresult, filenameOrExtension);
                return PerceivedType.Unspecified;
            }
            else
            {
                return result;
            }
        }

        // http://msdn.microsoft.com/en-us/library/windows/desktop/bb773463(v=vs.85).aspx
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint AssocGetPerceivedType(
            string pszExt,
            out PerceivedType ptype,
            out uint pflag,
            [Out] StringBuilder ppszType);

        [DllImport("Shlwapi.dll", SetLastError = true)]
        static extern uint AssocQueryString(
            AssocF flags,
            AssocStr str,
            string pszAssoc,
            string pszExtra,
            [Out] StringBuilder pszOut,
            [In][Out] ref uint pcchOut);


        [Flags]
        enum AssocF
        {
            None = 0,
            Init_NoRemapCLSID = 0x1,
            Init_ByExeName = 0x2,
            Open_ByExeName = 0x2,
            Init_DefaultToStar = 0x4,
            Init_DefaultToFolder = 0x8,
            NoUserSettings = 0x10,
            NoTruncate = 0x20,
            Verify = 0x40,
            RemapRunDll = 0x80,
            NoFixUps = 0x100,
            IgnoreBaseClass = 0x200,
            Init_IgnoreUnknown = 0x400,
            Init_FixedProgId = 0x800,
            IsProtocol = 0x1000,
            InitForFile = 0x2000,
        }

        enum AssocStr
        {
            Command = 1,
            Executable,
            FriendlyDocName,
            FriendlyAppName,
            NoOpen,
            ShellNewValue,
            DDECommand,
            DDEIfExec,
            DDEApplication,
            DDETopic,
            InfoTip,
            QuickTip,
            TileInfo,
            ContentType,
            DefaultIcon,
            ShellExtension,
            DropTarget,
            DelegateExecute,
            SupportedUriProtocols,
            Max,
        }

        public enum PerceivedType
        {
            /// <summary>
            /// The file's perceived type as defined in the registry is not a known type.
            /// </summary>
            Custom = -3,
            /// <summary>
            /// The file does not have a perceived type.
            /// </summary>
            Unspecified = -2,
            Folder = -1,
            Unknown = 0,
            Text = 1,
            Image = 2,
            Audio = 3,
            Video = 4,
            Compressed = 5,
            Document = 6,
            System = 7,
            Application = 8,
            Gamemedia = 9,
            Contacts = 10
        }
    }
}
