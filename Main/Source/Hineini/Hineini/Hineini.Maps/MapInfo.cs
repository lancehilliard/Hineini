using Hineini.FireEagle;

namespace Hineini.Maps {
    public class MapInfo {
        public MapInfo(string locationName, LatLong locationLatLong, LatLong upperCorner, LatLong lowerCorner, int mapZoomLevel) {
            LocationName = locationName;
            LocationLatLong = locationLatLong;
            UpperCornerLatLong = upperCorner;
            LowerCornerLatLong = lowerCorner;
            MapZoomLevel = mapZoomLevel;
        }

        public LatLong LowerCornerLatLong { get; set; }

        public LatLong UpperCornerLatLong { get; set; }

        public string LocationName { get; set; }

        public LatLong LocationLatLong { get; set; }

        public int MapZoomLevel { get; set; }
    }
}