using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XDMessaging;

namespace Glimpse.ExplorerMonitor
{
    public class ExplorerMonitorAdapter
    {
        readonly SynchronizationContext synchronizationContext;
        readonly XDMessagingClient messageClient;
        readonly IXDListener sink;
        readonly IXDBroadcaster broadcaster;

        public event EventHandler<ExplorerMonitorEventArgs> ExplorerWindowGotFocus;
        public event EventHandler<ExplorerMonitorEventArgs> ExplorerSelectionChanged;

        public ExplorerMonitorAdapter(SynchronizationContext synchronizationContext)
        {
            this.synchronizationContext = synchronizationContext ?? throw new ArgumentNullException(nameof(synchronizationContext));

            messageClient = new XDMessagingClient();
            broadcaster = messageClient.Broadcasters.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);

            sink = messageClient.Listeners.GetListenerForMode(XDTransportMode.HighPerformanceUI);
            sink.RegisterChannel("SelectionOfExplorerWindowChanged");

            sink.MessageReceived += Sink_MessageReceived;
        }

        private void Sink_MessageReceived(object sender, XDMessageEventArgs e)
        {
            // MessageReceived is called in the context of SendMessage() of the other process.
            // We leave the current callstack to let SendMessage() return.
            // Otherwise ExplorerAdapter's COM throws a RPC_E_CANTCALLOUT_ININPUTSYNCCALL error.
            synchronizationContext.Post(arg => OnMessage((XDMessageEventArgs)arg), e);
        }

        private void OnMessage(XDMessageEventArgs msg)
        {
            if (msg.DataGram.Channel == "SelectionOfExplorerWindowChanged")
            {
                if (int.TryParse(msg.DataGram.Message, out int hwndInteger))
                {
                    IntPtr hwnd = new IntPtr(hwndInteger);
                    ExplorerSelectionChanged?.Invoke(this, new ExplorerMonitorEventArgs(hwnd));
                }
            }
            else if (msg.DataGram.Channel == "ExplorerWindowGotFocus")
            {
                if (int.TryParse(msg.DataGram.Message, out int hwndInteger))
                {
                    IntPtr hwnd = new IntPtr(hwndInteger);
                    ExplorerWindowGotFocus?.Invoke(this, new ExplorerMonitorEventArgs(hwnd));
                }
            }
        }

        public void StartMonitor()
        {
            Process.Start("Glimpse.ExplorerMonitor.exe", $"/parent:{Process.GetCurrentProcess().Id.ToString()} /debugform");
        }

        public void StopMonitor()
        {
            broadcaster.SendToChannel("ExplorerObserverCommand", "shutdown");
        }
    }

    public class ExplorerMonitorEventArgs : EventArgs
    {
        public readonly IntPtr ExplorerWindowHandle;

        public ExplorerMonitorEventArgs(IntPtr hwnd)
        {
            ExplorerWindowHandle = hwnd;
        }
    }
}
