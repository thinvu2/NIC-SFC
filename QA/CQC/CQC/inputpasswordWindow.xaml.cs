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
using CQC.ViewModel;
using Sfc.Library.HttpClient.Helpers;

namespace CQC
{
    /// <summary>
    /// Interaction logic for inputpasswordWindow.xaml
    /// </summary>
    public partial class inputpasswordWindow : Window
    {
        private MainWindow formCQC;
        SfcHttpClient sfcClient;
        public inputpasswordWindow()
        {
            InitializeComponent();
        }
        public inputpasswordWindow(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            sfcClient = formCQC.sfcClient;
            InitializeComponent();
        }

        private async void Edit1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string sEMP = formCQC.C_pallet_no;
                var quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText= "SELECT emp_no, emp_name, emp_bc FROM SFIS1.C_EMP_DESC_T WHERE EMP_BC ='"+Edit1.Password+"' AND CLASS_NAME LIKE '%QE%' ",
                    SfcCommandType=SfcCommandType.Text
                });
                if (quryTemp.Data.Count() > 0)
                {
                    List<EMP> emplist = new List<EMP>();
                    emplist= quryTemp.Data.ToListObject<EMP>().ToList();
                    sEMP = emplist[0].EMP_NO;
                    formCQC.isPassword = true;
                    quryTemp = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "INSERT INTO SFISM4.R_SYSTEM_LOG_T (EMP_NO, PRG_NAME, ACTION_TYPE, TIME) values (:EMP_NO, :PRG_NAME, :ACTION_TYPE, sysdate)",
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter {Name = "EMP_NO", Value=sEMP },
                            new SfcParameter {Name = "PRG_NAME" , Value= formCQC.C_pallet_no},
                            new SfcParameter {Name = "ACTION_TYPE", Value="TA" }
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Passwork wrong!","CQC");
                    Edit1.Password = "";
                    Edit1.Focus();
                }
            }
        }
    }
}
