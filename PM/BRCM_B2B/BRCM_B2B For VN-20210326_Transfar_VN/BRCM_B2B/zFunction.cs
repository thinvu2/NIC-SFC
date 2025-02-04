using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;
using Newtonsoft.Json;

namespace BRCM_B2B
{
    class zFunction
    {
        DAL fDal = new DAL();
        #region get DataTable Header for Ship File
        public async Task<DataTable> NICdtHeaderShip(string type, string DN, SfcHttpClient sfcClient)
        {
            string strDN = "";
            string zstr = "";
            if (type == "AUTO") zstr = " and a.invoice not in(select  INVOICE from  SFISM4.R_EDI_HAWB2BOX_T  where BOX ='Y' and invoice is not null) ";
            if (DN != "")
                strDN = string.Format(@" and a.INVOICE = '{0}' ", DN);
            else strDN = "and TO_CHAR (a.finish_date, 'YYYYMMDD') = '" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "'";
            string str = "SELECT   TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS') \"Record Creation Date\",b.BROADCOM_PN \"Item\",b.DEPARTMENT_CODE ,TO_CHAR (a.finish_date, 'YYYYMMDDHH24MISS') \"Completion Date\"," +
                " TO_CHAR (a.finish_date, 'YYYYMMDDHH24MISS') \"Shipment Date\",a.invoice \"Shipment Number\",a.cust_po \"PO Number\"," +
                " a.shipping_qty \"Quantity Completed\",a.shipping_qty \"Quantity Shipped\",a.invoice \"Packing Slip Number\",a.tcom,TO_CHAR (a.finish_date, 'YYWW') \"WEEKTIME\" ,b.SOTEDIAG_VERSION,TO_CHAR (SYSDATE, 'YYWW') week" +
                " FROM sfism4.r_bpcs_invoice_t a, sfis1.c_brcm_pn_t b , SFIS1.C_MODEL_DESC_T c WHERE a.model_name = b.model_name and a.model_name = c.model_name and C.MODEL_SERIAL = 'NIC'" + zstr + " and a.invoice IN ( SELECT dn_no FROM sfism4.r_sap_dn_detail_t WHERE UPPER (ship_address) IN " +
                   " ('AVAGO TECHNOLOGIES INTERNATIONAL SALES@C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN','C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN' ) ) " +
                " AND  a.tcom in (select ship_no from sfism4.z107 where ship_no in a.tcom " +
                strDN +
                "and substr(mo_number,1,4)!='2279' and substr(mo_number,1,4)!='2280') ORDER BY finish_date ";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }
        public async Task<DataTable> ECDdtHeaderShip(string type, string DN, SfcHttpClient sfcClient)
        {
            string strDN = "";
            string zstr = "";
            if (type == "AUTO") zstr = " and a.invoice not in(select  INVOICE from  SFISM4.R_EDI_HAWB2BOX_T  where BOX ='Y' and invoice is not null) ";
            if (DN != "")
                strDN = string.Format(@" and a.INVOICE = '{0}' ", DN);
            else strDN = "and TO_CHAR (a.finish_date, 'YYYYMMDD') = '" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "'";
            string str = "SELECT   TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS') \"Record Creation Date\",b.BROADCOM_PN \"Item\",b.DEPARTMENT_CODE ,TO_CHAR (a.finish_date, 'YYYYMMDDHH24MISS') \"Completion Date\"," +
                " TO_CHAR (a.finish_date, 'YYYYMMDDHH24MISS') \"Shipment Date\",a.invoice \"Shipment Number\",a.cust_po \"PO Number\"," +
                " a.shipping_qty \"Quantity Completed\",a.shipping_qty \"Quantity Shipped\",a.invoice \"Packing Slip Number\",a.tcom,TO_CHAR (a.finish_date, 'YYWW') \"WEEKTIME\" ,b.SOTEDIAG_VERSION,TO_CHAR (SYSDATE, 'YYWW') week" +
                " FROM sfism4.r_bpcs_invoice_t a, sfis1.c_brcm_pn_t b , SFIS1.C_MODEL_DESC_T c WHERE a.model_name = b.model_name and a.model_name = c.model_name and C.MODEL_SERIAL = 'ECD' "+ zstr + " and a.invoice IN ( SELECT dn_no FROM sfism4.r_sap_dn_detail_t WHERE UPPER (ship_address) IN " +
                   " ('AVAGO TECHNOLOGIES INTERNATIONAL SALES@C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN','C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN' ) ) " +
                " AND  a.tcom in (select ship_no from sfism4.z107 where ship_no in a.tcom " +
                 strDN +
                "and substr(mo_number,1,4)!='2279' and substr(mo_number,1,4)!='2280')  ORDER BY finish_date ";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }
        public async Task<DataTable> SupercapdtHeaderShip(string type, string DN, SfcHttpClient sfcClient)
        {
            string strDN = "";
            string zstr = "";
            if (type == "AUTO") zstr = " and a.invoice not in(select  INVOICE from  SFISM4.R_EDI_HAWB2BOX_T  where BOX ='Y' and invoice is not null) ";
            if (DN != "")
                strDN = string.Format(@" and a.INVOICE = '{0}' ", DN);
            else strDN = "and TO_CHAR (a.finish_date, 'YYYYMMDD') = '" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "'";
            string str = "SELECT   TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS') \"Record Creation Date\",b.BROADCOM_PN \"Item\",b.DEPARTMENT_CODE ,TO_CHAR (a.finish_date, 'YYYYMMDDHH24MISS') \"Completion Date\"," +
                " TO_CHAR (a.finish_date, 'YYYYMMDDHH24MISS') \"Shipment Date\",a.invoice \"Shipment Number\",a.cust_po \"PO Number\"," +
                " a.shipping_qty \"Quantity Completed\",a.shipping_qty \"Quantity Shipped\",a.invoice \"Packing Slip Number\",a.tcom,TO_CHAR (a.finish_date, 'YYWW') \"WEEKTIME\" ,b.SOTEDIAG_VERSION,TO_CHAR (SYSDATE, 'YYWW') week" +
                " FROM sfism4.r_bpcs_invoice_t a, sfis1.c_brcm_pn_t b , SFIS1.C_MODEL_DESC_T c WHERE a.model_name = b.model_name and a.model_name = c.model_name and C.MODEL_SERIAL = 'SUPERCAP' " + zstr + " and a.invoice IN ( SELECT dn_no FROM sfism4.r_sap_dn_detail_t WHERE UPPER (ship_address) IN " +
                   " ('AVAGO TECHNOLOGIES INTERNATIONAL SALES@C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN','C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN' ) ) " +
                " AND  a.tcom in (select ship_no from sfism4.z107 where ship_no in a.tcom " +
                 strDN +
                "and substr(mo_number,1,4)!='2279' and substr(mo_number,1,4)!='2280')  ORDER BY finish_date ";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }
        #endregion
        #region get Datatable Carton for Ship File
        public async Task<DataTable> SuperCapShipCartonDT(string tcom, SfcHttpClient sfcClient)
        {
            string str = "select  a.* ,sum(num) over (order by mcarton_no rows between unbounded preceding and unbounded following ) lottotal from (SELECT  COUNT (mcarton_no) num,mo_number_old, mcarton_no," +
                      " 'P' ful FROM sfism4.z107 c WHERE c.ship_no = '" + tcom + "' GROUP BY mo_number_old,mcarton_no ) a";
            return await fDal.ExcuteSelectSQL(str, sfcClient);

        }
        public async Task<DataTable> NICShipCartonDT(string tcom, SfcHttpClient sfcClient)
        {
            string str = "select  a.* ,sum(num) over (order by license_no rows between unbounded preceding and unbounded following ) lottotal from (SELECT  COUNT (mcarton_no) num,mo_number_old, license_no, mcarton_no," +
                      " 'P' ful FROM sfism4.r_sec_lic_link_t b, sfism4.z107 c WHERE b.carton_no = mcarton_no  AND c.ship_no = '" + tcom + "' GROUP BY mo_number_old,license_no, mcarton_no ) a";
            return await fDal.ExcuteSelectSQL(str, sfcClient);

        }
        public async Task<DataTable> ECDShipCartonDT(string tcom, SfcHttpClient sfcHttpClient)
        {
            string str = "select  a.* ,sum(num) over (order by tray_no rows between unbounded preceding and unbounded following ) lottotal from (SELECT  COUNT (tray_no) num,mo_number_old, tray_no," +
                      " 'P' ful FROM sfism4.z107 c WHERE c.ship_no = '" + tcom + "' GROUP BY mo_number_old,tray_no ) a";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        #endregion
        #region BRCM ver
        public async Task<string> NICGetVer(string modelName, DataTable dtcarton, int j, SfcHttpClient sfcHttpClient)
        {
            string strver = string.Empty;
            string exmessage = "OK";
            string mcarton = dtcarton.Rows[j]["MCARTON_NO"].ToString();
            DataTable dt = new DataTable();
            //string sql = "select distinct decode(ssn3,'',substr(REGEXP_SUBSTR (ssn2,'[^,]+',1,4),-2),substr(b.ssn3,-2)) as ver  " +
            //   " from sfism4.z107 a, sfism4.r_custsn_t b where a.serial_number =b.SERIAL_NUMBER and a.MCARTON_NO='" + mcarton + "' ";
            //string sql = "select distinct substr(b.ssn3, -2) as ver from sfism4.z107 a, sfism4.r_custsn_t b where a.serial_number =b.SERIAL_NUMBER and a.MCARTON_NO='" + mcarton + "' ";

            string sql = "select distinct CUSTMODEL_DESC5 as ver from sfism4.z107 a, SFIS1.C_CUST_MODEL_T b where a.MODEL_NAME = b.MODEL_NAME and a.version_code=b.version_code and a.MCARTON_NO='" + mcarton + "' ";
            dt = await fDal.ExcuteSelectSQL(sql, sfcHttpClient);
            if (dt.Rows.Count > 1)
            {
                strver = mcarton + "存在多個BRCM Version,生成ship文件fail " + "\n";
                exmessage = "BRCM VERION IS MULTIPLE";
            }
            else if (dt.Rows.Count == 0)
            {
                strver = mcarton + "沒有BRCM Version,生成ship文件fail " + "\n";
                exmessage = "BRCM VERION IS NULL";
            }
            strver = dt.Rows[0]["VER"].ToString();
            if (modelName != "BCM957414A4141HC") 
            { 
                if (strver.Length == 1 ) strver = "00" + strver;
                if (strver.Length == 2 ) strver =  "0" + strver;
            }
            exmessage = "OK";
            return strver + "," +  exmessage;
        }
        public async Task<string> ECDGetVer(DataTable dtcarton, int j, SfcHttpClient sfcHttpClient)
        {
            string strver = string.Empty;
            string exmessage = "OK";
            string trayno = dtcarton.Rows[j]["TRAY_NO"].ToString();
            DataTable dt = new DataTable();
            string sql = "select distinct CUSTMODEL_DESC5 ver from sfism4.z107 a, SFIS1.C_CUST_MODEL_T b " +
                " where a.MODEL_NAME = b.MODEL_NAME and a.version_code=b.version_code and a.TRAY_NO = '" + trayno + "' ";

            dt = await fDal.ExcuteSelectSQL(sql, sfcHttpClient);
            if (dt.Rows.Count > 1)
            {
                strver = trayno + "存在多個BRCM Version,生成ship文件fail " + "\n";
                exmessage = "BRCM VERION IS MULTIPLE";
            }
            else if (dt.Rows.Count == 0)
            {
                strver = trayno + "沒有BRCM Version,生成ship文件fail " + "\n";
                exmessage = "BRCM VERION IS NULL";
            }
            strver = dt.Rows[0]["VER"].ToString();//PM yeu cau giu nguyen version toan bo hang ECD
            exmessage = "OK";
            return strver + "," + exmessage;
        }
        public async Task<string> SuperCapGetVer(DataTable dtcarton, int j, SfcHttpClient sfcHttpClient)
        {
            string strver = string.Empty;
            string exmessage = "OK";
            string mcarton = dtcarton.Rows[j]["MCARTON_NO"].ToString();
            DataTable dt = new DataTable();
            string str = "select distinct lPAD(CUSTMODEL_DESC5,3,'0') ver from sfism4.z107 a, SFIS1.C_CUST_MODEL_T b " +
                " where a.MODEL_NAME = b.MODEL_NAME and a.version_code=b.version_code and a.MCARTON_NO='" + mcarton + "'";
            dt= await fDal.ExcuteSelectSQL(str, sfcHttpClient);
            if (dt.Rows.Count > 1)
            {
                strver = dtcarton.Rows[j]["TRAY_NO"].ToString() + "存在多個BRCM Version,生成ship文件fail " + "\n";
                exmessage = "BRCM VERION IS MULTIPLE";
            }
            else if (dt.Rows.Count == 0)
            {
                strver = dtcarton.Rows[j]["TRAY_NO"].ToString() + "沒有BRCM Version,生成ship文件fail " + "\n";
                exmessage = "BRCM VERION IS NULL";
            }
            strver = dt.Rows[0]["VER"].ToString();
            exmessage = "OK";
            return strver + "," + exmessage;
        }
        #endregion
        
        public async Task<DataTable> NICShipMoDT(string mcarton, SfcHttpClient sfcHttpClient)
        {
            //--Apply for NIC , auto and manual
            string str = "select count(mo_number),mo_number,version_code from sfism4.z107 where mcarton_no='" + mcarton + "' group by mo_number,version_code order by mo_number desc";
            if (mcarton.StartsWith("BZ") && mcarton.Trim().Length == 10)
                str = "select count(mo_number),mo_number_old mo_number,version_code from sfism4.z107 where mcarton_no='" + mcarton + "' group by mo_number_old,version_code";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }

        #region get String Header for Ship File
        public async Task<string> NICGetStringHeader(string createtime, DataTable dthead,int i)
        {
            return "SHIP|B_FOXVN_BD|" + createtime + "|HEADER||" + dthead.Rows[i]["Item"].ToString() + "|EA|" + dthead.Rows[i]["DEPARTMENT_CODE"].ToString() + "|" +
                           dthead.Rows[i]["Completion Date"].ToString() + "|" + dthead.Rows[i]["Shipment Date"].ToString() + "|" + dthead.Rows[i]["Shipment Number"].ToString() + "|||||||" +
                            "||||" + dthead.Rows[i]["Quantity Completed"].ToString() + "|" + dthead.Rows[i]["Quantity Shipped"].ToString() + "||" +
                           dthead.Rows[i]["Packing Slip Number"].ToString() + "||||||||||||||||D|||||||||||||";
        }
        public async Task<string> ECDGetStringHeader(string createtime, DataTable dthead,  int i)
        {
            return "SHIP|B_FOXVN_BD|" + createtime + "|HEADER||" + dthead.Rows[i]["Item"].ToString() + "|EA|" + dthead.Rows[i]["DEPARTMENT_CODE"].ToString() + "|" +
                           dthead.Rows[i]["Completion Date"].ToString() + "|" + dthead.Rows[i]["Shipment Date"].ToString() + "|" + dthead.Rows[i]["Shipment Number"].ToString() + "|||||||" +
                           dthead.Rows[i]["PO Number"].ToString() + "||||" + dthead.Rows[i]["Quantity Completed"].ToString() + "|" + dthead.Rows[i]["Quantity Shipped"].ToString() + "||" +
                           dthead.Rows[i]["Packing Slip Number"].ToString() + "||||||||||||||||D|||||||||||";
        }
        public async Task<string> SuperCapGetStringHeader(string createtime, DataTable dthead,  int i)
        {
            return "SHIP|B_FOXVN_BD|" + createtime + "|HEADER||" + dthead.Rows[i]["Item"].ToString() + "|EA|" + dthead.Rows[i]["DEPARTMENT_CODE"].ToString() + "|" +
                           dthead.Rows[i]["Completion Date"].ToString() + "|" + dthead.Rows[i]["Shipment Date"].ToString() + "|" + dthead.Rows[i]["Shipment Number"].ToString() + "|||||||" +
                           dthead.Rows[i]["PO Number"].ToString() + "||||" + dthead.Rows[i]["Quantity Completed"].ToString() + "|" + dthead.Rows[i]["Quantity Shipped"].ToString() + "||" +
                           dthead.Rows[i]["Packing Slip Number"].ToString() + "||||||||||||||||D|||||||||||";
        }
        public async Task<string> NICGetStringContent(string createtime, string LotID, string version, DataTable dthead, int i, DataTable dtcarton,int j)
        {
            return "SHIP|B_FOXVN_BD|" + createtime + "|LOT||" + dthead.Rows[i]["Item"].ToString() + "||" + dthead.Rows[i]["DEPARTMENT_CODE"].ToString() + "|||" +
                                dthead.Rows[i]["Shipment Number"].ToString() + "||||||||||||||||" + LotID + "|" + dtcarton.Rows[j]["NUM"] + "|VN|" +
                                dtcarton.Rows[j]["LICENSE_NO"] + "|" + dthead.Rows[i]["Shipment Date"].ToString() + "||||" + version + "|||" 
                                + dthead.Rows[i]["SOTEDIAG_VERSION"].ToString() + "||||||||||" + dthead.Rows[i]["Shipment Date"].ToString() + "|||" 
                                + dthead.Rows[i]["WEEK"].ToString() + "||" + dtcarton.Rows[j]["LICENSE_NO"] + "|" + dtcarton.Rows[j]["FUL"] + "";
        }
        public async Task<string> ECDGetStringContent(string createtime, string LotID,string version, DataTable dthead, int i, DataTable dtcarton,int j)
        {
            return "SHIP|B_FOXVN_BD|" + createtime + "|LOT||" + dthead.Rows[i]["Item"].ToString() + "||" + dthead.Rows[i]["DEPARTMENT_CODE"].ToString() + "|||" +
                                dthead.Rows[i]["Shipment Number"].ToString() + "||||||||||||||||" + LotID + "|" + dtcarton.Rows[j]["NUM"] + "|VN|" +
                                dtcarton.Rows[j]["TRAY_NO"] + "|" + dthead.Rows[i]["Shipment Date"].ToString() + "||||" + version 
                                + "|||||||||||||" + dthead.Rows[i]["Shipment Date"].ToString() + "||||" + "";
        }
        public async Task<string> SuperCapGetStringContent(string createtime,string LotID, string version, DataTable dthead, int i, DataTable dtcarton, int j)
        {
            return "SHIP|B_FOXVN_BD|" + createtime + "|LOT||" + dthead.Rows[i]["Item"].ToString() + "||" + dthead.Rows[i]["DEPARTMENT_CODE"].ToString() + "|||" +
                            dthead.Rows[i]["Shipment Number"].ToString() + "||||||||||||||||" + LotID + "|" + dtcarton.Rows[j]["NUM"] + "|VN|" +
                            dtcarton.Rows[j]["mcarton_no"] + "|" + dthead.Rows[i]["Shipment Date"].ToString() + "||||" + version + "|||||||||||||" + dthead.Rows[i]["Shipment Date"].ToString() + "||||" + "";
        }
        #endregion

        public async Task<string> ShipGetLotID(string type,DataTable dtmo,DataTable dtcarton,int j)
        {
            string LotID = dtcarton.Rows[j]["MO_NUMBER_OLD"].ToString();//All ECD SuperCap
            if (type=="NIC")
            {
                LotID = dtmo.Rows[0]["MO_NUMBER"].ToString();
                if (dtcarton.Rows[j]["MCARTON_NO"].ToString().StartsWith("BZ") & dtcarton.Rows[j]["MCARTON_NO"].ToString().Trim().Length == 10)
                LotID = dtcarton.Rows[j]["MO_NUMBER_OLD"].ToString();
            }
            LotID = LotID.Trim();
            return LotID;
        }
        //------------------------------WIPC File------------------------------------------------------------------------------------
        #region WipCDT
        public async Task<DataTable> NICWipCmoDT(string invo, SfcHttpClient sfcClient)
        {
            string str = "select case when length(b.mcarton_no)=10 and substr(b.mcarton_no,1,2)='BZ' then mo_number_old else mo_number end mo_number,ship_no,count(*)*c.component_qty count  from sfism4.r_bpcs_invoice_t a,sfism4.z107 b ,sfis1.c_brcm_ic_t c where a.tcom=b.ship_no and a.model_name=c.model_name and invoice='" + invo + "' group by ship_no, case when length(b.mcarton_no)=10 and substr(b.mcarton_no,1,2)='BZ' then mo_number_old else mo_number end,c.component_qty";
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }
        public async Task<DataTable> ECDWipCmoDT(string invo, SfcHttpClient sfcHttpClient)
        {
            string str = " SELECT ship_no,b.mo_number_old,c.component_item,C.VENDER_ITEM, COUNT (*) * c.component_qty COUNT, C.COMPONENT_QTY  " +
                         "from sfism4.r_bpcs_invoice_t a,sfism4.z107 b ,sfis1.c_brcm_ic_t c " +
                         "where a.tcom = b.ship_no AND a.model_name = c.model_name AND invoice='" + invo + "' " +
                         "group BY ship_no,c.component_item, b.mo_number_old, c.component_qty,C.VENDER_ITEM order BY ship_no, b.mo_number_old, C.COMPONENT_QTY,C.VENDER_ITEM";
            return await fDal.ExcuteSelectSQL(str, sfcHttpClient);
        }
        public async Task<DataTable> NICWipcdt(string type, string DN, SfcHttpClient sfcHttpClient)
        {
            string strWhere1 = "", strWhere2 = "", sql ="";
            if (type == "AUTO")
            {
                strWhere1 = " and TO_CHAR (a.finish_date, 'YYYYMMDD') = '" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "'";
                strWhere2 = "and a.invoice not in(select  INVOICE from  SFISM4.R_EDI_HAWB2BOX_T  where BOX ='Y' and invoice is not null)"; 
            }else
            {
                if (DN != "") strWhere1 = string.Format(@" and a.INVOICE = '{0}' ", DN);
                else strWhere1 = " and TO_CHAR (a.finish_date, 'YYYYMMDD') = '" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "'";
                strWhere2 = "";
            }
            sql = "SELECT   TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS') \"Record Creation Date\",  a.invoice \"Shipment Number\"," +
                " b.broadcom_pn \"Assembly Item\",c.COMPONENT_ITEM \"Component Item\",b.DEPARTMENT_CODE ,c.COMPONENT_QTY \"Quantity Consumed\",a.model_name model,c.CHIP_DEPARTMENT_CODE chipdepart" +
                " FROM sfism4.r_bpcs_invoice_t a, sfis1.c_brcm_pn_t b,sfis1.C_BRCM_IC_T c , SFIS1.C_MODEL_DESC_T d" +
                " WHERE a.model_name = b.model_name and a.model_name = D.MODEL_NAME and D.MODEL_SERIAL = 'NIC'  and a.model_name=c.model_name "+strWhere2+"  and a.invoice IN ( SELECT dn_no FROM sfism4.r_sap_dn_detail_t WHERE UPPER (ship_address) IN " +
                " ('AVAGO TECHNOLOGIES INTERNATIONAL SALES@C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN','C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN' )) " +
                " AND a.tcom in (select ship_no from sfism4.z107 where ship_no in a.tcom and substr(mo_number,1,4)!='2279' and substr(mo_number,1,4)!='2280')  "+strWhere1+" ORDER BY finish_date ";
            return await fDal.ExcuteSelectSQL(sql, sfcHttpClient);
        }
        public async Task<DataTable> ECDWipcdt(string type, string DN, SfcHttpClient sfcHttpClient)
        {
            string strWhere1 = "", strWhere2 = "", sql = "";
            if (type == "AUTO")
            {
                strWhere1 = " and TO_CHAR (a.finish_date, 'YYYYMMDD') = '" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "'";
                strWhere2 = "and a.invoice not in(select  INVOICE from  SFISM4.R_EDI_HAWB2BOX_T  where BOX ='Y' and invoice is not null)";
            }
            else
            {
                if (DN != "") strWhere1 = string.Format(@" and a.INVOICE = '{0}' ", DN);
                else strWhere1 = " and TO_CHAR (a.finish_date, 'YYYYMMDD') = '" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "'";
                strWhere2 = "";
            }
            sql = " select distinct  * from ( SELECT   TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS') \"Record Creation Date\",  a.invoice \"Shipment Number\"," +
                " b.broadcom_pn \"Assembly Item\",b.DEPARTMENT_CODE,a.model_name model,c.CHIP_DEPARTMENT_CODE chipdepart" +
                " FROM sfism4.r_bpcs_invoice_t a, sfis1.c_brcm_pn_t b,sfis1.C_BRCM_IC_T c , SFIS1.C_MODEL_DESC_T d" +
                " WHERE a.model_name = b.model_name and a.model_name = D.MODEL_NAME and D.MODEL_SERIAL = 'ECD'  and a.model_name=c.model_name "+ strWhere2 + " and a.invoice IN ( SELECT dn_no FROM sfism4.r_sap_dn_detail_t WHERE UPPER (ship_address) IN " +
                   " ('AVAGO TECHNOLOGIES INTERNATIONAL SALES@C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN','C/O AVAGO TECHNOLOGIES (M) SDN BHD (RECEIVING),MALAYSIA,MALAYSIAN' )) " +
                " AND a.tcom in (select ship_no from sfism4.z107 where ship_no in a.tcom and substr(mo_number,1,4)!='2279' and substr(mo_number,1,4)!='2280')  "+ strWhere1 + " ORDER BY finish_date) ";
            return await fDal.ExcuteSelectSQL(sql, sfcHttpClient);
        }
        #endregion
        //------------------------------WIPC File------------------------------------------------------------------------------------
        //------------------------------BDSN File------------------------------------------------------------------------------------
        #region BDSN
        public async Task<DataTable> bdsnrecordSuperCap(string type, string DN, SfcHttpClient sfcClient)
        {
            string strWhere1 = "", strWhere2 = "";
            strWhere1 = "and TO_CHAR (c.finish_date, 'YYYYMMDD') = '" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "'";
            if (type == "AUTO")
            {
                strWhere2 = "and c.invoice not in(select  INVOICE from  SFISM4.R_EDI_HAWB2BOX_T  where BOX ='Y' and invoice is not null)";
            }
            else
            {
                if (DN != "") strWhere1 = string.Format(@"AND c.INVOICE = '{0}' ", DN);
            }
            string str = string.Format(@"SELECT (CASE  WHEN SUBSTR (a.mo_number, 1, 4) in ('2279','2280','2645')  THEN a.shipping_sn || 'RWK'  ELSE a.shipping_sn END) serial_number ,
                            a.mo_number_old LotID, a.tray_no, b.broadcom_pn ,a.mcarton_no
                            FROM sfism4.z107 a, sfis1.c_brcm_pn_t b, SFIS1.C_MODEL_DESC_T c 
                            WHERE b.model_name = a.model_name and a.model_name = c.model_name and c.model_serial = 'SUPERCAP' AND a.ship_no IN ( 
                            SELECT tcom FROM sfism4.r_bpcs_invoice_t c,sfis1.c_brcm_pn_t e,sfism4.r_sap_dn_detail_t f WHERE 1 = 1 {0}   
                            and c.model_name=e.model_name {1} and c.invoice=f.dn_no  and c.model_name=f.model_name and upper(f.ship_address) not in('LOT B, QUE VO INDUSTRIAL ZONE.,VIET NAM,VIETNAMESE')) order by mo_number_old , b.broadcom_pn", strWhere1,strWhere2);
            return await fDal.ExcuteSelectSQL(str, sfcClient);
        }
        public async Task<DataTable> bdsnrecordNIC(string type,string DN, SfcHttpClient sfcHttpClient)
        {
            string strWhere1="",strWhere2 = "";
            strWhere1 = "and TO_CHAR (c.finish_date, 'YYYYMMDD') = '" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "'";
            if (type=="AUTO")
            {
                strWhere2 = "and c.invoice not in(select  INVOICE from  SFISM4.R_EDI_HAWB2BOX_T  where BOX ='Y' and invoice is not null)";
            }
            else
            {
                if (DN != "") strWhere1 = " and c.invoice='"+DN+"'";
            }
            string sql = string.Format(@"SELECT*FROM( SELECT decode(a.model_name,'F5719A1904IFRU',h.key_part_sn,shipping_sn)shipping_sn,(CASE WHEN SUBSTR (a.mo_number, 1, 4) IN ('2279', '2280','2645')
                           THEN DECODE (SUBSTR (a.serial_number, 1, 3),'23S', SUBSTR (a.serial_number, 4),a.serial_number)|| 'RWK'
                           ELSE DECODE (SUBSTR (a.serial_number, 1, 3),'23S', SUBSTR (a.serial_number, 4),a.serial_number) END) AS SERIAL_NUMBER, 
                           case when d.carton_no=d.license_no and substr(d.carton_no,1,2)='BZ' then a.mo_number_old else a.mo_number end LotID, d.license_no, b.broadcom_pn 
                            FROM sfism4.z107 a, sfis1.c_brcm_pn_t b, sfism4.r_sec_lic_link_t d ,(select * from sfism4.r108 where key_part_no='SSN9') h 
                            WHERE a.serial_number=h.serial_number(+) and b.model_name = a.model_name AND a.mcarton_no = d.carton_no AND a.ship_no IN ( 
                            SELECT tcom FROM sfism4.r_bpcs_invoice_t c,sfis1.c_brcm_pn_t e,sfism4.r_sap_dn_detail_t f ,sfis1.c104 z WHERE 1 = 1 {0}  
                            and c.model_name = e.model_name and c.model_name = z.model_name and z.model_serial='NIC' {1} and c.invoice = f.dn_no and c.model_name = f.model_name and upper(f.ship_address) not in ('LOT B, QUE VO INDUSTRIAL ZONE.,VIET NAM,VIETNAMESE',
                            'C/O AVAGO TECHNOLOGIES (M) SDN.BHD,LOCKE,USA,AMERICAN','LOCKED BAG NO. 207 PEJABAT SERAHAN,MALAYSIA,MALAYSIAN'))  AND SUBSTR(a.mo_number, 1, 4) NOT IN('2279', '2280','2645')
                            UNION ALL
                            SELECT decode(a.model_name,'F5719A1904IFRU',h.key_part_sn,shipping_sn)shipping_sn,(CASE WHEN SUBSTR(a.mo_number, 1, 4) IN('2279', '2280', '2645')
                            THEN DECODE(SUBSTR(a.serial_number, 1, 3),'23S', SUBSTR(a.serial_number, 4),a.serial_number)|| 'RWK'
                            ELSE DECODE(SUBSTR(a.serial_number, 1, 3),'23S', SUBSTR(a.serial_number, 4),a.serial_number) END) AS SERIAL_NUMBER,
                           case when d.carton_no=d.license_no and substr(d.carton_no,1,2)='BZ' then a.mo_number_old else a.mo_number end LotID, d.license_no, b.broadcom_pn
                            FROM sfism4.z107 a, sfis1.c_brcm_pn_t b, sfism4.r_sec_lic_link_t d,(select * from sfism4.r108 where key_part_no='SSN9') h 
                            WHERE a.serial_number=h.serial_number(+) and b.model_name = a.model_name AND a.mcarton_no = d.carton_no AND a.ship_no IN(
                            SELECT tcom FROM sfism4.r_bpcs_invoice_t c, sfis1.c_brcm_pn_t e, sfism4.r_sap_dn_detail_t f ,sfis1.c104 z WHERE 1 = 1 {0}
                            and c.model_name = e.model_name and c.model_name = z.model_name and z.model_serial='NIC' {1} and c.invoice = f.dn_no and c.model_name = f.model_name) AND SUBSTR(a.mo_number, 1, 4) IN('2279', '2280', '2645')) order by broadcom_pn,SERIAL_NUMBER", strWhere1, strWhere2);
            return await fDal.ExcuteSelectSQL(sql, sfcHttpClient);
        }
        public async Task<DataTable> bdsnrecordECD(string type, string DN, SfcHttpClient sfcHttpClient)
        {
            string strWhere1 = "", strWhere2 = "";
            strWhere1 = "and TO_CHAR (c.finish_date, 'YYYYMMDD') = '" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "'";
            if (type == "AUTO")
            {
                strWhere2 = "and c.invoice not in(select  INVOICE from  SFISM4.R_EDI_HAWB2BOX_T  where BOX ='Y' and invoice is not null)";
            }
            else
            {
                if (DN != "") strWhere1 = " and c.invoice='" + DN + "'";
            }
            string sql = $@"SELECT (CASE  WHEN SUBSTR (a.mo_number, 1, 4) in ('2279','2280','2645')  THEN a.serial_number || 'RWK'  ELSE a.serial_number END) serial_number ,
                                   (CASE  WHEN SUBSTR (a.mo_number, 1, 4) in ('2279','2280','2645')  THEN a.shipping_sn || 'RWK'  ELSE a.shipping_sn END) shipping_sn,
                            a.mo_number_old LotID, a.tray_no, b.broadcom_pn 
                            FROM sfism4.z107 a, sfis1.c_brcm_pn_t b, SFIS1.C_MODEL_DESC_T c 
                            WHERE b.model_name = a.model_name and a.model_name = c.model_name and c.model_serial = 'ECD' AND a.ship_no IN ( 
                            SELECT tcom FROM sfism4.r_bpcs_invoice_t c,sfis1.c_brcm_pn_t e,sfism4.r_sap_dn_detail_t f WHERE 1=1 " +strWhere1 +
                            "and c.model_name=e.model_name " + strWhere2 + "  and c.invoice=f.dn_no and c.model_name=f.model_name and upper(f.ship_address) not in('LOT B, QUE VO INDUSTRIAL ZONE.,VIET NAM,VIETNAMESE')) order by mo_number_old, b.broadcom_pn,serial_number";

            return await fDal.ExcuteSelectSQL(sql, sfcHttpClient);
        }
        #endregion
        //------------------------------BDSN File------------------------------------------------------------------------------------
        public async Task<DataTable> getFile(string DN,string PN,string fromTime,string toTime,string modelSerial,string fileType, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = new DataTable();
            try
            {
                var zresult = sfcHttpClient.QueryList(new QuerySingleParameterModel
                {
                    CommandText = "sfis1.b2b_get_file",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                    new SfcParameter { Name = "IN_DN", Value = DN, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter { Name = "IN_PN", Value = PN, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter { Name = "IN_TIME1", Value = fromTime, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter { Name = "IN_TIME2", Value = toTime, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter { Name = "IN_DATA", Value = modelSerial, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter { Name = "IN_TYPE", Value = fileType, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter { Name = "o_dt",  SfcParameterDataType = SfcParameterDataType.RefCursor, SfcParameterDirection = SfcParameterDirection.Output },
                    new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }
                }
                });

                var temp = JsonConvert.SerializeObject(zresult.Data);
                dt = JsonConvert.DeserializeObject<DataTable>(temp);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return dt;
        }
    }
}
