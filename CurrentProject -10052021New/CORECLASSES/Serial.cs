/*===================================================================
//
// Copyright 2005-2006, Renu Electronics Pvt. Ltd., Pune, India.
// All Rights Reserved.
//
// The copyright above and this notice must be preserved in all
// copies of this source code.  The copyright above does not
// evidence any actual or intended publication of this source
// code.Connect
//
// This is unpublished proprietary trade secret source code of
// Renu Electronics Pvt. Ltd.  This source code may not be copied,
// disclosed, distributed, demonstrated or licensed except as
// expressly authorized by Renu Electronics Pvt. Ltd.
//
// This source code in its entirity is developed by Renu Electronics 
// Pvt. Ltd
//Plc_Download_Protocol1
// File Name	Serial.cs
// Author		Kapil Vyas
//=====================================================================
*/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
namespace ClassList
{
    /// <summary>
    /// Serial class allows to access( read/write ) Prizm unit from a serial port.
    /// DownLoad and upLoad can be done using the class.
    /// Call Connect(), then call SendFile or ReadFile for Download or upload resp.
    /// and lastly call Close to end communication.
    /// 
    /// A particular signal is sent to initiate communication, response for that 
    /// is sent by unit indicate its reply (is it free to communicate). If it is free, 
    /// then a set frame is send in case of dowloading, which includes all the project related info.
    /// while in case of uploading a corresponding signal is to be sent, in response of that
    /// unit sends a set frame similar in case of downloading. Frames followed by set frame 
    /// are the data frames constitutes the real project serially.
    /// Finally end of file signal is sent to indicate end of the communication.
    /// </summary>

    public class Serial : Device
    {
        #region Constants
        protected const int ACKFRAMESIZE = 1;		// In Bytes	
        protected const int FRAMEDIFFERENCE = 0;
        protected const int CRC_BYTE_SIZE = 1;		// In Bytes
        protected const int DATA_START_INDEX = 3;		// Index
        protected const int STARTFRAMESIZE = 20;		// Index
        protected int DATAFRAMESIZE = 256;	    // In Bytes
        protected int DATAFRAMESIZE1 = 50;	    // In Bytes
        protected const int THRTEE_SEC_DELAY = 30000;
        protected const int ONEFIFTY_SEC_DELAY = 100000;
        protected const int TEN_SEC_DELAY = 10000;    // In MiliSec
        protected const int SEVEN_SEC_DELAY = 7000;    // In MiliSec
        protected const int HALF_SEC_DELAY = 500;      // In MiliSec
        protected const int THREE_SEC_DELAY = 3000;	    // In MilSec
        protected const int TWO_SEC_DELAY = 2000;	    // In MilSec
        protected const int RECVFRAMESIZE = 1000;	    // In Bytes
        public int NO_OF_TRY_FOR_READY_PRIZM = 2;

        // Communication with Unit Constants
        protected const int DNLD_COMPLETE = 0xCC;
        protected const int CRC_ERROR = 0xFF;
        protected const int PRODUCT_MISSMATCH = 0x81;
        protected const int EXPANSIONSLOT_MISMATCH = 0x82;
        protected const int CORRECT_CRC = 0x01;
        protected const int FINISH = 0xE3;
        protected const int TOLERANCE_BYTE_1 = 0x01;
        protected const int TOLARANCE_BYTE_2 = 0x02;
        protected const int TOLARANCE_BYTE_3 = 0x03;
        protected const int TOLARANCE_BYTE_4 = 0x04;
        protected const int TOLERANCE_BYTE_5 = 0x05;
        protected const int TOLERANCE_BYTE_6 = 0x06;
        protected const int FINISH_UPLOAD = 0xDD;
        //New FP3035 Product Series_V2.3_Issue_447 SP
        protected const int TOLERANCE_BYTE_7 = 0x07;
        //End

        // DownLoads
        protected const int SERIAL_LADDER_DNLD = 0x77;
        protected const int SERIAL_FONT_DNLD = 0x88;
        protected const int SERIAL_FIRMWARE_DNLD = 0x99;
        protected const int SERIAL_APPLICATION_DNLD = 0xEE;
        protected const int SERIAL_ETHER_SETTINGS_DNLD = 0x44;
        protected const int SERIAL_LOGGEDDATA_UPLD = 0xAA;
        protected const int SERIAL_FHWT_DNLD = 0x33;
        protected const int SERIAL_HIST_ALARM_DATA_UPLD = 0x22;  //manik

        protected const int ERR_RECONNECTUNIT = 0x0A;
        protected const int ERR_SEARCHINGUNIT = 0x01;
        #endregion

        #region Private Variables

        protected int _serialTimeOut;
        protected byte _serialCrcByte;
        protected byte _serialTotalCrcByte;
        protected int _serialNoOfSent;
        protected byte _serialReceivedByte;
        protected int _serialNoOfByteRead;
        protected int _serialNoOfByteTORead;
        protected byte[] _serialRecvBuff = null;
        protected byte[] _serialSendBuff;
        protected int _serialFileID;
        protected string _serialIPAddress;
        protected string _serialSubnetMask;
        protected string _serialDefaultGateway;
        protected int _serialEthSettingPortNo;
        protected bool _serialDHCPFlag;
        protected bool _serialTimeOutFlag;//FlexiSoft2_2_Issue_no_546_AMIT
        private static bool _serialDataLoggerEraseMem;

        private Thread _serialThread;
        private Thread _serialThread1;
        protected System.Timers.Timer _serialATimer;

        protected System.IO.Ports.SerialPort _serialPort;
        private static List<byte[]> list = new List<byte[]>(); //Keeran (KA)
        private static int DATAFRAMESIZE11 = 256; //Keeran (KA)
        #region FP_Ethernet_Implementation-AMIT
        protected byte _serialBCC;
        #endregion

        //FP_CODE Pravin Download Mode
        private byte[] s_setupFrameMode;
        //End

        #region FP_CODE     Issue No. 190	Punam
        protected byte[] _serialProductIDBuffer;
        protected short iProductID = 0;
        #endregion FP_CODE

        //FP_Code Pravin Serial Upload
        private Thread _SerialThread;
        private int i_RetVal = 0;
        //End
        #endregion

        #region Public Events
        public delegate void DownloadPercentage(float pPercentage);
        public event DownloadPercentage _deviceDownloadPercentage;

        public delegate void DownloadStatus(int pMessage);
        public event DownloadStatus _deviceDownloadStatus;

        #endregion

        #region Public Methods

        #region GSM_Sanjay
        // to set the port for device information in serial type
        public Serial(string pPort)
        {
            string portID;
            portID = pPort.Substring(3, pPort.Length - 3);
            _devicePortNo = Convert.ToByte(portID);

            _serialSendBuff = new byte[DATAFRAMESIZE + STARTFRAMESIZE];
            _serialRecvBuff = new byte[DATAFRAMESIZE + STARTFRAMESIZE];
        }
        #endregion GSM_Sanjay

        public Serial()
        {
            _devicePortNo = 1;
            _serialNoOfSent = 0;
            _serialReceivedByte = 0;
            _serialNoOfByteRead = 0;

            _serialTimeOut = 0;
            _serialCrcByte = 0;
            _serialTotalCrcByte = 0;

            _serialSendBuff = new byte[DATAFRAMESIZE + STARTFRAMESIZE];
            _serialRecvBuff = new byte[DATAFRAMESIZE + STARTFRAMESIZE];

            #region FP_Ethernet_Implementation-AMIT
            _serialEthSettingPortNo = Convert.ToInt32(Device.EthernetSettings._DownloadPort);
            _serialIPAddress = Device.EthernetSettings._IPAdderess;
            _serialSubnetMask = Device.EthernetSettings._SubnetMask;
            _serialDefaultGateway = Device.EthernetSettings._DefaultGateway;
            _serialDHCPFlag = Device.EthernetSettings._DHCP;
            #endregion

            _serialThread = new Thread(new ThreadStart(DownLoadFunc));
        }
        public static string GetQTProductPort()
        {
            string strPort = "None";
            string[] strPortName = SerialPort.GetPortNames();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SerialPort");
            ManagementObjectCollection myPortCollection = searcher.Get();
            if (myPortCollection.Count > 0)
            {
                foreach (ManagementObject mo in myPortCollection)
                {
                    if (mo["PNPDeviceID"].ToString().ToUpper().Contains(@"USB\VID_1B31&PID_0002\"))
                        return mo["DeviceID"].ToString();
                    //if (mo["PNPDeviceID"].ToString().ToUpper().Contains(@"USB\VID_0525&PID_A4A7\"))
                    //    return mo["DeviceID"].ToString();
                }
            }
            return strPort;
        }
        // Attention
        // COM ports can be more than 3 !!!, how do we code in that case ??
        //FP_CODE Pravin Download Mode
        public Serial(string pPort, int pBaudRate, byte pParity, byte pByteSize, byte pStopBits, byte[] _setupFrameMode)
        {
            string[] ethSettingStrings;
            string portID;
            if (pPort.Contains("|"))
            {
                ethSettingStrings = pPort.Split('|');

                //FP_CODE  Pravin  EthernetSettings Download 
                /*if (ethSettingStrings[0] == "COM1") _devicePortNo = 1;
                else if (ethSettingStrings[0] == "COM2") _devicePortNo = 2;
                else if (ethSettingStrings[0] == "COM3") _devicePortNo = 3;
                else if (ethSettingStrings[0] == "COM4") _devicePortNo = 4;
                else if (ethSettingStrings[0] == "COM5") _devicePortNo = 5;
                else if (ethSettingStrings[0] == "COM6") _devicePortNo = 6;*/

                if (pPort.Contains("COM"))
                {
                    portID = ethSettingStrings[0].Substring(3, ethSettingStrings[0].Length - 3);
                    _devicePortNo = Convert.ToByte(portID);
                }
                else
                {
                    if (ethSettingStrings[0] == "COM1") _devicePortNo = 1;
                    else if (ethSettingStrings[0] == "COM2") _devicePortNo = 2;
                    else if (ethSettingStrings[0] == "COM3") _devicePortNo = 3;
                    else if (ethSettingStrings[0] == "COM4") _devicePortNo = 4;
                    else if (ethSettingStrings[0] == "COM5") _devicePortNo = 5;
                    else if (ethSettingStrings[0] == "COM6") _devicePortNo = 6;
                }

                _serialEthSettingPortNo = Convert.ToInt32(ethSettingStrings[4]);
                _serialIPAddress = ethSettingStrings[1];
                _serialSubnetMask = ethSettingStrings[2];
                _serialDefaultGateway = ethSettingStrings[3];
                if (ethSettingStrings[5] == "True")
                    _serialDHCPFlag = true;
            }
            else
            {
                if (pPort.Contains("COM"))
                {
                    portID = pPort.Substring(3, pPort.Length - 3);
                    _devicePortNo = Convert.ToByte(portID);
                }
                else
                {
                    if (pPort == "COM1") _devicePortNo = 1;
                    else if (pPort == "COM2") _devicePortNo = 2;
                    else if (pPort == "COM3") _devicePortNo = 3;
                    else if (pPort == "COM4") _devicePortNo = 4;
                    else if (pPort == "COM5") _devicePortNo = 5;
                    else if (pPort == "COM6") _devicePortNo = 6;
                }

                #region FP_Ethernet_Implementation-AMIT
                _serialEthSettingPortNo = Convert.ToInt32(Device.EthernetSettings._DownloadPort);
                _serialIPAddress = Device.EthernetSettings._IPAdderess;
                _serialSubnetMask = Device.EthernetSettings._SubnetMask;
                _serialDefaultGateway = Device.EthernetSettings._DefaultGateway;
                _serialDHCPFlag = Device.EthernetSettings._DHCP;
                #endregion
            }
            _serialNoOfSent = 0;
            _serialReceivedByte = 0;
            _serialNoOfByteRead = 0;

            _serialTimeOut = 0;
            _serialCrcByte = 0;
            _serialTotalCrcByte = 0;

            _arrdwnlsetupframe = new byte[10];
            _arrdwnlsetupframe = _setupFrameMode;
            //End

            _serialSendBuff = new byte[1000 + STARTFRAMESIZE];
            _serialRecvBuff = new byte[1000 + STARTFRAMESIZE];

            _serialThread = new Thread(new ThreadStart(DownLoadFunc));
        }
        //End

        ~Serial()
        {
            _serialRecvBuff = null;
            if (_serialPort != null)
                if (_serialPort.IsOpen)
                    _serialPort.Close();
        }

        //Software Crash Issue - While uploading if Port is Busy s/w crashes
        public bool IsDeviceConnected()
        {
            return _deviceIsConnected;
        }

        public override int Connect()
        {
            LogWriter.WriteToFile("Connect.cs - Connect()", "started--", "RTC_Upgrade");
            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));

            //if (!_deviceIsConnected)
            //{
            //    CommonConstants.communicationStatus = 0;
            //    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPortIsBusyOrUnavailable));
            //    // If Already Connected/Port already in use
            //    return FAILURE;
            //}
            try
            {
                _serialPort = new SerialPort(ClassList.CommonConstants.SetPortName);
                _serialPort.WriteTimeout = 2000;
                _serialPort.ReadTimeout = 0xFFFFFFF;

                _serialPort.BaudRate = Convert.ToInt32(ClassList.CommonConstants.setBaudRate);
                try
                {
                    switch (ClassList.CommonConstants.setParity)
                    {
                        case "Even":
                            _serialPort.Parity = Parity.Even;
                            break;
                        case "Odd":
                            _serialPort.Parity = Parity.Odd;
                            break;
                        case "None":
                        default:
                            _serialPort.Parity = Parity.None;
                            break;
                    }
                }
                catch
                {
                    _serialPort.Parity = Parity.None;
                }

                _serialPort.DataBits = Convert.ToInt32(ClassList.CommonConstants.setBitsLength);
                try
                {
                    switch (ClassList.CommonConstants.setStopbits)
                    {
                        case "1":
                            _serialPort.StopBits = StopBits.One;
                            break;
                        case "1.5":
                            _serialPort.StopBits = StopBits.OnePointFive;
                            break;
                        case "2":
                            _serialPort.StopBits = StopBits.Two;
                            break;
                        default:
                            _serialPort.StopBits = StopBits.None;
                            break;

                    }
                }
                catch
                {
                    _serialPort.StopBits = StopBits.None;
                }

                LogWriter.WriteToFile("default settings: ", _serialPort.PortName + " " + _serialPort.BaudRate + " " +
                _serialPort.Parity + " " + _serialPort.DataBits + " " + _serialPort.StopBits, "RTC_Upgrade");

                _serialPort.ReadBufferSize = 8500;

                try
                {
                    if (!_serialPort.IsOpen)
                        
                        _serialPort.Open();
                }
                catch (Exception exx)
                {
                    return FAILURE;
                }
           
                if (!_serialPort.IsOpen)
                {
                    _deviceIsConnected = false;
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPortIsBusyOrUnavailable));
                    return FAILURE;
                }
                else
                {
                    _deviceIsConnected = true;
                    return SUCCESS;
                }
            }
            catch (Exception e)
            {
                LogWriter.WriteToFile(e.StackTrace, e.Message, "RTC_Upgrade");
                MessageBox.Show("Port is either busy or not exists!!", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPortIsBusyOrUnavailable));
                return FAILURE;
            }

        }


        public int ConnectForMoveEBit()
        {
            LogWriter.WriteToFile("Connect.cs - Connect()", "started--", "RTC_Upgrade");
            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));

            //if (!_deviceIsConnected)
            //{
            //    CommonConstants.communicationStatus = 0;
            //    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPortIsBusyOrUnavailable));
            //    // If Already Connected/Port already in use
            //    return FAILURE;
            //}
            try
            {

                _serialPort.Close();
                Thread.Sleep(100);
                _serialPort = new SerialPort(ClassList.CommonConstants.SetPortName);
                _serialPort.WriteTimeout = 2000;
                _serialPort.ReadTimeout = 0xFFFFFFF;

                _serialPort.BaudRate = Convert.ToInt32(ClassList.CommonConstants.setBaudRate);
                try
                {
                    switch (ClassList.CommonConstants.setParity)
                    {
                        case "Even":
                            _serialPort.Parity = Parity.Even;
                            break;
                        case "Odd":
                            _serialPort.Parity = Parity.Odd;
                            break;
                        case "None":
                        default:
                            _serialPort.Parity = Parity.None;
                            break;
                    }
                }
                catch
                {
                    _serialPort.Parity = Parity.None;
                }

                _serialPort.DataBits = 8;
                try
                {
                    switch (ClassList.CommonConstants.setStopbits)
                    {
                        case "1":
                            _serialPort.StopBits = StopBits.One;
                            break;
                        case "1.5":
                            _serialPort.StopBits = StopBits.OnePointFive;
                            break;
                        case "2":
                            _serialPort.StopBits = StopBits.Two;
                            break;
                        default:
                            _serialPort.StopBits = StopBits.None;
                            break;

                    }
                }
                catch
                {
                    _serialPort.StopBits = StopBits.None;
                }

                LogWriter.WriteToFile("default settings: ", _serialPort.PortName + " " + _serialPort.BaudRate + " " +
                _serialPort.Parity + " " + _serialPort.DataBits + " " + _serialPort.StopBits, "RTC_Upgrade");

                _serialPort.ReadBufferSize = 8500;
                Thread.Sleep(50);
                try
                {
                    if (!_serialPort.IsOpen)
                        _serialPort.Open();
                }
                catch (Exception exx)
                {
                    return FAILURE;
                }

                if (!_serialPort.IsOpen)
                {
                    _deviceIsConnected = false;
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPortIsBusyOrUnavailable));
                    return FAILURE;
                }
                else
                {
                    _deviceIsConnected = true;
                    return SUCCESS;
                }
            }
            catch (Exception e)
            {
                LogWriter.WriteToFile(e.StackTrace, e.Message, "RTC_Upgrade");
                MessageBox.Show("Port is either busy or not exists!!", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPortIsBusyOrUnavailable));
                return FAILURE;
            }

        }

        public int ConnectActualSetting( int DBval)
        {
            LogWriter.WriteToFile("Connect.cs - Connect()", "started--", "RTC_Upgrade");
            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));

            //if (!_deviceIsConnected)
            //{
            //    CommonConstants.communicationStatus = 0;
            //    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPortIsBusyOrUnavailable));
            //    // If Already Connected/Port already in use
            //    return FAILURE;
            //}
            try
            {

                _serialPort.Close();
                Thread.Sleep(100);
                _serialPort = new SerialPort(ClassList.CommonConstants.SetPortName);
                _serialPort.WriteTimeout = 2000;
                _serialPort.ReadTimeout = 0xFFFFFFF;

                _serialPort.BaudRate = Convert.ToInt32(ClassList.CommonConstants.setBaudRate);
                try
                {
                    switch (ClassList.CommonConstants.setParity)
                    {
                        case "Even":
                            _serialPort.Parity = Parity.Even;
                            break;
                        case "Odd":
                            _serialPort.Parity = Parity.Odd;
                            break;
                        case "None":
                        default:
                            _serialPort.Parity = Parity.None;
                            break;
                    }
                }
                catch
                {
                    _serialPort.Parity = Parity.None;
                }

                _serialPort.DataBits = DBval;
                try
                {
                    switch (ClassList.CommonConstants.setStopbits)
                    {
                        case "1":
                            _serialPort.StopBits = StopBits.One;
                            break;
                        case "1.5":
                            _serialPort.StopBits = StopBits.OnePointFive;
                            break;
                        case "2":
                            _serialPort.StopBits = StopBits.Two;
                            break;
                        default:
                            _serialPort.StopBits = StopBits.None;
                            break;

                    }
                }
                catch
                {
                    _serialPort.StopBits = StopBits.None;
                }

                LogWriter.WriteToFile("default settings: ", _serialPort.PortName + " " + _serialPort.BaudRate + " " +
                _serialPort.Parity + " " + _serialPort.DataBits + " " + _serialPort.StopBits, "RTC_Upgrade");

                _serialPort.ReadBufferSize = 8500;
                Thread.Sleep(50);
                try
                {
                    if (!_serialPort.IsOpen)
                        _serialPort.Open();
                }
                catch (Exception exx)
                {
                    return FAILURE;
                }

                if (!_serialPort.IsOpen)
                {
                    _deviceIsConnected = false;
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPortIsBusyOrUnavailable));
                    return FAILURE;
                }
                else
                {
                    _deviceIsConnected = true;
                    return SUCCESS;
                }
            }
            catch (Exception e)
            {
                LogWriter.WriteToFile(e.StackTrace, e.Message, "RTC_Upgrade");
                MessageBox.Show("Port is either busy or not exists!!", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPortIsBusyOrUnavailable));
                return FAILURE;
            }

        }


        /// <summary>
        /// This function sends the given file to the unit.
        /// File is sent to the unit using the predefined protocol between IBM PC and unit.
        /// On success it returns zero and on failure error number specifying the type of error.
        /// </summary>
        /// <param name="pFileName">File Name which is to be send</param>
        /// <param name="pFileID">type of the file</param>
        /// <returns>SUCCESS or FAILURE</returns>
        public override int SendFile(string pFileName, byte pFileID)
        {
            LogWriter.WriteToFile("Serial.cs - SendFile()", "started--", "RTC_Upgrade");
            _serialFileID = pFileID;
            string appFileName = pFileName;
            _serialNoOfSent = 0;
            pFileName = GetFileID(pFileID);
            _deviceFileName = appFileName;

            if (!File.Exists(_deviceFileName) && _deviceProdID != SERIAL_ETHER_SETTINGS_DNLD)// will never occure, but precautionary
            {
                if (_deviceFileID == CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorApplicationFileCreation));
                else if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorFirmwareFileCreation));
                Close();
                CommonConstants.communicationStatus = 0;
                return FAILURE;
            }
            if (_deviceIsConnected == false)
            {
                LogWriter.WriteToFile("Serial.cs- SendFile()", "device not connected", "RTC_Upgrade");
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(10);
                Close();
                return FAILURE;
            }

            _serialThread.Priority = ThreadPriority.Highest;
            #region FP_Ethernet_Implementation-AMIT
            if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
            {
                _serialThread = new Thread(new ThreadStart(InitiateCommunicationEtherSettings));
                _serialThread.Start();
                return SUCCESS;
            }
            #endregion
            else if (InitiateCommunication() == SUCCESS)
            {
                Thread.Sleep(100);
                if (_deviceFileID != SERIAL_ETHER_SETTINGS_DNLD)
                    _serialThread.Start();
                return SUCCESS;
            }
            else
                return SUCCESS;

            CommonConstants.communicationStatus = 0;
            return FAILURE;
        }

        public int SendFile1(string pFileName, byte pFileID)
        {
            _serialFileID = pFileID;

            _serialNoOfSent = 0;
            string appFileName = Environment.GetCommandLineArgs()[0];

            string str1 = Path.GetDirectoryName(appFileName);
            _deviceFileName = pFileName;

            if (!File.Exists(pFileName) && _deviceProdID != SERIAL_ETHER_SETTINGS_DNLD)// will never occure, but precautionary
            {
                if (_deviceFileID == CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorApplicationFileCreation));
                else if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorFirmwareFileCreation));
                Close();
                CommonConstants.communicationStatus = 0;
                return FAILURE;
            }
            if (_deviceIsConnected == false)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(10);
                Close();
                return FAILURE;
            }
            //start thread
            else if (InitiateCommunication1() == SUCCESS)
            {
                _serialThread1 = new Thread(new ThreadStart(DownLoadFunc2));
                _serialThread1.Priority = ThreadPriority.Highest;
                _serialThread1.Start();
                return SUCCESS;
            }
            return SUCCESS;
        }

        /// <summary>
        /// This function receives the file from the unit.
        /// File is received from the unit using the predefined protocol between IBM PC and unit
        /// On success it returns zero and on failure error number specifying the type of error.
        /// </summary>
        /// <param name="pFileName">File name to be received/ copy </param>
        /// <returns>SUCCESS or FAILURE</returns>
        public override int ReceiveFile(string pFileName, int pFileID)
        {
            //FP_CODE Pravin Application + Ladder Upload
            CommonConstants.LADDER_PRESENT = false;
            //End

            _serialFileID = pFileID;
            _deviceFileName = pFileName;
            #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
            //if ((pFileID & (int)DownloadData.HistAlarmData) > 0)	// Manik
            //    GetUploadFileIDForAlarm(pFileID);
            //else
            //    GetUploadFileID(pFileID);
            GetUploadFileIDForAlarm(pFileID);
            #endregion
            if ((pFileID & (int)DownloadData.HistAlarmData) > 0)	//Manik
                _serialThread = new Thread(new ThreadStart(UpLoadFuncForAlarm));
            else
                _serialThread = new Thread(new ThreadStart(UpLoadFunc));
            _serialThread.Priority = ThreadPriority.Highest;

            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
            {
                _serialPort.DiscardInBuffer();
                Thread.Sleep(10);
                //issue_141_V2.2_sammed
                try
                {
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
                catch (Exception ex)
                {
                    CommonConstants.communicationType = 2;
                    CommonConstants.communicationStatus = -1;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }
                //End
                Thread.Sleep(100);
                if (ReceiveFrame(DOWN))
                {
                    if ((_serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2) || (_serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_5) || (_serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_6))
                        break;
                    #region New FP3035 Product Series_V2.3_Issue_447 SP
                    else if (_serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_7)
                    {
                        _deviceFileID = CommonConstants.SERIAL_APPLICATION_UPLD_FILEID;
                        _deviceFileName = CommonConstants.TEMP_DOWNLOAD_FILENAME;
                        _serialFileID = 0;
                        _serialSendBuff[0] = CommonConstants.SERIAL_APPLICATION_UPLD_FILEID;
                        //New FP3035 Product Series_V2.3_Issue_606 SP
                        if (pFileID == 21)
                            _serialFileID |= Convert.ToInt32(DownloadData.LoggedData);
                        else if (pFileID == 261)
                            _serialFileID |= Convert.ToInt32(DownloadData.HistAlarmData);
                        else if (pFileID == 277)
                        {
                            //_serialFileID |= Convert.ToInt32(DownloadData.LoggedData);
                            _serialFileID |= Convert.ToInt32(DownloadData.HistAlarmData);
                        }
                        //End
                    }
                    #endregion
                    else
                    {
                        _serialPort.DiscardInBuffer();
                        Thread.Sleep(SEVEN_SEC_DELAY);
                    }
                }
            }
            Thread.Sleep(500);
            //if (_serialReceivedByte == _deviceFileID || _serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_1 || _serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2 || _deviceFileID == SERIAL_LOGGEDDATA_UPLD)
            if (_serialReceivedByte == _deviceFileID || _serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_1 || _serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2 || ((_deviceFileID == SERIAL_LOGGEDDATA_UPLD) || (_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD)))	//manik
            {	//	If Prizm Ready,
                _serialTimeOut = 0;
                //if (_serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2 && _deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                if (_serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2 && ((_deviceFileID != SERIAL_LOGGEDDATA_UPLD) && (_deviceFileID != SERIAL_HIST_ALARM_DATA_UPLD)))	//manik
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);
                    _serialPort.Write(_serialSendBuff, 0, 4);
                }
            }
            else if (_serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_5) //Application or ladder not present
            {
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strApplicationNotPresent));
                CommonConstants.communicationType = 2;
                CommonConstants.communicationStatus = -1;
                Close();
                return FAILURE;
            }
            else if (_serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_6) //Not Supported for FlexiLogics
            {
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPLCNotSupportedToMPLC));
                CommonConstants.communicationType = 2;
                CommonConstants.communicationStatus = -1;
                Close();
                return FAILURE;
            }
            else // Unit Not Ready
            {
                CommonConstants.communicationType = 2;
                CommonConstants.communicationStatus = -1;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                Close();
                return FAILURE;	//	FAILURE
            }
            //_deviceDownloadStatus(2);

            int message = 256;
            if (_serialFileID == CommonConstants.SERIAL_LOGGED_UPLD_FILEID)
                message <<= 4;
            if (_serialFileID == CommonConstants.SERIAL_HISTALARM_UPLD_FILEID)
                message <<= 5;

            //FP_CODE Pravin Application + Ladder Upload
            if (_deviceFileID == CommonConstants.LADDER_UPLD_FILEID)
                message <<= 2;
            //End

            _deviceDownloadStatus(2 + message);

            Thread.Sleep(500);
            _serialThread.Start();
            //_serialThread.Join();               
            //_deviceDownloadStatus(18);
            //Close();
            return SUCCESS;
        }

        public int ReceiveFile_UploadLadder(string pFileName, byte pFileID)
        {
            _deviceFileName = pFileName;
            _serialFileID = pFileID;
            _serialSendBuff[0] = pFileID;
            GetUploadFileID(pFileID);
            _SerialThread = new Thread(new ThreadStart(UpLoadFunc_UploadLadder));


            if (pFileID != SERIAL_LOGGEDDATA_UPLD)
            {
                for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
                {
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(10);
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                    Thread.Sleep(100);
                    if (ReceiveFrame(DOWN))
                    {
                        if ((_serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2) || (_serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_5) || (_serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_6))
                            break;
                        else
                        {
                            _serialPort.DiscardInBuffer();
                            Thread.Sleep(SEVEN_SEC_DELAY);
                        }
                    }
                }
            }
            else
                _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);

            //End
            if (_serialReceivedByte == _deviceFileID || _serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_1 || _serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2 || ((_deviceFileID == SERIAL_LOGGEDDATA_UPLD) || (_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD)))	//manik
            {	//	If Prizm Ready,
                _serialTimeOut = 0;
                //if (_serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2 && _deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                if (_serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2 && ((_deviceFileID != SERIAL_LOGGEDDATA_UPLD) && (_deviceFileID != SERIAL_HIST_ALARM_DATA_UPLD)))	//manik
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);
                    _serialPort.Write(_serialSendBuff, 0, 4);
                }
            }
            else if (_serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_5) //Application or ladder not present
            {
                Close();
                return 2;
            }
            else if (_serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_6) //Not Supported for FlexiLogics
            {
                Close();
                return 3;
            }
            else // Unit Not Ready
            {
                CommonConstants.communicationType = 2;
                CommonConstants.communicationStatus = -1;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                Close();
                return FAILURE;	//	FAILURE
            }
            //_deviceDownloadStatus(2);
            if (_serialFileID == SERIAL_LOGGEDDATA_UPLD)
                _serialSendBuff = new byte[1024 * 8 + 1 + DATA_START_INDEX + 4];

            _SerialThread.Start();
            _SerialThread.Join();
            //_deviceDownloadStatus(4);
            if (i_RetVal == CommonConstants.FAILURE)
                return FAILURE;
            else
                return SUCCESS;
        }

        public void UpLoadFunc_UploadLadder()
        {
            byte[] _arr = new byte[4];
            bool error = false;
            int bytesRead = 0;
            FileStream fs;
            CommonConstants.communicationStatus = 2;
            CommonConstants.communicationType = 2;
            bool logger_flag = false;
            uint Ladder_Size = 0;

            try
            {
                if (_deviceFileName == CommonConstants.UPLOAD_LADDER_FILENAME)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        _serialRecvBuff[i] = (byte)_serialPort.ReadByte();
                    }
                    Ladder_Size = CommonConstants.MAKEUINT(_serialRecvBuff);
                    _serialSendBuff[0] = 0x01;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);

                }

                for (int iTemp = 0; true; iTemp++)
                {
                    fs = new FileStream(_deviceFileName, FileMode.Create);
                    if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                        ReceiveFrame(DOWN);
                    else
                    {
                        //while (true) //Remove this while loop to avoid infinte loop
                        for (int j = 0; j < 3; j++)
                        {
                            if (_serialRecvBuff[0] != SERIAL_LOGGEDDATA_UPLD)
                            {
                                if (ReceiveFrame(DOWN))
                                {
                                    logger_flag = true;
                                    break;
                                }
                            }
                        }
                        if (!logger_flag)
                        {
                            error = true;
                            break;
                        }
                    }
                    int message = 256;
                    if (_deviceFileID == CommonConstants.SERIAL_LOGGED_UPLD_FILEID)
                    {
                        message <<= 4;
                        //_deviceDownloadStatus(2 + message);
                    }
                    _deviceTotalFrames = 2; // Temp assigned to execute atleast once so that it get initialized
                    for (int i = 1; i < _deviceTotalFrames + 1; i++)
                    {
                        if (_deviceDownloadPercentage != null && CommonConstants.communicationStatus != -1 && i > 1)
                        {
                            float f;
                            if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                                f = 100 * i / Convert.ToInt32(_deviceTotalFrames / RECVFRAMESIZE);
                            else
                                f = 100 * i / Convert.ToInt32(_deviceTotalFrames);

                            //_deviceDownloadPercentage(f < 100 ? f : 100);
                        }

                        if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD && i == 1)
                        {
                            ReadLoggingStartBytes();
                            //ReceiveFrameTest(UP);
                            _serialNoOfByteTORead += 2;
                            int tempID = _serialSendBuff[0];
                            tempID = tempID & 0x0F;
                            _deviceTotalFrames = tempID == 3 ? (uint)(20 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : tempID == 1 ? (uint)(5 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : tempID == 2 ? (uint)(10 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : (uint)(2 * (1024 * 1024) + 256 * 1024) / (1024 * 8);
                            switch (tempID)
                            {
                                case 0:
                                    _deviceTotalFrames = 512 * 1024; //256+256 //Mem settings + 256
                                    break;
                                case 1:
                                    _deviceTotalFrames = 768 * 1024; //512+256
                                    break;
                                case 2:
                                    _deviceTotalFrames = 1280 * 1024; //1024+256
                                    break;
                                case 3:
                                    _deviceTotalFrames = 2304 * 1024; //2048+256
                                    break;
                            }

                            if (_serialSendBuff[0] > 0x0f)
                                _deviceTotalFrames -= 256 * 1024;
                            //_deviceTotalFrames = (uint)0x520;                    
                            bytesRead = _serialNoOfByteRead - 3;
                            _deviceTotalFrames /= 8192;
                        }
                        if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD && i != 1)
                            ReceiveFrameTest(UP);
                        else if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                            if (ReceiveFrame(UP) == false)
                            {
                                error = true;
                                break;
                            }

                        if (i == 1 && _deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                        {
                            if (_deviceFileID == CommonConstants.LADDER_UPLD_FILEID)
                            {
                                _deviceTotalFrames = Ladder_Size % RECVFRAMESIZE > 0 ? Ladder_Size / RECVFRAMESIZE + 1 : Ladder_Size / RECVFRAMESIZE;
                                if (_deviceTotalFrames == 0)
                                    _deviceDownloadPercentage(100);
                            }
                            else
                            {
                                iProductID = ClassList.CommonConstants.MAKEWORD(_serialSendBuff[6], _serialSendBuff[7]);
                                _serialNoOfByteTORead += 2;
                                _deviceTotalFrames = _serialSendBuff[2];
                                _deviceTotalFrames += Convert.ToUInt32(_serialSendBuff[3] * 256);
                                _deviceTotalFrames += Convert.ToUInt32(_serialSendBuff[4] << 16);
                                _deviceTotalFrames += Convert.ToUInt32(_serialSendBuff[5] << 24);
                            }

                            #region FP_CODE     Issue No. 190	Punam
                            _serialProductIDBuffer = _serialSendBuff;
                            iProductID = ClassList.CommonConstants.MAKEWORD(_serialProductIDBuffer[6], _serialProductIDBuffer[7]);
                            #endregion FP_CODE

                        }

                        if (CommonConstants.communicationStatus == -1 || CommonConstants.communicationStatus == -2)
                        {
                            //_deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationTimeout));
                            error = true;
                            break;
                        }

                        if (_serialSendBuff[0] == 0xdd)
                            break;

                        if (i != 1)
                        {
                            bytesRead = _serialSendBuff[0];
                            bytesRead += _serialSendBuff[1] * 256;
                        }
                        else
                        {
                            bytesRead = _serialNoOfByteRead - DATA_START_INDEX;
                        }
                        if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                            CalculateCRC(bytesRead);
                        if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                            if (i < 2)
                                fs.Write(_serialSendBuff, 1, bytesRead + 1);
                            else
                                fs.Write(_serialSendBuff, 3, bytesRead);
                        else
                            fs.Write(_serialSendBuff, 2, bytesRead);
                        _serialSendBuff[0] = 0x01;
                        _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);

                        if (_deviceFileID != CommonConstants.LADDER_UPLD_FILEID)
                        {
                            if (i >= (_deviceTotalFrames % RECVFRAMESIZE > 0 ? _deviceTotalFrames / RECVFRAMESIZE + 1 : _deviceTotalFrames / RECVFRAMESIZE) && _deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                                break;
                            if (i >= _deviceTotalFrames && _deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                                break;
                        }
                    }
                    if (!error)
                    {
                        ReceiveFrame(DOWN);
                        ReceiveFrame(DOWN);
                    }

                    _serialSendBuff[0] = 0xdd;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);


                    bool ldr_flag = false;
                    if (_deviceFileName == CommonConstants.UPLOAD_LADDER_FILENAME)
                    {
                        CommonConstants.LADDER_PRESENT = true;
                        ldr_flag = true;
                    }

                    _deviceFileName = GetUploadFileID(_serialFileID);

                    if (!ldr_flag && _deviceFileName == CommonConstants.UPLOAD_LADDER_FILENAME)
                        _deviceFileName = null;

                    if (_deviceFileName == null || _deviceFileName == CommonConstants.BinaryFile)
                        break;

                    if (_deviceFileName == null || error)
                        break;

                    if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                    {
                        if ((CommonConstants.IsProductSupportsDataLogger(iProductID)) == false)
                        {
                            CommonConstants.communicationStatus = 0;
                            //_deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strDataLogNotSupported));
                            fs.Close();
                            fs = null;
                            Close();
                            return;
                        }
                    }

                    _serialSendBuff[0] = _deviceFileID;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);

                    _serialSendBuff = new byte[1024 * 8 + 1 + DATA_START_INDEX + 4];

                    fs.Close();
                }
                Thread.Sleep(1000);
                if (true)
                {
                    _serialSendBuff[0] = 0x55;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
                else
                {
                    _serialSendBuff[0] = 0xdd;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
                fs.Close();
            }
            catch (Exception e)
            {
                error = true;
                CommonConstants.communicationType = 0;
                //_deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
            }
            Close();
            return;
        }

        #region FP_Ethernet_Implementation-AMIT
        private void InitiateCommunicationEtherSettings()
        {
            int message = 256;
            if (_serialReceivedByte == _deviceFileID || _serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_1 || _serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2)
            {	//	If Prizm Ready, Send here setFrame
                _serialTimeOut = 0;

                a: if (_serialReceivedByte == _deviceFileID + 2)
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);
                    _serialPort.Write(_serialSendBuff, 0, 4);
                    //Thread.Sleep(1000);
                    //_serialPort.Write(_serialSendBuff, 0, STARTFRAMESIZE);
                    _serialATimer = new System.Timers.Timer(THRTEE_SEC_DELAY); // KA
                    _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    _serialATimer.Enabled = true;
                    ReceiveFrame();

                    if (_serialRecvBuff[0] == 0)
                    {
                        _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY * 3);
                        _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                        _serialATimer.Enabled = true;
                        ReceiveFrame();
                    }
                    if (_serialReceivedByte == _deviceFileID + 2)
                        goto a;
                }

                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                {
                    message <<= 7;

                    GetSetFrameIntoBuffForEthSettings(DOWN);//New Code
                    _serialPort.Write(_serialSendBuff, 0, STARTFRAMESIZE);
                    Thread.Sleep(200);
                    ReceiveFrame();

                    if (_serialReceivedByte == PRODUCT_MISSMATCH)	//	if '1' received = OK received
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strProductMismatch));
                        Close();
                        return;
                    }

                    _deviceDownloadStatus(3 + message);

                    if (_serialRecvBuff[0] == 0)
                    {
                        _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY * 3);
                        _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                        _serialATimer.Enabled = true;
                        ReceiveFrame();
                    }
                    else if (_serialRecvBuff[0] == 0x1)
                    {
                        GetEtherSettingFrame(false);
                        //_serialTotalCrcByte = _serialBCC;
                        Thread.Sleep(200);
                        _serialPort.Write(_serialSendBuff, 0, 0x16);
                        //btarr[0] = CalculateBCC(18, 0);
                        //_serialPort.Write(btarr, 0, btarr.Length);
                        Thread.Sleep(200);

                        _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                        _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                        _serialATimer.Enabled = true;
                    }
                }
                else
                {
                    GetSetFrameIntoBuff(DOWN);
                    _serialPort.Write(_serialSendBuff, 0, STARTFRAMESIZE);
                }
                _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY * 3);
                _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                _serialATimer.Enabled = true;
                ReceiveFrame();
                if (_serialReceivedByte == 0)
                    ReceiveFrame();
                _serialATimer.Enabled = false;

                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                {
                    if (_serialReceivedByte == 168 || _serialReceivedByte == 0x01)
                    {
                        //FinalizeDownloading();
                        _deviceDownloadPercentage(100);
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));

                        Close();
                        return;
                    }
                }
                if (_serialReceivedByte != CORRECT_CRC)	//	if '1' received = OK received
                    if (_serialReceivedByte != 0xef)
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        //_deviceDownloadStatus(6);
                        Close();
                        return;
                    }
                if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                {

                    Thread.Sleep(1000);
                    _serialSendBuff[0] = 0xB8;
                    _serialPort.Write(_serialSendBuff, 0, 1);
                    _serialSendBuff.SetValue((Byte)0x60, 0);
                    _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                    _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    _serialATimer.Enabled = true;
                    //ReceiveFrame();
                    ReceiveFrameTEN();
                    _serialATimer.Enabled = false;
                    _serialSendBuff[0] = 0x38;
                    _serialSendBuff[1] = 0x0;
                    _serialSendBuff[2] = 0x0;
                }
            }
            //Ladder_change_R11
            else if (_serialReceivedByte == _deviceFileID + 3)
            {
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                Close();
                return;
            }
            //End
            else // Unit Not Ready
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(8);
                Close();
                return;
            }
            Close();

            X:

            int error = CommonConstants.communicationStatus; //Issue_718
            //int message = 256;
            if (_deviceFileID == CommonConstants.SERIAL_LADDER_DNLD_FILEID)
                message <<= 2;
            if (_deviceFileID == CommonConstants.SERIAL_FONT_DNLD_FILEID)
                message <<= 3;
            if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                message <<= 1;
            if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                message <<= 6;
            else
                message <<= 8;
            _deviceDownloadStatus(3 + message);

            CommonConstants.communicationStatus = error; //Issue_718
            return;
        }
        #endregion

        #region GSM_Sanjay
        //FP_CODE Sanjay Product Information(20-7-2011)GSM
        public int Plc_Download_Protocol(ref byte[] s_ProductInfo)
        {
            byte[] _serialSendbyte = new byte[4];
            byte[] _serialRecvbyte = new byte[64];
            byte initialByte = 0x99;
            byte temp_receivedbyte = 0;

            _serialSendbyte[0] = initialByte;

            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
            if (_deviceIsConnected == false)
            {
                Close();
                return FAILURE;
            }

            bool valid_byte = false;
            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
            {
                _serialPort.DiscardInBuffer();
                Thread.Sleep(10);
                //issue_141_V2.2_sammed
                try
                {
                    _serialPort.Write(_serialSendbyte, 0, 1);
                }
                catch (Exception ex)
                {
                    Close();
                    return FAILURE;
                }
                //End
                if (ReceiveFrame())
                {
                    if (_serialReceivedByte == (initialByte + TOLERANCE_BYTE_1) || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_2)
                            || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_3) || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_4))
                    {
                        valid_byte = true;
                        break;
                    }
                    if (_serialNoOfByteRead > 0)
                    {
                        _serialPort.DiscardInBuffer();
                        Thread.Sleep(SEVEN_SEC_DELAY);
                    }
                }
            }

            if (!valid_byte)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                Close();
                return FAILURE;
            }

            temp_receivedbyte = _serialReceivedByte;

            _serialSendbyte.SetValue((Byte)0x52, 0);
            _serialSendbyte.SetValue((Byte)0x45, 1);
            _serialSendbyte.SetValue((Byte)0x50, 2);
            _serialSendbyte.SetValue((Byte)0x4c, 3);

            _serialPort.Write(_serialSendbyte, 0, 4);
            Thread.Sleep(1000);
            _serialPort.ReadTimeout = TEN_SEC_DELAY;

            while (true)
            {
                MemoryStream ms = new MemoryStream();
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;

                try
                {
                    _serialNoOfByteRead = _serialPort.Read(_serialRecvbyte, 0, 64);
                    ms.Write(_serialRecvbyte, 0, _serialRecvbyte.Length);

                    int NoOfFrames = _serialRecvbyte[60];

                    for (int i = 0; i < NoOfFrames; i++)
                    {
                        byte tempinitialByte = 0x71;
                        byte nextChunkByte = 0x01;

                        if (i == 0)
                            _serialSendbyte[0] = tempinitialByte;
                        else
                            _serialSendbyte[0] = nextChunkByte;

                        _serialPort.Write(_serialSendbyte, 0, 1);
                        Thread.Sleep(1000);
                        _serialNoOfByteRead = _serialPort.Read(_serialRecvbyte, 0, 64);
                        ms.Write(_serialRecvbyte, 0, _serialRecvbyte.Length);
                    }
                    s_ProductInfo = ms.ToArray();
                    Close();
                }
                catch (System.TimeoutException timeOut)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }

                if (_serialNoOfByteRead > 0)
                {
                    //s_ProductInfo = _serialRecvbyte;
                    return SUCCESS;
                }
            }
            return FAILURE;
        }
        #endregion GSM_Sanjay

        #region ShitalG
        public int Plc_Download_ProtocolInitial(string pFileName, byte pFileID, byte NodeAddress)
        {
            LogWriter.WriteToFile("Serial.cs - Plc_Download_ProtocolInitial()", "started--", "RTC_Upgrade");
            byte[] _serialSendbyte = new byte[2];
            byte temp_receivedbyte = 0;
            byte initialByte = 0x70;
            bool valid_byte = false;

            _serialSendbyte[0] = initialByte;
            _serialSendbyte[1] = NodeAddress;

            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
            if (_deviceIsConnected == false)
            {
                Close();
                return FAILURE;
            }

            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
            {
                _serialPort.DiscardInBuffer();
                Thread.Sleep(10);
                try
                {
                    _serialPort.Write(_serialSendbyte, 0, 2);
                    LogWriter.WriteToFile(" 1)   => ", string.Join(",", _serialSendbyte), "RTC_Upgrade");
                    if (ReceiveFrame())
                    {
                        LogWriter.WriteToFile(" 1)   <= ", string.Join(",", _serialReceivedByte), "RTC_Upgrade");
                        if (_serialReceivedByte == (initialByte + TOLERANCE_BYTE_1) || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_2)
                            || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_3))
                        {
                            valid_byte = true;
                            break;
                        }
                        if (_serialNoOfByteRead > 0)
                        {
                            _serialPort.DiscardInBuffer();
                            Thread.Sleep(SEVEN_SEC_DELAY);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }
            }

            if (!valid_byte)
            {
                LogWriter.WriteToFile("Serial.cs-Plc_Download_ProtocolInitial()", "invalid  byte", "RTC_Upgrade");
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                Close();
                return FAILURE;
            }

            temp_receivedbyte = _serialReceivedByte;

            while (true)
            {
                if (_serialNoOfByteRead > 0)
                {
                    if (temp_receivedbyte == (initialByte + TOLERANCE_BYTE_1) || temp_receivedbyte == (initialByte + TOLARANCE_BYTE_2))
                    {
                        return ONLYBOOTBLOCK;
                    }
                    else if (temp_receivedbyte == (initialByte + TOLARANCE_BYTE_3))
                    {
                        try
                        {
                            byte[] _serialSendBuff = new byte[4];
                            _serialSendBuff.SetValue((Byte)0x52, 0);
                            _serialSendBuff.SetValue((Byte)0x45, 1);
                            _serialSendBuff.SetValue((Byte)0x50, 2);
                            _serialSendBuff.SetValue((Byte)0x4c, 3);
                            _serialRecvBuff[0] = 0;
                            _serialNoOfByteRead = _serialPort.Read(_serialRecvBuff, 0, 1);//Read ack byte 1
                            if (_serialRecvBuff[0] == 1)
                                return SUCCESS;
                            else
                            {
                                CommonConstants.communicationStatus = 0;
                                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                                Close();
                                return FAILURE;
                            }
                        }
                        catch (System.TimeoutException timeOut)
                        {
                            _serialRecvBuff[0] = 0;
                            return FAILURE;
                        }
                    }
                    else
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }
                }
            }
            return FAILURE;
        }
        #endregion


        #region Keeran (KA)
        //[20]=>.........01<=
        //B8=>...........B8<=
        public int SendFile_Update1(string pFileName, byte pFileID, byte nodeAddress)
        {
            byte[] _serialSendBuff1 = new byte[20];
            byte[] _serialSendByte1 = new byte[2];
            byte[] _serialReceiveByte1 = new byte[2];
            byte temp_receivedbyte = 0;
            bool valid_byte = false;
            FileInfo info = new FileInfo(pFileName);
            try
            {
                long fsize = info.Length;
                byte[] a = CommonConstants.RawSerialize(fsize);
                //LogWriter.WriteToFile("Size: ", fsize.ToString() +  " " + a.Length + " " + string.Join(",",a), "RTC_Upgrade");                
                bool base1 = true;
                _serialSendBuff1[0] = (byte)_serialSendBuff1.Length;
                _serialSendBuff1[1] = nodeAddress;
                _serialSendBuff1[2] = 0x00;//(byte)nodeAddress;
                _serialSendBuff1[3] = base1 ? (byte)01 : (byte)02;////(byte)nodeAddress;
                _serialSendBuff1[10] = a[0];
                _serialSendBuff1[11] = a[1];
                _serialSendBuff1[12] = a[2];
                _serialSendBuff1[13] = a[3];
                LogWriter.WriteToFile(" 20[] frame   => ", string.Join(",", _serialSendBuff1), "RTC_Upgrade");
                for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
                {
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(10);
                    try
                    {
                        _serialPort.Write(_serialSendBuff1, 0, _serialSendBuff1.Length);
                        Thread.Sleep(1000);
                        _serialPort.ReadTimeout = TEN_SEC_DELAY;
                        if (ReceiveFrame())
                        {
                            LogWriter.WriteToFile(" 3)   01<= ", string.Join(",", _serialReceivedByte), "RTC_Upgrade");
                            if (_serialReceivedByte == 0x01)
                            {
                                valid_byte = true;
                                break;
                            }
                            if (_serialNoOfByteRead > 0)
                            {
                                _serialPort.DiscardInBuffer();
                                Thread.Sleep(SEVEN_SEC_DELAY);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                        return FAILURE;
                    }
                }

                if (!valid_byte)
                {
                    LogWriter.WriteToFile("Serial.cs-SendFile_Update1()", "invalid  byte", "RTC_Upgrade");
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }

                _serialSendByte1.SetValue((Byte)0xB8, 0);

                LogWriter.WriteToFile(" 4)   B8=> ", string.Join(",", _serialSendByte1), "RTC_Upgrade");
                _serialReceivedByte = 0;
                for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
                {
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(10);
                    try
                    {
                        _serialPort.Write(_serialSendByte1, 0, _serialSendByte1.Length);
                        Thread.Sleep(1000);
                        _serialPort.ReadTimeout = TEN_SEC_DELAY;

                        if (ReceiveFrame())
                        {
                            LogWriter.WriteToFile(" 4)   B8<= ", string.Join(",", _serialReceivedByte), "RTC_Upgrade");
                            if (_serialReceivedByte == 0xB8)
                            {
                                valid_byte = true;
                                break;
                            }
                            if (_serialNoOfByteRead > 0)
                            {
                                _serialPort.DiscardInBuffer();
                                Thread.Sleep(SEVEN_SEC_DELAY);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                        return FAILURE;
                    }
                }

                if (!valid_byte)
                {
                    LogWriter.WriteToFile("Serial.cs-SendFile_Update1()", "invalid  byte", "RTC_Upgrade");
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }
                else
                {
                    _serialSendBuff = File.ReadAllBytes(pFileName);
                    LogWriter.WriteToFile("Serial.cs-SendFile_Update1() ", "file size: " + _serialSendBuff.Length.ToString(), "RTC_Upgrade");

                    if (_serialSendBuff.Length > 0)
                    {
                        int fullFrames = 0; int partialFrameSize = 0;
                        partialFrameSize = (_serialSendBuff.Length % DATAFRAMESIZE11);

                        fullFrames = Convert.ToInt32(Math.Floor(Convert.ToDouble(_serialSendBuff.Length) / DATAFRAMESIZE11));

                        int totalFrames = fullFrames + (partialFrameSize > 0 ? 1 : 0);
                        int frameNo = 1;
                        bool AllFramesSentSuccessfully = false;
                        LogWriter.WriteToFile("Serial.cs-GetSetFrameIntoBuff_Update5()", "total frames: " + totalFrames + " partial : " + partialFrameSize + " full frames: " + fullFrames, "RTC_Upgrade");
                        int index = 0;


                        const int chunkSize = 256; // read the file by chunks of 1KB
                        using (var file = info.OpenRead())
                        {
                            int bytesRead;
                            int res = 0;
                            var buffer = new byte[chunkSize];
                            while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                if (frameNo < totalFrames)
                                {
                                    res = GetSetFrameIntoBuff_Update5(buffer, frameNo, buffer.Length);
                                }

                                if (frameNo == totalFrames)
                                {
                                    LogWriter.WriteToFile("Serial.cs-SendFile_Update1()", "last frame", "RTC_Upgrade");
                                    res = GetSetFrameIntoBuff_Update5(buffer, frameNo, partialFrameSize);
                                    AllFramesSentSuccessfully = true;
                                }

                                LogWriter.WriteToFile("Serial.cs-SendFile_Update1()", "frameNo: " + frameNo + " buffer len: " + buffer.Length, "RTC_Upgrade");
                                if (res == ClassList.CommonConstants.SUCCESS)
                                {
                                    frameNo++;
                                }
                                else
                                {
                                    LogWriter.WriteToFile("Serial.cs-SendFile_Update1()", "send frames break", "RTC_Upgrade");
                                    break;
                                }
                            }
                        }





                        LogWriter.WriteToFile("Serial.cs-SendFile_Update1()", "AllFramesSentSuccessfully " + AllFramesSentSuccessfully.ToString(), "RTC_Upgrade");
                        return AllFramesSentSuccessfully ? SUCCESS : FAILURE;
                    }
                    return FAILURE;
                }

            }

            catch (Exception ex)
            {
                LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                return FAILURE;
            }
        }

        //70=>...........71/73<=
        //REPL=>.........[64]
        //Need to change 7 bit protocal
        public int Plc_Download_ProtocolInitial_Update1(string pFileName, byte pFileID, byte NodeAddress, int commType, int baseType)
        {

            try
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_ProtocolInitial_Update1()", "started--", "RTC_Upgrade");
                byte[] _serialSendbyte = null;

                byte[] _serialSendbyte1 = new byte[4];
                byte[] _serialRecvbyte = new byte[64];

                byte temp_receivedbyte = 0;
                byte initialByte = 0x70;
                bool valid_byte = false;
                //Bhushan Need to change
                if (commType == 2)  //commType    : 1 Serial 2: USB  
                {
                    _serialSendbyte = new byte[7];

                    _serialSendbyte[0] = (byte)'D';
                    _serialSendbyte[1] = (byte)'W';
                    _serialSendbyte[2] = (byte)'N';
                    _serialSendbyte[3] = (byte)'L';
                    _serialSendbyte[4] = initialByte;
                    _serialSendbyte[5] = NodeAddress;
                    _serialSendbyte[6] = (byte)CommonConstants.SetFileType;
                }
                else if (commType == 1)  //commType    : 1 Serial 2: USB  
                {
                  //  Working Data
                    _serialSendbyte = new byte[3];

                    _serialSendbyte[0] = initialByte;
                    _serialSendbyte[1] = NodeAddress;
                    _serialSendbyte[2] = (byte)CommonConstants.SetFileType;

                    #region Nodedata Split into Nibble Ritesh_Change
                    //Add Node data Into 2 Nibble

                    byte nibble1 = (byte)(NodeAddress & 0x0F);
                    byte nibble2 = (byte)((NodeAddress & 0xF0) >> 4);
                    _serialSendbyte = new byte[4];

                    _serialSendbyte[0] = initialByte;
                    _serialSendbyte[1] = nibble1;
                    _serialSendbyte[2] = nibble2;
                    _serialSendbyte[3] = (byte)CommonConstants.SetFileType;
                    #endregion

        }

        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
                if (_deviceIsConnected == false)
                {
                    Close();
                    return FAILURE;
                }

                for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
                {
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(10);
                    try
                    {
                        _serialPort.Write(_serialSendbyte, 0, _serialSendbyte.Length);
                        LogWriter.WriteToFile(" 1)   70=> ", string.Join(",", _serialSendbyte), "RTC_Upgrade");
                        //Thread.Sleep(1000);    KA 0920   
                        _serialPort.ReadTimeout = TEN_SEC_DELAY;
                        if (ReceiveFrame())
                        {
                            LogWriter.WriteToFile(" 1)   71/73<= ", string.Join(",", _serialReceivedByte), "RTC_Upgrade");
                            if (_serialReceivedByte == (initialByte + TOLERANCE_BYTE_1) || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_3) ||
                                _serialReceivedByte == 0x75 || _serialReceivedByte == PRODUCT_MISSMATCH) // 0x75 : node address not present in n/w
                            {
                                valid_byte = true;
                                break;
                            }
                            if (_serialNoOfByteRead > 0)
                            {
                                _serialPort.DiscardInBuffer();
                                Thread.Sleep(SEVEN_SEC_DELAY);
                            }
                            else
                            {
                                _deviceDownloadStatus(113);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }
                }

                if (!valid_byte)
                {
                    LogWriter.WriteToFile("Serial.cs-Plc_Download_ProtocolInitial_Update1()", "invalid  byte", "RTC_Upgrade");
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(114); //node address not available
                    Close();
                    return FAILURE;
                }

                if (_serialReceivedByte == 0x75)
                {
                    _deviceDownloadStatus(114);
                    Close();
                    return FAILURE;
                }
                if (_serialReceivedByte == PRODUCT_MISSMATCH)
                {
                    _deviceDownloadStatus(111);
                    Close();
                    return FAILURE;
                }


                  #region Move Setting 7 to 8 Bit Ritesh_Change
                ////As per 7 bit Download Move Setting to 8 bit
                if (ConnectForMoveEBit() == ClassList.CommonConstants.SUCCESS)
                #endregion


                  temp_receivedbyte = _serialReceivedByte;

                _serialSendbyte1.SetValue((Byte)0x52, 0);
                _serialSendbyte1.SetValue((Byte)0x45, 1);
                _serialSendbyte1.SetValue((Byte)0x50, 2);
                _serialSendbyte1.SetValue((Byte)0x4c, 3);

                LogWriter.WriteToFile(" 2)   REPL=> ", string.Join(",", _serialSendbyte1), "RTC_Upgrade");

                _serialPort.Write(_serialSendbyte1, 0, 4);
                Thread.Sleep(1000);
                _serialPort.ReadTimeout = TEN_SEC_DELAY;

                while (true)
                {
                    _serialRecvBuff[0] = 0;

                    try
                    {
                        //Need To Check While Module
                        _serialNoOfByteRead = _serialPort.Read(_serialRecvbyte, 0, 64);
                        LogWriter.WriteToFile(" 2)   <= ", string.Join(",", _serialRecvbyte), "RTC_Upgrade");
                    }
                    catch (System.TimeoutException timeOut)
                    {
                        LogWriter.WriteToFile(timeOut.StackTrace, timeOut.Message, "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }
                    catch (Exception er)
                    {
                        LogWriter.WriteToFile(er.StackTrace, er.Message, "RTC_Upgrade");
                    }

                    if (_serialNoOfByteRead > 0)
                    {
                        if (temp_receivedbyte == (initialByte + TOLERANCE_BYTE_1))
                        {
                            LogWriter.WriteToFile("Serial.cs - Plc_Download_ProtocolInitial_Update1()", " 64 <= bootblock", "RTC_Upgrade");
                            return ONLYBOOTBLOCK;
                        }
                        else if (temp_receivedbyte == (initialByte + TOLARANCE_BYTE_3))
                        {
                            LogWriter.WriteToFile("Serial.cs - Plc_Download_ProtocolInitial_Update1()", " 64 <= run", "RTC_Upgrade");
                            return SUCCESS;
                        }
                    }
                    else
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }
                }
                return FAILURE;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_ProtocolInitial_Update1()", ex.Message, "RTC_Upgrade");
                return FAILURE;
            }
        }

        //99=>..............9A/9B<=
        public int Plc_Download_Protocol_Update1(string pFileName, byte pFileID)
        {
            try
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update1()", "started--", "RTC_Upgrade");
                byte[] _serialSendbyte = new byte[2];
                byte[] _serialRecvbyte = new byte[2];
                byte initialByte = 0x99;
                byte temp_receivedbyte = 0;

                _serialSendbyte[0] = initialByte;

                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
                if (_deviceIsConnected == false)
                {
                    Close();
                    return FAILURE;
                }

                bool valid_byte = false;

                for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
                {
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(10);

                    try
                    {
                        LogWriter.WriteToFile(" 1)   99=> ", string.Join(",", _serialSendbyte), "RTC_Upgrade");
                        _serialPort.Write(_serialSendbyte, 0, 1);
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }

                    if (ReceiveFrame())
                    {
                        if (_serialReceivedByte == (initialByte + TOLERANCE_BYTE_1) ||
                            _serialReceivedByte == (initialByte + TOLARANCE_BYTE_2))
                        {
                            LogWriter.WriteToFile(" 1)   9A/9B<= ", string.Join(",", _serialReceivedByte), "RTC_Upgrade");
                            valid_byte = true;
                            break;
                        }
                        if (_serialNoOfByteRead > 0)
                        {
                            _serialPort.DiscardInBuffer();
                            Thread.Sleep(SEVEN_SEC_DELAY);
                        }
                    }
                }

                if (!valid_byte)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }

                temp_receivedbyte = _serialReceivedByte;

                while (true)
                {
                    _serialRecvBuff[0] = 0;

                    if ((_serialNoOfByteRead > 0) && (temp_receivedbyte == (initialByte + TOLERANCE_BYTE_1) ||
                        temp_receivedbyte == (initialByte + TOLARANCE_BYTE_2)))
                    {
                        LogWriter.WriteToFile(" 2)   9A/9B<= ", "SUCCESS " + temp_receivedbyte, "RTC_Upgrade");
                        return (temp_receivedbyte == 0x9A) ? ONLYBOOTBLOCK : SUCCESS; //Keeran (KA)
                    }
                    else
                    {
                        LogWriter.WriteToFile(" 2)   9A/9B<= ", "FAILURE", "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }
                }
                return FAILURE;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update1()", ex.Message, "RTC_Upgrade");
                //return FAILURE;
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                Close();
                return FAILURE;
            }
        }

        //D0=>..............D2<=
        public int Plc_Download_Protocol_Update2(string pFileName, byte pFileID)
        {
            try
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update2()", "started--", "RTC_Upgrade");
                byte[] _serialSendbyte = new byte[2];
                byte[] _serialRecvbyte = new byte[2];
                byte initialByte = 0xD0;
                byte temp_receivedbyte = 0;

                _serialSendbyte[0] = initialByte;

                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
                if (_deviceIsConnected == false)
                {
                    Close();
                    return FAILURE;
                }

                bool valid_byte = false;

                for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
                {
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(10);

                    try
                    {
                        LogWriter.WriteToFile(" 1)   D0=> ", string.Join(",", _serialSendbyte), "RTC_Upgrade");
                        _serialPort.Write(_serialSendbyte, 0, 1);
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }

                    if (ReceiveFrame())
                    {
                        if (_serialReceivedByte == (initialByte + TOLERANCE_BYTE_1) ||
                            _serialReceivedByte == (initialByte + TOLARANCE_BYTE_2))
                        {
                            LogWriter.WriteToFile(" 1)   D2<= ", string.Join(",", _serialReceivedByte), "RTC_Upgrade");
                            valid_byte = true;
                            break;
                        }
                        if (_serialNoOfByteRead > 0)
                        {
                            _serialPort.DiscardInBuffer();
                            Thread.Sleep(SEVEN_SEC_DELAY);
                        }
                    }
                }

                if (!valid_byte)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }

                temp_receivedbyte = _serialReceivedByte;

                while (true)
                {
                    _serialRecvBuff[0] = 0;

                    if ((_serialNoOfByteRead > 0) && (temp_receivedbyte == (initialByte + TOLARANCE_BYTE_2)))
                    {
                        LogWriter.WriteToFile(" 2)   D2<= ", "SUCCESS " + temp_receivedbyte, "RTC_Upgrade");
                        return SUCCESS; //Keeran (KA)
                    }
                    else
                    {
                        LogWriter.WriteToFile(" 2)   D2<= ", "FAILURE " + temp_receivedbyte, "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }
                }
                return FAILURE;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update2()", ex.Message, "RTC_Upgrade");
               // return FAILURE;
              
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                Close();
                return FAILURE;
            }
        }

        //REPL=>..............01<=
        public int Plc_Download_Protocol_Update3(string pFileName, byte pFileID)
        {
            try
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update3()", "started--", "RTC_Upgrade");

                byte[] _serialSendbyte = new byte[4];
                byte[] _serialRecvbyte = new byte[2];
                byte initialByte = 0xD0;
                byte temp_receivedbyte = 0;

                _serialSendbyte.SetValue((Byte)0x52, 0);
                _serialSendbyte.SetValue((Byte)0x45, 1);
                _serialSendbyte.SetValue((Byte)0x50, 2);
                _serialSendbyte.SetValue((Byte)0x4c, 3);

                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
                if (_deviceIsConnected == false)
                {
                    Close();
                    return FAILURE;
                }

                bool valid_byte = false;

                for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
                {
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(10);

                    try
                    {
                        LogWriter.WriteToFile(" 2)   REPL=> ", string.Join(",", _serialSendbyte), "RTC_Upgrade");

                        _serialPort.Write(_serialSendbyte, 0, 4);
                        Thread.Sleep(1000);
                        _serialPort.ReadTimeout = TEN_SEC_DELAY;
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }

                    if (ReceiveFrame())
                    {
                        if (_serialReceivedByte == 0x01)
                        {
                            LogWriter.WriteToFile(" 1)   01<= ", string.Join(",", _serialReceivedByte), "RTC_Upgrade");
                            valid_byte = true;
                            break;
                        }
                        if (_serialNoOfByteRead > 0)
                        {
                            _serialPort.DiscardInBuffer();
                            Thread.Sleep(SEVEN_SEC_DELAY);
                        }
                    }
                }

                if (!valid_byte)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }

                temp_receivedbyte = _serialReceivedByte;

                while (true)
                {
                    _serialRecvBuff[0] = 0;

                    if ((_serialNoOfByteRead > 0) && (temp_receivedbyte == 0x01))
                    {
                        LogWriter.WriteToFile(" 2)   01<= ", "SUCCESS " + temp_receivedbyte, "RTC_Upgrade");
                        return SUCCESS; //Keeran (KA)
                    }
                    else
                    {
                        LogWriter.WriteToFile(" 2)   01<= ", "FAILURE " + temp_receivedbyte, "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }
                }
                return FAILURE;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update3()", ex.Message, "RTC_Upgrade");
                return FAILURE;
            }
        }

        public static string[] GetUsbSerDevices()
        {
            try
            {
                // HKLM\SYSTEM\CurrentControlSet\services\usbser\Enum -> Device IDs of what is plugged in
                // HKLM\SYSTEM\CurrentControlSet\Enum\{Device_ID}\Device Parameters\PortName -> COM port name.

                List<string> ports = new List<string>();

                RegistryKey k1 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\usbser\Enum");

                if (k1 == null)
                {
                    //Debug.Fail("Unable to open Enum key");
                    MessageBox.Show("No USB devices found!!");
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
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Serial.cs - GetUsbSerDevices()", ex.Message, "RTC_Upgrade");
                return null;
            }
        }

        //REPL=>..............9A<=
        public int Plc_Download_Protocol_Update4(string pFileName, byte pFileID)
        {
            try
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update4()", "started--", "RTC_Upgrade");

                byte[] _serialSendbyte = new byte[4];
                byte[] _serialRecvbyte = new byte[2];
                byte initialByte = 0xD0;
                byte temp_receivedbyte = 0;

                _serialSendbyte.SetValue((Byte)0x52, 0);
                _serialSendbyte.SetValue((Byte)0x45, 1);
                _serialSendbyte.SetValue((Byte)0x50, 2);
                _serialSendbyte.SetValue((Byte)0x4c, 3);

                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
                if (_deviceIsConnected == false)
                {
                    Close();
                    return FAILURE;
                }

                bool valid_byte = false;

                for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
                {
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(10);

                    try
                    {
                        LogWriter.WriteToFile(" 2)   REPL=> ", string.Join(",", _serialSendbyte), "RTC_Upgrade");

                        try
                        {
                            WriteUpdate4(_serialSendbyte);
                        }
                        catch (Exception ex)
                        {
                            string[] list = GetUsbSerDevices();

                            if (list != null && list.Length > 0)
                            {
                                try
                                {
                                    _serialPort.DtrEnable = true;
                                    _serialPort.RtsEnable = true;

                                    _serialPort = new SerialPort(list[0].ToString(), 9600);
                                    _serialPort.Open();
                                    MessageBox.Show(list[0].ToString() + " Open = " + _serialPort.IsOpen.ToString());

                                    _serialPort.DtrEnable = true;
                                    _serialPort.RtsEnable = true;
                                }
                                catch (Exception et)
                                {
                                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                                    Close();
                                    return FAILURE;
                                }
                              
                                if (_serialPort != null && _serialPort.IsOpen)
                                {
                                    WriteUpdate4(_serialSendbyte);
                                }
                                else
                                {
                                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                                    Close();
                                    return FAILURE;
                                }
                            }
                            else
                            {
                                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                                Close();
                                return FAILURE;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }

                    

                    try
                    {
                        LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update4()", "USB Working Start...", "RTC_Upgrade");
                        if (_serialPort != null && _serialPort.IsOpen)
                        {
                            _serialPort.Close();
                            _serialPort = null;
                            //GetSerialConection(pProductID, pComNoOrIpAddress, _setupFrameMode);
                            LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update4()", "USB Working...", "RTC_Upgrade");
                        }
                    }
                    catch (Exception ae)
                    {
                        if (_serialPort.IsOpen)
                        {
                            _serialPort.Close();
                            _serialPort = null;
                            LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update4()", "USB Working...", "RTC_Upgrade");
                        }
                    }
                    finally {
                        if (_serialPort != null && _serialPort.IsOpen)
                        {
                            _serialPort.Close();
                            _serialPort = null;
                        }
                    }
                    Thread.Sleep(2000);
                    if (Connect() == ClassList.CommonConstants.SUCCESS)
                    {
                        System.Threading.Thread.Sleep(20);
                        if (ReceiveFrame())
                        {
                            if (_serialReceivedByte == 0x9A)
                            {
                                LogWriter.WriteToFile(" 1)   9A<= ", string.Join(",", _serialReceivedByte), "RTC_Upgrade");
                                valid_byte = true;
                                break;
                            }
                            if (_serialNoOfByteRead > 0)
                            {
                                _serialPort.DiscardInBuffer();
                                Thread.Sleep(SEVEN_SEC_DELAY);
                            }
                        }
                    }
                }

                if (!valid_byte)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }

                temp_receivedbyte = _serialReceivedByte;

                while (true)
                {
                    _serialRecvBuff[0] = 0;

                    if ((_serialNoOfByteRead > 0) && (temp_receivedbyte == 0x9A))
                    {
                        LogWriter.WriteToFile(" 2)   9A<= ", "SUCCESS " + temp_receivedbyte, "RTC_Upgrade");
                        return SUCCESS; //Keeran (KA)
                    }
                    else
                    {
                        LogWriter.WriteToFile(" 2)   9A<= ", "FAILURE " + temp_receivedbyte, "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }
                }
                return FAILURE;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update4()", ex.Message, "RTC_Upgrade");
                return FAILURE;
            }
        }


        //Serial 
        //REPL=>..............9A<=
        public int Plc_Download_Protocol_Update44(string pFileName, byte pFileID)
        {
            try
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update44()", "started--", "RTC_Upgrade");

                byte[] _serialSendbyte = new byte[4];
                byte[] _serialRecvbyte = new byte[2];
                byte initialByte = 0xD0;
                byte temp_receivedbyte = 0;

                _serialSendbyte.SetValue((Byte)0x52, 0);
                _serialSendbyte.SetValue((Byte)0x45, 1);
                _serialSendbyte.SetValue((Byte)0x50, 2);
                _serialSendbyte.SetValue((Byte)0x4c, 3);

                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
                if (_deviceIsConnected == false)
                {
                    Close();
                    return FAILURE;
                }

                bool valid_byte = false;

                for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
                {
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(10);

                    try
                    {
                        LogWriter.WriteToFile(" 2)   REPL=> ", string.Join(",", _serialSendbyte), "RTC_Upgrade");

                        try
                        {
                            WriteUpdate4(_serialSendbyte);
                        }
                        catch (Exception ex)
                        {
                            string[] list = GetUsbSerDevices();

                            if (list != null && list.Length > 0)
                            {
                                try
                                {
                                    _serialPort.DtrEnable = true;
                                    _serialPort.RtsEnable = true;

                                    _serialPort = new SerialPort(list[0].ToString(), 9600);
                                    _serialPort.Open();
                                    MessageBox.Show(list[0].ToString() + " Open = " + _serialPort.IsOpen.ToString());

                                    _serialPort.DtrEnable = true;
                                    _serialPort.RtsEnable = true;
                                }
                                catch (Exception et)
                                {
                                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                                    Close();
                                    return FAILURE;
                                }

                                if (_serialPort != null && _serialPort.IsOpen)
                                {
                                    WriteUpdate4(_serialSendbyte);
                                }
                                else
                                {
                                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                                    Close();
                                    return FAILURE;
                                }
                            }
                            else
                            {
                                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                                Close();
                                return FAILURE;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }



                  //  if (Connect() == ClassList.CommonConstants.SUCCESS)
                    {
                        System.Threading.Thread.Sleep(20);
                        if (ReceiveFrame())
                        {
                            if (_serialReceivedByte == 0x9A)
                            {
                                LogWriter.WriteToFile(" 1)   9A<= ", string.Join(",", _serialReceivedByte), "RTC_Upgrade");
                                valid_byte = true;
                                break;
                            }
                            if (_serialNoOfByteRead > 0)
                            {
                                _serialPort.DiscardInBuffer();
                                Thread.Sleep(SEVEN_SEC_DELAY);
                            }
                        }
                    }
                }

                if (!valid_byte)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }

                temp_receivedbyte = _serialReceivedByte;

                while (true)
                {
                    _serialRecvBuff[0] = 0;

                    if ((_serialNoOfByteRead > 0) && (temp_receivedbyte == 0x9A))
                    {
                        LogWriter.WriteToFile(" 2)   9A<= ", "SUCCESS " + temp_receivedbyte, "RTC_Upgrade");
                        return SUCCESS; //Keeran (KA)
                    }
                    else
                    {
                        LogWriter.WriteToFile(" 2)   9A<= ", "FAILURE " + temp_receivedbyte, "RTC_Upgrade");
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }
                }
                return FAILURE;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Serial.cs - Plc_Download_Protocol_Update44()", ex.Message, "RTC_Upgrade");
                return FAILURE;
            }
        }

        private void WriteUpdate4(byte[] _serialSendbyte)
        {
            _serialPort.Write(_serialSendbyte, 0, 4);

            _serialPort.ReadTimeout = 50000;// TEN_SEC_DELAY;
        }

        protected int GetSetFrameIntoBuff_Update5(byte[] _serialSendBuff, int frameNo, int frameSize)
        {
            LogWriter.WriteToFile("Serial.cs-GetSetFrameIntoBuff_Update5()", "started", "RTC_Upgrade");

            byte[] sendframe = new byte[260];

            try
            {
                byte crc = CalculateCRCs(frameSize, _serialSendBuff);

                for (int i = 0; i < _serialSendBuff.Length; i++)
                    sendframe[i + 3] = _serialSendBuff[i];

                //Array.Copy(_serialSendBuff, 0, sendframe, 3, frameSize);

                //if (frameSize == DATAFRAMESIZE11)
                //    sendframe[0] = 0;             //dataLength
                //else
                //    sendframe[0] = (byte)frameSize;

                byte[] btFrameNo = CommonConstants.RawSerialize(frameNo);
                sendframe[1] = btFrameNo[0];
                //sendframe[2] = btFrameNo[1];
                //byte crc = 0;//CalculateCRCs(frameSize, _serialSendBuff);            //CRC

                sendframe[259] = crc;//

                int res = NewSendFrames(sendframe);
                LogWriter.WriteToFile("GetSetFrameIntoBuff_Update5() ", "res : " + res.ToString() + " CRC: " + string.Join(",", crc), "RTC_Upgrade");
                return res;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                return FAILURE;
            }
        }

        private int NewSendFrames(byte[] sendframe)
        {
            LogWriter.WriteToFile(" => ", string.Join(",", sendframe), "RTC_Upgrade");
            try
            {
                _serialPort.Write(sendframe, 0, sendframe.Length);
                //Thread.Sleep(1000);
                _serialPort.ReadTimeout = TEN_SEC_DELAY;

                if (ReceiveFrame())
                {
                    LogWriter.WriteToFile(" NewSendFrames() <= ", string.Join(",", _serialReceivedByte), "RTC_Upgrade");
                    if (_serialReceivedByte == 0x01)
                        return SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                Close();
                return FAILURE;
            }
            return FAILURE;
        }
        public byte CalculateCRCs(int pNum, byte[] serialSendBuff)
        {
            byte serialCrcByte = 0;
            serialCrcByte = serialSendBuff[0];
            for (int i = 0; i < pNum; i++)
            {
                serialCrcByte ^= serialSendBuff[i];
            }
            serialTotalCrcByte ^= serialCrcByte;
            return serialCrcByte;
        }

        public byte serialTotalCrcByte { get; set; }

        #endregion




        public int Plc_Download_Protocol(string pFileName, byte pFileID)
        {
            byte[] _serialSendbyte = new byte[4];
            byte[] _serialRecvbyte = new byte[64];
            byte initialByte = 0x99;
            byte temp_receivedbyte = 0;

            _serialSendbyte[0] = initialByte;

            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
            if (_deviceIsConnected == false)
            {
                Close();
                return FAILURE;
            }

            bool valid_byte = false;

            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
            {
                _serialPort.DiscardInBuffer();
                Thread.Sleep(10);
                //issue_141_V2.2_sammed
                try
                {
                    _serialPort.Write(_serialSendbyte, 0, 1);
                }
                catch (Exception ex)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }
                //End
                if (ReceiveFrame())
                {
                    if (_serialReceivedByte == (initialByte + TOLERANCE_BYTE_1) || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_2)
                        || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_3) || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_4))
                    {
                        valid_byte = true;
                        break;
                    }
                    if (_serialNoOfByteRead > 0)
                    {
                        _serialPort.DiscardInBuffer();
                        Thread.Sleep(SEVEN_SEC_DELAY);
                    }
                }
            }

            if (!valid_byte)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                Close();
                return FAILURE;
            }

            temp_receivedbyte = _serialReceivedByte;

            while (true)
            {
                _serialRecvBuff[0] = 0;

                if (_serialNoOfByteRead > 0)
                {

                    if (temp_receivedbyte == (initialByte + TOLARANCE_BYTE_4))
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorModeMismatch));
                        Close();
                        return FAILURE;
                    }
                    else
                    {
                        if (temp_receivedbyte == (initialByte + TOLERANCE_BYTE_1) || temp_receivedbyte == (initialByte + TOLARANCE_BYTE_2))
                        {
                            return (temp_receivedbyte == 0x9A) ? SUCCESS : ONLYBOOTBLOCK; //Keeran (KA)
                        }
                        else if (temp_receivedbyte == (initialByte + TOLARANCE_BYTE_3))
                        {
                            if (_arrdwnlsetupframe[0] == 1)
                                return SUCCESS;
                            else
                            {
                                CommonConstants.communicationStatus = 0;
                                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                                Close();
                                return FAILURE;
                            }
                        }
                        else
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                            Close();
                            return FAILURE;
                        }
                    }
                }
                else
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }
            }
            return FAILURE;
        }

        public int Plc_Download_Protocol1(string pFileName, byte pFileID)
        {
            byte[] _serialSendbyte = new byte[4];
            byte[] _serialRecvbyte = new byte[64];
            byte initialByte = 0x99;
            byte temp_receivedbyte = 0;

            _serialSendbyte[0] = initialByte;

            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
            if (_deviceIsConnected == false)
            {
                Close();
                return FAILURE;
            }

            bool valid_byte = false;
            NO_OF_TRY_FOR_READY_PRIZM = 1;

            long SizeofFile = new System.IO.FileInfo(pFileName).Length; ////////New Change////////

            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
            {
                _serialPort.DiscardInBuffer();
                Thread.Sleep(10);
                //issue_141_V2.2_sammed
                try
                {
                    _serialPort.Write(_serialSendbyte, 0, 1);
                }
                catch (Exception ex)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }
                //End
                if (ReceiveFrame())
                {

                    byte[] Sizeinbyte = BitConverter.GetBytes(SizeofFile);

                    byte[] _SendSizeofFile = new byte[4];
                    _SendSizeofFile[0] = Sizeinbyte[3];
                    _SendSizeofFile[1] = Sizeinbyte[2];
                    _SendSizeofFile[2] = Sizeinbyte[1];
                    _SendSizeofFile[3] = Sizeinbyte[0];

                    _serialPort.Write(_SendSizeofFile, 0, 4);

                    _serialRecvBuff[0] = 0;

                    try
                    {
                        _serialNoOfByteRead = _serialPort.Read(_serialRecvBuff, 0, 1);
                    }
                    catch (System.TimeoutException timeOut)
                    {
                        _serialRecvBuff[0] = 0;
                        return FAILURE;
                    }
                    _serialReceivedByte = _serialRecvBuff[0];
                    //return SUCCESS;
                    if (_serialReceivedByte == (initialByte) || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_2)
                            || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_3) || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_4))
                    {
                        valid_byte = true;
                        break;
                    }
                    if (_serialNoOfByteRead > 0)
                    {
                        _serialPort.DiscardInBuffer();
                        Thread.Sleep(SEVEN_SEC_DELAY);
                    }
                }
            }

            if (!valid_byte)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                Close();
                return FAILURE;
            }

            temp_receivedbyte = _serialReceivedByte;

            while (true)
            {
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;

                if (_serialNoOfByteRead > 0)
                {

                    if (temp_receivedbyte == (initialByte + TOLARANCE_BYTE_4))
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorModeMismatch));
                        Close();
                        return FAILURE;
                    }
                    else
                    {
                        if (temp_receivedbyte == (initialByte + TOLERANCE_BYTE_1) || temp_receivedbyte == (initialByte))
                            return ONLYBOOTBLOCK;
                        else if (temp_receivedbyte == (initialByte + TOLARANCE_BYTE_2))
                        {
                            if (_arrdwnlsetupframe[0] == 1)
                                return SUCCESS;
                            else
                            {
                                CommonConstants.communicationStatus = 0;
                                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                                Close();
                                return FAILURE;
                            }
                        }
                        else
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                            Close();
                            return FAILURE;
                        }
                    }
                }
                else
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return FAILURE;
                }
            }
            return FAILURE;
        }

        //FP_CODE Pravin Download Mode
        public int PutUnit_HaltMode_BeforeDwnl(string pFileName, byte pFileID)
        {
            byte[] _serialSendbyte = new byte[4];
            byte[] _serialRecvbyte = new byte[4];
            byte initialByte = 0xD0;
            _serialSendbyte[0] = initialByte;

            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
            if (_deviceIsConnected == false)
            {
                Close();
                return FAILURE;
            }

            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
            {
                _serialPort.Write(_serialSendbyte, 0, 1);
                if (ReceiveFrame())
                {
                    i = NO_OF_TRY_FOR_READY_PRIZM;
                    break;
                }
            }

            if (_serialNoOfByteRead <= 0)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                Close();
                return FAILURE;
            }

            if (_serialReceivedByte == (initialByte + TOLARANCE_BYTE_3))
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                Close();
                return FAILURE;
            }

            else if (_serialReceivedByte == (initialByte + TOLARANCE_BYTE_4))
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorModeMismatch));
                Close();
                return FAILURE;
            }

            else if (_serialReceivedByte == (initialByte + TOLARANCE_BYTE_2))
            {
                _serialSendbyte.SetValue((Byte)0x52, 0);
                _serialSendbyte.SetValue((Byte)0x45, 1);
                _serialSendbyte.SetValue((Byte)0x50, 2);
                _serialSendbyte.SetValue((Byte)0x4c, 3);

                _serialPort.Write(_serialSendbyte, 0, 4);
                Thread.Sleep(1000);
                _serialPort.ReadTimeout = TEN_SEC_DELAY;
                while (true)
                {
                    _serialRecvBuff[0] = 0;
                    _serialReceivedByte = 0;

                    try
                    {
                        _serialNoOfByteRead = _serialPort.Read(_serialRecvbyte, 0, 1);
                    }
                    catch (System.TimeoutException timeOut)
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        return FAILURE;
                    }

                    if (_serialNoOfByteRead > 0)
                    {
                        return SUCCESS;
                    }
                }
            }
            else if (_serialReceivedByte == (initialByte + TOLERANCE_BYTE_1))
            {
                return SUCCESS;
            }
            else
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                Close();
            }
            return FAILURE;
        }
        //End00

        public int ReceiveLoggedData(string pFileName, byte pFileID)
        {
            _deviceFileName = pFileName;
            _serialFileID = pFileID;
            _serialSendBuff[0] = pFileID;
            _serialThread = new Thread(new ThreadStart(UpLoadLoggedData));
            _serialThread.Priority = ThreadPriority.Highest;
            _deviceDownloadStatus(0);

            _serialPort.WriteTimeout = TEN_SEC_DELAY * 2;

            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
            {
                _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                Thread.Sleep(1000);

                _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                _serialATimer.Enabled = true;
                if (ReceiveFrame(DOWN))
                {
                    i = NO_OF_TRY_FOR_READY_PRIZM;
                    break;
                }
                _serialTimeOut = 0;
                _serialATimer.Enabled = false;
            }
            if (_serialReceivedByte == pFileID || _serialReceivedByte == pFileID + TOLERANCE_BYTE_1 || _serialReceivedByte == pFileID + TOLARANCE_BYTE_2 || pFileID == SERIAL_LOGGEDDATA_UPLD)
            {	//	If Prizm Ready,
                _serialTimeOut = 0;
                if (_serialReceivedByte == pFileID + TOLARANCE_BYTE_2 && pFileID != SERIAL_LOGGEDDATA_UPLD)
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);
                    _serialPort.Write(_serialSendBuff, 0, 4);
                }
            }
            else // Unit Not Ready
            {
                _deviceDownloadStatus(7);
                return FAILURE;	//	FAILURE
            }

            int message = 256;
            if (_deviceFileID == CommonConstants.SERIAL_LOGGED_UPLD_FILEID)
                message <<= 4;

            _deviceDownloadStatus(2 + message);

            _serialSendBuff = new byte[1024 * 8 + 1 + DATA_START_INDEX + 4];

            Thread.Sleep(500);

            _serialThread.Start();
            _deviceDownloadStatus(18);
            return SUCCESS;
        }

        /// <summary>
        /// This Function Closes the the port by sending Finish Communication signal.
        /// It first checks if port is opened or not.
        /// </summary>
        public override void Close()
        {
            _serialRecvBuff = null;
            if (_serialPort != null && !_serialPort.IsOpen)
                return;
            else
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                    this.Dispose();
                }
            }
        }

        /// <summary>
        /// This is a thread routine called at the time of downloading,
        /// which helps not to overload on the main thread, which may be busy for GUI routines or other.
        /// </summary>
        public void DownLoadFunc()
        {
            bool error = false;
            int bytesRead = 0;
            FileStream fs = null;//FlexiSoft_IEC_Mngr_1255_AD
            int extraFrame = 1;
            CommonConstants.communicationStatus = 1;
            CommonConstants.communicationType = 1;
            _deviceFileInUse = false;//FlexiSoft_IEC_Mngr_1255_AD

            if (_deviceFileID != CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
            {
                FileInfo fi = new FileInfo(_deviceFileName);
                if (((uint)fi.Length) % DATAFRAMESIZE == 0)
                    extraFrame = 0;
                _deviceTotalFrames = Convert.ToUInt32(((uint)fi.Length) / DATAFRAMESIZE + extraFrame);
                fi = null;
            }

            for (int iTemp = 0; true; iTemp++)
            {
                int message = 256;
                if (_deviceFileID == CommonConstants.SERIAL_LADDER_DNLD_FILEID)
                    message <<= 2;
                if (_deviceFileID == CommonConstants.SERIAL_FONT_DNLD_FILEID)
                    message <<= 3;
                if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                    message <<= 1;
                #region FP_Ethernet_Implementation-AMIT
                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                    message <<= 7;

                #endregion
                _deviceDownloadStatus(3 + message);

                if (_deviceLength % DATAFRAMESIZE == 0)
                    extraFrame = 0;
                //AmitD.. When firmware file was exact divisible by 256 extraframe variable in that case was set to 0 which remained 0 in application and ladder case also when mulitple download was selected.
                else
                    extraFrame = 1;
                //

                try
                {
                    fs = new FileStream(_deviceFileName, FileMode.Open);

                    for (int i = 1; i <= _deviceTotalFrames; i++)
                    {
                        Thread.Sleep(0);
                        if (i == _deviceTotalFrames && extraFrame == 1)
                        {
                            bytesRead = (Convert.ToInt32(_deviceLength % DATAFRAMESIZE));
                            fs.Read(_serialSendBuff, DATA_START_INDEX, bytesRead);
                        }
                        else
                            bytesRead = fs.Read(_serialSendBuff, DATA_START_INDEX, DATAFRAMESIZE);

                        CalculateCRC(bytesRead);

                        if (i == 1)
                            _serialTotalCrcByte = _serialCrcByte;

                        if (i == _deviceTotalFrames)
                            SendFrame(i, bytesRead);
                        else
                            SendFrame(i, bytesRead, TEN_SEC_DELAY);

                        if (_deviceDownloadPercentage != null && CommonConstants.communicationStatus != -1 && CommonConstants.communicationStatus != -2)
                            _deviceDownloadPercentage(100 * i / Convert.ToInt32(_deviceTotalFrames));

                        //    if (!ReceiveFrame())
                        if (!ReceiveFrame_SetupFrameResponse())
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadStatus(10);
                            error = true;
                            break;
                        }

                        if (CommonConstants.communicationStatus < 0)
                        {
                            //_deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationTimeout));
                            error = true;
                            break;
                        }

                        if (_serialReceivedByte == 0 || _serialRecvBuff[0] != 0x01)
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadStatus(10);
                            error = true;
                            break;
                        }

                        if (_serialReceivedByte == PRODUCT_MISSMATCH)	//	if '1' received = OK received
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadStatus(5);
                            error = true;
                            break;
                        }

                        if (_serialReceivedByte == CRC_ERROR)
                            fs.Seek(fs.Position - DATAFRAMESIZE, SeekOrigin.Begin);

                        _serialATimer.Enabled = false;
                    }
                    if (!error)
                        FinalizeDownloading();

                    _deviceFileName = GetFileID(_serialFileID);

                    if (_deviceFileName == null || error)
                        break;
                }
                catch (UnauthorizedAccessException ex)//FlexiSoft_IEC_Mngr_1255_AD
                {
                    _deviceFileInUse = true;
                }
                catch (IOException ioEX)//VOffice_IssueNo_305
                {
                    _deviceFileInUse = true;
                }
                catch (Exception e)//VOffice_IssueNo_305
                {
                    error = true;
                    break;
                }
                finally//FlexiSoft_IEC_Mngr_1255_AD
                {
                    if (fs != null)
                        fs.Close();
                }

                if (_deviceProdID > 523 || _deviceProdID == 522)
                {
                    Thread.Sleep(THREE_SEC_DELAY);
                }
                //Thread.Sleep(TEN_SEC_DELAY);

                if (IsPrizmReady() != SUCCESS)
                {
                    if (_deviceProdID > 523 || _deviceProdID == 522)
                    {
                        Thread.Sleep(THREE_SEC_DELAY);
                        if (IsPrizmReady() != SUCCESS)
                        {
                            if (_deviceProdID > 523 || _deviceProdID == 522)
                            {
                                Thread.Sleep(THREE_SEC_DELAY);
                                if (IsPrizmReady() != SUCCESS)
                                    break;
                            }
                            else
                                break;
                        }
                    }
                    else
                        break;
                }
                if (InitiateCommunication() != SUCCESS)//FlexiSoft_IEC_Mngr_1255_AD
                {
                    error = true;
                    break;
                }

                #region FP_Ethernet_Implementation-AMIT
                if (_deviceFileName == null || _deviceFileName == "" || error)
                {
                    //if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                    //    FinalizeDownloading();
                    break;
                }

                if (_deviceFileID != CommonConstants.SERIAL_APPLICATION_DNLD_FILEID && _deviceFileName != null && _deviceFileName != "")
                #endregion
                {
                    FileInfo fi = new FileInfo(_deviceFileName);
                    if (((uint)fi.Length) % DATAFRAMESIZE == 0)
                        extraFrame = 0;
                    //AmitD.. When firmware file was exact divisible by 256 extraframe variable in that case was set to 0 which remained 0 in application and ladder case also when mulitple download was selected.
                    else
                        extraFrame = 1;
                    //
                    _deviceTotalFrames = Convert.ToUInt32(((uint)fi.Length) / DATAFRAMESIZE + extraFrame);
                    fi = null;
                }
            }
            if (!error)
            {
                //if (_serialDataLoggerEraseMem)
                //    SendEraseDataLoggerMemoryFrame();
                #region Issue2.2_418 Vijay
                //Issue 333 SP 9.10.12
                if (ClassList.CommonConstants.g_Support_IEC_Ladder && ClassList.CommonConstants.g_DownloadForOnLine == true && _deviceFileName == null)
                {
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strWaitDeviceIsInitializing));
                    if (ClassList.CommonConstants.IsProductSupportsEthernet(ClassList.CommonConstants.ProductDataInfo.iProductID))
                        System.Threading.Thread.Sleep(16000);
                    else
                        System.Threading.Thread.Sleep(10000);
                }
                //End
                #endregion
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
            }
            else if (CommonConstants.communicationStatus == -1 || CommonConstants.communicationStatus == -2)
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationAborted));
            else if (_deviceFileInUse)//FlexiSoft_IEC_Mngr_1255_AD
            {
                CommonConstants.communicationStatus = 0;
                if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorFirmwareFileCreation));
                if (_deviceFileID == CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorApplicationFileCreation));
            }
            else
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
            }

            if (fs != null)//FlexiSoft_IEC_Mngr_1255_AD
                fs.Close();
            fs = null;
            Close();
            return;
        }
        public void DownLoadFunc2()
        {
            bool error = false;
            int bytesRead = 0;
            FileStream fs = null;//FlexiSoft_IEC_Mngr_1255_AD
            int extraFrame = 1;
            CommonConstants.communicationStatus = 1;
            CommonConstants.communicationType = 1;
            _deviceFileInUse = false;//FlexiSoft_IEC_Mngr_1255_AD

            if (_deviceFileID != CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
            {
                FileInfo fi = new FileInfo(_deviceFileName);
                if (((uint)fi.Length) % DATAFRAMESIZE == 0)
                    extraFrame = 0;
                _deviceTotalFrames = Convert.ToUInt32(((uint)fi.Length) / DATAFRAMESIZE + extraFrame);
                fi = null;
            }

            _deviceDownloadStatus(Convert.ToInt32(ClassList.DownloadingStatusMessages.strBootBlock1));
            int x11 = Convert.ToInt32(ClassList.DownloadingStatusMessages.strBootBlock1);

            for (int iTemp = 0; true; iTemp++)
            {
                int message = 256;
                if (_deviceFileID == CommonConstants.SERIAL_LADDER_DNLD_FILEID)
                    message <<= 2;
                if (_deviceFileID == CommonConstants.SERIAL_FONT_DNLD_FILEID)
                    message <<= 3;
                if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                    message <<= 1;
                #region FP_Ethernet_Implementation-AMIT
                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                    message <<= 7;

                #endregion
                _deviceDownloadStatus(Convert.ToInt32(ClassList.DownloadingStatusMessages.strBootBlock1));

                if (_deviceLength % DATAFRAMESIZE == 0)
                    extraFrame = 0;
                //AmitD.. When firmware file was exact divisible by 256 extraframe variable in that case was set to 0 which remained 0 in application and ladder case also when mulitple download was selected.
                else
                    extraFrame = 1;
                //

                try
                {
                    fs = new FileStream(_deviceFileName, FileMode.Open);

                    for (int i = 1; i <= _deviceTotalFrames; i++)
                    {
                        Thread.Sleep(0);
                        if (i == _deviceTotalFrames && extraFrame == 1)
                        {
                            bytesRead = (Convert.ToInt32(_deviceLength % DATAFRAMESIZE));
                            fs.Read(_serialSendBuff, DATA_START_INDEX, bytesRead);
                        }
                        else
                            bytesRead = fs.Read(_serialSendBuff, DATA_START_INDEX, DATAFRAMESIZE);

                        CalculateCRC1(bytesRead);
                        //if (i == 1)
                        //    _serialTotalCrcByte = _serialCrcByte;

                        if (i == _deviceTotalFrames)
                            SendFrame1(i, bytesRead);
                        else
                            SendFrame1(i, bytesRead, TEN_SEC_DELAY);

                        if (_deviceDownloadPercentage != null && CommonConstants.communicationStatus != -1 && CommonConstants.communicationStatus != -2)
                            _deviceDownloadPercentage(99 * i / Convert.ToInt32(_deviceTotalFrames));

                        _deviceDownloadStatus(Convert.ToInt32(ClassList.DownloadingStatusMessages.strBootBlock1));

                        if (CommonConstants.communicationStatus < 0)
                        {
                            error = true;
                            break;
                        }

                        if (_serialReceivedByte == 0 || _serialRecvBuff[0] != 0x01)
                        {
                            CommonConstants.communicationStatus = 0;
                            //  _deviceDownloadStatus(10);
                            //  error = true;
                            //   break;
                        }

                        if (_serialReceivedByte == PRODUCT_MISSMATCH)	//	if '1' received = OK received
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadStatus(5);
                            error = true;
                            break;
                        }

                        if (_serialReceivedByte == CRC_ERROR)
                            fs.Seek(fs.Position - DATAFRAMESIZE, SeekOrigin.Begin);

                        _serialATimer.Enabled = false;
                    }

                    _serialRecvBuff[0] = 0;

                    try
                    {
                        _serialNoOfByteRead = _serialPort.Read(_serialRecvBuff, 0, 1);
                    }
                    catch (System.TimeoutException timeOut)
                    {
                        _serialRecvBuff[0] = 0;
                    }

                    _serialSendBuff[0] = _serialTotalCrcByte;
                    _serialPort.Write(_serialSendBuff, 0, 1);

                    _serialRecvBuff[0] = 0;

                    try
                    {
                        _serialNoOfByteRead = _serialPort.Read(_serialRecvBuff, 0, 1);
                    }
                    catch (System.TimeoutException timeOut)
                    {
                        _serialRecvBuff[0] = 0;
                    }
                    if (_serialSendBuff[0] == _serialRecvBuff[0])
                    {
                        _serialSendBuff[0] = 0x55;
                        _serialPort.Write(_serialSendBuff, 0, 1);

                        _serialRecvBuff[0] = 0;
                        try
                        {
                            _serialNoOfByteRead = _serialPort.Read(_serialRecvBuff, 0, 1);
                        }
                        catch (System.TimeoutException timeOut)
                        {
                            _serialRecvBuff[0] = 0;
                            _deviceDownloadPercentage(100);
                        }
                        _deviceDownloadPercentage(100);
                        _serialPort.Close();
                        Thread.Sleep(50);
                    }
                    else
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationAborted));
                }
                catch (UnauthorizedAccessException ex)//FlexiSoft_IEC_Mngr_1255_AD
                {
                    _deviceFileInUse = true;
                }
                catch (IOException ioEX)//VOffice_IssueNo_305
                {
                    _deviceFileInUse = true;
                }
                catch (Exception e)//VOffice_IssueNo_305
                {
                    error = true;
                    break;
                }
                finally//FlexiSoft_IEC_Mngr_1255_AD
                {
                    if (fs != null)
                        fs.Close();
                }
            }
            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));

            if (fs != null)//FlexiSoft_IEC_Mngr_1255_AD
                fs.Close();
            fs = null;
            Close();
            return;
        }

        /// <summary>
        /// This is a thread routine called at the time of uploading,
        /// which helps not to overload on the main thread, which may be busy for GUI routines or other.
        /// </summary>
        public void UpLoadFunc()
        {
            byte[] _arr = new byte[4];
            bool error = false;
            int bytesRead = 0;
            FileStream fs;
            CommonConstants.communicationStatus = 2;
            CommonConstants.communicationType = 2;
            uint Ladder_Size = 0;
            uint Application_Size = 0;
            bool logger_flag = false;

            try
            {
                if (_deviceFileName == CommonConstants.UPLOAD_LADDER_FILENAME)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        _serialRecvBuff[i] = (byte)_serialPort.ReadByte();
                    }
                    Ladder_Size = CommonConstants.MAKEUINT(_serialRecvBuff);
                    _serialSendBuff[0] = 0x01;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }

                for (int iTemp = 0; true; iTemp++)
                {
                    fs = new FileStream(_deviceFileName, FileMode.Create);
                    if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                        ReceiveFrame(DOWN);
                    else
                    {
                        //while (true) //Remove this while loop to avoid infinte loop
                        for (int j = 0; j < 3; j++)
                        {
                            if (_serialRecvBuff[0] != SERIAL_LOGGEDDATA_UPLD)
                            {
                                if (ReceiveFrame(DOWN))
                                {
                                    logger_flag = true;
                                    break;
                                }
                            }
                        }
                        if (!logger_flag)
                        {
                            error = true;
                            break;
                        }
                    }
                    int message = 256;
                    if (_deviceFileID == CommonConstants.SERIAL_LOGGED_UPLD_FILEID)
                    {
                        message <<= 4;
                        _deviceDownloadStatus(2 + message);
                    }
                    //_deviceDownloadStatus(2 + message);

                    _deviceTotalFrames = 2; // Temp assigned to execute atleast once so that it get initialized
                    for (int i = 1; i < _deviceTotalFrames + 1; i++)
                    {
                        if (_deviceDownloadPercentage != null && CommonConstants.communicationStatus != -1 && i > 1)
                        {
                            if (_deviceFileID == CommonConstants.LADDER_UPLD_FILEID)
                            {
                                float f;
                                f = i * 100 / _deviceTotalFrames;
                                _deviceDownloadPercentage(f);
                            }
                            else
                            {
                                float f;
                                if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                                    f = 100 * i / Convert.ToInt32(_deviceTotalFrames / RECVFRAMESIZE);
                                else
                                    f = 100 * i / Convert.ToInt32(_deviceTotalFrames);

                                _deviceDownloadPercentage(f < 100 ? f : 100);
                            }
                        }

                        if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD && i == 1)
                        {
                            ReadLoggingStartBytes();
                            //ReceiveFrameTest(UP);
                            _serialNoOfByteTORead += 2;
                            int tempID = _serialSendBuff[0];
                            tempID = tempID & 0x0F;
                            _deviceTotalFrames = tempID == 3 ? (uint)(20 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : tempID == 1 ? (uint)(5 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : tempID == 2 ? (uint)(10 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : (uint)(2 * (1024 * 1024) + 256 * 1024) / (1024 * 8);
                            switch (tempID)
                            {
                                case 0:
                                    _deviceTotalFrames = 512 * 1024; //256+256 //Mem settings + 256
                                    break;
                                case 1:
                                    _deviceTotalFrames = 768 * 1024; //512+256
                                    break;
                                case 2:
                                    _deviceTotalFrames = 1280 * 1024; //1024+256
                                    break;
                                case 3:
                                    _deviceTotalFrames = 2304 * 1024; //2048+256
                                    break;
                                #region Sammed_DataLogger_Memorysize
                                case 4:
                                    _deviceTotalFrames = 4352 * 1024; //4096+256 
                                    break;
                                case 5:
                                    _deviceTotalFrames = 8448 * 1024; //8192+256
                                    break;
                                case 6:
                                    _deviceTotalFrames = 12544 * 1024; //12288+256
                                    break;
                                case 7:
                                    _deviceTotalFrames = 16640 * 1024; //16384+256
                                    break;
                                case 8:
                                    _deviceTotalFrames = 20736 * 1024; //20840+256
                                    break;
                                    #endregion
                            }

                            if (_serialSendBuff[0] > 0x0f)
                                _deviceTotalFrames -= 256 * 1024;
                            //_deviceTotalFrames = (uint)0x520;                    
                            bytesRead = _serialNoOfByteRead - 3;
                            _deviceTotalFrames /= 8192;
                        }
                        if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD && i != 1)
                            ReceiveFrameTest(UP);
                        else if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                            if (ReceiveFrame(UP) == false)
                            {
                                error = true;
                                break;
                            }

                        if (i == 1 && _deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                        {
                            if (_deviceFileID == CommonConstants.LADDER_UPLD_FILEID)
                            {
                                _deviceTotalFrames = Ladder_Size % RECVFRAMESIZE > 0 ? Ladder_Size / RECVFRAMESIZE + 1 : Ladder_Size / RECVFRAMESIZE;
                                if (_deviceTotalFrames == 1)
                                    _deviceDownloadPercentage(100);
                            }
                            else
                            {
                                iProductID = ClassList.CommonConstants.MAKEWORD(_serialSendBuff[6], _serialSendBuff[7]);
                                _serialNoOfByteTORead += 2;
                                _deviceTotalFrames = _serialSendBuff[2];
                                _deviceTotalFrames += Convert.ToUInt32(_serialSendBuff[3] * 256);
                                _deviceTotalFrames += Convert.ToUInt32(_serialSendBuff[4] << 16);
                                _deviceTotalFrames += Convert.ToUInt32(_serialSendBuff[5] << 24);

                                _arr[0] = _serialSendBuff[2];
                                _arr[1] = _serialSendBuff[3];
                                _arr[2] = _serialSendBuff[4];
                                _arr[3] = _serialSendBuff[5];
                                Application_Size = CommonConstants.MAKEUINT(_arr);
                                //_deviceTotalFrames = Application_Size % RECVFRAMESIZE > 0 ? Application_Size / RECVFRAMESIZE + 1 : Application_Size / RECVFRAMESIZE;

                            }

                            #region FP_CODE     Issue No. 190	Punam
                            _serialProductIDBuffer = _serialSendBuff;
                            iProductID = ClassList.CommonConstants.MAKEWORD(_serialProductIDBuffer[6], _serialProductIDBuffer[7]);
                            #endregion FP_CODE

                        }

                        if (CommonConstants.communicationStatus == -1 || CommonConstants.communicationStatus == -2)
                        {
                            //_deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationTimeout));
                            error = true;
                            break;
                        }

                        if (_serialSendBuff[0] == 0xdd)
                            break;

                        if (i != 1)
                        {
                            bytesRead = _serialSendBuff[0];
                            bytesRead += _serialSendBuff[1] * 256;
                        }
                        else
                        {
                            bytesRead = _serialNoOfByteRead - DATA_START_INDEX;
                        }
                        if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                            CalculateCRC(bytesRead);
                        if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                            if (i < 2)
                                fs.Write(_serialSendBuff, 1, bytesRead + 1);
                            else
                                fs.Write(_serialSendBuff, 3, bytesRead);
                        else
                            fs.Write(_serialSendBuff, 2, bytesRead);

                        _serialPort.DiscardInBuffer();//Issue_505_Reopen_263, 266AMIT
                        _serialSendBuff[0] = 0x01;
                        _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                        //_serialPort.DiscardInBuffer();//Issue_263 AMIT.//Commented by AMIT //Issue 505
                        if (_deviceFileID != CommonConstants.LADDER_UPLD_FILEID)
                        {
                            if (i >= (_deviceTotalFrames % RECVFRAMESIZE > 0 ? _deviceTotalFrames / RECVFRAMESIZE + 1 : _deviceTotalFrames / RECVFRAMESIZE) && _deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                                break;
                            if (i >= _deviceTotalFrames && _deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                                break;
                        }
                    }

                    if (!error)
                    {
                        ReceiveFrame(DOWN);
                        ReceiveFrame(DOWN);
                    }

                    _serialSendBuff[0] = 0xdd;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);


                    bool ldr_flag = false;
                    if (_deviceFileName == CommonConstants.UPLOAD_LADDER_FILENAME)
                    {
                        CommonConstants.LADDER_PRESENT = true;
                        ldr_flag = true;
                        _deviceDownloadStatus(2 + message);
                    }

                    _deviceFileName = GetUploadFileID(_serialFileID);

                    if (!ldr_flag && _deviceFileName == CommonConstants.UPLOAD_LADDER_FILENAME)
                        _deviceFileName = null;

                    if (_deviceFileName == null)
                        break;

                    if (_deviceFileName == null || error)
                        break;

                    if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                    {
                        if ((CommonConstants.IsProductSupportsDataLogger(iProductID)) == false)
                        {
                            _serialSendBuff[0] = 0x55;
                            _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strDataLogNotSupported));
                            fs.Close();
                            fs = null;
                            Close();
                            return;
                        }
                    }

                    _serialSendBuff[0] = _deviceFileID;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);

                    if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                        _serialSendBuff = new byte[1024 * 8 + 1 + DATA_START_INDEX + 4];

                    fs.Close();
                }

                Thread.Sleep(1000);
                if (true)
                {
                    _serialSendBuff[0] = 0x55;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
                else
                {
                    _serialSendBuff[0] = 0xdd;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
                fs.Close();
            }
            catch (Exception e)
            {
                error = true;
                CommonConstants.communicationType = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
            }
            Close();
            if (!error)
            {
                CommonConstants.communicationStatus = 0;
                CommonConstants.communicationType = 2;
                _deviceDownloadStatus(18);
            }
            else if (CommonConstants.communicationStatus == -1)
            {
                //CommonConstants.communicationType = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationAborted));
            }
            else
            {
                CommonConstants.communicationType = 0;
                CommonConstants.communicationStatus = -1;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
            }

            return;
        }

        #region AdvancedAlarm

        /// <summary>
        /// This is a thread routine called at the time of uploading,
        /// which helps not to overload on the main thread, which may be busy for GUI routines or other.
        /// </summary>
        public void UpLoadFuncForAlarm()
        {
            bool error = false;
            int bytesRead = 0;
            FileStream fs;
            CommonConstants.communicationStatus = 2;
            CommonConstants.communicationType = 2;
            uint Ladder_Size = 0;//Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT

            try
            {
                #region #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
                if (_deviceFileName == CommonConstants.UPLOAD_LADDER_FILENAME)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        _serialRecvBuff[i] = (byte)_serialPort.ReadByte();
                    }
                    Ladder_Size = CommonConstants.MAKEUINT(_serialRecvBuff);
                    _serialSendBuff[0] = 0x01;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
                #endregion

                for (int iTemp = 0; true; iTemp++)
                {
                    fs = new FileStream(_deviceFileName, FileMode.Create);
                    if ((_deviceFileID != SERIAL_LOGGEDDATA_UPLD) && (_deviceFileID != SERIAL_HIST_ALARM_DATA_UPLD))
                        ReceiveFrame(DOWN);
                    else
                    {
                        while (true)
                        {
                            if ((_serialRecvBuff[0] != SERIAL_LOGGEDDATA_UPLD) && (_serialRecvBuff[0] != SERIAL_HIST_ALARM_DATA_UPLD))
                                ReceiveFrame(DOWN);
                            else
                                break;
                        }
                    }
                    int message = 256;
                    if (_deviceFileID == CommonConstants.SERIAL_LOGGED_UPLD_FILEID)
                    {
                        message <<= 4;
                        _deviceDownloadStatus(2 + message);
                    }
                    if (_deviceFileID == CommonConstants.SERIAL_HISTALARM_UPLD_FILEID)
                    {
                        message <<= 5;
                        _deviceDownloadStatus(2 + message);
                    }
                    _deviceTotalFrames = 2; // Temp assigned to execute atleast once so that it get initialized
                    for (int i = 1; i < _deviceTotalFrames + 1; i++)
                    {
                        if (_deviceDownloadPercentage != null && CommonConstants.communicationStatus != -1 && i > 1)
                        {
                            #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
                            if (_deviceFileID == CommonConstants.LADDER_UPLD_FILEID)
                            {
                                float f;
                                f = i * 100 / _deviceTotalFrames;
                                _deviceDownloadPercentage(f);
                            }
                            else
                            {
                                float f;
                                if ((_deviceFileID != SERIAL_LOGGEDDATA_UPLD) && (_deviceFileID != SERIAL_HIST_ALARM_DATA_UPLD))
                                    f = 100 * i / Convert.ToInt32(_deviceTotalFrames / RECVFRAMESIZE);
                                else
                                    f = 100 * i / Convert.ToInt32(_deviceTotalFrames);

                                _deviceDownloadPercentage(f < 100 ? f : 100);
                            }
                            #endregion
                        }

                        if ((_deviceFileID == SERIAL_LOGGEDDATA_UPLD && i == 1))
                        {

                            ReadLoggingStartBytes();
                            _serialNoOfByteTORead += 2;
                            int tempID = _serialSendBuff[0];
                            tempID = tempID & 0x0F;
                            _deviceTotalFrames = tempID == 3 ? (uint)(20 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : tempID == 1 ? (uint)(5 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : tempID == 2 ? (uint)(10 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : (uint)(2 * (1024 * 1024) + 256 * 1024) / (1024 * 8);
                            switch (tempID)
                            {
                                case 0:
                                    _deviceTotalFrames = 512 * 1024; //256+256 //Mem settings + 256
                                    break;
                                case 1:
                                    _deviceTotalFrames = 768 * 1024; //512+256
                                    break;
                                case 2:
                                    _deviceTotalFrames = 1280 * 1024; //1024+256
                                    break;
                                case 3:
                                    _deviceTotalFrames = 2304 * 1024; //2048+256
                                    break;
                                #region Sammed_DataLogger_Memorysize
                                case 4:
                                    _deviceTotalFrames = 4352 * 1024; //4096+256 
                                    break;
                                case 5:
                                    _deviceTotalFrames = 8448 * 1024; //8192+256
                                    break;
                                case 6:
                                    _deviceTotalFrames = 12544 * 1024; //12288+256
                                    break;
                                case 7:
                                    _deviceTotalFrames = 16640 * 1024; //16384+256
                                    break;
                                case 8:
                                    _deviceTotalFrames = 20736 * 1024; //20840+256
                                    break;
                                    #endregion
                            }

                            if (_serialSendBuff[0] > 0x0f)
                                _deviceTotalFrames -= 256 * 1024;
                            //_deviceTotalFrames = (uint)0x520;                    
                            bytesRead = _serialNoOfByteRead - 3;
                            _deviceTotalFrames /= 8192;
                        }
                        if ((_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD && i == 1))
                        {
                            ReadHistAlarmStartBytes();

                            _serialNoOfByteTORead += 2;
                            int tempID = _serialSendBuff[0];
                            _deviceTotalFrames = Convert.ToUInt32(tempID);

                            bytesRead = _serialNoOfByteRead - 3;
                        }

                        if ((_deviceFileID == SERIAL_LOGGEDDATA_UPLD && i != 1) || (_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD && i != 1))
                            ReceiveFrameTest(UP);
                        else if ((_deviceFileID != SERIAL_LOGGEDDATA_UPLD) && (_deviceFileID != SERIAL_HIST_ALARM_DATA_UPLD))
                            if (ReceiveFrame(UP) == false)
                            {
                                error = true;
                                break;
                            }

                        if ((i == 1 && _deviceFileID != SERIAL_LOGGEDDATA_UPLD) && (i == 1 && _deviceFileID != SERIAL_HIST_ALARM_DATA_UPLD))
                        {
                            #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
                            if (_deviceFileID == CommonConstants.LADDER_UPLD_FILEID)
                            {
                                _deviceTotalFrames = Ladder_Size % RECVFRAMESIZE > 0 ? Ladder_Size / RECVFRAMESIZE + 1 : Ladder_Size / RECVFRAMESIZE;
                                if (_deviceTotalFrames == 1)
                                    _deviceDownloadPercentage(100);
                            }
                            else
                            {
                                iProductID = ClassList.CommonConstants.MAKEWORD(_serialSendBuff[6], _serialSendBuff[7]);
                                _serialNoOfByteTORead += 2;
                                _deviceTotalFrames = _serialSendBuff[2];
                                _deviceTotalFrames += Convert.ToUInt32(_serialSendBuff[3] * 256);
                                _deviceTotalFrames += Convert.ToUInt32(_serialSendBuff[4] << 16);
                                _deviceTotalFrames += Convert.ToUInt32(_serialSendBuff[5] << 24);

                                /*   #region FP_CODE     Issue No. 190	Punam
                                   _serialProductIDBuffer = _serialSendBuff;
                                   iProductID = ClassList.CommonConstants.MAKEWORD(_serialProductIDBuffer[6], _serialProductIDBuffer[7]);
                                   #endregion FP_CODE*/
                            }
                            #endregion
                        }

                        if (CommonConstants.communicationStatus == -1)
                        {
                            //_deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationTimeout));
                            error = true;
                            break;
                        }

                        if (_serialSendBuff[0] == 0xdd)
                            break;

                        if (i != 1)
                        {
                            bytesRead = _serialSendBuff[0];
                            bytesRead += _serialSendBuff[1] * 256;
                        }
                        else
                        {
                            bytesRead = _serialNoOfByteRead - DATA_START_INDEX;
                        }
                        if ((_deviceFileID != SERIAL_LOGGEDDATA_UPLD) && (_deviceFileID != SERIAL_HIST_ALARM_DATA_UPLD))
                            CalculateCRC(bytesRead);
                        if ((_deviceFileID == SERIAL_LOGGEDDATA_UPLD) || (_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD))
                            if (i < 2)
                                fs.Write(_serialSendBuff, 1, bytesRead + 1);
                            else
                                fs.Write(_serialSendBuff, 3, bytesRead);
                        else
                            fs.Write(_serialSendBuff, 2, bytesRead);
                        _serialSendBuff[0] = 0x01;
                        _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                        #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
                        if (_deviceFileID != CommonConstants.LADDER_UPLD_FILEID)
                        {
                            if (i >= (_deviceTotalFrames % RECVFRAMESIZE > 0 ? _deviceTotalFrames / RECVFRAMESIZE + 1 : _deviceTotalFrames / RECVFRAMESIZE) && ((_deviceFileID != SERIAL_LOGGEDDATA_UPLD) && (_deviceFileID != SERIAL_HIST_ALARM_DATA_UPLD)))
                                break;
                            if (i >= _deviceTotalFrames && ((_deviceFileID == SERIAL_LOGGEDDATA_UPLD) || (_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD)))
                                break;
                        }
                        #endregion
                    }
                    if (!error)
                    {
                        ReceiveFrame(DOWN);
                        ReceiveFrame(DOWN);
                    }

                    _serialSendBuff[0] = 0xdd;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);

                    #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
                    bool ldr_flag = false;
                    if (_deviceFileName == CommonConstants.UPLOAD_LADDER_FILENAME)
                    {
                        CommonConstants.LADDER_PRESENT = true;
                        ldr_flag = true;
                        _deviceDownloadStatus(2 + message);
                    }
                    #endregion

                    _deviceFileName = GetUploadFileIDForAlarm(_serialFileID);

                    if (_deviceFileName == null || error)
                        break;

                    if (_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD)
                    {
                        if ((CommonConstants.IsProductSupportsHisAlarm(iProductID)) == false)
                        {
                            _serialSendBuff[0] = 0x55;
                            _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strHisAlarmNotSupported));
                            fs.Close();
                            fs = null;
                            Close();
                            return;
                        }
                    }

                    if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                    {
                        if ((CommonConstants.IsProductSupportsDataLogger(iProductID)) == false)
                        {
                            _serialSendBuff[0] = 0x55;
                            _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strDataLogNotSupported));
                            fs.Close();
                            fs = null;
                            Close();
                            return;
                        }
                    }


                    _serialSendBuff[0] = _deviceFileID;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);

                    _serialSendBuff = new byte[1024 * 8 + 1 + DATA_START_INDEX + 4];

                    fs.Close();
                }
                Thread.Sleep(1000);
                if (true)
                {
                    _serialSendBuff[0] = 0x55;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
                else
                {
                    _serialSendBuff[0] = 0xdd;
                    _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
                fs.Close();
            }
            catch (Exception e)
            {
                error = true;
                CommonConstants.communicationType = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
            }
            Close();
            if (!error)
            {
                CommonConstants.communicationStatus = 0;
                CommonConstants.communicationType = 2;
                _deviceDownloadStatus(18);
            }
            else if (CommonConstants.communicationStatus == -1)
            {
                CommonConstants.communicationType = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationAborted));
            }
            else
            {
                CommonConstants.communicationType = 0;
                CommonConstants.communicationStatus = -1;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
            }

            return;
        }

        private void ReadHistAlarmStartBytes()
        {
            int bytesRead = 0;

            ReceiveFrameTest(DOWN);
            bytesRead = _serialRecvBuff[0];

            ReceiveFrameTest(DOWN);
            bytesRead += _serialRecvBuff[0] * 256;
            _serialNoOfByteTORead = bytesRead + 2;

            ReceiveFrameTest(UP);
        }

        protected string GetUploadFileIDForAlarm(int pFileID)
        {
            string tempStr = null;
            _deviceFileID = 0;
            #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
            if ((pFileID & (byte)DownloadData.Ladder) > 0)
            {
                _deviceFileID = CommonConstants.LADDER_UPLD_FILEID;
                tempStr = CommonConstants.UPLOAD_LADDER_FILENAME;
                _deviceFileName = tempStr;
                _serialFileID ^= 4;
            }
            #endregion
            else if ((pFileID & (int)DownloadData.Application) > 0)
            {
                _deviceFileID = CommonConstants.SERIAL_APPLICATION_UPLD_FILEID;
                tempStr = CommonConstants.TEMP_DOWNLOAD_FILENAME;
                _serialFileID ^= 1;
            }
            else if ((pFileID & (int)DownloadData.HistAlarmData) > 0)
            {
                _deviceFileID = SERIAL_HIST_ALARM_DATA_UPLD;
                _serialFileID ^= (int)DownloadData.HistAlarmData;
                _deviceProdID = SERIAL_HIST_ALARM_DATA_UPLD;
                tempStr = CommonConstants.HistAlarmBinaryFile;
            }
            else if ((pFileID & (int)DownloadData.LoggedData) > 0)
            {
                _deviceFileID = SERIAL_LOGGEDDATA_UPLD;
                _serialFileID ^= 16;
                _deviceProdID = SERIAL_LOGGEDDATA_UPLD;
                tempStr = CommonConstants.BinaryFile;
            }

            _serialSendBuff[0] = _deviceFileID;
            return tempStr;
        }

        #endregion


        //public void UpLoadLoggedData()
        //{
        //    int bytesRead = 0;
        //    FileStream fs = new FileStream(_deviceFileName, FileMode.Create);
        //    StreamWriter fsw = new StreamWriter("UploadLog.txt");
        //    fsw.WriteLine("Start...");
        //    for (int i = 1; ; i++)
        //    {
        //        fsw.WriteLine("For i = " + i.ToString());

        //        if (i == 1)
        //        {
        //            if (!ReceiveFrameTest(DOWN))break;
        //            bytesRead = _serialRecvBuff[0];

        //            if (!ReceiveFrameTest(DOWN)) break;
        //            bytesRead += _serialRecvBuff[0] * 256;

        //            _serialNoOfByteTORead = bytesRead + 2;

        //            fsw.WriteLine("\t\t1 To Read = " + _serialNoOfByteTORead.ToString());
        //            fsw.WriteLine("\t\t1 Received = " + _serialNoOfByteRead.ToString());

        //            if (!ReceiveFrameTest(UP)) break;

        //            fsw.WriteLine("\t\t2 To Read = " + _serialNoOfByteTORead.ToString());
        //            fsw.WriteLine("\t\t2 Received = " + _serialNoOfByteRead.ToString());

        //            while (true)
        //            {
        //                if (!ReceiveFrameTest(DOWN))
        //                {
        //                    fsw.WriteLine("\t0Recieved data fail = " + _serialRecvBuff[0].ToString() );
        //                    break;
        //                }
        //                if (_serialRecvBuff[0] == 0xAA)
        //                {
        //                    fsw.WriteLine("\t0xAA recieved ");
        //                    break;
        //                }
        //                else
        //                    fsw.WriteLine("\t1Recieved data = " + _serialRecvBuff[0].ToString());
        //                //Thread.Sleep(1000);
        //            }

        //            if (!ReceiveFrameTest(DOWN))
        //                break;

        //            fsw.WriteLine("\t2Recieved data = " + _serialRecvBuff[0].ToString());

        //            bytesRead = _serialRecvBuff[0];

        //            fsw.WriteLine("\t\tBytes To Read = " + bytesRead.ToString());
        //            fsw.WriteLine("\t\tBytes Received = " + _serialNoOfByteRead.ToString());

        //            if (!ReceiveFrameTest(DOWN))
        //                break;

        //            fsw.WriteLine("\t3Recieved data = " + _serialRecvBuff[0].ToString());

        //            bytesRead += _serialRecvBuff[0] * 256;
        //            _serialNoOfByteTORead = bytesRead + 2;

        //            fsw.WriteLine("\t\tBytes To Read = " + _serialNoOfByteTORead.ToString());
        //            fsw.WriteLine("\t\tBytes Received = " + _serialNoOfByteRead.ToString());

        //        }
        //        if (!ReceiveFrameTest(UP)) break;
        //        fsw.WriteLine("\tBytes Received = " + _serialNoOfByteRead.ToString());

        //        if (i == 1)
        //        {
        //            _serialNoOfByteTORead += 2;
        //            _deviceTotalFrames = _serialSendBuff[2];
        //            _deviceTotalFrames += Convert.ToUInt32(_serialSendBuff[3] * 256);
        //        }
        //        if (_serialSendBuff[0] == 0xdd)break;

        //        if (i == 1)
        //        {
        //            int tempID = _serialSendBuff[2];
        //            _deviceTotalFrames = tempID == 3 ? (uint)(20 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : tempID == 1 ? (uint)(5 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : tempID == 2 ? (uint)(10 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : (uint)(2 * (1024 * 1024) + 256 * 1024) / (1024 * 8);
        //        }
        //        else
        //        {
        //            bytesRead = _serialSendBuff[0];
        //            bytesRead += _serialSendBuff[1] * 256;
        //        }
        //        if (bytesRead > RECVFRAMESIZE)
        //            bytesRead = RECVFRAMESIZE + 2;

        //        CalculateCRC(bytesRead);

        //        Thread.Sleep(500);

        //        if (i < 2)
        //            fs.Write(_serialSendBuff, 1, bytesRead + 1);
        //        else
        //            fs.Write(_serialSendBuff, 3, bytesRead);

        //        _serialSendBuff[0] = 0x01;
        //        _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
        //        if (i >= _deviceTotalFrames)break;
        //    }
        //    fsw.WriteLine("For loop Complete...");

        //    fsw.WriteLine("\t\tBytes To Read = " + _serialNoOfByteTORead.ToString());
        //    fsw.WriteLine("\t\tBytes Received = " + _serialNoOfByteRead.ToString());
        //    fsw.WriteLine("\t4Recieved data = " + _serialRecvBuff[0].ToString());

        //    if (ReceiveFrame(DOWN))
        //        if (ReceiveFrame(DOWN))
        //        {
        //            _serialSendBuff[0] = 0xdd;
        //            _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
        //        }

        //    fsw.WriteLine("\t\tBytes To Read = " + _serialNoOfByteTORead.ToString());
        //    fsw.WriteLine("\t\tBytes Received = " + _serialNoOfByteRead.ToString());
        //    fsw.WriteLine("\t5Recieved data = " + _serialRecvBuff[0].ToString());

        //    fsw.WriteLine("Finish...");
        //    fsw.Close();
        //    fs.Close();
        //    return;
        //}


        public void UpLoadLoggedData()
        {
            int bytesRead = 0;
            CommonConstants.communicationStatus = 2;
            FileStream fs = new FileStream(_deviceFileName, FileMode.Create);
            for (int iCount = 1; ; iCount++)
            {
                if (iCount == 1)
                {
                    ReadLoggingStartBytes();
                    //ReceiveFrameTest(UP);
                    _serialNoOfByteTORead += 2;
                    int tempID = _serialSendBuff[0];
                    tempID = tempID & 0x0F;
                    _deviceTotalFrames = tempID == 3 ? (uint)(20 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : tempID == 1 ? (uint)(5 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : tempID == 2 ? (uint)(10 * (1024 * 1024) + 256 * 1024) / (1024 * 8) : (uint)(2 * (1024 * 1024) + 256 * 1024) / (1024 * 8);
                    switch (tempID)
                    {
                        case 0:
                            _deviceTotalFrames = 512 * 1024; //256+256 //Mem settings + 256
                            break;
                        case 1:
                            _deviceTotalFrames = 768 * 1024; //512+256
                            break;
                        case 2:
                            _deviceTotalFrames = 1280 * 1024; //1024+256
                            break;
                        case 3:
                            _deviceTotalFrames = 2304 * 1024; //2048+256
                            break;
                        #region Sammed_DataLogger_Memorysize
                        case 4:
                            _deviceTotalFrames = 4352 * 1024; //4096+256 
                            break;
                        case 5:
                            _deviceTotalFrames = 8448 * 1024; //8192+256
                            break;
                        case 6:
                            _deviceTotalFrames = 12544 * 1024; //12288+256
                            break;
                        case 7:
                            _deviceTotalFrames = 16640 * 1024; //16384+256
                            break;
                        case 8:
                            _deviceTotalFrames = 20736 * 1024; //20840+256
                            break;
                            #endregion

                    }

                    if (_serialSendBuff[0] > 0x0f)
                        _deviceTotalFrames -= 256 * 1024;
                    //_deviceTotalFrames = (uint)0x520;                    
                    bytesRead = _serialNoOfByteRead - 3;
                }
                else
                {
                    ReceiveFrameTest(UP);
                    bytesRead = _serialSendBuff[0];
                    bytesRead += _serialSendBuff[1] * 256;
                }

                if (_serialSendBuff[0] == 0xdd)
                    break;

                //if (bytesRead > RECVFRAMESIZE)
                //bytesRead = RECVFRAMESIZE + 2;

                //CalculateCRC(bytesRead);

                if (iCount < 2)
                    fs.Write(_serialSendBuff, 1, bytesRead + 1);
                else
                    fs.Write(_serialSendBuff, 3, bytesRead);

                Thread.Sleep(500);
                _serialSendBuff[0] = 0x01;
                _serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);

                if (iCount >= _deviceTotalFrames)
                    break;
            }

            ReceiveFrame(DOWN);
            ReceiveFrame(DOWN);
            _serialSendBuff[0] = 0xdd;
            _serialSendBuff[1] = 0x55;
            _serialPort.Write(_serialSendBuff, 0, 2);
            fs.Close();
            CommonConstants.communicationStatus = 0;
            return;
        }

        private void ReadLoggingStartBytes()
        {
            int bytesRead = 0;
            //            while (true)
            //          {
            //                ReceiveFrameTest(DOWN);
            //                if (_serialRecvBuff[0] == 0xAA) break;
            //            }

            ReceiveFrameTest(DOWN);
            bytesRead = _serialRecvBuff[0];

            ReceiveFrameTest(DOWN);
            bytesRead += _serialRecvBuff[0] * 256;
            _serialNoOfByteTORead = bytesRead + 2;

            ReceiveFrameTest(UP);
        }

        public Thread MyThrd
        {
            get
            {
                return _serialThread;
            }
            set
            {
                _serialThread = value;
            }
        }
        public Thread MyThrd1
        {
            get
            {
                return _serialThread1;
            }
            set
            {
                _serialThread1 = value;
            }
        }

        #endregion

        #region Private Methods
        // Attention
        // why 140 specific coding in this routine ??
        // Please check

        /// <summary>
        /// This function prepares the setup frame required for initiating communication.
        /// Setup frame contain the information like file type, device type, file length etc.
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        protected int GetSetFrameIntoBuff(int pType)
        {
            LogWriter.WriteToFile("Serial.cs-> GetSetFrameIntoBuff()", "started-----", "RTC_Upgrade");
            //DATAFRAMESIZE = 256;
            uint tmp;
            int index = 0;
            int iTempDataFrameSize = 256;
            int oneByte = 8;  // In bits
            int twoBytes = 16; // In Bits
            int threeBytes = 24; // In Bits
            int sizeOfLength = 4;  // Size of _deviceLength vari. is 4 Bytes
            byte[] bytes = new byte[sizeOfLength];
            FileStream fs = null;//FlexiSoft_IEC_Mngr_1255_AD

            try
            {
                fs = new FileStream(_deviceFileName, FileMode.Open);//FlexiSoft_IEC_Mngr_1255_AD

                if (_deviceFileID != SERIAL_APPLICATION_DNLD)      //	If No Application read file length
                    _deviceLength = (uint)new FileInfo(_deviceFileName).Length;
                else		                                        //	If it is an application, read length from 1st 4 bytes of the file
                {
                    fs.Read(bytes, 0, sizeOfLength);
                    _deviceLength = 0;
                    _deviceLength = bytes[0];					    //	LByte of LWord 
                    _deviceLength += (uint)bytes[1] << oneByte;		    //	HByte of LWord 
                    _deviceLength += (uint)bytes[2] << twoBytes;		//	LByte of HWord
                    _deviceLength += (uint)bytes[3] << threeBytes;	    //	HByte of Hword
                }
                tmp = _deviceLength;
                _deviceTotalFrames = (uint)(_deviceLength / (DATAFRAMESIZE - FRAMEDIFFERENCE));//Total No of Frames
                _deviceTotalFrames = ((_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE)) > 0) ? _deviceTotalFrames + 1 : _deviceTotalFrames;

                _serialSendBuff.SetValue((byte)STARTFRAMESIZE, index++);

                //This code comes in picture only in case of expansion firmware upgrade
                if (_arrdwnlsetupframe[9] != 0)
                    _deviceProdID = _arrdwnlsetupframe[8];
                //End

                if (_deviceFileID == SERIAL_APPLICATION_DNLD)//	If No Application read file length
                {
                    fs.Seek(0L, System.IO.SeekOrigin.Begin);
                    fs.Read(_serialSendBuff, 0, 10);

                    _deviceProdID = _serialSendBuff[4];
                    _deviceProdID = Convert.ToInt16(_deviceProdID + (_serialSendBuff[5] << oneByte));

                    if (_deviceProdID == CommonConstants.PRODUCT_PRIZM140_EV3)
                        _deviceProdID = Convert.ToInt16(CommonConstants.PRODUCT_PRIZM140);
                    _serialSendBuff.SetValue((byte)STARTFRAMESIZE, index - 1);
                }
                {
                    //_serialSendBuff.SetValue((byte)_deviceProdID, index++);      //Samir
                    _serialSendBuff.SetValue((byte)CommonConstants.NodeAddress, index++);
                    //_serialSendBuff.SetValue((byte)(256 / iTempDataFrameSize), index++);
                    _serialSendBuff.SetValue((byte)(CommonConstants.SetFileType), index++);
                }
                if (_serialFileID != 0)
                    _serialSendBuff.SetValue((byte)1, index++);
                else
                    _serialSendBuff.SetValue((byte)0, index++);

                //FP_CODE Pravin Download Mode
                if (_arrdwnlsetupframe[1] == 1)
                    _serialSendBuff.SetValue((byte)1, index++);
                else
                    _serialSendBuff.SetValue((byte)0, index++);

                if (_arrdwnlsetupframe[2] == 1)
                    _serialSendBuff.SetValue((byte)1, index++);
                else
                    _serialSendBuff.SetValue((byte)0, index++);

                if (_arrdwnlsetupframe[3] == 1)
                    _serialSendBuff.SetValue((byte)1, index++);
                else
                    _serialSendBuff.SetValue((byte)0, index++);

                if (_arrdwnlsetupframe[4] == 1)
                    _serialSendBuff.SetValue((byte)1, index++);
                else
                    _serialSendBuff.SetValue((byte)0, index++);

                for (int i = 0; i < 2; i++)	//	These 4 Bytes r Not Used
                    _serialSendBuff.SetValue((byte)0, index++);
                //End          

                _serialSendBuff.SetValue((byte)tmp, index++);
                _serialSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), index++);
                _serialSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), index++);
                _serialSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), index++);

                for (int i = 0; i < 6; i++)	//	These 6 Bytes r not used
                    _serialSendBuff.SetValue((byte)0, index++);
            }
            catch (UnauthorizedAccessException ex)//FlexiSoft_IEC_Mngr_1255_AD
            {
                LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                _deviceFileInUse = true;
                return FAILURE;
            }
            catch (IOException ioEX)//VOffice_IssueNo_304
            {
                LogWriter.WriteToFile(ioEX.StackTrace, ioEX.Message, "RTC_Upgrade");
                _deviceFileInUse = true;
                return FAILURE;
            }
            catch (Exception e)//VOffice_IssueNo_304
            {
                LogWriter.WriteToFile(e.StackTrace, e.Message, "RTC_Upgrade");
                return FAILURE;
            }
            finally
            {
                if (fs != null)//FlexiSoft_IEC_Mngr_1255_AD
                    fs.Close();
            }
            //DATAFRAMESIZE = 60;
            return SUCCESS;
        }

        protected int GetSetFrameIntoBuff()
        {
            //DATAFRAMESIZE = 256;
            uint tmp;
            int index = 0;
            int iTempDataFrameSize = 256;
            int oneByte = 8;  // In bits
            int twoBytes = 16; // In Bits
            int threeBytes = 24; // In Bits
            int sizeOfLength = 4;  // Size of _deviceLength vari. is 4 Bytes
            byte[] bytes = new byte[sizeOfLength];

            tmp = _deviceLength;
            _deviceTotalFrames = (uint)(_deviceLength / (DATAFRAMESIZE - FRAMEDIFFERENCE));//Total No of Frames
            _deviceTotalFrames = ((_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE)) > 0) ? _deviceTotalFrames + 1 : _deviceTotalFrames;

            _serialSendBuff.SetValue((byte)STARTFRAMESIZE, index++);

            //FP_CODE  R12  Haresh
            //if (CommonConstants.IsProductFlexiPanels(ClassList.CommonConstants.ProductDataInfo.iProductID))
            // _deviceProdID = (short)ClassList.CommonConstants.ProductDataInfo.iProductID;
            //End

            //This code comes in picture only in case of expansion firmware upgrade
            if (_arrdwnlsetupframe[9] != 0)
                _deviceProdID = _arrdwnlsetupframe[8];
            //End            

            _serialSendBuff.SetValue((byte)_deviceProdID, index++);      //Samir
            _serialSendBuff.SetValue((byte)(_deviceProdID / iTempDataFrameSize), index++);


            //FP_CODE  R12  Haresh
            //Write 1 or 0 in setup Frame
            if (_serialFileID != 0)
                _serialSendBuff.SetValue((byte)1, index++);
            else
                _serialSendBuff.SetValue((byte)0, index++);
            //Write 1 or 0 in setup Frame

            //FP_CODE Pravin Download Mode
            if (_arrdwnlsetupframe[1] == 1)
                _serialSendBuff.SetValue((byte)1, index++);
            else
                _serialSendBuff.SetValue((byte)0, index++);

            if (_arrdwnlsetupframe[2] == 1)
                _serialSendBuff.SetValue((byte)1, index++);
            else
                _serialSendBuff.SetValue((byte)0, index++);

            if (_arrdwnlsetupframe[3] == 1)
                _serialSendBuff.SetValue((byte)1, index++);
            else
                _serialSendBuff.SetValue((byte)0, index++);

            if (_arrdwnlsetupframe[4] == 1)
                _serialSendBuff.SetValue((byte)1, index++);
            else
                _serialSendBuff.SetValue((byte)0, index++);

            for (int i = 0; i < 2; i++)	//	These 4 Bytes r Not Used
                _serialSendBuff.SetValue((byte)0, index++);
            //End          


            //Commented
            /* for (int i = 0; i < 7; i++)	//	These 7 Bytes r Not Used
                 _serialSendBuff.SetValue((byte)0, index++);
             */

            _serialSendBuff.SetValue((byte)tmp, index++);
            _serialSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), index++);
            _serialSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), index++);
            _serialSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), index++);

            for (int i = 0; i < 6; i++)	//	These 6 Bytes r not used
                _serialSendBuff.SetValue((byte)0, index++);

            return SUCCESS;
        }

        /// <summary>
        /// This Function sends a frame of data to the connected port of specified number of bytes 
        /// of perticular no of frame. 
        ///	The parameter pDelay specifies the time to wait for the acknowledgement signal
        ///	The pFrameNum and pSize specifies what is the no. of the frame to send and the size of
        /// that frame in bytes resp.
        /// </summary>
        /// <param name="pFrameNum"></param>
        /// <param name="pSize"></param>
        /// <param name="pDelay"></param>

        private void SendFrame1(int pFrameNum, int pSize, int pDelay)
        {
            if (pSize == 1)		//	If want to send an READY signal
            {
                if (_serialNoOfSent >= NO_OF_TRY_FOR_READY_PRIZM)
                    return;

                _serialSendBuff[0] = _deviceFileID;
                _serialNoOfSent++;
            }
            else		//	If want to send a DATA Frame
            {
                _serialSendBuff[0] = (byte)pSize;			//	First 3 bytes of data frame contains
                _serialSendBuff[1] = (byte)pFrameNum;	//  Size if Frame(1 Byte) and 
                _serialSendBuff[2] = (byte)(pFrameNum / DATAFRAMESIZE);	// No.of Frame(2 Bytes)
                _serialSendBuff[pSize + DATA_START_INDEX] = _serialCrcByte;	// Lastly Add here CRC byte
                pSize += (DATA_START_INDEX + CRC_BYTE_SIZE - 4);
            }
            _serialPort.Write(_serialSendBuff, 3, pSize);
            _serialSendBuff[0] = 0;
            _serialATimer = new System.Timers.Timer(pDelay);
            _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _serialATimer.Enabled = true;
        }

        private void SendFrame1(int pFrameNum, int pSize)
        {
            _serialSendBuff[0] = (byte)pSize;			//	First 3 bytes of data frame contains
            _serialSendBuff[1] = (byte)pFrameNum;	//  Size if Frame(1 Byte) and 
            _serialSendBuff[2] = (byte)(pFrameNum / DATAFRAMESIZE);	// No.of Frame(2 Bytes)
            _serialSendBuff[pSize + DATA_START_INDEX] = _serialCrcByte;	// Lastly Add here CRC byte
            pSize += (DATA_START_INDEX + CRC_BYTE_SIZE - 4);
            _serialPort.Write(_serialSendBuff, 3, pSize);
            _serialSendBuff[0] = 0;
        }

        private void SendFrame(int pFrameNum, int pSize, int pDelay)
        {
            try
            {
                if (pSize == 1)		//	If want to send an READY signal
                {
                    if (_serialNoOfSent >= NO_OF_TRY_FOR_READY_PRIZM)
                        return;

                    _serialSendBuff[0] = _deviceFileID;
                    _serialNoOfSent++;
                }
                else		//	If want to send a DATA Frame
                {
                    _serialSendBuff[0] = (byte)pSize;			//	First 3 bytes of data frame contains
                    _serialSendBuff[1] = (byte)pFrameNum;	//  Size if Frame(1 Byte) and 
                    _serialSendBuff[2] = (byte)(pFrameNum / DATAFRAMESIZE);	// No.of Frame(2 Bytes)
                    _serialSendBuff[pSize + DATA_START_INDEX] = _serialCrcByte;	// Lastly Add here CRC byte
                    pSize += (DATA_START_INDEX + CRC_BYTE_SIZE);
                }
                _serialPort.Write(_serialSendBuff, 0, pSize);
                _serialSendBuff[0] = 0;
                _serialATimer = new System.Timers.Timer(pDelay);
                _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                _serialATimer.Enabled = true;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Serial.cs->SendFrame() error", ex.Message, "RTC_Upgrade");
                MessageBox.Show("Device not responding");
            }
        }


        /// <summary>
        /// This Function sends a frame of data to the connected port of specified number of bytes 
        /// of perticular no of frame. 
        ///	The parameter pDelay specifies the time to wait for the acknowledgement signal
        ///	The pFrameNum and pSize specifies what is the no. of the frame to send and the size of
        /// that frame in bytes resp.
        /// </summary>
        /// <param name="pFrameNum"></param>
        /// <param name="pSize"></param>
        /// <param name="pDelay"></param>
        private void SendFrame(int pFrameNum, int pSize)
        {
            _serialSendBuff[0] = (byte)pSize;			//	First 3 bytes of data frame contains
            _serialSendBuff[1] = (byte)pFrameNum;	//  Size if Frame(1 Byte) and 
            _serialSendBuff[2] = (byte)(pFrameNum / DATAFRAMESIZE);	// No.of Frame(2 Bytes)
            _serialSendBuff[pSize + DATA_START_INDEX] = _serialCrcByte;	// Lastly Add here CRC byte
            pSize += (DATA_START_INDEX + CRC_BYTE_SIZE);
            _serialPort.Write(_serialSendBuff, 0, pSize);
            _serialSendBuff[0] = 0;
        }

        /// <summary>
        /// This function recieves a frame of data whose size is specifed, within a limited span of time.
        /// If it get time out it will return without receiving frame.
        ///	The function return true if the frames received successfully
        ///	otherwise returns false on failure.
        /// </summary>
        /// <returns></returns>
        private bool ReceiveFrame()
        {
           
            _serialTimeOut = 0;
            //_serialPort.ReadTimeout = ONEFIFTY_SEC_DELAY; // THRTEE_SEC_DELAY;  // KA 0920
            _serialPort.ReadTimeout = 30000;// SEVEN_SEC_DELAY;  //KA  //50000 as per ritesh change
           
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
            if (_serialPort != null && _serialPort.IsOpen) // KA 0910
            {
                _serialPort.ReadTimeout = 30000;  //50000 as per ritesh change
                LogWriter.WriteToFile("Serial.cs->ReceiveFrame() timeout", _serialPort.ReadTimeout.ToString(), "RTC_Upgrade");
               // while (_serialTimeOut == 0)
                {
                    if (_serialRecvBuff != null)
                    {
                        _serialRecvBuff[0] = 0;
                        _serialReceivedByte = 0;
                        try
                        {
                            _serialNoOfByteRead = _serialPort.BaseStream.Read(_serialRecvBuff, 0, 1);

                            //Keeran (KA)
                            //copied below if block from outside of try..catch, here
                            if (_serialNoOfByteRead > 0)
                            {
                                _serialReceivedByte = _serialRecvBuff[0];
                                return true;
                            }
                            else
                            {
                                _serialRecvBuff[0] = 0;
                                return false;
                            }
                        }
                        catch (System.TimeoutException timeOut)
                        {
                            _serialRecvBuff[0] = 0;
                            return false;
                        }
                        catch(Exception exx)
                        {
                            LogWriter.WriteToFile("Serial.cs->Receive frame()", exx.Message, "RTC_Upgrade");
                        }
                    }
                }
            }
            return false;
        }

        private bool ReceiveFrame11()
        {
            bool connn = false;
            
           // while (!connn)
            {
               // System.Threading.Thread.Sleep(20);
                _serialTimeOut = 0;
                //_serialPort.ReadTimeout = ONEFIFTY_SEC_DELAY; // THRTEE_SEC_DELAY;  // KA 0920
                _serialPort.ReadTimeout = 50000;// SEVEN_SEC_DELAY;  //KA
               
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }
                if (_serialPort != null && _serialPort.IsOpen) // KA 0910
                {
                    _serialPort.ReadTimeout = 50000;
                    LogWriter.WriteToFile("Serial.cs->ReceiveFrame() timeout", _serialPort.ReadTimeout.ToString(), "RTC_Upgrade");
                    // while (_serialTimeOut == 0)
                    {
                        if (_serialRecvBuff != null)
                        {
                            _serialRecvBuff[0] = 0;
                            _serialReceivedByte = 0;
                            try
                            {
                                _serialNoOfByteRead = _serialPort.BaseStream.Read(_serialRecvBuff, 0, 1);
                                connn = true;

                                //Keeran (KA)
                                //copied below if block from outside of try..catch, here
                                if (_serialNoOfByteRead > 0)
                                {
                                    _serialReceivedByte = _serialRecvBuff[0];
                                    return true;
                                }
                                else
                                {
                                    _serialRecvBuff[0] = 0;
                                    return false;
                                }
                            }
                            //catch (System.TimeoutException timeOut)
                            //{
                            //    _serialRecvBuff[0] = 0;
                            //    return false;
                            //}
                            catch (Exception exx)
                            {
                                connn = false;
                                LogWriter.WriteToFile("Serial.cs->Receive frame()", exx.Message, "RTC_Upgrade");
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            //if (!this.disposed)
            //{
                if (disposing)
                    this._serialPort.Dispose();

            //    this.disposed = true;
            //}
        }

        private bool ReceiveFrame_SetupFrameResponse()
        {
           // Thread.Sleep(50);
            _serialTimeOut = 0;
            #region Issue2.2_387 Vijay
            //_serialPort.ReadTimeout = THRTEE_SEC_DELAY;
            if (ClassList.CommonConstants.IsProductFL005MicroPLCBase(ClassList.CommonConstants.ProductDataInfo.iProductID))
            {
                _serialPort.ReadTimeout = THREE_SEC_DELAY;
            }
            else
            {
                _serialPort.ReadTimeout = THRTEE_SEC_DELAY;
            }
            #endregion

            while (_serialTimeOut == 0)
            {
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;
                try
                {
                    _serialNoOfByteRead = _serialPort.Read(_serialRecvBuff, 0, 1);
                }
                catch (System.TimeoutException timeOut)
                {
                    _serialRecvBuff[0] = 0;
                    return false;
                }
                if (_serialNoOfByteRead > 0)
                {
                    _serialReceivedByte = _serialRecvBuff[0];
                    return true;
                }
                else
                {
                    _serialRecvBuff[0] = 0;
                }
            }
            return false;
        }


        private bool ReceiveFrameTEN()
        {
            _serialTimeOut = 0;
            _serialPort.ReadTimeout = TEN_SEC_DELAY;
            while (_serialTimeOut == 0)
            {
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;
                try
                {
                    _serialNoOfByteRead = _serialPort.Read(_serialRecvBuff, 0, 1);
                }
                catch (System.TimeoutException timeOut)
                {
                    _serialRecvBuff[0] = 0;
                    return false;
                }
                if (_serialNoOfByteRead > 0)
                {
                    _serialReceivedByte = _serialRecvBuff[0];
                    return true;
                }
                else
                {
                    _serialRecvBuff[0] = 0;
                }
            }
            return false;
        }

        /// <summary>
        /// Overloaded for Uploading
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool ReceiveFrame(int size)
        {
            _serialTimeOut = 0;

            if (size == DOWN)
            {
                _serialPort.ReadTimeout = THREE_SEC_DELAY * 2;
                _serialTimeOut = 0;
                while (true)
                {
                    _serialRecvBuff[0] = 0;
                    _serialReceivedByte = 0;

                    try
                    {
                        _serialNoOfByteRead = _serialPort.Read(_serialRecvBuff, 0, ACKFRAMESIZE);
                    }
                    catch (System.TimeoutException timeOut)
                    {
                        //_serialRecvBuff[0] = 0;
                        return false;
                    }

                    if (_serialNoOfByteRead > 0)
                    {
                        _serialReceivedByte = _serialRecvBuff[0];
                        return true;
                    }
                }
                return false;
            }
            else if (size == 12)
            {
                _serialPort.ReadTimeout = TEN_SEC_DELAY;
                _serialTimeOut = 0;
                while (_serialTimeOut == 0)
                {
                    _serialRecvBuff[0] = 0;
                    _serialReceivedByte = 0;

                    try
                    {
                        _serialNoOfByteRead = _serialPort.Read(_serialRecvBuff, 0, size);
                    }
                    catch (System.TimeoutException timeOut)
                    {
                        _serialRecvBuff[0] = 0;
                        return false;
                    }

                    if (_serialNoOfByteRead > 0)
                    {
                        _serialReceivedByte = _serialRecvBuff[0];
                        return true;
                    }
                }
                return false;
            }
            //else if (_serialFileID == SERIAL_LOGGEDDATA_UPLD)
            else if ((_serialFileID == SERIAL_LOGGEDDATA_UPLD) || (_serialFileID == SERIAL_HIST_ALARM_DATA_UPLD))   //manik
            {
                _serialTimeOut = 0;
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;
                _serialNoOfByteRead = 0;
                _serialPort.ReadTimeout = TEN_SEC_DELAY * 2;
                _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                _serialATimer.Enabled = true;
                try
                {
                    int i = 0;
                    while (_serialTimeOut == 0 || _serialNoOfByteRead < _serialNoOfByteTORead)
                    {
                        //Thread.Sleep(0);
                        //if (_serialNoOfByteRead + 1024 < _serialNoOfByteTORead)
                        //    _serialNoOfByteRead += _serialPort.Read(_serialSendBuff, _serialNoOfByteRead, 1024);
                        //else
                        _serialNoOfByteRead += _serialPort.Read(_serialSendBuff, _serialNoOfByteRead, _serialNoOfByteTORead - _serialNoOfByteRead);

                        if (_serialNoOfByteRead >= _serialNoOfByteTORead) //|| _serialSendBuff[0] + (_serialSendBuff[1] << 8) <= _serialNoOfByteRead)
                            return true;
                    }
                    return true;
                }
                catch (System.TimeoutException timeOut)
                {
                    //_deviceDownloadStatus(9);
                    return false;
                }
                return false;
            }
            else
            {
                _serialTimeOut = 0;
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;
                _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                _serialATimer.Enabled = true;
                //FlexiSoft2_2_Issue_no_546_AMIT
                _serialNoOfByteRead = 0;
                _serialTimeOutFlag = false;
                _serialATimer.AutoReset = false;
                _serialATimer.Elapsed += new ElapsedEventHandler(_serialATimer_Elapsed);
                //End
                //_serialPort.ReadTimeout = TEN_SEC_DELAY * 2;
                try
                {
                    #region FlexiSoft2_2_Issue_no_546_AMIT
                    /*while (_serialTimeOut == 0)
                    {
                        Thread.Sleep(100);


                        _serialNoOfByteRead = _serialPort.Read(_serialSendBuff, 0, 256);
                        _serialNoOfByteRead += _serialPort.Read(_serialSendBuff, 256, 256);
                        Thread.Sleep(100);

                        _serialNoOfByteRead += _serialPort.Read(_serialSendBuff, 512, 256);
                        _serialNoOfByteRead += _serialPort.Read(_serialSendBuff, 768, 235);

                        if (_serialNoOfByteRead >= 1003)

                            return true;
                    }*/
                    _serialATimer.Start();
                    while (_serialPort.BytesToRead != 1003 && !_serialTimeOutFlag)
                    {
                        Thread.Sleep(100);
                    }

                    _serialNoOfByteRead = _serialPort.Read(_serialSendBuff, 0, 256);
                    _serialNoOfByteRead += _serialPort.Read(_serialSendBuff, 256, 256);

                    _serialNoOfByteRead += _serialPort.Read(_serialSendBuff, 512, 256);
                    _serialNoOfByteRead += _serialPort.Read(_serialSendBuff, 768, 235);

                    if (_serialNoOfByteRead >= 1003)
                    {
                        _serialATimer.Stop();
                        return true;
                    }
                    #endregion
                }
                catch (System.TimeoutException timeOut)
                {
                    //_deviceDownloadStatus(9);
                    return false;
                }
                return false;
            }
        }

        //FlexiSoft2_2_Issue_no_546_AMIT
        private void _serialATimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _serialTimeOutFlag = true;
        }
        //End

        private bool ReceiveFrame_Size(int size)
        {
            _serialPort.ReadTimeout = THREE_SEC_DELAY;
            while (true)
            {
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;

                try
                {
                    _serialNoOfByteRead = _serialPort.Read(_serialRecvBuff, 0, size);
                }
                catch (System.TimeoutException timeOut)
                {
                    return false;
                }

                if (_serialNoOfByteRead > 0)
                {
                    _serialReceivedByte = _serialRecvBuff[0];
                    return true;
                }
            }
            return false;

        }

        private bool ReceiveFrameTest(int size)
        {
            try
            {
                _serialTimeOut = 0;
                if (size == DOWN)
                {
                    _serialPort.ReadTimeout = THREE_SEC_DELAY * 2;
                    _serialTimeOut = 0;
                    while (true)
                    {
                        _serialRecvBuff[0] = 0;
                        _serialReceivedByte = 0;

                        _serialNoOfByteRead = _serialPort.Read(_serialRecvBuff, 0, ACKFRAMESIZE);

                        if (_serialNoOfByteRead > 0)
                        {
                            _serialReceivedByte = _serialRecvBuff[0];
                            return true;
                        }
                    }
                }
                else if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                {
                    _serialTimeOut = 0;
                    _serialRecvBuff[0] = 0;
                    _serialReceivedByte = 0;
                    _serialNoOfByteRead = 0;
                    _serialPort.ReadTimeout = TEN_SEC_DELAY * 2;

                    while (_serialTimeOut == 0 || _serialNoOfByteRead < _serialNoOfByteTORead)
                    {
                        _serialNoOfByteRead += _serialPort.Read(_serialSendBuff, _serialNoOfByteRead, _serialNoOfByteTORead - _serialNoOfByteRead);

                        if (_serialNoOfByteRead >= _serialNoOfByteTORead) //|| _serialSendBuff[0] + (_serialSendBuff[1] << 8) <= _serialNoOfByteRead)
                            return true;
                    }
                }
                #region AdvancedAlarm
                else if (_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD)
                {
                    _serialTimeOut = 0;
                    _serialRecvBuff[0] = 0;
                    _serialReceivedByte = 0;
                    _serialNoOfByteRead = 0;
                    _serialPort.ReadTimeout = TEN_SEC_DELAY * 2;

                    while (_serialTimeOut == 0 || _serialNoOfByteRead < _serialNoOfByteTORead)
                    {
                        _serialNoOfByteRead += _serialPort.Read(_serialSendBuff, _serialNoOfByteRead, _serialNoOfByteTORead - _serialNoOfByteRead);

                        if (_serialNoOfByteRead >= _serialNoOfByteTORead) //|| _serialSendBuff[0] + (_serialSendBuff[1] << 8) <= _serialNoOfByteRead)
                            return true;
                    }
                }
                #endregion

                return false;
            }
            catch (System.TimeoutException timeout)
            {
                //_deviceDownloadStatus(9);                
                return false;
            }
        }

        /// <summary>
        /// This function calculates CRC of a frame, and total frames as well.
        /// This calculated CRC of frame of data is being sent along with the data.
        /// Where at the receiving end it receives it and perform error checking.
        ///	If in communication some of the data gets lost in transfer, prizm unit returns 
        /// error as ack frame, as a reTransmitte signal.
        /// </summary>
        /// <param name="pNum"></param>
        protected void CalculateCRC(int pNum)
        {
            _serialCrcByte = _serialSendBuff[DATA_START_INDEX];
            //for (int i = 1; _serialSendBuff.Length > i + DATA_START_INDEX; i++)
            for (int i = 1; i < pNum; i++)
                _serialCrcByte ^= _serialSendBuff[i + DATA_START_INDEX];
            _serialTotalCrcByte ^= _serialCrcByte;
        }
        protected void CalculateCRC1(int pNum)
        {
            //  _serialCrcByte = _serialSendBuff[DATA_START_INDEX];
            //for (int i = 1; _serialSendBuff.Length > i + DATA_START_INDEX; i++)
            for (int i = 0; i < pNum; i++)
                _serialCrcByte ^= _serialSendBuff[i + DATA_START_INDEX];
            _serialTotalCrcByte = _serialCrcByte;
        }

        /// <summary>
        /// If reply(ACK) from the prizm unit doesn't reach within specified time span this function is 
        /// invoked, which generates time up signal for the class to get know and take the proper 
        /// action according to conditions.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _serialTimeOut = 0;
        }

        /// <summary>
        /// Checks if prizm is ready to communicate, for downloading and uploading
        /// </summary>
        private int IsPrizmReady()
        {
            int retVal = FAILURE;
            _serialNoOfSent = 0;
            //_deviceDownloadStatus(0);

            //Kapil_Issue_integration_#1484
            if (_serialPort.IsOpen)
            {
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
            }

            int tempDelay = 0;
            if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                tempDelay = 0;

            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM + tempDelay; i++)
            {
                SendFrame(1, CRC_BYTE_SIZE, THREE_SEC_DELAY);
                //Thread.Sleep(200); 
                if (ReceiveFrame())
                {
                    //Kapil_Issue_integration_#1484
                    if (_serialRecvBuff[0] != _deviceFileID + 1 && _serialRecvBuff[0] != _deviceFileID + 2)
                    {
                        Thread.Sleep(3000);
                        continue;
                    }
                    _serialATimer.Enabled = false;
                    i = NO_OF_TRY_FOR_READY_PRIZM;
                    retVal = SUCCESS;
                    break;
                }
                _serialTimeOut = 0;
                _serialATimer.Enabled = false;
            }
            return retVal;
        }

        /// <summary>
        /// If prizm is ready to communicate then start communication by sending  
        /// the set frame
        /// </summary>
        /// <returns></returns>
        /// 

        private int InitiateCommunication()
        {
            LogWriter.WriteToFile("Serial.cs- InitiateCommunication()", "started", "RTC_Upgrade");
            #region FP_Ethernet_Implementation-AMIT
            int message = 256;
            #endregion
            if (_serialReceivedByte == _deviceFileID || _serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_1 || _serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2)
            {	//	If Prizm Ready, Send here setFrame
                _serialTimeOut = 0;

                a: if (_serialReceivedByte == _deviceFileID + 2)
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);

                    LogWriter.WriteToFile("Serial.cs-> InitiateComm", string.Join(",", _serialSendBuff), "RTC_Upgrade");
                    _serialPort.Write(_serialSendBuff, 0, 4);
                    //Thread.Sleep(1000);
                    //_serialPort.Write(_serialSendBuff, 0, STARTFRAMESIZE);
                    _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                    _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    _serialATimer.Enabled = true;
                    ReceiveFrame();

                    if (_serialRecvBuff[0] == 0)
                    {
                        _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY * 3);
                        _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                        _serialATimer.Enabled = true;
                        ReceiveFrame();
                    }
                    if (_serialReceivedByte == _deviceFileID + 2)
                        goto a;
                }
                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                {

                    #region FP_Ethernet_Implementation-AMIT
                    message <<= 7;
                    _deviceDownloadStatus(3 + message);
                    GetSetFrameIntoBuffForEthSettings(DOWN);//New Code
                    LogWriter.WriteToFile("Serial.cs-> InitiateComm()1", string.Join(",", _serialSendBuff), "RTC_Upgrade");
                    _serialPort.Write(_serialSendBuff, 0, STARTFRAMESIZE);
                    Thread.Sleep(200);
                    ReceiveFrame();

                    if (_serialRecvBuff[0] == 0)
                    {
                        _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY * 3);
                        _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                        _serialATimer.Enabled = true;
                        ReceiveFrame();
                    }
                    else if (_serialRecvBuff[0] == 0x1)
                    {
                        GetEtherSettingFrame(false);
                        _serialTotalCrcByte = _serialBCC;
                        Thread.Sleep(200);
                        _serialPort.Write(_serialSendBuff, 0, 0x16);
                        Thread.Sleep(200);

                        _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                        _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                        _serialATimer.Enabled = true;
                    }
                    #endregion
                }
                else
                {
                    if (GetSetFrameIntoBuff(DOWN) == CommonConstants.FAILURE)//FlexiSoft_IEC_Mngr_1255_AD
                    {
                        if (_deviceFileInUse)
                        {
                            CommonConstants.communicationStatus = 0;
                            if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorFirmwareFileCreation));
                            if (_deviceFileID == CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
                                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorApplicationFileCreation));
                            Close();
                        }
                        return FAILURE;
                    }
                    LogWriter.WriteToFile("Serial.cs-> InitiateComm()2", string.Join(",", _serialSendBuff), "RTC_Upgrade");
                    _serialPort.Write(_serialSendBuff, 0, STARTFRAMESIZE);
                }
                _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY * 3);
                _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                _serialATimer.Enabled = true;
                ReceiveFrame_SetupFrameResponse();
                if (_serialReceivedByte == 0)
                    ReceiveFrame_SetupFrameResponse();
                _serialATimer.Enabled = false;
                if (_serialReceivedByte == PRODUCT_MISSMATCH)	//	if '1' received = OK received
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadStatus(111);
                    //_deviceDownloadStatus(5);
                    Close();
                    return FAILURE;
                }
                #region FP_Ethernet_Implementation-AMIT
                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                {
                    if (_serialReceivedByte == 168 || _serialReceivedByte == 0x01)
                    {
                        _deviceDownloadPercentage(100);
                        return SUCCESS;
                    }
                }
                #endregion
                if (_serialReceivedByte != CORRECT_CRC)	//	if '1' received = OK received
                    if (_serialReceivedByte != 0xef)
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        //_deviceDownloadStatus(6);
                        Close();
                        return FAILURE;
                    }
                if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                {

                    Thread.Sleep(1000);
                    _serialSendBuff[0] = 0xB8;
                    LogWriter.WriteToFile("Serial.cs-> InitiateComm()", string.Join(",", _serialSendBuff), "RTC_Upgrade");
                    _serialPort.Write(_serialSendBuff, 0, 1);
                    _serialSendBuff.SetValue((Byte)0x60, 0);
                    _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                    _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    _serialATimer.Enabled = true;
                    //ReceiveFrame();
                    ReceiveFrameTEN();
                    _serialATimer.Enabled = false;
                    _serialSendBuff[0] = 0x38;
                    _serialSendBuff[1] = 0x0;
                    _serialSendBuff[2] = 0x0;
                }
            }
            //Ladder_change_R11
            else if (_serialReceivedByte == _deviceFileID + 3)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                Close();
                return FAILURE;
            }
            //End
            else // Unit Not Ready
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadStatus(8);
                Close();
                return FAILURE;
            }

            #region FP_Ethernet_Implementation-AMIT
            //int message = 256;
            #endregion
            if (_deviceFileID == CommonConstants.SERIAL_LADDER_DNLD_FILEID)
                message <<= 2;
            if (_deviceFileID == CommonConstants.SERIAL_FONT_DNLD_FILEID)
                message <<= 3;
            if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                message <<= 1;
            _deviceDownloadStatus(3 + message);

            return SUCCESS;
        }

        private int InitiateCommunication1()
        {
            _deviceDownloadStatus(Convert.ToInt32(ClassList.DownloadingStatusMessages.strBootBlock1));
            #region FP_Ethernet_Implementation-AMIT
            int message = 256;
            #endregion
            if (_serialReceivedByte == _deviceFileID || _serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_1 || _serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2)
            {	//	If Prizm Ready, Send here setFrame
                _serialTimeOut = 0;

                a: if (_serialReceivedByte == _deviceFileID + 2)
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);
                    //   _serialPort.Write(_serialSendBuff, 0, 4);
                    //Thread.Sleep(1000);
                    //_serialPort.Write(_serialSendBuff, 0, STARTFRAMESIZE);
                    _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                    _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    _serialATimer.Enabled = true;
                }
                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                {

                    #region FP_Ethernet_Implementation-AMIT
                    _deviceDownloadStatus(Convert.ToInt32(ClassList.DownloadingStatusMessages.strBootBlock1));
                    GetSetFrameIntoBuffForEthSettings(DOWN);//New Code
                    //_serialPort.Write(_serialSendBuff, 0, STARTFRAMESIZE);
                    Thread.Sleep(200);
                    // ReceiveFrame();

                    if (_serialRecvBuff[0] == 0)
                    {
                        _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY * 3);
                        _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                        _serialATimer.Enabled = true;
                        //  ReceiveFrame();
                    }
                    else if (_serialRecvBuff[0] == 0x1)
                    {
                        GetEtherSettingFrame(false);
                        _serialTotalCrcByte = _serialBCC;
                        Thread.Sleep(200);
                        _serialPort.Write(_serialSendBuff, 0, 0x16);
                        Thread.Sleep(200);

                        _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                        _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                        _serialATimer.Enabled = true;
                    }
                    #endregion
                }
                else
                {
                    if (GetSetFrameIntoBuff(DOWN) == CommonConstants.FAILURE)//FlexiSoft_IEC_Mngr_1255_AD
                    {
                        if (_deviceFileInUse)
                        {
                            CommonConstants.communicationStatus = 0;
                            if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorFirmwareFileCreation));
                            if (_deviceFileID == CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
                                _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorApplicationFileCreation));
                            Close();
                        }
                        return FAILURE;
                    }
                    //_serialPort.Write(_serialSendBuff, 0, STARTFRAMESIZE);
                }
                _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY * 3);
                _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                _serialATimer.Enabled = true;
                // ReceiveFrame_SetupFrameResponse();
                //if (_serialReceivedByte == 0)
                //    ReceiveFrame_SetupFrameResponse();
                return SUCCESS;
                #region CM
                //      _serialATimer.Enabled = false;
                //      if (_serialReceivedByte == PRODUCT_MISSMATCH)	//	if '1' received = OK received
                //      {
                //          CommonConstants.communicationStatus = 0;
                //          _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strProductMismatch));
                //          //_deviceDownloadStatus(5);
                //          Close();
                //          return FAILURE;
                //      }
                //      #region FP_Ethernet_Implementation-AMIT
                //      if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                //      {
                //          if (_serialReceivedByte == 168 || _serialReceivedByte == 0x01)
                //          {
                //              _deviceDownloadPercentage(100);
                //              return SUCCESS;
                //          }
                //      }
                //      #endregion
                //      if (_serialReceivedByte != CORRECT_CRC)	//	if '1' received = OK received
                //          if (_serialReceivedByte != 0xef)
                //          {
                //              CommonConstants.communicationStatus = 0;
                //              _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                //              //_deviceDownloadStatus(6);
                //              Close();
                //              return FAILURE;
                //          }
                //      if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                //      {

                //          Thread.Sleep(1000);
                //          _serialSendBuff[0] = 0xB8;
                //          _serialPort.Write(_serialSendBuff, 0, 1);
                //          _serialSendBuff.SetValue((Byte)0x60, 0);
                //          _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                //          _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                //          _serialATimer.Enabled = true;
                //          //ReceiveFrame();
                //          ReceiveFrameTEN();
                //          _serialATimer.Enabled = false;
                //          _serialSendBuff[0] = 0x38;
                //          _serialSendBuff[1] = 0x0;
                //          _serialSendBuff[2] = 0x0;
                //      }
                //  }
                //  //Ladder_change_R11
                //  else if (_serialReceivedByte == _deviceFileID + 3)
                //  {
                //      CommonConstants.communicationStatus = 0;
                //      _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                //      Close();
                //      return FAILURE;
                //  }
                //  //End
                //  else // Unit Not Ready
                //  {
                //      CommonConstants.communicationStatus = 0;
                //      _deviceDownloadStatus(8);
                //      Close();
                //      return FAILURE;
                //  }

                //  #region FP_Ethernet_Implementation-AMIT
                //  //int message = 256;
                //  #endregion
                //  if (_deviceFileID == CommonConstants.SERIAL_LADDER_DNLD_FILEID)
                //      message <<= 2;
                //  if (_deviceFileID == CommonConstants.SERIAL_FONT_DNLD_FILEID)
                //      message <<= 3;
                //  if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                //      message <<= 1;
                ////  _deviceDownloadStatus(3 + message);
                //  _deviceDownloadStatus(Convert.ToInt32(ClassList.DownloadingStatusMessages.strBootBlock1));
                //  return SUCCESS; 
                #endregion
            }
            return SUCCESS;
        }

        #region FP_Ethernet_Implementation-AMIT
        /// <summary>
        /// calculate bcc
        /// </summary>
        /// <param name="piLength"></param>
        /// <param name="piStartIndex"></param>
        /// <returns></returns>
        protected byte CalculateBCC(int piLength, int piStartIndex)
        {
            byte BCCByte = _serialSendBuff[piStartIndex];
            for (int i = 1; i < piLength; i++)
                BCCByte ^= _serialSendBuff[i + piStartIndex];
            return _serialBCC = BCCByte;
        }

        protected void GetEtherSettingFrame(bool USB)
        {
            uint tmp;
            int index = 0;
            int oneByte = 8;  // In bits
            int twoBytes = 16; // In Bits
            int threeBytes = 24; // In Bits
            int sizeOfLength = 4;  // Size of _deviceLength vari. is 4 Bytes
            byte[] bytes = new byte[sizeOfLength];
            string[] tempStrArr = _serialIPAddress.Split('.');

            //_serialSendBuff.SetValue((byte)0x11, index++);
            //_serialSendBuff.SetValue(Convert.ToByte(0), index++);
            //_serialSendBuff.SetValue(Convert.ToByte(0), index++);
            if (USB)
                _serialSendBuff.SetValue(Convert.ToByte(60), index++);
            else
                _serialSendBuff.SetValue(Convert.ToByte(18), index++);
            _serialSendBuff.SetValue(Convert.ToByte(0), index++);
            _serialSendBuff.SetValue(Convert.ToByte(0), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[0]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[1]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[2]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[3]), index++);

            tempStrArr = _serialSubnetMask.Split('.');
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[0]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[1]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[2]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[3]), index++);

            tempStrArr = _serialDefaultGateway.Split('.');
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[0]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[1]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[2]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[3]), index++);

            _serialSendBuff.SetValue(Convert.ToByte(_serialEthSettingPortNo & 255), index++);
            _serialSendBuff.SetValue(Convert.ToByte((_serialEthSettingPortNo >> 8) & 255), index++);

            _serialSendBuff.SetValue(Convert.ToByte(0), index++);
            _serialSendBuff.SetValue(Convert.ToByte(0), index++);

            _serialSendBuff.SetValue(_serialDHCPFlag ? 0xFF : 0x00, index++);
            _serialSendBuff.SetValue((byte)0xFF, index++);
            if (USB)
            {
                for (int i = 0; i < 60 - 18; i++)
                    _serialSendBuff.SetValue(Convert.ToByte(0), index++);
                _serialSendBuff.SetValue(Convert.ToByte(CalculateBCC(60, 3)), index++);
            }
            else
                _serialSendBuff.SetValue(Convert.ToByte(CalculateBCC(18, 3)), index++);
            return;
        }

        protected int GetSetFrameIntoBuffForEthSettings(int pType)
        {
            LogWriter.WriteToFile("Serial.cs-> GetSetFrameIntoBuffForEthSettings()", "Started", "RTC_Upgrade");
            //DATAFRAMESIZE = 256;
            uint tmp;
            int index = 0;
            int iTempDataFrameSize = 256;
            int oneByte = 8;  // In bits
            int twoBytes = 16; // In Bits
            int threeBytes = 24; // In Bits
            int sizeOfLength = 4;  // Size of _deviceLength vari. is 4 Bytes
            byte[] bytes = new byte[sizeOfLength];
            _deviceLength = 19;
            _deviceProdID = Convert.ToInt16(CommonConstants.ProductDataInfo.iProductID);

            tmp = _deviceLength;
            _deviceTotalFrames = (uint)(_deviceLength / (DATAFRAMESIZE - FRAMEDIFFERENCE));//Total No of Frames
            _deviceTotalFrames = ((_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE)) > 0) ? _deviceTotalFrames + 1 : _deviceTotalFrames;

            _serialSendBuff.SetValue((byte)STARTFRAMESIZE, index++);

            _serialSendBuff.SetValue((byte)_deviceProdID, index++);      //Samir
            _serialSendBuff.SetValue((byte)(_deviceProdID / iTempDataFrameSize), index++);

            _serialSendBuff.SetValue((byte)(0), index++);// Multiple Download byte which will always be 0 over here as Ethernet setting option is last to be downloaded
            _serialSendBuff.SetValue(_arrdwnlsetupframe[1], index++);//Run Mode Byte
            _serialSendBuff.SetValue(_arrdwnlsetupframe[2], index++);//Keep Memory Byte
            _serialSendBuff.SetValue(_arrdwnlsetupframe[3], index++);//Except keep memory Byte

            #region FP_CODE Pravin Ethernet Settings
            //_serialSendBuff.SetValue((byte)(0), index++);//Data Download Mode Byte
            _serialSendBuff.SetValue(_arrdwnlsetupframe[4], index++);//Data Download Mode Byte for USB
            #endregion

            for (int i = 0; i < 2; i++)	//	These 2 Bytes r Not Used
                _serialSendBuff.SetValue((byte)0, index++);

            _serialSendBuff.SetValue((byte)tmp, index++);
            _serialSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), index++);
            _serialSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), index++);
            _serialSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), index++);

            for (int i = 0; i < 6; i++)	//	These 6 Bytes r not used
                _serialSendBuff.SetValue((byte)0, index++);
            LogWriter.WriteToFile("Serial.cs-> GetSetFrameIntoBuffForEthSettings()", string.Join(",", _serialSendBuff), "RTC_Upgrade");
            return SUCCESS;
        }

        #endregion

        /// <summary>
        /// When uploading or downloading finishes, this function completes 
        /// communication gracefully 
        /// </summary>
        private void FinalizeDownloading()
        {
            Thread.Sleep(100);
            _serialSendBuff[0] = DNLD_COMPLETE;
            _serialSendBuff[1] = _serialTotalCrcByte;
            _serialPort.Write(_serialSendBuff, 0, 2);
            //            _serialPort.Write(_serialSendBuff, 0, CRC_BYTE_SIZE);
            Thread.Sleep(100);
            _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
            _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _serialATimer.Enabled = true;
            ReceiveFrame();
            _serialATimer.Enabled = false;
            if (_serialReceivedByte == DNLD_COMPLETE)
            {
                #region Issue2.2_418 Vijay
                //Issue 333 SP 9.10.12
                //if (ClassList.CommonConstants.g_Support_IEC_Ladder && ClassList.CommonConstants.g_DownloadForOnLine == true)
                //{
                //    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strWaitDeviceIsInitializing));
                //    if (ClassList.CommonConstants.IsProductSupportsEthernet(ClassList.CommonConstants.ProductDataInfo.iProductID))
                //        System.Threading.Thread.Sleep(16000);
                //    else
                //        System.Threading.Thread.Sleep(10000);
                //}
                //End
                #endregion

                CommonConstants.downloadSucess = true;
                ///////
                //Commented for NQ products only
                //Remove Comment for Prizm Products

                //_serialSendBuff[0] = 0x55;
                //_serialPort.Write(_serialSendBuff, 0, 1);
                /////////

                //Keeran (KA)-------
                #region On Successful download

                if (CommonConstants.downloadSucess)
                {
                    //MessageBox.Show("Download Completed!!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                }
                #endregion
                //-----------------

                if (_deviceFileID == CommonConstants.SERIAL_LADDER_DNLD_FILEID)
                    ClassList.CommonConstants.g_LadderModified = false;
            }
            //Straton_change Haresh
            //CommonConstants.downloadSucess = true;
        }

        protected void GetEtherSettingFrame()
        {
            uint tmp;
            int index = 0;
            int oneByte = 8;  // In bits
            int twoBytes = 16; // In Bits
            int threeBytes = 24; // In Bits
            int sizeOfLength = 4;  // Size of _deviceLength vari. is 4 Bytes
            byte[] bytes = new byte[sizeOfLength];
            string[] tempStrArr = _serialIPAddress.Split('.');

            _serialSendBuff.SetValue((byte)0x11, index++);

            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[0]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[1]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[2]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[3]), index++);

            tempStrArr = _serialSubnetMask.Split('.');
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[0]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[1]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[2]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[3]), index++);

            tempStrArr = _serialDefaultGateway.Split('.');
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[0]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[1]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[2]), index++);
            _serialSendBuff.SetValue(Convert.ToByte(tempStrArr[3]), index++);

            _serialSendBuff.SetValue(Convert.ToByte(_serialEthSettingPortNo & 255), index++);
            _serialSendBuff.SetValue(Convert.ToByte((_serialEthSettingPortNo >> 8) & 255), index++);

            _serialSendBuff.SetValue(Convert.ToByte(0), index++);
            _serialSendBuff.SetValue(Convert.ToByte(0), index++);

            _serialSendBuff.SetValue(_serialDHCPFlag ? 0xFF : 0x00, index++);
            _serialSendBuff.SetValue((byte)0xFF, index++);
            return;
        }

        /// <summary>
        /// this function returns proper file id to communicate with the prizm
        /// </summary>
        /// <param name="pFileID"></param>
        protected string GetFileID(int pFileID)
        {
            string tempStr = null;
            _deviceFileID = 0;
            string currentBootBlockVer = strbootblock_Base; //Download_NewHardwareChnages_Files Vijay
            if ((pFileID & (byte)DownloadData.Firmware) > 0)
            {
                //FP_CODE  R12  Haresh
                _deviceFileID = CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID;
                //if (CommonConstants.IsProductFlexiPanels(ClassList.CommonConstants.ProductDataInfo.iProductID))
                //    tempStr = "HIO\\Object\\" + CommonConstants.GetFolderName(ClassList.CommonConstants.ProductDataInfo.iProductID) + "\\" + CommonConstants.DOWNLOAD_FIRMWARE_FILENAME;
                //else
                //{
                //    #region Download_NewHardwareChnages_Files Vijay
                //    if (ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_FL005_0808P0201L || ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_FL005_0808P || ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_FL005_0604P || //SS_ChangeFWScheme                        
                //        ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_HH5L_B0201A0808D_P ||
                //        ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_HH5L_B0604D_P ||
                //        ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_HH5L_B0808D_P)//Change_mail August 26, 2016 11:42 AM_PrashantK_SY //Hitachi Hi-Rel Vijay
                //    {
                //        if (currentBootBlockVer == "1.03" || currentBootBlockVer == "1.04")//Download_NewHardwareChnages_1.04 Vijay
                //            tempStr = "HIO\\Object\\" + CommonConstants.GetProductName(_deviceProdID) + "\\" + "Firmwarev1.abs";
                //        else
                //            tempStr = "HIO\\Object\\" + CommonConstants.GetProductName(_deviceProdID) + "\\" + CommonConstants.DOWNLOAD_FIRMWARE_FILENAME;
                //    }
                //    #endregion
                //    else
                //        tempStr = "HIO\\Object\\" + CommonConstants.GetProductName(_deviceProdID) + "\\" + CommonConstants.DOWNLOAD_FIRMWARE_FILENAME;
                //}
                //End
                _serialFileID ^= 2;

                /* //Can_HMI_change 1
                 StreamReader srReader;
                 String strVersion = "";
                 srReader = new StreamReader("HIO\\Object\\" + CommonConstants.GetProductName(_deviceProdID) + "\\" + "Version.txt");
                 strVersion = srReader.ReadLine();
                 tempStr = "HIO\\Object\\" + CommonConstants.GetProductName(_deviceProdID) + "\\" + strVersion;
                 srReader.Close();
                 //End
                  */

            }
            else if ((pFileID & (byte)DownloadData.Application) > 0)
            {
                _deviceFileID = CommonConstants.SERIAL_APPLICATION_DNLD_FILEID;
                tempStr = CommonConstants.TEMP_DOWNLOAD_FILENAME;
                _serialFileID ^= 1;
            }
            else if ((pFileID & (byte)DownloadData.Ladder) > 0)
            {
                _deviceFileID = CommonConstants.SERIAL_LADDER_DNLD_FILEID;
                tempStr = CommonConstants.DOWNLOAD_LADDER_FILENAME;
                _serialFileID ^= 4;
            }
            else if ((pFileID & (byte)DownloadData.Font) > 0)
            {
                _deviceFileID = CommonConstants.SERIAL_FONT_DNLD_FILEID;
                tempStr = CommonConstants.DOWNLOAD_FONT_FILENAME;
                _serialFileID ^= 8;
            }
            else if ((pFileID & (byte)DownloadData.EtherSettings) > 0)
            {
                _deviceFileID = SERIAL_ETHER_SETTINGS_DNLD;
                _serialFileID ^= 64;
                _deviceProdID = SERIAL_ETHER_SETTINGS_DNLD;
                #region FP_Ethernet_Implementation-AMIT
                tempStr = "";
                #endregion
            }
            else if ((pFileID & (byte)DownloadData.FHWT) > 0)
            {
                _deviceFileID = SERIAL_FHWT_DNLD;
                _serialFileID ^= 128;
                _deviceProdID = SERIAL_FHWT_DNLD;
                tempStr = CommonConstants.DOWNLOAD_FHWT_FILENAME;
            }
            else if ((pFileID & (byte)DownloadData.LoggedData) > 0)
            {
                _deviceFileID = SERIAL_FHWT_DNLD;
                //_serialFileID ^= 256;
                _deviceProdID = SERIAL_FHWT_DNLD;
                tempStr = CommonConstants.DOWNLOAD_FHWT_FILENAME;
            }

            _serialSendBuff[0] = _deviceFileID;
            LogWriter.WriteToFile("Serial.cs - SendFile() ", "path: " + tempStr + " fileId: " + string.Join(",", _deviceFileID), "RTC_Upgrade");
            return tempStr;
        }

        //FP_CODE Pravin Application + Ladder Upload
        protected string GetUploadFileID(int pFileID)
        {
            string tempStr = null;
            _deviceFileID = 0;

            #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
            if ((pFileID & (byte)DownloadData.Ladder) > 0)
            {
                _deviceFileID = CommonConstants.LADDER_UPLD_FILEID;
                tempStr = CommonConstants.UPLOAD_LADDER_FILENAME;
                _deviceFileName = tempStr;
                _serialFileID ^= 4;
            }
            #endregion
            else if ((pFileID & (byte)DownloadData.Application) > 0)
            {
                _deviceFileID = CommonConstants.SERIAL_APPLICATION_UPLD_FILEID;
                tempStr = CommonConstants.TEMP_DOWNLOAD_FILENAME;
                _serialFileID ^= 1;
            }
            #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
            else if ((pFileID & (int)DownloadData.HistAlarmData) > 0)
            {
                _deviceFileID = SERIAL_HIST_ALARM_DATA_UPLD;
                _serialFileID ^= (int)DownloadData.HistAlarmData;
                _deviceProdID = SERIAL_HIST_ALARM_DATA_UPLD;
                tempStr = CommonConstants.HistAlarmBinaryFile;
            }
            #endregion
            else if ((pFileID & (byte)DownloadData.LoggedData) > 0)
            {
                _deviceFileID = SERIAL_LOGGEDDATA_UPLD;
                _serialFileID ^= 16;
                _deviceProdID = SERIAL_LOGGEDDATA_UPLD;
                tempStr = CommonConstants.BinaryFile;
            }
            //Ladder_Change_R10
            else if (pFileID == CommonConstants.LADDER_UPLD_FILEID)
            {
                _deviceFileID = CommonConstants.LADDER_UPLD_FILEID;
                // tempStr = CommonConstants.TEMP_DOWNLOAD_FILENAME;
                tempStr = CommonConstants.UPLOAD_LADDER_FILENAME;
                _deviceFileName = tempStr;
                _serialFileID ^= 1;
            }
            //End//

            _serialSendBuff[0] = _deviceFileID;
            return tempStr;
        }
        //End


        public string[] GetSystemPorts()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            return ports;
        }


        /// <summary>
        /// Sends the erase data logger memory frame
        /// </summary>
        private void SendEraseDataLoggerMemoryFrame()
        {
            byte[] _btREPL = new byte[4];

            _btREPL[0] = 0x52;
            _btREPL[1] = 0x45;
            _btREPL[2] = 0x50;
            _btREPL[3] = 0x4C;
            Thread.Sleep(TEN_SEC_DELAY);
            _serialSendBuff[0] = 0x66;
            _serialPort.Write(_serialSendBuff, 0, 1);
            ReceiveFrame();
            if (_serialReceivedByte == 0x68)
                _serialPort.Write(_btREPL, 0, 4);
        }

        #endregion

        #region public properties
        public static bool EraseDataLoggerMemory
        {
            get
            {
                return _serialDataLoggerEraseMem;
            }
            set
            {
                _serialDataLoggerEraseMem = value;
            }
        }
        #endregion
    }
}

