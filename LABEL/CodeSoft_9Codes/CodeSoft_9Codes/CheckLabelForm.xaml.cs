using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


using Sfc.Core.Parameters;

namespace CodeSoft_9Codes
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CheckLabelForm : Window
    {
        public string password = "";
        string res, output;
        public string empNo, empName;
        public CheckLabelForm(string inMo,string inlabelType)
        {
            InitializeComponent();
            password = "";
            txtPassword.Focus();
            lblMO.Content = inMo;
            lblLabelType.Content = inlabelType;
            empNo = "";
            empName = "";
        }

        private async void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                password = txtPassword.Password;
                if (!string.IsNullOrWhiteSpace(password))
                {
                    await call_SP(password, "CheckLabelPWD");
                    if (res.StartsWith("OK"))
                    {
                        showMessage(res);
                        empNo = getVal(output, "EmpNo");
                        empName = getVal(output, "EmpName");
                        lblName.Content = "Employee : "+ empName;
                        txtPassword.IsEnabled = false;
                        txtLabel.Text = "";
                        txtLabel.Focus();
                    }
                    else
                    {
                        txtPassword.SelectAll();
                        txtPassword.Focus();
                    }
                }
            }
        }
        public async Task call_SP(string in_data, string in_func)
        {
            var result = await MainWindow.sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.SP_CODESOFT_NIC",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="AP_VER",Value=MainWindow.APVersion,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_EMP",Value=empNo,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="PCMAC",Value=MainWindow.PCMAC,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="PCIP",Value=MainWindow.PCIP,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_FUNC",Value=in_func,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_DATA",Value=in_data,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_LABELTYPE",Value=lblLabelType.Content,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_MO",Value=lblMO.Content,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_QTY",Value="",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output},
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

            });
            if (result.Data != null)
            {
                dynamic ads = result.Data;
                output = ads[0]["out_data"];
                res = ads[1]["res"];
                if (!res.StartsWith("OK"))
                {
                    showMessage(res);
                    return;
                }
            }
        }

        private async void txtLabel_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtLabel.Text))
                {
                    await call_SP(txtLabel.Text, "CheckLabelSN");
                    if (res.StartsWith("OK"))
                    {
                        showMessage(res);
                        txtLabel.Text = "";
                        txtLabel.Focus();
                    }
                    else
                    {
                        txtLabel.SelectAll();
                        txtLabel.Focus();
                    }
                }
            }
        }

        private string getVal(string input, string column)
        {
            string res = "";
            foreach (string arr in input.Split('|'))
            {
                if (arr.StartsWith(column + ":"))
                {
                    for (int i = 1; i < arr.Split(':').Count(); i++)
                    {
                        res += arr.Split(':')[i] + ":";
                    }
                    return res.Substring(0, res.Length - 1);
                }
            }
            return "";
        }
        private void showMessage(string msg)
        {
            string Emsg, Vmsg;
            if (msg.IndexOf("|") > 0)
            {
                Emsg = msg.Split('|')[0];
                Vmsg = msg.Split('|')[1];
            }
            else
            {
                Emsg = msg;
                Vmsg = msg;
            }
            if (Vmsg.StartsWith("OK"))
            {
                txtMsgV.Foreground = new System.Windows.Media.SolidColorBrush(Colors.White);
                txtMsgV.Text = Vmsg;
                txtMsgE.Text = "";
            }
            else
            {
                txtMsgV.Foreground = new System.Windows.Media.SolidColorBrush(Colors.Red);
                txtMsgE.Foreground = new System.Windows.Media.SolidColorBrush(Colors.Red);
                txtMsgV.Text = Vmsg;
                txtMsgE.Text = Emsg;
            }
        }
    }
}
