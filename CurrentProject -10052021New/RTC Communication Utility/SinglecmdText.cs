using ClassList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTC_Communication_Utility
{
    public partial class SinglecmdText : Form
    {
        //ModbusRTU modbusobj = null;
        clsModbus modbusobj = null;
        Thread th = null;
        private readonly BackgroundWorker worker;
        bool isRunning = false;
      //  private ClassList.Serial serialObj = new ClassList.Serial();
        public SinglecmdText()
        {

            //modbusobj = new ModbusRTU();
            modbusobj = new clsModbus();

            SetValues.Set_Form = "SinglecmdText";
            InitializeComponent();
            //modbusobj._GetLRCResultResult += new ModbusRTU.GetLRCResult(singlecmdtxt);
           
            modbusobj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 1;
            object userState = "";

            bool isRun = true;

            do
            {
                string portName = SetValues.Set_PortName;
                string baudRate = SetValues.Set_Baudrate;
                string parity = SetValues.Set_parity;
                int bitsLength = SetValues.Set_BitsLength;
                string stopBits = SetValues.Set_StopBits;

                try
                {

                    if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength))
                    {
                        byte[] RecieveData = null;

                        if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                        {
                            if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                            {
                                RecieveData = modbusobj.AscFrame((Convert.ToInt32(SetValues.Set_UnitAddress, 16)).ToString(), SetValues.Set_CommandType,
                                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                                if (RecieveData != null && RecieveData.Length > 0)
                                {
                                    string result = modbusobj.DisplayFrame(RecieveData);

                                    char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();

                                    if (recdata != null)
                                    {
                                        userState = recdata;

                                        txtBxRecievecmd.Invoke((Action)(() => txtBxRecievecmd.Text = string.Join("", userState)));

                                        worker.ReportProgress(i, userState);
                                    }

                                    if (worker.CancellationPending)
                                    {
                                        e.Cancel = true;
                                        worker.ReportProgress(0);
                                        //return;
                                        isRun = false;
                                    }
                                    i++;
                                }
                                else
                                {
                                    txtBxRecievecmd.Invoke((Action)(() => txtBxRecievecmd.Text = "Received time-out"));
                                }
                            }
                            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                            {
                                RecieveData = modbusobj.RtuFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                                    SetValues.Set_RegAddress, SetValues.Set_WordCount,SetValues.Set_Baudrate);

                                if (RecieveData != null && RecieveData.Length > 0)
                                {
                                    string result = modbusobj.DisplayFrame(RecieveData);

                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        userState = result;

                                        txtBxRecievecmd.Invoke((Action)(() => txtBxRecievecmd.Text = result));

                                        worker.ReportProgress(i, userState);
                                    }

                                    if (worker.CancellationPending)
                                    {
                                        e.Cancel = true;
                                        worker.ReportProgress(0);
                                        //return;
                                        isRun = false;
                                    }
                                    i++;
                                }
                                else
                                {
                                    txtBxRecievecmd.Invoke((Action)(() => txtBxRecievecmd.Text = "Received time-out"));
                                }
                            }


                        }
                    }

                }
                catch (Exception ex)
                {

                    // lblMessage.Invoke((Action)(() => lblMessage.Text = ex.Message));
                    // MessageBox.Show("Exception worker: " + ex.StackTrace);
                }
                finally
                {
                    if (modbusobj != null)
                    {
                        modbusobj.CloseSerialPort();
                    }
                }
            } while (isRun);

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                lblMessage.Invoke((Action)(() => lblMessage.Text = "Cancelled/Stopped"));
                 clsModbus.Singlecmdsleep = false;
            }
            else if (e.Error != null)
            {
                lblMessage.Invoke((Action)(() => lblMessage.Text = "Error occurred"));
            }
            else
            {
                lblMessage.Invoke((Action)(() => lblMessage.Text = "Done!!"));
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
            {
                if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                {
                    if (worker.CancellationPending)
                    {
                        if (e.UserState != null)
                        {
                            txtBxRecievecmd.Text = "";
                            txtBxRecievecmd.Text = Convert.ToString(string.Join("", (char[])e.UserState));
                            txtBxRecievecmd.Refresh();
                        }
                    }
                    else
                    {
                        txtBxRecievecmd.Text = "";
                        txtBxRecievecmd.Text = Convert.ToString(string.Join("", (char[])e.UserState));
                        txtBxRecievecmd.Refresh();
                    }
                }
                else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex == 2)
                {
                    if (worker.CancellationPending)
                    {
                        if (e.UserState != null)
                        {
                            txtBxRecievecmd.Text = "";
                            txtBxRecievecmd.Text = Convert.ToString(e.UserState);
                            txtBxRecievecmd.Refresh();
                        }
                    }
                    else
                    {
                        txtBxRecievecmd.Text = "";
                        txtBxRecievecmd.Text = Convert.ToString(e.UserState);
                        txtBxRecievecmd.Refresh();
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            SetValuesSingleLine.Addr = txtbxAddress.Text;
            SetValuesSingleLine.Cmd = cmbBx_command.SelectedIndex.ToString();
            SetValuesSingleLine.Func = txtBx_FuncAddress.Text;
            SetValuesSingleLine.WordC = txtBx_WordCount_Writedata.Text;

            isRunning = false;

            if (th != null)
            {
                if (th.IsAlive && th.ThreadState == ThreadState.Running)
                {
                    th.Abort();
                }
            }

            if (modbusobj.IsSerialPortOpen())
            {
                modbusobj.CloseSerialPort();
                modbusobj = null;
            }
            this.Close();
        }

        private void SinglecmdText_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            btnSend.Enabled = false;

            rdBtnSet.Visible = false;
            rdBtnReset.Visible = false;

            grpBxImage.Visible = true;
            grpBxWriteReg.Visible = false;

            cmbBx_command.DataSource = BasicProperties.commands;
            //cmbBx_command.SelectedIndex = 0;

            txtbxAddress.Text = SetValuesSingleLine.Addr;
            cmbBx_command.SelectedIndex = Convert.ToInt32(SetValuesSingleLine.Cmd);
            txtBx_FuncAddress.Text = SetValuesSingleLine.Func;
            txtBx_WordCount_Writedata.Text = SetValuesSingleLine.WordC;

            //txtBxLRC.Text = "";
            //txtBxSendCommnd.Text = "";

            if (string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol) ||
                SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
            {
                label21.Text = "LRC";
            }
            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
            {
                label21.Text = "CRC";
            }

        }

        private void txtbxAddress_TextChanged(object sender, EventArgs e)
        {
            string untaddress = string.Empty;
            string address = txtbxAddress.Text;

            if (string.IsNullOrEmpty(address))
            {
                txtBxSendCommnd.Text = "";
                txtBxLRC.Text = "";
            }
            else
            {
                SetValues.Set_UnitAddress = "00";
                if (address.Length > 0)
                {
                    SetValues.Set_UnitAddress = SetValues.Set_CommunicationProtocol.ToLower() == "ascii" || SetValues.Set_CommunicationProtocolindex == 1
                        ? txtbxAddress.Text.PadLeft(2, '0') : Convert.ToString(Convert.ToInt32(txtbxAddress.Text, 16)).PadLeft(2, '0');
                }
                ValidData();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (modbusobj != null)
            {
                if (modbusobj.IsSerialPortOpen())
                {
                    modbusobj.CloseSerialPort();
                }
            }
            else
            {
                modbusobj = new clsModbus();
                modbusobj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
            }
            lblMessage.Text = "";
            txtBxRecievecmd.Text = "";
            txtBxRecievecmd.Refresh();
            if (string.IsNullOrEmpty(txtBxSendCommnd.Text))
            {
                lblMessage.Text = "Please enter all fields.";
            }
            else
            {
                btnSend.Enabled = false;

                //Task<bool> task = new Task<bool>(SendFrameToDevice1);
                //task.Start();

                //bool status = await task;
                clsModbus.Singlecmdsleep = true;
                bool sent = SendFrameToDevice1();

            }
            btnSend.Enabled = true;
            StoreDefaultValues();
        }

        private static void StoreDefaultValues()
        {
            string portName = SetValues.Set_PortName;
            string baudRate = SetValues.Set_Baudrate;
            string parity = SetValues.Set_parity;
            int bitsLength = SetValues.Set_BitsLength;
            string stopBits = SetValues.Set_StopBits;

            if (!string.IsNullOrEmpty(portName) && !string.IsNullOrEmpty(baudRate) && !string.IsNullOrEmpty(parity) &&
                bitsLength != 0 && !string.IsNullOrEmpty(stopBits))
            {

            }

        }

        private bool SendFrameToDevice1()
        {
            string portName = SetValues.Set_PortName;
            string baudRate = SetValues.Set_Baudrate;
            string parity = SetValues.Set_parity;
            int bitsLength = SetValues.Set_BitsLength;
            string stopBits = SetValues.Set_StopBits;
            ///test 
            //string response = ":010610720080F7\r\n";
            if (!string.IsNullOrEmpty(portName) && !string.IsNullOrEmpty(baudRate) && !string.IsNullOrEmpty(parity) &&
            bitsLength > 0 && !string.IsNullOrEmpty(stopBits))
            {
                try
                {
                    if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength))
                    {

                        byte[] RecieveData = null;

                        if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                        {
                            if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                            {
                                RecieveData = modbusobj.AscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                                    SetValues.Set_RegAddress, SetValues.Set_WordCount);
                            }
                            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex == 2)
                            {
                                RecieveData = modbusobj.RtuFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                                    SetValues.Set_RegAddress, SetValues.Set_WordCount,SetValues.Set_Baudrate);
                            }

                            if (RecieveData != null && RecieveData.Length > 0)
                            {
                                char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();

                                string result = "";

                                if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                                {
                                    result = string.Join("", recdata);
                                }
                                else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex == 2)
                                {
                                    result = modbusobj.DisplayFrame(RecieveData);
                                }

                                txtBxRecievecmd.Text = result;
                                ///test
                                //if (txtBxRecievecmd.Text == response)
                                //{
                                //    MessageBox.Show("working");
                                //    serialObj.ConnectForMoveEBit();
                                   
                                //}
                                //else 
                                //{

                                //}
                                double int64Val = 0;
                                returnVal.Text = int64Val.ToString();
                                return true;
                            }
                            else
                            {
                                // data not received
                                txtBxRecievecmd.Text = "Received Data Timeout!";
                                return true;
                            }
                        }
                        else
                        {
                            txtBxRecievecmd.Text = "Received Data Timeout!";
                        }
                    }
                    else
                    {
                        txtBxRecievecmd.Text = "Connection failed";
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                    //LogWriter.WriteToFile("ModbusRTU: 5)", ex.Message, "DTC_ErrorLog");
                    //lblMessage.Text = "3" + ex.Message;
                    return false;
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
            return false;

        }

        private void SendFrameToDevice2()
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

                        if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol) && !string.IsNullOrEmpty(txtBxSendCommnd.Text))
                        {
                            byte[] send = Encoding.ASCII.GetBytes(txtBxSendCommnd.Text);
                            if (send != null)
                            {
                                RecieveData = modbusobj.SendFrame(send,baudRate);

                                if (RecieveData != null && RecieveData.Length > 0)
                                {
                                    char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();

                                    string result = "";

                                    if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                                    {
                                        result = string.Join("", recdata);
                                    }
                                    else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex == 2)
                                    {
                                        result = modbusobj.DisplayFrame(RecieveData);
                                    }

                                    txtBxRecievecmd.Text = result;

                                    double int64Val = 0;
                                    returnVal.Text = int64Val.ToString();
                                }
                                else
                                {
                                    // data not received
                                    txtBxRecievecmd.Text = "Received Data Timeout!";
                                }
                            }
                            else
                            {
                                // send data null
                                txtBxRecievecmd.Text = "Received Data Timeout!";
                            }
                        }
                        else
                        {
                            txtBxRecievecmd.Text = "Received Data Timeout!";
                        }
                    }
                    else
                        txtBxRecievecmd.Text = "Connection failed";
                }
                catch (Exception ex)
                {
                    //LogWriter.WriteToFile("ModbusRTU: 5)", ex.Message, "DTC_ErrorLog");
                    lblMessage.Text = "3" + ex.Message;
                }
            }
            else
            {
                // settings are empty
            }
        }

        private void SendFrameToDevice()
        {
            string portName = SetValues.Set_PortName;
            string baudRate = SetValues.Set_Baudrate;
            string parity = SetValues.Set_parity;
            int bitsLength = SetValues.Set_BitsLength;
            string stopBits = SetValues.Set_StopBits;

            try
            {
                if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength))
                {
                    byte[] RecieveData = null;

                    if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                    {
                        if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                        {
                            RecieveData = modbusobj.AscFrame((Convert.ToInt32(SetValues.Set_UnitAddress, 16)).ToString(), SetValues.Set_CommandType,
                                SetValues.Set_RegAddress, SetValues.Set_WordCount);
                        }
                        else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu" || SetValues.Set_CommunicationProtocolindex == 2)
                        {
                            RecieveData = modbusobj.RtuFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                                SetValues.Set_RegAddress, SetValues.Set_WordCount,SetValues.Set_Baudrate);
                        }

                        if (RecieveData != null && RecieveData.Length > 0)
                        {
                            string result = modbusobj.DisplayFrame(RecieveData);

                            if (txtBxRecievecmd.InvokeRequired)
                            {
                                txtBxRecievecmd.Invoke((Action)(() =>
                                {
                                    txtBxRecievecmd.Text = result;
                                    txtBxRecievecmd.Refresh();
                                }));
                            }
                        }
                        else
                        {
                            if (txtBxRecievecmd.InvokeRequired)
                            {
                                txtBxRecievecmd.Invoke((Action)(() =>
                                {
                                    txtBxRecievecmd.Text = "Received Data Timeout!";
                                    txtBxRecievecmd.Refresh();
                                }));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "1" + ex.Message;
            }
        }

        private void btnRepeat_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            if (string.IsNullOrEmpty(txtBxSendCommnd.Text))
            {
                lblMessage.Text = "Please enter all fields";
            }
            else
            {
                try
                {
                    if (btnRepeat.Text == "Repeat")
                    {
                        //th = new Thread(StartRepeat);
                        if (worker.IsBusy != true)
                        {
                            btnRepeat.Text = "Stop";
                            worker.RunWorkerAsync();
                        }
                        //clsModbus.Singlecmdrepeat = true;
                        clsModbus.Singlecmdsleep = true;
                        btnRepeat.Text = "Stop";
                        btnClose.Enabled = false;
                        ControlBox = false;

                        //th.Start();

                    }
                    else if (btnRepeat.Text == "Stop")
                    {
                        if (worker.IsBusy)
                        {
                            worker.CancelAsync();
                        }
                        
                        //clsModbus.Singlecmdrepeat = true;
                        //clsModbus.Singlecmdrepeat = false;
                        btnRepeat.Text = "Repeat";
                        btnClose.Enabled = true;
                        ControlBox = true;
                        Thread.Sleep(170);
                        //th.Abort();
                    }
                }
                catch (Exception ex)
                {
                    //lblMessage.Text = "2" + ex.Message;
                }
                finally
                {
                    if (modbusobj != null)
                    {
                        modbusobj.CloseSerialPort();
                    }
                    StoreDefaultValues();
                    
                }


            }
        }

        private void StartRepeat()
        {
            try
            {
                bool repeat = true;

                MessageBox.Show("Started");

                while (repeat)
                {
                    SendFrameToDevice1();

                    if (th.ThreadState == ThreadState.AbortRequested)
                    {
                        // stop
                        repeat = false;
                        MessageBox.Show("Aborted");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBxRecievecmd.Text = "";
        }

        private void cmbBx_command_SelectedIndexChanged(object sender, EventArgs e)
        {

            grpBxImage.Visible = true;
            grpBxWriteReg.Visible = false;

            rdBtnSet.Visible = false;
            rdBtnReset.Visible = false;

            txtBxSendCommnd.Text = "";
            txtBxSendCommnd.Refresh();

            txtBxLRC.Text = "";
            txtBxLRC.Refresh();
            txtBx_WordCount_Writedata.Visible = true;

            switch (cmbBx_command.SelectedIndex)
            {
                case 0:
                    label22.Text = "Word Count";
                    txtBx_WordCount_Writedata.MaxLength = 0;
                    break;
                case 1://Read Register
                    SetValues.Set_CommandType = "03";
                    label22.Text = "Word Count";
                    txtBx_WordCount_Writedata.MaxLength = 1;
                    break;
                case 2://Write One Word
                    SetValues.Set_CommandType = "06";
                    label22.Text = "Write Data";
                    txtBx_WordCount_Writedata.MaxLength = 4;
                    break;
                case 3://Write Multi Words
                    SetValues.Set_CommandType = "16";
                    label22.Text = "Word Count";
                    txtBx_WordCount_Writedata.MaxLength = 1;
                    grpBxImage.Visible = false;
                    grpBxWriteReg.Visible = true;
                    break;
                case 4://Read Bits
                    SetValues.Set_CommandType = "01";
                    label22.Text = "Bit Count";
                    txtBx_WordCount_Writedata.MaxLength = 2;
                    break;
                case 5://Write One Bit into Reg
                    SetValues.Set_CommandType = "05";
                    label22.Text = "Content";
                    txtBx_WordCount_Writedata.Visible = false;
                    rdBtnSet.Visible = true;
                    rdBtnReset.Visible = true;

                    txtBx_WordCount_Writedata.MaxLength = 4;
                    break;
            }

            int maxLen = txtBx_WordCount_Writedata.MaxLength;

            if (string.IsNullOrEmpty(txtBx_WordCount_Writedata.Text))
            {
                txtBx_WordCount_Writedata.Text = "0";
            }

            if (cmbBx_command.SelectedIndex == 5)
            {
                txtBx_WordCount_Writedata.Text = "";

                if (rdBtnSet.Checked)
                    txtBx_WordCount_Writedata.Text = "FF00";

                if (rdBtnReset.Checked)
                    txtBx_WordCount_Writedata.Text = "0000";
            }
            else
            {
                txtBx_WordCount_Writedata.Text = string.Format("{0:00}", "".PadLeft(maxLen, '0'));
            }

            ValidData();
        }

        private void txtBx_WordCount_Writedata_TextChanged(object sender, EventArgs e)
        {
            string word = txtBx_FuncAddress.Text;

            if (string.IsNullOrEmpty(word))
            {
                txtBxSendCommnd.Text = "";
                txtBxLRC.Text = "";
            }
            else
            {
                SetValues.Set_WordCount = word.PadLeft(4, '0');
                ValidData();
            }
        }

        public void singlecmdtxt()
        {
            try
            {
                txtBxLRC.Text = SetValues.Set_LRCFrame;
                txtBxSendCommnd.Text = SetValues.Set_ASKFrame;// +SetValues.Set_LRCFrame;
            }
            catch (Exception ae)
            {
                MessageBox.Show(ae.ToString());
            }
        }

        private void txtBx_FuncAddress_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string functionAddress = txtBx_FuncAddress.Text;
                if (string.IsNullOrEmpty(functionAddress))
                {
                    txtBxLRC.Text = "";
                    txtBxSendCommnd.Text = "";
                    SetValues.Set_RegAddress = functionAddress.PadLeft(4, '0');
                    //MessageBox.Show("Invalid");
                }
                else
                {
                    Regex rgx = new Regex("[^A-F0-9]+");
                    if (rgx.IsMatch(functionAddress))
                    {
                        MessageBox.Show("Special charactes are not allowed in Tags");
                    }
                    else
                    {
                        SetValues.Set_RegAddress = functionAddress.PadLeft(4, '0');
                        ValidData();
                       // MessageBox.Show("valid");
                    }
                }
            }
            catch (Exception ae)
            {
                MessageBox.Show(ae.ToString());
            }
        }

        public void ValidData()
        {
            try
            {
                byte[] buff = null;
              //  MessageBox.Show("ValidData main");
                if (!string.IsNullOrEmpty(SetValues.Set_WordCount) &&
                    !string.IsNullOrEmpty(SetValues.Set_CommandType) &&
                    !string.IsNullOrEmpty(SetValues.Set_UnitAddress) &&
                    !string.IsNullOrEmpty(SetValues.Set_RegAddress) &&
                    !string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol) &&
                    cmbBx_command.SelectedIndex != 0)
                {
                    string portName = SetValues.Set_PortName;
                    string baudRate = SetValues.Set_Baudrate;
                    string parity = SetValues.Set_parity;
                    int bitsLength = SetValues.Set_BitsLength;
                    string stopBits = SetValues.Set_StopBits;

                    //if (modbusobj.IsSerialPortOpen())
                    //{
                    //    modbusobj.CloseSerialPort();
                    //}

                    //if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength))
                    //{
                  
                    if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                    {
                        
                        buff = modbusobj.AscFrameSingle2(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                            SetValues.Set_RegAddress, SetValues.Set_WordCount);
                    }
                    else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex == 2)
                    {
                        buff = modbusobj.RtuFrameSingle(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                            SetValues.Set_RegAddress, SetValues.Set_WordCount);
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void txtBxSendCommnd_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBxSendCommnd.Text))
            { 
               // MessageBox.Show(txtBxSendCommnd.Text);
                btnSend.Enabled = true;
            }
            else
            {
                btnSend.Enabled = false;
              //  MessageBox.Show(txtBxSendCommnd.Text);
            }
            txtBxRecievecmd.Text = "";
        }

        private void txtBx_WordCount_Writedata_TextChanged_1(object sender, EventArgs e)
        {
            string txt = string.IsNullOrEmpty(txtBx_WordCount_Writedata.Text) ? "0" : txtBx_WordCount_Writedata.Text;
            string word = string.Format("{0:00}", txt.PadLeft(4, '0'));

            if (SetValues.Set_CommandType == "10")
            {
                SetValues.Set_Words1 = (Convert.ToInt32(txt) * 2).ToString("X").PadLeft(2, '0');
                GetWords();
            }

            SetValues.Set_WordCount = word;

            ValidData();
        }

        // write multi words
        private string GetWords()
        {
            string words = "";
            string txt = string.IsNullOrEmpty(txtBx_WordCount_Writedata.Text) ? "0" : txtBx_WordCount_Writedata.Text;
            for (int i = 1; i <= Convert.ToInt32(txt); i++)
            {
                switch (i)
                {
                    case 1:
                        words += string.Format("{0:00}", txtWrite1.Text.PadLeft(4, '0'));
                        break;
                    case 2:
                        words += string.Format("{0:00}", txtWrite2.Text.PadLeft(4, '0'));
                        break;
                    case 3:
                        words += string.Format("{0:00}", txtWrite3.Text.PadLeft(4, '0'));
                        break;
                    case 4:
                        words += string.Format("{0:00}", txtWrite4.Text.PadLeft(4, '0'));
                        break;
                    case 5:
                        words += string.Format("{0:00}", txtWrite5.Text.PadLeft(4, '0'));
                        break;
                    case 6:
                        words += string.Format("{0:00}", txtWrite6.Text.PadLeft(4, '0'));
                        break;
                    case 7:
                        words += string.Format("{0:00}", txtWrite7.Text.PadLeft(4, '0'));
                        break;
                    case 8:
                        words += string.Format("{0:00}", txtWrite8.Text.PadLeft(4, '0'));
                        break;
                    case 9:
                        words += "0000";
                        break;
                }
            }
            SetValues.Set_Words = words;
            return words;
        }

        private void txtWrite1_TextChanged(object sender, EventArgs e)
        {
            GetWords();
            ValidData();
        }

        private void txtWrite2_TextChanged(object sender, EventArgs e)
        {
            GetWords();
            ValidData();
        }

        private void txtWrite3_TextChanged(object sender, EventArgs e)
        {
            GetWords();
            ValidData();
        }

        private void txtWrite4_TextChanged(object sender, EventArgs e)
        {
            GetWords();
            ValidData();
        }

        private void txtWrite5_TextChanged(object sender, EventArgs e)
        {
            GetWords();
            ValidData();
        }

        private void txtWrite6_TextChanged(object sender, EventArgs e)
        {
            GetWords();
            ValidData();
        }

        private void txtWrite7_TextChanged(object sender, EventArgs e)
        {
            GetWords();
            ValidData();
        }

        private void txtWrite8_TextChanged(object sender, EventArgs e)
        {
            GetWords();
            ValidData();
        }

        private void rdBtnReset_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnReset.Checked)
                txtBx_WordCount_Writedata.Text = "0000";

            ValidData();
        }

        private void rdBtnSet_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnSet.Checked)
                txtBx_WordCount_Writedata.Text = "FF00";

            ValidData();
        }

        private void SinglecmdText_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void SinglecmdText_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (modbusobj != null)
                {
                    if (modbusobj.IsSerialPortOpen())
                    {
                        modbusobj.CloseSerialPort();
                        modbusobj = null;
                    }
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnRepeatOne_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnRepeatOne.Text == "Repeat One")
                {
                    #region Create Thread
                    if (th == null)
                    {
                        th = new Thread(ReadThread);
                    }
                    #endregion
                    th.IsBackground = true;
                    th.Start();

                    isRunning = true;
                    btnRepeatOne.Text = "Stop";
                }
                else
                {
                    isRunning = false;
                    btnRepeatOne.Text = "Repeat One";
                    if (th != null)
                    {
                        th.Abort();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

                if (modbusobj.IsSerialPortOpen())
                {
                    modbusobj.CloseSerialPort();
                }

                if (th != null)
                {
                    th = null;
                }
            }
        }

        private void ReadThread()
        {
            while (isRunning)
            {
                string portName = SetValues.Set_PortName;
                string baudRate = SetValues.Set_Baudrate;
                string parity = SetValues.Set_parity;
                int bitsLength = SetValues.Set_BitsLength;
                string stopBits = SetValues.Set_StopBits;

                if (!string.IsNullOrEmpty(portName) && !string.IsNullOrEmpty(baudRate) &&
                    !string.IsNullOrEmpty(parity) &&
                bitsLength > 0 && !string.IsNullOrEmpty(stopBits))
                {
                    try
                    {
                        if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength))
                        {
                            byte[] RecieveData = null;

                            if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                            {
                                if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                                {
                                    RecieveData = modbusobj.AscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                                        SetValues.Set_RegAddress, SetValues.Set_WordCount);
                                }
                                else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex == 2)
                                {
                                    RecieveData = modbusobj.RtuFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                                        SetValues.Set_RegAddress, SetValues.Set_WordCount,SetValues.Set_Baudrate);
                                }

                                if (RecieveData != null && RecieveData.Length > 0)
                                {
                                    char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();

                                    string result = "";

                                    if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                                    {
                                        result = string.Join("", recdata);
                                    }
                                    else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex == 2)
                                    {
                                        result = modbusobj.DisplayFrame(RecieveData);
                                    }

                                    if (txtBxRecievecmd.InvokeRequired)
                                    {
                                        txtBxRecievecmd.Invoke((Action)(() =>
                                        {
                                            txtBxRecievecmd.Text = "";
                                            txtBxRecievecmd.Text =
                                            string.Format("{0}", result);
                                        }));
                                    }


                                    double int64Val = 0;
                                    returnVal.Text = int64Val.ToString();

                                }
                                else
                                {
                                    if (txtBxRecievecmd.InvokeRequired)
                                    {
                                        txtBxRecievecmd.Invoke((Action)(() =>
                                        {
                                            txtBxRecievecmd.Text = string.Format("Received Data Timeout!");                   
                                        }));
                                    }
                                }
                            }
                            else
                            {
                                if (txtBxRecievecmd.InvokeRequired)
                                {
                                    txtBxRecievecmd.Invoke((Action)(() =>
                                    {
                                        txtBxRecievecmd.Text =
                                        "Received Data Timeout!";
                                    }));
                                }
                            }
                        }
                        else
                        {
                            if (txtBxRecievecmd.InvokeRequired)
                            {
                                txtBxRecievecmd.Invoke((Action)(() =>
                                {
                                    txtBxRecievecmd.Text =
                                    "Connection failed!!!";
                                }));
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.StackTrace);
                        //LogWriter.WriteToFile("ModbusRTU: 5)", ex.Message, "DTC_ErrorLog");
                        //lblMessage.Text = "3" + ex.Message;

                    }

                }
                else
                {
                    // settings are empty
                }
            }
        }

        private void RepeatOne()
        {

        }
    }
}
