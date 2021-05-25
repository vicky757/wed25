using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTC_Communication_Utility
{
    public partial class TryOutForm : Form
    {
        public TryOutForm()
        {
            InitializeComponent();
        }

        private void TryOutForm_Load(object sender, EventArgs e)
        {
            fILEToolStripMenuItem.Enabled = false;
            pARAMETERSToolStripMenuItem.Enabled = false;
            vIEWToolStripMenuItem.Enabled = false;
        }

        private void MakeMenusActiveInactive(ToolStripMenuItem deviceToolStripMenuItem)
        {
            string input = deviceToolStripMenuItem.Text;


            if (deviceToolStripMenuItem.Text.Contains("(ACTIVE)"))
            {
                int index = input.IndexOf("(");

                if (index > 0)
                    input = input.Substring(0, index);
                deviceToolStripMenuItem.Text = input;
            }
            else
            {
                deviceToolStripMenuItem.Text = input + " (ACTIVE)";
            }
        }

        #region Device active

        private void device1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeMenusActiveInactive(device1ToolStripMenuItem);
        }

        private void device2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeMenusActiveInactive(device2ToolStripMenuItem);
        }

        private void device3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeMenusActiveInactive(device3ToolStripMenuItem);
        }

        private void device4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeMenusActiveInactive(device4ToolStripMenuItem);
        }

        private void device5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeMenusActiveInactive(device5ToolStripMenuItem);
        }

        private void device6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeMenusActiveInactive(device6ToolStripMenuItem);
        }

        private void device7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeMenusActiveInactive(device7ToolStripMenuItem);
        }

        private void device8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeMenusActiveInactive(device8ToolStripMenuItem);
        }
        #endregion
    }
}
