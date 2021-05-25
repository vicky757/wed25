using ClassList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RTC_Communication_Utility
{
    public partial class GraphRecorder : Form
    {
        ChartArea chart;
        VerticalLineAnnotation v1;
        RectangleAnnotation r1, r2;
        Series S1;
        List<Dictionary<int, double>> listDictionary = new List<Dictionary<int, double>>();

        Dictionary<int, double> listDictionary1 = new Dictionary<int, double>();
        Dictionary<int, double> listDictionary2 = new Dictionary<int, double>();
        Dictionary<int, double> listDictionary3 = new Dictionary<int, double>();
        Dictionary<int, double> listDictionary4 = new Dictionary<int, double>();
        Dictionary<int, double> listDictionary5 = new Dictionary<int, double>();
        Dictionary<int, double> listDictionary6 = new Dictionary<int, double>();
        Dictionary<int, double> listDictionary7 = new Dictionary<int, double>();
        Dictionary<int, double> listDictionary8 = new Dictionary<int, double>();
        Dictionary<int, double> listDictionary9 = new Dictionary<int, double>();
        Dictionary<int, double> listDictionary10 = new Dictionary<int, double>();

        int counter = 1;
        double vall = 0;
        clsModbus modbusobj = null;

        public GraphRecorder()
        {
            InitializeComponent();
            chart = chart1.ChartAreas[0];

            modbusobj = new clsModbus();

            //modbusobj._GetLRCResultResult += new ModbusRTU.GetLRCResult(singlecmdtxt);
            modbusobj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
        }

        private void GraphRecorder_Load(object sender, EventArgs e)
        {
            toolStripBtnSave.Enabled = false;
            toolStripBtnRecorder.Enabled = false;
            toolStripBtnExit.Enabled = false;

            Type colorType = typeof(System.Drawing.Color);
            PropertyInfo[] propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo c in propInfoList)
            {
                if (c.Name.Equals("Black")) { }
                else
                {
                    this.cmbBackgroundColor.Items.Add(c.Name);
                }
            }

            cmbDisplayMode.SelectedIndex = 0;
            cmbBackgroundColor.SelectedIndex = 0;
            cmbDisplayMode_SelectedIndexChanged(null, null);
            txtMaxScale.Text = "0.0";
            txtMinScale.Text = "0.0";
            chart1.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
        }

        private void cmbDisplayMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbDisplayMode.SelectedIndex)
            {
                case 0:
                    txtMaxScale.Enabled = false;
                    txtMinScale.Enabled = false;

                    txtMaxScale.Text = double.IsNaN(chart.AxisY.Maximum) ? "0.0" : chart.AxisY.Maximum.ToString();
                    txtMinScale.Text = double.IsNaN(chart.AxisY.Minimum) ? "0.0" : chart.AxisY.Maximum.ToString();

                    //SetValues.Set_MaxGraphScale = txtMaxScale.Text;
                    //SetValues.Set_MinGraphScale = txtMinScale.Text;
                    break;
                case 1:
                    txtMaxScale.Enabled = true;
                    txtMinScale.Enabled = true;
                    txtMaxScale.Text = "0.0";
                    txtMinScale.Text = "0.0";
                    break;
            }
        }

        private void cmbBackgroundColor_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;
            if (e.Index >= 0)
            {
                string n = ((ComboBox)sender).Items[e.Index].ToString();
                Font f = new Font("Arial", 9, FontStyle.Regular);
                Color c = Color.FromName(n);
                Brush b = new SolidBrush(c);
                g.DrawString(n, f, Brushes.Black, rect.X, rect.Top);
                g.FillRectangle(b, rect.X + 110, rect.Y + 5, 20, 20);
            }
        }

        private void cmbBackgroundColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            string color = this.cmbBackgroundColor.SelectedItem.ToString();
            this.chart1.BackColor = Color.FromName(color);
            var chart = chart1.ChartAreas[0];
            chart.BackColor = Color.FromName(color);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (numericUpDownDiv1.Value > 0 || numericUpDownDiv3.Value > 0 || numericUpDownDiv5.Value > 0 || numericUpDownDiv7.Value > 0 ||
                numericUpDownDiv2.Value > 0 || numericUpDownDiv4.Value > 0 || numericUpDownDiv6.Value > 0 || numericUpDownDiv8.Value > 0)
            {
                if (btnStart.Text == "Start")
                {
                    if (modbusobj != null)
                    {
                        if (modbusobj.IsSerialPortOpen())
                        {
                            modbusobj.CloseSerialPort();
                        }

                        #region Port Settings
                        string portNameN = SetValues.Set_PortName;
                        int baudRateN = Convert.ToInt32(SetValues.Set_Baudrate);
                        string parityN = SetValues.Set_parity;
                        int bitsLengthN = SetValues.Set_BitsLength;
                        int stopBitsN = Convert.ToInt32(SetValues.Set_StopBits);

                        if (modbusobj.OpenSerialPort(portNameN, baudRateN, parityN, stopBitsN, bitsLengthN))
                        {
                            EnableDisableFields(false);

                            btnStart.Text = "Stop";

                            chart1.Series.Clear();
                            chart.AxisX.IntervalType = DateTimeIntervalType.Number;

                            chart.AxisX.LabelStyle.Format = "";
                            chart.AxisY.LabelStyle.Format = "";
                            chart.AxisY.LabelStyle.IsEndLabelVisible = true;

                            chart.AxisX.Minimum = 0;
                            chart.AxisX.Maximum = 40;

                            chart.AxisY.Minimum = 0;
                            chart.AxisY.Maximum = 35;

                            chart.AxisX.Interval = 5.0;
                            chart.AxisY.Interval = 5.0;

                            chart.AxisX.MajorGrid.Enabled = false;

                            chart.AxisX.ScaleView.Size = 300;//40;
                            chart.AxisY.ScaleView.Size = 35;

                            BindSeriesToChart();
                            S1 = chart1.Series[0];
                            txtMaxScale.Text = chart.AxisY.Maximum.ToString();
                            txtMinScale.Text = chart.AxisY.Minimum.ToString();
                            vall = Convert.ToDouble(numericUpDownTime.Value * 60);
                            timer1.Start();
                        }
                        #endregion
                    }


                }
                else if (btnStart.Text == "Stop")
                {
                    btnStart.Text = "Start";
                    //chart1.Series.Clear();

                    listDictionary.Add(listDictionary1);
                    listDictionary.Add(listDictionary2);
                    listDictionary.Add(listDictionary3);
                    listDictionary.Add(listDictionary4);
                    listDictionary.Add(listDictionary5);
                    listDictionary.Add(listDictionary6);
                    listDictionary.Add(listDictionary7);
                    listDictionary.Add(listDictionary8);


                    counter = 0;
                    timer1.Stop();
                    txtSamplingCounter.Text = counter.ToString().PadLeft(7, '0');

                    EnableDisableFields(true);
                    cmbDisplayMode_SelectedIndexChanged(null, null);

                    if (modbusobj != null)
                    {
                        if (modbusobj.IsSerialPortOpen())
                        {
                            modbusobj.CloseSerialPort();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Enter node address.");
            }
        }

        private void BindSeriesToChart()
        {
            if (numericUpDownDiv1.Value > 0)
            {
                chart1.Series.Add("Device1");
                chart1.Series["Device1"].ChartType = SeriesChartType.StepLine;
                chart1.Series["Device1"].BorderWidth = 2;
                chart1.Series["Device1"].Color = PnlDiv1.BackColor;
                //chart1.Series["Device1"].MarkerStyle = MarkerStyle.Circle;
            }

            if (numericUpDownDiv2.Value > 0)
            {
                chart1.Series.Add("Device2");
                chart1.Series["Device2"].ChartType = SeriesChartType.StepLine;
                chart1.Series["Device2"].BorderWidth = 2;
                chart1.Series["Device2"].Color = PnlDiv2.BackColor;
                //chart1.Series["Device2"].MarkerStyle = MarkerStyle.Circle;
            }

            if (numericUpDownDiv3.Value > 0)
            {
                chart1.Series.Add("Device3");
                chart1.Series["Device3"].ChartType = SeriesChartType.StepLine;
                chart1.Series["Device3"].BorderWidth = 2;
                chart1.Series["Device3"].Color = PnlDiv3.BackColor;
            }

            if (numericUpDownDiv4.Value > 0)
            {
                chart1.Series.Add("Device4");
                chart1.Series["Device4"].ChartType = SeriesChartType.StepLine;
                chart1.Series["Device4"].BorderWidth = 2;
                chart1.Series["Device4"].Color = PnlDiv4.BackColor;
            }

            if (numericUpDownDiv5.Value > 0)
            {
                chart1.Series.Add("Device5");
                chart1.Series["Device5"].ChartType = SeriesChartType.StepLine;
                chart1.Series["Device5"].BorderWidth = 2;
                chart1.Series["Device5"].Color = PnlDiv5.BackColor;
            }

            if (numericUpDownDiv6.Value > 0)
            {
                chart1.Series.Add("Device6");
                chart1.Series["Device6"].ChartType = SeriesChartType.StepLine;
                chart1.Series["Device6"].BorderWidth = 2;
                chart1.Series["Device6"].Color = PnlDiv6.BackColor;
            }

            if (numericUpDownDiv7.Value > 0)
            {
                chart1.Series.Add("Device7");
                chart1.Series["Device7"].ChartType = SeriesChartType.StepLine;
                chart1.Series["Device7"].BorderWidth = 2;
                chart1.Series["Device7"].Color = PnlDiv7.BackColor;
            }

            if (numericUpDownDiv8.Value > 0)
            {
                chart1.Series.Add("Device8");
                chart1.Series["Device8"].ChartType = SeriesChartType.StepLine;
                chart1.Series["Device8"].BorderWidth = 2;
                chart1.Series["Device8"].Color = PnlDiv8.BackColor;
            }
            chart1.ChartAreas[0].AxisX.Interval = 0.001;// Convert.ToDouble(DateTimeIntervalType.Milliseconds);
        }

        private void EnableDisableFields(bool flag)
        {
            numericUpDownTime.Enabled = flag;

            numericUpDownDiv1.Enabled = flag;
            numericUpDownDiv2.Enabled = flag;
            numericUpDownDiv3.Enabled = flag;
            numericUpDownDiv4.Enabled = flag;
            numericUpDownDiv5.Enabled = flag;
            numericUpDownDiv6.Enabled = flag;
            numericUpDownDiv7.Enabled = flag;
            numericUpDownDiv8.Enabled = flag;

            cmbDisplayMode.Enabled = flag;
            txtMaxScale.Enabled = flag;
            txtMinScale.Enabled = flag;
        }

        private void CreateGraph(string divName, double count, double x, double y)
        {
            #region increase Y size
            if (y > chart.AxisY.Maximum)
            {
                chart.AxisY.Maximum += 35;
                txtMaxScale.Text = chart.AxisY.Maximum.ToString();
            }
            #endregion

            switch (divName)
            {
                case "numericUpDownDiv1":
                    chart1.Series["Device1"].Points.AddXY(count, x);
                    break;
                case "numericUpDownDiv2":
                    chart1.Series["Device2"].Points.AddXY(count, x);
                    break;
                case "numericUpDownDiv3":
                    chart1.Series["Device3"].Points.AddXY(count, x);
                    break;
                case "numericUpDownDiv4":
                    chart1.Series["Device4"].Points.AddXY(count, x);
                    break;
                case "numericUpDownDiv5":
                    chart1.Series["Device5"].Points.AddXY(count, x);
                    break;
                case "numericUpDownDiv6":
                    chart1.Series["Device6"].Points.AddXY(count, x);
                    break;
                case "numericUpDownDiv7":
                    chart1.Series["Device7"].Points.AddXY(count, x);
                    break;
                case "numericUpDownDiv8":
                    chart1.Series["Device8"].Points.AddXY(count, x);
                    break;
            }
        }

        private void txtMaxScale_TextChanged(object sender, EventArgs e)
        {
            double value = string.IsNullOrEmpty(txtMaxScale.Text) ?
                0 : Math.Round(Convert.ToDouble(txtMaxScale.Text) / 10, 2);

            if (value > chart.AxisY.Maximum)
            {
                SetValues.Set_MaxGraphScale = value.ToString();
                chart.AxisY.Maximum = value;
            }
        }

        private void txtMinScale_TextChanged(object sender, EventArgs e)
        {
            double value = string.IsNullOrEmpty(txtMinScale.Text) ?
                0 : Math.Round(Convert.ToDouble(txtMinScale.Text) / 10, 2);

            if (value < chart.AxisY.Minimum)
            {
                SetValues.Set_MinGraphScale = value.ToString();
                chart.AxisY.Minimum = value;
            }
        }

        private void GraphRecorder_FormClosing(object sender, FormClosingEventArgs e)
        {


        }

        private void timer1_Tick(object sender, EventArgs e)
        {


            //Random random = new Random();

            //float x = random.Next(20, 30);
            //float y = random.Next(25, 49);

            double a = 0;

            if (numericUpDownDiv1.Value > 0)
            {
                a = SendFrameToDevice1(numericUpDownDiv1.Value.ToString());
                if (a != -1)
                {
                    CreateGraph("numericUpDownDiv1", counter, a, a);
                    txtDevice1.Text = Convert.ToString(a);
                    int c = listDictionary1.Count;
                    listDictionary1.Add(++c, a);
                }
            }

            if (numericUpDownDiv2.Value > 0)
            {
                if (a != -1)
                {
                    a = SendFrameToDevice1(numericUpDownDiv2.Value.ToString());

                    CreateGraph("numericUpDownDiv2", counter, a, a);

                    txtDevice2.Text = Convert.ToString(a);

                    int c = listDictionary2.Count;
                    listDictionary2.Add(++c, a);
                }
            }

            if (numericUpDownDiv3.Value > 0)
            {
                if (a != -1)
                {
                    a = SendFrameToDevice1(numericUpDownDiv3.Value.ToString());

                    CreateGraph("numericUpDownDiv3", counter, a, a);

                    txtDevice3.Text = Convert.ToString(a);

                    int c = listDictionary3.Count;
                    listDictionary3.Add(++c, a);
                }
            }

            if (numericUpDownDiv4.Value > 0)
            {
                if (a != -1)
                {
                    a = SendFrameToDevice1(numericUpDownDiv4.Value.ToString());

                    CreateGraph("numericUpDownDiv4", counter, a, a);

                    txtDevice4.Text = Convert.ToString(a);

                    int c = listDictionary4.Count;
                    listDictionary4.Add(++c, a);
                }
            }

            if (numericUpDownDiv5.Value > 0)
            {
                if (a != -1)
                {
                    a = SendFrameToDevice1(numericUpDownDiv5.Value.ToString());

                    CreateGraph("numericUpDownDiv5", counter, a, a);

                    txtDevice5.Text = Convert.ToString(a);

                    int c = listDictionary5.Count;
                    listDictionary5.Add(++c, a);
                }
            }

            if (numericUpDownDiv6.Value > 0)
            {
                if (a != -1)
                {
                    a = SendFrameToDevice1(numericUpDownDiv6.Value.ToString());

                    CreateGraph("numericUpDownDiv6", counter, a, a);

                    txtDevice6.Text = Convert.ToString(a);

                    int c = listDictionary6.Count;
                    listDictionary6.Add(++c, a);
                }
            }

            if (numericUpDownDiv7.Value > 0)
            {
                if (a != -1)
                {
                    a = SendFrameToDevice1(numericUpDownDiv7.Value.ToString());

                    CreateGraph("numericUpDownDiv7", counter, a, a);

                    txtDevice7.Text = Convert.ToString(a);

                    int c = listDictionary7.Count;
                    listDictionary7.Add(++c, a);
                }
            }

            if (numericUpDownDiv8.Value > 0)
            {
                if (a != -1)
                {
                    a = SendFrameToDevice1(numericUpDownDiv8.Value.ToString());

                    CreateGraph("numericUpDownDiv8", counter, a, a);

                    txtDevice8.Text = Convert.ToString(a);

                    int c = listDictionary8.Count;
                    listDictionary8.Add(++c, a);
                }
            }

            if (a != -1)// temp; need to think abt it
            {
                ++counter;

                this.txtSamplingCounter.Text = (counter).ToString().PadLeft(7, '0');


                if (Convert.ToInt32(vall) == counter)
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
                    v1.X = vall;
                    v1.IsInfinitive = true;

                    r1 = new RectangleAnnotation();
                    r1.AxisX = chart1.ChartAreas[0].AxisX;
                    r1.IsSizeAlwaysRelative = false;
                    r1.Width = 25;
                    r1.Height = 2;

                    r1.BackColor = Color.Red;
                    r1.LineColor = Color.Red;
                    r1.AxisY = chart1.ChartAreas[0].AxisY;

                    string time = System.DateTime.Now.ToLongTimeString();
                    r1.Text = time;

                    r1.ForeColor = Color.White;
                    r1.Font = new System.Drawing.Font("Arial", 7f);
                    r1.Y = -r1.Height;
                    r1.X = v1.X - r1.Width / 2;

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
                        chart1.Annotations.Add(r1);
                        chart1.Annotations.Add(r2);
                    }));
                    //chart1.Invoke((Action)(() =>
                    //{
                    //    chart1.Annotations.Add(v1);
                    //    chart1.Annotations.Add(r1);
                    //    chart1.Annotations.Add(r2);
                    //}
                    //));

                    #endregion

                    vall += Convert.ToDouble(numericUpDownTime.Value * 60);
                }
                ///increase X length
                if (counter > chart.AxisX.Maximum)
                {
                    chart.AxisX.Maximum += 40;
                }

                ///show X grid line on interval
                if (counter > chart.AxisX.Interval)
                {
                    chart.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                    chart.AxisX.MajorGrid.Enabled = true;
                }

                #region scroll bar X
                if (chart.AxisX.Maximum - chart.AxisX.Minimum > chart.AxisX.ScaleView.Size)
                {
                    chart.AxisX.ScrollBar.Enabled = true;
                    chart.AxisX.ScrollBar.ButtonColor = Color.Black;
                }
                else
                {
                    //chart.AxisX.ScrollBar.Enabled = false;
                }
                #endregion

                #region scroll bar Y
                if (chart.AxisY.Maximum - chart.AxisY.Minimum > chart.AxisY.ScaleView.Size)
                {
                    chart.AxisY.ScrollBar.Enabled = true;
                    chart.AxisY.ScrollBar.ButtonColor = Color.Black;
                    //chart.AxisY.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
                }
                else
                {
                    //chart.AxisY.ScrollBar.Enabled = false;
                }
                #endregion
            }
        }

        private void numericUpDownTime_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(this.numericUpDownTime.Value * 1000);
        }

        private double SendFrameToDevice1(string unitAddress)
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
                                RecieveData = modbusobj.RtuFrame(unitAddress.PadLeft(2, '0'), "03", "4700", "0001");
                            }

                            if (RecieveData != null && RecieveData.Length > 0)
                            {
                                char[] recdata = System.Text.Encoding.UTF8.GetString(RecieveData).ToCharArray();
                                byte[] sizeBytes = null;
                                string result = "";
                                int size = 0;
                                int value = 0;
                                if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii")
                                {
                                    result = string.Join("", recdata);
                                    sizeBytes = ExtractByteArray(RecieveData, 4, 7);
                                    value = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(sizeBytes), 16);
                                }
                                else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu")
                                {
                                    result = modbusobj.DisplayFrame(RecieveData);
                                    sizeBytes = ExtractByteArray(RecieveData, 2, 3);
                                    short val1 = Convert.ToInt16(string.Join("", sizeBytes), 16);
                                    value = val1;
                                }

                                LogWriter.WriteToFile("GraphRecoder: 5)", result, "GraphRecoder_ErrorLog");

                                //sizeBytes = ExtractByteArray(RecieveData, 4, 7);

                                //int value = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(sizeBytes), 16);

                                return (Convert.ToDouble(value) / 10);

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

        public void singlecmdtxt()
        {
            //txtBxLRC.Text = SetValues.Set_LRCFrame;
            //txtBxSendCommnd.Text = SetValues.Set_ASKFrame;
        }

        private static byte[] ExtractByteArray(byte[] RecieveData, int size, int startPostition)
        {
            byte[] sizeBytes = new byte[size];
            if (RecieveData != null && RecieveData.Length > 0)
            {
                Array.Copy(RecieveData, startPostition, sizeBytes, 0, size);
            }
            return sizeBytes;
        }

        private void chart1_AnnotationPositionChanging(object sender, AnnotationPositionChangingEventArgs e)
        {
            // move the rectangle with the line
            if (sender == v1) r1.X = v1.X - r1.Width / 2;

            // display the current Y-value
            int pt1 = (int)e.NewLocationX;
            double step = (S1.Points[pt1 + 1].YValues[0] - S1.Points[pt1].YValues[0]);
            double deltaX = e.NewLocationX - S1.Points[pt1].XValue;
            double val = S1.Points[pt1].YValues[0] + step * deltaX;
            chart1.Titles[0].Text = String.Format(
                                    "X = {0:0.00}   Y = {1:0.00}", e.NewLocationX, val);
            r1.Text = String.Format("{0:0.00}", val);
            chart1.Update();
        }

        private void GraphRecorder_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (modbusobj != null)
            {
                if (modbusobj.IsSerialPortOpen())
                {
                    modbusobj.CloseSerialPort();
                }

                modbusobj = null;
            }
        }

        private void chart1_AnnotationPositionChanged(object sender, EventArgs e)
        {
            v1.X = (int)(v1.X + 0.5);
            r1.X = v1.X - r1.Width / 2;
        }

        private void toolStripBtnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML|*.xml";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                }
                catch (Exception ex)
                {
                    MessageBoxEx.Show("Something went wrong!! Try Again");
                }
            }
        }

        private void toolStripBtnRecorder_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Paint(object sender, PaintEventArgs e)
        {
            //chart1.Invoke((Action)(() =>
            //       {
            //           if (v1 != null)
            //           {
            //               double xv = v1.X;   // get x-value of annotation
            //               for (int i = 0; i < chart1.ChartAreas.Count; i++)
            //               {
            //                   ChartArea ca = chart1.ChartAreas[i];
            //                   if (chart1.Series.Count > 0)
            //                   {
            //                       Series s = chart1.Series[i];
            //                       int px = (int)ca.AxisX.ValueToPixelPosition(xv);
            //                       var dp = s.Points.Where(x => x.XValue >= xv).FirstOrDefault();
            //                       if (dp != null)
            //                       {
            //                           int py = (int)ca.AxisY.ValueToPixelPosition(s.Points[0].YValues[0]) - 20;
            //                           e.Graphics.DrawString(dp.YValues[0].ToString("0.00"),
            //                                                 Font, Brushes.Red, px, py);

            //                       }
            //                   }
            //               }
            //           }
            //       }));



        }
    }
}
