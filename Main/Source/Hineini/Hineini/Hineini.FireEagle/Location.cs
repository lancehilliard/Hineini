using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Schema;
using System.Xml.Serialization;

namespace Hineini.FireEagle {
    [XmlRoot("location")]
    [Serializable]
    public class Location
    {
        [XmlElement("name", Form = XmlSchemaForm.Unqualified)]
        public string Name;

        [XmlElement("place-id", Form = XmlSchemaForm.Unqualified)]
        public string PlaceID;

        [XmlElement("woeid", Form = XmlSchemaForm.Unqualified)]
        public int WOEID;

        [XmlAttribute("best-guess", Form = XmlSchemaForm.Unqualified)]
        public bool isBestGuess;

        [XmlElement("id", Form = XmlSchemaForm.Unqualified)]
        public long ID;

        [XmlElement("level", Form = XmlSchemaForm.Unqualified)]
        public long Level;

        [XmlElement("level-name", Form = XmlSchemaForm.Unqualified)]
        public LocationType LevelName;

        [XmlElement("located-at", Form = XmlSchemaForm.Unqualified)]
        public string locatedAt_raw;

        [XmlElement("point", Form = XmlSchemaForm.Qualified, Namespace = "http://www.georss.org/georss")]
        public string point_raw;

        [XmlElement("box", Form = XmlSchemaForm.Qualified, Namespace = "http://www.georss.org/georss")]
        public string box_raw;

        public DateTime LocationDate
        {
            get
            {
                if (string.IsNullOrEmpty(locatedAt_raw)) return DateTime.MinValue;
                else return DateTime.Parse(locatedAt_raw);
            }
        }

        /// <summary>
        /// Gets the LatLong representing the exact point returned
        /// </summary>
        public LatLong ExactPoint
        {
            get
            {
                if (String.IsNullOrEmpty(this.point_raw))
                {
                    return null;
                }

                string[] parts = this.point_raw.Split(' ');
                if (parts.Length == 2)
                {
                    return this.ParseLatLong(parts[0], parts[1]);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the LatLong representing the lower corner of a box returned
        /// </summary>
        public LatLong LowerCorner
        {
            get
            {
                if (String.IsNullOrEmpty(this.box_raw))
                {
                    return null;
                }

                string[] parts = this.box_raw.Split(' ');
                if (parts.Length == 4)
                {
                    return this.ParseLatLong(parts[0], parts[1]);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the LatLong representing the lower corner of a box returned
        /// </summary>
        public LatLong UpperCorner
        {
            get
            {
                if (String.IsNullOrEmpty(this.box_raw))
                {
                    return null;
                }

                string[] parts = this.box_raw.Split(' ');
                if (parts.Length == 4)
                {
                    return this.ParseLatLong(parts[2], parts[3]);
                }

                return null;
            }
        }

        /// <summary>
        /// Tries to build a LatLong from two strings
        /// </summary>
        /// <param name="lat">Latitude string</param>
        /// <param name="lon">Longitude string</param>
        /// <returns>LatLong if valid, or null if not</returns>
        private LatLong ParseLatLong(string lat, string lon)
        {
            double latitude = double.MinValue;
            double longitude = double.MinValue;

            try {
                latitude = Double.Parse(lat);
                longitude = Double.Parse(lon);
                LatLong latLong = new LatLong(latitude, longitude);

                if (latLong.Valid()) {
                    return latLong;
                }
            }
            catch (Exception e) {}

            return null;
        }
    }
}