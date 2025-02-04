using Sfc.Core.Extentsions;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeSoft_9Codes.View
{
    /// <summary>
    /// Interaction logic for SetPrintData.xaml
    /// </summary>
    public partial class SetPrintData : UserControl
    {
        private SfcHttpClient sfcClient;
        private string sEmp;
        private string model,typeflag;
        private bool bedit;
        DataTable dt;
        DAL fDal;
        public SetPrintData(SfcHttpClient _sfc,string _emp)
        {
            InitializeComponent();
            sfcClient = _sfc;
            sEmp = _emp;
            dt = new DataTable();
            fDal = new DAL();
        }

        private void dtg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = e.PropertyName.ToUpper().Replace("_", "__");
        }

        private async  void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string sql = "select SSN1,SSN2,SSN3,SSN4,SSN5,MO_NUMBER,MODEL_NAME,MAC1,MAC2,MAC3,MAC4,MAC5,MODEL_FLAG,IN_STATION_TIME,SSN12 from sfism4.r_print_input_t where mo_number='PRINTDATA'  ";
            dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
            if (dt.Rows.Count >0)
            {
                dtg.DataContext = dt;
            }
            
        }

        private async void updatedatagrid()
        {
            string sql = "select SSN1,SSN2,SSN3,SSN4,SSN5,MO_NUMBER,MODEL_NAME,MAC1,MAC2,MAC3,MAC4,MAC5,MODEL_FLAG,IN_STATION_TIME,SSN12 from sfism4.r_print_input_t where mo_number='PRINTDATA'  ";
            dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
            if (dt.Rows.Count > 0)
            {
                dtg.DataContext = dt;
            }
        }

        private async void btnInsert_Click(object sender, RoutedEventArgs e)
        {
			if (txtModel.Text == "")
			{
				MessageBox.Show("Model Name rỗng | Model Name is null");
				txtModel.Focus();
				txtModel.SelectAll();
				return;
			}
			if (grbTypeFlag.Text == "")
			{
				MessageBox.Show("Model Flag rỗng | Model Flag is null");
				return;
			}

            string ssql = "select * from sfis1.c_model_desc_t where model_name='" + txtModel.Text + "'  ";
            dt = await fDal.ExcuteSelectSQL(ssql, sfcClient);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Tên hàng chưa có trong hệ thống, gọi IT thiết lập hàng mới | Model name not exist, call IT " + txtModel.Text);
                return;
            }
            ssql = "select * from sfism4.r_print_input_t where model_name='" + txtModel.Text + "' and mo_number='PRINTDATA' ";
            dt = await fDal.ExcuteSelectSQL(ssql, sfcClient);
            if (dt.Rows.Count == 0)
            {
                ssql = "insert into sfism4.r_print_input_t  (ssn1,ssn2,ssn3,ssn4,ssn5,ssn12,mo_number,model_name, mac1 , mac2 , mac3,mac4,mac5,in_station_time,model_flag,move_flag,print_flag)   values ('','','','','','','PRINTDATA', '" + txtModel.Text + "','','','', '','',sysdate+5000,'" + grbTypeFlag.Text + "','X','Y' )";
                await fDal.ExcuteNonQuerySQL(ssql, sfcClient);
                //db.DoNonQuerySQL(ssql);
                string loginfo = txtModel.Text +" MODEL_FLAG:" + grbTypeFlag.Text.ToString();
                insertLogTable(sEmp, "PrintDataInput", "INSERT", loginfo);
                MessageBox.Show("Insert OK!!!");
                updatedatagrid();

            }
			else
			{
				MessageBox.Show("Đã tồn tại, chỉ có thể sửa | Has exists, Please modify!!");
				txtModel.SelectAll();
			}
		}

        private async void txtModel1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                string sql = $"select SSN1,SSN2,SSN3,SSN4,SSN5,MO_NUMBER,MODEL_NAME,MAC1,MAC2,MAC3,MAC4,MAC5,MODEL_FLAG,IN_STATION_TIME,SSN12 from sfism4.r_print_input_t where mo_number='PRINTDATA' and model_name = '{txtModel1.Text.Trim()}' ";
                dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
                if (dt.Rows.Count > 0)
                {
                    dtg.DataContext = dt;
                }
                else
                {
                    MessageBox.Show("No data");
                }
            }
        }

        private async void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (!bedit)
            {
                if (dtg.SelectedItems.Count > 0)
                {
                    DataRowView row = (DataRowView)dtg.SelectedItem;
                    model = row["model_name"].ToString();
                    typeflag = row["model_flag"].ToString();
                    txtModel.Text = model;
                    grbTypeFlag.Text = typeflag;
                    txtModel.IsEnabled = false;
                    btnInsert.IsEnabled = false;
                    btnDelete.IsEnabled = false;
                    bedit = true;
                }
                else
                {
                    MessageBox.Show("Chưa chọn dữ liệu cần update | Please choose item need modify", "PrintDataInput", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                string ssql = "update sfism4.r_print_input_t set model_flag='" + grbTypeFlag.Text + "'   where model_name='" + txtModel.Text.Trim() + "' and mo_number='PRINTDATA' ";
                await fDal.ExcuteNonQuerySQL(ssql, sfcClient);
                //db.DoNonQuerySQL(ssql);
                string loginfo = txtModel.Text + "UPDATE MODEL_FLAG: "+ typeflag +"->" + grbTypeFlag.Text.ToString();
                insertLogTable(sEmp, "PrintDataInput", "UPDATE", loginfo);
                txtModel.Text = "";
                grbTypeFlag.Text = "";
                txtModel.IsEnabled = true;
                btnInsert.IsEnabled = true;
                btnDelete.IsEnabled = true;
                bedit = false;
                MessageBox.Show("update success!");
                updatedatagrid();
            }
            
            
        }

        private void dtg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dtg.SelectedItems.Count > 0)
            {
                DataRowView row = (DataRowView)dtg.SelectedItem;
                txtModel.Text = row["model_name"].ToString();
                grbTypeFlag.Text = row["model_flag"].ToString();
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dtg.SelectedItems.Count > 0)
            {
                DataRowView row = (DataRowView)dtg.SelectedItem;
                txtModel.Text = row["model_name"].ToString();
                grbTypeFlag.Text = row["model_flag"].ToString();
            }
            else
            return;
            string ssql = "select * from sfism4.r_print_input_t where model_name='" + txtModel.Text + "' and mo_number='PRINTDATA' ";
           dt =  await fDal.ExcuteSelectSQL(ssql, sfcClient);
            if (dt.Rows.Count> 0)
            {
                MessageBoxResult result =  MessageBox.Show("Bạn chắc chắn muốn xóa Model:" + txtModel.Text, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if(result == MessageBoxResult.OK)
                {
                    ssql = "delete sfism4.r_print_input_t   where model_name='" + txtModel.Text.Trim() + "' and mo_number='PRINTDATA' ";
                    await fDal.ExcuteNonQuerySQL(ssql, sfcClient);
                    //db.DoNonQuerySQL(ssql);

                    string loginfo = "DELETE MODEL:"+ txtModel.Text + " MODEL_FLAG:" + grbTypeFlag.Text.ToString();
                  await insertLogTable(sEmp, "PrintDataInput", "DELETE", loginfo);
                    MessageBox.Show("Delete OK!!!");
                    updatedatagrid();
                }
                
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtModel.Text = "";
            grbTypeFlag.Text = "";
            txtModel.IsEnabled = true;
            btnInsert.IsEnabled = true;
            btnDelete.IsEnabled = true;
            bedit = false;
        }

        public async Task<bool> insertLogTable(string emp_no, string prg_name, string actiontype, string actiondesc)
        {
            string sMac = MainWindow.GetMACAddress();
            string sIP = MainWindow.GetLocalIPAddress();
            string sMacIP = ";MAC:" + sMac + ";IP:" + sIP;
            actiondesc = actiondesc + sMacIP;
            try
            {
                string strSQL = " Insert into sfism4.r_system_log_t " +
                                " (EMP_NO,PRG_NAME,ACTION_TYPE,ACTION_DESC) " +
                                " values('" + emp_no + "','" + prg_name + "','" + actiontype + "','" + actiondesc + "') ";
                await fDal.ExcuteSelectSQL(strSQL, sfcClient);
                return (true);
            }
            catch
            {
                return (false);
            }
        }

        
    }
}
