using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Code_Editor.Modules
{
    internal class SearchModule
    {
        public string Text { get; set; }

        public SearchModule(string Text) 
        {
            this.Text = Text;
        }

        public List<string> SearchCode(string searchText)
        {
            List<string> lines = new List<string>();
            string[] splitText = Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 0; i < splitText.Length; i++)
            {
                if (splitText[i].Contains(searchText))
                {
                    lines.Add(splitText[i]);
                }
            }
            return lines;
        }

        public List<TreeViewItem> SearchFiles(TreeViewItem parent, string searchText)
        {
            List<TreeViewItem> result = new List<TreeViewItem>();

            foreach (TreeViewItem item in parent.Items)
            {
                if (item.Header is string header && header.Contains(searchText))
                {
                    result.Add(item);
                }

                if (item.Items.Count > 0)
                {
                    result.AddRange(SearchFiles(item, searchText));
                }
            }

            return result;
        }

        public List<TreeViewItem> SearchFiles(TreeView treeView, string searchText)
        {
            List<TreeViewItem> result = new List<TreeViewItem>();

            foreach (TreeViewItem item in treeView.Items)
            {
                result.AddRange(SearchFiles(item, searchText));
            }

            return result;
        }
    }
}
