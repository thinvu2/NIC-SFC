using CQC.ViewModel;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
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

namespace CQC
{
    /// <summary>
    /// Interaction logic for RejectReasonWindow.xaml
    /// </summary>
    public partial class RejectReasonWindow : Window
    {
        MainWindow formCQC;
        public SfcHttpClient sfcClient;
        public RejectReasonWindow(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            sfcClient = formCQC.sfcClient;
            InitializeComponent();
        }

        private void bbtnClear_Click(object sender, RoutedEventArgs e)
        {
            meno1.Document.Blocks.Clear();
            meno1.Focus();
        }

        private void bbtnCancel_Click(object sender, RoutedEventArgs e)
        {
            meno1.Document.Blocks.Clear();
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            if(formCQC.ItemRejectGroup.IsChecked)
            {
                lablNextGroup.IsEnabled = true;
                string sSQL = "SELECT DISTINCT B.GROUP_NEXT FROM  SFIS1.C_ROUTE_CONTROL_T B, " +
                                " (SELECT GROUP_NEXT, ROUTE_CODE FROM SFIS1.C_ROUTE_CONTROL_T " + 
                                " WHERE GROUP_NAME =:MyGroup AND STATE_FLAG = '1' " +
                                " AND ROUTE_CODE = (SELECT DISTINCT SPECIAL_ROUTE FROM SFISM4.R107 WHERE QA_NO =:Qa_no)) A " +
                                " WHERE A.ROUTE_CODE = B.ROUTE_CODE AND B.GROUP_NAME = A.GROUP_NEXT AND B.STATE_FLAG = '0' ";
                var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = sSQL,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter { Name="MyGroup", Value=formCQC.sMyGroup },
                    new SfcParameter { Name="Qa_no", Value=formCQC.combLot.Text }
                }
                });
                combNextGroup.IsEnabled = true;
                combNextGroup.Items.Clear();
                combNextGroup.Items.Add("");
                if(result.Data.Count() != 0)
                {
                    foreach (GroupName items in result.Data.ToListObject<GroupName>().ToList())
                    {
                        combNextGroup.Items.Add(items.GROUP_NEXT);
                    }
                }
                combNextGroup.SelectedIndex = 0;
            }
            else
            {
                lablNextGroup.IsEnabled = false;
                combNextGroup.IsEnabled = false;
            }
            meno1.Focus();
        }

        private void bbtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
