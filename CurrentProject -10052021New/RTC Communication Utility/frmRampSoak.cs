using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTC_Communication_Utility
{
    public partial class frmRampSoak : Form
    {
        public delegate void GetRampStart();
        public event GetRampStart _getRampStart;

        public delegate void GetRampStop();
        public event GetRampStop _getRampStop;

        public List<string> list { get; set; }

        public int RampSoakNodeAddress { get; set; }

        int count = 1;

        bool progress = false;

        Thread th = null;

        public frmRampSoak()
        {
            InitializeComponent();
        }

        private void frmRampSoak_Load(object sender, EventArgs e)
        {
            label2.Text = "Node Address : " + Convert.ToString(RampSoakNodeAddress);

            btnStart.Enabled = true;
            btnStop.Enabled = false;

            if (th == null)
            {
                th = new Thread(progressBar);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        private void progressBar()
        {
            try
            {
                if (RampSoakNodeAddress > 0)
                {
                    while (progress)
                    {
                        if (count < 99)
                        {
                            progressBar1.Invoke((Action)(() => { progressBar1.Value = count++; }));
                        }
                        Thread.Sleep(500);
                    }

                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

        }

        private void Stop()
        {
            try
            {
                count = 0;
                progress = false;

                if (th != null)
                {
                    th.Abort();
                    th = null;
                }

                _getRampStop();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BindGrid()
        {
            try
            {
                if (listBox1.InvokeRequired)
                {
                    listBox1.Invoke((Action)(() =>
                    {
                        listBox1.DataSource = list;
                        listBox1.Refresh();
                    }));
                    progressBar1.Invoke((Action)(() => { progressBar1.Value = 100; }));
                    Stop();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
