namespace FG_CHECK
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnChecIN = new System.Windows.Forms.Button();
            this.btnShipping_Notice = new System.Windows.Forms.Button();
            this.btnShippinglable = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(1, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(682, 334);
            this.panel1.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnChecIN);
            this.panel3.Controls.Add(this.btnShipping_Notice);
            this.panel3.Controls.Add(this.btnShippinglable);
            this.panel3.Controls.Add(this.pictureBox1);
            this.panel3.Location = new System.Drawing.Point(3, 60);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(677, 273);
            this.panel3.TabIndex = 1;
            // 
            // btnChecIN
            // 
            this.btnChecIN.BackColor = System.Drawing.Color.Aqua;
            this.btnChecIN.Font = new System.Drawing.Font("Courier New", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChecIN.Location = new System.Drawing.Point(193, 168);
            this.btnChecIN.Name = "btnChecIN";
            this.btnChecIN.Size = new System.Drawing.Size(280, 46);
            this.btnChecIN.TabIndex = 3;
            this.btnChecIN.Text = "CHECK_IN";
            this.btnChecIN.UseVisualStyleBackColor = false;
            this.btnChecIN.Visible = false;
            this.btnChecIN.Click += new System.EventHandler(this.btnCheckin_Click);
            // 
            // btnShipping_Notice
            // 
            this.btnShipping_Notice.BackColor = System.Drawing.Color.Aqua;
            this.btnShipping_Notice.Font = new System.Drawing.Font("Courier New", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShipping_Notice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnShipping_Notice.Location = new System.Drawing.Point(193, 107);
            this.btnShipping_Notice.Name = "btnShipping_Notice";
            this.btnShipping_Notice.Size = new System.Drawing.Size(280, 46);
            this.btnShipping_Notice.TabIndex = 2;
            this.btnShipping_Notice.Text = "Shipping_Notice";
            this.btnShipping_Notice.UseVisualStyleBackColor = false;
            this.btnShipping_Notice.Visible = false;
            this.btnShipping_Notice.Click += new System.EventHandler(this.btnShipping_Notice_Click);
            // 
            // btnShippinglable
            // 
            this.btnShippinglable.BackColor = System.Drawing.Color.Aqua;
            this.btnShippinglable.Font = new System.Drawing.Font("Courier New", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShippinglable.Location = new System.Drawing.Point(193, 45);
            this.btnShippinglable.Name = "btnShippinglable";
            this.btnShippinglable.Size = new System.Drawing.Size(280, 46);
            this.btnShippinglable.TabIndex = 1;
            this.btnShippinglable.Text = "Shipping Lable";
            this.btnShippinglable.UseVisualStyleBackColor = false;
            this.btnShippinglable.Visible = false;
            this.btnShippinglable.Click += new System.EventHandler(this.btnShippinglable_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.InitialImage = global::FG_CHECK.Properties.Resources.background_powerpoint_cong_nghe_17;
            this.pictureBox1.Location = new System.Drawing.Point(2, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(671, 273);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblVersion);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(2, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(679, 60);
            this.panel2.TabIndex = 0;
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(185)))), ((int)(((byte)(221)))));
            this.lblVersion.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVersion.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblVersion.Location = new System.Drawing.Point(0, 39);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.lblVersion.Size = new System.Drawing.Size(679, 15);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "Version: 1.0.0.0";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(151)))), ((int)(((byte)(205)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Courier New", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(679, 39);
            this.label1.TabIndex = 1;
            this.label1.Text = "SFIS SYSTEM";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 338);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(696, 377);
            this.MinimumSize = new System.Drawing.Size(696, 377);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnChecIN;
        private System.Windows.Forms.Button btnShipping_Notice;
        private System.Windows.Forms.Button btnShippinglable;
        private System.Windows.Forms.Label lblVersion;
    }
}

