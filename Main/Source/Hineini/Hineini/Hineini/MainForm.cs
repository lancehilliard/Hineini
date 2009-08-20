using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Hineini.FireEagle;
using Hineini.Location;
using Hineini.Location.Towers;
using Hineini.Maps;
using Hineini.Utility;

namespace Hineini {
    public partial class MainForm : Form {
        #region Fields
        private static string _formName;
        private static bool _needToShowIntroductionMessage = true;
        private static bool _messageWaitingToBeShown;
        private static int _secondsBeforeNextFireEagleProcessing;
        private static LocationManager _locationManager;
        private Thread _processFireEagleWorkerThread;
        private Token _requestToken;
        private string _fireEagleRequestAuthorizationUrl = string.Empty;
        private readonly FireEagle.FireEagle _fireEagle = new FireEagle.FireEagle(Constants.HINEINI_CONSUMER_KEY, Constants.HINEINI_CONSUMER_SECRET);
        private PreAuthForm _preAuthForm;
        private VerifyForm _verifyForm;
        private MessagesForm _messagesForm;
        private string _userUpdateLocation;
        private int _mapHeight;
        private int _mapWidth;
        private Bitmap _pendingMapImage;
        //private bool _mapImageIsPending;
        //private MapInfo _pendingMapInfo;
        private bool _needToHidePreAuthorizationFormAndShowMainForm;
        private bool _timerHasTicked;
        private bool _manualUpdateRequested;
        private static string _lastLocationMarker;
        private static Position _lastUpdatedPosition;
        private bool versionCheckPerformed;
        private bool _userShouldBeAdvisedAboutRecommendedVersion;
        private TagForm tagForm;
        FireEagle.Location _mostRecentLocation;
        private string _lastMappedUrl = string.Empty;
        private string _pendingMapUrl;
        private bool _attemptingUpdate;

        #endregion

        #region Properties

        public static LocationManager LocationManager {
            get { return _locationManager = _locationManager ?? new LocationManager(); }
        }

        private MessagesForm MessagesForm {
            get {
                return _messagesForm = _messagesForm ?? new MessagesForm();
            }
        }

        private VerifyForm VerifyForm {
            get {
                return _verifyForm = _verifyForm ?? new VerifyForm();
            }
        }

        private PreAuthForm PreAuthForm {
            get {
                return _preAuthForm = _preAuthForm ?? new PreAuthForm();
            }
        }

        public static string FormName {
            get { return _formName; }
            set { _formName = value; }
        }

        public static bool MessageWaitingToBeShown {
            get { return _messageWaitingToBeShown; }
            set { _messageWaitingToBeShown = value; }
        }

        public string FireEagleRequestAuthorizationUrl {
            get { return _fireEagleRequestAuthorizationUrl; }
            set { _fireEagleRequestAuthorizationUrl = value; }
        }

        public static bool NeedToShowIntroductionMessage {
            get { return _needToShowIntroductionMessage; }
            set { _needToShowIntroductionMessage = value; }
        }

        public static int SecondsBeforeNextFireEagleProcessing {
            get { return _secondsBeforeNextFireEagleProcessing; }
            set { _secondsBeforeNextFireEagleProcessing = value; }
        }

        #endregion


        #region Non-repeating code

        public MainForm() {

            InitializeComponent();

            _fireEagle.UserToken = Settings.FireEagleUserToken;

            InitializeUpdateControls();

            bool authorizedForFireEagle = _fireEagle.UserToken != null && Helpers.StringHasValue(_fireEagle.UserToken.SecretToken);

            SetupMainFormObjects(authorizedForFireEagle);

            ResetFireEagleWorkerThread();
        }

        private void ApplyEventHandlers() {
            PreAuthForm.Exit += _preAuthForm_Exit;
            PreAuthForm.Verify += _preAuthForm_Verify;
            VerifyForm.Verify += _verifyForm_Verify;
            VerifyForm.Exit += _verifyForm_Exit;
            MessagesForm.HideForm += _messagesForm_HideForm;
        }

        private void _verifyForm_Verify(object sender, VerifyEventArguments verifyEventArguments) {
            try {
                string oauth_verifier = verifyEventArguments.Verifier;
                //Helpers.WriteToExtraLog("Verify with '" + oauth_verifier + "'", null);
                _fireEagle.UserToken = _fireEagle.OAuthGetToken(_requestToken, oauth_verifier);
                _verifyForm.Status = "Succeeded!";
                //Helpers.WriteToExtraLog("Verify succeeded", null);
                Settings.FireEagleUserToken = _fireEagle.UserToken;
                Settings.Update();
                //whatisupwiththisline FireEagleRequestAuthorizationUrl = string.Empty;
                _needToHidePreAuthorizationFormAndShowMainForm = true;
            }
            catch (Exception e) {
                _verifyForm.Status = "Failed.  Try Again.";
                Helpers.WriteToExtraLog("Verify failed...", e);
                Helpers.WriteToExtraLog(e.Message, e);
            }
        }

        private void _preAuthForm_Verify(object sender, EventArgs e) {
            VerifyForm.Show();
            PreAuthForm.Hide();
        }

        private void InitializeUpdateControls() {
            MainUtility.ResizeLabel(updateLinkLabel, CreateGraphics(), locationPictureBox.Width);
            updateLinkLabel.Width += 4;
            MainUtility.SetBorders(updateTextBox, false, false);
        }

        private static void VerifySettings() {
            Settings.VerifyTowerLocationsSetting();
            Settings.VerifyLocateViaSetting();
            Settings.VerifyBacklightSetting();
        }

        private void UpdateInitialSettings() {
            MainUtility.ChangeTowerLocationsSetting(Settings.TowerLocationProvidersList, false, towerLocationsMenuItem, yahooAlwaysMenuItem, googleSometimesMenuItem, googleAlwaysMenuItem);
            MainUtility.ChangeLocateViaSetting(Settings.LocateViaList, false, locateViaMenuItem, gpsOnlyMenuItem, towersSometimesMenuItem, towersOnlyMenuItem);
            MainUtility.ChangeBacklightSetting(Settings.Backlight, false, backlightMenuItem, systemManagedMenuItem, alwaysOnMenuItem);
            MainUtility.ChangeMapEnabledSetting(Settings.MapEnabled, false, mapEnabledMenuItem);
            MainUtility.ChangeMapCenterMarkerSizeSetting(Settings.MapCenterMarkerSize, centerMarkerMenuItem);
            MainUtility.ChangeMapZoomLevelSetting(Settings.MapZoomLevel, zoomLevelMenuItem);
            MainUtility.ChangeExtraLogEnabledSetting(Settings.ExtraLogEnabled, extraLogMenuItem);
        }

        private void SetupMainFormObjects(bool authorizedForFireEagle) {
            locationPicturePanel.Visible = authorizedForFireEagle;
            mostRecentInfoMessageLabel.Visible = authorizedForFireEagle;
            updatePanel.Visible = authorizedForFireEagle;
            mainMenu.Enabled = authorizedForFireEagle;
            mainMenu.Text = authorizedForFireEagle ? "Menu" : string.Empty;
        }

        private void _messagesForm_HideForm(object sender, EventArgs e) {
            Show();
            MessagesForm.Hide();
        }

        void _preAuthForm_Exit(object sender, EventArgs e) {
            Quit();
        }

        void _verifyForm_Exit(object sender, EventArgs e) {
            Quit();
        }

        private void ResetFireEagleWorkerThread() {
            _processFireEagleWorkerThread = new Thread(FireEagleWorker);
            _processFireEagleWorkerThread.Start();
        }

        private static void ShowIntroductionMessage() {
            DateTime now = DateTime.Now;
            MessagesForm.AddMessage(now, Messages.UpdateIntervalMessage, Constants.MessageType.Info);
            MessagesForm.AddMessage(now, Descriptions.TowerProviders, Constants.MessageType.Info);
            MessagesForm.AddMessage(now, Messages.LocateViaMessage, Constants.MessageType.Info);
            MessagesForm.AddMessage(now, Messages.BacklightMessage, Constants.MessageType.Info);
            if (!Constants.UPDATE_INTERVAL_MANUAL_ONLY.Equals(Settings.UpdateIntervalInMinutes)) {
                MessagesForm.AddMessage(DateTime.Now, Constants.LOCATION_NOT_YET_IDENTIFIED, Constants.MessageType.Info);
            }
            NeedToShowIntroductionMessage = false;
        }

        private void PopulateRequestTokenUrl() {
            _requestToken = _fireEagle.OAuthGetRequestToken();
            FireEagleRequestAuthorizationUrl = _fireEagle.OAuthRequestUrl(_requestToken);
        }

        private void Quit() {
            Close();
            if (_processFireEagleWorkerThread != null) {
                QuitFireEagleWorkerThread();
            }
            Helpers.Dispose();
            Application.Exit();
        }

        private void QuitFireEagleWorkerThread() {
            _processFireEagleWorkerThread.Abort();
        }

        #endregion

        private void ShowMessagesForm(Constants.MessageType messageType) {
            MessagesForm.ShowMessages(messageType);
            Hide();
        }

        public static void ResetLastPositionMarkers() {
            _lastLocationMarker = string.Empty;
            _lastUpdatedPosition = new Position();
        }

        private bool UpdateLocationData(ref bool stationaryThresholdPreventedUpdate) {
            bool locationUpdated;
            try {
                if (Helpers.StringHasValue(_userUpdateLocation)) {
                    locationUpdated = UpdateLocationDataByUserInput(_userUpdateLocation);
                }
                else {
                    locationUpdated = UpdateLocationDataByEnvironmentInput(ref stationaryThresholdPreventedUpdate);
                }
            }
            catch (Exception e) {
                Helpers.WriteToExtraLog(e.Message, e);
                throw;
            }
            return locationUpdated;
        }

        private bool UpdateLocationDataByEnvironmentInput(ref bool stationaryThresholdPreventedUpdate) {
            bool locationUpdated = false;
            if (Boolean.GpsEnabled) {
                Position? currentGpsPosition = GetCurrentGpsPosition();
                locationUpdated = UpdateLocationDataByCurrentGpsPosition(currentGpsPosition, ref stationaryThresholdPreventedUpdate);
            }
            if (stationaryThresholdPreventedUpdate) {
                Thread.Sleep(2000);
                MessagesForm.AddMessage(_mostRecentLocation.LocationDate, _mostRecentLocation.Name + Constants.LOCATION_DESCRIPTION_PREFIX_GPS, Constants.MessageType.Info);
            }
            else if (!locationUpdated && LocationManager.UseTowers) {
                locationUpdated = UpdateLocationDataByCellTower();
            }
            return locationUpdated;
        }

        private bool UpdateLocationDataByUserInput(string userUpdateLocation) {
            bool locationUpdated = UpdateLocationDataByAddress(userUpdateLocation);
            return locationUpdated;
        }

        private bool UpdateLocationDataByCellTower() {
            bool locationUpdated;
            object locatedCellTower = null;
            Exception towerLocationException = null;
            try {
                CellTower? yahooLocatedCellTower = GetYahooLocatedCellTower();
                if (yahooLocatedCellTower.HasValue) {
                    locatedCellTower = yahooLocatedCellTower;
                }
            }
            catch (Exception e) {
                towerLocationException = e;
            }
            finally {
                if (locatedCellTower == null) {
                    Position? googleLocatedCellTower = GetGoogleLocatedCellTower();
                    if (googleLocatedCellTower.HasValue) {
                        locatedCellTower = googleLocatedCellTower;
                    }
                    else if (towerLocationException != null) {
                        throw towerLocationException;
                    }
                }
            }
            if (locatedCellTower != null) {
                string locationMessagePrefix = locatedCellTower is Position ? Constants.LOCATION_DESCRIPTION_PREFIX_GOOGLE : null;
                locationUpdated = UpdateLocationData(locatedCellTower, locationMessagePrefix);
            }
            else {
                throw new Exception(Constants.UNABLE_TO_IDENTIFY_CELL_TOWERS_MESSAGE);
            }
            return locationUpdated;
        }

        private bool UpdateLocationDataByCurrentGpsPosition(Position? currentGpsPosition, ref bool stationaryThresholdPreventedUpdate) {
            bool locationUpdated = false;
            bool gpsUpdateShouldProceed = GpsUpdateShouldProceed(currentGpsPosition, ref stationaryThresholdPreventedUpdate);
            if (gpsUpdateShouldProceed) {
                locationUpdated = UpdateLocationData(currentGpsPosition.Value, Constants.LOCATION_DESCRIPTION_PREFIX_GPS);
                _lastUpdatedPosition = currentGpsPosition.Value;
            }
            return locationUpdated;
        }

        private bool UpdateLocationDataByAddress(string userUpdateLocation) {
            bool locationUpdated = false;
            if (GetYahooKnowsLocationOfAddress(userUpdateLocation)) {
                locationUpdated = UpdateLocationData(userUpdateLocation, Constants.LOCATION_DESCRIPTION_PREFIX_USERSUPPLIED);
                _lastUpdatedPosition = new Position();
            }
            return locationUpdated;
        }

        private Position? GetCurrentGpsPosition() {
            return LocationManager.UseGps ? LocationManager.GetValidGpsLocation() : null;
        }

        private Position? GetGoogleLocatedCellTower() {
            Position? result = null;
            if (UserAllowsCellTowerLocatingViaGoogle()) {
                bool googleKnowsLocationOfCurrentCellTower = GetGoogleKnowsLocationOfCurrentCellTower();
                if (googleKnowsLocationOfCurrentCellTower) {
                    result = LocationManager.CurrentCellTowerPosition.Value;
                }
            }
            return result;
        }

        private CellTower? GetYahooLocatedCellTower() {
            CellTower? result = null;
            if (UserAllowsCellTowerLocatingViaYahoo()) {
                bool yahooKnowsLocationOfCurrentCellTower = GetYahooKnowsLocationOfCurrentCellTower();
                if (yahooKnowsLocationOfCurrentCellTower) {
                    result = LocationManager.CurrentCellTower;
                }
            }
            return result;
        }

        private bool GetGoogleKnowsLocationOfCurrentCellTower() {
            Position? currentCellTowerPosition = LocationManager.CurrentCellTowerPosition;
            bool result = currentCellTowerPosition.HasValue;
            return result;
        }

        private bool GetYahooKnowsLocationOfCurrentCellTower() {
            CellTower currentCellTower = LocationManager.CurrentCellTower;
            Locations locations = _fireEagle.Lookup(currentCellTower);
            return locations.LocationCollection.Length > 0;
        }

        private bool GetYahooKnowsLocationOfAddress(string address) {
            Locations locations = _fireEagle.Lookup(address);
            return locations.LocationCollection.Length > 0;
        }

        private bool UserAllowsCellTowerLocatingViaGoogle() {
            return !Constants.TOWER_LOCATIONS_YAHOO_ALWAYS.Equals(Settings.TowerLocationProvidersList);
        }

        private bool UserAllowsCellTowerLocatingViaYahoo() {
            return !Constants.TOWER_LOCATIONS_GOOGLE_ALWAYS.Equals(Settings.TowerLocationProvidersList);
        }

        private bool GpsUpdateShouldProceed(Position? currentGpsPosition, ref bool stationaryThresholdPreventedUpdate) {
            bool updateShouldProceed = false;
            if (currentGpsPosition.HasValue) {
                Position position = currentGpsPosition.Value;
                bool positionIsValid = position.Latitude != 0 && position.Longitude != 0;
                if (positionIsValid) {
                    bool distanceInMilesExceedsGpsStationaryThreshold = DistanceInMilesExceedsGpsStationaryThreshold(position);
                    updateShouldProceed = _manualUpdateRequested || distanceInMilesExceedsGpsStationaryThreshold;
                    if (!updateShouldProceed) {
                        stationaryThresholdPreventedUpdate = true;
                        MessagesForm.AddMessage(DateTime.Now, Constants.UPDATE_SKIPPED_GPS_THRESHOLD, Constants.MessageType.Error);
                    }
                }
                else {
                    MessagesForm.AddMessage(DateTime.Now, Constants.LOCATION_NOT_YET_IDENTIFIED, Constants.MessageType.Error);
                }
            }
            return updateShouldProceed;
        }

        private static bool DistanceInMilesExceedsGpsStationaryThreshold(Position currentGpsPosition) {
            double distanceInMiles = LocationManager.DistanceInMiles(currentGpsPosition, _lastUpdatedPosition);
            bool result = distanceInMiles == Constants.DISTANCE_UNKNOWN || distanceInMiles >= Settings.GpsStationaryThresholdInMiles;
            return result;
        }

        private bool UpdateLocationData(object locationObject, string locationMessagePrefix) {
            bool locationUpdated = false;
            if (locationObject != null) {
                string locationMarker = MainUtility.GetLocationMarker(locationObject);
                bool locationMarkerHasMoved = LocationMarkerHasMoved(locationMarker);
                if (locationObject is string) {
                    _attemptingUpdate = true;
                    _fireEagle.Update(LocationType.address, (string)locationObject);
                    _attemptingUpdate = false;
                    locationUpdated = true;
                }
                else {
                    if (locationObject is CellTower && (_manualUpdateRequested || locationMarkerHasMoved)) {
                        _attemptingUpdate = true;
                        _fireEagle.Update((CellTower)locationObject);
                        _attemptingUpdate = false;
                        locationUpdated = true;
                    }
                    else {
                        bool positionIsGoogleCellTower = Constants.LOCATION_DESCRIPTION_PREFIX_GOOGLE.Equals(locationMessagePrefix);
                        if (_manualUpdateRequested || !positionIsGoogleCellTower || locationMarkerHasMoved) {
                            Position position = (Position)locationObject;
                            if (position.Latitude != 0 && position.Longitude != 0) {
                                _attemptingUpdate = true;
                                _fireEagle.Update(position);
                                _attemptingUpdate = false;
                                locationUpdated = true;
                            }
                        }
                    }
                }
                if (locationUpdated) {
                    UpdateRecentLocationData(locationMarker, locationMessagePrefix);
                }
            }
            return locationUpdated;
        }

        private static bool LocationMarkerHasMoved(string locationMarker) {
            bool locationMarkerHasValue = Helpers.StringHasValue(locationMarker);
            bool result = !locationMarkerHasValue;
            if (!result) {
                bool locationMarkerHasChanged = !locationMarker.Equals(_lastLocationMarker);
                result = locationMarkerHasChanged;
            }
            Helpers.WriteToExtraLog("Location markers: '" + locationMarker + "' & '" + _lastLocationMarker + "'", null);
            return result;
        }

        private void UpdateRecentLocationData(string locationMarker, string locationMessagePrefix) {
            _lastLocationMarker = locationMarker;
            _mostRecentLocation = _fireEagle.User().LocationHierarchy.MostRecent;
            if (_mostRecentLocation == null) {
                MessagesForm.AddMessage(DateTime.Now, Constants.UNKNOWN_LOCATION_MESSAGE, Constants.MessageType.Error);
            }
            else {
                _mostRecentLocation.Name = MainUtility.GetLocationMessage(locationMessagePrefix, _mostRecentLocation);
                MessagesForm.AddMessage(_mostRecentLocation.LocationDate, _mostRecentLocation.Name, Constants.MessageType.Info);
                LogExtraInformation(_mostRecentLocation);
            }
        }

        //private void UpdatePendingMapInfo(FireEagle.Location mostRecentLocation, int mapZoomLevel) {
        //    if (mostRecentLocation != null) {
        //        _pendingMapInfo = new MapInfo(mostRecentLocation.ExactPoint, mostRecentLocation.UpperCorner, mostRecentLocation.LowerCorner, mapZoomLevel);
        //        string message;
        //        if (_pendingMapInfo.LocationLatLong == null) {
        //            message = "Pending map for: LAT/LONG MISSING!";
        //            Helpers.WriteToExtraLog("point_raw: " + mostRecentLocation.point_raw + "; box_raw: " + mostRecentLocation.box_raw + "; ExactPoint: " + mostRecentLocation.ExactPoint + "; UpperCorner: " + mostRecentLocation.UpperCorner + "; LowerCorner: " + mostRecentLocation.LowerCorner, null);
        //        }
        //        else {
        //            message = string.Format("Pending map for: {0}, {1}", _pendingMapInfo.LocationLatLong.Latitude, _pendingMapInfo.LocationLatLong.Longitude);
        //        }
        //        Helpers.WriteToExtraLog(message, null);
        //        _pendingMapImage = null;
        //    }
        //}

        private void HandleFirstTick() {
            if (!_timerHasTicked) {
                VerifySettings();

                MenuItems.SetUpdateIntervalMenuItemCheckmarks(updateIntervalMenuItem, manuallyMenuItem);
                MenuItems.SetGpsStationaryThresholdMenuItemCheckmarks(gpsStationaryThresholdMenuItem);

                ApplyEventHandlers();

                FormName = Text;
                UpdateInitialSettings();
                _timerHasTicked = true;

                //GeolocateTest();
            }
        }

        private void DrawMapCenterMarker() {
            if (locationPictureBox.Image != null && Settings.MapCenterMarkerSize > 0 && _mostRecentLocation.ExactPoint != null && _mostRecentLocation.ExactPoint.Valid()) {
                Graphics graphics = Graphics.FromImage(locationPictureBox.Image);
                Bitmap myBitmap = new Bitmap(locationPictureBox.Image);
                Rectangle rectangle = new Rectangle(locationPictureBox.Left, locationPictureBox.Top, myBitmap.Width, myBitmap.Height);
                graphics.DrawImage(myBitmap, rectangle, rectangle, GraphicsUnit.Pixel);
                int halfMarkerSize = Settings.MapCenterMarkerSize / 2;
                int yAxisCenter = locationPictureBox.Image.Height / 2;
                int markerYAxisStart = yAxisCenter - halfMarkerSize;
                int xAxisCenter = locationPictureBox.Image.Width / 2;
                int markerXAxisStart = xAxisCenter - halfMarkerSize;
                for (int i = 0; i < Settings.MapCenterMarkerSize; i++) {
                    Color color = Color.Black;
                    myBitmap.SetPixel(markerXAxisStart + i, yAxisCenter, color);
                    myBitmap.SetPixel(xAxisCenter, markerYAxisStart + i, color);
                }
                graphics.DrawImage(myBitmap, rectangle, rectangle, GraphicsUnit.Pixel);
            }
        }

        private void GeolocateTest() {
            CellTower cellTower = new CellTower(88, 222, 20041, 8228);
            try {
                Locations locations = _fireEagle.Lookup(cellTower);
                Helpers.WriteToExtraLog("GeolocateTest Celltower Count: " + locations.Count, null);
                Helpers.WriteToExtraLog("GeolocateTest Celltower TotalLocations: " + locations.TotalLocations, null);
                Helpers.WriteToExtraLog("GeolocateTest Celltower 1Location point_raw: " + locations.LocationCollection[0].point_raw, null);
                Helpers.WriteToExtraLog("GeolocateTest Celltower 1Location name: " + locations.LocationCollection[0].Name, null);
            }
            catch (Exception e) {
                Helpers.WriteToExtraLog(e.Message, e);
            }
            Position position;
            try {
                position = GoogleMapsCellService.GetLocation(cellTower);
                Helpers.WriteToExtraLog("GeolocateTest Position: " + position.Latitude + ", " + position.Longitude, null);
            }
            catch (Exception e) {
                Helpers.WriteToExtraLog(e.Message, e);
            }
        }

        private void PerformActiveApplicationUserInterfaceUpdates() {
            if (!Helpers.StringHasValue(_fireEagle.UserToken.SecretToken)) {
                ShowPreAuthForm();
            }
            else if (_needToHidePreAuthorizationFormAndShowMainForm) {
                ShowMainFormAfterPreAuthorization();
            }

            if (locationPictureBox.Image != _pendingMapImage) {
                locationPictureBox.Image = _pendingMapImage;
                if (_pendingMapImage != null) {
                    DrawMapCenterMarker();
                    ResetUpdateTextBoxAfterMapImageUpdate();
                }
            }
            AdjustInfoMessageLabelBackgroundColor();
            UpdateMostRecentInfoMessageLabel();

            ChangeToManualUpdateIntervalIfUserIsTypingLocation();

            if (_userShouldBeAdvisedAboutRecommendedVersion) {
                ShowClientUpdateMenuItem();
                _userShouldBeAdvisedAboutRecommendedVersion = false;
            }
        }

        private void AdjustInfoMessageLabelBackgroundColor() {
            if (_attemptingUpdate && Constants.MOST_RECENT_INFO_LABEL_BACKCOLOR_NORMAL.Equals(mostRecentInfoMessageLabel.BackColor)) {
                mostRecentInfoMessageLabel.BackColor = Constants.MOST_RECENT_INFO_LABEL_BACKCOLOR_UPDATE_IN_PROGRESS;
            }
            else if (!_attemptingUpdate && Constants.MOST_RECENT_INFO_LABEL_BACKCOLOR_UPDATE_IN_PROGRESS.Equals(mostRecentInfoMessageLabel.BackColor)) {
                mostRecentInfoMessageLabel.BackColor = Constants.MOST_RECENT_INFO_LABEL_BACKCOLOR_NORMAL;
            }
        }

        private void UpdatePendingMapUrl() {
            try {
                if (_mostRecentLocation == null) {
                    _pendingMapImage = null;
                }
                else {
                    MapInfo mapInfo = new MapInfo(_mostRecentLocation.ExactPoint, _mostRecentLocation.UpperCorner, _mostRecentLocation.LowerCorner);
                    _pendingMapUrl = mapInfo.GetMapUrl(_mapHeight, _mapWidth, Settings.MapZoomLevel);
                }
            }
            catch (Exception e) {
                Helpers.WriteToExtraLog(e.Message, e);
                _pendingMapImage = null;
            }
        }

        private void ShowClientUpdateMenuItem() {
            MenuItem clientUpdateMenuItem = new MenuItem();
            clientUpdateMenuItem.Text = Constants.CLIENT_UPDATE_AVAILABLE_MENU_ITEM_TEXT;
            clientUpdateMenuItem.Click += clientUpdateMenuItem_Click;
            mainMenu.MenuItems.Add(clientUpdateMenuItem);
        }

        static void clientUpdateMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show(Constants.CLIENT_UPDATE_AVAILABLE_MESSAGE);
        }

        private static bool UserShouldBeAdvisedAboutRecommendedVersion() {
            bool result = false;
            try {
                result = VersionManager.KnownRecommendedVersionDiffersFromCurrentVersion();
            }
            catch (Exception e) {
                Helpers.WriteToExtraLog(e.Message, e);
            }
            return result;
        }

        private void UpdateMostRecentInfoMessageLabel() {
            if (!mostRecentInfoMessageLabel.Text.Equals(MessagesForm.MostRecentInfoMessage)) {
                mostRecentInfoMessageLabel.Text = MessagesForm.MostRecentInfoMessage;
                MainUtility.ResizeLabel(mostRecentInfoMessageLabel, CreateGraphics(), locationPictureBox.Width);
            }
        }

        //private void UpdateUserInterfaceWithPendingMapImage() {
        //    if (_mapImageIsPending) {
        //        ResetUpdateTextBoxAfterMapImageUpdate();
        //        UpdatePictureBoxWithPendingMapImage();
        //        _pendingMapInfo = null;
        //    }
        //}

        private void ResetUpdateTextBoxAfterMapImageUpdate() {
            updateTextBox.Text = string.Empty;
            if (updateLinkLabel.Focused) {
                updateTextBox.Focus();
            }
        }

        private void ChangeToManualUpdateIntervalIfUserIsTypingLocation() {
            if (Helpers.StringHasValue(_userUpdateLocation) && !Constants.UPDATE_INTERVAL_MANUAL_ONLY.Equals(Settings.UpdateIntervalInMinutes)) {
                MainUtility.ChangeUpdateIntervalSetting(Constants.UPDATE_INTERVAL_MANUAL_ONLY, updateIntervalMenuItem, manuallyMenuItem);
            }
        }

        private void ShowMainFormAfterPreAuthorization() {
            //Helpers.WriteToExtraLog("Go: ShowMainFormAfterPreAuthorization", null);
            SetupMainFormObjects(true);
            Show();
            PreAuthForm.Dispose();
            VerifyForm.Dispose();
            _needToHidePreAuthorizationFormAndShowMainForm = false;
        }

        private void ShowPreAuthForm() {
            string message = FireEagleRequestAuthorizationUrl.Length == 0 ? Constants.LOADING_REQUEST_AUTHORIZATION_MESSAGE : string.Format(Constants.AUTHORIZATION_REQUEST_TEMPLATE, FireEagleRequestAuthorizationUrl);
            if (!message.Equals(PreAuthForm.Message)) {
                PreAuthForm.Message = message;
                PreAuthForm.Show();
                Hide();
            }
        }

        //private void UpdatePictureBoxWithPendingMapImage() {
        //    _mapImageIsPending = false;
        //    locationPictureBox.Image = _pendingMapImage;
        //}

        //private void UpdatePendingMapImage() {
        //    try {
        //        string imageUrl = GetMapImageUrl();
        //        if (Helpers.StringHasValue(imageUrl)) {
        //            _pendingMapImage = MapManager.GetMapImage(imageUrl);
        //            if (_pendingMapImage == null) {
        //                MessagesForm.AddMessage(DateTime.Now, Constants.GETTING_MAP_IMAGE_MESSAGE, Constants.MessageType.Error);
        //            }
        //            else {
        //                _mapImageIsPending = true;
        //            }
        //        }
        //        else {
        //            MessagesForm.AddMessage(DateTime.Now, Constants.IMAGE_URL_FETCH_FAILED_MESSAGE, Constants.MessageType.Error);
        //            _pendingMapImage = null;
        //        }
        //    }
        //    catch (Exception e) {
        //        //_pendingMapInfo = null;
        //        _pendingMapImage = null;
        //        Helpers.WriteToExtraLog(e.Message, e);
        //        MessagesForm.AddMessage(DateTime.Now, Constants.MAP_FETCH_FAILED_MESSAGE, Constants.MessageType.Error);
        //    }
        //}

        //private string GetMapImageUrl() {
        //    string result = null;
        //    if (_pendingMapInfo == null) {
        //        Helpers.WriteToExtraLog("GMIU: no map info...", null);
        //    }
        //    else {
        //        if (_pendingMapInfo.LocationLatLong == null) {
        //            Helpers.WriteToExtraLog("GMIU: no lat/long...", null);
        //        }
        //        else {
        //            result = String.Format(Constants.MAP_URL_TEMPLATE, _mapWidth, _mapHeight, _pendingMapInfo.LocationLatLong.Latitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo), _pendingMapInfo.LocationLatLong.Longitude.ToString(Constants.EnglishUnitedStatesNumberFormatInfo), _pendingMapInfo.MapZoomLevel);
        //        }
        //    }
        //    return result;
        //}

        private void FireEagleWorker() {
            while (true) {
                if (SecondsBeforeNextFireEagleProcessing == 0) {
                    ProcessFireEagle();
                    if (!versionCheckPerformed) {
                        versionCheckPerformed = true;
                        _userShouldBeAdvisedAboutRecommendedVersion = UserShouldBeAdvisedAboutRecommendedVersion();
                    }
                }
                if (_pendingMapUrl != null && !_pendingMapUrl.Equals(_lastMappedUrl)) {
                    if (Settings.MapEnabled && Boolean.IsActiveApplication) {
                        _attemptingUpdate = true;
                        _pendingMapImage = MapManager.GetMapImage(_pendingMapUrl);
                        _attemptingUpdate = false;
                        _lastMappedUrl = _pendingMapUrl;
                        _pendingMapUrl = null;
                    }
                    else {
                        _pendingMapImage = null;
                    }
                }
                Thread.Sleep(1000);
                if (SecondsBeforeNextFireEagleProcessing > 0) {
                    SecondsBeforeNextFireEagleProcessing--;
                }
            }
        }

        private void ProcessFireEagle() {
            if (!Helpers.StringHasValue(_fireEagle.UserToken.SecretToken)) {
                ProcessFireEaglePrerequisites();
            }
            else {
                if (NeedToShowIntroductionMessage) {
                    ShowIntroductionMessage();
                }
                ProcessFireEagleUpdate();
            }
        }

        private void ProcessFireEaglePrerequisites() {
            if (NeedFireEaglePrerequisiteRequestAuthorizationToken()) {
                PopulateRequestTokenUrl();
            }
        }

        private bool NeedFireEaglePrerequisiteRequestAuthorizationToken() {
            return _requestToken == null;
        }

        private void ProcessFireEagleUpdate() {
            bool successfulUpdate = false;
            bool unsuccessfulUpdateWasHandled = false;
            bool stationaryThresholdPreventedUpdate = false;
            try {
                try {
                    if (UpdateLocationData(ref stationaryThresholdPreventedUpdate)) {
                        MainUtility.SetTimerPerSuccessfulOrHandledUpdate();
                        successfulUpdate = true;
                        _manualUpdateRequested = false;
                    }
                }
                catch (Exception e) {
                    ExceptionManager.HandleExpectedErrors(e);
                    ResetRecentLocationData();
                    MainUtility.SetTimerPerSuccessfulOrHandledUpdate();
                    unsuccessfulUpdateWasHandled = true;
                }
                finally {
                    UpdatePendingMapUrl();
                }
            }
            catch (Exception e1) {
                string errorMessage = Constants.UNKNOWN_ERROR_MESSAGE;
                if (Settings.ExtraLogEnabled) {
                    errorMessage += " See Extra Log for details.";
                }
                else {
                    errorMessage += " See Error Log for details.";
                    MessagesForm.AddMessage(DateTime.Now, "The Extra Log (Menu>Log>Log Extra/Debug Information) may answer questions developers have about your error.", Constants.MessageType.Error);
                }
                MessagesForm.AddMessage(DateTime.Now, errorMessage, Constants.MessageType.Info);
                Helpers.WriteToExtraLog(e1.Message, e1);
            }
            finally {
                if (!(successfulUpdate || unsuccessfulUpdateWasHandled)) {
                    MainUtility.HandleFailedUpdate(stationaryThresholdPreventedUpdate);
                }
            }
        }

        private void ResetRecentLocationData() {
            _mostRecentLocation = null;
            _lastMappedUrl = string.Empty;
        }

        private static void LogExtraInformation(FireEagle.Location location) {
            Helpers.WriteToExtraLog("Using Cell Towers: " + LocationManager.UseTowers, null);
            Helpers.WriteToExtraLog("Using GPS: " + LocationManager.UseGps, null);
            Helpers.WriteToExtraLog("CellTowerInfo: " + LocationManager.CellTowerInfoString, null);
            RIL.OPERATORNAMES operatornames = LocationManager.GetCurrentOperator();
            if (operatornames != null) {
                Helpers.WriteToExtraLog("OperatorInfo CountryCode: " + operatornames.CountryCode, null);
                Helpers.WriteToExtraLog("OperatorInfo LongName: " + operatornames.LongName, null);
                Helpers.WriteToExtraLog("OperatorInfo NumName: " + operatornames.NumName, null);
                Helpers.WriteToExtraLog("OperatorInfo ShortName: " + operatornames.ShortName, null);
            }
            if (location != null) {
                Helpers.WriteToExtraLog("MostRecentLocation point_raw: " + location.point_raw, null);
                Helpers.WriteToExtraLog("MostRecentLocation box_raw: " + location.box_raw, null);
                Helpers.WriteToExtraLog("MostRecentLocation LevelName: " + location.LevelName, null);
                Helpers.WriteToExtraLog("MostRecentLocation locatedAt_raw: " + location.locatedAt_raw, null);
                Helpers.WriteToExtraLog("MostRecentLocation LocationDate: " + location.LocationDate, null);
                Helpers.WriteToExtraLog("MostRecentLocation Name: " + location.Name, null);
                Helpers.WriteToExtraLog("MostRecentLocation PlaceID: " + location.PlaceID, null);
                Helpers.WriteToExtraLog("MostRecentLocation WOEID: " + location.WOEID, null);
                LatLong upperCorner = location.UpperCorner;
                if (upperCorner != null) {
                    Helpers.WriteToExtraLog("MostRecentLocation UpperCorner Latitude: " + upperCorner.Latitude, null);
                    Helpers.WriteToExtraLog("MostRecentLocation UpperCorner Longitude: " + upperCorner.Longitude, null);
                }
                LatLong lowerCorner = location.LowerCorner;
                if (lowerCorner != null) {
                    Helpers.WriteToExtraLog("MostRecentLocation LowerCorner Latitude: " + lowerCorner.Latitude, null);
                    Helpers.WriteToExtraLog("MostRecentLocation LowerCorner Longitude: " + lowerCorner.Longitude, null);
                }
                LatLong exactPoint = location.ExactPoint;
                if (exactPoint != null) {
                    Helpers.WriteToExtraLog("MostRecentLocation ExactPoint Latitude: " + exactPoint.Latitude, null);
                    Helpers.WriteToExtraLog("MostRecentLocation ExactPoint Longitude: " + exactPoint.Longitude, null);
                }
            }
        }

        private void ResetFireEagleAuthorization() {
            QuitFireEagleWorkerThread();
            Settings.FireEagleUserToken = new Token();
            Settings.Update();
            _fireEagle.UserToken = Settings.FireEagleUserToken;
            SecondsBeforeNextFireEagleProcessing = 0;
            ResetFireEagleWorkerThread();
        }

        #region Menu click handlers

        private void oneMinuteMenuItem_Click(object sender, EventArgs e) {
            updateTextBox.Text = string.Empty;
            MainUtility.ChangeUpdateIntervalSetting(1, updateIntervalMenuItem, manuallyMenuItem);
        }

        private void fiveMinutesMenuItem_Click(object sender, EventArgs e) {
            updateTextBox.Text = string.Empty;
            MainUtility.ChangeUpdateIntervalSetting(5, updateIntervalMenuItem, manuallyMenuItem);
        }

        private void fifteenMinutesMenuItem_Click(object sender, EventArgs e) {
            updateTextBox.Text = string.Empty;
            MainUtility.ChangeUpdateIntervalSetting(15, updateIntervalMenuItem, manuallyMenuItem);
        }

        private void thirtyMinutesMenuItem_Click(object sender, EventArgs e) {
            updateTextBox.Text = string.Empty;
            MainUtility.ChangeUpdateIntervalSetting(30, updateIntervalMenuItem, manuallyMenuItem);
        }

        private void sixtyMinutesMenuItem_Click(object sender, EventArgs e) {
            updateTextBox.Text = string.Empty;
            MainUtility.ChangeUpdateIntervalSetting(60, updateIntervalMenuItem, manuallyMenuItem);
        }

        private void yahooAlwaysMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeTowerLocationsSetting(Constants.TOWER_LOCATIONS_YAHOO_ALWAYS, true, towerLocationsMenuItem, yahooAlwaysMenuItem, googleSometimesMenuItem, googleAlwaysMenuItem);
        }

        private void googleSometimesMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeTowerLocationsSetting(Constants.TOWER_LOCATIONS_YAHOO_THEN_GOOGLE, true, towerLocationsMenuItem, yahooAlwaysMenuItem, googleSometimesMenuItem, googleAlwaysMenuItem);
        }

        private void googleAlwaysMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeTowerLocationsSetting(Constants.TOWER_LOCATIONS_GOOGLE_ALWAYS, true, towerLocationsMenuItem, yahooAlwaysMenuItem, googleSometimesMenuItem, googleAlwaysMenuItem);
        }

        private void gpsOnlyMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeLocateViaSetting(Constants.LOCATE_VIA_GPS_ONLY, true, locateViaMenuItem, gpsOnlyMenuItem, towersSometimesMenuItem, towersOnlyMenuItem);
        }

        private void towersSometimesMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeLocateViaSetting(Constants.LOCATE_VIA_GPS_THEN_TOWERS, true, locateViaMenuItem, gpsOnlyMenuItem, towersSometimesMenuItem, towersOnlyMenuItem);
        }

        private void towersOnlyMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeLocateViaSetting(Constants.LOCATE_VIA_TOWERS_ONLY, true, locateViaMenuItem, gpsOnlyMenuItem, towersSometimesMenuItem, towersOnlyMenuItem);
        }

        private void exitMenuItem_Click(object sender, EventArgs e) {
            Quit();
        }

        private void manuallyMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeUpdateIntervalSetting(Constants.UPDATE_INTERVAL_MANUAL_ONLY, updateIntervalMenuItem, manuallyMenuItem);
        }

        private void UndoHineiniAuthorizationConfirmMenuItem_Click(object sender, EventArgs e) {
            ResetFireEagleAuthorization();
        }

        private void infoMenuItem_Click(object sender, EventArgs e) {
            ShowMessagesForm(Constants.MessageType.Info);
        }

        private void errorMenuItem_Click(object sender, EventArgs e) {
            ShowMessagesForm(Constants.MessageType.Error);
        }

        private void updateLinkLabel_Click(object sender, EventArgs e) {
            _manualUpdateRequested = true;
            SecondsBeforeNextFireEagleProcessing = 0;
        }

        private void UserManualMenuItem_Click(object sender, EventArgs e) {
            string userManualFilePath = Helpers.GetWorkingDirectoryFileName(Constants.USER_MANUAL_FILENAME);
            Process.Start(userManualFilePath, null);
            // TODO remove helpform from project?
            //_helpForm.Show();
            //Hide();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e) {
            string aboutFilePath = Helpers.GetWorkingDirectoryFileName(Constants.ABOUT_FILENAME);
            Process.Start(aboutFilePath, null);
            // TODO remove aboutForm from project?
            //_aboutForm.ResetAndShow();
            //Hide();
        }

        private void mobileWebsiteMenuItem_Click(object sender, EventArgs e) {
            Process.Start("http://m.fireeagle.yahoo.net", null);
        }

        private void noneMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeGpsStationaryThresholdSetting(Constants.GPS_STATIONARY_THRESHOLD_DISABLED, gpsStationaryThresholdMenuItem);
        }

        private void quarterMileMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeGpsStationaryThresholdSetting(Constants.GPS_STATIONARY_THRESHOLD_QUARTER_MILE, gpsStationaryThresholdMenuItem);
        }

        private void halfMileMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeGpsStationaryThresholdSetting(Constants.GPS_STATIONARY_THRESHOLD_HALF_MILE, gpsStationaryThresholdMenuItem);
        }

        private void oneMileMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeGpsStationaryThresholdSetting(Constants.GPS_STATIONARY_THRESHOLD_MILE, gpsStationaryThresholdMenuItem);
        }
        #endregion

        #region Keypress handler

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            #region Unused keys

            if ((e.KeyCode == Keys.F1)) {
                // Soft Key 1
                // Not handled when menu is present.
            }
            if ((e.KeyCode == Keys.F2)) {
                // Soft Key 2
                // Not handled when menu is present.
            }
            if ((e.KeyCode == Keys.Up)) {
                // Up
            }
            if ((e.KeyCode == Keys.Down)) {
                // Down
            }
            if ((e.KeyCode == Keys.Left)) {
                // Left
            }
            if ((e.KeyCode == Keys.Right)) {
                // Right
            }
            if ((e.KeyCode == Keys.Enter)) {
                // Enter
            }
            if ((e.KeyCode == Keys.D1)) {
                // 1
            }
            if ((e.KeyCode == Keys.D2)) {
                // 2
            }
            if ((e.KeyCode == Keys.D3)) {
                // 3
            }
            if ((e.KeyCode == Keys.D4)) {
                // 4
            }
            if ((e.KeyCode == Keys.D5)) {
                // 5
            }
            if ((e.KeyCode == Keys.D6)) {
                // 6
            }
            if ((e.KeyCode == Keys.D7)) {
                // 7
            }
            if ((e.KeyCode == Keys.D8)) {
                // 8
            }
            if ((e.KeyCode == Keys.D9)) {
                // 9
            }
            if ((e.KeyCode == Keys.F8)) {
                // *
            }
            if ((e.KeyCode == Keys.D0)) {
                // 0
            }
            if ((e.KeyCode == Keys.F9)) {
                // #
            }

            #endregion
        }


        #endregion

        private void locationPicturePanel_Resize(object sender, EventArgs e) {
            locationPictureBox.Dock = DockStyle.None;
            locationPictureBox.Size = new Size(locationPicturePanel.Size.Width - 2, locationPicturePanel.Size.Height - 2);
            locationPictureBox.Location = new Point(1, 1);
            mostRecentInfoMessageLabel.Size = new Size(0, mostRecentInfoMessageLabel.Size.Height);
            //mostRecentInfoMessageLabel.Location = new Point(1, locationPictureBox.Size.Height - mostRecentInfoMessageLabel.Size.Height + 1);
            mostRecentInfoMessageLabel.Location = new Point(1, 1);
            if (mostRecentInfoMessageLabel.Text.Length > 0) {
                MainUtility.ResizeLabel(mostRecentInfoMessageLabel, CreateGraphics(), locationPictureBox.Width);
            }
        }

        private void locationPictureBox_Resize(object sender, EventArgs e) {
            PictureBox pictureBox = (PictureBox)sender;
            _mapHeight = pictureBox.Height;
            _mapWidth = pictureBox.Width;
        }

        private void updateTextBox_TextChanged(object sender, EventArgs e) {
            _userUpdateLocation = updateTextBox.Text;
        }

        private void timer1_Tick(object sender, EventArgs e) {
            timer1.Enabled = false;
            try {
                HandleFirstTick();
                if (Boolean.IsActiveApplication) {
                    if (Boolean.BacklightAlwaysOn) {
                        MainUtility.ActivateBacklight();
                    }
                    PerformActiveApplicationUserInterfaceUpdates();
                }
                //if (locationPictureBox.Image != null && _pendingMapImage == null) {
                //    locationPictureBox.Image = null;
                //}
            }
            finally {
                timer1.Enabled = true;
            }
        }

        private void showTagMenuItem_Click(object sender, EventArgs e) {
            tagForm = new TagForm();
            tagForm.FormClosed += tagForm_FormClosed;
            tagForm.Show();
            Hide();
        }

        void tagForm_FormClosed(object sender, EventArgs e) {
            Show();
            tagForm.Close();
            tagForm.Dispose();
        }

        private void whatIsTagMenuItem_Click(object sender, EventArgs e) {
            string text = Constants.TAG_INFO_MESSAGE;
            string caption = Constants.TAG_INFO_CAPTION;
            DialogResult dialogResult = MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dialogResult.Equals(DialogResult.Yes)) {
                Process.Start("http://gettag.mobi", null);
            }
        }

        private void thirtyFeetMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeGpsStationaryThresholdSetting(Constants.GPS_STATIONARY_THRESHOLD_30_FEET, gpsStationaryThresholdMenuItem);
        }

        private void sixtyFeetMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeGpsStationaryThresholdSetting(Constants.GPS_STATIONARY_THRESHOLD_60_FEET, gpsStationaryThresholdMenuItem);
        }

        private void threeHundredFeetMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeGpsStationaryThresholdSetting(Constants.GPS_STATIONARY_THRESHOLD_300_FEET, gpsStationaryThresholdMenuItem);
        }

        private void systemManagedMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeBacklightSetting(Constants.BACKLIGHT_SYSTEM_MANAGED, true, backlightMenuItem, systemManagedMenuItem, alwaysOnMenuItem);
        }

        private void alwaysOnMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeBacklightSetting(Constants.BACKLIGHT_ALWAYS_ON, true, backlightMenuItem, systemManagedMenuItem, alwaysOnMenuItem);
        }

        private void mapEnabledMenuItem_Click(object sender, EventArgs e) {
            mapEnabledMenuItem.Checked = !mapEnabledMenuItem.Checked;
            MainUtility.ChangeMapEnabledSetting(mapEnabledMenuItem.Checked, true, mapEnabledMenuItem);
            if (!centerMarkerMenuItem.Checked) {
                _pendingMapImage = null;
            }
        }

        private void centerMarkerDisabledMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeMapCenterMarkerSizeSetting(Constants.CENTER_MARKER_SIZE_DISABLED, centerMarkerMenuItem);
        }

        private void centerMarkerSmallMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeMapCenterMarkerSizeSetting(Constants.CENTER_MARKER_SIZE_SMALL, centerMarkerMenuItem);
        }

        private void centerMarkerMediumMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeMapCenterMarkerSizeSetting(Constants.CENTER_MARKER_SIZE_MEDIUM, centerMarkerMenuItem);
        }

        private void centerMarkerLargeMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeMapCenterMarkerSizeSetting(Constants.CENTER_MARKER_SIZE_LARGE, centerMarkerMenuItem);
        }

        private void zoomLeastMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeMapZoomLevelSetting(Constants.MAP_ZOOM_LEVEL_LEAST, zoomLevelMenuItem);
        }

        private void zoomLessMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeMapZoomLevelSetting(Constants.MAP_ZOOM_LEVEL_LESS, zoomLevelMenuItem);
        }

        private void zoomMoreMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeMapZoomLevelSetting(Constants.MAP_ZOOM_LEVEL_MORE, zoomLevelMenuItem);
        }

        private void zoomMostMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeMapZoomLevelSetting(Constants.MAP_ZOOM_LEVEL_MOST, zoomLevelMenuItem);
        }

        private void extraLogMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeExtraLogEnabledSetting(!extraLogMenuItem.Checked, extraLogMenuItem);
        }

    }
}