using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACK_PALT.Model
{
    public class C_pallet_info
    {
        public string LINE_NAME { get; set; }
        public string MO_NUMBER { get; set; }
        public string MODEL_NAME { get; set; }
        public string PALLET_NO { get; set; }
        public string CUST_PALLET_NO { get; set; }
        public string TARGET { get; set; }
        public string QTY { get; set; }
        public string CLOSE_FLAG { get; set; }
        public string VERSION_CODE { get; set; }
    }
    public class ListTrackNo
    {
        public string track_no { get; set; }
    }
    public class ListShippingSn
    {
        public string shipping_sn { get; set; }
    }public class List_infomation_pallet
    {
        public string serial_number { get; set; }
        public string pallet_no { get; set; }
        public string track_no { get; set; }
        public string shipping_sn { get; set; }
        public string model_name { get; set; }
        public string mac1 { get; set; }
        public string ssn3 { get; set; }
    }
    public class List_info_carton_check
    {
        public string mcarton_no { get; set; }
        public string model_name { get; set; }
        public string pallet_no { get; set; }
        public string track_no { get; set; }
    }
    
}
