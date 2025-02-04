using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PRE_SCRAP.Resources;

namespace PRE_SCRAP
{
    public partial class Option : Form
    {
        Form opener;
        public Option(Form parentForm)
        {
            InitializeComponent();
            opener = parentForm;
        }


        private void Btn_Scrap_Click(object sender, EventArgs e)
        {
            Get_Station.sSkind = "Scrap";
            Main_Prescrap main = new Main_Prescrap(this);
            main.Show();
            this.Hide();
        }

        private void BT_Turn_Out_Click(object sender, EventArgs e)
        {
            Get_Station.sSkind = "Turn Out";
            Main_Prescrap main = new Main_Prescrap(this);
            main.Show();
            this.Hide();
        }

        private void Option_Shown(object sender, EventArgs e)
        {
            BT_Turn_Out.Focus();
        }

        private void Bnt_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Btn_Back_Click(object sender, EventArgs e)
        {
            this.Close();
            opener.Show();
        }
    }
}
