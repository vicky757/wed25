using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RTC_Communication_Utility.UserControls
{
    public partial class UserControl1 : UserControl, INotifyPropertyChanged
    {
        public delegate bool AddItemDelegate(string item);
        public AddItemDelegate AddItemCallback;

        public delegate bool RemoveItemDelegate(string item);
        public RemoveItemDelegate RemoveItemCallback;

        public delegate bool SelectedItemDelegate(string item);
        public SelectedItemDelegate SelectedItemCallback;

       
        public UserControl1()
        {
            InitializeComponent();

            PvRecords = new Dictionary<double, double>();//{ { 1, 10 }, { 2, 11 }, { 3, 15 }, { 4, 25 }, { 5, 10 }, { 6, 39 }, { 7, 11 } };
        }

        GraphicsPath gp = null;
        Region rg = null;

        private int nodeAddress;
        public int NodeAddress
        {
            get { return nodeAddress; }
            set
            {
                nodeAddress = value;
                this.OnPropertyChanged("NodeAddress");
            }
        }

        private int ledStatus;
        public int LedStatus
        {
            get { return ledStatus; }
            set
            {
                ledStatus = value;
                this.OnPropertyChanged("LedStatus");
            }
        }

        public string NodeId { get; set; }

        private string _pv;
        public string PV
        {
            get { return _pv; }
            set
            {
                _pv = string.IsNullOrEmpty(value) ? "0.0" : value;
                if (txtPV.InvokeRequired)
                {
                    txtPV.BeginInvoke((Action)(() => txtPV.Text = _pv));
                }
                else
                {
                    txtPV.Text = _pv;
                }
                this.OnPropertyChanged("PV");
            }
        }

        private bool _svBool;
        public bool SvBool
        {
            get { return _svBool; }
            set
            {
                _svBool = value;

                txtSV.Visible = _svBool;

                this.OnPropertyChanged("SvBool");
            }
        }

        private string _sv;
        public string SV
        {
            get { return _sv; }
            set
            {
                _sv = string.IsNullOrEmpty(value) ? "0.0" : value;
                if (txtSV.InvokeRequired)
                {
                    txtSV.BeginInvoke((Action)(() => txtSV.Text = _sv));
                }
                else
                {
                    txtSV.Text = _sv;
                }
                this.OnPropertyChanged("SV");
            }
        }

        private bool _selectedNode;
        public bool SelectedNode
        {
            get
            {
                return _selectedNode;
            }
            set
            {
                _selectedNode = value;

                if (_selectedNode)
                {
                    if (panel1.InvokeRequired)
                    {
                        panel1.BeginInvoke((Action)(() => panel1.BackColor = Color.YellowGreen));
                    }
                    else
                    {
                        panel1.BackColor = Color.YellowGreen;
                    }
                }
                else
                {
                    if (panel1.InvokeRequired)
                    {
                        panel1.BeginInvoke((Action)(() => panel1.BackColor = Color.LightGreen));
                    }
                    else
                    {
                        panel1.BackColor = Color.LightGreen;
                    }
                }

                this.OnPropertyChanged("SelectedNode");
            }
        }

        private bool _connecting;
        public bool Connecting
        {
            get { return _connecting; }
            set
            {
                _connecting = value;
                if (_connecting)
                {
                    if (pictureBox3.InvokeRequired)
                    {
                        pictureBox3.BeginInvoke((Action)(() => PictureBoxChange(pictureBox3, Color.Yellow)));
                    }
                }
                else
                {
                    if (pictureBox3.InvokeRequired)
                    {
                        pictureBox3.BeginInvoke((Action)(() => PictureBoxChange(pictureBox3, Color.Gray)));
                    }
                    //PictureBoxChange(pictureBox3, Color.Gray);
                }
                this.OnPropertyChanged("Connecting");
            }
        }

        private bool _connected;
        public bool Connected
        {
            get
            {
                return _connected;
            }
            set
            {
                _connected = value;
                if (_connected)
                {
                    if (pictureBox3.InvokeRequired)
                    {
                        pictureBox3.BeginInvoke((Action)(() => PictureBoxChange(pictureBox3, Color.Green)));
                        panel1.BeginInvoke((Action)(() => panel1.BackColor = Color.LightGreen));
                    }
                    else
                    {
                        PictureBoxChange(pictureBox3, Color.Green);
                        panel1.BackColor = Color.LightGreen;
                    }
                }
                else
                {
                    if (pictureBox3.InvokeRequired)
                    {
                        pictureBox3.BeginInvoke((Action)(() => PictureBoxChange(pictureBox3, Color.Gray)));
                        panel1.BeginInvoke((Action)(() => panel1.BackColor = Color.DarkGray));
                    }
                    else
                    {
                        PictureBoxChange(pictureBox3, Color.Gray);
                        panel1.BackColor = Color.DarkGray;
                    }
                }

                this.OnPropertyChanged("Connected");
            }
        }

        public string ButtonText
        {
            get
            {
                return btnConnect.Text;
            }
            set
            {
                //if (btnConnect.Text != value)
                //{
                if (btnConnect.InvokeRequired)
                {
                    btnConnect.BeginInvoke((Action)(() => btnConnect.Text = value));
                }
                else
                {
                    btnConnect.Text = value;
                }
                this.OnPropertyChanged("ButtonText");
                //}
            }
        }

        public Color BackgroundsColor
        {
            get
            {
                return panel1.BackColor;
            }
            set
            {
                if (panel1.InvokeRequired)
                {
                    panel1.BeginInvoke((Action)(() => panel1.BackColor = value));
                }
                else
                {
                    panel1.BackColor = value;
                }
                this.OnPropertyChanged("BackgroundsColor");
            }
        }

        private string _unit;
        public string Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                //if (lblUnit1.Text != value)

                if (lblUnit.InvokeRequired)
                {
                    lblUnit.BeginInvoke((Action)(() => lblUnit.Text = _unit));
                }
                else
                {
                    lblUnit.Text = _unit;
                }
                this.OnPropertyChanged("Unit");

            }
        }

        private decimal _out1;
        public decimal Out1Percent
        {
            get { return _out1; }
            set
            {
               // if (_out1 != value)
               // {
                
                    _out1 = value;
                   
                    if (progressBar1.InvokeRequired)
                    {
                        progressBar1.BeginInvoke((Action)(() => progressBar1.Value = _out1 < 0 ? 0 : Convert.ToInt32(_out1)));
                        if (progressBar1.Value > 0)
                        {
                            if (Out11 != 2)
                            {
                                lblout1per.Text = (progressBar1.Value).ToString() + "%";
                            }
                            else 
                            {
                                lblout1per.Text = "";
                            }
                        }
                        else 
                        {
                            lblout1per.Text = "";
                        }
                    }
                    else
                    {
                        progressBar1.Value = _out1 < 0 ? 0 : Convert.ToInt32(_out1);
                        if (progressBar1.Value > 0)
                        {
                            if (Out11 != 2)
                            {
                                lblout1per.Text = (progressBar1.Value).ToString() + "%";
                            }
                            else
                            {
                                lblout1per.Text = "";
                            }
                        }
                        else 
                        {
                            lblout1per.Text = "";
                        }
                    }
                    
                  

                this.OnPropertyChanged("Out1Percent");
               // }
            }
        }

        private decimal _out2;
        public decimal Out2Percent
        {
            get { return _out2; }
            set
            {
                //  if (_out2 != value)
               // {
                    _out2 = value;
                    if (progressBar2.InvokeRequired)
                    {
                        progressBar2.BeginInvoke((Action)(() => progressBar2.Value = _out2 < 0 ? 0 : Convert.ToInt32(_out2)));
                        if (progressBar2.Value > 0)
                        {
                            if (Out12 != 2)
                            {
                                lblout2per.Text = (progressBar2.Value).ToString() + "%";
                            }
                            else
                            {
                                lblout2per.Text = "";
                            }
                           
                        }
                        else
                        {
                            lblout2per.Text = "";
                        }
                    }
                    else
                    {
                        progressBar2.Value = _out2 < 0 ? 0 : Convert.ToInt32(_out2);
                        if (progressBar2.Value > 0)
                        {
                            if (Out12 != 2)
                            {
                                lblout2per.Text = (progressBar2.Value).ToString() + "%";
                            }
                            else
                            {
                                lblout2per.Text = "";
                            }
                           
                        }
                        else
                        {
                            lblout2per.Text = "";
                        }
                    }
                    this.OnPropertyChanged("Out2Percent");
              // }
            }
        }

        private int _out11;
        public int Out11
        {
            get { return _out11; }
            set
            {
                //if (_out1 != value)
                //{
                _out11 = value;
                BindOut1(_out11);
                this.OnPropertyChanged("Out11");
                //}
            }
        }



        private int _out12;
        public int Out12
        {
            get { return _out12; }
            set
            {
                //if (_out1 != value)
                //{
                _out12 = value;
                BindOut2(_out12);

                this.OnPropertyChanged("Out12");
                //}
            }
        }

        private bool _Alarm1Data;
        public bool Alarm1data
        {
            get { return _Alarm1Data; }
            set
            {
                _Alarm1Data = value;
                if (_Alarm1Data == true)
                {
                    PictureBoxChange(pictureBox1, Color.Red);
                }
                else
                {
                    PictureBoxChange(pictureBox1, Color.Gray);
                }

            }
        }

        private bool _Alarm2Data;
        public bool Alarm2data
        {
            get { return _Alarm2Data; }
            set
            {
                _Alarm2Data = value;
                if (_Alarm2Data == true)
                {
                    PictureBoxChange(pictureBox2, Color.Red);
                }
                else
                {
                    PictureBoxChange(pictureBox2, Color.Gray);
                }

            }
        }
        public int panelcol
        {
            get { return panelcol; }
            set
            {
                panelcolr();
            }

        }
        public int labels
        {
            get { return labels; }
            set
            {
                lablecolr();
            }

        }

        private void lablecolr()
        {
            try
            {
                {
                    panel5.BackColor = Color.LightGray;
                    label2.BackColor = Color.LightGray;
                }

                {
                    panel6.BackColor = Color.LightGray;
                    label3.BackColor = Color.LightGray;

                }
            }
            catch (Exception ae)
            { }
        }

        private void panelcolr()
        {
            try
            {
                {
                    panel1.BackColor = Color.LightGray;
                  
                }

                {
                    try
                    {
                        UserControl1 us = new UserControl1();
                        us.NodeId = "1";
                        // panel1.BackColor = Color.LightGray;
                        us.panel1.BackColor = Color.LightGray;
                        us.NodeId = "2";
                        us.panel1.BackColor = Color.LightGray;
                        us.NodeId = "3";
                        us.panel1.BackColor = Color.LightGray;
                        us.NodeId = "4";
                        us.panel1.BackColor = Color.LightGray;
                    }
                    catch (Exception ae) { }

                }
            }
            catch (Exception ae)
            { }
        }

        private void BindOut1(int _out11)
        {
            try
            {
                if (progressBar1.InvokeRequired)
                {
                    progressBar1.BeginInvoke((Action)(() => progressBar1.Visible = true));
                }
                else
                {
                    progressBar1.Visible = true;
                }

                if (panel5.InvokeRequired)
                {
                    panel5.BeginInvoke((Action)(() => panel5.Visible = true));
                }
                else
                {
                    panel5.Visible = true;
                }

                switch (_out11)
                {
                    case 0://h
                        //progressBar1
                        if (label2.InvokeRequired)
                        {
                            label2.BeginInvoke((Action)(() => label2.BackColor = Color.LightCoral));
                        }
                        else
                        {
                            label2.BackColor = Color.LightCoral;
                        }

                        if (panel5.InvokeRequired)
                        {
                            panel5.BeginInvoke((Action)(() => panel5.BackColor = Color.LightCoral));
                        }
                        else
                        {
                            panel5.BackColor = Color.LightCoral;
                        }

                        if (lblAlarm1.InvokeRequired)
                        {
                            lblAlarm1.BeginInvoke((Action)(() => lblAlarm1.Visible = false));
                        }
                        else
                        {
                            lblAlarm1.Visible = false;
                        }

                        if (pictureBox1.InvokeRequired)
                        {
                            pictureBox1.BeginInvoke((Action)(() => pictureBox1.Visible = false));
                        }
                        else
                        {
                            pictureBox1.Visible = false;
                        }
                        break;
                    case 1://c
                        //progressBar1
                        if (label2.InvokeRequired)
                        {
                            label2.BeginInvoke((Action)(() => label2.BackColor = Color.LightSteelBlue));
                        }
                        else
                        {
                            label2.BackColor = Color.LightSteelBlue;
                        }

                        if (panel5.InvokeRequired)
                        {
                            panel5.BeginInvoke((Action)(() => panel5.BackColor = Color.LightSteelBlue));
                        }
                        else
                        {
                            panel5.BackColor = Color.LightSteelBlue;
                        }

                        if (lblAlarm1.InvokeRequired)
                        {
                            lblAlarm1.BeginInvoke((Action)(() => lblAlarm1.Visible = false));
                        }
                        else
                        {
                            lblAlarm1.Visible = false;
                        }

                        if (pictureBox1.InvokeRequired)
                        {
                            pictureBox1.BeginInvoke((Action)(() => pictureBox1.Visible = false));
                        }
                        else
                        {
                            pictureBox1.Visible = false;
                        }
                        break;
                    case 2://a
                        //panel5.Visible = false;

                        if (label2.InvokeRequired)
                        {
                            label2.BeginInvoke((Action)(() => label2.BackColor = Color.LightGray));
                        }
                        else
                        {
                            label2.BackColor = Color.LightGray;
                        }

                        if (panel5.InvokeRequired)
                        {
                            panel5.BeginInvoke((Action)(() => panel5.BackColor = Color.LightGray));
                        }
                        else
                        {
                            panel5.BackColor = Color.LightGray;
                        }

                        if (pictureBox1.InvokeRequired)
                        {
                            pictureBox1.BeginInvoke((Action)(() => pictureBox1.BackColor = Color.LightGray));
                        }
                        else
                        {
                            pictureBox1.BackColor = Color.LightGray;
                        }

                        if (progressBar1.InvokeRequired)
                        {
                            progressBar1.BeginInvoke((Action)(() => progressBar1.Visible = false));
                            lblout1per.Text = "";
                        }
                        else
                        {
                            progressBar1.Visible = false;
                            lblout1per.Text = "";
                        }

                        if (panel5.InvokeRequired)
                        {
                            panel5.BeginInvoke((Action)(() => panel5.Visible = false));
                        }
                        else
                        {
                            panel5.Visible = false;
                        }

                        if (lblAlarm1.InvokeRequired)
                        {
                            lblAlarm1.BeginInvoke((Action)(() => lblAlarm1.Visible = true));
                        }
                        else
                        {
                            lblAlarm1.Visible = true;
                        }

                        if (pictureBox1.InvokeRequired)
                        {
                            pictureBox1.BeginInvoke((Action)(() => pictureBox1.Visible = true));
                        }
                        else
                        {
                            pictureBox1.Visible = true;
                        }
                        break;
                    default:
                        if (label2.InvokeRequired)
                        {
                            label2.BeginInvoke((Action)(() => label2.BackColor = Color.LightGray));
                        }
                        else
                        {
                            label2.BackColor = Color.LightGray;
                        }

                        if (panel5.InvokeRequired)
                        {
                            panel5.BeginInvoke((Action)(() => panel5.BackColor = Color.LightGray));
                        }
                        else
                        {
                            panel5.BackColor = Color.LightGray;
                        }

                        if (pictureBox1.InvokeRequired)
                        {
                            pictureBox1.BeginInvoke((Action)(() => pictureBox1.BackColor = Color.LightGray));
                        }
                        else
                        {
                            pictureBox1.BackColor = Color.LightGray;
                        }
                        ////ritesh_Change
                        //if (progressBar1.InvokeRequired)
                        //{
                        //    progressBar1.BeginInvoke((Action)(() => progressBar1.Visible = false));
                        //}
                        //else
                        //{
                        //    progressBar1.Visible = false;
                        //}
                       
                        //if (progressBar2.InvokeRequired)
                        //{
                        //    progressBar2.BeginInvoke((Action)(() => progressBar2.Visible = false));
                        //}
                        //else
                        //{
                        //   progressBar2.Visible = false;
                        //}


                        if (lblAlarm1.InvokeRequired)
                        {
                            lblAlarm1.BeginInvoke((Action)(() => lblAlarm1.Visible = false));
                        }
                        else
                        {
                            lblAlarm1.Visible = false;
                        }

                        if (pictureBox1.InvokeRequired)
                        {
                            pictureBox1.BeginInvoke((Action)(() => pictureBox1.Visible = false));
                        }
                        else
                        {
                            pictureBox1.Visible = false;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void BindOut2(int _out12)
        {
            try
            {
                if (progressBar2.InvokeRequired)
                {
                    progressBar2.BeginInvoke((Action)(() => progressBar2.Visible = true));
                }
                else
                {
                    progressBar2.Visible = true;
                }

                if (panel6.InvokeRequired)
                {
                    panel6.BeginInvoke((Action)(() => panel6.Visible = true));
                }
                else
                {
                    panel6.Visible = true;
                }

                switch (_out12)
                {
                    case 0://h
                        //progressBar2
                        if (label3.InvokeRequired)
                        {
                            label3.BeginInvoke((Action)(() => label3.BackColor = Color.LightCoral));
                        }
                        else
                        {
                            label3.BackColor = Color.LightCoral;
                        }

                        if (panel6.InvokeRequired)
                        {
                            panel6.BeginInvoke((Action)(() => panel6.BackColor = Color.LightCoral));
                        }
                        else
                        {
                            panel6.BackColor = Color.LightCoral;
                        }

                        if (lblAlarm2.InvokeRequired)
                        {
                            lblAlarm2.BeginInvoke((Action)(() => lblAlarm2.Visible = false));
                        }
                        else
                        {
                            lblAlarm2.Visible = false;
                        }

                        if (pictureBox2.InvokeRequired)
                        {
                            pictureBox2.BeginInvoke((Action)(() => pictureBox2.Visible = false));
                        }
                        else
                        {
                            pictureBox2.Visible = false;
                        }
                        break;
                    case 1://c
                        //progressBar2
                        if (label3.InvokeRequired)
                        {
                            label3.BeginInvoke((Action)(() => label3.BackColor = Color.LightSteelBlue));
                        }
                        else
                        {
                            label3.BackColor = Color.LightSteelBlue;
                        }

                        if (panel6.InvokeRequired)
                        {
                            panel6.BeginInvoke((Action)(() => panel6.BackColor = Color.LightSteelBlue));
                        }
                        else
                        {
                            panel6.BackColor = Color.LightSteelBlue;
                        }

                        if (lblAlarm2.InvokeRequired)
                        {
                            lblAlarm2.BeginInvoke((Action)(() => lblAlarm2.Visible = false));
                        }
                        else
                        {
                            lblAlarm2.Visible = false;
                        }

                        if (pictureBox2.InvokeRequired)
                        {
                            pictureBox2.BeginInvoke((Action)(() => pictureBox2.Visible = false));
                        }
                        else
                        {
                            pictureBox2.Visible = false;
                        }
                        break;
                    case 2://a
                        if (progressBar2.InvokeRequired)
                        {
                            progressBar2.BeginInvoke((Action)(() => progressBar2.Visible = false));
                        }
                        else
                        {
                            progressBar2.Visible = false;
                        }

                        if (panel6.InvokeRequired)
                        {
                            panel6.BeginInvoke((Action)(() => panel6.Visible = false));
                        }
                        else
                        {
                            panel6.Visible = false;
                        }

                        if (panel6.InvokeRequired)
                        {
                            panel6.BeginInvoke((Action)(() => panel6.BackColor = Color.LightGray));
                        }
                        else
                        {
                            panel6.BackColor = Color.LightGray;
                        }

                        if (label3.InvokeRequired)
                        {
                            label3.BeginInvoke((Action)(() => label3.BackColor = Color.LightGray));
                        }
                        else
                        {
                            label3.BackColor = Color.LightGray;
                        }

                        if (pictureBox2.InvokeRequired)
                        {
                            pictureBox2.BeginInvoke((Action)(() => pictureBox2.BackColor = Color.LightGray));
                        }
                        else
                        {
                            pictureBox2.BackColor = Color.LightGray;
                        }

                        if (lblAlarm2.InvokeRequired)
                        {
                            lblAlarm2.BeginInvoke((Action)(() => lblAlarm2.Visible = true));
                        }
                        else
                        {
                            lblAlarm2.Visible = true;
                        }

                        if (pictureBox2.InvokeRequired)
                        {
                            pictureBox2.BeginInvoke((Action)(() => pictureBox2.Visible = true));
                        }
                        else
                        {
                            pictureBox2.Visible = true;
                        }
                        break;
                    default:
                        if (panel6.InvokeRequired)
                        {
                            panel6.BeginInvoke((Action)(() => panel6.BackColor = Color.LightGray));
                        }
                        else
                        {
                            panel6.BackColor = Color.LightGray;
                        }

                        if (label3.InvokeRequired)
                        {
                            label3.BeginInvoke((Action)(() => label3.BackColor = Color.LightGray));
                        }
                        else
                        {
                            label3.BackColor = Color.LightGray;
                        }

                        if (pictureBox2.InvokeRequired)
                        {
                            pictureBox2.BeginInvoke((Action)(() => pictureBox2.BackColor = Color.LightGray));
                        }
                        else
                        {
                            pictureBox2.BackColor = Color.LightGray;
                        }

                        if (lblAlarm2.InvokeRequired)
                        {
                            lblAlarm2.BeginInvoke((Action)(() => lblAlarm2.Visible = false));
                        }
                        else
                        {
                            lblAlarm2.Visible = false;
                        }

                        if (pictureBox2.InvokeRequired)
                        {
                            pictureBox2.BeginInvoke((Action)(() => pictureBox2.Visible = false));
                        }
                        else
                        {
                            pictureBox2.Visible = false;
                        }
                        //test Ritesh_Change
                        //if (progressBar1.InvokeRequired)
                        //{
                        //    progressBar1.BeginInvoke((Action)(() => progressBar1.Visible = false));
                        //}
                        //else
                        //{
                        //    progressBar1.Visible = false;
                        //}
                        //if (progressBar2.InvokeRequired)
                        //{
                        //    progressBar2.BeginInvoke((Action)(() => progressBar2.Visible = false));
                        //}
                        //else
                        //{
                        //    progressBar2.Visible = false;
                        //}

                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool at;
        public bool AT
        {
            get { return at; }
            set
            {

                at = value;

                this.OnPropertyChanged("AT");

            }
        }

        private bool alarm1;
        public bool Alarm1
        {
            get { return alarm1; }
            set
            {

                alarm1 = value;

                pictureBox1.Visible = alarm1;
                lblAlarm1.Visible = alarm1;

                this.OnPropertyChanged("Alarm1");

            }
        }



        private bool alarm2;
        public bool Alarm2
        {
            get { return alarm2; }
            set
            {

                alarm2 = value;

                pictureBox2.Visible = alarm2;
                lblAlarm2.Visible = alarm2;


                this.OnPropertyChanged("Alarm2");

            }
        }


        private bool _remainTimeBool;
        public bool RemainTimeBool
        {
            get { return _remainTimeBool; }
            set
            {
                _remainTimeBool = value;

                lblRemainTime.Visible = _remainTimeBool;
                labelRemainTime.Visible = _remainTimeBool;
                this.OnPropertyChanged("RemainTimeBool");
            }
        }

        private string _remainTimeText;
        public string RemainTimeText
        {
            get { return _remainTimeText; }
            set
            {
                _remainTimeText = string.IsNullOrEmpty(value) ? "0.0" : value;
    
                if (lblRemainTime.InvokeRequired)
                {
                    lblRemainTime.BeginInvoke((Action)(() => lblRemainTime.Text = _remainTimeText));
                }
                else
                {
                    lblRemainTime.Text = _remainTimeText;
                }
                this.OnPropertyChanged("RemainTimeText");
            }
        }

        private bool _PatternStepBool;
        public bool PatternStepBool
        {
            get { return _PatternStepBool; }
            set
            {
                _PatternStepBool = value;

                lblStep.Visible = _PatternStepBool;
                labelStep.Visible = _PatternStepBool;
                this.OnPropertyChanged("PatternStepBool");
            }
        }

        private string _PatternStepText;
        public string PatternStepText
        {
            get { return _PatternStepText; }
            set
            {
                _PatternStepText = string.IsNullOrEmpty(value) ? "0.0" : value;
                if (lblStep.InvokeRequired)
                {
                    lblStep.BeginInvoke((Action)(() => lblStep.Text = _PatternStepText));
                }
                else
                {
                    lblStep.Text = _PatternStepText;
                }
                this.OnPropertyChanged("PatternStepText");
            }
        }

        public Dictionary<double, double> PvRecords { get; set; }
        public void NodeConnect( int a)
        {
          try
            {
               
                    // MonitorOnline ob = new MonitorOnline();
                   // NodeAddress = Convert.ToInt32(numericUpDown.Value);
                NodeAddress = a;
                if (btnConnect.Text == "Connect")
                {
                    bool validee = false;
                    int cnt = 0; 
                    while (!validee && cnt< 50)
                    {
                       
                       // System.Threading.Thread.Sleep(500);
                         this.Connecting = true;
                        if (this.NodeAddress > 0)
                        {
                            try
                            {
                                bool valid = AddItemCallback(NodeId.ToString());

                                if (valid)
                                {
                                    loadNode.Visible = false;
                                    validee = true;
                                    this.Connecting = false;
                                    this.ButtonText = "Disconnect";

                                    numericUpDown.Enabled = false;
                                    //ob.ContainerFilter();
                                    break;

                                }
                                cnt++;
                            }
                            catch (Exception ae)
                            { }

                         }
                        if (this.NodeAddress <= 0)
                        {
                          //  DeviceLoader.Visible = false;
                            MessageBox.Show("Select node address!!");
                        }

                    }
                    
                }
                    else if (btnConnect.Text == "Disconnect")
                    {
                        bool valid = false;
                        this.Connecting = true;
                        try
                        {
                            valid = RemoveItemCallback(NodeId.ToString());
                        }
                        catch
                        {
                            valid = true;
                        }

                        if (valid)
                        {
                            ResetControls();
                        }
                    }

                
             }
            catch (Exception ex)
            {
                //throw;
            }
        }

        public void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {


                
                
                NodeAddress = Convert.ToInt32(numericUpDown.Value);

                if (btnConnect.Text == "Connect")
                {
                    bool valid = false;
                    bool validee = false;
                    int cnt = 0; 
                    while (!validee && cnt< 20)
                    {
                       
                        this.Connecting = true;
                        if (this.NodeAddress > 0)
                        {
                            try
                            {
                                loadNode.Visible = true;
                                valid = AddItemCallback(NodeId.ToString());
                                if (valid)
                                {
                                    loadNode.Visible = false;
                                    //ob.device1ToolStripMenuItem
                                    validee = true;
                                    this.Connecting = false;
                                    this.ButtonText = "Disconnect";

                                    numericUpDown.Enabled = false;
                                    //ob.ContainerFilter();
                                    break;

                                }
                                cnt++;
                            }
                            catch (Exception ae)
                            {
                                loadNode.Visible = false;
                            }

                         }
                        if (this.NodeAddress <= 0)
                        {
                            MessageBox.Show(new Form() { TopMost = true }, "Select node address!",
                                                                                "Device Status",
                                                                                MessageBoxButtons.OK,
                                                                                MessageBoxIcon.Question);
                           // MessageBox.Show("Select node address!!");
                            cnt = 20;
                        }

                    }
                    loadNode.Visible = false;
                    if(valid==false)
                    //MessageBox.Show("Reconnect Device! device not found");
                    MessageBox.Show(new Form() { TopMost = true }, "Reconnect Device! device not found.",
                                                                                "Device Status",
                                                                                MessageBoxButtons.OK,
                                                                                MessageBoxIcon.Question);
                }
                    else if (btnConnect.Text == "Disconnect")
                    {
                        bool valid = false;
                        this.Connecting = true;
                        try
                        {
                            valid = RemoveItemCallback(NodeId.ToString());
                        }
                        catch
                        {
                            valid = true;
                        }

                        if (valid)
                        {
                            ResetControls();
                        }
                    }

                
             }
            catch (Exception ex)
            {
                //throw;
            }
        }

        public void ResetControls()
        {
            this.NodeAddress = 0;

            this.PV = "0";
            this.SV = "0";
            this.Out1Percent = 0;
            this.Out2Percent = 0;

            this.Out11 = -1;
            this.Out12 = -1;

            this.SelectedNode = false;
            this.Connecting = false;
            this.Connected = false;
            this.ButtonText = "Connect";
            this.BackgroundsColor = Color.DarkGray;

            numericUpDown.Enabled = true;

        }
        public void ResetControlsMoniter()
        {

            this.NodeAddress = 0;

            this.PV = "0";
            this.SV = "0";
            this.Out1Percent = 0;
            this.Out2Percent = 0;

            this.Out11 = -1;
            this.Out12 = -1;

           this.SelectedNode = false;
            this.Connecting = false;
            this.Connected = false;
            this.ButtonText = "Connect";
          this.BackgroundsColor = Color.DarkGray;

            numericUpDown.Enabled = true;

            panelcol = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnAddress_Click(object sender, EventArgs e)
        {
            if (this.NodeAddress > 0 && this.Connected)
            {
                bool valid = SelectedItemCallback(NodeId.ToString());
            }
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
         
            {
                pictureBox1.Invoke((Action)(() => PictureBoxChange(pictureBox1, Color.Gray)));
            }

            {
                pictureBox2.Invoke((Action)(() => PictureBoxChange(pictureBox2, Color.Gray)));
            }

            {
                pictureBox3.Invoke((Action)(() => PictureBoxChange(pictureBox3, Color.Gray)));
            }
            //if change_new 
        }

        private void PictureBoxChange(PictureBox pic, Color color)
        {
            try
            {
                pic.Image = new Bitmap(pic.Width, pic.Height);
                Graphics graphics = Graphics.FromImage(pic.Image);
                gp = new GraphicsPath();
                Brush brush = new SolidBrush(color);

                graphics.FillRectangle(brush,
                    new System.Drawing.Rectangle(0, 0, pic.Width, pic.Height));

                gp.AddEllipse(0, 0, pic.Width - 3, pic.Height - 3);

                rg = new Region(gp);
                pic.Region = rg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(NodeId + " " + ex.Message);
            }
        }

   
    }
}
