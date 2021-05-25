namespace RTC_Communication_Utility
{
    partial class RampSoakForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RampSoakForm));
            this.lblBottomText = new System.Windows.Forms.Label();
            this.numericUpDownMin = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownHr = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnTime = new System.Windows.Forms.Button();
            this.grpBxLinkPattern = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbBxLinkPattern = new System.Windows.Forms.ComboBox();
            this.btnLinkPattern = new System.Windows.Forms.Button();
            this.grpBxStep = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnStep = new System.Windows.Forms.Button();
            this.cmbBxStep = new System.Windows.Forms.ComboBox();
            this.grpBxSV = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSV = new System.Windows.Forms.TextBox();
            this.btnSV = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.grpBxTime = new System.Windows.Forms.GroupBox();
            this.btnLoopCount = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.btnReadFile = new System.Windows.Forms.Button();
            this.btnReadSettings = new System.Windows.Forms.Button();
            this.cmbBxStartPattern = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpBxLoop = new System.Windows.Forms.GroupBox();
            this.txtLoopCount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHr)).BeginInit();
            this.grpBxLinkPattern.SuspendLayout();
            this.grpBxStep.SuspendLayout();
            this.grpBxSV.SuspendLayout();
            this.grpBxTime.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpBxLoop.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBottomText
            // 
            this.lblBottomText.AutoSize = true;
            this.lblBottomText.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBottomText.Location = new System.Drawing.Point(12, 484);
            this.lblBottomText.Name = "lblBottomText";
            this.lblBottomText.Size = new System.Drawing.Size(46, 16);
            this.lblBottomText.TabIndex = 22;
            this.lblBottomText.Text = "label8";
            // 
            // numericUpDownMin
            // 
            this.numericUpDownMin.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDownMin.Location = new System.Drawing.Point(8, 84);
            this.numericUpDownMin.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericUpDownMin.Name = "numericUpDownMin";
            this.numericUpDownMin.Size = new System.Drawing.Size(100, 23);
            this.numericUpDownMin.TabIndex = 6;
            // 
            // numericUpDownHr
            // 
            this.numericUpDownHr.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDownHr.Location = new System.Drawing.Point(10, 31);
            this.numericUpDownHr.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numericUpDownHr.Name = "numericUpDownHr";
            this.numericUpDownHr.Size = new System.Drawing.Size(98, 23);
            this.numericUpDownHr.TabIndex = 5;
            this.numericUpDownHr.ValueChanged += new System.EventHandler(this.numericUpDownHr_ValueChanged);
            this.numericUpDownHr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownHr_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(8, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "Minute";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Hour";
            // 
            // btnTime
            // 
            this.btnTime.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTime.Location = new System.Drawing.Point(9, 110);
            this.btnTime.Name = "btnTime";
            this.btnTime.Size = new System.Drawing.Size(75, 23);
            this.btnTime.TabIndex = 2;
            this.btnTime.Text = "Enter";
            this.btnTime.UseVisualStyleBackColor = true;
            this.btnTime.Click += new System.EventHandler(this.btnTime_Click);
            // 
            // grpBxLinkPattern
            // 
            this.grpBxLinkPattern.Controls.Add(this.label3);
            this.grpBxLinkPattern.Controls.Add(this.cmbBxLinkPattern);
            this.grpBxLinkPattern.Controls.Add(this.btnLinkPattern);
            this.grpBxLinkPattern.Location = new System.Drawing.Point(1073, 172);
            this.grpBxLinkPattern.Name = "grpBxLinkPattern";
            this.grpBxLinkPattern.Size = new System.Drawing.Size(120, 142);
            this.grpBxLinkPattern.TabIndex = 19;
            this.grpBxLinkPattern.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Link Pattern";
            // 
            // cmbBxLinkPattern
            // 
            this.cmbBxLinkPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxLinkPattern.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxLinkPattern.FormattingEnabled = true;
            this.cmbBxLinkPattern.Items.AddRange(new object[] {
            "pat0",
            "pat1",
            "pat2",
            "pat3",
            "pat4",
            "pat5",
            "pat6",
            "pat7",
            "END"});
            this.cmbBxLinkPattern.Location = new System.Drawing.Point(7, 47);
            this.cmbBxLinkPattern.Name = "cmbBxLinkPattern";
            this.cmbBxLinkPattern.Size = new System.Drawing.Size(107, 24);
            this.cmbBxLinkPattern.TabIndex = 1;
            this.cmbBxLinkPattern.SelectedIndexChanged += new System.EventHandler(this.cmbBxLinkPattern_SelectedIndexChanged);
            // 
            // btnLinkPattern
            // 
            this.btnLinkPattern.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLinkPattern.Location = new System.Drawing.Point(6, 110);
            this.btnLinkPattern.Name = "btnLinkPattern";
            this.btnLinkPattern.Size = new System.Drawing.Size(75, 23);
            this.btnLinkPattern.TabIndex = 0;
            this.btnLinkPattern.Text = "Enter";
            this.btnLinkPattern.UseVisualStyleBackColor = true;
            this.btnLinkPattern.Click += new System.EventHandler(this.btnLinkPattern_Click);
            // 
            // grpBxStep
            // 
            this.grpBxStep.Controls.Add(this.label6);
            this.grpBxStep.Controls.Add(this.btnStep);
            this.grpBxStep.Controls.Add(this.cmbBxStep);
            this.grpBxStep.Location = new System.Drawing.Point(1073, 19);
            this.grpBxStep.Name = "grpBxStep";
            this.grpBxStep.Size = new System.Drawing.Size(120, 142);
            this.grpBxStep.TabIndex = 20;
            this.grpBxStep.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 16);
            this.label6.TabIndex = 2;
            this.label6.Text = "Step Count";
            // 
            // btnStep
            // 
            this.btnStep.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStep.Location = new System.Drawing.Point(7, 110);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(75, 23);
            this.btnStep.TabIndex = 1;
            this.btnStep.Text = "Enter";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // cmbBxStep
            // 
            this.cmbBxStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxStep.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxStep.FormattingEnabled = true;
            this.cmbBxStep.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
            this.cmbBxStep.Location = new System.Drawing.Point(6, 47);
            this.cmbBxStep.Name = "cmbBxStep";
            this.cmbBxStep.Size = new System.Drawing.Size(106, 24);
            this.cmbBxStep.TabIndex = 0;
            // 
            // grpBxSV
            // 
            this.grpBxSV.Controls.Add(this.label7);
            this.grpBxSV.Controls.Add(this.txtSV);
            this.grpBxSV.Controls.Add(this.btnSV);
            this.grpBxSV.Location = new System.Drawing.Point(947, 24);
            this.grpBxSV.Name = "grpBxSV";
            this.grpBxSV.Size = new System.Drawing.Size(120, 142);
            this.grpBxSV.TabIndex = 21;
            this.grpBxSV.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 16);
            this.label7.TabIndex = 2;
            this.label7.Text = "SV";
            // 
            // txtSV
            // 
            this.txtSV.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSV.Location = new System.Drawing.Point(7, 43);
            this.txtSV.Name = "txtSV";
            this.txtSV.Size = new System.Drawing.Size(100, 23);
            this.txtSV.TabIndex = 1;
            this.txtSV.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSV_KeyDown);
            // 
            // btnSV
            // 
            this.btnSV.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSV.Location = new System.Drawing.Point(9, 105);
            this.btnSV.Name = "btnSV";
            this.btnSV.Size = new System.Drawing.Size(75, 23);
            this.btnSV.TabIndex = 0;
            this.btnSV.Text = "Enter";
            this.btnSV.UseVisualStyleBackColor = true;
            this.btnSV.Click += new System.EventHandler(this.btnSV_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(170, 486);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(626, 18);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 23;
            // 
            // grpBxTime
            // 
            this.grpBxTime.Controls.Add(this.numericUpDownMin);
            this.grpBxTime.Controls.Add(this.numericUpDownHr);
            this.grpBxTime.Controls.Add(this.label5);
            this.grpBxTime.Controls.Add(this.label4);
            this.grpBxTime.Controls.Add(this.btnTime);
            this.grpBxTime.Location = new System.Drawing.Point(947, 172);
            this.grpBxTime.Name = "grpBxTime";
            this.grpBxTime.Size = new System.Drawing.Size(120, 142);
            this.grpBxTime.TabIndex = 18;
            this.grpBxTime.TabStop = false;
            // 
            // btnLoopCount
            // 
            this.btnLoopCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoopCount.Location = new System.Drawing.Point(9, 105);
            this.btnLoopCount.Name = "btnLoopCount";
            this.btnLoopCount.Size = new System.Drawing.Size(75, 23);
            this.btnLoopCount.TabIndex = 2;
            this.btnLoopCount.Text = "Enter";
            this.btnLoopCount.UseVisualStyleBackColor = true;
            this.btnLoopCount.Click += new System.EventHandler(this.btnLoopCount_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnBack);
            this.groupBox1.Controls.Add(this.btnEdit);
            this.groupBox1.Controls.Add(this.btnWrite);
            this.groupBox1.Controls.Add(this.btnSaveAs);
            this.groupBox1.Controls.Add(this.btnReadFile);
            this.groupBox1.Controls.Add(this.btnReadSettings);
            this.groupBox1.Controls.Add(this.cmbBxStartPattern);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(134, 468);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Image = ((System.Drawing.Image)(resources.GetObject("btnBack.Image")));
            this.btnBack.Location = new System.Drawing.Point(9, 427);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(101, 33);
            this.btnBack.TabIndex = 7;
            this.btnBack.Text = "Back";
            this.btnBack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Visible = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
            this.btnEdit.Location = new System.Drawing.Point(9, 216);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(119, 34);
            this.btnEdit.TabIndex = 6;
            this.btnEdit.Text = "Edit";
            this.btnEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Visible = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnWrite
            // 
            this.btnWrite.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWrite.Image = ((System.Drawing.Image)(resources.GetObject("btnWrite.Image")));
            this.btnWrite.Location = new System.Drawing.Point(9, 179);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(119, 31);
            this.btnWrite.TabIndex = 5;
            this.btnWrite.Text = "Write";
            this.btnWrite.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveAs.Image")));
            this.btnSaveAs.Location = new System.Drawing.Point(9, 139);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(122, 34);
            this.btnSaveAs.TabIndex = 4;
            this.btnSaveAs.Text = "Save As";
            this.btnSaveAs.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnReadFile
            // 
            this.btnReadFile.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReadFile.Image = ((System.Drawing.Image)(resources.GetObject("btnReadFile.Image")));
            this.btnReadFile.Location = new System.Drawing.Point(9, 102);
            this.btnReadFile.Name = "btnReadFile";
            this.btnReadFile.Size = new System.Drawing.Size(122, 31);
            this.btnReadFile.TabIndex = 3;
            this.btnReadFile.Text = "Read File";
            this.btnReadFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReadFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnReadFile.UseVisualStyleBackColor = true;
            this.btnReadFile.Click += new System.EventHandler(this.btnReadFile_Click);
            // 
            // btnReadSettings
            // 
            this.btnReadSettings.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReadSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnReadSettings.Image")));
            this.btnReadSettings.Location = new System.Drawing.Point(9, 63);
            this.btnReadSettings.Name = "btnReadSettings";
            this.btnReadSettings.Size = new System.Drawing.Size(122, 33);
            this.btnReadSettings.TabIndex = 2;
            this.btnReadSettings.Text = "Read Settings";
            this.btnReadSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReadSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnReadSettings.UseVisualStyleBackColor = true;
            this.btnReadSettings.Click += new System.EventHandler(this.btnReadSettings_Click);
            // 
            // cmbBxStartPattern
            // 
            this.cmbBxStartPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxStartPattern.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxStartPattern.FormattingEnabled = true;
            this.cmbBxStartPattern.Items.AddRange(new object[] {
            "Pattern 0",
            "Pattern 1",
            "Pattern 2",
            "Pattern 3",
            "Pattern 4",
            "Pattern 5",
            "Pattern 6",
            "Pattern 7"});
            this.cmbBxStartPattern.Location = new System.Drawing.Point(9, 33);
            this.cmbBxStartPattern.Name = "cmbBxStartPattern";
            this.cmbBxStartPattern.Size = new System.Drawing.Size(119, 24);
            this.cmbBxStartPattern.TabIndex = 1;
            this.cmbBxStartPattern.Tag = "4767";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start Pattern";
            // 
            // grpBxLoop
            // 
            this.grpBxLoop.Controls.Add(this.btnLoopCount);
            this.grpBxLoop.Controls.Add(this.txtLoopCount);
            this.grpBxLoop.Controls.Add(this.label2);
            this.grpBxLoop.Location = new System.Drawing.Point(947, 320);
            this.grpBxLoop.Name = "grpBxLoop";
            this.grpBxLoop.Size = new System.Drawing.Size(121, 142);
            this.grpBxLoop.TabIndex = 17;
            this.grpBxLoop.TabStop = false;
            // 
            // txtLoopCount
            // 
            this.txtLoopCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLoopCount.Location = new System.Drawing.Point(10, 42);
            this.txtLoopCount.Name = "txtLoopCount";
            this.txtLoopCount.Size = new System.Drawing.Size(100, 23);
            this.txtLoopCount.TabIndex = 1;
            this.txtLoopCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLoopCount_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Loop Count";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Location = new System.Drawing.Point(163, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(639, 468);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.GridColor = System.Drawing.Color.White;
            this.dataGridView1.Location = new System.Drawing.Point(7, 10);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(626, 450);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting_1);
            // 
            // RampSoakForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 509);
            this.Controls.Add(this.lblBottomText);
            this.Controls.Add(this.grpBxLinkPattern);
            this.Controls.Add(this.grpBxStep);
            this.Controls.Add(this.grpBxSV);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.grpBxTime);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpBxLoop);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RampSoakForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RampSoak Programmer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RampSoakForm_FormClosing);
            this.Load += new System.EventHandler(this.RampSoakForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHr)).EndInit();
            this.grpBxLinkPattern.ResumeLayout(false);
            this.grpBxLinkPattern.PerformLayout();
            this.grpBxStep.ResumeLayout(false);
            this.grpBxStep.PerformLayout();
            this.grpBxSV.ResumeLayout(false);
            this.grpBxSV.PerformLayout();
            this.grpBxTime.ResumeLayout(false);
            this.grpBxTime.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpBxLoop.ResumeLayout(false);
            this.grpBxLoop.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBottomText;
        private System.Windows.Forms.NumericUpDown numericUpDownMin;
        private System.Windows.Forms.NumericUpDown numericUpDownHr;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnTime;
        private System.Windows.Forms.GroupBox grpBxLinkPattern;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbBxLinkPattern;
        private System.Windows.Forms.Button btnLinkPattern;
        private System.Windows.Forms.GroupBox grpBxStep;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.ComboBox cmbBxStep;
        private System.Windows.Forms.GroupBox grpBxSV;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSV;
        private System.Windows.Forms.Button btnSV;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox grpBxTime;
        private System.Windows.Forms.Button btnLoopCount;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnReadFile;
        private System.Windows.Forms.Button btnReadSettings;
        private System.Windows.Forms.ComboBox cmbBxStartPattern;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpBxLoop;
        private System.Windows.Forms.TextBox txtLoopCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}