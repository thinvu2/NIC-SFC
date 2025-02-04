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
    /// Interaction logic for SetModel.xaml
    /// </summary>
    
    public partial class SetModel : UserControl
    {
        private SfcHttpClient sfcClient;
        private string sEmp;
        private bool bedit;
        private string model ;
        private string typename ;
        private  string typeflag ;
        private string typedesc ;
        DAL fDal;
        DataTable dt;
        public SetModel(SfcHttpClient _sfc,string _emp)
        {
            InitializeComponent();
            sfcClient = _sfc;
            sEmp = _emp;
            fDal = new DAL();
            dt = new DataTable();
        }

        private void dtg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = e.PropertyName.ToUpper().Replace("_", "__");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            
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
                await fDal.ExcuteNonQuerySQL(strSQL, sfcClient);
                return (true);
            }
            catch
            {
                return (false);
            }
        }
        

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string ssql = " select * from sfis1.c_model_confirm_t order by create_date desc";
            dt = await fDal.ExcuteSelectSQL(ssql, sfcClient);
            dtg.DataContext = dt;
        }

        private async void txtMo_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (txtModel1.Text.Trim() != "")
                {
                    string ssql = " select * from sfis1.c_model_confirm_t  where  model_type='" + txtModel1.Text.Trim() + "' ";
                    dt = await fDal.ExcuteSelectSQL(ssql, sfcClient);
                    dtg.DataContext = dt;
                    txtModel1.SelectAll();
                }
            }
        }
        private async void updateTabel()
        {
            string ssql = " select * from sfis1.c_model_confirm_t  order by create_date desc";
            dt = await fDal.ExcuteSelectSQL(ssql, sfcClient);
            dtg.DataContext = dt;
        }
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dtg.SelectedItems.Count > 0)
            {
                DataRowView row = (DataRowView)dtg.SelectedItem;
                string t_model = row["model_type"].ToString();
                string t_typename = row["type_name"].ToString();
                if (t_typename == "CONFIG_SW" || t_typename == "NPIMOCONFIG" || t_typename == "CONFIG_SHIP_VER")
                {
                    MessageBox.Show("Không được xóa config của PQE | Can not delete config of PQE");
                    return;
                }
                if (MessageBox.Show("Bạn chắc chắn muốn xóa model:(" + t_model + ")?", "PrintDataInput", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    string ssql = "delete from sfis1.c_model_confirm_t where model_type='" + t_model + "' and type_name='" + t_typename + "'";
                    await fDal.ExcuteNonQuerySQL(ssql, sfcClient);
                    string loginfo = "DELETE-->model_type:" + t_model + ";type_name:" + t_typename;
                    insertLogTable(sEmp, "PrintDataInput", "DELETE", loginfo);
                    updateTabel();
                }
            }
        }

        private async void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (txtModel.Text == "")
            {
                MessageBox.Show("Model name is null");
                txtModel.Focus();
                txtModel.SelectAll();
                return;
            }
            if (grbTypeName.Text == "")
            {
                MessageBox.Show("Type Name is null");
                return;
            }
            string model = txtModel.Text;
            string typename = grbTypeName.Text;
            string typeflag = txtTypeFlag.Text;
            string typedesc = txtTypeDesc.Text;
            string ssql1 = "select * from sfis1.c_model_desc_t where model_name='" + model + "'  ";
            dt =await fDal.ExcuteSelectSQL(ssql1, sfcClient);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Tên hàng chưa có trong hệ thống, gọi IT thiết lập hàng mới | Model name not exist, call IT "+ model);
                return;
            }

            string ssql = "select * from sfis1.c_model_confirm_t where model_type='" + model + "' and type_name='" + typename + "' ";
            dt = await fDal.ExcuteSelectSQL(ssql, sfcClient);
            if (dt.Rows.Count == 0)
            {
                ssql = "insert into sfis1.c_model_confirm_t values ('" + model + "', '" + typename + "','" + typeflag + "','" + typedesc + "', '" + sEmp + "',sysdate, '') ";
                await fDal.ExcuteNonQuerySQL(ssql, sfcClient);
                string loginfo = txtModel.Text + " type_name:" + grbTypeName.Text + ";type_flag:" + txtTypeFlag.Text + ";type_desc:" + txtTypeDesc.Text;
                insertLogTable(sEmp, "PrintDataInput", "INSERT", loginfo);
                updateTabel();
                MessageBox.Show("Insert OK!");
            }
            else
            {
                MessageBox.Show("TEN HANG DA CON FIG");
                txtModel.SelectAll();
            }
        }

        private async void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (!bedit)
            {
                if (dtg.SelectedItems.Count > 0)
                {
                    DataRowView row = (DataRowView)dtg.SelectedItem;
                    model = row["model_type"].ToString();
                    typename = row["type_name"].ToString();
                    typeflag = row["type_flag"].ToString();
                    typedesc = row["type_desc"].ToString();
                    txtModel.Text = model;
                    txtModel.IsEnabled = false;
                    grbTypeName.Text = typename;
                    txtTypeFlag.Text = typeflag;
                    txtTypeDesc.Text = typedesc;
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
                if (typename == "CONFIG_SW" || typename == "NPIMOCONFIG" || typename == "CONFIG_SHIP_VER")
                {
                    MessageBox.Show("Không được sửa config của PQE | Can not modify config of PQE");
                    return;
                }
                string ssql = "update sfis1.c_model_confirm_t set   type_name='" + grbTypeName.Text + "' ,type_flag='" + txtTypeFlag.Text + "'  ,type_desc='" + txtTypeDesc.Text + "'  ,create_date=sysdate,emp_no='" + sEmp + "'  where model_type='" + txtModel.Text.Trim() + "' and  type_name= '" + typename + "' and type_flag='" + typeflag + "'  and type_desc='" + typedesc + "' ";
               await fDal.ExcuteNonQuerySQL(ssql, sfcClient);
               
                string loginfo = txtModel.Text + " type_name: "+ typename +"-->" + grbTypeName.Text + ";type_flag " + typeflag + "-->" + txtTypeFlag.Text + ";type_desc: " + typedesc + "-->" + txtTypeDesc.Text;
              await  insertLogTable(sEmp, "PrintDataInput", "UPDATE", loginfo);
                txtModel.Text = "";
                grbTypeName.Text = "";
                txtTypeFlag.Text = "";
                txtTypeDesc.Text = "";
                txtModel.IsEnabled = true;
                btnInsert.IsEnabled = true;
                btnDelete.IsEnabled = true;
                bedit = false;
                MessageBox.Show("update success!");
                updateTabel();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtModel.Text = "";
            grbTypeName.Text = "";
            txtTypeFlag.Text = "";
            txtTypeDesc.Text = "";
            txtModel.IsEnabled = true;
            btnInsert.IsEnabled = true;
            btnDelete.IsEnabled = true;
            bedit = false;
        }
    }
}
