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


namespace PRE_SCRAP
{
    public partial class Form_Query : Form
    {
        Form opener;
        public string C_GD;
        string sql;
        SfcHttpClient _sfcHttpClient = DBAPI._sfcHttpClient;
        DataTable dt;

        public Form_Query(Form parentForm)
        {
            InitializeComponent();
            opener = parentForm;
            C_GD = Get_Station.gGD;
        }
        private async void Form_Query_Shown(object sender, EventArgs e)
        {
          await Get_View();
        }

        private async Task Get_View()
        {
            sql = " SELECT COUNT(*) COUNT, SCRAP_NO, MODEL_NAME, MO_NUMBER, VERSION_CODE, GD, EMP_NO, REMARK, IN_STATION_TIME FROM SFISM4.R_SCRAP_T WHERE GD = '" + C_GD + "' ";
            sql+= " AND IN_STATION_TIME > SYSDATE - 60 GROUP BY SCRAP_NO, MODEL_NAME, MO_NUMBER, VERSION_CODE, GD, EMP_NO, REMARK, IN_STATION_TIME ORDER BY IN_STATION_TIME DESC ";
            var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list1.Data.Count() != 0)
            {
                var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
                dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                DGView_Query.DataSource = dt;
                DGView_Query.AutoResizeColumns();
                DGView_Query.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void DGView_Query_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = DGView_Query.Rows[e.RowIndex];
                Txt_Remark.Text = row.Cells[7].Value.ToString();
            }
        }

        private async void Txt_Scrap_No_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sql = " SELECT COUNT(*) COUNT, SCRAP_NO, MODEL_NAME, MO_NUMBER, VERSION_CODE, GD, EMP_NO, REMARK, IN_STATION_TIME FROM SFISM4.R_SCRAP_T WHERE GD = '" + C_GD + "' ";
                sql = sql + " AND SCRAP_NO = '" + Txt_Scrap_No.Text + "' AND IN_STATION_TIME > SYSDATE - 60 GROUP BY SCRAP_NO, MODEL_NAME, MO_NUMBER, VERSION_CODE, GD, EMP_NO, REMARK, IN_STATION_TIME ORDER BY SCRAP_NO DESC ";
                var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list1.Data.Count() != 0)
                {
                    var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
                    dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                    DGView_Query.DataSource = dt;
                    DGView_Query.AutoResizeColumns();
                    DGView_Query.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    Txt_Scrap_No.Focus();
                    Txt_Scrap_No.SelectAll();
                }
                else
                {
                    showMessage("" + Txt_Scrap_No.Text + " Not found !", "" + Txt_Scrap_No.Text + " Không tìm thấy !", true);
                    Txt_Scrap_No.Focus();
                    Txt_Scrap_No.SelectAll();
                    return;
                }
            }
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

        private void Btn_Back_Click(object sender, EventArgs e)
        {
            this.Close();
            opener.Show();
        }

        private void Form_Query_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
