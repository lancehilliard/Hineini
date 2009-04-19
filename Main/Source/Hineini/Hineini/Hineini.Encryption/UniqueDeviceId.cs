using System.Runtime.InteropServices;
using System.Text;

namespace Hineini.Encryption {
    public class UniqueDeviceId {
        /*
            HRESULT GetDeviceUniqueID(
              LPBYTE pbApplicationData,
              DWORD cbApplictionData,
              DWORD dwDeviceIDVersion,
              LPBYTE pbDeviceIDOutput,
              DWORD* pcbDeviceIDOutput
            );
        */

        [DllImport("coredll.dll")]
        private static extern int GetDeviceUniqueID([In, Out] byte[] appdata, int cbApplictionData, int dwDeviceIDVersion, [In, Out] byte[] deviceIDOuput, out uint pcbDeviceIDOutput);

        public static string GetDeviceID(string AppString) {
            // Call the GetDeviceUniqueID
            byte[] AppData = new byte[AppString.Length];
            for (int count = 0; count < AppString.Length; count++) {
                AppData[count] = (byte) AppString[count];
            }
            int appDataSize = AppData.Length;
            byte[] DeviceOutput = new byte[20];
            //uint SizeOut = 20; // TODO remove 20 value?
            uint SizeOut;
            GetDeviceUniqueID(AppData, appDataSize, 1, DeviceOutput, out SizeOut);
            return Encoding.Default.GetString(DeviceOutput, 0, DeviceOutput.Length);
        }
    }
}