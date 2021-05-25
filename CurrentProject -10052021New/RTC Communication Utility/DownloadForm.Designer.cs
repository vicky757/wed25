namespace RTC_Communication_Utility
{
    partial class DownloadForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblMainMessage = new System.Windows.Forms.Label();
            this.lblDownloadProgress = new System.Windows.Forms.Label();
            this.btnCancel = new GaryPerkin.UserControls.Buttons.RoundButton();
            this.btnUpgrade1 = new GaryPerkin.UserControls.Buttons.RoundButton();
            this.pgbDownload = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFirmwarePath = new System.Windows.Forms.TextBox();
            this.btnOpen = new GaryPerkin.UserControls.Buttons.RoundButton();
            this.nud_NodeAddresss = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_NodeAddresss)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(102, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(320, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "Upgrade Firmware";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.lblMainMessage);
            this.groupBox1.Controls.Add(this.lblDownloadProgress);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnUpgrade1);
            this.groupBox1.Controls.Add(this.pgbDownload);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtFirmwarePath);
            this.groupBox1.Controls.Add(this.btnOpen);
            this.groupBox1.Controls.Add(this.nud_NodeAddresss);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(13, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(511, 233);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // lblMainMessage
            // 
            this.lblMainMessage.AutoSize = true;
            this.lblMainMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMainMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMainMessage.Location = new System.Drawing.Point(10, 168);
            this.lblMainMessage.Name = "lblMainMessage";
            this.lblMainMessage.Size = new System.Drawing.Size(0, 16);
            this.lblMainMessage.TabIndex = 68;
            // 
            // lblDownloadProgress
            // 
            this.lblDownloadProgress.AutoSize = true;
            this.lblDownloadProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDownloadProgress.Location = new System.Drawing.Point(478, 138);
            this.lblDownloadProgress.Name = "lblDownloadProgress";
            this.lblDownloadProgress.Size = new System.Drawing.Size(27, 16);
            this.lblDownloadProgress.TabIndex = 67;
            this.lblDownloadProgress.Text = "0%";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Bisque;
            this.btnCancel.BevelDepth = 5;
            this.btnCancel.BevelHeight = 5;
            this.btnCancel.Dome = true;
            this.btnCancel.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(425, 167);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.RecessDepth = 0;
            this.btnCancel.Size = new System.Drawing.Size(80, 46);
            this.btnCancel.TabIndex = 66;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnUpgrade1
            // 
            this.btnUpgrade1.BackColor = System.Drawing.Color.Bisque;
            this.btnUpgrade1.BevelDepth = 5;
            this.btnUpgrade1.BevelHeight = 5;
            this.btnUpgrade1.Dome = true;
            this.btnUpgrade1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpgrade1.Location = new System.Drawing.Point(329, 167);
            this.btnUpgrade1.Name = "btnUpgrade1";
            this.btnUpgrade1.RecessDepth = 0;
            this.btnUpgrade1.Size = new System.Drawing.Size(80, 46);
            this.btnUpgrade1.TabIndex = 65;
            this.btnUpgrade1.Text = "Upgrade";
            this.btnUpgrade1.UseVisualStyleBackColor = false;
            this.btnUpgrade1.Click += new System.EventHandler(this.btnUpgrade1_Click);
            // 
            // pgbDownload
            // 
            this.pgbDownload.Location = new System.Drawing.Point(7, 138);
            this.pgbDownload.Name = "pgbDownload";
            this.pgbDownload.Size = new System.Drawing.Size(467, 23);
            this.pgbDownload.TabIndex = 64;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(170, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 63;
            this.label3.Text = "(1-255)";
            // 
            // txtFirmwarePath
            // 
            this.txtFirmwarePath.Location = new System.Drawing.Point(84, 89);
            this.txtFirmwarePath.Name = "txtFirmwarePath";
            this.txtFirmwarePath.Size = new System.Drawing.Size(421, 20);
            this.txtFirmwarePath.TabIndex = 62;
            // 
            // btnOpen
            // 
            this.btnOpen.BackColor = System.Drawing.Color.Bisque;
            this.btnOpen.BevelDepth = 5;
            this.btnOpen.BevelHeight = 5;
            this.btnOpen.Dome = true;
            this.btnOpen.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpen.Location = new System.Drawing.Point(6, 78);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.RecessDepth = 0;
            this.btnOpen.Size = new System.Drawing.Size(71, 44);
            this.btnOpen.TabIndex = 61;
            this.btnOpen.Text = "Browse";
            this.btnOpen.UseVisualStyleBackColor = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // nud_NodeAddresss
            // 
            this.nud_NodeAddresss.Location = new System.Drawing.Point(96, 29);
            this.nud_NodeAddresss.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_NodeAddresss.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_NodeAddresss.Name = "nud_NodeAddresss";
            this.nud_NodeAddresss.Size = new System.Drawing.Size(59, 20);
            this.nud_NodeAddresss.TabIndex = 1;
            this.nud_NodeAddresss.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Node Address : ";
            // 
            // DownloadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(536, 311);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "DownloadForm";
            this.Text = "DownloadForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_NodeAddresss)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nud_NodeAddresss;
        private System.Windows.Forms.Label label2;

        private System.Windows.Forms.ProgressBar pgbDownload;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFirmwarePath;

        private System.Windows.Forms.Label lblDownloadProgress;
        private System.Windows.Forms.Label lblMainMessage;
    }
}