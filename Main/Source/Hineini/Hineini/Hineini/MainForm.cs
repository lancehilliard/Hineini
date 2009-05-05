using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Hineini.FireEagle;
using Hineini.Location;
using Hineini.Maps;
using Hineini.Utility;
using Microsoft.WindowsMobile.Status;

// TODO icons panel can be placed dynamically... if landscape, place on right side of display; if portrait, place on bottom of display

namespace Hineini {
    public partial class MainForm : Form {
        #region Fields
        private static string _formName;
        private static bool _needToShowWelcomeMessage = true;
        private static bool _messageWaitingToBeShown;
        private static int _secondsBeforeNextFireEagleProcessing;
        private static readonly LocationManager _locationManager = new LocationManager();
        private Thread _processFireEagleWorkerThread;
        private Token _requestToken;
        private string _fireEagleRequestAuthorizationUrl = string.Empty;
        private readonly FireEagle.FireEagle _fireEagle = new FireEagle.FireEagle(Constants.HINEINI_CONSUMER_KEY, Constants.HINEINI_CONSUMER_SECRET);
        private readonly PreAuthForm _preAuthForm = new PreAuthForm();
        private readonly MessagesForm _messagesForm = new MessagesForm();
        private string _userUpdateLocation;
        private int _mapHeight;
        private int _mapWidth;
        private Bitmap _pendingMapImage;
        private bool _mapImageIsPending;
        private bool _wasActiveApplicationAtLastTick;
        private MapInfo _pendingMapInfo;
        private string _lastLocationName;
        private static string _pictureBoxMessage;
        private readonly HelpForm _helpForm = new HelpForm();
        private readonly AboutForm _aboutForm = new AboutForm();
        private bool _needToHidePreAuthorizationFormAndShowMainForm;
        private bool _timerHasTicked;
        private bool _manualUpdateRequested;
        private static string _lastLocationMarker;
        private static Position _lastUpdatedPosition;
        #endregion

        #region Properties

        public static LocationManager LocationManager {
            get { return _locationManager; }
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

        public static bool NeedToShowWelcomeMessage {
            get { return _needToShowWelcomeMessage; }
            set { _needToShowWelcomeMessage = value; }
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
            VerifySettings();

            MenuItems.SetUpdateIntervalMenuItemCheckmarks(updateIntervalMenuItem, manuallyMenuItem);
            MenuItems.SetGpsStationaryThresholdMenuItemCheckmarks(gpsStationaryThresholdMenuItem);

            ApplyEventHandlers();

            bool authorizedForFireEagle = _fireEagle.UserToken != null && Helpers.StringHasValue(_fireEagle.UserToken.SecretToken);
            SetupMainFormObjects(authorizedForFireEagle);

            ResetFireEagleWorkerThread();
        }

        private void ApplyEventHandlers() {
            _preAuthForm.Exit += _preAuthForm_Exit;
            _messagesForm.HideForm += _messagesForm_HideForm;
            _helpForm.HideForm += _helpForm_HideForm;
            _aboutForm.HideForm += _aboutForm_HideForm;
        }

        void _aboutForm_HideForm(object sender, EventArgs e) {
            Show();
            _aboutForm.Hide();
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

        void _helpForm_HideForm(object sender, EventArgs e) {
            Show();
            _helpForm.Hide();
        }

        private void _messagesForm_HideForm(object sender, EventArgs e) {
            Show();
            _messagesForm.Hide();
        }

        void _preAuthForm_Exit(object sender, EventArgs e) {
            Quit();
        }

        private void ResetFireEagleWorkerThread() {
            _processFireEagleWorkerThread = new Thread(FireEagleWorker);
            _processFireEagleWorkerThread.Start();
        }

        private static void ShowWelcomeMessage() {
            DateTime now = DateTime.Now;
            MessagesForm.AddMessage(now, Messages.UpdateIntervalMessage, Constants.MessageType.Info);
            MessagesForm.AddMessage(now, Descriptions.TowerProviders, Constants.MessageType.Info);
            MessagesForm.AddMessage(now, Messages.LocateViaMessage, Constants.MessageType.Info);
            MessagesForm.AddMessage(now, Messages.BacklightMessage, Constants.MessageType.Info);
            MessagesForm.AddMessage(DateTime.Now, Constants.VERSIONED_WELCOME_MESSAGE, Constants.MessageType.Info);
            NeedToShowWelcomeMessage = false;
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
            _messagesForm.ShowMessages(messageType);
            Hide();
        }

        public static void ResetLastPositionMarkers() {
            _lastLocationMarker = string.Empty;
            _lastUpdatedPosition = new Position();
        }

        private bool TryLocationUpdate() {
            bool locationUpdated = false;
            try {
                if (Helpers.StringHasValue(_userUpdateLocation)) {
                    locationUpdated = UpdateAddressLocationData();
                }
                else {
                    Position? currentGpsPosition = _locationManager.UseGps ? _locationManager.GetValidGpsLocation() : null;
                    if (currentGpsPosition != null) {
                        UpdatePositionLocationData(currentGpsPosition);
                        locationUpdated = true;
                    }
                    else if (_locationManager.UseTowers) {
                        if (_locationManager.CanLocateTowers) {
                            locationUpdated = UpdateCellTowerLocationData();
                        }
                        else {
                            throw new Exception(Constants.UNABLE_TO_IDENTIFY_CELL_TOWERS_MESSAGE);
                        }
                    }
                }
            }
            catch (Exception e) {
                MessagesForm.AddMessage(DateTime.Now, "TLU: " + MainUtility.GetExceptionMessage(e), Constants.MessageType.Error);
                throw;
            }
            return locationUpdated;
        }

        private bool UpdateCellTowerLocationData() {
            bool locationUpdated = false;
            string towerLocationProvidersList = Settings.TowerLocationProvidersList;
            if (!Constants.TOWER_LOCATIONS_GOOGLE_ALWAYS.Equals(towerLocationProvidersList)) {
                locationUpdated = TryLocationUpdateYahoo();
            }
            if (!locationUpdated && !Constants.TOWER_LOCATIONS_YAHOO_ALWAYS.Equals(towerLocationProvidersList)) {
                locationUpdated = TryLocationUpdateGoogle();
            }
            return locationUpdated;
        }

        private void UpdatePositionLocationData(Position? currentGpsPosition) {
            bool updateShouldProceed = _manualUpdateRequested;
            if (!updateShouldProceed) {
                double distanceInMiles = _locationManager.DistanceInMiles(currentGpsPosition.Value, _lastUpdatedPosition);
                updateShouldProceed = distanceInMiles == Constants.DISTANCE_UNKNOWN || distanceInMiles >= Settings.GpsStationaryThresholdInMiles;
            }
            if (updateShouldProceed) {
                UpdateLocationData(currentGpsPosition.Value, Constants.LOCATION_DESCRIPTION_PREFIX_GPS);
                _lastUpdatedPosition = currentGpsPosition.Value;
            }
            else {
                MessagesForm.AddMessage(DateTime.Now, "No update (GPS Threshold)", Constants.MessageType.Error);
            }
        }

        private bool UpdateAddressLocationData() {
            bool result = false;
            Match postalCodeMatch = Regex.Match(_userUpdateLocation, Constants.REGEX_POSTAL_CODE);
            if (postalCodeMatch.Success) {
                string postalCode = postalCodeMatch.ToString();
                string streetAddress = _userUpdateLocation.Replace(postalCode, string.Empty);
                char[] endingCharactersToIgnoreInStreetAddress = {',', ' '};
                streetAddress = streetAddress.TrimEnd(endingCharactersToIgnoreInStreetAddress);
                Address address = new Address();
                address.Postal = postalCode;
                address.StreetAddress = streetAddress;
                UpdateLocationData(address, Constants.LOCATION_DESCRIPTION_PREFIX_USERSUPPLIED);
                _lastUpdatedPosition = new Position();
                result = true;
            }
            else {
                MessagesForm.AddMessage(DateTime.Now, Constants.USER_SUPPLIED_ADDRESS_MUST_CONTAIN_ZIPCODE, Constants.MessageType.Error);
            }
            return result;
        }

        private bool TryLocationUpdateGoogle() {
            bool locationUpdated = false;
            Position? currentCellTowerPosition = _locationManager.CurrentCellTowerPosition;
            if (currentCellTowerPosition.HasValue) {
                UpdateLocationData(currentCellTowerPosition.Value, Constants.LOCATION_DESCRIPTION_PREFIX_GOOGLE);
                locationUpdated = true;
            }
            return locationUpdated;
        }

        private bool TryLocationUpdateYahoo() {
            bool locationUpdated = false;
            CellTower currentCellTower = _locationManager.CurrentCellTower;
            Locations locations = _fireEagle.Lookup(currentCellTower);
            if (locations.LocationCollection.Length > 0) {
                UpdateLocationData(currentCellTower, null);
                locationUpdated = true;
            }
            return locationUpdated;
        }

        private void UpdateLocationData(object locationObject, string locationMessagePrefix) {
            if (locationObject != null) {
                string locationMarker = MainUtility.GetLocationMarker(locationObject);
                if (locationObject is Address) {
                    _fireEagle.Update((Address) locationObject);
                }
                else if (locationObject is CellTower && (_manualUpdateRequested || LocationMarkerHasMoved(locationMarker))) {
                    _fireEagle.Update((CellTower)locationObject);
                }
                else {
                    _fireEagle.Update((Position)locationObject);
                }
                UpdateRecentLocationData(locationMarker, locationMessagePrefix);
            }
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
                _pendingMapInfo = new MapInfo(mostRecentLocation.Name, mostRecentLocation.ExactPoint, mapZoomLevel);
                _pendingMapImage = null;
                _lastLocationName = mostRecentLocation.Name;
            }
        }

        private void HandleFirstTick() {
            if (!_timerHasTicked) {
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
            UpdatePictureBoxMessageLabel();
            UpdateMostRecentInfoMessageLabel();
            ChangeToManualUpdateIntervalIfUserIsTypingLocation();
            UpdateUserInterfaceWithPendingMapImage();
        }

        private void UpdateMostRecentInfoMessageLabel() {
            if (!mostRecentInfoMessageLabel.Text.Equals(MessagesForm.MostRecentInfoMessage)) {
                mostRecentInfoMessageLabel.Text = MessagesForm.MostRecentInfoMessage;
                MainUtility.ResizeLabel(mostRecentInfoMessageLabel, CreateGraphics());
            }
        }

        private void UpdateUserInterfaceWithPendingMapImage() {
            if (_mapImageIsPending) {
                _pictureBoxMessage = _pendingMapInfo.LocationName;
                ResetUpdateTextBoxAfterMapImageUpdate();
                UpdatePictureBoxWithPendingMapImage();
                _pendingMapInfo = null;
            }
            pictureBoxMessageLabel.Visible = Helpers.StringHasValue(_pictureBoxMessage);
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

        private void UpdatePictureBoxMessageLabel() {
            string candidateText = " " + _pictureBoxMessage;
            if (!pictureBoxMessageLabel.Text.Equals(candidateText)) {
                pictureBoxMessageLabel.Text = candidateText;
                MainUtility.ResizeLabel(pictureBoxMessageLabel, CreateGraphics());
                pictureBoxMessageLabel.Left = locationPictureBox.Width - pictureBoxMessageLabel.Width + 1;
            }
        }

        private void ShowMainFormAfterPreAuthorization() {
            SetupMainFormObjects(true);
            Show();
            _preAuthForm.Hide();
            _needToHidePreAuthorizationFormAndShowMainForm = false;
        }

        private void ShowPreAuthForm() {
            string message = FireEagleRequestAuthorizationUrl.Length == 0 ? Constants.LOADING_REQUEST_AUTHORIZATION_MESSAGE : string.Format(Constants.AUTHORIZATION_REQUEST_TEMPLATE, FireEagleRequestAuthorizationUrl);
            if (!message.Equals(_preAuthForm.Message)) {
                _preAuthForm.Message = message;
                _preAuthForm.Show();
                Hide();
            }
        }

        private void UpdatePictureBoxWithPendingMapImage() {
            _mapImageIsPending = false;
            locationPictureBox.Image = _pendingMapImage;
        }

        private void UpdatePendingMapImage() {
            string mapServiceUrl = string.Empty;
            try {
                mapServiceUrl = MapManager.GetMapServiceUrl(_pendingMapInfo, _mapWidth, _mapHeight);
                string imageUrl = null;
                if (Helpers.StringHasValue(mapServiceUrl)) {
                        _pictureBoxMessage = Constants.FETCHING_MAP_MESSAGE;
                        XmlTextReader xmlTextReader = new XmlTextReader(mapServiceUrl); // http://msdn.microsoft.com/en-us/library/aa446526.aspx#mgexmlnetcpctfrmwrk_topic2
                        xmlTextReader.WhitespaceHandling = WhitespaceHandling.Significant;
                        imageUrl = MapManager.GetMapImageUrl(xmlTextReader);
                        xmlTextReader.Close();
                }
                if (Helpers.StringHasValue(imageUrl)) {
                    _pictureBoxMessage = Constants.LOADING_MAP_MESSAGE;
                    Bitmap mapImage = MapManager.GetMapImage(imageUrl);
                    if (mapImage == null) {
                        MessagesForm.AddMessage(DateTime.Now, Constants.GETTING_MAP_IMAGE_MESSAGE, Constants.MessageType.Error);
                    }
                    else {
                        _pendingMapImage = mapImage;
                        _mapImageIsPending = true;
                    }
                }
            }
            catch (Exception e) {
                _pictureBoxMessage = Constants.MAP_FETCH_FAILED_MESSAGE;
                _pendingMapInfo = null;
                MessagesForm.AddMessage(DateTime.Now, "UPMI: " + e.Message, Constants.MessageType.Error);
            }
        }

        private void FireEagleWorker() {
            while (true) {
                if (SecondsBeforeNextFireEagleProcessing == 0) {
                    ProcessFireEagle();
                }
                if (Boolean.IsActiveApplication && _pendingMapInfo != null && _pendingMapImage == null) {
                    UpdatePendingMapImage();
                }
                if (Boolean.BacklightAlwaysOn) {
                    MainUtility.ActivateBacklight();
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
                if (NeedToShowWelcomeMessage) {
                    ShowWelcomeMessage();
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
                    if (TryLocationUpdate()) {
                        MainUtility.SetTimerPerSuccessfulOrHandledUpdate();
                        successfulUpdate = true;
                        _manualUpdateRequested = false;
                    }
                }
                catch (Exception e) {
                    ExceptionManager.HandleExpectedErrors(e);
                    MainUtility.SetTimerPerSuccessfulOrHandledUpdate();
                    unsuccessfulUpdateWasHandled = true;
                }
            }
            catch (Exception e1) {
                MessagesForm.AddMessage(DateTime.Now, "PFEU: " + MainUtility.GetExceptionMessage(e1), Constants.MessageType.Error);
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
            MainUtility.ChangeUpdateIntervalSetting(1, updateIntervalMenuItem, manuallyMenuItem);
        }

        private void fiveMinutesMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeUpdateIntervalSetting(5, updateIntervalMenuItem, manuallyMenuItem);
        }

        private void fifteenMinutesMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeUpdateIntervalSetting(15, updateIntervalMenuItem, manuallyMenuItem);
        }

        private void thirtyMinutesMenuItem_Click(object sender, EventArgs e) {
            MainUtility.ChangeUpdateIntervalSetting(30, updateIntervalMenuItem, manuallyMenuItem);
        }

        private void sixtyMinutesMenuItem_Click(object sender, EventArgs e) {
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
            _helpForm.Show();
            Hide();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e) {
            _aboutForm.ResetAndShow();
            Hide();
        }

        private void mobileWebsiteMenuItem_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("http://m.fireeagle.com", null);
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
            if ((e.KeyCode == Keys.C)) {
                string message = _locationManager.CellTowerInfoString;
                if (!Helpers.StringHasValue(message)) {
                    message = Constants.LOCATE_VIA_GPS_ONLY.Equals(Settings.LocateViaList) ? Constants.UNABLE_TO_IDENTIFY_CELL_TOWERS_WITH_CELL_TOWERS_DISABLED_DEBUG_MESSAGE : Constants.UNABLE_TO_IDENTIFY_CELL_TOWERS_DEBUG_MESSAGE;
                }
                MessagesForm.AddMessage(DateTime.Now, message, Constants.MessageType.Error);
            }
            if ((e.KeyCode == Keys.A)) {
                string message = SystemState.ActiveApplication;
                if (!Helpers.StringHasValue(message)) {
                    message = "Unable to determine Active Application...";
                }
                else {
                    message = "Active Application: " + message;
                }
                MessagesForm.AddMessage(DateTime.Now, message, Constants.MessageType.Error);
            }

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
            mostRecentInfoMessageLabel.Location = new Point(1, locationPictureBox.Size.Height - mostRecentInfoMessageLabel.Size.Height + 1);
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
                bool isActiveApplication = Boolean.IsActiveApplication;
                MainUtility.ReleaseBacklightIfNoLongerActiveApplication(_wasActiveApplicationAtLastTick, isActiveApplication);
                _wasActiveApplicationAtLastTick = isActiveApplication;
                if (isActiveApplication) {
                    PerformActiveApplicationUserInterfaceUpdates();
                }
            }
            finally {
                timer1.Enabled = true;
            }
        }

    }
}