using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Schema;
using System.Xml.Serialization;
using Hineini.Utility;

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
                    Helpers.WriteToExtraLog("ExactPoint is null due to point_raw.", null);
                    if (point_raw == null) {
                        Helpers.WriteToExtraLog("point_raw is null.", null);
                    }
                    return null;
                }

                string[] parts = this.point_raw.Split(' ');
                Helpers.WriteToExtraLog("ExactPoint parts length: " + parts.Length, null);
                if (parts.Length == 2)
                {
                    Helpers.WriteToExtraLog("ExactPoint parts: '" + parts[0] + "', '" + parts[1] + "'", null);
                    return this.ParseLatLong(parts[0], parts[1]);
                }

                Helpers.WriteToExtraLog("ExactPoint is null.", null);
                return null;
            }
        }

        /// <summary>
        /// Gets the LatLong representing the lower corner of a box returned
        /// </summary>
        public LatLong LowerCorner
        {
            get {
                LogBoxRaw(box_raw);
                return GetBoxCorner("LowerCorner");
            }
        }

        private LatLong GetBoxCorner(string whichCorner) {
            if (String.IsNullOrEmpty(this.box_raw))
            {
                Helpers.WriteToExtraLog(whichCorner + " is null due to box_raw.", null);
                if (box_raw == null) {
                    Helpers.WriteToExtraLog("box_raw is null.", null);
                }

                return null;
            }

            string[] parts = BowRawStringArray;
            Helpers.WriteToExtraLog(whichCorner + " parts length: " + parts.Length, null);
            if (parts.Length == 4)
            {
                int partStartIndex = "LowerCorner".Equals(whichCorner) ? 0 : 2;
                return this.ParseLatLong(parts[partStartIndex], parts[partStartIndex + 1]);
            }

            Helpers.WriteToExtraLog(whichCorner + " is null.", null);
            return null;
        }

        protected string[] BowRawStringArray {
            get {
                // return box_raw.Split(' ');
                return System.Text.RegularExpressions.Regex.Split(box_raw.Trim(), @"\s+");
            }
        }

        /// <summary>
        /// Gets the LatLong representing the lower corner of a box returned
        /// </summary>
        public LatLong UpperCorner
        {
            get
            {
                LogBoxRaw(box_raw);
                return GetBoxCorner("UpperCorner");
            }
        }

        private void LogBoxRaw(string boxRaw) {
            Helpers.WriteToExtraLog("box_raw in ticks: '" + boxRaw + "'", null);
            Helpers.WriteToExtraLog("box_raw TRIMMED in ticks: '" + boxRaw.Trim() + "'", null);

            char[] characters = boxRaw.ToCharArray();
            string[] integers = new string[characters.Length];
            Helpers.WriteToExtraLog("box_raw characters length: " + characters.Length, null);
            for (int i = 0; i < characters.Length; i++) {
                integers[i] = ((int)characters[i]).ToString();
            }
            Helpers.WriteToExtraLog("box_raw integers length: " + integers.Length, null);
            string integersString = string.Join(",", integers);
            Helpers.WriteToExtraLog("box_raw integers: " + integersString, null);
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