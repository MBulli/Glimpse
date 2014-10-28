using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Glimpse.ViewModels.Previews
{
    class ImagePreviewModel : PropertyChangedBase, IPreviewModel
    {
        private static readonly string[] supportedExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff" };

        private string source;
        public string Source
        {
            get { return source; }
            set { source = value; OnPropertyChanged(); }
        }

        public bool CanCreatePreview(string filename)
        {
            var ext = Path.GetExtension(filename).ToLower();
            return supportedExtensions.Contains(ext);
        }

        public void ShowPreview(string filename)
        {
            this.Source = filename;
        }

        public System.Windows.Size PreferredPreviewSize(System.Windows.Size currentSize)
        {
            throw new NotImplementedException();
        }
    }
}
