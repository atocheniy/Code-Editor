using Code_Editor.Classes;
using ICSharpCode.AvalonEdit;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;
using static System.Windows.Forms.AxHost;

namespace Code_Editor.Modules
{
    internal class SaveLoad
    {
        TreeView ViewXMLTags;
        Grid MainGrid;
        List<TabControlModel> tabCModels;

        Functions func = new Functions();
        AppState appst = new AppState();
        TreeViewPath tvp = new TreeViewPath();

        Window w;

        static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string myAppFolder = System.IO.Path.Combine(appDataPath, "CodeEditor");

        public SaveLoad(Window w, TreeView treeView, Grid grid, AppState appst, List<TabControlModel> Models, TreeViewPath tvp)
        {
            this.w = w;
            ViewXMLTags = treeView;
            MainGrid = grid;
            this.appst = appst;
            this.tvp = tvp;
            tabCModels = Models;
        }

        public void Save()
        {
            tvp.expandedPaths.Clear();
            tvp.selectedPath = null;
            SaveTreeViewState(ViewXMLTags);
            File.WriteAllLines(myAppFolder + "\\" + "expanded.txt", tvp.expandedPaths);
            File.WriteAllText(myAppFolder + "\\" + "selected.txt", tvp.selectedPath ?? "");


            XmlSerializer xs = new XmlSerializer(typeof(SaveTabs));

            SaveTabs st = new SaveTabs();
            st.tabCModels = tabCModels;
            st.current_path = appst.CurrentFolder;

            using (FileStream fs = new FileStream(myAppFolder + "\\" + "data.xml", FileMode.Create, FileAccess.Write, FileShare.None)) xs.Serialize(fs, st);

            XmlSerializer xs2 = new XmlSerializer(typeof(Classes.Settings));

            Classes.Settings s = new Classes.Settings();
            s.ResolutionWidth = w.ActualWidth.ToString();
            s.ResolutionHeight = w.ActualHeight.ToString();
            s.SidebarColumnWidth = MainGrid.ColumnDefinitions[0].Width.Value.ToString();
            s.ContentColumnWidth = MainGrid.ColumnDefinitions[1].Width.IsStar ? MainGrid.ColumnDefinitions[1].Width.Value.ToString() + "*" : MainGrid.ColumnDefinitions[1].Width.Value.ToString();
            s.isBlur = appst.isBlur;
            s.FontSize = appst.FontSize;
            s.CurrentTheme = appst.CurrentTheme;

            using (FileStream fs = new FileStream(myAppFolder + "\\" + "settings.xml", FileMode.Create, FileAccess.Write, FileShare.None)) xs2.Serialize(fs, s);
        }

        public void Load(Label PathFile, Action<int> CreateControl, Action<TreeView, TabControlModel, List<TabControlModel>, TabItemModel, Label, string, bool> CreateTab, 
            TextBox FontConf, Border SideBar, Border DockPanel, Border ContentPanel, Button SelectProjectButton, CheckBox BlurToggle)
        {
            SaveTabs st = null;

            try
            {
                if (File.Exists(myAppFolder + "\\" + "data.xml"))
                {
                    FileInfo fi = new FileInfo(myAppFolder + "\\" + "data.xml");
                    if (fi.Length > 0)
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(SaveTabs));

                        Stream fstream = File.OpenRead(myAppFolder + "\\" + "data.xml");
                        st = (SaveTabs)xs.Deserialize(fstream);
                        fstream.Close();

                        appst.CurrentFolder = st.current_path;
                        if (appst.CurrentFolder != null)
                        {
                            TreeViewItem item = new TreeViewItem();
                            if (appst.CurrentTheme == "light")
                            {
                                item.Header = System.IO.Path.GetFileName(appst.CurrentFolder);
                                item.Style = (Style)w.Resources["TreeViewItemStyleLight"];
                                item.Foreground = Brushes.Black;
                            }
                            else
                            {
                                item.Header = System.IO.Path.GetFileName(appst.CurrentFolder);
                                item.Style = (Style)w.Resources["TreeViewItemStyle2"];
                            }

                            // { Header = System.IO.Path.GetFileName(currentFolder), Style = (Style)this.Resources["TreeViewItemStyle2"] };
                            item = func.Files(appst.CurrentFolder, item);

                            ViewXMLTags.Items.Add(item);
                            SelectProjectButton.Content = System.IO.Path.GetFileName(appst.CurrentFolder);

                            if (fi.Length > 0) ReturnControls(st, PathFile, CreateControl, CreateTab);
                        }
                    }
                }
                else
                {
                    using (var fs = File.Create(myAppFolder + "\\" + "data.xml"))
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.WriteLine("<SaveTabs></SaveTabs>");
                    }
                }
            }
            catch { }

            try
            {
                if (File.Exists(myAppFolder + "\\" + "settings.xml"))
                {
                    XmlSerializer xs2 = new XmlSerializer(typeof(Classes.Settings));

                    Stream fstream2 = File.OpenRead(myAppFolder + "\\" + "settings.xml");
                    Classes.Settings s = (Classes.Settings)xs2.Deserialize(fstream2);
                    fstream2.Close();

                    w.Width = double.Parse(s.ResolutionWidth);
                    w.Height = double.Parse(s.ResolutionHeight);

                    appst.FontSize = s.FontSize;
                    FontConf.Text = s.FontSize.ToString();
                    appst.CurrentTheme = s.CurrentTheme;

                    if (double.TryParse(s.SidebarColumnWidth, out double sidebarWidth))
                    {
                        MainGrid.ColumnDefinitions[0].Width = new GridLength(sidebarWidth);
                    }
                    if (s.ContentColumnWidth.EndsWith("*"))
                    {
                        string val = s.ContentColumnWidth.TrimEnd('*');
                        if (double.TryParse(val, out double contentStar))
                        {
                            MainGrid.ColumnDefinitions[1].Width = new GridLength(contentStar, GridUnitType.Star);
                        }
                    }
                    else if (double.TryParse(s.ContentColumnWidth, out double contentWidth))
                    {
                        MainGrid.ColumnDefinitions[1].Width = new GridLength(contentWidth, GridUnitType.Pixel);
                    }

                    appst.isBlur = s.isBlur;
                    // SliderOpacity.Value = double.Parse(appst.Setting);

                    if (appst.isBlur == true)
                    {
                        func.AnimateOpacity(SideBar, 0.4, null);
                        func.AnimateOpacity(DockPanel, 0.9, null);

                        BlurToggle.IsChecked = true;
                    }

                    else if (appst.isBlur == false)
                    {
                        func.AnimateOpacity(SideBar, 1, null);
                        func.AnimateOpacity(DockPanel, 1, null);

                        BlurToggle.IsChecked = false;
                    }

                    // if (double.Parse(appst.Setting) >= 0.3) func.AnimateOpacity(SideBar, double.Parse(appst.Setting), appst.Setting);
                    // if (double.Parse(appst.Setting) >= 0.9) func.AnimateOpacity(DockPanel, double.Parse(appst.Setting), appst.Setting);
                    // if (double.Parse(appst.Setting) >= 0.95) func.AnimateOpacity(ContentPanel, double.Parse(appst.Setting), appst.Setting);

                    // if (double.Parse(appst.Setting) < 0.9) func.AnimateOpacity(DockPanel, 0.9, appst.Setting);
                    // if (double.Parse(appst.Setting) < 0.95) func.AnimateOpacity(ContentPanel, 0.95, appst.Setting);
                }
                else
                {
                    using (var fs = File.Create(myAppFolder + "\\" + "settings.xml"))
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.WriteLine("<Settings></Settings>");
                    }

                    w.Width = 1400;
                    w.Height = 945;
                }
            }
            catch { }
        }

        public void Load_Button(Button SelectProjectButton, Grid gridcol, Action DeleteMainBorder)
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                var dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = @"C:\";
                dialog.IsFolderPicker = true;
                CommonFileDialogResult result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok)
                {
                    foreach (var tabc in tabCModels) tabc.tb.Items.Clear();
                    tabCModels.Clear();

                    appst.CurrentFolder = dialog.FileName;

                    Save();
                    DeleteMainBorder();

                    ViewXMLTags.Items.Clear();
                    foreach (var tabc in tabCModels)
                    {
                        gridcol.Children.Remove(tabc.tb);
                        tabc.tb.Items.Clear();
                    }
                    tabCModels.Clear();

                    TreeViewItem item = new TreeViewItem();
                    if (appst.CurrentFolder == "light")
                    {
                        item.Header = System.IO.Path.GetFileName(dialog.FileName);
                        item.Style = (Style)w.Resources["TreeViewItemStyleLight"];
                        item.Foreground = Brushes.Black;
                    }
                    else
                    {
                        item.Header = System.IO.Path.GetFileName(dialog.FileName);
                        item.Style = (Style)w.Resources["TreeViewItemStyle2"];
                    }
                    //TreeViewItem item = new TreeViewItem { Header = System.IO.Path.GetFileName(dialog.FileName), Style = (Style)this.Resources["TreeViewItemStyle2"] };

                    item = func.Files(dialog.FileName, item);

                    ViewXMLTags.Items.Add(item);

                    SelectProjectButton.Content = System.IO.Path.GetFileName(dialog.FileName);
                }
            }
        }

        public void SaveButton()
        {
            foreach (var tabc in tabCModels)
            {
                foreach (var tab in tabc.TabItemModel)
                {
                    string s = System.IO.Path.GetFileName(tab.FullPath);

                    if (tab.tab.Header.ToString() == s)
                    {
                        if (tab.tab.Content is Border br && br.Child is Grid grid &&
                            grid.Children.OfType<TextEditor>().FirstOrDefault() is TextEditor tx)
                        {
                            File.WriteAllText(tab.FullPath, tx.Text);
                        }
                    }
                }
            }
        }

        public void SaveTreeViewState(ItemsControl parent, string currentPath = "")
        {
            foreach (var item in parent.Items)
            {
                if (item is TreeViewItem treeItem)
                {
                    string path = currentPath + "/" + treeItem.Header.ToString();

                    if (treeItem.IsExpanded)
                        tvp.expandedPaths.Add(path);

                    if (treeItem.IsSelected)
                        tvp.selectedPath = path;

                    SaveTreeViewState(treeItem, path);
                }
            }
        }

        public void RestoreTreeViewState(ItemsControl parent, string currentPath = "")
        {
            List<string> expandedPaths = File.Exists(myAppFolder + "\\" + "expanded.txt") ? File.ReadAllLines(myAppFolder + "\\" + "expanded.txt").ToList() : new List<string>();
            string selectedPath = File.Exists(myAppFolder + "\\" + "selected.txt") ? File.ReadAllText(myAppFolder + "\\" + "selected.txt") : "";

            foreach (var item in parent.Items)
            {
                if (item is TreeViewItem treeItem)
                {
                    if (appst.CurrentTheme == "light")
                    {
                        treeItem.Foreground = Brushes.Black;
                    }
                    string path = currentPath + "/" + treeItem.Header.ToString();

                    if (expandedPaths.Contains(path))
                        treeItem.IsExpanded = true;

                    if (selectedPath == path)
                        treeItem.IsSelected = true;

                    RestoreTreeViewState(treeItem, path);
                }
            }
        }

        public void ReturnControls(SaveTabs st, Label PathFile, Action<int> CreateControl, Action<TreeView, TabControlModel, List<TabControlModel>, TabItemModel, Label, string, bool> CreateTab)
        {
            if (st.tabCModels.Count == 0) return;
            CreateControl(st.tabCModels.Count);

            for (int i = 0; i < st.tabCModels.Count; i++)
            {
                var Tabc = st.tabCModels[i];
                foreach (var tab in Tabc.TabItemModel) CreateTab(ViewXMLTags, tabCModels[i], tabCModels, tab, PathFile, appst.CurrentFolder, true);
            }
        }
    }
}
