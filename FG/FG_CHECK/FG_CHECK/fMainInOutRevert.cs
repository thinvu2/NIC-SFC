using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Diagnostics;
using WIA;

namespace FG_CHECK
{
    public partial class fMainInOutRevert : Form
    {
        public String  sql, GD, EmpNo;
        private string checkSum ;
        private string resultData;
        public static string DB;
        public SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string loginApiUri = "";
        public string loginDB = "";
        public string empNo = "";
        public string empName = "";
        public string empPass = "";
        public string inputLogin = "";
        public string rework = "";
        public int passqty;
        public string CUST_PO;
        public static bool  is3s = false;

        public System.Data.DataTable dataTable = new System.Data.DataTable();
        public static string lang = "";//VNI/ENG
        private Ini ini;
        DataTableReader mdr;
        fDal dal;

        public fMainInOutRevert(SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            sfcClient = _sfcClient;
            dal = new fDal();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //SfcSubclass(txt_sn.Handle);
                loadlisview();
                grb_check.Enabled = false;
                txt_emp.Enabled = false;
                txt_emp.Text = Form1.empNO;
                txt_sn.Enabled = true;
                txt_sn.Focus();
                txt_mo.Enabled = false;
                txt_model.Enabled = false;
                textmo.Visible = false;
                label5.Visible = false;
                string host_name = Dns.GetHostName();
                string myip = Dns.GetHostByName(host_name).AddressList[0].ToString();
                string mac = "";
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up)
                    {
                        mac += String.Join(":", nic.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")).ToArray());
                        break;
                    }
                }
                ts_status.Text = "    " + "Design By TTC                                                                                                            ";
                ts_ip.Text = "IP : " + myip + "                                                                                                  ";
                ts_mac.Text = "Mac address: " + mac;

                ini = new Ini(System.Windows.Forms.Application.StartupPath + "\\SFIS_AMS.INI");
                lang = ini.IniReadValue("MainSection", "LANG");
                if (lang == "ENG")
                {
                    englishToolStripMenuItem.Checked = true;
                    tiếngViệtToolStripMenuItem.Checked = false;
                }
                else
                {
                    englishToolStripMenuItem.Checked = false;
                    tiếngViệtToolStripMenuItem.Checked = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }
        void loadlisview ()
        {
           
             listView1.Columns[0].Width = (int)(listView1.Width * 150);
             listView1.Columns[1].Width = (int)(listView1.Width * 150);
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = false;
        }
        private async void Txt_sn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (checkFQAToolStripMenuItem.Checked == true)
                {
                    string sn = txt_sn.Text;
                    string pass = "FALSE";
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if(sn == listView1.Items[i].Text)
                        {
                            if(is3s == true)
                            {
                                string sn_input = Interaction.InputBox("INPUT PO NO.", "INPUT PO NO", "", -1, -1);
                                if(sn_input != CUST_PO)
                                {
                                    setMessage("CUST_PO false|CUST_PO không đúng!");
                                    return;
                                }
                            }
                            listView1.Items[i].Remove();
                            passqty--;
                            pass = "TRUE";
                        }
                    }
                    if(pass == "TRUE" && passqty == 0)
                    {
                        setMessage("FINISH|KIỂM TRA THÀNH CÔNG");
                        string sqlpass = string.Format(@"INSERT INTO SFISM4.Z_CQA_CHECK_T(MO_NUMBER, MODEL_NAME, IMEI, MCARTON_NO, GROUP_CQA, IN_STATION_TIME)
                        SELECT DISTINCT MO_NUMBER,MODEL_NAME,IMEI,MCARTON_NO,'PASS',SYSDATE FROM SFISM4.Z107 WHERE IMEI = '{0}'",textmo.Text);
                        await sfcClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sqlpass, SfcCommandType = SfcCommandType.Text });
                        Btn_recover_Click(sender,e);
                        return;
                    }else if (pass == "TRUE" && passqty > 0)
                    {
                        setMessage("OK|OK");

                        txt_sn.Focus();
                        txt_sn.SelectAll();
                        return;
                    }
                    else
                    {
                        setMessage("INPUT DATA FALSE!|NHẬP VÀO DỮ LIỆU KHÔNG ĐÚNG!");
                        return;
                    }

                }
                else if (revertRMAToolStripMenuItem.Checked == true)
                {
                    checksn();
                }
                else
                {
                    checksn();
                    checkcarton();
                    checkimei();
                }
                btn_recover.Enabled = true;
            }
            //var deviceManager = new DeviceManager();
            //// Loop through the list of devices
            //for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            //{
            //    // Skip the device if it's not a scanner
            //    if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
            //    {
            //        if (e.KeyChar == 13)
            //        {
            //            if (revertRMAToolStripMenuItem.Checked == true)
            //            {
            //                checksn();
            //            }
            //            else
            //            {
            //                checksn();
            //                checkcarton();
            //                checkimei();
            //            }
            //            btn_recover.Enabled = true;
            //        }
            //    }

            //}
        }
        public async void checksn()
        {
            if (rd_sn.Checked || revertAddMoToolStripMenuItem.Checked == true || revertRMAToolStripMenuItem.Checked == true)
            {
                {
                    sql = $"SELECT * FROM SFISM4.Z_WIP_TRACKING_T WHERE (SERIAL_NUMBER='{txt_sn.Text}' OR SHIPPING_SN='{txt_sn.Text}') AND ROWNUM=1";
                    var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Data.Data != null)
                    {
                        txt_mo.Text = qry_Data.Data["mo_number"].ToString();
                        txt_model.Text = qry_Data.Data["model_name"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Please input SN");
                        txt_sn.Focus();
                        txt_sn.SelectAll();
                        return;
                    }
                    int row = listView1.Items.Count;

                        for (int i=0;i<listView1.Items.Count;i++)
                            if (txt_sn.Text==listView1.Items[i].Text)
                            {
                                MessageBox.Show("DUP SN, PLEASE INPUT NEW SN");
                                txt_sn.Focus();
                                txt_sn.SelectAll();
                            return;
                            }
                     sql = $"SELECT count(*) count FROM SFISM4.Z_WIP_TRACKING_T WHERE (SERIAL_NUMBER='{txt_sn.Text}' OR SHIPPING_SN='{txt_sn.Text}') AND ROWNUM=1";
                    var qry_Data_sn = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Data_sn.Data != null)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = txt_sn.Text;
                        listView1.Items.Add(item);

                        ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, (qry_Data_sn.Data["count"].ToString()));
                        item.SubItems.Add(subItem);
                        int totalqty = 0;
                        foreach (ListViewItem it in listView1.Items)
                        {
                            totalqty += int.Parse(it.SubItems[1].Text);
                        }

                        txt_count.Text = totalqty.ToString();
                    }
                    txt_sn.Text = String.Empty;
                    txt_mo.Enabled = false;
                    txt_model.Enabled = false;
                    txt_count.Enabled = false;
                }
            }
        }
        public async void checkcarton()
        {
            if (rd_carton.Checked)
            {
                sql = $"SELECT * FROM SFISM4.Z_WIP_TRACKING_T WHERE (mcarton_no='{txt_sn.Text}' OR carton_no='{txt_sn.Text}')";
                var qry_Data_carton = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Data_carton.Data != null)
                {
                    txt_mo.Text = qry_Data_carton.Data["mo_number"].ToString();
                    txt_model.Text = qry_Data_carton.Data["model_name"].ToString();
                }
                else
                {
                    MessageBox.Show("Please input carton");
                    txt_sn.Focus();
                    txt_sn.SelectAll();
                    return;
                }
                int row = listView1.Items.Count;

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (txt_sn.Text == listView1.Items[i].Text)
                    {
                        MessageBox.Show("DUP CARTON, PLEASE INPUT NEW CARTON");
                        txt_sn.Focus();
                        txt_sn.SelectAll();
                        return;
                    }

                    
                }
                sql = $"SELECT count(*) count FROM SFISM4.Z_WIP_TRACKING_T WHERE (mcarton_no='{txt_sn.Text}' OR carton_no='{txt_sn.Text}') ";
                var qry_Data_carton1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Data_carton1.Data != null)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = txt_sn.Text;
                    listView1.Items.Add(item);

                    ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, (qry_Data_carton1.Data["count"].ToString()));
                    item.SubItems.Add(subItem);
                    int totalqty = 0;
                    foreach (ListViewItem it in listView1.Items)
                    {
                        totalqty += int.Parse(it.SubItems[1].Text);
                    }
                    txt_count.Text = totalqty.ToString();
                }
                txt_sn.Text = String.Empty;
                txt_mo.Enabled = false;
                txt_model.Enabled = false;
                txt_count.Enabled = false;
            }

        }
        public async void checkimei()
        {
            if (rd_imei.Checked)
            {
                //sql = $"SELECT * FROM SFISM4.Z_WIP_TRACKING_T WHERE (imei='{txt_sn.Text}' OR pallet_No='{txt_sn.Text}') ";
                sql = $"SELECT * FROM SFISM4.Z_WIP_TRACKING_T WHERE imei='{txt_sn.Text}' ";
                var qry_Data_imei = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Data_imei.Data != null)
                {
                    txt_mo.Text = qry_Data_imei.Data["mo_number"].ToString();
                    txt_model.Text = qry_Data_imei.Data["model_name"].ToString();
                }
                else
                {
                    MessageBox.Show("Please input pallet");
                    txt_sn.Focus();
                    txt_sn.SelectAll();
                    return;
                }

                int row = listView1.Items.Count;

                for (int i = 0; i < listView1.Items.Count; i++)
                    if (txt_sn.Text == listView1.Items[i].Text)
                    {
                        MessageBox.Show("DUP PALLET, PLEASE INPUT NEW PALLET");
                        txt_sn.Focus();
                        txt_sn.SelectAll();
                        return;
                    }

                //sql = $"SELECT count(*) count FROM SFISM4.Z_WIP_TRACKING_T WHERE (imei='{txt_sn.Text}' OR pallet_No='{txt_sn.Text}') ";
                sql = $"SELECT count(*) count FROM SFISM4.Z_WIP_TRACKING_T WHERE imei='{txt_sn.Text}' ";
                var qry_Data_count_imei = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Data_count_imei.Data != null)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = txt_sn.Text;
                    listView1.Items.Add(item);

                    ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, (qry_Data_count_imei.Data["count"].ToString()));
                    item.SubItems.Add(subItem);

                    string d;

                    int totalqty = 0;
                    foreach (ListViewItem it in listView1.Items)
                    {
                        totalqty += int.Parse(it.SubItems[1].Text);
                    }
                    txt_count.Text = totalqty.ToString();
                }
                txt_sn.Text = String.Empty;
                txt_mo.Enabled = false;
                txt_model.Enabled = false;
                txt_count.Enabled = false;
            }
        }
        public async Task<string> checkTime()
        {
            sql = $"select TO_CHAR(sysdate,'YYYYMMDDHH24MISS') TIME  from dual";
            var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Data.Data != null)
            {
                string data = qry_Data.Data["time"].ToString();
                return data;
            }
            return "FALSE";
        }
        private void ReworkByOldOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (reworkByOldOrderToolStripMenuItem.Checked = true)
            {
                grb_check.Enabled = true;
                button1.Enabled = true;
                revertAddMoToolStripMenuItem.Enabled = false;
                checkInPalletToolStripMenuItem.Enabled = false;
                checkFQAToolStripMenuItem.Enabled = false;
                revertRMAToolStripMenuItem.Enabled = false;
                tOKITTINGSMTToolStripMenuItem.Enabled = false;
                reworkByOldOrderToolStripMenuItem.CheckState = CheckState.Checked;
                lbl_checkin.Text = "FG_CHECK_OUT";
            }

        }
        private void tOKITTINGSMTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grb_check.Enabled = true;
            button1.Enabled = true;
            revertAddMoToolStripMenuItem.Enabled = false;
            reworkByOldOrderToolStripMenuItem.Enabled = false;
            checkInPalletToolStripMenuItem.Enabled = false;
            checkFQAToolStripMenuItem.Enabled = false;
            revertRMAToolStripMenuItem.Enabled = false;
            tOKITTINGSMTToolStripMenuItem.CheckState = CheckState.Checked;
            lbl_checkin.Text = "FG_CHECK_OUT_KITTING";
        }
        private async void revertRMAToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (revertRMAToolStripMenuItem.Checked = true)
            {
                txt_sn.Enabled = true;
                txt_sn.Focus();
                grb_check.Visible = false;
                grb_check.Enabled = true;
                revertAddMoToolStripMenuItem.Enabled = false;
                tOKITTINGSMTToolStripMenuItem.Enabled = false;
                reworkByOldOrderToolStripMenuItem.Enabled = false;
                checkInPalletToolStripMenuItem.Enabled = false;
                revertRMAToolStripMenuItem.CheckState = CheckState.Checked;
                lbl_checkin.Text = "FG_REVERT_RMA";
            }
        }
        private void revertAddMoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (revertAddMoToolStripMenuItem.Checked = true)
            {
                grb_check.Visible = false;
                grb_check.Enabled = true;
                checkFQAToolStripMenuItem.Enabled = false;
                tOKITTINGSMTToolStripMenuItem.Enabled = false;
                revertRMAToolStripMenuItem.Enabled = false;
                checkInPalletToolStripMenuItem.Enabled = false;
                reworkByOldOrderToolStripMenuItem.Enabled = false;
                revertAddMoToolStripMenuItem.CheckState = CheckState.Checked;
                lbl_checkin.Text = "FG_REVERT_RMA";
            }
            label5.Visible = true;
            txt_mo.Visible = true;
            txt_mo.Enabled = true;
            txt_mo.Focus();
        }

        private void checkFQAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkFQAToolStripMenuItem.Checked = true)
            {
                grb_check.Visible = false;
                grb_check.Enabled = true;
                revertAddMoToolStripMenuItem.Enabled = false;
                revertRMAToolStripMenuItem.Enabled = false;
                reworkByOldOrderToolStripMenuItem.Enabled = false;
                checkInPalletToolStripMenuItem.Enabled = false;
                revertAddMoToolStripMenuItem.Enabled = false;
                tOKITTINGSMTToolStripMenuItem.Enabled = false;
                checkFQAToolStripMenuItem.CheckState = CheckState.Checked;
                lbl_checkin.Text = "FQACHECK";
            }
            txt_sn.Enabled = false;
            label5.Visible = true;
            textmo.Enabled = true;
            textmo.Visible = true;
            textmo.Focus();
            lbl_sn.Text = "CartonNo:";
            label5.Text = "PALLET";
        }
        private void Rd_sn_CheckedChanged(object sender, EventArgs e)
        {
            if (rd_sn.Checked)
            {
                lbl_sn.Text = "SERIAL_NUMBER";
                timer1.Stop();
                lbl_run.Text = "PLEASE INPUT SERIAL_NUMBER !!! ";
                lbl_run.Left =  lbl_run.Top;
                for (int i=0;i<listView1.Items.Count;i++)
                listView1.Items[i].Remove();
            }
        }
        private void Rd_carton_CheckedChanged(object sender, EventArgs e)
        {
            if (rd_carton.Checked)
            {
                lbl_sn.Text = rd_carton.Text;
               timer1.Stop();
                lbl_run.Text= "PLEASE  INPUT" + "  " + rd_carton.Text + "  " + "!!!";
                lbl_run.Left = lbl_run.Top;
                for (int i = 0; i < listView1.Items.Count; i++)
                    listView1.Items[i].Remove();
            }
        }
        private void Rd_imei_CheckedChanged(object sender, EventArgs e)
        {
            if (rd_imei.Checked)
            {
                lbl_sn.Text = rd_imei.Text;
                timer1.Stop();
                lbl_run.Text = "PLEASE  INPUT" + "  " + rd_imei.Text + "  " + "!!!";
                lbl_run.Left = lbl_run.Top;
                for (int i = 0; i < listView1.Items.Count; i++)
                    listView1.Items[i].Remove();
            }
        }
        private async void Button1_Click(object sender, EventArgs e)
        {
            if(rework == "")
            {
                rework = "FG_" + await checkTime();
            }
            var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = string.Format("SELECT*FROM SFISM4.Z107 WHERE REWORK_NO='"+ rework + "'"),
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Data.Data != null)
            {
                string Message = "DUP REWORK NO!";
                string title = "BOX";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(Message, title, buttons);
                return;
            }
            empNo = txt_emp.Text;
            string sql = string.Empty;
            string data = string.Empty;
            int row = listView1.Items.Count;
            string datasn = string.Empty;
            
            try{
                if (checkInPalletToolStripMenuItem.Checked)
                {
                    if (lbl_sn.Text != "IMEI")
                    {
                        string Message = "Xin tich IMEI khong tich vao SHIPING_SN or MCARTON_NO";
                        string title = "BOX";
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result = MessageBox.Show(Message, title, buttons);
                        return;
                    }
                }
                if (tOKITTINGSMTToolStripMenuItem.Checked)
                {
                    if (rd_sn.Checked)
                    {
                        string Message = "Xin tich IMEI OR MCARTON_NO khong tich vao SHIPING_SN";
                        string title = "BOX";
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result = MessageBox.Show(Message, title, buttons);
                        return;
                    }
                }
                if (row > 999)
                {
                    return;
                }
                for (int i = 0; i < listView1.Items.Count; i++)
                {                    if (i == 0)
                    {
                        datasn += "'" + listView1.Items[i].Text + "'";
                    }
                    else
                    {
                        datasn += ",'" + listView1.Items[i].Text + "'";
                    }
                }
                if (lbl_sn.Text == "IMEI")
                {
                    //sql = string.Format(@"update sfism4.z107 set rework_no ='"+ rework+"' where IMEI in({0}) or pallet_no in({0})", datasn);
                    sql = string.Format(@"update sfism4.z107 set rework_no ='" + rework + "' where IMEI in({0})", datasn);
                }
                else if (lbl_sn.Text == "MCARTON_NO")
                {
                    sql = string.Format(@"update sfism4.z107 set rework_no ='" + rework + "' where carton_no in({0}) or mcarton_no in({0})", datasn);
                }
                else if (lbl_sn.Text == "SERIAL_NUMBER")
                {
                    sql = string.Format(@"update sfism4.z107 set rework_no ='" + rework + "' where serial_number in({0}) or shipping_sn in({0})", datasn);
                }
                await sfcClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (revertRMAToolStripMenuItem.Checked)
                {
                    data = "IN_FUNC:RMA|IN_SUB_FUNC:REVERTSTOCKIN|EMP_NO:" + empNo + "|IN_REWORKNO:" + rework;
                }
                else if (revertAddMoToolStripMenuItem.Checked)
                {
                    data = "IN_FUNC:RMA|IN_SUB_FUNC:REVERTSTOCKIN|IN_MO:" + txt_mo.Text + "|EMP_NO:" + empNo + "|IN_REWORKNO:" + rework;
                }
                else if (reworkByOldOrderToolStripMenuItem.Checked)
                {
                    data = "IN_FUNC:CHECKOUT|IN_SUB_FUNC:DELCARTONNO|EMP_NO:" + empNo + "|IN_REWORKNO:" + rework;
                }
                else if (checkInPalletToolStripMenuItem.Checked)
                {
                    data = "IN_FUNC:CHECKOUT|IN_SUB_FUNC:NOTDELCARTONNO|EMP_NO:" + empNo + "|IN_REWORKNO:" + rework;
                }else if (tOKITTINGSMTToolStripMenuItem.Checked)
                {
                    data = "IN_FUNC:CHECKOUT|IN_SUB_FUNC:TO-KITTINGSMT|EMP_NO:" + empNo + "|IN_REWORKNO:" + rework;
                }
                var resultsp = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.SP_FG",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter {Name = "IN_DATA" ,Value = data, SfcParameterDataType = SfcParameterDataType.Varchar2 , SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter {Name = "OUT_DATA" ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output},
                        new SfcParameter {Name = "RES" ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output}
                    }
                });
                if (resultsp.Data != null)
                {
                    dynamic output = resultsp.Data;
                    string RES = output[1]["res"];
                    if (RES.Contains("OK"))
                    {
                        setMessage("OK|OK");
                        Btn_recover_Click(sender, e); 
                    }
                    setMessage(RES);
                    return;
                }
            }
            catch (Exception ex)
            {
                setMessage(ex.ToString());
            }
        }

        private async void CheckInPalletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkInPalletToolStripMenuItem.Checked = true)
            {
                lbl_checkin.Text = "FG_CHECK_OUT";
                grb_check.Enabled = true;
                tOKITTINGSMTToolStripMenuItem.Enabled = false;
                reworkByOldOrderToolStripMenuItem.Enabled = false;
                revertRMAToolStripMenuItem.Enabled = false;
                revertAddMoToolStripMenuItem.Enabled = false;
                checkFQAToolStripMenuItem.Enabled = false;
                revertRMAToolStripMenuItem.Enabled = false;
                checkInPalletToolStripMenuItem.CheckState = CheckState.Checked;
            }       
        }

        // BTNRECOVER
        private async void Btn_recover_Click(object sender, EventArgs e)
        {
            txt_sn.Text = "";
            textmo.Text = "";
            txt_mo.Text = "";
            txt_model.Text = "";
            listView1.Items.Clear();
        }

        public int l=30;    
        private void Timer1_Tick(object sender, EventArgs e)
        {
            string emp = "";
             lbl_run.Text= "WELCOM TO " +empName + " "+ "!!!";
            lbl_run.Left += l;
            if (lbl_run.Left >= this.Width - lbl_run.Width || lbl_run.Left <= 0)
                l = -l;
        }
        private void fMainInOutRevert_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Environment.Exit(0);
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tiếngViệtToolStripMenuItem.Checked = false;
            englishToolStripMenuItem.Checked = true;
            lang = "ENG";
            try { ini.IniWriteValue("MainSection", "LANG", "ENG"); } catch { }
        }
        private void tiếngViệtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tiếngViệtToolStripMenuItem.Checked = true;
            englishToolStripMenuItem.Checked = false;
            tiếngViệtToolStripMenuItem.CheckState = CheckState.Checked;
            lang = "VNI";
            try { ini.IniWriteValue("MainSection", "LANG", "VNI"); } catch { }
        }
        private async void txtmo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string pallet_no = textmo.Text;
                string sql = "";
                if (checkFQAToolStripMenuItem.Checked = true)
                {
                    DataTable dt = new DataTable();
                    sql = string.Format(@"SELECT * FROM SFISM4.Z_CQA_CHECK_T where IMEI ='{0}'", pallet_no);
                    dt = await ExcuteSelectSQL(sql, sfcClient);
                    if (dt.Rows.Count > 0)
                    {
                        setMessage("Pallet has checked CQA|Pallet đã có dữ liệu CQA");
                        return;
                    }
sql =string.Format(@"select distinct a.mcarton_no,a.ctnqty,b.palletqty,decode(c.model_name, '', 'NO3S', '3S') IS3S,CUST_PO,
ROUND((palletqty/100)*10)+1 checkpass
from(
select pallet_no, mcarton_no,model_name,version_code, count(*) ctnqty from sfism4.z107 where  pallet_no ='{0}' group by mcarton_no,pallet_no,model_name, version_code
) a,
(
select pallet_no, count(distinct mcarton_no) palletqty from sfism4.z107 where pallet_no ='{0}' group by pallet_no) b,
(select*from SFIS1.C_TMM_CONFIG_T where customer_name='LABEL3S' and temp5='1') c,
SFISM4.R_DN_CARTON_LINK_T d
where a.pallet_no  = b.pallet_no and a.MODEL_NAME = c.model_name(+) and a.version_code = c.SFC_NAME(+) and a.mcarton_no = d.mcarton_no(+)", pallet_no);
                   
                    dt =await ExcuteSelectSQL(sql, sfcClient);
                    if(dt.Rows.Count > 0)
                    {
                        if(dt.Rows[0]["is3s"].ToString() == "3S")
                        {
                            passqty = Int32.Parse(dt.Rows[0]["palletqty"].ToString());
                            CUST_PO = dt.Rows[0]["CUST_PO"].ToString();
                            if(CUST_PO == "")
                            {
                                setMessage("Print 3S label not yet|Chưa in label 3S");
                                return;
                            }
                            is3s = true;
                        }
                        passqty = Int32.Parse(dt.Rows[0]["checkpass"].ToString());
                    }
                    else
                    {
                        setMessage("Not found Pallet data|Không tìm thấy dữ liệu Pallet");
                        return;
                    }
                    for(int i=0; i< dt.Rows.Count; i++)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = dt.Rows[i]["MCARTON_NO"].ToString();
                        listView1.Items.Add(item);
                        ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, (dt.Rows[i]["ctnqty"].ToString()));
                        item.SubItems.Add(subItem);
                    }
                    txt_sn.Enabled = true;
                    textmo.Enabled = false;
                    txt_sn.Focus();
                }
            }
        }

        private async void txt_emp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DataTable dt;
                string sql = string.Empty;
                if (checkInPalletToolStripMenuItem.Checked = true)
                {
                    sql = string.Format(@"select*from SFIS1.C_PRIVILEGE A,  SFIS1.C_EMP_DESC_T B where  a.PASSW='IPQC' and A.FUN ='LOGIN' and A.PRG_NAME = 'CHECK_IN' AND A.EMP = B.EMP_NO AND b.emp_bc = '{0}' AND ROWNUM <2", txt_emp.Text);
                }
                else
                {
                    sql = string.Format(@"select*from SFIS1.C_PRIVILEGE A,  SFIS1.C_EMP_DESC_T B where   A.FUN ='LOGIN' and A.PRG_NAME = 'PM_FG' AND A.EMP = B.EMP_NO AND b.emp_bc = '{0}' AND ROWNUM <2", txt_emp.Text);
                }
                dt = await dal.ExcuteSelectSQL(sql, sfcClient);
                if (dt.Rows.Count > 0)
                {
                    empNo = dt.Rows[0]["emp_no"].ToString();
                    empName = dt.Rows[0]["emp_name"].ToString();
                    empPass = dt.Rows[0]["emp_bc"].ToString();
                    txt_emp.Text = empName;
                    txt_sn.Enabled = true;
                    txt_sn.Focus();
                }
                else
                {
                    setMessage("EMP no Privilege IPQC!| Mã thẻ không có quyên ipqc!");
                    return;
                }
            }
        }

        public async Task<DataTable> ExcuteSelectSQL(string sql, SfcHttpClient sfcHttpClient)
        {
            DataTable data;
            data = null;
            try
            {
                var datacust = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });


                if (datacust.Data != null)
                {
                    var vardatatabel = JsonConvert.SerializeObject(datacust.Data);
                    data = JsonConvert.DeserializeObject<DataTable>(vardatatabel);
                }
            }
            catch (Exception ex)
            {

            }
            return data;
        }
        public void setMessage(string ms)
        {
            if (lang == "EN")
            {
                lerror.Text = ms.Split('|')[0];
            }
            else
            {
                lerror.Text = ms.Split('|')[1];
            }
        }
    }
}
