using System;

namespace FG_CHECK
{
    partial class fMainShippingLabel
    {
        [System.Runtime.InteropServices.DllImport("SfcScann.dll")]
        public static extern int SfcSubclass(IntPtr h);
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
            this.reprintToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tiếngViệtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showParamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rESETToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.lPass = new System.Windows.Forms.Label();
            this.txt_carton = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_endcustpo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_custcode = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_finishdc = new System.Windows.Forms.TextBox();
            this.Lfinishdc = new System.Windows.Forms.Label();
            this.txt_shipaddress = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_finishdate = new System.Windows.Forms.TextBox();
            this.Lfinishdate = new System.Windows.Forms.Label();
            this.txt_custpn = new System.Windows.Forms.TextBox();
            this.txt_custname = new System.Windows.Forms.TextBox();
            this.txt_palletcount = new System.Windows.Forms.TextBox();
            this.txt_cartoncount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_labelqty = new System.Windows.Forms.TextBox();
            this.txt_ztext = new System.Windows.Forms.TextBox();
            this.txt_dn = new System.Windows.Forms.TextBox();
            this.txt_emp = new System.Windows.Forms.TextBox();
            this.Llabelqty = new System.Windows.Forms.Label();
            this.LZTEXT = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.errorMessage = new System.Windows.Forms.Label();
            this.pmes = new System.Windows.Forms.Panel();
            this.lerror = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pmes.SuspendLayout();
            this.SuspendLayout();
            // 
            // reprintToolStripMenuItem1
            // 
            this.reprintToolStripMenuItem1.Name = "reprintToolStripMenuItem1";
            this.reprintToolStripMenuItem1.Size = new System.Drawing.Size(115, 22);
            this.reprintToolStripMenuItem1.Text = "Reprint";
            this.reprintToolStripMenuItem1.Click += new System.EventHandler(this.reprintToolStripMenuItem1_Click);
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.tiếngViệtToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            this.languageToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.languageToolStripMenuItem.Text = "Language";
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Checked = true;
            this.englishToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.englishToolStripMenuItem.Text = "English";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // tiếngViệtToolStripMenuItem
            // 
            this.tiếngViệtToolStripMenuItem.Name = "tiếngViệtToolStripMenuItem";
            this.tiếngViệtToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.tiếngViệtToolStripMenuItem.Text = "Tiếng Việt";
            this.tiếngViệtToolStripMenuItem.Click += new System.EventHandler(this.tiếngViệtToolStripMenuItem_Click);
            // 
            // visibleToolStripMenuItem
            // 
            this.visibleToolStripMenuItem.Name = "visibleToolStripMenuItem";
            this.visibleToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.visibleToolStripMenuItem.Text = "Visible";
            this.visibleToolStripMenuItem.Click += new System.EventHandler(this.visibleToolStripMenuItem_Click);
            // 
            // showParamToolStripMenuItem
            // 
            this.showParamToolStripMenuItem.Name = "showParamToolStripMenuItem";
            this.showParamToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
            this.showParamToolStripMenuItem.Text = "ShowParam";
            this.showParamToolStripMenuItem.Click += new System.EventHandler(this.showParamToolStripMenuItem_Click);
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reprintToolStripMenuItem1,
            this.rESETToolStripMenuItem});
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.setupToolStripMenuItem.Text = "Setup";
            // 
            // rESETToolStripMenuItem
            // 
            this.rESETToolStripMenuItem.Name = "rESETToolStripMenuItem";
            this.rESETToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.rESETToolStripMenuItem.Text = "RESET";
            this.rESETToolStripMenuItem.Click += new System.EventHandler(this.rESETToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem,
            this.languageToolStripMenuItem,
            this.visibleToolStripMenuItem,
            this.showParamToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(966, 24);
            this.menuStrip1.TabIndex = 33;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 293);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 29);
            this.button1.TabIndex = 32;
            this.button1.Text = "TEST";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.DarkCyan;
            this.groupBox1.Controls.Add(this.txtPass);
            this.groupBox1.Controls.Add(this.lPass);
            this.groupBox1.Controls.Add(this.txt_carton);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.txt_endcustpo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txt_custcode);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txt_finishdc);
            this.groupBox1.Controls.Add(this.Lfinishdc);
            this.groupBox1.Controls.Add(this.txt_shipaddress);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txt_finishdate);
            this.groupBox1.Controls.Add(this.Lfinishdate);
            this.groupBox1.Controls.Add(this.txt_custpn);
            this.groupBox1.Controls.Add(this.txt_custname);
            this.groupBox1.Controls.Add(this.txt_palletcount);
            this.groupBox1.Controls.Add(this.txt_cartoncount);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txt_labelqty);
            this.groupBox1.Controls.Add(this.txt_ztext);
            this.groupBox1.Controls.Add(this.txt_dn);
            this.groupBox1.Controls.Add(this.txt_emp);
            this.groupBox1.Controls.Add(this.Llabelqty);
            this.groupBox1.Controls.Add(this.LZTEXT);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(949, 444);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            // 
            // txtPass
            // 
            this.txtPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPass.Location = new System.Drawing.Point(525, 15);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(157, 26);
            this.txtPass.TabIndex = 34;
            this.txtPass.Visible = false;
            this.txtPass.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPass_KeyPress);
            // 
            // lPass
            // 
            this.lPass.AutoSize = true;
            this.lPass.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lPass.ForeColor = System.Drawing.Color.White;
            this.lPass.Location = new System.Drawing.Point(354, 15);
            this.lPass.Name = "lPass";
            this.lPass.Size = new System.Drawing.Size(140, 20);
            this.lPass.TabIndex = 33;
            this.lPass.Text = "PASS_REPRINT:";
            this.lPass.Visible = false;
            // 
            // txt_carton
            // 
            this.txt_carton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_carton.Location = new System.Drawing.Point(354, 322);
            this.txt_carton.Name = "txt_carton";
            this.txt_carton.Size = new System.Drawing.Size(210, 26);
            this.txt_carton.TabIndex = 27;
            this.txt_carton.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_carton_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(202, 328);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 20);
            this.label4.TabIndex = 26;
            this.label4.Text = "CARTON:";
            // 
            // txt_endcustpo
            // 
            this.txt_endcustpo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_endcustpo.Location = new System.Drawing.Point(525, 242);
            this.txt_endcustpo.Name = "txt_endcustpo";
            this.txt_endcustpo.Size = new System.Drawing.Size(327, 26);
            this.txt_endcustpo.TabIndex = 25;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(368, 248);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 20);
            this.label3.TabIndex = 24;
            this.label3.Text = "ENDCUSTPO:";
            // 
            // txt_custcode
            // 
            this.txt_custcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_custcode.Location = new System.Drawing.Point(525, 207);
            this.txt_custcode.Name = "txt_custcode";
            this.txt_custcode.Size = new System.Drawing.Size(327, 26);
            this.txt_custcode.TabIndex = 23;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(379, 213);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(106, 20);
            this.label11.TabIndex = 22;
            this.label11.Text = "CUSTCODE:";
            // 
            // txt_finishdc
            // 
            this.txt_finishdc.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_finishdc.Location = new System.Drawing.Point(175, 203);
            this.txt_finishdc.Name = "txt_finishdc";
            this.txt_finishdc.Size = new System.Drawing.Size(157, 26);
            this.txt_finishdc.TabIndex = 21;
            // 
            // Lfinishdc
            // 
            this.Lfinishdc.AutoSize = true;
            this.Lfinishdc.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lfinishdc.ForeColor = System.Drawing.Color.White;
            this.Lfinishdc.Location = new System.Drawing.Point(23, 209);
            this.Lfinishdc.Name = "Lfinishdc";
            this.Lfinishdc.Size = new System.Drawing.Size(91, 20);
            this.Lfinishdc.TabIndex = 20;
            this.Lfinishdc.Text = "FINISHDC:";
            // 
            // txt_shipaddress
            // 
            this.txt_shipaddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_shipaddress.Location = new System.Drawing.Point(180, 271);
            this.txt_shipaddress.Name = "txt_shipaddress";
            this.txt_shipaddress.Size = new System.Drawing.Size(616, 26);
            this.txt_shipaddress.TabIndex = 19;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(9, 277);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(133, 20);
            this.label9.TabIndex = 18;
            this.label9.Text = "SHIPADDRESS:";
            // 
            // txt_finishdate
            // 
            this.txt_finishdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_finishdate.Location = new System.Drawing.Point(175, 164);
            this.txt_finishdate.Name = "txt_finishdate";
            this.txt_finishdate.Size = new System.Drawing.Size(157, 26);
            this.txt_finishdate.TabIndex = 17;
            // 
            // Lfinishdate
            // 
            this.Lfinishdate.AutoSize = true;
            this.Lfinishdate.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lfinishdate.ForeColor = System.Drawing.Color.White;
            this.Lfinishdate.Location = new System.Drawing.Point(4, 170);
            this.Lfinishdate.Name = "Lfinishdate";
            this.Lfinishdate.Size = new System.Drawing.Size(112, 20);
            this.Lfinishdate.TabIndex = 16;
            this.Lfinishdate.Text = "FINISHDATE:";
            // 
            // txt_custpn
            // 
            this.txt_custpn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_custpn.Location = new System.Drawing.Point(525, 164);
            this.txt_custpn.Name = "txt_custpn";
            this.txt_custpn.Size = new System.Drawing.Size(327, 26);
            this.txt_custpn.TabIndex = 15;
            // 
            // txt_custname
            // 
            this.txt_custname.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_custname.Location = new System.Drawing.Point(525, 127);
            this.txt_custname.Name = "txt_custname";
            this.txt_custname.Size = new System.Drawing.Size(327, 26);
            this.txt_custname.TabIndex = 14;
            // 
            // txt_palletcount
            // 
            this.txt_palletcount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_palletcount.Location = new System.Drawing.Point(525, 90);
            this.txt_palletcount.Name = "txt_palletcount";
            this.txt_palletcount.Size = new System.Drawing.Size(327, 26);
            this.txt_palletcount.TabIndex = 13;
            // 
            // txt_cartoncount
            // 
            this.txt_cartoncount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_cartoncount.Location = new System.Drawing.Point(525, 55);
            this.txt_cartoncount.Name = "txt_cartoncount";
            this.txt_cartoncount.Size = new System.Drawing.Size(327, 26);
            this.txt_cartoncount.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(400, 170);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "CUSTPN:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(379, 133);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "CUSTNAME:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(353, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(136, 20);
            this.label7.TabIndex = 9;
            this.label7.Text = "PALLETCOUNT:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(351, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(142, 20);
            this.label8.TabIndex = 8;
            this.label8.Text = "CARTONCOUNT:";
            // 
            // txt_labelqty
            // 
            this.txt_labelqty.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_labelqty.Location = new System.Drawing.Point(175, 124);
            this.txt_labelqty.Name = "txt_labelqty";
            this.txt_labelqty.Size = new System.Drawing.Size(157, 26);
            this.txt_labelqty.TabIndex = 7;
            // 
            // txt_ztext
            // 
            this.txt_ztext.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_ztext.Location = new System.Drawing.Point(175, 87);
            this.txt_ztext.Name = "txt_ztext";
            this.txt_ztext.Size = new System.Drawing.Size(157, 26);
            this.txt_ztext.TabIndex = 6;
            // 
            // txt_dn
            // 
            this.txt_dn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_dn.Location = new System.Drawing.Point(175, 50);
            this.txt_dn.Name = "txt_dn";
            this.txt_dn.Size = new System.Drawing.Size(157, 26);
            this.txt_dn.TabIndex = 5;
            this.txt_dn.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_DN_KeyPress);
            // 
            // txt_emp
            // 
            this.txt_emp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_emp.Location = new System.Drawing.Point(175, 15);
            this.txt_emp.Name = "txt_emp";
            this.txt_emp.Size = new System.Drawing.Size(157, 26);
            this.txt_emp.TabIndex = 4;
            // 
            // Llabelqty
            // 
            this.Llabelqty.AutoSize = true;
            this.Llabelqty.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Llabelqty.ForeColor = System.Drawing.Color.White;
            this.Llabelqty.Location = new System.Drawing.Point(10, 130);
            this.Llabelqty.Name = "Llabelqty";
            this.Llabelqty.Size = new System.Drawing.Size(102, 20);
            this.Llabelqty.TabIndex = 3;
            this.Llabelqty.Text = "LABELQTY:";
            // 
            // LZTEXT
            // 
            this.LZTEXT.AutoSize = true;
            this.LZTEXT.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LZTEXT.ForeColor = System.Drawing.Color.White;
            this.LZTEXT.Location = new System.Drawing.Point(43, 90);
            this.LZTEXT.Name = "LZTEXT";
            this.LZTEXT.Size = new System.Drawing.Size(66, 20);
            this.LZTEXT.TabIndex = 2;
            this.LZTEXT.Text = "ZTEXT:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(70, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "DN:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font(".VnArial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "EMPLOYEE:";
            // 
            // errorMessage
            // 
            this.errorMessage.BackColor = System.Drawing.SystemColors.Info;
            this.errorMessage.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.errorMessage.ForeColor = System.Drawing.Color.Red;
            this.errorMessage.Location = new System.Drawing.Point(26, 80);
            this.errorMessage.Name = "errorMessage";
            this.errorMessage.Size = new System.Drawing.Size(762, 56);
            this.errorMessage.TabIndex = 6;
            this.errorMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pmes
            // 
            this.pmes.BackColor = System.Drawing.SystemColors.Info;
            this.pmes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pmes.Controls.Add(this.lerror);
            this.pmes.Controls.Add(this.errorMessage);
            this.pmes.Location = new System.Drawing.Point(13, 481);
            this.pmes.Name = "pmes";
            this.pmes.Size = new System.Drawing.Size(953, 62);
            this.pmes.TabIndex = 31;
            // 
            // lerror
            // 
            this.lerror.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lerror.Location = new System.Drawing.Point(7, 17);
            this.lerror.Name = "lerror";
            this.lerror.Size = new System.Drawing.Size(927, 26);
            this.lerror.TabIndex = 35;
            // 
            // fMainShippingLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 546);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.pmes);
            this.Name = "fMainShippingLabel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SHIPPING LABEL";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.fMainShippingLabel_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pmes.ResumeLayout(false);
            this.pmes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem reprintToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tiếngViệtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showParamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_labelqty;
        private System.Windows.Forms.TextBox txt_ztext;
        private System.Windows.Forms.TextBox txt_dn;
        private System.Windows.Forms.TextBox txt_emp;
        private System.Windows.Forms.Label Llabelqty;
        private System.Windows.Forms.Label LZTEXT;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_shipaddress;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txt_finishdate;
        private System.Windows.Forms.Label Lfinishdate;
        private System.Windows.Forms.TextBox txt_custpn;
        private System.Windows.Forms.TextBox txt_custname;
        private System.Windows.Forms.TextBox txt_palletcount;
        private System.Windows.Forms.TextBox txt_cartoncount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_custcode;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_finishdc;
        private System.Windows.Forms.Label Lfinishdc;
        private System.Windows.Forms.TextBox txt_endcustpo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_carton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label errorMessage;
        private System.Windows.Forms.Panel pmes;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label lPass;
        private System.Windows.Forms.ToolStripMenuItem rESETToolStripMenuItem;
        private System.Windows.Forms.TextBox lerror;
    }
}