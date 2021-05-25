namespace RTC_Communication_Utility
{
    partial class RTCController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RTCController));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.protocolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setPCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDTCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.programToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateFWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.singleCommandTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphRecorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDeviceInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userControl1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripBtn1SetPC = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtn2SetDTC = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtn3Copyfn = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtn4MonitorPgm = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtn5RecorderPgm = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtn6Singlecmd = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtn7Exit = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.menuStrip1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.protocolToolStripMenuItem,
            this.programToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.newTabToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(739, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // protocolToolStripMenuItem
            // 
            this.protocolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setPCToolStripMenuItem,
            this.setDTCToolStripMenuItem});
            this.protocolToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.protocolToolStripMenuItem.Name = "protocolToolStripMenuItem";
            this.protocolToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.protocolToolStripMenuItem.Text = "Project";
            // 
            // setPCToolStripMenuItem
            // 
            this.setPCToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setPCToolStripMenuItem.Name = "setPCToolStripMenuItem";
            this.setPCToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.setPCToolStripMenuItem.Text = "Set PC Settings";
            this.setPCToolStripMenuItem.Click += new System.EventHandler(this.setPCToolStripMenuItem_Click);
            // 
            // setDTCToolStripMenuItem
            // 
            this.setDTCToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setDTCToolStripMenuItem.Name = "setDTCToolStripMenuItem";
            this.setDTCToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.setDTCToolStripMenuItem.Text = "Set TC Settings";
            this.setDTCToolStripMenuItem.Click += new System.EventHandler(this.setDTCToolStripMenuItem_Click);
            // 
            // programToolStripMenuItem
            // 
            this.programToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateFWToolStripMenuItem,
            this.singleCommandTextToolStripMenuItem,
            this.graphRecorderToolStripMenuItem,
            this.monitorToolStripMenuItem,
            this.viewDeviceInformationToolStripMenuItem});
            this.programToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.programToolStripMenuItem.Name = "programToolStripMenuItem";
            this.programToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.programToolStripMenuItem.Text = "Program";
            // 
            // updateFWToolStripMenuItem
            // 
            this.updateFWToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateFWToolStripMenuItem.Name = "updateFWToolStripMenuItem";
            this.updateFWToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.updateFWToolStripMenuItem.Text = "Update F/W";
            this.updateFWToolStripMenuItem.Click += new System.EventHandler(this.updateFWToolStripMenuItem_Click);
            // 
            // singleCommandTextToolStripMenuItem
            // 
            this.singleCommandTextToolStripMenuItem.Name = "singleCommandTextToolStripMenuItem";
            this.singleCommandTextToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.singleCommandTextToolStripMenuItem.Text = "Single Command Text";
            this.singleCommandTextToolStripMenuItem.Click += new System.EventHandler(this.singleCommandTextToolStripMenuItem_Click);
            // 
            // graphRecorderToolStripMenuItem
            // 
            this.graphRecorderToolStripMenuItem.Name = "graphRecorderToolStripMenuItem";
            this.graphRecorderToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.graphRecorderToolStripMenuItem.Text = "Graph Recorder";
            this.graphRecorderToolStripMenuItem.Click += new System.EventHandler(this.graphRecorderToolStripMenuItem_Click);
            // 
            // monitorToolStripMenuItem
            // 
            this.monitorToolStripMenuItem.Name = "monitorToolStripMenuItem";
            this.monitorToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.monitorToolStripMenuItem.Text = "Monitor";
            this.monitorToolStripMenuItem.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
            // 
            // viewDeviceInformationToolStripMenuItem
            // 
            this.viewDeviceInformationToolStripMenuItem.Name = "viewDeviceInformationToolStripMenuItem";
            this.viewDeviceInformationToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.viewDeviceInformationToolStripMenuItem.Text = "View Device Information (USB)";
            this.viewDeviceInformationToolStripMenuItem.Click += new System.EventHandler(this.viewDeviceInformationToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // newTabToolStripMenuItem
            // 
            this.newTabToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.userControl1ToolStripMenuItem,
            this.monitorFormToolStripMenuItem});
            this.newTabToolStripMenuItem.Name = "newTabToolStripMenuItem";
            this.newTabToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.newTabToolStripMenuItem.Text = "New Tab";
            this.newTabToolStripMenuItem.Visible = false;
            // 
            // userControl1ToolStripMenuItem
            // 
            this.userControl1ToolStripMenuItem.Name = "userControl1ToolStripMenuItem";
            this.userControl1ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.userControl1ToolStripMenuItem.Text = "User Control 1";
            this.userControl1ToolStripMenuItem.Click += new System.EventHandler(this.userControl1ToolStripMenuItem_Click);
            // 
            // monitorFormToolStripMenuItem
            // 
            this.monitorFormToolStripMenuItem.Name = "monitorFormToolStripMenuItem";
            this.monitorFormToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.monitorFormToolStripMenuItem.Text = "MonitorForm";
            this.monitorFormToolStripMenuItem.Click += new System.EventHandler(this.monitorFormToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(35, 35);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBtn1SetPC,
            this.toolStripBtn2SetDTC,
            this.toolStripBtn3Copyfn,
            this.toolStripBtn4MonitorPgm,
            this.toolStripBtn5RecorderPgm,
            this.toolStripBtn6Singlecmd,
            this.toolStripBtn7Exit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(739, 40);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripBtn1SetPC
            // 
            this.toolStripBtn1SetPC.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtn1SetPC.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripBtn1SetPC.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtn1SetPC.Image")));
            this.toolStripBtn1SetPC.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtn1SetPC.Name = "toolStripBtn1SetPC";
            this.toolStripBtn1SetPC.Size = new System.Drawing.Size(39, 37);
            this.toolStripBtn1SetPC.Text = "Set PC Protocol";
            this.toolStripBtn1SetPC.Click += new System.EventHandler(this.toolStripBtn1SetPC_Click);
            // 
            // toolStripBtn2SetDTC
            // 
            this.toolStripBtn2SetDTC.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtn2SetDTC.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripBtn2SetDTC.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtn2SetDTC.Image")));
            this.toolStripBtn2SetDTC.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtn2SetDTC.Name = "toolStripBtn2SetDTC";
            this.toolStripBtn2SetDTC.Size = new System.Drawing.Size(39, 37);
            this.toolStripBtn2SetDTC.Text = "Set Controller Parameter";
            this.toolStripBtn2SetDTC.Click += new System.EventHandler(this.toolStripBtn2SetDTC_Click);
            // 
            // toolStripBtn3Copyfn
            // 
            this.toolStripBtn3Copyfn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtn3Copyfn.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripBtn3Copyfn.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtn3Copyfn.Image")));
            this.toolStripBtn3Copyfn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtn3Copyfn.Name = "toolStripBtn3Copyfn";
            this.toolStripBtn3Copyfn.Size = new System.Drawing.Size(39, 37);
            this.toolStripBtn3Copyfn.Text = "TC Update Program";
            this.toolStripBtn3Copyfn.Click += new System.EventHandler(this.toolStripBtn3Copyfn_Click);
            // 
            // toolStripBtn4MonitorPgm
            // 
            this.toolStripBtn4MonitorPgm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtn4MonitorPgm.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripBtn4MonitorPgm.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtn4MonitorPgm.Image")));
            this.toolStripBtn4MonitorPgm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtn4MonitorPgm.Name = "toolStripBtn4MonitorPgm";
            this.toolStripBtn4MonitorPgm.Size = new System.Drawing.Size(39, 37);
            this.toolStripBtn4MonitorPgm.Text = "Online Monitoring";
            this.toolStripBtn4MonitorPgm.Click += new System.EventHandler(this.toolStripBtn4MonitorPgm_Click);
            // 
            // toolStripBtn5RecorderPgm
            // 
            this.toolStripBtn5RecorderPgm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtn5RecorderPgm.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripBtn5RecorderPgm.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtn5RecorderPgm.Image")));
            this.toolStripBtn5RecorderPgm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtn5RecorderPgm.Name = "toolStripBtn5RecorderPgm";
            this.toolStripBtn5RecorderPgm.Size = new System.Drawing.Size(39, 37);
            this.toolStripBtn5RecorderPgm.Text = "Record Graph";
            this.toolStripBtn5RecorderPgm.Click += new System.EventHandler(this.toolStripBtn5RecorderPgm_Click);
            // 
            // toolStripBtn6Singlecmd
            // 
            this.toolStripBtn6Singlecmd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtn6Singlecmd.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripBtn6Singlecmd.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtn6Singlecmd.Image")));
            this.toolStripBtn6Singlecmd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtn6Singlecmd.Name = "toolStripBtn6Singlecmd";
            this.toolStripBtn6Singlecmd.Size = new System.Drawing.Size(39, 37);
            this.toolStripBtn6Singlecmd.Text = "Read/Write Register";
            this.toolStripBtn6Singlecmd.Click += new System.EventHandler(this.toolStripBtn6Singlecmd_Click);
            // 
            // toolStripBtn7Exit
            // 
            this.toolStripBtn7Exit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtn7Exit.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripBtn7Exit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtn7Exit.Image")));
            this.toolStripBtn7Exit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtn7Exit.Name = "toolStripBtn7Exit";
            this.toolStripBtn7Exit.Size = new System.Drawing.Size(39, 37);
            this.toolStripBtn7Exit.Text = "Exit";
            this.toolStripBtn7Exit.Click += new System.EventHandler(this.toolStripBtn7Exit_Click);
            // 
            // RTCController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(739, 496);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RTCController";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RTC Communication Program";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RTCController_FormClosing);
            this.Load += new System.EventHandler(this.RTCController_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RTCController_Paint);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem protocolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setPCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setDTCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem programToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripBtn1SetPC;
        private System.Windows.Forms.ToolStripButton toolStripBtn2SetDTC;
        private System.Windows.Forms.ToolStripButton toolStripBtn3Copyfn;
        private System.Windows.Forms.ToolStripButton toolStripBtn4MonitorPgm;
        private System.Windows.Forms.ToolStripButton toolStripBtn5RecorderPgm;
        private System.Windows.Forms.ToolStripButton toolStripBtn6Singlecmd;
        private System.Windows.Forms.ToolStripButton toolStripBtn7Exit;
        private System.Windows.Forms.ToolStripMenuItem updateFWToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userControl1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem monitorFormToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem singleCommandTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem graphRecorderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem monitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewDeviceInformationToolStripMenuItem;
    }
}

