namespace BRCM_B2B
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.group1 = new System.Windows.Forms.GroupBox();
            this.labelDN = new System.Windows.Forms.Label();
            this.txtDN = new System.Windows.Forms.MaskedTextBox();
            this.btnupload = new System.Windows.Forms.Button();
            this.btnbdsn = new System.Windows.Forms.Button();
            this.btnship = new System.Windows.Forms.Button();
            this.btninvl = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnsndm = new System.Windows.Forms.Button();
            this.btnonhb = new System.Windows.Forms.Button();
            this.btnwipc = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rtbMessage = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.smtpService1 = new BRCM_B2B.WebReference.SmtpService();
            this.button3 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iCConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pNConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getFileAutoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAuto = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.group1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // group1
            // 
            this.group1.BackColor = System.Drawing.Color.Transparent;
            this.group1.Controls.Add(this.labelDN);
            this.group1.Controls.Add(this.txtDN);
            this.group1.Controls.Add(this.btnupload);
            this.group1.Controls.Add(this.btnbdsn);
            this.group1.Controls.Add(this.btnship);
            this.group1.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.group1.ForeColor = System.Drawing.Color.DarkBlue;
            this.group1.Location = new System.Drawing.Point(1, 107);
            this.group1.Name = "group1";
            this.group1.Size = new System.Drawing.Size(489, 121);
            this.group1.TabIndex = 7;
            this.group1.TabStop = false;
            this.group1.Text = "Operation Manual Mode";
            // 
            // labelDN
            // 
            this.labelDN.AutoSize = true;
            this.labelDN.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelDN.Location = new System.Drawing.Point(11, 37);
            this.labelDN.Name = "labelDN";
            this.labelDN.Size = new System.Drawing.Size(39, 21);
            this.labelDN.TabIndex = 13;
            this.labelDN.Text = "DN:";
            // 
            // txtDN
            // 
            this.txtDN.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtDN.Location = new System.Drawing.Point(71, 29);
            this.txtDN.Name = "txtDN";
            this.txtDN.Size = new System.Drawing.Size(228, 29);
            this.txtDN.TabIndex = 12;
            // 
            // btnupload
            // 
            this.btnupload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnupload.Font = new System.Drawing.Font("MS Reference Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnupload.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnupload.Location = new System.Drawing.Point(331, 29);
            this.btnupload.Name = "btnupload";
            this.btnupload.Size = new System.Drawing.Size(99, 77);
            this.btnupload.TabIndex = 10;
            this.btnupload.Text = "Upload";
            this.btnupload.UseVisualStyleBackColor = false;
            this.btnupload.Click += new System.EventHandler(this.button_manualupload_Click);
            // 
            // btnbdsn
            // 
            this.btnbdsn.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btnbdsn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnbdsn.Location = new System.Drawing.Point(199, 64);
            this.btnbdsn.Name = "btnbdsn";
            this.btnbdsn.Size = new System.Drawing.Size(100, 43);
            this.btnbdsn.TabIndex = 9;
            this.btnbdsn.Text = "BDSN";
            this.btnbdsn.UseVisualStyleBackColor = false;
            this.btnbdsn.Click += new System.EventHandler(this.btnbdsn_Click);
            // 
            // btnship
            // 
            this.btnship.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnship.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnship.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnship.Location = new System.Drawing.Point(71, 64);
            this.btnship.Name = "btnship";
            this.btnship.Size = new System.Drawing.Size(100, 43);
            this.btnship.TabIndex = 2;
            this.btnship.Text = "SHIP WIPC";
            this.btnship.UseVisualStyleBackColor = false;
            this.btnship.Click += new System.EventHandler(this.btnship_Click);
            // 
            // btninvl
            // 
            this.btninvl.Enabled = false;
            this.btninvl.Location = new System.Drawing.Point(496, 217);
            this.btninvl.Name = "btninvl";
            this.btninvl.Size = new System.Drawing.Size(84, 56);
            this.btninvl.TabIndex = 8;
            this.btninvl.Text = "INVL";
            this.btninvl.UseVisualStyleBackColor = true;
            this.btninvl.Click += new System.EventHandler(this.btninvl_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(496, 279);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(84, 56);
            this.button2.TabIndex = 7;
            this.button2.Text = "yield";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.Location = new System.Drawing.Point(495, 92);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 56);
            this.button1.TabIndex = 6;
            this.button1.Text = "WEEKLY\r\nONHB";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnsndm
            // 
            this.btnsndm.Enabled = false;
            this.btnsndm.Location = new System.Drawing.Point(495, 154);
            this.btnsndm.Name = "btnsndm";
            this.btnsndm.Size = new System.Drawing.Size(85, 57);
            this.btnsndm.TabIndex = 5;
            this.btnsndm.Text = "SNDM";
            this.btnsndm.UseVisualStyleBackColor = true;
            this.btnsndm.Click += new System.EventHandler(this.btnsndm_Click);
            // 
            // btnonhb
            // 
            this.btnonhb.Enabled = false;
            this.btnonhb.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnonhb.Location = new System.Drawing.Point(496, 29);
            this.btnonhb.Name = "btnonhb";
            this.btnonhb.Size = new System.Drawing.Size(84, 57);
            this.btnonhb.TabIndex = 4;
            this.btnonhb.Text = "DAILY\r\nONHB";
            this.btnonhb.UseVisualStyleBackColor = true;
            this.btnonhb.Click += new System.EventHandler(this.btnonhb_Click);
            // 
            // btnwipc
            // 
            this.btnwipc.Enabled = false;
            this.btnwipc.Location = new System.Drawing.Point(496, 344);
            this.btnwipc.Name = "btnwipc";
            this.btnwipc.Size = new System.Drawing.Size(84, 57);
            this.btnwipc.TabIndex = 3;
            this.btnwipc.Text = "WIPC";
            this.btnwipc.UseVisualStyleBackColor = false;
            this.btnwipc.Click += new System.EventHandler(this.btnwipc_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.rtbMessage);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox2.ForeColor = System.Drawing.Color.Black;
            this.groupBox2.Location = new System.Drawing.Point(1, 239);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(489, 232);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "RunInformation";
            // 
            // rtbMessage
            // 
            this.rtbMessage.BackColor = System.Drawing.SystemColors.Menu;
            this.rtbMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbMessage.Location = new System.Drawing.Point(3, 25);
            this.rtbMessage.Name = "rtbMessage";
            this.rtbMessage.Size = new System.Drawing.Size(483, 204);
            this.rtbMessage.TabIndex = 0;
            this.rtbMessage.Text = "";
            // 
            // timer1
            // 
            this.timer1.Interval = 30000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // smtpService1
            // 
            this.smtpService1.ContainsKeyValue = null;
            this.smtpService1.Credentials = null;
            this.smtpService1.Url = "http://10.132.48.76/SMTP/smtpService.asmx";
            this.smtpService1.UseDefaultCredentials = false;
            // 
            // button3
            // 
            this.button3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button3.Location = new System.Drawing.Point(496, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(63, 29);
            this.button3.TabIndex = 14;
            this.button3.Text = "TEST";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(492, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iCConfigToolStripMenuItem,
            this.pNConfigToolStripMenuItem,
            this.modeToolStripMenuItem,
            this.queryToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // iCConfigToolStripMenuItem
            // 
            this.iCConfigToolStripMenuItem.Name = "iCConfigToolStripMenuItem";
            this.iCConfigToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.iCConfigToolStripMenuItem.Text = "IC Config";
            this.iCConfigToolStripMenuItem.Click += new System.EventHandler(this.iCConfigToolStripMenuItem_Click);
            // 
            // pNConfigToolStripMenuItem
            // 
            this.pNConfigToolStripMenuItem.Name = "pNConfigToolStripMenuItem";
            this.pNConfigToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pNConfigToolStripMenuItem.Text = "PN Config";
            this.pNConfigToolStripMenuItem.Click += new System.EventHandler(this.pNConfigToolStripMenuItem_Click);
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualToolStripMenuItem,
            this.autoToolStripMenuItem,
            this.getFileAutoToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.modeToolStripMenuItem.Text = "Mode";
            // 
            // manualToolStripMenuItem
            // 
            this.manualToolStripMenuItem.Checked = true;
            this.manualToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.manualToolStripMenuItem.Name = "manualToolStripMenuItem";
            this.manualToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.manualToolStripMenuItem.Text = "Manual";
            this.manualToolStripMenuItem.Click += new System.EventHandler(this.manualToolStripMenuItem_Click);
            // 
            // autoToolStripMenuItem
            // 
            this.autoToolStripMenuItem.Name = "autoToolStripMenuItem";
            this.autoToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.autoToolStripMenuItem.Text = "Auto";
            this.autoToolStripMenuItem.Click += new System.EventHandler(this.autoToolStripMenuItem_Click);
            // 
            // getFileAutoToolStripMenuItem
            // 
            this.getFileAutoToolStripMenuItem.Name = "getFileAutoToolStripMenuItem";
            this.getFileAutoToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.getFileAutoToolStripMenuItem.Text = "GetFileAuto";
            this.getFileAutoToolStripMenuItem.Visible = false;
            this.getFileAutoToolStripMenuItem.Click += new System.EventHandler(this.getFileAutoToolStripMenuItem_Click);
            // 
            // queryToolStripMenuItem
            // 
            this.queryToolStripMenuItem.Name = "queryToolStripMenuItem";
            this.queryToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.queryToolStripMenuItem.Text = "Query";
            this.queryToolStripMenuItem.Click += new System.EventHandler(this.queryToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(24, 20);
            this.exitToolStripMenuItem.Text = "x";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft JhengHei", 55F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(56, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(391, 93);
            this.label2.TabIndex = 16;
            this.label2.Text = "BRCM VN";
            // 
            // btnAuto
            // 
            this.btnAuto.Font = new System.Drawing.Font("Microsoft JhengHei", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnAuto.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAuto.Location = new System.Drawing.Point(496, 407);
            this.btnAuto.Name = "btnAuto";
            this.btnAuto.Size = new System.Drawing.Size(84, 43);
            this.btnAuto.TabIndex = 1;
            this.btnAuto.Text = "Auto";
            this.btnAuto.UseVisualStyleBackColor = true;
            this.btnAuto.Click += new System.EventHandler(this.btnAuto_Click);
            // 
            // timer2
            // 
            this.timer2.Interval = 5000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::BRCM_B2B.Properties.Resources.background;
            this.ClientSize = new System.Drawing.Size(492, 473);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btninvl);
            this.Controls.Add(this.btnwipc);
            this.Controls.Add(this.btnsndm);
            this.Controls.Add(this.btnAuto);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnonhb);
            this.Controls.Add(this.group1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(508, 512);
            this.MinimumSize = new System.Drawing.Size(508, 512);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BRCM VN";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox group1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox rtbMessage;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnsndm;
        private System.Windows.Forms.Button btnonhb;
        private System.Windows.Forms.Button btnwipc;
        private System.Windows.Forms.Button btnship;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btninvl;
        private System.Windows.Forms.Button btnbdsn;
        private System.Windows.Forms.Button btnupload;
        private WebReference.SmtpService smtpService1;
        private System.Windows.Forms.Label labelDN;
        private System.Windows.Forms.MaskedTextBox txtDN;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iCConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pNConfigToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoToolStripMenuItem;
        private System.Windows.Forms.Button btnAuto;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.ToolStripMenuItem getFileAutoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem queryToolStripMenuItem;
    }
}

