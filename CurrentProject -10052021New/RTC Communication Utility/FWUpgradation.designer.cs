namespace RTC_Communication_Utility
{
    partial class FWUpgradation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FWUpgradation));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbBtnBase = new System.Windows.Forms.RadioButton();
            this.rbBtnExp = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.rdbtn_Close = new System.Windows.Forms.Button();
            this.btn_Download = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtB_NodeAddress = new System.Windows.Forms.NumericUpDown();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grpBx_download = new System.Windows.Forms.GroupBox();
            this.lbDownload_Progress = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pgbDownload = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdBtnCali = new System.Windows.Forms.RadioButton();
            this.rdBtnUser = new System.Windows.Forms.RadioButton();
            this.btnCMode = new System.Windows.Forms.Button();
            this.btnDMode = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtB_NodeAddress)).BeginInit();
            this.grpBx_download.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.rdbtn_Close);
            this.groupBox1.Controls.Add(this.btn_Download);
            this.groupBox1.Controls.Add(this.btnOpen);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtB_NodeAddress);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.grpBx_download);
            this.groupBox1.Location = new System.Drawing.Point(4, 74);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(331, 194);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbBtnBase);
            this.groupBox3.Controls.Add(this.rbBtnExp);
            this.groupBox3.Location = new System.Drawing.Point(100, 44);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(207, 37);
            this.groupBox3.TabIndex = 67;
            this.groupBox3.TabStop = false;
            // 
            // rbBtnBase
            // 
            this.rbBtnBase.AutoSize = true;
            this.rbBtnBase.Checked = true;
            this.rbBtnBase.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbBtnBase.Location = new System.Drawing.Point(5, 13);
            this.rbBtnBase.Name = "rbBtnBase";
            this.rbBtnBase.Size = new System.Drawing.Size(75, 18);
            this.rbBtnBase.TabIndex = 70;
            this.rbBtnBase.TabStop = true;
            this.rbBtnBase.Text = "Main Unit";
            this.rbBtnBase.UseVisualStyleBackColor = true;
            this.rbBtnBase.CheckedChanged += new System.EventHandler(this.rbBtnBase_CheckedChanged);
            // 
            // rbBtnExp
            // 
            this.rbBtnExp.AutoSize = true;
            this.rbBtnExp.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbBtnExp.Location = new System.Drawing.Point(93, 13);
            this.rbBtnExp.Name = "rbBtnExp";
            this.rbBtnExp.Size = new System.Drawing.Size(105, 18);
            this.rbBtnExp.TabIndex = 71;
            this.rbBtnExp.Text = "Expansion Unit";
            this.rbBtnExp.UseVisualStyleBackColor = true;
            this.rbBtnExp.CheckedChanged += new System.EventHandler(this.rbBtnExp_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(2, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 14);
            this.label5.TabIndex = 72;
            this.label5.Text = "Device Type :";
            // 
            // rdbtn_Close
            // 
            this.rdbtn_Close.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbtn_Close.Image = ((System.Drawing.Image)(resources.GetObject("rdbtn_Close.Image")));
            this.rdbtn_Close.Location = new System.Drawing.Point(217, 88);
            this.rdbtn_Close.Name = "rdbtn_Close";
            this.rdbtn_Close.Size = new System.Drawing.Size(90, 34);
            this.rdbtn_Close.TabIndex = 69;
            this.rdbtn_Close.Text = "Close";
            this.rdbtn_Close.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rdbtn_Close.UseVisualStyleBackColor = true;
            this.rdbtn_Close.Click += new System.EventHandler(this.rdbtn_Close_Click);
            // 
            // btn_Download
            // 
            this.btn_Download.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Download.Image = ((System.Drawing.Image)(resources.GetObject("btn_Download.Image")));
            this.btn_Download.Location = new System.Drawing.Point(100, 89);
            this.btn_Download.Name = "btn_Download";
            this.btn_Download.Size = new System.Drawing.Size(111, 34);
            this.btn_Download.TabIndex = 68;
            this.btn_Download.Text = "Upgrade";
            this.btn_Download.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Download.UseVisualStyleBackColor = true;
            this.btn_Download.Click += new System.EventHandler(this.btn_Upgrade_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Font = new System.Drawing.Font("Georgia", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpen.Location = new System.Drawing.Point(252, 324);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(68, 23);
            this.btnOpen.TabIndex = 67;
            this.btnOpen.Text = "Browse";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Georgia", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 328);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 15);
            this.label4.TabIndex = 65;
            this.label4.Text = "Browse";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(188, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 14);
            this.label3.TabIndex = 64;
            this.label3.Text = "(1-247)";
            // 
            // txtB_NodeAddress
            // 
            this.txtB_NodeAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtB_NodeAddress.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtB_NodeAddress.Location = new System.Drawing.Point(104, 16);
            this.txtB_NodeAddress.Maximum = new decimal(new int[] {
            247,
            0,
            0,
            0});
            this.txtB_NodeAddress.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtB_NodeAddress.Name = "txtB_NodeAddress";
            this.txtB_NodeAddress.Size = new System.Drawing.Size(79, 22);
            this.txtB_NodeAddress.TabIndex = 63;
            this.txtB_NodeAddress.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(72, 323);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(165, 20);
            this.textBox1.TabIndex = 59;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(2, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 14);
            this.label2.TabIndex = 58;
            this.label2.Text = "Node Address :";
            // 
            // grpBx_download
            // 
            this.grpBx_download.BackColor = System.Drawing.Color.White;
            this.grpBx_download.Controls.Add(this.lbDownload_Progress);
            this.grpBx_download.Controls.Add(this.statusStrip1);
            this.grpBx_download.Controls.Add(this.pgbDownload);
            this.grpBx_download.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBx_download.ForeColor = System.Drawing.Color.Black;
            this.grpBx_download.Location = new System.Drawing.Point(6, 129);
            this.grpBx_download.Name = "grpBx_download";
            this.grpBx_download.Size = new System.Drawing.Size(319, 55);
            this.grpBx_download.TabIndex = 2;
            this.grpBx_download.TabStop = false;
            // 
            // lbDownload_Progress
            // 
            this.lbDownload_Progress.AutoSize = true;
            this.lbDownload_Progress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDownload_Progress.ForeColor = System.Drawing.Color.Black;
            this.lbDownload_Progress.Location = new System.Drawing.Point(287, 9);
            this.lbDownload_Progress.Name = "lbDownload_Progress";
            this.lbDownload_Progress.Size = new System.Drawing.Size(13, 13);
            this.lbDownload_Progress.TabIndex = 3;
            this.lbDownload_Progress.Text = "0";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.White;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(3, 30);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(313, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.TextChanged += new System.EventHandler(this.statusStrip1_TextChanged);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(40, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // pgbDownload
            // 
            this.pgbDownload.Location = new System.Drawing.Point(6, 9);
            this.pgbDownload.Name = "pgbDownload";
            this.pgbDownload.Size = new System.Drawing.Size(275, 10);
            this.pgbDownload.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdBtnCali);
            this.groupBox2.Controls.Add(this.rdBtnUser);
            this.groupBox2.Location = new System.Drawing.Point(351, 163);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(221, 36);
            this.groupBox2.TabIndex = 75;
            this.groupBox2.TabStop = false;
            this.groupBox2.Visible = false;
            // 
            // rdBtnCali
            // 
            this.rdBtnCali.AutoSize = true;
            this.rdBtnCali.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdBtnCali.Location = new System.Drawing.Point(6, 10);
            this.rdBtnCali.Name = "rdBtnCali";
            this.rdBtnCali.Size = new System.Drawing.Size(105, 18);
            this.rdBtnCali.TabIndex = 73;
            this.rdBtnCali.Text = "Calibration F/w";
            this.rdBtnCali.UseVisualStyleBackColor = true;
            // 
            // rdBtnUser
            // 
            this.rdBtnUser.AutoSize = true;
            this.rdBtnUser.Checked = true;
            this.rdBtnUser.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdBtnUser.Location = new System.Drawing.Point(131, 10);
            this.rdBtnUser.Name = "rdBtnUser";
            this.rdBtnUser.Size = new System.Drawing.Size(74, 18);
            this.rdBtnUser.TabIndex = 74;
            this.rdBtnUser.TabStop = true;
            this.rdBtnUser.Text = "User F/w";
            this.rdBtnUser.UseVisualStyleBackColor = true;
            this.rdBtnUser.CheckedChanged += new System.EventHandler(this.rdBtnUser_CheckedChanged);
            // 
            // btnCMode
            // 
            this.btnCMode.Location = new System.Drawing.Point(463, 134);
            this.btnCMode.Name = "btnCMode";
            this.btnCMode.Size = new System.Drawing.Size(101, 23);
            this.btnCMode.TabIndex = 77;
            this.btnCMode.Text = "Calibration Mode";
            this.btnCMode.UseVisualStyleBackColor = true;
            this.btnCMode.Click += new System.EventHandler(this.btnCMode_Click);
            // 
            // btnDMode
            // 
            this.btnDMode.Location = new System.Drawing.Point(349, 134);
            this.btnDMode.Name = "btnDMode";
            this.btnDMode.Size = new System.Drawing.Size(100, 23);
            this.btnDMode.TabIndex = 76;
            this.btnDMode.Text = "Download Mode";
            this.btnDMode.UseVisualStyleBackColor = true;
            this.btnDMode.Click += new System.EventHandler(this.btnDMode_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(43, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(235, 33);
            this.label1.TabIndex = 0;
            this.label1.Text = "Upgrade Firmware";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(4, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(331, 56);
            this.panel1.TabIndex = 66;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(348, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(262, 13);
            this.label6.TabIndex = 78;
            this.label6.Text = "Click Download Mode if device not in bootblock mode";
            // 
            // FWUpgradation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(343, 270);
            this.Controls.Add(this.btnDMode);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCMode);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FWUpgradation";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Firmware Download";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FWUpgradation_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtB_NodeAddress)).EndInit();
            this.grpBx_download.ResumeLayout(false);
            this.grpBx_download.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox grpBx_download;
        private System.Windows.Forms.Label lbDownload_Progress;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ProgressBar pgbDownload;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.NumericUpDown txtB_NodeAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button rdbtn_Close;
        private System.Windows.Forms.Button btn_Download;
        private System.Windows.Forms.RadioButton rbBtnExp;
        private System.Windows.Forms.RadioButton rbBtnBase;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rdBtnUser;
        private System.Windows.Forms.RadioButton rdBtnCali;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnCMode;
        private System.Windows.Forms.Button btnDMode;
        private System.Windows.Forms.Label label6;
    }
}

