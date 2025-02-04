using Sfc.Core.Parameters;
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

namespace CQC
{
    /// <summary>
    /// Interaction logic for frloginWindow.xaml
    /// </summary>
    public partial class frloginWindow : Window
    {
        private MainWindow formCQC;
        private CheckboxssnWindow Fm_checkboxssn;
        public SfcHttpClient sfcClient;
        public frloginWindow(CheckboxssnWindow _Fm_checkboxssn, MainWindow _formCQC, SfcHttpClient _sfcClient)
        {
            sfcClient = _sfcClient;
            Fm_checkboxssn = _Fm_checkboxssn;
            formCQC = _formCQC;
            InitializeComponent();
        }

        private async void bbtnOK_Click(object sender, RoutedEventArgs e)
        {
            if(ed_BC.ToString() == "")
            {
                MessageBox.Show("This Employee NO. not found!!","CQC");
                ed_BC.Focus();
                formCQC.G_sTester = "";
                return;
            }
            var Query1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_BC='"+ ed_BC.Password.ToString() + "' AND ROWNUM=1",
                SfcCommandType = SfcCommandType.Text
            });
            if(Query1.Data != null)
            {
                if(await formCQC.CheckPrivilege(Query1.Data["emp_no"].ToString(),formCQC.sMyGroup))
                {
                    if(DateTime.Now < DateTime.Parse(Query1.Data["quit_date"].ToString()))
                    {
                        //ED_EMP.Text = Query1.Data["EMP_NAME"].ToString();
                        Fm_checkboxssn.chkMBD.IsChecked = false;
                        Fm_checkboxssn.lbl1.IsEnabled = false;
                        Fm_checkboxssn.lblKPN.IsEnabled = false;
                    }
                }
                else
                {
                    if (DateTime.Now > DateTime.Parse(Query1.Data["quit_date"].ToString()))
                    {
                        MessageBox.Show(" Employee Invalid","CQC");
                    }
                    else
                    {
                        MessageBox.Show(" Illegal EMP","CQC");
                        ed_BC.Focus();
                    }
                }
            }
            else
            {
                MessageBox.Show("This Employee NO. not found!!","CQC");
                ed_BC.Focus();
            }
            this.Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ed_BC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (ed_BC.ToString() == "")
                {
                    return;
                }
                bbtnOK_Click(new Object(), new RoutedEventArgs());
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            ed_BC.Focus();
        }
    }
}
