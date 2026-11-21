using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DanceLearning.Models;

namespace DanceLearning.Views
{
    /// <summary>
    /// Screen 4 — User Profile, gamification, progress bars, weekly calendar, playlist.
    /// </summary>
    public partial class ProfileView : Page
    {
        private readonly ObservableCollection<PlaylistEntry> _playlist = new();
        private readonly UserProfile _profile;

        public ProfileView()
        {
            InitializeComponent();
            _profile = AppData.GetUserProfile();
            InitialiseUi();
        }

        // ───────────────────────────────────────────────────────────
        //  Initialise
        // ───────────────────────────────────────────────────────────

        private void InitialiseUi()
        {
            // Header
            UserNameText.Text = _profile.Name;
            PointsText.Text = _profile.TotalPoints.ToString("N0");
            LessonsCompletedText.Text = _profile.LessonsCompleted.ToString();
            BundlesLearnedText.Text = _profile.BundlesLearned.ToString();

            // Build week calendar
            BuildWeekCalendar();

            // Playlist
            SeedDefaultPlaylist();
            PlaylistListBox.ItemsSource = _playlist;
        }

        // ───────────────────────────────────────────────────────────
        //  Week calendar builder
        // ───────────────────────────────────────────────────────────

        private void BuildWeekCalendar()
        {
            // Sample: Mon, Wed, Fri are scheduled; Mon and Wed are completed
            bool[] scheduled = { true, false, true, false, true, false, false };
            bool[] completed = { true, false, true, false, false, false, false };
            string[] labels = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };

            WeekGrid.Children.Clear();

            for (int i = 0; i < 7; i++)
            {
                int dayIndex = i;
                var dayBorder = new Border
                {
                    CornerRadius = new CornerRadius(10),
                    Margin = new Thickness(3),
                    Padding = new Thickness(4, 8, 4, 8),
                    Background = completed[i]
                        ? new LinearGradientBrush(
                            System.Windows.Media.Color.FromRgb(0x6C, 0x3C, 0xE1),
                            System.Windows.Media.Color.FromRgb(0xEC, 0x48, 0x99), 90)
                        : scheduled[i]
                            ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x25, 0x25, 0x45))
                            : new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x1A, 0x1A, 0x2E)),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                var sp = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
                sp.Children.Add(new TextBlock
                {
                    Text = labels[i],
                    Foreground = completed[i]
                        ? Brushes.White
                        : new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xB0, 0xB0, 0xC8)),
                    FontSize = 11,
                    FontFamily = new FontFamily("Segoe UI"),
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                // Dot indicator
                sp.Children.Add(new System.Windows.Shapes.Ellipse
                {
                    Width = 6, Height = 6,
                    Margin = new Thickness(0, 4, 0, 0),
                    Fill = scheduled[i]
                        ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x6C, 0x3C, 0xE1))
                        : new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x2A, 0x2A, 0x45)),
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                dayBorder.Child = sp;

                // Toggle schedule on click
                dayBorder.MouseLeftButtonUp += (_, _) => ToggleDaySchedule(dayBorder, dayIndex);

                WeekGrid.Children.Add(dayBorder);
            }
        }

        private void ToggleDaySchedule(Border dayBorder, int dayIndex)
        {
            // Visual toggle – in a real app this would persist to storage
            var accentBrush = new SolidColorBrush(
                System.Windows.Media.Color.FromRgb(0x25, 0x25, 0x45));
            dayBorder.Background = dayBorder.Background is SolidColorBrush sb
                                   && sb.Color.R == 0x25
                ? new LinearGradientBrush(
                    System.Windows.Media.Color.FromRgb(0x6C, 0x3C, 0xE1),
                    System.Windows.Media.Color.FromRgb(0xEC, 0x48, 0x99), 90)
                : accentBrush;
        }

        // ───────────────────────────────────────────────────────────
        //  Playlist
        // ───────────────────────────────────────────────────────────

        private void SeedDefaultPlaylist()
        {
            _playlist.Add(new PlaylistEntry { Title = "Hip-Hop — Базовые движения", Day = "Понедельник" });
            _playlist.Add(new PlaylistEntry { Title = "K-Pop — Первая связка", Day = "Среда" });
            _playlist.Add(new PlaylistEntry { Title = "Breakdance — Вход в топрок", Day = "Пятница" });
        }

        private void AddLesson_Click(object sender, RoutedEventArgs e)
        {
            // Simple demo: add a placeholder entry
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
        public string Day { get; set; } = string.Empty;
    }
}
