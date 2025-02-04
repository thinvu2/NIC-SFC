using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using SFC_Library;
using System.Collections;
using System.Diagnostics;
using Sfc.Library.HttpClient;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient.Helpers;
using HOLD.LogInfo;

namespace HOLD
{
    public partial class AutoHold : Form
    {
        //public EmployeeInfomation _empInfo = new EmployeeInfomation();
        //public OracleClientDBHelper dbsfis = null;

        public SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string empNo = "";

        private FormMain objfrmMain;
        const string errorstring = "Message";
        public int linecount, holdokcount, holdfailcount;
        public string strDB, DBLink, r107str, z107str, r105str, routenamestr, routecontrolstr;
        ArrayList GROUPLIST = new ArrayList();
        ArrayList STATIONLIST = new ArrayList();
        ArrayList MOLIST = new ArrayList();
        ArrayList CARTONLIST = new ArrayList();
        ArrayList PALLETLIST = new ArrayList();
        ArrayList teststationlist = new ArrayList();
        public AutoHold(FormMain frmMain)
        {
            InitializeComponent();
            objfrmMain = frmMain;
            //timer2.Start();
            loginInfor = frmMain.loginInfor;
            string[] digits = Regex.Split(loginInfor, @";");
            connectDB.Text = digits[0].ToString();
            tssEmp.Text = digits[1].ToString();
            empNo = Regex.Split(tssEmp.Text, @"-")[0].Trim();
        }

        private void showMessage(string Message)
        {
            StackFrame CallStack = new StackFrame(1, true);
            MessageBox.Show("Error: " + Message + ", File: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            string emp = "";
            //emp = _empInfo.EmployeeNo;
            emp = empNo;
            if(emp != "")
            {
                btnBack.Enabled = true;
                btnExit.Enabled = true;
                btnRun.Enabled = true;
                timer1.Enabled = false;
            }
        }

        private void AutoHold_Shown(object sender, EventArgs e)
        {
           // tssEmp.Text = string.Format("Login user: {0} - {1}", _empInfo.EmployeeNo, _empInfo.EmployeeName);
            //tssSysdate.Text = "| " + DateTime.Now.ToString();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string emp = "";
            //emp = _empInfo.EmployeeNo;
            emp = empNo;
            if (emp != "")
            {
                btnBack.Enabled = false;
                btnExit.Enabled = false;
                btnRun.Enabled = false;
                timer1.Enabled = false;
                runhold();
            }
        }

        public async void runhold()
        {
            string ssql = "", DOCNO, MODEL_NAME, GROUP_NAME, STATION_NAME, CONDITION_MO, INFORMATION_MO, CONDITION_CARTON,
                INFORMATION_CARTON, CONDITION_PALLET, INFORMATION_PALLET, HOLD_FLAG, CREATE_DATE,
                REQUEST_EMP, REQUEST_EMAIL, CC_EMAIL, HOLDSTR, MAIL_SUBJECT, MAIL_CONTENT1;
            GROUPLIST.Clear();
            STATIONLIST.Clear();
            MOLIST.Clear();
            CARTONLIST.Clear();
            PALLETLIST.Clear();
            teststationlist.Clear();
            int j = 0;
            ssql = "SELECT * FROM SFISM4.R_HOLD_NOTES_LINK_T WHERE RUN_FLAG = '0' ";
            var resultHoldNotes = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (resultHoldNotes.Data != null)
            {
                var a = resultHoldNotes.Data.ToListObject<infHoldNotes>();
                List<infHoldNotes> listHoldNotes = a.Cast<infHoldNotes>().ToList();
                //for (int i = 0; i < listGroup.Count; i++)
                //{
                //    cbbGroup.Items.Add(listGroup[i].GROUP_NAME.ToString());
                //    cbbStation.Items.Add(listGroup[i].GROUP_NAME.ToString());
                //}
            //}
            //DataTable dt = dbsfis.DoSelectQuery(ssql);
            //if(dt.Rows.Count != 0)
            //{
                for (int i = 0; i < listHoldNotes.Count; i++)
                {
                    j = j + 1;
                    textBox1.AppendText("--------------*---------------*--------------*--------------");
                    holdokcount = 0;
                    holdfailcount = 0;
                    DOCNO = listHoldNotes[i].DOCNO.ToString();
                    //DOCNO = dt.Rows[i]["DOCNO"].ToString();
                    MODEL_NAME = listHoldNotes[i].MODEL_NAME.ToString();
                    //MODEL_NAME = dt.Rows[i]["MODEL_NAME"].ToString();
                    GROUP_NAME = listHoldNotes[i].GROUP_NAME.ToString();
                    //GROUP_NAME = dt.Rows[i]["GROUP_NAME"].ToString();
                    GROUPLIST.Add(GROUP_NAME);
                    STATION_NAME = listHoldNotes[i].STATION_NAME.ToString();
                    //STATION_NAME = dt.Rows[i]["STATION_NAME"].ToString();
                    STATIONLIST.Add(STATION_NAME);
                    CONDITION_MO = listHoldNotes[i].CONDITION_MO.ToString();
                    //CONDITION_MO = dt.Rows[i]["CONDITION_MO"].ToString();
                    INFORMATION_MO = listHoldNotes[i].INFORMATION_MO.ToString();
                    //INFORMATION_MO = dt.Rows[i]["INFORMATION_MO"].ToString();
                    MOLIST.Add(INFORMATION_MO);
                    CONDITION_CARTON = listHoldNotes[i].CONDITION_CARTON.ToString();
                    //CONDITION_CARTON = dt.Rows[i]["CONDITION_CARTON"].ToString();
                    INFORMATION_CARTON = listHoldNotes[i].INFORMATION_CARTON.ToString();
                    //INFORMATION_CARTON = dt.Rows[i]["INFORMATION_CARTON"].ToString();
                    CARTONLIST.Add(INFORMATION_CARTON);
                    CONDITION_PALLET = listHoldNotes[i].CONDITION_PALLET.ToString();
                    //CONDITION_PALLET = dt.Rows[i]["CONDITION_PALLET"].ToString();
                    INFORMATION_PALLET = listHoldNotes[i].INFORMATION_PALLET.ToString();
                    //INFORMATION_PALLET = dt.Rows[i]["INFORMATION_PALLET"].ToString();
                    PALLETLIST.Add(INFORMATION_PALLET);
                    HOLD_FLAG = listHoldNotes[i].HOLD_FLAG.ToString();
                    //HOLD_FLAG = dt.Rows[i]["HOLD_FLAG"].ToString();
                    CREATE_DATE = listHoldNotes[i].CREATE_DATE.ToString();
                    //CREATE_DATE = dt.Rows[i]["CREATE_DATE"].ToString();
                    REQUEST_EMP = listHoldNotes[i].REQUEST_EMP.ToString();
                    //REQUEST_EMP = dt.Rows[i]["REQUEST_EMP"].ToString();
                    REQUEST_EMAIL = listHoldNotes[i].REQUEST_EMAIL.ToString();
                    //REQUEST_EMAIL = dt.Rows[i]["REQUEST_EMAIL"].ToString();
                    CC_EMAIL = listHoldNotes[i].CC_EMAIL.ToString();
                    //CC_EMAIL = dt.Rows[i]["CC_EMAIL"].ToString();
                    textBox1.AppendText("\r\nDocNo: " + DOCNO);
                    textBox1.AppendText("\r\nModel: " + MODEL_NAME);
                    dataGridView1.Rows[i].Cells[0].Value = DOCNO;
                    dataGridView1.Rows[i].Cells[1].Value = MODEL_NAME + ';' + GROUP_NAME + ';' + STATION_NAME + ';' + CREATE_DATE + ';' + REQUEST_EMP;
                    if (await findmodel(MODEL_NAME))
                    {
                        textBox1.AppendText("\r\nDB: " + strDB);
                        r107str = "sfism4.r_wip_tracking_t" + DBLink;
                        r105str = "sfism4.r_mo_base_t" + DBLink;
                        z107str = "sfism4.z_wip_tracking_t" + DBLink;
                        routenamestr = "sfis1.c_route_name_t" + DBLink;
                        routecontrolstr = "sfis1.c_route_control_t" + DBLink;

                        if(HOLD_FLAG == "0")
                        {
                            HOLDSTR = "HOLD";
                        }
                        else
                        {
                            HOLDSTR = "UNHOLD";
                        }

                        textBox1.AppendText("\r\nFashion: " + HOLDSTR);

                        if(HOLD_FLAG == "0")
                        {
                            //HOLD PALLET
                            if (PALLETLIST != null)
                            {
                                for (i = 0; i < PALLETLIST.Count; i++)
                                {
                                    if (await finddatatodb(PALLETLIST[i].ToString(), "P"))
                                    {
                                        if (await holdbypallet(PALLETLIST[i].ToString()))
                                        {
                                            textBox1.AppendText("\r\nPallet: " + PALLETLIST[i].ToString() + " Hold OK");
                                        }
                                        else
                                        {
                                            textBox1.AppendText("\r\nPallet: " + PALLETLIST[i].ToString() + " Hold FAIL");
                                        }
                                    }
                                    else
                                    {
                                        textBox1.AppendText("\r\nPallet: " + PALLETLIST[i].ToString() + " Not Found");
                                    }
                                }
                            }

                            //HOLD CARTON
                            if(CARTONLIST != null)
                            {
                                for (i = 0; i < CARTONLIST.Count; i++)
                                {
                                    if (await finddatatodb(CARTONLIST[i].ToString(), "C"))
                                    {
                                        if (await holdbycarton(CARTONLIST[i].ToString()))
                                        {
                                            textBox1.AppendText("\r\nCarton: " + CARTONLIST[i].ToString() + " Hold OK");
                                        }
                                        else
                                        {
                                            textBox1.AppendText("\r\nCarton: " + CARTONLIST[i].ToString() + " Hold FAIL");
                                        }
                                    }
                                    else
                                    {
                                        textBox1.AppendText("\r\nCarton: " + CARTONLIST[i].ToString() + " Not Found");
                                    }
                                }
                            }

                            //HOLD MO
                            if(MOLIST != null)
                            {
                                for (i = 0; i < MOLIST.Count; i++)
                                {
                                    if (await finddatatodb(MOLIST[i].ToString(), "M"))
                                    {
                                        if (await holdbymo(MOLIST[i].ToString()))
                                        {
                                            textBox1.AppendText("\r\nMo: " + MOLIST[i].ToString() + "Hold OK");
                                        }
                                        else
                                        {
                                            textBox1.AppendText("\r\nMo: " + MOLIST[i].ToString() + "Hold FAIL");
                                        }
                                    }
                                    else
                                    {
                                        textBox1.AppendText("\r\nMo: " + MOLIST[i].ToString() + "Not Found");
                                    }
                                }
                            }

                            textBox1.AppendText("\r\nHold Count: " + holdokcount);
                            textBox1.AppendText("\r\nFail Count: " + holdfailcount);
                            dataGridView1.Rows[i].Cells[2].Value = "Hold Count: " + holdokcount;
                            MAIL_SUBJECT = DOCNO + "Hold processed ,Please check ";
                            MAIL_CONTENT1 = "Dear All: \r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "Model: " + MODEL_NAME + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "Group: " + GROUP_NAME + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "Station: " + STATION_NAME + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "MO: " + INFORMATION_MO + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "Carton: " + INFORMATION_CARTON + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "Pallet: " + STATION_NAME + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "Hold Count: " + holdokcount + "\r\n";

                            await SENDMAIL(MAIL_SUBJECT, MAIL_CONTENT1, REQUEST_EMAIL, CC_EMAIL);
                            await UPDATEHOLDNOTE(DOCNO, holdokcount);
                            await SaveLog("HOLD", "HOLDSYSTEM - : " + MAIL_CONTENT1 + "Hold by Holdsystem");
                        }
                        else
                        {
                            //UNHOLD
                            //UNHOLD PALLET
                            if(PALLETLIST != null)
                            {
                                for(i = 0; i < PALLETLIST.Count; i++)
                                {
                                    if (await finddatatodb(PALLETLIST[i].ToString(), "P"))
                                    {
                                        if (await unholdbypallet(PALLETLIST[i].ToString()))
                                        {
                                            textBox1.AppendText("\r\nPallet: " + PALLETLIST[i].ToString() + "Unhold OK");
                                        }
                                        else
                                        {
                                            textBox1.AppendText("\r\nPallet: " + PALLETLIST[i].ToString() + "Unhold Fail");
                                        }
                                    }
                                    else
                                    {
                                        textBox1.AppendText("\r\nPallet: " + PALLETLIST[i].ToString() + "Not Found");
                                    }
                                }
                            }

                            //UNHOLD CARTON
                            if(CARTONLIST != null)
                            {
                                for(i = 0; i < CARTONLIST.Count; i++)
                                {
                                    if (await finddatatodb(CARTONLIST[i].ToString(), "C"))
                                    {
                                        if(await unholdbycarton(CARTONLIST[i].ToString()))
                                        {
                                            textBox1.AppendText("\r\nCarton: " + CARTONLIST[i].ToString() + "Unhold OK");
                                        }
                                        else
                                        {
                                            textBox1.AppendText("\r\nCarton: " + CARTONLIST[i].ToString() + "Unhold Fail");
                                        }
                                    }
                                    else
                                    {
                                        textBox1.AppendText("\r\nCarton: " + CARTONLIST[i].ToString() + "Not Found");
                                    }
                                }
                            }

                            //UNHOLD MO
                            if(MOLIST != null)
                            {
                                for(i = 0; i < MOLIST.Count; i++)
                                {
                                    if (await finddatatodb(MOLIST[i].ToString(), "M"))
                                    {
                                        if (await unholdbymo(MOLIST[i].ToString()))
                                        {
                                            textBox1.AppendText("\r\nMo: " + CARTONLIST[i].ToString() + "Unhold OK");
                                        }
                                        else
                                        {
                                            textBox1.AppendText("\r\nMo: " + CARTONLIST[i].ToString() + "Unhold Fail");
                                        }
                                    }
                                    else
                                    {
                                        textBox1.AppendText("\r\nMo: " + CARTONLIST[i].ToString() + "Not Found");
                                    }
                                }
                            }

                            textBox1.AppendText("\r\nUnhold Count: " + holdokcount);
                            textBox1.AppendText("\r\nFail Count: " + holdfailcount);
                            dataGridView1.Rows[i].Cells[2].Value = "Unhold Count: " + holdokcount;
                            MAIL_SUBJECT = DOCNO + " UnHold processed, Please check ";
                            MAIL_CONTENT1 = "Dear All: \r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "Model: " + MODEL_NAME + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "Group: " + GROUP_NAME + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "Station: " + STATION_NAME + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "MO: " + INFORMATION_MO + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "Carton: " + INFORMATION_CARTON + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "Pallet: " + STATION_NAME + "\r\n";
                            MAIL_CONTENT1 = MAIL_CONTENT1 + "UnHold : " + holdokcount + "\r\n";

                            await SENDMAIL(MAIL_SUBJECT, MAIL_CONTENT1, REQUEST_EMAIL, CC_EMAIL);
                            await UPDATEHOLDNOTE(DOCNO, holdokcount);
                            await SaveLog("UNHOLD", "HOLDSYSTEM - : " + MAIL_CONTENT1 + "UnHold by Holdsystem");
                        }
                    }
                    else
                    {
                        textBox1.AppendText("DB: Not Found");
                    }
                }
            }
            timer1.Enabled = true;
        }

        private async Task<bool> SaveLog(string action, string action_desc)
        {
            var logInfo = new
            {
                TYPE = "SAVELOG",
                EMP_NO = empNo,
                PRG_NAME = "HOLD",
                ACTION_TYPE = action,
                ACTION_DESC = action_desc

            };

            //Tranform it to Json object
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
            try
            {
                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="DATA",Value=logInfo,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                }
                });
                dynamic ads = result.Data;
                string Ok = ads[0]["output"];
                if (Ok.Substring(0, 2) == "OK")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }

        private async Task<bool> unholdbymo(string mo_number)
        {
            string ROUTE_CODE, tmpnextst, TMPSTATION;
            try
            {
                ROUTE_CODE = await getroutecode(mo_number);
                if(ROUTE_CODE != "")
                {
                    if (await unholdroute(ROUTE_CODE, mo_number))
                    {
                        ROUTE_CODE = await getroutecode(mo_number);
                        if(GROUPLIST.IndexOf("SMT") > 0)
                        {
                            tmpnextst = await findnextstation(ROUTE_CODE, "INSP");
                            await unholdbymotostation(mo_number, "HOLD" + tmpnextst);
                        }

                        if(GROUPLIST.IndexOf("FG") > 0)
                        {
                            await unholdbymotostation(mo_number, "FG");
                        }

                        if(GROUPLIST.IndexOf("Assembly") > 0)
                        {
                            if(STATIONLIST.IndexOf("ASSY") > 0)
                            {
                                for(int i = 0; i < STATIONLIST.Count; i++)
                                {
                                    if(STATIONLIST[i].ToString().IndexOf("ASSY") > 0)
                                    {
                                        tmpnextst = await findnextstation(ROUTE_CODE, STATIONLIST[i].ToString());
                                        if(tmpnextst.IndexOf("HOLD") > 0)
                                        {
                                            tmpnextst = tmpnextst.Substring(5, tmpnextst.Length - 4);
                                        }

                                        await unholdbymotostation(mo_number, "HOLD" + tmpnextst);
                                    }
                                }
                            }
                            else
                            {
                                TMPSTATION = await getfirstpackstation(ROUTE_CODE, "ASSY");
                                await unholdbymotostation(mo_number, "HOLD" + TMPSTATION);
                            }
                        }

                        if(GROUPLIST.IndexOf("Packing") > 0)
                        {
                            if(STATIONLIST.IndexOf("PACK") > 0)
                            {
                                for(int i = 0; i < STATIONLIST.Count; i++)
                                {
                                    if(STATIONLIST[i].ToString().IndexOf("PACK") > 0)
                                    {
                                        await unholdbymotostation(mo_number, "HOLD" + STATIONLIST[i].ToString());
                                    }
                                }
                            }
                            else
                            {
                                TMPSTATION = await getfirstpackstation(ROUTE_CODE,"PACK");
                                await unholdbymotostation(mo_number, "HOLD" + TMPSTATION);
                            }
                        }

                        if(GROUPLIST.IndexOf("Test") > 0)
                        {
                            if(teststationlist != null)
                            {
                                for(int i = 0; i < teststationlist.Count; i++)
                                {
                                    await unholdbymotostation(mo_number, "HOLD" + teststationlist[i].ToString());
                                }
                            }
                            else
                            {
                                await unholdbymotostation(mo_number, "HOLD" + getfirstteststation(ROUTE_CODE));
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> unholdbymotostation(string mo_number, string station)
        {
            string ssql = "";
            if(station == "FG")
            {
                ssql = "SELECT * FROM " + z107str + " WHERE MO_NUMBER = '" + mo_number + "' AND WIP_GROUP = 'HOLD'" + station + "'";
            }
            else
            {
                ssql = "SELECT * FROM " + r107str + " WHERE MO_NUMBER = '" + mo_number + "' AND WIP_GROUP = '" + station + "'";
            }
            try
            {
                //DataTable dt = dbsfis.DoSelectQuery(ssql);
                var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });                
                if (result.Data != null)
                {
                    var a = result.Data.ToListObject<infR107>();
                    List<infR107> listSerial = a.Cast<infR107>().ToList();
                    for (int i = 0; i < listSerial.Count; i++)
                    {
                        //if (await unholdbyserial(dt.Rows[0]["SERIAL_NUMBER"].ToString(), station))
                        if(await unholdbyserial(listSerial[i].SERIAL_NUMBER.ToString(), station))
                        {
                            holdokcount = holdokcount + 1;
                        }
                        else
                        {
                            holdfailcount = holdfailcount + 1;
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> unholdroute(string routecode,string mo_number)
        {
            string ssql, tmproutename, TMPROUTECODE;
            bool tmpflag = false;
            //DataTable dt = new DataTable();
            try
            {
                ssql = "SELECT * FROM " + routenamestr + " WHERE ROUTE_CODE = '" + routecode + "'";
                //dt = dbsfis.DoSelectQuery(ssql);
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                var ROUTE = result.Data.ToObject<LogInformation>();
                //tmproutename = dt.Rows[0]["ROUTE_NAME"].ToString();
                tmproutename = ROUTE.ROUTE_NAME;
                if(tmproutename.IndexOf("HOLD") > 0)
                {
                    tmproutename = tmproutename.Substring(1, tmproutename.IndexOf("HOLD") - 1);
                    tmpflag = true;
                }
                //if (!findroutename(tmproutename))
                if (await findroutename(tmproutename) == false)
                {
                    ssql = "SELECT * FROM " + routenamestr + " WHERE ROUTE_NAME='" + tmproutename + "'";
                    //dt = dbsfis.DoSelectQuery(ssql);
                    result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = ssql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    ROUTE = result.Data.ToObject<LogInformation>();
                    //TMPROUTECODE = dt.Rows[0]["ROUTE_CODE"].ToString();
                    TMPROUTECODE = ROUTE.ROUTE_CODE;
                    if (await updateroute(TMPROUTECODE, mo_number))
                    {
                        if (tmpflag)
                        {
                            await deleteholdroute(routecode);
                        }
                    }
                }
                else
                {
                    ssql  = "UPDATE " + routecontrolstr +
                          " SET GROUP_NEXT = DECODE (SUBSTR (GROUP_NEXT, 1, 4), 'HOLD', SUBSTR (GROUP_NEXT, 5, LENGTH (GROUP_NEXT)), GROUP_NEXT) WHERE ROUTE_CODE = '" + routecode + "'";
                    //dbsfis.ExecuteNonQuery(ssql);
                    var sbUpdate = new StringBuilder();
                    sbUpdate.Append(ssql);
                    var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbUpdate.ToString()
                    });
                    ssql = " UPDATE " + routenamestr + " SET ROUTE_NAME = '" + tmproutename + "' WHERE ROUTE_CODE = '" + routecode + "'";
                    //dbsfis.ExecuteNonQuery(ssql);
                    sbUpdate = new StringBuilder();
                    sbUpdate.Append(ssql);
                    resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbUpdate.ToString()
                    });
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> deleteholdroute(string route_code)
        {
            try
            {
                string ssql = "";
                ssql = "DELETE FROM " + routecontrolstr + " WHERE ROUTE_CODE = '" + route_code + "'";
                //dbsfis.ExecuteNonQuery(ssql);
                var sbDelete = new StringBuilder();
                sbDelete.Append(ssql);
                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbDelete.ToString()
                });
                ssql = "DELETE FROM " + routenamestr + " WHERE ROUTE_CODE = '" + route_code + "'";
                //dbsfis.ExecuteNonQuery(ssql);
                sbDelete = new StringBuilder();
                sbDelete.Append(ssql);
                result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbDelete.ToString()
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> unholdbycarton(string carton_no)
        {
            string ssql = "", station = "";
            if(GROUPLIST.IndexOf("FG") > 0)
            {
                ssql = "SELECT * FROM " + z107str + " WHERE CARTON_NO = '" + carton_no + "' OR MCARTON_NO = '" + carton_no + "'";
                //DataTable dt = dbsfis.DoSelectQuery(ssql);
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (result.Data == null)
                {
                    ssql = "SELECT * FROM " + r107str + " WHERE CARTON_NO = '" + carton_no + "' OR MCARTON_NO = '" + carton_no + "'";
                }
                else
                {
                    station = "FG";
                }
            }
            else
            {
                ssql = "SELECT * FROM " + r107str + " WHERE CARTON_NO = '" + carton_no + "' OR MCARTON_NO = '" + carton_no + "'";
            }
            try
            {
                //DataTable dt = dbsfis.DoSelectQuery(ssql);
                var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (result.Data != null)
                {
                    var a = result.Data.ToListObject<infR107>();
                    List<infR107> listSerial = a.Cast<infR107>().ToList();
                    for (int i = 0; i < listSerial.Count; i++)
                    {
                        if (await unholdbyserial(listSerial[i].SERIAL_NUMBER.ToString(), station))
                        //if (unholdbyserial(dt.Rows[0]["SERIAL_NUMBER"].ToString(), station))
                        {
                            holdokcount = holdokcount + 1;
                        }
                        else
                        {
                            holdfailcount = holdfailcount + 1;
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> unholdbypallet(string pallet_no)
        {
            string ssql = "", station = "";
            //DataTable dt = new DataTable();
            if(GROUPLIST.IndexOf("FG") > 0)
            {
                ssql = "SELECT * FROM " + z107str + " WHERE PALLET_NO='" + pallet_no + "' OR IMEI = '" + pallet_no + "'";
                //dt = dbsfis.DoSelectQuery(ssql);
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (result.Data == null)
                {
                    ssql = "SELECT * FROM " + r107str + " WHERE PALLET_NO='" + pallet_no + "' OR IMEI = '" + pallet_no + "'";
                }
                else
                {
                    station = "FG";
                }
            }
            else
            {
                ssql = "SELECT * FROM " + r107str + " WHERE PALLET_NO='" + pallet_no + "' OR IMEI = '" + pallet_no + "'";
            }
            try
            {
                //dt = dbsfis.DoSelectQuery(ssql);
                var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (result.Data != null)
                {
                    var a = result.Data.ToListObject<infR107>();
                    List<infR107> listSerial = a.Cast<infR107>().ToList();
                    for (int i = 0; i < listSerial.Count; i++)
                    {
                        //if (unholdbyserial(dt.Rows[0]["SERIAL_NUMBER"].ToString(), station))
                        if (await unholdbyserial(listSerial[i].SERIAL_NUMBER.ToString(), station))
                        {
                            holdokcount = holdokcount + 1;
                        }
                        else
                        {
                            holdfailcount = holdfailcount + 1;
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> unholdbyserial(string serial_number, string station)
        {
            string ssql = "";
            try
            {
                if (await checunkwip(serial_number, station))
                {
                    if (station == "FG")
                    {
                        ssql = "UPDATE " + z107str + " SET GROUP_NAME = DECODE (SUBSTR (GROUP_NAME, 1, 4), 'HOLD', SUBSTR(GROUP_NAME, 5, LENGTH(GROUP_NAME)), GROUP_NAME), " +
                              " NEXT_STATION = DECODE (SUBSTR (NEXT_STATION, 1, 4), 'HOLD', SUBSTR(NEXT_STATION, 5, LENGTH(NEXT_STATION)), NEXT_STATION), " +
                              " WIP_GROUP = DECODE (SUBSTR (WIP_GROUP, 1, 4), 'HOLD', SUBSTR(WIP_GROUP, 5, LENGTH(WIP_GROUP)), WIP_GROUP) " +
                              " WHERE SERIAL_NUMBER = '" + serial_number + "'";
                        //dbsfis.ExecuteNonQuery(ssql);
                        var sbUpdate = new StringBuilder();
                        sbUpdate.Append(ssql);
                        var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sbUpdate.ToString()
                        });
                    }
                    else
                    {
                        ssql = "UPDATE " + r107str + " SET GROUP_NAME = DECODE (SUBSTR (GROUP_NAME, 1, 4), 'HOLD', SUBSTR(GROUP_NAME, 5, LENGTH(GROUP_NAME)), GROUP_NAME), " +
                              " NEXT_STATION = DECODE (SUBSTR (NEXT_STATION, 1, 4), 'HOLD', SUBSTR(NEXT_STATION, 5, LENGTH(NEXT_STATION)), NEXT_STATION), " +
                              " WIP_GROUP = DECODE (SUBSTR (WIP_GROUP, 1, 4), 'HOLD', SUBSTR(WIP_GROUP, 5, LENGTH(WIP_GROUP)), WIP_GROUP) " +
                              " WHERE SERIAL_NUMBER = '" + serial_number + "'";
                        //dbsfis.ExecuteNonQuery(ssql);
                        var sbUpdate = new StringBuilder();
                        sbUpdate.Append(ssql);
                        var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sbUpdate.ToString()
                        });
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> checunkwip(string serial_number, string station)
        {
            string ssql = "", tmpwip = "";
            if(station == "FG")
            {
                ssql = "SELECT * FROM " + z107str + " WHERE SERIAL_NUMBER='" + serial_number + "'";
            }
            else
            {
                ssql = "SELECT * FROM " + r107str + " WHERE SERIAL_NUMBER='" + serial_number + "'";
            }
            //DataTable dt = dbsfis.DoSelectQuery(ssql);
            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                var WIP = result.Data.ToObject<infR107>();
                //tmpwip = dt.Rows[0]["WIP_GROUP"].ToString();
                tmpwip = WIP.WIP_GROUP;
                if (tmpwip.IndexOf("HOLD") > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> UPDATEHOLDNOTE(string docno, int countout)
        {
            try
            {
                string ssql = "";
                ssql = "UPDATE SFISM4.R_HOLD_NOTES_LINK_T SET RUN_FLAG = '1', " +
                    "RUN_DESC = 'SUCC:" + countout + "' WHERE DOCNO='" + docno + "'";
                //dbsfis.ExecuteNonQuery(ssql);
                var sbUpdate = new StringBuilder();
                sbUpdate.Append(ssql);
                var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> SENDMAIL(string MAIL_SUBJECT, string MAIL_CONTENT1, string MAIL_TO, string MAIL_CC)
        {
            string MAIL_ID = "", MAILSQL = "";
            try
            {
                MAIL_ID = DateTime.Now.ToString("yyyyMMddHHmmsszzzz");
                if(MAIL_TO == "")
                {
                    return false;
                }
                MAILSQL = "INSERT INTO SFIS1.C_MAIL_T " + DBLink +
                        " (MAIL_ID, MAIL_TO, MAIL_FROM,MAIL_CC,MAIL_SUBJECT, MAIL_SEQUENCE, MAIL_CONTENT, MAIL_FLAG, MAIL_PROGRAM) " +
                        "  Values('" +  MAIL_ID + "','" + MAIL_TO + "','HOLD','" + MAIL_CC + "','" + MAIL_SUBJECT + "','0','" + MAIL_CONTENT1 + "','0','HOLD')";
                //dbsfis.ExecuteNonQuery(MAILSQL);
                var sbInsert = new StringBuilder();
                sbInsert.Append(MAILSQL);
                var resultInsert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbInsert.ToString()
                });
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private async Task<bool> holdbymo(string mo_number)
        {
            string route_code, tmpstation, tmpnextst;
            try
            {
                route_code = await getroutecode(mo_number);
                if(route_code != "")
                {
                    if (await holdroute(route_code, mo_number))
                    {
                        route_code = await getroutecode(mo_number);
                        if(GROUPLIST.IndexOf("SMT") > 0)
                        {
                            tmpnextst = await findnextstation(route_code, "INSP");
                            if(tmpnextst.IndexOf("HOLD") > 0)
                            {
                                tmpnextst = tmpnextst.Substring(5,tmpnextst.Length-4);
                            }
                            await holdbymotostation(mo_number, tmpnextst);
                        }

                        if(GROUPLIST.IndexOf("FG") > 0)
                        {
                            await holdbymotostation(mo_number, "FG");
                        }

                        if(GROUPLIST.IndexOf("Assembly") > 0)
                        {
                            if(STATIONLIST.IndexOf("ASSY") > 0)
                            {
                                for(int i = 0; i < STATIONLIST.Count; i++)
                                {
                                    if(STATIONLIST[i].ToString().IndexOf("ASSY") > 0)
                                    {
                                        tmpnextst = await findnextstation(route_code, STATIONLIST[i].ToString());
                                        if(tmpnextst.IndexOf("HOLD") > 0)
                                        {
                                            tmpnextst = tmpnextst.Substring(5, tmpnextst.Length - 4);
                                        }
                                        await holdbymotostation(mo_number, tmpnextst);
                                    }
                                }
                            }
                            else
                            {
                                tmpstation = await getfirstpackstation(route_code, "ASSY");
                               await holdbymotostation(mo_number, tmpstation);
                            }
                        }

                        if(GROUPLIST.IndexOf("Packing") > 0)
                        {
                            if(STATIONLIST.IndexOf("PACK") > 0)
                            {
                                for(int i = 0; i < STATIONLIST.Count; i++)
                                {
                                    if(STATIONLIST[i].ToString().IndexOf("PACK") > 0)
                                    {
                                        await holdbymotostation(mo_number, STATIONLIST[i].ToString());
                                    }
                                }
                            }
                            else
                            {
                                tmpstation = await getfirstpackstation(route_code, "PACK");
                                await holdbymotostation(mo_number, tmpstation);
                            }
                        }

                        if(GROUPLIST.IndexOf("Test") > 0)
                        {
                            if(teststationlist != null)
                            {
                                for(int i = 0; i < teststationlist.Count; i++)
                                {
                                    await holdbymotostation(mo_number, teststationlist[i].ToString());
                                }
                            }
                            else
                            {
                                await holdbymotostation(mo_number, await getfirstteststation(route_code));
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private async Task<bool> holdbymotostation(string mo_number, string station)
        {
            try
            {
                string ssql = "";
                if (station == "FG")
                {
                    ssql = "SELECT * FROM " + z107str + " WHERE MO_NUMBER='" + mo_number + "' AND WIP_GROUP='" + station + "'";
                }
                else
                {
                    ssql = "SELECT * FROM " + r107str + " WHERE MO_NUMBER='" + mo_number + "' AND WIP_GROUP='" + station + "'";
                }
                //DataTable dt = dbsfis.DoSelectQuery(ssql);
                var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });                
                if (result.Data != null)
                {
                    var a = result.Data.ToListObject<infcbxFG>();
                    List<infcbxFG> listSerial = a.Cast<infcbxFG>().ToList();
                    for (int i = 0; i < listSerial.Count; i++)
                    {
                        if(await holdbyserial(listSerial[i].SERIAL_NUMBER.ToString(),station))
                        //if (holdbyserial(dt.Rows[i]["SERIAL_NUMBER"].ToString(), station))
                        {
                            holdokcount = holdokcount + 1;
                        }
                        else
                        {
                            holdfailcount = holdfailcount + 1;
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> holdroute(string route_code, string mo_number)
        {
            string newroutecode, tmpnextst = "", tmpstation;
            ArrayList tmpstrings = new ArrayList();
            try
            {
                if (GROUPLIST != null)
                {
                    newroutecode = await GETNEWROUTE(route_code);
                    if (newroutecode != "")
                    {
                        if (await updateroute(route_code, mo_number))
                        {
                            //HOLD SMT
                            if (GROUPLIST.IndexOf("SMT") > 0)
                            {
                                tmpnextst = await findnextstation(newroutecode, "INSP");
                                if (tmpnextst != "")
                                {
                                    await holdroutenext(newroutecode, tmpnextst);
                                }
                            }
                            //HOLD ASSY
                            if (GROUPLIST.IndexOf("Assembly") > 0)
                            {
                                tmpstation = "ASSY";
                                if (STATIONLIST.IndexOf(tmpstation) > 0)
                                {
                                    tmpstation = getstringstring(STATIONLIST, tmpstation);
                                    if (tmpstation != "")
                                    {
                                        tmpnextst = await findnextstation(newroutecode, tmpstation);
                                    }
                                }
                                else
                                {
                                    tmpnextst = await findnextstation(newroutecode, tmpstation);
                                }
                                if (tmpnextst != "")
                                {
                                    await holdroutenext(newroutecode, tmpnextst);
                                }
                            }

                            if (GROUPLIST.IndexOf("Packing") > 0)
                            {
                                tmpstation = "PACK";
                                if (STATIONLIST.IndexOf(tmpstation) > 0)
                                {
                                    tmpstation = getstringstring(STATIONLIST, tmpstation);
                                    if (tmpstation != "")
                                    {
                                        tmpnextst = tmpstation;
                                    }
                                }
                                else
                                {
                                    tmpnextst = await getfirstpackstation(newroutecode, tmpstation);
                                }
                                if (tmpnextst != "")
                                {
                                    await holdroutenext(newroutecode, tmpnextst);
                                }
                            }

                            //HOLD TEST
                            if (GROUPLIST.IndexOf("Test") > 0)
                            {
                                tmpstrings.Add(checkstationtolist(newroutecode));
                                if (tmpstrings != null)
                                {
                                    for (int i = 0; i < tmpstrings.Count; i++)
                                    {
                                        await holdroutenext(newroutecode, tmpstrings[i].ToString());
                                    }
                                }
                                else
                                {
                                    tmpnextst = await getfirstteststation(newroutecode);
                                    if (tmpnextst != "")
                                    {
                                        await holdroutenext(newroutecode, tmpnextst);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<string> getfirstteststation(string route_code)
        {
            string ssql = "";
            ArrayList tmplist = new ArrayList();
            ssql = "SELECT * FROM " + routecontrolstr +
                " WHERE  ROUTE_CODE='" + route_code + "' AND STEP_SEQUENCE = (SELECT MIN(A.STEP_SEQUENCE) FROM " +
                " SFIS1.C_ROUTE_CONTROL_T A, SFIS1.C_GROUP_CONFIG_T B WHERE " +
                " A.STATE_FLAG = '0' AND A.ROUTE_CODE='" + route_code + "' AND A.GROUP_NEXT = B.GROUP_NAME " +
                " AND B.SECTION_NAME = 'TEST' AND SUBSTR(A.GROUP_NEXT,1,4) NOT IN('PACK','ASSY','STOC'))";
            //DataTable dt = dbsfis.DoSelectQuery(ssql);
            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                return result.Data["GROUP_NEXT"].ToString();
                //return dt.Rows[0]["GROUP_NEXT"].ToString();
            }
            else
            {
                return "";
            }
        }

        private async Task<ArrayList> checkstationtolist(string route_code)
        {
            string ssql = "";
            ssql = " SELECT * FROM SFIS1.C_ROUTE_CONTROL_T A, SFIS1.C_GROUP_CONFIG_T B WHERE " +
                " A.STATE_FLAG = '0' AND A.ROUTE_CODE='" + route_code + "' AND A.GROUP_NEXT = B.GROUP_NAME " +
                " AND B.SECTION_NAME = 'TEST' AND SUBSTR(A.GROUP_NEXT,1,4) NOT IN('PACK','ASSY','STOC')";
            //DataTable dt = dbsfis.DoSelectQuery(ssql);
            var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                var a = result.Data.ToListObject<infCheckStation>();
                List<infCheckStation> listGroup = a.Cast<infCheckStation>().ToList();
                for (int i = 0; i < listGroup.Count; i++)
                {
                    //if (STATIONLIST.IndexOf(dt.Rows[i]["GROUP_NEXT"].ToString()) > 0)
                    if(STATIONLIST.IndexOf(listGroup[i].GROUP_NEXT.ToString()) > 0)
                    {
                        //teststationlist.Add(dt.Rows[i]["GROUP_NEXT"].ToString());
                        teststationlist.Add(listGroup[i].GROUP_NEXT.ToString());
                    }
                }
                return teststationlist;
            }
            return null;
        }

        private async Task<string> getfirstpackstation(string route_code, string str)
        {
            string ssql = "";
            ssql = "SELECT * FROM " + routecontrolstr + " WHERE STATE_FLAG = '0' " +
               " AND ROUTE_CODE='" + route_code + "' AND STEP_SEQUENCE = (" +
               " SELECT MIN(STEP_SEQUENCE) FROM " + routecontrolstr + " WHERE STATE_FLAG = '0' AND " +
               " ROUTE_CODE='" + route_code + "' AND SUBSTR(GROUP_NEXT,1,4)='" + str + "') AND ROWNUM=1";
            //DataTable dt = dbsfis.DoSelectQuery(ssql);
            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                return result.Data["GROUP_NEXT"].ToString();
                //return dt.Rows[0]["GROUP_NEXT"].ToString();
            }
            else
            {
                return "";
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            tssSysdate.Text = DateTime.Now.ToString();
        }

        private string getstringstring(ArrayList getstring, string str)
        {
            for(int i = 0; i < getstring.Count; i++)
            {
                if(getstring[i].ToString().IndexOf(str) > 0)
                {
                    return getstring[i].ToString();
                }
                break;
            }
            return "";
        }


        private async Task<bool> holdroutenext(string route_code, string nextstation)  
        {
            string ssql = "";
            ssql = "UPDATE " + routecontrolstr +
             " SET GROUP_NEXT = DECODE (SUBSTR (GROUP_NEXT, 1, 4), 'HOLD', GROUP_NEXT,'HOLD' || GROUP_NEXT) " +
             " WHERE STATE_FLAG = '0' AND ROUTE_CODE ='" + route_code + "' AND GROUP_NEXT = '" + nextstation + "'";
            //dbsfis.ExecuteNonQuery(ssql);
            var sbUpdate = new StringBuilder();
            sbUpdate.Append(ssql);
            var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = sbUpdate.ToString()
            });
            return true;
        }

        private async Task<string> findnextstation(string route_code, string station)
        {
            string ssql = "", group_next;
            ssql = "SELECT * FROM " + routecontrolstr + " WHERE GROUP_NAME='" + station + "' AND ROUTE_CODE='" + route_code + "' AND STATE_FLAG='0'";
            //DataTable dt = dbsfis.DoSelectQuery(ssql);
            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                //group_next = dt.Rows[0]["GROUP_NEXT"].ToString();
                group_next = result.Data["GROUP_NEXT"].ToString();
                return group_next;
            }
            else
            {
                return "";
            }
        }

        private async Task<bool> updateroute(string route_code, string mo_number)
        {
            string ssql = "";
            try
            {
                ssql = "UPDATE " + r105str + " SET ROUTE_CODE = '" + route_code + "' WHERE MO_NUMBER='" + mo_number + "'";
                //dbsfis.ExecuteNonQuery(ssql);
                var sbUpdate = new StringBuilder();
                sbUpdate.Append(ssql);
                var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()
                });
                ssql = "UPDATE " + r107str + " SET SPECIAL_ROUTE='" + route_code + "' WHERE MO_NUMBER='" + mo_number + "'";
                //dbsfis.ExecuteNonQuery(ssql);
                sbUpdate = new StringBuilder();
                sbUpdate.Append(ssql);
                resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<string> GETNEWROUTE(string route_code)
        {
            string ssql = "", routename, tmproutename, tmproutecode = "";
            int i = 1;
            ssql = "SELECT * FROM " + routenamestr + " WHERE ROUTE_CODE='" + route_code + "'";
            //DataTable dt = dbsfis.DoSelectQuery(ssql);
            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                routename = result.Data["ROUTE_NAME"].ToString();
                //routename = dt.Rows[0]["ROUTE_NAME"].ToString();
                if (routename.IndexOf("HOLD") > 0)
                {
                    return route_code;
                }
                else
                {
                    try
                    {
                        tmproutename = routename + "HOLD";
                        while (true)
                        {
                            if (await findroutename(tmproutename + i.ToString()))
                            {
                                tmproutename = tmproutename + i.ToString();
                                break;
                            }
                            i = i + 1;
                        }
                        ssql = "SELECT MAX(ROUTE_CODE)+1 ROUTECODE FROM " + routenamestr;
                        //dt = dbsfis.DoSelectQuery(ssql);
                        var resultRouteCode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = ssql,
                            SfcCommandType = SfcCommandType.Text,
                        });
                        if (resultRouteCode.Data != null)
                        {
                            tmproutecode = resultRouteCode.Data["ROUTECODE"].ToString();
                            //tmproutecode = dt.Rows[0]["ROUTECODE"].ToString();
                        }

                        ssql = " INSERT INTO " + routenamestr +
                               " (ROUTE_CODE, ROUTE_NAME, ROUTE_DESC) SELECT " +
                               " '" + tmproutecode + "','" + tmproutename + "',ROUTE_DESC FROM   " +
                               " SFIS1.C_ROUTE_NAME_T WHERE ROUTE_CODE='" + route_code + "' ";
                        //dbsfis.ExecuteNonQuery(ssql);
                        var sbInsert = new StringBuilder();
                        sbInsert.Append(ssql);
                        var resultInsert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sbInsert.ToString()
                        });

                        ssql = " INSERT INTO " + routecontrolstr +
                               " (ROUTE_CODE, GROUP_NAME, GROUP_NEXT, STATE_FLAG, STEP_SEQUENCE, ROUTE_DESC) " +
                               " SELECT '" + tmproutecode + "', GROUP_NAME, GROUP_NEXT, STATE_FLAG, " +
                               " STEP_SEQUENCE, ROUTE_DESC FROM " + routecontrolstr +
                               " WHERE ROUTE_CODE='" + route_code + "' ";
                        //dbsfis.ExecuteNonQuery(ssql);
                        sbInsert = new StringBuilder();
                        sbInsert.Append(ssql);
                        resultInsert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sbInsert.ToString()
                        });

                        return tmproutecode;
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }
            }
            else
            {
                return "";
            }
        }

        private void AutoHold_Load(object sender, EventArgs e)
        {
            timer2.Enabled = true;
            timer2.Start();
            tssSysdate.Text = DateTime.Now.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            runhold();
        }

        private async Task<bool> findroutename(string routename)
        {
            string ssql = "";
            ssql = "SELECT * FROM " + routenamestr + " WHERE ROUTE_NAME='" + routename + "'";
            //DataTable dt = dbsfis.DoSelectQuery(ssql);
            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

         private async Task<string> getroutecode(string mo_number)
        {
            string ssql = "", route_code;
            ssql = "SELECT * FROM " + r105str + " WHERE MO_NUMBER='" + mo_number + "'";
            //DataTable dt = dbsfis.DoSelectQuery(ssql);
            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                var MO = result.Data.ToObject<infMo>();
                //route_code = dt.Rows[0]["ROUTE_CODE"].ToString();
                route_code = MO.ROUTE_CODE;
                return route_code;
            }
            else
            {
                return "";
            }
        }

        private async Task<bool> holdbycarton(string carton_no)
        {
            string ssql = "", station = "";
            try
            {
                if(GROUPLIST.IndexOf("FG")> 0)
                {
                    ssql = "SELECT * FROM " + z107str + " WHERE CARTON_NO='" + carton_no + "' OR MCARTON_NO='" + carton_no + "'";
                    //DataTable dt = dbsfis.DoSelectQuery(ssql);
                    var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = ssql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (result.Data == null)
                    {
                        ssql = "SELECT * FROM " + r107str + " WHERE CARTON_NO='" + carton_no + "' OR MCARTON_NO='" + carton_no + "'";
                    }
                    else
                    {
                        station = "FG";
                    }
                }
                else
                {
                    ssql = "SELECT * FROM " + r107str + " WHERE CARTON_NO='" + carton_no + "' OR MCARTON_NO='" + carton_no + "'";
                    //DataTable dt = dbsfis.DoSelectQuery(ssql);
                    var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = ssql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (result.Data != null)
                    {
                        var SERIAL = result.Data.ToObject<infR107>();
                        //if (holdbyserial(dt.Rows[0]["SERIAL_NUMBER"].ToString(), station))
                        if (await holdbyserial(SERIAL.SERIAL_NUMBER, station))
                        {
                            holdokcount = holdokcount + 1;
                        }
                        else
                        {
                            holdfailcount = holdfailcount + 1;
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private async Task<bool> holdbypallet(string pallet_no)
        {
            string ssql = "", station = "";
            try
            {
                if (GROUPLIST.IndexOf("FG") > 0)
                {
                    ssql = "SELECT * FROM " + z107str + " WHERE PALLET_NO = '" + pallet_no + "' OR IMEI = '" + pallet_no + "'";
                    //DataTable dt = dbsfis.DoSelectQuery(ssql);
                    var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = ssql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (result.Data == null)
                    {
                        ssql = "SELECT * FROM " + r107str + " WHERE PALLET_NO='" + pallet_no + "' OR IMEI='" + pallet_no + "'";
                    }
                    else
                    {
                        station = "FG";
                    }
                }
                else
                {
                    ssql = "SELECT * FROM " + r107str + " WHERE PALLET_NO='" + pallet_no + "' OR IMEI='" + pallet_no + "'";
                    //DataTable dt = dbsfis.DoSelectQuery(ssql);
                    var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = ssql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (result.Data != null)
                    {
                        var SERIAL = result.Data.ToObject<infR107>();
                        //if (holdbyserial(dt.Rows[0]["SERIAL_NUMBER"].ToString(), station))
                        if (await holdbyserial(SERIAL.SERIAL_NUMBER, station))
                        {
                            holdokcount = holdokcount + 1;
                        }
                        else
                        {
                            holdfailcount = holdfailcount + 1;
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private async Task<bool> holdbyserial(string serial_number,string station)
        {
            string ssql = "";
            try
            {
                if (await checkwip(serial_number, station))
                {
                    if (station == "FG")
                    {
                        ssql = "UPDATE " + z107str + " SET GROUP_NAME = DECODE (SUBSTR (GROUP_NAME, 1, 5),'HOLD-', GROUP_NAME,'HOLD-' || GROUP_NAME )," +
                               "NEXT_STATION = DECODE (SUBSTR (NEXT_STATION, 1, 5),'HOLD-', NEXT_STATION, 'HOLD-' || NEXT_STATION  )," +
                               "WIP_GROUP = DECODE (SUBSTR (WIP_GROUP, 1, 5), 'HOLD-', WIP_GROUP,'HOLD-'||WIP_GROUP)" +
                               " WHERE SERIAL_NUMBER = '" + serial_number + "'";
                        //dbsfis.ExecuteNonQuery(ssql);
                        var sbUpdate = new StringBuilder();
                        sbUpdate.Append(ssql);
                        var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sbUpdate.ToString()
                        });
                    }
                    else
                    {
                        ssql = "UPDATE " + r107str + " SET GROUP_NAME = DECODE (SUBSTR (GROUP_NAME, 1, 5),'HOLD-', GROUP_NAME,'HOLD-' || GROUP_NAME )," +
                               "NEXT_STATION = DECODE (SUBSTR (NEXT_STATION, 1, 5),'HOLD-', NEXT_STATION, 'HOLD-' || NEXT_STATION  )," +
                               "WIP_GROUP = DECODE (SUBSTR (WIP_GROUP, 1, 5), 'HOLD-', WIP_GROUP,'HOLD-'||WIP_GROUP)" +
                               " WHERE SERIAL_NUMBER = '" + serial_number + "'";
                        //dbsfis.ExecuteNonQuery(ssql);
                        var sbUpdate = new StringBuilder();
                        sbUpdate.Append(ssql);
                        var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sbUpdate.ToString()
                        });
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private async Task<bool> checkwip(string serial_number, string station)
        {
            string ssql = "", tmpwip;
            if (station == "FG")
            {
                ssql = "SELECT * FROM " + z107str + " WHERE SERIAL_NUMBER = '" + serial_number + "'";
            }
            else
            {
                ssql = "SELECT * FROM " + r107str + " WHERE SERIAL_NUMBER = '" + serial_number + "'";
            }
            //DataTable dt = dbsfis.DoSelectQuery(ssql);
            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                //tmpwip = dt.Rows[0]["WIP_GROUP"].ToString();
                tmpwip = result.Data["WIP_GROUP"].ToString();
                if(station == "FG")
                {
                    if(tmpwip.IndexOf("HOLD") > 0 || tmpwip == "SHIPPING")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if(tmpwip.IndexOf("HOLD") > 0 || tmpwip == "FG")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> finddatatodb(string data, string flag)
        {
            string ssql = "";
            ssql = "SELECT * FROM " + r107str + " WHERE ";
            if (flag == "M")
            {
                ssql = ssql + "MO_NUMBER = '" + data.ToString() + "' AND ROWNUM = 1";
            }
            else if(flag == "P")
            {
                ssql = ssql + "PALLET_NO = '" + data.ToString() + "' OR IMEI = '" + data.ToString() + "' AND ROWNUM = 1";
            }
            else
            {
                ssql = ssql + "CARTON_NO = '" + data.ToString() + "' OR MCARTON_NO = '" + data.ToString() + "' AND ROWNUM = 1";
            }

            //DataTable dt = dbsfis.DoSelectQuery(ssql);
            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> findmodel(string model_name)
        {
            string ssql = "";
            try
            {
                ssql = "SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME ='" + model_name + "'";
                //DataTable dt = dbsfis.DoSelectQuery(ssql);
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (result.Data == null)
                {
                    ssql = "SELECT * FROM SFIS1.C_MODEL_DESC_T@SFCODBE5A1 WHERE MODEL_NAME='" + model_name + "'";
                    //dt = dbsfis.DoSelectQuery(ssql);
                    result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = ssql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (result.Data == null)
                    {
                        ssql = "SELECT * FROM SFIS1.C_MODEL_DESC_T@SFCODBE5B1 WHERE MODEL_NAME='" + model_name + "'";
                        //dt = dbsfis.DoSelectQuery(ssql);
                        result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = ssql,
                            SfcCommandType = SfcCommandType.Text,
                        });
                        if (result.Data == null)
                        {
                            ssql = "SELECT * FROM SFIS1.C_MODEL_DESC_T@SFCODBE61 WHERE MODEL_NAME='" + model_name + "'";
                            //dt = dbsfis.DoSelectQuery(ssql);
                            result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = ssql,
                                SfcCommandType = SfcCommandType.Text,
                            });
                            if (result.Data != null)
                            {
                                DBLink = "@SFCODBE61";
                                strDB = "E6";
                            }
                        }
                        else
                        {
                            DBLink = "@SFCODBE5B1";
                            strDB = "E5B";
                        }
                    }
                    else
                    {
                        DBLink = "@SFCODBE5A1";
                        strDB = "E5A";
                    }
                }
                else
                {
                    DBLink = "";
                    strDB = "E5";
                }
                return true;
            }
            catch(Exception)
            {
                return false;
                throw;
            }
        }
    }
}
