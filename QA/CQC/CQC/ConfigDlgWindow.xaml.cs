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
using System.Collections.ObjectModel;
using System.Collections;

namespace CQC
{
    /// <summary>
    /// Interaction logic for ConfigDlgWindow.xaml
    /// </summary>
    public partial class ConfigDlgWindow : Window
    {
        SfcHttpClient sfcClient;
        INIFile iniCqc = new INIFile("SFIS.ini");
        private MainWindow formCQC;
        public ConfigDlgWindow()
        {
            InitializeComponent();
        }
        public ConfigDlgWindow(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            sfcClient = _formCQC.sfcClient;
            InitializeComponent();
        }

        private async void bbtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EditGroup.Text))
            {
                MessageBox.Show("Please input the key value","CQC");
                return;
            }
            else if (string.IsNullOrEmpty(EditStation.Text))
            {
                MessageBox.Show("Please input the key value", "CQC");
                return;
            }
            else if (string.IsNullOrEmpty(EditSection.Text))
            {
                MessageBox.Show("Please input the key value", "CQC");
                return;
            }
            else if (string.IsNullOrEmpty(combNext.Text))
            {
                MessageBox.Show("Please input the key value", "CQC");
                combNext.Focus();
            }
            else if (EditSection.Text == formCQC.sMySection && EditStation.Text == formCQC.sMyStation && EditGroup.Text == formCQC.sMyGroup)
            {
                this.Close();
            }
            else
            {
                if (formCQC.bLogin)
                {
                    if (await formCQC.CheckPrivilege(formCQC.G_sTester, EditGroup.Text) != true)
                    {
                        MessageBox.Show("No Privilege to use it ","CQC");
                        return;
                    }
                }

                //check Group if exist

                //if Line ALL
                if (formCQC.itemLineAll.IsChecked)
                {
                    var quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "select * from SFIS1.C_PARAMETER_INI where PRG_NAME='CQC' and VR_ITEM='Line_All' and VR_VALUE= '" + EditGroup.Text + "'",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (quryTemp.Data.Count() > 0)
                    {
                        MessageBox.Show("Other(A) Line Using this Group !!","CQC");
                        return;
                    }
                }

                if (formCQC.itemLineSingle.IsChecked)
                {
                    var query = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText= "select * from SFIS1.C_PARAMETER_INI where PRG_NAME='CQC' and VR_ITEM='Line_Single' and VR_VALUE='"+EditGroup.Text+"' and VR_NAME= '"+formCQC.sIinLine + "'",
                        SfcCommandType=SfcCommandType.Text
                    });
                    if (query.Data.Count() > 0)
                    {
                        MessageBox.Show("Other(S) Line Using this Group !!","CQC");
                        return;
                    }
                }

                var update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText= "Update SFIS1.C_PARAMETER_INI set VR_VALUE='"+EditGroup.Text+"' where PRG_NAME='CQC' and VR_CLASS='"+MainWindow.GetIPAddress()+"'",
                    SfcCommandType=SfcCommandType.Text
                });

                iniCqc.Write("CQC", "Station", EditStation.Text);
                iniCqc.Write("CQC", "Section", EditSection.Text);
                iniCqc.Write("CQC", "Group", EditGroup.Text);
                iniCqc.Write("CQC", "NextGroup", combNext.Text);

                formCQC.sMySection = iniCqc.Read("CQC", "Section");
                formCQC.sMyGroup = iniCqc.Read("CQC", "Group");
                formCQC.sMyStation = iniCqc.Read("CQC", "Station");
                formCQC.PanelTitle.Content = formCQC.sIinLine + " " + formCQC.sMyGroup;
                this.Close();

            }
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            dbgrid1.ItemsSource = null;
            EditGroup.Text = iniCqc.Read("CQC", "Group");
            EditSection.Text = iniCqc.Read("CQC", "Section");
            EditStation.Text = iniCqc.Read("CQC", "Station");
            string sCheckG = iniCqc.Read("CQC", "NextGroup");

            var qurystation = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select section_name, group_name, station_name from sfis1.c_station_config_t where hostid = -1 group by section_name, group_name, station_name",
                SfcCommandType = SfcCommandType.Text
            });
            if (qurystation.Data.Count() > 0)
            {
                dbgrid1.ItemsSource = qurystation.Data.ToListObject<STATIONCONFIG>().ToList();
            }
            combNext.Items.Add("N/A");
            var quryGroup = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select group_name from sfis1.c_group_config_t  group by group_name",
                SfcCommandType = SfcCommandType.Text
            });
            if (quryGroup.Data.Count() != 0)
            {
                foreach (R107 items in quryGroup.Data.ToListObject<R107>().ToList())
                    combNext.Items.Add(items.GROUP_NAME);
            }
            if (string.IsNullOrEmpty(sCheckG))
                sCheckG = "N/A";
            combNext.SelectedIndex = combNext.Items.IndexOf(sCheckG);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(formCQC.sMyGroup))
                this.Close();
            else
            {
                MessageBox.Show("Please input Section , Group , Station Name!!","CQC");
                return;
            }
        }

        private void dbgrid1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            List<STATIONCONFIG> datalist= dbgrid1.ItemsSource as List<STATIONCONFIG>;
            int index = dbgrid1.SelectedIndex;
            EditGroup.Text = datalist[index].GROUP_NAME;
            EditStation.Text = datalist[index].STATION_NAME;
            EditSection.Text = datalist[index].SECTION_NAME;
        }
    }
}
