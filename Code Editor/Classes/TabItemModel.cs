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
    public class TabItemModel
    {
        public string FullPath { get; set; }

        [XmlIgnore]
        public TabItem tab { get; set; }

        public bool isImg { get; set; }

        public TabItemModel() { }
    }
}
