namespace FG_CHECK
{
    partial class fDetail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fDetail));
            this.panel1 = new System.Windows.Forms.Panel();
            this.dtgDetail = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.ListInvoice = new System.Windows.Forms.ListBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblShipNo = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rdSn = new System.Windows.Forms.RadioButton();
            this.rdTray = new System.Windows.Forms.RadioButton();
            this.rdCarton = new System.Windows.Forms.RadioButton();
            this.rdPallet = new System.Windows.Forms.RadioButton();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txbData = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtgDetail)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dtgDetail);
            this.panel1.Location = new System.Drawing.Point(-1, 181);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(882, 431);
            this.panel1.TabIndex = 0;
            // 
            // dtgDetail
            // 
            this.dtgDetail.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dtgDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgDetail.Location = new System.Drawing.Point(4, 3);
            this.dtgDetail.Name = "dtgDetail";
            this.dtgDetail.Size = new System.Drawing.Size(876, 425);
            this.dtgDetail.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnExit);
            this.panel2.Controls.Add(this.btnExcel);
            this.panel2.Controls.Add(this.btnQuery);
            this.panel2.Controls.Add(this.ListInvoice);
            this.panel2.Controls.Add(this.lblCount);
            this.panel2.Controls.Add(this.lblShipNo);
            this.panel2.Location = new System.Drawing.Point(12, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(401, 136);
            this.panel2.TabIndex = 1;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(277, 98);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 28);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "&Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.AutoSize = true;
            this.btnExcel.Location = new System.Drawing.Point(277, 58);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(75, 28);
            this.btnExcel.TabIndex = 4;
            this.btnExcel.Text = "&Excel";
            this.btnExcel.UseVisualStyleBackColor = true;
            // 
            // btnQuery
            // 
            this.btnQuery.AutoSize = true;
            this.btnQuery.Location = new System.Drawing.Point(277, 14);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 28);
            this.btnQuery.TabIndex = 3;
            this.btnQuery.Text = "&Query";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // ListInvoice
            // 
            this.ListInvoice.FormattingEnabled = true;
            this.ListInvoice.ItemHeight = 18;
            this.ListInvoice.Location = new System.Drawing.Point(106, 14);
            this.ListInvoice.Name = "ListInvoice";
            this.ListInvoice.Size = new System.Drawing.Size(120, 112);
            this.ListInvoice.TabIndex = 2;
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(14, 79);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(41, 18);
            this.lblCount.TabIndex = 1;
            this.lblCount.Text = "Total";
            // 
            // lblShipNo
            // 
            this.lblShipNo.AutoSize = true;
            this.lblShipNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblShipNo.Location = new System.Drawing.Point(14, 27);
            this.lblShipNo.Name = "lblShipNo";
            this.lblShipNo.Size = new System.Drawing.Size(76, 18);
            this.lblShipNo.TabIndex = 0;
            this.lblShipNo.Text = "lbShipNo";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rdSn);
            this.panel3.Controls.Add(this.rdTray);
            this.panel3.Controls.Add(this.rdCarton);
            this.panel3.Controls.Add(this.rdPallet);
            this.panel3.Location = new System.Drawing.Point(488, 28);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(325, 55);
            this.panel3.TabIndex = 2;
            // 
            // rdSn
            // 
            this.rdSn.AutoSize = true;
            this.rdSn.Location = new System.Drawing.Point(256, 14);
            this.rdSn.Name = "rdSn";
            this.rdSn.Size = new System.Drawing.Size(51, 22);
            this.rdSn.TabIndex = 3;
            this.rdSn.Text = "S/N";
            this.rdSn.UseVisualStyleBackColor = true;
            // 
            // rdTray
            // 
            this.rdTray.AutoSize = true;
            this.rdTray.Location = new System.Drawing.Point(175, 14);
            this.rdTray.Name = "rdTray";
            this.rdTray.Size = new System.Drawing.Size(55, 22);
            this.rdTray.TabIndex = 2;
            this.rdTray.Text = "Tray";
            this.rdTray.UseVisualStyleBackColor = true;
            // 
            // rdCarton
            // 
            this.rdCarton.AutoSize = true;
            this.rdCarton.Checked = true;
            this.rdCarton.Location = new System.Drawing.Point(98, 14);
            this.rdCarton.Name = "rdCarton";
            this.rdCarton.Size = new System.Drawing.Size(71, 22);
            this.rdCarton.TabIndex = 1;
            this.rdCarton.TabStop = true;
            this.rdCarton.Text = "Carton";
            this.rdCarton.UseVisualStyleBackColor = true;
            // 
            // rdPallet
            // 
            this.rdPallet.AutoSize = true;
            this.rdPallet.Location = new System.Drawing.Point(13, 14);
            this.rdPallet.Name = "rdPallet";
            this.rdPallet.Size = new System.Drawing.Size(62, 22);
            this.rdPallet.TabIndex = 0;
            this.rdPallet.Text = "Pallet";
            this.rdPallet.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.txbData);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Location = new System.Drawing.Point(460, 84);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(353, 48);
            this.panel4.TabIndex = 3;
            // 
            // txbData
            // 
            this.txbData.Location = new System.Drawing.Point(126, 9);
            this.txbData.Name = "txbData";
            this.txbData.Size = new System.Drawing.Size(160, 24);
            this.txbData.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "Get Invoice by ";
            // 
            // fDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 614);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "fDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "fDetail";
            this.Shown += new System.EventHandler(this.fDetail_Shown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtgDetail)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label lblShipNo;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.ListBox ListInvoice;
        private System.Windows.Forms.RadioButton rdSn;
        private System.Windows.Forms.RadioButton rdTray;
        private System.Windows.Forms.RadioButton rdCarton;
        private System.Windows.Forms.RadioButton rdPallet;
        private System.Windows.Forms.TextBox txbData;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dtgDetail;
    }
}