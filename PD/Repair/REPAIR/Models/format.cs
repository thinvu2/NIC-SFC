using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPAIR.Models
{
    class format
    {
    }
    public class listModelName
    {
        public string ModelName { get; set; }
    }
    public class ListError
    {
        public string ERROR_CODE { get; set; }


    }
    public class ListReasonCode
    {
        public string REASON_CODE { get; set; }

        public string REASON_DESC { get; set; }
        public string REASON_DESC2 { get; set; }
        public string DUTY_STATION { get; set; }

    }
    public class ALLPART_DATA
    {
        public string STATUS { get; set; }

        public string KP_NO { get; set; }
        public string MFR_KP_NO { get; set; }
        public string MFR_NAME { get; set; }

        public string DATE_CODE { get; set; }

        public string LOT_CODE { get; set; }
        public string TR_SN { get; set; }
        public string MESSAGE { get; set; }
    }

}
