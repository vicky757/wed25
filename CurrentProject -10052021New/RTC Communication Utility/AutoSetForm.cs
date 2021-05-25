using ClassList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RTC_Communication_Utility
{
    public partial class AutoSetForm : Form
    {
        public AutoSetForm()
        {
            InitializeComponent();
            SetValues.Set_Form = "AutoSet";

        }

        static string[] address = {  "--Select--", "01-10", "11-20", "21-30", "31-40", "41-50",  
                                     "51-60",  "61-70", "71-80", "81-90", "91-A0", "A1-B0", 
                                     "B1-C0",  "C1-D0", "D1-E0", "E1-F0","F1-F7"};
        static string[] baudRates = { "--Select--", "2400", "4800", "9600", "19200", "38400", "115200" };
        static string[] dataLengths = { "--Select--", "8" };
        static string[] parity = { "--Select--", "None", "Odd", "Even" };
        static string[] stopBits = { "--Select--", "1", "2" };
        static string[] protocol = { "--Select--", "ASCII", "RTU" };
        Thread _thread = null;
        string m_exePath1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
             "\\DefaultSettings.xml";

        DataTable table;
        //ModbusRTU modbusRTUobj = new ModbusRTU();
        clsModbus modbusRTUobj;
        bool valid = false;
        string stringHex = "";
        string stringHexToStore = "";
        int ResponseCnt = 1;



        private DataTable GetResultsTable()
        {
            table = new DataTable();
            table.Columns.Add("#");
            table.Columns.Add("Address");
            table.Columns.Add("Communication Protocol");

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = table;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;

            return table;
        }

        void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            int index = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            string address = row.Cells[1].Value.ToString();

            string[] parameters = new string[4];
            parameters = row.Cells[2].Value.ToString().Split(',');
            string baudrateIndex = "0"; string parityIndex = "0"; string bitsLengthIndex = "0"; string stopbitsIndex = "0"; string protocolIndex = "0";
            if (parameters.Count() > 0)
            {
                SetValues.Set_Baudrate = parameters[0];
                SetValues.Set_parity = parameters[1];
                SetValues.Set_StopBits = parameters[2];
                SetValues.Set_BitsLength = Convert.ToInt32(parameters[3]);
                SetValues.Set_CommunicationProtocol = protocol[parameters[4] == "1" ? 1 : parameters[4] == "2" ? 2 : 0];

                baudrateIndex = Array.IndexOf(baudRates, parameters[0]).ToString();
                parityIndex = Array.IndexOf(parity, parameters[1]).ToString();
                stopbitsIndex = Array.IndexOf(stopBits, parameters[2]).ToString();
                bitsLengthIndex = Array.IndexOf(dataLengths, parameters[3]).ToString();
                protocolIndex = parameters[4] == "1" ? "1" : parameters[4] == "2" ? "2" : "0";

                valid = true;
            }
            ReadWriteFile("1", baudrateIndex, parityIndex, bitsLengthIndex, stopbitsIndex, protocolIndex);

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (_thread != null)
                {
                    if (_thread.IsAlive)
                    {
                        _thread.Abort();
                    }
                }

                if (modbusRTUobj != null)
                {
                    if (modbusRTUobj.IsSerialPortOpen())
                    {
                        //modbusRTUobj.Port_Close();
                        modbusRTUobj.CloseSerialPort();
                    }
                }
                dataGridView1.DataSource = null;
                //if (dataGridView1.Rows.Count > 0)
                //{
                //    dataGridView1.Rows.Clear();
                //    dataGridView1.Refresh();
                //}

                btnStart.Enabled = false;
                btnSet.Enabled = true;

                //if (modbusRTUobj.OpenCOMPort(SetValues.Set_PortName, Convert.ToInt32(SetValues.Set_Baudrate),
                //    SetValues.Set_parity, Convert.ToInt32(SetValues.Set_StopBits), SetValues.Set_BitsLength))
                if (modbusRTUobj.OpenSerialPort(SetValues.Set_PortName, Convert.ToInt32(SetValues.Set_Baudrate),
                SetValues.Set_parity, Convert.ToInt32(SetValues.Set_StopBits), SetValues.Set_BitsLength))
                {
                    _thread = new Thread(new ThreadStart(MyThread));
                    _thread.IsBackground = true;
                    _thread.Start();
                }
                else
                {
                    btnStart.Enabled = true;
                    btnSet.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("1" + ex.Message);
            }

        }

        public void MyThread()
        {
            lblMsg.Invoke((Action)(() => lblMsg.Text = "Searching.."));

            try
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                if (cmbBxAddress.SelectedIndex == 0)
                {
                    if (cmbBxBaudrate.SelectedIndex == 0)
                    {
                        for (int x = 1; x <= cmbBxBaudrate.Items.Count; x++)
                        {
                            SetValues.Set_Baudrate = TakeBaudrate(x);

                            if (cmbBxParity.SelectedIndex == 0)
                            {
                                for (int y = 1; y <= cmbBxParity.Items.Count; y++)
                                {
                                    SetValues.Set_parity = TakeParity(y);
                                    SelectStopbitsForAddresses();
                                }
                            }
                            else
                            {
                                SetValues.Set_parity = cmbBxParity.Text;
                                SelectStopbitsForAddresses();
                            }
                        }
                    }
                    else
                    {
                        SetValues.Set_Baudrate = cmbBxBaudrate.Text;
                        if (cmbBxParity.SelectedIndex == 0)
                        {
                            for (int y = 1; y < cmbBxParity.Items.Count; y++)
                            {
                                SetValues.Set_parity = TakeParity(y);
                                SelectStopbitsForAddresses();
                            }
                        }
                        else
                        {
                            SetValues.Set_parity = cmbBxParity.Text;
                            SelectStopbitsForAddresses();
                        }
                    }

                }
                else
                {
                    switch (cmbBxAddress.SelectedIndex)
                    {
                        case 1:
                            stringHexToStore = "00";
                            break;
                        case 2:
                            stringHexToStore = "10";
                            break;
                        case 3:
                            stringHexToStore = "20";
                            break;
                        case 4:
                            stringHexToStore = "30";
                            break;
                        case 5:
                            stringHexToStore = "40";
                            break;
                        case 6:
                            stringHexToStore = "50";
                            break;
                        case 7:
                            stringHexToStore = "60";
                            break;
                        case 8:
                            stringHexToStore = "70";
                            break;
                        case 9:
                            stringHexToStore = "80";
                            break;
                        case 10:
                            stringHexToStore = "90";
                            break;
                        case 11:
                            stringHexToStore = "A0";
                            break;
                        case 12:
                            stringHexToStore = "B0";
                            break;
                        case 13:
                            stringHexToStore = "C0";
                            break;
                        case 14:
                            stringHexToStore = "D0";
                            break;
                        case 15:
                            stringHexToStore = "E0";
                            break;
                        case 16:
                            stringHexToStore = "F0";
                            break;
                    }
                    ProtocolForWithoutselectingAddresses();
                }


                btnSet.Enabled = true;
                btnStart.Enabled = true;
                lblMsg.Invoke((Action)(() => lblMsg.Text = ""));
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("AutoSetForm: ", ex.StackTrace, "DTC_ErrorLog");
                MessageBox.Show("2" + ex.Message);
            }
            finally
            {
                if (modbusRTUobj != null)
                {
                    if (modbusRTUobj.IsSerialPortOpen())
                    {
                        //modbusRTUobj.Port_Close();
                        modbusRTUobj.CloseSerialPort();
                    }
                }
            }
        }

        private void ProtocolForWithoutselectingAddresses()
        {
            if (cmbBxBaudrate.SelectedIndex == 0)
            {
                for (int x = 1; x < cmbBxBaudrate.Items.Count; x++)
                {
                    SetValues.Set_Baudrate = TakeBaudrate(x);

                    if (cmbBxParity.SelectedIndex == 0)
                    {
                        for (int y = 1; y < cmbBxParity.Items.Count; y++)
                        {
                            SetValues.Set_parity = TakeParity(y);
                            SelectStopbits();
                        }
                    }
                    else
                    {
                        SetValues.Set_parity = cmbBxParity.Text;
                        SelectStopbits();
                    }
                }
            }
            else
            {
                SetValues.Set_Baudrate = cmbBxBaudrate.Text;
                if (cmbBxParity.SelectedIndex == 0)
                {
                    for (int y = 1; y < cmbBxParity.Items.Count; y++)
                    {
                        SetValues.Set_parity = TakeParity(y);
                        SelectStopbits();
                    }
                }
                else
                {
                    SetValues.Set_parity = cmbBxParity.Text;
                    SelectStopbits();
                }
            }
        }

        private void SelectStopbitsForAddresses()
        {
            if (cmbBxStopbits.SelectedIndex == 0)
            {
                for (int z = 1; z < cmbBxStopbits.Items.Count; z++)
                {
                    SetValues.Set_StopBits = TakeStopBits(z);
                    if (cmbBxBitslength.SelectedIndex == 0)
                    {
                        for (int b = 1; b < cmbBxBitslength.Items.Count; b++)
                        {
                            SetValues.Set_BitsLength = TakeBitsLength(b);
                            Write16AddrsForSamePropertiesForAllAddresses();
                        }
                    }
                    else
                    {
                        int bitLen = cmbBxBitslength.SelectedIndex;
                        if (bitLen > 0)
                            SetValues.Set_BitsLength = Convert.ToInt32(cmbBxBitslength.SelectedItem.ToString());
                        else
                            SetValues.Set_BitsLength = Convert.ToInt32(cmbBxBitslength.Items[1].ToString());

                        Write16AddrsForSamePropertiesForAllAddresses();
                    }
                }
            }
            else
            {
                SetValues.Set_StopBits = cmbBxStopbits.Text.Substring(0, 1);

                if (cmbBxBitslength.SelectedIndex == 0)
                {
                    for (int b = 1; b < cmbBxStopbits.Items.Count; b++)
                    {
                        SetValues.Set_BitsLength = TakeBitsLength(b);
                        Write16AddrsForSamePropertiesForAllAddresses();
                    }
                }
                else
                {
                    int bitLen = cmbBxBitslength.SelectedIndex;

                    if (bitLen > 0)
                        SetValues.Set_BitsLength = Convert.ToInt32(cmbBxBitslength.SelectedItem.ToString());
                    else
                        SetValues.Set_BitsLength = Convert.ToInt32(cmbBxBitslength.Items[1].ToString());
                    Write16AddrsForSamePropertiesForAllAddresses();
                }
            }
        }

        private void SelectStopbits()
        {
            if (cmbBxStopbits.SelectedIndex == 0)
            {
                for (int z = 1; z < cmbBxStopbits.Items.Count; z++)
                {
                    SetValues.Set_StopBits = TakeStopBits(z);
                    if (cmbBxBitslength.SelectedIndex == 0)
                    {
                        for (int b = 1; b < cmbBxBitslength.Items.Count; b++)
                        {
                            SetValues.Set_BitsLength = TakeBitsLength(b);
                            stringHex = stringHexToStore;
                            Write16AddrsForSameProperties();
                        }
                    }
                    else
                    {
                        int bitLen = cmbBxBitslength.SelectedIndex;
                        if (bitLen == 1 || bitLen == 0)
                            SetValues.Set_BitsLength = 7;
                        else if (bitLen == 2)
                            SetValues.Set_BitsLength = 8;

                        stringHex = stringHexToStore;
                        Write16AddrsForSameProperties();
                    }
                }
            }
            else
            {
                SetValues.Set_StopBits = cmbBxStopbits.Text.Substring(0, 1);

                if (cmbBxBitslength.SelectedIndex == 0)
                {
                    for (int b = 1; b < cmbBxBitslength.Items.Count; b++)
                    {
                        SetValues.Set_BitsLength = TakeBitsLength(b);
                        stringHex = stringHexToStore;
                        Write16AddrsForSameProperties();
                    }
                }
                else
                {
                    int bitLen = cmbBxBitslength.SelectedIndex;
                    if (bitLen == 1 || bitLen == 0)
                        SetValues.Set_BitsLength = 7;
                    else if (bitLen == 2)
                        SetValues.Set_BitsLength = 8;

                    stringHex = stringHexToStore;
                    Write16AddrsForSameProperties();
                }
            }
        }

        private int TakeBitsLength(int index)
        {
            return Convert.ToInt32(cmbBxBitslength.Items[index].ToString());
        }

        private string TakeBaudrate(int index)
        {
            return cmbBxBaudrate.Items[index].ToString();
        }

        private string TakeParity(int index)
        {
            return cmbBxParity.Items[index].ToString();
        }

        private string TakeStopBits(int index)
        {
            return cmbBxStopbits.Items[index].ToString();
        }

        private void Write16AddrsForSamePropertiesForAllAddresses()
        {
            //byte[] GetASKdata = null;
            try
            {
                stringHex = "00";

                for (int j = 1; j <= 256; j++)
                {
                    int intFromHex = int.Parse(stringHex, System.Globalization.NumberStyles.HexNumber) + 1;
                    string UntAddrs = intFromHex.ToString("X");
                    stringHex = UntAddrs;

                    if (UntAddrs.Length == 1)
                        UntAddrs = "0" + UntAddrs;

                    SetValues.Set_UnitAddress = UntAddrs;
                    SetValues.Set_CommandType = "03";// "03";
                    SetValues.Set_RegAddress = "1000";// "4701";
                    SetValues.Set_WordCount = "0001";

                    byte[] RecieveData = null;
                    int ii = 0;
                    if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                    {

                        if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
                        {
                            var arr = modbusRTUobj.AscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                                SetValues.Set_RegAddress, SetValues.Set_WordCount);

                            string str = SetValues.Set_Baudrate + "," + SetValues.Set_BitsLength + "," +
                                SetValues.Set_parity + "," + SetValues.Set_StopBits;

                            lblStatus.Invoke((Action)(() => lblStatus.Text = str));

                            if (arr != null)
                            {
                                if (arr.Length > 0)
                                {
                                    RecieveData = new byte[arr.Length];
                                    RecieveData = arr;
                                    ii = 1;
                                }
                            }
                        }
                        else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
                        {
                            var arr = modbusRTUobj.RtuFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                                SetValues.Set_RegAddress, SetValues.Set_WordCount);
                            if (arr != null)
                            {
                                if (arr.Length > 0)
                                {
                                    RecieveData = new byte[arr.Length];
                                    RecieveData = arr;
                                    ii = 2;
                                }
                            }
                        }
                    }

                    if (RecieveData != null)
                    {
                        string result = modbusRTUobj.DisplayFrame(RecieveData);


                        //modbusRTUobj.MakeAscFrameCombinations(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                        //    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                        //GetASKdata = modbusRTUobj.Read_AscDataRegisterCombinations(1);

                        //if (GetASKdata != null)
                        //{
                        //    if (GetASKdata[0] != 0)
                        //    {
                        if (!string.IsNullOrEmpty(result))
                        {
                            table.Rows.Add(new Object[] { ResponseCnt
                                , SetValues.Set_UnitAddress,
                                                SetValues.Set_Baudrate + "," + SetValues.Set_parity + "," +
                                                SetValues.Set_StopBits + "," + SetValues.Set_BitsLength+ "," +ii});

                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = table;
                            dataGridView1.Columns[0].Width = 50;
                            dataGridView1.Columns[2].Width = 120;
                            ResponseCnt++;
                        }
                        //  }
                        //}

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("3" + ex.Message);
            }
        }

        private void Write16AddrsForSameProperties()
        {
            //byte[] GetASKdata = null;
            try
            {
                for (int j = 1; j <= 16; j++)
                {
                    int intFromHex = int.Parse(stringHex, System.Globalization.NumberStyles.HexNumber) + 1;
                    string UntAddrs = intFromHex.ToString("X");
                    stringHex = UntAddrs;
                    if (UntAddrs.Length == 1)
                        UntAddrs = "0" + UntAddrs;

                    SetValues.Set_UnitAddress = UntAddrs;
                    if (j == 16)
                        SetValues.Set_UnitAddress = "10";
                    SetValues.Set_CommandType = "03";//"03";
                    SetValues.Set_RegAddress = "1000";//"4701";
                    SetValues.Set_WordCount = "0001";

                    byte[] RecieveData = null;
                    int ii = 0;
                    if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                    {
                        if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
                        {
                            string str = SetValues.Set_Baudrate + "," + SetValues.Set_BitsLength + "," +
                               SetValues.Set_parity + "," + SetValues.Set_StopBits;

                            lblStatus.Invoke((Action)(() => lblStatus.Text = str));

                            var arr = modbusRTUobj.AscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                                SetValues.Set_RegAddress, SetValues.Set_WordCount);
                            if (arr != null)
                            {
                                if (arr.Length > 0)
                                {
                                    RecieveData = new byte[arr.Length];
                                    RecieveData = arr;
                                    ii = 1;
                                }
                            }
                        }
                        else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
                        {
                            var arr = modbusRTUobj.RtuFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                                SetValues.Set_RegAddress, SetValues.Set_WordCount);
                            if (arr != null)
                            {
                                if (arr.Length > 0)
                                {
                                    RecieveData = new byte[arr.Length];
                                    RecieveData = arr;
                                    ii = 2;
                                }
                            }
                        }
                    }

                    if (RecieveData != null)
                    {
                        string result = modbusRTUobj.DisplayFrame(RecieveData);

                        //modbusRTUobj.MakeAscFrameCombinations(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                        //     SetValues.Set_RegAddress, SetValues.Set_WordCount);

                        //GetASKdata = modbusRTUobj.Read_AscDataRegisterCombinations(1);

                        //if (GetASKdata != null)
                        //{
                        //    if (GetASKdata[0] != 0)
                        //    {
                        if (!string.IsNullOrEmpty(result))
                        {
                            table.Rows.Add(new Object[] { ResponseCnt, SetValues.Set_UnitAddress,
                                                SetValues.Set_Baudrate + "," + SetValues.Set_parity + "," +
                                                SetValues.Set_StopBits + "," + SetValues.Set_BitsLength+ "," +ii});

                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = table;
                            dataGridView1.Columns[0].Width = 50;
                            dataGridView1.Columns[2].Width = 120;
                            ResponseCnt++;
                        }
                        //}
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("4" + ex.Message);
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if (valid)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select ");
            }

        }

        public void Reset()
        {
            try
            {
                lblMsg.Invoke((Action)(() => lblMsg.Text = ""));
                //modbusRTUobj.Port_Close();
                modbusRTUobj.CloseSerialPort();
                ResponseCnt = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("5" + ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (_thread != null)
                {
                    if (_thread.IsAlive)
                    {
                        _thread.Abort();
                    }
                    _thread = null;
                }

                btnStart.Enabled = true;
                btnSet.Enabled = false;

                if (modbusRTUobj != null)
                {
                    if (modbusRTUobj.IsSerialPortOpen())
                    {
                        //modbusRTUobj.Port_Close();
                        modbusRTUobj.CloseSerialPort();
                    }

                    modbusRTUobj = null;
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("6" + ex.Message);
            }
        }

        private void cmbBxAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
        }

        private void cmbBxBaudrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxBaudrate.SelectedIndex == 0)
                SetValues.Set_Baudrate = cmbBxBaudrate.Items[1].ToString();
            else
                SetValues.Set_Baudrate = cmbBxBaudrate.SelectedItem.ToString();

            btnStart.Enabled = true;
        }

        private void cmbBxBitslength_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxBitslength.SelectedIndex == 0)
                SetValues.Set_BitsLength = Convert.ToInt32(cmbBxBitslength.Items[1].ToString());
            else
                SetValues.Set_BitsLength = Convert.ToInt32(cmbBxBitslength.SelectedItem.ToString());

            btnStart.Enabled = true;
        }

        private void cmbBxStopbits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxStopbits.SelectedIndex == 0)
                SetValues.Set_StopBits = cmbBxStopbits.Items[1].ToString();
            else
                SetValues.Set_StopBits = cmbBxStopbits.SelectedItem.ToString();

            btnStart.Enabled = true;
        }

        private void cmbBxProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxBitslength.SelectedIndex == 0)
                SetValues.Set_CommunicationProtocol = cmbBxProtocol.Items[1].ToString();
            else
                SetValues.Set_CommunicationProtocol = cmbBxProtocol.SelectedItem.ToString();

            btnStart.Enabled = true;
        }

        private void cmbBxParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxParity.SelectedIndex == 0)
                SetValues.Set_parity = cmbBxParity.Items[1].ToString();
            else
                SetValues.Set_parity = cmbBxParity.SelectedItem.ToString();

            btnStart.Enabled = true;
        }

        private bool CheckIfFileExists()
        {
            return File.Exists(m_exePath1);
        }

        private bool ReadWriteFile(string cmbBxPorts, string cmbBxBaudrate, string cmbBxParityBit,
            string cmbBxDataLength, string cmbBxStopBits, string protocolIndex)
        {

            //check if file exists or not
            if (CheckIfFileExists())
            {
                //read settings from file
                LogWriter.WriteToFile("SetPCSettings.cs => SaveToFile() - Write", "Path: " +
                        m_exePath1, "RTC_Upgrade");

                //to check if file is empty or it contains data to modify
                try
                {
                    XElement xelement = XElement.Load(m_exePath1);
                    IEnumerable<XElement> settings = xelement.Elements();

                    if (settings.Count() > 0)
                    {
                        //modify
                        try
                        {
                            XNamespace empNM = "urn:lst-settings:settings";

                            XDocument xDoc = new XDocument(
                                        new XDeclaration("1.0", "UTF-16", null),
                                            new XElement("settings",
                                                new XElement("port", cmbBxPorts.ToString()),
                                                new XElement("baudrate", cmbBxBaudrate.ToString()),
                                                new XElement("parity", cmbBxParityBit.ToString()),
                                                new XElement("datalength", cmbBxDataLength.ToString()),
                                                new XElement("stopbits", cmbBxStopBits.ToString()),
                                                new XElement("mode", protocolIndex.ToString())
                                                ));

                            xDoc.Save(m_exePath1);
                            LogWriter.WriteToFile("SetPCSettings.cs => ReadWriteFile()", "Modify complete "
                                , "RTC_Upgrade");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
                    return false;
                }
            }

            return false;
        }

        public string[] getText()
        {
            string[] parameters = new string[5];
            parameters[0] = SetValues.Set_Baudrate;
            parameters[1] = SetValues.Set_parity;
            parameters[2] = SetValues.Set_StopBits;
            parameters[3] = SetValues.Set_BitsLength.ToString();
            parameters[4] = SetValues.Set_CommunicationProtocol.ToString();

            return parameters;
        }

        private void AutoSetForm_Load_1(object sender, EventArgs e)
        {
            GetResultsTable();

            cmbBxAddress.DataSource = address;
            cmbBxBaudrate.DataSource = baudRates;
            cmbBxBitslength.DataSource = dataLengths;
            cmbBxParity.DataSource = parity;
            cmbBxStopbits.DataSource = stopBits;
            cmbBxProtocol.DataSource = protocol;

            modbusRTUobj = new clsModbus();
        }
    }

}
