/*===================================================================
//
// Copyright 2005-2006, Renu Electronics Pvt. Ltd., Pune, India.
// All Rights Reserved.
//
// The copyright above and this notice must be preserved in all
// copies of this source code.  The copyright above does not
// evidence any actual or intended publication of this source
// code.
//
// This is unpublished proprietary trade secret source code of
// Renu Electronics Pvt. Ltd.  This source code may not be copied,
// disclosed, distributed, demonstrated or licensed except as
// expressly authorized by Renu Electronics Pvt. Ltd.
//
// This source code in its entirity is developed by Renu Electronics 
// Pvt. Ltd
//
// File Name	USB.cs
// Author		Samir Karve
//=====================================================================
*/

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.Win32.SafeHandles;
using CommConst = ClassList.CommonConstants;
using System.Threading;
using System.Timers;
using TestVCDLL;

namespace ClassList
{
    /// <summary>
    /// This class is derived for Rectangular class.
    /// It contains Read, Write and Draw method for Trend.
    /// </summary>
    // 
    [Serializable]
    public class USB : Serial
    {
        bool blFlag_Connect = false;
        private Thread _USBThread;
        private TestVCDLL.Class1 obj = new Class1();
        private ArrayList m_dataList = new ArrayList();

        //FP_CODE Pravin USB Timeout        
        private int i_RetVal = 0;
        private bool b_USBReadCompleteFlag = false;
        private int refCount = 0;
        //End

        private string strFileName1_Analog = string.Empty;
        private string strFileName2_Analog = string.Empty;
        private byte m_byteFirst = 0;
        private byte m_byteSecond = 0;
        //End

        #region Public Events
        public delegate void DownloadPercentage(float pPercentage);
        public event DownloadPercentage _deviceDownloadUSBPercentage;

        public delegate void DownloadStatus(int pMessage);
        public event DownloadStatus _deviceDownloadUSBStatus;
        

        #endregion


        #region Public Properties
        public static Guid HIDGuid
        {
            get
            {
                //return new Guid("0DFF9F3F-16D5-44A5-A6C1-1E4348AC9626"); //ID specified in Prizm3.12. Its Global Unique ID
                return new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); //USB_PORT_RESET
            }
        }
        #endregion

        #region Public Methods

        public USB()
        {

            DATAFRAMESIZE = 60;
            refCount = 0;

            _serialSendBuff = new byte[1000 + STARTFRAMESIZE];
            _serialRecvBuff = new byte[1000 + STARTFRAMESIZE];

            _USBThread = new Thread(new ThreadStart(DownLoadUSBFunc));
        }

        public USB(short pProductID)
        {
            _deviceProdID = pProductID;
            DATAFRAMESIZE = 60;
            refCount = 0;

            _serialSendBuff = new byte[1000 + STARTFRAMESIZE];
            _serialRecvBuff = new byte[1000 + STARTFRAMESIZE];

            _USBThread = new Thread(new ThreadStart(DownLoadUSBFunc));
        }

        //FP_CODE Pravin  EthernetSettings Download
        //FP_CODE Pravin Download Mode 
        public USB(short pProductID, string pPort, byte[] _setupFrameMode)
        {
            _deviceProdID = pProductID;
            DATAFRAMESIZE = 60;
            refCount = 0;

            string[] ethSettingStrings;
            string portID;
            if (pPort.Contains("|"))
            {
                ethSettingStrings = pPort.Split('|');
                _serialEthSettingPortNo = Convert.ToInt32(ethSettingStrings[4]);
                _serialIPAddress = ethSettingStrings[1];
                _serialSubnetMask = ethSettingStrings[2];
                _serialDefaultGateway = ethSettingStrings[3];
                if (ethSettingStrings[5] == "True")
                    _serialDHCPFlag = true;
            }

            _serialSendBuff = new byte[1000 + STARTFRAMESIZE];
            _serialRecvBuff = new byte[1000 + STARTFRAMESIZE];

            _arrdwnlsetupframe = new byte[10];
            _arrdwnlsetupframe = _setupFrameMode;

            #region FP_CODE Pravin Ethernet Settings
            _serialEthSettingPortNo = Convert.ToInt32(Device.EthernetSettings._DownloadPort);
            _serialIPAddress = Device.EthernetSettings._IPAdderess;
            _serialSubnetMask = Device.EthernetSettings._SubnetMask;
            _serialDefaultGateway = Device.EthernetSettings._DefaultGateway;
            _serialDHCPFlag = Device.EthernetSettings._DHCP;
            #endregion

            _USBThread = new Thread(new ThreadStart(DownLoadUSBFunc));

        }

        public USB(short pProductID, string pFileName1, string pFileName2, byte byteFirst, byte byteSecond, byte[] _setupFrameMode)
        {
            strFileName1_Analog = pFileName1;
            strFileName2_Analog = pFileName2;
            m_byteFirst = byteFirst;
            m_byteSecond = byteSecond;
            refCount = 0;

            _deviceProdID = pProductID;
            DATAFRAMESIZE = 60;

            _serialSendBuff = new byte[1000 + STARTFRAMESIZE];
            _serialRecvBuff = new byte[1000 + STARTFRAMESIZE];

            _arrdwnlsetupframe = new byte[10];
            _arrdwnlsetupframe = _setupFrameMode;

            _USBThread = new Thread(new ThreadStart(DownLoadUSBFunc_AnalogModule));

        }
        //End
        //USB_PORT_RESET
        public int Connect_ForFirstTime()
        {
            bool blFlag = false;
            Guid gHid = HIDGuid;
            _deviceDownloadUSBStatus(0);
            TestVCDLL.Class1.ProductName = "";
            blFlag = obj.OpenUSB_Dev(gHid);

            if (blFlag)
            {
                //USB_PORT_RESET - START
                if (ClassList.CommonConstants.IsProductUSBPortReset(ClassList.CommonConstants.ProductDataInfo.iProductID))
                {
                    obj.USB_CancelPendingIO();
                }

                //USB_PORT_RESET - END
                if (blFlag_Connect == false)
                {
                    //FP_CODE Haresh  Delay is Needed for faster machines
                    Thread.Sleep(500);
                    blFlag_Connect = true;
                }
                _deviceIsConnected = true;
                return SUCCESS;
            }
            else
            {
                _deviceIsConnected = false;
                Thread.Sleep(100);
                if (ReConnect() == CommonConstants.SUCCESS)
                    return SUCCESS;

                return FAILURE;
            }
        }

        public override int Connect()
        {
            bool blFlag = false;
            Guid gHid = HIDGuid;
            _deviceDownloadUSBStatus(0);
            TestVCDLL.Class1.ProductName = "";
            blFlag = obj.OpenUSB_Dev(gHid);

            if (blFlag)
            {
                if (blFlag_Connect == false)
                {
                    //FP_CODE Haresh  Delay is Needed for faster machines
                    Thread.Sleep(500);
                    blFlag_Connect = true;
                }  
                _deviceIsConnected = true;
                return SUCCESS;
            }
            else
            {
                //If Not Connected
                //_deviceDownloadUSBStatus(ERR_RECONNECTUNIT);                       
                //CommonConstants.communicationStatus = 0;

                /*if (obj.productID == -1)
                    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strProductMismatch));
                else
                    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));*/

                _deviceIsConnected = false;

                Thread.Sleep(100);
                if (ReConnect() == CommonConstants.SUCCESS)
                    return SUCCESS;

                return FAILURE;
            }
        }

        public int ReConnect()
        {
            bool blFlag = false;
            Guid gHid = HIDGuid;
            _deviceDownloadUSBStatus(0);
            TestVCDLL.Class1.ProductName = "";      // CommConst.GetProductNameForUSBHostFileName(_deviceProdID);

            blFlag = obj.OpenUSB_Dev(gHid);

            if (blFlag)
            {
                Thread.Sleep(100);
                _deviceIsConnected = true;
                return SUCCESS;
            }
            else
            {
                CommonConstants.communicationStatus = 0;
                #region Issue 259 SP
                /*if (obj.productID == -1)
                    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strProductMismatch));
                else if (_deviceFileName == null) //Issue_2.0*/
                #endregion
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));

                _deviceIsConnected = false;
                return FAILURE;
            }
        }

        #region FP_CODE Pravin Ethernet Settings
        public int ReConnect_DataDownload()
        {
            bool blFlag = false;
            Guid gHid = HIDGuid;
            _deviceDownloadUSBStatus(0);
            TestVCDLL.Class1.ProductName = "";      // CommConst.GetProductNameForUSBHostFileName(_deviceProdID);

            blFlag = obj.OpenUSB_Dev(gHid);

            if (blFlag)
            {
                Thread.Sleep(100);
                _deviceIsConnected = true;
                return SUCCESS;
            }
            else
            {
                //CommonConstants.communicationStatus = 0;
                _deviceIsConnected = false;
                return FAILURE;
            }
        }
        #endregion

        public override int ReceiveFile(string pFileName, int pFileID)
        {
            //FP_CODE Pravin Application + Ladder Upload
            CommonConstants.LADDER_PRESENT = false;
            //End

            _deviceFileName = pFileName;
            _serialFileID = pFileID;
            #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
            //if ((pFileID & (int)DownloadData.HistAlarmData) > 0)	// Manik
            //GetUploadFileIDForAlarm(pFileID);
            //else
            //    GetUploadFileID(pFileID);
            GetUploadFileIDForAlarm(pFileID);
            #endregion
            _serialSendBuff[0] = _deviceFileID;
            if ((pFileID & (int)DownloadData.HistAlarmData) > 0)	//Manik
                _USBThread = new Thread(new ThreadStart(UpLoadFuncForAlarm));
            else
                _USBThread = new Thread(new ThreadStart(UpLoadFunc));

            Thread.Sleep(100);

            if ((pFileID != SERIAL_LOGGEDDATA_UPLD) && (pFileID != SERIAL_HIST_ALARM_DATA_UPLD))
            {
                if (ReadUSBData(_deviceFileID) == CommonConstants.FAILURE)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));

                    return FAILURE;
                }
            }
            else
                SendReport(_deviceFileID, 1);
            //End

            #region New FP3035 Product Series_V2.3_Issue_447 SP
            if (_serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_7)
            {
                DownloadData download = DownloadData.Application;
                _deviceFileID = CommonConstants.SERIAL_APPLICATION_UPLD_FILEID;
                _deviceFileName = CommonConstants.TEMP_DOWNLOAD_FILENAME;
                _serialFileID = 0;
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
                if (ReadUSBData(_deviceFileID) == CommonConstants.FAILURE)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    return FAILURE;
                }
            }
            #endregion

            if (_serialReceivedByte == _deviceFileID || _serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_1 || _serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2 || ((_deviceFileID == SERIAL_LOGGEDDATA_UPLD) || (_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD)))
            {	//	If Prizm Ready,
                _serialTimeOut = 0;
                if (_serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2 && ((_deviceFileID != SERIAL_LOGGEDDATA_UPLD) && (_deviceFileID != SERIAL_LOGGEDDATA_UPLD)))
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);
                    Thread.Sleep(100);

                    unsafe
                    {
                        bool blFlag = false;
                        fixed (byte* p = _serialSendBuff)
                        {
                            blFlag = obj.WriteUSB_Device(p, 4);
                        }
                    }
                }
            }
            else if (_serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_5) //Application or ladder not present
            {
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strApplicationNotPresent));
                CommonConstants.communicationStatus = 0;
                return FAILURE;
            }
            else // Unit Not Ready
            {
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotReady));
                //_deviceDownloadStatus(7);
                return FAILURE;	//	FAILURE
            }
            //_deviceDownloadStatus(2);
            if ((_deviceFileID == SERIAL_LOGGEDDATA_UPLD) || (_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD))
                _serialSendBuff = new byte[1024 * 8 + 1 + DATA_START_INDEX + 4];

            int message = 256;
            if (_deviceFileID == CommonConstants.SERIAL_LOGGED_UPLD_FILEID)
                message <<= 4;
            if (_deviceFileID == CommonConstants.SERIAL_HISTALARM_UPLD_FILEID)
                message <<= 5;


            //FP_CODE Pravin Application + Ladder Upload
            if (_deviceFileID == CommonConstants.LADDER_UPLD_FILEID)
                message <<= 2;
            //End

            _deviceDownloadUSBStatus(2 + message);

            _USBThread.Start();

            if (i_RetVal == CommonConstants.FAILURE)
                return FAILURE;
            else
                return SUCCESS;

            return SUCCESS;
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
            _serialNoOfSent = 0;
            _serialFileID = pFileID;

            //////////Remove this part /////////
            //_deviceProdID = 686; // for 550N
            ///////////////////////////////////
           
            pFileName = GetFileID(pFileID);
            _deviceFileName = pFileName;

            #region FP_CODE Pravin Ethernet Settings
            if (!File.Exists(pFileName) && _arrdwnlsetupframe[4] == 1 && _deviceProdID != SERIAL_ETHER_SETTINGS_DNLD)
            {
                if (GetDataDownload() == CommonConstants.SUCCESS)
                    return SUCCESS;
                else
                    return FAILURE;
            }
            #endregion

            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
            if (!File.Exists(pFileName) && _deviceProdID != SERIAL_ETHER_SETTINGS_DNLD)// will never occure, but precautionary
            {
                Close();
                return FAILURE;
            }
            if (_deviceIsConnected == false)
            {
                Close();
                return FAILURE;
            }

            //FP_CODE Pravin USB timeout - Rev 1
            //IsPrizmUSBReady();
            #region FP_CODE Pravin Ethernet Settings
            if (IsPrizmUSBReady_Initial() == SUCCESS)
            {
                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                {
                    _USBThread = new Thread(new ThreadStart(InitiateCommunicationForEthernetSetting));
                    _USBThread.Start();
                    return SUCCESS;

                }
                else if (InitiateCommunication() == SUCCESS)
                {
                    if (_deviceFileID != SERIAL_ETHER_SETTINGS_DNLD)
                        _USBThread.Start();
                    return SUCCESS;
                }
            }
            else
            {
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                return FAILURE;
            }
            #endregion
            return FAILURE;
        }

        public int SendFile(string pFileName)
        {
            _deviceFileID = CommonConstants.SERIAL_EXP_DNLD_FILEID;
            _deviceFileName = pFileName;

            if (!File.Exists(pFileName))
                return FAILURE;

            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));

            if (_deviceIsConnected == false)
            {
                Close();
                return FAILURE;
            }

            if (IsPrizmUSBReady_Initial() == SUCCESS)
            {
                if (InitiateCommunication() == SUCCESS)
                {
                    _USBThread.Start();
                    return SUCCESS;
                }
            }
            else
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
            }
            //End
            return FAILURE;
        }

        public int SendFile_AnalogExpansion(string pFileName)
        {
            _deviceFileID = m_byteFirst;
            _deviceFileName = pFileName;

            if (!File.Exists(pFileName))
                return FAILURE;

            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));

            if (_deviceIsConnected == false)
            {
                Close();
                return FAILURE;
            }

            if (IsPrizmUSBReady_Initial() == SUCCESS)
            {
                if (InitiateCommunication() == SUCCESS)
                {
                    _USBThread.Start();
                    return SUCCESS;
                }
            }
            else
            {
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                return FAILURE;
            }
            //End
            return FAILURE;
        }


        public void UpLoadFuncForAlarm()
        {
            bool error = false;
            int bytesRead = 0;
            FileStream fs = new FileStream(_deviceFileName, FileMode.Create);

            CommonConstants.communicationStatus = 2;
            CommonConstants.communicationType = 2;

            if ((_deviceFileID != SERIAL_LOGGEDDATA_UPLD) && (_deviceFileID != SERIAL_HIST_ALARM_DATA_UPLD))
                ReceiveReport(DOWN);

            else
            {
                while (true)
                {
                    if ((_serialRecvBuff[0] != SERIAL_LOGGEDDATA_UPLD) && (_serialRecvBuff[0] != SERIAL_HIST_ALARM_DATA_UPLD))
                        ReceiveReport(DOWN);
                    else
                        break;
                }
            }
            for (int iTemp = 0; true; iTemp++)
            {
                for (int i = 1; ; i++)
                {
                    CommonConstants.communicationType = 2;

                    if (((_deviceFileID != SERIAL_LOGGEDDATA_UPLD) && (_deviceFileID != SERIAL_HIST_ALARM_DATA_UPLD)) && i == 1)
                    {
                        SendReport(1, 0, THREE_SEC_DELAY);
                        _serialNoOfByteTORead = 4;
                        ReceiveReport(4);
                        _deviceTotalFrames = CommonConstants.MAKEUINT(_serialRecvBuff);
                        _deviceTotalFrames = _deviceTotalFrames % 64 > 0 ? _deviceTotalFrames / 64 + 1 : _deviceTotalFrames / 64;
                    }

                    if (_deviceTotalFrames == 0)
                        break;

                    if (_deviceDownloadUSBPercentage != null)
                        _deviceDownloadUSBPercentage(100 * i / Convert.ToInt32(_deviceTotalFrames));

                    Thread.Sleep(0);
                    if (!ReceiveReport(UP))
                    {
                        /*error = true;
                        _deviceDownloadUSBPercentage(7);
                        Close();*/

                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        fs.Close();
                        i_RetVal = CommonConstants.FAILURE;
                        return;
                    }

                    if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD && _deviceFileID != SERIAL_HIST_ALARM_DATA_UPLD && i == 1)
                        iProductID = ClassList.CommonConstants.MAKEWORD(_serialRecvBuff[4], _serialRecvBuff[5]);

                    if (CommonConstants.communicationStatus == -1)
                    {
                        #region Sammed
                        //_deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationTimeout));
                        error = true;
                        break;
                        #endregion
                    }
                    if (_serialSendBuff[0] == 0xdd)
                        break;

                    bytesRead = 64;

                    fs.Write(_serialRecvBuff, 0, bytesRead);

                    if (i >= _deviceTotalFrames)
                        break;
                }
                //USB_PORT_RESET_ISSUE_SAMMED
                if (error)
                    break;
                //End
                #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
                bool ldr_flag = false;
                if (_deviceFileName == CommonConstants.UPLOAD_LADDER_FILENAME)
                {
                    _serialSendBuff[0] = 0x55;
                    SendReport(1, 2, THREE_SEC_DELAY);
                    CommonConstants.LADDER_PRESENT = true;
                    ldr_flag = true;
                }
                #endregion

                _deviceFileName = GetUploadFileIDForAlarm(_serialFileID);
                if (_deviceFileName == null || error)
                    break;

                fs.Close();


                if (_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD)
                {
                    //Issue_939_FL100_sammed
                    if (ClassList.CommonConstants.IsProductPLC(iProductID))//Issue_939_FL100_sammed_added
                    {
                        _deviceFileName = GetUploadFileIDForAlarm(_serialFileID);
                        if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                        {
                            if ((CommonConstants.IsProductSupportsDataLogger(iProductID)) == false)
                            {
                                _serialSendBuff[0] = 0x55;
                                SendReport(1, 2, THREE_SEC_DELAY);

                                CommonConstants.communicationStatus = 0;
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strHisAlarmAndLoggedDataNotSupported));
                                fs = null;
                                Close();
                                i_RetVal = CommonConstants.FAILURE;
                                return;
                            }

                        }

                    }

                    if ((CommonConstants.IsProductSupportsHisAlarm(iProductID)) == false)
                    {
                        _serialSendBuff[0] = 0x55;
                        SendReport(1, 2, THREE_SEC_DELAY);

                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strHisAlarmNotSupported));
                        fs = null;
                        Close();
                        i_RetVal = CommonConstants.FAILURE;
                        return;
                    }

                    //end

                }

                if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                {
                    if ((CommonConstants.IsProductSupportsDataLogger(iProductID)) == false)
                    {
                        _serialSendBuff[0] = 0x55;
                        SendReport(1, 2, THREE_SEC_DELAY);

                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDataLogNotSupported));
                        fs = null;
                        Close();
                        i_RetVal = CommonConstants.FAILURE;
                        return;
                    }
                }

                //if (PrevDwnld && _deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                {
                    Close();
                    Thread.Sleep(THREE_SEC_DELAY);
                    //Thread.Sleep(TEN_SEC_DELAY);
                    //Thread.Sleep(TEN_SEC_DELAY);

                    //Create new handle 
                    Connect();
                    //Thread.Sleep(THREE_SEC_DELAY);
                    if (_deviceIsConnected == false)
                    {
                        _deviceDownloadUSBStatus(10);
                        Close();
                        return;
                    }
                    //PrevDwnld = false;
                }
                Thread.Sleep(500);
                IsPrizmUSBReady();

                #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
                bool blFlag = false;
                if (ldr_flag)
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);
                    Thread.Sleep(100);

                    unsafe
                    {

                        fixed (byte* p = _serialSendBuff)
                        {

                            blFlag = obj.WriteUSB_Device(p, 4);
                        }
                    }
                }                
                else
                {
                    _serialSendBuff[0] = 0x01;                    
                    unsafe
                    {
                        fixed (byte* p = _serialSendBuff)
                        {
                            blFlag = obj.WriteUSB_Device(p, 1);
                        }
                    }
                }
                #endregion
                Thread.Sleep(100);

                if (!ReceiveReport(4))
                    break;

                fs = new FileStream(_deviceFileName, FileMode.Create);

                if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                {
                    bytesRead = _serialRecvBuff[0];
                    bytesRead += _serialRecvBuff[1] * 256;
                    bytesRead += _serialRecvBuff[2] * 256 * 256;
                    bytesRead += _serialRecvBuff[3] * 256 * 256 * 256;
                    _deviceTotalFrames = (uint)bytesRead;
                    _deviceTotalFrames = _deviceTotalFrames % 64 > 0 ? _deviceTotalFrames / 64 + 1 : _deviceTotalFrames / 64;   //RC1_Issue1339 (Manik)
                }

                if (_deviceFileID == SERIAL_HIST_ALARM_DATA_UPLD)
                {
                    bytesRead = _serialRecvBuff[0];
                    bytesRead += _serialRecvBuff[1] * 256;
                    bytesRead += _serialRecvBuff[2] * 256 * 256;
                    bytesRead += _serialRecvBuff[3] * 256 * 256 * 256;
                    _deviceTotalFrames = (uint)bytesRead;
                    _deviceTotalFrames = _deviceTotalFrames % 64 > 0 ? _deviceTotalFrames / 64 + 1 : _deviceTotalFrames / 64;   //RC1_Issue1339 (Manik)
                }

                int message = 256;
                if (_deviceFileID == CommonConstants.SERIAL_LOGGED_UPLD_FILEID)
                    message <<= 4;
                if (_deviceFileID == CommonConstants.SERIAL_HISTALARM_UPLD_FILEID)
                    message <<= 5;
                _deviceDownloadUSBStatus(2 + message);
            }

            if (!error)//USB_PORT_RESET_ISSUE_SAMMED
            {
                Thread.Sleep(1000);
                if (true)
                {
                    _serialSendBuff[0] = 0x55;
                    SendReport(1, 2, THREE_SEC_DELAY);
                    //_serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
                else
                {
                    _serialSendBuff[0] = 0xdd;
                    SendReport(1, 1, THREE_SEC_DELAY);
                    //_serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
            }

            fs.Close();
            Close();
            if (!error)
            {
                CommonConstants.communicationStatus = 0;
                CommonConstants.communicationType = 2;
                _deviceDownloadUSBStatus(18);
            }
            else if (CommonConstants.communicationStatus == -1)
            {
                //CommonConstants.communicationType = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationAborted));
            }
            else
            {
                CommonConstants.communicationType = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
            }
            return;
        }

        /// <summary>
        /// Checks if prizm is ready to communicate, for downloading and uploading
        /// </summary>
        /*  private void IsPrizmUSBReady()
          {
              _serialNoOfSent = 0;
              //_deviceDownloadUSBStatus(0);
              for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
              {
                  SendReport(1, CRC_BYTE_SIZE, TEN_SEC_DELAY);
                  Thread.Sleep(100);
                  if (ReceiveReport())
                  {
                      _serialATimer.Enabled = false;
                      i = NO_OF_TRY_FOR_READY_PRIZM;
                      break;
                  }
                  _serialTimeOut = 0;
                  _serialATimer.Enabled = false;
              }
          }
          */
        //FP_CODE  R12  Haresh
        private void IsPrizmUSBReady()
        {
            _serialNoOfSent = 0;
            //_deviceDownloadUSBStatus(0);
            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
            {
                SendReport(1, CRC_BYTE_SIZE, TEN_SEC_DELAY);
                Thread.Sleep(100);
                if (ReceiveReport())
                {
                    if (_serialReceivedByte == 0x00)
                        continue;
                    _serialATimer.Enabled = false;
                    i = NO_OF_TRY_FOR_READY_PRIZM;
                    break;
                }
                _serialTimeOut = 0;
                _serialATimer.Enabled = false;
            }
        }

        private void SendReport(int pFrameNum, int pSize, int pDelay)
        {
            if (pSize == 1)		//	If want to send an READY signal
            {
                if (_serialNoOfSent >= NO_OF_TRY_FOR_READY_PRIZM)
                    return;
                _serialSendBuff[0] = (byte)_deviceFileID;
                _serialNoOfSent++;
            }
            else if (pSize == 0)		//	send an ACK signal
            {
                _serialSendBuff[0] = (byte)0x01;
                pSize = 1;
            }
            else if (pSize == 2)		//	send an ACK signal
            {
                pSize = 1;
            }
            else		//	If want to send a DATA Frame
            {
                _serialSendBuff[0] = (byte)(pSize);			//	First 3 bytes of data frame contains
                _serialSendBuff[1] = (byte)pFrameNum;	//  Size if Frame(1 Byte) and 
                _serialSendBuff[2] = (byte)(pFrameNum / DATAFRAMESIZE);	// No.of Frame(2 Bytes)
                _serialSendBuff[pSize + DATA_START_INDEX] = _serialCrcByte;	// Lastly Add here CRC byte
                pSize += (DATA_START_INDEX + CRC_BYTE_SIZE);
            }
            bool blFlag = false;
            unsafe
            {
                fixed (byte* p = _serialSendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, pSize);
                }
            }

            _serialSendBuff[0] = 0;
            _serialATimer = new System.Timers.Timer(pDelay);
            _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _serialATimer.Enabled = true;
        }

        //FP_CODE Pravin USB Timeout
        private void SendReport(byte CommandByte, int pSize)
        {
            _serialSendBuff[0] = CommandByte;
            bool blFlag = false;
            unsafe
            {
                fixed (byte* p = _serialSendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, pSize);
                }
            }

            _serialSendBuff[0] = 0;
        }
        //End

        /// <summary>
        /// This Function sends a frame of data to the connected port of specified number of bytes 
        /// of perticular no of frame. 
        ///	The parameter pDelay specifies the time to wait for the acknowledgement signal
        ///	The pFrameNum and  pSize specifies what is the no. of the frame to send and the size of
        /// that frame in bytes resp.
        /// </summary>
        /// <param name="pFrameNum"></param>
        /// <param name="pSize"></param>
        /// <param name="pDelay"></param>
        private void SendReport_DataFrame(int pFrameNum, int pSize, int pDelay)
        {
            if (pSize == 0)		//	If want to send an READY signal
            {
                if (_serialNoOfSent >= NO_OF_TRY_FOR_READY_PRIZM)
                    return;
                _serialSendBuff[0] = (byte)_deviceFileID;
                _serialNoOfSent++;
            }
            else if (pSize == 0)		//	send an ACK signal
            {
                _serialSendBuff[0] = (byte)0x01;
                pSize = 1;
            }
            else if (pSize == 0)		//	send an ACK signal
            {
                pSize = 1;
            }
            else		//	If want to send a DATA Frame
            {
                _serialSendBuff[0] = (byte)(pSize);			//	First 3 bytes of data frame contains
                _serialSendBuff[1] = (byte)pFrameNum;	//  Size if Frame(1 Byte) and 
                _serialSendBuff[2] = (byte)(pFrameNum / DATAFRAMESIZE);	// No.of Frame(2 Bytes)
                _serialSendBuff[pSize + DATA_START_INDEX] = _serialCrcByte;	// Lastly Add here CRC byte
                pSize += (DATA_START_INDEX + CRC_BYTE_SIZE);
            }
            bool blFlag = false;
            unsafe
            {
                fixed (byte* p = _serialSendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, pSize);
                }
            }

            _serialSendBuff[0] = 0;
        }

        /// <summary>
        /// This function recieves a frame of data whose size is specifed, within a limited span of time.
        /// If it get time out it will return without receiving frame.
        ///	The function return true if the frames received successfully
        ///	otherwise returns false on failure.
        /// </summary>
        /// <returns></returns>
        private bool ReceiveReport()
        {
            _serialTimeOut = 0;
            //m_oFile.ReadTimeout = THREE_SEC_DELAY;
            //while (_serialTimeOut == 0)
            {
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;
                try
                {
                    unsafe
                    {
                        fixed (byte* p = _serialRecvBuff)
                        {
                            _serialNoOfByteRead = obj.ReadUSB_Device(p, 1);
                        }
                    }

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
        /// If prizm is ready to communicate then start communication by sending  
        /// the set frame
        /// </summary>
        /// <returns></returns>
        private int InitiateCommunication()
        {
            if (_serialReceivedByte == _deviceFileID || _serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_1 || _serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2)
            {	//	If Prizm Ready, Send here setFrame
                _serialTimeOut = 0;

                if (_serialReceivedByte == _deviceFileID + 2)
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);

                    bool blFlag = false;
                    unsafe
                    {
                        fixed (byte* p = _serialSendBuff)
                        {
                            blFlag = obj.WriteUSB_Device(p, 4);
                        }
                    }

                    Thread.Sleep(100);
                    _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                    _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    _serialATimer.Enabled = true;
                    ReceiveReport();
                }
                #region EthernetSetting
                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                {
                    #region FP_CODE Pravin Ethernet Settings
                    int message = 256;
                    message <<= 7;
                    _deviceDownloadUSBStatus(3 + message);
                    if (ClassList.CommonConstants.IsProductPLC(ClassList.CommonConstants.ProductDataInfo.iProductID) &&
                        ClassList.CommonConstants.IsProductSupportsEthernet(ClassList.CommonConstants.ProductDataInfo.iProductID))
                    {
                        #region sammed FL100
                        if (ClassList.CommonConstants.IsProductMXSpecialCase_Based(ClassList.CommonConstants.ProductDataInfo.iProductID) || ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_FL055 //FL055_Product_Addition_Suyash
                            || ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_FL050_V2) //Issue_1281 Vijay
                        {
                            GetSetFrameIntoBuffForEthSettings(DOWN);
                        }
                        #endregion
                        else
                        {
                            GetEtherSettingFrame();
                        }
                    }
                    else
                        GetSetFrameIntoBuffForEthSettings(DOWN);


                    bool blFlag = false;
                    unsafe
                    {
                        fixed (byte* p = _serialSendBuff)
                        {
                            if (ClassList.CommonConstants.IsProductPLC(ClassList.CommonConstants.ProductDataInfo.iProductID) &&
                        ClassList.CommonConstants.IsProductSupportsEthernet(ClassList.CommonConstants.ProductDataInfo.iProductID))
                            {
                                #region sammed FL100
                                if (ClassList.CommonConstants.IsProductMXSpecialCase_Based(ClassList.CommonConstants.ProductDataInfo.iProductID) || ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_FL055//FL055_Product_Addition_Suyash
                                    || ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_FL050_V2) //Issue_1281 Vijay
                                {
                                    blFlag = obj.WriteUSB_Device(p, 0x16);
                                }
                                #endregion
                                else
                                {
                                    blFlag = obj.WriteUSB_Device(p, 0x13);
                                }
                                //blFlag = obj.WriteUSB_Device(p, 0x13);
                            }
                            else
                                blFlag = obj.WriteUSB_Device(p, 0x16);
                        }
                    }

                    _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                    _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    _serialATimer.Enabled = true;

                    //if (ClassList.CommonConstants.IsProductPLC(ClassList.CommonConstants.ProductDataInfo.iProductID) &&
                    //    ClassList.CommonConstants.IsProductSupportsEthernet(ClassList.CommonConstants.ProductDataInfo.iProductID))
                    if (ClassList.CommonConstants.IsProductPLC(ClassList.CommonConstants.ProductDataInfo.iProductID) &&
                       ClassList.CommonConstants.IsProductSupportsEthernet(ClassList.CommonConstants.ProductDataInfo.iProductID) && ClassList.CommonConstants.IsProductMXSpecialCase_Based(ClassList.CommonConstants.ProductDataInfo.iProductID) == false
                        && ClassList.CommonConstants.ProductDataInfo.iProductID != ClassList.CommonConstants.PRODUCT_FL055 && ClassList.CommonConstants.ProductDataInfo.iProductID != ClassList.CommonConstants.PRODUCT_FL050_V2)//FL055_Product_Addition_Suyash//sammed FL100 //Issue_1281 Vijay
                    {
                        //if (ReadUSBData() == CommonConstants.FAILURE)
                        //{
                        //    CommonConstants.communicationStatus = 0;
                        //    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        //    Close();
                        //    return FAILURE;
                        //}
                        ReceiveReport();
                        _serialATimer.Enabled = false;

                        if (_serialReceivedByte == 0x01)
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                            _deviceDownloadUSBPercentage(100);
                            Close();
                            return SUCCESS;
                        }
                        else
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                            Close();
                            return FAILURE;
                        }
                    }
                    else
                    {
                        ReceiveReport();

                        if (_serialReceivedByte == CORRECT_CRC)
                        {
                            GetEtherSettingFrame(true);
                            blFlag = false;
                            unsafe
                            {
                                fixed (byte* p = _serialSendBuff)
                                {
                                    blFlag = obj.WriteUSB_Device(p, 0x40);
                                }
                            }

                            _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                            _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                            _serialATimer.Enabled = true;

                        }
                    }
                    #endregion
                }
                #endregion
                else
                {
                    bool blFlag = false;

                    //This code comes in picture only case of expansion firmware download
                    if (_arrdwnlsetupframe[9] != 0)
                    {
                        _serialSendBuff[0] = _arrdwnlsetupframe[9];
                        unsafe
                        {
                            fixed (byte* p = _serialSendBuff)
                            {
                                blFlag = obj.WriteUSB_Device(p, 1);
                            }
                        }
                        ReceiveReport();
                    }
                    //End

                    Thread.Sleep(1000);
                    if (GetSetFrameIntoBuff(DOWN) == CommonConstants.FAILURE)//VOffice_IssueNo_304
                    {
                        if (_deviceFileInUse)
                        {
                            CommonConstants.communicationStatus = 0;
                            if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorFirmwareFileCreation));
                            if (_deviceFileID == CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorApplicationFileCreation));
                            Close();
                        }
                        return FAILURE;
                    }
                    blFlag = false;
                    unsafe
                    {
                        fixed (byte* p = _serialSendBuff)
                        {
                            blFlag = obj.WriteUSB_Device(p, STARTFRAMESIZE);
                        }
                    }
                }
                _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY * 3);
                _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                _serialATimer.Enabled = true;
                ReceiveReport();
                _serialATimer.Enabled = false;

                #region FP_CODE Pravin Ethernet Settings
                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                {
                    if (_serialReceivedByte == CORRECT_CRC)
                    {
                        SendCRCForEtherNEtSettings(_serialBCC);
                        ReceiveReport();
                        _deviceDownloadUSBPercentage(100);
                        if (_arrdwnlsetupframe[4] == 0)
                        {
                            if (_serialReceivedByte == 0xCC)
                            {
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                                CommonConstants.communicationStatus = 0;
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                                Close();
                                return SUCCESS;
                            }
                            else
                            {
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                                CommonConstants.communicationStatus = 0;
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                                Close();
                                return SUCCESS;
                            }
                        }
                    }
                }
                #endregion

                if (_serialReceivedByte == PRODUCT_MISSMATCH)	//	if '1' received = OK received
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strProductMismatch));
                    //_deviceDownloadUSBStatus(5);
                    Close();
                    return FAILURE;
                }

                if (_serialReceivedByte == EXPANSIONSLOT_MISMATCH)	//	if '1' received = OK received
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strExpansionMismatch));
                    Close();
                    return FAILURE;
                }

                if (_serialReceivedByte != CORRECT_CRC)	//	if '1' received = OK received
                    if (_serialReceivedByte != 0xef)
                    {
                        CommonConstants.communicationStatus = 0;
                        //_deviceDownloadUSBStatus(6);
                        Close();
                        return FAILURE;
                    }
            }
            //FP_CODE  R12  Haresh
            //Mode mismatch
            else if (_serialReceivedByte == _deviceFileID + 3)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                Close();
                return FAILURE;
            }
            //End
            else if (_serialReceivedByte == (_deviceFileID + 4))
            {
                ErrorReset_Command();
                return FAILURE;
            }

            else // Unit Not Ready
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(8);
                Close();
                return FAILURE;
            }
            return SUCCESS;
        }

        #region FP_CODE Pravin Ethernet Settings
        private void InitiateCommunicationForEthernetSetting()
        {
            if (_serialReceivedByte == _deviceFileID || _serialReceivedByte == _deviceFileID + TOLERANCE_BYTE_1 || _serialReceivedByte == _deviceFileID + TOLARANCE_BYTE_2)
            {
                if (_serialReceivedByte == _deviceFileID + 2)
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);

                    bool blFlag = false;
                    unsafe
                    {
                        fixed (byte* p = _serialSendBuff)
                        {
                            blFlag = obj.WriteUSB_Device(p, 4);
                        }
                    }

                    Thread.Sleep(100);
                    ReceiveReport();
                }
                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                {
                    int message = 256;
                    message <<= 7;
                    if (ClassList.CommonConstants.IsProductPLC(ClassList.CommonConstants.ProductDataInfo.iProductID)
                        && ClassList.CommonConstants.IsProductSupportsEthernet(ClassList.CommonConstants.ProductDataInfo.iProductID)
                        && !ClassList.CommonConstants.IsProductMXSpecialCase_Based(ClassList.CommonConstants.ProductDataInfo.iProductID))//FL_100_sammed
                    {
                        #region FL055_Product_Addition_Suyash
                        if (CommonConstants.ProductDataInfo.iProductID == CommonConstants.PRODUCT_FL055 || CommonConstants.ProductDataInfo.iProductID == CommonConstants.PRODUCT_FL050_V2) //Issue_1281 Vijay
                        {
                            GetSetFrameIntoBuffForEthSettings(DOWN);
                        }
                        else
                            GetEtherSettingFrame();
                        #endregion
                    }
                    else
                        GetSetFrameIntoBuffForEthSettings(DOWN);
                    Thread.Sleep(200);
                    {
                        bool blFlag = false;
                        unsafe
                        {
                            fixed (byte* p = _serialSendBuff)
                            {
                                if (ClassList.CommonConstants.IsProductPLC(ClassList.CommonConstants.ProductDataInfo.iProductID)
                                    && ClassList.CommonConstants.IsProductSupportsEthernet(ClassList.CommonConstants.ProductDataInfo.iProductID)
                                    && !ClassList.CommonConstants.IsProductMXSpecialCase_Based(ClassList.CommonConstants.ProductDataInfo.iProductID) && ClassList.CommonConstants.ProductDataInfo.iProductID != ClassList.CommonConstants.PRODUCT_FL055//FL055_Product_Addition_Suyash//FL_100_sammed
                                    && ClassList.CommonConstants.ProductDataInfo.iProductID != ClassList.CommonConstants.PRODUCT_FL050_V2) //Issue_1281 Vijay
                                {
                                    blFlag = obj.WriteUSB_Device(p, 0x13);
                                }
                                else
                                    blFlag = obj.WriteUSB_Device(p, 0x16);
                            }
                        }
                        ReceiveReport();
                    }

                    if (ClassList.CommonConstants.IsProductPLC(ClassList.CommonConstants.ProductDataInfo.iProductID)
                        && ClassList.CommonConstants.IsProductSupportsEthernet(ClassList.CommonConstants.ProductDataInfo.iProductID)
                        && ClassList.CommonConstants.ProductDataInfo.iProductID != ClassList.CommonConstants.PRODUCT_FL055
                        && !ClassList.CommonConstants.IsProductMXSpecialCase_Based(ClassList.CommonConstants.ProductDataInfo.iProductID) && ClassList.CommonConstants.ProductDataInfo.iProductID != ClassList.CommonConstants.PRODUCT_FL050_V2)//FL055_Product_Addition_Suyash//FL_100_sammed //Issue_1281 Vijay
                    {
                        if (_serialReceivedByte == 0x01)
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadEthernetSettings));
                            _deviceDownloadUSBPercentage(100);
                            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                            Close();

                        }
                        else
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                            Close();
                            return;
                        }
                    }
                    else
                    {
                        #region Product mismatch error for ethernet settings download
                        if (_serialReceivedByte == PRODUCT_MISSMATCH)	//	if '1' received = OK received
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strProductMismatch));
                            Close();
                            return;
                        }
                        _deviceDownloadUSBStatus(3 + message);
                        #endregion
                        if (_serialReceivedByte == CORRECT_CRC)
                        {
                            GetEtherSettingFrame(true);
                            bool blFlag = false;
                            unsafe
                            {
                                fixed (byte* p = _serialSendBuff)
                                {
                                    blFlag = obj.WriteUSB_Device(p, 0x40);
                                }
                            }
                            // _serialSendBuff[0] = _serialDHCPFlag ? (byte)0xFF : (byte)0x00;
                            //_serialPort.Write(_serialSendBuff, 0, 1);
                        }
                    }
                }
                ReceiveReport();
                if (_serialReceivedByte == PRODUCT_MISSMATCH)	//	if '1' received = OK received
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strProductMismatch));
                    Close();
                    return;
                }
                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                {
                    if (_serialReceivedByte == CORRECT_CRC)
                    {
                        SendCRCForEtherNEtSettings(_serialBCC);
                        Thread.Sleep(100);
                        ReceiveReport();
                        _deviceDownloadUSBPercentage(100);
                        if (_arrdwnlsetupframe[4] == 0)
                        {
                            if (_serialReceivedByte == 0xCC)
                            {
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                                CommonConstants.communicationStatus = 0;
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                                Close();
                                return;
                            }
                            else
                            {
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                                CommonConstants.communicationStatus = 0;
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                                Close();
                                return;
                            }
                        }
                    }
                }
            }
            //Ladder_change_R11
            //Mode mismatch
            else if (_serialReceivedByte == _deviceFileID + 3)
            {
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                Close();
                return;
            }
            //End
            else // Unit Not Ready
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(8);
                Close();
                return;
            }

            Close();

            //FP CODE Pravin Ethernet Settings
            if (_arrdwnlsetupframe[4] == 1)
            {
                Thread.Sleep(1000);
                //Special case for data download for FP4035T/FP4035T-E and FP4055T/FP4057T-E bcoz device gets soft restart
                if (CommonConstants.IsProductCompatibleWith4035(_deviceProdID) || CommonConstants.IsProductCompatibleWith4057(_deviceProdID)) //|| ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_FL050_V2)//Issue1537_FL50-v2_SY //24.10.2016_Vijay
                {
                    if (CommonConstants.IsProductMX257_Based(_deviceProdID)) //|| ClassList.CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_FL050_V2)//Issue1537_FL50-v2_SY //24.10.2016_Vijay
                    {
                        if (CommonConstants.IsProductSupportsEthernet(_deviceProdID))
                        {
                            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM + 4; i++)
                            {
                                Thread.Sleep(2000);
                                if (ReConnect_DataDownload() == CommonConstants.SUCCESS)
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(4000);
                        Connect();
                    }

                    if (_deviceIsConnected == false)
                    {
                        _deviceDownloadUSBStatus(10);
                        Close();
                        return;
                    }
                }
                //End
                ReceiveReport();
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
                if (DataDownload())
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                    Close();
                    return;
                }
                else
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    Close();
                    return;
                }

            }
            //End

            return;
        }

        private void SendCRCForEtherNEtSettings(byte btByte)
        {
            bool blFlag = false;
            byte[] btarr = new byte[2];

            btarr[0] = 0;
            btarr[1] = btByte;
            unsafe
            {
                fixed (byte* p = btarr)
                {
                    blFlag = obj.WriteUSB_Device(p, 2);
                }
            }

            _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
            _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _serialATimer.Enabled = true;



            _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
            _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _serialATimer.Enabled = true;
            // ReceiveReport(12);
            _serialATimer.Enabled = false;
        }
        #endregion

        public override void Close()
        {
            //_serialRecvBuff = null;
            obj.USB_CLose();
        }

        /// <summary>
        /// This is a thread routine called at the time of downloading,
        /// which helps not to overload on the main thread, which may be busy for GUI routines or other.
        /// </summary>
        public void DownLoadUSBFunc()
        {
            bool error = false;
            bool PrevDwnld = false;
            int bytesRead = 0;
            FileStream fs;

            CommonConstants.communicationStatus = 1;
            CommonConstants.communicationType = 1;

            //_deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloading));

            if (_deviceFileID != CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
            {
                FileInfo fi = new FileInfo(_deviceFileName);
                _deviceTotalFrames = Convert.ToUInt32(((uint)fi.Length) / DATAFRAMESIZE) + 1;
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
                if (_deviceFileID == CommonConstants.SERIAL_EXP_DNLD_FILEID)
                    message <<= 6;
                #region FP_CODE Pravin Ethernet Settings
                if (_deviceFileID == SERIAL_ETHER_SETTINGS_DNLD)
                    message <<= 7;
                #endregion
                _deviceDownloadUSBStatus(3 + message);

                fs = new FileStream(_deviceFileName, FileMode.Open);

                //uint iTotalFrames = Convert.ToUInt16(_deviceLength / (DATAFRAMESIZE));

                for (int i = 1; i <= _deviceTotalFrames; i++)
                {
                    if (_deviceDownloadUSBPercentage != null && CommonConstants.communicationStatus != -1 && CommonConstants.communicationStatus != -2)
                        _deviceDownloadUSBPercentage(100 * i / Convert.ToInt32(_deviceTotalFrames));

                    //Thread.Sleep(0);
                    if (i == (_deviceTotalFrames))
                    {
                        bytesRead = (Convert.ToInt32(_deviceLength % (DATAFRAMESIZE)));
                        if (_deviceLength >= fs.Position && bytesRead == 0)
                            bytesRead = Convert.ToInt32(_deviceLength - fs.Position);
                        if (bytesRead <= 0)
                            break;
                        fs.Read(_serialSendBuff, DATA_START_INDEX, bytesRead);
                    }
                    else
                        bytesRead = fs.Read(_serialSendBuff, DATA_START_INDEX, DATAFRAMESIZE);
                    CalculateCRC(bytesRead);

                    if (i == 1)
                        _serialTotalCrcByte = _serialCrcByte;

                    SendReport_DataFrame(i, bytesRead, TEN_SEC_DELAY);
                    if (!ReceiveReport())
                    {
                        error = true;
                        break;
                    }
                    if (CommonConstants.communicationStatus < 0)
                    {
                        //_deviceDownloadStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationTimeout));
                        error = true;
                        break;
                    }

                    if (_serialReceivedByte == CRC_ERROR)
                        fs.Seek(fs.Position - DATAFRAMESIZE, SeekOrigin.Begin); //samir
                    //_serialATimer.Enabled = false;
                }

                if (!error)
                    FinalizeUSBDownloading();
                else//Kapil_Issue_integration_#73
                    break;
                if (_deviceFileID == CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
                {
                    PrevDwnld = true;
                }
                _deviceFileName = GetFileID(_serialFileID);
                #region FP_CODE SURAJ 6-02-2012  //Issue_520_SurajP
                fs.Close();
                #endregion

                if (_deviceFileName == null)
                    break;

                //fs.Close();//comment by surajP

                //if (PrevDwnld && _deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                {
                    Close();
                    //Thread.Sleep(THREE_SEC_DELAY);
                    //Thread.Sleep(TEN_SEC_DELAY);
                    //FP_CODE Haresh
                    //Unit restart time adjusted
                    /* if (CommonConstants.IsProductFlexiPanels(_deviceProdID))
                     {
                         Thread.Sleep(10000);
                     }
                     else
                     {
                         Thread.Sleep(TEN_SEC_DELAY);
                     }*/

                    //End

                    //Haresh Feb10/2011 As informed by Alankar Longer Delay is not needed for Free scale products
                    if (CommonConstants.IsProductFreeScale(CommonConstants.ProductDataInfo.iProductID))
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Thread.Sleep(TEN_SEC_DELAY);
                    }

                    //Create new handle 
                    Connect();

                    if (CommonConstants.IsProductFreeScale(CommonConstants.ProductDataInfo.iProductID))
                    {
                        Thread.Sleep(500);
                    }
                    else
                    {
                        Thread.Sleep(THREE_SEC_DELAY);
                    }



                    if (_deviceIsConnected == false)
                    {
                        _deviceDownloadUSBStatus(10);
                        Close();
                        return;
                    }
                    PrevDwnld = false;
                }

                IsPrizmUSBReady();

                if (InitiateCommunication() != SUCCESS)
                    break;

                #region FP_CODE Pravin Ethernet Settings
                if (_deviceFileName == null || _deviceFileName == "" || error)
                {
                    break;
                }

                //FP_CODE Pravin Ethernet Settings
                //if (_deviceFileID != CommonConstants.SERIAL_APPLICATION_DNLD_FILEID && _deviceFileID != SERIAL_ETHER_SETTINGS_DNLD)
                if (_deviceFileID != CommonConstants.SERIAL_APPLICATION_DNLD_FILEID && _deviceFileName != null && _deviceFileName != string.Empty)
                #endregion
                {
                    int extraFrame = 1;
                    FileInfo finfo = new FileInfo(_deviceFileName);
                    if (((uint)finfo.Length) % DATAFRAMESIZE == 0)
                        extraFrame = 0;
                    _deviceTotalFrames = Convert.ToUInt32(((uint)finfo.Length) / DATAFRAMESIZE + extraFrame);
                    finfo = null;
                }
            }

            #region FP_CODE Pravin Ethernet Settings
            if (_arrdwnlsetupframe[4] == 1)
            {
                Thread.Sleep(1000);
                //Special case for data download for FP4035T/FP4035T-E and FP4055T/FP4057T-E bcoz device gets soft restart
                if (CommonConstants.IsProductCompatibleWith4035(_deviceProdID) || CommonConstants.IsProductCompatibleWith4057(_deviceProdID) || CommonConstants.IsProductMXSpecialCase_Based(_deviceProdID) || CommonConstants.IsProductFL005MicroPLCBase(_deviceProdID) || CommonConstants.IsProductSupportedFP4030MT(_deviceProdID) || CommonConstants.ProductDataInfo.iProductID == CommonConstants.PRODUCT_FL050_V2)//issue_FL100_748_sammed //Issue2.2_321 Vijay//Issue1537_FL50-v2_SY
                {
                    if (CommonConstants.IsProductCompatibleWith4035(_deviceProdID) || CommonConstants.IsProductCompatibleWith4057(_deviceProdID) || CommonConstants.IsProductMX257_Based(_deviceProdID) || CommonConstants.IsProductMXSpecialCase_Based(_deviceProdID) || CommonConstants.ProductDataInfo.iProductID == CommonConstants.PRODUCT_FL055 || CommonConstants.ProductDataInfo.iProductID == CommonConstants.PRODUCT_FL050_V2)//FL055_Product_Addition_Suyash//issue_FL100_748_sammed //Issue2.2_599 Vijay_Pravin//Issue_1281 Vijay
                    {
                        if (CommonConstants.IsProductSupportsEthernet(_deviceProdID))
                        {
                            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM + 4; i++)
                            {
                                Thread.Sleep(2000);
                                if (ReConnect_DataDownload() == CommonConstants.SUCCESS)
                                    break;
                            }
                        }
                        #region Issue2.2_599 Vijay_Pravin
                        else
                        {
                            Thread.Sleep(4000);
                            Connect();
                        }
                        #endregion
                    }
                    else
                    {
                        #region Issue2.2_321 Vijay
                        if (CommonConstants.IsProductFL005MicroPLCBase(_deviceProdID) || CommonConstants.IsProductSupportedFP4030MT(_deviceProdID))
                        {
                            Thread.Sleep(6000);
                            Connect();
                        }
                        else
                        {
                            Thread.Sleep(4000);
                            Connect();
                        }
                        #endregion
                    }

                    if (_deviceIsConnected == false)
                    {
                        _deviceDownloadUSBStatus(10);
                        Close();
                        return;
                    }
                }
                //End
                ReceiveReport();
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
                if (DataDownload())
                {
                    error = false;
                }
                else
                    error = true;

            }
            #endregion

            fs.Close();
            fs = null;
            Close();

            if (!error)
            {
                //if (EraseDataLoggerMemory)
                //    SendEraseDataLoggerMemoryFrame();
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
            }
            else if (CommonConstants.communicationStatus == -1 || CommonConstants.communicationStatus == -2)
            {
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationAborted));
            }
            else
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));

            }
            
            return;
        }

        public void DownLoadUSBFunc_AnalogModule()
        {
            bool error = false;
            bool PrevDwnld = false;
            int bytesRead = 0;
            FileStream fs;

            CommonConstants.communicationStatus = 1;
            CommonConstants.communicationType = 1;

            if (_deviceFileID != CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
            {
                FileInfo fi = new FileInfo(_deviceFileName);
                _deviceTotalFrames = Convert.ToUInt32(((uint)fi.Length) / DATAFRAMESIZE) + 1;
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
                if (_deviceFileID == CommonConstants.SERIAL_EXP_DNLD_FILEID)
                    message <<= 6;
                if (_deviceFileID == CommonConstants.SERIAL_ANALOGEXP_DNLD_FILEID)
                    message <<= 6;
                _deviceDownloadUSBStatus(3 + message);

                fs = new FileStream(_deviceFileName, FileMode.Open);

                for (int i = 1; i <= _deviceTotalFrames; i++)
                {
                    if (_deviceDownloadUSBPercentage != null && CommonConstants.communicationStatus != -1 && CommonConstants.communicationStatus != -2)
                        _deviceDownloadUSBPercentage(100 * i / Convert.ToInt32(_deviceTotalFrames));

                    if (i == (_deviceTotalFrames))
                    {
                        bytesRead = (Convert.ToInt32(_deviceLength % (DATAFRAMESIZE)));
                        if (_deviceLength >= fs.Position && bytesRead == 0)
                            bytesRead = Convert.ToInt32(_deviceLength - fs.Position);
                        if (bytesRead <= 0)
                            break;
                        fs.Read(_serialSendBuff, DATA_START_INDEX, bytesRead);
                    }
                    else
                        bytesRead = fs.Read(_serialSendBuff, DATA_START_INDEX, DATAFRAMESIZE);
                    CalculateCRC(bytesRead);

                    if (i == 1)
                        _serialTotalCrcByte = _serialCrcByte;

                    SendReport_DataFrame(i, bytesRead, TEN_SEC_DELAY);


                    if (!ReceiveReport())
                    {
                        error = true;
                        break;
                    }
                    if (_serialRecvBuff[0] == 0xFF)
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strCRCMismatch));
                        fs.Close();
                        fs = null;
                        Close();
                        return;
                    }
                    if (CommonConstants.communicationStatus < 0)
                    {
                        error = true;
                        break;
                    }

                    if (_serialReceivedByte == CRC_ERROR)
                        fs.Seek(fs.Position - DATAFRAMESIZE, SeekOrigin.Begin); //samir                    
                }

                if (!error)
                    FinalizeUSBDownloading();
                else//Kapil_Issue_integration_#73
                    break;
                if (_deviceFileID == CommonConstants.SERIAL_APPLICATION_DNLD_FILEID)
                {
                    PrevDwnld = true;
                }

                if (iTemp == 1)
                    break;

                //_deviceFileName = GetFileID(_serialFileID);
                _deviceFileName = strFileName2_Analog;
                _deviceFileID = m_byteSecond;
                if (_deviceFileName == null)
                    break;

                fs.Close();
                Close();
                Thread.Sleep(TEN_SEC_DELAY);
                Connect();
                Thread.Sleep(THREE_SEC_DELAY);
                if (_deviceIsConnected == false)
                {
                    _deviceDownloadUSBStatus(10);
                    Close();
                    return;
                }
                PrevDwnld = false;

                IsPrizmUSBReady();

                if (InitiateCommunication() != SUCCESS)
                    break;
            }

            if (!error)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
            }
            else if (CommonConstants.communicationStatus == -1 || CommonConstants.communicationStatus == -2)
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationAborted));
            else
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));

            }
            fs.Close();
            fs = null;
            Close();
            return;
        }

        private void FinalizeUSBDownloading()
        {
            _serialSendBuff[0] = DNLD_COMPLETE;
            _serialSendBuff[1] = _serialTotalCrcByte;
            unsafe
            {
                bool blFlag = false;
                fixed (byte* p = _serialSendBuff)
                {

                    blFlag = obj.WriteUSB_Device(p, 2);
                }
            }

            _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
            _serialATimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _serialATimer.Enabled = true;

            ReceiveReport();

            _serialATimer.Enabled = false;
            if (_serialSendBuff[0] == DNLD_COMPLETE)
            {
                if (_deviceFileID == CommonConstants.SERIAL_LADDER_DNLD_FILEID)
                {
                    _serialSendBuff[0] = 0x55;
                    ClassList.CommonConstants.g_LadderModified = false;

                    unsafe
                    {
                        bool blFlag = false;
                        fixed (byte* p = _serialSendBuff)
                        {
                            //    blFlag = obj.WriteUSB_Device(p, 1);
                        }
                    }
                    //Issue 333 SP 9.10.12
                    if (ClassList.CommonConstants.g_Support_IEC_Ladder && ClassList.CommonConstants.g_DownloadForOnLine == true)
                    {
                        _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strWaitDeviceIsInitializing));
                        if (ClassList.CommonConstants.IsProductSupportsEthernet(ClassList.CommonConstants.ProductDataInfo.iProductID))
                            System.Threading.Thread.Sleep(16000);
                        else
                            System.Threading.Thread.Sleep(10000);
                    }
                    //End
                    CommonConstants.downloadSucess = true;

                }
            }
            //Straton_change Haresh
           
        }


        /// <summary>
        /// Overloaded for Uploading
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool ReceiveReport(int size)
        {
            _serialTimeOut = 0;

            if (size == DOWN)
            {
                _serialTimeOut = 0;
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;

                try
                {
                    unsafe
                    {
                        fixed (byte* p = _serialRecvBuff)
                        {
                            _serialNoOfByteRead = obj.ReadUSB_Device(p, ACKFRAMESIZE);
                        }
                    }
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
                return false;
            }
            else if (size == 4)
            {
                _serialTimeOut = 0;
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;

                try
                {
                    unsafe
                    {
                        fixed (byte* p = _serialRecvBuff)
                        {
                            _serialNoOfByteRead = obj.ReadUSB_Device(p, size);
                        }
                    }
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
                return false;
            }
            //FP_CODE Pravin Data Download
            else if (size == 8)
            {
                _serialTimeOut = 0;
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;

                try
                {
                    unsafe
                    {
                        fixed (byte* p = _serialRecvBuff)
                        {
                            _serialNoOfByteRead = obj.ReadUSB_Device(p, 8);
                        }
                    }
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
                return false;
            }
            //End
            else if (size == 12)
            {
                _serialTimeOut = 0;
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;

                try
                {
                    unsafe
                    {
                        fixed (byte* p = _serialRecvBuff)
                        {
                            _serialNoOfByteRead = obj.ReadUSB_Device(p, size);
                        }
                    }
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
                return false;
            }
            else
            {
                _serialTimeOut = 0;
                _serialRecvBuff[0] = 0;
                _serialReceivedByte = 0;
                _serialATimer = new System.Timers.Timer(TEN_SEC_DELAY);
                _serialATimer.Enabled = true;
                try
                {
                    //Thread.Sleep(100);
                    unsafe
                    {
                        fixed (byte* p = _serialRecvBuff)
                        {

                            _serialNoOfByteRead = obj.ReadUSB_Device(p, 64);
                        }
                    }
                    if (_serialNoOfByteRead >= 64)
                        return true;
                }
                catch (System.TimeoutException timeOut)
                {
                    _deviceDownloadUSBStatus(9);
                    return false;
                }
                return false;
            }
        }



        /// <summary>
        /// This is a thread routine called at the time of uploading,
        /// which helps not to overload on the main thread, which may be busy for GUI routines or other.
        /// </summary>
        public void UpLoadFunc()
        {
            bool error = false;
            int bytesRead = 0;
            FileStream fs = new FileStream(_deviceFileName, FileMode.Create);

            CommonConstants.communicationStatus = 2;
            CommonConstants.communicationType = 2;

            if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD)
                ReceiveReport(DOWN);
            else
            {
                while (true)
                {
                    if (_serialRecvBuff[0] != SERIAL_LOGGEDDATA_UPLD)
                        ReceiveReport(DOWN);
                    else
                        break;
                }
            }
            for (int iTemp = 0; true; iTemp++)
            {
                for (int i = 1; ; i++)
                {
                    CommonConstants.communicationType = 2;

                    if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD && i == 1)
                    {
                        SendReport(1, 0, THREE_SEC_DELAY);
                        _serialNoOfByteTORead = 4;
                        ReceiveReport(4);
                        _deviceTotalFrames = CommonConstants.MAKEUINT(_serialRecvBuff);
                        _deviceTotalFrames = _deviceTotalFrames % 64 > 0 ? _deviceTotalFrames / 64 + 1 : _deviceTotalFrames / 64;



                    }

                    if (_deviceTotalFrames == 0)//For Logged data _deviceTotalFrames == 0 hence crash in below line 
                        break;

                    if (_deviceDownloadUSBPercentage != null)
                        _deviceDownloadUSBPercentage(100 * i / Convert.ToInt32(_deviceTotalFrames));

                    Thread.Sleep(0);
                    if (!ReceiveReport(UP))
                    {
                        /*error = true;
                        _deviceDownloadUSBPercentage(7);
                        Close();
                        return;*/

                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                        Close();
                        fs.Close();
                        i_RetVal = CommonConstants.FAILURE;
                        return;

                    }

                    if (_deviceFileID != SERIAL_LOGGEDDATA_UPLD && i == 1)
                        iProductID = ClassList.CommonConstants.MAKEWORD(_serialRecvBuff[4], _serialRecvBuff[5]);

                    if (CommonConstants.communicationStatus == -1)
                    {
                        #region Sammed
                        //_deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationAborted));


                        error = true;
                        break;
                        #endregion
                    }
                    if (_serialSendBuff[0] == 0xdd)
                        break;

                    bytesRead = 64;

                    fs.Write(_serialRecvBuff, 0, bytesRead);

                    if (i >= _deviceTotalFrames)
                        break;
                }
                //USB_PORT_RESET_ISSUE_SAMMED
                if (error)
                    break;
                //End

                //FP_CODE Pravin Application + Ladder Upload
                bool ldr_flag = false;
                if (_deviceFileName == CommonConstants.UPLOAD_LADDER_FILENAME)
                {
                    _serialSendBuff[0] = 0x55;
                    SendReport(1, 2, THREE_SEC_DELAY);
                    CommonConstants.LADDER_PRESENT = true;
                    ldr_flag = true;
                }

                _deviceFileName = GetUploadFileID(_serialFileID);

                if (!ldr_flag && _deviceFileName == CommonConstants.UPLOAD_LADDER_FILENAME)
                    _deviceFileName = null;

                if (_deviceFileName == null)
                    break;
                //End   

                fs.Close();

                if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                {
                    if ((CommonConstants.IsProductSupportsDataLogger(iProductID)) == false)
                    {
                        _serialSendBuff[0] = 0x55;
                        SendReport(1, 2, THREE_SEC_DELAY);

                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDataLogNotSupported));
                        fs = null;
                        Close();
                        i_RetVal = CommonConstants.FAILURE;
                        return;
                    }
                }

                //if (PrevDwnld && _deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                {
                    Close();
                    Thread.Sleep(THREE_SEC_DELAY);
                    //Thread.Sleep(TEN_SEC_DELAY);
                    //Thread.Sleep(TEN_SEC_DELAY);

                    //Create new handle 
                    Connect();
                    //Thread.Sleep(THREE_SEC_DELAY);
                    if (_deviceIsConnected == false)
                    {
                        _deviceDownloadUSBStatus(10);
                        Close();
                        return;
                    }
                    //PrevDwnld = false;
                }

                Thread.Sleep(500);
                IsPrizmUSBReady();
                //FP_CODE Pravin Application + Ladder Upload
                bool blFlag = false;
                if (ldr_flag)
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);
                    Thread.Sleep(100);

                    unsafe
                    {

                        fixed (byte* p = _serialSendBuff)
                        {

                            blFlag = obj.WriteUSB_Device(p, 4);
                        }
                    }
                }
                else
                {
                    _serialSendBuff[0] = 0x01;
                    blFlag = false;
                    unsafe
                    {
                        fixed (byte* p = _serialSendBuff)
                        {
                            blFlag = obj.WriteUSB_Device(p, 1);
                        }
                    }
                }
                //End
                Thread.Sleep(100);

                if (!ReceiveReport(4))
                    break;

                fs = new FileStream(_deviceFileName, FileMode.Create);

                if (_deviceFileID == SERIAL_LOGGEDDATA_UPLD)
                {
                    bytesRead = _serialRecvBuff[0];
                    bytesRead += _serialRecvBuff[1] * 256;
                    bytesRead += _serialRecvBuff[2] * 256 * 256;
                    bytesRead += _serialRecvBuff[3] * 256 * 256 * 256;
                    _deviceTotalFrames = (uint)bytesRead;

                    //_deviceTotalFrames = _deviceTotalFrames % 64 > 0 ? _deviceTotalFrames / 64 + 1 : _deviceTotalFrames / 64 + 1;

                    _deviceTotalFrames = _deviceTotalFrames % 64 > 0 ? _deviceTotalFrames / 64 + 1 : _deviceTotalFrames / 64;   //RC1_Issue1339 (Manik)
                }
                int message = 256;
                if (_deviceFileID == CommonConstants.SERIAL_LOGGED_UPLD_FILEID)
                    message <<= 4;
                _deviceDownloadUSBStatus(2 + message);
            }

            if (!error)//USB_PORT_RESET_ISSUE_SAMMED
            {
                Thread.Sleep(1000);
                if (true)
                {
                    _serialSendBuff[0] = 0x55;
                    SendReport(1, 2, THREE_SEC_DELAY);
                    //_serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
                else
                {
                    _serialSendBuff[0] = 0xdd;
                    SendReport(1, 1, THREE_SEC_DELAY);
                    //_serialPort.Write(_serialSendBuff, 0, ACKFRAMESIZE);
                }
            }

            fs.Close();
            Close();
            if (!error)
            {
                CommonConstants.communicationStatus = 0;
                CommonConstants.communicationType = 2;
                _deviceDownloadUSBStatus(18);
            }
            else if (CommonConstants.communicationStatus == -1)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationAborted));
            }
            else
            {
                CommonConstants.communicationType = 0;
                CommonConstants.communicationStatus = -1;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
            }
            return;
        }
        #endregion

        #region private methods
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
            Close();
            Thread.Sleep(THREE_SEC_DELAY);
            Connect();
            _serialSendBuff[0] = 0x66;
            bool blFlag = false;
            unsafe
            {
                fixed (byte* p = _serialSendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, 1);
                }
            }
            //SendReport(1, 1, THREE_SEC_DELAY);

            ReceiveReport();
            if (_serialReceivedByte == 0x67)
            {
                if (_serialSendBuff.Length < 4)
                    _serialSendBuff = new byte[4];
                _serialSendBuff[0] = 0x52;//REPL
                _serialSendBuff[1] = 0x45;
                _serialSendBuff[2] = 0x50;
                _serialSendBuff[3] = 0x4C;
                unsafe
                {
                    fixed (byte* p = _serialSendBuff)
                    {
                        blFlag = obj.WriteUSB_Device(p, 4);
                    }
                }
                //SendReport(1, 4, THREE_SEC_DELAY);
                //_serialPort.Write(_btREPL, 0, 4);
            }
            Close();
        }
        #endregion


        #region Ladder_change
        public int ReceiveFile_UploadLadder(string pFileName, byte pFileID)
        {
            _deviceFileName = pFileName;
            _serialFileID = pFileID;
            _serialSendBuff[0] = pFileID;
            _USBThread = new Thread(new ThreadStart(UpLoadFunc_UploadLadder));


            if (pFileID != SERIAL_LOGGEDDATA_UPLD)
            {
                if (ReadUSBData(pFileID) == CommonConstants.FAILURE)
                {
                    //_deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    //CommonConstants.communicationStatus = 0;
                    return FAILURE;
                }
            }
            else
                SendReport(pFileID, 1);

            //End

            if (_serialReceivedByte == pFileID || _serialReceivedByte == pFileID + TOLERANCE_BYTE_1 || _serialReceivedByte == pFileID + TOLARANCE_BYTE_2 || pFileID == SERIAL_LOGGEDDATA_UPLD)
            {	//	If Prizm Ready,
                _serialTimeOut = 0;
                if (_serialReceivedByte == pFileID + TOLARANCE_BYTE_2 && pFileID != SERIAL_LOGGEDDATA_UPLD)
                {
                    _serialSendBuff.SetValue((Byte)0x52, 0);
                    _serialSendBuff.SetValue((Byte)0x45, 1);
                    _serialSendBuff.SetValue((Byte)0x50, 2);
                    _serialSendBuff.SetValue((Byte)0x4c, 3);
                    Thread.Sleep(100);

                    unsafe
                    {
                        bool blFlag = false;
                        fixed (byte* p = _serialSendBuff)
                        {

                            blFlag = obj.WriteUSB_Device(p, 4);
                        }
                    }
                }
            }
            else if (_serialReceivedByte == pFileID + TOLERANCE_BYTE_5) //Application or ladder not present
            {
                //_deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strApplicationNotPresent));
                //CommonConstants.communicationStatus = 0;
                return 2;
            }
            else // Unit Not Ready
            {
                //_deviceDownloadStatus(7);
                return FAILURE;	//	FAILURE
            }
            //_deviceDownloadStatus(2);
            if (_serialFileID == SERIAL_LOGGEDDATA_UPLD)
                _serialSendBuff = new byte[1024 * 8 + 1 + DATA_START_INDEX + 4];

            _USBThread.Start();
            _USBThread.Join();
            //_deviceDownloadStatus(4);
            if (i_RetVal == CommonConstants.FAILURE)
                return FAILURE;
            else
                return SUCCESS;
        }

        public void UpLoadFunc_UploadLadder()
        {
            int bytesRead = 0;

            byte[] buffLadderData = new byte[4];

            FileStream fs = new FileStream(_deviceFileName, FileMode.Create);
            if (_serialFileID != SERIAL_LOGGEDDATA_UPLD)
                ReceiveReport(DOWN);


            else
            {
                while (true)
                {
                    if (_serialRecvBuff[0] != SERIAL_LOGGEDDATA_UPLD)
                        ReceiveReport(DOWN);
                    else
                        break;
                }
            }
            for (int i = 1; ; i++)
            {
                if (_serialFileID == SERIAL_LOGGEDDATA_UPLD && i == 1)
                {
                    ReceiveReport(DOWN);
                    bytesRead = _serialRecvBuff[0];
                    ReceiveReport(DOWN);
                    bytesRead += _serialRecvBuff[0] * 256;
                    _serialNoOfByteTORead = bytesRead + 2;
                }
                if (_serialFileID != SERIAL_LOGGEDDATA_UPLD && i == 1)
                {
                    SendReport(1, 0, THREE_SEC_DELAY);
                    _serialNoOfByteTORead = 4;
                    ReceiveReport(4);
                    //Ladder_change_R10
                    if (_serialFileID == CommonConstants.LADDER_UPLD_FILEID)
                    {
                        //This is applicable for first frame only
                        //Get Total size of data to be uploaded and calulate frames
                        buffLadderData[0] = _serialRecvBuff[6];
                        buffLadderData[1] = _serialRecvBuff[7];
                        buffLadderData[2] = _serialRecvBuff[8];
                        buffLadderData[3] = _serialRecvBuff[9];
                        _deviceTotalFrames = CommonConstants.MAKEUINT(buffLadderData);
                        _deviceTotalFrames += 8;

                        _deviceTotalFrames = CommonConstants.MAKEUINT(_serialRecvBuff);
                    }
                    //End///
                    else
                    {
                        _deviceTotalFrames = CommonConstants.MAKEUINT(_serialRecvBuff);

                    }

                    _deviceTotalFrames = _deviceTotalFrames % 64 > 0 ? _deviceTotalFrames / 64 + 1 : _deviceTotalFrames / 64 + 1;
                }

                /*if (_deviceDownloadUSBPercentage != null)
                    _deviceDownloadUSBPercentage(100 * i / Convert.ToInt32(_deviceTotalFrames));*/


                Thread.Sleep(0);
                if (!ReceiveReport(UP))
                {
                    //break;
                }
                if (_serialSendBuff[0] == 0xdd)
                    break;

                bytesRead = 64;

                fs.Write(_serialRecvBuff, 0, bytesRead);

                if (i >= _deviceTotalFrames)
                    break;
            }
            if (true)
            {
                _serialSendBuff[0] = 0x55;
                SendReport(1, 2, THREE_SEC_DELAY);
            }
            else
            {
                _serialSendBuff[0] = 0xFF;
                SendReport(1, 0, THREE_SEC_DELAY);
            }
            fs.Close();
            //Close();
            return;
        }

        //FP_CODE Pravin USB Timeout - Rev 1
        public int Plc_Download_Protocol(string pFileName, byte pFileID)
        {
            /*GetFileID(pFileID);
            if (pFileID != 2)
                return SUCCESS;*/

            byte initialByte = 0x70;
            byte temp_receivedbyte = 0;

            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
            if (_deviceIsConnected == false)
            {
                Close();
                return FAILURE;
            }

            if (ReadUSBData(initialByte) == CommonConstants.FAILURE)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                return FAILURE;
            }

            if (_serialReceivedByte != (initialByte + TOLERANCE_BYTE_1) && _serialReceivedByte != (initialByte + TOLARANCE_BYTE_2)
                            && _serialReceivedByte != (initialByte + TOLARANCE_BYTE_3) && _serialReceivedByte != (initialByte + TOLARANCE_BYTE_4))
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                return FAILURE;
            }
            temp_receivedbyte = _serialReceivedByte;

            _serialSendBuff.SetValue((Byte)0x52, 0);
            _serialSendBuff.SetValue((Byte)0x45, 1);
            _serialSendBuff.SetValue((Byte)0x50, 2);
            _serialSendBuff.SetValue((Byte)0x4c, 3);

            bool blFlag = false;
            unsafe
            {
                fixed (byte* p = _serialSendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, 4);
                }
            }
            try
            {
                unsafe
                {
                    fixed (byte* p = _serialRecvBuff)
                    {
                        _serialNoOfByteRead = obj.ReadUSB_Device(p, 64);
                    }
                }

            }
            catch (System.TimeoutException timeOut)
            {
                return FAILURE;
            }


            if (_serialNoOfByteRead > 0)
            {
                uint tempID = CommonConstants.MAKEUWORD(_serialRecvBuff[1], _serialRecvBuff[2]);

                #region Download_NewHardwareChnages_Files Vijay
                string tempStr = _serialRecvBuff[3].ToString("X");
                tempStr = tempStr + ".";
                strbootblock_Base = _serialRecvBuff[4].ToString("X");
                if (strbootblock_Base.Length == 1)
                    strbootblock_Base = "0" + strbootblock_Base;
                strbootblock_Base = tempStr + strbootblock_Base;
                BootBlockVersion = strbootblock_Base;
                #endregion

                #region FP_Vertical_Product_Change_AMIT
                if (ClassList.CommonConstants.IsProductCompatibleWith4030MT(_deviceProdID))
                {
                    if (CommonConstants.Is4030MTHorizontalProduct(Convert.ToInt32(tempID)) 
                        && CommonConstants.Is4030MTVerticalProduct(_deviceProdID))//FP4030MT_addition_AMIT
                        tempID = Convert.ToUInt16(_deviceProdID);
                    else if (CommonConstants.Is4030MTVerticalProduct(Convert.ToInt32(tempID))//VerticalOri_SY_Keypad-4030MT
                            && CommonConstants.Is4030MTHorizontalProduct(_deviceProdID))//FP4030MT_addition_AMIT
                        tempID = Convert.ToUInt16(_deviceProdID);
                }
                #endregion

                #region New_Product_Addition_Vertical Vijay-AMIT
                //This is done bcoz same Hardware is used for horizontal and vertical products with different product IDs
                //If product selected is horizontal and firmware present in unit is Vertical assign tempID == Vertical product ID
                //If product selected is vertical and firmware present in unit is Horizontal assign tempID == Horizontal product ID
                //In order to avoid product mistmatch error
                if (_deviceProdID == CommonConstants.PRODUCT_FP4057T_E || _deviceProdID == CommonConstants.PRODUCT_FP4057T_E_VERTICAL)
                {
                    if (tempID == CommonConstants.PRODUCT_FP4057T_E || tempID == CommonConstants.PRODUCT_FP4057T_E_VERTICAL)
                        tempID = Convert.ToUInt16(_deviceProdID);
                }
                #endregion 
                if (tempID == _deviceProdID)
                {
                    if (temp_receivedbyte == (initialByte + TOLARANCE_BYTE_4))
                    {
                        if (_serialRecvBuff[58] == 1)
                            ErrorReset_Command_New();
                        else
                            ErrorReset_Command();
                        return FAILURE;
                    }
                    else
                    {
                        if (temp_receivedbyte == (initialByte + TOLERANCE_BYTE_1) || temp_receivedbyte == (initialByte + TOLARANCE_BYTE_2))
                            return ONLYBOOTBLOCK;
                        else if (temp_receivedbyte == (initialByte + TOLARANCE_BYTE_3))
                        {
                            if (_arrdwnlsetupframe[0] == 1)
                                return SUCCESS;
                            else
                            {
                                CommonConstants.communicationStatus = 0;
                                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                                Close();
                                return FAILURE;

                            }
                        }
                        else
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                            Close();
                            return FAILURE;
                        }
                    }
                }
                else
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strProductMismatch));
                    Close();
                    return FAILURE;
                }
            }
            return FAILURE;
        }

        //FP_CODE Pravin Product Information
        public int Plc_Download_Protocol(ref byte[] s_ProductInfo)
        {
            byte initialByte = 0x70;
            byte check_PLCinRunMode = 0xA0;

            if (_deviceIsConnected == false)
            {
                Close();
                return FAILURE;
            }

            if (ReadUSBData(initialByte) == CommonConstants.FAILURE)
            {
                CommonConstants.communicationStatus = 0;
                return FAILURE;
            }


            _serialSendBuff.SetValue((Byte)0x52, 0);
            _serialSendBuff.SetValue((Byte)0x45, 1);
            _serialSendBuff.SetValue((Byte)0x50, 2);
            _serialSendBuff.SetValue((Byte)0x4c, 3);

            bool blFlag = false;
            unsafe
            {
                fixed (byte* p = _serialSendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, 4);
                }
            }

            try
            {
                MemoryStream ms = new MemoryStream();
                unsafe
                {
                    fixed (byte* p = _serialRecvBuff)
                    {
                        _serialNoOfByteRead = obj.ReadUSB_Device(p, 64);
                        ms.Write(_serialRecvBuff, 0, 64);
                    }

                    int NoOfFrames = _serialRecvBuff[60];
                    for (int i = 0; i < NoOfFrames; i++)
                    {
                        byte tempinitialByte = 0x71;
                        byte nextChunkByte = 0x01;
                        byte* p;

                        if (i == 0)
                            _serialSendBuff[0] = tempinitialByte;
                        else
                            _serialSendBuff[0] = nextChunkByte;

                        unsafe
                        {
                            fixed (byte* a = _serialSendBuff)
                            {
                                blFlag = obj.WriteUSB_Device(a, 1);
                            }
                        }

                        Thread.Sleep(1000);
                        unsafe
                        {
                            fixed (byte* b = _serialRecvBuff)
                            {
                                _serialNoOfByteRead = obj.ReadUSB_Device(b, 64);
                                ms.Write(_serialRecvBuff, 0, 64);
                            }
                        }
                    }
                    s_ProductInfo = ms.ToArray();
                }

            }
            catch (System.TimeoutException timeOut)
            {
                return FAILURE;
            }

            if (_serialNoOfByteRead > 0)
            {
                //s_ProductInfo = _serialRecvBuff;
                return SUCCESS;
            }
            //}
            return FAILURE;
        }
        //End

        //FP_CODE Pravin Download Mode
        public int PutUnit_HaltMode_BeforeDwnl(string pFileName, byte pFileID)
        {
            byte initialByte = 0xD0;
            GetFileID(pFileID);

            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
            if (_deviceIsConnected == false)
            {
                Close();
                return FAILURE;
            }

            if (ReadUSBData(initialByte) == CommonConstants.FAILURE)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                return FAILURE;
            }
            //End

            if (_serialReceivedByte == (initialByte + TOLARANCE_BYTE_3))
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                Close();
                return FAILURE;
            }
            else if (_serialReceivedByte == (initialByte + TOLARANCE_BYTE_4))
            {
                ErrorReset_Command();
                return FAILURE;
            }
            else if (_serialReceivedByte == (initialByte + TOLARANCE_BYTE_2))
            {
                _serialSendBuff.SetValue((Byte)0x52, 0);
                _serialSendBuff.SetValue((Byte)0x45, 1);
                _serialSendBuff.SetValue((Byte)0x50, 2);
                _serialSendBuff.SetValue((Byte)0x4c, 3);

                bool blFlag = false;
                unsafe
                {
                    fixed (byte* p = _serialSendBuff)
                    {
                        blFlag = obj.WriteUSB_Device(p, 4);
                    }
                }

                try
                {
                    unsafe
                    {
                        fixed (byte* p = _serialRecvBuff)
                        {
                            _serialNoOfByteRead = obj.ReadUSB_Device(p, 1);
                        }
                    }

                }
                catch (System.TimeoutException timeOut)
                {
                    return FAILURE;
                }
                if (_serialNoOfByteRead > 0)
                {
                    //FP_CODE Pravin Data Download
                    if (_arrdwnlsetupframe[4] == 1 && _serialFileID == 0)
                    {
                        _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
                        if (DataDownload())
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                            return FAILURE;
                        }
                        else
                        {
                            CommonConstants.communicationStatus = 0;
                            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                            return FAILURE;
                        }
                    }
                    //End      
                    return SUCCESS;

                }
            }
            else if (_serialReceivedByte == (initialByte + TOLERANCE_BYTE_1))
            {
                return SUCCESS;
            }
            return FAILURE;
        }


        //FP_CODE Pravin USB timeout - Rev 1
        private int IsPrizmUSBReady_Initial()
        {
            if (ReadUSBData(_deviceFileID) == CommonConstants.SUCCESS)
                return SUCCESS;
            else
                return FAILURE;
        }

        private int ReadUSBData(byte initialByte)
        {
            Thread _USBReadThread;
            for (int i = 0; i < NO_OF_TRY_FOR_READY_PRIZM; i++)
            {
                b_USBReadCompleteFlag = false;
                _USBReadThread = new Thread(delegate()
                {
                    ReadFunction(initialByte);
                });
                _USBReadThread.IsBackground = true;
                _USBReadThread.Start();

                for (int j = 0; j < CommonConstants.SleepMultiplier; j++)
                {
                    if (b_USBReadCompleteFlag)
                        break;
                    Thread.Sleep(CommonConstants.SleepCount);
                }

                if (b_USBReadCompleteFlag)
                {
                    refCount = 0;
                    break;
                }
                else
                {
                    //USB_PORT_RESET - START
                    if (ClassList.CommonConstants.IsProductUSBPortReset(ClassList.CommonConstants.ProductDataInfo.iProductID))
                        obj.USB_CancelPendingIO();
                    //End
                    else
                        refCount++;
                }
            }

            if (!b_USBReadCompleteFlag)
                return FAILURE;
            else
                return SUCCESS;
        }

        private void ReadFunction(byte m_temp_initialByte)
        {
            SendReport(m_temp_initialByte, 1);
            Thread.Sleep(100);
            if (refCount == 0)
            {
                if (!b_USBReadCompleteFlag)
                {
                    if (ReceiveReport())
                        b_USBReadCompleteFlag = true;
                    else
                        b_USBReadCompleteFlag = false;
                }
            }

        }
        //End

        private void ErrorReset_Command()
        {
            bool b_chk = false;
            //_deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));

            DialogResult result;
            result = MessageBox.Show("PLC is in Error mode, select 'Yes' to reset error mode or 'No' to quit.", "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                #region Issue_838_FL100_Sammed
                if (ClassList.CommonConstants.g_Support_IEC_Ladder && CommConst.IsProductPLC(ClassList.CommonConstants.ProductDataInfo.iProductID))
                {
                    #region SS_DefaultTagEdit
                    String strVarName_1 = "Error_Flag_1";
                    String strVarName_2 = "Error_Flag_2";
                    String strVarName = "PLC_mode_control";
                    //String strVarName_1 = ClassList.ProjectTagInformation.getTagInfoDefaultTag(ClassList.CommonConstants.SelectedProjectID, "MW0001", 50, 2, 0)._TagName;
                    //String strVarName_2 = ClassList.ProjectTagInformation.getTagInfoDefaultTag(ClassList.CommonConstants.SelectedProjectID, "MW0002", 50, 2, 0)._TagName;
                    //string strVarName = ClassList.ProjectTagInformation.getTagInfoDefaultTag(ClassList.CommonConstants.SelectedProjectID, ClassList.CommonConstants.SystemTag_PLCMode, 50, 2, 0)._TagName;
                    #endregion
                    uint address1 = 0;
                    uint address2 = 0;
                    uint address = 0;
                    address1 = GetOffsetAddress(strVarName_1);
                    address2 = GetOffsetAddress(strVarName_2);
                    address = GetOffsetAddress(strVarName);

                    b_chk = WriteQuery_PLCMode_IEC(address1, 0x00);
                    WriteQuery_PLCMode_IEC(address2, 0x00);
                    WriteQuery_PLCMode_IEC(address, 0x01);
                    if (!b_chk)
                        MessageBox.Show("Failed to execute error reset command");

                }
                else
                {
                #endregion
                    b_chk = WriteQuery_PLCMode(ClassList.CommonConstants.SystemTag_ErrorHandle1, 0x00);
                    WriteQuery_PLCMode(ClassList.CommonConstants.SystemTag_ErrorHandle2, 0x00);
                    WriteQuery_PLCMode(ClassList.CommonConstants.SystemTag_PLCMode, 0x01);
                    if (!b_chk)
                        MessageBox.Show("Failed to execute error reset command");
                }

                
            }

            CommonConstants.communicationStatus = 0;
            if (b_chk)
            {
                ClassList.CommonConstants.IsErrorResetChecked = true;//ShitalG Utility
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strReady));
            }
            else
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorModeMismatch));
            Close();
        }

        private void ErrorReset_Command_New()
        {
            bool b_chk = false;
            //_deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));

            DialogResult result;
            result = MessageBox.Show("PLC is in Error mode, select 'Yes' to reset error mode or 'No' to quit.", "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                /*  #region Issue_838_FL100_Sammed
                  if (ClassList.CommonConstants.g_Support_IEC_Ladder && CommConst.IsProductPLC(ClassList.CommonConstants.ProductDataInfo.iProductID))
                  {
                      #region SS_DefaultTagEdit
                      //String strVarName_1 = "Error_Flag_1";
                      //String strVarName_2 = "Error_Flag_2";
                      //String strVarName = "PLC_mode_control";
                      String strVarName_1 = ClassList.ProjectTagInformation.getTagInfoDefaultTag(ClassList.CommonConstants.SelectedProjectID, "MW0001", 50, 2, 0)._TagName;
                      String strVarName_2 = ClassList.ProjectTagInformation.getTagInfoDefaultTag(ClassList.CommonConstants.SelectedProjectID, "MW0002", 50, 2, 0)._TagName;
                      string strVarName = ClassList.ProjectTagInformation.getTagInfoDefaultTag(ClassList.CommonConstants.SelectedProjectID, ClassList.CommonConstants.SystemTag_PLCMode, 50, 2, 0)._TagName;
                      #endregion
                      uint address1 = 0;
                      uint address2 = 0;
                      uint address = 0;
                      address1 = GetOffsetAddress(strVarName_1);
                      address2 = GetOffsetAddress(strVarName_2);
                      address = GetOffsetAddress(strVarName);

                      b_chk = WriteQuery_PLCMode_IEC(address1, 0x00);
                      WriteQuery_PLCMode_IEC(address2, 0x00);
                      WriteQuery_PLCMode_IEC(address, 0x01);
                      if (!b_chk)
                          MessageBox.Show("Failed to execute error reset command");

                  }
                  else
                  {
                  #endregion
                      b_chk = WriteQuery_PLCMode(ClassList.CommonConstants.SystemTag_ErrorHandle1, 0x00);
                      WriteQuery_PLCMode(ClassList.CommonConstants.SystemTag_ErrorHandle2, 0x00);
                      WriteQuery_PLCMode(ClassList.CommonConstants.SystemTag_PLCMode, 0x01);
                      if (!b_chk)
                          MessageBox.Show("Failed to execute error reset command");
                  }*/
                b_chk = WriteQuery_ErrorResetCommand();
                if (b_chk == false)
                {
                    MessageBox.Show("Failed to execute error reset command");
                }
               
            }

            CommonConstants.communicationStatus = 0;
            if (b_chk)
            {
                ClassList.CommonConstants.IsErrorResetChecked = true;//ShitalG
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strReady));
            }
                // else
             //   _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorModeMismatch));
            Close();
        }
        //ssaa
        public UInt32 GetOffsetAddress(String strTagName)
        {
            UInt32 addrValue = 0;
            String strLineText = "";
            String strFileName = "";
            byte[] btTagAddressBytes = new byte[6];
            int LenExt = ClassList.CommonConstants.ProjectExtension.Length;
            strFileName = CommonConstants.g_ProjectPath.Remove(CommonConstants.g_ProjectPath.Length - LenExt, LenExt);

            strFileName = strFileName + "\\" + ClassList.CommonConstants._IECFoldername;//Yadav_Sanjay
            strFileName += "\\APPLI.SYB";
            String strTemp = "";
            String strAddress = "";
            bool found = false;


            if (File.Exists(strFileName))
            {
                System.IO.StreamReader file = new System.IO.StreamReader(strFileName);
                while ((strLineText = file.ReadLine()) != null)
                {
                    strTemp = strLineText;
                    if (strLineText.Contains("=")) //Haresh added to resolve CPO file setting issue 
                    {
                        string[] tempStr = strLineText.Split('=');
                        if (tempStr[1] == strTagName)
                        //   if (strTemp.Contains(strTagName) && strTemp.Contains("0x"))
                        {
                            found = true;
                            break;
                        }
                    }

                }

                if (found)
                {
                    strTemp = strTemp.Remove(0, 2);
                    strTemp = strTemp.Remove(8, strTemp.Length - 8);
                    if (strTemp.Length == 8)
                    {
                        addrValue = Convert.ToUInt32(strTemp, 16);
                    }
                }
                file.Close();
            }

            return addrValue;
        }
        //

        private bool WriteQuery_PLCMode(string SystemRegisterAddr, int QueryID)
        {
            byte[] WriteBuff = new byte[8];

            MakeFrame_WriteRegisterQuery(SystemRegisterAddr, QueryID, ref WriteBuff);
            SendWriteQuery(ref WriteBuff);

            if (!ReceiveReport(8))
            {
                return false;
            }

            return true;
        }
        public bool WriteQuery_ErrorResetCommand()
        {
            byte[] WriteBuff = new byte[8];
            WriteBuff[0] = 0x45;
            WriteBuff[1] = 0x52;
            WriteBuff[2] = 0x52;
            WriteBuff[3] = 0x53;
            WriteBuff[4] = 0x54;
            WriteBuff[5] = 0;
            WriteBuff[6] = 0;
            WriteBuff[7] = 0;

            SendWriteQuery(ref WriteBuff);

            if (!ReceiveReport(8))
            {
                return false;
            }
            return true;
        }

        #region Issue_838_FL100_Sammed
        private bool WriteQuery_PLCMode_IEC(uint SystemRegisterAddr, int QueryID)
        {
            byte[] WriteBuff = new byte[21];
            int i;
            MakeFrame_WriteRegisterQuery_IEC(SystemRegisterAddr, QueryID, ref WriteBuff);
            SendWriteQuery_IEC(ref WriteBuff);


            for (i = 0; i < 14; i++)
            {
                ReceiveReport();
            }
            if (i != 14)
            {
                return false;
            }
            return true;
        }
        public void MakeFrame_WriteRegisterQuery_IEC(uint address, int value, ref byte[] WriteBuff)
        {
            ClassList.LadderMonitor _objLadderMonitor = null;

            //FP Code Pravin Serial Monitor
            _objLadderMonitor = new ClassList.LadderMonitor(ClassList.CommonConstants.ON_LINE_COMMUNICATION);
            //End

            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];
            int bufflen = 0;

            WriteBuff = new byte[21];

            WriteBuff[bufflen++] = 0x54;
            WriteBuff[bufflen++] = 0x35;


            WriteBuff[bufflen++] = 0x00;
            WriteBuff[bufflen++] = 0x0F;
            WriteBuff[bufflen++] = 0x80;
            WriteBuff[bufflen++] = 0x01;
            WriteBuff[bufflen++] = 0x0B;
            WriteBuff[bufflen++] = 0x00;
            WriteBuff[bufflen++] = 0x57;
            WriteBuff[bufflen++] = 0x01;

            uint raw_variable;
            raw_variable = (address) & 255;
            temp_arr[0] = Convert.ToByte(raw_variable);
            raw_variable = address;
            raw_variable = raw_variable >> 8;
            temp_arr[1] = Convert.ToByte((raw_variable & 255));

            WriteBuff[bufflen++] = temp_arr[0];
            WriteBuff[bufflen++] = temp_arr[1];

            WriteBuff[bufflen++] = 0x00;
            WriteBuff[bufflen++] = 0x11;
            WriteBuff[bufflen++] = Convert.ToByte(value);

            WriteBuff[bufflen++] = 0x00;
            WriteBuff[bufflen++] = 0x00;
            WriteBuff[bufflen++] = 0x00;
            WriteBuff[bufflen++] = 0x00;

            _objLadderMonitor.CalculateCRC(ref CRC, WriteBuff);
            WriteBuff[bufflen++] = CRC[1];
            WriteBuff[bufflen++] = CRC[0];
        }
        #endregion

        public void MakeFrame_WriteRegisterQuery(string pStr, int value, ref byte[] WriteBuff)
        {
            ClassList.LadderMonitor _objLadderMonitor = null;

            //FP Code Pravin Serial Monitor
            _objLadderMonitor = new ClassList.LadderMonitor(ClassList.CommonConstants.ON_LINE_COMMUNICATION);
            //End

            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];
            int bufflen = 0;

            WriteBuff = new byte[8];

            WriteBuff[bufflen++] = 0x00;
            WriteBuff[bufflen++] = 0x06;

            temp_arr = _objLadderMonitor.GetAddress(pStr);
            WriteBuff[bufflen++] = temp_arr[1];
            WriteBuff[bufflen++] = temp_arr[0];


            int raw_variable;
            raw_variable = (value) & 255;
            temp_arr[0] = Convert.ToByte(raw_variable);
            raw_variable = value;
            raw_variable = raw_variable >> 8;
            temp_arr[1] = Convert.ToByte((raw_variable & 255));

            WriteBuff[bufflen++] = temp_arr[1];
            WriteBuff[bufflen++] = temp_arr[0];

            _objLadderMonitor.CalculateCRC(ref CRC, WriteBuff);
            WriteBuff[bufflen++] = CRC[0];
            WriteBuff[bufflen++] = CRC[1];
        }

        public void SendWriteQuery(ref byte[] WriteBuff)
        {
            int len_buff = 8;

            unsafe
            {
                bool blFlag = false;
                fixed (byte* p = WriteBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, len_buff);
                }
            }

        }
        //Issue_838_FL100_Sammed
        public void SendWriteQuery_IEC(ref byte[] WriteBuff)
        {
            int len_buff = 21;

            unsafe
            {
                bool blFlag = false;
                fixed (byte* p = WriteBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, len_buff);
                }
            }

        }
        //Issue_838_FL100_Sammed
        public bool GetResponse_WriteQuery()
        {
            int len = 8;
            int NoOfByteRead = 0;

            byte[] RecvBuff = new byte[len];

            DateTime endTime = DateTime.Now.AddMilliseconds(20);
            try
            {
                unsafe
                {
                    fixed (byte* p = RecvBuff)
                    {
                        NoOfByteRead = obj.ReadUSB_Device(p, len);
                    }
                }
            }
            catch (System.TimeoutException timeOut)
            {
                return false;
            }
            if (NoOfByteRead > 0)
            {
                return true;
            }
            return false;
        }

        //FP_CODE Pravin Data Download
        public bool DataDownload()
        {
            ClassList.LadderMonitor _ObjLadderMonitor = null;
            //FP Code Pravin Serial Monitor
            _ObjLadderMonitor = new ClassList.LadderMonitor(ClassList.CommonConstants.ON_LINE_COMMUNICATION);
            //End  

            byte[] SendBuff;
            //DmBlockInfo objDMBlockInfo;
            int Total_Frames = 0;
            int bytesRead = 0;
            int NumberofRegisterRead = 0;
            int Read_Cnt = 0;
            int MAXBIT = 52;
            int MAXREGISTER = 26;
            int REMAININGBYTE = 12;
            bool bflagResponse = false;
            const int FRAMESIZE = 64;
            int m_i_RetryCount = 3;
            int LengthBuffer = 0;

            m_dataList = ClassList.CommonConstants.objListDataMonitorData;

            _ObjLadderMonitor.InitDataMonitor_BlockListInfo(m_dataList);
            Total_Frames = _ObjLadderMonitor.Calculate_DataMonitorTotalFrames();

            if (Initial_ProtocolForDataMonitor(Total_Frames) == false)
                return false;
            else
            {
                /*for (int i = 0; i < m_dataList.Count; i++)
                {
                    objDMBlockInfo = (DmBlockInfo)m_dataList[i];

                    if (objDMBlockInfo.strType == "Bit")
                        Total_Frames = (objDMBlockInfo.TagList.Count / MAXBIT) + 1;
                    else
                        Total_Frames = (objDMBlockInfo.TagList.Count / MAXREGISTER) + 1;

                    if (_deviceDownloadUSBPercentage != null && CommonConstants.communicationStatus != -1 && CommonConstants.communicationStatus != -2)
                        _deviceDownloadUSBPercentage(100 * (i + 1) / Convert.ToInt32(m_dataList.Count));

                    Read_Cnt = 0;
                    for (int iTemp = 1; iTemp <= Total_Frames; iTemp++)
                    {

                        if (iTemp == (Total_Frames))
                        {
                            if (objDMBlockInfo.strType == "Bit")
                            {
                                bytesRead = ((objDMBlockInfo.TagList.Count) % (FRAMESIZE - REMAININGBYTE));
                                NumberofRegisterRead = objDMBlockInfo.TagList.Count % MAXBIT;
                            }
                            else
                            {
                                bytesRead = ((objDMBlockInfo.TagList.Count * 2) % (FRAMESIZE - REMAININGBYTE));
                                NumberofRegisterRead = objDMBlockInfo.TagList.Count % MAXREGISTER;
                            }
                            if (bytesRead == 0)
                                break;
                        }
                        else
                        {
                            bytesRead = FRAMESIZE - REMAININGBYTE;

                            if (objDMBlockInfo.strType == "Bit")
                                NumberofRegisterRead = MAXBIT;
                            else
                                NumberofRegisterRead = MAXREGISTER;
                        }

                        SendBuff = new byte[bytesRead + 10];

                        if (objDMBlockInfo.strType == "Bit")
                        {
                            LengthBuffer = bytesRead + 10;
                            SendBuff = new byte[LengthBuffer];
                            SendBuff = _ObjLadderMonitor.MakeFrame_MultipleCoilWrite(iTemp, i, bytesRead, NumberofRegisterRead, Read_Cnt);
                        }
                        else
                        {
                            LengthBuffer = bytesRead + 9;
                            SendBuff = new byte[LengthBuffer];
                            SendBuff = _ObjLadderMonitor.MakeFrame_MultipleRegisterWrite(iTemp, i, bytesRead, NumberofRegisterRead, Read_Cnt);
                        }

                        SendConfigureQuery(LengthBuffer, SendBuff);
                        if (!ReceiveReport(8))
                            return false;

                        Read_Cnt += NumberofRegisterRead;

                        if (b_USBReadCompleteFlag == true)
                            continue;
                        else
                            return false;
                    }
                }*/
            }

            return true;
        }

        public int GetDataDownload()
        {
            _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strConnectingtoPrizm));
            if (DataDownload())
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                return SUCCESS;
            }
            else
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadUSBStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                return FAILURE;
            }
        }

        public void SendConfigureQuery(int Length_Buffer, byte[] SendBuff)
        {
            unsafe
            {
                bool blFlag = false;
                fixed (byte* p = SendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, Length_Buffer);
                }
            }

        }

        public bool Initial_ProtocolForDataMonitor(int Total_Frames)
        {
            byte initialByte = 0x80;
            int iNumberofFrames = 0;
            byte[] m_sendBuff = new byte[4];
            byte[] m_receBuff = new byte[2];
            m_sendBuff[0] = initialByte;

            WriteFrame(1, m_sendBuff);
            ReceiveFrame(ref m_receBuff, 1);

            if (_serialReceivedByte == initialByte + 2)
            {
                m_sendBuff.SetValue((Byte)0x52, 0);
                m_sendBuff.SetValue((Byte)0x45, 1);
                m_sendBuff.SetValue((Byte)0x50, 2);
                m_sendBuff.SetValue((Byte)0x4c, 3);

                WriteFrame(4, m_sendBuff);
                ReceiveFrame(ref m_receBuff, 1);
            }

            if (_serialReceivedByte == initialByte + 1)
            {
                GetSetFrameIntoBuff();
                WriteFrame(STARTFRAMESIZE, _serialSendBuff);

                if (!ReceiveReport())
                    return false;
                else
                {
                    if (_serialReceivedByte != CORRECT_CRC)	//	if '1' received = OK received
                        return false;
                }

                m_sendBuff = CommonConstants.GetHalfWord(Total_Frames);
                WriteFrame(2, m_sendBuff);
                ReceiveFrame(ref m_receBuff, 2);
                iNumberofFrames = CommonConstants.MAKEWORD(m_receBuff[0], m_receBuff[1]);
                if (iNumberofFrames == Total_Frames)
                    return true;
            }
            return false;
        }

        private void WriteFrame(int pSize, byte[] m_sendBuff)
        {
            unsafe
            {
                bool blFlag = false;
                fixed (byte* p = m_sendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, pSize);
                }
            }
        }

        private bool ReceiveFrame(ref byte[] m_receBuff, int pSize)
        {
            try
            {
                unsafe
                {
                    fixed (byte* p = m_receBuff)
                    {
                        _serialNoOfByteRead = obj.ReadUSB_Device(p, pSize);
                    }
                }
            }
            catch (System.TimeoutException timeOut)
            {
                m_receBuff[0] = 0;
                return false;
            }
            if (_serialNoOfByteRead > 0)
            {
                _serialReceivedByte = m_receBuff[0];
                return true;
            }
            else
            {
                m_receBuff[0] = 0;
            }
            return false;
        }

        //End
        #endregion
        
       
    }
}
