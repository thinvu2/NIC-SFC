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
    public partial class VisibleForm : Form
    {
        SfcHttpClient sfcClient;
        public VisibleForm(SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            sfcClient = _sfcClient;
        }
        public delegate void SendVisible();
        public SendVisible _sendVisible;
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (await Check_permission(textBox1.Text.Trim()))
            {
                _sendVisible();
                this.Dispose();
            }
            else
            {
                MessageBox.Show("EMP not privilege to visible", "Visible Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
            {
                button1_Click(this, new EventArgs());
            }
        }
        private async Task< bool> Check_permission(string emp_pass)
        {
            R107Service fdal = new R107Service();
            string sql = String.Format("SELECT * FROM SFIS1.C_EMP_DESC_T WHERE INSTR(CLASS_NAME,'V') > 0 AND EMP_PASS = '{0}' OR EMP_BC = '{0}' AND ROWNUM =1", emp_pass);
         
            DataTable dt =await fdal.ExcuteSelectSQL(sql,sfcClient);
            if (dt == null ) return false;
            if (dt.Rows.Count > 0) return true;
            else return false;
        }
    }
}
