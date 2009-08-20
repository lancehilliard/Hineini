using System;
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
                result = Constants.GPS_STATIONARY_THRESHOLD_DISABLED.Equals(Settings.GpsStationaryThresholdInMiles);
            }
            else if (menuItem.Text.StartsWith("30 ")) {
                result = Constants.GPS_STATIONARY_THRESHOLD_30_FEET.Equals(Settings.GpsStationaryThresholdInMiles);
            }
            else if (menuItem.Text.StartsWith("60 ")) {
                result = Constants.GPS_STATIONARY_THRESHOLD_60_FEET.Equals(Settings.GpsStationaryThresholdInMiles);
            }
            else if (menuItem.Text.StartsWith("300 ")) {
                result = Constants.GPS_STATIONARY_THRESHOLD_300_FEET.Equals(Settings.GpsStationaryThresholdInMiles);
            }
            else if (menuItem.Text.StartsWith("1/4 ")) {
                result = Constants.GPS_STATIONARY_THRESHOLD_QUARTER_MILE.Equals(Settings.GpsStationaryThresholdInMiles);
            }
            else if (menuItem.Text.StartsWith("1/2 ")) {
                result = Constants.GPS_STATIONARY_THRESHOLD_HALF_MILE.Equals(Settings.GpsStationaryThresholdInMiles);
            }
            else if (menuItem.Text.StartsWith("1 ")) {
                result = Constants.GPS_STATIONARY_THRESHOLD_MILE.Equals(Settings.GpsStationaryThresholdInMiles);
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

        public static void SetMapCenterMarkerSizeCheckmarks(MenuItem centerMarkerMenuItem) {
            foreach (MenuItem menuItem in centerMarkerMenuItem.MenuItems) {
                menuItem.Checked = MapCenterMarkerSizeMenuItemMatchesSetting(menuItem);
            }
        }

        private static bool MapCenterMarkerSizeMenuItemMatchesSetting(MenuItem menuItem) {
            bool result = false;
            if (menuItem.Text.StartsWith("Disabled")) {
                result = Constants.CENTER_MARKER_SIZE_DISABLED.Equals(Settings.MapCenterMarkerSize);
            }
            else if (menuItem.Text.StartsWith("Small")) {
                result = Constants.CENTER_MARKER_SIZE_SMALL.Equals(Settings.MapCenterMarkerSize);
            }
            else if (menuItem.Text.StartsWith("Medium")) {
                result = Constants.CENTER_MARKER_SIZE_MEDIUM.Equals(Settings.MapCenterMarkerSize);
            }
            else if (menuItem.Text.StartsWith("Large")) {
                result = Constants.CENTER_MARKER_SIZE_LARGE.Equals(Settings.MapCenterMarkerSize);
            }
            return result;
        }

        public static void SetMapZoomLevelCheckmarks(MenuItem mapZoomLevelMenuItem) {
            foreach (MenuItem menuItem in mapZoomLevelMenuItem.MenuItems) {
                menuItem.Checked = MapZoomLevelMatchesSetting(menuItem);
            }
        }

        private static bool MapZoomLevelMatchesSetting(MenuItem menuItem) {
            bool result = false;
            if (menuItem.Text.StartsWith("Least")) {
                result = Constants.MAP_ZOOM_LEVEL_LEAST.Equals(Settings.MapZoomLevel);
            }
            else if (menuItem.Text.StartsWith("Less")) {
                result = Constants.MAP_ZOOM_LEVEL_LESS.Equals(Settings.MapZoomLevel);
            }
            else if (menuItem.Text.StartsWith("More")) {
                result = Constants.MAP_ZOOM_LEVEL_MORE.Equals(Settings.MapZoomLevel);
            }
            else if (menuItem.Text.StartsWith("Most")) {
                result = Constants.MAP_ZOOM_LEVEL_MOST.Equals(Settings.MapZoomLevel);
            }
            return result;
        }
    }
}