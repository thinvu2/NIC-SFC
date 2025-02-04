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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sfc.Library.HttpClient;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Windows.Threading;
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient.Helpers;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using PACKINGBOXID_CFG.Model;

namespace PACKINGBOXID_CFG
{
    /// <summary>
    /// Interaction logic for Station_Setup.xaml
    /// </summary>        

    public partial class FormStationSetup : Window
    {
        public ObservableCollection<StationName> StationName { get; set; }
        public ObservableCollection<LineName> LineName { get; set; }

        SfcHttpClient sfcClient;
        INIFile iniPackingboxid_cfg = new INIFile("C:\\PACKING\\Packing.ini");
        public MainWindow formPackBox_Cfg;
        public bool scb_change_line;
        public FormStationSetup()
        {
            InitializeComponent();
        }
        public FormStationSetup(MainWindow main, SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            sfcClient = _sfcClient;
            this.formPackBox_Cfg = main;
            Load_main();
            load_line();
            load_station();
        }
        public static ObservableCollection<StationName> Convert_Station(IEnumerable original)
        {
            return new ObservableCollection<StationName>(original.Cast<StationName>());
        }
        public static ObservableCollection<LineName> Convert_LineName(IEnumerable original)
        {
            return new ObservableCollection<LineName>(original.Cast<LineName>());
        }

        private void Load_main()
        {
            Edt_Group.Text = this.formPackBox_Cfg.M_sThisGroup;
            Edt_Section.Text = this.formPackBox_Cfg.M_sThisSection;
            Edt_Station.Text = this.formPackBox_Cfg.M_sThisStation;
            cbb_line_name.Text = this.formPackBox_Cfg.M_sThisLineName;
            scb_change_line = bool.Parse(iniPackingboxid_cfg.Read("PACKY", "IsChangeLine"));
            cb_change_line.IsChecked = scb_change_line;
            cbb_line_name.IsEnabled = scb_change_line;
        }
        private async void load_line()
        {
            try
            {
                string strGetLineName = "select distinct line_name from sfis1.c_line_desc_t order by line_name";
                var qry_LineName = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetLineName,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_LineName.Data.Count() > 0)
                {
                    LineName = Convert_LineName(qry_LineName.Data.ToListObject<LineName>());
                }
                cbb_line_name.ItemsSource = LineName;
                cbb_line_name.DisplayMemberPath = "LINE_NAME";
            }
            catch (Exception e)
            {
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = e.Message;
                _er.MessageEnglish = e.Message;
                _er.ShowDialog();
                return;
            }
        }
        private async void load_station()
        {
            var qry_GroupName = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select section_name, group_name, station_name from sfis1.c_station_config_t where hostid = -1 " +
                "and substr(group_name,1,2)<>'R_' and GROUP_NAME like 'PACK%' group by section_name, group_name, station_name ",
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_GroupName.Data.Count() > 0)
            {
                StationName = Convert_Station(qry_GroupName.Data.ToListObject<StationName>());
            }
            DataGrid.ItemsSource = StationName;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Edt_Group.Text))
            {
                MessageBox.Show("Please input the key value", "Message", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }
            else if (string.IsNullOrEmpty(Edt_Station.Text))
            {
                MessageBox.Show("Please input the key value", "Message", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }
            else if (string.IsNullOrEmpty(Edt_Section.Text))
            {
                MessageBox.Show("Please input the key value", "Message", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }
            else if (string.IsNullOrEmpty(cbb_line_name.Text))
            {
                MessageBox.Show("Please input the line value", "Message", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }
            else if (Edt_Group.Text != null)
            {
                this.formPackBox_Cfg.M_sThisGroup = Edt_Group.Text;
                this.formPackBox_Cfg.M_sThisSection = Edt_Section.Text;
                this.formPackBox_Cfg.M_sThisStation = Edt_Station.Text;
                this.formPackBox_Cfg.M_sThisLineName = cbb_line_name.Text;
                iniPackingboxid_cfg.Write("PrePacking", "Group", Edt_Group.Text);
                iniPackingboxid_cfg.Write("PrePacking", "Section", Edt_Section.Text);
                iniPackingboxid_cfg.Write("PrePacking", "Station", Edt_Station.Text);
                iniPackingboxid_cfg.Write("PACKY", "LINE", cbb_line_name.Text);
                iniPackingboxid_cfg.Write("PACKY", "IsChangeLine", cb_change_line.IsChecked.ToString());
                this.formPackBox_Cfg.Inputdata.IsEnabled = true;
                this.formPackBox_Cfg.Inputdata.Focus();
                this.Close();
            }
        }
        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Double_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedItem == null)
            {
                return;
            }
            else
            {
                var setline = DataGrid.SelectedItem as StationName;

                Edt_Section.Text = setline.SECTION_NAME;
                Edt_Group.Text = setline.GROUP_NAME;
                Edt_Station.Text = setline.STATION_NAME;
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            this.formPackBox_Cfg.Inputdata.IsEnabled = true;
            this.Close();
        }
        private void cb_change_line_Click(object sender, RoutedEventArgs e)
        {
            cbb_line_name.IsEnabled = (bool)cb_change_line.IsChecked;
        }
    }
}
