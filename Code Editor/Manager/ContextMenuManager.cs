using Code_Editor.Classes;
using Code_Editor.Modules;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Code_Editor.MainWindow;

namespace Code_Editor.Manager
{
    internal class ContextMenuManager
    {
        List<TabControlModel> tabCModels;
        Functions func = new Functions();
        AppState appst;
        TreeViewPath tvp;
        SaveLoad sl;

        TreeView ViewXMLTags;
        Window w;

        static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string myAppFolder = System.IO.Path.Combine(appDataPath, "CodeEditor");

        public ContextMenuManager(AppState appst, List<TabControlModel> Models, TreeViewPath tvp, TreeView ViewXMLTags, SaveLoad sl, Window w)
        {
            this.appst = appst;
            this.tabCModels = Models;
            this.tvp = tvp;
            this.sl = sl;

            this.ViewXMLTags = ViewXMLTags;
            this.w = w;
        }


        public void SearchTab(Operation operation, Border ContextMenuCustom)
        {
            TabItem selectedTab = null;
            foreach (var tabControlModel in tabCModels)
            {
                if (tabControlModel.tb != null)
                {
                    var currentSelected = tabControlModel.tb.SelectedItem as TabItem;
                    if (currentSelected != null)
                    {
                        selectedTab = currentSelected;
                        break;
                    }
                }
            }

            if (selectedTab == null)
                return;

            TabItemModel selectedModel = null;

            foreach (var tabControlModel in tabCModels)
            {
                foreach (var tabItemModel in tabControlModel.TabItemModel)
                {
                    if (tabItemModel.tab == selectedTab)
                    {
                        selectedModel = tabItemModel;
                        break;
                    }
                }
                if (selectedModel != null)
                    break;
            }

            if (selectedModel == null)
                return;


            if (selectedTab.Content is Border br && br.Child is Grid grid)
            {
                foreach (var child in grid.Children)
                {
                    if (child is ICSharpCode.AvalonEdit.TextEditor editor)
                    {
                        switch (operation)
                        {
                            case Operation.Undo:
                                if (editor.CanUndo)
                                {
                                    editor.Undo();
                                }
                                ContextMenuCustom.Visibility = Visibility.Hidden;
                                return;

                            case Operation.Redo:
                                if (editor.CanRedo)
                                {
                                    editor.Redo();
                                }
                                ContextMenuCustom.Visibility = Visibility.Hidden;
                                return;

                            case Operation.Copy:
                                Clipboard.SetText(editor.SelectedText);
                                ContextMenuCustom.Visibility = Visibility.Hidden;
                                return;

                            case Operation.Paste:
                                editor.TextArea.Selection.ReplaceSelectionWithText(Clipboard.GetText());
                                ContextMenuCustom.Visibility = Visibility.Hidden;
                                return;

                            case Operation.Cut:
                                Clipboard.SetText(editor.SelectedText);
                                editor.SelectedText = "";
                                ContextMenuCustom.Visibility = Visibility.Hidden;
                                return;
                        }
                    }
                }
            }

        }

        public void SearchTabFiles(Operation operation, bool isFolder, Action HideContextMenus)
        {
            var selectedItem = ViewXMLTags.SelectedItem as TreeViewItem;

            if (selectedItem != null)
            {
                string sourceFilePath = func.GetFullPath(selectedItem, appst.CurrentFolder);

                if (File.Exists(sourceFilePath) || Directory.Exists(sourceFilePath))
                {
                    StringCollection files = Clipboard.GetFileDropList();

                    try
                    {
                        tvp.expandedPaths.Clear();
                        tvp.selectedPath = null;
                        sl.SaveTreeViewState(ViewXMLTags);
                        File.WriteAllLines(myAppFolder + "\\" + "expanded.txt", tvp.expandedPaths);
                        File.WriteAllText(myAppFolder + "\\" + "selected.txt", tvp.selectedPath ?? "");

                        switch (operation)
                        {
                            case Operation.Copy:
                                StringCollection f = new StringCollection();
                                f.Add(sourceFilePath);

                                Clipboard.SetFileDropList(f);

                                HideContextMenus();
                                return;

                            case Operation.Paste:

                                foreach (String file in files)
                                {
                                    if (File.Exists(file))
                                    {
                                        File.Copy(file, func.GetFullPath(selectedItem, appst.CurrentFolder) + "\\" + System.IO.Path.GetFileName(file));
                                    }
                                    else
                                    {
                                        Functions f1 = new Functions();
                                        f1.CopyDirectory(file, func.GetFullPath(selectedItem, appst.CurrentFolder) + "\\" + System.IO.Path.GetFileName(file), true);
                                    }
                                }

                                HideContextMenus();
                                break;

                            case Operation.Cut:

                                files.Add(sourceFilePath);
                                Clipboard.SetFileDropList(files);

                                if (Directory.Exists(sourceFilePath)) Directory.Delete(sourceFilePath, true);
                                else File.Delete(sourceFilePath);

                                HideContextMenus();
                                break;

                            case Operation.Delete:
                                if (Directory.Exists(sourceFilePath)) Directory.Delete(sourceFilePath, true);
                                else File.Delete(sourceFilePath);

                                HideContextMenus();
                                break;

                            case Operation.CopyPath:
                                Clipboard.SetText(sourceFilePath);
                                HideContextMenus();
                                return;
                        }

                        ViewXMLTags.Items.Clear();

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
                        //TreeViewItem item = new TreeViewItem { Header = System.IO.Path.GetFileName(currentFolder), Style = (Style)this.Resources["TreeViewItemStyle2"] };
                        item = func.Files(appst.CurrentFolder, item);

                        ViewXMLTags.Items.Add(item);

                        sl.RestoreTreeViewState(ViewXMLTags);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
    }
}
