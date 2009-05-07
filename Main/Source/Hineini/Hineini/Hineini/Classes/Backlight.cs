using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Hineini {
    public class Backlight {

        //ensure the power requirement is released
        ~Backlight() {
            Release();
        }

        //handle to the power requirement
        private static IntPtr handle;

        private enum PowerState {
            PwrDeviceUnspecified = -1,
            //full on
            D0 = 0,
            //low power
            D1 = 1,
            //standby
            D2 = 2,
            //sleep
            D3 = 3,
            //off
            D4 = 4,
            PwrDeviceMaximum = 5
        }

        //keep the backlight lit
        public static void Activate() {
            //request full power
            handle = SetPowerRequirement("BKL1:", PowerState.D0, 1, IntPtr.Zero, 0);
            MessageBox.Show(handle.ToString());
        }

        //release power requirement
        public static void Release() {
            if (handle.ToInt32() != 0) {
                int result = 0;
                result = ReleasePowerRequirement(handle);
                handle = IntPtr.Zero;
            }
        }
        [DllImport("coredll.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetPowerRequirement(string pvDevice, PowerState DeviceState, int DeviceFlags, IntPtr pvSystemState, int StateFlags);
        [DllImport("coredll.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ReleasePowerRequirement(IntPtr handle);
    }
}
