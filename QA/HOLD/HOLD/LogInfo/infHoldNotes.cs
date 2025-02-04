using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOLD.LogInfo
{
    class infHoldNotes
    {
        public string DOCNO { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public string CONDITION_MO { get; set; }
        public string INFORMATION_MO { get; set; }
        public string CONDITION_CARTON { get; set; }
        public string INFORMATION_CARTON { get; set; }
        public string CONDITION_PALLET { get; set; }
        public string INFORMATION_PALLET { get; set; }
        public string HOLD_FLAG { get; set; }
        public string CREATE_DATE { get; set; }
        public string REQUEST_EMP { get; set; }
        public string REQUEST_EMAIL { get; set; }
        public string CC_EMAIL { get; set; }
        public string RUN_FLAG { get; set; }
        public string RUN_DESC { get; set; }
    }
}
