using ClassList;
using RTC_Communication_Utility.UserControls;
using System;
using System.Collections;
using System.Collections.Concurrent;
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

namespace RTC_Communication_Utility
{
    public partial class MonitorForm : Form
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
            NodeName = null
        };
        #endregion

        public static string rampSoakNodeAddress = "15";

        List<MonitorUserControl> usrList = null;
        ConcurrentDictionary<int, int> usrDictionary = null;
        ConcurrentDictionary<int, int> usrDictionaryCopy = null;
        ArrayList selectedNodeList = new ArrayList(8);

        private ManualResetEvent resetEvent = new ManualResetEvent(true);

        bool _keepRunning = false;
        bool _pause = false;
        bool _readFlag = true;
        bool _isRunning = false;
        Thread th1 = null;
        string m_exePath1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public clsModbus modObj = null;

        int count = 0;

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

        static string StaticVarAddress = "";
        static string StaticVarValue = "";

        public MonitorForm()
        {
            InitializeComponent();

            modObj = new clsModbus();

            usrList = new List<MonitorUserControl>();
            usrDictionary = new ConcurrentDictionary<int, int>();
            usrDictionaryCopy = new ConcurrentDictionary<int, int>();

            try
            {
                #region Addcontrols
                MonitorUserControl obj1 = new MonitorUserControl()
                {
                    NodeId = 1,
                    NodeAddress = 0,
                    PV = "0",
                    SV = "0",
                    SelectedNode = false,
                    Connected = false,
                    Out1Percent = 0,
                    Out2Percent = 0,
                    Alarm1 = false,
                    Alarm2 = false
                };
                obj1.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
                obj1.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
                obj1.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

                MonitorUserControl obj2 = new MonitorUserControl()
                {
                    NodeId = 2,
                    NodeAddress = 0,
                    PV = "0",
                    SV = "0",
                    SelectedNode = false,
                    Connected = false,
                    Out1Percent = 0,
                    Out2Percent = 0,
                    Alarm1 = false,
                    Alarm2 = false
                };
                obj2.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
                obj2.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
                obj2.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

                MonitorUserControl obj3 = new MonitorUserControl()
                {
                    NodeId = 3,
                    NodeAddress = 0,
                    PV = "0",
                    SV = "0",
                    SelectedNode = false,
                    Connected = false,
                    Out1Percent = 0,
                    Out2Percent = 0,
                    Alarm1 = false,
                    Alarm2 = false
                };
                obj3.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
                obj3.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
                obj3.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

                MonitorUserControl obj4 = new MonitorUserControl()
                {
                    NodeId = 4,
                    NodeAddress = 0,
                    PV = "0",
                    SV = "0",
                    SelectedNode = false,
                    Connected = false,
                    Out1Percent = 0,
                    Out2Percent = 0,
                    Alarm1 = false,
                    Alarm2 = false
                };
                obj4.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
                obj4.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
                obj4.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

                MonitorUserControl obj5 = new MonitorUserControl()
                {
                    NodeId = 5,
                    NodeAddress = 0,
                    PV = "0",
                    SV = "0",
                    SelectedNode = false,
                    Connected = false,
                    Out1Percent = 0,
                    Out2Percent = 0,
                    Alarm1 = false,
                    Alarm2 = false
                };
                obj5.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
                obj5.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
                obj5.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

                MonitorUserControl obj6 = new MonitorUserControl()
                {
                    NodeId = 6,
                    NodeAddress = 0,
                    PV = "0",
                    SV = "0",
                    SelectedNode = false,
                    Connected = false,
                    Out1Percent = 0,
                    Out2Percent = 0,
                    Alarm1 = false,
                    Alarm2 = false
                };
                obj6.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
                obj6.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
                obj6.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

                MonitorUserControl obj7 = new MonitorUserControl()
                {
                    NodeId = 7,
                    NodeAddress = 0,
                    PV = "0",
                    SV = "0",
                    SelectedNode = false,
                    Connected = false,
                    Out1Percent = 0,
                    Out2Percent = 0,
                    Alarm1 = false,
                    Alarm2 = false
                };
                obj7.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
                obj7.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
                obj7.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

                MonitorUserControl obj8 = new MonitorUserControl()
                {
                    NodeId = 8,
                    NodeAddress = 0,
                    PV = "0",
                    SV = "0",
                    SelectedNode = false,
                    Connected = false,
                    Out1Percent = 0,
                    Out2Percent = 0,
                    Alarm1 = false,
                    Alarm2 = false
                };
                obj8.AddItemCallback = new MonitorUserControl.AddItemDelegate(this.AddItemCallbackFn);
                obj8.RemoveItemCallback = new MonitorUserControl.RemoveItemDelegate(this.RemoveItemCallbackFn);
                obj8.SelectedItemCallback = new MonitorUserControl.SelectedItemDelegate(this.SelectedItemCallbackFn);

                usrList.Add(obj1);
                usrList.Add(obj2);
                usrList.Add(obj3);
                usrList.Add(obj4);
                usrList.Add(obj5);
                usrList.Add(obj6);
                usrList.Add(obj7);
                usrList.Add(obj8);

                foreach (var item in usrList)
                {
                    flowLayoutPanel1.Controls.Add(item);
                }
                flowLayoutPanel2.Controls.Add(monitorParams);
                flowLayoutPanel2.Visible = false;
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InstantiateThread()
        {
            while (_isRunning)
            {
                if (_pause)
                {
                    this.resetEvent.WaitOne();

                }
                //Thread.Sleep(500);
                foreach (var nodeAddress in usrDictionaryCopy.Keys)
                {
                    if (_pause)
                    {
                        this.resetEvent.WaitOne();
                    }
                    //Thread.Sleep(500);
                    if (modObj.IsSerialPortOpen())
                    {
                        if (_readFlag)
                        {
                            for (int k = 0; k < frameList.Count; k++)
                            {
                                if (_pause)
                                {
                                    this.resetEvent.WaitOne();
                                }
                                //Thread.Sleep(500);

                                var list = CreateFrames(nodeAddress.ToString(), frameList[k][1],
                                            frameList[k][2], frameList[k][3], _readFlag);

                                #region SendFrames
                                RTU_Response(k, list);
                                #endregion
                            }
                        }
                        else
                        {
                            //Write
                            var list = CreateFrames(nodeAddress.ToString(), "06", StaticVarAddress, StaticVarValue, _readFlag);
                            if (list != null)
                            {
                                _readFlag = true;
                            }
                        }
                    }
                }
            }
        }

        private void RTU_Response(int k, List<string> list)
        {
            if (list != null && list.Count > 0)
            {
                switch (frameList[k][0])
                {
                    case "0":
                        //funct_0(list);
                        foreach (var ctrll in usrList)
                        {
                            if (ctrll.Connected)
                            {
                                if (ctrll.SelectedNode)
                                {

                                    //monitorParams.Invoke((Action)(() => monitorParams.UnitType = string.IsNullOrEmpty(list[1]) ? -1 : Convert.ToInt16(string.Join(",", list[1]), 16)));
                                }
                            }
                        }
                        break;
                    case "1":
                        if (list.Count == 6)
                        {
                            foreach (var ctrll in usrList)
                            {
                                if (ctrll.Connected)
                                {
                                    int out1 = string.IsNullOrEmpty(list[0]) ? -1 : Convert.ToInt32(Convert.ToInt16(string.Join(",", list[0]), 16));
                                    int out2 = string.IsNullOrEmpty(list[5]) ? -1 : Convert.ToInt32(Convert.ToInt16(string.Join(",", list[5]), 16));

                                    ctrll.Invoke((Action)(() => ctrll.Alarm1 = (out1 == 2) ? true : false));
                                    ctrll.Invoke((Action)(() => ctrll.Alarm2 = (out2 == 2) ? true : false));

                                    if (ctrll.SelectedNode)
                                    {


                                        //471A                                                                
                                        monitorParams.Invoke((Action)(() =>
                                              monitorParams.Out1Percent = out1));
                                        //monitorParams.Out1Percent = string.IsNullOrEmpty(list[0]) ?
                                        //-1 : Convert.ToInt16(string.Join(",", list[0]), 16)));

                                        //471B
                                        //471C
                                        //471D
                                        //471E

                                        //471F   
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.Out2Percent = out2));
                                        //monitorParams.Invoke((Action)(() =>
                                        //    monitorParams.Out2Percent = string.IsNullOrEmpty(list[5]) ?
                                        //    -1 : Convert.ToInt16(string.Join(",", list[5]), 16)));
                                    }
                                }
                            }
                        }
                        break;
                    case "2":
                        if (list.Count == 4)
                        {
                            foreach (var ctrll in usrList)
                            {
                                if (ctrll.Connected)
                                {
                                    string pv = GetDecimalPlaces(string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]));
                                    string sv = GetDecimalPlaces(string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]));
                                    int out1 = string.IsNullOrEmpty(list[2]) ? -1 : Convert.ToInt32(Convert.ToInt16(string.Join(",", list[2]), 16));
                                    int out2 = string.IsNullOrEmpty(list[3]) ? -1 : Convert.ToInt32(Convert.ToInt16(string.Join(",", list[3]), 16));


                                    ctrll.Invoke((Action)(() => ctrll.PV = pv));
                                    ctrll.Invoke((Action)(() => ctrll.SV = sv));
                                    ctrll.Invoke((Action)(() => ctrll.Out1Percent = out1));
                                    ctrll.Invoke((Action)(() => ctrll.Out2Percent = out2));

                                    //ctrll.Invoke((Action)(() =>
                                    //    ctrll.PV = string.IsNullOrEmpty(list[0]) ? "0" :
                                    //    ConvertShortToHexString(list[0])));
                                    //ctrll.Invoke((Action)(() =>
                                    //    ctrll.SV = string.IsNullOrEmpty(list[1]) ? "0" :
                                    //    ConvertShortToHexString(list[1])));

                                    //ctrll.Invoke((Action)(() =>
                                    //  ctrll.Out1Percent = string.IsNullOrEmpty(list[2]) ?
                                    //                    0 :
                                    //                    Convert.ToInt32(GetDecimalPlaces(ConvertShortToHexString(list[2])))
                                    //                    ));

                                    //ctrll.Invoke((Action)(() =>
                                    //  ctrll.Out2Percent = string.IsNullOrEmpty(list[3]) ?
                                    //                    0 :
                                    //                    Convert.ToInt32(GetDecimalPlaces(ConvertShortToHexString(list[3])))
                                    // 
                                    //));
                                    ctrll.Invoke((Action)(() => ctrll.Alarm1 = (out1 == 2) ? true : false));
                                    ctrll.Invoke((Action)(() => ctrll.Alarm2 = (out2 == 2) ? true : false));


                                    if (ctrll.SelectedNode)
                                    {
                                        //4700

                                        //4701
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.SV11 = sv));

                                        //4702
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.Out11 = out1.ToString()));

                                        //4703
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.Out12 = out2.ToString()));

                                        // String.Format("{0:0.0}", Convert.ToDouble(string.IsNullOrEmpty(list[3]) ?
                                        // "0" : ConvertShortToHexString(list[3]).ToString()) / 10)));
                                    }
                                }
                            }
                        }
                        break;
                    case "3":
                        if (list.Count == 8)
                        {
                            foreach (var ctrll in usrList)
                            {
                                if (ctrll.Connected)
                                {
                                    //string ver = string.IsNullOrEmpty(list[2]) ? "0.0" : Convert.ToInt16(string.Join(",", list[2]), 16).ToString();
                                    string ver = string.IsNullOrEmpty(list[2]) ? "0.0" : list[2];
                                    if (ctrll.SelectedNode)
                                    {
                                        //4728                                                         

                                        //4729
                                        //472A                                                                   
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.SwVersion = ver));

                                        //472B
                                        //472C
                                        //472D
                                        //472E
                                        //472F                                                             

                                    }
                                }
                            }
                        }

                        break;
                    case "4":
                        if (list.Count == 4)
                        {
                            foreach (var ctrll in usrList)
                            {
                                if (ctrll.Connected)
                                {
                                    string pv = GetDecimalPlaces(string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]));
                                    string sv = GetDecimalPlaces(string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]));

                                    //4700
                                    ctrll.Invoke((Action)(() => ctrll.PV = pv));

                                    //4701
                                    ctrll.Invoke((Action)(() => ctrll.SV = sv));

                                    if (ctrll.SelectedNode)
                                    {
                                        //4701
                                        monitorParams.Invoke((Action)(() => monitorParams.SV11 = sv));

                                        //4702
                                        //4703
                                    }
                                }
                            }
                        }
                        break;
                    case "5":
                        if (list.Count == 6)
                        {
                            foreach (var ctrll in usrList)
                            {
                                if (ctrll.Connected)
                                {
                                    switch (Convert.ToInt16(string.Join(",", list[1]), 16))
                                    {
                                        case -1:
                                            ctrll.Invoke((Action)(() => ctrll.Unit = ""));
                                            break;
                                        case 0:
                                            ctrll.Invoke((Action)(() => ctrll.Unit = "F"));
                                            break;
                                        case 1:
                                            ctrll.Invoke((Action)(() => ctrll.Unit = "C"));
                                            break;
                                        case 2:
                                            ctrll.Invoke((Action)(() => ctrll.Unit = "EU"));
                                            break;
                                    }

                                    if (ctrll.SelectedNode)
                                    {
                                        //4724
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.UnitType = string.IsNullOrEmpty(list[1]) ?
                                            -1 : Convert.ToInt16(string.Join(",", list[1]), 16)));
                                    }
                                }
                            }
                        }


                        break;
                    case "6":
                        //funct_6(list);

                        break;
                    case "7":
                        if (list.Count == 2)
                        {
                            foreach (var ctrll in usrList)
                            {
                                if (ctrll.Connected)
                                {
                                    string pv = GetDecimalPlaces(string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]));
                                    string sv = GetDecimalPlaces(string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1]));

                                    ctrll.Invoke((Action)(() =>
                                        ctrll.PV = pv));

                                    ctrll.Invoke((Action)(() =>
                                        ctrll.SV = sv));

                                    if (ctrll.SelectedNode)
                                    {
                                        monitorParams.Invoke((Action)(() => monitorParams.SV11 = sv));
                                    }
                                }
                            }
                        }

                        break;
                    case "8":
                        //funct_8(list);

                        break;
                    case "9":
                        if (list.Count == 8)
                        {
                            foreach (var ctrll in usrList)
                            {
                                if (ctrll.Connected)
                                {
                                    int alarm1M = string.IsNullOrEmpty(list[0]) ?
                                            -1 : Convert.ToInt16(string.Join(",", list[0]), 16);
                                    int alarm2M = string.IsNullOrEmpty(list[1]) ?
                                            -1 : Convert.ToInt16(string.Join(",", list[1]), 16);
                                    int run = string.IsNullOrEmpty(list[4]) ?
                                            -1 : Convert.ToInt16(string.Join(",", list[4]), 16);

                                    if (ctrll.SelectedNode)
                                    {
                                        //4720
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.Alarm1Mode = alarm1M));

                                        //4721
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.Alarm2Mode = alarm2M));

                                        //4722
                                        //4723

                                        //4724
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.RunHaltmode = run));

                                        //4725
                                        //4726
                                        //4727                                                                    
                                    }
                                }
                            }
                        }

                        break;
                    case "10":
                        if (list.Count == 8)
                        {
                            foreach (var ctrll in usrList)
                            {
                                if (ctrll.Connected)
                                {
                                    if (ctrll.SelectedNode)
                                    {
                                        //4710

                                        //4711
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.PCoefficient = list[1]));

                                        //4712
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.Deadband = list[2]));

                                        //4713
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.Hysteresis1 = list[3]));

                                        //4714
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.Hysteresis2 = list[4]));

                                        //4715
                                        //4716
                                        //4717

                                    }
                                }
                            }
                        }

                        break;
                    case "11":
                        if (list.Count == 8)
                        {
                            foreach (var ctrll in usrList)
                            {
                                if (ctrll.Connected)
                                {
                                    if (ctrll.SelectedNode)
                                    {
                                        //4718
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.SensorType = string.IsNullOrEmpty(list[0]) ? -1 :
                                            Convert.ToInt16(string.Join(",", list[0]), 16)));

                                        //4719
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.CtrlAction = string.IsNullOrEmpty(list[1]) ? -1 :
                                            Convert.ToInt16(string.Join(",", list[1]), 16)));
                                    }
                                }
                            }
                        }
                        break;
                    case "12":
                        if (list.Count == 8)
                        {
                            foreach (var ctrll in usrList)
                            {
                                if (ctrll.Connected)
                                {
                                    if (ctrll.SelectedNode)
                                    {
                                        //4708
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.Alarm2Up = list[0]));

                                        //4709
                                        monitorParams.Invoke((Action)(() =>
                                             monitorParams.Alarm2Down = list[1]));

                                        //470A
                                        //470B

                                        //470C
                                        monitorParams.Invoke((Action)(() =>
                                             monitorParams.PD = list[4]));

                                        //470D
                                        monitorParams.Invoke((Action)(() =>
                                             monitorParams.Ti = list[5]));

                                        //470E
                                        monitorParams.Invoke((Action)(() =>
                                             monitorParams.Td = list[6]));


                                        //470F

                                    }
                                }
                            }
                        }

                        break;
                    case "13":
                        if (list.Count == 7)
                        {
                            foreach (var ctrll in usrList)
                            {

                                if (ctrll.Connected)
                                {
                                    string sv = GetDecimalPlaces(string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0]));
                                    int out1 = string.IsNullOrEmpty(list[1]) ? -1 : Convert.ToInt32(Convert.ToInt16(string.Join(",", list[1]), 16));
                                    int out2 = string.IsNullOrEmpty(list[2]) ? -1 : Convert.ToInt32(Convert.ToInt16(string.Join(",", list[2]), 16));

                                    ctrll.Invoke((Action)(() => ctrll.Alarm1 = (out1 == 2) ? true : false));
                                    ctrll.Invoke((Action)(() => ctrll.Alarm2 = (out2 == 2) ? true : false));

                                    string highTemp = GetDecimalPlaces(string.IsNullOrEmpty(list[3]) ? "0" : ConvertShortToHexString(list[3]));
                                    string lowTemp = GetDecimalPlaces(string.IsNullOrEmpty(list[4]) ? "0" : ConvertShortToHexString(list[4]));

                                    string alarm1U = GetDecimalPlaces(string.IsNullOrEmpty(list[5]) ? "0" : ConvertShortToHexString(list[5]));
                                    string alarm1D = GetDecimalPlaces(string.IsNullOrEmpty(list[6]) ? "0" : ConvertShortToHexString(list[6]));

                                    //4701
                                    ctrll.Invoke((Action)(() =>
                                        ctrll.SV = sv));

                                    ctrll.Invoke((Action)(() =>
                                     ctrll.Out1Percent = out1
                                                       ));

                                    ctrll.Invoke((Action)(() =>
                                      ctrll.Out2Percent = out2
                                                        ));

                                    if (ctrll.SelectedNode)
                                    {
                                        //4701
                                        monitorParams.Invoke((Action)(() =>
                                            monitorParams.SV11 = sv));

                                        //4702
                                        monitorParams.Invoke((Action)(() =>
                                             monitorParams.Out11 = out1.ToString()));

                                        //4703
                                        monitorParams.Invoke((Action)(() =>
                                             monitorParams.Out12 = out2.ToString()));

                                        //4704
                                        monitorParams.Invoke((Action)(() =>
                                             monitorParams.HighTemp = highTemp));

                                        //4705
                                        monitorParams.Invoke((Action)(() =>
                                             monitorParams.LowTemp = lowTemp));

                                        //4706
                                        monitorParams.Invoke((Action)(() =>
                                             monitorParams.Alarm1Up = alarm1U));

                                        //4707
                                        monitorParams.Invoke((Action)(() =>
                                             monitorParams.Alarm1Down = alarm1D));
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void MonitorForm_Load(object sender, EventArgs e)
        {
            #region Port Settings
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
            #endregion
        }

        private void StartTh()
        {
            if (this._isRunning)
            {
                return;
            }

            this._isRunning = true;

            th1 = new Thread(InstantiateThread);
            th1.Start();
        }

        private void AddItemCallbackFn(string item)
        {
            try
            {
                bool updateDictionary = false;
                if (!string.IsNullOrEmpty(item))
                {
                    //find node with nodeId from node list
                    MonitorUserControl ctrl = usrList.Find(x => x.NodeId == Convert.ToInt32(item));

                    if (ctrl != null)
                    {
                        ctrl.Invoke((Action)(() =>
                                       ctrl.LabelMessage = "Please wait.."));

                        if (this._isRunning)
                        {
                            _pause = true;
                            this.resetEvent.Reset();
                        }

                        bool checkNode = this.CheckNodeAvailability(ctrl.NodeAddress.ToString());

                        if (checkNode)
                        {


                            //check if dictionary contains nodeAddress
                            //if present increase its count value
                            //otherwise add that nodeAddress
                            if (usrDictionary.ContainsKey(ctrl.NodeAddress))
                            {
                                usrDictionary[ctrl.NodeAddress]++;
                                updateDictionary = true;// need to check
                            }
                            else
                            {
                                updateDictionary = usrDictionary.TryAdd(ctrl.NodeAddress, 1);
                            }

                            ctrl.Connected = true;

                            //check if selectionlist is empty or not
                            //if empty add current nodeid to it 
                            //also set selection property true in nodelist
                            if (selectedNodeList.Count == 0)
                            {
                                if (updateDictionary)
                                {
                                    ctrl.SelectedNode = true;
                                    monitorParams.Name = item;
                                    usrList.Find(x => x.NodeId == Convert.ToInt32(item)).SelectedNode = true;
                                    usrDictionaryCopy = usrDictionary;

                                    rampSoakNodeAddress = ctrl.NodeAddress.ToString();


                                    // add nodeId to selection list
                                    selectedNodeList.Add(item);
                                    Thread.Sleep(100);
                                    ctrl.Invoke((Action)(() =>
                                      ctrl.LabelMessage = "Connected.."));

                                    StartTh();
                                }
                            }
                            else
                            {
                                if (updateDictionary)
                                {
                                    _keepRunning = false;
                                    usrDictionaryCopy = usrDictionary;
                                    _keepRunning = true;

                                    ctrl.Invoke((Action)(() =>
                                      ctrl.LabelMessage = "Connected.."));


                                    /// add nodeId to selection list
                                    selectedNodeList.Add(item);

                                    this.resetEvent.Set();
                                    _pause = false;
                                }
                            }
                            flowLayoutPanel2.Visible = true;
                        }
                        else
                        {
                            ctrl.Connected = false;
                            ctrl.NodeAddress = 0;
                            ctrl.ButtonText = "Connect";
                            ctrl.SV = "0";
                            ctrl.PV = "0";
                            ctrl.BackgroundsColor = Color.White;

                            ctrl.Invoke((Action)(() =>
                                      ctrl.LabelMessage = "Cannot connect."));

                            this.resetEvent.Set();
                            _pause = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool CheckNodeAvailability(string item)
        {
            try
            {
                if (modObj.IsSerialPortOpen())
                {
                    _keepRunning = false;
                    Thread.Sleep(10);

                    // 0F 67 0012 0003 
                    var list = CreateFrames(item, "0103", "0012", "0003", true);

                    _keepRunning = true;

                    if (list == null)
                        return false;
                    else
                        return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void RemoveItemCallbackFn(string item)
        {
            bool updateDictionary = false;
            try
            {
                if (!string.IsNullOrEmpty(item))
                {
                    MonitorUserControl ctrl = usrList.Find(x => x.NodeId == Convert.ToInt32(item));
                    ctrl.Invoke((Action)(() =>
                                    ctrl.LabelMessage = "Disconnected.."));
                    //workerThread.CancelAsync();

                    int val = 0;

                    //remove key from dictionary if present otherwise decrease its value
                    if (usrDictionary.ContainsKey(ctrl.NodeAddress))
                    {
                        usrDictionary[ctrl.NodeAddress]--;

                        updateDictionary = true;

                        int itemValue1 = 0;

                        if (updateDictionary)
                        {
                            if (usrDictionary.TryGetValue(ctrl.NodeAddress, out itemValue1))
                            {
                                if (itemValue1 == 0)
                                {
                                    usrDictionary.TryRemove(ctrl.NodeAddress, out val);
                                }

                                //remove from selection list
                                if (selectedNodeList.Contains(item))
                                {
                                    selectedNodeList.Remove(item);
                                }
                            }
                        }
                    }

                    //make other connected node selected
                    if (selectedNodeList.Count > 0)
                    {
                        MonitorUserControl ctrll = usrList.Find(x => x.NodeId == Convert.ToInt32(selectedNodeList[0]));
                        ctrll.SelectedNode = true;
                        flowLayoutPanel2.Visible = true;
                        monitorParams.Name = ctrll.NodeId.ToString();
                        if (updateDictionary)
                        {
                            rampSoakNodeAddress = ctrl.NodeAddress.ToString();

                            usrDictionaryCopy = usrDictionary;
                            // InstantiateWorkerThread();
                            //workerThread.RunWorkerAsync();


                        }
                    }
                    else if (selectedNodeList.Count <= 0)
                    {
                        flowLayoutPanel2.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Remove: " + ex.Message);
            }
        }

        private void SelectedItemCallbackFn(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                MonitorUserControl ctrl = usrList.Find(x => x.NodeId == Convert.ToInt32(item));

                //set all connected nodes selection property to false
                foreach (var ele in usrList)
                {
                    if (ele.Connected)
                    {
                        ele.SelectedNode = false;
                    }
                }
                ctrl.SelectedNode = true;

                //set current node selection property to true
                usrList.Find(x => x.NodeId == Convert.ToInt32(item)).SelectedNode = true;

                monitorParams.Name = item;
                rampSoakNodeAddress = ctrl.NodeAddress.ToString();
            }
        }

        private void BeginThreadToReadWrite(string item)
        {
            foreach (var nodeAddress in usrDictionary.Keys)
            {
                if (modObj.IsSerialPortOpen())
                {
                    if (_readFlag)
                    {
                        for (int k = 0; k < frameList.Count; k++)
                        {
                            if (!_readFlag)
                            {
                                break;
                            }

                            var list = CreateFrames(nodeAddress.ToString(), frameList[k][1],
                                frameList[k][2], frameList[k][3], _readFlag);

                            if (list != null && list.Count > 0)
                            {
                                switch (frameList[k][0])
                                {
                                    case "0":
                                        //funct_0(list);

                                        break;
                                    case "1":
                                        //funct_1(list);

                                        break;
                                    case "2":
                                        //funct_2(list);

                                        break;
                                    case "3":
                                        //funct_3(list);

                                        break;
                                    case "4":
                                        //funct_4(list);

                                        break;
                                    case "5":
                                        //funct_5(list);

                                        break;
                                    case "6":
                                        //funct_6(list);

                                        break;
                                    case "7":
                                        //funct_7(list);
                                        foreach (var ctrll in usrList)
                                        {
                                            if (ctrll.Connected)
                                            {
                                                ctrll.Invoke((Action)(() => ctrll.PV = string.IsNullOrEmpty(list[0]) ? "0" : ConvertShortToHexString(list[0])));
                                                ctrll.Invoke((Action)(() => ctrll.SV = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1])));

                                                if (ctrll.SelectedNode)
                                                {
                                                    monitorParams.Invoke((Action)(() => monitorParams.SV11 = string.IsNullOrEmpty(list[1]) ? "0" : ConvertShortToHexString(list[1])));
                                                }
                                            }
                                        }


                                        break;
                                    case "8":
                                        //funct_8(list);

                                        break;
                                    case "9":
                                        //funct_9(list);

                                        break;
                                    case "10":
                                        //funct_10(list);

                                        break;
                                    case "11":
                                        //funct_11(list);

                                        break;
                                    case "12":
                                        //funct_12(list);

                                        break;
                                    case "13":
                                        //funct_13(list);
                                        break;
                                }
                            }
                        }
                    }
                    else if (!_readFlag)
                    {
                        //Write
                        var list = CreateFrames(nodeAddress.ToString(), "06", StaticVarAddress, StaticVarValue, _readFlag);
                        //if (list != null)
                        {
                            _readFlag = true;
                        }
                    }
                }
            }
        }

        private static byte[] ExtractByteArray(byte[] RecieveData, int size, int startPostition)
        {
            byte[] sizeBytes = new byte[size];

            Array.Copy(RecieveData, startPostition, sizeBytes, 0, size);
            return sizeBytes;
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
                        RecieveData = modObj.AscFrame(Convert.ToInt32(nodeAddress).ToString("X").PadLeft(2, '0'),
                            Convert.ToInt32(functionCode).ToString("X").PadLeft(2, '0'), regAddress, wordCount);

                        if (RecieveData != null)
                        {
                            //get no. of bytes to read from recieved frame
                            byte[] sizeBytes = ExtractByteArray(RecieveData, 2, 5);

                            int size = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(sizeBytes), 16);

                            if (RecieveData.Length > size)
                            {
                                byte[] newArr = ExtractByteArray(RecieveData, size * 2, 7);

                                List<string> returnValues = new List<string>();
                                int count = 0;
                                if (read)
                                {
                                    for (int i = 0; i < size; i = i + 2)
                                    {
                                        byte[] bytes = ExtractByteArray(newArr, 4, count);

                                        string byteArrayToString = System.Text.Encoding.UTF8.GetString(bytes);

                                        returnValues.Add(byteArrayToString);
                                        count = count + 4;
                                    }
                                }
                                return returnValues;
                            }
                        }
                    }
                    else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
                    {
                        RecieveData = modObj.RtuFrame(nodeAddress, functionCode, regAddress, wordCount);

                        if (RecieveData != null)
                        {
                            //01 03 02 02 FF F9 64 
                            int size = Convert.ToInt32(RecieveData[2]);
                            if (RecieveData.Length > (size + 3))
                            {
                                byte[] newArr = new byte[size];
                                Array.Copy(RecieveData, 3, newArr, 0, size);

                                List<string> returnValues = new List<string>();
                                int count = 0;
                                if (read)
                                {
                                    for (int i = 0; i < size; i = i + 2)
                                    {
                                        byte[] bytes = new byte[2];

                                        Array.Copy(newArr, count, bytes, 0, 2);

                                        string byteArrayToString = clsModbus.ByteArrayToString(bytes);

                                        returnValues.Add(byteArrayToString);
                                        count = count + 2;
                                    }
                                }
                                return returnValues;
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("CreateFrames: " + ex.Message);
            }
            return null;
        }

        private void MonitorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (th1 != null)
            {
                if (th1.IsAlive || (th1.ThreadState == ThreadState.Running))
                {
                    th1.Abort();
                }
            }
            //if (e.CloseReason == CloseReason.UserClosing)
            //{
            //    //// Prompt user to save his data
            //    //MessageBox.Show("closing");
            //}
            //else if (e.CloseReason == CloseReason.WindowsShutDown)
            //{
            //    //// Autosave and clear up resources
            //    //MessageBox.Show("Closing forcefully");
            //}

            if (modObj != null)
            {
                //LogWriter.WriteToFile("Closing() =>", "Port closed"
                //        , "OnlineMonitor");
                modObj.CloseSerialPort();
            }
        }

        private string ConvertShortToHexString(string strValue)
        {
            try
            {
                return Convert.ToInt16(string.Join(",", strValue), 16).ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string DecodeFrameValues(string frameValue, bool frameValueToDecimal)
        {
            string result = string.IsNullOrEmpty(frameValue) ? "0" : ConvertShortToHexString(frameValue);

            return frameValueToDecimal ? GetDecimalPlaces(result) : result;
        }

        private string GetDecimalPlaces(string val)
        {
            string value = (string.IsNullOrEmpty(val) ? "0" : val);
            int index = 0;
            index = monitorParams.DecimalPlace;
            //CmbBxfraction.Invoke((Action)(() => index = Convert.ToInt32(CmbBxfraction.SelectedIndex)));
            //index = monitorParams.Fraction;
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

        private void rampSoakProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RampSoakForm rampSoakForm = new RampSoakForm(modObj);
            rampSoakForm.NodeAddress = "1";// Convert.ToString(nodeAddress1.Value); // "1";
            rampSoakForm.Show();

        }
    }
}

