using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ClassList
{
    public static class ExceptionHelper
    {
        static string m_exePath1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
               "\\";

        static string exceptionLog = "logfile.txt";

        public static int LineNumber(this Exception e)
        {
            int linenum = 0;
            try
            {
                linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(":line") + 5));
            }
            catch
            {
                //Stack trace is not available!
            }
            return linenum;
        }

        public static void LogFile(string sExceptionName, string sEventName, string sControlName, int nErrorLineNo,
            string sFormName, string fileName)
        {
            try
            {
                if (m_exePath1.Contains(fileName)) { }
                else
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        m_exePath1 += exceptionLog;
                    }
                    else
                    {
                        m_exePath1 += fileName;
                    }
                }
                StreamWriter log;
                if (!File.Exists(m_exePath1))
                {
                    log = new StreamWriter(m_exePath1);
                }
                else
                {
                    log = File.AppendText(m_exePath1);
                }

                log.WriteLine();
                log.WriteLine("===========================================Start==========================================");
                var windowsName = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\", "ProductName", "");
                var version = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\", "CurrentVersion", "");
                var build = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\", "CurrentBuild", "");

                // Write to the file:

                log.WriteLine("windowsName : " + windowsName.ToString());
                log.WriteLine("version : " + version.ToString());
                log.WriteLine("build : " + build.ToString());
                log.WriteLine("Product ID : " + ClassList.CommonConstants.ProductIdentifier.ToString());
                log.WriteLine("64 Bit operating system? : {0}", Environment.Is64BitOperatingSystem ? "Yes" : "No");
                log.WriteLine("---------------------------------------------------------------------------------------------");
                log.WriteLine("Data Time:" + DateTime.Now);
                log.WriteLine("Exception Name:" + sExceptionName);
                log.WriteLine("Event Name:" + sEventName);
                log.WriteLine("Control Name:" + sControlName);
                log.WriteLine("Error Line No.:" + nErrorLineNo);
                log.WriteLine("Form Name:" + sFormName);
                log.WriteLine("=========================================END============================================");
                // Close the stream:
                log.Close();
            }
            catch
            {

            }
        }

    }
}
