using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;
using ClosedXML.Excel;
using Newtonsoft.Json;
using System.IO;

namespace BRCM_B2B
{
    public partial class Query : Form
    {
        DAL fDal;
        SfcHttpClient sfcClient;
        public string sqlShip = "",sqlWipC="",sqlBDSN="",sqlShipCfm="";
        public string modelSerial = "";
        public DataTable dt,dt1, dt2, dt3, dt4, dt5;

        

        private async void Query_Load(object sender, EventArgs e)
        {
            queryByDNToolStripMenuItem1_Click(sender, e);
            dt = new DataTable();
            dt = await fDal.ExcuteSelectSQL("select  a.invoice,result,finish_date as ship_time,a.create_date as last_run_time from  sfism4.R_EDI_KN_NDC_T a,SFISM4.R_BPCS_INVOICE_T b where hawb='0' and a.invoice=b.invoice(+) order by ship_time desc", sfcClient);
            dataGridView5.DataSource = dt;
        }
        private async void queryByTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            queryByDNToolStripMenuItem1.Checked = false;
            queryByTimeToolStripMenuItem.Checked = true;
            groupBox1.Text = "by Time : ";
            txtDN.Visible = false;
            txtDN.Focus();
            //---------------------------------------------
            cbxModelSerial.Visible = true;
            cbxPN.Visible = true;
            dtTimeFrom.Visible = true;
            dtTimeTo.Visible = true;
            btnQuery.Visible = true;
            //-----------------------------------------------
            cbxModelSerial.Items.Clear();
            dt = new DataTable();
            dt = await fDal.ExcuteSelectSQL("select distinct groupcode from SFISM4.R_SHIPPING_DATA order by groupcode", sfcClient);
            for(int i=0;i<dt.Rows.Count;i++)
            {
                cbxModelSerial.Items.Add(dt.Rows[i][0].ToString());
            }
            cbxModelSerial.SelectedIndex = 0;
            cbxPN.Items.Clear();
            dt = new DataTable();
            dt = await fDal.ExcuteSelectSQL("select 0 as row_num, 'ALL' as custpn from dual  union select* from(select distinct ROW_NUMBER() OVER(ORDER BY custpn) row_num,custpn from( select distinct custpn from SFISM4.R_SHIPPING_DATA) )", sfcClient);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                cbxPN.Items.Add(dt.Rows[i][1].ToString());
            }
            cbxPN.SelectedIndex = 0;
        }

        private void queryByDNToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            queryByDNToolStripMenuItem1.Checked = true;
            queryByTimeToolStripMenuItem.Checked = false;
            groupBox1.Text = "by DN : ";
            txtDN.Visible = true;
            txtDN.Focus();
            //---------------------------------------------
            cbxModelSerial.Visible = false;
            cbxPN.Visible = false;
            dtTimeFrom.Visible = false;
            dtTimeTo.Visible = false;
            btnQuery.Visible = false;
            //---------------------------------------------
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dt1==null || dt2 == null || dt3 == null  || dt4 == null)
            {
                MessageBox.Show("No data for export");
                return;
            }
            if (dt1.Rows.Count == 0&& dt2.Rows.Count == 0&& dt3.Rows.Count == 0&& dt4.Rows.Count == 0)
            {
                MessageBox.Show("No data for export");
                return;
            }
            XLWorkbook wb = new XLWorkbook();
            if (dt1.Rows.Count > 0)
            {
                wb.Worksheets.Add(dt1, "SHIP");
                string path = Directory.GetCurrentDirectory() + "\\Files\\QueryFileDat\\SHIP";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string creatime = DateTime.Now.ToString("yyyyMMdd");
                path += "\\SHIPFILE_" + creatime + ".dat";

                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();

                if (dt1.Rows.Count == 1 && dt1.Rows[0][0].ToString() == "0")
                {
                    dt1.Clear();
                }
                else
                {
                    for (int j = 0; j < dt1.Rows.Count; j++)
                    {
                        string str = "";
                        for (int k = 0; k < dt1.Columns.Count; k++)
                        {
                            str += dt1.Rows[j][k].ToString();
                            if (k != dt1.Columns.Count - 1) str += "|";
                        }
                        stw.WriteLine(str);
                    }
                }
                stw.WriteLine("CTL|" + dt1.Rows.Count.ToString());
                stw.Flush();
                stw.Close();
            }
            if (dt2.Rows.Count > 0)
            {
                wb.Worksheets.Add(dt2, "WIPC");
                string path = Directory.GetCurrentDirectory() + "\\Files\\QueryFileDat\\WIPC";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string creatime = DateTime.Now.ToString("yyyyMMdd");
                path += "\\SHIPFILE_" + creatime + ".dat";

                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();

                if (dt2.Rows.Count == 1 && dt2.Rows[0][0].ToString() == "0")
                {
                    dt2.Clear();
                }
                else
                {
                    for (int j = 0; j < dt2.Rows.Count; j++)
                    {
                        string str = "";
                        for (int k = 0; k < dt2.Columns.Count; k++)
                        {
                            str += dt2.Rows[j][k].ToString();
                            if (k != dt2.Columns.Count - 1) str += "|";
                        }
                        stw.WriteLine(str);
                    }
                }
                stw.WriteLine("CTL|" + dt2.Rows.Count.ToString());
                stw.Flush();
                stw.Close();
            }
            if (dt3.Rows.Count > 0) 
            {
                wb.Worksheets.Add(dt3, "BDSN");
                string path = Directory.GetCurrentDirectory() + "\\Files\\QueryFileDat\\BDSN";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string creatime = DateTime.Now.ToString("yyyyMMdd");
                path += "\\SHIPFILE_" + creatime + ".dat";

                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                if (dt3.Rows.Count == 1 && dt3.Rows[0][0].ToString() == "0")
                {
                    dt3.Clear();
                }
                else
                {
                    for (int j = 0; j < dt3.Rows.Count; j++)
                    {
                        string str = "";
                        for (int k = 0; k < dt3.Columns.Count; k++)
                        {
                            str += dt3.Rows[j][k].ToString();
                            if (k != dt3.Columns.Count - 1) str += "|";
                        }
                        stw.WriteLine(str);
                    }
                }
                stw.WriteLine("CTL|" + dt3.Rows.Count.ToString());
                stw.Flush();
                stw.Close();
            }

            if (dt4.Rows.Count > 0)
            {
                wb.Worksheets.Add(dt4, "SHPCFM");
                string path = Directory.GetCurrentDirectory() + "\\Files\\QueryFileDat\\SHPCFM";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string creatime = DateTime.Now.ToString("yyyyMMdd");
                path += "\\SHIPFILE_" + creatime + ".dat";

                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                if (dt4.Rows.Count == 1 && dt4.Rows[0][0].ToString() == "0")
                {
                    dt4.Clear();
                }
                else
                {
                    for (int j = 0; j < dt4.Rows.Count; j++)
                    {
                        string str = "";
                        for (int k = 0; k < dt4.Columns.Count; k++)
                        {
                            str += dt4.Rows[j][k].ToString();
                            if (k != dt4.Columns.Count - 1) str += "|";
                        }
                        stw.WriteLine(str);
                    }
                }
                stw.WriteLine("CTL|" + dt4.Rows.Count.ToString());
                stw.Flush();
                stw.Close();
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.DefaultExt = "xlsx";
            saveFileDialog1.Filter = "Excel |*.xlsx|All files (*.*)|*.*";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.Title = "Where do you want to save the file?";
            saveFileDialog1.InitialDirectory = @"C:/";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    wb.SaveAs(saveFileDialog1.FileName);
                    MessageBox.Show("Save OK: " + saveFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("You hit cancel or closed the dialog.");
            }
            saveFileDialog1.Dispose();
            saveFileDialog1 = null;
        }
        private async void txtDN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==13)
            {
                string DN = txtDN.Text.Trim();
                /*
                DataTable dataSP = new DataTable();
                List<SfcParameter> ListPara;
                ListPara = new List<SfcParameter>()
                {
                new SfcParameter { Name = "in_dn", Value = DN, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }
                };

                dataSP = await fDal.ExcuteSP("SFIS1.GET_SHIPPINGFILE", ListPara, sfcClient);
                if (!dataSP.Rows[0]["res"].ToString().StartsWith("OK"))
                {
                    MessageBox.Show(dataSP.Rows[0]["res"].ToString());
                    return;
                }
                else
                {
                    modelSerial = dataSP.Rows[0]["res"].ToString().Split('|')[1];
                }*/
                await ClearData();
                dt1 = await Z.getFile(DN, "", "", "", "", "SHIP", sfcClient);
                dt2 = await Z.getFile(DN, "", "", "", "", "WIPC", sfcClient);
                dt3 = await Z.getFile(DN, "", "", "", "", "BDSN", sfcClient);
                dt4 = await Z.getFile(DN, "", "", "", "", "SHPCFM", sfcClient);

                await FillData();
            }
        }

        zFunction Z = new zFunction();
        public Query(SfcHttpClient varsfcClient)
        {
            fDal = new DAL();
            sfcClient = varsfcClient;
            InitializeComponent();
        }

        private async void btnQuery_Click(object sender, EventArgs e)
        {
            string strPN = cbxPN.SelectedItem.ToString();
            string strFrom = dtTimeFrom.Value.ToString("yyyyMMdd") + "000000";
            string strTo = dtTimeTo.Value.ToString("yyyyMMdd") + "235959";
            string strGr = cbxModelSerial.SelectedItem.ToString();
            await ClearData();

            dt1 = await Z.getFile("", strPN, strFrom, strTo, strGr, "SHIP", sfcClient);
            dt2 = await Z.getFile("", strPN, strFrom, strTo, strGr, "WIPC", sfcClient);
            dt3 = await Z.getFile("", strPN, strFrom, strTo, strGr, "BDSN", sfcClient);
            dt4 = await Z.getFile("", strPN, strFrom, strTo, strGr, "SHPCFM", sfcClient);
            await FillData();
            
        }
        public async Task FillData()
        {
            dataGridView1.DataSource = dt1;
            dataGridView2.DataSource = dt2;
            dataGridView3.DataSource = dt3;
            dataGridView4.DataSource = dt4;

            lblShipQty.Text = "Rows total : "+dt1.Rows.Count.ToString();
            lblWipCQty.Text = "Rows total : " + dt2.Rows.Count.ToString();
            lblBDSNQty.Text = "Rows total : " + dt3.Rows.Count.ToString();
            lblShpCfmQty.Text = "Rows total : " + dt4.Rows.Count.ToString();
        }
        public async Task ClearData()
        {
            dt1 = null; dt2 = null; dt3 = null; dt4 = null;
            dataGridView1.DataSource = dt1;
            dataGridView2.DataSource = dt2;
            dataGridView3.DataSource = dt3;
            dataGridView4.DataSource = dt4;
            dataGridView1.Refresh();

            lblShipQty.Text = "Rows total : 0" ;
            lblWipCQty.Text = "Rows total : 0" ;
            lblBDSNQty.Text = "Rows total : 0" ;
            lblShpCfmQty.Text = "Rows total : 0" ;
        }
    }
}
