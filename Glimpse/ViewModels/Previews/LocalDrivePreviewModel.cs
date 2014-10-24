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
        private int driveFreeSpace;

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

        public int DriveFreeSpace
        {
            get { return driveFreeSpace; }
            set { driveFreeSpace = value; OnPropertyChanged(); }
        }

        public bool CanCreatePreview(string filename)
        {
            throw new NotImplementedException();
        }

        public System.Windows.FrameworkElement GetPreview(string filename)
        {
            Views.LocalDriveView view = new Views.LocalDriveView();
            view.DataContext = this;

            return view;
        }

        public System.Windows.Size PreferredPreviewSize(System.Windows.Size currentSize)
        {
            throw new NotImplementedException();
        }
    }
}
