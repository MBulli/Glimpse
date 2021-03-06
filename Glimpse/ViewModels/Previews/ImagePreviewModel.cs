﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Glimpse.ViewModels.Previews
{
    class ImagePreviewModel : PropertyChangedBase, IPreviewModel
    {
        private static readonly string[] supportedExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff" };

        private BitmapFrame image;

        private string source;
        public string Source
        {
            get { return source; }
            set { source = value; OnPropertyChanged(); }
        }

        public bool CanCreatePreview(Models.GlimpseItem item)
        {
            return supportedExtensions.Contains(item.FileExtension);
        }

        public void ShowPreview(Models.GlimpseItem item)
        {
            this.Source = item.FullPath;

            BitmapDecoder decoder = BitmapDecoder.Create(item.Uri, BitmapCreateOptions.None, BitmapCacheOption.None);
            this.image = decoder.Frames[0];
        }

        public System.Windows.Size? PreferredPreviewSize(System.Windows.Size currentSize)
        {
            return new System.Windows.Size(image.PixelWidth, image.PixelHeight);   
        }
    }
}
