using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse.Models
{
    public class GlimpseItem
    {
        private Lazy<PerceivedType> perceivedType;
        private Lazy<GlimpseItemKind> itemKind;

        /// <summary>
        /// Gets a string representing the item's full path (eg. "C:\Temp\foo.txt")
        /// </summary>
        public string FullPath { get; private set; }
        /// <summary>
        /// Gets a Uri representing the item's full path (eg. "file:///C:/Temp/foo.txt")
        /// </summary>
        public Uri Uri { get; private set; }
        /// <summary>
        /// Gets a string representing the item's filename (eg. "foo.txt")
        /// </summary>
        public string Filename { get; private set; }
        /// <summary>
        /// Gets a string representing the item's filname without the extension (eg. "foo")
        /// </summary>
        public string FilenameWithoutExtension { get; private set; }
        /// <summary>
        /// Gets a lowercase string representing the item's extension (eg. ".txt")
        /// </summary>
        public string FileExtension { get; private set; }
        /// <summary>
        /// Gets a string representing the directory's full path (eg. "C:\Temp")
        /// </summary>
        public string DirectoryName { get; private set; }
        /// <summary>
        /// Gets a value which indicates the item's type (eg. "File")
        /// </summary>
        public GlimpseItemKind Kind { get { return itemKind.Value; } }
        /// <summary>
        /// Returns true if the item exists and is a file
        /// </summary>
        public bool IsFile { get { return Kind == GlimpseItemKind.File; } }
        /// <summary>
        /// Returns true if the item exists and is a directory
        /// </summary>
        public bool IsDirectory { get { return Kind == GlimpseItemKind.Directory; } }
        /// <summary>
        /// Returns true if the item exists and is a local drive
        /// </summary>
        public bool IsLocalDrive { get { return Kind == GlimpseItemKind.LocalDrive; } }
        /// <summary>
        /// 
        /// </summary>
        public bool IsUncPath { get; private set; }

        public PerceivedType PerceivedType { get { return perceivedType.Value; } }

        public GlimpseItem(string path)
        {
            if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException("path");
          
            this.FullPath = path;
            this.Uri = new Uri(path);
            this.Filename = Path.GetFileName(path);
            this.FilenameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            this.FileExtension = Path.GetExtension(path);
            this.DirectoryName = Path.GetDirectoryName(path);

            if (this.FileExtension != null)
                this.FileExtension = FileExtension.ToLower();

            // Lazy load attributes which trigger a file access
            perceivedType = new Lazy<PerceivedType>(LoadPerceivedType);
            itemKind = new Lazy<GlimpseItemKind>(LoadItemKind);
        }

        private PerceivedType LoadPerceivedType()
        {
            return (PerceivedType)Glimpse.Interop.Win32.GetPerceivedType(this.Filename);
        }

        private GlimpseItemKind LoadItemKind()
        {
            if (File.Exists(this.FullPath))
                return GlimpseItemKind.File;

            // First test drive because 'C:\' is a DirectoryInfo too
            // on the other hand 'C:\Users' is also valid with DriveInfo
            if (Directory.GetLogicalDrives().Contains(this.FullPath))
                return GlimpseItemKind.LocalDrive;

            if (Directory.Exists(this.FullPath))
                return GlimpseItemKind.Directory;

            return GlimpseItemKind.Unknown;
        }
    }

    public enum GlimpseItemKind
    {
        Unknown,
        File,
        Directory,
        LocalDrive
    }

    // see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb762520(v=vs.85).aspx
    public enum PerceivedType
    {
        /// <summary>
        /// The file's perceived type as defined in the registry is not a known type.
        /// </summary>
        Custom = -3,
        /// <summary>
        /// The file does not have a perceived type.
        /// </summary>
        Unspecified = -2,
        Folder = -1,
        Unknown = 0,
        Text = 1,
        Image = 2,
        Audio = 3,
        Video = 4,
        Compressed = 5,
        Document = 6,
        System = 7,
        Application = 8,
        Gamemedia = 9,
        Contacts = 10
    }
}
