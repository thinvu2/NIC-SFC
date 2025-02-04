using System;

namespace FG_CHECK
{
    partial class fMainInOutRevert
    {
        //[System.Runtime.InteropServices.DllImport("SfcScann.dll")]
        //public static extern int SfcSubclass(IntPtr h);
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fMainInOutRevert));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkInPalletToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revertRMAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revertAddMoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkFQAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tOKITTINGSMTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reworkByOldOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tiếngViệtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbl_icon = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbl_checkin = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lerror = new System.Windows.Forms.TextBox();
            this.lbl_run = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textmo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.DATA = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.COUNT = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btn_recover = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txt_sn = new System.Windows.Forms.TextBox();
            this.lbl_sn = new System.Windows.Forms.Label();
            this.grb_check = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbl_check = new System.Windows.Forms.Label();
            this.rd_imei = new System.Windows.Forms.RadioButton();
            this.rd_carton = new System.Windows.Forms.RadioButton();
            this.rd_sn = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_count = new System.Windows.Forms.TextBox();
            this.txt_model = new System.Windows.Forms.TextBox();
            this.txt_mo = new System.Windows.Forms.TextBox();
            this.txt_emp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ts_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.ts_ip = new System.Windows.Forms.ToolStripStatusLabel();
            this.ts_mac = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.grb_check.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem,
            this.configToolStripMenuItem,
            this.languageToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1306, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkInPalletToolStripMenuItem,
            this.revertRMAToolStripMenuItem,
            this.revertAddMoToolStripMenuItem,
            this.checkFQAToolStripMenuItem,
            this.tOKITTINGSMTToolStripMenuItem});
            this.setupToolStripMenuItem.Font = new System.Drawing.Font(".VnArial Narrow", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setupToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(52, 21);
            this.setupToolStripMenuItem.Text = "Setup";
            // 
            // checkInPalletToolStripMenuItem
            // 
            this.checkInPalletToolStripMenuItem.Name = "checkInPalletToolStripMenuItem";
            this.checkInPalletToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.checkInPalletToolStripMenuItem.Text = "Check Out Pallet";
            this.checkInPalletToolStripMenuItem.Click += new System.EventHandler(this.CheckInPalletToolStripMenuItem_Click);
            // 
            // revertRMAToolStripMenuItem
            // 
            this.revertRMAToolStripMenuItem.Name = "revertRMAToolStripMenuItem";
            this.revertRMAToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.revertRMAToolStripMenuItem.Text = "Revert RMA";
            this.revertRMAToolStripMenuItem.Click += new System.EventHandler(this.revertRMAToolStripMenuItem_Click);
            // 
            // revertAddMoToolStripMenuItem
            // 
            this.revertAddMoToolStripMenuItem.Name = "revertAddMoToolStripMenuItem";
            this.revertAddMoToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.revertAddMoToolStripMenuItem.Text = "RevertAddMo";
            this.revertAddMoToolStripMenuItem.Click += new System.EventHandler(this.revertAddMoToolStripMenuItem_Click);
            // 
            // checkFQAToolStripMenuItem
            // 
            this.checkFQAToolStripMenuItem.Name = "checkFQAToolStripMenuItem";
            this.checkFQAToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.checkFQAToolStripMenuItem.Text = "Check FQA";
            this.checkFQAToolStripMenuItem.Click += new System.EventHandler(this.checkFQAToolStripMenuItem_Click);
            // 
            // tOKITTINGSMTToolStripMenuItem
            // 
            this.tOKITTINGSMTToolStripMenuItem.Name = "tOKITTINGSMTToolStripMenuItem";
            this.tOKITTINGSMTToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.tOKITTINGSMTToolStripMenuItem.Text = "TO-KITTINGSMT";
            this.tOKITTINGSMTToolStripMenuItem.Click += new System.EventHandler(this.tOKITTINGSMTToolStripMenuItem_Click);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reworkByOldOrderToolStripMenuItem});
            this.configToolStripMenuItem.Font = new System.Drawing.Font(".VnArial Narrow", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(56, 21);
            this.configToolStripMenuItem.Text = "Config";
            // 
            // reworkByOldOrderToolStripMenuItem
            // 
            this.reworkByOldOrderToolStripMenuItem.Name = "reworkByOldOrderToolStripMenuItem";
            this.reworkByOldOrderToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.reworkByOldOrderToolStripMenuItem.Text = "Rework by old order";
            this.reworkByOldOrderToolStripMenuItem.Click += new System.EventHandler(this.ReworkByOldOrderToolStripMenuItem_Click);
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.tiếngViệtToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            this.languageToolStripMenuItem.Size = new System.Drawing.Size(71, 21);
            this.languageToolStripMenuItem.Text = "Language";
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Checked = true;
            this.englishToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.englishToolStripMenuItem.Text = "English";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // tiếngViệtToolStripMenuItem
            // 
            this.tiếngViệtToolStripMenuItem.Name = "tiếngViệtToolStripMenuItem";
            this.tiếngViệtToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.tiếngViệtToolStripMenuItem.Text = "Tiếng Việt";
            this.tiếngViệtToolStripMenuItem.Click += new System.EventHandler(this.tiếngViệtToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Font = new System.Drawing.Font(".VnArial Narrow", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(40, 21);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkCyan;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.lbl_icon);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.lbl_checkin);
            this.panel1.Location = new System.Drawing.Point(12, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1291, 114);
            this.panel1.TabIndex = 2;
            // 
            // lbl_icon
            // 
            this.lbl_icon.AutoSize = true;
            this.lbl_icon.Font = new System.Drawing.Font(".VnArial", 33.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_icon.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lbl_icon.Image = ((System.Drawing.Image)(resources.GetObject("lbl_icon.Image")));
            this.lbl_icon.Location = new System.Drawing.Point(376, 17);
            this.lbl_icon.Name = "lbl_icon";
            this.lbl_icon.Size = new System.Drawing.Size(0, 53);
            this.lbl_icon.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font(".VnArial", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(530, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(145, 22);
            this.label7.TabIndex = 6;
            this.label7.Text = "Version: 1.0.0.3";
            // 
            // lbl_checkin
            // 
            this.lbl_checkin.AutoSize = true;
            this.lbl_checkin.Font = new System.Drawing.Font(".VnArial", 33.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_checkin.ForeColor = System.Drawing.Color.White;
            this.lbl_checkin.Location = new System.Drawing.Point(462, 17);
            this.lbl_checkin.Name = "lbl_checkin";
            this.lbl_checkin.Size = new System.Drawing.Size(239, 53);
            this.lbl_checkin.TabIndex = 5;
            this.lbl_checkin.Text = "CHECK IN";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkCyan;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.grb_check);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Location = new System.Drawing.Point(12, 156);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1291, 561);
            this.panel2.TabIndex = 3;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.lerror);
            this.panel4.Controls.Add(this.lbl_run);
            this.panel4.Location = new System.Drawing.Point(16, 492);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1263, 56);
            this.panel4.TabIndex = 3;
            // 
            // lerror
            // 
            this.lerror.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lerror.Location = new System.Drawing.Point(81, 17);
            this.lerror.Name = "lerror";
            this.lerror.Size = new System.Drawing.Size(927, 29);
            this.lerror.TabIndex = 36;
            // 
            // lbl_run
            // 
            this.lbl_run.AutoSize = true;
            this.lbl_run.Font = new System.Drawing.Font(".VnArial Narrow", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_run.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lbl_run.Location = new System.Drawing.Point(49, 18);
            this.lbl_run.Name = "lbl_run";
            this.lbl_run.Size = new System.Drawing.Size(0, 25);
            this.lbl_run.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.DarkCyan;
            this.groupBox3.Controls.Add(this.textmo);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.listView1);
            this.groupBox3.Controls.Add(this.btn_recover);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.txt_sn);
            this.groupBox3.Controls.Add(this.lbl_sn);
            this.groupBox3.Location = new System.Drawing.Point(444, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(835, 459);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            // 
            // textmo
            // 
            this.textmo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textmo.Location = new System.Drawing.Point(199, 17);
            this.textmo.Name = "textmo";
            this.textmo.Size = new System.Drawing.Size(181, 26);
            this.textmo.TabIndex = 15;
            this.textmo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtmo_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(26, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(115, 20);
            this.label5.TabIndex = 13;
            this.label5.Text = "MO_NUMBER";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DATA,
            this.COUNT});
            this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(29, 85);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(782, 352);
            this.listView1.TabIndex = 12;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // DATA
            // 
            this.DATA.Text = "DATA";
            this.DATA.Width = 400;
            // 
            // COUNT
            // 
            this.COUNT.Text = "COUNT";
            this.COUNT.Width = 400;
            // 
            // btn_recover
            // 
            this.btn_recover.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_recover.ForeColor = System.Drawing.Color.Salmon;
            this.btn_recover.Location = new System.Drawing.Point(621, 51);
            this.btn_recover.Name = "btn_recover";
            this.btn_recover.Size = new System.Drawing.Size(152, 26);
            this.btn_recover.TabIndex = 11;
            this.btn_recover.Text = "RECOVER";
            this.btn_recover.UseVisualStyleBackColor = true;
            this.btn_recover.Click += new System.EventHandler(this.Btn_recover_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.Salmon;
            this.button1.Location = new System.Drawing.Point(429, 51);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(151, 26);
            this.button1.TabIndex = 10;
            this.button1.Text = "EXECUTE";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // txt_sn
            // 
            this.txt_sn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_sn.Location = new System.Drawing.Point(199, 51);
            this.txt_sn.Name = "txt_sn";
            this.txt_sn.Size = new System.Drawing.Size(181, 26);
            this.txt_sn.TabIndex = 8;
            this.txt_sn.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Txt_sn_KeyPress);
            // 
            // lbl_sn
            // 
            this.lbl_sn.AutoSize = true;
            this.lbl_sn.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_sn.ForeColor = System.Drawing.Color.White;
            this.lbl_sn.Location = new System.Drawing.Point(25, 54);
            this.lbl_sn.Name = "lbl_sn";
            this.lbl_sn.Size = new System.Drawing.Size(150, 20);
            this.lbl_sn.TabIndex = 4;
            this.lbl_sn.Text = "SERIAL_NUMBER";
            // 
            // grb_check
            // 
            this.grb_check.BackColor = System.Drawing.Color.DarkCyan;
            this.grb_check.Controls.Add(this.panel3);
            this.grb_check.Controls.Add(this.rd_imei);
            this.grb_check.Controls.Add(this.rd_carton);
            this.grb_check.Controls.Add(this.rd_sn);
            this.grb_check.ForeColor = System.Drawing.Color.White;
            this.grb_check.Location = new System.Drawing.Point(16, 297);
            this.grb_check.Name = "grb_check";
            this.grb_check.Size = new System.Drawing.Size(408, 177);
            this.grb_check.TabIndex = 1;
            this.grb_check.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.lbl_check);
            this.panel3.ForeColor = System.Drawing.Color.DarkGray;
            this.panel3.Location = new System.Drawing.Point(199, 19);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(186, 126);
            this.panel3.TabIndex = 3;
            // 
            // lbl_check
            // 
            this.lbl_check.AutoSize = true;
            this.lbl_check.Font = new System.Drawing.Font(".VnArial", 38.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_check.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.lbl_check.Location = new System.Drawing.Point(3, 22);
            this.lbl_check.Name = "lbl_check";
            this.lbl_check.Size = new System.Drawing.Size(0, 60);
            this.lbl_check.TabIndex = 3;
            // 
            // rd_imei
            // 
            this.rd_imei.AutoSize = true;
            this.rd_imei.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rd_imei.ForeColor = System.Drawing.Color.White;
            this.rd_imei.Location = new System.Drawing.Point(6, 121);
            this.rd_imei.Name = "rd_imei";
            this.rd_imei.Size = new System.Drawing.Size(59, 24);
            this.rd_imei.TabIndex = 2;
            this.rd_imei.TabStop = true;
            this.rd_imei.Text = "IMEI";
            this.rd_imei.UseVisualStyleBackColor = true;
            this.rd_imei.CheckedChanged += new System.EventHandler(this.Rd_imei_CheckedChanged);
            // 
            // rd_carton
            // 
            this.rd_carton.AutoSize = true;
            this.rd_carton.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rd_carton.ForeColor = System.Drawing.Color.White;
            this.rd_carton.Location = new System.Drawing.Point(6, 72);
            this.rd_carton.Name = "rd_carton";
            this.rd_carton.Size = new System.Drawing.Size(143, 24);
            this.rd_carton.TabIndex = 1;
            this.rd_carton.TabStop = true;
            this.rd_carton.Text = "MCARTON_NO";
            this.rd_carton.UseVisualStyleBackColor = true;
            this.rd_carton.CheckedChanged += new System.EventHandler(this.Rd_carton_CheckedChanged);
            // 
            // rd_sn
            // 
            this.rd_sn.AutoSize = true;
            this.rd_sn.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rd_sn.ForeColor = System.Drawing.Color.White;
            this.rd_sn.Location = new System.Drawing.Point(6, 19);
            this.rd_sn.Name = "rd_sn";
            this.rd_sn.Size = new System.Drawing.Size(163, 24);
            this.rd_sn.TabIndex = 0;
            this.rd_sn.TabStop = true;
            this.rd_sn.Text = "SHIPPING_SN/SN";
            this.rd_sn.UseVisualStyleBackColor = true;
            this.rd_sn.CheckedChanged += new System.EventHandler(this.Rd_sn_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.DarkCyan;
            this.groupBox1.Controls.Add(this.txt_count);
            this.groupBox1.Controls.Add(this.txt_model);
            this.groupBox1.Controls.Add(this.txt_mo);
            this.groupBox1.Controls.Add(this.txt_emp);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(408, 265);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // txt_count
            // 
            this.txt_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_count.Location = new System.Drawing.Point(175, 209);
            this.txt_count.Name = "txt_count";
            this.txt_count.Size = new System.Drawing.Size(210, 26);
            this.txt_count.TabIndex = 7;
            // 
            // txt_model
            // 
            this.txt_model.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_model.Location = new System.Drawing.Point(175, 149);
            this.txt_model.Name = "txt_model";
            this.txt_model.Size = new System.Drawing.Size(210, 26);
            this.txt_model.TabIndex = 6;
            // 
            // txt_mo
            // 
            this.txt_mo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_mo.Location = new System.Drawing.Point(175, 85);
            this.txt_mo.Name = "txt_mo";
            this.txt_mo.Size = new System.Drawing.Size(210, 26);
            this.txt_mo.TabIndex = 5;
            // 
            // txt_emp
            // 
            this.txt_emp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_emp.Location = new System.Drawing.Point(175, 23);
            this.txt_emp.Name = "txt_emp";
            this.txt_emp.Size = new System.Drawing.Size(210, 26);
            this.txt_emp.TabIndex = 4;
            this.txt_emp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_emp_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(4, 215);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "REWORK COUNT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(6, 155);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "MODEL NAME";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(10, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "MO NUMBER";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "EMPLOYEE";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font(".VnArial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ts_status,
            this.ts_ip,
            this.ts_mac});
            this.statusStrip1.Location = new System.Drawing.Point(0, 724);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1306, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ts_status
            // 
            this.ts_status.ForeColor = System.Drawing.Color.Tomato;
            this.ts_status.Name = "ts_status";
            this.ts_status.Size = new System.Drawing.Size(142, 17);
            this.ts_status.Text = "toolStripStatusLabel1";
            // 
            // ts_ip
            // 
            this.ts_ip.ForeColor = System.Drawing.Color.Tomato;
            this.ts_ip.Name = "ts_ip";
            this.ts_ip.Size = new System.Drawing.Size(142, 17);
            this.ts_ip.Text = "toolStripStatusLabel1";
            // 
            // ts_mac
            // 
            this.ts_mac.ForeColor = System.Drawing.Color.Tomato;
            this.ts_mac.Name = "ts_mac";
            this.ts_mac.Size = new System.Drawing.Size(142, 17);
            this.ts_mac.Text = "toolStripStatusLabel1";
            // 
            // fMainInOutRevert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1306, 746);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "fMainInOutRevert";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CHECK IN";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.fMainInOutRevert_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grb_check.ResumeLayout(false);
            this.grb_check.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkInPalletToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reworkByOldOrderToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label lbl_checkin;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_recover;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txt_sn;
        private System.Windows.Forms.Label lbl_sn;
        private System.Windows.Forms.GroupBox grb_check;
        private System.Windows.Forms.RadioButton rd_imei;
        private System.Windows.Forms.RadioButton rd_carton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_count;
        private System.Windows.Forms.TextBox txt_model;
        private System.Windows.Forms.TextBox txt_mo;
        private System.Windows.Forms.TextBox txt_emp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader DATA;
        private System.Windows.Forms.ColumnHeader COUNT;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lbl_check;
        private System.Windows.Forms.RadioButton rd_sn;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lbl_run;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        public System.Windows.Forms.Label lbl_icon;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel ts_status;
        private System.Windows.Forms.ToolStripStatusLabel ts_ip;
        private System.Windows.Forms.ToolStripStatusLabel ts_mac;
        private System.Windows.Forms.ToolStripMenuItem revertRMAToolStripMenuItem;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripMenuItem revertAddMoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tiếngViệtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkFQAToolStripMenuItem;
        private System.Windows.Forms.TextBox textmo;
        private System.Windows.Forms.TextBox lerror;
        private System.Windows.Forms.ToolStripMenuItem tOKITTINGSMTToolStripMenuItem;
    }
}

