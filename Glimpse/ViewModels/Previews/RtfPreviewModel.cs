using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.ViewModels.Previews
{
    class RtfPreviewModel : PropertyChangedBase, IPreviewModel
    {
        private string rtfText;
        public string RtfText
        {
            get { return rtfText; }
            set { rtfText = value; OnPropertyChanged(); }
        }

        public bool CanCreatePreview(Models.GlimpseItem item)
        {
            return item.FileExtension == ".rtf";
        }

        public void ShowPreview(Models.GlimpseItem item)
        {
            this.RtfText = System.IO.File.ReadAllText(item.FullPath);
        }

        public System.Windows.Size? PreferredPreviewSize(System.Windows.Size currentSize)
        {
            return null;
        }
    }
}
