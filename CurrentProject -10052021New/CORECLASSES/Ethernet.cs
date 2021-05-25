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
// File Name	Ethernet.cs
// Author		Amit Deshmukh
//=====================================================================
*/

using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ClassList
{
    /// <summary>
    /// Ethernet class allows to access(read/write) Prizm unit from EtherNet port.
    /// DownLoad and UpLoad of Prizm project files can be done using the class.
    /// Call Connect() to connect to the ethernet port, then call SendFile() or ReadFile() 
    /// for Download or upload resp. and lastly call Close() to End Communication 
    /// </summary>

    public class Ethernet : Device
    {
        #region Constants
        const string OK = "OK";
        const string OK4 = "OK4.0";

        #region FP_Ethernet_Implementation-AMIT
        const string WAIT = "WAIT";
        const string LWAIT = "LWAIT";
        const string HWAIT = "HWAIT";
        const string COMPLETE = "Complete";
        public int _EraseData;
        const string NO = "NO";
        #endregion

        const int ACKFRAMESIZE = 5;
        const int STARTFRAMESIZE = 40;
        const int FRAMEDIFFERENCE = 100;
        const int DATAFRAMESIZE = 1500;
        const int PLC_INFO_FRAMESIZE = 65;
        const int TOLERANCE_BYTE_1 = 0x01;
        const int TOLERANCE_BYTE_2 = 0x02;
        const int TOLERANCE_BYTE_3 = 0x03;
        const int TOLERANCE_BYTE_4 = 0x04;

        const byte ETHER_APP_LADD4_DNLD = 6;
        const byte ETHER_LADD_FIRM1_DNLD = 5;
        const byte ETHER_LADDER_DNLD = 4;
        const byte ETHER_FONT_DNLD = 3;
        const byte ETHER_APPLICATION_DNLD = 2;
        const byte ETHER_FIRMWARE_LADD5_DNLD = 1;

        private const int SERIAL_LADDER_DNLD = 0x77;
        private const int SERIAL_FONT_DNLD = 0x88;
        private const int SERIAL_FIRMWARE_DNLD = 0x99;
        private const int SERIAL_APPLICATION_DNLD = 0xEE;
        private const int SERIAL_ETHER_SETTINGS_DNLD = 0x44;
        private const int SERIAL_LOGGEDDATA_UPLD = 0xAA;
        private const int SERIAL_FHWT_DNLD = 0x33;
        private const int SERIAL_HIST_ALARM_DATA_UPLD = 0x22;  //manik
        private byte _etherCRCBYTE = 0;

        protected const int ERR_RECONNECTUNIT = 0x0A;
        protected const int ERR_SEARCHINGUNIT = 0x01;

        #region FP_Ethernet_Implementation-AMIT
        private int _ethernetResponseTimeOut = 20000;
        private bool _ethernetEraseDataLoggerMemory = false;
        private bool _ethernetEraseHistoricalAlarm = false;
        private bool _productMisMatch = false; //Product Mismatch Error
        private int DataLogSize = 0; //FIFO Change
        #endregion
        #endregion

        #region Private Members
        private int _ethernetFrameNum = 0;
        private Socket _ethernetServerSock = null;
        private IPAddress _ethernetIpAdd = null;
        private byte[] _ethernetRecvBuff;
        private byte[] _ethernetSendBuff;
        private Thread _EthernetThread;
        private string m_pFileName = string.Empty;
        private byte m_pFileID = 0;
        private byte _unitHaltMode = 0;
        private byte _unitRunMode = 0;
        private bool _IsPLCModeSent;
        private int _productID = 0;//Amit(26-6-13)
        #endregion

        #region Public Properties

        #region FP_Ethernet_Implementation-AMIT

        private byte UnitRunMode
        {
            get
            {
                return _unitRunMode;
            }
            set
            {
                _unitRunMode = value;
            }
        }

        private byte UnitHaltMode
        {
            get
            {
                return _unitHaltMode;
            }
            set
            {
                _unitHaltMode = value;
            }
        }

        private bool IsPLCModeSent
        {
            get
            {
                return _IsPLCModeSent;
            }
            set
            {
                _IsPLCModeSent = value;
            }
        }

        public int ResponseTimeOut
        {
            get
            {
                return _ethernetResponseTimeOut;
            }
            set
            {
                _ethernetResponseTimeOut = value;
            }
        }

        public CommunicationOperationType OperationType
        {
            get { return _ethernetOperationType; }
            set { _ethernetOperationType = value; }
        }

        public int UpldProductID //Amit(26-6-13)
        {
            get { return _productID; }
            set { _productID = value; }
        }

        #endregion

        #endregion

        #region Public Members
        public int _serialFileID;
        #endregion

        #region Public Events
        public delegate void DownloadPercentage(float pPercentage);
        public event DownloadPercentage _deviceDownloadEthernetPercentage;

        public delegate void DownloadStatus(int pMessage);
        public event DownloadStatus _deviceDownloadEthernetStatus;

        private CommunicationOperationType _ethernetOperationType;

        #endregion

        #region Private Methods
        /// <summary>
        /// This function prepares the setup frame required for communication. 
        /// Setup frame may contain the information like file type, device type, file length etc.
        /// </summary>
        /// <param name="pType">indicates Uploading or Downloading</param>
        /// <returns>SUCCESS or FAILURE</returns>
        private int GetSetFrameIntoBuff(int pType)
        {
            int i = 0;
            uint tmp = 0;
            int oneByte = 8;
            int twoByte = 16;
            int threeByte = 24;
            uint oneByteBits = 255;
            int iTempDataFrameSize = 256;
            byte[] bytes = new byte[5];
            FileStream fs = null;//FlexiSoft_IEC_Mngr_1255_AD

            if (pType == DOWN)
            {
                try
                {
                    fs = new FileStream(_deviceFileName, FileMode.Open);//FlexiSoft_IEC_Mngr_1255_AD

                    if (_deviceFileName == CommonConstants.TEMP_DOWNLOAD_FILENAME)
                    {
                        fs.Read(bytes, 0, 4);
                        _deviceLength = (uint)bytes[0];
                        _deviceLength += (uint)bytes[1] << oneByte;
                        _deviceLength += (uint)bytes[2] << twoByte;
                        _deviceLength += (uint)bytes[3] << threeByte;
                    }
                    else if (_deviceFileID == CommonConstants.ETHER_ETHERNET_SETTINGS_DNLD_FILEID)
                    {
                        _deviceLength = 18;
                    }
                    else
                        _deviceLength = (uint)fs.Length;
                }
                catch (UnauthorizedAccessException UnException)//FlexiSoft_IEC_Mngr_1255_AD
                {                    
                    return FAILURE;
                }
                catch (IOException ioEX)//VOffice_IssueNo_304
                {
                    return FAILURE;
                }
                catch (Exception ex)//FlexiSoft_IEC_Mngr_1255_AD
                {
                    return FAILURE;
                }
                finally//FlexiSoft_IEC_Mngr_1255_AD
                {
                    if (fs != null)
                        fs.Close();
                }
                tmp = _deviceLength;

                _ethernetSendBuff.SetValue((byte)(_deviceProdID), i++);
                _ethernetSendBuff.SetValue((byte)(_deviceProdID / oneByteBits), i++);//Product ID

                _ethernetSendBuff.SetValue((byte)(pType), i++);
                _ethernetSendBuff.SetValue((byte)(pType / oneByteBits), i++);//Type Download Or UpLoad

                _ethernetSendBuff.SetValue((byte)tmp, i++); //File Size
                _ethernetSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), i++);//File Size
                _ethernetSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), i++);//File Size
                _ethernetSendBuff.SetValue((byte)(tmp /= (uint)iTempDataFrameSize), i++);//File Size

                _deviceTotalFrames = (uint)(_deviceLength / (DATAFRAMESIZE - FRAMEDIFFERENCE));//Total No of Frames
                _deviceTotalFrames = ((_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE)) > 0) ? _deviceTotalFrames + 1 : _deviceTotalFrames;
                tmp = _deviceTotalFrames;
                _ethernetSendBuff.SetValue((byte)tmp, i++);//Total No of Frames
                _ethernetSendBuff.SetValue((byte)(tmp = tmp >> oneByte), i++);//Total No of Frames
                _ethernetSendBuff.SetValue((byte)(tmp = tmp >> oneByte), i++);//Total No of Frames
                _ethernetSendBuff.SetValue((byte)(tmp = tmp >> oneByte), i++);//Total No of Frames

                if (IsPLCModeSent)
                    _ethernetSendBuff[i++] = (byte)CommonConstants.PLC_STATUS_FILEID;// PLC Mode ID
                else
                    _ethernetSendBuff[i++] = (byte)_deviceFileID;// File ID
                _ethernetSendBuff.SetValue(UnitHaltMode, i++);//Halt Mode Byte
                _ethernetSendBuff.SetValue(UnitRunMode, i++);//Run Mode Byte
                _ethernetSendBuff.SetValue((byte)_arrdwnlsetupframe[2], i++);//Keep Memory after Download Byte
                _ethernetSendBuff.SetValue((byte)_arrdwnlsetupframe[3], i++);//Initialize all device registers except keep memory area after download Byte

                for (int cnt = 0; cnt < 23; cnt++)
                    _ethernetSendBuff.SetValue((byte)(0), i++);

                _ethernetSendBuff[i++] = (byte)'\0';

                // Following Code To Be Removed
                string str = "";
                for (i = 0; i < STARTFRAMESIZE; i++)
                    str += _ethernetSendBuff[i] + " ";
                str += _deviceLength + " " + _deviceTotalFrames + " ";
            }
            else if (pType == UP) 	//	UpLoad
            {
                _ethernetSendBuff.SetValue((byte)(1), i++);
                _ethernetSendBuff.SetValue((byte)(0), i++);//Product ID

                _ethernetSendBuff.SetValue((byte)(UP), i++);
                _ethernetSendBuff.SetValue((byte)(0), i++);//Type Download Or UpLoad

                _ethernetSendBuff.SetValue((byte)0, i++);
                _ethernetSendBuff.SetValue((byte)(0), i++);
                _ethernetSendBuff.SetValue((byte)(0), i++);
                _ethernetSendBuff.SetValue((byte)(0), i++);//File Size

                _deviceTotalFrames = 0;

                _ethernetSendBuff.SetValue((byte)_deviceTotalFrames, i++);
                _ethernetSendBuff.SetValue((byte)(_deviceTotalFrames), i++);
                _ethernetSendBuff.SetValue((byte)(_deviceTotalFrames), i++);
                _ethernetSendBuff.SetValue((byte)(_deviceTotalFrames), i++);

                _ethernetSendBuff[i++] = (byte)_deviceFileID;// File ID
                _ethernetSendBuff[i++] = (byte)'\0';
            }
            return SUCCESS;
        }


        /// <summary>
        /// File is transmitted using fixed size frames 
        /// this function prepares the data frames for transmission.
        /// </summary>
        /// <param name="pType">indicates Uploading or Downloading</param>
        /// <param name="sr">SUCCESS or FAILURE</param>
        /// <returns></returns>
        private int GetDataFrameIntoBuff(int pType, FileStream sr)
        {
            if (pType == DOWN)
            {
                if (_ethernetFrameNum + 1 == _deviceTotalFrames)
                {
                    if (_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE) == 0)
                    {
                        _ethernetSendBuff = new byte[DATAFRAMESIZE];
                        sr.Read(_ethernetSendBuff, 0, (DATAFRAMESIZE - FRAMEDIFFERENCE));
                    }
                    else
                    {
                        // Need to send 41 byte frame as setup frame is of 40 bytes
                        if (_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE) == 40)
                        {
                            _ethernetSendBuff = new byte[_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE) + 1];
                            sr.Read(_ethernetSendBuff, 0, (int)_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE));
                            _ethernetSendBuff[_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE)] = Convert.ToByte(0);
                        }
                        else
                        {
                            _ethernetSendBuff = new byte[_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE)];
                            sr.Read(_ethernetSendBuff, 0, (int)_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE));
                        }
                    }
                }
                else
                {
                    // Need to send 41 byte frame as setup frame is of 40 bytes
                    //if (_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE) == 40)
                    //{
                    //    _ethernetSendBuff = new byte[_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE) + 1];
                    //    sr.Read(_ethernetSendBuff, 0, (int)_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE));
                    //    _ethernetSendBuff[_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE) + 1] = Convert.ToByte(0);
                    //}
                    //else
                    //{
                        if (_ethernetSendBuff.Length < DATAFRAMESIZE)
                            _ethernetSendBuff = new byte[DATAFRAMESIZE];
                        sr.Read(_ethernetSendBuff, 0, (DATAFRAMESIZE - FRAMEDIFFERENCE));
                    //}
                }
                _ethernetFrameNum++;
                return SUCCESS;
            }
            else
            {
                if (_ethernetFrameNum + 1 == _deviceTotalFrames)
                {
                    //_ethernetRecvBuff = new byte[_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE)];
                    if (_deviceFileID == CommonConstants.ETHER_APP_UPLD_FILEID)
                        sr.Write(_ethernetRecvBuff, 0, (int)_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE));
                    else
                        sr.Write(_ethernetRecvBuff, 0, (int)_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE));
                }
                else
                    sr.Write(_ethernetRecvBuff, 0, (DATAFRAMESIZE - FRAMEDIFFERENCE));
                _ethernetFrameNum++;
                return SUCCESS;
            }
        }


        /// <summary>
        /// When PC is downloading any file; data send to the unit is is in the form of frames and is copied into the buffer using  this function.
        /// In uploading it send ACK frames
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        private int SendFrame(int pType)
        {
            if (pType == DOWN)
            {
                if (_deviceFileID == CommonConstants.ETHER_ETHERNET_SETTINGS_DNLD_FILEID)
                {
                    if (_ethernetServerSock.Send(_ethernetSendBuff, 0, 20, 0) > 0)
                    {
                        return SUCCESS;
                    }
                    else
                    {
                        return FAILURE;
                    }
                }
                else
                {
                    if (_ethernetFrameNum == _deviceTotalFrames)// Occurs in case of Last frame
                    {
                        if (_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE) == 40)///In this if last frame is of 40 byte i.e Set up frame
                        {
                            if (_ethernetServerSock.Send(_ethernetSendBuff, 0, Convert.ToInt32(_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE) + 1), 0) > 0)
                            {
                                return SUCCESS;
                            }
                            else
                            {
                                return FAILURE;
                            }
                        }
                        else
                        {
                            if (_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE) == 0)
                            {
                                if (_ethernetServerSock.Send(_ethernetSendBuff, 0, (DATAFRAMESIZE - FRAMEDIFFERENCE), 0) > 0)///In this if last frame is of 1400 byte
                                {
                                    return SUCCESS;
                                }
                                else
                                {
                                    return FAILURE;
                                }
                            }
                            else
                            {
                                if (_ethernetServerSock.Send(_ethernetSendBuff, 0, (int)_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE), 0) > 0)
                                {
                                    return SUCCESS;
                                }
                                else
                                {
                                    return FAILURE;
                                }
                            }
                        }
                    }
                    else
                    {
                        //if (_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE) == 40)///In this if last frame is of 40 byte i.e Set up frame
                        //{
                        //    if (_ethernetServerSock.Send(_ethernetSendBuff, 0, (DATAFRAMESIZE - FRAMEDIFFERENCE) + 1, 0) > 0)
                        //    {
                        //        return SUCCESS;
                        //    }
                        //    else
                        //    {
                        //        return FAILURE;
                        //    }
                        //}
                        //else
                        //{
                            if (_ethernetServerSock.Send(_ethernetSendBuff, 0, (DATAFRAMESIZE - FRAMEDIFFERENCE), 0) > 0)
                            {
                                return SUCCESS;
                            }
                            else
                            {
                                return FAILURE;
                            }
                        //}
                    }
                }
            }
            else	// UpLoad
            {
                _ethernetSendBuff[0] = Convert.ToByte(OK[0]);
                _ethernetSendBuff[1] = Convert.ToByte(OK[1]);
                _ethernetSendBuff[2] = Convert.ToByte(0);
                _ethernetServerSock.Send(_ethernetSendBuff, 0, 3, 0);
                return SUCCESS;
            }
        }


        /// <summary>
        /// When PC is uploading any file; data received from the unit is is in the form of frames 
        /// and is copied into the file using this function.
        /// While downloading it recieves ACK frames.
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        private int ReceiveFrame(int pType)
        {
            System.Text.ASCIIEncoding a = new System.Text.ASCIIEncoding();
            try
            {
                if (pType == DOWN)
                {
                    string str = "";
                    int size = _ethernetServerSock.Receive(_ethernetRecvBuff, 0, (DATAFRAMESIZE - FRAMEDIFFERENCE), 0);

                    for (int i = 0; i < size - 1 && _ethernetRecvBuff[i] != 0; i++)
                        str += (char)_ethernetRecvBuff[i];
                    if (str == OK)
                    {
                        return SUCCESS;
                    }
                    else if (str == NO)
                    {
                        Close();
                        return SUCCESS;
                    }
                    else if (str == OK4)
                    {
                        _ethernetSendBuff[0] = Convert.ToByte('R');
                        _ethernetSendBuff[1] = Convert.ToByte('E');
                        _ethernetSendBuff[2] = Convert.ToByte('P');
                        _ethernetSendBuff[3] = Convert.ToByte('L');
                        _ethernetServerSock.Send(_ethernetSendBuff, 4, 0);
                        if (ReceiveFrame(DOWN) == FAILURE)
                            return FAILURE;
                        else
                            return SUCCESS;
                    }
                    else if (str.Contains("ERROR")) //product mismatch
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadEthernetStatus(5);
                        _productMisMatch = true;//Product Mismatch Error AMIT
                        return FAILURE;
                    }
                    return FAILURE;
                }
                else if (pType == UP)
                {
                    string str = "";
                    SocketError se = SocketError.Success;

                    bool flag = _ethernetServerSock.IsBound;

                    int size = _ethernetServerSock.Receive(_ethernetRecvBuff, 0, (DATAFRAMESIZE - FRAMEDIFFERENCE + 1), 0);

                    if (size < DATAFRAMESIZE - FRAMEDIFFERENCE + 1 && size != 40)
                        if (_ethernetFrameNum + 1 != _deviceTotalFrames)
                        {
                            size = _ethernetServerSock.Receive(_ethernetRecvBuff, 0, (DATAFRAMESIZE - FRAMEDIFFERENCE + 1), 0);
                        }
                        else
                        {
                            if (_deviceLength > _ethernetFrameNum * 1400 + size)
                            {
                                size = _ethernetServerSock.Receive(_ethernetRecvBuff, 0, (DATAFRAMESIZE - FRAMEDIFFERENCE + 1), 0);
                            }

                            if (!ClassList.CommonConstants.IsProductPLC(UpldProductID))//Amit(26-6-13)
                            {
                                if (_deviceFileID == CommonConstants.ETHER_APP_UPLD_FILEID ||
                                _deviceFileID == CommonConstants.ETHER_APPLICATION_DNLD_FILEID)
                                {
                                    byte[] _temp = new byte[_ethernetRecvBuff.Length];
                                    Array.Copy(_ethernetRecvBuff, 0, _temp, 0, size);//Changed as per discussion with Alankar-AbhijitT regarding Issue/2016-17/1919 -KV
                                    Array.Copy(_temp, 0, _ethernetRecvBuff, 0, size);
                                }
                            }
                        }
                    return SUCCESS;
                    //}
                }
            }
            catch (Exception e)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));

                //_ethernetServerSock = null;
                _deviceLastError = e.Message;
                return FAILURE;
            }
            return FAILURE;
        }

        /// <summary>       
        /// This function prepares ethernet settings frame required for communication. 
        /// Setup frame contains the information like Ip address, Subnet mask, Default gateway, Port next, DHCP and CRC.
        /// </summary>
        private void GetEthernetSettingsFrame()
        {
            uint tmp;
            int index = 0;
            int oneByte = 8;  // In bits
            int twoBytes = 16; // In Bits
            int threeBytes = 24; // In Bits
            int sizeOfLength = 4;  // Size of _deviceLength vari. is 4 Bytes
            byte[] bytes = new byte[sizeOfLength];
            string[] tempStrArr;

            tempStrArr = Device.EthernetSettings._IPAdderess.Split('.');  //Ip adderess next 4 bytes

            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[0]);
            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[1]);
            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[2]);
            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[3]);

            tempStrArr = Device.EthernetSettings._SubnetMask.Split('.'); //Subnet mask
            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[0]);
            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[1]);
            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[2]);
            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[3]);

            tempStrArr = Device.EthernetSettings._DefaultGateway.Split('.'); //Default gateway
            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[0]);
            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[1]);
            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[2]);
            _ethernetSendBuff[index++] = Convert.ToByte(tempStrArr[3]);

            int iPORT = Convert.ToInt32(Device.EthernetSettings._DownloadPort);
            _ethernetSendBuff[index++] = Convert.ToByte(iPORT & 255);   //Port next 4 bytes
            _ethernetSendBuff[index++] = Convert.ToByte((iPORT >> 8) & 255);
            _ethernetSendBuff[index++] = 0;
            _ethernetSendBuff[index++] = 0;

            _ethernetSendBuff[index++] = Device.EthernetSettings._DHCP ? (byte)0xFF : (byte)0x00; //DHCP flag
            _ethernetSendBuff[index++] = (byte)0xFF;

            _ethernetSendBuff[index++] = 0; //CRC next 2 bytes
            _ethernetSendBuff[index++] = _etherCRCBYTE = CalculateCrc(0, 18);
        }

        ///<summary>
        /// This function calculates CRC byte to be send in EthernetSetting Frame
        ///</summary>
        ///<param name="piStartIndex">It is 0 i.e start index of EthernetSetting Frame</param>
        ///<param name="piLength">Last byte of Frame</param>
        ///<returns name="piLength">returns CRCbyte calculated</returns>

        private byte CalculateCrc(int piStartIndex, int piLength)
        {
            byte CRCbyte = _ethernetSendBuff[piStartIndex];
            for (int i = 1; i < piLength; i++)
                CRCbyte ^= _ethernetSendBuff[i + piStartIndex];
            return CRCbyte;
        }

        #endregion

        #region Public Methods
        public Ethernet(short pProductID)
        {
            _deviceProdID = pProductID;
            _ethernetRecvBuff = new byte[DATAFRAMESIZE];
            _ethernetSendBuff = new byte[DATAFRAMESIZE];
            _ethernetOperationType = CommunicationOperationType.NONE;
            _EthernetThread = new Thread(new ThreadStart(DownLoadFunc));
        }

        public Ethernet(short pProductID, byte[] _setupFrameMode)
        {
            _deviceProdID = pProductID;
            _ethernetRecvBuff = new byte[DATAFRAMESIZE];
            _ethernetSendBuff = new byte[DATAFRAMESIZE];
            _ethernetOperationType = CommunicationOperationType.NONE;
            _EthernetThread = new Thread(new ThreadStart(DownLoadFunc));

            _arrdwnlsetupframe = new byte[10];
            _arrdwnlsetupframe = _setupFrameMode;
        }

        public void SetIpAndPort(string pIPAdd, int pPort)
        {
            _ethernetIpAdd = IPAddress.Parse(pIPAdd);
            _devicePortNo = pPort;
        }

        /// <summary>
        /// This function returns proper file id to communicate with the FlexiSoft
        /// </summary>
        /// <param name="pFileID"></param>
        public string GetFileID(int pFileID)
        {
            string tempStr = null;
            _deviceFileID = 0;

            if ((pFileID & (byte)DownloadData.Firmware) > 0)
            {
                //FP_CODE  R12  Haresh
                _deviceFileID = CommonConstants.ETHER_FIRMWARE_LADD_DNLD_FILEID;
                if (CommonConstants.IsProductFlexiPanels(ClassList.CommonConstants.ProductDataInfo.iProductID))
                    tempStr = "HIO\\Object\\" + CommonConstants.GetFolderName(ClassList.CommonConstants.ProductDataInfo.iProductID) + "\\" + CommonConstants.DOWNLOAD_FIRMWARE_FILENAME;
                else
                    tempStr = "HIO\\Object\\" + CommonConstants.GetProductName(_deviceProdID) + "\\" + CommonConstants.DOWNLOAD_FIRMWARE_FILENAME;
                //End
                _serialFileID ^= 2;
            }
            else if ((pFileID & (byte)DownloadData.Application) > 0)
            {
                if ((pFileID & (byte)DownloadData.Ladder) > 0)
                    _deviceFileID = CommonConstants.ETHER_APP_LADD_DNLD_FILEID;
                else
                    _deviceFileID = CommonConstants.ETHER_APPLICATION_DNLD_FILEID;

                tempStr = CommonConstants.TEMP_DOWNLOAD_FILENAME;
                _serialFileID ^= 1;
            }
            else if ((pFileID & (byte)DownloadData.Ladder) > 0)
            {
                _deviceFileID = CommonConstants.ETHER_LADDER_DNLD_FILEID;
                tempStr = CommonConstants.DOWNLOAD_LADDER_FILENAME;
                _serialFileID ^= 4;
            }
            else if ((pFileID & (byte)DownloadData.Font) > 0)
            {
                _deviceFileID = CommonConstants.ETHER_FONT_DNLD_FILEID;
                tempStr = CommonConstants.DOWNLOAD_FONT_FILENAME;
                _serialFileID ^= 8;
            }
            else if ((pFileID & (byte)DownloadData.EtherSettings) > 0)
            {
                _deviceFileID = CommonConstants.ETHER_ETHERNET_SETTINGS_DNLD_FILEID;
                _serialFileID ^= 64;
                StreamWriter sw = new StreamWriter("EthernetSettings.txt");
                sw.WriteLine();
                sw.Close();
                tempStr = "EthernetSettings.txt";
            }
            else if ((pFileID & (byte)DownloadData.FHWT) > 0)
            {
                _deviceFileID = SERIAL_FHWT_DNLD;
                _serialFileID ^= 128;
                //_deviceProdID = SERIAL_FHWT_DNLD;
                tempStr = CommonConstants.DOWNLOAD_FHWT_FILENAME;
            }
            else if ((pFileID & (byte)DownloadData.LoggedData) > 0)
            {
                _deviceFileID = SERIAL_FHWT_DNLD;
                //_serialFileID ^= 256;
                //_deviceProdID = SERIAL_FHWT_DNLD;
                tempStr = CommonConstants.DOWNLOAD_FHWT_FILENAME;
            }

            return tempStr;
        }


        /// <summary>
        /// This function does the initialization of selected  port and sends the request to the unit.
        /// On success it returns zero and failure on error. 
        /// </summary>
        /// <returns>SUCESS or FAILURE</returns>
        public override int Connect()
        {
            _deviceDownloadEthernetStatus(0);

            IPEndPoint ipe = new IPEndPoint(_ethernetIpAdd, _devicePortNo);
            _ethernetFrameNum = 0;
            try
            {
                _ethernetServerSock = new Socket(_ethernetIpAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _ethernetServerSock.ReceiveTimeout = _ethernetResponseTimeOut * 1000;
                _ethernetServerSock.SendTimeout = _ethernetResponseTimeOut * 1000;
            }
            catch (Exception e1)
            {
                _ethernetServerSock = new Socket(_ethernetIpAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                _ethernetServerSock.ReceiveTimeout = _ethernetResponseTimeOut * 1000;
                _ethernetServerSock.SendTimeout = _ethernetResponseTimeOut * 1000;

                if (_ethernetIpAdd == null || _devicePortNo == 0)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    return FAILURE;
                }
                try
                {
                    _ethernetServerSock.Connect(ipe);
                }
                catch (Exception e)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    ipe = null;
                    _ethernetServerSock = null;
                    _deviceLastError = e.Message;
                    return FAILURE;
                }
                _deviceIsConnected = true;

                return SUCCESS;
            }

            if (_ethernetIpAdd == null || _devicePortNo == 0)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                return FAILURE;
            }
            try
            {
                _ethernetServerSock.Connect(ipe);
            }
            catch (Exception e)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                ipe = null;
                _ethernetServerSock = null;
                _deviceLastError = e.Message;
                return FAILURE;
            }
            _deviceIsConnected = true;

            return SUCCESS;
        }


        /// <summary>
        ///This function sends the given file to the unit.
        ///File is sent to the unit using the predefined protocol between IBM PC and unit.
        ///On success it returns zero and on failure error number specifying the type of error.
        /// </summary>
        /// <param name="pFileName">File to send</param>
        /// <param name="pFileID">the type of file to send</param>
        /// <returns>SUCCESS or FAILURE</returns>
        public override int SendFile(string pFileName, byte pFileID)
        {
            _serialFileID = pFileID;
            pFileName = GetFileID(_serialFileID);
            _deviceFileName = pFileName;
            _EthernetThread.Priority = ThreadPriority.Highest;
            Thread.Sleep(100);
            _EthernetThread.Start();
            return SUCCESS;
        }

        /// <summary>
        /// This function does the reinitialize of selected  port and sends the request to the unit.
        /// On success it returns zero and failure on error. 
        /// </summary>
        /// <returns>SUCESS or FAILURE</returns>
        public int Reconnect()
        {
            _deviceDownloadEthernetStatus(0);

            IPEndPoint ipe = new IPEndPoint(_ethernetIpAdd, _devicePortNo);
            _ethernetFrameNum = 0;
            try
            {
                _ethernetServerSock = new Socket(_ethernetIpAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                _ethernetServerSock.ReceiveTimeout = _ethernetResponseTimeOut * 1000;

                _ethernetServerSock.SendTimeout = _ethernetResponseTimeOut * 1000;
            }
            catch (Exception e1)
            {
                _ethernetServerSock = new Socket(_ethernetIpAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                _ethernetServerSock.ReceiveTimeout = _ethernetResponseTimeOut * 1000;

                _ethernetServerSock.SendTimeout = _ethernetResponseTimeOut * 1000;

                if (_ethernetIpAdd == null || _devicePortNo == 0)
                {
                    return FAILURE;
                }

                try
                {
                    _ethernetServerSock.Connect(ipe);
                }
                catch (Exception e)
                {
                    ipe = null;
                    _ethernetServerSock = null;
                    _deviceLastError = e.Message;
                    return FAILURE;
                }
                _deviceIsConnected = true;

                return SUCCESS;
            }

            if (_ethernetIpAdd == null || _devicePortNo == 0)
            {
                return FAILURE;
            }
            try
            {
                _ethernetServerSock.Connect(ipe);
            }
            catch (Exception e)
            {
                ipe = null;
                _ethernetServerSock = null;
                _deviceLastError = e.Message;
                return FAILURE;
            }
            _deviceIsConnected = true;

            return SUCCESS;
        }

        /// <summary>
        /// This is a thread routine called at the time of downloading through Ethernet,
        /// which helps not to overload on the main thread, which may be busy for GUI routines or other.
        /// </summary>
        public void DownLoadFunc()
        {
            FileStream sr = null;
            bool error = false;
            int returnVal = SUCCESS;
            byte[] _bytReset = new byte[1];
            _bytReset[0] = 0x55;
            int iTry;
            bool ModeMismatch = false;
            _deviceFileInUse = false;//FlexiSoft_IEC_Mngr_1255_AD

            try
            {
                for (int itemp = 0; true; itemp++)
                {
                    IsPLCModeSent = true;
                    _ethernetFrameNum = 0;
                    CommonConstants.communicationStatus = 1;
                    CommonConstants.communicationType = 1;
                    int message = 256;
                    if (_deviceFileID == CommonConstants.ETHER_LADDER_DNLD_FILEID)
                        message <<= 2;
                    if (_deviceFileID == CommonConstants.SERIAL_FONT_DNLD_FILEID)
                        message <<= 3;
                    if (_deviceFileID == CommonConstants.SERIAL_FIRMWARE_DNLD_FILEID)
                        message <<= 1;
                    if (_deviceFileID == CommonConstants.ETHER_FIRMWARE_LADD_DNLD_FILEID)
                        message <<= 1;
                    if (_deviceFileID == CommonConstants.ETHER_FONT_DNLD_FILEID)
                        message <<= 3;
                    if (_deviceFileID == CommonConstants.ETHER_ETHERNET_SETTINGS_DNLD_FILEID)
                        message <<= 7;
                    _deviceDownloadEthernetStatus(3 + message);

                    _ethernetSendBuff = new byte[DATAFRAMESIZE];
                    _ethernetRecvBuff = new byte[DATAFRAMESIZE];

                    if (GetSetFrameIntoBuff(DOWN) == CommonConstants.FAILURE)//FlexiSoft_IEC_Mngr_1255_AD
                    {
                        error = true;
                        returnVal = FAILURE;
                        _deviceFileInUse = true;
                        break;
                    }

                    IsPLCModeSent = false;

                    _ethernetServerSock.Send(_ethernetSendBuff, STARTFRAMESIZE, 0);

                    if (ReceivePLCInfo() == SUCCESS)
                    {
                        if (_ethernetRecvBuff[0] == CommonConstants.PLC_STATUS_FILEID + TOLERANCE_BYTE_3)
                        {
                            if (_arrdwnlsetupframe[0] == 1)
                                UnitHaltMode = 1;
                            else
                            {
                                ModeMismatch = true;
                                UnitHaltMode = 0;
                                error = true;
                                returnVal = FAILURE;
                            }
                        }
                        else
                            UnitHaltMode = 0;

                        if (_ethernetRecvBuff[0] == CommonConstants.PLC_STATUS_FILEID + TOLERANCE_BYTE_4)
                        {
                            _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorModeMismatch));
                            error = true;
                            returnVal = FAILURE;
                        }
                    }
                    else
                    {
                        error = true;
                        returnVal = FAILURE;
                    }

                    if (_serialFileID == 0 && _arrdwnlsetupframe[1] == 1)
                        UnitRunMode = 1;
                    else
                        UnitRunMode = 0;

                    GetSetFrameIntoBuff(DOWN);

                    try
                    {
                        sr = new FileStream(_deviceFileName, FileMode.Open);

                        if (error)
                            break;

                        _ethernetServerSock.Send(_ethernetSendBuff, STARTFRAMESIZE, 0);

                    ConnectAgain:
                        if (_ethernetServerSock.Connected == false)
                        {
                            sr.Close();

                            Thread.Sleep(500);

                            if (ConnectSocket() == FAILURE)
                            {
                                returnVal = FAILURE;
                                break;
                            }

                            _ethernetSendBuff = new byte[DATAFRAMESIZE];
                            _ethernetRecvBuff = new byte[DATAFRAMESIZE];

                            GetSetFrameIntoBuff(DOWN);

                            sr = new FileStream(_deviceFileName, FileMode.Open);

                            _ethernetServerSock.Send(_ethernetSendBuff, STARTFRAMESIZE, 0);
                        }

                        if (!File.Exists(_deviceFileName))
                        {
                            error = true;
                            returnVal = FAILURE;// fileNotFound;
                            break;
                        }

                        for (int i = 1; i <= _deviceTotalFrames /*&& !error*/; i++)
                        {
                            if (_deviceFileID != CommonConstants.ETHER_ETHERNET_SETTINGS_DNLD_FILEID)
                                if (_deviceDownloadEthernetPercentage != null && CommonConstants.communicationStatus != -1 && CommonConstants.communicationStatus != -2)
                                    _deviceDownloadEthernetPercentage(100 * i / Convert.ToInt32(_deviceTotalFrames));

                            if (ReceiveFrame(DOWN) == FAILURE)
                            {
                                error = true;
                                returnVal = FAILURE; // Data Receiving;
                                break;
                            }
                            else
                            {
                                if (_ethernetServerSock.Connected == false)
                                    goto ConnectAgain; ////This is called if Unit is in Firmware Mode

                                if (_deviceFileID == CommonConstants.ETHER_ETHERNET_SETTINGS_DNLD_FILEID)
                                {
                                    GetEthernetSettingsFrame();
                                    _deviceDownloadEthernetPercentage(100);
                                }
                                else
                                    GetDataFrameIntoBuff(DOWN, sr);

                                error = SendFrame(DOWN) != SUCCESS ? true : false;

                            }
                            if (CommonConstants.communicationStatus < 0)
                            {
                                error = true;
                                break;
                            }
                        }

                        if (!error)
                            if (ReceiveFrame(DOWN) == FAILURE)
                            {
                                error = true;
                                returnVal = FAILURE; // Data Receiving;
                                break;
                            }

                        _deviceFileName = GetFileID(_serialFileID);
                        if (_deviceFileName == null || _deviceFileName == "")
                            break;
                    }
                    catch (UnauthorizedAccessException UnException)//FlexiSoft_IEC_Mngr_1255_AD
                    {                        
                        error = true;
                        returnVal = FAILURE;
                        _deviceFileInUse = true;
                        break;
                    }
                    catch (IOException ioEX)//VOffice_IssueNo_304
                    {
                        error = true;
                        returnVal = FAILURE;
                        _deviceFileInUse = true;
                        break;
                    }
                    catch (Exception e)//FlexiSoft_IEC_Mngr_1255_AD
                    {
                        error = true;
                        returnVal = FAILURE;
                        break;
                    }
                    finally//FlexiSoft_IEC_Mngr_1255_AD
                    {
                        if (sr != null)
                            sr.Close();
                    }
                    
                    if (error == true)
                    {
                        break;
                    }

                }
                Close();
                if (returnVal == FAILURE)
                {
                    if (_productMisMatch)
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strProductMismatch));
                    }
                    else if (ModeMismatch)
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strModeMismatch));
                    }
                    else if (_deviceFileInUse)//FlexiSoft_IEC_Mngr_1255_AD
                    {
                        CommonConstants.communicationStatus = 0;
                        if (_deviceFileID == CommonConstants.ETHER_APP_LADD_DNLD_FILEID)
                            _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorApplicationFileCreation));
                        else if (_deviceFileID == CommonConstants.ETHER_FIRMWARE_LADD_DNLD_FILEID)
                            _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorFirmwareFileCreation));                        
                    }
                    else
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    }

                    if (_ethernetServerSock != null)
                        if (_ethernetServerSock.Connected)
                            _ethernetServerSock.Close();
                }
                else if (CommonConstants.communicationStatus == -1 || CommonConstants.communicationStatus == -2)
                {
                    _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationAborted));
                    _ethernetServerSock.Close();
                }
                else
                {
                    //Issue 636 SP 15.02.13
                    if (ClassList.CommonConstants.g_Support_IEC_Ladder && ClassList.CommonConstants.g_DownloadForOnLine == true)
                    {
                        _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strWaitDeviceIsInitializing));
                        if (ClassList.CommonConstants.IsProductSupportsEthernet(ClassList.CommonConstants.ProductDataInfo.iProductID))
                            System.Threading.Thread.Sleep(16000);
                        else
                            System.Threading.Thread.Sleep(10000);
                    }
                    //End
                    //Straton_change Haresh
                    CommonConstants.downloadSucess = true;
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strDownloadComplete));
                    ClassList.CommonConstants.g_LadderModified = false; 
                    //Straton_change Haresh
                    //CommonConstants.downloadSucess = true;

                }
                if (sr != null)
                {
                    sr.Close();
                    sr = null;
                }
                Close();
                return;
            }
            catch (FileNotFoundException e)
            {
                if (_deviceFileID == CommonConstants.ETHER_APP_LADD_DNLD_FILEID)
                    _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorApplicationFileCreation));
                else if (_deviceFileID == CommonConstants.ETHER_FIRMWARE_LADD_DNLD_FILEID)
                    _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorFirmwareFileCreation));

                Close();
                return;
            }
            catch (SystemException ee)
            {
                if (_deviceFileID == CommonConstants.ETHER_APP_LADD_DNLD_FILEID)
                    _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorApplicationFileCreation));
                else if (_deviceFileID == CommonConstants.ETHER_FIRMWARE_LADD_DNLD_FILEID)
                    _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strErrorFirmwareFileCreation));

                Close();
                return;
            }
            catch (Exception e)
            {
                ClassList.ExceptionLogger.Operationslog(e, CoreConstStrings.ExCommunicationError, CoreConstStrings.ExGlobalErrorHdr);
            }
        }

        private int ReceivePLCInfo()
        {
            int size = 0;
            string str = "";
            try
            {
                //Receive 65 Byte Frame
                //(1 Byte PLC Mode + 64 Byte Device Information
                size = _ethernetServerSock.Receive(_ethernetRecvBuff, 0, (DATAFRAMESIZE - FRAMEDIFFERENCE), 0);
                for (int i = 0; i < size - 1 && _ethernetRecvBuff[i] != 0; i++)
                    str += (char)_ethernetRecvBuff[i];

                if (size == 65)
                {
                    uint tempID = CommonConstants.MAKEUWORD(_ethernetRecvBuff[2], _ethernetRecvBuff[3]);
                    if (tempID == _deviceProdID)
                    {
                        return SUCCESS;
                    }
                    else
                    {
                        CommonConstants.communicationStatus = 0;
                        _deviceDownloadEthernetStatus(5);
                        _productMisMatch = true;//Product Mismatch Error AMIT
                        return FAILURE;
                    }
                }
                else if (str.Contains("ERROR")) //product mismatch
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadEthernetStatus(5);
                    _productMisMatch = true;//Product Mismatch Error AMIT
                    return FAILURE;
                }
            }
            catch (Exception e)
            {
                CommonConstants.communicationStatus = 0;
                _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                _deviceLastError = e.Message;
                return FAILURE;
            }
            return FAILURE;
        }

        private int ConnectSocket()
        {
            IPEndPoint ipe = new IPEndPoint(_ethernetIpAdd, _devicePortNo);

            try
            {
                _ethernetServerSock = new Socket(_ethernetIpAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _ethernetServerSock.ReceiveTimeout = _ethernetResponseTimeOut * 1000;
                _ethernetServerSock.SendTimeout = _ethernetResponseTimeOut * 1000;
            }
            catch (Exception e)
            {
                _ethernetServerSock = new Socket(_ethernetIpAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                if (_ethernetIpAdd == null || _devicePortNo == 0)
                {
                    CommonConstants.communicationStatus = 0;
                    _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    return FAILURE;
                }
            }

            try
            {
                _ethernetServerSock.Connect(ipe);
            }
            catch (Exception e)
            {
                int returnVal;
                CommonConstants.communicationStatus = 0;

                ipe = null;
                _ethernetServerSock = null;
                _deviceLastError = e.Message;
                returnVal = FAILURE;
                return returnVal;
            }

            return SUCCESS;
        }



        /// <summary>
        /// This function receives the file from the unit.
        /// File is received from the unit using the predefined protocol between IBM PC and unit
        /// On success it returns zero and on failure error number specifying the type of error.
        /// </summary>
        /// <param name="pFileName">Name of the file to save received</param>
        /// <returns>SUCCESS or FAILURE</returns>
        public override int ReceiveFile(string pFileName, int pFileID)
        {
            CommonConstants.LADDER_PRESENT = false;
            _serialFileID = pFileID;
            pFileName = GetUploadFileID(_serialFileID, pFileName);
            _deviceFileName = pFileName;
            _EthernetThread = new Thread(new ThreadStart(UpLoadFunc));
            _EthernetThread.Priority = ThreadPriority.Highest;
            Thread.Sleep(100);
            if (_deviceFileID != SERIAL_ETHER_SETTINGS_DNLD)
            {
                _EthernetThread.Start();
                return SUCCESS;
            }
            else
                return SUCCESS;

            return FAILURE;
        }

        public void UpLoadFunc()
        {
            int iTry;
            FileStream sr;
            CommonConstants.communicationStatus = 2;
            CommonConstants.communicationType = 2;
            bool error = false;
            int returnVal = SUCCESS;
            _ethernetSendBuff = new byte[DATAFRAMESIZE];

            for (int itemp = 0; true; itemp++)
            {
                if (!File.Exists(_deviceFileName))
                {
                    error = true;
                    returnVal = FAILURE;// fileNotFound;
                }

                int message = 256;
                if (_deviceFileID == CommonConstants.ETHER_LOGG_UPLD_FILEID ||
                    _deviceFileID == CommonConstants.ETHER_LOGG_UPLD_FLASH_FILEID)
                    message <<= 4;
                else if (_deviceFileID == CommonConstants.ETHER_HISTORICAL_ALARM_UPLD_FILEID)
                    message <<= 5;
                else if (_deviceFileID == CommonConstants.ETHER_LADDER_DNLD_FILEID)
                    message <<= 2;
                #region Ethernet Setting Upload AMIT
                else if (_deviceFileID == CommonConstants.ETHER_ETHERNET_SETTINGS_DNLD_FILEID)
                    message <<= 7;
                #endregion Ethernet Setting Upload AMIT

                _deviceDownloadEthernetStatus(2 + message);

                _ethernetSendBuff = new byte[DATAFRAMESIZE];
                _ethernetRecvBuff = new byte[DATAFRAMESIZE];

                Thread.Sleep(2000);
                GetSetFrameIntoBuff(UP);

                sr = new FileStream(_deviceFileName, FileMode.Create);

                try
                {
                    _ethernetServerSock.Send(_ethernetSendBuff, STARTFRAMESIZE, 0);
                }
                catch (Exception e)
                {
                    CommonConstants.communicationType = 0;
                    CommonConstants.communicationStatus = -1;
                    _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    _deviceLastError = e.Message;
                    returnVal = FAILURE;
                    error = true;
                    break;
                }

                if (ReceiveFrame(DOWN) == SUCCESS)
                    SendFrame(UP);
                else
                {
                    error = true;
                    returnVal = FAILURE;
                    break;
                }

                if (ReceiveFrame(UP) == SUCCESS)
                    ReadSetupFrame();
                else
                {
                    error = true;
                    returnVal = FAILURE;
                    break;
                }

                _ethernetSendBuff[0] = Convert.ToByte(OK[0]);
                _ethernetSendBuff[1] = Convert.ToByte(OK[1]);
                _ethernetSendBuff[2] = Convert.ToByte(0);
                try
                {
                    _ethernetServerSock.Send(_ethernetSendBuff, 3, 0);
                }
                catch (Exception e)
                {
                    CommonConstants.communicationType = 0;
                    CommonConstants.communicationStatus = -1;
                    _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));
                    _deviceLastError = e.Message;
                    returnVal = FAILURE;
                    error = true;
                    break;
                }
                _ethernetSendBuff = new byte[DATAFRAMESIZE];
                _ethernetFrameNum = 0;

                for (int i = 1; i <= _deviceTotalFrames && returnVal != FAILURE && !error; i++)
                {
                    if (_deviceDownloadEthernetPercentage != null && CommonConstants.communicationStatus != -1 && CommonConstants.communicationStatus != -2)
                        _deviceDownloadEthernetPercentage(100 * i / Convert.ToInt32(_deviceTotalFrames));

                    if (ReceiveFrame(UP) == FAILURE)
                    {
                        error = true;
                        returnVal = FAILURE; // Data Receiving;
                        break;
                    }
                    else
                    {
                        #region PR1004
                        if (_deviceFileID == CommonConstants.ETHER_APP_LADD_DNLD_FILEID ||
                            _deviceFileID == CommonConstants.ETHER_APPLICATION_DNLD_FILEID && i == 1)
                        {
                            _productID = ClassList.CommonConstants.MAKEWORD(_ethernetRecvBuff[4], _ethernetRecvBuff[5]);//Amit(26-6-13)
                            _serialFileID |= (int)DownloadData.EtherSettings;
                        }
                        #endregion PR1004
                        if (_deviceFileID == CommonConstants.ETHER_LADDER_DNLD_FILEID)
                        {
                            CommonConstants.LADDER_PRESENT = true;
                            _serialFileID |= (int)DownloadData.Application;
                        }
                        error = GetDataFrameIntoBuff(UP, sr) == SUCCESS;
                        error = SendFrame(UP) != SUCCESS && error ? true : false;
                    }
                    if (CommonConstants.communicationStatus < 0)
                    {
                        error = true;
                        break;
                    }
                }

                _deviceFileName = GetUploadFileID(_serialFileID, _deviceFileName);
                if (_deviceFileName == "" || _deviceFileName == null || error)
                {
                    sr.Close();
                    break;
                }

                sr.Close();
            }

            # region Issue_1267
            if (sr != null)
                sr.Close();
            sr = null;
            Close();
            #endregion

            if (returnVal == FAILURE)
            {
                CommonConstants.communicationType = 0;
                CommonConstants.communicationStatus = -1;
                _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strPrizmNotResponding));

            }
            else if (CommonConstants.communicationStatus == -1 || CommonConstants.communicationStatus == -2)
            {
                //sr.Close();
                //Close();
                //CommonConstants.communicationStatus = 0;
                _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strCommunicationAborted));

            }
            else
            {
                _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strUploadComplete)); //Issue_1267
                CommonConstants.communicationStatus = 0;
                CommonConstants.communicationType = 2;
                _deviceDownloadEthernetStatus(Convert.ToInt32(DownloadingStatusMessages.strUploadComplete));
            }


            return;
        }

        protected string GetUploadFileID(int pFileID, string pFilename)
        {
            string tempStr = null;
            _deviceFileID = 0;

            #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
            if ((pFileID & (byte)DownloadData.Ladder) > 0)
            {
                _deviceFileID = CommonConstants.ETHER_LADDER_DNLD_FILEID;
                tempStr = CommonConstants.UPLOAD_LADDER_FILENAME;
                _deviceFileName = tempStr;
                _serialFileID ^= 4;
            }
            #endregion
            else if ((pFileID & (int)DownloadData.Application) > 0)
            {
                _deviceFileID = CommonConstants.ETHER_APPLICATION_DNLD_FILEID;
                tempStr = CommonConstants.TEMP_DOWNLOAD_FILENAME;
                _serialFileID ^= 1;
                if (_serialFileID > 0)
                    _deviceFileID = CommonConstants.ETHER_APP_UPLD_FILEID;
            }
            #region PR1145 AMIT
            else if ((pFileID & (int)DownloadData.HistAlarmData) > 0)
            {
                _deviceFileID = CommonConstants.ETHER_HISTORICAL_ALARM_UPLD_FILEID;
                _serialFileID ^= 256;
                //_deviceProdID = SERIAL_LOGGEDDATA_UPLD;
                tempStr = CommonConstants.HistAlarmBinaryFile;
            }
            #endregion PR1145 AMIT
            else if ((pFileID & (int)DownloadData.LoggedData) > 0)
            {
                _deviceFileID = CommonConstants.ETHER_LOGG_UPLD_FLASH_FILEID;
                _serialFileID ^= 16;
                //_deviceProdID = SERIAL_LOGGEDDATA_UPLD;
                tempStr = CommonConstants.BinaryFile;
            }
            #region Ethernet Setting Upload AMIT
            else if ((pFileID & (byte)DownloadData.EtherSettings) > 0)
            {
                _deviceFileID = CommonConstants.ETHER_ETHERNET_SETTINGS_DNLD_FILEID;
                _serialFileID ^= 64;
                tempStr = CommonConstants.Ethernet_UploadEthernetSettingBinaryFile;
            }
            #endregion Ethernet Setting Upload AMIT
            //Ladder_Change_R10
            //else if (pFileID == CommonConstants.LADDER_UPLD_FILEID)
            #region Upload_Support Appln + Ladder + Hist Alarm + Logged Data --AMIT
            //else if ((pFileID & (byte)DownloadData.Ladder) > 0)
            //{
            //    _deviceFileID = CommonConstants.ETHER_LADDER_DNLD_FILEID;                
            //    tempStr = CommonConstants.UPLOAD_LADDER_FILENAME;
            //    _deviceFileName = tempStr;
            //    _serialFileID ^= 4;
            //}
            #endregion

            //End//            
            return tempStr;
        }

        private void ReadSetupFrame()
        {
            _deviceProdID = _ethernetRecvBuff[0];
            _deviceProdID += Convert.ToInt16(_ethernetRecvBuff[1] << 8);

            //_ethernetSendBuff.SetValue((byte)(pType), i++);
            //_ethernetSendBuff.SetValue((byte)(pType / oneByteBits), i++);//Type Download Or UpLoad

            _deviceLength = _ethernetRecvBuff[4];
            _deviceLength += Convert.ToUInt32(_ethernetRecvBuff[5] << 8);
            _deviceLength += Convert.ToUInt32(_ethernetRecvBuff[6] << 16);
            _deviceLength += Convert.ToUInt32(_ethernetRecvBuff[7] << 32);

            _deviceTotalFrames = (uint)(_deviceLength / (DATAFRAMESIZE - FRAMEDIFFERENCE));//Total No of Frames
            _deviceTotalFrames = ((_deviceLength % (DATAFRAMESIZE - FRAMEDIFFERENCE)) > 0) ? _deviceTotalFrames + 1 : _deviceTotalFrames;

            _deviceTotalFrames = _ethernetRecvBuff[8];
            _deviceTotalFrames += Convert.ToUInt32(_ethernetRecvBuff[9] << 8);
            _deviceTotalFrames += Convert.ToUInt32(_ethernetRecvBuff[10] << 16);
            _deviceTotalFrames += Convert.ToUInt32(_ethernetRecvBuff[11] << 32);

            //_deviceFileID = _ethernetRecvBuff[12];// File ID
            //_deviceFileID += Convert.ToInt16(_ethernetSendBuff[13] << 8);
        }

        /// <summary>
        /// This Function Closes the the port by sending Finish Communication signal. 
        /// It first checks if port is opend, if yes then Closed it.
        /// </summary>
        public override void Close()
        {
            _ethernetRecvBuff = null;
            _ethernetSendBuff = null;
            if (_ethernetServerSock != null)
            {
                _ethernetServerSock.Close();
            }
        }


        ~Ethernet()
        {
            _ethernetRecvBuff = null;
            _ethernetSendBuff = null;
            if (_ethernetServerSock != null)
            {
                _ethernetServerSock.Close();
            }
        }

        #endregion
    }
}
