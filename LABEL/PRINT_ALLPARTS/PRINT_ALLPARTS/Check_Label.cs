using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRINT_ALLPART
{
    public partial class Check_Label : Form
    {
        //public Check_Label()
        //{
        //    InitializeComponent();
        //}
        public string mo,skud;
        public string emp_no;
        SfcHttpClient _sfcHttpClient;
        public string[] _RESArray = { "NULL" };
        public Check_Label(string _mo,string _skud, SfcHttpClient sfcHttpClient)
        {
            InitializeComponent();
            mo = _mo;
            skud = _skud;
            txt_MoNumber.Text = mo;
            _sfcHttpClient = sfcHttpClient;
            txtPw.Focus();
            txtPw.UseSystemPasswordChar = true;
        }


        private void Check_Label_Load(object sender, EventArgs e)
        {

        }

        private async void txtPw_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (await Check_PrivilegeAsync(txtPw.Text) != "OK")
                {
                    MessageBox.Show("Mã thẻ không có quyền!", "WARRING", MessageBoxButtons.OK);
                    return;
                }
            }
        }
        public async Task<string> Check_PrivilegeAsync(string pw)
        {
            string pass = string.Format("SELECT a.emp_no,a.emp_name FROM SFIS1.C_EMP_DESC_T a, SFIS1.C_PRIVILEGE b " +
                "        WHERE EMP = emp_no AND FUN = 'PRINT_OVER' and emp_bc = '{0}'" +
                "        AND PRG_NAME = 'CODESOFT' AND PRIVILEGE = '2' ", pw);
            var qry_KeyPart2 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = pass,
                SfcCommandType = SfcCommandType.Text
            });


            if (qry_KeyPart2.Data.Count() > 0)
            {
                dynamic _ads = qry_KeyPart2.Data;

                emp_no = txtPw.Text;
                txtemp.Text = _ads[0]["emp_name"];
                txtemp.Visible = true;
                txtScan.Enabled = true;
                txtemp.Enabled = false;
                txtPw.Enabled = false;
                txtScan.Focus();
                return "OK";
            }
            else
            {
                return "Fail";
            }
        }

        private async void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtScan.Text.ToUpper().Contains("CLOSE"))
                {
                    this.Close();
                    return;
                }
                await Check_LabelAsync(txt_MoNumber.Text, txtScan.Text, emp_no);
            }
        }

        private void txtPw_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        public async Task<bool> Check_LabelAsync(string mo_number, string txtSN, string emp)
        {
            var logInfo = new
            {
                step = "CheckLabelSN",
                C_TYPE = skud,
                monumber = mo_number,
                IN_QTY = 0,
                version = "",
                beginsn = txtSN,
                endsn = emp,
            };
            string Jsdata = JsonConvert.SerializeObject(logInfo).ToString();

            var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.SP_PRINT_ALLPARTS",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="step",Value= "CheckLabelSN",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="C_TYPE",Value= skud,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="monumber",Value= mo_number,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_QTY",Value=  0,SfcParameterDataType=SfcParameterDataType.Int32,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="version",Value= "N/A",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="beginsn",Value= txtSN,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="endsn",Value= emp,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output},
                        new SfcParameter{Name="endsn",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

            });
            if (result.Data != null)
            {
                dynamic ads = result.Data;
                string _RES = ads[0]["res"];
                if (!_RES.Contains("OK"))
                {
                    MessageBox.Show(_RES.ToString(), "ALERT", MessageBoxButtons.OK);
                    return false;
                }
                else
                {
                    string res = string.Format("select * from SFISM4.r_data_input_t where ssn1 = '{0}'", _RES.Substring(3));
                    var qryssn = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = res,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qryssn.Data.Count() > 0)
                    {
                        var vardatatabel = JsonConvert.SerializeObject(qryssn.Data);
                        dataGridView1.DataSource = JsonConvert.DeserializeObject<DataTable>(vardatatabel);
                        txtScan.SelectAll();
                        txtScan.Focus();
                        return true;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
