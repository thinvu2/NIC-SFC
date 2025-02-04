using System.Windows.Forms;

namespace HOLD
{
    partial class HandHold
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HandHold));
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkTest = new System.Windows.Forms.CheckBox();
            this.cbxHoldMaterial = new System.Windows.Forms.CheckBox();
            this.cbxUnHoldMaterial = new System.Windows.Forms.CheckBox();
            this.lblTotalQty = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxAteStation = new System.Windows.Forms.CheckBox();
            this.cbbStation = new System.Windows.Forms.ComboBox();
            this.cbbGroup = new System.Windows.Forms.ComboBox();
            this.txtAteStation = new System.Windows.Forms.TextBox();
            this.dateTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.cbbTimeEnd = new System.Windows.Forms.ComboBox();
            this.dateTimeStart = new System.Windows.Forms.DateTimePicker();
            this.cbbTimeStart = new System.Windows.Forms.ComboBox();
            this.cbxTestTime = new System.Windows.Forms.CheckBox();
            this.cbxHoldByStation = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbxGroup = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.txtCondition = new System.Windows.Forms.TextBox();
            this.cbbCondition = new System.Windows.Forms.ComboBox();
            this.cbxFG = new System.Windows.Forms.CheckBox();
            this.cbxUnLineMo = new System.Windows.Forms.CheckBox();
            this.cbxHoldStockin = new System.Windows.Forms.CheckBox();
            this.cbbModel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnHold = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnUnHold = new System.Windows.Forms.Button();
            this.lblFailQty = new System.Windows.Forms.Label();
            this.lblSuccessQty = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.connectDB = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssEmp = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.txtErrorSN = new System.Windows.Forms.TextBox();
            this.btnExcel = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.chkShowStack = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.chkShowStack);
            this.panel1.Controls.Add(this.chkTest);
            this.panel1.Controls.Add(this.cbxHoldMaterial);
            this.panel1.Controls.Add(this.cbxUnHoldMaterial);
            this.panel1.Controls.Add(this.lblTotalQty);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnQuery);
            this.panel1.Controls.Add(this.txtCondition);
            this.panel1.Controls.Add(this.cbbCondition);
            this.panel1.Controls.Add(this.cbxFG);
            this.panel1.Controls.Add(this.cbxUnLineMo);
            this.panel1.Controls.Add(this.cbxHoldStockin);
            this.panel1.Controls.Add(this.cbbModel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 1);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1178, 182);
            this.panel1.TabIndex = 2;
            // 
            // chkTest
            // 
            this.chkTest.AutoSize = true;
            this.chkTest.Location = new System.Drawing.Point(7, 163);
            this.chkTest.Name = "chkTest";
            this.chkTest.Size = new System.Drawing.Size(381, 17);
            this.chkTest.TabIndex = 19;
            this.chkTest.Text = "Tính năng thử nghiệm : tăng tốc độ hold/unhold khi dùng mã mPallet(IMEI)";
            this.chkTest.UseVisualStyleBackColor = true;
            this.chkTest.CheckedChanged += new System.EventHandler(this.chkTest_CheckedChanged);
            // 
            // cbxHoldMaterial
            // 
            this.cbxHoldMaterial.AutoSize = true;
            this.cbxHoldMaterial.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxHoldMaterial.Location = new System.Drawing.Point(462, 10);
            this.cbxHoldMaterial.Margin = new System.Windows.Forms.Padding(2);
            this.cbxHoldMaterial.Name = "cbxHoldMaterial";
            this.cbxHoldMaterial.Size = new System.Drawing.Size(109, 21);
            this.cbxHoldMaterial.TabIndex = 18;
            this.cbxHoldMaterial.Text = "HoldMaterial";
            this.cbxHoldMaterial.UseVisualStyleBackColor = true;
            this.cbxHoldMaterial.CheckedChanged += new System.EventHandler(this.cbxHoldMaterial_CheckedChanged);
            // 
            // cbxUnHoldMaterial
            // 
            this.cbxUnHoldMaterial.AutoSize = true;
            this.cbxUnHoldMaterial.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxUnHoldMaterial.Location = new System.Drawing.Point(462, 43);
            this.cbxUnHoldMaterial.Margin = new System.Windows.Forms.Padding(2);
            this.cbxUnHoldMaterial.Name = "cbxUnHoldMaterial";
            this.cbxUnHoldMaterial.Size = new System.Drawing.Size(127, 21);
            this.cbxUnHoldMaterial.TabIndex = 17;
            this.cbxUnHoldMaterial.Text = "UnHoldMaterial";
            this.cbxUnHoldMaterial.UseVisualStyleBackColor = true;
            this.cbxUnHoldMaterial.CheckedChanged += new System.EventHandler(this.cbxUnHoldMaterial_CheckedChanged);
            // 
            // lblTotalQty
            // 
            this.lblTotalQty.Font = new System.Drawing.Font("Cambria", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalQty.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalQty.Location = new System.Drawing.Point(1056, 127);
            this.lblTotalQty.Name = "lblTotalQty";
            this.lblTotalQty.Size = new System.Drawing.Size(110, 32);
            this.lblTotalQty.TabIndex = 16;
            this.lblTotalQty.Text = "0";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbxAteStation);
            this.groupBox1.Controls.Add(this.cbbStation);
            this.groupBox1.Controls.Add(this.cbbGroup);
            this.groupBox1.Controls.Add(this.txtAteStation);
            this.groupBox1.Controls.Add(this.dateTimeEnd);
            this.groupBox1.Controls.Add(this.cbbTimeEnd);
            this.groupBox1.Controls.Add(this.dateTimeStart);
            this.groupBox1.Controls.Add(this.cbbTimeStart);
            this.groupBox1.Controls.Add(this.cbxTestTime);
            this.groupBox1.Controls.Add(this.cbxHoldByStation);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cbxGroup);
            this.groupBox1.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(7, 64);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(737, 99);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hold by test time";
            // 
            // cbxAteStation
            // 
            this.cbxAteStation.AutoSize = true;
            this.cbxAteStation.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxAteStation.Location = new System.Drawing.Point(4, 69);
            this.cbxAteStation.Margin = new System.Windows.Forms.Padding(2);
            this.cbxAteStation.Name = "cbxAteStation";
            this.cbxAteStation.Size = new System.Drawing.Size(96, 21);
            this.cbxAteStation.TabIndex = 22;
            this.cbxAteStation.Text = "AteStation:";
            this.cbxAteStation.UseVisualStyleBackColor = true;
            // 
            // cbbStation
            // 
            this.cbbStation.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbStation.FormattingEnabled = true;
            this.cbbStation.Location = new System.Drawing.Point(564, 67);
            this.cbbStation.Margin = new System.Windows.Forms.Padding(2);
            this.cbbStation.Name = "cbbStation";
            this.cbbStation.Size = new System.Drawing.Size(164, 27);
            this.cbbStation.TabIndex = 20;
            // 
            // cbbGroup
            // 
            this.cbbGroup.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbGroup.FormattingEnabled = true;
            this.cbbGroup.Location = new System.Drawing.Point(582, 27);
            this.cbbGroup.Margin = new System.Windows.Forms.Padding(2);
            this.cbbGroup.Name = "cbbGroup";
            this.cbbGroup.Size = new System.Drawing.Size(145, 27);
            this.cbbGroup.TabIndex = 19;
            // 
            // txtAteStation
            // 
            this.txtAteStation.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAteStation.Location = new System.Drawing.Point(103, 67);
            this.txtAteStation.Margin = new System.Windows.Forms.Padding(2);
            this.txtAteStation.Name = "txtAteStation";
            this.txtAteStation.Size = new System.Drawing.Size(185, 26);
            this.txtAteStation.TabIndex = 17;
            // 
            // dateTimeEnd
            // 
            this.dateTimeEnd.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimeEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimeEnd.Location = new System.Drawing.Point(296, 28);
            this.dateTimeEnd.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimeEnd.Name = "dateTimeEnd";
            this.dateTimeEnd.Size = new System.Drawing.Size(103, 24);
            this.dateTimeEnd.TabIndex = 15;
            // 
            // cbbTimeEnd
            // 
            this.cbbTimeEnd.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbTimeEnd.FormattingEnabled = true;
            this.cbbTimeEnd.Items.AddRange(new object[] {
            "00:00",
            "01:00",
            "02:00",
            "03:00",
            "04:00",
            "05:00",
            "06:00",
            "07:00",
            "08:00",
            "09:00",
            "10:00",
            "11:00",
            "12:00",
            "13:00",
            "14:00",
            "15:00",
            "16:00",
            "17:00",
            "18:00",
            "19:00",
            "20:00",
            "21:00",
            "22:00",
            "23:00"});
            this.cbbTimeEnd.Location = new System.Drawing.Point(399, 28);
            this.cbbTimeEnd.Margin = new System.Windows.Forms.Padding(2);
            this.cbbTimeEnd.Name = "cbbTimeEnd";
            this.cbbTimeEnd.Size = new System.Drawing.Size(76, 24);
            this.cbbTimeEnd.TabIndex = 14;
            // 
            // dateTimeStart
            // 
            this.dateTimeStart.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimeStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimeStart.Location = new System.Drawing.Point(94, 28);
            this.dateTimeStart.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimeStart.Name = "dateTimeStart";
            this.dateTimeStart.Size = new System.Drawing.Size(103, 24);
            this.dateTimeStart.TabIndex = 13;
            // 
            // cbbTimeStart
            // 
            this.cbbTimeStart.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbTimeStart.FormattingEnabled = true;
            this.cbbTimeStart.Items.AddRange(new object[] {
            "00:00",
            "01:00",
            "02:00",
            "03:00",
            "04:00",
            "05:00",
            "06:00",
            "07:00",
            "08:00",
            "09:00",
            "10:00",
            "11:00",
            "12:00",
            "13:00",
            "14:00",
            "15:00",
            "16:00",
            "17:00",
            "18:00",
            "19:00",
            "20:00",
            "21:00",
            "22:00",
            "23:00"});
            this.cbbTimeStart.Location = new System.Drawing.Point(197, 28);
            this.cbbTimeStart.Margin = new System.Windows.Forms.Padding(2);
            this.cbbTimeStart.Name = "cbbTimeStart";
            this.cbbTimeStart.Size = new System.Drawing.Size(70, 24);
            this.cbbTimeStart.TabIndex = 12;
            // 
            // cbxTestTime
            // 
            this.cbxTestTime.AutoSize = true;
            this.cbxTestTime.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxTestTime.Location = new System.Drawing.Point(4, 29);
            this.cbxTestTime.Margin = new System.Windows.Forms.Padding(2);
            this.cbxTestTime.Name = "cbxTestTime";
            this.cbxTestTime.Size = new System.Drawing.Size(89, 21);
            this.cbxTestTime.TabIndex = 3;
            this.cbxTestTime.Text = "TestTime:";
            this.cbxTestTime.UseVisualStyleBackColor = true;
            // 
            // cbxHoldByStation
            // 
            this.cbxHoldByStation.AutoSize = true;
            this.cbxHoldByStation.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxHoldByStation.Location = new System.Drawing.Point(447, 70);
            this.cbxHoldByStation.Margin = new System.Windows.Forms.Padding(2);
            this.cbxHoldByStation.Name = "cbxHoldByStation";
            this.cbxHoldByStation.Size = new System.Drawing.Size(121, 21);
            this.cbxHoldByStation.TabIndex = 24;
            this.cbxHoldByStation.Text = "HoldbyStation:";
            this.cbxHoldByStation.UseVisualStyleBackColor = true;
            this.cbxHoldByStation.CheckedChanged += new System.EventHandler(this.cbxHoldByStation_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(270, 31);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 19);
            this.label6.TabIndex = 16;
            this.label6.Text = "to";
            // 
            // cbxGroup
            // 
            this.cbxGroup.AutoSize = true;
            this.cbxGroup.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxGroup.Location = new System.Drawing.Point(508, 30);
            this.cbxGroup.Margin = new System.Windows.Forms.Padding(2);
            this.cbxGroup.Name = "cbxGroup";
            this.cbxGroup.Size = new System.Drawing.Size(70, 21);
            this.cbxGroup.TabIndex = 23;
            this.cbxGroup.Text = "Group:";
            this.cbxGroup.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(1007, 137);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 19);
            this.label3.TabIndex = 10;
            this.label3.Text = "Count:";
            // 
            // btnQuery
            // 
            this.btnQuery.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuery.Image = ((System.Drawing.Image)(resources.GetObject("btnQuery.Image")));
            this.btnQuery.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnQuery.Location = new System.Drawing.Point(1011, 11);
            this.btnQuery.Margin = new System.Windows.Forms.Padding(2);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(125, 48);
            this.btnQuery.TabIndex = 8;
            this.btnQuery.Text = "&Search";
            this.btnQuery.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // txtCondition
            // 
            this.txtCondition.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtCondition.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCondition.Location = new System.Drawing.Point(748, 2);
            this.txtCondition.Margin = new System.Windows.Forms.Padding(2);
            this.txtCondition.Multiline = true;
            this.txtCondition.Name = "txtCondition";
            this.txtCondition.Size = new System.Drawing.Size(255, 162);
            this.txtCondition.TabIndex = 7;
            // 
            // cbbCondition
            // 
            this.cbbCondition.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbbCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbCondition.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbCondition.FormattingEnabled = true;
            this.cbbCondition.Items.AddRange(new object[] {
            "SERIAL_NUMBER",
            "SSN1",
            "MO_NUMBER",
            "TRAY_NO",
            "CARTON_NO",
            "MCARTON_NO",
            "PALLET_NO",
            "IMEI",
            "SHIPPING_SN",
            "HOLD_NO",
            "SHIPPING_SN2",
            "MODEL_NAME",
            "PO_NO",
            "REWORK_NO"});
            this.cbbCondition.Location = new System.Drawing.Point(590, 8);
            this.cbbCondition.Margin = new System.Windows.Forms.Padding(2);
            this.cbbCondition.Name = "cbbCondition";
            this.cbbCondition.Size = new System.Drawing.Size(152, 27);
            this.cbbCondition.TabIndex = 16;
            this.cbbCondition.SelectedIndexChanged += new System.EventHandler(this.cbbCondition_SelectedIndexChanged);
            this.cbbCondition.SelectedValueChanged += new System.EventHandler(this.cbbCondition_SelectedValueChanged);
            // 
            // cbxFG
            // 
            this.cbxFG.AutoSize = true;
            this.cbxFG.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxFG.Location = new System.Drawing.Point(388, 10);
            this.cbxFG.Margin = new System.Windows.Forms.Padding(2);
            this.cbxFG.Name = "cbxFG";
            this.cbxFG.Size = new System.Drawing.Size(44, 21);
            this.cbxFG.TabIndex = 4;
            this.cbxFG.Text = "FG";
            this.cbxFG.UseVisualStyleBackColor = true;
            this.cbxFG.CheckedChanged += new System.EventHandler(this.cbxFG_CheckedChanged);
            // 
            // cbxUnLineMo
            // 
            this.cbxUnLineMo.AutoSize = true;
            this.cbxUnLineMo.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxUnLineMo.Location = new System.Drawing.Point(235, 43);
            this.cbxUnLineMo.Margin = new System.Windows.Forms.Padding(2);
            this.cbxUnLineMo.Name = "cbxUnLineMo";
            this.cbxUnLineMo.Size = new System.Drawing.Size(122, 21);
            this.cbxUnLineMo.TabIndex = 3;
            this.cbxUnLineMo.Text = "HoldUnLineMo";
            this.cbxUnLineMo.UseVisualStyleBackColor = true;
            this.cbxUnLineMo.CheckedChanged += new System.EventHandler(this.cbxUnLineMo_CheckedChanged);
            // 
            // cbxHoldStockin
            // 
            this.cbxHoldStockin.AutoSize = true;
            this.cbxHoldStockin.Font = new System.Drawing.Font("Cambria", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxHoldStockin.Location = new System.Drawing.Point(235, 10);
            this.cbxHoldStockin.Margin = new System.Windows.Forms.Padding(2);
            this.cbxHoldStockin.Name = "cbxHoldStockin";
            this.cbxHoldStockin.Size = new System.Drawing.Size(140, 21);
            this.cbxHoldStockin.TabIndex = 2;
            this.cbxHoldStockin.Text = "HoldStockinbymo";
            this.cbxHoldStockin.UseVisualStyleBackColor = true;
            this.cbxHoldStockin.CheckedChanged += new System.EventHandler(this.cbxHoldStockin_CheckedChanged);
            // 
            // cbbModel
            // 
            this.cbbModel.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbModel.FormattingEnabled = true;
            this.cbbModel.Location = new System.Drawing.Point(64, 8);
            this.cbbModel.Margin = new System.Windows.Forms.Padding(2);
            this.cbbModel.Name = "cbbModel";
            this.cbbModel.Size = new System.Drawing.Size(152, 27);
            this.cbbModel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Model:";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(2, 187);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1001, 262);
            this.dataGridView1.TabIndex = 3;
            // 
            // txtReason
            // 
            this.txtReason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReason.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtReason.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReason.Location = new System.Drawing.Point(186, 465);
            this.txtReason.Margin = new System.Windows.Forms.Padding(2);
            this.txtReason.Multiline = true;
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(676, 64);
            this.txtReason.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(9, 470);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(175, 19);
            this.label5.TabIndex = 6;
            this.label5.Text = "Hold or Unhold Reason:";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(888, 509);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 19);
            this.label7.TabIndex = 7;
            this.label7.Text = "Fail Count:";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(881, 465);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 19);
            this.label8.TabIndex = 8;
            this.label8.Text = "Hold Count:";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(968, 507);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(18, 19);
            this.label9.TabIndex = 9;
            this.label9.Text = "0";
            // 
            // btnHold
            // 
            this.btnHold.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnHold.Enabled = false;
            this.btnHold.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHold.Image = ((System.Drawing.Image)(resources.GetObject("btnHold.Image")));
            this.btnHold.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHold.Location = new System.Drawing.Point(316, 540);
            this.btnHold.Margin = new System.Windows.Forms.Padding(2);
            this.btnHold.Name = "btnHold";
            this.btnHold.Size = new System.Drawing.Size(99, 41);
            this.btnHold.TabIndex = 11;
            this.btnHold.Text = "&Hold";
            this.btnHold.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnHold.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHold.UseVisualStyleBackColor = true;
            this.btnHold.Click += new System.EventHandler(this.btnHold_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnClear.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClear.Location = new System.Drawing.Point(592, 540);
            this.btnClear.Margin = new System.Windows.Forms.Padding(2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(99, 41);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "&Clear";
            this.btnClear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnUnHold
            // 
            this.btnUnHold.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnUnHold.Enabled = false;
            this.btnUnHold.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnHold.Image = ((System.Drawing.Image)(resources.GetObject("btnUnHold.Image")));
            this.btnUnHold.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUnHold.Location = new System.Drawing.Point(449, 539);
            this.btnUnHold.Margin = new System.Windows.Forms.Padding(2);
            this.btnUnHold.Name = "btnUnHold";
            this.btnUnHold.Size = new System.Drawing.Size(109, 42);
            this.btnUnHold.TabIndex = 13;
            this.btnUnHold.Text = "&Unhold";
            this.btnUnHold.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUnHold.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUnHold.UseVisualStyleBackColor = true;
            this.btnUnHold.Click += new System.EventHandler(this.btnUnHold_Click);
            // 
            // lblFailQty
            // 
            this.lblFailQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFailQty.Font = new System.Drawing.Font("Cambria", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFailQty.ForeColor = System.Drawing.Color.Blue;
            this.lblFailQty.Location = new System.Drawing.Point(970, 499);
            this.lblFailQty.Name = "lblFailQty";
            this.lblFailQty.Size = new System.Drawing.Size(130, 32);
            this.lblFailQty.TabIndex = 14;
            this.lblFailQty.Text = "0";
            // 
            // lblSuccessQty
            // 
            this.lblSuccessQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSuccessQty.Font = new System.Drawing.Font("Cambria", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSuccessQty.ForeColor = System.Drawing.Color.Blue;
            this.lblSuccessQty.Location = new System.Drawing.Point(970, 454);
            this.lblSuccessQty.Name = "lblSuccessQty";
            this.lblSuccessQty.Size = new System.Drawing.Size(166, 32);
            this.lblSuccessQty.TabIndex = 15;
            this.lblSuccessQty.Text = "0";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectDB,
            this.tssEmp,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 592);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1178, 24);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // connectDB
            // 
            this.connectDB.AutoSize = false;
            this.connectDB.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectDB.Name = "connectDB";
            this.connectDB.Size = new System.Drawing.Size(200, 19);
            this.connectDB.Text = "status_connectDB";
            // 
            // tssEmp
            // 
            this.tssEmp.AutoSize = false;
            this.tssEmp.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tssEmp.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tssEmp.Name = "tssEmp";
            this.tssEmp.Size = new System.Drawing.Size(320, 19);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.AutoSize = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(500, 18);
            // 
            // txtErrorSN
            // 
            this.txtErrorSN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtErrorSN.BackColor = System.Drawing.Color.AntiqueWhite;
            this.txtErrorSN.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtErrorSN.Location = new System.Drawing.Point(1002, 187);
            this.txtErrorSN.Multiline = true;
            this.txtErrorSN.Name = "txtErrorSN";
            this.txtErrorSN.Size = new System.Drawing.Size(176, 262);
            this.txtErrorSN.TabIndex = 17;
            this.txtErrorSN.Text = "Error Message:";
            // 
            // btnExcel
            // 
            this.btnExcel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnExcel.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExcel.Image = global::HOLD.Properties.Resources.iconfinder_Picture22_3289563;
            this.btnExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExcel.Location = new System.Drawing.Point(725, 540);
            this.btnExcel.Margin = new System.Windows.Forms.Padding(2);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(99, 41);
            this.btnExcel.TabIndex = 18;
            this.btnExcel.Text = "&Excel";
            this.btnExcel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExcel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.BtnExcel_Click);
            // 
            // chkShowStack
            // 
            this.chkShowStack.AutoSize = true;
            this.chkShowStack.Location = new System.Drawing.Point(11, 45);
            this.chkShowStack.Name = "chkShowStack";
            this.chkShowStack.Size = new System.Drawing.Size(126, 17);
            this.chkShowStack.TabIndex = 20;
            this.chkShowStack.Text = "Show message detail";
            this.chkShowStack.UseVisualStyleBackColor = true;
            // 
            // HandHold
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 616);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.txtErrorSN);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lblSuccessQty);
            this.Controls.Add(this.lblFailQty);
            this.Controls.Add(this.btnUnHold);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnHold);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtReason);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label5);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "HandHold";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HandHold : Message default is Vietnamese,You can change to English in Main form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.HandHold_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cbbCondition;
        private System.Windows.Forms.CheckBox cbxFG;
        private System.Windows.Forms.CheckBox cbxUnLineMo;
        private System.Windows.Forms.CheckBox cbxHoldStockin;
        private System.Windows.Forms.ComboBox cbbModel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbbGroup;
        private System.Windows.Forms.TextBox txtAteStation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateTimeEnd;
        private System.Windows.Forms.ComboBox cbbTimeEnd;
        private System.Windows.Forms.DateTimePicker dateTimeStart;
        private System.Windows.Forms.ComboBox cbbTimeStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.TextBox txtCondition;
        private System.Windows.Forms.CheckBox cbxAteStation;
        private System.Windows.Forms.CheckBox cbxTestTime;
        private System.Windows.Forms.ComboBox cbbStation;
        private System.Windows.Forms.Label lblTotalQty;
        private System.Windows.Forms.CheckBox cbxHoldByStation;
        private System.Windows.Forms.CheckBox cbxGroup;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnHold;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnUnHold;
        private System.Windows.Forms.Label lblFailQty;
        private System.Windows.Forms.Label lblSuccessQty;
        private System.Windows.Forms.CheckBox cbxHoldMaterial;
        private System.Windows.Forms.CheckBox cbxUnHoldMaterial;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel tssEmp;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripStatusLabel connectDB;
        private TextBox txtErrorSN;
        private Button btnExcel;
        private SaveFileDialog saveFileDialog1;
        private CheckBox chkTest;
        private CheckBox chkShowStack;
    }
}