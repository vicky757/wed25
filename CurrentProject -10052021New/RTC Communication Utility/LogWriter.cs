using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Globalization;


namespace RealTimeGraph
{
    public class LogWriter
    {
        private string m_exePath = string.Empty;
        public LogWriter()
        {

        }
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }
        public void LogWrite(string logMessage)
        {
            LogWriteForUpload(logMessage);
        }
        public void LogWriteForUpload(string logMessage)
        {
            string appFileName = Environment.GetCommandLineArgs()[0];
            string directory = Path.GetDirectoryName(appFileName);
            
            
            m_exePath = directory;
            
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "StatusReport.txt"))
                {
                    Log(logMessage, w);
                    w.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\n");
                txtWriter.WriteLine("{0} {1} {2}  Message :- {3}", DateTime.Now.ToLongTimeString(), DateTime.Now.TimeOfDay.Milliseconds,
                DateTime.Now.ToLongDateString(), logMessage);

            }
            catch (Exception ex)
            {
            }
        }
        public static void WriteLog(string msg)
        {
            new LogWriter(msg);
        }
        public static void WriteLogForUpload(string msg)
        {
            LogWriter log = new LogWriter();
            log.LogWriteForUpload(msg);
        }

    }
}
