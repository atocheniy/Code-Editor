using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Code_Editor
{
    public class ResizePanels
    {
        private bool isResizing = false;
        private Point clickPosition;
        private Window window;

        public ResizePanels(Window window)
        {
            this.window = window;
        }

        private void StartResizing(object sender, MouseButtonEventArgs e, UIElement element)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                isResizing = true;
                clickPosition = e.GetPosition(window);
                Mouse.Capture(element);
            }
        }

        private void StopResizing(object sender, MouseButtonEventArgs e)
        {
            isResizing = false;
            Mouse.Capture(null);
        }

        private void Resize(object sender, MouseEventArgs e, double widthFactor, double heightFactor, bool adjustLeft, bool adjustTop)
        {
            if (!isResizing) return;

            var currentPosition = e.GetPosition(window);
            var widthChange = clickPosition.X - currentPosition.X;
            var heightChange = clickPosition.Y - currentPosition.Y;

            window.Width += widthChange * widthFactor;
            window.Height += heightChange * heightFactor;

            if (adjustLeft) window.Left -= widthChange;
            if (adjustTop) window.Top -= heightChange;

            clickPosition = currentPosition;
        }

        public void Handler(UIElement element, string direction)
        {
            element.MouseLeftButtonDown += (s, e) => StartResizing(s, e, element);
            element.MouseLeftButtonUp += StopResizing;
            element.MouseMove += (s, e) =>
            {
                switch (direction)
                {
                    case "Up": Resize(s, e, 0, 1, false, true); break;
                    case "Down": Resize(s, e, 0, -1, false, false); break;
                    case "Left": Resize(s, e, 1, 0, true, false); break;
                    case "Right": Resize(s, e, -1, 0, false, false); break;
                    case "LeftTop": Resize(s, e, 1, 1, true, true); break;
                    case "RightTop": Resize(s, e, -1, 1, false, true); break;
                    case "LeftDown": Resize(s, e, 1, -1, true, false); break;
                    case "RightDown": Resize(s, e, -1, -1, false, false); break;
                }
            };
        }
    }
}
