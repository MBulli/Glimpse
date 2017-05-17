using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.ViewModels.Previews
{
    class HtmlPreviewModel : PropertyChangedBase, IPreviewModel
    {
        private string htmlText;
        public string MarkdownHtml
        {
            get { return htmlText; }
            set { htmlText = value; OnPropertyChanged(); }
        }

        public bool CanCreatePreview(Models.GlimpseItem item)
        {
            return item.FileExtension == ".htm" 
                || item.FileExtension == ".html" 
                || item.ShellFile?.Properties.System.ContentType?.Value == "text/html";
        }

        public void ShowPreview(Models.GlimpseItem item)
        {
            this.MarkdownHtml = System.IO.File.ReadAllText(item.FullPath);
        }

        public System.Windows.Size? PreferredPreviewSize(System.Windows.Size currentSize)
        {
            return null;
        }
    }
}
