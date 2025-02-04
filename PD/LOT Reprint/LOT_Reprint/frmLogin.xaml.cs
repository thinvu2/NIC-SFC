using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System.Windows;
using System.Windows.Input;

namespace LOT_REPRINT
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmLogin : Window
    {
        public System.Windows.Forms.DialogResult ok;
        public string type;
        public SfcHttpClient sfcClient;
        public string emp_input;
        public static string emp_no, sql, s_privilege;
        
        //public string Option;
        //public string Emp_Bc;
        public frmLogin()
        {
            InitializeComponent();
            txtPassword.Focus();
        }
        private async void privilege()
        {
            if (type == "REPRINT")
            {
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = "SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_BC = '" + txtPassword.Password.ToString() + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null)
                {
                    emp_no = result.Data["emp_no"]?.ToString() ?? "";
                    result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = "select * from SFIS1.C_EMP_2_GROUP_T where group_name in('LOT_REPRINT','ALL') AND EMP_NO = '" + emp_no + "'",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (result.Data != null)
                    {
                        ok = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                        return;
                    }
                    else
                    {
                        showMessage("EMP_NO have not privilege REPRINT!", "Mã thẻ này không có quyền REPRINT!", true);
                        txtPassword.SelectAll();
                        txtPassword.Focus();
                        return;
                    }
                }
                else
                {
                    showMessage("Password is incorrect!", "Mật khẩu không chính xác!", true);
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                    return;
                }
            }
            if (type == "ROUTE")
            {
                sql = "SELECT A.EMP_NO,A.EMP_NAME,A.QUIT_DATE,B.Privilege FROM SFIS1.C_EMP_DESC_T A,SFIS1.C_PRIVILEGE B " +
                    " WHERE A.EMP_NO=B.EMP AND B.PRG_NAME='CHECK_LABEL' AND A.EMP_BC='" + txtPassword.Password.ToString() + "'";
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null)
                {
                    s_privilege = result.Data["privilege"]?.ToString() ?? "";
                    if (s_privilege == "2")
                    {
                        ok = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                        frmInputRoute frmInputRoute = new frmInputRoute();
                        frmInputRoute.ShowDialog();
                        return;
                    }
                    else
                    {
                        showMessage("EMP_NO have not privilege!", "Mã thẻ này không có quyền!", true);
                        txtPassword.SelectAll();
                        txtPassword.Focus();
                        return;
                    }
                }
                else
                {
                    showMessage("Password is incorrect!", "Mật khẩu không chính xác!", true);
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                    return;
                }
            }
            if (type == "MSG")
            {
                sql = "SELECT * FROM  SFIS1.C_EMP_DESC_T WHERE  (EMP_BC='" + txtPassword.Password.ToString() + "' OR EMP_PASS='" + txtPassword.Password.ToString() + "') AND  INSTR(CLASS_NAME,'8')>0";
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null)
                {
                    ok = System.Windows.Forms.DialogResult.OK;
                    emp_input = txtPassword.Password.ToString();
                    this.Close();
                    return;
                }
                else
                {
                    showMessage("Password is incorrect!", "Mật khẩu không chính xác!", true);
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                    return;
                }
            }
            if (type == "CHECKROUTE")
            {
                sql = "SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_BC ='" + txtPassword.Password.ToString() + "' AND CLASS_NAME LIKE '%V%'";
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null)
                {
                    ok = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                    frmInputRoute frmInputRoute = new frmInputRoute();
                    frmInputRoute.ShowDialog();
                    return;
                }
                else
                {
                    showMessage("Password is incorrect or EMP_NO not privilege!", "Mật khẩu không chính xác hoặc mã thẻ không có quyền!", true);
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                    return;
                }
            }
            if (type == "Visible")
            {
                sql = "SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_BC ='" + txtPassword.Password.ToString() + "' AND CLASS_NAME LIKE '%V%'";
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null)
                {
                    ok = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                    return;
                }
                else
                {
                    showMessage("EMP_NO have not privilege Visible!", "Mã thẻ này không có quyền Visible!", true);
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                    return;
                }
            }
        }
        private void showMessage(string MessageEnglish, string MessageVietNam, bool CustomFlag)
        {
            frmMessage frmMessage = new frmMessage();
            frmMessage.sfcClient = sfcClient;
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

        private void login_Click(object sender, RoutedEventArgs e)
        {
            privilege();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                privilege();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
