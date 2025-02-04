using CQC.ViewModel;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
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
    /// Interaction logic for Form3Window.xaml
    /// </summary>
    public partial class Form3Window : Window
    {
        MainWindow formCQC;
        public SfcHttpClient sfcClient;
        public Form3Window(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            sfcClient = formCQC.sfcClient;
            formCQC.logic1 = false;
            InitializeComponent();
            passworded.Focus();
        }

        private void EMPED_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btn1_Click(new object(), new RoutedEventArgs());
            }
        }

        private void passworded_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btn1_Click(new object(), new RoutedEventArgs());
            }
        }

        private async void btn1_Click(object sender, RoutedEventArgs e)
        {
            if (passworded.Password == "")
            {
                MessageBox.Show("Pls input password ", "CQC", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                passworded.Focus();
                return;
            }
            else
            {
                var quryprivi = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = "Select EMP_NAME,EMP_NO,QUIT_DATE From SFIS1.C_EMP_DESC_T WHERE EMP_BC = :emp_bc",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter {Name="emp_bc", Value=passworded.Password }
                    }
                });
                if (quryprivi.Data == null)
                {
                    MessageBox.Show("Employee Not Exists !", "CQC", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
                else if (DateTime.Now > DateTime.Parse(quryprivi.Data["quit_date"].ToString()))
                {
                    MessageBox.Show("Employee Expired !", "CQC", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
                else
                {
                    EMPED.Text = quryprivi.Data["emp_no"].ToString();
                    var quryprivi2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "SELECT FUN, PRIVILEGE FROM SFIS1.C_PRIVILEGE WHERE  EMP = :EMP_NO AND FUN='CQC' AND PRG_NAME = 'CQCNOTPASSOOBA'",
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter {Name="EMP_NO", Value=EMPED.Text }
                    }
                    });
                    if (quryprivi2.Data.Count() == 0 || quryprivi2.Data == null)
                    {
                        MessageBox.Show("Employee No PRIVILEGE !", "CQC", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }
                    else
                    {
                        EMPED.Clear();
                        passworded.Clear();
                        formCQC.logic1 = true;
                        this.Close();
                    }
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            formCQC.combDef.Focus();
        }
    }
}
