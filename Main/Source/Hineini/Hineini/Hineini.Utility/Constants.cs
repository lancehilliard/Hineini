using System;
using System.Globalization;

namespace Hineini.Utility {
    public class Constants {
        public const string IMAGE_URL_FETCH_FAILED_MESSAGE = "Failed building map image URL...";
        public const string TAG_INFO_MESSAGE = "Help your friends get Hineini - simply show the Hineini tag on your phone's screen and point someone's Microsoft Tag Reader at it!\n\nWould you like to visit the Microsoft Tag homepage to learn more?";
        public const string TAG_INFO_CAPTION = "Microsoft Tag";
        public const string UPDATE_SKIPPED_GPS_THRESHOLD = "No update (within GPS Threshold)";
        public const string ABOUT_FILENAME = "About.html";
        public const string USER_MANUAL_FILENAME = "UserManual.html";
        public const string CLIENT_UPDATE_AVAILABLE_MENU_ITEM_TEXT = "Client Update Available";
        public const string CLIENT_UPDATE_AVAILABLE_MESSAGE = "An updated version of Hineini can be found at http://hineini.codeplex.com.";
        public const string CURRENT_VERSION = "0.6.6";
        public const string RECOMMENDED_VERSION_HTML_DELIMITER_END = " --";
        public const string RECOMMENDED_VERSION_HTML_DELIMITER_START = "!-- Recommend ";
        public const string RECOMMENDED_VERSION_URL = "http://hineini.codeplex.com/Wiki/View.aspx?title=recommendedVersion";
        public const string MAP_URL_TEMPLATE = "http://maps.google.com/staticmap?size={0}x{1}&maptype=mobile&key=ABQIAAAAu-YXjAmyKTn4bLyq60KPJxRCmR3BMzCOmnDxzV__D6GogjP-bxS2YsxdOmDDPViifiljA1OCCzYkPQ&sensor=false&center={2},{3}&zoom={4}";
        public const string FETCHING_MAP_MESSAGE = "Fetching map...";
        public const double DISTANCE_UNKNOWN = -1;
        public const string GETTING_AUTH_TOKEN_MESSAGE = "Getting auth token...";
        public const string RETRIEVED_MAP_URL_MESSAGE = "Succeeded building map image URL...";
        public const string GETTING_MAP_IMAGE_MESSAGE = "Getting map image...";
        public const string MAP_FETCH_FAILED_MESSAGE = "Map fetch failed...";
        public const string USER_SUPPLIED_ADDRESS_MUST_CONTAIN_ZIPCODE = "Address must contain zipcode...";
        public const int MAP_ZOOM_LEVEL_CLOSER = 16;
        public const int MAP_ZOOM_LEVEL_MIDDLE = 15;
        public const int MAP_ZOOM_LEVEL_FARTHER = 11;
        public const string LOCATION_NOT_YET_IDENTIFIED = "Location not yet identified...";
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
        public const string AUTHORIZATION_REQUEST_TEMPLATE = "Visit the URL below to authorize Hineini.  Part of the authorization is a verification code.  Press 'Verify' when you have it.\r\n\r\n{0}";
        public const string FAILED_UPDATE_MESSAGE_IDLE = "Won't update until you choose 'Update'.";
        public const string LOADING_REQUEST_AUTHORIZATION_MESSAGE = "Starting Fire Eagle authorization setup...";
        public const string MANUAL_UPDATE_STARTED_MESSAGE = "Manual update started...";
        public const string UNABLE_TO_IDENTIFY_CELL_TOWERS_DEBUG_MESSAGE = "Unable to get cell tower info string...";
        public const string UNABLE_TO_IDENTIFY_CELL_TOWERS_MESSAGE = "Trying to identify cell towers...";
        public const string UNABLE_TO_IDENTIFY_CELL_TOWERS_WITH_CELL_TOWERS_DISABLED_DEBUG_MESSAGE = "Locate via Cell Towers to see cell info...";
        public const string UPDATE_INTERVAL_MESSAGE_MANUAL = "Update interval: Manually";
        public const string UPDATE_INTERVAL_MESSAGE_MANUAL_AFTER_AUTOMATIC = "Update interval: Manually";
        public const string PROJECT_NAME_WITH_VERSION = "Hineini (v" + CURRENT_VERSION + ")";
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

        public static readonly IFormatProvider EnglishUnitedStatesNumberFormatInfo = new NumberFormatInfo() { NumberGroupSeparator = "." };

        public enum MessageType {
            Info = 0,
            Error = 1
        }

        //public const string YAHOO_MAPS_APPID = "lr79Yr_V34HxBTNJGpnzRLJXIo8y8HDN_9MamXpcC_XPSJMhADy4pPWrXIq4jddw";
        //public const string REGEX_POSTAL_CODE = @"\d{5}$";
    }
}