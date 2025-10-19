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
        //private readonly Color _background = Color.FromRgb(196, 196, 196);
        //private readonly Color _foreground = Colors.Black;
        //private readonly Color _accent = Color.FromRgb(0, 122, 204);
        //private readonly Color _border = Color.FromRgb(204, 204, 204);

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
                        img.Source = new BitmapImage(new Uri("Images\\icons\\sidebar_toggle_nav_icon_145937_negate.png", UriKind.Relative));
                        break;

                    case "UndoB":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;

                        img.Source = new BitmapImage(new Uri("Images\\icons\\arrow_back_chevron_direction_left_navigation_right_icon_123223_negate.png", UriKind.Relative));
                        break;

                    case "RedoB":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;

                        img.Source = new BitmapImage(new Uri("Images\\icons\\arrows_chevron_direction_left_move_next_right_icon_123222_negate.png", UriKind.Relative));
                        break;

                    case "CloseProjectButton":
                        img.Width = 18;
                        img.Margin = new Thickness(0, -1, 0, -1);
                        img.HorizontalAlignment = HorizontalAlignment.Left;

                        img.Source = new BitmapImage(new Uri("Images\\icons\\arrows_chevron_direction_left_move_next_right_icon_123222_negate.png", UriKind.Relative));
                        break;

                    case "SaveButton":
                        img.Source = new BitmapImage(new Uri("Images\\icons\\save_arrow_right_regular_icon_203324_negate.png", UriKind.Relative));
                        break;

                    case "ExitWindowControl1":
                        img.Source = new BitmapImage(new Uri("Images\\titlebar\\close.png", UriKind.Relative));
                        break;

                    case "MinWindowControl1":
                        img.Source = new BitmapImage(new Uri("Images\\titlebar\\min.png", UriKind.Relative));
                        break;

                    case "MaxWindowControl1":
                        img.Source = new BitmapImage(new Uri("Images\\titlebar\\max.png", UriKind.Relative));
                        break;

                    case "SplitButton":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Margin = new Thickness(4, 4, 4, 4);
                        img.Source = new BitmapImage(new Uri("Images\\icons\\layout_split_icon_159993_negate.png", UriKind.Relative));
                        break;

                    case "NextButton":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Source = new BitmapImage(new Uri("Images\\icons\\arrow_back_chevron_direction_left_navigation_right_icon_123223_negate.png", UriKind.Relative));
                        break;

                    case "NextCodeButton":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Source = new BitmapImage(new Uri("Images\\icons\\arrow_back_chevron_direction_left_navigation_right_icon_123223_negate.png", UriKind.Relative));
                        break;

                    case "PrevButton":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Source = new BitmapImage(new Uri("Images\\icons\\arrows_chevron_direction_left_move_next_right_icon_123222_negate.png", UriKind.Relative));
                        break;

                    case "PrevCodeButton":
                        img.Width = 32;
                        img.Height = 32;
                        img.Stretch = Stretch.UniformToFill;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Source = new BitmapImage(new Uri("Images\\icons\\arrows_chevron_direction_left_move_next_right_icon_123222_negate.png", UriKind.Relative));
                        break;
                }
            }

            if (button.Name == "SideBarHideButton") return;

            if (button.Tag != null && button.Tag.ToString() == "ContextButton")
            {
                button.Foreground = new SolidColorBrush(Colors.White);
                return;
            }

            if (button.Tag == null || button.Tag.ToString() != "BlueButton")
            {
                button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F2F2F2F"));
                button.BorderBrush = new SolidColorBrush(Colors.Transparent);
                button.Foreground = new SolidColorBrush(Colors.White);
                button.Padding = new Thickness(8, 4, 8, 4);

                if (button.Name == "btnSave" || button.Name == "btnRun")
                {
                    // button.Background = new SolidColorBrush(_accent);
                    button.Foreground = Brushes.White;
                }
            }
            if (button.Tag != null && button.Tag.ToString() == "wrapButton")
            {
                button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1D1D1D"));
                button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2E2E2E"));
                button.Foreground = new SolidColorBrush(Colors.White);

                button.Style = (Style)Application.Current.Resources["TitleBarButton"];

                return;
            }

            if (button.Tag != null && button.Tag.ToString() == "WindowsControls")
            {
                button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F131313"));
                button.Foreground = new SolidColorBrush(Colors.White);

                return;
            }

            button.Style = (Style)Application.Current.Resources["ButtonStyle3"];
        }

        private void StyleTabItem(TabItem tabItem)
        {
            tabItem.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF292929"));
            tabItem.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF565656"));
            tabItem.Foreground = new SolidColorBrush(Colors.White);

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
                tc.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1A1A1A"));
                tc.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1A1A1A"));
            }
            // tc.BorderBrush = new SolidColorBrush(_border);

            var itemStyle = new Style(typeof(TabItem));
            // itemStyle.Setters.Add(new Setter(Control.BorderBrushProperty, new SolidColorBrush(_border)));
            itemStyle.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.White)));
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
            tv.Foreground = new SolidColorBrush(Colors.White);

            if (Application.Current.Resources["TreeViewStyle1"] is Style treeStyle)
                tv.Style = treeStyle;
            tv.ItemContainerStyle = (Style)Application.Current.Resources["TreeViewItemStyle2"];

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
                            tvi.Foreground = new SolidColorBrush(Colors.White);
                            StyleTreeViewItem(tvi);
                        }
                    }
                }
            };
        }

        private void StyleTreeViewItem(TreeViewItem item)
        {
            item.Style = (Style)Application.Current.Resources["TreeViewItemStyle2"];

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
            editor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#181818"));
            editor.Foreground = new SolidColorBrush(Colors.White);

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
                    if (b.Name == "SideBar")
                    {
                        b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1D1D1D"));
                        b.BorderBrush = new SolidColorBrush(Colors.Black);
                        break;
                    }
                    if(b.Name == "DockPanel")
                    {
                        b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF212121"));
                        b.BorderBrush = new SolidColorBrush(Colors.Black);
                        break;
                    }
                    if(b.Name == "ContentPanel")
                    {
                        b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1A1A1A"));
                        b.BorderBrush = new SolidColorBrush(Colors.Black);
                        break;
                    }
                    if( b.Name == "tx_border")
                    {
                        b.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF323031"));
                        b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#181818"));
                        break;
                    }
                    if(b.Tag != null && b.Tag.ToString() == "ContextMenu")
                    {
                        b.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4B4B4B"));
                        b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF222222"));
                        break;
                    }
                    if (b.Tag != null) if (b.Tag.ToString() == "resizeElement") break;
                    b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1A1A1A"));
                    b.BorderBrush = new SolidColorBrush(Colors.Black);
                    break;

                case TextBox tb:
                    tb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF292929"));
                    tb.Foreground = new SolidColorBrush(Colors.White);
                    tb.SelectionBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1C6BDE"));
                    tb.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    break;

                case TextBlock tbl:
                    tbl.Foreground = Brushes.Gray;
                    break;

                case Image i:
                    if (i.Tag != null && i.Tag.ToString() == "FirstImage")
                    {
                        i.Source = new BitmapImage(new Uri("Images\\other\\code_optimization_icon_iconscom_53810_negate.png", UriKind.Relative));
                    }
                    break;

                case ComboBox cb:
                    cb.Style = (Style)Application.Current.Resources["comboboxstyle"];
                    cb.ItemContainerStyle = (Style)Application.Current.Resources["ComboBoxDark"];

                    cb.Background = Brushes.Gray;
                    cb.Foreground = new SolidColorBrush(Colors.White);
                    break;

                case Label lbl:
                    if(lbl.Tag != null && lbl.Tag.ToString() == "ControlLabel")
                    {
                        lbl.Foreground = Brushes.White;
                        break;
                    }
                    lbl.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9A9A9A"));
                    break;

                case DataGrid dg:
                    //dg.Background = Brushes.White;
                    //dg.Foreground = new SolidColorBrush(_foreground);
                    //dg.BorderBrush = new SolidColorBrush(_border);
                    //dg.RowBackground = Brushes.White;
                    //dg.AlternatingRowBackground = new SolidColorBrush(Color.FromArgb(10, 0, 0, 0));
                    break;

                case GridSplitter gs:
                    if (gs.Tag != null && gs.Tag.ToString() == "SystemSplitter")
                        gs.Background = new SolidColorBrush(Colors.White);
                    break;
            }

        }
    }
}
