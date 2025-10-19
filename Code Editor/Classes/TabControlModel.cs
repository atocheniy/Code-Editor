using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Code_Editor.Classes
{
    [Serializable]
    public class TabControlModel
    {
        public int ID { get; set; }

        [XmlIgnore]
        public Label linesCountLabel { get; set; }

        [XmlIgnore]
        public Label positionCountLabel { get; set; }

        [XmlIgnore]
        public Label languageLabel { get; set; }

        [XmlIgnore]
        public TabControl tb { get; set; }

        public List<TabItemModel> TabItemModel { get; set; } = new List<TabItemModel>();

        public TabControlModel() { }
    }
}
