using ClassList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTC_Communication_Utility
{
    public partial class frmViewDeviceInfo : Form
    {
        clsModbus modbusobj = null;
        public string portName { get; set; }
        SerialPort _port = null;
        Dictionary<string, string> StopBitDict = null;
        Dictionary<string, string> ProtocalDict = null;
        Dictionary<string, string> ParityDict = null;
        Dictionary<string, string> BoudRateDict = null;
        Dictionary<string, string> DataLenthDict = null;


        public void FillDict()
        {
            DataLenthDict = new Dictionary<string, string> {
                    { "0", "8" },
                    { "1", "7" },

             };
            StopBitDict = new Dictionary<string, string> {
                    { "0", "2" },
                    { "1", "1" },
                   
             };
            ProtocalDict = new Dictionary<string, string> {
                    { "0", "ASCII" },
                    { "1", "RTU" },

             };
            ParityDict = new Dictionary<string, string> {
                    { "00", "None" },
                    { "01", "Even" },
                    { "10", "Odd" }
             };
            BoudRateDict = new Dictionary<string, string> {
                    { "000", "2400" },
                    { "001", "4800" },
                    { "010", "9600" },
                    { "011", "19200" },
                    { "100", "38400" },
                    { "101", "115200" },
             };


        }
        public frmViewDeviceInfo()
        {
            modbusobj = new clsModbus();
            FillDict();
            InitializeComponent();
        }

        private void frmViewDeviceInfo_Load(object sender, EventArgs e)
        {
          
            
            try
            {

                // exclude ports, except virtual ports
                portName = Serial.GetQTProductPort(); // "COM1";
                string[] list = Serial.GetUsbSerDevices();
                if (list.Length > 1) 
                {
                    portName = SetValues.Set_PortName;
                }
                _port = new SerialPort(portName);
                if (_port != null)
                {
                    if (!_port.IsOpen)
                    {
                        _port.Open();
                    }
                    //    _port = null;
                }
            }
            catch (Exception ae)
            {

            }


        }

        private string[] GetAllComPortsOnCurrentMachine()
        {
            return SerialPort.GetPortNames(); //get all ports
        }

        private bool CheckIfPortExists(string port)
        {
            try
            {
                bool portExists = GetAllComPortsOnCurrentMachine().Any(x => x == port);

                return portExists;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void btnGetInfo_Click(object sender, EventArgs e)
        {

           
           // GetInfo();
        }


     


        private void GetInfo()
        {
            //1: initialize port
            //---a: get virtual port (if many, get first one)
            //---b: instantiate serial port 
            //---c: open port
            //2: send "INFO"
            //3: check if receive frame is 64 bytes
            //4: decode 64 byte[]
            //5: display in grid

            //1: 
            try
            {

                ClassList.CommonConstants.isDeviceInfo = true;
                if (!string.IsNullOrEmpty(portName))
                {
                    if (CheckIfPortExists(portName))
                    {


                        if (_port != null && _port.IsOpen)
                        {
                            byte[] res = SendFrame();

                            _port.Close();
                            if (res != null && res.Length > 0)
                            {
                                Decode(res);
                            }
                            else
                            {
                                MessageBox.Show("No device detected.");
                            }
                        }
                        else {
                            try
                            {

                                // exclude ports, except virtual ports
                                portName = Serial.GetQTProductPort(); // "COM1";
                                _port = new SerialPort(portName);
                                if (_port != null)
                                {
                                    if (!_port.IsOpen)
                                    {
                                        _port.Open();

                                        byte[] res = SendFrame();

                                        _port.Close();
                                        if (res != null && res.Length > 0)
                                        {
                                            Decode(res);
                                        }
                                        else
                                        {
                                            MessageBox.Show("No device detected.");
                                        }
                                    }
                                    //    _port = null;
                                }
                            }
                            catch (Exception ae)
                            {

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Connection establishing error");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ClassList.CommonConstants.isDeviceInfo = false;
        }

        private void Decode(byte[] res)
        {
            try
            {
                byte fileSize = res[0];

                byte[] nodeAddresses = new byte[8];
                Array.Copy(res, 1, nodeAddresses, 0, 8);

                int nodes = 0;

                foreach (var item in nodeAddresses.ToList())
                {
                    if (item != 0)
                    {
                        nodes++;
                    }
                }

                byte[] devices = new byte[8];
                Array.Copy(res, 9, devices, 0, 8);

                byte[] bootBlock = new byte[8];
                Array.Copy(res, 17, bootBlock, 0, 8);

                byte[] firmWare = new byte[16];
                Array.Copy(res, 25, firmWare, 0, 16);

                byte[] hardWare = new byte[16];
                Array.Copy(res, 41, hardWare, 0, 16);

                int temp1 = -1;
                int temp2 = 0;

                for (int i = 0; i < nodes; i++)
                {
                    string node = Convert.ToString(nodeAddresses[i]);

                   // string device = Convert.ToString(devices[i]) == "1" ? "FL002-0102TV" : (Convert.ToString(devices[i]) == "2" ? "FLA0102TV" : "");
                    string device = string.Empty;
                    if (devices[i] == 1)
                    {
                        device = "FL002-0102TV";
                    }
                    else if (devices[i] == 2)
                    {
                        device = "FLA0102TV";
                    }
                    else if (devices[i] == 3)
                    {
                        device = "FL002-0102TR";
                    }
                    else if (devices[i] == 4)
                    {
                        device = "FLA0102TR";
                    }
                    else 
                    {
                        device = "No Device";
                    }
                    string boot = Convert.ToString(bootBlock[i]).PadLeft(2, '0') + ".00";

                     string fww =  Convert.ToString(firmWare[++temp1]).PadRight(2, '0');
                    char[] charArr = fww.ToCharArray();
                    string fw = Convert.ToString(firmWare[++temp1]).PadLeft(2, '0') ;
                    char[] fff = fw.ToCharArray();
                    string firm = charArr[1] + "" + charArr[0] + "." + fff[0] + "" + fff[1];
                    string hw = Convert.ToInt16(Convert.ToString(hardWare[temp2 + 2])).ToString("X");  //+ Convert.ToString(hardWare[++temp2])
                    string PP = string.Empty;
                    int n = i;
                    string Protocal = String.Empty;
                    //Need To Add Code 
                    try
                    {

                        string sent = string.Empty;
                      //  while (sent == "" || sent == null)
                        {
                            sent = SendFrameToDevice1(node);
                        }
                        Protocal = sent.Replace(" ", string.Empty);
                        //if (sent != null || sent != "")
                        {
                            PP = Convesion(Protocal);
                        }
                      
                    }
                    catch (Exception ae)
                    {

                    }
                    dataGridView1.Rows.Add(n + 1, node, device, boot, hw, firm, PP); //Protocal

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public string Convesion(string sent)
        {
            try
            {
                Thread.Sleep(500);
                char[] charAr = sent.ToCharArray();
                string resultresp = (charAr[6] + "" + charAr[7] + "" + charAr[8] + "" + charAr[9]).ToString();
                string dd = hex2binary(resultresp);
            //   MessageBox.Show(dd);
                char[] charArd = dd.ToCharArray();
                Array.Reverse(charArd);
                string Stopbit = (charArd[0]).ToString();
                string proto = (charArd[1]).ToString();
                string Parity = (charArd[3]).ToString()+""+ (charArd[2]).ToString();
                string boudR = (charArd[6]).ToString() + "" + (charArd[5]).ToString() + "" + (charArd[4]).ToString();
                string DL = (charArd[7]).ToString();
                var myKeyStopBit = StopBitDict.FirstOrDefault(x => x.Key == Stopbit).Value;
                 var myKeyProto = ProtocalDict.FirstOrDefault(x => x.Key == proto).Value;
                var myKeyPar = ParityDict.FirstOrDefault(x => x.Key == Parity).Value;
                var myKeyBoud = BoudRateDict.FirstOrDefault(x => x.Key == boudR).Value;
                var myKeyDT = DataLenthDict.FirstOrDefault(x => x.Key == DL).Value;
                
                string returnData = myKeyBoud + "-" + myKeyDT +"-"+ myKeyPar + "-" + myKeyStopBit + "-" + myKeyProto;
               // MessageBox.Show(returnData);
                return returnData;
            }
            catch (Exception ae)
            {
                return "";
            }

        }
        
        private string hex2binary(string hexvalue)
        {
            string binaryval = "";
            binaryval = Convert.ToString(Convert.ToInt32(hexvalue, 16), 2);
            return binaryval;
        }
        private byte[] SendFrame()
        {
            try
            {
                byte[] recBuff = null;

                // 49 4E 46 4F
                byte[] sendBuff = new byte[4] { (byte)'I', (byte)'N', (byte)'F', (byte)'O' };

                _port.DiscardOutBuffer();
                _port.DiscardInBuffer();
                _port.Write(sendBuff, 0, sendBuff.Length);

                LogWriter.WriteToFile("SendFrame()", "Frame sent: "
                                 , "clsModbus");

                Thread.Sleep(50);

                int size = _port.BytesToRead;

                if (size > 0)
                {
                    recBuff = new byte[_port.BytesToRead];

                    int byteRead = _port.Read(recBuff, 0, recBuff.Length);

                    LogWriter.WriteToFile("SendFrame()", "Frame received(" + size + "): "
                                 , "ViewDeviceInfo");
                }

                return recBuff;
            }
            catch
            {
                return null;
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Rows.Clear();
              //  portName = Serial.GetQTProductPort(); // "COM1";

                GetInfo();
            }
            catch (Exception ae)
            { }
        }

        private void frmViewDeviceInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_port != null)
            {
                if (_port.IsOpen)
                {
                    _port.Close();
                    
                }
                //    _port = null;
            }
        }

        private string SendFrameToDevice1(string node)
        {
            string portName = SetValues.Set_PortName;
            string baudRate = SetValues.Set_Baudrate;
            string parity = SetValues.Set_parity;
            int bitsLength = SetValues.Set_BitsLength;
            string stopBits = SetValues.Set_StopBits;

            if (!string.IsNullOrEmpty(portName) && !string.IsNullOrEmpty(baudRate) && !string.IsNullOrEmpty(parity) &&
            bitsLength > 0 && !string.IsNullOrEmpty(stopBits))
            {
                try
                {
                    if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength))
                    {
                        byte[] RecieveData = null;
                        string result = "";

                        if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                        {
                            if ((SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1) && SetValues.Set_CommType == 1)
                            {
                                RecieveData = modbusobj.AscFrame(node, "03",
                                    "470B", "0001");
                               
                            }
                            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex==2)
                            {
                                RecieveData = modbusobj.RtuFrame(node, "03",
                                    "470B", "0001", SetValues.Set_Baudrate);
                            }

                            if (RecieveData != null && RecieveData.Length > 0)
                            {
                                char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();



                                if ((SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1) && SetValues.Set_CommType == 1)
                                {
                                    result = string.Join("", recdata);
                                 //  MessageBox.Show(result);
                                }
                                else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                                {
                                    result = modbusobj.DisplayFrame(RecieveData);
                                   // MessageBox.Show(result);
                                }

                                //txtBxRecievecmd.Text = result;

                                double int64Val = 0;
                              //  returnVal.Text = int64Val.ToString();
                                return result;
                            }
                            else
                            {
                                // data not received
                              //  txtBxRecievecmd.Text = "Received Data Timeout!";
                                return result;
                            }
                        }
                        else
                        {
                          //  txtBxRecievecmd.Text = "Received Data Timeout!";
                        }
                    }
                    else
                    {
                        //txtBxRecievecmd.Text = "Connection failed";
                        return "";
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                    //LogWriter.WriteToFile("ModbusRTU: 5)", ex.Message, "DTC_ErrorLog");
                    //lblMessage.Text = "3" + ex.Message;
                    return "";
                }
                finally
                {
                    if (modbusobj.IsSerialPortOpen())
                    {
                        modbusobj.CloseSerialPort();
                    }
                }
            }
            else
            {
                // settings are empty
            }
            return "";

        }
    }
}
