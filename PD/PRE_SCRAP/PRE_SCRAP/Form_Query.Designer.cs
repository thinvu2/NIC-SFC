namespace PRE_SCRAP
{
    partial class Form_Query
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Query));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Btn_Back = new System.Windows.Forms.PictureBox();
            this.Btn_Exit = new System.Windows.Forms.PictureBox();
            this.DGView_Query = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Txt_Remark = new System.Windows.Forms.TextBox();
            this.Txt_Scrap_No = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Btn_Back)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Btn_Exit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGView_Query)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SkyBlue;
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(2, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(824, 450);
            this.panel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.PowderBlue;
            this.groupBox2.Controls.Add(this.Btn_Back);
            this.groupBox2.Controls.Add(this.Btn_Exit);
            this.groupBox2.Controls.Add(this.DGView_Query);
            this.groupBox2.Location = new System.Drawing.Point(4, 88);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(817, 356);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            // 
            // Btn_Back
            // 
            this.Btn_Back.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Back.Image = ((System.Drawing.Image)(resources.GetObject("Btn_Back.Image")));
            this.Btn_Back.Location = new System.Drawing.Point(41, 310);
            this.Btn_Back.Name = "Btn_Back";
            this.Btn_Back.Size = new System.Drawing.Size(34, 34);
            this.Btn_Back.TabIndex = 33;
            this.Btn_Back.TabStop = false;
            this.Btn_Back.Click += new System.EventHandler(this.Btn_Back_Click);
            // 
            // Btn_Exit
            // 
            this.Btn_Exit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Btn_Exit.Image = ((System.Drawing.Image)(resources.GetObject("Btn_Exit.Image")));
            this.Btn_Exit.Location = new System.Drawing.Point(734, 310);
            this.Btn_Exit.Name = "Btn_Exit";
            this.Btn_Exit.Size = new System.Drawing.Size(35, 34);
            this.Btn_Exit.TabIndex = 32;
            this.Btn_Exit.TabStop = false;
            this.Btn_Exit.Click += new System.EventHandler(this.Btn_Exit_Click);
            // 
            // DGView_Query
            // 
            this.DGView_Query.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGView_Query.Location = new System.Drawing.Point(7, 21);
            this.DGView_Query.Name = "DGView_Query";
            this.DGView_Query.Size = new System.Drawing.Size(804, 273);
            this.DGView_Query.TabIndex = 27;
            this.DGView_Query.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DGView_Query_CellMouseClick);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.PowderBlue;
            this.groupBox1.Controls.Add(this.Txt_Remark);
            this.groupBox1.Controls.Add(this.Txt_Scrap_No);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Location = new System.Drawing.Point(4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(817, 77);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            // 
            // Txt_Remark
            // 
            this.Txt_Remark.Enabled = false;
            this.Txt_Remark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Txt_Remark.Location = new System.Drawing.Point(561, 15);
            this.Txt_Remark.MinimumSize = new System.Drawing.Size(208, 53);
            this.Txt_Remark.Multiline = true;
            this.Txt_Remark.Name = "Txt_Remark";
            this.Txt_Remark.Size = new System.Drawing.Size(208, 53);
            this.Txt_Remark.TabIndex = 29;
            // 
            // Txt_Scrap_No
            // 
            this.Txt_Scrap_No.Location = new System.Drawing.Point(95, 30);
            this.Txt_Scrap_No.Name = "Txt_Scrap_No";
            this.Txt_Scrap_No.Size = new System.Drawing.Size(99, 20);
            this.Txt_Scrap_No.TabIndex = 26;
            this.Txt_Scrap_No.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_Scrap_No_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(494, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 14);
            this.label1.TabIndex = 28;
            this.label1.Text = "Remark:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label18.Location = new System.Drawing.Point(32, 32);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(57, 14);
            this.label18.TabIndex = 25;
            this.label18.Text = "Scrap_No:";
            // 
            // Form_Query
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 450);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(842, 489);
            this.Name = "Form_Query";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form_Query";
            this.Shown += new System.EventHandler(this.Form_Query_Shown);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Btn_Back)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Btn_Exit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGView_Query)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView DGView_Query;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox Txt_Scrap_No;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.PictureBox Btn_Back;
        private System.Windows.Forms.PictureBox Btn_Exit;
        private System.Windows.Forms.TextBox Txt_Remark;
    }
}