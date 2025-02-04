namespace ADD_RWMAC
{
    partial class Move_Range
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Move_Range));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Button_Reset = new System.Windows.Forms.Button();
            this.Button_Move = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Edit_Mo = new System.Windows.Forms.TextBox();
            this.Edit_SN_End = new System.Windows.Forms.TextBox();
            this.Edit_SN_Begin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(1, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(721, 542);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.MediumTurquoise;
            this.panel2.Controls.Add(this.Button_Reset);
            this.panel2.Controls.Add(this.Button_Move);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.Edit_Mo);
            this.panel2.Controls.Add(this.Edit_SN_End);
            this.panel2.Controls.Add(this.Edit_SN_Begin);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Location = new System.Drawing.Point(10, 10);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(701, 522);
            this.panel2.TabIndex = 0;
            // 
            // Button_Reset
            // 
            this.Button_Reset.Image = ((System.Drawing.Image)(resources.GetObject("Button_Reset.Image")));
            this.Button_Reset.Location = new System.Drawing.Point(508, 117);
            this.Button_Reset.Name = "Button_Reset";
            this.Button_Reset.Size = new System.Drawing.Size(48, 36);
            this.Button_Reset.TabIndex = 9;
            this.Button_Reset.UseVisualStyleBackColor = true;
            this.Button_Reset.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Button_Move
            // 
            this.Button_Move.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_Move.ForeColor = System.Drawing.Color.Blue;
            this.Button_Move.Image = ((System.Drawing.Image)(resources.GetObject("Button_Move.Image")));
            this.Button_Move.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Button_Move.Location = new System.Drawing.Point(295, 221);
            this.Button_Move.Name = "Button_Move";
            this.Button_Move.Size = new System.Drawing.Size(111, 37);
            this.Button_Move.TabIndex = 8;
            this.Button_Move.Text = "        Move Range";
            this.Button_Move.UseVisualStyleBackColor = true;
            this.Button_Move.Click += new System.EventHandler(this.Button_Move_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(246, 178);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 14);
            this.label4.TabIndex = 7;
            this.label4.Text = "New MO:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(246, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 14);
            this.label3.TabIndex = 6;
            this.label3.Text = "End SN:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(248, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 14);
            this.label2.TabIndex = 5;
            this.label2.Text = "Begin SN:";
            // 
            // Edit_Mo
            // 
            this.Edit_Mo.Location = new System.Drawing.Point(317, 172);
            this.Edit_Mo.Name = "Edit_Mo";
            this.Edit_Mo.Size = new System.Drawing.Size(125, 20);
            this.Edit_Mo.TabIndex = 4;
            this.Edit_Mo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Edit_Mo_KeyDown);
            // 
            // Edit_SN_End
            // 
            this.Edit_SN_End.Location = new System.Drawing.Point(317, 126);
            this.Edit_SN_End.Name = "Edit_SN_End";
            this.Edit_SN_End.Size = new System.Drawing.Size(125, 20);
            this.Edit_SN_End.TabIndex = 3;
            this.Edit_SN_End.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Edit_SN_End_KeyDown);
            // 
            // Edit_SN_Begin
            // 
            this.Edit_SN_Begin.Location = new System.Drawing.Point(317, 78);
            this.Edit_SN_Begin.Name = "Edit_SN_Begin";
            this.Edit_SN_Begin.Size = new System.Drawing.Size(125, 20);
            this.Edit_SN_Begin.TabIndex = 2;
            this.Edit_SN_Begin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Edit_SN_Begin_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(248, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(195, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "MOVE RANGE MO";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.PowderBlue;
            this.panel3.Controls.Add(this.dataGridView1);
            this.panel3.Location = new System.Drawing.Point(6, 277);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(690, 238);
            this.panel3.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(684, 232);
            this.dataGridView1.TabIndex = 0;
            // 
            // Move_Range
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 546);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(740, 585);
            this.Name = "Move_Range";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MOVE RANGE";
            this.Shown += new System.EventHandler(this.Move_Range_Shown);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox Edit_Mo;
        private System.Windows.Forms.TextBox Edit_SN_End;
        private System.Windows.Forms.TextBox Edit_SN_Begin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Button_Move;
        private System.Windows.Forms.Button Button_Reset;
    }
}