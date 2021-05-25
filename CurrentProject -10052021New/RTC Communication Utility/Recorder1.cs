using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RTC_Communication_Utility
{
    public partial class Recorder1 : Form
    {
        public Recorder1()
        {
            SetValues.Set_Form = "Recorder1";
            InitializeComponent();
            Chart = chart_Databx.ChartAreas[0];
        }
        int ii = 0;
        byte[] GetASKdata = new byte[19];
        ModbusRTU modbusobj = new ModbusRTU();
        ChartArea Chart;
        Dictionary<int, double> mydictionary1 = new Dictionary<int, double>();
        Dictionary<int, double> mydictionary2 = new Dictionary<int, double>();
        Dictionary<int, double> mydictionary3 = new Dictionary<int, double>();
        Dictionary<int, double> mydictionary4 = new Dictionary<int, double>();
        Dictionary<int, double> mydictionary5 = new Dictionary<int, double>();
        Dictionary<int, double> mydictionary6 = new Dictionary<int, double>();
        Dictionary<int, double> mydictionary7 = new Dictionary<int, double>();
        Dictionary<int, double> mydictionary8 = new Dictionary<int, double>();
        string filepath = "";
        VerticalLineAnnotation VA;
        int i = 0;

        bool deviceNo1 = false;
        bool deviceNo2 = false;
        bool deviceNo3 = false;
        bool deviceNo4 = false;
        bool deviceNo5 = false;
        bool deviceNo6 = false;
        bool deviceNo7 = false;
        bool deviceNo8 = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            numUpDnSamplingTm.AutoSize = true;
            numUpDnSamplingTm.Height = 25;
            numUpDnSamplingTm.Value = 1;
            cmbbx_Disply.SelectedIndex = 0;
            numericUpDownDiv1.Value = 1;
            SetValues.Set_UnitAddress = "0" + numericUpDownDiv1.Value.ToString();
            SetValues.Set_CommandType = "03";
            SetValues.Set_RegAddress = "4700";
            SetValues.Set_WordCount = "0002";

            char[] arr = modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType, SetValues.Set_RegAddress, SetValues.Set_WordCount);
        }

        public void func1(int num)
        {
            if (num.ToString().Length == 1)
                SetValues.Set_UnitAddress = "0" + Convert.ToString(num);
            else
                SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv1.Value);
            modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType, SetValues.Set_RegAddress, SetValues.Set_WordCount);
            GetASKdata = modbusobj.Read_AscDataRegister(1);
            //if (GetASKdata[0] != 0)
            //    goto Outer; ;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            deviceNo1 = false;
            deviceNo2 = false;
            deviceNo3 = false;
            deviceNo4 = false;
            deviceNo5 = false;
            deviceNo6 = false;
            deviceNo7 = false;
            deviceNo8 = false;

            i = 0;
            mydictionary1.Clear();
            mydictionary2.Clear();
            mydictionary3.Clear();
            mydictionary4.Clear();
            mydictionary5.Clear();
            mydictionary6.Clear();
            mydictionary7.Clear();
            mydictionary8.Clear();

            numUpDnSamplingTm.Enabled = false;
            cmbbx_Disply.Enabled = false;
            numericUpDownDiv1.Enabled = false;
            numericUpDownDiv2.Enabled = false;
            numericUpDownDiv3.Enabled = false;
            numericUpDownDiv4.Enabled = false;
            numericUpDownDiv5.Enabled = false;
            numericUpDownDiv6.Enabled = false;
            numericUpDownDiv7.Enabled = false;
            numericUpDownDiv8.Enabled = false;
            toolStripBtnSave.Enabled = false;
            toolStripBtnRecorder.Enabled = false;
            btnStart.Enabled = false;

            modbusobj.OpenCOMPort(SetValues.Set_PortName, Convert.ToInt32(SetValues.Set_Baudrate),
                SetValues.Set_parity, Convert.ToInt32(SetValues.Set_StopBits), SetValues.Set_BitsLength);

            byte[] GetASKdata = new byte[19];

            if (numericUpDownDiv1.Value != 0)
            {
                if (numericUpDownDiv1.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv1.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv1.Value);

                modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                GetASKdata = modbusobj.Read_AscDataRegister(1);
                if (GetASKdata != null)
                {
                    if (GetASKdata[0] != 0)
                        goto Outer; ;
                }
            }
            if (numericUpDownDiv2.Value != 0)
            {
                if (numericUpDownDiv2.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv2.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv2.Value);

                modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                GetASKdata = modbusobj.Read_AscDataRegister(1);
                if (GetASKdata != null)
                {
                    if (GetASKdata[0] != 0)
                        goto Outer; ;
                }
            }
            if (numericUpDownDiv3.Value != 0)
            {
                if (numericUpDownDiv3.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv3.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv3.Value);

                modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                GetASKdata = modbusobj.Read_AscDataRegister(1);
                if (GetASKdata != null)
                {
                    if (GetASKdata[0] != 0)
                        goto Outer; ;
                }
            }
            if (numericUpDownDiv4.Value != 0)
            {
                if (numericUpDownDiv4.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv4.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv4.Value);

                modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                GetASKdata = modbusobj.Read_AscDataRegister(1);
                if (GetASKdata != null)
                {
                    if (GetASKdata[0] != 0)
                        goto Outer; ;
                }
            }
            if (numericUpDownDiv5.Value != 0)
            {
                if (numericUpDownDiv5.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv5.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv5.Value);

                modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                GetASKdata = modbusobj.Read_AscDataRegister(1);
                if (GetASKdata != null)
                {
                    if (GetASKdata[0] != 0)
                        goto Outer; ;
                }
            }
            if (numericUpDownDiv6.Value != 0)
            {
                if (numericUpDownDiv6.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv6.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv6.Value);

                modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                GetASKdata = modbusobj.Read_AscDataRegister(1);
                if (GetASKdata != null)
                {
                    if (GetASKdata[0] != 0)
                        goto Outer; ;
                }
            }
            if (numericUpDownDiv7.Value != 0)
            {
                if (numericUpDownDiv7.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv7.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv7.Value);

                modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                GetASKdata = modbusobj.Read_AscDataRegister(1);
                if (GetASKdata != null)
                {
                    if (GetASKdata[0] != 0)
                        goto Outer; ;
                }
            }
            if (numericUpDownDiv8.Value != 0)
            {
                if (numericUpDownDiv8.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv8.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv8.Value);

                modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                GetASKdata = modbusobj.Read_AscDataRegister(1);
                if (GetASKdata != null)
                {
                    if (GetASKdata[0] != 0)
                        goto Outer; ;
                }
            }
            if (GetASKdata != null)
            {
                if (GetASKdata[0] == 0 || GetASKdata[1] == 0)
                {
                    modbusobj.Port_Close();

                    numUpDnSamplingTm.Enabled = true;
                    cmbbx_Disply.Enabled = true;
                    numericUpDownDiv1.Enabled = true;
                    numericUpDownDiv2.Enabled = true;
                    numericUpDownDiv3.Enabled = true;
                    numericUpDownDiv4.Enabled = true;
                    numericUpDownDiv5.Enabled = true;
                    numericUpDownDiv6.Enabled = true;
                    numericUpDownDiv7.Enabled = true;
                    numericUpDownDiv8.Enabled = true;
                    toolStripBtnSave.Enabled = false;
                    toolStripBtnRecorder.Enabled = false;
                    btnStart.Enabled = true;
                    return; ;
                }
            }

        Outer:
            if (GetASKdata != null)
            {
                char[] recdata = System.Text.Encoding.UTF8.GetString(GetASKdata).ToCharArray();

                string Temperture = "";
                for (int j = 0; j < 4; j++)
                {
                    Temperture += recdata[j + 7].ToString();
                }
                double int64Val = Convert.ToInt64(Temperture, 16);
                int64Val = int64Val * .1;

                if (cmbbx_Disply.SelectedIndex == 1)
                {
                    Chart.AxisY.Minimum = Convert.ToInt32(SetValues.Set_MinGraphScale);
                    Chart.AxisY.Maximum = Convert.ToInt32(SetValues.Set_MaxGraphScale);

                    double maxlimit = (Chart.AxisY.Maximum - Chart.AxisY.Minimum) / 5;
                    if (Chart.AxisY.Minimum > int64Val)
                    {
                        Chart.AxisY.Minimum = int64Val - 1;
                        Chart.AxisY.Maximum = int64Val + 20;
                        Chart.AxisY.Interval = 2;
                    }
                    else
                        Chart.AxisY.Interval = maxlimit;
                }
                else
                {
                    Chart.AxisY.Minimum = int64Val - 2;
                    Chart.AxisY.Maximum = int64Val + 2;
                    Chart.AxisY.Interval = 0.5;
                }
                chart_Databx.Series.Clear();
                Chart.AxisX.IntervalType = DateTimeIntervalType.Number;

                Chart.AxisX.LabelStyle.Format = "";
                Chart.AxisY.LabelStyle.Format = "";
                Chart.AxisY.LabelStyle.IsEndLabelVisible = true;

                Chart.AxisX.Minimum = 1;
                Chart.AxisX.Maximum = 1000;

                Chart.AxisX.Interval = 0.1;

                chart_Databx.Series.Add("Divice1");
                chart_Databx.Series["Divice1"].ChartType = SeriesChartType.Spline;
                chart_Databx.Series["Divice1"].BorderWidth = 2;
                chart_Databx.Series["Divice1"].Color = PnlDiv1.BackColor;

                chart_Databx.Series.Add("Divice2");
                chart_Databx.Series["Divice2"].ChartType = SeriesChartType.Spline;
                chart_Databx.Series["Divice2"].BorderWidth = 2;
                chart_Databx.Series["Divice2"].Color = PnlDiv2.BackColor;

                chart_Databx.Series.Add("Divice3");
                chart_Databx.Series["Divice3"].ChartType = SeriesChartType.Spline;
                chart_Databx.Series["Divice3"].BorderWidth = 2;
                chart_Databx.Series["Divice3"].Color = PnlDiv3.BackColor;

                chart_Databx.Series.Add("Divice4");
                chart_Databx.Series["Divice4"].ChartType = SeriesChartType.Spline;
                chart_Databx.Series["Divice4"].BorderWidth = 2;
                chart_Databx.Series["Divice4"].Color = PnlDiv4.BackColor;

                chart_Databx.Series.Add("Divice5");
                chart_Databx.Series["Divice5"].ChartType = SeriesChartType.Spline;
                chart_Databx.Series["Divice5"].BorderWidth = 2;
                chart_Databx.Series["Divice5"].Color = PnlDiv5.BackColor;

                chart_Databx.Series.Add("Divice6");
                chart_Databx.Series["Divice6"].ChartType = SeriesChartType.Spline;
                chart_Databx.Series["Divice6"].BorderWidth = 2;
                chart_Databx.Series["Divice6"].Color = PnlDiv6.BackColor;

                chart_Databx.Series.Add("Divice7");
                chart_Databx.Series["Divice7"].ChartType = SeriesChartType.Spline;
                chart_Databx.Series["Divice7"].BorderWidth = 2;
                chart_Databx.Series["Divice7"].Color = PnlDiv7.BackColor;

                chart_Databx.Series.Add("Divice8");
                chart_Databx.Series["Divice8"].ChartType = SeriesChartType.Spline;
                chart_Databx.Series["Divice8"].BorderWidth = 2;
                chart_Databx.Series["Divice8"].Color = PnlDiv7.BackColor;

                chart_Databx.Legends.Clear();
                chart_Databx.ChartAreas[0].AxisX.LabelStyle.Enabled = false;

                if (tmr == null)
                    this.tmr = new System.Windows.Forms.Timer(this.components);

                tmr.Interval = (10);
                tmr.Enabled = true;
                ii++;
                tmr.Tick += new EventHandler(timer1_Tick);
                tmr.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblforsamplecnt.Text = ii.ToString();
            tmr.Interval = (Convert.ToInt32(numUpDnSamplingTm.Value) * 1000);
            if (numericUpDownDiv1.Value != 0)
            {
                chart_Databx.Series["Divice1"].Color = PnlDiv1.BackColor; ;
                if (numericUpDownDiv1.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv1.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv1.Value);

                if (deviceNo1 == false)
                {
                    modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                    GetASKdata = modbusobj.Read_AscDataRegister(1);
                    if (GetASKdata != null)
                    {

                        if (GetASKdata[0] != 0)
                        {
                            DrawGraph("numericUpDownDiv1");
                        }
                        else
                        {
                            deviceNo1 = true;
                        }
                    }
                }
            }

            if (numericUpDownDiv2.Value != 0)
            {
                if (numericUpDownDiv2.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv2.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv2.Value);
                if (deviceNo2 == false)
                {
                    modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                    GetASKdata = modbusobj.Read_AscDataRegister(1);
                    if (GetASKdata != null)
                    {

                        if (GetASKdata[0] != 0)
                        {
                            DrawGraph("numericUpDownDiv2");
                        }
                        else
                        {

                            deviceNo2 = true;
                        }
                    }
                }

            }
            if (numericUpDownDiv3.Value != 0)
            {
                if (numericUpDownDiv3.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv3.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv3.Value);
                if (deviceNo3 == false)
                {
                    modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                    GetASKdata = modbusobj.Read_AscDataRegister(1);
                    if (GetASKdata != null)
                    {

                        if (GetASKdata[0] != 0)
                        {
                            DrawGraph("numericUpDownDiv3");
                        }
                        else
                        {
                            deviceNo3 = true;
                        }
                    }
                }
            }
            if (numericUpDownDiv4.Value != 0)
            {
                if (numericUpDownDiv4.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv4.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv4.Value);
                if (deviceNo4 == false)
                {
                    modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                    GetASKdata = modbusobj.Read_AscDataRegister(1);
                    if (GetASKdata != null)
                    {

                        if (GetASKdata[0] != 0)
                        {
                            DrawGraph("numericUpDownDiv4");
                        }
                        else
                        {
                            deviceNo4 = true;
                        }
                    }
                }

            }
            if (numericUpDownDiv5.Value != 0)
            {
                if (numericUpDownDiv5.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv5.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv5.Value);
                if (deviceNo5 == false)
                {
                    modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                    GetASKdata = modbusobj.Read_AscDataRegister(1);
                    if (GetASKdata != null)
                    {

                        if (GetASKdata[0] != 0)
                        {
                            DrawGraph("numericUpDownDiv5");
                        }
                        else
                        {
                            deviceNo5 = true;
                        }
                    }
                }
            }
            if (numericUpDownDiv6.Value != 0)
            {
                if (numericUpDownDiv6.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv6.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv6.Value);
                if (deviceNo6 == false)
                {
                    modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                    GetASKdata = modbusobj.Read_AscDataRegister(1);
                    if (GetASKdata != null)
                    {

                        if (GetASKdata[0] != 0)
                        {
                            DrawGraph("numericUpDownDiv6");
                        }
                        else
                        {
                            deviceNo6 = true;
                        }
                    }
                }
            }
            if (numericUpDownDiv7.Value != 0)
            {
                if (numericUpDownDiv7.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv7.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv7.Value);
                if (deviceNo7 == false)
                {
                    modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                    GetASKdata = modbusobj.Read_AscDataRegister(1);
                    if (GetASKdata != null)
                    {

                        if (GetASKdata[0] != 0)
                        {
                            DrawGraph("numericUpDownDiv7");
                        }
                        else
                        {
                            deviceNo7 = true;
                        }
                    }
                }
            }
            if (numericUpDownDiv8.Value != 0)
            {
                if (numericUpDownDiv8.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv8.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv8.Value);
                if (deviceNo8 == false)
                {
                    modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                    GetASKdata = modbusobj.Read_AscDataRegister(1);
                    if (GetASKdata != null)
                    {
                        if (GetASKdata[0] != 0)
                        {
                            DrawGraph("numericUpDownDiv8");
                        }
                        else
                        {
                            deviceNo8 = true;
                        }
                    }
                }
            }
            if (deviceNo1 || deviceNo2 || deviceNo3 || deviceNo4 ||
                deviceNo5 || deviceNo6 || deviceNo7 || deviceNo8)
            {
                tmr.Stop();
                modbusobj.Port_Close();
                return;
            }
            i++;
        }

        private void DrawGraph(string NumText)
        {
            if (GetASKdata != null)
            {

                char[] recdata = System.Text.Encoding.UTF8.GetString(GetASKdata).ToCharArray();

                string Temperture = "";
                for (int j = 0; j < 4; j++)
                {
                    Temperture += recdata[j + 7].ToString();
                }
                double int64Val = Convert.ToInt64(Temperture, 16);
                int64Val = int64Val * .1;

                switch (NumText)
                {
                    case "numericUpDownDiv1":
                        if (numericUpDownDiv1.Value.ToString().Length == 1)
                        {
                            if (SetValues.Set_UnitAddress == "0" + numericUpDownDiv1.Value.ToString())
                                mydictionary1.Add(i, int64Val);
                        }
                        else
                            mydictionary1.Add(i, int64Val);

                        chart_Databx.Series["Divice1"].Points.AddXY(i, float.Parse(int64Val.ToString()));
                        break;
                    case "numericUpDownDiv2":
                        if (numericUpDownDiv2.Value.ToString().Length == 1)
                        {
                            if (SetValues.Set_UnitAddress == "0" + numericUpDownDiv2.Value.ToString())
                                mydictionary2.Add(i, int64Val);
                        }
                        else
                            mydictionary2.Add(i, int64Val);

                        chart_Databx.Series["Divice2"].Points.AddXY(i, float.Parse(int64Val.ToString()));
                        break;
                    case "numericUpDownDiv3":
                        if (numericUpDownDiv3.Value.ToString().Length == 1)
                        {
                            if (SetValues.Set_UnitAddress == "0" + numericUpDownDiv3.Value.ToString())
                                mydictionary3.Add(i, int64Val);
                        }
                        else
                            mydictionary3.Add(i, int64Val);

                        chart_Databx.Series["Divice3"].Points.AddXY(i, float.Parse(int64Val.ToString()));
                        break;
                    case "numericUpDownDiv4":
                        if (numericUpDownDiv4.Value.ToString().Length == 1)
                        {
                            if (SetValues.Set_UnitAddress == "0" + numericUpDownDiv4.Value.ToString())
                                mydictionary4.Add(i, int64Val);
                        }
                        else
                            mydictionary4.Add(i, int64Val);

                        chart_Databx.Series["Divice4"].Points.AddXY(i, float.Parse(int64Val.ToString()));
                        break;
                    case "numericUpDownDiv5":
                        if (numericUpDownDiv5.Value.ToString().Length == 1)
                        {
                            if (SetValues.Set_UnitAddress == "0" + numericUpDownDiv5.Value.ToString())
                                mydictionary5.Add(i, int64Val);
                        }
                        else
                            mydictionary5.Add(i, int64Val);

                        chart_Databx.Series["Divice5"].Points.AddXY(i, float.Parse(int64Val.ToString()));
                        break;
                    case "numericUpDownDiv6":
                        if (numericUpDownDiv6.Value.ToString().Length == 1)
                        {
                            if (SetValues.Set_UnitAddress == "0" + numericUpDownDiv6.Value.ToString())
                                mydictionary6.Add(i, int64Val);
                        }
                        else
                            mydictionary6.Add(i, int64Val);
                        chart_Databx.Series["Divice6"].Points.AddXY(i, float.Parse(int64Val.ToString()));
                        break;
                    case "numericUpDownDiv7":

                        if (numericUpDownDiv7.Value.ToString().Length == 1)
                        {
                            if (SetValues.Set_UnitAddress == "0" + numericUpDownDiv7.Value.ToString())
                                mydictionary7.Add(i, int64Val);
                        }
                        else
                            mydictionary7.Add(i, int64Val);

                        chart_Databx.Series["Divice7"].Points.AddXY(i, float.Parse(int64Val.ToString()));
                        break;
                    case "numericUpDownDiv8":
                        if (numericUpDownDiv8.Value.ToString().Length == 1)
                        {
                            if (SetValues.Set_UnitAddress == "0" + numericUpDownDiv8.Value.ToString())
                                mydictionary8.Add(i, int64Val);
                        }
                        else
                            mydictionary8.Add(i, int64Val);

                        chart_Databx.Series["Divice8"].Points.AddXY(i, float.Parse(int64Val.ToString()));
                        break;
                }

                Chart.AxisX.Interval = 10;
                Chart.AxisX.Maximum = 1000;
            }
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            tmr.Stop();
            modbusobj.Port_Close();
            lblforsamplecnt.Text = "0";
            numUpDnSamplingTm.Enabled = true;
            cmbbx_Disply.Enabled = true;
            numericUpDownDiv1.Enabled = true;
            numericUpDownDiv2.Enabled = true;
            numericUpDownDiv3.Enabled = true;
            numericUpDownDiv4.Enabled = true;
            numericUpDownDiv5.Enabled = true;
            numericUpDownDiv6.Enabled = true;
            numericUpDownDiv7.Enabled = true;
            numericUpDownDiv8.Enabled = true;
            toolStripBtnSave.Enabled = false;
            toolStripBtnRecorder.Enabled = false;
            btnStart.Enabled = true;
            toolStripBtnSave.Enabled = true;
            toolStripBtnRecorder.Enabled = true;
        }

        private void btn_backColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();

            if (colorDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                chart_Databx.ChartAreas[0].BackColor = colorDlg.Color;
            }
        }

        private void toolStripBtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_DrawLineColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            if (colorDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    PnlDiv1.BackColor = colorDlg.Color;
                }
                catch (Exception err)
                {

                }
            }

        }

        private void cmbbx_Disply_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbbx_Disply.SelectedIndex == 0)
            {
                txtBx_Maxscale.Enabled = false;
                txtBx_Minscale.Enabled = false;
            }
            else if (cmbbx_Disply.SelectedIndex == 1)
            {
                txtBx_Maxscale.Enabled = true;
                txtBx_Minscale.Enabled = true;
                txtBx_Maxscale.Text = "50";
                txtBx_Minscale.Text = "25";
            }
        }
        private void numUpDnSamplingTm_ValueChanged(object sender, EventArgs e)
        {

        }
        private void Recorder1_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmr.Stop();
            modbusobj.Port_Close();
        }

        private void txtBx_Minscale_TextChanged(object sender, EventArgs e)
        {
            SetValues.Set_MinGraphScale = txtBx_Minscale.Text;
        }

        private void txtBx_Maxscale_TextChanged(object sender, EventArgs e)
        {
            SetValues.Set_MaxGraphScale = txtBx_Maxscale.Text;
        }

        private void toolStripBtnSave_Click(object sender, EventArgs e)
        {
            filepath = @"ChartData.csv";
            StringBuilder sb = new StringBuilder();

            DataTable table1 = new DataTable();
            DataTable table2 = new DataTable();
            DataTable table3 = new DataTable();
            DataTable table4 = new DataTable();
            DataTable table5 = new DataTable();
            DataTable table6 = new DataTable();
            DataTable table7 = new DataTable();
            DataTable table8 = new DataTable();

            table1.Columns.Add("Time");
            table1.Columns.Add("Value");

            foreach (var row in mydictionary1)
            {
                table1.Rows.Add(row.Key, row.Value);
            }
            foreach (var row in mydictionary2)
            {
                table2.Rows.Add(row.Key, row.Value);
            }
            foreach (var row in mydictionary3)
            {
                table3.Rows.Add(row.Key, row.Value);
            }
            foreach (var row in mydictionary4)
            {
                table4.Rows.Add(row.Key, row.Value);
            }
            foreach (var row in mydictionary5)
            {
                table5.Rows.Add(row.Key, row.Value);
            }
            foreach (var row in mydictionary6)
            {
                table6.Rows.Add(row.Key, row.Value);
            }
            foreach (var row in mydictionary7)
            {
                table7.Rows.Add(row.Key, row.Value);
            }
            foreach (var row in mydictionary8)
            {
                table8.Rows.Add(row.Key, row.Value);
            }
            ToCSV(table1, filepath);
        }
        public static void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers  
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        private void toolStripBtnRecorder_Click(object sender, EventArgs e)
        {
            if (filepath != null || filepath != string.Empty)
            {
                try
                {
                    System.Diagnostics.Process _pr = new System.Diagnostics.Process();
                    _pr.StartInfo.FileName = filepath;
                    _pr.Start();
                }
                catch (FileNotFoundException ex)
                {

                }
                catch (Exception es)
                {

                }
            }
            else
            {
                MessageBox.Show("Path not found");
                return;
            }
        }

        private void numericUpDownDiv1_ValueChanged(object sender, EventArgs e)
        {

        }


    }
}
