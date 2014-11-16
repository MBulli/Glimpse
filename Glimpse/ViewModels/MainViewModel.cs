using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Glimpse.Models;

using Rect = System.Windows.Rect;
using Size = System.Windows.Size;
using Point = System.Windows.Point;
using Application = System.Windows.Application;

namespace Glimpse.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private List<IPreviewModel> previews;

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
                    applicationExitCommand = new RelayCommand(() => Application.Current.Shutdown());

                return applicationExitCommand;
            }
        }

        private RelayCommand showPreviewCommand;
        public ICommand ShowPreviewCommand
        {
            get
            {
                if (showPreviewCommand == null)
                    showPreviewCommand = new RelayCommand(() => ShowPreview());

                return showPreviewCommand;
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; OnPropertyChanged(); }
        }

        private System.Windows.Threading.Dispatcher MainDispatcher
        {
            get { return Application.Current.Dispatcher; }
        }
        
        public MainViewModel()
        {
            // Single instance is started in App.xaml.cs:
            SingleInstanceApplication.CommandReceived += SingleInstanceApplication_CommandReceived;
            this.ErrorMessage = "Nothing to preview";

            previews = new List<IPreviewModel>();

            previews.Add(new Previews.ImagePreviewModel());
            previews.Add(new Previews.TextPreviewModel());
            previews.Add(new Previews.RtfPreviewModel());
            previews.Add(new Previews.DirectoryPreviewModel());
            previews.Add(new Previews.LocalDrivePreviewModel());

            // add new views above these two
            //previews.Add(new Previews.WindowsPreviewModel());  // slow and painful, so our last resort
            previews.Add(new Previews.DefaultPreviewModel()); // thats our fallback which will always display something
        }

        public void ShowPreview()
        {
            // args[0] == exe name
            var args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            ShowPreview(args);
        }

        public void ShowPreview(string[] args)
        {
            ResetErrorState();
            string fileToPreview = null;

            if (GetPreviewFileFromCommandLine(args, ref fileToPreview))
            {
                DisplayFile(fileToPreview);
            }
        }

        private void ResetErrorState()
        {
            this.ErrorMessage = null;
        }

        private void SingleInstanceApplication_CommandReceived(object sender, string[] e)
        {
            ShowPreview(e);
        }

        private bool GetPreviewFileFromCommandLine(string[] args, ref string fileToPreview)
        {
            if (args == null || args.Length == 0)
            {
                this.ErrorMessage = "Nothing to preview";
                return false;
            }

            if (args[0].StartsWith("0x"))
            {
                try
                {
                    long target = Convert.ToInt64(args[0], 16);
                    IntPtr hwnd = new IntPtr(target);

                    return GetExplorerSelectedItemPath(hwnd, ref fileToPreview);
                }
                catch
                {
                    this.ErrorMessage = "Invalid command line parameter";
                    return false;
                }
            }
            else
            {
                fileToPreview = args[0];
                return true;
            }
        }

        private bool GetExplorerSelectedItemPath(IntPtr hwnd, ref string selectedItem)
        {
            try
            {
                selectedItem = ExplorerAdapter.GetSelectedItem(hwnd);
                return true;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = "Failed to access explorer with error:\n" + ex.ToString();
                return false;
            }
        }

        private void DisplayFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            if (!FileSystemItemExist(path))
                return;

            GlimpseItem item = new GlimpseItem(path);

            foreach (var preview in this.previews)
            {
                if (preview.CanCreatePreview(item))
                {
                    // set preview view in main thread
                    MainDispatcher.Invoke(() =>
                        {
                            this.CurrentPreviewModel = preview;
                            preview.ShowPreview(item);

                            // Ask for preffered Size and set it
                            Size wndSize = Application.Current.MainWindow.GetClientSize();
                            Size? prefferedSize = preview.PreferredPreviewSize(wndSize);
                            if (prefferedSize != null)
                            {
                                SetPrefferedPreviewSize(prefferedSize.Value);
                            }
                        });
                    break;
                }
            }
        }

        private void SetPrefferedPreviewSize(Size prefferedSize)
        {
            var screen = Screen.FromMainWindow();
            var maxBounds = Rect.Inflate(screen.WorkingArea, -100, -100);

            Size wndSize = prefferedSize;

            if (prefferedSize.Width > maxBounds.Width)
            {
                double ratio = maxBounds.Width / prefferedSize.Width;

                int w = (int)Math.Floor(prefferedSize.Width * ratio);
                int h = (int)Math.Floor(prefferedSize.Height * ratio);

                wndSize = new Size(w, h);
            }
            else if (prefferedSize.Height > maxBounds.Height)
            {
                double ratio = maxBounds.Height / prefferedSize.Height;

                int w = (int)Math.Floor(prefferedSize.Width * ratio);
                int h = (int)Math.Floor(prefferedSize.Height * ratio);

                wndSize = new Size(w, h);
            }

            // ensure visible
            // (We break MVVM on purpose because binding these values doesn't work well)
            Rect wndBounds = Application.Current.MainWindow.GetBounds();

            double xOffset = Math.Min(0.0, maxBounds.Right - (wndBounds.X + wndSize.Width));
            double yOffset = Math.Min(0.0, maxBounds.Bottom - (wndBounds.Y + wndSize.Height));
            
            Application.Current.MainWindow.SetBounds(new Rect()
            {
                X = wndBounds.X + xOffset,
                Y = wndBounds.Y + yOffset,
                Width = wndSize.Width,
                Height = wndSize.Height
            });
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
