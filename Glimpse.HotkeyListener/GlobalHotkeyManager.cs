using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glimpse.HotkeyListener
{
    class GlobalHotkeyManager : IMessageFilter
    {
        class HotkeyInfo
        {
            public IntPtr WindowHandle;
            public ushort ID;
            public Hotkey Hotkey;
            public Action<Hotkey> Action;
        }

        private Dictionary<Hotkey, HotkeyInfo> registeredHotkeys = new Dictionary<Hotkey, HotkeyInfo>();


        public GlobalHotkeyManager()
        {
            
        }
    
        public void RegisterHotkey(Hotkey hotkey, Action<Hotkey> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (registeredHotkeys.ContainsKey(hotkey))
                return;
          
            HotkeyInfo hi = new HotkeyInfo();
            hi.Hotkey = hotkey;
            hi.Action = action;
            hi.WindowHandle = IntPtr.Zero;           

            string uniqueID = Guid.NewGuid().ToString("N");
            hi.ID = NativeMethods.GlobalAddAtom(uniqueID);

            if (hi.ID == 0)
                throw new Exception($"Failed to create unique ID for hotkey {hotkey}", new Win32Exception(Marshal.GetLastWin32Error()));

            var modifiers = ModifiersFromHotkey(hi.Hotkey);
            var keycode = hi.Hotkey.KeyCode & Keys.KeyCode;

            if (NativeMethods.RegisterHotKey(hi.WindowHandle, hi.ID, modifiers, (uint)keycode))
            {
                registeredHotkeys.Add(hotkey, hi);
            }
            else
            {
                int error = Marshal.GetLastWin32Error();

                // Cleanup atom
                NativeMethods.GlobalDeleteAtom(hi.ID);
                hi.ID = 0;

                throw new Exception($"Failed to register hotkey {hotkey}", new Win32Exception(error));
            }
        }

        public void UnregisterHotkey(Hotkey hotkey)
        {
            if (registeredHotkeys.TryGetValue(hotkey, out HotkeyInfo hi))
            {
                if (NativeMethods.UnregisterHotKey(hi.WindowHandle, hi.ID))
                {
                    NativeMethods.GlobalDeleteAtom(hi.ID);
                    registeredHotkeys.Remove(hotkey);
                }
            }
        }
        
        public void UnregisterAllHotkeys()
        {
            foreach (var hotkey in registeredHotkeys.Keys)
            {
                UnregisterHotkey(hotkey);
            }
        }

        private NativeMethods.HotkeyModifiers ModifiersFromHotkey(Hotkey hotkey)
        {
            NativeMethods.HotkeyModifiers result = 0;

            if (hotkey.Alt)
                result |= NativeMethods.HotkeyModifiers.MOD_ALT;
            if (hotkey.Ctrl)
                result |= NativeMethods.HotkeyModifiers.MOD_CONTROL;
            if (hotkey.Shift)
                result |= NativeMethods.HotkeyModifiers.MOD_SHIFT;
            if (hotkey.Win)
                result |= NativeMethods.HotkeyModifiers.MOD_WIN;

            return result;
        }

        /// <summary>
        /// Filters out a message before it is dispatched.
        /// </summary>
        /// <param name="m">The message to be dispatched. You cannot modify this message.</param>
        /// <returns>true to filter the message and stop it from being dispatched; false to allow the message to continue to the next filter or control.</returns>
        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            bool filtered = false;

            if (m.Msg == NativeMethods.WH_HOTKEY)
            {
                // low-order word:  pressed modifier keys
                // high-order word: the keycode of the hotkey
                Keys keycode = (Keys)(((uint)m.LParam) >> 16);
                var modifier = (NativeMethods.HotkeyModifiers)(((uint)m.LParam) & 0x0000_FFFF);

                var hotkey = new Hotkey(keycode, 
                                        alt:   modifier.HasFlag(NativeMethods.HotkeyModifiers.MOD_ALT),
                                        ctrl:  modifier.HasFlag(NativeMethods.HotkeyModifiers.MOD_CONTROL),
                                        shift: modifier.HasFlag(NativeMethods.HotkeyModifiers.MOD_SHIFT),
                                        win:   modifier.HasFlag(NativeMethods.HotkeyModifiers.MOD_WIN));


                if (registeredHotkeys.TryGetValue(hotkey, out HotkeyInfo hi))
                {
                    hi.Action?.Invoke(hi.Hotkey);
                    filtered = true;
                }
            }

            return filtered;
        }
    }

    public struct Hotkey
    {
        public readonly Keys KeyCode;
        public readonly bool Alt;
        public readonly bool Ctrl;
        public readonly bool Shift;
        public readonly bool Win;

        public Hotkey(Keys keyCode, bool alt = false, bool ctrl = false, bool shift = false, bool win = false)
        {
            KeyCode = keyCode;
            Alt = alt;
            Ctrl = ctrl;
            Shift = shift;
            Win = win;
        }

        public override string ToString()
        {
            return string.Concat(
                "Hotkey: ",
                (Win ? "Win + " : ""),
                (Ctrl ? "Ctrl + " : ""),
                (Shift ? "Shift + " : ""),
                (Alt ? "Alt + " : ""),
                KeyCode);
        }
    }
}
