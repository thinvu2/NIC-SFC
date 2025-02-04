using PACK_BOX.Model;
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

namespace PACK_BOX
{
    /// <summary>
    /// Interaction logic for SetSN_Location.xaml
    /// </summary>
    public partial class SetSN_Location : Window
    {
        SfcHttpClient sfcClient;
        public MainWindow formPackBox;
        INIFile ini = new INIFile("C:\\Windows\\SFIS.ini");
        public SetSN_Location()
        {
            InitializeComponent();
        }
        public SetSN_Location(MainWindow main, SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            sfcClient = _sfcClient;
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
            if (string.IsNullOrEmpty(txtSsn1.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (string.IsNullOrEmpty(txtSsn1to.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            if (string.IsNullOrEmpty(txtSsn2.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (string.IsNullOrEmpty(txtSsn2to.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            if (string.IsNullOrEmpty(txtSsn3to.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (string.IsNullOrEmpty(txtMac1.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            if (string.IsNullOrEmpty(txtMac1to.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (string.IsNullOrEmpty(txtMac2.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            if (string.IsNullOrEmpty(txtMac2to.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (string.IsNullOrEmpty(txtMac3.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            if (string.IsNullOrEmpty(txtMac3to.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (string.IsNullOrEmpty(txtMac4to.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            if (string.IsNullOrEmpty(txtMac5.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (string.IsNullOrEmpty(txtMac5to.Text))
            {
                MessageBox.Show("Please input the key value");
                return;
            }
            else if (txtSN.Text != null)
            {
                ini.Write("PrePacking", "SNEXAMPLE", txtexample.Text);
                ini.Write("PrePacking", "SN", txtSN.Text);
                ini.Write("PrePacking", "SNTO", txtSnto.Text);
                ini.Write("PrePacking", "SSN1", txtSsn1.Text);
                ini.Write("PrePacking", "SSN1TO", txtSsn1to.Text);
                ini.Write("PrePacking", "SSN2", txtSsn2.Text);
                ini.Write("PrePacking", "SSN2TO", txtSsn2to.Text);
                ini.Write("PrePacking", "SSN3", txtSsn3.Text);
                ini.Write("PrePacking", "SSN3TO", txtSsn3to.Text);
                ini.Write("PrePacking", "MAC1", txtMac1.Text);
                ini.Write("PrePacking", "MAC1TO", txtMac1to.Text);
                ini.Write("PrePacking", "MAC2", txtMac2.Text);
                ini.Write("PrePacking", "MAC2TO", txtMac2to.Text);
                ini.Write("PrePacking", "MAC3", txtMac3.Text);
                ini.Write("PrePacking", "MAC3TO", txtMac3to.Text);
                ini.Write("PrePacking", "MAC4", txtMac4.Text);
                ini.Write("PrePacking", "MAC4TO", txtMac4to.Text);
                ini.Write("PrePacking", "MAC5", txtMac5.Text);
                ini.Write("PrePacking", "MAC5TO", txtMac5to.Text);
                //

                EditSntoend.Text = MainWindow.getInputdataWithINI(txtexample.Text, "SN");
                EditSsn1toend.Text = MainWindow.getInputdataWithINI(txtexample.Text, "SSN1");
                EditSsn2toend.Text = MainWindow.getInputdataWithINI(txtexample.Text, "SSN2");
                EditSsn3toend.Text = MainWindow.getInputdataWithINI(txtexample.Text, "SSN3");
                EditMac1toend.Text = MainWindow.getInputdataWithINI(txtexample.Text, "MAC1");
                EditMac2toend.Text = MainWindow.getInputdataWithINI(txtexample.Text, "MAC2");
                EditMac3toend.Text = MainWindow.getInputdataWithINI(txtexample.Text, "MAC3");
                EditMac4toend.Text = MainWindow.getInputdataWithINI(txtexample.Text, "MAC4");
                EditMac5toend.Text = MainWindow.getInputdataWithINI(txtexample.Text, "MAC5");

            }
        }

        public void FormShow()
        {
            INIFile ini = new INIFile("C:\\Windows\\SFIS.ini");
            txtexample.Text = ini.Read("PrePacking", "SNEXAMPLE");
            txtSN.Text = ini.Read("PrePacking", "SN");
            txtSnto.Text = ini.Read("PrePacking", "SNTO");
            txtSsn1.Text = ini.Read("PrePacking", "SSN1");
            txtSsn1to.Text = ini.Read("PrePacking", "SSN1TO");
            txtSsn2.Text = ini.Read("PrePacking", "SSN2");
            txtSsn2to.Text = ini.Read("PrePacking", "SSN2TO");
            txtSsn3.Text = ini.Read("PrePacking", "SSN3");
            txtSsn3to.Text = ini.Read("PrePacking", "SSN3TO");
            txtMac1.Text = ini.Read("PrePacking", "MAC1");
            txtMac1to.Text = ini.Read("PrePacking", "MAC1TO");
            txtMac2.Text = ini.Read("PrePacking", "MAC2");
            txtMac2to.Text = ini.Read("PrePacking", "MAC2TO");
            txtMac3.Text = ini.Read("PrePacking", "MAC3");
            txtMac3to.Text = ini.Read("PrePacking", "MAC3TO");
            txtMac4.Text = ini.Read("PrePacking", "MAC4");
            txtMac4to.Text = ini.Read("PrePacking", "MAC4TO");
            txtMac5.Text = ini.Read("PrePacking", "MAC5");
            txtMac5to.Text = ini.Read("PrePacking", "MAC5TO");
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            txtexample.Text = "";
            txtSN.Text = "";
            txtSsn1.Text = "";
            txtSsn2.Text = "";
            txtSsn3.Text = "";
            txtMac1.Text = "";
            txtMac2.Text = "";
            txtMac3.Text = "";
            txtMac4.Text = "";
            txtMac5.Text = "";
            txtSnto.Text = "";
            txtSsn1to.Text = "";
            txtSsn2to.Text = "";
            txtSsn3to.Text = "";
            txtMac1to.Text = "";
            txtMac2to.Text = "";
            txtMac3to.Text = "";
            txtMac4to.Text = "";
            txtMac5to.Text = "";
            EditSntoend.Text = "";
            EditSsn1toend.Text = "";
            EditSsn2toend.Text = "";
            EditSsn3toend.Text = "";
            EditMac1toend.Text = "";
            EditMac2toend.Text = "";
            EditMac3toend.Text = "";
            EditMac4toend.Text = "";
            EditMac5toend.Text = "";
        }

        private void txtexample_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
