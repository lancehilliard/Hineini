using System.Windows.Forms;
using Hineini.Utility;

namespace Hineini {
    public class MenuItems {
        public static void SetUpdateIntervalMenuItemCheckmarks(MenuItem updateIntervalMenuItem, MenuItem manuallyMenuItem) {
            foreach (MenuItem menuItem in updateIntervalMenuItem.MenuItems) {
                menuItem.Checked = menuItem.Text.StartsWith(Settings.UpdateIntervalInMinutes + " ");
            }
            manuallyMenuItem.Checked = (Constants.UPDATE_INTERVAL_MANUAL_ONLY).Equals(Settings.UpdateIntervalInMinutes);
        }

        public static void SetGpsStationaryThresholdMenuItemCheckmarks(MenuItem gpsStationaryThresholdMenuItem) {
            foreach (MenuItem menuItem in gpsStationaryThresholdMenuItem.MenuItems) {
                menuItem.Checked = GpsStationaryThresholdMenuItemMatchesSetting(menuItem);
            }
        }

        private static bool GpsStationaryThresholdMenuItemMatchesSetting(MenuItem menuItem) {
            bool result = false;
            if (menuItem.Text.StartsWith("None")) {
                result = 0.0.Equals(Settings.GpsStationaryThresholdInMiles);
            }
            else if (menuItem.Text.StartsWith("1/4 ")) {
                result = 0.25.Equals(Settings.GpsStationaryThresholdInMiles);
            }
            else if (menuItem.Text.StartsWith("1/2 ")) {
                result = 0.5.Equals(Settings.GpsStationaryThresholdInMiles);
            }
            else if (menuItem.Text.StartsWith("1 ")) {
                result = 1.0.Equals(Settings.GpsStationaryThresholdInMiles);
            }
            return result;
        }

        public static void SetTowerLocationsMenuItemCheckmarks(MenuItem towerLocationsMenuItem, MenuItem yahooAlwaysMenuItem, MenuItem googleSometimesMenuItem, MenuItem googleAlwaysMenuItem) {
            foreach (MenuItem menuItem in towerLocationsMenuItem.MenuItems) {
                menuItem.Checked = false;
            }
            MenuItem selectedMenuItem = null;
            switch (Settings.TowerLocationProvidersList) {
                case Constants.TOWER_LOCATIONS_YAHOO_ALWAYS:
                    selectedMenuItem = yahooAlwaysMenuItem;
                    break;
                case Constants.TOWER_LOCATIONS_YAHOO_THEN_GOOGLE:
                    selectedMenuItem = googleSometimesMenuItem;
                    break;
                case Constants.TOWER_LOCATIONS_GOOGLE_ALWAYS:
                    selectedMenuItem = googleAlwaysMenuItem;
                    break;
            }
            if (selectedMenuItem != null) {
                selectedMenuItem.Checked = true;
            }
        }

        public static void SetLocateViaMenuItemCheckmarks(MenuItem locateViaMenuItem, MenuItem gpsOnlyMenuItem, MenuItem towersSometimesMenuItem, MenuItem towersOnlyMenuItem) {
            foreach (MenuItem menuItem in locateViaMenuItem.MenuItems) {
                if (menuItem.MenuItems.Count == 0) {
                    menuItem.Checked = false;
                }
            }
            MenuItem selectedMenuItem = null;
            switch (Settings.LocateViaList) {
                case Constants.LOCATE_VIA_GPS_ONLY:
                    selectedMenuItem = gpsOnlyMenuItem;
                    break;
                case Constants.LOCATE_VIA_GPS_THEN_TOWERS:
                    selectedMenuItem = towersSometimesMenuItem;
                    break;
                case Constants.LOCATE_VIA_TOWERS_ONLY:
                    selectedMenuItem = towersOnlyMenuItem;
                    break;
            }
            if (selectedMenuItem != null) {
                selectedMenuItem.Checked = true;
            }
        }

        public static void SetBacklightMenuItemCheckmarks(MenuItem backlightMenuItem, MenuItem systemManagedMenuItem, MenuItem alwaysOnMenuItem) {
            foreach (MenuItem menuItem in backlightMenuItem.MenuItems) {
                menuItem.Checked = false;
            }
            MenuItem selectedMenuItem = null;
            switch (Settings.Backlight) {
                case Constants.BACKLIGHT_SYSTEM_MANAGED:
                    selectedMenuItem = systemManagedMenuItem;
                    break;
                case Constants.BACKLIGHT_ALWAYS_ON:
                    selectedMenuItem = alwaysOnMenuItem;
                    break;
            }
            if (selectedMenuItem != null) {
                selectedMenuItem.Checked = true;
            }
        }

    }
}