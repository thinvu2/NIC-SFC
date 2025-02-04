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
using PACK_CTN;
using PACK_CTN.Models;
using Sfc.Core.Parameters;

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for CheckCartonLabel.xaml
    /// </summary>
    public partial class CheckCartonLabel : Window
    {
        public  string MODELNAME ,CUSTMODELDESC ,CUSTMODELNAME ,EmpNo , LineName;
        public static bool MACEDIT = false;
        private string sqlStr;
        private string sqlCheckData = " SELECT SSN1,MAC1, COUNT (*) QTY  FROM SFISM4.R_CUSTSN_T A, SFISM4.R107 B WHERE b.mcarton_no= :CARTON " +
            "  and A.SERIAL_NUMBER = B.SERIAL_NUMBER  and (SSN1 =:SN OR SSN2 = :SN OR SSN3 = :SN OR SSN4 = :SN OR SSN5 = :SN OR SSN6 = :SN OR SSN7 = :SN OR SSN8 = :SN" +
            " OR SSN9 = :SN OR SSN10 = :SN OR SSN11 = :SN OR SSN12 = :SN OR SSN13 = :SN OR SSN14 = :SN OR SSN15 = :SN OR MAC1 = :SN" +
            " OR MAC2 = :SN OR MAC3 = :SN OR MAC4 = :SN OR MAC5 = :SN )" +
            " GROUP BY ssn1,MAC1  "  ;
        MainWindow frmMain = new MainWindow();
        List<ListShippingSN> itemsSSN = new List<ListShippingSN>();
        List<ListMac> itemsMAC = new List<ListMac>();
        public CheckCartonLabel()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void txtShippingSN_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    sqlStr = " SELECT SSN1,MAC1, COUNT (*) QTY  FROM SFISM4.R_CUSTSN_T A, SFISM4.R107 B WHERE b.mcarton_no= '" + txtCartonNo.Text + "' " +
                                   "  and A.SERIAL_NUMBER = B.SERIAL_NUMBER  and (SSN1 = '" + txtShippingSN.Text + "' )" +
                                   " GROUP BY ssn1,MAC1  ";

                    var result = await MainWindow._sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = " SELECT SSN1,MAC1, COUNT (*) QTY  FROM SFISM4.R_CUSTSN_T A, SFISM4.R107 B WHERE b.mcarton_no= '"+txtCartonNo.Text+"' " +
                                     "  and A.SERIAL_NUMBER = B.SERIAL_NUMBER  and (SSN1 = '"+txtShippingSN.Text+"' )" +
                                     " GROUP BY ssn1,MAC1  "  ,
                        SfcCommandType = SfcCommandType.Text ,
                        SfcParameters = new List<SfcParameter>
                        {
                          new SfcParameter{ Name = "SN", Value = txtShippingSN.Text},
                          new SfcParameter{ Name = "CARTON", Value = txtCartonNo.Text}
                        }
                    });
                    if (result.Data != null)
                    {
                        if (((result.Data.Count != 0) && itemsSSN.Any(i => i.SHIPPING_SN == result.Data["ssn1"].ToString() )) )
                        {
                            itemsSSN.Remove(itemsSSN.Single(i => i.SHIPPING_SN == result.Data["ssn1"].ToString() ));
                            lstvShippingSN.ItemsSource = null;
                            lstvShippingSN.ItemsSource = itemsSSN;
       
                            lblQty.Content = (int.Parse(lblQty.Content.ToString()) - 1).ToString();
                            if (lstvMac.Items.Count == 1)
                            {
                                txtMac.Text = "N/A";
                                txtMac.IsEnabled = false;
                                if (int.Parse(lblQty.Content.ToString()) != 0)
                                {
                                    txtShippingSN.SelectAll();
                                    txtShippingSN.Focus();
                                    lblMess.Content = "Scan SSN";
                                }
                                else
                                {
                                    txtQty.SelectAll();
                                    txtQty.Focus();
                                    lblMess.Content = "Scan QTY";
                                }
                            }
                            else
                            {
                                txtMac.IsEnabled = true;
                                txtMac.SelectAll();
                                txtMac.Focus();
                            }
                            lblMess.Content = "Scan MAC";
                        }
                        else
                        {
                            MessageError frmMessage = new MessageError();
                            frmMessage.CustomFlag = true;
                            frmMessage.MessageEnglish = "DATA WRONG , PLEASE CONFIRM!!";
                            frmMessage.MessageVietNam = "SSN SẢO VÀO KHÔNG ĐÚNG HOẶC ĐÃ ĐƯỢC SẢO , VUI LÒNG KIỂM XÁC NHẬN LẠI !! ";
                            frmMessage.ShowDialog();
                            txtShippingSN.SelectAll();
                            txtShippingSN.Focus();
                        }
                    }
                    else
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = "DATA WRONG, PLEASE CONFIRM!!";
                        frmMessage.MessageVietNam = "DỮ LIỆU SẢO VÀO KHÔNG ĐÚNG, VUI LÒNG KIỂM XÁC NHẬN LẠI";
                        frmMessage.ShowDialog();
                        txtShippingSN.SelectAll();
                        txtShippingSN.Focus();  
                    }
                }
                catch (Exception ex)
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "Check Shipping SN error!!";
                    frmMessage.MessageVietNam = ex.ToString();
                    frmMessage.ShowDialog();
                    txtShippingSN.SelectAll();
                    txtShippingSN.Focus();
                }
            }
        }

        private void frmCheckCarton_Loaded(object sender, RoutedEventArgs e)
        {
            txtCartonNo.SelectAll();
            txtCartonNo.Focus();
            lblMess.Content = "Scan carton no";
        }

        private async void txtMac_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                { 
                    var result = await MainWindow._sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = sqlCheckData,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>
                        {
                          new SfcParameter{ Name = "SN", Value = txtMac.Text},
                            new SfcParameter{ Name = "CARTON", Value = txtCartonNo.Text}
                        }
                    });
                    if (result.Data != null)
                    {
                        if ((result.Data.Count != 0) && (itemsMAC.Any(i => i.SHIPPING_SN2 == result.Data["mac1"].ToString())))
                        {
                            itemsMAC.Remove(itemsMAC.Single(i => i.SHIPPING_SN2 == result.Data["mac1"].ToString() ));
                            lstvMac.ItemsSource = null;
                            lstvMac.ItemsSource = itemsMAC;

                            if (int.Parse(lblQty.Content.ToString()) != 0)
                            {
                                txtShippingSN.SelectAll();
                                txtShippingSN.Focus();
                                lblMess.Content = "Scan SSN";
                            }
                            else
                            {
                                txtQty.SelectAll();
                                txtQty.Focus();
                                lblMess.Content = "Scan QTY";
                            }
                            
                        }
                        else
                        {
                            MessageError frmMessage = new MessageError();
                            frmMessage.CustomFlag = true;
                            frmMessage.MessageEnglish = "DATA MAC SCAN WRONG , PLEASE CONFIRM!!";
                            frmMessage.MessageVietNam = "MAC SẢO VÀO KHÔNG ĐÚNG HOẶC ĐÃ ĐƯỢC SẢO , VUI LÒNG KIỂM XÁC NHẬN LẠI !! ";
                            frmMessage.ShowDialog();
                            txtMac.SelectAll();
                            txtMac.Focus();
                        }
                    }
                    else
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = "DATA MAC SCAN WRONG, PLEASE CONFIRM!!";
                        frmMessage.MessageEnglish = "DỮ LIỆU SẢO VÀO KHÔNG ĐÚNG, VUI LÒNG KIỂM XÁC NHẬN LẠI";
                        frmMessage.ShowDialog();
                        txtMac.SelectAll();
                        txtMac.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "Check MAC error!!";
                    frmMessage.MessageVietNam = ex.ToString();
                    frmMessage.ShowDialog();
                    txtMac.SelectAll();
                    txtMac.Focus();
                }
            }
        }

        private async void txtQty_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sqlStr = " SELECT  SSN1,MAC1,COUNT(*)QTY  FROM SFISM4.R107 A,SFISM4.R_CUSTSN_T B" +
                    " WHERE  CARTON_NO IN(SELECT DISTINCT CARTON_NO  FROM SFISM4.R117 WHERE MODEL_NAME= :MODEL " +
                    " AND GROUP_NAME='PACK_CTN') AND  (MCARTON_NO= :CARTON OR CARTON_NO= :CARTON )  " +
                    " AND A.SERIAL_NUMBER=B.SERIAL_NUMBER GROUP BY SSN1,MAC1";
                var result = await MainWindow._sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sqlStr,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                          new SfcParameter{ Name = "MODEL", Value = MODELNAME },
                          new SfcParameter{ Name = "CARTON", Value = txtCartonNo.Text},
                    }
                });
                if (result.Data != null)
                {
                    if (result.Data.Count() == int.Parse(txtQty.Text.ToString()))
                    {
                        sqlStr = "   Insert into SFISM4.R_SYSTEM_PRGLOG_T " +
                            " (emp_no, prg_name, action_type, action_desc, time, REASON_DESC, doc_no)" +
                            "  Values( '"+EmpNo+"', 'Packing2_3Com', 'CHECKLABEL','"+txtCartonNo.Text+"', SYSDATE,'"+LineName+"','"+MODELNAME+"')";
                        var _insert = await MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sqlStr
                        });
                        if (_insert.Result.ToString() == "OK")
                        {
                            this.Close();
                        }
                        else
                        {
                            MessageError frmMessage = new MessageError();
                            frmMessage.CustomFlag = true;
                            frmMessage.MessageEnglish = " SAVE LOG ERROR ,CALL IT CHECK !!";
                            frmMessage.MessageVietNam = " LƯU LOG PHÁT SINH LÔI , LIÊN HỆ IT CHECK !!";
                            frmMessage.ShowDialog();
                            txtQty.SelectAll();
                            txtQty.Focus();
                        }
                    }
                    else
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = "DATA QTY SCAN NOT MATCH WITH QTY OF CARTON !!";
                        frmMessage.MessageVietNam = "SỐ LƯỢNG SẢO VÀO KHÔNG ĐÚNG VỚI SỐ LƯỢNG CỦA CARTON!!";
                        frmMessage.ShowDialog();
                        txtQty.SelectAll();
                        txtQty.Focus();
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "Not found QTY of carton no!!";
                    frmMessage.MessageVietNam = "Không tìm thấy số lượng của carton!!";
                    frmMessage.ShowDialog();
                    txtQty.SelectAll();
                    txtQty.Focus();
                }
            }
        }
        private async void TxtCartonNo_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                if (lblCartonNo.Content.ToString() == txtCartonNo.Text)
                {
                    sqlStr = "SELECT SSN1 , MAC1 FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER  IN (SELECT SERIAL_NUMBER FROM SFISM4.R107 WHERE MCARTON_NO ='" + txtCartonNo.Text + "')";
                    var result = await MainWindow._sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sqlStr ,
                        SfcCommandType  = SfcCommandType.Text 
                    });

                    if (result.Data != null)
                    {
                        if (result.Data.Count() != 0)
                        {
                            lblQty.Content = result.Data.Count();

                            foreach (var row in result.Data)
                            {
                                itemsSSN.Add(new ListShippingSN() { SHIPPING_SN = row["ssn1"].ToString() });
                                if (row["mac1"] != null)
                                {
                                    itemsMAC.Add(new ListMac() { SHIPPING_SN2 = row["mac1"].ToString() });
                                }
                            }
                            lstvShippingSN.ItemsSource = itemsSSN;
                            lstvMac.ItemsSource = itemsMAC;

                            txtCartonNo.IsEnabled = false;
                            txtShippingSN.IsEnabled = false;
                            txtMac.IsEnabled = false;
                            txtQty.IsEnabled = false;
                            txtCustModelDesc.Text = "";
                            txtCustModelDesc.Focus();
                            lblMess.Content = "SCAN PN/SKU";
                        }
                        else
                        {
                            MessageError frmMessage = new MessageError();
                            frmMessage.CustomFlag = true;
                            frmMessage.MessageEnglish = "Not found data ssn1/mac1 of Carton_no!!";
                            frmMessage.MessageVietNam = "Không tìm thấy dữ liệu ssn1/mac1 cuar carton!!";
                            frmMessage.ShowDialog();
                            txtCartonNo.SelectAll();
                            txtCartonNo.Focus();
                        }
                    }
                    else
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = "Not found data carton no!!";
                        frmMessage.MessageVietNam = "Không tìm thấy dữ liệu carton!!";
                        frmMessage.ShowDialog();
                        txtCartonNo.SelectAll();
                        txtCartonNo.Focus();
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "THE CARTON NO SCAN NOT MATCH WITH CARTON NEED CHECK";
                    frmMessage.MessageVietNam = "MA THUNG VUA XAO, KHONG PHAI MA THUNG CAN CHECK!";
                    frmMessage.ShowDialog();
                    txtCartonNo.SelectAll();
                    txtCartonNo.Focus();
                }
            }
        }

        private  async void  txtCustModelDesc_Click(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sqlStr = " SELECT DISTINCT CUST_MODEL_DESC,replace(CUST_MODEL_NAME,'FXN','') CUST_MODEL_NAME  FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME = '" + MODELNAME + "' ";
                var result = await MainWindow._sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                     CommandText = sqlStr,
                     SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null )
                {
                    CUSTMODELDESC = result.Data["cust_model_desc"].ToString();
                    CUSTMODELNAME = result.Data["cust_model_name"].ToString();

                    if  ((txtCustModelDesc.Text == CUSTMODELDESC)  || (txtCustModelDesc.Text == CUSTMODELNAME))
                    {
                        txtShippingSN.Text = "";
                        txtShippingSN.IsEnabled = true;
                        txtMac.IsEnabled = true;
                        txtQty.IsEnabled = true;
                        txtShippingSN.SelectAll();
                        txtShippingSN.Focus();
                        lblMess.Content = "SCAN SSN";
                    }
                    else
                    {
                        MessageError frmMessage = new MessageError();

                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = " DATA WRONG, PLEASE CONFIRM!! ";
                        frmMessage.MessageVietNam = " DỮ LIỆU SẢO VÀO KHÔNG ĐÚNG, VUI LÒNG KIỂM XÁC NHẬN LẠI ";
                        frmMessage.ShowDialog();
                        txtCustModelDesc.SelectAll();
                        txtCustModelDesc.Focus();
                    }
                }
            }
        }

    }
}
