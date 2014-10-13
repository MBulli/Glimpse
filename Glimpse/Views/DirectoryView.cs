using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.Views
{
    class DirectoryView : IGlimpseView
    {
        Controls.DirectoryControl dirPreview;

        public bool CanCreatePreview(string filename)
        {
            if (Directory.GetLogicalDrives().Contains(filename))
                return false;
            else
                return Directory.Exists(filename);
        }

        public System.Windows.Forms.Control GetPreview(string filename)
        {
            if (dirPreview == null)
            {
                dirPreview = new Controls.DirectoryControl();
                dirPreview.AutoSize = true;
                dirPreview.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            }

            dirPreview.SetDirectoryToShow(filename);
            return dirPreview;
        }

        public System.Drawing.Size PreferredPreviewSize(System.Drawing.Size currentSize)
        {
            return dirPreview.Size;
        }
    }
}
