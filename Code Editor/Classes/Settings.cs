using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Editor.Classes
{
    [Serializable]
    public class Settings
    {
        public string ResolutionWidth { get; set; }
        public string ResolutionHeight { get; set; }
        public string SidebarColumnWidth { get; set; }
        public string ContentColumnWidth { get; set; }
        public string OpacityTransparent { get; set; }
        public int FontSize { get; set; }
        public string CurrentTheme { get; set; }

        public Settings() { }
    }
}
