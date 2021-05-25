using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using ClassList;
using System.Collections;

namespace RTC_Communication_Utility
{
    public class ModbusRTU
    {
        SerialPort _port = null;
        byte[] _SendBuff;// = new byte[17];
        ArrayList listSendBuff = null;
        byte[] _RecieveBuff;
        char[] _charBuff;
        public string m_portName;
        public delegate void GetLRCResult();

        public event GetLRCResult _GetLRCResultResult;

        public ModbusRTU()
        {
            _SendBuff = new byte[17];
            listSendBuff = new ArrayList();
        }

        public bool OpenCOMPort(string portName, int baudrate, string parity, int stopbit, int bitsLength)
        {
            try
            {
                if (_port == null)
                {
                    _port = new SerialPort(portName);

                    _port.BaudRate = baudrate;

                    switch (parity)
                    {
                        case "Even":
                            _port.Parity = Parity.Even;
                            break;
                        case "Odd":
                            _port.Parity = Parity.Odd;
                            break;
                        case "None":
                        default:
                            _port.Parity = Parity.None;
                            break;
                    }

                    switch (stopbit.ToString())
                    {
                        case "1":
                            _port.StopBits = StopBits.One;
                            break;
                        case "1.5":
                            _port.StopBits = StopBits.OnePointFive;
                            break;
                        case "2":
                            _port.StopBits = StopBits.Two;
                            break;
                        default:
                            _port.StopBits = StopBits.None;
                            break;
                    }

                    _port.DataBits = bitsLength;

                    _port.ReadTimeout = 1000;

                    m_portName = portName;

                    try
                    {
                        if (!_port.IsOpen)
                        {
                            _port.Open();
                            return true;
                        }

                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        LogWriter.WriteToFile("ModbusRTU: 1) ", ex.StackTrace, "DTC_ErrorLog");
                        return false;
                    }
                }
                else
                {
                    if (!_port.IsOpen)
                    {
                        _port.Open();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("ModbusRTU: 2)", ex.StackTrace, "DTC_ErrorLog");
                return false;
            }
            return false;
        }

        public bool IsSerialPortOpen()
        {
            if (_port != null)
            {
                if (_port.IsOpen)
                    return true;
            }
            return false;
        }

        public void Port_Close()
        {
            if (_port != null)
            {
                if (_port.IsOpen)
                {
                    _port.Close();
                }
            }
        }

        public char[] Create1(string unitAddress, string commandType, string regAddress, string wordCount)
        {
            listSendBuff.Clear();
            ArrayList list = new ArrayList();

            string protocolss = SetValues.Set_CommunicationProtocol;
            
            if (!string.IsNullOrEmpty(protocolss))
            {
                if (protocolss.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                {
                    #region ascii
                    list.Add(':');

                    #endregion
                }
            }
            char[] untaddr = unitAddress.ToCharArray();
            list.Add(untaddr[0]);
            list.Add(untaddr[1]);

            char[] cmdType = commandType.ToCharArray();
            list.Add(cmdType[0]);
            list.Add(cmdType[1]);

            char[] Fnctnaddr = regAddress.ToCharArray();
            foreach (char item in Fnctnaddr)
            {
                list.Add(item);
            }

            char[] WrdCntaddr = wordCount.ToCharArray();
            foreach (char item in WrdCntaddr)
            {
                list.Add(item);
            }
            if (commandType == "10")
            {
                if (!string.IsNullOrEmpty(SetValues.Set_Words1))
                {
                    char[] words = SetValues.Set_Words1.ToCharArray();
                    foreach (char item in words)
                    {
                        list.Add(item);
                    }
                }
                if (!string.IsNullOrEmpty(SetValues.Set_Words))
                {
                    char[] words = SetValues.Set_Words.ToCharArray();
                    foreach (char item in words)
                    {
                        list.Add(item);
                    }
                }
            }
            return (char[])list.ToArray(typeof(char));
        }

        #region Serialization Methods
        public static object RawDeserialize(byte[] rawdatas, Type anytype)
        {
            int rawsize = Marshal.SizeOf(anytype);
            if (rawsize > rawdatas.Length)
                return null;
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawdatas, 0, buffer, rawsize);
            object retobj = Marshal.PtrToStructure(buffer, anytype);
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }

        public static byte[] RawSerialize(object anything)
        {
            int rawsize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte[] rawdatas = new byte[rawsize];
            Marshal.Copy(buffer, rawdatas, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            return rawdatas;
        }
        #endregion

        
        public byte[] Read_AscDataRegisterCombinations(int Reg_No)
        {
            _RecieveBuff = new byte[15];

            _port.BaudRate = Convert.ToInt32(SetValues.Set_Baudrate);

            switch (SetValues.Set_parity)
            {
                case "Even":
                    _port.Parity = Parity.Even;
                    break;
                case "Odd":
                    _port.Parity = Parity.Odd;
                    break;
                case "None":
                default:
                    _port.Parity = Parity.None;
                    break;
            }

            switch (SetValues.Set_StopBits)
            {
                case "1":
                    _port.StopBits = StopBits.One;
                    break;
                case "2":
                    _port.StopBits = StopBits.Two;
                    break;
                default:
                    _port.StopBits = StopBits.None;
                    break;
            }

            _port.DataBits = SetValues.Set_BitsLength;

            _port.ReadTimeout = 500;

            try
            {
                _port.Write(_SendBuff, 0, _SendBuff.Length);

                System.Threading.Thread.Sleep(300);

                _port.Read(_RecieveBuff, 0, _RecieveBuff.Length);
            }
            catch (Exception err)
            {
                LogWriter.WriteToFile("ModbusRTU: 6)", err.StackTrace, "DTC_ErrorLog");
                return null;
            }

            return _RecieveBuff;
        }

        /// <summary>
        ///  | START  |  ADDRESS |  FUNCTION  |  DATA   |  LRC CHECK  |  END          |
        ///  |1 char :|  2 chars |  2 chars   |  n chars|  2 chars    |  2 chars CRLF |
        /// </summary>
        public char[] MakeAscFrame(string unitAddress, string commandType, string regAddress, string wordCount)
        {
            string protocolss = SetValues.Set_CommunicationProtocol;

            if (!string.IsNullOrEmpty(protocolss))
            {
                if (protocolss.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                {
                    return CreateAsciiFrame(unitAddress, commandType, regAddress, wordCount);
                }
                else if (protocolss.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                {
                    return CreateRtuFrame(unitAddress, commandType, regAddress, wordCount);
                }
            }

            return null;
        }

        public char[] CreateAsciiFrame(string unitAddress, string commandType, string regAddress, string wordCount)
        {
            char[] obj = Create1(unitAddress, commandType, regAddress, wordCount);

            byte[] CalLRCBts = new byte[2];

            if (!string.IsNullOrEmpty(unitAddress) && !string.IsNullOrEmpty(commandType) &&
                !string.IsNullOrEmpty(regAddress) && !string.IsNullOrEmpty(wordCount))
            {
                try
                {
                    //---------------------------------------------
                    for (int s = 0; s < obj.Length; s++)
                    {
                        byte hex = Convert.ToByte(obj[s]);
                        listSendBuff.Add(hex);
                    }

                    string myStringData = CalculateLRC(obj);//obj

                    char[] addrLRCchr = myStringData.ToCharArray();

                    for (int s = 0; s < addrLRCchr.Length; s++)
                    {
                        byte LRCByte = Convert.ToByte(addrLRCchr[s]);
                        listSendBuff.Add(LRCByte);
                    }

                    listSendBuff.Add(Convert.ToByte(0x0D));
                    listSendBuff.Add(Convert.ToByte(0x0A));

                    SetValues.Set_ASKFrame = new string(obj);
                    SetValues.Set_LRCFrame = new string(addrLRCchr);

                    if (wordCount != "" && SetValues.Set_Form == "SinglecmdText")
                    {
                        _GetLRCResultResult();
                    }

                    return obj;
                }
                catch (Exception err)
                {
                    LogWriter.WriteToFile("ModbusRTU: 3)", err.StackTrace, "DTC_ErrorLog");
                    return new char[0];
                }
            }
            return obj;
        }

        /// <summary>
        ///  | START      | ADDRESS | FUNCTION  |     DATA   |  CRC CHECK  |  END          |
        ///  |T1-T2-T3-T4*| 8 bits  |  8 bits   |  n * 8 bits|   16 bits   |  T1-T2-T3-T4* |
        /// </summary>
        public char[] CreateRtuFrame(string unitAddress, string commandType, string regAddress, string wordCount)
        {
            if (!string.IsNullOrEmpty(unitAddress) && !string.IsNullOrEmpty(commandType) &&
                !string.IsNullOrEmpty(regAddress) && !string.IsNullOrEmpty(wordCount))
            {
                try
                {
                    _SendBuff = ReadHoldingRegister(Convert.ToByte(unitAddress), Convert.ToByte(commandType),
                        Convert.ToUInt16(regAddress, 16), Convert.ToUInt16(wordCount, 16));

                    SetValues.Set_ASKFrame = this.DisplayFrame(_SendBuff);
                    SetValues.Set_LRCFrame = string.Format("{0:X2} {1:X2}",
                        _SendBuff[_SendBuff.Length - 2], _SendBuff[_SendBuff.Length - 1]);

                    if (wordCount != "" && SetValues.Set_Form == "SinglecmdText")
                    {
                        _GetLRCResultResult();
                    }

                }
                catch (Exception err)
                {
                    return new char[0];
                }
            }
            return Encoding.UTF8.GetString(_SendBuff).ToCharArray();
        }
        
        public void MakeAscFrameCombinations(string unitAddress, string commandType, string regAddress, string wordCount)
        {
            byte[] CalLRCBts = new byte[2];
            try
            {
                _charBuff = new char[13];
                byte[] LRC = new byte[2];

                byte[] pTemparr = new byte[2];
                if (!string.IsNullOrEmpty(unitAddress) && !string.IsNullOrEmpty(commandType) &&
                    !string.IsNullOrEmpty(regAddress) && !string.IsNullOrEmpty(wordCount))
                {
                    _charBuff[0] = ':';
                    char[] untaddr = unitAddress.ToCharArray();
                    _charBuff[1] = untaddr[0];
                    _charBuff[2] = untaddr[1];

                    char[] cmdType = commandType.ToCharArray();
                    _charBuff[3] = cmdType[0];
                    _charBuff[4] = cmdType[1];

                    char[] Fnctnaddr = regAddress.ToCharArray();
                    _charBuff[5] = Fnctnaddr[0];
                    _charBuff[6] = Fnctnaddr[1];
                    _charBuff[7] = Fnctnaddr[2];
                    _charBuff[8] = Fnctnaddr[3];

                    char[] WrdCntaddr = wordCount.ToCharArray();
                    if (wordCount != "")
                    {
                        _charBuff[9] = WrdCntaddr[0];
                        _charBuff[10] = WrdCntaddr[1];
                        _charBuff[11] = WrdCntaddr[2];
                        _charBuff[12] = WrdCntaddr[3];
                    }

                    for (int s = 0; s < _charBuff.Length; s++)
                    {
                        byte hex = Convert.ToByte(_charBuff[s]);
                        _SendBuff[s] = hex;
                    }

                    string myStringData = CalculateLRC(_charBuff);

                    char[] addrLRCchr = myStringData.ToCharArray();
                    for (int s = 0; s < addrLRCchr.Length; s++)
                    {
                        byte LRCByte = Convert.ToByte(addrLRCchr[s]);
                        _SendBuff[s + 13] = LRCByte;
                    }

                    _SendBuff[15] = 0x0D;
                    _SendBuff[16] = 0x0A;

                    SetValues.Set_ASKFrame = new string(_charBuff);
                    SetValues.Set_LRCFrame = new string(addrLRCchr);

                    if (wordCount != "" && SetValues.Set_Form == "SinglecmdText")
                    {
                        _GetLRCResultResult();
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("ModbusRTU: 4)", ex.StackTrace, "DTC_ErrorLog");
            }
        }

        public byte[] Read_AscDataRegister(int Reg_No)
        {
            try
            {
                if (IsSerialPortOpen())
                {

                    if (string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol) ||
                        SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                    {
                        if (listSendBuff.Count > 0)
                        {
                            byte[] sendBuff = new byte[listSendBuff.Count];

                            for (int i = 0; i < listSendBuff.Count; i++)
                            {
                                sendBuff[i] = Convert.ToByte(listSendBuff[i]);

                            }

                            _port.Write(sendBuff, 0, sendBuff.Length);
                        }
                    }
                    else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                    {
                        _port.Write(_SendBuff, 0, _SendBuff.Length);
                    }


                    Thread.Sleep(250);

                    int size = _port.ReadBufferSize;

                    if (size > 0)
                    {
                        _RecieveBuff = new byte[_port.BytesToRead];
                        int byteRead = _port.Read(_RecieveBuff, 0, _RecieveBuff.Length);
                    }
                    else
                    {
                        _RecieveBuff = Encoding.ASCII.GetBytes(_port.ReadExisting());
                    }

                    return _RecieveBuff;
                }
            }
            catch (Exception err)
            {
                LogWriter.WriteToFile("ModbusRTU: 5)", err.StackTrace, "DTC_ErrorLog");
                return null;
            }
            return null;
        }

        public string AsciiToHex(string ascii)
        {
            StringBuilder sb = new StringBuilder();

            byte[] inputBytes = Encoding.UTF8.GetBytes(ascii);

            foreach (byte b in inputBytes)
            {
                sb.Append(string.Format("{0:x2}", b));
            }
            return sb.ToString();
        }

        static byte[] HexToBytes(string input)
        {
            byte[] result = new byte[input.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(input.Substring(2 * i, 2), 16);
            }
            return result;
        }
        
        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        public string CalculateLRC(char[] chrLRC)
        {
            int num = 0;

            if (chrLRC.Length > 0)
            {
                for (int i = 1; i < chrLRC.Length; i++)
                {
                    string hexValue = chrLRC[i].ToString() + chrLRC[i + 1].ToString();
                    int a = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                    num += a;
                    i++;
                }
                int res = 0;

                if (num > 255)
                {
                    byte[] b = BitConverter.GetBytes(num);

                    if (b != null)
                    {
                        res = 255 - Convert.ToInt32(b[0]) + 1;
                    }
                }
                else
                {
                    res = (255 - num) + 1;
                }
                return res.ToString("X");
            }
            else
                return null;
        }

        public static byte LOBYTE(int iValue)
        {
            return Convert.ToByte(iValue & 255);
        }

        public static byte HIBYTE(int iValue)
        {
            return Convert.ToByte((iValue >> 8) & 255);
        }   

        // MODBUS RTU Frame creation
        private byte[] ReadHoldingRegister(byte slaveAddress, byte functionCode, ushort startAddress, ushort numberOfPoints)
        {
            byte[] frame = new byte[8];

            frame[0] = slaveAddress;
            frame[1] = functionCode;
            frame[2] = (byte)(startAddress >> 8);   // Hi
            frame[3] = (byte)(startAddress);        //Lo
            frame[4] = (byte)(numberOfPoints >> 8); // Hi
            frame[5] = (byte)(numberOfPoints);      //Lo

            byte[] crc = this.CalculateCRC(frame);
            frame[6] = crc[0];                      //Lo
            frame[7] = crc[1];                      //Hi
            return frame;
        }

        // Compute the MODBUS RTU CRC
        private byte[] CalculateCRC(byte[] frame)
        {
            byte[] result = new byte[2];
            ushort CRCFull = 0xFFFF;        //set 16-bit register (CRC register) = FFFFH

            char CRCLSB;

            for (int i = 0; i < frame.Length - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ frame[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                    {
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                    }
                }
            }
            result[1] = (byte)((CRCFull >> 8) & 0xFF);
            result[0] = (byte)(CRCFull & 0xFF);

            return result;
        }

        private string DisplayFrame(byte[] frame)
        {
            string result = string.Empty;
            foreach (byte item in frame)
            {
                result += string.Format("{0:X2} ", item);
            }
            return result;
        }

    }


}
