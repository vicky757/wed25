using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Resources;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTC_Communication_Utility
{
    public partial class RTCController : Form
    {
        public RTCController()
        {
            InitializeComponent();
        }

        private void RTCController_Load(object sender, EventArgs e)
        {
          
            SetValues.Set_CommType = 2;  // 1: serial   2: USB

            SetValuesSingleLine.Addr = "";
            SetValuesSingleLine.Cmd = "0";
            SetValuesSingleLine.Func = "";
            SetValuesSingleLine.WordC = "";

            SetValues.Set_PortName = string.Empty;// "COM1";
            SetValues.Set_Baudrate = string.Empty;//"9600";
            SetValues.Set_parity = string.Empty;// "Even";
            SetValues.Set_BitsLength = 0;// 7;
            SetValues.Set_StopBits = string.Empty;// "1";

            SetPcSettings();
            
    }

        private void SetCommTypeSettings()
        {
            if (SetValues.Set_CommType == 1) // serial
            {
                setDTCToolStripMenuItem.Enabled = true;
                toolStripBtn2SetDTC.Enabled = true;
                toolStripBtn6Singlecmd.Enabled = true;
                singleCommandTextToolStripMenuItem.Enabled = true;
                viewDeviceInformationToolStripMenuItem.Enabled = false;
            }
            else if (SetValues.Set_CommType == 2) // usb
            {
                setDTCToolStripMenuItem.Enabled = true;
                toolStripBtn2SetDTC.Enabled = true;
                toolStripBtn6Singlecmd.Enabled = true;
                singleCommandTextToolStripMenuItem.Enabled = true;  //Test Need To change


                setDTCToolStripMenuItem.Enabled = true;
                toolStripBtn2SetDTC.Enabled = true;
                //toolStripBtn6Singlecmd.Enabled = false;
                //singleCommandTextToolStripMenuItem.Enabled = false;  //Test Need To change
                viewDeviceInformationToolStripMenuItem.Enabled = true;
            }
        }

        private void SetPcSettings()
        {
            
            using (SetPCSettings setPcCom = new SetPCSettings())
            {
                DialogResult dr = setPcCom.ShowDialog();

                if (dr == DialogResult.Cancel)
                {
                    setPcCom.Close();

                }
                else if (dr == DialogResult.OK)
                {
                }
                SetCommTypeSettings();
            }

        }

        private static void setUpgrade()
        {
            using (FWUpgradation upgradefwr = new FWUpgradation())
            {
                upgradefwr.ShowDialog();
            }
        }
        private static void setNewUpgrade()
        {
            using (NewFWUpgradation newupgradefwr = new NewFWUpgradation())
            {
                newupgradefwr.ShowDialog();
            }
        }


        private static void SetRecorder()
        {
            //Recorder1 recorder = new Recorder1();
            //recorder.ShowDialog();
            //GraphRecorder
            using (Timer2 rec = new Timer2())
            {
                rec.ShowDialog();
            }
        }

        private static void SetAbout()
        {
            using (About abt = new About())
            {
                abt.ShowDialog();
            }
        }

        private static void SetDeviceInfo()
        {
            using (frmViewDeviceInfo rec = new frmViewDeviceInfo())
            {
                rec.ShowDialog();
            }
        }

        public static void SetMonitoring(int type)
        {
            switch (type)
            {
                case 1:
                    //TryOutForm obj1 = new TryOutForm();
                    using (MonitorOnline obj1 = new MonitorOnline())  //MonitorForm
                    {
                        obj1.ShowDialog();
                    }
                    break;
                case 2:
                    using (MonitorOnline obj2 = new MonitorOnline())
                    {
                        //TryOutForm obj2 = new TryOutForm();
                        obj2.ShowDialog();
                    }
                    break;

                case 3:
                    //using (MonitorForm monitor = new MonitorForm())
                    //{
                    //    DialogResult dr = monitor.ShowDialog(this);
                    //    if (dr == DialogResult.Cancel)
                    //    {
                    //        monitor.Close();
                    //    }
                    //    else if (dr == DialogResult.OK)
                    //    {

                    //    }
                    //}

                    ////using (frmOnlineMonitor rampSoakForm = new frmOnlineMonitor())
                    ////{
                    ////    DialogResult dr = rampSoakForm.ShowDialog();
                    ////    if (dr == DialogResult.Cancel)
                    ////    {
                    ////        rampSoakForm.Close();
                    ////        //MessageBox.Show("Closed");
                    ////    }
                    ////    else if (dr == DialogResult.OK)
                    ////    {
                    ////        //MessageBox.Show("Monitor");
                    ////    }
                    ////}
                    break;
            }

        }

        private static void SetTcSettings()
        {
            // SetDTC setdtc = new SetDTC();
            using (TcSettings setdtc = new TcSettings())
            {
                setdtc.ShowDialog();
            }
        }

        private static void SingleCommandText()
        {
            using (SinglecmdText snglcmd = new SinglecmdText())
            {
                snglcmd.ShowDialog();
            }
        }

        private void SetClose()
        {
            Application.ExitThread();

            //Environment.Exit();
            Application.Exit();
            //Process.GetCurrentProcess().CloseMainWindow().
        }

        private void toolStripBtn1SetPC_Click(object sender, EventArgs e)
        {
            SetPcSettings();
        }

        private void toolStripBtn2SetDTC_Click(object sender, EventArgs e)
        {
            SetTcSettings();
        }

        private void toolStripBtn3Copyfn_Click(object sender, EventArgs e)
        {
            //setUpgrade();
            setNewUpgrade();
        }

        private void toolStripBtn4MonitorPgm_Click(object sender, EventArgs e)
        {
            SetMonitoring(2);  //3
        }

        private void toolStripBtn5RecorderPgm_Click(object sender, EventArgs e)
        {
            SetRecorder();
        }

        private void toolStripBtn6Singlecmd_Click(object sender, EventArgs e)
        {
            SingleCommandText();
        }

        private void toolStripBtn8About_Click(object sender, EventArgs e)
        {
            SetAbout();
        }

        private void setPCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPcSettings();

        }

        private void setDTCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTcSettings();
        }

        private void RTCController_Paint(object sender, PaintEventArgs e)
        {
            //string imgpth = Directory.GetCurrentDirectory() + "\\MicroPLC.bmp";
            //Bitmap bmap = (Bitmap)Bitmap.FromFile(imgpth, false);
            //Graphics g = this.CreateGraphics();
            //g.DrawImage(bmap, (this.Width - 160) / 2, (this.Height - 280) / 2, 150, 250);
        }

        private void monitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMonitoring(2);
        }

        private void decoderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetRecorder();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetAbout();
        }

        private void toolStripBtn7Exit_Click(object sender, EventArgs e)
        {
            SetClose();
        }

        private void updateFWToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // setUpgrade();
            setNewUpgrade();
        }

        private void RTCController_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetClose();
        }

        private void userControl1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMonitoring(2);
        }

        private void monitorFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMonitoring(1);
        }

        private void singleCommandTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SingleCommandText();
        }

        private void graphRecorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetRecorder();
        }

        private void viewDeviceInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDeviceInfo();
        }


    }
    static class SetValues
    {
        private static string set_release;
        public static string Set_Release
        {
            get { return set_release; }
            set { set_release = value; }
        }


        private static int set_commType;
        public static int Set_CommType
        {
            get { return set_commType; }
            set { set_commType = value; }
        }

        private static string set_words1;
        public static string Set_Words1
        {
            get { return set_words1; }
            set { set_words1 = value; }
        }

        private static string set_words;
        public static string Set_Words
        {
            get { return set_words; }
            set { set_words = value; }
        }

        private static string set_portname;
        public static string Set_PortName
        {
            get { return set_portname; }
            set { set_portname = value; }
        }
        private static string set_Baudrate;
        public static string Set_Baudrate
        {
            get { return set_Baudrate; }
            set { set_Baudrate = value; }
        }
        private static int set_BitsLength;
        public static int Set_BitsLength
        {
            get { return set_BitsLength; }
            set { set_BitsLength = value; }
        }
        private static string set_parity;
        public static string Set_parity
        {
            get { return set_parity; }
            set { set_parity = value; }
        }
        private static string set_StopBits;
        public static string Set_StopBits
        {
            get { return set_StopBits; }
            set { set_StopBits = value; }
        }

        private static string set_CommunicationProtocol;
        private static int set_CommunicationProtocolindex;
        
        public static string Set_CommunicationProtocol
        {
            get { return set_CommunicationProtocol; }
            set { set_CommunicationProtocol = value; }
        }
        private static bool set_BoolUSBdata;
        public static bool Set_BoolUSBdata
        {
            get { return set_BoolUSBdata; }
            set { set_BoolUSBdata = value; }
        }

        public static int Set_CommunicationProtocolindex
        {
            get { return set_CommunicationProtocolindex; }
            set { set_CommunicationProtocolindex = value; }
        }
        private static string set_SelectedPath;
        public static string Set_SelectedPath
        {
            get { return set_SelectedPath; }
            set { set_SelectedPath = value; }
        }
        private static string set_CtrlAction;
        public static string Set_CtrlAction
        {
            get { return set_CtrlAction; }
            set { set_CtrlAction = value; }
        }
        private static string set_SensorType;
        public static string Set_SensorType
        {
            get { return set_SensorType; }
            set { set_SensorType = value; }
        }
        private static string set_RunOrhalt;
        public static string Set_RunOrhalt
        {
            get { return set_RunOrhalt; }
            set { set_RunOrhalt = value; }
        }
        private static string set_Foutput;
        public static string Set_Foutput
        {
            get { return set_Foutput; }
            set { set_Foutput = value; }
        }
        private static string set_Soutput;
        public static string Set_Soutput
        {
            get { return set_Soutput; }
            set { set_Soutput = value; }
        }
        private static string set_lockStatus;
        public static string Set_lockStatus
        {
            get { return set_lockStatus; }
            set { set_lockStatus = value; }
        }
        private static string set_Autotunning;
        public static string Set_Autotunning
        {
            get { return set_Autotunning; }
            set { set_Autotunning = value; }
        }
        private static string set_Unit;
        public static string Set_Unit
        {
            get { return set_Unit; }
            set { set_Unit = value; }
        }
        private static string set_Fraction;
        public static string Set_Fraction
        {
            get { return set_Fraction; }
            set { set_Fraction = value; }
        }
        private static string set_SetValue;
        public static string Set_SetValue
        {
            get { return set_SetValue; }
            set { set_SetValue = value; }
        }
        private static string set_PresentValue;
        public static string Set_PresentValue
        {
            get { return set_PresentValue; }
            set { set_PresentValue = value; }
        }

        private static string set_Device1;
        public static string Set_Device1
        {
            get { return set_Device1; }
            set { set_Device1 = value; }
        }
        private static string set_Device2;
        public static string Set_Device2
        {
            get { return set_Device2; }
            set { set_Device2 = value; }
        }
        private static string set_Device3;
        public static string Set_Device3
        {
            get { return set_Device3; }
            set { set_Device3 = value; }
        }
        private static string set_Device4;
        public static string Set_Device4
        {
            get { return set_Device4; }
            set { set_Device4 = value; }
        }
        private static string set_Device5;
        public static string Set_Device5
        {
            get { return set_Device5; }
            set { set_Device5 = value; }
        }
        private static string set_Device6;
        public static string Set_Device6
        {
            get { return set_Device6; }
            set { set_Device6 = value; }
        }
        private static string set_Device7;
        public static string Set_Device7
        {
            get { return set_Device7; }
            set { set_Device7 = value; }
        }
        private static string set_Device8;
        public static string Set_Device8
        {
            get { return set_Device8; }
            set { set_Device8 = value; }
        }
        private static string set_UnitAddress;
        public static string Set_UnitAddress
        {
            get { return set_UnitAddress; }
            set { set_UnitAddress = value; }
        }
        private static string set_RegAddress;
        public static string Set_RegAddress
        {
            get { return set_RegAddress; }
            set { set_RegAddress = value; }
        }
        private static string set_CommandType;
        public static string Set_CommandType
        {
            get { return set_CommandType; }
            set { set_CommandType = value; }
        }
        private static string set_WordCount;
        public static string Set_WordCount
        {
            get { return set_WordCount; }
            set { set_WordCount = value; }
        }
        private static string set_ASKFrame;
        public static string Set_ASKFrame
        {
            get { return set_ASKFrame; }
            set { set_ASKFrame = value; }
        }
        private static string set_LRCFrame;
        public static string Set_LRCFrame
        {
            get { return set_LRCFrame; }
            set { set_LRCFrame = value; }
        }
        private static string set_Form;
        public static string Set_Form
        {
            get { return set_Form; }
            set { set_Form = value; }
        }
        private static string set_GetDrawingData;
        public static string Set_GetDrawingData
        {
            get { return set_GetDrawingData; }
            set { set_GetDrawingData = value; }
        }
        private static string set_MinGraphScale;
        public static string Set_MinGraphScale
        {
            get { return set_MinGraphScale; }
            set { set_MinGraphScale = value; }
        }
        private static string set_MaxGraphScale;
        public static string Set_MaxGraphScale
        {
            get { return set_MaxGraphScale; }
            set { set_MaxGraphScale = value; }
        }
        private static bool set_ChartAtLaod;
        public static bool Set_ChartAtLaod
        {
            get { return set_ChartAtLaod; }
            set { set_ChartAtLaod = value; }
        }
        private static byte set_NodeAddress;
        public static byte Set_NodeAddress
        {
            get { return set_NodeAddress; }
            set { set_NodeAddress = value; }
        }
    }

    static class SetValuesSingleLine
    {
        private static string addr;
        public static string Addr
        {
            get { return addr; }
            set { addr = value; }
        }

        private static string cmd;
        public static string Cmd
        {
            get { return cmd; }
            set { cmd = value; }
        }

        private static string func;
        public static string Func
        {
            get { return func; }
            set { func = value; }
        }

        private static string wordC;
        public static string WordC
        {
            get { return wordC; }
            set { wordC = value; }
        }

    }

    public static class Globals<T>
    {
        public static List<T> lists { get; set; }

    }
}
