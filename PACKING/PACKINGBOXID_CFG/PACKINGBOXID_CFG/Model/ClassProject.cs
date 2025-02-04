using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACKINGBOXID_CFG.Model
{
    class ClassProject
    {

    }
    public class StationName
    {
        public string SECTION_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string STATION_NAME { get; set; }
    }
    public class LineName
    {
        public string LINE_NAME { get; set; }
    }
    public class TSSN_RULE
    {
        public string SSN { get; set; }
        public bool SSN_OK { get; set; }
        public string cSSN_PREFIX { get; set; }
        public string cSSN_POSTFIX { get; set; }
        public string cSSN_STR { get; set; }
        public string sSHIPPINGSN_CODE { get; set; }
        public string sCHECK_SSN_RULE { get; set; }
        public string sCHECK_SSN_RANGE { get; set; }
        public string sCHECK_SSN { get; set; }
        public int nSSN_LENGTH { get; set; }
        public int nSSN_PREFIX_LEN { get; set; }
        public int nSSN_POSTFIX_LEN { get; set; }
        public string sCOMPARE_SSN { get; set; }
        public int nSSN_Self_StartDigit { get; set; }
        public int nSSN_Self_FlowNO { get; set; }
        public int nSSN_Compare_StartDigit { get; set; }
        public int nSSN_Compare_FlowNO { get; set; }

    }
    public class TMAC_RULE
    {
        public string MAC { get; set; }
        public bool MAC_OK { get; set; }
        public string cMAC_PREFIX { get; set; }
        public string cMAC_POSTFIX { get; set; }
        public string cMAC_STR { get; set; }
        public string sMACID_CODE { get; set; }
        public string sCHECK_MAC_RULE { get; set; }
        public string sCHECK_MAC { get; set; }
        public string sCHECK_MAC_RANGE { get; set; }
        public int nMAC_LENGTH { get; set; }
        public int nMAC_PREFIX_LEN { get; set; }
        public int nMAC_POSTFIX_LEN { get; set; }
        public string sCOMPARE_MAC { get; set; }
        public int nMAC_Self_StartDigit { get; set; }
        public int nMAC_Self_FlowNO { get; set; }
        public int nMAC_Compare_StartDigit { get; set; }
        public int nMAC_Compare_FlowNO { get; set; }
    }
    public class R107
    {
        public string SERIAL_NUMBER { get; set; }
        public string WIP_GROUP { get; set; }
        public string TRAY_NO { get; set; }
        public string TRACK_NO { get; set; }
        public string SCRAP_FLAG { get; set; }
        public string MO_NUMBER { get; set; }
        public string MODEL_NAME { get; set; }
        public string SECTION_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public string SHIPPING_SN { get; set; }
        public string SHIPPING_SN2 { get; set; }
        public string VERSION_CODE { get; set; }
        public string FINISH_FLAG { get; set; }
        public string CUSTOMER_NO { get; set; }
        public string GROUP_NAME_CQC { get; set; }
        public string MCARTON_NO { get; set; }

    }
    public class R105
    {
        public string MODEL_NAME { get; set; }
        public string BOM_NO { get; set; }
        public string INPUT_QTY { get; set; }
        public string TARGET_QTY { get; set; }
        public string VERSION_CODE { get; set; }
        public string MO_TYPE { get; set; }
        public string CLOSE_FLAG { get; set; }
        public string MSN_MO_OPTION { get; set; }
    }
    public class PACK_PARAM
    {
        public int TRAY_QTY { get; set; }
    }
    public class R108
    {
        public string SERIAL_NUMBER { get; set; }
        public string KEY_PART_SN { get; set; }
        public string KEY_PART_NO { get; set; }
        public string KP_RELATION { get; set; }
        public string KP_CODE { get; set; }
        public string MACPREFIX { get; set; }
    }
    public class H108
    {
        public string SERIAL_NUMBER { get; set; }
        public string KEY_PART_SN { get; set; }
        public string KEY_PART_NO { get; set; }
        public string KP_RELATION { get; set; }
        public string KP_CODE { get; set; }

    }
    public class PACK_SEQUENCE
    {
        public string CUSTSN_NAME { get; set; }
        public string MODEL_NAME { get; set; }
        public string VERSION_CODE { get; set; }
        public string MO_TYPE { get; set; }
    }
    public class SOURCE
    {
        public string STEP { get; set; }
        public string SN { get; set; }
        public string EQUAL { get; set; }
        public string PREFIX { get; set; }
        public string LENGTH { get; set; }
    }
    public class CUST_RULE
    {
        public string MO_NUMBER { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string CUSTSN_CODE { get; set; }
        public string CUSTSN_PREFIX { get; set; }
        public string CUSTSN_POSTFIX { get; set; }
        public string CUSTSN_STR { get; set; }
        public string SHIPPINGSN_CODE { get; set; }
        public string CHECK_RULE_NAME { get; set; }
        public string CHECK_RANGE { get; set; }
        public string CHECK_SSN { get; set; }
        public string CUSTSN_LENG { get; set; }
        public string COMPARE_SN { get; set; }
        public int CUSTSN_START { get; set; }
        public int CUSTSN_END { get; set; }
        public int COMPARE_SN_START { get; set; }
        public int COMPARE_SN_END { get; set; }

    }
    public class BOM
    {
        public int KP_RELATION { get; set; }
        public string KEY_PART_NO { get; set; }
    }
    public class R_CUSTSN
    {
        public string MAC1 { get; set; }
        public string MAC2 { get; set; }
        public string MAC3 { get; set; }
        public string MAC4 { get; set; }
        public string MAC5 { get; set; }
        public string MAC6 { get; set; }
        public string MAC7 { get; set; }
        public string MACID7 { get; set; }
        public string MAC8 { get; set; }
        public string MAC9 { get; set; }
        public string MAC10 { get; set; }
        public string SSN1 { get; set; }
        public string SSN2 { get; set; }
        public string SSN3 { get; set; }
        public string SSN4 { get; set; }
        public string SSN5 { get; set; }
        public string SSN6 { get; set; }
        public string SSN7 { get; set; }
        public string SSN8 { get; set; }
        public string SSN9 { get; set; }
        public string SSN10 { get; set; }
        public string SSN11 { get; set; }
        public string SSN12 { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string MO_NUMBER { get; set; }
    }

    public class CUST_SNRULE
    {
        public string CUST_MODEL_NAME { get; set; }
        public string CUST_MODEL_DESC { get; set; }
        public string UPCEANDATA { get; set; }
        public string MODEL_NAME { get; set; }
    }
    public class C_WORK_DESC
    {
        public string DAY_DISTINCT { get; set; }
    }
    public class R_STATION_REC
    {
        public string TOTAL_QTY { get; set; }
    }
    public class T_TESTSN
    {
        public string SERIAL_NUMBER { get; set; }
        public string TEST_RESULT { get; set; }
        public string ERROR_CODE { get; set; }
        public string SFC_RESULT { get; set; }
        public bool FIRST_SN { get; set; }
        public bool LAST_SN { get; set; }

    }
    public class C104
    {
        public string MODEL_TYPE { get; set; }
        public string SITE { get; set; }
    }
    public class SOURCE_BOXID
    {
        public string NO { get; set; }
        public string MODEL_NAME { get; set; }
        public string MO_NUMBER { get; set; }
        public string VERSION_CODE { get; set; }
        public string BOX_ID { get; set; }
        public string QTY { get; set; }

    }
}
