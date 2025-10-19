using Code_Editor.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Code_Editor
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        double _targetOffset = 0;

        private void TabScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                e.Handled = true;

                _targetOffset = Math.Max(0, Math.Min(scrollViewer.ScrollableWidth, _targetOffset - e.Delta / 120 * 50));

                var animation = new DoubleAnimation
                {
                    To = _targetOffset,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                scrollViewer.BeginAnimation(ScrollViewerOffsetHelper.HorizontalOffsetProperty, animation);
            }
        }
    }
}
