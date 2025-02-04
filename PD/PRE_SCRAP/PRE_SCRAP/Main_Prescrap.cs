using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using PRE_SCRAP.Resources;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.IO;

namespace PRE_SCRAP
{
    public partial class Main_Prescrap : Form
    {

        public static string sStation;
        public String Temp_Station, sql, GD, EmpNo;
        string VC_REASON_TYPE, VC_DUTY_STATION;
        string Login_line, Login_section, Login_group, ConnectDB;
        DataTable dt;
        DataTable dtShow = new DataTable();
        Form opener;
        public SfcHttpClient _sfcHttpClient = DBAPI._sfcHttpClient;
        int iTotalScrapQty;
        public string C_Stock_No, C_Ship_No, C_Line, C_Section, C_Group, C_Station, C_WipGroup, C_Modelname, C_Version, C_KeyPartNo;
        public string C_GroupLogin, C_LineLogin, C_SectionLogin, Temp_Mo, CK_DB;
        int Success_Count = 0, Fail_Count = 0;
        public Main_Prescrap(Form parentForm)
        {
            InitializeComponent();
            opener = parentForm;
            Lab_Station.Text = Get_Station.sStation;
            C_GroupLogin = Get_Station.gGroup_name;
            C_LineLogin = Get_Station.lLine;
            C_SectionLogin = Get_Station.sSection_name;
            Lab_Skind.Text = Get_Station.sSkind;
            lblManager.Text = Get_Station.eEmp_Manager;
            EmpNo = Get_Station.eEmp_No;

            Login_line = Get_Station.lLine;
            Login_section = Get_Station.sSection_name;
            Login_group = Get_Station.gGroup_name;
            ConnectDB = Get_Station.cConnectStatus;

        }

        private void ReportError(string Message)
        {
            StackFrame CallStack = new StackFrame(1, true);
            MessageBox.Show("Error: " + Message + ",\n\rFile: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private async void Set_Time() 
        {
            sql = " SELECT TO_CHAR(SYSDATE,'YYYYMMDD') TIME_T FROM DUAL ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            int Date_Time = Int32.Parse(result.Data["time_t"].ToString());
        }

       
        private async Task Qury_Data()
        {
            sql = " SELECT MO_NUMBER, SERIAL_NUMBER, MODEL_NAME, VERSION_CODE, KEY_PART_NO, SCRAP_FLAG, ERROR_FLAG FROM SFISM4.R_WIP_TRACKING_T ";
            sql = sql + " WHERE SERIAL_NUMBER = '"+ Txt_CrapSn.Text + "' ORDER BY MO_NUMBER, MODEL_NAME, VERSION_CODE, SERIAL_NUMBER ";
            var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
            dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
            Grid_View.DataSource = dt;
            Grid_View.AutoResizeColumns();
            Grid_View.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            Gbox_InputQty.Text = "Scrap SN : (" + List_Scrap_Sn.Items.Count + ")  QTY";
        }

        private async Task Clear_Data(int Kind)
        {
            List_Scrap_Sn.Items.Clear();
            Comb_Mo.Text = "";
            Txt_Qty.Text = "";
            Lab_Model.Text = "";
            Lab_KPN.Text = "";
            Lab_Version.Text = "";
            Comb_Reason.Text = "";
            List_DutyStation.Items.Clear();
            List_ReasonCode.Items.Clear();
            List_RCodeType.Items.Clear();
            List_TurnOutGD.Items.Clear();
            List_TurnOutLoc.Items.Clear();
            Txt_CrapSn.Text = "";
            Txt_Desc1.Text = "";
            Txt_Desc2.Text = "";
            Comb_GD.Text = "";
            Comb_GDTurn.Text = "";
            Comb_GD.Enabled = true;
            Txt_DelInput.Text = "";
            CK_Label_del.Checked = false;

            if (Kind == 0)
            {
                Gbox_InputQty.Text = "Scrap SN : (0) QTY";

                Grid_View.DataSource = null;
                Grid_View.Rows.Clear();
                Grid_View.Refresh();

                iTotalScrapQty = 0;
            }

            Grid_View.DataSource = null;
            Grid_View.Rows.Clear();
            Grid_View.Refresh();
        }

        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to close the program ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Btn_Back_Click(object sender, EventArgs e)
        {
            this.Close();
            opener.Show();
        }

        private async void Btn_Clear_Click(object sender, EventArgs e)
        {

            if (dtShow.Rows.Count != 0)
            {
                dtShow.Rows.Clear();
            }

            Lab_Fail.Text = "0";
            Lab_Success.Text = "0";
            Txt_Error.Text = "";

            if ((R1.Checked) || (R3.Checked))
            {
                await Clear_Data(0);
            }
            else
            {
                await Clear_Data(1);
            }
            Gbox_InputQty.Text = "Scrap SN : (" + List_Scrap_Sn.Items.Count + ")  QTY";
        }

        private async void Txt_CrapSn_KeyDown(object sender, KeyEventArgs e) 
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Txt_CrapSn.Text.Length == 0)
                {
                    showMessage("Please Input SN !", "Vui lòng nhập SN !", true);
                    Txt_CrapSn.Focus();
                    Txt_CrapSn.SelectAll();
                    return;
                }

                if (!await Check_Condition())
                {
                    return;
                }

                await Load_SNScrap_Key();
            }
        }

        private async Task Load_SNScrap_Key() 
        {
            string T_Mo = "", C_Error_Flag, Model_Temp;

            if (Ck_DotSn.Checked)
            {
                Txt_CrapSn.Text = "@" + Txt_CrapSn.Text;
            }

            sql = " SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + Txt_CrapSn.Text + "'  ";
            sql = sql + " AND  WIP_GROUP IS NOT NULL  AND GROUP_NAME IS NOT NULL ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                Txt_Error.Text += "" + Txt_CrapSn.Text + ": Not in R107 or WIP - GROUP null\n";
                return;
            }
            else
            {   
                if ((result.Data["scrap_flag"]?.ToString() ?? "") == "2")
                {
                    Txt_Error.Text += "" + Txt_CrapSn.Text + ": Scrap_flag = 2\n";
                    return;
                }

                sql = " SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + Txt_CrapSn.Text + "' AND GROUP_NAME NOT IN ('TEST_PRE_SC', 'SMT_PRE_SC') ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data == null)
                {
                    Txt_Error.Text += "" + Txt_CrapSn.Text + ":WIP_GROUP K hợp lệ\n";
                    return;
                }

                sql = " SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + Txt_CrapSn.Text + "' AND WIP_GROUP like '%HOLD%' ";
                var result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result1.Data != null)
                {
                    Txt_Error.Text += "" + Txt_CrapSn.Text + ":WIP_GROUP = HOLD,Pls UnHold!\n";
                    return;
                }

                C_Error_Flag = result.Data["error_flag"]?.ToString() ?? "";
                T_Mo = result.Data["mo_number"].ToString();
                Model_Temp = result.Data["model_name"].ToString();
            }

            sql = " SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME = '" + Model_Temp + "' ";
            result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                Txt_Error.Text += "" + Model_Temp + ": Chưa thiết lập config6\n";
                return;
            }

            sql = " SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '" + T_Mo + "' ";
            result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                Txt_Error.Text += "" + T_Mo + ": Không tồn tại R105\n";
                return;
            }

            sql = " SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = 'REPAIR' AND VR_CLASS='CHECK_IN' AND VR_ITEM='CHECK_OUT' AND VR_VALUE='YES' ";
            result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)    // B4
            {
                if ((Comb_GD.Text == "BC8M") || (Comb_GD.Text == "BC3F") || (Comb_GD.Text == "BCFA"))
                {
                    if (C_Error_Flag != "7")
                    {
                        if ((C_Error_Flag == "0") || (C_Error_Flag == "1"))
                        {
                            Txt_Error.Text += "" + Txt_CrapSn.Text + ": Phải đưa vào BC2M\n";
                            return;
                        }
                        else if (C_Error_Flag == "8")
                        {
                            Txt_Error.Text += "" + Txt_CrapSn.Text + ": Đã được sửa chữa(Error_Flag=8)\n";
                            Txt_CrapSn.Focus();
                            Txt_CrapSn.SelectAll();
                            return;
                        }
                    }
                }
                else
                {
                    if (Comb_GD.Text != "SCRAP")
                    {
                        if (C_Error_Flag == "7")
                        {
                            Txt_Error.Text += "" + Txt_CrapSn.Text + ": Đẩy bản vào BC8M,BC3F,BCFA\n";
                            Txt_CrapSn.Focus();
                            Txt_CrapSn.SelectAll();
                            return;
                        }

                        if (C_Error_Flag == "8")
                        {
                            Txt_Error.Text += "" + Txt_CrapSn.Text + ": Đã được sửa chữa\n";
                            Txt_CrapSn.Focus();
                            Txt_CrapSn.SelectAll();
                            return;
                        }
                    }

                }
            }
            else  // B5, SFO, ROKU
            {
                if ((Comb_GD.Text == "BC8M") || (Comb_GD.Text == "BC3F") || (Comb_GD.Text == "BCFA"))
                {
                    sql = " SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + Txt_CrapSn.Text + "' AND WIP_GROUP IN ('BC8M', 'BC3F', 'BCFA') ";
                    result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result.Data != null)
                    {
                        Txt_Error.Text += "" + Txt_CrapSn.Text + ": Đã đẩy vào PreScrap\n";
                        Txt_CrapSn.Focus();
                        Txt_CrapSn.SelectAll();
                        return;
                    }

                    if (C_Error_Flag != "1")
                    {
                        Txt_Error.Text += "" + Txt_CrapSn.Text + ": Error_flag <> 1\n";
                        Txt_CrapSn.Focus();
                        Txt_CrapSn.SelectAll();
                        return;
                    }

                }
            }

            if (Menu_Scrap.Checked)    // B4, NIC 
            {
                sql = " SELECT * FROM SFISM4.R_REPAIR_IN_OUT_T WHERE SERIAL_NUMBER = '" + Txt_CrapSn.Text + "' AND  OUT_DATETIME IS NULL ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    Txt_Error.Text += "" + Txt_CrapSn.Text + ": Đã được CHECK IN. Không được di chuyển\n";
                    return;
                }
            }

            if (Ck_TSN.Checked)
            {
                sql = " SELECT * FROM SFISM4.R108  WHERE KEY_PART_SN = '" + Txt_CrapSn.Text + "' ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data == null)
                {
                    Txt_Error.Text += "" + Txt_CrapSn.Text + ": Không tìm thấy in R108\n";
                    Txt_CrapSn.Focus();
                    Txt_CrapSn.SelectAll();
                    return;
                }
                else
                {
                    Txt_SN.Text = Txt_CrapSn.Text;
                    Txt_CrapSn.Text = result.Data["serial_number"].ToString();
                }
            }

            if (List_Scrap_Sn.Items.IndexOf(Txt_CrapSn.Text) != -1)
            {
                Txt_Error.Text += "" + Txt_CrapSn.Text + ": Đã tồn tại trong file\n";
                Txt_CrapSn.Focus();
                Txt_CrapSn.SelectAll();
                return;
            }

            if (!await Check_State(Txt_CrapSn.Text))
            {
                Txt_Error.Text += "" + Txt_CrapSn.Text + ": Tồn tại TRAY/CARTON/PALLET\n";
                return;
            }
            else if (C_Stock_No != "N/A")
            {
                Txt_Error.Text += "" + Txt_CrapSn.Text + ": Đã nhập Kho\n";
                return;
            }
            else if (C_Ship_No != "N/A")
            {
                Txt_Error.Text += "" + Txt_CrapSn.Text + ": Đã xuất hàng\n";
                return;
            }
            else
            {
                List_Scrap_Sn.Items.Add(Txt_CrapSn.Text);

                List_ReasonCode.Items.Add(Comb_Reason.Text);
                List_RCodeType.Items.Add(VC_REASON_TYPE);
                List_DutyStation.Items.Add(VC_DUTY_STATION);

                List_TurnOutGD.Items.Add(Comb_GD.Text);
                List_TurnOutLoc.Items.Add(Comb_LocCost.Text);
            }

            
            if (CK_DB == "CPEII") //For CPEII, SFO
            {
                sql = " SELECT * FROM SFISM4.R_SCRAP_T  WHERE SERIAL_NUMBER ='" + Txt_CrapSn.Text + "' ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    sql = " INSERT INTO SFISM4.H_SCRAP_T (SELECT * FROM SFISM4.R_SCRAP_T  WHERE SERIAL_NUMBER ='" + Txt_CrapSn.Text + "')  ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }
                }
            }
            else
            {
                sql = " UPDATE  SFISM4.R_SCRAP_T SET  SERIAL_NUMBER = SERIAL_NUMBER||MO_NUMBER WHERE SERIAL_NUMBER ='" + Txt_CrapSn.Text + "' AND MO_NUMBER <> '" + T_Mo + "'  ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }

                sql = " UPDATE  SFISM4.H_SCRAP_T SET  SERIAL_NUMBER = SERIAL_NUMBER||MO_NUMBER WHERE SERIAL_NUMBER ='" + Txt_CrapSn.Text + "' AND MO_NUMBER <> '" + T_Mo + "'  ";
                a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }
            }

            Txt_CrapSn.Focus();
            Txt_CrapSn.SelectAll();

            if (List_Scrap_Sn.Items.Count != 0)
            {
                Comb_GD.Enabled = false;
            }

            Gbox_InputQty.Text = "Scrap SN : (" + List_Scrap_Sn.Items.Count + ")  QTY";
        }

        private async void BonePile(string C_SN) 
        {
            string C_Remark = "BonePile-";

            if (Comb_GD.Text == "")
            {
                showMessage("Must choose GD (ex: BC8M/BC2M/...) !", "Phải chọn GD (ex: BC8M/BC2M/...) !", true);
                Comb_GD.Focus();
                return;
            }

            if (Ck_DotSn.Checked)
            {
                C_SN = "@" + C_SN;
            }

            sql = " SELECT COUNT(*) AS TMB  FROM SFISM4.R_REPAIR_T  ";
            sql = sql + " WHERE SERIAL_NUMBER = '" + C_SN + "' AND REPAIR_TIME IS NULL  ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                sql = " SELECT WIP_GROUP FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + C_SN + "' ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                C_Remark = C_Remark + result.Data["wip_group"].ToString();

                sql = "  UPDATE SFISM4.R_REPAIR_T SET DESCRIP = '" + C_Remark + "' WHERE SERIAL_NUMBER = '" + C_SN + "' AND REPAIR_TIME IS NULL ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    Txt_SN.SelectAll();
                    Txt_SN.Focus();
                    return;
                }

                sql = " INSERT INTO SFISM4.h109 ";
                sql = sql + " (SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, TEST_TIME, TEST_CODE, TEST_STATION, TEST_GROUP, TEST_SECTION, TEST_LINE, TESTER, REPAIRER, REPAIR_TIME,  ";
                sql = sql + " REASON_CODE, REPAIR_STATION, REPAIR_GROUP, REPAIR_SECTION, REPAIR_STATUS, DUTY_STATION, DUTY_TYPE, ERROR_ITEM_CODE, RECORD_TYPE, SOLDER_COUNT,  ";
                sql = sql + " DESCRIP, ATE_STATION_NO, T_WORK_SECTION, T_CLASS, T_CLASS_DATE, R_WORK_SECTION, R_CLASS, R_CLASS_DATE, MOVE_FLAG, MEMO)  ";
                sql = sql + " SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, TEST_TIME, TEST_CODE, TEST_STATION, TEST_GROUP, TEST_SECTION, TEST_LINE, TESTER, REPAIRER, REPAIR_TIME,  ";
                sql = sql + " REASON_CODE, REPAIR_STATION, REPAIR_GROUP, REPAIR_SECTION, REPAIR_STATUS, DUTY_STATION, DUTY_TYPE, ERROR_ITEM_CODE, RECORD_TYPE, SOLDER_COUNT,  ";
                sql = sql + " DESCRIP, ATE_STATION_NO, T_WORK_SECTION, T_CLASS, T_CLASS_DATE, R_WORK_SECTION, R_CLASS, R_CLASS_DATE, MOVE_FLAG, MEMO  ";
                sql = sql + " FROM SFISM4.R_REPAIR_T WHERE SERIAL_NUMBER = '" + C_SN + "' AND REPAIR_TIME IS NULL ";
                a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }

                sql = "  DELETE SFISM4.R_REPAIR_T WHERE SERIAL_NUMBER = '" + C_SN + "' AND DESCRIP = '" + C_Remark + "' AND REPAIR_TIME IS NULL  ";
                a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }
            }
        }

        private async void Move_BonePile(string C_SN)
        {
            string C_Remark = "SCRAP-";

            if (Comb_GD.Text == "")
            {
                showMessage("Must choose GD (ex: BC8M/BC2M/...) !", "Phải chọn GD (ex: BC8M/BC2M/...) !", true);
                Comb_GD.Focus();
                return;
            }

            if (Ck_DotSn.Checked)
            {
                C_SN = "@" + C_SN;
            }

            sql = " SELECT COUNT(*) AS NVH  FROM SFISM4.R_REPAIR_T  ";
            sql = sql + " WHERE SERIAL_NUMBER = '" + C_SN + "' AND REPAIR_TIME IS NULL  ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                sql = " SELECT WIP_GROUP FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + C_SN + "' ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                C_Remark = C_Remark + result.Data["wip_group"].ToString();

                sql = "  UPDATE SFISM4.R_REPAIR_T SET DESCRIP = '" + C_Remark + "' WHERE SERIAL_NUMBER = '" + C_SN + "' AND REPAIR_TIME IS NULL ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    Txt_SN.SelectAll();
                    Txt_SN.Focus();
                    return;
                }

                sql = " INSERT INTO SFISM4.h109 ";
                sql = sql + " (SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, TEST_TIME, TEST_CODE, TEST_STATION, TEST_GROUP, TEST_SECTION, TEST_LINE, TESTER, REPAIRER, REPAIR_TIME,  ";
                sql = sql + " REASON_CODE, REPAIR_STATION, REPAIR_GROUP, REPAIR_SECTION, REPAIR_STATUS, DUTY_STATION, DUTY_TYPE, ERROR_ITEM_CODE, RECORD_TYPE, SOLDER_COUNT,  ";
                sql = sql + " DESCRIP, ATE_STATION_NO, T_WORK_SECTION, T_CLASS, T_CLASS_DATE, R_WORK_SECTION, R_CLASS, R_CLASS_DATE, MOVE_FLAG, MEMO)  ";
                sql = sql + " SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, TEST_TIME, TEST_CODE, TEST_STATION, TEST_GROUP, TEST_SECTION, TEST_LINE, TESTER, REPAIRER, REPAIR_TIME,  ";
                sql = sql + " REASON_CODE, REPAIR_STATION, REPAIR_GROUP, REPAIR_SECTION, REPAIR_STATUS, DUTY_STATION, DUTY_TYPE, ERROR_ITEM_CODE, RECORD_TYPE, SOLDER_COUNT,  ";
                sql = sql + " DESCRIP, ATE_STATION_NO, T_WORK_SECTION, T_CLASS, T_CLASS_DATE, R_WORK_SECTION, R_CLASS, R_CLASS_DATE, MOVE_FLAG, MEMO  ";
                sql = sql + " FROM SFISM4.R_REPAIR_T WHERE SERIAL_NUMBER = '" + C_SN + "' AND REPAIR_TIME IS NULL ";
                a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }

                sql = "  DELETE SFISM4.R_REPAIR_T WHERE SERIAL_NUMBER = '" + C_SN + "' AND DESCRIP = '" + C_Remark + "' AND REPAIR_TIME IS NULL  ";
                a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }
            }
        }

        private async void Btn_Confirm_Click(object sender, EventArgs e) 
        {
           
            string C_Nextval, C_Mo, Sn_Scrap, C_ScrapKind = "", C_ITHT;
            String  Temp_Model = "";
            int T_Time, Qty = 0, STT = 0;
            Fail_Count = 0;
            Success_Count = 0;
            Lab_Fail.Text = "0";
            Lab_Success.Text = "0";

            Set_Time();

            string zsql = " SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + List_Scrap_Sn.Items[0].ToString() + "' or shipping_sn = '" + List_Scrap_Sn.Items[0].ToString() + "' or shipping_sn2 = '" + List_Scrap_Sn.Items[0].ToString() + "'";
            var zresult = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = zsql, SfcCommandType = SfcCommandType.Text });
            if (zresult.Data == null)
            {
                showMessage("SN exist in FG, please check", "Du lieu ton tai trong kho xuat hang!", true);
                Txt_CrapSn.Focus();
                Txt_CrapSn.SelectAll();
            }

            sql = " SELECT TO_CHAR(SFISM4.R_STOCK_NO_SEQUENCE.NEXTVAL,'0000000') C_NEXTVAL FROM DUAL ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            C_Nextval = "SFS" + result.Data["c_nextval"].ToString().Trim();

            if (R0.Checked)
            {
                C_ScrapKind = "1";
            }
            else if (R1.Checked)
            {
                C_ScrapKind = "2";
            }
            else if (R2.Checked)
            {
                C_ScrapKind = "5";
            }
            else if (R3.Checked)
            {
                C_ScrapKind = "6";
            }

            sql = " SELECT TO_CHAR(SYSDATE,'YYYYMMDD') Get_Time FROM DUAL  ";
            result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            T_Time = Int32.Parse(result.Data["get_time"].ToString());

            if ((R0.Checked) || (R2.Checked))
            {
                if (List_Scrap_Sn.Items.Count == 0)
                {
                    showMessage("Please input SN !", "Vui lòng nhập SN !", true);
                    Txt_CrapSn.Focus();
                    Txt_CrapSn.SelectAll();
                    return;
                }

                sql = " SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + List_Scrap_Sn.Items[0].ToString() + "'  AND WIP_GROUP IS NOT NULL  AND GROUP_NAME  IS NOT NULL";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    Temp_Mo = result.Data["mo_number"].ToString();
                    Temp_Model = result.Data["model_name"].ToString();
                }

                for (int i = 0; i < List_Scrap_Sn.Items.Count; i++)
                {
                    Gif_Load.Visible = true;

                    Sn_Scrap = List_Scrap_Sn.Items[i].ToString();

                    sql = " SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + Sn_Scrap + "'  AND WIP_GROUP IS NOT NULL  AND GROUP_NAME  IS NOT NULL";
                    result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result.Data == null)
                    {
                        showMessage("SN: " + Sn_Scrap + " not exist in R107 or WIP_GROUP IS NULL!", "SN: " + Sn_Scrap + " không tồn tại trong R107 !", true);
                        Fail_Count = Fail_Count + 1;
                        Lab_Fail.Text = Fail_Count.ToString();
                        continue;
                    }
                    else
                    {
                        C_Mo = result.Data["mo_number"].ToString();
                    }

                    if (!await Check_State(Sn_Scrap))
                    {
                        showMessage("SN: " + Sn_Scrap + " Exist TRAY/CARTON/PALLET in R107 !", "SN: " + Sn_Scrap + "đã tồn tại mã TRAY/CARTON/PALLET !", true);
                        Fail_Count = Fail_Count + 1;
                        Lab_Fail.Text = Fail_Count.ToString();
                        continue;
                    }

                    if (!await Get_R107SN(Sn_Scrap))
                    {
                        Fail_Count = Fail_Count + 1;
                        Lab_Fail.Text = Fail_Count.ToString();
                        continue;
                    }

                    sql = " SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '" + C_Mo + "' ";
                    result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result.Data == null)
                    {
                        showMessage("MO: " + C_Mo + " not exist in R105 !", "MO: " + C_Mo + " không tồn tại trong R105 !", true);
                        Fail_Count = Fail_Count + 1;
                        Lab_Fail.Text = Fail_Count.ToString();
                        continue;
                    }

                    if((R0.Checked) || (R2.Checked))
                    {
                        C_ITHT = "Y";
                    }
                    else
                    {
                        C_ITHT = "N";
                    }

                    sql = " INSERT INTO SFISM4.R_SCRAP_T ( SCRAP_NO, EMP_NO, MO_NUMBER, MODEL_NAME, VERSION_CODE, SERIAL_NUMBER, LINE_NAME, SECTION_NAME, GROUP_NAME, STATION_NAME,  ";
                    sql = sql + " REMARK, KEY_PART_NO, SCRAP_FLAG, SCRAP_KIND, REASON_CODE, REASON_TYPE, DUTY_STATION, QTY, PRE_SCRAP_GROUP_NAME, QC_EMP_DATA, GD, LOC, ITHT ) ";
                    sql = sql + " VALUES ('" + C_Nextval + "', '" + EmpNo + "', '" + C_Mo + "', '" + C_Modelname + "', '" + C_Version + "', '" + Sn_Scrap + "', '" + C_Line + "', '" + C_Section + "', ";
                    sql = sql + " '" + C_Group + "', '" + C_Station + "', '" + TXT_Remark.Text + "', '" + C_KeyPartNo + "', '2', '" + C_ScrapKind + "', '" + List_ReasonCode.Items[i].ToString() + "',  ";
                    sql = sql + " '" + List_RCodeType.Items[i].ToString() + "', '" + List_DutyStation.Items[i].ToString() + "', '1', '" + C_GroupLogin + "', '" + C_WipGroup + "',  ";
                    sql = sql + " '" + List_TurnOutGD.Items[i].ToString() + "', '" + List_TurnOutLoc.Items[i].ToString() + "', '" + C_ITHT + "' ) ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }

                    UpdateMOScrapQty(C_Mo, 1);
                    
                    if (Menu_Scrap.Checked)
                    {
                        sql = " SELECT * FROM SFISM4.R_REPAIR_IN_OUT_T WHERE  SERIAL_NUMBER = '" + Sn_Scrap + "' AND  OUT_DATETIME IS NULL ";
                        result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result.Data != null)
                        {
                            showMessage("SN: " + Sn_Scrap + " has been CHECK IN. Do not move !", "SN: " + Sn_Scrap + " đã được CHECK IN. Không được di chuyển !", true);
                            List_Scrap_Sn.SelectedIndex = List_Scrap_Sn.Items.IndexOf(Sn_Scrap);
                            Txt_CrapSn.Focus();
                            Txt_CrapSn.SelectAll();
                            continue;
                        }

                        Move_BonePile(Sn_Scrap);
                    }
                    else
                    {
                        await UpdateR107SN(Sn_Scrap);
                        await Insert_R117_SN(Sn_Scrap);
                        if(!await Check_In_Out(Sn_Scrap))  //B4
                        {
                            Txt_Error.Text += "" + Sn_Scrap + ": Check IN OUT Error\n"; 
                            Fail_Count = Fail_Count + 1;
                            Lab_Fail.Text = Fail_Count.ToString();
                        }
                    }

                    Success_Count = Success_Count + 1;
                    Lab_Success.Text = Success_Count.ToString();

                    if(C_Mo != Temp_Mo)
                    {
                        STT = STT + 1;

                        sql = " INSERT INTO SFISM4.ITHT ( TID, TTYPE, TRES, TWHS, TLOCT, TPROD, TQTY, TTDTE, TREF, TCOM, TUSER, TSEQ, TSET, TCOMA ) ";
                        sql = sql + " VALUES ( 'TH', 'R', '00', '" + List_TurnOutGD.Items[i].ToString() + "', '" + List_TurnOutLoc.Items[i].ToString() + "', ";
                        sql = sql + " '" + Temp_Model + "', '" + Qty + "', '" + T_Time + "', '"+ Temp_Mo + "', '" + C_Nextval + "', '" + EmpNo + "', '"+ STT + "', 1, '' ) ";
                        a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return;
                        }
                        else
                        {
                            Temp_Mo = C_Mo;
                            Temp_Model = C_Modelname;
                            Qty = 0;
                        }
                    }

                    Deletewl(Sn_Scrap);
                    Qty = Qty + 1;

                    if(Menu_Scrap.Checked)
                    {
                        Insert_PWIP_SN(Sn_Scrap);
                    }
                    else
                    {
                        sql = " SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '"+ Sn_Scrap + "' AND ERROR_FLAG = '7'  ";
                        result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result.Data != null)
                        {
                            sql = " UPDATE SFISM4.R109 SET REPAIRER = 'SYSTEM' WHERE SERIAL_NUMBER = '" + Sn_Scrap + "' AND  REPAIRER IS NULL ";
                            a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return;
                            }
                        }
                        else
                        {
                            BonePile(Sn_Scrap);
                        }
                    }
                }

                if (Lab_Success.Text != "0")
                {
                    if ((R0.Checked) || (R2.Checked))
                    {
                        STT = STT + 1;

                        sql = " INSERT INTO SFISM4.ITHT ( TID, TTYPE, TRES, TWHS, TLOCT, TPROD, TQTY, TTDTE, TREF, TCOM, TUSER, TSEQ, TSET, TCOMA ) ";
                        sql = sql + " VALUES ( 'TH', 'R', '00', '" + List_TurnOutGD.Items[0].ToString() + "', '" + List_TurnOutLoc.Items[0].ToString() + "', ";
                        sql = sql + " '" + C_Modelname + "', '" + Qty + "', '" + T_Time + "', '" + Temp_Mo + "', '" + C_Nextval + "', '" + EmpNo + "', '" + STT + "', 1, '' ) ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return;
                        }
                    }
                }

                Gif_Load.Visible = false;
                MessageBox.Show("Add Scrap NO: " + C_Nextval + " - QTY: " + Lab_Success.Text + "", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await Clear_Data(0);
                Txt_CrapSn.Focus();
                Txt_CrapSn.SelectAll();
            }

            if ((iTotalScrapQty > 0) && ((R1.Checked) || (R3.Checked)))
            {
                if ((R1.Checked) || (R3.Checked))
                {
                    C_ITHT = "Y";
                }
                else
                {
                    C_ITHT = "N";
                }

                foreach (DataRow dtRow in dtShow.Rows)
                {
                    STT = STT + 1;

                    sql = " INSERT INTO SFISM4.R_SCRAP_T ( SCRAP_NO, EMP_NO, MO_NUMBER, MODEL_NAME, VERSION_CODE, SERIAL_NUMBER, LINE_NAME, SECTION_NAME, GROUP_NAME,   ";
                    sql = sql + " STATION_NAME, REMARK, KEY_PART_NO, SCRAP_FLAG, SCRAP_KIND, QTY, REASON_CODE, REASON_TYPE, DUTY_STATION, PRE_SCRAP_GROUP_NAME, GD, LOC, ITHT ) ";
                    sql = sql + " VALUES ('" + C_Nextval + "', '" + EmpNo + "', '" + dtRow["MO_NUMBER"].ToString() + "', '" + dtRow["MODEL_NAME"].ToString() + "', '" + dtRow["VERSION"].ToString() + "', ";
                    sql = sql + " '" + C_Nextval + "'||'NOSN', '" + C_LineLogin + "', '" + C_SectionLogin + "', '" + C_GroupLogin + "', '" + Lab_Station.Text + "', ";
                    sql = sql + " '" + TXT_Remark.Text + "', '" + dtRow["KEY_PART_NO"].ToString() + "', '2', '" + C_ScrapKind + "', '" + dtRow["QTY"].ToString() + "',  ";
                    sql = sql + " '" + List_ReasonCode.Items[0].ToString() + "', '" + List_RCodeType.Items[0].ToString() + "', '" + List_DutyStation.Items[0].ToString() + "', '" + C_GroupLogin + "',  ";
                    sql = sql + " '" + List_TurnOutGD.Items[0].ToString() + "', '" + List_TurnOutLoc.Items[0].ToString() + "', '" + C_ITHT + "' ) ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }

                    sql = " INSERT INTO SFISM4.ITHT ( TID, TTYPE, TRES, TWHS, TLOCT, TPROD, TQTY, TTDTE, TREF, TCOM, TUSER, TSEQ, TSET, TCOMA ) ";
                    sql = sql + " VALUES ( 'TH', 'R', '00', '" + List_TurnOutGD.Items[0].ToString() + "', '" + List_TurnOutLoc.Items[0].ToString() + "', ";
                    sql = sql + " '" + dtRow["MODEL_NAME"].ToString() + "', '" + dtRow["QTY"].ToString() + "', '" + T_Time + "', '" + dtRow["MO_NUMBER"].ToString() + "', '" + C_Nextval + "', '" + EmpNo + "', '" + STT + "', 1, '' ) ";
                    a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }
                }

                MessageBox.Show("Add Scrap NO: " + C_Nextval + " ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await Clear_Data(0);
                if (dtShow.Rows.Count != 0)
                {
                    dtShow.Rows.Clear();
                }
                Comb_Mo.Focus();
                Comb_Mo.SelectAll();
            }

            Comb_GD.Enabled = true;
        }

        private async void Main_Prescrap_Load(object sender, EventArgs e)
        {
            await Get_Reason_Code();
            Txt_Desc1.Text = "";
            Txt_Desc2.Text = "";

            Lab_Fail.Text = "0";
            Lab_Success.Text = "0";
            Txt_Error.Text = "";
            lblVersion.Text = "Version: " + Application.ProductVersion;
            Gif_Load.Visible = false;
        }
        private async Task<Boolean> Compute_Qty(string C_Mo, int C_Qty)
        {
            sql = " SELECT INPUT_QTY, TARGET_QTY, TOTAL_SCRAP_QTY, TURN_OUT_QTY   FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '" + C_Mo + "'";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                int C_Total_scrap = int.Parse(result.Data["total_scrap_qty"]?.ToString() ?? "0");
                int C_Turn_out = int.Parse(result.Data["turn_out_qty"]?.ToString() ?? "0");
                int C_Input_qty = int.Parse(result.Data["input_qty"]?.ToString() ?? "0");
                int C_Ttarget = int.Parse(result.Data["target_qty"]?.ToString() ?? "0");

                if ((C_Qty + C_Total_scrap - C_Turn_out + C_Input_qty) > C_Ttarget)
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

        private async Task Get_Mo()
        {
            sql = " SELECT  MO_NUMBER  FROM SFISM4.R_MO_BASE_T  WHERE CLOSE_FLAG = '2' ORDER BY MO_NUMBER ";
            var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list1.Data != null)
            {
                var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
                dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                Comb_Mo.DataSource = dt.Copy();
                Comb_Mo.DisplayMember = "mo_number";
                Comb_Mo.ValueMember = "mo_number";
            }
        }
        private async void Gbox_Choose_Click()
        {
            if ((R1.Checked) || (R3.Checked))
            {
                Gbox_Mo.Enabled = true;
                await Get_Mo();
                Comb_GDTurn.Text = "";
                Comb_LocTurn.Text = "";

                iTotalScrapQty = 0;
                Gbox_SN.Enabled = false;
                await Clear_Data(0);
                Comb_Reason.Focus();

                Grid_View.DataSource = null;
                Grid_View.Rows.Clear();
                Grid_View.Refresh();
            }
            else
            {
                Gbox_Mo.Enabled = false;
                Gbox_SN.Enabled = true;
                await Clear_Data(1);
                Txt_CrapSn.Focus();
            }
        }

        private async void UpdateMOScrapQty(string C_Mo, int C_Qty)  
        {
            sql = "  UPDATE SFISM4.R_MO_BASE_T SET TOTAL_SCRAP_QTY = TOTAL_SCRAP_QTY +  " + C_Qty + " ";
            if ((R0.Checked) || (R2.Checked))
            {
                sql = sql + " ,TURN_OUT_QTY = TURN_OUT_QTY + " + C_Qty + "  ";
            }
            sql = sql + " WHERE MO_NUMBER = '" + C_Mo + "'  ";
            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }
        }
        private async Task UpdateR107SN(string C_SN)
        {
            string C_Wip = "";

            if (R2.Checked)
            {
                C_Wip = Comb_GD.Text + Comb_LocCost.Text;
            }
            else
            {
                C_Wip = Login_group;
            }

            sql = " UPDATE SFISM4.R_WIP_TRACKING_T  SET SECTION_NAME = '" + Login_section + "', GROUP_NAME = '" + Login_group + "', STATION_NAME = '" + Lab_Station.Text + "',  ";
            sql = sql + " wip_group = '" + C_Wip + "', SCRAP_FLAG ='2' WHERE SERIAL_NUMBER ='" + C_SN + "' ";
            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }
        }

        private async Task<bool> Check_In_Out(string C_SN_Scr) 
        {
            string Temp_Error_Flag = "";

            sql = "  SELECT * FROM SFISM4.R107 WHERE SERIAL_NUMBER = '" + C_SN_Scr + "' AND  WIP_GROUP IN ('BC8M', 'BC3F', 'BCFA')  ";
            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_1.Data != null)
            {
                Temp_Error_Flag = result_1.Data["error_flag"]?.ToString() ?? "";

                if (Temp_Error_Flag == "7")
                {
                    sql = " SELECT * FROM SFISM4.R_REPAIR_IN_OUT_T  WHERE SERIAL_NUMBER = '" + C_SN_Scr + "' AND OUT_DATETIME IS NULL ";
                    var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result.Data != null)
                    {
                        sql = " UPDATE SFISM4.R_REPAIR_IN_OUT_T SET OUT_DATETIME = SYSDATE, P_RECEIVER ='NEW' WHERE SERIAL_NUMBER ='" + C_SN_Scr + "' AND OUT_DATETIME IS NULL ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        Txt_Error.Text += "" + C_SN_Scr + ":OUT_DATETIME IS NOT NULL\n";
                        Fail_Count = Fail_Count + 1;
                        Lab_Fail.Text = Fail_Count.ToString();
                    }
                }
            }
            return true;
        }

        private async void Insert_PWIP_SN(string C_SN) 
        {
            sql = " UPDATE SFISM4.R107 SET IN_STATION_TIME = SYSDATE, EMP_NO = '" + EmpNo + "' WHERE SERIAL_NUMBER = '" + C_SN + "' ";
            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }

            sql = " INSERT INTO SFISM4.P_WIP_TRACKING_T ( SELECT * FROM SFISM4.R107 WHERE SERIAL_NUMBER = '" + C_SN + "') ";
            a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }

            sql = " DELETE SFISM4.R107 WHERE SERIAL_NUMBER = '" + C_SN + "' ";
            a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }
        }

        private async Task Insert_R117_SN(string C_SN) 
        {
            sql = " INSERT INTO SFISM4.R_SN_DETAIL_T(SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, LINE_NAME,  SECTION_NAME, GROUP_NAME, STATION_NAME, LOCATION, STATION_SEQ,  ERROR_FLAG, IN_STATION_TIME, SHIPPING_SN,  ";
            sql = sql + " WORK_FLAG, FINISH_FLAG, ENC_CNT, SPECIAL_ROUTE, PALLET_NO, CONTAINER_NO, QA_NO,QA_RESULT, SCRAP_FLAG, NEXT_STATION, CUSTOMER_NO, BOM_NO, KEY_PART_NO, CARTON_NO, REPAIR_CNT, EMP_NO, PALLET_FULL_FLAG,  ";
            sql = sql + " GROUP_NAME_CQC, MSN, IMEI, JOB, SO_NUMBER, SO_LINE, STOCK_NO, TRAY_NO, SHIP_NO, WIP_GROUP, SHIPPING_SN2)  ";
            sql = sql + " SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, LINE_NAME,  SECTION_NAME, GROUP_NAME, STATION_NAME, LOCATION, STATION_SEQ,  ERROR_FLAG, SYSDATE, SHIPPING_SN, WORK_FLAG, FINISH_FLAG,  ";
            sql = sql + " ENC_CNT, SPECIAL_ROUTE, PALLET_NO, CONTAINER_NO, QA_NO,QA_RESULT, SCRAP_FLAG, NEXT_STATION, CUSTOMER_NO, BOM_NO, KEY_PART_NO, CARTON_NO, REPAIR_CNT, EMP_NO, PALLET_FULL_FLAG,  ";
            sql = sql + " GROUP_NAME_CQC, MSN, IMEI, JOB, SO_NUMBER, SO_LINE, STOCK_NO, TRAY_NO, SHIP_NO, '" + Comb_GD.Text + "', SHIPPING_SN2 FROM SFISM4.R107 WHERE SERIAL_NUMBER = '" + C_SN + "'   ";
            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return;
            }
        }
        private async void Comb_Mo_TextChanged(object sender, EventArgs e) 
        {
            sql = " SELECT MODEL_NAME, VERSION_CODE, KEY_PART_NO FROM  SFISM4.R105 WHERE MO_NUMBER ='" + Comb_Mo.Text + "' ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                Lab_Model.Text = result.Data["model_name"]?.ToString() ?? "";
                Lab_Version.Text = result.Data["version_code"]?.ToString() ?? "";
                Lab_KPN.Text = result.Data["key_part_no"]?.ToString() ?? "";
            }
            else
            {
                Lab_Model.Text = "";
                Lab_Version.Text = "";
                Lab_KPN.Text = "";
            }
        }
        
        private async Task Get_Reason_Code()
        {
            Comb_Reason.Items.Clear();

            sql = "  SELECT DISTINCT  REASON_CODE  FROM SFIS1.C_REASON_CODE_T  ORDER BY REASON_CODE ";
            var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list1.Data.Count() != 0)
            {
                foreach (var dtRow in result_list1.Data)
                {
                    if (!string.IsNullOrEmpty(dtRow["reason_code"]?.ToString() ?? ""))
                    {
                        Comb_Reason.Items.Add(dtRow["reason_code"].ToString());
                    }
                }
            }
        }
        private async void Comb_Reason_TextChanged(object sender, EventArgs e)
        {
            sql = "   SELECT REASON_TYPE,DUTY_STATION,REASON_DESC,REASON_DESC2 FROM SFIS1.C_REASON_CODE_T WHERE REASON_CODE = '" + Comb_Reason.Text.Trim() + "' ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                Txt_Desc1.Text = result.Data["reason_desc"]?.ToString() ?? "";
                Txt_Desc2.Text = result.Data["reason_desc2"]?.ToString() ?? "";
                VC_REASON_TYPE = result.Data["reason_type"]?.ToString() ?? "";
                VC_DUTY_STATION = result.Data["duty_station"]?.ToString() ?? "";
            }
            else
            {
                Txt_Desc1.Text = "";
                Txt_Desc2.Text = "";
                VC_REASON_TYPE = "";
                VC_DUTY_STATION = "";
            }
        }

        private async Task<bool> Get_R107SN(string C_SN)
        {
            sql = "  SELECT * FROM  SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + C_SN + "' AND WIP_GROUP IS NOT NULL  AND GROUP_NAME IS NOT NULL ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                C_Version = result.Data["version_code"]?.ToString() ?? "";
                C_Modelname = result.Data["model_name"]?.ToString() ?? "";
                C_KeyPartNo = result.Data["key_part_no"]?.ToString() ?? "";

                C_Line = result.Data["line_name"]?.ToString() ?? "";
                C_Section = result.Data["section_name"]?.ToString() ?? "";
                C_Group = result.Data["group_name"]?.ToString() ?? "";
                C_Station = result.Data["station_name"]?.ToString() ?? "";
                C_WipGroup = result.Data["wip_group"]?.ToString() ?? "";
            }
            else
            {
                showMessage("" + C_SN + " Not found in R107 or WIP_GROUP IS NULL !", "" + C_SN + " Không tìm thấy trong R107 !", true);
                return false;
            }

            return true;
        }

        private async void Get_GD()
        {
            Comb_GD.Items.Clear();
            Comb_GDTurn.Items.Clear();

            if ((R0.Checked) || (R1.Checked))
            {
                Comb_GD.Items.Add("BD2C").ToString();
                Comb_GDTurn.Items.Add("BD2C").ToString();
                return;
            }

            sql = " SELECT GD FROM SFIS1.C_GD_LOC_T GROUP BY GD ORDER BY GD ";
            var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list.Data.Count() > 0)
            {
                foreach (var row in result_list.Data)
                {
                    Comb_GDTurn.Items.Add(row["gd"]?.ToString() ?? "");
                    Comb_GD.Items.Add(row["gd"]?.ToString() ?? "");
                }
            }
        }

        private async void Get_Loc(string C_GD)
        {
            Comb_LocCost.Items.Clear();
            Comb_LocTurn.Items.Clear();

            if (C_GD.Length != 0)
            {
                sql = " SELECT LOC FROM SFIS1.C_GD_LOC_T  WHERE GD = '" + C_GD + "' GROUP BY LOC  ORDER BY LOC ";
                var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list.Data.Count() > 0)
                {
                    foreach (var row in result_list.Data)
                    {
                        Comb_LocCost.Items.Add(row["loc"]?.ToString() ?? "");
                        Comb_LocTurn.Items.Add(row["loc"]?.ToString() ?? "");
                    }
                }
            }
        }

        private async void Main_Prescrap_Shown(object sender, EventArgs e)
        {
            Gbox_InputQty.Text = "Scrap SN: (0) QTY";
            Lab_connect.Text = ConnectDB;

            if (Lab_Skind.Text == "Scrap")
            {
                Gbox_Skind.Enabled = true;
                Option_SN0.Checked = true;
                R0.Checked = true;

            }
            else if (Lab_Skind.Text == "Turn Out")
            {
                Gbox_Skind.Enabled = true;
                Option_SN0.Checked = true;
                R2.Checked = true;
            }

            Gbox_Choose_Click();
            Get_GD();
            Comb_Mo.Text = "";
            Lab_Model.Text = "";
            Lab_Version.Text = "";
            Lab_KPN.Text = "";

            sql = "SELECT* FROM SFIS1.C_PARAMETER_INI where PRG_NAME = 'PRESCRAP' AND VR_NAME IN('CPEII', 'SFO') AND VR_VALUE = 'TRUE' ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                CK_DB = "CPEII";
            }
        }


        private void Option_SN0_CheckedChanged(object sender, EventArgs e)
        {
            if (Option_SN0.Checked == true)
            {
                if (Lab_Skind.Text == "Scrap")
                {
                    R0.Checked = true;
                }
                else if (Lab_Skind.Text == "Turn Out")
                {
                    R2.Checked = true;
                }
            }
            else
            {
                if (Lab_Skind.Text == "Scrap")
                {
                    R1.Checked = true;
                }
                else if (Lab_Skind.Text == "Turn Out")
                {
                    R3.Checked = true;
                }
            }

            Gbox_Choose_Click();
        }

        private void Menu_DelInput_Click(object sender, EventArgs e) 
        {
            if (Menu_DelInput.Checked)
            {
                Txt_DelInput.Visible = true;
                label_sn.Visible = false;
                Txt_CrapSn.Visible = false;
                Ck_DotSn.Visible = false;
                Txt_DelInput.Text = "";
                Txt_DelInput.Focus();
            }
            else
            {
                Txt_DelInput.Visible = false;
                label_sn.Visible = true;
                Txt_CrapSn.Visible = true;
                Ck_DotSn.Visible = true;
                Txt_DelInput.Text = "";
                Txt_CrapSn.Focus();
            }
        }

       

        private void Menu_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void Txt_DelInput_KeyDown(object sender, KeyEventArgs e)  
        {
            String C_Del_Reason, C_Del_RCode, C_Del_Duty, C_Del_TurGD, C_Del_TurLOC;
            int K;

            if (e.KeyCode == Keys.Enter)
            {
                if ((R0.Checked) || (R2.Checked))
                {
                    if (List_Scrap_Sn.Items.IndexOf(Txt_DelInput.Text) == -1)
                    {
                        showMessage(""+ Txt_DelInput.Text + " None Serial_Number in List SN !", "" + Txt_DelInput.Text + " Không có Serial_Number trong danh sách SN !", true);
                        Txt_DelInput.SelectAll();
                        Txt_DelInput.Focus();
                        return;
                    }
                    else
                    {
                        K = List_Scrap_Sn.Items.IndexOf(Txt_DelInput.Text);

                        C_Del_Reason = List_ReasonCode.Items[K].ToString();
                        List_ReasonCode.Items.Remove(C_Del_Reason);

                        C_Del_RCode = List_RCodeType.Items[K].ToString();
                        List_RCodeType.Items.Remove(C_Del_RCode);

                        C_Del_Duty = List_DutyStation.Items[K].ToString();
                        List_DutyStation.Items.Remove(C_Del_Duty);

                        C_Del_TurGD = List_TurnOutGD.Items[K].ToString();
                        List_TurnOutGD.Items.Remove(C_Del_TurGD);

                        C_Del_TurLOC = List_TurnOutLoc.Items[K].ToString();
                        List_TurnOutLoc.Items.Remove(C_Del_TurLOC);

                        List_Scrap_Sn.Items.Remove(Txt_DelInput.Text);
                        await Qury_Data();
                    }
                }
                else
                {
                    showMessage("The have not delete data !", "Không xóa dữ liệu !", true);
                    return;
                }
                Txt_DelInput.SelectAll();
                Txt_DelInput.Focus();
            }
        }

        private void CK_Label_del_CheckedChanged(object sender, EventArgs e)
        {
            if (CK_Label_del.Checked)
            {
                Txt_DelInput.Visible = true;
                label_sn.Visible = false;
                Txt_CrapSn.Visible = false;
                Ck_DotSn.Visible = false;
                Txt_DelInput.Text = "";
                Txt_DelInput.Focus();
            }
            else
            {
                Txt_DelInput.Visible = false;
                label_sn.Visible = true;
                Txt_CrapSn.Visible = true;
                Ck_DotSn.Visible = true;
                Txt_DelInput.Text = "";
                Txt_CrapSn.Focus();
            }
        }

        private void Grid_View_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Txt_CrapSn_TextChanged(object sender, EventArgs e)
        {

        }

        private async void Deletewl(string C_SN) 
        {
            sql = " SELECT MODEL_TYPE FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME IN ";
            sql = sql + " (SELECT MODEL_NAME FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + C_SN + "') ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                showMessage("Not setup Config 6 or SN not Exist in R107 !", "Chưa thiết lập Config 6 hoặc SN không tồn tại trong R107 !", true);
                return;
            }
            else
            {
                if ((result.Data["model_type"]?.ToString()?? "").IndexOf("048") != -1)
                {
                    sql = "  INSERT INTO SFISM4.R_WIP_KEYPARTS_UNDO_T SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER = '" + C_SN + "'  ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }

                    sql = "  INSERT INTO SFISM4.R_CUSTSN_T_BAK SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER = '" + C_SN + "'  ";
                    a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }

                    sql = "  DELETE SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER = '" + C_SN + "'  ";
                    a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                    sql = "  DELETE SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER = '" + C_SN + "'  ";
                    a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                    sql = "  INSERT INTO SFISM4.R_SYSTEM_LOG_T (EMP_NO,PRG_NAME,ACTION_TYPE,ACTION_DESC,TIME) ";
                    sql = sql + " VALUES('" + EmpNo + "', 'SCRAP_REWORK', 'DELETE R108 R_CUSTSN_T', '" + C_SN + "',SYSDATE) ";
                    a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }
                }
            }
        }

        private void Mn_Out_Click(object sender, EventArgs e)
        {
            GD = "OUT";
            Get_Station.gGD = GD;
            Form_Query form_Query = new Form_Query(this);
            form_Query.Show();
            this.Hide();
        }
        private void Mn_BC8M_Click(object sender, EventArgs e)
        {
            GD = "BC8M";
            Get_Station.gGD = GD;
            Form_Query form_Query = new Form_Query(this);
            form_Query.Show();
            this.Hide();
        }
        private void Mn_BC9M_Click(object sender, EventArgs e)
        {
            GD = "BC9M";
            Get_Station.gGD = GD;
            Form_Query form_Query = new Form_Query(this);
            form_Query.Show();
            this.Hide();
        }
        private void Mn_BC2M_Click(object sender, EventArgs e)
        {
            GD = "BC2M";
            Get_Station.gGD = GD;
            Form_Query form_Query = new Form_Query(this);
            form_Query.Show();
            this.Hide();
        }

        private void Comb_GDTurn_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Comb_LocTurn.Text = "";
            Get_Loc(Comb_GDTurn.SelectedItem.ToString());
        }

        private async void Txt_Qty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)

            {
                if (await Check_Data())
                {
                    Create_Table(Comb_Mo.Text, Lab_Model.Text, Lab_Version.Text, Lab_KPN.Text, Txt_Qty.Text, Comb_Reason.Text, Comb_GDTurn.Text, Comb_LocTurn.Text);
                }   
            }
        }

        private void Create_Table(String C_Mo, String C_Model, String C_Ver, String C_KPS, String C_Qty, String C_Reason, String C_GD, String C_LOC)
        {
            if (dtShow.Columns.Count == 0)
            {
                dtShow.Columns.Add("MO_NUMBER");
                dtShow.Columns.Add("MODEL_NAME");
                dtShow.Columns.Add("VERSION");
                dtShow.Columns.Add("KEY_PART_NO");
                dtShow.Columns.Add("QTY");
                dtShow.Columns.Add("REASON_CODE");
                dtShow.Columns.Add("GD");
                dtShow.Columns.Add("LOC");
            }
            else
            {
                foreach (DataRow dtRow in dtShow.Rows)
                {
                    if (Comb_Mo.Text == dtRow["MO_NUMBER"].ToString())
                    {
                        showMessage("Duplicate MO: " + Comb_Mo.Text, "Trùng lặp MO: " + Comb_Mo.Text, true);
                        Comb_Mo.Focus();
                        Comb_Mo.SelectAll();
                        return;
                    }
                }
            }
            dtShow.Rows.Add(C_Mo, C_Model, C_Ver, C_KPS, C_Qty, C_Reason, C_GD, C_LOC);
            Grid_View.DataSource = dtShow;

            List_ReasonCode.Items.Add(Comb_Reason.Text);
            List_RCodeType.Items.Add(VC_REASON_TYPE);
            List_DutyStation.Items.Add(VC_DUTY_STATION);
            List_TurnOutGD.Items.Add(Comb_GDTurn.Text);
            List_TurnOutLoc.Items.Add(Comb_LocTurn.Text);

            iTotalScrapQty = iTotalScrapQty + Int32.Parse(Txt_Qty.Text);
            Gbox_InputQty.Text = "Scrap SN : (" + iTotalScrapQty + ")  QTY";
        }

        private async Task<bool> Check_Data()
        {
            if (Comb_Mo.Text.Length == 0)
            {
                showMessage("Please input MO !", "Vui lòng nhập MO !", true);
                Comb_Mo.Focus();
                Comb_Mo.SelectAll();
                return false;
            }

            if (Txt_Qty.Text.Length == 0)
            {
                showMessage("Please input Qty !", "Vui lòng nhập Qty !", true);
                Txt_Qty.Focus();
                Txt_Qty.SelectAll();
                return false;
            }

            if (!await CHECK_CHARACTER_0_9(Txt_Qty.Text))
            {
                showMessage("" + Txt_Qty.Text + " - Invalid characters(0_9 A_Z) !", "" + Txt_Qty.Text + " - Ký tự không hợp lệ (0_9 A_Z) !", true);
                Txt_Qty.Focus();
                Txt_Qty.SelectAll();
                return false;
            }

            if (!await CHECK_CHARACTER_0_9(Comb_Mo.Text))
            {
                showMessage("" + Comb_Mo.Text + " - Invalid characters(0_9 A_Z) !", "" + Comb_Mo.Text + " - Ký tự không hợp lệ (0_9 A_Z) !", true);
                Comb_Mo.Focus();
                Comb_Mo.SelectAll();
                return false;
            }

            sql = "  SELECT LOC FROM SFIS1.C_GD_LOC_T  WHERE GD = '" + Comb_GDTurn.Text + "' ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                showMessage("Illegal GD !", "GD không hợp lệ. GD không nằm trong list !", true);
                Comb_GDTurn.Focus();
                Comb_GDTurn.SelectAll();
                return false;
            }

            if(!await Compute_Qty(Comb_Mo.Text, Int32.Parse(Txt_Qty.Text)))
            {
                showMessage("The Scrap_Qty + Input_Qty > Target Qty !", "Txt_Qty: " + Txt_Qty.Text + " + TOTAL_SCRAP_QTY - TURN_OUT_QTY + INPUT_QTY > TARGET_QTY !", true);
                Comb_Mo.Focus();
                Comb_Mo.SelectAll();
                return false;
            }

            sql = "  SELECT REASON_TYPE,DUTY_STATION,REASON_DESC,REASON_DESC2 FROM SFIS1.C_REASON_CODE_T WHERE REASON_CODE = '" + Comb_Reason.Text + "' ";
            result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                showMessage("Illegal REASON_CODE !", "REASON_CODE không hợp lệ. REASON_CODE không nằm trong list !", true);
                Comb_Reason.Focus();
                Comb_Reason.SelectAll();
                return false;
            }
            return true;
        }

        private void Mn_BC9F_Click(object sender, EventArgs e)
        {
            GD = "BC9F";
            Get_Station.gGD = GD;
            Form_Query form_Query = new Form_Query(this);
            form_Query.Show();
            this.Hide();
        }
        private void Mn_BC3F_Click(object sender, EventArgs e)
        {
            GD = "BC3F";
            Get_Station.gGD = GD;
            Form_Query form_Query = new Form_Query(this);
            form_Query.Show();
            this.Hide();
        }

        private async Task<bool> Check_Condition()
        {
            if ((R0.Checked) || (R1.Checked))
            {
                if (Comb_GD.Text != "BD2C")
                {
                    showMessage("Illegal GD !", "GD không hợp lệ. GD không nằm trong list !", true);
                    Comb_GD.Focus();
                    Comb_GD.SelectAll();
                    return false;
                }
            }
            else
            {
                sql = "  SELECT LOC FROM SFIS1.C_GD_LOC_T  WHERE GD = '" + Comb_GD.Text + "' ";
                var result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result1.Data == null)
                {
                    showMessage("Illegal GD !", "GD không hợp lệ. GD không nằm trong list !", true);
                    Comb_GD.Focus();
                    Comb_GD.SelectAll();
                    return false;
                }
            }

            sql = "  SELECT REASON_TYPE,DUTY_STATION,REASON_DESC,REASON_DESC2 FROM SFIS1.C_REASON_CODE_T WHERE REASON_CODE = '" + Comb_Reason.Text + "' ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                showMessage("Illegal REASON_CODE !", "REASON_CODE không hợp lệ. REASON_CODE không nằm trong list !", true);
                Comb_Reason.Focus();
                Comb_Reason.SelectAll();
                return false ;
            }

            return true;
        }


        private async void Btn_ImportEX_Click(object sender, EventArgs e)
        {
            string path = "";
            string C_SN;
           
            if(!await Check_Condition())
            {
                return;
            }

            List<string> listSheet = new List<string>();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel File|*.xlsx";
            dlg.Multiselect = false;

            DialogResult dlgResult = dlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                path = dlg.FileName;
                if (path.Equals(""))
                {
                    showMessage("Please choose file Excel !", "Vui lòng chọn file Excel!", true);
                    return;
                }
                if (!File.Exists(path))
                {
                    showMessage("Can not open file or file not exist !", "Không thể mở File hoặc File không tồn tại !", true);
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
                    for (int i = 0; i < countRow; i++)
                    {
                        Gif_Load.Visible = true;

                        C_SN = dt.Rows[i][0].ToString();  
                        if (!string.IsNullOrEmpty(C_SN))
                        {
                            Txt_CrapSn.Text = C_SN;
                            await Load_SNScrap_Key();
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

        private async void Btn_InputText_Click(object sender, EventArgs e)
        {
            if (!await Check_Condition())
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Text Files (.txt*)|*.txt*";
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
                    Gif_Load.Visible = true;

                    foreach (string C_SN in dataSN)
                    {
                        if(!string.IsNullOrEmpty(C_SN))
                        {
                            Txt_CrapSn.Text = C_SN;
                            await Load_SNScrap_Key();
                        }    
                    }
                    await Qury_Total();
                    Gif_Load.Visible = false;
                }
            }
        }

        private async Task Qury_Total()
        {
            sql = " SELECT MO_NUMBER, SERIAL_NUMBER, MODEL_NAME, VERSION_CODE, KEY_PART_NO, SCRAP_FLAG, ERROR_FLAG FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER IN ( '' ";
            for (int i = 0; i < List_Scrap_Sn.Items.Count; i++)
            {
                sql = sql + ", '" + List_Scrap_Sn.Items[i].ToString() + "'";
            }
            sql = sql + " ) ORDER BY MO_NUMBER, MODEL_NAME, VERSION_CODE, SERIAL_NUMBER ";
            var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
            dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
            Grid_View.DataSource = dt;
            Grid_View.AutoResizeColumns();
            Grid_View.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            Gbox_InputQty.Text = "Scrap SN : (" + List_Scrap_Sn.Items.Count + ")  QTY";
        }

        private async void Menu_Scrap_Click(object sender, EventArgs e)
        {
            if(Menu_Scrap.Checked)
            {
                sql = "  SELECT * FROM SFIS1.C_PRIVILEGE WHERE PRG_NAME ='PRESCRAP' AND FUN='SCRAP'  AND EMP = '" + EmpNo + "'  ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data == null)
                {
                    Menu_Scrap.Checked = false;
                    showMessage("" + EmpNo + " not permisson !\nPRG_NAME = PRESCRAP AND FUN = SCRAP", "Mã thẻ không có quyền !", true);
                    return;
                }

                Menu_Scrap.Checked = true;
            }
            else
            {
                Menu_Scrap.Checked = false;
            }
        }

        private void Comb_GD_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Comb_LocCost.Text = "";
            Get_Loc(Comb_GD.SelectedItem.ToString());
        }

        private void Menu_Modify_Click(object sender, EventArgs e)
        {
            PreS_Modify preS_Modify = new PreS_Modify(this);
            preS_Modify.Show();
            this.Hide();
        }

        private void Menu_About_Click(object sender, EventArgs e)
        {
            Info_AB info_About = new Info_AB();
            info_About.ShowDialog();
        }

        private async Task<bool> Check_State(String Sn_Check)
        {
            string C_Tray, C_Carton, C_Mcarton, C_pallet, C_Imei;

            sql = " SELECT TRAY_NO, CARTON_NO, MCARTON_NO, PALLET_NO, IMEI, STOCK_NO, SHIP_NO FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + Sn_Check + "' ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                C_Tray = result.Data["tray_no"]?.ToString().Trim() ?? "";
                C_Carton = result.Data["carton_no"]?.ToString().Trim() ?? "";
                C_Mcarton = result.Data["mcarton_no"]?.ToString().Trim() ?? "";
                C_pallet = result.Data["pallet_no"]?.ToString().Trim() ?? "";
                C_Imei = result.Data["imei"]?.ToString().Trim() ?? "";
                C_Stock_No = result.Data["stock_no"]?.ToString().Trim() ?? "N/A";
                C_Ship_No = result.Data["ship_no"]?.ToString().Trim() ?? "N/A";

                if ((C_Tray.Length < 4) && 
                    (C_Carton.Length < 4) &&
                    (C_Mcarton.Length < 4) &&
                    (C_pallet.Length < 4) &&
                    (C_Imei.Length < 4))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> CHECK_CHARACTER_0_9(string TMPSTR)
        {
            TMPSTR = TMPSTR.Trim();

            var regexItem = new Regex("^[0-9/-]*$");
            if (!regexItem.IsMatch(TMPSTR))
            {
                showMessage("" + TMPSTR + " - Invalid characters(0_9) !", "" + TMPSTR + " - Ký tự không hợp lệ (0_9) !", true);
                return false;
            }
            return true;
        }


    }
}
