namespace Hineini.Utility {
    public class Constants {
        public static double DISTANCE_UNKNOWN = -1;
        public static string GETTING_AUTH_TOKEN_MESSAGE = "Getting auth token...";
        public static string GETTING_MAP_IMAGE_MESSAGE = "Getting map image...";
        public static string UPDATING_PENDING_MAP_IMAGE_MESSAGE = "Updating pending map image...";
        public static string USER_SUPPLIED_ADDRESS_MUST_CONTAIN_ZIPCODE = "Address must contain zipcode...";
        public static int MAP_ZOOM_LEVEL_CLOSER = 3;
        public static int MAP_ZOOM_LEVEL_FARTHER = 6;
        public const string LOADING_MAP_MESSAGE = "Loading map...";
        public const string HINEINI_CONSUMER_KEY = "GaAkDhCTfNsa";
        public const string HINEINI_CONSUMER_SECRET = "q1tIZbRR0FP4yHpZWwoyLzVWrD1RoMJt";
        public const bool IS_DEVELOPMENT_RUNTIME = true;
        public const string BACKLIGHT_SYSTEM_MANAGED_DESCRIPTION = "Use system setting";
        public const string BACKLIGHT_ALWAYS_ON_DESCRIPTION = "Always on";
        public const string LOCATE_VIA_GPS_ONLY_DESCRIPTION = "GPS only";
        public const string LOCATE_VIA_GPS_THEN_TOWERS_DESCRIPTION = "GPS (Cell Towers backup)";
        public const string LOCATE_VIA_TOWERS_ONLY_DESCRIPTION = "Cell Towers only";
        public const string TOWER_LOCATIONS_YAHOO_ALWAYS_DESCRIPTION = "Yahoo always";
        public const string TOWER_LOCATIONS_YAHOO_THEN_GOOGLE_DESCRIPTION = "Yahoo (Google backup)";
        public const string TOWER_LOCATIONS_GOOGLE_ALWAYS_DESCRIPTION = "Google always";
        public const string BACKLIGHT_MESSAGE_TEMPLATE = "Backlight: {0}";
        public const string GPS_STATIONARY_THRESHOLD_MESSAGE_TEMPLATE = "GPS Threshold: {0}";
        public const string ADD_MESSAGE_TEMPLATE = "{0}: {1}";
        public const string FAILED_UPDATE_MESSAGE_AUTOMATIC_TEMPLATE = "Attempting update via {0}...";
        public const string LOCATE_VIA_MESSAGE_TEMPLATE = "Locate w/: {0}";
        public const string TOWER_LOCATIONS_MESSAGE_TEMPLATE = "Geolocate towers w/: {0}";
        public const string UPDATE_INTERVAL_MESSAGE_AUTOMATIC_TEMPLATE = "Locate every: {0} {1}";
        public const string SHOW_MESSAGES_NEXT_MESSAGE_TEMPLATE = "{0}\r\n{1}";
        public const string AUTHORIZATION_REQUEST_TEMPLATE = "Visit the URL below to authorize Hineini.  When the authorization is confirmed, Hineini will automatically start updating your location to Fire Eagle after a short delay.\r\n\r\n{0}";
        public const string FAILED_UPDATE_MESSAGE_IDLE = "Won't update until you choose 'Update'.";
        public const string LOADING_REQUEST_AUTHORIZATION_MESSAGE = "Starting Fire Eagle authorization setup...";
        public const string MANUAL_UPDATE_STARTED_MESSAGE = "Manual update started...";
        public const string UNABLE_TO_IDENTIFY_CELL_TOWERS_DEBUG_MESSAGE = "Unable to get cell tower info string...";
        public const string UNABLE_TO_IDENTIFY_CELL_TOWERS_MESSAGE = "Trying to identify cell towers...";
        public const string UNABLE_TO_IDENTIFY_CELL_TOWERS_WITH_CELL_TOWERS_DISABLED_DEBUG_MESSAGE = "Locate via Cell Towers to see cell info...";
        public const string UPDATE_INTERVAL_MESSAGE_MANUAL = "Update interval: Manually";
        public const string UPDATE_INTERVAL_MESSAGE_MANUAL_AFTER_AUTOMATIC = "Update interval: Manually";
        public const string VERSIONED_WELCOME_MESSAGE = "Hineini (v0.0.3.1)";
        public const string BACKLIGHT_ALWAYS_ON = "AlwaysOn";
        public const string BACKLIGHT_SYSTEM_MANAGED = "SystemManaged";
        public const char ESCAPE_CHARACTER = '\x001b';
        public const string LOCATE_VIA_GPS_ONLY = "GPS";
        public const string LOCATE_VIA_GPS_THEN_TOWERS = "GPS,Towers";
        public const string LOCATE_VIA_TOWERS_ONLY = "Towers";
        public const string LOCATION_DESCRIPTION_PREFIX_GOOGLE = "~";
        public const string LOCATION_DESCRIPTION_PREFIX_GPS = "+";
        public const string LOCATION_DESCRIPTION_PREFIX_USERSUPPLIED = "*";
        public const string TOWER_LOCATIONS_YAHOO_ALWAYS = "Yahoo";
        public const string TOWER_LOCATIONS_YAHOO_THEN_GOOGLE = "Yahoo,Google";
        public const string TOWER_LOCATIONS_GOOGLE_ALWAYS = "Google";
        public const int UPDATE_INTERVAL_MANUAL_ONLY = -1;

        public enum MessageType {
            Info = 0,
            Error = 1
        }

        public const string YAHOO_MAPS_APPID = "lr79Yr_V34HxBTNJGpnzRLJXIo8y8HDN_9MamXpcC_XPSJMhADy4pPWrXIq4jddw";
        public const string REGEX_POSTAL_CODE = @"\d{5}$";
    }
}