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
using PM.Model;
using System.Collections.ObjectModel;
using System.Collections;

namespace PM
{
    /// <summary>
    /// Interaction logic for CheckBOMForm.xaml
    /// </summary>
    public partial class CheckBOMForm : Window
    {
        public MO_InsertForm frm_MO_InsertForm;
        public SfcHttpClient sfcClient;
        public ObservableCollection<ListBOM> _ListBOM { get; set; }
        public static ObservableCollection<ListBOM> ConvertBOM(IEnumerable original)
        {
            return new ObservableCollection<ListBOM>(original.Cast<ListBOM>());
        }
        public CheckBOMForm()
        {
            InitializeComponent();
        }
        public CheckBOMForm(MO_InsertForm _frm_MO_InsertForm, SfcHttpClient _sfcClient)
        {
            frm_MO_InsertForm = _frm_MO_InsertForm;
            sfcClient = _sfcClient;
            InitializeComponent();
        }
        private void CheckBOMForm_Close(object sender, EventArgs e)
        {
            if (frm_MO_InsertForm.Lb_Mess.Content.ToString() == "0")
            {
                MessageBox.Show("Xac nhan lai BOM -- Please check BOM","PM",MessageBoxButton.OK,MessageBoxImage.Question);
            }
        }
        private async void CheckBOMForm_Initialized(object sender, EventArgs e)
        {
            string strGetBOM = "SELECT b.kp_name, a.key_part_no, a.group_name,a.kp_count, a.kp_relation "
                + " FROM sfis1.c_bom_keypart_t a, sfis1.c_keyparts_desc_t b "
                + $" WHERE a.bom_no ='{frm_MO_InsertForm.Cbb_BOM.Text}' AND a.key_part_no = b.key_part_no ORDER BY a.kp_relation";
            var qry_BOM = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetBOM,
                SfcCommandType = SfcCommandType.Text
            });
            _ListBOM = ConvertBOM(qry_BOM.Data.ToListObject<ListBOM>().ToList());
            Lv_BOM.ItemsSource = _ListBOM;
        }
    }
}
