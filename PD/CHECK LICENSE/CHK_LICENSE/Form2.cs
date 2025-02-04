using Oracle;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CHK_LICENSE
{
    public partial class Form2 : Form
    {
        DataTable dt = new DataTable();
        DataTable dtPass = new DataTable();
        public String line;
        public String routecode;
        public static string empno = string.Empty;
        public static string empname = string.Empty;
        public static int checkMax = 0;
        public static int checkPass = 0;
        public static string sqlBox = "select  distinct to_char(sysdate+1/48,'HH24') AS W_SECTION, to_char(sysdate, 'YYYYMMDD') AS MO_DATE, A.LINE_NAME AS LINE,A.WIP_GROUP W_STATION,'N/A' EC, " +
                                    " a.WIP_GROUP MYGROUP, A.SECTION_NAME SECTION, a.mcarton_no CARTONNO, a.tray_no BOXID, b.KEY_PART_SN MACID, C.KEY_PART_SN SSN2, A.SERIAL_NUMBER,A.SHIPPING_SN " +
                                     "from SFISM4.R_WIP_TRACKING_T a  " +
                                    "left join SFISM4.R_WIP_KEYPARTS_T b on a.serial_number = b.serial_number and  b.KEY_PART_NO = 'MACID' " +
                                    "left join SFISM4.R_WIP_KEYPARTS_T C on a.serial_number = C.serial_number and C.KEY_PART_NO = 'SSN2' " +
                                    "where a.mcarton_no IN ( " +
                                    "select mcarton_no from SFISM4.R_WIP_TRACKING_T where A.tray_no = '{0}')";

        public static string sqlSN = "select DISTINCT To_char(SYSDATE + 1 / 48, 'HH24') AS W_SECTION, To_char(SYSDATE, 'YYYYMMDD') AS MO_DATE, C.LINE_NAME AS LINE,C.WIP_GROUP W_STATION,'N/A' EC, c.WIP_GROUP MYGROUP, C.SECTION_NAME SECTION, a.serial_number,SHIPPING_SN,SSN2 from " +
                                    " (select serial_number,key_part_SN SHIPPING_SN from sfism4.r108  where serial_number ='{0}' and key_part_no='SSN1') a," +
                                    " (select serial_number,key_part_SN SSN2 from sfism4.r108  where serial_number ='{0}' and key_part_no='SSN2') b," +
                                    " (SELECT LINE_NAME,WIP_GROUP,SERIAL_NUMBER,SECTION_NAME FROM SFISM4.R107 WHERE  serial_number = '{0}' ) c" +
                                    " where a.serial_number = b.serial_number and a.SERIAL_NUMBER = c.SERIAL_NUMBER(+)";
        public static string[] strString;
        public static string[] arrMac;
        public static string[] arrSSN;
        public static string[] arrSSN2;
        public static string[] arrBoID;
        public static string[] arrCarton;
        public static SfcHttpClient _sfcHttpClient;
        public static DB oracle = null;
        public Form2()
        {

            InitializeComponent();
            checklabel();
            _sfcHttpClient = Form1._sfcHttpClient;
            lblVersion.Text = " Version: " + Application.ProductVersion;
        }
        private void showMessage(string MessageEnglish, string MessageVietNam)
        {
            string msgText;
            if (Form1.lang == "VNI")
            {
                msgText = MessageVietNam;
            }
            else msgText = MessageEnglish;
            errorMessage.Text = msgText;
            MessageBox.Show(msgText);
        }
        private async void employeeno_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (employeeno.Text == "UNDO")
                {
                    Reset();
                }
                string str = "select EMP_NAME,EMP_NO from sfis1.c_emp_desc_t where emp_bc='" + employeeno.Text.Trim() + "' ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = str, SfcCommandType = SfcCommandType.Text });
                if (result.Data == null)
                {
                    showMessage("No employee password", "Không tìm thấy mã nhân viên");
                }
                else
                {
                    employeeno.PasswordChar = (char)0;
                    employeeno.Text = result.Data["emp_name"]?.ToString() ?? "";
                    empname = result.Data["emp_name"]?.ToString() ?? "";
                    empno = result.Data["emp_no"]?.ToString() ?? "";
                    employeeno.Enabled = false;
                    txtck1.Enabled = true;
                    txtck1.Focus();
                }
            }
        }
        public async Task<string> getSql(string type)
        {
            type = type.Replace(":", "").Trim();
            if (type == "SN") return string.Format(@sqlSN, txtck1.Text);
            if (type == "BOX_ID") return string.Format(@sqlBox, txtck1.Text);

            return "";
        }
        private async void txtck1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (txtck1.Text == "UNDO")
                {
                    Reset();
                    UnlogRadioButom();
                    return;
                }
                logRadioButom();
                string sql = string.Format(await getSql(label1.Text), txtck1.Text);
                oracle = new DB(_sfcHttpClient);
                var Resultt = await oracle.GetList<DataSN>(sql);
                if (Resultt.Count == 0 )
                {
                    txtck1.SelectAll();
                    errorMessage.Text = strString[0] + ":" + txtck1.Text + " No data. Please check.";
                    return;
                }
                else
                {
                    List<string> listSN = new List<string>();
                    List<string> listSSN = new List<string>();
                    List<string> listSSN2 = new List<string>();
                    List<string> listBox = new List<string>();
                    List<string> listCTN = new List<string>();
                    foreach (var item in Resultt)
                    {
                        listSN.Add(item.SERIAL_NUMBER);
                        listSSN.Add(item.SHIPPING_SN);
                        listSSN2.Add(item.SSN2);
                        listBox.Add(item.BOXID);
                        listCTN.Add(item.CARTONNO);
                    };
                   
                    arrMac = listSN.ToArray();
                    arrSSN = listSSN.ToArray();
                    arrSSN2 = listSSN2.ToArray();
                    arrBoID = listBox.ToArray();
                    arrCarton = listCTN.ToArray();

                    checkMax += 1;
                    if (checkMax == checkPass)
                    {
                        if (await cPassStation() != "OK")
                        {
                            return;
                        }
                        Reset();
                        errorMessage.Text = "OK";
                        return;
                    }
                    errorMessage.Text = strString[0] + ": OK";
                    txtck1.Enabled = false;
                    txtck2.Enabled = true;
                    txtck2.Focus();
                }
            }
        }

        private async void txtck2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                await runTxt(txtck2, txtck3);
                #region closeCode
                /*
                if (txtck2.Text == "UNDO")
                {
                    Reset();
                    UnlogRadioButom();
                    return;
                }
                logRadioButom();
                if (Check(txtck2.Text, strString[1]) == "FALSE")
                {
                    txtck2.SelectAll();
                    errorMessage.Text = strString[1] + ": " + txtck2.Text + " No data. Please check.";
                    return;
                }
                else
                {
                    checkMax += 1;
                    if (checkMax == checkPass)
                    {
                        if (await cPassStation() != "OK")
                        {
                            return;
                        }
                        Reset();
                        errorMessage.Text = "OK";
                        return;
                    }
                    errorMessage.Text = strString[1] + ": OK";
                    txtck2.Enabled = false;
                    txtck3.Enabled = true;
                    txtck3.Focus();
                }
                */
                #endregion
            }
        }
        public async Task runTxt(TextBox thisTxt,TextBox nextTxt)
        {
            if (thisTxt.Text == "UNDO")
            {
                Reset();
                UnlogRadioButom();
                return;
            }
            logRadioButom();
            if (Check(thisTxt.Text, strString[checkMax]) == "FALSE")
            {
                thisTxt.SelectAll();
                errorMessage.Text = strString[checkMax] + ": " + thisTxt.Text + " No data. Please check.";
                return;
            }
            else
            {
                errorMessage.Text = strString[checkMax] + ": OK";
                checkMax += 1;
                if (checkMax == checkPass)
                {
                    if (await cPassStation() != "OK")
                    {
                        checkMax -= 1;
                        return;
                    }
                    Reset();
                    errorMessage.Text = "OK";
                    return;
                }
                if (checkMax < 6 )
                {
                    thisTxt.Enabled = false;
                    nextTxt.Enabled = true;
                    nextTxt.Focus();
                }
            }
        }
        private async void txtck3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                await runTxt(txtck3, txtck4);
                #region closeCode
                /*
                if (txtck3.Text == "UNDO")
                {
                    Reset();
                    UnlogRadioButom();
                    return;
                }
                logRadioButom();
                if (Check(txtck3.Text, strString[2]) == "FALSE")
                {
                    txtck3.SelectAll();
                    errorMessage.Text = strString[2] + ": " + txtck3.Text + " No data. Please check.";
                    return;
                }
                else
                {
                    checkMax += 1;
                    if (checkMax == checkPass)
                    {
                        if (await cPassStation() != "OK")
                        {
                            checkMax -= 1;
                            return;
                        }
                        Reset();
                        errorMessage.Text = ": OK";
                        return;
                    }
                    errorMessage.Text = strString[2] + "OK";
                    txtck3.Enabled = false;
                    txtck4.Enabled = true;
                    txtck4.Focus();
                }*/
                #endregion 
            }
        }
        private async void txtck4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                await runTxt(txtck4, txtck5);
                #region closeCode
                /*
                if (txtck4.Text == "UNDO")
                {
                    Reset();
                    UnlogRadioButom();
                    return;
                }
                logRadioButom();
                if (Check(txtck4.Text, strString[3]) == "FALSE")
                {
                    txtck4.SelectAll();
                    errorMessage.Text = strString[3] + ": " + txtck4.Text + " No data. Please check.";
                    return;
                }
                else
                {
                    checkMax += 1;
                    if (checkMax == checkPass)
                    {
                        if (await cPassStation() != "OK")
                        {
                            return;
                        }
                        Reset();
                        errorMessage.Text = "OK";
                        return;
                    }
                    errorMessage.Text = strString[3] + ": OK";
                    txtck4.Enabled = false;
                    txtck5.Enabled = true;
                    txtck5.Focus();
                }*/
                #endregion
            }
        }

        private async void txtck5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                await runTxt(txtck5, txtck6);
                #region closeCode
                /*
                if (txtck5.Text == "UNDO")
                {
                    Reset();
                    UnlogRadioButom();
                    return;
                }
                logRadioButom();
                if (Check(txtck5.Text, strString[4]) == "FALSE")
                {
                    txtck5.SelectAll();
                    errorMessage.Text = strString[4] + ": " + txtck5.Text + " No data. Please check.";
                    return;
                }
                else
                {
                    checkMax += 1;
                    if (checkMax == checkPass)
                    {
                        if (await cPassStation() != "OK")
                        {
                            return;
                        }
                        Reset();
                        errorMessage.Text = "OK";
                        return;
                    }
                }
                errorMessage.Text = strString[4] + ": OK";
                txtck5.Enabled = false;
                txtck6.Enabled = true;
                txtck6.Focus();
                */
                #endregion
            }
        }
        private async void txtck6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                await runTxt(txtck6, txtck6);
                #region closeCode
                /*
                if (txtck5.Text == "UNDO")
                {
                    Reset();
                    UnlogRadioButom();
                    return;
                }
                logRadioButom();
                if (Check(txtck6.Text, strString[5]) == "FALSE")
                {
                    txtck6.SelectAll();
                    errorMessage.Text = strString[5] + ": " + txtck5.Text + " No data. Please check.";
                    return;
                }
                else
                {
                    checkMax += 1;
                    if (checkMax == checkPass)
                    {
                        if (await cPassStation() != "OK")
                        {
                            return;
                        }
                        Reset();
                        errorMessage.Text = "OK";
                        return;
                    }
                }
                */
                #endregion
            }
        }

        public string Check(string data, string coulum)
        {
            if (coulum == "BOX_ID")
            {
                for (int i = 0; i < arrBoID.Length; i++)
                {
                    if (data == arrBoID[i])
                    {
                        return "OK";
                    }
                }
            }
            if (coulum == "SN_MACID")
            {
                for (int i = 0; i < arrMac.Length; i++)
                {
                    if (data == arrMac[i])
                    {
                        return "OK";
                    }
                }

            }
            if (coulum == "CARTON_NO")
            {
                for (int i = 0; i < arrCarton.Length; i++)
                {
                    if (data == arrCarton[i])
                    {
                        return "OK";
                    }
                }
            }
            if (coulum == "CARTON_NO")
            {
                for (int i = 0; i < arrCarton.Length; i++)
                {
                    if (data == arrCarton[i])
                    {
                        return "OK";
                    }
                }
            }
            if (coulum == "SSN2")
            {
                for (int i = 0; i < arrSSN2.Length; i++)
                {
                    if (data == arrSSN2[i])
                    {
                        return "OK";
                    }
                }
            }
            if (coulum == "SSN")
            {
                for (int i = 0; i < arrSSN.Length; i++)
                {
                    if (data == arrSSN[i])
                    {
                        return "OK";
                    }
                }
            }
            return "FALSE";
        }
       
       
        public async Task<string> cPassStation()
        {
            string strResult = string.Empty;
            string sql = string.Format(await getSql(label1.Text), txtck1.Text);
            var Resultt = await oracle.GetList<DataSN>(sql);
            if (Resultt.Count > 0)
            {
                foreach (var item in Resultt)
                {
                    var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.CHECK_ROUTE ",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="LINE",Value=item.LINE,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="MYGROUP",Value="COMPARE_LABEL",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="DATA",Value=item.SERIAL_NUMBER,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                        }
                    });

                    dynamic ads = result.Data;
                    if (ads[0]["res"] != "OK")
                    {
                        showMessage("Next group not in Compare_Label. Please check","Lỗi lưu trình:"+ ads[0]["res"]);
                        return "False";
                    }

                };
                foreach (var item in Resultt)
                {
                    var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.test_input ",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="LINE",Value=item.LINE,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="SECTION",Value=item.SECTION,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="W_STATION",Value=item.W_STATION,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="DATETIME",Value=DateTime.Now,SfcParameterDataType=SfcParameterDataType.Date,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="EC",Value="N/A",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="DATA",Value=item.SERIAL_NUMBER,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="MO_DATE",Value=item.MO_DATE,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="W_SECTION",Value=item.W_SECTION,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="MYGROUP",Value="COMPARE_LABEL",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output},
                            new SfcParameter{Name="EMP",Value=empno,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        }
                    });


                    dynamic ads = result.Data;
                    if( ads[0]["res"] != "OK")
                    {
                        errorMessage.Text = ads[0]["res"];
                        return "FALSE";
                    }
                };
            }
            return "OK";
    }

        private void rcp1_CheckedChanged(object sender, EventArgs e)
        {
            checklabel1();
        }

        private void rcp2_CheckedChanged(object sender, EventArgs e)
        {
            checklabel1();
        }

        private void rcp3_CheckedChanged(object sender, EventArgs e)
        {
            checklabel1();
        }

        private void rcp4_CheckedChanged(object sender, EventArgs e)
        {
            checklabel1();
        }

        private void rcp5_CheckedChanged(object sender, EventArgs e)
        {
            checklabel1();
        }
        public void checklabel()
        {
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label10.Visible = false;
            txtck1.Visible = false;
            txtck2.Visible = false;
            txtck3.Visible = false;
            txtck4.Visible = false;
            txtck5.Visible = false;
            employeeno.Visible = false;
        }

        public void logRadioButom()
        {
            rcp1.Enabled = false;
            rcp2.Enabled = false;
            rcp3.Enabled = false;
            rcp4.Enabled = false;
            rcp5.Enabled = false;
            rcp6.Enabled = false;
            rcp7.Enabled = false;
        }
        public void UnlogRadioButom()
        {
            rcp1.Enabled = true;
            rcp2.Enabled = true;
            rcp3.Enabled = true;
            rcp4.Enabled = true;
            rcp5.Enabled = true;
            rcp6.Enabled = true;
            rcp7.Enabled = true;
        }
        public void checklabel1()
        {
            if (rcp1.Checked)
            {

                strString = new string[6] { "BOX_ID", "SN_MACID", "CARTON_NO", "CARTON_NO", "SN_MACID", "CARTON_NO" };
                checkPass = 6;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                label1.Text = strString[0] + " : ";
                label2.Text = strString[1] + " : ";
                label3.Text = strString[2] + " : ";
                label4.Text = strString[3] + " : ";
                label5.Text = strString[4] + " : ";
                label6.Text = strString[5] + " : ";
                txtck1.Visible = true;
                txtck2.Visible = true;
                txtck3.Visible = true;
                txtck4.Visible = true;
                txtck5.Visible = true;
                txtck6.Visible = true;
                label10.Visible = true;
            }
            else if (rcp2.Checked)
            {

                strString = new string[6] { "BOX_ID", "SN_MACID", "CARTON_NO", "SN_MACID", "CARTON_NO", "" };
                checkPass = 5;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                label6.Visible = false;
                label1.Text = strString[0] + " : ";
                label2.Text = strString[1] + " : ";
                label3.Text = strString[2] + " : ";
                label4.Text = strString[3] + " : ";
                label5.Text = strString[4] + " : ";
                txtck1.Visible = true;
                txtck2.Visible = true;
                txtck3.Visible = true;
                txtck4.Visible = true;
                txtck5.Visible = true;
                txtck6.Visible = false;
                label10.Visible = true;

            }
            else if (rcp3.Checked)
            {
                strString = new string[6] { "BOX_ID", "SN_MACID", "CARTON_NO", "SN_MACID", "", "" };
                checkPass = 4;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = false;
                label6.Visible = false;
                label1.Text = strString[0] + " : ";
                label2.Text = strString[1] + " : ";
                label3.Text = strString[2] + " : ";
                label4.Text = strString[3] + " : ";
                txtck1.Visible = true;
                txtck2.Visible = true;
                txtck3.Visible = true;
                txtck4.Visible = true;
                txtck5.Visible = false;
                txtck6.Visible = false;
                label10.Visible = true;
            }
            else if (rcp4.Checked)
            {
                strString = new string[6] { "BOX_ID", "SN_MACID", "SSN2", "CARTON_NO", "SN_MACID", "SSN2" };
                checkPass = 6;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                label1.Text = strString[0] + " : ";
                label2.Text = strString[1] + " : ";
                label3.Text = strString[2] + " : ";
                label4.Text = strString[3] + " : ";
                label5.Text = strString[4] + " : ";
                label6.Text = strString[5] + " : ";
                txtck1.Visible = true;
                txtck2.Visible = true;
                txtck3.Visible = true;
                txtck4.Visible = true;
                txtck5.Visible = true;
                txtck6.Visible = true;
                label10.Visible = true;
            }
            else if (rcp5.Checked)
            {
                strString = new string[6] { "BOX_ID", "SSN", "CARTON_NO", "SSN", "", "" };
                checkPass = 4;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = false;
                label6.Visible = false;
                label1.Text = strString[0] + " : ";
                label2.Text = strString[1] + " : ";
                label3.Text = strString[2] + " : ";
                label4.Text = strString[3] + " : ";
                txtck1.Visible = true;
                txtck2.Visible = true;
                txtck3.Visible = true;
                txtck4.Visible = true;
                txtck5.Visible = false;
                txtck6.Visible = false;
                label10.Visible = true;
            }
            else if (rcp6.Checked)
            {
                strString = new string[3] { "SN", "SSN", "SSN2"};
                checkPass = 3;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = false;
                label5.Visible = false;
                label6.Visible = false;
                label1.Text = strString[0] + " : ";
                label2.Text = strString[1] + " : ";
                label3.Text = strString[2] + " : ";
                txtck1.Visible = true;
                txtck2.Visible = true;
                txtck3.Visible = true;
                txtck4.Visible = false;
                txtck5.Visible = false;
                txtck6.Visible = false;
                label10.Visible = true;
            }
            else if (rcp7.Checked)
            {
                strString = new string[6] { "BOX_ID", "SN_MACID", "SSN", "CARTON_NO", "SN_MACID", "SSN" };
                checkPass = 6;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                label1.Text = strString[0] + " : ";
                label2.Text = strString[1] + " : ";
                label3.Text = strString[2] + " : ";
                label4.Text = strString[3] + " : ";
                label5.Text = strString[4] + " : ";
                label6.Text = strString[5] + " : ";
                txtck1.Visible = true;
                txtck2.Visible = true;
                txtck3.Visible = true;
                txtck4.Visible = true;
                txtck5.Visible = true;
                txtck6.Visible = true;
                label10.Visible = true;
            }

            if (empno != null && empno != "")
            {
                employeeno.Enabled = false;
                employeeno.Visible = true;
                employeeno.Text = empname;
                txtck1.Enabled = true;
                txtck1.Focus();
            }
            else
            {
                employeeno.Visible = true;
                employeeno.Enabled = true;
                employeeno.Focus();
            }
        }
        public void Reset()
        {
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label10.Visible = false;
            txtck1.Visible = false;
            txtck2.Visible = false;
            txtck3.Visible = false;
            txtck4.Visible = false;
            txtck5.Visible = false;
            txtck6.Visible = false;
            txtck1.Clear();
            txtck2.Clear();
            txtck3.Clear();
            txtck4.Clear();
            txtck5.Clear();
            txtck6.Clear();
            txtck1.Enabled = false;
            txtck2.Enabled = false;
            txtck3.Enabled = false;
            txtck4.Enabled = false;
            txtck5.Enabled = false;
            txtck6.Enabled = false;
            employeeno.Enabled = false;
            checkMax = 0;
            checkPass = 0;
            checklabel1();
            if (errorMessage.Text != "OK")
                errorMessage.Text = "";
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("CHK_LICENSE"))
            {
                process.Kill();
            }
        }

        private void checkLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 main = new Form1();
            this.Visible = false;
            main.Show();
        }

        private void lblCheckLicense_Click(object sender, EventArgs e)
        {
            checkMax = 0;
            checkPass = 0;
            Form1 main = new Form1();
            this.Visible = false;
            main.Show();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rcp6_CheckedChanged(object sender, EventArgs e)
        {
            checklabel1();
        }

        private void rcp7_CheckedChanged(object sender, EventArgs e)
        {
            checklabel1();
        }
    }
    public class DataSN
    {
        public string W_SECTION { get; set; }
        public string AP_VERSION { get; set; }
        public string LINE { get; set; }
        public string W_STATION { get; set; }
        public string EC { get; set; }
        public string MYGROUP { get; set; }
        public string SECTION { get; set; }
        public string CARTONNO { get; set; }
        public string BOXID { get; set; }
        public string MACID { get; set; }
        public string SSN2 { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string SHIPPING_SN { get; set; }
        public string MO_DATE { get; set; }
    }
    
}
