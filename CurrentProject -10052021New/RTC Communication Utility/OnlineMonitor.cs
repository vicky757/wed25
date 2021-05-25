using ClassList;
using RTC_Communication_Utility.UserControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTC_Communication_Utility
{
    public partial class OnlineMonitor : Form
    {
        #region monitor params obj
        MonitorParameters monitorParams = new MonitorParameters()
        {
            CtrlAction = -1,
            RunHaltmode = -1,
            Out1Percent = -1,
            Out2Percent = -1,
            Lockstatus = -1,
            Autotuning = -1,
            SensorType = -1,
            UnitType = -1,
            Setvalue = -1,
            Fraction = 0,
            Alarm1Mode = -1,
            Alarm2Mode = -1,
            SV11 = null,
            HighTemp = null,
            LowTemp = null,
            Alarm1Up = null,
            Alarm1Down = null,
            Alarm2Up = null,
            Alarm2Down = null,
            PVOffset = null,
            SwVersion = null,
            Hysteresis1 = null,
            Hysteresis2 = null,
            HysteresisDeadBand = null,
            PD = null,
            Ti = null,
            Td = null,
            CtrlPer1 = null,
            CtrlPer2 = null,
            Ioffset = null,
            PCoefficient = null,
            Deadband = null,
            Out11 = null,
            Out12 = null,
            CtrlPeriod1 = null,
            CtrlPeriod2 = null,
            TuneOHigh = null,
            TuneOLow = null,
        };
        #endregion

        List<MonitorUserControl> usrList = null;
        Dictionary<int, int> usrDictionary = null;
        ArrayList selectedNodeList = new ArrayList(8);

        string m_exePath1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        clsModbus modObj = null;
        Thread th = null;
        System.Threading.Timer TheTimer = null;
        ModbusVariables sourceObject = null;
        CancellationTokenSource cancellationTokenSource;
        CancellationToken token;
        bool ReadFlag = true;
        bool parametersVisibleFlag = false;

        static string StaticVarAddress = "";
        static string StaticVarValue = "";
        Thread t = null;
        string decimalPlaces = "0";

        List<List<string>> frameList = new List<List<string>>() 
        {
            new List<string>() { "0", "03", "4751", "0001" } ,
            new List<string>() { "1", "03", "471A", "0006" } ,
            new List<string>() { "2", "03", "4700", "0004" } ,
            new List<string>() { "3", "03", "4728", "0008" } ,
            new List<string>() { "4", "03", "4700", "0004" } ,
            new List<string>() { "5", "03", "4723", "0006" } ,
            new List<string>() { "6", "03", "4751", "0001" } ,
            new List<string>() { "7", "03", "4700", "0002" } ,
            new List<string>() { "8", "03", "4700", "0001" } ,
            new List<string>() { "9", "03", "4720", "0008" } ,
            new List<string>() { "10", "03", "4710", "0008" } ,
            new List<string>() { "11", "03", "4718", "0008" } ,
            new List<string>() { "12", "03", "4708", "0008" } ,
            new List<string>() { "13", "03", "4701", "0007" } ,
        };

        public OnlineMonitor()
        {
            InitializeComponent();

            modObj = new clsModbus();
            sourceObject = new ModbusVariables();

            usrList = new List<MonitorUserControl>();
            usrDictionary = new Dictionary<int, int>();
        }

        private void OnlineMonitor_Load(object sender, EventArgs e)
        {
            MonitorUserControl monitorUserControl1 = new MonitorUserControl() { NodeId = 1, NodeAddress = 0, PV = "0.0", SV = "0.0" };
            monitorUserControl1.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
            monitorUserControl1.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
            monitorUserControl1.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

            MonitorUserControl monitorUserControl2 = new MonitorUserControl() { NodeId = 2, NodeAddress = 0, PV = "0.0", SV = "0.0" };
            monitorUserControl2.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
            monitorUserControl2.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
            monitorUserControl2.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

            MonitorUserControl monitorUserControl3 = new MonitorUserControl() { NodeId = 3, NodeAddress = 0, PV = "0.0", SV = "0.0" };
            monitorUserControl3.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
            monitorUserControl3.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
            monitorUserControl3.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

            MonitorUserControl monitorUserControl4 = new MonitorUserControl() { NodeId = 4, NodeAddress = 0, PV = "0.0", SV = "0.0" };
            monitorUserControl4.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
            monitorUserControl4.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
            monitorUserControl4.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

            usrList.Add(monitorUserControl1);
            usrList.Add(monitorUserControl2);
            usrList.Add(monitorUserControl3);
            usrList.Add(monitorUserControl4);

            foreach (var item in usrList)
            {
                flowLayoutPanel1.Controls.Add(item);
            }
            //CmbBxCtrlAction.SelectedIndex = 0;
            //CmbBxRunHaltmode.SelectedIndex = 0;
            //CmbBx1stout.SelectedIndex = 0;
            //CmbBx2ndout.SelectedIndex = 0;
            CmbBxLockstatus.SelectedIndex = 0;
            CmbBxAutotuning.SelectedIndex = 0;

            //CmbBxSensorType.SelectedIndex = 0;
            //CmbBxUnitTyp.SelectedIndex = 0;

            CmbBxfraction.SelectedIndex = 1;

            //cmbBxAlarm1Mode.SelectedIndex = 0;
            //cmbBxAlarm2Mode.SelectedIndex = 0;

            grpBxParameters.Visible = parametersVisibleFlag;

            cmbBxSetvalue.Visible = false;

            grpbx_PID_Parameter.Visible = false;
            grpBxManual.Visible = false;
            grpBxOnOff.Visible = false;
            grpbx_Alarm.Visible = false;
            grpbx_Alarm1.Visible = false;
            grpbx_Alarm2.Visible = false;

            //frameList.Add(new List<string>() {"01", "03", "4751", "0001" });

            txtPV2.DataBindings.Add("Text", sourceObject, "PV");
            txtSV2.DataBindings.Add("Text", sourceObject, "SV");

            string portName = SetValues.Set_PortName;
            int baudRate = Convert.ToInt32(SetValues.Set_Baudrate);
            string parity = SetValues.Set_parity;
            int bitsLength = SetValues.Set_BitsLength;
            int stopBits = Convert.ToInt32(SetValues.Set_StopBits);

            if (modObj.OpenSerialPort(portName, baudRate, parity, stopBits, bitsLength))
            {
                LogWriter.WriteToFile("Load() =>", "Port Opened"
                        , "OnlineMonitor");
            }
        }

        private void SelectedItemCallbackFn(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                MonitorUserControl ctrl = usrList.Find(x => x.NodeId == Convert.ToInt32(item));

                foreach (var ele in usrList)
                {
                    if (ele.Connected)
                    {
                        ele.SelectedNode = false;
                    }
                }
                ctrl.SelectedNode = true;

            }
        }

        private void RemoveItemCallbackFn(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                MonitorUserControl ctrl = usrList.Find(x => x.NodeId == Convert.ToInt32(item));

                if (selectedNodeList.Contains(item))
                {
                    selectedNodeList.Remove(item);
                }
                if (selectedNodeList.Count > 0)
                {
                    MonitorUserControl ctrll = usrList.Find(x => x.NodeId == Convert.ToInt32(selectedNodeList[0]));
                    ctrll.SelectedNode = true;

                    //grpBxParameters.Visible = true;
                }
                else
                {
                    //grpBxParameters.Visible = false;
                }
            }
        }

        private void AddItemCallbackFn(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                MonitorUserControl ctrl = usrList.Find(x => x.NodeId == Convert.ToInt32(item));

                if (usrDictionary.ContainsKey(ctrl.NodeAddress))
                {
                    usrDictionary[ctrl.NodeAddress]++;
                }
                else
                {
                    usrDictionary.Add(ctrl.NodeAddress, 1);
                }

                if (selectedNodeList.Count == 0)
                {
                    ctrl.SelectedNode = true;
                }
                selectedNodeList.Add(item);

                //grpBxParameters.Visible = true;
            }
        }

        public void ThreadFunc()
        {
            th = new Thread(() =>
            {
                CreateFrames("01", "03", "1000", "0002", true);
            });
        }

        private void OnlineMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //// Prompt user to save his data
                //MessageBox.Show("closing");
            }
            else if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                //// Autosave and clear up resources
                //MessageBox.Show("Closing forcefully");
            }

            if (modObj != null)
            {
                LogWriter.WriteToFile("Closing() =>", "Port closed"
                        , "OnlineMonitor");
                modObj.CloseSerialPort();
            }
        }

        private void btnConnect1_Click(object sender, EventArgs e)
        {

            try
            {
                if (btnConnect1.Text == "Connect")
                {
                    btnConnect1.Text = "Disconnect";

                    int node = Convert.ToInt32(nodeAddress1.Value);

                    if (node > 0)
                    {
                        string nodeAddress = node.ToString(); //"01";

                        t = new Thread(() => SampleThread(nodeAddress));

                        t.SetApartmentState(ApartmentState.STA);

                        t.Start();

                        while (t.IsAlive)
                        {
                            Application.DoEvents();
                        }
                    }
                }
                else if (btnConnect1.Text == "Disconnect")
                {
                    if (t != null)
                    {
                        if (t.IsAlive || (t.ThreadState == ThreadState.Running))
                        {
                            t.Abort();
                        }
                    }
                    btnConnect1.Text = "Connect";
                }
            }
            catch (Exception ex)
            {

            }
        }

        private List<string> CreateFrames(string nodeAddress, string functionCode, string regAddress, string wordCount, bool read)
        {
            byte[] RecieveData = null;
            try
            {
                if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                {
                    if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
                    {
                        RecieveData = modObj.AscFrame(nodeAddress, functionCode, regAddress, wordCount);
                    }
                    else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
                    {
                        RecieveData = modObj.RtuFrame(nodeAddress, functionCode, regAddress, wordCount);
                    }

                    if (RecieveData != null && RecieveData.Length > 5)
                    {
                        //01 03 02 02 FF F9 64 
                        int size = Convert.ToInt32(RecieveData[2]);

                        List<string> returnValues = new List<string>();
                        if (read)
                        {
                            for (int i = 0; i < size; i++)
                            {
                                byte[] bytes = new byte[2];

                                Array.Copy(RecieveData, (3 + i), bytes, 0, 2);

                                string byteArrayToString = clsModbus.ByteArrayToString(bytes);

                                returnValues.Add(byteArrayToString);
                                i = i + 1;
                            }
                        }
                        return returnValues;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private List<string> CreateFrames2(string nodeAddress, string functionCode, string regAddress, string wordCount)
        {
            LogWriter.WriteToFile("CreateFrames2() =>", "Creating frames"
                        , "OnlineMonitor");

            byte[] RecieveData = null;

            if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
            {
                if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
                {
                    RecieveData = modObj.AscFrame(nodeAddress, functionCode, regAddress, wordCount);
                }
                else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
                {
                    RecieveData = modObj.RtuFrame(nodeAddress, functionCode, regAddress, wordCount);
                }

                if (RecieveData != null && RecieveData.Length > 5)
                {
                    //01 03 02 02 FF F9 64 
                    int size = Convert.ToInt32(RecieveData[2]);

                    List<string> returnValues = new List<string>();

                    for (int i = 0; i < size; i++)
                    {
                        byte[] bytes = new byte[2];

                        Array.Copy(RecieveData, (3 + i), bytes, 0, 2);

                        string byteArrayToString = clsModbus.ByteArrayToString(bytes);
                        decimal decValue = Convert.ToInt64(byteArrayToString, 16);

                        returnValues.Add((decValue / 10).ToString());
                        i = i + 1;
                    }

                    return returnValues;
                    //return RecieveData;
                }
            }

            return null;
        }

        public void SampleThread(string nodeAddress)
        {
            try
            {

                //for (int i = 0; i < 10; i++)
                while (t.IsAlive)
                {
                    int i = 0;
                    grpBxParameters.BeginInvoke(
                        new Action(() =>
                        {
                            grpBxParameters.Visible = true;
                        }
                    ));

                    if (modObj.IsSerialPortOpen())
                    {
                        if (ReadFlag)
                        {
                            for (int k = 0; k < frameList.Count; k++)
                            {
                                if (!ReadFlag)
                                {
                                    break;
                                }

                                var list = CreateFrames(nodeAddress, frameList[k][1], frameList[k][2], frameList[k][3], ReadFlag);

                                if (list != null)
                                {
                                    switch (frameList[k][0])
                                    {
                                        case "0":
                                            funct_0(list);

                                            break;
                                        case "1":
                                            funct_1(list);

                                            break;
                                        case "2":
                                            funct_2(list);

                                            break;
                                        case "3":
                                            funct_3(list);

                                            break;
                                        case "4":
                                            funct_4(list);

                                            break;
                                        case "5":
                                            funct_5(list);

                                            break;
                                        case "6":
                                            funct_6(list);

                                            break;
                                        case "7":
                                            funct_7(list);

                                            break;
                                        case "8":
                                            funct_8(list);

                                            break;
                                        case "9":
                                            funct_9(list);

                                            break;
                                        case "10":
                                            funct_10(list);

                                            break;
                                        case "11":
                                            funct_11(list);

                                            break;
                                        case "12":
                                            funct_12(list);

                                            break;
                                        case "13":
                                            funct_13(list);
                                            //parametersVisibleFlag = true;
                                            break;
                                    }


                                }


                            }
                        }
                        else if (!ReadFlag)
                        {
                            //Write
                            var list = CreateFrames(nodeAddress, "06", StaticVarAddress, StaticVarValue, ReadFlag);
                            //if (list != null)
                            {
                                ReadFlag = true;
                            }
                        }
                    }
                    progressBar1.BeginInvoke(
                        new Action(() =>
                        {
                            progressBar1.Value = (100 * i) / 15;
                        }
                    ));
                    Thread.Sleep(1);
                    i++;
                }
                progressBar1.BeginInvoke(
                        new Action(() =>
                        {
                            progressBar1.Value = 100;
                        }
                    ));
            }
            catch (Exception ex)
            {

            }
        }

        private void funct_0(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    textBox73.Invoke((Action)(() =>
                    {
                        textBox73.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox73.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_1(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    //471A
                    textBox1.Invoke((Action)(() =>
                    {
                        textBox1.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox1.Refresh();
                    }));
                    CmbBx1stout.Invoke((Action)(() =>
                    {
                        CmbBx1stout.SelectedIndex = string.IsNullOrEmpty(list[0]) ? -1 : Convert.ToInt16(string.Join(",", list[0]), 16);
                        CmbBx1stout.Refresh();
                    }));

                    //471B
                    textBox2.Invoke((Action)(() =>
                    {
                        textBox2.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                        textBox2.Refresh();
                    }));

                    //471C
                    textBox3.Invoke((Action)(() =>
                    {
                        textBox3.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        textBox3.Refresh();
                    }));

                    //471D
                    textBox4.Invoke((Action)(() =>
                    {
                        textBox4.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        textBox4.Refresh();
                    }));

                    //471E
                    textBox5.Invoke((Action)(() =>
                    {
                        textBox5.Text = string.IsNullOrEmpty(list[4]) ? "0" : ConvertShortToHexString(list[4]);
                        textBox5.Refresh();
                    }));

                    //471F
                    textBox6.Invoke((Action)(() =>
                    {
                        textBox6.Text = string.IsNullOrEmpty(list[5]) ? "0" : ConvertShortToHexString(list[5]);
                        textBox6.Refresh();
                    }));
                    CmbBx2ndout.Invoke((Action)(() =>
                    {
                        CmbBx2ndout.SelectedIndex = string.IsNullOrEmpty(list[5]) ? -1 : Convert.ToInt16(string.Join(",", list[5]), 16);
                        CmbBx2ndout.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_2(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    string pvS = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                    string pv = GetDecimalPlaces(pvS);

                    string svS = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                    string sv = GetDecimalPlaces(svS);

                    //4700
                    textBox7.Invoke((Action)(() =>
                    {
                        textBox7.Text = pv;
                        textBox7.Refresh();
                    }));
                    txtPV1.Invoke((Action)(() =>
                    {
                        txtPV1.Text = pv;
                        txtPV1.Refresh();
                    }));

                    //4701
                    textBox8.Invoke((Action)(() =>
                    {
                        textBox8.Text = sv;
                        textBox8.Refresh();
                    }));
                    txtSV1.Invoke((Action)(() =>
                    {
                        txtSV1.Text = sv;
                        txtSV1.Refresh();
                    }));

                    //4702          
                    textBox9.Invoke((Action)(() =>
                    {
                        textBox9.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        textBox9.Refresh();
                    }));
                    txtBxOut1.Invoke((Action)(() =>
                    {
                        txtBxOut1.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        txtBxOut1.Refresh();
                    }));


                    //4703
                    textBox10.Invoke((Action)(() =>
                    {
                        textBox10.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        textBox10.Refresh();
                    }));
                    txtBxOut2.Invoke((Action)(() =>
                    {
                        txtBxOut2.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        txtBxOut2.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_3(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    //4728
                    textBox11.Invoke((Action)(() =>
                    {
                        textBox11.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox11.Refresh();
                    }));

                    //4729
                    textBox12.Invoke((Action)(() =>
                    {
                        textBox12.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                        textBox12.Refresh();
                    }));

                    //472A
                    textBox13.Invoke((Action)(() =>
                    {
                        textBox13.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        textBox13.Refresh();
                    }));
                    lblSwVersion.Invoke((Action)(() =>
                    {
                        string s = string.IsNullOrEmpty(list[2]) ? "0" : list[2];
                        lblSwVersion.Text = "v" + String.Format("{0:0.00}", Convert.ToDouble(Convert.ToInt32(s) * 0.010));
                        lblSwVersion.Refresh();
                    }));


                    //472B
                    textBox14.Invoke((Action)(() =>
                    {
                        textBox14.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        textBox14.Refresh();
                    }));

                    //472C
                    textBox15.Invoke((Action)(() =>
                    {
                        textBox15.Text = string.IsNullOrEmpty(list[4]) ? "0" : ConvertShortToHexString(list[4]);
                        textBox15.Refresh();
                    }));

                    //472D
                    textBox16.Invoke((Action)(() =>
                    {
                        textBox16.Text = string.IsNullOrEmpty(list[5]) ? "0" : ConvertShortToHexString(list[5]);
                        textBox16.Refresh();
                    }));

                    //472E
                    textBox17.Invoke((Action)(() =>
                    {
                        textBox17.Text = string.IsNullOrEmpty(list[6]) ? "0" : ConvertShortToHexString(list[6]);
                        textBox17.Refresh();
                    }));

                    //472F
                    textBox18.Invoke((Action)(() =>
                    {
                        textBox18.Text = string.IsNullOrEmpty(list[7]) ? "0" : ConvertShortToHexString(list[7]);
                        textBox18.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_4(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    string pvS = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                    string pv = GetDecimalPlaces(pvS);

                    string svS = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                    string sv = GetDecimalPlaces(svS);

                    //4700
                    textBox19.Invoke((Action)(() =>
                    {
                        textBox19.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox19.Refresh();
                    }));
                    txtPV1.Invoke((Action)(() =>
                    {
                        txtPV1.Text = pv;
                        txtPV1.Refresh();
                    }));

                    //4701
                    textBox20.Invoke((Action)(() =>
                    {
                        textBox20.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                        textBox20.Refresh();
                    }));
                    txtSV1.Invoke((Action)(() =>
                    {
                        txtSV1.Text = sv;
                        txtSV1.Refresh();
                    }));

                    //4702
                    textBox21.Invoke((Action)(() =>
                    {
                        textBox21.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        textBox21.Refresh();
                    }));

                    //4703
                    textBox22.Invoke((Action)(() =>
                    {
                        textBox22.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        textBox22.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_5(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    //4723
                    textBox23.Invoke((Action)(() =>
                    {
                        textBox23.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox23.Refresh();
                    }));

                    //4724
                    CmbBxUnitTyp.Invoke((Action)(() =>
                    {
                        CmbBxUnitTyp.SelectedIndex = string.IsNullOrEmpty(list[1]) ? -1 : Convert.ToInt16(string.Join(",", list[1]), 16);
                        CmbBxUnitTyp.Refresh();
                    }));
                    textBox24.Invoke((Action)(() =>
                    {
                        textBox24.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                        textBox24.Refresh();
                    }));

                    //4725
                    textBox25.Invoke((Action)(() =>
                    {
                        textBox25.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        textBox25.Refresh();
                    }));

                    //4726
                    textBox26.Invoke((Action)(() =>
                    {
                        textBox26.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        textBox26.Refresh();
                    }));

                    //4727
                    textBox27.Invoke((Action)(() =>
                    {
                        textBox27.Text = string.IsNullOrEmpty(list[4]) ? "0" : ConvertShortToHexString(list[4]);
                        textBox27.Refresh();
                    }));

                    //4728
                    textBox28.Invoke((Action)(() =>
                    {
                        textBox28.Text = string.IsNullOrEmpty(list[5]) ? "0" : ConvertShortToHexString(list[5]);
                        textBox28.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_6(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    textBox29.Invoke((Action)(() =>
                    {
                        textBox29.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox29.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_7(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    string pvS = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                    string pv = GetDecimalPlaces(pvS);

                    string svS = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                    string sv = GetDecimalPlaces(svS);

                    //4700
                    textBox30.Invoke((Action)(() =>
                    {
                        textBox30.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox30.Refresh();
                    }));
                    txtPV1.Invoke((Action)(() =>
                    {
                        txtPV1.Text = pv;
                        txtPV1.Refresh();
                    }));

                    //4701
                    textBox31.Invoke((Action)(() =>
                    {
                        textBox31.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                        textBox31.Refresh();
                    }));
                    txtSV1.Invoke((Action)(() =>
                    {
                        txtSV1.Text = sv;
                        txtSV1.Refresh();
                    }));
                    txtBxSetvalue.Invoke((Action)(() =>
                    {
                        txtBxSetvalue.Text = sv;
                        txtBxSetvalue.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_8(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    textBox32.Invoke((Action)(() =>
                    {
                        textBox32.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox32.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_9(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    //4720
                    textBox33.Invoke((Action)(() =>
                    {
                        textBox33.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox33.Refresh();
                    }));
                    cmbBxAlarm1Mode.Invoke((Action)(() =>
                    {
                        cmbBxAlarm1Mode.SelectedIndex = string.IsNullOrEmpty(list[0]) ? -1 : Convert.ToInt16(string.Join(",", list[0]), 16); //ConvertShortToHexString(list[0]);
                        cmbBxAlarm1Mode.Refresh();
                    }));


                    //4721
                    textBox34.Invoke((Action)(() =>
                    {
                        textBox34.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                        textBox34.Refresh();
                    }));
                    cmbBxAlarm2Mode.Invoke((Action)(() =>
                    {
                        cmbBxAlarm2Mode.SelectedIndex = string.IsNullOrEmpty(list[1]) ? -1 : Convert.ToInt16(string.Join(",", list[1]), 16); ;//ConvertShortToHexString(list[1]);
                        cmbBxAlarm2Mode.Refresh();
                    }));

                    //4722
                    textBox35.Invoke((Action)(() =>
                    {
                        textBox35.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        textBox35.Refresh();
                    }));

                    //4723
                    textBox36.Invoke((Action)(() =>
                    {
                        textBox36.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        textBox36.Refresh();
                    }));

                    //4724
                    CmbBxRunHaltmode.Invoke((Action)(() =>
                    {
                        CmbBxRunHaltmode.SelectedIndex = string.IsNullOrEmpty(list[4]) ? -1 : Convert.ToInt16(string.Join(",", list[4]), 16);
                        CmbBxRunHaltmode.Refresh();
                    }));
                    textBox37.Invoke((Action)(() =>
                    {
                        textBox37.Text = string.IsNullOrEmpty(list[4]) ? "0" : ConvertShortToHexString(list[4]);
                        textBox37.Refresh();
                    }));

                    //4725
                    textBox38.Invoke((Action)(() =>
                    {
                        textBox38.Text = string.IsNullOrEmpty(list[5]) ? "0" : ConvertShortToHexString(list[5]);
                        textBox38.Refresh();
                    }));

                    //4726
                    textBox39.Invoke((Action)(() =>
                    {
                        textBox39.Text = string.IsNullOrEmpty(list[6]) ? "0" : ConvertShortToHexString(list[6]);
                        textBox39.Refresh();
                    }));

                    //4727
                    textBox40.Invoke((Action)(() =>
                    {
                        textBox40.Text = string.IsNullOrEmpty(list[7]) ? "0" : ConvertShortToHexString(list[7]);
                        textBox40.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_10(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    //4717
                    textBox41.Invoke((Action)(() =>
                    {
                        textBox41.Text = string.IsNullOrEmpty(list[7]) ? "0" : ConvertShortToHexString(list[7]);
                        textBox41.Refresh();
                    }));

                    //4716
                    textBox42.Invoke((Action)(() =>
                    {
                        textBox42.Text = string.IsNullOrEmpty(list[6]) ? "0" : ConvertShortToHexString(list[6]);
                        textBox42.Refresh();
                    }));

                    //4715
                    textBox43.Invoke((Action)(() =>
                    {
                        textBox43.Text = string.IsNullOrEmpty(list[5]) ? "0" : ConvertShortToHexString(list[5]);
                        textBox43.Refresh();
                    }));

                    //4714
                    txtHysteresis2.Invoke((Action)(() =>
                    {
                        txtHysteresis2.Text = string.IsNullOrEmpty(list[4]) ? "0" : ConvertShortToHexString(list[4]);
                        txtHysteresis2.Refresh();
                    }));
                    textBox44.Invoke((Action)(() =>
                    {
                        textBox44.Text = string.IsNullOrEmpty(list[4]) ? "0" : ConvertShortToHexString(list[4]);
                        textBox44.Refresh();
                    }));

                    //4713
                    txtHysteresis1.Invoke((Action)(() =>
                    {
                        txtHysteresis1.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        txtHysteresis1.Refresh();
                    }));
                    textBox45.Invoke((Action)(() =>
                    {
                        textBox45.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        textBox45.Refresh();
                    }));

                    //4712
                    textBox46.Invoke((Action)(() =>
                    {
                        textBox46.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        textBox46.Refresh();
                    }));
                    txtBxdeadband.Invoke((Action)(() =>
                    {
                        txtBxdeadband.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        txtBxdeadband.Refresh();
                    }));

                    //4711
                    textBox47.Invoke((Action)(() =>
                    {
                        textBox47.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                        textBox47.Refresh();
                    }));
                    txtBxPCoefficient.Invoke((Action)(() =>
                    {
                        txtBxPCoefficient.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                        txtBxPCoefficient.Refresh();
                    }));


                    //4710
                    textBox48.Invoke((Action)(() =>
                    {
                        textBox48.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox48.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_11(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    textBox49.Invoke((Action)(() =>
                    {
                        textBox49.Text = string.IsNullOrEmpty(list[7]) ? "0" : ConvertShortToHexString(list[7]);
                        textBox49.Refresh();
                    }));
                    textBox50.Invoke((Action)(() =>
                    {
                        textBox50.Text = string.IsNullOrEmpty(list[6]) ? "0" : ConvertShortToHexString(list[6]);
                        textBox50.Refresh();
                    }));
                    textBox51.Invoke((Action)(() =>
                    {
                        textBox51.Text = string.IsNullOrEmpty(list[5]) ? "0" : ConvertShortToHexString(list[5]);
                        textBox51.Refresh();
                    }));
                    textBox52.Invoke((Action)(() =>
                    {
                        textBox52.Text = string.IsNullOrEmpty(list[4]) ? "0" : ConvertShortToHexString(list[4]);
                        textBox52.Refresh();
                    }));
                    textBox53.Invoke((Action)(() =>
                    {
                        textBox53.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        textBox53.Refresh();
                    }));
                    textBox54.Invoke((Action)(() =>
                    {
                        textBox54.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        textBox54.Refresh();
                    }));

                    //-4719---------------------------------
                    CmbBxCtrlAction.Invoke((Action)(() =>
                    {
                        CmbBxCtrlAction.SelectedIndex = string.IsNullOrEmpty(list[1]) ? -1 : Convert.ToInt16(string.Join(",", list[1]), 16);
                        CmbBxCtrlAction.Refresh();
                    }));
                    textBox55.Invoke((Action)(() =>
                    {
                        textBox55.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                        textBox55.Refresh();
                    }));


                    //input sensor type
                    //-4718---------------------------------
                    CmbBxSensorType.Invoke((Action)(() =>
                    {
                        CmbBxSensorType.SelectedIndex = string.IsNullOrEmpty(list[0]) ? -1 : Convert.ToInt16(string.Join(",", list[0]), 16);
                        CmbBxSensorType.Refresh();
                    }));
                    textBox56.Invoke((Action)(() =>
                    {
                        textBox56.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox56.Refresh();
                    }));
                    //----------------------------------
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_12(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    //4708
                    textBox57.Invoke((Action)(() =>
                    {
                        textBox57.Text = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox57.Refresh();
                    }));
                    txtBxAlarm2Up.Invoke((Action)(() =>
                    {
                        txtBxAlarm2Up.Text = string.IsNullOrEmpty(list[0]) ? "0" : GetDecimalPlaces(ConvertShortToHexString(list[0]));
                        txtBxAlarm2Up.Refresh();
                    }));

                    //4709
                    textBox58.Invoke((Action)(() =>
                    {
                        textBox58.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                        textBox58.Refresh();
                    }));
                    txtBxAlarm2Down.Invoke((Action)(() =>
                    {
                        txtBxAlarm2Down.Text = string.IsNullOrEmpty(list[1]) ? "0" : GetDecimalPlaces(ConvertShortToHexString(list[1]));
                        txtBxAlarm2Down.Refresh();
                    }));

                    //470A
                    textBox59.Invoke((Action)(() =>
                    {
                        textBox59.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        textBox59.Refresh();
                    }));

                    //470B
                    textBox60.Invoke((Action)(() =>
                    {
                        textBox60.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        textBox60.Refresh();
                    }));

                    //470C
                    textBox61.Invoke((Action)(() =>
                    {
                        textBox61.Text = String.Format("{0:0.0}", Convert.ToDouble(string.IsNullOrEmpty(list[4]) ? "0" : ConvertShortToHexString(list[4]).ToString()) / 10);
                        textBox61.Refresh();
                    }));
                    txtBxPD.Invoke((Action)(() =>
                    {
                        txtBxPD.Text = String.Format("{0:0.0}", Convert.ToDouble(string.IsNullOrEmpty(list[4]) ? "0" : ConvertShortToHexString(list[4]).ToString()) / 10);
                        txtBxPD.Refresh();
                    }));

                    //470D
                    textBox62.Invoke((Action)(() =>
                    {
                        textBox62.Text = string.IsNullOrEmpty(list[5]) ? "0" : ConvertShortToHexString(list[5]);
                        textBox62.Refresh();
                    }));
                    txtBxTi.Invoke((Action)(() =>
                    {
                        txtBxTi.Text = string.IsNullOrEmpty(list[5]) ? "0" : ConvertShortToHexString(list[5]);
                        txtBxTi.Refresh();
                    }));


                    //470E
                    textBox63.Invoke((Action)(() =>
                    {
                        textBox63.Text = string.IsNullOrEmpty(list[6]) ? "0" : ConvertShortToHexString(list[6]);
                        textBox63.Refresh();
                    }));
                    txtBxTd.Invoke((Action)(() =>
                    {
                        txtBxTd.Text = string.IsNullOrEmpty(list[6]) ? "0" : ConvertShortToHexString(list[6]);
                        txtBxTd.Refresh();
                    }));


                    //470F
                    textBox64.Invoke((Action)(() =>
                    {
                        textBox64.Text = string.IsNullOrEmpty(list[7]) ? "0" : ConvertShortToHexString(list[7]);
                        textBox64.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void funct_13(List<string> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    //4707
                    textBox65.Invoke((Action)(() =>
                    {
                        textBox65.Text = string.IsNullOrEmpty(list[6]) ? "0" : ConvertShortToHexString(list[6]);
                        textBox65.Refresh();
                    }));
                    txtBxAlarm1Down.Invoke((Action)(() =>
                    {
                        txtBxAlarm1Down.Text = string.IsNullOrEmpty(list[6]) ? "0" : GetDecimalPlaces(ConvertShortToHexString(list[6]));
                        txtBxAlarm1Down.Refresh();
                    }));

                    //4706
                    textBox66.Invoke((Action)(() =>
                    {
                        textBox66.Text = string.IsNullOrEmpty(list[5]) ? "0" : ConvertShortToHexString(list[5]);
                        textBox66.Refresh();
                    }));
                    txtBxAlarm1Up.Invoke((Action)(() =>
                    {
                        txtBxAlarm1Up.Text = string.IsNullOrEmpty(list[5]) ? "0" : GetDecimalPlaces(ConvertShortToHexString(list[5]));
                        txtBxAlarm1Up.Refresh();
                    }));


                    //4705
                    textBox67.Invoke((Action)(() =>
                    {
                        textBox67.Text = string.IsNullOrEmpty(list[4]) ? "0" : ConvertShortToHexString(list[4]);
                        textBox67.Refresh();
                    }));
                    txtBxLowtemp.Invoke((Action)(() =>
                    {
                        txtBxLowtemp.Text = string.IsNullOrEmpty(list[4]) ? "0" : GetDecimalPlaces(ConvertShortToHexString(list[4]));
                        txtBxLowtemp.Refresh();
                    }));

                    //4704
                    textBox68.Invoke((Action)(() =>
                    {
                        textBox68.Text = string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]);
                        textBox68.Refresh();
                    }));
                    txtBxHightemp.Invoke((Action)(() =>
                    {
                        txtBxHightemp.Text = string.IsNullOrEmpty(list[3]) ? "0" : GetDecimalPlaces(ConvertShortToHexString(list[3]));
                        txtBxHightemp.Refresh();
                    }));


                    //4703
                    textBox69.Invoke((Action)(() =>
                    {
                        textBox69.Text = string.IsNullOrEmpty(list[2]) ? "0" : ConvertShortToHexString(list[2]);
                        textBox69.Refresh();
                    }));

                    //4702
                    textBox70.Invoke((Action)(() =>
                    {
                        textBox70.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]);
                        textBox70.Refresh();
                    }));

                    //4701
                    textBox71.Invoke((Action)(() =>
                    {
                        textBox71.Text = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[0]);
                        textBox71.Refresh();
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void SetPV(string value)
        {
            //TextBox tbx = this.Controls.Find("textBox1", true).FirstOrDefault() as TextBox;
            if (string.IsNullOrEmpty(value))
            {
                txtPV1.Invoke((Action)(() =>
                {
                    txtPV1.Text = "0.0";
                    txtPV1.Refresh();
                }));
            }
            else
            {
                txtPV1.Invoke((Action)(() =>
                {
                    txtPV1.Refresh();
                    txtPV1.Text = value;

                }));
            }
        }

        private void SetSV(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                txtSV1.Invoke((Action)(() =>
                {
                    txtSV1.Text = "0.0";
                    txtSV1.Refresh();
                }));
            }
            else
            {
                txtSV1.Invoke((Action)(() =>
                {
                    txtSV1.Text = value;
                    txtSV1.Refresh();
                }));
            }
        }

        private void btnConnect2_Click(object sender, EventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;

            if (btnConnect2.Text == "Connect")
            {
                Get2();
                btnConnect2.Text = "Disconnect";
            }
            else
            {
                cancellationTokenSource.Cancel();
                btnConnect2.Text = "Connect";
            }
        }

        private void Get2()
        {
            try
            {
                Task task7 = new Task(() =>
                {
                    while (true)
                    {
                        if (modObj.IsSerialPortOpen())
                        {
                            var list = CreateFrames2("01", "03", "1000", "0002");

                            if (list != null)
                            {
                                string res = string.Join(",", list.ToArray());

                                lblReply1.Invoke((Action)(() => lblReply1.Text = res));

                                if (token.IsCancellationRequested)
                                {
                                    lblReply1.Invoke((Action)(() => lblReply1.Text = res));
                                    break;
                                }
                            }
                        }
                    }
                }, token);

                if (task7.Status == TaskStatus.Created)
                    task7.Start();
            }
            catch (Exception ex)
            {
            }
        }

        private void lblReply1_TextChanged(object sender, EventArgs e)
        {
            //int value = Convert.ToInt32(lblPV1.Text);

            sourceObject.PV = lblReply1.Text;
            sourceObject.SV = lblReply1.Text;
        }

        public void GetString(string stringToConvert)
        {
            short decValue = Convert.ToInt16(stringToConvert, 16);
        }

        private void CmbBxfraction_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private string ConvertShortToHexString(string strValue)
        {
            return Convert.ToInt16(string.Join(",", strValue), 16).ToString();
        }

        private string GetDecimalPlaces(string value)
        {
            int index = 0;
            CmbBxfraction.Invoke((Action)(() => index = Convert.ToInt32(CmbBxfraction.SelectedIndex)));
            double res = 0;
            string result = "";
            switch (index)
            {
                case 0:
                    res = Convert.ToDouble(value) / 10;
                    result = String.Format("{0:0}", res);
                    break;
                case 1:
                    res = Convert.ToDouble(value) / 10;
                    result = String.Format("{0:0.0}", res);
                    break;
                case 2:
                    res = Convert.ToDouble(value) / 100;
                    result = String.Format("{0:0.00}", res);
                    break;
                case 3:
                    res = Convert.ToDouble(value) / 1000;
                    result = String.Format("{0:0.000}", res);
                    break;
            }
            return result;
        }

        private string GetDecimalPlacesRev(string value)
        {
            int index = 0;
            CmbBxfraction.Invoke((Action)(() => index = Convert.ToInt32(CmbBxfraction.SelectedIndex)));
            double res = 0;
            string result = "";
            switch (index)
            {
                case 0:
                    res = Convert.ToDouble(value) * 10;
                    result = String.Format("{0:0}", res);
                    break;
                case 1:
                    res = Convert.ToDouble(value) * 10;
                    result = String.Format("{0:0.0}", res);
                    break;
                case 2:
                    res = Convert.ToDouble(value) * 100;
                    result = String.Format("{0:0.00}", res);
                    break;
                case 3:
                    res = Convert.ToDouble(value) * 1000;
                    result = String.Format("{0:0.000}", res);
                    break;
            }
            return result;
        }

        private void CmbBxCtrlAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            //show auto tuning only in PID other wise false
            #region A
            lblAutotuning.Visible = false;
            CmbBxAutotuning.Visible = false;
            grpbx_PID_Parameter.Visible = false;
            #endregion

            #region B
            grpBxOnOff.Visible = false;
            #endregion

            #region C
            grpBxManual.Visible = false;
            #endregion

            switch (CmbBxCtrlAction.SelectedIndex)
            {
                //PID
                case 0:
                    #region A
                    lblAutotuning.Visible = true;
                    CmbBxAutotuning.Visible = true;
                    grpbx_PID_Parameter.Visible = true;
                    #endregion

                    EnableDisableAlarms(CmbBx1stout.SelectedIndex, CmbBx2ndout.SelectedIndex);
                    break;

                //ON/OFF
                case 1:
                    #region B
                    grpBxOnOff.Visible = true;

                    #endregion

                    EnableDisableAlarms(CmbBx1stout.SelectedIndex, CmbBx2ndout.SelectedIndex);
                    break;

                //Manual
                case 2:
                    #region C
                    grpBxManual.Visible = true;
                    #endregion

                    EnableDisableAlarms(CmbBx1stout.SelectedIndex, CmbBx2ndout.SelectedIndex);
                    break;

                //Ramp Soak
                case 3:
                    #region D
                    grpbx_PID_Parameter.Visible = true;
                    cmbBxSetvalue.Visible = true;
                    txtBxSetvalue.Visible = false;
                    #endregion

                    EnableDisableAlarms(CmbBx1stout.SelectedIndex, CmbBx2ndout.SelectedIndex);
                    break;
            }

            ReadFlag = false;
            StaticVarAddress = "4719";
            StaticVarValue = CmbBxCtrlAction.SelectedIndex.ToString().PadLeft(4, '0');
        }

        private void CmbBx1stout_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (CmbBx1stout.SelectedIndex >= 0 && CmbBx2ndout.SelectedIndex >= 0)
                {
                    EnableDisableAlarms(CmbBx1stout.SelectedIndex, CmbBx2ndout.SelectedIndex);
                }

                ReadFlag = false;
                StaticVarAddress = "471A";
                StaticVarValue = CmbBx1stout.SelectedIndex.ToString().PadLeft(4, '0');
            }
            catch (Exception ex)
            {

            }
            #region old1
            //lblCtrlPer1.Visible = true;
            //txtBxCtrlPer1.Visible = true;

            //switch (CmbBx1stout.SelectedIndex)
            //{
            //        //Heat
            //    case 0:


            //        lblPCoefficient.Visible = false;
            //        txtBxPCoefficient.Visible = false;

            //        lbldeadband.Visible = false;
            //        txtBxdeadband.Visible = false;

            //        //alarm1 mode false---
            //        grpbx_Alarm1.Visible = false;

            //        if(CmbBx2ndout.SelectedIndex == 2)// Alarm
            //        {
            //            //show alarm mode----
            //            grpbx_Alarm1.Visible = true;
            //        }
            //        break;

            //        //Cool
            //    case 1:                    

            //        //alarm1 mode false---
            //        grpbx_Alarm1.Visible = false;

            //        break;
            //        //Alarm
            //    case 2:
            //        lblCtrlPer1.Visible = false;
            //        txtBxCtrlPer1.Visible = false;

            //        //show alarm mode----
            //        grpbx_Alarm1.Visible = true;
            //        break;

            //        //Re-transmission
            //    case 3:
            //        break;
            //}
            #endregion
        }

        private void CmbBx2ndout_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (CmbBx1stout.SelectedIndex >= 0 && CmbBx2ndout.SelectedIndex >= 0)
                {
                    EnableDisableAlarms(CmbBx1stout.SelectedIndex, CmbBx2ndout.SelectedIndex);
                }
                ReadFlag = false;
                StaticVarAddress = "471F";
                StaticVarValue = CmbBx2ndout.SelectedIndex.ToString().PadLeft(4, '0');
            }
            catch (Exception ex)
            {

            }
            #region old2
            //grpbx_Alarm2.Visible = false;

            //switch (CmbBx2ndout.SelectedIndex)
            //{
            //    //Heat
            //    case 0:

            //        break;

            //    //Cool
            //    case 1:
            //        lblPCoefficient.Visible = true;
            //        txtBxPCoefficient.Visible = true;

            //        lbldeadband.Visible = true;
            //        txtBxdeadband.Visible = true;
            //        break;

            //    //Alarm
            //    case 2:
            //        //alarm
            //        lblctrlPer2.Visible = false;
            //        txtBxctrlPer2.Visible = false;

            //        grpbx_Alarm2.Visible = true;
            //        break;
            //}
            #endregion
        }

        private void EnableDisableAlarms(int out1, int out2)
        {
            lblPCoefficient.Visible = false;
            txtBxPCoefficient.Visible = false;

            lbldeadband.Visible = false;
            txtBxdeadband.Visible = false;

            grpbx_Alarm1.Visible = false;
            grpbx_Alarm2.Visible = false;
            grpbx_Alarm.Visible = false;
            switch (CmbBxCtrlAction.SelectedIndex)
            {
                //PID
                case 0:
                    #region A
                    lblAutotuning.Visible = true;
                    CmbBxAutotuning.Visible = true;
                    grpbx_PID_Parameter.Visible = true;
                    #endregion

                    #region A2
                    if (CmbBx1stout.SelectedIndex == 0 &&
                CmbBx2ndout.SelectedIndex == 0) // h h
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = true;
                        txtBxCtrlPer1.Visible = true;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = true;
                        txtBxctrlPer2.Visible = true;

                        lblPCoefficient.Visible = false;
                        txtBxPCoefficient.Visible = false;

                        lbldeadband.Visible = false;
                        txtBxdeadband.Visible = false;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 1 &&
                        CmbBx2ndout.SelectedIndex == 1) // c c
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = true;
                        txtBxCtrlPer1.Visible = true;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = true;
                        txtBxctrlPer2.Visible = true;

                        lblPCoefficient.Visible = false;
                        txtBxPCoefficient.Visible = false;

                        lbldeadband.Visible = false;
                        txtBxdeadband.Visible = false;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 0 &&
                        CmbBx2ndout.SelectedIndex == 1) // h c
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = true;
                        txtBxCtrlPer1.Visible = true;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = true;
                        txtBxctrlPer2.Visible = true;

                        lblPCoefficient.Visible = true;
                        txtBxPCoefficient.Visible = true;

                        lbldeadband.Visible = true;
                        txtBxdeadband.Visible = true;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 1 &&
                        CmbBx2ndout.SelectedIndex == 0) // c h
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = true;
                        txtBxCtrlPer1.Visible = true;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = true;
                        txtBxctrlPer2.Visible = true;

                        lblPCoefficient.Visible = true;
                        txtBxPCoefficient.Visible = true;

                        lbldeadband.Visible = true;
                        txtBxdeadband.Visible = true;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 2 &&
                        (CmbBx2ndout.SelectedIndex == 0 ||
                            CmbBx2ndout.SelectedIndex == 1)) // a h,c
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = false;
                        txtBxCtrlPer1.Visible = false;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = true;
                        txtBxctrlPer2.Visible = true;

                        lblPCoefficient.Visible = false;
                        txtBxPCoefficient.Visible = false;

                        lbldeadband.Visible = false;
                        txtBxdeadband.Visible = false;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = true;
                        grpbx_Alarm2.Visible = false;
                        grpbx_Alarm.Visible = true;
                    }
                    else if ((CmbBx1stout.SelectedIndex == 0 ||
                            CmbBx1stout.SelectedIndex == 1) &&
                        CmbBx2ndout.SelectedIndex == 2) // h,c a
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = true;
                        txtBxCtrlPer1.Visible = true;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = false;
                        txtBxctrlPer2.Visible = false;

                        lblPCoefficient.Visible = false;
                        txtBxPCoefficient.Visible = false;

                        lbldeadband.Visible = false;
                        txtBxdeadband.Visible = false;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = true;
                        grpbx_Alarm.Visible = true;
                    }
                    //-----------------------------------------------------------------------------
                    #endregion
                    break;

                //ON/OFF
                case 1:
                    #region B
                    grpBxOnOff.Visible = true;
                    #endregion

                    #region B2
                    if (CmbBx1stout.SelectedIndex == 0 &&
                CmbBx2ndout.SelectedIndex == 0) // h h
                    {
                        lblHysteresis1.Visible = true;
                        txtHysteresis1.Visible = true;

                        lblHysteresis2.Visible = true;
                        txtHysteresis2.Visible = true;

                        lblHysteresisDeadBand.Visible = false;
                        txtHysteresisDeadBand.Visible = false;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 1 &&
                        CmbBx2ndout.SelectedIndex == 1) // c c
                    {
                        lblHysteresis1.Visible = true;
                        txtHysteresis1.Visible = true;

                        lblHysteresis2.Visible = true;
                        txtHysteresis2.Visible = true;

                        lblHysteresisDeadBand.Visible = false;
                        txtHysteresisDeadBand.Visible = false;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 0 &&
                        CmbBx2ndout.SelectedIndex == 1) // h c
                    {
                        lblHysteresis1.Visible = true;
                        txtHysteresis1.Visible = true;

                        lblHysteresis2.Visible = true;
                        txtHysteresis2.Visible = true;

                        lblHysteresisDeadBand.Visible = true;
                        txtHysteresisDeadBand.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 1 &&
                        CmbBx2ndout.SelectedIndex == 0) // c h
                    {
                        lblHysteresis1.Visible = true;
                        txtHysteresis1.Visible = true;

                        lblHysteresis2.Visible = true;
                        txtHysteresis2.Visible = true;

                        lblHysteresisDeadBand.Visible = true;
                        txtHysteresisDeadBand.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 2 &&
                        (CmbBx2ndout.SelectedIndex == 0 ||
                            CmbBx2ndout.SelectedIndex == 1)) // a h,c
                    {
                        lblHysteresis1.Visible = false;
                        txtHysteresis1.Visible = false;

                        lblHysteresis2.Visible = true;
                        txtHysteresis2.Visible = true;

                        lblHysteresisDeadBand.Visible = false;
                        txtHysteresisDeadBand.Visible = false;

                        grpbx_Alarm1.Visible = true;
                        grpbx_Alarm2.Visible = false;
                        grpbx_Alarm.Visible = true;
                    }
                    else if ((CmbBx1stout.SelectedIndex == 0 ||
                            CmbBx1stout.SelectedIndex == 1) &&
                        CmbBx2ndout.SelectedIndex == 2) // h,c a
                    {
                        lblHysteresis1.Visible = true;
                        txtHysteresis1.Visible = true;

                        lblHysteresis2.Visible = false;
                        txtHysteresis2.Visible = false;

                        lblHysteresisDeadBand.Visible = false;
                        txtHysteresisDeadBand.Visible = false;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = true;
                        grpbx_Alarm.Visible = true;
                    }
                    //-----------------------------------------------------------------------------
                    #endregion
                    break;

                //Manual
                case 2:
                    #region C
                    grpBxManual.Visible = true;
                    #endregion

                    #region C2
                    if (CmbBx1stout.SelectedIndex == 0 &&
                CmbBx2ndout.SelectedIndex == 0) // h h
                    {
                        lblOut1.Visible = true;
                        txtBxOut1.Visible = true;

                        lblCtrlPeriod1.Visible = true;
                        txtBxCtrlPeriod1.Visible = true;

                        lblOut2.Visible = true;
                        txtBxOut2.Visible = true;

                        lblCtrlPeriod1.Visible = true;
                        txtBxCtrlPeriod2.Visible = true;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOHigh.Visible = true;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOLow.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 1 &&
                        CmbBx2ndout.SelectedIndex == 1) // c c
                    {
                        lblOut1.Visible = true;
                        txtBxOut1.Visible = true;

                        lblCtrlPeriod1.Visible = true;
                        txtBxCtrlPeriod1.Visible = true;

                        lblOut2.Visible = true;
                        txtBxOut2.Visible = true;

                        lblCtrlPeriod1.Visible = true;
                        txtBxCtrlPeriod2.Visible = true;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOHigh.Visible = true;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOLow.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 0 &&
                        CmbBx2ndout.SelectedIndex == 1) // h c
                    {
                        lblOut1.Visible = true;
                        txtBxOut1.Visible = true;

                        lblCtrlPeriod1.Visible = true;
                        txtBxCtrlPeriod1.Visible = true;

                        lblOut2.Visible = true;
                        txtBxOut2.Visible = true;

                        lblCtrlPeriod1.Visible = true;
                        txtBxCtrlPeriod2.Visible = true;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOHigh.Visible = true;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOLow.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 1 &&
                        CmbBx2ndout.SelectedIndex == 0) // c h
                    {
                        lblOut1.Visible = true;
                        txtBxOut1.Visible = true;

                        lblCtrlPeriod1.Visible = true;
                        txtBxCtrlPeriod1.Visible = true;

                        lblOut2.Visible = true;
                        txtBxOut2.Visible = true;

                        lblCtrlPeriod1.Visible = true;
                        txtBxCtrlPeriod2.Visible = true;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOHigh.Visible = true;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOLow.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 2 &&
                        (CmbBx2ndout.SelectedIndex == 0 ||
                            CmbBx2ndout.SelectedIndex == 1)) // a h,c
                    {
                        lblOut1.Visible = false;
                        txtBxOut1.Visible = false;

                        lblCtrlPeriod1.Visible = false;
                        txtBxCtrlPeriod1.Visible = false;

                        lblOut2.Visible = true;
                        txtBxOut2.Visible = true;

                        lblCtrlPeriod1.Visible = true;
                        txtBxCtrlPeriod2.Visible = true;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOHigh.Visible = true;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOLow.Visible = true;

                        grpbx_Alarm1.Visible = true;
                        grpbx_Alarm2.Visible = false;
                        grpbx_Alarm.Visible = true;
                    }
                    else if ((CmbBx1stout.SelectedIndex == 0 ||
                            CmbBx1stout.SelectedIndex == 1) &&
                        CmbBx2ndout.SelectedIndex == 2) // h,c a
                    {
                        lblOut1.Visible = true;
                        txtBxOut1.Visible = true;

                        lblCtrlPeriod1.Visible = true;
                        txtBxCtrlPeriod1.Visible = true;

                        lblOut2.Visible = false;
                        txtBxOut2.Visible = false;

                        lblCtrlPeriod1.Visible = false;
                        txtBxCtrlPeriod2.Visible = false;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOHigh.Visible = true;

                        lblTuneOHigh.Visible = true;
                        txtBxTuneOLow.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = true;
                        grpbx_Alarm.Visible = true;
                    }
                    //-----------------------------------------------------------------------------
                    #endregion
                    break;

                //Ramp Soak
                case 3:
                    #region D
                    grpbx_PID_Parameter.Visible = true;
                    cmbBxSetvalue.Visible = true;
                    txtBxSetvalue.Visible = false;
                    #endregion

                    #region D2
                    if (CmbBx1stout.SelectedIndex == 0 &&
                CmbBx2ndout.SelectedIndex == 0) // h h
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = true;
                        txtBxCtrlPer1.Visible = true;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = true;
                        txtBxctrlPer2.Visible = true;

                        lblPCoefficient.Visible = false;
                        txtBxPCoefficient.Visible = false;

                        lbldeadband.Visible = false;
                        txtBxdeadband.Visible = false;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 1 &&
                        CmbBx2ndout.SelectedIndex == 1) // c c
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = true;
                        txtBxCtrlPer1.Visible = true;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = true;
                        txtBxctrlPer2.Visible = true;

                        lblPCoefficient.Visible = false;
                        txtBxPCoefficient.Visible = false;

                        lbldeadband.Visible = false;
                        txtBxdeadband.Visible = false;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 0 &&
                        CmbBx2ndout.SelectedIndex == 1) // h c
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = true;
                        txtBxCtrlPer1.Visible = true;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = true;
                        txtBxctrlPer2.Visible = true;

                        lblPCoefficient.Visible = true;
                        txtBxPCoefficient.Visible = true;

                        lbldeadband.Visible = true;
                        txtBxdeadband.Visible = true;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 1 &&
                        CmbBx2ndout.SelectedIndex == 0) // c h
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = true;
                        txtBxCtrlPer1.Visible = true;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = true;
                        txtBxctrlPer2.Visible = true;

                        lblPCoefficient.Visible = true;
                        txtBxPCoefficient.Visible = true;

                        lbldeadband.Visible = true;
                        txtBxdeadband.Visible = true;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = false;
                    }
                    else if (CmbBx1stout.SelectedIndex == 2 &&
                        (CmbBx2ndout.SelectedIndex == 0 ||
                            CmbBx2ndout.SelectedIndex == 1)) // a h,c
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = false;
                        txtBxCtrlPer1.Visible = false;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = true;
                        txtBxctrlPer2.Visible = true;

                        lblPCoefficient.Visible = false;
                        txtBxPCoefficient.Visible = false;

                        lbldeadband.Visible = false;
                        txtBxdeadband.Visible = false;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = true;
                        grpbx_Alarm2.Visible = false;
                        grpbx_Alarm.Visible = true;
                    }
                    else if ((CmbBx1stout.SelectedIndex == 0 ||
                            CmbBx1stout.SelectedIndex == 1) &&
                        CmbBx2ndout.SelectedIndex == 2) // h,c a
                    {
                        lblPD.Visible = true;
                        txtBxPD.Visible = true;

                        lblTi.Visible = true;
                        txtBxTi.Visible = true;

                        lblTd.Visible = true;
                        txtBxTd.Visible = true;

                        lblCtrlPer1.Visible = true;
                        txtBxCtrlPer1.Visible = true;

                        lblIoffset.Visible = true;
                        txtBxIoffset.Visible = true;

                        lblctrlPer2.Visible = false;
                        txtBxctrlPer2.Visible = false;

                        lblPCoefficient.Visible = false;
                        txtBxPCoefficient.Visible = false;

                        lbldeadband.Visible = false;
                        txtBxdeadband.Visible = false;

                        lblAutotuning.Visible = true;
                        CmbBxAutotuning.Visible = true;

                        grpbx_Alarm1.Visible = false;
                        grpbx_Alarm2.Visible = true;
                        grpbx_Alarm.Visible = true;
                    }
                    //-----------------------------------------------------------------------------
                    #endregion
                    break;
            }

            #region E
            if (out1 == 0 && out2 == 0) // h h
            {
                lblCtrlPer1.Visible = true;
                txtBxCtrlPer1.Visible = true;

                lblctrlPer2.Visible = true;
                txtBxctrlPer2.Visible = true;
            }
            else if (out1 == 0 && out2 == 1) //h c
            {
                lblPCoefficient.Visible = true;
                txtBxPCoefficient.Visible = true;

                lbldeadband.Visible = true;
                txtBxdeadband.Visible = true;

                lblCtrlPer1.Visible = true;
                txtBxCtrlPer1.Visible = true;

                lblctrlPer2.Visible = true;
                txtBxctrlPer2.Visible = true;
            }
            else if (out1 == 0 && out2 == 2) // h a
            {
                lblPCoefficient.Visible = false;
                txtBxPCoefficient.Visible = false;

                lbldeadband.Visible = false;
                txtBxdeadband.Visible = false;

                //grpbx_Alarm1.Visible = true;

                lblCtrlPer1.Visible = true;
                txtBxCtrlPer1.Visible = true;

                lblctrlPer2.Visible = false;
                txtBxctrlPer2.Visible = false;
            }
            else if (out1 == 1 && out2 == 0) // c h 
            {
                lblPCoefficient.Visible = true;
                txtBxPCoefficient.Visible = true;

                lbldeadband.Visible = true;
                txtBxdeadband.Visible = true;

                lblCtrlPer1.Visible = true;
                txtBxCtrlPer1.Visible = true;

                lblctrlPer2.Visible = true;
                txtBxctrlPer2.Visible = true;
            }
            else if (out1 == 1 && out2 == 1) // c c 
            {
                lblCtrlPer1.Visible = true;
                txtBxCtrlPer1.Visible = true;

                lblctrlPer2.Visible = true;
                txtBxctrlPer2.Visible = true;
            }
            else if (out1 == 1 && out2 == 2) // c a 
            {
                //grpbx_Alarm2.Visible = true;

                lblCtrlPer1.Visible = true;
                txtBxCtrlPer1.Visible = true;

                lblctrlPer2.Visible = false;
                txtBxctrlPer2.Visible = false;
            }
            else if (out1 == 2 && out2 == 0) // a h 
            {
                //grpbx_Alarm1.Visible = true;

                lblCtrlPer1.Visible = false;
                txtBxCtrlPer1.Visible = false;

                lblctrlPer2.Visible = true;
                txtBxctrlPer2.Visible = true;
            }
            else if (out1 == 2 && out2 == 1) // a c 
            {
                //grpbx_Alarm1.Visible = true;

                lblCtrlPer1.Visible = false;
                txtBxCtrlPer1.Visible = false;

                lblctrlPer2.Visible = true;
                txtBxctrlPer2.Visible = true;
            }
            else if (out1 == 2 && out2 == 2) // a a
            {
                grpbx_Alarm1.Visible = true;
                grpbx_Alarm2.Visible = true;
                grpbx_Alarm.Visible = true;

                lblCtrlPer1.Visible = false;
                txtBxCtrlPer1.Visible = false;

                lblctrlPer2.Visible = false;
                txtBxctrlPer2.Visible = false;
            }
            #endregion
            //-----------------------------------------------------------------------------

            //grpbx_Alarm.Visible = grpbx_Alarm1.Visible || grpbx_Alarm2.Visible;
        }

        private void txtBxOut2_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBxSetvalue_TextChanged(object sender, EventArgs e)
        {
            ReadFlag = false;
            StaticVarAddress = "4701";
            int decPlace = 1;
            int index = 0;
            CmbBxfraction.Invoke((Action)(() => index = Convert.ToInt32(CmbBxfraction.SelectedIndex)));
            switch (index)
            {
                case 0:
                    decPlace = 1;
                    break;
                case 1:
                    decPlace = 10;
                    break;
                case 2:
                    decPlace = 100;
                    break;
                case 3:
                    decPlace = 1000;
                    break;
            }

            double val = string.IsNullOrEmpty(txtBxSetvalue.Text) ? 0 : Convert.ToDouble(txtBxSetvalue.Text);

            StaticVarValue = IntToHex(Convert.ToInt32(val * 10)).PadLeft(4, '0');
        }

        private void btnConnect3_Click(object sender, EventArgs e)
        {
            int node = Convert.ToInt32(nodeAddress3.Value);

            if (node > 0)
            {
                string nodeAddress = node.ToString(); //"01";

                Thread t = new Thread(() => SampleThread(nodeAddress));

                t.SetApartmentState(ApartmentState.STA);

                t.Start();

                while (t.IsAlive)
                {
                    Application.DoEvents();
                }
            }
        }

        private void groupBox9_Enter(object sender, EventArgs e)
        {

        }

        public static int HexToInt(string hexValue)
        {
            // Convert the hex string back to the number
            return int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
        }

        public static string IntToHex(int intValue)
        {
            // Convert integer 182 as a hex in a string variable
            return intValue.ToString("X");
        }

        private void CmbBx1stout_SelectedValueChanged(object sender, EventArgs e)
        {

        }


    }
}
