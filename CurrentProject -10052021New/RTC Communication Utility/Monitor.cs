using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RTC_Communication_Utility
{
    enum Colors
    {
        Red,
        Green,
        Blue
    }
    public partial class Monitor : Form
    {
        public Monitor()
        {
            SetValues.Set_Form = "Monitor";
            InitializeComponent();
            Chart = chart_Databx.ChartAreas[0];
        }
        Sync obj = new Sync();
        DataSet datastgbl = new DataSet();
        ChartArea Chart;

        System.Timers.Timer t1 = null;
        System.Timers.Timer t2 = null;
        System.Timers.Timer t3 = null;
        System.Timers.Timer t4 = null;
        System.Timers.Timer t5 = null;
        System.Timers.Timer t6 = null;
        System.Timers.Timer t7 = null;
        System.Timers.Timer t8 = null;

        ModbusRTU modbusobj = new ModbusRTU();
        byte[] GetASKdata = new byte[19];
        Dictionary<int, double> mydictionary1 = new Dictionary<int, double>();
        int i = 0;
        private void Monitor_Load(object sender, EventArgs e)
        {
            TableLayoutPanel dynamicTableLayoutPanel = new TableLayoutPanel();
            dynamicTableLayoutPanel.Location = new System.Drawing.Point(26, 12);
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;
            tableLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tableLayoutPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tableLayoutPanel2.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;
            tableLayoutPanel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tableLayoutPanel3.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;
            tableLayoutPanel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tableLayoutPanel4.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;

            tableLayoutPanel5.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;
            tableLayoutPanel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tableLayoutPanel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tableLayoutPanel6.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;
            tableLayoutPanel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tableLayoutPanel7.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;
            tableLayoutPanel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tableLayoutPanel8.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;


            groupBox1.Paint += delegate(object o, PaintEventArgs p)
            {
                ControlPaint.DrawBorder(p.Graphics, this.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
            };

            groupBoxDiv1.Paint += delegate(object o, PaintEventArgs p)
            {
                ControlPaint.DrawBorder(p.Graphics, this.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
            };
            groupBoxDiv2.Paint += delegate(object o, PaintEventArgs p)
            {
                ControlPaint.DrawBorder(p.Graphics, this.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
            };
            groupBoxDiv3.Paint += delegate(object o, PaintEventArgs p)
            {
                ControlPaint.DrawBorder(p.Graphics, this.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
            };
            groupBoxDiv4.Paint += delegate(object o, PaintEventArgs p)
            {
                ControlPaint.DrawBorder(p.Graphics, this.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
            };
            groupBoxDiv5.Paint += delegate(object o, PaintEventArgs p)
            {
                ControlPaint.DrawBorder(p.Graphics, this.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
            };
            groupBoxDiv6.Paint += delegate(object o, PaintEventArgs p)
            {
                ControlPaint.DrawBorder(p.Graphics, this.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
            };
            groupBoxDiv7.Paint += delegate(object o, PaintEventArgs p)
            {
                ControlPaint.DrawBorder(p.Graphics, this.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
            };
            groupBoxDiv8.Paint += delegate(object o, PaintEventArgs p)
            {
                ControlPaint.DrawBorder(p.Graphics, this.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
            };

            lblPV1.Text = "00.00";
            lblPV2.Text = "00.00";
            lblPV3.Text = "00.00";
            lblPV4.Text = "00.00";
            lblPV5.Text = "00.00";
            lblPV6.Text = "00.00";
            lblPV7.Text = "00.00";
            lblPV8.Text = "00.00";

            lblSV1.Text = "00.00";
            lblSV2.Text = "00.00";
            lblSV3.Text = "00.00";
            lblSV4.Text = "00.00";
            lblSV5.Text = "00.00";
            lblSV6.Text = "00.00";
            lblSV7.Text = "00.00";
            lblSV8.Text = "00.00";

            ReadXMLParameter();

            //------------------------------------------------------------------
            #region KA 30/04/2019
            pnlDetails.Visible = true;
            pnlViewRecords.Visible = false;
            settingsToolStripMenuItem.Checked = true;
            #endregion
            //------------------------------------------------------------------
        }

        public void ReadXMLParameter()
        {
            datastgbl.ReadXml("TCParameter.xml");
            foreach (DataTable item in datastgbl.Tables)
            {
                ReadTable(item);
            }
            CmbBxCtrlAction.SelectedIndex = 0;
            CmbBxSensorType.SelectedIndex = 0;
            CmbBxLockstatus.SelectedIndex = 0;
            CmbBx1stout.SelectedIndex = 0;
            CmbBx2ndout.SelectedIndex = 0;
            CmbBxRunHaltmode.SelectedIndex = 0;
            CmbBxfraction.SelectedIndex = 0;
            CmbBxUnitTyp.SelectedIndex = 0;
            CmbBxAutotunning.SelectedIndex = 0;

        }
        public void WriteXMLParameter()
        {
            datastgbl.WriteXml("TCParameter.xml");

            foreach (DataTable item in datastgbl.Tables)
            {
                WriteTable(item);
            }

        }
        public void WriteTable(DataTable Dt)
        {
            DataRow[] drow = Dt.Select();
            string selected = "";
            foreach (DataRow item in drow)
            {
                switch (Dt.ToString())
                {
                    case "Ctrl_Action":
                        selected = item.ItemArray[0].ToString();
                        CmbBxCtrlAction.Items.Add(selected);
                        break;
                    case "First_output":
                        selected = item.ItemArray[0].ToString();
                        CmbBx1stout.Items.Add(selected);
                        CmbBx2ndout.Items.Add(selected);
                        break;
                    case "Run_Halt":
                        selected = item.ItemArray[0].ToString();
                        CmbBxRunHaltmode.Items.Add(selected);
                        break;
                    case "LockStatus":
                        selected = item.ItemArray[0].ToString();
                        CmbBxLockstatus.Items.Add(selected);
                        break;
                    case "Fraction":
                        selected = item.ItemArray[0].ToString();
                        CmbBxfraction.Items.Add(selected);
                        break;
                    case "Unit":
                        selected = item.ItemArray[0].ToString();
                        CmbBxUnitTyp.Items.Add(selected);
                        break;
                    case "Autotunning":
                        selected = item.ItemArray[0].ToString();
                        CmbBxAutotunning.Items.Add(selected);
                        break;
                    case "PIDParameter":
                        string parmtr = item.ItemArray[0].ToString();
                        selected = item.ItemArray[1].ToString();
                        switch (parmtr)
                        {
                            case "PD":
                                txtBxPD.Text = selected;
                                break;
                            case "Ti":
                                txtBxTi.Text = selected;
                                break;
                            case "Td":
                                txtBxTd.Text = selected;
                                break;
                            case "CtrlPeriod1":
                                txtBxCtrlPer1.Text = selected;
                                break;
                            case "IOffset":
                                txtBxIoffset.Text = selected;
                                break;
                            case "CtrlPeriod2":
                                txtBxctrlPer2.Text = selected;
                                break;
                            case "PCoefficient":
                                txtBxPCoefficient.Text = selected;
                                break;
                            case "Deadband":
                                txtBxdeadband.Text = selected;
                                break;
                            default:
                                break;
                        }
                        txtBxPD.Text = selected;
                        break;
                    case "Input":
                        break;
                    case "FWVersion":
                        break;
                    case "Sensor_type":
                        selected = item.ItemArray[0].ToString();
                        CmbBxSensorType.Items.Add(selected);
                        break;
                    default:
                        break;
                }


            }
        }
        public void ReadTable(DataTable Dt)
        {
            DataRow[] drow = Dt.Select();
            string selected = "";
            foreach (DataRow item in drow)
            {
                switch (Dt.ToString())
                {
                    case "Ctrl_Action":
                        selected = item.ItemArray[0].ToString();
                        CmbBxCtrlAction.Items.Add(selected);
                        break;
                    case "First_output":
                        selected = item.ItemArray[0].ToString();
                        CmbBx1stout.Items.Add(selected);
                        CmbBx2ndout.Items.Add(selected);
                        break;
                    case "Run_Halt":
                        selected = item.ItemArray[0].ToString();
                        CmbBxRunHaltmode.Items.Add(selected);
                        break;
                    case "LockStatus":
                        selected = item.ItemArray[0].ToString();
                        CmbBxLockstatus.Items.Add(selected);
                        break;
                    case "Fraction":
                        selected = item.ItemArray[0].ToString();
                        CmbBxfraction.Items.Add(selected);
                        break;
                    case "Unit":
                        selected = item.ItemArray[0].ToString();
                        CmbBxUnitTyp.Items.Add(selected);
                        break;
                    case "Autotunning":
                        selected = item.ItemArray[0].ToString();
                        CmbBxAutotunning.Items.Add(selected);
                        break;
                    case "PIDParameter":
                        string parmtr = item.ItemArray[0].ToString();
                        selected = item.ItemArray[1].ToString();
                        switch (parmtr)
                        {
                            case "PD":
                                txtBxPD.Text = selected;
                                break;
                            case "Ti":
                                txtBxTi.Text = selected;
                                break;
                            case "Td":
                                txtBxTd.Text = selected;
                                break;
                            case "CtrlPeriod1":
                                txtBxCtrlPer1.Text = selected;
                                break;
                            case "IOffset":
                                txtBxIoffset.Text = selected;
                                break;
                            case "CtrlPeriod2":
                                txtBxctrlPer2.Text = selected;
                                break;
                            case "PCoefficient":
                                txtBxPCoefficient.Text = selected;
                                break;
                            case "Deadband":
                                txtBxdeadband.Text = selected;
                                break;
                            default:
                                break;
                        }
                        txtBxPD.Text = selected;
                        break;
                    case "Input":
                        break;
                    case "FWVersion":
                        break;
                    case "Sensor_type":
                        selected = item.ItemArray[0].ToString();
                        CmbBxSensorType.Items.Add(selected);
                        break;
                    default:
                        break;
                }
            }

        }


        private void pnlDevice1_Paint(object sender, PaintEventArgs e)
        {
            Graphics v = e.Graphics;
            DrawRoundRect(v, Pens.Blue, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1, 10);
            //Without rounded corners
            //e.Graphics.DrawRectangle(Pens.Blue, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
            base.OnPaint(e);
        }

        public void DrawRoundRect(Graphics g, Pen p, float X, float Y, float width, float height, float radius)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(X + radius, Y, X + width - (radius * 2), Y);
            gp.AddArc(X + width - (radius * 2), Y, radius * 2, radius * 2, 270, 90);
            gp.AddLine(X + width, Y + radius, X + width, Y + height - (radius * 2));
            gp.AddArc(X + width - (radius * 2), Y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
            gp.AddLine(X + width - (radius * 2), Y + height, X + radius, Y + height);
            gp.AddArc(X, Y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
            gp.AddLine(X, Y + height - (radius * 2), X, Y + radius);
            gp.AddArc(X, Y, radius * 2, radius * 2, 180, 90);
            gp.CloseFigure();
            g.DrawPath(p, gp);
            gp.Dispose();
        }

        private void pnlDevice2_Paint(object sender, PaintEventArgs e)
        {
            Graphics v = e.Graphics;
            DrawRoundRect(v, Pens.Blue, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1, 10);
            //Without rounded corners
            //e.Graphics.DrawRectangle(Pens.Blue, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
            base.OnPaint(e);
        }

        private void pnlDevice3_Paint(object sender, PaintEventArgs e)
        {
            Graphics v = e.Graphics;
            DrawRoundRect(v, Pens.Blue, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1, 10);
            //Without rounded corners
            //e.Graphics.DrawRectangle(Pens.Blue, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
            base.OnPaint(e);
        }

        private void pnlDevice4_Paint(object sender, PaintEventArgs e)
        {
            Graphics v = e.Graphics;
            DrawRoundRect(v, Pens.Blue, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1, 10);
            base.OnPaint(e);
        }

        public bool ConnectToModbus()
        {
            return modbusobj.OpenCOMPort(SetValues.Set_PortName, Convert.ToInt32(SetValues.Set_Baudrate),
                           SetValues.Set_parity, Convert.ToInt32(SetValues.Set_StopBits), SetValues.Set_BitsLength);
        }

        #region RndbtnClick
        public void roundBtndiv1_Click(object senders, EventArgs es)
        {
            //Sync obj1 = new Sync();

            //if (ConnectToModbus())
            //    obj.ClickEvents(sender, e, numericUpDownDiv1.Value.ToString(), lblPV1, true);

            if (numericUpDownDiv1.Value.ToString() != "0")
            {
                if (roundBtndiv1.Text == "Connect")
                {
                    if (ConnectToModbus())
                    {
                        if (t1 == null)
                        {
                            t1 = new System.Timers.Timer();
                            t1.Elapsed += (sender, e) => tmr_Elapsed(sender, e, lblPV1, numericUpDownDiv1.Value.ToString());
                            t1.Start();
                        }

                        roundBtndiv1.Text = "Cancel";
                        roundBtndiv1.BackColor = Color.Lime;
                    }
                    else
                    {
                        MessageBox.Show("Device not connected.");
                    }
                }
                else if (roundBtndiv1.Text == "Cancel")
                {
                    if (t1 != null)
                    {
                        t1.Stop();
                        t1 = null;
                    }

                    roundBtndiv1.BackColor = Color.White;
                    roundBtndiv1.Text = "Connect";

                    if (lblPV1.InvokeRequired)
                        lblPV1.Invoke((Action)(() => lblPV1.Text = "00.00"));
                    else
                        lblPV1.Text = "00.00";
                    //modbusobj.Port_Close();
                }
            }
            else
            {
                MessageBox.Show("Select node address.");
            }
        }

        private void tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e, Label resLabel, string nodeAddress)
        {
            double temp = GetTemp_1(nodeAddress.PadLeft(2, '0'));

            if (temp != 0)
            {
                if (resLabel.InvokeRequired)
                {
                    resLabel.Invoke((Action)(() => resLabel.Text = temp.ToString()));
                }
                Thread.Sleep(100);
            }
        }

        private void SetDefaultVauesForGraph_PV()
        {
            SetValues.Set_CommandType = "03";
            SetValues.Set_RegAddress = "1000"; //"4700";
            SetValues.Set_WordCount = "0001";//"0002";
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //timer2.Interval = 10;
            //if (modbusobj.IsSerialPortOpen())
            //{
            //    double temp = GetTemp_1(null);

            //    if (temp != 0)
            //    {
            //        lblPV1.Text = temp.ToString();
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Device disconnected.");
            //}
        }

        private double GetTemp_1(string unitAddress)
        {
            try
            {
                SetDefaultVauesForGraph_PV();

                char[] frames = modbusobj.MakeAscFrame(unitAddress, SetValues.Set_CommandType,
                        SetValues.Set_RegAddress, SetValues.Set_WordCount);

                if (frames.Length > 0)
                {
                    GetASKdata = modbusobj.Read_AscDataRegister(1);
                    if (GetASKdata != null)
                    {
                        char[] recdata = System.Text.Encoding.UTF8.GetString(GetASKdata).ToCharArray();
                        string valStr = string.Join("", recdata);

                        string Temperture = "";
                        for (int j = 0; j < 4; j++)
                        {
                            Temperture += recdata[j + 7].ToString();
                        }
                        double int64Val = 0;
                        if (Double.TryParse(Temperture, out int64Val))
                        {
                            int64Val = int64Val * .1;
                        }
                        return int64Val;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 0;
        }

        private void roundBtndiv3_Click(object sender3, EventArgs e3)
        {
            //if (ConnectToModbus())
            //    obj.ClickEvents(sender, e, numericUpDownDiv2.Value.ToString(), lblPV2, true);

            if (numericUpDownDiv3.Value.ToString() != "0")
            {
                if (roundBtndiv3.Text == "Connect")
                {
                    if (ConnectToModbus())
                    {
                        if (t3 == null)
                        {
                            t3 = new System.Timers.Timer();
                            t3.Elapsed += (sender, e) => tmr_Elapsed(sender, e, lblPV3, numericUpDownDiv3.Value.ToString());
                            t3.Start();
                        }

                        roundBtndiv3.Text = "Cancel";
                        roundBtndiv3.BackColor = Color.Lime;
                    }
                    else
                    {
                        MessageBox.Show("Device not connected.");
                    }
                }
                else if (roundBtndiv3.Text == "Cancel")
                {
                    if (t3 != null)
                    {
                        t3.Stop();
                        t3 = null;
                    }

                    roundBtndiv3.BackColor = Color.White;
                    roundBtndiv3.Text = "Connect";

                    if (lblPV3.InvokeRequired)
                        lblPV3.Invoke((Action)(() => lblPV3.Text = "00.00"));

                    // modbusobj.Port_Close();
                }
            }
            else
            {
                MessageBox.Show("Select node address.");
            }
        }

        private void roundBtndiv4_Click(object sender4, EventArgs e4)
        {
            //roundBtndiv3.BackColor = Color.Lime;
            //SetValues.Set_UnitAddress = numericUpDownDiv3.Value.ToString();
            string val = numericUpDownDiv4.Value.ToString();
            if (val != "0")
            {
                if (roundBtndiv4.Text == "Connect")
                {
                    if (ConnectToModbus())
                    {
                        if (t4 == null)
                        {
                            t4 = new System.Timers.Timer();
                            t4.Elapsed += (sender, e) => tmr_Elapsed(sender, e, lblPV4, val);
                            t4.Start();
                        }

                        roundBtndiv4.Text = "Cancel";
                        roundBtndiv4.BackColor = Color.Lime;
                    }
                    else
                    {
                        MessageBox.Show("Device not connected.");
                    }
                }
                else if (roundBtndiv4.Text == "Cancel")
                {
                    if (t4 != null)
                    {
                        t4.Stop();
                        t4 = null;
                    }

                    roundBtndiv4.BackColor = Color.White;
                    roundBtndiv4.Text = "Connect";

                    if (lblPV4.InvokeRequired)
                        lblPV4.Invoke((Action)(() => lblPV4.Text = "00.00"));

                    // modbusobj.Port_Close();
                }
            }
            else
            {
                MessageBox.Show("Select node address.");
            }
        }

        private void roundBtndiv2_Click(object sender2, EventArgs e2)
        {
            //roundBtndiv4.BackColor = Color.Lime;
            //SetValues.Set_UnitAddress = numericUpDownDiv4.Value.ToString();

            if (numericUpDownDiv2.Value.ToString() != "0")
            {
                if (roundBtndiv2.Text == "Connect")
                {
                    if (ConnectToModbus())
                    {
                        if (t2 == null)
                        {
                            t2 = new System.Timers.Timer();
                            t2.Elapsed += (sender, e) => tmr_Elapsed(sender, e, lblPV2, numericUpDownDiv2.Value.ToString());
                            t2.Start();
                        }

                        roundBtndiv2.Text = "Cancel";
                        roundBtndiv2.BackColor = Color.Lime;
                    }
                    else
                    {
                        MessageBox.Show("Device not connected.");
                    }
                }
                else if (roundBtndiv2.Text == "Cancel")
                {
                    if (t2 != null)
                    {
                        t2.Stop();
                        t2 = null;
                    }

                    roundBtndiv2.BackColor = Color.White;
                    roundBtndiv2.Text = "Connect";

                    if (lblPV2.InvokeRequired)
                        lblPV2.Invoke((Action)(() => lblPV2.Text = "00.00"));

                    //modbusobj.Port_Close();
                }
            }
            else
            {
                MessageBox.Show("Select node address.");
            }
        }

        private void roundBtndiv5_Click(object sender5, EventArgs e5)
        {
            //roundBtndiv5.BackColor = Color.Lime;
            //SetValues.Set_UnitAddress = numericUpDownDiv5.Value.ToString();

            if (numericUpDownDiv5.Value.ToString() != "0")
            {
                if (roundBtndiv5.Text == "Connect")
                {
                    if (ConnectToModbus())
                    {
                        if (t5 == null)
                        {
                            t5 = new System.Timers.Timer();
                            t5.Elapsed += (sender, e) => tmr_Elapsed(sender, e, lblPV5, numericUpDownDiv5.Value.ToString());
                            t5.Start();
                        }

                        roundBtndiv5.Text = "Cancel";
                        roundBtndiv5.BackColor = Color.Lime;
                    }
                    else
                    {
                        MessageBox.Show("Device not connected.");
                    }
                }
                else if (roundBtndiv5.Text == "Cancel")
                {
                    if (t5 != null)
                    {
                        t5.Stop();
                        t5 = null;
                    }

                    roundBtndiv5.BackColor = Color.White;
                    roundBtndiv5.Text = "Connect";

                    if (lblPV5.InvokeRequired)
                        lblPV5.Invoke((Action)(() => lblPV5.Text = "00.00"));

                    // modbusobj.Port_Close();
                }
            }
            else
            {
                MessageBox.Show("Select node address.");
            }
        }

        private void roundBtndiv6_Click(object sender6, EventArgs e6)
        {
            // roundBtndiv6.BackColor = Color.Lime;
            // SetValues.Set_UnitAddress = numericUpDownDiv6.Value.ToString();

            if (numericUpDownDiv6.Value.ToString() != "0")
            {
                if (roundBtndiv6.Text == "Connect")
                {
                    if (ConnectToModbus())
                    {
                        if (t6 == null)
                        {
                            t6 = new System.Timers.Timer();
                            t6.Elapsed += (sender, e) => tmr_Elapsed(sender, e, lblPV6, numericUpDownDiv6.Value.ToString());
                            t6.Start();
                        }

                        roundBtndiv6.Text = "Cancel";
                        roundBtndiv6.BackColor = Color.Lime;
                    }
                    else
                    {
                        MessageBox.Show("Device not connected.");
                    }
                }
                else if (roundBtndiv6.Text == "Cancel")
                {
                    if (t6 != null)
                    {
                        t6.Stop();
                        t6 = null;
                    }

                    roundBtndiv6.BackColor = Color.White;
                    roundBtndiv6.Text = "Connect";

                    if (lblPV6.InvokeRequired)
                        lblPV6.Invoke((Action)(() => lblPV6.Text = "00.00"));

                    // modbusobj.Port_Close();
                }
            }
            else
            {
                MessageBox.Show("Select node address.");
            }
        }

        private void roundBtndiv7_Click(object sender7, EventArgs e7)
        {
            //roundBtndiv7.BackColor = Color.Lime;
            //SetValues.Set_UnitAddress = numericUpDownDiv7.Value.ToString();

            if (numericUpDownDiv7.Value.ToString() != "0")
            {
                if (roundBtndiv7.Text == "Connect")
                {
                    if (ConnectToModbus())
                    {
                        if (t7 == null)
                        {
                            t7 = new System.Timers.Timer();
                            t7.Elapsed += (sender, e) => tmr_Elapsed(sender, e, lblPV7, numericUpDownDiv7.Value.ToString());
                            t7.Start();
                        }

                        roundBtndiv7.Text = "Cancel";
                        roundBtndiv7.BackColor = Color.Lime;
                    }
                    else
                    {
                        MessageBox.Show("Device not connected.");
                    }
                }
                else if (roundBtndiv7.Text == "Cancel")
                {
                    if (t7 != null)
                    {
                        t7.Stop();
                        t7 = null;
                    }

                    roundBtndiv7.BackColor = Color.White;
                    roundBtndiv7.Text = "Connect";

                    if (lblPV7.InvokeRequired)
                        lblPV7.Invoke((Action)(() => lblPV7.Text = "00.00"));

                    // modbusobj.Port_Close();
                }
            }
            else
            {
                MessageBox.Show("Select node address.");
            }
        }

        private void roundBtndiv8_Click(object senders, EventArgs es)
        {
            // roundBtndiv8.BackColor = Color.Lime;
            //SetValues.Set_UnitAddress = numericUpDownDiv8.Value.ToString();

            if (numericUpDownDiv8.Value.ToString() != "0")
            {
                if (roundBtndiv8.Text == "Connect")
                {
                    if (ConnectToModbus())
                    {
                        if (t8 == null)
                        {
                            t8 = new System.Timers.Timer();
                            t8.Elapsed += (sender, e) => tmr_Elapsed(sender, e, lblPV8, numericUpDownDiv8.Value.ToString());
                            t8.Start();
                        }

                        roundBtndiv8.Text = "Cancel";
                        roundBtndiv8.BackColor = Color.Lime;
                    }
                    else
                    {
                        MessageBox.Show("Device not connected.");
                    }
                }
                else if (roundBtndiv8.Text == "Cancel")
                {
                    if (t8 != null)
                    {
                        t8.Stop();
                        t8 = null;
                    }

                    roundBtndiv8.BackColor = Color.White;
                    roundBtndiv8.Text = "Connect";

                    if (lblPV8.InvokeRequired)
                        lblPV8.Invoke((Action)(() => lblPV8.Text = "00.00"));

                    // modbusobj.Port_Close();
                }
            }
            else
            {
                MessageBox.Show("Select node address.");
            }

        }
        #endregion

        #region picBx_Paint
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "PV");
        }

        private void pictureBoxSV1_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "SV");
        }

        private void pictureBoxPV3_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "PV");
        }

        private void pictureBoxSV3_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "SV");
        }

        private void pictureBoxPV4_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "PV");
        }

        private void pictureBoxSV4_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "SV");
        }

        private void pictureBoxPV2_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "PV");
        }

        private void pictureBoxSV2_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "SV");
        }

        private void pictureBoxPV5_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "PV");
        }

        private void pictureBoxPV6_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "PV");
        }

        private void pictureBoxPV7_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "PV");
        }

        private void pictureBoxPV8_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "PV");
        }

        private void pictureBoxSV5_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "SV");
        }

        private void pictureBoxSV6_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "SV");
        }

        private void pictureBoxSV7_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "SV");
        }

        private void pictureBoxSV8_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox(sender, e, "SV");
        }
        #endregion

        private void PaintPictureBox(object sender, PaintEventArgs e, string value)
        {
            using (Font myFont = new Font("Georgia", 10))
            {
                e.Graphics.DrawString(value, myFont, Brushes.Green, new Point(0, 2));
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CmbBxCtrlAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbBxCtrlAction.SelectedIndex == 0)
            {
                grpbx_PID_Parameter.Visible = true;
                grpBxManual.Visible = false;
                grpBxOnOff.Visible = false;
            }
            else if (CmbBxCtrlAction.SelectedIndex == 1)
            {
                grpbx_PID_Parameter.Visible = false;
                grpBxOnOff.Visible = true;
                grpBxManual.Visible = true;
            }
            else if (CmbBxCtrlAction.SelectedIndex == 2)
            {
                grpbx_PID_Parameter.Visible = false;
                grpBxManual.Visible = true;
                grpBxOnOff.Visible = false;
            }
            else if (CmbBxCtrlAction.SelectedIndex == 3)
            {
                grpbx_PID_Parameter.Visible = true;
                grpBxManual.Visible = false;
                grpBxOnOff.Visible = false;
            }
        }

        private void CmbBxRunHaltmode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbBxRunHaltmode.SelectedIndex == 0)
            {

            }
            else if (CmbBxRunHaltmode.SelectedIndex == 1)
            {

            }
            else if (CmbBxRunHaltmode.SelectedIndex == 2)
            {

            }
            else if (CmbBxRunHaltmode.SelectedIndex == 3)
            {

            }
        }

        private void CmbBx1stout_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbBx1stout.SelectedIndex == 0)
            {

            }
            else if (CmbBx1stout.SelectedIndex == 1)
            {

            }
            else if (CmbBx1stout.SelectedIndex == 2)
            {

            }
            else if (CmbBx1stout.SelectedIndex == 3)
            {

            }
        }

        private void CmbBx2ndout_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbBx2ndout.SelectedIndex == 0)
            {

            }
            else if (CmbBx2ndout.SelectedIndex == 1)
            {

            }
            else if (CmbBx2ndout.SelectedIndex == 2)
            {

            }
            else if (CmbBx2ndout.SelectedIndex == 3)
            {

            }
        }

        private void CmbBxLockstatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbBxLockstatus.SelectedIndex == 0)
            {

            }
            else if (CmbBxLockstatus.SelectedIndex == 1)
            {

            }
            else if (CmbBxLockstatus.SelectedIndex == 2)
            {

            }
            else if (CmbBxLockstatus.SelectedIndex == 3)
            {

            }
        }

        private void CmbBxAutotunning_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbBxAutotunning.SelectedIndex == 0)
            {

            }
        }

        private void CmbBxSensorType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbBxSensorType.SelectedIndex == 0)
            {

            }
            else if (CmbBxSensorType.SelectedIndex == 1)
            {

            }
            else if (CmbBxSensorType.SelectedIndex == 2)
            {

            }
            else if (CmbBxSensorType.SelectedIndex == 3)
            {

            }
            else if (CmbBxSensorType.SelectedIndex == 4)
            {

            }
            else if (CmbBxSensorType.SelectedIndex == 5)
            {

            }
            else if (CmbBxSensorType.SelectedIndex == 6)
            {

            }
            else if (CmbBxSensorType.SelectedIndex == 7)
            {

            }
            else if (CmbBxSensorType.SelectedIndex == 8)
            {

            }
            else if (CmbBxSensorType.SelectedIndex == 9)
            {

            }
        }

        private void CmbBxfraction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbBxfraction.SelectedIndex == 0)
            {

            }
            else if (CmbBxfraction.SelectedIndex == 1)
            {

            }
        }

        private void CmbBxUnitTyp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbBxUnitTyp.SelectedIndex == 0)
            {

            }
            else if (CmbBxUnitTyp.SelectedIndex == 1)
            {

            }
            else if (CmbBxUnitTyp.SelectedIndex == 2)
            {

            }
        }

        private void txtBxSetvalue_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBxHightemp_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBxLowtemp_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDownDiv1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownDiv1.Value.ToString().Length == 1)
                SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv1.Value);
            else
                SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv1.Value);
        }

        private void numericUpDownDiv2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownDiv2.Value.ToString().Length == 1)
                SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv2.Value);
            else
                SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv2.Value);
        }

        private void numericUpDownDiv3_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDownDiv4_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDownDiv5_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDownDiv6_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDownDiv7_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDownDiv8_ValueChanged(object sender, EventArgs e)
        {

        }

        #region (KA) 30/04/2019
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ViewToolStrip(sender, e))
            {
                pnlViewRecords.Visible = false;
                pnlDetails.Visible = true;

                if (timer1 != null)
                {
                    timer1.Stop();
                }
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void pVRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ViewToolStrip(sender, e))
            {
                GetPvRecord("Device 1");
                pnlViewRecords.Visible = true;
                pnlDetails.Visible = false;
            }
        }

        private void GetPvRecord(string deviceName)
        {
            if (ConnectToModbus())
            {
                if (numericUpDownDiv1.Value.ToString().Length == 1)
                    SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv1.Value);
                else
                    SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv1.Value);

                modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

                GetASKdata = modbusobj.Read_AscDataRegister(1);
                if (GetASKdata != null && GetASKdata[0] != 0)
                    goto Outer; ;

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

                    chart_Databx.Series.Clear();
                    Chart.AxisX.IntervalType = DateTimeIntervalType.Number;

                    Chart.AxisX.LabelStyle.Format = "";
                    Chart.AxisY.LabelStyle.Format = "";
                    Chart.AxisY.LabelStyle.IsEndLabelVisible = true;

                    Chart.AxisX.Minimum = 1;
                    Chart.AxisX.Maximum = 1000;

                    Chart.AxisX.Interval = 0.1;

                    chart_Databx.Series.Add("Device1");
                    chart_Databx.Series["Device1"].ChartType = SeriesChartType.Spline;
                    chart_Databx.Series["Device1"].BorderWidth = 2;
                    chart_Databx.Series["Device1"].Color = Color.Red;

                    if (timer1 == null)
                        this.timer1 = new System.Windows.Forms.Timer(this.components);

                    timer1.Interval = (10);
                    timer1.Enabled = true;

                    timer1.Tick += new EventHandler(timer1_Tick);
                    timer1.Start();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (numericUpDownDiv1.Value.ToString().Length == 1)
                SetValues.Set_UnitAddress = "0" + Convert.ToString(numericUpDownDiv1.Value);
            else
                SetValues.Set_UnitAddress = Convert.ToString(numericUpDownDiv1.Value);

            modbusobj.MakeAscFrame(SetValues.Set_UnitAddress, SetValues.Set_CommandType,
                    SetValues.Set_RegAddress, SetValues.Set_WordCount);

            GetASKdata = modbusobj.Read_AscDataRegister(1);
            i++;

            if (GetASKdata[0] != 0)
            {
                DrawGraph("numericUpDownDiv1");
            }
        }

        private void DrawGraph(string NumText)
        {
            try
            {
                char[] recdata = System.Text.Encoding.UTF8.GetString(GetASKdata).ToCharArray();

                string Temperture = "";
                for (int j = 0; j < 4; j++)
                {
                    Temperture += recdata[j + 7].ToString();
                }
                double int64Val = Convert.ToInt64(Temperture, 16);
                int64Val = int64Val * .1;

                mydictionary1.Add(i, int64Val);

                chart_Databx.Series["Device1"].Points.AddXY(i, float.Parse(int64Val.ToString()));

                Chart.AxisX.Interval = 10;
                Chart.AxisX.Maximum = 1000;
            }
            catch (Exception ex)
            {

            }
        }

        private void Create1(object obj)
        {

        }

        private bool ViewToolStrip(object sender, EventArgs e)
        {
            var currentItem = sender as ToolStripMenuItem;
            if (currentItem != null)
            {
                ((ToolStripMenuItem)currentItem.OwnerItem).DropDownItems
                    .OfType<ToolStripMenuItem>().ToList()
                    .ForEach(item =>
                    {
                        item.Checked = false;
                    });

                //Check the current items
                currentItem.Checked = true;
            }
            return currentItem.Checked;
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }

    public class Sync
    {
        System.Timers.Timer t1 = null;

        public Sync()
        {
        }

        public void ClickEvents(object senders, EventArgs es, string nodeAddress, Label resLabel, bool ConnectToModbus)
        {
            Button btn = (senders) as Button;

            if (nodeAddress != "0")
            {
                if (btn.Text == "Connect")
                {
                    if (ConnectToModbus)
                    {
                        if (t1 == null)
                        {
                            t1 = new System.Timers.Timer();
                            //t1.Elapsed += tmr_t1_Elapsed;    // Uses an event instead of a delegate
                            t1.Elapsed += (sender, e) => tmr_t1_Elapsed(sender, e, resLabel);
                            t1.Start();
                        }

                        //timer2.Enabled = true;
                        btn.Text = "Cancel";
                        btn.BackColor = Color.Lime;
                    }
                    else
                    {
                        MessageBox.Show("Device not connected.");
                    }
                }
                else if (btn.Text == "Cancel")
                {
                    if (t1 != null)
                    {
                        //t1.Change(-1, -1); // Stop the timer from running.
                        t1.Stop();
                    }
                    //timer2.Enabled = false;
                    btn.BackColor = Color.White;
                    btn.Text = "Connect";
                    resLabel.Text = "00.00";

                    //modbusobj.Port_Close();
                }
            }
            else
            {
                MessageBox.Show("Select node address.");
            }
        }

        private void tmr_t1_Elapsed(object sender, System.Timers.ElapsedEventArgs e, Label resLabel)
        {
            if (resLabel.InvokeRequired)
            {
                resLabel.Invoke((Action)(() => resLabel.Text = Environment.TickCount.ToString()));
            }
            Thread.Sleep(1000);
        }
    }
}
