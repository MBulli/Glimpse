using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.ViewModels.Previews
{
    class RtfPreviewModel : PropertyChangedBase, IPreviewModel
    {
        public bool CanCreatePreview(Models.GlimpseItem item)
        {
            return item.FileExtension == ".rtf";
        }

        public void ShowPreview(Models.GlimpseItem item)
        {
            throw new NotImplementedException();
        }

        public System.Windows.Size? PreferredPreviewSize(System.Windows.Size currentSize)
        {
            throw new NotImplementedException();
        }
    }
}
