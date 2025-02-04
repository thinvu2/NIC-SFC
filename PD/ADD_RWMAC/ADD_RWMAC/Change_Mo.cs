using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using SFC_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADD_RWMAC
{
    public partial class Change_Mo : Form
    {
        public SfcHttpClient _sfcHttpClient;
        public string sql, empNo, PLANT_CODE_SN1, PLANT_CODE_SN2;
        MessageError FrmMessage = new MessageError();
        
        public Change_Mo(SfcHttpClient sfcHttpClient)
        {
            InitializeComponent();
            _sfcHttpClient = sfcHttpClient;
        }

        private void ReportError(string Message)
        {
            StackFrame CallStack = new StackFrame(1, true);
            MessageBox.Show("Error: " + Message + ",\n\rFile: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            string FCC1, FCC2;

            if  (Edit_Mo_From.Text.Length == 0)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("Input MO ! ");
                FrmMessage.MessageVietNam = string.Format("Nhập MO !");
                FrmMessage.ShowDialog();
                Edit_Mo_From.SelectAll();
                Edit_Mo_From.Focus();
                return;
            }

            if (Edit_Mo_To.Text.Length == 0)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("Input MO ! ");
                FrmMessage.MessageVietNam = string.Format("Nhập MO !");
                FrmMessage.ShowDialog();
                Edit_Mo_To.SelectAll();
                Edit_Mo_To.Focus();
                return;
            }

            sql = " SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER = '" + Edit_Mo_From.Text + "'";
            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_1.Data == null)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("MO not found (R_BPCS_MOPLAN_T) ! ");
                FrmMessage.MessageVietNam = string.Format("MO không tìm thấy !");
                FrmMessage.ShowDialog();
                Edit_Mo_From.SelectAll();
                Edit_Mo_From.Focus();
                return;
            }
            else
            {
                PLANT_CODE_SN1 = result_1.Data["site"].ToString();
                Model_From.Text = result_1.Data["model_name"].ToString();
                Edit_Mo_To.SelectAll();
                Edit_Mo_To.Focus();
            }


            sql = " SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER = '" + Edit_Mo_To.Text + "'";
            var result_4 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_4.Data == null)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("MO not found (R_BPCS_MOPLAN_T) ! ");
                FrmMessage.MessageVietNam = string.Format("MO không tìm thấy !");
                FrmMessage.ShowDialog();
                Edit_Mo_To.SelectAll();
                Edit_Mo_To.Focus();
                return;
            }
            else
            {
                PLANT_CODE_SN2 = result_1.Data["site"].ToString();
                Model_To.Text = result_4.Data["model_name"].ToString();
            }

            if (Model_From.Text != Model_To.Text)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("MODEL name different cannot merger MO !\n\n " + Model_From.Text + " - " + Model_To.Text + "");
                FrmMessage.MessageVietNam = string.Format("Tên MODEL khác nhau không thể hợp nhất MO !");
                FrmMessage.ShowDialog();
                Edit_Mo_From.SelectAll();
                Edit_Mo_From.Focus();
                return;
            }

            sql = " SELECT DISTINCT SUBSTR(SHIPPING_SN,1,4) AS PREFIX1 FROM SFISM4.R_NETG_PRIN_ALL_T WHERE MO_NUMBER = '" + Edit_Mo_From.Text + "'";
            var result_5 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_5.Data == null)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("MO_FROM not found (R_NETG_PRIN_ALL_T) ! ");
                FrmMessage.MessageVietNam = string.Format("MO không tìm thấy trong bảng R_NETG_PRIN_ALL_T !");
                FrmMessage.ShowDialog();
                Edit_Mo_From.SelectAll();
                Edit_Mo_From.Focus();
                return;
            }
            else
            {
                FCC1 = result_5.Data["prefix1"].ToString();

                sql = " SELECT DISTINCT SUBSTR(SHIPPING_SN,1,4) AS PREFIX2 FROM SFISM4.R_NETG_PRIN_ALL_T WHERE MO_NUMBER = '" + Edit_Mo_To.Text + "'";
                var result_6 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_6.Data == null)
                {
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("MO_TO not found (R_NETG_PRIN_ALL_T) ! ");
                    FrmMessage.MessageVietNam = string.Format("MO không tìm thấy trong bảng R_NETG_PRIN_ALL_T !");
                    FrmMessage.ShowDialog();
                    Edit_Mo_To.SelectAll();
                    Edit_Mo_To.Focus();
                    return;
                }
                else
                {
                    FCC2 = result_6.Data["prefix2"].ToString();
                }
            }

            if (PLANT_CODE_SN1 != PLANT_CODE_SN2)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("PLANT_CODE: "+ PLANT_CODE_SN1 + " <> " + PLANT_CODE_SN2 + ". NOT CHANGE ");
                FrmMessage.MessageVietNam = string.Format("Khác mã xưởng. Không thể LINK ");
                FrmMessage.ShowDialog();
                Edit_Mo_From.SelectAll();
                Edit_Mo_From.Focus();
                return;
            }

            if (FCC1 != FCC2)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("Prefix FCC 2 MO different: "+ FCC1 + " <> "+ FCC2+ " ");
                FrmMessage.MessageVietNam = string.Format("Dải FCC label 2 MO khác nhau !");
                FrmMessage.ShowDialog();
                Edit_Mo_From.SelectAll();
                Edit_Mo_From.Focus();
                return;
            }
            else
            {
                sql = " SELECT DISTINCT '"+Edit_Mo_To.Text+"' MO_NUMBER,'' SN ,A.MACID MAC ,A.SHIPPING_SN SSN,A.MODEL_NAME TYPE, ";
                sql = sql + " SYSDATE IN_STATION_TIME,'"+ empNo + "' EMP_NO,A.MACID1 MAC2,'N/A' MAC3,'' MAC4,A.PIN_CODE MAC5,'' SSN2,'N/A' SSN3,SSID SSID,TEMP4 SSN5 ";
                sql = sql + " FROM SFISM4.R_NETG_PRIN_ALL_T A WHERE A.MO_NUMBER IN ('" + Edit_Mo_From.Text + "') ";
                sql = sql + " AND A.MACID NOT IN (SELECT B.SHIPPING_SN2 FROM SFISM4.R_WIP_TRACKING_T B  WHERE A.MACID =B.SHIPPING_SN2) ";
                sql = sql + " AND A.MACID NOT IN (SELECT B.MAC FROM SFISM4.R_RW_MAC_SSN_T B WHERE A.MACID =B.MAC) ";
                var result_list1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list1.Data != null )
                {

                    sql = " INSERT INTO SFISM4.R_RW_MAC_SSN_T " ;
                    sql = sql + " SELECT DISTINCT '" + Edit_Mo_To.Text + "' MO_NUMBER,'' SN ,A.MACID MAC ,A.SHIPPING_SN SSN,A.MODEL_NAME TYPE, ";
                    sql = sql + " SYSDATE IN_STATION_TIME,'" + empNo + "' EMP_NO,A.MACID1 MAC2,'N/A' MAC3,'' MAC4,A.PIN_CODE MAC5,'' SSN2,'N/A' SSN3,SSID SSID,TEMP4 SSN5 ";
                    sql = sql + " FROM SFISM4.R_NETG_PRIN_ALL_T A WHERE A.MO_NUMBER IN ('" + Edit_Mo_From.Text + "') ";
                    sql = sql + " AND A.MACID NOT IN (SELECT B.SHIPPING_SN2 FROM SFISM4.R_WIP_TRACKING_T B  WHERE A.MACID =B.SHIPPING_SN2) ";
                    sql = sql + " AND A.MACID NOT IN (SELECT B.MAC FROM SFISM4.R_RW_MAC_SSN_T B WHERE A.MACID =B.MAC )";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        return;
                    }
                    else
                    {
                        sql = "INSERT INTO SFISM4.R_SYSTEM_LOG_T(EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC, TIME) ";
                        sql = sql + "VALUES ('" + empNo + "', 'ADDRWMAC', 'CHANGE MO', 'MOVE FCC: " + Edit_Mo_From.Text + " - TO MO: " + Edit_Mo_To.Text + " ', SYSDATE)";
                        var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        if (b.Result != "OK")
                        {
                            ReportError(b.Message.ToString());
                            return;
                        }
                        else
                        {
                            FrmMessage.CustomFlag = true;
                            FrmMessage.MessageEnglish = string.Format("Thay đổi MO thành công ! ");
                            FrmMessage.MessageVietNam = string.Format("Change MO successfully !");
                            FrmMessage.ShowDialog();
                            Edit_Mo_From.SelectAll();
                            Edit_Mo_From.Focus();
                            return;
                        }
                    }

                }
                else
                {
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("No data to transfer (R_NETG_PRIN_ALL_T)! ");
                    FrmMessage.MessageVietNam = string.Format("Không có dữ liệu để chuyển !");
                    FrmMessage.ShowDialog();
                    Edit_Mo_From.SelectAll();
                    Edit_Mo_From.Focus();
                    return;
                }
            }
        }

        private async void Edit_Mo_From_KeyDown(object sender, KeyEventArgs e)
        {
            Model_From.Text = "";

            if ((e.KeyCode == Keys.Enter) && (Edit_Mo_From.Text.Length != 0))
            {
                sql = " SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER = '" + Edit_Mo_From.Text + "'";
                var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_1.Data == null)
                {
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("MO not found (R_BPCS_MOPLAN_T) ! ");
                    FrmMessage.MessageVietNam = string.Format("MO không tìm thấy !");
                    FrmMessage.ShowDialog();
                    Edit_Mo_From.SelectAll();
                    Edit_Mo_From.Focus();
                    return;
                }
                else
                {
                    PLANT_CODE_SN1 = result_1.Data["site"].ToString();
                    Model_From.Text = result_1.Data["model_name"].ToString();
                    Edit_Mo_To.SelectAll();
                    Edit_Mo_To.Focus();
                }
            }
        }

        private async  void Edit_Mo_To_KeyDown(object sender, KeyEventArgs e)
        {
            Model_To.Text = "";

            if ((e.KeyCode == Keys.Enter) && (Edit_Mo_To.Text.Length != 0))
            {
                sql = " SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER = '" + Edit_Mo_To.Text + "'";
                var result_2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_2.Data == null)
                {
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("MO not found (R_BPCS_MOPLAN_T) ! ");
                    FrmMessage.MessageVietNam = string.Format("MO không tìm thấy !");
                    FrmMessage.ShowDialog();
                    Edit_Mo_To.SelectAll();
                    Edit_Mo_To.Focus();
                    return;
                }
                else
                {
                    PLANT_CODE_SN2 = result_2.Data["site"].ToString();
                    Model_To.Text = result_2.Data["model_name"].ToString();

                    if (Model_From.Text != Model_To.Text)
                    {
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = string.Format("MODEL name different cannot merger MO !\n\n " + Model_From.Text + " - " + Model_To.Text + "");
                        FrmMessage.MessageVietNam = string.Format("Tên MODEL khác nhau không thể hợp nhất MO !");
                        FrmMessage.ShowDialog();
                        Edit_Mo_From.SelectAll();
                        Edit_Mo_From.Focus();
                        return;
                    }     
                }
            }
        }
    }
}
