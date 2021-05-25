using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTC_Communication_Utility
{
    class BasicProperties
    {
        public static string[] baudRates = { "--Select--", "2400", "4800", "9600", "19200", "115200", "38400" };
        public static string[] dataLengths = { "--Select--", "7", "8" };
        public static string[] parity = { "--Select--", "None", "Odd", "Even" };
        public static string[] stopBits = { "--Select--", "1", "2" };
        public static string[] protocol = { "--Select--", "ASCII", "RTU" };
        public static string[] commands = { "--Select--", "Read Register", "Write One Word",
                                          "Write Multi Words", "Read Bits", "Write Bit" };

        public static string[] address = {  "--Select--", "01-10", "11-20", "21-30", "31-40", "41-50",  
                                     "51-60",  "61-70", "71-80", "81-90", "91-A0", "A1-B0", 
                                     "B1-C0",  "C1-D0", "D1-E0", "E1-F0","F1-F7"
            };

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        private static string[] GetUsbSerDevices()
        {
            // HKLM\SYSTEM\CurrentControlSet\services\usbser\Enum -> Device IDs of what is plugged in
            // HKLM\SYSTEM\CurrentControlSet\Enum\{Device_ID}\Device Parameters\PortName -> COM port name.

            List<string> ports = new List<string>();

            RegistryKey k1 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\usbser\Enum");

            if (k1 == null)
            {
                //Debug.Fail("Unable to open Enum key");
            }
            else
            {
                int count = (int)k1.GetValue("Count");
                for (int i = 0; i < count; i++)
                {
                    object deviceID = k1.GetValue(i.ToString("D", CultureInfo.InvariantCulture));
                    //Debug.Assert(deviceID != null && deviceID is string);

                    RegistryKey k2 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\" +
                        (string)deviceID + @"\Device Parameters");

                    if (k2 == null)
                    {
                        continue;
                    }
                    object portName = k2.GetValue("PortName");

                    //Debug.Assert(portName != null && portName is string);

                    ports.Add((string)portName);
                }
            }
            return ports.ToArray();
        }
    }
}
