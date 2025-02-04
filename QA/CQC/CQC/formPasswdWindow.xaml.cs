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
    /// Interaction logic for formPasswdWindow.xaml
    /// </summary>
    public partial class formPasswdWindow : Window
    {
        private ModelFileWindow frmModelFile;
        public formPasswdWindow(ModelFileWindow _frmModelFile)
        {
            frmModelFile = _frmModelFile;
            InitializeComponent();
        }
        public formPasswdWindow()
        {
            InitializeComponent();
        }

        private void bbtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (meditPassword.Password != "")
            {
                if (frmModelFile != null)
                    frmModelFile.emppwd = meditPassword.Password;
                this.Close();
            }
            else
            {
                MessageBox.Show("please input password","CQC");
                meditPassword.Focus();
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            meditPassword.Clear();
            meditPassword.Focus();
        }
    }
}
