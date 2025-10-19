using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Code_Editor
{
    internal class Functions
    {
        public Functions() { }

        public string GetFullPath(TreeViewItem item, string currentFolder)
        {
            if(item == null) return null;
            string path = item.Header.ToString();

            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(item);

            while (parent is TreeViewItem parentItem)
            {
                path = parentItem.Header.ToString() + "\\" + path;
                parent = ItemsControl.ItemsControlFromItemContainer(parentItem);
            }

            string path_file = "";
            string[] s = path.Split('\\');
            for (int i = 1; i < s.Length; i++)
            {
                path_file += "\\" + s[i];
            }

            return currentFolder + path_file;
        }

        public string AnimateOpacity(UIElement element, double to, string setting)
        {
            double current = (double)element.GetValue(UIElement.OpacityProperty);

            element.BeginAnimation(UIElement.OpacityProperty, null);

            var animation = new DoubleAnimation
            {
                From = current,
                To = to,
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            element.BeginAnimation(UIElement.OpacityProperty, animation);

            if (element is Border b && b.Name == "SideBar")
            {
                setting = to.ToString();
                return setting;
            }

            return setting;
        }

        public void AnimateOpacity(UIElement element, double to)
        {
            double current = (double)element.GetValue(UIElement.OpacityProperty);

            element.BeginAnimation(UIElement.OpacityProperty, null);

            var animation = new DoubleAnimation
            {
                From = current,
                To = to,
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            element.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        public Storyboard Animation(UIElement element, Storyboard storyboard, TimeSpan total, Color? color,
            double scaleX_old_val, double scaleY_old_val, double traslateX_old_val, double translateY_old_val,
            double scaleX_new_val, double scaleY_new_val, double traslateX_new_val, double translateY_new_val,
            bool withSecondAnimation, UIElement el2,
            double translateY2_old_val, double translateY2_new_val)
        {
            TimeSpan totalDuration = total;

            var scaleTransform = new ScaleTransform(0.8, 0.8);
            var translateTransform = new TranslateTransform(0, 0);
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            element.RenderTransform = transformGroup;
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            var scaleX = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(scaleX, element);
            Storyboard.SetTargetProperty(scaleX, new PropertyPath("RenderTransform.Children[0].(ScaleTransform.ScaleX)"));
            scaleX.KeyFrames.Add(new DiscreteDoubleKeyFrame(scaleX_old_val, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            scaleX.KeyFrames.Add(new SplineDoubleKeyFrame(scaleX_new_val, KeyTime.FromTimeSpan(totalDuration), new KeySpline(0.5, 0, 0.5, 1)));
            storyboard.Children.Add(scaleX);

            var scaleY = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(scaleY, element);
            Storyboard.SetTargetProperty(scaleY, new PropertyPath("RenderTransform.Children[0].(ScaleTransform.ScaleY)"));
            scaleY.KeyFrames.Add(new DiscreteDoubleKeyFrame(scaleY_old_val, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            scaleY.KeyFrames.Add(new SplineDoubleKeyFrame(scaleY_new_val, KeyTime.FromTimeSpan(totalDuration), new KeySpline(0.5, 0, 0.5, 1)));
            storyboard.Children.Add(scaleY);

            var translateX = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(translateX, element);
            Storyboard.SetTargetProperty(translateX, new PropertyPath("RenderTransform.Children[1].(TranslateTransform.X)"));
            translateX.KeyFrames.Add(new SplineDoubleKeyFrame(traslateX_old_val, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            translateX.KeyFrames.Add(new SplineDoubleKeyFrame(traslateX_new_val, KeyTime.FromTimeSpan(totalDuration), new KeySpline(0.5, 0, 0.5, 1)));
            storyboard.Children.Add(translateX);

            var translateY = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(translateY, element);
            Storyboard.SetTargetProperty(translateY, new PropertyPath("RenderTransform.Children[1].(TranslateTransform.Y)"));
            translateY.KeyFrames.Add(new SplineDoubleKeyFrame(translateY_old_val, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            translateY.KeyFrames.Add(new SplineDoubleKeyFrame(translateY_new_val, KeyTime.FromTimeSpan(totalDuration), new KeySpline(0.5, 0, 0.5, 1)));
            storyboard.Children.Add(translateY);

            if (color != null)
            {
                var colorAnimation = new ColorAnimation()
                {
                    To = color,
                    Duration = TimeSpan.FromSeconds(totalDuration.TotalSeconds),
                };
                Storyboard.SetTarget(colorAnimation, element);
                Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Shape.Fill).(SolidColorBrush.Color)"));
                storyboard.Children.Add(colorAnimation);
            }

            if (withSecondAnimation && el2 != null)
            {
                var translateTransform2 = new TranslateTransform(0, 0);
                var transformGroup2 = new TransformGroup();
                transformGroup2.Children.Add(translateTransform2);
                el2.RenderTransform = transformGroup2;
                el2.RenderTransformOrigin = new Point(0.5, 0.5);

                var translateY2 = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(translateY2, el2);
                Storyboard.SetTargetProperty(translateY2, new PropertyPath("RenderTransform.Children[0].(TranslateTransform.Y)"));
                translateY2.KeyFrames.Add(new SplineDoubleKeyFrame(translateY2_old_val, KeyTime.FromTimeSpan(TimeSpan.Zero)));
                translateY2.KeyFrames.Add(new SplineDoubleKeyFrame(translateY2_new_val, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.6)), new KeySpline(0.5, 0, 0.5, 1)));
                storyboard.Children.Add(translateY2);
            }

            return storyboard;
        }

        public TreeViewItem Files(string path, TreeViewItem tree)
        {

            DirectoryInfo dinfo = new DirectoryInfo(path);

            if (dinfo.Exists)
            {
                try
                {

                    FileInfo[] files = dinfo.GetFiles();
                    foreach (FileInfo current in files)
                    {
                        TreeViewItem item = new TreeViewItem { Header = current.Name, Style = (Style)Application.Current.Resources["TreeViewItemStyle2"] };
                        tree.Items.Add(item);
                    }

                    DirectoryInfo[] dirs = dinfo.GetDirectories();
                    foreach (DirectoryInfo current in dirs)
                    {
                        TreeViewItem item = new TreeViewItem { Header = current.Name, Style = (Style)Application.Current.Resources["TreeViewItemStyle2"] };
                        item = Files(current.FullName, item);
                        tree.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Path is not exists");
            }

            return tree;
        }

        public void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = System.IO.Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = System.IO.Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public TreeViewItem GetParentFolderTreeViewItem(TreeViewItem item)
        {
            if (item == null) return null;

            TreeViewItem current = item;

            while (current != null)
            {
                if (current.Items.Count > 0)
                    return current;

                current = GetParentTreeViewItem(current);
            }
            return null;
        }

        public TreeViewItem GetParentTreeViewItem(DependencyObject item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (parent != null && !(parent is TreeViewItem))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as TreeViewItem;
        }

        public TreeViewItem GetTreeViewItemUnderMouse(ItemsControl container, Point point)
        {
            HitTestResult hitTest = VisualTreeHelper.HitTest(container, point);
            if (hitTest == null) return null;

            DependencyObject current = hitTest.VisualHit;
            while (current != null)
            {
                if (current is TreeViewItem item)
                    return item;

                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }
}
