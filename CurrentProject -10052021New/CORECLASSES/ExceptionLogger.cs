using System;
using System.Collections.Generic;
using System.Text;

namespace ClassList
{
    public static class ExceptionLogger
    {
        public static bool BackupFlag = true;

        /// <summary>
        /// This method writes log in WExcp.fpe and displays error message to user
        /// Log during save application operation
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="pErrMsg"></param>
        /// <param name="pErrMsgHdr"></param>
        /// <param name="filePath"></param>
        public static void log(Exception ex, string pErrMsg,string pErrMsgHdr,string filePath)
        {
            DateTime datetime = DateTime.Now;
            String oFileName = filePath + CommonConstants.WriteExcepFile;
            System.IO.StreamWriter writer = null;
            try
            {
                writer = new System.IO.StreamWriter(oFileName, true);
                
                writer.WriteLine("====================================================");
                writer.WriteLine(datetime.ToString());
                writer.WriteLine(ex.ToString());
                writer.WriteLine(pErrMsg);
                writer.WriteLine("====================================================");
                
                writer.Close();
                writer.Dispose();
                System.Windows.Forms.MessageBox.Show(pErrMsg, pErrMsgHdr, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
                //System.Windows.Forms.MessageBox.Show("Error occured while writing log file","Log Error", System.Windows.Forms.MessageBoxButtons.OK);
            }

        }

        /// <summary>
        /// This method writes log in DExcp.fpe but do not display message to user 
        /// Log during download application 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="pErrMsg"></param>
        /// <param name="pErrMsgHdr"></param>
        /// <param name="filePath"></param>
        public static void Downloadlog(Exception ex, string pErrMsg, string pErrMsgHdr, string filePath)
        {
            DateTime datetime = DateTime.Now;
            String oFileName = filePath + CommonConstants.DwnlExcepFile;
            System.IO.StreamWriter writer = null;
            try
            {
                writer = new System.IO.StreamWriter(oFileName, true);

                writer.WriteLine("====================================================");
                writer.WriteLine(datetime.ToString());
                writer.WriteLine(ex.ToString());
                writer.WriteLine(pErrMsg);
                writer.WriteLine("====================================================");

                writer.Close();
                writer.Dispose();
               // System.Windows.Forms.MessageBox.Show(pErrMsg, pErrMsgHdr, System.Windows.Forms.MessageBoxButtons.OK);
            }
            catch (Exception e)
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
                //System.Windows.Forms.MessageBox.Show("Error occured while writing log file", "Log Error", System.Windows.Forms.MessageBoxButtons.OK);
            }

        }
        public static void DisplayError(string pErrMsg,string pErrMsgHdr)
        {
            System.Windows.Forms.MessageBox.Show(pErrMsg, pErrMsgHdr, System.Windows.Forms.MessageBoxButtons.OK); 
        }

        /// <summary>
        /// This method writes log in RExcp.fpe and displays error message to user
        /// Log during application open
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="pErrMsg"></param>
        /// <param name="pErrMsgHdr"></param>
        /// <param name="filePath"></param>
        public static void Readlog(Exception ex, string pErrMsg, string pErrMsgHdr, string filePath)
        {
            DateTime datetime = DateTime.Now;
            String oFileName = filePath + CommonConstants.ReadExcepFile;
            System.IO.StreamWriter writer = null;
            try
            {
                BackupFlag = false;
                writer = new System.IO.StreamWriter(oFileName, true);
                
                writer.WriteLine("====================================================");
                writer.WriteLine(datetime.ToString());
                writer.WriteLine(ex.ToString());
                writer.WriteLine(pErrMsg);
                writer.WriteLine("====================================================");


                writer.Close();
                writer.Dispose();
                System.Windows.Forms.MessageBox.Show(pErrMsg, pErrMsgHdr, System.Windows.Forms.MessageBoxButtons.OK);
            }
            catch (Exception e)
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
                //System.Windows.Forms.MessageBox.Show("Error occured while writing log file", "Log Error", System.Windows.Forms.MessageBoxButtons.OK);
            }

        }


        /// <summary>
        /// This method writes log in OperExcp.fpe and displays error message to user
        /// Log during operations performed by user while creating application(e.g. Get/Set object properties)
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="pErrMsg"></param>
        /// <param name="pErrMsgHdr"></param>
        /// <param name="filePath"></param>
        public static void Operationslog(Exception ex, string pErrMsg, string pErrMsgHdr, string filePath)
        {
            DateTime datetime = DateTime.Now;
            String oFileName = filePath + CommonConstants.OperExcepFile;
            System.IO.StreamWriter writer = null;
            try
            {
                if (!System.IO.Directory.Exists(filePath))
                    System.IO.Directory.CreateDirectory(filePath);

                writer = new System.IO.StreamWriter(oFileName, true);

                writer.WriteLine("====================================================");
                writer.WriteLine(datetime.ToString());
                writer.WriteLine(ex.ToString());
                writer.WriteLine(pErrMsg);
                writer.WriteLine("====================================================");

                writer.Close();
                writer.Dispose();
                System.Windows.Forms.MessageBox.Show(pErrMsg, pErrMsgHdr, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }

        }
        /// <summary>
        /// This method writes log in OperExcp.fpe and displays error message to user
        /// Log during operations performed by user while creating application(e.g. Get/Set object properties)
        /// XDir i.e. Log path is constructed in method
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="pErrMsg"></param>
        /// <param name="pErrMsgHdr"></param>
        public static void Operationslog(Exception ex, string pErrMsg, string pErrMsgHdr)
        {
            DateTime datetime = DateTime.Now;
            System.IO.StreamWriter writer = null;
            try
            {
                if (System.IO.Directory.Exists(CommonConstants.ApplicationPath))
                {                    
                    if (!System.IO.Directory.Exists(CommonConstants.ApplicationPath + "\\" + CommonConstants.XProjectData))
                        System.IO.Directory.CreateDirectory(CommonConstants.ApplicationPath + "\\" + CommonConstants.XProjectData);
                    
                    String oFileName = CommonConstants.ApplicationPath + "\\" + CommonConstants.XProjectData + "\\" + CommonConstants.OperExcepFile;
                    writer = new System.IO.StreamWriter(oFileName, true);

                    writer.WriteLine("====================================================");
                    writer.WriteLine(datetime.ToString());
                    writer.WriteLine(ex.ToString());
                    writer.WriteLine(pErrMsg);
                    writer.WriteLine("====================================================");

                    writer.Close();
                    writer.Dispose();
                    System.Windows.Forms.MessageBox.Show(pErrMsg, pErrMsgHdr, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
                else
                    DisplayError(pErrMsg, pErrMsgHdr);
            }
            catch (Exception e)
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        /// <summary>
        /// This method writes log in OperExcp.fpe and displays error message to user
        /// Log during operations performed by user while creating application(e.g. Get/Set object properties)
        /// XDir i.e. Log path is constructed in method Silent logging
        /// </summary>
        /// <param name="ex"></param>
        public static void Operationslog(Exception ex)
        {
            DateTime datetime = DateTime.Now;
            System.IO.StreamWriter writer = null;
            try
            {
                if (System.IO.Directory.Exists(CommonConstants.ApplicationPath))
                {
                    if (!System.IO.Directory.Exists(CommonConstants.ApplicationPath + "\\" + CommonConstants.XProjectData))
                        System.IO.Directory.CreateDirectory(CommonConstants.ApplicationPath + "\\" + CommonConstants.XProjectData);

                    String oFileName = CommonConstants.ApplicationPath + "\\" + CommonConstants.XProjectData + "\\" + CommonConstants.OperExcepFile;
                    writer = new System.IO.StreamWriter(oFileName, true);

                    writer.WriteLine("====================================================");
                    writer.WriteLine(datetime.ToString());
                    writer.WriteLine(ex.ToString());
                    writer.WriteLine("====================================================");

                    writer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception e)
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public static void Operationslog(Exception ex, string pDescp)//SS_Issue1892
        {
            DateTime datetime = DateTime.Now;
            System.IO.StreamWriter writer = null;
            try
            {
                if (System.IO.Directory.Exists(CommonConstants.ApplicationPath))
                {
                    if (!System.IO.Directory.Exists(CommonConstants.ApplicationPath + "\\" + CommonConstants.XProjectData))
                        System.IO.Directory.CreateDirectory(CommonConstants.ApplicationPath + "\\" + CommonConstants.XProjectData);

                    String oFileName = CommonConstants.ApplicationPath + "\\" + CommonConstants.XProjectData + "\\" + CommonConstants.OperExcepFile;
                    writer = new System.IO.StreamWriter(oFileName, true);

                    writer.WriteLine("====================================================");
                    writer.WriteLine(datetime.ToString());
                    writer.WriteLine(ex.ToString());
                    writer.WriteLine("Error Descp: " + pDescp);
                    writer.WriteLine("====================================================");

                    writer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception e)
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }
    }
}
