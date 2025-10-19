using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Code_Editor.Modules
{
    public static class ScrollViewerOffsetHelper
    {
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollViewerOffsetHelper), new PropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static double GetVerticalOffset(DependencyObject obj) => (double)obj.GetValue(VerticalOffsetProperty);
        public static void SetVerticalOffset(DependencyObject obj, double value) => obj.SetValue(VerticalOffsetProperty, value);

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer) scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.RegisterAttached(
    "HorizontalOffset",
    typeof(double),
    typeof(ScrollViewerOffsetHelper),
    new PropertyMetadata(0.0, OnHorizontalOffsetChanged)
);

        public static double GetHorizontalOffset(DependencyObject obj) => (double)obj.GetValue(HorizontalOffsetProperty);
        public static void SetHorizontalOffset(DependencyObject obj, double value) => obj.SetValue(HorizontalOffsetProperty, value);

        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
                scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
        }
    }
}
