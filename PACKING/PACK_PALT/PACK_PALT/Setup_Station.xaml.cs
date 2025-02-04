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
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using PACK_PALT;
using System.Net;


namespace PACK_PALT
{
    /// <summary>
    /// Interaction logic for Setup_Station.xaml
    /// </summary>
    public partial class Setup_Station : Window
    {
        public List<Item_line> _line_name;
        public static string line;
        public static string section;
        public static string group_name;
        public static string station_name;
        public static string strINIPath = "C:\\Windows\\SFIS.ini";
        private MainWindow _mainprogram;
        public static SfcHttpClient sfcHttpClient;
        public Setup_Station(MainWindow mainprogram, SfcHttpClient _sfcHttpClient)
        {
            InitializeComponent();
            this._mainprogram = mainprogram;
            sfcHttpClient = _sfcHttpClient;
            load_Line();
            load_station();
            
        }
        public async void load_Line()
        {
            try
            {
                var result_infoLine = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = "select distinct line_name from sfis1.c_line_desc_t order by line_name",
                    SfcCommandType = SfcCommandType.Text
                    //SfcParameters = new List<SfcParameter>
                    //    {
                    //        new SfcParameter{ Name = "emp_pass", Value = PasswordBox.Password  }
                    //    }
                });
                if (result_infoLine.Data != null)
                {
                    var a = result_infoLine.Data.ToListObject<Item_line>();
                    List<Item_line> list_show_line = a.Cast<Item_line>().ToList();
                    dynamic ads = result_infoLine.Data;

                    cbb_Line_name.ItemsSource = list_show_line;
                    cbb_Line_name.SelectedValuePath = "line_name";
                    cbb_Line_name.DisplayMemberPath = "line_name";


                    cbb_Line_name.Text = MainWindow.G_sLine_Name;
                    txt_section.Text = MainWindow.M_sThisSection;
                    txt_group_name.Text = MainWindow.M_sThisGroup;
                    txt_station_name.Text = MainWindow.My_Station;
                    //cbb_Line_name.DataContext = result.Data.ToList();
                    // cbb_Line_name.DisplayMemberPath=
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public async void load_station()
        {
            string str_sql = "select section_name, group_name, station_name " +
                           " from sfis1.c_station_config_t " +
                            "where hostid = -1 and SECTION_NAME ='SI' and GROUP_NAME not like 'R_%'" +
                            "group by section_name, group_name, station_name " +
                           " order by section_name, group_name, station_name";
            try
            {
                var result_station = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = str_sql,
                    SfcCommandType = SfcCommandType.Text

                });
                var a_station = result_station.Data.ToListObject<Item_station>();
                List<Item_station> list_station = a_station.Cast<Item_station>().ToList();
                dgr_group.ItemsSource = list_station;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        } 
        public class Item_line
        {
            public string line_name { get; set; }
        }
        public class Item_station
        {
            public string section_name { get; set; }
            public string group_name { get; set; }
            public string station_name { get; set; }
        }
        

        private void Cancel_station_Click(object sender, RoutedEventArgs e)
        {
            this._mainprogram.Input_Carton.IsEnabled = true;
            this.Close();
        }

        private void ok_station_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(cbb_Line_name.Text) && !string.IsNullOrEmpty(txt_section.Text) && !string.IsNullOrEmpty(txt_group_name.Text) && !string.IsNullOrEmpty(txt_station_name.Text))
            {
                line = cbb_Line_name.Text.Trim();
                section = txt_section.Text.Trim();
                group_name = txt_group_name.Text.Trim();
                station_name = txt_station_name.Text.Trim();

                this._mainprogram.txt_name_station_and_line.Text = group_name + " : " + line;
                MainWindow.G_sLine_Name = line;
                MainWindow.M_sThisSection = section;
                MainWindow.M_sThisGroup = group_name;
                MainWindow.My_Station = station_name;
                writeINI("LINE", line);
                //Insert_parameter_ini();
                this._mainprogram.Input_Carton.IsEnabled = true;
                this._mainprogram.Input_Carton.Focus();
                this.Close();
            }
            else
            {
                MessageBox.Show("Line name or Group name is null. please Choose Line and group name", "WARRING", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            
        }
        private void writeINI(string key_name,string  key_value)
        {
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK3", key_name, key_value);
        }
        private void Insert_parameter_ini()
        {
            string str_sql = "INSERT INTO SFIS1.C_PARAMETER_INI (PRG_NAME,VR_CLASS,VR_ITEM,VR_NAME,VR_VALUE,VR_DESC) "
                            + "   VALUES('"+ MainWindow.G_sPrgName + "', '" + this._mainprogram.local_strIP.Trim()+"', '"+ section + "', '"+ group_name + "', '"+ station_name + "', '"+ line + "') ";

            var sbUpdate = new StringBuilder();
            sbUpdate.Append(str_sql);
            try
            {
                var resultExe = sfcHttpClient.Execute(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()
                });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void setup_station_Closed(object sender, EventArgs e)
        {
            this._mainprogram.Input_Carton.IsEnabled = true;
            this.Close();
        }
    }
}
