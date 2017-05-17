using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glimpse.ExplorerMonitor
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();

            this.Text = $"Glimpse.ExplorerMonitor ({Process.GetCurrentProcess().Id})";

            Debug.Listeners.Add(new TextboxTraceListener(logTextBox));
        }
    }

    class TextboxTraceListener : TraceListener
    {
        private TextBox textbox;

        public TextboxTraceListener(TextBox tb)
        {
            textbox = tb ?? throw new ArgumentNullException(nameof(tb));
        }

        public override void Write(string message)
        {
            if (textbox.InvokeRequired)
            {
                textbox.Invoke(new Action(() => WriteLine(message)));
            }
            else
            {
                textbox.AppendText(message);
            }
        }

        public override void WriteLine(string message)
        {
            if (textbox.InvokeRequired)
            {
                textbox.Invoke(new Action(() => WriteLine(message)));
            }
            else
            {
                textbox.AppendText($"{DateTime.Now.ToString()}: {message} {Environment.NewLine}");
            }
        }
    }
}
