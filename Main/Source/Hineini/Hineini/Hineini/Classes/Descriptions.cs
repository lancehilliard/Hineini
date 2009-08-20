using System;
using Hineini.Utility;

namespace Hineini {
    public class Descriptions {

        public static string TowerProviders {
            get {
                string result = null;
                switch (Settings.TowerLocationProvidersList) {
                    case Constants.TOWER_LOCATIONS_YAHOO_ALWAYS:
                        result = Constants.TOWER_LOCATIONS_YAHOO_ALWAYS_DESCRIPTION;
                        break;
                    case Constants.TOWER_LOCATIONS_YAHOO_THEN_GOOGLE:
                        result = Constants.TOWER_LOCATIONS_YAHOO_THEN_GOOGLE_DESCRIPTION;
                        break;
                    case Constants.TOWER_LOCATIONS_GOOGLE_ALWAYS:
                        result = Constants.TOWER_LOCATIONS_GOOGLE_ALWAYS_DESCRIPTION;
                        break;
                }
                result = String.Format(Constants.TOWER_LOCATIONS_MESSAGE_TEMPLATE, result);
                return result;
            }
        }

        public static string LocateViaDescription {
            get {
                string locateViaDescription = String.Empty;
                switch (Settings.LocateViaList) {
                    case Constants.LOCATE_VIA_GPS_ONLY:
                        locateViaDescription = Constants.LOCATE_VIA_GPS_ONLY_DESCRIPTION;
                        break;
                    case Constants.LOCATE_VIA_GPS_THEN_TOWERS:
                        locateViaDescription = Constants.LOCATE_VIA_GPS_THEN_TOWERS_DESCRIPTION;
                        break;
                    case Constants.LOCATE_VIA_TOWERS_ONLY:
                        locateViaDescription = Constants.LOCATE_VIA_TOWERS_ONLY_DESCRIPTION;
                        break;
                }
                return locateViaDescription;
            }
        }

        public static string GpsStationaryThresholdDescription {
            get {
                string result = String.Empty;
                if (Constants.GPS_STATIONARY_THRESHOLD_DISABLED.Equals(Settings.GpsStationaryThresholdInMiles)) {
                    result = "None";
                }
                if (Constants.GPS_STATIONARY_THRESHOLD_30_FEET.Equals(Settings.GpsStationaryThresholdInMiles)) {
                    result = "30 feet";
                }
                if (Constants.GPS_STATIONARY_THRESHOLD_60_FEET.Equals(Settings.GpsStationaryThresholdInMiles)) {
                    result = "60 feet";
                }
                if (Constants.GPS_STATIONARY_THRESHOLD_300_FEET.Equals(Settings.GpsStationaryThresholdInMiles)) {
                    result = "300 feet";
                }
                else if (Constants.GPS_STATIONARY_THRESHOLD_QUARTER_MILE.Equals(Settings.GpsStationaryThresholdInMiles)) {
                    result = "1/4 mile";
                }
                else if (Constants.GPS_STATIONARY_THRESHOLD_HALF_MILE.Equals(Settings.GpsStationaryThresholdInMiles)) {
                    result = "1/2 mile";
                }
                else if (Constants.GPS_STATIONARY_THRESHOLD_MILE.Equals(Settings.GpsStationaryThresholdInMiles)) {
                    result = "1 mile";
                }
                return result;
            }
        }
    }
}