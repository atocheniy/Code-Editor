using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Code_Editor.Modules
{
    internal class NotificationBuilder
    {

        public NotificationBuilder() { }

        public Border Create(string label_s, Grid notificationGrid, int pos)
        {
            Border b = new Border()
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF222222")),
                CornerRadius = new CornerRadius(12),
                BorderThickness = new Thickness(1),
                RenderTransformOrigin = new Point(0.5, 0.5),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C4C4C")),
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = Double.NaN,
                Height = Double.NaN,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 6, 0, 6),
            };
            TransformGroup transformGroup = new TransformGroup();
            TranslateTransform translateTransform = new TranslateTransform();

            transformGroup.Children.Add(translateTransform);
            b.RenderTransform = transformGroup;

            b.Effect = new DropShadowEffect()
            {
                BlurRadius = 40,
                Opacity = 0.5,
                ShadowDepth = 5,
                RenderingBias = RenderingBias.Quality
            };

            StackPanel g = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5, 5, 12, 10),
            };

            StackPanel h = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };

            Label l = new Label()
            {
                Content = label_s,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9A9A9A")),
                FontSize = 16,
                FontFamily = new FontFamily("Segoe UI Variable Text Semibold")
            };
            g.Children.Add(l);

            Button b1 = new Button()
            {
                Style = (Style)Application.Current.Resources["ButtonStyle3"],
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4A4A4A")),
                BorderThickness = new Thickness(0),
                Content = "Ок",
                FontSize = 16,
                FontFamily = new FontFamily("Segoe UI Variable Text Semibold"),
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 138,
                Height = 28,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(8, 10, 0, 1),
            };

            Button b2 = new Button()
            {
                Style = (Style)Application.Current.Resources["ButtonStyle2"],
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1C6BDE")),
                BorderThickness = new Thickness(0),
                Content = "Включить",
                FontSize = 16,
                FontFamily = new FontFamily("Segoe UI Variable Text Semibold"),
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 138,
                Height = 28,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(8, 10, 0, 1),
            };

            b1.Click += (se, es) =>
            {
                var storyboard2 = new Storyboard();
                var moveXAnimation2 = new DoubleAnimationUsingKeyFrames();

                moveXAnimation2.KeyFrames.Add(new SplineDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new KeySpline(0.1, 0.1, 0.1, 1)));

                Storyboard.SetTarget(moveXAnimation2, b);
                Storyboard.SetTargetProperty(moveXAnimation2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));

                storyboard2.Children.Add(moveXAnimation2);

                storyboard2.Completed += async (s2e, e2s) =>
                {
                    notificationGrid.Children.Remove(b);
                };

                storyboard2.Begin();
            };

            b2.Click += (se, es) =>
            {
                var storyboard2 = new Storyboard();
                var moveXAnimation2 = new DoubleAnimationUsingKeyFrames();

                moveXAnimation2.KeyFrames.Add(new SplineDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new KeySpline(0.1, 0.1, 0.1, 1)));

                Storyboard.SetTarget(moveXAnimation2, b);
                Storyboard.SetTargetProperty(moveXAnimation2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));

                storyboard2.Children.Add(moveXAnimation2);

                storyboard2.Completed += async (s2e, e2s) =>
                {
                    notificationGrid.Children.Remove(b);
                };

                storyboard2.Begin();
            };

            h.Children.Add(b1);
            h.Children.Add(b2);

            g.Children.Add(h);

            b.Child = g;

            Grid.SetRow(b, pos);

            notificationGrid.Children.Add(b);

            var storyboard = new Storyboard();
            var moveXAnimation = new DoubleAnimationUsingKeyFrames();

            moveXAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(-350, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new KeySpline(0.1, 0.1, 0.1, 1)));

            Storyboard.SetTarget(moveXAnimation, b);
            Storyboard.SetTargetProperty(moveXAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));

            storyboard.Children.Add(moveXAnimation);

            storyboard.Completed += async (se, es) =>
            {
            };

            storyboard.Begin();

            return b;
        }
    }
}
