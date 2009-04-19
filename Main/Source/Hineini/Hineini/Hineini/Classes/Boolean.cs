using Hineini.Utility;
using Microsoft.WindowsMobile.Status;

namespace Hineini {
    public class Boolean {
        public static bool TowersEnabled {
            get {
                bool result = !Constants.LOCATE_VIA_GPS_ONLY.Equals(Settings.LocateViaList);
                return result;
            }
        }

        public static bool IsActiveApplication {
            // BUG this returns true if a File Explorer looking at a folder (whose name matches _formName) and is the active application
            get {
                bool result = false;
                string applicationsState = SystemState.ActiveApplication;
                if (applicationsState != null) {
                    string[] applicationNames = applicationsState.Split(Constants.ESCAPE_CHARACTER);
                    if (applicationNames.Length > 1) {
                        string activeApplicationName = applicationNames[1];
                        if (activeApplicationName != null) {
                            result = activeApplicationName.Equals(MainForm.FormName);
                        }
                    }
                }
                return result;
            }
        }

        public static bool BacklightAlwaysOn {
            get {
                bool result = Constants.BACKLIGHT_ALWAYS_ON.Equals(Settings.Backlight);
                return result;
            }
        }
    }
}