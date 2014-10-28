using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.ViewModels.Previews
{
    class LocalDrivePreviewModel : PropertyChangedBase, IPreviewModel
    {
        private string driveName;
        private string driveSpace;
        private double driveFreeSpace;

        public string DriveName
        {
            get { return driveName; }
            set { driveName = value; OnPropertyChanged(); }
        }

        public string DriveSpace
        {
            get { return driveSpace; }
            set { driveSpace = value; OnPropertyChanged(); }
        }

        public double DriveFreeSpace
        {
            get { return driveFreeSpace; }
            set { driveFreeSpace = value; OnPropertyChanged(); }
        }

        public bool CanCreatePreview(string filename)
        {
            throw new NotImplementedException();
        }

        public void ShowPreview(string filename)
        {
            this.DriveName = filename;
        }

        public System.Windows.Size PreferredPreviewSize(System.Windows.Size currentSize)
        {
            throw new NotImplementedException();
        }
    }
}
