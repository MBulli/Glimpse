using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.Views
{
    class DefaultView : IGlimpseView
    {
        Controls.GenericFileControl filePreview;

        public bool CanCreatePreview(string filename)
        {
            return true;
        }

        public System.Windows.Forms.Control GetPreview(string filename)
        {
            if (filePreview == null)
            {
                filePreview = new Controls.GenericFileControl();
                filePreview.AutoSize = true;
                filePreview.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            }

            filePreview.SetFileToShow(filename);
            return filePreview;
        }

        public System.Drawing.Size PreferredPreviewSize(System.Drawing.Size currentSize)
        {
            return filePreview.Size;
        }
    }
}
