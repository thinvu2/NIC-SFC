using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Globalization;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using Sfc.Core.Parameters;

namespace FG_CHECK
{
    public partial class frm_station : Form
    {
        public SfcHttpClient sfcHttpClient;
        public fMainInOutRevert frm_main;
        public System.Data.DataTable dataTable = new System.Data.DataTable();
        public frm_station()
        {
            InitializeComponent();
        }
        public frm_station(fMainInOutRevert _frm_main,SfcHttpClient _sfcHttpClient)
        {
            frm_main = _frm_main;
            sfcHttpClient = _sfcHttpClient;
            InitializeComponent();
        }

        private void Frm_station_Load(object sender, EventArgs e)
        {
            Loadcombobox();
            loaddatagridview();
            //bingding();
            comboBox1.Enabled = false;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        async void Loadcombobox ()
        {
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select distinct line_name from sfis1.c_line_desc_t",
                SfcCommandType = SfcCommandType.Text
            });
            ResouceClass rc = new ResouceClass();
            if (result.Data != null)
            {
                dataTable = rc.ToDataTable<Line_name>(result.Data.ToListObject<Line_name>());
            }
            comboBox1.DisplayMember = "line_name";
            comboBox1.DataSource = dataTable;

        }

        async void loaddatagridview ()
        {
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select distinct STATION_NAME,GROUP_NAME,SECTION_NAME from SFIS1.C_STATION_CONFIG_T where SECTION_NAME ='FG' ORDER BY GROUP_NAME ASC",
                SfcCommandType = SfcCommandType.Text
            });
            ResouceClass rc = new ResouceClass();
            if (result.Data != null)
            {
                dataTable = rc.ToDataTable<stationconfig>(result.Data.ToListObject<stationconfig>());
            }
            
            dataGridView1.DataSource = dataTable;
            bingding();
            txt_group.Enabled = false;
            txt_section.Enabled = false;
            txt_station.Enabled = false;
            dataGridView1.Enabled = false;
        }
        void bingding ()
        {
            txt_section.DataBindings.Clear();
            txt_section.DataBindings.Add("text", dataGridView1.DataSource, "SECTION_NAME");
            txt_group.DataBindings.Clear();
            txt_group.DataBindings.Add("text", dataGridView1.DataSource, "GROUP_NAME");
            txt_station.DataBindings.Clear();
            txt_station.DataBindings.Add("text", dataGridView1.DataSource, "STATION_NAME");
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                comboBox1.Enabled = true;
            }
             else
                comboBox1.Enabled = false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {   

          // label1.Text= dataGridView1.CurrentRow.Cells[1].Value.ToString();
          //  label1.Visible = true;
            frm_main.lbl_checkin.Text = comboBox1.Text + "_" + dataGridView1.CurrentRow.Cells[1].Value.ToString();
            frm_main.lbl_checkin.Visible = true;
            this.Close();

        }
    }
}
