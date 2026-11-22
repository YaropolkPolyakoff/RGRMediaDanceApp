using System.Windows;
using System.Windows.Controls;
using DanceLearning.Views;

namespace DanceLearning
{
    /// <summary>
    /// Main window — left navigation + content Frame.
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
            _catalogView = new CatalogView();
            _videoPlayerView = new VideoPlayerView();
            _scene3DView = new Scene3DView();
            _profileView = new ProfileView();

            _catalogView.OpenLessonRequested += (style, lesson) =>
            {
                _videoPlayerView.LoadLesson(style, lesson);
                NavigateTo(_videoPlayerView, "Видеоплеер");
                SetActiveNavButton(NavPlayerBtn);
            };

            NavigateTo(_catalogView, "Каталог стилей");
        }

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

        protected override void OnClosed(System.EventArgs e)
        {
            _videoPlayerView.StopMedia();
            base.OnClosed(e);
        }
        private void ToggleMaximize()
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }
}
