using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XDMessaging;

namespace Glimpse.ExplorerMonitor
{
    class MonitorApplicationContext : ApplicationContext
    {
        XDMessagingClient messageClient;

        IXDListener sink;
        IXDBroadcaster broadcaster;
        ExplorerMonitor observer;

        public MonitorApplicationContext(CommandlineOptions commandline)
        {
            messageClient = new XDMessagingClient();
            broadcaster = messageClient.Broadcasters.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);

            sink = messageClient.Listeners.GetListenerForMode(XDTransportMode.HighPerformanceUI);
            sink.RegisterChannel("ExplorerObserverCommand");

            sink.MessageReceived += Sink_MessageReceived;

            observer = new ExplorerMonitor();
            observer.ExplorerSelectionChanged += Observer_ExplorerSelectionChanged;
            observer.ExplorerWindowGotFocus += Observer_ExplorerWindowGotFocus;

            WatchParentProcessExit(commandline.ParentProcessId);

            observer.Start();

            if (commandline.ShowDebugForm)
            {
                this.MainForm = new DebugForm();
                this.MainForm.Show();
            }
        }

        private void WatchParentProcessExit(int parentProcessId)
        {
            if (parentProcessId <= 0)
                return;

            var parentProcess = Process.GetProcessById(parentProcessId);
            parentProcess.EnableRaisingEvents = true;
            parentProcess.Exited += ParentProcess_Exited;
        }

        private void ParentProcess_Exited(object sender, EventArgs e)
        {
            Debug.WriteLine($"Parent process exited.");
            Shutdown();
        }

        private void Shutdown()
        {
            Debug.WriteLine("Shutting down.");

            observer.Stop();
            Application.Exit();
        }

        private void Sink_MessageReceived(object sender, XDMessageEventArgs e)
        {
            if (e.DataGram.Channel == "ExplorerObserverCommand")
            {
                if (e.DataGram.Message == "shutdown")
                {
                    Debug.WriteLine("Shutdown command received.");
                    Shutdown();
                }
            }
        }

        private void Observer_ExplorerSelectionChanged(object sender, IntPtr e)
        {
            broadcaster.SendToChannel("SelectionOfExplorerWindowChanged", e.ToString());
        }

        private void Observer_ExplorerWindowGotFocus(object sender, IntPtr e)
        {
            broadcaster.SendToChannel("ExplorerWindowGotFocus", e.ToString());
        }
    }
}
