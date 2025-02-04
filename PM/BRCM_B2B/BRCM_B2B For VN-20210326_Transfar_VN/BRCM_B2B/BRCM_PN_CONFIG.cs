using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BRCM_B2B
{
    public partial class BRCM_PN_CONFIG : Form
    {
        DataTable dt = new DataTable();
        private string model_name, sqlstr = "", sqllog = "", strDesc = "";
        SfcHttpClient sfcClient;
        DAL fDal = new DAL();
        public BRCM_PN_CONFIG(SfcHttpClient varsfcClient)
        {
            InitializeComponent();
            sfcClient = varsfcClient;
            fDal = new DAL();
        }

        private async void buttonQuery_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_model_name.Text.Trim()))
            {
                sqlstr = $@" SELECT 'PICK UP',model_name,broadcom_pn,department_code,sotediag_version,box_code,box_weight FROM sfis1.c_brcm_pn_t ";
                dt = await fDal.ExcuteSelectSQL(sqlstr, sfcClient );
            }
            else
            {
                sqlstr = $@" SELECT 'PICK UP',model_name,broadcom_pn,department_code,sotediag_version,box_code,box_weight FROM sfis1.c_brcm_pn_t WHERE model_name='{textBox_model_name.Text.Trim()}' ";
                dt = await fDal.ExcuteSelectSQL(sqlstr, sfcClient);
            }
            dataGridView1.DataSource = dt;
        }

        private async void button_insert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_model_name.Text.Trim()))
            {
                MessageBox.Show("model_name can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_boradcom_pn.Text.Trim()))
            {
                MessageBox.Show("boradcom_pn can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_dep_code.Text.Trim()))
            {
                MessageBox.Show("chip_depart_ment_code can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_sdversion.Text.Trim()))
            {
                MessageBox.Show("sotediag_version can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_box_code.Text.Trim()))
            {
                MessageBox.Show("box_code can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_box_weight.Text.Trim()))
            {
                MessageBox.Show("box_weight can't be null!");
                return;
            }
            sqlstr = $@" INSERT INTO sfis1.c_brcm_pn_t(model_name,broadcom_pn,department_code,sotediag_version,box_code,box_weight)
                                            VALUES('{textBox_model_name.Text.Trim()}','{textBox_boradcom_pn.Text.Trim()}','{textBox_dep_code.Text.Trim()}','{textBox_sdversion.Text.Trim()}','{textBox_box_code.Text.Trim()}','{textBox_box_weight.Text.Trim()}')";
            strDesc = "Model:" + textBox_model_name.Text.Trim() + ",PN:" + textBox_boradcom_pn.Text.Trim() + ",DCode:" + textBox_dep_code.Text.Trim() + ",Ver:" + textBox_sdversion.Text.Trim() + ",BCode:" + textBox_box_code.Text.Trim() + ",Weight:" + textBox_box_weight.Text.Trim() + "|MAC:" + Form1.zMac + "|IP:" + Form1.zIP;
            sqllog = "insert into SFISM4.R_SYSTEM_PRGLOG_T(emp_no,prg_name,action_type,action_desc,time)";
            sqllog += " values ('N/A','B2B PN','NEW','" + strDesc + "',sysdate)";
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

        private async void button_modify_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_model_name.Text.Trim()))
            {
                MessageBox.Show("model_name can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_boradcom_pn.Text.Trim()))
            {
                MessageBox.Show("boradcom_pn can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_dep_code.Text.Trim()))
            {
                MessageBox.Show("department_code can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_sdversion.Text.Trim()))
            {
                MessageBox.Show("sotediag_version can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_box_code.Text.Trim()))
            {
                MessageBox.Show("box_code can't be null!");
                return;
            }
            if (string.IsNullOrEmpty(textBox_box_weight.Text.Trim()))
            {
                MessageBox.Show("box weight can't be null!");
                return;
            }
            sqlstr = $@" UPDATE sfis1.c_brcm_pn_t SET 
                    model_name='{textBox_model_name.Text.Trim()}',broadcom_pn='{textBox_boradcom_pn.Text.Trim()}',department_code='{textBox_dep_code.Text.Trim()}',sotediag_version='{textBox_sdversion.Text.Trim()}',box_code='{textBox_box_code.Text.Trim()}',box_weight='{textBox_box_weight.Text.Trim()}'
                         WHERE model_name='{model_name}'";
            strDesc = "Model:" + textBox_model_name.Text.Trim()+",PN:"+ textBox_boradcom_pn.Text.Trim()+",DCode:"+ textBox_dep_code.Text.Trim()+",Ver:"+ textBox_sdversion.Text.Trim()+",BCode:"+ textBox_box_code.Text.Trim()+",Weight:"+ textBox_box_weight.Text.Trim() + "|MAC:" + Form1.zMac + "|IP:" + Form1.zIP;
            sqllog = "insert into SFISM4.R_SYSTEM_PRGLOG_T(emp_no,prg_name,action_type,action_desc,time)";
            sqllog += " values ('N/A','B2B PN','MODIFY','" + strDesc + "',sysdate)";
            try
            {
                await fDal.ExcuteNonQuerySQL(sqllog, sfcClient);
                await  fDal.ExcuteNonQuerySQL(sqlstr, sfcClient);
                MessageBox.Show("Modify Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data modify fail! " + ex.Message);
            }
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.CurrentCell.RowIndex;
            int cell = dataGridView1.CurrentCell.ColumnIndex;
            if (dataGridView1.Rows[row].Cells[0].Value.ToString() == "PICK UP")
            {
                textBox_model_name.Text = model_name = dataGridView1.Rows[row].Cells[1].Value.ToString();
                textBox_boradcom_pn.Text = dataGridView1.Rows[row].Cells[2].Value.ToString();
                textBox_dep_code.Text = dataGridView1.Rows[row].Cells[3].Value.ToString();
                textBox_sdversion.Text = dataGridView1.Rows[row].Cells[4].Value.ToString();
                textBox_box_code.Text = dataGridView1.Rows[row].Cells[5].Value.ToString();
                textBox_box_weight.Text = dataGridView1.Rows[row].Cells[6].Value.ToString();
            }
        }

        private async void button_delete_Click(object sender, EventArgs e)
        {
            MessageBox.Show("delete just need input model_name!");
            sqlstr = $@" DELETE sfis1.c_brcm_pn_t WHERE model_name='{textBox_model_name.Text.Trim()}'";
            strDesc = "Model:" + textBox_model_name.Text.Trim() + "|MAC:" + Form1.zMac + "|IP:" + Form1.zIP;
            sqllog = "insert into SFISM4.R_SYSTEM_PRGLOG_T(emp_no,prg_name,action_type,action_desc,time)";
            sqllog += " values ('N/A','B2B PN','DELETE','" + strDesc + "',sysdate)";
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
    }
}
