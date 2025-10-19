using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Editor.Classes
{
    [Serializable]
    public class SaveTabs
    {
        public string current_path { get; set; }

        public List<TabControlModel> tabCModels { get; set; } = new List<TabControlModel>();

        public SaveTabs() { }
    }
}
