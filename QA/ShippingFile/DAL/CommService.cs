using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class CommService
    {
        /// <summary>
        /// G12Z024  downloading
        /// </summary>
        /// <param name="dn"></param>
        /// <returns></returns>
        public static DataTable GetShipInfoByDN(string dn)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT   '' ESN_DEC,
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
                                                     WHERE INVOICE = :INVOICE)) A,
                                (SELECT INVOICE, TCOM, CUST_PO,MODEL_NAME
                                   FROM SFISM4.R_BPCS_INVOICE_T
                                  WHERE INVOICE = :INVOICE) B,
                                 SFIS1.C_MODEL_DESC_T C
                          WHERE A.SHIP_NO = B.TCOM
                          AND C.MODEL_NAME = A.MODEL_NAME
                          ORDER BY A.SERIAL_NUMBER";

                OracleParameter parameter = new OracleParameter(":INVOICE", dn);
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return dt;
        }

        /// <summary>
        /// G12Z024  downloading
        /// </summary>
        /// <param name="dn"></param>
        /// <returns></returns>
        public static DataTable GetShipCodeByDN(string dn)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT DISTINCT A.MODEL_NAME,
                A.CUST_PN,
                A.CUST_PO,
                C.POE,
                C.SHIPPING_QTY,
                A.DN_NO,
                TO_CHAR(B.SHIPPING_TIME, 'YYYY/MM/DD') SHIPPING_TIME,
                E.BROADCOMPO,
                F.*
                FROM SFISM4.R_SAP_DN_DETAIL_T A,
                    SFISM4.P_TMP_CUSTOMER_T@LDBC.WORLD B,
                    SFISM4.R_BPCS_INVOICE_T C,
                    SFIS1.C_HPPO_T E,
                    (SELECT X.KP_NO,
                            X.DATE_CODE || X.LOT_CODE LotID,
                            COUNT(X.LOT_CODE) LotID_QTY
                        FROM MES4.R_TR_CODE_DETAIL@ALLPARTS.WORLD    X,
                            MES4.R_TR_PRODUCT_DETAIL@ALLPARTS.WORLD Y,
                            SFISM4.R_SN_TRSN_LINK_T                 Z,
                            MES4.R_CHIPPN_T@ALLPARTS.WORLD          W
                        WHERE Z.SERIAL_NUMBER IN
                            (SELECT SERIAL_NUMBER
                                FROM SFISM4.P_TMP_CUSTOMER_T@LDBC.WORLD
                                WHERE INVOICE IN
                                    (SELECT INVOICE || ' ' || INV_NO
                                        FROM SFISM4.R_BPCS_INVOICE_T
                                        WHERE INVOICE IN ({0})))
                        AND Z.PANEL_NO = Y.P_SN
                        AND X.TR_CODE = Y.TR_CODE
                        AND (X.P_NO = W.MODEL_NAME AND X.KP_NO = W.KP_NO)
                        GROUP BY X.KP_NO, X.DATE_CODE || X.LOT_CODE) F
                WHERE C.INVOICE IN ({0})
                AND B.INVOICE = C.INVOICE || ' ' || C.INV_NO
                AND A.DN_NO = C.INVOICE
                AND C.CUST_PO = E.CUST_PO", dn);

                dt = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return dt;
        }

        public static DataTable GetShipCSVByDN(string dn)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT   '26' factory,
                            A.CUSTOMER PART_NUMBER,
                            '' SYSCODE,
                            SUBSTR (C.SERIAL_NUMBER, 1, 14) IMEI,
                            SUBSTR (C.SERIAL_NUMBER, 15, 1) CD,
                            'CSTM' PHASE,
                            SUBSTR (C.SERIAL_NUMBER, 1, 14) BSN,
                            'COK' FINALCODE,
                            'LN930' BOARDID,
                            C.JOB SWVERSION,
                            '' SWVERSION2,
                            '' SVN,
                            TO_CHAR (C.WARRANTY_DATE, 'YYYY-MM-DD') TEST_DATE,
                            TO_CHAR (C.WARRANTY_DATE, 'HH24:MI:SS') TEST_HOUR,
                            '' PALLET,
                            ''  MULTIBOX,
                            C.MCARTON_NO BOX,
                            TO_CHAR (C.IN_STATION_TIME, 'YYYY-MM-DD') SHIP_DATE,
                            B.INVOICE DELIVERY_NOTE,
                            '' NOTE,
                            0 STATUS,
                            '' OTHER,
                            '' ICCID,
                            C.VERSION_CODE HWVERSION
                        FROM   SFISM4.R_BPCS_INVOICE_T B, SFISM4.Z_WIP_TRACKING_T C,SFISM4.R_SAP_DN_DETAIL_T D,SFIS1.C_MODEL_DESC_T A
                        WHERE   B.INVOICE = '{0}' AND B.INVOICE=D.DN_NO AND B.INV_NO=D.DN_ITEM_NO AND B.TCOM = C.SHIP_NO AND A.MODEL_NAME=B.MODEL_NAME ORDER BY C.MCARTON_NO", dn);
                //C.MCARTON_NO MULTIBOX,
                //  LPAD (CEIL(ROWNUM/720), 5, '0')||'SH'||TO_CHAR(IN_STATION_TIME,'DDMMYY') BOX,

                dt = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return dt;
        }

        public static DataTable GetShipXMLByDN(string dn, string model_name)
        {
            DataTable dt = null;

            try
            {

                string sql_pls62 = string.Format(@"SELECT distinct Z.SERIAL_NUMBER,
                                               SAPDN.CUST_PO PO_NO,
                                               Z.IMEI,
                                               Z.TRAY_NO,
                                               Z.MCARTON_NO,
                                               Z.MODEL_NAME AS MATNOFACTORY,
                                               CUST.SSN7 AS ACCKEY,
                                               '' AS A4,
                                               TO_CHAR (PR.IN_STATION_TIME, 'YYYYMMDDHHMMSS') AS BAG_TIME,
                                               TO_CHAR (PB.IN_STATION_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                               TO_CHAR (FT.IN_STATION_TIME, 'YYYYMMDD') AS FT_DATE,
                                               RB.INVOICE DN_NO,
                                               CONCAT (R117.YEA, R117.MONT) AS PRODUCT_DC,
                                               RB.DNP,
                                               TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS FT_DATE,
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
                                               TO_CHAR (Z.IN_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,
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
                                                 WHERE INVOICE = :DN AND ROWNUM = 1) RB,
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
                                                         AND B.INVOICE = :DN
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
                                               SFISM4.R_SAP_DN_DETAIL_T SAPDN
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
                                               AND CUST.SERIAL_NUMBER = Z.SERIAL_NUMBER");

                string sql_serval = string.Format(@"SELECT distinct Z.SERIAL_NUMBER,
                                                           SAPDN.CUST_PO PO_NO,
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
                                                           TO_CHAR (RMB.MO_KP_START_DATE, 'YYYYMMDD') AS FT_DATE,
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
                                                           TO_CHAR (Z.IN_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,
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
                                                             WHERE INVOICE = :DN AND ROWNUM = 1) RB,
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
                                                                     AND B.INVOICE = :DN
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
                                                           mes1.c_product_config@GZF12AP pro
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
                                                           AND PRO.P_NO = Z.MODEL_NAME");


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
                                                 WHERE INVOICE = :DN AND ROWNUM = 1) RB,
                                               SFISM4.Z_WIP_TRACKING_T Z,
                                               (  SELECT C.SERIAL_NUMBER, MIN (C.IN_LINE_TIME) IN_LINE_TIME
                                                    FROM SFISM4.Z_WIP_TRACKING_T A,
                                                         SFISM4.R_BPCS_INVOICE_T B,
                                                         SFISM4.R_SN_DETAIL_T C
                                                   WHERE     A.SHIP_NO = B.TCOM
                                                         AND B.INVOICE = :DN
                                                         AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                                GROUP BY C.SERIAL_NUMBER
                                                UNION
                                                  SELECT C.SERIAL_NUMBER, MIN (C.IN_LINE_TIME) IN_LINE_TIME
                                                    FROM SFISM4.Z_WIP_TRACKING_T A,
                                                         SFISM4.R_BPCS_INVOICE_T B,
                                                         SFISM4.H_SN_DETAIL_T C
                                                   WHERE     A.SHIP_NO = B.TCOM
                                                         AND B.INVOICE = :DN
                                                         AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                                GROUP BY C.SERIAL_NUMBER
                                                UNION
                                                  SELECT C.SERIAL_NUMBER, MIN (C.IN_LINE_TIME) IN_LINE_TIME
                                                    FROM SFISM4.Z_WIP_TRACKING_T A,
                                                         SFISM4.R_BPCS_INVOICE_T B,
                                                         SFISM4.H_SN_DETAIL_T C
                                                   WHERE     A.SHIP_NO = B.TCOM
                                                         AND B.INVOICE = :DN
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
                                               AND Z.MO_NUMBER = RMB.MO_NUMBER");
                /* :string.Format(@"SELECT DECODE (LENGTH (Z.SERIAL_NUMBER),
                                7, Z.SHIPPING_SN,
                                10, Z.SHIPPING_SN,
                                Z.SERIAL_NUMBER)
                           SERIAL_NUMBER,
                        Z.IMEI,
                        Z.MCARTON_NO,
                        DECODE (Z.TRAY_NO,
                                'N/A', '',
                                Z.SHIPPING_SN, '',
                                Z.SERIAL_NUMBER, '',
                                Z.TRAY_NO)
                           TRAY_NO,
                        RB.DNP,
                        CC.DIVICE_NUMBER || '-' || SUBSTR (RMB.SW_BOM, 1, 2)
                           AS CUST_MODEL_NAME,
                        RMB.HW_BOM AS BATCH_HEAT,
                        TO_CHAR (ADD_MONTHS (RB.FINISH_DATE, 12), 'YYYYMMDD')
                           AS WARRANTY_EXPIRED_DATE,
                        TO_CHAR (RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,
                        TO_CHAR (RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
                        RCR.PRODUCT_DATE,
                        RCR.PRODUCT_TIME,
                        RCR.PRODUCT_DC,
                        RCR.FT_DATE,
                        TO_CHAR (Z.OUT_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,
                        CC.CUST_MODEL_NAME PRODUCT_TYPE,
                        RCR.PANEL_NO,
                        RMB.SW_BOM AS SW,
                        RMB.OPTION_DESC AS MODULE_STATUS,
                        RCT.MPCKEY,
                        DECODE (
                           SUBSTR (Z.MODEL_NAME, 0, 4),
                           'T77H', '',
                           'EMS3', (SELECT PWDEBUG
                                      FROM SFIS1.R_CINTERION_TESTLOG_T
                                     WHERE    Z.SHIPPING_SN = SERIAL_NUMBER
                                           OR Z.SERIAL_NUMBER = SERIAL_NUMBER),
                           (SELECT ACCKEY
                              FROM SFIS1.R_CINTERION_TESTLOG_T
                             WHERE    Z.SHIPPING_SN = SERIAL_NUMBER
                                   OR Z.SERIAL_NUMBER = SERIAL_NUMBER))
                           ACCKEY,
                        RCR.TEST_LINE,
                        CC.DATA1 || '-' || CC.HW AS PART_NUMBER_SHIELD,
                        CC.DATA1 || '-' || SUBSTR (RMB.SW_BOM, 1, 2) AS PART_NUMBER_CUSTOMER,
                        LPAD (
                           TO_CHAR (
                                TRUNC (SYSDATE, 'dd')
                              + (RCR.BAG_TIME - NVL (RCR.BURN_OUTTIME, RCR.PANEL_IN_TIME)),
                              'HH24MISS'),
                           8,
                           '0')
                           AS FLOOR_TIME,
                        TO_CHAR (RCR.BAG_TIME, 'YYYYMMDDHH24MISS') AS BAG_TIME,
                        DECODE (SUBSTR (Z.MODEL_NAME, 0, 4), 'T77H', Z.SERIAL_NUMBER, '') A45,
                        DECODE (SUBSTR (Z.MODEL_NAME, 0, 4), 'T77H', RCT.ACCKEY, '') A46
                   FROM (SELECT FINISH_DATE,
                                TCOM,
                                INVOICE,
                                INV_NO,
                                MODEL_NAME,
                                ROWNUM * 10 AS DNP
                           FROM SFISM4.R_BPCS_INVOICE_T
                          WHERE INVOICE = '{0}' AND ROWNUM = 1) RB,
                        SFISM4.Z_WIP_TRACKING_T Z,
                        SFIS1.R_CINTERION_REPORT_T RCR,
                        SFIS1.R_CINTERION_TESTLOG_T RCT,
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
                        RMB
                  WHERE     RB.TCOM = Z.SHIP_NO
                        AND Z.SERIAL_NUMBER = RCR.SERIAL_NUMBER(+)
                        AND Z.SERIAL_NUMBER = RCT.SERIAL_NUMBER(+)
                        AND Z.MODEL_NAME = CC.MODEL_NAME
                        AND Z.MO_NUMBER = RMB.MO_NUMBER", dn);*/

                

                OracleParameter parameter = new OracleParameter(":DN", dn);

                if (model_name.Equals("PLS62-W"))
                {
                    dt = DBHelper.GetDataTable(sql_pls62, parameter);
                }
                else
                {
                    dt = DBHelper.GetDataTable(sql_serval, parameter);
                }

                // dt = DBHelper.GetDataTable(sql, null);                      
            }
            catch (Exception e)
            {

            }
            return dt;
        }

        public static int GetCountZ107(string dn)
        {
            string sql_check = @"SELECT A.* FROM SFISM4.Z107 A, SFISM4.R_BPCS_INVOICE_T B  WHERE SHIP_NO = TCOM AND INVOICE = '" + dn + "'";
            DataTable dt2 = new DataTable();
            dt2 = DBHelper.GetDataTable(sql_check, null);

            return dt2.Rows.Count;
        }

        public static DataTable GetCinterionBoxQty(string dn)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT TRAY_NO,COUNT(TRAY_NO) BOX_QTY FROM SFISM4.Z107
                                                                      WHERE SHIP_NO IN (SELECT TCOM FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE = '{0}')
                                                                           AND TRAY_NO LIKE 'DBF%'
                                                                      GROUP BY TRAY_NO
                                                                      HAVING COUNT(TRAY_NO) > 500", dn);
                dt = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return dt;
        }

        public static DataTable GetBPCSInvoiceInfo(string dn)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT INVOICE,TCOM FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE = '{0}' ", dn);
                dt = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return dt;
        }

        //        public static DataTable GetCinterionFile(string dn)
        //        {
        //            DataTable dt = null;
        //            try
        //            {
        //string sql = GetT77T943(dn) > 0 ? string.Format(@"SELECT Z.SERIAL_NUMBER,Z.IMEI,Z.MCARTON_NO,'' AS A4,RB.INVOICE DN_NO,RB.DNP,
        //                                               CC.DIVICE_NUMBER || '-' || SUBSTR(RMB.SW_BOM, 1, 2) AS CUST_MODEL_NAME,
        //                                               RMB.HW_BOM AS BATCH_HEAT,TO_CHAR(ADD_MONTHS(RB.FINISH_DATE, 12), 'YYYYMMDD') AS WARRANTY_EXPIRED_DATE,
        //                                               TO_CHAR(RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,TO_CHAR(RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
        //                                               TO_CHAR(R117.IN_LINE_TIME, 'YYYYMMDD') AS PRODUCT_DATE,TO_CHAR(R117.IN_LINE_TIME, 'HH24MISS') AS PRODUCT_TIME,
        //                                               '' AS A14,'' AS A15,TO_CHAR(Z.OUT_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,CC.CUST_MODEL_NAME PRODUCT_TYPE,
        //                                               '' AS A18,'' AS A19,'' AS A20,'' AS A21,'' AS A22,'' AS A23,'' AS A24,'' AS A25,'' AS A26,'' AS A27,
        //                                               '' AS A28,'' AS A29,'' AS A30,'' AS A31,'' AS A32,'' AS A33,'' AS A34,'' AS A35,'' AS A36,'' AS A37,
        //                                               'FOX' AS PRODUCT_LOCATION,CSNMAC.CUST_SN AS CUID,
        //                                               '' AS A40,'' AS A41,'' AS A42,'' AS A43,'' AS A44,'' AS A45,'' AS A46,'' AS A47,'' AS A48,'' AS A49,
        //                                               '' AS A50,'' AS A51,'' AS A52,'' AS A53,'' AS A54,'' AS A55,'' AS A56,'' AS A57,'' AS A58,'' AS A59,
        //                                               CSNMAC.UIM AS UIM,CSNMAC.ETHERNET_MAC AS MAC,CSNMAC.TELEPHONE_NUMBER AS MBSN,CSNMAC.IMEI AS MODULE_IMEI,
        //                                               CSNMAC.FW_VERSION AS FWVERSION
        //                                          FROM (SELECT FINISH_DATE,TCOM,INVOICE,INV_NO,MODEL_NAME,ROWNUM * 10 AS DNP FROM SFISM4.R_BPCS_INVOICE_T
        //                                                 WHERE INVOICE = :DN AND ROWNUM = 1) RB,
        //                                               SFISM4.Z_WIP_TRACKING_T Z,
        //                                               (SELECT C.SERIAL_NUMBER, MIN(C.IN_LINE_TIME) IN_LINE_TIME
        //                                                  FROM SFISM4.Z_WIP_TRACKING_T A,SFISM4.R_BPCS_INVOICE_T B,SFISM4.R_SN_DETAIL_T    C
        //                                                 WHERE A.SHIP_NO = B.TCOM AND B.INVOICE = :DN AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
        //                                                 GROUP BY C.SERIAL_NUMBER
        //                                                UNION
        //                                                SELECT C.SERIAL_NUMBER, MIN(C.IN_LINE_TIME) IN_LINE_TIME
        //                                                  FROM SFISM4.Z_WIP_TRACKING_T A,SFISM4.R_BPCS_INVOICE_T B,SFISM4.H_SN_DETAIL_T    C
        //                                                 WHERE A.SHIP_NO = B.TCOM AND B.INVOICE = :DN AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
        //                                                 GROUP BY C.SERIAL_NUMBER
        //                                                UNION
        //                                                SELECT C.SERIAL_NUMBER, MIN(C.IN_LINE_TIME) IN_LINE_TIME
        //                                                  FROM SFISM4.Z_WIP_TRACKING_T A,SFISM4.R_BPCS_INVOICE_T B,SFISM4.H_SN_DETAIL_T@DB163.WORLD C
        //                                                 WHERE A.SHIP_NO = B.TCOM AND B.INVOICE = :DN AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
        //                                                 GROUP BY C.SERIAL_NUMBER) R117,
        //                                               SFISM4.R_T77T943_CSNMAC_T CSNMAC,
        //                                               (SELECT MODEL_NAME,DIVICE_NUMBER,CUST_MODEL_NAME,DATA1,DATA2,DATA3,DATA4,HW,INFO06
        //                                                  FROM SFIS1.C_CINTERION_SHIP_T) CC,
        //                                               (SELECT MO_NUMBER, SW_BOM, HW_BOM, OPTION_DESC
        //                                                  FROM SFISM4.R_MO_BASE_T WHERE MODEL_NAME IN
        //                                                (SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T)) RMB
        //                                         WHERE RB.TCOM = Z.SHIP_NO AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+) AND Z.SERIAL_NUMBER = CSNMAC.SERIAL_NUMBER(+)
        //                                           AND Z.MODEL_NAME = CC.MODEL_NAME AND Z.MO_NUMBER = RMB.MO_NUMBER")
        //: string.Format(@"SELECT DECODE(LENGTH(Z.SERIAL_NUMBER),7, Z.SHIPPING_SN,10, Z.SHIPPING_SN,Z.SERIAL_NUMBER) SERIAL_NUMBER,
        //                       Z.IMEI,Z.MCARTON_NO,DECODE(Z.TRAY_NO, 'N/A', '',Z.SHIPPING_SN, '',Z.SERIAL_NUMBER, '',Z.TRAY_NO) TRAY_NO,
        //                       RB.INVOICE DN_NO, RB.DNP, CC.DIVICE_NUMBER || '-' || SUBSTR(RMB.SW_BOM, 1, 2) AS CUST_MODEL_NAME,
        //                       RMB.HW_BOM AS BATCH_HEAT,TO_CHAR(ADD_MONTHS(RB.FINISH_DATE, 12), 'YYYYMMDD') AS WARRANTY_EXPIRED_DATE,
        //                       TO_CHAR(RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,TO_CHAR(RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
        //                       RCR.PRODUCT_DATE,RCR.PRODUCT_TIME,RCR.PRODUCT_DC,RCR.FT_DATE,TO_CHAR(Z.OUT_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,
        //                       CC.CUST_MODEL_NAME PRODUCT_TYPE,Z.MODEL_NAME,'' AS A19,RCR.PANEL_NO,RMB.SW_BOM AS SW,RMB.OPTION_DESC AS MODULE_STATUS,
        //                       '' AS A23,'' AS A24,'' AS A25,'' AS A26,'' AS A27,RCT.MPCKEY,'' AS A29,'' AS A30,'' AS A31,'' AS A32,
        //                       '' AS A33,DECODE(SUBSTR(Z.MODEL_NAME,0,4), 'T77H', '', RCT.ACCKEY) ACCKEY,'9999','Cinterion Wireless Modules',
        //                       RCR.TEST_LINE,'FOX',CC.DATA1 || '-' || CC.HW AS PART_NUMBER_SHIELD,CC.DATA1 || '-' || SUBSTR(RMB.SW_BOM, 1, 2) AS PART_NUMBER_CUSTOMER,
        //                       LPAD(TRUNC((RCR.BAG_TIME - NVL(RCR.BURN_OUTTIME, RCR.PANEL_IN_TIME)) * 24,0) ||LPAD(TRUNC(((RCR.BAG_TIME -
        //                       NVL(RCR.BURN_OUTTIME, RCR.PANEL_IN_TIME)) * 24 - TRUNC((RCR.BAG_TIME - NVL(RCR.BURN_OUTTIME, RCR.PANEL_IN_TIME)) * 24,
        //                       0)) * 60, 0), 2, '0') || LPAD(TRUNC((((RCR.BAG_TIME - NVL(RCR.BURN_OUTTIME, RCR.PANEL_IN_TIME)) * 24 -
        //                       TRUNC((RCR.BAG_TIME - NVL(RCR.BURN_OUTTIME, RCR.PANEL_IN_TIME)) * 24,0)) * 60 - TRUNC(((RCR.BAG_TIME -
        //                       NVL(RCR.BURN_OUTTIME, RCR.PANEL_IN_TIME)) * 24 - TRUNC((RCR.BAG_TIME - NVL(RCR.BURN_OUTTIME, RCR.PANEL_IN_TIME)) * 24,
        //                       0)) * 60, 0)) * 60, 0), 2, '0'), 8, '0') AS FLOOR_TIME,DECODE(LENGTH(Z.SERIAL_NUMBER),10,Z.SERIAL_NUMBER,'')  AS A42,
        //                       '' AS A43,TO_CHAR(RCR.BAG_TIME, 'YYYYMMDDHH24MISS') AS BAG_TIME,DECODE(SUBSTR(Z.MODEL_NAME,0,4),'T77H',Z.SERIAL_NUMBER,'')  A45,
        //                       DECODE(SUBSTR(Z.MODEL_NAME,0,4),'T77H','',RCT.ACCKEY,'')  A46,RCT.FLASH_KEY,CC.DATA2,'' AS A49,CC.CUST_MODEL_NAME PRODUCT_TYPE,
        //                       '' AS A51,CC.DATA3,CC.DATA4,CC.INFO06 AS A54,'' AS A55,'' AS A56,'' AS A57,'' AS A58
        //                       FROM (SELECT FINISH_DATE,TCOM,INVOICE,INV_NO,MODEL_NAME,ROWNUM * 10 AS DNP FROM SFISM4.R_BPCS_INVOICE_T
        //                         WHERE INVOICE = :DN AND ROWNUM = 1) RB,SFISM4.Z_WIP_TRACKING_T Z,SFISM4.R_CINTERION_REPORT_T RCR,
        //                       SFISM4.R_CINTERION_TESTLOG_T RCT,
        //                       (SELECT MODEL_NAME,DIVICE_NUMBER,CUST_MODEL_NAME,DATA1,DATA2,DATA3,DATA4,HW,INFO06
        //                          FROM SFIS1.C_CINTERION_SHIP_T) CC,
        //                       (SELECT MO_NUMBER, SW_BOM, HW_BOM, OPTION_DESC
        //                          FROM SFISM4.R_MO_BASE_T
        //                         WHERE MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T)) RMB
        //                   WHERE RB.TCOM = Z.SHIP_NO
        //                   AND Z.SERIAL_NUMBER = RCR.SERIAL_NUMBER(+)
        //                   AND Z.SERIAL_NUMBER = RCT.SERIAL_NUMBER(+)
        //                   AND Z.MODEL_NAME = CC.MODEL_NAME
        //                   AND Z.MO_NUMBER = RMB.MO_NUMBER");

        //                OracleParameter parameter = new OracleParameter(":DN", dn);
        //                dt = DBHelper.GetDataTable(sql, parameter);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception(ex.ToString());
        //            }
        //            return dt;
        //        }

        public static DataTable GetCinterionFile(string dn, string type = "NORMAL")
        {
            DataTable dt = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("INVOICE",OracleDbType.Varchar2,25),
                    new OracleParameter("DOWNLOAD_TYPE", OracleDbType.Varchar2, 25),
                    new OracleParameter("RE_CURSOR",OracleDbType.RefCursor)
                };
                parameter[0].Direction = ParameterDirection.Input;
                parameter[1].Direction = ParameterDirection.Input;
                parameter[2].Direction = ParameterDirection.Output;
                parameter[0].Value = dn;
                parameter[1].Value = type;
                dt = DBHelper.RunProcedureTable("SFIS1.CHECK_GEMALTO_SHIP_FILE.GET_SHIP_DATASET", parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return dt;
        }

        public void exportXMLFile()
        {

        }

        public static DataTable GetCinterion(string model_name)
        {
            DataTable dt = null;
            try
            {
                string sql = "SELECT * FROM SFIS1.C_CINTERION_SHIP_T ORDER BY MODEL_NAME";
                if (model_name != null)
                {
                    sql = string.Format("SELECT * FROM SFIS1.C_CINTERION_SHIP_T WHERE MODEL_NAME='{0}' ORDER BY MODEL_NAME", model_name);
                }
                dt = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return dt;
        }

        public static int GetT77T943(string invoice)
        {
            int count;
            try
            {
                string sql = @" SELECT COUNT(1) FROM SFISM4.R_T77T943_CSNMAC_T A,SFISM4.R_BPCS_INVOICE_T B
                                WHERE A.MODEL_NAME=B.MODEL_NAME AND B.INVOICE= :DN ";
                OracleParameter parameter = new OracleParameter(":DN", invoice);
                count = int.Parse(DBHelper.ExecuteSclar(sql, parameter).ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return count;
        }

        public static string GetModelName(string invoice)
        {
            string model;
            try
            {
                string sql = @" SELECT MODEL_NAME FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE= :DN ";
                OracleParameter parameter = new OracleParameter(":DN", invoice);
                model = DBHelper.ExecuteSclar(sql, parameter).ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return model;
        }

        public static string GetMulModelName(string invoice)
        {
            string model;
            try
            {
                string sql = string.Format(@" SELECT DISTINCT MODEL_NAME FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE IN  {0} ", invoice);
                model = DBHelper.ExecuteSclar(sql, null).ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return model;
        }

        public static DataTable GetModelType(string invoice)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT DISTINCT UPPER(MODEL_SERIAL) MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T 
                                WHERE MODEL_NAME IN (SELECT MODEL_NAME FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE IN  {0} ) ", invoice);

                dt = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static string GetCartonPrefix(string model)
        {
            string pre;
            try
            {
                string sql = "SELECT CUST_CARTON_PREFIX FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME= :MODEL AND ROWNUM=1 ";
                OracleParameter parameter = new OracleParameter(":MODEL", model);
                pre = DBHelper.ExecuteSclar(sql, parameter).ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return pre;
        }

        public static string CheckFileSP(string invoice)
        {
            string res;
            try
            {
                OracleParameter[] parameter = new OracleParameter[2]
                {
                    //注意:第一個參數只能是函數的返回值(這是花了1.5天才找到的原因)，可自行命名。
                    new OracleParameter("ReturnValue", OracleDbType.Varchar2, 100, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null),
                    new OracleParameter("INVOICE",OracleDbType.Varchar2,15),
                };
                //parameter[0].Direction = ParameterDirection.ReturnValue;
                //parameter[1].Direction = ParameterDirection.Input;

                parameter[1].Value = invoice;

                res = DBHelper.RunFunction("SFIS1.CHECK_GEMALTO_SHIP_FILE.GET_CHECK_SHIP_FILE_RES", parameter);

               /* string sql_check = @"SELECT Z.SERIAL_NUMBER,
                                     Z.IMEI,
                                     Z.MCARTON_NO,
                                     RB.DNP,
                                     CC.DIVICE_NUMBER || '-' || SUBSTR(RMB.SW_BOM, 1, 2) AS CUST_MODEL_NAME,
                                     RMB.HW_BOM AS BATCH_HEAT,
                                     TO_CHAR(ADD_MONTHS(RB.FINISH_DATE, 12), 'YYYYMMDD') AS WARRANTY_EXPIRED_DATE,
                                     TO_CHAR(RB.FINISH_DATE, 'YYYYMMDD') AS DELIVERY_DATE,
                                     TO_CHAR(RB.FINISH_DATE, 'HH24MMSS') AS DELIVERY_TIME,
                                     TO_CHAR(R117.IN_LINE_TIME, 'YYYYMMDD') AS PRODUCT_DATE,
                                     TO_CHAR(R117.IN_LINE_TIME, 'HH24MISS') AS PRODUCT_TIME,
                                     TO_CHAR(Z.OUT_LINE_TIME, 'YYYYMMDD') AS PACKING_DATE,
                                     CC.CUST_MODEL_NAME PRODUCT_TYPE,
                                     CSNMAC.CUST_SN AS CUID,
                                     CSNMAC.UIM AS UIM,
                                     CSNMAC.ETHERNET_MAC AS MAC,
                                     CSNMAC.TELEPHONE_NUMBER AS MBSN,
                                     CSNMAC.IMEI AS MODULE_IMEI,
                                     CSNMAC.FW_VERSION AS FWVERSION,
                                     DNDETAIL.CUST_PO AS PO_NO,
                                     CASE
                                        WHEN CSNMAC.MODEL_NAME = 'PLS62-W' THEN
                                         RPAD('U' || CSNMAC.CUST_SN || 'SM-NLTETH0' || CSNMAC.FW_VERSION || CASE
                                        WHEN LENGTH(CSNMAC.TELEPHONE_NUMBER) < 20 THEN
                                         RPAD(CSNMAC.TELEPHONE_NUMBER, 20)
                                        ELSE
                                         TO_CHAR(CSNMAC.TELEPHONE_NUMBER)
                                      END || CASE
                                        WHEN LENGTH(CSNMAC.ETHERNET_MAC) < 16 THEN
                                         RPAD(CSNMAC.ETHERNET_MAC, 16)
                                        ELSE
                                         CSNMAC.ETHERNET_MAC
                                      END || CASE
                                        WHEN LENGTH(CSNMAC.WISUNRF_MAC) < 16 THEN
                                         RPAD(CSNMAC.WISUNRF_MAC, 16)
                                        ELSE
                                         CSNMAC.WISUNRF_MAC
                                      END, 93) WHEN CSNMAC.MODEL_NAME = 'T77T943.01' THEN RPAD('U' || CSNMAC.CUST_SN || 'SM-LTET00                ' || --ROUTE A MAC為空16碼長
                                     CASE
                                       WHEN LENGTH(CSNMAC.IMEI) < 15 THEN
                                        RPAD(CSNMAC.IMEI, 15)
                                       ELSE
                                        TO_CHAR(CSNMAC.IMEI)
                                     END || CASE
                                       WHEN LENGTH(CSNMAC.ETHERNET_MAC) < 18-- - ETHERNET_MAC長度16碼，不夠補空格,Smart meterID 2碼空格
                                         THEN
                                        RPAD(CSNMAC.ETHERNET_MAC, 18)
                                       ELSE
                                        TO_CHAR(CSNMAC.ETHERNET_MAC)
                                     END || CASE
                                       WHEN LENGTH(CSNMAC.FW_VERSION) < 16 THEN
                                        RPAD(CSNMAC.FW_VERSION, 16)
                                       ELSE
                                        CSNMAC.FW_VERSION
                                     END || CASE
                                       WHEN LENGTH(CSNMAC.WISUNRF_MAC) < 16 THEN
                                        RPAD(CSNMAC.WISUNRF_MAC, 16)
                                       ELSE
                                        CSNMAC.WISUNRF_MAC
                                     END, 122) END QR_CODE,
                                     REEL.ACTIVDATE AS ACTIVDATE,
                                     TRIM(REEL.REEL_NUM) AS DOCREEL
                                FROM(SELECT FINISH_DATE,
                                             TCOM,
                                             INVOICE,
                                             INV_NO,
                                             MODEL_NAME,
                                             ROWNUM * 10 AS DNP
                                        FROM SFISM4.R_BPCS_INVOICE_T
                                       WHERE INVOICE = '5142002352' 
                                         AND ROWNUM = 1) RB,
                                     SFISM4.Z_WIP_TRACKING_T Z,
                                     (SELECT C.SERIAL_NUMBER, MIN(C.IN_LINE_TIME) IN_LINE_TIME
                                        FROM SFISM4.Z_WIP_TRACKING_T A,
                                             SFISM4.R_BPCS_INVOICE_T B,
                                             SFISM4.R_SN_DETAIL_T    C
                                       WHERE A.SHIP_NO = B.TCOM
                                         AND B.INVOICE = '5142002352' 
                                         AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                       GROUP BY C.SERIAL_NUMBER
                                      UNION
                                      SELECT C.SERIAL_NUMBER, MIN(C.IN_LINE_TIME) IN_LINE_TIME
                                        FROM SFISM4.Z_WIP_TRACKING_T A,
                                             SFISM4.R_BPCS_INVOICE_T B,
                                             SFISM4.H_SN_DETAIL_T    C
                                       WHERE A.SHIP_NO = B.TCOM
                                         AND B.INVOICE = '5142002352' 
                                         AND A.SERIAL_NUMBER = C.SERIAL_NUMBER
                                       GROUP BY C.SERIAL_NUMBER ) R117,
                                     SFIS1.R_T77T943_CSNMAC_T CSNMAC,
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
                                     (SELECT MO_NUMBER, SW_BOM, HW_BOM, OPTION_DESC
                                        FROM SFISM4.R_MO_BASE_T
                                       WHERE MODEL_NAME IN
                                             (SELECT MODEL_NAME FROM SFIS1.C_CINTERION_SHIP_T)) RMB,
                                     SFISM4.R_SAP_DN_DETAIL_T DNDETAIL,
                                     SFIS1.R_T77T943_CSN_REEL_T REEL
                               WHERE RB.TCOM = Z.SHIP_NO
                                 AND Z.SERIAL_NUMBER = R117.SERIAL_NUMBER(+)
                                 AND Z.SERIAL_NUMBER = CSNMAC.SERIAL_NUMBER(+)
                                 AND CSNMAC.TELEPHONE_NUMBER = REEL.TELEPHONE_NUMBER(+)
                                 AND Z.MODEL_NAME = CC.MODEL_NAME
                                 AND Z.MO_NUMBER = RMB.MO_NUMBER
                                 AND RB.INVOICE = DNDETAIL.DN_NO(+)";*/

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return res;
        }

    }
}
