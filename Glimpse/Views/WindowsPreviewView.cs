using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.Views
{
    class WindowsPreviewView : IGlimpseView
    {
        public bool CanCreatePreview(string filename)
        {
            return Glimpse.Interop.Win32.PreviewHandlerGuid(filename) != Guid.Empty;
        }

        public System.Windows.Forms.Control GetPreview(string filename)
        {
            Controls.PreviewHandlerHostControl ctrl = new Controls.PreviewHandlerHostControl();
            ctrl.Open(filename);
            return ctrl;
        }

        public System.Drawing.Size PreferredPreviewSize(System.Drawing.Size currentSize)
        {
            return currentSize;
        }
    }
}
