using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACK_CTN.Models
{
    class FormatQuery
    {
        public string sn { get; set; }
        public string shipping_Sn { get; set; }
    }
    public class SNformat
    {
        public string ModelName { get; set; }
        public string BarcodePrefix { get; set; }

    }
    public class ListSN
    {
        public string SN { get; set; }

        public string SHIPPING_SN { get; set; }

    }
    public class ListShippingSN
    {
        public string SHIPPING_SN { get; set; }

    }
    public class ListSSN7
    {
        public string SSN7 { get; set; }

    }
    public class ListMac
    {
        public string SHIPPING_SN2 { get; set; }

    }
    public class ReprintListSN
    {
        public string SHIPPING_SN { get; set; }

        public string CHECK { get; set; }

    }

}
