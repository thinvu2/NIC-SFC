using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using myFtpApp;
using System.Data;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using Sfc.Library.HttpClient;
using System.Threading.Tasks;
using Sfc.Core.Parameters;

namespace BRCM_B2B
{
    class manual
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
        DAL fDal = new DAL();
        zFunction z= new zFunction();
        public async Task<bool> ShipfileNIC(string DN, SfcHttpClient sfcClient)
        {
            try
            {
                DN_MO_NUM = new Dictionary<string, int>();
                DataTable dthead = new DataTable();
                DataTable dtcarton = new DataTable();
                DataTable dtmo = new DataTable();
                DataTable dtDN = new DataTable();
                DataTable dtlic = new DataTable();
                string path = Directory.GetCurrentDirectory() + "\\Files\\SHIP";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path); 
                //string creatime = DateTime.Now.ToString("yyyyMMddHHmmss");
                path += "\\XXAT_SHIP_B_FOXVN_BD_" + Form1.sentimenic + ".dat";
                string path11 = "XXAT_SHIP_B_FOXVN_BD_" + Form1.sentimenic + ".dat";
                string filename = "XXAT_SHIP_B_FOXVN_BD_" + Form1.sentimenic + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                dtlic = await fDal.ExcuteSelectSQL("select count(*),carton_no from sfism4.r_sec_lic_link_t group by carton_no having count(*)>1", sfcClient);
                for (int i = 0; i < dtlic.Rows.Count; i++)
                {
                    await fDal.ExcuteNonQuerySQL("update  sfism4.r_sec_lic_link_t set  carton_no=carton_no||'OLD' where carton_no='" + dtlic.Rows[i]["CARTON_NO"] + "' and link_time <>(select max(link_time) from sfism4.r_sec_lic_link_t where carton_no= '" + dtlic.Rows[i]["CARTON_NO"] + "')", sfcClient);
                }
                dthead =await z.NICdtHeaderShip("Manual", DN, sfcClient);
                int headnum = dthead.Rows.Count;

                int lotnum = 0;
                for (int i = 0; i < headnum; i++)
                {
                    string strhead = await z.NICGetStringHeader(Form1.sentimenic,dthead, i);
                    stw.WriteLine(strhead);
                    dtcarton =await z.NICShipCartonDT(dthead.Rows[i]["TCOM"].ToString(), sfcClient);
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
                            else  {
                                DN_MO_NUM.Add((dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID), Convert.ToInt32(dtcarton.Rows[j]["NUM"].ToString()));
                            }
                            string strlot = await z.NICGetStringContent(Form1.sentimenic,LotID, version, dthead, i, dtcarton, j);
                            stw.WriteLine(strlot);
                        }
                }
                int total = headnum + lotnum;
                string strctl = "CTL|" + total.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();
                for (int i = 0; i < headnum; i++)
                {
                    DataTable dttime = new DataTable();
                    dttime = await fDal.ExcuteSelectSQL("select to_char(sysdate,'YYYYMMDD') as datetime,to_char(sysdate+1,'YYYYMMDD') as datetime1  from dual", sfcClient );
                    if (dttime.Rows[0][0].ToString() == DateTime.Now.ToString("yyyyMMdd")|| dttime.Rows[0][1].ToString() == DateTime.Now.ToString("yyyyMMdd"))
                    {
                        string type = DateTime.Now.ToString("yyyyMMddHHmmss");
                        if (DN != "")
                        {
                            type = DN;
                        }
                        string sql = string.Format(@"insert into  SFISM4.R_EDI_HAWB2BOX_T
                                                     (BOX,PN, INVOICE, HAWB, EXTRA, YYYYMMDD,DN_LINE)
                                                     valueS('N','{0}', '{1}', 'SHIPFILE', '{2}', '{3}','{4}')", getMac(), dthead.Rows[i]["Shipment Number"].ToString().Trim(), filename, DateTime.Now.ToString("yyyyMMdd"), type);
                        await fDal.ExcuteNonQuerySQL(sql, sfcClient);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //  Form1.mo_number_count.Clear();
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task<bool> ShipfileECD(string DN, SfcHttpClient sfcClient)
        {
            try
            {
                DN_MO_NUMECD = new Dictionary<string, int>();

                DataTable dthead = new DataTable();
                DataTable dtcarton = new DataTable();
                DataTable dtDN = new DataTable();
                DataTable dtlic = new DataTable();

                string path = Directory.GetCurrentDirectory() + "\\Files\\SHIP";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                path += "\\XXAT_SHIP_B_FOXVN_BD_" + Form1.sentimeecd + ".dat";
                string path11 = "XXAT_SHIP_B_FOXVN_BD_" + Form1.sentimeecd + ".dat";
                string filename = "XXAT_SHIP_B_FOXVN_BD_" + Form1.sentimeecd + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                dthead =await z.ECDdtHeaderShip("Manual", DN, sfcClient);
                int headnum = dthead.Rows.Count;
                int lotnum = 0;
                for (int i = 0; i < headnum; i++)
                {
                    string strhead = await z.ECDGetStringHeader(Form1.sentimeecd,dthead, i);
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
                        string strlot = await z.ECDGetStringContent(Form1.sentimeecd,LotID, version, dthead, i, dtcarton, j);
                        stw.WriteLine(strlot);
                    }
                }

                //end ECD

                int total = headnum + lotnum;
                string strctl = "CTL|" + total.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();
                for (int i = 0; i < headnum; i++)
                {
                    DataTable dttime = new DataTable();
                    dttime =  await fDal.ExcuteSelectSQL("select to_char(sysdate,'YYYYMMDD') as datetime,to_char(sysdate+1,'YYYYMMDD') as datetime1  from dual",sfcClient);
                    if (dttime.Rows[0][0].ToString() == DateTime.Now.ToString("yyyyMMdd")|| dttime.Rows[0][1].ToString() == DateTime.Now.ToString("yyyyMMdd"))
                    {

                        string type = DateTime.Now.ToString("yyyyMMddHHmmss");
                        if (DN != "")
                        {
                            type = DN;
                        }
                        string sql = string.Format(@"insert into  SFISM4.R_EDI_HAWB2BOX_T
                                                     (BOX,PN, INVOICE, HAWB, EXTRA, YYYYMMDD,DN_LINE)
                                                     valueS('N','{0}', '{1}', 'SHIPFILEECD', '{2}', '{3}','{4}')", getMac(), dthead.Rows[i]["Shipment Number"].ToString().Trim(), filename, DateTime.Now.ToString("yyyyMMdd"), type);

                        await fDal.ExcuteNonQuerySQL(sql, sfcClient);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task<bool> wipcfileNIC(string DN, SfcHttpClient sfcClient)
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
                //string creatime = DateTime.Now.ToString("yyyyMMddHHmmss");
                path += "\\XXAT_WIPC_B_FOXVN_BD_" + Form1.sentimenic + ".dat";
                string path11 = "XXAT_WIPC_B_FOXVN_BD_" + Form1.sentimenic + ".dat";
                string filename = "XXAT_WIPC_B_FOXVN_BD_" + Form1.sentimenic + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                int num = 0;

                dtlic = await fDal.ExcuteSelectSQL("select a.model_name from  sfism4.r_bpcs_invoice_t a, sfis1.c_brcm_pn_t b  WHERE a.model_name = b.model_name  and a.invoice IN ( SELECT dn_no FROM sfism4.r_sap_dn_detail_t WHERE UPPER (ship_address) IN " +
                   " ('AVAGO TECHNOLOGIES INTERNATIONAL SALES@C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN','C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN' )) AND TO_CHAR (a.finish_date, 'YYYYMMDD') = '" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "' Minus Select model_name from sfis1.c_brcm_ic_t", sfcClient);
                if (dtlic.Rows.Count > 0)
                {
                    exmessage = "有机种没有配置sfis1.c_brcm_ic_t表";
                    return false;
                }
                dtic =await z.NICWipcdt("Manual",DN, sfcClient);
                for (int i = 0; i < dtic.Rows.Count; i++)
                {
                    string icqty = " SELECT *  FROM sfis1.c_brcm_ic_t  WHERE MODEL_NAME ='" + dtic.Rows[i]["model"].ToString() + "' ";
                    dticqty = await fDal.ExcuteSelectSQL(icqty, sfcClient);
                    int totalicnum = dticqty.Rows.Count;
                    dtmo =await z.NICWipCmoDT(dtic.Rows[i]["Shipment Number"].ToString(), sfcClient);                
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
                            throw new Exception(exmessage + "\n" + dtic.Rows[i]["Component Item"].ToString() + dataSP.Rows[0]["res"].ToString());
                        }
                        string strwipc = string.Empty;
                        string col22 = string.Empty;
                        dtminus = await fDal.ExcuteSelectSQL("select mo_number,sum(qty) QTY from  sfism4.r_ic_minus_vn_t where mo_number='" + mo + "' and ship_no='" + shipno + "' and minus_time='" + DateTime.Now.ToString("yyyyMMdd") + "' and brcm_ic='" + dtic.Rows[i]["Component Item"].ToString() + "' group by mo_number", sfcClient);
                            //2021/3/25 客戶say LOT_CODE改為統一格式,為V替換BCM後的IC名稱 ex:BCM5719A1KFBG --> V5719A1KFBG 而客戶要求，file的line 要按照mo_number合併,一個mo_number隻能在一個invoice下出現一次

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

                                dtminus =  await fDal.ExcuteSelectSQL("select sum(qty) from  sfism4.r_ic_minus_vn_t where mo_number='" + mo + "' and ship_no='" + shipno + "' and minus_time='" + DateTime.Now.ToString("yyyyMMdd") + "'and brcm_ic='" + dtic.Rows[i]["Component Item"].ToString() + "' ", sfcClient);

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
                stw.WriteLine("CTL|" + num);
                stw.Flush();
                stw.Close();
                for (int i = 0; i < dtic.Rows.Count; i++)
                {
                    DataTable dttime = new DataTable();
                    dttime = await fDal.ExcuteSelectSQL("select to_char(sysdate,'YYYYMMDD') as datetime,to_char(sysdate+1,'YYYYMMDD') as datetime1  from dual", sfcClient );
                    if (dttime.Rows[0][0].ToString() == DateTime.Now.ToString("yyyyMMdd")|| dttime.Rows[0][1].ToString() == DateTime.Now.ToString("yyyyMMdd"))
                    {
                        string type = DateTime.Now.ToString("yyyyMMddHHmmss");
                        if (DN != "")
                        {
                            type = DN;
                        }
                        string sql = string.Format(@"insert into  SFISM4.R_EDI_HAWB2BOX_T (BOX,PN, INVOICE, HAWB, EXTRA, YYYYMMDD,DN_LINE)
                                                     valueS('N','{0}', '{1}', 'WIPCFILE', '{2}', '{3}','{4}')", getMac(), dtic.Rows[i]["Shipment Number"].ToString().Trim(), filename, DateTime.Now.ToString("yyyyMMdd"), type);
                      await  fDal.ExcuteNonQuerySQL(sql, sfcClient);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task<bool> ShipfileSuperCap(string DN, SfcHttpClient sfcClient)
        {
            try
            {
                DN_MO_NUMSUPERCAP = new Dictionary<string, int>();

                DataTable dthead = new DataTable();
                DataTable dtcarton = new DataTable();
                DataTable dtDN = new DataTable();
                DataTable dtlic = new DataTable();

                string path = Directory.GetCurrentDirectory() + "\\Files\\SHIP";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                path += "\\XXAT_SHIP_B_FOXVN_BD_" + Form1.sentimesupercap + ".dat";
                string path11 = "XXAT_SHIP_B_FOXVN_BD_" + Form1.sentimesupercap + ".dat";
                string filename = "XXAT_SHIP_B_FOXVN_BD_" + Form1.sentimesupercap + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();

                dthead = await z.SupercapdtHeaderShip("Manual", DN, sfcClient);
                int headnum = dthead.Rows.Count;

                int lotnum = 0;
                for (int i = 0; i < headnum; i++)
                {
                    string strhead = await z.SuperCapGetStringHeader(Form1.sentimesupercap,dthead, i);
                    stw.WriteLine(strhead);
                    //******-----------------------------------------------------------------------------------------
                    DataTable dttime = new DataTable();
                    dttime = await fDal.ExcuteSelectSQL("select to_char(sysdate,'YYYYMMDD') as datetime,to_char(sysdate+1,'YYYYMMDD') as datetime1  from dual", sfcClient);
                    if (dttime.Rows[0][0].ToString() == DateTime.Now.ToString("yyyyMMdd") || dttime.Rows[0][1].ToString() == DateTime.Now.ToString("yyyyMMdd"))
                    {
                        string type = DateTime.Now.ToString("yyyyMMddHHmmss");
                        if (DN != "")
                        {
                            type = DN;
                        }
                        string sql = string.Format(@"insert into  SFISM4.R_EDI_HAWB2BOX_T
                                                     (BOX,PN, INVOICE, HAWB, EXTRA, YYYYMMDD,DN_LINE)
                                                     valueS('N','{0}', '{1}', 'ShipFileSP', '{2}', '{3}','{4}')", getMac(), dthead.Rows[i]["Shipment Number"].ToString().Trim(), filename, DateTime.Now.ToString("yyyyMMdd"), type);
                        await fDal.ExcuteNonQuerySQL(sql, sfcClient);
                    }//******-----------------------------------------------------------------------------------------
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
                        string LotID =await z.ShipGetLotID("SUPERCAP", null, dtcarton, j);
                        if (DN_MO_NUMSUPERCAP.ContainsKey(dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID))
                        {
                            DN_MO_NUMSUPERCAP[dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID] += Convert.ToInt32(dtcarton.Rows[j]["NUM"].ToString());
                        }
                        else{
                            DN_MO_NUMSUPERCAP.Add((dthead.Rows[i]["Shipment Number"].ToString().Trim() + LotID), Convert.ToInt32(dtcarton.Rows[j]["NUM"].ToString()));
                        }
                        string strlot = await z.SuperCapGetStringContent(Form1.sentimesupercap,LotID, version, dthead, i, dtcarton, j);
                        stw.WriteLine(strlot);
                    }
                }
                int total = headnum + lotnum;
                string strctl = "CTL|" + total.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                MessageBox.Show(exmessage);
                return false;
            }
        }
        public async Task<bool> wipcfileSuperCap(string DN, SfcHttpClient sfcClient)
        {
            try
            {
                //Hang SupperCap de file WIPC NULL neu co file SHip
                //DataTable dthead = new DataTable();
                //dthead = await z.SupercapdtHeaderShip("Manual",DN, sfcClient);
                //if (dthead.Rows.Count == 0) return true;
                string path = Directory.GetCurrentDirectory() + "\\Files\\WIPC";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                //string creatime = DateTime.Now.ToString("yyyyMMddHHmmss");
                path += "\\XXAT_WIPC_B_FOXVN_BD_" + Form1.sentimesupercap + ".dat";
                string filename = "XXAT_WIPC_B_FOXVN_BD_" + Form1.sentimesupercap + ".dat";
                string path11 = "XXAT_WIPC_B_FOXVN_BD_" + Form1.sentimesupercap + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                int num = 0;
                int total = num;
                stw.WriteLine("CTL|" + total.ToString());
                stw.Flush();
                stw.Close();
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task< bool> wipcfileECD(string DN, SfcHttpClient sfcClient)
        {
            try
            {
                DataTable dtic = new DataTable();
                DataTable dtlot = new DataTable();
                DataTable dtmo = new DataTable();
                DataTable dtminus = new DataTable();
                DataTable dtlic = new DataTable();
                DataTable dtDN = new DataTable(); 
                DataTable dticqty = new DataTable();
                string path = Directory.GetCurrentDirectory() + "\\Files\\WIPC";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                path += "\\XXAT_WIPC_B_FOXVN_BD_" + Form1.sentimeecd + ".dat";
                string filename = "XXAT_WIPC_B_FOXVN_BD_" + Form1.sentimeecd + ".dat";
                string path11 = "XXAT_WIPC_B_FOXVN_BD_" + Form1.sentimeecd + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                int num = 0;

                dtic =await z.ECDWipcdt("Manual",DN, sfcClient);
                for (int i = 0; i < dtic.Rows.Count; i++)
                {
                    dtmo =await z.ECDWipCmoDT(dtic.Rows[i]["Shipment Number"].ToString(), sfcClient);
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
                                if (DN_MO_NUMECD.ContainsKey(dtic.Rows[i]["Shipment Number"].ToString().Trim() + mo.Trim()))//為解決因為合箱產生的wipc的mo比ship多的問題
                                {
                                    string sql = $@"SELECT component_qty  FROM sfis1.c_brcm_ic_t where model_name='{dtic.Rows[i]["MODEL"].ToString()}' and COMPONENT_ITEM = '{strics}' ";
                                    dtlot = await fDal.ExcuteSelectSQL(sql, sfcClient);
                                    int use_level = string.IsNullOrEmpty(dtlot.Rows[0][0].ToString()) ? 1 : Convert.ToInt32(dtlot.Rows[0][0].ToString());
                                    strwipc = "WIPC|B_FOXVN_BD|" + Form1.sentimeecd + "||" + dtic.Rows[i]["Shipment Number"].ToString() + "|" +
                                        dtic.Rows[i]["Assembly Item"].ToString() + "|" + dtmo.Rows[j]["vender_item"].ToString() + "|EA|" +
                                        DN_MO_NUMECD[dtic.Rows[i]["Shipment Number"].ToString().Trim() + mo.Trim()] * use_level + "|" + dtic.Rows[i]["CHIPDEPART"].ToString() + "|||||||||||" + mo + "|" + "V" + dtmo.Rows[j]["vender_item"].ToString() + "|" + DN_MO_NUMECD[dtic.Rows[i]["Shipment Number"].ToString().Trim() + mo.Trim()] * use_level + "||||";
                                    stw.WriteLine(strwipc);
                                    num++;
                                }
                            }

                            dtminus = await fDal.ExcuteSelectSQL("select sum(qty) from  sfism4.r_ic_minus_vn_t where mo_number='" + mo + "' and ship_no='" + shipno + "' and minus_time='" + DateTime.Now.ToString("yyyyMMdd") + "'and brcm_ic='" + strics + "' ", sfcClient );

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
                for (int i = 0; i < dtic.Rows.Count; i++)
                {
                    DataTable dttime = new DataTable();
                    dttime = await fDal.ExcuteSelectSQL("select to_char(sysdate,'YYYYMMDD') as datetime,to_char(sysdate+1,'YYYYMMDD') as datetime1  from dual", sfcClient );
                    if (dttime.Rows[0][0].ToString() == DateTime.Now.ToString("yyyyMMdd")|| dttime.Rows[0][1].ToString() == DateTime.Now.ToString("yyyyMMdd"))
                    {

                        string type = DateTime.Now.ToString("yyyyMMddHHmmss");
                        if (DN != "")
                        {
                            type = DN;
                        }
                        string sql = string.Format(@"insert into  SFISM4.R_EDI_HAWB2BOX_T
                                                     (BOX,PN, INVOICE, HAWB, EXTRA, YYYYMMDD,DN_LINE)
                                                     valueS('N','{0}', '{1}', 'WIPCFILEECD', '{2}', '{3}','{4}')", getMac(), dtic.Rows[i]["Shipment Number"].ToString().Trim(), filename, DateTime.Now.ToString("yyyyMMdd"), type);

                        await fDal.ExcuteNonQuerySQL(sql, sfcClient);
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task<bool> weeklyonhbfile(SfcHttpClient sfcClient)
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
                string creatime = DateTime.Now.ToString("yyyyMMddHHmmss");
                path += "\\XXAT_ONHB_B_FOXVN_BD_" + creatime + ".dat";
                string path11 = "XXAT_ONHB_B_FOXVN_BD_" + creatime + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                /*  orac.ExcuteSelectSQL("select distinct COMPONENT_ITEM from sfis1.c_brcm_ic_t", ref dtcrec);
                  for (int n = 0; n < dtcrec.Rows.Count; n++)
                  {
                      string kp = dtcrec.Rows[0][0].ToString();
                      dtcrec = crecsum(kp);
                      for (int k = 0; k < dtcrec.Rows.Count; k++)
                      {
                          string strin1 = "SELECT * FROM sfism4.r_shipping_ic_t WHERE (mo_number, tr_sn, total_qty, shipping_qty, last_shipping_time,STANDBY,lot_code ) in " +
                                          "(select '" + kp + "','" + kp + "'," + Convert.ToInt32(dtcrec.Rows[k]["lotsum"]) + "," + Convert.ToInt32(dtcrec.Rows[k]["lotsum"]) +
                                             ",to_date(" + dtcrec.Rows[k]["Cut-Off Date"].ToString() + ",'yyyymmdd hh24:mi:ss'),'STOCK_IN','" + dtcrec.Rows[k]["Lot"].ToString() + "' from dual)";
                          orac.ExcuteSelectSQL(strin1, ref dticin);
                          if (dticin.Rows.Count == 0)
                          {
                              string strin2 = "insert into sfism4.r_shipping_ic_t values('" + kp + "','" + kp + "'," + Convert.ToInt32(dtcrec.Rows[k]["lotsum"]) + "," + Convert.ToInt32(dtcrec.Rows[k]["lotsum"]) +
                                             ",to_date(" + dtcrec.Rows[k]["Cut-Off Date"].ToString() + ",'yyyymmdd hh24:mi:ss'),'STOCK_IN','" + dtcrec.Rows[k]["Lot"].ToString() + "')";
                              orac.ExcuteNonQuerySQL(strin2);
                          }
                      }
                  }*/
                dticsum =await icsum(sfcClient);
                int icnum = dticsum.Rows.Count;
                int crecnum = 0;
                for (int i = 0; i < icnum; i++)
                {
                    string kp = dticsum.Rows[i]["Item"].ToString();
                    int ics = Convert.ToInt32(dticsum.Rows[i]["icsum"]);
                    if (ics < 0)
                    {
                        throw new Exception(kp + "的数量小于0");
                    }
                    else if (ics > 0)
                    {
                        dtcrec =await crecsum(kp, sfcClient);
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
                    /* string str = "ONHB|B_FOXVN_BD|" + creatime + "|" + kp + "|" + dticsum.Rows[i]["CHIP_DEPARTMENT_CODE"].ToString() + "|EA|" +
                         dticsum.Rows[i]["Record Creation Date"].ToString() + "|" + dticsum.Rows[i]["icsum"].ToString() + "|N||||||||||||||||";
                     stw.WriteLine(str);*/
                    /*  dtcrec = crecsum(kp);
                      crecnum += dtcrec.Rows.Count;
                      for (int j = 0; j < dtcrec.Rows.Count; j++)
                      {
                          string str1 = "ONHB|B_FOXVN_BD|" + creatime + "|" + kp + "|" + dticsum.Rows[i]["CHIP_DEPARTMENT_CODE"].ToString() + "|EA|" +
                              dtcrec.Rows[j]["Cut-Off Date"].ToString() + "|" + dtcrec.Rows[j]["lotsum"].ToString() + "|N|||CREC|" + dtcrec.Rows[j]["Lot"].ToString() + "||||||||||||";

                          stw.WriteLine(str1);

                      }*/
                }
                /*   dtforecast = allforecast();
                   for (int k = 0; k < dtforecast.Rows.Count; k++)
                   {
                       string str = "ONHB|B_FOXVN_BD|" + creatime+"|" + dtforecast.Rows[k]["BROADCOM_PN"].ToString() + "|" + dtforecast.Rows[k]["DEPARTMENT_CODE"].ToString() + "|EA|" +
                           dtforecast.Rows[k]["CUTOFF_DATA"].ToString() + "|" + dtforecast.Rows[k]["SHIPPING_QTY"].ToString() + "|N|||"
                           + dtforecast.Rows[k]["Transaction type"].ToString() + "|||Y||||||||||";
                       stw.WriteLine(str);
                   }*/
                int total =/* icnum +*/ crecnum /*+ dtforecast.Rows.Count*/;
                string strctl = "CTL|" + total.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();

                //string ftpstr = ftpupload(path, path11);
                //if (ftpstr != "true")
                //{
                //    exmessage = ftpstr;
                //    throw new Exception(exmessage+"\n上傳ftp失敗");
                //}

                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }

        }
        public async Task<bool> yieldfile(SfcHttpClient sfcClient)
        {
            try
            {
                await fDal.ExcuteSP("sfis1.brcm_reasoncode", null, sfcClient);

                DataTable dtyield = new DataTable();
                string path = Directory.GetCurrentDirectory() + "\\Files\\yield";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string creatime = DateTime.Now.ToString("yyyyMMdd");
                path += "\\XXAT_YIELD_B_FOXVN_BD_" + creatime + ".dat";
                string path11 = "XXAT_YIELD_B_FOXVN_BD_" + creatime + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                dtyield =await ftrecord(sfcClient);
                int numyield = dtyield.Rows.Count;
                for (int i = 0; i < numyield; i++)
                {
                    string str = creatime + "|" + dtyield.Rows[i]["RecordSequence"].ToString().Trim() + "|B_FOXVN_BD|" +
                        dtyield.Rows[i]["BROADCOM_PN"].ToString() + "|" + dtyield.Rows[i]["REV_CODE"].ToString() + "|" + dtyield.Rows[i]["SHIPPING_SN"].ToString() + "|" +
                        dtyield.Rows[i]["TESTDATE"].ToString() + "|" + dtyield.Rows[i]["TESTTIME"].ToString() + "|" + dtyield.Rows[i]["GROUP_NAME"].ToString() + "|" +
                        dtyield.Rows[i]["PASSED_QUANTITY"].ToString() + "|" + dtyield.Rows[i]["TEST_CODE"].ToString() + "||" + dtyield.Rows[i]["EMP_NO"].ToString();
                    stw.WriteLine(str);
                }



                // string strctl = "CTL|" + numyield.ToString();
                //stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();
                //string ftpstr = ftpupload(path, path11);
                //if (ftpstr != "true")
                //{
                //    exmessage = ftpstr;
                //    throw new Exception(exmessage+"\n上傳ftp失敗");
                //}

                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }

        }
        public async Task<bool> invlfile(SfcHttpClient sfcClient)
        {
            try
            {
                DataTable dtinvl = new DataTable();


                string path = Directory.GetCurrentDirectory() + "\\Files\\INVL";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string creatime = DateTime.Now.ToString("yyyyMMddHHmmss");
                path += "\\XXAT_INVL_B_FOXVN_BD_" + creatime + ".dat";
                string path11 = "XXAT_INVL_B_FOXVN_BD_" + creatime + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                dtinvl =await invlrecord(sfcClient);
                int numyield = dtinvl.Rows.Count;
                for (int i = 0; i < numyield; i++)
                {
                    string str = "INVL|B_FOXVN_BD|" + creatime
                        + "|C|" + dtinvl.Rows[i]["CUST_KP_NO"].ToString().Trim() + "|" + dtinvl.Rows[i]["DEPARTMENT_CODE"].ToString().Trim() + "|SCRP||SOURCE|" +
                        dtinvl.Rows[i]["LOT_CODE"].ToString() + "|" + dtinvl.Rows[i]["QTY"].ToString() + "|||||||||||||||||||||||||";
                    stw.WriteLine(str);
                    string s = " update sfis1.c_brcm_icscrp_t set sendflag='Y' where COMPONENT_ITEM='" + dtinvl.Rows[i]["CUST_KP_NO"].ToString() + "' and LOT_DATE ='" + dtinvl.Rows[i]["LOT_CODE"].ToString() + "' and sendflag='N'";
                    await fDal.ExcuteNonQuerySQL(s, sfcClient);
                }

                string strctl = "CTL|" + numyield.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();
                //string ftpstr = ftpupload(path, path11);
                //if (ftpstr != "true")
                //{
                //    exmessage = ftpstr;
                //    throw new Exception(exmessage+"\n上傳ftp失敗");
                //}

                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }

        }

        public async Task<bool> bdsnfile(string DN, SfcHttpClient sfcClient)
        {
            try
            {
                DataTable dtbdsn = new DataTable();
                DataTable dtlic = new DataTable();

                string path = Directory.GetCurrentDirectory() + "\\Files\\BDSN";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string creatime = DateTime.Now.ToString("yyyyMMddHHmmss");
                path += "\\XXAT_BDSN_B_FOXVN_BD_" + Form1.sentimenic + ".dat";
                string path11 = "XXAT_BDSN_B_FOXVN_BD_" + Form1.sentimenic + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();

                dtlic = await fDal.ExcuteSelectSQL("select count(*),carton_no from sfism4.r_sec_lic_link_t group by carton_no having count(*)>1", sfcClient);
                for (int i = 0; i < dtlic.Rows.Count; i++)
                {
                    await fDal.ExcuteNonQuerySQL("update  sfism4.r_sec_lic_link_t set  carton_no=carton_no||'OLD' where carton_no='" + dtlic.Rows[i]["CARTON_NO"] + "' and link_time <>(select max(link_time) from sfism4.r_sec_lic_link_t where carton_no= '" + dtlic.Rows[i]["CARTON_NO"] + "')", sfcClient);
                }

                dtbdsn =await z.bdsnrecordNIC("Manual", DN, sfcClient);
                int numyield = dtbdsn.Rows.Count;
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
                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }

        }
        public async Task<bool> bdsnfileECD(string DN,SfcHttpClient sfcClient)
        {
            try
            {
                DataTable dtbdsn = new DataTable();
                DataTable dtlic = new DataTable();

                string path = Directory.GetCurrentDirectory() + "\\Files\\BDSN";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string creatime = DateTime.Now.ToString("yyyyMMddHHmmss");
                path += "\\XXAT_BDSN_B_FOXVN_BD_" + Form1.sentimeecd + ".dat";
                string path11 = "XXAT_BDSN_B_FOXVN_BD_" + Form1.sentimeecd + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();

                dtbdsn =await z.bdsnrecordECD("Manual",DN,sfcClient);
                int numyield = dtbdsn.Rows.Count;
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

                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task<bool> bdsnfileSuperCap(string DN, SfcHttpClient sfcClient)
        {
            try
            {
                DataTable dtbdsn = new DataTable();
                DataTable dtlic = new DataTable();

                string path = Directory.GetCurrentDirectory() + "\\Files\\BDSN";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string creatime = DateTime.Now.ToString("yyyyMMddHHmmss");
                path += "\\XXAT_BDSN_B_FOXVN_BD_" + Form1.sentimesupercap + ".dat";
                string path11 = "XXAT_BDSN_B_FOXVN_BD_" + Form1.sentimesupercap + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();

                dtbdsn = await z.bdsnrecordSuperCap("Manual",DN, sfcClient);
                int numyield = dtbdsn.Rows.Count;
                for (int i = 0; i < numyield; i++)
                {
                    string str = "BDSN|B_FOXVN_BD|" + dtbdsn.Rows[i]["serial_number"].ToString().Trim() + "||" + dtbdsn.Rows[i]["LotID"].ToString().Trim() + "|" +
                       dtbdsn.Rows[i]["MCARTON_NO"].ToString().Trim() + "|" + Form1.sentimesupercap + "|" + dtbdsn.Rows[i]["BROADCOM_PN"].ToString() + "||VN||";
                    stw.WriteLine(str);
                }
                int total = numyield;
                string strctl = "CTL|" + total.ToString();
                stw.WriteLine(strctl);
                stw.Flush();
                stw.Close();

                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }
        }
        public async Task<bool> sndmfile(SfcHttpClient sfcClient)
        {

            try
            {
                DataTable dtlic = new DataTable();
                string path = Directory.GetCurrentDirectory() + "\\Files\\SNDM";
                if (!Directory.Exists(path))
                { Directory.CreateDirectory(path); }
                string creatime = DateTime.Now.ToString("yyyyMMddHHmmss");
                path += "\\XXAT_SNDM_B_FOXVN_BD_" + creatime + ".dat";
                string path11 = "XXAT_SNDM_B_FOXVN_BD_" + creatime + ".dat";
                FileInfo fi = new FileInfo(path);
                StreamWriter stw = fi.CreateText();
                dtlic = await fDal.ExcuteSelectSQL("select count(*),carton_no from sfism4.r_sec_lic_link_t group by carton_no having count(*)>1", sfcClient );
                for (int i = 0; i < dtlic.Rows.Count; i++)
                {
                    await fDal.ExcuteNonQuerySQL("update  sfism4.r_sec_lic_link_t set  carton_no=carton_no||'OLD' where carton_no='" + dtlic.Rows[i]["CARTON_NO"] + "' and link_time <>(select max(link_time) from sfism4.r_sec_lic_link_t where carton_no= '" + dtlic.Rows[i]["CARTON_NO"] + "')", sfcClient);
                }
                await fDal.ExcuteNonQuerySQL("alter session set nls_language='AMERICAN'", sfcClient);
                string sql = "SELECT ROWNUM rown_num, z.\"LPN Number\",  (CASE  WHEN SUBSTR (z.\"LOT Number\", 1, 4) ='2279'  THEN z.\"Customer serial no\" || 'RWK' " +
                            "  ELSE z.\"Customer serial no\"   END  ) \"Customer serial no\"," +
          "z.\"ManCtry\",(CASE  WHEN SUBSTR (z.\"LOT Number\", 1, 4) ='2279'  THEN z.\"Serial No.\" || 'RWK' " +
                            "  ELSE z.\"Serial No.\"   END  ) \"Serial No.\", z.\"MATERIAL\", z.\"MASTERWARR\"," +
          "z.\"WARRANTY\", z.\"WTYEND\", z.\"Sold-to pt\", z.\"Sold-to-name\"," +
          "z.\"Ship-to\", z.\"Ship-to party name\", z.\"WW customer name\"," +
          "z.\"Equipment descriptn\", z.\"System status\", z.\"DELIVERY\"," +
          "z.\"material status\", z.\"Strike Cnt\", z.\"LOT Number\" " +
          " FROM (SELECT  f.license_no \"LPN Number\",a.shipping_sn \"Customer serial no\", 'VN' \"ManCtry\"," +
          " shipping_sn \"Serial No.\",g.cpn material, NULL masterwarr,TO_CHAR (a.in_station_time, 'DD-MON-YYYY') warranty," +
          " NULL wtyend, NULL \"Sold-to pt\", NULL \"Sold-to-name\",NULL \"Ship-to\", NULL \"Ship-to party name\",NULL \"WW customer name\", eq_desc \"Equipment descriptn\",NULL \"System status\", NULL delivery," +
          " run_type \"material status\", NULL \"Strike Cnt\",a.mo_number \"LOT Number\"FROM sfism4.z107 a, sfism4.r_custsn_t e," +
          " sfism4.r_sec_lic_link_t f, sfis1.c_cust_model_match_t g WHERE a.ship_no IN ( SELECT tcom FROM sfism4.r_bpcs_invoice_t c, sfis1.c_cust_model_match_t d " +
          " WHERE TO_CHAR (finish_date, 'YYYYMMDD') >='" + DateTime.Now.AddDays(-7).ToString("yyyyMMdd") + "' AND TO_CHAR (finish_date, 'YYYYMMDD') < '" + DateTime.Now.ToString("yyyyMMdd") + "'" +
          " AND c.invoice IN ( SELECT dn_no FROM sfism4.r_sap_dn_detail_t WHERE UPPER (ship_address) IN " +
                   " ('AVAGO TECHNOLOGIES INTERNATIONAL SALES@C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN' ,'C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN')) AND d.f_md = c.model_name AND d.site = 'CQ' AND d.cust IS NOT NULL) " +
          " AND a.serial_number = e.serial_number AND LENGTH (a.serial_number) = 12 AND a.mcarton_no = f.carton_no and a.model_name=g.f_md AND g.site = 'CQ' " +
          " AND g.cust IS NOT NULL  ORDER BY a.in_station_time DESC) z ";
               dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        string str = dt.Rows[i][0] + "|" + dt.Rows[i][1] + "|" + dt.Rows[i][2] + "|" + dt.Rows[i][3] + "|" + dt.Rows[i][4] + "|" + dt.Rows[i][5] + "|" + dt.Rows[i][6] + "|" + dt.Rows[i][7] + "|" + dt.Rows[i][8] + "|" + dt.Rows[i][9] + "|" + dt.Rows[i][10] + "|" + dt.Rows[i][11] + "|" + dt.Rows[i][12] + "|" + dt.Rows[i][13] + "|" + dt.Rows[i][14] + "|" + dt.Rows[i][15] + "|" + dt.Rows[i][16] + "|" + dt.Rows[i][17] + "|" + dt.Rows[i][18] + "|" + dt.Rows[i][19] + "|" + dt.Rows[i][19] + "|";
                        stw.WriteLine(str);
                    }
                }
                stw.WriteLine("CTL|" + dt.Rows.Count.ToString());
                stw.Flush();
                stw.Close();
                //string ftpstr = ftpupload(path, path11);
                //if (ftpstr != "true")
                //{
                //    exmessage = ftpstr;
                //    throw new Exception(exmessage+"\n上傳ftp失敗");
                //}

                return true;
            }
            catch (Exception ex)
            {
                exmessage = ex.Message;
                return false;
            }

        }

        //private async Task<string> createtime(SfcHttpClient sfcClient)
        //{

        //    string str = "select to_char(sysdate,'YYYYMMDDHH24MISS') from dual";
        //    dt =  await fDal.ExcuteSelectSQL(str, sfcClient);
        //    return dt.Rows[0][0].ToString();
        //}

        
        
        
        
        //private async Task<DataTable> shipcartonECD(string tcom, SfcHttpClient sfcClient)
        //{
        //    string str = "select  a.* ,sum(num) over (order by tray_no rows between unbounded preceding and unbounded following ) lottotal from (SELECT  COUNT (tray_no) num,mo_number_old, tray_no," +
        //              " 'P' ful FROM sfism4.z107 c WHERE c.ship_no = '" + tcom + "' GROUP BY mo_number_old,tray_no ) a";
        //    return await fDal.ExcuteSelectSQL(str, sfcClient);
        //}
        
        //private async Task<DataTable> shipmoECD(string trayno, SfcHttpClient sfcClient)
        //{
        //    string str = "select MO_NUMBER_OLD,version_code from sfism4.z107 where tray_no='" + trayno + "' group by mo_number,version_code";
        //    return await fDal.ExcuteSelectSQL(str, sfcClient);

        //}
        //private async Task<DataTable> shipBRCMVersonSupercap(string carton, SfcHttpClient sfcClient)
        //{
        //    string str = "select distinct lPAD(BRCM_VER,3,'0') ver from sfism4.z107 a, SFIS1.C_MODEL_BRCM_VER_T b where a.mo_number =b.mo_number and a.MCARTON_NO='" + carton + "'";
        //    return await fDal.ExcuteSelectSQL(str, sfcClient);
        //}
        
        //private async Task<DataTable> modtECD(string invo,SfcHttpClient sfcClient)
        //{
        //    string str = " SELECT ship_no,b.mo_number_old,c.component_item,C.VENDER_ITEM, COUNT (*) * c.component_qty COUNT, C.COMPONENT_QTY  " +
        //                 "from sfism4.r_bpcs_invoice_t a,sfism4.z107 b ,sfis1.c_brcm_ic_t c " +
        //                 "where a.tcom = b.ship_no AND a.model_name = c.model_name AND invoice='" + invo + "' " +
        //                 "group BY ship_no,c.component_item, b.mo_number_old, c.component_qty,C.VENDER_ITEM order BY ship_no, b.mo_number_old, C.COMPONENT_QTY,C.VENDER_ITEM";
        //    return await fDal.ExcuteSelectSQL(str, sfcClient);

        //}
        /// <summary>
        /// 根據ship_no 穫取每個工單數量
        /// </summary>
        /// <param name="tcom">ship_no</param>
        /// <returns></returns>
        //private async Task<DataTable> shipmo_tcom(string tcom, SfcHttpClient sfcClient)
        //{
        //    string str = "select count(mo_number) monum,mo_number from sfism4.z107 where ship_no='" + tcom + "' group by mo_number";
        //    return await fDal.ExcuteSelectSQL(str, sfcClient);

        //}
        /*modify by finger, for同一機種有不同的IC,但只能實現單顆IC數量為1 */

        private async Task<DataTable> icqty(string model, SfcHttpClient sfcClient)
        {
            string str = "SELECT count(1) totalNum FROM sfis1.c_brcm_ic_t  WHERE MODEL_NAME ='" + model + "' ";
            return await fDal.ExcuteSelectSQL(str, sfcClient);

        }

        /*modify by finger,增加BRCM Version*/
        private async Task<DataTable> shipBRCMVerson(string carton, SfcHttpClient sfcClient)
        {
            string str = "select distinct substr(b.ssn3,-2) ver from sfism4.z107 a, sfism4.r_custsn_t b where a.serial_number =b.SERIAL_NUMBER and a.MCARTON_NO='" + carton + "'";
            return await fDal.ExcuteSelectSQL(str, sfcClient);

        }
        private async Task<DataTable> shipBRCMVersonECD(string tray_no, SfcHttpClient sfcClient)
        {
            string str = "select distinct BRCM_VER ver from sfism4.z107 a, SFIS1.C_MODEL_BRCM_VER_T b where a.mo_number =b.mo_number and a.TRAY_NO='" + tray_no + "' ";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }

        private async Task<string> item(SfcHttpClient sfcClient)
        {
            string str = "select  distinct(COMPONENT_ITEM) \"Item\"  from sfis1.C_BRCM_IC_T  ";
            dt =  await fDal.ExcuteSelectSQL(str, sfcClient);
            string icstr = string.Empty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                icstr += "'" + dt.Rows[i]["Item"].ToString() + "',";
            }
            return icstr.Substring(0, icstr.Length - 1);
        }

        private async Task<DataTable> icsum(SfcHttpClient sfcClient)
        {
            // string str = "select TO_CHAR(SYSDATE, 'YYYYMMDDHH24MISS') \"Record Creation Date\",a.component_item \"Item\",decode(sum(ext_qty),null,0,sum(ext_qty)) \"icsum\",c.DEPARTMENT_CODE from sfis1.C_BRCM_IC_T a,(select * from  mes4.r_tr_sn@NSD04.MES where location_flag = '0')b,SFIS1.C_BRCM_PN_T  c where a.component_item =b.cust_kp_no(+) and a.model_name=c.model_name group by a.component_item,c.DEPARTMENT_CODE";

            string str = " SELECT   TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS') \"Record Creation Date\",component_item \"Item\", DECODE (SUM (shipping_qty), NULL, 0, SUM (shipping_qty)) \"icsum\", " +
                  "CHIP_DEPARTMENT_CODE   FROM (SELECT * FROM sfism4.r_shipping_ic_vn_t  WHERE STANDBY = 'STOCK_IN') a, (SELECT DISTINCT component_item, CHIP_DEPARTMENT_CODE " +
                    "  FROM sfis1.c_brcm_ic_t e where substr(MODEL_NAME,1,1) in('F','V') union SELECT '58804HB0KFSB30G','NICCC' FROM dual union SELECT '58802HB0KFSB30G','NICCC' FROM dual union SELECT '58802HA1FSB30G','NICCC' FROM dual  ) b  WHERE b.component_item = a.mo_number(+) GROUP BY b.component_item, CHIP_DEPARTMENT_CODE";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }

        private async Task<DataTable> crecsum(string custkpno, SfcHttpClient sfcClient)
        {
            if (custkpno.Length > 15)//客戶IC長度大於15位,SAP可能刪掉了BCM或者用I取代BCM了,少了BCM客戶接受不了文件
            {
                if (custkpno.Substring(0, 3) == "BCM")
                {
                    custkpno = custkpno.Substring(3);
                }
            }
            string str = "SELECT lot_code \"Lot\",qty \"lotsum\",to_char(start_time,'yyyymmddhh24miss') \"Cut-Off Date\" FROM mes4.r_tr_sn@vnap" +
                         " WHERE start_time BETWEEN TO_DATE ('" + DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd") + "'||'11:00:00', 'YYYY/MM/DD HH24:MI:SS' ) AND TO_DATE ('" + DateTime.Now.ToString("yyyy/MM/dd") + "'||'11:00:00', 'YYYY/MM/DD HH24:MI:SS')" +
                          " AND cust_kp_no in ('" + custkpno + "') and doc_flag in('0','1','2') ";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }

        private async Task<DataTable> ftrecord(SfcHttpClient sfcClient)
        {

            string str = "select to_char(rownum,'000000') RecordSequence,a.* from (SELECT SERIAL_NUMBER,broadcom_pn, rev_code, shipping_sn,TO_CHAR (in_station_time, 'yyyymmdd') testdate,TO_CHAR (in_station_time, 'hh24miss') testtime," +
                "group_name,decode(PASSED_QUANTITY,'0','1','0') PASSED_QUANTITY, test_code, emp_no FROM sfism4.r_brcm_yield_t WHERE (insert_time < TO_DATE ('" + DateTime.Now.ToString("yyyy/MM/dd") + "'||' 00:00:00', 'YYYY/MM/DD HH24:MI:SS') and PASSED_QUANTITY='0' and flag='N' and rev_code is not null and shipping_sn <>'N/A' )  " +
                " or ( insert_time < TO_DATE ('" + DateTime.Now.ToString("yyyy/MM/dd") + "'||' 00:00:00', 'YYYY/MM/DD HH24:MI:SS' ) AND PASSED_QUANTITY='1' AND TEST_CODE IS NOT NULL AND FLAG ='N'  and rev_code is not null and shipping_sn <>'N/A') order by serial_number) a ";

            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }
        private async Task<DataTable> invlrecord(SfcHttpClient sfcClient)
        {
            string str = "SELECT  cust_kp_no, lot_code, DEPARTMENT_CODE,SUM (qty) qty FROM mes4.r_tr_sn@nsd04.mes a,sfis1.c_brcm_icscrp_t b,sfis1.c_brcm_pn_t c,sfis1.c_brcm_ic_t d WHERE doc_flag <> '3' AND a.cust_kp_no = b.COMPONENT_ITEM " +
                 " and a.LOT_CODE=B.LOT_DATE and B.sendflag='N' and a.cust_kp_no=d.COMPONENT_ITEM and d.model_name=c.MODEL_NAME GROUP BY cust_kp_no, lot_code ,DEPARTMENT_CODE ORDER BY 1, 2,3";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }

        private async Task<DataTable> allforecast(SfcHttpClient sfcClient)
        {
            string str = "select a.*, (case when RANK () OVER (PARTITION BY broadcom_pn ORDER BY cutoff_data )<5 then 'COMM' else 'CORR' end) \"Transaction type\"  from(select a.broadcom_pn,to_char(cutoff_data,'yyyymmddhh24miss') cutoff_data,shipping_qty,a.DEPARTMENT_CODE from sfis1.c_brcm_pn_t a,sfis1.C_BRCM_forecast_T b  where a.model_name=b.model_name and trans_time = (select max(trans_time) from sfis1.c_brcm_forecast_t) )a";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }
        //public async Task<DataTable> gettime(SfcHttpClient sfcClient)
        //{
        //    string str = " select sysdate,TO_CHAR(sysdate,'YYYYMMDD') TIME ,TO_CHAR(sysdate,'HH24') daily,TO_CHAR(sysdate,'DAY') weekly from dual";
        //    return await fDal.ExcuteSelectSQL(str, sfcClient);
        //}
        public async Task<DataTable> getsndmflag(SfcHttpClient sfcClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM' and vr_class='SNDM'";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }
        public async Task<DataTable> getonhbflag(SfcHttpClient sfcClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM' and vr_class='ONHB'";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }
        public async Task<DataTable> getshipflag(SfcHttpClient sfcClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM' and vr_class='SHIP'";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }
        public async Task<string> getwipcflag(SfcHttpClient sfcClient)
        {
            string str = " select vr_class,substr(vr_name,0,8) time ,VR_VALUE from sfis1.c_parameter_ini WHERE PRG_NAME='BRCM' and vr_class='WIPC'";
            dt = await fDal.ExcuteSelectSQL(str, sfcClient);
            return dt.Rows[0][0].ToString();
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
    }

}
