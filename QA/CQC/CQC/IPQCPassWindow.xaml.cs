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

namespace CQC
{
    /// <summary>
    /// Interaction logic for IPQCPassWindow.xaml
    /// </summary>
    public partial class IPQCPassWindow : Window
    {
        private SfcHttpClient sfcClient;
        string MO_NUMBER, LINE_NAME;
        string G_Emp_No = "";
        public IPQCPassWindow(SfcHttpClient _sfcClient, string _mo_number, string _line_name)
        {
            sfcClient = _sfcClient;
            MO_NUMBER = _mo_number;
            LINE_NAME = _line_name;
            InitializeComponent();
        }

        private async void BitBtn2_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Edit1.Text))
            {
                MessageBox.Show("Input Mo","CQC");
                Edit1.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Edit2.Text))
            {
                MessageBox.Show("Input Line", "CQC");
                Edit2.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Edit3.Text))
            {
                MessageWindow mes = new MessageWindow(sfcClient, "00199");
                mes.Owner = this;
                mes.ShowDialog();
                Edit3.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Edit4.Password))
            {
                MessageBox.Show("Input password", "CQC");
                Edit4.Focus();
                return;
            }
            if (await qrynotpass(Edit2.Text, Edit1.Text))
            {
                if (await F_CHECK_BC(Edit4.Password))
                {
                    updatepass();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Mo in this line need not click PASS!", "CQC");
                return;
            }

        }

        async void updatepass()
        {
            try
            {
                string strupdate = "UPDATE SFISM4.R_MODELFILE_CHECK_T SET CHECK_EMP='" + G_Emp_No + "',PASS_FLAG='2'," +
                "CHECK_PASS_DATE=SYSDATE,CHECK_FILE_NO='" + Edit3.Text + "'  WHERE LINE_NAME='" + Edit2.Text + "' " +
                " AND MO_NUMBER='" + Edit1.Text + "' AND PASS_FLAG='1'";
                var Update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                {
                    CommandText = strupdate,
                    SfcCommandType = SfcCommandType.Text
                });
                if (Update.Result == "OK")
                    MessageBox.Show("pass OK", "CQC");
            }
            catch
            {
                MessageBox.Show("UPDATE SFISM4.R_MODELFILE_CHECK_T FAIL", "CQC");
                return;
            }

        }

        async Task<bool> F_CHECK_BC(string _pass)
        {
            string sSQL = "SELECT A.EMP_NO,A.EMP_NAME,A.QUIT_DATE,B.Privilege " +
                          "FROM SFIS1.C_EMP_DESC_T A,SFIS1.C_PRIVILEGE B " +
                          "WHERE A.EMP_NO=B.EMP AND B.FUN='CHECK_FILE_PASS' AND " +
                          "A.EMP_BC='" + _pass + "'";
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
            {
                string G_Privilege = qury.Data["privilege"].ToString();
                if (DateTime.Parse(qury.Data["quit_date"].ToString()) < DateTime.Now)
                {
                    MessageWindow mes = new MessageWindow(sfcClient, "00007");
                    mes.Owner = this;
                    mes.ShowDialog();
                    Edit3.SelectAll();

                }
                else
                {
                    G_Emp_No = qury.Data["emp_no"].ToString();
                    if (G_Privilege != "2")
                    {
                        MessageBox.Show("NO privilege to click Pass!", "CQC");
                    }
                    else
                        return true;
                }
            }
            else
                MessageBox.Show("No emp!");
            return false;
        }
        async Task<bool> qrynotpass(string _line_name, string _mo)
        {
            var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT * FROM SFISM4.R_MODELFILE_CHECK_T WHERE LINE_NAME='" + _line_name + "' AND MO_NUMBER='" + _mo + "' AND (PASS_FLAG<>'2' AND PASS_FLAG<>'0')",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data.Count() > 0)
                return true;
            return false;
        }

        private void BitBtn1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            Edit1.Text = MO_NUMBER;
            Edit2.Text = LINE_NAME;
            Edit3.Text = "";
            Edit4.Password = "";
            Edit3.Focus();
        }
    }
}
