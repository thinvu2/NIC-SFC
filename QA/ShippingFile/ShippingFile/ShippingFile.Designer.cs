namespace ShippingFile
{
    partial class ShipingFile
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShipingFile));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_textEx = new System.Windows.Forms.Label();
            this.lblNnumber = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.btnUpLoad = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtnXML = new System.Windows.Forms.RadioButton();
            this.rbtnCSV = new System.Windows.Forms.RadioButton();
            this.rbtnTXT = new System.Windows.Forms.RadioButton();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDN = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtInvoice = new System.Windows.Forms.TextBox();
            this.lblInvoice = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label_textEx);
            this.groupBox1.Controls.Add(this.lblNnumber);
            this.groupBox1.Controls.Add(this.lblPath);
            this.groupBox1.Controls.Add(this.btnUpLoad);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.btnSelect);
            this.groupBox1.Controls.Add(this.btnExcel);
            this.groupBox1.Controls.Add(this.lblInvoice);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtInvoice);
            this.groupBox1.Controls.Add(this.txtDN);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(928, 94);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scan";
            // 
            // label_textEx
            // 
            this.label_textEx.AutoSize = true;
            this.label_textEx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_textEx.ForeColor = System.Drawing.Color.Red;
            this.label_textEx.Location = new System.Drawing.Point(790, 47);
            this.label_textEx.Name = "label_textEx";
            this.label_textEx.Size = new System.Drawing.Size(95, 17);
            this.label_textEx.TabIndex = 12;
            this.label_textEx.Text = "Please wait....";
            // 
            // lblNnumber
            // 
            this.lblNnumber.AutoSize = true;
            this.lblNnumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblNnumber.ForeColor = System.Drawing.Color.Green;
            this.lblNnumber.Location = new System.Drawing.Point(87, 18);
            this.lblNnumber.Name = "lblNnumber";
            this.lblNnumber.Size = new System.Drawing.Size(0, 15);
            this.lblNnumber.TabIndex = 11;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(506, 20);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(0, 13);
            this.lblPath.TabIndex = 10;
            this.lblPath.Visible = false;
            // 
            // btnUpLoad
            // 
            this.btnUpLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpLoad.Location = new System.Drawing.Point(685, 39);
            this.btnUpLoad.Name = "btnUpLoad";
            this.btnUpLoad.Size = new System.Drawing.Size(75, 34);
            this.btnUpLoad.TabIndex = 9;
            this.btnUpLoad.Text = "Upload";
            this.btnUpLoad.UseVisualStyleBackColor = true;
            this.btnUpLoad.Click += new System.EventHandler(this.btnUpLoad_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtnXML);
            this.panel1.Controls.Add(this.rbtnCSV);
            this.panel1.Controls.Add(this.rbtnTXT);
            this.panel1.Location = new System.Drawing.Point(296, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(180, 55);
            this.panel1.TabIndex = 8;
            // 
            // rbtnXML
            // 
            this.rbtnXML.AutoSize = true;
            this.rbtnXML.Location = new System.Drawing.Point(119, 25);
            this.rbtnXML.Name = "rbtnXML";
            this.rbtnXML.Size = new System.Drawing.Size(47, 17);
            this.rbtnXML.TabIndex = 10;
            this.rbtnXML.Text = "XML";
            this.rbtnXML.UseVisualStyleBackColor = true;
            // 
            // rbtnCSV
            // 
            this.rbtnCSV.AutoSize = true;
            this.rbtnCSV.Location = new System.Drawing.Point(62, 25);
            this.rbtnCSV.Name = "rbtnCSV";
            this.rbtnCSV.Size = new System.Drawing.Size(46, 17);
            this.rbtnCSV.TabIndex = 9;
            this.rbtnCSV.Text = "CSV";
            this.rbtnCSV.UseVisualStyleBackColor = true;
            // 
            // rbtnTXT
            // 
            this.rbtnTXT.AutoSize = true;
            this.rbtnTXT.Checked = true;
            this.rbtnTXT.Location = new System.Drawing.Point(15, 25);
            this.rbtnTXT.Name = "rbtnTXT";
            this.rbtnTXT.Size = new System.Drawing.Size(46, 17);
            this.rbtnTXT.TabIndex = 8;
            this.rbtnTXT.TabStop = true;
            this.rbtnTXT.Text = "TXT";
            this.rbtnTXT.UseVisualStyleBackColor = true;
            // 
            // btnSelect
            // 
            this.btnSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelect.Location = new System.Drawing.Point(509, 39);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 34);
            this.btnSelect.TabIndex = 5;
            this.btnSelect.Text = "Query";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExcel.Location = new System.Drawing.Point(595, 39);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(75, 34);
            this.btnExcel.TabIndex = 4;
            this.btnExcel.Text = "Export File";
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Green;
            this.label3.Location = new System.Drawing.Point(26, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "DN:";
            // 
            // txtDN
            // 
            this.txtDN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtDN.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDN.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtDN.Location = new System.Drawing.Point(86, 14);
            this.txtDN.Name = "txtDN";
            this.txtDN.Size = new System.Drawing.Size(191, 26);
            this.txtDN.TabIndex = 1;
            this.txtDN.TextChanged += new System.EventHandler(this.txtDN_TextChanged);
            this.txtDN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDN_KeyPress);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.Location = new System.Drawing.Point(12, 127);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(928, 473);
            this.dataGridView1.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txtInvoice
            // 
            this.txtInvoice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtInvoice.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtInvoice.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtInvoice.Location = new System.Drawing.Point(86, 49);
            this.txtInvoice.Name = "txtInvoice";
            this.txtInvoice.Size = new System.Drawing.Size(191, 26);
            this.txtInvoice.TabIndex = 1;
            this.txtInvoice.TextChanged += new System.EventHandler(this.txtDN_TextChanged);
            // 
            // lblInvoice
            // 
            this.lblInvoice.AutoSize = true;
            this.lblInvoice.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblInvoice.ForeColor = System.Drawing.Color.Green;
            this.lblInvoice.Location = new System.Drawing.Point(6, 52);
            this.lblInvoice.Name = "lblInvoice";
            this.lblInvoice.Size = new System.Drawing.Size(80, 16);
            this.lblInvoice.TabIndex = 2;
            this.lblInvoice.Text = "Invoice:";
            // 
            // ShipingFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 613);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ShipingFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShipFile";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShipingFile_FormClosed);
            this.Load += new System.EventHandler(this.ShipingFile_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDN;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.RadioButton rbtnCSV;
        private System.Windows.Forms.RadioButton rbtnTXT;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnUpLoad;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Label lblNnumber;
        private System.Windows.Forms.RadioButton rbtnXML;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label_textEx;
        private System.Windows.Forms.Label lblInvoice;
        private System.Windows.Forms.TextBox txtInvoice;
    }
}

