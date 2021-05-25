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
    public partial class DownloadForm : Form
    {
        SerialPort _serialPort = null;
        public bool looping = true;
        static BackgroundWorker _bw;

        protected const int TOLERANCE_BYTE_1 = 0x01;
        protected const int TOLARANCE_BYTE_2 = 0x02;
        protected const int TOLARANCE_BYTE_3 = 0x03;
        protected const int TOLARANCE_BYTE_4 = 0x04;
        protected const int SUCCESS = CommonConstants.SUCCESS;
        protected const int FAILURE = CommonConstants.FAILURE;
        protected const int ONLYBOOTBLOCK = CommonConstants.ONLYBOOTBLOCK;    

        public DownloadForm()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    SetValues.Set_SelectedPath = txtFirmwarePath.Text = openFileDialog1.FileName;
                }
            }
        }

        private void btnUpgrade1_Click(object sender, EventArgs e)
        {
            LogWriter.WriteToFile("FWUpgradation.cs - btn_Upgrade_Click", "================================================ Upgrade Start ================================================", "RTC_Upgrade");
            pgbDownload.Value = 0;
            lblDownloadProgress.Text = 0 + "%";

            if (!string.IsNullOrEmpty(SetValues.Set_SelectedPath) || !string.IsNullOrEmpty(txtFirmwarePath.Text))
            {
                if ((!string.IsNullOrEmpty(nud_NodeAddresss.Text)))
                {
                    lblMainMessage.Text = "Ready..";

                    if (Connect())
                    {
                        _bw = new BackgroundWorker
                        {
                            WorkerReportsProgress = true,
                            WorkerSupportsCancellation = true
                        };
                        _bw.DoWork += bw_DoWork;
                        _bw.ProgressChanged += bw_ProgressChanged;
                        _bw.RunWorkerCompleted += bw_RunWorkerCompleted;

                        _bw.RunWorkerAsync("Hello to worker");


                    }
                }
            }
            else
            {
                lblMainMessage.Text = "Select firmware file";
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (lblMainMessage.InvokeRequired)
                {
                    lblMainMessage.Invoke((System.Action)(() => lblMainMessage.Text = "You cancelled"));
                }
                lblMainMessage.Text = "You cancelled";
            }
            else if (e.Error != null)
            {
                if (lblMainMessage.InvokeRequired)
                {
                    lblMainMessage.Invoke((System.Action)(() => lblMainMessage.Text = e.Error.ToString()));
                }
                lblMainMessage.Text = "error";
            }
            else
            {
                if (lblMainMessage.InvokeRequired)
                {
                    lblMainMessage.Invoke((System.Action)(() => lblMainMessage.Text = "Download Completed."));
                }
                lblMainMessage.Text = "complete";
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (lblMainMessage.InvokeRequired)
            {
                if (e.ProgressPercentage > 0 && e.ProgressPercentage < 100)
                    lblMainMessage.Invoke((System.Action)(() => lblMainMessage.Text = "Downloading.."));
            }

            if (lblDownloadProgress.InvokeRequired)
                lblDownloadProgress.Invoke((System.Action)(() => lblDownloadProgress.Text = e.ProgressPercentage + "%"));
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 0;

            //for (int i = 0; i <= 100; i += 20)
            //{
            //    if (_bw.CancellationPending)
            //    {
            //        e.Cancel = true; return;
            //    }
            //_bw.ReportProgress(i);
            //Thread.Sleep(1000);

            Thread testThread = new Thread(() => InitialConnection());
            testThread.Start();

            if (lblMainMessage.InvokeRequired)
            {
                lblMainMessage.Invoke((System.Action)(() => lblMainMessage.Text = "Downloading.."));
            }

            if (lblDownloadProgress.InvokeRequired)
                lblDownloadProgress.Invoke((System.Action)(() => lblDownloadProgress.Text = i + "%"));

            if (pgbDownload.InvokeRequired)
                pgbDownload.Invoke((System.Action)(() => pgbDownload.Value = i));
            //}
        }

        public bool IsLooping()
        {
            lock (this)
            {
                return looping;
            }
        }

        private void InitialConnection()
        {
            if (_serialPort.IsOpen)
            {

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                if (_bw.IsBusy)
                    _bw.CancelAsync();

                _serialPort.DiscardInBuffer();
                _serialPort.Close();
                LogWriter.WriteToFile("Connect.cs - btnCancel_Click()", "port closed--", "RTC_Upgrade");
            }
            this.Close();
        }

        public bool Connect()
        {
            LogWriter.WriteToFile("Connect.cs - Connect()", "started--", "RTC_Upgrade");

            try
            {
                using (_serialPort = new SerialPort(SetValues.Set_PortName))
                {
                    _serialPort.WriteTimeout = 2000;
                    _serialPort.ReadTimeout = 0xFFFFFFF;

                    _serialPort.BaudRate = Convert.ToInt32(SetValues.Set_Baudrate);
                    try
                    {
                        switch (SetValues.Set_parity)
                        {
                            case "Even":
                                _serialPort.Parity = Parity.Even;
                                break;
                            case "Odd":
                                _serialPort.Parity = Parity.Odd;
                                break;
                            case "None":
                            default:
                                _serialPort.Parity = Parity.None;
                                break;
                        }
                    }
                    catch
                    {
                        _serialPort.Parity = Parity.None;
                    }

                    _serialPort.DataBits = Convert.ToInt32(SetValues.Set_BitsLength);
                    try
                    {
                        switch (SetValues.Set_StopBits)
                        {
                            case "1":
                                _serialPort.StopBits = StopBits.One;
                                break;
                            case "1.5":
                                _serialPort.StopBits = StopBits.OnePointFive;
                                break;
                            case "2":
                                _serialPort.StopBits = StopBits.Two;
                                break;
                            default:
                                _serialPort.StopBits = StopBits.None;
                                break;

                        }
                    }
                    catch
                    {
                        _serialPort.StopBits = StopBits.None;
                    }

                    LogWriter.WriteToFile("default settings: ", _serialPort.PortName + " " + _serialPort.BaudRate + " " +
                    _serialPort.Parity + " " + _serialPort.DataBits + " " + _serialPort.StopBits, "RTC_Upgrade");

                    if (!_serialPort.IsOpen)
                    {
                        LogWriter.WriteToFile("Connect() ", "Port " + _serialPort.IsOpen.ToString(), "RTC_Upgrade");
                        _serialPort.Open();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                LogWriter.WriteToFile(e.StackTrace, e.Message, "RTC_Upgrade");
                MessageBox.Show("Port is either busy or not exists!!", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _serialPort.Close();
                return false;
            }
        }

        //public int Plc_Download_ProtocolInitial_Update1(string pFileName, byte pFileID, byte NodeAddress)
        //{
        //    LogWriter.WriteToFile("DownloadForm.cs - Plc_Download_ProtocolInitial_Update1()", "started--", "RTC_Upgrade");
        //    byte[] _serialSendByte = new byte[2];
        //    byte[] _serialReceivedByte = new byte[2];

        //    byte[] _serialSendByte1 = new byte[4];
        //    byte[] _serialReceivedByte1 = new byte[64];

        //    byte temp_receivedbyte = 0;
        //    byte initialByte = 0x70;
        //    bool IsByteValid = false;

        //    _serialSendByte[0] = initialByte;
        //    _serialSendByte[1] = NodeAddress;

        //    for (int i = 0; i < 2; i++)
        //    {
        //        _serialPort.DiscardInBuffer();
        //        Thread.Sleep(10);
        //        try
        //        {
        //            _serialPort.Write(_serialSendByte, 0, 2);
        //            LogWriter.WriteToFile(" 1)   70=> ", string.Join(",", _serialSendByte), "RTC_Upgrade");

        //            int _serialNoOfByteRead = ReceiveFrame();

        //            if (_serialNoOfByteRead > 0)
        //            {
        //                LogWriter.WriteToFile(" 1)   71/73<= ", string.Join(",", _serialReceivedByte), "RTC_Upgrade");
        //                if (_serialReceivedByte == (initialByte + TOLERANCE_BYTE_1) || _serialReceivedByte == (initialByte + TOLARANCE_BYTE_3))
        //                {
        //                    IsByteValid = true;
        //                    break;
        //                }
        //                _serialPort.DiscardInBuffer();
        //                Thread.Sleep(7000);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LogWriter.WriteToFile(ex.StackTrace, ex.Message, "RTC_Upgrade");
        //            return FAILURE;
        //        }
        //    }

        //    if (!IsByteValid)
        //    {
        //        LogWriter.WriteToFile("Serial.cs-Plc_Download_ProtocolInitial_Update1()", "invalid  byte", "RTC_Upgrade");
        //        return FAILURE;
        //    }

        //    temp_receivedbyte = _serialReceivedByte;

        //    _serialSendbyte1.SetValue((Byte)0x52, 0);
        //    _serialSendbyte1.SetValue((Byte)0x45, 1);
        //    _serialSendbyte1.SetValue((Byte)0x50, 2);
        //    _serialSendbyte1.SetValue((Byte)0x4c, 3);

        //    LogWriter.WriteToFile(" 2)   REPL=> ", string.Join(",", _serialSendbyte1), "RTC_Upgrade");

        //    _serialPort.Write(_serialSendbyte1, 0, 4);
        //    Thread.Sleep(1000);
        //    _serialPort.ReadTimeout = 10000;

        //    while (true)
        //    {
        //        _serialRecvBuff[0] = 0;

        //        try
        //        {
        //            int _serialNoOfByteRead = _serialPort.Read(_serialRecvbyte, 0, 64);
        //            LogWriter.WriteToFile(" 2)   <= ", string.Join(",", _serialRecvbyte), "RTC_Upgrade");
        //        }
        //        catch (System.TimeoutException timeOut)
        //        {
        //            LogWriter.WriteToFile(timeOut.StackTrace, timeOut.Message, "RTC_Upgrade");
        //            return FAILURE;
        //        }
        //        catch (Exception er)
        //        {
        //            LogWriter.WriteToFile(er.StackTrace, er.Message, "RTC_Upgrade");
        //            return FAILURE;
        //        }

        //        if (_serialNoOfByteRead > 0)
        //        {
        //            if (temp_receivedbyte == (initialByte + TOLERANCE_BYTE_1))
        //            {
        //                LogWriter.WriteToFile("Serial.cs - Plc_Download_ProtocolInitial_Update1()", " 64 <= bootblock", "RTC_Upgrade");
        //                return ONLYBOOTBLOCK;
        //            }
        //            else if (temp_receivedbyte == (initialByte + TOLARANCE_BYTE_3))
        //            {
        //                LogWriter.WriteToFile("Serial.cs - Plc_Download_ProtocolInitial_Update1()", " 64 <= run", "RTC_Upgrade");
        //                return SUCCESS;
        //            }
        //        }
        //        else
        //        {
        //            return FAILURE;
        //        }
        //    }
        //    return FAILURE;
        //}

        private int ReceiveFrame()
        {
            return 0;
        }

    }
}
