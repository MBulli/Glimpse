using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaState = System.Windows.Controls.MediaState;

namespace Glimpse.ViewModels.Previews
{
    class VideoPreviewModel : PropertyChangedBase, IPreviewModel
    {
        private Uri source;
        public Uri Source
        {
            get { return source; }
            set { source = value; OnPropertyChanged(); }
        }

        private MediaState playbackState;
        public MediaState PlaybackState
        {
            get { return playbackState; }
            set { playbackState = value; OnPropertyChanged(); }
        }

        public bool CanCreatePreview(Models.GlimpseItem item)
        {
            return item.PerceivedType == Models.PerceivedType.Video;
        }

        public void ShowPreview(Models.GlimpseItem item)
        {
            this.Source = item.Uri;
            this.PlaybackState = MediaState.Play;
        }

        public System.Windows.Size? PreferredPreviewSize(System.Windows.Size currentSize)
        {
            return null;
        }
    }
}
