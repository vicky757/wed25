using ClassList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace RTC_Communication_Utility
{
    public partial class RampSoakForm : Form
    {
        //public delegate void GetRampStart();
        //public event GetRampStart _getRampStart;

        //public delegate void GetRampStop();
        //public event GetRampStop _getRampStop;

        public delegate bool PauseItemDelegate(bool item);
        public PauseItemDelegate PauseItemCallback;

        public List<string> list { get; set; }

        int count = 1;

        bool progress = false;

        Thread th = null;

        public string NodeAddress { get; set; }

        public clsModbus modObj = null;

        DataSet dsFile = null;
        DataSet dsFile1 = null;
        DataTable dt2 = null;
        //string m_exePath1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string m_exePath1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
             "\\DefaultSettings.xml";
        List<List<string>> frameList = new List<List<string>>() 
        {
            new List<string>() { "0", "03", "4767", "0001" } ,
            new List<string>() { "1", "03", "4768", "0007" } ,
            new List<string>() { "2", "03", "4770", "0007" } ,
            new List<string>() { "3", "03", "4778", "0007" } ,
        };

        string[] linkPatterns = new string[8] { "pat0", "pat1", "pat2", "pat3", "pat4", "pat5", "pat6", "pat7" };

        List<RampSoakMap> lists = new List<RampSoakMap>();
        List<RampSoakMap> writeList = new List<RampSoakMap>();

        public RampSoakForm(clsModbus obj)
        {
            InitializeComponent();
            //NodeAddress = MonitorForm.rampSoakNodeAddress;
            modObj = obj;
            //modObj = new clsModbus();
            dsFile = new DataSet();
            dsFile1 = new DataSet();
            modObj._GetLRCResultResult += new clsModbus.GetLRCResult(singlecmdtxt);// no use 


        }

        static string selectedCell = "";

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBoxEx.Show("Test");
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {


                DataGridView senderGrid = (DataGridView)sender;
                int i = senderGrid.CurrentCell.RowIndex;

                int rowIndex = senderGrid.CurrentCell.RowIndex;
                int colIndex = senderGrid.CurrentCell.ColumnIndex;

                if (senderGrid.CurrentCell.ColumnIndex > 0)
                {
                    string selectedText = senderGrid[0, i].Value.ToString();

                    selectedCell = senderGrid.CurrentCell.RowIndex + "-" + senderGrid.CurrentCell.ColumnIndex;

                    string result = senderGrid[colIndex, rowIndex].Value.ToString();

                    Point p = new Point(21, 231);
                    if (selectedText.Contains("Link"))
                    {
                        if (string.IsNullOrEmpty(result))
                        {
                            cmbBxLinkPattern.SelectedIndex = -1;
                        }
                        else
                        {
                            var index = Array.FindIndex(linkPatterns, row => row.Contains(result));
                            if (Convert.ToInt32(index) > -1 && Convert.ToInt32(index) < 8)
                            {
                                cmbBxLinkPattern.SelectedIndex = Convert.ToInt32(index);
                            }
                        }

                        grpBxLinkPattern.Visible = true;
                        grpBxLinkPattern.BringToFront();

                        grpBxLinkPattern.Location = p;
                    }
                    else if (selectedText.Contains("Loop"))
                    {
                        txtLoopCount.Text = string.IsNullOrEmpty(result) ? "0" : result;

                        grpBxLoop.Visible = true;
                        grpBxLoop.BringToFront();
                        grpBxLoop.Location = p;
                    }
                    else if (selectedText.Contains("Step"))
                    {
                        if (string.IsNullOrEmpty(result))
                        {
                            cmbBxStep.SelectedIndex = -1;
                        }
                        else if (Convert.ToInt32(result) > -1 && Convert.ToInt32(result) < 8)
                        {
                            cmbBxStep.SelectedIndex = Convert.ToInt32(result);
                        }
                        grpBxStep.Visible = true;
                        grpBxStep.BringToFront();
                        grpBxStep.Location = p;
                    }
                    else if (selectedText.Contains("SV"))
                    {
                        txtSV.Text = string.IsNullOrEmpty(result) ? "0" : result;

                        grpBxSV.Visible = true;
                        grpBxSV.BringToFront();
                        grpBxSV.Location = p;
                    }
                    else if (selectedText.Contains("Time"))
                    {
                        string time = string.IsNullOrEmpty(result) ? "0" : result;
                        if (time.Equals("0"))
                        {
                            numericUpDownHr.Value = 0;
                            numericUpDownMin.Value = 0;
                        }
                        else
                        {
                            string[] times = time.Split(':');

                            int hrs = Convert.ToInt32(times[0]);
                            int mins = Convert.ToInt32(times[1]);

                            numericUpDownHr.Value = hrs;
                            numericUpDownMin.Value = mins;
                        }
                        grpBxTime.Visible = true;
                        grpBxTime.BringToFront();
                        grpBxTime.Location = p;
                    }

                    //MessageBoxEx.Show("i: "+senderGrid[colIndex, rowIndex].Value.ToString());
                }
            }
            catch (Exception)
            {
                //MessageBoxEx.Show("Something went wrong!! Try Again");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView senderGrid = (DataGridView)sender;

                int rowIndex = senderGrid.CurrentCell.RowIndex;
                int colIndex = senderGrid.CurrentCell.ColumnIndex;

                grpBxLinkPattern.Visible = false;
                grpBxLoop.Visible = false;
                grpBxStep.Visible = false;
                grpBxSV.Visible = false;
                grpBxTime.Visible = false;
                ////MessageBoxEx.Show(e.RowIndex + "-" + e.ColumnIndex);
                //MessageBoxEx.Show(senderGrid[colIndex, rowIndex].Tag.ToString());
            }
            catch (Exception ex)
            {
                //MessageBoxEx.Show("CellClick" + ex.Message);
            }
        }

        private void btnSV_Click(object sender, EventArgs e)
        {
            try
            {
                string text = txtSV.Text;

                if (string.IsNullOrEmpty(text))
                {
                    MessageBoxEx.Show("Please enter values");
                }
                else
                {
                    if (SetValueToCell(text, 2))
                    {
                        txtSV.Text = "";
                        grpBxSV.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBoxEx.Show("SV " + ex.Message);
            }
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            try
            {
                int text = cmbBxStep.SelectedIndex;

                if (text > -1)
                {
                    if (SetValueToCell(cmbBxStep.SelectedItem.ToString()))
                    {
                        cmbBxStep.SelectedIndex = 0;
                        grpBxStep.Visible = false;
                    }
                }
                else
                {
                    MessageBoxEx.Show("Please enter values");
                }
            }
            catch (Exception ex)
            {
                //MessageBoxEx.Show("Step " + ex.Message);
            }
        }

        private void btnLinkPattern_Click(object sender, EventArgs e)
        {
            try
            {
                int text = cmbBxLinkPattern.SelectedIndex;

                if (text > -1)
                {
                    if (SetValueToCell(cmbBxLinkPattern.SelectedItem.ToString()))
                    {
                        cmbBxLinkPattern.SelectedIndex = 0;
                        grpBxLinkPattern.Visible = false;
                    }
                }
                else
                {
                    MessageBoxEx.Show("Link " + "Please enter values");
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("Link " + ex.Message);
            }
        }

        private void btnLoopCount_Click(object sender, EventArgs e)
        {
            try
            {
                string text = txtLoopCount.Text;

                if (string.IsNullOrEmpty(text))
                {
                    MessageBoxEx.Show("Please enter values");
                }
                else
                {
                    if (SetValueToCell(text))
                    {
                        txtLoopCount.Text = "";
                        grpBxLoop.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("Loop " + ex.Message);
            }
        }

        private void btnTime_Click(object sender, EventArgs e)
        {
            try
            {
                int textHr = Convert.ToInt32(numericUpDownHr.Value);
                int textMin = Convert.ToInt32(numericUpDownMin.Value);

                if (textHr > 0 || textMin > 0)
                {
                    string value = textHr.ToString().PadLeft(2, '0') + ":" + textMin.ToString().PadLeft(2, '0');
                    if (SetValueToCell(value, 3))
                    {
                        numericUpDownHr.Value = 0;
                        numericUpDownMin.Value = 0;

                        grpBxTime.Visible = false;
                    }
                }
                else
                {
                    MessageBoxEx.Show("Please enter values");
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("Time " + ex.Message);
            }
        }

        private bool SetValueToCell(string text, int type = 0)
        {
            try
            {
                if (!string.IsNullOrEmpty(selectedCell))
                {
                    string[] indexes = selectedCell.Split('-');
                    int rowIndex = Convert.ToInt32(indexes[0]);
                    int colIndex = Convert.ToInt32(indexes[1]);

                    switch (type)
                    {
                        case 0:
                            dataGridView1[colIndex, rowIndex].Value = String.Format("{0}", text);
                            break;
                        case 2:
                            dataGridView1[colIndex, rowIndex].Value = String.Format("{0:0.0}", text);
                            break;
                        case 3:
                            dataGridView1[colIndex, rowIndex].Value = String.Format("{0:00:00}", text);
                            break;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("SetValueToCell " + ex.Message);
            }
            return false;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (th != null)
            {
                if (th.IsAlive)
                {
                    th.Abort();
                }
            }
            th = null;
            PauseItemCallback(false);
            this.Close();
        }

        private void Connect()
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

        private void btnReadSettings_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Enabled = true;
                btnReadSettings.Enabled = false;
                btnReadFile.Enabled = false;
                btnSaveAs.Enabled = false;
                btnWrite.Enabled = false;
                //_getRampStart();
                if (PauseItemCallback(true))
                {
                    Connect();

                    CreateGridWithTags();
                    ControlBox = false;
                    dataGridView1.CellBorderStyle =
                        DataGridViewCellBorderStyle.Sunken;

                    for (int i = 0; i < 19; i++)
                    {
                        dataGridView1.Rows[i].Cells[0].Style.BackColor = Color.LightGray;
                    }

                    lblBottomText.Text = "Reading..";
                    lblBottomText.Visible = true;
                    progressBar1.Visible = true;
                    lblBottomText.BackColor = Color.Green;
                    lblBottomText.ForeColor = Color.White;
                    th = null;
                    th = new Thread(Read);
                    th.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("Ramp ReadSettings:" + ex.Message);
            }
            finally
            {
                th = null;
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {

            btnReadSettings.Enabled = false;
            btnReadFile.Enabled = false;
            btnSaveAs.Enabled = false;
            btnWrite.Enabled = false;

            if (modObj != null)
            {
                if (modObj.IsSerialPortOpen())
                {
                    modObj.CloseSerialPort();
                }
            }

            Thread thread = null;

            try
            {
                if (PauseItemCallback(true))
                {
                    Connect();

                    lblBottomText.Text = "Writing..";
                    lblBottomText.Visible = true;
                    progressBar1.Visible = true;
                    lblBottomText.BackColor = Color.Green;
                    lblBottomText.ForeColor = Color.White;
                    ControlBox = false;
                    int rows = dataGridView1.Rows.Count;
                    int cols = dataGridView1.Columns.Count;

                    if (cmbBxStartPattern.SelectedIndex > -1)
                    {
                        writeList.Add(new RampSoakMap() { Key = "4767", Value = Convert.ToString(cmbBxStartPattern.SelectedIndex).PadLeft(4, '0') });
                    }

                    if (rows > 0 && cols > 0)
                    {
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            for (int j = 1; j < dataGridView1.Columns.Count; j++)
                            {
                                string key = dataGridView1.Rows[i].Cells[j].Tag.ToString();
                                string value = "0";

                                int val = int.Parse(key, System.Globalization.NumberStyles.HexNumber);

                                if (val >= 18280 && val <= 18287)
                                {
                                    //step
                                    //value = Convert.ToString(Convert.ToDecimal(Convert.ToString(dataGridView1.Rows[i].Cells[j].Value)) * 10).PadLeft(4, '0');// need to optimize
                                    value = (Convert.ToInt32(dataGridView1.Rows[i].Cells[j].Value)).ToString("X").PadLeft(4, '0');
                                }
                                else if (val >= 18288 && val <= 18295)
                                {
                                    //loop
                                    //value = Convert.ToString(Convert.ToDecimal(Convert.ToString(dataGridView1.Rows[i].Cells[j].Value)) * 10).PadLeft(4, '0');
                                    value = (Convert.ToInt32(dataGridView1.Rows[i].Cells[j].Value) ).ToString("X").PadLeft(4, '0');
                                }
                                   // * 10
                                else if (val >= 18296 && val <= 18303)
                                {
                                    //link
                                    //value = Convert.ToString(dataGridView1.Rows[i].Cells[j].Value).PadLeft(4, '0');
                                    int index = Array.FindIndex(linkPatterns, row => row.Contains(Convert.ToString(dataGridView1.Rows[i].Cells[j].Value)));
                                    value = (index).ToString("X").PadLeft(4, '0');
                                }
                                else if (val >= 18304 && val <= 18431)
                                {
                                    //SV ET
                                    string str = dataGridView1.Rows[i].Cells[j].Value.ToString();

                                    if (str.Contains(":"))
                                    {
                                        string[] res = str.Split(':');
                                        if (res.Length > 0)
                                        {
                                            int hr = Convert.ToInt32(res[0]);
                                            int min = Convert.ToInt32(res[1]);

                                            if (hr > 0)
                                            {
                                                min += (hr * 60);
                                            }

                                            value = min.ToString("X").PadLeft(4, '0');
                                        }
                                    }
                                    else
                                    {
                                        //value = Convert.ToString(Convert.ToDecimal(Convert.ToString(dataGridView1.Rows[i].Cells[j].Value)) * 10).PadLeft(4, '0');
                                        value = (Convert.ToInt32(Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) )).ToString("X").PadLeft(4, '0');
                                       // * 10
                                    }
                                }

                                writeList.Add(new RampSoakMap() { Key = key, Value = value });
                            }
                        }
                        thread = new Thread(() => WriteToDevice(writeList));
                        thread.Start();
                        // _getRampStart();
                      
                    }
                }
                //MessageBoxEx.Show(dataGridView1.Rows[18].Cells[8].Tag.ToString());
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("Write " + ex.Message);
            }
            finally
            {
                thread = null;
            }
        }

        private void WriteToDevice(List<RampSoakMap> writeList)
        {
            try
            {
                progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; }));
                int progress = 0;
                if (writeList.Count > 0)
                {
                    foreach (var item in writeList)
                    {
                        if (modObj != null && modObj.IsSerialPortOpen())
                        {
                            var list = CreateFrames(NodeAddress, "06", item.Key, item.Value, true);
                        }
                        if (progress <= 95)
                        {
                            progressBar1.Invoke((Action)(() => { progressBar1.Value = progress++; }));
                        }
                    }
                    progressBar1.Invoke((Action)(() => { progressBar1.Value = 100; }));
                    lblBottomText.Invoke((Action)(() => { lblBottomText.Visible = false; }));
                    progressBar1.Invoke((Action)(() => { progressBar1.Visible = false; }));
                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { ControlBox = true; });
                  
                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { btnReadFile.Enabled = true; });
                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { btnSaveAs.Enabled = true; });
                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { btnWrite.Enabled = true; });
                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { btnReadSettings.Enabled = true; });
                    MessageBoxEx.Show("Data uploaded to device successfully.");
                    //PauseItemCallback(false);
                }
                else
                {
                    MessageBoxEx.Show("Empty records");
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("Ramp WriteToDevice:" + ex.Message);
                PauseItemCallback(false);
            }
            //_getRampStop();
        }

        struct RampSoakMap
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private void Read()
        {
            try
            {
                progressBar1.Invoke((Action)(() => { progressBar1.Value = 0; }));

                lists.Clear();

                if (modObj != null && modObj.IsSerialPortOpen())
                {
                    //int i = 18279;
                    int progress = 0;
                    for (int i = 18279; i < 18432; i++)
                    {
                        var list = CreateFrames(NodeAddress, "03", i.ToString("X2"), "0001", true);

                        if (list != null)
                        {
                            lists.Add(new RampSoakMap()
                            {
                                Key = i.ToString("X2"),
                                Value = list
                            });
                            //dataGridView1.Invoke((Action)(() => { }));

                        }
                        if (progress <= 95)
                        {
                            progressBar1.Invoke((Action)(() => { progressBar1.Value = progress++; }));
                        }
                    }
                    progressBar1.Invoke((Action)(() => { progressBar1.Value = 100; }));
                    lblBottomText.Invoke((Action)(() => { lblBottomText.Visible = false; }));
                    progressBar1.Invoke((Action)(() => { progressBar1.Visible = false; }));
                    //return lists;
                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { ControlBox = true; });
                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { btnReadFile.Enabled = true; });
                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { btnSaveAs.Enabled = true; });
                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { btnWrite.Enabled = true; });
                    this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { btnReadSettings.Enabled = true; });
                   
                   
                    Globals<RampSoakMap>.lists = new List<RampSoakMap>();
                    Globals<RampSoakMap>.lists = lists;

                    Bind(lists);

                   // PauseItemCallback(false);
                   
                }
            }
            catch (Exception ex)
            {
                //PauseItemCallback(false);
                this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { ControlBox = true; });
                //MessageBoxEx.Show(ex.Message);
            }
        }

        private void CreateGridWithTags()
        {
            try
            {
                dataGridView1.Invoke((Action)(() => dataGridView1.DataSource = null));

                DataTable dt = new DataTable();
                dt.Columns.Add("Col");
                dt.Columns.Add("Pattern0");
                dt.Columns.Add("Pattern1");
                dt.Columns.Add("Pattern2");
                dt.Columns.Add("Pattern3");
                dt.Columns.Add("Pattern4");
                dt.Columns.Add("Pattern5");
                dt.Columns.Add("Pattern6");
                dt.Columns.Add("Pattern7");

                dt.Rows.Add(new object[] { "StepMax", "0", "0", "0", "0", "0", "0", "0", "0" });
                dt.Rows.Add(new object[] { "SV 0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" });
                dt.Rows.Add(new object[] { "Time 0", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00" });

                dt.Rows.Add(new object[] { "SV 1", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" });
                dt.Rows.Add(new object[] { "Time 1", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00" });

                dt.Rows.Add(new object[] { "SV 2", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" });
                dt.Rows.Add(new object[] { "Time 2", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00" });

                dt.Rows.Add(new object[] { "SV 3", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" });
                dt.Rows.Add(new object[] { "Time 3", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00" });

                dt.Rows.Add(new object[] { "SV 4", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" });
                dt.Rows.Add(new object[] { "Time 4", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00" });

                dt.Rows.Add(new object[] { "SV 5", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" });
                dt.Rows.Add(new object[] { "Time 5", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00" });

                dt.Rows.Add(new object[] { "SV 6", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" });
                dt.Rows.Add(new object[] { "Time 6", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00" });

                dt.Rows.Add(new object[] { "SV 7", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" });
                dt.Rows.Add(new object[] { "Time 7", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00", "00:00" });

                dt.Rows.Add(new object[] { "Loop Count", "0", "0", "0", "0", "0", "0", "0", "0" });
                dt.Rows.Add(new object[] { "Link Pattern", linkPatterns[0], linkPatterns[0], linkPatterns[0], linkPatterns[0],
                linkPatterns[0], linkPatterns[0], linkPatterns[0], linkPatterns[0] });

                dataGridView1.Invoke((Action)(() => dataGridView1.DataSource = dt));
                dataGridView1.Invoke((Action)(() =>
                {
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }

                    AddTagsToGrid();

                    dataGridView1.Rows[0].Cells[0].Tag = "-1";
                }));
            }
            catch (Exception ex)
            {
                //MessageBoxEx.Show("CreateGridWithTags " + ex.Message);
            }
        }

        private void AddTagsToGrid()
        {
            try
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    //int startPattern = 18279;
                    //dataGridView1.Rows[0].Cells[0].Tag = startPattern.ToString("X2");

                    int valStep = 18280; // 4768 - 467F
                    for (int j = 1; j <= 8; j++)
                    {
                        dataGridView1.Rows[0].Cells[j].Tag = valStep.ToString("X2");
                        valStep++;
                    }

                    int valLoop = 18288; // 4770 - 4777
                    for (int j = 1; j <= 8; j++)
                    {
                        dataGridView1.Rows[17].Cells[j].Tag = valLoop.ToString("X2");
                        valLoop++;
                    }

                    int valLink = 18296; // 4778 - 477F
                    for (int j = 1; j <= 8; j++)
                    {
                        dataGridView1.Rows[18].Cells[j].Tag = valLink.ToString("X2");
                        valLink++;
                    }

                    int val = 18304; //4780 - 47FF
                    for (int i = 1; i <= 8; i++)
                    {
                        for (int j = 1; j <= 16; j++)
                        {
                            dataGridView1.Rows[j].Cells[i].Tag = val.ToString("X2");
                            val++;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
               // MessageBoxEx.Show("Something went wrong!! Try Again");
            }
        }

        private void Bind(List<RampSoakMap> lists)
        {
            try
            {
                CreateGridWithTags();

                int k = 0;
                if (lists != null)
                {
                    if (lists.Count > 0) //18279
                    {
                        if (lists[k].Key == "4767")
                        {
                            int val = string.IsNullOrEmpty(lists[k].Value) ?
                                -1 : Convert.ToInt32(lists[k].Value);

                            if (val > -1 && val < 8)
                            {
                                cmbBxStartPattern.Invoke((Action)(() =>
                                {
                                    cmbBxStartPattern.SelectedIndex = val;
                                }));
                            }
                        }

                        //step
                        int step = 1;
                        for (int j = 1; j <= 8; j++)
                        {
                            if (lists[step].Key == Convert.ToString(dataGridView1.Rows[0].Cells[j].Tag))
                            {
                                dataGridView1.Rows[0].Cells[j].Value =
                                    String.Format("{0:0}", Convert.ToDouble(
                                   (Convert.ToInt64(lists[step].Value, 16))) / 1);
                            }
                            step++;
                        }

                        //Loop = 18288; // 4770 - 4777
                        int loop = 9;
                        for (int j = 1; j <= 8; j++)
                        {
                            if (lists[loop].Key == Convert.ToString(dataGridView1.Rows[17].Cells[j].Tag))
                            {
                                dataGridView1.Rows[17].Cells[j].Value = String.Format("{0:0}", Convert.ToDouble(
                                   (Convert.ToInt64(lists[loop].Value, 16))) / 10);
                            }
                            loop++;
                        }

                        //Link = 18296; // 4778 - 477F
                        int link = 17;
                        for (int j = 1; j <= 8; j++)
                        {
                            if (lists[link].Key == Convert.ToString(dataGridView1.Rows[18].Cells[j].Tag))
                            {
                                int res = Convert.ToInt32(Convert.ToInt64(lists[link].Value, 16));

  

                                dataGridView1.Rows[18].Cells[j].Value = Convert.ToString(linkPatterns[res]);
                            }
                            link++;
                        }

                        //val = 18304; //4780 - 47FF
                        int vall = 25;
                        for (int i = 1; i <= 8; i++)
                        {
                            for (int j = 1; j <= 16; j++)
                            {
                                //MessageBoxEx.Show("key:" + lists[vall].Key + "Value:" + Convert.ToString(dataGridView1.Rows[j].Cells[i].Tag) +
                                //    "i:" + i.ToString() + "j:" + j.ToString());



                                if (lists[vall].Key == Convert.ToString(dataGridView1.Rows[j].Cells[i].Tag))
                                {

                                    // to check if its SV or time values
                                    if (j % 2 == 0)
                                    {
                                        Int64 res = (Convert.ToInt64(lists[vall].Value, 16));

                                        if (res == 0)
                                        {
                                            dataGridView1.Rows[j].Cells[i].Value = String.Format("{0:00:00}", Convert.ToDouble(res));
                                        }
                                        else
                                        {
                                            if (res >= 60)
                                            {
                                                int h = Convert.ToInt32(res / 60);
                                                int m = Convert.ToInt32(Convert.ToDouble(res) % 60);
                                                dataGridView1.Rows[j].Cells[i].Value = h + ":" + m;

                                                //dataGridView1.Rows[i].Cells[j].Value =
                                                //    String.Format("{0:00:00}", (Convert.ToDouble(res) / 60) * 100);
                                            }
                                            else
                                            {
                                                dataGridView1.Rows[j].Cells[i].Value =
                                                    String.Format("{0:00:00}", Convert.ToDouble(res));  //* 100
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string v = String.Format("{0:0.0}", Convert.ToDouble(
                                        (Convert.ToInt64(lists[vall].Value, 16))));   // / 10 Ritesh Discuss

                                        dataGridView1.Rows[j].Cells[i].Value = (v == "0" ? "0.0" : v);
                                    }
                                    vall++;
                                }

                            }
                        }
                    }
                }

                // _getRampStop();
            }
            catch (Exception ex)
            {
                //MessageBoxEx.Show("Bind " + ex.Message + " " + ex.StackTrace);
                //_getRampStop();
            }
        }

        // no use
        public void singlecmdtxt()
        {
            string lrc = SetValues.Set_LRCFrame;
            string ark = SetValues.Set_ASKFrame;// +SetValues.Set_LRCFrame;
        }

        private void btnReadFile_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtR = new DataTable();
                dataGridView1.DataSource = null;
                dataGridView1.Refresh();

                String strFileName = string.Empty;
                OpenFileDialog dialog = new OpenFileDialog();

                //Now we set the file type we want to be available to the user. In this case, text files.  
                //dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

                //Next is the starting directory for the dialog and the title for the dialog box are set.
                dialog.InitialDirectory = "C:";
                dialog.Title = "Select a text file";

                //Once the dialog properties are set, it is ready to present to the user.  
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    dtR.Clear();
                    dsFile.Clear();

                    strFileName = dialog.FileName;

                    if (strFileName == String.Empty) return;

                    dsFile.ReadXml(strFileName);

                    XElement xEmp1 = XElement.Load(strFileName);

                    bool b = xEmp1.Descendants("NewSettings").Any();

                    if (b)
                    {
                        if (dsFile.Tables.Count > 0)
                        {
                            dtR = dsFile.Tables[0];
                            if (dtR != null && dtR.Rows.Count > 0)
                            {
                                dataGridView1.CellBorderStyle =
                                        DataGridViewCellBorderStyle.Sunken;
                                dataGridView1.DataSource = dtR;
                                dataGridView1.Refresh();
                                for (int i = 0; i < 19; i++)
                                {
                                    dataGridView1.Rows[i].Cells[0].Style.BackColor = Color.LightGray;
                                }
                                dataGridView1.Refresh();
                            }
                            else
                            {
                                MessageBoxEx.Show("File is empty");
                            }
                        }
                    }
                    else
                    {
                        MessageBoxEx.Show("File data is invalid");
                    }
                }

                AddTagsToGrid();

                XElement xEmp = XElement.Load(m_exePath1);

                var empDetails = from emps in xEmp.Elements("rampSoak")
                                 select emps;

                cmbBxStartPattern.SelectedIndex = empDetails != null ? Convert.ToInt32(empDetails.First().Element("start").Value) : -1;

                dsFile1.ReadXml(m_exePath1);
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("ReadFile " + ex.Message);
            }
        }

        class clsTags
        {
            public string Name { get; set; }
            public int MyProperty { get; set; }
        }

        private void RampSoakForm_Load(object sender, EventArgs e)
        {
            try
            {
              
                dataGridView1.DataSource = null;
                btnReadSettings.Enabled = false;
                btnReadFile.Enabled = false;
                btnSaveAs.Enabled = false;
                btnWrite.Enabled = false;
                cmbBxStartPattern.SelectedIndex = 0;
                NodeAddress = Convert.ToInt32(NodeAddress) == 0 ? "1" : NodeAddress;

                if (!string.IsNullOrEmpty(NodeAddress))
                {
                    if (Convert.ToInt32(NodeAddress) > 0)
                    {
                        btnReadSettings.Enabled = true;
                        btnReadFile.Enabled = true;
                        btnSaveAs.Enabled = true;
                        btnWrite.Enabled = true;

                        lblBottomText.Visible = false;
                        progressBar1.Visible = false;

                        grpBxLinkPattern.Visible = false;
                        grpBxLoop.Visible = false;
                        grpBxStep.Visible = false;
                        grpBxSV.Visible = false;
                        grpBxTime.Visible = false;

                        if (Globals<RampSoakMap>.lists != null)
                        {
                            if (Globals<RampSoakMap>.lists.Count > 0)
                            {
                                CreateGridWithTags();

                                dataGridView1.CellBorderStyle =
                                    DataGridViewCellBorderStyle.Sunken;

                                for (int i = 0; i < 19; i++)
                                {
                                    dataGridView1.Rows[i].Cells[0].Style.BackColor = Color.LightGray;
                                }

                                Bind(Globals<RampSoakMap>.lists);
                            }
                        }
                    }
                    else
                    {

                    }
                }
                dataGridView1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("Load: " + ex.Message);
            }
        }

        private string CreateFrames(string nodeAddress, string functionCode, string regAddress, string wordCount, bool read)
        {
            
            byte[] RecieveData = null;
            int GotoCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(SetValues.Set_CommunicationProtocol))
                {
                    if (SetValues.Set_CommunicationProtocol.ToLower() == "ascii"|| SetValues.Set_CommunicationProtocolindex == 1)
                    {
                       ///bbbb
                    startagain:
                        RecieveData = modObj.AscFrame(nodeAddress,
                            functionCode, regAddress, wordCount); // Convert.ToInt32(nodeAddress).ToString("X").PadLeft(2, '0')
                   
                        if (functionCode == "03")
                        {
                            if (RecieveData != null && RecieveData.Length > 0)
                            {
                                string funcCode = System.Text.Encoding.UTF8.GetString(
                                                ExtractByteArray(RecieveData, 2, 3));

                                // check if received function code is empty or not
                                if (!string.IsNullOrEmpty(funcCode))
                                {
                                    // check if function code sent and received are same
                                    if (funcCode.Equals(functionCode))
                                    {
                                        //get no. of bytes to read from recieved frame
                                        byte[] sizeBytes = ExtractByteArray(RecieveData, 2, 5);

                                        int size = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(sizeBytes));

                                        if (size > 0)
                                        {
                                            string byteArrayToString =
                                                System.Text.Encoding.UTF8.GetString(ExtractByteArray(RecieveData, size * 2, 7));

                                            return byteArrayToString;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                GotoCount++;
                                if (GotoCount > 25)
                                {
                                    // MessageBox.Show( "Result Not found .Invalid value");
                                    // return "Result Not found .Invalid SV value";
                                }
                                else
                                    goto startagain;
                            }
                        }
                        else if (functionCode == "06")
                        {
                            return "done";
                        }
                    }
                    else if (SetValues.Set_CommunicationProtocol.ToLower() == "rtu"|| SetValues.Set_CommunicationProtocolindex == 2)
                    {
                        //need to optimize
                        RecieveData = modObj.RtuFrame(nodeAddress, functionCode, regAddress, wordCount,SetValues.Set_Baudrate);

                        if (functionCode == "03")
                        {
                            if (RecieveData != null && RecieveData.Length > 0)
                            {
                                string funcCode = BitConverter.ToString(ExtractByteArray(RecieveData, 1, 1)).Replace("-", "");
                                //  ExtractByteArray(RecieveData, 1, 1).ToString("X");

                                // check if received function code is empty or not
                                if (!string.IsNullOrEmpty(funcCode))
                                {
                                    // check if function code sent and received are same
                                    if (funcCode.Equals(functionCode))
                                    {
                                        //get no. of bytes to read from recieved frame
                                        byte[] sizeBytes = ExtractByteArray(RecieveData, 1, 2);

                                        int size = Convert.ToInt32(
                                            BitConverter.ToString(sizeBytes).Replace("-", ""));

                                        string byteArrayToString =
                                   BitConverter.ToString(ExtractByteArray(RecieveData, size, 3)).Replace("-", "");

                                        return byteArrayToString;

                                    }
                                }

                            }
                        }
                        else if (functionCode == "06")
                        {
                            return "done";
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                //MessageBoxEx.Show("CreateFrames RampSoak: " + ex.Message);
            }
            return null;
        }

        private static byte[] ExtractByteArray(byte[] RecieveData, int size, int startPostition)
        {
            byte[] sizeBytes = new byte[size];
            try
            {
                Array.Copy(RecieveData, startPostition, sizeBytes, 0, size);
            }
            catch (Exception ex)
            {
                //MessageBoxEx.Show("ExtractByteArray " + ex.Message);
            }
            return sizeBytes;
        }

        private void ExportDgvToXML()
        {
            try
            {
                try
                {
                    if (dt2.Rows.Count <= 0 || dt2 != null)
                    {
                        dt2.Rows.Clear();
                        dt2 = null;
                        dsFile.Clear();
                        dt2 = new DataTable("NewSettings");
                    }
                    else 
                    {
                        if (!dt2.TableName.Contains("NewSettings"))
                        {
                            dt2 = new DataTable("NewSettings");
                        }
                    }
                }
                catch (Exception ae)
                {
                    dt2 = new DataTable("NewSettings");
                }
               
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    dt2.Columns.Add(column.Name);
                }

                if (dataGridView1.Rows.Count > 0)
                {
                    //don't save last row of datagridview which is the blank editable row
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        DataGridViewRow row = dataGridView1.Rows[i];
                        DataRow newRow = dt2.Rows.Add();

                        for (int j = 0; j < row.Cells.Count; j++)
                        {
                            newRow[j] = row.Cells[j].Value;

                            XElement xe = new XElement(dataGridView1.Columns[j].HeaderText,
                                new XAttribute("Tag", row.Cells[j].Value));
                        }
                    }
                   // count++;
                    dsFile.Tables.Add(dt2);

                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "XML|*.xml";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            dt2.WriteXml(sfd.FileName, XmlWriteMode.WriteSchema);

                            MessageBoxEx.Show("File saved successfully.");


                            dsFile.Tables.Remove(dt2);

                        }
                        catch (Exception ex)
                        {
                            //MessageBoxEx.Show("Something went wrong!! Try Again");
                        }
                    }
                    //else if (sfd.ShowDialog() == DialogResult.Cancel)
                    //{
                    //    try
                    //    {

                    //        dsFile.Tables.Remove(dt2);

                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        //MessageBoxEx.Show("Something went wrong!! Try Again");
                    //    }
                    //}
                    else 
                    {
                        try
                        {

                            dsFile.Tables.Remove(dt2);

                        }
                        catch (Exception ex)
                        {
                            //MessageBoxEx.Show("Something went wrong!! Try Again");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               MessageBoxEx.Show("ExportDgvToXML " + ex.Message);
            }
        }

        private void ExportDgvToXML2()
        {
            try
            {
                XElement xEmp = XElement.Load(m_exePath1);

                var empDetails = from emps in xEmp.Elements("rampSoak")
                                 select emps;

                empDetails.First().Element("start").Value = Convert.ToString(cmbBxStartPattern.SelectedIndex);

                xEmp.Save(m_exePath1);
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("ExportDgvToXML2 " + ex.Message);
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            ExportDgvToXML();
            ExportDgvToXML2();
        }

        public static double ConvertByteArrayToInt32(byte[] b)
        {
            return BitConverter.ToInt32(b, 0);
        }

        private void dataGridView1_CellPainting_1(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected == true)
                    {
                        e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                        using (Pen p = new Pen(Color.Red, 1))
                        {
                            Rectangle rect = e.CellBounds;
                            rect.Width -= 2;
                            rect.Height -= 2;
                            e.Graphics.DrawRectangle(p, rect);
                        }
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("CellPainting " + ex.Message);
            }
        }

        private void cmbBxLinkPattern_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtSV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSV_Click(null, null);
                this.ActiveControl = null;
            }
        }

        private void txtLoopCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLoopCount_Click(null, null);
                this.ActiveControl = null;
            }          
        }

        private void numericUpDownHr_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void numericUpDownHr_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownMin.Enabled = true;

            if (numericUpDownHr.Value == 15)
            {
                numericUpDownMin.Value = 00;
                numericUpDownMin.Enabled = false;
            }
        }

        public void BindGrid()
        {
            try
            {
                Bind(null);

                progressBar1.Invoke((Action)(() => { progressBar1.Value = 100; }));
                //Stop();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RampSoakForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (th != null)
                {
                    if (th.IsAlive)
                    {
                        th.Abort();
                    }
                }
                th = null;
                PauseItemCallback(false);
                // Thread.Sleep(2000);
                clsModbus mo = new clsModbus();

                mo.ACloseSerialPort();
            }
            catch(Exception ae)
            {

            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }
    }
}
