using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using xNet;
using System.Xml;
using System.Xml.Serialization;
using ShippingFile;
using System.Linq;
using ShippingFile.Model;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Web;
using System.Net.Http;
using System.Xml.Linq;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Threading;
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;

namespace ShippingFile
{
    public partial class ShipingFile : Form
    {
        private string FileName;
        public string path;
        Task threadGetTime;
        public static SfcHttpClient _sfcHttpClient;
        DAL fDal;
        public ShipingFile(SfcHttpClient varsfcHttpClient)
        {
            InitializeComponent();
            _sfcHttpClient = varsfcHttpClient;
            fDal = new DAL();
            CheckForIllegalCrossThreadCalls = false;
           
        }
        public  void conne33csfc2()
        {
           

        }

        private async void btnSelect_Click(object sender, EventArgs e)
        {
            string sql_check = "";
            DataTable dt_check = new DataTable();
            if (txtDN.Text.Trim() == "")
            {
                return;
            }
           

            DataTable dt_model = new DataTable();
            string sql_getModel =string.Format(@"SELECT MODEL_NAME FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE =
                                                '{0}'",txtDN.Text);
            dt_model = await fDal.ExecuteSQL(sql_getModel, _sfcHttpClient);
            string model_name = "";
            if (dt_model.Rows.Count > 0)
            {
                model_name = dt_model.Rows[0][0].ToString();
            }
            else
            {
                MessageBox.Show("DN have not exist. \nPlease check", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            sql_check = "SELECT * FROM SFIS1.C_CINTERION_SHIP_T WHERE model_name = '" + model_name + "'";
            dt_check = await fDal.ExecuteSQL(sql_check, _sfcHttpClient);
            if (dt_check.Rows.Count == 0)
            {
                MessageBox.Show("PC haven't set up data for this model "+ model_name + ". \nPlease check HW_CONFIG", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            sql_check = "select * from sfism4.r105 where mo_number in (SELECT distinct mo_number FROM SFISm4.z107 WHERE ship_no in (SELECT TCOM FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE = '" + txtDN.Text + "'))";
            dt_check = await fDal.ExecuteSQL(sql_check, _sfcHttpClient);
            if (dt_check.Rows.Count >  0)
            {
                string mo = dt_check.Rows[0]["MO_NUMBER"].ToString();
                string hw_bom = dt_check.Rows[0]["HW_BOM"].ToString();
                string sw_bom = dt_check.Rows[0]["SW_BOM"].ToString();
                if (string.IsNullOrEmpty(hw_bom))
                {
                    MessageBox.Show("PC haven't set up data HW_BOM for this MO " + mo + ". \nPlease check HW_CONFIG", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrEmpty(sw_bom))
                {
                    MessageBox.Show("PC haven't set up data SW_BOM for this MO " + mo + ". \nPlease check HW_CONFIG", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string check_po = $"select * from SFIS1.C_PO_CONFIG_T where MODEL_NAME = '{txtDN.Text.Trim()}'";
            dt_check = await fDal.ExecuteSQL(check_po, _sfcHttpClient);
            if (dt_check.Rows.Count < 1)
            {
                MessageBox.Show("DN haven't set up data CONFIG for this DN: " + txtDN.Text.Trim() + ". \nPlease check PO_CONFIG. Call PM", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable dt = null;
            if (rbtnTXT.Checked) //txt格式
            {
                dt =await fDal.GetShipInfoByDN(txtDN.Text.Trim(), _sfcHttpClient);
                if (dt.Rows.Count > 0)
                {
                    FileName = dt.Rows[0]["CUST_PO"].ToString() + "_" + dt.Rows[0]["AT_CUSTOMIZATION_FILENAME"].ToString();
                    dt.Columns.Remove("CUST_PO");
                }
            }
            else if (rbtnXML.Checked)
            {
                dt = await fDal.GetShipXMLByDN (txtDN.Text.Trim(), model_name,txtInvoice.Text, _sfcHttpClient);

                //add new radiobutton get name file with XML file
                FileName = "XML" + System.DateTime.Now + "Data";

            }
            else //csv格式
            {
                dt = await fDal.GetShipXMLByDN(txtDN.Text.Trim(), model_name, txtInvoice.Text, _sfcHttpClient);
            }

            dataGridView1.DataSource = dt;
            //lblNnumber.Text = dt.Rows.Count.ToString();
        }


        //Tham add new format filer XML custom
        public void ExportDataGridViewToTxt(SaveFileDialog save, DataTable dt)
        {
            Stream myStream = save.OpenFile();
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));

            try
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    string tempStr = "";

                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        tempStr += dt.Rows[j][k].ToString() + "\t";
                    }
                    sw.WriteLine(tempStr);
                }
                sw.Close();
                myStream.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("ExportData Error");
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }
        }

        public void ExportXMLFile(DataTable dt)
        {

            if (dt.Rows.Count > 0)
            {
                SaveFileDialog save = new SaveFileDialog();
                save.FilterIndex = 0;
                save.RestoreDirectory = true;

                save.Filter = "txt files(*.txt)|*.txt";
                save.Title = "Save a File";
                save.FileName = "GUL_FOV_" + DateTime.Now.ToString("yyyyMMdd_HH24mmss") + ".txt";
                if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (save.FileName.Contains(".txt"))
                    {
                        ExportDataGridViewToTxt(save, dt);
                        btnUpLoad.Enabled = true;
                    }
                    #region 將數據寫入 xml 中，modify by 張官軍
                    var fileName = save.FileName.Replace("txt", "xml").Replace("xls", "xml").Replace("xlsx", "xml");
                    lblPath.Text = fileName;
                    string xmlString = string.Empty;
                    GULFile file = new GULFile();
                    file.Header = null;
                    file.Body = new List<Module>();

                    var moduleProperties = typeof(Module).GetProperties();
                    var headerProperties = typeof(Header).GetProperties();
                    string propertyName = string.Empty;
                    List<string> Columns = new List<string>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        Columns.Add(dc.ColumnName.ToUpper());
                    }

                    //写入xml文件
                    foreach (DataRow row in dt.Rows)
                    {
                        if (file.Header == null)
                        {
                            Header header = new Header();
                            foreach (var property in headerProperties)
                            {
                                var attribute = property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() as ColumnAttribute;
                                if (attribute != null)
                                {
                                    propertyName = attribute.Name;
                                }
                                else
                                {
                                    propertyName = property.Name;
                                }
                                try
                                {
                                    if (Columns.Contains(propertyName))
                                    {
                                        if (row[propertyName].ToString().Length > 0)
                                        {
                                            property.SetValue(header, row[propertyName].ToString(), null);
                                        }
                                    }

                                }
                                catch
                                {
                                    continue;
                                }
                            }
                            file.Header = header;
                        }

                        Module module = new Module();
                        foreach (var property in moduleProperties)
                        {
                            var attribute = property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() as ColumnAttribute;
                            if (attribute != null)
                            {
                                propertyName = attribute.Name;
                            }
                            else
                            {
                                propertyName = property.Name;
                            }
                            try
                            {
                                if (Columns.Contains(propertyName))
                                {
                                    if (row[propertyName].ToString().Length > 0)
                                    {
                                        property.SetValue(module, row[propertyName].ToString(), null);
                                    }
                                    //else
                                    //{
                                    //    //如果栏位的内容是空,则xml中要去掉此栏位,如果不加这个ELSE
                                    //    //例如这个BoardNo栏位的内容是空,则xml中会显示:<BoardNo />
                                    //    Columns.Remove(propertyName);
                                    //}
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        file.Body.Add(module);
                    }

                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(GULFile));
                    XmlWriterSettings xws = new XmlWriterSettings();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        settings.Encoding = Encoding.UTF8;
                        using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            MemoryStream stream = new MemoryStream();
                            using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
                            {
                                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                                namespaces.Add(string.Empty, string.Empty);
                                xmlSerializer.Serialize(xmlWriter, file, namespaces);
                            }

                            BinaryWriter bw = new BinaryWriter(fs);
                            var result = Encoding.UTF8.GetString(stream.ToArray());
                            result = result.Replace("<GULFile>", "<GULFile xmlns=\"https://transfer.cinterion-shop.com/tracedata/FileUpload.aspx\">");
                            bw.Write(Encoding.UTF8.GetBytes(result));
                            bw.Flush();
                            bw.Close();
                            stream.Close();
                        }
                    }
                    #endregion
                    MessageBox.Show("Export OK！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("as", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }

        //Tham add new format file XLM export 2021-02-23
        private void ExportDataGridViewToXML(SaveFileDialog dialog, DataTable dt)
        {
            DataTable data = dt;
            SaveFileDialog sfd = dialog;
            sfd.Filter = "XML|*.xml";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    dt.WriteXml(sfd.FileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

        }

        private async void btnExcel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                DataTable dt = dataGridView1.DataSource as DataTable;

                SaveFileDialog save = new SaveFileDialog();
                save.FilterIndex = 0;
                save.RestoreDirectory = true;

                save.InitialDirectory = System.Windows.Forms.Application.ExecutablePath;
                if (rbtnXML.Checked)
                {
                    string check = await fDal.CheckFileSP(txtDN.Text.Trim(), _sfcHttpClient);
                    string checkmodel = await fDal.CheckModel(txtDN.Text.Trim(), _sfcHttpClient);
                    //if (checkmodel == "T99W565.02" || checkmodel == "95.3443T00" || checkmodel =="PLS63-W-R1.1" || checkmodel == "BGS2-E REL.4.5" || checkmodel.StartsWith("TX62-WC"))
                    if (checkmodel.StartsWith("TX62-WC") || checkmodel == "T99W565.02" || checkmodel.StartsWith("T99W321"))
                    {
                        check = "OK";
                    }
                    //string check = "OK";

                    if (!check.Equals("OK"))
                    {
                        MessageBox.Show(check);
                    }
                    else
                    {
                        if (await fDal.GetCountZ107(txtDN.Text.Trim(), _sfcHttpClient) != dataGridView1.RowCount)
                        {
                            MessageBox.Show("Please check Sum product in DN:" + fDal.GetCountZ107(txtDN.Text.Trim(), _sfcHttpClient) + "---Sum products correct in file: " + dataGridView1.RowCount);
                            return;
                        }
                        else
                        {

                            //if (checkmodel == "T99W565.02" || checkmodel == "95.3443T00" || checkmodel == "PLS63-W-R1.1" || checkmodel == "TX62-WC BOARDA")
                            if (checkmodel.StartsWith("TX62-WC")|| checkmodel == "T99W565.02"|| checkmodel.StartsWith("T99W321.0"))
                            {
                                goto Nocheck;
                            }

                            DataRow[] dr1 = dt.Select("PO_NO IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("PO_NO IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr2 = dt.Select("IMEI IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("IMEI IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr3 = dt.Select("TRAY_NO IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("TRAY_NO IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr4 = dt.Select("MCARTON_NO IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("MCARTON_NO IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            DataRow[] dr5 = dt.Select("MATNOFACTORY IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("MATNOFACTORY IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr6 = dt.Select("ACCKEY IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("ACCKEY IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr7 = dt.Select("BAG_TIME IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("BAG_TIME IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr8 = dt.Select("PRODUCT_DC IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("PRODUCT_DC IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr9 = dt.Select("INFO04 IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("INFO04 IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr10 = dt.Select("INFO05 IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("INFO05 IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr11 = dt.Select("EAN IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("EAN IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr12 = dt.Select("INFO06 IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("INFO06 IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr13 = dt.Select("PART_NUMBER_SHIELD IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("PART_NUMBER_SHIELD IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr14 = dt.Select("CUST_MODEL_NAME IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("CUST_MODEL_NAME IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr15 = dt.Select("REVISION IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("REVISION IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr16 = dt.Select("PANEL_NO IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("PANEL_NO IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr17 = dt.Select("SW IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("SW_BOM IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr18 = dt.Select("PART_NUMBER_CUSTOMER IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("PART_NUMBER_CUSTOMER IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            //DataRow[] dr19 = dt.Select("PACKING_DATE1 IS NULL");
                            //if (dr1.Length > 0)
                            //{
                            //    MessageBox.Show("PACKING_DATE1 IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //    return;
                            //}
                            DataRow[] dr20 = dt.Select("TEST_LINE IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("TEST_LINE IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr21 = dt.Select("PACKING_DATE IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("PACKING_DATE IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr22 = dt.Select("DNP IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("DNP IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            DataRow[] dr23 = dt.Select("FT_DATE IS NULL");
                            if (dr1.Length > 0)
                            {
                                MessageBox.Show("FT_DATE IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            Nocheck:
                            ExportXMLFile(dt);
                        }
                    }

                }
                else if (rbtnTXT.Checked) //txt格式
                {
                    save.Filter = "|*.txt";
                    save.Title = "Save a File";
                    //CUST_PO
                    save.FileName = "FCS0" + FileName + ".txt";
                    if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ExportDataGridViewToExcel(save);
                        lblPath.Text = save.FileName;
                    }
                }
                else
                {
                    string checkmodel = await fDal.CheckModel(txtDN.Text.Trim(), _sfcHttpClient);
                    if (checkmodel == "T99W565.02")
                    {
                        goto Nocheck_T99w565;
                    }
                    
                    DataRow[] dr1 = dt.Select("PO_NO IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("PO_NO IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr2 = dt.Select("IMEI IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("IMEI IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr3 = dt.Select("TRAY_NO IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("TRAY_NO IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr4 = dt.Select("MCARTON_NO IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("MCARTON_NO IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DataRow[] dr5 = dt.Select("MATNOFACTORY IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("MATNOFACTORY IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr6 = dt.Select("ACCKEY IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("ACCKEY IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr7 = dt.Select("BAG_TIME IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("BAG_TIME IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr8 = dt.Select("PRODUCT_DC IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("PRODUCT_DC IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr9 = dt.Select("INFO04 IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("INFO04 IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr10 = dt.Select("INFO05 IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("INFO05 IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr11 = dt.Select("EAN IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("EAN IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr12 = dt.Select("INFO06 IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("INFO06 IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr13 = dt.Select("PART_NUMBER_SHIELD IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("PART_NUMBER_SHIELD IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr14 = dt.Select("CUST_MODEL_NAME IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("CUST_MODEL_NAME IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr15 = dt.Select("REVISION IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("REVISION IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr16 = dt.Select("PANEL_NO IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("PANEL_NO IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr17 = dt.Select("SW IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("SW_BOM IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr18 = dt.Select("PART_NUMBER_CUSTOMER IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("PART_NUMBER_CUSTOMER IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    //DataRow[] dr19 = dt.Select("PACKING_DATE1 IS NULL");
                    //if (dr1.Length > 0)
                    //{
                    //    MessageBox.Show("PACKING_DATE1 IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}
                    DataRow[] dr20 = dt.Select("TEST_LINE IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("TEST_LINE IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr21 = dt.Select("PACKING_DATE IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("PACKING_DATE IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr22 = dt.Select("DNP IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("DNP IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DataRow[] dr23 = dt.Select("FT_DATE IS NULL");
                    if (dr1.Length > 0)
                    {
                        MessageBox.Show("FT_DATE IS NULL Cannot Export file！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Nocheck_T99w565:

                    save.Filter = "|*.csv";
                    save.Title = "Save a File";
                    //CUST_PO
                    save.FileName = "FCS0" + FileName + ".txt";
                    save.FileName = "DF_" + dt.Rows[0]["DN_NO"].ToString() + "_" + txtDN.Text.Trim() + "_" + DateTime.Now.ToString("yyyyMMdd_HH24mmss") + ".csv";
                    if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ExportDataGridViewToCSV(save, dt);
                    }
                }
                lblNnumber.Text = null;
            }
        }

        public void ExportDataGridViewToExcel(SaveFileDialog save)
        {
            Stream myStream = save.OpenFile();
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));

            try
            {
                for (int j = 0; j < dataGridView1.Rows.Count; j++)
                {
                    string tempStr = "";

                    for (int k = 0; k < dataGridView1.Columns.Count; k++)
                    {
                        tempStr += dataGridView1.Rows[j].Cells[k].Value.ToString() + ",";
                    }
                    tempStr = tempStr.Remove(tempStr.Length - 1);
                    sw.WriteLine(tempStr);
                }
                sw.Close();
                myStream.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("ExportData Error");
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }
        }


        //Vi Van Tham edit new format CSV file 2021-01-30
        public void ExportDataGridViewToCSV(SaveFileDialog save, DataTable dt)
        {
            Stream myStream = save.OpenFile();

            //Vi Van Tham edit new format CSV file 2021-01-30
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            //headers    
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sw.Write(dt.Columns[i]);
                if (i < dt.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(String.Format("\t{0}", dr[i].ToString()));
                        }
                    }
                    if (i < dt.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
            MessageBox.Show("Export Success!", "Message");

        }


        private const string ChromeAppKey = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

        public void startTime()
        {
            Thread.Sleep(1000);
        }

        public async void HTTP_GET()
        {
            timer1.Enabled = true;
            var userName = "fd-prod";
            var passwd = "Gem#Fgs4-Pr45";
            var url = "https://transfer.cinterion-shop.com/tracedata/fileupload.aspx";

            string filePath = path;

            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = System.Net.Http.HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri(url);

            var streamContent = new System.Net.Http.StreamContent(File.Open(filePath, FileMode.Open));


            var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));

            var content = new MultipartFormDataContent();

            content.Add(streamContent, "File1", Path.GetFileName(filePath));

            // Thêm vào MultipartFormDataContent một StringContent

            try
            {

                //Thread t = new Thread(new ThreadStart(startTime));

                var response = await httpClient.PostAsync(new Uri(url), content);
                var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(responseContent);

                if (responseContent.Contains("The file has been uploaded"))
                {
                    MessageBox.Show("Upload success!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    timer1.Enabled = false;
                    label_textEx.Visible = false;
                }
                else
                {
                    MessageBox.Show("Upload fail, please check!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    timer1.Enabled = false;
                    label_textEx.Visible = false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Upload Fail! Please check" + e.Message);
                timer1.Enabled = false;
                label_textEx.Visible = false;
            }
            finally
            {
                MessageBox.Show("Upload Fail! Please check");
                timer1.Enabled = false;
                label_textEx.Visible = false;
            }
        }

        private void btnUpLoad_Click(object sender, EventArgs e)
        {

            string fileName = lblPath.Text;
            path = fileName;
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Cannot upload File. Please check！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                timer1.Enabled = true;
                Task t = new Task(HTTP_GET);
                t.Start();
            }


            //upload file to website customer's

            //if (lblPath.Text.Trim() == null || lblPath.Text.Trim() == "")
            //{
            //    MessageBox.Show("Cannot upload File. Please check！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            //string path = lblPath.Text;
            ////string CompressFilePath = path.Replace(".xml", ".zip");
            //string url = string.Format(@"curl -k -u fd-prod:Gem#Fgs4-Pr45 --form File1=@{0} --form press=Upload https://transfer.cinterion-shop.com/tracedata/FileUpload.aspx", path);

            ////string url = string.Format(@"curl -k -u fd-prod:Gem#Fgs4-Pr45 -F File1=@{0} https://transfer.cinterion-shop.com/tracedata/FileUpload.aspx", path);
            ////string url = string.Format(@"curl -k -u foxconn:Z45a12#qay58 -F File1=@{0} https://transfer.cinterion-shop.com/FoxcFabrd/FileUpload.aspx", path);
            ////xml格式文件上傳測試地址 https://transfer.cinterion-shop.com/tracedata-test/FileUpload.aspx
            ////xml格式文件上傳正式地址 fd-prod:Gem#Fgs4-Pr45 https://transfer.cinterion-shop.com/tracedata/FileUpload.aspx
            /////zip格式文件上傳正式地址 foxconn:Z45a12#qay58 https://transfer.cinterion-shop.com/FoxcFabrd/FileUpload.aspx
            ////CreateZipFile(path, path.Replace(".txt", ".zip"));
            ////modify by champion 20200508 只上傳xml格式的壓縮文件
            ////CreateZipFile(path.Replace(".txt", ".xml"), CompressFilePath);
            ////string result = Execute(url);
            //string result = Execute("curl -i https://transfer.cinterion-shop.com/tracedata/FileUpload.aspx");
            //if (result.Contains("The file has been uploaded"))
            //{
            //    //判断文件是不是存在
            //    //if (File.Exists(path))
            //    //{
            //    //    //如果存在则删除
            //    //    File.Delete(path);
            //    //}
            //    MessageBox.Show("Upload success！", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    lblPath.Text = null;
            //    btnUpLoad.Enabled = false;
            //}
            //else
            //{
            //    //判断文件是不是存在
            //    if (File.Exists(path) || File.Exists(path.Replace(".txt", ".xml")))
            //    {
            //        //如果存在则删除
            //        File.Delete(path);
            //        File.Delete(path.Replace(".txt", ".xml"));
            //        File.Delete(path.Replace(".txt", ".zip"));
            //    }

            //    lblPath.Text = null;
            //    MessageBox.Show("Upload fail！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}


        }

        /// <summary>
        /// Writes multi part HTTP POST request. Author : Farhan Ghumra
        /// </summary>
        private void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData)
        {
            /// The first boundary
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            /// the last boundary.
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            /// the form data, properly formatted
            string formdataTemplate = "Content-Dis-data; name=\"{0}\"\r\n\r\n{1}";
            /// the form-data file upload, properly formatted
            string fileheaderTemplate = "Content-Dis-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

            /// Added to track if we need a CRLF or not.
            bool bNeedsCRLF = false;

            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    /// if we need to drop a CRLF, do that.
                    if (bNeedsCRLF)
                        WriteToStream(s, "\r\n");

                    /// Write the boundary.
                    WriteToStream(s, boundarybytes);

                    /// Write the key.
                    WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    bNeedsCRLF = true;
                }
            }

            /// If we don't have keys, we don't need a crlf.
            if (bNeedsCRLF)
                WriteToStream(s, "\r\n");

            WriteToStream(s, boundarybytes);
            WriteToStream(s, string.Format(fileheaderTemplate, "file", fileName, fileContentType));
            /// Write the file data to the stream.
            WriteToStream(s, fileData);
            WriteToStream(s, trailer);
        }

        /// <summary>
        /// Writes string to stream. Author : Farhan Ghumra
        /// </summary>
        private void WriteToStream(Stream s, string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes byte array to stream. Author : Farhan Ghumra
        /// </summary>
        private void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Returns byte array from StorageFile. Author : Farhan Ghumra
        /// </summary>
        private async Task<byte[]> GetBytesAsync(string file_name)
        {
            string filename = @"c:\Temp\userinputlog.txt";
            byte[] result;

            using (FileStream SourceStream = File.Open(filename, FileMode.Open))
            {
                result = new byte[SourceStream.Length];
                await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
            }

            // UserInput.Text = System.Text.Encoding.ASCII.GetString(result);

            return result;
        }

        public static async Task<string> Upload(byte[] image, String fileName)
        {
            using (var client = new HttpClient())
            {
                using (var content =
                    new MultipartFormDataContent(fileName))
                {
                    content.Add(new System.Net.Http.StreamContent(new MemoryStream(image)), "bilddatei", "upload.jpg");

                    using (
                       var message =
                           await client.PostAsync("http://www.directupload.net/index.php?mode=upload", content))
                    {
                        var input = await message.Content.ReadAsStringAsync();

                        return !string.IsNullOrWhiteSpace(input) ? Regex.Match(input, @"http://\w*\.directupload\.net/images/\d*/\w*\.[a-z]{3}").Value : null;
                    }
                }
            }
        }

        public static string Execute(string command)
        {
            int seconds = 0;
            string output = ""; //输出字符串   
            if (command != null && !command.Equals(""))
            {
                Process process = new Process();//创建进程对象
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令   
                startInfo.Arguments = "/c " + command;
                //+ " /c";//“/C”表示执行完命令后马上退出   
                //startInfo.Arguments = command;
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动 
                startInfo.RedirectStandardInput = true;//不重定向输入   
                startInfo.RedirectStandardOutput = true; //重定向输出   
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = false;//不创建窗口   
                process.StartInfo = startInfo;
                seconds = 5000;
                try
                {
                    if (process.Start())//开始进程
                    {
                        while (!process.StandardOutput.EndOfStream)
                        {
                            string line = process.StandardOutput.ReadLine();
                            Console.WriteLine(line);
                            // do something with line
                        }

                        string err = process.StandardError.ReadToEnd();
                        Console.WriteLine(err);

                        StreamReader sr = process.StandardOutput;

                        output = sr.ReadToEnd();//读取进程的输出 

                        Console.WriteLine(output);

                        //if (seconds == 0)
                        //{
                        process.WaitForExit();//这里无限等待进程结束
                                              //}
                                              //else
                                              //{
                                              //    process.WaitForExit(seconds); //等待进程结束，等待时间为执行等待的毫秒
                                              //}




                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally
                {
                    if (process != null)
                        process.Close();
                }
            }
            return output;
        }

        private static void CreateZipFile(string filesPath, string zipFilePath)
        {

        }

        private void Upload(string fileName)
        {
            //upload file with httpsrequest
            Login("http://10.224.81.136:98/Home", null);
        }

        public string GetLoginDataToken(String html)
        {
            string token = "";

            var res = Regex.Matches(html, @"(?<=__RequestVerificationToken"" type=""hidden"" value="").*?(?="")", RegexOptions.Singleline);
            if (res != null && res.Count > 0)
            {
                token = res[1].ToString();
            }

            return token;
        }

        public void Login(string url, string cookie = null)
        {
            xNet.HttpRequest http = new xNet.HttpRequest();
            http.Cookies = new CookieDictionary();
            string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.82 Safari/537.36";

            var html = GetData(url, http, UserAgent);

            //string token = GetLoginDataToken(html);

            string data = "__EVENTTARGET=&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=%2FwEPDwUKMTEyMjcwMDI1NQ9kFgICAw9kFggCBw8QDxYGHg1EYXRhVGV4dEZpZWxkBQVTaGlmdB4ORGF0YVZhbHVlRmllbGQFBVNoaWZ0HgtfIURhdGFCb3VuZGdkEBUDA0RheQVOaWdodAAVAwNEYXkFTmlnaHQAFCsDA2dnZ2RkAgkPEA8WBh8ABQdGYWN0b3J5HwEFB0ZhY3RvcnkfAmdkEBUHAkdXA0JOMwJHWgtHWi8gTmV0d29yawtHWi8gQWxscGFydAdHWi8gU0ZDABUHAkdXA0JOMwJHWgtHWi8gTmV0d29yawtHWi8gQWxscGFydAdHWi8gU0ZDABQrAwdnZ2dnZ2dnFgFmZAILDxAPFgIeB0VuYWJsZWRoZGQWAGQCEA8WAh4LXyFJdGVtQ291bnQCBxYOAgEPZBYCZg8VBwhWMDkzOTk5NR%2FpmK7mlofpoa8vIE5ndXnhu4VuIFbEg24gSGnhu4NuETIwNjY0LyAwMzY2MTgxMjM4AkdXCyAyMDIxLTAyLTA4CEIwMy8gU0ZDA0RheWQCAg9kFgJmDxUHCFYwOTkxMjkwH%2BmYruW6reWOmi9OZ3V54buFbiDEkMOsbmggSOG6rXURMjA2NTkvIDA5NjQwMDMwNDcCR1cLIDIwMjEtMDItMDgIQjA0LyBTRkMDRGF5ZAIDD2QWAmYPFQcIVjA5OTQ3NzQg5qKB6YCy5b6LLyBMxrDGoW5nIFRp4bq%2FbiBMdeG6rXQRMjY0ODYvIDAzNDYzMDU0NzUCR1cLIDIwMjEtMDItMDgIQjA2LyBTRkMDRGF5ZAIED2QWAmYPFQcIVjA5Nzk1MDMa6Kix5rCP546yL0jhu6lhIFRo4buLIExpbmgRMjY1NzYvIDA5Njk1NjA5OTECR1cLIDIwMjEtMDItMDgIQzAzLyBTRkMDRGF5ZAIFD2QWAmYPFQcIVjA5Mzk5OTce5LiB5rCP6IqzL8SQaW5oIFRo4buLIFBoxrDGoW5nETI2NjU1LyAwOTYxNTEzMjk1AkdXCyAyMDIxLTAyLTA4DEIwNi8gQWxscGFydANEYXlkAgYPZBYCZg8VBwhWMDk2MDg1MhjorJ3ntq3oiIgvVOG6oSBEdXkgSMawbmcRMjc2NjMvIDAzODMwNTI0ODgCR1cLIDIwMjEtMDItMDgIQjA0LyBORVQDRGF5ZAIHD2QWAmYPFQcIVjA5NzE5NTgZ55WZ5paH5YipL0zGsHUgVsSDbiBM4bujaREyNjcxOS8gMDk2OTA3NDI5MAJHVwsgMjAyMS0wMi0wOAhDMDMvIE5FVANEYXlkZDM8t02wGzzbj5WEfWsZENKlB4SrNG1xFL6TFqLW5gRI&__VIEWSTATEGENERATOR=8D0E13E6&__EVENTVALIDATION=%2FwEdAA5o3xQhxrs2lrULYt683Gcaewm7r%2FS7r0qw%2BqzUV%2BfTcnstl8mzFKpaLmprjGPr7rzNL1vv1fCqddfcgXqmuq56YhVX7bpWYVdv89Aq91zazCoWkENnhpOUCoy7QdUCMoFqpG9jz%2By2RWpdbOUGHt5Wpw6AswKOwr9ih6ZByROABhCaIiuHzrsv790waUYJ2AwA7eD1jT29ncKkjEHXcAFNMqcmyeDG%2FA1MErp39O0LyfTJKQFXlGmXdrZVZ5%2Fxnov3bGDxObEINqOB3lDqODjSjtTdVzRZn7DFyWrI8V%2FOY3suSg3%2FBzdRPDOJLqYNrXZAQz5QcX1oC7NYjWtmKMGy&txtNameCn=&txtDayDuty=2021-03-15&drlShift=Night&drlFacBig=GZ&btnSearch=%E6%90%9C%E7%B4%A2+%2F+T%C3%ACm+ki%E1%BA%BFm";//coppydata login to here

            html = PostData(http, "http://10.224.81.136:98/Home", data, "application/x-www-form-urlencoded").ToString();

            //Process.Start();

            html = GetData("http://10.224.81.136:98/Home", http, UserAgent).ToString();

        }


        void AddCookie(xNet.HttpRequest http, string cookie)
        {

            //Dictionary<string, string> cookie_temp = new Dictionary<string, string>();

            var temp = cookie.Split(';');
            foreach (var item in temp)
            {
                var temp2 = item.Split('=');
                if (temp2.Count() > 1)
                {
                    http.Cookies.Add(temp2[0], temp2[1]);

                }
            }
        }

        public void UploadFile(string path)
        {
            xNet.MultipartContent data = new xNet.MultipartContent() {

                //add data stringcontent here
                //format data in the browser preview response data JSON
                {new xNet.StringContent("key1"), "value1" },
                {new xNet.StringContent("key2"), "value2" },
                {new xNet.StringContent("key3"), "value3" },
                {new xNet.StringContent("key4"), "value4" },
                {new xNet.StringContent("key5"), "value5" },
                //add filecontent here
                {new xNet.FileContent(""/*here input file path*/), "key6"/*input attribute in the browser*/, ""/*input file name*/ +Path.GetFileName(path)}
            };

            var html = PostFile(null/*if have httprequest you can use get it from top*/, ""/*input url here*/, data);

            JsonConvert.DeserializeObject(html);

        }

        //use FormUrlEncodedContent
        static async Task FormUrlEncodedContent(string fileName)
        {

            /***
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = System.Net.Http.HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri("http://10.224.69.100/SFC/maHanhVi");

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("ctl00$MainContent$Text_modelName", "asdasda"));

            parameters.Add(new KeyValuePair<string, string>("ctl00$MainContent$Text_station", "FFFFFFFFFFFFFFFFFT"));
           // parameters.Add(new KeyValuePair<string, string>("key2", "value2-2"));

            // Thiết lập Content
            var content = new System.Net.Http.FormUrlEncodedContent(parameters);
            httpRequestMessage.Content = content;

            // Thực hiện Post
            var response = await httpClient.SendAsync(httpRequestMessage);

            var responseContent = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseContent);


            MessageBox.Show(responseContent,"apbsc");
            // Khi chạy kết quả trả về cho biết Server đã nhận được dữ liệu Post đến
            */

            //test cach 2

            /*
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                   { "ctl00$MainContent$Text_modelName", "cai dit cu may" },
                   { "ctl00$MainContent$Text_station", "test nhnu con ca" }
                };

                var content = new System.Net.Http.FormUrlEncodedContent(values);

                var response = await client.PostAsync("http://10.224.69.100/SFC/maHanhVi", content);

                var responseString = await response.Content.ReadAsStringAsync();

                MessageBox.Show(responseString, "apbsc");*/

            //Test upload File
            /*
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = System.Net.Http.HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri("http://10.224.69.100/SFC/maHanhVi");

            //--------------------------------------------------------------
            //var client = new HttpClient();
           // const string weblinkUrl = "http://testserver.com/attach?";
           // var content = new MultipartFormDataContent();
            //string fileName = "C:\file.txt";
            var streamContent = new System.Net.Http.StreamContent(File.Open(fileName, FileMode.Open));
            //content.Add(streamContent, "filename");


            // Tạo đối tượng MultipartFormDataContent
            var content = new MultipartFormDataContent();

            // Tạo StreamContent chứa nội dung file upload, sau đó đưa vào content
            //Stream fileStream = System.IO.File.OpenRead("Program.cs");
             content.Add(streamContent, "fileupload", "abc.xyz");

            // Thêm vào MultipartFormDataContent một StringContent
            content.Add(new System.Net.Http.StringContent("value1"), "key1");
            // Thêm phần tử chứa mạng giá trị (HTML Multi Select)
            content.Add(new System.Net.Http.StringContent("value2-1"), "key2[]");
            content.Add(new System.Net.Http.StringContent("value2-2"), "key2[]");


            httpRequestMessage.Content = content;
            var response = await httpClient.SendAsync(httpRequestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);

            */

        }

        void TestLogin()
        {
            //TEST LOGIN
            // first, request the login form to get the viewstate value
            //HttpWebRequest webRequest = WebRequest.Create("http://10.224.69.100/sfc/Login") as HttpWebRequest;
            /*
            StreamReader responseReader = new StreamReader(
                    webRequest.GetResponse().GetResponseStream()
                );
            string responseData = responseReader.ReadToEnd();
            responseReader.Close();
           
            // extract the viewstate value and build out POST data
            string viewState = ExtractViewState(responseData);
            string postData =
                    String.Format(
                    "__VIEWSTATE={0}&Username={1}&Password={2}&LoginButton=Login",
                    viewState, "V0991292", "V0991292"
                    );

            // have a cookie container ready to receive the forms auth cookie
            CookieContainer cookies = new CookieContainer();

            // now post to the login form
            webRequest = WebRequest.Create("http://10.224.69.100/sfc/Login") as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            //webRequest.CookieContainer = cookies;

            // write the form values into the request message
            StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(postData);
            requestWriter.Close();

            // we don't need the contents of the response, just the cookie it issues
            //webRequest.GetResponse().Close();

            // now we can send out cookie along with a request for the protected page
            webRequest = WebRequest.Create("http://10.224.69.100/sfc/Login") as HttpWebRequest;
            webRequest.CookieContainer = cookies;
            responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());

            // and read the response
            responseData = responseReader.ReadToEnd();
            responseReader.Close();

            MessageBox.Show(responseData, "abc");

            */
            //Get channel ID and date
            // ChannelItem channel = cmbChannel.SelectedItem as ChannelItem;
            //DateTime date = datePicker.Value;

            //Set up a request object
            // WebRequest request = WebRequest.Create("http://10.224.69.100/sfc/Login");

            //Specify the method: POST or GET
            //request.Method = "POST";

            //data we want to send is included here
            /* ctl00$DropDownList_db: 10.224.134.98:1527:SFCODB
             ctl00$MainContent$UserName: V0991292
             ctl00$MainContent$Password: yr564
             ctl00$MainContent$Button_Login: Log in
             */

            /* string data = string.Format("ctl00$DropDownList_db={0}&ctl00$MainContent$UserName={1}&ctl00$MainContent$Password={2}&ctl00$MainContent$Button_Login={3}", "10.224.134.98:1527:SFCODB", "V0991292", "123456","Log in" );

             //convert the data into byte array
             byte[] byteArray = Encoding.UTF8.GetBytes(data);

             //set Content-Length and Content-Type 
             request.ContentLength = byteArray.Length;
             request.ContentType = "application/x-www-form-urlencoded";

             //get the request stream and write data to it and close it
             StreamWriter dataStream = new StreamWriter(request.GetRequestStream());
             dataStream.Write(data);
             dataStream.Close();

             //Now the real request is called here when we call the GetResponse method
             WebResponse response = request.GetResponse();
             StreamReader responseReader = new StreamReader(
                      request.GetResponse().GetResponseStream()
                  );
             string responseData = responseReader.ReadToEnd();
             responseReader.Close();

             //Get the data from the Response


             MessageBox.Show(responseData, "asdas");

            // var responseData = dataStream;

             //Open stream using XMLTextReader
             // XmlTextReader reader = new XmlTextReader(dataStream);*/


        }

        public void TestLog()
        {
            xNet.HttpRequest request = new xNet.HttpRequest();

        }

        //EXTRACT
        string ExtractViewState(string s)
        {
            string viewStateNameDelimiter = "__VIEWSTATE";
            string valueDelimiter = "value=\"";

            int viewStateNamePosition = s.IndexOf(viewStateNameDelimiter);
            int viewStateValuePosition = s.IndexOf(
                    valueDelimiter, viewStateNamePosition
                );

            int viewStateStartPosition = viewStateValuePosition +
                                        valueDelimiter.Length;
            int viewStateEndPosition = s.IndexOf("\"", viewStateStartPosition);

            return HttpUtility.UrlEncodeUnicode(
                    s.Substring(
                        viewStateStartPosition,
                        viewStateEndPosition - viewStateStartPosition
                    )
                    );
        }

        public string GetData(string URL, xNet.HttpRequest http = null, string userAgent = "", string cookie = null)
        {
            if (http == null)
            {
                http = new xNet.HttpRequest();
                http.Cookies = new CookieDictionary();
            }

            if (!string.IsNullOrEmpty(cookie))
            {
                AddCookie(http, cookie);
            }

            if (!string.IsNullOrEmpty(userAgent))
            {
                http.UserAgent = userAgent;
            }
            string html = http.Get(URL).ToString();
            return html;

        }

        public string PostData(xNet.HttpRequest http, string url, string data = null, string contentType = null, string userAgent = "", string cookie = null)
        {
            //var userName = "";
            //var passwd = "";

            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            //           Convert.ToBase64String(authToken));



            if (http == null)
            {
                http = new xNet.HttpRequest();
                http.Cookies = new CookieDictionary();
            }

            if (!string.IsNullOrEmpty(cookie))
            {
                AddCookie(http, cookie);
            }

            if (!string.IsNullOrEmpty(userAgent))
            {
                http.UserAgent = userAgent;
            }
            string html = http.Post(url, data, contentType).ToString();
            return html;

        }

        public string PostFile(xNet.HttpRequest http, string url, xNet.MultipartContent data = null, string contentType = null, string userAgent = "", string cookie = null)
        {
            if (http == null)
            {
                http = new xNet.HttpRequest();
                http.Cookies = new CookieDictionary();
            }

            if (!string.IsNullOrEmpty(cookie))
            {
                AddCookie(http, cookie);
            }

            if (!string.IsNullOrEmpty(userAgent))
            {
                http.UserAgent = userAgent;
            }
            string html = http.Post(url, data).ToString();
            return html;

        }

        /* public void PostData(HttpRequest)
         {

         }**/


        private void txtDN_TextChanged(object sender, EventArgs e)
        {
            
        }

        int i = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            label_textEx.Visible = true;
            i++;
            threadGetTime = new Task(() =>
            {
                //label_textEx.Text = "Please wait.... " + i.ToString();
                getTimeExecute(i);
            });
            threadGetTime.Start();
        }

        private void getTimeExecute(int i)
        {
            label_textEx.Text = "Please wait.... " + i;
        }

        private async void ShipingFile_Load(object sender, EventArgs e)
        {

            DataTable dt_model = new DataTable();
            string sql_getver = "select * from sfism4.AMS_AP where ap_name = 'ShippingFile'";
            dt_model = await fDal.ExecuteSQL( sql_getver, _sfcHttpClient);
            string Ver = "";
            if (dt_model.Rows.Count > 0)
            {
                Ver = dt_model.Rows[0]["AP_VERSION"].ToString();
                if (Ver != Application.ProductVersion)
                {
                    MessageBox.Show("Version not same, Please update to new version " + Ver);
                    Environment.Exit(0);
                }
            }
            label_textEx.Visible = false;
            timer1.Enabled = false;
            txtDN.Focus();
            this.WindowState = FormWindowState.Normal;
        }

        private void ShipingFile_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void txtDN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==13)
            {
                txtInvoice.SelectAll();
                txtInvoice.Focus();
            }
        }
    }

    //add new model here
}
