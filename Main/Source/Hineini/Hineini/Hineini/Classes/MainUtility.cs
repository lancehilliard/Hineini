using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Hineini.FireEagle;
using Hineini.Utility;

namespace Hineini {
    public class MainUtility {
        public static void ChangeTowerLocationsSetting(string towerLocationsProvidersList, bool showMessage, MenuItem towerLocationsMenuItem, MenuItem yahooAlwaysMenuItem, MenuItem googleSometimesMenuItem, MenuItem googleAlwaysMenuItem) {
            Settings.TowerLocationProvidersList = towerLocationsProvidersList;
            Settings.Update();
            MenuItems.SetTowerLocationsMenuItemCheckmarks(towerLocationsMenuItem, yahooAlwaysMenuItem, googleSometimesMenuItem, googleAlwaysMenuItem);
            if (Boolean.TowersEnabled) {
                ReadyFireEagleProcessingTimerForNextUpdate();
            }
            if (showMessage) {
                MessagesForm.AddMessage(DateTime.Now, Descriptions.TowerProviders, Constants.MessageType.Info);
                MessagesForm.AddMessage(DateTime.Now, Messages.LocateViaMessage, Constants.MessageType.Info);
            }
        }

        public static void ChangeUpdateIntervalSetting(int updateIntervalInMinutes, MenuItem updateIntervalMenuItem, MenuItem manuallyMenuItem) {
            Settings.UpdateIntervalInMinutes = updateIntervalInMinutes;
            Settings.Update();
            MenuItems.SetUpdateIntervalMenuItemCheckmarks(updateIntervalMenuItem, manuallyMenuItem);
            MessagesForm.AddMessage(DateTime.Now, Messages.UpdateIntervalMessage, Constants.MessageType.Info);
            ReadyFireEagleProcessingTimerForNextUpdate();
        }

        public static void ChangeGpsStationaryThresholdSetting(double gpsStationaryThresholdInMiles, MenuItem gpsStationaryThresholdMenuItem) {
            Settings.GpsStationaryThresholdInMiles = gpsStationaryThresholdInMiles;
            Settings.Update();
            MenuItems.SetGpsStationaryThresholdMenuItemCheckmarks(gpsStationaryThresholdMenuItem);
            MessagesForm.AddMessage(DateTime.Now, Messages.GpsStationaryThresholdMessage, Constants.MessageType.Info);
            ReadyFireEagleProcessingTimerForNextUpdate();
        }

        public static void ReadyFireEagleProcessingTimerForNextUpdate() {
            MainForm.SecondsBeforeNextFireEagleProcessing = Constants.UPDATE_INTERVAL_MANUAL_ONLY.Equals(Settings.UpdateIntervalInMinutes) ? Settings.UpdateIntervalInMinutes : 0;
        }

        public static void SetTimerPerSuccessfulOrHandledUpdate() {
            if (Constants.UPDATE_INTERVAL_MANUAL_ONLY.Equals(Settings.UpdateIntervalInMinutes)) {
                MainForm.SecondsBeforeNextFireEagleProcessing = Constants.UPDATE_INTERVAL_MANUAL_ONLY;
            }
            else {
                MainForm.SecondsBeforeNextFireEagleProcessing = Settings.UpdateIntervalInMinutes*60;
            }
        }

        public static string GetExceptionMessage(Exception e1) {
            string message = e1.InnerException == null ? e1.Message : e1.InnerException.Message;
            return message;
        }

        public static void HandleFailedUpdate() {
            string message;
            if (Constants.UPDATE_INTERVAL_MANUAL_ONLY.Equals(Settings.UpdateIntervalInMinutes)) {
                message = Constants.FAILED_UPDATE_MESSAGE_IDLE;
                MessagesForm.AddMessage(DateTime.Now, message, Constants.MessageType.Info);
                MainForm.SecondsBeforeNextFireEagleProcessing = Settings.UpdateIntervalInMinutes;
            }
            else {
                message = String.Format(Constants.FAILED_UPDATE_MESSAGE_AUTOMATIC_TEMPLATE, Descriptions.LocateViaDescription);
                MessagesForm.AddMessage(DateTime.Now, message, Constants.MessageType.Error);
                MainForm.SecondsBeforeNextFireEagleProcessing = 2;
            }
        }

        public static string GetLocationMarker(object locationObject) {
            string result = String.Empty;
            Position? position = locationObject as Position?;
            if (position.HasValue) {
                Position positionValue = position.Value;
                result = String.Format("{0}{1}", positionValue.Latitude, positionValue.Longitude);
            }
            else if (locationObject is CellTower) {
                CellTower cellTower = (CellTower)locationObject;
                result = String.Format("{0}{1}", cellTower.lac, cellTower.cellid);
            }
            else if (locationObject is Address) {
                Address address = (Address)locationObject;
                result = String.Format("{0}{1}", address.StreetAddress, address.Postal);
            }
            return result;
        }

        public static string GetLocationMessage(string locationMessagePrefix, FireEagle.Location location) {
            string locationName = (!Helpers.StringHasValue(locationMessagePrefix) ? String.Empty : locationMessagePrefix) + (location == null ? "Unnamed location" : location.Name);
            string result = locationName;
            return result;
        }

        public static string GetShortTimeString(DateTime dateTime) {
            return dateTime.ToShortTimeString();
        }

        public static void ChangeLocateViaSetting(string locateViaList, bool showMessage, MenuItem locateViaMenuItem, MenuItem gpsOnlyMenuItem, MenuItem towersSometimesMenuItem, MenuItem towersOnlyMenuItem) {
            Settings.LocateViaList = locateViaList;
            Settings.Update();
            MainForm.LocationManager.UseTowers = !Constants.LOCATE_VIA_GPS_ONLY.Equals(Settings.LocateViaList);
            MainForm.LocationManager.UseGps = !Constants.LOCATE_VIA_TOWERS_ONLY.Equals(Settings.LocateViaList);
            MenuItems.SetLocateViaMenuItemCheckmarks(locateViaMenuItem, gpsOnlyMenuItem, towersSometimesMenuItem, towersOnlyMenuItem);
            MainForm.ResetLastPositionMarkers();
            ReadyFireEagleProcessingTimerForNextUpdate();
            if (showMessage) {
                MessagesForm.AddMessage(DateTime.Now, Messages.LocateViaMessage, Constants.MessageType.Info);
            }
        }

        public static void ChangeBacklightSetting(string backlight, bool showMessage, MenuItem backlightMenuItem, MenuItem systemManagedMenuItem, MenuItem alwaysOnMenuItem) {
            Settings.Backlight = backlight;
            Settings.Update();
            MenuItems.SetBacklightMenuItemCheckmarks(backlightMenuItem, systemManagedMenuItem, alwaysOnMenuItem);
            if (showMessage) {
                MessagesForm.AddMessage(DateTime.Now, Messages.BacklightMessage, Constants.MessageType.Info);
            }
        }

        [DllImport("coredll.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void ActivateBacklight() {
                //Backlight.Activate();
                // TODO refactor backlight code
                byte VK_F24 = 0x87;
                int KEYEVENTF_KEYUP = 2;
                keybd_event(VK_F24, 0, KEYEVENTF_KEYUP, 0);
        }

        private static string GetWorkingDirectory() {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + @"\";
        }

        public static string GetWorkingDirectoryFileName(string fileName) {
            return GetWorkingDirectory() + fileName;
        }

        public static void SetBorders(Control edit, bool border, bool focusBorder) {
            // Optionally remove the border around the edit control
            // that occurs whenever the control has focus
            if (!focusBorder) {
                SendMessage(edit.Handle, EM_SETEXTENDEDSTYLE, ES_EX_FOCUSBORDERDISABLED, ES_EX_FOCUSBORDERDISABLED);
            }
            // A .NET CF Edit control is actually two windows (this is an internal
            // implementation detail so is subject to change) so add the WS_BORDER
            // style to the window that contains the native edit control in order
            // to get a border.
            int style = GetWindowLong(GetParent(edit.Handle), GWL_STYLE);
            if (border) {
                style |= WS_BORDER;
            }
            else {
                style &= ~WS_BORDER;
            }
            SetWindowLong(GetParent(edit.Handle), GWL_STYLE, style);
        }

        [DllImport("coredll.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int idx);

        [DllImport("coredll.dll")]
        private static extern void SetWindowLong(IntPtr hWnd, int idx, int value);

        [DllImport("coredll.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("coredll.dll", EntryPoint = "SendMessage")]
        private static extern uint SendMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);


        private const int GWL_STYLE = -16;
        private const int WS_BORDER = 0x00800000;
        private const uint EM_SETEXTENDEDSTYLE = 0x00E0;
        private const uint ES_EX_FOCUSBORDERDISABLED = 0x00000002;

        public static int GetMapZoomLevel(FireEagle.Location location) {
            List<LocationType> granularLocationTypes = new List<LocationType> {LocationType.exact, LocationType.address};
            int result = granularLocationTypes.Contains(location.LevelName) ? Constants.MAP_ZOOM_LEVEL_CLOSER : Constants.MAP_ZOOM_LEVEL_FARTHER;
            //MessagesForm.AddMessage(DateTime.Now, location.LevelName + " = " + result, Constants.MessageType.Error);
            return result;
        }

        public static void ReleaseBacklightIfNoLongerActiveApplication(bool wasActiveApplicationAtLastTick, bool isActiveApplication) {
            if (wasActiveApplicationAtLastTick && !isActiveApplication) {
                Backlight.Release();
            }
        }

        public static void ResizeLabel(Label label, Graphics graphics) {
            SizeF size = graphics.MeasureString(label.Text, label.Font);
            int width = (int)size.Width;
            width = (int) (width*1.03);
            label.Size = new Size(width, label.Size.Height);
        }
    }
}