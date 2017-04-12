using System;
using System.Collections.Generic;
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

        public MonitorApplicationContext()
        {
            messageClient = new XDMessagingClient();
            broadcaster = messageClient.Broadcasters.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);

            sink = messageClient.Listeners.GetListenerForMode(XDTransportMode.HighPerformanceUI);
            sink.RegisterChannel("ExplorerObserverCommand");

            sink.MessageReceived += Sink_MessageReceived;

            observer = new ExplorerMonitor();
            observer.ExplorerSelectionChanged += Observer_ExplorerSelectionChanged; ;
            observer.ExplorerWindowGotFocus += Observer_ExplorerWindowGotFocus;

            observer.Start();
        }

        private void Sink_MessageReceived(object sender, XDMessageEventArgs e)
        {
            if (e.DataGram.Channel == "ExplorerObserverCommand")
            {
                if (e.DataGram.Message == "shutdown")
                {
                    observer.Stop();
                    Application.Exit();
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
