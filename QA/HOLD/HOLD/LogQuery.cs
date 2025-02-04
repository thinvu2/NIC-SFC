using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using SFC_Library;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using Sfc.Library.HttpClient;
using System.Text.RegularExpressions;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient.Helpers;
using HOLD.LogInfo;
using System.Globalization;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using HOLD;

namespace HOLD
{
    public partial class LogQuery : Form
    {
        private FormMain objfrmMain;
        const string ERRORSTRING = "Message";

        public SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string empNo = "";
        List<infLogQuery> listLogQuery = new List<infLogQuery>();
        public LogQuery(FormMain frmMain)
        {
            InitializeComponent();
            objfrmMain = frmMain;
            loginInfor = frmMain.loginInfor;
            string[] digits = Regex.Split(loginInfor, @";");
            connectDB.Text = digits[0].ToString();
            tssEmp.Text = digits[1].ToString();
            empNo = Regex.Split(tssEmp.Text, @"-")[0].Trim();
        }

        private void showMessage(string Message)
        {
            StackFrame CallStack = new StackFrame(1, true);
            MessageBox.Show("Error: " + Message + ",\n\rFile: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }

        private void LogQuery_Shown(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private async void btnQuery_Click(object sender, EventArgs e)
        {
            string sql = "", ssql = "";
            listLogQuery = null;
            if (textBox1.Text.Trim() != "")
            {
                ssql = "SELECT SUBSTR (B.EMP_NAME, 1, 8) EMP_NAME, " +
                      " SUBSTR (A.PRG_NAME, 1, 8) PRG_NAME, " +
                      " SUBSTR (A.ACTION_TYPE, 1, 8) ACTION_TYPE, " +
                      " SUBSTR (A.ACTION_DESC, 1, 80) ACTION_DESC, TO_CHAR(TIME, 'YYYY-MM-DD HH24:MI:SS') TIME " +
                      " FROM SFISM4.R_SYSTEM_LOG_T A, SFIS1.C_EMP_DESC_T B " +
                      " WHERE A.EMP_NO = B.EMP_NO " +
                      " AND A.ACTION_TYPE IN ('" + comboBox1.Text + "') " +
                      " AND ACTION_DESC LIKE '%" + textBox1.Text + "%') " +
                      " AND A.TIME > SYSDATE - " + comboBox2.Text + 
                      " ORDER BY A.TIME DESC ";

                sql = " SELECT B.EMP_NAME EMP_NAME, " +
                        " A.PRG_NAME PRG_NAME, " +
                        " A.ACTION_TYPE ACTION_TYPE," +
                        "  SUBSTR (A.ACTION_DESC, 1, 80) ACTION_DESC, TO_CHAR(TIME, 'YYYY-MM-DD HH24:MI:SS') TIME " +
                        " FROM SFISM4.R_SYSTEM_LOG_T A, SFIS1.C_EMP_DESC_T B " +
                        " WHERE A.EMP_NO = B.EMP_NO " +
                        " AND A.ACTION_TYPE IN ('" + comboBox1.Text + "') " +
                        " AND ACTION_DESC LIKE '%" + textBox1.Text + "%') " +
                        " AND A.TIME > SYSDATE - " + comboBox2.Text +
                        " ORDER BY A.TIME DESC ";
            }
            else
            {
                ssql = " SELECT SUBSTR (B.EMP_NAME, 1, 8) EMP_NAME, " +
                      " SUBSTR (A.PRG_NAME, 1, 8) PRG_NAME, " +
                      " SUBSTR (A.ACTION_TYPE, 1, 8) ACTION_TYPE, " +
                      " SUBSTR (A.ACTION_DESC, 1,500) ACTION_DESC, TO_CHAR(TIME, 'YYYY-MM-DD HH24:MI:SS') TIME " +
                      " FROM SFISM4.R_SYSTEM_LOG_T A, SFIS1.C_EMP_DESC_T B " +
                      " WHERE A.EMP_NO = B.EMP_NO " +
                      " AND A.ACTION_TYPE IN ('" + comboBox1.Text + "') " +
                      " AND A.TIME > SYSDATE - " + comboBox2.Text +
                      " ORDER BY A.TIME DESC ";

                sql = " SELECT B.EMP_NAME EMP_NAME, " +
                        " A.PRG_NAME PRG_NAME, " +
                        " A.ACTION_TYPE ACTION_TYPE," +
                        " SUBSTR (A.ACTION_DESC, 1,500) ACTION_DESC, TO_CHAR(TIME, 'YYYY-MM-DD HH24:MI:SS') TIME " +
                        " FROM SFISM4.R_SYSTEM_LOG_T A, SFIS1.C_EMP_DESC_T B " +
                        " WHERE A.EMP_NO = B.EMP_NO " +
                        " AND A.ACTION_TYPE IN ('" + comboBox1.Text + "') " +
                        " AND A.TIME > SYSDATE - " + comboBox2.Text +
                        " ORDER BY A.TIME DESC ";
            }
            if (comboBox1.Text == "AUTO HOLD")
            {
                if (textBox1.Text.Trim() != "")
                {
                    sql = "SELECT SERIAL_NUMBER, MODEL_NAME, HOLD_EMP_NO, TO_CHAR(HOLD_TIME, 'YYYY-MM-DD HH24:MI:SS') HOLD_TIME, HOLD_REASON FROM SFISM4.R_SYSTEM_HOLD_T WHERE" +
                          " HOLD_TIME > SYSDATE - " + comboBox2.Text + " AND HOLD_REASON LIKE '%AUTO HOLD%' AND MODEL_NAME ='" + textBox1.Text + "' ORDER BY HOLD_TIME DESC";

                    ssql = "SELECT SERIAL_NUMBER, MODEL_NAME, HOLD_EMP_NO, TO_CHAR(HOLD_TIME, 'YYYY-MM-DD HH24:MI:SS') HOLD_TIME, HOLD_REASON FROM SFISM4.R_SYSTEM_HOLD_T WHERE" +
                           " HOLD_TIME > SYSDATE - " + comboBox2.Text + " AND HOLD_REASON LIKE '%AUTO HOLD%' ORDER BY HOLD_TIME DESC";
                }
                else
                {
                    sql = "SELECT SERIAL_NUMBER, MODEL_NAME, HOLD_EMP_NO, TO_CHAR(HOLD_TIME, 'YYYY-MM-DD HH24:MI:SS') HOLD_TIME, HOLD_REASON FROM SFISM4.R_SYSTEM_HOLD_T WHERE" +
                          " HOLD_TIME >SYSDATE - " + comboBox2.Text + " AND HOLD_REASON LIKE '%AUTO HOLD%' ORDER BY HOLD_TIME DESC";

                    ssql = "SELECT SERIAL_NUMBER, MODEL_NAME, HOLD_EMP_NO, TO_CHAR(HOLD_TIME, 'YYYY-MM-DD HH24:MI:SS') HOLD_TIME, HOLD_REASON FROM SFISM4.R_SYSTEM_HOLD_T WHERE" +
                           " HOLD_TIME >SYSDATE - " + comboBox2.Text + " AND HOLD_REASON LIKE '%AUTO HOLD%' ORDER BY HOLD_TIME DESC";
                }
            }
            var resultLogQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text,
            });
            listLogQuery = resultLogQuery.Data.ToListObject<infLogQuery>().ToList();

            dataGridView1.DataSource = null;            
            if (comboBox1.Text == "AUTO HOLD")
            {
                dataGridView1.DataSource = listLogQuery.Select(p => new { p.SERIAL_NUMBER, p.MODEL_NAME, p.HOLD_EMP_NO, p.HOLD_REASON, p.HOLD_TIME }).ToList();
            }
            else
            {
                dataGridView1.DataSource = listLogQuery.Select(p => new { p.EMP_NAME, p.PRG_NAME, p.ACTION_TYPE, p.ACTION_DESC, p.TIME }).ToList();
            }           
            dataGridView1.Refresh();
            lblQty.Text = dataGridView1.Rows.Count.ToString();
        }

        private void ExportDgvToXML(List<infLogQuery> list)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                if (comboBox1.Text == "AUTO HOLD")
                {
                    dt.Columns.Add("SERIAL_NUMBER");
                    dt.Columns.Add("MODEL_NAME");
                    dt.Columns.Add("HOLD_EMP_NO");
                    dt.Columns.Add("HOLD_TIME");
                    dt.Columns.Add("HOLD_REASON");
                    foreach (infLogQuery items in list)
                    {
                        dt.Rows.Add(items.SERIAL_NUMBER, items.MODEL_NAME, items.HOLD_EMP_NO, items.HOLD_TIME, items.HOLD_REASON);
                    }

                }
                else
                {
                    dt.Columns.Add("EMP_NAME");
                    dt.Columns.Add("PRG_NAME");
                    dt.Columns.Add("ACTION_TYPE");
                    dt.Columns.Add("ACTION_DESC");
                    dt.Columns.Add("TIME");

                    foreach (infLogQuery items in list)
                    {
                        dt.Rows.Add(items.EMP_NAME, items.PRG_NAME, items.ACTION_TYPE, items.ACTION_DESC, items.TIME);
                    }
                }

                dt.TableName = "DATA";
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Files (2003)|*.xls|Excel Files (2007)|*.xlsx";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        dt.WriteXml(sfd.FileName);
                        MessageBox.Show("Export Successfully", "Notice");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                throw;
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExportDgvToXML(listLogQuery);            
        }
    }
}
