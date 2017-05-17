using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glimpse.HotkeyListener
{
    class HotkeyApplicationContext : ApplicationContext
    {
        GlobalHotkeyManager hotkeyManager;

        public HotkeyApplicationContext()
        {
            hotkeyManager = new GlobalHotkeyManager();
            hotkeyManager.RegisterHotkey(new Hotkey(Keys.Space, ctrl: true, shift: true), OnHotkeyPressed);

            Application.AddMessageFilter(hotkeyManager);
        }

        protected override void Dispose(bool disposing)
        {
            Application.RemoveMessageFilter(hotkeyManager);
            hotkeyManager.UnregisterAllHotkeys();

            base.Dispose(disposing);
        }

        private void OnHotkeyPressed(Hotkey hotkey)
        {
            if (hotkey.Ctrl && hotkey.Shift && hotkey.KeyCode == Keys.Space)
            {
                IntPtr hwnd = NativeMethods.GetForegroundWindow();
                string wndClass = NativeMethods.GetClassName(hwnd);

                if (wndClass == "CabinetWClass")
                {
                    Process.Start("Glimpse.exe", $"0x{hwnd.ToString("X")}");
                }
            }
        }
    }
}
