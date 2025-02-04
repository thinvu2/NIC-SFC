using Sfc.Core.Parameters;
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
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using REPAIR.Models;
using Newtonsoft.Json;
using MES.OpINI;

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class frmSelectGroupNext : Window
    {
        MainWindow frmMain = new MainWindow();
        public SfcHttpClient sfcHttpClient;
        public string group, route, C_Not_Group = "'B'", C_SN;
        public string[] RESArray = { "NULL" };
        private char charSub = IniUtil.CharSub;
        public frmSelectGroupNext()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (cbbGroup.Text == "")
            {
                MessageBox.Show("Please select one group!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MainWindow.F_GROUP_NEXT = cbbGroup.Text;
            this.Close();
        }
  

        private void tbInputData_Keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(sender, e);
            }   

        }

        private async Task<bool> Get_Wip_Return()
        {
            try
            {
                var logInfo = new
                {
                    OPTION = "GET_WIP",
                    DATA = C_SN,
                    ROUTE = route,
                    GROUP = group
                };
                string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

                var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REPAIR_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                {
                    new SfcParameter {Name = "DATA" ,Value = jsonData, SfcParameterDataType = SfcParameterDataType.Varchar2 , SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter {Name = "OUTPUT" ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output}
                }
                });
                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string RES = output[0]["output"];
                    RESArray = RES.Split(charSub);
                    if (RESArray[0] == "OK")
                    {
                        C_Not_Group = RESArray[1].ToString();

                        if(C_Not_Group.Length < 2)
                        {
                            C_Not_Group = "'B'";
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async void frmSelectGroupNext_Loaded(object sender, RoutedEventArgs e)
        {

            string sql = "  SELECT * FROM SFIS1.C_PARAMETER_INI  WHERE PRG_NAME ='REPAIR'  AND VR_CLASS ='TRUE' AND VR_ITEM ='MBD1'  AND VR_DESC ='SN DA XOA KPN K DC XOA R_ TAI TRAM'  ";
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });

            if (result.Data != null && result.Data.Count() > 0) // BTB. 24/10/23   SN DA XOA KPN K DC XOA R_ TAI TRAM (SO SANH TRAM KPS VOI TRAM RETURN DAU KHI GIAI R_)
            {
               await Get_Wip_Return();
            }


            sql = "  SELECT GROUP_NEXT   FROM sfis1.C_ROUTE_CONTROL_T  WHERE  group_name = '" + group + "'  AND state_flag = 0   AND route_code = '" + route + "' and GROUP_NEXT not in (" + C_Not_Group + ") ";
            result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count() > 0)
            {
                foreach (var row in result.Data)
                {
                    cbbGroup.Items.Add(row["group_next"].ToString());
                }
                cbbGroup.SelectedIndex = 0;
            }
            else
            {
                showError("Get group next result null ", " Route: " + route + " ;Group: " + group, true);
                return;
            }
            cbbGroup.Focus();
        }

        private void showError(string strVN, string strEng, bool Flag)
        {
            MessageError FrmMessage = new MessageError();
            FrmMessage.sfcHttpClient = this.sfcHttpClient;
            if (Flag)
            {
                FrmMessage.CustomFlag = Flag;
                FrmMessage.MessageVietNam = strVN;
                FrmMessage.MessageEnglish = strEng;
            }
            else
            {
                FrmMessage.CustomFlag = Flag;
                FrmMessage.errorcode = strEng;
            }

            FrmMessage.ShowDialog();
        }
    }
}
