using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Hineini.Encryption;
using Hineini.FireEagle;
using Hineini.Utility;

namespace Hineini {
    /// <summary>
    /// Class for managing user settings
    /// </summary>
    public class Settings {
        private static readonly NameValueCollection _settings;
        private static readonly string _settingsPath;
        private static readonly List<int> validUpdateIntervalValues = new List<int> { -1, 1, 5, 15, 30, 60 };
        private static readonly List<int> validMapZoomLevels = new List<int> { Constants.MAP_ZOOM_LEVEL_LEAST, Constants.MAP_ZOOM_LEVEL_LESS, Constants.MAP_ZOOM_LEVEL_MORE, Constants.MAP_ZOOM_LEVEL_MOST };
        private static readonly List<double> validGpsStationaryThresholdValues = new List<double> { Constants.GPS_STATIONARY_THRESHOLD_DISABLED, Constants.GPS_STATIONARY_THRESHOLD_30_FEET, Constants.GPS_STATIONARY_THRESHOLD_60_FEET, Constants.GPS_STATIONARY_THRESHOLD_300_FEET, Constants.GPS_STATIONARY_THRESHOLD_QUARTER_MILE, Constants.GPS_STATIONARY_THRESHOLD_HALF_MILE, Constants.GPS_STATIONARY_THRESHOLD_MILE };
        private static string _fireEagleToken;
        private static string _fireEagleSecret;
        private const int UPDATE_INTERVAL_IN_MINUTES_DEFAULT_VALUE = 5;
        private const int MAP_ZOOM_LEVEL_DEFAULT_VALUE = Constants.MAP_ZOOM_LEVEL_MORE;
        private const double GPS_STATIONARY_THRESHOLD_DEFAULT_VALUE = Constants.GPS_STATIONARY_THRESHOLD_QUARTER_MILE;
        
        static Settings() {
            _settingsPath = Helpers.GetWorkingDirectoryFileName("Settings.xml");

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(_settingsPath);
            XmlElement root = xdoc.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes.Item(0).ChildNodes;

            _fireEagleToken = GetDecrypted(DataReader.TOKEN_FILENAME);
            _fireEagleSecret = GetDecrypted(DataReader.SECRET_FILENAME);

            _settings = new NameValueCollection();
            _settings.Add("UpdateIntervalInMinutes", nodeList.Item(0).Attributes["value"].Value);
            _settings.Add("TowerLocationProvidersList", nodeList.Item(1).Attributes["value"].Value);
            _settings.Add("LocateViaList", nodeList.Item(2).Attributes["value"].Value);
            _settings.Add("Backlight", nodeList.Item(3).Attributes["value"].Value);
            _settings.Add("GpsStationaryThresholdInMiles", nodeList.Item(4).Attributes["value"].Value);
            _settings.Add("MapEnabled", nodeList.Item(5).Attributes["value"].Value);
            _settings.Add("MapCenterMarkerSize", nodeList.Item(6).Attributes["value"].Value);
            _settings.Add("MapZoomLevel", nodeList.Item(7).Attributes["value"].Value);
            _settings.Add("ExtraLogEnabled", nodeList.Item(8).Attributes["value"].Value);
        }

        private static string GetDecrypted(string which) {
            string result = string.Empty;
            byte[] encryptedToken = DataReader.ReadData(which);
            if (encryptedToken != null) {
                try {
                    result = Encryptor.Decrypt(encryptedToken, Encryptor.Password);
                }
                catch (Exception e) {
                    Helpers.WriteToExtraLog(e.Message, e);
                }
            }
            return result;
        }

        private static string FireEagleToken {
            get { return _fireEagleToken; }
            set { _fireEagleToken = value; }
        }

        private static string FireEagleSecret {
            get { return _fireEagleSecret; }
            set { _fireEagleSecret = value; }
        }

        public static string TowerLocationProvidersList {
            get { return _settings.Get("TowerLocationProvidersList"); }
            set { _settings.Set("TowerLocationProvidersList", value); }
        }

        public static double GpsStationaryThresholdInMiles {
            get {
                double result = 0.0;
                try {
                    result = double.Parse(_settings.Get("GpsStationaryThresholdInMiles"), Constants.EnglishUnitedStatesNumberFormatInfo);
                }
                catch (Exception e) {
                    Helpers.WriteToExtraLog(e.Message, e);
                    GpsStationaryThresholdInMiles = result;
                }
                if (!validGpsStationaryThresholdValues.Contains(result)) {
                    result = GPS_STATIONARY_THRESHOLD_DEFAULT_VALUE;
                }
                return result;
            }
            set { _settings.Set("GpsStationaryThresholdInMiles", value.ToString(Constants.EnglishUnitedStatesNumberFormatInfo)); }
        }

        public static bool MapEnabled {
            get { return System.Boolean.TrueString.Equals(_settings.Get("MapEnabled")); }
            set { _settings.Set("MapEnabled", value.ToString()); }
        }

        public static string LocateViaList {
            get { return _settings.Get("LocateViaList"); }
            set { _settings.Set("LocateViaList", value); }
        }

        public static string Backlight {
            get { return _settings.Get("Backlight"); }
            set { _settings.Set("Backlight", value); }
        }

        public static void Update() {
            XmlTextWriter writer = new XmlTextWriter(_settingsPath, Encoding.UTF8);
            writer.WriteStartDocument();
            writer.WriteStartElement("configuration");
            writer.WriteStartElement("appSettings");

            for (int i = 0; i < _settings.Count; ++i) {
                writer.WriteStartElement("add");
                writer.WriteStartAttribute("key", String.Empty);
                writer.WriteRaw(_settings.GetKey(i));
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("value", String.Empty);
                writer.WriteRaw(_settings.Get(i));
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.Close();

            byte[] tokenDataToWrite = !Helpers.StringHasValue(FireEagleToken) ? Encoding.UTF8.GetBytes(string.Empty) : Encryptor.Encrypt(FireEagleToken, Encryptor.Password);
            DataReader.WriteData(tokenDataToWrite, DataReader.TOKEN_FILENAME);
            byte[] secretDataToWrite = !Helpers.StringHasValue(FireEagleSecret) ? Encoding.UTF8.GetBytes(string.Empty) : Encryptor.Encrypt(FireEagleSecret, Encryptor.Password);
            DataReader.WriteData(secretDataToWrite, DataReader.SECRET_FILENAME);
        }

        public static void VerifyTowerLocationsSetting() {
            string towerLocationProvidersList = TowerLocationProvidersList;
            if (towerLocationProvidersList != Constants.TOWER_LOCATIONS_YAHOO_ALWAYS && towerLocationProvidersList != Constants.TOWER_LOCATIONS_YAHOO_THEN_GOOGLE && towerLocationProvidersList != Constants.TOWER_LOCATIONS_GOOGLE_ALWAYS) {
                TowerLocationProvidersList = Constants.TOWER_LOCATIONS_YAHOO_ALWAYS;
                Update();
            }
        }

        public static void VerifyLocateViaSetting() {
            string locateViaList = LocateViaList;
            if (locateViaList != Constants.LOCATE_VIA_GPS_ONLY && locateViaList != Constants.LOCATE_VIA_GPS_THEN_TOWERS && locateViaList != Constants.LOCATE_VIA_TOWERS_ONLY) {
                LocateViaList = Constants.LOCATE_VIA_TOWERS_ONLY;
                Update();
            }
        }

        public static void VerifyBacklightSetting() {
            string backlight = Backlight;
            if (backlight != Constants.BACKLIGHT_SYSTEM_MANAGED && backlight != Constants.BACKLIGHT_ALWAYS_ON) {
                Backlight = Constants.BACKLIGHT_SYSTEM_MANAGED;
                Update();
            }
        }

        public static Token FireEagleUserToken {
            get {
                Token result = new Token(FireEagleToken, FireEagleSecret);
                return result;
            }
            set {
                FireEagleToken = value.PublicToken;
                FireEagleSecret = value.SecretToken;
            }
        }

        public static int MapCenterMarkerSize {
            get {
                int result = 5;
                try {
                    result = Int32.Parse(_settings.Get("MapCenterMarkerSize"));
                }
                catch (Exception e) {
                    Helpers.WriteToExtraLog(e.Message, e);
                }
                return result;
            }
            set { _settings.Set("MapCenterMarkerSize", value.ToString()); }
        }

        public static int MapZoomLevel {
            get {
                int result = 0;
                try {
                    result = Int32.Parse(_settings.Get("MapZoomLevel"));
                }
                catch (Exception e) {
                    Helpers.WriteToExtraLog(e.Message, e);
                }
                if (!validMapZoomLevels.Contains(result)) {
                    result = MAP_ZOOM_LEVEL_DEFAULT_VALUE;
                }
                return result;
            }
            set { _settings.Set("MapZoomLevel", value.ToString()); }
        }

        public static int UpdateIntervalInMinutes {
            get {
                int result = 0;
                try {
                    result = Int32.Parse(_settings.Get("UpdateIntervalInMinutes"));
                }
                catch (Exception e) {
                    Helpers.WriteToExtraLog(e.Message, e);
                }
                if (!validUpdateIntervalValues.Contains(result)) {
                    result = UPDATE_INTERVAL_IN_MINUTES_DEFAULT_VALUE;
                }
                return result;
            }
            set { _settings.Set("UpdateIntervalInMinutes", value.ToString()); }
        }

        public static bool ExtraLogEnabled {
            get { return System.Boolean.TrueString.Equals(_settings.Get("ExtraLogEnabled")); }
            set { _settings.Set("ExtraLogEnabled", value.ToString()); }
        }
    }
}