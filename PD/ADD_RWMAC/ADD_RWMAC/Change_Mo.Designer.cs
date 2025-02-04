namespace ADD_RWMAC
{
    partial class Change_Mo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Change_Mo));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.Button_Move = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Edit_Mo_To = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Edit_Mo_From = new System.Windows.Forms.TextBox();
            this.Model_From = new System.Windows.Forms.Label();
            this.Model_To = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(491, 454);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.MediumTurquoise;
            this.panel2.Controls.Add(this.Model_To);
            this.panel2.Controls.Add(this.Model_From);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.Button_Move);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.Edit_Mo_To);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.Edit_Mo_From);
            this.panel2.Location = new System.Drawing.Point(6, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(478, 440);
            this.panel2.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("VNI-Souvir", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Navy;
            this.label3.Location = new System.Drawing.Point(162, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 29);
            this.label3.TabIndex = 11;
            this.label3.Text = "CHANGE MO";
            // 
            // Button_Move
            // 
            this.Button_Move.BackColor = System.Drawing.Color.AntiqueWhite;
            this.Button_Move.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_Move.ForeColor = System.Drawing.Color.Blue;
            this.Button_Move.Image = ((System.Drawing.Image)(resources.GetObject("Button_Move.Image")));
            this.Button_Move.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Button_Move.Location = new System.Drawing.Point(188, 299);
            this.Button_Move.Name = "Button_Move";
            this.Button_Move.Size = new System.Drawing.Size(82, 39);
            this.Button_Move.TabIndex = 10;
            this.Button_Move.Text = "       MOVE";
            this.Button_Move.UseVisualStyleBackColor = false;
            this.Button_Move.Click += new System.EventHandler(this.Button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(82, 212);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 14);
            this.label1.TabIndex = 9;
            this.label1.Text = "MO TO:";
            // 
            // Edit_Mo_To
            // 
            this.Edit_Mo_To.Location = new System.Drawing.Point(176, 210);
            this.Edit_Mo_To.Name = "Edit_Mo_To";
            this.Edit_Mo_To.Size = new System.Drawing.Size(121, 20);
            this.Edit_Mo_To.TabIndex = 8;
            this.Edit_Mo_To.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Edit_Mo_To_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(82, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 14);
            this.label2.TabIndex = 7;
            this.label2.Text = "MO CHANGE:";
            // 
            // Edit_Mo_From
            // 
            this.Edit_Mo_From.Location = new System.Drawing.Point(176, 143);
            this.Edit_Mo_From.Name = "Edit_Mo_From";
            this.Edit_Mo_From.Size = new System.Drawing.Size(121, 20);
            this.Edit_Mo_From.TabIndex = 6;
            this.Edit_Mo_From.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Edit_Mo_From_KeyDown);
            // 
            // Model_From
            // 
            this.Model_From.AutoSize = true;
            this.Model_From.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Model_From.ForeColor = System.Drawing.Color.Navy;
            this.Model_From.Location = new System.Drawing.Point(315, 146);
            this.Model_From.Name = "Model_From";
            this.Model_From.Size = new System.Drawing.Size(0, 13);
            this.Model_From.TabIndex = 12;
            // 
            // Model_To
            // 
            this.Model_To.AutoSize = true;
            this.Model_To.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Model_To.ForeColor = System.Drawing.Color.Navy;
            this.Model_To.Location = new System.Drawing.Point(315, 213);
            this.Model_To.Name = "Model_To";
            this.Model_To.Size = new System.Drawing.Size(0, 13);
            this.Model_To.TabIndex = 13;
            // 
            // Change_Mo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 452);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(507, 491);
            this.Name = "Change_Mo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CHANGE MO";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Edit_Mo_From;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Edit_Mo_To;
        private System.Windows.Forms.Button Button_Move;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label Model_From;
        private System.Windows.Forms.Label Model_To;
    }
}