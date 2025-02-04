using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient.Helpers;

namespace CQC.ViewModel
{
    public class R_QC_SN_T
    {
        public string LOT_NO { get; set; }
        public string MODEL_NAME { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string SHIPPING_SN { get; set; }
        public string ERROR_FLAG { get; set; }
        public string TEST_TIME { get; set; }
        public string TESTER { get; set; }
        public string COUNTER { get; set; }
        public string SECTION_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public string CLASS { get; set; }
        public string CLASS_DATE { get; set; }
        public string MOVE_FLAG { get; set; }
        public string CHECK_FLAG { get; set; }
    }
    public class WORK_DESC_T
    {
        public string WORK_SECTION { get; set; }
        public string CLASS { get; set; }
        public string DAY_DISTINCT { get; set; }
    }
    public class OOBA
    {
        public string STT { get; set; }
        public string HEAD { get; set; }
        public string MO_NUMBER { get; set; }
        public string MODEL_NAME { get; set; }
        public string VERSION_CODE { get; set; }
        public string MO_QTY { get; set; }
        public string PA_NO { get; set; }
        public string QTY { get; set; }
        public string AQL { get; set; }
        public string PACK_PARAM { get; set; }
        public string MO_NEED { get; set; }

        public string PA_NEED { get; set; }
        public string PA_HAVE { get; set; }
        public string DIF { get; set; }
        public string MSG { get; set; }

    }
    public class R_MODELFILE_CHECK
    {
        public string MO_NUMBER { get; set; }
        public string MODEL_NAME { get; set; }
        public string VERSION_CODE { get; set; }
        public string LINE_NAME { get; set; }
        public string FIRST_INSTATION_TIME { get; set; }
        public string PASS_DATE { get; set; }
        public string FILE_NO { get; set; }
        public string EMP_NO { get; set; }
        public string PASS_FLAG { get; set; }
    }
    public class EMP
    {
        public string EMP_NO { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_RANK { get; set; }
        public string CLASS_NAME { get; set; }

        public string STATION_NAME { get; set; }

        public string QUIT_DATE { get; set; }
    }
    public class C104
    {
        public string MODEL_NAME { get; set; }
        public string MODEL_SERIAL { get; set; }
        public string MODEL_TYPE { get; set; }
    }
    public class STATIONCONFIG
    {
        public string SECTION_NAME { get; set; }

        public string GROUP_NAME { get; set; }

        public string STATION_NAME { get; set; }
    }
    public class PARAMETER
    {
        public string PRG_NAME { get; set; }

        public string VR_CLASS { get; set; }

        public string VR_ITEM { get; set; }

        public string VR_NAME { get; set; }

        public string VR_VALUE { get; set; }

        public string VR_DESC { get; set; }

        public PARAMETER() { }
        public PARAMETER(string prg_name, string vr_class, string vr_item, string vr_name, string vr_value, string vr_desc)
        {
            this.PRG_NAME = prg_name;
            this.VR_CLASS = vr_class;
            this.VR_ITEM = vr_item;
            this.VR_NAME = vr_name;
            this.VR_VALUE = vr_value;
            this.VR_DESC = vr_desc;
        }
    }

    public class CUSTOMERS
    {
        public string CUSTOMER { get; set; }
    }
    public class ERROR
    {
        public string ERROR_CODE { get; set; }

        public string ERROR_DEGREE { get; set; }

        public string ERROR_TYPE { get; set; }

        public string ERROR_DESC { get; set; }

        public string ERROR_DESC2 { get; set; }

        public string TEST_CODE { get; set; }

    }

    public class DUTY
    {
        public string DUTY_TYPE { get; set; }

        public string DUTY_DESC { get; set; }
    }

    public class CQCREC
    {
        public string LOT_NO { get; set; }

        public string MODEL_NAME { get; set; }

        public string QA_RESULT { get; set; }

        public string START_TIME { get; set; }
        public string CRITICAL_FAIL_QTY { get; set; }
        public string MAJOR_FAIL_QTY { get; set; }

        public string MINOR_FAIL_QTY { get; set; }
    }
    public class REASON
    {
        public string REASON_CODE { get; set; }

        public string REASON_TYPE { get; set; }

        public string REASON_DESC { get; set; }

        public string REASON_DESC2 { get; set; }

        public string DUTY_STATION { get; set; }

        public string STATISTIC_FLAG { get; set; }
    }

    public class OQA
    {
        public string OQA_TYPE { get; set; }

        public string LOT_SIZE_MIN { get; set; }

        public string LOT_SIZE_MAX { get; set; }

        public string SAMPLE_SIZE { get; set; }

        public string MAJOR_REJECT_QTY { get; set; }

        public string MINOR_REJECT_QTY { get; set; }

        public string OQA_DESC { get; set; }
    }
    public class EMP_2_GROUP_T
    {
        public string EMP_NO { get; set; }
        public string GROUP_NAME { get; set; }
    }
    public class LineName
    {
        public string LINE_NAME { get; set; }

    }

    public class GroupName
    {
        public string GROUP_NEXT { get; set; }

    }

    public class passw
    {
        public string EMP { get; set; }
        public string PASSW { get; set; }

        public string FUN { get; set; }

        public string PRIVILEGE { get; set; }
        public string PRG_NAME { get; set; }
    }

    public class GROUP
    {
        public string GROUP_NAME { get; set; }
    }
    public class R107
    {
        public string SERIAL_NUMBER { get; set; }
        public string SECTION_FLAG { get; set; }
        public string MO_NUMBER { get; set; }
        public string MODEL_NAME { get; set; }
        public string TYPE { get; set; }
        public string VERSION_CODE { get; set; }
        public string LINE_NAME { get; set; }

        public string SECTION_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public string ERROR_FLAG { get; set; }
        public string IN_STATION_TIME { get; set; }
        public string SHIPPING_SN { get; set; }
        public string WORK_FLAG { get; set; }
        public string FINISH_FLAG { get; set; }
        public string SPECIAL_ROUTE { get; set; }
        public string PALLET_NO { get; set; }
        public string QA_NO { get; set; }
        public string QA_RESULT { get; set; }
        public string SCRAP_FLAG { get; set; }
        public string NEXT_STATION { get; set; }
        public string BOM_NO { get; set; }
        public string TRACK_NO { get; set; }
        public string PO_NO { get; set; }
        public string PO_LINE { get; set; }
        public string CARTON_NO { get; set; }
        public string IMEI { get; set; }
        public string MCARTON_NO { get; set; }
        public string TRAY_NO { get; set; }
        public string WIP_GROUP { get; set; }
        public string SHIPPING_SN2 { get; set; }

        public string MODEL_TYPE { get; set; }
        public string COUNT { get; set; }

        public string GROUP_NEXT { get; set; }

        public string SN { get; set; }

        public string FLAG { get; set; }

        public string SSN { get; set; }

        public string CARTON { get; set; }

        public string PALLET { get; set; }

        public string MODEL { get; set; }

    }
}
