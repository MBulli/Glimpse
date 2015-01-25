using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Glimpse.Views
{
    /// <summary>
    /// Interaction logic for VideoView.xaml
    /// </summary>
    public partial class VideoView : UserControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(VideoView), 
                                        new PropertyMetadata(null, OnSourceChanged));

        public static readonly DependencyProperty PlaybackStateProperty =
            DependencyProperty.Register("PlaybackState", typeof(MediaState), typeof(VideoView),
                                        new PropertyMetadata(MediaState.Close, OnPlaybackStateChanged));

        public static readonly DependencyProperty IsSoundMutedProperty =
            DependencyProperty.Register("IsSoundMuted", typeof(bool), typeof(VideoView),
                                        new PropertyMetadata(false, OnIsSoundMutedChanged));

        private MediaState currentMediaState = MediaState.Pause;
        private DispatcherTimer seekTimer;


        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public MediaState PlaybackState
        {
            get { return (MediaState)GetValue(PlaybackStateProperty); }
            set { SetValue(PlaybackStateProperty, value); }
        }

        public bool IsSoundMuted
        {
            get { return (bool)GetValue(IsSoundMutedProperty); }
            set { SetValue(IsSoundMutedProperty, value); }
        }
     
        public VideoView()
        {
            InitializeComponent();

            this.mediaElement.LoadedBehavior = MediaState.Manual;
            this.mediaElement.UnloadedBehavior = MediaState.Stop;

            this.seekTimer = new DispatcherTimer();
            this.seekTimer.Interval = TimeSpan.FromSeconds(0.5);
            this.seekTimer.Tick += seekTimer_Tick;
        }

        void seekTimer_Tick(object sender, EventArgs e)
        {
            UpdateUIElements();
        }

        private void OnSourceChanged()
        {
            Open(this.Source);
        }

        private void OnPlaybackStateChanged()
        {
            switch (this.PlaybackState)
            {
                case MediaState.Pause: Pause(); break;
                case MediaState.Play: Play(); break;
                case MediaState.Stop: Stop(); break;
                case MediaState.Close:
                case MediaState.Manual:
                default:
                    break;
            }
        }

        public void Open(Uri file)
        {
            this.mediaElement.LoadedBehavior = MediaState.Manual;
            this.mediaElement.UnloadedBehavior = MediaState.Manual;
            this.mediaElement.Source = file;
        }

        public void Play()
        {
            this.mediaElement.Play();
            this.currentMediaState = MediaState.Play;
            this.seekTimer.IsEnabled = true;

            UpdateUIElements();
        }

        public void Pause()
        {
            this.mediaElement.Pause();
            this.currentMediaState = MediaState.Pause;
            this.PlaybackState = MediaState.Pause;
            this.seekTimer.IsEnabled = false;

            UpdateUIElements();
        }

        public void Stop()
        {
            this.mediaElement.Stop();
            this.currentMediaState = MediaState.Stop;
            this.seekTimer.IsEnabled = false;

            UpdateUIElements();
        }

        public void TogglePlayPause()
        {
            if (currentMediaState == MediaState.Play)
                Pause();
            else
                Play();
        }

        public void MuteVolume(bool mute)
        {
            this.mediaElement.IsMuted = mute;

            UpdateUIElements();
        }

        public void ToggleMuteVolume()
        {
            MuteVolume(!this.mediaElement.IsMuted);
        }

        private void UpdateUIElements()
        {
            // Update play/pause button
            // TODO binding
            //if (currentMediaState == MediaState.Play)
            //    this.playPauseButton.Content = Resources["MediaStatePauseImage"];
            //else
            //    this.playPauseButton.Content = Resources["MediaStatePlayImage"];

            //if (this.mediaElement.IsMuted)
            //    this.muteSoundButton.Content = Resources["MediaSoundMutedImage"];
            //else
            //    this.muteSoundButton.Content = Resources["MediaSoundRegularImage"];

            UpdateSeekbar();
        }

        private void UpdateSeekbar()
        {
            this.seekbar.Minimum = 0;

            if (this.mediaElement.NaturalDuration.HasTimeSpan)
            {
                var duration = this.mediaElement.NaturalDuration.TimeSpan;
                var position = this.mediaElement.Position;

                this.seekbar.Maximum = duration.TotalMilliseconds;
                this.seekbar.Value = position.TotalMilliseconds;

                this.timeLabel.Text = string.Format(@"{0} / {1}", FormatTimeSpan(position), FormatTimeSpan(duration));
            }
        }

        private string FormatTimeSpan(TimeSpan span)
        {
            if (span.Hours > 0)
                return span.ToString(@"hh\:mm\:ss");
            else
                return span.ToString(@"mm\:ss");
        }

        void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            UpdateUIElements();
        }

        void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            UpdateUIElements();
        }

        private void PlayCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Play();
        }

        private void PauseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Pause();
        }

        private void StopCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Stop();
        }

        private void TogglePlayPauseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TogglePlayPause();
        }

        private void MuteVolumeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ToggleMuteVolume();
        }

        private void seekBar_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            
        }

        private void seekBar_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {

        }

        private void seekBar_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        #region DP value change callbacks
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoView)d).OnSourceChanged();
            //((VideoView)d).mediaElement.Source = (Uri)e.NewValue;
        }

        private static void OnPlaybackStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoView)d).OnPlaybackStateChanged();
        }

        private static void OnIsSoundMutedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion

        





    }
}
