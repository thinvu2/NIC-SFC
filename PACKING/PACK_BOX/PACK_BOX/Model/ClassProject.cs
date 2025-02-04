using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACK_BOX.Model
{
    public class LineName
    {
        public string SECTION_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public string LINE_NAME { get; set; }
        public LineName() { }
        public LineName(string section_name, string group_name, string station_name, string line_name)
        {
            this.SECTION_NAME = section_name;
            this.GROUP_NAME = group_name;
            this.STATION_NAME = station_name;
            this.LINE_NAME = line_name;
        }
    }
    public class TSSN_RULE
    {
        public string SSN { get; set; }
        public bool SSN_OK { get; set; }

        public string cSSN_PREFIX { get; set; }

        public string cSSN_POSTFIX { get; set; }

        public string cSSN_STR { get; set; }

        public string sSHIPPINGSN_CODE { get; set; }

        public string sSHIPPINGSN_CODE2 { get; set; }

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

        public TSSN_RULE() { }

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
        public int nMAC_LENGTH { get; set; }
        public string sCHECK_MAC_RANGE { get; set; }
        public int nMAC_PREFIX_LEN { get; set; }
        public int nMAC_POSTFIX_LEN { get; set; }
        public string sCOMPARE_MAC { get; set; }
        public int nMAC_Self_StartDigit { get; set; }
        public int nMAC_Self_FlowNO { get; set; }
        public int nMAC_Compare_StartDigit { get; set; }
        public int nMAC_Compare_FlowNO { get; set; }
        public TMAC_RULE() { }
    }
    public class OTHER_USE_ASSN
    {
        public bool USED { get; set; }
        public string USEKEY { get; set; }
        public int indexid { get; set; }
        public bool checked1 { get; set; }
        public OTHER_USE_ASSN() { }
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
    public class TMAC_MODEL
    {
        public string MAC { get; set; }
        public string MODEL_NAME { get; set; }
        public string CUSTSN_MAC { get; set; }

    }
    public class R108
    {
        public string SERIAL_NUMBER { get; set; }
        public string KEY_PART_SN { get; set; }
        public string KEY_PART_NO { get; set; }
        public string KP_RELATION { get; set; }
        public string KP_CODE { get; set; }
        public string MO_NUMBER { get; set; }
    }
    public class R_CUSTDATE
    {
        public bool CHECKED { get; set; }
        public int THE_LENGTH { get; set; }
        public bool Dataexit { get; set; }
    }
    public class Material
    {
        public DateTime INSTATION_TIME { get; set; }
    }
}
