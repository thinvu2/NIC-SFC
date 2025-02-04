using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;
using System.Diagnostics;
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;
using System.Linq;
using System.Threading.Tasks;
using Oracle;
using Sfc.Core.Models;
using System.Reflection;
using LabelManager2;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.IO.Ports;
using System.Windows.Threading;
using FG_CHECK.Model;
using FG_CHECK.Resources;

namespace FG_CHECK
{
    public partial class fMainShippingLabel : Form
    {
        bool _ChkMD5Flag = true;
        DataTable dt = new DataTable();
        public String line;
        public String routecode;
        public String empno;
        public string inputLogin, checkSum, baseUrl, empNo, empPass, error_code, CK_DB;
        public string val_carton, val_security1, val_security2, val_license, val_CartonQty;
        public static string val_empNo = string.Empty, val_empName, loginDB, val_emppwd;
        public static Boolean F_visible = false, F_reprint = false, F_LSSC = false;
        public static SfcHttpClient sfcClient;
        public DB _oracle;
        public static string lang = "", ipAddress = "", macAddress = "";//VNI/ENG
        private Ini ini;
        string strProcess = "", downloadfail = "";
        public static bool ChkReprin = false;
        public Boolean PrintOK, print;
        string My_LabelFileName, LabelFileName_ftp, publicfilepath = "";
        string sql, SourceString, G_sLabl_Name;
        public static DataTable dtParams = new DataTable();
        private ApplicationClass lb;
        sfcconnect sfc;
        LabelManager2.Application labApp = null;
        public fMainShippingLabel(SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            sfcClient = _sfcClient;
            ipAddress = Ini.GetLocalIPAddress();
            macAddress = Ini.GetMacAddress();
            txt_emp.Text = Form1.empName;
            txt_emp.Enabled = false;
            txt_ztext.Enabled = false;
            txt_labelqty.Enabled = false;
            txt_finishdate.Enabled = false;
            txt_finishdc.Enabled = false;
            txt_cartoncount.Enabled = false;
            txt_palletcount.Enabled = false;
            txt_custname.Enabled = false;
            txt_custpn.Enabled = false;
            txt_shipaddress.Enabled = false;
            txt_custcode.Enabled = false;
            txt_endcustpo.Enabled = false;
            lerror.Enabled = false;
            val_empNo = Form1.empNO;
            ini = new Ini(System.Windows.Forms.Application.StartupPath + "\\SFIS_AMS.INI");
            lang = ini.IniReadValue("MainSection", "LANG");
            if (lang == "ENG")
            {
                englishToolStripMenuItem.Checked = true;
                tiếngViệtToolStripMenuItem.Checked = false;
            }
            else
            {
                englishToolStripMenuItem.Checked = false;
                tiếngViệtToolStripMenuItem.Checked = true;
            }
            try
            {
                this.lb = new ApplicationClass();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't Connect CodeSoft！.. Please Check.. Thanks", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }
        private async void txtPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            txtPass.PasswordChar = '*';
            if (e.KeyChar == 13)
            {
                string sql = string.Format(@"SELECT*FROM SFIS1.C_EMP_DESC_T WHERE EMP_PASS ='{0}'", txtPass.Text);
                var qry_Data_emp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Data_emp.Data == null)
                {
                    MessageBox.Show("PASS False!");
                }
                else
                {
                    txtPass.PasswordChar = (char)0;
                    txtPass.Text = qry_Data_emp.Data["emp_no"]?.ToString() ?? "";
                    ChkReprin = true;
                    txtPass.Enabled = false;
                    txt_carton.Enabled = true;
                    txt_carton.Focus();
                }
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            ExecuteResult exeRes = new ExecuteResult();
            string data = "IN_FUNC:SHIPPINGLABEL|IN_SUB_FUNC:LABEL3S MCARTON|ACTION:REPRINT|INVOICE:5022110419|MCARTON_NO:M4520C24000000J5|PCMAC:" + macAddress + "|IP:" + ipAddress + "|EMP_NO:V1041643";
            exeRes = await getDataBySP(data);
            if (exeRes.Status == true)
            {
                DataTable dt = new DataTable();
                dt = (DataTable)exeRes.Anything;
                await P_PrintToCodeSoft(dt.Rows[0]["LabelFile"].ToString(), dt);
            }
            else
            {
                setMessage(exeRes.Message);
            }
        }
        private async void txt_DN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                
                ExecuteResult exeRes = new ExecuteResult();
                string DN = txt_dn.Text;
                if(DN != "")
                {
                   // string data = "IN_FUNC:SHIPPINGLABEL|IN_SUB_FUNC:LABEL3S MCARTON|ACTION:CHECKDN|INVOICE:5028029236|MCARTON_NO:AAAAAAAAAA|PCMAC:" + macAddress + "|IP:" + ipAddress + "|EMP_NO:V1041643";
                    string data = "IN_FUNC:SHIPPINGLABEL|IN_SUB_FUNC:LABEL3S MCARTON|ACTION:CHECKDN|INVOICE:" + DN + "|MCARTON_NO:AAAAAAAAAA|PCMAC:" + macAddress + "|IP:" + ipAddress + "|EMP_NO:V1041643";
                    exeRes = await getDataBySP(data);
                    if(exeRes.Status == true)
                    {
                        DataTable dt = new DataTable();
                        dt = (DataTable)exeRes.Anything;
                        if (dt.Rows.Count > 0)
                        {
                            txt_ztext.Text = dt.Rows[0]["ZTEXT"].ToString();
                            txt_labelqty.Text = dt.Rows[0]["LABELQTY"].ToString();
                            txt_finishdate.Text = dt.Rows[0]["FinishDate"].ToString();
                            txt_finishdc.Text = dt.Rows[0]["FinishDC"].ToString();
                            txt_cartoncount.Text = dt.Rows[0]["CartonCount"].ToString();
                            txt_palletcount.Text = dt.Rows[0]["PalletCount"].ToString();
                            txt_custname.Text = dt.Rows[0]["CustName"].ToString();
                            txt_custpn.Text = dt.Rows[0]["CustPN"].ToString();
                            txt_shipaddress.Text = dt.Rows[0]["ShipAddress"].ToString();
                            txt_custcode.Text = dt.Rows[0]["CustCode"].ToString();
                            txt_endcustpo.Text = dt.Rows[0]["EndCustPO"].ToString();
                            txt_dn.Enabled = false;
                        }
                    }
                    else
                    {
                        setMessage(exeRes.Message);
                    }
                }
            }
        }
        private async void txt_carton_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                ExecuteResult exeRes = new ExecuteResult();
                string CTN = txt_carton.Text;
                string DN = txt_dn.Text;
                string data = string.Empty;
                string empreprint = txtPass.Text;
                if (DN != "")
                {
                    if(ChkReprin == true)
                    {
                        // string data = "IN_FUNC:SHIPPINGLABEL|IN_SUB_FUNC:LABEL3S MCARTON|ACTION:CHECKDN|INVOICE:5028029236|MCARTON_NO:AAAAAAAAAA|PCMAC:" + macAddress + "|IP:" + ipAddress + "|EMP_NO:V1041643";
                        data = "IN_FUNC:SHIPPINGLABEL|IN_SUB_FUNC:LABEL3S MCARTON|ACTION:REPRINT|INVOICE:" + DN + "|MCARTON_NO:"+ CTN + "|PCMAC:" + macAddress + "|IP:" + ipAddress + "|EMP_NO:"+ empreprint;
                    }
                    else
                    {
                        data = "IN_FUNC:SHIPPINGLABEL|IN_SUB_FUNC:LABEL3S MCARTON|ACTION:PRINT|INVOICE:" + DN + "|MCARTON_NO:"+ CTN + "|PCMAC:" + macAddress + "|IP:" + ipAddress + "|EMP_NO:"+ Form1.empNO;
                    }
                    exeRes = await getDataBySP(data);
                    if (exeRes.Status == true)
                    {
                        lerror.Text = "OK";
                        DataTable dt = new DataTable();
                        dt = (DataTable)exeRes.Anything;
                        for(int i=0; i< Int32.Parse(dt.Rows[0]["LABELQTY"].ToString()); i++)
                        {
                            await P_PrintToCodeSoft(dt.Rows[0]["LabelFile"].ToString(), dt);
                        }
                    }
                    else
                    {
                        setMessage(exeRes.Message);
                    }
                }
                else
                {
                    setMessage("Please input DN first|Vui lòng nhập mã DN trước");
                    txt_dn.Clear();
                    txt_dn.Focus();
                }
            }
        }

        private void fMainShippingLabel_Load(object sender, EventArgs e)
        {
            if (val_empNo != "V1019802")
            {
                try
                {
                    SfcSubclass(txt_carton.Handle);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    System.Windows.Forms.Application.Exit();
                }
            }
        }
        private async Task<ExecuteResult> getDataBySP(string input)
        {
            ExecuteResult exeRes = new ExecuteResult();
            DataTable dt;
            exeRes.Status = false;

            string OUT_DATA;
            string res;
           
            try
            {
                var resultsp = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.SP_FG",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter {Name = "IN_DATA" ,Value = input, SfcParameterDataType = SfcParameterDataType.Varchar2 , SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter {Name = "OUT_DATA" ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output},
                        new SfcParameter {Name = "RES" ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output}
                    }
                });

                if (resultsp != null)
                {

                    dynamic output = resultsp.Data;
                    OUT_DATA = output[0]["out_data"];
                    res = output[1]["res"];

                    if (res.Contains("OK"))
                    {
                        OUT_DATA = output[0]["out_data"];
                        string test = OUT_DATA.Replace(":", ": '");
                        string test1 = test.Replace("|", "',");
                        string test2 = "{result:[{" + test1 + "'}]}";
                        JObject jObj = JObject.Parse(test2);
                        dt = jObj["result"].ToObject<DataTable>();
                        exeRes.Status = true;
                        exeRes.Anything = dt;
                    }
                    else
                    {
                        exeRes.Message = res;
                    }
                }
            }
            catch (Exception ex)
            {
                exeRes.Message = ex.Message.ToString();
                return exeRes;
            }
            return exeRes;
        }
        
        public void AddParams(DataTable str)
        {
            string[] params1;
            List<string> lstcheckDup = new List<string>();
            try
            {
                dtParams.Clear();
                foreach (DataColumn row in str.Columns)
                {
                    string ColumnName = row.ColumnName.ToString();
                    string dataColumnName = str.Rows[0][ColumnName].ToString();
                    if (dtParams.Columns.Count == 0)
                    {
                        dtParams.Columns.Add("Name");
                        dtParams.Columns.Add("Value");
                    }
                    DataRow[] foundAuthors = dtParams.Select("Name = '" + ColumnName + "'");
                    if (foundAuthors.Length != 0)
                    {
                        MessageBox.Show("IT add param duplicate:" + ColumnName);
                        return;
                    }
                    dtParams.Rows.Add(new object[] { ColumnName, dataColumnName });
                }
            }
            catch (Exception ex)
            {
                //setMessage(ex.Message.ToString() + "|" + ex.Message.ToString());
                return;
            }
        }
        private void rESETToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txt_ztext.Text = "";
            txt_labelqty.Text = "";
            txt_finishdate.Text = "";
            txt_finishdc.Text = "";
            txt_cartoncount.Text = "";
            txt_palletcount.Text = "";
            txt_custname.Text = "";
            txt_custpn.Text = "";
            txt_shipaddress.Text = "";
            txt_custcode.Text = "";
            txt_endcustpo.Text = "";
            txt_dn.Text = "";
            txt_carton.Text = "";
            ChkReprin = false;
            txtPass.Text = "";
        }

        private async void visibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(My_LabelFileName))
            {
                ApplicationClass labApp = new ApplicationClass();
                try
                {
                    string checkp =string.Format(@"select*from SFIS1.C_PRIVILEGE where emp ='{0}' and fun = 'VISIBLE' and rownum =1", val_empNo);
                    var qry_Data_emp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = checkp,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Data_emp.Data == null)
                    {
                        MessageBox.Show("PRIVILEGE False!");
                        return;
                    }
                    string actionDesc = "Visible MCartonNo:" + txt_carton.Text + ";" + ipAddress + ";" + macAddress;
                    string sqlSaveLog = "insert into SFISM4.R_SYSTEM_PRGLOG_T(emp_no,prg_name,action_type,action_desc,time) values ('" + val_empNo + "','CHK_LICENSE','VISIBLE','" + actionDesc + "',sysdate)";
                    var b = await sfcClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sqlSaveLog, SfcCommandType = SfcCommandType.Text });
                    //---------------

                    labApp.Documents.Open(My_LabelFileName, false);
                    Document doc = labApp.ActiveDocument;
                    doc.Application.Visible = true;
                }
                catch (Exception ex)
                {
                    setMessage(ex.Message.ToString() + "|" + ex.Message.ToString());
                    return;
                }
            }
            else
            {
                setMessage("Can not find Label file!|Không tìm thấy tệp label!");
                return;
            }
        }
        public async Task P_PrintToCodeSoft(string paramLabelFile, DataTable dt)
        {
            try
            {
                AddParams(dt);
                LabelFileName_ftp = paramLabelFile;
                My_LabelFileName = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\" + paramLabelFile;

                publicfilepath = My_LabelFileName;

                Comm.InitLabelBOX(lb, LabelFileName_ftp);


                Document doc = lb.ActiveDocument;
                bool _chkprint = await CallCodesoftToPrint(doc, LabelFileName_ftp, dtParams);
                if (_chkprint)
                {
                    PrintOK = true;
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public async Task<bool> CallCodesoftToPrint(Document doctemp, string LabelName, DataTable DT)
        {
            string _param_Name = string.Empty, _labName;
            string cTEST = string.Empty;
            try
            {
                LabelTracking _lbt = new LabelTracking();
                _lbt.sfcHttpClient = sfcClient;

                //Check MD5 of label file
                if (_ChkMD5Flag)
                {
                    if (!await _lbt.doMD5Label(LabelName, 7, true))
                    {
                        return false;
                    }
                    _ChkMD5Flag = false;
                }
                for (int i = 1; i < doctemp.Variables.FormVariables.Count + 1; i++)
                {
                    _labName = doctemp.Variables.FormVariables.Item(i).Name;
                    doctemp.Variables.FormVariables.Item(_labName).Value = "";
                }
                // Set value into label file
                foreach (DataRow param in dtParams.Rows)
                {
                    _param_Name = param["Name"].ToString().ToUpper();
                    string vl = param["Value"].ToString();
                    try
                    {
                        for (int i = 1; i < doctemp.Variables.FormVariables.Count + 1; i++)
                        {
                            if (doctemp.Variables.FormVariables.Item(i).Name.ToUpper() == _param_Name)
                            {
                                doctemp.Variables.FormVariables.Item(_param_Name).CounterUse = 0;
                                doctemp.Variables.FormVariables.Item(_param_Name).Value = param["Value"]?.ToString() ?? "";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                // Get value of label items to compare with label tracking
                DataTable dtbVarName = new DataTable();
                dtbVarName.Columns.Add("VAR_NAME", typeof(string));
                dtbVarName.Columns.Add("VALUE", typeof(string));
                dtbVarName.Columns.Add("TYPE", typeof(string));
                int TotalBarcode = doctemp.DocObjects.Barcodes.Count;
                int TotalText = doctemp.DocObjects.Texts.Count;
                int TotalVar = doctemp.Variables.FormVariables.Count;
                int TotalFreeVar = doctemp.Variables.FreeVariables.Count;
                int TotalFomula = doctemp.Variables.Formulas.Count;

                for (int i = 1; i <= TotalText; i++)
                {
                    var _name = doctemp.DocObjects.Texts.Item(i).Name;
                    if (_name != null)
                    {
                        var _var = doctemp.DocObjects.Texts.Item(i).VariableName;
                        var _data = doctemp.DocObjects.Texts.Item(i).Value;
                        var _font = doctemp.DocObjects.Texts.Item(i).Font.Name;
                        dtbVarName.Rows.Add(new object[] { _name, _data, _font });
                    }
                }

                for (int i = 1; i <= TotalBarcode; i++)
                {
                    var _name = doctemp.DocObjects.Barcodes.Item(i).Name;
                    if (_name != null)
                    {
                        var _var = doctemp.DocObjects.Barcodes.Item(i).VariableName;
                        var _bar = doctemp.DocObjects.Barcodes.Item(i).Symbology.ToString().Replace("lppx", "").Trim();
                        var _data = doctemp.DocObjects.Barcodes.Item(i).Value;
                        dtbVarName.Rows.Add(new object[] { _name, _data, _bar });
                    }
                }
                doctemp.Save();
                doctemp.PrintDocument(0);
                //Call label tracking
                try
                {
                    if (!await _lbt.doLabelTracking_AddParamValue(LabelName, 7, dtParams, doctemp, dtbVarName, true))
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                int printQTY = 1;
                doctemp.PrintDocument(printQTY);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> downloadlabel(string labelname)
        {
            string ftpaddress, TMPDIR, ftpuser, ftppassword, LocalDir;
            string DestinationString, sModel_name = "Z_FG_LABEL";
            LocalDir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\" + labelname;
            string ftplabelpath;

            try
            {
                string sql = string.Format("SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME ='{0}'", sModel_name);
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });

                if (_result.Data != null)
                {
                    ftpaddress = _result.Data["model_serial"].ToString();
                    ftplabelpath = _result.Data["customer"].ToString();
                    SourceString = "http://" + ftpaddress + ftplabelpath + "//" + labelname;
                    if (!await DownLoadInternetFile(SourceString, LocalDir))
                    {
                        downloadfail = SourceString;

                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }

        private void showParamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmParam frm = new frmParam(dtParams);
            frm.ShowDialog();
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tiếngViệtToolStripMenuItem.Checked = false;
            englishToolStripMenuItem.Checked = true;
            lang = "ENG";
            try { ini.IniWriteValue("MainSection", "LANG", "ENG"); } catch { }
        }

        private void tiếngViệtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tiếngViệtToolStripMenuItem.Checked = true;
            englishToolStripMenuItem.Checked = false;
            lang = "VNI";
            try { ini.IniWriteValue("MainSection", "LANG", "VNI"); } catch { }
        }

        public async Task<bool> DownLoadInternetFile(string Source, string Dest)
        {
            if (!File.Exists(Dest))
            {
                try
                {
                    WebClient wc = new WebClient();
                    wc.DownloadFile(Source, Dest);
                    return true;
                }
                catch (Exception ex)
                {
                    downloadfail = Source;
                    return false;
                }
            }
            return false;
        }

        private void reprintToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (reprintToolStripMenuItem1.Checked = true)
            {
                reprintToolStripMenuItem1.CheckState = CheckState.Checked;
                lPass.Visible = true;
                txtPass.Visible = true;
                txtPass.Focus();
                txtPass.Enabled = true;
                txt_carton.Enabled = false;
            }
            else
            {
                reprintToolStripMenuItem1.CheckState = CheckState.Checked;
                lPass.Visible = false;
                txtPass.Visible = false;
                txtPass.Text = "";
                txtPass.Visible = false;
                lPass.Visible = false;
            }
        }

        public void Form1_FormClosing(object sender, FormClosedEventArgs e)
        {
            try
            {
                killprocess();
                labApp = new LabelManager2.Application();
                String _DirPath = System.Windows.Forms.Application.StartupPath + @"\";
                foreach (string sFile in System.IO.Directory.GetFiles(_DirPath, "*.LAB"))
                {
                    File.Delete(sFile);
                }
            }
            catch(Exception ex)
            {
                setMessage(ex.Message);
            }
            finally
            {
                //Environment.Exit(0);
            }
        }
        public void setMessage(string ms)
        {
            string Emsg = "";
            string Vmsg = "";
            if(ms.IndexOf("|") > 0)
            {
                Emsg = ms.Split('|')[0];
                Vmsg = ms.Split('|')[1];
            }
            else
            {
                Emsg = ms;
                Vmsg = ms;
            }
            if (lang == "ENG") lerror.Text = Emsg;
            else               lerror.Text = Vmsg;
        }

        private async Task<bool> killprocess()
        {
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("lppa"))
            {
                process.Kill();
            }
            return true;
        }
    }
}
