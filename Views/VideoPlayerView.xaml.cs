using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DanceLearning.Models;

namespace DanceLearning.Views
{
    /// <summary>
    /// Screen 2 – Custom Video Player with speed control, mirror, and A-B loop.
    /// </summary>
    public partial class VideoPlayerView : Page
    {
        // ── State ──────────────────────────────────────────────────
        private bool _isPlaying;
        private bool _isDraggingSeek;
        private bool _isMirrored;
        private bool _isLoopActive;
        private TimeSpan _loopStart;
        private TimeSpan _loopEnd;
        private bool _loopPointASet;
        private double _speedRatio = 1.0;
        private readonly DispatcherTimer _positionTimer;
        private DanceStyle? _currentStyle;
        private Lesson? _currentLesson;

        // Path data constants for play/pause icon switching
        private const string PlayData  = "M4,2 L18,10 L4,18 Z";
        private const string PauseData = "M3,2 H8 V18 H3 Z M12,2 H17 V18 H12 Z";

        public VideoPlayerView()
        {
            InitializeComponent();

            _positionTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            _positionTimer.Tick += PositionTimer_Tick;

            LoopIndicator.Visibility = Visibility.Collapsed;
        }

        // ───────────────────────────────────────────────────────────
        //  Public API
        // ───────────────────────────────────────────────────────────

        /// <summary>Loads a lesson's video and step list into the player.</summary>
        public void LoadLesson(DanceStyle style, Lesson lesson)
        {
            _currentStyle = style;
            _currentLesson = lesson;

            LessonTitleText.Text = lesson.Title;
            StyleNameText.Text = style.Name;
            StepsList.ItemsSource = lesson.Steps;

            if (!string.IsNullOrWhiteSpace(lesson.VideoPath))
            {
                VideoPlayer.Source = new Uri(lesson.VideoPath, UriKind.RelativeOrAbsolute);
                NoVideoPlaceholder.Visibility = Visibility.Collapsed;
                Play();
            }
            else
            {
                VideoPlayer.Source = null;
                NoVideoPlaceholder.Visibility = Visibility.Visible;
                SetPlayIcon(false);
                SeekSlider.IsEnabled = false;
            }
        }

        /// <summary>Stops playback and releases the media file handle.</summary>
        public void StopMedia()
        {
            _positionTimer.Stop();
            VideoPlayer.Stop();
            VideoPlayer.Source = null;
        }

        // ───────────────────────────────────────────────────────────
        //  Playback helpers
        // ───────────────────────────────────────────────────────────

        private void Play()
        {
            VideoPlayer.Play();
            VideoPlayer.SpeedRatio = _speedRatio;
            _isPlaying = true;
            _positionTimer.Start();
            SetPlayIcon(true);
        }

        private void Pause()
        {
            VideoPlayer.Pause();
            _isPlaying = false;
            _positionTimer.Stop();
            SetPlayIcon(false);
        }

        private void SetPlayIcon(bool isPlaying)
        {
            PlayPath.Data = Geometry.Parse(isPlaying ? PauseData : PlayData);
        }

        /// <summary>
        /// Changes playback speed and updates the highlighted speed button.
        /// Uses tag-based approach to avoid fragile visual-tree traversal.
        /// </summary>
        private void SetSpeed(double ratio, Button activeBtn)
        {
            _speedRatio = ratio;
            if (_isPlaying)
                VideoPlayer.SpeedRatio = _speedRatio;

            // Update speed button highlight colours directly
            UpdateSpeedButtonHighlight(Speed05Btn,  "0.5×",  ratio == 0.5);
            UpdateSpeedButtonHighlight(Speed075Btn, "0.75×", ratio == 0.75);
            UpdateSpeedButtonHighlight(Speed1Btn,   "1×",    ratio == 1.0);
        }

        private void UpdateSpeedButtonHighlight(Button btn, string label, bool isActive)
        {
            if (btn.Content is TextBlock tb)
                tb.Foreground = isActive
                    ? (Brush)FindResource("AccentBrush")
                    : (Brush)FindResource("TextSecondaryBrush");
        }

        // ───────────────────────────────────────────────────────────
        //  Timer
        // ───────────────────────────────────────────────────────────

        private void PositionTimer_Tick(object? sender, EventArgs e)
        {
            if (!VideoPlayer.NaturalDuration.HasTimeSpan || _isDraggingSeek) return;

            var total = VideoPlayer.NaturalDuration.TimeSpan;
            var pos   = VideoPlayer.Position;

            SeekSlider.Value = total.TotalSeconds > 0
                ? (pos.TotalSeconds / total.TotalSeconds) * 100.0
                : 0;

            CurrentTimeText.Text = FormatTime(pos);

            // A-B loop enforcement
            if (_isLoopActive && _loopPointASet && pos >= _loopEnd)
                VideoPlayer.Position = _loopStart;
        }

        private static string FormatTime(TimeSpan t)
            => t.Hours > 0 ? t.ToString(@"h\:mm\:ss") : t.ToString(@"m\:ss");

        // ───────────────────────────────────────────────────────────
        //  MediaElement events
        // ───────────────────────────────────────────────────────────

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                var total = VideoPlayer.NaturalDuration.TimeSpan;
                TotalTimeText.Text = FormatTime(total);
                SeekSlider.IsEnabled = true;
                _loopEnd = total;
            }
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_isLoopActive)
            {
                VideoPlayer.Position = _loopStart;
                VideoPlayer.Play();
            }
            else
            {
                VideoPlayer.Position = TimeSpan.Zero;
                Pause();
            }
        }

        private void VideoPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show($"Ошибка загрузки видео: {e.ErrorException?.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            NoVideoPlaceholder.Visibility = Visibility.Visible;
            SeekSlider.IsEnabled = false;
        }

        // ───────────────────────────────────────────────────────────
        //  Control click handlers
        // ───────────────────────────────────────────────────────────

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying) Pause(); else Play();
        }

        private void Rewind_Click(object sender, RoutedEventArgs e)
        {
            var newPos = VideoPlayer.Position - TimeSpan.FromSeconds(5);
            VideoPlayer.Position = newPos < TimeSpan.Zero ? TimeSpan.Zero : newPos;
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (!VideoPlayer.NaturalDuration.HasTimeSpan) return;
            var total  = VideoPlayer.NaturalDuration.TimeSpan;
            var newPos = VideoPlayer.Position + TimeSpan.FromSeconds(5);
            VideoPlayer.Position = newPos > total ? total : newPos;
        }

        private void Speed05_Click(object sender, RoutedEventArgs e)  => SetSpeed(0.5,  Speed05Btn);
        private void Speed075_Click(object sender, RoutedEventArgs e) => SetSpeed(0.75, Speed075Btn);
        private void Speed1_Click(object sender, RoutedEventArgs e)   => SetSpeed(1.0,  Speed1Btn);

        private void Mirror_Click(object sender, RoutedEventArgs e)
        {
            _isMirrored = !_isMirrored;
            VideoMirrorTransform.ScaleX = _isMirrored ? -1 : 1;
        }

        /// <summary>
        /// A-B Loop (three-state):
        ///   Click 1 → set loop start (A)
        ///   Click 2 → set loop end (B) and activate loop
        ///   Click 3 → cancel loop
        /// </summary>
        private void LoopSegment_Click(object sender, RoutedEventArgs e)
        {
            if (!VideoPlayer.NaturalDuration.HasTimeSpan) return;

            if (!_loopPointASet)
            {
                _loopStart = VideoPlayer.Position;
                _loopPointASet = true;
                LoopRangeText.Text = $"A: {FormatTime(_loopStart)}…";
                LoopIndicator.Visibility = Visibility.Visible;
            }
            else if (!_isLoopActive)
            {
                _loopEnd = VideoPlayer.Position;
                if (_loopEnd <= _loopStart)
                    _loopEnd = VideoPlayer.NaturalDuration.TimeSpan;
                _isLoopActive = true;
                LoopRangeText.Text = $"{FormatTime(_loopStart)} – {FormatTime(_loopEnd)}";
                // Begin the pulsing storyboard with this element as the name scope
                var sb = (Storyboard)FindResource("LoopPulse");
                sb.Begin(LoopIndicator, HandoffBehavior.SnapshotAndReplace, true);
            }
            else
            {
                // Cancel loop
                _isLoopActive = false;
                _loopPointASet = false;
                LoopIndicator.Visibility = Visibility.Collapsed;
                var sb = (Storyboard)FindResource("LoopPulse");
                sb.Stop(LoopIndicator);
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VideoPlayer != null)
                VideoPlayer.Volume = e.NewValue;
        }

        private void SeekSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => _isDraggingSeek = true;

        private void SeekSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDraggingSeek = false;
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                var total = VideoPlayer.NaturalDuration.TimeSpan;
                VideoPlayer.Position = TimeSpan.FromSeconds(
                    (SeekSlider.Value / 100.0) * total.TotalSeconds);
            }
        }

        private void SeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Intentionally empty — seeking is committed only on mouse-up
        }

        private void StepsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StepsList.SelectedItem is DanceStep step)
            {
                step.IsCompleted = true;
                StepsList.Items.Refresh();
            }
        }
    }
}
