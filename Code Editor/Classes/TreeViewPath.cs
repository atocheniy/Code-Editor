using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Code_Editor
{
    internal class TreeViewPath
    {
        public List<string> expandedPaths { get; set; }
        public string selectedPath { get; set; }

        public TreeViewPath()
        {
            expandedPaths = new List<string>();
            selectedPath = null;
        }
    }
}
