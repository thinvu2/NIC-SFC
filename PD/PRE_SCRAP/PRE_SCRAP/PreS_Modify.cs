using Newtonsoft.Json;
using Oracle;
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
using PRE_SCRAP.Resources;
using System.Threading;

namespace PRE_SCRAP
{
    public partial class PreS_Modify : Form
    {
        public DB _oracle;
        public string inputLogin, checkSum, baseUrl, loginDB, EmpNo, empPass, error_code, sql;
        DataTable dt;
        public SfcHttpClient _sfcHttpClient = DBAPI._sfcHttpClient;
        Form opener;
        MessageError FrmMessage = new MessageError();

        public string Line, Section, Group, Station, WipGroup, C_MoNumber, Temp_Mo;

        public string VC_SCRAP_KIND, VC_MO_NUMBER, VC_SCRAP_KIND_REAL, C_Skind, CK_DB;
        public int VC_SCRAP_QTY, Success_Count = 0, Fail_Count = 0, C_Count = 0;
        public PreS_Modify(Form parentForm)
        {
            InitializeComponent();
            opener = parentForm;
            EmpNo = Get_Station.eEmp_No;
        }

        private void ReportError(string Message)
        {
            StackFrame CallStack = new StackFrame(1, true);
            MessageBox.Show("Error: " + Message + ",\n\rFile: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Btn_Close_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to close the program ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            if (List_Scrap.SelectedItem != null)
            {
                int Temp;

                Temp = List_Scrap.Items.IndexOf(List_Scrap.SelectedItem);

                if (List_Orginal.Items.IndexOf(List_Scrap.SelectedItem) != -1)
                {
                    showMessage("Dup SN: " + List_Scrap.SelectedItem + " in List Orginal !", "Trùng lặp SN: " + List_Scrap.SelectedItem + " trong List Orginal!", true);
                    List_Orginal.SelectedIndex = List_Orginal.Items.IndexOf(List_Scrap.SelectedItem);
                    
                    if(List_Scrap.Items.Count >= 2)
                    {
                        if (Temp + 1 == List_Scrap.Items.Count)
                        {
                            List_Scrap.SelectedIndex = 0;
                        }
                        else
                        {
                            List_Scrap.SelectedIndex = List_Scrap.Items.IndexOf(List_Scrap.SelectedItem) + 1;
                        }
                    }
                    else
                    {
                        List_Scrap.SelectedIndex = List_Scrap.Items.IndexOf(List_Scrap.SelectedItem);
                    }
                    return;
                }
                else
                {
                    List_Orginal.Items.Add(List_Scrap.SelectedItem);
                    List_Scrap.Items.Remove(List_Scrap.SelectedItem);
                    List_Scrap.Refresh();

                    if (List_Scrap.Items.Count >= 2)
                    {
                        if(Temp == List_Scrap.Items.Count)
                        {
                            List_Scrap.SelectedIndex = 0;
                        }
                        else
                        {
                            List_Scrap.SelectedIndex = Temp;
                        }
                    }
                    else if (List_Scrap.Items.Count == 1)
                    {
                        List_Scrap.SelectedIndex = 0;
                    }

                }
            }
            else
            {
                showMessage("Please choose one SN !", "Hãy chọn 1 SN !", true);
                return;
            }

            if (List_Scrap.Items.Count != 0)
            {
                Lab_NameCount.Visible = true;
                Lab_Count.Visible = true;
                Lab_Count.Text = List_Scrap.Items.Count.ToString();
            }
            else
            {
                Lab_NameCount.Visible = false;
                Lab_Count.Visible = false;
            }
        }



        private void Btn_Add_Click(object sender, EventArgs e)
        {
            if (List_Orginal.SelectedItem != null)
            {
                List_Scrap.Items.Add(List_Orginal.SelectedItem);
                List_Orginal.Items.Remove(List_Orginal.SelectedItem);
                List_Orginal.Refresh();

                if (List_Orginal.Items.Count != 0)
                {
                    List_Orginal.SelectedIndex = 0;
                }
            }
            else
            {
                showMessage("Please choose one SN !", "Hãy chọn 1 SN !", true);
                return;
            }

            if (List_Scrap.Items.Count != 0)
            {
                Lab_NameCount.Visible = true;
                Lab_Count.Visible = true;
                Lab_Count.Text = List_Scrap.Items.Count.ToString();
            }
            else
            {
                Lab_NameCount.Visible = false;
                Lab_Count.Visible = false;
            }
        }

        private void Btn_Add_All_Click(object sender, EventArgs e)
        {
            if (List_Orginal.Items.Count != 0)
            {
                List_Scrap.Items.AddRange(List_Orginal.Items);
                List_Orginal.Items.Clear();
                List_Scrap.SelectedIndex = 0;
                Lab_NameCount.Visible = true;
                Lab_Count.Visible = true;
                Lab_Count.Text = List_Scrap.Items.Count.ToString();
            }
        }

        private void Btn_Clear_All_Click(object sender, EventArgs e)
        {
            if (List_Scrap.Items.Count != 0)
            {
                List_Orginal.Items.AddRange(List_Scrap.Items);
                List_Scrap.Items.Clear();
                List_Orginal.SelectedIndex = 0;
                Lab_NameCount.Visible = true;
                Lab_Count.Visible = true;
                Lab_Count.Text = List_Scrap.Items.Count.ToString();
            }
        }

        private async void PreS_Modify_Shown(object sender, EventArgs e)
        {

            if (await Check_Privilege())
            {
                Comb_ScrapNo.Enabled = true;
                Comb_ScrapNo.Focus();
                await GetScrapNO();
                Txt_SerialN.Enabled = true;
                Comb_ScrapNo.Text = "";
            }

            Txt_SN.Text = "";
            Txt_SN.Enabled = true;
            GrB_ScrapWithout.Enabled = false;
            Btn_SaveExcel.Enabled = false;
            Lab_NameCount.Visible = false;
            Lab_Count.Visible = false;
            Txt_SN.Focus();
            Lab_Success.Text = "0";
            Lab_Fail.Text = "0";

            sql = "SELECT* FROM SFIS1.C_PARAMETER_INI where PRG_NAME = 'PRESCRAP' AND VR_NAME IN('CPEII', 'SFO') AND VR_VALUE = 'TRUE' ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                CK_DB = "CPEII";
            }

            Gif_Load.Visible = false;
            Txt_Error.Text = "";
        }

        private async void Btn_Scrap_Click(object sender, EventArgs e)
        {
            Gif_Load.Visible = true;
            Success_Count = 0;
            Fail_Count = 0;
            Lab_Success.Text = "0";
            Lab_Fail.Text = "0";
            Txt_Error.Text += "#########################\n";

            if (List_Orginal.Items.Count == 0)
            {
                Gif_Load.Visible = false;
                showMessage("Please input SN in list Orginal To Be Scraped !", "Vui lòng chuyển SN sang lits Orginal To Be Scraped !", true);
                Txt_SN.Focus();
                Txt_SN.SelectAll();
                return;
            }

            for (int i = 0; i < List_Orginal.Items.Count; i++)
            {
                sql = " SELECT * FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER = '" + List_Orginal.Items[i].ToString() + "' AND GD ='BD2C' ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    Fail_Count = Fail_Count + 1;
                    Lab_Fail.Text = Fail_Count.ToString();
                    Txt_Error.Text += "" + List_Orginal.Items[i].ToString() + ": Đã tồn tại ở BD2C. Không sử dụng lại\n";
                    List_Orginal.Items.Remove(List_Orginal.Items[i].ToString());
                    i --;
                }
            }

            if (List_Orginal.Items.Count != 0)
            {
                await Not_Scrap();

                if (await Check_Privilege())
                {
                    await GetScrapNO();
                    Comb_ScrapNo.Text = "";
                }
                Gif_Load.Visible = false;
                Clear_Data();
                List_Orginal.Items.Clear();
                MessageBox.Show("Push out success: " + Lab_Success.Text + "", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Txt_SN.Focus();
                Txt_SN.SelectAll();
            }
            Gif_Load.Visible = false;
        }

        private async Task<string> ScrapKind(string C_Scrap_No)  
        {
            sql = " SELECT SCRAP_KIND, QTY, MO_NUMBER FROM SFISM4.R_SCRAP_T WHERE SCRAP_NO = '" + C_Scrap_No + "'  ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                C_Skind = result.Data["scrap_kind"]?.ToString() ?? "";
                VC_SCRAP_QTY = int.Parse(result.Data["qty"]?.ToString() ?? "");
                VC_MO_NUMBER = result.Data["mo_number"]?.ToString() ?? "";
            }
            return C_Skind;
        }                                                                                               


        private async Task UpdateScrapMOQty(string C_Mo_Number, int C_Qty)  
        {
            sql = " UPDATE SFISM4.R_MO_BASE_T SET TOTAL_SCRAP_QTY = TOTAL_SCRAP_QTY + " + C_Qty + "   ";

            if (VC_SCRAP_KIND != "4")
            {
                sql = sql + " ,TURN_OUT_QTY = TURN_OUT_QTY + " + C_Qty + " ";
            }

            sql = sql + " WHERE MO_NUMBER = '" + C_Mo_Number + "' ";

            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }

        }

        private async Task DeleteScrap(string C_Scrap_No)  
        {
            sql = " DELETE SFISM4.R_SCRAP_T WHERE SCRAP_NO ='" + C_Scrap_No + "' AND MO_NUMBER = '" + Txt_Mo.Text + "' ";
            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }
        }
        private async Task <bool> CHECK_CHARACTER_0_9A_Z(string TMPSTR)
        {
            var regexItem = new Regex("^[0-9A-Z]*$");
            if (!regexItem.IsMatch(TMPSTR))
            {
                showMessage("" + TMPSTR + " - Invalid characters(0_9 A_Z) !", "" + TMPSTR + " - Ký tự không hợp lệ (0_9 A_Z) !", true);
                return false;
            }

            return true;
        }

        private async Task GetScrapSN(string C_Scrap_No) 
        {
            sql = " SELECT SERIAL_NUMBER FROM SFISM4.R_SCRAP_T WHERE SCRAP_NO = '" + C_Scrap_No + "'  ORDER BY SERIAL_NUMBER ";
            var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list.Data != null)
            {
                foreach (var dtRow in result_list.Data)
                {
                    if (!string.IsNullOrEmpty(dtRow["serial_number"]?.ToString() ?? ""))
                    {
                        if(List_Scrap.Items.IndexOf(dtRow["serial_number"].ToString()) != -1)
                        {
                            Comb_ScrapNo.SelectAll();
                        }
                        else
                        {
                            List_Scrap.Items.Add(dtRow["serial_number"].ToString());
                        }
                    }
                }
                
                if(List_Scrap.Items.Count > 0)
                {
                    Lab_NameCount.Visible = true;
                    Lab_Count.Visible = true;
                    Lab_Count.Text = List_Scrap.Items.Count.ToString();
                }
                else
                {
                    Lab_NameCount.Visible = false;
                    Lab_Count.Visible = false;
                }
            }
        }

        private async Task DeleteScrapSN(string C_SN)  
        {
            sql = " UPDATE  SFISM4.R_SCRAP_T SET EMP_NO ='" + EmpNo + "', IN_STATION_TIME = SYSDATE WHERE SERIAL_NUMBER ='" + C_SN + "' ";
            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }

            sql = " INSERT INTO SFISM4.H_SCRAP_T (SELECT * FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER ='" + C_SN + "') ";
            a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }

            sql = "  DELETE SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER ='" + C_SN + "' ";
            a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }
        }
        private async Task<bool> Find_Mo(string C_SN)
        {
            C_MoNumber = "";

            sql = " SELECT MO_NUMBER FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + C_SN + "'  ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                C_MoNumber = result.Data["mo_number"].ToString();
            }

            sql = " SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER =  '" + C_MoNumber + "' ";
            result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                Txt_Error.Text += "" + C_MoNumber + " Không tồn tại trong R105 !\n";
                return false;
            }
            return true;
        }

        private async Task GetScrapNO()
        {
            sql = "  SELECT DISTINCT SCRAP_NO FROM SFISM4.R_SCRAP_T WHERE SCRAP_FLAG ='2' ORDER BY SCRAP_NO ";
            var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list.Data.Count() != 0)
            {
                foreach (var dtRow in result_list.Data)
                {
                    if (!string.IsNullOrEmpty(dtRow["scrap_no"]?.ToString() ?? ""))
                    {
                        Comb_ScrapNo.Items.Add(dtRow["scrap_no"].ToString());
                    }
                }
            }
        }

        private async Task Check_In_Out(string C_SN) 
        {
            sql = " SELECT * FROM SFISM4.R107 WHERE WIP_GROUP IN ('BC8M', 'BC3F', 'BCFA') AND ERROR_FLAG ='7' AND SERIAL_NUMBER = '" + C_SN + "'  ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                sql = " SELECT * FROM SFISM4.R_REPAIR_IN_OUT_T WHERE SERIAL_NUMBER = '" + C_SN + "' AND OUT_DATETIME IS NOT NULL AND R_SENDER IS NULL ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    sql = "  INSERT INTO SFISM4.R_REPAIR_IN_OUT_T  ";
                    sql = sql + " (SERIAL_NUMBER,LINE_NAME,STATION_NAME,MO_NUMBER,P_SENDER,R_RECEIVER,IN_DATETIME,R_SENDER,P_RECEIVER,OUT_DATETIME,STATUS,B_TYPE,RE_TYPE)  ";
                    sql = sql + " SELECT SERIAL_NUMBER,LINE_NAME,STATION_NAME,MO_NUMBER,P_SENDER,R_RECEIVER,SYSDATE, '', '', '',STATUS,B_TYPE,RE_TYPE FROM SFISM4.R_REPAIR_IN_OUT_T  ";
                    sql = sql + " WHERE SERIAL_NUMBER = '" + C_SN + "' AND OUT_DATETIME IS NOT NULL AND P_RECEIVER ='NEW'  ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }

                    sql = " UPDATE  SFISM4.R_REPAIR_IN_OUT_T SET R_SENDER ='SYSTEM',P_RECEIVER ='SYSTEM'  ";
                    sql = sql + " WHERE SERIAL_NUMBER = '" + C_SN + "' AND OUT_DATETIME IS NOT NULL AND R_SENDER IS NULL AND P_RECEIVER ='NEW'  ";
                    a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }

                }
            }
        }
        private async Task<string> R109_BAK(string C_SN) 
        {
            sql = " SELECT * FROM SFISM4.R107 WHERE SERIAL_NUMBER = '" + C_SN + "' AND ERROR_FLAG ='7' ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                sql = " UPDATE SFISM4.R109 SET REPAIRER =''  WHERE SERIAL_NUMBER ='" + C_SN + "' AND  REPAIRER ='SYSTEM' ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return "Fail";
                }
            }
            else
            {
                sql = " INSERT INTO SFISM4.R_REPAIR_T ";
                sql = sql + " (SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,TEST_TIME,TEST_CODE,TEST_STATION,TEST_GROUP,TEST_SECTION,TEST_LINE,TESTER,REPAIRER,REPAIR_TIME,  ";
                sql = sql + " REASON_CODE,REPAIR_STATION,REPAIR_GROUP,REPAIR_SECTION,REPAIR_STATUS,DUTY_STATION,DUTY_TYPE,ERROR_ITEM_CODE,RECORD_TYPE,SOLDER_COUNT,  ";
                sql = sql + " DESCRIP,ATE_STATION_NO,T_WORK_SECTION,T_CLASS,T_CLASS_DATE,R_WORK_SECTION,R_CLASS,R_CLASS_DATE,MOVE_FLAG,MEMO )   ";
                sql = sql + " SELECT SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,TEST_TIME,TEST_CODE,TEST_STATION,TEST_GROUP,TEST_SECTION,TEST_LINE,TESTER,REPAIRER,  ";
                sql = sql + " REPAIR_TIME,REASON_CODE,REPAIR_STATION,REPAIR_GROUP,REPAIR_SECTION,REPAIR_STATUS,DUTY_STATION,DUTY_TYPE,ERROR_ITEM_CODE,RECORD_TYPE,  ";
                sql = sql + " SOLDER_COUNT,DESCRIP,ATE_STATION_NO,T_WORK_SECTION,T_CLASS,T_CLASS_DATE,R_WORK_SECTION,R_CLASS,R_CLASS_DATE,MOVE_FLAG,MEMO  ";
                sql = sql + " FROM SFISM4.h109 WHERE SERIAL_NUMBER ='" + C_SN + "' AND DESCRIP LIKE 'BonePile%' ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return "Fail";
                }

                sql = " DELETE SFISM4.h109 WHERE SERIAL_NUMBER ='" + C_SN + "' AND DESCRIP like 'BonePile%' ";
                a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return "Fail";
                }
            }
            return "True";
        }

        private  void Clear_Data()
        {
            Comb_ScrapNo.Text = "";
            Txt_SN.Text = "";
            Txt_SerialN.Text = "";
            Txt_Mo.Text = "";
            Txt_QtyModify.Text = "";
            Txt_QtyOrgin.Text = "";
            GrB_ScrapWithout.Enabled = false;
            Btn_SaveExcel.Enabled = false;
            Line = ""; Section = "";
            Group = ""; Station = ""; WipGroup = "";

            GridView_SnDetail.DataSource = null;
            GridView_SnDetail.Rows.Clear();
            GridView_SnDetail.Refresh();
        }
        private async Task UpdateR107(string C_SN)  
        {
            if ((!string.IsNullOrEmpty(Group)) && (!string.IsNullOrEmpty(WipGroup)))
            {
                sql = "  UPDATE SFISM4.R_WIP_TRACKING_T SET SCRAP_FLAG= '0', LINE_NAME = '" + Line + "', SECTION_NAME ='" + Section + "', ";
                sql = sql + "  GROUP_NAME = '" + Group + "', STATION_NAME ='" + Station + "', WIP_GROUP = '" + WipGroup + "' WHERE SERIAL_NUMBER = '" + C_SN + "' AND SCRAP_FLAG = '2' ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }
            }
            else
            {
                Fail_Count = Fail_Count + 1;
                Lab_Fail.Text = Fail_Count.ToString();
                Txt_Error.Text += "" + Txt_SN.Text + ": GROUP_NAME or QC_EMP_DATA in Scrap null\n";
                return;
            }
        }
        private async Task Insert_R117(string C_SN)  
        {
            sql = "  INSERT INTO SFISM4.R_SN_DETAIL_T(SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, LINE_NAME,  SECTION_NAME, GROUP_NAME, STATION_NAME, LOCATION, STATION_SEQ, ERROR_FLAG,  ";
            sql = sql + "  IN_STATION_TIME, SHIPPING_SN, WORK_FLAG, FINISH_FLAG, ENC_CNT, SPECIAL_ROUTE, PALLET_NO, CONTAINER_NO, QA_NO,QA_RESULT, SCRAP_FLAG, NEXT_STATION, CUSTOMER_NO, BOM_NO,  ";
            sql = sql + "  KEY_PART_NO, CARTON_NO, REPAIR_CNT, EMP_NO, PALLET_FULL_FLAG, GROUP_NAME_CQC, MSN, IMEI, JOB, SO_NUMBER, SO_LINE, STOCK_NO, TRAY_NO, SHIP_NO, WIP_GROUP, SHIPPING_SN2)  ";
            sql = sql + "  SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, LINE_NAME,  SECTION_NAME, WIP_GROUP, STATION_NAME, LOCATION, STATION_SEQ,  ERROR_FLAG, SYSDATE, SHIPPING_SN,  ";
            sql = sql + "  WORK_FLAG, FINISH_FLAG, ENC_CNT, SPECIAL_ROUTE, PALLET_NO, CONTAINER_NO, QA_NO,QA_RESULT, SCRAP_FLAG, NEXT_STATION, CUSTOMER_NO, BOM_NO, KEY_PART_NO, CARTON_NO, REPAIR_CNT,  ";
            sql = sql + "  EMP_NO, PALLET_FULL_FLAG, GROUP_NAME_CQC, MSN, IMEI, JOB, SO_NUMBER, SO_LINE, STOCK_NO, TRAY_NO, SHIP_NO, '" + WipGroup + "', SHIPPING_SN2 FROM SFISM4.R107 WHERE SERIAL_NUMBER ='" + C_SN + "' ";
            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }
        }


        private async Task<bool> Get_Data_Scrap(String Sn_Scrap) // For CPEII, SFO
        {
            sql = " SELECT * FROM SFISM4.R_SCRAP_T  WHERE SERIAL_NUMBER = '" + Sn_Scrap + "'  AND GROUP_NAME  AND  GROUP_NAME  IN ('TEST_PRE_SC', 'SMT_PRE_SC')  ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                sql = " DELETE SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER = '" + Sn_Scrap + "'  AND  GROUP_NAME  IN ('TEST_PRE_SC', 'SMT_PRE_SC')  ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return false;
                }
            }

            sql = " SELECT * FROM SFISM4.R_SCRAP_T  WHERE SERIAL_NUMBER = '" + Sn_Scrap + "'  AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ";
            result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                sql = " SELECT * FROM SFISM4.H_SCRAP_T WHERE SERIAL_NUMBER = '" + Sn_Scrap + "' AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    sql = " INSERT INTO SFISM4.R_SCRAP_T ";
                    sql = sql + " SELECT DISTINCT * FROM SFISM4.H_SCRAP_T A WHERE SERIAL_NUMBER = '" + Sn_Scrap + "' AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ";
                    sql = sql + " AND IN_STATION_TIME = (SELECT MAX(IN_STATION_TIME) FROM SFISM4.H_SCRAP_T WHERE SERIAL_NUMBER = A.SERIAL_NUMBER AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ) ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return false;
                    }
                }
                else
                {
                    Txt_Error.Text += "" + Txt_SN.Text + ": không có dữ liệu Scrap\n";
                    return false;
                }
            }
            else
            {
                return true;
            }
            return true;
        }

        private async Task<bool> Find_Detail(string C_SN) 
        {
            String Mo_Sn = "", Mo_Scrap = "";
            Line = ""; Section = ""; Group = ""; Station = ""; WipGroup = "";

            sql = " SELECT MO_NUMBER FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + C_SN + "'  ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                Mo_Sn = result.Data["mo_number"].ToString();
            }
            else
            {
                Txt_Error.Text += "" + C_SN + " - Không tồn tại trong R107 !\n";
                return false;
            }

            sql = " SELECT * FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER = '" + C_SN + "'  AND MO_NUMBER = '"+ Mo_Sn + "' ";
            result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                sql = " SELECT * FROM SFISM4.R_SCRAP_T A  WHERE SERIAL_NUMBER = '" + C_SN + "' AND MO_NUMBER = '" + Mo_Sn + "'  AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ";
                sql = sql + " AND IN_STATION_TIME = (SELECT MAX(IN_STATION_TIME) FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER=A.SERIAL_NUMBER AND MO_NUMBER=A.MO_NUMBER AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ) AND ROWNUM = 1";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    Line = result.Data["line_name"]?.ToString() ?? "";
                    Section = result.Data["section_name"]?.ToString() ?? "";
                    Group = result.Data["group_name"]?.ToString() ?? "";
                    Station = result.Data["station_name"]?.ToString() ?? "";
                    WipGroup = result.Data["qc_emp_data"]?.ToString() ?? "";
                }
                else
                {
                    Txt_Error.Text += "" + C_SN + " :Không có dữ liệu Scrap !\n";
                    return false;
                }
            }
            else
            {
                sql = " SELECT * FROM SFISM4.R_SCRAP_T  WHERE SERIAL_NUMBER = '" + C_SN + "' ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    Mo_Scrap = result.Data["mo_number"]?.ToString() ?? "";
                }

                Txt_Error.Text += "" + C_SN + " - Mo_Sn: " + Mo_Sn + " <> " + Mo_Scrap + ":Mo_Sc SN với MO không khớp !\n";
                return false;
            }

            if(string.IsNullOrEmpty(Group) || string.IsNullOrEmpty(WipGroup))
            {
                Txt_Error.Text += "" + C_SN + " - Group Or WipGroup in R_SCRAP_T is NULL !\n";
                return false;
            }
            else
            {
                return true;
            }           
        }

        private async void Btn_Confirm_Click(object sender, EventArgs e)
        {
            int Qty;

            if (!await CHECK_CHARACTER_0_9(Txt_QtyModify.Text))
            {
                Txt_QtyModify.Focus();
                Txt_QtyModify.SelectAll();
                return;
            }

            try
            {
                Qty = Int32.Parse(Txt_QtyModify.Text);
            }
            catch
            {
                showMessage("" + Txt_QtyModify.Text + " - Invalid characters !", "" + Txt_QtyModify.Text + " - Ký tự không hợp lệ !", true);
                Txt_QtyModify.Focus();
                Txt_QtyModify.SelectAll();
                return;
            }

            if(Qty < 0)
            {
                showMessage("Modify Qty < 0 !", "Modify Qty < 0 !", true);
                Txt_QtyModify.Focus();
                Txt_QtyModify.SelectAll();
                return;
            }
            else if(Qty == 0)
            {
                await UpdateScrapMOQty(Txt_Mo.Text, 0 - VC_SCRAP_QTY);
                await DeleteScrap(Comb_ScrapNo.Text);
            }
            else
            {
                if (!await CHECK_CHARACTER_0_9(Txt_QtyOrgin.Text))
                {
                    Txt_QtyOrgin.Focus();
                    Txt_QtyOrgin.SelectAll();
                    return;
                }

                if (!await Compute_Qty(Txt_Mo.Text, Int32.Parse(Txt_QtyModify.Text)))
                {
                    showMessage("The Scrap_Qty + Input_Qty > Target Qty !", "Txt_QtyModify: "+ Txt_QtyModify.Text + " + TOTAL_SCRAP_QTY - TURN_OUT_QTY + INPUT_QTY > TARGET_QTY !", true);
                    Txt_QtyModify.Focus();
                    Txt_QtyModify.SelectAll();
                    return;
                }
                else
                {
                    await UpdateScrapMOQty(Txt_Mo.Text, 0 - VC_SCRAP_QTY);
                    await UpdateScrapQty(Comb_ScrapNo.Text, Int32.Parse(Txt_QtyModify.Text));
                    await UpdateScrapMOQty(Txt_Mo.Text, Int32.Parse(Txt_QtyModify.Text));
                }
            }

            Clear_Data();
            await GetScrapNO();
        }

        private async Task UpdateScrapQty(string Scrap_no, int C_Qty) 
        {
            sql = " UPDATE SFISM4.R_SCRAP_T SET QTY = '"+ C_Qty + "' WHERE SCRAP_NO ='" + Scrap_no + "' AND MO_NUMBER ='" + Txt_Mo.Text + "'  ";
            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }
        }

        private async Task<Boolean> Compute_Qty(string C_Mo, int C_Qty)
        {
            sql = " SELECT INPUT_QTY, TARGET_QTY, TOTAL_SCRAP_QTY, TURN_OUT_QTY   FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '" + C_Mo + "'";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                int C_Total_scrap = int.Parse(result.Data["total_scrap_qty"]?.ToString()?? "0");
                int C_Turn_out = int.Parse(result.Data["turn_out_qty"]?.ToString() ?? "0");
                int C_Input_qty = int.Parse(result.Data["input_qty"]?.ToString() ?? "0");
                int C_Ttarget = int.Parse(result.Data["target_qty"]?.ToString() ?? "0");

                if ((C_Qty - Int32.Parse(Txt_QtyOrgin.Text) + C_Total_scrap - C_Turn_out + C_Input_qty) > C_Ttarget)
                {
                    return false;
                }
            }
            else
            {
                showMessage("" + C_Mo + " Not found in R105 !", "" + C_Mo + " Không tìm thấy trong R105 !", true);
                return false;
            }
            return true;
        }

        private void Ck_Cut_Click(object sender, EventArgs e)
        {
            if(Ck_Cut.Checked)
            {
                Txt_Begin.Visible = true;
                Txt_End.Visible = true;
                C_Keo.Visible = true;
                Txt_Begin.Focus();
            }
            else
            {
                Txt_Begin.Visible = false;
                Txt_End.Visible = false;
                C_Keo.Visible = false;
                Txt_SN.Focus();
            }
        }

        private async void Txt_Begin_TextChanged(object sender, EventArgs e)
        {
            await Check_Begin();
        }

        private async Task<bool> Check_Begin()
        {
            if (!await CHECK_CHARACTER_0_9(Txt_Begin.Text))
            {
                Txt_Begin.Focus();
                Txt_Begin.SelectAll();
                return false;
            }

            if (Txt_Begin.Text.Length > 2)
            {
                showMessage("" + Txt_Begin.Text + " - Length is too big !\nLength > 2", "" + Txt_Begin.Text + " - Độ dài quá lớn !", true);
                Txt_Begin.Focus();
                Txt_Begin.SelectAll();
                return false;
            }

            return true;
        }

        

        private async void Txt_End_TextChanged(object sender, EventArgs e)
        {
            await Check_End();
        }

        private async Task<bool> Check_End()
        {
            if (!await CHECK_CHARACTER_0_9(Txt_End.Text))
            {
                Txt_End.Focus();
                Txt_End.SelectAll();
                return false;
            }

            if (Txt_End.Text.Length > 2)
            {
                showMessage("" + Txt_End.Text + " - Length is too big !\nLength > 2", "" + Txt_End.Text + " - Độ dài quá lớn !", true);
                Txt_End.Focus();
                Txt_End.SelectAll();
                return false;
            }

            return true;
        }

        private void Btn_SaveExcel_Click(object sender, EventArgs e)
        {
            export();
        }
        async void export()
        {
            if (GridView_SnDetail.RowCount > 0)
            {
                string filePath = "";
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Excel| *.xlsx | Excel 2003| *.xls";
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                filePath = dialog.FileName;
                if (string.IsNullOrEmpty(filePath))
                {
                    showMessage("Path file null or empty! Try again...", "Đường dẫn file rỗng hoặc không tồn tại! Hãy thử lại...", true);
                    return;
                }

                if (await ExportToExcel(dt, filePath))
                {
                    var Result = MessageBox.Show("Excel file saved. Open file now?", "Answer Yes or No?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Result == DialogResult.Yes)
                    {
                        Process.Start(filePath);
                        return;
                    }
                }
                else
                {
                    showMessage("Excel file false!", "Xuất file không thành công !", true);
                }
            }
        }



        private async Task<bool> ExportToExcel(DataTable dataTable, string filePath)
        {
            try
            {
                if (GridView_SnDetail.RowCount > 0)
                {
                    Microsoft.Office.Interop.Excel.Application excel = null;
                    Microsoft.Office.Interop.Excel.Workbook wb = null;
                    object missing = Type.Missing;
                    Microsoft.Office.Interop.Excel.Worksheet ws = null;
                    Microsoft.Office.Interop.Excel.Range rng = null;

                    excel = new Microsoft.Office.Interop.Excel.Application();
                    wb = excel.Workbooks.Add();
                    ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.ActiveSheet;
                    ws.Columns.EntireColumn.AutoFit();

                    for (int Idx = 0; Idx < dataTable.Columns.Count; Idx++)
                    {
                        ws.Range["A1"].Offset[0, Idx].Value = dataTable.Columns[Idx].ColumnName;
                    }

                    for (int Idx = 0; Idx < dataTable.Rows.Count; Idx++)
                    {
                        ws.Range["A2"].Offset[Idx].Resize[1, dataTable.Columns.Count].NumberFormat = "@";
                        ws.Range["A2"].Offset[Idx].Resize[1, dataTable.Columns.Count].Value = dataTable.Rows[Idx].ItemArray;
                    }

                    excel.Visible = false;
                    ws.SaveAs(filePath);
                    excel.Quit();
                }
                return true;
            }
            catch (Exception ex)
            {
                showMessage("Exception: " + ex.Message, "Ngoại lệ: " + ex.Message, true);
                return false;
            }
        }



        private async void Txt_SerialN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sql = " SELECT DISTINCT SCRAP_NO FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER = '" + Txt_SerialN.Text + "' AND ROWNUM = 1 ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    Comb_ScrapNo.Text = result.Data["scrap_no"].ToString();
                    await Option_Scrap();
                    Txt_SerialN.SelectAll();
                }
                else
                {
                    showMessage("SN: " + Txt_SerialN.Text + " not in table SCRAP_T. Not found Scrap_No !", "SN: " + Txt_SerialN.Text + " không tìm thấy trong SCRAP_T. Không tìm thấy Scrap_No !", true);
                    GridView_SnDetail.DataSource = null;
                    GridView_SnDetail.Rows.Clear();
                    GridView_SnDetail.Refresh();
                    Comb_ScrapNo.Text = "";
                    List_Orginal.Items.Clear();
                    Txt_SerialN.Focus();
                    Txt_SerialN.SelectAll();
                    return;
                }
            }
        }

        private void GridView_SnDetail_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                DataGridViewRow row = GridView_SnDetail.Rows[e.RowIndex];
                Txt_Mo.Text = row.Cells[2].Value.ToString();
                Txt_QtyOrgin.Text = row.Cells[14].Value.ToString();
            }
        }

        private async void Comb_ScrapNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                List_Scrap.Items.Clear();
                await GetScrapSN(Comb_ScrapNo.Text);
            }
        }

        private async void Comb_ScrapNo_TextChanged(object sender, EventArgs e)
        {
            await Option_Scrap();

            if (List_Scrap.Items.Count != 0)
            {
                List_Scrap.SelectedIndex = 0;
            }

            Txt_Mo.Text = "";
            Txt_QtyModify.Text = "";
            Txt_QtyOrgin.Text = "";
        }
        private async Task Option_Scrap()
        {
            List_Scrap.Items.Clear();

            await GetScrapSN(Comb_ScrapNo.Text);

            VC_SCRAP_KIND_REAL = await ScrapKind(Comb_ScrapNo.Text);

            if ((VC_SCRAP_KIND_REAL == "2") ||
                (VC_SCRAP_KIND_REAL == "4") ||
                (VC_SCRAP_KIND_REAL == "6") ||
                (VC_SCRAP_KIND_REAL == "7"))
            {
                VC_SCRAP_KIND = "4";
                GrB_ScrapWithout.Enabled = true;
                Txt_Mo.Text = VC_MO_NUMBER;
                Txt_QtyOrgin.Text = VC_SCRAP_QTY.ToString();
                Ck_ChangeQty.Checked = false;
                Txt_QtyModify.Visible = false;
                Btn_Confirm.Visible = false;
                Txt_SN.Text = "";
                Txt_SN.Enabled = false;
            }
            else
            {
                VC_SCRAP_KIND = "1";
                GrB_ScrapWithout.Enabled = false;
                VC_SCRAP_KIND = VC_SCRAP_KIND_REAL;
                Txt_SN.Text = "";
                Txt_SN.Enabled = true;

                GridView_SnDetail.DataSource = null;
                GridView_SnDetail.Rows.Clear();
                GridView_SnDetail.Refresh();
            }

            FindScrapSNDetail(Comb_ScrapNo.Text);
        }

        private async Task Not_Scrap()
        {
            if (List_Orginal.Items.Count != 0)
            {
                if (VC_SCRAP_KIND != "4")
                {
                    for (int i = 0; i < List_Orginal.Items.Count; i++)
                    {
                        if (await Find_Detail(List_Orginal.Items[i].ToString()))
                        {
                            if (await Find_Mo(List_Orginal.Items[i].ToString()))
                            {
                                if ((!string.IsNullOrEmpty(Group)) && (!string.IsNullOrEmpty(WipGroup)))
                                {
                                    await Check_In_Out(List_Orginal.Items[i].ToString());
                                    await Insert_R117(List_Orginal.Items[i].ToString());
                                    await UpdateR107(List_Orginal.Items[i].ToString());
                                    await R109_BAK(List_Orginal.Items[i].ToString());
                                    await UpdateScrapMOQty(C_MoNumber, -1);
                                    await DeleteScrapSN(List_Orginal.Items[i].ToString());

                                    Success_Count = Success_Count + 1;
                                    Lab_Success.Text = Success_Count.ToString();
                                }
                                else
                                {
                                    Fail_Count = Fail_Count + 1;
                                    Lab_Fail.Text = Fail_Count.ToString();
                                    Txt_Error.Text += "" + Txt_SN.Text + ": GROUP_NAME or QC_EMP_DATA in Scrap null\n";
                                }
                            }
                            else
                            {
                                Fail_Count = Fail_Count + 1;
                                Lab_Fail.Text = Fail_Count.ToString();
                            }
                        }
                        else
                        {
                            Fail_Count = Fail_Count + 1;
                            Lab_Fail.Text = Fail_Count.ToString();
                        }
                    }
                }
            }
        }
        
        private async void FindScrapSNDetail(string C_Scrap)
        {
            if (await CHECK_CHARACTER_0_9A_Z(Comb_ScrapNo.Text))
            {
                sql = " SELECT  SCRAP_NO, B.EMP_NAME, MO_NUMBER, MODEL_NAME, VERSION_CODE, SERIAL_NUMBER, PRE_SCRAP_GROUP_NAME,IN_STATION_TIME PRE_SCRAP_TIME,   ";
                sql = sql + " KEY_PART_NO,  SCRAP_REASON, DECODE(SCRAP_KIND, '1','Scrap with SN', '2','Scrap', '3','Scrap without entity with SN', '4','Scrap without entity',  ";
                sql = sql + " '5','Turn Out with SN', '6','Turn Out', '7','Shortage', SCRAP_KIND) SCRAP_KIND , REASON_CODE,  REASON_TYPE, DUTY_STATION, QTY, GD, LOC ";
                sql = sql + " FROM SFISM4.R_SCRAP_T A, SFIS1.C_EMP_DESC_T B WHERE SCRAP_NO = '" + C_Scrap + "' AND A.EMP_NO = B.EMP_NO  ORDER BY SERIAL_NUMBER ";
                var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list1.Data.Count() != 0)
                {
                    Btn_SaveExcel.Enabled = true;

                    var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
                    dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                    GridView_SnDetail.DataSource = dt;
                    GridView_SnDetail.AutoResizeColumns();
                    GridView_SnDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }
                else
                {
                    Btn_SaveExcel.Enabled = false;
                }
            }
        }

        private void Ck_ChangeQty_Click(object sender, EventArgs e) 
        {
            if(Ck_ChangeQty.Checked)
            {
                Txt_QtyModify.Text = "";
                Txt_QtyModify.Visible = true;
                Btn_Confirm.Visible = true;
                Txt_QtyModify.Focus();
            }
            else
            {
                Txt_QtyModify.Text = "";
                Txt_QtyModify.Visible = false;
                Btn_Confirm.Visible = false;
            }
        }

        private async void Txt_SN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await Load_SN_Key();

                sql = " SELECT * FROM SFISM4.R_SCRAP_T A WHERE SERIAL_NUMBER = '" + Txt_SN.Text + "' AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ";
                sql = sql + " AND IN_STATION_TIME = (SELECT MAX(IN_STATION_TIME) FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER=A.SERIAL_NUMBER AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') )   ";
                var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list1.Data.Count() != 0)
                {
                    var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
                    dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                    GridView_SnDetail.DataSource = dt;
                    GridView_SnDetail.AutoResizeColumns();
                    GridView_SnDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }
            }
        }

        private async Task Load_SN_Key() 
        {
            if(Ck_Cut.Checked) 
            {
                if (Txt_Begin.Text.Length == 0)
                {
                    showMessage("Please enter where to cut the string !", "Vui lòng nhập vị trí cần cắt chuỗi !", true);
                    Txt_Begin.Focus();
                    Txt_Begin.SelectAll();
                    return;
                }

                if (Txt_End.Text.Length == 0)
                {
                    showMessage("Please enter the number of characters to cut !", "Vui lòng nhập số lượng ký tự cần cắt !", true);
                    Txt_End.Focus();
                    Txt_End.SelectAll();
                    return;
                }

                if (!await Check_Begin())
                {
                    return;
                }

                if (!await Check_End())
                {
                    return;
                }

                if (Txt_SN.Text.Length < (Int32.Parse(Txt_Begin.Text) + Int32.Parse(Txt_End.Text)))
                {
                    showMessage("The strings length is not enough for cutting. Please check again !", "Độ dài chuỗi không đủ để cắt. Vui lòng kiểm tra lại !", true);
                    Txt_End.Focus();
                    Txt_End.SelectAll();
                    return;
                }

                Txt_SN.Text = Txt_SN.Text.Substring(Int32.Parse(Txt_Begin.Text), Int32.Parse(Txt_End.Text));
            }

            int CK = List_Scrap.Items.IndexOf(Txt_SN.Text);

            if (CK != -1)
            {
                if (List_Orginal.Items.IndexOf(Txt_SN.Text) != -1)
                {
                    Txt_Error.Text += "" + Txt_SN.Text + ": DUP trong List Orginal\n";
                    Txt_SN.Focus();
                    Txt_SN.SelectAll();
                    List_Orginal.SelectedIndex = List_Orginal.Items.IndexOf(Txt_SN.Text);
                    return;
                }
                else
                {
                    List_Orginal.Items.Add(Txt_SN.Text);
                    List_Scrap.Items.Remove(Txt_SN.Text);

                    if (List_Scrap.Items.Count != 0)
                    {
                        Lab_NameCount.Visible = true;
                        Lab_Count.Visible = true;
                        Lab_Count.Text = List_Scrap.Items.Count.ToString();
                    }
                    else
                    {
                        Lab_NameCount.Visible = false;
                        Lab_Count.Visible = false;
                    }
                }
            }
            else
            {
                sql = " SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE KEY_PART_SN = '" + Txt_SN.Text + "' AND ROWNUM = 1 ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    Txt_SN.Text = result.Data["serial_number"].ToString();
                }

                sql = " SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + Txt_SN.Text + "'  ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    if (result.Data["scrap_flag"].ToString() != "2")
                    {
                        Txt_Error.Text += "" + Txt_SN.Text + ": SCRAP_FLAG <> 2\n";
                        Txt_SN.Focus();
                        Txt_SN.SelectAll();
                        return;
                    }
                }
                else
                {
                    Txt_Error.Text += "" + Txt_SN.Text + ": Not exist R107\n";
                    Txt_SN.Focus();
                    Txt_SN.SelectAll();
                    return;
                }

                if (CK_DB == "CPEII")
                {
                    if (!await Get_Data_Scrap(Txt_SN.Text)) // For CPEII, SFO
                    {
                        return;
                    }
                }

                sql = " SELECT * FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER = '" + Txt_SN.Text + "' AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data == null)
                {
                    sql = " SELECT * FROM SFISM4.H_SCRAP_T WHERE SERIAL_NUMBER = '" + Txt_SN.Text + "' AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ";
                    result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result.Data == null)
                    {
                        Txt_Error.Text += "" + Txt_SN.Text + ": Không tìm thấy (R-H)_SCRAP_T\n";
                        Txt_SN.Focus();
                        Txt_SN.SelectAll();
                        return;
                    }
                    else
                    {
                        sql = " INSERT INTO SFISM4.R_SCRAP_T ";
                        sql = sql + " SELECT DISTINCT * FROM SFISM4.H_SCRAP_T A WHERE SERIAL_NUMBER = '" + Txt_SN.Text + "' AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ";
                        sql = sql + " AND IN_STATION_TIME = (SELECT MAX(IN_STATION_TIME) FROM SFISM4.H_SCRAP_T WHERE SERIAL_NUMBER=A.SERIAL_NUMBER AND  GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ) AND ROWNUM = 1 ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return;
                        }
                    }
                }

                int CK1 = List_Orginal.Items.IndexOf(Txt_SN.Text);
                if (CK1 == -1)
                {
                    List_Scrap.Items.Add(Txt_SN.Text);

                    if (List_Scrap.Items.Count != 0)
                    {
                        Lab_NameCount.Visible = true;
                        Lab_Count.Visible = true;
                        Lab_Count.Text = List_Scrap.Items.Count.ToString();
                    }
                    else
                    {
                        Lab_NameCount.Visible = false;
                        Lab_Count.Visible = false;
                    }
                }
                else
                {
                    Txt_Error.Text += "" + Txt_SN.Text + ": DUP trong List Orginal\n";
                    Txt_SN.Focus();
                    Txt_SN.SelectAll();
                    List_Orginal.SelectedIndex = List_Orginal.Items.IndexOf(Txt_SN.Text);
                    return;
                }

            }

            Txt_SN.Focus();
            Txt_SN.SelectAll();
        }

        private async void Btn_Excel_Click(object sender, EventArgs e)
        {
            string path = "";
            string C_SN;

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

                    Gif_Load.Visible = true;
                    for (int i = 0; i < countRow; i++)
                    {
                        C_SN = dt.Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(C_SN))
                        {
                            Txt_SN.Text = C_SN;
                            await Load_SN_Key();
                        }
                    }
                    await Qury_Total();
                    Gif_Load.Visible = false;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private async Task Qury_Total()
        {
            sql = " SELECT * FROM SFISM4.R_SCRAP_T A WHERE GROUP_NAME <> 'TEST_PRE_SC' AND SERIAL_NUMBER IN ( '' ";
            for (int i = 0; i < List_Scrap.Items.Count; i++)
            {
                sql = sql + ", '" + List_Scrap.Items[i].ToString() + "'";
            }
            sql = sql + " ) AND IN_STATION_TIME = (SELECT MAX(IN_STATION_TIME) FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER=A.SERIAL_NUMBER AND GROUP_NAME <> 'TEST_PRE_SC'  ) ";
            var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
            dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
            GridView_SnDetail.DataSource = dt;
            GridView_SnDetail.AutoResizeColumns();
            GridView_SnDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }

        private async void Btn_InputText_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Text Files (.txt*)|*.txt*|All Files (*.*)|*.*";
            openFileDialog.Multiselect = false;

            try
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                DialogResult dlgResult = openFileDialog.ShowDialog();
                if (dlgResult == DialogResult.OK)
                {
                    foreach (string filename in openFileDialog.FileNames)
                        Path.GetFileName(openFileDialog.FileNames.ToString());
                    string[] dataSN = File.ReadAllLines(openFileDialog.FileName.ToString());
                    if (dataSN != null)
                    {
                        Gif_Load.Visible = true;
                        foreach (string C_SN in dataSN)
                        {
                            if (!string.IsNullOrEmpty(C_SN))
                            {
                                Txt_SN.Text = C_SN;
                                await Load_SN_Key();
                            }
                        }
                        await Qury_Total();
                        Gif_Load.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Btn_Back_Click(object sender, EventArgs e)
        {
            Clear_Data();
            this.Close();
            opener.Show();
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

        private async Task<Boolean> Check_Privilege()
        {
            sql = "  SELECT*FROM SFIS1.C_PRIVILEGE WHERE EMP = '" + EmpNo + "' AND PRG_NAME = 'PRESCRAP' AND FUN='SCRAP_NO' AND PRIVILEGE = 2  ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task<bool> CHECK_CHARACTER_0_9(string TMPSTR)
        {
            TMPSTR = TMPSTR.Trim();

            var regexItem = new Regex("^[0-9]*$");
            if (!regexItem.IsMatch(TMPSTR))
            {
                showMessage("" + TMPSTR + " - Invalid characters(0_9) !", "" + TMPSTR + " - Ký tự không hợp lệ (0_9) !", true);
                return false;
            }

            return true;
        }

    }
}
