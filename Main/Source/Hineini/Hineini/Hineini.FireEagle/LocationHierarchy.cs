using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Schema;
using System.Xml.Serialization;

namespace Hineini.FireEagle {
    [XmlRoot("location-hierarchy")]
    [Serializable]
    public class LocationHierarchy
    {
        private Location[] m_locations = new Location[0];
        private Location m_bestguess;
        private Location m_mostrecent;

        [XmlElement("location", Form = XmlSchemaForm.Unqualified)]
        public Location[] LocationCollection
        {
            get { return m_locations; }
            set
            {
                m_locations = value == null ? new Location[0] : value;
                DateTime mostRecentLocationDate = DateTime.MinValue;
                foreach (Location l in m_locations)
                {
                    if (l.isBestGuess) {
                        m_bestguess = l;
                    }
                    DateTime locationDate = l.LocationDate;
                    if (locationDate > mostRecentLocationDate) {
                        mostRecentLocationDate = locationDate;
                        m_mostrecent = l;
                    }
                }
            }
        }

        public Location BestGuess
        {
            get
            {
                return m_bestguess;
            }
        }

        public Location MostRecent {
            get { return m_mostrecent; }
        }
    }
}