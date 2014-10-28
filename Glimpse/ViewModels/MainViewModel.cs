using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Size = System.Windows.Size;

namespace Glimpse.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private List<IPreviewModel> previews;

        private double windowHeight;
        public double WindowHeight
        {
            get { return windowHeight; }
            set { windowHeight = value; OnPropertyChanged(); }
        }

        private double windowWidth;
        public double WindowWidth
        {
            get { return windowWidth; }
            set { windowWidth = value; OnPropertyChanged(); }
        }

        private IPreviewModel currentPreviewModel;
        public IPreviewModel CurrentPreviewModel
        {
            get { return currentPreviewModel; }
            set { currentPreviewModel = value; OnPropertyChanged(); }
        }

        private RelayCommand applicationExitCommand;
        public ICommand ApplicationExitCommand
        {
            get
            {
                if (applicationExitCommand == null)
                {
                    applicationExitCommand = new RelayCommand(() => 
                    {
                        System.Windows.Application.Current.Shutdown();
                    });
                }
                return applicationExitCommand;
            }
        }

        private RelayCommand showPreviewCommand;
        public ICommand ShowPreviewCommand
        {
            get
            {
                if (showPreviewCommand == null)
                {
                    showPreviewCommand = new RelayCommand(() => ShowPreview());
                }
                return showPreviewCommand;
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            previews = new List<IPreviewModel>();

            previews.Add(new Previews.ImagePreviewModel());
            //previews.Add(new Previews.TextPreviewModel());
            //previews.Add(new Previews.DirectoryPreviewModel());
            previews.Add(new Previews.LocalDrivePreviewModel());

            // add new views above these two
            //previews.Add(new Previews.WindowsPreviewModel());  // slow and painful, so our last resort
            previews.Add(new Previews.DefaultPreviewModel()); // thats our fallback which will always display something
        }


        public void ShowPreview()
        {
            ShowPreview(Environment.GetCommandLineArgs());
        }

        public void ShowPreview(string[] args)
        {
            
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
                    this.ErrorMessage = "Invalid cmd parameter";
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
                this.ErrorMessage = "Failed to access explorer with error:\n" + ex.ToString();
            }

            return file;
        }

        private void DisplayFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            if (!FileSystemItemExist(path))
                return;

            foreach (var view in this.previews)
            {
                if (view.CanCreatePreview(path))
                {
                    this.CurrentPreviewModel = view;
                    view.ShowPreview(path);

                    // 5. Ask for preffered Size and set it
                    //Size prefferedSize = view.PreferredPreviewSize(this.viewContainer.ClientSize);
                    //SetPrefferedPreviewSize(prefferedSize);
                    break;
                }
            }
        }

        private void SetPrefferedPreviewSize(Size prefferedSize)
        {
            //var screen = Screen.FromControl(this);
            //var maxSize = Rectangle.Inflate(screen.Bounds, -100, -100).Size;

            //Size wndSize = prefferedSize;

            //if (prefferedSize.Width > maxSize.Width)
            //{
            //    float ratio = maxSize.Width / (float)prefferedSize.Width;

            //    int w = (int)Math.Floor(prefferedSize.Width * ratio);
            //    int h = (int)Math.Floor(prefferedSize.Height * ratio);

            //    wndSize = new Size(w, h);
            //}
            //else if (prefferedSize.Height > maxSize.Height)
            //{
            //    float ratio = maxSize.Height / (float)prefferedSize.Height;

            //    int w = (int)Math.Floor(prefferedSize.Width * ratio);
            //    int h = (int)Math.Floor(prefferedSize.Height * ratio);

            //    wndSize = new Size(w, h);
            //}

            //this.ClientSize = new Size(wndSize.Width + this.Padding.Horizontal,
            //                           wndSize.Height + this.Padding.Vertical);

            //// ensure visible
            //int xOffset = Math.Max(0, this.Bounds.Right - screen.Bounds.Width);
            //int yOffset = Math.Max(0, this.Bounds.Bottom - screen.Bounds.Height);
            //this.Location = Point.Subtract(this.Location, new Size(xOffset, yOffset));
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
    }
}
