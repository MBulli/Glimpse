using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glimpse.Views
{
    class ImageView : IGlimpseView
    {
        private static readonly string[] supportedExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff" };

        private PictureBox pictureBox;

        public bool CanCreatePreview(string filename)
        {
            var ext = Path.GetExtension(filename).ToLower();
            return supportedExtensions.Contains(ext);
        }

        public Control GetPreview(string filename)
        {
            if (pictureBox == null)
            {
                pictureBox = new PictureBox();
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }

            pictureBox.Image = Image.FromFile(filename); ;

            return pictureBox;
        }

        public System.Drawing.Size PreferredPreviewSize(System.Drawing.Size currentSize)
        {
            return pictureBox.Image.Size;
        }
    }
}
