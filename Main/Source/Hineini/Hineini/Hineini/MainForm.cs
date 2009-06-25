using System;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Hineini.FireEagle;
using Hineini.Location;
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
        private MessagesForm _messagesForm;
        private string _userUpdateLocation;
        private int _mapHeight;
        private int _mapWidth;
        private Bitmap _pendingMapImage;
        private bool _mapImageIsPending;
        private MapInfo _pendingMapInfo;
        private string _lastLocationName;
        private bool _needToHidePreAuthorizationFormAndShowMainForm;
        private bool _timerHasTicked;
        private bool _manualUpdateRequested;
        private static string _lastLocationMarker;
        private static Position _lastUpdatedPosition;
        private bool versionCheckPerformed;
        private bool _userShouldBeAdvisedAboutRecommendedVersion;
        private TagForm tagForm;
        private readonly string errorLogFilePath = MainUtility.GetWorkingDirectoryFileName("errors.log");

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
            MessagesForm.HideForm += _messagesForm_HideForm;
        }

        private void InitializeUpdateControls() {
            MainUtility.ResizeLabel(updateLinkLabel, CreateGraphics());
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
            MessagesForm.AddMessage(DateTime.Now, Constants.LOCATION_NOT_YET_IDENTIFIED, Constants.MessageType.Info);
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

        private bool UpdateLocationData() {
            bool locationUpdated;
            try {
                if (Helpers.StringHasValue(_userUpdateLocation)) {
                    locationUpdated = UpdateLocationDataByUserInput(_userUpdateLocation);
                }
                else {
                    MessagesForm.AddMessage(DateTime.Now, "TLU: UpdateLocationDataByEnvironmentInput" + _userUpdateLocation, Constants.MessageType.Error);
                    locationUpdated = UpdateLocationDataByEnvironmentInput();
                }
            }
            catch (Exception e) {
                MessagesForm.AddMessage(DateTime.Now, "TLU: " + MainUtility.GetExceptionMessage(e), Constants.MessageType.Error);
                throw;
            }
            return locationUpdated;
        }

        private bool UpdateLocationDataByEnvironmentInput() {
            bool locationUpdated;
            Position? currentGpsPosition = GetCurrentGpsPosition();
            locationUpdated = UpdateLocationDataByCurrentGpsPosition(currentGpsPosition);
            if (!locationUpdated && LocationManager.UseTowers) {
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
            object locatedCellTower = GetYahooLocatedCellTower() ?? GetGoogleLocatedCellTower();
            if (locatedCellTower != null) {
                string locationMessagePrefix = locatedCellTower is Position ? Constants.LOCATION_DESCRIPTION_PREFIX_GOOGLE : null;
                locationUpdated = UpdateLocationData(locatedCellTower, locationMessagePrefix);
            }
            else {
                throw new Exception(Constants.UNABLE_TO_IDENTIFY_CELL_TOWERS_MESSAGE);
            }
            return locationUpdated;
        }

        private bool UpdateLocationDataByCurrentGpsPosition(Position? currentGpsPosition) {
            bool locationUpdated = false;
            bool gpsUpdateShouldProceed = GpsUpdateShouldProceed(currentGpsPosition);
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

        private object GetGoogleLocatedCellTower() {
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

        private bool GpsUpdateShouldProceed(Position? currentGpsPosition) {
            bool updateShouldProceed = false;
            if (currentGpsPosition.HasValue) {
                updateShouldProceed = _manualUpdateRequested;
                if (!updateShouldProceed) {
                    updateShouldProceed = DistanceInMilesExceedsGpsStationaryThreshold(currentGpsPosition);
                    if (!updateShouldProceed) {
                        MessagesForm.AddMessage(DateTime.Now, Constants.UPDATE_SKIPPED_GPS_THRESHOLD, Constants.MessageType.Error);
                    }
                }
            }
            return updateShouldProceed;
        }

        private static bool DistanceInMilesExceedsGpsStationaryThreshold(Position? currentGpsPosition) {
            double distanceInMiles = LocationManager.DistanceInMiles(currentGpsPosition.Value, _lastUpdatedPosition);
            bool result = distanceInMiles == Constants.DISTANCE_UNKNOWN || distanceInMiles >= Settings.GpsStationaryThresholdInMiles;
            return result;
        }

        //private Address? GetAddressFromUserUpdateLocation() {
        //    Address? result = null;
        //    Match postalCodeMatch = Regex.Match(_userUpdateLocation, Constants.REGEX_POSTAL_CODE);
        //    if (postalCodeMatch.Success) {
        //        string postalCode = postalCodeMatch.ToString();
        //        string streetAddress = RemovePostalCodeFromUserUpdateLocation(postalCode);
        //        result = BuildAddress(streetAddress, postalCode);
        //    }
        //    else {
        //        MessagesForm.AddMessage(DateTime.Now, Constants.USER_SUPPLIED_ADDRESS_MUST_CONTAIN_ZIPCODE, Constants.MessageType.Error);
        //    }
        //    return result;
        //}

        //private Address BuildAddress(string streetAddress, string postalCode) {
        //    Address address = new Address();
        //    address.StreetAddress = streetAddress;
        //    address.Postal = postalCode;
        //    return address;
        //}

        //private string RemovePostalCodeFromUserUpdateLocation(string postalCode) {
        //    string streetAddress = _userUpdateLocation.Replace(postalCode, string.Empty);
        //    char[] endingCharactersToIgnoreInStreetAddress = {',', ' '};
        //    streetAddress = streetAddress.TrimEnd(endingCharactersToIgnoreInStreetAddress);
        //    return streetAddress;
        //}

        private void TryLocationUpdateGoogle() {
            //bool locationUpdated = false;
            Position? currentCellTowerPosition = LocationManager.CurrentCellTowerPosition;
            if (currentCellTowerPosition.HasValue) {
                UpdateLocationData(currentCellTowerPosition.Value, Constants.LOCATION_DESCRIPTION_PREFIX_GOOGLE);
            }
            //return locationUpdated;
        }

        private void TryLocationUpdateYahoo() {
            //bool locationUpdated = false;
            CellTower currentCellTower = LocationManager.CurrentCellTower;
            Locations locations = _fireEagle.Lookup(currentCellTower);
            if (locations.LocationCollection.Length > 0) {
                UpdateLocationData(currentCellTower, null);
            }
            //return locationUpdated;
        }

        private bool UpdateLocationData(object locationObject, string locationMessagePrefix) {
            bool locationUpdated = false;
            if (locationObject != null) {
                string locationMarker = MainUtility.GetLocationMarker(locationObject);
                if (locationObject is string) {
                    _fireEagle.Update(LocationType.address, (string)locationObject);
                }
                else if (locationObject is CellTower && (_manualUpdateRequested || LocationMarkerHasMoved(locationMarker))) {
                    _fireEagle.Update((CellTower)locationObject);
                }
                else {
                    _fireEagle.Update((Position)locationObject);
                }
                locationUpdated = true;
                UpdateRecentLocationData(locationMarker, locationMessagePrefix);
            }
            return locationUpdated;
        }

        private static bool LocationMarkerHasMoved(string locationMarker) {
            return !Helpers.StringHasValue(locationMarker) || !locationMarker.Equals(_lastLocationMarker);
        }

        private void UpdateRecentLocationData(string locationMarker, string locationMessagePrefix) {
            _lastLocationMarker = locationMarker;
            FireEagle.Location mostRecentLocation = _fireEagle.User().LocationHierarchy.MostRecent;
            DateTime mostRecentUpdate = mostRecentLocation == null ? DateTime.Now : mostRecentLocation.LocationDate;
            MessagesForm.AddMessage(mostRecentUpdate, MainUtility.GetLocationMessage(locationMessagePrefix, mostRecentLocation), Constants.MessageType.Info);
            UpdatePendingMapInfo(mostRecentLocation, MainUtility.GetMapZoomLevel(mostRecentLocation));
        }

        private void UpdatePendingMapInfo(FireEagle.Location mostRecentLocation, int mapZoomLevel) {
            if (mostRecentLocation != null && !mostRecentLocation.Name.Equals(_lastLocationName)) {
                _pendingMapInfo = new MapInfo(mostRecentLocation.ExactPoint, mostRecentLocation.UpperCorner, mostRecentLocation.LowerCorner, mapZoomLevel);
                string message;
                if (_pendingMapInfo.LocationLatLong == null) {
                    message = "Pending map for: LAT/LONG MISSING!";
                }
                else {
                    message = string.Format("Pending map for: {0}, {1}", _pendingMapInfo.LocationLatLong.Latitude, _pendingMapInfo.LocationLatLong.Longitude);
                }
                MessagesForm.AddMessage(DateTime.Now, message, Constants.MessageType.Info);
                _pendingMapImage = null;
                _lastLocationName = mostRecentLocation.Name;
            }
        }

        private void HandleFirstTick() {
            if (!_timerHasTicked) {
                VerifySettings();

                MenuItems.SetUpdateIntervalMenuItemCheckmarks(updateIntervalMenuItem, manuallyMenuItem);
                MenuItems.SetGpsStationaryThresholdMenuItemCheckmarks(gpsStationaryThresholdMenuItem);

                ApplyEventHandlers();

                FormName = Text;
                UpdateInitialSettings();
                _timerHasTicked = true;
            }
        }

        private void PerformActiveApplicationUserInterfaceUpdates() {
            if (!Helpers.StringHasValue(_fireEagle.UserToken.SecretToken)) {
                ShowPreAuthForm();
            }
            else if (_needToHidePreAuthorizationFormAndShowMainForm) {
                ShowMainFormAfterPreAuthorization();
            }
            UpdateMostRecentInfoMessageLabel();
            ChangeToManualUpdateIntervalIfUserIsTypingLocation();
            UpdateUserInterfaceWithPendingMapImage();
            if (_userShouldBeAdvisedAboutRecommendedVersion) {
                ShowClientUpdateMenuItem();
                _userShouldBeAdvisedAboutRecommendedVersion = false;
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
                MessagesForm.AddMessage(DateTime.Now, "PVC: " + e.Message, Constants.MessageType.Error);
            }
            return result;
        }

        private void UpdateMostRecentInfoMessageLabel() {
            if (!mostRecentInfoMessageLabel.Text.Equals(MessagesForm.MostRecentInfoMessage)) {
                mostRecentInfoMessageLabel.Text = MessagesForm.MostRecentInfoMessage;
                MainUtility.ResizeLabel(mostRecentInfoMessageLabel, CreateGraphics());
            }
        }

        private void UpdateUserInterfaceWithPendingMapImage() {
            if (_mapImageIsPending) {
                ResetUpdateTextBoxAfterMapImageUpdate();
                UpdatePictureBoxWithPendingMapImage();
                _pendingMapInfo = null;
            }
        }

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
            SetupMainFormObjects(true);
            Show();
            PreAuthForm.Hide();
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

        private void UpdatePictureBoxWithPendingMapImage() {
            _mapImageIsPending = false;
            locationPictureBox.Image = _pendingMapImage;
        }

        private void UpdatePendingMapImage() {
            try {
                string imageUrl = GetMapImageUrl();
                if (Helpers.StringHasValue(imageUrl)) {
                    MessagesForm.AddMessage(DateTime.Now, Constants.RETRIEVED_MAP_URL_MESSAGE, Constants.MessageType.Error);
                    _pendingMapImage = MapManager.GetMapImage(imageUrl);
                    if (_pendingMapImage == null) {
                        MessagesForm.AddMessage(DateTime.Now, Constants.GETTING_MAP_IMAGE_MESSAGE, Constants.MessageType.Error);
                    }
                    else {
                        _mapImageIsPending = true;
                    }
                }
                else {
                    MessagesForm.AddMessage(DateTime.Now, Constants.IMAGE_URL_FETCH_FAILED_MESSAGE, Constants.MessageType.Error);
                    _pendingMapImage = null;
                }
            }
            catch (Exception e) {
                _pendingMapInfo = null;
                _pendingMapImage = null;
                MessagesForm.AddMessage(DateTime.Now, "UPMI: " + e.Message, Constants.MessageType.Error);
                MessagesForm.AddMessage(DateTime.Now, Constants.MAP_FETCH_FAILED_MESSAGE, Constants.MessageType.Error);
            }
        }

        private string GetMapImageUrl() {
            string result = null;
            if (_pendingMapInfo == null) {
                MessagesForm.AddMessage(DateTime.Now, "GMIU: no map info...", Constants.MessageType.Error);
            }
            else {
                if (_pendingMapInfo.LocationLatLong == null) {
                    MessagesForm.AddMessage(DateTime.Now, "GMIU: no lat/long...", Constants.MessageType.Error);
                }
                else {
                    result = String.Format(Constants.MAP_URL_TEMPLATE, _mapWidth, _mapHeight, _pendingMapInfo.LocationLatLong.Latitude, _pendingMapInfo.LocationLatLong.Longitude, _pendingMapInfo.MapZoomLevel);
                }
            }
            return result;
        }

        private void FireEagleWorker() {
            while (true) {
                if (SecondsBeforeNextFireEagleProcessing == 0) {
                    ProcessFireEagle();
                    if (!versionCheckPerformed) {
                        versionCheckPerformed = true;
                        _userShouldBeAdvisedAboutRecommendedVersion = UserShouldBeAdvisedAboutRecommendedVersion();
                    }
                }
                if (Boolean.IsActiveApplication) {
                    if (_pendingMapInfo != null && _pendingMapImage == null) {
                        UpdatePendingMapImage();
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
            else {
                try {
                    _fireEagle.UserToken = _fireEagle.OAuthGetToken(_requestToken);
                    Settings.FireEagleUserToken = _fireEagle.UserToken;
                    Settings.Update();
                    FireEagleRequestAuthorizationUrl = string.Empty;
                    _needToHidePreAuthorizationFormAndShowMainForm = true;
                }
                catch (Exception) {
                    MessagesForm.AddMessage(DateTime.Now, Constants.GETTING_AUTH_TOKEN_MESSAGE, Constants.MessageType.Error);
                }
            }
            SecondsBeforeNextFireEagleProcessing = 0;
        }

        private bool NeedFireEaglePrerequisiteRequestAuthorizationToken() {
            return _requestToken == null;
        }

        private void ProcessFireEagleUpdate() {
            bool successfulUpdate = false;
            bool unsuccessfulUpdateWasHandled = false;
            try {
                try {
                    if (UpdateLocationData()) {
                        MainUtility.SetTimerPerSuccessfulOrHandledUpdate();
                        successfulUpdate = true;
                        _manualUpdateRequested = false;
                    }
                }
                catch (Exception e) {
                    ExceptionManager.HandleExpectedErrors(e, ref _pendingMapImage);
                    MainUtility.SetTimerPerSuccessfulOrHandledUpdate();
                    unsuccessfulUpdateWasHandled = true;
                }
            }
            catch (Exception e1) {
                string errorDescriptor = "PFEU: " + MainUtility.GetExceptionMessage(e1);
                MessagesForm.AddMessage(DateTime.Now, errorDescriptor, Constants.MessageType.Error);
                Helpers.WriteToFile(DateTime.Now.ToShortTimeString() + ": " + errorDescriptor, e1, errorLogFilePath, true);
            }
            finally {
                if (!(successfulUpdate || unsuccessfulUpdateWasHandled)) {
                    MainUtility.HandleFailedUpdate();
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

        private void systemManagedMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeBacklightSetting(Constants.BACKLIGHT_SYSTEM_MANAGED, true, backlightMenuItem, systemManagedMenuItem, alwaysOnMenuItem);
        }

        private void alwaysOnMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeBacklightSetting(Constants.BACKLIGHT_ALWAYS_ON, true, backlightMenuItem, systemManagedMenuItem, alwaysOnMenuItem);
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
            MessagesForm.AddMessage(DateTime.Now, Constants.MANUAL_UPDATE_STARTED_MESSAGE, Constants.MessageType.Info);
            _manualUpdateRequested = true;
            SecondsBeforeNextFireEagleProcessing = 0;
        }

        private void UserManualMenuItem_Click(object sender, EventArgs e) {
            string userManualFilePath = MainUtility.GetWorkingDirectoryFileName(Constants.USER_MANUAL_FILENAME);
            Process.Start(userManualFilePath, null);
            // TODO remove helpform from project?
            //_helpForm.Show();
            //Hide();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e) {
            string aboutFilePath = MainUtility.GetWorkingDirectoryFileName(Constants.ABOUT_FILENAME);
            Process.Start(aboutFilePath, null);
            // TODO remove aboutForm from project?
            //_aboutForm.ResetAndShow();
            //Hide();
        }

        private void mobileWebsiteMenuItem_Click(object sender, EventArgs e) {
            Process.Start("http://m.fireeagle.com", null);
        }

        private void noneMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeGpsStationaryThresholdSetting(0.0, gpsStationaryThresholdMenuItem);
        }

        private void quarterMileMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeGpsStationaryThresholdSetting(0.25, gpsStationaryThresholdMenuItem);
        }

        private void halfMileMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeGpsStationaryThresholdSetting(0.5, gpsStationaryThresholdMenuItem);
        }

        private void oneMileMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeGpsStationaryThresholdSetting(1.0, gpsStationaryThresholdMenuItem);
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
                MainUtility.ResizeLabel(mostRecentInfoMessageLabel, CreateGraphics());
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
                if (locationPictureBox.Image != null && _pendingMapImage == null) {
                    locationPictureBox.Image = null;
                }
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
    }
}