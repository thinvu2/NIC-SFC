using Newtonsoft.Json;
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
    public partial class Move_Range : Form
    {
        DataTable dt;
        public string sql, empNo;
        public SfcHttpClient _sfcHttpClient;


        MessageError FrmMessage = new MessageError();

        public Move_Range(SfcHttpClient sfcHttpClient)
        {
            InitializeComponent();
            _sfcHttpClient = sfcHttpClient;
        }

        private void ReportError(string Message)
        {
            StackFrame CallStack = new StackFrame(1, true);
            MessageBox.Show("Error: " + Message + ",\n\rFile: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void Edit_SN_Begin_KeyDown(object sender, KeyEventArgs e)
        {
            if ( (e.KeyCode == Keys.Enter) && (Edit_SN_Begin.Text.Length != 0) ) 
            {
                int a = Edit_SN_Begin.Text.IndexOf("A");


                sql =  " SELECT * FROM SFISM4.R_MO_EXT_T WHERE '"+ Edit_SN_Begin.Text + "' <= ITEM_2 AND '" + Edit_SN_Begin.Text + "' >= ITEM_1  ";
                sql = sql + " AND LENGTH( '" + Edit_SN_Begin.Text + "' )=LENGTH(ITEM_1) AND LENGTH( '" + Edit_SN_Begin.Text + "' ) = LENGTH(ITEM_2) ";
                var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_1.Data == null)
                {
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("SN begin range not found!");
                    FrmMessage.MessageVietNam = string.Format("Đầu dải SN không tìm thấy!");
                    FrmMessage.ShowDialog();
                    Edit_SN_Begin.SelectAll();
                    Edit_SN_Begin.Focus();
                    return;
                }
                else
                {
                    Edit_SN_End.SelectAll();
                    Edit_SN_End.Focus();
                }
            }
        }


        private async void Edit_SN_End_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (Edit_SN_Begin.Text.Length != 0))
            {
                sql = " SELECT * FROM SFISM4.R_MO_EXT_T WHERE '" + Edit_SN_End.Text + "' <= ITEM_2 AND '" + Edit_SN_End.Text + "' >=ITEM_1  ";
                sql = sql + " AND LENGTH( '" + Edit_SN_End.Text + "' )=LENGTH(ITEM_1) AND LENGTH( '" + Edit_SN_End.Text + "' )=LENGTH(ITEM_2) ";
                var result_2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_2.Data == null)
                {
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("SN end range not found!");
                    FrmMessage.MessageVietNam = string.Format("Dải cuối SN không tìm thấy!");
                    FrmMessage.ShowDialog();
                    Edit_SN_End.SelectAll();
                    Edit_SN_End.Focus();
                    return;
                }
                else
                {
                    Edit_Mo.SelectAll();
                    Edit_Mo.Focus();
                }
            }
        }

        private async void Edit_Mo_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (Edit_Mo.Text.Length != 0))
            {
                sql = " SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER = '"+Edit_Mo.Text+"'   ";
                var result_3 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_3.Data == null)
                {
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("MO: " + Edit_Mo.Text + " not found (R_BPCS_MOPLAN_T)!");
                    FrmMessage.MessageVietNam = string.Format("MO: " + Edit_Mo.Text + " không tìm thấy !");
                    FrmMessage.ShowDialog();
                    Edit_Mo.SelectAll();
                    Edit_Mo.Focus();
                    return;
                }
                else
                {
                    Button_Move.Focus();
                }
            }
        }


        private async void Move_Range_Shown(object sender, EventArgs e)
        {
            Edit_SN_Begin.SelectAll();
            Edit_SN_Begin.Focus();

            sql = string.Format("SELECT * FROM SFISM4.R_MO_EXT_TEMP_T");
            var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list.Data.Count() != 0)
             {
             var stringListDic = JsonConvert.SerializeObject(result_list.Data);
             dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
             dataGridView1.DataSource = dt;
             dataGridView1.AutoResizeColumns();
             dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
             }
         }


        private async void Button1_Click(object sender, EventArgs e)
        {
            Edit_SN_Begin.Text = "";
            Edit_SN_Begin.Focus();

            Edit_SN_End.Text = "";

            Edit_Mo.Text = "";

            sql = string.Format("SELECT * FROM SFISM4.R_MO_EXT_TEMP_T");
            var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list.Data.Count() != 0)
            {
                var stringListDic = JsonConvert.SerializeObject(result_list.Data);
                dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                dataGridView1.DataSource = dt;
                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }


        private async void Button_Move_Click(object sender, EventArgs e)
        {
            string c_model, c_model_new;

            if ( (Edit_SN_Begin.Text.Length) == 0) 
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("Please input Begin SN ranges !");
                FrmMessage.MessageVietNam = string.Format("Vui lòng nhập dải đầu SN !");
                FrmMessage.ShowDialog();
                Edit_SN_Begin.SelectAll();
                Edit_SN_Begin.Focus();
                return;
            }

            if ((Edit_SN_End.Text.Length) == 0)  
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("Please input End SN ranges !");
                FrmMessage.MessageVietNam = string.Format("Vui lòng nhập dải cuối SN !");
                FrmMessage.ShowDialog();
                Edit_SN_End.SelectAll();
                Edit_SN_End.Focus();
                return;
            }
            if ((Edit_Mo.Text.Length) == 0)  
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("Please input new MO !");
                FrmMessage.MessageVietNam = string.Format("Vui lòng nhập MO !");
                FrmMessage.ShowDialog();
                Edit_Mo.SelectAll();
                Edit_Mo.Focus();
                return;
            }


            sql = " SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER = '" + Edit_Mo.Text + "'   ";
            var result_4 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_4.Data == null)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("MO: " + Edit_Mo.Text + " not found (R_BPCS_MOPLAN_T)!");
                FrmMessage.MessageVietNam = string.Format("MO: " + Edit_Mo.Text + " không tìm thấy !");
                FrmMessage.ShowDialog();
                Edit_Mo.SelectAll();
                Edit_Mo.Focus();
                return;
            }
            else
            {
                c_model_new = result_4.Data["model_name"].ToString();
            }

            sql = "  SELECT * FROM SFISM4.R_MO_EXT_T WHERE ( '" + Edit_SN_Begin.Text + "' BETWEEN ITEM_1 AND ITEM_2 ) AND ( '" + Edit_SN_End.Text + "'  BETWEEN ITEM_1 AND ITEM_2 )  AND  ( '" + Edit_SN_End.Text + "'   >= '" + Edit_SN_Begin.Text + "' )  and rownum=1 ";
            var result_5 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_5.Data == null)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("SN range: " + Edit_SN_Begin.Text + " - " + Edit_SN_End.Text + " not found !");
                FrmMessage.MessageVietNam = string.Format("Dải SN: " + Edit_SN_Begin.Text + " - " + Edit_SN_End.Text + " không tìm thấy !");
                FrmMessage.ShowDialog();
                Edit_SN_Begin.SelectAll();
                Edit_SN_Begin.Focus();
                return;
            }
            else
            {
                c_model = result_5.Data["ver_5"].ToString();
            }

            if (c_model_new != c_model)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("Model name of range not same with model name of MO ! \nModel_Range: " + c_model + " - Model_Mo: " + c_model_new + "");
                FrmMessage.MessageVietNam = string.Format("Model của dải SN khác với Model của công lệnh ! \nModel_Range: " + c_model + " - Model_Mo: " + c_model_new + "");
                FrmMessage.ShowDialog();
                Edit_SN_Begin.SelectAll();
                Edit_SN_Begin.Focus();
                return;
            }

            sql = "  SELECT * FROM SFISM4.R_MO_EXT_TEMP_T WHERE '" + Edit_SN_Begin.Text + "' <= ITEM_2 AND '" + Edit_SN_Begin.Text + "' >= ITEM_1 AND LENGTH('" + Edit_SN_Begin.Text + "')=LENGTH(ITEM_1) AND LENGTH('" + Edit_SN_Begin.Text + "')=LENGTH(ITEM_2)  ";
            var result_6 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_6.Data != null)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("SN begin had moved !\n" + Edit_SN_Begin.Text + " - R_MO_EXT_TEMP_T");
                FrmMessage.MessageVietNam = string.Format("Dải đầu SN đã chuyển đi !\n" + Edit_SN_Begin.Text + " - R_MO_EXT_TEMP_T");
                FrmMessage.ShowDialog();
                Edit_SN_Begin.SelectAll();
                Edit_SN_Begin.Focus();
                return;
            }

            sql = "  SELECT * FROM SFISM4.R107 WHERE SERIAL_NUMBER BETWEEN '" + Edit_SN_Begin.Text + "' AND '" + Edit_SN_End.Text + "'  ";
            var result_7 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_7.Data != null)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("SN Range have SN inputed! Can not move to other MO!\nCheck range in R107 !");
                FrmMessage.MessageVietNam = string.Format("Dải SN đã được sử dụng! Không thể chuyển sang công lệnh khác!\n Kiểm tra dải trong R107 !");
                FrmMessage.ShowDialog();
                Edit_SN_Begin.SelectAll();
                Edit_SN_Begin.Focus();
                return;
            }


            sql = "  SELECT * FROM SFISM4.R_MO_EXT_TEMP_T WHERE '"+Edit_SN_End.Text +"' <= ITEM_2 AND '"+ Edit_SN_End.Text +"' >=ITEM_1 AND LENGTH('"+Edit_SN_End.Text +"')=LENGTH(ITEM_1) AND LENGTH('"+Edit_SN_End.Text+"')=LENGTH(ITEM_2)  ";
            var result_8 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_8.Data != null)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("SN End had moved !\n"+ Edit_SN_End.Text +" - R_MO_EXT_TEMP_T");
                FrmMessage.MessageVietNam = string.Format("Dải cuối SN đã chuyển đi !\n" + Edit_SN_End.Text + " - R_MO_EXT_TEMP_T");
                FrmMessage.ShowDialog();
                Edit_SN_End.SelectAll();
                Edit_SN_End.Focus();
                return;
            }

            sql = sql + " SELECT * FROM SFISM4.R_MO_EXT_T WHERE '" + Edit_SN_Begin.Text + "' BETWEEN ITEM_1 AND ITEM_2 AND  '" + Edit_SN_End.Text + "' BETWEEN ITEM_1 AND ITEM_2 ";
            sql = sql + " AND '" + Edit_SN_End.Text + "' >= '" + Edit_SN_Begin.Text + "' AND MO_NUMBER = '"+ Edit_Mo.Text + "'  ";
            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            result_8 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_8.Data != null)
            {
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format("SN begin had moved !\n" + Edit_SN_End.Text + " - R_MO_EXT_TEMP_T");
                FrmMessage.MessageVietNam = string.Format("Dải đầu SN đã chuyển đi !\n" + Edit_SN_End.Text + " - R_MO_EXT_TEMP_T");
                FrmMessage.ShowDialog();
                Edit_SN_End.SelectAll();
                Edit_SN_End.Focus();


                sql = sql + " SELECT * FROM SFISM4.R_MO_EXT_T WHERE '" + Edit_SN_Begin.Text + "' BETWEEN ITEM_1 AND ITEM_2 AND  '" + Edit_SN_End.Text + "' BETWEEN ITEM_1 AND ITEM_2 ";
                sql = sql + " AND '" + Edit_SN_End.Text + "' >= '" + Edit_SN_Begin.Text + "' AND MO_NUMBER = '" + Edit_Mo.Text + "'  ";
                var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list.Data.Count() != 0)
                {
                    var stringListDic = JsonConvert.SerializeObject(result_list.Data);
                    dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoResizeColumns();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }

                return;
            }


            DialogResult result_9 = MessageBox.Show("  CONFIRM MOVE RANGE\n\nSN_BEGIN: "+ Edit_SN_Begin.Text + "\nSN_END:    "+Edit_SN_End.Text+"\nMO_NEW: "+ Edit_Mo.Text+"\nAre you sure you want to transfer?", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result_9 == DialogResult.OK)
            {
                sql = "INSERT INTO SFISM4.R_MO_EXT_TEMP_T ";
                sql = sql + " SELECT '"+Edit_Mo.Text+"', '"+Edit_SN_Begin.Text+"',VER_1,'" + Edit_SN_End.Text+"',VER_2,ITEM_3,VER_3,ITEM_4,VER_4,ITEM_5,VER_5,ITEM_6,VER_6 ";
                sql = sql + " FROM SFISM4.R_MO_EXT_T WHERE '" + Edit_SN_Begin.Text + "' BETWEEN ITEM_1 AND ITEM_2 AND  '" + Edit_SN_End.Text + "' BETWEEN ITEM_1 AND ITEM_2 ";
                sql = sql + " AND '" + Edit_SN_End.Text + "' >= '" + Edit_SN_Begin.Text + "' AND ROWNUM = 1 ";
                a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }
                else
                {
                    sql = "INSERT INTO SFISM4.R_SYSTEM_LOG_T(EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC, TIME) ";
                    sql = sql + "VALUES ('"+empNo+ "', 'ADDRWMAC', 'MOVERANGE', 'MOVE RANGE: "+Edit_SN_Begin.Text+" - "+Edit_SN_End.Text+ " TO MO: "+Edit_Mo.Text+"', SYSDATE)";
                    var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                    if (b.Result != "OK")
                    {
                        ReportError(b.Message.ToString());
                        return;
                    }
                    else
                    {
                        sql = string.Format("SELECT * FROM SFISM4.R_MO_EXT_TEMP_T WHERE MO_NUMBER = '"+Edit_Mo.Text+"' ");
                        var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_list.Data.Count() != 0)
                        {
                            var stringListDic = JsonConvert.SerializeObject(result_list.Data);
                            dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                            dataGridView1.DataSource = dt;
                            dataGridView1.AutoResizeColumns();
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        }
                    }
                }
            }
        }
    }

}
