using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ClassList;

namespace RTC_Communication_Utility
{
    public partial class Timer2 : Form
    {
        //IEnumerable<Control> controls = null;
        clsModbus modbusobj = null;
        bool breakFlag = true;
        public Timer2()
        {
            InitializeComponent();
            modbusobj = new clsModbus();
            modbusobj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);
            objList = new GraphPointsList();
            objListMain = new GraphPointsList();

            myNumericUpDown = new List<NumericUpDown>();
        }

        private void singlecmdtxt()
        {

        }

        private void Timer2_Load(object sender, EventArgs e)
        {
            try
            {
                txtSamplingTime.Minimum = 0.5M;
                txtSamplingTime.Maximum = 100;
                txtSamplingTime.Increment = 1;
                txtSamplingTime.DecimalPlaces = 1;


                BindColors();

                btnStart.Enabled = true;
                btnStop.Enabled = false;

                btnPause.Visible = false;
                pnlRecorder.Visible = false;
                lblRecording.Visible = false;

                recorder = false;
                lblFlag.Visible = false;

                cmbDisplayFormat.SelectedIndex = 0;
                cmbColors.SelectedIndex = 0;

                CreateChart();

                CreateDefaultChartSeries();
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Timer2Form, Timer2_Load()", ae.Message, "DTC_ErrorLog");
            }
        }

        private Thread cpuThread, th;
        private double[] cpuArray = new double[30];

        public int SleepTime { get; set; }

        string jsonRes = null;
        bool pauseRec = false;
        bool recordFlag = false;
        bool recorder = false;
        int annotationCounter = 0;

        string path = string.Empty;

        List<NumericUpDown> myNumericUpDown = null;
        Color[] colors = { Color.Orange, Color.Blue, Color.Black, Color.DarkMagenta };

        GraphPointsList objList = null;
        GraphPointsList objListMain = null;

        private void CreateChart()
        {
            try
            {
                int index = 0;

                cpuChart.ChartAreas[index].BackColor = Color.FromName(cmbColors.SelectedItem.ToString());

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
                    Color = System.Drawing.Color.Green,
                    IsVisibleInLegend = false,
                    IsXValueIndexed = true,
                    ChartType = SeriesChartType.Spline,
                    BorderWidth = 2
                };

                this.cpuChart.Series.Add(series1);

                //for (int i = 0; i < 25; i++)
                //{
                //    series1.Points.AddXY(i, randomNumber.Next(0, 25));
                //}
                series1.Points.AddXY(0, randomNumber.Next(0, 25));
                cpuChart.Invalidate();
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile(" Timer2Form, CreateDefaultChartSeries()", ae.Message, "DTC_ErrorLog");
            }
        }

        private void BindColors()
        {
            cmbColors.Items.Add("Black");
            cmbColors.Items.Add("Blue");
            cmbColors.Items.Add("Lime");
            cmbColors.Items.Add("Cyan");
            cmbColors.Items.Add("Red");
            cmbColors.Items.Add("Fuchsia");
            cmbColors.Items.Add("Yellow");
            cmbColors.Items.Add("White");
            cmbColors.Items.Add("Navy");
            cmbColors.Items.Add("Green");
            cmbColors.Items.Add("Teal");
            cmbColors.Items.Add("Maroon");
            cmbColors.Items.Add("Purple");
            cmbColors.Items.Add("Olive");
            cmbColors.Items.Add("Gray");
        }

        private void txtSamplingTime_ValueChanged(object sender, EventArgs e)
        {
            //if (txtSamplingTime.Value == 0)
            //{
            //    txtSamplingTime.Value = 0.5m;
            //}
        }

        private void cmbDisplayFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbDisplayFormat.SelectedIndex == 0)
                {
                    txtMinScale.Enabled = txtMaxScale.Enabled = false;
                }
                else
                {
                    txtMinScale.Enabled = txtMaxScale.Enabled = true;
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Timer2Form , cmbDisplayFormat_SelectedIndexChanged()", ae.Message, "DTC_ErrorLog");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                objList.PointsList.Clear();
                objListMain.PointsList.Clear();

                DisableControlsOnStart(false);

                ClearAllAnnotations();

                SetSleepTime();

                int nodes = CheckNodesToCreate();
               
                if (recorder)
                {
                    btnStart.Enabled = false;

                    btnPause.Enabled = false;

                    if (ReadFile())
                    {
                        if (trackBar1.Value == trackBar1.Maximum)
                        {
                            trackBar1.Value = 0;
                        }
                        SetIntervalForTimer();
                        annotationCounter = 0;
                        timer1.Start();
                    }
                }
                else
                {
                    StartGraphPlotting();
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile(" Timer2Form, btnStart_Click() )", ex.Message, "DTC_ErrorLog");
            }
        }

        private void SetIntervalForTimer()
        {
            timer1.Interval = SleepTime;
        }

        private void SetSleepTime()
        {
            SleepTime = (int)((Decimal)Convert.ToDouble(txtSamplingTime.Value) * 1000);
        }

        private bool ReadFile()
        {
            try
            {
                objList.PointsList.Clear();
                cpuChart.Series.Clear();

                if (!string.IsNullOrEmpty(path))
                {
                    if (File.Exists(path))
                    {
                        using (var tw = new StreamReader(path, true))
                        {
                            jsonRes = tw.ReadLine();
                            tw.Close();

                            if (!string.IsNullOrEmpty(jsonRes))
                            {
                                objList = JsonConvert.DeserializeObject<GraphPointsList>(jsonRes);

                                trackBar1.Maximum = objList.PointsList[0].Points.Count;
                                txtSamplingTime.Value = (decimal)Convert.ToDouble(objList.PointsList[0].SamplingTime);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
        }

        private int CheckNodesToCreate()
        {
            myNumericUpDown.Clear();

            if (numDwn1.Value > 0 && numDwn1.Value < 248) { myNumericUpDown.Add(numDwn1); } else { myNumericUpDown.Remove(numDwn1); }
            if (numDwn2.Value > 0 && numDwn2.Value < 248) { myNumericUpDown.Add(numDwn2); } else { myNumericUpDown.Remove(numDwn2); }
            if (numDwn3.Value > 0 && numDwn3.Value < 248) { myNumericUpDown.Add(numDwn3); } else { myNumericUpDown.Remove(numDwn3); }
            if (numDwn4.Value > 0 && numDwn4.Value < 248) { myNumericUpDown.Add(numDwn4); } else { myNumericUpDown.Remove(numDwn4); }
            if (numDwn5.Value > 0 && numDwn5.Value < 248) { myNumericUpDown.Add(numDwn5); } else { myNumericUpDown.Remove(numDwn5); }
            if (numDwn6.Value > 0 && numDwn6.Value < 248) { myNumericUpDown.Add(numDwn6); } else { myNumericUpDown.Remove(numDwn6); }
            if (numDwn7.Value > 0 && numDwn7.Value < 248) { myNumericUpDown.Add(numDwn7); } else { myNumericUpDown.Remove(numDwn7); }
            if (numDwn8.Value > 0 && numDwn8.Value < 248) { myNumericUpDown.Add(numDwn8); } else { myNumericUpDown.Remove(numDwn8); }
            if (numDwn9.Value > 0 && numDwn9.Value < 248) { myNumericUpDown.Add(numDwn9); } else { myNumericUpDown.Remove(numDwn9); }
            if (numDwn10.Value > 0 && numDwn10.Value < 248) { myNumericUpDown.Add(numDwn10); } else { myNumericUpDown.Remove(numDwn10); }

            cpuChart.Series.Clear();

            int counter = 1;

            foreach (var item in myNumericUpDown)
            {
                string seriesName = "Series" + counter++;

                CreateSeries(seriesName, item.Tag.ToString());
                //Fill List data
                objList.PointsList.Add(new GraphPoints() { PanelId = item.Name, SeriesName = seriesName, NodeAddress = item.Value.ToString(), Color = item.Tag.ToString(), SamplingTime = (txtSamplingTime.Value).ToString() });
                objListMain.PointsList.Add(new GraphPoints() { PanelId = item.Name, SeriesName = seriesName, NodeAddress = item.Value.ToString(), Color = item.Tag.ToString(), SamplingTime = (txtSamplingTime.Value).ToString() });
            }

            return 0;
        }

        private void CreateSeries(string seriesName, string color)
        {
            try
            {
                if (!string.IsNullOrEmpty(seriesName) && !string.IsNullOrEmpty(color))
                {
                    var series = new System.Windows.Forms.DataVisualization.Charting.Series
                    {
                        Name = seriesName,
                        Color = Color.FromName(color),
                        IsVisibleInLegend = false,
                        IsXValueIndexed = true,
                        ChartType = SeriesChartType.Spline,
                        BorderWidth = 2
                    };

                    this.cpuChart.Series.Add(series);
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Timer2Form, CreateSeries()", ae.Message, "DTC_ErrorLog ,Timer2Form");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (recorder)
                {
                    if (timer1.Enabled)
                    {
                        annotationCounter = 0;
                        timer1.Stop();

                        trackBar1.Value = trackBar1.Maximum;

                        btnPause.Enabled = false;

                        BindTrackBarValueToDisplay();
                    }
                }
                else
                {
                    StopGraphPlotting();
                }
                DisableControlsOnStart(true);

                int counter = 0;
                if (txtCounter.InvokeRequired)
                {
                    txtCounter.BeginInvoke((Action)(() => txtCounter.Text = counter.ToString().PadLeft(8, '0')));
                }
                else
                {
                    txtCounter.Text = counter.ToString().PadLeft(8, '0');
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Timer2Form, btnStop_Click() )", ae.Message, "DTC_ErrorLog");
            }
            
        }

        private void ClearAllAnnotations()
        {
            try
            {
                int anCount = cpuChart.Annotations.Count;
                if (anCount > 0)
                {
                    foreach (var item in cpuChart.Annotations)
                    {
                        cpuChart.BeginInvoke((Action)(() => { cpuChart.Annotations.Remove(item); }));
                    }
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile(" Timer2Form, ClearAllAnnotations() )", ae.Message, "DTC_ErrorLog");
            }
        }

        private void cmbColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbColors.SelectedItem.ToString()))
            {
                cpuChart.ChartAreas[0].BackColor = Color.FromName(cmbColors.SelectedItem.ToString());
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //check if recorder or live graph is running,
            //stop it and/or close this form
            try
            {
                if (modbusobj != null && modbusobj.IsSerialPortOpen())
                {
                    modbusobj.CloseSerialPort();
                }

                cpuThread.Abort();


                btnStop_Click(sender, e);

                this.Close();
            }
            catch (Exception ae)
            {
                btnStop_Click(sender, e);

                this.Close();
            }

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                recorder = !recorder;

                pnlRecorder.Visible = recorder;
                btnPause.Enabled = false;

                if (recorder)
                {
                    bool returnedVal = OpenGraphFile();

                    recorder = returnedVal;

                    pnlRecorder.Visible = recorder;
                    btnPause.Enabled = returnedVal;
                    ReadFile(); // As per DTC While read File Add Sampling Time before start
                    //controls = GetAll(this, typeof(NumericUpDown));
                    //MessageBox.Show("Total Controls: " + controls.Count());
                    ClearAllAnnotations();
                }
            }catch(Exception ae)
            {
                LogWriter.WriteToFile("Timer2Form , btnOpen_Click() )", ae.Message, "DTC_ErrorLog");
            }

        }

        private bool OpenGraphFile()
        {
            try
            {
                //Browse the file with recorded graph data
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt";
                openDialog.InitialDirectory = @"D:\jsonFiles";
                openDialog.RestoreDirectory = true;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    path = openDialog.FileName;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Timer2Form ,OpenGraphFile() )", ae.Message, "DTC_ErrorLog");
                return false;
               
            }
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            try
            {
                recordFlag = !recordFlag;

                if (recordFlag)
                {
                    //Open dialog for file location and save data to it
                    // path = @"D:\jsonFiles\jsonPoints6.json";  working 
                    //string pathdata = string.Empty;
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    // saveFileDialog1.InitialDirectory = @"C:";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        path = saveFileDialog1.FileName + ".json";
                    }

                }

                lblFlag.Visible = recordFlag;
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile(" Tmer2, btnRecord_Click() )", ae.Message, "DTC_ErrorLog");
            }
        }

        private void StartGraphPlotting()
        {
            try
            {
                cpuThread = new Thread(new ThreadStart(this.getPerformanceCounters));

                cpuThread.IsBackground = true;

                cpuThread.Start();
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile(" Timer2Form, StartGraphPlotting() )", ae.Message, "DTC_ErrorLog");
            }
        }

        private void DisableControlsOnStart(bool flag)
        {
            txtSamplingTime.Enabled = flag;

            cmbDisplayFormat.Enabled = flag;

            btnStart.Enabled = flag;
            btnStop.Enabled = !flag;

            //cmbColors.Enabled = flag;

            panel1.Enabled = flag;
            panel2.Enabled = flag;
            panel3.Enabled = flag;
            panel4.Enabled = flag;
            panel5.Enabled = flag;
            panel6.Enabled = flag;
            panel7.Enabled = flag;
            panel8.Enabled = flag;
            panel9.Enabled = flag;
            panel10.Enabled = flag;

            btnOpen.Enabled = flag;
            btnRecord.Enabled = flag;
            //btnClose.Enabled = !flag;
        }

        private void getPerformanceCounters()
        {
            try
            {
                //var cpuPerfCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
                //DisplayToChart(cpuPerfCounter);

                DisplayToChart2();
            }
            catch (Exception ex)
            {

            }
        }

        private void DisplayToChart(PerformanceCounter cpuPerfCounter)
        {
            try
            {
                Random randomNumber = new Random();

                double counter = 1;
                double counterMain = 1;

                while (true)
                {
                    if (objList != null && objListMain != null)
                    {
                        double value = Math.Round(cpuPerfCounter.NextValue(), 0);

                        for (int i = 0; i < objListMain.PointsList.Count; i++)
                        {
                            double pointValue = Math.Round((value * randomNumber.Next(1, 3)), 2);

                            string pvValue = objListMain.PointsList[i].PanelId;

                            BindTextBoxesInPanel(pointValue, pvValue);

                            objListMain.PointsList[i].Points.Add(counterMain, pointValue);

                            double chartYMax = cpuChart.ChartAreas[0].AxisY.Maximum;
                            double chartYMin = cpuChart.ChartAreas[0].AxisY.Minimum;

                            double chartXMax = cpuChart.ChartAreas[0].AxisX.Maximum;
                            double chartXMin = cpuChart.ChartAreas[0].AxisX.Minimum;

                            #region AdjustMaxScale
                            if (pointValue > chartYMax)
                            {
                                double newValue = pointValue + 10;
                                cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisY.Maximum = newValue; }));

                                txtMaxScale.BeginInvoke((Action)(() => { txtMaxScale.Text = newValue.ToString(); }));
                            }
                            if (pointValue > chartXMax)
                            {
                                cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisX.Maximum = pointValue; }));
                            }
                            #endregion

                            #region AdjustMinScale
                            if (pointValue < chartYMin)
                            {
                                double newValue = pointValue - 10;
                                cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisY.Minimum = newValue; }));
                                txtMaxScale.BeginInvoke((Action)(() => { txtMinScale.Text = newValue.ToString(); }));
                            }
                            if (pointValue < chartXMin)
                            {
                                cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisX.Minimum = pointValue; }));
                            }
                            #endregion
                            if (recordFlag)
                            {
                                objList.PointsList[i].Points.Add(counterMain, pointValue);

                                counter++;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                    if (recordFlag)
                    {
                        lblRecording.BeginInvoke((Action)(() => lblRecording.Visible = !(lblRecording.Visible)));
                    }

                    if (cpuChart.IsHandleCreated)
                    {
                        this.Invoke((MethodInvoker)delegate { UpdateCpuchart(objListMain); });
                    }
                    else
                    {
                        // 
                        break;
                    }

                    txtCounter.BeginInvoke((Action)(() => txtCounter.Text = counterMain.ToString().PadLeft(8, '0')));

                    Thread.Sleep(SleepTime);

                    counterMain++;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error occurred: " + ex.Message);
            }
        }

        private void DisplayToChart2()
        {
            try
            {
              
                Random randomNumber = new Random();
                annotationCounter = 0;
               // MessageBox.Show(annotationCounter.ToString());
                double counter = 1;
                double counterMain = 1;
                int breakCount = 0;
                while (breakFlag)
                {
                    if (objList != null && objListMain != null)
                    {
                        //double value = Math.Round(cpuPerfCounter.NextValue(), 0);
                        //double value = 0;
                      
                        for (int i = 0; i < objListMain.PointsList.Count; i++)
                        {
                            if (breakFlag)
                            {
                                string nodeAddress = objListMain.PointsList[i].NodeAddress;

                                if (!string.IsNullOrEmpty(nodeAddress))
                                {
                                    // 1: connect USB/Serial
                                    // 2: send query
                                    // 3: decode query

                                    double pointValue = 0; // Math.Round((value * randomNumber.Next(1, 3)), 2);

                                    bool? valRe = SendFrameToDevice1(nodeAddress, "03", "4700", "0001", ref pointValue);

                                    if (valRe == true)
                                    {
                                        string pvValue = objListMain.PointsList[i].PanelId;

                                        BindTextBoxesInPanel(pointValue, pvValue);
                                        double xcount = Convert.ToDouble(txtCounter.Text);
                                       // MessageBox.Show(pointValue.ToString());
                                       // MessageBox.Show(pvValue.ToString());

                                        objListMain.PointsList[i].Points.Add(counterMain, pointValue);

                                        double chartYMax = cpuChart.ChartAreas[0].AxisY.Maximum;
                                        double chartYMin = cpuChart.ChartAreas[0].AxisY.Minimum;

                                        double chartXMax = cpuChart.ChartAreas[0].AxisX.Maximum;
                                        double chartXMin = cpuChart.ChartAreas[0].AxisX.Minimum;

                                        #region AdjustMaxScale
                                       if (pointValue > chartYMax)
                                        {
                                            double newValue = pointValue + 20; //10
                                            cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisY.Maximum = newValue; }));

                                            txtMaxScale.BeginInvoke((Action)(() => { txtMaxScale.Text = newValue.ToString(); }));
                                        }
                                        if (xcount > chartXMax)
                                        {
                                            double newValue = chartXMax + 20; //10
                                            cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisX.Maximum = newValue; }));
                                        }
                                        #endregion

                                        #region AdjustMinScale
                                         if (pointValue < chartYMin)  //Bhushan Comment As per DTC SW Change
                                        {
                                            double newValue = pointValue - 20; //10
                                            cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisY.Minimum = newValue; }));
                                            txtMaxScale.BeginInvoke((Action)(() => { txtMinScale.Text = newValue.ToString(); }));
                                        }
                                        if (pointValue < chartXMin)
                                        {
                                            cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisX.Minimum = 0; })); //pointValue bhushan Tested
                                        }
                                        #endregion

                                        if (recordFlag)
                                        {
                                            objList.PointsList[i].Points.Add(counterMain, pointValue);

                                            counter++;
                                        }

                                        if (annotationCounter % 60 == 0)
                                        {
                                            CreateAndAddAnnotation(pointValue, annotationCounter);
                                        }
                                    }
                                    else if (valRe == false)
                                    {
                                        breakCount++;

                                        if (breakCount == 3) { breakFlag = false; }
                                        //this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { btnStart.Enabled = true; });
                                        //this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { btnStop.Enabled = false; });
                                       
                                    }
                                    else if (valRe == null)
                                    {
                                        //MessageBox.Show("Connection lost.");//SG
                                        //breakFlag = false;//SG
                                    }
                                }
                            }
                            else
                                break;
                        }
                        annotationCounter++;

                    }
                    else
                    {
                        break;
                    }

                    if (recordFlag)
                    {
                        lblRecording.BeginInvoke((Action)(() => lblRecording.Visible = !(lblRecording.Visible)));
                    }

                    if (cpuChart.IsHandleCreated)
                    {
                        this.Invoke((MethodInvoker)delegate { UpdateCpuchart(objListMain); });
                    }
                    else
                    {
                        // 
                        break;
                    }

                    txtCounter.BeginInvoke((Action)(() => txtCounter.Text = counterMain.ToString().PadLeft(8, '0')));

                    Thread.Sleep(SleepTime);

                    counterMain++;
                }

                if (breakFlag == false)
                {
                    //btnStop_Click(null, null);
                    //StopGraphPlotting();
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile(" Timer2Form, DisplayToChart2() )", ex.Message, "DTC_ErrorLog");
                //MessageBox.Show("Error occurred: " + ex.Message);
            }
        }

        private void UpdateCpuchart(GraphPointsList objList)
        {
            try
            {
                ClearAllSeriesPoints();

                foreach (GraphPoints item in objList.PointsList)
                {
                    for (int i = 1; i < item.Points.Count; i++)
                    {
                        cpuChart.Series[item.SeriesName].Points.Add(item.Points[i]);
                    }
                }
                cpuChart.ChartAreas[0].RecalculateAxesScale();
                if (cpuChart.ChartAreas[0].AxisX.Maximum > cpuChart.ChartAreas[0].AxisX.ScaleView.Size)
                    cpuChart.ChartAreas[0].AxisX.ScaleView.Scroll(cpuChart.ChartAreas[0].AxisX.Maximum);

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

        private void StopGraphPlotting()
        {
            try
            {
                if (cpuThread != null && cpuThread.IsAlive)
                {
                    cpuThread.Abort();

                    if (recordFlag)
                    {
                        bool saved = AddToFile();
                      //  AddToFileXML();
                        if (saved)
                        {
                            MessageBox.Show("Saved to file");
                        }
                    }
                }
                lblRecording.Visible = false;
            }
            catch (ThreadAbortException exx)
            {
                Debug.Write(exx.Message);
                //AddToFile();
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Timer2Form, StopGraphPlotting )", ex.Message, "DTC_ErrorLog");
            }
        }

        private bool AddToFileXML()
        {
            try
            {
                if (objList != null)
                {
                    int numberOfNode = 0;
                    string FileName = string.Empty;
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        FileName = saveFileDialog1.FileName + ".xls";
                    }
                    FileStream stream = new FileStream(FileName, FileMode.OpenOrCreate);
                    ExcelWriter writer = new ExcelWriter(stream);
                    writer.BeginWrite();
                    while (numberOfNode < objList.PointsList.Count)
                    {
                        try
                        {
                            var listkey = new List<double>(objList.PointsList[numberOfNode].Points.Keys);
                            var listvalue = new List<double>(objList.PointsList[numberOfNode].Points.Values);
                          ///  MessageBox.Show(objList.PointsList[numberOfNode].Points.Count.ToString());

                            if (numberOfNode == 0)
                            {
                                writer.WriteCell(0, 0, "sample Count node 1");
                                writer.WriteCell(0, 1, "PV Value");
                                int xd = 1, z = 1;
                                foreach (double key in listkey)
                                {
                                    writer.WriteCell(xd, 0, (key).ToString());
                                    xd++;
                                }

                                foreach (double val in listvalue)
                                {
                                    writer.WriteCell(z, 1, (val).ToString());
                                    z++;
                                }
                            }
                            if (numberOfNode == 1)
                            {
                                writer.WriteCell(0, 3, "sample Count node 2");
                                writer.WriteCell(0, 4, "PV Value");
                                int xd = 1, z = 1;
                                foreach (double key in listkey)
                                {
                                    writer.WriteCell(xd, 3, (key).ToString());
                                    xd++;
                                }

                                foreach (double val in listvalue)
                                {
                                    writer.WriteCell(z, 4, (val).ToString());
                                    z++;
                                }
                            }

                            if (numberOfNode == 2)
                            {

                                int xd = 1, z = 1;
                                foreach (double key in listkey)
                                {
                                    writer.WriteCell(xd, 6, (key).ToString());
                                    xd++;
                                }

                                foreach (double val in listvalue)
                                {
                                    writer.WriteCell(z, 7, (val).ToString());
                                    z++;
                                }
                            }
                            if (numberOfNode == 3)
                            {

                                int xd = 1, z = 1;
                                foreach (double key in listkey)
                                {
                                    writer.WriteCell(xd, 9, (key).ToString());
                                    xd++;
                                }

                                foreach (double val in listvalue)
                                {
                                    writer.WriteCell(z, 10, (val).ToString());
                                    z++;
                                }
                            }

                        }
                        catch (Exception ae)
                        {
                            
                            LogWriter.WriteToFile("Timer2Form, AddToFileXML )", ae.Message, "DTC_ErrorLog");
                        }
                      
                        numberOfNode++;
                       
                    }
                    writer.EndWrite();
                    stream.Close();
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        private bool AddToFile()
        {
            try
            {
                if (objList != null)
                {
                    string JSONres = JsonConvert.SerializeObject(objList);
                  //  MessageBox.Show(JSONres);
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (File.Exists(path))
                        {
                            File.SetAttributes(path, FileAttributes.Normal);
                            File.Delete(path);
                            using (var tw = new StreamWriter(path, true))
                            {
                                tw.WriteLine(JSONres.ToString());
                                tw.Close();
                            }
                        }
                        else if (!File.Exists(path))
                        {
                            using (var tw = new StreamWriter(path, true))
                            {
                                tw.WriteLine(JSONres.ToString());
                                tw.Close();
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
               LogWriter.WriteToFile("Timer2Form, AddToFile)", ex.Message, "DTC_ErrorLog");
               return false;
            }

        }

        private void UpdateCpuchart()
        {
            try
            {
                cpuChart.Series["Series1"].Points.Clear();

                for (int i = 0; i < cpuArray.Length - 1; i++)
                {
                    cpuChart.Series["Series1"].Points.Add(cpuArray[i]);
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Timer2Form, UpdateCpuchart )", ae.Message, "DTC_ErrorLog");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //annotationCounter = 0;
            try
            {
                ClearAllSeriesPoints();

                if (trackBar1.Value < trackBar1.Maximum)
                {
                    ++trackBar1.Value;
                    BindTrackBarValueToDisplay();
                }

                //Thread.Sleep(Convert.ToInt32(txtSamplingTime.Value));

                Thread.Sleep(SleepTime);

                if (trackBar1.Value == trackBar1.Maximum)
                {
                    timer1.Stop();

                    DisableControlsOnStart(true);
                }

                btnPause.Visible = true;
                btnPause.Enabled = true;
            }
            catch (Exception ex)
            {
                LogWriter.WriteToFile("Timer2Form, timer1_Tick() )", ex.Message, "DTC_ErrorLog");
            }
        }

        private void BindTrackBarValueToDisplay()
        {
            txtCounter.Text = Convert.ToString(trackBar1.Value).PadLeft(8, '0');

            DisplayPointsOnChart(trackBar1.Value);
        }

        private void DisplayPointsOnChart(int val)
        {
            try
            {
                //annotationCounter = 0;
                if (ReadFile())
                {
                    
                    if (objList != null && objList.PointsList.Count > 0)
                    {
                       
                        foreach (var item in objList.PointsList)
                        {
                            if (item.Points.Count > 0)
                            {
                                CreateSeries(item.SeriesName, item.Color);

                                string name = item.PanelId;

                                switch (name)
                                {
                                    case "numDwn1":
                                        numDwn1.Value = Convert.ToInt32(item.NodeAddress);
                                        break;
                                    case "numDwn2":
                                        numDwn2.Value = Convert.ToInt32(item.NodeAddress);
                                        break;
                                    case "numDwn3":
                                        numDwn3.Value = Convert.ToInt32(item.NodeAddress);
                                        break;
                                    case "numDwn4":
                                        numDwn4.Value = Convert.ToInt32(item.NodeAddress);
                                        break;
                                    case "numDwn5":
                                        numDwn5.Value = Convert.ToInt32(item.NodeAddress);
                                        break;
                                    case "numDwn6":
                                        numDwn6.Value = Convert.ToInt32(item.NodeAddress);
                                        break;
                                    case "numDwn7":
                                        numDwn7.Value = Convert.ToInt32(item.NodeAddress);
                                        break;
                                    case "numDwn8":
                                        numDwn8.Value = Convert.ToInt32(item.NodeAddress);
                                        break;
                                    case "numDwn9":
                                        numDwn9.Value = Convert.ToInt32(item.NodeAddress);
                                        break;
                                    case "numDwn10":
                                        numDwn10.Value = Convert.ToInt32(item.NodeAddress);
                                        break;
                                }
                            }
                        }

                        foreach (var item in objList.PointsList)
                        {
                            if (item.Points.Count > 0)
                            {
                                if (cpuChart.Series[item.SeriesName].Points.Count < val)
                                {
                                    for (int i = 1; i <= val; i++)
                                    {
                                        double pointValue = item.Points[i];

                                        string pvValue = item.PanelId;

                                        BindTextBoxesInPanel(pointValue, pvValue);

                                        //double chartYMax = cpuChart.ChartAreas[0].AxisY.Maximum;
                                        //double chartYMin = cpuChart.ChartAreas[0].AxisY.Minimum;

                                        //if (pointValue > chartYMax)
                                        //{
                                        //    double newValue = pointValue + 10;

                                        //    cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisY.Maximum = newValue; }));
                                        //    txtMaxScale.BeginInvoke((Action)(() => { txtMaxScale.Text = newValue.ToString(); }));
                                        //}

                                        //if (pointValue < chartYMin)
                                        //{
                                        //    double newValue = pointValue - 10;

                                        //    cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisY.Minimum = newValue; }));
                                        //    txtMaxScale.BeginInvoke((Action)(() => { txtMinScale.Text = newValue.ToString(); }));
                                        //}


                                        double chartYMax = cpuChart.ChartAreas[0].AxisY.Maximum;
                                        double chartYMin = cpuChart.ChartAreas[0].AxisY.Minimum;

                                        double chartXMax = cpuChart.ChartAreas[0].AxisX.Maximum;
                                        double chartXMin = cpuChart.ChartAreas[0].AxisX.Minimum;

                                        #region AdjustMaxScale
                                        if (pointValue > chartYMax)
                                        {
                                            double newValue = pointValue + 20;
                                            cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisY.Maximum = newValue; }));

                                            txtMaxScale.BeginInvoke((Action)(() => { txtMaxScale.Text = newValue.ToString(); }));
                                        }
                                        if (pointValue > chartXMax)
                                        {
                                            cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisX.Maximum = pointValue; }));
                                        }
                                        #endregion

                                        #region AdjustMinScale
                                        if (pointValue < chartYMin)
                                        {
                                            double newValue = pointValue - 20;
                                            cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisY.Minimum = newValue; }));
                                            txtMaxScale.BeginInvoke((Action)(() => { txtMinScale.Text = newValue.ToString(); }));
                                        }
                                        if (pointValue < chartXMin)
                                        {
                                            cpuChart.BeginInvoke((Action)(() => { cpuChart.ChartAreas[0].AxisX.Minimum = pointValue; }));
                                        }
                                        #endregion
                                        cpuChart.Series[item.SeriesName].Points.Add(pointValue);

                                        if ((annotationCounter % 60 == 0) && (i==1))
                                        {
                                            CreateAndAddAnnotation(pointValue, annotationCounter);
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    cpuChart.Invoke((Action)(() => { cpuChart.Series[item.SeriesName].Points.Add(item.Points[val + 1]); }));
                                }
                                
                            }//

                            
                        }
                        annotationCounter++;
                    }
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Timer2Form, DisplayPointsOnChart() )", ae.Message, "DTC_ErrorLog");
            }
        }

        private void BindTextBoxesInPanel(double yCord, string pvValue)
        {
            switch (pvValue)
            {
                case "numDwn1":
                    txtPV1.BeginInvoke((Action)(() => { txtPV1.Text = yCord.ToString("0.00"); }));
                    break;
                case "numDwn2":
                    txtPV2.BeginInvoke((Action)(() => { txtPV2.Text = yCord.ToString("0.00"); }));
                    break;
                case "numDwn3":
                    txtPV3.BeginInvoke((Action)(() => { txtPV3.Text = yCord.ToString("0.00"); }));
                    break;
                case "numDwn4":
                    txtPV4.BeginInvoke((Action)(() => { txtPV4.Text = yCord.ToString("0.00"); }));
                    break;
                case "numDwn5":
                    txtPV5.BeginInvoke((Action)(() => { txtPV5.Text = yCord.ToString("0.00"); }));
                    break;
                case "numDwn6":
                    txtPV6.BeginInvoke((Action)(() => { txtPV6.Text = yCord.ToString("0.00"); }));
                    break;
                case "numDwn7":
                    txtPV7.BeginInvoke((Action)(() => { txtPV7.Text = yCord.ToString("0.00"); }));
                    break;
                case "numDwn8":
                    txtPV8.BeginInvoke((Action)(() => { txtPV8.Text = yCord.ToString("0.00"); }));
                    break;
                case "numDwn9":
                    txtPV9.BeginInvoke((Action)(() => { txtPV9.Text = yCord.ToString("0.00"); }));
                    break;
                case "numDwn10":
                    txtPV10.BeginInvoke((Action)(() => { txtPV10.Text = yCord.ToString("0.00"); }));
                    break;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            ClearAllSeriesPoints();

            BindTrackBarValueToDisplay();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            try
            {
                string textName = btnPause.Text;

                btnPause.Text = (textName == "Resume") ? "Pause" : "Resume";

                switch (textName)
                {
                    case "Pause":
                        if (timer1.Enabled)
                        {
                            timer1.Stop();
                            txtSamplingTime.Enabled = true;
                        }
                        break;
                    case "Resume":
                        if (!timer1.Enabled)
                        {
                            txtSamplingTime.Enabled = false;

                            SetSleepTime();

                            SetIntervalForTimer();

                            timer1.Start();
                        }
                        break;
                }
            }
            catch (Exception ae)
            {
                LogWriter.WriteToFile("Timer2Form, btnPause_Click() )", ae.Message, "DTC_ErrorLog");
            }

        }

        public IEnumerable<Control> GetAll(Control control, Type type)
        {
           
                var controls = control.Controls.Cast<Control>();

                return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                          .Concat(controls)
                                          .Where(c => c.GetType() == type);
            
            
        }

        private bool? SendFrameToDevice1(string nodeAddress, string functionCode, string regAddress, string wordCount, ref double retValue)
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
                            else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex == 2)
                            {
                                RecieveData = modbusobj.RtuFrame(nodeAddress, functionCode, regAddress, wordCount,SetValues.Set_Baudrate);

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

        private static byte[] ExtractByteArray(byte[] RecieveData, int size, int startPostition)
        {
            byte[] sizeBytes = new byte[size];

            Array.Copy(RecieveData, startPostition, sizeBytes, 0, size);
            return sizeBytes;
        }

        private void CreateAndAddAnnotation(double a, double vall)
        {
            #region VerticalAnno
            VerticalLineAnnotation v1 = new VerticalLineAnnotation();
            v1.AxisX = cpuChart.ChartAreas[0].AxisX;
       
            v1.LineColor = Color.Red;
            v1.LineDashStyle = ChartDashStyle.Dash;
            v1.LineWidth = 1;
            v1.AllowMoving = false;
            v1.AllowSelecting = false;
            v1.AllowResizing = false;
            v1.X = vall;
           
            v1.IsInfinitive = true;
            
            RectangleAnnotation r1 = new RectangleAnnotation();
            r1.AxisX = cpuChart.ChartAreas[0].AxisX;
            r1.IsSizeAlwaysRelative = false;
            r1.Width = 20;  // 20 test Dtc
            r1.Height = 2;

            r1.BackColor = Color.Red;
            r1.LineColor = Color.Red;
            r1.AxisY = cpuChart.ChartAreas[0].AxisY;

            string time = System.DateTime.Now.ToLongTimeString();
            r1.Text = time;

            r1.ForeColor = Color.White;
            r1.Font = new System.Drawing.Font("Arial", 7f);
            r1.Y = -r1.Height;
            r1.X = v1.X - r1.Width / 2;

            RectangleAnnotation r2 = new RectangleAnnotation();
            r2.AxisX = cpuChart.ChartAreas[0].AxisX;
            r2.IsSizeAlwaysRelative = false;
            r2.Width = 10;
            r2.Height = 2;

            r2.BackColor = Color.FromArgb(128, Color.Transparent);
            r2.LineColor = Color.Transparent;
            r2.AxisY = cpuChart.ChartAreas[0].AxisY;
           // MessageBox.Show(a.ToString());
            r2.Text = a.ToString();

            r2.ForeColor = Color.Black;
            r2.Font = new System.Drawing.Font("Arial", 8f);

            r2.Y = a;
            r2.X = v1.X;

            cpuChart.Invoke((Action)(() =>
            {
                cpuChart.Annotations.Add(v1);
                cpuChart.Annotations.Add(r1);
                cpuChart.Annotations.Add(r2);
            }));

            #endregion
        }

      

      

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtCounter.Text = "0".PadLeft(8, '0');

        }
    }
}