using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse
{
    interface IGlimpseView
    {
        bool CanCreatePreview(string filename);
        System.Windows.Forms.Control GetPreview(string filename);
        System.Drawing.Size PreferredPreviewSize(System.Drawing.Size currentSize);
    }
}
