using System;
using System.Data;
using System.Windows.Forms;
using LabelManager2;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using Sfc.Library.HttpClient;
using System.Threading.Tasks;
using SFCThales;

namespace MSL_PRINT
{
    public partial class MSL_PRINT : Form
    {
        private ApplicationClass lb;
        string msGroup, msStation, msVersion, tmpSN, msPrintDate, msREV, msREV1, msCustModel, routeResult, roastResult, inputResult, printResult,statusMSL, MslResult;
        public static string EMP_NO;
        SfcHttpClient _sfcHttpClient;
        R107Service fdal;
        private void showVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowVariable frm = new ShowVariable(_dic_list_variable);
            frm.ShowDialog();
        }

        private void visibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisibleForm frm = new VisibleForm(_sfcHttpClient);
            frm._sendVisible = VisibleOpen;
            frm.ShowDialog();
        }
        private void VisibleOpen()
        {
            lb.Visible = true;
        }
        int iOvertimeQty;

        private void rePrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reprint_Form frm = new Reprint_Form(_sfcHttpClient);
            frm.sendRePrint = RePrint;
            frm.ShowDialog();
        }

        Dictionary<string, string> _dic_list_variable = new Dictionary<string, string>()
        {
            ["ModelName"] = "",
            ["MO"] = "",
            ["VERSION"] = "",
            ["QTY"] = "",
            ["ID"] = "",
            ["BOXID"] = "",
            ["REELID"] = "",
            ["DDMMYYYY"] = "",
            ["REV"] = "",
            ["REV1"] = "",
            ["MSLPN"] = "",
        };
        
        
        public MSL_PRINT()
        {
            InitializeComponent();
            _sfcHttpClient =Form1.sfcHttpClient;
            fdal = new R107Service();
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

        private void MSL_PRINT_Load(object sender, EventArgs e)
        {
            msGroup = "PRINT";
            msStation = "PRINT";
        }

        
        private async void txbReelID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (string.IsNullOrEmpty(txbReelID.Text))
                {
                    MessageBox.Show("The value is null！", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txbReelID.SelectAll();
                    return;
                }

                DataTable dt =await QueryReelID(txbReelID.Text);

                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("ReelID does not exist, please confirm!", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txbReelID.SelectAll();
                    return;
                }
                msVersion = dt.Rows[0]["VERSION_CODE"].ToString();
                txbMoNumber.Text = dt.Rows[0]["MO_NUMBER"].ToString();
                txbModelName.Text = dt.Rows[0]["MODEL_NAME"].ToString();
                txbLineName.Text = dt.Rows[0]["LINE_NAME"].ToString();
                txbLastSection.Text = dt.Rows[0]["SECTION_NAME"].ToString();
                txbLastGroup.Text = dt.Rows[0]["MODEL_NAME"].ToString();
                txbWipGroup.Text = dt.Rows[0]["MODEL_NAME"].ToString();

                lbxOverTime.Items.Clear();
                lbxFail.Items.Clear();

               
                //1.CHECK ROUTE
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tmpSN = dt.Rows[i]["SERIAL_NUMBER"].ToString();
                    routeResult =await fdal.ExecCheckRouteProcedure(txbLineName.Text, msGroup, tmpSN, _sfcHttpClient);
                    if (!routeResult.Contains("OK"))
                    {
                        lbxFail.Items.Add(txbReelID.Text);
                        MessageBox.Show(tmpSN + ": " + routeResult, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txbReelID.SelectAll();
                        return;
                    }
                }
                //2.Check MSL config
                DataTable dtModelSerial = await QueryModelDesc(txbModelName.Text);
                if (dtModelSerial.Rows.Count <= 0)
                {
                    MessageBox.Show("No data found MODEL", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txbReelID.SelectAll();
                    return;
                }
                else
                {
                    if (dtModelSerial.Rows[0]["MODEL_SERIAL"].ToString() == "Cinterion")
                    {
                        DataTable dtMSLconfig = await QueryMSLConfig(txbModelName.Text);
                        if (dtMSLconfig.Rows.Count <= 0)
                        {
                            MessageBox.Show("PM not yet setup MSL LABEL CONFIG", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            txbReelID.SelectAll();
                            return;
                        }
                        else
                        {
                            statusMSL = dtMSLconfig.Rows[0]["MODEL_NAME"].ToString();
                            if (statusMSL == "WAITING_LABEL_CONFIRM")
                            {
                                MessageBox.Show("CALL LABELROOM CONFIRM MSL LABEL CONFIG", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                txbReelID.SelectAll();
                                return;
                            }
                            if (statusMSL == "WAITING_PQE_CONFIRM")
                            {
                                MessageBox.Show("CALL PQE CONFIRM MSL LABEL CONFIG", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                txbReelID.SelectAll();
                                return;
                            }
                            if (statusMSL == "REJECT")
                            {
                                MessageBox.Show("PQE or LABELROOM HAS REJECT MSL LABEL CONFIG", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                txbReelID.SelectAll();
                                return;
                            }
                            if (statusMSL == "CONFIRM")
                            {
                                MslResult = dtMSLconfig.Rows[0]["ATTRIBUTE_VALUE"].ToString();
                            }
                        }
                    }
                }

                //3.CHECK ROAST TIME
                iOvertimeQty = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tmpSN = dt.Rows[i]["SERIAL_NUMBER"].ToString();
                    roastResult =await fdal.ExecCheckSNPackTimeProcedure(tmpSN, msGroup, _sfcHttpClient);
                    if (!roastResult.Contains("OK"))
                    {
                        lbxOverTime.Items.Add(tmpSN +"--->"+ roastResult);
                        iOvertimeQty += 1;
                    }
                }

                if (iOvertimeQty > 0)
                {
                    lbxFail.Items.Add(txbReelID.Text);
                    MessageBox.Show(txbReelID.Text + ": Have " + iOvertimeQty + " pcs of product has timed out, please bake again!！", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txbReelID.SelectAll();
                    return;
                }

                DataTable dtCustRule =await GetCustRule(txbModelName.Text, msVersion);
                if (dtCustRule.Rows.Count > 0)
                {
                    msREV = dtCustRule.Rows[0]["REV"].ToString();
                    msREV1 = dtCustRule.Rows[0]["REV1"].ToString();
                    msCustModel = dtCustRule.Rows[0]["CUSTMODELNAME"].ToString();
                }
                //4.PASS STATION
                int x;
                for (x = 0; x < dt.Rows.Count; x++)
                {
                    tmpSN = dt.Rows[x]["SERIAL_NUMBER"].ToString();
                    inputResult =await fdal.ExecR107Procedure(txbLineName.Text, txbLastSection.Text, msGroup, msStation, tmpSN, _sfcHttpClient);
                    if (!inputResult.Contains("OK"))
                    {
                        lbxFail.Items.Add(txbReelID.Text);
                        MessageBox.Show(tmpSN + ": " + inputResult, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txbReelID.SelectAll();
                        return;
                    }
                }
                
                if (x == dt.Rows.Count) //DFV000000001
                {
                    lbxPass.Items.Add(txbReelID.Text);
                    DataTable dtPrintDate =await GetPrintDate(tmpSN);
                    if (dtPrintDate.Rows.Count > 0)
                    {
                        msPrintDate = dtPrintDate.Rows[0]["PRINTDATE"].ToString();
                    }
                    printResult = Print(dt);
                    MessageBox.Show(txbReelID.Text + ": " + printResult, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txbReelID.SelectAll();
                    return;
                }
            }
        }
        private async void RePrint(string _data)
        {
            if (string.IsNullOrEmpty(_data))
            {
                MessageBox.Show("The value is null！", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable dt =await QueryReelID(_data);

            if (dt.Rows.Count <= 0)
            {
                MessageBox.Show("ReelID does not exist, please confirm!", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable dtCustRule =await GetCustRule(dt.Rows[0]["MODEL_NAME"].ToString(), dt.Rows[0]["VERSION_CODE"].ToString());
            if (dtCustRule.Rows.Count > 0)
            {
                msREV = dtCustRule.Rows[0]["REV"].ToString();
                msREV1 = dtCustRule.Rows[0]["REV1"].ToString();
                msCustModel = dtCustRule.Rows[0]["CUSTMODELNAME"].ToString();
            }
            tmpSN = dt.Rows[0]["SERIAL_NUMBER"].ToString();
            DataTable dtPrintDate =await GetPrintDate(tmpSN);
            if (dtPrintDate.Rows.Count > 0)
            {
                msPrintDate = dtPrintDate.Rows[0]["PRINTDATE"].ToString();
            }
            printResult = Re_Print(dt);
                if (printResult.Contains("OK"))
                {
                    // insert print log
                }
                MessageBox.Show(_data + ": " + printResult, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            lbxPass.Items.Clear();
            lbxFail.Items.Clear();
            lbxOverTime.Items.Clear();
            txbReelID.Text = "";
            txbMoNumber.Text = "";
            txbModelName.Text = "";
            txbLineName.Text = "";
            txbLastSection.Text = "";
            txbLastGroup.Text = "";
            txbWipGroup.Text = "";
        }

        public async Task< DataTable> QueryReelID(string sReelID)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO ='{0}'", sReelID);
                dt = await fdal.ExcuteSelectSQL(sql, _sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt;
        }

        public async Task<DataTable> QueryModelDesc(string model_name)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T where MODEL_NAME ='{0}'", model_name);
                dt = await fdal.ExcuteSelectSQL(sql, _sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt;
        }

        public async Task<DataTable> QueryMSLConfig(string model_name)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_MODEL_ATTR_CONFIG_T WHERE ATTRIBUTE_NAME='MSL_LABEL_CONFIG' AND TYPE_VALUE ='{0}'", model_name);
                dt = await fdal.ExcuteSelectSQL(sql, _sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt;
        }

        public async Task< DataTable> GetSYSDATETIME(string sReelID)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT SYSDATE DATETIME, TO_CHAR(SYSDATE,'YYYYMMDD') MO_DATE, TO_CHAR(SYSDATE,'HH24MI') TIME FROM DUAL";
                dt =  await fdal.ExcuteSelectSQL(sql, _sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt;
        }

        public async Task<DataTable> GetCustRule(string sModel, string sVersion)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT DISTINCT VERSION_CODE      AS REV1,
                                               CUST_MODEL_NAME   AS CUSTMODELNAME,
                                               CUST_VERSION_CODE AS REV,
                                               UPCEANDATA        AS HW
                                 FROM SFIS1.C_CUST_SNRULE_T 
                                WHERE MODEL_NAME ='{0}' AND VERSION_CODE ='{0}' ", sModel, sVersion);
               
                dt =await fdal.ExcuteSelectSQL(sql, _sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt;
        }

        public async Task< DataTable> GetPrintDate(string sSN)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT TO_CHAR(MAX(IN_STATION_TIME),'DDMMYYYY') PRINTDATE FROM SFISM4.R_SN_DETAIL_T WHERE SERIAL_NUMBER ='{0}' AND GROUP_NAME='PRINT' ", sSN);
               
                dt =await fdal.ExcuteSelectSQL(sql, _sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt;
        }

        public string Print(DataTable dt)
        {
            string sModel = dt.Rows[0]["MODEL_NAME"].ToString();
            string sVersion = dt.Rows[0]["VERSION_CODE"].ToString();
            string sLabelFileName;
            if (sModel.IndexOf('.') > 0)
            {
                try
                {
                    sLabelFileName = sModel.Replace('.', '_') + "_BAG.LAB";
                }
                catch
                {
                    return "LabelFile名稱轉換異常，請聯繫MIS！";
                }
            }
            else
            {
                sLabelFileName = sModel + "_BAG.LAB";
            }

            try
            {
                Comm.InitLabelBOX(lb, sLabelFileName);
                if (!Comm.userLB.ContainsKey(this.Name))
                {
                    Comm.userLB.Add(this.Name, lb);
                }
            }
            catch
            {
                return "LabelFile is abnormal, please contact Label TE!";
                lb.Quit();
            }

            try
            {
                for (int i = 1; i <= this.lb.ActiveDocument.Variables.FormVariables.Count; i++)
                {
                    switch (this.lb.ActiveDocument.Variables.FormVariables.Item(i).Name.ToString())
                    {
                        case "ModelName":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("ModelName").Value = sModel;
                            _dic_list_variable["ModelName"] = sModel;
                            break;
                        case "MO":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("MO").Value = txbMoNumber.Text;
                            _dic_list_variable["MO"] = txbMoNumber.Text;
                            break;
                        case "VERSION":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("VERSION").Value = sVersion;
                            _dic_list_variable["VERSION"] = sVersion;
                            break;
                        case "QTY":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("QTY").Value = dt.Rows.Count.ToString();
                            _dic_list_variable["QTY"] = dt.Rows.Count.ToString();
                            break;
                        case "ID":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("ID").Value = txbReelID.Text;
                            _dic_list_variable["ID"] = txbReelID.Text;
                            break;
                        case "BOXID":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("BOXID").Value = txbReelID.Text;
                            _dic_list_variable["BOXID"] = txbReelID.Text;
                            break;
                        case "REELID":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("REELID").Value = txbReelID.Text;
                            _dic_list_variable["REELID"] = txbReelID.Text;
                            break;
                        case "DDMMYYYY":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("DDMMYYYY").Value = msPrintDate;
                            _dic_list_variable["DDMMYYYY"] = msPrintDate;
                            break;
                        case "REV":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("REV").Value = msREV;
                            _dic_list_variable["REV"] = msREV;
                            break;
                        case "REV1":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("REV1").Value = msREV1;
                            _dic_list_variable["REV1"] = msREV1;
                            break;
                        case "MSLPN":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("MSLPN").Value = MslResult;
                            _dic_list_variable["MSLPN"] = MslResult;
                            break;
                    }
                }
            }
            catch
            {
                return "The value field does not exist, please contact Label TE!";
            }

            return Comm.PrintTest1(lb);

        }

        public string Re_Print(DataTable dt)
        {
            string sModel = dt.Rows[0]["MODEL_NAME"].ToString();
            string sVersion = dt.Rows[0]["VERSION_CODE"].ToString();
            string sMo_number = dt.Rows[0]["MO_NUMBER"].ToString();
            string Sline = dt.Rows[0]["LINE_NAME"].ToString();
            string sSection = dt.Rows[0]["SECTION_NAME"].ToString();
            string sTray = dt.Rows[0]["TRAY_NO"].ToString();
            string sLabelFileName;
            if (sModel.IndexOf('.') > 0)
            {
                try
                {
                    sLabelFileName = sModel.Replace('.', '_') + "_BAG.LAB";
                }
                catch
                {
                    return "LabelFile名稱轉換異常，請聯繫MIS！";
                }
            }
            else
            {
                sLabelFileName = sModel + "_BAG.LAB";
            }

            try
            {
                Comm.InitLabelBOX(lb, sLabelFileName);
                if (!Comm.userLB.ContainsKey(this.Name))
                {
                    Comm.userLB.Add(this.Name, lb);
                }
            }
            catch(Exception ex)
            {
                return "LabelFile is abnormal, please contact Label TE !!";
                lb.Quit();
            }

            try
            {
                for (int i = 1; i <= this.lb.ActiveDocument.Variables.FormVariables.Count; i++)
                {
                    switch (this.lb.ActiveDocument.Variables.FormVariables.Item(i).Name.ToString())
                    {
                        case "ModelName":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("ModelName").Value = sModel;
                            _dic_list_variable["ModelName"] = sModel;
                            break;
                        case "MO":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("MO").Value = sMo_number;
                            _dic_list_variable["MO"] = sMo_number;
                            break;
                        case "VERSION":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("VERSION").Value = sVersion;
                            _dic_list_variable["VERSION"] = sVersion;
                            break;
                        case "QTY":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("QTY").Value = dt.Rows.Count.ToString();
                            _dic_list_variable["QTY"] = dt.Rows.Count.ToString();
                            break;
                        case "ID":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("ID").Value = sTray;
                            _dic_list_variable["ID"] = sTray;
                            break;
                        case "BOXID":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("BOXID").Value = sTray;
                            _dic_list_variable["BOXID"] = sTray;
                            break;
                        case "REELID":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("REELID").Value = sTray;
                            _dic_list_variable["REELID"] = sTray;
                            break;
                        case "DDMMYYYY":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("DDMMYYYY").Value = msPrintDate;
                            _dic_list_variable["DDMMYYYY"] = msPrintDate;
                            break;
                        case "REV":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("REV").Value = msREV;
                            _dic_list_variable["REV"] = msREV;
                            break;
                        case "REV1":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("REV1").Value = msREV1;
                            _dic_list_variable["REV1"] = msREV1;
                            break;
                        case "MSLPN":
                            this.lb.ActiveDocument.Variables.FormVariables.Item("MSLPN").Value = MslResult;
                            _dic_list_variable["MSLPN"] = MslResult;
                            break;
                    }
                }
            }
            catch
            {
                return "The value field does not exist, please contact Label TE!";
                lb.Quit();
            }

            return Comm.PrintTest1(lb);

        }
    }
}
