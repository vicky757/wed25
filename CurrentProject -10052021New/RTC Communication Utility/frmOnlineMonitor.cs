using ClassList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RTC_Communication_Utility
{
    public partial class frmOnlineMonitor : Form
    {
        clsModbus modbusobj = null;
        Thread thread = null;
        ChartArea chart;
        double vallCount = 0;
        VerticalLineAnnotation v1;
        RectangleAnnotation r1, r2;
        private ManualResetEvent _manualResetEvent = new ManualResetEvent(true);
        bool fullyLoaded = true;
        bool loopbreak = false;

        bool ctrlVal = true;
        bool setVal = true;
        bool runVal = true;
        bool out1Val = true;
        bool out2Val = true;

        bool senseVal = true;
        bool unitVal = true;
        bool highVal = true;
        bool lowVal = true;

        bool alM1Val = true;
        bool alM1UVal = true;
        bool alM1DVal = true;
        bool alM2Val = true;
        bool alM2UVal = true;
        bool alM2DVal = true;

        bool hys1V = true;
        bool hys2V = true;
        bool deadV = true;

        bool out1V = true;
        bool out2V = true;
        bool ctrl1V = true;
        bool ctrl2V = true;

        bool pdV = true;
        bool tiV = true;
        bool tdV = true;
        bool coefV = true;
        bool offVal = true;

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
        int timerCount = 0;
        public frmOnlineMonitor()
        {
            InitializeComponent();

            //menuStrip1.Items[0].Enabled = false;
            //menuStrip1.Items[1].Enabled = false;
            //menuStrip1.Items[2].Enabled = false;

            modbusobj = new clsModbus();
            modbusobj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
        }

        private void singlecmdtxt()
        {

        }

        private void btnConnect1_Click(object sender, EventArgs e)
        {
            if (modbusobj != null)
            {
                if (modbusobj.IsSerialPortOpen())
                {
                    modbusobj.CloseSerialPort();
                }
            }
            circularProgressBar1.Minimum = 0;
            circularProgressBar1.Maximum = 98;

            if (btnConnect1.Text == "Connect")
            {
                btnConnect1.BackColor = Color.YellowGreen;
                try
                {
                    if (thread == null)
                    {
                        thread = new Thread(() =>
                        {
                            int i = 0;
                            while (true)
                            {
                                if (fullyLoaded)
                                {
                                    circularProgressBar1.Invoke((Action)(() =>
                                    {
                                        circularProgressBar1.Visible = true;
                                        circularProgressBar1.Value = (i * 7);
                                    }));
                                    if (i == 14)
                                    {
                                        circularProgressBar1.Invoke((Action)(() =>
                                        {
                                            circularProgressBar1.Visible = false;
                                            circularProgressBar1.Value = 0;
                                        }));
                                        grpBxParameters.Invoke((Action)(() => grpBxParameters.Visible = true));
                                    }
                                }
                                else
                                {
                                    circularProgressBar1.Invoke((Action)(() =>
                                    {
                                        circularProgressBar1.Visible = false;
                                        circularProgressBar1.Value = 0;
                                    }));
                                    grpBxParameters.Invoke((Action)(() => grpBxParameters.Visible = true));
                                }

                                if (i < 14)
                                {

                                    //enable menus
                                    menuStrip1.Invoke((Action)(() =>
                                    {
                                        menuStrip1.Items[0].Enabled = true;
                                        menuStrip1.Items[1].Enabled = true;
                                        menuStrip1.Items[2].Enabled = true;
                                    }));



                                    List<string> list = frameList[i];
                                    List<string> val = CreateFrames(nodeAddress1.Value.ToString(), list[1], list[2], list[3], true);
                                    if (i == 1 && val == null)
                                    {
                                        loopbreak = true;
                                        break;
                                    }
                                    else
                                    {
                                        btnConnect1.BackColor = Color.OrangeRed;
                                    }
                                    if (val != null && val.Count > 0)
                                    {
                                        switch (i)
                                        {
                                            #region cases

                                            case 1:
                                                //471A
                                                if (out1Val)
                                                {
                                                    CmbBx1stout.Invoke((Action)(() =>
                                                    {
                                                        int vall = Convert.ToInt32(ConvertHexToShort(val[0], true));
                                                        CmbBx1stout.SelectedIndex = vall;
                                                        //switch (vall)
                                                        //{
                                                        //    case 0://heat
                                                        //        ModifyProgressBarColor.SetState(progressBar1, 2);
                                                        //        break;
                                                        //    case 1://cool
                                                        //        ModifyProgressBarColor.SetState(progressBar1, 1);
                                                        //        break;
                                                        //    case 2://Alarm
                                                        //        ModifyProgressBarColor.SetState(progressBar1, 0);
                                                        //        break;

                                                        //}
                                                    }));
                                                }

                                                //471B
                                                if (ctrl1V)
                                                {
                                                    //txtBxCtrlPeriod1.Invoke((Action)(() => txtBxCtrlPeriod1.Text =
                                                    //    (Convert.ToDouble(Convert.ToInt16(val[1], 16)) / DecimalPlaces()).ToString()));

                                                    txtBxCtrlPeriod1.Invoke((Action)(() => txtBxCtrlPeriod1.Text =
                                                        PlaceDecimal(ConvertHexToShort(val[1], true))));

                                                    txtBxCtrlPer1.Invoke((Action)(() => txtBxCtrlPer1.Text =
                                                       PlaceDecimal(ConvertHexToShort(val[1], true))));
                                                }


                                                //471C
                                                if (ctrl2V)
                                                {
                                                    txtBxCtrlPeriod2.Invoke((Action)(() => txtBxCtrlPeriod2.Text =
                                                        PlaceDecimal(ConvertHexToShort(val[2], true))));

                                                    txtBxctrlPer2.Invoke((Action)(() => txtBxctrlPer2.Text =
                                                        PlaceDecimal(ConvertHexToShort(val[2], true))));
                                                }
                                                //471D
                                                //471E
                                                //471F
                                                if (out2Val)
                                                {
                                                    CmbBx2ndout.Invoke((Action)(() =>
                                                    {
                                                        int vall = Convert.ToInt32(ConvertHexToShort(val[5], true));
                                                        CmbBx2ndout.SelectedIndex = vall;
                                                        //switch (vall)
                                                        //{
                                                        //    case 0://heat
                                                        //        ModifyProgressBarColor.SetState(progressBar2, 2);
                                                        //        break;
                                                        //    case 1://cool
                                                        //        ModifyProgressBarColor.SetState(progressBar2, 1);
                                                        //        break;
                                                        //    case 2://Alarm
                                                        //        ModifyProgressBarColor.SetState(progressBar2, 0);
                                                        //        break;

                                                        //}
                                                    }));
                                                }
                                                break;
                                            case 2:
                                                //4700
                                                txtPV1.Invoke((Action)(() => txtPV1.Text = PlaceDecimal(ConvertHexToShort(val[0], false))));

                                                //4701
                                                txtSV1.Invoke((Action)(() => txtSV1.Text = PlaceDecimal(ConvertHexToShort(val[1], false))));
                                                if (setVal == true)
                                                {
                                                    txtBxSetvalue.Invoke((Action)(() => txtBxSetvalue.Text = PlaceDecimal(ConvertHexToShort(val[1], false))));
                                                }

                                                //4702
                                                double out1 = ConvertHexToShort(val[2], false);
                                                if (out1V)
                                                {
                                                    txtBxOut1.Invoke((Action)(() => txtBxOut1.Text = PlaceDecimal(ConvertHexToShort(val[2], false))));

                                                    progressBar1.Invoke((Action)(() =>
                                                    {
                                                        int v = Convert.ToInt32(ConvertHexToShort(val[2], false));
                                                        progressBar1.Value = v;
                                                    }
                                                   ));
                                                }

                                                double out2 = ConvertHexToShort(val[3], false);
                                                if (out2V)
                                                {
                                                    txtBxOut2.Invoke((Action)(() => txtBxOut2.Text = PlaceDecimal(ConvertHexToShort(val[3], false))));
                                                    //progressBar2.Invoke((Action)(() => progressBar2.Value = Convert.ToInt32(out2)));
                                                    progressBar2.Invoke((Action)(() =>
                                                    {
                                                        int v = Convert.ToInt32(ConvertHexToShort(val[3], false));
                                                        progressBar2.Value = v;

                                                    }));
                                                }
                                                //4703
                                                break;
                                            case 3:
                                                //4728                                                         

                                                //4729
                                                //472A 
                                                double ver = string.IsNullOrEmpty(val[2]) ? 0.0 : Convert.ToDouble(val[2]) / 100;
                                                lblSwVersion.Invoke((Action)(() => lblSwVersion.Text = String.Format("{0:0.00}", ver)));

                                                //472B
                                                //472C
                                                //472D
                                                //472E
                                                //472F

                                                break;
                                            case 5:
                                                //4723

                                                //4724
                                                int val2 = Convert.ToInt32(ConvertHexToShort(val[1], true));
                                                if (unitVal)
                                                {
                                                    CmbBxUnitType.Invoke((Action)(() => CmbBxUnitType.SelectedIndex = val2));
                                                    switch (val2)
                                                    {
                                                        case -1:
                                                            lblUnit1.Invoke((Action)(() => lblUnit1.Text = ""));
                                                            break;
                                                        case 0:
                                                            lblUnit1.Invoke((Action)(() => lblUnit1.Text = "F"));
                                                            break;
                                                        case 1:
                                                            lblUnit1.Invoke((Action)(() => lblUnit1.Text = "C"));
                                                            break;
                                                        case 2:
                                                            lblUnit1.Invoke((Action)(() => lblUnit1.Text = "EU"));
                                                            break;
                                                    }
                                                }
                                                break;
                                            case 9:
                                                //4720
                                                if (alM1Val)
                                                {
                                                    cmbBxAlarm1Mode.Invoke((Action)(() => cmbBxAlarm1Mode.SelectedIndex = Convert.ToInt32(ConvertHexToShort(val[0], true))));
                                                }

                                                //4721
                                                if (alM2Val)
                                                {
                                                    cmbBxAlarm2Mode.Invoke((Action)(() => cmbBxAlarm2Mode.SelectedIndex = Convert.ToInt32(ConvertHexToShort(val[1], true))));
                                                }
                                                //4722

                                                //4723
                                                int valInt = Convert.ToInt32(ConvertHexToShort(val[3], true));
                                                if (runVal)
                                                {
                                                    CmbBxRunHaltmode.Invoke((Action)(() => CmbBxRunHaltmode.SelectedIndex = (valInt == 4) ? 0 : valInt));
                                                }
                                                break;
                                            case 10:
                                                //4710
                                                if (offVal)
                                                {
                                                    txtBxIoffset.Invoke((Action)(() => txtBxIoffset.Text = Conversion1(val[0])));
                                                }

                                                //4711
                                                double coef = ConvertHexToShort(val[1], false) / 10;
                                                if (coefV)
                                                {
                                                    txtBxPCoefficient.Invoke((Action)(() =>
                                                        txtBxPCoefficient.Text = PlaceDecimal(ConvertHexToShort(val[1], false) / 10)));
                                                }

                                                //4712
                                                double hy1 = ConvertHexToShort(val[2], false);
                                                if (deadV)
                                                {
                                                    txtHysteresisDeadBand.Invoke((Action)(() =>
                                                        txtHysteresisDeadBand.Text = PlaceDecimal(ConvertHexToShort(val[2], false))));
                                                    txtBxdeadband.Invoke((Action)(() =>
                                                        txtBxdeadband.Text = PlaceDecimal(ConvertHexToShort(val[2], false))));
                                                }

                                                //4713
                                                double hy2 = ConvertHexToShort(val[3], false);
                                                if (hys1V)
                                                {
                                                    txtHysteresis1.Invoke((Action)(() =>
                                                        txtHysteresis1.Text = PlaceDecimal(ConvertHexToShort(val[3], false))));
                                                }

                                                //4714
                                                double hy3 = ConvertHexToShort(val[4], false);
                                                if (hys2V)
                                                {
                                                    txtHysteresis2.Invoke((Action)(() =>
                                                        txtHysteresis2.Text = PlaceDecimal(ConvertHexToShort(val[4], false))));
                                                }
                                                //4715
                                                //4716
                                                //4717
                                                break;
                                            case 11:
                                                //4718
                                                if (senseVal)
                                                {
                                                    CmbBxSensorType.Invoke((Action)(() => CmbBxSensorType.SelectedIndex = Convert.ToInt32(ConvertHexToShort(val[0], true))));
                                                }

                                                //4719
                                                if (ctrlVal)
                                                {
                                                    CmbBxCtrlAction.Invoke((Action)(() => CmbBxCtrlAction.SelectedIndex = Convert.ToInt32(ConvertHexToShort(val[1], true))));
                                                }
                                                break;
                                            case 12:
                                                //4708
                                                double a2U = ConvertHexToShort(val[0], false);
                                                if (alM2UVal)
                                                {
                                                    txtBxAlarm2Up.Invoke((Action)(() =>
                                                        txtBxAlarm2Up.Text = PlaceDecimal(ConvertHexToShort(val[0], false))));
                                                }

                                                //4709
                                                double a2D = ConvertHexToShort(val[1], false);
                                                if (alM2DVal)
                                                {
                                                    txtBxAlarm2Down.Invoke((Action)(() =>
                                                         txtBxAlarm2Down.Text = PlaceDecimal(ConvertHexToShort(val[1], false))));
                                                }
                                                //470A
                                                //470B

                                                //470C
                                                double pb = ConvertHexToShort(val[4], false);
                                                if (pdV)
                                                {
                                                    txtBxPD.Invoke((Action)(() =>
                                                         txtBxPD.Text = PlaceDecimal(pb)));
                                                }

                                                //470D
                                                double td = ConvertHexToShort(val[5], true);
                                                if (tdV)
                                                {
                                                    txtBxTd.Invoke((Action)(() =>
                                                         txtBxTd.Text = PlaceDecimal(td)));
                                                }

                                                //470E
                                                double ti = ConvertHexToShort(val[6], true);
                                                if (tiV)
                                                {
                                                    txtBxTi.Invoke((Action)(() =>
                                                         txtBxTi.Text = PlaceDecimal(ti)));
                                                }

                                                //470F
                                                break;
                                            case 13:
                                                //4701

                                                //4702
                                                if (out1V)
                                                {
                                                    txtBxOut1.Invoke((Action)(() => txtBxOut1.Text =
                                                        PlaceDecimal(ConvertHexToShort(val[1], false))));
                                                }

                                                //4703
                                                if (out2V)
                                                {
                                                    txtBxOut2.Invoke((Action)(() => txtBxOut2.Text =
                                                        PlaceDecimal(ConvertHexToShort(val[2], false))));
                                                }

                                                //4704
                                                if (highVal)
                                                {
                                                    txtBxHightemp.Invoke((Action)(() => txtBxHightemp.Text =
                                                        PlaceDecimal(ConvertHexToShort(val[3], false))));
                                                }

                                                //4705
                                                if (lowVal)
                                                {
                                                    txtBxLowtemp.Invoke((Action)(() => txtBxLowtemp.Text =
                                                        PlaceDecimal(ConvertHexToShort(val[4], false))));
                                                }

                                                //4706     
                                                if (alM1UVal)
                                                {
                                                    txtBxAlarm1Up.Invoke((Action)(() => txtBxAlarm1Up.Text =
                                                       PlaceDecimal(ConvertHexToShort(val[5], false))));
                                                }

                                                //4707
                                                if (alM1DVal)
                                                {
                                                    txtBxAlarm1Down.Invoke((Action)(() => txtBxAlarm1Down.Text =
                                                       PlaceDecimal(ConvertHexToShort(val[6], false))));
                                                }
                                                break;
                                            #endregion
                                        }
                                    }
                                }
                                _manualResetEvent.WaitOne(Timeout.Infinite);
                                Thread.Sleep(10);
                                if (i == 14)
                                {
                                    i = 0;
                                    fullyLoaded = false;
                                }
                                else
                                {
                                    i++;
                                }
                            }

                            if (loopbreak)
                            {
                                btnConnect1.Invoke((Action)(() => btnConnect1.BackColor = Color.LightGreen));

                                grpBxParameters.Invoke((Action)(() => grpBxParameters.Visible = false));

                                circularProgressBar1.Invoke((Action)(() =>
                                {
                                    circularProgressBar1.Visible = false;
                                    circularProgressBar1.Value = 0;
                                }));

                                try
                                {
                                    if (thread != null)
                                    {
                                        btnConnect1.Invoke((Action)(() => btnConnect1.Text = "Connect"));
                                        _manualResetEvent.Set();
                                        thread = null;
                                        thread.Abort();
                                        fullyLoaded = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show(ex.Message);
                                }
                            }
                        });
                        thread.IsBackground = true;

                        btnConnect1.Text = "Disconnect";
                        thread.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (btnConnect1.Text == "Disconnect")
            {
                btnConnect1.BackColor = Color.LightGreen;

                grpBxParameters.Visible = false;

                circularProgressBar1.Invoke((Action)(() =>
                {
                    circularProgressBar1.Visible = false;
                    circularProgressBar1.Value = 0;
                }));
                progressBar1.Invoke((Action)(() =>
                {
                    progressBar1.Value = 0;
                }));

                progressBar2.Invoke((Action)(() =>
                {
                    progressBar2.Value = 0;
                }));

                txtPV1.Invoke((Action)(() =>
                   {
                       txtPV1.Text = "0.0";
                   }));

                txtSV1.Invoke((Action)(() =>
               {
                   txtSV1.Text = "0.0";
               }));
                try
                {
                    if (thread != null)
                    {

                        btnConnect1.Text = "Connect";
                        _manualResetEvent.Set();

                        thread.Abort();
                        thread = null;
                        fullyLoaded = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    thread = null;
                }

            }
        }

        private int DecimalPlaces()
        {
            int index = 0;
            switch (CmbBxfraction.SelectedIndex)
            {
                case 0:
                case 1:
                    index = 10;
                    break;
                case 2:
                    index = 10;
                    break;
                case 3:
                    index = 100;
                    break;
            }
            return index;
        }

        private double ConvertHexToShort(string hexVal, bool type)
        {
            if (string.IsNullOrEmpty(hexVal))
            {
                return 0;
            }
            else
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

        private string ConvertShortToHex(string val, bool type)
        {
            if (string.IsNullOrEmpty(val))
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

        private string Conversion1(string val)
        {
            return GetDecimalPlaces((Convert.ToDouble(Convert.ToInt32(val, 16)) / 1).ToString());
        }

        private string SendFrameToDevice(string unitAddress, string func, string regAddr, string data)
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
                    int readTime = 150;
                    if (func == "06")
                    {
                        readTime = 400;
                    }
                    Thread.Sleep(100);
                    if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength, readTime))
                    {
                        byte[] RecieveData = null;

                        if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                        {
                            if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
                            {
                                RecieveData = modbusobj.AscFrame(unitAddress, func, regAddr, data);
                            }
                            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
                            {
                                RecieveData = modbusobj.RtuFrame(unitAddress, func, regAddr, data);
                            }

                            if (RecieveData != null && RecieveData.Length > 0)
                            {
                                if (func == "03" || func == "06")
                                {
                                    char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();

                                    string result = "";

                                    if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
                                    {
                                        result = string.Join("", recdata);
                                    }
                                    else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
                                    {
                                        result = modbusobj.DisplayFrame(RecieveData);
                                    }

                                    LogWriter.WriteToFile("GraphRecoder: 5)", result, "GraphRecoder_ErrorLog");

                                    byte[] sizeBytes = ExtractByteArray(RecieveData, 4, 7);

                                    //int size = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(sizeBytes), 16);

                                    //return (Convert.ToDouble(size) / 10);

                                    return System.Text.Encoding.UTF8.GetString(sizeBytes);
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                // data not received
                                LogWriter.WriteToFile("GraphRecoder: 4)", "Received Data Timeout!", "GraphRecoder_ErrorLog");
                                //txtBxRecievecmd.Text = "Received Data Timeout!";
                                return null;
                            }
                        }
                        else
                        {
                            LogWriter.WriteToFile("GraphRecoder: 3)", "Received Data Timeout!", "GraphRecoder_ErrorLog");
                            //txtBxRecievecmd.Text = "Received Data Timeout!";
                            return null;
                        }
                    }
                    else
                    {
                        LogWriter.WriteToFile("GraphRecoder: 2)", "Connection failed", "GraphRecoder_ErrorLog");
                        //txtBxRecievecmd.Text = "Connection failed";
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    LogWriter.WriteToFile("GraphRecoder: 1)", ex.Message, "GraphRecoder_ErrorLog");
                    //lblMessage.Text = "3" + ex.Message;
                    return null;
                }
            }
            else
            {
                // settings are empty
                return null;
            }
        }

        private static byte[] ExtractByteArray(byte[] RecieveData, int size, int startPostition)
        {
            byte[] sizeBytes = new byte[size];

            Array.Copy(RecieveData, startPostition, sizeBytes, 0, size);
            return sizeBytes;
        }

        private void frmOnlineMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void txtBxSetvalue_Enter(object sender, EventArgs e)
        {
            setVal = false;
            //EnterText();
        }

        private void txtBxSetvalue_Leave(object sender, EventArgs e)
        {
            //EnterText();
            SetSVText();
            setVal = true;
        }

        private void SetSVText()
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxSetvalue.Text) ? "0" : txtBxSetvalue.Text;

                //int x = Convert.ToInt32(Convert.ToDouble(SetDecimalPlaces(val)));
                //txtBxSetvalue.Text = ConvertHexToShort(LeaveText("4701", x.ToString("X2").PadLeft(4, '0')),false).ToString();

                string x = ConvertShortToHex(val, false);
                txtBxSetvalue.Text = ConvertHexToShort(LeaveText("4701", x), false).ToString();

                this.ActiveControl = null;
                Resume();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxSetvalue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //SetSVText();
                this.ActiveControl = null;
            }
        }

        private void Resume()
        {
            //button2.Text = "Pause";

            try
            {
                _manualResetEvent.Set();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Pause()
        {
            //button2.Text = "Resume";

            try
            {
                _manualResetEvent.Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EnterText()
        {
            if (thread != null)
            {
                //if (button2.Text == "Pause")
                //{
                Pause();
                //}
                //else if (button2.Text == "Resume")
                //{
                //    Resume();
                //}
            }
        }

        private string LeaveText(string regAddr, string data)
        {
            if (thread != null)
            {
                string val = SendFrameToDevice(nodeAddress1.Value.ToString(), "06", regAddr, data);
                Resume();

                return string.IsNullOrEmpty(val) ? null : val;
            }
            else
                return null;
        }

        private List<string> CreateFrames(string nodeAddress, string functionCode, string regAddress, string wordCount, bool read)
        {
            byte[] RecieveData = null;

            try
            {
                string portName = SetValues.Set_PortName;
                string baudRate = SetValues.Set_Baudrate;
                string parity = SetValues.Set_parity;
                int bitsLength = SetValues.Set_BitsLength;
                string stopBits = SetValues.Set_StopBits;

                if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength))
                {
                    if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                    {
                        if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
                        {
                            //RecieveData = modbusobj.AscFrame(nodeAddress.PadLeft(2, '0'), functionCode, regAddress, wordCount);
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
                            else
                            {
                                return null;
                            }
                        }
                        else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
                        {
                            RecieveData = modbusobj.RtuFrame(nodeAddress, functionCode, regAddress, wordCount);

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
                                return null;
                            }

                        }


                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("CreateFrames: " + ex.Message);
                return null;
            }
            return null;
        }

        private void frmOnlineMonitor_Load(object sender, EventArgs e)
        {
            grpBxParameters.Visible = false;
            circularProgressBar1.Visible = false;

            CmbBxfraction.SelectedIndex = 1;
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;

            fillChart();

            vallCount = 30;
            //timer1.Start();
            #region Port Settings
            string portName = SetValues.Set_PortName;
            int baudRate = Convert.ToInt32(SetValues.Set_Baudrate);
            string parity = SetValues.Set_parity;
            int bitsLength = SetValues.Set_BitsLength;
            int stopBits = Convert.ToInt32(SetValues.Set_StopBits);

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
            }

            if (modbusobj.OpenSerialPort(portName, baudRate, parity, stopBits, bitsLength))
            {
                LogWriter.WriteToFile("Load() =>", "Port Opened"
                        , "OnlineMonitor");
            }
            #endregion
        }

        private void fillChart()
        {
            chart1.ChartAreas[0].BackColor = Color.Black;

            chart1.Series.Clear();
            chart1.Series.Add("Device1");
            chart1.Series["Device1"].ChartType = SeriesChartType.StepLine;
            chart1.Series["Device1"].BorderWidth = 1;
            chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
            chart1.ChartAreas[0].AxisY.IntervalType = DateTimeIntervalType.Number;

            chart1.ChartAreas[0].AxisX.ScaleView.Size = 300;
            chart1.ChartAreas[0].AxisY.ScaleView.Size = 35;

            ////chart title  
            //chart1.Titles.Add("Chart Name");

            ////X-axis-------------------------------------------------
            // X-axis line color
            this.chart1.ChartAreas[0].AxisX.LineColor = Color.White;
            // | vertical lines color
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.White;
            // X-axis label color
            this.chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
            // disable X-axis vertical lines
            chart1.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;

            ////Y-axis-------------------------------------------------
            // Y-axis line color
            this.chart1.ChartAreas[0].AxisY.LineColor = Color.White;
            // -- horizontal lines color
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.WhiteSmoke;
            // Y-axis label color
            this.chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            // disable Y-axis vertical lines
            //chart1.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;

            chart1.ChartAreas[0].AxisX.Interval = 1.0;// 0.001;
            chart1.ChartAreas[0].AxisY.Interval = 15;
        }

        //private void CmbBxCtrlAction_Leave(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        int x = Convert.ToInt32(CmbBxCtrlAction.SelectedIndex);
        //        LeaveText("4719", x.ToString("X4").PadLeft(4, '0'));
        //        //CmbBxCtrlAction.SelectedIndex = Convert.ToInt32(LeaveText("4719", x.ToString("X4").PadLeft(4, '0')), 16);
        //        this.ActiveControl = null;
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        private void CmbBx1stout_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
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

        private void CmbBxCtrlAction_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                int x = Convert.ToInt32(CmbBxCtrlAction.SelectedIndex);
                LeaveText("4719", x.ToString("X4").PadLeft(4, '0'));
                this.ActiveControl = null;
                Resume();
                ctrlVal = true;
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

        private void CmbBxSensorType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                int x = Convert.ToInt32(CmbBxSensorType.SelectedIndex);
                if (x > 0)
                {
                    LeaveText("4718", x.ToString("X4").PadLeft(4, '0'));
                }
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

        private void CmbBxCtrlAction_Enter(object sender, EventArgs e)
        {
            ctrlVal = false;
            //Pause();
        }

        private void CmbBxRunHaltmode_Enter(object sender, EventArgs e)
        {
            //Pause();
            runVal = false;
        }

        private void CmbBx1stout_Enter(object sender, EventArgs e)
        {
            //Pause();
            out1Val = false;
        }

        private void CmbBx2ndout_Enter(object sender, EventArgs e)
        {
            //Pause();
            out2Val = false;
        }

        private void CmbBxSensorType_Enter(object sender, EventArgs e)
        {
            //Pause();
            senseVal = false;
        }

        private void CmbBxUnitType_Enter(object sender, EventArgs e)
        {
            //Pause();
            unitVal = false;
        }

        private void cmbBxAlarm1Mode_Enter(object sender, EventArgs e)
        {
            //Pause();
            alM1Val = false;
        }

        private void cmbBxAlarm2Mode_Enter(object sender, EventArgs e)
        {
            //Pause();
            alM2Val = false;
        }

        private void rampSoakProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RampSoakForm rampSoakForm = new RampSoakForm(modbusobj);
            rampSoakForm.Owner = this;
            rampSoakForm.NodeAddress = Convert.ToString(nodeAddress1.Value); // "1";
            rampSoakForm.Show();
        }

        private void CmbBxCtrlAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            ContainerFilter();
        }

        private void ContainerFilter()
        {
            Point p = new Point(350, 14);
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

            switch (ctrlActionSelected)
            {
                case 1://ON/OFF
                    pgrpBxOnOff.Location = p;
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
                    #endregion
                    break;
                case 2://Manual Tuning
                    pgrpBxManual.Location = p;
                    #region 2

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
                    #region 3
                    if (CmbBx1stout1 == 0 && CmbBx2ndout2 == 0) //h h
                    {
                        txtBxPD.Visible = lblPD.Visible = true;
                        txtBxTi.Visible = lblTi.Visible = true;

                        txtBxTd.Visible = lblTd.Visible = true;
                        txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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
                    #region 3
                    if (CmbBx1stout1 == 0 && CmbBx2ndout2 == 0) //h h
                    {
                        txtBxPD.Visible = lblPD.Visible = true;
                        txtBxTi.Visible = lblTi.Visible = true;

                        txtBxTd.Visible = lblTd.Visible = true;
                        txtBxCtrlPer1.Visible = lblCtrlPer1.Visible = true;

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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

                        txtBxIoffset.Visible = lblIoffset.Visible = true;
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
                    cmbBxSetvalue.Visible = true;
                    txtBxSetvalue.Visible = false;
                    break;
            }
        }

        private void CmbBx2ndout_SelectedIndexChanged(object sender, EventArgs e)
        {
            ContainerFilter();

            int CmbBx2ndout2 = CmbBx2ndout.SelectedIndex;
            progressBar2.Visible = true;
            lblOut2.Visible = true;
            switch (CmbBx2ndout2)
            {
                case 0://h
                    //progressBar2
                    lblOut2.BackColor = Color.LightCoral;
                    pictureBox2.Visible = false;
                    break;
                case 1://c
                    //progressBar2
                    lblOut2.BackColor = Color.LightSteelBlue;
                    pictureBox2.Visible = false;
                    break;
                case 2://a
                    progressBar2.Visible = false;
                    lblOut2.Visible = false;
                    pictureBox2.Visible = true;
                    pictureBox2.BackColor = Color.LightGray;
                    break;
            }
        }

        private void CmbBx1stout_SelectedIndexChanged(object sender, EventArgs e)
        {
            ContainerFilter();
            int CmbBx1stout1 = CmbBx1stout.SelectedIndex;

            progressBar1.Visible = true;
            lblOut1.Visible = true;

            switch (CmbBx1stout1)
            {
                case 0://h
                    //progressBar1
                    lblOut1.BackColor = Color.LightCoral;
                    pictureBox1.Visible = false;

                    break;
                case 1://c
                    //progressBar1
                    lblOut1.BackColor = Color.LightSteelBlue;
                    pictureBox1.Visible = false;

                    break;
                case 2://a
                    progressBar1.Visible = false;
                    lblOut1.Visible = false;
                    pictureBox1.Visible = true;
                    pictureBox1.BackColor = Color.LightGray;
                    break;
            }
        }

        private void CmbBxfraction_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private string GetDecimalPlaces(string val)
        {
            string value = (string.IsNullOrEmpty(val) ? "0" : val);
            int index = 0;
            index = CmbBxfraction.SelectedIndex;
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
            return (string.IsNullOrEmpty(result) ? "0" : result);
        }

        private string SetDecimalPlaces(string val)
        {
            string value = (string.IsNullOrEmpty(val) ? "0" : val);
            int index = 0;
            index = CmbBxfraction.SelectedIndex;
            double res = 0;
            string result = "";
            switch (index)
            {
                case 0:
                    res = Convert.ToDouble(value) * 1;
                    result = String.Format("{0:0}", res);
                    break;
                case 1:
                    res = Convert.ToDouble(value) * 1;
                    result = String.Format("{0:0.0}", res);
                    break;
                case 2:
                    res = Convert.ToDouble(value) * 10;
                    result = String.Format("{0:0.00}", res);
                    break;
                case 3:
                    res = Convert.ToDouble(value) * 100;
                    result = String.Format("{0:0.000}", res);
                    break;
            }
            return (string.IsNullOrEmpty(result) ? "0" : result);
        }

        private string PlaceDecimal(double res)
        {
            int index = 0;
            index = CmbBxfraction.SelectedIndex;

            string result = "";

            switch (index)
            {
                case 0:
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

        private void CmbBxSensorType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CmbBxRunHaltmode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CmbBxUnitType_MouseLeave(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
            unitVal = true;
        }

        private void CmbBxSensorType_Leave(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
            senseVal = true;
        }

        private void txtBxHightemp_Enter(object sender, EventArgs e)
        {
            //EnterText();
            highVal = false;
        }

        private void txtBxHightemp_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxHightemp.Text) ? "0" : txtBxHightemp.Text;

                //int x = Convert.ToInt32(Convert.ToDouble(SetDecimalPlaces(val)));
                //txtBxHightemp.Text = Conversion1(LeaveText("4704", x.ToString("X2").PadLeft(4, '0')));

                string x = ConvertShortToHex(val, false);
                txtBxHightemp.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4704", x), false));

                this.ActiveControl = null;
                Resume();
                highVal = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxHightemp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxLowtemp_Enter(object sender, EventArgs e)
        {
            //EnterText();
            lowVal = false;
        }

        private void txtBxLowtemp_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxLowtemp.Text) ? "0" : txtBxLowtemp.Text;

                //int x = Convert.ToInt32(Convert.ToDouble(SetDecimalPlaces(val)));
                //txtBxLowtemp.Text = Conversion1(LeaveText("4705", x.ToString("X2").PadLeft(4, '0')));

                string x = ConvertShortToHex(val, false);
                txtBxLowtemp.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4705", x), false));

                this.ActiveControl = null;
                Resume();
                lowVal = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxLowtemp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxAlarm1Up_Enter(object sender, EventArgs e)
        {
            //EnterText();
            alM1UVal = false;
        }

        private void txtBxAlarm1Up_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxAlarm1Up_Leave(object sender, EventArgs e)
        {
            try
            {
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
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxAlarm1Down_Enter(object sender, EventArgs e)
        {
            //EnterText();
            alM1DVal = false;
        }

        private void txtBxAlarm1Down_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
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
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxAlarm2Up_Enter(object sender, EventArgs e)
        {
            //EnterText();
            alM2UVal = false;
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
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxAlarm2Up_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxAlarm2Down_Enter(object sender, EventArgs e)
        {
            //EnterText();
            alM2DVal = false;
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
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxAlarm2Down_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void cmbBxAlarm1Mode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                int x = Convert.ToInt32(cmbBxAlarm1Mode.SelectedIndex);
                if (x > 0)
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
                if (x > 0)
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

        private void CmbBxUnitType_Leave(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
            unitVal = true;
        }

        private void txtHysteresis1_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtHysteresis1.Text) ? "0" : txtHysteresis1.Text;

                string x = ConvertShortToHex(val, false);
                txtHysteresis1.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4713", x), false));

                this.ActiveControl = null;
                Resume();
                hys1V = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtHysteresis2_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtHysteresis2.Text) ? "0" : txtHysteresis2.Text;

                string x = ConvertShortToHex(val, false);
                txtHysteresis2.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4714", x), false));

                this.ActiveControl = null;
                Resume();
                hys2V = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtHysteresisDeadBand_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtHysteresisDeadBand.Text) ? "0" : txtHysteresisDeadBand.Text;

                string x = ConvertShortToHex(val, false);
                txtHysteresisDeadBand.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4712", x), false));

                this.ActiveControl = null;
                Resume();
                deadV = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxOut1_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxOut1.Text) ? "0" : txtBxOut1.Text;

                string x = ConvertShortToHex(val, false);
                txtBxOut1.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4702", x), false));

                this.ActiveControl = null;
                Resume();
                out1V = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxCtrlPeriod1_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxCtrlPeriod1.Text) ? "0" : txtBxCtrlPeriod1.Text;

                string x = ConvertShortToHex(val, true);
                txtBxCtrlPeriod1.Text = PlaceDecimal(ConvertHexToShort(LeaveText("471B", x), true));

                this.ActiveControl = null;
                Resume();
                ctrl1V = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxOut2_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxOut2.Text) ? "0" : txtBxOut2.Text;

                string x = ConvertShortToHex(val, false);
                txtBxOut2.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4703", x), false));

                this.ActiveControl = null;
                Resume();
                out2V = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxCtrlPeriod2_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxCtrlPeriod2.Text) ? "0" : txtBxCtrlPeriod2.Text;

                string x = ConvertShortToHex(val, true);
                txtBxCtrlPeriod2.Text = PlaceDecimal(ConvertHexToShort(LeaveText("471C", x), true));

                this.ActiveControl = null;
                Resume();
                ctrl2V = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxPD_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxPD.Text) ? "0" : txtBxPD.Text;

                string x = ConvertShortToHex(val, false);
                txtBxPD.Text = PlaceDecimal(ConvertHexToShort(LeaveText("470C", x), false));

                this.ActiveControl = null;
                Resume();
                pdV = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxTi_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxTi.Text) ? "0" : txtBxTi.Text;

                string x = ConvertShortToHex(val, true);
                txtBxTi.Text = PlaceDecimal(ConvertHexToShort(LeaveText("470E", x), true));

                this.ActiveControl = null;
                Resume();
                tiV = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxTd_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxTd.Text) ? "0" : txtBxTd.Text;

                string x = ConvertShortToHex(val, true);
                txtBxTd.Text = PlaceDecimal(ConvertHexToShort(LeaveText("470D", x), true));

                this.ActiveControl = null;
                Resume();
                tdV = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxCtrlPer1_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxCtrlPer1.Text) ? "0" : txtBxCtrlPer1.Text;

                string x = ConvertShortToHex(val, true);
                txtBxCtrlPer1.Text = PlaceDecimal(ConvertHexToShort(LeaveText("471B", x), true));

                this.ActiveControl = null;
                Resume();
                ctrl1V = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxctrlPer2_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxctrlPer2.Text) ? "0" : txtBxctrlPer2.Text;

                string x = ConvertShortToHex(val, true);
                txtBxctrlPer2.Text = PlaceDecimal(ConvertHexToShort(LeaveText("471C", x), true));

                this.ActiveControl = null;
                Resume();
                ctrl2V = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtHysteresis1_Enter(object sender, EventArgs e)
        {
            //EnterText();
            hys1V = false;
        }

        private void txtHysteresis1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtHysteresis2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtHysteresis2_Enter(object sender, EventArgs e)
        {
            //EnterText();
            hys2V = false;
        }

        private void txtHysteresisDeadBand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtHysteresisDeadBand_Enter(object sender, EventArgs e)
        {
            //EnterText();
            deadV = false;
        }

        private void txtBxOut1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxOut1_Enter(object sender, EventArgs e)
        {
            //EnterText();
            out1V = false;
        }

        private void txtBxCtrlPeriod1_Enter(object sender, EventArgs e)
        {
            //EnterText();
            ctrl1V = false;
        }

        private void txtBxCtrlPeriod1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxOut2_Enter(object sender, EventArgs e)
        {
            //EnterText();
            out2V = false;
        }

        private void txtBxOut2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxCtrlPeriod2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxCtrlPeriod2_Enter(object sender, EventArgs e)
        {
            //EnterText();
            ctrl2V = false;
        }

        private void txtBxTuneOHigh_Enter(object sender, EventArgs e)
        {
            //EnterText();
        }

        private void txtBxTuneOHigh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxPD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxPD_Enter(object sender, EventArgs e)
        {
            //EnterText();
            pdV = false;
        }

        private void txtBxTi_Enter(object sender, EventArgs e)
        {
            //EnterText();
            tiV = false;
        }

        private void txtBxTi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxTd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxTd_Enter(object sender, EventArgs e)
        {
            //EnterText();
            tdV = false;
        }

        private void txtBxCtrlPer1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxCtrlPer1_Enter(object sender, EventArgs e)
        {
            //EnterText();
            ctrl1V = false;
        }

        private void txtBxctrlPer2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxctrlPer2_Enter(object sender, EventArgs e)
        {
            //EnterText();
            ctrl2V = false;
        }

        private void txtBxPCoefficient_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxPCoefficient_Enter(object sender, EventArgs e)
        {
            //EnterText();
            coefV = false;
        }

        private void txtBxPCoefficient_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxPCoefficient.Text) ? "0" : (Convert.ToDouble(txtBxPCoefficient.Text) * 10).ToString();

                string x = ConvertShortToHex(val, false);
                txtBxPCoefficient.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4711", x), false) / 10);

                this.ActiveControl = null;
                Resume();
                coefV = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxdeadband_Leave(object sender, EventArgs e)
        {
            try
            {
                string val = string.IsNullOrEmpty(txtBxdeadband.Text) ? "0" : txtBxdeadband.Text;

                string x = ConvertShortToHex(val, false);
                txtBxdeadband.Text = PlaceDecimal(ConvertHexToShort(LeaveText("4712", x), false));

                this.ActiveControl = null;
                Resume();
                deadV = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxdeadband_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void txtBxdeadband_Enter(object sender, EventArgs e)
        {
            //EnterText();
            deadV = false;
        }

        private void CmbBxSensorType_DropDownClosed(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
        }

        private void CmbBxUnitType_DropDownClosed(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
        }

        private void CmbBxCtrlAction_DropDownClosed(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
        }

        private void CmbBxRunHaltmode_DropDownClosed(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
        }

        private void CmbBx1stout_DropDownClosed(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
        }

        private void CmbBx2ndout_DropDownClosed(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
        }

        private void CmbBxfraction_DropDownClosed(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            //Resume();
        }

        private void cmbBxAlarm1Mode_DropDownClosed(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
        }

        private void cmbBxAlarm2Mode_DropDownClosed(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
        }

        private void cmbBxAlarm1Mode_Leave(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
            alM1Val = true;
        }

        private void cmbBxAlarm2Mode_Leave(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            Resume();
            alM2Val = true;
        }

        private void frmOnlineMonitor_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (thread != null)
                {
                    if (thread.IsAlive)
                    {
                        thread.Abort();
                        thread = null;
                    }
                }

                if (modbusobj != null)
                {
                    if (modbusobj.IsSerialPortOpen())
                    {
                        modbusobj.CloseSerialPort();
                        modbusobj = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("frmOnlineMonitor: ", ex.StackTrace, "frmOnlineMonitor_ErrorLog");
                MessageBox.Show("3" + ex.Message);
            }
            finally
            {
                modbusobj = null;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (thread != null)
                {
                    if (thread.IsAlive)
                    {
                        thread.Abort();
                        thread = null;
                    }
                }

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
                LogWriter.WriteToFile("frmOnlineMonitor: ", ex.StackTrace, "frmOnlineMonitor_ErrorLog");
                MessageBox.Show("3" + ex.Message);
            }
            finally
            {
                modbusobj = null;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsToolStripMenuItem.Checked = true;
            pVRecordsToolStripMenuItem.Checked = false;

            settingsToolStripMenuItem.CheckState = CheckState.Checked;
            pVRecordsToolStripMenuItem.CheckState = CheckState.Unchecked;

            grpBxParameters.Visible = true;
            grpBx_PvRecords.Visible = false;
        }

        private void pVRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsToolStripMenuItem.Checked = false;
            pVRecordsToolStripMenuItem.Checked = true;

            settingsToolStripMenuItem.CheckState = CheckState.Unchecked;
            pVRecordsToolStripMenuItem.CheckState = CheckState.Checked;

            grpBxParameters.Visible = false;
            grpBx_PvRecords.Visible = true;

        }
        private double SendGraph(string unitAddress)
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
                    if (modbusobj.IsSerialPortOpen())
                    {
                        modbusobj.CloseSerialPort();
                    }

                    if (modbusobj.OpenSerialPort(portName, Convert.ToInt32(baudRate), parity, Convert.ToInt32(stopBits), bitsLength))
                    {
                        byte[] RecieveData = null;

                        if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                        {
                            if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
                            {
                                RecieveData = modbusobj.AscFrame(unitAddress.PadLeft(2, '0'), "03", "4700", "0002");
                            }
                            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
                            {
                                RecieveData = modbusobj.RtuFrame(unitAddress.PadLeft(2, '0'), "03", "4700", "0002");
                            }

                            if (RecieveData != null && RecieveData.Length > 0)
                            {
                                char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();

                                string result = "";

                                if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
                                {
                                    result = string.Join("", recdata);
                                }
                                else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
                                {
                                    result = modbusobj.DisplayFrame(RecieveData);
                                }

                                LogWriter.WriteToFile("GraphRecoder: 5)", result, "GraphRecoder_ErrorLog");

                                byte[] sizeBytes = ExtractByteArray(RecieveData, 4, 7);

                                int size = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(sizeBytes), 16);

                                return (Convert.ToDouble(size) / 10);

                                //returnVal.Text = int64Val.ToString();
                            }
                            else
                            {
                                // data not received
                                LogWriter.WriteToFile("GraphRecoder: 4)", "Received Data Timeout!", "GraphRecoder_ErrorLog");
                                //txtBxRecievecmd.Text = "Received Data Timeout!";
                                return -1;
                            }
                        }
                        else
                        {
                            LogWriter.WriteToFile("GraphRecoder: 3)", "Received Data Timeout!", "GraphRecoder_ErrorLog");
                            //txtBxRecievecmd.Text = "Received Data Timeout!";
                            return -1;
                        }
                    }
                    else
                    {
                        LogWriter.WriteToFile("GraphRecoder: 2)", "Connection failed", "GraphRecoder_ErrorLog");
                        //txtBxRecievecmd.Text = "Connection failed";
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    LogWriter.WriteToFile("GraphRecoder: 1)", ex.Message, "GraphRecoder_ErrorLog");
                    //lblMessage.Text = "3" + ex.Message;
                    return -1;
                }
            }
            else
            {
                // settings are empty
                return -1;
            }
        }
        private void CreateGraph(double a, double b)
        {
            chart1.Series["Device1"].Points.AddXY(a, b);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timerCount++;
            double a = 0.0;
            a = SendGraph("01");

            if (a != -1)
            {
                CreateGraph(timerCount, a);
            }
            if (Convert.ToInt32(vallCount) == timerCount)
            {
                #region VerticalAnno
                v1 = new VerticalLineAnnotation();
                v1.AxisX = chart1.ChartAreas[0].AxisX;
                v1.LineColor = Color.Red;
                v1.LineDashStyle = ChartDashStyle.Dash;
                v1.LineWidth = 1;
                v1.AllowMoving = false;
                v1.AllowSelecting = false;
                v1.AllowResizing = false;
                v1.X = vallCount;
                v1.IsInfinitive = true;

                r2 = new RectangleAnnotation();
                r2.AxisX = chart1.ChartAreas[0].AxisX;
                r2.IsSizeAlwaysRelative = false;
                r2.Width = 15;
                r2.Height = 2;

                r2.BackColor = Color.FromArgb(128, Color.Transparent);
                r2.LineColor = Color.Transparent;
                r2.AxisY = chart1.ChartAreas[0].AxisY;

                r2.Text = a.ToString();

                r2.ForeColor = Color.Black;
                r2.Font = new System.Drawing.Font("Arial", 8f);

                r2.Y = a;
                r2.X = v1.X;

                chart1.Invoke((Action)(() =>
                {
                    chart1.Annotations.Add(v1);
                    chart1.Annotations.Add(r2);
                }
                    ));
                //}
                #endregion

                vallCount += Convert.ToDouble(1 * 30);
            }
        }

        private void chart1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                chart1.Invoke((Action)(() =>
                {
                    if (v1 != null)
                    {
                        double xv = v1.X;   // get x-value of annotation
                        for (int i = 0; i < chart1.ChartAreas.Count; i++)
                        {
                            ChartArea ca = chart1.ChartAreas[i];
                            if (chart1.Series.Count > 0)
                            {
                                Series s = chart1.Series[i];
                                int px = (int)ca.AxisX.ValueToPixelPosition(xv);
                                var dp = s.Points.Where(x => x.XValue >= xv).FirstOrDefault();
                                if (dp != null)
                                {
                                    int py = (int)ca.AxisY.ValueToPixelPosition(s.Points[0].YValues[0]) - 20;
                                    e.Graphics.DrawString(dp.YValues[0].ToString("0.00"),
                                                          Font, Brushes.Red, px, py);
                                }
                            }
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBxAlarm2Down_TextChanged(object sender, EventArgs e)
        {

        }
    }

    //1 = normal (green); 2 = error (red); 3 = warning (yellow)
    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}
