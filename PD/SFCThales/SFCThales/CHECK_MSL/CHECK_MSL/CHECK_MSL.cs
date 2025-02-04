using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Sfc.Library.HttpClient;
using SFCThales;

namespace CHECK_MSL
{
    public partial class CHECK_MSL : Form
    {
        string tmpSN, routeResult, passResult;
        //public EmployeeInfomation empInfo = new EmployeeInfomation();
        public static string empno_msl;
        R107Service fdal;


        /*string line_Name = Comm.GetConfigValue("line_Name");
        string section_Name = Comm.GetConfigValue("section_Name");
        string group_Name = Comm.GetConfigValue("group_Name");
        string station_Name = Comm.GetConfigValue("station_Name");*/
        SfcHttpClient _sfcHttpClient;
        public CHECK_MSL()
        {
            
            InitializeComponent();
            groupname.Text = "CHECK_MSL";
            sectionname.Text = "TEST";
            stationname.Text = "CHECK_MSL";
            Lab_Station.Text = linename.Text + " " + groupname.Text;
            GetForm(groupname.Text);
            fdal = new R107Service();
            _sfcHttpClient = Form1.sfcHttpClient;
        }

        private void CHECK_MSL_Load(object sender, EventArgs e)
        {
            /*empInfo = SFCLogin.Login(false, "CHECK_MSL");
            if (empInfo.CheckPrivilege("CHECK_MSL", "LOGIN") == true)
            {
                
            }
            else
           {
                SFCMessage.Show("Login error | Lỗi đăng nhập", "You not have privilege to login!", "Bạn không có quyền để đăng nhập!");
                System.Windows.Forms.Application.Exit();
            }*/
            btnClear_Click(sender, e);
        }

        private async void txbReelNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (e.KeyChar == 13)
            {
                if(linename.Text == "linename")
                {
                    MessageBox.Show("Please Setup Line", "Messenge", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (string.IsNullOrEmpty(txbReelNo.Text))
                {
                    MessageBox.Show("ReelNo不能為空，請確認！", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    lblMessage.Text = "ReelNo不能為空，請確認！";
                    lblMessage.ForeColor = Color.Red;
                    txbReelNo.SelectAll();
                    return;
                }

                DataTable dtReelNo = await fdal.GetR107ByReelNo(txbReelNo.Text, _sfcHttpClient);
                dgvDataList.DataSource = dtReelNo;
                if (dtReelNo.Rows.Count <= 0)
                {
                    MessageBox.Show("ReelNo不存在，請確認！", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    lblMessage.Text = "ReelNo不存在，請確認！";
                    lblMessage.ForeColor = Color.Red;
                    txbReelNo.SelectAll();
                    return;
                }
                else
                {//檢查時間CHK_REEID到CHECK_MSL,不能超過五分鐘
                 //因為一個Tray過站時間一樣,所以找到任一片SN即可
                 //20200728 QA&PD要求必須5分鐘內,否則提示錯誤,然後產線打回,重新作業
                    string TempOverSN = dtReelNo.Rows[0]["SERIAL_NUMBER"].ToString();
                    string TempModel = dtReelNo.Rows[0]["MODEL_NAME"].ToString();
                    int tempSNOverMinues = 0;
                    int tempOverDefineMinutes = 0;
                    //string TEST = "";
                    DataTable dtOverMinutesBySN = await fdal.GetOverTimeMMinutesBySNandGroup(TempOverSN, "CHK_REEID", _sfcHttpClient);
                    if (dtOverMinutesBySN.Rows.Count > 0)
                    {
                        //TEST = dtOverMinutesBySN.Rows[0]["OVER_MINUTES"].ToString();
                        tempSNOverMinues = int.Parse(dtOverMinutesBySN.Rows[0]["OVER_MINUTES"].ToString());
                            //Convert.ToInt32(dtOverMinutesBySN.Rows[0]["OVER_MINUTES"].ToString());
                    }
                    //查是否有定義,有定義,就檢查,沒有定義就不管
                    //SELECT CONTROL_TIME FROM SFIS1.C_ROAST_TIME_CONTROL_T WHERE MODEL_NAME = 'T77H459.03' AND DEFAULT_GROUP = 'ROAST_IN'  AND END_GROUP = 'ROAST_OUT';
                     DataTable GetOverMinutesByModel =await fdal.GetOverTimeInfoByModelGroup(TempModel, "CHK_REEID", "CHECK_MSL", _sfcHttpClient);
                    if (GetOverMinutesByModel.Rows.Count > 0)
                    {
                        tempOverDefineMinutes = Convert.ToInt32(GetOverMinutesByModel.Rows[0]["CONTROL_TIME"].ToString());
                        if (tempSNOverMinues > tempOverDefineMinutes)
                        {
                            MessageBox.Show("CHK_REEID to CHECK_MSL, more than: " + tempSNOverMinues + "Minutes", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            lblMessage.Text = "CHK_REEID to CHECK_MSL, more than: " + tempSNOverMinues + "Minutes";
                            lblMessage.ForeColor = Color.Red;
                            txbReelNo.SelectAll();
                            return;
                        }
                    }
                }

                int i;
                for (i = 0; i < dtReelNo.Rows.Count; i++)
                {
                    tmpSN = dtReelNo.Rows[i]["SERIAL_NUMBER"].ToString();
                    routeResult = await fdal.ExecCheckRouteProcedure(linename.Text, groupname.Text, tmpSN, _sfcHttpClient);
                    if (!routeResult.Contains("OK"))
                    {
                        MessageBox.Show(tmpSN + ": " + routeResult, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        lblMessage.Text = tmpSN + ": " + routeResult;
                        lblMessage.ForeColor = Color.Red;
                        txbReelNo.SelectAll();
                        return;
                    }
                }

                lblMessage.Text = "ReelNo OK! Please Input MslNo!";
                lblMessage.ForeColor = Color.Green;
                txbReelNo.Enabled = false;
                txbMslNo.Enabled = true;
                txbMslNo.Focus();
            }
        }

        private async void txbMslNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (string.IsNullOrEmpty(txbMslNo.Text))
                {
                    MessageBox.Show("MslNo不能為空，請確認！", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    lblMessage.Text = "MslNo不能為空，請確認！";
                    lblMessage.ForeColor = Color.Red;
                    txbMslNo.SelectAll();
                    return;
                }

                if (txbMslNo.Text != txbReelNo.Text)
                {
                    MessageBox.Show("MslNo與ReelNo不一致，請確認！", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    lblMessage.Text = "MslNo與ReelNo不一致，請確認！";
                    lblMessage.ForeColor = Color.Red;
                    txbMslNo.SelectAll();
                    return;
                }

                DataTable dtMslNo = await fdal.GetR107ByReelNo(txbMslNo.Text, _sfcHttpClient);
                if (dtMslNo.Rows.Count <= 0)
                {
                    MessageBox.Show("MslNo不存在，請確認！", "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    lblMessage.Text = "MslNo不存在，請確認！";
                    lblMessage.ForeColor = Color.Red;
                    txbReelNo.SelectAll();
                    return;
                }

                for (int i = 0; i < dtMslNo.Rows.Count; i++)
                {
                    tmpSN = dtMslNo.Rows[i]["SERIAL_NUMBER"].ToString();
                    passResult = await fdal.ExecR107Procedure(linename.Text, sectionname.Text, groupname.Text, stationname.Text, tmpSN, empno_msl, _sfcHttpClient);

                    if (!passResult.Contains("OK"))
                    {
                        MessageBox.Show(tmpSN + ": " + passResult, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        lblMessage.Text = tmpSN + ": " + passResult;
                        lblMessage.ForeColor = Color.Red;
                        txbReelNo.SelectAll();
                        return;
                    }
                }

                lblMessage.Text = "MslNo CHECK OK! Please Input Next ReelNo!";
                lblMessage.ForeColor = Color.Green;
                txbMslNo.Text = "";
                txbMslNo.Enabled = false;
                txbReelNo.Text = "";
                txbReelNo.Enabled = true;
                txbReelNo.Focus();
            }
        }

        private async void changeLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string content = Interaction.InputBox("Please input Line:", "Message", "", -1, -1);
            DataTable dtline = await fdal.CheckLine(content.Trim().ToUpper(), _sfcHttpClient);
            if (dtline.Rows.Count <= 0)
            {
                MessageBox.Show("Line Wrong.. Please check！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            linename.Text = content.Trim().ToUpper();
            groupname.Text = "CHECK_MSL";
            sectionname.Text = "TEST";
            stationname.Text = "CHECK_MSL";
            Lab_Station.Text = linename.Text + " " + groupname.Text;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvDataList.DataSource = null;
            lblMessage.Text = "Please Input ReelNo!";
            lblMessage.ForeColor = Color.Green;
            txbMslNo.Text = "";
            txbMslNo.Enabled = false;
            txbReelNo.Text = "";
            txbReelNo.Enabled = true;
            txbReelNo.Focus();
        }

        public void GetForm(string groupName)
        {
                if (groupname.Text == "CHECK_MSL")
            {
                gbxCheckMSL.Visible = true;
                dgvDataList.DataSource = null;
                lblMessage.Text = "Please Input ReelNo!";
                lblMessage.ForeColor = Color.Green;
                txbReelNo.Enabled = true;
                txbMslNo.Enabled = false;
                txbReelNo.Focus();
                txbReelNo.Text = "";
                txbMslNo.Text = "";
            }
            else
            {
                gbxCheckMSL.Visible = false;
            }
        }
    }
}
