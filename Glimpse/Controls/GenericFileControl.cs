using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell;
using System.IO;

namespace Glimpse.Controls
{
    public partial class GenericFileControl : UserControl
    {
        public GenericFileControl()
        {
            InitializeComponent();
        }

        public void SetFileToShow(string filename)
        {
            using (var sf = ShellFolder.FromParsingName(filename))
            {
                var bmp = sf.Thumbnail.ExtraLargeBitmap;
                bmp.MakeTransparent(Color.Black);
                this.pictureBoxIcon.Image = bmp;

                this.labelFileName.Text = sf.GetDisplayName(DisplayNameType.Default);
            }

            FileInfo fi = new FileInfo(filename);
            this.labelSize.Text = Helper.FormattedByteSize(fi.Length);
            this.labelCreated.Text = fi.CreationTime.ToString("F");
            this.labelModified.Text = fi.LastWriteTime.ToString("F");
        }
    }
}
