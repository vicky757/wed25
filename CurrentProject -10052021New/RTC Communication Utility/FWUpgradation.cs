using ClassList;
using System;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace RTC_Communication_Utility
{
    public partial class FWUpgradation : Form
    {
        private object _prizmMDIStatusBar;
        private object _prizmMDIProgressBar;
        private object _prizmMDIProgressLabel;
        protected int _devicePortNo;
        protected string _deviceLastError;
        protected bool _deviceIsConnected;
        private SerialPort _serialPort = null;
        public SerialPort _portTemp = null;
        SettingClass obj = null;
        public int currentDb = 0;
        public FWUpgradation()
        {
            InitializeComponent();
        }

        private SerialPort port = new SerialPort();
        private ResourceManager _manager = new ResourceManager(typeof(FWUpgradation));
        private DataSet ds3;
        private ClassList.Serial serial = null;
        private string m_exePath1 = string.Empty;
        private string logFilePath = string.Empty;
        private clsModbus modObj = null;
        private IWin32Window ownerr;
        #region Events and Delegates
        public delegate void DownloadPercentage(float pPercentage);
        public delegate void DownloadStatus(int pMessage);
        public delegate void SetProgressBarDelegate(float pPercent);
        public delegate void SetStatusBarDelegate(int pMessage);
        public delegate void DisableBtnAtRuntime();
        public event DisableBtnAtRuntime _DisableBtnAtRuntime;
        public delegate void EnableBtnAtRuntime();
        public event EnableBtnAtRuntime _EnableBtnAtRuntime;
        #endregion

        #region Variables
        private Boolean DownloadComplete = false;
        private int _downloadStatus = 0;
        public int StartTestResponse = 0;
        public string strUSBDwnlTest = "";
        private bool blDownloadFlag = false;
        public string Set_SplPortName = "";
        private bool blDownloadTwiceFlag = false;
        #endregion

        private byte[] _setupFrameMode = new byte[10];

        private void Form1_Load(object sender, EventArgs e)
        {
              // bhushan Test
          //  int bitsLength = SetValues.Set_BitsLength;
          
            this.Text = "Firmware Download";

            if (SetValues.Set_CommType == 1)
            {
                this.Text += " [Serial]";

            }
            else if (SetValues.Set_CommType == 2)
            {
                this.Text += " [USB]";
                //string portName = Serial.GetQTProductPort(); // KA 0910    6:45
                //SetValues.Set_PortName = portName;           // KA 0910    6:45
            }

            if (SetValues.Set_Release == "0")
            {
                //MessageBox.Show("Release");
            }
            else if (SetValues.Set_Release == "1")
            {
                //MessageBox.Show("Production");
            }

            try
            {
                string[] Allports = SerialPort.GetPortNames();
                // btn_Download.Enabled = false;
                m_exePath1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                rbBtnBase.Checked = true;
                ownerr = this;

                modObj = new clsModbus();
                modObj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
            }
            catch (Exception err)
            {
                toolStripStatusLabel1.Text = err.Message;
            }
        }

        private void cmbBx_ComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClassList.CommonConstants.SetPortName = SetValues.Set_PortName;
        }

        private void cmbBx_current_product_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbBx_new_product_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rdbtn_Firmware_CheckedChanged(object sender, EventArgs e)
        {
            //cmbBx_current_product.Enabled = true;
            //cmbBx_new_product.Enabled = false;
        }

        private void rdbtn_bootblock_CheckedChanged(object sender, EventArgs e)
        {
            //cmbBx_current_product.Enabled = false;
            //cmbBx_new_product.Enabled = true;
        }

        #region Serial Application Download----AN

        public void GetPercentage(float pPercentage)
        {
            applicationDownloadPercentage(pPercentage);
        }

        public void serial__deviceDownloadStatus(int pMessage)
        {
            applicationDownloadStatus(pMessage);
        }

        private void GetDownloadPercentage(float pPercentage)
        {
            applicationDownloadPercentage(pPercentage);
        }

        private void applicationDownloadStatus(int pMessage)
        {
            try
            {
                if (_prizmMDIStatusBar != null)
                {
                    if (((StatusStrip)_prizmMDIStatusBar).InvokeRequired)
                        ((StatusStrip)_prizmMDIStatusBar).Invoke(new SetStatusBarDelegate(applicationDownloadStatus), (pMessage));
                    else
                    {
                        ((StatusStrip)_prizmMDIStatusBar).Items[0].Text = "";
                        ((StatusStrip)_prizmMDIStatusBar).Items[0].Text = _manager.GetString(((ClassList.DownloadingStatusMessages)(pMessage & 255)).ToString());
                        if ((pMessage & 65280) > 0)
                            ((StatusStrip)_prizmMDIStatusBar).Items[0].Text += _manager.GetString(((ClassList.DownloadingStatusMessages)(pMessage & 65280)).ToString());
                        ((StatusStrip)_prizmMDIStatusBar).Parent.Refresh();
                    }
                }
                if (pMessage == 17)
                {

                    //#region reset setting Ritesh_Change
                    ////Ritesh_Change Again need to move previous setting. 
                    //if (currentDb == 7)
                    //{
                    //    serial.ConnectActualSetting(currentDb);
                    //}
                    //#endregion
                    toolStripStatusLabel1.Text = "Download Completed. (Please Re-Power Unit)";
                    ownerr = this;
                   
                    // MessageBoxEx.Show(ownerr, "Please Re-Power Unit", "Download Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("Please Re-Power Unit");
                }
                if (pMessage == 0)
                    toolStripStatusLabel1.Text = "";
                if (pMessage == 101)
                    toolStripStatusLabel1.Text = "Select Appropriate File";
                if (pMessage == 102)
                    toolStripStatusLabel1.Text = "Incorrect File Format";
                if (pMessage == 103)
                    toolStripStatusLabel1.Text = "Please enter valid node address";
                if (pMessage == 104)
                    toolStripStatusLabel1.Text = "No firmware file ";
                if (pMessage == 105)
                    toolStripStatusLabel1.Text = "Please wait..";
                if (pMessage == 106)
                    toolStripStatusLabel1.Text = "Device type and node address mismatch";
                if (pMessage == 107)
                    toolStripStatusLabel1.Text = "Folder not found";
                if (pMessage == 108)
                    toolStripStatusLabel1.Text = "Empty folder";
                if (pMessage == 109)
                    toolStripStatusLabel1.Text = "Device not connected";
                if (pMessage == 110)
                    toolStripStatusLabel1.Text = "Something went wrong";
                if (pMessage == 111)
                    toolStripStatusLabel1.Text = "Node address and device type mismatch";

                if (pMessage == 112)
                    toolStripStatusLabel1.Text = "Preparing flash for firmware";
                if (pMessage == 113)
                    toolStripStatusLabel1.Text = "Node address is not responding ";
                if (pMessage == 114)
                    toolStripStatusLabel1.Text = "Node address is not available";
                if (pMessage == 115)
                    toolStripStatusLabel1.Text = "Check device connection?";
                if (pMessage == 116)
                    toolStripStatusLabel1.Text = "Download failed.Please try again later";

                if (toolStripStatusLabel1.Text == "Download Completed")
                {
                    btn_Download.Enabled = true;
                    rdbtn_Close.Enabled = true;

                    _EnableBtnAtRuntime();
                    pgbDownload.Value = 0;
                    lbDownload_Progress.Text = "0";

                    if (DownloadComplete == false)
                    {
                        DownloadComplete = true;
                    }
                    _downloadStatus = 1;
                    StartTestResponse = 1;
                    strUSBDwnlTest = "Download Completed";


                }
                else
                {

                }
                if (toolStripStatusLabel1.Text == "Product Mismatch")
                {
                    btn_Download.Enabled = true;
                    rdbtn_Close.Enabled = true;

                    _EnableBtnAtRuntime();
                    ClassList.CommonConstants.communicationStatus = 0;

                    StartTestResponse = 1;
                }
                if (toolStripStatusLabel1.Text == "Device not responding")
                {
                    btn_Download.Enabled = true;
                    rdbtn_Close.Enabled = true;
                    // btnOpen.Enabled = true;

                    // _EnableBtnAtRuntime();
                    ClassList.CommonConstants.communicationStatus = 0;

                    StartTestResponse = 1;
                }
            }
            catch (Exception err)
            {
                toolStripStatusLabel1.Text = err.Message;
            }
        }

        private void applicationDownloadPercentage(float pPercentage)
        {
            //Thread.Sleep(20);
            if (pPercentage >= 0 && pPercentage < 100)
            {
                toolStripStatusLabel1.Text = "Downloading..";
            }
            if (_prizmMDIProgressBar != null && ClassList.CommonConstants.communicationStatus >= 0)
            {
                if (((ProgressBar)_prizmMDIProgressBar).InvokeRequired)
                {
                    ((ProgressBar)_prizmMDIProgressBar).Invoke(new SetProgressBarDelegate(applicationDownloadPercentage), (pPercentage));
                }
                else
                {
                    ((ProgressBar)_prizmMDIProgressBar).Value = Convert.ToInt32(pPercentage);
                    ((Label)_prizmMDIProgressLabel).Text = Convert.ToInt32(pPercentage).ToString() + "%";
                }
            }
        }

        private void GetDownLoadParameterOnSerial(ProgressBar pProgressBar, StatusStrip pStatusBar, Label pProgressLabel)
        {
            try
            {
                LogWriter.WriteToFile("FWUpgradation.cs - GetDownLoadParameterOnSerial()", "started--", "RTC_Upgrade");
                Refresh();
                _prizmMDIProgressBar = pProgressBar;
                _prizmMDIStatusBar = pStatusBar;
                _prizmMDIProgressLabel = pProgressLabel;
                int ID = 0;
                ds3 = new DataSet();
                serial__deviceDownloadStatus(105);

                serial = new ClassList.Serial();
                serial._deviceDownloadPercentage += new ClassList.Serial.DownloadPercentage(GetDownloadPercentage);
                serial._deviceDownloadStatus += new ClassList.Serial.DownloadStatus(serial__deviceDownloadStatus);

                ClassList.CommonConstants.TEMP_DOWNLOAD_FILENAME = SetValues.Set_SelectedPath;
                _setupFrameMode[0] = 1; //Halt before DWNL
                _setupFrameMode[1] = 1; //Run Aft DWNL
                _setupFrameMode[2] = 0; // Clean memory
                _setupFrameMode[3] = 1; // Non keep memory
                _setupFrameMode[4] = 0; // Data Dwnl

                String _DwnlFilename = SetValues.Set_SelectedPath;
                int iProductID = ID;
                ClassList.CommonConstants.ProductIdentifier = iProductID;

                LogWriter.WriteToFile("Product Id : ", iProductID.ToString(), "RTC_Upgrade");

                int tempDnldData = 0;
                ClassList.DownloadData downloadData;

                tempDnldData += (int)ClassList.DownloadData.Firmware;
                downloadData = (ClassList.DownloadData)tempDnldData;
                DownloadOnSerial_Update1(downloadData, _DwnlFilename, iProductID, SetValues.Set_PortName, _setupFrameMode);
               // DownloadOnSerial(downloadData, _DwnlFilename, iProductID, SetValues.Set_PortName, _setupFrameMode);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        public void DownloadOnSerial(ClassList.DownloadData pDownloadData, string pFileName, int pProductID, string pComNoOrIpAddress, byte[] _setupFrameMode)
        {
            LogWriter.WriteToFile("FWUpgradation.cs - DownloadOnSerial()", "started--", "RTC_Upgrade");
            //ClassList.DownloadData pDownloadData
            ClassList.Serial serial = new ClassList.Serial(pComNoOrIpAddress, 0, 0, 0, 0, _setupFrameMode);

            serial._deviceDownloadPercentage += new ClassList.Serial.DownloadPercentage(GetDownloadPercentage);
            serial._deviceDownloadStatus += new ClassList.Serial.DownloadStatus(serial__deviceDownloadStatus);
            serial__deviceDownloadStatus(0);
            serial.DeviceProdID = Convert.ToInt16(pProductID);

            ClassList.CommonConstants.SetPortName = SetValues.Set_PortName;
            ClassList.CommonConstants.setBaudRate = SetValues.Set_Baudrate;
            ClassList.CommonConstants.setParity = SetValues.Set_parity;
            ClassList.CommonConstants.setBitsLength = SetValues.Set_BitsLength.ToString();
            ClassList.CommonConstants.setStopbits = SetValues.Set_StopBits;


            if (serial.Connect() == ClassList.CommonConstants.SUCCESS)
            {
                LogWriter.WriteToFile("FWUpgradation.cs-", "Start", "RTC_Upgrade");
                int dwnl_type;
                int initial_dwnl;
                byte NodeAddress = SetValues.Set_NodeAddress;

                initial_dwnl = serial.Plc_Download_ProtocolInitial(pFileName, (byte)pDownloadData, NodeAddress);

                LogWriter.WriteToFile("inital dwnld: ", initial_dwnl.ToString(), "RTC_Upgrade");
                if (initial_dwnl == ClassList.CommonConstants.SUCCESS)
                {
                    dwnl_type = serial.Plc_Download_Protocol(pFileName, (byte)pDownloadData);
                    LogWriter.WriteToFile("dwnl_type: Success 1", dwnl_type.ToString(), "RTC_Upgrade");
                    if (dwnl_type == ClassList.CommonConstants.SUCCESS)
                    {
                        LogWriter.WriteToFile("dwnl_type: Success 2", dwnl_type.ToString(), "RTC_Upgrade");
                        if (_setupFrameMode[0] == 1)
                        {
                            if (dwnl_type == 0)
                            {
                                LogWriter.WriteToFile("dwnl_type: Success 2-1", dwnl_type.ToString(), "RTC_Upgrade");
                                serial.SendFile1(pFileName, (byte)pDownloadData);
                            }
                            LogWriter.WriteToFile("dwnl_type: Success 2-2", dwnl_type.ToString(), "RTC_Upgrade");
                        }
                        else
                        {
                            LogWriter.WriteToFile("dwnl_type: Success 2-3", dwnl_type.ToString(), "RTC_Upgrade");
                            serial.SendFile(pFileName, (byte)pDownloadData);
                        }
                    }
                    else if (dwnl_type == ClassList.CommonConstants.ONLYBOOTBLOCK)
                    {
                        LogWriter.WriteToFile("dwnl_type: success 1 - bootblock 1", dwnl_type.ToString(), "RTC_Upgrade");
                        serial.SendFile(pFileName, (byte)pDownloadData);
                    }
                    else if (ClassList.CommonConstants.IsErrorResetChecked == true && toolStripStatusLabel1.Text == "Ready")
                    {
                        StartTestResponse = 0;
                        ClassList.CommonConstants.communicationStatus = 0;
                        DialogResult result1;
                        result1 = MessageBox.Show("Connect Serial cable and press Yes if you want to continue with downloading or No to cancel ", "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result1 == DialogResult.Yes)
                        {
                            if (this.InvokeRequired)
                            {
                                btn_Upgrade_Click(null, null);

                                while (true)
                                {
                                    if (StartTestResponse == 0)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                                btn_Upgrade_Click(null, null);
                        }
                        if (result1 == DialogResult.No)
                        {
                            StartTestResponse = 1;
                        }
                    }
                }
                #region device when in bootblock
                else if (initial_dwnl == ClassList.CommonConstants.ONLYBOOTBLOCK)//Keeran (KA) (14/03/2019)
                {

                    dwnl_type = serial.Plc_Download_Protocol(pFileName, (byte)pDownloadData);
                    LogWriter.WriteToFile("dwnl_type: bootblock 1", dwnl_type.ToString(), "RTC_Upgrade");
                    if (dwnl_type == ClassList.CommonConstants.SUCCESS)
                    {
                        serial.SendFile(pFileName, (byte)pDownloadData);
                    }
                }
                #endregion
                else if (ClassList.CommonConstants.IsErrorResetChecked == true && toolStripStatusLabel1.Text == "Ready")
                {
                    StartTestResponse = 0;
                    ClassList.CommonConstants.communicationStatus = 0;
                    DialogResult result1;
                    result1 = MessageBox.Show("Connect Serial cable and press Yes if you want to continue with downloading or No to cancel ", "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result1 == DialogResult.Yes)
                    {
                        if (this.InvokeRequired)
                        {
                            btn_Upgrade_Click(null, null);

                            while (true)
                            {
                                if (StartTestResponse == 0)
                                {
                                    Thread.Sleep(100);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                            btn_Upgrade_Click(null, null);
                    }
                    if (result1 == DialogResult.No)
                    {
                        StartTestResponse = 1;
                    }
                }
            }
            else
            {

                btn_Download.Enabled = true;
                rdbtn_Close.Enabled = true;
                serial__deviceDownloadStatus(109);

            }
            //btn_Download.Enabled = true;
        }

        #endregion


        public int serialConn()
        {
            obj.Comport = SetValues.Set_PortName;
            obj.Baudrate = 115200;
            obj.Parity = "None";
            obj.Stopbits = "2";
            obj.Bitslength = 8;
            obj.Protocol = "Ascii";

            _portTemp.BaudRate = obj.Baudrate;
            // _portTemp.Parity = obj.Parity;

            switch (obj.Parity)
            {
                case "Even":
                    _portTemp.Parity = Parity.Even;
                    break;
                case "Odd":
                    _portTemp.Parity = Parity.Odd;
                    break;
                case "None":
                default:
                    _portTemp.Parity = Parity.None;
                    break;
            }

            // set stopbit
            switch (obj.Stopbits)
            {
                case "1":
                    _portTemp.StopBits = StopBits.One;
                    break;
                case "1.5":
                    _portTemp.StopBits = StopBits.OnePointFive;
                    break;
                case "2":
                    _portTemp.StopBits = StopBits.Two;
                    break;
                default:
                    _portTemp.StopBits = StopBits.None;
                    break;
            }

            _portTemp.DataBits = obj.Bitslength;
            _portTemp.ReadBufferSize = 8500;

            try
            {
                if (!_portTemp.IsOpen)
                    _portTemp.Open();
            }
            catch (Exception exx)
            {
                return 0;
            }

            if (!_serialPort.IsOpen)
            {
                _deviceIsConnected = false;
                CommonConstants.communicationStatus = 0;
                return 0;
            }
            else
            {
                _deviceIsConnected = true;
                return 1;
            }


          
        }

        #region Keeran (KA)
        public void DownloadOnSerial_Update1(ClassList.DownloadData pDownloadData, string pFileName, int pProductID, string pComNoOrIpAddress, byte[] _setupFrameMode)
        {
            try
            {
                LogWriter.WriteToFile("FWUpgradation.cs - DownloadOnSerial_Update1()", "started--", "RTC_Upgrade");

                GetSerialConection(pProductID, pComNoOrIpAddress, _setupFrameMode);
                serial__deviceDownloadStatus(105);
                {
                    if (serial.Connect() == ClassList.CommonConstants.SUCCESS)
                    {
                        LogWriter.WriteToFile("FWUpgradation.cs-", "Start", "RTC_Upgrade");
                        int dwnl_type, dwnl_type2, dwnl_type3, dwnl_type4;
                        int initial_dwnl;
                        byte NodeAddress = SetValues.Set_NodeAddress;

                        int baseType = ClassList.CommonConstants.SetFileType;   // 1 : Base
                        int commType = SetValues.Set_CommType; // 1 : Serial     2 : Usb

                        // USB :  //DWNL 70 14 1/2->    <-71/73    ////DWNL(HEX) 70 nodeaddress setfiletype  
                        //Serial : //70 14 1/2->    <-71/73    ////70 nodeaddress setfiletype  
                        //REPL->     <-[64]
                        //7Bit_work
                        initial_dwnl = serial.Plc_Download_ProtocolInitial_Update1(pFileName, (byte)pDownloadData, NodeAddress, commType, baseType);

                        LogWriter.WriteToFile("inital dwnld: ", initial_dwnl.ToString(), "RTC_Upgrade");



                        //if 73
                        if (initial_dwnl == ClassList.CommonConstants.SUCCESS)
                        {
                            //D0->    <-D2
                            dwnl_type2 = serial.Plc_Download_Protocol_Update2(pFileName, (byte)pDownloadData);
                            LogWriter.WriteToFile("dwnl_type2: Success 1", dwnl_type2.ToString(), "RTC_Upgrade");

                            //if D2
                            if (dwnl_type2 == ClassList.CommonConstants.SUCCESS)
                            {
                                //REPL->     <-01
                                dwnl_type3 = serial.Plc_Download_Protocol_Update3(pFileName, (byte)pDownloadData);
                                LogWriter.WriteToFile("dwnl_type3: Success 3", dwnl_type3.ToString(), "RTC_Upgrade");

                                //if 01
                                if (dwnl_type3 == ClassList.CommonConstants.SUCCESS)
                                {
                                    //99->     <-9B
                                    dwnl_type4 = serial.Plc_Download_Protocol_Update1(pFileName, (byte)pDownloadData);
                                    LogWriter.WriteToFile("dwnl_type4: Success 4", dwnl_type4.ToString(), "RTC_Upgrade");
                                    try
                                    {


                                        //if 9B
                                        if (dwnl_type4 == ClassList.CommonConstants.SUCCESS)
                                        {
                                            //REPL->     <-9A

                                            //USB: close and open virtual port again 
                                            bool valid = false;
                                            if (commType == 2) // usb
                                            {

                                                valid = true;


                                                if (valid)
                                                {
                                                    dwnl_type = serial.Plc_Download_Protocol_Update4(pFileName, (byte)pDownloadData);
                                                    LogWriter.WriteToFile("dwnl_type: Success 0", dwnl_type.ToString(), "RTC_Upgrade");

                                                    //if 9A
                                                    if (dwnl_type == ClassList.CommonConstants.SUCCESS)
                                                    {
                                                        //serial.SendFile_Update1(pFileName, (byte)pDownloadData, (byte)NodeAddress);

                                                        //[20]->    <-01
                                                        //B8->      <-B8
                                                        if (ClassList.CommonConstants.SUCCESS == serial.SendFile(pFileName, (byte)pDownloadData))
                                                        {

                                                            // serial__deviceDownloadStatus(109);
                                                        }
                                                        else
                                                        {
                                                            serial__deviceDownloadStatus(110);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        serial__deviceDownloadStatus(116);
                                                    }
                                                }
                                                else
                                                {
                                                    serial__deviceDownloadStatus(116);
                                                }
                                            }
                                            else if (commType == 1)
                                            {
                                                valid = true;

                                                if (valid)
                                                {
                                                    dwnl_type = serial.Plc_Download_Protocol_Update44(pFileName, (byte)pDownloadData);
                                                    LogWriter.WriteToFile("dwnl_type: Success 0", dwnl_type.ToString(), "RTC_Upgrade");

                                                    //if 9A
                                                    if (dwnl_type == ClassList.CommonConstants.SUCCESS)
                                                    {
                                                       //serial.SendFile_Update1(pFileName, (byte)pDownloadData, (byte)NodeAddress);

                                                        //[20]->    <-01
                                                        //B8->      <-B8
                                                        if (ClassList.CommonConstants.SUCCESS == serial.SendFile(pFileName, (byte)pDownloadData))
                                                        {

                                                            // serial__deviceDownloadStatus(109);
                                                        }
                                                        else
                                                        {
                                                            serial__deviceDownloadStatus(110);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        serial__deviceDownloadStatus(116);
                                                    }
                                                }
                                                else
                                                {
                                                    serial__deviceDownloadStatus(116);
                                                }
                                            }


                                        }
                                    }
                                    catch (Exception)
                                    {
                                        serial__deviceDownloadStatus(116);
                                    }

                                }
                            }


                        }
                        //if 71
                        else if (initial_dwnl == ClassList.CommonConstants.ONLYBOOTBLOCK)
                        {
                            //99->    <-9A
                            dwnl_type = serial.Plc_Download_Protocol_Update1(pFileName, (byte)pDownloadData);
                            LogWriter.WriteToFile("dwnl_type: bootblock 1", dwnl_type.ToString(), "RTC_Upgrade");

                            //if 9A
                            if (dwnl_type == ClassList.CommonConstants.ONLYBOOTBLOCK)
                            {
                                //[20]->     <-01
                                //B8->       <-B8

                                //if B8
                                //int res = serial.SendFile_Update1(pFileName, (byte)pDownloadData, (byte)NodeAddress);
                                int res = serial.SendFile(pFileName, (byte)pDownloadData);
                                LogWriter.WriteToFile("res: bootblock 1", res.ToString(), "RTC_Upgrade");
                                if (res == ClassList.CommonConstants.SUCCESS)
                                {
                                    //serial__deviceDownloadStatus(105);
                                }
                                else
                                    serial__deviceDownloadStatus(110);
                            }
                            else
                            {
                                serial__deviceDownloadStatus(116);
                            }
                        }
                        else if (ClassList.CommonConstants.IsErrorResetChecked == true && toolStripStatusLabel1.Text == "Ready")
                        {
                            StartTestResponse = 0;
                            ClassList.CommonConstants.communicationStatus = 0;
                            DialogResult result1;
                            result1 = MessageBox.Show("Connect Serial cable and press Yes if you want to continue with downloading or No to cancel ", "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result1 == DialogResult.Yes)
                            {
                                if (this.InvokeRequired)
                                {
                                    btn_Upgrade_Click(null, null);

                                    while (true)
                                    {
                                        if (StartTestResponse == 0)
                                        {
                                            Thread.Sleep(100);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                    btn_Upgrade_Click(null, null);
                            }
                            if (result1 == DialogResult.No)
                            {
                                StartTestResponse = 1;
                            }
                        }
                    }
                    else
                    {

                        btn_Download.Enabled = true;
                        rdbtn_Close.Enabled = true;
                        serial__deviceDownloadStatus(109);
                    }
                }
              
            }
            catch (Exception ex)
            {
                btn_Download.Enabled = true;
                rdbtn_Close.Enabled = true;
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void GetSerialConection(int pProductID, string pComNoOrIpAddress, byte[] _setupFrameMode)
        {
            serial = new ClassList.Serial(pComNoOrIpAddress, 0, 0, 0, 0, _setupFrameMode);

            serial._deviceDownloadPercentage += new ClassList.Serial.DownloadPercentage(GetDownloadPercentage);
            serial._deviceDownloadStatus += new ClassList.Serial.DownloadStatus(serial__deviceDownloadStatus);
            serial__deviceDownloadStatus(112);
            serial.DeviceProdID = Convert.ToInt16(pProductID);

            ClassList.CommonConstants.SetPortName = SetValues.Set_PortName;
            ClassList.CommonConstants.setBaudRate = SetValues.Set_Baudrate;
            ClassList.CommonConstants.setParity = SetValues.Set_parity;
            ClassList.CommonConstants.setBitsLength = SetValues.Set_BitsLength.ToString();
            ClassList.CommonConstants.setStopbits = SetValues.Set_StopBits;
        }

        #endregion

        private void statusStrip1_TextChanged(object sender, EventArgs e)
        {
            if (statusStrip1.Text == "Download Completed" || statusStrip1.Text == "Communication Aborted")
            {
                btn_Download.Enabled = true;
            }
            if ((statusStrip1.Items[0].Text == "Communication Aborted"))
            {
                btn_Download.Enabled = true;
            }
            if ((statusStrip1.Items[0].Text == "Download Completed"))
            {
                btn_Download.Enabled = true;
            }
            if ((statusStrip1.Items[0].Text == "Device not responding"))
            {
                btn_Download.Enabled = true;
            }
        }

        private void rdbtn_Close_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to close upgrade Firmware?",
                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                if (serial != null)
                {
                    serial.Close();
                }
                //Application.ExitThread();
                this.Close();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileNames = openFileDialog1.FileName;
                CheckBrowseFile(fileNames);
            }
        }

        public bool CheckBrowseFile(string fileNames)
        {
            if (!string.IsNullOrEmpty(fileNames))
            {
                if (Path.HasExtension(fileNames))
                {
                    string ext = Path.GetExtension(fileNames).ToLower();
                    if (!string.IsNullOrEmpty(ext) && ext == ".abs")
                    {
                        SetValues.Set_SelectedPath = textBox1.Text = fileNames;
                        btn_Download.Enabled = true;
                        serial__deviceDownloadStatus(0);
                        return true;
                    }
                    else
                    {
                        btn_Download.Enabled = false;
                        //incorrect file format
                        serial__deviceDownloadStatus(102);
                    }
                }
                else
                {
                    //incorrect file
                    serial__deviceDownloadStatus(101);
                }
            }
            else
            {
                //Select file to download
                serial__deviceDownloadStatus(104);
            }
            SetValues.Set_SelectedPath = string.Empty;


            return false;
        }

        public int CheckFileLoc()
        {
            string logFilePath1 = "";

            if (rbBtnBase.Checked)
                logFilePath = m_exePath1 + "\\HIO\\Object\\FL002-0102TV";
            else
                logFilePath = m_exePath1 + "\\HIO\\Object\\FLA0102TV";

            if (!string.IsNullOrEmpty(logFilePath))
            {

                DirectoryInfo logDirInfo = new DirectoryInfo(logFilePath);

                if (logDirInfo.Exists)
                {
                    if (rdBtnCali.Checked)
                    {
                        logFilePath1 = logFilePath + "\\CalibrationFw";
                    }
                    else
                    {
                        logFilePath1 = logFilePath + "\\UserFw";
                    }

                    if (!string.IsNullOrEmpty(logFilePath1))
                    {
                        DirectoryInfo logDirInfo1 = new DirectoryInfo(logFilePath1);
                        if (logDirInfo1.Exists)
                        {
                            int numABS = logDirInfo1.GetFiles("*.abs", SearchOption.AllDirectories).Length;

                            if (numABS > 0)
                            {
                                string[] files = Directory.GetFiles(logFilePath1);
                                Array.Sort<string>(files);
                                string fileName = files[files.Length - 1];
                                SetValues.Set_SelectedPath = textBox1.Text = fileName;
                                //    serial__deviceDownloadStatus(0);

                                return 3;


                                #region other code - absfile
                                // string fileNames = files[0];

                                // if (Path.HasExtension(fileNames))
                                //{
                                //     string ext = Path.GetExtension(fileNames).ToLower();

                                //     if (!string.IsNullOrEmpty(ext) && ext == ".abs")
                                //     {
                                ////multiple abs files present
                                //if (files.Length > 1)
                                //{
                                //    return 4;
                                //}
                                ////only single abs file present
                                //else
                                //{
                                //    SetValues.Set_SelectedPath = textBox1.Text = fileNames;                                   
                                //    serial__deviceDownloadStatus(0);
                                //    return 5;
                                //}
                                //     }
                                //    else
                                //    {
                                //        return 3;//invalid file extension
                                //    }
                                //}
                                //else
                                //    return 1;//invalid file
                                #endregion
                            }
                            else
                                return 2; // no abs
                        }
                        else
                        {
                            logDirInfo1.Create();
                            return 0;//folder not found
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    logDirInfo.Create();
                    return 0;//folder not found

                }
            }
            return -1;
        }

        private void btn_Upgrade_Click(object sender, EventArgs e)
        {
            LogWriter.WriteToFile("FWUpgradation.cs - btn_Upgrade_Click", "================================================ Upgrade Start ================================================", "RTC_Upgrade");
            pgbDownload.Value = 0;
            lbDownload_Progress.Text = "0%";
            int res = CheckFileLoc();

            currentDb = SetValues.Set_BitsLength;
         //   MessageBox.Show(currentDb.ToString());
            switch (res)
            {
                case 0:
                    //folder not found
                    serial__deviceDownloadStatus(107);
                    break;
                case 1:
                    //invalid file format
                    serial__deviceDownloadStatus(102);
                    break;
                case 2:
                    //no abs file                    
                    serial__deviceDownloadStatus(104);
                    break;
                case 3:
                    if (SetValues.Set_SelectedPath != null)
                    {
                        if ((!string.IsNullOrEmpty(txtB_NodeAddress.Text)))
                        {
                            if (rbBtnBase.Checked)
                                ClassList.CommonConstants.SetFileType = 1;
                            else
                                ClassList.CommonConstants.SetFileType = 2;

                            SetValues.Set_NodeAddress = Convert.ToByte(txtB_NodeAddress.Text);
                            ClassList.CommonConstants.NodeAddress = SetValues.Set_NodeAddress;
                            LogWriter.WriteToFile("Node Address : ", SetValues.Set_NodeAddress.ToString(), "RTC_Upgrade");
                            DialogResult result;
                            string msgs = SetValues.Set_CommType == 1 ? "Connect serial cable between PC COM Port to Unit" : "Connect USB cable between PC COM Port to Unit";  // 1: serial 2: USB

                            result = MessageBox.Show(msgs, "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                
                                rdbtn_Close.Enabled = false;
                                btn_Download.Enabled = false;
                                if (!string.IsNullOrEmpty(SetValues.Set_PortName))
                                {
                                    Set_SplPortName = SetValues.Set_PortName;
                                    LogWriter.WriteToFile("port name : ", Set_SplPortName.ToString(), "RTC_Upgrade");
                                    GetDownLoadParameterOnSerial(pgbDownload, statusStrip1, lbDownload_Progress);
                                }
                                else
                                {
                                    serial__deviceDownloadStatus(115);
                                }
                            }
                            if (result == DialogResult.No)
                            {
                                return;
                            }
                        }
                        else
                        {
                            //Please enter valid node address
                            serial__deviceDownloadStatus(103);
                        }
                    }
                    else
                        serial__deviceDownloadStatus(104);
                    break;
            }
              Thread.Sleep(600);
            btn_Download.Enabled = true;
           
            rdbtn_Close.Enabled = true; //Keeran (KA)
        }

        //Keeran (KA) 12042018_0624
        //private void btn_Upgrade_Click(object sender, EventArgs e)
        //{
        //    LogWriter.WriteToFile("FWUpgradation.cs - btn_Upgrade_Click", "================================================ Upgrade Start ================================================", "RTC_Upgrade");
        //    pgbDownload.Value = 0;
        //    lbDownload_Progress.Text = "0%";
        //    if (CheckBrowseFile(textBox1.Text))
        //    {
        //        if (SetValues.Set_SelectedPath != null)
        //        {
        //            if ((!string.IsNullOrEmpty(txtB_NodeAddress.Text)))
        //            {
        //                SetValues.Set_NodeAddress = Convert.ToByte(txtB_NodeAddress.Text);
        //                LogWriter.WriteToFile("Node Address : ", SetValues.Set_NodeAddress.ToString(), "RTC_Upgrade");
        //                DialogResult result;
        //                result = MessageBox.Show("Connect serial cable between PC COM Port to Unit", "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //                if (result == DialogResult.Yes)
        //                {
        //                    rdbtn_Close.Enabled = false;
        //                    btn_Download.Enabled = false;

        //                    Set_SplPortName = SetValues.Set_PortName;
        //                    LogWriter.WriteToFile("port name : ", Set_SplPortName.ToString(), "RTC_Upgrade");
        //                    GetDownLoadParameterOnSerial(pgbDownload, statusStrip1, lbDownload_Progress);
        //                }
        //                if (result == DialogResult.No)
        //                {
        //                    return;
        //                }
        //            }
        //            else
        //            {
        //                //Please enter valid node address
        //                serial__deviceDownloadStatus(103);
        //                //MessageBox.Show("Please enter valid node address", "Download", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            }
        //        }
        //        else
        //        {
        //            serial__deviceDownloadStatus(104);
        //        }
        //    }
        //    else
        //    {
        //        //Select file to download
        //        //serial__deviceDownloadStatus(104);
        //        //MessageBox.Show("Please select path to download firmware", "Download", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }

        //    rdbtn_Close.Enabled = true; //Keeran (KA)
        //    //btn_Download.Enabled = true; //Keeran (KA)
        //}

        private void rbBtnBase_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBtnBase.Checked)
            {
                rbBtnExp.Checked = false;

            }
        }

        private void rbBtnExp_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBtnExp.Checked)
            {
                rbBtnBase.Checked = false;
            }
        }


        private void rdBtnUser_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnDMode_Click(object sender, EventArgs e)
        {
            try
            {
                if (SendOnDevice("0080"))
                {
                    ownerr = this;
                    MessageBoxEx.Show(ownerr, "Device is in Download mode");

                    if (modObj != null)
                    {
                        if (modObj.IsSerialPortOpen())
                        {
                            modObj.CloseSerialPort();
                        }
                    }
                    else
                    {
                        modObj = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message);
            }
        }

        private void btnCMode_Click(object sender, EventArgs e)
        {
            if (SendOnDevice("0040"))
            {
                ownerr = this;
                MessageBoxEx.Show(ownerr, "Device is in Calibration mode");

                if (modObj != null)
                {
                    if (modObj.IsSerialPortOpen())
                    {
                        modObj.CloseSerialPort();
                    }
                }
                else
                {
                    modObj = null;
                }
            }
        }

        private bool SendOnDevice(string downloadAddress)
        {
            byte[] RecieveData = null;
            try
            {
                #region Port Settings
                string portNameN = SetValues.Set_PortName;
                int baudRateN = Convert.ToInt32(SetValues.Set_Baudrate);
                string parityN = SetValues.Set_parity;
                int bitsLengthN = SetValues.Set_BitsLength;
                int stopBitsN = Convert.ToInt32(SetValues.Set_StopBits);
                #endregion

                if (modObj == null)
                {
                    modObj = new clsModbus();
                    modObj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
                }
                else
                {
                    if (modObj.IsSerialPortOpen())
                        modObj.CloseSerialPort();

                    string wordCount = downloadAddress;
                    string regAddress = "1072";
                    string functionCode = "06";
                    string nodeAddress = Convert.ToString(txtB_NodeAddress.Value);

                    if (modObj.OpenSerialPort(portNameN, baudRateN, parityN, stopBitsN, bitsLengthN))
                    {
                        //send
                        if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                        {
                            if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1)
                            {
                                RecieveData = modObj.AscFrame(Convert.ToInt32(nodeAddress).ToString("X").PadLeft(2, '0'),
                                    functionCode, regAddress, wordCount.PadLeft(4, '0'));
                            }
                            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                            {
                                RecieveData = modObj.RtuFrame(Convert.ToInt32(nodeAddress).ToString("X").PadLeft(4, '0'),
                                    functionCode, regAddress, wordCount.PadLeft(4, '0'),SetValues.Set_Baudrate);
                            }


                        }
                        //reply
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message);
            }

            if (RecieveData != null)
                return true;
            else
                return false;
        }

        public class SettingClass
        {
          
            public string Comport { get; set; }
            public string Address { get; set; }
            public int Baudrate { get; set; }
            public string Parity { get; set; }
            public string Stopbits { get; set; }
            public int Bitslength { get; set; }
            public string Protocol { get; set; }

           
        }

        private void singlecmdtxt()
        {

        }

        private void FWUpgradation_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (modObj != null)
            {
                if (modObj.IsSerialPortOpen())
                {
                    modObj.CloseSerialPort();
                    modObj = null;
                }
            }
        }


    }
}