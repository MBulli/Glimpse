using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Shell;

namespace Glimpse.Controls
{
    public partial class LocalDriveControl : UserControl
    {
        public LocalDriveControl()
        {
            InitializeComponent();
        }

        public void SetLocalDriveToShow(string driveName)
        {
            using (var sf = ShellFolder.FromParsingName(driveName))
            {
                var bmp = sf.Thumbnail.ExtraLargeBitmap;
                bmp.MakeTransparent(Color.Black);
                this.pictureBoxIcon.Image = bmp;

                this.labelDriveName.Text = sf.GetDisplayName(DisplayNameType.Default);
            }

            DriveInfo di = new DriveInfo(driveName);
            this.labelFreeSpace.Text = string.Format("{0} free of {1}", 
                                Helper.FormattedByteSize(di.TotalFreeSpace),
                                Helper.FormattedByteSize(di.TotalSize, showDecimalPlaces: false));

            float relativeFreeSpace = (di.TotalFreeSpace / (float)di.TotalSize) * 100;
            this.progressBarFreeSpace.Value = (int)(100 - relativeFreeSpace);
        }
    }
}
