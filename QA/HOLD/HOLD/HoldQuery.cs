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
using System.IO;

namespace HOLD
{
    public partial class HoldQuery : Form
    {
        //public EmployeeInfomation _empInfo = new EmployeeInfomation();
        //public OracleClientDBHelper dbsfis = null;

        private FormMain objfrmMain;
        const string ERRORSTRING = "Message";
        public int linecount;

        public SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string empNo = "";

        public HoldQuery(FormMain frmMain)
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

        private void HoldQuery_Shown(object sender, EventArgs e)
        {
            //tssEmp.Text = string.Format("Login user: {0} - {1}", _empInfo.EmployeeNo, _empInfo.EmployeeName);
            cbbCondition.SelectedIndex = 0;
            cbbAction.SelectedIndex = 0;
        }

        private async void btnQuery_Click(object sender, EventArgs e)
        {
            string sql = "", strsql = "", head = "", type="";
            linecount = Regex.Matches(txtCondition.Text, "\r\n").Count + 1;

            if (cbbCondition.Text== "MAIN_DESC")
            {
                strsql = "SELECT SERIAL_NUMBER, MODEL_NAME, MAIN_DESC, HOLD_EMP_NO,TO_CHAR(HOLD_TIME, 'YYYY-MM-DD HH24:MI:SS') HOLD_TIME, HOLD_REASON, HOLD_PROGRAM, UNHOLD_EMP_NO,TO_CHAR(UNHOLD_TIME, 'YYYY-MM-DD HH24:MI:SS') UNHOLD_TIME, UNHOLD_REASON, UNHOLD_PROGRAM, FINISH_FLAG, DATA1, DATA2, DATA3 FROM SFISM4.R_SYSTEM_HOLD_T WHERE MAIN_DESC IN(";
                for (int i = 0; i < linecount; i++)
                {
                    if (i == 0)
                    {
                        strsql = strsql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                    }
                    else
                    {
                        strsql = strsql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                    }
                }
                strsql = strsql + " ) ";
                goto startQuery;
            }

            if (cbbAction.Text == "")
            {
                showMessage("PLEASE INPUT HOLD OR UNHOLD");
                //MessageBox.Show("PLEASE INPUT HOLD OR UNHOLD", errorstring);
                cbbAction.Focus();
                return;
            }

            if (cbxFG.Checked)
            {
                head = "SFISM4.Z_WIP_TRACKING_T";
            }
            else
            {
                head = "SFISM4.R_WIP_TRACKING_T";
            }

            if (cbbAction.Text == "HOLD")
            {
                type = " GROUP_NAME LIKE '%HOLD%' AND ";
            }
            else if (cbbAction.Text == "UNHOLD")
            {
                type = " ";
            }

            sql = "SELECT SERIAL_NUMBER FROM " + head + " WHERE " + type + cbbCondition.Text + " IN (";
            for(int i = 0; i < linecount; i++)
            {
                if (i == 0)
                {
                    sql = sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                }
                else
                {
                    sql = sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                }
            }
            sql = sql + " ) ";

            strsql = "SELECT SERIAL_NUMBER, MODEL_NAME, MAIN_DESC, HOLD_EMP_NO,TO_CHAR(HOLD_TIME, 'YYYY-MM-DD HH24:MI:SS') HOLD_TIME, HOLD_REASON, HOLD_PROGRAM, UNHOLD_EMP_NO,TO_CHAR(UNHOLD_TIME, 'YYYY-MM-DD HH24:MI:SS') UNHOLD_TIME, UNHOLD_REASON, UNHOLD_PROGRAM, FINISH_FLAG, DATA1, DATA2, DATA3 FROM SFISM4.R_SYSTEM_HOLD_T WHERE SERIAL_NUMBER IN (";
            strsql = strsql + sql + ")";

            if (cbbAction.Text == "HOLD")
            {
                strsql = strsql + " AND FINISH_FLAG = '0' ";
            }
            else if (cbbAction.Text == "UNHOLD")
            {
                strsql = strsql + " AND FINISH_FLAG = '1' ";
            }

            startQuery:
            var resultHoldQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strsql,
                SfcCommandType = SfcCommandType.Text,
            });

            if(resultHoldQuery.Data is null)
            {
                showMessage("No data found");
                txtCondition.SelectAll();
                return;
            }

            if (resultHoldQuery.Data.Count() != 0)
            {
                var a = resultHoldQuery.Data.ToListObject<infHoldQuery>();
                List<infHoldQuery> listHoldQuery = a.Cast<infHoldQuery>().ToList();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = listHoldQuery;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.Refresh();
                lblQty.Text = dataGridView1.Rows.Count.ToString();
            }
            else
            {
                showMessage("No data found");
                txtCondition.SelectAll();
                return;
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                List<infHoldQuery> datalist = dataGridView1.DataSource as List<infHoldQuery>;
                ExportDgvToXML(datalist);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void ExportDgvToXML(List<infHoldQuery> list)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("SERIAL_NUMBER");
            dt.Columns.Add("MODEL_NAME");
            dt.Columns.Add("MAIN_DESC");
            dt.Columns.Add("HOLD_EMP_NO");
            dt.Columns.Add("HOLD_TIME");
            dt.Columns.Add("HOLD_REASON");
            dt.Columns.Add("HOLD_PROGRAM");
            dt.Columns.Add("UNHOLD_EMP_NO");
            dt.Columns.Add("UNHOLD_TIME");
            dt.Columns.Add("UNHOLD_REASON");
            dt.Columns.Add("UNHOLD_PROGRAM");
            dt.Columns.Add("FINISH_FLAG");
            dt.Columns.Add("DATA1");
            dt.Columns.Add("DATA2");
            dt.Columns.Add("DATA3");
            foreach(infHoldQuery items in list)
            {
                dt.Rows.Add(items.SERIAL_NUMBER, items.MODEL_NAME, items.MAIN_DESC, items.HOLD_EMP_NO, items.HOLD_TIME, items.HOLD_REASON, items.HOLD_PROGRAM, items.UNHOLD_EMP_NO, items.UNHOLD_TIME, items.UNHOLD_REASON, items.UNHOLD_PROGRAM, items.FINISH_FLAG, items.DATA1, items.DATA2, items.DATA3);
            }
            dt.TableName = "DATA";
            SaveFileDialog sfd = new SaveFileDialog();
            //sfd.Filter = "XML|*.xml";
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
    }
}
