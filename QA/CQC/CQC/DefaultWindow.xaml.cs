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
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;
using CQC.ViewModel;
using System.Collections;
using System.Collections.ObjectModel;
using Sfc.Library.HttpClient.Helpers;

namespace CQC
{
    /// <summary>
    /// Interaction logic for DefaultWindow.xaml
    /// </summary>
    public partial class DefaultWindow : Window
    {
        SfcHttpClient sfcClient;
        private MainWindow formCQC;
        private string M_sLineName, M_sQaNo, M_sEMP, M_sCustomer;

        private void combCus_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                bbtnOK_Click(new object(), new RoutedEventArgs());
        }

        private void bbtnCancel_Click(object sender, RoutedEventArgs e)
        {
            formCQC.ModalResult = false;
            this.Close();
        }

        private async void bbtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(combLot.Text) || string.IsNullOrEmpty(combLine.Text) || string.IsNullOrEmpty(combCus.Text) || string.IsNullOrEmpty(combEmp.Text))
            {
                MessageBox.Show("TABLES COLUMNS iS NULL!!?", "CQC", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                var quryFLot = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select  lot_no, model_name ,qa_result from  sfism4.r_cqc_rec_t where lot_no like '" + combLot.Text + "%" + "' order by lot_no",
                    SfcCommandType = SfcCommandType.Text
                });
                if (quryFLot.Data != null)
                {
                    if (quryFLot.Data["model_name"].ToString() != formCQC.editModel.Text)
                    {
                        MessageBox.Show("DIFFERENT MODEL NAME, PLEASE CREATE NEW LOT NO", "CQC", MessageBoxButton.OK, MessageBoxImage.Error);
                        combLot.Focus();
                        return;
                    }
                    if (quryFLot.Data["qa_result"].ToString() == "0")
                    {
                        MessageBox.Show("THIS LOT N.O. HAVE BEEN PASSED ! PLEASE INPUT NEW LOT N.O.", "CQC", MessageBoxButton.OK, MessageBoxImage.Error);
                        combLot.Focus();
                        return;
                    }
                    if (quryFLot.Data["qa_result"].ToString() == "1")
                    {
                        MessageBox.Show("THIS LOT N.O. HAVE BEEN REJECTED ! PLEASE INPUT NEW LOT N.O.", "CQC", MessageBoxButton.OK, MessageBoxImage.Error);
                        combLot.Focus();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("THIS LOT N.O. DUPLICATED ! PLEASE INPUT NEW LOT N.O.", "CQC", MessageBoxButton.OK, MessageBoxImage.Error);
                        combLot.Focus();
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(combLot.Text))
                {
                    var quryILot = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "insert into sfism4.r_cqc_rec_t (lot_no, qa_result, PASS_QTY, FAIL_QTY, tester,line_name,customer,lot_size,ng_num,po_number,ssn) values (:lot_no, :qa_result, :PASS_QTY, :FAIL_QTY, :tester,:line_name,:customer,:lot_size,:ng_num,:po_number,:ssn)",
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter {Name = "lot_no", Value=combLot.Text.ToUpper() },
                            new SfcParameter {Name = "qa_result" , Value= "N/A"},
                            new SfcParameter {Name = "PASS_QTY", Value=0 },
                            new SfcParameter {Name = "FAIL_QTY", Value=0 },
                            new SfcParameter {Name = "tester", Value=formCQC.G_sTester },
                            new SfcParameter {Name = "line_name", Value=combLine.Text.ToUpper() },
                            new SfcParameter {Name = "customer" , Value=combCus.Text.ToUpper()},
                            new SfcParameter {Name = "lot_size", Value=0 },
                            new SfcParameter {Name = "ng_num", Value=0 },
                            new SfcParameter {Name = "po_number", Value=formCQC.ComboBox1.Text },
                            new SfcParameter {Name = "ssn", Value=formCQC.qurySerN[0].MO_NUMBER }
                        }
                    });
                    if (quryFLot.Result != "OK")
                    {
                        MessageBox.Show("insert into sfism4.r_cqc_rec_t ERROR", "CQC");
                    }
                    if (formCQC.chkbSamplingPlan)
                    {
                        quryILot = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "Update sfism4.r_cqc_rec_t  Set MAJOR_FAIL_QTY=0,MINOR_FAIL_QTY=0 ,CRITICAL_FAIL_QTY=0 where lot_no = '" + combLot.Text.ToUpper() + "'",
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (quryILot.Result != "OK")
                            MessageBox.Show("Update sfism4.r_cqc_rec_t ERROR");
                    }
                    if (formCQC.chkbInsertQCSN)
                    {
                        quryILot = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "Update sfism4.r_cqc_rec_t  Set section_name= '" + formCQC.sMySection + "' ,group_name= '" + formCQC.sMyGroup + "' ,Station_name= '" + formCQC.sMyStation + "' where lot_no='" + combLot.Text.ToUpper() + "'",
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (quryILot.Result != "OK")
                            MessageBox.Show("Update sfism4.r_cqc_rec_t ERROR", "CQC");
                    }

                    formCQC.combLot.Items.Add(combLot.Text.ToUpper());
                    formCQC.combLot.SelectedIndex = combLot.Items.IndexOf(combLot.Text.ToUpper());
                    formCQC.combLot.Text = combLot.Text;
                    formCQC.combEmp.Text = combEmp.Text;
                    formCQC.editCount.Content = string.Empty;
                    formCQC.editCheckedQty.Content = string.Empty;
                    formCQC.editFailedQty.Content = string.Empty;
                    formCQC.clstrgridInLot.ItemsSource=null;
                    formCQC.clstrgridNotChecked.ItemsSource=null;
                    formCQC.combDef.Focus();
                }
                formCQC.logic1 = false;
                formCQC.ModalResult = true;
                this.Close();
            }
        }


        public DefaultWindow()
        {
            InitializeComponent();
        }


        public DefaultWindow(MainWindow _formCQC, SfcHttpClient _sfcClient)
        {
            formCQC = _formCQC;
            sfcClient = _sfcClient;
            InitializeComponent();
        }


        private async void Window_Initialized(object sender, EventArgs e)
        {
            string sSQL;
            combEmp.Text = formCQC.combEmp.Text;
            combEmp.Items.Add(combEmp.Text);
            combEmp.SelectedIndex = combEmp.Items.IndexOf(combEmp.Text);
            combEmp.IsEnabled = false;
            //query cust
            var quryCus = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select customer from SFIS1.C_CUSTOMER_T group by customer order by customer",
                SfcCommandType = SfcCommandType.Text
            });

            if (quryCus.Data.Count() > 0)
            {
                foreach (CUSTOMERS item in quryCus.Data.ToListObject<CUSTOMERS>().ToList())
                    combCus.Items.Add(item.CUSTOMER);
            }

            //querycust mo
            if (formCQC.qurySerN.Count == 0)
                sSQL = "select customer from SFIS1.C_CUSTOMER_T where cust_code = (select cust_code from sfism4.r_mo_base_t where mo_number = '')";
            else
                sSQL = "select customer from SFIS1.C_CUSTOMER_T where cust_code = (select cust_code from sfism4.r_mo_base_t where mo_number = '" + formCQC.qurySerN[0].MO_NUMBER + "')";
            combCus.Focus();
            var quryCustCode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (combCus.Items.Count > 0)
            {
                if (quryCustCode.Data != null)
                {
                    if (combCus.Items.IndexOf(quryCustCode.Data["customer"].ToString()) == -1)
                        MessageBox.Show("No Customer can Select!", "CQC");
                    else
                        combCus.SelectedIndex = combCus.Items.IndexOf(quryCustCode.Data["customer"].ToString());
                }
                else
                    combCus.SelectedIndex = 0;
            }

            var quryLine = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select line_name from sfis1.c_line_desc_t order by line_name",
                SfcCommandType = SfcCommandType.Text
            });
            if (quryLine.Data.Count() != 0)
            {
                foreach (LineName items in quryLine.Data.ToListObject<LineName>().ToList())
                    combLine.Items.Add(items.LINE_NAME);
            }
            combLine.SelectedIndex = combLine.Items.IndexOf(formCQC.qurySerN[0].LINE_NAME);
            await autocreateLotNo();
            M_sLineName = combLine.Text;
            M_sCustomer = combCus.Text;
            M_sEMP = combEmp.Text;
            M_sQaNo = combLot.Text;
        }

        async Task autocreateLotNo()
        {
            string sql = "";
            if (formCQC.itemLineAll.IsChecked)
                sql = "select  lot_no, model_name ,qa_result from  sfism4.r_cqc_rec_t where lot_no like '" + formCQC.sMyGroup + DateTime.Now.ToString("yyyyMMdd") + "%" + "' order by lot_no desc";
            if (formCQC.itemLineSingle.IsChecked)
                sql = "select  lot_no, model_name ,qa_result from  sfism4.r_cqc_rec_t where lot_no like '" + formCQC.qurySerN[0].LINE_NAME + formCQC.sMyGroup + DateTime.Now.ToString("yyyyMMdd") + "%" + "' order by lot_no desc";

            var quryFLot = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryFLot.Data.Count() == 0)
            {
                if (formCQC.itemLineAll.IsChecked)
                    combLot.Text = formCQC.sMyGroup + DateTime.Now.ToString("yyyyMMdd") + "0001";
                if (formCQC.itemLineSingle.IsChecked)
                    combLot.Text = formCQC.qurySerN[0].LINE_NAME + formCQC.sMyGroup + DateTime.Now.ToString("yyyyMMdd") + "0001";
            }
            else
            {
                List<CQCREC> LotNoList = new List<CQCREC>();
                LotNoList = quryFLot.Data.ToListObject<CQCREC>().ToList();
                string sTemp = LotNoList[0].LOT_NO.Substring(LotNoList[0].LOT_NO.Length - 4, 4);
                sTemp = (int.Parse(sTemp) + 1).ToString();
                int iLength = sTemp.Length;
                switch (iLength)
                {
                    case 1:
                        sTemp = string.Concat("000", sTemp);
                        break;
                    case 2:
                        sTemp = string.Concat("00", sTemp);
                        break;
                    case 3:
                        sTemp = string.Concat("0", sTemp);
                        break;
                }
                if (formCQC.itemLineAll.IsChecked)
                    combLot.Text = formCQC.sMyGroup + DateTime.Now.ToString("yyyyMMdd") + sTemp;
                if (formCQC.itemLineSingle.IsChecked)
                    combLot.Text = formCQC.qurySerN[0].LINE_NAME + formCQC.sMyGroup + DateTime.Now.ToString("yyyyMMdd") + sTemp;
            }
            if (combLot.Items.IndexOf(combLot.Text) == -1)
            {
                combLot.Items.Add(combLot.Text);
            }
            combLot.SelectedIndex = combLot.Items.IndexOf(combLot.Text);
        }
    }
}
