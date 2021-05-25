namespace RTC_Communication_Utility
{
    partial class AutoSetForm
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
            this.label7 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cmbBxProtocol = new System.Windows.Forms.ComboBox();
            this.cmbBxStopbits = new System.Windows.Forms.ComboBox();
            this.cmbBxParity = new System.Windows.Forms.ComboBox();
            this.cmbBxBitslength = new System.Windows.Forms.ComboBox();
            this.btnSet = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.pictureBox1Ttl = new System.Windows.Forms.PictureBox();
            this.cmbBxBaudrate = new System.Windows.Forms.ComboBox();
            this.cmbBxAddress = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblMsg = new System.Windows.Forms.Label();
            this.progressBarSearch = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1Ttl)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.White;
            this.label7.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(29, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(255, 66);
            this.label7.TabIndex = 24;
            this.label7.Text = "Communication\r\n     Protocol Settings";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.ColumnHeadersVisible = false;
            this.dataGridView1.Location = new System.Drawing.Point(7, 108);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(289, 95);
            this.dataGridView1.TabIndex = 26;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // cmbBxProtocol
            // 
            this.cmbBxProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxProtocol.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxProtocol.FormattingEnabled = true;
            this.cmbBxProtocol.Location = new System.Drawing.Point(213, 79);
            this.cmbBxProtocol.Name = "cmbBxProtocol";
            this.cmbBxProtocol.Size = new System.Drawing.Size(82, 23);
            this.cmbBxProtocol.TabIndex = 25;
            this.cmbBxProtocol.SelectedIndexChanged += new System.EventHandler(this.cmbBxProtocol_SelectedIndexChanged);
            // 
            // cmbBxStopbits
            // 
            this.cmbBxStopbits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxStopbits.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxStopbits.FormattingEnabled = true;
            this.cmbBxStopbits.Location = new System.Drawing.Point(114, 79);
            this.cmbBxStopbits.Name = "cmbBxStopbits";
            this.cmbBxStopbits.Size = new System.Drawing.Size(82, 23);
            this.cmbBxStopbits.TabIndex = 24;
            this.cmbBxStopbits.SelectedIndexChanged += new System.EventHandler(this.cmbBxStopbits_SelectedIndexChanged);
            // 
            // cmbBxParity
            // 
            this.cmbBxParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxParity.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxParity.FormattingEnabled = true;
            this.cmbBxParity.Location = new System.Drawing.Point(7, 79);
            this.cmbBxParity.Name = "cmbBxParity";
            this.cmbBxParity.Size = new System.Drawing.Size(82, 23);
            this.cmbBxParity.TabIndex = 23;
            this.cmbBxParity.SelectedIndexChanged += new System.EventHandler(this.cmbBxParity_SelectedIndexChanged);
            // 
            // cmbBxBitslength
            // 
            this.cmbBxBitslength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxBitslength.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxBitslength.FormattingEnabled = true;
            this.cmbBxBitslength.Location = new System.Drawing.Point(213, 36);
            this.cmbBxBitslength.Name = "cmbBxBitslength";
            this.cmbBxBitslength.Size = new System.Drawing.Size(82, 23);
            this.cmbBxBitslength.TabIndex = 22;
            this.cmbBxBitslength.SelectedIndexChanged += new System.EventHandler(this.cmbBxBitslength_SelectedIndexChanged);
            // 
            // btnSet
            // 
            this.btnSet.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSet.Location = new System.Drawing.Point(121, 312);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(87, 30);
            this.btnSet.TabIndex = 21;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(14, 312);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(87, 30);
            this.btnStart.TabIndex = 20;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pictureBox1Ttl
            // 
            this.pictureBox1Ttl.BackColor = System.Drawing.Color.White;
            this.pictureBox1Ttl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1Ttl.Location = new System.Drawing.Point(3, 13);
            this.pictureBox1Ttl.Name = "pictureBox1Ttl";
            this.pictureBox1Ttl.Size = new System.Drawing.Size(306, 81);
            this.pictureBox1Ttl.TabIndex = 19;
            this.pictureBox1Ttl.TabStop = false;
            // 
            // cmbBxBaudrate
            // 
            this.cmbBxBaudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxBaudrate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxBaudrate.FormattingEnabled = true;
            this.cmbBxBaudrate.Location = new System.Drawing.Point(114, 35);
            this.cmbBxBaudrate.Name = "cmbBxBaudrate";
            this.cmbBxBaudrate.Size = new System.Drawing.Size(82, 23);
            this.cmbBxBaudrate.TabIndex = 21;
            this.cmbBxBaudrate.SelectedIndexChanged += new System.EventHandler(this.cmbBxBaudrate_SelectedIndexChanged);
            // 
            // cmbBxAddress
            // 
            this.cmbBxAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxAddress.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxAddress.FormattingEnabled = true;
            this.cmbBxAddress.Location = new System.Drawing.Point(7, 34);
            this.cmbBxAddress.Name = "cmbBxAddress";
            this.cmbBxAddress.Size = new System.Drawing.Size(82, 23);
            this.cmbBxAddress.TabIndex = 20;
            this.cmbBxAddress.SelectedIndexChanged += new System.EventHandler(this.cmbBxAddress_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(126, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 16);
            this.label6.TabIndex = 19;
            this.label6.Text = "Stop Bits";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(217, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 16);
            this.label5.TabIndex = 18;
            this.label5.Text = "Bit Length";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(217, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 16);
            this.label4.TabIndex = 17;
            this.label4.Text = "Protocol";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 16);
            this.label3.TabIndex = 16;
            this.label3.Text = "Parity";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(125, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 16);
            this.label2.TabIndex = 15;
            this.label2.Text = "Baudrate";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 16);
            this.label1.TabIndex = 14;
            this.label1.Text = "Address";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Controls.Add(this.cmbBxProtocol);
            this.groupBox1.Controls.Add(this.cmbBxStopbits);
            this.groupBox1.Controls.Add(this.cmbBxParity);
            this.groupBox1.Controls.Add(this.cmbBxBitslength);
            this.groupBox1.Controls.Add(this.cmbBxBaudrate);
            this.groupBox1.Controls.Add(this.cmbBxAddress);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 212);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(220, 312);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(87, 30);
            this.btnClose.TabIndex = 22;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Location = new System.Drawing.Point(10, 349);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(0, 14);
            this.lblMsg.TabIndex = 25;
            // 
            // progressBarSearch
            // 
            this.progressBarSearch.Location = new System.Drawing.Point(100, 348);
            this.progressBarSearch.Name = "progressBarSearch";
            this.progressBarSearch.Size = new System.Drawing.Size(199, 12);
            this.progressBarSearch.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarSearch.TabIndex = 26;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(97, 363);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(23, 14);
            this.lblStatus.TabIndex = 27;
            this.lblStatus.Text = "----";
            // 
            // AutoSetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 386);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBarSearch);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.pictureBox1Ttl);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AutoSetForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Auto Detect";
            this.Load += new System.EventHandler(this.AutoSetForm_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1Ttl)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox cmbBxProtocol;
        private System.Windows.Forms.ComboBox cmbBxStopbits;
        private System.Windows.Forms.ComboBox cmbBxParity;
        private System.Windows.Forms.ComboBox cmbBxBitslength;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PictureBox pictureBox1Ttl;
        private System.Windows.Forms.ComboBox cmbBxBaudrate;
        private System.Windows.Forms.ComboBox cmbBxAddress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.ProgressBar progressBarSearch;
        private System.Windows.Forms.Label lblStatus;
    }
}