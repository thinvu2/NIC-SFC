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
using System.Deployment.Application;
using Sfc.Library.HttpClient;
using AMSLabel;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Windows.Threading;
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient.Helpers;
using CQC.ViewModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace CQC
{
    /// <summary>
    /// Interaction logic for SetupLineWindow.xaml
    /// </summary>

    public partial class SetupLineWindow : Window
    {
        ListView listbTemp = new ListView();
        INIFile ini = new INIFile("SFIS.ini");
        private SfcHttpClient sfcClient;
        public string sMyGroup;
        MainWindow formCQC;
        public SetupLineWindow()
        {
            InitializeComponent();
        }

        public SetupLineWindow(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            sfcClient = formCQC.sfcClient;
            InitializeComponent();
        }
        
        public async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "Delete SFIS1.C_PARAMETER_INI Where Prg_Name='CQC' AND VR_CLASS=:MyIP and  VR_ITEM='Line_Single'",
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="MyIP" , Value = MainWindow.GetIPAddress().ToString()}
                }
                });

                if (listSelectLine.Items.Count != 0)
                {
                    foreach (string items in listSelectLine.Items)
                    {

                        var sql = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = "insert into SFIS1.C_PARAMETER_INI (prg_name, vr_class, vr_item, vr_name, vr_value) values (:prg_name, :vr_class, :vr_item, :vr_name, :vr_value)",
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter {Name = "prg_name", Value="CQC" },
                            new SfcParameter {Name = "vr_class" , Value=MainWindow.GetIPAddress().ToString()},
                            new SfcParameter {Name = "vr_item", Value="Line_Single" },
                            new SfcParameter {Name = "vr_name", Value=items },
                            new SfcParameter {Name = "vr_value", Value=sMyGroup }
                        }
                        });
                        ini.Write("CQC", "Line Count", listSelectLine.Items.Count.ToString());
                        for (int i = 0; i < listSelectLine.Items.Count - 1; i++)
                        {
                            ini.Write("CQC", "Line" + (i + 1).ToString(), listSelectLine.Items[i].ToString());
                        }
                        formCQC.listSelectLine = listSelectLine;
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Please input Line Name");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnRight_Click(object sender, RoutedEventArgs e)
        {
            if (listLine.SelectedIndex > -1)
            {
                int SelectedIndex = listLine.SelectedIndex;
                string SelectedItem = listLine.Items[SelectedIndex].ToString();
                if (listSelectLine.Items.IndexOf(SelectedItem) == -1)
                {
                    if (listLine != null && await checkLine(SelectedItem) == true)
                    {
                        listSelectLine.Items.Add(SelectedItem);
                        listLine.Items.RemoveAt(SelectedIndex);
                    }
                }
                listLine.Items.Refresh();
                listSelectLine.Items.Refresh();
            }


        }

        public async Task<bool> checkLine(string line)
        {
            var count = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = "select * from  SFIS1.C_PARAMETER_INI where PRG_NAME='CQC' AND VR_ITEM='Line_Single' and VR_NAME='"+line+"' and VR_VALUE='"+formCQC.sMyGroup+"' and VR_CLASS<>'"+ MainWindow.GetIPAddress().ToString() + "'",
                SfcCommandType=SfcCommandType.Text
            });
            if (count.Data.Count() != 0)
            {
                MessageBox.Show("Other PC Using this Line&Group !!");
                return false;
            }
            return true;
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            if (listSelectLine.SelectedIndex > -1)
            {
                int SelectedIndex = listSelectLine.SelectedIndex;
                string SelectedItem = listSelectLine.Items[SelectedIndex].ToString();
                if (listLine.Items.IndexOf(SelectedItem) == -1)
                    listLine.Items.Add(SelectedItem);
                listSelectLine.Items.RemoveAt(SelectedIndex);
                listSelectLine.Items.Refresh();
                listLine.Items.Refresh();
            }
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            listLine.Items.Clear();
            int i, iLine;
            INIFile ini = new INIFile("SFIS.ini");

            if (string.IsNullOrEmpty(ini.Read("CQC", "Line Count")))
                iLine = 0;

            iLine = int.Parse(ini.Read("CQC", "Line Count"));

            for (i = 1; i <= iLine; i++)
            {
                if (!string.IsNullOrEmpty(ini.Read("CQC", "Line" + i.ToString())))
                {
                    listSelectLine.Items.Add(ini.Read("CQC", "Line" + i.ToString()));
                }
            }

            var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT DISTINCT LINE_NAME FROM  SFIS1.C_LINE_DESC_T order by line_name"
            });

            if (result.Data.Count() != 0)
            {
                foreach (LineName items in result.Data.ToListObject<LineName>().ToList())
                    listLine.Items.Add(items.LINE_NAME);
            }
        }
    }
}
