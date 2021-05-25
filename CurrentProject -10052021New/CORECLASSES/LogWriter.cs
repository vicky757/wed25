using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Management;
using System.Globalization;
using Microsoft.Win32;
using System.Reflection;
using System.Security.AccessControl;

namespace ClassList
{
    public class LogWriter
    {
        private static int count = 0; //Keeran (KA)
        private string m_exePath = string.Empty;

        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim(); // Keeran (KA)

        public static bool DeviceDriver = true;

        public LogWriter()
        {

        }
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }
        public void LogWrite(string logMessage)
        {
            //LogWriteForUpload(logMessage);
        }
        //public void LogWriteForUpload(string logMessage)
        //{
        //    if (!ClassList.CommonConstants.IslogEnabled)
        //        return;
        //    string appFileName = Environment.GetCommandLineArgs()[0];
        //    string directory = Path.GetDirectoryName(appFileName);
        //    string str = ClassList.CommonConstants.ApplicationPath;
        //    m_exePath = directory;
        //    if (str == "")
        //    {
        //        m_exePath = directory;
        //    }
        //    else
        //    {
        //        m_exePath = str;
        //    }
        //    try
        //    {
        //        using (StreamWriter w = File.AppendText(m_exePath + "\\" + "StatusReport.txt"))
        //        {
        //            Log(logMessage, w);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //public void Log(string logMessage, TextWriter txtWriter)
        //{
        //    try
        //    {
        //        if (ClassList.CommonConstants.IslogEnabled)
        //        {
        //            if (DeviceDriver)
        //            {
        //                ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_PnPSignedDriver");
        //                ManagementObjectCollection objCollection = objSearcher.Get();
        //                foreach (ManagementObject obj in objCollection)
        //                {
        //                    if (Convert.ToString(obj["DeviceName"]) == "HMI USB Device")
        //                    {
        //                        string info = String.Format("Device Driver Info :: Device='{0}',Manufacturer='{1}',DriverVersion='{2}' ", obj["DeviceName"], obj["Manufacturer"], obj["DriverVersion"]);
        //                        txtWriter.WriteLine(info);
        //                    }
        //                }
        //                txtWriter.Write("\r\n");
        //                var windowsName = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\", "ProductName", "");
        //                var version = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\", "CurrentVersion", "");
        //                var build = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\", "CurrentBuild", "");
        //                txtWriter.WriteLine("windowsName : " + windowsName.ToString());
        //                txtWriter.WriteLine("version : " + version.ToString());
        //                txtWriter.WriteLine("build : " + build.ToString());
        //                txtWriter.WriteLine("Product ID : " + ClassList.CommonConstants.ProductIdentifier.ToString());
        //                txtWriter.WriteLine("64 Bit operating system? : {0}", Environment.Is64BitOperatingSystem ? "Yes" : "No");
        //                txtWriter.WriteLine("===========================================Start==========================================");
        //                ManagementClass myManagementClass = new ManagementClass("Win32_Processor");
        //                ManagementObjectCollection myManagementCollection = myManagementClass.GetInstances();
        //                PropertyDataCollection myProperties = myManagementClass.Properties;
        //                Dictionary<string, object> myPropertyResults =
        //                   new Dictionary<string, object>();

        //                foreach (var obj in myManagementCollection)
        //                {
        //                    foreach (var myProperty in myProperties)
        //                    {
        //                        myPropertyResults.Add(myProperty.Name, obj.Properties[myProperty.Name].Value);
        //                    }
        //                }

        //                foreach (var myPropertyResult in myPropertyResults)
        //                {
        //                    if (myPropertyResult.Key.ToString().Trim() == "Caption" ||
        //                        myPropertyResult.Key.ToString().Trim() == "CreationClassName" ||
        //                        myPropertyResult.Key.ToString().Trim() == "CurrentClockSpeed" ||
        //                        myPropertyResult.Key.ToString().Trim() == "CurrentVoltage" ||
        //                        myPropertyResult.Key.ToString().Trim() == "Manufacturer" ||
        //                        myPropertyResult.Key.ToString().Trim() == "MaxClockSpeed" ||
        //                        myPropertyResult.Key.ToString().Trim() == "Name" ||
        //                        myPropertyResult.Key.ToString().Trim() == "NumberOfCores" ||
        //                        myPropertyResult.Key.ToString().Trim() == "NumberOfLogicalProcessors" ||
        //                        myPropertyResult.Key.ToString().Trim() == "PowerManagementSupported" ||
        //                        myPropertyResult.Key.ToString().Trim() == "Status")
        //                    {
        //                        txtWriter.WriteLine("{0} : {1}", myPropertyResult.Key, myPropertyResult.Value);
        //                    }

        //                }
        //                txtWriter.WriteLine("=========================================END============================================");
        //            }
        //            txtWriter.Write("\r\n");
        //            txtWriter.WriteLine("{0} {1} {2}  Message :- {3}", DateTime.Now.ToLongTimeString(), DateTime.Now.TimeOfDay.Milliseconds,
        //            DateTime.Now.ToLongDateString(), logMessage);
        //            DeviceDriver = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        public static void WriteLog(string msg)
        {
            new LogWriter(msg);
        }
        public static void WriteLogForUpload(string msg)
        {
            LogWriter log = new LogWriter();
            //log.LogWriteForUpload(msg);
        }

        #region Keeran (KA)
        public static void WriteToFile(string source, string strLog, string fileName)
        {

            string m_exePath1 = string.Empty;
            m_exePath1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            try
            {
                //if (!Directory.Exists(m_exePath1 + "\\Logsss"))
                //{
                //    // Try to create the directory.
                //    DirectoryInfo di = Directory.CreateDirectory(m_exePath1 + "\\Logsss");
                //}

                string logFilePath = m_exePath1 + "\\Logsss\\" + fileName + "-" +
                System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";

                FileInfo logFileInfo = new FileInfo(logFilePath);

                DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);

                if (!logDirInfo.Exists)
                    logDirInfo.Create();

                if (!File.Exists(logFilePath))
                    File.Create(logFilePath).Dispose();

                // Set Status to Locked
                _readWriteLock.EnterWriteLock();
                try
                {
                    FileSecurity fSecurity = File.GetAccessControl(logFilePath);
                    fSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                    File.SetAccessControl(logFilePath, fSecurity);
                    using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append))
                    {
                        using (StreamWriter log = new StreamWriter(fileStream))
                        {
                            string sourceFile = string.Empty;
                            if (count++ == 0)
                                log.WriteLine("===================================== " + System.DateTime.Now.ToString("dd MMM yyyy HH:mm") + " =====================================");

                            if (source.Contains("\\"))
                            {
                                int idx = source.LastIndexOf("\\");

                                if (idx != -1)
                                {
                                    sourceFile = source.Substring(idx + 1);
                                }

                                log.WriteLine("Source: " + sourceFile);
                            }
                            else
                            {
                                log.WriteLine("Source: " + source);
                            }

                            log.WriteLine("Message: " + strLog);
                        }
                    }
                }
                finally
                {
                    // Release lock
                    _readWriteLock.ExitWriteLock();
                }
            }
            catch (IOException ioex)
            {
                Console.WriteLine(ioex.Message);
            }


        }
        #endregion
    }
}
