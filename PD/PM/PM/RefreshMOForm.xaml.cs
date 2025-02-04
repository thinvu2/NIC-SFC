using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PM
{
    /// <summary>
    /// Interaction logic for RefreshMOForm.xaml
    /// </summary>
    public partial class RefreshMOForm : Window
    {
        public MO_ManageForm frm_MO_ManageForm;
        public SfcHttpClient sfcClient;
        public ObservableCollection<ListDataWIP> _ListDataWIP { get; set; }
        public ObservableCollection<ListMO> _ListDataMO { get; set; }
        public static ObservableCollection<ListDataWIP> Convert_WIP(IEnumerable original)
        {
            return new ObservableCollection<ListDataWIP>(original.Cast<ListDataWIP>());
        }
        public static ObservableCollection<ListMO> Convert_MO(IEnumerable original)
        {
            return new ObservableCollection<ListMO>(original.Cast<ListMO>());
        }
        public RefreshMOForm()
        {
            InitializeComponent();
        }
        public RefreshMOForm(MO_ManageForm _frm_MO_ManageForm, SfcHttpClient _sfcClient)
        {
            frm_MO_ManageForm = _frm_MO_ManageForm;
            sfcClient = _sfcClient;
            InitializeComponent();
            Edt_MO.Focus();
        }
        private async void MO_Refresh()
        {
            string MO_Count, Null_Count, MO_InputQtyR107;
            int Input_Count, MO_Target;
            Edt_MO.Text = Edt_MO.Text.ToUpper();
            Input_Count = 0;
            MO_Target = 0;
            if (string.IsNullOrEmpty(Edt_MO.Text))
            {
                MessageBox.Show("Please input MO number", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Edt_MO.Focus();
                Edt_MO.SelectAll();
                return;
            }
            if (Edt_MO.Text.Length != 10)
            {
                MessageBox.Show("Length MO wrong, Please input again!", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Edt_MO.Focus();
                Edt_MO.SelectAll();
                return;
            }
            string strGetRefreshMO = $"select * from sfism4.r_wip_tracking_t where mo_number= '{Edt_MO.Text}'";
            var qry_RefreshMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetRefreshMO,
                SfcCommandType = SfcCommandType.Text
            });
            _ListDataWIP = Convert_WIP(qry_RefreshMO.Data.ToListObject<ListDataWIP>().ToList());
            Data_WIP.ItemsSource = _ListDataWIP;
            MO_Count = qry_RefreshMO.Data.Count().ToString();
            Lb_Count.Content = "RECORD : " + MO_Count;

            string strGetDataMO = $"select * from sfism4.r_wip_tracking_t where mo_number= '{Edt_MO.Text}' and group_name='ASSY' and wip_group is null";
            var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataMO,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_DataMO.Data.Count() <= 0)
            {
                MessageBox.Show($"Not found data MO '{Edt_MO.Text}' have WIP_GROUP is NULL, or MO not exist. Cannot Refresh!", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Edt_MO.Focus();
                Edt_MO.SelectAll();
                return;
            }
            else
            {
                Null_Count = qry_DataMO.Data.Count().ToString();
                string strGetMO = $"select * from sfism4.r_mo_base_t where mo_number='{Edt_MO.Text}'";
                var qry_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetMO,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_MO.Data.Count() <= 0)
                {
                    MessageBox.Show($"Mo '{Edt_MO.Text}' not exist!", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    Edt_MO.Focus();
                    Edt_MO.SelectAll();
                    return;
                }
                _ListDataMO = Convert_MO(qry_MO.Data.ToListObject<ListMO>().ToList());
                Data_MO.ItemsSource = _ListDataMO;
                dynamic ads = qry_MO.Data;
                Input_Count = Convert.ToInt32(ads[0]["input_qty"]);
                MO_Target = Convert.ToInt32(ads[0]["target_qty"]);
                if(Int32.Parse(MO_Count)  < MO_Target)
                {
                    MessageBox.Show($"Current quantity MO '{Edt_MO.Text}' < TARGET_QTY. Cannot Refresh!", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    Edt_MO.Focus();
                    Edt_MO.SelectAll();
                    return;
                }
                try
                {
                    string strDelete = $"delete sfism4.r_wip_tracking_t where mo_number= '{Edt_MO.Text}' and group_name='ASSY' and wip_group is null";
                    var Delete = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strDelete,
                        SfcCommandType = SfcCommandType.Text
                    });
                    string strGetInputQty = $"select * from sfism4.r_wip_tracking_t where mo_number= '{Edt_MO.Text}'";
                    var qry_InputQty = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetInputQty,
                        SfcCommandType = SfcCommandType.Text
                    });
                    MO_InputQtyR107 = qry_InputQty.Data.Count().ToString();

                    string strUpdate = $"update sfism4.r_mo_base_t set input_qty='{MO_InputQtyR107}' where mo_number= '{Edt_MO.Text}'";
                    var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strUpdate,
                        SfcCommandType = SfcCommandType.Text
                    });

                    //------ Save To Log ------//
                    string strInsert_Log = "INSERT INTO sfism4.r_system_log_t(EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC) "
                            + " VALUES ("
                            + " :EMP_NO, :PRG_NAME, :ACTION_TYPE, :ACTION_DESC)";
                    var Insert_Log = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strInsert_Log,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="EMP_NO",Value=frm_MO_ManageForm._EMP},
                            new SfcParameter{Name="PRG_NAME",Value="PM"},
                            new SfcParameter{Name="ACTION_TYPE",Value="Refresh MO"},
                            new SfcParameter{Name="ACTION_DESC",Value= $"MO : {Edt_MO.Text}"}
                        }
                    });
                    MessageBox.Show($"Refresh MO success full", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    Edt_MO.Focus();
                    Edt_MO.SelectAll();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }
        private void MO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MO_Refresh();
            }
        }
        private void btn_RefreshMO(object sender, RoutedEventArgs e)
        {
            MO_Refresh();
        }
    }
}
