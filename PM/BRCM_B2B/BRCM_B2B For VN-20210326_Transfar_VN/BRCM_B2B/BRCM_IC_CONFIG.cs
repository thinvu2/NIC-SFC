using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BRCM_B2B
{
    public partial class BRCM_IC_CONFIG : Form
    {
        DataTable dt = new DataTable();
        private string model_name,sqlstr = "",sqllog="",strDesc="";
        DAL fDal;
        SfcHttpClient sfcClient;
        public BRCM_IC_CONFIG(SfcHttpClient varsfcClient)
        {
            fDal = new DAL();
            sfcClient = varsfcClient;
            InitializeComponent();
        }

        private async void button_delete_Click(object sender, EventArgs e)
        {
            MessageBox.Show("delete just need input model_name!");
            sqlstr = $@" DELETE sfis1.c_brcm_ic_t WHERE model_name='{model_name}'";
            strDesc = "Model:"+ model_name+"|MAC:"+Form1.zMac+"|IP:"+Form1.zIP;
            sqllog = "insert into SFISM4.R_SYSTEM_PRGLOG_T(emp_no,prg_name,action_type,action_desc,time)";
            sqllog +=" values ('N/A','B2B IC','DELETE','"+ strDesc + "',sysdate)";
            try
            {
                await fDal.ExcuteNonQuerySQL(sqllog, sfcClient);
                await fDal.ExcuteNonQuerySQL(sqlstr, sfcClient);
                MessageBox.Show("Delete Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete fail! " + ex.Message);
            }
        }

        private async void button_modify_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_model_name.Text.Trim()))
            {
                MessageBox.Show("model_name can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_chip_department.Text.Trim()))
            {
                MessageBox.Show("chip_department can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_comp_item.Text.Trim()))
            {
                MessageBox.Show("comp_item can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(txb_vender_item.Text.Trim()))
            {
                MessageBox.Show("vender_item can't be null!");
                return;
            }
            Regex rx = new Regex("^[1-9]$");
            if (string.IsNullOrEmpty(textBox_comp_qty.Text.Trim()) || !rx.IsMatch(textBox_comp_qty.Text.Trim()))
            {
                MessageBox.Show("comp_qty error!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_emp.Text.Trim()))
            {
                MessageBox.Show("Please input your emp_no!");
                return;
            }
            sqlstr = $@" UPDATE sfis1.c_brcm_ic_t SET 
                    model_name='{textBox_model_name.Text.Trim()}',component_item='{textBox_comp_item.Text.Trim()}',vender_item='{txb_vender_item.Text.Trim()}',component_qty='{textBox_comp_qty.Text.Trim()}',emp_no='{textBox_emp.Text.Trim()}',chip_department_code='{textBox_chip_department.Text.Trim()}' 
                        WHERE model_name='{model_name}'";
            strDesc = "Model:" + textBox_model_name.Text.Trim()+ ",Component:"+ textBox_comp_item.Text.Trim()+ ",Vender:"+ txb_vender_item.Text.Trim()+",Qty:"+ textBox_comp_qty.Text.Trim()+",Emp:"+ textBox_emp.Text.Trim()+",Chip:"+ textBox_chip_department.Text.Trim() + "|MAC:" + Form1.zMac + "|IP:" + Form1.zIP;
            sqllog = "insert into SFISM4.R_SYSTEM_PRGLOG_T(emp_no,prg_name,action_type,action_desc,time)";
            sqllog += " values ('N/A','B2B IC','MODIFY','" + strDesc + "',sysdate)";
            try
            {
                await fDal.ExcuteNonQuerySQL(sqllog, sfcClient);
                await fDal.ExcuteNonQuerySQL(sqlstr, sfcClient);
                MessageBox.Show("Modify Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data modify fail! " + ex.Message);
            }
        }

        private async void button_insert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_model_name.Text.Trim()))
            {
                MessageBox.Show("model_name can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_chip_department.Text.Trim()))
            {
                MessageBox.Show("chip_department can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_comp_item.Text.Trim()))
            {
                MessageBox.Show("comp_item can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(txb_vender_item.Text.Trim()))
            {
                MessageBox.Show("vender_item can't be null!");
                return;
            }
            Regex rx = new Regex("^[1-9]$");
            if (string.IsNullOrEmpty(textBox_comp_qty.Text.Trim())|| !rx.IsMatch(textBox_comp_qty.Text.Trim()))
            {
                MessageBox.Show("comp_qty error!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_emp.Text.Trim()))
            {
                MessageBox.Show("Please input your emp_no!");
                return;
            }
            sqlstr = $@" INSERT INTO sfis1.c_brcm_ic_t(model_name,component_item,vender_item,component_qty,emp_no,chip_department_code)
                                            VALUES('{textBox_model_name.Text.Trim()}','{textBox_comp_item.Text.Trim()}','{txb_vender_item.Text.Trim()}','{textBox_comp_qty.Text.Trim()}','{textBox_emp.Text.Trim()}','{textBox_chip_department.Text.Trim()}')";
            strDesc = "Model:" + textBox_model_name.Text.Trim() + ",Component:" + textBox_comp_item.Text.Trim() + ",Vender:" + txb_vender_item.Text.Trim() + ",Qty:" + textBox_comp_qty.Text.Trim() + ",Emp:" + textBox_emp.Text.Trim() + ",Chip:" + textBox_chip_department.Text.Trim() + "|MAC:" + Form1.zMac + "|IP:" + Form1.zIP;
            sqllog = "insert into SFISM4.R_SYSTEM_PRGLOG_T(emp_no,prg_name,action_type,action_desc,time)";
            sqllog += " values ('N/A','B2B IC','NEW','" + strDesc + "',sysdate)";
            try
            {
                await fDal.ExcuteNonQuerySQL(sqllog, sfcClient);
                await fDal.ExcuteNonQuerySQL(sqlstr, sfcClient);
                MessageBox.Show("Insert Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data insert fail! " + ex.Message);
            }
            
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.CurrentCell.RowIndex;
            int cell = dataGridView1.CurrentCell.ColumnIndex;
            if (dataGridView1.Rows[row].Cells[0].Value.ToString() == "PICK UP")
            {
                textBox_model_name.Text= model_name = dataGridView1.Rows[row].Cells[1].Value.ToString();
                textBox_comp_item.Text = dataGridView1.Rows[row].Cells[2].Value.ToString();
                txb_vender_item.Text = dataGridView1.Rows[row].Cells[3].Value.ToString();
                textBox_comp_qty.Text = dataGridView1.Rows[row].Cells[4].Value.ToString();
                textBox_emp.Text = dataGridView1.Rows[row].Cells[5].Value.ToString();
                textBox_chip_department.Text = dataGridView1.Rows[row].Cells[6].Value.ToString();
            }
        }

        private async void button_query_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_model_name.Text.Trim()))
            {
                sqlstr = $@" SELECT 'PICK UP',model_name,component_item,vender_item,component_qty,emp_no,chip_department_code FROM sfis1.c_brcm_ic_t ";
                dt = await fDal.ExcuteSelectSQL(sqlstr, sfcClient );
            }
            else
            {
                sqlstr = $@" SELECT 'PICK UP',model_name,component_item,vender_item,component_qty,emp_no,chip_department_code FROM sfis1.c_brcm_ic_t WHERE model_name='{textBox_model_name.Text.Trim()}' ";
                dt = await fDal.ExcuteSelectSQL(sqlstr, sfcClient );
            }
            dataGridView1.DataSource = dt;
            
        }
    }
}
