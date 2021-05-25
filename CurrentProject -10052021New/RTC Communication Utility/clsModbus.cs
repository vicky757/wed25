using ClassList;
using RTC_Communication_Utility.UserControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTC_Communication_Utility
{
    public class clsModbus
    {
        // serial port variable
     public SerialPort _port = null;

        // serial port static variable
        private static SerialPort _port1;
        string Rlrc = string.Empty;
        string strr = string.Empty;
        string RlrcAgain = string.Empty;
        string strAgain = string.Empty;
       // public static bool Singlecmdrepeat;
        public static bool Singlecmdsleep;
        char[] res11Again;
        char[] ressAgain;
        char[] res11;
        char[] ress;
        
             
        // serial port property
        public static SerialPort Port1
        {
            get
            {
                // check if serial port instance is available
                if (_port1 == null)
                {
                    // create new serial port instance
                    _port1 = new SerialPort();
                    _port1.BreakState = false;
                  
                }

                return _port1;
            }
        }

        static void _port1_Disposed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        static void _port1_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        static void _port1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
           // throw new NotImplementedException();
           // MessageBoxEx.Show("Disconnect Error");
        }

        static void _port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
           // throw new NotImplementedException();
          //  MessageBoxEx.Show("Disconnect Error");
        }


        // delegate to return CRC-LRC 
        public delegate void GetLRCResult();
        // event 
        public event GetLRCResult _GetLRCResultResult;

        public SerialPort GetSerialPort
        {
            get
            {
                return _port;
            }
        }


        //constructor
        public clsModbus()
        {

        }




      


        // open serial port method 
        // portName 
        // baudrate
        // parity
        // stopbit
        // bitsLength
        // readTimeOut
        public bool OpenSerialPort(string portName, int baudrate, string parity,
            int stopbit, int bitsLength, int readTimeOut = 100)
        {
            try
            {
                // check if serial port instance is available
                // check if serial port instance is available
                if (_port == null)
                {
                    // creates new instance
                    _port = new SerialPort(portName);
                   
                  //  _port.Disposed += _port1_Disposed;
                }

                // set baudrate
                _port.BaudRate = baudrate;

                // set parity
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

                // set stopbit
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

                // databit
                _port.DataBits = bitsLength;

                // readtimeout
                _port.ReadTimeout = readTimeOut;

                try
                {
                    // check if port is open or not
                    if (_port.IsOpen)
                    {
                        return true;
                    }
                    else
                    {
                        try
                        {
                            _port.DataReceived += _port1_DataReceived;
                            // open port
                            if (!_port.IsOpen)

                                _port.Open();

                            return true;
                        }catch(Exception aa)
                            {
                                if (!_port.IsOpen)

                                    _port.Open();
                                return true;
                            }
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    LogWriter.WriteToFile("OpenComPort() : ", ex.StackTrace, "clsModbus_ErrorLog");
                    return false;
                }

                #region 22082019 Cls

                //if (_port == null)
                //{
                //    _port = new SerialPort(portName);

                //    _port.BaudRate = baudrate;

                //    switch (parity)
                //    {
                //        case "Even":
                //            _port.Parity = Parity.Even;
                //            break;
                //        case "Odd":
                //            _port.Parity = Parity.Odd;
                //            break;
                //        case "None":
                //        default:
                //            _port.Parity = Parity.None;
                //            break;
                //    }

                //    switch (stopbit.ToString())
                //    {
                //        case "1":
                //            _port.StopBits = StopBits.One;
                //            break;
                //        case "1.5":
                //            _port.StopBits = StopBits.OnePointFive;
                //            break;
                //        case "2":
                //            _port.StopBits = StopBits.Two;
                //            break;
                //        default:
                //            _port.StopBits = StopBits.None;
                //            break;
                //    }

                //    _port.DataBits = bitsLength;

                //    _port.ReadTimeout = readTimeOut;

                //    try
                //    {
                //        if (!_port.IsOpen)
                //        {
                //            _port.Open();
                //            return true;
                //        }

                //    }
                //    catch (Exception ex)
                //    {
                //        string msg = ex.Message;
                //        LogWriter.WriteToFile("OpenComPort() : ", ex.StackTrace, "clsModbus_ErrorLog");
                //        return false;
                //    }
                //}
                //else
                //{
                //    if (!_port.IsOpen)
                //    {
                //        _port.Open();
                //    }

                //    LogWriter.WriteToFile("OpenSerialPort()", "Port opened"
                //         , "clsModbus");

                //    return true;
                //}
                #endregion
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("clsModbus: 2)", ex.StackTrace, "clsModbus_ErrorLog");
                return false;
            }

        }

        public bool IsSerialPortOpenDeviceinfo()
        {
            try
            {
                // check if instance id available

                if (_port != null)
                {
                    // check if port is open or not 
                    if (_port.IsOpen)
                        return true;
                    else
                    {
                        // open port
                        _port.Open();
                        return true;
                    }
                }
                else
                {
                    string portName = Serial.GetQTProductPort(); // "COM1";
                    _port = new SerialPort(portName);
                    if (_port != null)
                    {
                        if (!_port.IsOpen)
                        {
                            _port.Open();
                            return true;
                        }
                        //    _port = null;
                    }
                    return false;
                }



            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }
        // check if serial port is opened, return true if opened else false
        public bool IsSerialPortOpen()
        {
            try
            {
                // check if instance id available
                if (!ClassList.CommonConstants.isDeviceInfo)
                {
                    // check if port is open or not 
                    if (_port.IsOpen)
                        return true;
                    else
                    {
                        // open port
                        _port.Open();
                        return true;
                    }
                }
               else{
                    if (_port != null)
                    {
                        // check if port is open or not 
                        if (_port.IsOpen)
                            return true;
                        else
                        {
                            // open port
                            _port.Open();
                            return true;
                        }
                    }
                    else
                    {
                        string portName = Serial.GetQTProductPort(); // "COM1";
                        _port = new SerialPort(portName);
                        if (_port != null)
                        {
                            if (!_port.IsOpen)
                            {
                                _port.Open();
                                return true;
                            }
                            //    _port = null;
                        }
                        return false;
                    }
                }
                  
            }
            catch (Exception ex)
            {
                ex.ToString();
                Thread.Sleep(100);
                //MessageBoxEx.Show("Access to the port is denied");
                return false;
            }
        }

        // close serial port
        public void CloseSerialPort()
        {
            // check if instance id available
            if (_port != null)
            {
                // check if port is open or not 
                if (_port.IsOpen)
                {
                    // close port
                    _port.Close();
                }
            }
        }
        public void ACloseSerialPort()
        {
            // check if instance id available
          //  if (_port != null)
            try
            {
                // check if port is open or not 
                if (_port.IsOpen)
                {
                    // close port
                    _port.Close();
                }
                if (!_port.IsOpen)
                {
                    // close port
                    _port.Open();
                }
               
            }
            catch (Exception ae)
            {
                ae.ToString();
                _port.Close();
               // MessageBoxEx.Show(ae.ToString());
            }
        }

        // create modbus ascii frame
        // method accepts node address, function code, starting address, and no of words
        // 
        public byte[] AscFrame(string unitAddress, string commandType,
            string regAddress, string wordCount)
        {
            byte[] sendBuff = null;
            byte[] receiveBuff = null;
            
            // check if parameters are null or empty
            if (!string.IsNullOrEmpty(unitAddress) && !string.IsNullOrEmpty(commandType) &&
                !string.IsNullOrEmpty(regAddress) && !string.IsNullOrEmpty(wordCount))
            {
                try
                {
                    
                    sendBuff = CreateAscFrame((Convert.ToInt32(unitAddress).ToString("X2")).PadLeft(2, '0'),
                        commandType, regAddress, wordCount);

                    // converts byte array to char array
                    char[] res = ConvertByteArrayToCharArray(sendBuff);

                    // check if send frame is empty or not
                    if (sendBuff != null && sendBuff.Length > 0)
                    {
                        // creates concatenated char array with space
                        SetValues.Set_ASKFrame = string.Join("", res);

                        // send byte array over network
                        receiveBuff = SendFrame(sendBuff, SetValues.Set_Baudrate);


                        // check if received byte array is empty or not
                        if (receiveBuff != null)
                        {

                            // converts byte array to char array

                            res11 = ConvertByteArrayToCharArray(receiveBuff);
                            ress = new char[res11.Length - 4];
                            for (int i = 0; i < res11.Length - 4; i++)
                            {
                                ress[i] = res11[i];

                            }
                             strr = new string(res11, res11.Count() - 4, 2);

                             Rlrc = this.CalculateLRC(ress);
                            if(Rlrc == "1")
                            {
                                Rlrc = "01";
                            }


                            //if (wordCount != "" && SetValues.Set_Form == "SinglecmdText")
                            //{
                            //    _GetLRCResultResult();
                            //}
                            if (strr != Rlrc)
                            {
                                Thread.Sleep(10);
                                sendBuff = CreateAscFrame((Convert.ToInt32(unitAddress).ToString("X2")).PadLeft(2, '0'),
                                commandType, regAddress, wordCount);

                                // converts byte array to char array
                                char[] resAgain = ConvertByteArrayToCharArray(sendBuff);

                                // check if send frame is empty or not
                                if (sendBuff != null && sendBuff.Length > 0)
                                {
                                    // creates concatenated char array with space
                                    SetValues.Set_ASKFrame = string.Join("", resAgain);

                                    // send byte array over network
                                    receiveBuff = SendFrame(sendBuff, SetValues.Set_Baudrate);


                                    // check if received byte array is empty or not
                                    if (receiveBuff != null)
                                    {

                                        // converts byte array to char array

                                        res11Again = ConvertByteArrayToCharArray(receiveBuff);
                                         ressAgain = new char[res11Again.Length - 4];
                                        for (int i = 0; i < res11Again.Length - 4; i++)
                                        {
                                            ressAgain[i] = res11Again[i];

                                        }
                                         strAgain = new string(res11Again, res11Again.Count() - 4, 2);

                                         RlrcAgain = this.CalculateLRC(ressAgain);
                                        if (strAgain != RlrcAgain)
                                        {
                                           // LogWriter.WriteToFile("calculate LRC!=" + Rlrc, regAddress, "clsModbus_ErrorLog");
                                           // LogWriter.WriteToFile("Receive LRC != " + str, regAddress, "clsModbus_ErrorLog");

                                        }
                                        if (wordCount != "" && SetValues.Set_Form == "SinglecmdText")
                                        {
                                            _GetLRCResultResult();
                                        }
                                    }
                                }
                            }
                            else
                            {
                               // LogWriter.WriteToFile("calculate LRC==" + Rlrc, regAddress, "clsModbus_ErrorLog");
                               // LogWriter.WriteToFile("Receive LRC == " + str, regAddress, "clsModbus_ErrorLog");
                            }



                        
                    }
                }
                       
                    
                    return receiveBuff;
                }
                catch (Exception err)
                {
                    LogWriter.WriteToFile("AscFrame() : ", err.StackTrace, "clsModbus_ErrorLog");
                    return null;
                }
            }
            return receiveBuff;
        }

       //  Converts Byte Array To Char Array
        private static char[] ConvertByteArrayToCharArray(byte[] sendBuff)
        {
            return System.Text.Encoding.UTF8.GetString(sendBuff).ToCharArray();
        }

        // creates modbus ascii frame (not used)
        public byte[] AscFrameSingle(string unitAddress, string commandType, string regAddress,
           string wordCount)
        {
            byte[] sendBuff = null;
            byte[] receiveBuff = null;

            if (!string.IsNullOrEmpty(unitAddress) && !string.IsNullOrEmpty(commandType) &&
                !string.IsNullOrEmpty(regAddress) && !string.IsNullOrEmpty(wordCount))
            {
                try
                {
                    sendBuff = CreateAscFrame(unitAddress, commandType, regAddress, wordCount);

                    char[] res = ConvertByteArrayToCharArray(sendBuff);

                    if (sendBuff != null && sendBuff.Length > 0)
                    {
                        SetValues.Set_ASKFrame = string.Join("", res);

                        receiveBuff = SendFrame(sendBuff,SetValues.Set_Baudrate);

                        if (receiveBuff != null)
                        {
                            char[] res11 = ConvertByteArrayToCharArray(receiveBuff);

                            if (wordCount != "" && SetValues.Set_Form == "SinglecmdText")
                            {
                                _GetLRCResultResult();
                            }
                        }
                        return receiveBuff;
                    }
                }
                catch (Exception err)
                {
                    LogWriter.WriteToFile("AscFrame() : ", err.StackTrace, "clsModbus_ErrorLog");
                    return null;
                }
            }
            return receiveBuff;
        }

        // create modbus ascii frame for single command text form
        public byte[] AscFrameSingle2(string unitAddress, string commandType, string regAddress,
   string wordCount)
        {
            byte[] sendBuff = null;
            byte[] receiveBuff = null;

            if (!string.IsNullOrEmpty(unitAddress) && !string.IsNullOrEmpty(commandType) &&
                !string.IsNullOrEmpty(regAddress) && !string.IsNullOrEmpty(wordCount))
            {
                try
                {
                    sendBuff = CreateAscFrame(unitAddress, commandType, regAddress, wordCount);
                 //   Console.WriteLine(builder.ToString());
                  //  MessageBoxEx.Show(sendBuff.ToString());
                    if (sendBuff != null && sendBuff.Length > 0)
                    {
                        char[] res = ConvertByteArrayToCharArray(sendBuff);
                        string str = new string(res);
                       // MessageBoxEx.Show(str);
                        if (res != null)
                        {
                            SetValues.Set_ASKFrame = string.Join("", res);
                          //  MessageBoxEx.Show(SetValues.Set_ASKFrame);

                            if (wordCount != "" && SetValues.Set_Form == "SinglecmdText")
                            {
                              //  MessageBoxEx.Show(SetValues.Set_ASKFrame);
                                _GetLRCResultResult();
                            }
                        }
                        return receiveBuff;
                    }
                }
                catch (Exception err)
                {
                    LogWriter.WriteToFile("AscFrame() : ", err.StackTrace, "clsModbus_ErrorLog");
                 
                    return null;
                }
            }
            return receiveBuff;
        }

        // MODBUS ASCII Frame creation
        public byte[] CreateAscFrame(string slaveAddress, string functionCode,
            string startAddress, string wordCount)
        {
            ArrayList list = new ArrayList();
            try
            {
                list.Add(':');

                char[] untaddr = slaveAddress.ToCharArray();
                list.Add(untaddr[0]);
                list.Add(untaddr[1]);

                char[] cmdType = functionCode.ToCharArray();
                list.Add(cmdType[0]);
                list.Add(cmdType[1]);

                char[] Fnctnaddr = startAddress.ToCharArray();
                foreach (char item in Fnctnaddr)
                {
                    list.Add(item);
                }

                char[] WrdCntaddr = wordCount.ToCharArray();
                foreach (char item in WrdCntaddr)
                {
                    list.Add(item);
                }

                if (functionCode == "10")
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

                char[] lrc = this.CalculateLRC((char[])list.ToArray(typeof(char))).ToCharArray();
                if (lrc.Length == 1)
                
                {
                    char[] arr = new char[2];
                   
                        char a = lrc[0];
                        arr[0] = '0';
                        arr[1] = a;
                    
                    foreach (char item in arr)
                    {
                        list.Add(item);
                    }
                    SetValues.Set_LRCFrame = string.Join("", arr);

                    byte[] bytes = new byte[list.Count + 2];
                    int i = 0;
                    while (i < list.Count)
                    {
                        byte[] n = Encoding.ASCII.GetBytes(list[i].ToString());
                        foreach (var item in n)
                        {
                            bytes[i] = item;

                        }
                        i++;
                    }
                    bytes[list.Count] = Convert.ToByte(0x0D);
                    bytes[list.Count + 1] = Convert.ToByte(0x0A);

                    LogWriter.WriteToFile("CreateAscFrame()", string.Join(",", bytes)
                             , "clsModbus");

                    return bytes;

                }
                else
                {
                    foreach (char item in lrc)
                    {
                        list.Add(item);
                    }
                    SetValues.Set_LRCFrame = string.Join("", lrc);

                    byte[] bytes = new byte[list.Count + 2];
                    int i = 0;
                    while (i < list.Count)
                    {
                        byte[] n = Encoding.ASCII.GetBytes(list[i].ToString());
                        foreach (var item in n)
                        {
                            bytes[i] = item;

                        }
                        i++;
                    }
                    bytes[list.Count] = Convert.ToByte(0x0D);
                    bytes[list.Count + 1] = Convert.ToByte(0x0A);

                    LogWriter.WriteToFile("CreateAscFrame()", string.Join(",", bytes)
                             , "clsModbus");

                    return bytes;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("CreateAscFrame() : ", ex.StackTrace, "clsModbus_ErrorLog");
                return null;
            }

        }

        // Creates MODBUS RTU frame
        public byte[] RtuFrame(string unitAddress, string commandType, string regAddress,
            string wordCount,string boudrate)
        {
            byte[] sendBuff = null;
            byte[] receiveBuff = null;

            if (!string.IsNullOrEmpty(unitAddress) && !string.IsNullOrEmpty(commandType) &&
                !string.IsNullOrEmpty(regAddress) && !string.IsNullOrEmpty(wordCount))
            {
                try
                {
                    sendBuff = CreateRtuFrame(Convert.ToByte(unitAddress), Convert.ToByte(commandType),
                           Convert.ToUInt16(regAddress, 16), (ushort)Convert.ToInt16(wordCount, 16));

                    if (sendBuff != null && sendBuff.Length > 0)
                    {
                        receiveBuff = SendFrame(sendBuff, boudrate);

                        if (receiveBuff != null)
                        {
                            SetValues.Set_ASKFrame = this.DisplayFrame(sendBuff);
                            SetValues.Set_LRCFrame = string.Format("{0:X2} {1:X2}",
                                sendBuff[sendBuff.Length - 2], sendBuff[sendBuff.Length - 1]);

                            //if (wordCount != "" && SetValues.Set_Form == "SinglecmdText")
                            //{
                            //    _GetLRCResultResult();
                            //}

                            return receiveBuff;
                        }
                    }
                }
                catch (Exception err)
                {
                    
                    LogWriter.WriteToFile("RtuFrame() : ", err.StackTrace, "clsModbus_ErrorLog");
                   
                    return null;
                }
            }
            return receiveBuff;
        }

        // RTU frame 
        public byte[] RtuFrameSingle(string unitAddress, string commandType, string regAddress,
            string wordCount)
        {
            byte[] sendBuff = null;
            byte[] receiveBuff = null;

            if (!string.IsNullOrEmpty(unitAddress) && !string.IsNullOrEmpty(commandType) &&
                !string.IsNullOrEmpty(regAddress) && !string.IsNullOrEmpty(wordCount))
            {
                try
                {
                    sendBuff = CreateRtuFrame(Convert.ToByte(unitAddress), Convert.ToByte(commandType),
                           Convert.ToUInt16(regAddress, 16), Convert.ToUInt16(wordCount, 16));

                    if (sendBuff != null && sendBuff.Length > 0)
                    {

                        SetValues.Set_ASKFrame = this.DisplayFrame(sendBuff);
                        SetValues.Set_LRCFrame = string.Format("{0:X2} {1:X2}",
                            sendBuff[sendBuff.Length - 2], sendBuff[sendBuff.Length - 1]);

                        if (wordCount != "" && SetValues.Set_Form == "SinglecmdText")
                        {
                            _GetLRCResultResult();
                        }

                        return sendBuff;

                    }
                }
                catch (Exception err)
                {
                    LogWriter.WriteToFile("RtuFrame() : ", err.StackTrace, "clsModbus_ErrorLog");
                  
                    return null;
                }
            }
            return receiveBuff;
        }

        // MODBUS RTU Frame creation
        public byte[] CreateRtuFrame(byte slaveAddress, byte functionCode, ushort startAddress,
            ushort numberOfPoints)
        {
            try
            {
                if (functionCode == 16)
                {
                    List<byte> list = new List<byte>();

                    list.Add(slaveAddress);
                    list.Add(functionCode);
                    list.Add((byte)(startAddress >> 8));
                    list.Add((byte)(startAddress));        //Lo
                    list.Add((byte)(numberOfPoints >> 8)); // Hi
                    list.Add((byte)(numberOfPoints));      //Lo
                    list.Add((byte)(numberOfPoints*2));

                    if (!string.IsNullOrEmpty(SetValues.Set_Words1))
                    {
                        ushort count = Convert.ToUInt16(SetValues.Set_Words1, 16);
                        list.Add((byte)(count >> 8));
                        list.Add((byte)(count));
                    }
                    if (!string.IsNullOrEmpty(SetValues.Set_Words))
                    {
                        var strs = GetNextChars(SetValues.Set_Words, 4);
                        foreach (var item in strs)
                        {
                            ushort count = Convert.ToUInt16(item, 16);
                            list.Add((byte)(count >> 8));
                            list.Add((byte)(count));
                        }
                        //list.Add((byte)(0));
                        
                    }
                    byte[] crc = this.CalculateCRC2(list.ToArray());

                    list.Add(crc[0]);       //Lo
                    list.Add(crc[1]);       //Hi

                    LogWriter.WriteToFile("CreateRtuFrame()", string.Join(",", list)
                         , "clsModbus");

                    return list.ToArray();
                }
                else
                {
                    byte[] frame = new byte[8];

                    frame[0] = slaveAddress;
                    frame[1] = functionCode;
                    frame[2] = (byte)(startAddress >> 8);   // Hi
                    frame[3] = (byte)(startAddress);        //Lo
                    frame[4] = (byte)(numberOfPoints >> 8); // Hi
                    frame[5] = (byte)(numberOfPoints);      //Lo

                    byte[] crc = this.CalculateCRC(frame);

                    frame[frame.Length - 2] = crc[0];                      //Lo
                    frame[frame.Length - 1] = crc[1];                      //Hi

                    LogWriter.WriteToFile("CreateRtuFrame()", string.Join(",", frame)
                         , "clsModbus");

                    return frame;

                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("CreateRtuFrame() : ", ex.StackTrace, "clsModbus_ErrorLog");
               
                return null;
            }
        }

        IEnumerable<string> GetNextChars(string str, int iterateCount)
        {
            var words = new List<string>();

            for (int i = 0; i < str.Length; i += iterateCount)
                if (str.Length - i >= iterateCount) 
                    words.Add(str.Substring(i, iterateCount));
                else words.Add(str.Substring(i, str.Length - i));

            return words;
        }

        // Compute the MODBUS RTU CRC
        private byte[] CalculateCRC(byte[] frame)
        {
            byte[] result = new byte[2];
            ushort CRCFull = 0xFFFF;        //set 16-bit register (CRC register) = FFFFH

            char CRCLSB;
            try
            {
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

                LogWriter.WriteToFile("CalculateCRC()", string.Join(",", result)
                                         , "clsModbus");

                return result;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("CalculateCRC() : ", ex.StackTrace, "clsModbus_ErrorLog");
                return null;
            }
        }

        private byte[] CalculateCRC2(byte[] frame)
        {
          byte[] result = new byte[2];
          ushort CRCFull = 0xFFFF;        //set 16-bit register (CRC register) = FFFFH

          char CRCLSB;
          try
          {
            for (int i = 0; i < frame.Length; i++)
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

            LogWriter.WriteToFile("CalculateCRC()", string.Join(",", result)
                                     , "clsModbus");

            return result;
          }
          catch (Exception ex)
          {
            LogWriter.WriteToFile("CalculateCRC() : ", ex.StackTrace, "clsModbus_ErrorLog");
            return null;
          }
        }
    // Compute the MODBUS ASCII LRC
    private string CalculateLRC(char[] chrLRC)
        {
            Int16 num = 0;
            try
            {
                if (chrLRC.Length > 0)
                {
                    for (int i = 1; i < chrLRC.Length; i++)
                    {
                        string hexValue = chrLRC[i].ToString() + chrLRC[i + 1].ToString();
                        Int16 a = Int16.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                        num += a;
                        i++;
                    }
                    Int16 res = 0;

                    if (num > 255)
                    {
                        byte[] b = BitConverter.GetBytes(num);

                        if (b != null)
                        {
                            res = Convert.ToInt16(255 - Convert.ToInt16(b[0]) + 1);
                        }
                    }
                    else
                    {
                        res = Convert.ToInt16((255 - num) + 1);
                    }
                    
                    LogWriter.WriteToFile("CalculateLRC()", res.ToString()
                                         , "clsModbus");
                    //if (res == 1)
                    //{
                    //    return res.ToString("01");
                    //}
                    return res.ToString("X");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("CalculateLRC() : ", ex.StackTrace, "clsModbus_ErrorLog");
                return null;
            }
            return null;
        }

        // Compute the MODBUS ASCII LRC
        private string CalculateLRC2(char[] chrLRC)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(chrLRC);

            byte LRC = 0;
            try
            {

                for (int i = 0; i < byteArray.Length; i++)
                {
                    LRC ^= byteArray[i];
                }
                Int16 x = Convert.ToInt16(LRC);

                return x.ToString("x2");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Display byte buffer in string format
        public string DisplayFrame(byte[] frame)
        {
            string result = string.Empty;

            foreach (byte item in frame)
            {
                result += string.Format("{0:X2} ", item);
            }
            return result;
        }

        // Send Frame over serial port 
        public byte[] SendFrame(byte[] sendBuff, string boudrate)
        { 
            byte[] recBuff = null;

            try
            {
                // check if serial port
                if (IsSerialPortOpen())
                {
                   // Thread.Sleep(5);
                    // clear out stream
                    _port.DiscardOutBuffer();

                    // clear in stream
                    _port.DiscardInBuffer();

                    // send byte array over stream
                    _port.Write(sendBuff, 0, sendBuff.Length);

                    LogWriter.WriteToFile("SendFrame()", "Frame sent: " , "clsModbus");

                    // thread sleep 
                    if (boudrate == "2400")
                    {
                        Thread.Sleep(250);  //1000 Ascii  //ritesh Discuss
                    }
                    else if (boudrate == "4800")
                    {
                        Thread.Sleep(127);
                    }
                    else if (boudrate == "9600")
                    {
                        Thread.Sleep(68); // (KA) 0918    120-> 50-> 90 (VM)68
                    }
                    else if (boudrate == "19200")
                    {
                        Thread.Sleep(40); // (KA) 0918    120-> 50-> 90 (VM)40
                    }
                    else if (boudrate == "115200")
                    {
                        Thread.Sleep(18); // (KA) 0918    21-> 120-> 50-> 90
                    }
                    else
                    {
                        Thread.Sleep(26); // (KA) 0918    120-> 50-> 90 ->45 While Download
                    }
                    if(Singlecmdsleep)
                    {
                      Thread.Sleep(40);
                      //if(!Singlecmdrepeat)
                      //{
                      //  Singlecmdsleep = false;
                      //}
                    }
                    // number of bytes on stream to read
                    int size = _port.BytesToRead;
                    //size =1;
                    // number of bytes greater then 0
                    if (size > 0)
                    {
                        // create receive buffer 
                        recBuff = new byte[_port.BytesToRead];

                        // read data from stream
                        int byteRead = _port.Read(recBuff, 0, recBuff.Length);

                        LogWriter.WriteToFile("SendFrame()", "Frame received(" + size + "): "
                                     , "clsModbus");
                    }
                }
                // return received data
                return recBuff;
            }
            catch (Exception err)
            {
                LogWriter.WriteToFile("SendFrame() : ", err.Message, "clsModbus_ErrorLog");
                return null;
            }
        }

        // converts byte array to string
        public static string ByteArrayToString(byte[] ba)
        {
            return ba==null && ba.Length <= 0? null:BitConverter.ToString(ba).Replace("-", "");
        }
    }
}
