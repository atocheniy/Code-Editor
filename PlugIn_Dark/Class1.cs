using ICSharpCode.AvalonEdit;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using Application = System.Windows.Application;
using Label = System.Windows.Controls.Label;

namespace PlugIn_Dark
{
    public class PlugIn
    {
        private readonly Color _background = Color.FromRgb(196, 196, 196);
        private readonly Color _foreground = Colors.Black;
        private readonly Color _accent = Color.FromRgb(0, 122, 204);
        private readonly Color _border = Color.FromRgb(204, 204, 204);

        public void ChangeWindow(Window window)
        {
            if (window.Content is DependencyObject content)
            {
                ApplyStylesRecursively(content);
            }
        }

        private void ApplyStylesRecursively(DependencyObject parent)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>())
            {
                switch (child)
                {
                    case TabItem tabItem:
                        StyleTabItem(tabItem);
                        break;
                    case Button button:
                        StyleButton(button);
                        break;
                    case TabControl tabControl:
                        StyleTabControl(tabControl);
                        break;
                    case TreeView treeView:
                        StyleTreeView(treeView);
                        break;
                    case TextEditor editor:
                        StyleTextEditor(editor);
                        break;
                    case ScrollViewer scroll:
                        StyleScrollViewer(scroll);
                        break;
                    default:
                        StyleCommonElements(child);
                        break;
                }

                if (child is Panel || child is ContentControl || child is Decorator || child is ItemsControl || child is TabControl || child is TabItem || child is TreeView || child is Grid || child is ScrollViewer || child is Border)
                {
                    ApplyStylesRecursively(child);
                }
            }
        }

        private void StyleButton(Button button)
        {
            if (button.Content is Image img)
            {
                switch (button.Name)
                {
                    case "SideBarHideButton":
                        img.Source = new BitmapImage(new Uri("Images\\light\\sidebar_toggle_nav_icon_145937.png", UriKind.Relative));
                        break;

                    case "UndoB":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Source = new BitmapImage(new Uri("Images\\light\\arrow_back_chevron_direction_left_navigation_right_icon_123223.png", UriKind.Relative));
                        break;

                    case "RedoB":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Source = new BitmapImage(new Uri("Images\\light\\arrows_chevron_direction_left_move_next_right_icon_123222.png", UriKind.Relative));
                        break;

                    case "CloseProjectButton":
                        img.Width = 18;
                        img.Margin = new Thickness(0, -1, 0, -1);
                        img.HorizontalAlignment = HorizontalAlignment.Left;

                        img.Source = new BitmapImage(new Uri("Images\\light\\arrows_chevron_direction_left_move_next_right_icon_123222.png", UriKind.Relative));
                        break;

                    case "SaveButton":
                        img.Source = new BitmapImage(new Uri("Images\\light\\save_arrow_right_regular_icon_203324.png", UriKind.Relative));
                        break;

                    case "ExitWindowControl1":
                        img.Source = new BitmapImage(new Uri("Images\\light\\cancel_close_delete_exit_logout_remove_x_icon_123217.png", UriKind.Relative));
                        break;

                    case "MinWindowControl1":
                        img.Source = new BitmapImage(new Uri("Images\\light\\minus-gross-horizontal-straight-line-symbol_icon-icons.com_74137.png", UriKind.Relative));
                        break;

                    case "MaxWindowControl1":
                        img.Source = new BitmapImage(new Uri("Images\\light\\popup_icon_216699.png", UriKind.Relative));
                        break;

                    case "SplitButton":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Margin = new Thickness(4, 4, 4, 4);

                        img.Source = new BitmapImage(new Uri("Images\\light\\layout_split_icon_159993.png", UriKind.Relative));
                        break;

                    case "NextButton":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Source = new BitmapImage(new Uri("Images\\light\\arrow_back_chevron_direction_left_navigation_right_icon_123223.png", UriKind.Relative));
                        break;

                    case "NextCodeButton":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Source = new BitmapImage(new Uri("Images\\light\\arrow_back_chevron_direction_left_navigation_right_icon_123223.png", UriKind.Relative));
                        break;

                    case "PrevButton":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Source = new BitmapImage(new Uri("Images\\light\\arrows_chevron_direction_left_move_next_right_icon_123222.png", UriKind.Relative));
                        break;

                    case "PrevCodeButton":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Source = new BitmapImage(new Uri("Images\\light\\arrows_chevron_direction_left_move_next_right_icon_123222.png", UriKind.Relative));
                        break;
                }
            }

            if (button.Name == "SideBarHideButton") return;

            if (button.Tag != null && button.Tag.ToString() == "ContextButton")
            {
                button.Foreground = new SolidColorBrush(_foreground);
                return;
            }

            if (button.Tag == null || button.Tag.ToString() != "BlueButton")
            {
                button.Background = new SolidColorBrush(Colors.White);
                button.BorderBrush = new SolidColorBrush(_border);
                button.Foreground = new SolidColorBrush(_foreground);
                button.Padding = new Thickness(8, 4, 8, 4);

                if (button.Name == "btnSave" || button.Name == "btnRun")
                {
                    button.Background = new SolidColorBrush(_accent);
                    button.Foreground = Brushes.White;
                }

                if (button.Tag != null && button.Tag.ToString() == "WindowsControls") return;

                button.Style = (Style)Application.Current.Resources["ButtonStyleLight"];
            }
        }

        private void StyleTabItem(TabItem tabItem)
        {
            tabItem.Background = Brushes.White;
            tabItem.Foreground = new SolidColorBrush(_foreground);

            if (tabItem.Tag.ToString() != "SystemTab")
            {
                if (Application.Current.Resources["TabItemLight"] is Style lightStyle)
                {
                    tabItem.Style = lightStyle;
                }
            }
        }

        private void StyleTabControl(TabControl tc)
        {
            if (tc.Tag == null || tc.Tag.ToString() != "SystemTabC")
            {
                tc.Background = Brushes.White;
            }
            tc.BorderBrush = new SolidColorBrush(_border);

            var itemStyle = new Style(typeof(TabItem));
            itemStyle.Setters.Add(new Setter(Control.BorderBrushProperty, new SolidColorBrush(_border)));
            itemStyle.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(_foreground)));
            itemStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.SemiBold));

            tc.ItemContainerGenerator.StatusChanged += (s, e) =>
            {
                if (tc.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    foreach (var item in tc.Items)
                    {
                        if (tc.ItemContainerGenerator.ContainerFromItem(item) is TabItem tabItem)
                        {
                            StyleTabItem(tabItem);

                            if (tabItem.Content is DependencyObject tabContent)
                            {
                                ApplyStylesRecursively(tabContent);
                            }

                            ApplyStylesRecursively(tabItem);
                        }
                    }
                }
            };

            tc.ItemContainerStyle = itemStyle;
        }

        private void StyleTreeView(TreeView tv)
        {
            // tv.Background = Brushes.White;
            tv.Foreground = new SolidColorBrush(_foreground);

            if (Application.Current.Resources["TreeViewStyleLight"] is Style treeStyle)
                tv.Style = treeStyle;
            tv.ItemContainerStyle = (Style)Application.Current.Resources["TreeViewItemStyleLight"];

            tv.Loaded += (s, e) =>
            {
                foreach (var item in tv.Items)
                {
                    if (tv.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem tvi)
                    {
                        StyleTreeViewItem(tvi);
                        ApplyStylesRecursively(tvi);
                    }
                }
            };

            tv.ItemContainerGenerator.StatusChanged += (s, e) =>
            {
                if (tv.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    foreach (var item in tv.Items)
                    {
                        if (tv.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem tvi)
                        {
                            tvi.Foreground = new SolidColorBrush(_foreground);
                            StyleTreeViewItem(tvi);
                        }
                    }
                }
            };
        }

        private void StyleTreeViewItem(TreeViewItem item)
        {
            item.Style = (Style)Application.Current.Resources["TreeViewItemStyleLight"];

            item.Loaded += (s, e) =>
            {
                foreach (var subItem in item.Items)
                {
                    if (item.ItemContainerGenerator.ContainerFromItem(subItem) is TreeViewItem subTvi)
                    {
                        StyleTreeViewItem(subTvi);
                        ApplyStylesRecursively(subTvi);
                    }
                }
            };

            item.Expanded += (s, e) =>
            {
                foreach (var subItem in item.Items)
                {
                    if (item.ItemContainerGenerator.ContainerFromItem(subItem) is TreeViewItem subTvi)
                    {
                        StyleTreeViewItem(subTvi);
                        ApplyStylesRecursively(subTvi);
                    }
                }
            };
        }

        private void StyleTextEditor(TextEditor editor)
        {
            editor.Background = Brushes.White;
            editor.Foreground = new SolidColorBrush(_foreground);
            editor.ShowLineNumbers = true;
            editor.LineNumbersForeground = new SolidColorBrush(Color.FromRgb(150, 150, 150));

            editor.TextArea.TextView.CurrentLineBackground = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
        }

        private void StyleScrollViewer(ScrollViewer scroll)
        {
            // scroll.Background = Brushes.White;
            // scroll.BorderBrush = new SolidColorBrush(_border);
        }

        private void StyleCommonElements(DependencyObject element)
        {
            switch (element)
            {
                case Border b:
                    if (b.Name == "HideBorder") break;
                    if (b.Name == "MainBorder") break;
                    if(b.Name == "SideBar")
                    {
                        b.Background = new SolidColorBrush(_background);
                        b.BorderBrush = new SolidColorBrush(_border);
                        break;
                    }
                    if (b.Tag != null) if (b.Tag.ToString() == "resizeElement") break;
                    b.Background = Brushes.White;
                    b.BorderBrush = new SolidColorBrush(_border);
                    break;

                case TextBox tb:
                    tb.Background = new SolidColorBrush(Colors.White);
                    tb.Foreground = new SolidColorBrush(_foreground);
                    tb.BorderBrush = new SolidColorBrush(_border);
                    break;

                case TextBlock tbl:
                    tbl.Foreground = Brushes.Gray;
                    break;

                case Image i:
                    if (i.Tag != null && i.Tag.ToString() == "FirstImage")
                    {
                        i.Source = new BitmapImage(new Uri("Images\\light\\code-optimization_icon-icons.com_53810.png", UriKind.Relative));
                    }
                    break;

                case ComboBox cb:
                    cb.Style = (Style)Application.Current.Resources["LightComboBoxStyle"];
                    cb.ItemContainerStyle = (Style)Application.Current.Resources["LightComboBoxItemStyle"];

                    cb.Background = Brushes.White;
                            cb.Foreground = new SolidColorBrush(Colors.Black);
                            cb.BorderBrush = new SolidColorBrush(_border);
                            break;

                        case Label lbl:
                            lbl.Foreground = new SolidColorBrush(_foreground);
                            break;

                        case DataGrid dg:
                            dg.Background = Brushes.White;
                            dg.Foreground = new SolidColorBrush(_foreground);
                            dg.BorderBrush = new SolidColorBrush(_border);
                            dg.RowBackground = Brushes.White;
                            dg.AlternatingRowBackground = new SolidColorBrush(Color.FromArgb(10, 0, 0, 0));
                            break;

                        case GridSplitter gs:
                            if (gs.Tag != null && gs.Tag.ToString() == "SystemSplitter")
                                gs.Background = new SolidColorBrush(Colors.Black);
                            break;
                        }
                    
        }
    }
}
