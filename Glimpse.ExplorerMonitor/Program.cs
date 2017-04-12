using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glimpse.ExplorerMonitor
{
    static class Program
    {
        static Mutex singleInstanceMutex = new Mutex(true, "GlimpseExplorerMonitor_11E8EFF0-196D-493F-99F5-2B9CF36AA6BB");

        [STAThread]
        static void Main(string[] args)
        {
            if (singleInstanceMutex.WaitOne(TimeSpan.Zero, exitContext: true))
            {
                // TODO: pass this process id as parameter to observer. This allows the child process to monitor its parent and gracfully exit when glimpse is closed
                //int parentProcessID = int.Parse(args[0]);

                //var parentProc = System.Diagnostics.Process.GetProcessById(parentProcessID);
                //parentProc.Exited += ParentProc_Exited;


                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form1());

                Application.Run(new MonitorApplicationContext());
                singleInstanceMutex.ReleaseMutex();
            }
        }

        private static void ParentProc_Exited(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
