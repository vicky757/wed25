using ClassList;
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
    public partial class Form11 : Form
    {
        static string[] address = {  "--Select--", "01-10", "11-20", "21-30", "31-40", "41-50",
                                     "51-60",  "61-70", "71-80", "81-90", "91-A0", "A1-B0",
                                     "B1-C0",  "C1-D0", "D1-E0", "E1-F0","F1-F7"};
        static string[] baudRates = { "--Select--", "4800", "9600", "19200", "115200", "38400", "2400" };//SG  
        static string[] dataLengths = { "--Select--", "7", "8" };
        static string[] parity = { "--Select--", "None", "Odd", "Even" };
        static string[] stopBits = { "--Select--", "1", "2" };
        static string[] protocol = { "--Select--", "ASCII", "RTU" };
        Thread _thread = null;
        string m_exePath1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
             "\\DefaultSettings.xml";
        
        DataTable table;
        clsModbus modbusRTUobj;
        bool valid = false;
        string stringHex = "";
        string stringHexToStore = "";
        int ResponseCnt = 1;
        cls_1 obj = null;




//  #region
//        private static SerialPort _serialPort;

//    private static string[] _serialPorts;

//    private static ManagementEventWatcher arrival;

//    private static ManagementEventWatcher removal;

//    static SerialPortService()
//    {
//        _serialPorts = GetAvailableSerialPorts();
//        MonitorDeviceChanges();
//    }

//    /// <summary>
//    /// If this method isn't called, an InvalidComObjectException will be thrown (like below):
//    /// System.Runtime.InteropServices.InvalidComObjectException was unhandled
//    ///Message=COM object that has been separated from its underlying RCW cannot be used.
//    ///Source=mscorlib
//    ///StackTrace:
//    ///     at System.StubHelpers.StubHelpers.StubRegisterRCW(Object pThis, IntPtr pThread)
//    ///     at System.Management.IWbemServices.CancelAsyncCall_(IWbemObjectSink pSink)
//    ///     at System.Management.SinkForEventQuery.Cancel()
//    ///     at System.Management.ManagementEventWatcher.Stop()
//    ///     at System.Management.ManagementEventWatcher.Finalize()
//    ///InnerException: 
//    /// </summary>
//    public static void CleanUp()
//    {
//        arrival.Stop();
//        removal.Stop();
//    }

//    public static event EventHandler<PortsChangedArgs> PortsChanged;

//    private static void MonitorDeviceChanges()
//    {
//        try
//        {
//            var deviceArrivalQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
//            var deviceRemovalQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");

//            arrival = new ManagementEventWatcher(deviceArrivalQuery);
//            removal = new ManagementEventWatcher(deviceRemovalQuery);

//            arrival.EventArrived += (o, args) => RaisePortsChangedIfNecessary(EventType.Insertion);
//            removal.EventArrived += (sender, eventArgs) => RaisePortsChangedIfNecessary(EventType.Removal);

//            // Start listening for events
//            arrival.Start();
//            removal.Start();
//        }
//        catch (ManagementException err)
//        {

//        }
//    }

//    private static void RaisePortsChangedIfNecessary(EventType eventType)
//    {
//        lock (_serialPorts)
//        {
//            var availableSerialPorts = GetAvailableSerialPorts();
//            if (!_serialPorts.SequenceEqual(availableSerialPorts))
//            {
//                _serialPorts = availableSerialPorts;
//                PortsChanged.Raise(null, new PortsChangedArgs(eventType, _serialPorts));
//            }
//        }
//    }

//    public static string[] GetAvailableSerialPorts()
//    {
//        return SerialPort.GetPortNames();
//    }
//}

//public enum EventType
//{
//    Insertion,
//    Removal,
//}

//public class PortsChangedArgs : EventArgs
//{
//    private readonly EventType _eventType;

//    private readonly string[] _serialPorts;

//    public PortsChangedArgs(EventType eventType, string[] serialPorts)
//    {
//        _eventType = eventType;
//        _serialPorts = serialPorts;
//    }

//    public string[] SerialPorts
//    {
//        get
//        {
//            return _serialPorts;
//        }
//    }

//    public EventType EventType
//    {
//        get
//        {
//            return _eventType;
//        }
//    }
//    #endregion

        Dictionary<int, List<string>> tags = new Dictionary<int, List<string>>();

        List<string> protocolList = new List<string>();
        List<string> addressList = new List<string>();
        List<string> baudRateList = new List<string>();
        List<string> parityList = new List<string>();
        List<string> bitLengthList = new List<string>();
        List<string> stopBitsList = new List<string>();
        public string comName { get; set; }
         
        public Form11()
        {
            InitializeComponent();
        }

        private void Form11_Load(object sender, EventArgs e)
        {
           
            if (modbusRTUobj == null)
            {
                modbusRTUobj = new clsModbus();
                modbusRTUobj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
            }

            if (obj == null)
            {
                obj = new cls_1();
            }
            
            cmbNodeAddress.DataSource = address;
            cmbBaudRate.DataSource = baudRates;
            cmbBitslength.DataSource = dataLengths;
            cmbParity.DataSource = parity;
            cmbStopBits.DataSource = stopBits;
            cmbProtocol.DataSource = protocol;

            obj.Comport = SetValues.Set_PortName;
            obj.Baudrate = SetValues.Set_Baudrate;
            obj.Parity = SetValues.Set_parity;
            obj.Stopbits = SetValues.Set_StopBits;
            obj.Bitslength = Convert.ToString(SetValues.Set_BitsLength);
            obj.Protocol = SetValues.Set_CommunicationProtocol;
            obj.ProtocalIndex = SetValues.Set_CommunicationProtocolindex;

            btnSet.Enabled = false;
            btnSearch.Visible = true;
            btnCancel.Visible = false;

            lblStatus1.Text = "";
            lblFrame.Text = "";
        }

        private void singlecmdtxt()
        {

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

        private void cmbProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            protocolList.Clear();

            if (cmbProtocol.SelectedIndex == 0 || cmbProtocol.SelectedIndex == -1)
            {
                for (int i = 1; i < cmbProtocol.Items.Count; i++)
                {
                    protocolList.Add(cmbProtocol.Items[i].ToString());
                }
            }
            else
            {
                protocolList.Add(cmbProtocol.SelectedItem.ToString());
            }
        }

        private void cmbNodeAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            addressList.Clear();

            if (cmbNodeAddress.SelectedIndex == 0 || cmbNodeAddress.SelectedIndex == -1)
            {
                for (int i = 1; i < cmbNodeAddress.Items.Count; i++)
                {
                    string[] group = cmbNodeAddress.Items[i].ToString().Split('-');
                    if (group.Length > 0)
                    {
                        for (short j = Convert.ToInt16(group[0], 16); j <= Convert.ToInt16(group[1], 16); j++)
                        {
                            addressList.Add(j.ToString("X2").PadLeft(2, '0'));
                        }
                    }
                }
            }
            else
            {
                string[] group = cmbNodeAddress.SelectedItem.ToString().Split('-');
                if (group.Length > 0)
                {
                    for (short i = Convert.ToInt16(group[0], 16); i <= Convert.ToInt16(group[1], 16); i++)
                    {
                        addressList.Add(i.ToString("X2").PadLeft(2, '0'));
                    }
                }
            }
        }

        private void cmbBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            baudRateList.Clear();

            if (cmbBaudRate.SelectedIndex == 0 || cmbBaudRate.SelectedIndex == -1)
            {
                for (int i = 1; i < cmbBaudRate.Items.Count; i++)
                {
                    baudRateList.Add(cmbBaudRate.Items[i].ToString());
                }
            }
            else
            {
                baudRateList.Add(cmbBaudRate.Items[cmbBaudRate.SelectedIndex].ToString());
            }
        }

        private void cmbParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            parityList.Clear();

            if (cmbParity.SelectedIndex == 0 || cmbParity.SelectedIndex == -1)
            {
                for (int i = 1; i < cmbParity.Items.Count; i++)
                {
                    parityList.Add(cmbParity.Items[i].ToString());
                }
            }
            else
            {
                parityList.Add(cmbParity.SelectedItem.ToString());
            }
        }

        private void cmbBitslength_SelectedIndexChanged(object sender, EventArgs e)
        {
            bitLengthList.Clear();

            if (cmbBitslength.SelectedIndex == 0 || cmbBitslength.SelectedIndex == -1)
            {
                for (int i = 1; i < cmbBitslength.Items.Count; i++)
                {
                    bitLengthList.Add(cmbBitslength.Items[i].ToString());
                }
            }
            else
            {
                bitLengthList.Add(cmbBitslength.SelectedItem.ToString());
            }
        }

        private void cmbStopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            stopBitsList.Clear();

            if (cmbStopBits.SelectedIndex == 0 || cmbStopBits.SelectedIndex == -1)
            {
                for (int i = 1; i < cmbStopBits.Items.Count; i++)
                {
                    stopBitsList.Add(cmbStopBits.Items[i].ToString());
                }
            }
            else
            {
                stopBitsList.Add(cmbStopBits.SelectedItem.ToString());
            }
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

        private bool CheckIfFileExists()
        {
            return File.Exists(m_exePath1);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Visible = false;
            btnCancel.Visible = true;
            try
            {
                _thread = null;

                if (modbusRTUobj != null)
                {
                    if (modbusRTUobj.IsSerialPortOpen())
                    {
                       modbusRTUobj.CloseSerialPort();
                    }
                    modbusRTUobj = null;
                }

                modbusRTUobj = new clsModbus();
               // modbusRTUobj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);


                table = null;

                //GetResultsTable();

                dataGridView1.Refresh();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
                ControlBox = false;
                if (_thread == null)
                {
                    _thread = new Thread(new ThreadStart(MyThread));
                    _thread.IsBackground = true;

                    _thread.Start();
                    //btnClose.Text = "Cancel";
                }
                else
                {
                    btnSearch.Enabled = true;
                    btnSet.Enabled = false;

                    //_thread.Abort();
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show("1" + ex.Message);
            }
        }

        private void MyThread()
        {

            int counter = 0;
            int MaxVal = 0;
            ResponseCnt = 1;
            int compaire = -1;
            // table.Clear();

            List<cls_1> outputs = new List<cls_1>();
           
            lblStatus1.Invoke((Action)(() => lblStatus1.Text = "Searching.."));

            MaxVal = stopBitsList.Count * bitLengthList.Count * parityList.Count *
                                                    baudRateList.Count * addressList.Count * protocolList.Count;
            //MessageBox.Show("MaxVal: " + MaxVal);
            progressBar1.Invoke((Action)(() => progressBar1.Maximum = MaxVal));

            try
            {
                if (_thread != null)
                {
                    if (_thread.IsAlive)
                    {
                        foreach (string address in addressList)
                        {
                            foreach (string baudRate in baudRateList)
                            {
                                foreach (string parity in parityList)
                                {
                                    foreach (string bitsLength in bitLengthList)
                                    {
                                        foreach (string stopBits in stopBitsList)
                                        {
                                            foreach (string protocol in protocolList)
                                            {

                                                //if (modbusRTUobj.IsSerialPortOpen())
                                                //{
                                                //    modbusRTUobj.CloseSerialPort();
                                                //}

                                                Thread.Sleep(150);

                                                obj.Comport = SetValues.Set_PortName;
                                                obj.Baudrate = baudRate;
                                                obj.Parity = parity;
                                                obj.Stopbits = stopBits;
                                                obj.Bitslength = bitsLength;
                                                obj.Protocol = protocol;

                                                string UntAddrs1 = Convert.ToInt32(address, 16).ToString();
                                                string UntAddrs = UntAddrs1.Length < 2 ? UntAddrs1.PadLeft(2, '0') : UntAddrs1;

                                                string strParams =
                                                    string.Format("{0},{1},{2},{3},{4},{5}H,{6}",
                                                    obj.Comport,
                                                    obj.Baudrate,
                                                    obj.Parity,
                                                    obj.Bitslength,
                                                    obj.Stopbits,
                                                    Convert.ToInt32(UntAddrs).ToString("X").PadLeft(2, '0'),
                                                    obj.Protocol);
                                               if(!(obj.Baudrate =="2400"))
                                                {
                                                 if (modbusRTUobj != null)
                                                 {
                                                    //if (modbusRTUobj.IsSerialPortOpen() == false)
                                                    {
                                                        if (modbusRTUobj.OpenSerialPort(obj.Comport,
                                                            Convert.ToInt32(obj.Baudrate),
                                                            obj.Parity,
                                                            Convert.ToInt32(obj.Stopbits),
                                                            Convert.ToInt32(obj.Bitslength),
                                                            100))
                                                        {
                                                            //MessageBox.Show("port opened");
                                                        }
                                                    }
                                                     string result = string.Empty;
                                                   if (modbusRTUobj.IsSerialPortOpen())
                                                    {
                                                        lblFrame.Invoke((Action)(() => lblFrame.Text = strParams));

                                                        progressBar1.Invoke((Action)(() => progressBar1.Value = counter++));

                                                        byte[] RecieveData = null;

                                                        //Send

                                                        System.Threading.Thread.Sleep(1500); 

                                                        if (obj.Protocol.Equals("ASCII"))
                                                        {
                                                          
                                                            RecieveData = modbusRTUobj.AscFrame(
                                                                Convert.ToInt32(UntAddrs).ToString("X").PadLeft(2, '0'),
                                                                "03", "4701", "0001");
                                                                try
                                                                {
                                                                    if (RecieveData != null && RecieveData.Length > 0)
                                                                    {
                                                                        char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();
                                                                        result = string.Join("", recdata);
                                                                        if (!string.IsNullOrEmpty(result))
                                                                        {
                                                                            if (RecieveData.Length > 10 && result.Contains("0302") && result.StartsWith(":"))
                                                                            {
                                                                                dataGridView1.Invoke((Action)(() =>
                                                                                {
                                                                                    dataGridView1.Rows.Add(
                                                                                        string.Format("{0}H",
                                                                                        Convert.ToInt32(UntAddrs).ToString("X").PadLeft(2, '0')
                                                                                      ),
                                                                                        string.Format("{0},{1},{2},{3},{4},{5}",
                                                                                        comName,
                                                                                        obj.Baudrate,
                                                                                        obj.Bitslength,
                                                                                        parity,
                                                                                        obj.Stopbits,
                                                                                        obj.Protocol
                                                                                        )
                                                                                    );
                                                                                })
                                                                             );

                                                                                ResponseCnt++;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                catch (Exception ae) { }
                                                            }
                                                        else if (obj.Protocol.Equals("RTU"))
                                                        {
                                                            RecieveData = modbusRTUobj.RtuFrame(UntAddrs, "03"
                                                                , "4701", "0001",baudRate);
                                                                try
                                                                {
                                                                    if (RecieveData != null && RecieveData.Length > 0)
                                                                    {
                                                                        result = modbusRTUobj.DisplayFrame(RecieveData);
                                                                        if (!string.IsNullOrEmpty(result))
                                                                        {
                                                                            //if (RecieveData.Length > 5 && result.StartsWith(":")) //&& result.Contains("0302")
                                                                            {
                                                                                dataGridView1.Invoke((Action)(() =>
                                                                                {
                                                                                    dataGridView1.Rows.Add(
                                                                                        string.Format("{0}H",
                                                                                        Convert.ToInt32(UntAddrs).ToString("X").PadLeft(2, '0')
                                                                                      ),
                                                                                        string.Format("{0},{1},{2},{3},{4},{5}",
                                                                                        comName,
                                                                                        obj.Baudrate,
                                                                                        obj.Bitslength,
                                                                                        parity,
                                                                                        obj.Stopbits,
                                                                                        obj.Protocol
                                                                                        )
                                                                                    );
                                                                                })
                                                                             );

                                                                                ResponseCnt++;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                catch (Exception ae) { }
                                                        }

                                                      
                                                    }
                                                }
                                            }
                                            }
                                        }

                                    }
                                }
                            }
                        }

                        progressBar1.Invoke((Action)(() => progressBar1.Value = MaxVal));
                        lblFrame.Invoke((Action)(() => lblFrame.Text = ""));
                        lblStatus1.Invoke((Action)(() => lblStatus1.Text = "Search Completed"));
                        this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { ControlBox = true; });
                        btnClose.Invoke((Action)(() => btnClose.Text = "Close"));
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("AutoDetectsettings: ", ex.StackTrace, "DTC_ErrorLog");
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            //Setting not set issue add close port function
            try
            {
                ControlBox = true;
                clsModbus mo = new clsModbus();
                //mo.ACloseSerialPort();
            }
            catch (Exception ae)
            {
 
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                //ControlBox = true;
                clsModbus mo = new clsModbus();
                //mo.ACloseSerialPort();
                CloseForm();
            }
            catch (Exception ae)
            {

            }
        }

        private void CloseForm()
        {
            try
            {


                if (btnClose.Text == "Close")
                {
                    if (modbusRTUobj != null)
                    {
                        if (modbusRTUobj.IsSerialPortOpen())
                        {
                            modbusRTUobj.CloseSerialPort();
                            modbusRTUobj = null;
                        }
                    }
                    this.Close();
                }
                else
                {
                    progressBar1.Invoke((Action)(() => progressBar1.Value = 0));
                    if (_thread != null)
                    {
                        if (_thread.IsAlive)
                        {
                            _thread.Abort();
                           // btnClose.Invoke((Action)(() => btnClose.Text = "Close"));
                            _thread = null;

                            if (lblStatus1.InvokeRequired)
                            {
                                lblStatus1.Invoke((Action)(() => lblStatus1.Text = ""));
                            }
                            else
                            {
                                lblStatus1.Text = "Search Abort";
                            }

                            if (lblFrame.InvokeRequired)
                            {
                                lblFrame.Invoke((Action)(() => lblFrame.Text = ""));
                            }
                            else
                            {
                                lblFrame.Text = "";
                            }

                        }
                    }

                    if (modbusRTUobj != null)
                    {
                        if (modbusRTUobj.IsSerialPortOpen())
                        {
                            modbusRTUobj.CloseSerialPort();
                            modbusRTUobj = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("AutoDetectsettings: ", ex.StackTrace, "DTC_ErrorLog");
            }
            finally
            {
                _thread = null;
                modbusRTUobj = null;
            }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex > -1 && e.ColumnIndex > -1)
                {
                    string address = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();                   

                    string[] parameters = new string[6];
                    parameters = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString().Split(',');

                    string baudrateIndex = "0"; string parityIndex = "0"; string bitsLengthIndex = "0"; string stopbitsIndex = "0"; string protocolIndex = "0";

                    if (parameters.Count() > 0)
                    {
                        SetValues.Set_Baudrate = parameters[1];
                        SetValues.Set_parity = parameters[3];
                        SetValues.Set_StopBits = parameters[4];
                        SetValues.Set_BitsLength = Convert.ToInt32(parameters[2]);
                        SetValues.Set_CommunicationProtocol = protocol[parameters[5] == "ASCII" ? 1 : (parameters[5] == "RTU" ? 2 : 0)];

                        baudrateIndex = Array.IndexOf(baudRates, parameters[0]).ToString();
                        parityIndex = Array.IndexOf(parity, parameters[1]).ToString();
                        stopbitsIndex = Array.IndexOf(stopBits, parameters[2]).ToString();
                        bitsLengthIndex = Array.IndexOf(dataLengths, parameters[3]).ToString();
                        protocolIndex = parameters[4] == "ASCII" ? "1" : (parameters[4] == "RTU" ? "2" : "0");

                        valid = true;
                        btnSet.Enabled = true;
                    }
                    ReadWriteFile("1", baudrateIndex, parityIndex, bitsLengthIndex, stopbitsIndex, protocolIndex);

                    //CloseForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CancelSearch()
        {
            try
            {

                progressBar1.Invoke((Action)(() => progressBar1.Value = 0));
                if (_thread != null)
                {
                    if (_thread.IsAlive)
                    {
                        _thread.Abort();
                        // btnClose.Invoke((Action)(() => btnClose.Text = "Close"));
                        // lblStatus1.Invoke((Action)(() => lblStatus1.Text = "Search Abort"));
                        _thread = null;

                        if (lblStatus1.InvokeRequired)
                        {
                            lblStatus1.Invoke((Action)(() => lblStatus1.Text = ""));
                        }
                        else
                        {
                            lblStatus1.Text = "Search Abort";
                        }

                        if (lblFrame.InvokeRequired)
                        {
                            lblFrame.Invoke((Action)(() => lblFrame.Text = ""));
                        }
                        else
                        {
                            lblFrame.Text = "";
                        }

                    }
                }

                if (modbusRTUobj != null)
                {
                    if (modbusRTUobj.IsSerialPortOpen())
                    {
                        modbusRTUobj.CloseSerialPort();
                        modbusRTUobj = null;
                    }
                }
            }

            catch (Exception ex)
            {
                LogWriter.WriteToFile("AutoDetectsettings: ", ex.StackTrace, "DTC_ErrorLog");
            }
            finally
            {
                _thread = null;
                modbusRTUobj = null;
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Visible = false;
            btnSearch.Visible = true;
            try
            {
                //ControlBox = true;
                clsModbus mo = new clsModbus();
                //mo.ACloseSerialPort();
                CancelSearch();
            }
            catch (Exception ae)
            {

            }
        }
        // bbbb
        //public byte[] SendFrameToDevice1(string A, string B, string C, string D)
        //{
        //    clsModbus modbusobj = new clsModbus();
        //    string result = string.Empty;
        //    string bb = string.Empty;
        //    //string result = "";
        //    byte[] RecieveData = null;

        //    if (!string.IsNullOrEmpty(obj.Comport) && !string.IsNullOrEmpty(obj.Baudrate) && !string.IsNullOrEmpty(obj.Parity) &&
        //    (obj.Bitslength) > 0 && !string.IsNullOrEmpty(obj.Stopbits))
        //    {
        //        try
        //        {
        //            if (modbusobj.OpenSerialPortForm11(obj.Comport, Convert.ToInt32(obj.Baudrate), obj.Parity, Convert.ToInt32(obj.Stopbits), obj.Bitslength))
        //            {
                        

        //                if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
        //                {
        //                    if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
        //                    {
        //                        RecieveData = modbusobj.AscFrame(Convert.ToInt32(A).ToString("X").PadLeft(2, '0'), B,
        //                            C, D);
        //                    }
        //                    else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
        //                    {
        //                       // RecieveData = modbusobj.RtuFrame(key, "03",
        //                         //  addres, "0001");
        //                        RecieveData = modbusobj.RtuFrame(Convert.ToInt32(A).ToString("X").PadLeft(2, '0'), B,
        //                           C, D);
        //                    }

        //                    if (RecieveData != null && RecieveData.Length > 0)
        //                    {

        //                        return RecieveData;
        //                    }

        //                }

        //            }


        //        }
        //        catch (Exception ex)
        //        {
        //            //MessageBox.Show(ex.StackTrace);
        //            //LogWriter.WriteToFile("ModbusRTU: 5)", ex.Message, "DTC_ErrorLog");
        //            //lblMessage.Text = "3" + ex.Message;

        //            return RecieveData;
        //        }
        //        finally
        //        {
        //            if (modbusobj.IsSerialPortOpen())
        //            {
        //                modbusobj.CloseSerialPort();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Setting is Empty");
        //    }
        //    return RecieveData;

        //}

        ////public byte[] frame()
        ////{

        ////    if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
        ////    {
        ////        if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
        ////        {
        ////            RecieveData = modbusobj.AscFrame(Convert.ToInt32(A).ToString("X").PadLeft(2, '0'), B,
        ////                C, D);
        ////        }
        ////        else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
        ////        {
        ////            RecieveData = modbusobj.RtuFrame(key, "03",
        ////               addres, "0001");
        ////            RecieveData = modbusobj.RtuFrame(Convert.ToInt32(A).ToString("X").PadLeft(2, '0'), B,
        ////               C, D);
        ////        }

        ////        if (RecieveData != null && RecieveData.Length > 0)
        ////        {

        ////            return RecieveData;
        ////        }

        ////    }
        ////}
    }

    public class cls_1
    {
        public int Srno { get; set; }
        public string Comport { get; set; }
        public string Address { get; set; }
        public string Baudrate { get; set; }
        public string Parity { get; set; }
        public string Stopbits { get; set; }
        public string Bitslength { get; set; }
        public string Protocol { get; set; }

        public int ProtocalIndex { get; set; }
    }


    
}
