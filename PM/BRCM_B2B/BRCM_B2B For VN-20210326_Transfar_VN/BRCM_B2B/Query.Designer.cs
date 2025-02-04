
namespace BRCM_B2B
{
    partial class Query
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Query));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblShipQty = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblWipCQty = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lblBDSNQty = new System.Windows.Forms.Label();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lblShpCfmQty = new System.Windows.Forms.Label();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.dataGridView5 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxPN = new System.Windows.Forms.ComboBox();
            this.cbxModelSerial = new System.Windows.Forms.ComboBox();
            this.dtTimeTo = new System.Windows.Forms.DateTimePicker();
            this.dtTimeFrom = new System.Windows.Forms.DateTimePicker();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.txtDN = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.queryByDNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queryByDNToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.queryByTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.INVOICE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.finish_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.last_run_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(1, 144);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1295, 512);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblShipQty);
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1287, 486);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "SHIP File";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblShipQty
            // 
            this.lblShipQty.AutoSize = true;
            this.lblShipQty.Location = new System.Drawing.Point(3, 468);
            this.lblShipQty.Name = "lblShipQty";
            this.lblShipQty.Size = new System.Drawing.Size(66, 13);
            this.lblShipQty.TabIndex = 1;
            this.lblShipQty.Text = "Rows total : ";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(2, 1);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(1284, 461);
            this.dataGridView1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lblWipCQty);
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1287, 486);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "WIPC File";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblWipCQty
            // 
            this.lblWipCQty.AutoSize = true;
            this.lblWipCQty.Location = new System.Drawing.Point(6, 468);
            this.lblWipCQty.Name = "lblWipCQty";
            this.lblWipCQty.Size = new System.Drawing.Size(66, 13);
            this.lblWipCQty.TabIndex = 2;
            this.lblWipCQty.Text = "Rows total : ";
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView2.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(1, 2);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.Size = new System.Drawing.Size(1284, 461);
            this.dataGridView2.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.lblBDSNQty);
            this.tabPage3.Controls.Add(this.dataGridView3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1287, 486);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "BDSN File";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // lblBDSNQty
            // 
            this.lblBDSNQty.AutoSize = true;
            this.lblBDSNQty.Location = new System.Drawing.Point(7, 468);
            this.lblBDSNQty.Name = "lblBDSNQty";
            this.lblBDSNQty.Size = new System.Drawing.Size(66, 13);
            this.lblBDSNQty.TabIndex = 2;
            this.lblBDSNQty.Text = "Rows total : ";
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.AllowUserToDeleteRows = false;
            this.dataGridView3.AllowUserToResizeRows = false;
            this.dataGridView3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView3.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Location = new System.Drawing.Point(1, 2);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.ReadOnly = true;
            this.dataGridView3.RowHeadersVisible = false;
            this.dataGridView3.Size = new System.Drawing.Size(1284, 461);
            this.dataGridView3.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.lblShpCfmQty);
            this.tabPage4.Controls.Add(this.dataGridView4);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1287, 486);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "SHIPCFM File";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // lblShpCfmQty
            // 
            this.lblShpCfmQty.AutoSize = true;
            this.lblShpCfmQty.Location = new System.Drawing.Point(6, 468);
            this.lblShpCfmQty.Name = "lblShpCfmQty";
            this.lblShpCfmQty.Size = new System.Drawing.Size(66, 13);
            this.lblShpCfmQty.TabIndex = 2;
            this.lblShpCfmQty.Text = "Rows total : ";
            // 
            // dataGridView4
            // 
            this.dataGridView4.AllowUserToAddRows = false;
            this.dataGridView4.AllowUserToDeleteRows = false;
            this.dataGridView4.AllowUserToResizeRows = false;
            this.dataGridView4.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView4.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView4.Location = new System.Drawing.Point(1, 2);
            this.dataGridView4.Name = "dataGridView4";
            this.dataGridView4.ReadOnly = true;
            this.dataGridView4.RowHeadersVisible = false;
            this.dataGridView4.Size = new System.Drawing.Size(1284, 461);
            this.dataGridView4.TabIndex = 1;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.dataGridView5);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1287, 486);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "List DN error";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // dataGridView5
            // 
            this.dataGridView5.AllowUserToAddRows = false;
            this.dataGridView5.AllowUserToDeleteRows = false;
            this.dataGridView5.AllowUserToResizeColumns = false;
            this.dataGridView5.AllowUserToResizeRows = false;
            this.dataGridView5.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView5.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.INVOICE,
            this.finish_date,
            this.result,
            this.last_run_time});
            this.dataGridView5.Location = new System.Drawing.Point(1, 2);
            this.dataGridView5.Name = "dataGridView5";
            this.dataGridView5.ReadOnly = true;
            this.dataGridView5.RowHeadersVisible = false;
            this.dataGridView5.Size = new System.Drawing.Size(1284, 461);
            this.dataGridView5.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.cbxPN);
            this.groupBox1.Controls.Add(this.cbxModelSerial);
            this.groupBox1.Controls.Add(this.dtTimeTo);
            this.groupBox1.Controls.Add(this.dtTimeFrom);
            this.groupBox1.Controls.Add(this.btnExport);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Controls.Add(this.txtDN);
            this.groupBox1.Location = new System.Drawing.Point(1, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(886, 87);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "by DN : ";
            // 
            // cbxPN
            // 
            this.cbxPN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPN.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxPN.FormattingEnabled = true;
            this.cbxPN.Location = new System.Drawing.Point(67, 52);
            this.cbxPN.Name = "cbxPN";
            this.cbxPN.Size = new System.Drawing.Size(188, 32);
            this.cbxPN.TabIndex = 5;
            // 
            // cbxModelSerial
            // 
            this.cbxModelSerial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxModelSerial.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxModelSerial.FormattingEnabled = true;
            this.cbxModelSerial.Location = new System.Drawing.Point(67, 9);
            this.cbxModelSerial.Name = "cbxModelSerial";
            this.cbxModelSerial.Size = new System.Drawing.Size(188, 32);
            this.cbxModelSerial.TabIndex = 4;
            // 
            // dtTimeTo
            // 
            this.dtTimeTo.Location = new System.Drawing.Point(293, 57);
            this.dtTimeTo.Name = "dtTimeTo";
            this.dtTimeTo.Size = new System.Drawing.Size(200, 20);
            this.dtTimeTo.TabIndex = 3;
            // 
            // dtTimeFrom
            // 
            this.dtTimeFrom.Location = new System.Drawing.Point(293, 14);
            this.dtTimeFrom.Name = "dtTimeFrom";
            this.dtTimeFrom.Size = new System.Drawing.Size(200, 20);
            this.dtTimeFrom.TabIndex = 2;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.DarkOrange;
            this.btnExport.Font = new System.Drawing.Font("VNI-Briquet", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.Location = new System.Drawing.Point(625, 26);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(73, 45);
            this.btnExport.TabIndex = 1;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.BackColor = System.Drawing.Color.DarkOrange;
            this.btnQuery.Font = new System.Drawing.Font("VNI-Briquet", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuery.Location = new System.Drawing.Point(532, 26);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 45);
            this.btnQuery.TabIndex = 1;
            this.btnQuery.Text = "Search";
            this.btnQuery.UseVisualStyleBackColor = false;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // txtDN
            // 
            this.txtDN.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDN.Location = new System.Drawing.Point(39, 32);
            this.txtDN.Name = "txtDN";
            this.txtDN.Size = new System.Drawing.Size(176, 29);
            this.txtDN.TabIndex = 0;
            this.txtDN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDN_KeyPress);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.queryByDNToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1296, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // queryByDNToolStripMenuItem
            // 
            this.queryByDNToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.queryByDNToolStripMenuItem1,
            this.queryByTimeToolStripMenuItem});
            this.queryByDNToolStripMenuItem.Name = "queryByDNToolStripMenuItem";
            this.queryByDNToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.queryByDNToolStripMenuItem.Text = "Option";
            // 
            // queryByDNToolStripMenuItem1
            // 
            this.queryByDNToolStripMenuItem1.Checked = true;
            this.queryByDNToolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.queryByDNToolStripMenuItem1.Name = "queryByDNToolStripMenuItem1";
            this.queryByDNToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.queryByDNToolStripMenuItem1.Text = "Query by DN";
            this.queryByDNToolStripMenuItem1.Click += new System.EventHandler(this.queryByDNToolStripMenuItem1_Click);
            // 
            // queryByTimeToolStripMenuItem
            // 
            this.queryByTimeToolStripMenuItem.Name = "queryByTimeToolStripMenuItem";
            this.queryByTimeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.queryByTimeToolStripMenuItem.Text = "Query by Time";
            this.queryByTimeToolStripMenuItem.Click += new System.EventHandler(this.queryByTimeToolStripMenuItem_Click);
            // 
            // INVOICE
            // 
            this.INVOICE.DataPropertyName = "invoice";
            this.INVOICE.HeaderText = "INVOICE";
            this.INVOICE.Name = "INVOICE";
            this.INVOICE.ReadOnly = true;
            // 
            // finish_date
            // 
            this.finish_date.DataPropertyName = "ship_time";
            this.finish_date.HeaderText = "Shipping Date";
            this.finish_date.Name = "finish_date";
            this.finish_date.ReadOnly = true;
            // 
            // result
            // 
            this.result.DataPropertyName = "result";
            this.result.HeaderText = "Result";
            this.result.Name = "result";
            this.result.ReadOnly = true;
            this.result.Width = 800;
            // 
            // last_run_time
            // 
            this.last_run_time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.last_run_time.DataPropertyName = "last_run_time";
            this.last_run_time.HeaderText = "Last run time";
            this.last_run_time.Name = "last_run_time";
            this.last_run_time.ReadOnly = true;
            // 
            // Query
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::BRCM_B2B.Properties.Resources.background;
            this.ClientSize = new System.Drawing.Size(1296, 656);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Query";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Query";
            this.Load += new System.EventHandler(this.Query_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtDN;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem queryByDNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem queryByDNToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem queryByTimeToolStripMenuItem;
        private System.Windows.Forms.DateTimePicker dtTimeTo;
        private System.Windows.Forms.DateTimePicker dtTimeFrom;
        private System.Windows.Forms.ComboBox cbxModelSerial;
        private System.Windows.Forms.ComboBox cbxPN;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.DataGridView dataGridView4;
        private System.Windows.Forms.DataGridView dataGridView5;
        private System.Windows.Forms.Label lblShipQty;
        private System.Windows.Forms.Label lblWipCQty;
        private System.Windows.Forms.Label lblBDSNQty;
        private System.Windows.Forms.Label lblShpCfmQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn INVOICE;
        private System.Windows.Forms.DataGridViewTextBoxColumn finish_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn result;
        private System.Windows.Forms.DataGridViewTextBoxColumn last_run_time;
    }
}