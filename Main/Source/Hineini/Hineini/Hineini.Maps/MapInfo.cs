namespace Hineini.Maps {
    public class MapInfo {
        public MapInfo(string locationName, FireEagle.LatLong locationLatLong, int mapZoomLevel) {
            LocationName = locationName;
            LocationLatLong = locationLatLong;
            MapZoomLevel = mapZoomLevel;
        }

        public string LocationName { get; set; }

        public FireEagle.LatLong LocationLatLong { get; set; }

        public int MapZoomLevel { get; set; }
    }
}