using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using LabelManager2;
using System.Net;
using System.Collections.Generic;

namespace MSL_PRINT
{
    public partial class MSL_PRINT : Form
    {
        private ApplicationClass lb;
        string msGroup, msStation, msVersion, tmpSN, msPrintDate, msREV, msREV1, msCustModel, routeResult, roastResult, inputResult, printResult;

        private void showVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowVariable frm = new ShowVariable(_dic_list_variable);
            frm.ShowDialog();
        }

        private void visibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisibleForm frm = new VisibleForm();
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
            Reprint_Form frm = new Reprint_Form();
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
        };
        
        public MSL_PRINT()
        {
            InitializeComponent();
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

        
        private void txbReelID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (string.IsNullOrEmpty(txbReelID.Text))
                {
                    MessageBox.Show("The value is null！", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txbReelID.SelectAll();
                    return;
                }

                DataTable dt = QueryReelID(txbReelID.Text);

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
                    routeResult = R107Service.ExecCheckRouteProcedure(txbLineName.Text, msGroup, tmpSN);
                    if (!routeResult.Contains("OK"))
                    {
                        lbxFail.Items.Add(txbReelID.Text);
                        MessageBox.Show(tmpSN + ": " + routeResult, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txbReelID.SelectAll();
                        return;
                    }
                }

                //2.CHECK ROAST TIME
                iOvertimeQty = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tmpSN = dt.Rows[i]["SERIAL_NUMBER"].ToString();
                    roastResult = R107Service.ExecCheckSNPackTimeProcedure(tmpSN, msGroup);
                    if (!roastResult.Contains("OK"))
                    {
                        lbxOverTime.Items.Add(tmpSN);
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

                DataTable dtCustRule = GetCustRule(txbModelName.Text, msVersion);
                if (dtCustRule.Rows.Count > 0)
                {
                    msREV = dtCustRule.Rows[0]["REV"].ToString();
                    msREV1 = dtCustRule.Rows[0]["REV1"].ToString();
                    msCustModel = dtCustRule.Rows[0]["CUSTMODELNAME"].ToString();
                }
                //3.PASS STATION
                int x;
                for (x = 0; x < dt.Rows.Count; x++)
                {
                    tmpSN = dt.Rows[x]["SERIAL_NUMBER"].ToString();
                    inputResult = R107Service.ExecR107Procedure(txbLineName.Text, txbLastSection.Text, msGroup, msStation, tmpSN);
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
                    DataTable dtPrintDate = GetPrintDate(tmpSN);
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
        private void RePrint(string _data)
        {
            if (string.IsNullOrEmpty(_data))
            {
                MessageBox.Show("The value is null！", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable dt = QueryReelID(_data);

            if (dt.Rows.Count <= 0)
            {
                MessageBox.Show("ReelID does not exist, please confirm!", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
           
            printResult = Print(dt);
            if(printResult.Contains("OK"))
            {
                // insert print log
            }
            MessageBox.Show(_data + ": " + printResult, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        public static DataTable QueryReelID(string sReelID)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO =:REELID";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":REELID", sReelID)
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable GetSYSDATETIME(string sReelID)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT SYSDATE DATETIME, TO_CHAR(SYSDATE,'YYYYMMDD') MO_DATE, TO_CHAR(SYSDATE,'HH24MI') TIME FROM DUAL";
                dt = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable GetCustRule(string sModel, string sVersion)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT DISTINCT VERSION_CODE      AS REV1,
                                               CUST_MODEL_NAME   AS CUSTMODELNAME,
                                               CUST_VERSION_CODE AS REV,
                                               UPCEANDATA        AS HW
                                 FROM SFIS1.C_CUST_SNRULE_T 
                                WHERE MODEL_NAME =:MODEL AND VERSION_CODE =:VERSION ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":MODEL", sModel),
                     new OracleParameter(":VERSION", sVersion)
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable GetPrintDate(string sSN)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT TO_CHAR(MAX(IN_STATION_TIME),'DDMMYYYY') PRINTDATE FROM SFISM4.R_SN_DETAIL_T WHERE SERIAL_NUMBER =:SN AND GROUP_NAME='PRINT' ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sSN)
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
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
                    }
                }
            }
            catch
            {
                return "The value field does not exist, please contact Label TE!";
            }

            return Comm.PrintTest1(lb);

        }
    }
}
