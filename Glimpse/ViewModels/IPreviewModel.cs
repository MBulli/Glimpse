using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.ViewModels
{
    public interface IPreviewModel
    {
        // Multi file selection: Image ThumbnailImage(string filename);
        // void WillDismissPreview(DismissReason[App Quit/selected other file])

        bool CanCreatePreview(string filename);
        void ShowPreview(string filename);

        System.Windows.Size? PreferredPreviewSize(System.Windows.Size currentSize);
    }
}
