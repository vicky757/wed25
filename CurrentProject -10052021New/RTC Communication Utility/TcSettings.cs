using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RTC_Communication_Utility
{
    public partial class TcSettings : Form
    {
        int errorCode = 0;
        clsModbus modObj = null;
        DataSet dsFile = null;
        FileSettings fileSettings = null;
        string m_exePath1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
               "\\DefaultSettings.xml";
        Thread th;
        string portNo = "0";
        //582289863
        public string[] baudRates = new string[BasicProperties.baudRates.Length];
        public string[] dataLengths = new string[BasicProperties.dataLengths.Length];
        public string[] parity = new string[BasicProperties.parity.Length];
        public string[] stopBits = new string[BasicProperties.stopBits.Length];
        public string[] protocol = new string[BasicProperties.protocol.Length];

        #region NodeDictionary

        public Dictionary<string, string> TcMap = new Dictionary<string, string>()
        {
            {"2400-8-None-2-A", "0"},
            {"4800-8-None-2-A", "16"},
            {"9600-8-None-2-A", "32"},
            {"19200-8-None-2-A", "48"},
            {"38400-8-None-2-A", "64"},
            {"115200-8-None-2-A", "80"},

            {"2400-8-None-1-A", "1"},
            {"4800-8-None-1-A", "17"},
            {"9600-8-None-1-A", "33"}, 
            {"19200-8-None-1-A", "49"},
            {"38400-8-None-1-A", "65"},
            {"115200-8-None-1-A", "81"},

            {"2400-8-Even-2-A", "4"},
            {"4800-8-Even-2-A", "20"},
            {"9600-8-Even-2-A", "36"},
            {"19200-8-Even-2-A", "52"},
            {"38400-8-Even-2-A", "68"},
            {"115200-8-Even-2-A", "84"},

            {"2400-8-Even-1-A", "5"},
            {"4800-8-Even-1-A", "21"},
            {"9600-8-Even-1-A", "37"},
            {"19200-8-Even-1-A", "53"},
            {"38400-8-Even-1-A", "69"},
            {"115200-8-Even-1-A", "85"},

            {"2400-8-Odd-2-A", "8"},
            {"4800-8-Odd-2-A", "24"},
            {"9600-8-Odd-2-A", "40"},
            {"19200-8-Odd-2-A", "56"},
            {"38400-8-Odd-2-A", "72"},
            {"115200-8-Odd-2-A", "88"},

            {"2400-8-Odd-1-A", "9"},
            {"4800-8-Odd-1-A", "25"},
            {"9600-8-Odd-1-A", "41"},
            {"19200-8-Odd-1-A", "57"},
            {"38400-8-Odd-1-A", "73"},
            {"115200-8-Odd-1-A", "89"},




            {"2400-8-None-2-R", "2"},
            {"4800-8-None-2-R", "18"},
            {"9600-8-None-2-R", "34"},
            {"19200-8-None-2-R", "50"},
            {"38400-8-None-2-R", "66"},
            {"115200-8-None-2-R", "82"},

            {"2400-8-None-1-R", "3"},
            {"4800-8-None-1-R", "19"},
            {"9600-8-None-1-R", "35"}, 
            {"19200-8-None-1-R", "51"},
            {"38400-8-None-1-R", "67"}, 
            {"115200-8-None-1-R", "83"},

            {"2400-8-Even-2-R", "6"},
            {"4800-8-Even-2-R", "22"},
            {"9600-8-Even-2-R", "38"},
            {"19200-8-Even-2-R", "54"},
            {"38400-8-Even-2-R", "70"},
            {"115200-8-Even-2-R", "86"},

            {"2400-8-Even-1-R", "7"},
            {"4800-8-Even-1-R", "23"},
            {"9600-8-Even-1-R", "39"},
            {"19200-8-Even-1-R", "55"},
            {"38400-8-Even-1-R", "71"},
            {"115200-8-Even-1-R", "87"},

            {"2400-8-Odd-2-R", "10"},
            {"4800-8-Odd-2-R", "26"},
            {"9600-8-Odd-2-R", "42"},
            {"19200-8-Odd-2-R", "58"},
            {"38400-8-Odd-2-R", "74"},
            {"115200-8-Odd-2-R", "90"},

            {"2400-8-Odd-1-R", "11"},
            {"4800-8-Odd-1-R", "27"},
            {"9600-8-Odd-1-R", "43"},
            {"19200-8-Odd-1-R", "59"},
            {"38400-8-Odd-1-R", "75"},
            {"115200-8-Odd-1-R", "91"},
            //7 bit 
            {"2400-7-None-2-A", "128"},
            {"4800-7-None-2-A", "144"},
            {"9600-7-None-2-A", "160"},
            {"19200-7-None-2-A", "176"},
            {"38400-7-None-2-A", "192"},
            {"115200-7-None-2-A", "208"},

            {"2400-7-None-1-A", "129"},
            {"4800-7-None-1-A", "145"},
            {"9600-7-None-1-A", "161"},
            {"19200-7-None-1-A", "177"},
            {"38400-7-None-1-A", "193"},
            {"115200-7-None-1-A", "209"},

            {"2400-7-Even-2-A", "132"},
            {"4800-7-Even-2-A", "148"},
            {"9600-7-Even-2-A", "164"},
            {"19200-7-Even-2-A", "180"},
            {"38400-7-Even-2-A", "196"},
            {"115200-7-Even-2-A", "212"},

            {"2400-7-Even-1-A", "133"},
            {"4800-7-Even-1-A", "149"},
            {"9600-7-Even-1-A", "165"},
            {"19200-7-Even-1-A", "181"},
            {"38400-7-Even-1-A", "197"},
            {"115200-7-Even-1-A", "213"},

            {"2400-7-Odd-2-A", "136"},
            {"4800-7-Odd-2-A", "152"},
            {"9600-7-Odd-2-A", "168"},
            {"19200-7-Odd-2-A", "184"},
            {"38400-7-Odd-2-A", "200"},
            {"115200-7-Odd-2-A", "216"},

            {"2400-7-Odd-1-A", "137"},
            {"4800-7-Odd-1-A", "153"},
            {"9600-7-Odd-1-A", "169"},
            {"19200-7-Odd-1-A", "185"},
            {"38400-7-Odd-1-A", "201"},
            {"115200-7-Odd-1-A", "217"},
            

            {"2400-7-None-2-R", "130"},
            {"4800-7-None-2-R", "146"},
            {"9600-7-None-2-R", "162"},
            {"19200-7-None-2-R", "178"},
            {"38400-7-None-2-R", "194"},
            {"115200-7-None-2-R", "210"},

            {"2400-7-None-1-R", "131"},
            {"4800-7-None-1-R", "147"},
            {"9600-7-None-1-R", "163"},
            {"19200-7-None-1-R", "179"},
            {"38400-7-None-1-R", "195"},
            {"115200-7-None-1-R", "211"},

            {"2400-7-Even-2-R", "134"},
            {"4800-7-Even-2-R", "150"},
            {"9600-7-Even-2-R", "166"},
            {"19200-7-Even-2-R", "182"},
            {"38400-7-Even-2-R", "198"},
            {"115200-7-Even-2-R", "214"},  //not work

            {"2400-7-Even-1-R", "135"},
            {"4800-7-Even-1-R", "151"},
            {"9600-7-Even-1-R", "167"},
            {"19200-7-Even-1-R", "183"},
            {"38400-7-Even-1-R", "199"},
            {"115200-7-Even-1-R", "215"},

            {"2400-7-Odd-2-R", "138"},
            {"4800-7-Odd-2-R", "154"},
            {"9600-7-Odd-2-R", "170"},
            {"19200-7-Odd-2-R", "186"},
            {"38400-7-Odd-2-R", "202"},
            {"115200-7-Odd-2-R", "218"},

            {"2400-7-Odd-1-R", "139"},
            {"4800-7-Odd-1-R", "155"},
            {"9600-7-Odd-1-R", "171"},
            {"19200-7-Odd-1-R", "187"},
            {"38400-7-Odd-1-R", "203"},
            {"115200-7-Odd-1-R", "219"},
        };
        #endregion

        public TcSettings()
        {
            InitializeComponent();

            SetValues.Set_Form = "SetDTC";

            modObj = new clsModbus();
            modObj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
            fileSettings = new FileSettings();
            dsFile = new DataSet();

            Array.Copy(BasicProperties.baudRates, baudRates, BasicProperties.baudRates.Length);
            Array.Copy(BasicProperties.dataLengths, dataLengths, BasicProperties.dataLengths.Length);
            Array.Copy(BasicProperties.parity, parity, BasicProperties.parity.Length);
            Array.Copy(BasicProperties.stopBits, stopBits, BasicProperties.stopBits.Length);
            Array.Copy(BasicProperties.protocol, protocol, BasicProperties.protocol.Length);

            fileSettings.baudrate = "-1";
            fileSettings.datalength = "-1";
            fileSettings.mode = "-1";
            fileSettings.parity = "-1";
            fileSettings.port = "-1";
            fileSettings.stopbits = "-1";
        }

        private void TcSettings_Load(object sender, EventArgs e)
        {
            lblMsg.Visible = false;
            lblMsg.Text = "";
            string[] ports = SerialPort.GetPortNames();

            cmbBxBaudrate.DataSource = BasicProperties.baudRates;
            cmbBxDataLength.DataSource = BasicProperties.dataLengths;
            cmbBxParityBit.DataSource = BasicProperties.parity;
            cmbBxStopBits.DataSource = BasicProperties.stopBits;
            cmbBxProtocol.DataSource = BasicProperties.protocol;

            btnSet.Enabled = false;

            cmbBxBaudrate1.DataSource = baudRates;
            cmbBxDataLength1.DataSource = dataLengths;
            cmbBxParityBit1.DataSource = parity;
            cmbBxStopBits1.DataSource = stopBits;
            cmbBxProtocol1.DataSource = protocol;

            try
            {
                dsFile.ReadXml(m_exePath1);

                if (dsFile.Tables.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt = dsFile.Tables["settings"];

                    if (dt.Rows.Count > 0)
                    {
                        fileSettings.baudrate = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["baudrate"])) ? "-1" : Convert.ToString(dt.Rows[0]["baudrate"]);
                        fileSettings.datalength = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["datalength"])) ? "-1" : Convert.ToString(dt.Rows[0]["datalength"]);
                        fileSettings.mode = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["mode"])) ? "-1" : Convert.ToString(dt.Rows[0]["mode"]);
                        fileSettings.parity = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["parity"])) ? "-1" : Convert.ToString(dt.Rows[0]["parity"]);
                        portNo = fileSettings.port = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["port"])) ? "-1" : Convert.ToString(dt.Rows[0]["port"]);
                        fileSettings.stopbits = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["stopbits"])) ? "-1" : Convert.ToString(dt.Rows[0]["stopbits"]);
                    }
                }

                cmbBxBaudrate.SelectedIndex = Convert.ToInt32(fileSettings.baudrate);
                cmbBxDataLength.SelectedIndex = Convert.ToInt32(fileSettings.datalength);
                cmbBxParityBit.SelectedIndex = Convert.ToInt32(fileSettings.parity);
                cmbBxStopBits.SelectedIndex = Convert.ToInt32(fileSettings.stopbits);
                cmbBxProtocol.SelectedIndex = Convert.ToInt32(fileSettings.mode);

                cmbBxBaudrate1.SelectedIndex = Convert.ToInt32(fileSettings.baudrate);
                cmbBxDataLength1.SelectedIndex = Convert.ToInt32(fileSettings.datalength);
                cmbBxParityBit1.SelectedIndex = Convert.ToInt32(fileSettings.parity);
                cmbBxStopBits1.SelectedIndex = Convert.ToInt32(fileSettings.stopbits);
                cmbBxProtocol1.SelectedIndex = Convert.ToInt32(fileSettings.mode);

                txtBx_TC_Address.Text = txtBx_PC_Address.Text;

                if (modObj != null)
                {
                    if (modObj.IsSerialPortOpen())
                    {
                        modObj.CloseSerialPort();
                    }
                }
                else
                {
                    modObj = new clsModbus();
                    modObj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
                }

            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "(R)Default settings file not found.";
                errorCode++;
            }
        }

        private void cmbBxBaudrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDefaultValues();
           // unsupportedSetting();
        }

        private void cmbBxParityBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDefaultValues();
        }

        private void cmbBxDataLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDefaultValues();
        }

        private void cmbBxStopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDefaultValues();
            unsupportedSetting();
        }

        private void cmbBxProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDefaultValues();
           // unsupportedSetting();
        }

        private void CheckDefaultValues()
        {
            if (!string.IsNullOrEmpty(txtBx_PC_Address.Text) && Convert.ToInt32(txtBx_PC_Address.Text) > 0 &&
                cmbBxBaudrate.SelectedIndex > -1 && cmbBxDataLength.SelectedIndex > -1 && cmbBxParityBit.SelectedIndex > -1 &&
                cmbBxProtocol.SelectedIndex > 0 && cmbBxStopBits.SelectedIndex > -1 &&
                !string.IsNullOrEmpty(txtBx_TC_Address.Text) && Convert.ToInt32(txtBx_TC_Address.Text) > 0 &&
                cmbBxBaudrate1.SelectedIndex > -1 && cmbBxDataLength1.SelectedIndex > -1 && cmbBxParityBit1.SelectedIndex > -1 &&
                cmbBxProtocol1.SelectedIndex > 0 && cmbBxStopBits1.SelectedIndex > -1 && errorCode < 1)
            {
                btnSet.Enabled = true;
            }
            else
            {
                btnSet.Enabled = false;
            }
        }

        private void unsupportedSetting()
        {
            if (cmbBxProtocol1.SelectedIndex == 2)
            {
                cmbBxDataLength1.SelectedIndex = 2;
            }

            if ((cmbBxParityBit1.SelectedIndex == 1) && (cmbBxDataLength1.SelectedIndex == 1) && (cmbBxStopBits1.SelectedIndex == 1))
            {
                cmbBxStopBits1.SelectedIndex = 2;
                
            }
            else if ((cmbBxParityBit1.SelectedIndex == 3) && (cmbBxDataLength1.SelectedIndex == 2) && (cmbBxStopBits1.SelectedIndex == 2))
            {
                cmbBxStopBits1.SelectedIndex = 1;
            }
            else if ((cmbBxParityBit1.SelectedIndex == 2) && (cmbBxDataLength1.SelectedIndex == 2) && (cmbBxStopBits1.SelectedIndex == 2))
            {
                cmbBxStopBits1.SelectedIndex = 1;
            }

            
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            //MessageBoxEx.Show(this, "Please Connect FLA0102TV Expansion & Power-ON to enable Auto Sync", "Communication Auto Sync Success", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            try
            {
                #region Port Settings
                string portNameN = SetValues.Set_PortName;
                int baudRateN = Convert.ToInt32(SetValues.Set_Baudrate);
                string parityN = SetValues.Set_parity;
                int bitsLengthN = SetValues.Set_BitsLength;
                int stopBitsN = Convert.ToInt32(SetValues.Set_StopBits);

                if (modObj == null)
                {
                    modObj = new clsModbus();
                    modObj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
                }
                else
                {
                    if (modObj.IsSerialPortOpen())
                        modObj.CloseSerialPort();
                }

              //  if (modObj.OpenSerialPort(portNameN, baudRateN, parityN, stopBitsN, bitsLengthN))
                {
                    //LogWriter.WriteToFile("Load() =>", "Port Opened"
                    //        , "OnlineMonitor");

                    string finalString = "";
                    string oLdNodeAddress = txtBx_PC_Address.Text;// Convert.ToInt32(txtBx_PC_Address.Text).ToString("X2").PadLeft(2, '0');
                    string newValue = txtBx_TC_Address.Text;// Convert.ToInt32(txtBx_TC_Address.Text).ToString("X2").PadLeft(2, '0');

                    finalString += cmbBxBaudrate1.SelectedItem + "-";
                    finalString += cmbBxDataLength1.SelectedItem + "-";
                    finalString += cmbBxParityBit1.SelectedItem + "-";
                    finalString += cmbBxStopBits1.SelectedItem;

                    if (cmbBxProtocol1.SelectedIndex == 1)
                    {
                        finalString += "-A";
                    }
                    else if (cmbBxProtocol1.SelectedIndex == 2)
                    {
                        finalString += "-R";
                    }

                    //need to USB Setting
                   // finalString += "-R";

                    if (TcMap.ContainsKey(finalString))
                    {
                        string val = Convert.ToInt16(newValue).ToString("X2").PadLeft(2, '0') + Convert.ToInt16(TcMap[finalString]).ToString("X2").PadLeft(2, '0');
                        //MessageBox.Show(val);
                        int b = Convert.ToInt32(cmbBxBaudrate.SelectedItem);
                        string p = Convert.ToString(cmbBxParityBit.SelectedItem);
                        int d = Convert.ToInt32(cmbBxDataLength.SelectedItem);
                        int s = Convert.ToInt32(cmbBxStopBits.SelectedItem);

                        th = new Thread(() =>
                        {
                            int res = CreateFrames(oLdNodeAddress, "06", "470B", val, false, SetValues.Set_PortName, b, p, d, s);

                            if (res == 1)
                            {
                                try
                                {
                                    XElement xEmp = XElement.Load(m_exePath1);

                                    var settingsDetails = from emps in xEmp.Elements("settings")
                                                          select emps;

                                    settingsDetails.First().Element("port").Value = Convert.ToString(portNo);
                                    string valBaud = "0";
                                    cmbBxBaudrate1.Invoke((Action)(() =>
                                    {
                                        valBaud = Convert.ToString(cmbBxBaudrate1.SelectedIndex);
                                        SetValues.Set_Baudrate = cmbBxBaudrate1.SelectedItem.ToString();
                                    }));
                                    settingsDetails.First().Element("baudrate").Value = valBaud;

                                    string valParity = "0";
                                    cmbBxParityBit1.Invoke((Action)(() =>
                                    {
                                        valParity = Convert.ToString(cmbBxParityBit1.SelectedIndex);
                                        SetValues.Set_parity = cmbBxParityBit1.SelectedItem.ToString();
                                    }));

                                    settingsDetails.First().Element("parity").Value = valParity;

                                    string valData = "0";
                                    cmbBxDataLength1.Invoke((Action)(() =>
                                    {
                                        valData = Convert.ToString(cmbBxDataLength1.SelectedIndex);
                                        SetValues.Set_BitsLength = Convert.ToInt32(cmbBxDataLength1.SelectedItem.ToString());
                                    }));
                                    settingsDetails.First().Element("datalength").Value = valData;

                                    string valStop = "0";
                                    cmbBxStopBits1.Invoke((Action)(() =>
                                    {
                                        valStop = Convert.ToString(cmbBxStopBits1.SelectedIndex);
                                        SetValues.Set_StopBits = cmbBxStopBits1.SelectedItem.ToString();
                                    }));
                                    settingsDetails.First().Element("stopbits").Value = valStop;

                                    string valProtocol = "0";
                                    cmbBxProtocol1.Invoke((Action)(() =>
                                    {
                                        valProtocol = Convert.ToString(cmbBxProtocol1.SelectedIndex);
                                        SetValues.Set_CommunicationProtocol = cmbBxProtocol1.SelectedItem.ToString();
                                    }));
                                    settingsDetails.First().Element("mode").Value = valProtocol;

                                    xEmp.Save(m_exePath1);

                                    string valNode = "0";
                                    txtBx_TC_Address.Invoke((Action)(() => { valNode = Convert.ToString(txtBx_TC_Address.Text); }));
                                    txtBx_PC_Address.Invoke((Action)(() => txtBx_PC_Address.Text = valNode));

                                    cmbBxBaudrate.Invoke((Action)(() =>
                                    {
                                        cmbBxBaudrate.SelectedIndex = Convert.ToInt32(valBaud);

                                    }));

                                    cmbBxDataLength.Invoke((Action)(() =>
                                    {
                                        cmbBxDataLength.SelectedIndex = Convert.ToInt32(valData);

                                    }));

                                    cmbBxParityBit.Invoke((Action)(() =>
                                    {
                                        cmbBxParityBit.SelectedIndex = Convert.ToInt32(valParity);

                                    }));

                                    cmbBxStopBits.Invoke((Action)(() =>
                                    {
                                        cmbBxStopBits.SelectedIndex = Convert.ToInt32(valStop);

                                    }));

                                    cmbBxProtocol.Invoke((Action)(() =>
                                    {
                                        cmbBxProtocol.SelectedIndex = Convert.ToInt32(valProtocol);

                                    }));

                                    string successMsg = "Updated ";

                                    if (chkBx_Autosync.Checked)
                                    {

                                        try
                                        {
                                            if (SetValues.Set_BoolUSBdata != true)
                                            {
                                                Thread.Sleep(500);
                                                IWin32Window ownerr = this;
                                                //if (clicked == DialogResult.OK)
                                                //{
                                                int res1 = CreateFrames(newValue, "06", "1022", "0001", false,
                                                    SetValues.Set_PortName, Convert.ToInt32(SetValues.Set_Baudrate),
                                                    SetValues.Set_parity, SetValues.Set_BitsLength, Convert.ToInt32(SetValues.Set_StopBits));

                                                if (res1 == 1)
                                                {
                                                    this.Invoke((Action)(() =>
                                                    {
                                                        DialogResult clicked = MessageBoxEx.Show(ownerr, "Please Connect FLA0102TV Expansions & Re-Power ON to enable Auto Sync", "Communication Auto Sync Success", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                                    }));

                                                    successMsg += " and Auto Synced expansions ";
                                                }
                                            }
                                            else 
                                            {

                                                Thread.Sleep(500);
                                                IWin32Window ownerr = this;
                                                //if (clicked == DialogResult.OK)
                                                //{
                                                int res1 = CreateFramesRTUonly(newValue, "06", "1022", "0001", false,
                                                    SetValues.Set_PortName, Convert.ToInt32(SetValues.Set_Baudrate),
                                                    SetValues.Set_parity, SetValues.Set_BitsLength, Convert.ToInt32(SetValues.Set_StopBits));

                                                if (res1 == 1)
                                                {
                                                    this.Invoke((Action)(() =>
                                                    {
                                                        DialogResult clicked = MessageBoxEx.Show(ownerr, "Please Connect FLA0102TV Expansions & Re-Power ON to enable Auto Sync", "Communication Auto Sync Success", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                                    }));

                                                    successMsg += " and Auto Synced expansions ";
                                                }

                                            }
                                        }
                                        catch (Exception em)
                                        {

                                        }
                                    }

                                    lblMsg.Invoke((Action)(() =>
                                    {
                                        Thread.Sleep(400);
                                        lblMsg.Visible = true;
                                        lblMsg.Text = successMsg + " successfully.";
                                       
                                    }));



                                }
                                catch (Exception ex)
                                {
                                    lblMsg.Invoke((Action)(() =>
                                    {
                                        lblMsg.Visible = true;
                                        lblMsg.Text = "(W)Default settings file not found.";
                                        errorCode++;
                                    }));
                                }
                            }
                            else if (res == 2)
                            {
                                lblMsg.Invoke((Action)(() =>
                                {
                                    lblMsg.Visible = true;
                                    lblMsg.Text = "Something went wrong. Or Check connection";
                                }));

                            }
                            else if (res == 3)
                            {
                                lblMsg.Invoke((Action)(() =>
                                {
                                    lblMsg.Visible = true;
                                    lblMsg.Text = "Something went wrong.";
                                }));
                            }
                        });
                        th.IsBackground = true;
                        th.Start();
                    }
                    else
                    {
                        lblMsg.Invoke((Action)(() =>
                        {
                            lblMsg.Visible = true;
                            lblMsg.Text = "(K)Error occurred.";
                        }));
                    }

                }
                //else
                //{
                //    lblMsg.Visible = true;
                //    lblMsg.Text = "Connection error.";
                //}
                #endregion
            }
            catch (Exception ex)
            {
                this.Invoke((Action)(() =>
                                                {
                                                    MessageBoxEx.Show(ex.Message);
                                                }));
            }
            finally
            {
               
                //if (modObj != null)
                //{
                //    if (modObj.IsSerialPortOpen())
                //        modObj.CloseSerialPort();
                //}
            }
        }

        private void singlecmdtxt()
        {

        }

        private DataSet CreateDynamicDataSet()
        {
            DataSet ds = new DataSet("Default");

            DataTable stdTable = new DataTable("settings");
            DataColumn col1 = new DataColumn("port");
            DataColumn col2 = new DataColumn("baudrate");
            DataColumn col3 = new DataColumn("parity");
            DataColumn col4 = new DataColumn("datalength");
            DataColumn col5 = new DataColumn("stopbits");
            DataColumn col6 = new DataColumn("mode");

            stdTable.Columns.Add(col1);
            stdTable.Columns.Add(col2);
            stdTable.Columns.Add(col3);
            stdTable.Columns.Add(col4);
            stdTable.Columns.Add(col5);
            stdTable.Columns.Add(col6);

            ds.Tables.Add(stdTable);

            //Add student Data to the table  
            DataRow newRow; newRow = stdTable.NewRow();

            newRow["port"] = portNo;
            newRow["baudrate"] = Convert.ToString(cmbBxBaudrate1.SelectedIndex);
            newRow["parity"] = Convert.ToString(cmbBxParityBit1.SelectedIndex);
            newRow["datalength"] = Convert.ToString(cmbBxDataLength1.SelectedIndex);
            newRow["stopbits"] = Convert.ToString(cmbBxStopBits1.SelectedIndex);
            newRow["mode"] = Convert.ToString(cmbBxProtocol1.SelectedIndex);

            stdTable.Rows.Add(newRow);
            ds.AcceptChanges();
            return ds;
        }

        private int CreateFrames(string nodeAddress, string functionCode, string regAddress, string wordCount, bool read,
            string portNameN, int baudRateN, string parityN, int bitsLengthN, int stopBitsN)
        {
            byte[] RecieveData = null;
            try
            {
                //if (modObj != null)
                //{
                //    if (modObj.IsSerialPortOpen())
                //    {
                //        modObj.CloseSerialPort();
                //    }
                //}
                //else
                //{
                //    modObj = new clsModbus();
                //    modObj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
                //}

               // if (!modObj.IsSerialPortOpen())
                {
                    #region Port Settings
                    //string portNameN = SetValues.Set_PortName;
                    //int baudRateN = Convert.ToInt32(SetValues.Set_Baudrate);
                    //string parityN = SetValues.Set_parity;
                    //int bitsLengthN = SetValues.Set_BitsLength;
                    //int stopBitsN = Convert.ToInt32(SetValues.Set_StopBits);
                    int readTimeOut = baudRateN == 2400 ? 1000 : 100;
                    if (modObj.OpenSerialPort(portNameN, baudRateN, parityN, stopBitsN, bitsLengthN, readTimeOut))
                    {
                       
                    }
                    else
                    {
                        lblMsg.Visible = true;
                        lblMsg.Text = "Connection error.";
                    }
                    #endregion
                }

              //  if (modObj.IsSerialPortOpen())
                {
                    if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                    {
                        if ((SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1) && SetValues.Set_CommType == 1) //|| SetValues.Set_CommunicationProtocolindex == 1
                        {
                               Thread.Sleep(700);
                            RecieveData = modObj.AscFrame(nodeAddress,
                                functionCode, regAddress, wordCount.PadLeft(4, '0'));
                            
                            if(baudRateN == 9600 && bitsLengthN == 8)
                            {
                                
                                    return 1;
                                
                            }
                            if (RecieveData != null && RecieveData.Length > 0)
                            {
                                string funcCode = System.Text.Encoding.UTF8.GetString(
                                                ExtractByteArray(RecieveData, 2, 3));

                                // check if received function code is empty or not
                                if (!string.IsNullOrEmpty(funcCode))
                                {
                                    // check if function code sent and received are same
                                    if (funcCode.Equals(functionCode) || (baudRateN == 9600 && bitsLengthN == 7))
                                    {
                                        return 1;
                                    }
                                }
                                else
                                {
                                    return 2;
                                }
                            }

                        }
                        else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2) //|| SetValues.Set_CommunicationProtocolindex == 2
                        {
                            //need to optimize
                            Thread.Sleep(700);
                            RecieveData = modObj.RtuFrame(nodeAddress,
                                functionCode, regAddress,
                               wordCount.PadLeft(4, '0'), SetValues.Set_Baudrate); // Convert.ToInt32(nodeAddress).ToString("X").PadLeft(4, '0')

                            if (RecieveData != null && RecieveData.Length > 0)
                            {
                              
                                string funcCode = BitConverter.ToString(ExtractByteArray(RecieveData, 1, 1)).Replace("-", "");
                               string result = modObj.DisplayFrame(RecieveData);
                               // MessageBox.Show(result);
                                // check if received function code is empty or not
                                if (!string.IsNullOrEmpty(funcCode))
                                {
                                    // check if function code sent and received are same
                                    if (funcCode.Equals(functionCode))
                                    {
                                        return 1;

                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return 3;
            }
            return 2;
        }


        private int CreateFramesRTUonly(string nodeAddress, string functionCode, string regAddress, string wordCount, bool read,
          string portNameN, int baudRateN, string parityN, int bitsLengthN, int stopBitsN)
        {
            byte[] RecieveData = null;
            try
            {
                //if (modObj != null)
                //{
                //    if (modObj.IsSerialPortOpen())
                //    {
                //        modObj.CloseSerialPort();
                //    }
                //}
                //else
                //{
                //    modObj = new clsModbus();
                //    modObj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
                //}

                // if (!modObj.IsSerialPortOpen())
                {
                    #region Port Settings
                    //string portNameN = SetValues.Set_PortName;
                    //int baudRateN = Convert.ToInt32(SetValues.Set_Baudrate);
                    //string parityN = SetValues.Set_parity;
                    //int bitsLengthN = SetValues.Set_BitsLength;
                    //int stopBitsN = Convert.ToInt32(SetValues.Set_StopBits);
                    int readTimeOut = baudRateN == 2400 ? 1000 : 100;
                    if (modObj.OpenSerialPort(portNameN, baudRateN, parityN, stopBitsN, bitsLengthN, readTimeOut))
                    {

                    }
                    else
                    {
                        lblMsg.Visible = true;
                        lblMsg.Text = "Connection error.";
                    }
                    #endregion
                }

                //  if (modObj.IsSerialPortOpen())
                {
                    if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                    {

                        #region comment test
                        //if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii") //|| SetValues.Set_CommunicationProtocolindex == 1
                        //{
                        //    Thread.Sleep(700);
                        //    RecieveData = modObj.AscFrame(nodeAddress,
                        //        functionCode, regAddress, wordCount.PadLeft(4, '0'));

                        //    if (baudRateN == 9600 && bitsLengthN == 8)
                        //    {

                        //        return 1;

                        //    }
                        //    if (RecieveData != null && RecieveData.Length > 0)
                        //    {
                        //        string funcCode = System.Text.Encoding.UTF8.GetString(
                        //                        ExtractByteArray(RecieveData, 2, 3));

                        //        // check if received function code is empty or not
                        //        if (!string.IsNullOrEmpty(funcCode))
                        //        {
                        //            // check if function code sent and received are same
                        //            if (funcCode.Equals(functionCode) || (baudRateN == 9600 && bitsLengthN == 7))
                        //            {
                        //                return 1;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            return 2;
                        //        }
                        //    }

                        //}
                        //else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu") //|| SetValues.Set_CommunicationProtocolindex == 2
                        #endregion
                        {
                            //need to optimize
                            Thread.Sleep(700);
                            RecieveData = modObj.RtuFrame(nodeAddress,
                                functionCode, regAddress,
                               wordCount.PadLeft(4, '0'), SetValues.Set_Baudrate); // Convert.ToInt32(nodeAddress).ToString("X").PadLeft(4, '0')

                            if (RecieveData != null && RecieveData.Length > 0)
                            {

                                string funcCode = BitConverter.ToString(ExtractByteArray(RecieveData, 1, 1)).Replace("-", "");
                                string result = modObj.DisplayFrame(RecieveData);
                                // MessageBox.Show(result);
                                // check if received function code is empty or not
                                if (!string.IsNullOrEmpty(funcCode))
                                {
                                    // check if function code sent and received are same
                                    if (funcCode.Equals(functionCode))
                                    {
                                        return 1;

                                    }
                                    else
                                    {
                                        return 2;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return 3;
            }
            return 2;
        }

        private static byte[] ExtractByteArray(byte[] RecieveData, int size, int startPostition)
        {
            byte[] sizeBytes = new byte[size];
            if (RecieveData != null && RecieveData.Length > 0)
            {
                Array.Copy(RecieveData, startPostition, sizeBytes, 0, size);
            }
            return sizeBytes;
        }

        private void TcSettingsThread(object obj)
        {

        }

        private void txtBx_PC_Address_TextChanged(object sender, EventArgs e)
        {
            //if (System.Text.RegularExpressions.Regex.IsMatch(txtBx_PC_Address.Text, "[^0-9]"))
            //{
            //    MessageBoxEx.Show("Please enter only numbers.");
            //    txtBx_PC_Address.Text = txtBx_PC_Address.Text.Remove(txtBx_PC_Address.Text.Length - 1);
            //}
            //else
            //{
            //    CheckDefaultValues();
            //}
            lblMsg.Text = "";
            if (BasicProperties.IsDigitsOnly(txtBx_PC_Address.Text))
            {
                lblMsg.Text = "";
                CheckDefaultValues();
            }
            else
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Address should only be integers.";
            }
        }

        private void txtBx_TC_Address_TextChanged(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            if (BasicProperties.IsDigitsOnly(txtBx_TC_Address.Text))
            {
                lblMsg.Text = "";
                CheckDefaultValues();
            }
            else
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Address should only be integers.";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (th != null && th.IsAlive)
                {
                    th.Abort();
                    th = null;
                }
                if (modObj != null)
                {
                    if (modObj.IsSerialPortOpen())
                    {
                        modObj.CloseSerialPort();
                        modObj = null;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((Action)(() =>
                                                {
                                                    MessageBoxEx.Show(ex.Message);
                                                }));
            }
            finally
            {
                th = null;
                modObj = null;
            }


            this.Close();
        }

        private void cmbBxBaudrate1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDefaultValues();
        }

        private void cmbBxParityBit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDefaultValues();
            unsupportedSetting();
        }

        private void cmbBxDataLength1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDefaultValues();
            unsupportedSetting();
        }

        private void cmbBxStopBits1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDefaultValues();
            unsupportedSetting();
        }

        private void cmbBxProtocol1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckDefaultValues();
            unsupportedSetting();
        }

        private void TcSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void TcSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (th != null)
                {
                    th.Abort();
                    th = null;
                }

                if (modObj != null)
                {
                    if (modObj.IsSerialPortOpen())
                    {
                        modObj.CloseSerialPort();
                        modObj = null;
                    }
                }

                this.Close();
            }
            catch (Exception ex)
            {
                this.Invoke((Action)(() =>
                                                {
                                                    MessageBoxEx.Show(ex.Message);
                                                }));
            }
            finally
            {
                th = null;
                modObj = null;
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void chkBx_Autosync_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    class FileSettings
    {
        public string port { get; set; }
        public string baudrate { get; set; }
        public string parity { get; set; }
        public string datalength { get; set; }
        public string mode { get; set; }
        public string stopbits { get; set; }
    }
}
