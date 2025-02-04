using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.NetworkInformation;
using Sfc.Library.HttpClient;
using Make_Weight.DataObject;
using System.IO.Ports;
using System.Windows.Threading;
using LabelManager2;
using System.IO;
using Make_Weight.Resource;

namespace Make_Weight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Show_Params : Window
    {
        public static Show_Params prWD { get; private set; }

        public DataTable dt;
        public MainWindow MyForm;

        public Show_Params(DataTable inDt, MainWindow form)
        {
            InitializeComponent();
            dt = inDt;
            this.MyForm = form;
            Loaded += Show_Params_Load;
            this.dtgShowparams.AutoGeneratingColumn += dtgShowparams_AutoGeneratingColumn;
        }       

        private void Show_Params_Closed(object sender, EventArgs e)
        {
            prWD.Close();
        }

        private void dtgShowparams_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ExtensionData")
            {
                e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }
        public void Show_Params_Load(object sender, EventArgs e)
        {
            try
            {
                    List<Params> lstPr = new List<Params>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Params pr = new Params();
                        pr.name = dt.Rows[i]["name"].ToString();
                        pr.value = dt.Rows[i]["value"].ToString();
                        lstPr.Add(pr);
                    }
                    dtgShowparams.ItemsSource = lstPr;
                    prWD = this;
                    prWD.Show();
                    MainWindow.isVisible = "N";
                
            }
            catch (Exception ex)
            {
                this.MyForm.txtError.Text = ex.Message.ToString();
                MainWindow.isVisible = "N";
                prWD.Close();
            }

        }
    }
}
