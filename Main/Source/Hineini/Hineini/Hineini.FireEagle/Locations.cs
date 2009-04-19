using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Serialization;
using System.Xml.Schema;

namespace Hineini.FireEagle {
    [XmlRoot("locations")]
    [Serializable]
    public class Locations
    {
        private Location[] m_locations = new Location[0];
        private string m_query = "";

        public string QueryString
        {
            get { return m_query; }
            set
            {
                m_query = value;
            }
        }

        [XmlElement("location", Form = XmlSchemaForm.Unqualified)]
        public Location[] LocationCollection
        {
            get { return m_locations; }
            set
            {
                m_locations = value == null ? new Location[0] : value;
            }
        }

        [XmlAttribute("start", Form = XmlSchemaForm.Unqualified)]
        public long Start;

        [XmlAttribute("total", Form = XmlSchemaForm.Unqualified)]
        public long TotalLocations;

        [XmlAttribute("count", Form = XmlSchemaForm.Unqualified)]
        public long Count;
    }
}