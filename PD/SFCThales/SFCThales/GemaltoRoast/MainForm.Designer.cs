namespace GemaltoRoast
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStation = new System.Windows.Forms.ToolStripMenuItem();
            this.ScanType = new System.Windows.Forms.ToolStripMenuItem();
            this.sERIALNUMBERToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sHIPPINGSNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tiếngViệtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.registerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.manageUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.addMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupScan = new System.Windows.Forms.GroupBox();
            this.txbBakeNo = new System.Windows.Forms.TextBox();
            this.lblBakeNo = new System.Windows.Forms.Label();
            this.ckbNG = new System.Windows.Forms.CheckBox();
            this.lblts = new System.Windows.Forms.Label();
            this.textScan = new System.Windows.Forms.TextBox();
            this.lblScan = new System.Windows.Forms.Label();
            this.groupSNLIST = new System.Windows.Forms.GroupBox();
            this.groupSM = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.textVersion = new System.Windows.Forms.TextBox();
            this.textModel = new System.Windows.Forms.TextBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblModel = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.textCount = new System.Windows.Forms.TextBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.textTrayNo = new System.Windows.Forms.TextBox();
            this.lbltray = new System.Windows.Forms.Label();
            this.groupNG = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.textEC = new System.Windows.Forms.TextBox();
            this.lblEC = new System.Windows.Forms.Label();
            this.textSN = new System.Windows.Forms.TextBox();
            this.lblSN = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStation = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.groupScan.SuspendLayout();
            this.groupSNLIST.SuspendLayout();
            this.groupSM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.groupNG.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.languageToolStripMenuItem,
            this.tsmiHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1066, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiStation,
            this.ScanType,
            this.tsmiExit,
            this.exitToolStripMenuItem});
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.Size = new System.Drawing.Size(37, 20);
            this.tsmiFile.Text = "File";
            // 
            // tsmiStation
            // 
            this.tsmiStation.Name = "tsmiStation";
            this.tsmiStation.Size = new System.Drawing.Size(130, 22);
            this.tsmiStation.Text = "Station";
            this.tsmiStation.Click += new System.EventHandler(this.tsmiStation_Click);
            // 
            // ScanType
            // 
            this.ScanType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sERIALNUMBERToolStripMenuItem,
            this.sHIPPINGSNToolStripMenuItem});
            this.ScanType.Name = "ScanType";
            this.ScanType.Size = new System.Drawing.Size(130, 22);
            this.ScanType.Text = "Scan Type";
            // 
            // sERIALNUMBERToolStripMenuItem
            // 
            this.sERIALNUMBERToolStripMenuItem.Name = "sERIALNUMBERToolStripMenuItem";
            this.sERIALNUMBERToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.sERIALNUMBERToolStripMenuItem.Text = "SERIAL_NUMBER";
            this.sERIALNUMBERToolStripMenuItem.Click += new System.EventHandler(this.sERIALNUMBERToolStripMenuItem_Click);
            // 
            // sHIPPINGSNToolStripMenuItem
            // 
            this.sHIPPINGSNToolStripMenuItem.Name = "sHIPPINGSNToolStripMenuItem";
            this.sHIPPINGSNToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.sHIPPINGSNToolStripMenuItem.Text = "SHIPPING_SN";
            this.sHIPPINGSNToolStripMenuItem.Click += new System.EventHandler(this.sHIPPINGSNToolStripMenuItem_Click);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(130, 22);
            this.tsmiExit.Text = "Tray SetUp";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click_1);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.tiếngViệtToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            this.languageToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.languageToolStripMenuItem.Text = "Language";
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.englishToolStripMenuItem.Text = "English";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // tiếngViệtToolStripMenuItem
            // 
            this.tiếngViệtToolStripMenuItem.Checked = true;
            this.tiếngViệtToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tiếngViệtToolStripMenuItem.Name = "tiếngViệtToolStripMenuItem";
            this.tiếngViệtToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.tiếngViệtToolStripMenuItem.Text = "Tiếng Việt";
            this.tiếngViệtToolStripMenuItem.Click += new System.EventHandler(this.tiếngViệtToolStripMenuItem_Click);
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.Name = "tsmiHelp";
            this.tsmiHelp.Size = new System.Drawing.Size(44, 20);
            this.tsmiHelp.Text = "Help";
            this.tsmiHelp.Click += new System.EventHandler(this.tsmiHelp_Click);
            // 
            // registerToolStripMenuItem
            // 
            this.registerToolStripMenuItem.Enabled = false;
            this.registerToolStripMenuItem.Name = "registerToolStripMenuItem";
            this.registerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.registerToolStripMenuItem.Text = "Register User";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(149, 6);
            // 
            // manageUserToolStripMenuItem
            // 
            this.manageUserToolStripMenuItem.Enabled = false;
            this.manageUserToolStripMenuItem.Name = "manageUserToolStripMenuItem";
            this.manageUserToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.manageUserToolStripMenuItem.Text = "Manage User";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // addMenuToolStripMenuItem
            // 
            this.addMenuToolStripMenuItem.Enabled = false;
            this.addMenuToolStripMenuItem.Name = "addMenuToolStripMenuItem";
            this.addMenuToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addMenuToolStripMenuItem.Text = "Add Menu";
            // 
            // groupScan
            // 
            this.groupScan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupScan.Controls.Add(this.txbBakeNo);
            this.groupScan.Controls.Add(this.lblBakeNo);
            this.groupScan.Controls.Add(this.ckbNG);
            this.groupScan.Controls.Add(this.lblts);
            this.groupScan.Controls.Add(this.textScan);
            this.groupScan.Controls.Add(this.lblScan);
            this.groupScan.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupScan.Location = new System.Drawing.Point(5, 167);
            this.groupScan.Name = "groupScan";
            this.groupScan.Size = new System.Drawing.Size(1061, 153);
            this.groupScan.TabIndex = 5;
            this.groupScan.TabStop = false;
            this.groupScan.Text = "Scan";
            // 
            // txbBakeNo
            // 
            this.txbBakeNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txbBakeNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txbBakeNo.Enabled = false;
            this.txbBakeNo.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txbBakeNo.Location = new System.Drawing.Point(701, 29);
            this.txbBakeNo.Name = "txbBakeNo";
            this.txbBakeNo.Size = new System.Drawing.Size(126, 29);
            this.txbBakeNo.TabIndex = 6;
            // 
            // lblBakeNo
            // 
            this.lblBakeNo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBakeNo.AutoSize = true;
            this.lblBakeNo.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblBakeNo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblBakeNo.Location = new System.Drawing.Point(633, 34);
            this.lblBakeNo.Name = "lblBakeNo";
            this.lblBakeNo.Size = new System.Drawing.Size(70, 21);
            this.lblBakeNo.TabIndex = 5;
            this.lblBakeNo.Text = "BakeNo";
            // 
            // ckbNG
            // 
            this.ckbNG.AutoSize = true;
            this.ckbNG.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ckbNG.Location = new System.Drawing.Point(552, 34);
            this.ckbNG.Name = "ckbNG";
            this.ckbNG.Size = new System.Drawing.Size(54, 25);
            this.ckbNG.TabIndex = 4;
            this.ckbNG.Text = "NG";
            this.ckbNG.UseVisualStyleBackColor = true;
            this.ckbNG.CheckedChanged += new System.EventHandler(this.ckbNG_CheckedChanged);
            // 
            // lblts
            // 
            this.lblts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblts.AutoSize = true;
            this.lblts.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblts.ForeColor = System.Drawing.Color.Brown;
            this.lblts.Location = new System.Drawing.Point(228, 98);
            this.lblts.Name = "lblts";
            this.lblts.Size = new System.Drawing.Size(129, 26);
            this.lblts.TabIndex = 3;
            this.lblts.Text = "lblMessage";
            // 
            // textScan
            // 
            this.textScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.textScan.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textScan.Font = new System.Drawing.Font("PMingLiU", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textScan.Location = new System.Drawing.Point(258, 27);
            this.textScan.Name = "textScan";
            this.textScan.Size = new System.Drawing.Size(254, 33);
            this.textScan.TabIndex = 1;
            this.textScan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textScan_KeyPress);
            // 
            // lblScan
            // 
            this.lblScan.AutoSize = true;
            this.lblScan.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblScan.Location = new System.Drawing.Point(138, 29);
            this.lblScan.Name = "lblScan";
            this.lblScan.Size = new System.Drawing.Size(118, 26);
            this.lblScan.TabIndex = 0;
            this.lblScan.Text = "Scan Data:";
            // 
            // groupSNLIST
            // 
            this.groupSNLIST.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupSNLIST.Controls.Add(this.groupSM);
            this.groupSNLIST.Controls.Add(this.textBox1);
            this.groupSNLIST.Controls.Add(this.label2);
            this.groupSNLIST.Controls.Add(this.dgv);
            this.groupSNLIST.Controls.Add(this.textVersion);
            this.groupSNLIST.Controls.Add(this.textModel);
            this.groupSNLIST.Controls.Add(this.lblVersion);
            this.groupSNLIST.Controls.Add(this.lblModel);
            this.groupSNLIST.Controls.Add(this.btnClose);
            this.groupSNLIST.Controls.Add(this.textCount);
            this.groupSNLIST.Controls.Add(this.lblCount);
            this.groupSNLIST.Controls.Add(this.textTrayNo);
            this.groupSNLIST.Controls.Add(this.lbltray);
            this.groupSNLIST.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupSNLIST.Location = new System.Drawing.Point(12, 327);
            this.groupSNLIST.Name = "groupSNLIST";
            this.groupSNLIST.Size = new System.Drawing.Size(1061, 366);
            this.groupSNLIST.TabIndex = 6;
            this.groupSNLIST.TabStop = false;
            this.groupSNLIST.Text = "SNLIST";
            // 
            // groupSM
            // 
            this.groupSM.Controls.Add(this.label1);
            this.groupSM.Controls.Add(this.label5);
            this.groupSM.Controls.Add(this.label4);
            this.groupSM.Controls.Add(this.label3);
            this.groupSM.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupSM.ForeColor = System.Drawing.Color.Red;
            this.groupSM.Location = new System.Drawing.Point(3, 96);
            this.groupSM.Name = "groupSM";
            this.groupSM.Size = new System.Drawing.Size(1049, 178);
            this.groupSM.TabIndex = 6;
            this.groupSM.TabStop = false;
            this.groupSM.Text = "Program description";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(8, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(603, 26);
            this.label1.TabIndex = 4;
            this.label1.Text = "2. At ROAST_IN station, brush TRAY_NO to pass the station";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(8, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(987, 26);
            this.label5.TabIndex = 3;
            this.label5.Text = "4. The VI station will solve TRAY_NO, and at the same time, single-chip products " +
    "can be defective";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(8, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(713, 26);
            this.label4.TabIndex = 2;
            this.label4.Text = "3. ROAST_OUT and VI stations default to TRAY_NO to pass the station";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(8, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(1141, 26);
            this.label3.TabIndex = 1;
            this.label3.Text = "1. This program is currently only applicable to PACK_TRAY, PACK_TRAYII, ROAST_IN," +
    " ROAST_OUT and VI stations";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox1.ForeColor = System.Drawing.Color.SeaGreen;
            this.textBox1.Location = new System.Drawing.Point(539, 30);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.textBox1.Size = new System.Drawing.Size(50, 29);
            this.textBox1.TabIndex = 18;
            this.textBox1.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(485, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 24);
            this.label2.TabIndex = 17;
            this.label2.Text = "Qty:";
            // 
            // dgv
            // 
            this.dgv.AccessibleRole = System.Windows.Forms.AccessibleRole.ButtonMenu;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgv.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.Location = new System.Drawing.Point(3, 147);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowTemplate.Height = 24;
            this.dgv.Size = new System.Drawing.Size(1051, 212);
            this.dgv.TabIndex = 15;
            // 
            // textVersion
            // 
            this.textVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.textVersion.Enabled = false;
            this.textVersion.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textVersion.Location = new System.Drawing.Point(565, 94);
            this.textVersion.Name = "textVersion";
            this.textVersion.Size = new System.Drawing.Size(174, 29);
            this.textVersion.TabIndex = 14;
            // 
            // textModel
            // 
            this.textModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.textModel.Enabled = false;
            this.textModel.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textModel.Location = new System.Drawing.Point(193, 94);
            this.textModel.Name = "textModel";
            this.textModel.Size = new System.Drawing.Size(174, 29);
            this.textModel.TabIndex = 13;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblVersion.Location = new System.Drawing.Point(401, 96);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(165, 24);
            this.lblVersion.TabIndex = 10;
            this.lblVersion.Text = "VERSION_CODE:";
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblModel.Location = new System.Drawing.Point(36, 96);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(155, 24);
            this.lblModel.TabIndex = 9;
            this.lblModel.Text = "MODEL_NAME:";
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnClose.Location = new System.Drawing.Point(605, 33);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(134, 44);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Close Tray";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // textCount
            // 
            this.textCount.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textCount.ForeColor = System.Drawing.Color.SeaGreen;
            this.textCount.Location = new System.Drawing.Point(405, 30);
            this.textCount.Name = "textCount";
            this.textCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.textCount.Size = new System.Drawing.Size(50, 29);
            this.textCount.TabIndex = 6;
            this.textCount.Text = "0";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCount.Location = new System.Drawing.Point(337, 33);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(71, 24);
            this.lblCount.TabIndex = 4;
            this.lblCount.Text = "Count:";
            // 
            // textTrayNo
            // 
            this.textTrayNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.textTrayNo.Enabled = false;
            this.textTrayNo.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textTrayNo.Location = new System.Drawing.Point(145, 30);
            this.textTrayNo.Name = "textTrayNo";
            this.textTrayNo.Size = new System.Drawing.Size(174, 29);
            this.textTrayNo.TabIndex = 2;
            // 
            // lbltray
            // 
            this.lbltray.AutoSize = true;
            this.lbltray.Font = new System.Drawing.Font("Microsoft JhengHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbltray.Location = new System.Drawing.Point(38, 33);
            this.lbltray.Name = "lbltray";
            this.lbltray.Size = new System.Drawing.Size(106, 24);
            this.lbltray.TabIndex = 1;
            this.lbltray.Text = "TRAY_NO:";
            // 
            // groupNG
            // 
            this.groupNG.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupNG.Controls.Add(this.btnCancel);
            this.groupNG.Controls.Add(this.btnOK);
            this.groupNG.Controls.Add(this.textEC);
            this.groupNG.Controls.Add(this.lblEC);
            this.groupNG.Controls.Add(this.textSN);
            this.groupNG.Controls.Add(this.lblSN);
            this.groupNG.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupNG.Location = new System.Drawing.Point(32, 326);
            this.groupNG.Name = "groupNG";
            this.groupNG.Size = new System.Drawing.Size(873, 178);
            this.groupNG.TabIndex = 16;
            this.groupNG.TabStop = false;
            this.groupNG.Text = "MakeNG";
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCancel.Location = new System.Drawing.Point(618, 95);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 38);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOK.Location = new System.Drawing.Point(618, 33);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(91, 41);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // textEC
            // 
            this.textEC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.textEC.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textEC.Font = new System.Drawing.Font("PMingLiU", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textEC.Location = new System.Drawing.Point(246, 38);
            this.textEC.Name = "textEC";
            this.textEC.Size = new System.Drawing.Size(229, 33);
            this.textEC.TabIndex = 5;
            // 
            // lblEC
            // 
            this.lblEC.AutoSize = true;
            this.lblEC.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblEC.Location = new System.Drawing.Point(108, 41);
            this.lblEC.Name = "lblEC";
            this.lblEC.Size = new System.Drawing.Size(126, 26);
            this.lblEC.TabIndex = 4;
            this.lblEC.Text = "Error Code:";
            // 
            // textSN
            // 
            this.textSN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.textSN.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textSN.Font = new System.Drawing.Font("PMingLiU", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textSN.Location = new System.Drawing.Point(246, 91);
            this.textSN.Name = "textSN";
            this.textSN.Size = new System.Drawing.Size(229, 33);
            this.textSN.TabIndex = 3;
            // 
            // lblSN
            // 
            this.lblSN.AutoSize = true;
            this.lblSN.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSN.Location = new System.Drawing.Point(188, 95);
            this.lblSN.Name = "lblSN";
            this.lblSN.Size = new System.Drawing.Size(46, 26);
            this.lblSN.TabIndex = 2;
            this.lblSN.Text = "SN:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.lblStation);
            this.panel1.Location = new System.Drawing.Point(6, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1054, 122);
            this.panel1.TabIndex = 4;
            // 
            // lblStation
            // 
            this.lblStation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStation.AutoSize = true;
            this.lblStation.BackColor = System.Drawing.Color.Transparent;
            this.lblStation.Font = new System.Drawing.Font("SimSun", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblStation.Location = new System.Drawing.Point(436, 38);
            this.lblStation.Name = "lblStation";
            this.lblStation.Size = new System.Drawing.Size(370, 48);
            this.lblStation.TabIndex = 0;
            this.lblStation.Text = "W103 PACK_TRAY";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1066, 697);
            this.Controls.Add(this.groupSNLIST);
            this.Controls.Add(this.groupScan);
            this.Controls.Add(this.groupNG);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupScan.ResumeLayout(false);
            this.groupScan.PerformLayout();
            this.groupSNLIST.ResumeLayout(false);
            this.groupSNLIST.PerformLayout();
            this.groupSM.ResumeLayout(false);
            this.groupSM.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.groupNG.ResumeLayout(false);
            this.groupNG.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiStation;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelp;
        private System.Windows.Forms.ToolStripMenuItem registerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem manageUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem addMenuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ScanType;
        private System.Windows.Forms.ToolStripMenuItem sERIALNUMBERToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sHIPPINGSNToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label lblStation;
        private System.Windows.Forms.GroupBox groupScan;
        private System.Windows.Forms.TextBox textScan;
        private System.Windows.Forms.Label lblScan;
        private System.Windows.Forms.GroupBox groupSNLIST;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox textCount;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.TextBox textTrayNo;
        private System.Windows.Forms.Label lbltray;
        private System.Windows.Forms.Label lblts;
        private System.Windows.Forms.TextBox textVersion;
        private System.Windows.Forms.TextBox textModel;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.CheckBox ckbNG;
        private System.Windows.Forms.GroupBox groupNG;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox textEC;
        private System.Windows.Forms.Label lblEC;
        private System.Windows.Forms.TextBox textSN;
        private System.Windows.Forms.Label lblSN;
        private System.Windows.Forms.GroupBox groupSM;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txbBakeNo;
        private System.Windows.Forms.Label lblBakeNo;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tiếngViệtToolStripMenuItem;
    }
}