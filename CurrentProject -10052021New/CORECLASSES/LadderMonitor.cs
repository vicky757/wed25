/*===================================================================
 //
// Copyright 2005-2008, Renu Electronics Pvt. Ltd., Pune, India.
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
// File Name	LadderMonitor.cs
// Author		Pravin Yadav
//=====================================================================
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.Win32.SafeHandles;
using CommConst = ClassList.CommonConstants;
using System.Threading;
using System.Timers;
using System.IO;
using System.IO.Ports;
using System.Net;//Monitoring Port- AD
using System.Net.Sockets;//Monitoring Port- AD
using TestVCDLL;

namespace ClassList
{
    public class LadderMonitor
    {
        #region public Declarations
        public ArrayList m_listTagAddress = null;
        public int Total_Frames = 0;
        public int FRAMESIZE = 64;
        public int DataCount = 0;
        public int Num_Register;
        public bool bConnect = false;
        #endregion

        #region Private Declarations
        private TestVCDLL.Class1 obj = new Class1();
        private Thread _USBReadThread;
        private Thread _USBWriteThread;
        private Thread _USBCloseThread;

        private int T_Temporary_Start_Addr = 0; //for Temporary Register
        private int X_PhysicalInput_Start_Addr = 2; //for Physical Input
        private int Y_PhysicalOutput_Start_Addr = 802; //for Physical Output
        private int B_InternalCoil_Start_Addr = 1602; //for Auxiliary registers
        private int M_ConfigRegister_Start_Addr = 2114; //for Configuration Register
        private int T_TimerRegister_Start_Addr = 5314; //for Timer Register
        private int C_CounterRegister_Start_Addr = 5826; //for Counter Register
        private int D_DataRegister_Start_Addr = 6338; //for Data Register        
        private int S_SystemRegister_Start_Addr = 14530; //for System Register
        private int S_SystemDevice_Start_Addr = 15042; //for System Register        
        private int T_TimerDevice_Start_Addr = 15056; //for Timer Devices
        private int C_CounterDevice_Start_Addr = 15088; //for Counter Devices
        private int I_Index_Start_Addr = 15122; //for I 
        private int J_Index_Start_Addr = 15124; //for J
        private int K_Index_Start_Addr = 15126; //for K
        private int R_Retentive_Start_Addr = 20000; //for Retentive registers
        private const string X_Prefix = "X";
        private const string Y_Prefix = "Y";
        private const string R_Prefix = "B";
        private const string S_Prefix = "S";
        private const string M_Prefix = "M";
        private const string TDevice_Prefix = "T.";
        private const string CDevice_Prefix = "C.";
        private const string XW_Prefix = "XW";
        private const string YW_Prefix = "YW";
        private const string RW_Prefix = "BW";
        private const string SW_Prefix = "SW";
        private const string MW_Prefix = "MW";
        private const string T_Prefix = "T";
        private const string C_Prefix = "C";
        private const string D_Prefix = "D";
        private const string I_Prefix = "I";
        private const string J_Prefix = "J";
        private const string K_Prefix = "K";
        private const string Retentive_Prefix = "R";
        private int m_Starting_Addr = 0;
        private int m_byte_len = 0;

        private byte[] SendBuff;
        private byte[] SendBuff_EventHistory;
        private byte[] WriteBuff;
        private byte[] RecvBuff;
        private int NoOfByteRead;
        private int Length_Buffer;
        private int m_i_RetryCount = 3;
        private int m_i_Interframedelay;
        private int m_i_Timeout;
        private int Bit_Position;
        private ArrayList _listOfSlotsIoAllocation = null;
        ClassList.CommonConstants.PlcModuleHeaderInfo _ObjModuleHeaderInfo;
        private ArrayList m_ListofBlocks = new ArrayList();
        public int _serialNoOfByteRead = 0;
        private byte _serialReceivedByte = 0;
        #region Monitoring Port- AD
        public int _ethernetNoOfByteRead = 0;
        private Socket _ethernetSocket;
        private byte _ethernetReceivedByte = 0;
        #endregion
        public bool b_ReadCompleteFlag = false;
        private bool b_USBReadCompleteFlag = false;
        private bool b_loopBreak = false;

        private int i_Communication_Type = 0;
        private SerialPort _serialPort;        
        private bool _deviceIsConnected;
        private int _devicePortNo = 4;
        public const int STARTFRAMESIZE = 20;
        public const int Divider = 124;
        public const int m_ReadTimeout = 300;
        public const int m_ReadInterval = 1000;
        public int Scan_Time = 2000;
        #endregion

        public bool b_PortResetFlag = false; //USB_PORT_RESET

        #region Public Properties
        public static Guid HIDGuid
        {
            get
            {
                //return new Guid("0DFF9F3F-16D5-44A5-A6C1-1E4348AC9626"); //ID specified in Prizm3.12. Its Global Unique ID                            
                return new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");  //USB_PORT_RESET
            }
        }
        #endregion

        #region Public Constructor
        public LadderMonitor(int commType)
        {
            i_Communication_Type = commType;

            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.USB))
            {
                _USBReadThread = new Thread(new ThreadStart(ReadFunction));
                _USBWriteThread = new Thread(new ThreadStart(WriteFunction));
                _USBCloseThread = new Thread(new ThreadStart(CloseUSBDevice));
                FRAMESIZE = 64;
            }
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Serial))
                FRAMESIZE = 256;

            m_listTagAddress = new ArrayList();

        }
        #endregion

        #region Public Methods
        public bool Connect()
        {
            /*bool blFlag = false;
            Guid gHid = HIDGuid;
            TestVCDLL.Class1.ProductName = string.Empty;
            blFlag = obj.OpenUSB_Dev(gHid);

            if (blFlag)
            {
                //Thread.Sleep(100);
                Thread.Sleep(500);
                return true;
            }
            else
            {
                MessageBox.Show("USB device not found, hence can not start communication with PLC.");
                return false;
            }*/

            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.USB))
            {
                FRAMESIZE = 64;

                try
                {
                    if (Connect_USB())
                        return true;
                }
                catch (Exception e)
                {
                    ClassList.ExceptionLogger.Operationslog(e, CoreConstStrings.ExCommunicationError, CoreConstStrings.ExGlobalErrorHdr);
                }
            }
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Serial))
            {
                FRAMESIZE = 256;
                if (Connect_Serial())
                    return true;
            }
            //Monitoring Port- AD
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Ethernet))
            {
                FRAMESIZE = 1400;
                if (Connect_Ethernet())
                    return true;
            }
            //End

            return false;

        }

        /*public bool IsDevice_Connected()
        {
            bool bFlag = false;
            Guid gHid = HIDGuid;
            bFlag = obj.IsDevice_Connected(gHid);

            if (bFlag)
                return true;
            else
                return false;
        }
        */

        private bool Connect_USB()
        {
            bool blFlag = false;
            Guid gHid = HIDGuid;
            TestVCDLL.Class1.ProductName = string.Empty;
            blFlag = obj.OpenUSB_Dev(gHid);

            #region CR for FL005 product for retry- AD
            if (!blFlag)
            {
                for (int retryCount = 0; retryCount < 3; retryCount++)
                { 
                    Thread.Sleep(2000);
                    blFlag = obj.OpenUSB_Dev(gHid);
                    if (blFlag)
                        break;
                }
            }
            #endregion

            if (blFlag)
            {
                //USB_PORT_RESET - START
                if (ClassList.CommonConstants.IsProductUSBPortReset(ClassList.CommonConstants.ProductDataInfo.iProductID))
                    obj.USB_CancelPendingIO();
                //USB_PORT_RESET - END
                //Thread.Sleep(100);
                bConnect = true;
                Thread.Sleep(500);
                return true;
            }
            else
            {
                MessageBox.Show("USB device not found, hence can not start communication.");//Issue600_SY
                return false;
            }
        }

        public bool AutoConnect()
        {
            bool blFlag = false;
            Guid gHid = HIDGuid;
            TestVCDLL.Class1.ProductName = string.Empty;
            blFlag = obj.OpenUSB_Dev(gHid);

            if (blFlag)
            {
                bConnect = true;
                Thread.Sleep(500);
                return true;
            }
            return false;
        }

        private bool Connect_Serial()
        {
            if (_deviceIsConnected)
            {
                // MessageBox.Show("Port is busy or unavailable");
                MessageBox.Show("Port is busy or unavailable.", CommonConstants._softwareName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return false;
            }
            try
            {
                _serialPort = new SerialPort("COM" + ClassList.CommonConstants.SERIAL_PORT.ToString());
                _serialPort.ReadBufferSize = 8500;
                _serialPort.Open();
            }
            catch (SystemException ex)
            {
                ClassList.ExceptionLogger.Operationslog(ex, CoreConstStrings.ExCommunicationError, CoreConstStrings.ExGlobalErrorHdr);
            }
            if (!_serialPort.IsOpen)
            {
                //If Not Connected                
                // MessageBox.Show("Port is busy or unavailable");
                MessageBox.Show("Port is busy or unavailable.", CommonConstants._softwareName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return false;
            }

            _serialPort.WriteTimeout = 2000;
            _serialPort.ReadTimeout = 0xFFFFFFF;

            _serialPort.BaudRate = ClassList.CommonConstants.BAUDRATE;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = ClassList.CommonConstants.BITESIZE;
            _serialPort.StopBits = StopBits.One;

            _deviceIsConnected = true;
            return true;
        }

        #region Monitoring Port- AD
        /// <summary>
        /// This function is used to establish socket connection
        /// </summary>
        /// <returns></returns>        
        private bool Connect_Ethernet()
        {
            IPAddress _ethrIPAddress = IPAddress.Parse(CommConst.strIPAddress);
            IPEndPoint objIPEndPoint = new IPEndPoint(_ethrIPAddress, CommonConstants._ethernetMonitoringPort);

            try
            {
                _ethernetSocket = new Socket(_ethrIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _ethernetSocket.ReceiveTimeout = 10000;
                _ethernetSocket.SendTimeout = 10000;
            }
            catch (Exception ex)
            {
                return false;
            }

            try
            {
                _ethernetSocket.Connect(objIPEndPoint);
            }
            catch (System.Net.Sockets.SocketException SocketException)
            {
                MessageBox.Show("Please check ethernet setting and port in unit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This is a setup frame query which consist list of all tags which need to be monitored
        /// This function is called when datamonitor or ladder screen window resizes or scrolled
        /// </summary>
        /// <returns> 0 and 2 (0 for sucessful and 2 for giving error message)</returns>
        public int Setup_ConfigureQuery_Ethernet()
        {
            int Total_Frames_Minus;
            int bytesRead = 0;
            int NumberofRegisterRead = 0;
            int Read_Cnt = 0;
            int EtherFrameSize = 1400;
            Num_Register = m_listTagAddress.Count;
            bool bflagResponse = false;

            int maxNoOfRegistersInFrame = 696;
            Total_Frames = (Num_Register / maxNoOfRegistersInFrame) + 1;

            if ((Num_Register % maxNoOfRegistersInFrame) == 0)
                Total_Frames_Minus = Total_Frames - 1;
            else
                Total_Frames_Minus = Total_Frames;

            for (int FrameCount = 1; FrameCount <= Total_Frames; FrameCount++)
            {
                if (FrameCount == Total_Frames)
                {
                    bytesRead = Convert.ToInt32((Num_Register * 2) % (EtherFrameSize - 8));
                    NumberofRegisterRead = Num_Register % maxNoOfRegistersInFrame;
                    if (bytesRead == 0)
                        break;
                }
                else
                {
                    bytesRead = EtherFrameSize - 8;
                    NumberofRegisterRead = maxNoOfRegistersInFrame;
                }

                MakeFrame_ConfigureQuery(Total_Frames_Minus, bytesRead, FrameCount, NumberofRegisterRead, Read_Cnt);

                SendConfigureQuery_Ethernet();
                Thread.Sleep(0);
                bflagResponse = ReadConfigureQuery_Ethernet();

                Read_Cnt += NumberofRegisterRead;

                if (bflagResponse && _ethernetReceivedByte == 0x01)
                    continue;
                else
                    return 2;
            }

            if (bflagResponse && _ethernetReceivedByte == 0x01)
            {
                MakeFrame_ReadQuery();
            }
            else
                return 2;

            return 0;
        }

        /// <summary>
        /// Sends configure query
        /// Configure query (Frame Details - following are fixed 8 bytes in frame)
        /// 01 - Identifer
        /// 30 - Function code
        /// 00 - Frame Number
        /// 00 - Num of Reg
        /// 00 - Num of Reg
        ///         |
        ///         |
        ///         |
        ///         | Tag Addresses
        ///         |
        ///         |
        ///         |
        /// 00 - CRC
        /// 00 - CRC
        /// </summary>
        public void SendConfigureQuery_Ethernet()
        {
            try
            {
                _ethernetSocket.Send(SendBuff, Length_Buffer, 0);
            }
            catch (Exception ex)
            {
                ex.GetType();
            }
        }

        /// <summary>
        /// This is a response for Configure Query
        /// receives 8 byte as 1, 1, 1, 1, 1, 1, 1, 1 
        /// </summary>
        /// <returns></returns>
        public bool ReadConfigureQuery_Ethernet()
        {
            _ethernetSocket.ReceiveTimeout = Scan_Time;
            RecvBuff = new byte[8];
            _ethernetReceivedByte = 0;
            _ethernetNoOfByteRead = 0;

            try
            {
                _ethernetNoOfByteRead = _ethernetSocket.Receive(RecvBuff, 0, RecvBuff.Length, 0);
            }
            catch (TimeoutException timeOut)
            {
                return false;
            }
            catch (Exception ex)
            {
                ex.GetType();
            }

            if (_ethernetNoOfByteRead > 0)
            {
                _ethernetReceivedByte = RecvBuff[0];
                return true;
            }

            return false;
        }

        /// <summary>
        /// sends query for "n" no. of registers
        /// </summary>
        public void RequestQueryForRegisters()
        {
            try
            {
                _ethernetSocket.Send(SendBuff, Length_Buffer, 0);
            }
            catch (SystemException ex)
            {
                ex.GetType();
            }
        }

        /// <summary>
        /// Receives response for query which contains updated value for tags and these 
        /// updated value are reflected in ladder screen and data monitor
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool ResponseQueryForRegisters(int length)
        {
            int BytesToBeRead = length + 8;
            RecvBuff = new byte[BytesToBeRead];

            try
            {
                _ethernetNoOfByteRead = _ethernetSocket.Receive(RecvBuff, BytesToBeRead, 0);
            }
            catch (SystemException ex)
            {
                ex.GetType();
                return false;
            }

            //if (CheckResponse(RecvBuff))
            {
                CommonConstants.LadderMonitorTagInfo objTagInfo;
                if (_ethernetNoOfByteRead > 0)
                {
                    byte[] Tag_Data = new byte[2];
                    int[] Tag_value = new int[length / 2];

                    int cnt = 0;
                    int m = 0;

                    for (int i = 6; i < BytesToBeRead - 2; i++)
                    {
                        Tag_Data[++cnt] = RecvBuff[i++];
                        Tag_Data[--cnt] = RecvBuff[i];
                        Tag_value[m] = CommonConstants.MAKEWORD(Tag_Data);
                        objTagInfo = (CommonConstants.LadderMonitorTagInfo)m_listTagAddress[DataCount];
                        objTagInfo.value = Tag_value[m];
                        m_listTagAddress[DataCount++] = objTagInfo;

                        m++;
                    }
                    return true;
                }
            }
            //else
            //{
            //    return false;
            //}
            return false;
        }
        #endregion

        public int Setup_ConfigureQuery()
        {
            int retValue = 0;
            /*switch (i_Communication_Type)
            {
                case Convert.ToInt16(ClassList.CommunicationType.USB)
                    retValue = Setup_ConfigureQuery_USB();
                    break;
                case Convert.ToInt16(ClassList.CommunicationType.Serial):
                    retValue = Setup_ConfigureQuery_Serial();
                    break;
            }*/

            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.USB))
                retValue = Setup_ConfigureQuery_USB();
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Serial))
                retValue = Setup_ConfigureQuery_Serial();
            #region Monitoring Port- AD
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Ethernet))
                retValue = Setup_ConfigureQuery_Ethernet();
            #endregion

            return retValue;
        }

        public int Setup_ConfigureQuery_USB()
        {

            //bool blFlag = false;
            bool bflagResponse = false;
            //Guid gHid = HIDGuid;

            int Total_Frames_Minus = 0;

            Num_Register = m_listTagAddress.Count;
            int bytesRead = 0;
            int NumberofRegisterRead = 0;
            int Read_Cnt = 0;

            Total_Frames = (Num_Register / 28) + 1;

            if ((Num_Register % 28) == 0)
                Total_Frames_Minus = Total_Frames - 1;
            else
                Total_Frames_Minus = Total_Frames;
            //blFlag = obj.OpenUSB_Dev(gHid);
            //if (blFlag)
            //{
            for (int iTemp = 1; iTemp <= Total_Frames; iTemp++)
            {
                /*if (iTemp == (Total_Frames))
                {
                    bytesRead = (Convert.ToInt32((Num_Register * 2) % (FRAMESIZE - 8)));
                    NumberofRegisterRead = Num_Register % 28;
                    if (bytesRead == 0)
                        break;
                }
                else
                {
                    bytesRead = FRAMESIZE - 8;
                    NumberofRegisterRead = 28;
                }

                MakeFrame_ConfigureQuery(Total_Frames_Minus, bytesRead, iTemp, NumberofRegisterRead, Read_Cnt);

                for (int i = 0; i < m_i_RetryCount; i++)
                {
                    SendConfigureQuery();
                    bflagResponse = ReadConfigureQuery();                    
                    if (bflagResponse == true)
                        break;
                }

                Read_Cnt += NumberofRegisterRead;

                if (bflagResponse == true)
                    continue;
                else
                    return 2;*/

                if (iTemp == (Total_Frames))
                {
                    bytesRead = (Convert.ToInt32((Num_Register * 2) % (FRAMESIZE - 8)));
                    NumberofRegisterRead = Num_Register % 28;
                    if (bytesRead == 0)
                        break;
                }
                else
                {
                    bytesRead = FRAMESIZE - 8;
                    NumberofRegisterRead = 28;
                }

                MakeFrame_ConfigureQuery(Total_Frames_Minus, bytesRead, iTemp, NumberofRegisterRead, Read_Cnt);
                //SendConfigureQuery();
                b_ReadCompleteFlag = false;

                for (int i = 0; i < m_i_RetryCount; i++)
                {
                    _USBReadThread = new Thread(new ThreadStart(ReadFunction));
                    _USBWriteThread = new Thread(new ThreadStart(WriteFunction));
                    _USBWriteThread.Start();
                    _USBReadThread.Start();

                    for (int j = 0; j < CommonConstants.SleepMultiplier; j++)
                    {
                        if (b_ReadCompleteFlag)
                            break;
                        Thread.Sleep(CommonConstants.SleepCount);
                    }
                    if (b_ReadCompleteFlag)
                        break;
                    /*else
                    {
                        SendConfigureQuery();
                    }*/

                }

                Read_Cnt += NumberofRegisterRead;

                if (b_ReadCompleteFlag == true)
                    continue;
                else
                    return 2;

            }

            if (b_ReadCompleteFlag)
            //if(bflagResponse)
            {
                MakeFrame_ReadQuery();
            }
            else
                return 2;
            /*}
            else
            {
                return 1;
            }*/
            return 0;
        }

        #region USB_PORT_RESET
        public int Setup_ConfigureQuery_New()
        {
            bool bflagResponse = false;
            int Total_Frames_Minus = 0;
            Num_Register = m_listTagAddress.Count;
            int bytesRead = 0;
            int NumberofRegisterRead = 0;
            int Read_Cnt = 0;

            Total_Frames = (Num_Register / 28) + 1;

            if ((Num_Register % 28) == 0)
                Total_Frames_Minus = Total_Frames - 1;
            else
                Total_Frames_Minus = Total_Frames;

            for (int iTemp = 1; iTemp <= Total_Frames; iTemp++)
            {
                if (iTemp == (Total_Frames))
                {
                    bytesRead = (Convert.ToInt32((Num_Register * 2) % (FRAMESIZE - 8)));
                    NumberofRegisterRead = Num_Register % 28;
                    if (bytesRead == 0)
                        break;
                }
                else
                {
                    bytesRead = FRAMESIZE - 8;
                    NumberofRegisterRead = 28;
                }

                MakeFrame_ConfigureQuery(Total_Frames_Minus, bytesRead, iTemp, NumberofRegisterRead, Read_Cnt);
                b_ReadCompleteFlag = false;

                //for (int i = 0; i < m_i_RetryCount; i++)
                for (int i = 0; i < 1; i++)
                {
                    _USBWriteThread = new Thread(new ThreadStart(WriteFunction));
                    _USBWriteThread.Start();
                    Thread.Sleep(100);
                    _USBReadThread = new Thread(new ThreadStart(ReadFunction));
                    _USBReadThread.Start();

                    for (int j = 0; j < CommonConstants.SleepMultiplier; j++)
                    {
                        if (b_ReadCompleteFlag)
                            break;
                        Thread.Sleep(CommonConstants.SleepCount);
                    }
                    if (b_ReadCompleteFlag)
                        break;
                    else
                    {
                        //USB_PORT_RESET 
                        if (ClassList.CommonConstants.IsProductUSBPortReset(ClassList.CommonConstants.ProductDataInfo.iProductID))
                        {
                            //MessageBox.Show("+1");
                            //Thread.Sleep(100);
                            //USB_PORT_RESET();
                            //Thread.Sleep(500);
                        }
                        //End
                    }
                }

                Read_Cnt += NumberofRegisterRead;
                if (b_ReadCompleteFlag == true)
                    continue;
                else
                    return 2;
            }

            if (b_ReadCompleteFlag)
                MakeFrame_ReadQuery();
            else
                return 2;

            return 0;
        }
        #endregion

        public int Setup_ConfigureQuery_Serial()
        {
            bool bflagResponse = false;
            int Total_Frames_Minus = 0;
            Num_Register = m_listTagAddress.Count;
            int bytesRead = 0;
            int NumberofRegisterRead = 0;
            int Read_Cnt = 0;

            Total_Frames = (Num_Register / Divider) + 1;

            if ((Num_Register % Divider) == 0)
                Total_Frames_Minus = Total_Frames - 1;
            else
                Total_Frames_Minus = Total_Frames;
            for (int iTemp = 1; iTemp <= Total_Frames; iTemp++)
            {
                if (iTemp == (Total_Frames))
                {
                    bytesRead = (Convert.ToInt32((Num_Register * 2) % (FRAMESIZE - 8)));
                    NumberofRegisterRead = Num_Register % Divider;
                    if (bytesRead == 0)
                        break;
                }
                else
                {
                    bytesRead = FRAMESIZE - 8;
                    NumberofRegisterRead = Divider;
                }

                MakeFrame_ConfigureQuery(Total_Frames_Minus, bytesRead, iTemp, NumberofRegisterRead, Read_Cnt);

                for (int i = 0; i < m_i_RetryCount; i++)
                {
                    SendConfigureQuery_Serial();
                    Thread.Sleep(0);
                    bflagResponse = ReadConfigureQuery_Serial();
                    if (bflagResponse && _serialReceivedByte == 0x01)
                        break;
                }

                Read_Cnt += NumberofRegisterRead;

                if (bflagResponse && _serialReceivedByte == 0x01)
                    continue;
                else
                    return 2;
            }
            if (bflagResponse && _serialReceivedByte == 0x01)
            {
                MakeFrame_ReadQuery();
            }
            else
                return 2;

            return 0;
        }

        private void ReadFunction()
        {
            NewReadConfigureQuery();
        }

        private void WriteFunction()
        {
            SendConfigureQuery();
        }

        public void MakeFrame_ConfigureQuery(int total_frames, int bytesRead, int frame_number, int NumofReg, int rCnt)
        {
            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];


            int bufflen = 0;
            int Size = bytesRead + 8;

            Length_Buffer = Size;

            SendBuff = new byte[Size];

            //SendBuff[bufflen++] = 0xAC;
            SendBuff[bufflen++] = 0x01;
            SendBuff[bufflen++] = 0x30;
            SendBuff[bufflen++] = Convert.ToByte(total_frames);
            SendBuff[bufflen++] = Convert.ToByte(frame_number);

            temp_arr = CommonConstants.GetHalfWord(NumofReg);
            SendBuff[bufflen++] = temp_arr[1];
            SendBuff[bufflen++] = temp_arr[0];

            for (int i = 0; i < NumofReg; i++)
            {
                temp_arr = GetAddress(((CommonConstants.LadderMonitorTagInfo)m_listTagAddress[rCnt++]).TagAddress);
                SendBuff[bufflen++] = temp_arr[1];
                SendBuff[bufflen++] = temp_arr[0];
            }

            CalculateCRC(ref CRC, SendBuff);
            SendBuff[bufflen++] = CRC[0];
            SendBuff[bufflen++] = CRC[1];
        }

        public void SendConfigureQuery()
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
        public void SendConfigureQuery_Serial()
        {
            try
            {
                _serialPort.Write(SendBuff, 0, Length_Buffer);
            }
            catch (SystemException ex)
            {
                ex.GetType();
            }
        }

        public bool ReadConfigureQuery()
        {
            RecvBuff = new byte[6];
            m_i_Timeout = 0;

            DateTime endTime = DateTime.Now.AddMilliseconds(20);

            while (DateTime.Now < endTime)
            {
                try
                {
                    unsafe
                    {
                        fixed (byte* p = RecvBuff)
                        {

                            NoOfByteRead = obj.ReadUSB_Device(p, 1);
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

                else
                {
                    //MessageBox.Show("CRC error");
                    return false;
                }
            }

            return false;

        }

        public bool ReadConfigureQuery_Serial()
        {
            //_serialPort.ReadTimeout = m_ReadTimeout;
            _serialPort.ReadTimeout = Scan_Time;
            RecvBuff = new byte[8];
            _serialReceivedByte = 0;
            _serialNoOfByteRead = 0;
            try
            {
                for (int i = 0; i < RecvBuff.Length; i++)
                {
                    RecvBuff[i] = Convert.ToByte(_serialPort.ReadByte());
                    _serialNoOfByteRead++;
                }
                //_serialNoOfByteRead = _serialPort.Read(RecvBuff, 0, 8);

            }
            catch (System.TimeoutException timeOut)
            {
                return false;
            }
            catch (SystemException ex)
            {
                ex.GetType();
            }

            if (_serialNoOfByteRead > 0)
            {
                _serialReceivedByte = RecvBuff[0];
                return true;
            }
            return false;
        }
               
        public void NewReadConfigureQuery()
        {
            RecvBuff = new byte[6];
            m_i_Timeout = 0;

            unsafe
            {
                fixed (byte* p = RecvBuff)
                {
                    NoOfByteRead = obj.ReadUSB_Device(p, 1);
                    if (NoOfByteRead > 0)
                        b_ReadCompleteFlag = true;
                    else
                        b_ReadCompleteFlag = false;
                }
            }

        }

        public void MakeFrame_ReadQuery()
        {
            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];

            int bufflen = 0;

            Num_Register = m_listTagAddress.Count;

            Length_Buffer = 6;

            SendBuff = new byte[Length_Buffer];

            SendBuff[bufflen++] = 0x01;
            SendBuff[bufflen++] = 0x31;

            temp_arr = CommonConstants.GetHalfWord(Num_Register);
            SendBuff[bufflen++] = temp_arr[1];
            SendBuff[bufflen++] = temp_arr[0];

            CalculateCRC(ref CRC, SendBuff);
            SendBuff[bufflen++] = CRC[0];
            SendBuff[bufflen++] = CRC[1];

        }

        public void SendReadQuery()
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

        //USB_PORT_RESET
        public void SendReadQueryPortReset()
        {
            b_PortResetFlag = false;
            unsafe
            {
                bool blFlag = false;
                fixed (byte* p = SendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, Length_Buffer);
                    if (blFlag)
                        b_PortResetFlag = true;
                    else
                        b_PortResetFlag = false;
                }
            }
        }
        //End

        public void SendReadQueryNew()
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

        public void DebugSendReadQuery()
        {
            byte[] m_arr = new byte[2];
            m_arr[0] = 0xAC;
            m_arr[1] = 0xBC;
            unsafe
            {
                bool blFlag = false;
                fixed (byte* p = m_arr)
                {

                    blFlag = obj.WriteUSB_Device(p, 2);
                }
            }
        }


        public bool GetResponse_ReadQuery(int bytes_lengh)
        {

            int len = bytes_lengh + 8;
            RecvBuff = new byte[len];


            DateTime endTime = DateTime.Now.AddMilliseconds(20);

            while (DateTime.Now < endTime)
            {
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
                    break;
                }
            }




            if (CheckResponse(RecvBuff))
            {
                CommonConstants.LadderMonitorTagInfo objTagInfo;// = new CommonConstants.LadderMonitorTagInfo();
                if (NoOfByteRead > 0)
                {
                    byte[] Tag_Data = new byte[2];
                    int[] Tag_value = new int[bytes_lengh / 2];

                    int cnt = 0;
                    int m = 0;

                    unsafe
                    {
                        bool blFlag = false;
                        byte[] ReadResponse = new byte[2];
                        ReadResponse[0] = 0x01;
                        ReadResponse[1] = 0x01;
                        fixed (byte* p = ReadResponse)
                        {
                            blFlag = obj.WriteUSB_Device(p, 2);
                        }
                    }

                    for (int i = 6; i < len - 2; i++)
                    {
                        Tag_Data[++cnt] = RecvBuff[i++];
                        Tag_Data[--cnt] = RecvBuff[i];
                        Tag_value[m] = CommonConstants.MAKEWORD(Tag_Data);
                        objTagInfo = (CommonConstants.LadderMonitorTagInfo)m_listTagAddress[DataCount];
                        objTagInfo.value = Tag_value[m];
                        m_listTagAddress[DataCount++] = objTagInfo;

                        m++;
                    }

                    return true;
                }
            }
            else
            {
                unsafe
                {
                    bool blFlag = false;
                    byte[] ReadResponse = new byte[2];
                    ReadResponse[0] = 0xFF;
                    ReadResponse[1] = 0xFF;
                    fixed (byte* p = ReadResponse)
                    {
                        blFlag = obj.WriteUSB_Device(p, 2);
                    }
                }
                //MessageBox.Show("CRC error");
                return false;
            }

            return false;
        }

        public void SendReadQuery_Serial()
        {
            try
            {
                _serialPort.Write(SendBuff, 0, Length_Buffer);
            }
            catch (SystemException ex)
            {
                ex.GetType();
            }
        }

        public bool GetResponse_ReadQuery_Serial(int bytes_lengh)
        {
            byte[] ReadResponse = new byte[2];
            int len = bytes_lengh + 8;
            RecvBuff = new byte[len];

            //_serialPort.ReadTimeout = m_ReadInterval;            
            _serialPort.ReadTimeout = Scan_Time;
            _serialReceivedByte = 0;
            _serialNoOfByteRead = 0;

            try
            {
                //_serialNoOfByteRead = _serialPort.Read(RecvBuff, 0, len);
                for (int i = 0; i < len; i++)
                {
                    RecvBuff[i] = (byte)_serialPort.ReadByte();
                    _serialNoOfByteRead++;
                }
            }
            catch (System.TimeoutException timeOut)
            {
                return false;
            }
            catch (SystemException ex)
            {
                ex.GetType();
            }

            if (CheckResponse(RecvBuff))
            {
                CommonConstants.LadderMonitorTagInfo objTagInfo;
                if (_serialNoOfByteRead > 0)
                {
                    byte[] Tag_Data = new byte[2];
                    int[] Tag_value = new int[bytes_lengh / 2];

                    int cnt = 0;
                    int m = 0;

                    for (int i = 6; i < len - 2; i++)
                    {
                        Tag_Data[++cnt] = RecvBuff[i++];
                        Tag_Data[--cnt] = RecvBuff[i];
                        Tag_value[m] = CommonConstants.MAKEWORD(Tag_Data);
                        objTagInfo = (CommonConstants.LadderMonitorTagInfo)m_listTagAddress[DataCount];
                        objTagInfo.value = Tag_value[m];
                        m_listTagAddress[DataCount++] = objTagInfo;

                        m++;
                    }
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        public void FlushBuffer()
        {
            try
            {
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
            }
            catch (SystemException ex)
            {
                ex.GetType();
            }
        }


        public void GetResponse_ReadQueryNew(int bytes_lengh)
        {
            b_ReadCompleteFlag = false;
            int len = bytes_lengh + 8;
            RecvBuff = new byte[len];

            unsafe
            {
                fixed (byte* p = RecvBuff)
                {
                    NoOfByteRead = obj.ReadUSB_Device(p, len);
                    if (NoOfByteRead > 0)
                        b_ReadCompleteFlag = true;
                    else
                        b_ReadCompleteFlag = false;
                }
            }

            /*if (CheckResponse(RecvBuff))
            {
                CommonConstants.LadderMonitorTagInfo objTagInfo;// = new CommonConstants.LadderMonitorTagInfo();
                if (NoOfByteRead > 0)
                {
                    byte[] Tag_Data = new byte[2];
                    int[] Tag_value = new int[bytes_lengh / 2];

                    int cnt = 0;
                    int m = 0;

                    unsafe
                    {
                        bool blFlag = false;
                        byte[] ReadResponse = new byte[2];
                        ReadResponse[0] = 0x01;
                        ReadResponse[1] = 0x01;
                        fixed (byte* p = ReadResponse)
                        {
                            blFlag = obj.WriteUSB_Device(p, 2);
                        }
                    }

                    for (int i = 6; i < len - 2; i++)
                    {
                        Tag_Data[++cnt] = RecvBuff[i++];
                        Tag_Data[--cnt] = RecvBuff[i];
                        Tag_value[m] = CommonConstants.MAKEWORD(Tag_Data);
                        objTagInfo = (CommonConstants.LadderMonitorTagInfo)m_listTagAddress[DataCount];
                        objTagInfo.value = Tag_value[m];
                        m_listTagAddress[DataCount++] = objTagInfo;

                        m++;
                    }

                    b_ReadCompleteFlag = true;
                    //return true;
                }
            }
            else
            {
                unsafe
                {
                    bool blFlag = false;
                    byte[] ReadResponse = new byte[2];
                    ReadResponse[0] = 0xFF;
                    ReadResponse[1] = 0xFF;
                    fixed (byte* p = ReadResponse)
                    {
                        blFlag = obj.WriteUSB_Device(p, 2);
                    }
                }
                b_ReadCompleteFlag = false;
                //MessageBox.Show("CRC error");
                //return false;
            }*/

            //return false;
        }

        public bool SendAck(int m_byte_len)
        {
            int len = m_byte_len + 8;
            if (CheckResponse(RecvBuff))
            {
                CommonConstants.LadderMonitorTagInfo objTagInfo;// = new CommonConstants.LadderMonitorTagInfo();
                //if (NoOfByteRead > 0)
                if (b_ReadCompleteFlag)
                {
                    byte[] Tag_Data = new byte[2];
                    int[] Tag_value = new int[m_byte_len / 2];

                    int cnt = 0;
                    int m = 0;

                    unsafe
                    {
                        bool blFlag = false;
                        byte[] ReadResponse = new byte[2];
                        ReadResponse[0] = 0x01;
                        ReadResponse[1] = 0x01;
                        fixed (byte* p = ReadResponse)
                        {
                            blFlag = obj.WriteUSB_Device(p, 2);
                        }
                    }

                    for (int i = 6; i < len - 2; i++)
                    {
                        Tag_Data[++cnt] = RecvBuff[i++];
                        Tag_Data[--cnt] = RecvBuff[i];
                        Tag_value[m] = CommonConstants.MAKEWORD(Tag_Data);
                        objTagInfo = (CommonConstants.LadderMonitorTagInfo)m_listTagAddress[DataCount];
                        objTagInfo.value = Tag_value[m];
                        m_listTagAddress[DataCount++] = objTagInfo;

                        m++;
                    }

                    return true;

                }
            }
            else
            {
                unsafe
                {
                    bool blFlag = false;
                    byte[] ReadResponse = new byte[2];
                    ReadResponse[0] = 0xFF;
                    ReadResponse[1] = 0xFF;
                    fixed (byte* p = ReadResponse)
                    {
                        blFlag = obj.WriteUSB_Device(p, 2);
                    }
                }
                return false;
            }
            return false;
        }

        public void GetNewResponse_ReadQuery(int bytes_lengh)
        {
            b_ReadCompleteFlag = false;
            m_byte_len = bytes_lengh;
            /*_USBReadFromTimerThread = new Thread(new ThreadStart(GetResponse_ReadQueryNew));
            _USBReadFromTimerThread.Start();*/
        }

        public void MakeFrame_WriteRegisterQuery(string pStr, int value)
        {
            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];
            int bufflen = 0;

            WriteBuff = new byte[8];

            WriteBuff[bufflen++] = 0x01;
            WriteBuff[bufflen++] = 0x06;

            temp_arr = GetAddress(pStr);
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

            CalculateCRC(ref CRC, WriteBuff);
            WriteBuff[bufflen++] = CRC[0];
            WriteBuff[bufflen++] = CRC[1];
        }

        public void MakeFrame_WriteRegisterQuery(int addr, int value)
        {
            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];
            int bufflen = 0;

            WriteBuff = new byte[8];

            WriteBuff[bufflen++] = 0x01;
            WriteBuff[bufflen++] = 0x06;

            int raw_variable;
            raw_variable = (addr) & 255;
            temp_arr[0] = Convert.ToByte(raw_variable);
            raw_variable = addr;
            raw_variable = raw_variable >> 8;
            temp_arr[1] = Convert.ToByte((raw_variable & 255));

            WriteBuff[bufflen++] = temp_arr[1];
            WriteBuff[bufflen++] = temp_arr[0];

            raw_variable = (value) & 255;
            temp_arr[0] = Convert.ToByte(raw_variable);
            raw_variable = value;
            raw_variable = raw_variable >> 8;
            temp_arr[1] = Convert.ToByte((raw_variable & 255));

            WriteBuff[bufflen++] = temp_arr[1];
            WriteBuff[bufflen++] = temp_arr[0];

            CalculateCRC(ref CRC, WriteBuff);
            WriteBuff[bufflen++] = CRC[0];
            WriteBuff[bufflen++] = CRC[1];
        }

        public void MakeFrame_WriteCoilQuery(string pStr, int value)
        {
            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];
            int bufflen = 0;

            WriteBuff = new byte[8];

            WriteBuff[bufflen++] = 0x01;
            WriteBuff[bufflen++] = 0x05;

            temp_arr = GetAddress_Device(pStr);
            WriteBuff[bufflen++] = temp_arr[1];
            WriteBuff[bufflen++] = temp_arr[0];
            WriteBuff[bufflen++] = System.Convert.ToByte(Bit_Position);

            WriteBuff[bufflen++] = System.Convert.ToByte(value);

            CalculateCRC(ref CRC, WriteBuff);
            WriteBuff[bufflen++] = CRC[0];
            WriteBuff[bufflen++] = CRC[1];
        }

        public void MakeFrame_ForceCoilQuery(string pStr, int value)
        {
            byte AddType = 0;
            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];
            int bufflen = 0;
            String strPrefix = "";
            String strTemp = "";
            String strExpAdd = "";
            byte bitNumber = 0;


            strPrefix = GetPrefix(pStr);
            strTemp = pStr.Remove(0, strPrefix.Length);
            strExpAdd = strTemp.Remove(2, strTemp.Length - 2);
            AddType = (byte)Convert.ToInt32(strExpAdd);
            strTemp = strTemp.Remove(0, 2);
            bitNumber = (byte)Convert.ToInt32(strTemp);
            WriteBuff = new byte[8];

            WriteBuff[bufflen++] = 0x01;
            if (strPrefix == "X")
                WriteBuff[bufflen++] = 0x07;
            else
                WriteBuff[bufflen++] = 0x08;

            WriteBuff[bufflen++] = AddType;

            WriteBuff[bufflen++] = bitNumber;

            WriteBuff[bufflen++] = 0;
            WriteBuff[bufflen++] = System.Convert.ToByte(value);


            CalculateCRC(ref CRC, WriteBuff);
            WriteBuff[bufflen++] = CRC[0];
            WriteBuff[bufflen++] = CRC[1];
        }

        public void MakeFrame_ReleaseCoilQuery(string pStr, int value)
        {
            byte AddType = 0;
            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];
            int bufflen = 0;
            String strPrefix = "";
            String strTemp = "";
            byte bitNumber = 0;
            String strExpAdd = "";

            strPrefix = GetPrefix(pStr);
            strTemp = pStr.Remove(0, strPrefix.Length);
            strExpAdd = strTemp.Remove(2, strTemp.Length - 2);
            AddType = (byte)Convert.ToInt32(strExpAdd);
            strTemp = strTemp.Remove(0, 2);
            bitNumber = (byte)Convert.ToInt32(strTemp);
            WriteBuff = new byte[8];

            WriteBuff[bufflen++] = 0x01;
            if (strPrefix == "X")
                WriteBuff[bufflen++] = 0x9;
            else
                WriteBuff[bufflen++] = 0x11;

            WriteBuff[bufflen++] = AddType;

            WriteBuff[bufflen++] = bitNumber;

            WriteBuff[bufflen++] = 0;
            WriteBuff[bufflen++] = 0;


            CalculateCRC(ref CRC, WriteBuff);
            WriteBuff[bufflen++] = CRC[0];
            WriteBuff[bufflen++] = CRC[1];
        }

        public void MakeFrame_ReleaseAllQuery()
        {
            byte AddType = 0;
            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];
            int bufflen = 0;
            String strPrefix = "";
            String strTemp = "";


            WriteBuff = new byte[8];

            WriteBuff[bufflen++] = 0x01;

            WriteBuff[bufflen++] = 0x12;

            WriteBuff[bufflen++] = 0;

            WriteBuff[bufflen++] = 0;

            WriteBuff[bufflen++] = 0;
            WriteBuff[bufflen++] = 0;


            CalculateCRC(ref CRC, WriteBuff);
            WriteBuff[bufflen++] = CRC[0];
            WriteBuff[bufflen++] = CRC[1];
        }

        public void MakeFrame_ForceReadQuery(byte slot, byte Type)
        {

            byte[] CRC = new byte[2];
            int bufflen = 0;
            WriteBuff = new byte[8];

            WriteBuff[bufflen++] = 0x01;

            WriteBuff[bufflen++] = 0x13;

            WriteBuff[bufflen++] = slot;

            WriteBuff[bufflen++] = Type;

            WriteBuff[bufflen++] = 0;
            WriteBuff[bufflen++] = 0;


            CalculateCRC(ref CRC, WriteBuff);
            WriteBuff[bufflen++] = CRC[0];
            WriteBuff[bufflen++] = CRC[1];

        }

        public bool GetResponse_ForceReadQuery(byte slot, byte Type)
        {
            int len = 10;
            // len = size;
            RecvBuff = new byte[10];

            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.USB))
            {
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
            }
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Serial))
            {
                //_serialPort.ReadTimeout = m_ReadTimeout;
                _serialPort.ReadTimeout = Scan_Time;
                _serialReceivedByte = 0;
                try
                {
                    _serialNoOfByteRead = _serialPort.Read(RecvBuff, 0, len);
                }
                catch (System.TimeoutException timeOut)
                {
                    return false;
                }
                catch (SystemException ex)
                {
                    ex.GetType();
                }
                if (_serialNoOfByteRead > 0)
                {
                    return true;
                }

            }

            if (CheckResponse(RecvBuff))
            {
                CommonConstants.LadderMonitorTagInfo objTagInfo;//= new CommonConstants.LadderMonitorTagInfo();
                if (NoOfByteRead > 0)
                {
                    String strValue = "";
                    int value1 = ClassList.CommonConstants.MAKEWORD(RecvBuff[5], RecvBuff[4]);

                    strValue = ClassList.CommonConstants.DecimalToBinary((ushort)value1);

                    //  MessageBox.Show(value1.ToString() );
                    String strTemp = "";
                    int tempValue = 0;
                    int len2 = strValue.Length;
                    for (int i = 0; i < len2; i++)
                    {
                        if (strValue[i] == '1')
                        {
                            tempValue = len2 - i - 1;
                            strTemp = "";
                            if (tempValue < 10)
                            {
                                if (Type == 1)
                                    strTemp = "X";
                                else
                                    strTemp = "Y";
                                strTemp += "0";
                                strTemp += slot.ToString();
                                strTemp += "00";
                                strTemp += tempValue.ToString();
                                if (ClassList.CommonConstants.objListForceIO.Contains(strTemp) == false)
                                    ClassList.CommonConstants.objListForceIO.Add(strTemp);
                            }
                            else
                            {
                                if (Type == 1)
                                    strTemp = "X";
                                else
                                    strTemp = "Y";
                                strTemp += "0";
                                strTemp += slot.ToString();
                                strTemp += "0";
                                strTemp += tempValue.ToString();
                                if (ClassList.CommonConstants.objListForceIO.Contains(strTemp) == false)
                                    ClassList.CommonConstants.objListForceIO.Add(strTemp);

                            }


                        }

                    }


                    return true;

                }
            }
            else
            {
                return false;
            }

            return false;
        }
        public void MakeFrame_WriteRegisterQuery_Float(string pStr, byte btTempData1, byte btTempData2)
        {
            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];
            int bufflen = 0;

            WriteBuff = new byte[8];

            WriteBuff[bufflen++] = 0x01;
            WriteBuff[bufflen++] = 0x06;

            temp_arr = GetAddress(pStr);
            WriteBuff[bufflen++] = temp_arr[1];
            WriteBuff[bufflen++] = temp_arr[0];

            WriteBuff[bufflen++] = btTempData1;
            WriteBuff[bufflen++] = btTempData2;

            CalculateCRC(ref CRC, WriteBuff);
            WriteBuff[bufflen++] = CRC[0];
            WriteBuff[bufflen++] = CRC[1];
        }

        public void SendWriteQuery()
        {
            int len_buff = 8;

            /*unsafe
            {
                bool blFlag = false;
                fixed (byte* p = WriteBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, len_buff);
                }
            }*/

            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.USB))
            {
                unsafe
                {
                    bool blFlag = false;
                    fixed (byte* p = WriteBuff)
                    {
                        blFlag = obj.WriteUSB_Device(p, len_buff);
                    }
                }
            }
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Serial))
            {
                try
                {
                    _serialPort.Write(WriteBuff, 0, len_buff);
                    Thread.Sleep(50);
                }
                catch (SystemException ex)
                {
                    ex.GetType();
                }
            }
            //Monitoring Port- AD
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Ethernet))
            {
                try
                {
                    _ethernetSocket.Send(WriteBuff, len_buff, 0);
                }
                catch (Exception ex)
                {
                    ex.GetType();
                }
            }
            //End
        }

        public void WriteVPData_USB(byte CommandByte, int pSize)
        {
            byte[] _serialSendBuff = new byte[2];
            _serialSendBuff[0] = CommandByte;
            bool blFlag = false;
            unsafe
            {
                fixed (byte* p = _serialSendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, pSize);
                }
            }
        }

        public void WriteVPData_USB(byte[] _serialSendBuff, int pSize)
        {
            bool blFlag = false;
            unsafe
            {
                fixed (byte* p = _serialSendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, pSize);
                }
            }
        }

        //USB_PORT_RESET - START
        public byte ReadVPData_USBPortReset()
        {
            byte[] _serialRecvBuff = new byte[2];
            byte _serialReceivedByte = 0;
            _serialRecvBuff[0] = 0;

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
                return 0;
            }
            if (_serialNoOfByteRead > 0)
            {
                _serialReceivedByte = _serialRecvBuff[0];
                return _serialReceivedByte;
            }
            else
            {
                _serialRecvBuff[0] = 0;
            }
            return 0;

            //byte[] _serialRecvBuff = new byte[2];
            //b_ReadCompleteFlag = false;
            //_serialReceivedByte = 0;

            //_USBReadThread = new Thread(delegate()
            //{
            //    ReadVPData_ReceiveFrame(1, ref _serialRecvBuff);
            //});
            //_USBReadThread.Start();

            //for (int j = 0; j < 2000; j++)
            ////for (int j = 0; j < CommonConstants.SleepMultiplier; j++)
            //{
            //    if (b_ReadCompleteFlag)
            //        break;
            //    Thread.Sleep(1);
            //    //Thread.Sleep(CommonConstants.SleepCount);
            //}

            //if (!b_ReadCompleteFlag)
            //{
            //    USB_PORT_RESET();
            //    return _serialReceivedByte;
            //}
            //else
            //    return _serialReceivedByte;
        }

        private void ReadVPData_ReceiveFrame(int pSize, ref byte[] s_receBuff)
        {
            if (ReceiveFrame(ref s_receBuff, 1))
                b_ReadCompleteFlag = true;
            else
                b_ReadCompleteFlag = false;
        }

        //USB_PORT_RESET - END

        public byte ReadVPData_USB()
        {
            byte[] _serialRecvBuff = new byte[2];
            byte _serialReceivedByte = 0;
            _serialRecvBuff[0] = 0;
            _serialNoOfByteRead = 0;//USB_PORT_RESET 

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
                return 0;
            }
            if (_serialNoOfByteRead > 0)
            {
                _serialReceivedByte = _serialRecvBuff[0];
                return _serialReceivedByte;
            }
            else
            {
                _serialRecvBuff[0] = 0;
            }
            return 0;
        }

        public bool ReadVPData_USB(ref byte[] _serialRecvBuff, int Size)
        {
            byte _serialReceivedByte = 0;
            try
            {
                unsafe
                {
                    fixed (byte* p = _serialRecvBuff)
                    {
                        _serialNoOfByteRead = obj.ReadUSB_Device(p, Size);
                    }
                }

            }
            catch (System.TimeoutException timeOut)
            {
                _serialRecvBuff[0] = 0;
                return false;
            }
            if (_serialNoOfByteRead >= Size)
            {
                return true;
            }
            else
            {
                _serialRecvBuff[0] = 0;
            }
            return false;
        }

        public bool GetResponse_WriteQuery()
        {
            int len = 8;
            // len = size;
            RecvBuff = new byte[len];

            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.USB))
            {
                try
                {
                    unsafe
                    {
                        fixed (byte* p = RecvBuff)
                        {
                            NoOfByteRead = obj.ReadUSB_Device(p, len);
                            // MessageBox.Show(NoOfByteRead.ToString());
                        }
                    }
                }
                catch (System.TimeoutException timeOut)
                {
                    // MessageBox.Show("Exception");
                    return false;
                }
            }
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Serial))
            {
                //_serialPort.ReadTimeout = m_ReadTimeout;
                _serialPort.ReadTimeout = Scan_Time;
                _serialReceivedByte = 0;
                try
                {
                    _serialNoOfByteRead = _serialPort.Read(RecvBuff, 0, len);
                }
                catch (System.TimeoutException timeOut)
                {
                    return false;
                }
                catch (SystemException ex)
                {
                    ex.GetType();
                }
                if (_serialNoOfByteRead > 0)
                {
                    return true;
                }
            }
            //Monitoring Port- AD
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Ethernet))
            {
                try
                {
                    _ethernetNoOfByteRead = _ethernetSocket.Receive(RecvBuff, len, 0);
                }
                catch (System.TimeoutException timeout)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    ex.GetType();
                }

                if (_ethernetNoOfByteRead > 0)
                    return true;
            }
            //End

            if (CheckResponse(RecvBuff))
            {
                CommonConstants.LadderMonitorTagInfo objTagInfo;//= new CommonConstants.LadderMonitorTagInfo();
                if (NoOfByteRead > 0)
                {
                    byte[] Tag_Data = new byte[2];
                    int[] Tag_value = new int[Num_Register];

                    int cnt = 0;
                    int m = 0;

                    for (int i = 6; i < len - 2; i++)
                    {
                        Tag_Data[++cnt] = RecvBuff[i++];
                        Tag_Data[--cnt] = RecvBuff[i];
                        Tag_value[m] = CommonConstants.MAKEWORD(Tag_Data);
                        objTagInfo = (CommonConstants.LadderMonitorTagInfo)m_listTagAddress[m];
                        objTagInfo.value = Tag_value[m];
                        m_listTagAddress[m] = objTagInfo;

                        m++;
                    }
                    //    MessageBox.Show(NoOfByteRead.ToString()); 
                    return true;

                }
            }
            else
            {
                return false;
            }

            return false;
        }



        public void MakeFrame_ReadQuery_EventHistory(int op_addr, int NumberReg)
        {
            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];

            int bufflen = 0;

            SendBuff_EventHistory = new byte[8];

            SendBuff_EventHistory[bufflen++] = 0x01;
            SendBuff_EventHistory[bufflen++] = 0x03;

            int raw_variable;
            raw_variable = (op_addr) & 255;
            temp_arr[0] = Convert.ToByte(raw_variable);
            raw_variable = op_addr;
            raw_variable = raw_variable >> 8;
            temp_arr[1] = Convert.ToByte((raw_variable & 255));

            SendBuff_EventHistory[bufflen++] = temp_arr[1];
            SendBuff_EventHistory[bufflen++] = temp_arr[0];

            temp_arr = CommonConstants.GetHalfWord(NumberReg);
            SendBuff_EventHistory[bufflen++] = temp_arr[1];
            SendBuff_EventHistory[bufflen++] = temp_arr[0];

            CalculateCRC(ref CRC, SendBuff_EventHistory);
            SendBuff_EventHistory[bufflen++] = CRC[0];
            SendBuff_EventHistory[bufflen++] = CRC[1];
        }

        public void SendWriteQuery_EventHistory()
        {
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.USB))
            {
                unsafe
                {
                    bool blFlag = false;
                    fixed (byte* p = SendBuff_EventHistory)
                    {

                        blFlag = obj.WriteUSB_Device(p, 8);
                    }
                }
            }
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Serial))
            {
                try
                {
                    _serialPort.Write(SendBuff_EventHistory, 0, 8);
                }
                catch (SystemException ex)
                {
                    ex.GetType();
                }
            }
        }

        public bool GetResponse_WriteQuery_EventHistory(ref byte[] m_eventList, ref int buff_Count)
        {
            int len = 63;
            RecvBuff = new byte[len];

            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.USB))
            {
                len = 63;
                RecvBuff = new byte[len];

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
            }
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Serial))
            {
                //len = 256;
                len = 255;
                RecvBuff = new byte[len];

                //_serialPort.ReadTimeout = m_ReadTimeout;
                _serialPort.ReadTimeout = Scan_Time;
                _serialReceivedByte = 0;
                try
                {
                    int Index = 0;
                    _serialNoOfByteRead = 0;
                  //  _serialNoOfByteRead = _serialPort.Read(RecvBuff, 0, len);
                    _serialNoOfByteRead = _serialPort.Read(RecvBuff, 0, 3);
                    len = RecvBuff[2] + 2;
                    Index += 3;
                   // MessageBox.Show(len.ToString());
                    for (int i = 0; i < len; i++)
                    {
                        RecvBuff[Index] = (byte)_serialPort.ReadByte();
                        _serialNoOfByteRead++;
                        Index++;
                    }
                    NoOfByteRead = _serialNoOfByteRead;
                }
                catch (System.TimeoutException timeOut)
                {
                    return false;
                }
                catch (SystemException ex)
                {
                    ex.GetType();
                }
            }

            if (CheckResponse(RecvBuff))
            {
                if (NoOfByteRead > 0)
                {
                    int cnt = 0;
                    int m = 0;
                    byte[] Tag_Data = new byte[2];

                    for (int i = 3; i < len - 3; i++)
                    {
                        cnt = 0;

                        if (buff_Count == 512)
                            break;

                        Tag_Data[++cnt] = RecvBuff[i++];
                        Tag_Data[--cnt] = RecvBuff[i];

                        m_eventList[buff_Count++] = Tag_Data[cnt++];
                        m_eventList[buff_Count++] = Tag_Data[cnt];
                    }
                    return true;
                }
            }
            else
            {
                if (NoOfByteRead > 0)
                {
                    if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.USB))
                    {
                        unsafe
                        {
                            bool blFlag = false;
                            byte[] ReadResponse = new byte[2];
                            ReadResponse[0] = 0xFF;
                            ReadResponse[1] = 0xFF;
                            fixed (byte* p = ReadResponse)
                            {
                                blFlag = obj.WriteUSB_Device(p, 2);
                            }
                        }
                    }
                }
                return false;
            }
            return false;
        }

        public bool GetResponse_WriteQuery_IOExpansion(ref byte[] m_ioexpList)
        {
            int len = 39;

            RecvBuff = new byte[len];

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

            if (CheckResponse(RecvBuff))
            {
                if (NoOfByteRead > 0)
                {
                    int cnt = 0;
                    int m = 0;
                    byte[] Tag_Data = new byte[2];

                    for (int i = 3; i < len - 3; i++)
                    {
                        cnt = 0;

                        Tag_Data[++cnt] = RecvBuff[i++];
                        Tag_Data[--cnt] = RecvBuff[i];

                        m_ioexpList[m++] = Tag_Data[cnt++];
                        m_ioexpList[m++] = Tag_Data[cnt];
                    }
                    return true;
                }
            }
            else
            {
                unsafe
                {
                    bool blFlag = false;
                    byte[] ReadResponse = new byte[2];
                    ReadResponse[0] = 0xFF;
                    ReadResponse[1] = 0xFF;
                    fixed (byte* p = ReadResponse)
                    {
                        blFlag = obj.WriteUSB_Device(p, 2);
                    }
                }
                return false;
            }
            return false;
        }

        public void MakeFrame_AttachQuery()
        {
            byte[] temp_arr = new byte[2];
            byte[] CRC = new byte[2];

            int bufflen = 0;
            int Size = 14;
            int NumofReg = 3;

            SendBuff = new byte[Size];

            SendBuff[bufflen++] = 0x01;
            SendBuff[bufflen++] = 0x30;
            SendBuff[bufflen++] = 0x01;
            SendBuff[bufflen++] = 0x01;

            temp_arr = CommonConstants.GetHalfWord(NumofReg);
            SendBuff[bufflen++] = temp_arr[1];
            SendBuff[bufflen++] = temp_arr[0];

            temp_arr = GetAddress(CommonConstants.SystemTag_MainLoopScanTime);
            SendBuff[bufflen++] = temp_arr[1];
            SendBuff[bufflen++] = temp_arr[0];

            temp_arr = GetAddress(CommonConstants.SystemTag_PLCMode);
            SendBuff[bufflen++] = temp_arr[1];
            SendBuff[bufflen++] = temp_arr[0];

            temp_arr = GetAddress(CommonConstants.SystemTag_LadderScanTime);
            SendBuff[bufflen++] = temp_arr[1];
            SendBuff[bufflen++] = temp_arr[0];


            CalculateCRC(ref CRC, SendBuff);
            SendBuff[bufflen++] = CRC[0];
            SendBuff[bufflen++] = CRC[1];
        }


        public void SendAttachFrame()
        {
            unsafe
            {
                bool blFlag = false;
                fixed (byte* p = SendBuff)
                {
                    blFlag = obj.WriteUSB_Device(p, 14);
                }
            }
        }

        public bool GetResponse_AttachQuery()
        {

            int len = 14;
            RecvBuff = new byte[len];

            DateTime endTime = DateTime.Now.AddMilliseconds(20);

            while (DateTime.Now < endTime)
            {
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
                    break;
                }
            }

            if (CheckResponse(RecvBuff))
            {
                CommonConstants.LadderMonitorTagInfo objTagInfo;//= new CommonConstants.LadderMonitorTagInfo();
                if (NoOfByteRead > 0)
                {
                    byte[] Tag_Data = new byte[2];
                    int[] Tag_value = new int[3];

                    int cnt = 0;
                    int m = 0;

                    unsafe
                    {
                        bool blFlag = false;
                        byte[] ReadResponse = new byte[2];
                        ReadResponse[0] = 0x01;
                        ReadResponse[1] = 0x01;
                        fixed (byte* p = ReadResponse)
                        {
                            blFlag = obj.WriteUSB_Device(p, 2);
                        }
                    }

                    for (int i = 6; i < len - 2; i++)
                    {
                        Tag_Data[++cnt] = RecvBuff[i++];
                        Tag_Data[--cnt] = RecvBuff[i];
                        Tag_value[m] = CommonConstants.MAKEWORD(Tag_Data);
                        objTagInfo = (CommonConstants.LadderMonitorTagInfo)m_listTagAddress[DataCount];
                        // objTagInfo.TagAddress = ((CommonConstants.LadderMonitorTagInfo)m_listTagAddress[DataCount]).TagAddress;
                        objTagInfo.value = Tag_value[m];
                        m_listTagAddress[DataCount++] = objTagInfo;

                        m++;
                    }

                    return true;
                }
            }
            else
            {
                unsafe
                {
                    bool blFlag = false;
                    byte[] ReadResponse = new byte[2];
                    ReadResponse[0] = 0xFF;
                    ReadResponse[1] = 0xFF;
                    fixed (byte* p = ReadResponse)
                    {
                        blFlag = obj.WriteUSB_Device(p, 2);
                    }
                }
                return false;
            }

            return false;
        }

        public bool ReadUSBData()
        {
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.USB))
            {
                b_USBReadCompleteFlag = false;
                for (int i = 0; i < m_i_RetryCount; i++)
                {
                    _USBReadThread = new Thread(new ThreadStart(ReadFunction_GetResponse));
                    _USBReadThread.Start();

                    for (int j = 0; j < CommonConstants.SleepMultiplier; j++)
                    {
                        if (b_USBReadCompleteFlag)
                            break;
                        Thread.Sleep(CommonConstants.SleepCount);
                    }
                    if (b_USBReadCompleteFlag)
                        break;
                    else
                    {
                        if (ClassList.CommonConstants.IsProductUSBPortReset(ClassList.CommonConstants.ProductDataInfo.iProductID))
                            USB_PORT_RESET();//USB_PORT_RESET
                    }
                }

                if (!b_USBReadCompleteFlag)
                    return false;
                else
                    return true;
            }
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Serial) ||
                ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Ethernet))//Monitoring Port- AD
            {
                for (int i = 0; i < m_i_RetryCount; i++)
                {
                    SendWriteQuery();
                    Thread.Sleep(50);
                    if (GetResponse_WriteQuery())
                        return true;
                }
            }
            return false;
        }

        public bool ReadUSBData_Multiple()
        {
            b_USBReadCompleteFlag = false;
            for (int i = 0; i < m_i_RetryCount; i++)
            {
                _USBReadThread = new Thread(new ThreadStart(ReadFunction_GetResponseMultiple));
                _USBReadThread.Start();

                for (int j = 0; j < CommonConstants.SleepMultiplier; j++)
                {
                    if (b_USBReadCompleteFlag)
                        break;
                    Thread.Sleep(CommonConstants.SleepCount);
                }
                if (b_USBReadCompleteFlag)
                    break;
            }

            if (!b_USBReadCompleteFlag)
                return false;
            else
                return true;
        }

        public bool ReadUSBData(ref byte[] m_receBuff, int pSize)
        {
            int m_Size = pSize;
            byte[] s_receBuff = new byte[2];
            s_receBuff = m_receBuff;
            b_USBReadCompleteFlag = false;
            for (int i = 0; i < m_i_RetryCount; i++)
            {
                _USBReadThread = new Thread(delegate()
                {
                    ReadFunction_ReceiveFrame(m_Size, ref s_receBuff);
                });
                _USBReadThread.Start();

                for (int j = 0; j < CommonConstants.SleepMultiplier; j++)
                {
                    if (b_USBReadCompleteFlag)
                        break;
                    Thread.Sleep(CommonConstants.SleepCount);
                }
                if (b_USBReadCompleteFlag)
                    break;
            }

            if (!b_USBReadCompleteFlag)
                return false;
            else
                return true;
        }

        public void ReadData_Event()
        {
            //obj.ReadDataReceived_Event();
        }

        private void ReadFunction_ReceiveFrame(int pSize, ref byte[] s_receBuff)
        {
            if (ReceiveFrame(ref s_receBuff, 1))
                b_USBReadCompleteFlag = true;
            else
                b_USBReadCompleteFlag = false;
        }

        private void ReadFunction_GetResponse()
        {
            SendWriteQuery();
            if (GetResponse_WriteQuery())
                b_USBReadCompleteFlag = true;
            else
                b_USBReadCompleteFlag = false;
        }

        private void ReadFunction_GetResponseMultiple()
        {
            SendConfigureQuery();
            if (GetResponse_MultipleRegisterWriteQuery())
                b_USBReadCompleteFlag = true;
            else
                b_USBReadCompleteFlag = false;
        }

        public void CalculateCRC(ref byte[] CRC, byte[] pDataBuff)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (pDataBuff.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ pDataBuff[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }

            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }

        public bool CheckResponse(byte[] response)
        {
            byte[] CRC = new byte[2];
            CalculateCRC(ref CRC, response);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
            {
                //MessageBox.Show("CRC Mismatch");
                return false;
            }
        }

        public string GetRegisterAddress_Expansion(string pStr, string m_str_opcode, ref string slotNumber, ref string regNumber)
        {
            int ActualAddr = 0;
            int expAddr = 0;
            int expNumber = 0;

            string expstr = pStr[0].ToString() + pStr[1].ToString();
            string strAddr = pStr[2].ToString() + pStr[3].ToString();

            /*expAddr = Convert.ToInt32(strAddr);
            expNumber = Convert.ToInt32(expstr);

            if (expNumber == 0)
                expNumber = expAddr;*/


            slotNumber = expstr;
            regNumber = strAddr;
            return (expstr);
        }

        public string GetBitAddress_Expansion(string pStr)
        {
            int ActualAddr = 0;
            int expAddr = 0;
            int expNumber = 0;
            string expstr = pStr[0].ToString() + pStr[1].ToString();
            string strAddr = pStr[2].ToString() + pStr[3].ToString() + pStr[4].ToString();
            expAddr = Convert.ToInt32(strAddr);
            expNumber = Convert.ToInt32(expstr);

            //ActualAddr = (expNumber * 16) + expAddr;
            ActualAddr = expNumber;
            return (ActualAddr.ToString());
        }

        public byte[] GetAddress(string opr)
        {
            int qot = 0;
            int op_addr = 0;
            int remainder = 0;
            string addr = string.Empty;
            string prefix = string.Empty;
            byte[] temparr = new byte[2];
            string Slot_Number = string.Empty;
            string Reg_Number = string.Empty;
            int raw_variable = 0;

            InitStartingAdressVariables();

            prefix = GetPrefix(opr);
            addr = opr.Remove(0, prefix.Length);

            if (prefix == XW_Prefix || prefix == YW_Prefix || prefix == MW_Prefix)
                addr = GetRegisterAddress_Expansion(addr, prefix, ref Slot_Number, ref Reg_Number);

            op_addr = int.Parse(addr);

            switch (prefix)
            {
                case R_Prefix:
                case RW_Prefix:
                    m_Starting_Addr = B_InternalCoil_Start_Addr;
                    break;
                case X_Prefix:
                case XW_Prefix:
                    m_Starting_Addr = X_PhysicalInput_Start_Addr;
                    break;
                case Y_Prefix:
                case YW_Prefix:
                    m_Starting_Addr = Y_PhysicalOutput_Start_Addr;
                    break;
                case S_Prefix:
                    m_Starting_Addr = S_SystemDevice_Start_Addr;
                    break;
                case SW_Prefix:
                    m_Starting_Addr = S_SystemRegister_Start_Addr;
                    break;
                case M_Prefix:
                case MW_Prefix:
                    m_Starting_Addr = M_ConfigRegister_Start_Addr;
                    break;
                case C_Prefix:
                    m_Starting_Addr = C_CounterRegister_Start_Addr;
                    break;
                case CDevice_Prefix:
                    m_Starting_Addr = C_CounterDevice_Start_Addr;
                    break;
                case T_Prefix:
                    m_Starting_Addr = T_TimerRegister_Start_Addr;
                    break;
                case TDevice_Prefix:
                    m_Starting_Addr = T_TimerDevice_Start_Addr;
                    break;
                case D_Prefix:
                    m_Starting_Addr = D_DataRegister_Start_Addr;
                    break;
                case I_Prefix:
                    m_Starting_Addr = I_Index_Start_Addr;
                    break;
                case J_Prefix:
                    m_Starting_Addr = J_Index_Start_Addr;
                    break;
                case K_Prefix:
                    m_Starting_Addr = K_Index_Start_Addr;
                    break;
                case Retentive_Prefix:
                    m_Starting_Addr = R_Retentive_Start_Addr;
                    break;
                default:
                    break;
            }

            if (prefix != X_Prefix && prefix != Y_Prefix && prefix != R_Prefix && prefix != M_Prefix)
            {
                if (prefix == XW_Prefix)
                {

                    int ram_offset = 0;
                    ClassList.CommonConstants.PlcModuleInfo objModuleInfo;

                    if (op_addr == 0)
                    {
                        op_addr = (m_Starting_Addr + _ObjModuleHeaderInfo.intRAM_OffsetXW) + (2 * Convert.ToInt32(Reg_Number));
                    }
                    else
                    {
                        for (int i = 1; i <= _ObjModuleHeaderInfo.intNoModules; i++)
                        {
                            objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                            if (objModuleInfo.intModuleType > 0)
                            {
                                if (i == op_addr)
                                {
                                    objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                                    op_addr = (m_Starting_Addr + objModuleInfo.intRAM_OffsetXW) + (2 * Convert.ToInt32(Reg_Number));
                                    break;
                                }

                            }
                        }
                    }
                }
                else if (prefix == YW_Prefix)
                {

                    int ram_offset = 0;
                    ClassList.CommonConstants.PlcModuleInfo objModuleInfo;

                    if (op_addr == 0)
                    {
                        op_addr = (m_Starting_Addr + _ObjModuleHeaderInfo.intRAM_OffsetYW) + (2 * Convert.ToInt32(Reg_Number));
                    }
                    else
                    {
                        for (int i = 1; i <= _ObjModuleHeaderInfo.intNoModules; i++)
                        {
                            objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                            if (objModuleInfo.intModuleType > 0)
                            {
                                if (i == op_addr)
                                {
                                    objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                                    op_addr = (m_Starting_Addr + objModuleInfo.intRAM_OffsetYW) + (2 * Convert.ToInt32(Reg_Number));
                                    break;
                                }

                            }
                        }
                    }
                }
                else if (prefix == MW_Prefix)
                {

                    int ram_offset = 0;
                    ClassList.CommonConstants.PlcModuleInfo objModuleInfo;

                    if (op_addr == 0)
                    {
                        op_addr = (m_Starting_Addr + _ObjModuleHeaderInfo.intRAM_OffsetMW) + (2 * Convert.ToInt32(Reg_Number));
                    }
                    else
                    {
                        for (int i = 1; i <= _ObjModuleHeaderInfo.intNoModules; i++)
                        {
                            objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                            if (objModuleInfo.intModuleType > 0)
                            {
                                if (i == op_addr)
                                {
                                    objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                                    op_addr = (m_Starting_Addr + objModuleInfo.intRAM_OffsetMW) + (2 * Convert.ToInt32(Reg_Number));
                                    break;
                                }

                            }
                        }
                    }
                }
                else
                {
                    op_addr = m_Starting_Addr + op_addr * 2;
                }
            }
            else
            {
                qot = op_addr / 16;
                Bit_Position = op_addr % 16;
                op_addr = m_Starting_Addr + qot * 2;
            }

            raw_variable = (op_addr) & 255;
            temparr[0] = Convert.ToByte(raw_variable);
            raw_variable = op_addr;
            raw_variable = raw_variable >> 8;
            temparr[1] = Convert.ToByte((raw_variable & 255));

            return temparr;
        }

        public byte[] GetAddress_Device(string opr)
        {
            int qot = 0;
            int op_addr = 0;
            int remainder = 0;
            string addr = string.Empty;
            string prefix = string.Empty;
            byte[] temparr = new byte[2];

            string Slot_Number = string.Empty;
            string Reg_Number = string.Empty;
            string BitNumber = string.Empty;
            int RegisterNumber = 0;

            InitStartingAdressVariables();

            prefix = GetPrefix(opr);
            addr = opr.Remove(0, prefix.Length);
            BitNumber = addr;

            if (prefix == XW_Prefix || prefix == YW_Prefix || prefix == MW_Prefix)
                addr = GetRegisterAddress_Expansion(addr, prefix, ref Slot_Number, ref Reg_Number);
            else if (prefix == X_Prefix || prefix == Y_Prefix || prefix == M_Prefix)
                addr = GetBitAddress_Expansion(addr);

            op_addr = int.Parse(addr);

            switch (prefix)
            {
                case R_Prefix:
                case RW_Prefix:
                    m_Starting_Addr = B_InternalCoil_Start_Addr;
                    break;
                case X_Prefix:
                case XW_Prefix:
                    m_Starting_Addr = X_PhysicalInput_Start_Addr;
                    break;
                case Y_Prefix:
                case YW_Prefix:
                    m_Starting_Addr = Y_PhysicalOutput_Start_Addr;
                    break;
                case S_Prefix:
                    m_Starting_Addr = S_SystemDevice_Start_Addr;
                    break;
                case SW_Prefix:
                    m_Starting_Addr = S_SystemRegister_Start_Addr;
                    break;
                case M_Prefix:
                case MW_Prefix:
                    m_Starting_Addr = M_ConfigRegister_Start_Addr;
                    break;
                case C_Prefix:
                    m_Starting_Addr = C_CounterRegister_Start_Addr;
                    break;
                case CDevice_Prefix:
                    m_Starting_Addr = C_CounterDevice_Start_Addr;
                    break;
                case T_Prefix:
                    m_Starting_Addr = T_TimerRegister_Start_Addr;
                    break;
                case TDevice_Prefix:
                    m_Starting_Addr = T_TimerDevice_Start_Addr;
                    break;
                case D_Prefix:
                    m_Starting_Addr = D_DataRegister_Start_Addr;
                    break;
                case I_Prefix:
                    m_Starting_Addr = I_Index_Start_Addr;
                    break;
                case J_Prefix:
                    m_Starting_Addr = J_Index_Start_Addr;
                    break;
                case K_Prefix:
                    m_Starting_Addr = K_Index_Start_Addr;
                    break;
                default:
                    break;
            }

            if (prefix == X_Prefix)
            {
                ClassList.CommonConstants.PlcModuleInfo objModuleInfo;
                BitNumber = BitNumber[2].ToString() + BitNumber[3].ToString() + BitNumber[4].ToString();
                if (op_addr == 0)
                {
                    op_addr = m_Starting_Addr + _ObjModuleHeaderInfo.intRAM_OffsetXW;
                }
                else
                {
                    for (int i = 1; i <= _ObjModuleHeaderInfo.intNoModules; i++)
                    {
                        objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                        if (objModuleInfo.intModuleType > 0)
                        {
                            if (i == op_addr)
                            {
                                objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                                op_addr = m_Starting_Addr + objModuleInfo.intRAM_OffsetXW;
                                break;
                            }
                        }

                    }
                }
                Bit_Position = Convert.ToInt32(BitNumber) % 16;
                RegisterNumber = Convert.ToInt32(BitNumber) / 16;
                op_addr = op_addr + (2 * RegisterNumber);
            }
            else if (prefix == Y_Prefix)
            {
                ClassList.CommonConstants.PlcModuleInfo objModuleInfo;
                BitNumber = BitNumber[2].ToString() + BitNumber[3].ToString() + BitNumber[4].ToString();
                if (op_addr == 0)
                {
                    op_addr = m_Starting_Addr + _ObjModuleHeaderInfo.intRAM_OffsetYW;
                }
                else
                {
                    for (int i = 1; i <= _ObjModuleHeaderInfo.intNoModules; i++)
                    {
                        objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                        if (objModuleInfo.intModuleType > 0)
                        {
                            if (i == op_addr)
                            {
                                objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                                op_addr = m_Starting_Addr + objModuleInfo.intRAM_OffsetYW;
                                break;
                            }
                        }

                    }
                }
                Bit_Position = Convert.ToInt32(BitNumber) % 16;
                RegisterNumber = Convert.ToInt32(BitNumber) / 16;
                op_addr = op_addr + (2 * RegisterNumber);
            }
            else if (prefix == M_Prefix)
            {
                ClassList.CommonConstants.PlcModuleInfo objModuleInfo;
                BitNumber = BitNumber[2].ToString() + BitNumber[3].ToString() + BitNumber[4].ToString();
                if (op_addr == 0)
                {
                    op_addr = m_Starting_Addr + _ObjModuleHeaderInfo.intRAM_OffsetMW;
                }
                else
                {
                    for (int i = 1; i <= _ObjModuleHeaderInfo.intNoModules; i++)
                    {
                        objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                        if (objModuleInfo.intModuleType > 0)
                        {
                            if (i == op_addr)
                            {
                                objModuleInfo = (ClassList.CommonConstants.PlcModuleInfo)_listOfSlotsIoAllocation[i - 1];
                                op_addr = m_Starting_Addr + objModuleInfo.intRAM_OffsetMW;
                                break;
                            }
                        }

                    }
                }

                Bit_Position = Convert.ToInt32(BitNumber) % 16;
                RegisterNumber = Convert.ToInt32(BitNumber) / 16;
                op_addr = op_addr + (2 * RegisterNumber);
            }
            else
            {
                qot = op_addr / 16;
                Bit_Position = op_addr % 16;
                op_addr = m_Starting_Addr + qot * 2;
            }


            int raw_variable;
            raw_variable = (op_addr) & 255;
            temparr[0] = Convert.ToByte(raw_variable);
            raw_variable = op_addr;
            raw_variable = raw_variable >> 8;
            temparr[1] = Convert.ToByte((raw_variable & 255));

            return temparr;
        }

        public string GetPrefix(string pStr)
        {
            string prefix = string.Empty;

            for (int i = 0; i <= pStr.Length; i++)
            {
                if ((pStr[i] >= 65 && pStr[i] <= 90) || pStr[i] == 46)
                    prefix = prefix.Insert(i, pStr[i].ToString());
                else
                    break;
            }

            return prefix;
        }
        public void InitIOAllocationListInfo(ClassList.CommonConstants.PlcModuleHeaderInfo objHeaderInfo, ArrayList objListIOAllocation)
        {
            _ObjModuleHeaderInfo = objHeaderInfo;
            _listOfSlotsIoAllocation = objListIOAllocation;
        }
        public void InitDataMonitor_BlockListInfo(ArrayList objListDataMonitorBlockList)
        {
            m_ListofBlocks = objListDataMonitorBlockList;
        }
        public int Calculate_DataMonitorTotalFrames()
        {
            Total_Frames = 0;
            //DmBlockInfo objDMBlockInfo;
            
            //int MAXBIT = 52;
            //int MAXREGISTER = 26;
            //for (int i = 0; i < m_ListofBlocks.Count; i++)
            //{
            //    objDMBlockInfo = (DmBlockInfo)m_ListofBlocks[i];

            //    if (objDMBlockInfo.strType == "Bit")
            //    {
            //        if (objDMBlockInfo.TagList.Count % MAXBIT == 0)
            //            Total_Frames = Total_Frames + (objDMBlockInfo.TagList.Count / MAXBIT);
            //        else
            //            Total_Frames = Total_Frames + (objDMBlockInfo.TagList.Count / MAXBIT) + 1;
            //    }
            //    else
            //    {
            //        if (objDMBlockInfo.TagList.Count % MAXREGISTER == 0)
            //            Total_Frames = Total_Frames + (objDMBlockInfo.TagList.Count / MAXREGISTER);
            //        else
            //            Total_Frames = Total_Frames + (objDMBlockInfo.TagList.Count / MAXREGISTER) + 1;
            //    }
            //}
            return Total_Frames;
        }

        public bool Initial_ProtocolForDataMonitor()
        {
            byte initialByte = 0x80;
            int iNumberofFrames = 0;
            byte[] m_sendBuff = new byte[4];
            byte[] m_receBuff = new byte[2];
            m_sendBuff[0] = initialByte;

            WriteFrame(1, m_sendBuff);
            //ReceiveFrame(ref m_receBuff, 1);
            if (ReadUSBData(ref m_receBuff, 1) == false)
                return false;

            if (_serialReceivedByte == initialByte + 2)
            {
                m_sendBuff.SetValue((Byte)0x52, 0);
                m_sendBuff.SetValue((Byte)0x45, 1);
                m_sendBuff.SetValue((Byte)0x50, 2);
                m_sendBuff.SetValue((Byte)0x4c, 3);

                WriteFrame(4, m_sendBuff);
                //ReceiveFrame(ref m_receBuff, 1);
                if (ReadUSBData(ref m_receBuff, 1) == false)
                    return false;
            }

            if (_serialReceivedByte == initialByte + 1)
            {
                m_sendBuff = CommonConstants.GetHalfWord(Total_Frames);
                WriteFrame(2, m_sendBuff);
                //ReceiveFrame(ref m_receBuff, 2);
                if (ReadUSBData(ref m_receBuff, 2) == false)
                    return false;
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

        //FP_CODE Pravin Data Download
        public byte[] MakeFrame_MultipleRegisterWrite(int FrameNumber, int BlockNumber, int bytesRead, int NumofReg, int rCnt)
        {
            //byte[] temp_arr = new byte[2];
            //byte[] byte_arr = new byte[4];
            //byte[] CRC = new byte[2];
            //DmBlockInfo objDmBlockInfo;
            //DmTagInfo objDmTagInfo;

            //double Fourbyte_NextAddressValue = 0;
            //int startAddrCnt = 0;
            //int maxCount = 0;
            //int bufflen = 0;
            //int Size = bytesRead + 9;
            //Length_Buffer = Size;
            //SendBuff = new byte[Size];
            //objDmBlockInfo = (DmBlockInfo)m_ListofBlocks[BlockNumber];

            //SendBuff[bufflen++] = 0x01;
            //SendBuff[bufflen++] = 0x10;

            ////Start Address
            //objDmTagInfo = (DmTagInfo)objDmBlockInfo.TagList[rCnt];
            //temp_arr = GetAddress(objDmTagInfo.strTagAddress);
            //SendBuff[bufflen++] = temp_arr[1];
            //SendBuff[bufflen++] = temp_arr[0];

            //temp_arr = CommonConstants.GetHalfWord(NumofReg);
            //SendBuff[bufflen++] = temp_arr[1];
            //SendBuff[bufflen++] = temp_arr[0];

            //SendBuff[bufflen++] = (byte)(NumofReg * 2);

            //for (int i = 0; i < NumofReg; i++)
            //{
            //    objDmTagInfo = (DmTagInfo)objDmBlockInfo.TagList[rCnt++];

            //    if (objDmBlockInfo.intDataType == 3)//For Float
            //    {

            //        float _fTemp = new float();
            //        byte[] btTempData = new byte[Marshal.SizeOf(_fTemp)];

            //        btTempData = ClassList.CommonConstants.RawSerialize((float)objDmTagInfo.doubleTagValue);

            //        SendBuff[bufflen++] = btTempData[1];
            //        SendBuff[bufflen++] = btTempData[0];

            //        SendBuff[bufflen++] = btTempData[3];
            //        SendBuff[bufflen++] = btTempData[2];
            //        i++;
            //        rCnt++;

            //    }
            //    else
            //    {
            //        if (objDmBlockInfo.intDataSize == 2)
            //        {
            //            if (objDmBlockInfo.intDataType == 2)
            //                CommonConstants.BREAKINT(byte_arr, (int)objDmTagInfo.doubleTagValue);
            //            else
            //                CommonConstants.BREAKUINT(byte_arr, (uint)objDmTagInfo.doubleTagValue);

            //            SendBuff[bufflen++] = byte_arr[1];
            //            SendBuff[bufflen++] = byte_arr[0];

            //            SendBuff[bufflen++] = byte_arr[3];
            //            SendBuff[bufflen++] = byte_arr[2];

            //            i++;
            //            rCnt++;
            //        }
            //        else
            //        {
            //            if (objDmBlockInfo.intDataType == 2)
            //                temp_arr = CommonConstants.GetHalfWord((int)objDmTagInfo.doubleTagValue);
            //            else
            //                CommonConstants.BREAKWORD(temp_arr, (uint)objDmTagInfo.doubleTagValue);

            //            SendBuff[bufflen++] = temp_arr[1];
            //            SendBuff[bufflen++] = temp_arr[0];
            //        }
            //    }


            //}

            //CalculateCRC(ref CRC, SendBuff);
            //SendBuff[bufflen++] = CRC[0];
            //SendBuff[bufflen++] = CRC[1];

            return SendBuff;
        }
        //End

        public byte[] MakeFrame_MultipleRegisterWrite_Float(int FrameNumber, int BlockNumber, int bytesRead, int NumofReg, int rCnt)
        {
            byte[] temp_arr = new byte[2];
            byte[] byte_arr = new byte[4];
            byte[] CRC = new byte[2];
            float _fTemp = new float();
            byte[] btTempData = new byte[Marshal.SizeOf(_fTemp)];
            //DmBlockInfo objDmBlockInfo;
            //DmTagInfo objDmTagInfo;

            //double Fourbyte_NextAddressValue = 0;
            //int startAddrCnt = 0;
            //int maxCount = 0;
            //int bufflen = 0;
            //int Size = bytesRead + 9;
            //Length_Buffer = Size;
            //SendBuff = new byte[Size];
            //objDmBlockInfo = (DmBlockInfo)m_ListofBlocks[BlockNumber];

            //SendBuff[bufflen++] = 0x01;
            //SendBuff[bufflen++] = 0x10;

            ////Start Address
            //objDmTagInfo = (DmTagInfo)objDmBlockInfo.TagList[rCnt];
            //temp_arr = GetAddress(objDmTagInfo.strTagAddress);
            //SendBuff[bufflen++] = temp_arr[1];
            //SendBuff[bufflen++] = temp_arr[0];

            //temp_arr = CommonConstants.GetHalfWord(NumofReg);
            //SendBuff[bufflen++] = temp_arr[1];
            //SendBuff[bufflen++] = temp_arr[0];

            //SendBuff[bufflen++] = (byte)(NumofReg * 2);

            //for (int i = 0; i < NumofReg; i++)
            //{
            //    objDmTagInfo = (DmTagInfo)objDmBlockInfo.TagList[rCnt++];
            //    btTempData = ClassList.CommonConstants.RawSerialize((float)objDmTagInfo.doubleTagValue);

            //    SendBuff[bufflen++] = btTempData[1];
            //    SendBuff[bufflen++] = btTempData[0];

            //    SendBuff[bufflen++] = btTempData[3];
            //    SendBuff[bufflen++] = btTempData[2];

            //    i++;
            //    rCnt++;

            //}

            //CalculateCRC(ref CRC, SendBuff);
            //SendBuff[bufflen++] = CRC[0];
            //SendBuff[bufflen++] = CRC[1];

            return SendBuff;
        }

        //FP_CODE Pravin Data Download
        public byte[] MakeFrame_MultipleCoilWrite(int FrameNumber, int BlockNumber, int bytesRead, int NumofReg, int rCnt)
        {
            byte[] temp_arr = new byte[2];
            byte[] byte_arr = new byte[4];
            byte[] CRC = new byte[2];
            //DmBlockInfo objDmBlockInfo;
            //DmTagInfo objDmTagInfo;

            //double Fourbyte_NextAddressValue = 0;
            //int startAddrCnt = 0;
            //int maxCount = 0;
            //int bufflen = 0;
            //int Size = bytesRead + 10;
            //Length_Buffer = Size;
            //SendBuff = new byte[Size];
            //objDmBlockInfo = (DmBlockInfo)m_ListofBlocks[BlockNumber];

            //SendBuff[bufflen++] = 0x01;
            //SendBuff[bufflen++] = 0x0F;

            ////Start Address
            //objDmTagInfo = (DmTagInfo)objDmBlockInfo.TagList[rCnt];
            //temp_arr = GetAddress_Device(objDmTagInfo.strTagAddress);
            //SendBuff[bufflen++] = temp_arr[1];
            //SendBuff[bufflen++] = temp_arr[0];

            //temp_arr = CommonConstants.GetHalfWord(NumofReg);
            //SendBuff[bufflen++] = temp_arr[1];
            //SendBuff[bufflen++] = temp_arr[0];

            //SendBuff[bufflen++] = (byte)NumofReg;
            //SendBuff[bufflen++] = System.Convert.ToByte(Bit_Position);

            //for (int i = 0; i < NumofReg; i++)
            //{
            //    objDmTagInfo = (DmTagInfo)objDmBlockInfo.TagList[rCnt++];
            //    SendBuff[bufflen++] = (byte)objDmTagInfo.doubleTagValue;
            //}

            //CalculateCRC(ref CRC, SendBuff);
            //SendBuff[bufflen++] = CRC[0];
            //SendBuff[bufflen++] = CRC[1];

            return SendBuff;
        }
        //End

        public bool GetResponse_MultipleRegisterWriteQuery()
        {

            int len = 8;
            RecvBuff = new byte[len];


            DateTime endTime = DateTime.Now.AddMilliseconds(20);

            while (DateTime.Now < endTime)
            {
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
            }
            return false;
        }


        public void Close()
        {
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.USB))
                obj.USB_CLose();
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Serial))
            {
                try
                {
                    RecvBuff = null;
                    if (!_serialPort.IsOpen)
                        return;
                    _serialPort.Close();
                    _deviceIsConnected = false;
                }
                catch (SystemException ex)
                {
                    ex.GetType();
                }
            }

            //Monitoring Port- AD
            if (ClassList.CommonConstants.ON_LINE_COMMUNICATION == Convert.ToInt16(ClassList.CommunicationType.Ethernet))
                _ethernetSocket.Close();
            //End
            // _USBCloseThread.Start();
        }

        public bool IsSerialPort_Open()
        {
            if (_serialPort.IsOpen)
                return true;
            else
                return false;
        }


        public void CloseUSBDevice()
        {
            obj.USB_CLose();
            bConnect = false;
        }

        //USB_PORT_RESET
        public void USB_PORT_RESET()
        {
            obj.USB_CancelPendingIO();
        }
        //End

        public void InitStartingAdressVariables()
        {
            if (CommonConstants.IsProductCompatibleWith4020(CommonConstants.ProductDataInfo.iProductID) ||
                CommonConstants.IsProductCompatibleWith4030(CommonConstants.ProductDataInfo.iProductID) ||
                CommonConstants.IsProductFL005MicroPLCBase(CommonConstants.ProductDataInfo.iProductID) || //FL005-MicroPLC Base Module Series Vijay
                CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_GSM900 ||//GWY_900_SanjayY
                CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_GSM901 ||//GWY_900_SanjayY //GWY-901 SP
                CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_GSM910)//GWY_910_Suyash //GWY_900_SanjayY //GWY-901 SP
            {
                T_Temporary_Start_Addr = 0; //for Temporary Register
                X_PhysicalInput_Start_Addr = 2; //for Physical Input
                Y_PhysicalOutput_Start_Addr = 802; //for Physical Output
                B_InternalCoil_Start_Addr = 1602; //for Auxiliary registers
                M_ConfigRegister_Start_Addr = 2114; //for Configuration Register
                T_TimerRegister_Start_Addr = 5314; //for Timer Register
                C_CounterRegister_Start_Addr = 5826; //for Counter Register
                D_DataRegister_Start_Addr = 6026; //for Data Register        
                S_SystemRegister_Start_Addr = 14218; //for System Register
                S_SystemDevice_Start_Addr = 14730; //for System Register        
                T_TimerDevice_Start_Addr = 14744; //for Timer Devices
                C_CounterDevice_Start_Addr = 14776; //for Counter Devices

                I_Index_Start_Addr = 14792; //for I 
                J_Index_Start_Addr = 14794; //for J
                K_Index_Start_Addr = 14796; //for K

                R_Retentive_Start_Addr = 20000; //for Retentive registers

                if (CommonConstants.IsProductFL005ExpandablePLCSeries(CommonConstants.ProductDataInfo.iProductID))
                {
                    D_DataRegister_Start_Addr = 6338;
                    S_SystemRegister_Start_Addr = 14530;
                    S_SystemDevice_Start_Addr = 15042;
                    T_TimerDevice_Start_Addr = 15056;
                    C_CounterDevice_Start_Addr = 15088;
                    I_Index_Start_Addr = 15122; //for I 
                    J_Index_Start_Addr = 15124; //for J
                    K_Index_Start_Addr = 15126; //for K
                    R_Retentive_Start_Addr = 20000;
                }
            }
            //Lohia_710_change
            else if (CommonConstants.ProductDataInfo.iProductID == ClassList.CommonConstants.PRODUCT_PRIZM_710_S0)
            {
                int offset = 200;
                D_DataRegister_Start_Addr = 6338 + offset; //for Data Register        
                S_SystemRegister_Start_Addr = 14530 + offset; //for System Register
                S_SystemDevice_Start_Addr = 15042 + offset; //for System Register        
                T_TimerDevice_Start_Addr = 15056 + offset; //for Timer Devices
                C_CounterDevice_Start_Addr = 15088 + offset; //for Counter Devices
                I_Index_Start_Addr = 15122 + offset; //for I 
                J_Index_Start_Addr = 15124 + offset; //for J
                K_Index_Start_Addr = 15126 + offset; //for K
                R_Retentive_Start_Addr = 20000 + offset; //for Retentive registers

            }
        }

        #endregion
    }
}
