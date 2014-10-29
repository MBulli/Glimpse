using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.ViewModels.Previews
{
    class TextPreviewModel : PropertyChangedBase, IPreviewModel
    {
        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; OnPropertyChanged(); }
        }

        public bool CanCreatePreview(Models.GlimpseItem item)
        {
            return item.PerceivedType == Models.PerceivedType.Text;
        }

        public void ShowPreview(Models.GlimpseItem item)
        {
            this.Text = System.IO.File.ReadAllText(item.FullPath);
        }

        public System.Windows.Size? PreferredPreviewSize(System.Windows.Size currentSize)
        {
            return null;
        }
    }
}
