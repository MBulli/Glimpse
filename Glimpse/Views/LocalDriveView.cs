using Glimpse.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.Views
{
    class LocalDriveView : IGlimpseView
    {
        Controls.LocalDriveControl drivePreview;

        public bool CanCreatePreview(string filename)
        {
            return Directory.GetLogicalDrives().Contains(filename);
        }

        public System.Windows.Forms.Control GetPreview(string filename)
        {
            if (drivePreview == null)
            {
                drivePreview = new LocalDriveControl();
                drivePreview.AutoSize = true;
                drivePreview.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            }

            drivePreview.SetLocalDriveToShow(filename);
            return drivePreview;
        }

        public System.Drawing.Size PreferredPreviewSize(System.Drawing.Size currentSize)
        {
            return drivePreview.Size;
        }
    }
}
