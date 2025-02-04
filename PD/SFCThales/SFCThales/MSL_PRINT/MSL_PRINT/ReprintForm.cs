using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSL_PRINT
{
    public partial class Reprint_Form : Form
    {
        SfcHttpClient sfcClient;
        public Reprint_Form(SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            sfcClient = _sfcClient;
        }
        public delegate void SendRePrint(string _data);
        public SendRePrint sendRePrint;
        private async void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
            {
                if(await Check_permission(textBox1.Text.Trim()))
                {
                    textBox1.Enabled = false;
                    groupBox1.Visible = true;
                }
                else
                {
                    MessageBox.Show("EMP not privilege to reprint", "Reprint Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private async Task< bool> Check_permission(string emp_pass)
        {
            string sql = String.Format(@"SELECT * FROM SFIS1.C_PRIVILEGE WHERE PRG_NAME = 'MSL_PRINT' AND FUN ='REPRINT' AND EMP = (SELECT EMP_NO FROM SFIS1.C_EMP_DESC_T WHERE EMP_PASS = '{0}' OR EMP_BC = '{0}' AND ROWNUM = 1)", emp_pass);
          
            R107Service fdal = new R107Service();
            DataTable dt = await fdal.ExcuteSelectSQL(sql, sfcClient);
            return dt.Rows.Count == 0 ? false : true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendRePrint(textBox2.Text.Trim());
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button1_Click(this, new EventArgs());
            }
        }
    }
}
