using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFC_Library;

namespace FG_CHECK
{
    public partial class fDetail : Form
    {
        public EmployeeInfomation empInfo = new EmployeeInfomation();
        public OracleClientDBHelper dbsfis = null;
        private string MyLocation;
        public bool index;
        public fDetail()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            Tcom(btnQuery);
        }

        private void Tcom(Button b)
        {
            ListInvoice.Items.Clear();
            lblCount.Text = "Total Qty: 0";
            string qryDetail = "SELECT DISTINCT INVOICE " +
                " from SFISM4.R_BPCS_INVOICE_T  where tcom = '"+lblShipNo.Text+"'" +
                " and SITE = '"+MyLocation+"' order by invoice";
        }

        private void fDetail_Shown(object sender, EventArgs e)
        {
            if (index == false)
            {
                MyLocation = "TCAB01";
            }
            else
            {
                MyLocation = "HCAB01";
            }
            lblShipNo.Text = "";
            ListInvoice.Items.Clear();
            lblCount.Text = "Total Qty: 0";
            txbData.Text = "";
        }
    }
}
