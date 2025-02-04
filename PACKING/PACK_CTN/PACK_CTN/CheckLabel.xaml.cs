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
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using PACK_CTN.Models;

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for CheckCartonLabel.xaml
    /// </summary>
    public partial class CheckLabel : Window
    {
        public MainWindow _frmMain = new MainWindow();
        public string LineName, MoNumber;
        public bool _CheckLabel = false ;
        public CheckLabel()
        {
            InitializeComponent();
        }

        private async void TbEMP_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbEMP.Password == "")
                {
                    MessageBox.Show("Please input emp no ", "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    return;
                }
                var _emp_desc = await MainWindow._sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = " select * from SFIS1.C_EMP_DESC_T  where emp_no ='" + tbEMP.Password + "' ",
                    SfcCommandType = SfcCommandType.Text
                });

                if (_emp_desc.Data != null)
                {
                    dynamic ads = _emp_desc.Data;
                    string ClassName = ads["class_name"];
                    if (ClassName.Contains("3")) 
                    {
                        EmpName.Content = ads["emp_name"];
                        tbEMP.IsEnabled = false;
                        tbSN.IsEnabled = true;
                        tbSN.Focus();
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = "Nhân viên không có quyền check label đầu !";
                        FrmMessage.MessageEnglish = "EMP no privilege to check first document !";
                        FrmMessage.ShowDialog();
                        return;
                    }
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "EMP không tồn tại !";
                    FrmMessage.MessageEnglish = "EMP not exit !";
                    FrmMessage.ShowDialog();
                    return;
                }
            }
        }

        private void CheckLabel_Loaded(object sender, RoutedEventArgs e)
        {
            EmpName.Content = "";
            tbEMP.IsEnabled = true;
            tbSN.IsEnabled = false;
            tbEMP.Focus();
        }

        private  async void TbSN_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key ==  Key.Enter)
            {
                if (tbSN.Text == "")
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = false;
                    FrmMessage.errorcode = "00199";
                    FrmMessage.ShowDialog();
                    tbSN.SelectAll();
                    tbSN.Focus();
                    return;
                }
                var _R107 = await MainWindow._sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = " SELECT * FROM SFISM4.R107 WHERE  ( SERIAL_NUMBER  = '"+tbSN.Text+"' OR SHIPPING_SN  = '"+tbSN.Text+"') AND WIP_GROUP ='PACK_CTN'  ",
                    SfcCommandType = SfcCommandType.Text
                });

                if (_R107.Data != null)
                {
                    String _SQL =  " UPDATE SFISM4.R_MODELFILE_CHECK_T SET PASS_DATE = SYSDATE,FILE_NO = '"+tbSN.Text+"', "+
                             " EMP_NO= '"+tbEMP.Password+"' , PASS_FLAG='1' WHERE PASS_FLAG='0' AND " +
                                 " LINE_NAME = '"+LineName+"'  AND MO_NUMBER = '"+MoNumber+"' " ;
                    var _Result = await MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = _SQL
                    });
                
                    if (_Result.Result.ToString() != "OK")
                    {
                        MessageBox.Show("Check file error ", "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Check file complete ", "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Error);
                       _CheckLabel = true;
                        this.Close();
                        
                    }
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "SN : '"+tbSN.Text+"' không ở trạm PACK_CTN !";
                    FrmMessage.MessageEnglish = "This '"+tbSN.Text+"' not in Pack_CTN station. Please recheck again";
                    FrmMessage.ShowDialog();
                    return;
                }

            }
        }
    }
}
