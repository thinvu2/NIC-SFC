using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Sfc.Library.HttpClient;

namespace GemaltoRoast
{
    public partial class StationSetup : Form
    {
        SfcHttpClient sfcClient;
        UserService fdal;
        StationService sdal;
        public StationSetup(SfcHttpClient _sfcHttpClient)
        {
            InitializeComponent();
            sfcClient = _sfcHttpClient;
            fdal = new UserService();
            sdal = new StationService();
        }
        //Load
        private async void StationSetup_Load(object sender, EventArgs e)
        {
            DataTable dt =await sdal.GetAllStation(sfcClient);
            cmbLineName.DataSource = dt;
            cmbLineName.DisplayMember = "LINE_NAME";
            cmbLineName.ValueMember = "LINE_NAME";

            string LineName = cmbLineName.Text.ToString() ;
            DataTable dt1 = await sdal.GetGetLineStation(LineName, sfcClient);
            dgvStation.DataSource = dt1;
        }
        //Cancel
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            MainForm MF = new MainForm();
            MF.Show();
        }
        //OK
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dgvStation.SelectedRows.Count > 0)
            {
                string line_Name = this.cmbLineName.Text.ToString();
                string section_Name = this.dgvStation.SelectedRows[0].Cells[0].Value.ToString();
                string group_Name = this.dgvStation.SelectedRows[0].Cells[1].Value.ToString();
                string station_Name = this.dgvStation.SelectedRows[0].Cells[2].Value.ToString();

                Comm.SetConfigValue("line_Name", line_Name);
                Comm.SetConfigValue("section_Name", section_Name);
                Comm.SetConfigValue("group_Name", group_Name);
                Comm.SetConfigValue("station_Name", station_Name);

                // if(group_Name )
                Control[] control = Controls.Find("lblStation", true);
                if (control.Length > 0)
                {
                    ((Label)Controls.Find("lblStation", true)[0]).Text = line_Name + " " + station_Name;
                }
                this.Close();
                MainForm MF = new MainForm();
                MF.Show();
            }
        }

        private async void cmbLineName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string LineName = cmbLineName.Text.ToString();
            DataTable dt =await sdal.GetGetLineStation(LineName, sfcClient);
            dgvStation.DataSource = dt;
        }
    }
}
