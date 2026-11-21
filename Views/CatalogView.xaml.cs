using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DanceLearning.Models;
namespace DanceLearning.Views
{
    /// <summary>
    /// Screen 1 – Dance Styles Catalog.
    /// Displays a grid of interactive dance style cards.
    /// </summary>
    public partial class CatalogView : Page
    {
        /// <summary>
        /// Raised when the user clicks "Open" on a lesson inside a style detail popup.
        /// Arguments: selected DanceStyle, selected Lesson.
        /// </summary>
        public event Action<DanceStyle, Lesson>? OpenLessonRequested;
        public CatalogView()
        {
            InitializeComponent();
            LoadStyles();
        }
        private void LoadStyles()
        {
            var styles = AppData.GetDanceStyles();
            StylesItemsControl.ItemsSource = styles;
        }
        /// <summary>
        /// Handles clicking on a dance style card.
        /// Opens the first available lesson in that style.
        /// </summary>
        private void Card_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is DanceStyle style)
            {
                if (style.Lessons.Count == 0)
                {
                    MessageBox.Show($"В стиле «{style.Name}» пока нет уроков.",
                                    "Нет уроков", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                // Find the first incomplete lesson, or just the first one
                var lesson = style.Lessons.Find(l => !l.IsCompleted) ?? style.Lessons[0];
                OpenLessonRequested?.Invoke(style, lesson);
            }
        }
    }
}
