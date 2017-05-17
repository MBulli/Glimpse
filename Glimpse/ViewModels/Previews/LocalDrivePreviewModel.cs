using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Glimpse.ViewModels.Previews
{
    class LocalDrivePreviewModel : PropertyChangedBase, IPreviewModel
    {
        private BitmapSource thumbnail;
        public BitmapSource Thumbnail
        {
            get { return thumbnail; }
            set { thumbnail = value; OnPropertyChanged(); }
        }
        
        private string driveName;
        public string DriveName
        {
            get { return driveName; }
            set { driveName = value; OnPropertyChanged(); }
        }

        private long freeBytes;
        public long FreeBytes
        {
            get { return freeBytes; }
            set { freeBytes = value; OnPropertyChanged(); }
        }

        private long totalBytes;
        public long TotalBytes
        {
            get { return totalBytes; }
            set { totalBytes = value; OnPropertyChanged(); }
        }
        
        private double freeSpaceRatio;
        public double FreeSpaceRatio
        {
            get { return freeSpaceRatio; }
            set { freeSpaceRatio = value; OnPropertyChanged(); }
        }
        
        public bool CanCreatePreview(Models.GlimpseItem item)
        {
            return item.IsLocalDrive;
        }

        public void ShowPreview(Models.GlimpseItem item)
        {
            using (var sf = ShellFolder.FromParsingName(item.FullPath))
            {
                this.Thumbnail = sf.Thumbnail.ExtraLargeBitmapSource;
                this.DriveName = sf.GetDisplayName(DisplayNameType.Default);
            }

            DriveInfo di = new DriveInfo(item.FullPath);
            this.FreeBytes = di.TotalFreeSpace;
            this.TotalBytes = di.TotalSize;

            this.FreeSpaceRatio = (di.TotalFreeSpace / (double)di.TotalSize);
        }

        public System.Windows.Size? PreferredPreviewSize(System.Windows.Size currentSize)
        {
            return new System.Windows.Size(400, 160);
        }
    }
}
