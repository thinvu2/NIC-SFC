using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CHK_LICENSE
{
    public partial class frmParam : Form
    {
        public frmParam()
        {
            InitializeComponent();
        }
        public DataTable dt;
        public frmParam(DataTable inDt)
        {
            InitializeComponent();
            dt = inDt;
        }

        private void frmParam_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = dt;
        }
    }
}
