using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFC_Library;
using Microsoft.Office.Interop.Excel;// import thu vien file excel
using System.Xml;
using System.Data;
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;

namespace FG_CHECK
{
    public partial class fMainNotice : Form
    {
        public EmployeeInfomation empInfo = new EmployeeInfomation();
        dbsfis dbsfis;
        private bool   isguest, isquery,isadmin;
        private string mysql;
        public static SfcHttpClient sfcClient;
        sfcconnect sfc;

        public fMainNotice(SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            mysql = "SELECT TO_NO,TO_ITEM_NO,DN_NO,DN_ITEM_NO,MODEL_NAME," +
                   "SHIPPING_QTY,CREATE_DATE,MATERIAL_GROUP,SHOW_FLAG," +
                   "CUST_PO,MODEL_DESC,CUST_NAME,CUST_ID,SO_NUMBER,SO_LINE," +
                   "UNIT_NW,UNIT_GW,UNIT_WEIGHT,UNIT_PRICE," +
                   "MATERIAL_TYPE,SAP_MODEL_NAME,INVOICE,CUSTOMER_CODE," +
                   "SHIP_CODE,SHIP_ADDRESS,HUB_FLAG,STORAGE_LOCATION " +
                   "FROM SFISM4.R_SAP_DN_DETAIL_T ";
            dbsfis = new dbsfis();
            sfcClient = _sfcClient;

        }
        private void fMain_Load(object sender, EventArgs e)
        {
            isguest = true;
            isquery = true;
            isadmin = true;
            status_Login.Text = string.Format("Login user: {0} - {1}", Form1.empNO, Form1.empName);
            GetSAPData();
            GetDNITEMData();
            GetDBCheckData();
            if (isquery)
            {
                btnShow.Visible = false;
                btnHide.Visible = false;
            }
            panel3.Enabled = true;
            panel3.Visible = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                panel3.Enabled = true;
                txbDn_No.Clear();
                txbSo_Number.Clear();
                txbModel_Name.Clear();
                txbDn_Item_No.Clear();
                txbCust_Po.Clear();
                txbModel_Name.Clear();
                btnPrint.Enabled = true;
                btnPacking.Enabled = true;
                txbSearch.Visible = true;
                panel3.Visible = true;
            }
            if (tabControl1.SelectedIndex == 1)
            {
                panel3.Visible = false;
                btnPrint.Enabled = true;
                btnPacking.Enabled = true;
                txbSearch.Visible = true;
            }

            if (tabControl1.SelectedIndex == 2)
            {
                btnPrint.Enabled = false;
                btnPacking.Enabled = false;
                txbSearch.Visible = false;
                dateTimePicker1.Value = DateTime.Today.AddDays(-6);
                dateTimePicker2.Value = DateTime.Today;
                panel3.Visible = false;
                setDBCheckTimeData();
            }
        }
        private void setDBCheckTimeData()//add gio
        {
            comboBox1.Items.Clear();
            for (int i = 0; i <= 9; i++)
            {
                comboBox1.Items.Add("0" + Convert.ToString(i) + ":00");
                comboBox2.Items.Add("0" + Convert.ToString(i) + ":00");
                comboBox1.Items.Add("0" + Convert.ToString(i) + ":30");
                comboBox2.Items.Add("0" + Convert.ToString(i) + ":30");
            }
            for (int i = 10; i <= 23; i++)
            {
                comboBox1.Items.Add(Convert.ToString(i) + ":00");
                comboBox2.Items.Add(Convert.ToString(i) + ":00");
                comboBox1.Items.Add(Convert.ToString(i) + ":30");
                comboBox2.Items.Add(Convert.ToString(i) + ":30");
            }

            comboBox2.Items.Add("23:59");
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                SortSAPData();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                GetDNITEMData();
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                GetDBCheckData();
            }
        }
        private async void GetDBCheckData()
        {
            string TIME1, TIME2;
            TIME1 = dateTimePicker1.Value.Date.ToString("yyyy-MM-dd") + " " + comboBox1.Text;
            TIME2 = dateTimePicker2.Value.Date.ToString("yyyy-MM-dd") + " " + comboBox2.Text;

            string query1 = "SELECT INV_NO,INVOICE,SO_QTY," +
                          "SHIPPING_QTY,FINISH_DATE,TCOM,MODEL_NAME  FROM sfism4.r_bpcs_invoice_t " +
                          "WHERE FORWARD IS NULL AND so_qty = shipping_qty " +
                          "AND tcom <> ' ' AND " +
                          "tcom <> 'N/A'  AND finish_date > " +
                          "TO_DATE ('" + TIME1 + "','YYYY/MM/DD HH24:MI:SS') AND " +
                          "finish_date < " +
                          "TO_DATE( '" + TIME2 + "','YYYY/MM/DD HH24:MI:SS')";

            System.Data.DataTable dt = await dbsfis.ExcuteSelectSQL(query1, sfcClient);
            DataView dv = new DataView(dt);
            dtgvDbCheck.DataSource = dv;
            //dtgvDbCheck.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            //dtgvDbCheck.RowHeadersVisible = false;
            //dtgvDbCheck.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }


        private async void GetSAPData()
        {
            string query2 = mysql + " WHERE SHOW_FLAG = 'Y' ";
            if (isguest)
            {
                query2 += " ORDER BY TO_NO, TO_ITEM_NO, DN_NO, DN_ITEM_NO";
            }
            System.Data.DataTable dt =await dbsfis.ExcuteSelectSQL(query2, sfcClient);
            DataView dv = new DataView(dt);
            dtgvSap.DataSource = dv;
            //dtgvSap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
        private async void SortSAPData()
        {

            if (!string.IsNullOrWhiteSpace(txbDn_No.Text) || !string.IsNullOrWhiteSpace(txbSo_Number.Text) || !string.IsNullOrWhiteSpace(txbModel_Name.Text) || !string.IsNullOrWhiteSpace(txbDn_Item_No.Text) || !string.IsNullOrWhiteSpace(txbCust_Po.Text))
            {

                string querySort = mysql + "WHERE 1=1";//noi chuoi

                if (txbDn_No.Text.Trim() != "")
                    querySort += " AND DN_NO= '" + txbDn_No.Text.Trim() + "'"; //select them dn_no

                if (txbCust_Po.Text.Trim() != "")
                    querySort += " AND CUST_PO= '" + txbCust_Po.Text.Trim() + "'"; // select them cust po

                if (txbSo_Number.Text.Trim() != "")
                    querySort += " AND SO_NUMBER= '" + txbSo_Number.Text.Trim() + "'"; //query them dk so_number

                if (txbDn_Item_No.Text.Trim() != "")
                    querySort += " AND DN_ITEM_NO= '" + txbDn_Item_No.Text.Trim() + "'"; //query them  dn_item_no

                if (txbModel_Name.Text.Trim() != "")
                    querySort += " AND MODEL_NAME = '" + txbModel_Name.Text.Trim() + "' "; //query them model_name

                querySort += " ORDER BY TO_NO, TO_ITEM_NO, DN_NO, DN_ITEM_NO";//noi chuoi
                btnShow.Enabled = true;
                btnHide.Enabled = true;

                //day du lieu len gridview
                System.Data.DataTable dt = await dbsfis.ExcuteSelectSQL(querySort, sfcClient);
                DataView dv = new DataView(dt);
                dtgvSap.DataSource = dv;
                //dtgvSap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            else
            {
                btnShow.Enabled = false;
                btnHide.Enabled = false;
                GetSAPData();
            }
        }
        private async void GetDNITEMData()
        {
            string query4 = " SELECT A.DN_NO,A.TO_ITEM_NO AS ITEM_NO,A.TO_NO," +
                   "A.CUSTOMER_CODE AS CUSTOMER,A.CREATE_DATE AS CREATETIME ," +
                   "B.MODEL_NAME AS P_NO," +
                   "A.MODEL_DESC,B.SO_QTY AS QTY,B.SHIPPING_QTY AS REAL_QTY " +
            "FROM SFISM4.R_SAP_DN_DETAIL_T A,SFISM4.R_BPCS_INVOICE_T B " +
            "WHERE A.DN_NO=B.INVOICE AND A.DN_ITEM_NO=B.INV_NO ";
            if (!string.IsNullOrWhiteSpace(txbDnNoPn.Text))
            {
                query4 += " AND A.DN_NO= '" + txbDnNoPn.Text.Trim() + "' ";
            }
            if (!string.IsNullOrWhiteSpace(txbToNoPn.Text))
            {
                query4 += " AND A.TO_NO= '" + txbToNoPn.Text.Trim() + "' ";
            }
            if (!string.IsNullOrWhiteSpace(txbModelNamePn.Text))
            {
                query4 += " AND A.MODEL_NAME= '" + txbModelNamePn.Text.Trim() + "' ";
            }
            System.Data.DataTable dt =await dbsfis.ExcuteSelectSQL(query4, sfcClient);
            DataView dv = new DataView(dt);
            dtgvDnItem.DataSource = dv;
            //dtgvDnItem.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;//rut thoi gian load du lieu len gridview
            //dtgvDnItem.RowHeadersVisible = false;//rut thoi gian load du lieu len gridview
            //dtgvDnItem.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private async void PrintSAPData()
        {
            try
            {
                int index = dtgvSap.CurrentCell.RowIndex;//lay ra chi muc dong hien tai
                DataGridViewRow selectRow = dtgvSap.Rows[index];//lay ra gia tri dong co index
                string DN_ITEM_NO = selectRow.Cells[3].Value.ToString();
                string INVOICE = selectRow.Cells[2].Value.ToString();//lay ra invoice
                string SO_NUMBER = selectRow.Cells[13].Value.ToString();
                string SO_LINE = selectRow.Cells[14].Value.ToString();
                int SHIPPING_QTY = Convert.ToInt32(selectRow.Cells[5].Value.ToString());
                string NW = selectRow.Cells[15].Value.ToString();//lay ra nw
                string GW = selectRow.Cells[16].Value.ToString();//lay ra gw
                string CUST_PO = selectRow.Cells[9].Value.ToString();
                string MODEL = selectRow.Cells[4].Value.ToString();//lay ra model
                string CUST_NAME = selectRow.Cells[11].Value.ToString();
                string CUST_ID = selectRow.Cells[12].Value.ToString();
                string VER = "0";
                if (NW == "")//nw bang rong thi nw = 0
                {
                    NW = "0";
                }
                if (GW == "")// gw bang rong thi gw= 0
                {
                    GW = "0";
                }
                string query3 = "select * from SFISM4.R_BPCS_INVOICE_T where INVOICE= '" + INVOICE + "' and ROWNUM=1";
                System.Data.DataTable dt = await dbsfis.ExcuteSelectSQL(query3, sfcClient);
                if (dt.Rows.Count != 0)// neu tra ve dong khac 0
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        VER = row["MODEL_VER"].ToString(); //lay ra ver la ver cua query
                    }
                }

                fPackNotice f = new fPackNotice(sfcClient); //tao doi tuong form fPackNotice
                f.empInfo = this.empInfo;
               // f.dbsfis = "NIC"; this.dbsfis;
                f.Sender(DN_ITEM_NO, INVOICE, SO_NUMBER, SO_LINE, SHIPPING_QTY, NW, GW, CUST_PO, MODEL, CUST_NAME, CUST_ID, VER);//gui du lieu den con tro
                f.ShowDialog();// show form
            }
            catch (Exception ex)
            {

            }
        }

        /*
        // ham xuat file excel
        private void ExportFileExcel(DataGridView dataGridView1) 
        {
            DateTime dtNow = DateTime.Now;
            DateTime excelDate = dtNow.AddYears(-1);
            int intExcelID = 0;
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = dataGridView1.Rows.Count;
            saveFileDialog1.InitialDirectory = "C:";
            saveFileDialog1.Title = "Save as Excel File";
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = "Excel Files (2003)|*.xls|Excel Files (2007)|*.xlsx";
            Microsoft.Office.Interop.Excel.Application Excelapp = new Microsoft.Office.Interop.Excel.Application();
            foreach (System.Diagnostics.Process excelProc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
            {
                if (excelProc.StartTime > excelDate)
                {
                    excelDate = excelProc.StartTime;
                    intExcelID = excelProc.Id;
                }
            }
            if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                progressBar1.Visible = true;
                try
                {
                    Excelapp.Application.Workbooks.Add(Type.Missing);
                    for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
                    {
                        Excelapp.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
                    }

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            string temp = "";
                            if (dataGridView1.Rows[i].Cells[j].Value == null)
                            {
                                temp = "";
                            }
                            else
                            {
                                temp = dataGridView1.Rows[i].Cells[j].Value.ToString();
                            }
                            Excelapp.Cells[i + 2, j + 1] = temp;
                        }
                        progressBar1.Value++;
                    }
                    Excelapp.ActiveWorkbook.SaveCopyAs(saveFileDialog1.FileName.ToString());
                    Excelapp.ActiveWorkbook.Saved = true;
                    Excelapp.Quit();

                    MessageBox.Show("Export to Excel successful! Path: " + saveFileDialog1.FileName, "Message");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ExportToExcel: Excel file could not be saved! Check filepath.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                foreach (System.Diagnostics.Process excelProc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                {
                    if (excelProc.Id == intExcelID)
                    {
                        excelProc.Kill();
                    }
                }
            }
            progressBar1.Visible = false;
        }
        */

        private void btnSaveToExcel_Click(object sender, EventArgs e)
        {
            //ExportFileExcel(dtgvDnItem);

        }

        private void dtgvSap_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;//lay ra chi muc dong hien tai
            DataGridViewRow selectRow = dtgvSap.Rows[index];//lay ra dong hien tai
            txbDn_No.Text = selectRow.Cells[2].Value.ToString();//lay ra dn va gan gia tri
            txbSo_Number.Text = selectRow.Cells[13].Value.ToString();//lay ra so number
            txbModel_Name.Text = selectRow.Cells[4].Value.ToString();//lay ra model name
            txbDn_Item_No.Text = selectRow.Cells[3].Value.ToString();//lay ra dn_item_no
            txbCust_Po.Text = selectRow.Cells[9].Value.ToString();// lay ra cust_po
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //ExportFileExcel(dtgvDbCheck);
        }

        private async void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                SortSAPData();
                if (dtgvSap.Rows.Count <= 1)
                {
                    MessageBox.Show("Warning: no data for operate");
                    btnShow.Enabled = false;
                    btnHide.Enabled = false;
                    return;
                }
                if (!string.IsNullOrWhiteSpace(txbDn_No.Text) || !string.IsNullOrWhiteSpace(txbSo_Number.Text) || !string.IsNullOrWhiteSpace(txbModel_Name.Text) || !string.IsNullOrWhiteSpace(txbDn_Item_No.Text) || !string.IsNullOrWhiteSpace(txbCust_Po.Text))
                {

                    string query4 = "";

                    if (txbDn_No.Text.Trim() != "")
                        query4 += " AND DN_NO= '" + txbDn_No.Text.Trim() + "'"; //select them dn_no

                    if (txbCust_Po.Text.Trim() != "")
                        query4 += " AND CUST_PO='" + txbCust_Po.Text.Trim() + "'"; // select them cust po

                    if (txbSo_Number.Text.Trim() != "")
                        query4 += " AND SO_NUMBER= '" + txbSo_Number.Text.Trim() + "'"; //query them dk so_number

                    if (txbDn_Item_No.Text.Trim() != "")
                        query4 += " AND DN_ITEM_NO='" + txbDn_Item_No.Text.Trim() + "'"; //query them  dn_item_no

                    if (txbModel_Name.Text.Trim() != "")
                        query4 +=  " AND MODEL_NAME = '" + txbModel_Name.Text.Trim() + "'"; //query them model_name
                    string queryupdate = "UPDATE SFISM4.R_SAP_DN_DETAIL_T SET SHOW_FLAG= 'Y'" +
                     ",material_group='" + empInfo.EmployeeNo.ToString() + "'" +
                     ",create_date=sysdate" + " WHERE 1=1 " + query4;
                  await  dbsfis.ExcuteNonQuerySQL(queryupdate, sfcClient);
                    SortSAPData();
                    MessageBox.Show("Modify OK", "Message");
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void btnHide_Click(object sender, EventArgs e)
        {
            try
            {
                SortSAPData();
                if (dtgvSap.Rows.Count <= 1)
                {
                    MessageBox.Show("Warning: no data for operate");
                    btnShow.Enabled = false;
                    btnHide.Enabled = false;
                    return;
                }
                if (!string.IsNullOrWhiteSpace(txbDn_No.Text) || !string.IsNullOrWhiteSpace(txbSo_Number.Text) || !string.IsNullOrWhiteSpace(txbModel_Name.Text) || !string.IsNullOrWhiteSpace(txbDn_Item_No.Text) || !string.IsNullOrWhiteSpace(txbCust_Po.Text))
                {

                    string query4 = "";

                    if (txbDn_No.Text.Trim() != "")
                        query4 +=  " AND DN_NO='" + txbDn_No.Text.Trim() + "'"; //select them dn_no

                    if (txbCust_Po.Text.Trim() != "")
                        query4 +=  " AND CUST_PO='" + txbCust_Po.Text.Trim() + "'"; // select them cust po

                    if (txbSo_Number.Text.Trim() != "")
                        query4 +=  " AND SO_NUMBER='" + txbSo_Number.Text.Trim() + "'"; //query them dk so_number

                    if (txbDn_Item_No.Text.Trim() != "")
                        query4 += " AND DN_ITEM_NO='" + txbDn_Item_No.Text.Trim() + "'"; //query them  dn_item_no

                    if (txbModel_Name.Text.Trim() != "")
                        query4 +=  " AND MODEL_NAME = '" + txbModel_Name.Text.Trim() + "'"; //query them model_name
                    string queryupdate = "UPDATE SFISM4.R_SAP_DN_DETAIL_T SET SHOW_FLAG= 'N'" +
                     ",material_group='" + empInfo.EmployeeNo.ToString() + "'" +
                     ",create_date=sysdate" + " WHERE 1=1 " + query4;
                    await dbsfis.ExcuteNonQuerySQL(queryupdate, sfcClient);
                    SortSAPData();
                    MessageBox.Show("Modify OK", "Message");
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void txbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (txbSearch.Text == "")
                {
                    SFCMessage.Show("Error | Lỗi", "Please Input Invoice NO. !!", "Vui lòng nhập vào Invoice NO");
                }
            }
        }

        private void txbDn_No_KeyDown(object sender, KeyEventArgs e)
        {
            btnShow.Enabled = false;
            btnHide.Enabled = false;
        }

        private void txbDn_No_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txbDn_Item_No.Focus();
            }
        }

        private void txbDn_Item_No_KeyDown(object sender, KeyEventArgs e)
        {
            btnShow.Enabled = false;
            btnHide.Enabled = false;
        }

        private void txbDn_Item_No_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txbSo_Number.Focus();
            }
        }

        private void txbSo_Number_KeyDown(object sender, KeyEventArgs e)
        {
            btnShow.Enabled = false;
            btnHide.Enabled = false;
        }

        private void txbSo_Number_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txbCust_Po.Focus();
            }
        }

        private void txbCust_Po_KeyDown(object sender, KeyEventArgs e)
        {
            btnShow.Enabled = false;
            btnHide.Enabled = false;
        }

        private void txbCust_Po_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txbModel_Name.Focus();
            }
        }

        private void txbModel_Name_KeyDown(object sender, KeyEventArgs e)
        {
            btnShow.Enabled = false;
            btnHide.Enabled = false;
        }

        private void txbModel_Name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnQuery.Focus();
            }
        }

        
        
        private void ExportDgvToXML()
        {
            System.Data.DataView dv = null;
            if (tabControl1.SelectedIndex == 1)
            {
               dv = (System.Data.DataView)dtgvDnItem.DataSource;
            }
            else if(tabControl1.SelectedIndex==2)
            {
               dv = (System.Data.DataView)dtgvDbCheck.DataSource;
            }
            
            System.Data.DataTable dt = dv.Table;
            dt.TableName = "DATA";
            SaveFileDialog sfd = new SaveFileDialog();
            //sfd.Filter = "XML|*.xml";
            sfd.Filter = "Excel Files (2003)|*.xls|Excel Files (2007)|*.xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    dt.WriteXml(sfd.FileName);
                    MessageBox.Show("Export Successfully", "Notice");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        
        

        private void btnSaveToExcel_Click_1(object sender, EventArgs e)
        {
            //ExportFileExcel(dtgvDnItem);
            ExportDgvToXML();

        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            //ExportFileExcel(dtgvDbCheck);
            ExportDgvToXML();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                PrintSAPData();
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            fDetail f = new fDetail();
            f.empInfo = this.empInfo;
            //f.dbsfis = this.dbsfis;
            f.ShowDialog();
        }
    }
}
