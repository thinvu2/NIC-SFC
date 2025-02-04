
namespace PRINT_ALLPART
{
    partial class Check_Label
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
            this.label1 = new System.Windows.Forms.Label();
            this.txt_MoNumber = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPw = new System.Windows.Forms.TextBox();
            this.txtScan = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Employess = new System.Windows.Forms.Label();
            this.txtemp = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Passsword :";
            // 
            // txt_MoNumber
            // 
            this.txt_MoNumber.AutoSize = true;
            this.txt_MoNumber.Location = new System.Drawing.Point(348, 19);
            this.txt_MoNumber.Name = "txt_MoNumber";
            this.txt_MoNumber.Size = new System.Drawing.Size(69, 13);
            this.txt_MoNumber.TabIndex = 0;
            this.txt_MoNumber.Text = "Mo_number :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(44, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Scan Label :";
            // 
            // txtPw
            // 
            this.txtPw.Location = new System.Drawing.Point(117, 16);
            this.txtPw.Name = "txtPw";
            this.txtPw.Size = new System.Drawing.Size(132, 20);
            this.txtPw.TabIndex = 1;
            this.txtPw.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPw_KeyDown);
            this.txtPw.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPw_KeyPress);
            // 
            // txtScan
            // 
            this.txtScan.Enabled = false;
            this.txtScan.Location = new System.Drawing.Point(117, 80);
            this.txtScan.Name = "txtScan";
            this.txtScan.Size = new System.Drawing.Size(132, 20);
            this.txtScan.TabIndex = 1;
            this.txtScan.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtScan_KeyDown);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(1, 126);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(510, 182);
            this.dataGridView1.TabIndex = 1;
            // 
            // Employess
            // 
            this.Employess.AutoSize = true;
            this.Employess.Location = new System.Drawing.Point(47, 51);
            this.Employess.Name = "Employess";
            this.Employess.Size = new System.Drawing.Size(63, 13);
            this.Employess.TabIndex = 3;
            this.Employess.Text = "Employess :";
            // 
            // txtemp
            // 
            this.txtemp.AutoSize = true;
            this.txtemp.Location = new System.Drawing.Point(116, 51);
            this.txtemp.Name = "txtemp";
            this.txtemp.Size = new System.Drawing.Size(35, 13);
            this.txtemp.TabIndex = 3;
            this.txtemp.Text = "label2";
            this.txtemp.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(273, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Mo_number :";
            // 
            // Check_Label
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 308);
            this.Controls.Add(this.txtemp);
            this.Controls.Add(this.Employess);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.txtScan);
            this.Controls.Add(this.txtPw);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_MoNumber);
            this.Controls.Add(this.label1);
            this.Name = "Check_Label";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Check_Label";
            this.Load += new System.EventHandler(this.Check_Label_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label txt_MoNumber;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPw;
        private System.Windows.Forms.TextBox txtScan;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label Employess;
        private System.Windows.Forms.Label txtemp;
        private System.Windows.Forms.Label label2;
    }
}