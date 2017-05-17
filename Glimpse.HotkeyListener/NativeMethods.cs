using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.HotkeyListener
{
    class NativeMethods
    {
        [Flags]
        public enum HotkeyModifiers : uint
        {
            /// <summary>
            /// Either ALT key must be held down.
            /// </summary>
            MOD_ALT = 0x0001,
            /// <summary>
            /// Either CTRL key must be held down.
            /// </summary>
            MOD_CONTROL = 0x0002,
            /// <summary>
            /// Changes the hotkey behavior so that the keyboard auto-repeat does not yield multiple hotkey notifications.
            /// Windows Vista or older:  This flag is not supported.
            /// </summary>
            MOD_NOREPEAT = 0x4000,
            /// <summary>
            /// Either SHIFT key must be held down.
            /// </summary>
            MOD_SHIFT = 0x0004,
            /// <summary>
            /// Either WINDOWS key was held down. Keyboard shortcuts that involve the WINDOWS key are reserved for use by the operating system.
            /// </summary>
            MOD_WIN = 0x0008
        }

        public const uint WH_HOTKEY = 0x0312;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();
                
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern ushort GlobalAddAtom(string lpString);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern ushort GlobalDeleteAtom(ushort nAtom);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, HotkeyModifiers fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        public static string GetClassName(IntPtr hWnd)
        {
            var buffer = new StringBuilder(255); // 255 bytes should be enough space for any string

            if (GetClassName(hWnd, buffer, buffer.Capacity) == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return buffer.ToString();
        }
    }
}
