namespace BRCM_B2B
{
    partial class BRCM_IC_CONFIG
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox_emp = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_chip_department = new System.Windows.Forms.TextBox();
            this.textBox_comp_qty = new System.Windows.Forms.TextBox();
            this.textBox_comp_item = new System.Windows.Forms.TextBox();
            this.textBox_model_name = new System.Windows.Forms.TextBox();
            this.button_query = new System.Windows.Forms.Button();
            this.button_insert = new System.Windows.Forms.Button();
            this.button_modify = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txb_vender_item = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.txb_vender_item);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.textBox_emp);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.textBox_chip_department);
            this.panel1.Controls.Add(this.textBox_comp_qty);
            this.panel1.Controls.Add(this.textBox_comp_item);
            this.panel1.Controls.Add(this.textBox_model_name);
            this.panel1.Controls.Add(this.button_query);
            this.panel1.Controls.Add(this.button_insert);
            this.panel1.Controls.Add(this.button_modify);
            this.panel1.Controls.Add(this.button_delete);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(480, 195);
            this.panel1.TabIndex = 0;
            // 
            // textBox_emp
            // 
            this.textBox_emp.Location = new System.Drawing.Point(146, 163);
            this.textBox_emp.Name = "textBox_emp";
            this.textBox_emp.Size = new System.Drawing.Size(130, 20);
            this.textBox_emp.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(40, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 16);
            this.label5.TabIndex = 12;
            this.label5.Text = "emp no";
            // 
            // textBox_chip_department
            // 
            this.textBox_chip_department.Location = new System.Drawing.Point(146, 132);
            this.textBox_chip_department.Name = "textBox_chip_department";
            this.textBox_chip_department.Size = new System.Drawing.Size(130, 20);
            this.textBox_chip_department.TabIndex = 12;
            // 
            // textBox_comp_qty
            // 
            this.textBox_comp_qty.Location = new System.Drawing.Point(146, 100);
            this.textBox_comp_qty.Name = "textBox_comp_qty";
            this.textBox_comp_qty.Size = new System.Drawing.Size(130, 20);
            this.textBox_comp_qty.TabIndex = 11;
            // 
            // textBox_comp_item
            // 
            this.textBox_comp_item.Location = new System.Drawing.Point(146, 41);
            this.textBox_comp_item.Name = "textBox_comp_item";
            this.textBox_comp_item.Size = new System.Drawing.Size(130, 20);
            this.textBox_comp_item.TabIndex = 9;
            // 
            // textBox_model_name
            // 
            this.textBox_model_name.Location = new System.Drawing.Point(146, 12);
            this.textBox_model_name.Name = "textBox_model_name";
            this.textBox_model_name.Size = new System.Drawing.Size(130, 20);
            this.textBox_model_name.TabIndex = 8;
            // 
            // button_query
            // 
            this.button_query.Location = new System.Drawing.Point(380, 28);
            this.button_query.Name = "button_query";
            this.button_query.Size = new System.Drawing.Size(75, 23);
            this.button_query.TabIndex = 7;
            this.button_query.Text = "Query";
            this.button_query.UseVisualStyleBackColor = true;
            this.button_query.Click += new System.EventHandler(this.button_query_Click);
            // 
            // button_insert
            // 
            this.button_insert.Location = new System.Drawing.Point(380, 56);
            this.button_insert.Name = "button_insert";
            this.button_insert.Size = new System.Drawing.Size(75, 23);
            this.button_insert.TabIndex = 6;
            this.button_insert.Text = "Insert";
            this.button_insert.UseVisualStyleBackColor = true;
            this.button_insert.Click += new System.EventHandler(this.button_insert_Click);
            // 
            // button_modify
            // 
            this.button_modify.Location = new System.Drawing.Point(380, 86);
            this.button_modify.Name = "button_modify";
            this.button_modify.Size = new System.Drawing.Size(75, 23);
            this.button_modify.TabIndex = 5;
            this.button_modify.Text = "Modify";
            this.button_modify.UseVisualStyleBackColor = true;
            this.button_modify.Click += new System.EventHandler(this.button_modify_Click);
            // 
            // button_delete
            // 
            this.button_delete.Location = new System.Drawing.Point(380, 115);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(75, 23);
            this.button_delete.TabIndex = 4;
            this.button_delete.Text = "Delete";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(16, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "chip department";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(32, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "comp qty";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(29, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "comp item";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "model name";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Location = new System.Drawing.Point(13, 213);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(479, 265);
            this.panel2.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(10, 14);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(458, 263);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.Click += new System.EventHandler(this.dataGridView1_Click);
            // 
            // txb_vender_item
            // 
            this.txb_vender_item.Location = new System.Drawing.Point(146, 72);
            this.txb_vender_item.Name = "txb_vender_item";
            this.txb_vender_item.Size = new System.Drawing.Size(130, 20);
            this.txb_vender_item.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(29, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 16);
            this.label6.TabIndex = 14;
            this.label6.Text = "vender item";
            // 
            // BRCM_IC_CONFIG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 481);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BRCM_IC_CONFIG";
            this.Text = "BRCM_CONFIG";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox_chip_department;
        private System.Windows.Forms.TextBox textBox_comp_qty;
        private System.Windows.Forms.TextBox textBox_comp_item;
        private System.Windows.Forms.TextBox textBox_model_name;
        private System.Windows.Forms.Button button_query;
        private System.Windows.Forms.Button button_insert;
        private System.Windows.Forms.Button button_modify;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox_emp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txb_vender_item;
        private System.Windows.Forms.Label label6;
    }
}