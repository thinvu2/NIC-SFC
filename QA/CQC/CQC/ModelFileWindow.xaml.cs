using CQC.ViewModel;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace CQC
{
    /// <summary>
    /// Interaction logic for ModelFileWindow.xaml
    /// </summary>
    public partial class ModelFileWindow : Window
    {
        private DispatcherTimer dispatcherTimer;
        private SfcHttpClient sfcClient;
        public string PASSDATETIME1,PASSDATETIME2,emppwd,G_Emp_No;

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //R107 tmp = clstrgridInLot.Items[1] as R107;
            int index = DBGridPlus1.SelectedIndex;
            if (index != -1)
            {
                R_MODELFILE_CHECK tmp = DBGridPlus1.Items[index] as R_MODELFILE_CHECK;
                IPQCPassWindow frmFilePass = new IPQCPassWindow(sfcClient,tmp.MO_NUMBER, tmp.LINE_NAME);
                frmFilePass.Owner = this;
                frmFilePass.ShowDialog();
            }
            Window_Initialized(new Object(), new EventArgs());
        }

        private void Label2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Button2.IsVisible == true)
                Button2.Visibility = Visibility.Hidden;
            else
                Button2.Visibility = Visibility.Visible;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            int index = DBGridPlus1.SelectedIndex;
            if (index != -1)
            {
                R_MODELFILE_CHECK tmp = DBGridPlus1.Items[index] as R_MODELFILE_CHECK;
                PDPassWindow frmpdpass = new PDPassWindow(sfcClient, tmp.MO_NUMBER, tmp.LINE_NAME);
                frmpdpass.Owner = this;
                frmpdpass.ShowDialog();
            }
            Window_Initialized(new Object(), new EventArgs());
        }

        private async void Button3_Click(object sender, RoutedEventArgs e)
        {
            formPasswdWindow formPasswd = new formPasswdWindow(this);
            formPasswd.Owner = this;
            formPasswd.ShowDialog();

            string SQLSTR = "SELECT A.EMP_NO,A.EMP_NAME,A.QUIT_DATE,B.Privilege " +
           "FROM SFIS1.C_EMP_DESC_T A,SFIS1.C_PRIVILEGE B " +
           "WHERE A.EMP_NO=B.EMP AND B.FUN='CHECK_FILE_CLEAR AND A.EMP_BC='" + emppwd + "'";

            var qrypass = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText=SQLSTR,
                SfcCommandType=SfcCommandType.Text
            });
            if (qrypass.Data == null)
                return;
            else
            {
                string G_Privilege = qrypass.Data["privilege"].ToString();
                string DQuitDate = qrypass.Data["quit_date"].ToString();
                if (DateTime.Parse(DQuitDate) < DateTime.Now)
                {
                    MessageWindow mes = new MessageWindow(sfcClient, "00007");
                    mes.Owner = this;
                    mes.ShowDialog();
                    return;
                }
                else if (G_Privilege != "2")
                {
                    MessageBox.Show("You do not have authority for clear!");
                    return;
                }
                else
                {
                    G_Emp_No = qrypass.Data["emp_no"].ToString();
                    updatepass();
                }
            }
            Window_Initialized(new Object(), new EventArgs());
        }
        async void updatepass()
        {
            try
            {
              string  strupdate= "UPDATE SFISM4.R_MODELFILE_CHECK_T SET CHECK_EMP='"+ G_Emp_No + "',PASS_FLAG='2'," +
                                 "CHECK_PASS_DATE=SYSDATE,CHECK_FILE_NO='CLEAR HISTORY RECORD'  WHERE  " +
                                 " FIRST_INSTATION_TIME<SYSDATE-7 AND PASS_FLAG<>'2'";
                var Update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                {
                    CommandText = strupdate,
                    SfcCommandType = SfcCommandType.Text
                });
                if (Update.Result == "OK")
                    MessageBox.Show("clear history record success!");
            }
            catch
            {
                MessageBox.Show("clear history record success!");
                return;
            }

        }


        public ModelFileWindow(SfcHttpClient _sfcClient)
        {
            sfcClient = _sfcClient;
            InitializeComponent();
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0, 2000000);
            dispatcherTimer.Start();
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            DBGridPlus1.ItemsSource = null;
            List<R_MODELFILE_CHECK> checklist = new List<R_MODELFILE_CHECK>();
            checklist.Clear();
            string strsql;
            strsql = "SELECT MO_NUMBER, MODEL_NAME, VERSION_CODE, LINE_NAME, FIRST_INSTATION_TIME ,PASS_DATE," +
                    " FILE_NO,EMP_NO,PASS_FLAG FROM SFISM4.R_MODELFILE_CHECK_T" +
                    " WHERE  PASS_FLAG<>'2' ORDER BY FIRST_INSTATION_TIME  DESC";
            var qryFileInfo = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strsql,
                SfcCommandType = SfcCommandType.Text,               
            });
            checklist = qryFileInfo.Data.ToListObject<R_MODELFILE_CHECK>().ToList();
            DBGridPlus1.ItemsSource = checklist;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            Window_Initialized(new Object(), new EventArgs());
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
