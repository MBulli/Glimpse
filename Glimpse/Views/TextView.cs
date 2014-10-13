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
    class TextView : IGlimpseView
    {
        public bool CanCreatePreview(string filename)
        {
            return Path.GetExtension(filename) == ".rtf"
                   || Glimpse.Interop.Win32.GetPerceivedType(filename) == Interop.Win32.PerceivedType.Text;
        }

        public Control GetPreview(string filename)
        {
            RichTextBox textbox = new RichTextBox();
            textbox.ReadOnly = true;
            textbox.BorderStyle = BorderStyle.None;
            textbox.BackColor = Color.White;

            bool isRtf = (Path.GetExtension(filename) == ".rtf");
            string text = File.ReadAllText(filename);

            if (isRtf)
            {
                textbox.Rtf = text;
            }
            else
            {
                textbox.Font = GetMonospaceFont();
                textbox.Text = text;
            }

            return textbox;
        }

        public System.Drawing.Size PreferredPreviewSize(System.Drawing.Size currentSize)
        {
            return currentSize;
        }

        private Font GetMonospaceFont()
        {
            const float fontSize = 10;
            var fonts = new System.Drawing.Text.InstalledFontCollection();

            if (fonts.Families.Any((ff) => ff.Name == "Consolas"))
                return new Font("Consolas", fontSize);
            else
                return new Font(FontFamily.GenericMonospace, fontSize);
        }
    }
}
