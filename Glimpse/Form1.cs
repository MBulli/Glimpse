using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glimpse
{
    public partial class Form1 : Form
    {
        private bool once = false;
        private List<IGlimpseView> views = new List<IGlimpseView>();

        public string TargetWindow { get; set; }
        public IntPtr TargetExplorerWindow { get; set; }

        public string FileToPreview { get; set; }

        public Form1()
        {
            InitializeComponent();

#if DEBUG
            this.TopMost = false;
#endif

            views.Add(new Views.ImageView());
            views.Add(new Views.TextView());
            views.Add(new Views.DirectoryView());
            views.Add(new Views.LocalDriveView());

            // add new views above these two
            views.Add(new Views.WindowsPreviewView()); // slow and painful, so our last resort
            views.Add(new Views.DefaultView()); // thats our fallback which will always display something
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (once)
                return;
            else
                once = true;

            DisplayFile(this.FileToPreview);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyData == Keys.Escape)
            {
                Application.Exit();
            }
        }

        public void LaunchWithArguments(string[] args)
        {
            this.FileToPreview = PreviewFileFromCommandLine(args);
        }

        /// <summary>
        /// Updates the preview with the specified file in the commandline arguments.
        /// It's safe to call this method from thread other the main thread
        /// </summary>
        /// <param name="args"></param>
        public void DisplayPreview(string[] args)
        {
            if (this.InvokeRequired)
            {
                Action action = () => { DisplayPreview(args); };
                this.Invoke(action);
            }
            else
            {
                this.FileToPreview = PreviewFileFromCommandLine(args);
                DisplayFile(this.FileToPreview);
            }
        }

        private string PreviewFileFromCommandLine(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Length == 0)
                return null;

            if (args[0].StartsWith("0x"))
            {
                try
                {
                    long target = Convert.ToInt64(args[0], 16);
                    IntPtr hwnd = new IntPtr(target);

                    return GetExplorerSelectedItemPath(hwnd);
                }
                catch
                {
                    MessageBox.Show("Invalid cmd parameter");
                    return null;
                }
            }
            else
            {
                return args[0];
            }
        }

        private string GetExplorerSelectedItemPath(IntPtr hwnd)
        {
            string file = null;
            try
            {
                file = ExplorerAdapter.GetSelectedItem(hwnd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to access explorer with error:\n" + ex.ToString());
            }

            return file;
        }

        private void DisplayFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            if (!FileSystemItemExist(path))
                return;

            this.viewContainer.Controls.Clear();
            foreach (var view in this.views)
            {
                if (view.CanCreatePreview(path))
                {
                    // 1. Let view create/update its control
                    Control ctrl = view.GetPreview(path);
                    
                    // 2. Remove margin (Form has padding)
                    ctrl.Margin = new Padding(0);
                    // 3. Remove dock so control can use AutoSize
                    ctrl.Dock = DockStyle.None;
                    // 4. Add to Control hirachy otherwise AutoSize won't trigger
                    this.viewContainer.Controls.Add(ctrl);

                    // 5. Ask for preffered Size and set it
                    Size prefferedSize = view.PreferredPreviewSize(this.viewContainer.ClientSize);
                    SetPrefferedPreviewSize(prefferedSize);

                    // 6. Now we can dock it to the container
                    ctrl.Dock = DockStyle.Fill;
                  
                    break;
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            System.Diagnostics.Debug.WriteLine("Size: " + this.Size);
        }

        private void SetPrefferedPreviewSize(Size prefferedSize)
        {
            var screen = Screen.FromControl(this);
            var maxSize = Rectangle.Inflate(screen.Bounds, -100, -100).Size;

            Size wndSize = prefferedSize;

            if (prefferedSize.Width > maxSize.Width)
            {
                float ratio = maxSize.Width / (float)prefferedSize.Width;

                int w = (int)Math.Floor(prefferedSize.Width * ratio);
                int h = (int)Math.Floor(prefferedSize.Height * ratio);

                wndSize = new Size(w, h);
            }
            else if (prefferedSize.Height > maxSize.Height)
            {
                float ratio = maxSize.Height / (float)prefferedSize.Height;

                int w = (int)Math.Floor(prefferedSize.Width * ratio);
                int h = (int)Math.Floor(prefferedSize.Height * ratio);

                wndSize = new Size(w, h);
            }

            this.ClientSize = new Size(wndSize.Width + this.Padding.Horizontal,
                                       wndSize.Height + this.Padding.Vertical);

            // ensure visible
            int xOffset = Math.Max(0, this.Bounds.Right - screen.Bounds.Width);
            int yOffset = Math.Max(0, this.Bounds.Bottom - screen.Bounds.Height);
            this.Location = Point.Subtract(this.Location, new Size(xOffset, yOffset));
        }

        private bool FileSystemItemExist(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                return true;
            }

            // First test drive because 'C:\' is a DirectoryInfo too
            // on the other hand 'C:\Users' is also valid with DriveInfo
            bool isDrive = Directory.GetLogicalDrives().Contains(path);
            if (isDrive)
            {
                DriveInfo driveInfo = new DriveInfo(path);
                return driveInfo.IsReady;
            }

            DirectoryInfo dir = new DirectoryInfo(path);
            return dir.Exists;
        }

        [Obsolete]
        private void SetWindowSizeWithImage(Image img)
        {
            if (img == null)
                throw new ArgumentNullException("img");

            var screen = Screen.FromControl(this);
            var maxSize = Rectangle.Inflate(screen.Bounds, -100, -100).Size;

            Size wndSize = img.Size;

            if (img.Width > maxSize.Width)
            {
                float ratio = maxSize.Width / (float)img.Width;

                int w = (int)Math.Floor(img.Width * ratio);
                int h = (int)Math.Floor(img.Height * ratio);

                wndSize = new Size(w, h);
            }
            else if (img.Height > maxSize.Height)
            {
                float ratio = maxSize.Height / (float)img.Height;

                int w = (int)Math.Floor(img.Width * ratio);
                int h = (int)Math.Floor(img.Height * ratio);

                wndSize = new Size(w, h);
            }

            this.Size = wndSize;

            // ensure visible
            int xOffset = Math.Max(0, this.Bounds.Right - screen.Bounds.Width);
            int yOffset = Math.Max(0, this.Bounds.Bottom - screen.Bounds.Height);
            this.Location = Point.Subtract(this.Location, new Size(xOffset, yOffset));
        }
    }
}