/*
 * I tried to locate/contact the author of this code:
 * http://read.pudn.com/downloads98/sourcecode/embed/401078/RilTest/Ril.cs__.htm
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Hineini.Location.Towers {
    public class RIL {
        public static RIL_EVENTS EVENTS = new RIL_EVENTS();
        private const int MAXLENGTH_OPERATOR_LONG = 32;
        private const int MAXLENGTH_OPERATOR_SHORT = 16;
        private const int MAXLENGTH_OPERATOR_NUMERIC = 16;
        private const int MAXLENGTH_OPERATOR_COUNTRY_CODE = 8;
        private const int MAXLENGTH_BCCH = 48;
        private const int MAXLENGTH_NMR = 16;


        private const int RIL_NCLASS_MISC = 0x00400000;
        private const int RIL_NOTIFY_SIGNALQUALITY = (0x00000005 | RIL_NCLASS_MISC);
        private const string RIL_PARAM_NOT_AVAILABLE = "NA/REQ";
        private const string RIL_CMD_TIMEOUT = "Timeout";

        //OPERATOR PARAM 
        private const int RIL_PARAM_ON_LONGNAME = 0x00000001;
        private const int RIL_PARAM_ON_SHORTNAME = 0x00000002;
        private const int RIL_PARAM_ON_NUMNAME = 0x00000004;
        private const int RIL_PARAM_ON_COUNTRY_CODE = 0x00000008;
        private const int RIL_PARAM_ON_GSM_ACT = 0x00000010;
        private const int RIL_PARAM_ON_GSMCOMPACT_ACT = 0x00000020;
        private const int RIL_PARAM_ON_UMTS_ACT = 0x00000040;
        private const int RIL_PARAM_ON_ALL = 0x0000007f;


        //SIGNAL QUALITY PARAMS 
        private const int RIL_PARAM_SQ_SIGNALSTRENGTH = 0x00000001;
        private const int RIL_PARAM_SQ_MINSIGNALSTRENGTH = 0x00000002;
        private const int RIL_PARAM_SQ_MAXSIGNALSTRENGTH = 0x00000004;
        private const int RIL_PARAM_SQ_BITERRORRATE = 0x00000008;
        private const int RIL_PARAM_SQ_LOWSIGNALSTRENGTH = 0x00000010;
        private const int RIL_PARAM_SQ_HIGHSIGNALSTRENGTH = 0x00000020;
        private const int RIL_PARAM_SQ_ALL = 0x0000003f;



        //CELLINFO PARAM 
        private const int RIL_PARAM_CTI_MOBILECOUNTRYCODE = 0x00000001;
        private const int RIL_PARAM_CTI_MOBILENETWORKCODE = 0x00000002;
        private const int RIL_PARAM_CTI_LOCATIONAREACODE = 0x00000004;
        private const int RIL_PARAM_CTI_CELLID = 0x00000008;
        private const int RIL_PARAM_CTI_BASESTATIONID = 0x00000010;
        private const int RIL_PARAM_CTI_BROADCASTCONTROLCHANNEL = 0x00000020;
        private const int RIL_PARAM_CTI_RXLEVEL = 0x00000040;
        private const int RIL_PARAM_CTI_RXLEVELFULL = 0x00000080;
        private const int RIL_PARAM_CTI_RXLEVELSUB = 0x00000100;
        private const int RIL_PARAM_CTI_RXQUALITY = 0x00000200;
        private const int RIL_PARAM_CTI_RXQUALITYFULL = 0x00000400;
        private const int RIL_PARAM_CTI_RXQUALITYSUB = 0x00000800;
        private const int RIL_PARAM_CTI_IDLETIMESLOT = 0x00001000;
        private const int RIL_PARAM_CTI_TIMINGADVANCE = 0x00002000;
        private const int RIL_PARAM_CTI_GPRSCELLID = 0x00004000;
        private const int RIL_PARAM_CTI_GPRSBASESTATIONID = 0x00008000;
        private const int RIL_PARAM_CTI_NUMBCCH = 0x00010000;
        private const int RIL_PARAM_CTI_NMR = 0x00020000;
        private const int RIL_PARAM_CTI_BCCH = 0x00040000;
        private const int RIL_PARAM_CTI_ALL = 0x0007ffff;


        private delegate void RILRESULTCALLBACK(IntPtr dwCode, IntPtr hrCmdID, System.IntPtr lpData, IntPtr cbData, IntPtr dwParam);
        private delegate void RILNOTIFYCALLBACK(IntPtr dwCode, System.IntPtr lpData, IntPtr cbData, IntPtr dwParam);

        private static bool _vInitialized = false;
        private static RILOPERATORNAMES _vOperatore;
        private static RILCELLTOWERINFO _cellTowerInfo;
        private static RILSIGNALQUALITY _signalQuality;


        private static int _vTimeOut = 4;

        private static RILNOTIFYCALLBACK NotifyCallback;
        private static RILRESULTCALLBACK ResultCallback;
        private static System.IntPtr NotifyCallbackPointer;
        private static System.IntPtr ResultCallbackPointer;

        private static IntPtr hRil;
        private static RIL_CMD rilCmd = new RIL_CMD(0, RIL_CMD_TYPE.NONE);
        private static RIL_RESULT ril_result;

        public static bool isInitialized {
            get { return _vInitialized; }
        }

        #region Initialize RIL
        public static bool Initialize() {
            if (_vInitialized)
                return false;

            IntPtr res = IntPtr.Zero;
            IntPtr port = new IntPtr((int)1);
            hRil = IntPtr.Zero;

            NotifyCallback = new RIL.RILNOTIFYCALLBACK(RIL._NotifyCallback);
            NotifyCallbackPointer = Marshal.GetFunctionPointerForDelegate(NotifyCallback);

            ResultCallback = new RIL.RILRESULTCALLBACK(RIL._ResultCallback);
            ResultCallbackPointer = Marshal.GetFunctionPointerForDelegate(ResultCallback);

            IntPtr dwNotif = new IntPtr((int)0x00FF0000);
            //IntPtr dwParam = new IntPtr(0x33FF33FF); 

            IntPtr dwParam = new IntPtr(0x55AA55AA);

            res = RIL_Initialize(port, ResultCallbackPointer, NotifyCallbackPointer, dwNotif, dwParam, out hRil);

            if (res != IntPtr.Zero)
                return false;

            _vInitialized = true;
            return true;

        }
        #endregion

        #region DeInitialize RIL
        public static void DeInitialize() {
            if (!_vInitialized)
                return;

            IntPtr res = IntPtr.Zero;
            res = RIL_Deinitialize(hRil);
            if ((System.Int32)res >= 0) {
                _vInitialized = false;
                return;
            }
            return;
        }
        #endregion

        #region GetSignalQuality
        public static SIGNALQUALITY GetSignalQuality() {
            IntPtr res = IntPtr.Zero;
            res = RIL_GetSignalQuality(hRil);
            if ((System.Int32)res > 0) {
                rilCmd.CmdId = (System.Int32)res;
                rilCmd.CmdType = RIL_CMD_TYPE.SIGNALQUALITY;

                int mult = 0;
                while ((mult < (_vTimeOut * 1000)) && (rilCmd.CmdType != RIL_CMD_TYPE.NONE)) {
                    System.Threading.Thread.Sleep(100);
                    mult += 100;
                }
                if (rilCmd.CmdType != RIL_CMD_TYPE.NONE)
                    return new SIGNALQUALITY(RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT);

                if (ril_result != RIL_RESULT.OK)
                    return new SIGNALQUALITY(ril_result.ToString(), ril_result.ToString(), ril_result.ToString(), ril_result.ToString(), ril_result.ToString(), ril_result.ToString());
                else
                    return new SIGNALQUALITY(((_signalQuality.dwParams & RIL_PARAM_SQ_SIGNALSTRENGTH) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.nSignalStrength.ToString(),
                                             ((_signalQuality.dwParams & RIL_PARAM_SQ_MINSIGNALSTRENGTH) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.nMinSignalStrength.ToString(),
                                             ((_signalQuality.dwParams & RIL_PARAM_SQ_MAXSIGNALSTRENGTH) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.nMaxSignalStrength.ToString(),
                                             ((_signalQuality.dwParams & RIL_PARAM_SQ_BITERRORRATE) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.dwBitErrorRate.ToString(),
                                             ((_signalQuality.dwParams & RIL_PARAM_SQ_LOWSIGNALSTRENGTH) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.nLowSignalStrength.ToString(),
                                             ((_signalQuality.dwParams & RIL_PARAM_SQ_HIGHSIGNALSTRENGTH) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.nHighSignalStrength.ToString());
            }
            return null;
        }
        #endregion


        #region GetCellTowerInfo
        public static CELLINFO GetCellTowerInfo() {
            IntPtr res = IntPtr.Zero;
            res = RIL_GetCellTowerInfo(hRil);
            if ((System.Int32)res > 0) {
                rilCmd.CmdId = (System.Int32)res;
                rilCmd.CmdType = RIL_CMD_TYPE.CELLTOWERINFO;

                int mult = 0;
                while ((mult < (_vTimeOut * 1000)) && (rilCmd.CmdType != RIL_CMD_TYPE.NONE)) {
                    System.Threading.Thread.Sleep(100);
                    mult += 100;
                }
                if (rilCmd.CmdType != RIL_CMD_TYPE.NONE)
                    return new CELLINFO(RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT);

                if (ril_result != RIL_RESULT.OK)
                    return new CELLINFO(ril_result.ToString(), ril_result.ToString(), ril_result.ToString(), ril_result.ToString(), ril_result.ToString(), ril_result.ToString());
                else
                    return new CELLINFO(((_cellTowerInfo.dwParams & RIL_PARAM_CTI_MOBILECOUNTRYCODE) == 0) ? RIL_PARAM_NOT_AVAILABLE : _cellTowerInfo.dwMobileCountryCode.ToString(),
                                        ((_cellTowerInfo.dwParams & RIL_PARAM_CTI_MOBILENETWORKCODE) == 0) ? RIL_PARAM_NOT_AVAILABLE : _cellTowerInfo.dwMobileNetworkCode.ToString(),
                                        ((_cellTowerInfo.dwParams & RIL_PARAM_CTI_LOCATIONAREACODE) == 0) ? RIL_PARAM_NOT_AVAILABLE : _cellTowerInfo.dwLocationAreaCode.ToString(),
                                        ((_cellTowerInfo.dwParams & RIL_PARAM_CTI_CELLID) == 0) ? RIL_PARAM_NOT_AVAILABLE : _cellTowerInfo.dwCellID.ToString(),
                                        ((_cellTowerInfo.dwParams & RIL_PARAM_CTI_RXLEVEL) == 0) ? RIL_PARAM_NOT_AVAILABLE : _cellTowerInfo.dwRxLevel.ToString(),
                                        ((_cellTowerInfo.dwParams & RIL_PARAM_CTI_RXQUALITY) == 0) ? RIL_PARAM_NOT_AVAILABLE : _cellTowerInfo.dwRxQuality.ToString());
            }
            return null;
        }
        #endregion

        #region GetCurrentOperator
        public static OPERATORNAMES GetCurrentOperator(RIL_OPFORMAT format) {
            IntPtr res = IntPtr.Zero;
            res = RIL_GetCurrentOperator(hRil, new System.IntPtr((int)format));
            if ((System.Int32)res > 0) {
                rilCmd.CmdId = (System.Int32)res;
                rilCmd.CmdType = RIL_CMD_TYPE.OPERATOR;

                int mult = 0;
                while ((mult < (_vTimeOut * 1000)) && (rilCmd.CmdType != RIL_CMD_TYPE.NONE)) {
                    System.Threading.Thread.Sleep(100);
                    mult += 100;
                }
                if (rilCmd.CmdType != RIL_CMD_TYPE.NONE)
                    return new OPERATORNAMES(RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT, RIL_CMD_TIMEOUT);

                if (ril_result != RIL_RESULT.OK)
                    return new OPERATORNAMES(ril_result.ToString(), ril_result.ToString(), ril_result.ToString(), ril_result.ToString());
                else
                    return new OPERATORNAMES(((_vOperatore.dwParams & RIL_PARAM_ON_LONGNAME) == 0) ? RIL_PARAM_NOT_AVAILABLE : System.Text.Encoding.ASCII.GetString(_vOperatore.szLongName, 0, _vOperatore.szLongName.Length).Replace("\0", ""),
                                             ((_vOperatore.dwParams & RIL_PARAM_ON_SHORTNAME) == 0) ? RIL_PARAM_NOT_AVAILABLE : System.Text.Encoding.ASCII.GetString(_vOperatore.szShortName, 0, _vOperatore.szShortName.Length).Replace("\0", ""),
                                             ((_vOperatore.dwParams & RIL_PARAM_ON_NUMNAME) == 0) ? RIL_PARAM_NOT_AVAILABLE : System.Text.Encoding.ASCII.GetString(_vOperatore.szNumName, 0, _vOperatore.szNumName.Length).Replace("\0", ""),
                                             ((_vOperatore.dwParams & RIL_PARAM_ON_COUNTRY_CODE) == 0) ? RIL_PARAM_NOT_AVAILABLE : System.Text.Encoding.ASCII.GetString(_vOperatore.szCountryCode, 0, _vOperatore.szCountryCode.Length).Replace("\0", ""));
            }
            return null;
        }
        #endregion

        private static void _NotifyCallback(IntPtr dwCode, System.IntPtr lpData, IntPtr cbData, IntPtr dwParam) {
            switch ((Int32)dwCode) {
                case RIL_NOTIFY_SIGNALQUALITY:
                    RILSIGNALQUALITY _signalQuality = (RILSIGNALQUALITY)Marshal.PtrToStructure(lpData, typeof(RILSIGNALQUALITY));
                    EVENTS.RiseSignalQualityChanged(new SIGNALQUALITY(((_signalQuality.dwParams & RIL_PARAM_SQ_SIGNALSTRENGTH) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.nSignalStrength.ToString(),
                                                                      ((_signalQuality.dwParams & RIL_PARAM_SQ_MINSIGNALSTRENGTH) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.nMinSignalStrength.ToString(),
                                                                      ((_signalQuality.dwParams & RIL_PARAM_SQ_MAXSIGNALSTRENGTH) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.nMaxSignalStrength.ToString(),
                                                                      ((_signalQuality.dwParams & RIL_PARAM_SQ_BITERRORRATE) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.dwBitErrorRate.ToString(),
                                                                      ((_signalQuality.dwParams & RIL_PARAM_SQ_LOWSIGNALSTRENGTH) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.nLowSignalStrength.ToString(),
                                                                      ((_signalQuality.dwParams & RIL_PARAM_SQ_HIGHSIGNALSTRENGTH) == 0) ? RIL_PARAM_NOT_AVAILABLE : _signalQuality.nHighSignalStrength.ToString()));
                    break;

            }
        }

        private static void _ResultCallback(IntPtr dwCode, IntPtr hrCmdID, System.IntPtr lpData, IntPtr cbData, IntPtr dwParam) {
            if ((rilCmd.CmdType != RIL_CMD_TYPE.NONE) && (rilCmd.CmdId == (Int32)hrCmdID)) {
                if ((Int32)dwCode == (Int32)RIL_RESULT.OK) {
                    switch (rilCmd.CmdType) {
                        case RIL_CMD_TYPE.OPERATOR:
                            _vOperatore = (RILOPERATORNAMES)Marshal.PtrToStructure(lpData, typeof(RILOPERATORNAMES));
                            break;
                        case RIL_CMD_TYPE.CELLTOWERINFO:
                            _cellTowerInfo = (RILCELLTOWERINFO)Marshal.PtrToStructure(lpData, typeof(RILCELLTOWERINFO));
                            break;
                        case RIL_CMD_TYPE.SIGNALQUALITY:
                            _signalQuality = (RILSIGNALQUALITY)Marshal.PtrToStructure(lpData, typeof(RILSIGNALQUALITY));
                            break;
                    }
                }
                ril_result = (RIL_RESULT)((Int32)dwCode);
                rilCmd.CmdType = RIL_CMD_TYPE.NONE;
            }
        }

        #region Invoke/RIL
        [DllImport("ril.dll")]
        private static extern IntPtr RIL_Initialize(IntPtr dwIndex, System.IntPtr pfnResult, System.IntPtr pfnNotify, IntPtr dwNotificationClasses, IntPtr dwParam, out IntPtr lphRil);

        [DllImport("ril.dll")]
        private static extern IntPtr RIL_Deinitialize(IntPtr lphRil);

        [DllImport("ril.dll")]
        private static extern IntPtr RIL_GetCurrentOperator(IntPtr hRil, IntPtr dwFormat);

        [DllImport("ril.dll")]
        private static extern IntPtr RIL_GetCellTowerInfo(IntPtr hRil);

        [DllImport("ril.dll")]
        private static extern IntPtr RIL_GetSignalQuality(IntPtr hRil);
        #endregion

        private struct RIL_CMD {
            Int32 _cmdId;
            RIL_CMD_TYPE _cmdType;
            public Int32 CmdId {
                get { return _cmdId; }
                set { _cmdId = value; }
            }
            public RIL_CMD_TYPE CmdType {
                get { return _cmdType; }
                set { _cmdType = value; }
            }
            public RIL_CMD(Int32 cmdId, RIL_CMD_TYPE cmdType) {
                _cmdId = cmdId;
                _cmdType = cmdType;
            }
        }

        private enum RIL_CMD_TYPE {
            NONE = 0,
            OPERATOR = 1,
            CELLTOWERINFO = 2,
            SIGNALQUALITY = 3
        }

        private enum RIL_RESULT {
            OK = 1,
            NOCARRIER = 2,
            ERROR = 3,
            NODIALTONE = 4,
            BUSY = 5,
            NOANSWER = 6,
            CALLABORTED = 7
        }

        #region OPERATORNAMES struct
        public enum RIL_OPFORMAT {
            LONG = 0x00000001,
            SHORT = 0x00000002,
            NUM = 0x00000003
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct RILOPERATORNAMES {
            public Int32 cbSize;
            public Int32 dwParams;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = RIL.MAXLENGTH_OPERATOR_LONG)]
            public byte[] szLongName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = RIL.MAXLENGTH_OPERATOR_SHORT)]
            public byte[] szShortName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = RIL.MAXLENGTH_OPERATOR_NUMERIC)]
            public byte[] szNumName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = RIL.MAXLENGTH_OPERATOR_COUNTRY_CODE)]
            public byte[] szCountryCode;
        }

        public class OPERATORNAMES {
            public string LongName;
            public string ShortName;
            public string NumName;
            public string CountryCode;
            public OPERATORNAMES(string LongName, string ShortName, string NumName, string CountryCode) {
                this.LongName = LongName;
                this.ShortName = ShortName;
                this.NumName = NumName;
                this.CountryCode = CountryCode;
            }
        }
        #endregion

        #region CELLINFO struct
        public class CELLINFO {
            public string MobileCountryCode;
            public string MobileNetworkCode;
            public string LocationAreaCode;
            public string CellID;
            public string RxLevel;
            public string RxQuality;
            public CELLINFO(string MobileCountryCode, string MobileNetworkCode, string LocationAreaCode, string CellID, string RxLevel, string RxQuality) {
                this.MobileCountryCode = MobileCountryCode;
                this.MobileNetworkCode = MobileNetworkCode;
                this.LocationAreaCode = LocationAreaCode;
                this.CellID = CellID;
                this.RxLevel = RxLevel;
                this.RxQuality = RxQuality;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RILCELLTOWERINFO {
            public UInt32 cbSize;                       // @field structure size in bytes 
            public UInt32 dwParams;                     // @field indicates valid parameters 
            public UInt32 dwMobileCountryCode;          // @field TBD 
            public UInt32 dwMobileNetworkCode;          // @field TBD 
            public UInt32 dwLocationAreaCode;           // @field TBD 
            public UInt32 dwCellID;                     // @field TBD 
            public UInt32 dwBaseStationID;              // @field TBD 
            public UInt32 dwBroadcastControlChannel;    // @field TBD 
            public UInt32 dwRxLevel;                    // @field Value from 0-63 (see GSM 05.08, 8.1.4) 
            public UInt32 dwRxLevelFull;                // @field Value from 0-63 (see GSM 05.08, 8.1.4) 
            public UInt32 dwRxLevelSub;                 // @field Value from 0-63 (see GSM 05.08, 8.1.4) 
            public UInt32 dwRxQuality;                  // @field Value from 0-7  (see GSM 05.08, 8.2.4) 
            public UInt32 dwRxQualityFull;              // @field Value from 0-7  (see GSM 05.08, 8.2.4) 
            public UInt32 dwRxQualitySub;               // @field Value from 0-7  (see GSM 05.08, 8.2.4) 
            public UInt32 dwIdleTimeSlot;               // @field TBD 
            public UInt32 dwTimingAdvance;              // @field TBD 
            public UInt32 dwGPRSCellID;                 // @field TBD 
            public UInt32 dwGPRSBaseStationID;          // @field TBD 
            public UInt32 dwNumBCCH;                    // @field TBD 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = RIL.MAXLENGTH_BCCH)]
            public byte[] rgbBCCH;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = RIL.MAXLENGTH_NMR)]
            public byte[] rgbNMR;
        }
        #endregion

        #region SIGNALQUALITY struct
        [StructLayout(LayoutKind.Sequential)]
        private struct RILSIGNALQUALITY {
            public UInt32 cbSize;                       // @field structure size in bytes 
            public UInt32 dwParams;                     // @field indicates valid parameters           
            public Int32 nSignalStrength;                // @field TBD 
            public Int32 nMinSignalStrength;             // @field TBD 
            public Int32 nMaxSignalStrength;             // @field TBD 
            public UInt32 dwBitErrorRate;               // @field bit error rate in 1/100 of a percent 
            public Int32 nLowSignalStrength;             // @field TBD 
            public Int32 nHighSignalStrength;            // @field TBD 
        }


        #endregion



    }

    public class SIGNALQUALITY {
        public string SignalStrength;                // @field TBD 
        public string MinSignalStrength;             // @field TBD 
        public string MaxSignalStrength;             // @field TBD 
        public string BitErrorRate;               // @field bit error rate in 1/100 of a percent 
        public string LowSignalStrength;             // @field TBD 
        public string HighSignalStrength;            // @field TBD              

        public SIGNALQUALITY(string SignalStrength, string MinSignalStrength, string MaxSignalStrength, string BitErrorRate, string LowSignalStrength, string HighSignalStrength) {
            this.SignalStrength = SignalStrength;
            this.MinSignalStrength = MinSignalStrength;
            this.MaxSignalStrength = MaxSignalStrength;
            this.BitErrorRate = BitErrorRate;
            this.LowSignalStrength = LowSignalStrength;
            this.HighSignalStrength = HighSignalStrength;
        }
    }

    public class RIL_EVENTS {
        public RIL_EVENTS() {
        }
        #region Events
        public delegate void SignalQualityChangedSelectedHandler(SIGNALQUALITY signal);
        public event SignalQualityChangedSelectedHandler SignalQualityChanged;
        #endregion

        public void RiseSignalQualityChanged(SIGNALQUALITY signal) {
            if (SignalQualityChanged != null) {
                SignalQualityChanged(signal);
            }
        }
    }
}