
using Code_Editor.Classes;
using Code_Editor.Manager;
using Code_Editor.Modules;
using Code_Editor.Properties;
using Examination_SP;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using static ICSharpCode.AvalonEdit.Document.TextDocumentWeakEventManager;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;



namespace Code_Editor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// firstTab
    public partial class MainWindow : Window
    {
        //-----------------Modules----------------
        EffectBlur ef = null;
        AI ai = new AI("YOUR_API_KEY"); // Возможно будет не работать, я проверял последний раз полгода назад
        Functions func = new Functions();
        PlugInManager plugInManager;
        SaveLoad sl;

        ContextMenuManager cmm;


        //----------------Данные--------------------
        AppState appst = new AppState();


        //==============Сохранение состояния TreeView================
        TreeViewPath tvp = new TreeViewPath();

        //--------------Список всех TabControl------------------------
        List<TabControlModel> tabCModels = new List<TabControlModel>();

        static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string myAppFolder = System.IO.Path.Combine(appDataPath, "CodeEditor");

        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                File.WriteAllText("error_log.txt", ex.ToString());
                System.Windows.MessageBox.Show(ex.ToString());
                throw;
            }
            Directory.CreateDirectory(myAppFolder);
            appst.CurrentTheme = "dark";
            appst.CurrentFolder = null;
            appst.isBlur = false;
            appst.isExpandedSidebar = true;
            // appst.Setting = "1";
            appst.FontSize = 20;
            appst.IsLoaded = false;
            

            Directory.CreateDirectory(myAppFolder + "\\Plug-ins");

            plugInManager = new PlugInManager(this);

            sl = new SaveLoad(this, ViewXMLTags, MainGrid, appst, tabCModels, tvp);

            ef = new EffectBlur(this);
            // ef.EnableBlur();

            //ResizePanels r = new ResizePanels(this);
            //r.Handler(ResizeUp, "Up");
            //r.Handler(ResizeDown, "Down");
            //r.Handler(ResizeLeft, "Left");
            //r.Handler(ResizeRight, "Right");
            //r.Handler(ResizeLeftTop, "LeftTop");
            //r.Handler(ResizeRightTop, "RightTop");
            //r.Handler(ResizeLeftDown, "LeftDown");
            //r.Handler(ResizeRightDown, "RightDown");

            LoadData();
            sl.RestoreTreeViewState(ViewXMLTags);

            ThemesComboBox.ItemsSource = plugInManager.GetPlugInsNames();

            if (appst.CurrentTheme == "light") ThemesComboBox.SelectedIndex = 0;
            else if (appst.CurrentTheme == "dark");

            cmm = new ContextMenuManager(appst, tabCModels, tvp, ViewXMLTags, sl, this);

            this.Focus();
        }

        #region window_controls

        private void SideBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { if (e.ButtonState == MouseButtonState.Pressed) this.DragMove(); }
        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { if (e.ButtonState == MouseButtonState.Pressed) this.DragMove(); }

        private void MinWindowControl1_Click(object sender, RoutedEventArgs e) 
        {
            AnimateMinimize();
        }
        private void ExitWindowControl1_Click(object sender, RoutedEventArgs e) { AnimateWindowResizeAndFade(true);}

        private void FileManagerButton_Click(object sender, RoutedEventArgs e)
        {
            TitleSideBar.Content = "Файлы";
        }

        bool isMax = false;

        private void MaxWindowControl1_Click(object sender, RoutedEventArgs e)
        {
            if (!isMax)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
                isMax = true;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
                isMax = false;
            }
        }

        #endregion

        #region AnimationWindow

        private bool _isSidebarCollapsed = true;

        private void AnimateMinimize()
        {
            var storyboard = new Storyboard();
            Timeline.SetDesiredFrameRate(storyboard, 144);
            ef?.DisableBlur();

            var opacityAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(opacityAnimation, this);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Window.OpacityProperty));
            storyboard.Children.Add(opacityAnimation);

            var scaleXAnimation = new DoubleAnimation
            {
                To = 0.8,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(scaleXAnimation, this);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleX)"));
            storyboard.Children.Add(scaleXAnimation);

            var scaleYAnimation = new DoubleAnimation
            {
                To = 0.8,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(scaleYAnimation, this);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleY)"));
            storyboard.Children.Add(scaleYAnimation);

            storyboard.Completed += (s, ev) =>
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            };

            storyboard.Begin();
        }

        private void AnimateRestore()
        {
            this.Opacity = 0;
            if (appst.isBlur)  ef?.EnableBlur();
            (this.RenderTransform as ScaleTransform).ScaleX = 0.8;
            (this.RenderTransform as ScaleTransform).ScaleY = 0.8;

            var storyboard = new Storyboard();
            Timeline.SetDesiredFrameRate(storyboard, 144);

            var opacityAnimation = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(opacityAnimation, this);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Window.OpacityProperty));
            storyboard.Children.Add(opacityAnimation);

            var scaleXAnimation = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(scaleXAnimation, this);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleX)"));
            storyboard.Children.Add(scaleXAnimation);

            var scaleYAnimation = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(scaleYAnimation, this);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleY)"));
            storyboard.Children.Add(scaleYAnimation);

            storyboard.Begin();
        }


        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                AnimateRestore();
            }
        }

        private void AnimateSidebar(bool collapse)
        {
            var animation = new GridLengthAnimation
            {
                From = new GridLength(collapse ? SideBar.ActualWidth : 0),
                To = new GridLength(collapse ? 0 : 300),
                Duration = TimeSpan.FromMilliseconds(600),
                EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseInOut
                }
            };

            animation.Completed += (s, e) =>
            {
                LeftColumn.BeginAnimation(ColumnDefinition.WidthProperty, null);
                LeftColumn.Width = collapse ? new GridLength(0) : new GridLength(300);
            };

            LeftColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
        }

        private void ToggleSidebar()
        {
            AnimateSidebar(_isSidebarCollapsed);
            _isSidebarCollapsed = !_isSidebarCollapsed;
            appst.isExpandedSidebar = !appst.isExpandedSidebar;
        }

        private void AnimateWindowResizeAndFade(bool isShrinking)
        {
            if (appst.isBlur) ef.DisableBlur();

            var storyboard = new Storyboard();
            Timeline.SetDesiredFrameRate(storyboard, 144);
            TimeSpan totalDuration = TimeSpan.FromSeconds(0.6);

            var opacityAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(opacityAnimation, MainGrid);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            storyboard.Children.Add(opacityAnimation);

            var scaleXAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(scaleXAnimation, MainGrid);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleX)"));
            scaleXAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.Zero)));

            //scaleXAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(1.05, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25)),
            //    new KeySpline(0.5, 0, 0.5, 1)));

            scaleXAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(0.8, KeyTime.FromTimeSpan(totalDuration),
                new KeySpline(0.5, 0, 0.5, 1)));
            storyboard.Children.Add(scaleXAnimation);


            var scaleYAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(scaleYAnimation, MainGrid);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleY)"));
            scaleYAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            //scaleYAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(1.05, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25)),
            //    new KeySpline(0.5, 0, 0.5, 1)));
            scaleYAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(0.8, KeyTime.FromTimeSpan(totalDuration),
                new KeySpline(0.5, 0, 0.5, 1)));
            storyboard.Children.Add(scaleYAnimation);


            storyboard.Completed += (s, ev) =>
            {
                SaveData();
                this.Close();
            };
            storyboard.Begin();
        }



        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            MainGrid.RenderTransformOrigin = new Point(0.5, 0.5);
            var scaleTransform = new ScaleTransform(0.8, 0.8);
            MainGrid.RenderTransform = scaleTransform;

            var storyboard = new Storyboard();
            Timeline.SetDesiredFrameRate(storyboard, 144);


            TimeSpan totalDuration = TimeSpan.FromSeconds(0.6);


            var opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Storyboard.SetTarget(opacityAnimation, MainGrid);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            storyboard.Children.Add(opacityAnimation);


            var scaleXAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(scaleXAnimation, MainGrid);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleX)"));


            scaleXAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(0.8, KeyTime.FromTimeSpan(TimeSpan.Zero)));


            //scaleXAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(1.05, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5)),
            //     new KeySpline(0.5, 0, 0.5, 1)));


            scaleXAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(1, KeyTime.FromTimeSpan(totalDuration),
                new KeySpline(0.5, 0, 0.5, 1)));
            storyboard.Children.Add(scaleXAnimation);


            var scaleYAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(scaleYAnimation, MainGrid);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleY)"));
            scaleYAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(0.8, KeyTime.FromTimeSpan(TimeSpan.Zero)));


            //scaleYAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(1.05, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5)),
            //    new KeySpline(0.5, 0, 0.5, 1)));
            scaleYAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(1, KeyTime.FromTimeSpan(totalDuration),
                new KeySpline(0.5, 0, 0.5, 1)));
            storyboard.Children.Add(scaleYAnimation);

            storyboard.Completed += (s, ev) =>
            {
                if(appst.isBlur) ef.EnableBlur();
            };
            storyboard.Begin();
        }

        #endregion AnimationWindow

        //==============Сохранение вкладок и настроек в файл==================
        void SaveData()
        {
            sl.Save();
        }

        //==============Загрузка предыдущих вкладок и настроек==================
        void LoadData()
        {
            sl.Load(PathFile, CreateControl, CreateTab, FontConf, SideBar, DockPanel, ContentPanel, SelectProjectButton, BlurToggle, ef, wrap_panel);

            if (appst.CurrentFolder ==  null)
            {
                CloseProjectButton.Visibility = Visibility.Hidden;
                SaveButton.Visibility = Visibility.Hidden;

                UndoB.Visibility = Visibility.Hidden;
                RedoB.Visibility = Visibility.Hidden;
                SplitButton.Visibility = Visibility.Hidden;
                firstTab.Visibility = Visibility.Visible;
            }
            if (appst.isExpandedSidebar == false) _isSidebarCollapsed = !_isSidebarCollapsed;
        }


        //==============Выбор и загрузка проекта===========================
        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            sl.Load_Button(SelectProjectButton, gridcol, DeleteMainBorder);

            if (appst.CurrentFolder != null)
            {
                PathFile.Content = "";
                CloseProjectButton.Visibility = Visibility.Visible;
                SaveButton.Visibility = Visibility.Visible;
                UndoB.Visibility = Visibility.Visible;
                RedoB.Visibility = Visibility.Visible;
                SplitButton.Visibility = Visibility.Visible;
                firstTab.Visibility = Visibility.Hidden;
            }
        }


        //===============Сохранить все вкладки=============================
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            sl.SaveButton();
        }


        //===============Удаление текущих TabControl======================
        private void DeleteMainBorder()
        {
            UIElement elementToRemove = null;
            foreach (UIElement element in gridcol.Children)
            {
                if (element is Border b && b.Tag == "BGrid")
                {
                    elementToRemove = b;
                    break;
                }
            }

            if (elementToRemove != null)
            {
                gridcol.Children.Remove(elementToRemove);
            }
        }

        

        #region scrollBar

        double _targetOffset = 0;
        double ScrollSpeed = 50;

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer_PreviewMouseWheel2(sender, e);
        }

        private void ScrollViewer_PreviewMouseWheel2(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                e.Handled = true;

                _targetOffset = Math.Max(0, Math.Min(scrollViewer.ScrollableHeight, _targetOffset - e.Delta / 120 * ScrollSpeed));

                var animation = new DoubleAnimation
                {
                    To = _targetOffset,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                scrollViewer.BeginAnimation(ScrollViewerOffsetHelper.VerticalOffsetProperty, animation);
            }
        }

        // TextEditor

        #endregion
        

        //==============Создание уведомлений============================
        void CreateNotification(int pos, string label_s)
        {
            // NotificationBuilder nb = new NotificationBuilder();
            //nb.Create(label_s, notificationGrid, pos);
        }


        //=====================Создание одного TabControl=========================
        public void CreateControl(int count)
        {
            DeleteMainBorder();

            Border border = new Border();
            border.Tag = "BGrid";
            border.BorderThickness = new Thickness(0);
            border.CornerRadius = new CornerRadius(20);

            TabControlBuilder tbc = new TabControlBuilder();
            Grid g = tbc.Create(ViewXMLTags, count, appst.CurrentFolder, tabCModels, RestoreControls, PathFile, EnableContextMenu, appst.FontSize, appst.CurrentTheme);

            border.Child = g;
            gridcol.Children.Add(border);
        }

        //=====================Создание одного TabItem=========================
        public void CreateTab(TreeView ViewXMLTags, TabControlModel tabC, List<TabControlModel> tabCModels, TabItemModel tab, Label PathFile, string currentFolder, bool isReturn)
        {
            if (tabCModels.Count == 0) CreateControl(1);

            TabBuilder tb = new TabBuilder(ViewXMLTags, tabC, tabCModels, tab, PathFile, currentFolder);
            tb.Create(isReturn, RestoreControls, appst.FontSize, appst.CurrentTheme);

        }

        //=====================Выбор файла в TreeView=========================
        private void ViewXMLTags_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tabCModels.Count == 0)
            {
                DeleteMainBorder();
                CreateControl(1);
            }
            CreateTab(ViewXMLTags, tabCModels[0], tabCModels, null, PathFile, appst.CurrentFolder, false);
        }


        // Переназначение индексов после удаления одного из нескольких TabControl
        private void RestoreControls(int count)
        {
            DeleteMainBorder();
            if(count == 0) return;

            int counterID = 0;
            List<TabControlModel> New_tabCModels = new List<TabControlModel>();
            foreach (var Tabc in tabCModels)
            {
                TabControlModel new_Tabc = new TabControlModel();
                foreach (var tab in Tabc.TabItemModel)
                {
                    TabItemModel new_tab = new TabItemModel();
                    new_tab.FullPath = tab.FullPath;
                    new_tab.tab = tab.tab;
                    new_tab.isImg = tab.isImg;

                    new_Tabc.TabItemModel.Add(new_tab);
                }
                new_Tabc.ID = counterID;
                new_Tabc.languageLabel = Tabc.languageLabel;
                new_Tabc.linesCountLabel = Tabc.linesCountLabel;
                new_Tabc.positionCountLabel = Tabc.positionCountLabel;
                new_Tabc.tb = Tabc.tb;

                counterID++;

                New_tabCModels.Add(new_Tabc);
            }

            tabCModels.Clear();

            CreateControl(count);


            for (int i = 0; i < tabCModels.Count; i++)
            {
                TabControlModel tabctrl = tabCModels[i];
                TabControlModel tabctrlnew = New_tabCModels[i];

                for (int j = 0; j < tabctrlnew.TabItemModel.Count; j++)
                {
                    TabItemModel tab = tabctrlnew.TabItemModel[j];
                    CreateTab(ViewXMLTags, tabctrl, tabCModels, tab, PathFile, appst.CurrentFolder, true);
                }
            }

            New_tabCModels.Clear();
        }


        //=============Скопировать вклаки на новый TabControl========================
        private void ReturnTabsOnSplit(SaveTabs st, TabItemModel tab)
        {
            ReturnControls(st, tab);
            CreateTab(ViewXMLTags, tabCModels[st.tabCModels.Count], tabCModels, tab, PathFile, appst.CurrentFolder, true);
        }


        // Загрузка TabControls при запуске
        private void ReturnControls(SaveTabs st)
        {
            if (st.tabCModels.Count == 0) return;
            CreateControl(st.tabCModels.Count);

            for (int i = 0; i < st.tabCModels.Count; i++)
            {
                var Tabc = st.tabCModels[i];
                foreach (var tab in Tabc.TabItemModel) CreateTab(ViewXMLTags, tabCModels[i], tabCModels, tab, PathFile, appst.CurrentFolder, true);
            }
        }

         private void ReturnControls(SaveTabs st, TabItemModel tb)
        {
            if (st.tabCModels.Count == 0) return;
            CreateControl(st.tabCModels.Count + 1);

            for (int i = 0; i < st.tabCModels.Count; i++)
            {
                var Tabc = st.tabCModels[i];
                foreach (var tab in Tabc.TabItemModel) CreateTab(ViewXMLTags, tabCModels[i], tabCModels, tab, PathFile, appst.CurrentFolder, true);
            }
        }


        private void BlurToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (SideBar != null && DockPanel != null)
            {
                func.AnimateOpacity(SideBar, 0.4, null);
                func.AnimateOpacity(DockPanel, 0.9, null);
                ef?.EnableBlur();

                if(appst != null) appst.isBlur = true;
            }
        }

        private void BlurToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (SideBar != null && DockPanel != null)
            {
                func.AnimateOpacity(SideBar, 1, null);
                func.AnimateOpacity(DockPanel, 1, null);
                ef?.DisableBlur();

                if (appst != null) appst.isBlur = false;
            }
        }

        //============Регулировка прозрачности======================
        /*private void SliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Math.Abs(SideBar.Opacity - e.NewValue) < 0.01)
                return;

            if (e.NewValue >= 0.3) appst.Setting = func.AnimateOpacity(SideBar, e.NewValue, appst.Setting);
            if (e.NewValue >= 0.9) appst.Setting = func.AnimateOpacity(DockPanel, e.NewValue, appst.Setting);
            if (e.NewValue >= 0.95) appst.Setting = func.AnimateOpacity(ContentPanel, e.NewValue, appst.Setting);

            if(e.NewValue <= 0.1 && ef != null) ef.DisableBlur();
            else if(e.NewValue >= 0.1 && ef != null) ef.EnableBlur();
        }

        //============Кнопка регулировки прозрачности======================
        private void TransparentButton_Click(object sender, RoutedEventArgs e)
        {
            OpacityBorder.Visibility = Visibility.Visible;

            Storyboard st = new Storyboard();
            st = func.Animation(OpacityBorder, st, TimeSpan.FromSeconds(0.6), null, 0, 0, 0, 0, 1, 1, 0, 80, false, null, 0, 0);
            st.Begin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard st = new Storyboard();
            st = func.Animation(OpacityBorder, st, TimeSpan.FromSeconds(0.6), null, 1, 1, 0, 80, 0, 0, 0, 0, false, null, 0, 0);
            st.Completed += (se, es) =>
            {
                OpacityBorder.Visibility = Visibility.Hidden;
            };

            st.Begin();
        }
        */


        private void MainGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ContextMenuCustom.Visibility != Visibility.Visible && ContextMenuCustomFiles.Visibility != Visibility.Visible && ContextMenuCustomFolders.Visibility != Visibility.Visible)
                return;

            if (!IsClickInsideElement(ContextMenuCustom, e))
            {
                ContextMenuCustom.Visibility = Visibility.Hidden;
                ContextMenuCustomFiles.Visibility = Visibility.Hidden;
                ContextMenuCustomFolders.Visibility = Visibility.Hidden;
            }
        }

        private bool IsClickInsideElement(FrameworkElement element, MouseButtonEventArgs e)
        {
            Point clickPos = e.GetPosition(element);
            return clickPos.X >= 0 && clickPos.X <= element.ActualWidth &&
                   clickPos.Y >= 0 && clickPos.Y <= element.ActualHeight;
        }

        void EnableContextMenu(object s, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(MainGrid);

            ContextMenuTransform.X = mousePos.X;
            ContextMenuTransform.Y = mousePos.Y;

            ContextMenuCustom.Visibility = Visibility.Visible;
            ContextMenuCustomFiles.Visibility = Visibility.Hidden;
        }


        public enum Operation
        {
            Undo,
            Redo,
            Copy,
            Paste,
            Cut,
            Delete,
            Rename,
            CreateFile,
            CreateFolder,
            CopyPath,
        }


        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTab(Operation.Copy, ContextMenuCustom);
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTab(Operation.Paste, ContextMenuCustom);
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTab(Operation.Cut, ContextMenuCustom);
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTab(Operation.Undo, ContextMenuCustom);
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTab(Operation.Redo, ContextMenuCustom);
        }

        private void ViewXMLTags_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(MainGrid);

            TreeViewItem item = ViewXMLTags.SelectedItem as TreeViewItem;

            if (Directory.Exists(func.GetFullPath(item, appst.CurrentFolder)))
            {
                ContextMenuFoldersTransform.X = mousePos.X;
                ContextMenuFoldersTransform.Y = mousePos.Y;

                ContextMenuCustomFolders.Visibility = Visibility.Visible;
                ContextMenuCustomFiles.Visibility = Visibility.Hidden;
            }
            else
            {
                ContextMenuFilesTransform.X = mousePos.X;
                ContextMenuFilesTransform.Y = mousePos.Y;

                ContextMenuCustomFiles.Visibility = Visibility.Visible;
                ContextMenuCustomFolders.Visibility = Visibility.Hidden;
            }
            ContextMenuCustom.Visibility = Visibility.Hidden;
        }

        private void CopyFileButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTabFiles(Operation.Copy, false, HideContextMenus);
        }

        private void PasteFileButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTabFiles(Operation.Paste, false, HideContextMenus);
        }

        private void CutFileButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTabFiles(Operation.Cut, false, HideContextMenus);
        }

        private void DeleteFileButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTabFiles(Operation.Delete, false, HideContextMenus);
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            isCreate = false;
            isFile = true;

            RenameBorder.Visibility = Visibility.Visible;

            var st = new Storyboard();
            st = func.Animation(RenameBorder, st, TimeSpan.FromSeconds(0.6), null, 0, 0, 0, 0, 1, 1, 0, -15, true, scrollViewer, 0, 40);
            st.Begin();

            ContextMenuCustomFiles.Visibility = Visibility.Hidden;
        }

        private void RenameButtonFolder_Click(object sender, RoutedEventArgs e)
        {
            isCreate = false;
            isFile = false;

            RenameBorder.Visibility = Visibility.Visible;

            var st = new Storyboard();
            st = func.Animation(RenameBorder, st, TimeSpan.FromSeconds(0.6), null, 0, 0, 0, 0, 1, 1, 0, -15, true, scrollViewer, 0, 40);
            st.Begin();

            ContextMenuCustomFiles.Visibility = Visibility.Hidden;
        }

        private void CopyPathButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTabFiles(Operation.CopyPath, false, HideContextMenus);
        }


        bool isCreate = false;
        bool isFile = false;


        private void renameOk_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ViewXMLTags.SelectedItem as TreeViewItem;

            if (selectedItem != null)
            {
                try
                {
                    tvp.expandedPaths.Clear();
                    tvp.selectedPath = null;
                    sl.SaveTreeViewState(ViewXMLTags);
                    File.WriteAllLines(myAppFolder + "\\" + "expanded.txt", tvp.expandedPaths);
                    File.WriteAllText(myAppFolder + "\\" + "selected.txt", tvp.selectedPath ?? "");

                    if (isCreate)
                    {
                        if (isFile)
                        {
                            string fullPath = func.GetFullPath(selectedItem, appst.CurrentFolder) + "\\" + renametextbox.Text;
                            using (FileStream fs = File.Create(fullPath)) { };
                            // File.Create(GetFullPath(selectedItem) + "\\" + renametextbox.Text);
                        }
                        else
                        {
                            Directory.CreateDirectory(func.GetFullPath(selectedItem, appst.CurrentFolder) + "\\" + renametextbox.Text);
                        }
                    }
                    else
                    {
                        if (isFile)
                        {
                            string path = func.GetFullPath(selectedItem, appst.CurrentFolder);

                            ContextMenuCustomFiles.Visibility = Visibility.Hidden;

                            File.Copy(path, System.IO.Path.GetDirectoryName(path) + "\\" + renametextbox.Text);
                            File.Delete(path);
                        }
                        else
                        {
                            string path = func.GetFullPath(selectedItem, appst.CurrentFolder);

                            ContextMenuCustomFiles.Visibility = Visibility.Hidden;

                            func.CopyDirectory(path, System.IO.Path.GetDirectoryName(path) + "\\" + renametextbox.Text, true);
                            Directory.Delete(path, true);
                        }
                    }

                    ViewXMLTags.Items.Clear();

                    TreeViewItem item = new TreeViewItem();
                    if (appst.CurrentTheme == "light")
                    {
                        item.Header = System.IO.Path.GetFileName(appst.CurrentFolder);
                        item.Style = (Style)this.Resources["TreeViewItemStyleLight"];
                        item.Foreground = Brushes.Black;
                    }
                    else
                    {
                        item.Header = System.IO.Path.GetFileName(appst.CurrentFolder);
                        item.Style = (Style)this.Resources["TreeViewItemStyle2"];
                    }

                    //TreeViewItem item = new TreeViewItem { Header = System.IO.Path.GetFileName(currentFolder), Style = (Style)this.Resources["TreeViewItemStyle2"] };
                    item = func.Files(appst.CurrentFolder, item);

                    ViewXMLTags.Items.Add(item);

                    sl.RestoreTreeViewState(ViewXMLTags);
                }
                catch (Exception ex)
                {
                }
            }

            AnimationOff();
        }

        private void renamecancel_Click(object sender, RoutedEventArgs e)
        {
            AnimationOff();
        }

        void AnimationOff()
        {
            var st = new Storyboard();
            st = func.Animation(RenameBorder, st, TimeSpan.FromSeconds(0.6), null, 1, 1, 0, -15, 0, 0, 0, 0, true, scrollViewer, 40, 0);
            st.Completed += (se, es) =>
            {
                RenameBorder.Visibility = Visibility.Hidden;
            };

            st.Begin();
        }




        private void CreateFolderButton_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuCustomFolders.Visibility = Visibility.Hidden;

            isCreate = true;
            isFile = false;

            RenameBorder.Visibility = Visibility.Visible;
            var st = new Storyboard();
            st = func.Animation(RenameBorder, st, TimeSpan.FromSeconds(0.6), null, 0, 0, 0, 0, 1, 1, 0, -15, true, scrollViewer, 0, 40);
            st.Begin();

        }

        private void CreateFileButton_Click(object sender, RoutedEventArgs e)
        {
            isCreate = true;
            isFile = true;

            ContextMenuCustomFolders.Visibility = Visibility.Hidden;

            RenameBorder.Visibility = Visibility.Visible;
            var st = new Storyboard();
            st = func.Animation(RenameBorder, st, TimeSpan.FromSeconds(0.6), null, 0, 0, 0, 0, 1, 1, 0, -15, true, scrollViewer, 0, 40);
            st.Begin();

        }

        private void CopyFolderButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTabFiles(Operation.Copy, true, HideContextMenus);
        }

        private void PasteFolderButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTabFiles(Operation.Paste, true, HideContextMenus);
        }

        private void CutFolderButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTabFiles(Operation.Cut, true, HideContextMenus);
        }

        private void RenameFolderButton_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuCustomFolders.Visibility = Visibility.Hidden;

            RenameButtonFolder_Click(null, null);
        }

        private void DeleteFolderButton_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTabFiles(Operation.Delete, true, HideContextMenus);
        }

        private void UndoB_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTab(Operation.Undo, ContextMenuCustom);
        }

        private void RedoB_Click(object sender, RoutedEventArgs e)
        {
            cmm.SearchTab(Operation.Redo, ContextMenuCustom);
        }


        void HideContextMenus()
        {
            ContextMenuCustomFiles.Visibility = Visibility.Hidden;
            ContextMenuCustomFolders.Visibility = Visibility.Hidden;
        }

        //==============Кнопка закрытия проекта=======================
        private void CloseProjectButton_Click(object sender, RoutedEventArgs e)
        {
            appst.CurrentFolder = null;
            ViewXMLTags.Items.Clear();
            foreach (var tabc in tabCModels)
            {
                gridcol.Children.Remove(tabc.tb);
                tabc.tb.Items.Clear();
            }
            tabCModels.Clear();
            DeleteMainBorder();

            SelectProjectButton.Content = "Выберите или создайте проект";
            PathFile.Content = "";
            File.WriteAllText("pathProject.txt", "");

            if (tabCModels.Count == 0) firstTab.Visibility = Visibility.Visible;
            CloseProjectButton.Visibility = Visibility.Hidden;
            SaveButton.Visibility = Visibility.Hidden;

            UndoB.Visibility = Visibility.Hidden;
            RedoB.Visibility = Visibility.Hidden;
            SplitButton.Visibility = Visibility.Hidden;
        }


        //=======================Кнопка разделения вкладок на TabControl====================
        private void SplitButton_Click(object sender, RoutedEventArgs e)
        {
            if (tabCModels.Count == 0) return;

            SaveTabs st = new SaveTabs();
            //st.tabCModels = tabCModels;

            TabItemModel tab = new TabItemModel();
            TabItem ti = tabCModels[0].tb.SelectedItem as TabItem;
            tab.tab = ti;
            tab.FullPath = ti.Tag.ToString();
            tab.isImg = false;

            foreach (var tabc in tabCModels)
            {
                TabControlModel tbc = new TabControlModel();
                tbc.ID = tabc.ID;
                tbc.tb = tabc.tb;

                foreach (var tabit in tabc.TabItemModel)
                {
                    if (tabit.tab == ti) tab.isImg = tabit.isImg;

                    TabItemModel tabItem = new TabItemModel();
                    tabItem.tab = tabit.tab;
                    tabItem.FullPath = tabit.FullPath;
                    tabItem.isImg = tabit.isImg;

                    tbc.TabItemModel.Add(tabItem);
                }

                st.tabCModels.Add(tbc);
            }

            int cnt = st.tabCModels.Count;
            List<TabControlModel> tabCModelsDelete = new List<TabControlModel>();
            List<TabItemModel> items = new List<TabItemModel>();

            foreach (var tabc in tabCModels)
            {
                items = tabc.TabItemModel;
                tabCModelsDelete.Add(tabc);
            }

            foreach(var tabc in tabCModelsDelete)
            {
                tabCModels.Remove(tabc);
            }

            DeleteMainBorder();

            // CreateControl(cnt + 1);
            ReturnTabsOnSplit(st, tab);
        }

        private void ViewXMLTags_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ViewXMLTags_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                TreeView tree = sender as TreeView;
                TreeViewItem selectedItem = tree.SelectedItem as TreeViewItem;

                if (selectedItem != null)
                {
                    string path = func.GetFullPath(selectedItem, appst.CurrentFolder);
                    if (string.IsNullOrEmpty(path) || (!File.Exists(path) && !Directory.Exists(path)))
                        return;

                    DataObject data = new DataObject();
                    data.SetData(DataFormats.FileDrop, new string[] { path });
                    data.SetData("CustomTreeViewItem", selectedItem); 

                    DragDrop.DoDragDrop(tree, data, DragDropEffects.Copy);
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBorder.Visibility = Visibility.Visible;
        }

        private void CloseSearchBorderButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBorder.Visibility = Visibility.Hidden;
        }

        private List<TreeViewItem> searchResults = new List<TreeViewItem>();
        private int currentSearchIndex = -1;

        private List<(TextEditor editor, int offset)> codeSearchResults = new List<(TextEditor editor, int offset)>();
        private int currentCodeSearchIndex = -1;

        private void SearchInCode(string searchText)
        {
            codeSearchResults.Clear();
            currentCodeSearchIndex = -1;

            if (string.IsNullOrWhiteSpace(searchText)) return;

            foreach (var tabc in tabCModels)
            {
                foreach (var tab in tabc.TabItemModel)
                {
                    if (tab.tab.Content is Border br && br.Child is Grid grid &&
                       grid.Children.OfType<TextEditor>().FirstOrDefault() is TextEditor tx)
                    {
                        string text = tx.Text;
                        int index = 0;

                        while ((index = text.IndexOf(searchText, index, StringComparison.OrdinalIgnoreCase)) != -1)
                        {
                            codeSearchResults.Add((tx, index));
                            index += searchText.Length;
                        }
                    }
                }
            }

            if (codeSearchResults.Count > 0)
            {
                currentCodeSearchIndex = 0;
                FocusCodeMatch(codeSearchResults[currentCodeSearchIndex]);
                StatLbl_Code.Content = $"1 / {codeSearchResults.Count}";
            }
            else
            {
                StatLbl_Code.Content = "0 / 0";
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchTextBox.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                searchResults.Clear();
                currentSearchIndex = -1;
                StatLbl.Content = "0 / 0";
                return;
            }

            SearchModule sm = new SearchModule(text);
            searchResults = sm.SearchFiles(ViewXMLTags, text);
            SearchInCode(text);
            currentSearchIndex = 0;

            if (searchResults.Count == 0)
            {
                StatLbl.Content = "0 / 0";
                return;
            }

            FocusMatch(searchResults[currentSearchIndex]);
            StatLbl.Content = $"1 / {searchResults.Count}" + "   " + searchResults[currentSearchIndex].Header;
        }

        private void ReplaceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string replacement = ReplaceTextBox.Text;

            foreach (var tabControlModel in tabCModels)
            {
                foreach (var tabItemModel in tabControlModel.TabItemModel)
                {
                    if (tabItemModel.tab.Content is Border br && br.Child is Grid grid &&
                       grid.Children.OfType<TextEditor>().FirstOrDefault() is TextEditor editor)
                    {
                        var selection = editor.TextArea.Selection;
                        if (!selection.IsEmpty)
                        {
                            int startOffset = selection.SurroundingSegment.Offset;

                            editor.TextArea.Selection.ReplaceSelectionWithText(replacement);

                            editor.Select(startOffset, replacement.Length);
                        }
                    }
                }
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchResults.Count == 0) return;

            currentSearchIndex = (currentSearchIndex + 1) % searchResults.Count;

            FocusMatch(searchResults[currentSearchIndex]);
            StatLbl.Content = $"{currentSearchIndex + 1} / {searchResults.Count}" + "   " + searchResults[currentSearchIndex].Header;
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchResults.Count == 0) return;

            currentSearchIndex = (currentSearchIndex - 1 + searchResults.Count) % searchResults.Count;

            FocusMatch(searchResults[currentSearchIndex]);
            StatLbl.Content = $"{currentSearchIndex + 1} / {searchResults.Count}" + "   " + searchResults[currentSearchIndex].Header;
        }

        private void FocusMatch(TreeViewItem item)
        {
            item.IsSelected = true;
            item.BringIntoView();
            item.Focus();

            TabBuilder tb = new TabBuilder(ViewXMLTags, tabCModels[0], tabCModels, null, PathFile, appst.CurrentFolder);
            tb.Create(false, RestoreControls, appst.FontSize, appst.CurrentTheme);
        }

        private void FocusCodeMatch((TextEditor editor, int offset) match)
        {
            var editor = match.editor;
            int offset = match.offset;

            foreach (var tabc in tabCModels)
            {
                foreach (var tab in tabc.TabItemModel)
                {
                    if (tab.tab.Content is Border br && br.Child is Grid grid &&
                      grid.Children.OfType<TextEditor>().FirstOrDefault() is TextEditor tx && tx == editor)
                    {
                        tabc.tb.SelectedItem = tab.tab;

                        editor.Select(offset, SearchTextBox.Text.Length);
                        editor.ScrollToVerticalOffset(editor.Document.GetLineByOffset(offset).LineNumber);

                        return;
                    }
                }
            }
        }

        private void NextCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (codeSearchResults.Count == 0) return;

            currentCodeSearchIndex = (currentCodeSearchIndex + 1) % codeSearchResults.Count;
            FocusCodeMatch(codeSearchResults[currentCodeSearchIndex]);
            StatLbl_Code.Content = $"{currentCodeSearchIndex + 1} / {codeSearchResults.Count}";
        }

        private void PrevCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (codeSearchResults.Count == 0) return;

            currentCodeSearchIndex = (currentCodeSearchIndex - 1 + codeSearchResults.Count) % codeSearchResults.Count;
            FocusCodeMatch(codeSearchResults[currentCodeSearchIndex]);
            StatLbl_Code.Content = $"{currentCodeSearchIndex + 1} / {codeSearchResults.Count}";
        }

        
        private void ThemesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tvp.expandedPaths.Clear();
            tvp.selectedPath = null;
            sl.SaveTreeViewState(ViewXMLTags);
            File.WriteAllLines(myAppFolder + "\\" + "expanded.txt", tvp.expandedPaths);
            File.WriteAllText(myAppFolder + "\\" + "selected.txt", tvp.selectedPath ?? "");

            if (ThemesComboBox.SelectedIndex == 0)
            {
                appst.CurrentTheme = "light";
            }
            else if (ThemesComboBox.SelectedIndex == 1)
            {
                appst.CurrentTheme = "dark";
            }
            plugInManager.ActivatePlugIn(ThemesComboBox.SelectedItem.ToString());

            ViewXMLTags.Items.Clear();

            TreeViewItem item = new TreeViewItem();
            if (appst.CurrentFolder == "light")
            {
                item.Header = System.IO.Path.GetFileName(appst.CurrentFolder);
                item.Style = (Style)this.Resources["TreeViewItemStyleLight"];
                item.Foreground = Brushes.Black;
            }
            else
            {
                item.Header = System.IO.Path.GetFileName(appst.CurrentFolder);
                item.Style = (Style)this.Resources["TreeViewItemStyle2"];
            }
            //TreeViewItem item = new TreeViewItem { Header = System.IO.Path.GetFileName(currentFolder), Style = (Style)this.Resources["TreeViewItemStyle2"] };
            item = func.Files(appst.CurrentFolder, item);

            ViewXMLTags.Items.Add(item);
            sl.RestoreTreeViewState(ViewXMLTags);

            SaveTabs st = new SaveTabs();
            ///
            foreach (var tabc in tabCModels)
            {
                TabControlModel tbc = new TabControlModel();
                tbc.ID = tabc.ID;
                tbc.tb = tabc.tb;

                foreach (var tabit in tabc.TabItemModel)
                {
                    TabItemModel tabItem = new TabItemModel();
                    tabItem.tab = tabit.tab;
                    tabItem.FullPath = tabit.FullPath;
                    tabItem.isImg = tabit.isImg;

                    tbc.TabItemModel.Add(tabItem);
                }

                st.tabCModels.Add(tbc);
            }
            ///

            tabCModels.Clear();
            DeleteMainBorder();

            if (st.tabCModels.Count > 0) ReturnControls(st);
        }

        private void SideBarHideButton_Click(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = new Storyboard();

            TimeSpan totalDuration = TimeSpan.FromSeconds(1);

            var translateTransform = new TranslateTransform(0, 0);
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(translateTransform);
            wrap_panel.RenderTransform = transformGroup;
            wrap_panel.RenderTransformOrigin = new Point(0.5, 0.5);

            var translateX = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(translateX, wrap_panel);
            Storyboard.SetTargetProperty(translateX, new PropertyPath("RenderTransform.Children[0].(TranslateTransform.X)"));
            translateX.KeyFrames.Add(new SplineDoubleKeyFrame(_isSidebarCollapsed ? 0 : 60, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            translateX.KeyFrames.Add(new SplineDoubleKeyFrame(_isSidebarCollapsed ? 60 : 0, KeyTime.FromTimeSpan(totalDuration), new KeySpline(0.5, 0, 0.5, 1)));
            storyboard.Children.Add(translateX);

            storyboard.Begin();

            ToggleSidebar();
            //
        }

        //=================Create Message===================
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ai.SendButton(tabCModels, AITextBox, PathFile, AnsverPanel);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Load_Button_Click(null, null);
        }

        private void ViewXMLTags_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                var target = func.GetTreeViewItemUnderMouse(ViewXMLTags, e.GetPosition(ViewXMLTags));
                var folderNode = func.GetParentFolderTreeViewItem(target);

                string path = func.GetFullPath(folderNode ?? target, appst.CurrentFolder);
                if (path == null) return;

                e.Handled = true;

                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                {
                    foreach (string sourceFilePath in files)
                    {
                        try
                        {
                            tvp.expandedPaths.Clear();
                            tvp.selectedPath = null;
                            sl.SaveTreeViewState(ViewXMLTags);
                            File.WriteAllLines(myAppFolder + "\\" + "expanded.txt", tvp.expandedPaths);
                            File.WriteAllText(myAppFolder + "\\" + "selected.txt", tvp.selectedPath ?? "");

                            if (File.Exists(sourceFilePath))
                            {
                                string fileName = System.IO.Path.GetFileName(sourceFilePath);
                                string destFilePath = System.IO.Path.Combine(path, fileName);
                                File.Copy(sourceFilePath, destFilePath, true);
                            }
                            else if (Directory.Exists(sourceFilePath))
                            {
                                string folderName = System.IO.Path.GetFileName(sourceFilePath);
                                string destDirPath = System.IO.Path.Combine(path, folderName);
                                func.CopyDirectory(sourceFilePath, destDirPath, true);
                            }

                            ViewXMLTags.Items.Clear();

                            TreeViewItem item = new TreeViewItem();
                            if (appst.CurrentTheme == "light")
                            {
                                item.Header = System.IO.Path.GetFileName(appst.CurrentFolder);
                                item.Style = (Style)this.Resources["TreeViewItemStyleLight"];
                                item.Foreground = Brushes.Black;
                            }
                            else
                            {
                                item.Header = System.IO.Path.GetFileName(appst.CurrentFolder);
                                item.Style = (Style)this.Resources["TreeViewItemStyle2"];
                            }
                            // TreeViewItem item = new TreeViewItem { Header = System.IO.Path.GetFileName(currentFolder), Style = (Style)this.Resources["TreeViewItemStyle2"] };
                            item = func.Files(appst.CurrentFolder, item);

                            ViewXMLTags.Items.Add(item);

                            sl.RestoreTreeViewState(ViewXMLTags);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                {
                }
            }
        }

        private void ViewXMLTags_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(FontConf.Text, out _)) appst.FontSize = int.Parse(FontConf.Text);

            foreach (var tabc in tabCModels)
            {
                foreach (var tab in tabc.TabItemModel)
                {
                    if (tab.tab.Content is Border br && br.Child is Grid grid &&
                        grid.Children.OfType<TextEditor>().FirstOrDefault() is TextEditor tx)
                    {
                        if (appst.FontSize == 0) return;
                        tx.FontSize = appst.FontSize;
                    }
                }
            }
        }

        private void Splitter_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                func.AnimateOpacity(Splitter, 1);
            }
            else
            {
                func.AnimateOpacity(Splitter, 0);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}