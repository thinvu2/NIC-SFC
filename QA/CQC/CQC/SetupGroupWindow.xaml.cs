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
    /// Interaction logic for SetupGroupWindow.xaml
    /// </summary>
    public partial class SetupGroupWindow : Window
    {
        private SfcHttpClient sfcClient;
        INIFile ini = new INIFile("SFIS.ini");
        private TransferWindow formTransfer;
        public SetupGroupWindow(TransferWindow _formTransfer, SfcHttpClient _sfcClient)
        {
            formTransfer = _formTransfer;
            sfcClient = _sfcClient;
            InitializeComponent();
        }

        private void bbtnOK_Click(object sender, RoutedEventArgs e)
        {
            formTransfer.combPriorGroup = combPriorGroup.Text;
            formTransfer.WriteIniFile();
            this.Close();
        }

        private void bbtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            combPriorGroup.Text = ini.Read("CQC", "Transer Prior Group");
            string sSQL = " select distinct group_name from SFIS1.C_GROUP_CONFIG_T order by group_name ";
            var sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            combPriorGroup.Items.Clear();

            foreach (GROUP items in sql.Data.ToListObject<GROUP>().ToList())
                combPriorGroup.Items.Add(items.GROUP_NAME);
            if(combPriorGroup.Text == "")
            {
                combPriorGroup.SelectedIndex = combPriorGroup.Items.IndexOf("PK");
            }
        }
    }
}
