using System;
using Hineini.Utility;

namespace Hineini {
    public class Messages {
        public static string BacklightMessage {
            get {
                string backlightDescription = null;
                switch (Settings.Backlight) {
                    case Constants.BACKLIGHT_SYSTEM_MANAGED:
                        backlightDescription = Constants.BACKLIGHT_SYSTEM_MANAGED_DESCRIPTION;
                        break;
                    case Constants.BACKLIGHT_ALWAYS_ON:
                        backlightDescription = Constants.BACKLIGHT_ALWAYS_ON_DESCRIPTION;
                        break;
                }
                string result = String.Format(Constants.BACKLIGHT_MESSAGE_TEMPLATE, backlightDescription);
                return result;
            }
        }

        public static string LocateViaMessage {
            get {
                string result = String.Format(Constants.LOCATE_VIA_MESSAGE_TEMPLATE, Descriptions.LocateViaDescription);
                return result;
            }
        }

        public static string UpdateIntervalMessage {
            get {
                int updateIntervalInMinutes = Settings.UpdateIntervalInMinutes;
                string updatingDescription;
                if (Constants.UPDATE_INTERVAL_MANUAL_ONLY.Equals(updateIntervalInMinutes)) {
                    updatingDescription = MainForm.NeedToShowWelcomeMessage ? Constants.UPDATE_INTERVAL_MESSAGE_MANUAL : Constants.UPDATE_INTERVAL_MESSAGE_MANUAL_AFTER_AUTOMATIC;
                }
                else {
                    updatingDescription = String.Format(Constants.UPDATE_INTERVAL_MESSAGE_AUTOMATIC_TEMPLATE, updateIntervalInMinutes, (updateIntervalInMinutes == 1 ? "minute" : "minutes"));
                }
                return updatingDescription;
            }
        }

        public static string GpsStationaryThresholdMessage {
            get {
                string result = String.Format(Constants.GPS_STATIONARY_THRESHOLD_MESSAGE_TEMPLATE, Descriptions.GpsStationaryThresholdDescription);
                return result;
            }
        }
    }
}