using System;
using Hineini.FireEagle;
using Hineini.Utility;

namespace Hineini.Maps {
    public class MapInfo {
        private LatLong locationLatLong;
        public MapInfo(LatLong locationLatLong, LatLong upperCorner, LatLong lowerCorner) {
            LocationLatLong = locationLatLong;
            UpperCornerLatLong = upperCorner;
            LowerCornerLatLong = lowerCorner;
        }

        public string GetMapUrl(int height, int width) {
            string mapPath = MapPath;
            string zoomedCenter = "center=" + LocationLatLong.Latitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo) + "," + LocationLatLong.Longitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo) + "&zoom=" + Constants.MAP_ZOOM_LEVEL_MIDDLE;
            string location = mapPath.Length > 0 ? mapPath : zoomedCenter;
            string result = string.Format("http://maps.google.com/staticmap?format=jpg-baseline&size={0}x{1}&maptype=mobile&key=ABQIAAAAu-YXjAmyKTn4bLyq60KPJxRCmR3BMzCOmnDxzV__D6GogjP-bxS2YsxdOmDDPViifiljA1OCCzYkPQ&sensor=false&{2}", width, height, location);
            return result;
        }

        private LatLong LowerCornerLatLong { get; set; }

        private LatLong UpperCornerLatLong { get; set; }

        public LatLong LocationLatLong { 
            get {
                LatLong result = null;
                if (locationLatLong != null) {
                    result = locationLatLong;
                }
                else {
                    if (LowerCornerLatLong != null && UpperCornerLatLong != null) {
                        double centerLatitude = (LowerCornerLatLong.Latitude + UpperCornerLatLong.Latitude) / 2;
                        double centerLongitude = (LowerCornerLatLong.Longitude + UpperCornerLatLong.Longitude) / 2;
                        result = new LatLong(centerLatitude, centerLongitude);
                    }
                }
                return result;
            }
            set {
                locationLatLong = value;
            }
        }

        public string MapPath {
            get {
                string result = string.Empty;
                if (LowerCornerLatLong != null && UpperCornerLatLong != null) {
                    string lowerLeftCorner = LowerCornerLatLong.Latitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo) + "," + LowerCornerLatLong.Longitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo);
                    string upperLeftCorner = UpperCornerLatLong.Latitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo) + "," + LowerCornerLatLong.Longitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo);
                    string upperRightCorner = UpperCornerLatLong.Latitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo) + "," + UpperCornerLatLong.Longitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo);
                    string lowerRightCorner = LowerCornerLatLong.Latitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo) + "," + UpperCornerLatLong.Longitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo);
                    result = "path=rgb:0x000000,weight:2|" + lowerLeftCorner + "|" + upperLeftCorner + "|" + upperRightCorner + "|" + lowerRightCorner + "|" + lowerLeftCorner;
                }
                return result;
            }
        }
    }
}