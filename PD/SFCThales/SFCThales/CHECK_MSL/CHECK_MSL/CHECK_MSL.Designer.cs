namespace CHECK_MSL
{
    partial class CHECK_MSL
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CHECK_MSL));
            this.lblMessage = new System.Windows.Forms.Label();
            this.gbxCheckMSL = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.dgvDataList = new System.Windows.Forms.DataGridView();
            this.txbMslNo = new System.Windows.Forms.TextBox();
            this.lblMslNo = new System.Windows.Forms.Label();
            this.txbReelNo = new System.Windows.Forms.TextBox();
            this.lblReelNo = new System.Windows.Forms.Label();
            this.plTitle = new System.Windows.Forms.Panel();
            this.stationname = new System.Windows.Forms.Label();
            this.Lab_Station = new System.Windows.Forms.Label();
            this.sectionname = new System.Windows.Forms.Label();
            this.linename = new System.Windows.Forms.Label();
            this.groupname = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbxCheckMSL.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataList)).BeginInit();
            this.plTitle.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.Color.Green;
            this.lblMessage.Location = new System.Drawing.Point(500, 85);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(260, 92);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.Text = "Please Input ReelNO";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbxCheckMSL
            // 
            this.gbxCheckMSL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxCheckMSL.Controls.Add(this.btnClear);
            this.gbxCheckMSL.Controls.Add(this.lblMessage);
            this.gbxCheckMSL.Controls.Add(this.dgvDataList);
            this.gbxCheckMSL.Controls.Add(this.txbMslNo);
            this.gbxCheckMSL.Controls.Add(this.lblMslNo);
            this.gbxCheckMSL.Controls.Add(this.txbReelNo);
            this.gbxCheckMSL.Controls.Add(this.lblReelNo);
            this.gbxCheckMSL.Location = new System.Drawing.Point(36, 161);
            this.gbxCheckMSL.Name = "gbxCheckMSL";
            this.gbxCheckMSL.Size = new System.Drawing.Size(1270, 400);
            this.gbxCheckMSL.TabIndex = 8;
            this.gbxCheckMSL.TabStop = false;
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.Green;
            this.btnClear.Location = new System.Drawing.Point(647, 23);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(113, 43);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // dgvDataList
            // 
            this.dgvDataList.AllowUserToAddRows = false;
            this.dgvDataList.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvDataList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDataList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDataList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvDataList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvDataList.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDataList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDataList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("PMingLiU", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDataList.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDataList.Location = new System.Drawing.Point(0, 190);
            this.dgvDataList.Name = "dgvDataList";
            this.dgvDataList.RowHeadersVisible = false;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvDataList.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvDataList.RowTemplate.Height = 24;
            this.dgvDataList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDataList.Size = new System.Drawing.Size(1270, 204);
            this.dgvDataList.TabIndex = 4;
            // 
            // txbMslNo
            // 
            this.txbMslNo.BackColor = System.Drawing.Color.Yellow;
            this.txbMslNo.Enabled = false;
            this.txbMslNo.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbMslNo.Location = new System.Drawing.Point(218, 143);
            this.txbMslNo.Multiline = true;
            this.txbMslNo.Name = "txbMslNo";
            this.txbMslNo.Size = new System.Drawing.Size(257, 32);
            this.txbMslNo.TabIndex = 3;
            this.txbMslNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbMslNo_KeyPress);
            // 
            // lblMslNo
            // 
            this.lblMslNo.AutoSize = true;
            this.lblMslNo.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMslNo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMslNo.Location = new System.Drawing.Point(136, 148);
            this.lblMslNo.Name = "lblMslNo";
            this.lblMslNo.Size = new System.Drawing.Size(82, 22);
            this.lblMslNo.TabIndex = 2;
            this.lblMslNo.Text = "MSL NO:";
            // 
            // txbReelNo
            // 
            this.txbReelNo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txbReelNo.BackColor = System.Drawing.Color.Yellow;
            this.txbReelNo.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbReelNo.Location = new System.Drawing.Point(218, 86);
            this.txbReelNo.Name = "txbReelNo";
            this.txbReelNo.Size = new System.Drawing.Size(257, 27);
            this.txbReelNo.TabIndex = 1;
            this.txbReelNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbReelNo_KeyPress);
            // 
            // lblReelNo
            // 
            this.lblReelNo.AutoSize = true;
            this.lblReelNo.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReelNo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblReelNo.Location = new System.Drawing.Point(136, 88);
            this.lblReelNo.Name = "lblReelNo";
            this.lblReelNo.Size = new System.Drawing.Size(85, 22);
            this.lblReelNo.TabIndex = 0;
            this.lblReelNo.Text = "REEL NO:";
            // 
            // plTitle
            // 
            this.plTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plTitle.BackColor = System.Drawing.Color.LightPink;
            this.plTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plTitle.Controls.Add(this.stationname);
            this.plTitle.Controls.Add(this.Lab_Station);
            this.plTitle.Controls.Add(this.sectionname);
            this.plTitle.Controls.Add(this.linename);
            this.plTitle.Controls.Add(this.groupname);
            this.plTitle.Controls.Add(this.menuStrip1);
            this.plTitle.Location = new System.Drawing.Point(36, 34);
            this.plTitle.Name = "plTitle";
            this.plTitle.Size = new System.Drawing.Size(1270, 121);
            this.plTitle.TabIndex = 7;
            // 
            // stationname
            // 
            this.stationname.AutoSize = true;
            this.stationname.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.stationname.ForeColor = System.Drawing.Color.Green;
            this.stationname.Location = new System.Drawing.Point(14, 92);
            this.stationname.Name = "stationname";
            this.stationname.Size = new System.Drawing.Size(85, 17);
            this.stationname.TabIndex = 96;
            this.stationname.Text = "stationname";
            this.stationname.Visible = false;
            // 
            // Lab_Station
            // 
            this.Lab_Station.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Lab_Station.AutoSize = true;
            this.Lab_Station.BackColor = System.Drawing.Color.Transparent;
            this.Lab_Station.Font = new System.Drawing.Font("Arial", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lab_Station.Location = new System.Drawing.Point(478, 39);
            this.Lab_Station.Name = "Lab_Station";
            this.Lab_Station.Size = new System.Drawing.Size(370, 46);
            this.Lab_Station.TabIndex = 0;
            this.Lab_Station.Text = "L101 CHECK_MSL";
            // 
            // sectionname
            // 
            this.sectionname.AutoSize = true;
            this.sectionname.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.sectionname.ForeColor = System.Drawing.Color.Green;
            this.sectionname.Location = new System.Drawing.Point(14, 73);
            this.sectionname.Name = "sectionname";
            this.sectionname.Size = new System.Drawing.Size(86, 17);
            this.sectionname.TabIndex = 95;
            this.sectionname.Text = "sectionname";
            this.sectionname.Visible = false;
            // 
            // linename
            // 
            this.linename.AutoSize = true;
            this.linename.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.linename.ForeColor = System.Drawing.Color.Green;
            this.linename.Location = new System.Drawing.Point(14, 34);
            this.linename.Name = "linename";
            this.linename.Size = new System.Drawing.Size(65, 17);
            this.linename.TabIndex = 93;
            this.linename.Text = "linename";
            this.linename.Visible = false;
            // 
            // groupname
            // 
            this.groupname.AutoSize = true;
            this.groupname.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupname.ForeColor = System.Drawing.Color.Green;
            this.groupname.Location = new System.Drawing.Point(14, 53);
            this.groupname.Name = "groupname";
            this.groupname.Size = new System.Drawing.Size(79, 17);
            this.groupname.TabIndex = 94;
            this.groupname.Text = "groupname";
            this.groupname.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1270, 24);
            this.menuStrip1.TabIndex = 97;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuStrip2
            // 
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(1354, 24);
            this.menuStrip2.TabIndex = 9;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeLineToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // changeLineToolStripMenuItem
            // 
            this.changeLineToolStripMenuItem.Name = "changeLineToolStripMenuItem";
            this.changeLineToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.changeLineToolStripMenuItem.Text = "Change Line";
            this.changeLineToolStripMenuItem.Click += new System.EventHandler(this.changeLineToolStripMenuItem_Click);
            // 
            // CHECK_MSL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1354, 573);
            this.Controls.Add(this.gbxCheckMSL);
            this.Controls.Add(this.plTitle);
            this.Controls.Add(this.menuStrip2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CHECK_MSL";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.CHECK_MSL_Load);
            this.gbxCheckMSL.ResumeLayout(false);
            this.gbxCheckMSL.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataList)).EndInit();
            this.plTitle.ResumeLayout(false);
            this.plTitle.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.GroupBox gbxCheckMSL;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.DataGridView dgvDataList;
        private System.Windows.Forms.TextBox txbMslNo;
        private System.Windows.Forms.Label lblMslNo;
        private System.Windows.Forms.TextBox txbReelNo;
        private System.Windows.Forms.Label lblReelNo;
        private System.Windows.Forms.Panel plTitle;
        private System.Windows.Forms.Label stationname;
        public System.Windows.Forms.Label Lab_Station;
        private System.Windows.Forms.Label sectionname;
        private System.Windows.Forms.Label linename;
        private System.Windows.Forms.Label groupname;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeLineToolStripMenuItem;
    }
}

