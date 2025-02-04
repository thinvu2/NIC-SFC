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
    /// Interaction logic for PDPassWindow.xaml
    /// </summary>
    public partial class PDPassWindow : Window
    {
        private SfcHttpClient sfcClient;
        MainWindow formCQC = new MainWindow();
        string G_Emp_No;
        public PDPassWindow(SfcHttpClient _sfcClient, string _mo, string _line)
        {
            sfcClient = _sfcClient;
            InitializeComponent();
            Edit1.Text = _mo;
            Edit2.Text = _line;
        }

        private void BitBtn1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BitBtn2_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Edit1.Text))
            {
                MessageBox.Show("Input Mo");
                Edit1.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Edit2.Text))
            {
                MessageBox.Show("Input Line");
                Edit2.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Edit3.Text))
            {
                MessageBox.Show("Input File Name");
                Edit3.Focus();
                return;
            }
            if (string.IsNullOrEmpty(edit4.Password))
            {
                MessageBox.Show("Input password");
                edit4.Focus();
                edit4.SelectAll();
                return;
            }

            if (await qrynotpass(Edit2.Text, Edit1.Text))
            {
                if(await F_CHECK_BC(edit4.Password))
                {
                    updatepass();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Mo in this line need not click PASS!");
            }
        }


        async void updatepass()
        {
            try
            {
                string strupdate = "UPDATE SFISM4.R_MODELFILE_CHECK_T SET PASS_DATE=SYSDATE,FILE_NO='"+Edit3.Text+"', " +
                " EMP_NO='"+G_Emp_No+"',PASS_FLAG='1' WHERE PASS_FLAG='0' AND " +
                " LINE_NAME='"+Edit2.Text+"' AND MO_NUMBER='"+Edit1.Text+"' ";
                var Update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                {
                    CommandText = strupdate,
                    SfcCommandType = SfcCommandType.Text
                });
                if (Update.Result == "OK")
                    MessageBox.Show("PASS OK!");
            }
            catch
            {
                MessageBox.Show("UPDATE SFISM4.R_MODELFILE_CHECK_T FAIL");
                return;
            }

        }
        
        async Task<bool> qrynotpass(string _line_name, string _mo_number)
        {

            var query = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText= "SELECT * FROM SFISM4.R_MODELFILE_CHECK_T WHERE LINE_NAME='"+_line_name+"' AND MO_NUMBER='"+_mo_number+"' AND PASS_FLAG='0'",
                SfcCommandType=SfcCommandType.Text
            });
            if (query.Data.Count() > 0)
                return true;
            return false;
        }

        private void edit4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (string.IsNullOrEmpty(edit4.Password))
                {
                    MessageBox.Show("Input password");
                    edit4.SelectAll();
                    edit4.Focus();
                    return;
                }
                else
                    BitBtn2_Click(new object(), new RoutedEventArgs());
            }
        }

        async Task<bool> F_CHECK_BC(string strpassword)
        {
            string SQLSTR = "SELECT *  FROM SFIS1.C_EMP_DESC_T WHERE EMP_PASS='" + strpassword + "'";
            var qrypdpass = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = SQLSTR,
                SfcCommandType = SfcCommandType.Text
            });
            if (qrypdpass.Data == null)
                return false;
            if (qrypdpass.Data.Count > 0)
            {
                string str_class = qrypdpass.Data["class_name"].ToString();
                int i = int.Parse(str_class.IndexOf("3").ToString());
                if (i == -1)
                {
                    MessageBox.Show(await formCQC.GetPubMessage("00197"));
                    edit4.Focus();
                    return false;
                }
                else
                {
                    G_Emp_No = qrypdpass.Data["emp_no"].ToString();
                    return true;
                }
            }
            else
            {
                MessageBox.Show("No emp!");
                return false;
            }
        }
    }
}
