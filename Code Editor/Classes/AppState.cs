using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Editor.Classes
{
    public class AppState
    {
        public string CurrentTheme { get; set; }
        public string CurrentFolder { get; set; }
        public string Setting { get; set; }
        public int FontSize { get; set; }
        public bool IsLoaded { get; set; }
    }
}
