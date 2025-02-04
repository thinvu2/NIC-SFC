using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using myFtpApp;
using BRCM_B2B.mail;
using Sfc.Library.HttpClient;
using System.Threading.Tasks;
using Sfc.Core.Parameters;
using Newtonsoft.Json;
using System.Net.NetworkInformation;

namespace BRCM_B2B
{

    class B2B
    {
        private string exmessage;
        private string version;
        private Dictionary<string, int> DN_LOT_NIC;
        private Dictionary<string, int> DN_LOT_ECD;
        private Dictionary<string, int> DN_LOT_SUPERCAP;

        public Dictionary<string, int> DN_MO_NUM
        {
            get { return DN_LOT_NIC; }
            set { DN_LOT_NIC = value; }
        }
        public Dictionary<string, int> DN_MO_NUMECD
        {
            get { return DN_LOT_ECD; }
            set { DN_LOT_ECD = value; }
        }
        public Dictionary<string, int> DN_MO_NUMSUPERCAP
        {
            get { return DN_LOT_SUPERCAP; }
            set { DN_LOT_SUPERCAP = value; }
        }
        public string Version
        {
            get { return version; }
            set { version = value; }
        }
        public string ExMessage
        {
            get { return exmessage; }
            set { exmessage = value; }
        }
        DataTable dt = new DataTable();
        private static string path_ship = "";
        private static string path11_ship = "";
        private static string path_shipECD = "";
        private static string ShipNameECD = "";
        private static string path_shipSUPERCAP = "";
        private static string ShipNameSUPERCAP = "";
        private static string path_wipsSUPERCAP = "";
        private static string WipsNameSUPERCAP = "";
        public string MAIL_SEND;
        public string MAIL_CC;
        public string MAIL_FROM;
        DAL fDal = new DAL();
        zFunction z = new zFunction();
        public static SfcHttpClient sfcClient;


        public void GoToSendMail(string body)
        {
            mail.mail_service mail = new mail_service();
            mail.sendMail(MAIL_SEND, MAIL_FROM, MAIL_CC, body, body);
        }
        public async Task<bool> ShipfileNIC(SfcHttpClient sfcClient)
        {
            try
            {
                DN_MO_NUM = new Dictionary<string, int>();
                DataTable dt_mo_tcom = new DataTable();//根據TCOM穫取每個卡通下的數量，按工單分組
                DataTable dthead = new DataTable();
                DataTable dtcarton = new DataTable();
                DataTable dtDN = new DataTable();
                DataTable dtmo = new DataTable();
                DataTable dtlic = new DataTable();
                path_ship = Directory.GetCurrentDirectory() + "\\Files\\SHIP";
                if (!Directory.Exists(path_ship))
                { Directory.CreateDirectory(path_ship); }
                string contentTime =await getcontentTime(sfcClient);
                string titleTime =await gettitleDate(sfcClient) + "080000";
                path_ship += "\\XXAT_SHIP_B_FOXVN_BD_" + titleTime + ".dat";
                path11_ship = "XXAT_SHIP_B_FOXVN_BD_" + titleTime + ".dat";
                dtlic = await fDal.ExcuteSelectSQL("select count(*),carton_no from sfism4.r_sec_lic_link_t group by carton_no having count(*)>1", sfcClient);
                for (int i = 0; i < dtlic.Rows.Count; i++)
                {
                   await fDal.ExcuteNonQuerySQL("update  sfism4.r_sec_lic_link_t set  carton_no=carton_no||'OLD' where carton_no='" + dtlic.Rows[i]["CARTON_NO"] + "' and link_time <>(select max(link_time) from sfism4.r_sec_lic_link_t where carton_no= '" + dtlic.Rows[i]["CARTON_NO"] + "')", sfcClient);
                }
                dthead =await z.NICdtHeaderShip("AUTO","",sfcClient);
                int headnum = dthead.Rows.Count;
                if (headnum == 0)
                {
                    GoToSendMail("NIC SHIP IS NULL!");
                    return true;
                }
                FileInfo fi = new FileInfo(path_ship);
                StreamWriter stw = fi.CreateText();
                int lotnum = 0;
                for (int i = 0; i < headnum; i++)
                {
                        string strhead = await z.NICGetStringHeader(contentTime,dthead, i);
                        stw.WriteLine(strhead);
                        dtcarton =await z.NICShipCartonDT(dthead.Rows[i]["TCOM"].ToString(), sfcClient);
                        int cartonnum = dtcarton.Rows.Count;
                        if (cartonnum == 0)
                        {
                            exmessage = "Header Qty与Lot Total Qty不符";
                            stw.Close();
                            return false;
                        }
                        lotnum += cartonnum;
                        for (int j = 0; j < cartonnum; j++)
                        {
                            if (dthead.Rows[i]["Quantity Shipped"].ToString() != dtcarton.Rows[j]["LOTTOTAL"].ToString())
                            {
                                exmessage = "Header Qty与Lot Total Qty不符";
                                stw.Close();
                                return false;
                            }
                            dtmo =await z.NICShipMoDT(dtcarton.Rows[j]["MCARTON_NO"].ToString(), sfcClient);

                            string res = await z.NICGetVer(dthead.Rows[i]["item"].ToString(), dtcarton, j, sfcClient);
                            version = res.Split(',')[0];
                            if (res.Split(',')[1] != "OK")
                            {
                                exmessage = res.Split(',')[1];
                                return false;
                            }
                            string LotID = await z.ShipGetLotID("NIC", dtmo, dtcarton, j);
                            if (DN_MO_NUM.ContainsKey(dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID))
                            {
                                DN_MO_NUM[dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID] += Convert.ToInt32(dtcarton.Rows[j]["NUM"].ToString());
                            }
                            else{
                                DN_MO_NUM.Add((dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID), Convert.ToInt32(dtcarton.Rows[j]["NUM"].ToString()));
                            }
                            string strlot = await z.NICGetStringContent(contentTime,LotID, version, dthead, i, dtcarton, j);
                            stw.WriteLine(strlot);
                        }

                }
                int total = headnum + lotnum;
                string strctl = "CTL|" + total.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();
                GoToSendMail("NIC SHIP FILE 生成成功!");
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task< bool> ShipfileECD(SfcHttpClient sfcClient)
        {
            try
            {
                DN_MO_NUMECD = new Dictionary<string, int>();
                DataTable dthead = new DataTable();
                DataTable dtcarton = new DataTable();
                //DataTable dtmo = new DataTable(); 
                DataTable dtDN = new DataTable(); 
                DataTable dtlic = new DataTable();
                string contentTime = await getcontentTime(sfcClient);
                string titleTime = await gettitleDate(sfcClient) + "080100";
                path_shipECD = Directory.GetCurrentDirectory() + "\\Files\\SHIP";
                if (!Directory.Exists(path_shipECD))
                { Directory.CreateDirectory(path_shipECD); }
                path_shipECD += "\\XXAT_SHIP_B_FOXVN_BD_" + titleTime + ".dat";
                ShipNameECD = "XXAT_SHIP_B_FOXVN_BD_" + titleTime + ".dat";
                FileInfo fi = new FileInfo(path_shipECD);
                StreamWriter stw = fi.CreateText();

                dthead =await z.ECDdtHeaderShip("AUTO", "",sfcClient);
                int headnum = dthead.Rows.Count;
                if (headnum == 0)
                {
                    GoToSendMail("ECD SHIP IS NULL!");
                    return true;
                }
                int lotnum = 0;
                for (int i = 0; i < headnum; i++)
                {
                        string strhead = await z.ECDGetStringHeader(contentTime,dthead, i);
                        stw.WriteLine(strhead);
                        dtcarton =await z.ECDShipCartonDT(dthead.Rows[i]["TCOM"].ToString(), sfcClient);
                        int cartonnum = dtcarton.Rows.Count;
                        if (cartonnum == 0)
                        {
                            exmessage = "Header Qty与Lot Total Qty不符";
                            return false;
                        }
                        lotnum += cartonnum;
                        for (int j = 0; j < cartonnum; j++)
                        {
                            if (dthead.Rows[i]["Quantity Shipped"].ToString() != dtcarton.Rows[j]["LOTTOTAL"].ToString())
                            {
                                exmessage = "Header Qty与Lot Total Qty不符";
                                return false;
                            }
                            //----Ver
                            string res = await z.ECDGetVer(dtcarton, j, sfcClient);
                            version = res.Split(',')[0];
                            if (res.Split(',')[1] != "OK")
                            {
                                exmessage = res.Split(',')[1];
                                return false;
                            }
                            string LotID = await z.ShipGetLotID("ECD", null, dtcarton, j);
                            if (DN_MO_NUMECD.ContainsKey(dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID))
                            {
                                DN_MO_NUMECD[dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID] += Convert.ToInt32(dtcarton.Rows[j]["NUM"].ToString());
                            }
                            else
                            {
                                DN_MO_NUMECD.Add((dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID), Convert.ToInt32(dtcarton.Rows[j]["NUM"].ToString()));
                            }
                            string strlot = await z.ECDGetStringContent(contentTime,LotID, version, dthead, i, dtcarton, j);
                            stw.WriteLine(strlot);
                        }
                }

                int total = headnum + lotnum;
                string strctl = "CTL|" + total.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();
                GoToSendMail("Create SHIP FILE ECD OK!");
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task<bool> wipcfileNIC(SfcHttpClient sfcClient)
        {
            try
            {
                DataTable dtic = new DataTable();
                DataTable dtlot = new DataTable();
                DataTable dtmo = new DataTable();
                DataTable dtminus = new DataTable();
                DataTable dtDN = new DataTable();
                DataTable dtlic = new DataTable();
                DataTable dticqty = new DataTable();
                string path = Directory.GetCurrentDirectory() + "\\Files\\WIPC";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string contentTime = await getcontentTime(sfcClient);
                string titleTime = await gettitleDate(sfcClient) + "080000";
                path += "\\XXAT_WIPC_B_FOXVN_BD_" + titleTime + ".dat";
                string path11 = "XXAT_WIPC_B_FOXVN_BD_" + titleTime + ".dat";
                int num = 0;

                dtlic = await fDal.ExcuteSelectSQL("select a.model_name from  sfism4.r_bpcs_invoice_t a, sfis1.c_brcm_pn_t b  WHERE a.model_name = b.model_name and a.invoice IN ( SELECT dn_no FROM sfism4.r_sap_dn_detail_t WHERE UPPER (ship_address) IN " +
                    " ('AVAGO TECHNOLOGIES INTERNATIONAL SALES@C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN','C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN' ))  AND TO_CHAR (a.finish_date, 'YYYYMMDD') = to_char(sysdate,'YYYYMMDD') Minus Select model_name from sfis1.c_brcm_ic_t", sfcClient );
                if (dtlic.Rows.Count > 0)
                {
                    exmessage = "有机种没有配置sfis1.c_brcm_ic_t表";
                    return false;
                }
                dtic =await z.NICWipcdt("AUTO","",sfcClient);
                if (dtic.Rows.Count == 0)
                {
                    GoToSendMail("NIC WIPC IS NULL!");
                    return true;
                }
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                for (int i = 0; i < dtic.Rows.Count; i++)
                {
                    string icqty = " SELECT *  FROM sfis1.c_brcm_ic_t  WHERE MODEL_NAME ='" + dtic.Rows[i]["model"].ToString() + "' ";
                    dticqty = await fDal.ExcuteSelectSQL(icqty, sfcClient);

                    int totalicnum = dticqty.Rows.Count;
                    dtmo = await z.NICWipCmoDT(dtic.Rows[i]["Shipment Number"].ToString(), sfcClient);

                    for (int j = 0; j < dtmo.Rows.Count; j++)
                    {
                        int totalcount = 0;
                        string mo = dtmo.Rows[j]["mo_number"].ToString();
                        string shipno = dtmo.Rows[j]["ship_no"].ToString();

                        if (totalicnum > 1)
                        {
                            totalcount = Convert.ToInt32(dtmo.Rows[j]["count"]) / 2;
                        }
                        else
                        {
                            totalcount = Convert.ToInt32(dtmo.Rows[j]["count"]);
                        }

                        DataTable dataSP = new DataTable();
                        List<SfcParameter> ListPara;
                        ListPara = new List<SfcParameter>()
                            {
                                new SfcParameter { Name = "mo", Value = mo, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "ic", Value = dtic.Rows[i]["Component Item"].ToString(), SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "shipno", Value = shipno, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "num", Value = totalcount, SfcParameterDataType = SfcParameterDataType.Int32, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "multiple", Value = Convert.ToInt32(dtic.Rows[i]["Quantity Consumed"].ToString()), SfcParameterDataType = SfcParameterDataType.Int32, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "times", Value = DateTime.Now.ToString("yyyyMMdd"), SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }

                            };

                        dataSP = await fDal.ExcuteSP("sfis1.BRCM_SHIPIC2_VN", ListPara, sfcClient);

                        if (dataSP.Rows[0]["res"].ToString() != "OK")
                        {
                            stw.Close();
                            throw new Exception(exmessage + "\n" + dtic.Rows[i]["Component Item"].ToString() + dataSP.Rows[0]["res"].ToString());
                        }

                        string strwipc = string.Empty;
                        string col22 = string.Empty;
                        dtminus = await fDal.ExcuteSelectSQL("select mo_number,sum(qty) QTY from  sfism4.r_ic_minus_vn_t where mo_number='" + mo + "' and ship_no='" + shipno + "' and minus_time=to_char(sysdate,'YYYYMMDD') and brcm_ic='" + dtic.Rows[i]["Component Item"].ToString() + "' group by mo_number", sfcClient);

                        if (dtminus.Rows.Count > 0)
                        {
                            for (int k = 0; k < dtminus.Rows.Count; k++)
                            {
                                if (DN_MO_NUM.ContainsKey(dtic.Rows[i]["Shipment Number"].ToString().Trim() + mo.Trim()))//為解決因為合箱產生的wipc的mo比ship多的問題
                                {
                                    col22 = "|V" + dtic.Rows[i]["Component Item"].ToString().Substring(3);
                                    if (dtic.Rows[i]["Component Item"].ToString() == "2SZ3-0001")
                                    {
                                        col22 = "|V" + dtic.Rows[i]["Component Item"].ToString();
                                    }
                                    string sql = $@"SELECT component_qty  FROM sfis1.c_brcm_ic_t where model_name='{dtic.Rows[i]["MODEL"].ToString()}'";
                                    dtlot = await fDal.ExcuteSelectSQL(sql, sfcClient);

                                    int use_level = string.IsNullOrEmpty(dtlot.Rows[0][0].ToString()) ? 1 : Convert.ToInt32(dtlot.Rows[0][0].ToString());
                                    strwipc = "WIPC|B_FOXVN_BD|" + Form1.sentimenic + "||" + dtic.Rows[i]["Shipment Number"].ToString() + "|" +
                                        dtic.Rows[i]["Assembly Item"].ToString() + "|" + dtic.Rows[i]["Component Item"].ToString() + "|EA|" +
                                        DN_MO_NUM[dtic.Rows[i]["Shipment Number"].ToString().Trim() + mo.Trim()] * use_level + "|" + dtic.Rows[i]["CHIPDEPART"].ToString() + "|||||||||||" + mo + col22 + "|" + DN_MO_NUM[dtic.Rows[i]["Shipment Number"].ToString().Trim() + mo.Trim()] * use_level + "||||";
                                    stw.WriteLine(strwipc);
                                    num++;
                                }
                            }

                            dtminus = await fDal.ExcuteSelectSQL("select sum(qty) from  sfism4.r_ic_minus_vn_t where mo_number='" + mo + "' and ship_no='" + shipno + "' and minus_time=to_char(sysdate,'YYYYMMDD') and brcm_ic='" + dtic.Rows[i]["Component Item"].ToString() + "'", sfcClient);

                            if (Convert.ToInt32(dtminus.Rows[0][0].ToString()) != totalcount)
                            {
                                stw.Close();
                                throw new Exception(exmessage + "\n" + mo + " " + shipno + " minus sum qty<>mo qty");
                            }
                        }
                        else
                        {
                            stw.Close();
                            throw new Exception(exmessage + "\n" + "r_ic_minus_t无" + mo + " " + shipno + " 记录");
                        }
                    }
                }
                stw.WriteLine("CTL|" + num);
                stw.Flush();
                stw.Close();
                GoToSendMail("NIC WIPC FILE 生成成功!");
                string ftpstr = await sftpupload33(path_ship, path11_ship);//上傳ship文件,因為客戶要求ship和wipc上傳間隔不能超過1min，因此ship放到此處upload
                if (ftpstr != "true")
                {
                    exmessage = ftpstr;
                    throw new Exception(exmessage + "\n上傳ftp失敗");
                }
                GoToSendMail("NIC SHIP Upload FTP Successfully!");

                ftpstr = await sftpupload33(path, path11);
                if (ftpstr != "true")
                {
                    exmessage = ftpstr;
                    stw.Close();
                    throw new Exception(exmessage + "\n上傳ftp失敗");
                }
                GoToSendMail("NIC WIPC Upload FTP Successfully!");
                return true;

            }
            catch (Exception ex)
            {

                exmessage = ex.Message;
                return false;
            }
        }
        public async Task<bool> wipcfileECD(SfcHttpClient sfcClient)
        {
            try
            {
                DataTable dtic = new DataTable();
                DataTable dtlot = new DataTable();
                DataTable dtmo = new DataTable();
                DataTable dtminus = new DataTable();
                DataTable dtDN = new DataTable(); 
                 DataTable dtlic = new DataTable();
                DataTable dticqty = new DataTable();
                string path = Directory.GetCurrentDirectory() + "\\Files\\WIPC";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string contentTime =await getcontentTime(sfcClient);
                string titleTime = await gettitleDate(sfcClient) + "080100";
                path += "\\XXAT_WIPC_B_FOXVN_BD_" + titleTime + ".dat";
                string path11 = "XXAT_WIPC_B_FOXVN_BD_" + titleTime + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                int num = 0;

                dtic =await z.ECDWipcdt("AUTO","",sfcClient);
                if (dtic.Rows.Count == 0)
                {
                    GoToSendMail("ECD WIPC IS NULL!");
                    return true;
                }
                for (int i = 0; i < dtic.Rows.Count; i++)
                {
                    dtmo = await z.ECDWipCmoDT(dtic.Rows[i]["Shipment Number"].ToString(), sfcClient);
                    string mo = string.Empty;
                    string shipno = string.Empty;
                    int totalcount = 0;
                    int multiple = 0;
                    string strics = string.Empty;
                    for (int j = 0; j < dtmo.Rows.Count; j++)
                    {
                        mo = dtmo.Rows[j]["mo_number_old"].ToString();
                        shipno = dtmo.Rows[j]["ship_no"].ToString();
                        totalcount = int.Parse(dtmo.Rows[j]["count"].ToString());
                        multiple = int.Parse(dtmo.Rows[j]["component_qty"].ToString());
                        strics = dtmo.Rows[j]["component_item"].ToString();

                        DataTable dataSP = new DataTable();
                        List<SfcParameter> ListPara;
                        ListPara = new List<SfcParameter>()
                            {
                                new SfcParameter { Name = "mo", Value = mo, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "ic", Value = strics, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "shipno", Value = shipno, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "num", Value = totalcount, SfcParameterDataType = SfcParameterDataType.Int32, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "multiple", Value = multiple, SfcParameterDataType = SfcParameterDataType.Int32, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "times", Value = DateTime.Now.ToString("yyyyMMdd"), SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }

                            };

                        dataSP = await fDal.ExcuteSP("sfis1.BRCM_SHIPIC2_VN", ListPara, sfcClient);

                        if (dataSP.Rows[0]["res"].ToString() != "OK")
                        {
                            throw new Exception(exmessage + "\n" + strics + dataSP.Rows[0]["res"].ToString());
                        }

                        string strwipc = string.Empty;
                        dtminus = await fDal.ExcuteSelectSQL("select mo_number,sum(qty) QTY from  sfism4.r_ic_minus_vn_t where mo_number='" + mo + "' and ship_no='" + shipno + "' and minus_time='" + DateTime.Now.ToString("yyyyMMdd") + "' and brcm_ic='" + strics + "' group by mo_number", sfcClient);
                        //2021/3/25 客戶say LOT_CODE改為統一格式,為V替換BCM後的IC名稱 ex:BCM5719A1KFBG --> V5719A1KFBG 而客戶要求，file的line 要按照mo_number合併,一個mo_number隻能在一個invoice下出現一次
                        if (dtminus.Rows.Count > 0)
                        {
                            for (int k = 0; k < dtminus.Rows.Count; k++)
                            {
                                if (DN_MO_NUMECD.ContainsKey(dtic.Rows[i]["Shipment Number"].ToString().Trim() + mo.Trim()))//??????????wipc?mo?ship????
                                {
                                    string sql = $@"SELECT component_qty  FROM sfis1.c_brcm_ic_t where model_name='{dtic.Rows[i]["MODEL"].ToString()}' and COMPONENT_ITEM = '{strics}' ";
                                    dtlot = await fDal.ExcuteSelectSQL(sql, sfcClient);
                                    int use_level = string.IsNullOrEmpty(dtlot.Rows[0][0].ToString()) ? 1 : Convert.ToInt32(dtlot.Rows[0][0].ToString());
                                    strwipc = "WIPC|B_FOXVN_BD|" + contentTime + "||" + dtic.Rows[i]["Shipment Number"].ToString() + "|" +
                                        dtic.Rows[i]["Assembly Item"].ToString() + "|" + dtmo.Rows[j]["vender_item"].ToString() + "|EA|" +
                                        DN_MO_NUMECD[dtic.Rows[i]["Shipment Number"].ToString().Trim() + mo.Trim()] * use_level + "|" + dtic.Rows[i]["CHIPDEPART"].ToString() + "|||||||||||" + mo + "|" + "V" + dtmo.Rows[j]["vender_item"].ToString() + "|" + DN_MO_NUMECD[dtic.Rows[i]["Shipment Number"].ToString().Trim() + mo.Trim()] * use_level + "||||";
                                    stw.WriteLine(strwipc);
                                    num++;
                                }
                            }

                            dtminus = await fDal.ExcuteSelectSQL("select sum(qty) from  sfism4.r_ic_minus_vn_t where mo_number='" + mo + "' and ship_no='" + shipno + "' and minus_time='" + DateTime.Now.ToString("yyyyMMdd") + "'and brcm_ic='" + strics + "' ", sfcClient);

                            if (Convert.ToInt32(dtminus.Rows[0][0].ToString()) != totalcount)
                            {
                                throw new Exception(exmessage + "\n" + mo + " " + shipno + " " + DateTime.Now.ToString("yyyyMMdd") + "minus sum qty<>mo qty");

                            }
                        }
                        else
                        {
                            throw new Exception(exmessage + "\n" + "r_ic_minus_t无" + mo + " " + shipno + " " + DateTime.Now.ToString("yyyyMMdd") + "记录");

                        }

                    }
                }
                int total = num;
                stw.WriteLine("CTL|" + total.ToString());
                stw.Flush();
                stw.Close();

                GoToSendMail("Create WIPC ECD FILE OK!");
                string ftpstr = await sftpupload33(path_shipECD, ShipNameECD);//upload ship file ECD
                if (ftpstr != "true")
                {
                    exmessage = ftpstr;
                    throw new Exception(exmessage + "\n上傳ftp失敗");
                }
                GoToSendMail("ECD SHIP Upload FTP Successfully!");

                ftpstr = await sftpupload33(path, path11); // Upload WIPC file ECD file path, wipcname
                if (ftpstr != "true")
                {
                    exmessage = ftpstr;
                    stw.Close();
                    throw new Exception(exmessage + "\n上傳ftp失敗");
                }
                GoToSendMail("ECD WIPC Upload FTP Successfully!");

                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task< bool> weeklyonhbfile(SfcHttpClient sfcClient)
        {
            try
            {
                DataTable dticsum = new DataTable();
                DataTable dtcrec = new DataTable();
                DataTable dtforecast = new DataTable();
                DataTable dticin = new DataTable();

                string path = Directory.GetCurrentDirectory() + "\\Files\\ONHB";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string creatime =await getcontentTime(sfcClient);
                path += "\\XXAT_ONHB_B_FOXVN_BD_" + creatime + ".dat";
                string path11 = "XXAT_ONHB_B_FOXVN_BD_" + creatime + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();

                dticsum = await icsum(sfcClient);
                int icnum = dticsum.Rows.Count;
                int crecnum = 0;
                for (int i = 0; i < icnum; i++)
                {
                    string kp = dticsum.Rows[i]["Item"].ToString();
                    int ics = Convert.ToInt32(dticsum.Rows[i]["icsum"]);
                    if (ics < 0)
                    {
                        stw.Close();
                        throw new Exception(kp + "的数量小于0");
                    }
                    else if (ics > 0)
                    {
                        dtcrec = await crecsum(kp, sfcClient);
                        crecnum += dtcrec.Rows.Count;
                        for (int j = 0; j < dtcrec.Rows.Count; j++)
                        {
                            string lot_code = dtcrec.Rows[j]["Lot"].ToString().Trim();
                            if (!lot_code.Contains("BCM"))
                            {
                                if (("BCM" + lot_code).Length > 15)//客戶要求以後的lot_code統一改成 "V"+IC
                                {
                                    if (lot_code.Substring(0, 1) != "I")
                                    {
                                        lot_code = "V" + lot_code;
                                    }
                                    else
                                    {
                                        lot_code = "V" + lot_code.Substring(1);
                                    }
                                }
                            }
                            else
                            {
                                lot_code = "V" + lot_code.Substring(3);
                            }
                            string str1 = "ONHB|B_FOXVN_BD|" + creatime + "|" + kp + "|" + dticsum.Rows[i]["CHIP_DEPARTMENT_CODE"].ToString() + "|EA|" +
                                dtcrec.Rows[j]["Cut-Off Date"].ToString() + "|" + dtcrec.Rows[j]["lotsum"].ToString() + "|N|||CREC|" + lot_code + "||||||||||||";

                            stw.WriteLine(str1);

                        }
                    }
                }
                string strctl = "CTL|" + crecnum.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();
                if (crecnum == 0)
                {
                    exmessage = "CREC is 0";
                    return true;
                }
                string ftpstr = await sftpupload33(path, path11);
                if (ftpstr != "true")
                {
                    stw.Close();
                    exmessage = ftpstr;
                    throw new Exception(exmessage + "\n上傳ftp失敗");
                }
                GoToSendMail("NIC ONHB Upload FTP Successfully!");

                return true;
            }
            catch (Exception ex)
            {

                exmessage = ex.Message;
                return false;
            }

        }
        public async Task<bool> ShipfileSuperCap(SfcHttpClient sfcClient)
        {
            try
            {
                DN_MO_NUMSUPERCAP = new Dictionary<string, int>();
                path_shipSUPERCAP = Directory.GetCurrentDirectory() + "\\Files\\SHIP";
                if (!Directory.Exists(path_shipSUPERCAP))
                { Directory.CreateDirectory(path_shipSUPERCAP); }

                DataTable dthead = new DataTable();
                DataTable dtcarton = new DataTable();
                DataTable dtDN = new DataTable();
                DataTable dtlic = new DataTable();
                //DataTable dtver = new DataTable();
                string contentTime = await getcontentTime(sfcClient);
                string titleTime = await gettitleDate(sfcClient) + "080200";

                string path = Directory.GetCurrentDirectory() + "\\Files\\SHIP";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                path_shipSUPERCAP += "\\XXAT_SHIP_B_FOXVN_BD_" + titleTime + ".dat";
                ShipNameSUPERCAP = "XXAT_SHIP_B_FOXVN_BD_" + titleTime + ".dat";
                string path11 = "XXAT_SHIP_B_FOXVN_BD_" + titleTime + ".dat";
                string filename = "XXAT_SHIP_B_FOXVN_BD_" + titleTime + ".dat";


                dthead = await z.SupercapdtHeaderShip("AUTO", "",sfcClient);
                int headnum = dthead.Rows.Count;
                if (headnum == 0)
                {
                    GoToSendMail("SUPERCAP SHIP IS NULL!");
                    return true;
                }
                FileInfo fi = new FileInfo(path_shipSUPERCAP);
                StreamWriter stw = fi.CreateText();

                int lotnum = 0;
                for (int i = 0; i < headnum; i++)
                {
                    string strhead = await z.SuperCapGetStringHeader(contentTime,dthead, i);
                    stw.WriteLine(strhead);
                    dtcarton = await z.SuperCapShipCartonDT(dthead.Rows[i]["TCOM"].ToString(), sfcClient);
                    int cartonnum = dtcarton.Rows.Count;
                    if (cartonnum == 0)
                    {
                        exmessage = "Header Qty与Lot Total Qty不符";
                        return false;
                    }
                    lotnum += cartonnum;
                    for (int j = 0; j < cartonnum; j++)
                    {
                        if (dthead.Rows[i]["Quantity Shipped"].ToString() != dtcarton.Rows[j]["LOTTOTAL"].ToString())
                        {
                            exmessage = "Header Qty与Lot Total Qty不符";
                            return false;
                        }
                        //----Ver
                        string res = await z.SuperCapGetVer(dtcarton, j, sfcClient);
                        version = res.Split(',')[0];
                        if (res.Split(',')[1] != "OK")
                        {
                            exmessage = res.Split(',')[1];
                            return false;
                        }
                        //----Ver
                        string LotID = await z.ShipGetLotID("SUPERCAP", null, dtcarton, j);
                        if (DN_MO_NUMSUPERCAP.ContainsKey(dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID))
                        {
                            DN_MO_NUMSUPERCAP[dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID] += Convert.ToInt32(dtcarton.Rows[j]["NUM"].ToString());
                        }
                        else
                        {
                            DN_MO_NUMSUPERCAP.Add((dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID), Convert.ToInt32(dtcarton.Rows[j]["NUM"].ToString()));
                        }
                        string strlot = await z.SuperCapGetStringContent(contentTime,LotID, version, dthead, i, dtcarton, j);
                        stw.WriteLine(strlot);
                    }
                }
                //end SUPPERCAP

                int total = headnum + lotnum;
                string strctl = "CTL|" + total.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();
                GoToSendMail("Create WIPC SUPERCAP FILE OK!");
                
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task<bool> wipcfileSUPERCAP(SfcHttpClient sfcClient)
        {
            try
            {
                DataTable dthead = new DataTable();
                dthead = await z.SupercapdtHeaderShip("AUTO", "", sfcClient);
                if (dthead.Rows.Count == 0) return true;
                string path = Directory.GetCurrentDirectory() + "\\Files\\WIPC";
                path_wipsSUPERCAP = Directory.GetCurrentDirectory() + "\\Files\\WIPC";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string titleTime = await gettitleDate(sfcClient);
                titleTime = titleTime + "080200";
                path += "\\XXAT_WIPC_B_FOXVN_BD_" + titleTime + ".dat";
                string path11 = "XXAT_WIPC_B_FOXVN_BD_" + titleTime + ".dat";
                path_wipsSUPERCAP += "\\XXAT_WIPC_B_FOXVN_BD_" + titleTime + ".dat";
                WipsNameSUPERCAP = "XXAT_WIPC_B_FOXVN_BD_" + titleTime + ".dat";

                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                int num = 0;
                int total = num;
                
                stw.WriteLine("CTL|" + total.ToString());
                stw.Flush();
                stw.Close();
                string ftpstr = await sftpupload33(path_wipsSUPERCAP, WipsNameSUPERCAP); // Upload WIPC file ECD file path, wipcname
                if (ftpstr != "true")
                {
                    exmessage = ftpstr;
                    stw.Close();
                    throw new Exception(exmessage + "\n上傳ftp失敗");
                }
                GoToSendMail("SUPERCAP WIPC Upload FTP Successfully!");
                ftpstr = await sftpupload33(path_shipSUPERCAP, ShipNameSUPERCAP);// upload ship file ECD
                if (ftpstr != "true")
                {
                    exmessage = ftpstr;
                    throw new Exception(exmessage + "\n上傳ftp失敗");
                }
                GoToSendMail("SUPERCAP SHIP Upload FTP Successfully!");
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task<bool> bdsnfile(SfcHttpClient sfcHttpClient)
        {
            try
            {
                DataTable dtbdsn = new DataTable();
                DataTable dtlic = new DataTable();

                string path = Directory.GetCurrentDirectory() + "\\Files\\BDSN";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string titleTime =await gettitleDate(sfcHttpClient)+"140000";
                path += "\\XXAT_BDSN_B_FOXVN_BD_" + titleTime + ".dat";
                string path11 = "XXAT_BDSN_B_FOXVN_BD_" + titleTime + ".dat";

                dtlic =await fDal.ExcuteSelectSQL("select count(*),carton_no from sfism4.r_sec_lic_link_t group by carton_no having count(*)>1", sfcHttpClient );
                for (int i = 0; i < dtlic.Rows.Count; i++)
                {
                    await fDal.ExcuteNonQuerySQL("update  sfism4.r_sec_lic_link_t set  carton_no=carton_no||'OLD' where carton_no='" + dtlic.Rows[i]["CARTON_NO"] + "' and link_time <>(select max(link_time) from sfism4.r_sec_lic_link_t where carton_no= '" + dtlic.Rows[i]["CARTON_NO"] + "')",sfcHttpClient);
                }
                dtbdsn =await z.bdsnrecordNIC("AUTO","",sfcHttpClient);
                int numyield = dtbdsn.Rows.Count;
                if (numyield == 0)
                {
                    GoToSendMail("NIC BDSN IS NULL!");
                    return true;
                }
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                for (int i = 0; i < numyield; i++)
                {
                    string str = "BDSN|B_FOXVN_BD|" + dtbdsn.Rows[i]["serial_number"].ToString().Trim() + "|" + dtbdsn.Rows[i]["SHIPPING_SN"].ToString().Trim() + "|" +
                       dtbdsn.Rows[i]["LotID"].ToString().Trim() + "|" + dtbdsn.Rows[i]["LICENSE_NO"].ToString() + "||" + dtbdsn.Rows[i]["BROADCOM_PN"].ToString() + "||VN||||||||||";
                    stw.WriteLine(str);
                }

                string strctl = "CTL|" + numyield.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();
                string ftpstr = await sftpupload33(path, path11);
                if (ftpstr != "true")
                {
                    exmessage = ftpstr;
                    stw.Close();
                    throw new Exception(exmessage + "\n上傳ftp失敗");
                }
                GoToSendMail("NIC BDSN Upload FTP Successfully!");
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }

        }
        public async Task<bool> bdsnfileECD(SfcHttpClient sfcHttpClient)
        {
            try
            {
                DataTable dtbdsn = new DataTable();
                DataTable dtlic = new DataTable();

                string path = Directory.GetCurrentDirectory() + "\\Files\\BDSN";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string titleTime = await gettitleDate(sfcHttpClient) + "140100";
                path += "\\XXAT_BDSN_B_FOXVN_BD_" + titleTime + ".dat";
                string path11 = "XXAT_BDSN_B_FOXVN_BD_" + titleTime + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();

                dtbdsn =await z.bdsnrecordECD("AUTO","",sfcHttpClient);
                int numyield = dtbdsn.Rows.Count;

                if (numyield == 0)
                {
                    GoToSendMail("ECD BDSN IS NULL!");
                    return true;
                }

                for (int i = 0; i < numyield; i++)
                {
                    string str = "BDSN|B_FOXVN_BD|" + dtbdsn.Rows[i]["serial_number"].ToString().Trim() + "|" + dtbdsn.Rows[i]["shipping_sn"].ToString().Trim() + "|" +
                       dtbdsn.Rows[i]["LotID"].ToString().Trim() + "|" + dtbdsn.Rows[i]["TRAY_NO"].ToString() + "||" + dtbdsn.Rows[i]["BROADCOM_PN"].ToString() + "||VN||||||||||";
                    stw.WriteLine(str);

                }
                int total = numyield;
                string strctl = "CTL|" + total.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();

                string ftpstr = await sftpupload33(path, path11);
                if (ftpstr != "true")
                {
                    exmessage = ftpstr;
                    stw.Close();
                    throw new Exception(exmessage + "\n上傳ftp失敗");
                }
                GoToSendMail("ECD BDSN Upload FTP Successfully!");
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        //public async Task<bool> sndmfile(SfcHttpClient sfcHttpClient)
        //{
        //    try
        //    {
        //        DataTable dtlic = new DataTable();
        //        string path = Directory.GetCurrentDirectory() + "\\Files\\SNDM";
        //        if (!Directory.Exists(path))
        //        { Directory.CreateDirectory(path); }
        //        string creatime =await getcontentTime(sfcHttpClient);
        //        path += "\\XXAT_SNDM_B_FOXVN_BD_" + creatime + ".dat";
        //        string path11 = "XXAT_SNDM_B_FOXVN_BD_" + creatime + ".dat";
        //        FileInfo fi = new FileInfo(path);
        //        StreamWriter stw = fi.CreateText();
        //        dtlic = await fDal.ExcuteSelectSQL("select count(*),carton_no from sfism4.r_sec_lic_link_t group by carton_no having count(*)>1", sfcHttpClient);
        //        for (int i = 0; i < dtlic.Rows.Count; i++)
        //        {
        //           await fDal.ExcuteNonQuerySQL("update  sfism4.r_sec_lic_link_t set  carton_no=carton_no||'OLD' where carton_no='" + dtlic.Rows[i]["CARTON_NO"] + "' and link_time <>(select max(link_time) from sfism4.r_sec_lic_link_t where carton_no= '" + dtlic.Rows[i]["CARTON_NO"] + "')", sfcHttpClient);
        //        }
        //        await fDal.ExcuteNonQuerySQL("alter session set nls_language='AMERICAN'",sfcHttpClient);
        //        dt = await fDal.ExcuteSelectSQL("select * from sfis1.sn_dm", sfcHttpClient);
        //        if (dt.Rows.Count > 0)
        //        {

        //            for (int i = 0; i <= dt.Rows.Count - 1; i++)
        //            {
        //                string str = dt.Rows[i][0] + "|" + dt.Rows[i][1] + "|" + dt.Rows[i][2] + "|" + dt.Rows[i][3] + "|" + dt.Rows[i][4] + "|" + dt.Rows[i][5] + "|" + dt.Rows[i][6] + "|" + dt.Rows[i][7] + "|" + dt.Rows[i][8] + "|" + dt.Rows[i][9] + "|" + dt.Rows[i][10] + "|" + dt.Rows[i][11] + "|" + dt.Rows[i][12] + "|" + dt.Rows[i][13] + "|" + dt.Rows[i][14] + "|" + dt.Rows[i][15] + "|" + dt.Rows[i][16] + "|" + dt.Rows[i][17] + "|" + dt.Rows[i][18] + "|" + dt.Rows[i][19] + "|" + dt.Rows[i][19] + "|";
        //                stw.WriteLine(str);
        //            }
        //        }
        //        stw.WriteLine("CTL|" + dt.Rows.Count.ToString());
        //        stw.Flush();
        //        stw.Close();
        //        string ftpstr = ftpupload(path, path11);
        //        if (ftpstr != "true")
        //        {
        //            exmessage = ftpstr;
        //            stw.Close();
        //            throw new Exception(exmessage + "\n上傳ftp失敗");
        //        }
        //        //sendmail("NIC SNDM Upload FTP Successfully!");
        //        GoToSendMail("NIC SNDM Upload FTP Successfully!");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        exmessage = ex.Message;
        //        return false;
        //    }

        //}
        public async Task<bool> bdsnfileSuperCap(SfcHttpClient sfcClient)
        {
            try
            {
                DataTable dtbdsn = new DataTable();
                DataTable dtlic = new DataTable();

                string path = Directory.GetCurrentDirectory() + "\\Files\\BDSN";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string titleTime = await gettitleDate(sfcClient) + "140200";
                string contentTime = await getcontentTime(sfcClient);
                path += "\\XXAT_BDSN_B_FOXVN_BD_" + titleTime + ".dat";
                string path11 = "XXAT_BDSN_B_FOXVN_BD_" + titleTime + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();

                dtbdsn = await z.bdsnrecordSuperCap("AUTO","",sfcClient);
                int numyield = dtbdsn.Rows.Count;
                if (numyield == 0)
                {
                    GoToSendMail("SUPERCAP BDSN IS NULL!");
                    return true;
                }
                for (int i = 0; i < numyield; i++)
                {
                    string str = "BDSN|B_FOXVN_BD|" + dtbdsn.Rows[i]["serial_number"].ToString().Trim() + "||" + dtbdsn.Rows[i]["LotID"].ToString().Trim() + "|" +
                       dtbdsn.Rows[i]["MCARTON_NO"].ToString().Trim() + "|" + contentTime + "|" + dtbdsn.Rows[i]["BROADCOM_PN"].ToString() + "||VN||";
                    stw.WriteLine(str);

                }
                int total = numyield;
                string strctl = "CTL|" + total.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();

                string ftpstr = await sftpupload33(path, path11);
                if (ftpstr != "true")
                {
                    exmessage = ftpstr;
                    stw.Close();
                    throw new Exception(exmessage + "\n上傳ftp失敗");
                }
                GoToSendMail("SUPERCAP BDSN Upload SFTP Successfully!");
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }

        private async Task<string> getcontentTime(SfcHttpClient sfcHttpClient)
        {

            string str = "select to_char(sysdate,'YYYYMMDDHH24MISS') from dual";
            dt =await fDal.ExcuteSelectSQL(str, sfcHttpClient);
            return dt.Rows[0][0].ToString();
        }
        private async Task<string> gettitleDate(SfcHttpClient sfcHttpClient)
        {
            DataTable dt = new DataTable();
            string str = "select to_char(sysdate,'YYYYMMDD') from dual";
            dt =  await fDal.ExcuteSelectSQL(str, sfcHttpClient);
            return dt.Rows[0][0].ToString();
        }
        //private async Task<string> createdaytime(SfcHttpClient sfcHttpClient)
        //{
        //    DataTable dt = new DataTable();
        //    string str = "select to_char(sysdate,'YYYYMMDD') from dual";
        //    dt =  await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        //    return dt.Rows[0][0].ToString();
        //}

        //private async Task<DataTable> shipmoECD(string trayno, SfcHttpClient sfcHttpClient)
        //{
        //    string str = "select mo_number,version_code from sfism4.z107 where tray_no='" + trayno + "' group by mo_number,version_code";
        //    return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        //}
        /// <summary>
        /// 根據ship_no 穫取每個工單數量
        /// </summary>
        /// <param name="tcom">ship_no</param>
        /// <returns></returns>
        private async Task<DataTable> shipmo_tcom(string tcom, SfcHttpClient sfcHttpClient)
        {
            string str = "select count(mo_number) monum,mo_number from sfism4.z107 where ship_no='" + tcom + "' group by mo_number";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }

        /*modify by finger, for同一機種有不同的IC,但只能實現單顆IC數量為1 */

        //private async Task<DataTable> icqty(string model, SfcHttpClient sfcHttpClient)
        //{
        //    string str = "SELECT count(1) totalNum FROM sfis1.c_brcm_ic_t  WHERE MODEL_NAME ='" + model + "' ";
        //    return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        //}


        //private async Task<DataTable> lotdt(string kp, SfcHttpClient sfcHttpClient)
        //{
        //    string str = "SELECT * FROM (SELECT a.*, ROWNUM FROM (SELECT   lot_code \"Lot\", SUM (qty) \"lotsum\", MIN (start_time) \"Cut-Off_Date\" " +
        //           " FROM mes4.r_tr_sn@nsd04.mes  WHERE cust_kp_no = '" + kp + "'  AND doc_flag IN ('0', '1', '2') GROUP BY lot_code " +
        //         " ORDER BY MIN (start_time) DESC) a) WHERE ROWNUM = 1";
        //    return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        //}
        //private async Task<string> item(SfcHttpClient sfcHttpClient)
        //{
        //    DataTable dt = new DataTable();
        //    string str = "select  distinct(COMPONENT_ITEM) \"Item\"  from sfis1.C_BRCM_IC_T  ";
        //   dt =  await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        //    string icstr = string.Empty;
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        icstr += "'" + dt.Rows[i]["Item"].ToString() + "',";
        //    }
        //    return icstr.Substring(0, icstr.Length - 1);
        //}

        private async Task<DataTable> icsum(SfcHttpClient sfcHttpClient)
        {
            // string str = "select TO_CHAR(SYSDATE, 'YYYYMMDDHH24MISS') \"Record Creation Date\",a.component_item \"Item\",c.DEPARTMENT_CODE ,decode(sum(ext_qty),null,0,sum(ext_qty)) \"icsum\" from sfis1.C_BRCM_IC_T a,(select * from  mes4.r_tr_sn@NSD04.MES where location_flag = '0')b ,SFIS1.C_BRCM_PN_T  c where a.component_item =b.cust_kp_no(+) and a.model_name=c.model_name group by a.component_item,c.DEPARTMENT_CODE";
            // string str = "select TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS') \"Record Creation Date\",c.COMPONENT_ITEM \"Item\",c.qty-d.qty \"icsum\",department_code from (SELECT  component_item, decode(sum(total_qty),null,0,sum(shipping_qty)) qty ,department_code" +
            //            " FROM (select * from sfism4.r_shipping_ic_t where standby='STOCK_IN' ) a, (SELECT DISTINCT component_item,department_code FROM sfis1.c_brcm_ic_t e,sfis1.c_brcm_pn_t f where e.model_name=f.model_name) b " +
            //            "  WHERE    b.component_item = a.tr_sn(+) GROUP BY b.component_item,department_code ) c,(SELECT  component_item, decode(sum(total_qty),null,0,sum(total_qty)) qty " +
            //            " FROM (select * from sfism4.r_shipping_ic_t where standby='STOCK_OUT' ) a, (SELECT DISTINCT component_item FROM sfis1.c_brcm_ic_t) b " +
            //             " WHERE    b.component_item = a.tr_sn(+) GROUP BY b.component_item) d where c.COMPONENT_ITEM=d.COMPONENT_ITEM";
            string str = " SELECT   TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS') \"Record Creation Date\",component_item \"Item\", DECODE (SUM (shipping_qty), NULL, 0, SUM (shipping_qty)) \"icsum\", " +
                  "CHIP_DEPARTMENT_CODE   FROM (SELECT * FROM sfism4.r_shipping_ic_vn_t  WHERE STANDBY = 'STOCK_IN') a, (SELECT DISTINCT component_item, CHIP_DEPARTMENT_CODE " +
                    "  FROM sfis1.c_brcm_ic_t e where substr(MODEL_NAME,1,1) in('F','V') union SELECT '58804HB0KFSB30G','NICCC' FROM dual union SELECT '58802HB0KFSB30G','NICCC' FROM dual union SELECT '58802HA1FSB30G','NICCC' FROM dual ) b  WHERE b.component_item = a.mo_number(+) GROUP BY b.component_item, CHIP_DEPARTMENT_CODE";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }

        private async Task<DataTable> crecsum(string custkpno, SfcHttpClient sfcHttpClient)
        {
            if (custkpno.Length > 15)//客戶IC長度大於15位,SAP可能刪掉了BCM或者用I取代BCM了,少了BCM客戶接受不了文件
            {
                if (custkpno.Substring(0, 3) == "BCM")
                {
                    custkpno = custkpno.Substring(3);
                }
            }
            string str = "SELECT lot_code \"Lot\",qty \"lotsum\",to_char(start_time,'YYYYMMDDHH24MISS') \"Cut-Off Date\" FROM mes4.r_tr_sn@vnap" +
                         " WHERE start_time BETWEEN TO_DATE (to_char(sysdate-1,'YYYY/MM/DD')||'11:00:00', 'YYYY/MM/DD HH24:MI:SS' ) AND TO_DATE (to_char(sysdate,'YYYY/MM/DD')||'11:00:00', 'YYYY/MM/DD HH24:MI:SS')" +
                          " AND cust_kp_no in( '" + custkpno + "' ) and doc_flag in('0','1','2') ";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> zgetTestData(SfcHttpClient sfcHttpClient)
        {
            string str = "select SFIS1.Z_PKG.get_sql('GET_MDSYLD') FROM dual";
            DataTable tempDT = await fDal.ExcuteSelectSQL(str, sfcHttpClient);
            str = tempDT.Rows[0][0].ToString();
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }

        public async Task<string> getCmSite(SfcHttpClient sfcHttpClient)
        {
            string str = "select SFIS1.Z_PKG.get_data('MDSYLD','GET_CM_SITE') FROM dual";
            DataTable tempDT = await fDal.ExcuteSelectSQL(str, sfcHttpClient);
            str = tempDT.Rows[0][0].ToString();
            if(tempDT.Rows.Count ==0)
            {
                return "B_FOXVN_BD";
            }
            else
            {
                return tempDT.Rows[0][0].ToString();
            }
        }

        public async Task<DataTable> gettime(SfcHttpClient sfcHttpClient)
        {
            string str1 = "alter session set nls_language='AMERICAN'";
            await fDal.ExcuteNonQuerySQL(str1, sfcHttpClient);
            string str = " select sysdate,TO_CHAR(sysdate,'YYYYMMDD') TIME ,TO_CHAR(sysdate,'HH24') daily,TO_CHAR(sysdate,'DAY') weekly from dual";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> zgettime(SfcHttpClient sfcHttpClient)
        {
            string str = " select to_char(sysdate,'YYYY/MM/DD HH24:MI') as fromTime from dual";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> zgetalarm(SfcHttpClient sfcHttpClient,string fromTime)
        {
            string str = " select case when (sysdate-to_date('"+ fromTime + "','YYYY/MM/DD HH24:MI'))*24*60>15 then '1' else '0' end alarmFlag from dual";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getsndmflag(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM-VN' and vr_class='SNDM'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getinvlflag(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM-VN' and vr_class='INVL'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }

        public async Task<DataTable> getbdsnflag(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM-VN' and vr_class='BDSN'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getbdsnflagECD(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='ECD-VN' and vr_class='BDSN'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getbdsnflagSUPERCAP(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='SUPERCAP-VN' and vr_class='BDSN'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getsendflag(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM-VN' and vr_class='SEND'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        //private async Task<DataTable> shipBRCMVersonSupercap(string carton, SfcHttpClient sfcClient)
        //{
        //    string str = "select distinct lPAD(BRCM_VER,3,'0') ver from sfism4.z107 a, SFIS1.C_MODEL_BRCM_VER_T b where a.mo_number =b.mo_number and a.MCARTON_NO='" + carton + "'";
        //    return await fDal.ExcuteSelectSQL(str, sfcClient);
        //}
        public async Task<DataTable> getonhbflag(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM-VN' and vr_class='ONHB'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }

        public async Task<DataTable> spflag(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM-VN' and vr_class='SP'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getshipflag(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM-VN' and vr_class='SHIP'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable>  getshipflagECD(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='ECD-VN' and vr_class='SHIP'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getwipcflag(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM-VN' and vr_class='WIPC'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getwipcflagECD(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='ECD-VN' and vr_class='WIPC'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getyieldflag(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM-VN' and vr_class='YIELD'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        //supercap
        public async Task<DataTable> getshipflagSUPERCAP(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='SUPERCAP-VN' and vr_class='SHIP'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getwipcflagSUPERCAP(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='SUPERCAP-VN' and vr_class='WIPC'";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getallflag(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM-VN' and vr_class IN('WIPC','SHIP','ONHB','BDSN','INVL')";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> getallflag2(SfcHttpClient sfcHttpClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM-VN' and vr_class IN('WIPC','SHIP','ONHB','BDSN','INVL','YIELD')";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
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

        private async Task<string> sftpupload33(string path, string path11 )
        {
            bool isOK = false;
            try
            {
                //Neu da upload isOK=false, return "File da upload"
                dt = await Isuploaded(path11, Form1.sfcClient);

                if(dt.Rows.Count > 0)
                {
                    return "File da upload: "+ path11 + "";
                }
                else
                {
                    await Insertlogupload(path11, Form1.sfcClient);
                }

                sFtpWeb sftpweb = new sFtpWeb("ftpprod.broadcom.com", null, "scfoxcn", "vgy76tfc",22);
                sftpweb.uploadsftp(path);
                
                //if (sftpweb.FileExist(path11))
                //{
                //    // ftpweb.ReName(path,path.Split(".")+".tmp");
                //    sftpweb.ReName(path11, path11.Substring(0, path11.Length - 4) + ".dat");
                //}

            }
            catch (Exception exx)
            {
                GoToSendMail("SFTP Exception: " + exx.ToString());
                return exx.Message;
            }
            return "true";
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
        //public async Task<SfcHttpClient> connect()
        //{
        //    sfcClient = new SfcHttpClient(loginApiUri, loginDB, "helloApp", "123456");
        //    await sfcClient.GetAccessTokenAsync(empNo, empPass);
        //    return sfcClient;
        //}
        public async Task<bool> ExcuteNonQuerySQL(string sql, SfcHttpClient sfcHttpClient)
        {
            try
            {
                await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
