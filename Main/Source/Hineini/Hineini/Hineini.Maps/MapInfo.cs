using Hineini.FireEagle;

namespace Hineini.Maps {
    public class MapInfo {
        private LatLong locationLatLong = null;
        public MapInfo(LatLong locationLatLong, LatLong upperCorner, LatLong lowerCorner, int mapZoomLevel) {
            LocationLatLong = locationLatLong;
            UpperCornerLatLong = upperCorner;
            LowerCornerLatLong = lowerCorner;
            MapZoomLevel = mapZoomLevel;
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
                        double centerLongitude = (UpperCornerLatLong.Longitude + UpperCornerLatLong.Longitude) / 2;
                        result = new LatLong(centerLatitude, centerLongitude);
                    }
                }
                return result;
            }
            set {
                locationLatLong = value;
            }
        }

        public int MapZoomLevel { get; set; }
    }
}