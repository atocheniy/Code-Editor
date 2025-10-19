using Code_Editor.Classes;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Code_Editor
{
    public class AI
    {
        private HttpClient httpClient;
        string API;
        static string answer_;

        public AI(string API_key)
        {
            httpClient = new HttpClient();
            answer_ = "";
            API = API_key;
        }

        public async Task<string> GetAnswer(string input)
        {
            await AskMistral("Отвечай на все инструкции в формате кода. Запрос:" + input);
            return answer_;
        }

        private async Task AskMistral(string question)
        {
            answer_ = "";

            string api_key = API; 

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", api_key);

            string url = "https://api-inference.huggingface.co/models/mistralai/Mistral-7B-Instruct-v0.3";

            var payload = new
            {
                inputs = $"[INST] {question} [/INST]",
                parameters = new { max_new_tokens = 1000 }
            };

            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    var root = JsonSerializer.Deserialize<List<MistralResponse>>(result);
                    string fullText = root?[0].generated_text ?? "";

                    string answer = fullText.Split(new[] { "[/INST]" }, StringSplitOptions.None)
                        .LastOrDefault()?.Trim() ?? "";


                    answer_ = answer;
                }
                else
                {
                    answer_ = $"Ошибка запроса: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                answer_ = $"Исключение: {ex.Message}";
            }
        }

        class MistralResponse
        {
            public string generated_text { get; set; }
        }

        public async Task SendButton(List<TabControlModel> tabCModels, TextBox AITextBox, Label PathFile, StackPanel AnsverPanel)
        {
            double _targetOffset = 0;
            double ScrollSpeed = 70;

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

            if (selectedTab.Content is Border br && br.Child is Grid grid &&
                grid.Children.OfType<TextEditor>().FirstOrDefault() is TextEditor tx)
            {

                string s = tx.Text;
                string ai_text = await GetAnswer("Код: " + s + "Запрос: " + AITextBox);

                Border b = new Border()
                {
                    Margin = new Thickness(10, 15, 10, 0),
                    CornerRadius = new CornerRadius(10),
                    ClipToBounds = true,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF212121")),
                    VerticalAlignment = VerticalAlignment.Top,
                };

                Grid g = new Grid();
                ScrollViewer sv = new ScrollViewer()
                {
                    Margin = new Thickness(0, 10, 0, 10),
                    Style = (Style)Application.Current.Resources["ScrollViewerStyle1"],
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    PanningMode = PanningMode.Both,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                };

                TextEditor te = new TextEditor()
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    ShowLineNumbers = true,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF212121")),
                    Foreground = new SolidColorBrush(Colors.White),
                };

                string language = PathFile.Content as string;
                language = language.Replace('>', '\\');

                if (System.IO.Path.GetExtension(language) == ".cs")
                {
                    te.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
                }
                else if (System.IO.Path.GetExtension(language) == ".py")
                {
                    te.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Python");
                }
                else if (System.IO.Path.GetExtension(language) == ".js")
                {
                    te.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
                }
                else if (System.IO.Path.GetExtension(language) == ".html")
                {
                    te.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML");
                }
                else if (System.IO.Path.GetExtension(language) == ".css")
                {
                    te.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CSS");
                }
                else if (System.IO.Path.GetExtension(language) == ".xml" || System.IO.Path.GetExtension(language) == ".csproj" || System.IO.Path.GetExtension(language) == ".config")
                {
                    te.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
                }
                else if (System.IO.Path.GetExtension(language) == ".xaml")
                {
                    te.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
                }

                te.Text = ai_text;


                sv.ScrollChanged += (se, es) =>
                {

                    sv.ScrollToVerticalOffset(sv.VerticalOffset);
                };
                sv.PreviewMouseWheel += (se, es) =>
                {
                    if (se is ScrollViewer scrollViewer)
                    {
                        es.Handled = true;

                        _targetOffset = Math.Max(0, Math.Min(scrollViewer.ScrollableHeight, _targetOffset - es.Delta / 120 * ScrollSpeed));

                        var animation = new DoubleAnimation
                        {
                            To = _targetOffset,
                            Duration = TimeSpan.FromMilliseconds(300),
                            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                        };

                        scrollViewer.BeginAnimation(Modules.ScrollViewerOffsetHelper.VerticalOffsetProperty, animation);
                    }
                };

                g.Children.Add(sv);
                g.Children.Add(te);

                b.Child = g;

                AnsverPanel.Children.Add(b);
            }
        }
    }
}
