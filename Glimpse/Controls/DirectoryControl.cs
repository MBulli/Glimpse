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
using System.Threading;

namespace Glimpse.Controls
{
    public partial class DirectoryControl : UserControl
    {
        private Dictionary<string, FolderStats> folderStatsCache;
        private FolderStats currentFolderStats;

        public DirectoryControl()
        {
            InitializeComponent();

            folderStatsCache = new Dictionary<string, FolderStats>();
        }

        public void SetDirectoryToShow(string directory)
        {
            using (var sf = ShellFolder.FromParsingName(directory))
            {
                var bmp = sf.Thumbnail.ExtraLargeBitmap;
                bmp.MakeTransparent(Color.Black);
                this.pictureBoxIcon.Image = bmp;

                this.labelFolderName.Text = sf.GetDisplayName(DisplayNameType.Default);
            }

            DirectoryInfo di = new DirectoryInfo(directory);
            
            this.labelCreated.Text = string.Format("{0:F}", di.CreationTime);

            if (folderStatsCache.ContainsKey(directory))
            {
                this.currentFolderStats = folderStatsCache[directory];
            }
            else
            {
                currentFolderStats = new FolderStats(di);
                currentFolderStats.StartCalculation();
                timerFolderStats.Start();

                this.folderStatsCache.Add(directory, currentFolderStats);
            }

            UpdateFolderStatsLabels();
        }

        private void UpdateFolderStatsLabels()
        {
            this.labelFileCount.Text = string.Format("{0:#,#} Files, {1:#,#} Folders", currentFolderStats.FileCount, currentFolderStats.FolderCount);
            this.labelSize.Text = string.Format("{0} ({1:#,#} Bytes)", Helper.FormattedByteSize(currentFolderStats.TotalSize), currentFolderStats.TotalSize);
        }

        private void timerFolderStats_Tick(object sender, EventArgs e)
        {
            UpdateFolderStatsLabels();

            if (currentFolderStats.Completed)
            {
                this.timerFolderStats.Stop();
            }
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
