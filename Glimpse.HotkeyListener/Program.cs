using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glimpse.HotkeyListener
{
    static class Program
    {
        static Mutex singleInstanceMutex = new Mutex(true, "GlimpseHotkeyListener_DFACB039-F556-433E-9986-B86A32F9CD15");

        [STAThread]
        static void Main()
        {
            if (singleInstanceMutex.WaitOne(TimeSpan.Zero, exitContext: true))
            {
                Application.Run(new HotkeyApplicationContext());
                singleInstanceMutex.ReleaseMutex();
            }
        }
    }
}
