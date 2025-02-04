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
using PACK_BOX.Model;

namespace PACK_BOX
{
    /// <summary>
    /// Interaction logic for Station_Setup.xaml
    /// </summary>        

    public partial class Station_Setup : Window
    {
        public ObservableCollection<LineName> LineNameList { get; set; }
        SfcHttpClient sfcClient;
        INIFile iniPackbox = new INIFile("SFIS.ini");
        public MainWindow formPackBox;
        public Station_Setup()
        {
            InitializeComponent();
        }
        public Station_Setup(MainWindow main, SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            sfcClient = _sfcClient;
            this.formPackBox = main;
            LoadMain();
            LoadGroup();
        }
        public static ObservableCollection<LineName> Convert(IEnumerable original)
        {
            return new ObservableCollection<LineName>(original.Cast<LineName>());
        }
        private void LoadMain()
        {
            EditGroup.Text = this.formPackBox.M_sThisGroup;
            EditSection.Text = this.formPackBox.M_sThisSection;
            EditStation.Text = this.formPackBox.M_sThisStation;
            EditLine.Text = this.formPackBox.M_sThisLine;
        }
        public async void LoadGroup()
        {
            string strGetGroup = "select section_name, group_name, station_name, line_name from sfis1.c_station_config_t where "
                + " substr(group_name,1,8)='PACK_BOX' group by section_name, group_name, station_name, line_name order by line_name";
            var qry_Group = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetGroup,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Group.Data.Count() != 0)
            {
                LineNameList = Convert(qry_Group.Data.ToListObject<LineName>());
            }
            DataGrid.ItemsSource = LineNameList;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EditGroup.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (string.IsNullOrEmpty(EditStation.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (string.IsNullOrEmpty(EditSection.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (string.IsNullOrEmpty(EditLine.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (EditGroup.Text != null)
            {
                this.formPackBox.M_sThisGroup = EditGroup.Text;
                this.formPackBox.M_sThisSection = EditSection.Text;
                this.formPackBox.M_sThisStation = EditStation.Text;
                this.formPackBox.M_sThisLine = EditLine.Text;

                iniPackbox.Write("PrePacking", "Group", EditGroup.Text);
                iniPackbox.Write("PrePacking", "Section", EditSection.Text);
                iniPackbox.Write("PrePacking", "Station", EditStation.Text);
                iniPackbox.Write("PrePacking", "Line", EditLine.Text);
                this.formPackBox.InputData.IsEnabled = true;
                this.formPackBox.InputData.Focus();
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
                var setline = DataGrid.SelectedItem as LineName;

                EditSection.Text = setline.SECTION_NAME;
                EditGroup.Text = setline.GROUP_NAME;
                EditStation.Text = setline.STATION_NAME;
                EditLine.Text = setline.LINE_NAME;
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            this.formPackBox.InputData.IsEnabled = true;
            this.Close();
        }
    }
}
