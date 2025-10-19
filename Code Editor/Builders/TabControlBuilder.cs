using Code_Editor.Classes;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Code_Editor.Modules
{
    internal class TabControlBuilder
    {
        string current_folder = "";
        Functions func = new Functions();
        public TabControlBuilder() { }

        public Grid Create(TreeView ViewXMLTags, int count, string current_folder, List<TabControlModel> tabCModels, Action<int> RestoreControls, Label PathFile, Action<object, MouseButtonEventArgs> EnableContextMenu, int font_size, string currentTheme)
        {
            Grid gr = new Grid();

            gr.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }); // TabControl
            gr.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(31) });

            for (int i = 0; i < count; i++)
            {
                int currentColumn = i;

                TabControl TabC = new TabControl();
                this.current_folder = current_folder;

                gr.ColumnDefinitions.Add(new ColumnDefinition());

                TabC.SetValue(Grid.ColumnProperty, i);
                TabC.SetValue(Grid.RowProperty, 0);
                gr.Children.Add(TabC);

                TabC.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1A1A1A"));
                TabC.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1A1A1A"));
                if(i > 0) TabC.Style = (Style)Application.Current.Resources["TabControlStyle4"];  
                else TabC.Style = (Style)Application.Current.Resources["TabControlStyle1"];
                TabC.Margin = new Thickness(0, 0, 0, -15);

                Border stats = new Border()
                {
                    Height = 23,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1A1A1A")),
                    CornerRadius = new CornerRadius(0, 0, 10, 0),
                };

                Grid statsGrid = new Grid();
                statsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                statsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                WrapPanel leftPanel = new WrapPanel() { Margin = new Thickness(14, 0, 0, 0) };
                Label linesCountLabel = new Label()
                {
                    Tag = "ControlLabel",
                    Name = "LinesCount",
                    Content = "Lines: 0",
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Segoe UI Variable Text Semibold"),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = double.NaN,
                    Height = 29,
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                Label positionCountLabel = new Label()
                {
                    Tag = "ControlLabel",
                    Name = "PositionCount",
                    Content = "Position: 0,0",
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Segoe UI Variable Text Semibold"),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = double.NaN,
                    Height = 29,
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                leftPanel.SetValue(Grid.ColumnProperty, 0);

                leftPanel.Children.Add(linesCountLabel);
                leftPanel.Children.Add(positionCountLabel);

                WrapPanel rightPanel = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 14, 0) };
                Label languageLabel = new Label()
                {
                    Tag = "ControlLabel",
                    Name = "Language_label",
                    Content = "Language",
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Segoe UI Variable Text Semibold"),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = double.NaN,
                    Height = 31,
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                rightPanel.Children.Add(languageLabel);

                rightPanel.SetValue(Grid.ColumnProperty, 1);

                statsGrid.Children.Add(leftPanel);
                statsGrid.Children.Add(rightPanel);
                stats.Child = statsGrid;

                stats.SetValue(Grid.ColumnProperty, i);
                stats.SetValue(Grid.RowProperty, 1);
                gr.Children.Add(stats);


                if (i > 0)
                {
                    GridSplitter gridSplitter = new GridSplitter();

                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Left;
                    gridSplitter.Width = 5;
                    gridSplitter.Margin = new Thickness(-2, 0, 0, 0);
                    gridSplitter.Cursor = Cursors.SizeWE;
                    gridSplitter.Opacity = 0;
                    gridSplitter.SetValue(Grid.ColumnProperty, i);

                    Rectangle line = new Rectangle();
                    line.Width = 3;
                    line.Height = 400;
                    line.Cursor = Cursors.SizeWE;
                    line.RadiusX = 5;
                    line.RadiusY = 5;
                    line.Margin = new Thickness(-2, 0, 0, 0);
                    line.Stroke = new SolidColorBrush(Colors.Transparent);
                    line.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3A3A3A"));
                    line.StrokeThickness = 1;
                    line.HorizontalAlignment = HorizontalAlignment.Left;
                    line.SetValue(Grid.ColumnProperty, i);
                    line.IsHitTestVisible = false;
                    line.RenderTransformOrigin = new Point(0.5, 0.5);
                    line.RenderTransform = new ScaleTransform(1, 1);

                    gr.Children.Add(gridSplitter);
                    gr.Children.Add(line);

                    Functions functions = new Functions();

                    Storyboard st = new Storyboard();
                    st = functions.Animation(line, st, TimeSpan.FromMilliseconds(200), Colors.White, 1, 1, 0, 0, 1, 1, 0, 0, false, null, 0, 0);

                    Storyboard st2 = new Storyboard();
                    st2 = functions.Animation(line, st2, TimeSpan.FromMilliseconds(200), (Color)ColorConverter.ConvertFromString("#FF3A3A3A"), 1, 1, 0, 0, 1, 1, 0, 0, false, null, 0, 0);

                    gridSplitter.MouseMove += (s, e) =>
                    {
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            st.Begin();
                        }
                        else
                        {
                            st2.Begin();
                        }
                    };

                }

                if (currentTheme == "light")
                {
                    Color _background = Color.FromRgb(196, 196, 196);
                    Color _foreground = Colors.Black;
                    Color _accent = Color.FromRgb(0, 122, 204);
                    Color _border = Color.FromRgb(204, 204, 204);

                    TabC.Background = Brushes.White;
                    TabC.BorderBrush = new SolidColorBrush(_border);

                    var itemStyle = new Style(typeof(TabItem));
                    itemStyle.Setters.Add(new Setter(Control.BorderBrushProperty, new SolidColorBrush(_border)));
                    itemStyle.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(_foreground)));
                    itemStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.SemiBold));

                    TabC.ItemContainerStyle = itemStyle;

                    linesCountLabel.Foreground = Brushes.Black;
                    languageLabel.Foreground = Brushes.Black;
                    positionCountLabel.Foreground = Brushes.Black;

                    stats.Background = Brushes.White;
                }

                //TabC.Drop += (es, se) =>
                //{

                //};

                Border dragIndicator = null;
                TabC.AllowDrop = true;

                TabC.DragOver += (se, es) =>
                    {
                        if (es.Data.GetDataPresent(typeof(TabItem)))
                        {
                            if (dragIndicator == null)
                            {
                                dragIndicator = new Border
                                {
                                    Width = 2,
                                    Height = 25,
                                    Background = Brushes.White,
                                    CornerRadius = new CornerRadius(10),
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness(0, 7, 0, 0)
                                };

                                if(currentTheme == "light")
                                {
                                    dragIndicator.Background = Brushes.Black;
                                }

                                dragIndicator.SetValue(Grid.ColumnProperty, currentColumn);
                                //if (TabC.Parent is Grid parentGrid && !parentGrid.Children.Contains(dragIndicator))
                                //{
                                //    parentGrid.Children.Add(dragIndicator);
                                //}

                                gr.Children.Add(dragIndicator);
                            }

                            Point dropPosition = es.GetPosition(TabC);

                            int insertIndex = TabC.Items.Count;
                            double indicatorX = 0;

                            for (int j = 0; j < TabC.Items.Count; j++)
                            {
                                TabItem tab = TabC.ItemContainerGenerator.ContainerFromIndex(j) as TabItem;
                                if (tab != null)
                                {
                                    Point tabPosition = tab.TransformToAncestor(TabC).Transform(new Point(0, 0));
                                    if (dropPosition.X < tabPosition.X + tab.ActualWidth / 2)
                                    {
                                        insertIndex = j;
                                        indicatorX = tabPosition.X;
                                        break;
                                    }
                                    indicatorX = tabPosition.X + tab.ActualWidth;
                                }
                            }

                            TranslateTransform transform = dragIndicator.RenderTransform as TranslateTransform;
                            if (transform == null)
                            {
                                transform = new TranslateTransform();
                                dragIndicator.RenderTransform = transform;
                            }
                            transform.X = indicatorX;

                        }
                    };

                TabC.Drop += (se, es) =>
                    {
                        if (es.Data.GetDataPresent(typeof(TabItem)))
                        {
                            TabItem droppedTab = es.Data.GetData(typeof(TabItem)) as TabItem;
                            if (droppedTab == null)
                                return;

                            Point dropPosition = es.GetPosition(gr); 

                            int insertIndex = TabC.Items.Count;
                            double indicatorX = 0;

                            for (int j = 0; j < TabC.Items.Count; j++)
                            {
                                TabItem tab = TabC.ItemContainerGenerator.ContainerFromIndex(j) as TabItem;
                                if (tab != null)
                                {
                                    Point tabPosition = tab.TransformToAncestor(gr).Transform(new Point(0, 0)); 

                                    if (dropPosition.X < tabPosition.X + tab.ActualWidth / 2)
                                    {
                                        insertIndex = j;
                                        indicatorX = tabPosition.X;
                                        break;
                                    }
                                    indicatorX = tabPosition.X + tab.ActualWidth;
                                }
                            }

                            var oldTabControl = ItemsControl.ItemsControlFromItemContainer(droppedTab) as TabControl;
                            if (oldTabControl != null && oldTabControl != TabC)
                            {
                                foreach (TabItem tempTab in TabC.Items)
                                {
                                    if (tempTab.Tag == droppedTab.Tag)
                                    {
                                        oldTabControl.Items.Remove(droppedTab);

                                        TabControlModel tabControlremove2 = new TabControlModel();
                                        foreach (var tabControl in tabCModels)
                                        {
                                            if (tabControl.tb == oldTabControl)
                                            {
                                                foreach (var tabItem in tabControl.TabItemModel)
                                                {
                                                    if (tabItem.tab == droppedTab)
                                                    {
                                                        tabControl.TabItemModel.Remove(tabItem);
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        if (dragIndicator != null)
                                        {
                                            if (TabC.Parent is Grid parentGrid && parentGrid.Children.Contains(dragIndicator))
                                            {
                                                parentGrid.Children.Remove(dragIndicator);
                                            }
                                            dragIndicator = null;
                                        }

                                        return;
                                    }
                                }

                                oldTabControl.Items.Remove(droppedTab);

                                TabControlModel tabControlremove = new TabControlModel();
                                foreach (var tabControl in tabCModels)
                                {
                                    if (tabControl.tb == oldTabControl)
                                    {
                                        foreach(var tabItem in tabControl.TabItemModel)
                                        {
                                            if (tabItem.tab == droppedTab)
                                            {
                                                tabControl.TabItemModel.Remove(tabItem);
                                                break;
                                            }
                                        }
                                    }
                                }
                                // tabCModels.Remove(tabControlremove);
                            }
                            else if (TabC.Items.Contains(droppedTab)) 
                            {
                                int oldIndex = TabC.Items.IndexOf(droppedTab);
                                TabC.Items.Remove(droppedTab);
                                if (oldIndex < insertIndex)
                                    insertIndex--;
                            }

                            TabC.Items.Insert(insertIndex, droppedTab);

                            foreach (var tabControl in tabCModels)
                            {
                                if (tabControl.tb == TabC)
                                {
                                    bool exists = false;
                                    foreach (var tabItemModel in tabControl.TabItemModel)
                                    {
                                        if (tabItemModel.tab == droppedTab)
                                        {
                                            exists = true;
                                            break;
                                        }
                                    }

                                    bool IMG = false;
                                    if (System.IO.Path.GetExtension(droppedTab.Tag.ToString()) == ".png" || System.IO.Path.GetExtension(droppedTab.Tag.ToString()) == ".jpg" || System.IO.Path.GetExtension(droppedTab.Tag.ToString()) == ".jpeg" || System.IO.Path.GetExtension(droppedTab.Tag.ToString()) == ".webp")
                                    {
                                        IMG = true;
                                    }

                                    if (!exists)
                                    {
                                        tabControl.TabItemModel.Insert(insertIndex, new TabItemModel()
                                        {
                                            tab = droppedTab,
                                            FullPath = droppedTab.Tag?.ToString() ?? "",
                                            isImg = IMG
                                        });
                                    }
                                    else
                                    {
                                        tabControl.TabItemModel.RemoveAll(m => m.tab == droppedTab);

                                        tabControl.TabItemModel.Insert(insertIndex, new TabItemModel()
                                        {
                                            tab = droppedTab,
                                            FullPath = droppedTab.Tag?.ToString() ?? "",
                                            isImg = IMG
                                        });
                                    }

                                    break;
                                }
                            }

                            TabC.SelectedItem = droppedTab;
                        }

                        if (es.Data.GetDataPresent("CustomTreeViewItem"))
                        {
                            TreeViewItem selectedItem = es.Data.GetData("CustomTreeViewItem") as TreeViewItem;
                            if (selectedItem != null)
                            {
                                var selectedTag = selectedItem.ToString();

                                TabBuilder tb1 = new TabBuilder(ViewXMLTags, tabCModels[currentColumn], tabCModels, null, PathFile, current_folder);
                                tb1.Create(false, RestoreControls, font_size, currentTheme);
                            }
                        }

                        if (dragIndicator != null)
                        {
                            if (TabC.Parent is Grid parentGrid && parentGrid.Children.Contains(dragIndicator))
                            {
                                parentGrid.Children.Remove(dragIndicator);
                            }
                            dragIndicator = null;
                        }
                    };

                TabC.DragLeave += (se, es) =>
                {
                    if (dragIndicator != null)
                    {
                        if (TabC.Parent is Grid parentGrid && parentGrid.Children.Contains(dragIndicator))
                        {
                            parentGrid.Children.Remove(dragIndicator);
                        }
                        dragIndicator = null;
                    }
                };

                TabC.MouseRightButtonDown += (se, es) =>
                {
                    EnableContextMenu(se, es);
                };

                TabControlModel tbm = new TabControlModel();
                tbm.tb = TabC;
                tbm.languageLabel = languageLabel;
                tbm.linesCountLabel = linesCountLabel;
                tbm.positionCountLabel = positionCountLabel;

                languageLabel.Visibility = Visibility.Hidden;
                linesCountLabel.Visibility = Visibility.Hidden;
                positionCountLabel.Visibility = Visibility.Hidden;
                tbm.ID = i;

                tabCModels.Add(tbm);
            }


            return gr;
        }
    }
}
