using System;
using Hineini.FireEagle;
using Hineini.Location.GPS;
using Hineini.Location.Towers;
using Hineini.Utility;

// LocationAreaCode = 29011
// MobileNetworkCode = 26
// MobileCountryCode = 310
// TowerId = 1402

namespace Hineini.Location {
    public class LocationManager {
        const string cellTowerInfoTemplate = "{0} - {1} - {2} - {3}";
        private static readonly Gps _gps = new Gps();
        private bool _useGps;
        private bool _useTowers;

        public LocationManager() {
            UseGps = false;
            UseTowers = false;
        }

        public bool UseGps {
            get { return _useGps; }
            set {
                _useGps = value;
                if (_useGps) {
                    _gps.Open();
                }
                else {
                    _gps.Close();
                }
            }
        }

        public bool UseTowers {
            get { return _useTowers; }
            set {
                _useTowers = value;
                if (_useTowers) {
                    RIL.Initialize();
                }
                else {
                    RIL.DeInitialize();
                }
            }
        }

        public Position? GetBestGuessPosition() {
            Position? result = GetValidGpsLocation();
            if (result == null) {
                Position geoLocation = GoogleMapsCellService.GetLocation(CurrentCellTower);
                bool locationIsValid = !double.NaN.Equals(geoLocation.Latitude) && !double.NaN.Equals(geoLocation.Longitude);
                if (locationIsValid) {
                    result = geoLocation;
                }
            }
            return result;
        }

        //private static CellTower GetGoogleCellTower(RIL.CELLINFO cellinfo) {
        //    CellTower result = new CellTower();
        //    result.lac = Convert.ToInt32(cellinfo.LocationAreaCode);
        //    result.mcc = Convert.ToInt32(cellinfo.MobileCountryCode);
        //    result.mnc = Convert.ToInt32(cellinfo.MobileNetworkCode);
        //    result.cellid = Convert.ToInt32(cellinfo.CellID);
        //    return result;
        //}

        public Position? CurrentCellTowerPosition {
            get {
                Position? result = null;
                Position geoLocation = GoogleMapsCellService.GetLocation(CurrentCellTower);
                bool locationIsValid = !double.NaN.Equals(geoLocation.Latitude) && !double.NaN.Equals(geoLocation.Longitude);
                if (locationIsValid) {
                    result = geoLocation;
                }
                return result;
            }
        }

        ~LocationManager() {
            //if (_cellTowerLocationProvider != null) {
            //    _cellTowerLocationProvider.Stop();
            //}
            RIL.DeInitialize();
            if (_gps != null) {
                _gps.Close();
            }
        }

        public Position? GetValidGpsLocation() {
            Position? result = null;
            if (_gps != null) {
                GpsPosition gpsPosition = _gps.GetPosition();
                if (gpsPosition != null && gpsPosition.LatitudeValid && gpsPosition.LongitudeValid) {
                    result = new Position(gpsPosition.Latitude, gpsPosition.Longitude);
                }
            }
            return result;
        }

        public CellTower CurrentCellTower {
            get {
                //CellTower cellTower = CellTowerLocationProvider.GetCellTowerInfo();
                RIL.CELLINFO cellTowerInfo = RIL.GetCellTowerInfo();
                CellTower result = new CellTower();
                result.cellid = GetCellTowerIdInt(cellTowerInfo);
                result.lac = GetLocationAreaCodeInt(cellTowerInfo);
                try {
                    result.mcc = GetMobileCountryCodeInt(cellTowerInfo);
                    result.mnc = GetMobileNetworkCodeInt(cellTowerInfo);
                }
                catch (Exception e) {
                    RIL.OPERATORNAMES currentOperator = GetCurrentOperator();
                    result.mcc = GetMobileCountryCodeInt(currentOperator);
                    result.mnc = GetMobileNetworkCodeInt(currentOperator);
                }
                return result;
            }
        }

        private int GetMobileNetworkCodeInt(RIL.OPERATORNAMES currentOperator) {
            return Convert.ToInt32(currentOperator.NumName.Substring(3, 2));
        }

        private int GetMobileCountryCodeInt(RIL.OPERATORNAMES currentOperator) {
            return Convert.ToInt32(currentOperator.NumName.Substring(0, 3));
        }

        public RIL.OPERATORNAMES GetCurrentOperator() {
            return RIL.GetCurrentOperator(RIL.RIL_OPFORMAT.NUM);
        }

        private int GetMobileNetworkCodeInt(RIL.CELLINFO cellTowerInfo) {
            return Convert.ToInt32(cellTowerInfo.MobileNetworkCode);
        }

        private int GetMobileCountryCodeInt(RIL.CELLINFO cellTowerInfo) {
            return Convert.ToInt32(cellTowerInfo.MobileCountryCode);
        }

        private int GetLocationAreaCodeInt(RIL.CELLINFO cellTowerInfo) {
            return Convert.ToInt32(cellTowerInfo.LocationAreaCode);
        }

        private int GetCellTowerIdInt(RIL.CELLINFO cellTowerInfo) {
            return Convert.ToInt32(cellTowerInfo.CellID);
        }

        public string CellTowerInfoString {
            get {
                string result = string.Format(cellTowerInfoTemplate, CurrentCellTower.cellid, CurrentCellTower.lac, CurrentCellTower.mcc, CurrentCellTower.mnc);
                return result;
            }
        }

        public double DistanceInMiles(Position startLocation, Position endLocation) {
            double theta = startLocation.Longitude - endLocation.Longitude;
            double result = Math.Sin(deg2rad(startLocation.Latitude)) * Math.Sin(deg2rad(endLocation.Latitude)) + Math.Cos(deg2rad(startLocation.Latitude)) * Math.Cos(deg2rad(endLocation.Latitude)) * Math.Cos(deg2rad(theta));
            result = Math.Acos(result);
            result = rad2deg(result);
            result = result * 60 * 1.1515;
            return (result);
        }

        private static double deg2rad(double deg) {
            return (deg * Math.PI / 180.0);
        }

        private static double rad2deg(double rad) {
            return (rad / Math.PI * 180.0);
        }
    }
}