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
using PACK_PALT_NEW.Model;

namespace PACK_PALT_NEW
{
    /// <summary>
    /// Interaction logic for Cut_BarcodeForm.xaml
    /// </summary>
    public partial class Cut_BarcodeForm : Window
    {
        private MainWindow _mainprogram;
        public static SfcHttpClient sfcHttpClient;
        INIFile ini = new INIFile("C:\\Windows\\SFIS.ini");
        public static string SN;
        public static string SNto;

        public Cut_BarcodeForm(MainWindow mainprogram, SfcHttpClient _sfcHttpClient)
        {
            InitializeComponent();
            this._mainprogram = mainprogram;
            sfcHttpClient = _sfcHttpClient;
            FormShow();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSN.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (string.IsNullOrEmpty(txtSnto.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (txtSN.Text != null)
            {
                ini.Write("PACK3", "SNEXAMPLE", txtexample.Text);
                ini.Write("PACK3", "SN", txtSN.Text);
                ini.Write("PACK3", "SNTO", txtSnto.Text);

                SN = txtSN.Text;
                SNto = txtSnto.Text;

                //
                EditSntoend.Text = MainWindow.getInputdataWithINI(txtexample.Text, "SN");
            }
        }

        public void FormShow()
        {
            INIFile ini = new INIFile("C:\\Windows\\SFIS.ini");
            txtexample.Text = ini.Read("PACK3", "SNEXAMPLE");
            txtSN.Text = ini.Read("PACK3", "SN");
            txtSnto.Text = ini.Read("PACK3", "SNTO");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            txtexample.Text = "";
            txtSN.Text = "";
            txtSnto.Text = "";
            EditSntoend.Text = "";
        }
    }
}
