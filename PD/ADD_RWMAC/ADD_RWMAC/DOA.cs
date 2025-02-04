using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using SFC_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADD_RWMAC
{
    public partial class DOA_Form : Form
    {
        public string sql, empNo, EMP_NAME, SHIPP_SN, SHIPP_MAC, TMP_SN, TMP_MAC, TMP_MO, SHIP_MO, SHIPP_MODEL, FINISH_DATE_IN, loginDB;
        public string TMP_SSN = "", TMP_SHIP_DATE = "", CK_DB;
        public SfcHttpClient _sfcHttpClient;
        public int Fail_Count, Success_Count;
        MessageError FrmMessage = new MessageError();
        public DOA_Form(SfcHttpClient sfcHttpClient)
        {
            InitializeComponent();
            _sfcHttpClient = sfcHttpClient;
        }

        private void ReportError(string Message)
        {
            StackFrame CallStack = new StackFrame(1, true);
            MessageBox.Show("Error: " + Message + ",\n\rFile: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async Task Get_EMP_NAME()
        {
            sql = " SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO = '" + empNo + "' ";
            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            Lab_Name.Text = result_1.Data["emp_name"].ToString();
        }

        private void showMessage(string MessageEnglish, string MessageVietNam, bool CustomFlag)
        {
            MessageError frmMessage = new MessageError();
            frmMessage._sfcHttpClient = _sfcHttpClient;
            if (CustomFlag)
            {
                frmMessage.MessageEnglish = MessageEnglish;
                frmMessage.MessageVietNam = MessageVietNam;
                frmMessage.CustomFlag = CustomFlag;
            }
            else
            {
                frmMessage.errorcode = MessageEnglish;
                frmMessage.CustomFlag = CustomFlag;
            }
            frmMessage.ShowDialog();
        }

        private void Button_Exit_Click(object sender, EventArgs e)
        {
            DialogResult result_9 = MessageBox.Show("Are you sure you want to close ?", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result_9 == DialogResult.OK)
            {
                this.Close();
            }
        }

        private async void DOA_Form_Shown(object sender, EventArgs e)
        {

            if (CK_DB == "ROKU")  //ROKU
            {
                Menu_SN.Enabled = true;
            }

            Lab_Emp.Text = empNo;
            await Get_EMP_NAME();

            Edit_SSN.Text = "";
            Edit_MAC.Text = "";

            Menu_Mac_ID.Checked = false;
            Menu_SSN.Checked = false;
     
            Edit_Mo_DOA.Focus();  
            Lab_Message.Text = "Please Input MO";
            Lab_TB.Text = "Please Input MO";

            if (Menu_SN.Checked == true)
            {
                Edit_MAC.Focus();
                Lab_Message.Text = "Please Input MAC";
                Lab_TB.Text = "Please Input DOA Product MAC";
            }


        }

        private async Task<Boolean> Check_History()   //ROKU
        {
            try
            {
                sql = " SELECT SYSDATE FROM DUAL@SFCODBH ";
                var result_2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_2.Data != null)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        private async void Butt_Clear_Click(object sender, EventArgs e)
        {
            List_Log.Items.Clear();
            Lab_Success.Text = "0";
            Lab_Fail.Text = "0";
            Fail_Count = 0;
            List_File.Items.Clear();
        }

        private void Menu_Mac_ID_Click(object sender, EventArgs e)
        {
            if (Menu_Mac_ID.Checked == false)
            {
                Edit_MAC.Enabled = true;
                Edit_MAC.Clear();

                if (Menu_SN.Checked == false)
                {
                    Edit_Serial.Focus();
                    Lab_Message.Text = "Please Input SN";
                    Lab_TB.Text = "Please Input DOA Product SN";
                }
                else if (Menu_Mac_ID.Checked == false)
                {
                    Edit_MAC.Focus();
                    Lab_Message.Text = "Please Input MAC";
                    Lab_TB.Text = "Please Input DOA Product MAC";
                }
                else
                {
                    Edit_SSN.Focus();
                    Lab_Message.Text = "Please Input SSN";
                    Lab_TB.Text = "Please Input DOA Product SSN";
                }
            }
            else
            {
                if (Menu_SN.Checked == false)
                {
                    Edit_Serial.Focus();
                    Lab_Message.Text = "Please Input SN";
                    Lab_TB.Text = "Please Input DOA Product SN";
                }
                else if (Menu_SSN.Checked == false)
                {
                    Edit_SSN.Focus();
                    Lab_Message.Text = "Please Input SSN";
                    Lab_TB.Text = "Please Input DOA Product SSN";
                }
                Edit_MAC.Enabled = false;
                Edit_MAC.Clear();
                Edit_MAC.Text = "N/A";
            }
        }

        private void Menu_SSN_Click(object sender, EventArgs e)
        {
            if (Menu_SSN.Checked == false)
            {
                Edit_SSN.Enabled = true;
                Edit_SSN.Clear();

                if (Menu_SN.Checked == false)
                {
                    Edit_Serial.Focus();
                    Lab_Message.Text = "Please Input SN";
                    Lab_TB.Text = "Please Input DOA Product SN";
                }
                else if (Menu_Mac_ID.Checked == false)
                {
                    Edit_MAC.Focus();
                    Lab_Message.Text = "Please Input MAC";
                    Lab_TB.Text = "Please Input DOA Product MAC";
                }
                else
                {
                    Edit_SSN.Focus();
                    Lab_Message.Text = "Please Input SSN";
                    Lab_TB.Text = "Please Input DOA Product SSN";
                }
            }
            else
            {
                Edit_SSN.Enabled = false;
                Edit_SSN.Clear();
                Edit_SSN.Text = "N/A";
            }
        }

        private void Menu_SN_Click(object sender, EventArgs e)
        {
            if (Menu_SN.Checked == false)
            {
                if (CK_DB == "ROKU")//ROKU
                {
                    Edit_Serial.Visible = true;
                    Edit_Serial.Enabled = true;
                    label2.Visible = true;
                }

                Edit_Serial.Enabled = true;
                Edit_Serial.Clear();
                Edit_Serial.Focus();
                Lab_Message.Text = "Please Input SN";
                Lab_TB.Text = "Please Input DOA Product SN";
            }
            else
            {
                if (CK_DB == "ROKU")//ROKU
                {
                    Edit_Serial.Enabled = false;
                    label2.Visible = true;

                }
                else if (Menu_Mac_ID.Checked == false)
                {
                    Edit_MAC.Focus();
                    Lab_Message.Text = "Please Input MAC";
                    Lab_TB.Text = "Please Input DOA Product MAC";
                }
                else
                {
                    Edit_SSN.Focus();
                    Lab_Message.Text = "Please Input SSN";
                    Lab_TB.Text = "Please Input DOA Product SSN";
                }

                Edit_Serial.Enabled = false;
                Edit_Serial.Clear();
                Edit_Serial.Text = "N/A";
            }

        }

        private void Menu_Exit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to close the program ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private async void Edit_Serial_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                if (Edit_Serial.Text.Length != 0)
                {
                    if(CK_DB == "ROKU")
                    {
                        if (await CHECKRMASN(Edit_Serial.Text))
                        {
                            if (Menu_Mac_ID.Checked == false)
                            {
                                Edit_MAC.Focus();
                                Lab_Message.Text = "Please Input MACID";
                                Lab_TB.Text = "Please Input DOA Product MACID";
                            }
                            else if (Menu_SSN.Checked == false)
                            {
                                Edit_SSN.Focus();
                                Lab_Message.Text = "Please Input SSN";
                                Lab_TB.Text = "Please Input DOA Product SHIPPING_SN";
                            }
                        }

                        UPDATE_MODEL_DATE(Edit_Mo_DOA.Text, Edit_SSN.Text);
                    }
                    else
                    {
                        if (await CHECK_SN(Edit_Serial.Text))
                        {
                            if (Menu_Mac_ID.Checked == false)
                            {
                                Edit_MAC.Focus();
                                Lab_Message.Text = "Please Input MACID";
                                Lab_TB.Text = "Please Input DOA Product MACID";
                            }
                            else if (Menu_SSN.Checked == false)
                            {
                                Edit_SSN.Focus();
                                Lab_Message.Text = "Please Input SSN";
                                Lab_TB.Text = "Please Input DOA Product SHIPPING_SN";
                            }
                        }
                    }
                }
                else
                {
                    Lab_Message.Text = "Please Input SN";
                    Lab_TB.Text = "Please Input DOA Product SN";
                }

            }
        }
        private async Task<bool> CHECKRMASN(string Sn_Check)
        {
            sql = " SELECT * FROM SFISM4.R_RMA_LINK_T WHERE SERIAL_NUMBER =  '" + Sn_Check + "' ";
            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_1.Data != null)
            {
                Lab_Message.Text = "Please Input SN";
                List_Log.Items.Add("SN: " + Sn_Check + " Has Exists");
                Fail_Count = Fail_Count + 1;
                Lab_Fail.Text = Fail_Count.ToString();

                Edit_Serial.SelectAll();
                Edit_Serial.Focus();

                return false;

            }
            else
            {
                sql = " SELECT * FROM SFISM4.R_RMA_SN_T WHERE  OUTPUT_DATE IS NULL AND SERIAL_NUMBER = '" + Sn_Check + "' ";
                var result_2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_2.Data != null)
                {
                    TMP_SN = result_2.Data["serial_number"].ToString();
                    TMP_SSN = result_2.Data["shipping_sn"].ToString();
                    TMP_MO = result_2.Data["mo_number"].ToString();
                    SHIPP_MODEL = result_2.Data["model_name"].ToString();

                    Lab_Mo.Visible = true;
                    Lab_Model.Visible = true;
                    Lab_Mo.Text = TMP_MO;
                    Lab_Model.Text = SHIPP_MODEL;

                    if (TMP_MO != Edit_Mo_DOA.Text)
                    {
                        showMessage("MO of SN not match with MO in R_RMA_SN_T\n" + TMP_MO + " <> " + Edit_Mo_DOA.Text + "!", "MO của SN không khớp với MO trong R_RMA_SN_T", true);
                        return false;
                    }

                    if (TMP_SHIP_DATE == "0")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Edit_Serial.SelectAll();
                    Lab_Message.Text = "Not found SN (R_RMA_SN_T)!";
                    Lab_TB.Text = "Please Input DOA Product SN";
                    return false;
                }
            }
        }

        private async Task<bool> UPDATE_MODEL_DATE(string C_Mo, string Sn_Check)  
        {
            string C_Model;

            sql = " SELECT * FROM SFISM4.R105 WHERE MO_NUMBER =  '" + C_Mo + "' ";
            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_1.Data != null)
            {
                C_Model = result_1.Data["model_name"].ToString();
            }
            else
            {
                showMessage("" + C_Mo + " not found in R105 !", "" + C_Mo + " không tìm thấy trong R105 !", true);
                return false;
            }

            sql = " SELECT * FROM SFIS1.C_MODEL_SITE_T WHERE SITE = 'ION' AND MODEL_NAME = '" + C_Model + "' ";
            result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_1.Data == null)
            {
                return false;
            }

            sql = " SELECT * FROM SFISM4.R_PRINT_INPUT_T  WHERE SSN1 =  '" + Sn_Check + "'  AND SSN1 IS NOT NULL";
            result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_1.Data == null)
            {
                sql = "SELECT * FROM SFISM4.R_PRINT_INPUT_T@SFCODBH WHERE SSN1 =  '" + Sn_Check + "' AND SSN1 IS NOT NULL ";
                result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_1.Data != null)
                {
                    sql = " INSERT INTO SFISM4.R_PRINT_INPUT_T (SELECT * FROM SFISM4.R_PRINT_INPUT_T@SFCODBH WHERE SSN1 = '" + Sn_Check + "')  AND SSN1 IS NOT NULL ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return false;
                    }
                }
            }

            sql = " UPDATE SFISM4.R_PRINT_INPUT_T SET MO_NUMBER = '" + C_Mo + "', MODEL_NAME ='" + C_Model + "' WHERE SSN1 = '" + Sn_Check + "' ";
            var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (b.Result != "OK")
            {
                ReportError(b.Message.ToString());
                return false;
            }

            sql = " SELECT * FROM SFISM4.R_RW_MAC_SSN_T WHERE SN =  '" + Sn_Check + "' ";
            result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_1.Data != null)
            {
                showMessage("" + Sn_Check + " exist in R_RW_MAC_SSN_T !", "" + Sn_Check + " đã tồn tại trong R_RW_MAC_SSN_T !", true);
                return false;
            }
            else
            {
                sql = " INSERT INTO  SFISM4.R_RW_MAC_SSN_T (MO_NUMBER,SN,TYPE ,EMP_NO) VALUES ('" + C_Mo + "','" + Sn_Check + "' ,'" + C_Model + "' ,'" + empNo + "')  ";
                b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (b.Result != "OK")
                {
                    ReportError(b.Message.ToString());
                    return false;
                }
            }

            sql = " SELECT * FROM SFISM4.R108 WHERE SERIAL_NUMBER = '" + Sn_Check + "' ";
            result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_1.Data == null)
            {
                sql = " INSERT INTO SFISM4.R108( SELECT * FROM SFISM4.R108@SFCODBH WHERE SERIAL_NUMBER = '" + Sn_Check + "' ) ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return false;
                }
            }

            List_Log.Items.Add("SN: " + Sn_Check + " Success");
            Success_Count = Success_Count + 1;
            Lab_Success.Text = Success_Count.ToString();
            return true;
        }

        private async Task<bool> CHECK_SN(string Sn_Check)
        {
            sql = " SELECT * FROM SFISM4.R_RMA_LINK_T WHERE SERIAL_NUMBER =  '" + Sn_Check + "' ";
            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_1.Data != null)
            {
                Lab_Message.Text = "Please Input SN";
                List_Log.Items.Add("SN: " + Sn_Check + " Has Exists");
                Fail_Count = Fail_Count + 1;
                Lab_Fail.Text = Fail_Count.ToString();

                Edit_Serial.SelectAll();
                Edit_Serial.Focus();

                return false;

            }
            else
            {
                sql = "SELECT * FROM SFISM4.H_TMP_CUSTOMER_T WHERE SERIAL_NUMBER = '" + Sn_Check + "' ";
                var result_2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_2.Data != null)
                {
                    SHIPP_SN = result_2.Data["shipping_sn"].ToString();
                    SHIPP_MAC = result_2.Data["macid"].ToString();
                    TMP_SN = result_2.Data["serial_number"].ToString();
                    TMP_MAC = result_2.Data["macid"].ToString();
                    TMP_SSN = result_2.Data["shipping_sn"].ToString();
                    TMP_MO = result_2.Data["mo_number"].ToString();
                    TMP_SHIP_DATE = await GET_SHIP_DATE_INVOICE(result_2.Data["invoice"].ToString());
                    SHIP_MO = result_2.Data["mo_number"].ToString();
                    SHIPP_MODEL = result_2.Data["model_name"].ToString();

                    Lab_Mo.Visible = true;
                    Lab_Model.Visible = true;
                    Lab_Mo.Text = TMP_MO;
                    Lab_Model.Text = SHIPP_MODEL;

                    if (TMP_SHIP_DATE == "0")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Edit_Serial.SelectAll();
                    Lab_Message.Text = "Not found SN (H_TMP_CUSTOMER_T)!";
                    Lab_TB.Text = "Please Input DOA Product SN";
                    return false;
                }
            }
        }

        private async Task<bool> CHECK_MAC(String Mac_Check)
        {
            sql = " SELECT * FROM SFISM4.R_RMA_LINK_T WHERE MAC1 =  '" + Mac_Check + "' ";
            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_1.Data != null)
            {
                Lab_Message.Text = "MACID Has Exists";
                Lab_TB.Text = "Please Input DOA Product MACID";
                List_Log.Items.Add("MAC: " + Mac_Check + " Has Exists");

                Edit_MAC.SelectAll();
                Edit_MAC.Focus();

                return false;
            }
            else
            {
                if (CK_DB == "ROKU")
                {
                    sql = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE MACID ='" + Mac_Check + "' ";
                    var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result.Data == null)
                    {
                        sql = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T WHERE MACID ='" + Mac_Check + "' ";
                    }
                }
                else
                {
                    sql = "SELECT * FROM SFISM4.H_TMP_CUSTOMER_T WHERE MACID ='" + Mac_Check + "' ";
                }
                var result_2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_2.Data != null)
                {
                    SHIPP_SN = result_2.Data["shipping_sn"].ToString();
                    SHIPP_MAC = result_2.Data["macid"].ToString();

                    if (Menu_SN.Checked == true)
                    {
                        TMP_SN = result_2.Data["serial_number"].ToString();
                        TMP_MAC = result_2.Data["macid"].ToString();
                        TMP_SSN = result_2.Data["shipping_sn"].ToString();
                    }
                    else
                    {
                        if (TMP_MAC != result_2.Data["macid"].ToString())
                        {
                            Lab_Message.Text = "ERROR, MACID NOT MATCH !";
                            Lab_TB.Text = "Please Input DOA Product MACID";
                        }

                    }

                    TMP_SHIP_DATE = await GET_SHIP_DATE_INVOICE(result_2.Data["invoice"].ToString());
                    SHIP_MO = result_2.Data["mo_number"].ToString();
                    TMP_MO = result_2.Data["mo_number"].ToString();
                    SHIPP_MODEL = result_2.Data["model_name"].ToString();

                    Lab_Mo.Visible = true;
                    Lab_Model.Visible = true;
                    Lab_Mo.Text = TMP_MO;
                    Lab_Model.Text = SHIPP_MODEL;

                    if (TMP_SHIP_DATE == "0")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Edit_MAC.SelectAll();
                    Lab_Message.Text = "Not found MACID (H_TMP_CUSTOMER_T)!";
                    Lab_TB.Text = "Please Input DOA Product MACID";
                    return false;
                }
            }
        }
        private async Task<bool> CHECK_SSN(String Snn_Check)
        {
            sql = " SELECT * FROM SFISM4.R_RMA_LINK_T WHERE SSN1 =  '" + Snn_Check + "' ";
            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_1.Data != null)
            {
                Lab_Message.Text = "SSN Has Exists";
                Lab_TB.Text = "Please Input DOA Product SHIPPING_SN";
                List_Log.Items.Add("SN: " + Snn_Check + " Has Exists");

                Edit_SSN.SelectAll();
                Edit_SSN.Focus();

                return false;
            }
            else
            {
                if (CK_DB == "ROKU")
                {
                    sql = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE  SHIPPING_SN =  '" + Snn_Check + "' ";
                    var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result.Data == null)
                    {
                        sql = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T WHERE  SHIPPING_SN =  '" + Snn_Check + "' ";
                    }
                }
                else
                {
                    sql = "SELECT * FROM SFISM4.H_TMP_CUSTOMER_T WHERE SHIPPING_SN ='" + Snn_Check + "' ";
                }

                var result_2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_2.Data != null)
                {
                    SHIPP_SN = result_2.Data["shipping_sn"].ToString();
                    SHIPP_MAC = result_2.Data["macid"].ToString();

                    if ((Menu_SN.Checked == true) && (Menu_Mac_ID.Checked == true))
                    {
                        TMP_SN = result_2.Data["serial_number"].ToString();
                        TMP_MAC = result_2.Data["macid"].ToString();
                        TMP_SSN = result_2.Data["shipping_sn"].ToString();
                    }
                    else if ((Menu_SN.Checked == false) && (Menu_Mac_ID.Checked == false))
                    {
                        if (TMP_SSN != result_2.Data["shipping_sn"].ToString())
                        {
                            Lab_Message.Text = "ERROR, SSN NOT MATCH !";
                            Lab_TB.Text = "Please Input DOA Product SHIPPING_SN";
                        }

                    }

                    TMP_SHIP_DATE = await GET_SHIP_DATE_INVOICE(result_2.Data["invoice"].ToString());
                    SHIP_MO = result_2.Data["mo_number"]?.ToString()?? "";
                    TMP_MO = result_2.Data["mo_number"]?.ToString()?? "";

                    if (TMP_SHIP_DATE == "0")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Edit_MAC.SelectAll();
                    Lab_Message.Text = "Not found SHIPPING_SN (H_TMP_CUSTOMER_T)!";
                    Lab_TB.Text = "Please Input DOA Product SHIPPING_SN";
                    return false;
                }
            }

        }

        private async Task<bool> MARK_HISTORY_DATA(string SN, string MAC, string SSN)
        {
            string Query = "", C_WHERE = "", Q_R107 = "", Update_R107 = "", Q_Z107 = "";
            string Q_CUST = "", Update_CUST = "", Q_R108 = "", Update_R108 = "";

            sql = " SELECT TO_CHAR(SYSDATE,'YYDDMM') TIME_T FROM DUAL ";
            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            String Doa_Time = "DOA" + result_1.Data["time_t"].ToString();

            if (Menu_SN.Checked == false)
            {
                if (CK_DB == "ROKU")
                {
                    if (await Check_History())
                    {
                        Query = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE  SERIAL_NUMBER = '" + SN + "'";
                    }
                    else
                    {
                        Query = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T WHERE  SERIAL_NUMBER = '" + SN + "'";
                    }
                }
                else
                {
                    Query = " SELECT * FROM SFISM4.H_TMP_CUSTOMER_T where  SERIAL_NUMBER = '" + SN + "' ";
                }
                
                C_WHERE = " SERIAL_NUMBER = '" + SN + "' ";
                Q_R107 = " SELECT * FROM SFISM4.R107 WHERE SERIAL_NUMBER = '" + SN + "' ";
                Update_R107 = " SERIAL_NUMBER = '" + SN + "' ";
                Q_Z107 = " SELECT * FROM SFISM4.Z107 WHERE SERIAL_NUMBER = '" + SN + "' ";
                Q_CUST = " SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER = '" + SN + "' ";
                Update_CUST = " SERIAL_NUMBER = '" + SN + "' ";
                Q_R108 = " SELECT * FROM SFISM4.R108 WHERE SERIAL_NUMBER = '" + SN + "' ";
                Update_R108 = " SERIAL_NUMBER = '" + SN + "' ";
            }
            else if (Menu_SSN.Checked == false)
            {
                if (CK_DB == "ROKU")
                {
                    if (await Check_History())
                    {
                        Query = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE MACID = '" + MAC + "' ";
                    }
                    else
                    {
                        Query = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T WHERE MACID = '" + MAC + "' ";
                    }
                }
                else
                {
                    Query = "  SELECT * FROM SFISM4.H_TMP_CUSTOMER_T WHERE  MACID = '" + MAC + "' ";
                }
                
                C_WHERE = " MACID = '" + MAC + "' ";
                Q_R107 = " SELECT * FROM SFISM4.R107 WHERE SHIPPING_SN2 = '" + MAC + "' ";
                Update_R107 = " SHIPPING_SN2 = '" + MAC + "' ";
                Q_Z107 = " SELECT * FROM SFISM4.Z107 WHERE SHIPPING_SN2 = '" + MAC + "' ";
                Q_CUST = " SELECT * FROM SFISM4.R_CUSTSN_T WHERE MAC1 = '" + MAC + "' ";
                Update_CUST = " MAC1 = '" + MAC + "' ";
                Q_R108 = " SELECT * FROM SFISM4.R108 WHERE KEY_PART_SN = '" + MAC + "' ";
                Update_R108 = " KEY_PART_SN = '" + MAC + "' ";

            }
            else if (Menu_Mac_ID.Checked == false)
            {
                if (CK_DB == "ROKU")
                {
                    if (await Check_History())
                    {
                        Query = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE SHIPPING_SN = '" + SSN + "' ";
                    }
                    else
                    {
                        Query = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T WHERE SHIPPING_SN = '" + SSN + "' ";
                    }
                }
                else
                {
                    Query = " SELECT * FROM SFISM4.H_TMP_CUSTOMER_T WHERE  SHIPPING_SN = '" + SSN + "' ";
                }
                
                C_WHERE = " SHIPPING_SN = '" + SSN + "' ";
                Q_R107 = " SELECT * FROM SFISM4.R107 WHERE SHIPPING_SN = '" + SSN + "' ";
                Update_R107 = " SHIPPING_SN = '" + SSN + "' ";
                Q_Z107 = " SELECT * FROM SFISM4.Z107 WHERE SHIPPING_SN = '" + SSN + "' ";
                Q_CUST = " SELECT * FROM SFISM4.R_CUSTSN_T WHERE SSN1 = '" + SSN + "' ";
                Update_CUST = " SSN1 = '" + SSN + "' "; ;
            }

            try
            {
                if (Query != "")
                {
                    var result1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Query, SfcCommandType = SfcCommandType.Text });
                    if (result1.Data.Count() != 0)
                    {
                        if (Menu_Mac_ID.Checked == true)
                        {
                            if (CK_DB == "ROKU")
                            {
                                if (await Check_History())
                                {
                                    sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER = SERIAL_NUMBER||'" + Doa_Time + "', SHIPPING_SN=SHIPPING_SN||'" + Doa_Time + "' WHERE " + C_WHERE + " ";
                                }
                                else
                                {
                                    sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T SET SERIAL_NUMBER=SERIAL_NUMBER|| '" + Doa_Time + "', SHIPPING_SN=SHIPPING_SN|| '" + Doa_Time + "' WHERE " + C_WHERE + " ";
                                }
                            }
                            else
                            {
                                sql = " UPDATE SFISM4.H_TMP_CUSTOMER_T SET SERIAL_NUMBER = SERIAL_NUMBER || '" + Doa_Time + "', SHIPPING_SN = SHIPPING_SN || '" + Doa_Time + "' WHERE " + C_WHERE + " ";
                            }
                            
                            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return false;
                            }
                        }
                        else
                        {
                            if (CK_DB == "ROKU")
                            {
                                if (await Check_History())
                                {
                                    sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER=SERIAL_NUMBER||'" + Doa_Time + "',MACID=MACID||'" + Doa_Time + "',SHIPPING_SN=SHIPPING_SN||'" + Doa_Time + "' WHERE " + C_WHERE + " ";
                                }
                                else
                                {
                                    sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T SET SERIAL_NUMBER=SERIAL_NUMBER||'" + Doa_Time + "',MACID=MACID||'" + Doa_Time + "',SHIPPING_SN=SHIPPING_SN||'" + Doa_Time + "' WHERE " + C_WHERE + "  ";
                                }
                            }
                            else
                            {
                                sql = " UPDATE SFISM4.H_TMP_CUSTOMER_T SET SERIAL_NUMBER = SERIAL_NUMBER||'" + Doa_Time + "', MACID = MACID||'" + Doa_Time + "', SHIPPING_SN = SHIPPING_SN||'" + Doa_Time + "' WHERE " + C_WHERE + " ";
                            }

                            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return false;
                            }
                        }
                    }
                }

                if (Q_R107 != "")
                {
                    var result1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Q_R107, SfcCommandType = SfcCommandType.Text });
                    if (result1.Data.Count() != 0)
                    {
                        if (Menu_Mac_ID.Checked == true)
                        {
                            sql = " UPDATE SFISM4.R107 SET SERIAL_NUMBER=SERIAL_NUMBER||'" + Doa_Time + "', SHIPPING_SN = SHIPPING_SN||'" + Doa_Time + "' WHERE '" + Update_R107 + "' ";
                            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return false;
                            }
                        }
                        else
                        {
                            sql = " UPDATE SFISM4.R107 SET SERIAL_NUMBER=SERIAL_NUMBER||'" + Doa_Time + "', SHIPPING_SN2 = SHIPPING_SN2||'" + Doa_Time + "', SHIPPING_SN = SHIPPING_SN||'" + Doa_Time + "' WHERE " + Update_R107 + " ";
                            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return false;
                            }
                        }
                    }
                }

                if (Q_Z107 != "")
                {
                    var result1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Q_Z107, SfcCommandType = SfcCommandType.Text });
                    if (result1.Data.Count() != 0)
                    {
                        if (Menu_Mac_ID.Checked == true)
                        {
                            sql = " UPDATE SFISM4.Z107 SET SERIAL_NUMBER = SERIAL_NUMBER||'" + Doa_Time + "', SHIPPING_SN = SHIPPING_SN||'" + Doa_Time + "' WHERE " + Update_R107 + " ";
                            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return false;
                            }
                        }
                        else
                        {
                            sql = " UPDATE SFISM4.Z107 SET SERIAL_NUMBER = SERIAL_NUMBER||'" + Doa_Time + "', SHIPPING_SN2 = SHIPPING_SN2||'" + Doa_Time + "', SHIPPING_SN = SHIPPING_SN||'" + Doa_Time + "' WHERE " + Update_R107 + " ";
                            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return false;
                            }
                        }
                    }
                }

                if (Q_CUST != "")
                {
                    var result1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Q_CUST, SfcCommandType = SfcCommandType.Text });
                    if (result1.Data.Count() != 0)
                    {
                        if (Menu_Mac_ID.Checked == true)
                        {
                            sql = " UPDATE SFISM4.R_CUSTSN_T SET SERIAL_NUMBER = SERIAL_NUMBER||'" + Doa_Time + "', SSN1 = SSN1||'" + Doa_Time + "' WHERE " + Update_CUST + " ";
                            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return false;
                            }
                        }
                        else
                        {
                            sql = " UPDATE SFISM4.R_CUSTSN_T SET SERIAL_NUMBER = SERIAL_NUMBER||'" + Doa_Time + "', SSN1 = SSN1||'" + Doa_Time + "', MAC1 = MAC1||'" + Doa_Time + "' WHERE  " + Update_CUST + " ";
                            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return false;
                            }
                        }
                    }
                }

                if (Q_R108 != "")
                {
                    var result1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Q_R108, SfcCommandType = SfcCommandType.Text });
                    if (result1.Data.Count() != 0)
                    {
                        sql = " UPDATE SFISM4.R108 SET SERIAL_NUMBER = SERIAL_NUMBER||'" + Doa_Time + "', KEY_PART_SN = KEY_PART_SN||'" + Doa_Time + "' WHERE " + Update_R108 + " ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return false;
                        }
                    }
                }

                return true;

            }
            catch
            {
                return false;
            }
        }
        private void Menu_Text_Click(object sender, EventArgs e)
        {
            if (Menu_Text.Checked == true)
            {
                Menu_Excel.Checked = false;
                Menu_Text.Checked = true;
            }
            else
            {
                Menu_Excel.Checked = true;
                Menu_Text.Checked = false;
            }
        }

        private void Menu_Excel_Click(object sender, EventArgs e)
        {
            if (Menu_Excel.Checked == true)
            {
                Menu_Excel.Checked = true;
                Menu_Text.Checked = false;
            }
            else
            {
                Menu_Excel.Checked = false;
                Menu_Text.Checked = true;
            }
        }

        private async void Edit_MAC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if(Edit_Mo_DOA.Text.Length != 0)
                {
                    await Load_Mac_key();
                }
                else
                {
                    Lab_Message.Text = "Please Input MO";
                    Lab_TB.Text = "Please Input MO";
                    Edit_Mo_DOA.Focus();
                    Edit_Mo_DOA.SelectAll();
                }
            }
        }

        private async Task Load_Mac_key()
        {
            if (Edit_MAC.Text.Length != 0)
            {
                if (await CHECK_MAC(Edit_MAC.Text))
                {
                    if (Menu_SSN.Checked == false)
                    {
                        Lab_Message.Text = "Please Input SSN";
                        Lab_TB.Text = "Please Input DOA Product SSN";
                        Edit_SSN.Focus();
                        Edit_SSN.SelectAll();
                    }
                    else if ((Menu_SSN.Checked == true) && (Menu_SN.Checked == false))
                    {
                        Lab_Message.Text = "Please Input SN";
                        Lab_TB.Text = "Please Input DOA Product SN";
                    }
                    else if ((Menu_SSN.Checked == true) && (Menu_SN.Checked == true))
                    {
                        if (await INSERT_RMA_LINK())
                        {
                            if (await MARK_HISTORY_DATA(TMP_SN, TMP_MAC, TMP_SSN))
                            {
                                await CHECKRWSSNMAC(TMP_MAC, TMP_SSN);
                                Edit_MAC.Text = "";
                                Edit_MAC.Focus();

                                Lab_Message.Text = "OK";
                                Lab_TB.Text = "Please Input DOA Product MACID";

                                List_Log.Items.Add("MAC: " + TMP_MAC + " Success");
                                Success_Count = Success_Count + 1;
                                Lab_Success.Text = Success_Count.ToString();

                                TMP_SN = "";
                                TMP_MAC = "";
                                TMP_SSN = "";
                            }
                            else
                            {
                                await CHECKRWSSNMAC(TMP_MAC, TMP_SSN);
                                Edit_MAC.Text = "";
                                Edit_MAC.Focus();

                                Lab_Message.Text = "ERROR";
                                Lab_TB.Text = "UPDATE DATABASE FAIL";

                                List_Log.Items.Add("MAC: " + TMP_MAC + " Update Fail");
                                Fail_Count = Fail_Count + 1;
                                Lab_Fail.Text = Fail_Count.ToString();
                            }
                        }
                    }

                }
                else
                {
                    if (Menu_SN.Checked == true)
                    {
                        Fail_Count = Fail_Count + 1;
                        Lab_Fail.Text = Fail_Count.ToString();
                    }
                }
            }
            else
            {
                Lab_Message.Text = "Please Input MAC";
                Lab_TB.Text = "Please Input DOA Product MAC";
            }
        }

        private async Task<string> GET_SHIP_DATE_INVOICE(string invoice)
        {
            sql = " SELECT FINISH_DATE FROM SFISM4.R_BPCS_INVOICE_T  WHERE INVOICE||' '|| INV_NO = '" + invoice + "' AND FINISH_DATE IS NOT NULL ";
            var result_3 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if ((result_3.Data != null))
            {
                FINISH_DATE_IN = result_3.Data["finish_date"].ToString();
                return FINISH_DATE_IN;
            }
            else
            {
                sql = " SELECT FINISH_DATE FROM SFISM4.R_BPCS_INVOICE_T@SFCODBH  WHERE INVOICE||' '|| INV_NO = '" + invoice + "' ";
                var result_4 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_4.Data != null)
                {
                    FINISH_DATE_IN = result_3.Data["finish_date"].ToString();
                    return FINISH_DATE_IN;
                }
                else
                {
                    FINISH_DATE_IN = "0";
                    return FINISH_DATE_IN;
                }
            }
        }

        private  async void Edit_SSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Edit_Mo_DOA.Text.Length != 0)
                {
                    await Load_SSN_Key();
                }
                else
                {
                    Lab_Message.Text = "Please Input MO";
                    Lab_TB.Text = "Please Input MO";
                    Edit_Mo_DOA.Focus();
                    Edit_Mo_DOA.SelectAll();
                }
            }
            else
            {
                Lab_Message.Text = "Please Input SSN";
                Lab_TB.Text = "Please Input DOA Product SSN";
            }
        }

        private async Task Load_SSN_Key()
        {
            if (Edit_SSN.Text.Length != 0)
            {
                if (await CHECK_SSN(Edit_SSN.Text))
                {
                    if (Menu_SN.Checked == false)
                    {
                        Edit_Serial.Focus();

                        if (await INSERT_RMA_LINK())
                        {
                            if (await MARK_HISTORY_DATA(TMP_SN, TMP_MAC, TMP_SSN))
                            {
                                TMP_SN = ""; 
                                TMP_MAC = "";
                                TMP_SSN = "";

                                await CHECKRWSSNMAC(TMP_MAC, TMP_SSN);

                                List_Log.Items.Add("SN: " + Edit_Serial + " Success");
                                Success_Count = Success_Count + 1;
                                Lab_Success.Text = Success_Count.ToString();

                                Edit_SSN.Text = "";
                                Edit_MAC.Text = "";
                                Edit_Serial.Text = "";
                                Edit_Serial.Focus();

                                Lab_Message.Text = "OK";
                                Lab_TB.Text = "Please Input DOA Product SN";
                            }
                            else
                            {
                                await CHECKRWSSNMAC(TMP_MAC, TMP_SSN);

                                List_Log.Items.Add("SN: " + Edit_Serial + " Update Fail");
                                Fail_Count = Fail_Count + 1;
                                Lab_Fail.Text = Fail_Count.ToString();

                                Edit_SSN.Text = "";
                                Edit_MAC.Text = "";
                                Edit_Serial.Text = "";
                                Edit_Serial.Focus();

                                Lab_Message.Text = "ERROR";
                                Lab_TB.Text = "UPDATE DATABASE FAIL";
                            }
                        }
                    }
                    else if (Menu_Mac_ID.Checked == false)
                    {
                        if (await INSERT_RMA_LINK())
                        {
                            if (await MARK_HISTORY_DATA(TMP_SN, TMP_MAC, TMP_SSN))
                            {
                                await CHECKRWSSNMAC(TMP_MAC, TMP_SSN);

                                List_Log.Items.Add("MAC/SSN: " + TMP_MAC + ", " + TMP_SSN + " Success");
                                Success_Count = Success_Count + 1;
                                Lab_Success.Text = Success_Count.ToString();

                                Lab_Message.Text = "OK";
                                Lab_TB.Text = "Please Input DOA Product SN";

                                TMP_SN = "";
                                TMP_MAC = "";
                                TMP_SSN = "";

                                Edit_SSN.Text = "";
                                Edit_MAC.Text = "";
                                Edit_MAC.Focus();
                            }
                            else
                            {
                                await CHECKRWSSNMAC(TMP_MAC, TMP_SSN);
                                Edit_SSN.Text = "";
                                Edit_MAC.Text = "";
                                Edit_MAC.Focus();

                                List_Log.Items.Add("MAC/SSN: " + TMP_MAC + ", " + TMP_SSN + " Update Fail");
                                Fail_Count = Fail_Count + 1;
                                Lab_Fail.Text = Fail_Count.ToString();

                                Lab_Message.Text = "ERROR";
                                Lab_TB.Text = "UPDATE DATABASE FAIL";
                            }
                        }
                    }
                    else
                    {
                        if (await INSERT_RMA_LINK())
                        {
                            if (await MARK_HISTORY_DATA(TMP_SN, TMP_MAC, TMP_SSN))
                            {
                                await CHECKRWSSNMAC(TMP_MAC, TMP_SSN);
                                List_Log.Items.Add("SSN: " + TMP_SSN + " Success");
                                Success_Count = Success_Count + 1;
                                Lab_Success.Text = Success_Count.ToString();

                                Lab_Message.Text = "OK";
                                Lab_TB.Text = "Please Input DOA Product SSN";

                                TMP_SN = "";
                                TMP_MAC = "";
                                TMP_SSN = "";

                                Edit_SSN.Text = "";
                                Edit_SSN.Focus();
                            }
                            else
                            {
                                await CHECKRWSSNMAC(TMP_MAC, TMP_SSN);
                                Edit_SSN.Text = "";
                                Edit_SSN.Focus();

                                List_Log.Items.Add("SSN: " + TMP_SSN + " Update Fail");
                                Fail_Count = Fail_Count + 1;
                                Lab_Fail.Text = Fail_Count.ToString();

                                Lab_Message.Text = "ERROR";
                                Lab_TB.Text = "UPDATE DATABASE FAIL";
                            }
                        }
                    }
                }
                else
                {
                    Fail_Count = Fail_Count + 1;
                    Lab_Fail.Text = Fail_Count.ToString();
                    Edit_SSN.SelectAll();
                }
            }
        }

        private async Task<bool> INSERT_RMA_LINK()
        {
            string Temp_SSN, Temp_MAC;

            if ((TMP_SSN == "") || (TMP_SSN == "N/A"))
            {
                Temp_SSN = TMP_SN;
            }
            else
            {
                Temp_SSN = TMP_SSN;
            }

            if (Menu_Mac_ID.Checked)
            {
                Temp_MAC = TMP_SSN;
            }
            else
            {
                Temp_MAC = TMP_MAC;
            }

            sql = " INSERT INTO SFISM4.R_RMA_LINK_T (MO_NUMBER, SERIAL_NUMBER, MO1, SSN1, MAC1, SHIPPING_DATE1, IN_STATION_TIME1, LINK_EMP , STEP)  ";
            sql = sql + " VALUES ('" + Edit_Mo_DOA.Text + "', '" + TMP_SN + "', '" + TMP_MO + "', '" + Temp_SSN + "','" + Temp_MAC + "', TO_DATE('" + TMP_SHIP_DATE + "', 'YYYY/MM/DD HH12:MI:SS PM'), SYSDATE,'" + empNo + "','1')  ";
            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

            if (a.Result != "OK")
            {
                Lab_Message.Text = "ERROR";
                Lab_TB.Text = "INSERT INTO SFISM4.R_RMA_LINK_T FAIL (ERROR)";
                ReportError(a.Message.ToString());
                return false;
            }
            else
            {
                return true;
            }
        }

        private async void Butt_Import_Click(object sender, EventArgs e)
        {
            string path = "";
            string snCheck, snCheck_2;


            if (Menu_Excel.Checked)
            {
                List<string> listSheet = new List<string>();
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Excel Files (.xls*)|*.xls*|All Files (*.*)|*.*";
                dlg.Multiselect = false;

                DialogResult dlgResult = dlg.ShowDialog();
                if (dlgResult == DialogResult.OK)
                {
                    path = dlg.FileName;
                    if (path.Equals(""))
                    {
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = string.Format("PLEASE CHOSE FILE EXCEL!");
                        FrmMessage.MessageVietNam = string.Format("PLEASE CHOSE FILE EXCEL!");
                        FrmMessage.ShowDialog();
                        return;
                    }

                    if (!File.Exists(path))
                    {
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = string.Format("CAN NOT OPEN FILE!");
                        FrmMessage.MessageVietNam = string.Format("CAN NOT OPEN FILE!");
                        FrmMessage.ShowDialog();
                        return;
                    }

                    String filePath = path;
                    string excelcon;
                    if (filePath.Substring(filePath.LastIndexOf('.')).ToLower() == ".xlsx")
                    {
                        excelcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1'";
                    }
                    else
                    {
                        excelcon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1'";
                    }
                    OleDbConnection conexcel = new OleDbConnection(excelcon);

                    try
                    {
                        conexcel.Open();
                        DataTable dtExcel = conexcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcel.Rows[0]["Table_Name"].ToString();
                        OleDbCommand cmdexcel1 = new OleDbCommand();
                        cmdexcel1.Connection = conexcel;
                        cmdexcel1.CommandText = "select * from[" + sheetName + "]";
                        DataTable dt = new DataTable();
                        OleDbDataAdapter da = new OleDbDataAdapter();
                        da.SelectCommand = cmdexcel1;
                        da.Fill(dt);
                        conexcel.Close();

                        int countRow = dt.Rows.Count;

                        if ( (Menu_SSN.Checked) && (!Menu_Mac_ID.Checked) )
                        {
                            for (int i = 0; i < countRow; i++)
                            {
                                snCheck = dt.Rows[i][0].ToString();

                                if (!string.IsNullOrEmpty(snCheck))
                                {
                                    List_File.Items.Add(snCheck + " line " + i);

                                    if (snCheck.Length != 12)
                                    {
                                        List_Log.Items.Add("MAC: " + snCheck + " Length Error");
                                        Fail_Count = Fail_Count + 1;
                                        Lab_Fail.Text = Fail_Count.ToString();
                                    }
                                    else
                                    {
                                        if (snCheck.IndexOf(",") != -1)
                                        {
                                            List_Log.Items.Add("MAC: " + snCheck + " Format Error");
                                            Fail_Count = Fail_Count + 1;
                                            Lab_Fail.Text = Fail_Count.ToString();
                                            Edit_SSN.SelectAll();
                                        }
                                        else
                                        {
                                            Edit_MAC.Text = snCheck;
                                            await Load_Mac_key();
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else if ( (!Menu_SSN.Checked) && (Menu_Mac_ID.Checked) )
                        {
                            for (int i = 0; i < countRow; i++)
                            {
                                snCheck = dt.Rows[i][0].ToString();

                                if (!string.IsNullOrEmpty(snCheck))
                                {
                                    if (snCheck.IndexOf(",") != -1)
                                    {
                                        List_Log.Items.Add("SSN: " + snCheck + " Format Error");
                                        Fail_Count = Fail_Count + 1;
                                        Lab_Fail.Text = Fail_Count.ToString();
                                        Edit_SSN.SelectAll();
                                    }
                                    else
                                    {
                                        Edit_SSN.Text = snCheck;
                                        Load_SSN_Key();
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else if((!Menu_SSN.Checked) && (!Menu_Mac_ID.Checked))
                        {
                            for (int i = 0; i < countRow; i++)
                            {
                                snCheck = dt.Rows[i][0].ToString();
                                snCheck_2 = dt.Rows[i][1].ToString();
                                if ( (!string.IsNullOrEmpty(snCheck)) && (!string.IsNullOrEmpty(snCheck_2)) )
                                {
                                    Edit_MAC.Text = snCheck;
                                    await Load_Mac_key();

                                    Edit_SSN.Text = snCheck_2;
                                    await Load_SSN_Key();
                                } 
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Text Files (.txt*)|*.txt*|All Files (*.*)|*.*";
                openFileDialog.Multiselect = false;

                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                DialogResult dlgResult = openFileDialog.ShowDialog();
                if (dlgResult == DialogResult.OK)
                {

                        foreach (string filename in openFileDialog.FileNames)
                            Path.GetFileName(openFileDialog.FileNames.ToString());
                        string[] dataSN = File.ReadAllLines(openFileDialog.FileName.ToString());
                        if (dataSN != null)
                        {
                            foreach (string sn in dataSN)
                            {
                                List_File.Items.Add(sn);
                            }
                        }
                }

                try
                {
                    int Count_Row = List_File.Items.Count;

                    if ((Menu_SSN.Checked) && (!Menu_Mac_ID.Checked))
                    {
                        for (int i = 0; i < Count_Row; i++)
                        {
                            snCheck = List_File.Items[i].ToString();

                            if (!string.IsNullOrEmpty(snCheck))
                            {
                                if (snCheck.Length != 12)
                                {
                                    List_Log.Items.Add("MAC: " + snCheck + " Length Error");
                                    Fail_Count = Fail_Count + 1;
                                    Lab_Fail.Text = Fail_Count.ToString();
                                }
                                else
                                {
                                    if (snCheck.IndexOf(",") != -1)
                                    {
                                        List_Log.Items.Add("MAC: " + snCheck + " Format Error");
                                        Fail_Count = Fail_Count + 1;
                                        Lab_Fail.Text = Fail_Count.ToString();
                                        Edit_SSN.SelectAll();
                                    }
                                    else
                                    {
                                        Edit_MAC.Text = snCheck;
                                        await Load_Mac_key();
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                    else if ((!Menu_SSN.Checked) && (Menu_Mac_ID.Checked))
                    {
                        for (int i = 0; i < Count_Row; i++)
                        {
                            snCheck = List_File.Items[i].ToString();

                            if (!string.IsNullOrEmpty(snCheck))
                            {
                                if (snCheck.IndexOf(",") != -1)
                                {
                                    List_Log.Items.Add("SSN: " + snCheck + " Format Error");
                                    Fail_Count = Fail_Count + 1;
                                    Lab_Fail.Text = Fail_Count.ToString();
                                    Edit_SSN.SelectAll();
                                }
                                else
                                {
                                    Edit_SSN.Text = snCheck;
                                    Load_SSN_Key();
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else if ((!Menu_SSN.Checked) && (!Menu_Mac_ID.Checked))
                    {
                        for (int i = 0; i < Count_Row; i++)
                        {
                            snCheck = List_File.Items[i].ToString();

                            if (!string.IsNullOrEmpty(snCheck))
                            {
                                if (snCheck.IndexOf(",") != -1)
                                {
                                    Edit_MAC.Text = snCheck.Substring(0, snCheck.IndexOf(","));
                                    await Load_Mac_key();
                                    Edit_SSN.Text = snCheck.Substring(snCheck.IndexOf(",") + 1, snCheck.Length - snCheck.IndexOf(",") -1 );
                                    await Load_SSN_Key();
                                }
                                else
                                {
                                    Edit_SSN.Text = "";
                                    Edit_MAC.SelectAll();
                                    List_Log.Items.Add("MAC/SSN: " + snCheck + " Format Error");
                                    Fail_Count = Fail_Count + 1;
                                    Lab_Fail.Text = Fail_Count.ToString();
                                    return;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Environment.Exit(0);
                }
            }
        }


        private async Task CHECKRWSSNMAC(String MAC, string SSN)
        {
            string foundmac = "";

            if ((Menu_Mac_ID.Checked == false) && (Menu_SSN.Checked == true))
            {
                sql = "SELECT * FROM  SFISM4.R_RW_MAC_SSN_T WHERE MAC = '" + MAC + "' ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    if (result.Data["mo_number"].ToString() != Edit_Mo_DOA.Text)
                    {
                        DialogResult result_1 = MessageBox.Show(" ERROR ! MAC R_RW_MAC_SSN_T ALREADY IN ADD MO_NUMBER: " + result.Data["mo_number"].ToString() + ", WHETHER NEED CHANGE NONCE REWORK MO ? ", "QUESTION", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (result_1 == DialogResult.OK)
                        {
                            foundmac = "Y";
                        }
                    }
                    else
                    {
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = string.Format("ERROR! MAC R_RW_MAC_SSN_T ALREADY IN ADD !");
                        FrmMessage.MessageVietNam = string.Format("Lỗi! MAC đã tồn tại trong R_RW_MAC_SSN_T !");
                        FrmMessage.ShowDialog();
                        Edit_MAC.SelectAll();
                        return;
                    }
                }

                if (foundmac == "Y")
                {
                    sql = " INSERT INTO SFISM4.R_RW_DETAIL_T ";
                    sql = sql + " SELECT * FROM  SFISM4.R_RW_MAC_SSN_T WHERE MAC = '" + MAC + "' ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }
                    else
                    {
                        sql = " UPDATE  SFISM4.R_RW_MAC_SSN_T SET  MO_NUMBER = '" + Edit_Mo_DOA.Text + "', ";
                        sql = sql + " EMP_NO = '"+empNo+"', IN_STATION_TIME = SYSDATE  WHERE MAC = '"+MAC+"' ";
                        var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        if (b.Result != "OK")
                        {
                            ReportError(b.Message.ToString());
                            return;
                        }
                    }
                }
                else
                {
                    sql = " INSERT INTO  SFISM4.R_RW_MAC_SSN_T ( MO_NUMBER, MAC,TYPE , EMP_NO ) ";
                    sql = sql + " VALUES ('" + Edit_Mo_DOA.Text + "', '" + MAC + "', '" + Lab_Model.Text + "' , '" + empNo + "' ) ";
                    var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                    if (b.Result != "OK")
                    {
                        ReportError(b.Message.ToString());
                        return;
                    }
                }
            }
            else if ((Menu_Mac_ID.Checked == false) && (Menu_SSN.Checked == false))
            {
                sql = "SELECT * FROM  SFISM4.R_RW_MAC_SSN_T WHERE MAC = '" + MAC + "'  OR SSN = '" + SSN + "'  ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    if (result.Data["mo_number"].ToString() != Edit_Mo_DOA.Text)
                    {
                        DialogResult result_1 = MessageBox.Show(" ERROR ! MAC R_RW_MAC_SSN_T ALREADY IN ADD MO_NUMBER: " + result.Data["mo_number"].ToString() + ", WHETHER NEED CHANGE NONCE REWORK MO ? ", "QUESTION", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (result_1 == DialogResult.OK)
                        {
                            foundmac = "Y";
                        }
                    }
                    else
                    {
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = string.Format("ERROR! MAC OR SSN R_RW_MAC_SSN_T ALREADY IN ADD !");
                        FrmMessage.MessageVietNam = string.Format("Lỗi! MAC OR SSN đã tồn tại trong R_RW_MAC_SSN_T !");
                        FrmMessage.ShowDialog();
                        Edit_MAC.SelectAll();
                        return;
                    }
                }

                if (foundmac == "Y")
                {
                    sql = " INSERT INTO SFISM4.R_RW_DETAIL_T ";
                    sql = sql + " SELECT * FROM  SFISM4.R_RW_MAC_SSN_T WHERE MAC = '" + MAC + "'  OR SSN = '" + SSN + "' ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }
                    else
                    {
                        sql = " UPDATE  SFISM4.R_RW_MAC_SSN_T SET  MO_NUMBER = '" + Edit_Mo_DOA.Text + "', ";
                        sql = sql + " EMP_NO = '"+empNo+"', IN_STATION_TIME = SYSDATE  WHERE MAC = '"+MAC+"'  OR SSN = '" + SSN + "' ";
                        var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        if (b.Result != "OK")
                        {
                            ReportError(b.Message.ToString());
                            return;
                        }
                    }
                }
                else
                {
                    sql = " INSERT INTO  SFISM4.R_RW_MAC_SSN_T ( MO_NUMBER, MAC, SSN, TYPE , EMP_NO ) ";
                    sql = sql + " VALUES ('" + Edit_Mo_DOA.Text + "', '" + MAC + "', '" + SSN + "', '" + Lab_Model.Text + "', '" + empNo + "' ) ";
                    var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                    if (b.Result != "OK")
                    {
                        ReportError(b.Message.ToString());
                        return;
                    }
                }
            }
            else if ((Menu_Mac_ID.Checked == true) && (Menu_SSN.Checked == false))
            {
                sql = "SELECT * FROM  SFISM4.R_RW_MAC_SSN_T WHERE SSN = '" + SSN + "' ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    if (result.Data["mo_number"].ToString() != Edit_Mo_DOA.Text)
                    {
                        DialogResult result_1 = MessageBox.Show(" ERROR ! SSN R_RW_MAC_SSN_T ALREADY IN ADD MO_NUMBER: " + result.Data["mo_number"].ToString() + ", WHETHER NEED CHANGE NONCE REWORK MO ? ", "QUESTION", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (result_1 == DialogResult.OK)
                        {
                            foundmac = "Y";
                        }
                    }
                    else
                    {
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = string.Format("ERROR! SSN R_RW_MAC_SSN_T ALREADY IN ADD !");
                        FrmMessage.MessageVietNam = string.Format("Lỗi! SSN đã tồn tại trong R_RW_MAC_SSN_T !");
                        FrmMessage.ShowDialog();
                        Edit_SSN.SelectAll();
                        return;
                    }
                }

                if (foundmac == "Y")
                {
                    sql = " INSERT INTO SFISM4.R_RW_DETAIL_T ";
                    sql = sql + " SELECT * FROM  SFISM4.R_RW_MAC_SSN_T WHERE SSN = '" + SSN + "' ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }

                    sql = " UPDATE  SFISM4.R_RW_MAC_SSN_T SET  MO_NUMBER = '" + Edit_Mo_DOA.Text + "', ";
                    sql = sql + " EMP_NO = '" + empNo + "', IN_STATION_TIME = SYSDATE  WHERE MAC = '" + SSN + "' ";
                    var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                    if (b.Result != "OK")
                    {
                        ReportError(b.Message.ToString());
                        return;
                    }

                }
                else
                {
                    sql = " INSERT INTO  SFISM4.R_RW_MAC_SSN_T ( MO_NUMBER, SSN, TYPE , EMP_NO ) ";
                    sql = sql + " VALUES ('" + Edit_Mo_DOA.Text + "', '" + SSN + "', '" + Lab_Model.Text + "' , '" + empNo + "' ) ";
                    var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                    if (b.Result != "OK")
                    {
                        ReportError(b.Message.ToString());
                        return;
                    }
                }
            }
        }

        private async void Edit_Mo_DOA_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sql = "SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER =  '" + Edit_Mo_DOA.Text + "' ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data == null)
                {
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("" + Edit_Mo_DOA.Text + " Not dowload SAP !");
                    FrmMessage.MessageVietNam = string.Format("" + Edit_Mo_DOA.Text + " Chưa được Dowload từ SAP !");
                    FrmMessage.ShowDialog();
                    return;
                }
                else
                {
                    if(Edit_Serial.Visible)
                    {
                        Edit_Serial.Focus();
                        Edit_Serial.SelectAll();
                    }
                    else if (Edit_MAC.Enabled)
                    {
                        Edit_MAC.Focus();
                        Edit_MAC.SelectAll();
                    } 
                    else
                    {
                        Edit_SSN.Focus();
                        Edit_SSN.SelectAll();
                    }

                }
            }
        }
    }
}
