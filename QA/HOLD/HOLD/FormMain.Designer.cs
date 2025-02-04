namespace HOLD
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.btnAutoHold = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnHandHold = new System.Windows.Forms.Button();
            this.btnLogQuery = new System.Windows.Forms.Button();
            this.btnHoldQuery = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changePasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblerror = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status_connectedDb = new System.Windows.Forms.ToolStripStatusLabel();
            this.status_loginInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblversion = new System.Windows.Forms.Label();
            this.tiếngViệtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAutoHold
            // 
            this.btnAutoHold.BackColor = System.Drawing.Color.White;
            this.btnAutoHold.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAutoHold.Image = ((System.Drawing.Image)(resources.GetObject("btnAutoHold.Image")));
            this.btnAutoHold.Location = new System.Drawing.Point(38, 24);
            this.btnAutoHold.Margin = new System.Windows.Forms.Padding(2);
            this.btnAutoHold.Name = "btnAutoHold";
            this.btnAutoHold.Size = new System.Drawing.Size(140, 143);
            this.btnAutoHold.TabIndex = 0;
            this.btnAutoHold.Text = "&Auto Hold";
            this.btnAutoHold.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAutoHold.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnAutoHold.UseVisualStyleBackColor = false;
            this.btnAutoHold.Click += new System.EventHandler(this.btnAutoHold_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnHandHold);
            this.groupBox1.Controls.Add(this.btnLogQuery);
            this.groupBox1.Controls.Add(this.btnHoldQuery);
            this.groupBox1.Controls.Add(this.btnAutoHold);
            this.groupBox1.Location = new System.Drawing.Point(51, 58);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(382, 355);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // btnHandHold
            // 
            this.btnHandHold.BackColor = System.Drawing.Color.White;
            this.btnHandHold.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHandHold.Image = ((System.Drawing.Image)(resources.GetObject("btnHandHold.Image")));
            this.btnHandHold.Location = new System.Drawing.Point(199, 24);
            this.btnHandHold.Margin = new System.Windows.Forms.Padding(2);
            this.btnHandHold.Name = "btnHandHold";
            this.btnHandHold.Size = new System.Drawing.Size(140, 143);
            this.btnHandHold.TabIndex = 3;
            this.btnHandHold.Text = "&Hand Hold";
            this.btnHandHold.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnHandHold.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnHandHold.UseVisualStyleBackColor = false;
            this.btnHandHold.Click += new System.EventHandler(this.btnHandHold_Click);
            // 
            // btnLogQuery
            // 
            this.btnLogQuery.BackColor = System.Drawing.Color.White;
            this.btnLogQuery.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogQuery.Image = ((System.Drawing.Image)(resources.GetObject("btnLogQuery.Image")));
            this.btnLogQuery.Location = new System.Drawing.Point(199, 197);
            this.btnLogQuery.Margin = new System.Windows.Forms.Padding(2);
            this.btnLogQuery.Name = "btnLogQuery";
            this.btnLogQuery.Size = new System.Drawing.Size(140, 143);
            this.btnLogQuery.TabIndex = 2;
            this.btnLogQuery.Text = "&Log Query";
            this.btnLogQuery.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnLogQuery.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnLogQuery.UseVisualStyleBackColor = false;
            this.btnLogQuery.Click += new System.EventHandler(this.btnLogQuery_Click);
            // 
            // btnHoldQuery
            // 
            this.btnHoldQuery.BackColor = System.Drawing.Color.White;
            this.btnHoldQuery.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHoldQuery.Image = ((System.Drawing.Image)(resources.GetObject("btnHoldQuery.Image")));
            this.btnHoldQuery.Location = new System.Drawing.Point(38, 197);
            this.btnHoldQuery.Margin = new System.Windows.Forms.Padding(2);
            this.btnHoldQuery.Name = "btnHoldQuery";
            this.btnHoldQuery.Size = new System.Drawing.Size(140, 143);
            this.btnHoldQuery.TabIndex = 1;
            this.btnHoldQuery.Text = "&Hold Query\r\n";
            this.btnHoldQuery.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnHoldQuery.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnHoldQuery.UseVisualStyleBackColor = false;
            this.btnHoldQuery.Click += new System.EventHandler(this.btnHoldQuery_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Gainsboro;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem,
            this.changePasswordToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(481, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languageToolStripMenuItem});
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.setupToolStripMenuItem.Text = "Config";
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tiếngViệtToolStripMenuItem,
            this.englishToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            this.languageToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.languageToolStripMenuItem.Text = "Language";
            // 
            // changePasswordToolStripMenuItem
            // 
            this.changePasswordToolStripMenuItem.Name = "changePasswordToolStripMenuItem";
            this.changePasswordToolStripMenuItem.Size = new System.Drawing.Size(113, 20);
            this.changePasswordToolStripMenuItem.Text = "Change Password";
            this.changePasswordToolStripMenuItem.Click += new System.EventHandler(this.changePasswordToolStripMenuItem_Click);
            // 
            // lblerror
            // 
            this.lblerror.AutoSize = true;
            this.lblerror.Location = new System.Drawing.Point(20, 32);
            this.lblerror.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblerror.Name = "lblerror";
            this.lblerror.Size = new System.Drawing.Size(38, 13);
            this.lblerror.TabIndex = 4;
            this.lblerror.Text = "lblerror";
            this.lblerror.Visible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status_connectedDb,
            this.status_loginInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 432);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(481, 23);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // status_connectedDb
            // 
            this.status_connectedDb.AutoSize = false;
            this.status_connectedDb.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.status_connectedDb.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_connectedDb.Name = "status_connectedDb";
            this.status_connectedDb.Size = new System.Drawing.Size(160, 18);
            this.status_connectedDb.Text = "status_connectedDb";
            this.status_connectedDb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // status_loginInfo
            // 
            this.status_loginInfo.AutoSize = false;
            this.status_loginInfo.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_loginInfo.Name = "status_loginInfo";
            this.status_loginInfo.Size = new System.Drawing.Size(200, 18);
            this.status_loginInfo.Text = "status_loginInfo";
            this.status_loginInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblversion
            // 
            this.lblversion.AutoSize = true;
            this.lblversion.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblversion.Location = new System.Drawing.Point(412, 415);
            this.lblversion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblversion.Name = "lblversion";
            this.lblversion.Size = new System.Drawing.Size(58, 14);
            this.lblversion.TabIndex = 9;
            this.lblversion.Text = "lblversion";
            this.lblversion.Visible = false;
            // 
            // tiếngViệtToolStripMenuItem
            // 
            this.tiếngViệtToolStripMenuItem.Checked = true;
            this.tiếngViệtToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tiếngViệtToolStripMenuItem.Name = "tiếngViệtToolStripMenuItem";
            this.tiếngViệtToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tiếngViệtToolStripMenuItem.Text = "Tiếng Việt";
            this.tiếngViệtToolStripMenuItem.Click += new System.EventHandler(this.tiếngViệtToolStripMenuItem_Click);
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.englishToolStripMenuItem.Text = "English";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 455);
            this.Controls.Add(this.lblversion);
            this.Controls.Add(this.lblerror);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximumSize = new System.Drawing.Size(497, 494);
            this.MinimumSize = new System.Drawing.Size(497, 494);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HOLD System";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.groupBox1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAutoHold;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnHandHold;
        private System.Windows.Forms.Button btnLogQuery;
        private System.Windows.Forms.Button btnHoldQuery;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changePasswordToolStripMenuItem;
        private System.Windows.Forms.Label lblerror;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status_connectedDb;
        private System.Windows.Forms.Label lblversion;
        private System.Windows.Forms.ToolStripStatusLabel status_loginInfo;
        private System.Windows.Forms.ToolStripMenuItem tiếngViệtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
    }
}

