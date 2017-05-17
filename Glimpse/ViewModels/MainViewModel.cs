using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Glimpse.Models;
using Glimpse.ExplorerMonitor;

using Rect = System.Windows.Rect;
using Size = System.Windows.Size;
using Point = System.Windows.Point;
using Application = System.Windows.Application;

namespace Glimpse.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private List<IPreviewModel> previews;
        private ExplorerAdapter explorer;
        private ExplorerMonitorAdapter explorerMonitor;

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

            this.explorer = new ExplorerAdapter();
            this.explorerMonitor = new ExplorerMonitorAdapter(System.Threading.SynchronizationContext.Current);
            this.explorerMonitor.ExplorerWindowGotFocus += OnExplorerWindowGotFocus;
            this.explorerMonitor.ExplorerSelectionChanged += OnExplorerSelectionChanged;
            
            if (!IsDesignMode())
            {
                // If we're running inside a designer we don't want to track explorer selection.
                // Otherwise WPF Designer might crash or cause deadlocks
                try
                {
                    this.explorerMonitor.StartMonitor();
                }
                catch(Exception ex)
                {
                    System.Windows.MessageBox.Show($"Failed to start Explorer Monitor with error: {ex}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }

            this.ErrorMessage = "Nothing to preview";

            previews = new List<IPreviewModel>();

            previews.Add(new Previews.ImagePreviewModel());
            previews.Add(new Previews.TextPreviewModel());
            previews.Add(new Previews.RtfPreviewModel());
            previews.Add(new Previews.VideoPreviewModel());
            previews.Add(new Previews.MarkdownPreviewModel());
            previews.Add(new Previews.HtmlPreviewModel());
            previews.Add(new Previews.DirectoryPreviewModel());
            previews.Add(new Previews.LocalDrivePreviewModel());

            // add new views above these two
            previews.Add(new Previews.WindowsPreviewModel());  // slow and painful, so our last resort
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
                // TODO multiselect
                DisplayFile(fileToPreview);
            }
        }

        public void ShowPreview(IntPtr hWnd)
        {
            ResetErrorState();
            string fileToPreview = null;

            if (GetExplorerSelectedItemPath(hWnd, ref fileToPreview))
            {
                // TODO multiselect
                DisplayFile(fileToPreview);
            }
        }


        private void ResetErrorState()
        {
            this.ErrorMessage = null;
        }

        private void SingleInstanceApplication_CommandReceived(object sender, string[] e)
        {
            MainDispatcher.InvokeAsync(() => {
                ShowPreview(e);
            });
        }

        private void OnExplorerWindowGotFocus(object sender, ExplorerMonitorEventArgs e)
        {
            MainDispatcher.InvokeAsync(() => {
                ShowPreview(e.ExplorerWindowHandle);
            });
        }

        private void OnExplorerSelectionChanged(object sender, ExplorerMonitorEventArgs e)
        {
            MainDispatcher.InvokeAsync(() => {
                ShowPreview(e.ExplorerWindowHandle);
            });
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
                // TODO multiselect
                fileToPreview = args[0];
                return true;
            }
        }

        private bool GetExplorerSelectedItemPath(IntPtr hwnd, ref string selectedItem)
        {
            try
            {
                string[] items = explorer.GetSelectedItems(hwnd);

                if (items == null || items.Length == 0)
                {
                    this.ErrorMessage = "Nothing to preview";
                    return false;
                }

                // TODO multiselect
                selectedItem = items.First();
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
                    MainDispatcher.InvokeAsync(() =>
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

        /// <summary>
        /// Sets the window size but ensures that the window is always fully visible (within screen bounds)
        /// If prefferedSize is bigger than the screen bounds, prefferedSize is scaled to fit the screen keeping the aspect ratio
        /// </summary>
        /// <param name="prefferedSize"></param>
        private void SetPrefferedPreviewSize(Size prefferedSize)
        {
            var screen = Screen.FromMainWindow();
            var maxBounds = Rect.Inflate(screen.WorkingArea, -100, -100);

            Size wndSize = prefferedSize;

            // scale the prefferedSize while keeping the aspect ratio
            double ratio = Math.Min(maxBounds.Width / prefferedSize.Width,
                                    maxBounds.Height / prefferedSize.Height);

            // but only scale if prefferedSize is bigger than the screen bounds
            if (ratio < 1.0)
            {
                int w = (int)Math.Floor(prefferedSize.Width * ratio);
                int h = (int)Math.Floor(prefferedSize.Height * ratio);

                wndSize = new Size(w, h);                
            }

            // ensure visible
            Rect wndBounds = Application.Current.MainWindow.GetBounds();

            double xOffset = Math.Min(0.0, maxBounds.Right - (wndBounds.X + wndSize.Width));
            double yOffset = Math.Min(0.0, maxBounds.Bottom - (wndBounds.Y + wndSize.Height));

            // We break MVVM on purpose because binding these values doesn't work well
            Application.Current.MainWindow.Left = wndBounds.X + xOffset;
            Application.Current.MainWindow.Top = wndBounds.Y + yOffset;
            Application.Current.MainWindow.SetClientSize(wndSize);        
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

        private static bool IsDesignMode()
        {
            // see: http://msdn.microsoft.com/en-us/library/bb546934.aspx
            return (bool)(System.ComponentModel.DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(System.Windows.DependencyObject)).DefaultValue);
        }
    }
}
