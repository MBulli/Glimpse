using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Markdig;

namespace Glimpse.ViewModels.Previews
{
    class MarkdownPreviewModel : PropertyChangedBase, IPreviewModel
    {
        private string markdownHtml;
        public string MarkdownHtml
        {
            get { return markdownHtml; }
            set { markdownHtml = value; OnPropertyChanged(); }
        }

        public bool CanCreatePreview(Models.GlimpseItem item)
        {
            return item.FileExtension == ".md";
        }

        public void ShowPreview(Models.GlimpseItem item)
        {
            var source = System.IO.File.ReadAllText(item.FullPath);

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var content = Markdown.ToHtml(source, pipeline);
            var template = Properties.Resources.MarkdownHtmlTemplate;

            MarkdownHtml = template.Replace("<$CONTENT-PLACEHOLDER$ />", content);
        }

        public System.Windows.Size? PreferredPreviewSize(System.Windows.Size currentSize)
        {
            return new System.Windows.Size(980, 600);
        }
    }
}
