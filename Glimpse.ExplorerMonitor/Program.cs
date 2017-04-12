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
                var cmdArgs = ParseCommandlineArguments(args);

                if (cmdArgs.ShowDebugForm)
                {
                    // needs to be called befor MonitorApplicationContext.ctor()
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                }

                Application.Run(new MonitorApplicationContext(cmdArgs));
                singleInstanceMutex.ReleaseMutex();
            }
        }

        static CommandlineOptions ParseCommandlineArguments(string[] args)
        {
            CommandlineOptions result = new CommandlineOptions();

            foreach (var arg in args.Select(a => a.ToLowerInvariant()))
            {
                if (arg.StartsWith("/parent:"))
                {
                    var parts = arg.Split(':');

                    if (parts?.Length == 2 && int.TryParse(parts[1], out int pid))
                    {
                        result.ParentProcessId = pid;
                    }
                }
                else if (arg == "/debugform")
                {
                    result.ShowDebugForm = true;
                }
            }

            return result;
        }
    }
}
