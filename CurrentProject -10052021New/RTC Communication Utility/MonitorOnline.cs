using ClassList;
using ClosedXML.Excel;
using RTC_Communication_Utility.UserControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RTC_Communication_Utility
{
    public partial class MonitorOnline : Form
    {
        int ATT;
        int annotationCounter, cnt = 0;
        bool onchangeeHigh, onchangeeSetvalue, onchangeeLowTemp, onchangeHyst1, onchangeHyst2,
            OnchangeHysDead, onchangeOut1, OnchangetxtBxCtrlPeriod1, onchangetxtBxOut2,
            onchangetxtBxCtrlPeriod2, onchangetxtBxPD, onchangetxtBxTi,
            onchangetxtBxTd, onchangetxtBxdeadband, onchangetxtBxctrlPer2,
            onchangetxtBxPCoefficient, PVV, onchangeIoffset, onchangePDoffset, LeaveEvent = false;
        static AutoResetEvent _AREvt;
        int countChart = 0;
        DataTable MoniterAllData = null;
        string pathdExcelata = string.Empty;

        int countt = 0;
        int OLK = 0;
        string pvdat = string.Empty;
        string pv = "0";
        string sv = "0";
        decimal out1 = 0;
        decimal out2 = 0;

        string key = string.Empty;
        string value = string.Empty;
        int[] errorCount;
        ArrayList newList = new ArrayList();
        string regadd = string.Empty;
        string oout2 = string.Empty;
        int o22 = 0;
        // List<string> val;
        string led = string.Empty;
        string ledd470A = string.Empty;
        int bb = 0;
        int c = 0;
        double ver = 0.0;
        int step = 0;
        // int val2 = 0;

        string regaddd = string.Empty;
        string oAT = string.Empty;
        string oAT4700 = string.Empty;
        int remTime = 0;
        string str = string.Empty;
        int hours = 0;
        int minutes = 0;
        int seconds = 0;

        // int valInt = 0;
        double coef = 0.0;
        double hy1 = 0.0;
        double hy2 = 0.0;
        double hy3 = 0.0;

        double a2U = 0.0;
        double a2D = 0.0;
        double pb = 0.0;
        int td = 0;
        // double ti = 0.0;
        double ioffset = 0.0;
        // double PvValue = 0.0;
        // double GraphSV = 0.0;
        string nodeAddress = string.Empty;
        int val11 = 0;
        int val12 = 0;

        int ledd = 0;
        int val2BindUnitType = 0;
        bool ErrFlag = false;
        int vall471A = 0;
        int valOff = 0;
        int o2out = 0;
        int val2CmbBxUnitType = 0;
        int a1cmbBxAlarm1Mode = 0;
        int a2cmbBxAlarm2Mode = 0;
        int valIntCmbBxRunHaltmode = 0;
        int a1CmbBxSensorType = 0;
        int a2CmbBxCtrlAction = 0;
        int oA2CmbBxAutotuning = 0;
        //int oA2CmbBxctrlper2 = 0;

        string oATT4728 = string.Empty;
        double oA2470E = 0;

        Dictionary<Int32, Int32> SensertypeDict = null;
        Dictionary<Int32, Int32> SensertypeDictread = null;

        public void FillDict()
        {
            SensertypeDict = new Dictionary<Int32, Int32> {
                    { 0, 0 },
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 6, 6 },
                    { 7, 7 },
                    { 8, 11 },
                    { 9, 12 },
                    { 10, 13 },
                    { 11, 14 },
                    { 12, 15 },
                    { 13, 16 },
                    { 14, 17 },
                    { 15, 18 }, 
             };
            SensertypeDictread = new Dictionary<Int32, Int32> {
                    { 0, 0 },
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 6, 6 },
                    { 7, 7 },
                    { 11, 8 },
                    { 12, 9 },
                    { 13, 10 },
                    { 14, 11 },
                    { 15, 12 },
                    { 16, 13 },
                    { 17, 14 },
                    { 18, 15 },
             };
        }
        public MonitorOnline()
        {


            InitializeComponent();

            pics[1] = pictureBox1;
            pics[2] = pictureBox2;
            pics[3] = pictureBox3;
            pics[4] = pictureBox4;
            pics[5] = pictureBox5;
            pics[6] = pictureBox6;
            pics[7] = pictureBox7;
            pics[8] = pictureBox8;
            //pics[9] = pictureBox9;
            FillDict();
            // create modbus instance
            modbusobj = new clsModbus();

            //attach delegate event
            modbusobj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);

            //struct instance 
            myStruct = new DeviceSettings();

            // create new instance list of usercontrol
            usrList = new List<UserControl1>();

            // Dictionary instance to store the node and its count when connected
            usrDictionary = new Dictionary<string, int>();

            // copy Dictionary instance to store the node and its count when connected
            usrDictionaryCopy = new Dictionary<string, int>();

            // thread to read/write
            InstantiateThread();

            // initialize usercontrols and add to form
            #region Usercontrols

            try
            {
                UserControl1 user1 = new UserControl1()
                {
                    NodeId = "1",
                    NodeAddress = 0,
                    ButtonText = "Connect",
                    SelectedNode = false,
                    Connected = false,
                    Alarm1 = false,
                    Alarm2 = false,
                    PatternStepBool = false,
                    RemainTimeBool = false
                };
                user1.AddItemCallback = new UserControl1.AddItemDelegate(UserAddConnect);
                user1.RemoveItemCallback = new UserControl1.RemoveItemDelegate(UserRemoveConnect);
                user1.SelectedItemCallback = new UserControl1.SelectedItemDelegate(UserSelectedConnect);

                UserControl1 user2 = new UserControl1()
                {
                    NodeId = "2",
                    NodeAddress = 0,
                    ButtonText = "Connect",
                    SelectedNode = false,
                    Connected = false,
                    Alarm1 = false,
                    Alarm2 = false,
                    PatternStepBool = false,
                    RemainTimeBool = false
                };
                user2.AddItemCallback = new UserControl1.AddItemDelegate(UserAddConnect);
                user2.RemoveItemCallback = new UserControl1.RemoveItemDelegate(UserRemoveConnect);
                user2.SelectedItemCallback = new UserControl1.SelectedItemDelegate(UserSelectedConnect);

                UserControl1 user3 = new UserControl1()
                {
                    NodeId = "3",
                    NodeAddress = 0,
                    ButtonText = "Connect",
                    SelectedNode = false,
                    Connected = false,
                    Alarm1 = false,
                    Alarm2 = false,
                    PatternStepBool = false,
                    RemainTimeBool = false
                };
                user3.AddItemCallback = new UserControl1.AddItemDelegate(UserAddConnect);
                user3.RemoveItemCallback = new UserControl1.RemoveItemDelegate(UserRemoveConnect);
                user3.SelectedItemCallback = new UserControl1.SelectedItemDelegate(UserSelectedConnect);

                UserControl1 user4 = new UserControl1()
                {
                    NodeId = "4",
                    NodeAddress = 0,
                    ButtonText = "Connect",
                    SelectedNode = false,
                    Connected = false,
                    Alarm1 = false,
                    Alarm2 = false,
                    PatternStepBool = false,
                    RemainTimeBool = false
                };
                user4.AddItemCallback = new UserControl1.AddItemDelegate(UserAddConnect);
                user4.RemoveItemCallback = new UserControl1.RemoveItemDelegate(UserRemoveConnect);
                user4.SelectedItemCallback = new UserControl1.SelectedItemDelegate(UserSelectedConnect);

                usrList.Add(user1);
                usrList.Add(user2);
                usrList.Add(user3);
                usrList.Add(user4);

                foreach (var item in usrList)
                {
                    flowLayoutPanel1.Controls.Add(item);
                }
            #endregion

            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }


        private void singlecmdtxt()
        {

        }
        PictureBox[] pics = new PictureBox[10];
        public DeviceSettings myStruct;
        private ManualResetEvent _manualResetEvent = new ManualResetEvent(true);
        private GraphicsPath gp = null;
        private Region rg = null;
        private bool autoDisconnect = false;
        #region Variables
        private clsModbus modbusobj = null;
        private Thread th = null;
        private Thread mainThread = null;

        private RampSoakForm frm = null;
        private bool running = false;
        private bool online = false;
        private bool ramp = false;
        private bool pvRec = false;
        private bool Writedatab = false;
        private bool _isRunning = false;
        private bool _pause = false;
        private bool _loopBreak = false;
        private bool ctrlVal = true;
        private bool setVal = false;
        private bool runVal = true;
        private bool out1Val = true;
        private bool out2Val = true;
        private bool senseVal = true;
        private bool unitVal = true;
        private bool highVal = false;
        private bool lowVal = false;
        private bool alM1Val = true;
        private bool alM1UVal = true;
        private bool alM1DVal = true;
        private bool alM2Val = true;
        private bool alM2UVal = true;
        private bool alM2DVal = true;
        private bool hys1V = false;
        private bool hys2V = false;
        private bool deadV = true;
        private bool out1V = false;
        private bool out2V = false;
        private bool ctrl1V = true;
        private bool ctrl2V = true;
        private bool pdV = false;
        private bool tiV = true;
        private bool tdV = true;
        private bool coefV = true;
        private bool offVal = true;
        private int rampCount = 1;
        private bool autoTune = true;
        private bool pvOffset = true;
        char[] charArrAT = null;
        char[] charArratLED = null;
        private static ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        #endregion

        private List<UserControl1> usrList = null;

        private Dictionary<string, int> usrDictionary = null;
        private Dictionary<string, int> usrDictionaryCopy = null;
        private ArrayList selectedNodeList = new ArrayList(8);
        private int selectedNode = 0;

        #region Frames
        private List<List<string>> frameList = new List<List<string>>()
        {
            //  case,  funnction address, starting Address, total bits.
          //  new List<string>() { "0", "25", "0014", "0003" } ,
            new List<string>() { "0", "03", "4751", "0001" } ,
            new List<string>() { "1", "03", "471A", "0006" } ,
            new List<string>() { "2", "03", "4700", "0004" } ,
            new List<string>() { "3", "03", "4728", "0008" } ,
            new List<string>() { "4", "03", "4723", "0006" } ,
            new List<string>() { "5", "03", "4720", "0008" } ,
            new List<string>() { "6", "03", "4710", "0008" } ,
            new List<string>() { "7", "03", "4718", "0008" } ,
            new List<string>() { "8", "03", "4708", "0008" } ,
            new List<string>() { "9", "03", "4701", "0007" } ,
        };
        #endregion

        private void InstantiateThread()
        {
            try
            {
                //Bhushanthread
                ThreadPool.SetMinThreads(70, 70);
                // creates new thread
                for (int i = 0; i <= 70; i++)
                {
                    //  if (mainThread == null)
                    {
                        _AREvt = new AutoResetEvent(false);
                        mainThread = new Thread(ExecuteThread);
                        mainThread.IsBackground = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool UserSelectedConnect(string item)
        {
            try
            {
                UserControl1 ctrl1 = usrList.Find(x => x.SelectedNode == true);
                ctrl1.SelectedNode = false;

                //find node with nodeId from node list
                UserControl1 ctrl = usrList.Find(x => x.NodeId == item);
                ctrl.SelectedNode = true;

                selectedNode = ctrl.NodeAddress; // for rampSoak
                //settingsToolStripMenuItem_Click(null, null);
                rampSoakProgramToolStripMenuItem.Enabled = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void BindHMI(UserControl1 ctrl)
        {
            try
            {
                if (Writedatab != true) 
                {
                    lblNodeId.Invoke((Action)(() => lblNodeId.Text = ctrl.NodeAddress.ToString()));

                    pvdat = string.IsNullOrEmpty(ctrl.PV) ? ctrl.PV : ctrl.PV;
                    //if (pvdat != "0.0") // || pvdat != "0.0"
                    {
                        if (pvdat == "Input Error")
                        {
                            txtPV4.Invoke((Action)(() => txtPV4.Font = new Font("Microsoft Sans Serif", 18, FontStyle.Bold)));
                            txtPV4.Invoke((Action)(() => txtPV4.ForeColor = Color.OrangeRed));
                            txtPV4.Invoke((Action)(() => txtPV4.Text = pvdat));
                        }

                        if (pvdat == "No Connect")
                        {
                            txtPV4.Invoke((Action)(() => txtPV4.Font = new Font("Microsoft Sans Serif", 18, FontStyle.Bold)));
                            txtPV4.Invoke((Action)(() => txtPV4.ForeColor = Color.OrangeRed));
                            txtPV4.Invoke((Action)(() => txtPV4.Text = pvdat));
                        }
                        else
                        {
                            txtPV4.Invoke((Action)(() => txtPV4.Font = new Font("Microsoft Sans Serif", 32, FontStyle.Bold)));
                            txtPV4.Invoke((Action)(() => txtPV4.ForeColor = Color.OrangeRed));
                            txtPV4.Invoke((Action)(() => txtPV4.Text = pvdat));
                        }
                    }
                }

                txtSV4.Invoke((Action)(() => txtSV4.Text = string.IsNullOrEmpty(ctrl.SV) ? "0" : ctrl.SV));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool UserRemoveConnect(string item)
        {
            try
            {
                if (usrDictionary != null)
                {
                    //find node with nodeId from node list
                    UserControl1 ctrl = usrList.Find(x => x.NodeId == item);

                    int itemValue1 = 0;
                    int itemValue2 = 0;

                    //// ** check if dictionary contains the nodeaddress, get its count in  out variable
                    if (usrDictionary.TryGetValue(ctrl.NodeAddress.ToString(), out itemValue1))
                    {
                        //// ** if the node to disconnect is selected node
                        if (ctrl.SelectedNode)
                        {
                            //// if its selected, then check if other nodes are present, if Y then connect other else close port

                            //// ** reduce its count by 1
                            usrDictionary[ctrl.NodeAddress.ToString()]--;

                            //// set its parameters to false
                            ctrl.Connected = false;
                            ctrl.SelectedNode = false;
                            //try
                            //{
                            //    eXITToolStripMenuItem.Enabled = true;
                            //}
                            //catch (Exception ae)
                            //{ }

                            //// if selection list contains the node, remove from selection list

                            int a = Convert.ToInt32(ctrl.NodeId);
                            if (a == 1)
                            {
                                try
                                {

                                    device1ToolStripMenuItem.Text = null;
                                    device1ToolStripMenuItem.Text = "Device 1";
                                }
                                catch (Exception ae) { }
                            }
                            if (a == 2)
                            {
                                try
                                {

                                    device2ToolStripMenuItem.Text = null;
                                    device2ToolStripMenuItem.Text = "Device 2";
                                }
                                catch (Exception ae) { }
                            }
                            if (a == 3)
                            {
                                try
                                {

                                    device3ToolStripMenuItem.Text = null;
                                    device3ToolStripMenuItem.Text = "Device 3";
                                }
                                catch (Exception ae) { }
                            }
                            if (a == 4)
                            {
                                try
                                {

                                    device4ToolStripMenuItem.Text = null;
                                    device4ToolStripMenuItem.Text = "Device 4";
                                }
                                catch (Exception ae) { }
                            }

                            if (selectedNodeList.Contains(ctrl.NodeId))
                            {
                                selectedNodeList.Remove(ctrl.NodeId);
                            }

                            //// if selection list has elements, then retians selected node status
                            if (selectedNodeList.Count > 0)
                            {
                                string node = Convert.ToString(selectedNodeList[0]);
                                usrList.Find(x => x.NodeId == node).SelectedNode = true;
                            }

                            //// get the nodeaddress count remained
                            usrDictionary.TryGetValue(ctrl.NodeAddress.ToString(), out itemValue2);

                            //// if dictionary has not element left for key the remove its key from dictionary
                            if (Convert.ToInt32(itemValue2) == 0)
                            {
                                usrDictionary.Remove(ctrl.NodeAddress.ToString());
                                //removed = usrDictionary.TryRemove(ctrl.NodeAddress.ToString(), out val);
                            }

                            //  OR
                            //// if dictionary has not element left for key the remove its key from dictionary
                            if (usrDictionary.ContainsKey(ctrl.NodeAddress.ToString()))
                            {
                                if (usrDictionary[ctrl.NodeAddress.ToString()] == 0)
                                {
                                    usrDictionary.Remove(ctrl.NodeAddress.ToString());
                                }
                            }

                            //// bind HMI default values
                            BindHMI(new UserControl1()
                            {
                                PV = "0",
                                SV = "0",
                                Unit = "K",
                                Out11 = 7,
                                Out12 = 7,
                                PatternStepBool = false,
                                RemainTimeBool = false
                            });

                            PictureBoxChange(pictureBox1, Color.Red);
                            PictureBoxChange(pictureBox3, Color.Red);
                            PictureBoxChange(pictureBox2, Color.Red);
                            PictureBoxChange(pictureBox4, Color.Red);
                            PictureBoxChange(pictureBox5, Color.Red);
                            PictureBoxChange(pictureBox6, Color.Red);
                            PictureBoxChange(pictureBox7, Color.Red);
                            PictureBoxChange(pictureBox8, Color.Red);
                            //   PictureBoxChange(pictureBox9, Color.LightGray);
                        }
                        else //if its not selected then remove it directly
                        {
                            if (itemValue1 == 0)
                            {
                                usrDictionary.Remove(ctrl.NodeAddress.ToString());
                                //removed = usrDictionary.TryRemove(ctrl.NodeAddress.ToString(), out val);
                            }

                            usrDictionary[ctrl.NodeAddress.ToString()]--;

                            ctrl.Connected = false;
                            ctrl.SelectedNode = false;

                            //remove from selection list
                            if (selectedNodeList.Contains(ctrl.NodeId))
                            {
                                selectedNodeList.Remove(ctrl.NodeId);
                                int a = Convert.ToInt32(ctrl.NodeId);
                                if (a == 1)
                                {
                                    try
                                    {

                                        device1ToolStripMenuItem.Text = null;
                                        device1ToolStripMenuItem.Text = "Device 1";
                                    }
                                    catch (Exception ae) { }
                                }
                                if (a == 2)
                                {
                                    try
                                    {

                                        device2ToolStripMenuItem.Text = null;
                                        device2ToolStripMenuItem.Text = "Device 2";
                                    }
                                    catch (Exception ae) { }
                                }
                                if (a == 3)
                                {
                                    try
                                    {

                                        device3ToolStripMenuItem.Text = null;
                                        device3ToolStripMenuItem.Text = "Device 3";
                                    }
                                    catch (Exception ae) { }
                                }
                                if (a == 4)
                                {
                                    try
                                    {

                                        device4ToolStripMenuItem.Text = null;
                                        device4ToolStripMenuItem.Text = "Device 4";
                                    }
                                    catch (Exception ae) { }
                                }
                            }

                            if (usrDictionary.ContainsKey(ctrl.NodeAddress.ToString()))
                            {
                                if (usrDictionary[ctrl.NodeAddress.ToString()] == 0)
                                {
                                    usrDictionary.Remove(ctrl.NodeAddress.ToString());
                                }
                            }
                        }
                        string nodel = ctrl.NodeAddress.ToString();
                        //if (usrDictionary[nodel])
                        //{ }
                        if (usrDictionary.Count == 0)
                        {

                            menuStrip1.Invoke((Action)(() =>
                            {
                                //rampSoakProgramToolStripMenuItem.Enabled = false;
                                vIEWToolStripMenuItem.Enabled = false;
                                pARAMETERSToolStripMenuItem.Enabled = false;
                                fILEToolStripMenuItem.Enabled = false;
                                grpSettings.Visible = settingsToolStripMenuItem.Checked = false;
                            }));

                            _isRunning = false;

                            //  removeChart("Series" + ctrl.NodeAddress.ToString());
                            usrDictionary = null;
                            //usrDictionary = new ConcurrentDictionary<string, int>();
                            usrDictionary = new Dictionary<string, int>();
                            BindHMI(new UserControl1()
                            {
                                PV = "0",
                                SV = "0",
                                Unit = "K",
                                Out11 = 7,
                                Out12 = 7,
                                PatternStepBool = false,
                                RemainTimeBool = false
                            });

                            if (modbusobj != null)
                            {
                                modbusobj.CloseSerialPort();
                            }

                            mainThread.Abort();
                            mainThread = null;
                        }

                        if (selectedNodeList.Count == 0)
                        {
                            grpSettings.Visible = false;
                            grpPvRecords.Visible = false;
                        }
                        return true;
                    }

                }
                return false;
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool UserRemoveConnectMoniter(string item)
        {
            try
            {
                if (usrDictionary != null)
                {
                    //find node with nodeId from node list
                    UserControl1 ctrl = usrList.Find(x => x.NodeId == item);

                    int itemValue1 = 0;
                    int itemValue2 = 0;

                    //// ** check if dictionary contains the nodeaddress, get its count in  out variable
                    // if (usrDictionary.TryGetValue(ctrl.NodeAddress.ToString(), out itemValue1))
                    {
                        //// ** if the node to disconnect is selected node
                        //if (ctrl.SelectedNode)
                        {
                            //// if its selected, then check if other nodes are present, if Y then connect other else close port

                            //// ** reduce its count by 1
                            usrDictionary[item.ToString()]--;

                            //// set its parameters to false
                            ctrl.Connected = false;
                            ctrl.SelectedNode = false;
                            //try
                            //{
                            //    eXITToolStripMenuItem.Enabled = true;
                            //}
                            //catch (Exception ae)
                            //{ }

                            //// if selection list contains the node, remove from selection list



                            //// if selection list has elements, then retians selected node status


                            //// get the nodeaddress count remained
                            usrDictionary.TryGetValue(item.ToString(), out itemValue2);

                            //// if dictionary has not element left for key the remove its key from dictionary
                            if (Convert.ToInt32(itemValue2) == 0)
                            {
                                usrDictionary.Remove(item.ToString());
                                //removed = usrDictionary.TryRemove(ctrl.NodeAddress.ToString(), out val);
                            }

                            //  OR
                            //// if dictionary has not element left for key the remove its key from dictionary
                            if (usrDictionary.ContainsKey(item.ToString()))
                            {
                                if (usrDictionary[item.ToString()] == 0)
                                {
                                    usrDictionary.Remove(item.ToString());
                                }
                            }

                            //// bind HMI default values
                            BindHMI(new UserControl1()
                            {
                                PV = "0",
                                SV = "0",
                                Unit = "K",
                                Out11 = 7,
                                Out12 = 7,
                                PatternStepBool = false,
                                RemainTimeBool = false
                            });

                            PictureBoxChange(pictureBox1, Color.Red);
                            PictureBoxChange(pictureBox3, Color.Red);
                            PictureBoxChange(pictureBox2, Color.Red);
                            PictureBoxChange(pictureBox4, Color.Red);
                            PictureBoxChange(pictureBox5, Color.Red);
                            PictureBoxChange(pictureBox6, Color.Red);
                            PictureBoxChange(pictureBox7, Color.Red);
                            PictureBoxChange(pictureBox8, Color.Red);
                            //   PictureBoxChange(pictureBox9, Color.LightGray);
                        }
                        try
                        {
                            device1ToolStripMenuItem.Text = null;
                            device1ToolStripMenuItem.Text = "Device 1";
                            device2ToolStripMenuItem.Text = null;
                            device2ToolStripMenuItem.Text = "Device 2";
                            device3ToolStripMenuItem.Text = null;
                            device3ToolStripMenuItem.Text = "Device 3";
                            device4ToolStripMenuItem.Text = null;
                            device4ToolStripMenuItem.Text = "Device 4";
                            //  pictureBox3.BeginInvoke((Action)(() => PictureBoxChange(pictureBox3, Color.Gray)));
                            panel1.BeginInvoke((Action)(() => panel1.BackColor = Color.DarkGray));
                        }
                        catch (Exception ae)
                        { }

                        if (usrDictionary.Count == 0)
                        {

                            menuStrip1.Invoke((Action)(() =>
                            {
                                //rampSoakProgramToolStripMenuItem.Enabled = false;
                                vIEWToolStripMenuItem.Enabled = false;
                                pARAMETERSToolStripMenuItem.Enabled = false;
                                fILEToolStripMenuItem.Enabled = false;
                                grpSettings.Visible = settingsToolStripMenuItem.Checked = false;
                            }));

                            _isRunning = false;

                            // removeChart("Series" + item.ToString());
                            usrDictionary = null;
                            //usrDictionary = new ConcurrentDictionary<string, int>();
                            usrDictionary = new Dictionary<string, int>();
                            BindHMI(new UserControl1()
                            {
                                PV = "0",
                                SV = "0",
                                Unit = "K",
                                Out11 = 7,
                                Out12 = 7,
                                PatternStepBool = false,
                                RemainTimeBool = false
                            });

                            if (modbusobj != null)
                            {
                                modbusobj.CloseSerialPort();
                            }

                            mainThread.Abort();
                            mainThread = null;
                        }

                        //if (selectedNodeList.Count == 0)
                        {
                            grpSettings.Visible = false;
                            grpPvRecords.Visible = false;
                            UserControl1 uc = new UserControl1();
                            ctrl.Invoke((Action)(() => ctrl.panelcol = 1));
                            // ctrl.Invoke((Action)(() => ctrl.ButtonText = "Connect"));

                        }
                        return true;
                    }

                }
                return false;
            }
            catch (ThreadAbortException)
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UserAddConnect(string item)
        {
            bool valid = false;

            // 1) check if item is null or empty
            // 2) check if usrDictionary is empty or not i.e if thread is already running
            // 3)   Y: pause thread 
            // 4)      check if item's nodeAddress is valid i.e r/w 1 query
            // 5)       Y: add item to ursDictionary and resume thread
            // 6)       N: resume thread
            // 7)   N: pause thread 
            // 8)      check if item's nodeAddress is valid i.e r/w 1 query
            // 9)        Y: update item's count in ursDictionary and resume thread
            // 10)       N: resume thread

            if (!string.IsNullOrEmpty(item))
            {
                try
                {
                    bool added = false;

                    if (usrDictionary != null)
                    {
                        //find node with nodeId from node list
                        UserControl1 ctrl = usrList.Find(x => x.NodeId == item);
                        ctrl.Connecting = true;

                        bool checkNoded = this.CheckNodeAvailability(ctrl.NodeAddress.ToString());
                        if (checkNoded)
                        {
                            int a = Convert.ToInt32(ctrl.NodeId);
                            if (a == 1)
                            {
                                try
                                {
                                    // device1ToolStripMenuItem.Text;
                                    string input = device1ToolStripMenuItem.Text;
                                    if (input.Length < 16)
                                    {
                                        device1ToolStripMenuItem.Text = input + " (Active)";

                                    }


                                }
                                catch (Exception ae) { }
                            }
                            if (a == 2)
                            {
                                try
                                {
                                    string input = device2ToolStripMenuItem.Text;
                                    if (input.Length < 16)
                                    {
                                        device2ToolStripMenuItem.Text = input + " (Active)";

                                    }

                                }
                                catch (Exception ae) { }
                            }
                            if (a == 3)
                            {
                                try
                                {
                                    string input = device3ToolStripMenuItem.Text;
                                    if (input.Length < 16)
                                    {
                                        device3ToolStripMenuItem.Text = input + " (Active)";
                                    }

                                }
                                catch (Exception ae) { }
                            }
                            if (a == 4)
                            {
                                try
                                {
                                    string input = device4ToolStripMenuItem.Text;
                                    if (input.Length < 16)
                                    {
                                        device4ToolStripMenuItem.Text = input + " (Active)";

                                    }

                                }
                                catch (Exception ae) { }
                            }
                        }
                        if (usrDictionary.Count > 0)
                        {
                            online = false; // pause online for a moment
                            _loopBreak = true;

                            bool checkNode = this.CheckNodeAvailability(ctrl.NodeAddress.ToString());

                            if (checkNode)
                            {
                                if (usrDictionary.ContainsKey(ctrl.NodeAddress.ToString()))
                                {
                                    usrDictionary[ctrl.NodeAddress.ToString()]++;
                                    added = true;
                                }
                                else
                                {
                                    usrDictionary.Add(ctrl.NodeAddress.ToString(), 1);
                                    //added = usrDictionary.TryAdd(ctrl.NodeAddress.ToString(), 1);
                                    added = true;
                                    if (added)
                                    {
                                        //createChart("Series" + ctrl.NodeAddress.ToString());
                                    }
                                }

                                if (added)
                                {
                                    //KA 0918
                                    if (selectedNodeList.Count > 0)
                                    {
                                        ctrl.SelectedNode = false;
                                        usrList.Find(x => x.NodeId == item).SelectedNode = false;
                                    }
                                    else
                                    {
                                        ctrl.SelectedNode = true;
                                        usrList.Find(x => x.NodeId == item).SelectedNode = true;
                                    }

                                    usrList.Find(x => x.NodeId == item).Connecting = true;
                                    usrList.Find(x => x.NodeId == item).Connected = true; // KA 0918
                                    usrList.Find(x => x.NodeId == item).PvRecords = null;
                                    usrList.Find(x => x.NodeId == item).PvRecords = new Dictionary<double, double>();

                                    // add nodeId to selection list
                                    selectedNodeList.Add(ctrl.NodeId);

                                    _loopBreak = false;
                                    online = true; // resume online 

                                    valid = true;
                                }
                                else { }
                            }
                            else
                            {
                                ctrl.Connecting = false;
                            }
                        }
                        if (usrDictionary.Count == 0)
                        {
                            //first entry
                            bool checkNode = this.CheckNodeAvailability(ctrl.NodeAddress.ToString());

                            if (checkNode)
                            {
                                if (usrDictionary.ContainsKey(ctrl.NodeAddress.ToString()))
                                {
                                    usrDictionary[ctrl.NodeAddress.ToString()]++;
                                    added = true;
                                }
                                else
                                {
                                    usrDictionary.Add(ctrl.NodeAddress.ToString(), 1);
                                    //added = usrDictionary.TryAdd(ctrl.NodeAddress.ToString(), 1);
                                    added = true;
                                    if (added)
                                    {
                                        //   createChart("Series" + ctrl.NodeAddress.ToString());
                                    }
                                }

                                if (added)
                                {
                                    //ctrl.SelectedNode = true;
                                    usrList.Find(x => x.NodeId == item).Connecting = true;
                                    usrList.Find(x => x.NodeId == item).Connected = true;
                                    usrList.Find(x => x.NodeId == item).SelectedNode = true;
                                    usrList.Find(x => x.NodeId == item).PvRecords = null;
                                    usrList.Find(x => x.NodeId == item).PvRecords = new Dictionary<double, double>();

                                    // add nodeId to selection list
                                    if (selectedNodeList != null)
                                    {
                                        if (!selectedNodeList.Contains(ctrl.NodeId)) // KA 1015
                                        {
                                            selectedNodeList.Add(ctrl.NodeId);
                                        }
                                    }

                                    selectedNode = ctrl.NodeAddress; // for rampSoak


                                    if (mainThread == null)
                                    {
                                        InstantiateThread();
                                    }



                                    if (mainThread != null)
                                    {
                                        _AREvt = new AutoResetEvent(false);
                                        _isRunning = true;
                                        try
                                        {
                                            mainThread.Priority = ThreadPriority.Lowest;
                                        }
                                        catch {
                                            if (!mainThread.IsAlive)
                                            {
                                                _AREvt = new AutoResetEvent(false);
                                                mainThread = null;
                                                InstantiateThread();
                                                mainThread.Priority = ThreadPriority.Lowest;
                                              //  mainThread.Start();
                                            }
                                        }
                                        mainThread.Start();

                                    }
                                    _loopBreak = false;
                                    online = true; // start online 
                                    openToolStripMenuItem.Enabled = true;
                                    fILEToolStripMenuItem.Enabled = true;
                                    pARAMETERSToolStripMenuItem.Enabled = true;
                                    vIEWToolStripMenuItem.Enabled = true;
                                    grpSettings.Visible = settingsToolStripMenuItem.Checked = true;

                                    valid = true;
                                }
                            }
                            else
                            {
                                ctrl.Connecting = false;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                    return false;
                }
            }

            return valid;
        }

        private bool CheckNodeAvailability(string nodeAddress)
        {

            byte[] response = new byte[25];
            bool reply = false;

            string portName = SetValues.Set_PortName;
            string baudRate = SetValues.Set_Baudrate;
            string parity = SetValues.Set_parity;
            int bitsLength = SetValues.Set_BitsLength;
            string stopBits = SetValues.Set_StopBits;

            if (modbusobj.IsSerialPortOpen()) { }
            else
            {
                if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength))
                { }
            }
            //Convert.ToInt32(nodeAddress).ToString("X").PadLeft(2, '0')
            for (int i = 0; i < 3; i++)
            {
               // Thread.Sleep(1500);
                if ((SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1)&& SetValues.Set_CommType==1)
                {
                    response = modbusobj.AscFrame(nodeAddress, "03", "4701", "0001");
                }
                else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                {
                    response = modbusobj.RtuFrame(nodeAddress, "03", "4701", "0001", baudRate);
                }

                if (response == null)
                {
                    reply = false;

                }
                else
                {
                    reply = true;
                    break;
                }
            }
            return reply;
        }



        public void LED4728(UserControl1 ctrl)
        {
            try
            {
                regaddd = "4728";
                oATT4728 = SendFrameToDevice1(key, regaddd);
                myStruct.LEDdata = oATT4728;
                led = hex2binary(oATT4728);
                bb = 8;
                c = bb - led.Length;



                for (int k = 0; k < c; k++)
                {
                    led = "0" + led;
                }


                //LogWriter.WriteToFile("+4728+" + oATT4728, "", "MonitorOnline_ErrorLog");
                charArratLED = led.ToCharArray();
                Array.Reverse(charArratLED = led.ToCharArray());
                // Array.Reverse(charArratLED);

                {
                    //  BindLedStatus(charArrat);
                    for (int z = 1; z <= led.Length - 1; z++)
                    {

                        pics[z].Invoke((Action)(() =>
                        {

                            if (charArratLED[z] == '1')
                            {
                                PictureBoxChange(pics[z], Color.LimeGreen);
                                if (charArratLED[4] == '1')
                                {
                                    ctrl.Invoke((Action)(() => ctrl.Alarm1data = true));
                                }
                                else
                                {
                                    ctrl.Invoke((Action)(() => ctrl.Alarm1data = false));
                                }
                                if (charArratLED[1] == '1')
                                {
                                    ctrl.Invoke((Action)(() => ctrl.Alarm2data = true));
                                }
                                else
                                {
                                    ctrl.Invoke((Action)(() => ctrl.Alarm2data = false));
                                }
                                // LogWriter.WriteToFile("[" + z + "]" + charArratLED[z].ToString(), "", "MonitorOnline_ErrorLog");
                            }
                            else
                            {
                                if (z != 7)
                                {
                                    PictureBoxChange(pics[z], Color.Red);
                                    if (charArratLED[4] == '1')
                                    {
                                        ctrl.Invoke((Action)(() => ctrl.Alarm1data = true));
                                    }
                                    else
                                    {
                                        ctrl.Invoke((Action)(() => ctrl.Alarm1data = false));
                                    }
                                    if (charArratLED[1] == '1')
                                    {
                                        ctrl.Invoke((Action)(() => ctrl.Alarm2data = true));
                                    }
                                    else
                                    {
                                        ctrl.Invoke((Action)(() => ctrl.Alarm2data = false));
                                    }
                                    //LogWriter.WriteToFile("[" + z + "]" + charArratLED[z].ToString(), "", "MonitorOnline_ErrorLog");
                                    //  PictureBoxChange(pics[7], Color.Red);
                                    
                                }
                            }



                            //pics[i].BackColor =  ? Color.LimeGreen : Color.Red;
                        }));
                    }



                }
            }
            catch (Exception ae)
            {
 
            }
        }
        public void LED470A()
        {
            try
            {
                regaddd = "470A";
                oATT4728 = SendFrameToDevice1(key, regaddd);
                myStruct.LEDdata = oATT4728;
                ledd470A = hex2binary(oATT4728);
                bb = 8;
                c = bb - ledd470A.Length;

                //  int bitsdifference = val[0].Length * 4 - binaryval.Length;
                for (int k = 0; k < c; k++)
                {
                    ledd470A = "0" + ledd470A;
                }

                charArrAT = ledd470A.ToCharArray();

                Array.Reverse(charArrAT);
                LogWriter.WriteToFile("+470A+" + oATT4728, "", "MonitorOnline_ErrorLog");
                {

                    // for (int z = 1; z < 7; z++)
                    {

                        pics[7].Invoke((Action)(() =>
                        {
                            if (charArrAT[6] == '1')
                            {
                                PictureBoxChange(pics[7], Color.LimeGreen);
                                // LogWriter.WriteToFile(charArrAT[6].ToString(), "", "MonitorOnline_ErrorLog");
                            }
                            else
                            {

                                PictureBoxChange(pics[7], Color.Red);
                                // PictureBoxChange(pics[7], Color.Red);
                                //  LogWriter.WriteToFile(charArrAT[6].ToString(), "", "MonitorOnline_ErrorLog");

                            }
                        }));


                        pics[8].Invoke((Action)(() =>
                        {

                            if (charArrAT[0] == '1')
                            {
                                PictureBoxChange(pics[8], Color.LimeGreen);
                            }
                            else
                            {
                                PictureBoxChange(pics[8], Color.Red);

                            }

                        }));

                    }
                }
            }
            catch (Exception ae)
            { 

            }
        }
        /// <summary>
        /// 1) loop through user control dictionary and then through frame list,
        /// 2) send each frame over network
        /// 3) check the frame counter and set the controls with its value in the switch case
        /// </summary>
        private void ExecuteThread()
        {
            LogWriter.WriteToFile("ExecuteThread:", "Thread started", "MonitorOnline_Log");
           countt = 1;
            try
            {
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                OLK = 0;
                if (mainThread != null)
                {
                    pdV = false;
                    if (usrDictionary.Count > 0)
                    {

                        while (_isRunning)
                        {
                            Application.DoEvents();
                            if (online)
                            {
                                pv = "0";
                                sv = "0";
                                out1 = 0;
                                out2 = 0;

                                for (int i = usrDictionary.Count; i > 0; i--)
                                {
                                    _AREvt.WaitOne(10, true);
                                    key = Convert.ToString(usrDictionary.ElementAt(i - 1).Key);
                                    value = Convert.ToString(usrDictionary.ElementAt(i - 1).Value);
                                    errorCount = new int[5];
                                    ArrayList newList = new ArrayList();
                                    List<UserControl1> lists = usrList.Where(x => x.Connected == true).ToList<UserControl1>();

                                    if (_loopBreak)
                                    {
                                        break;
                                    }

                                    if (lists != null)
                                    {
                                        if (lists.Count > 0)
                                        {
                                            foreach (UserControl1 ctrl in lists)
                                            {
                                                ErrFlag = false;
                                                if (ctrl.Connected == true)
                                                {

                                                    if (_loopBreak)
                                                    {
                                                        break;
                                                    }

                                                    //  if (selectedNode.ToString() == key)
                                                    if (ctrl.NodeAddress.ToString() == key)
                                                    {

                                                        if (!LeaveEvent)
                                                        {
                                                            for (int frameCount = 1; frameCount < frameList.Count; frameCount++)
                                                            {
                                                                //Password Function
                                                                try
                                                                {
                                                                    regadd = "106E";
                                                                    oout2 = SendFrameToDevice1(key, regadd);
                                                                    o22 = Convert.ToInt32(oout2);
                                                                    myStruct.lockStatus = oout2;
                                                                    if (selectedNode.ToString() == key)
                                                                    {
                                                                        CmbBxLockstatus.Invoke((Action)(() =>
                                                                        {
                                                                            if (o22 < CmbBxLockstatus.Items.Count) // o2
                                                                            {
                                                                                CmbBxLockstatus.SelectedIndex = o22;  //o2
                                                                            }


                                                                        }));
                                                                    }

                                                                }
                                                                catch (Exception ae) { }
                                                                List<string> getList = frameList[frameCount];
                                                                List<string> val = null;
                                                                try
                                                                {
                                                                    val = CreateFrames(key, getList[1], getList[2], getList[3], true);
                                                                }
                                                                catch (Exception ae)
                                                                {

                                                                }
                                                                if (val == null)
                                                                {

                                                                    {
                                                                        OLK++;
                                                                        if (OLK > 20) //220//150//100
                                                                        {
                                                                            if (ctrl.NodeAddress.ToString() == key)
                                                                            {

                                                                                MessageBox.Show(new Form() { TopMost = true }, "Device is disconnect!",
                                                                                            "Device Status",
                                                                                            MessageBoxButtons.OK,
                                                                                            MessageBoxIcon.Question);
                                                                                ctrl.Invoke((Action)(() =>
                                                                                {

                                                                                    UserRemoveConnectMoniter(ctrl.NodeAddress.ToString());

                                                                                    ctrl.ResetControlsMoniter();

                                                                                }));
                                                                            }
                                                                        }
                                                                    }

                                                                }
                                                                else if (val != null && val.Count > 0)
                                                                {
                                                                    if (ctrl.SelectedNode == true)
                                                                    {
                                                                        switch (frameCount)
                                                                        {
                                                                            #region Cases
                                                                            case 0:
                                                                                //    #region Case 0
                                                                                CmbBxfraction.Invoke((Action)(() =>
                                                                                {
                                                                                    CmbBxfraction.Enabled = true;
                                                                                    //  ispause = false;
                                                                                }));
                                                                                //    #endregion
                                                                                break;
                                                                            case 1:
                                                                                #region Case 1
                                                                                if (val.Count == 6)
                                                                                {

                                                                                    //471A
                                                                                    try
                                                                                    {

                                                                                        OLK = 0;
                                                                                        vall471A = Convert.ToInt32(ConvertHexToShort(val[0], true));
                                                                                        myStruct.out1 = val[0];
                                                                                        myStruct.out1cmb = val[0];

                                                                                        if (out1Val)
                                                                                        {
                                                                                            CmbBx1stout.Invoke((Action)(() =>
                                                                                            {
                                                                                                if (vall471A < CmbBx1stout.Items.Count)
                                                                                                {
                                                                                                    CmbBx1stout.SelectedIndex = vall471A;
                                                                                                }

                                                                                            }));
                                                                                        }
                                                                                        ctrl.Invoke((Action)(() => ctrl.Out11 = vall471A));



                                                                                        //catch (Exception ae) { }

                                                                                        //if (MoniterStatus)

                                                                                        OLK = 0;
                                                                                        //471B
                                                                                        if (ctrl1V)
                                                                                        {
                                                                                            //txtBxCtrlPeriod1.Invoke((Action)(() => txtBxCtrlPeriod1.Text =
                                                                                            //    (Convert.ToDouble(Convert.ToInt16(val[1], 16)) / DecimalPlaces()).ToString()));

                                                                                            txtBxCtrlPeriod1.Invoke((Action)(() => txtBxCtrlPeriod1.Text =
                                                                                                PlaceDecimal(ConvertHexToShort(val[1], true))));
                                                                                            // MessageBox.Show(txtBxCtrlPeriod1.Text);
                                                                                            txtBxCtrlPer1.Invoke((Action)(() => txtBxCtrlPer1.Text =
                                                                                               (ConvertHexToShortInt(val[1], true)).ToString()));
                                                                                        }
                                                                                        myStruct.ctrlPeriod1 = val[1];




                                                                                        // if (MoniterStatus)

                                                                                        OLK = 0;
                                                                                        //471C
                                                                                        if (ctrl2V)
                                                                                        {

                                                                                            //regadd = "471C";
                                                                                            //oAT = SendFrameToDevice1(key, regadd);

                                                                                            txtBxCtrlPeriod2.Invoke((Action)(() => txtBxCtrlPeriod2.Text =
                                                                                                PlaceDecimal(ConvertHexToShort(val[2], true))));

                                                                                            txtBxctrlPer2.Invoke((Action)(() => txtBxctrlPer2.Text =
                                                                                                (ConvertHexToShort(val[2], true)).ToString()));

                                                                                        }
                                                                                        myStruct.ctrlPeriod2 = val[1];

                                                                                        //471D
                                                                                        if (!PVV)
                                                                                        {
                                                                                            OLK = 0;
                                                                                            valOff = Convert.ToInt32(ConvertHexToShort(val[3], true));
                                                                                            myStruct.pvOffset = val[3];
                                                                                            CmbBx2ndout.Invoke((Action)(() =>
                                                                                            {
                                                                                                txtBxPVOffset.Text = valOff.ToString();
                                                                                                //  ispause = false;
                                                                                            }));

                                                                                        }
                                                                                        //471E
                                                                                        //471F

                                                                                        //  if (MoniterStatus)

                                                                                        OLK = 0;
                                                                                        //  int vall12 = Convert.ToInt32(ConvertHexToShort(val[5], true));
                                                                                        regadd = "471F";
                                                                                        oout2 = SendFrameToDevice1(key, regadd);
                                                                                        o2out = Convert.ToInt32(oout2);
                                                                                        myStruct.out2 = oout2;
                                                                                        myStruct.out2cmb = oout2;
                                                                                        if (out2Val)
                                                                                        {
                                                                                            CmbBx2ndout.Invoke((Action)(() =>
                                                                                            {
                                                                                                if (o2out < CmbBx2ndout.Items.Count) // o2
                                                                                                {
                                                                                                    CmbBx2ndout.SelectedIndex = o2out;  //o2
                                                                                                }

                                                                                            }));
                                                                                        }
                                                                                        ctrl.Invoke((Action)(() => ctrl.Out12 = o2out));  //o2

                                                                                        BindHMI(ctrl);

                                                                                    }

                                                                                    catch (Exception ae)
                                                                                    { }
                                                                                }

                                                                                #endregion
                                                                                break;
                                                                            case 2:
                                                                                #region Case 2

                                                                                if (val.Count == 4)
                                                                                {

                                                                                    try
                                                                                    {
                                                                                        if (val[0] == "8003")
                                                                                        {
                                                                                            OLK = 0;
                                                                                            pv = PlaceDecimal(ConvertHexToShort(val[0], false));
                                                                                            ctrl.Invoke((Action)(() => ctrl.PV = "No Connect"));
                                                                                            myStruct.pv = val[0];

                                                                                        }
                                                                                        else if (val[0] == "8004")
                                                                                        {
                                                                                            OLK = 0;
                                                                                            pv = PlaceDecimal(ConvertHexToShort(val[0], false));
                                                                                            ctrl.Invoke((Action)(() => ctrl.PV = "Input Error"));
                                                                                            myStruct.pv = val[0];
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            OLK = 0;
                                                                                            pv = PlaceDecimal(ConvertHexToShort(val[0], false));
                                                                                            ctrl.Invoke((Action)(() => ctrl.PV = pv));
                                                                                            myStruct.pv = val[0];
                                                                                        }
                                                                                        //4701

                                                                                        OLK = 0;
                                                                                        sv = PlaceDecimal(ConvertHexToShort(val[1], false));
                                                                                        ctrl.Invoke((Action)(() => ctrl.SV = sv.ToString(CultureInfo.InvariantCulture)));
                                                                                        myStruct.sv = val[1];
                                                                                        if (!onchangeeSetvalue)
                                                                                        {
                                                                                            txtBxSetvalue.Invoke((Action)(() => txtBxSetvalue.Text = sv));

                                                                                        }

                                                                                        BindHMI(ctrl);

                                                                                        OLK = 0;
                                                                                        //4702
                                                                                        out1 = Convert.ToDecimal(PlaceDecimal(ConvertHexToShort(val[2], false)));
                                                                                        ctrl.Invoke((Action)(() => ctrl.Out1Percent = out1));
                                                                                        myStruct.out1 = val[2];
                                                                                        if (!out1V)
                                                                                        {
                                                                                            txtBxOut1.Invoke((Action)(() => txtBxOut1.Text = Convert.ToString(out1, CultureInfo.InvariantCulture)));
                                                                                        }

                                                                                        OLK = 0;
                                                                                        //4703
                                                                                        out2 = Convert.ToDecimal(PlaceDecimal(ConvertHexToShort(val[3], false)));
                                                                                        ctrl.Invoke((Action)(() => ctrl.Out2Percent = out2));
                                                                                        myStruct.out2 = val[3];
                                                                                        if (!out2V)
                                                                                        {
                                                                                            txtBxOut2.Invoke((Action)(() => txtBxOut2.Text = Convert.ToString(out2, CultureInfo.InvariantCulture)));
                                                                                        }


                                                                                    }
                                                                                    catch (Exception ae)
                                                                                    { }


                                                                                }
                                                                                #endregion
                                                                                break;
                                                                            case 3:
                                                                                try
                                                                                {
                                                                                    if (Writedatab != true)
                                                                                    {
                                                                                        LED4728(ctrl);

                                                                                        LED470A();
                                                                                      
                                                                                    }
                                                                                    BindHMI(ctrl);
                                                                                }
                                                                                catch (Exception ae) { }

                                                                                #region Case 3
                                                                                if (val.Count == 8)
                                                                                {
                                                                                    //4728
                                                                                    try
                                                                                    {
                                                                                        OLK = 0;
                                                                                        if (Writedatab != true)
                                                                                        {
                                                                                            led = hex2binary(val[0]);
                                                                                            myStruct.LEDdata = val[0];
                                                                                            bb = 8;
                                                                                            c = bb - led.Length;


                                                                                            // int bitsdifference = val[0].Length * 4 - binaryval.Length;
                                                                                            for (int k = 0; k < c; k++)
                                                                                            {
                                                                                                led = "0" + led;
                                                                                            }

                                                                                            Array.Reverse(charArratLED = led.ToCharArray());
                                                                                            LogWriter.WriteToFile("4728 (1)" + val[0], "", "MonitorOnline_ErrorLog");
                                                                                            // Array.Reverse(charArratLED);

                                                                                            {
                                                                                                // BindLedStatus(charArrat);
                                                                                                LogWriter.WriteToFile("++", "", "MonitorOnline_ErrorLog");


                                                                                                for (int z = 1; z <= led.Length - 1; z++)
                                                                                                {

                                                                                                    pics[z].Invoke((Action)(() =>
                                                                                                    {

                                                                                                        if (charArratLED[z] == '1')
                                                                                                        {
                                                                                                            PictureBoxChange(pics[z], Color.LimeGreen);
                                                                                                            if (charArratLED[4] == '1')
                                                                                                            {
                                                                                                                ctrl.Invoke((Action)(() => ctrl.Alarm1data = true));
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                ctrl.Invoke((Action)(() => ctrl.Alarm1data = false));
                                                                                                            }
                                                                                                            if (charArratLED[1] == '1')
                                                                                                            {
                                                                                                                ctrl.Invoke((Action)(() => ctrl.Alarm2data = true));
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                ctrl.Invoke((Action)(() => ctrl.Alarm2data = false));
                                                                                                            }
                                                                                                            LogWriter.WriteToFile("[" + z + "]" + charArratLED[z].ToString(), "", "MonitorOnline_ErrorLog");
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (z != 7)
                                                                                                            {
                                                                                                                PictureBoxChange(pics[z], Color.Red);
                                                                                                                LogWriter.WriteToFile("[" + z + "]" + charArratLED[z].ToString(), "", "MonitorOnline_ErrorLog");
                                                                                                          
                                                                                                                //  PictureBoxChange(pics[7], Color.Red);
                                                                                                                if (charArratLED[4] == '1')
                                                                                                                {
                                                                                                                    ctrl.Invoke((Action)(() => ctrl.Alarm1data = true));
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    ctrl.Invoke((Action)(() => ctrl.Alarm1data = false));
                                                                                                                }
                                                                                                                if (charArratLED[1] == '1')
                                                                                                                {
                                                                                                                    ctrl.Invoke((Action)(() => ctrl.Alarm2data = true));
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    ctrl.Invoke((Action)(() => ctrl.Alarm2data = false));
                                                                                                                }
                                                                                                            }
                                                                                                        }



                                                                                                        //pics[i].BackColor =  ? Color.LimeGreen : Color.Red;
                                                                                                    }));
                                                                                                }
                                                                                            }
                                                                                        }

                                                                                            OLK = 0;
                                                                                            //472A 
                                                                                            string hex = System.Convert.ToInt16(val[2].Substring(0,2),16).ToString();
                                                                                            string hex2 = System.Convert.ToInt16(val[2].Substring(2,2),16).ToString(); 
                                                                                            string ver2 = hex+"."+hex2;
                                                                                            //ver = string.IsNullOrEmpty(val[2]) ? 0.0 : Convert.ToDouble(val[2]) / 100;
                                                                                            lblSwVersion.Invoke((Action)(() => lblSwVersion.Text = String.Format("{0:0.00}", ver2.ToString(CultureInfo.InvariantCulture))));

                                                                                            myStruct.swVersion = val[2];

                                                                                            //472D
                                                                                            step = Convert.ToInt32(ConvertHexToShort(val[5], true));
                                                                                            ctrl.Invoke((Action)(() =>
                                                                                                ctrl.PatternStepText = step.ToString()
                                                                                            ));

                                                                                          //  if (Writedatab != true)
                                                                                            {
                                                                                                BindHMI(ctrl);  //test
                                                                                            }
                                                                                    }
                                                                                    catch (Exception ae)
                                                                                    {
                                                                                    }
                                                                                }
                                                                                #endregion

                                                                                break;
                                                                            case 4:

                                                                                #region Case 4
                                                                                if (val.Count == 6)
                                                                                {

                                                                                    //4723

                                                                                    try
                                                                                    {
                                                                                        if (!LeaveEvent)
                                                                                        {
                                                                                            regadd = "4700";
                                                                                            oAT4700 = SendFrameToDevice1(key, regadd);
                                                                                            if (oAT4700 == "")
                                                                                            {
                                                                                                while (oAT4700 == "")
                                                                                                {
                                                                                                    oAT4700 = SendFrameToDevice1(key, regadd);
                                                                                                }
                                                                                            }
                                                                                            if (oAT4700 == "8003")
                                                                                            {
                                                                                                OLK = 0;
                                                                                                pv = PlaceDecimal(ConvertHexToShort(oAT4700, false));
                                                                                                ctrl.Invoke((Action)(() => ctrl.PV = "No Connect"));
                                                                                                myStruct.pv = oAT4700;

                                                                                            }
                                                                                            else if (oAT4700 == "8004")
                                                                                            {
                                                                                                OLK = 0;
                                                                                                pv = PlaceDecimal(ConvertHexToShort(oAT4700, false));
                                                                                                ctrl.Invoke((Action)(() => ctrl.PV = "Input Error"));
                                                                                                myStruct.pv = oAT4700;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                OLK = 0;
                                                                                                pv = PlaceDecimal(ConvertHexToShort(oAT4700, false));
                                                                                                ctrl.Invoke((Action)(() => ctrl.PV = pv));
                                                                                                myStruct.pv = oAT4700;
                                                                                            }
                                                                                        }
                                                                                        //4701
                                                                                       // if (Writedatab != true)
                                                                                        {
                                                                                            BindHMI(ctrl);
                                                                                        }
                                                                                        OLK = 0;
                                                                                        //4724
                                                                                        val2CmbBxUnitType = Convert.ToInt32(ConvertHexToShort(val[1], true));
                                                                                        myStruct.unit = val[1];
                                                                                        if (unitVal)
                                                                                        {
                                                                                            string type = "";
                                                                                            if (val2CmbBxUnitType < CmbBxUnitType.Items.Count)
                                                                                            {
                                                                                                CmbBxUnitType.Invoke((Action)(() => CmbBxUnitType.SelectedIndex = val2CmbBxUnitType));
                                                                                                type = BindUnitType(val2CmbBxUnitType);

                                                                                                ctrl.Invoke((Action)(() => ctrl.Unit = type));
                                                                                            }
                                                                                        }



                                                                                        //4725


                                                                                        //  if (MoniterStatus)
                                                                                        {
                                                                                            //4726
                                                                                            regaddd = "4726";
                                                                                            oAT = SendFrameToDevice1(key, regaddd);
                                                                                            myStruct.RemTime = oAT;
                                                                                            remTime = Convert.ToInt32(ConvertHexToShort(oAT, true));
                                                                                            // int remTime = Convert.ToInt32(ConvertHexToShort(val[3], true));

                                                                                            //int remTime = Convert.ToInt32(led);
                                                                                            str = "00:00:00";

                                                                                            if (remTime > 0)
                                                                                            {
                                                                                                hours = remTime / 3600;
                                                                                                minutes = remTime / 60 % 60;
                                                                                                seconds = remTime % 60;
                                                                                                str = hours + ":" + minutes + ":" + seconds;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                str = "00:00:00"; ;
                                                                                            }


                                                                                            ctrl.Invoke((Action)(() =>
                                                                                                ctrl.RemainTimeText = str
                                                                                            ));
                                                                                        }
                                                                                    }
                                                                                    catch (Exception ae)
                                                                                    {
                                                                                    }
                                                                                    //4727

                                                                                }

                                                                                #endregion
                                                                                break;
                                                                            case 5:
                                                                                #region Case 5
                                                                                try
                                                                                {
                                                                                    if (Writedatab != true)
                                                                                    {
                                                                                        LED4728(ctrl);

                                                                                        LED470A();
                                                                                    }
                                                                                }
                                                                                catch (Exception ae)
                                                                                { }
                                                                                if (val.Count == 8)
                                                                                {

                                                                                    try
                                                                                    {
                                                                                        // if (MoniterStatus)

                                                                                        OLK = 0;
                                                                                        a1cmbBxAlarm1Mode = Convert.ToInt32(ConvertHexToShort(val[0], true));
                                                                                        if (a1cmbBxAlarm1Mode < cmbBxAlarm1Mode.Items.Count)
                                                                                        {
                                                                                            //4720
                                                                                            if (alM1Val)
                                                                                            {
                                                                                                cmbBxAlarm1Mode.Invoke((Action)(() => cmbBxAlarm1Mode.SelectedIndex = a1cmbBxAlarm1Mode));
                                                                                            }
                                                                                            myStruct.alarm1Mode = val[0];
                                                                                        }





                                                                                        // if (MoniterStatus)

                                                                                        OLK = 0;
                                                                                        a2cmbBxAlarm2Mode = Convert.ToInt32(ConvertHexToShort(val[1], true));
                                                                                        if (a2cmbBxAlarm2Mode < cmbBxAlarm2Mode.Items.Count)
                                                                                        {
                                                                                            //4721
                                                                                            if (alM2Val)
                                                                                            {
                                                                                                cmbBxAlarm2Mode.Invoke((Action)(() => cmbBxAlarm2Mode.SelectedIndex = a2cmbBxAlarm2Mode));
                                                                                            }
                                                                                            myStruct.alarm2Mode = val[1];
                                                                                        }



                                                                                        //4722  -2


                                                                                        //  if (MoniterStatus)

                                                                                        OLK = 0;
                                                                                        //4723  -3
                                                                                        valIntCmbBxRunHaltmode = Convert.ToInt32(ConvertHexToShort(val[3], true));
                                                                                        if (valIntCmbBxRunHaltmode <=  CmbBxRunHaltmode.Items.Count)
                                                                                        {
                                                                                            if (runVal)
                                                                                            {
                                                                                                CmbBxRunHaltmode.Invoke((Action)(() => CmbBxRunHaltmode.SelectedIndex = (valIntCmbBxRunHaltmode == 4) ? 0 : valIntCmbBxRunHaltmode));
                                                                                            }
                                                                                            myStruct.run = val[3];
                                                                                            if(val[3]== "0004")
                                                                                            {
                                                                                                myStruct.run = "0000";
                                                                                            }
                                                                                        }
                                                                                      //  if (Writedatab != true)
                                                                                        {
                                                                                            BindHMI(ctrl);  //test
                                                                                        }
                                                                                    }
                                                                                    catch (Exception ae)
                                                                                    {
                                                                                    }
                                                                                    //4724  -4
                                                                                    //4725  -5
                                                                                    //4726  -6



                                                                                }
                                                                                #endregion
                                                                                break;
                                                                            case 6:
                                                                                #region Case 6
                                                                                if (val.Count == 8)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        if (Writedatab != true)
                                                                                        {
                                                                                            LED4728(ctrl);

                                                                                            LED470A();
                                                                                        }


                                                                                        //4710
                                                                                        if (!onchangePDoffset)
                                                                                        {
                                                                                            txtPDOffset.Invoke((Action)(() =>
                                                                                                       txtPDOffset.Text = PlaceDecimal(ConvertHexToShort(val[0], false))));
                                                                                        }
                                                                                        myStruct.pdoffset = val[0];

                                                                                        //4711
                                                                                        if (!onchangetxtBxPCoefficient)
                                                                                        {
                                                                                            coef = ConvertHexToShort(val[1], false) / 10;
                                                                                          
                                                                                            if (coefV)
                                                                                            {
                                                                                                txtBxPCoefficient.Invoke((Action)(() =>
                                                                                                    txtBxPCoefficient.Text =  PlaceDecimal(coef) + "0" ));
                                                                                                //testdata
                                                                                            }
                                                                                            myStruct.coef = val[1];
                                                                                        }

                                                                                        //  if (MoniterStatus)

                                                                                        if (!onchangetxtBxdeadband)
                                                                                        {
                                                                                            //4712
                                                                                            hy1 = ConvertHexToShort(val[2], false);
                                                                                            if (deadV)
                                                                                            {
                                                                                                txtHysteresisDeadBand.Invoke((Action)(() =>
                                                                                                    txtHysteresisDeadBand.Text = PlaceDecimal(ConvertHexToShort(val[2], false))));
                                                                                                txtBxdeadband.Invoke((Action)(() =>
                                                                                                    txtBxdeadband.Text = PlaceDecimal(ConvertHexToShort(val[2], false))));
                                                                                            }
                                                                                            myStruct.dead1 = val[2];
                                                                                        }

                                                                                        //  if (MoniterStatus)

                                                                                        //4713
                                                                                        hy2 = ConvertHexToShort(val[3], false);
                                                                                        if (!hys1V)
                                                                                        {
                                                                                            txtHysteresis1.Invoke((Action)(() =>
                                                                                                txtHysteresis1.Text = PlaceDecimal(ConvertHexToShort(val[3], false))));
                                                                                        }
                                                                                        myStruct.hys1 = val[3];

                                                                                        //   if (MoniterStatus)

                                                                                        //4714
                                                                                        hy3 = ConvertHexToShort(val[4], false);
                                                                                        if (!hys2V)
                                                                                        {
                                                                                            txtHysteresis2.Invoke((Action)(() =>
                                                                                                txtHysteresis2.Text = PlaceDecimal(ConvertHexToShort(val[4], false))));
                                                                                        }
                                                                                        myStruct.hys2 = val[4];
                                                                                       // if (Writedatab != true)
                                                                                        {
                                                                                            BindHMI(ctrl);  //test
                                                                                        }
                                                                                    }
                                                                                    catch (Exception ae)
                                                                                    { }
                                                                                    //4715
                                                                                    //4716
                                                                                    //4717
                                                                                }
                                                                                #endregion
                                                                                break;
                                                                            case 7:
                                                                                #region Case 7
                                                                                if (val.Count == 8)
                                                                                {

                                                                                    OLK = 0;
                                                                                    try
                                                                                    {
                                                                                        //need
                                                                                       // SensertypeDict
                                                                                        a1CmbBxSensorType = Convert.ToInt32(ConvertHexToShort(val[0], true));
                                                                                       if (a1CmbBxSensorType <= 18)
                                                                                        {
                                                                                            //4718
                                                                                            if (senseVal)
                                                                                            {

                                                                                                var sesvals = SensertypeDictread.FirstOrDefault(x => x.Key == a1CmbBxSensorType).Value;
                                                                                                CmbBxSensorType.Invoke((Action)(() => CmbBxSensorType.SelectedIndex = sesvals));
                                                                                            }
                                                                                            myStruct.sensorType = val[0];
                                                                                        }

                                                                                        a2CmbBxCtrlAction = Convert.ToInt32(ConvertHexToShort(val[1], true));
                                                                                        // if (MoniterStatus)

                                                                                        if (a2CmbBxCtrlAction < CmbBxCtrlAction.Items.Count)
                                                                                        {
                                                                                            //4719
                                                                                            if (ctrlVal)
                                                                                            {
                                                                                                CmbBxCtrlAction.Invoke((Action)(() => CmbBxCtrlAction.SelectedIndex = a2CmbBxCtrlAction));
                                                                                            }
                                                                                            myStruct.ctrlAction = val[1];
                                                                                        }



                                                                                        //4727  -7
                                                                                        if (autoTune)
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                regadd = "4727";
                                                                                                oAT = SendFrameToDevice1(key, regadd);
                                                                                                oA2CmbBxAutotuning = Convert.ToInt32(oAT);
                                                                                                // int autoInt = Convert.ToInt32(ConvertHexToShort(val[7], true));
                                                                                                if (oA2CmbBxAutotuning < CmbBxAutotuning.Items.Count)
                                                                                                {
                                                                                                    if (autoTune) //&& MoniterStatus
                                                                                                    {
                                                                                                        // CmbBxAutotuning.Invoke((Action)(() => CmbBxAutotuning.SelectedIndex = autoInt));
                                                                                                        CmbBxAutotuning.Invoke((Action)(() => CmbBxAutotuning.SelectedIndex = oA2CmbBxAutotuning));

                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        CmbBxAutotuning.Invoke((Action)(() =>
                                                                                                        {
                                                                                                            CmbBxAutotuning.SelectedIndex = -1;
                                                                                                            //  ispause = false;
                                                                                                        }));
                                                                                                    }
                                                                                                    ctrl.AT = oA2CmbBxAutotuning >= 1 ? true : false;
                                                                                                    myStruct.autoTune = oAT;


                                                                                                }
                                                                                            }
                                                                                            catch (Exception ae)
                                                                                            { }
                                                                                        }
                                                                                        if (Writedatab != true)
                                                                                        {
                                                                                            LED4728(ctrl);
                                                                                        }

                                                                                        if (!LeaveEvent)
                                                                                        {
                                                                                            regadd = "4700";
                                                                                            oAT4700 = SendFrameToDevice1(key, regadd);
                                                                                            if (oAT4700 == "")
                                                                                            {
                                                                                                while (oAT4700 == "")
                                                                                                {
                                                                                                    oAT4700 = SendFrameToDevice1(key, regadd);
                                                                                                }
                                                                                            }
                                                                                            if (oAT4700 == "8003")
                                                                                            {
                                                                                                OLK = 0;

                                                                                                pv =
                                                                                                    PlaceDecimal(ConvertHexToShort(oAT, false));
                                                                                                ctrl.Invoke((Action)(() => ctrl.PV = "No Connect"));
                                                                                                myStruct.pv = oAT4700;

                                                                                            }
                                                                                            else if (oAT4700 == "8004")
                                                                                            {
                                                                                                OLK = 0;
                                                                                                pv = PlaceDecimal(ConvertHexToShort(oAT4700, false));
                                                                                                ctrl.Invoke((Action)(() => ctrl.PV = "Input Error"));
                                                                                                myStruct.pv = oAT4700;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                OLK = 0;
                                                                                                pv = PlaceDecimal(ConvertHexToShort(oAT4700, false));
                                                                                                ctrl.Invoke((Action)(() => ctrl.PV = pv));
                                                                                                myStruct.pv = oAT4700;
                                                                                            }
                                                                                        }
                                                                                        //4701
                                                                                      //  if (Writedatab != true)
                                                                                        {
                                                                                            BindHMI(ctrl);
                                                                                        }

                                                                                    }
                                                                                    catch (Exception ae)
                                                                                    { }


                                                                                }
                                                                                #endregion
                                                                                break;
                                                                            case 8:
                                                                                #region Case 8
                                                                                if (val.Count == 8)
                                                                                {
                                                                                    OLK = 0;

                                                                                    try
                                                                                    {
                                                                                        if (Writedatab != true)
                                                                                        {
                                                                                            LED4728(ctrl);
                                                                                        }

                                                                                        // if (MoniterStatus)
                                                                                        {
                                                                                            //4708
                                                                                            a2U = ConvertHexToShort(val[0], false);
                                                                                            if (alM2UVal)
                                                                                            {
                                                                                                txtBxAlarm2Up.Invoke((Action)(() =>
                                                                                                    txtBxAlarm2Up.Text = PlaceDecimal(ConvertHexToShort(val[0], false))));
                                                                                            }
                                                                                            myStruct.alarm2Up = val[0];
                                                                                        }




                                                                                        //  if (MoniterStatus)
                                                                                        {
                                                                                            //4709
                                                                                            a2D = ConvertHexToShort(val[1], false);
                                                                                            if (alM2DVal)
                                                                                            {
                                                                                                txtBxAlarm2Down.Invoke((Action)(() =>
                                                                                                     txtBxAlarm2Down.Text = PlaceDecimal(ConvertHexToShort(val[1], false))));
                                                                                            }
                                                                                            myStruct.alarm2Down = val[1];
                                                                                        }



                                                                                        //470B


                                                                                        // if (MoniterStatus)
                                                                                        {
                                                                                            //470C
                                                                                            pb = ConvertHexToShort(val[4], false);
                                                                                            if (!pdV)
                                                                                            {
                                                                                                txtBxPD.Invoke((Action)(() =>
                                                                                                     txtBxPD.Text = PlaceDecimal(pb)));
                                                                                            }
                                                                                            myStruct.pb = val[4];
                                                                                        }


                                                                                        //  if (MoniterStatus)
                                                                                        {
                                                                                            //470D
                                                                                            td = ConvertHexToShortInt(val[5], true);
                                                                                            if (tdV)
                                                                                            {
                                                                                                txtBxTd.Invoke((Action)(() =>
                                                                                                     txtBxTd.Text = (td).ToString()));

                                                                                                if (txtBxTd.Text == "0")
                                                                                                {
                                                                                                    //  this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate () { MessageBox.Show(txtBxTd.Text); });
                                                                                                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { txtPDOffset.Visible = lblPDoffset.Visible = true; });
                                                                                                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { txtBxIoffset.Visible = lblIoffset.Visible = false; });

                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    // this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate () { MessageBox.Show(txtBxTd.Text); });
                                                                                                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { txtPDOffset.Visible = lblPDoffset.Visible = false; });
                                                                                                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { txtBxIoffset.Visible = lblIoffset.Visible = true; });
                                                                                                }
                                                                                            }
                                                                                            myStruct.td = val[5];
                                                                                        }


                                                                                        //   if (MoniterStatus)
                                                                                        {
                                                                                            
                                                                                            try
                                                                                            {
                                                                                                regadd = "470E";
                                                                                                oAT = SendFrameToDevice1(key, regadd);
                                                                                                while (oAT == "")
                                                                                                {
                                                                                                    oAT = SendFrameToDevice1(key, regadd);
                                                                                                }
                                                                                                oA2470E = ConvertHexToShortInt(oAT, true);
                                                                                            }
                                                                                            catch (Exception ae)
                                                                                            {
                                                                                                MessageBox.Show(ae.ToString());
                                                                                            }
                                                                                            if (tiV)
                                                                                            {
                                                                                                txtBxTi.Invoke((Action)(() =>
                                                                                                     txtBxTi.Text =
                                                                                               (oA2470E).ToString()));
                                                                                            }
                                                                                            myStruct.ti = oAT;

                                                                                        }


                                                                                        {
                                                                                            //470F
                                                                                            ioffset = ConvertHexToShort(val[7], false);
                                                                                            if (!onchangeIoffset)
                                                                                            {
                                                                                                txtBxIoffset.Invoke((Action)(() =>
                                                                                                     txtBxIoffset.Text = PlaceDecimal(ioffset)));
                                                                                            }
                                                                                            myStruct.iOffset = val[7];
                                                                                        }
                                                                                       // if (Writedatab != true)
                                                                                        {
                                                                                            BindHMI(ctrl);  //test
                                                                                        }
                                                                                    }
                                                                                    catch (Exception ae) { }
                                                                                }
                                                                                #endregion
                                                                                break;
                                                                            case 9:
                                                                                #region Case 9
                                                                                OLK = 0;


                                                                                if (val.Count == 7)
                                                                                {

                                                                                    //4701
                                                                                    try
                                                                                    {
                                                                                        {
                                                                                            //4702
                                                                                            if (!out1V)
                                                                                            {

                                                                                                txtBxOut1.Invoke((Action)(() => txtBxOut1.Text =
                                                                                                    PlaceDecimal(ConvertHexToShort(val[1], false))));
                                                                                            }
                                                                                            myStruct.out1 = val[1];
                                                                                        }


                                                                                        //  if (MoniterStatus)
                                                                                        {
                                                                                            //4703
                                                                                            if (!out2V)
                                                                                            {
                                                                                                txtBxOut2.Invoke((Action)(() => txtBxOut2.Text =
                                                                                                    PlaceDecimal(ConvertHexToShort(val[2], false))));
                                                                                            }
                                                                                            myStruct.out2 = val[2];
                                                                                        }

                                                                                        {
                                                                                            //4704
                                                                                            if (!highVal)
                                                                                            {
                                                                                                txtBxHightemp.Invoke((Action)(() => txtBxHightemp.Text =
                                                                                                    PlaceDecimal(ConvertHexToShort(val[3], false))));
                                                                                            }
                                                                                            myStruct.highTemp = val[3];
                                                                                        }

                                                                                        {
                                                                                            //4705
                                                                                            if (!lowVal)
                                                                                            {
                                                                                                txtBxLowtemp.Invoke((Action)(() => txtBxLowtemp.Text =
                                                                                                    PlaceDecimal(ConvertHexToShort(val[4], false))));
                                                                                               // MessageBox.Show(txtBxLowtemp.Text);
                                                                                            }
                                                                                            myStruct.lowTemp = val[4];
                                                                                        }



                                                                                        {
                                                                                            //4706     
                                                                                            if (alM1UVal)
                                                                                            {
                                                                                                txtBxAlarm1Up.Invoke((Action)(() => txtBxAlarm1Up.Text =
                                                                                                   PlaceDecimal(ConvertHexToShort(val[5], false))));
                                                                                            }
                                                                                            myStruct.alarm1Up = val[5];
                                                                                        }

                                                                                        {
                                                                                            //4707
                                                                                            if (alM1DVal)
                                                                                            {
                                                                                                txtBxAlarm1Down.Invoke((Action)(() => txtBxAlarm1Down.Text =
                                                                                                   PlaceDecimal(ConvertHexToShort(val[6], false))));
                                                                                            }
                                                                                            myStruct.alarm1Down = val[6];
                                                                                        }

                                                                                    }
                                                                                    catch (Exception ae)
                                                                                    {
                                                                                    }
                                                                                }

                                                                                try
                                                                                {
                                                                                    if (Writedatab != true)
                                                                                    {
                                                                                        LED4728(ctrl);
                                                                                       
                                                                                    }
                                                                                    BindHMI(ctrl);  //test
                                                                                    
                                                                                }
                                                                                catch (Exception ae)
                                                                                { }

                                                                                #endregion
                                                                                break;
                                                                            #endregion
                                                                        }
                                                                        //Bhushan need to check

                                                                        if (ctrl.SelectedNode == true && ctrl.Connected == true)
                                                                        {
                                                                            if (newList.Count == 3)
                                                                            {
                                                                                try
                                                                                {

                                                                                    ctrl.btnConnect_Click(null, null);

                                                                                }
                                                                                catch (Exception exx)
                                                                                {
                                                                                    //throw exx;
                                                                                }
                                                                                break;
                                                                            }
                                                                            else
                                                                            {
                                                                              //  if (Writedatab != true)
                                                                                {
                                                                                    BindHMI(ctrl);
                                                                                }
                                                                                //displayChart("Series" + ctrl.NodeAddress, ctrl.PvRecords);
                                                                            }
                                                                        }

                                                                        #region PV Graph
                                                                      //  while Rampsoak Need To check With Comment

                                                                        try
                                                                        {
                                                                            if (cnt == 10)
                                                                            {

                                                                                double PvValue = countt * Convert.ToInt32(key);
                                                                                double GraphPV = ConvertHexToShort(myStruct.pv, false);
                                                                                try
                                                                                {
                                                                                    ctrl.PvRecords.Add(countt, GraphPV); //PvValue
                                                                                }
                                                                                catch (Exception ae)
                                                                                {
                                                                                    Thread.Sleep(100);
                                                                                    countt = ctrl.PvRecords.Count;
                                                                                    countt = countt + 1;
                                                                                    ctrl.PvRecords.Add(countt, GraphPV);
                                                                                }
                                                                                DisplayToChart2("Series" + ctrl.NodeAddress, ctrl.PvRecords, key);
                                                                                cnt = 0;
                                                                                countt++;
                                                                            }
                                                                            cnt++;
                                                                        }
                                                                        catch (Exception ae)
                                                                        {
                                                                            //MessageBox.Show("PV Grapg issue");
                                                                        }
                                                                      
                                                                       
                                                                        #endregion

                                                                    }

                                                                    if (ctrl.Connected == true && ctrl.SelectedNode == false)
                                                                    {
                                                                        try
                                                                        {
                                                                            switch (frameCount)
                                                                            {
                                                                                case 1:
                                                                                    //471A
                                                                                    val11 = Convert.ToInt32(ConvertHexToShort(val[0], true));
                                                                                    ctrl.Invoke((Action)(() => ctrl.Out11 = val11));

                                                                                    //471F
                                                                                    val12 = Convert.ToInt32(ConvertHexToShort(val[5], true));
                                                                                    ctrl.Invoke((Action)(() => ctrl.Out12 = val12));
                                                                                    break;
                                                                                case 2:
                                                                                    #region Case 2
                                                                                    //4700
                                                                                    if (!LeaveEvent)
                                                                                    {
                                                                                        if (val[0] == "")
                                                                                        {
                                                                                            while (val[0] == "")
                                                                                            {
                                                                                                val[0] = SendFrameToDevice1(key, regadd);
                                                                                            }
                                                                                        }
                                                                                        if (val[0] == "8003")
                                                                                        {
                                                                                            OLK = 0;
                                                                                            pv = PlaceDecimal(ConvertHexToShort(val[0], false));
                                                                                            ctrl.Invoke((Action)(() => ctrl.PV = "No Connect"));
                                                                                        }
                                                                                        else if (val[0] == "8004")
                                                                                        {
                                                                                            OLK = 0;
                                                                                            pv = PlaceDecimal(ConvertHexToShort(val[0], false));
                                                                                            ctrl.Invoke((Action)(() => ctrl.PV = "Input Error"));
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            OLK = 0;
                                                                                            pv = PlaceDecimal(ConvertHexToShort(val[0], false));
                                                                                            ctrl.Invoke((Action)(() => ctrl.PV = pv));
                                                                                        }
                                                                                    }
                                                                                    //4701
                                                                                    sv = PlaceDecimal(ConvertHexToShort(val[1], false));
                                                                                    ctrl.Invoke((Action)(() => ctrl.SV = sv));

                                                                                    //While connected node Password Protection true avoid show  
                                                                                    //  if (MoniterStatus)
                                                                                    {
                                                                                        //4702
                                                                                        out1 = Convert.ToInt32(ConvertHexToShort(val[2], false));
                                                                                        ctrl.Invoke((Action)(() => ctrl.Out1Percent = out1));

                                                                                        //4703
                                                                                        out2 = Convert.ToInt32(ConvertHexToShort(val[3], false));
                                                                                        ctrl.Invoke((Action)(() => ctrl.Out2Percent = out2));
                                                                                    }
                                                                                    #endregion
                                                                                    break;
                                                                                case 4:
                                                                                    val2BindUnitType = Convert.ToInt32(ConvertHexToShort(val[1], true));
                                                                                    ctrl.Invoke((Action)(() => ctrl.Unit = BindUnitType(val2BindUnitType)));
                                                                                    break;

                                                                                case 8:
                                                                                    ledd = Convert.ToInt32(ConvertHexToShort(val[2], true));
                                                                                    ctrl.LedStatus = ledd;
                                                                                    break;
                                                                            }
                                                                        }
                                                                        catch (Exception ae) { }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        if (ErrFlag)
                                                        {
                                                            ctrl.Invoke((Action)(() =>
                                                            {
                                                                UserRemoveConnect(ctrl.NodeAddress.ToString());
                                                                ctrl.ResetControls();
                                                                MessageBox.Show("Error");
                                                            }));
                                                        }
                                                    }
                                                }
                                            }// foreach close
                                        }
                                    }

                                }
                            }

                            #region RampSoak
                            try
                            {
                                if (ramp)
                                {
                                    if (rampCount <= 10)
                                    {
                                        if (textBox1.InvokeRequired)
                                        {
                                            textBox1.Invoke((Action)(() =>
                                            {
                                                textBox1.Text = "ramp " + (rampCount++).ToString();
                                            }));
                                        }
                                    }

                                    if (rampCount > 10)
                                    {
                                        ReadRampSoakStop();

                                        frm.list = new List<string>() { "1", "2", "3", "4", "5" };

                                        frm.BindGrid();
                                    }
                                }
                            }
                            catch (Exception ae) { }
                            #endregion
                            //Thread.Sleep(1000);
                        }
                    }
                }
            }
            //catch (ThreadAbortException ev)
            //{
            //if (mainThread != null)
            //{
            //    if (mainThread.IsAlive)
            //    { mainThread = null; }


            //}

            //InstantiateThread();
            //_isRunning = true;
            //mainThread.Start();
            // }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("ExecuteThread:", ex.Message, "MonitorOnline_ErrorLog");
                //MessageBox.Show(ex.Message);
            }
        }


        private static string BindUnitType(int val2)
        {
            try
            {
                string type = "";

                switch (val2)
                {
                    case -1:
                        type = "";
                        break;
                    case 0:
                        type = "F";
                        break;
                    case 1:
                        type = "C";
                        break;
                    case 2:
                        type = "EU";
                        break;
                }
                return type;
            }
            catch (Exception ae)
            {
                return null;
            }
        }

        private void AutoDisconnect(ArrayList newList, int frameCount)
        {
            #region AutoDisconnect

            if (newList.Count == 0)
            {
                newList.Add(frameCount);
            }
            else if (newList.Count == 1)
            {
                if (Convert.ToInt32(newList[0]) == frameCount - 1)
                {
                    newList.Add(frameCount);
                }
                else
                {
                    newList.Clear();
                }
            }
            else if (newList.Count == 2)
            {
                if (Convert.ToInt32(newList[1]) == frameCount - 1)
                {
                    newList.Add(frameCount);
                }
                else
                {
                    newList.Clear();
                }
            }

            if (newList.Count == 3)
            {
                autoDisconnect = true;
            }
            #endregion
        }


        //private string PlaceDecimalHMI(double res)
        //{
        //    try
        //    {
        //        int index = 2;
        //        //index = CmbBxfraction.SelectedIndex;
        //        //   CmbBxfraction.Invoke((Action)(() => index = Convert.ToInt32(CmbBxfraction.SelectedIndex)));

        //        string result = "";

        //        switch (index)
        //        {
        //            case 0:
        //                result = String.Format("{0:0}", res / 1);
        //                break;
        //            case 1:
        //                result = String.Format("{0:0.0}", res / 1);
        //                break;
        //            case 2:
        //                result = String.Format("{0:0.00}", res / 10);
        //                break;
        //            case 3:
        //                result = String.Format("{0:0.000}", res / 100);
        //                break;
        //        }
        //        return (string.IsNullOrEmpty(result) ? "0" : result);
        //    }
        //    catch (Exception)
        //    {
        //        return "0";
        //    }
        //}

        private string PlaceDecimal(double res)
        {
            try
            {
                int index = 1;
                //index = CmbBxfraction.SelectedIndex;
                CmbBxfraction.Invoke((Action)(() => index = Convert.ToInt32(CmbBxfraction.SelectedIndex)));

                string result = "";

                switch (index)
                {
                    case 0:
                        res = res * 10;
                        result = String.Format("{0:0}", res / 1);
                        break;
                    case 1:
                        result = String.Format("{0:0.0}", res / 1);
                        break;
                    case 2:
                        result = String.Format("{0:0.00}", res / 10);
                        break;
                    case 3:
                        result = String.Format("{0:0.000}", res / 100);
                        break;
                }
                return (string.IsNullOrEmpty(result) ? "0" : result);
            }
            catch (Exception)
            {
                return "0";
            }
        }

        private double ConvertHexToShort(string hexVal, bool type)
        {
           
            {
                try
                {
                    short val1 = Convert.ToInt16(hexVal, 16);

                    double val2 = type ? Convert.ToDouble(val1) : Convert.ToDouble(val1) / 10;

                    return val2;
                }
                catch
                {
                    
                    return 0;
                }
            }
        }


        private Int32 ConvertHexToShortInt(string hexVal, bool type)
        {

            {
                try
                {
                    short val1 = Convert.ToInt16(hexVal, 16);

                    Int32 val2 = type ? Convert.ToInt32(val1) : Convert.ToInt32(val1) / 10;

                    return val2;
                }
                catch
                {

                    return 0;
                }
            }
        }



        //private string ConvertHexToShortSV(string hexVal, bool type)
        //{
        //    //if (string.IsNullOrEmpty(hexVal))
        //    //{
        //    //    return 0;
        //    //}
        //    //else
        //    string val2 = string.Empty;
        //    {
        //        try
        //        {
        //            short val1 = Convert.ToInt16(hexVal, 16);

        //             val2 =( type ? Convert.ToDouble(val1) : Convert.ToDouble(val1) / 10).ToString();

        //            return val2;
        //        }
        //        catch
        //        {
        //            //  MessageBox.Show(hexVal);
        //            return val2;
        //        }
        //    }
        //}


        private string hex2binary(string hexvalue)
        {
            string binaryval = "";
            binaryval = Convert.ToString(Convert.ToInt32(hexvalue, 16), 2);
            return binaryval;
        }

        //private string SendFrameToDevice(string unitAddress, string func, string regAddr, string data)
        //{
        //    string portName = SetValues.Set_PortName;
        //    string baudRate = SetValues.Set_Baudrate;
        //    string parity = SetValues.Set_parity;
        //    int bitsLength = SetValues.Set_BitsLength;
        //    string stopBits = SetValues.Set_StopBits;

        //    if (!string.IsNullOrEmpty(portName) && !string.IsNullOrEmpty(baudRate) && !string.IsNullOrEmpty(parity) &&
        //        bitsLength > 0 && !string.IsNullOrEmpty(stopBits))
        //    {
        //        try
        //        {
        //            int readTime = 150;
        //            if (func == "06")
        //            {
        //                readTime = 400;
        //            }
        //            //Thread.Sleep(100);  //AK 25/10
        //            //  if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength, readTime))
        //            {
        //                byte[] RecieveData = null;

        //                if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
        //                {
        //                    if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
        //                    {
        //                        RecieveData = modbusobj.AscFrame(unitAddress, func, regAddr, data);
        //                    }
        //                    else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex == 2)
        //                    {
        //                        RecieveData = modbusobj.RtuFrame(unitAddress, func, regAddr, data, baudRate);
        //                    }

        //                    if (RecieveData != null && RecieveData.Length > 0)
        //                    {
        //                        if (func == "03" || func == "06")
        //                        {
        //                            char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();

        //                            string result = "";

        //                            if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
        //                            {
        //                                result = string.Join("", recdata);
        //                            }
        //                            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex == 2)
        //                            {
        //                                result = modbusobj.DisplayFrame(RecieveData);
        //                            }

        //                            LogWriter.WriteToFile("MonitorOnline: 5)", result, "OnlineMonitor_ErrorLog");

        //                            byte[] sizeBytes = ExtractByteArray(RecieveData, 4, 7);

        //                            //int size = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(sizeBytes), 16);

        //                            //return (Convert.ToDouble(size) / 10);

        //                            return System.Text.Encoding.UTF8.GetString(sizeBytes);
        //                        }
        //                        else
        //                        {
        //                            return null;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        // data not received
        //                        LogWriter.WriteToFile("MonitorOnline: 4)", "Received Data Timeout!", "OnlineMonitor_ErrorLog");
        //                        //txtBxRecievecmd.Text = "Received Data Timeout!";
        //                        return null;
        //                    }
        //                }
        //                else
        //                {
        //                    LogWriter.WriteToFile("MonitorOnline: 3)", "Received Data Timeout!", "OnlineMonitor_ErrorLog");
        //                    //txtBxRecievecmd.Text = "Received Data Timeout!";
        //                    return null;
        //                }
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            LogWriter.WriteToFile("MonitorOnline: 1)", ex.Message, "OnlineMonitor_ErrorLog");
        //            //lblMessage.Text = "3" + ex.Message;

        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        // settings are empty
        //        return null;
        //    }
        //}

        private List<string> CreateFrames(string nodeAddress, string functionCode, string regAddress,
            string wordCount, bool read)
        {
            byte[] RecieveData = null;

            try
            {
                string portName = SetValues.Set_PortName;
                string baudRate = SetValues.Set_Baudrate;
                string parity = SetValues.Set_parity;
                int bitsLength = SetValues.Set_BitsLength;
                string stopBits = SetValues.Set_StopBits;

                // if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength))
                {
                    if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                    {
                        #region ASCII

                        if ((SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1) && SetValues.Set_CommType == 1)
                        {
                            //RecieveData = modbusobj.AscFrame(nodeAddress.PadLeft(2, '0'), functionCode, regAddress, wordCount);
                           // Convert.ToInt32(nodeAddress).ToString("X").PadLeft(2, '0')
                            RecieveData = modbusobj.AscFrame(nodeAddress,
                                Convert.ToInt32(functionCode).ToString("X").PadLeft(2, '0'), regAddress, wordCount);
                            if (functionCode == "03")
                            {
                                if (RecieveData != null && RecieveData.Length > 0)
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
                                else
                                {
                                    // MessageBox.Show("Can't Communicate");

                                }
                            }
                            else
                            {
                                // MessageBox.Show("Disconnected");
                                return null;
                            }
                        }
                        #endregion

                        #region RTU
                        else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                        {
                            RecieveData = modbusobj.RtuFrame(nodeAddress, functionCode, regAddress, wordCount, baudRate);

                            if (functionCode == "03")
                            {
                                if (RecieveData != null && RecieveData.Length > 0)
                                {
                                    //01 03 02 02 FF F9 64 
                                    int size = Convert.ToInt32(RecieveData[2]);
                                    if (RecieveData.Length >= (size + 3))
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
                            else
                            {
                                //  MessageBox.Show("Disconnected");
                                return null;
                            }

                        }
                        #endregion

                    }
                }

            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        private static byte[] ExtractByteArray(byte[] RecieveData, int size, int startPostition)
        {
            byte[] sizeBytes = new byte[size];

            Array.Copy(RecieveData, startPostition, sizeBytes, 0, size);
            return sizeBytes;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (modbusobj != null)
                {
                    if (modbusobj.IsSerialPortOpen())
                    {
                        modbusobj.CloseSerialPort();
                    }
                }

                if (th != null)
                {
                    if (online || ramp)
                    {
                        online = false;
                        ramp = false;
                    }
                    th.Abort();

                    th = null;
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
        }

        //temp : to remove
        #region old ramp

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void Connect()
        {
            try
            {
                if (btnConnect.Text == "Connect")
                {
                    btnConnect.Text = "Disconnect";

                    label1.Text = "Running Online";

                    // rampSoakProgramToolStripMenuItem.Enabled = true;

                    if (th != null)
                    {

                        running = true;
                        online = true;
                        th.Start();
                    }
                }
                else
                {

                    online = false;
                    running = false;
                    btnConnect.Text = "Connect";
                    th.Abort();
                    label1.Text = "";
                    //rampSoakProgramToolStripMenuItem.Enabled = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ReadWrite()
        {
            try
            {
                if (modbusobj != null)
                {
                    if (modbusobj.IsSerialPortOpen())
                    {
                        modbusobj.CloseSerialPort();
                    }
                }

                if (th != null)
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        if (running)
                        {
                            if (online)
                            {
                                if (textBox1.InvokeRequired)
                                {
                                    textBox1.Invoke((Action)(() =>
                                    {
                                        textBox1.Text = "online " + i.ToString();
                                    }));
                                }
                            }

                            if (ramp)
                            {
                                if (rampCount <= 10)
                                {
                                    if (textBox1.InvokeRequired)
                                    {
                                        textBox1.Invoke((Action)(() =>
                                        {
                                            textBox1.Text = "ramp " + (rampCount++).ToString();
                                        }));
                                    }
                                }

                                if (rampCount > 10)
                                {
                                    ReadRampSoakStop();

                                    frm.list = new List<string>() { "1", "2", "3", "4", "5" };

                                    frm.BindGrid();
                                }
                            }
                        }
                        else
                        {
                            if (label1.InvokeRequired)
                            {
                                label1.Invoke((Action)(() =>
                                {
                                    label1.Text = "Stopped";
                                }));
                            }
                            break;
                        }

                        //Thread.Sleep(1000);  //AK 25/10
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void CopyDictionary(Dictionary<string, int> usrDictionary, ref Dictionary<string, int> usrDictionaryCopy)
        {
            foreach (var item in usrDictionary)
            {
                if (!usrDictionaryCopy.ContainsKey(item.Key))
                {
                    usrDictionaryCopy.Add(item.Key, item.Value);
                }
            }
        }

        private void ReadRampSoakStart()
        {
            try
            {
                Pause();

                online = false;

                ramp = true;

                CopyDictionary(usrDictionary, ref usrDictionaryCopy);

                if (label1.InvokeRequired)
                {
                    label1.Invoke((Action)(() =>
                    {
                        label1.Text = "Running Ramp";
                    }));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ReadRampSoakStop()
        {
            try
            {
                rampCount = 1;

                ramp = false;

                _isRunning = online = true;

                if (label1.InvokeRequired)
                {
                    label1.Invoke((Action)(() =>
                    {
                        label1.Text = "Running Online";
                    }));
                }
                Resume();
                CopyDictionary(usrDictionaryCopy, ref usrDictionary);
                if (!mainThread.IsAlive)
                {
                    _AREvt = new AutoResetEvent(false);
                    mainThread = null;
                    InstantiateThread();
                    mainThread.Priority = ThreadPriority.Lowest;
                    mainThread.Start();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        private void rampSoakProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (modbusobj != null)
            {
                frm = new RampSoakForm(modbusobj);
                frm.PauseItemCallback += PauseThisForm;
                PauseThisForm(true);
                frm.NodeAddress = selectedNode.ToString();
                rampSoakProgramToolStripMenuItem.Enabled = false;

                DialogResult dr = frm.ShowDialog();
            }
            rampSoakProgramToolStripMenuItem.Enabled = true;


            //selectedNode = 2;
            //Rampsoak form = new Rampsoak(modbusobj, (byte)selectedNode);
            //form.ShowDialog();

        }

        private bool PauseThisForm(bool item)
        {
            if (item)
            {
                online = false;
                _isRunning = false;
                ramp = true;
                CopyDictionary(usrDictionary, ref usrDictionaryCopy);
                this._manualResetEvent.Reset();
            }
            else
            {
               
                online = true;
                _isRunning = true;
                ramp = false;
                usrDictionary.Clear();
                CopyDictionary(usrDictionaryCopy, ref usrDictionary);
                this._manualResetEvent.Set();
                if (mainThread != null)
                {
                    string str = Convert.ToString(mainThread.ThreadState);
                    if (mainThread.ThreadState == ThreadState.Stopped)
                    {

                    }
                  //  _AREvt = new AutoResetEvent(false);
                    mainThread = null;

                    if (mainThread == null)
                    {
                        //rampsoak form to moniter form Switch so need to sleep proper response
                       // Thread.Sleep(15000);
                        InstantiateThread();
                    }

                    if (mainThread != null)
                    {
                        _AREvt = new AutoResetEvent(false);
                        _isRunning = true;
                        mainThread.Priority = ThreadPriority.Lowest;
                        mainThread.Start();

                    }
                }
            }

            return true;
        }

        #endregion

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                grpSettings.Visible = settingsToolStripMenuItem.Checked = true;
                grpPvRecords.Visible = pVRecordsToolStripMenuItem.Checked = false;

                grpSettings.BringToFront();
                grpPvRecords.SendToBack();
            }
            catch (Exception ae)
            { }
        }

        private void pVRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string nodedata = string.Empty;
                grpSettings.Visible = settingsToolStripMenuItem.Checked = false;
                grpPvRecords.Visible = pVRecordsToolStripMenuItem.Checked = true;

                grpPvRecords.BringToFront();
                grpSettings.SendToBack();
                Point p = new Point(60, 360);

                grpPvRecords.Location = p;

                UserControl1 ctrl = usrList.Find(x => x.SelectedNode == true);
                string nodeAddress = "Series" + ctrl.NodeAddress;
                CreateChart();
                CreateDefaultChartSeries();
                DisplayToChart2(nodeAddress, ctrl.PvRecords, nodedata);
            }
            catch (Exception ae) { }
        }

        private Dictionary<string, Series> listSeries = new Dictionary<string, Series>();
        private VerticalLineAnnotation v1;
        private RectangleAnnotation r1, r2;

        private void MonitorOnline_Load(object sender, EventArgs e)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                panel1.Visible = false;//temp:  to remove

                CmbBxfraction.SelectedIndex = 1;

                fILEToolStripMenuItem.Enabled = false;
                pARAMETERSToolStripMenuItem.Enabled = false;
                vIEWToolStripMenuItem.Enabled = false;
                //pARAMETERSToolStripMenuItem.Enabled = true;
                fILEToolStripMenuItem.Enabled = false;
                rampSoakProgramToolStripMenuItem.Enabled = true; // false;

                grpSettings.Visible = false;
                grpPvRecords.Visible = false;

                PictureBoxChange(pictureBox1, Color.LightGray);
                PictureBoxChange(pictureBox3, Color.LightGray);
                PictureBoxChange(pictureBox2, Color.LightGray);
                PictureBoxChange(pictureBox4, Color.LightGray);
                PictureBoxChange(pictureBox5, Color.LightGray);
                PictureBoxChange(pictureBox6, Color.LightGray);
                PictureBoxChange(pictureBox7, Color.LightGray);
                PictureBoxChange(pictureBox8, Color.LightGray);
                // PictureBoxChange(pictureBox9, Color.LightGray);
                if (th == null)
                {
                    th = new Thread(ReadWrite);
                    th.IsBackground = true;
                }
                
                // ContainerFilter();
            }
            catch (Exception ae) { }
        }

        private void MakeMenusActiveInactive(ToolStripMenuItem deviceToolStripMenuItem, bool status)
        {
            try
            {
                string input = deviceToolStripMenuItem.Text;

                if (status)
                {
                    //deviceToolStripMenuItem.Text = input + " (ACTIVE)";
                }
                else
                {
                    deviceToolStripMenuItem.Text = input;
                }
            }
            catch (Exception ae)
            {

            }

            //if (deviceToolStripMenuItem.Text.Contains("(ACTIVE)"))
            //{
            //    int index = input.IndexOf("(");

            //    if (index > 0)
            //        input = input.Substring(0, index);
            //    deviceToolStripMenuItem.Text = input;
            //}
            //else
            //{
            //    deviceToolStripMenuItem.Text = input + " (ACTIVE)";
            //}
        }

        //------------------------------------------------------------------------------------------------------
        #region Device active

        public void device1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserControl1 user = usrList.Find(x => x.NodeId == "1");

            user.btnConnect_Click(null, null);

            if (user.Connected)
            {
                MakeMenusActiveInactive(device1ToolStripMenuItem, true);
            }
            else
            {
                MakeMenusActiveInactive(device1ToolStripMenuItem, false);
            }
        }

        private void device2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserControl1 user = usrList.Find(x => x.NodeId == "2");

            user.btnConnect_Click(null, null);

            if (user.Connected)
            {
                MakeMenusActiveInactive(device2ToolStripMenuItem, true);
            }
            else
            {
                MakeMenusActiveInactive(device2ToolStripMenuItem, false);
            }
        }

        private void device3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserControl1 user = usrList.Find(x => x.NodeId == "3");

            user.btnConnect_Click(null, null);

            if (user.Connected)
            {
                MakeMenusActiveInactive(device3ToolStripMenuItem, true);
            }
            else
            {
                MakeMenusActiveInactive(device3ToolStripMenuItem, false);
            }
        }

        private void device4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserControl1 user = usrList.Find(x => x.NodeId == "4");

            user.btnConnect_Click(null, null);

            if (user.Connected)
            {
                MakeMenusActiveInactive(device4ToolStripMenuItem, true);
            }
            else
            {
                MakeMenusActiveInactive(device4ToolStripMenuItem, false);
            }
        }

        //private void device5ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UserControl1 user = usrList.Find(x => x.NodeId == "5");

        //    user.btnConnect_Click(null, null);

        //    if (user.Connected)
        //    {
        //        MakeMenusActiveInactive(device5ToolStripMenuItem, true);
        //    }
        //    else
        //    {
        //        MakeMenusActiveInactive(device5ToolStripMenuItem, false);
        //    }
        //}

        //private void device6ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UserControl1 user = usrList.Find(x => x.NodeId == "6");

        //    user.btnConnect_Click(null, null);

        //    if (user.Connected)
        //    {
        //        MakeMenusActiveInactive(device6ToolStripMenuItem, true);
        //    }
        //    else
        //    {
        //        MakeMenusActiveInactive(device6ToolStripMenuItem, false);
        //    }
        //}

        //private void device7ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UserControl1 user = usrList.Find(x => x.NodeId == "7");

        //    user.btnConnect_Click(null, null);

        //    if (user.Connected)
        //    {
        //        MakeMenusActiveInactive(device7ToolStripMenuItem, true);
        //    }
        //    else
        //    {
        //        MakeMenusActiveInactive(device7ToolStripMenuItem, false);
        //    }
        //}

        //private void device8ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UserControl1 user = usrList.Find(x => x.NodeId == "8");

        //    user.btnConnect_Click(null, null);

        //    if (user.Connected)
        //    {
        //        MakeMenusActiveInactive(device8ToolStripMenuItem, true);
        //    }
        //    else
        //    {
        //        MakeMenusActiveInactive(device8ToolStripMenuItem, false);
        //    }
        //}
        #endregion

        //------------------------------------------------------------------------------------------------------
        #region Private Methods
        //private void createChart(string seriesName)
        //{
        //    try
        //    {
        //        chart1.Invoke((Action)(() =>
        //        {
        //            if (chart1.Series.IsUniqueName(seriesName))
        //            {
        //                chart1.ChartAreas[0].BackColor = Color.Black;

        //                chart1.Series.Clear();
        //                chart1.Series.Add(seriesName);
        //                chart1.Series[seriesName].ChartType = SeriesChartType.StepLine;
        //                chart1.Series[seriesName].BorderWidth = 1;
        //                chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
        //                chart1.ChartAreas[0].AxisY.IntervalType = DateTimeIntervalType.Number;

        //                //chart1.ChartAreas[0].AxisX.ScaleView.Size = 300;
        //                //chart1.ChartAreas[0].AxisY.ScaleView.Size = 35;

        //                ////chart title  
        //                //chart1.Titles.Add("Chart Name");

        //                ////X-axis-------------------------------------------------
        //                // X-axis line color
        //                this.chart1.ChartAreas[0].AxisX.LineColor = Color.White;
        //                // | vertical lines color
        //                this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.White;
        //                // X-axis label color
        //                this.chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
        //                // disable X-axis vertical lines
        //                chart1.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;

        //                chart1.Series[seriesName].IsVisibleInLegend = false;

        //                ////Y-axis-------------------------------------------------
        //                // Y-axis line color
        //                this.chart1.ChartAreas[0].AxisY.LineColor = Color.White;
        //                // -- horizontal lines color
        //                this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.WhiteSmoke;
        //                // Y-axis label color
        //                this.chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
        //                // disable Y-axis vertical lines
        //                //chart1.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;

        //                //chart1.ChartAreas[0].AxisX.Interval = 1.0;// 0.001;
        //                //chart1.ChartAreas[0].AxisY.Interval = 15;

        //                if (listSeries.ContainsKey(seriesName))
        //                {
        //                    listSeries.Remove(seriesName);
        //                }

        //                listSeries.Add(seriesName, new Series());
        //            }
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void removeChart(string seriesName)
        //{
        //    if (listSeries.ContainsKey(seriesName))
        //    {
        //        listSeries.Remove(seriesName);
        //    }
        //}

        //private void fillChart(string seriesName, double key, double value)
        //{
        //    try
        //    {
        //        Series series = listSeries.Where(x => x.Key == seriesName).First().Value;

        //        series.Points.AddXY(key, value);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void displayChart(string seriesName, Dictionary<double, double> list)
        //{
        //    try
        //    {

        //        Series series = new Series();
        //        series.ChartType = SeriesChartType.StepLine;
        //        series.BorderWidth = 1;
        //        series.IsVisibleInLegend = false;
        //      foreach (var item in list)
        //        {
        //            //MessageBox.Show(countChart.ToString());
        //            if (countChart % 60 == 0) //20
        //            {
        //                CreateVerticalAnnotation(countChart, item.Value);
        //            }
        //            // string a = (item.Value).ToString();
        //            // MessageBox.Show(a);
        //            // series.Points.AddXY(item.Key, a);
        //            series.Points.AddXY(item.Key, item.Value);
        //            countChart++;
        //        }

        //        //Series series = listSeries.Where(x => x.Key == seriesName).First().Value;

        //        chart1.Invoke((Action)(() =>
        //        {
        //            chart1.Series.Clear();

        //            chart1.ChartAreas[0].BackColor = Color.Black;

        //            ////X-axis-------------------------------------------------
        //            // X-axis line color
        //            this.chart1.ChartAreas[0].AxisX.LineColor = Color.White;
        //            // | vertical lines color
        //            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.White;
        //            // X-axis label color
        //            this.chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
        //            // disable X-axis vertical lines
        //            chart1.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;

        //            chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
        //            chart1.ChartAreas[0].AxisY.IntervalType = DateTimeIntervalType.Number;

        //            chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.Maximum;//300; Bhushan Test 
        //            chart1.ChartAreas[0].AxisY.ScaleView.Size = chart1.ChartAreas[0].AxisY.Maximum; //35;

        //            ////Y-axis-------------------------------------------------
        //            // Y-axis line color
        //            this.chart1.ChartAreas[0].AxisY.LineColor = Color.White;
        //            // -- horizontal lines color
        //            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.WhiteSmoke;
        //            // Y-axis label color
        //            this.chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;

        //            //chart1.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;

        //            chart1.ChartAreas[0].AxisX.Interval = 100.0;// 0.001;
        //            chart1.ChartAreas[0].AxisY.Interval = 50;
        //            if (chart1.Series.IsUniqueName(seriesName))
        //            {
        //                chart1.Series.Add(series);
        //            }
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void CreateVerticalAnnotation(double vallCount, double a)
        //{
        //    try
        //    {
        //        #region VerticalAnno
        //        chart1.Invoke((Action)(() =>
        //        {
        //            double maxDataPoint = chart1.ChartAreas[0].AxisY.Maximum;
        //            double minDataPoint = chart1.ChartAreas[0].AxisY.Minimum;

        //            LineAnnotation annotation2 = new LineAnnotation();
        //            annotation2.IsSizeAlwaysRelative = false;
        //            annotation2.AxisX = chart1.ChartAreas[0].AxisX;
        //            annotation2.AxisY = chart1.ChartAreas[0].AxisY;
        //            annotation2.AnchorY = minDataPoint;
        //            annotation2.Height = maxDataPoint;// -minDataPoint;
        //            annotation2.Width = 0;
        //            annotation2.LineWidth = 1;
        //            annotation2.LineDashStyle = ChartDashStyle.Dot;
        //            annotation2.StartCap = LineAnchorCapStyle.None;
        //            annotation2.EndCap = LineAnchorCapStyle.None;
        //            annotation2.AnchorX = vallCount;  //vallCount;  // <- your point  
        //            annotation2.LineColor = Color.White; // <- your color
        //            chart1.Annotations.Add(annotation2);

        //            //below Commented Code Enable For test
        //            //v1 = new VerticalLineAnnotation();
        //            //v1.AxisX = chart1.ChartAreas[0].AxisX;
        //            //v1.LineColor = Color.Red;
        //            //v1.LineDashStyle = ChartDashStyle.Dash;
        //            //v1.LineWidth = 1;
        //            //v1.AllowMoving = false;
        //            //v1.AllowSelecting = false;
        //            //v1.AllowResizing = false;
        //            //v1.X = vallCount;
        //            //v1.IsInfinitive = true;

        //            //r2 = new RectangleAnnotation();
        //            //r2.AxisX = chart1.ChartAreas[0].AxisX;
        //            //r2.IsSizeAlwaysRelative = false;
        //            //r2.Width = 15;
        //            //r2.Height = 2;

        //            //r2.BackColor = Color.FromArgb(128, Color.Transparent);
        //            //r2.LineColor = Color.Transparent;
        //            //r2.AxisY = chart1.ChartAreas[0].AxisY;

        //            //r2.Text = a.ToString();

        //            //r2.ForeColor = Color.Black;
        //            //r2.Font = new System.Drawing.Font("Arial", 8f);

        //            //r2.Y = a;
        //            //r2.X = v1.X;

        //            //chart1.Annotations.Add(v1);
        //            //chart1.Annotations.Add(r2);
        //            chart1.Update();
        //        }));
        //        #endregion
        //    }
        //    catch (Exception ae) { }
        //}

        public void ContainerFilter()
        {
            //  if (MoniterStatus)
            {
                try
                {

                    Point p = new Point(420, 21);
                    Point q = new Point(916, 144);
                    Point r = new Point(915, 19);
                    Point s = new Point(1091, 18);

                    pgrpBxManual.Location = q;
                    pgrpbx_PID_Parameter.Location = s;
                    pgrpBxOnOff.Location = r;

                    cmbBxSetvalue.Visible = false;
                    txtBxSetvalue.Visible = true;

                    int ctrlActionSelected = CmbBxCtrlAction.SelectedIndex;

                    int CmbBx1stout1 = CmbBx1stout.SelectedIndex;
                    int CmbBx2ndout2 = CmbBx2ndout.SelectedIndex;

                    pgrpbx_Alarm.Visible = true;
                    grpbx_Alarm1.Visible = true;
                    grpbx_Alarm2.Visible = true;

                    UserControl1 ctrl = usrList.Find(x => x.SelectedNode == true);

                    ctrl.PatternStepBool = false;
                    ctrl.RemainTimeBool = false;
                    ctrl.SvBool = false;

                    switch (ctrlActionSelected)
                    {
                        case 1://ON/OFF
                            pgrpBxOnOff.Location = p;
                            ctrl.SvBool = true;

                            #region 1

                            if (CmbBx1stout1 == 0 && CmbBx2ndout2 == 0) //h h
                            {
                                txtHysteresis1.Visible = lblHysteresis1.Visible = true;

                                txtHysteresis2.Visible = lblHysteresis2.Visible = true;

                                txtHysteresisDeadBand.Visible = lblHysteresisDeadBand.Visible = false;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 0 && CmbBx2ndout2 == 1) // h c
                            {
                                txtHysteresis1.Visible = lblHysteresis1.Visible = true;

                                txtHysteresis2.Visible = lblHysteresis2.Visible = true;

                                txtHysteresisDeadBand.Visible = lblHysteresisDeadBand.Visible = true;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 1 && CmbBx2ndout2 == 0) // c h 
                            {
                                txtHysteresis1.Visible = lblHysteresis1.Visible = true;

                                txtHysteresis2.Visible = lblHysteresis2.Visible = true;

                                txtHysteresisDeadBand.Visible = lblHysteresisDeadBand.Visible = true;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = false;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 1 && CmbBx2ndout2 == 1) // c c 
                            {
                                txtHysteresis1.Visible = lblHysteresis1.Visible = true;

                                txtHysteresis2.Visible = lblHysteresis2.Visible = true;

                                txtHysteresisDeadBand.Visible = lblHysteresisDeadBand.Visible = false;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = false;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 2 && (CmbBx2ndout2 == 0 || CmbBx2ndout2 == 1)) //a h/c 
                            {
                                txtHysteresis1.Visible = lblHysteresis1.Visible = false;

                                txtHysteresis2.Visible = lblHysteresis2.Visible = true;

                                txtHysteresisDeadBand.Visible = lblHysteresisDeadBand.Visible = false;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = false;

                                grpbx_Alarm1.Visible = true;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if ((CmbBx1stout1 == 0 || CmbBx1stout1 == 1) && CmbBx2ndout2 == 2) // h/c  a
                            {
                                txtHysteresis1.Visible = lblHysteresis1.Visible = true;

                                txtHysteresis2.Visible = lblHysteresis2.Visible = false;

                                txtHysteresisDeadBand.Visible = lblHysteresisDeadBand.Visible = false;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = true;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 2 && CmbBx2ndout2 == 2)
                            {
                                txtHysteresis1.Visible = lblHysteresis1.Visible = false;

                                txtHysteresis2.Visible = lblHysteresis2.Visible = false;

                                txtHysteresisDeadBand.Visible = lblHysteresisDeadBand.Visible = false;
                            }
                            AlarmGroupBoxWhenEitherisNotSet(CmbBx1stout1, CmbBx2ndout2);
                            #endregion
                            break;
                        case 2://Manual Tuning
                            pgrpBxManual.Location = p;
                            ctrl.SvBool = true;

                            #region 2
                            AlarmGroupBoxWhenEitherisNotSet(CmbBx1stout1, CmbBx2ndout2);
                            if (CmbBx1stout1 == 0 && CmbBx2ndout2 == 0) //h h
                            {
                                txtBxOut1.Visible = lblOut11.Visible = true;
                                txtBxCtrlPeriod1.Visible = lblCtrlPeriod1.Visible = true;

                                txtBxOut2.Visible = lblOut12.Visible = true;
                                txtBxCtrlPeriod2.Visible = lblCtrlPeriod2.Visible = true;

                                txtBxTuneOHigh.Visible = lblTuneOHigh.Visible = true;
                                txtBxTuneOLow.Visible = lblTuneOLow.Visible = true;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = false;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 0 && CmbBx2ndout2 == 1) // h c
                            {
                                txtBxOut1.Visible = lblOut11.Visible = true;
                                txtBxCtrlPeriod1.Visible = lblCtrlPeriod1.Visible = true;

                                txtBxOut2.Visible = lblOut12.Visible = true;
                                txtBxCtrlPeriod2.Visible = lblCtrlPeriod2.Visible = true;

                                txtBxTuneOHigh.Visible = lblTuneOHigh.Visible = true;
                                txtBxTuneOLow.Visible = lblTuneOLow.Visible = true;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = false;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 1 && CmbBx2ndout2 == 0) // c h 
                            {
                                txtBxOut1.Visible = lblOut11.Visible = true;
                                txtBxCtrlPeriod1.Visible = lblCtrlPeriod1.Visible = true;

                                txtBxOut2.Visible = lblOut12.Visible = true;
                                txtBxCtrlPeriod2.Visible = lblCtrlPeriod2.Visible = true;

                                txtBxTuneOHigh.Visible = lblTuneOHigh.Visible = true;
                                txtBxTuneOLow.Visible = lblTuneOLow.Visible = true;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = false;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 1 && CmbBx2ndout2 == 1) // c c 
                            {
                                txtBxOut1.Visible = lblOut11.Visible = true;
                                txtBxCtrlPeriod1.Visible = lblCtrlPeriod1.Visible = true;

                                txtBxOut2.Visible = lblOut12.Visible = true;
                                txtBxCtrlPeriod2.Visible = lblCtrlPeriod2.Visible = true;

                                txtBxTuneOHigh.Visible = lblTuneOHigh.Visible = true;
                                txtBxTuneOLow.Visible = lblTuneOLow.Visible = true;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = false;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 2 && (CmbBx2ndout2 == 0 || CmbBx2ndout2 == 1)) //a h/c 
                            {
                                txtBxOut1.Visible = lblOut11.Visible = false;
                                txtBxCtrlPeriod1.Visible = lblCtrlPeriod1.Visible = false;

                                txtBxOut2.Visible = lblOut12.Visible = true;
                                txtBxCtrlPeriod2.Visible = lblCtrlPeriod2.Visible = true;

                                txtBxTuneOHigh.Visible = lblTuneOHigh.Visible = true;
                                txtBxTuneOLow.Visible = lblTuneOLow.Visible = true;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = false;

                                grpbx_Alarm1.Visible = true;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if ((CmbBx1stout1 == 0 || CmbBx1stout1 == 1) && CmbBx2ndout2 == 2) // h/c  a
                            {
                                txtBxOut1.Visible = lblOut11.Visible = true;
                                txtBxCtrlPeriod1.Visible = lblCtrlPeriod1.Visible = true;

                                txtBxOut2.Visible = lblOut12.Visible = false;
                                txtBxCtrlPeriod2.Visible = lblCtrlPeriod2.Visible = false;

                                txtBxTuneOHigh.Visible = lblTuneOHigh.Visible = true;
                                txtBxTuneOLow.Visible = lblTuneOLow.Visible = true;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = false;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = true;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            #endregion
                            break;
                        case 0://PID
                            pgrpbx_PID_Parameter.Location = p;
                            ctrl.SvBool = true;

                            #region 0
                            AlarmGroupBoxWhenEitherisNotSet(CmbBx1stout1, CmbBx2ndout2);
                            if (CmbBx1stout1 == 0 && CmbBx2ndout2 == 0) //h h
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;
                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;
                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae)
                                { }

                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = true;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = false;
                                txtBxdeadband.Visible = lbldeadband.Visible = false;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 0 && CmbBx2ndout2 == 1) // h c
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;

                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;
                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        // MessageBox.Show(txtBxTd.Text);
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae)
                                {

                                }
                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = true;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = true;
                                txtBxdeadband.Visible = lbldeadband.Visible = true;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 1 && CmbBx2ndout2 == 0) // c h 
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;

                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;

                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae)
                                { }

                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = true;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = true;
                                txtBxdeadband.Visible = lbldeadband.Visible = true;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 1 && CmbBx2ndout2 == 1) // c c 
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;

                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;
                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae)
                                { }
                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = true;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = false;
                                txtBxdeadband.Visible = lbldeadband.Visible = false;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 2 && (CmbBx2ndout2 == 0 || CmbBx2ndout2 == 1)) //a h/c 
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;

                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = false;

                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae) { }

                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = true;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = false;
                                txtBxdeadband.Visible = lbldeadband.Visible = false;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = true;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if ((CmbBx1stout1 == 0 || CmbBx1stout1 == 1) && CmbBx2ndout2 == 2) // h/c  a
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;

                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;

                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae)
                                { }

                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = false;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = false;
                                txtBxdeadband.Visible = lbldeadband.Visible = false;

                                CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = true;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            #endregion
                            break;
                        case 3://PID Program Control
                            pgrpbx_PID_Parameter.Location = p;
                            lblAutotuning.Visible = false;
                            CmbBxAutotuning.Visible = false;
                            #region 3
                            AlarmGroupBoxWhenEitherisNotSet(CmbBx1stout1, CmbBx2ndout2);
                            if (CmbBx1stout1 == 0 && CmbBx2ndout2 == 0) //h h
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;

                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;

                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae) { }

                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = true;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = false;
                                txtBxdeadband.Visible = lbldeadband.Visible = false;

                                //CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 0 && CmbBx2ndout2 == 1) // h c
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;

                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;

                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae) { }
                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = true;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = true;
                                txtBxdeadband.Visible = lbldeadband.Visible = true;

                                // CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 1 && CmbBx2ndout2 == 0) // c h 
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;

                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;

                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae) { }

                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = true;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = true;
                                txtBxdeadband.Visible = lbldeadband.Visible = true;

                                // CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 1 && CmbBx2ndout2 == 1) // c c 
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;

                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;

                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae) { }
                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = true;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = false;
                                txtBxdeadband.Visible = lbldeadband.Visible = false;

                                //  CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if (CmbBx1stout1 == 2 && (CmbBx2ndout2 == 0 || CmbBx2ndout2 == 1)) //a h/c 
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;

                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = false;

                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae)
                                { }

                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = true;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = false;
                                txtBxdeadband.Visible = lbldeadband.Visible = false;

                                // CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = true;

                                grpbx_Alarm2.Visible = false;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            else if ((CmbBx1stout1 == 0 || CmbBx1stout1 == 1) && CmbBx2ndout2 == 2) // h/c  a
                            {
                                txtBxPD.Visible = lblPD.Visible = true;
                                txtBxTi.Visible = lblTi.Visible = true;

                                txtBxTd.Visible = lblTd.Visible = true;
                                txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;
                                try
                                {
                                    if (txtBxTd.Text == "0")
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = true;
                                        txtBxIoffset.Visible = lblIoffset.Visible = false;
                                    }
                                    else
                                    {
                                        txtPDOffset.Visible = lblPDoffset.Visible = false;
                                        txtBxIoffset.Visible = lblIoffset.Visible = true;
                                    }
                                }
                                catch (Exception ae)
                                { }
                                txtBxctrlPer2.Visible = lblctrlPer2.Visible = false;

                                txtBxPCoefficient.Visible = lblPCoefficient.Visible = false;
                                txtBxdeadband.Visible = lbldeadband.Visible = false;

                                //  CmbBxAutotuning.Visible = lblAutotuning.Visible = true;

                                grpbx_Alarm1.Visible = false;

                                grpbx_Alarm2.Visible = true;

                                if (grpbx_Alarm1.Visible || grpbx_Alarm2.Visible)
                                {
                                    pgrpbx_Alarm.Visible = true;
                                }
                                else
                                {
                                    pgrpbx_Alarm.Visible = false;
                                }
                            }
                            #endregion
                            cmbBxSetvalue.Visible = true;
                            txtBxSetvalue.Visible = false;
                            cmbBxSetvalue.SelectedIndex = 1;
                            break;
                    }
                }
                catch (Exception ae) { }
            }
        }

        private void AlarmGroupBoxWhenEitherisNotSet(int CmbBx1stout1, int CmbBx2ndout2)
        {
            try
            {
                if (CmbBx1stout1 == -1 || CmbBx2ndout2 == -1)
                {
                    pgrpbx_Alarm.Visible = true;

                    if (CmbBx1stout1 == -1)
                    {
                        grpbx_Alarm1.Visible = false;
                    }
                    if (CmbBx2ndout2 == -1)
                    {
                        grpbx_Alarm2.Visible = false;
                    }
                }
            }
            catch (Exception ae) { }
        }

        private string LeaveText(string regAddr, string data)
        {
            try
            {
                LeaveEvent = true;
                if (mainThread != null)
                {

                    string val = SendFrameToDevice2(selectedNode.ToString(), "06", regAddr, data);
                    // Resume();
                    if (val == null || val == "" || val.Length != 4 || val.Contains("?") || val.Contains("("))
                    {
                        val = SendFrameToDevice2(selectedNode.ToString(), "06", regAddr, data);

                        if (val == null || val == "" || val.Length != 4 || val.Contains("?") || val.Contains("("))
                        {
                            val = SendFrameToDevice2(selectedNode.ToString(), "06", regAddr, data);
                        }
                    }
                    LeaveEvent = false;
                    //MessageBox.Show(val);
                    return string.IsNullOrEmpty(val) ? null : val;
                }
                else
                    return null;
            }
            catch (Exception ae)
            {
                return null;
            }
        }

        private void Resume()
        {

            try
            {
                // ispause = false;
                _manualResetEvent.Set();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void Pause()
        {
            try
            {

                // ispause = true;
                _manualResetEvent.Reset();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void EnterText()
        {
            if (mainThread != null)
            {
                Pause();
            }
        }

        private string ConvertShortToHex1(int val, bool type)
        {
            try
            {
                if (val <= 0)
                {
                    return "0000";
                }
                else
                {
                    try
                    {
                        double val3 = type ? Convert.ToDouble(val) : Convert.ToDouble(val) * 10;
                        return Convert.ToInt16(val3).ToString("X2").PadLeft(4, '0');
                    }
                    catch
                    {
                        return "0000";
                    }
                }
            }
            catch (Exception ae)
            {
                return "0000";
            }
        }

        private string ConvertShortToHex(string val, bool type)
        {

            if (string.IsNullOrEmpty(val))
            {
                //  MessageBox.Show(val);
                return "0000";

            }
            else
            {
                try
                {
                    double val3 = type ? Convert.ToDouble(val) : Convert.ToDouble(val) * 10;
                    return Convert.ToInt16(val3).ToString("X2").PadLeft(4, '0');
                }
                catch
                {
                    //  MessageBox.Show(val);
                    return "0000";
                }
            }
        }

        private string ConvertShortToHex1(string val, bool type)
        {

            if (string.IsNullOrEmpty(val))
            {
                //  MessageBox.Show(val);
                return "0000";

            }
            else
            {
                try
                {
                    double val3 = type ? Convert.ToDouble(val) : Convert.ToDouble(val) * 100;
                    return Convert.ToInt32(val3).ToString("X2").PadLeft(4, '0');
                }
                catch
                {
                    //  MessageBox.Show(val);
                    return "0000";
                }
            }
        }

        private void PictureBoxChange(PictureBox pic, Color color)
        {
            try
            {
                pic.Image = new Bitmap(pic.Width, pic.Height);
                Graphics graphics = Graphics.FromImage(pic.Image);
                gp = new GraphicsPath();
                Brush brush = new SolidBrush(color);

                graphics.FillRectangle(brush,
                    new System.Drawing.Rectangle(0, 0, pic.Width, pic.Height));

                gp.AddEllipse(0, 0, pic.Width - 3, pic.Height - 3);

                rg = new Region(gp);
                pic.Region = rg;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------
        #region  SelectedIndexChanged

        private void CmbBxCtrlAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ContainerFilter();
            }
            catch (Exception ae)
            { }

        }

        private void CmbBxRunHaltmode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CmbBx1stout_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ContainerFilter();

            }
            catch (Exception ae) { }
        }

        private void CmbBx2ndout_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ContainerFilter();
            }
            catch (Exception ae) { }
        }

        private void CmbBxLockstatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CmbBxAutotuning_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ContainerFilter();
            }
            catch (Exception ae) { }
        }

        private void CmbBxfraction_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion

        //------------------------------------------------------------------------------------------------------      
        #region Enter
        
        private void CmbBxRunHaltmode_Enter(object sender, EventArgs e)
        {
            runVal = false;
        }

        private void CmbBx1stout_Enter(object sender, EventArgs e)
        {
            out1Val = false;
        }

        private void CmbBx2ndout_Enter(object sender, EventArgs e)
        {
            out2Val = false;
        }

        private void CmbBxLockstatus_Enter(object sender, EventArgs e)
        {

        }

        private void CmbBxSensorType_Enter(object sender, EventArgs e)
        {
            senseVal = false;
        }

        private void CmbBxUnitType_Enter(object sender, EventArgs e)
        {
            unitVal = false;
        }

        private void cmbBxAlarm1Mode_Enter(object sender, EventArgs e)
        {
            alM1Val = false;
        }

        private void cmbBxAlarm2Mode_Enter(object sender, EventArgs e)
        {
            alM2Val = false;
        }

        private void txtBxSetvalue_Enter(object sender, EventArgs e)
        {
            // setVal = false;
        }

        private void txtBxHightemp_Enter(object sender, EventArgs e)
        {
            // highVal = false;


        }

        private void txtBxLowtemp_Enter(object sender, EventArgs e)
        {
            // lowVal = false;

        }

        private void txtBxAlarm1Up_Enter(object sender, EventArgs e)
        {
            alM1UVal = false;
        }

        private void txtBxAlarm1Down_Enter(object sender, EventArgs e)
        {
            alM1DVal = false;
        }

        private void txtBxAlarm2Up_Enter(object sender, EventArgs e)
        {
            alM2UVal = false;
        }

        private void txtBxAlarm2Down_Enter(object sender, EventArgs e)
        {
            alM2DVal = false;
        }

        private void txtHysteresis1_Enter(object sender, EventArgs e)
        {
            // hys1V = false;
        }

        private void txtHysteresis2_Enter(object sender, EventArgs e)
        {
            // hys2V = false;
        }

        private void txtHysteresisDeadBand_Enter(object sender, EventArgs e)
        {
            deadV = false;
        }

        private void txtBxOut1_Enter(object sender, EventArgs e)
        {
            // out1V = false;
        }

        private void txtBxCtrlPeriod1_Enter(object sender, EventArgs e)
        {
            ctrl1V = false;
        }

        private void txtBxOut2_Enter(object sender, EventArgs e)
        {
            // out2V = false;
        }

        private void txtBxCtrlPeriod2_Enter(object sender, EventArgs e)
        {
            ctrl2V = false;
        }

        private void txtBxPD_Enter(object sender, EventArgs e)
        {
            // pdV = false;
        }

        private void txtBxTi_Enter(object sender, EventArgs e)
        {
            tiV = false;
        }

        private void txtBxTd_Enter(object sender, EventArgs e)
        {
            tdV = false;
        }

        private void txtBxCtrlPer1_Enter(object sender, EventArgs e)
        {
            ctrl1V = false;
        }

        private void txtBxctrlPer2_Enter(object sender, EventArgs e)
        {
            ctrl2V = false;
        }

        private void txtBxPCoefficient_Enter(object sender, EventArgs e)
        {
            coefV = false;
        }

        private void txtBxdeadband_Enter(object sender, EventArgs e)
        {
            deadV = false;
        }
        #endregion

        //------------------------------------------------------------------------------------------------------
        #region DropDownClosed
        private void CmbBxCtrlAction_DropDownClosed(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
        }


        private void CmbBxRunHaltmode_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                Resume();
            }
            catch (Exception ae)
            { }
        }

        private void CmbBx1stout_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                Resume();
            }
            catch (Exception ae) { }
        }

        private void CmbBx2ndout_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                Resume();
            }
            catch (Exception ae) { }
        }

        private void CmbBxSensorType_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                Resume();
            }
            catch (Exception ae) { }
        }

        private void CmbBxUnitType_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                Resume();
            }
            catch (Exception ae) { }
        }

        private void cmbBxAlarm1Mode_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                Resume();
            }
            catch (Exception ae) { }
        }

        private void cmbBxAlarm2Mode_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                Resume();
            }
            catch (Exception ae) { }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------       
        #region SelectionChangeCommitted
        private void CmbBxCtrlAction_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                Writedatab = true;
                int x = Convert.ToInt32(CmbBxCtrlAction.SelectedIndex);
                LeaveText("4719", x.ToString("X4").PadLeft(4, '0'));
                this.ActiveControl = null;
                Resume();
                ctrlVal = true;
                Writedatab = false;
            }
            catch (Exception)
            {
            }
        }


        private void CmbBxRunHaltmode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                int x = Convert.ToInt32(CmbBxRunHaltmode.SelectedIndex);
                LeaveText("4723", x.ToString("X4").PadLeft(4, '0'));
                this.ActiveControl = null;
                Resume();
                runVal = true;
            }
            catch (Exception)
            {
            }
        }

        private void CmbBx1stout_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                Thread.Sleep(10);
                int x = Convert.ToInt32(CmbBx1stout.SelectedIndex);
                LeaveText("471A", x.ToString("X4").PadLeft(4, '0'));
                this.ActiveControl = null;
                Resume();
                out1Val = true;
            }
            catch (Exception)
            {
            }
        }

        private void CmbBx2ndout_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                Thread.Sleep(10);
                int x = Convert.ToInt32(CmbBx2ndout.SelectedIndex);
                LeaveText("471F", x.ToString("X4").PadLeft(4, '0'));
                this.ActiveControl = null;
                Resume();
                out2Val = true;
            }
            catch (Exception)
            {
            }
        }

        private void CmbBxSensorType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                // int x = Convert.ToInt32(CmbBxSensorType.SelectedIndex); As per New requirement Create Dictionary For SensorType Data
                var sesval = SensertypeDict.FirstOrDefault(x => x.Key == CmbBxSensorType.SelectedIndex).Value;
                // MessageBox.Show(sesval.ToString());
                LeaveText("4718", sesval.ToString("X4").PadLeft(4, '0'));
                this.ActiveControl = null;
                Resume();
                senseVal = true;
            }
            catch (Exception)
            {
            }
        }

        private void CmbBxUnitType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                int x = Convert.ToInt32(CmbBxUnitType.SelectedIndex);
                LeaveText("4724", x.ToString("X4").PadLeft(4, '0'));
                this.ActiveControl = null;
                Resume();
                unitVal = true;
            }
            catch (Exception)
            {
            }
        }

        private void cmbBxAlarm1Mode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                int x = Convert.ToInt32(cmbBxAlarm1Mode.SelectedIndex);
                if (x >= 0)
                {
                    LeaveText("4720", x.ToString("X4").PadLeft(4, '0'));
                }
                this.ActiveControl = null;
                Resume();
                alM1Val = true;
            }
            catch (Exception)
            {
            }
        }

        private void cmbBxAlarm2Mode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                int x = Convert.ToInt32(cmbBxAlarm2Mode.SelectedIndex);
                if (x >= 0)
                {
                    LeaveText("4721", x.ToString("X4").PadLeft(4, '0'));
                }
                this.ActiveControl = null;
                Resume();
                alM2Val = true;
            }
            catch (Exception)
            {
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------       
        #region KeyDown
        private void txtBxSetvalue_KeyDown(object sender, KeyEventArgs e)
        {
            Writedatab = true;
            setVal = true;
            onchangeeSetvalue = true;
            if (e.KeyCode == Keys.Enter)
            {

                this.ActiveControl = null;

                try
                {
                    if (onchangeeSetvalue)
                    {
                        string valsetval = txtBxSetvalue.Text;
                        string x = ConvertShortToHex(valsetval, false);
                        if (x.Length > 4)
                        {
                            string MinusVal = x.Substring(4, 4);  
                            if (valsetval.ToString().Contains("."))
                            {
                                txtBxSetvalue.Text = valsetval.ToString();
                            }
                            else
                            {
                                txtBxSetvalue.Text = (valsetval.ToString() + ".0");
                            }
                            // Thread.Sleep(10);
                            string aa = (LeaveText("4701", MinusVal)).ToString();
                            //short val1 = Convert.ToInt16(aa, 16);
                            //MessageBox.Show(val2.ToString());
                            //  txtBxSetvalue.Text = val2.ToString();

                            this.ActiveControl = null;
                            Resume();
                            onchangeeSetvalue = false;
                            Writedatab = false;
                        }
                        else
                        {
                            if (valsetval.ToString().Contains("."))
                            {
                                txtBxSetvalue.Text = valsetval.ToString();
                            }
                            else
                            {
                                txtBxSetvalue.Text = (valsetval.ToString() + ".0");
                            }
                             Thread.Sleep(10);
                            string aa = (LeaveText("4701", x)).ToString();
                            //short val1 = Convert.ToInt16(aa, 16);
                            //MessageBox.Show(val2.ToString());
                            //  txtBxSetvalue.Text = val2.ToString();

                            this.ActiveControl = null;
                            Resume();
                            onchangeeSetvalue = false;
                            Writedatab = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //  MessageBox.Show(ex.Message);
                    onchangeeSetvalue = false;
                    Writedatab = false;
                    //Thread.Sleep(10);
                }

            }
        }

        private void txtBxHightemp_KeyDown(object sender, KeyEventArgs e)
        {
            highVal = true;
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
                onchangeeHigh = true;
                try
                {
                    if (onchangeeHigh)
                    {
                        Thread.Sleep(18);
                        //string val = string.IsNullOrEmpty(txtBxHightemp.Text) ? "0" : txtBxHightemp.Text;

                        ////int x = Convert.ToInt32(Convert.ToDouble(SetDecimalPlaces(val)));
                        ////txtBxHightemp.Text = Conversion1(LeaveText("4704", x.ToString("X2").PadLeft(4, '0')));

                        //string x = ConvertShortToHex(val, false);
                        //txtBxHightemp.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4704", x), false));


                        string val = txtBxHightemp.Text;

                        //  txtBxHightemp.Text = (Convert.ToDouble(val)).ToString();

                        string x = ConvertShortToHex(val, false);
                        string aa = (LeaveText("4704", x)).ToString();
                        // txtBxSetvalue.Text = ConvertHexToShortSV((aa), false).ToString();
                        short val1 = Convert.ToInt16(aa, 16);

                        double val2 = Convert.ToDouble(val1) / 10;
                        //MessageBox.Show(val2.ToString());
                        //  txtBxHightemp.Text = val2.ToString();


                        this.ActiveControl = null;
                        Resume();
                        onchangeeHigh = false;
                        highVal = false;
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }

            }
        }

        private void txtBxLowtemp_KeyDown(object sender, KeyEventArgs e)
        {
            lowVal = true;
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
                onchangeeLowTemp = true;

                try
                {
                    if (onchangeeLowTemp)
                    {
                        Thread.Sleep(18);
                        string val = string.IsNullOrEmpty(txtBxLowtemp.Text) ? "0" : txtBxLowtemp.Text;

                        //int x = Convert.ToInt32(Convert.ToDouble(SetDecimalPlaces(val)));
                        //txtBxLowtemp.Text = Conversion1(LeaveText("4705", x.ToString("X2").PadLeft(4, '0')));

                        string x = ConvertShortToHex(val, false);
                        txtBxLowtemp.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4705", x), false));

                        this.ActiveControl = null;
                        Resume();
                        lowVal = false;
                        onchangeeLowTemp = false;
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxAlarm1Up_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    Thread.Sleep(18);
                    string val = string.IsNullOrEmpty(txtBxAlarm1Up.Text) ? "0" : txtBxAlarm1Up.Text;

                    //int x = Convert.ToInt32(Convert.ToDouble(SetDecimalPlaces(val)));
                    //txtBxAlarm1Up.Text = Conversion1(LeaveText("4706", x.ToString("X2").PadLeft(4, '0')));

                    string x = ConvertShortToHex(val, false);
                    txtBxAlarm1Up.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4706", x), false));

                    this.ActiveControl = null;
                    Resume();
                    alM1UVal = true;
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxAlarm1Down_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxAlarm2Up_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxAlarm2Down_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtHysteresis1_KeyDown(object sender, KeyEventArgs e)
        {
            hys1V = true;
            if (e.KeyCode == Keys.Enter)
            {
                onchangeHyst1 = true;
                try
                {
                    if (onchangeHyst1)
                    {
                        Thread.Sleep(18);
                        //string val = string.IsNullOrEmpty(txtHysteresis1.Text) ? "0" : txtHysteresis1.Text;

                        //string x = ConvertShortToHex(val, false);
                        //txtHysteresis1.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4713", x), false));

                        string valHysteresis1 = txtHysteresis1.Text;
                        string x = ConvertShortToHex(valHysteresis1, false);


                        if (txtHysteresis1.ToString().Contains("."))
                        {
                            txtHysteresis1.Text = valHysteresis1.ToString();
                        }
                        else
                        {
                            txtHysteresis1.Text = (valHysteresis1.ToString() + ".0");
                        }
                        string aa = (LeaveText("4713", x)).ToString();


                        // txtHysteresis1.Text = val2.ToString();

                        this.ActiveControl = null;
                        Resume();
                        onchangeHyst1 = false;
                    }
                }
                catch (Exception ex)
                {
                    //  MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtHysteresis2_KeyDown(object sender, KeyEventArgs e)
        {
            hys2V = true;
            if (e.KeyCode == Keys.Enter)
            {
                onchangeHyst2 = true;
                try
                {
                    if (onchangeHyst2)
                    {

                        //string val = string.IsNullOrEmpty(txtHysteresis2.Text) ? "0" : txtHysteresis2.Text;

                        //string x = ConvertShortToHex(val, false);
                        //txtHysteresis2.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4714", x), false));
                        string valHysteresis2 = txtHysteresis2.Text;
                        // txtHysteresis2.Text = (Convert.ToDouble(val)).ToString();
                        string x = ConvertShortToHex(valHysteresis2, false);
                        if (txtHysteresis2.ToString().Contains("."))
                        {
                            txtHysteresis2.Text = valHysteresis2.ToString();
                        }
                        else
                        {
                            txtHysteresis2.Text = (valHysteresis2.ToString() + ".0");
                        }
                        Thread.Sleep(18);
                        string aa = (LeaveText("4714", x)).ToString();
                        //short val1 = Convert.ToInt16(aa, 16);
                        //double val2 = Convert.ToDouble(val1) / 10;
                        // txtHysteresis2.Text = val2.ToString();

                        this.ActiveControl = null;
                        Resume();
                        onchangeHyst2 = false;
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtHysteresisDeadBand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnchangeHysDead = true;
                try
                {
                    if (OnchangeHysDead)
                    {

                        //string val = string.IsNullOrEmpty(txtHysteresisDeadBand.Text) ? "0" : txtHysteresisDeadBand.Text;

                        //string x = ConvertShortToHex(val, false);
                        //txtHysteresisDeadBand.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4712", x), false));

                        string valHysteresisDeadBand = txtHysteresisDeadBand.Text;
                        // txtHysteresisDeadBand.Text = (Convert.ToDouble(val)).ToString();
                        string x = ConvertShortToHex(valHysteresisDeadBand, false);


                        if (txtHysteresisDeadBand.ToString().Contains("."))
                        {
                            txtHysteresisDeadBand.Text = valHysteresisDeadBand.ToString();
                        }
                        else
                        {
                            txtHysteresisDeadBand.Text = (valHysteresisDeadBand.ToString() + ".0");
                        }
                        Thread.Sleep(10);
                        string aa = (LeaveText("4712", x)).ToString();
                        //short val1 = Convert.ToInt16(aa, 16);
                        //double val2 = Convert.ToDouble(val1) / 10;
                        //  txtHysteresisDeadBand.Text = val2.ToString();

                        this.ActiveControl = null;
                        Resume();
                        deadV = true;
                        OnchangeHysDead = false;
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxOut1_KeyDown(object sender, KeyEventArgs e)
        {
            out1V = true;
            if (e.KeyCode == Keys.Enter)
            {
                onchangeOut1 = true;
                try
                {
                    if (onchangeOut1)
                    {
                        //  Thread.Sleep(18);
                        //string val = string.IsNullOrEmpty(txtBxOut1.Text) ? "0" : txtBxOut1.Text;

                        //string x = ConvertShortToHex(val, false);
                        //txtBxOut1.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4702", x), false));

                        string valBxOut1 = txtBxOut1.Text;  //string.IsNullOrEmpty(txtBxSetvalue.Text) ? "0" :
                        //MessageBox.Show(val);

                        if (txtBxOut1.ToString().Contains("."))
                        {
                            txtBxOut1.Text = valBxOut1.ToString();
                        }
                        else
                        {
                            txtBxOut1.Text = (valBxOut1.ToString() + ".0");
                        }

                        Thread.Sleep(10);
                        string x = ConvertShortToHex(valBxOut1, false);
                        string aa = (LeaveText("4702", x)).ToString();
                        // txtBxSetvalue.Text = ConvertHexToShortSV((aa), false).ToString();
                        //short val1 = Convert.ToInt16(aa, 16);

                        //double val2 = Convert.ToDouble(val1) / 10;
                        ////MessageBox.Show(val2.ToString());
                        //txtBxOut1.Text = val2.ToString(CultureInfo.InvariantCulture);

                        this.ActiveControl = null;
                        Resume();
                        out1V = false;
                        onchangeOut1 = false;
                    }
                }
                catch (Exception ex)
                {
                    //   MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxCtrlPeriod1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnchangetxtBxCtrlPeriod1 = true;
                try
                {
                    if (OnchangetxtBxCtrlPeriod1)
                    {
                        //  Thread.Sleep(18);
                        //string val = (txtBxCtrlPeriod1.Text);

                        //string x = ConvertShortToHex(val, true);
                        //txtBxCtrlPeriod1.Text = PlaceDecimal(ConvertHexToShort(LeaveText("471B", x), true));


                        string valBxCtrlPeriod1 = txtBxCtrlPeriod1.Text;

                        // txtBxCtrlPeriod1.Text = (Convert.ToDouble(val)).ToString();

                        if (txtBxCtrlPeriod1.ToString().Contains("."))
                        {
                            txtBxCtrlPeriod1.Text = valBxCtrlPeriod1.ToString();
                        }
                        else
                        {
                            txtBxCtrlPeriod1.Text = (valBxCtrlPeriod1.ToString() + ".0");
                        }

                        Thread.Sleep(18);
                        string x = ConvertShortToHex(valBxCtrlPeriod1, true);
                        string aa = (LeaveText("471B", x)).ToString();

                        // short val1 = Convert.ToInt16(aa, 16);

                        //  double val2 = Convert.ToDouble(val1) / 10;

                        //  txtBxCtrlPeriod1.Text = (val2).ToString();
                        //MessageBox.Show(txtBxCtrlPeriod1.Text);
                        this.ActiveControl = null;
                        Resume();
                        ctrl1V = true;
                        OnchangetxtBxCtrlPeriod1 = false;
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxOut2_KeyDown(object sender, KeyEventArgs e)
        {
            out2V = true;
            if (e.KeyCode == Keys.Enter)
            {
                onchangetxtBxOut2 = true;
                try
                {
                    if (onchangetxtBxOut2)
                    {
                        Thread.Sleep(18);
                        //string val = string.IsNullOrEmpty(txtBxOut2.Text) ? "0" : txtBxOut2.Text;

                        //string x = ConvertShortToHex(val, false);
                        //txtBxOut2.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4703", x), false));

                        string valBxOut2 = txtBxOut2.Text;

                        // txtBxOut2.Text = (Convert.ToDouble(val)).ToString();
                        if (txtBxOut2.ToString().Contains("."))
                        {
                            txtBxOut2.Text = valBxOut2.ToString();
                        }
                        else
                        {
                            txtBxOut2.Text = (valBxOut2.ToString() + ".0");
                        }

                        Thread.Sleep(10);
                        string x = ConvertShortToHex(valBxOut2, false);
                        string aa = (LeaveText("4703", x)).ToString();
                        // txtBxSetvalue.Text = ConvertHexToShortSV((aa), false).ToString();
                        //  short val1 = Convert.ToInt16(aa, 16);

                        //  double val2 = Convert.ToDouble(val1) / 10;
                        //MessageBox.Show(val2.ToString());
                        //  txtBxOut2.Text = val2.ToString();

                        this.ActiveControl = null;
                        Resume();

                        onchangetxtBxOut2 = false;
                    }
                }
                catch (Exception ex)
                {
                    //  MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxCtrlPeriod2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                onchangetxtBxCtrlPeriod2 = true;
                try
                {
                    if (onchangetxtBxCtrlPeriod2)
                    {
                        // Thread.Sleep(18);
                        //string val = string.IsNullOrEmpty(txtBxCtrlPeriod2.Text) ? "0" : txtBxCtrlPeriod2.Text;

                        //string x = ConvertShortToHex(val, true);
                        //txtBxCtrlPeriod2.Text = PlaceDecimal(ConvertHexToShort(LeaveText("471C", x), true));

                        string valtxtBxCtrlPeriod2 = txtBxCtrlPeriod2.Text;

                        // txtBxCtrlPeriod2.Text = (Convert.ToDouble(val)).ToString();

                        if (txtBxCtrlPeriod2.ToString().Contains("."))
                        {
                            txtBxCtrlPeriod2.Text = valtxtBxCtrlPeriod2.ToString();
                        }
                        else
                        {
                            txtBxCtrlPeriod2.Text = (valtxtBxCtrlPeriod2.ToString() + ".0");
                        }
                        Thread.Sleep(10);
                        string x = ConvertShortToHex(valtxtBxCtrlPeriod2, true);
                        string aa = (LeaveText("471C", x)).ToString();

                        //short val1 = Convert.ToInt16(aa, 16);

                        //double val2 = Convert.ToDouble(val1) / 10;

                        // txtBxCtrlPeriod2.Text = val2.ToString();

                        this.ActiveControl = null;
                        Resume();
                        ctrl2V = true;
                        onchangetxtBxCtrlPeriod2 = false;
                    }
                }
                catch (Exception ex)
                {
                    //  MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxPD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                pdV = true;
                try
                {
                    if (pdV)
                    {

                        string valtxtBxPD = txtBxPD.Text;
                        // string x = ConvertShortToHex(val, false);
                        string x = ConvertShortToHex(valtxtBxPD, false);

                        if (valtxtBxPD.ToString().Contains("."))
                        {
                            txtBxPD.Text = valtxtBxPD.ToString();
                        }
                        else
                        {
                            txtBxPD.Text = (valtxtBxPD.ToString() + ".0");
                        }
                        Thread.Sleep(10);
                        string aa = (LeaveText("470C", x)).ToString();
                        // txtBxSetvalue.Text = ConvertHexToShortSV((aa), false).ToString();
                        short val1 = Convert.ToInt16(aa, 16);

                        // double val2 = Convert.ToDouble(val1) / 10;
                        //  txtBxPD.Text = val2.ToString();
                        //  return val2;



                        this.ActiveControl = null;
                        Resume();
                        pdV = false;
                        onchangetxtBxPD = false;
                        //regadd = "470C";
                        //oout2 = SendFrameToDevice1(key, regadd);
                        //o22 = Convert.ToInt32(oout2);
                        //txtBxPD.Text = PlaceDecimal(ConvertHexToShort(o22.ToString(), true));
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }

            }
        }

        private void txtBxTi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                onchangetxtBxTi = true;
                try
                {
                    if (onchangetxtBxTi)
                    {

                        //string val = string.IsNullOrEmpty(txtBxTi.Text) ? "0" : txtBxTi.Text;


                        //string x = ConvertShortToHex(val, true);
                        //txtBxTi.Text = PlaceDecimal(ConvertHexToShort(LeaveText("470E", x), true));

                        string val = txtBxTi.Text;
                        txtBxTi.Text = (Convert.ToDouble(val)).ToString();
                        string x = ConvertShortToHex(val, true);
                        txtBxTi.Text = val.ToString();
                        Thread.Sleep(10);

                        string aa = (LeaveText("470E", x)).ToString();
                        //short val1 = Convert.ToInt16(aa, 16);

                        double val2 = Convert.ToDouble(Convert.ToInt16(aa, 16));

                        this.ActiveControl = null;
                        Resume();
                        tiV = true;
                        onchangetxtBxTi = false;
                    }

                }
                catch (Exception ex)
                {
                    //  MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxTd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                onchangetxtBxTd = true;
                try
                {
                    if (onchangetxtBxTd)
                    {

                        //string val = string.IsNullOrEmpty(txtBxTd.Text) ? "0" : txtBxTd.Text;
                        //string x = ConvertShortToHex(val, true);
                        //txtBxTd.Text = PlaceDecimal(ConvertHexToShort(LeaveText("470D", x), true));

                        string valtxtBxTd = txtBxTd.Text;
                        //  txtBxTd.Text = (Convert.ToDouble(val)).ToString();
                        //if (txtBxTd.ToString().Contains("."))
                        //{
                        txtBxTd.Text = valtxtBxTd.ToString();
                        //}
                        //else
                        //{
                        //    txtBxTd.Text = (valtxtBxTd.ToString() + ".0");
                        //}
                        Thread.Sleep(18);
                        string x = ConvertShortToHex(valtxtBxTd, true);
                        string aa = (LeaveText("470D", x)).ToString();
                        //short val1 = Convert.ToInt16(aa, 16);
                        //double val2 = Convert.ToDouble(val1) / 10;
                        //  txtBxTd.Text = val2.ToString();

                        this.ActiveControl = null;
                        Resume();
                        tdV = true;
                        onchangetxtBxTd = false;
                    }
                }
                catch (Exception ex)
                {
                    //   MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxCtrlPer1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnchangetxtBxCtrlPeriod1 = true;
                try
                {
                    if (OnchangetxtBxCtrlPeriod1)
                    {
                        //Thread.Sleep(18);
                        //string val = string.IsNullOrEmpty(txtBxCtrlPer1.Text) ? "0" : txtBxCtrlPer1.Text;

                        //string x = ConvertShortToHex(val, true);
                        //txtBxCtrlPer1.Text = PlaceDecimal(ConvertHexToShort(LeaveText("471B", x), true));

                        string valtxtBxCtrlPer1 = txtBxCtrlPer1.Text;
                        //   txtBxCtrlPer1.Text = (Convert.ToDouble(val)).ToString();
                        //if (txtBxCtrlPer1.ToString().Contains("."))
                        //{
                        txtBxCtrlPer1.Text = valtxtBxCtrlPer1.ToString();
                        //}
                        //else
                        //{
                        //    txtBxCtrlPer1.Text = (valtxtBxCtrlPer1.ToString() + ".0");
                        //}
                        Thread.Sleep(10);
                        string x = ConvertShortToHex(valtxtBxCtrlPer1, true);
                        string aa = (LeaveText("471B", x)).ToString();
                        // short val1 = Convert.ToInt16(aa, 16);
                        // double val2 = Convert.ToDouble(val1) / 10;
                        //  txtBxCtrlPer1.Text = val2.ToString();

                        this.ActiveControl = null;
                        Resume();
                        ctrl1V = true;
                        OnchangetxtBxCtrlPeriod1 = false;
                    }
                }
                catch (Exception ae)
                { }
            }
        }

        private void txtBxctrlPer2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                onchangetxtBxctrlPer2 = true;
                try
                {
                    if (onchangetxtBxctrlPer2)
                    {
                        Thread.Sleep(18);
                        //string val = string.IsNullOrEmpty(txtBxctrlPer2.Text) ? "0" : txtBxctrlPer2.Text;
                        //string x = ConvertShortToHex(val, true);
                        //txtBxctrlPer2.Text = PlaceDecimal(ConvertHexToShort(LeaveText("471C", x), true));

                        string valtxtBxctrlPer2 = txtBxctrlPer2.Text;
                        // txtBxctrlPer2.Text = (Convert.ToDouble(val)).ToString();
                        //if (txtBxctrlPer2.ToString().Contains("."))
                        //{
                        txtBxctrlPer2.Text = valtxtBxctrlPer2.ToString();
                        //}
                        //else
                        //{
                        //    txtBxctrlPer2.Text = (valtxtBxctrlPer2.ToString() + ".0");
                        //}
                        Thread.Sleep(10);
                        string x = ConvertShortToHex(valtxtBxctrlPer2, true);
                        string aa = (LeaveText("471C", x)).ToString();
                        //  short val1 = Convert.ToInt16(aa, 16);
                        //  double val2 = Convert.ToDouble(val1) / 10;
                        //  txtBxctrlPer2.Text = val2.ToString();

                        this.ActiveControl = null;
                        Resume();
                        ctrl2V = true;
                        onchangetxtBxctrlPer2 = false;
                    }
                }
                catch (Exception ex)
                {
                    //  MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxPCoefficient_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                onchangetxtBxPCoefficient = true;
                try
                {
                    if (onchangetxtBxPCoefficient)
                    {

                        //string val = string.IsNullOrEmpty(txtBxPCoefficient.Text) ? "0" : (Convert.ToDouble(txtBxPCoefficient.Text) * 10).ToString();

                        //string x = ConvertShortToHex(val, false);
                        //txtBxPCoefficient.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4711", x), false) / 10);

                        string valtxtBxPCoefficient = txtBxPCoefficient.Text;
                        //txtBxPCoefficient.Text = (Convert.ToDouble(val)).ToString();
                        if (txtBxPCoefficient.ToString().Contains("."))
                        {
                            txtBxPCoefficient.Text = valtxtBxPCoefficient.ToString();
                        }
                        else
                        {
                            txtBxPCoefficient.Text = (valtxtBxPCoefficient.ToString() + ".00");
                        }
                        Thread.Sleep(18);
                        string x = ConvertShortToHex1(valtxtBxPCoefficient, false);
                        string aa = (LeaveText("4711", x)).ToString();
                        //  short val1 = Convert.ToInt16(aa, 16);
                        // double val2 = Convert.ToDouble(val1) / 10;
                        // txtBxPCoefficient.Text = val2.ToString();

                        this.ActiveControl = null;
                        Resume();
                        coefV = true;
                        onchangetxtBxPCoefficient = false;
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxdeadband_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                onchangetxtBxdeadband = true;
                try
                {
                    if (onchangetxtBxdeadband)
                    {

                        //string val = string.IsNullOrEmpty(txtBxdeadband.Text) ? "0" : txtBxdeadband.Text;

                        //string x = ConvertShortToHex(val, false);
                        //txtBxdeadband.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4712", x), false));

                        string valtxtBxdeadband = txtBxdeadband.Text;
                        if (txtBxdeadband.ToString().Contains("."))
                        {
                            txtBxdeadband.Text = valtxtBxdeadband.ToString();
                        }
                        else
                        {
                            txtBxdeadband.Text = (valtxtBxdeadband.ToString() + ".0");
                        }
                        // txtBxdeadband.Text = (Convert.ToDouble(val)).ToString();
                        Thread.Sleep(18);
                        string x = ConvertShortToHex(valtxtBxdeadband, false);
                        string aa = (LeaveText("4712", x)).ToString();
                        //  short val1 = Convert.ToInt16(aa, 16);
                        //  double val2 = Convert.ToDouble(val1) / 10;
                        //   txtBxdeadband.Text = val2.ToString();

                        this.ActiveControl = null;
                        Resume();
                        deadV = true;
                        onchangetxtBxdeadband = false;
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------       
        #region Leave

        private void txtBxLowtemp_Leave(object sender, EventArgs e)
        {
            lowVal = false;
        }

        private void txtBxHightemp_Leave(object sender, EventArgs e)
        {
            highVal = false;
        }

        private void txtBxSetvalue_Leave(object sender, EventArgs e)
        {

            setVal = false;
        }

        private void txtBxAlarm1Up_Leave(object sender, EventArgs e)
        {

        }

        private void txtBxAlarm1Down_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxAlarm1Down.Text) ? "0" : txtBxAlarm1Down.Text;

                //int x = Convert.ToInt32(Convert.ToDouble(SetDecimalPlaces(val)));
                //txtBxAlarm1Down.Text = Conversion1(LeaveText("4707", x.ToString("X2").PadLeft(4, '0')));

                string x = ConvertShortToHex(val, false);
                txtBxAlarm1Down.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4707", x), false));

                this.ActiveControl = null;
                Resume();
                alM1DVal = true;
            }
            catch (Exception ex)
            {
                //  MessageBox.Show(ex.Message);
            }
        }

        private void txtBxAlarm2Up_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxAlarm2Up.Text) ? "0" : txtBxAlarm2Up.Text;

                //int x = Convert.ToInt32(Convert.ToDouble(SetDecimalPlaces(val)));
                //txtBxAlarm2Up.Text = Conversion1(LeaveText("4708", x.ToString("X2").PadLeft(4, '0')));

                string x = ConvertShortToHex(val, false);
                txtBxAlarm2Up.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4708", x), false));

                this.ActiveControl = null;
                Resume();
                alM2UVal = true;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }

        private void txtBxAlarm2Down_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxAlarm2Down.Text) ? "0" : txtBxAlarm2Down.Text;

                //int x = Convert.ToInt32(Convert.ToDouble(SetDecimalPlaces(val)));
                //txtBxAlarm2Down.Text = Conversion1(LeaveText("4709", x.ToString("X2").PadLeft(4, '0')));

                string x = ConvertShortToHex(val, false);
                txtBxAlarm2Down.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4709", x), false));

                this.ActiveControl = null;
                Resume();
                alM2DVal = true;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }

        private void txtHysteresis1_Leave(object sender, EventArgs e)
        {
            hys1V = false;
        }

        private void txtHysteresis2_Leave(object sender, EventArgs e)
        {
            hys2V = false;
        }

        private void txtHysteresisDeadBand_Leave(object sender, EventArgs e)
        {

        }

        private void txtBxOut1_Leave(object sender, EventArgs e)
        {
            out1V = false;
        }

        private void txtBxCtrlPeriod1_Leave(object sender, EventArgs e)
        {

        }

        private void txtBxOut2_Leave(object sender, EventArgs e)
        {
            out2V = false;
        }

        #endregion

        //------------------------------------------------------------------------------------------------------      
        #region MenuClick
        //Read file XMl to Datatable then After write file data into device 
        private void writeToSelectedDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FilePiclodaer.Visible = true;
                ReadDeviceSettings(1);
                FilePiclodaer.Visible = false;
            }
            catch (Exception ae)
            {
                FilePiclodaer.Visible = false;
                LogWriter.WriteToFile("Exception While ReadDeviceSettings(1)", ae.Message, "DTC_ErrorLog");
            }
        }
        //write file data into device only Selected node address
        private bool WritedataFromExcelSelectedNode(string regAddr, string data)
        {
            try
            {

                string val = SendFrameToDevice2(selectedNode.ToString(), "06", regAddr, data);

                if (val != null || val != "")
                {
                    // Resume();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Write XML Selected Node Function, WritedataFromXMlSelectedNode()", ae.Message, "DTC_ErrorLog");
                return false;

            }
        }

        //write file data into device only Connected node address
        private bool WritedataFromExcelConnectedNode(string regAddr, string data, int nodeId)
        {
            try
            {

                string val = SendFrameToDevice2(nodeId.ToString(), "06", regAddr, data);

                if (val != null || val != "")
                {
                    // Resume();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Write MoniterDatatable Data Connected Node Function, WritedataFromExcelConnectedNode()", ae.Message, "DTC_ErrorLog");
                return false;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }


        //Connected Device Write Data Functionality
        private void writeToConnectedDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ReadDeviceSettings(2);
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Exception While ReadDeviceSettings(2)", ae.Message, "DTC_ErrorLog");
            }
        }
        #endregion

        private void CmbBxAutotuning_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = null;
                Resume();
            }
            catch (Exception ae)
            {

            }
        }

        private void CmbBxAutotuning_Enter(object sender, EventArgs e)
        {
            autoTune = false;
        }

        private void CmbBxAutotuning_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                autoTune = false;

                int x = Convert.ToInt32(CmbBxAutotuning.SelectedIndex);
                LeaveText("4727", x.ToString("X4").PadLeft(4, '0'));
                // Thread.Sleep(300);
                this.ActiveControl = null;
                Resume();
                ATT = CmbBxAutotuning.SelectedIndex;
                autoTune = true;
            }
            catch (Exception)
            {
            }
        }



        private void txtPDOffset_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
                onchangePDoffset = true;
                try
                {
                    if (onchangePDoffset)
                    {
                        string val = string.IsNullOrEmpty(txtPDOffset.Text) ? "0" : txtPDOffset.Text;
                        string x = ConvertShortToHex(val, false);
                        //  txtBxIoffset.Text = PlaceDecimal(ConvertHexToShort(LeaveText("100D", x), false));

                        this.ActiveControl = null;
                        Resume();
                        onchangePDoffset = false;
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
            }
        }

        private void menuPropertySettingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void txtBxSetvalue_TextChanged(object sender, EventArgs e)
        {
            onchangeIoffset = true;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }

        private void vIEWToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void txtBxIoffset_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
                onchangeIoffset = true;
                try
                {
                    if (onchangeIoffset)
                    {
                        string val = string.IsNullOrEmpty(txtBxIoffset.Text) ? "0" : txtBxIoffset.Text;

                        string x = ConvertShortToHex(val, false);
                        //   txtBxIoffset.Text = PlaceDecimal(ConvertHexToShort(LeaveText("100C", x), false));

                        this.ActiveControl = null;
                        Resume();
                        onchangeIoffset = false;
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtBxPVOffset_TextChanged(object sender, EventArgs e)
        {




        }

        private void txtBxHightemp_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBxPVOffset_Enter(object sender, EventArgs e)
        {

            pvOffset = false;


        }

        private void txtBxPVOffset_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                PVV = true;
                // System.Threading.Thread.Sleep(4000);
                if (e.KeyCode == Keys.Enter)
                {

                    this.ActiveControl = null;

                    // int sample = Convert.ToInt32(txtBxPVOffset.Text);
                    // string val = string.IsNullOrEmpty(
                    //? "0" : txtBxPVOffset.Text;

                    //  string x = ConvertShortToHex1(sample, false);
                    //   txtBxPVOffset.Text = PlaceDecimal(ConvertHexToShort(LeaveText("471D", x), false));

                    string val = txtBxPVOffset.Text;  //string.IsNullOrEmpty(txtBxSetvalue.Text) ? "0" :
                    //MessageBox.Show(val);
                    ///   txtBxPVOffset.Text = (Convert.ToDouble(val)).ToString();

                    string x = ConvertShortToHex(val, false);
                    string aa = (LeaveText("471D", x)).ToString();
                    // txtBxSetvalue.Text = ConvertHexToShortSV((aa), false).ToString();
                    short val1 = Convert.ToInt16(aa, 16);

                    double val2 = Convert.ToDouble(val1) / 10;
                    //MessageBox.Show(val2.ToString());
                    //  txtBxPVOffset.Text = val2.ToString();

                    this.ActiveControl = null;

                    Resume();
                    pvOffset = true;

                    PVV = false;
                }
            }
            catch (Exception ae)
            {

            }

        }

        private void txtBxPVOffset_Leave(object sender, EventArgs e)
        {
            PVV = false;
        }

        private void eXITToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (modbusobj != null && modbusobj.IsSerialPortOpen())
                {
                    modbusobj.CloseSerialPort();
                }

                this.Close();
                mainThread.Abort();
            }
            catch (Exception ae)
            { }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = string.Empty;
                OpenFileDialog openFileDialog1 = new OpenFileDialog
                {
                    Title = "Browse xlsx Files",

                    CheckFileExists = true,
                    CheckPathExists = true,

                    DefaultExt = "xlsx",
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FilterIndex = 2,
                    RestoreDirectory = true,

                    ReadOnlyChecked = true,
                    ShowReadOnly = true
                };

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //  Moniterloder.Visible = true;
                    filePath = openFileDialog1.FileName;
                }

                if (MoniterAllData != null)
                {
                    if (MoniterAllData.Rows.Count > 0)
                    {
                        MoniterAllData = null;
                    }
                }
                // Open the Excel file using ClosedXML.
                // Keep in mind the Excel file cannot be open when trying to read it
                using (XLWorkbook workBook = new XLWorkbook(filePath))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);
                    DataTable dts = new DataTable();
                    //Create a new DataTable.

                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        //Use the first row to add columns to DataTable.
                        if (firstRow)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                dts.Columns.Add(cell.Value.ToString());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            //Add rows to DataTable.
                            dts.Rows.Add();
                            int i = 0;
                            foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                            {
                                dts.Rows[dts.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }
                        }
                    }
                    MoniterAllData = dts;
                    MessageBox.Show("Excel File data Write into Buffer:");
                    writeToSelectedDeviceToolStripMenuItem.Enabled = true;
                    writeToConnectedDevicesToolStripMenuItem.Enabled = true;
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("openToolStripMenuItem_Click() Excel File to datatable", ae.Message, "DTC_ErrorLog");
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MoniterAllData = CreateDynamicDataTable();
                if (!File.Exists(pathdExcelata))
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(MoniterAllData, "Sheet1");
                        wb.SaveAs(pathdExcelata + ".xlsx");
                    }
                    MessageBox.Show("All Save Parameter to file done");
                }
                else
                {
                    File.Delete(Path.GetFullPath(pathdExcelata));
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(MoniterAllData, "Sheet1");
                        wb.SaveAs(pathdExcelata + ".xlsx");
                    }
                   MessageBox.Show("All Save Parameter to file done");
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("saveToolStripMenuItem_Click() While datatable to Excel Write data", ae.Message, "DTC_ErrorLog");
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MoniterAllData.Rows.Count > 0)
                {

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        pathdExcelata = saveFileDialog1.FileName;
                    }
                    if (!File.Exists(pathdExcelata))
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            wb.Worksheets.Add(MoniterAllData, "Sheet1");
                            wb.SaveAs(pathdExcelata + ".xlsx");
                        }
                        MessageBox.Show("All Save Parameter to file done");
                        saveToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            wb.Worksheets.Add(MoniterAllData, "Sheet1");
                            wb.SaveAs(pathdExcelata + ".xlsx");
                        }
                        MessageBox.Show("All Save Parameter to file done");
                        saveToolStripMenuItem.Enabled = true;
                    }
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("saveAsToolStripMenuItem_Click() While datatable to Excel Write data", ae.Message, "DTC_ErrorLog");
            }
        }

        private void cmbBxSetvalue_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                UserControl1 ctrl = usrList.Find(x => x.SelectedNode);
                int index = cmbBxSetvalue.SelectedIndex;
                //pattern step
                //SV
                //Remaining time
                switch (index)
                {
                    case 0:
                        ctrl.PatternStepBool = true;
                        ctrl.RemainTimeBool = false;
                        ctrl.SvBool = false;
                        break;
                    case 1:
                        ctrl.PatternStepBool = false;
                        ctrl.RemainTimeBool = false;
                        ctrl.SvBool = true;
                        break;
                    case 2:
                        ctrl.PatternStepBool = false;
                        ctrl.RemainTimeBool = true;
                        ctrl.SvBool = false;
                        break;
                }
            }
            catch (Exception ae) { }
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                MoniterAllData = CreateDynamicDataTable();
                if (MoniterAllData.Rows.Count > 0)
                {
                    saveAsToolStripMenuItem.Enabled = true;
                    openToolStripMenuItem.Enabled = true;
                    writeToSelectedDeviceToolStripMenuItem.Enabled = true;
                    writeToConnectedDevicesToolStripMenuItem.Enabled = true;
                    MessageBox.Show("All parameter successFul upload into Buffer:");
                }

            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Write Datatable  Data Connected Node Data DataTable to Send Query ReadDeviceSettings(), ", ae.Message, "DTC_ErrorLog");
                MessageBox.Show(ae.ToString());
            }
        }

        // All Selected And Connected Node Write Data Functionality Work here
        private void ReadDeviceSettings(int nodedata)
        {
            try
            {
                if (nodedata == 1)
                {
                    try
                    {
                        //selected Node data
                        {
                            if (MoniterAllData.Rows.Count > 0)
                            {
                                DataRow[] drow = MoniterAllData.Select();
                                foreach (DataRow prjdatainfo in drow)
                                {
                                    OLK = 0;
                                    string strAdress = Convert.ToString(prjdatainfo["Address"]);
                                    string strRegiData = Convert.ToString(prjdatainfo["RegisterData"]);
                                    //  MessageBox.Show(strAdress +"   "+ strRegiData);
                                    //Tests if any write Value
                                    if (strRegiData.Length == 1)
                                     {
                                         strRegiData = "000" + strRegiData;
                                    }
                                    else if (strRegiData.Length == 2)
                                    {
                                        strRegiData = "00" + strRegiData;
                                    }
                                    else if (strRegiData.Length == 3)
                                    {
                                        strRegiData = "0" + strRegiData;
                                    }
                                    bool wrdata = WritedataFromExcelSelectedNode(strAdress, strRegiData);
                                    if (wrdata)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            MessageBox.Show("File Data Write into Device SuccessFully.");
                        }
                    }
                    catch (Exception ae)
                    {
                        LogWriter.WriteToFile("Write Datatable Data Connected Node Data DataTable to Send Query ReadDeviceSettings(), ", ae.Message, "DTC_ErrorLog");
                    }
                }
                if (nodedata == 2)
                {
                    try
                    {
                        if (selectedNodeList != null)
                        {
                            for (int k = 1; k <= selectedNodeList.Count; k++)
                            {

                                if (k == selectedNode)
                                    continue;
                                if (MoniterAllData.Rows.Count > 0)
                                {
                                    DataRow[] drow = MoniterAllData.Select();
                                    foreach (DataRow prjdatainfo in drow)
                                    {
                                        string strAdress = Convert.ToString(prjdatainfo["Address"]);
                                        string strRegiData = Convert.ToString(prjdatainfo["RegisterData"]);
                                        if (strRegiData.Length == 1)
                                        {
                                            strRegiData = "000" + strRegiData;
                                        }
                                        else if (strRegiData.Length == 2)
                                        {
                                            strRegiData = "00" + strRegiData;
                                        }
                                        else if (strRegiData.Length == 3)
                                        {
                                            strRegiData = "0" + strRegiData;
                                        }
                                        bool wrdata = WritedataFromExcelConnectedNode(strAdress, strRegiData, k);
                                        if (wrdata)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            break;
                                        }

                                    }
                                }
                            }
                        }
                        MessageBox.Show("File Data Write into Device SuccessFully.");
                    }
                    catch (Exception ae)
                    {

                        LogWriter.WriteToFile("Write Datatable  Data Connected Node Data DataTable to Send Query ReadDeviceSettings(), ", ae.Message, "DTC_ErrorLog");
                    }
                }

            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("ReadDeviceSettings()", ae.Message, "DTC_ErrorLog");
            }
        }


        public string SendFrameToDevice1(string key, string addres)
        {
            string bb = string.Empty;
            string result = "";
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
                    //   if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength))
                    {
                        byte[] RecieveData = null;

                        if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                        {
                            if ((SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1) && SetValues.Set_CommType == 1)
                            {
                                RecieveData = modbusobj.AscFrame(key, "03",
                                    addres, "0001");
                            }
                            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                            {
                                RecieveData = modbusobj.RtuFrame(key, "03",
                                   addres, "0001", baudRate);
                            }

                            if (RecieveData != null && RecieveData.Length > 0)
                            {
                                char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();

                                //  MessageBox.Show(recdata.ToString());

                                if ((SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1) && SetValues.Set_CommType == 1)
                                {
                                    result = string.Join("", recdata);
                                    result = result.Substring(result.Length - 8, 4);
                                }
                                else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                                {
                                    result = modbusobj.DisplayFrame(RecieveData);
                                    //result = result.Substring(result.Length - 6, 4);

                                    try
                                    {
                                        char[] aa = result.ToArray();

                                        result = (aa[9] + "" + aa[10] + "" + aa[12] + "" + aa[13]).ToString();   //
                                    }
                                    catch (Exception ae)
                                    {

                                    }
                                }


                            }

                        }

                    }


                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.StackTrace);
                    //LogWriter.WriteToFile("ModbusRTU: 5)", ex.Message, "DTC_ErrorLog");
                    //lblMessage.Text = "3" + ex.Message;

                    return result;
                }
                finally
                {
                    //if (modbusobj.IsSerialPortOpen())
                    //{
                    //    modbusobj.CloseSerialPort();
                    //}
                }
            }
            else
            {
                MessageBox.Show("Setting is Empty");
            }
            return result;

        }



        public string SendFrameToDevice2(string key, string cmdd, string reg, string data)
        {
            string result = string.Empty;
            int GotoCount = 0;
            string portName = SetValues.Set_PortName;
            string baudRate = SetValues.Set_Baudrate;
            string parity = SetValues.Set_parity;
            int bitsLength = SetValues.Set_BitsLength;
            string stopBits = SetValues.Set_StopBits;
        startagain:
            if (!string.IsNullOrEmpty(portName) && !string.IsNullOrEmpty(baudRate) && !string.IsNullOrEmpty(parity) &&
            bitsLength > 0 && !string.IsNullOrEmpty(stopBits))
            {
                try
                {
                    byte[] RecieveData = null;

                    if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                    {
                        if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1)
                        {
                            RecieveData = modbusobj.AscFrame(key, cmdd,
                                reg, data);
                        }
                        else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                        {
                            RecieveData = modbusobj.RtuFrame(key, cmdd,
                                reg, data, baudRate);
                        }

                        if (RecieveData != null && RecieveData.Length > 0)
                        {
                            char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();

                            if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1)
                            {
                                result = string.Join("", recdata);
                                //  MessageBox.Show(result);
                                // result = result.Substring(result.Length - 8, 4);
                                char[] aa = result.ToArray();

                                result = (aa[9] + "" + aa[10] + "" + aa[11] + "" + aa[12]).ToString();
                                //   MessageBox.Show(result);
                                return result;

                            }
                            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                            {
                                result = modbusobj.DisplayFrame(RecieveData);
                                //   MessageBox.Show(result);
                                char[] aa = result.ToArray();

                                result = (aa[12] + "" + aa[13] + "" + aa[15] + "" + aa[16]).ToString();
                                return result;

                            }

                            // return result;
                        }
                        else
                        {
                            GotoCount++;
                            if (GotoCount > 10)
                            {
                                // MessageBox.Show( "Result Not found .Invalid value");
                                // return "Result Not found .Invalid SV value";
                            }
                            else
                                goto startagain;
                            //SendFrameToDevice2(key, cmdd, reg, data);

                        }

                    }
                    else
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                     MessageBox.Show(ex.Message);
                   // MessageBox.Show(ex.Data);
                     //MessageBox.Show(result);
                    //  MessageBox.Show("Result Not found .Invalid value");
                    return result;
                }
                finally
                {
                    //if (modbusobj.IsSerialPortOpen())
                    //{
                    //    modbusobj.CloseSerialPort();
                    //}
                }
            }
            else
            {
                // settings are empty
            }
            return result;

        }

        private DataTable CreateDynamicDataTable()
        {
            try
            {

                DataTable stdTable = new DataTable("DTCAddressData");
                DataColumn col1 = new DataColumn("Address");
                DataColumn col2 = new DataColumn("RegisterData");
                DataColumn col3 = new DataColumn("Decription");
                stdTable.Columns.Add(col1);
                stdTable.Columns.Add(col2);
                stdTable.Columns.Add(col3);

                //Add Moniter Register Data to the table  
                DataRow newRow; newRow = stdTable.NewRow();

                newRow["Address"] = "0811";
                newRow["RegisterData"] = myStruct.unit;
                newRow["Decription"] = "Temperature unit display selection";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "0813";
                newRow["RegisterData"] = myStruct.autoTune;
                newRow["Decription"] = "Read/write auto-tuning status";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();
            
                newRow["Address"] = "1000";
                newRow["RegisterData"] = myStruct.pv;
                newRow["Decription"] = "Present temperature value(PV)";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();
              
                newRow["Address"] = "1001";
                newRow["RegisterData"] = myStruct.sv;
                newRow["Decription"] = "Set point (SV)";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();
          
                newRow["Address"] = "1002";
                newRow["RegisterData"] = myStruct.highTemp;
                newRow["Decription"] = "Upper-limit of temperature range";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1003";
                newRow["RegisterData"] = myStruct.lowTemp;
                newRow["Decription"] = "Lower-limit of temperature range";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1004";
                newRow["RegisterData"] = myStruct.sensorType;
                newRow["Decription"] = "Input temperature sensor type";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1005";
                newRow["RegisterData"] = myStruct.ctrlAction;
                newRow["Decription"] = "Control method";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1007";
                newRow["RegisterData"] = myStruct.ctrlPeriod1;
                newRow["Decription"] = "1st group of Heating / Cooling control cycle";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1008";
                newRow["RegisterData"] = myStruct.ctrlPeriod2;
                newRow["Decription"] = "2st group of Heating / Cooling control cycle";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1009";
                newRow["RegisterData"] = myStruct.pb;
                newRow["Decription"] = "PB : Proportional band value";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "100A";
                newRow["RegisterData"] = myStruct.td;
                newRow["Decription"] = "Ti: Integral time";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "100B";
                newRow["RegisterData"] = myStruct.ti;
                newRow["Decription"] = "Td :Derivative time ";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "100C";
                newRow["RegisterData"] = myStruct.iOffset;
                newRow["Decription"] = "Integration default(I-Offset)";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "100D";
                newRow["RegisterData"] = myStruct.pdoffset;
                newRow["Decription"] = "Proportional control offset error value";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();
        
                newRow["Address"] = "100E";
                newRow["RegisterData"] = myStruct.coef;
                newRow["Decription"] = "COEF setting when Dual Loop output control are used";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "100F";
                newRow["RegisterData"] = myStruct.dead1;
                newRow["Decription"] = "Dead band setting when Dual Loop output control are used";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1010";
                newRow["RegisterData"] = myStruct.hys1;
                newRow["Decription"] = "Hysteresis setting value of the 1st output group";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1011";
                newRow["RegisterData"] = myStruct.hys2;
                newRow["Decription"] = "Hysteresis setting value of the 2nd output group";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1012";
                newRow["RegisterData"] = myStruct.out1;
                newRow["Decription"] = "Output value read and write of Output 1";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1013";
                newRow["RegisterData"] = myStruct.out2;
                newRow["Decription"] = "Output value read and write of Output 2";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1016";
                newRow["RegisterData"] = myStruct.pvOffset;
                newRow["Decription"] = "Temperature regulation value";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1020";
                newRow["RegisterData"] = myStruct.alarm1Mode;
                newRow["Decription"] = "Alarm 1 type";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                //newRow["Address"] = "1020";
                //newRow["RegisterData"] = myStruct.alarm1Mode;
                //newRow["Decription"] = "Alarm 1 type";
                //stdTable.Rows.Add(newRow);
                //newRow = stdTable.NewRow();

                newRow["Address"] = "1021";
                newRow["RegisterData"] = myStruct.alarm2Mode;
                newRow["Decription"] = "Alarm 2 type";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();
                
                newRow["Address"] = "1024";
                newRow["RegisterData"] = myStruct.alarm1Up;
                newRow["Decription"] = "Upper-limit alarm 1 AL1H";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1025";
                newRow["RegisterData"] = myStruct.alarm1Down;
                newRow["Decription"] = "Lower-limit alarm 1 AL1L";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1026";
                newRow["RegisterData"] = myStruct.alarm2Up;
                newRow["Decription"] = "Upper-limit alarm 2 AL2H";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1027";
                newRow["RegisterData"] = myStruct.alarm2Down;
                newRow["Decription"] = "Lower-limit alarm 2 AL2L";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "102A";
                newRow["RegisterData"] = myStruct.LEDdata;
                newRow["Decription"] = "Read/Write Status";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "102F";
                newRow["RegisterData"] = myStruct.swVersion;
                newRow["Decription"] = "Software version";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1032";
                newRow["RegisterData"] = myStruct.RemTime;
                newRow["Decription"] = "Remaining time of read execution";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1068";
                newRow["RegisterData"] = myStruct.run;
                newRow["Decription"] = "Control execution / Stop setting";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "1069";
                newRow["RegisterData"] = myStruct.out1cmb;
                newRow["Decription"] = "Output 1: control selection";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "106A";
                newRow["RegisterData"] = myStruct.out2cmb;
                newRow["Decription"] = "Output2: control selection";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

                newRow["Address"] = "106E";
                newRow["RegisterData"] = myStruct.lockStatus;
                newRow["Decription"] = "lockStatus";
                stdTable.Rows.Add(newRow);
                newRow = stdTable.NewRow();

           
                //    ds.AcceptChanges();
                return stdTable;
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("CreateDynamicDataTable() Create Datatable for File upload Data", ae.Message, "DTC_ErrorLog");
                return null;
            }

        }

        private void CreateAndAddAnnotation(double a, double vall)
        {
            #region VerticalAnno
            VerticalLineAnnotation v1 = new VerticalLineAnnotation();
            v1.AxisX = cpuChart.ChartAreas[0].AxisX;
            v1.LineColor = Color.White;
            v1.LineDashStyle = ChartDashStyle.Dash;
            v1.LineWidth = 1;
            v1.AllowMoving = false;
            v1.AllowSelecting = false;
            v1.AllowResizing = false;
            v1.X = vall;
            v1.IsInfinitive = true;

            #region Comment code
            // RectangleAnnotation r1 = new RectangleAnnotation();
            // r1.AxisX = cpuChart.ChartAreas[0].AxisX;
            // r1.IsSizeAlwaysRelative = false;
            // r1.Width = 20;  // 20 test Dtc
            // r1.Height = 0.8;

            //r1.BackColor = Color.Red;
            // r1.LineColor = Color.Red;
            // r1.AxisY = cpuChart.ChartAreas[0].AxisY;

            // string time = System.DateTime.Now.ToLongTimeString();
            // r1.Text = time;

            // r1.ForeColor = Color.White;
            // r1.Font = new System.Drawing.Font("Arial", 8f);
            // r1.Y = -r1.Height;
            // r1.X = v1.X - r1.Width / 2;
            #endregion

            TextAnnotation textobj = new TextAnnotation();
            textobj.Text = a.ToString();
            textobj.AxisX = cpuChart.ChartAreas[0].AxisX;
            textobj.X = v1.X;
            textobj.AxisY = cpuChart.ChartAreas[0].AxisY;
            textobj.Font = new System.Drawing.Font("Arial", 8f);
            textobj.ForeColor = Color.White;
            textobj.Y = a;


            cpuChart.Invoke((Action)(() =>
            {
                cpuChart.Annotations.Add(v1);
                // cpuChart.Annotations.Add(r2);
                cpuChart.Annotations.Add(textobj);
                // cpuChart.Annotations.Add(annotation);
            }));

            

            #endregion
        }

        private void CreateChart()
        {
            try
            {
                int index = 0;

                cpuChart.ChartAreas[index].BackColor = Color.Black;

                ////chart title  
                //cpuChart.Titles.Add("Chart Name");

                /// enable autoscroll
                this.cpuChart.ChartAreas[index].CursorX.AutoScroll = true;

                ////X-axis-------------------------------------------------
                /// X-axis line color
                this.cpuChart.ChartAreas[index].AxisX.LineColor = Color.White;
                /// | vertical lines color
                this.cpuChart.ChartAreas[index].AxisX.MajorGrid.LineColor = Color.DarkRed;
                /// X-axis label color
                this.cpuChart.ChartAreas[index].AxisX.LabelStyle.ForeColor = Color.Blue;
                /// disable X-axis vertical lines
                cpuChart.ChartAreas[index].AxisX.Enabled = AxisEnabled.False;

                ////Y-axis-------------------------------------------------
                /// Y-axis line color
                this.cpuChart.ChartAreas[index].AxisY.LineColor = Color.White;
                /// -- horizontal lines color
                this.cpuChart.ChartAreas[index].AxisY.MajorGrid.LineColor = Color.Red;
                /// Y-axis label color
                this.cpuChart.ChartAreas[index].AxisY.LabelStyle.ForeColor = Color.Green;
                /// disable Y-axis vertical lines
                cpuChart.ChartAreas[index].AxisY.Enabled = AxisEnabled.True;

                //cpuChart.ChartAreas[index].AxisX.Interval = 10;
                //cpuChart.ChartAreas[index].AxisY.Interval = 10;

                /// y axis min and max points on scale
                //cpuChart.ChartAreas[index].AxisY.Minimum = 0;
                //cpuChart.ChartAreas[index].AxisY.Maximum = 32;

                cpuChart.ChartAreas[index].AxisX.Minimum = 0;
                cpuChart.ChartAreas[index].AxisX.Maximum = 500;


                //cpuChart.ChartAreas[index].AxisX.ScaleView.Size = 1;
                //cpuChart.ChartAreas[index].AxisY.ScaleView.Size = 1;
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Timer2Form, CreateChart() )", ae.Message, "DTC_ErrorLog");
            }
        }

        private void CreateDefaultChartSeries()
        {
            try
            {
                Random randomNumber = new Random();

                cpuChart.Series.Clear();
                var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Series1",
                    Color = System.Drawing.Color.White,
                    IsVisibleInLegend = false,
                    IsXValueIndexed = true,
                    ChartType = SeriesChartType.Spline,
                    BorderWidth = 2
                };

                this.cpuChart.Series.Add(series1);


                series1.Points.AddXY(0, randomNumber.Next(0, 25));
                cpuChart.Invalidate();
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile(" Timer2Form, CreateDefaultChartSeries()", ae.Message, "DTC_ErrorLog");
            }
        }

        private bool? SendFrameToDevicepv(string nodeAddress, string functionCode, string regAddress, string wordCount, ref double retValue)
        {
            List<string> returnValues = null;

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
                    if (!modbusobj.IsSerialPortOpen())
                    {
                        modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength);
                    }

                    if (modbusobj.IsSerialPortOpen())
                    {
                        byte[] RecieveData = null;

                        if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                        {
                            #region ASCII

                            if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1)
                            {

                                RecieveData = modbusobj.AscFrame(Convert.ToInt32(nodeAddress).ToString("X").PadLeft(2, '0'),
                                    Convert.ToInt32(functionCode).ToString("X").PadLeft(2, '0'), regAddress, wordCount);

                                if (functionCode == "03")
                                {
                                    if (RecieveData != null && RecieveData.Length > 0)
                                    {
                                        //get no. of bytes to read from recieved frame
                                        byte[] sizeBytes = ExtractByteArray(RecieveData, 2, 5);

                                        int size = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(sizeBytes), 16);

                                        if (RecieveData.Length > size)
                                        {
                                            byte[] newArr = ExtractByteArray(RecieveData, size * 2, 7);

                                            returnValues = new List<string>();
                                            int count = 0;

                                            for (int i = 0; i < size; i = i + 2)
                                            {
                                                byte[] bytes = ExtractByteArray(newArr, 4, count);

                                                string byteArrayToString = System.Text.Encoding.UTF8.GetString(bytes);

                                                returnValues.Add(byteArrayToString);
                                                count = count + 4;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region RTU
                            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                            {
                                RecieveData = modbusobj.RtuFrame(nodeAddress, functionCode, regAddress, wordCount, SetValues.Set_Baudrate);

                                if (functionCode == "03")
                                {
                                    if (RecieveData != null && RecieveData.Length > 0)
                                    {
                                        //01 03 02 02 FF F9 64 
                                        int size = Convert.ToInt32(RecieveData[2]);

                                        if (RecieveData.Length > (size + 3))
                                        {
                                            byte[] newArr = new byte[size];
                                            Array.Copy(RecieveData, 3, newArr, 0, size);

                                            returnValues = new List<string>();
                                            int count = 0;

                                            for (int i = 0; i < size; i = i + 2)
                                            {
                                                byte[] bytes = new byte[2];

                                                Array.Copy(newArr, count, bytes, 0, 2);

                                                string byteArrayToString = clsModbus.ByteArrayToString(bytes);

                                                returnValues.Add(byteArrayToString);
                                                count = count + 2;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception ex)
                {
                    //LogWriter.WriteToFile("ModbusRTU: 5)", ex.Message, "DTC_ErrorLog");
                    //lblMessage.Text = "3" + ex.Message;
                    return null;
                }
            }
            else
            {
                // settings are empty
            }

            if (returnValues != null)
            {
                retValue = ConvertHexToShort(returnValues[0], false);
                // MessageBox.Show(retValue.ToString());
                return true;
            }
            else
            {
                return false;
            }
        }

        public void annotClear()
        {
           
        }

        private void DisplayToChart2(string seriesName, Dictionary<double, double> list, string nodeaddressval)
        {
            Dictionary<int, double> UpdatePV = new Dictionary<int, double>();
           
            try
            {
                UpdatePV.Clear();

                Dictionary<double, double>.ValueCollection values = list.Values;
                for (int i = 1; i < list.Count; i++)
                {
                    foreach (double val in values)
                    {
                        int cn = i++;
                        {
                            UpdatePV.Add(cn, val);
                        }
                        
                    }
                }
              
                double counterMain = 1;
                double pointValue = 0;
                string nodeAddress = nodeaddressval;//"1";
                if (!string.IsNullOrEmpty(nodeAddress))
                {
                    // 1: connect USB/Serial
                    // 2: send query
                    // 3: decode query
                   
                    bool? valRe = SendFrameToDevicepv(nodeAddress, "03", "4700", "0001", ref pointValue);
                    {
                        double chartYMax = cpuChart.ChartAreas[0].AxisY.Maximum;
                        double chartYMin = cpuChart.ChartAreas[0].AxisY.Minimum;

                        double chartXMax = cpuChart.ChartAreas[0].AxisX.Maximum;
                        double chartXMin = cpuChart.ChartAreas[0].AxisX.Minimum;

                        #region AdjustMaxScale
                        if (pointValue > chartYMax)
                        {
                            double newValue = pointValue + 20; //10
                            cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisY.Maximum = newValue; }));

                        }
                        if (annotationCounter > chartXMax)
                        {
                            if (annotationCounter > 540)
                            {
                                int diff = annotationCounter - 540;
                                cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisX.Minimum = diff; }));
                                double newValue = annotationCounter + 20;
                                cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisX.Maximum = newValue; }));
                            }
                            else
                            {
                                double newValue = annotationCounter + 20;
                                cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisX.Maximum = newValue; }));
                                // pvgraphresize = true;
                                cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisX.Minimum = 20; }));
                            }
                        }
                        #endregion

                        #region AdjustMinScale
                        if (pointValue < chartYMin)  //Bhushan Comment As per DTC SW Change
                        {
                            double newValue = pointValue - 20; //10
                            cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisY.Minimum = newValue; }));

                        }
                        //if (pointValue < chartXMin)
                        //{
                        //    cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisX.Minimum = 0; })); //pointValue bhushan Tested
                        //}
                        #endregion



                       
                    }

                }
                annotationCounter++;

                if (cpuChart.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateCpuchart(UpdatePV, annotationCounter, pointValue); });    //objListMain
                   // Thread.Sleep(1000);
                }
                else
                {

                }

                Thread.Sleep(50);

                counterMain++;


            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile(" Timer2Form, DisplayToChart2() )", ex.Message, "DTC_ErrorLog");
                //MessageBox.Show("Error occurred: " + ex.Message);
            }
        }

        private void UpdateCpuchart(Dictionary<int, double> Dict, int annotationCounterD,double pp) //GraphPointsList objList
        {
            VerticalLineAnnotation v1 = new VerticalLineAnnotation();
            TextAnnotation textobj = new TextAnnotation();

            try
            {
                  ClearAllSeriesPoints();
                  #region Comment need to check
                  //cpuChart.Annotations.Clear();
                  //if (Dict.Count % 60 == 0)
                  //{


                  //    v1.AxisX = cpuChart.ChartAreas[0].AxisX;
                  //    v1.LineColor = Color.White;
                  //    v1.LineDashStyle = ChartDashStyle.Dash;
                  //    v1.LineWidth = 1;
                  //    v1.AllowMoving = false;
                  //    v1.AllowSelecting = false;
                  //    v1.AllowResizing = false;
                  //    v1.X = annotationCounter;
                  //    v1.IsInfinitive = true;


                  //    textobj.Text = "60";//.ToString();
                  //    textobj.AxisX = cpuChart.ChartAreas[0].AxisX;
                  //    textobj.X = v1.X;
                  //    textobj.AxisY = cpuChart.ChartAreas[0].AxisY;
                  //    textobj.Font = new System.Drawing.Font("Arial", 8f);
                  //    textobj.ForeColor = Color.White;
                  //    textobj.Y = pp;


                  //    cpuChart.Invoke((Action)(() =>
                  //    {
                  //        cpuChart.Annotations.Add(v1);
                  //        // cpuChart.Annotations.Add(r2);
                  //        cpuChart.Annotations.Add(textobj);
                  //        // cpuChart.Annotations.Add(annotation);
                  //    }));
                  //}
                  #endregion

                  for (int i = 1; i < Dict.Count; i++)
                    {
                        cpuChart.Series[0].Points.Add((Dict[i]));
                        
                    }
                  
                cpuChart.ChartAreas[0].RecalculateAxesScale();
                if (cpuChart.ChartAreas[0].AxisX.Maximum > cpuChart.ChartAreas[0].AxisX.ScaleView.Size)
                    cpuChart.ChartAreas[0].AxisX.ScaleView.Scroll(cpuChart.ChartAreas[0].AxisX.Maximum);
                //if (cpuChart.ChartAreas[0].AxisX.Maximum > cpuChart.ChartAreas[0].AxisX.ScaleView.Size)
                //    cpuChart.ChartAreas[0].AxisX.ScaleView.Scroll(cpuChart.ChartAreas[0].AxisX.Maximum);

            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Timer2Form, UpdateCpuchart()", ae.Message, "DTC_ErrorLog");
            }
        }

        private void ClearAllSeriesPoints()
        {
            foreach (Series item in cpuChart.Series)
            {
                item.Points.Clear();
            }
        }

        private void txtBxHightemp_KeyPress(object sender, KeyPressEventArgs e)
        {
            //  onchange = true;
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public void MonitorOnline_FormClosing(object sender, FormClosingEventArgs e)
        {


        }

        private void txtBxTi_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBxPD_Click(object sender, EventArgs e)
        {
            pdV = true;
        }

        private void txtBxPD_MouseCaptureChanged(object sender, EventArgs e)
        {
            pdV = true;
        }

    


    }

    public struct DeviceSettings   //internal
    {
        public string pv;
        public string sv;
        public string ctrlAction;
        public string run;
        public string out1;
        public string out2;
        public string sensorType;
        public string unit;
        public string highTemp;
        public string lowTemp;
        public string fraction;
        public string pvOffset;
        public string iOffset;
        public string swVersion;
        public string alarm1Mode;
        public string alarm1Up;
        public string alarm1Down;
        public string alarm2Mode;
        public string alarm2Up;
        public string alarm2Down;
        public string hys1;
        public string hys2;
        public string dead1;
        public string outPer1;
        public string outPer2;
        public string ctrlPeriod1;
        public string ctrlPeriod2;
        public string pb;
        public string ti;
        public string td;
        public string ctrlPeriod11;
        public string ctrlPeriod12;
        public string coef;
        public string dead2;
        public string autoTune;
        public string lockStatus;
        public string ioffset;
        public string pdoffset;
        public string LEDdata;
        public string RemTime;
        public string out1cmb;
        public string out2cmb;
    }
}