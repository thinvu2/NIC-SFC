using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Deployment.Application;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using System.Reflection;
using PM.Model;

namespace PM
{
    /// <summary>
    /// Interaction logic for InitForm.xaml
    /// </summary>
    public partial class InitForm : Window
    {
        public SfcHttpClient sfcClient;
        public string sCompanyName;
        public InitForm()
        {
            InitializeComponent();
            Lb_Version.Content = "Version :" + getRunningVersion().ToString();
        }
        private Version getRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void InitForm_Initialized(object sender, EventArgs e)
        {
            try
            {
                INIFile ini = new INIFile("SFIS.ini");
                sCompanyName = ini.Read("PM", "Company");
            }
            catch (Exception ex)
            {
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = "Excute have error:" + ex.Message;
                _er.MessageVietNam = "Có lỗi phát sinh: " + ex.Message;
                _er.ShowDialog();
                this.Close();
            }
            if (!string.IsNullOrEmpty(sCompanyName))
            {
                Lb_License.Content = "Licensed to : " + sCompanyName;
            }
            else
            {
                Lb_License.Content = "";
            }
        }
    }
}
