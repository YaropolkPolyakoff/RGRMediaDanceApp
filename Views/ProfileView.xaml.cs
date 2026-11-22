using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DanceLearning.Models;

namespace DanceLearning.Views
{
    /// <summary>
    /// Screen 4 — User profile: stats, progress, weekly calendar, playlist.
    /// </summary>
    public partial class ProfileView : Page
    {
        private readonly ObservableCollection<PlaylistEntry> _playlist = new();
        private readonly UserProfile _profile;

        // Mutable per-day flags (toggled on click)
        private readonly bool[] _scheduled = { true, false, true, false, true, false, false };
        private readonly bool[] _completed  = { true, false, true, false, false, false, false };
        private readonly string[] _dayLabels = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };

        public ProfileView()
        {
            InitializeComponent();
            _profile = AppData.GetUserProfile();
            InitialiseUi();
        }

        private void InitialiseUi()
        {
            UserNameText.Text         = _profile.Name;
            PointsText.Text           = _profile.TotalPoints.ToString("N0");
            LessonsCompletedText.Text = _profile.LessonsCompleted.ToString();
            BundlesLearnedText.Text   = _profile.BundlesLearned.ToString();
            BuildWeekCalendar();
            SeedDefaultPlaylist();
            PlaylistListBox.ItemsSource = _playlist;
        }

        private void BuildWeekCalendar()
        {
            WeekGrid.Children.Clear();

            for (int i = 0; i < 7; i++)
            {
                int dayIndex = i;
                bool isCompleted = _completed[i];
                bool isSched     = _scheduled[i];

                var dayBorder = new Border
                {
                    CornerRadius = new CornerRadius(6),
                    Margin       = new Thickness(2),
                    Padding      = new Thickness(4, 6, 4, 6),
                    Background   = GetDayBackground(isCompleted, isSched),
                    Cursor       = System.Windows.Input.Cursors.Hand
                };

                var sp = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
                sp.Children.Add(new TextBlock
                {
                    Text        = _dayLabels[i],
                    Foreground  = isCompleted
                        ? Brushes.White
                        : new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xB0, 0xB0, 0xC8)),
                    FontSize    = 11,
                    FontFamily  = new FontFamily("Segoe UI"),
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                sp.Children.Add(new System.Windows.Shapes.Ellipse
                {
                    Width = 6, Height = 6,
                    Margin = new Thickness(0, 3, 0, 0),
                    Fill   = isSched
                        ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x5B, 0x6C, 0xF6))
                        : new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x3A, 0x3A, 0x55)),
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                dayBorder.Child = sp;
                dayBorder.MouseLeftButtonUp += (_, _) => ToggleDaySchedule(dayBorder, dayIndex);
                WeekGrid.Children.Add(dayBorder);
            }
        }

        private static Brush GetDayBackground(bool completed, bool scheduled)
        {
            if (completed) return new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x5B, 0x6C, 0xF6));
            if (scheduled) return new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x35, 0x35, 0x50));
            return new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x27, 0x27, 0x3A));
        }

        private void ToggleDaySchedule(Border dayBorder, int dayIndex)
        {
            // Don't toggle completed days
            if (_completed[dayIndex]) return;
            _scheduled[dayIndex] = !_scheduled[dayIndex];
            dayBorder.Background = GetDayBackground(_completed[dayIndex], _scheduled[dayIndex]);
        }

        private void SeedDefaultPlaylist()
        {
            _playlist.Add(new PlaylistEntry { Title = "Hip-Hop — Базовые движения", Day = "Понедельник" });
            _playlist.Add(new PlaylistEntry { Title = "K-Pop — Первая связка", Day = "Среда" });
            _playlist.Add(new PlaylistEntry { Title = "Breakdance — Вход в топрок", Day = "Пятница" });
        }

        private void AddLesson_Click(object sender, RoutedEventArgs e)
        {
            string[] days = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" };
            string day = days[_playlist.Count % days.Length];
            _playlist.Add(new PlaylistEntry { Title = "High Heels — Новый урок", Day = day });
        }

        private void RemovePlaylistItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is PlaylistEntry entry)
                _playlist.Remove(entry);
        }
    }

    /// <summary>Represents a lesson entry in the weekly playlist.</summary>
    public class PlaylistEntry
    {
        public string Title { get; set; } = string.Empty;
        public string Day   { get; set; } = string.Empty;
    }
}
