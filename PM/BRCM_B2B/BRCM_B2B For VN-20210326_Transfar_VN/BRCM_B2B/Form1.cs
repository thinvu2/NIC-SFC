using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using BRCM_B2B.mail;
using myFtpApp;
using System.Threading;
using System.Net.NetworkInformation;
using Sfc.Library.HttpClient;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Text.RegularExpressions;
using System.Reflection;
using Sfc.Core.Parameters;

namespace BRCM_B2B
{

    // 目前越南版本和重慶版本最大的不同就是獲取IC和扣IC的方式有差異。其他部份代碼可以互相參照。至於流程和原理問題可以問JBD PM的蘭洋
    public partial class Form1 : Form
    {
        B2B b2b = new B2B();
        manual man = new manual();
        INIFile ini = new INIFile(Application.StartupPath + @"\BRCM_B2B.ini");
        string MAIL_SEND = string.Empty;
        string MAIL_CC = string.Empty;
        string MAIL_FROM = string.Empty;
        public static string sentimenic = string.Empty;
        public static string sentimeecd = string.Empty; 
        public static string sentimesupercap = string.Empty;
        public static string guid = string.Empty;
        public string loginApiUri = "";
        public string loginDB = "";
        public static SfcHttpClient sfcClient;
        DAL fDal;
        string fromTime;
        DataTable zdt;
        public static string zMac = "", zIP = "";
        public static string empNo = "", APVersion;
        public string empPass = "";
        public string inputLogin = "";

        public Form1()
        {
            InitializeComponent();

            //string[] Args = Environment.GetCommandLineArgs();
            ////if (Args.Length == 1)
            ////{
            ////    MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING");
            ////    Environment.Exit(0);
            ////}
            //foreach (string s in Args)
            //{
            //    inputLogin = s.ToString();
            //}
            //string[] argsInfor = Regex.Split(inputLogin, @";");
            //loginApiUri = argsInfor[1].ToString();
            //loginDB = argsInfor[2].ToString();
            //empNo = argsInfor[3].ToString();
            //empPass = argsInfor[4].ToString();

            loginApiUri = @"http://10.220.96.223:8080/sfcapi";
            loginDB = "NIC";
            empNo = "SMO";
            empPass = "1";

            string sendtime = DateTime.Now.ToString("yyyyMMddhhmmss");
            fDal = new DAL();
            zMac = INIFile.GetMacAddress();
            zIP = INIFile.GetLocalIPAddress();
            APVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Check_run_app();
        }

        public async void Check_run_app()
        {
            //onlyone exe
            string program = AppDomain.CurrentDomain.FriendlyName;
            int index = program.IndexOf(".exe");
            if (index != -1)
            {
                program = program.Remove(index, 4);
            }

            Process[] processes = Process.GetProcesses();
            List<dynamic> LIST = new List<dynamic>();

            foreach (Process process in processes)
            {
                LIST.Add(process.ProcessName); 
            }

            int count = LIST.Count(x => x == program);

            if (count > 1)
            {
                MessageBox.Show("Chương trình BRCM_B2B đang bật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            //****************************************

        }

        private void btnAuto_Click(object sender, EventArgs e)
        {
            if (btnAuto.Text == "Auto")
            {
                timer1.Enabled = true;
                btnAuto.Text = "Stop";
            }
            else
            {
                timer1.Enabled = false;
                btnAuto.Text = "Auto";

            }
        }

        public void GoToSendMail(string body)
        {
            mail.mail_service mail = new mail_service();
            mail.sendMail(MAIL_SEND, MAIL_FROM, MAIL_CC, body, body);
        }
        public void GetMail()
        {
            MAIL_SEND = ini.Read("SETUP", "MAIL_SEND");
            MAIL_CC = ini.Read("SETUP", "MAIL_CC");
            MAIL_FROM = ini.Read("SETUP", "MAIL_FROM");
            b2b.MAIL_SEND = MAIL_SEND;
            b2b.MAIL_CC = MAIL_CC;
            b2b.MAIL_FROM = MAIL_FROM;
        }
        private async void timer2_Tick(object sender, EventArgs e)
        {
            if (timer1.Enabled == true) return;
            zdt = new DataTable();
            zdt = await b2b.zgetalarm(sfcClient,fromTime);
            if (zdt.Rows.Count == 0)
            {
                await connect();
                zdt = await b2b.zgetalarm(sfcClient, fromTime);
                if (zdt.Rows.Count > 0)
                    writelog("Connect other session OK");
                else writelog("Connect other session FAIL");
            }
            else
            {
                if (zdt.Rows[0][0].ToString() == "1")
                    Notify("B2B Manual mode running over 15 mins");
            }
        }
        private void Notify(string text)
        {
            var iconUri = "file:///" + Path.GetFullPath("Resource icon/Tatice-Operating-Systems-Linux.ico");
            var imageUri = "file:///" + Path.GetFullPath("Resource icon/Tatice-Operating-Systems-Linux.ico");

            new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                .AddText(text)
                .AddAppLogoOverride(new Uri(iconUri), ToastGenericAppLogoCrop.Circle)
                .AddHeroImage(new Uri(imageUri))
                .Show();
        }
        private async void timer1_Tick(object sender, EventArgs e)
        {
            Boolean SHIPFILE_NIC = false;
            Boolean SHIPFILE_ECD = false;
            Boolean SHIPFILE_SUPERCAP = false;
            timer1.Enabled = false;
            if (timer2.Enabled == true) return;
            GetMail();
            await SendTestData();
            DataTable dt = new DataTable();
            DataTable flagdt = new DataTable();
            string str = string.Empty;
            try
            {
                dt =await b2b.gettime(sfcClient);
                if(sfcClient.AccessTokenResponse.Expires < DateTime.Now)
                {
                    sfcClient = null;
                    await connect();
                    dt = await b2b.gettime(sfcClient);
                    if(dt.Rows.Count>0)
                    writelog("Re-Connect OK");
                    else writelog("Re-Connect FAIL");
                }
                if (dt.Rows.Count == 0)
                {
                    await connect();
                    dt = await b2b.gettime(sfcClient);
                    if (dt.Rows.Count > 0)
                        writelog("Connect other session OK");
                    else writelog("Connect other session FAIL");
                }
                if (dt.Rows[0]["DAILY"].ToString() == "08")//生成SHIP&WIPC file
                {
                    try
                    {
                        flagdt = await b2b.getshipflag(sfcClient);
                        if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                        {
                            await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='N' WHERE PRG_NAME='BRCM-VN' and vr_class='SHIP'", sfcClient);
                            if (await b2b.ShipfileNIC(sfcClient))
                            {
                                await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='Y' WHERE PRG_NAME='BRCM-VN' and vr_class='SHIP'", sfcClient);
                                str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + "SHIP文件生成成功";
                                rtbMessage.Text += str;
                                GoToSendMail("SHIP文件生成成功!");
                                SHIPFILE_NIC = true;
                            }
                            else
                            {
                                str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + b2b.ExMessage + "\n" + "SHIP文件生成失敗";
                                rtbMessage.Text += str;
                                GoToSendMail(str);
                                return;
                            }
                            writelog(str);
                        }
                        if (SHIPFILE_NIC)
                        {
                            flagdt = await b2b.getwipcflag(sfcClient);
                            if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                            {
                                await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='N' WHERE PRG_NAME='BRCM-VN' and vr_class='WIPC'", sfcClient);
                                if (await b2b.wipcfileNIC(sfcClient))
                                {
                                    await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='Y' WHERE PRG_NAME='BRCM-VN' and vr_class='WIPC'", sfcClient);
                                    str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + "WIPC文件生成成功";
                                    rtbMessage.Text += str;
                                    GoToSendMail("WIPC文件生成成功!");
                                }
                                else
                                {
                                    str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + b2b.ExMessage + "\n" + "WIPC文件生成失敗";
                                    rtbMessage.Text += str;
                                    GoToSendMail(str);
                                    return;
                                }
                                writelog(str);

                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        writelog("NIC FILE have exception :"+ex.Message);
                    }

                    //wenchun ship ecd
                    try
                    {
                        flagdt = await b2b.getshipflagECD(sfcClient);
                        if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                        {
                            await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='N' WHERE PRG_NAME='ECD-VN' and vr_class='SHIP'", sfcClient);
                            if (await b2b.ShipfileECD(sfcClient))
                            {
                                await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='Y' WHERE PRG_NAME='ECD-VN' and vr_class='SHIP'", sfcClient);
                                str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + "ECD SHIP文件生成成功";
                                rtbMessage.Text += str;
                                GoToSendMail("ECD SHIP文件生成成功!");
                                SHIPFILE_ECD = true;
                            }
                            else
                            {
                                str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + b2b.ExMessage + "\n" + "ECD SHIP文件生成失敗";
                                rtbMessage.Text += str;
                                GoToSendMail(str);
                                return;
                            }
                            writelog(str);
                        }

                        //wenchun wipc ecd
                        if (SHIPFILE_ECD)
                        {
                            flagdt = await b2b.getwipcflagECD(sfcClient);
                            if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                            {

                                await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='N' WHERE PRG_NAME='ECD-VN' and vr_class='WIPC'", sfcClient);
                                if (await b2b.wipcfileECD(sfcClient))
                                {
                                    await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='Y' WHERE PRG_NAME='ECD-VN' and vr_class='WIPC'", sfcClient);
                                    str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + "ECD WIPC文件生成成功";
                                    rtbMessage.Text += str;
                                    GoToSendMail("ECD WIPC文件生成成功!");
                                }
                                else
                                {
                                    str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + b2b.ExMessage + "\n" + "ECD WIPC文件生成失敗";
                                    rtbMessage.Text += str;
                                    GoToSendMail(str);
                                    return;
                                }
                                writelog(str);

                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        writelog("ECD FILE have exception :" + ex.Message);
                    }

                    //TOP ship SuperCap
                    flagdt = await b2b.getshipflagSUPERCAP(sfcClient);
                    if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                    {
                        await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='N' WHERE PRG_NAME='SUPERCAP-VN' and vr_class='SHIP'", sfcClient);
                        if (await b2b.ShipfileSuperCap(sfcClient))
                        {
                            await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='Y' WHERE PRG_NAME='SUPERCAP-VN' and vr_class='SHIP'", sfcClient);
                            str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + "SUPERCAP SHIP文件生成成功";
                            rtbMessage.Text += str;
                            GoToSendMail("SUPERCAP SHIP文件生成成功!");
                            SHIPFILE_SUPERCAP = true;
                        }
                        else
                        {
                            str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + b2b.ExMessage + "\n" + "SUPERCAP SHIP文件生成失敗";
                            rtbMessage.Text += str;
                            GoToSendMail(str);
                            return;
                        }
                        writelog(str);
                    }
                    //TOP wipc SuperCap
                    if (SHIPFILE_SUPERCAP)
                    {
                        flagdt = await b2b.getwipcflagSUPERCAP(sfcClient);
                        if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                        {
                            await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='N' WHERE PRG_NAME='SUPERCAP-VN' and vr_class='WIPC'", sfcClient);
                            if (await b2b.wipcfileSUPERCAP(sfcClient))
                            {
                                await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='Y' WHERE PRG_NAME='SUPERCAP-VN' and vr_class='WIPC'", sfcClient);
                                str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + "SUPERCAP WIPC文件生成成功";
                                rtbMessage.Text += str;
                                GoToSendMail("SUPERCAP WIPC文件生成成功!");
                            }
                            else
                            {
                                str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + b2b.ExMessage + "\n" + "SUPERCAP WIPC文件生成失敗";
                                rtbMessage.Text += str;
                                GoToSendMail(str);
                            }
                            writelog(str);
                        }
                    }
                }
                if (dt.Rows[0]["DAILY"].ToString() == "11")
                {
                    flagdt =await b2b.getonhbflag(sfcClient);
                    if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                    {

                       await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='N' WHERE PRG_NAME='BRCM-VN' and vr_class='ONHB'", sfcClient);
                        if (await b2b.weeklyonhbfile(sfcClient))
                        {
                           await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='Y' WHERE PRG_NAME='BRCM-VN' and vr_class='ONHB'", sfcClient);
                            str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + "ONHB文件生成成功";
                            rtbMessage.Text += str;
                            GoToSendMail("ONHB文件生成成功!");
                        }
                        else
                        {
                            str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + b2b.ExMessage + "\n" + "ONHB文件生成失敗";
                            rtbMessage.Text += str;
                            GoToSendMail(str);
                            return;
                        }
                        writelog(str);

                    }
                }

                //***********************************************************************************
                //modify by finger , for VN send BDSN files
                if (dt.Rows[0]["DAILY"].ToString() == "14")
                {
                    flagdt =await b2b.getbdsnflag(sfcClient);
                    if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                    {
                       await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDD'),VR_VALUE='N' WHERE PRG_NAME='BRCM-VN' and vr_class='BDSN'", sfcClient);
                        if (await b2b.bdsnfile(sfcClient))
                        {
                        await    fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDD'),VR_VALUE='Y' WHERE PRG_NAME='BRCM-VN' and vr_class='BDSN'", sfcClient);
                            str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + "BDSN文件生成成功";
                            rtbMessage.Text += str;
                            GoToSendMail("BDSN文件生成成功!");
                        }
                        else
                        {
                           await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDD'),VR_VALUE='N' WHERE PRG_NAME='BRCM-VN' and vr_class='BDSN'", sfcClient);
                            str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + b2b.ExMessage + "\n" + "BDSN文件生成失敗";
                            rtbMessage.Text += str;
                            GoToSendMail(str);
                            return;
                        }
                        writelog(str);

                    }

                    //wenchun hang ECD
                    flagdt = await b2b.getbdsnflagECD(sfcClient);
                    if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                    {
                        await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDD'),VR_VALUE='N' WHERE PRG_NAME='ECD-VN' and vr_class='BDSN'", sfcClient);
                        if (await b2b.bdsnfileECD(sfcClient))
                        {
                            await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDD'),VR_VALUE='Y' WHERE PRG_NAME='ECD-VN' and vr_class='BDSN'", sfcClient);
                            str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + "ECD BDSN文件生成成功";
                            rtbMessage.Text += str;
                            GoToSendMail("ECD BDSN文件生成成功!");
                        }
                        else
                        {
                            await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDD'),VR_VALUE='N' WHERE PRG_NAME='ECD-VN' and vr_class='BDSN'", sfcClient);
                            str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + b2b.ExMessage + "\n" + "BDSN文件生成失敗";
                            rtbMessage.Text += str;
                            GoToSendMail(str);
                            return;
                        }
                        writelog(str);

                    }
                    //TOP ADD AUTO SUPERCAP
                    flagdt = await b2b.getbdsnflagSUPERCAP(sfcClient);
                    if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                    {
                        await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDD'),VR_VALUE='N' WHERE PRG_NAME='SUPERCAP-VN' and vr_class='BDSN'", sfcClient);
                        if (await b2b.bdsnfileSuperCap(sfcClient))
                        {
                            await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDD'),VR_VALUE='Y' WHERE PRG_NAME='SUPERCAP-VN' and vr_class='BDSN'", sfcClient);
                            str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + "SUPERCAP BDSN文件生成成功";
                            rtbMessage.Text += str;
                            GoToSendMail("SUPERCAP BDSN文件生成成功!");
                        }
                        else
                        {
                            await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDD'),VR_VALUE='N' WHERE PRG_NAME='SUPERCAP-VN' and vr_class='BDSN'", sfcClient);
                            str = "\n" + dt.Rows[0]["SYSDATE"].ToString() + "\n" + b2b.ExMessage + "\n" + "BDSN文件生成失敗";
                            rtbMessage.Text += str;
                            GoToSendMail(str);
                            return;
                        }
                        writelog(str);

                    }
                }

                
                if (dt.Rows[0]["DAILY"].ToString() == "12" && dt.Rows[0]["WEEKLY"].ToString().Trim() != "MONDAY")
                {
                    flagdt =await b2b.getsendflag(sfcClient);
                    if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                    {
                        await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='N' WHERE PRG_NAME='BRCM' and vr_class='SEND'", sfcClient);
                        flagdt =await b2b.getallflag(sfcClient);
                        for (int i = 0; i < flagdt.Rows.Count; i++)
                        {
                            if (flagdt.Rows[i]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[i]["VR_VALUE"].ToString() != "Y")
                            {
                                string strerror = string.Empty;
                                strerror += flagdt.Rows[i]["VR_CLASS"].ToString() + ", ";
                            }
                        }
                       await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='Y' WHERE PRG_NAME='BRCM' and vr_class='SEND'", sfcClient);
                    }

                }
                if (dt.Rows[0]["DAILY"].ToString() == "12" && dt.Rows[0]["WEEKLY"].ToString().Trim() == "MONDAY")
                {
                    flagdt =await b2b.getsendflag(sfcClient);
                    if (flagdt.Rows[0]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[0]["VR_VALUE"].ToString() != "Y")
                    {
                        await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='N' WHERE PRG_NAME='BRCM' and vr_class='SEND'", sfcClient);
                        flagdt =await b2b.getallflag2(sfcClient);
                        for (int i = 0; i < flagdt.Rows.Count; i++)
                        {
                            if (flagdt.Rows[i]["TIME"].ToString() != dt.Rows[0]["TIME"].ToString() || flagdt.Rows[i]["VR_VALUE"].ToString() != "Y")
                            {
                                string strerror = string.Empty;
                                strerror += flagdt.Rows[i]["VR_CLASS"].ToString() + ", ";
                            }
                        }
                        await fDal.ExcuteNonQuerySQL("update sfis1.c_parameter_ini set VR_NAME=to_char(sysdate,'YYYYMMDDHH24MISS'),VR_VALUE='Y' WHERE PRG_NAME='BRCM' and vr_class='SEND'", sfcClient);
                    }

                }
            }
            catch (Exception ex)
            {
                var lineNumber = 0;
                const string lineSearch = ":line ";
                var index = ex.StackTrace.LastIndexOf(lineSearch);
                if (index != -1)
                {
                    var lineNumberText = ex.StackTrace.Substring(index + lineSearch.Length);
                    if (int.TryParse(lineNumberText, out lineNumber))
                    {
                    }
                }
                writelog(ex.Message + " at line:" + lineNumber);
                sfcClient = null;
                await connect();
                rtbMessage.Text += str;
                timer1.Enabled = true;
            }
            finally
            {
                timer1.Enabled = true;
            }
        }
        private async Task<string> checkTime(string DN)
        {
            DataTable dt;
            string sql = string.Format(@"select TO_CHAR (finish_date+1, 'YYYYMMDD') finish_date from SFISM4.R_BPCS_INVOICE_T where INVOICE ='{0}'", DN);
            dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
            return dt.Rows[0]["finish_date"].ToString();
        }

        private void writelog(string str)
        {
            string path = Directory.GetCurrentDirectory() + "\\log.txt";
            string time = DateTime.Now.ToString();
            StreamWriter stw = new StreamWriter(path, true);
            stw.WriteLine(time+" : "+str);
            stw.Flush();
            stw.Close();
        }

        private async void btnship_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定生成SHIP,WIPC文件", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                btnship.Enabled = false;
                Guid obj = Guid.NewGuid();
                string DN = txtDN.Text;
                if (DN != "")
                {
                    sentimenic = Int64.Parse(await checkTime(DN)) + DateTime.Now.ToString("HHmmss");
                }
                else
                {
                    sentimenic = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                sentimeecd = (Int64.Parse(sentimenic) + 1).ToString();
                sentimesupercap = (Int64.Parse(sentimenic) + 2).ToString();
                //---------------------------------------------------------------------------------
                if (await man.ShipfileNIC(DN, sfcClient))
                    MessageBox.Show("Create SHIP NIC OK");
                else
                {
                    MessageBox.Show(man.ExMessage);
                    return;
                }
                if (await man.wipcfileNIC(txtDN.Text, sfcClient))
                    MessageBox.Show("Create WIPC NIC OK");
                else
                {
                    MessageBox.Show(man.ExMessage);
                    return;
                }
                //---------------------------------------------------------------------------------
                if (await man.ShipfileECD(DN, sfcClient))
                    MessageBox.Show("Create SHIP ECD OK");
                else
                {
                    MessageBox.Show(man.ExMessage);
                    return;
                }
                if (await man.wipcfileECD(txtDN.Text, sfcClient))
                    MessageBox.Show("Create WIPC ECD OK");
                else
                {
                    MessageBox.Show(man.ExMessage);
                    return;
                }
                //---------------------------------------------------------------------------------
                if (await man.ShipfileSuperCap(DN, sfcClient))
                    MessageBox.Show("Create SHIP SuperCap OK");
                else
                {
                    MessageBox.Show(man.ExMessage);
                    return;
                }
                if (await man.wipcfileSuperCap(DN, sfcClient))
                    MessageBox.Show("Create WIPC SuperCap OK");
                else
                {
                    MessageBox.Show(man.ExMessage);
                    return;
                }
                btnship.Enabled = true;
                //---------------------------------------------------------------------------------
            }
        }

        private async void btnsndm_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定生成SNDM文件", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (await man.sndmfile(sfcClient))
                {
                    MessageBox.Show("生成SNDM文件OK");
                }
                else
                {
                    MessageBox.Show(man.ExMessage);
                }
            }
        }

        private async void btnwipc_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定生成WIPC文件", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (await man.wipcfileNIC(txtDN.Text, sfcClient))
                {
                    MessageBox.Show("生成WIPC文件OK");
                }
                else
                {
                    MessageBox.Show(man.ExMessage);
                }
                if (await man.wipcfileECD(txtDN.Text, sfcClient))
                    MessageBox.Show("Create WIPC ECD OK");
                else
                    MessageBox.Show(man.ExMessage);
            }
        }

        private void btnonhb_Click(object sender, EventArgs e)
        {
            /* if (MessageBox.Show("確定生成DailyONHB文件", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
             {
                 if (man.dailyonhbfile())
                 {
                     MessageBox.Show("生成DailyONHB文件OK");
                 }
                 else
                 {
                     MessageBox.Show(man.ExMessage);
                 }
             }*/

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定生成WeeklyONHB文件", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (await man.weeklyonhbfile(sfcClient))
                {
                    MessageBox.Show("生成WeeklyONHB文件OK");
                }
                else
                {
                    MessageBox.Show(man.ExMessage);
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定生成yield文件", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (await man.yieldfile(sfcClient))
                {
                    string sql = "update sfism4.r_brcm_yield_t set Flag='Y',send_time=sysdate where (insert_time < TO_DATE ('" + DateTime.Now.ToString("yyyy/MM/dd") + "'||' 08:00:00', 'YYYY/MM/DD HH24:MI:SS' ) AND passed_quantity = '0' and flag='N') " +
                    " OR (insert_time < TO_DATE ('" + DateTime.Now.ToString("yyyy/MM/dd") + "'||' 08:00:00', 'YYYY/MM/DD HH24:MI:SS' ) AND PASSED_QUANTITY='1' AND TEST_CODE IS NOT NULL AND FLAG ='N') and rev_code is not null and shipping_sn <>'N/A'";
                    await fDal.ExcuteNonQuerySQL(sql, sfcClient);
                    MessageBox.Show("生成yield文件OK");
                }
                else
                {
                    MessageBox.Show(man.ExMessage);
                }
            }

        }

        private async void btninvl_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定生成INVL文件", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (await man.invlfile(sfcClient))
                {
                    MessageBox.Show("生成INVL文件OK");

                }
                else
                {
                    MessageBox.Show(man.ExMessage);
                }
            }
        }

        private async  void btnbdsn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定生成BDSN文件", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                btnbdsn.Enabled = false;
                string DN = txtDN.Text;
                if (DN != "")
                {
                    sentimenic = Int64.Parse(await checkTime(DN)) + DateTime.Now.ToString("HHmmss");
                }
                else
                {
                    sentimenic = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                sentimeecd = (Int64.Parse(sentimenic) + 1).ToString();
                sentimesupercap = (Int64.Parse(sentimenic) + 2).ToString();
                if (await man.bdsnfile(DN,sfcClient))
                {
                    MessageBox.Show("Create BDSN NIC OK");
                }
                else
                {
                    MessageBox.Show(man.ExMessage);
                    return;
                }
                if (await man.bdsnfileECD(DN,sfcClient))
                {
                    MessageBox.Show("Create BDSN ECD OK");
                }
                else
                {
                    MessageBox.Show(man.ExMessage);
                    return;
                }
                if (await man.bdsnfileSuperCap(DN, sfcClient))
                {
                    MessageBox.Show("Create BDSN SuperCap OK");
                    return;
                }
                else
                {
                    MessageBox.Show(man.ExMessage);
                    return;
                }
                btnbdsn.Enabled = true;
            }


        }
        public void SendMailManualUpload(string _FileName)
        {
            mail.mail_service mail = new mail_service();
            mail.sendMail(MAIL_SEND, MAIL_FROM, MAIL_CC, _FileName + " upload success!", _FileName + " upload success!");
        }
        string uploadresult2, file_name2, file_path2;
        private DataTable dt;

        private async void button_manualupload_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(" notice:please select .txt file,it will auto rename to the .dat \n one flie at a time!", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                GetMail();
                OpenFileDialog op = new OpenFileDialog();
                op.InitialDirectory = Directory.GetCurrentDirectory() + "\\Files";
                // op.Filter = "txt files(*.txt)|*.txt";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    string uploadresult, file_name, file_path = op.FileName;
                    string[] tmp = file_path.Split('\\');
                    file_name = tmp[tmp.Length - 1];
                    if (file_name.Substring(0, 5) != "XXAT_")
                    {
                        if (file_name.Substring(0, 5).ToUpper() == "STAND")
                        {
                            uploadresult = sftpuploadcsv(file_path, file_name);
                            if (uploadresult == "true")
                            {
                                SendMailManualUpload(file_name);
                                MessageBox.Show("file:" + file_path + " upload success!");
                                updateEDi(file_name);
                                return;
                            }
                            else
                            {
                                MessageBox.Show(uploadresult);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("file format error! \n" + file_path);
                            return;
                        }
                    }
                    if (MessageBox.Show("Continue to select the next file?Yes will be continue.", "Yes OR NO", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        OpenFileDialog op2 = new OpenFileDialog();
                        op2.InitialDirectory = Directory.GetCurrentDirectory() + "\\Files\\WIPC";
                        op2.Filter = "txt files(*.txt)|*.txt";
                        if (op2.ShowDialog() == DialogResult.OK)
                        {
                            /*string uploadresult2, file_name2,*/
                            file_path2 = op2.FileName;
                            string[] tmp2 = file_path2.Split('\\');
                            file_name2 = tmp2[tmp.Length - 1];
                            if (file_name2.Substring(0, 5) != "XXAT_")
                            {
                                MessageBox.Show("file format error! \n" + file_path2);
                                return;
                            }
                            Thread uploadthread = new Thread(new ThreadStart(createThread));
                            uploadthread.Start();
                            uploadresult = await sftpupload33(file_path, file_name);
                            if (uploadresult == "true")
                            {
                                SendMailManualUpload(file_name);
                                MessageBox.Show("file:" + file_path + " upload success!");
                                updateEDi(file_name);
                            }
                            else
                            {
                                MessageBox.Show(uploadresult);
                            }
                            if (uploadresult2 == "true")
                            {
                                MessageBox.Show("file:" + file_path2 + " upload success!");
                                updateEDi(file_name);
                            }
                            else
                            {
                                MessageBox.Show(uploadresult2);
                            }
                        }
                        else
                        {
                            uploadresult = await sftpupload33(file_path, file_name);
                            if (uploadresult == "true")
                            {
                                SendMailManualUpload(file_name);
                                updateEDi(file_name);
                                MessageBox.Show("file:" + file_path + " upload success!");
                                return;
                            }
                            else
                            {
                                MessageBox.Show(uploadresult);
                                return;
                            }
                        }
                    }
                    else
                    {
                        uploadresult = await sftpupload33(file_path, file_name);
                        if (uploadresult == "true")
                        {
                            SendMailManualUpload(file_name);
                            MessageBox.Show("file:" + file_path + " upload success!");
                            updateEDi(file_name);
                            return;
                        }
                        else
                        {
                            MessageBox.Show(uploadresult);
                            return;
                        }
                    }
                }
            }
        }
        private async void createThread()
        {
            uploadresult2 = await sftpupload33(file_path2, file_name2);
        }
        private string sftpuploadcsv(string path, string path11)
        {
            try
            {
                sFtpWeb sftpweb = new sFtpWeb("ftpprod.broadcom.com", null, "scfoxcn", "vgy76tfc",22);
                sftpweb.uploadsftp(path);
            }
            catch (Exception exx)
            {
                return exx.Message;
            }
            return "true";

        }
        public async Task<DataTable> Isuploaded(string path, SfcHttpClient sfcHttpClient)
        {
            string str = " select * from SFISM4.R_EDI_HAWB2BOX_T where EXTRA ='" + path + "' and invoice is null";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }

        public async Task<DataTable> Insertlogupload(string path, SfcHttpClient sfcHttpClient)
        {
            string sql = string.Format(@"insert into  SFISM4.R_EDI_HAWB2BOX_T
                                                     (PN, YYYYMMDD,EXTRA)
                                                     valueS('{0}', '{1}', '{2}')", getMac(), DateTime.Now.ToString("yyyyMMdd"), path);
            return await fDal.ExcuteSelectSQL(sql, sfcHttpClient);
        }
        public string getMac()
        {
            string mac = null;
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet && nic.OperationalStatus == OperationalStatus.Up)
                {
                    mac = nic.GetPhysicalAddress().ToString();
                }
            };
            return mac;
        }

        private async Task<string> sftpupload33(string path, string path11)
        {
            try
            {
                dt = await Isuploaded(path11, Form1.sfcClient);

                if (dt.Rows.Count > 0)
                {
                    return "File da upload: " + path11 + "";
                }
                else
                {
                    await Insertlogupload(path11, Form1.sfcClient);
                }
                sFtpWeb sftpweb = new sFtpWeb("ftpprod.broadcom.com", null, "scfoxcn", "vgy76tfc",22);
                sftpweb.uploadsftp(path);
            }
            catch (Exception exx)
            {
                return exx.Message;
            }
            return "true";

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            autoToolStripMenuItem_Click(sender,e);
            await connect();
            await SendTestData();
            DataTable dataSP = new DataTable();
            List<SfcParameter> ListPara;
            ListPara = new List<SfcParameter>()
            {
                new SfcParameter { Name = "APNAME", Value = "B2B_BRCM", SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                new SfcParameter { Name = "APVER", Value = APVersion, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }
             };

            dataSP = await fDal.ExcuteSP("SFIS1.SP_CHECK_APVER", ListPara, sfcClient);
            if (dataSP.Rows[0]["res"].ToString() != "OK")
            {
                MessageBox.Show(dataSP.Rows[0]["res"].ToString());
                Environment.Exit(0);
            }
            string DN = txtDN.Text;
            if (DN != "")
            {
                sentimenic = Int64.Parse(await checkTime(DN)) + DateTime.Now.ToString("HHmmss");
            }
            else
            {
                sentimenic = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            sentimeecd = (Int64.Parse(sentimenic) + 1).ToString();
            sentimesupercap = (Int64.Parse(sentimenic) + 2).ToString();
            
        }
        public async Task SendTestData()
        {
            string cmName = await b2b.getCmSite(sfcClient) ;
            DataTable dt = new DataTable();
            dt = await b2b.zgetTestData(sfcClient);
            if (dt is null) return;
            if (dt.Rows.Count > 0 )
            {
                try
                {
                    string path = Directory.GetCurrentDirectory() + "\\Files\\MDSYLD";
                    if (!Directory.Exists(path))
                    { Directory.CreateDirectory(path); }
                    string creatime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    path += "\\XXAT_MDSYLD_"+ cmName + "_" + creatime + ".dat";

                    FileInfo fi = new FileInfo(path);
                    StreamWriter stw = fi.CreateText();
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        string str = "";
                        for (int z = 0; z < dt.Columns.Count; z++)
                        {
                           if(z==0) str += dt.Rows[j][z].ToString();
                           else str +="|"+  dt.Rows[j][z].ToString();
                        }
                        stw.WriteLine(str);
                    }
                    stw.WriteLine("CTL|"+dt.Rows.Count.ToString());
                    stw.Flush();
                    stw.Close();
                    sFtpWeb sftpweb = new sFtpWeb("ftpprod.broadcom.com", null, "scfoxcn", "vgy76tfc", 22);
                    sftpweb.uploadMDSYLD(path);
                    writelog("Send MDSYLD file OK");
                    await fDal.ExcuteNonQuerySQL("update sfism4.R_YIELD_DATA_TRASMITTAL_T set flag=2  where TO_CHAR(YIELD_DATE,'YYYYMMDD') = TO_CHAR(sysdate-1,'YYYYMMDD') ", sfcClient);
                    writelog("Updated MDSYLD file");
                }
                catch(Exception ex)
                {
                    writelog("Send MDSYLD file Fail:"+ex.Message);
                }
            }
            
        }
        public async Task<SfcHttpClient> connect()
        {
            sfcClient = new SfcHttpClient(loginApiUri, loginDB, "helloApp", "123456");
            await sfcClient.GetAccessTokenAsync(empNo, empPass);
            //CheckPC
            DataTable dt = new DataTable();
            string sql = string.Format(@"SELECT * FROM SFIS1.C_MODEL_ATTR_CONFIG_T where TYPE_NAME ='CHECKPC_B2B' and ATTRIBUTE_NAME = '{0}'",zIP);
            dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
            if (dt.Rows.Count == 0)
            {
               MessageBox.Show("IP PC:" + zIP + "không được bật chương trình BRCM_B2B", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
               Environment.Exit(0);
            }

            //****************************************
            return sfcClient;
        }

        private void button3_Click(object sender, EventArgs e)
        {
             timer1_Tick(sender, e);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            timer1_Tick(sender, e);
        }

        private void iCConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BRCM_IC_CONFIG icform = new BRCM_IC_CONFIG(sfcClient);
            icform.Activate();
            icform.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("B2B_BRCM"))
            {
                process.Kill();
            }
        }

        private async void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zdt = new DataTable();
            zdt = await b2b.zgettime(sfcClient);
            if (zdt.Rows.Count == 0)
            {
                await connect();
                zdt = await b2b.zgettime(sfcClient);
                if (zdt.Rows.Count > 0)
                    writelog("Connect other session OK");
                else writelog("Connect other session FAIL");
            }
            fromTime = zdt.Rows[0][0].ToString();
            group1.Text = "Manual Mode from "+ fromTime;
            timer1.Enabled = false;
            autoToolStripMenuItem.Checked = false;
            //---------------------------------------
            manualToolStripMenuItem.Checked = true;
            btnship.Enabled = true;
            btnbdsn.Enabled = true;
            btnupload.Enabled = true;
            timer2.Enabled = true;
        }

        private void queryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Query pnform = new Query(sfcClient);
            pnform.Activate();
            pnform.Show();
        }

        private void autoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            group1.Text = "Auto Mode";
            timer1.Enabled = true;
            autoToolStripMenuItem.Checked = true;
            //---------------------------------------
            manualToolStripMenuItem.Checked = false;
            btnship.Enabled = false;
            btnbdsn.Enabled = false;
            btnupload.Enabled = false;
            timer2.Enabled = false;
        }

        private async void getFileAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(txtDN.Text=="      ")
            {
                if (await b2b.ShipfileNIC(sfcClient)) { }
                else
                {
                    MessageBox.Show(b2b.ExMessage + "\n" + "SHIP文件生成失敗");
                    return;
                }
                if (await b2b.wipcfileNIC(sfcClient)) { }
                else
                {
                    MessageBox.Show(b2b.ExMessage + "\n" + "SHIP文件生成失敗");
                    return;
                }
                if (await b2b.ShipfileECD(sfcClient)) { }
                else
                {
                    MessageBox.Show(b2b.ExMessage + "\n" + "SHIP文件生成失敗");
                    return;
                }
                if (await b2b.wipcfileECD(sfcClient)) { }
                else
                {
                    MessageBox.Show(b2b.ExMessage + "\n" + "SHIP文件生成失敗");
                    return;
                }
                if (await b2b.ShipfileSuperCap(sfcClient)) { }
                else
                {
                    MessageBox.Show(b2b.ExMessage + "\n" + "SHIP文件生成失敗");
                    return;
                }
                if (await b2b.wipcfileSUPERCAP(sfcClient)) { }
                else
                {
                    MessageBox.Show(b2b.ExMessage + "\n" + "SHIP文件生成失敗");
                    return;
                }
                MessageBox.Show("Ship and WipC Finish");
                if (await b2b.bdsnfile(sfcClient))
                {
                    
                }
                else
                {
                    MessageBox.Show(b2b.ExMessage + "\n" + "SHIP文件生成失敗");
                    return;
                }
                if (await b2b.bdsnfileECD(sfcClient))
                {
                    
                }
                else
                {
                    MessageBox.Show(b2b.ExMessage + "\n" + "SHIP文件生成失敗");
                    return;
                }
                if (await b2b.bdsnfileSuperCap(sfcClient))
                {
                    
                }
                else
                {
                    MessageBox.Show(b2b.ExMessage + "\n" + "SHIP文件生成失敗");
                    return;
                }
                MessageBox.Show("BDSN Finish");
            }
            else
            {
                MessageBox.Show("No privilege");
                return;
            }

        }

        private void pNConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BRCM_PN_CONFIG pnform = new BRCM_PN_CONFIG(sfcClient);
            pnform.Activate();
            pnform.Show();
        }
        private async void updateEDi(string filename)
        {
            string sql = string.Format(@"update sfism4.R_EDI_HAWB2BOX_T set BOX ='Y'
                                        where SUBSTR(EXTRA,1, LENGTH(EXTRA) -4)   =(
                                        select substr(FILENAME,0,length(FILENAME) -4)from(
                                        select '{0}' FILENAME from dual))", filename);
           await fDal.ExcuteNonQuerySQL(sql, sfcClient);
        }
        
    }
}
