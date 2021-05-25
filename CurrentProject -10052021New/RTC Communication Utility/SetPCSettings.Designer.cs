namespace RTC_Communication_Utility
{
    partial class SetPCSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetPCSettings));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbBxMode = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBxStopBits = new System.Windows.Forms.ComboBox();
            this.cmbBxDataLength = new System.Windows.Forms.ComboBox();
            this.cmbBxParityBit = new System.Windows.Forms.ComboBox();
            this.cmbBxBaudrate = new System.Windows.Forms.ComboBox();
            this.cmbBxPorts = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblErrorMsg = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbdUsb = new System.Windows.Forms.RadioButton();
            this.rbdSerial = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1Ttl = new System.Windows.Forms.PictureBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnAutoset = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1Ttl)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbBxMode);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbBxStopBits);
            this.groupBox1.Controls.Add(this.cmbBxDataLength);
            this.groupBox1.Controls.Add(this.cmbBxParityBit);
            this.groupBox1.Controls.Add(this.cmbBxBaudrate);
            this.groupBox1.Controls.Add(this.cmbBxPorts);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(5, 154);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(282, 202);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // cmbBxMode
            // 
            this.cmbBxMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxMode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxMode.FormattingEnabled = true;
            this.cmbBxMode.Location = new System.Drawing.Point(113, 166);
            this.cmbBxMode.Name = "cmbBxMode";
            this.cmbBxMode.Size = new System.Drawing.Size(150, 23);
            this.cmbBxMode.TabIndex = 26;
            this.cmbBxMode.SelectedIndexChanged += new System.EventHandler(this.cmbBxMode_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(8, 166);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 14);
            this.label7.TabIndex = 25;
            this.label7.Text = "Protocol:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(8, 135);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 14);
            this.label5.TabIndex = 24;
            this.label5.Text = "Stop Bits:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 104);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 14);
            this.label4.TabIndex = 23;
            this.label4.Text = "DataLength:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 75);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 14);
            this.label3.TabIndex = 22;
            this.label3.Text = "Parity:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 14);
            this.label2.TabIndex = 21;
            this.label2.Text = "Baudrate:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 14);
            this.label1.TabIndex = 20;
            this.label1.Text = "Port:";
            // 
            // cmbBxStopBits
            // 
            this.cmbBxStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxStopBits.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxStopBits.FormattingEnabled = true;
            this.cmbBxStopBits.Location = new System.Drawing.Point(113, 135);
            this.cmbBxStopBits.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbBxStopBits.Name = "cmbBxStopBits";
            this.cmbBxStopBits.Size = new System.Drawing.Size(150, 23);
            this.cmbBxStopBits.TabIndex = 19;
            this.cmbBxStopBits.SelectedIndexChanged += new System.EventHandler(this.cmbBxStopBits_SelectedIndexChanged);
            // 
            // cmbBxDataLength
            // 
            this.cmbBxDataLength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxDataLength.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxDataLength.FormattingEnabled = true;
            this.cmbBxDataLength.Location = new System.Drawing.Point(113, 104);
            this.cmbBxDataLength.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbBxDataLength.Name = "cmbBxDataLength";
            this.cmbBxDataLength.Size = new System.Drawing.Size(150, 23);
            this.cmbBxDataLength.TabIndex = 18;
            this.cmbBxDataLength.SelectedIndexChanged += new System.EventHandler(this.cmbBxDataLength_SelectedIndexChanged);
            // 
            // cmbBxParityBit
            // 
            this.cmbBxParityBit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxParityBit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxParityBit.FormattingEnabled = true;
            this.cmbBxParityBit.Location = new System.Drawing.Point(113, 75);
            this.cmbBxParityBit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbBxParityBit.Name = "cmbBxParityBit";
            this.cmbBxParityBit.Size = new System.Drawing.Size(150, 23);
            this.cmbBxParityBit.TabIndex = 17;
            this.cmbBxParityBit.SelectedIndexChanged += new System.EventHandler(this.cmbBxParityBit_SelectedIndexChanged);
            // 
            // cmbBxBaudrate
            // 
            this.cmbBxBaudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxBaudrate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxBaudrate.FormattingEnabled = true;
            this.cmbBxBaudrate.Location = new System.Drawing.Point(113, 46);
            this.cmbBxBaudrate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbBxBaudrate.Name = "cmbBxBaudrate";
            this.cmbBxBaudrate.Size = new System.Drawing.Size(150, 23);
            this.cmbBxBaudrate.TabIndex = 16;
            this.cmbBxBaudrate.SelectedIndexChanged += new System.EventHandler(this.cmbBxBaudrate_SelectedIndexChanged);
            // 
            // cmbBxPorts
            // 
            this.cmbBxPorts.BackColor = System.Drawing.Color.White;
            this.cmbBxPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxPorts.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxPorts.FormattingEnabled = true;
            this.cmbBxPorts.Location = new System.Drawing.Point(113, 17);
            this.cmbBxPorts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbBxPorts.Name = "cmbBxPorts";
            this.cmbBxPorts.Size = new System.Drawing.Size(150, 23);
            this.cmbBxPorts.TabIndex = 15;
            this.cmbBxPorts.SelectedIndexChanged += new System.EventHandler(this.cmbBxPorts_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label6.Location = new System.Drawing.Point(13, 20);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(255, 66);
            this.label6.TabIndex = 11;
            this.label6.Text = "Communication\r\n     Protocol Settings";
            // 
            // lblErrorMsg
            // 
            this.lblErrorMsg.AutoSize = true;
            this.lblErrorMsg.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblErrorMsg.ForeColor = System.Drawing.Color.Red;
            this.lblErrorMsg.Location = new System.Drawing.Point(10, 401);
            this.lblErrorMsg.Name = "lblErrorMsg";
            this.lblErrorMsg.Size = new System.Drawing.Size(54, 14);
            this.lblErrorMsg.TabIndex = 12;
            this.lblErrorMsg.Text = "errorMsg";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.rbdUsb);
            this.panel1.Controls.Add(this.rbdSerial);
            this.panel1.Location = new System.Drawing.Point(5, 103);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(282, 45);
            this.panel1.TabIndex = 13;
            // 
            // rbdUsb
            // 
            this.rbdUsb.AutoSize = true;
            this.rbdUsb.Location = new System.Drawing.Point(117, 14);
            this.rbdUsb.Name = "rbdUsb";
            this.rbdUsb.Size = new System.Drawing.Size(49, 19);
            this.rbdUsb.TabIndex = 1;
            this.rbdUsb.Text = "USB";
            this.rbdUsb.UseVisualStyleBackColor = true;
            this.rbdUsb.CheckedChanged += new System.EventHandler(this.rbdUsb_CheckedChanged);
            // 
            // rbdSerial
            // 
            this.rbdSerial.AutoSize = true;
            this.rbdSerial.Checked = true;
            this.rbdSerial.Location = new System.Drawing.Point(33, 14);
            this.rbdSerial.Name = "rbdSerial";
            this.rbdSerial.Size = new System.Drawing.Size(59, 19);
            this.rbdSerial.TabIndex = 0;
            this.rbdSerial.TabStop = true;
            this.rbdSerial.Text = "Serial";
            this.rbdSerial.UseVisualStyleBackColor = true;
            this.rbdSerial.CheckedChanged += new System.EventHandler(this.rbdSerial_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(179, 107);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 37);
            this.button1.TabIndex = 14;
            this.button1.Text = "Device Manager";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1Ttl
            // 
            this.pictureBox1Ttl.BackColor = System.Drawing.Color.White;
            this.pictureBox1Ttl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1Ttl.Location = new System.Drawing.Point(5, 11);
            this.pictureBox1Ttl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox1Ttl.Name = "pictureBox1Ttl";
            this.pictureBox1Ttl.Size = new System.Drawing.Size(282, 85);
            this.pictureBox1Ttl.TabIndex = 9;
            this.pictureBox1Ttl.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(207, 362);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 32);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.Location = new System.Drawing.Point(128, 362);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(71, 32);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnAutoset
            // 
            this.btnAutoset.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAutoset.Image = ((System.Drawing.Image)(resources.GetObject("btnAutoset.Image")));
            this.btnAutoset.Location = new System.Drawing.Point(9, 362);
            this.btnAutoset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAutoset.Name = "btnAutoset";
            this.btnAutoset.Size = new System.Drawing.Size(111, 32);
            this.btnAutoset.TabIndex = 6;
            this.btnAutoset.Text = "Auto Detect";
            this.btnAutoset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAutoset.UseVisualStyleBackColor = true;
            this.btnAutoset.Click += new System.EventHandler(this.btnAutoset_Click);
            // 
            // SetPCSettings
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(293, 418);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblErrorMsg);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1Ttl);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnAutoset);
            this.Font = new System.Drawing.Font("Georgia", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetPCSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set PC Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SetPCSettings_FormClosed);
            this.Load += new System.EventHandler(this.SetPCSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1Ttl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAutoset;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pictureBox1Ttl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBxStopBits;
        private System.Windows.Forms.ComboBox cmbBxDataLength;
        private System.Windows.Forms.ComboBox cmbBxParityBit;
        private System.Windows.Forms.ComboBox cmbBxBaudrate;
        private System.Windows.Forms.ComboBox cmbBxPorts;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbBxMode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblErrorMsg;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbdUsb;
        private System.Windows.Forms.RadioButton rbdSerial;
        private System.Windows.Forms.Button button1;
    }
}