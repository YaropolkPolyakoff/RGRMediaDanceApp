using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace DanceLearning.Views
{
    /// <summary>
    /// Screen 3 — Interactive 3D Dancer Scene.
    /// Uses HelixToolkit.Wpf (HelixViewport3D) which provides built-in mouse orbit and zoom.
    /// The right panel allows selecting predefined movement animations applied via WPF 3D animations.
    /// </summary>
    public partial class Scene3DView : Page
    {
        private bool _autoRotating;

        private static readonly string[] StepDescriptions =
        {
            "Нейтральная стойка — исходная позиция тела перед началом связки.",
            "Шаг вправо — скользящий перенос веса на правую ногу.",
            "Волна руками — последовательное движение от плеча к запястью.",
            "Поворот на 180° — разворот на опорной ноге.",
            "Прыжок — отрыв от пола с фиксацией позы на высшей точке."
        };

        public Scene3DView()
        {
            InitializeComponent();
        }

        // ── Auto-rotation ────────────────────────────────────────────

        private void AutoRotate_Click(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("ModelRotateAnim");

            if (_autoRotating)
            {
                storyboard.Stop();
                AutoRotateBtn.Content = "▶  Авто-вращение";
                _autoRotating = false;
            }
            else
            {
                storyboard.Begin(DancerVisual);
                AutoRotateBtn.Content = "⏸  Остановить";
                _autoRotating = true;
            }
        }

        // ── Camera presets ───────────────────────────────────────────

        private void CameraFront_Click(object sender, RoutedEventArgs e)
            => MoveCameraTo(new Point3D(0, 1.5, 8), new Vector3D(0, -0.2, -1));

        private void CameraSide_Click(object sender, RoutedEventArgs e)
            => MoveCameraTo(new Point3D(8, 1.5, 0), new Vector3D(-1, -0.2, 0));

        private void CameraTop_Click(object sender, RoutedEventArgs e)
            => MoveCameraTo(new Point3D(0, 10, 0.001), new Vector3D(0, -1, 0));

        private void MoveCameraTo(Point3D targetPos, Vector3D targetLook)
        {
            if (PerspCam == null) return;
            var dur = new Duration(TimeSpan.FromSeconds(0.6));
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
            PerspCam.BeginAnimation(ProjectionCamera.PositionProperty,
                new Point3DAnimation(targetPos, dur) { EasingFunction = ease });
            PerspCam.BeginAnimation(ProjectionCamera.LookDirectionProperty,
                new Vector3DAnimation(targetLook, dur) { EasingFunction = ease });
        }

        // ── Movement step selection ───────────────────────────────────

        private void MovementList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = MovementListBox.SelectedIndex;
            if (idx < 0) return;

            MovementDescText.Text = idx < StepDescriptions.Length
                ? StepDescriptions[idx]
                : string.Empty;

            RunMovementAnimation(idx);
        }

        private void RunMovementAnimation(int stepIndex)
        {
            if (DancerVisual == null) return;

            // Temporarily pause auto-rotation
            if (_autoRotating)
                ((Storyboard)FindResource("ModelRotateAnim")).Stop();

            var dur = new Duration(TimeSpan.FromSeconds(0.8));
            var ease = new CubicEase { EasingMode = EasingMode.EaseInOut };

            switch (stepIndex)
            {
                case 0:
                    ApplyRotationAnimation(0, dur, ease);
                    ApplyTranslationAnimation(new Vector3D(0, 0, 0), dur, ease);
                    break;

                case 1:
                    ApplyTranslationAnimation(new Vector3D(0.5, 0, 0), dur, ease);
                    break;

                case 2:
                    ApplyRotationAnimation(15, dur, ease);
                    break;

                case 3:
                    var spinDur = new Duration(TimeSpan.FromSeconds(1.0));
                    ApplyRotationAnimation(180, spinDur, ease);
                    break;

                case 4:
                    var jumpUp = new Duration(TimeSpan.FromSeconds(0.4));
                    ApplyTranslationAnimation(new Vector3D(0, 0.6, 0), jumpUp, ease);
                    ScheduleCallback(0.5, () =>
                        ApplyTranslationAnimation(new Vector3D(0, 0, 0),
                            new Duration(TimeSpan.FromSeconds(0.4)), ease));
                    break;
            }

            // Resume auto-rotation after move completes
            if (_autoRotating)
            {
                ScheduleCallback(1.2, () =>
                    ((Storyboard)FindResource("ModelRotateAnim")).Begin(DancerVisual));
            }
        }

        // ── Animation helpers ─────────────────────────────────────────

        private void ApplyRotationAnimation(double targetAngle, Duration dur, IEasingFunction ease)
        {
            var anim = new DoubleAnimation(targetAngle, dur) { EasingFunction = ease };
            ModelRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, anim);
        }

        private void ApplyTranslationAnimation(Vector3D target, Duration dur, IEasingFunction ease)
        {
            // Ensure the dancer visual has a Transform3DGroup with a TranslateTransform3D
            if (DancerVisual.Transform is not Transform3DGroup group)
            {
                group = new Transform3DGroup();
                if (DancerVisual.Transform != null)
                    group.Children.Add(DancerVisual.Transform);
                var newTranslate = new TranslateTransform3D();
                group.Children.Add(newTranslate);
                DancerVisual.Transform = group;
            }

            // Find the translate transform inside the group
            TranslateTransform3D? tt = null;
            foreach (var child in group.Children)
            {
                if (child is TranslateTransform3D t3d) { tt = t3d; break; }
            }
            if (tt == null)
            {
                tt = new TranslateTransform3D();
                group.Children.Add(tt);
            }

            tt.BeginAnimation(TranslateTransform3D.OffsetXProperty,
                new DoubleAnimation(target.X, dur) { EasingFunction = ease });
            tt.BeginAnimation(TranslateTransform3D.OffsetYProperty,
                new DoubleAnimation(target.Y, dur) { EasingFunction = ease });
            tt.BeginAnimation(TranslateTransform3D.OffsetZProperty,
                new DoubleAnimation(target.Z, dur) { EasingFunction = ease });
        }

        private static void ScheduleCallback(double delaySec, Action action)
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(delaySec) };
            timer.Tick += (s, _) => { timer.Stop(); action(); };
            timer.Start();
        }

        // ── Mouse double-click resets camera ──────────────────────────

        private void HelixViewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                CameraFront_Click(sender, e);
        }
    }
}
