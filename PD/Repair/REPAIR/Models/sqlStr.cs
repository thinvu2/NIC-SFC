using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPAIR.Models
{
    class sqlStr
    {
        public const string getDataRepair = " SELECT DISTINCT A.Repair_Time, " +
                              " A.Reason_Code, " +
                              " C.Reason_Desc, " +
                              " A.Error_Item_Code Location, " +
                              " D.Item_Name Location_description, " +
                              " A.Duty_Type, " +
                              " B.Duty_Desc, " +
                              " A.Duty_Station, " +
                              " A.Repair_Status, " +
                              " A.Record_Type, " +
                              " A.Tester, " +
                              " A.memo, " +
                              " A.Supplier, " +
                              " A.EC_ext, " +
                              " A.Test_Time, " +
                              " A.Test_Code, " +
                              " A.Test_Section, " +
                              " A.Test_Group, " +
                              " A.Test_Station, " +
                              " A.Test_Line, " +
                              " A.Repair_Station, " +
                              " A.Repairer, " +
                              " A.ROWID, " +
                              " A.DATE_CODE " +
                              " FROM   SFISM4.R109 A, " +
                              " SFIS1.C_DUTY_T B, " +
                              " SFIS1.C_REASON_CODE_T C, " +
                              " SFIS1.C_ITEM_DESC_T D " +
                              " WHERE A.REASON_CODE = C.REASON_CODE(+) " +
                              " AND A.DUTY_TYPE = B.DUTY_TYPE (+) " +
                              " AND A.SERIAL_NUMBER ='{0}' " +
                              " AND D.ITEM_CODE(+) = A.Error_Item_Code " +
                              " AND(A.MODEL_NAME = D.MODEL_NAME(+)) " +
                              " AND A.TEST_CODE   = '{1}' " +
                              " AND A.TEST_STATION = '{2}' " +
                              " AND A.TEST_TIME   = TO_DATE ('{3}' , 'YYYY/MM/DD HH24:MI:SS') " +
                              " order by A.Repair_time";

        public const string qryTopReason = " select reason_code,item_code,happen_cnt " +
                                       " from sfism4.R_REASON_STATISTIC_T " +
                                       " where error_code='{0}' " +
                                       " and model_name='{1}' " +
                                       "  order by happen_cnt DESC " ;

        public const string qryTableDuty = "SELECT * FROM SFIS1.C_DUTY_T";

        public const string qryDuty = "SELECT * FROM SFIS1.C_DUTY_T WHERE duty_type = '{0}' or duty_desc = '{0}'  ";

        public const string qryReasonCode = " select REASON_CODE ,REASON_TYPE ,REASON_DESC ,REASON_DESC2 ,DUTY_STATION " +
                               " from SFIS1.C_REASON_CODE_T " +
                               " order by REASON_CODE ";
        public const string qryReasonCode2 = " select REASON_CODE ,REASON_TYPE ,REASON_DESC ,REASON_DESC2 ,DUTY_STATION " +
                       " from SFIS1.C_REASON_CODE_T WHERE REASON_CODE LIKE  '{0}%' " +
                       " order by REASON_CODE ";

        public const string qryAllError = " SELECT TEST_CODE,TEST_STATION ,reason_code, TO_CHAR(TEST_TIME,'YYYY/MM/DD HH24:MI:SS') TEST_TIME ,record_type  FROM ( " +
                                   " SELECT* FROM   sfism4.r_repair_t " +
                          " UNION SELECT* FROM   sfism4.h_repair_t) " + 
                          " WHERE serial_number ='{0}' " +
                          " AND(REPAIR_STATUS <> 'D'   OR REPAIR_STATUS IS NULL) ORDER BY TEST_TIME ASC ";
        public const string qryListNormalError = "  Select TEST_CODE,TEST_STATION ,reason_code, TO_CHAR(TEST_TIME,'YYYY/MM/DD HH24:MI:SS') TEST_TIME ,record_type " +
                               " from (select * from sfism4.r_repair_t "+
                               "      UNION select * from sfism4.h_repair_t ) "+
                               " where Serial_number='{0}' AND "+
                               "       (REPAIR_STATUS <> 'D' OR REPAIR_STATUS is NULL) AND "+
                               "       (TEST_GROUP ='{1}' OR "+
                               "        Test_Group IN (select group_name "+
                               "                       from  sfis1.C_ROUTE_CONTROL_T C110 "+
                               "                       where C110.group_next= '{1}' AND "+
                               "                             C110.state_flag=1 AND "+
                               "                             C110.route_code= '{2}')) ORDER BY TEST_TIME ASC ";
        public const string qryCheckReason = " SELECT * FROM SFIS1.C_REASON_CODE_T WHERE REASON_CODE = '{0}' ";
        public const string qryItemCode = " SELECT Item_Code, Item_name,Item_serial,SIDE,MFR_CODE,MFR_NAME from SFIS1.C_ITEM_DESC_T " +
                                " WHERE MODEL_NAME ='{0}' or MODEL_NAME='DEFAULT' " +
                                " ORDER BY ITEM_CODE ";
        public const string qryItemCode2 = " SELECT Item_Code, Item_name,Item_serial,SIDE,MFR_CODE,MFR_NAME from SFIS1.C_ITEM_DESC_T " +
                           " WHERE ( MODEL_NAME ='{0}'  or MODEL_NAME='DEFAULT' ) AND Item_Code LIKE '{1}%' " +
                           " ORDER BY ITEM_CODE ";
        public const string qryR108 = " SELECT A.KEY_PART_NO , A.KEY_PART_SN , B.KP_NAME FROM SFISM4.R108 A , SFIS1.C_KEYPARTS_DESC_T B WHERE A.SERIAL_NUMBER  = '{0}' AND A.KEY_PART_NO = B.KEY_PART_NO";
        //
        public const string qryDcLc = @"SELECT DISTINCT B.DATE_CODE,
                                                        B.LOT_CODE,
                                                        D.MFR_NAME
                                        FROM   (SELECT A.KP_NO,
                                                       DATE_CODE,
                                                       STATION,
                                                       TR_CODE,
                                                       LOT_CODE,
                                                       MFR_CODE,
                                                       MFR_KP_NO,
                                                       TR_SN,
                                                       A.WO,
                                                       location
                                                FROM   MES4.R_TR_CODE_DETAIL@vnap A,
                                                       MES1.C_SMT_AP_LOCATION@vnap B
                                                WHERE  1 = 1
                                                       AND A.SMT_CODE = B.SMT_CODE(+)
                                                       AND A.KP_NO = B.KP_NO(+)
                                                       AND A.STATION_FLAG = '1'
                                                       AND location = '{1}'
                                                       AND A.P_NO IN (SELECT P_NO
                                                                      FROM   MES1.C_PRODUCT_CONFIG@vnap
                                                                      WHERE  1 = 1)) B,
                                               MES4.R_TR_PRODUCT_DETAIL@vnap C,
                                               MES1.C_MFR_CONFIG@vnap D,
                                               mes4.r_sn_link@vnap G
                                        WHERE  C.TR_CODE = B.TR_CODE
                                               AND B.MFR_CODE = D.MFR_CODE
                                               AND g.p_Sn = c.p_Sn
                                               AND g.wo = c.wo
                                               AND C.WO = B.WO
                                               AND b.location = '{1}'
                                               AND c.p_sn = '{0}'
                                        UNION
                                        SELECT DISTINCT B.DATE_CODE,
                                                        B.LOT_CODE,
                                                        D.MFR_NAME
                                        FROM   MES4.R_TR_PRODUCT_DETAIL @VNAP A,
                                               MES4.R_GRN @VNAP E,
                                               MES1.C_VENDER_CONFIG @VNAP F,
                                               MES4.R_TR_CODE_DETAIL @VNAP B,
                                               MES1.C_MFR_CONFIG@vnap D,
                                               MES1.C_STATION_KP @VNAP C
                                        WHERE  A.P_SN = '{0}'
                                               AND LOCATION = '{1}'
                                               AND C.KP_NO = B.KP_NO
                                               AND B.P_NO = C.P_NO
                                               AND A.TR_CODE = B.TR_CODE
                                               AND B.MFR_KP_NO = E.MFR_KP_NO
                                               AND E.VENDOR_CODE = F.VENDER_CODE
                                               AND b.mfr_code = d.mfr_code 
                                        ";
        public const string qryGetTempAllpart = @"SELECT A.LOT_CODE,
                                                         A.DATE_CODE,
                                                         B.MFR_NAME
                                                  FROM   MES4.R_TR_SN@VNAP A,
                                                         MES1.C_MFR_CONFIG@VNAP B
                                                  WHERE  A.MFR_CODE = B.MFR_CODE
                                                         AND A.TR_SN = '{0}' ";
    }
}
