using Code_Editor.Classes;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using static ICSharpCode.AvalonEdit.Document.TextDocumentWeakEventManager;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Code_Editor.Modules
{

    internal class TabBuilder
    {
        TreeView ViewXMLTags;
        TabControlModel TabC;
        TabItemModel tab;
        List<TabControlModel> tabCModels;
        Label PathFile;

        private ScrollViewer _editorScrollViewer;

        Functions func;

        double _targetOffset = 0;
        double _targetOffsetHorizontal = 0;
        double ScrollSpeed = 50;
        string currentFolder;

        bool isIMG = false;

        public TabBuilder(TreeView ViewXMLTags, TabControlModel TabC, List<TabControlModel> tabCModels, TabItemModel tab, Label PathFile, string currentFolder) 
        { 
            this.ViewXMLTags = ViewXMLTags;
            this.TabC = TabC;
            this.tab = tab;
            this.tabCModels = tabCModels;
            this.PathFile = PathFile;
            this.currentFolder = currentFolder;

            func = new Functions();
        }

        bool IsBinary(string path)
        {
            try
            {
                byte[] buffer = File.ReadAllBytes(path);
                for (int i = 0; i < Math.Min(512, buffer.Length); i++)
                {
                    if (buffer[i] == 0 || (buffer[i] < 0x09 && buffer[i] != 0x0A && buffer[i] != 0x0D))
                        return true;
                }
                return false;
            }
            catch { return false; }
        }

        private void TabC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabC.tb.SelectedItem is TabItem selectedTab)
            {
                if (selectedTab.Content is Border br && br.Child is Grid grid &&
                    grid.Children.OfType<TextEditor>().FirstOrDefault() is TextEditor tx)
                {
                    int lineCount = tx.Text.Split('\n').Length;
                    TabC.linesCountLabel.Content = "Lines: " + lineCount;
                    if (tx.SyntaxHighlighting != null) TabC.languageLabel.Content = "Language: " + tx.SyntaxHighlighting.ToString();
                    else TabC.languageLabel.Content = "Language: " + "Other";
                    try
                    {
                        PathFile.Content = TabC.TabItemModel[TabC.tb.SelectedIndex].FullPath.Replace("\\", " > ");
                    }
                    catch { };
                }
            }
        }


        public void Create(bool isReturn, Action<int> RestoreControls, int font_size, string currentTheme)
        {
            string fullPath = "";

            if (isReturn)
            {
                fullPath = tab.FullPath;
                if(tab.isImg == true) isIMG = true;
            }
            else
            {
                TreeViewItem treeViewItem = ViewXMLTags.SelectedItem as TreeViewItem;
                if (treeViewItem != null)
                {
                    if (Directory.Exists(func.GetFullPath(treeViewItem, currentFolder))) return;
                    fullPath = func.GetFullPath(treeViewItem, currentFolder);

                    if (System.IO.Path.GetExtension(fullPath) == ".png" || System.IO.Path.GetExtension(fullPath) == ".jpg" || System.IO.Path.GetExtension(fullPath) == ".jpeg" || System.IO.Path.GetExtension(fullPath) == ".webp")
                    {
                        isIMG = true;
                    }
                    else if (IsBinary(fullPath))
                    {
                        MessageBox.Show("Файл является бинарным и не может быть открыт как текст.");
                        return;
                    }

                    foreach (TabItem item in TabC.tb.Items)
                    {
                        if (item.Tag?.ToString() == fullPath)
                        {
                            TabC.tb.SelectedItem = item;
                            return;
                        }
                    }
                }

            }

            string tabName = System.IO.Path.GetFileName(fullPath);

            if (File.Exists(fullPath))
            {

                TabItem tb = new TabItem()
                {
                    Height = 28,
                    Tag = fullPath,
                    Header = System.IO.Path.GetFileName(fullPath),
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(9, 1, -9, 0),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF292929")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF565656")),
                    Foreground = new SolidColorBrush(Colors.White),
                    FontFamily = new FontFamily("Segoe UI Variable Text Semibold"),
                    FontSize = 16
                };

                if(currentTheme == "light")
                {
                    tb.Style = Application.Current.Resources["TabItemLight"] as Style;
                }

                TabC.linesCountLabel.Visibility = Visibility.Visible;
                TabC.positionCountLabel.Visibility = Visibility.Visible;
                TabC.languageLabel.Visibility = Visibility.Visible;

                tb.Loaded += (s, e) =>
                {
                    Button closeButton = (Button)tb.Template.FindName("close_tabButton", tb);
                    if (closeButton != null)
                    {
                        closeButton.Click += (sender, args) =>
                        {
                            // ((TabControl)tb.Parent).Items.Remove(tb);

                            var tabsToRemove = new List<string>();

                            if (sender is Button button && button.Tag is TabItem tabItem)
                                if (tabItem.Parent is TabControl tabControl)
                                {
                                    foreach (TabItem tab in TabC.tb.Items)
                                    {
                                        if (tab.Tag.ToString() == tabItem.Tag.ToString()) tabsToRemove.Add(tab.Tag.ToString());
                                    }
                                    tabControl.Items.Remove(tabItem);
                                }

                            foreach (var tab in tabsToRemove)
                            {
                                TabC.TabItemModel.RemoveAll(t => t.FullPath == tab);
                                TabC.tb.Items.Remove(tab);
                            }
                            // if (tabs.Count == 0) firstTab.Visibility = Visibility.Visible;


                            if (TabC.tb.Items.Count == 0)
                            {
                                tabCModels.Remove(TabC);
                                RestoreControls(tabCModels.Count);
                            }

                        };
                    }
                };
                Point dragStartPoint = new Point();

                tb.PreviewMouseLeftButtonDown += (se, es) =>
                {

                    if (es.LeftButton == MouseButtonState.Pressed)
                    {
                        dragStartPoint = es.GetPosition(null);
                    }
                };

                tb.MouseMove += (se, es) =>
                {
                    if (es.LeftButton == MouseButtonState.Pressed)
                    {
                        Point currentPosition = es.GetPosition(null);

                        double headerHeight = 130;
                        if (currentPosition.Y < headerHeight)
                        {
                            if (Math.Abs(currentPosition.X - dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                                Math.Abs(currentPosition.Y - dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                            {
                                DragDrop.DoDragDrop(tb, tb, DragDropEffects.Move);

                                foreach (var Tabc in tabCModels)
                                {
                                    if (Tabc.TabItemModel.Count == 0)
                                    {
                                        //if (tabCModels.Count > 1)
                                        //{
                                            tabCModels.Remove(Tabc);
                                            RestoreControls(tabCModels.Count);
                                            break;
                                        //}
                                    }
                                }
                            }
                        }
                    }
                };

                //Grid gr = new Grid()
                //{
                //    VerticalAlignment = VerticalAlignment.Stretch,
                //};

                Border br = new Border()
                {
                    Name = "tx_border",
                    CornerRadius = new CornerRadius(10),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF323031")),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, -2, 2, 10),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#181818")),
                };

                //Grid grbr = new Grid()
                //{
                //    Margin = new Thickness(10, 0, 10, 0)
                //};

                //grbr.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                //grbr.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                //ScrollViewer sv = new ScrollViewer()
                //{
                //    Name = "LineNumbersScrollViewer",
                //    VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                //    HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                //    Margin = new Thickness(0, 8, 5, 7)
                //};

                //Grid.SetColumn(sv, 0);

                //TextBlock textbl = new TextBlock()
                //{
                //    Name = "LineNumbers",
                //    FontFamily = new FontFamily("Cascadia Mono SemiLight"),
                //    FontSize = 20,
                //    Foreground = new SolidColorBrush(Colors.Gray),
                //    Padding = new Thickness(5),
                //    TextAlignment = TextAlignment.Right,
                //};

                Border imgB = new Border();
                TextEditor tx = new TextEditor();

                // ScrollViewer sv2 = new ScrollViewer();
                System.Windows.Controls.RichTextBox rct = new System.Windows.Controls.RichTextBox();

                if (isIMG)
                {
                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    img.Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
                    imgB.BorderThickness = new Thickness(1);
                    imgB.Padding = new Thickness(10, 6, 0, 0);

                    imgB.Child = img;
                }
                else
                {
                    tx.BorderThickness = new Thickness(1);
                    tx.Padding = new Thickness(10, 6, 0, 0);
                    tx.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#181818"));
                    tx.Foreground = new SolidColorBrush(Colors.White);
                    //tx.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    //tx.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    tx.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    tx.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    tx.Name = "textEditor";
                    // tx.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
                    tx.FontFamily = new FontFamily("JetBrains Mono");
                    tx.FontSize = font_size;


                    tx.TextArea.TextView.Options.EnableVirtualSpace = false;
                    tx.TextArea.TextView.Options.AllowScrollBelowDocument = true;
                    tx.TextArea.TextView.Options.EnableRectangularSelection = true;

                    tx.ShowLineNumbers = true;


                    var indentGuideLinesRenderer = new IndentGuideLinesRenderer(tx);
                    tx.TextArea.TextView.BackgroundRenderers.Add(indentGuideLinesRenderer);

                    tx.Options.ConvertTabsToSpaces = true;
                    tx.Options.IndentationSize = 4;

                    //sv2 = new ScrollViewer()
                    //{
                    //    Style = (Style)Application.Current.Resources["ScrollViewerStyle1"],
                    //    Name = "TextScrollViewer",
                    //    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    //    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    //    Margin = new Thickness(6, 8, 0, 7),
                    //};
                    //sv2.ScrollChanged += (se, es) =>
                    //{

                    //    sv.ScrollToVerticalOffset(sv2.VerticalOffset);
                    //};
                    //sv2.PreviewMouseWheel += (se, es) =>
                    //{
                    //    if (se is ScrollViewer scrollViewer)
                    //    {
                    //        es.Handled = true;

                    //        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    //        {
                    //            ScrollSpeed = 0;

                    //            int steps = es.Delta / 120;
                    //            tx.FontSize = Math.Max(8, tx.FontSize + steps);
                    //            es.Handled = true;

                    //            ScrollSpeed = 50;
                    //            return;
                    //        }

                    //        if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    //        {
                    //            _targetOffsetHorizontal = Math.Max(0, Math.Min(scrollViewer.ScrollableWidth, _targetOffsetHorizontal - es.Delta / 120 * ScrollSpeed));

                    //            var animation = new DoubleAnimation
                    //            {
                    //                To = _targetOffsetHorizontal,
                    //                Duration = TimeSpan.FromMilliseconds(300),
                    //                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    //            };

                    //            scrollViewer.BeginAnimation(ScrollViewerOffsetHelper.HorizontalOffsetProperty, animation);
                    //        }
                    //        else
                    //        {
                    //            _targetOffset = Math.Max(0, Math.Min(scrollViewer.ScrollableHeight, _targetOffset - es.Delta / 120 * ScrollSpeed));

                    //            var animation = new DoubleAnimation
                    //            {
                    //                To = _targetOffset,
                    //                Duration = TimeSpan.FromMilliseconds(300),
                    //                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    //            };

                    //            scrollViewer.BeginAnimation(ScrollViewerOffsetHelper.VerticalOffsetProperty, animation);
                    //        }
                    //    }
                    //};
                    //Grid.SetColumn(sv2, 1);

                    rct = new System.Windows.Controls.RichTextBox()
                    {

                        Name = "textboxxml",
                        Padding = new Thickness(10, 4, 0, 0),
                        Style = (Style)Application.Current.Resources["RichTextBoxStyle1"],
                        AcceptsReturn = true,
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF28282A")),
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFABAdB3")),
                        Foreground = new SolidColorBrush(Colors.White),
                        FontFamily = new FontFamily("Cascadia Mono SemiLight"),
                        FontSize = 20,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                        Margin = new Thickness(10, 13, 3, 12),
                        Height = double.NaN,
                        Visibility = Visibility.Hidden,
                    };
                    //Grid.SetColumn(rct, 1);
                    //Grid.SetColumnSpan(rct, 2);

                    tx.HorizontalAlignment = HorizontalAlignment.Stretch;
                    tx.VerticalAlignment = VerticalAlignment.Stretch;
                    tx.Height = double.NaN;
                    tx.Width = double.NaN;

                    tx.PreviewMouseUp += (se, es) => UpdateCursorPosition(tx);
                    tx.PreviewMouseDown += (se, es) => UpdateCursorPosition(tx);
                    tx.PreviewMouseMove += (se, es) =>
                    {
                        if (Mouse.LeftButton == MouseButtonState.Pressed) UpdateCursorPosition(tx);
                    };

                    if (System.IO.Path.GetExtension(fullPath) == ".cs")
                    {
                        TabC.languageLabel.Content = "Language: C#";
                        tx.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
                    }
                    else if (System.IO.Path.GetExtension(fullPath) == ".py")
                    {
                        TabC.languageLabel.Content = "Language: Python";
                        tx.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Python");
                    }
                    else if (System.IO.Path.GetExtension(fullPath) == ".js")
                    {
                        TabC.languageLabel.Content = "Language: JavaScript";
                        tx.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
                    }
                    else if (System.IO.Path.GetExtension(fullPath) == ".html")
                    {
                        TabC.languageLabel.Content = "Language: HTML";
                        tx.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML");
                    }
                    else if (System.IO.Path.GetExtension(fullPath) == ".css")
                    {
                        TabC.languageLabel.Content = "Language: CSS";
                        tx.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CSS");
                    }
                    else if (System.IO.Path.GetExtension(fullPath) == ".xml" || System.IO.Path.GetExtension(fullPath) == ".csproj" || System.IO.Path.GetExtension(fullPath) == ".config")
                    {
                        TabC.languageLabel.Content = "Language: XML";
                        tx.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
                    }
                    else if (System.IO.Path.GetExtension(fullPath) == ".xaml")
                    {
                        TabC.languageLabel.Content = "Language: XAML";
                        tx.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
                    }
                    else TabC.languageLabel.Content = "Language: Other";

                    tx.Margin = new Thickness(0, -2, -4, -2);
                    tx.TextArea.IndentationStrategy = new DefaultIndentationStrategy();

                    //var reader = new XmlTextReader("C:\\Users\\Андрей\\source\\repos\\Code Editor\\Code Editor\\Resources\\CSharp-Mode.xshd");
                    //var customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    //tx.SyntaxHighlighting = customHighlighting;

                    tx.Text = File.ReadAllText(fullPath).TrimEnd('\r', '\n').Replace("\t", "    ");

                    CompletionWindow completionWindow = new CompletionWindow(tx.TextArea);

                    string[] CSharpKeywords = {
                    "public", "private", "class", "void", "int", "string",
                    "var", "if", "else", "for", "foreach", "while", "return",
                    "Console", "WriteLine", "DateTime", "Math", "List", "Array"
                };

                    string[] HtmlTags = {
                    "html", "head", "body", "div", "p", "span", "h1", "h2",
                    "h3", "ul", "li", "a", "img", "table", "tr", "td", "form",
                    "input", "button", "style", "script"
                };

                         string[] HtmlTagsExtented = {
                    "class", "id", "display", "name", "value", "margin", "padding", 
                };

                    Dictionary<char, char> PairCharacters = new Dictionary<char, char>()
                    {
                        ['"'] = '"',
                        ['\''] = '\'',
                        ['('] = ')',
                        ['['] = ']',
                        ['{'] = '}',
                        ['<'] = '>'
                    };

                    tx.TextArea.TextEntering += textEditor_TextArea_TextEntering;
                    tx.TextArea.TextEntered += textEditor_TextArea_TextEntered;

                    tx.TextArea.TextEntering += OnTextEntering;
                    tx.TextArea.PreviewKeyDown += OnPreviewKeyDown;

                    tx.Loaded += (se, es) =>
                    {
                        if (se is TextEditor editor)
                        {
                            editor.ApplyTemplate();
                            _editorScrollViewer = editor.Template?.FindName("PART_ScrollViewer", editor) as ScrollViewer;
                            _editorScrollViewer.Style = (Style)Application.Current.Resources["ScrollViewerStyle1"];
                        }
                    };

                    tx.PreviewMouseWheel += (se, es) =>
                    {
                        ScrollViewer scrollViewer = _editorScrollViewer;

                        if (scrollViewer != null)
                        {
                            es.Handled = true;

                            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                            {
                                ScrollSpeed = 0;

                                int steps = es.Delta / 120;
                                tx.FontSize = Math.Max(8, tx.FontSize + steps);
                                es.Handled = true;

                                ScrollSpeed = 50;
                                return;
                            }

                            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                            {
                                _targetOffsetHorizontal = Math.Max(0, Math.Min(scrollViewer.ScrollableWidth, _targetOffsetHorizontal - es.Delta / 120 * ScrollSpeed));

                                var animation = new DoubleAnimation
                                {
                                    To = _targetOffsetHorizontal,
                                    Duration = TimeSpan.FromMilliseconds(300),
                                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                                };

                                scrollViewer.BeginAnimation(ScrollViewerOffsetHelper.HorizontalOffsetProperty, animation);
                            }
                            else
                            {
                                _targetOffset = Math.Max(0, Math.Min(scrollViewer.ScrollableHeight, _targetOffset - es.Delta / 120 * 70));

                                var animation = new DoubleAnimation
                                {
                                    To = _targetOffset,
                                    Duration = TimeSpan.FromMilliseconds(300),
                                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                                };

                                scrollViewer.BeginAnimation(ScrollViewerOffsetHelper.VerticalOffsetProperty, animation);
                            }
                        }
                    };

                    void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
                    {
                        TabC.linesCountLabel.Content = "Lines: " + (tx.LineCount);

                        if (e.Text == ".")
                        {
                            ShowCompletionWindow(CSharpKeywords
                                .Select(k => new MyCompletionData(k))
                                .ToList());
                        }
                        else if (e.Text == "<" || e.Text == "/")
                        {
                            ShowCompletionWindow(HtmlTags
                                .Select(t => new MyCompletionData(
                                    e.Text == "/" ? $"/{t}" : t
                                ))
                                .ToList());
                        }
                        else if (e.Text.Length == 1 && char.IsLetter(e.Text[0]))
                        {
                            var filtered = CSharpKeywords
                                .Where(k => k.StartsWith(e.Text, StringComparison.OrdinalIgnoreCase))
                                .Select(k => new MyCompletionData(k))
                                .ToList();

                            if (filtered.Any())
                                ShowCompletionWindow(filtered);
                        }
                    }

                    void ShowCompletionWindow(IEnumerable<ICompletionData> items)
                    {
                        completionWindow = new CompletionWindow(tx.TextArea);

                        foreach (var item in items)
                        {
                            completionWindow.CompletionList.CompletionData.Add(item);
                        }

                        completionWindow.Show();
                        completionWindow.Closed += (s, args) => completionWindow = null;
                    }

                    void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
                    {

                        if (e.Text.Length > 0 && completionWindow != null)
                        {
                            if (!char.IsLetterOrDigit(e.Text[0]))
                            {
                                completionWindow.CompletionList.RequestInsertion(e);
                            }
                        }
                    }

                    void OnTextEntering(object sender, TextCompositionEventArgs e)
                    {
                        if (string.IsNullOrEmpty(e.Text) || completionWindow != null) return;

                        var editor = tx.TextArea;
                        var document = editor.Document;
                        var caretOffset = editor.Caret.Offset;
                        var currentChar = e.Text[0];

                        if (PairCharacters.TryGetValue(currentChar, out var closingChar))
                        {
                            e.Handled = true;

                            document.Insert(caretOffset, $"{currentChar}{closingChar}");

                            editor.Caret.Offset = caretOffset + 1;
                        }

                        else if (PairCharacters.ContainsValue(currentChar))
                        {
                            if (caretOffset < document.TextLength &&
                                document.GetCharAt(caretOffset) == currentChar)
                            {
                                e.Handled = true;
                                editor.Caret.Offset++;
                            }
                        }
                    }

                    void OnPreviewKeyDown(object sender, KeyEventArgs e)
                    {
                        if (e.Key == Key.Back)
                        {
                            TabC.linesCountLabel.Content = "Lines: " + (tx.LineCount - 1);

                            var editor = tx.TextArea;
                            var document = editor.Document;
                            var caretOffset = editor.Caret.Offset;

                            if (caretOffset > 0 && caretOffset < document.TextLength)
                            {
                                var prevChar = document.GetCharAt(caretOffset - 1);
                                var nextChar = document.GetCharAt(caretOffset);

                                if (PairCharacters.TryGetValue(prevChar, out var closingChar) &&
                                    nextChar == closingChar)
                                {
                                    document.Remove(caretOffset - 1, 2);
                                    editor.Caret.Offset = caretOffset - 1;
                                    e.Handled = true;
                                }
                            }
                        }
                    }

                }

                TabC.tb.SelectionChanged += new SelectionChangedEventHandler((sender, args) => TabC_SelectionChanged(sender, args));

                if (currentTheme == "light")
                {
                     Color _border = Color.FromRgb(204, 204, 204);

                    imgB.Background = Brushes.White;

                    br.Background = Brushes.White; 
                    tx.Background = Brushes.White;
                    tx.Foreground = Brushes.Black;

                    br.BorderBrush = new SolidColorBrush(_border);
                }

                if (isIMG)
                {
                    tb.Content = imgB;
                }
                else
                {
                    Grid grsc = new Grid();
                    grsc.Children.Add(tx);
                    if (isIMG) grsc.Children.Add(imgB);

                    grsc.Margin = new Thickness(10, 5, 5, 5);
                    // grsc.Children.Add(rct);

                    //sv.Content = textbl;
                    // sv2.Content = grsc;

                    // grbr.Children.Add(sv);
                    // grbr.Children.Add(grsc); // sv2

                    br.Child = grsc;

                    // gr.Children.Add(br);

                    tb.Content = br;
                }

                TabC.tb.Items.Add(tb);

                TabItemModel tab1 = new TabItemModel()
                {
                    FullPath = fullPath,
                    tab = tb
                };

                if(isIMG) tab1.isImg = true;

                TabC.TabItemModel.Add(tab1);

                TabC.tb.SelectedItem = tb;
            }
            return;

            //string selectedText = treeViewItem.Header.ToString();
            //string searchTerm = selectedText.Contains("=") ? selectedText.Split('=')[1].Trim(' ', '"') : selectedText.Split(' ')[0];

            //HighlightTag(searchTerm);
        }

        private void UpdateCursorPosition(TextEditor editor)
        {
            var pos = editor.TextArea.Caret.Position;
            TabC.positionCountLabel.Content = $"Position line: {pos.Line}, column: {pos.Column}";
        }
    }

    public class IndentGuideLinesRenderer : IBackgroundRenderer
    {
        private ScrollViewer scrollViewer;
        private TextEditor editor;

        public IndentGuideLinesRenderer(TextEditor editor)
        {
            this.editor = editor;

            scrollViewer = FindVisualChild<ScrollViewer>(editor);
            if (scrollViewer != null)
            {
                // Override the scrolling behavior by handling the ScrollChanged event
                scrollViewer.ScrollChanged += OnScrollChanged;
            }
        }

        public KnownLayer Layer => KnownLayer.Background;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            Size pixelSize = PixelSnapHelpers.GetPixelSize(editor);
            textView.EnsureVisualLines();

            foreach (var visualLine in textView.VisualLines)
            {
                var line = editor.Document.GetLineByNumber(visualLine.FirstDocumentLine.LineNumber);
                var text = editor.Document.GetText(line);
                var indentation = 0;

                int tabSize = editor.Options.IndentationSize; 

                foreach (var c in text)
                {
                    if (c == ' ')
                    {
                        indentation++;
                    }
                    else if (c == '\t')
                    {
                        indentation++;
                    }
                    else
                    {
                        break;
                    }

                    if (indentation >= 2 && (indentation - 2) % 4 == 0)
                    {
                        double scrollOffsetX = textView.ScrollOffset.X;

                        var startX = textView.GetVisualPosition(
                            new TextViewPosition(line.LineNumber, indentation),
                            VisualYPosition.TextTop
                        ).X - scrollOffsetX - 5;

                        var startY = visualLine.GetTextLineVisualYPosition(visualLine.TextLines[0], VisualYPosition.LineTop) - editor.VerticalOffset;
                        var endY = visualLine.GetTextLineVisualYPosition(visualLine.TextLines[0], VisualYPosition.LineBottom) - editor.VerticalOffset;

                        Pen dottedPen = new Pen(Brushes.Gray, 1)
                        {
                            DashStyle = DashStyles.Dot,
                            StartLineCap = PenLineCap.Square,
                            EndLineCap = PenLineCap.Square,
                            LineJoin = PenLineJoin.Miter
                        };

                        if (IsDivisibleByMultipleOf(indentation, 8))
                            drawingContext.DrawLine(dottedPen, new System.Windows.Point(startX - pixelSize.Width / 2, startY + pixelSize.Height / 2), new System.Windows.Point(startX - pixelSize.Width / 2, endY + pixelSize.Height / 2));
                        else
                            drawingContext.DrawLine(dottedPen, new System.Windows.Point(startX, startY), new System.Windows.Point(startX, endY));
                    }
                }
            }
        }

        // Helper method to find a child of a specified type in the visual tree
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        // Function to check if a number is divisible by a multiple of another number
        private bool IsDivisibleByMultipleOf(int number, int multiple)
        {
            return number % multiple == 0;
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Calls the InvalidateVisual method to refresh the vertical guide lines on scroll
            editor.TextArea.TextView.InvalidateVisual();
        }
    }
}
