using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingFile
{
    class DAL
    {
        public string loginNO = null;

        public string loginName = null;

        public string userRank = "9";
        public async Task<string> CheckLogin(string emp, string LoginPwd, SfcHttpClient sfcHttpClient)
        {
            string result = null;
            DataTable reader = null;
            try
            {
                string sql = "select emp_no,emp_name,emp_rank from SFIS1.C_EMP_DESC_T where emp_no= '" + emp + "' and emp_bc = '" + LoginPwd + "'";
                reader = await ExecuteSQL(sql, sfcHttpClient);
                if (reader.Rows.Count > 0)
                {
                    result = reader.Rows[0]["emp_no"].ToString();
                    result += "-" + reader.Rows[0]["emp_name"].ToString();
                    result += "-" + reader.Rows[0]["emp_rank"].ToString();

                }
            }
            catch
            {
                result = null;
            }

            return result;
        }
        public async Task<DataTable> GetShipInfoByDN(string dn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT   '' ESN_DEC,
                                A.SERIAL_NUMBER FSN,
                                '' ESN_HEX,
                                C.CUSTOMER PN,
                                A.JOB FW_REV,
                                '' AKEY,
                                '' IMSI,
                                '' MSL,
                                '' OTKSL,
                                '' LOCK_CODE,
                                '' NAI,
                                '' CHAP_KEY,
                                '' QNC_USER,
                                '' QNC_PASS,
                                '' HA_KEY,
                                '' AAA_KEY,
                                TO_CHAR (A.WARRANTY_DATE, 'MM/DD/YY HH24:MM:SS') CONFIG_DATE,
                                '' U_VER,
                                '' PRI_VER,
                                '' AKEY2,
                                A.SERIAL_NUMBER IMEI,
                                '' GSM_LOCK,
                                '' UNLOCK_NS,
                                '' ME_LOCK,
                                '' ICCID,
                                '' IMSI_2,
                                '' BATCH_NO,
                                '' HOST_FSN,
                                '' HOST_FW_VER,
                                A.MCARTON_NO CARTON_ID,
                                '' PALLET_ID,
                                '' MNA,
                                '' HOST_BATCH,
                                '' PRL,
                                '' ROYALTY_1,
                                '' ROYALTY_2,
                                '' CSN,
                                '' HDRAN,
                                '' MEID_DEC,
                                SUBSTR (A.SERIAL_NUMBER, 1, 14) MEID_HEX,
                                '' PESN,
                                '' PHEX,
                                '' WIFI_MAC,
                                '' WIMAX,
                                '' IDSCRE,
                                '' IDSTPS,
                                '' APPPBK,
                                '' DEVLOGIN,
                                '' DEVPWD,
                                '' FWPBK,
                                '' NONCE,
                                '' SVRPWD,
                                '' PRL2,
                                '' DCN,
                                '' STN,
                                '' OUTER_BOX,
                                '' W_SSID,
                                '' W_ADMIN_PW,
                                '' WPS_PIN,
                                '' W_PASSPHRASE,
                                TO_CHAR (A.IN_STATION_TIME, 'YYMMDD') AT_CUSTOMIZATION_FILENAME,
                                '' FW_REV_VZW_GOBI,
                                 B.CUST_PO 
                                FROM   (SELECT   SERIAL_NUMBER,
                                        IN_STATION_TIME,
                                        OUT_LINE_TIME,
                                        MCARTON_NO,
                                        IMEI,
                                        SHIP_NO,
                                        WARRANTY_DATE,
                                        JOB,
                                        MODEL_NAME
                                   FROM SFISM4.Z107
                                  WHERE SHIP_NO IN (SELECT TCOM
                                                      FROM SFISM4.R_BPCS_INVOICE_T
                                                     WHERE INVOICE = '{0}')) A,
                                (SELECT INVOICE, TCOM, CUST_PO,MODEL_NAME
                                   FROM SFISM4.R_BPCS_INVOICE_T
                                  WHERE INVOICE = '{0}') B,
                                 SFIS1.C_MODEL_DESC_T C
                          WHERE A.SHIP_NO = B.TCOM
                          AND C.MODEL_NAME = A.MODEL_NAME
                          ORDER BY A.SERIAL_NUMBER", dn);


                dt = await ExecuteSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return dt;
        }
        public async Task<DataTable> GetShipXMLByDN(string dn, string model_name, string invoice, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;

            try
            {
                string sql_pls62 = string.Format(@"SELECT distinct Z.SERIAL_NUMBER,
                                               SAPDN.CUST_PO||'-'||PO.TYPE   PO_NO,
                                               Z.IMEI,
                                               Z.TRAY_NO,
                                               Z.MCARTON_NO,
                                               Z.MODEL_NAME AS MATNOFACTORY,
                                               DECODE(CUST.SSN7,null,ship.acckey,CUST.SSN7) AS ACCKEY,
                                               '' AS A4,
                                               TO_CHAR (PR.IN_STATION_TIME, 'YYYYMMDDHHMMSS') AS BAG_TIME,
                                               TO_CHAR (PB.IN_STATION_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                               TO_CHAR (FT.IN_STATION_TIME, 'YYYYMMDD') AS FT_DATE,
                                               RB.INVOICE DN_NO,
                                               CONCAT (R117.YEA, R117.MONT) AS PRODUCT_DC,
                                               RB.DNP,
                                               --TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS FT_DATE,
                                               RMB.DEFAULT_LINE AS TEST_LINE,
                                               RB.SO_NUMBER AS PO_NUMBER,
                                               LPAD(ROUND(((PB.IN_STATION_TIME - RO.IN_STATION_TIME) * 24 * 60 * 60),0),9,0)
                                                  AS FLOOR_TIME,
                                               CC.DATA3 AS INFO04,
                                               CC.DATA4 AS INFO05,
                                               CC.DATA2 AS EAN,
                                               CC.INFO06,
                                               CC.DATA1 || '-' || CC.HW AS PART_NUMBER_SHIELD,
                                               CC.DIVICE_NUMBER || '-' || SUBSTR (RMB.HW_BOM, 1, 2)
                                                  AS CUST_MODEL_NAME,
                                               RMB.HW_BOM AS REVISION,
                                               TRSN.PANEL_NO,
                                               RMB.SW_BOM AS SW,
                                               TO_CHAR (ADD_MONTHS (RB.FINISH_DATE, 12), 'YYYYMMDD')
                                                  AS WARRANTY_EXPIRED_DATE,
                                               TO_CHAR (RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,
                                               TO_CHAR (RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
                                               TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS PRODUCT_DATE,
                                               TO_CHAR (RMB.MO_KP_START_DATE, 'HH24MISS') AS PRODUCT_TIME,
                                               CC.DATA1 || '-' || SUBSTR (RMB.HW_BOM, 1, 2) AS PART_NUMBER_CUSTOMER,
                                               '' AS A14,
                                               '' AS A15,
                                               --TO_CHAR (Z.IN_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                               CC.CUST_MODEL_NAME PRODUCT_TYPE,
                                               CC.CUST_MODEL_NAME INF02,
                                               'FOV' AS PRODUCT_LOCATION,
                                               '' AS A40
                                          FROM (SELECT FINISH_DATE,
                                                       SO_NUMBER,
                                                       TCOM,
                                                       INVOICE,
                                                       INV_NO,
                                                       MODEL_NAME,
                                                       ROWNUM * 10 AS DNP
                                                  FROM SFISM4.R_BPCS_INVOICE_T
                                                 WHERE INVOICE = '{0}' AND ROWNUM = 1) RB,
                                               SFISM4.Z_WIP_TRACKING_T Z,
                                               (  SELECT distinct C.SERIAL_NUMBER,
                                                         CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'MM')
                                                            WHEN '01' THEN '1'
                                                            WHEN '02' THEN '2'
                                                            WHEN '03' THEN '3'
                                                            WHEN '04' THEN '4'
                                                            WHEN '05' THEN '5'
                                                            WHEN '06' THEN '6'
                                                            WHEN '07' THEN '7'
                                                            WHEN '08' THEN '8'
                                                            WHEN '09' THEN '9'
                                                            WHEN '10' THEN 'O'
                                                            WHEN '11' THEN 'N'
                                                            WHEN '12' THEN 'D'
                                                         END
                                                            AS MONT,
                                                         CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'YYYY')
                                                            WHEN '2020' THEN 'M'
                                                            WHEN '2021' THEN 'N'
                                                            WHEN '2022' THEN 'P'
                                                            WHEN '2023' THEN 'R'
                                                            WHEN '2024' THEN 'S'
                                                            WHEN '2025' THEN 'T'
                                                            WHEN '2026' THEN 'U'
                                                            WHEN '2027' THEN 'V'
                                                            WHEN '2028' THEN 'W'
                                                            WHEN '2029' THEN 'X'
                                                         END
                                                            AS YEA
                                                    FROM SFISM4.Z_WIP_TRACKING_T A,
                                                         SFISM4.R_BPCS_INVOICE_T B,
                                                         SFISM4.R_SN_DETAIL_T C
                                                   WHERE     A.SHIP_NO = B.TCOM
                                                         AND B.INVOICE = '{0}'
                                                         AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                                GROUP BY C.SERIAL_NUMBER) R117,
                                               (SELECT MODEL_NAME,
                                                       DIVICE_NUMBER,
                                                       CUST_MODEL_NAME,
                                                       DATA1,
                                                       DATA2,
                                                       DATA3,
                                                       DATA4,
                                                       HW,
                                                       INFO06
                                                  FROM SFIS1.C_CINTERION_SHIP_T) CC,
                                               (SELECT *
                                                  FROM SFISM4.R_MO_BASE_T
                                                 WHERE MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T))
                                               RMB,
                                               SFISM4.R_SN_TRSN_LINK_T TRSN,
                                               SFISM4.R117 PR,
                                               SFISM4.R117 PB,
                                               SFISM4.R117 FT,
                                               SFISM4.R_CUSTSN_T CUST,
                                               SFISM4.R117 RO,
                                               SFISM4.R_SAP_DN_DETAIL_T SAPDN,SFISM4.R_CINTERION_TESTLOG_T ship,
                                               SFIS1.C_PO_CONFIG_T PO
                                         WHERE     RB.TCOM = Z.SHIP_NO
                                               AND SAPDN.DN_NO = RB.INVOICE
                                               AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+)
                                               AND Z.SERIAL_NUMBER = ship.SERIAL_NUMBER(+)
                                               AND Z.MODEL_NAME = CC.MODEL_NAME
                                               AND Z.MO_NUMBER = RMB.MO_NUMBER
                                               AND Z.SERIAL_NUMBER = TRSN.SERIAL_NUMBER
                                               AND PR.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                               AND PR.GROUP_NAME = 'PRINT'
                                               AND PR.IN_STATION_TIME =
                                                      (SELECT MAX (IN_STATION_TIME)
                                                         FROM SFISM4.R117
                                                        WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                              AND GROUP_NAME = 'PRINT')
                                               AND PB.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                               AND PB.GROUP_NAME = 'CHECK_MSL'
                                               AND PB.IN_STATION_TIME =
                                                      (SELECT MAX (IN_STATION_TIME)
                                                         FROM SFISM4.R117
                                                        WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                              AND GROUP_NAME = 'CHECK_MSL')
                                               AND RO.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                               AND RO.GROUP_NAME = 'ROAST_OUT'
                                               AND RO.IN_STATION_TIME =
                                                      (SELECT MAX (IN_STATION_TIME)
                                                         FROM SFISM4.R117
                                                        WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                              AND GROUP_NAME = 'ROAST_OUT')
                                               AND FT.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                               AND FT.GROUP_NAME = 'FT'
                                               AND FT.IN_STATION_TIME =
                                                      (SELECT MAX (IN_STATION_TIME)
                                                         FROM SFISM4.R117
                                                        WHERE SERIAL_NUMBER = Z.SERIAL_NUMBER AND GROUP_NAME = 'FT')
                                               AND CUST.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                AND PO.MODEL_NAME = SAPDN.DN_NO ", dn);

                string sql_serval = string.Format(@"SELECT distinct Z.SERIAL_NUMBER,
                                                           SAPDN.CUST_PO||'-'||PO.TYPE PO_NO,
                                                           Z.IMEI,
                                                           Z.TRAY_NO,
                                                           Z.MCARTON_NO,
                                                           Z.MODEL_NAME AS MATNOFACTORY,
                                                           SHIP.ACCKEY AS ACCKEY,
                                                           SHIP.FLASH_KEY,
                                                           TO_CHAR(SHIP.TEST_TIME, 'YYYYMMDDHHMMSS') AS TEST_TIME,
                                                           TO_CHAR(SHIP.TRANSFER_TIME, 'YYYYMMDDHHMMSS') AS TRANSFER_TIME,
                                                           SHIP.CERTIFICATE_ID AS CERTIFICATE_ID,
                                                           SHIP.CERTIFICATE_PROFILE AS CERTIFICATE_PROFILE,
                                                           SHIP.PWFACTORY,
                                                           SHIP.PWDEBUG,
                                                           SHIP.PWPRETLS,
                                                           SHIP.DTLSPSK,
                                                           SHIP.BBSN,
                                                           SHIP.SPCKEY,
                                                           SHIP.RESET_CODE,
                                                           PRO.LINK_QTY AS BOARDNO,
                                                           '' AS A4,
                                                           TO_CHAR (PR.IN_STATION_TIME, 'YYYYMMDDHHMMSS') AS BAG_TIME,
                                                           TO_CHAR (PB.IN_STATION_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                                           TO_CHAR (FT.IN_STATION_TIME, 'YYYYMMDD') AS FT_DATE,
                                                           RB.INVOICE DN_NO,
                                                           CONCAT (R117.YEA, R117.MONT) AS PRODUCT_DC,
                                                           RB.DNP,
                                                          -- TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS FT_DATE,
                                                           RMB.DEFAULT_LINE AS TEST_LINE,
                                                           RB.SO_NUMBER AS PO_NUMBER,
                                                           LPAD (
                                                              ROUND (
                                                                 ( (PB.IN_STATION_TIME - RO.IN_STATION_TIME) * 24 * 60 * 60),
                                                                 0),
                                                              9,
                                                              0)
                                                              AS FLOOR_TIME,
                                                           CC.DATA3 AS INFO04,
                                                           CC.DATA4 AS INFO05,
                                                           CC.DATA2 AS EAN,
                                                           CC.INFO06,
                                                           CC.INFO01,
                                                           CC.DATA1 || '-' || CC.HW AS PART_NUMBER_SHIELD,
                                                           CC.DIVICE_NUMBER || '-' || SUBSTR (RMB.HW_BOM, 1, 2)
                                                              AS CUST_MODEL_NAME,
                                                           RMB.HW_BOM AS REVISION,
                                                           TRSN.PANEL_NO,
                                                           RMB.SW_BOM AS SW,
                                                           TO_CHAR (ADD_MONTHS (RB.FINISH_DATE, 12), 'YYYYMMDD')
                                                              AS WARRANTY_EXPIRED_DATE,
                                                           TO_CHAR (RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,
                                                           TO_CHAR (RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
                                                           TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS PRODUCT_DATE,
                                                           TO_CHAR (RMB.MO_KP_START_DATE, 'HH24MISS') AS PRODUCT_TIME,
                                                           CC.DATA1 || '-' || SUBSTR (RMB.HW_BOM, 1, 2) AS PART_NUMBER_CUSTOMER,
                                                           '' AS A14,
                                                           '' AS A15,
                                                           --TO_CHAR (Z.IN_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                                           CC.CUST_MODEL_NAME PRODUCT_TYPE,
                                                           CC.CUST_MODEL_NAME INF02,
                                                           'FOV' AS PRODUCT_LOCATION,
                                                           '' AS A40
                                                      FROM (SELECT FINISH_DATE,
                                                                   SO_NUMBER,
                                                                   TCOM,
                                                                   INVOICE,
                                                                   INV_NO,
                                                                   MODEL_NAME,
                                                                   ROWNUM * 10 AS DNP
                                                              FROM SFISM4.R_BPCS_INVOICE_T
                                                             WHERE INVOICE = '{0}' AND ROWNUM = 1) RB,
                                                           SFISM4.Z_WIP_TRACKING_T Z,
                                                           (  SELECT DISTINCT C.SERIAL_NUMBER,
                                                                     CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'MM')
                                                                        WHEN '01' THEN '1'
                                                                        WHEN '02' THEN '2'
                                                                        WHEN '03' THEN '3'
                                                                        WHEN '04' THEN '4'
                                                                        WHEN '05' THEN '5'
                                                                        WHEN '06' THEN '6'
                                                                        WHEN '07' THEN '7'
                                                                        WHEN '08' THEN '8'
                                                                        WHEN '09' THEN '9'
                                                                        WHEN '10' THEN 'O'
                                                                        WHEN '11' THEN 'N'
                                                                        WHEN '12' THEN 'D'
                                                                     END
                                                                        AS MONT,
                                                                     CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'YYYY')
                                                                        WHEN '2020' THEN 'M'
                                                                        WHEN '2021' THEN 'N'
                                                                        WHEN '2022' THEN 'P'
                                                                        WHEN '2023' THEN 'R'
                                                                        WHEN '2024' THEN 'S'
                                                                        WHEN '2025' THEN 'T'
                                                                        WHEN '2026' THEN 'U'
                                                                        WHEN '2027' THEN 'V'
                                                                        WHEN '2028' THEN 'W'
                                                                        WHEN '2029' THEN 'X'
                                                                     END
                                                                        AS YEA
                                                                FROM SFISM4.Z_WIP_TRACKING_T A,
                                                                     SFISM4.R_BPCS_INVOICE_T B,
                                                                     SFISM4.R_SN_DETAIL_T C
                                                               WHERE     A.SHIP_NO = B.TCOM
                                                                     AND B.INVOICE = '{0}'
                                                                     AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                                            GROUP BY C.SERIAL_NUMBER) R117,
                                                           (SELECT *
                                                              FROM SFIS1.C_CINTERION_SHIP_T) CC,
                                                           (SELECT *
                                                              FROM SFISM4.R_MO_BASE_T
                                                             WHERE MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T))
                                                           RMB,
                                                           SFISM4.R_SN_TRSN_LINK_T TRSN,
                                                           SFISM4.R117 PR,
                                                           SFISM4.R117 PB,
                                                           SFISM4.R117 FT,
                                                           SFISM4.R_CUSTSN_T CUST,
                                                           SFISM4.R117 RO,
                                                           SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                                           SFISM4.R_CINTERION_TESTLOG_T SHIP,
                                                           mes1.c_product_config@GZF12AP pro,
                                                           SFIS1.C_PO_CONFIG_T PO
                                                     WHERE     RB.TCOM = Z.SHIP_NO
                                                           AND SAPDN.DN_NO = RB.INVOICE
                                                           AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+)
                                                           AND Z.MODEL_NAME = CC.MODEL_NAME
                                                           AND Z.MO_NUMBER = RMB.MO_NUMBER
                                                           AND Z.SERIAL_NUMBER = TRSN.SERIAL_NUMBER
                                                           AND PR.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND PR.GROUP_NAME = 'PRINT'
                                                           AND PR.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                          AND GROUP_NAME = 'PRINT')
                                                           AND PB.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND PB.GROUP_NAME = 'CHECK_MSL'
                                                           AND PB.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                          AND GROUP_NAME = 'CHECK_MSL')
                                                           AND RO.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND RO.GROUP_NAME = 'ROAST_OUT'
                                                           AND RO.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                          AND GROUP_NAME = 'ROAST_OUT')
                                                           AND FT.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND FT.GROUP_NAME = 'FT'
                                                           AND FT.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE SERIAL_NUMBER = Z.SERIAL_NUMBER AND GROUP_NAME = 'FT')
                                                           AND CUST.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND SHIP.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND PRO.P_NO = Z.MODEL_NAME
                                                           AND PO.MODEL_NAME = SAPDN.DN_NO", dn);

                ///PLS63-W-R2.1
                //string sql_pls63_w_r21 = string.Format(@"SELECT distinct Z.SERIAL_NUMBER,
                //                                           SAPDN.CUST_PO||'-'||PO.TYPE PO_NO,
                //                                           Z.IMEI,
                //                                           Z.TRAY_NO,
                //                                           Z.MCARTON_NO,
                //                                           Z.MODEL_NAME AS MATNOFACTORY,
                //                                           SHIP.ACCKEY AS ACCKEY,
                //                                           SHIP.FLASH_KEY,
                //                                           TO_CHAR(SHIP.TEST_TIME, 'YYYYMMDDHHMMSS') AS TEST_TIME,
                //                                           TO_CHAR(SHIP.TRANSFER_TIME, 'YYYYMMDDHHMMSS') AS TRANSFER_TIME,
                //                                           SHIP.CERTIFICATE_ID AS CERTIFICATE_ID,
                //                                           SHIP.CERTIFICATE_PROFILE AS CERTIFICATE_PROFILE,
                //                                           SHIP.PWFACTORY,
                //                                           SHIP.PWDEBUG,
                //                                           SHIP.PWPRETLS,
                //                                           SHIP.BBSN,
                //                                           SHIP.SPCKEY,
                //                                           SHIP.RESET_CODE,
                //                                           PRO.LINK_QTY AS BOARDNO,
                //                                           '' AS A4,
                //                                           TO_CHAR (PR.IN_STATION_TIME, 'YYYYMMDDHHMMSS') AS BAG_TIME,
                //                                           TO_CHAR (PB.IN_STATION_TIME, 'YYYYMMDD') AS PACKING_DATE,
                //                                           TO_CHAR (FT.IN_STATION_TIME, 'YYYYMMDD') AS FT_DATE,
                //                                           RB.INVOICE DN_NO,
                //                                           CONCAT (R117.YEA, R117.MONT) AS PRODUCT_DC,
                //                                           RB.DNP,
                //                                          -- TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS FT_DATE,
                //                                           RMB.DEFAULT_LINE AS TEST_LINE,
                //                                           RB.SO_NUMBER AS PO_NUMBER,
                //                                           LPAD (
                //                                              ROUND (
                //                                                 ( (PB.IN_STATION_TIME - RO.IN_STATION_TIME) * 24 * 60 * 60),
                //                                                 0),
                //                                              9,
                //                                              0)
                //                                              AS FLOOR_TIME,
                //                                           CC.DATA3 AS INFO04,
                //                                           CC.DATA4 AS INFO05,
                //                                           CC.DATA2 AS EAN,
                //                                           CC.INFO06,
                //                                           CC.INFO01,
                //                                           CC.DATA1 || '-' || CC.HW AS PART_NUMBER_SHIELD,
                //                                           CC.DIVICE_NUMBER || '-' || SUBSTR (RMB.HW_BOM, 1, 2)
                //                                              AS CUST_MODEL_NAME,
                //                                           RMB.HW_BOM AS REVISION,
                //                                           TRSN.PANEL_NO,
                //                                           RMB.SW_BOM AS SW,
                //                                           TO_CHAR (ADD_MONTHS (RB.FINISH_DATE, 12), 'YYYYMMDD')
                //                                              AS WARRANTY_EXPIRED_DATE,
                //                                           TO_CHAR (RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,
                //                                           TO_CHAR (RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
                //                                           TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS PRODUCT_DATE,
                //                                           TO_CHAR (RMB.MO_KP_START_DATE, 'HH24MISS') AS PRODUCT_TIME,
                //                                           CC.DATA1 || '-' || SUBSTR (RMB.HW_BOM, 1, 2) AS PART_NUMBER_CUSTOMER,
                //                                           '' AS A14,
                //                                           '' AS A15,
                //                                           --TO_CHAR (Z.IN_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,
                //                                           CC.CUST_MODEL_NAME PRODUCT_TYPE,
                //                                           CC.CUST_MODEL_NAME INF02,
                //                                           'FOV' AS PRODUCT_LOCATION,
                //                                           '' AS A40
                //                                      FROM (SELECT FINISH_DATE,
                //                                                   SO_NUMBER,
                //                                                   TCOM,
                //                                                   INVOICE,
                //                                                   INV_NO,
                //                                                   MODEL_NAME,
                //                                                   ROWNUM * 10 AS DNP
                //                                              FROM SFISM4.R_BPCS_INVOICE_T
                //                                             WHERE INVOICE = '{0}' AND ROWNUM = 1) RB,
                //                                           SFISM4.Z_WIP_TRACKING_T Z,
                //                                           (  SELECT DISTINCT C.SERIAL_NUMBER,
                //                                                     CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'MM')
                //                                                        WHEN '01' THEN '1'
                //                                                        WHEN '02' THEN '2'
                //                                                        WHEN '03' THEN '3'
                //                                                        WHEN '04' THEN '4'
                //                                                        WHEN '05' THEN '5'
                //                                                        WHEN '06' THEN '6'
                //                                                        WHEN '07' THEN '7'
                //                                                        WHEN '08' THEN '8'
                //                                                        WHEN '09' THEN '9'
                //                                                        WHEN '10' THEN 'O'
                //                                                        WHEN '11' THEN 'N'
                //                                                        WHEN '12' THEN 'D'
                //                                                     END
                //                                                        AS MONT,
                //                                                     CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'YYYY')
                //                                                        WHEN '2020' THEN 'M'
                //                                                        WHEN '2021' THEN 'N'
                //                                                        WHEN '2022' THEN 'P'
                //                                                        WHEN '2023' THEN 'R'
                //                                                        WHEN '2024' THEN 'S'
                //                                                        WHEN '2025' THEN 'T'
                //                                                        WHEN '2026' THEN 'U'
                //                                                        WHEN '2027' THEN 'V'
                //                                                        WHEN '2028' THEN 'W'
                //                                                        WHEN '2029' THEN 'X'
                //                                                     END
                //                                                        AS YEA
                //                                                FROM SFISM4.Z_WIP_TRACKING_T A,
                //                                                     SFISM4.R_BPCS_INVOICE_T B,
                //                                                     SFISM4.R_SN_DETAIL_T C
                //                                               WHERE     A.SHIP_NO = B.TCOM
                //                                                     AND B.INVOICE = '{0}'
                //                                                     AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                //                                            GROUP BY C.SERIAL_NUMBER) R117,
                //                                           (SELECT *
                //                                              FROM SFIS1.C_CINTERION_SHIP_T) CC,
                //                                           (SELECT *
                //                                              FROM SFISM4.R_MO_BASE_T
                //                                             WHERE MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T))
                //                                           RMB,
                //                                           SFISM4.R_SN_TRSN_LINK_T TRSN,
                //                                           SFISM4.R117 PR,
                //                                           SFISM4.R117 PB,
                //                                           SFISM4.R117 FT,
                //                                           SFISM4.R_CUSTSN_T CUST,
                //                                           SFISM4.R117 RO,
                //                                           SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                //                                           SFISM4.R_CINTERION_TESTLOG_T SHIP,
                //                                           mes1.c_product_config@GZF12AP pro,
                //                                           SFIS1.C_PO_CONFIG_T PO
                //                                     WHERE     RB.TCOM = Z.SHIP_NO
                //                                           AND SAPDN.DN_NO = RB.INVOICE
                //                                           AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+)
                //                                           AND Z.MODEL_NAME = CC.MODEL_NAME
                //                                           AND Z.MO_NUMBER = RMB.MO_NUMBER
                //                                           AND Z.SERIAL_NUMBER = TRSN.SERIAL_NUMBER
                //                                           AND PR.SERIAL_NUMBER = Z.SERIAL_NUMBER
                //                                           AND PR.GROUP_NAME = 'PRINT'
                //                                           AND PR.IN_STATION_TIME =
                //                                                  (SELECT MAX (IN_STATION_TIME)
                //                                                     FROM SFISM4.R117
                //                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                //                                                          AND GROUP_NAME = 'PRINT')
                //                                           AND PB.SERIAL_NUMBER = Z.SERIAL_NUMBER
                //                                           AND PB.GROUP_NAME = 'CHECK_MSL'
                //                                           AND PB.IN_STATION_TIME =
                //                                                  (SELECT MAX (IN_STATION_TIME)
                //                                                     FROM SFISM4.R117
                //                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                //                                                          AND GROUP_NAME = 'CHECK_MSL')
                //                                           AND RO.SERIAL_NUMBER = Z.SERIAL_NUMBER
                //                                           AND RO.GROUP_NAME = 'ROAST_OUT'
                //                                           AND RO.IN_STATION_TIME =
                //                                                  (SELECT MAX (IN_STATION_TIME)
                //                                                     FROM SFISM4.R117
                //                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                //                                                          AND GROUP_NAME = 'ROAST_OUT')
                //                                           AND FT.SERIAL_NUMBER = Z.SERIAL_NUMBER
                //                                           AND FT.GROUP_NAME = 'FT'
                //                                           AND FT.IN_STATION_TIME =
                //                                                  (SELECT MAX (IN_STATION_TIME)
                //                                                     FROM SFISM4.R117
                //                                                    WHERE SERIAL_NUMBER = Z.SERIAL_NUMBER AND GROUP_NAME = 'FT')
                //                                           AND CUST.SERIAL_NUMBER = Z.SERIAL_NUMBER
                //                                           AND SHIP.SERIAL_NUMBER = Z.SERIAL_NUMBER
                //                                           AND PRO.P_NO = Z.MODEL_NAME
                //                                           AND PO.MODEL_NAME = SAPDN.DN_NO", dn);




                string sqlT99w56502 = string.Format(@"SELECT distinct SUBSTR(Z.SERIAL_NUMBER,1, INSTR(Z.SERIAL_NUMBER, ';') - 1) as SERIAL_NUMBER,
                                                           SAPDN.CUST_PO||'-'|| PO.TYPE PO_NO,
                                                           Z.IMEI,
                                                           Z.TRAY_NO,
                                                           Z.MCARTON_NO,
                                                           Z.MODEL_NAME AS MATNOFACTORY,
                                                           PRO.LINK_QTY AS BOARDNO,
                                                           '' AS A4,
                                                           TO_CHAR (PR.IN_STATION_TIME, 'YYYYMMDDHHMMSS') AS BAG_TIME,
                                                           TO_CHAR (PB.IN_STATION_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                                           TO_CHAR (FT.IN_STATION_TIME, 'YYYYMMDD') AS FT_DATE,
                                                           RB.INVOICE DN_NO,
                                                           CONCAT (R117.YEA, R117.MONT) AS PRODUCT_DC,
                                                           RB.DNP,
                                                          -- TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS FT_DATE,
                                                           RMB.DEFAULT_LINE AS TEST_LINE,
                                                           RB.SO_NUMBER AS PO_NUMBER,
                                                           LPAD (
                                                              ROUND (
                                                                 ( (PB.IN_STATION_TIME - RO.IN_STATION_TIME) * 24 * 60 * 60),
                                                                 0),
                                                              9,
                                                              0)
                                                              AS FLOOR_TIME,
                                                           CC.DATA3 AS INFO04,
                                                           CC.DATA4 AS INFO05,
                                                           CC.DATA2 AS EAN,
                                                           CC.INFO06,
                                                           CC.INFO01,
                                                           CC.DIVICE_NUMBER   AS CUST_MODEL_NAME,
                                                           TRSN.PANEL_NO,
                                                           RMB.SW_BOM AS SW,
                                                           TO_CHAR (ADD_MONTHS (RB.FINISH_DATE, 12), 'YYYYMMDD')
                                                              AS WARRANTY_EXPIRED_DATE,
                                                           TO_CHAR (RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,
                                                           TO_CHAR (RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
                                                           TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS PRODUCT_DATE,
                                                           TO_CHAR (RMB.MO_KP_START_DATE, 'HH24MISS') AS PRODUCT_TIME,
                                                           '' AS A14,
                                                           '' AS A15,
                                                           --TO_CHAR (Z.IN_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                                           CC.CUST_MODEL_NAME PRODUCT_TYPE,
                                                           CC.CUST_MODEL_NAME INF02,
                                                           'FOV' AS PRODUCT_LOCATION,
                                                           '' AS A40
                                                      FROM (SELECT FINISH_DATE,
                                                                   SO_NUMBER,
                                                                   TCOM,
                                                                   INVOICE,
                                                                   INV_NO,
                                                                   MODEL_NAME,
                                                                   ROWNUM * 10 AS DNP
                                                              FROM SFISM4.R_BPCS_INVOICE_T
                                                             WHERE INVOICE = '{0}' AND ROWNUM = 1) RB,
                                                           SFISM4.Z_WIP_TRACKING_T Z,
                                                           (  SELECT DISTINCT C.SERIAL_NUMBER,
                                                                     CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'MM')
                                                                        WHEN '01' THEN '1'
                                                                        WHEN '02' THEN '2'
                                                                        WHEN '03' THEN '3'
                                                                        WHEN '04' THEN '4'
                                                                        WHEN '05' THEN '5'
                                                                        WHEN '06' THEN '6'
                                                                        WHEN '07' THEN '7'
                                                                        WHEN '08' THEN '8'
                                                                        WHEN '09' THEN '9'
                                                                        WHEN '10' THEN 'O'
                                                                        WHEN '11' THEN 'N'
                                                                        WHEN '12' THEN 'D'
                                                                     END
                                                                        AS MONT,
                                                                     CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'YYYY')
                                                                        WHEN '2020' THEN 'M'
                                                                        WHEN '2021' THEN 'N'
                                                                        WHEN '2022' THEN 'P'
                                                                        WHEN '2023' THEN 'R'
                                                                        WHEN '2024' THEN 'S'
                                                                        WHEN '2025' THEN 'T'
                                                                        WHEN '2026' THEN 'U'
                                                                        WHEN '2027' THEN 'V'
                                                                        WHEN '2028' THEN 'W'
                                                                        WHEN '2029' THEN 'X'
                                                                     END
                                                                        AS YEA
                                                                FROM SFISM4.Z_WIP_TRACKING_T A,
                                                                     SFISM4.R_BPCS_INVOICE_T B,
                                                                     SFISM4.R_SN_DETAIL_T C
                                                               WHERE     A.SHIP_NO = B.TCOM
                                                                     AND B.INVOICE = '{0}'
                                                                     AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                                            GROUP BY C.SERIAL_NUMBER) R117,
                                                           (SELECT *
                                                              FROM SFIS1.C_CINTERION_SHIP_T) CC,
                                                           (SELECT *
                                                              FROM SFISM4.R_MO_BASE_T
                                                             WHERE MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T))
                                                           RMB,
                                                           SFISM4.R_SN_TRSN_LINK_T TRSN,
                                                           SFISM4.R117 PR,
                                                           SFISM4.R117 PB,
                                                           SFISM4.R117 FT,
                                                           SFISM4.R_CUSTSN_T CUST,
                                                           SFISM4.R117 RO,
                                                           SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                                           mes1.c_product_config@GZF12AP pro,
                                                           SFIS1.C_PO_CONFIG_T PO
                                                     WHERE     RB.TCOM = Z.SHIP_NO
                                                           AND SAPDN.DN_NO = RB.INVOICE
                                                           AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+)
                                                           AND Z.MODEL_NAME = CC.MODEL_NAME
                                                           AND Z.MO_NUMBER = RMB.MO_NUMBER
                                                           AND Z.SERIAL_NUMBER = TRSN.SERIAL_NUMBER
                                                           AND PR.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND PR.GROUP_NAME = 'PRINT'
                                                           AND PR.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                          AND GROUP_NAME = 'PRINT')
                                                           AND PB.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND PB.GROUP_NAME = 'CHECK_MSL'
                                                           AND PB.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                          AND GROUP_NAME = 'CHECK_MSL')
                                                           AND RO.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND RO.GROUP_NAME = 'ROAST_OUT'
                                                           AND RO.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                          AND GROUP_NAME = 'ROAST_OUT')
                                                           AND FT.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND FT.GROUP_NAME = 'FT'
                                                           AND FT.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE SERIAL_NUMBER = Z.SERIAL_NUMBER AND GROUP_NAME = 'FT')
                                                           AND CUST.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND PRO.P_NO = Z.MODEL_NAME
                                                           AND PO.PO_NO = SAPDN.CUST_PO
                                                            AND SAPDN.DN_NO = PO.MODEL_NAME", dn);

                string sqlTN23 = string.Format(@"SELECT distinct Z.SERIAL_NUMBER,
                                                           SAPDN.CUST_PO||'-'||PO.TYPE PO_NO,
                                                           Z.IMEI,
                                                           Z.TRAY_NO,
                                                           Z.MCARTON_NO,
                                                           Z.MODEL_NAME AS MATNOFACTORY,
                                                           SHIP.ACCKEY AS ACCKEY,
                                                           SHIP.FLASH_KEY,
                                                           TO_CHAR(SHIP.TEST_TIME, 'YYYYMMDDHHMMSS') AS TEST_TIME,
                                                           TO_CHAR(SHIP.TRANSFER_TIME, 'YYYYMMDDHHMMSS') AS TRANSFER_TIME,
                                                           SHIP.CERTIFICATE_ID AS CERTIFICATE_ID,
                                                           SHIP.CERTIFICATE_PROFILE AS CERTIFICATE_PROFILE,
                                                           SHIP.PWFACTORY,
                                                           SHIP.PWDEBUG,
                                                           SHIP.PWPRETLS,
                                                           SHIP.DTLSPSK,
                                                           SHIP.BBSN,
                                                           SHIP.SPCKEY,
                                                           SHIP.RESET_CODE,
                                                           SHIP.IMSI,
                                                           PRO.LINK_QTY AS BOARDNO,
                                                           '' AS A4,
                                                           TO_CHAR (PR.IN_STATION_TIME, 'YYYYMMDDHHMMSS') AS BAG_TIME,
                                                           TO_CHAR (PB.IN_STATION_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                                           TO_CHAR (FT.IN_STATION_TIME, 'YYYYMMDD') AS FT_DATE,
                                                           RB.INVOICE DN_NO,
                                                           CONCAT (R117.YEA, R117.MONT) AS PRODUCT_DC,
                                                           RB.DNP,
                                                          -- TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS FT_DATE,
                                                           RMB.DEFAULT_LINE AS TEST_LINE,
                                                           RB.SO_NUMBER AS PO_NUMBER,
                                                           LPAD (
                                                              ROUND (
                                                                 ( (PB.IN_STATION_TIME - RO.IN_STATION_TIME) * 24 * 60 * 60),
                                                                 0),
                                                              9,
                                                              0)
                                                              AS FLOOR_TIME,
                                                           CC.DATA3 AS INFO04,
                                                           CC.DATA4 AS INFO05,
                                                           CC.DATA2 AS EAN,
                                                           CC.INFO06,
                                                           CC.INFO01,
                                                           CC.DATA1 || '-' || CC.HW AS PART_NUMBER_SHIELD,
                                                           CC.DIVICE_NUMBER || '-' || SUBSTR (RMB.HW_BOM, 1, 2)
                                                              AS CUST_MODEL_NAME,
                                                           RMB.HW_BOM AS REVISION,
                                                           TRSN.PANEL_NO,
                                                           RMB.SW_BOM AS SW,
                                                           TO_CHAR (ADD_MONTHS (RB.FINISH_DATE, 12), 'YYYYMMDD')
                                                              AS WARRANTY_EXPIRED_DATE,
                                                           TO_CHAR (RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,
                                                           TO_CHAR (RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
                                                           TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS PRODUCT_DATE,
                                                           TO_CHAR (RMB.MO_KP_START_DATE, 'HH24MISS') AS PRODUCT_TIME,
                                                           CC.DATA1 || '-' || SUBSTR (RMB.HW_BOM, 1, 2) AS PART_NUMBER_CUSTOMER,
                                                           '' AS A14,
                                                           '' AS A15,
                                                           --TO_CHAR (Z.IN_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                                           CC.CUST_MODEL_NAME PRODUCT_TYPE,
                                                           CC.CUST_MODEL_NAME INF02,
                                                           'FOV' AS PRODUCT_LOCATION,
                                                           '' AS A40
                                                      FROM (SELECT FINISH_DATE,
                                                                   SO_NUMBER,
                                                                   TCOM,
                                                                   INVOICE,
                                                                   INV_NO,
                                                                   MODEL_NAME,
                                                                   ROWNUM * 10 AS DNP
                                                              FROM SFISM4.R_BPCS_INVOICE_T
                                                             WHERE INVOICE = '{0}' AND ROWNUM = 1) RB,
                                                           SFISM4.Z_WIP_TRACKING_T Z,
                                                           (  SELECT DISTINCT C.SERIAL_NUMBER,
                                                                     CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'MM')
                                                                        WHEN '01' THEN '1'
                                                                        WHEN '02' THEN '2'
                                                                        WHEN '03' THEN '3'
                                                                        WHEN '04' THEN '4'
                                                                        WHEN '05' THEN '5'
                                                                        WHEN '06' THEN '6'
                                                                        WHEN '07' THEN '7'
                                                                        WHEN '08' THEN '8'
                                                                        WHEN '09' THEN '9'
                                                                        WHEN '10' THEN 'O'
                                                                        WHEN '11' THEN 'N'
                                                                        WHEN '12' THEN 'D'
                                                                     END
                                                                        AS MONT,
                                                                     CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'YYYY')
                                                                        WHEN '2020' THEN 'M'
                                                                        WHEN '2021' THEN 'N'
                                                                        WHEN '2022' THEN 'P'
                                                                        WHEN '2023' THEN 'R'
                                                                        WHEN '2024' THEN 'S'
                                                                        WHEN '2025' THEN 'T'
                                                                        WHEN '2026' THEN 'U'
                                                                        WHEN '2027' THEN 'V'
                                                                        WHEN '2028' THEN 'W'
                                                                        WHEN '2029' THEN 'X'
                                                                     END
                                                                        AS YEA
                                                                FROM SFISM4.Z_WIP_TRACKING_T A,
                                                                     SFISM4.R_BPCS_INVOICE_T B,
                                                                     SFISM4.R_SN_DETAIL_T C
                                                               WHERE     A.SHIP_NO = B.TCOM
                                                                     AND B.INVOICE = '{0}'
                                                                     AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                                            GROUP BY C.SERIAL_NUMBER) R117,
                                                           (SELECT *
                                                              FROM SFIS1.C_CINTERION_SHIP_T) CC,
                                                           (SELECT *
                                                              FROM SFISM4.R_MO_BASE_T
                                                             WHERE MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T))
                                                           RMB,
                                                           SFISM4.R_SN_TRSN_LINK_T TRSN,
                                                           SFISM4.R117 PR,
                                                           SFISM4.R117 PB,
                                                           SFISM4.R117 FT,
                                                           SFISM4.R_CUSTSN_T CUST,
                                                           SFISM4.R117 RO,
                                                           SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                                           SFISM4.R_CINTERION_TESTLOG_T SHIP,
                                                           mes1.c_product_config@GZF12AP pro,
                                                           SFIS1.C_PO_CONFIG_T PO
                                                     WHERE     RB.TCOM = Z.SHIP_NO
                                                           AND SAPDN.DN_NO = RB.INVOICE
                                                           AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+)
                                                           AND Z.MODEL_NAME = CC.MODEL_NAME
                                                           AND Z.MO_NUMBER = RMB.MO_NUMBER
                                                           AND Z.SERIAL_NUMBER = TRSN.SERIAL_NUMBER
                                                           AND PR.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND PR.GROUP_NAME = 'PRINT'
                                                           AND PR.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                          AND GROUP_NAME = 'PRINT')
                                                           AND PB.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND PB.GROUP_NAME = 'CHECK_MSL'
                                                           AND PB.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                          AND GROUP_NAME = 'CHECK_MSL')
                                                           AND RO.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND RO.GROUP_NAME = 'ROAST_OUT'
                                                           AND RO.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE     SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                          AND GROUP_NAME = 'ROAST_OUT')
                                                           AND FT.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND FT.GROUP_NAME = 'FT'
                                                           AND FT.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE SERIAL_NUMBER = Z.SERIAL_NUMBER AND GROUP_NAME = 'FT')
                                                           AND CUST.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND SHIP.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND PRO.P_NO = Z.MODEL_NAME
                                                           AND PO.MODEL_NAME = SAPDN.DN_NO", dn);

                string sql = string.Format(@"SELECT distinct Z.SERIAL_NUMBER,
                                               Z.IMEI,
                                               Z.MCARTON_NO,
                                               Z.MODEL_NAME AS MATNOFACTORY,
                                               '' AS A4,
                                               RB.INVOICE DN_NO,
                                               RB.DNP,
                                               PO.PO_NO,
                                               RCR.PRODUCT_DATE,
                                               RCR.PRODUCT_TIME,
                                               RCR.PRODUCT_DC,
                                               RCR.FT_DATE,
                                               RCR.PANEL_NO,
                                               RCR.TEST_LINE,
                                               CC.DATA2 AS EAN,
                                               CC.INFO06,
                                               CC.DATA1 || '-' || CC.HW AS PART_NUMBER_SHIELD,
                                               CC.DIVICE_NUMBER || '-' || SUBSTR (RMB.HW_BOM, 1, 2)
                                                  AS CUST_MODEL_NAME,
                                               RMB.HW_BOM AS REVISION,
                                               RMB.SW_BOM AS SW,
                                               TO_CHAR (ADD_MONTHS (RB.FINISH_DATE, 12), 'YYYYMMDD')
                                                  AS WARRANTY_EXPIRED_DATE,
                                               TO_CHAR (RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,
                                               TO_CHAR (RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
                                               TO_CHAR (R117.IN_LINE_TIME, 'YYYYMMDD') AS PRODUCT_DATE,
                                               TO_CHAR (R117.IN_LINE_TIME, 'HH24MISS') AS PRODUCT_TIME,
                                               CC.DATA1 || '-' || SUBSTR (RMB.HW_BOM, 1, 2) AS PART_NUMBER_CUSTOMER,
                                               '' AS A14,
                                               '' AS A15,
                                               TO_CHAR (Z.OUT_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                               CC.CUST_MODEL_NAME PRODUCT_TYPE,
                                               CC.CUST_MODEL_NAME INF02,
                                               '' AS A18,
                                               '' AS A19,
                                               '' AS A20,
                                               '' AS A21,
                                               '' AS A22,
                                               '' AS A23,
                                               '' AS A24,
                                               '' AS A25,
                                               '' AS A26,
                                               '' AS A27,
                                               '' AS A28,
                                               '' AS A29,
                                               '' AS A30,
                                               '' AS A31,
                                               '' AS A32,
                                               '' AS A33,
                                               '' AS A34,
                                               '' AS A35,
                                               '' AS A36,
                                               '' AS A37,
                                               'FOX' AS PRODUCT_LOCATION,
                                               DECODE(CSNMAC.CUST_SN,NULL,'N/A') AS CUID,
                                               '' AS A40,
                                               '' AS A41,
                                               '' AS A42,
                                               '' AS A43,
                                               '' AS A44,
                                               '' AS A45,
                                               '' AS A46,
                                               '' AS A47,
                                               '' AS A48,
                                               '' AS A49,
                                               '' AS A50,
                                               '' AS A51,
                                               '' AS A52,
                                               '' AS A53,
                                               '' AS A54,
                                               '' AS A55,
                                               '' AS A56,
                                               '' AS A57,
                                               '' AS A58,
                                               '' AS A59,
                                               CSNMAC.UIM AS UIM,
                                               CSNMAC.ETHERNET_MAC AS MAC,
                                               CSNMAC.TELEPHONE_NUMBER AS MBSN,
                                               CSNMAC.IMEI AS MODULE_IMEI,
                                               CSNMAC.FW_VERSION AS FWVERSION
                                          FROM (SELECT FINISH_DATE,
                                                       TCOM,
                                                       INVOICE,
                                                       INV_NO,
                                                       MODEL_NAME,
                                                       ROWNUM * 10 AS DNP
                                                  FROM SFISM4.R_BPCS_INVOICE_T
                                                 WHERE INVOICE = '{0}' AND ROWNUM = 1) RB,
                                               SFISM4.Z_WIP_TRACKING_T Z,
                                               (  SELECT C.SERIAL_NUMBER, MIN (C.IN_LINE_TIME) IN_LINE_TIME
                                                    FROM SFISM4.Z_WIP_TRACKING_T A,
                                                         SFISM4.R_BPCS_INVOICE_T B,
                                                         SFISM4.R_SN_DETAIL_T C
                                                   WHERE     A.SHIP_NO = B.TCOM
                                                         AND B.INVOICE = '{0}'
                                                         AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                                GROUP BY C.SERIAL_NUMBER
                                                UNION
                                                  SELECT C.SERIAL_NUMBER, MIN (C.IN_LINE_TIME) IN_LINE_TIME
                                                    FROM SFISM4.Z_WIP_TRACKING_T A,
                                                         SFISM4.R_BPCS_INVOICE_T B,
                                                         SFISM4.H_SN_DETAIL_T C
                                                   WHERE     A.SHIP_NO = B.TCOM
                                                         AND B.INVOICE = '{0}'
                                                         AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                                GROUP BY C.SERIAL_NUMBER
                                                UNION
                                                  SELECT C.SERIAL_NUMBER, MIN (C.IN_LINE_TIME) IN_LINE_TIME
                                                    FROM SFISM4.Z_WIP_TRACKING_T A,
                                                         SFISM4.R_BPCS_INVOICE_T B,
                                                         SFISM4.H_SN_DETAIL_T C
                                                   WHERE     A.SHIP_NO = B.TCOM
                                                         AND B.INVOICE = '{0}'
                                                         AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                                GROUP BY C.SERIAL_NUMBER) R117,
                                               SFISM4.R_T77T943_CSNMAC_T CSNMAC,
                                               (SELECT MODEL_NAME,
                                                       DIVICE_NUMBER,
                                                       CUST_MODEL_NAME,
                                                       DATA1,
                                                       DATA2,
                                                       DATA3,
                                                       DATA4,
                                                       HW,
                                                       INFO06
                                                  FROM SFIS1.C_CINTERION_SHIP_T) CC,
                                               (SELECT MO_NUMBER,
                                                       SW_BOM,
                                                       HW_BOM,
                                                       OPTION_DESC
                                                  FROM SFISM4.R_MO_BASE_T
                                                 WHERE MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T))
                                               RMB, SFIS1.C_PO_CONFIG_T PO,
                                               SFISM4.R_CINTERION_REPORT_T RCR
                                         WHERE     RB.TCOM = Z.SHIP_NO
                                               AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+)
                                               AND Z.SERIAL_NUMBER = CSNMAC.SERIAL_NUMBER(+)
                                               AND Z.MODEL_NAME = CC.MODEL_NAME
                                               AND Z.MODEL_NAME = PO.MODEL_NAME
                                               AND Z.SERIAL_NUMBER = RCR.SERIAL_NUMBER
                                               AND Z.MO_NUMBER = RMB.MO_NUMBER", dn);

                string sqlMPLS83_W = string.Format(@"SELECT distinct substr(Z.SERIAL_NUMBER,2) SERIAL_NUMBER,
                                                           SAPDN.CUST_PO||'-'||PO.TYPE PO_NO,
                                                           Z.IMEI,
                                                           ''TRAY_NO,
                                                           Z.MCARTON_NO,
                                                           Z.MODEL_NAME AS MATNOFACTORY,
                                                           SHIP.ACCKEY AS ACCKEY,
                                                           SHIP.FLASH_KEY,
                                                           TO_CHAR(SHIP.TEST_TIME, 'YYYYMMDDHHMMSS') AS TEST_TIME,
                                                           TO_CHAR(SHIP.TRANSFER_TIME, 'YYYYMMDDHHMMSS') AS TRANSFER_TIME,
                                                           SHIP.CERTIFICATE_ID AS CERTIFICATE_ID,
                                                           SHIP.CERTIFICATE_PROFILE AS CERTIFICATE_PROFILE,
                                                           SHIP.PWFACTORY,
                                                           SHIP.PWDEBUG,
                                                           SHIP.PWPRETLS,
                                                           SHIP.DTLSPSK,
                                                           SHIP.BBSN,
                                                           SHIP.SPCKEY,
                                                           SHIP.RESET_CODE,
                                                           '' BOARDNO,
                                                           '' AS A4,
                                                           TO_CHAR (PR.IN_STATION_TIME, 'YYYYMMDDHHMMSS') AS BAG_TIME,
                                                           TO_CHAR (PACK.IN_STATION_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                                           TO_CHAR (FT.IN_STATION_TIME, 'YYYYMMDD') AS FT_DATE,
                                                           RB.INVOICE DN_NO,
                                                           CONCAT (R117.YEA, R117.MONT) AS PRODUCT_DC,
                                                           RB.DNP,
                                                           RMB.DEFAULT_LINE AS TEST_LINE,
                                                           RB.SO_NUMBER AS PO_NUMBER,
                                                           LPAD (
                                                              ROUND (
                                                                 ( (PB.IN_STATION_TIME - RO.IN_STATION_TIME) * 24 * 60 * 60),
                                                                 0),
                                                              9,
                                                              0)
                                                              AS FLOOR_TIME,
                                                           CC.DATA3 AS INFO04,
                                                           CC.DATA4 AS INFO05,
                                                           CC.DATA2 AS EAN,
                                                           CC.INFO06,
                                                           CC.INFO01,
                                                           CC.DATA1 || '-' || CC.HW AS PART_NUMBER_SHIELD,
                                                           CC.DIVICE_NUMBER || '-' || SUBSTR (RMB.HW_BOM, 1, 2)
                                                              AS CUST_MODEL_NAME,
                                                           RMB.HW_BOM AS REVISION,
                                                           TRSN.PANEL_NO,
                                                           RMB.SW_BOM AS SW,
                                                           TO_CHAR (ADD_MONTHS (RB.FINISH_DATE, 12), 'YYYYMMDD')
                                                              AS WARRANTY_EXPIRED_DATE,
                                                           TO_CHAR (RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,
                                                           TO_CHAR (RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
                                                           TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS PRODUCT_DATE,
                                                           TO_CHAR (RMB.MO_KP_START_DATE, 'HH24MISS') AS PRODUCT_TIME,
                                                           CC.DATA1 || '-' || SUBSTR (RMB.HW_BOM, 1, 2) AS PART_NUMBER_CUSTOMER,
                                                           '' AS A14,
                                                           '' AS A15,
                                                           CC.CUST_MODEL_NAME PRODUCT_TYPE,
                                                           CC.CUST_MODEL_NAME INF02,
                                                           'FOV' AS PRODUCT_LOCATION,
                                                           '' AS A40
                                                      FROM (SELECT FINISH_DATE,
                                                                   SO_NUMBER,
                                                                   TCOM,
                                                                   INVOICE,
                                                                   INV_NO,
                                                                   MODEL_NAME,
                                                                   ROWNUM * 10 AS DNP
                                                              FROM SFISM4.R_BPCS_INVOICE_T
                                                             WHERE INVOICE = '{0}' AND ROWNUM = 1) RB,
                                                           SFISM4.Z_WIP_TRACKING_T Z,
                                                           (  SELECT DISTINCT C.SERIAL_NUMBER,
                                                                     CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'MM')
                                                                        WHEN '01' THEN '1'
                                                                        WHEN '02' THEN '2'
                                                                        WHEN '03' THEN '3'
                                                                        WHEN '04' THEN '4'
                                                                        WHEN '05' THEN '5'
                                                                        WHEN '06' THEN '6'
                                                                        WHEN '07' THEN '7'
                                                                        WHEN '08' THEN '8'
                                                                        WHEN '09' THEN '9'
                                                                        WHEN '10' THEN 'O'
                                                                        WHEN '11' THEN 'N'
                                                                        WHEN '12' THEN 'D'
                                                                     END
                                                                        AS MONT,
                                                                     CASE TO_CHAR (MIN(C.IN_LINE_TIME), 'YYYY')
                                                                        WHEN '2020' THEN 'M'
                                                                        WHEN '2021' THEN 'N'
                                                                        WHEN '2022' THEN 'P'
                                                                        WHEN '2023' THEN 'R'
                                                                        WHEN '2024' THEN 'S'
                                                                        WHEN '2025' THEN 'T'
                                                                        WHEN '2026' THEN 'U'
                                                                        WHEN '2027' THEN 'V'
                                                                        WHEN '2028' THEN 'W'
                                                                        WHEN '2029' THEN 'X'
                                                                     END
                                                                        AS YEA
                                                                FROM SFISM4.Z_WIP_TRACKING_T A,
                                                                     SFISM4.R_BPCS_INVOICE_T B,
                                                                     SFISM4.R_SN_DETAIL_T C
                                                               WHERE     A.SHIP_NO = B.TCOM
                                                               AND B.INVOICE = '{0}'
                                                                     AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                                            GROUP BY C.SERIAL_NUMBER) R117,
                                                           (SELECT *
                                                              FROM SFIS1.C_CINTERION_SHIP_T) CC,
                                                           (SELECT *
                                                              FROM SFISM4.R_MO_BASE_T
                                                             WHERE MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T))
                                                           RMB,
                                                           SFISM4.R_SN_TRSN_LINK_T TRSN,
                                                           SFISM4.R117 PR,
                                                           SFISM4.R117 PB,
                                                           SFISM4.R117 FT,
                                                           SFISM4.R117 PACK,
                                                           SFISM4.R117 RO,
                                                           SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                                           SFISM4.R_CINTERION_TESTLOG_T SHIP,
                                                           mes1.c_product_config@GZF12AP pro,
                                                           SFIS1.C_PO_CONFIG_T PO
                                                     WHERE     RB.TCOM = Z.SHIP_NO
                                                           AND SAPDN.DN_NO = RB.INVOICE
                                                           AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+)
                                                           AND Z.MODEL_NAME = CC.MODEL_NAME
                                                           AND Z.MO_NUMBER = RMB.MO_NUMBER
                                                           AND Z.SERIAL_NUMBER = TRSN.SERIAL_NUMBER
                                                           AND PR.SERIAL_NUMBER = substr(Z.SERIAL_NUMBER,2)
                                                           AND PR.GROUP_NAME = 'PRINT'
                                                           AND PR.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE SERIAL_NUMBER = substr(Z.SERIAL_NUMBER,2)
                                                                          AND GROUP_NAME = 'PRINT')
                                                           AND PB.SERIAL_NUMBER = substr(Z.SERIAL_NUMBER,2)
                                                           AND PB.GROUP_NAME = 'CHECK_MSL'
                                                           AND PB.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE  SERIAL_NUMBER = substr(Z.SERIAL_NUMBER,2)
                                                                          AND GROUP_NAME = 'CHECK_MSL')
                                                           AND RO.SERIAL_NUMBER = substr(Z.SERIAL_NUMBER,2)
                                                           AND RO.GROUP_NAME = 'ROAST_OUT'
                                                           AND RO.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                    WHERE     SERIAL_NUMBER = substr(Z.SERIAL_NUMBER,2)
                                                                          AND GROUP_NAME = 'ROAST_OUT')
                                                           AND FT.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND FT.GROUP_NAME = 'FT1'
                                                           AND FT.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                     WHERE SERIAL_NUMBER = Z.SERIAL_NUMBER AND GROUP_NAME = 'FT1')
                                                           AND PACK.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND PACK.GROUP_NAME = 'PACK_CTN'
                                                           AND PACK.IN_STATION_TIME =
                                                                  (SELECT MAX (IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                     WHERE SERIAL_NUMBER = Z.SERIAL_NUMBER AND GROUP_NAME = 'PACK_CTN')
                                                           AND SHIP.SERIAL_NUMBER = substr(Z.SERIAL_NUMBER,2)
                                                           AND PRO.P_NO = Z.MODEL_NAME
                                                           AND PO.MODEL_NAME = SAPDN.DN_NO", dn);

                string sqlRattleSnake = string.Format(@"select 'FOV' AS PRODUCT_LOCATION,Z.serial_number,Z.serial_number MODULE_IMEI,Z.MODEL_NAME AS MATNOFACTORY,Z.IMEI,Z.MCARTON_NO,DECODE(Z.TRAY_NO,'N/A','',Z.TRAY_NO) TRAY_NO,
                                                        SAPDN.CUST_PO||'-'||PO.TYPE PO_NO,
                                                        BPCS.INVOICE DN_NO,RMB.HW_BOM AS REVISION,
                                                        TO_CHAR(BPCS.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,
                                                        TO_CHAR(ADD_MONTHS(BPCS.FINISH_DATE, 12), 'YYYYMMDD') AS WARRANTY_EXPIRED_DATE,
                                                        TO_CHAR(RMB.MO_KP_START_DATE, 'YYYYMMDD') AS PRODUCT_DATE,
                                                        TO_CHAR(RMB.MO_KP_START_DATE, 'HH24MISS') AS PRODUCT_TIME,
                                                        TEST_LINE, FT_DATE, PACKING_DATE,DECODE(upper(CUST.SSN5),'N/A','',CUST.SSN5) A59, PANEL_NO,
                                                        CONCAT(CASE TO_CHAR(Z.IN_LINE_TIME, 'YYYY')
                                                        WHEN '2020' THEN 'M' WHEN '2021' THEN 'N' WHEN '2022' THEN 'P' WHEN '2023' THEN 'R'
                                                        WHEN '2024' THEN 'S' WHEN '2025' THEN 'T' WHEN '2026' THEN 'U' WHEN '2027' THEN 'V'
                                                        WHEN '2028' THEN 'W' WHEN '2029' THEN 'X' END,
                                                        CASE TO_CHAR(Z.IN_LINE_TIME, 'MM')
                                                        WHEN '01' THEN '1' WHEN '02' THEN '2' WHEN '03' THEN '3' WHEN '04' THEN '4'
                                                        WHEN '05' THEN '5' WHEN '06' THEN '6' WHEN '07' THEN '7' WHEN '08' THEN '8'
                                                        WHEN '09' THEN '9' WHEN '10' THEN 'O' WHEN '11' THEN 'N' WHEN '12' THEN 'D' END) AS PRODUCT_DC,
                                                        CC.CUST_MODEL_NAME PRODUCT_TYPE,
                                                        CC.INFO02 INF02,
                                                        CC.DATA3 AS INFO04,
                                                        CC.DATA4 AS INFO05,
                                                        CC.DATA2 AS EAN,
                                                        CC.INFO06,
                                                        CC.INFO01,
                                                        CC.DATA1 || '-' || CC.HW AS PART_NUMBER_SHIELD,
                                                        CC.DIVICE_NUMBER || '-' || SUBSTR(RMB.HW_BOM, 1, 2) AS CUST_MODEL_NAME,
                                                        '' ACCKEY,''BAG_TIME,''SW,''PART_NUMBER_CUSTOMER,''DNP,REPLACE(substr(CC.INFO06,5,10),'|','.') as FWVERSION,Z.SERIAL_NUMBER AS QR_CODE
                                                        from SFISM4.R_BPCS_INVOICE_T BPCS ,
                                                        SFISM4.Z_WIP_TRACKING_T Z,
                                                        SFISM4.R_CUSTSN_T CUST,
                                                        SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                                        SFISM4.R_SN_TRSN_LINK_T SNLINK,
                                                        SFIS1.C_CINTERION_SHIP_T CC,
                                                        (SELECT * FROM SFISM4.R_MO_BASE_T WHERE MODEL_NAME IN(SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T))RMB,
                                                        (SELECT SERIAL_NUMBER, LINE_NAME TEST_LINE,TO_CHAR(IN_STATION_TIME, 'YYYYMMDD') FT_DATE FROM SFISM4.R117 WHERE(SERIAL_NUMBER, IN_STATION_TIME) in(
                                                          SELECT SERIAL_NUMBER, MAX(IN_STATION_TIME)  FROM SFISM4.R117 WHERE GROUP_NAME = 'FT' group by serial_number)) FT,
                                                        (SELECT SERIAL_NUMBER,TO_CHAR(MAX(IN_STATION_TIME), 'YYYYMMDD') PACKING_DATE FROM SFISM4.R117 WHERE GROUP_NAME = 'PACK_CTN' group by serial_number) PACK,
                                                           SFIS1.C_PO_CONFIG_T PO
                                                           where BPCS.invoice = '{0}'
                                                        and BPCS.TCOM = Z.SHIP_NO
                                                        AND BPCS.INVOICE = SAPDN.DN_NO
                                                        AND Z.MO_NUMBER = RMB.MO_NUMBER
                                                        AND Z.SERIAL_NUMBER = FT.serial_number
                                                        AND Z.SERIAL_NUMBER = PACK.serial_number
                                                        AND Z.SERIAL_NUMBER = CUST.serial_number
                                                        AND Z.SERIAL_NUMBER = SNLINK.serial_number
                                                        AND Z.MODEL_NAME = CC.MODEL_NAME
                                                        AND PO.MODEL_NAME = SAPDN.DN_NO", dn);

                string sql95 = string.Format(@"SELECT distinct Z.SERIAL_NUMBER,SAPDN.CUST_PO||PO.TYPE PO_NO,
                                                           Z.IMEI,
                                                           Z.TRAY_NO,
                                                           Z.MCARTON_NO,
                                                           Z.MODEL_NAME AS MATNOFACTORY,
                                                           PRO.LINK_QTY AS BOARDNO,
                                                           '' AS A4,
                                                           TO_CHAR(FT.IN_STATION_TIME, 'YYYYMMDD') AS FT_DATE,
                                                           RB.INVOICE DN_NO,
                                                           CONCAT(R117.YEA, R117.MONT) AS PRODUCT_DC,
                                                           RB.DNP,
                                                           RMB.DEFAULT_LINE AS TEST_LINE,
                                                           RB.SO_NUMBER AS PO_NUMBER,
                                                           CC.DATA3 AS INFO04,
                                                           CC.DATA4 AS INFO05,
                                                           CC.DATA2 AS EAN,
                                                           CC.INFO06,
                                                           CC.INFO01,
                                                           RMB.HW_BOM AS REVISION,
                                                           TRSN.PANEL_NO,
                                                           RMB.SW_BOM AS SW,
                                                           TO_CHAR(ADD_MONTHS(RB.FINISH_DATE, 12), 'YYYYMMDD')
                                                              AS WARRANTY_EXPIRED_DATE,
                                                           TO_CHAR(RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,
                                                           TO_CHAR(RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
                                                           TO_CHAR(RMB.MO_KP_START_DATE, 'YYYYMMDD') AS PRODUCT_DATE,
                                                           TO_CHAR(RMB.MO_KP_START_DATE, 'HH24MISS') AS PRODUCT_TIME,
                                                           '' AS A14,
                                                           '' AS A15,
                                                           CC.CUST_MODEL_NAME PRODUCT_TYPE,
                                                           CC.CUST_MODEL_NAME INF02,
                                                           'FOV' AS PRODUCT_LOCATION,
                                                           '' AS A40
                                                      FROM(SELECT FINISH_DATE,
                                                                   SO_NUMBER,
                                                                   TCOM,
                                                                   INVOICE,
                                                                   INV_NO,
                                                                   MODEL_NAME,
                                                                   ROWNUM * 10 AS DNP
                                                              FROM SFISM4.R_BPCS_INVOICE_T
                                                             WHERE INVOICE = '{0}' AND ROWNUM = 1) RB,
                                                           SFISM4.Z_WIP_TRACKING_T Z,
                                                           (SELECT DISTINCT C.SERIAL_NUMBER,
                                                                     CASE TO_CHAR(MIN(C.IN_LINE_TIME), 'MM')
                                                                        WHEN '01' THEN '1'
                                                                        WHEN '02' THEN '2'
                                                                        WHEN '03' THEN '3'
                                                                        WHEN '04' THEN '4'
                                                                        WHEN '05' THEN '5'
                                                                        WHEN '06' THEN '6'
                                                                        WHEN '07' THEN '7'
                                                                        WHEN '08' THEN '8'
                                                                        WHEN '09' THEN '9'
                                                                        WHEN '10' THEN 'O'
                                                                        WHEN '11' THEN 'N'
                                                                        WHEN '12' THEN 'D'
                                                                     END
                                                                        AS MONT,
                                                                     CASE TO_CHAR(MIN(C.IN_LINE_TIME), 'YYYY')
                                                                        WHEN '2020' THEN 'M'
                                                                        WHEN '2021' THEN 'N'
                                                                        WHEN '2022' THEN 'P'
                                                                        WHEN '2023' THEN 'R'
                                                                        WHEN '2024' THEN 'S'
                                                                        WHEN '2025' THEN 'T'
                                                                        WHEN '2026' THEN 'U'
                                                                        WHEN '2027' THEN 'V'
                                                                        WHEN '2028' THEN 'W'
                                                                        WHEN '2029' THEN 'X'
                                                                     END
                                                                        AS YEA
                                                                FROM SFISM4.Z_WIP_TRACKING_T A,
                                                                     SFISM4.R_BPCS_INVOICE_T B,
                                                                     SFISM4.R_SN_DETAIL_T C
                                                               WHERE     A.SHIP_NO = B.TCOM
                                                                     AND B.INVOICE = '{0}'
                                                                     AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                                            GROUP BY C.SERIAL_NUMBER) R117,
                                                           (SELECT *
                                                              FROM SFIS1.C_CINTERION_SHIP_T) CC,
                                                           (SELECT *
                                                              FROM SFISM4.R_MO_BASE_T
                                                             WHERE MODEL_NAME IN(SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T))
                                                           RMB,
                                                           SFISM4.R_SN_TRSN_LINK_T TRSN,
                                                           SFISM4.R117 FT,
                                                           SFISM4.R117 TE,
                                                           SFISM4.R_CUSTSN_T CUST,
                                                           SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                                           mes1.c_product_config @GZF12AP pro,
                                                           SFIS1.C_PO_CONFIG_T PO
                                                     WHERE RB.TCOM = Z.SHIP_NO
                                                           AND SAPDN.DN_NO = RB.INVOICE
                                                           AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+)
                                                           AND Z.MODEL_NAME = CC.MODEL_NAME
                                                           AND Z.MO_NUMBER = RMB.MO_NUMBER
                                                           AND Z.SERIAL_NUMBER = TRSN.SERIAL_NUMBER
                                                           AND Z.SERIAL_NUMBER = FT.SERIAL_NUMBER(+)
                                                           AND FT.GROUP_NAME = 'FT1'
                                                           AND FT.IN_STATION_TIME =
                                                                  (SELECT MAX(IN_STATION_TIME)
                                                                     FROM SFISM4.R117
                                                                     WHERE SERIAL_NUMBER = Z.SERIAL_NUMBER AND GROUP_NAME = 'FT1')
                                                        AND TE.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                         FROM   SFISM4.R117
                                                         WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                         AND GROUP_NAME = 'PACK_CTN')
                                                           AND CUST.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                           AND PRO.P_NO = Z.MODEL_NAME
                                                           AND PO.PO_NO = SAPDN.CUST_PO
                                                            AND SAPDN.DN_NO = PO.MODEL_NAME", dn);

                string sqlTX62WC = string.Format(@"SELECT DISTINCT Z.SERIAL_NUMBER,
                                                    SAPDN.CUST_PO
                                                    ||'-'
                                                    ||PO.TYPE                                             PO_NO,
                                                    Z.IMEI,
                                                    Z.MCARTON_NO,
                                                    Z.MODEL_NAME                                          AS MATNOFACTORY,
                                                    SHIP.ACCKEY                                           AS ACCKEY,
                                                    SHIP.FLASH_KEY,
                                                    To_char(SHIP.TEST_TIME, 'YYYYMMDDHHMMSS')             AS TEST_TIME,
                                                    To_char(SHIP.TRANSFER_TIME, 'YYYYMMDDHHMMSS')         AS TRANSFER_TIME,
                                                    SHIP.CERTIFICATE_ID                                   AS CERTIFICATE_ID,
                                                    SHIP.CERTIFICATE_PROFILE                              AS CERTIFICATE_PROFILE,
                                                    SHIP.PWFACTORY,
                                                    SHIP.PWDEBUG,
                                                    SHIP.PWPRETLS,
                                                    SHIP.DTLSPSK,
                                                    SHIP.BBSN,
                                                    SHIP.SPCKEY,
                                                    SHIP.RESET_CODE,
                                                    PRO.LINK_QTY                                          AS BOARDNO,
                                                    ''                                                    AS A4,
                                                    RB.INVOICE                                            DN_NO,
                                                    Concat (R117.YEA, R117.MONT)                          AS PRODUCT_DC,
                                                    RB.DNP,
                                                    RMB.DEFAULT_LINE                                      AS TEST_LINE,
                                                    RB.SO_NUMBER                                          AS PO_NUMBER,
                                                    CC.DATA3                                              AS INFO04,
                                                    CC.DATA4                                              AS INFO05,
                                                    CC.DATA2                                              AS EAN,
                                                    CC.INFO01,
                                                    CC.INFO06,
                                                    CC.DATA1
                                                    || '-'
                                                    || CC.HW                                              AS PART_NUMBER_SHIELD,
                                                    CC.DIVICE_NUMBER
                                                    || '-'
                                                    || Substr (RMB.HW_BOM, 1, 2)                          AS CUST_MODEL_NAME,
                                                    RMB.HW_BOM                                            AS REVISION,
                                                    TRSN.PANEL_NO,
                                                    RMB.SW_BOM                                            AS SW,
                                                    To_char (Add_months (RB.FINISH_DATE, 12), 'YYYYMMDD') AS WARRANTY_EXPIRED_DATE,
                                                    To_char (RB.FINISH_DATE, 'YYYYMMDD')                  AS DELIVERY_DATE,
                                                    To_char (RB.FINISH_DATE, 'HH24MMSS')                  AS DELIVERY_TIME,
                                                    To_char (RMB.MO_KP_START_DATE, 'YYYYMMDD')            AS PRODUCT_DATE,
                                                    To_char (RMB.MO_KP_START_DATE, 'HH24MISS')            AS PRODUCT_TIME,
                                                    CC.DATA1
                                                    || '-'
                                                    || Substr (RMB.HW_BOM, 1, 2)                          AS PART_NUMBER_CUSTOMER,
                                                    ''                                                    AS A14,
                                                    ''                                                    AS A15,
                                                    CC.CUST_MODEL_NAME                                    PRODUCT_TYPE,
                                                    CC.CUST_MODEL_NAME                                    INF02,
                                                    'FOV'                                                 AS PRODUCT_LOCATION,
                                                    ''                                                    AS A40,
                                                    To_char (FT.IN_STATION_TIME, 'YYYYMMDD')                                               AS FT_DATE,
                                                    To_char (PB.IN_STATION_TIME, 'YYYYMMDD')                                               AS PACKING_DATE,
                                                    SHIP.IMSI                                                                              AS IMSI,
                                                    SHIP.EUICCID                                                                         AS A59,
                                                    R108.KEY_PART_SN                                                                      AS MODULE_IMEI
                                    FROM   (SELECT FINISH_DATE,
                                                   SO_NUMBER,
                                                   TCOM,
                                                   INVOICE,
                                                   INV_NO,
                                                   MODEL_NAME,
                                                   ROWNUM * 10 AS DNP
                                            FROM   SFISM4.R_BPCS_INVOICE_T
                                            WHERE  INVOICE = '{0}'
                                                   AND ROWNUM = 1) RB,
                                           SFISM4.Z_WIP_TRACKING_T Z,
                                           (SELECT DISTINCT C.SERIAL_NUMBER,
                                                            CASE To_char (MIN(C.IN_LINE_TIME), 'MM')
                                                              WHEN '01' THEN '1'
                                                              WHEN '02' THEN '2'
                                                              WHEN '03' THEN '3'
                                                              WHEN '04' THEN '4'
                                                              WHEN '05' THEN '5'
                                                              WHEN '06' THEN '6'
                                                              WHEN '07' THEN '7'
                                                              WHEN '08' THEN '8'
                                                              WHEN '09' THEN '9'
                                                              WHEN '10' THEN 'O'
                                                              WHEN '11' THEN 'N'
                                                              WHEN '12' THEN 'D'
                                                            END AS MONT,
                                                            CASE To_char (MIN(C.IN_LINE_TIME), 'YYYY')
                                                              WHEN '2020' THEN 'M'
                                                              WHEN '2021' THEN 'N'
                                                              WHEN '2022' THEN 'P'
                                                              WHEN '2023' THEN 'R'
                                                              WHEN '2024' THEN 'S'
                                                              WHEN '2025' THEN 'T'
                                                              WHEN '2026' THEN 'U'
                                                              WHEN '2027' THEN 'V'
                                                              WHEN '2028' THEN 'W'
                                                              WHEN '2029' THEN 'X'
                                                            END AS YEA
                                            FROM   SFISM4.Z_WIP_TRACKING_T A,
                                                   SFISM4.R_BPCS_INVOICE_T B,
                                                   SFISM4.R_SN_DETAIL_T C
                                            WHERE  A.SHIP_NO = B.TCOM
                                                   AND B.INVOICE = '{0}'
                                                   AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                            GROUP  BY C.SERIAL_NUMBER) R117,
                                           (SELECT *
                                            FROM   SFIS1.C_CINTERION_SHIP_T) CC,
                                           (SELECT *
                                            FROM   SFISM4.R_MO_BASE_T
                                            WHERE  MODEL_NAME IN (SELECT MODEL_NAME
                                                                  FROM   SFIS1.C_CINTERION_SHIP_T)) RMB,
                                           SFISM4.R_SN_TRSN_LINK_T TRSN,
                                           SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                           SFISM4.R117 FT,
                                           SFISM4.R117 PB,
                                           SFISM4.R_CINTERION_TESTLOG_T SHIP,
                                           mes1.c_product_config@GZF12AP pro,
                                           SFIS1.C_PO_CONFIG_T PO,
                                           SFISM4.R108 R108
                                    WHERE  RB.TCOM = Z.SHIP_NO
                                           AND SAPDN.DN_NO = RB.INVOICE
                                           AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+)
                                           AND Z.MODEL_NAME = CC.MODEL_NAME
                                           AND Z.MO_NUMBER = RMB.MO_NUMBER
                                           AND Z.SERIAL_NUMBER = TRSN.SERIAL_NUMBER
                                           AND SHIP.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND R108.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND PRO.P_NO = Z.MODEL_NAME
                                           AND PO.MODEL_NAME = SAPDN.DN_NO
                                           AND FT.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND FT.GROUP_NAME = 'FT1'
                                           AND FT.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                                     FROM   SFISM4.R117
                                                                     WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                     AND GROUP_NAME = 'FT1')
                                                                     AND PB.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND PB.GROUP_NAME = 'PACK_CTN'
                                           AND PB.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                                     FROM   SFISM4.R117
                                                                     WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                            AND GROUP_NAME = 'PACK_CTN')", dn);
                string sqlT99W321 = string.Format(@"SELECT DISTINCT 'FOV'                                                 AS PRODUCT_LOCATION,
                                           Z.serial_number,
                                           Z.serial_number                                       MODULE_IMEI,
                                           Z.MODEL_NAME                                          AS MATNOFACTORY,
                                           Z.IMEI,
                                           Z.MCARTON_NO,
                                           Decode(Z.TRAY_NO, 'N/A', '',
                                                             Z.TRAY_NO)                          TRAY_NO,
                                           SAPDN.CUST_PO
                                           ||'-'
                                           ||PO.TYPE                                             PO_NO,
                                           BPCS.INVOICE                                          DN_NO,
                                           RMB.HW_BOM                                            AS REVISION,
                                           To_char(BPCS.FINISH_DATE, 'YYYYMMDD')                 AS DELIVERY_DATE,
                                           To_char(Add_months(BPCS.FINISH_DATE, 12), 'YYYYMMDD') AS WARRANTY_EXPIRED_DATE,
                                           To_char(RMB.MO_KP_START_DATE, 'YYYYMMDD')             AS PRODUCT_DATE,
                                           To_char(RMB.MO_KP_START_DATE, 'HH24MISS')             AS PRODUCT_TIME,
                                           TEST_LINE,
                                           FT_DATE,
                                           PACKING_DATE,
                                           Decode(Upper(CUST.SSN5), 'N/A', '',
                                                                    CUST.SSN5)                   A59,
                                           PANEL_NO,
                                           Concat(CASE To_char(Z.IN_LINE_TIME, 'YYYY')
                                                    WHEN '2020' THEN 'M'
                                                    WHEN '2021' THEN 'N'
                                                    WHEN '2022' THEN 'P'
                                                    WHEN '2023' THEN 'R'
                                                    WHEN '2024' THEN 'S'
                                                    WHEN '2025' THEN 'T'
                                                    WHEN '2026' THEN 'U'
                                                    WHEN '2027' THEN 'V'
                                                    WHEN '2028' THEN 'W'
                                                    WHEN '2029' THEN 'X'
                                                  END, CASE To_char(Z.IN_LINE_TIME, 'MM')
                                                         WHEN '01' THEN '1'
                                                         WHEN '02' THEN '2'
                                                         WHEN '03' THEN '3'
                                                         WHEN '04' THEN '4'
                                                         WHEN '05' THEN '5'
                                                         WHEN '06' THEN '6'
                                                         WHEN '07' THEN '7'
                                                         WHEN '08' THEN '8'
                                                         WHEN '09' THEN '9'
                                                         WHEN '10' THEN 'O'
                                                         WHEN '11' THEN 'N'
                                                         WHEN '12' THEN 'D'
                                                       END)                                      AS PRODUCT_DC,
                                           CC.CUST_MODEL_NAME                                    PRODUCT_TYPE,
                                           CC.INFO02                                             INF02,
                                           CC.DATA3                                              AS INFO04,
                                           CC.DATA4                                              AS INFO05,
                                           CC.DATA2                                              AS EAN,
                                           CC.INFO06,
                                           CC.INFO01,
                                           CC.DATA1
                                           || '-'
                                           || CC.HW                                              AS PART_NUMBER_SHIELD,
                                           CC.DIVICE_NUMBER
                                           || '-'
                                           || Substr(RMB.HW_BOM, 1, 2)                           AS CUST_MODEL_NAME,
                                           ''                                                    ACCKEY,
                                           ''                                                    BAG_TIME,
                                           RMB.SW_bom                                            SW,
                                           ''                                                    PART_NUMBER_CUSTOMER,
                                           ''                                                    DNP,
                                           Replace(Substr(CC.INFO06, 1, 13), '|', '.')           AS FWVERSION,
                                           Z.SERIAL_NUMBER                                       AS QR_CODE,
                                           CUST.SSN13                                            BBSN
                                    FROM   SFISM4.R_BPCS_INVOICE_T BPCS,
                                           SFISM4.Z_WIP_TRACKING_T Z,
                                           SFISM4.R_CUSTSN_T CUST,
                                           SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                           SFISM4.R_SN_TRSN_LINK_T SNLINK,
                                           SFIS1.C_CINTERION_SHIP_T CC,
                                           (SELECT *
                                            FROM   SFISM4.R_MO_BASE_T
                                            WHERE  MODEL_NAME IN(SELECT MODEL_NAME
                                                                 FROM   SFIS1.C_CINTERION_SHIP_T))RMB,
                                           (SELECT SERIAL_NUMBER,
                                                   LINE_NAME                            TEST_LINE,
                                                   To_char(IN_STATION_TIME, 'YYYYMMDD') FT_DATE
                                            FROM   SFISM4.R117
                                            WHERE  ( SERIAL_NUMBER, IN_STATION_TIME ) IN(SELECT SERIAL_NUMBER,
                                                                                                MAX(IN_STATION_TIME)
                                                                                         FROM   SFISM4.R117
                                                                                         WHERE  GROUP_NAME = 'FT'
                                                                                         GROUP  BY serial_number)) FT,
                                           (SELECT SERIAL_NUMBER,
                                                   To_char(MAX(IN_STATION_TIME), 'YYYYMMDD') PACKING_DATE
                                            FROM   SFISM4.R117
                                            WHERE  GROUP_NAME = 'PACK_CTN'
                                            GROUP  BY serial_number) PACK,
                                           SFIS1.C_PO_CONFIG_T PO
                                    WHERE  BPCS.invoice = '{0}'
                                           AND BPCS.TCOM = Z.SHIP_NO
                                           AND BPCS.INVOICE = SAPDN.DN_NO
                                           AND Z.MO_NUMBER = RMB.MO_NUMBER
                                           AND Z.SERIAL_NUMBER = FT.serial_number
                                           AND Z.SERIAL_NUMBER = PACK.serial_number
                                           AND Z.SERIAL_NUMBER = CUST.serial_number
                                           AND Z.SERIAL_NUMBER = SNLINK.serial_number
                                           AND Z.MODEL_NAME = CC.MODEL_NAME
                                           AND PO.MODEL_NAME = SAPDN.DN_NO ", dn);

                ///
                if (model_name.StartsWith("PLS62-W"))
                {
                    dt = await ExecuteSQL(sql_pls62, sfcHttpClient);
                }
                else if (model_name.StartsWith("MPLS"))
                {
                    dt = await ExecuteSQL(sqlMPLS83_W, sfcHttpClient);
                }
                else if (model_name.StartsWith("T99W373.0"))
                {
                    dt = await ExecuteSQL(sqlRattleSnake, sfcHttpClient);
                }                
                else if (model_name.StartsWith("T99W321.2"))
                {
                    sqlT99W321 = string.Format(@"SELECT 'FOV'                                    AS PRODUCT_LOCATION,
                                           Z.serial_number,
                                           Z.serial_number                                       MODULE_IMEI,
                                           Z.MODEL_NAME                                          AS MATNOFACTORY,
                                           Z.IMEI,
                                           Z.MCARTON_NO,
                                           Decode(Z.TRAY_NO, 'N/A', '',
                                                             Z.TRAY_NO)                          TRAY_NO,
                                           SAPDN.CUST_PO
                                           ||'-'
                                           ||PO.TYPE                                             PO_NO,
                                           BPCS.INVOICE                                          DN_NO,
                                           RMB.HW_BOM                                            AS REVISION,
                                           To_char(BPCS.FINISH_DATE, 'YYYYMMDD')                 AS DELIVERY_DATE,
                                           To_char(Add_months(BPCS.FINISH_DATE, 12), 'YYYYMMDD') AS WARRANTY_EXPIRED_DATE,
                                           To_char(RMB.MO_KP_START_DATE, 'YYYYMMDD')             AS PRODUCT_DATE,
                                           To_char(RMB.MO_KP_START_DATE, 'HH24MISS')             AS PRODUCT_TIME,
                                           TEST_LINE,
                                           FT_DATE,
                                           PACKING_DATE,
                                           PANEL_NO,
                                           Concat(CASE To_char(Z.IN_LINE_TIME, 'YYYY')
                                                    WHEN '2020' THEN 'M'
                                                    WHEN '2021' THEN 'N'
                                                    WHEN '2022' THEN 'P'
                                                    WHEN '2023' THEN 'R'
                                                    WHEN '2024' THEN 'S'
                                                    WHEN '2025' THEN 'T'
                                                    WHEN '2026' THEN 'U'
                                                    WHEN '2027' THEN 'V'
                                                    WHEN '2028' THEN 'W'
                                                    WHEN '2029' THEN 'X'
                                                  END, CASE To_char(Z.IN_LINE_TIME, 'MM')
                                                         WHEN '01' THEN '1'
                                                         WHEN '02' THEN '2'
                                                         WHEN '03' THEN '3'
                                                         WHEN '04' THEN '4'
                                                         WHEN '05' THEN '5'
                                                         WHEN '06' THEN '6'
                                                         WHEN '07' THEN '7'
                                                         WHEN '08' THEN '8'
                                                         WHEN '09' THEN '9'
                                                         WHEN '10' THEN 'O'
                                                         WHEN '11' THEN 'N'
                                                         WHEN '12' THEN 'D'
                                                       END)                                      AS PRODUCT_DC,
                                           CC.CUST_MODEL_NAME                                    PRODUCT_TYPE,
                                           CC.INFO02                                             INF02,
                                           CC.DATA3                                              AS INFO04,
                                           CC.DATA4                                              AS INFO05,
                                           CC.DATA2                                              AS EAN,
                                           CC.INFO06,
                                           CC.INFO01,
                                           CC.DATA1
                                           || '-'
                                           || CC.HW                                              AS PART_NUMBER_SHIELD,
                                           CC.DIVICE_NUMBER
                                           || '-'
                                           || Substr(RMB.HW_BOM, 1, 2)                           AS CUST_MODEL_NAME,
                                           wpa.WPAKEY                                                    ACCKEY,
                                           RMB.SW_bom                                                     SW,
                                           CUST.SSN8||'-'||       RMB.HW_BOM                                             PART_NUMBER_CUSTOMER,
                                           ''                                                    DNP,
                                           Replace(Substr(CC.INFO06, 1, 13), '|', '.')           AS FWVERSION,
                                           Z.SERIAL_NUMBER                                       AS QR_CODE,
                                           To_char (PR.IN_STATION_TIME, 'YYYYMMDDHHMMSS')                                       AS BAG_TIME,
                                           Lpad(Round(( ( PB.IN_STATION_TIME - RO.IN_STATION_TIME ) * 24 * 60 * 60 ), 0), 9, 0) AS FLOOR_TIME,
                                           CUST.SSN12                                            BBSN,
                                           CUST.SSN13                                            FLASH_KEY,
                                           CUST.SSN14                                                A59,
                                           CUST.SSN15                                            IMSI,
                                           CUST.SSN19                                                                           A42,
                                           CUST.SSN17                                            CERTIFICATE_PROFILE,
                                           CUST.SSN18                                            CERTIFICATE_ID                                          
                                    FROM   SFISM4.R_BPCS_INVOICE_T BPCS,
                                           SFISM4.Z_WIP_TRACKING_T Z,
                                           SFISM4.R117 PR,
                                           SFISM4.R117 PB,
                                           SFISM4.R117 RO,
                                           (SELECT * FROM SFISM4.R_CUSTSN_T_BAK WHERE ROWID IN ( SELECT ROWID FROM 
                                    (SELECT SERIAL_NUMBER,GROUP_NAME,IN_STATION_TIME,(ROW_NUMBER() OVER (PARTITION BY SERIAL_NUMBER,GROUP_NAME ORDER BY IN_STATION_TIME DESC)) AS ROW1
                                     FROM SFISM4.R_CUSTSN_T_BAK WHERE  GROUP_NAME = 'RC' AND  SERIAL_NUMBER IN (
                                     SELECT SERIAL_NUMBER FROM SFISM4.Z107 WHERE SHIP_NO = (SELECT tcom FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE ='{0}')) ) WHERE ROW1 = '1') ) CUST,
                                           SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                           SFISM4.R_SN_TRSN_LINK_T SNLINK,
                                           SFIS1.C_CINTERION_SHIP_T CC,
                                           (SELECT *
                                            FROM   SFISM4.R_MO_BASE_T
                                            WHERE  MODEL_NAME IN(SELECT MODEL_NAME
                                                                 FROM   SFIS1.C_CINTERION_SHIP_T))RMB,
                                           (SELECT SERIAL_NUMBER,
                                                   LINE_NAME                            TEST_LINE,
                                                   To_char(IN_STATION_TIME, 'YYYYMMDD') FT_DATE
                                            FROM   SFISM4.R117
                                            WHERE ( SERIAL_NUMBER, IN_STATION_TIME ) IN(SELECT SERIAL_NUMBER,
                                                                                               MAX(IN_STATION_TIME)
                                                                                        FROM   SFISM4.R117
                                                                                        WHERE  GROUP_NAME = 'FT'
                                                                                        GROUP  BY serial_number)) FT,
                                           (SELECT SERIAL_NUMBER,
                                                   To_char(MAX(IN_STATION_TIME), 'YYYYMMDD') PACKING_DATE
                                            FROM   SFISM4.R117
                                            WHERE  GROUP_NAME = 'PACK_CTN'
                                            GROUP  BY serial_number) PACK,
                                            SFIS1.C_PO_CONFIG_T PO,
                                            SFISM4.R_WPAKEY_WPSPIN_T wpa
                                    WHERE  BPCS.invoice = '{0}'
                                           AND BPCS.TCOM = Z.SHIP_NO
                                           AND BPCS.INVOICE = SAPDN.DN_NO
                                           AND Z.MO_NUMBER = RMB.MO_NUMBER
                                           AND Z.SERIAL_NUMBER = FT.serial_number
                                           AND Z.SERIAL_NUMBER = PACK.serial_number
                                           AND Z.SERIAL_NUMBER = CUST.serial_number
                                           AND Z.SERIAL_NUMBER = wpa.serial_number
                                           AND CUST.GROUP_NAME = 'RC'
                                           AND Z.SERIAL_NUMBER = SNLINK.serial_number
                                           AND Z.MODEL_NAME = CC.MODEL_NAME
                                           AND PO.MODEL_NAME = SAPDN.DN_NO 
                                           AND PR.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND PR.GROUP_NAME = 'PRINT'
                                           AND PR.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                                     FROM   SFISM4.R117
                                                                     WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                            AND GROUP_NAME = 'PRINT')
                                           AND RO.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND RO.GROUP_NAME = 'ROAST_OUT'
                                           AND RO.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                                     FROM   SFISM4.R117
                                                                     WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                            AND GROUP_NAME = 'ROAST_OUT')
                                           AND PB.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND PB.GROUP_NAME = 'CHECK_MSL'
                                           AND PB.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                                     FROM   SFISM4.R117
                                                                     WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                            AND GROUP_NAME = 'CHECK_MSL')", dn);
                    dt = await ExecuteSQL(sqlT99W321, sfcHttpClient);
                }
                else if (model_name.StartsWith("T99W321.1")|| model_name.StartsWith("T99W321.3")|| model_name.StartsWith("T99W321.4") ||  model_name.StartsWith("T99W321.06"))
                {
                    sqlT99W321 = string.Format(@"SELECT 'FOV'                                    AS PRODUCT_LOCATION,
                                           Z.serial_number,
                                           Z.serial_number                                       MODULE_IMEI,
                                           Z.MODEL_NAME                                          AS MATNOFACTORY,
                                           Z.IMEI,
                                           Z.MCARTON_NO,
                                           Decode(Z.TRAY_NO, 'N/A', '',
                                                             Z.TRAY_NO)                          TRAY_NO,
                                           SAPDN.CUST_PO
                                           ||'-'
                                           ||PO.TYPE                                             PO_NO,
                                           BPCS.INVOICE                                          DN_NO,
                                           RMB.HW_BOM                                            AS REVISION,
                                           To_char(BPCS.FINISH_DATE, 'YYYYMMDD')                 AS DELIVERY_DATE,
                                           To_char(Add_months(BPCS.FINISH_DATE, 12), 'YYYYMMDD') AS WARRANTY_EXPIRED_DATE,
                                           To_char(RMB.MO_KP_START_DATE, 'YYYYMMDD')             AS PRODUCT_DATE,
                                           To_char(RMB.MO_KP_START_DATE, 'HH24MISS')             AS PRODUCT_TIME,
                                           TEST_LINE,
                                           FT_DATE,
                                           PACKING_DATE,
                                           Decode(Upper(CUST.SSN14), 'N/A', '',
                                                                    CUST.SSN14)                   A59,
                                           PANEL_NO,
                                           Concat(CASE To_char(Z.IN_LINE_TIME, 'YYYY')
                                                    WHEN '2020' THEN 'M'
                                                    WHEN '2021' THEN 'N'
                                                    WHEN '2022' THEN 'P'
                                                    WHEN '2023' THEN 'R'
                                                    WHEN '2024' THEN 'S'
                                                    WHEN '2025' THEN 'T'
                                                    WHEN '2026' THEN 'U'
                                                    WHEN '2027' THEN 'V'
                                                    WHEN '2028' THEN 'W'
                                                    WHEN '2029' THEN 'X'
                                                  END, CASE To_char(Z.IN_LINE_TIME, 'MM')
                                                         WHEN '01' THEN '1'
                                                         WHEN '02' THEN '2'
                                                         WHEN '03' THEN '3'
                                                         WHEN '04' THEN '4'
                                                         WHEN '05' THEN '5'
                                                         WHEN '06' THEN '6'
                                                         WHEN '07' THEN '7'
                                                         WHEN '08' THEN '8'
                                                         WHEN '09' THEN '9'
                                                         WHEN '10' THEN 'O'
                                                         WHEN '11' THEN 'N'
                                                         WHEN '12' THEN 'D'
                                                       END)                                      AS PRODUCT_DC,
                                           CC.CUST_MODEL_NAME                                    PRODUCT_TYPE,
                                           CC.INFO02                                             INF02,
                                           CC.DATA3                                              AS INFO04,
                                           CC.DATA4                                              AS INFO05,
                                           CC.DATA2                                              AS EAN,
                                           CC.INFO06,
                                           CC.INFO01,
                                           CC.DATA1
                                           || '-'
                                           || CC.HW                                              AS PART_NUMBER_SHIELD,
                                           CC.DIVICE_NUMBER
                                           || '-'
                                           || Substr(RMB.HW_BOM, 1, 2)                           AS CUST_MODEL_NAME,
                                           wpa.WPAKEY                                                    ACCKEY,
                                           RMB.SW_bom                                                     SW,
                                           CUST.SSN8||'-'||       RMB.HW_BOM                                             PART_NUMBER_CUSTOMER,
                                           ''                                                    DNP,
                                           Replace(Substr(CC.INFO06, 1, 13), '|', '.')           AS FWVERSION,
                                           Z.SERIAL_NUMBER                                       AS QR_CODE,
                                           To_char (PR.IN_STATION_TIME, 'YYYYMMDDHHMMSS')                                       AS BAG_TIME,
                                           Lpad(Round(( ( PB.IN_STATION_TIME - RO.IN_STATION_TIME ) * 24 * 60 * 60 ), 0), 9, 0) AS FLOOR_TIME,
                                           CUST.SSN12                                            BBSN,
                                           CUST.SSN13                                            FLASH_KEY,
                                           CUST.SSN17                                            CERTIFICATE_PROFILE,
                                           CUST.SSN18                                            CERTIFICATE_ID                                          
                                    FROM   SFISM4.R_BPCS_INVOICE_T BPCS,
                                           SFISM4.Z_WIP_TRACKING_T Z,
                                           SFISM4.R117 PR,
                                           SFISM4.R117 PB,
                                           SFISM4.R117 RO,
                                           (SELECT * FROM SFISM4.R_CUSTSN_T_BAK WHERE ROWID IN ( SELECT ROWID FROM 
                                    (SELECT SERIAL_NUMBER,GROUP_NAME,IN_STATION_TIME,(ROW_NUMBER() OVER (PARTITION BY SERIAL_NUMBER,GROUP_NAME ORDER BY IN_STATION_TIME DESC)) AS ROW1
                                     FROM SFISM4.R_CUSTSN_T_BAK WHERE  GROUP_NAME = 'RC' AND  SERIAL_NUMBER IN (
                                     SELECT SERIAL_NUMBER FROM SFISM4.Z107 WHERE SHIP_NO = (SELECT tcom FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE ='{0}')) ) WHERE ROW1 = '1') ) CUST,
                                           SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                           SFISM4.R_SN_TRSN_LINK_T SNLINK,
                                           SFIS1.C_CINTERION_SHIP_T CC,
                                           (SELECT *
                                            FROM   SFISM4.R_MO_BASE_T
                                            WHERE  MODEL_NAME IN(SELECT MODEL_NAME
                                                                 FROM   SFIS1.C_CINTERION_SHIP_T))RMB,
                                           (SELECT SERIAL_NUMBER,
                                                   LINE_NAME                            TEST_LINE,
                                                   To_char(IN_STATION_TIME, 'YYYYMMDD') FT_DATE
                                            FROM   SFISM4.R117
                                            WHERE ( SERIAL_NUMBER, IN_STATION_TIME ) IN(SELECT SERIAL_NUMBER,
                                                                                               MAX(IN_STATION_TIME)
                                                                                        FROM   SFISM4.R117
                                                                                        WHERE  GROUP_NAME = 'FT'
                                                                                        GROUP  BY serial_number)) FT,
                                           (SELECT SERIAL_NUMBER,
                                                   To_char(MAX(IN_STATION_TIME), 'YYYYMMDD') PACKING_DATE
                                            FROM   SFISM4.R117
                                            WHERE  GROUP_NAME = 'PACK_CTN'
                                            GROUP  BY serial_number) PACK,
                                            SFIS1.C_PO_CONFIG_T PO,
                                            SFISM4.R_WPAKEY_WPSPIN_T wpa
                                    WHERE  BPCS.invoice = '{0}'
                                           AND BPCS.TCOM = Z.SHIP_NO
                                           AND BPCS.INVOICE = SAPDN.DN_NO
                                           AND Z.MO_NUMBER = RMB.MO_NUMBER
                                           AND Z.SERIAL_NUMBER = FT.serial_number
                                           AND Z.SERIAL_NUMBER = PACK.serial_number
                                           AND Z.SERIAL_NUMBER = CUST.serial_number
                                           AND Z.SERIAL_NUMBER = wpa.serial_number
                                           AND CUST.GROUP_NAME = 'RC'
                                           AND Z.SERIAL_NUMBER = SNLINK.serial_number
                                           AND Z.MODEL_NAME = CC.MODEL_NAME
                                           AND PO.MODEL_NAME = SAPDN.DN_NO 
                                           AND PR.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND PR.GROUP_NAME = 'PRINT'
                                           AND PR.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                                     FROM   SFISM4.R117
                                                                     WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                            AND GROUP_NAME = 'PRINT')
                                           AND RO.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND RO.GROUP_NAME = 'ROAST_OUT'
                                           AND RO.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                                     FROM   SFISM4.R117
                                                                     WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                            AND GROUP_NAME = 'ROAST_OUT')
                                           AND PB.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND PB.GROUP_NAME = 'CHECK_MSL'
                                           AND PB.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                                     FROM   SFISM4.R117
                                                                     WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                            AND GROUP_NAME = 'CHECK_MSL')", dn);
                    dt = await ExecuteSQL(sqlT99W321, sfcHttpClient);
                }
                else if (model_name.StartsWith("T99W321."))
                {
                    sqlT99W321 = string.Format(@"SELECT DISTINCT 'FOV'                                    AS PRODUCT_LOCATION,
                                           Z.serial_number,
                                           Z.serial_number                                       MODULE_IMEI,
                                           Z.MODEL_NAME                                          AS MATNOFACTORY,
                                           Z.IMEI,
                                           Z.MCARTON_NO,
                                           Decode(Z.TRAY_NO, 'N/A', '',
                                                             Z.TRAY_NO)                          TRAY_NO,
                                           Decode(Upper(CUST.SSN14), 'N/A', '',
                                                                    CUST.SSN14)                   A59,
                                           SAPDN.CUST_PO
                                           ||'-'
                                           ||PO.TYPE                                             PO_NO,
                                           BPCS.INVOICE                                          DN_NO,
                                           RMB.HW_BOM                                            AS REVISION,
                                           To_char(BPCS.FINISH_DATE, 'YYYYMMDD')                 AS DELIVERY_DATE,
                                           To_char(Add_months(BPCS.FINISH_DATE, 12), 'YYYYMMDD') AS WARRANTY_EXPIRED_DATE,
                                           To_char(RMB.MO_KP_START_DATE, 'YYYYMMDD')             AS PRODUCT_DATE,
                                           To_char(RMB.MO_KP_START_DATE, 'HH24MISS')             AS PRODUCT_TIME,
                                           TEST_LINE,
                                           FT_DATE,
                                           PACKING_DATE,
                                           PANEL_NO,
                                           Concat(CASE To_char(Z.IN_LINE_TIME, 'YYYY')
                                                    WHEN '2020' THEN 'M'
                                                    WHEN '2021' THEN 'N'
                                                    WHEN '2022' THEN 'P'
                                                    WHEN '2023' THEN 'R'
                                                    WHEN '2024' THEN 'S'
                                                    WHEN '2025' THEN 'T'
                                                    WHEN '2026' THEN 'U'
                                                    WHEN '2027' THEN 'V'
                                                    WHEN '2028' THEN 'W'
                                                    WHEN '2029' THEN 'X'
                                                  END, CASE To_char(Z.IN_LINE_TIME, 'MM')
                                                         WHEN '01' THEN '1'
                                                         WHEN '02' THEN '2'
                                                         WHEN '03' THEN '3'
                                                         WHEN '04' THEN '4'
                                                         WHEN '05' THEN '5'
                                                         WHEN '06' THEN '6'
                                                         WHEN '07' THEN '7'
                                                         WHEN '08' THEN '8'
                                                         WHEN '09' THEN '9'
                                                         WHEN '10' THEN 'O'
                                                         WHEN '11' THEN 'N'
                                                         WHEN '12' THEN 'D'
                                                       END)                                      AS PRODUCT_DC,
                                           CC.CUST_MODEL_NAME                                    PRODUCT_TYPE,
                                           CC.INFO02                                             INF02,
                                           CC.DATA3                                              AS INFO04,
                                           CC.DATA4                                              AS INFO05,
                                           CC.DATA2                                              AS EAN,
                                           CC.INFO06,
                                           CC.INFO01,
                                           CC.DATA1
                                           || '-'
                                           || CC.HW                                              AS PART_NUMBER_SHIELD,
                                           CC.DIVICE_NUMBER
                                           || '-'
                                           || Substr(RMB.HW_BOM, 1, 2)                           AS CUST_MODEL_NAME,
                                           wpa.WPAKEY                                                    ACCKEY,
                                           RMB.SW_bom                                                     SW,
                                           CUST.SSN8||'-'||       RMB.HW_BOM                                             PART_NUMBER_CUSTOMER,
                                           ''                                                    DNP,
                                           Replace(Substr(CC.INFO06, 1, 13), '|', '.')           AS FWVERSION,
                                           Z.SERIAL_NUMBER                                       AS QR_CODE,
                                           To_char (PR.IN_STATION_TIME, 'YYYYMMDDHHMMSS')                                       AS BAG_TIME,
                                           Lpad(Round(( ( PB.IN_STATION_TIME - RO.IN_STATION_TIME ) * 24 * 60 * 60 ), 0), 9, 0) AS FLOOR_TIME,
                                           CUST.SSN12                                            BBSN,
                                           CUST.SSN13                                            FLASH_KEY,
                                           CUST.SSN15                                                                           IMSI,
                                           CUST.SSN17                                            CERTIFICATE_PROFILE,
                                           CUST.SSN18                                            CERTIFICATE_ID                                          
                                    FROM   SFISM4.R_BPCS_INVOICE_T BPCS,
                                           SFISM4.Z_WIP_TRACKING_T Z,
                                           SFISM4.R117 PR,
                                           SFISM4.R117 PB,
                                           SFISM4.R117 RO,
                                           (SELECT * FROM SFISM4.R_CUSTSN_T_BAK WHERE ROWID IN ( SELECT ROWID FROM 
                                    (SELECT SERIAL_NUMBER,GROUP_NAME,IN_STATION_TIME,(ROW_NUMBER() OVER (PARTITION BY SERIAL_NUMBER,GROUP_NAME ORDER BY IN_STATION_TIME DESC)) AS ROW1
                                     FROM SFISM4.R_CUSTSN_T_BAK WHERE  GROUP_NAME = 'RC' AND  SERIAL_NUMBER IN (
                                     SELECT SERIAL_NUMBER FROM SFISM4.Z107 WHERE SHIP_NO = (SELECT tcom FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE ='{0}')) ) WHERE ROW1 = '1') ) CUST,
                                           SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                           SFISM4.R_SN_TRSN_LINK_T SNLINK,
                                           SFIS1.C_CINTERION_SHIP_T CC,
                                           (SELECT *
                                            FROM   SFISM4.R_MO_BASE_T
                                            WHERE  MODEL_NAME IN(SELECT MODEL_NAME
                                                                 FROM   SFIS1.C_CINTERION_SHIP_T))RMB,
                                           (SELECT SERIAL_NUMBER,
                                                   LINE_NAME                            TEST_LINE,
                                                   To_char(IN_STATION_TIME, 'YYYYMMDD') FT_DATE
                                            FROM   SFISM4.R117
                                            WHERE ( SERIAL_NUMBER, IN_STATION_TIME ) IN(SELECT SERIAL_NUMBER,
                                                                                               MAX(IN_STATION_TIME)
                                                                                        FROM   SFISM4.R117
                                                                                        WHERE  GROUP_NAME = 'FT'
                                                                                        GROUP  BY serial_number)) FT,
                                           (SELECT SERIAL_NUMBER,
                                                   To_char(MAX(IN_STATION_TIME), 'YYYYMMDD') PACKING_DATE
                                            FROM   SFISM4.R117
                                            WHERE  GROUP_NAME = 'PACK_CTN'
                                            GROUP  BY serial_number) PACK,
                                            SFIS1.C_PO_CONFIG_T PO,
                                            SFISM4.R_WPAKEY_WPSPIN_T wpa
                                    WHERE  BPCS.invoice = '{0}'
                                           AND BPCS.TCOM = Z.SHIP_NO
                                           AND BPCS.INVOICE = SAPDN.DN_NO
                                           AND Z.MO_NUMBER = RMB.MO_NUMBER
                                           AND Z.SERIAL_NUMBER = FT.serial_number
                                           AND Z.SERIAL_NUMBER = PACK.serial_number
                                           AND Z.SERIAL_NUMBER = CUST.serial_number
                                           AND Z.SERIAL_NUMBER = wpa.serial_number
                                           AND CUST.GROUP_NAME = 'RC'
                                           AND Z.SERIAL_NUMBER = SNLINK.serial_number
                                           AND Z.MODEL_NAME = CC.MODEL_NAME
                                           AND PO.MODEL_NAME = SAPDN.DN_NO 
                                           AND PR.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND PR.GROUP_NAME = 'PRINT'
                                           AND PR.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                                     FROM   SFISM4.R117
                                                                     WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                            AND GROUP_NAME = 'PRINT')
                                           AND RO.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND RO.GROUP_NAME = 'ROAST_OUT'
                                           AND RO.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                                     FROM   SFISM4.R117
                                                                     WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                            AND GROUP_NAME = 'ROAST_OUT')
                                           AND PB.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                           AND PB.GROUP_NAME = 'CHECK_MSL'
                                           AND PB.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                                     FROM   SFISM4.R117
                                                                     WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                            AND GROUP_NAME = 'CHECK_MSL')", dn);
                    dt = await ExecuteSQL(sqlT99W321, sfcHttpClient);
                }
                else if (model_name.StartsWith("T99W321"))
                {
                    dt = await ExecuteSQL(sqlT99W321, sfcHttpClient);
                }
                else if (model_name.StartsWith("TN23"))
                {
                    dt = await ExecuteSQL(sqlTN23, sfcHttpClient);
                }
                else if (model_name.StartsWith("T99W565.02"))
                {
                    dt = await ExecuteSQL(sqlT99w56502, sfcHttpClient);
                }
                else if (model_name == "95.3443T00")
                {
                    dt = await ExecuteSQL(sql95, sfcHttpClient);
                }
                else if (model_name.StartsWith("TX62-WC"))
                {
                    dt = await ExecuteSQL(sqlTX62WC, sfcHttpClient);
                }
                else if (model_name.StartsWith("95.2999T20"))
                {
                    var sql952699 = string.Format(@"SELECT DISTINCT Z.SERIAL_NUMBER,
                                            SAPDN.CUST_PO
                                            ||PO.TYPE                                           PO_NO,
                                            Z.IMEI,
                                            Z.TRAY_NO,
                                            Z.MCARTON_NO,
                                            Z.MODEL_NAME                                        AS MATNOFACTORY,
                                            PRO.LINK_QTY                                        AS BOARDNO,
                                            ''                                                  AS A4,
                                            To_char(FT.IN_STATION_TIME, 'YYYYMMDD')             AS FT_DATE,
                                            RB.INVOICE                                          DN_NO,
                                            Concat(R117.YEA, R117.MONT)                         AS PRODUCT_DC,
                                            RB.DNP,
                                            RMB.DEFAULT_LINE                                    AS TEST_LINE,
                                            RB.SO_NUMBER                                        AS PO_NUMBER,
                                            CC.DATA3                                            AS INFO04,
                                            CC.DATA4                                            AS INFO05,
                                            CC.DATA2                                            AS EAN,
                                            CC.INFO06,
                                            CC.INFO01,
                                            RMB.HW_BOM                                          AS REVISION,
                                            TRSN.PANEL_NO,
                                            RMB.SW_BOM                                          AS SW,
                                            To_char(Add_months(RB.FINISH_DATE, 12), 'YYYYMMDD') AS WARRANTY_EXPIRED_DATE,
                                            To_char(RB.FINISH_DATE, 'YYYYMMDD')                 AS DELIVERY_DATE,
                                            To_char(RB.FINISH_DATE, 'HH24MMSS')                 AS DELIVERY_TIME,
                                            To_char(RMB.MO_KP_START_DATE, 'YYYYMMDD')           AS PRODUCT_DATE,
                                            To_char(RMB.MO_KP_START_DATE, 'HH24MISS')           AS PRODUCT_TIME,
                                            ''                                                  AS A14,
                                            ''                                                  AS A15,
                                            CC.CUST_MODEL_NAME                                  PRODUCT_TYPE,
                                            CC.CUST_MODEL_NAME                                  INF02,
                                            'FOV'                                               AS PRODUCT_LOCATION,
                                            ''                                                  AS A40,
                                            CC.DATA1 || '-' || SUBSTR (RMB.HW_BOM, 1, 2) AS PART_NUMBER_CUSTOMER,
                                                                                       R_CUST.SSN12                                            BBSN,
                                                                       R_CUST.SSN13                                            FLASH_KEY,
                                                                       R_CUST.SSN14                                                A59,
                                                                       R_CUST.SSN15                                            IMSI,
                                                                       R_CUST.SSN19                                                                           A42,
                                                                       R_CUST.SSN17                                            CERTIFICATE_PROFILE,
                                                                       R_CUST.SSN18                                            CERTIFICATE_ID     
                            FROM  (SELECT FINISH_DATE,
                                          SO_NUMBER,
                                          TCOM,
                                          INVOICE,
                                          INV_NO,
                                          MODEL_NAME,
                                          ROWNUM * 10 AS DNP
                                   FROM   SFISM4.R_BPCS_INVOICE_T
                                   WHERE  INVOICE = '{0}'
                                          AND ROWNUM = 1) RB,
                                  SFISM4.Z_WIP_TRACKING_T Z,
                                  (SELECT DISTINCT C.SERIAL_NUMBER,
                                                   CASE To_char(MIN(C.IN_LINE_TIME), 'MM')
                                                     WHEN '01' THEN '1'
                                                     WHEN '02' THEN '2'
                                                     WHEN '03' THEN '3'
                                                     WHEN '04' THEN '4'
                                                     WHEN '05' THEN '5'
                                                     WHEN '06' THEN '6'
                                                     WHEN '07' THEN '7'
                                                     WHEN '08' THEN '8'
                                                     WHEN '09' THEN '9'
                                                     WHEN '10' THEN 'O'
                                                     WHEN '11' THEN 'N'
                                                     WHEN '12' THEN 'D'
                                                   END AS MONT,
                                                   CASE To_char(MIN(C.IN_LINE_TIME), 'YYYY')
                                                     WHEN '2020' THEN 'M'
                                                     WHEN '2021' THEN 'N'
                                                     WHEN '2022' THEN 'P'
                                                     WHEN '2023' THEN 'R'
                                                     WHEN '2024' THEN 'S'
                                                     WHEN '2025' THEN 'T'
                                                     WHEN '2026' THEN 'U'
                                                     WHEN '2027' THEN 'V'
                                                     WHEN '2028' THEN 'W'
                                                     WHEN '2029' THEN 'X'
                                                   END AS YEA
                                   FROM   SFISM4.Z_WIP_TRACKING_T A,
                                          SFISM4.R_BPCS_INVOICE_T B,
                                          SFISM4.R_SN_DETAIL_T C
                                   WHERE  A.SHIP_NO = B.TCOM
                                          AND B.INVOICE = '{0}'
                                          AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                   GROUP  BY C.SERIAL_NUMBER) R117,
                                  (SELECT *
                                   FROM   SFIS1.C_CINTERION_SHIP_T) CC,
                                  (SELECT *
                                   FROM   SFISM4.R_MO_BASE_T
                                   WHERE  MODEL_NAME IN(SELECT MODEL_NAME
                                                        FROM   SFIS1.C_CINTERION_SHIP_T)) RMB,
                                  SFISM4.R_SN_TRSN_LINK_T TRSN,
                                  SFISM4.R117 FT,
                                  SFISM4.R117 TE,
                                  SFISM4.R_CUSTSN_T CUST,
                                  SFISM4.R_SAP_DN_DETAIL_T SAPDN,
                                  mes1.c_product_config @GZF12AP pro,
                                  SFIS1.C_PO_CONFIG_T PO,
                                  SFISM4.R108 R108,
                                  SFISM4.R_CUSTSN_T R_CUST
                            WHERE  RB.TCOM = Z.SHIP_NO
                                   AND SAPDN.DN_NO = RB.INVOICE
                                   AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+)
                                   AND Z.SERIAL_NUMBER = R108.SERIAL_NUMBER(+)
                                   AND R108.KEY_PART_SN = R_CUST.SERIAL_NUMBER
                                   AND Z.MODEL_NAME = CC.MODEL_NAME
                                   AND Z.MO_NUMBER = RMB.MO_NUMBER
                                   AND Z.SERIAL_NUMBER = TRSN.SERIAL_NUMBER
                                   AND Z.SERIAL_NUMBER = FT.SERIAL_NUMBER(+)
                                   AND FT.GROUP_NAME = 'FT1'
                                   AND FT.IN_STATION_TIME = (SELECT MAX(IN_STATION_TIME)
                                                             FROM   SFISM4.R117
                                                             WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                    AND GROUP_NAME = 'FT1')
                                   AND TE.IN_STATION_TIME = (SELECT MAX (IN_STATION_TIME)
                                                             FROM   SFISM4.R117
                                                             WHERE  SERIAL_NUMBER = Z.SERIAL_NUMBER
                                                                    AND GROUP_NAME = 'PACK_CTN')
                                   AND CUST.SERIAL_NUMBER = Z.SERIAL_NUMBER
                                   AND PRO.P_NO = Z.MODEL_NAME
                                   AND PO.PO_NO = SAPDN.CUST_PO
                                   AND SAPDN.DN_NO = PO.MODEL_NAME ", dn);
                    dt = await ExecuteSQL(sql952699, sfcHttpClient);
                }
                else
                {
                    dt = await ExecuteSQL(sql_serval, sfcHttpClient);
                }
            }
            catch (Exception e)
            {

            }
            if (dt.Rows.Count == 0)
                System.Windows.Forms.MessageBox.Show("No data");
            else
            {
                dt.Columns.Add("ManualInvoice");
                foreach (DataRow dr in dt.Rows)
                {
                    dr["ManualInvoice"] = invoice;
                }
            }
            return dt;
        }
        public async Task<string> CheckFileSP(string invoice, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;

            dt = await ExecuteSQL("select  SFIS1.CHECK_GEMALTO_SHIP_FILE.GET_CHECK_SHIP_FILE_RES('" + invoice + "') as returnvalue from dual", sfcHttpClient);
            return dt.Rows[0]["returnvalue"].ToString();
        }
        public async Task<string> CheckModel(string invoice, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;

            dt = await ExecuteSQL("select MODEL_NAME from SFISM4.R_BPCS_INVOICE_T where INVOICE = '" + invoice + "'", sfcHttpClient);
            return dt.Rows[0]["MODEL_NAME"].ToString();
        }
        public async Task<int> GetCountZ107(string dn, SfcHttpClient sfcHttpClient)
        {
            string sql_check = @"SELECT A.* FROM SFISM4.Z107 A, SFISM4.R_BPCS_INVOICE_T B  WHERE SHIP_NO = TCOM AND INVOICE = '" + dn + "'";
            DataTable dt2 = new DataTable();
            dt2 = await ExecuteSQL(sql_check, sfcHttpClient);

            return dt2.Rows.Count;
        }

        public async Task<DataTable> ExecuteSQL(string sql, SfcHttpClient sfcHttpClient)
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
    }
}
