using Sfc.Library.HttpClient;
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
using PACK_BOX.Model;
using Sfc.Library.HttpClient.Helpers;

namespace PACK_BOX
{
    /// <summary>
    /// Interaction logic for FormNo_Check_Mo.xaml
    /// </summary>
    public partial class FormNo_Check_Mo : Window
    {
        public SfcHttpClient sfcClient;
        public MainWindow frm_main;
        public FormNo_Check_Mo(MainWindow main,SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            this.frm_main = main;
            sfcClient = _sfcClient;
            formShow();
            edt_pwd.Focus();
        }
        public void formShow()
        {
            edt_pwd.Password = "";
            lst_message.Items.Clear();
            if (frm_main.item_nocheckmo1.IsChecked == true)
            {
                lst_message.Items.Add("After right password!!");
                lst_message.Items.Add("This programe will only for the products which belong to the same MO use!!");
                lst_message.Items.Add("Only SFC input password,do it carefully please!!");
            }
            else
            {
                lst_message.Items.Add("After right password!!");
                lst_message.Items.Add("This programe will allow the products which has the same model and version pass!!");
                lst_message.Items.Add("Only SFC input password,do it carefully please!!");
            }
        }
        private async void edt_pwdKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (!string.IsNullOrEmpty(edt_pwd.Password))
                {
                    string strGetEMP = $"SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_BC='{edt_pwd.Password}'";
                    var qry_EMP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetEMP,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_EMP.Data != null)
                    {
                        string strGetPrivilege = "select * from sfis1.c_privilege "
                           + $" where emp='{qry_EMP.Data["emp_no"].ToString()}' AND FUN='NOT_CHECK_MO' AND PRG_NAME='NETG_PACK_BOX'";
                        var qry_Privilege = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetPrivilege,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_Privilege.Data.Count() > 0)
                        {
                            if (frm_main.item_nocheckmo1.IsChecked == true)
                            {
                                frm_main.item_nocheckmo1.IsChecked = true;
                                frm_main.CHECK_MO_FLAG = false;
                                frm_main.lbError.Text = "Uncheck MACID";
                            }
                            else
                            {
                                frm_main.item_nocheckmo1.IsChecked = false;
                                frm_main.lbError.Text = "";
                                frm_main.CHECK_MO_FLAG = true;
                            }
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("The account don't privilege!", "Message",MessageBoxButton.OK,MessageBoxImage.Question);
                            edt_pwd.SelectAll();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Pass Wrong!", "Message", MessageBoxButton.OK, MessageBoxImage.Question);
                        edt_pwd.SelectAll();
                    }
                }
            }
        }
    }
}
