using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using DirectoryInfo = System.IO.DirectoryInfo;

namespace Glimpse.ViewModels.Previews
{
    class DirectoryPreviewModel : PropertyChangedBase, IPreviewModel
    {
        private static Dictionary<string, FolderStats> folderStatsCache = new Dictionary<string,FolderStats>();
        private FolderStats currentFolderStats;
        private Timer folderStatsUpdateTimer;

        private BitmapSource thumbnail;
        public BitmapSource Thumbnail
        {
            get { return thumbnail; }
            set { thumbnail = value; OnPropertyChanged(); }
        }      

        private string foldername;
        public string Foldername
        {
            get { return foldername; }
            set { foldername = value; OnPropertyChanged(); }
        }

        private DateTime creationTime;
        public DateTime CreationTime
        {
            get { return creationTime; }
            set { creationTime = value; OnPropertyChanged(); }
        }

        private long fileCount;
        public long FileCount
        {
            get { return fileCount; }
            set { fileCount = value; OnPropertyChanged(); }
        }

        private long folderCount;
        public long FolderCount
        {
            get { return folderCount; }
            set { folderCount = value; OnPropertyChanged(); }
        }       

        private long totalSize;
        public long TotalSize
        {
            get { return totalSize; }
            set { totalSize = value; OnPropertyChanged(); }
        }

        public bool CanCreatePreview(Models.GlimpseItem item)
        {
            return item.IsDirectory;
        }

        public void ShowPreview(Models.GlimpseItem item)
        {
            using (var sf = ShellFolder.FromParsingName(item.FullPath))
            {
                this.Thumbnail = sf.Thumbnail.ExtraLargeBitmapSource; ;
                this.Foldername = sf.GetDisplayName(DisplayNameType.Default);
            }

            var di = new DirectoryInfo(item.FullPath);
            this.CreationTime = di.CreationTime;

            if (!folderStatsCache.TryGetValue(item.FullPath, out this.currentFolderStats))
            {
                this.currentFolderStats = new FolderStats(di);
                this.currentFolderStats.StartCalculation();
                this.folderStatsUpdateTimer = new Timer((state) => 
                { 
                    UpdateFolderStats();
                }, state: null, dueTime: 0, period: 500);

                folderStatsCache.Add(item.FullPath, currentFolderStats);                
            }
        }

        public System.Windows.Size? PreferredPreviewSize(System.Windows.Size currentSize)
        {
            return null;
        }

        void UpdateFolderStats()
        {
            this.TotalSize = currentFolderStats.TotalSize;
            this.FileCount = currentFolderStats.FileCount;
            this.FolderCount = currentFolderStats.FolderCount;

            if (currentFolderStats.Completed)
            {
                folderStatsUpdateTimer.Dispose();
            }
        }


        class FolderStats
        {
            private DirectoryInfo root;
            private CancellationTokenSource cancellationTokenSource;

            public long TotalSize { get; private set; }
            public long FileCount { get; private set; }
            public long FolderCount { get; private set; }
            public bool Completed { get; private set; }

            public FolderStats(DirectoryInfo rootFolder)
            {
                if (rootFolder == null)
                    throw new ArgumentNullException("rootFolder");

                this.root = rootFolder;
            }

            public void StartCalculation()
            {
                if (cancellationTokenSource != null)
                    return;

                cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;

                Task.Run(() => Calculate(), token)
                    .ContinueWith((task) =>
                    {
                        cancellationTokenSource.Dispose();
                        cancellationTokenSource = null;
                    });
            }

            public void Cancel()
            {
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Cancel();
                }
            }

            private void Calculate()
            {
                Stack<DirectoryInfo> stack = new Stack<DirectoryInfo>();
                stack.Push(root);

                while (stack.Count != 0)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    DirectoryInfo dir = stack.Pop();

                    foreach (var file in dir.GetFiles())
                    {
                        FileCount++;
                        TotalSize += file.Length;
                    }

                    foreach (var child in dir.GetDirectories())
                    {
                        FolderCount++;
                        stack.Push(child);
                    }
                }

                Completed = true;
            }
        }
    }
}
