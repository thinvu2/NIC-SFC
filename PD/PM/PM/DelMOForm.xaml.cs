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
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
namespace PM
{
    /// <summary>
    /// Interaction logic for DelMOForm.xaml
    /// </summary>
    public partial class DelMOForm : Window
    {
        public MainWindow main;
        public SfcHttpClient sfcClient;
        public bool AllFlag;
        public DelMOForm()
        {
            InitializeComponent();
        }
        public DelMOForm(MainWindow _main,SfcHttpClient _sfcClient)
        {
            main = _main;
            sfcClient = _sfcClient;
            InitializeComponent();
            DelMOForm_FormShow();
        }
        private async void DelMOForm_FormShow()
        {
            string strGetMO = "SELECT r105.*, crn.route_name"
               + " FROM sfism4.r_mo_base_t r105, sfis1.c_route_name_t crn"
               + " WHERE close_flag LIKE :closeflag"
               + " AND crn.route_code = r105.route_code"
               + " AND close_flag <> '9' ORDER BY mo_number";
            var qry_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetMO,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="CloseFlag",Value="%"}
                }
            });
            Cbb_MO.Items.Clear();
            foreach (var row in qry_MO.Data)
            {
                Cbb_MO.Items.Add(row["mo_number"]);
            }
            if (AllFlag)
            {
                Cbb_MO.Items.Add("ALL");
                Cbb_MO.Text = Cbb_MO.Items[Cbb_MO.Items.IndexOf("ALL")].ToString();
            }
            else
            {
                Cbb_MO.Text = Cbb_MO.Items[0].ToString();
            }
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
