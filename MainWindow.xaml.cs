using System.Windows;
using System.Windows.Controls;
using DanceLearning.Views;

namespace DanceLearning
{
    /// <summary>
    /// Main application window with fixed left navigation and content frame.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CatalogView _catalogView;
        private readonly VideoPlayerView _videoPlayerView;
        private readonly Scene3DView _scene3DView;
        private readonly ProfileView _profileView;
        public MainWindow()
        {
            InitializeComponent();
            // Pre-create all views so state is preserved during navigation
            _catalogView = new CatalogView();
            _videoPlayerView = new VideoPlayerView();
            _scene3DView = new Scene3DView();
            _profileView = new ProfileView();
            // Wire catalog "Open Lesson" event to navigate to player
            _catalogView.OpenLessonRequested += (style, lesson) =>
            {
                _videoPlayerView.LoadLesson(style, lesson);
                NavigateTo(_videoPlayerView, "Видеоплеер");
                SetActiveNavButton(NavPlayerBtn);
            };
            // Start on catalog
            NavigateTo(_catalogView, "Каталог стилей");
        }
        // ───────────────────────────────────────────────────────────
        //  Navigation helpers
        // ───────────────────────────────────────────────────────────
        private void NavigateTo(Page page, string title)
        {
            PageTitleText.Text = title;
            MainFrame.Navigate(page);
        }
        private void SetActiveNavButton(Button active)
        {
            Button[] allButtons = { NavCatalogBtn, NavPlayerBtn, Nav3DBtn, NavProfileBtn };
            foreach (var btn in allButtons)
            {
                btn.Style = (Style)FindResource("NavButtonStyle");
            }
            active.Style = (Style)FindResource("NavButtonActiveStyle");
        }
        // ───────────────────────────────────────────────────────────
        //  Navigation click handlers
        // ───────────────────────────────────────────────────────────
        private void NavCatalog_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNavButton(NavCatalogBtn);
            NavigateTo(_catalogView, "Каталог стилей");
        }
        private void NavPlayer_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNavButton(NavPlayerBtn);
            NavigateTo(_videoPlayerView, "Видеоплеер");
        }
        private void Nav3D_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNavButton(Nav3DBtn);
            NavigateTo(_scene3DView, "3D-демонстрация");
        }
        private void NavProfile_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNavButton(NavProfileBtn);
            NavigateTo(_profileView, "Личный кабинет");
        }
        // ───────────────────────────────────────────────────────────
        //  Custom window chrome handlers
        // ───────────────────────────────────────────────────────────
        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleMaximize();
            }
            else
            {
                DragMove();
            }
        }
        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            // Stop media before closing to release file handles
            _videoPlayerView.StopMedia();
            Close();
        }
        private void ToggleMaximize()
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }
}
