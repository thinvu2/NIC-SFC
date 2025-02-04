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
    /// Interaction logic for frmGroupWindow.xaml
    /// </summary>
    public partial class frmGroupWindow : Window
    {
        SfcHttpClient sfcClient;
        INIFile ini = new INIFile("SFIS.ini");

        public frmGroupWindow(SfcHttpClient _sfcClient, string _group)
        {
            combGroup.Text = _group;
            sfcClient = _sfcClient;
            InitializeComponent();
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {   
            combGroup.Items.Clear();
            var quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select distinct group_name from SFIS1.C_GROUP_CONFIG_T",
                SfcCommandType=SfcCommandType.Text
            });
            if (quryTemp.Data != null)
            {
                List<STATIONCONFIG> grNamelist = new List<STATIONCONFIG>();
                grNamelist = quryTemp.Data.ToListObject<STATIONCONFIG>().ToList();
                combGroup.ItemsSource = grNamelist;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ini.Write("CQC", "CQC Prior Group", combGroup.Text);
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
}
