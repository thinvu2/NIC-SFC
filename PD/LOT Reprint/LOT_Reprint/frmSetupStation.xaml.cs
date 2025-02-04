using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using Sfc.Library.HttpClient.Helpers;
using Newtonsoft.Json;

namespace LOT_REPRINT
{
    /// <summary>
    /// Interaction logic for frmSetupStation.xaml
    /// </summary>
    public partial class frmSetupStation : Window
    {
        public SfcHttpClient sfcClient;
        public System.Windows.Forms.DialogResult ok;
        public string sql = string.Empty, My_Section, M_Station, M_Group, M_Line;
        public bool AutomationChecked=true;
        MainWindow main;
        public frmSetupStation()
        {
            InitializeComponent();
        }
        public frmSetupStation(MainWindow _main)
        {
            main = _main;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (cbb_Line_name.SelectedValue is null)
            {
                showMessage("Please select line name!", "Vui lòng chọn line!", true);
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(txt_station_name.Text) || !string.IsNullOrEmpty(txt_group_name.Text) || !string.IsNullOrEmpty(txt_section.Text))
                {
                    SetStation();
                }
                else
                {
                    showMessage("Please click checkbox the grid to select station!", "Vui lòng nhấp vào hộp kiểm lưới để chọn trạm!", true);
                    return;
                }
            }
        }
        private async void SetStation()
        {
            sql = "select * from sfis1.c_station_config_t where station_name = '" + txt_station_name.Text + "' and group_name = '" + txt_group_name.Text + "' and section_name = '" + txt_section.Text + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                SetParameter();
            }
            else
            {
                txt_station_name.Text = "";
            }
        }
        private async void SetParameter()
        {
            if (AutomationChecked)
            {
                if (await FindIP())
                {
                    sql = "update sfis1.c_parameter_ini set vr_item = '" + txt_section.Text + "', vr_value = '" + txt_station_name.Text + "', vr_name = '" + txt_group_name.Text + "',vr_desc='" + cbb_Line_name.Text + "' " +
                        " where prg_name = 'LOT_Reprint' and vr_class = '" + GetIPAddress() + "'";
                    var excute = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                }
                else
                {
                    sql = "insert into sfis1.c_parameter_ini(prg_name, vr_class, vr_item, vr_name, vr_value,vr_desc) " +
                        " values('LOT_Reprint','" + GetIPAddress() + "','" + txt_section.Text + "','" + txt_group_name.Text + "', '" + txt_station_name.Text + "','" + cbb_Line_name.Text + "')";
                    var excute = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                }
            }
            else
            {
                M_Line = cbb_Line_name.Text;
                M_Group = txt_group_name.Text;
                M_Station = txt_station_name.Text;
                My_Section = txt_section.Text;

                main.M_Group = M_Group;
                ok = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }
        private async Task<bool> FindIP()
        {
            sql = "select * from sfis1.c_parameter_ini where prg_name = 'LOT_Reprint' and vr_class = '" + GetIPAddress() + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
                return true;
            return false;
        }
        public static IPAddress GetIPAddress()
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses("");

            foreach (IPAddress hostAddress in hostAddresses)
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(hostAddress) &&
                    !hostAddress.ToString().StartsWith("169.254."))
                    return hostAddress;
            }
            return null;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sql = "select * from sfis1.c_parameter_ini where prg_name = 'LOT_Reprint' and VR_CLASS = '" + GetIPAddress() + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                My_Section = _result.Data["vr_item"]?.ToString() ?? "";
                M_Group = _result.Data["vr_name"]?.ToString() ?? "";
                M_Station = _result.Data["vr_value"]?.ToString() ?? "";
                M_Line = _result.Data["vr_desc"]?.ToString() ?? "";
                main.M_Group = M_Group;
            }

            sql = "select section_name, group_name,group_name station_name from SFIS1.C_GROUP_CONFIG_T where GROUP_NAME LIKE 'PRINT%' or GROUP_NAME LIKE'LABEL%' ORDER BY GROUP_NAME ";
            var result_station = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            var a_station = result_station.Data.ToListObject<Item_station>();
            List<Item_station> list_station = a_station.Cast<Item_station>().ToList();
            dgr_group.ItemsSource = list_station;
            SetStation();
            load_Line();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ok = System.Windows.Forms.DialogResult.None;
            this.Close();
        }

        public class Item_station
        {
            public string section_name { get; set; }
            public string group_name { get; set; }
            public string station_name { get; set; }
        }
        public class Item_line
        {
            public string line_name { get; set; }
        }
        private void showMessage(string MessageEnglish, string MessageVietNam, bool CustomFlag)
        {
            frmMessage frmMessage = new frmMessage();
            frmMessage.sfcClient = sfcClient;
            if (CustomFlag)
            {
                frmMessage.MessageEnglish = MessageEnglish;
                frmMessage.MessageVietNam = MessageVietNam;
                frmMessage.CustomFlag = CustomFlag;
            }
            else
            {
                frmMessage.errorcode = MessageEnglish;
                frmMessage.CustomFlag = CustomFlag;
            }
            frmMessage.ShowDialog();
        }
        private void showMessage(string content)
        {
            string MessageEnglish = "", MessageVietNam = "";
            if (content.IndexOf("|") > 0)
            {
                MessageEnglish = content.Split('|')[0].ToString().Trim();
                MessageVietNam = content.Split('|')[1].ToString().Trim();
            }
            else
            {
                MessageEnglish = content;
                MessageVietNam = content;
            }
            frmMessage frmMessage = new frmMessage();
            frmMessage.sfcClient = sfcClient;
            frmMessage.MessageEnglish = MessageEnglish;
            frmMessage.MessageVietNam = MessageVietNam;
            frmMessage.CustomFlag = true;
            frmMessage.ShowDialog();
        }
        public async void load_Line()
        {
            try
            {
                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {

                    CommandText = "SFIS1.SP_LotReprint",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="AP_VER",Value=MainWindow.prgVer,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="MYGROUP",Value="N/A",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_DATA",Value="N/A",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_FUNC",Value="GetLineName",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
                });
                dynamic _ads = result.Data;
                string _RES = _ads[1]["res"];
                string _output = _ads[0]["out_data"];
                if (_RES.StartsWith("OK"))
                {
                    List<Item_line> EnumerableLine = new List<Item_line>();
                    var list = _output.Split('|');
                    for(int i=0;i<list.Count();i++)
                    {
                        Item_line items = new Item_line();
                        items.line_name = list[i];
                        EnumerableLine.Add(items);
                    }
                    cbb_Line_name.ItemsSource = EnumerableLine;
                    cbb_Line_name.SelectedValuePath = "line_name";
                    cbb_Line_name.DisplayMemberPath = "line_name";
                }
                else
                {
                    showMessage(_RES);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
