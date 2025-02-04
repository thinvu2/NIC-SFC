namespace BRCM_B2B
{
    partial class BRCM_PN_CONFIG
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
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_box_code = new System.Windows.Forms.TextBox();
            this.textBox_box_weight = new System.Windows.Forms.TextBox();
            this.textBox_sdversion = new System.Windows.Forms.TextBox();
            this.textBox_dep_code = new System.Windows.Forms.TextBox();
            this.textBox_boradcom_pn = new System.Windows.Forms.TextBox();
            this.textBox_model_name = new System.Windows.Forms.TextBox();
            this.buttonQuery = new System.Windows.Forms.Button();
            this.button_insert = new System.Windows.Forms.Button();
            this.button_modify = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label_model_name = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.textBox_box_code);
            this.panel1.Controls.Add(this.textBox_box_weight);
            this.panel1.Controls.Add(this.textBox_sdversion);
            this.panel1.Controls.Add(this.textBox_dep_code);
            this.panel1.Controls.Add(this.textBox_boradcom_pn);
            this.panel1.Controls.Add(this.textBox_model_name);
            this.panel1.Controls.Add(this.buttonQuery);
            this.panel1.Controls.Add(this.button_insert);
            this.panel1.Controls.Add(this.button_modify);
            this.panel1.Controls.Add(this.button_delete);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label_model_name);
            this.panel1.Location = new System.Drawing.Point(13, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(479, 186);
            this.panel1.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(34, 160);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "box weight";
            // 
            // textBox_box_code
            // 
            this.textBox_box_code.Location = new System.Drawing.Point(180, 126);
            this.textBox_box_code.Name = "textBox_box_code";
            this.textBox_box_code.Size = new System.Drawing.Size(130, 20);
            this.textBox_box_code.TabIndex = 14;
            // 
            // textBox_box_weight
            // 
            this.textBox_box_weight.Location = new System.Drawing.Point(180, 157);
            this.textBox_box_weight.Name = "textBox_box_weight";
            this.textBox_box_weight.Size = new System.Drawing.Size(130, 20);
            this.textBox_box_weight.TabIndex = 13;
            // 
            // textBox_sdversion
            // 
            this.textBox_sdversion.Location = new System.Drawing.Point(180, 97);
            this.textBox_sdversion.Name = "textBox_sdversion";
            this.textBox_sdversion.Size = new System.Drawing.Size(130, 20);
            this.textBox_sdversion.TabIndex = 12;
            // 
            // textBox_dep_code
            // 
            this.textBox_dep_code.Location = new System.Drawing.Point(180, 68);
            this.textBox_dep_code.Name = "textBox_dep_code";
            this.textBox_dep_code.Size = new System.Drawing.Size(130, 20);
            this.textBox_dep_code.TabIndex = 11;
            // 
            // textBox_boradcom_pn
            // 
            this.textBox_boradcom_pn.Location = new System.Drawing.Point(180, 38);
            this.textBox_boradcom_pn.Name = "textBox_boradcom_pn";
            this.textBox_boradcom_pn.Size = new System.Drawing.Size(130, 20);
            this.textBox_boradcom_pn.TabIndex = 10;
            // 
            // textBox_model_name
            // 
            this.textBox_model_name.Location = new System.Drawing.Point(180, 9);
            this.textBox_model_name.Name = "textBox_model_name";
            this.textBox_model_name.Size = new System.Drawing.Size(130, 20);
            this.textBox_model_name.TabIndex = 9;
            // 
            // buttonQuery
            // 
            this.buttonQuery.Location = new System.Drawing.Point(363, 15);
            this.buttonQuery.Name = "buttonQuery";
            this.buttonQuery.Size = new System.Drawing.Size(75, 23);
            this.buttonQuery.TabIndex = 8;
            this.buttonQuery.Text = "Query";
            this.buttonQuery.UseVisualStyleBackColor = true;
            this.buttonQuery.Click += new System.EventHandler(this.buttonQuery_Click);
            // 
            // button_insert
            // 
            this.button_insert.Location = new System.Drawing.Point(363, 58);
            this.button_insert.Name = "button_insert";
            this.button_insert.Size = new System.Drawing.Size(75, 23);
            this.button_insert.TabIndex = 7;
            this.button_insert.Text = "Insert";
            this.button_insert.UseVisualStyleBackColor = true;
            this.button_insert.Click += new System.EventHandler(this.button_insert_Click);
            // 
            // button_modify
            // 
            this.button_modify.Location = new System.Drawing.Point(363, 104);
            this.button_modify.Name = "button_modify";
            this.button_modify.Size = new System.Drawing.Size(75, 23);
            this.button_modify.TabIndex = 6;
            this.button_modify.Text = "Modify";
            this.button_modify.UseVisualStyleBackColor = true;
            this.button_modify.Click += new System.EventHandler(this.button_modify_Click);
            // 
            // button_delete
            // 
            this.button_delete.Location = new System.Drawing.Point(363, 149);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(75, 23);
            this.button_delete.TabIndex = 5;
            this.button_delete.Text = "Delete";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(34, 130);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "box code";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(16, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "sotediagversion";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(170, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "borad department code";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(25, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "broadcom pn";
            // 
            // label_model_name
            // 
            this.label_model_name.AutoSize = true;
            this.label_model_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_model_name.Location = new System.Drawing.Point(25, 12);
            this.label_model_name.Name = "label_model_name";
            this.label_model_name.Size = new System.Drawing.Size(93, 16);
            this.label_model_name.TabIndex = 0;
            this.label_model_name.Text = "model name";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(8, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(463, 246);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.Click += new System.EventHandler(this.dataGridView1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Location = new System.Drawing.Point(12, 199);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(480, 270);
            this.panel2.TabIndex = 1;
            // 
            // BRCM_PN_CONFIG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 481);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BRCM_PN_CONFIG";
            this.Text = "BRCM_PN_CONFIG";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox_box_weight;
        private System.Windows.Forms.TextBox textBox_sdversion;
        private System.Windows.Forms.TextBox textBox_dep_code;
        private System.Windows.Forms.TextBox textBox_boradcom_pn;
        private System.Windows.Forms.TextBox textBox_model_name;
        private System.Windows.Forms.Button buttonQuery;
        private System.Windows.Forms.Button button_insert;
        private System.Windows.Forms.Button button_modify;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_model_name;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_box_code;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel2;
    }
}