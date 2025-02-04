namespace MSL_PRINT
{
    partial class MSL_PRINT
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSL_PRINT));
            this.btnClear = new System.Windows.Forms.Button();
            this.lbxOverTime = new System.Windows.Forms.ListBox();
            this.lbxFail = new System.Windows.Forms.ListBox();
            this.lbxPass = new System.Windows.Forms.ListBox();
            this.txbWipGroup = new System.Windows.Forms.TextBox();
            this.txbLastGroup = new System.Windows.Forms.TextBox();
            this.txbLastSection = new System.Windows.Forms.TextBox();
            this.txbLineName = new System.Windows.Forms.TextBox();
            this.txbModelName = new System.Windows.Forms.TextBox();
            this.plTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txbMoNumber = new System.Windows.Forms.TextBox();
            this.txbReelID = new System.Windows.Forms.TextBox();
            this.lblWipGroup = new System.Windows.Forms.Label();
            this.lblLastGroup = new System.Windows.Forms.Label();
            this.lblLastSection = new System.Windows.Forms.Label();
            this.lblLineName = new System.Windows.Forms.Label();
            this.lblModelName = new System.Windows.Forms.Label();
            this.lblMoNumber = new System.Windows.Forms.Label();
            this.lblOverTimeList = new System.Windows.Forms.Label();
            this.lblFail = new System.Windows.Forms.Label();
            this.lblPass = new System.Windows.Forms.Label();
            this.plPrintInfo = new System.Windows.Forms.Panel();
            this.Lab_ReelID = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rePrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.visibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.showVariableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plTitle.SuspendLayout();
            this.plPrintInfo.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.Green;
            this.btnClear.Location = new System.Drawing.Point(658, 24);
            this.btnClear.Margin = new System.Windows.Forms.Padding(2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 35);
            this.btnClear.TabIndex = 21;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lbxOverTime
            // 
            this.lbxOverTime.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lbxOverTime.Font = new System.Drawing.Font("Microsoft YaHei", 11F);
            this.lbxOverTime.FormattingEnabled = true;
            this.lbxOverTime.ItemHeight = 20;
            this.lbxOverTime.Location = new System.Drawing.Point(566, 104);
            this.lbxOverTime.Margin = new System.Windows.Forms.Padding(2);
            this.lbxOverTime.Name = "lbxOverTime";
            this.lbxOverTime.Size = new System.Drawing.Size(226, 184);
            this.lbxOverTime.TabIndex = 20;
            // 
            // lbxFail
            // 
            this.lbxFail.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lbxFail.Font = new System.Drawing.Font("Microsoft YaHei", 11F);
            this.lbxFail.FormattingEnabled = true;
            this.lbxFail.ItemHeight = 20;
            this.lbxFail.Location = new System.Drawing.Point(296, 104);
            this.lbxFail.Margin = new System.Windows.Forms.Padding(2);
            this.lbxFail.Name = "lbxFail";
            this.lbxFail.Size = new System.Drawing.Size(226, 184);
            this.lbxFail.TabIndex = 19;
            // 
            // lbxPass
            // 
            this.lbxPass.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lbxPass.Font = new System.Drawing.Font("Microsoft YaHei", 11F);
            this.lbxPass.FormattingEnabled = true;
            this.lbxPass.ItemHeight = 20;
            this.lbxPass.Location = new System.Drawing.Point(26, 104);
            this.lbxPass.Margin = new System.Windows.Forms.Padding(2);
            this.lbxPass.Name = "lbxPass";
            this.lbxPass.Size = new System.Drawing.Size(226, 184);
            this.lbxPass.TabIndex = 18;
            // 
            // txbWipGroup
            // 
            this.txbWipGroup.BackColor = System.Drawing.Color.Beige;
            this.txbWipGroup.Enabled = false;
            this.txbWipGroup.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbWipGroup.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txbWipGroup.Location = new System.Drawing.Point(600, 410);
            this.txbWipGroup.Margin = new System.Windows.Forms.Padding(2);
            this.txbWipGroup.Name = "txbWipGroup";
            this.txbWipGroup.Size = new System.Drawing.Size(183, 27);
            this.txbWipGroup.TabIndex = 17;
            // 
            // txbLastGroup
            // 
            this.txbLastGroup.BackColor = System.Drawing.Color.Beige;
            this.txbLastGroup.Enabled = false;
            this.txbLastGroup.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbLastGroup.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txbLastGroup.Location = new System.Drawing.Point(600, 368);
            this.txbLastGroup.Margin = new System.Windows.Forms.Padding(2);
            this.txbLastGroup.Name = "txbLastGroup";
            this.txbLastGroup.Size = new System.Drawing.Size(183, 27);
            this.txbLastGroup.TabIndex = 16;
            // 
            // txbLastSection
            // 
            this.txbLastSection.BackColor = System.Drawing.Color.Beige;
            this.txbLastSection.Enabled = false;
            this.txbLastSection.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbLastSection.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txbLastSection.Location = new System.Drawing.Point(600, 327);
            this.txbLastSection.Margin = new System.Windows.Forms.Padding(2);
            this.txbLastSection.Name = "txbLastSection";
            this.txbLastSection.Size = new System.Drawing.Size(183, 27);
            this.txbLastSection.TabIndex = 15;
            // 
            // txbLineName
            // 
            this.txbLineName.BackColor = System.Drawing.Color.Beige;
            this.txbLineName.Enabled = false;
            this.txbLineName.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbLineName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txbLineName.Location = new System.Drawing.Point(150, 410);
            this.txbLineName.Margin = new System.Windows.Forms.Padding(2);
            this.txbLineName.Name = "txbLineName";
            this.txbLineName.Size = new System.Drawing.Size(183, 27);
            this.txbLineName.TabIndex = 14;
            // 
            // txbModelName
            // 
            this.txbModelName.BackColor = System.Drawing.Color.Beige;
            this.txbModelName.Enabled = false;
            this.txbModelName.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbModelName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txbModelName.Location = new System.Drawing.Point(150, 368);
            this.txbModelName.Margin = new System.Windows.Forms.Padding(2);
            this.txbModelName.Name = "txbModelName";
            this.txbModelName.Size = new System.Drawing.Size(183, 27);
            this.txbModelName.TabIndex = 13;
            // 
            // plTitle
            // 
            this.plTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plTitle.Controls.Add(this.lblTitle);
            this.plTitle.Location = new System.Drawing.Point(16, 23);
            this.plTitle.Margin = new System.Windows.Forms.Padding(2);
            this.plTitle.Name = "plTitle";
            this.plTitle.Size = new System.Drawing.Size(814, 95);
            this.plTitle.TabIndex = 7;
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft YaHei", 31.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Blue;
            this.lblTitle.Location = new System.Drawing.Point(218, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(414, 57);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "MSL LABEL PRINT";
            // 
            // txbMoNumber
            // 
            this.txbMoNumber.BackColor = System.Drawing.Color.Beige;
            this.txbMoNumber.Enabled = false;
            this.txbMoNumber.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbMoNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txbMoNumber.Location = new System.Drawing.Point(150, 327);
            this.txbMoNumber.Margin = new System.Windows.Forms.Padding(2);
            this.txbMoNumber.Name = "txbMoNumber";
            this.txbMoNumber.Size = new System.Drawing.Size(183, 27);
            this.txbMoNumber.TabIndex = 12;
            // 
            // txbReelID
            // 
            this.txbReelID.BackColor = System.Drawing.Color.Yellow;
            this.txbReelID.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbReelID.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txbReelID.Location = new System.Drawing.Point(93, 24);
            this.txbReelID.Margin = new System.Windows.Forms.Padding(2);
            this.txbReelID.Name = "txbReelID";
            this.txbReelID.Size = new System.Drawing.Size(253, 29);
            this.txbReelID.TabIndex = 11;
            this.txbReelID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbReelID_KeyPress);
            // 
            // lblWipGroup
            // 
            this.lblWipGroup.AutoSize = true;
            this.lblWipGroup.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWipGroup.ForeColor = System.Drawing.Color.Navy;
            this.lblWipGroup.Location = new System.Drawing.Point(470, 410);
            this.lblWipGroup.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWipGroup.Name = "lblWipGroup";
            this.lblWipGroup.Size = new System.Drawing.Size(114, 22);
            this.lblWipGroup.TabIndex = 9;
            this.lblWipGroup.Text = "WIP_GROUP:";
            // 
            // lblLastGroup
            // 
            this.lblLastGroup.AutoSize = true;
            this.lblLastGroup.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastGroup.ForeColor = System.Drawing.Color.Navy;
            this.lblLastGroup.Location = new System.Drawing.Point(470, 369);
            this.lblLastGroup.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLastGroup.Name = "lblLastGroup";
            this.lblLastGroup.Size = new System.Drawing.Size(122, 22);
            this.lblLastGroup.TabIndex = 8;
            this.lblLastGroup.Text = "LAST_GROUP:";
            // 
            // lblLastSection
            // 
            this.lblLastSection.AutoSize = true;
            this.lblLastSection.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastSection.ForeColor = System.Drawing.Color.Navy;
            this.lblLastSection.Location = new System.Drawing.Point(470, 328);
            this.lblLastSection.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLastSection.Name = "lblLastSection";
            this.lblLastSection.Size = new System.Drawing.Size(135, 22);
            this.lblLastSection.TabIndex = 7;
            this.lblLastSection.Text = "LAST_SECTION:";
            // 
            // lblLineName
            // 
            this.lblLineName.AutoSize = true;
            this.lblLineName.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLineName.ForeColor = System.Drawing.Color.Navy;
            this.lblLineName.Location = new System.Drawing.Point(22, 410);
            this.lblLineName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLineName.Name = "lblLineName";
            this.lblLineName.Size = new System.Drawing.Size(110, 22);
            this.lblLineName.TabIndex = 6;
            this.lblLineName.Text = "LINE_NAME:";
            // 
            // lblModelName
            // 
            this.lblModelName.AutoSize = true;
            this.lblModelName.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModelName.ForeColor = System.Drawing.Color.Navy;
            this.lblModelName.Location = new System.Drawing.Point(22, 369);
            this.lblModelName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblModelName.Name = "lblModelName";
            this.lblModelName.Size = new System.Drawing.Size(133, 22);
            this.lblModelName.TabIndex = 5;
            this.lblModelName.Text = "MODEL_NAME:";
            // 
            // lblMoNumber
            // 
            this.lblMoNumber.AutoSize = true;
            this.lblMoNumber.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMoNumber.ForeColor = System.Drawing.Color.Navy;
            this.lblMoNumber.Location = new System.Drawing.Point(22, 328);
            this.lblMoNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMoNumber.Name = "lblMoNumber";
            this.lblMoNumber.Size = new System.Drawing.Size(124, 22);
            this.lblMoNumber.TabIndex = 4;
            this.lblMoNumber.Text = "MO_NUMBER:";
            // 
            // lblOverTimeList
            // 
            this.lblOverTimeList.AutoSize = true;
            this.lblOverTimeList.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOverTimeList.ForeColor = System.Drawing.Color.Navy;
            this.lblOverTimeList.Location = new System.Drawing.Point(562, 78);
            this.lblOverTimeList.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOverTimeList.Name = "lblOverTimeList";
            this.lblOverTimeList.Size = new System.Drawing.Size(213, 19);
            this.lblOverTimeList.TabIndex = 3;
            this.lblOverTimeList.Text = "List products Overtime control";
            // 
            // lblFail
            // 
            this.lblFail.AutoSize = true;
            this.lblFail.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFail.ForeColor = System.Drawing.Color.Navy;
            this.lblFail.Location = new System.Drawing.Point(296, 78);
            this.lblFail.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFail.Name = "lblFail";
            this.lblFail.Size = new System.Drawing.Size(45, 22);
            this.lblFail.TabIndex = 2;
            this.lblFail.Text = "FAIL";
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPass.ForeColor = System.Drawing.Color.Navy;
            this.lblPass.Location = new System.Drawing.Point(22, 78);
            this.lblPass.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(53, 22);
            this.lblPass.TabIndex = 1;
            this.lblPass.Text = "PASS";
            // 
            // plPrintInfo
            // 
            this.plPrintInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plPrintInfo.Controls.Add(this.btnClear);
            this.plPrintInfo.Controls.Add(this.lbxOverTime);
            this.plPrintInfo.Controls.Add(this.lbxFail);
            this.plPrintInfo.Controls.Add(this.lbxPass);
            this.plPrintInfo.Controls.Add(this.txbWipGroup);
            this.plPrintInfo.Controls.Add(this.txbLastGroup);
            this.plPrintInfo.Controls.Add(this.txbLastSection);
            this.plPrintInfo.Controls.Add(this.txbLineName);
            this.plPrintInfo.Controls.Add(this.txbModelName);
            this.plPrintInfo.Controls.Add(this.txbMoNumber);
            this.plPrintInfo.Controls.Add(this.txbReelID);
            this.plPrintInfo.Controls.Add(this.lblWipGroup);
            this.plPrintInfo.Controls.Add(this.lblLastGroup);
            this.plPrintInfo.Controls.Add(this.lblLastSection);
            this.plPrintInfo.Controls.Add(this.lblLineName);
            this.plPrintInfo.Controls.Add(this.lblModelName);
            this.plPrintInfo.Controls.Add(this.lblMoNumber);
            this.plPrintInfo.Controls.Add(this.lblOverTimeList);
            this.plPrintInfo.Controls.Add(this.lblFail);
            this.plPrintInfo.Controls.Add(this.lblPass);
            this.plPrintInfo.Controls.Add(this.Lab_ReelID);
            this.plPrintInfo.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plPrintInfo.Location = new System.Drawing.Point(16, 123);
            this.plPrintInfo.Margin = new System.Windows.Forms.Padding(2);
            this.plPrintInfo.Name = "plPrintInfo";
            this.plPrintInfo.Size = new System.Drawing.Size(814, 468);
            this.plPrintInfo.TabIndex = 8;
            // 
            // Lab_ReelID
            // 
            this.Lab_ReelID.AutoSize = true;
            this.Lab_ReelID.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lab_ReelID.ForeColor = System.Drawing.Color.Navy;
            this.Lab_ReelID.Location = new System.Drawing.Point(22, 26);
            this.Lab_ReelID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Lab_ReelID.Name = "Lab_ReelID";
            this.Lab_ReelID.Size = new System.Drawing.Size(69, 22);
            this.Lab_ReelID.TabIndex = 0;
            this.Lab_ReelID.Text = "Reel_ID";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(847, 24);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rePrintToolStripMenuItem,
            this.toolStripSeparator1,
            this.visibleToolStripMenuItem,
            this.toolStripSeparator2,
            this.showVariableToolStripMenuItem});
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.printToolStripMenuItem.Text = "Print";
            // 
            // rePrintToolStripMenuItem
            // 
            this.rePrintToolStripMenuItem.Name = "rePrintToolStripMenuItem";
            this.rePrintToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rePrintToolStripMenuItem.Text = "RePrint";
            this.rePrintToolStripMenuItem.Click += new System.EventHandler(this.rePrintToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // visibleToolStripMenuItem
            // 
            this.visibleToolStripMenuItem.Name = "visibleToolStripMenuItem";
            this.visibleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.visibleToolStripMenuItem.Text = "Visible";
            this.visibleToolStripMenuItem.Click += new System.EventHandler(this.visibleToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // showVariableToolStripMenuItem
            // 
            this.showVariableToolStripMenuItem.Name = "showVariableToolStripMenuItem";
            this.showVariableToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.showVariableToolStripMenuItem.Text = "Show Variable";
            this.showVariableToolStripMenuItem.Click += new System.EventHandler(this.showVariableToolStripMenuItem_Click);
            // 
            // MSL_PRINT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(847, 599);
            this.Controls.Add(this.plTitle);
            this.Controls.Add(this.plPrintInfo);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MSL_PRINT";
            this.Text = "MSL PRINT";
            this.Load += new System.EventHandler(this.MSL_PRINT_Load);
            this.plTitle.ResumeLayout(false);
            this.plTitle.PerformLayout();
            this.plPrintInfo.ResumeLayout(false);
            this.plPrintInfo.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ListBox lbxOverTime;
        private System.Windows.Forms.ListBox lbxFail;
        private System.Windows.Forms.ListBox lbxPass;
        private System.Windows.Forms.TextBox txbWipGroup;
        private System.Windows.Forms.TextBox txbLastGroup;
        private System.Windows.Forms.TextBox txbLastSection;
        private System.Windows.Forms.TextBox txbLineName;
        private System.Windows.Forms.TextBox txbModelName;
        private System.Windows.Forms.Panel plTitle;
        public System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txbMoNumber;
        private System.Windows.Forms.TextBox txbReelID;
        private System.Windows.Forms.Label lblWipGroup;
        private System.Windows.Forms.Label lblLastGroup;
        private System.Windows.Forms.Label lblLastSection;
        private System.Windows.Forms.Label lblLineName;
        private System.Windows.Forms.Label lblModelName;
        private System.Windows.Forms.Label lblMoNumber;
        private System.Windows.Forms.Label lblOverTimeList;
        private System.Windows.Forms.Label lblFail;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.Panel plPrintInfo;
        private System.Windows.Forms.Label Lab_ReelID;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rePrintToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem visibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem showVariableToolStripMenuItem;
    }
}

