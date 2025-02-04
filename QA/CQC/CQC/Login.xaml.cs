using Sfc.Core.Parameters;
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
using CQC.ViewModel;
using Sfc.Library.HttpClient.Helpers;

namespace CQC
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        bool chkbRejectReason, chkbInsertQCSN, chkbCheckRoute, chkbUpdateQANoResult, chkbRemoveFailSSN, chkbInsertSNtoPallet, chkbTransferbyPiece, chkbWarehouseNO, chkbPO, chkbClearPallet, chkbClearCarton, chkbSamplingPlan;
        string editPalletFullFlag, editCompany;
        SfcHttpClient sfcClient;
        public Login()
        {
            InitializeComponent();
        }
        public Login(SfcHttpClient _sfcClient, bool _chkbRejectReason, bool _chkbInsertQCSN,bool _chkbCheckRoute, bool _chkbUpdateQANoResult, bool _chkbRemoveFailSSN, bool _chkbInsertSNtoPallet, bool _chkbTransferbyPiece, bool _chkbWarehouseNO, bool _chkbPO, bool _chkbClearPallet, bool _chkbClearCarton, bool _chkbSamplingPlan,string _editPalletFullFlag,string _editCompany)
        {
            sfcClient = _sfcClient;
            chkbRejectReason = _chkbRejectReason;
            chkbInsertQCSN = _chkbInsertQCSN;
            chkbInsertQCSN = _chkbInsertQCSN;
            chkbCheckRoute = _chkbCheckRoute;
            chkbUpdateQANoResult = _chkbUpdateQANoResult;
            chkbRemoveFailSSN = _chkbRemoveFailSSN;
            chkbInsertSNtoPallet = _chkbInsertSNtoPallet;
            chkbTransferbyPiece = _chkbTransferbyPiece;
            chkbWarehouseNO = _chkbWarehouseNO;
            chkbPO = _chkbPO;
            chkbClearPallet = _chkbClearPallet;
            chkbClearCarton = _chkbClearCarton;
            chkbSamplingPlan = _chkbSamplingPlan;
            editPalletFullFlag = _editPalletFullFlag;
            editCompany = _editCompany;
            InitializeComponent();
        }
        private async void bbtnOK_Click(object sender, RoutedEventArgs e)
        {
            List<passw> passlist = new List<passw>();
            int flag = 0;
            if (editPWD.Password == (int.Parse(DateTime.Now.ToString("yyyyMMdd")) + 3).ToString())
                flag = 1;
            if (flag != 1 && editUserID.Text == "")
            {
                MessageBox.Show("Please Input User ID!", "mtError", MessageBoxButton.OKCancel);
                editUserID.SelectAll();
                editUserID.Focus();
                flag = 1;
            }
            if (flag != 1 && editPWD.Password == "")
            {
                MessageBox.Show("Please Input Password!", "mtError", MessageBoxButton.OKCancel);
                editPWD.SelectAll();
                editPWD.Focus();
                flag = 1;
            }
            if (flag != 1)
            {
                var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from SFIS1.C_PRIVILEGE where EMP =:sUser and PRG_NAME='CQC_CRoute'",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                        {
                            new SfcParameter {Name = "sUser" , Value=editUserID.Text }
                        }
                });
                passlist = result.Data.ToListObject<passw>().ToList();
                if (result.Data.Count()!=0)
                {
                    MessageBox.Show("Illegal User ID.","WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                    editUserID.SelectAll();
                    editUserID.Focus();
                    flag= 1;
                }
                else
                {
                    if(passlist.Count(c => c.PASSW == editPWD.Password) == 0)
                    {
                        MessageBox.Show("Password check failed.");
                        editPWD.SelectAll();
                        editPWD.Focus();
                        flag= 1;
                    }
                }
            }
            if (editPWD.Password == (int.Parse(DateTime.Now.ToString("yyyyMMdd")) + 3).ToString())
                flag = 0;
            if (flag == 0)
            {
                SystemWindow formSystem = new SystemWindow(chkbRejectReason, chkbInsertQCSN, chkbCheckRoute, chkbUpdateQANoResult, chkbRemoveFailSSN, chkbInsertSNtoPallet, chkbTransferbyPiece, chkbWarehouseNO, chkbPO, chkbClearPallet, chkbClearCarton, chkbSamplingPlan, editPalletFullFlag, editCompany);
                this.Hide();
                formSystem.ShowDialog();
            }
            this.Close();
        }

        private void bbtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            var query = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select * from  SFIS1.C_PRIVILEGE where emp='dba' and prg_name='CQC_CRoute' AND PRIVILEGE='0'",
                SfcCommandType = SfcCommandType.Text
            });
            if (query.Result.Count() == 0)
            {
                var insertsql = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "INSERT INTO SFIS1.C_PRIVILEGE (EMP, PASSW, FUN, PRIVILEGE, PRG_NAME) values (:EMP, :PASSW, :FUN, :PRIVILEGE, :PRG_NAME)",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter {Name = "EMP", Value="dba" },
                            new SfcParameter {Name = "PASSW" ,Value= "dba"},
                            new SfcParameter {Name = "FUN", Value="" },
                            new SfcParameter {Name = "PRIVILEGE", Value="0" },
                            new SfcParameter {Name = "PRG_NAME", Value="CQC_CRoute" }
                        }
                });
            }
            editUserID.Text = "";
            editPWD.Password = "";
            editUserID.Focus();
        }
    }
}
