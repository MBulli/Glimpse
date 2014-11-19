using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.ViewModels.Previews
{
    class WindowsPreviewModel : PropertyChangedBase, IPreviewModel
    {
        private string source;
        public string Source
        {
            get { return source; }
            set { source = value; OnPropertyChanged(); }
        }

        public bool CanCreatePreview(Models.GlimpseItem item)
        {
            return Interop.Win32.PreviewHandlerGuid(item.FullPath) != Guid.Empty;
        }

        public void ShowPreview(Models.GlimpseItem item)
        {
            this.Source = item.FullPath;
        }

        public System.Windows.Size? PreferredPreviewSize(System.Windows.Size currentSize)
        {
            return null;
        }
    }
}
