using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GemaltoRoast
{
    public partial class TraySetup : Form
    {
        MainForm f;
        OperateINI openIni = new OperateINI();
        SfcHttpClient sfcClient;
        public TraySetup(SfcHttpClient _sfcClient )
        {
            InitializeComponent();
            sfcClient = _sfcClient;
            f = new MainForm();
        }

        private void txttrayprefix_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && txttrayprefix.Text.Trim() != "")
            {
                txttraylength.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            f.writeini(txttrayprefix.Text.Trim(), txttraylength.Text.Trim());
            f.readini();
            this.Hide();
        }

        private void txttraylength_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) )
            {
                e.Handled = true;
            }

            if (e.KeyChar == '\r' && txttraylength.Text.Trim() != "")
            {
                button1_Click(sender, e);
            }
        }

        private void TraySetup_Load(object sender, EventArgs e)
        {
            txttrayprefix.Text =  openIni.ReadINI(f.g_INIPath, "TRAY", "PREFIX").ToString().Trim();
            txttraylength.Text = openIni.ReadINI(f.g_INIPath, "TRAY", "LENGTH").ToString().Trim();
        }
    }
}
