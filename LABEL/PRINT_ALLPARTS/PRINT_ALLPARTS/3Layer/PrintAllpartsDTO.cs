using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using AF.DesignPatterns.Singleton;
using PRINT_ALLPARTS._3Layer;

namespace PRINT_ALLPARTS._3Layer
{
    [Serializable]
    public class PrintAllpartsDTO: Singleton<PrintAllpartsDTO>
    {
        public string iMoNumber { get; set; }
        public string iModelName { get; set; }
        public string iTargetQty { get; set; }
        public string iBarcodePrefix { get; set; }
        public string iBarcodeLength { get; set; }
        public string iValidChar { get; set; }
        public string iYearMonth { get; set; }
        public string iMacIdStep { get; set; }
        public string iLabelPrefix { get; set; }
        public string iLastData { get; set; }
        public string iQtyPrinted { get; set; }
        public string iVerSion { get; set; }
        public string iFirstSnBeforePrint { get; set; }
        public string iUrl { get; set; }
        public string isfisVersion { get; set; }
        public string iBU { get; set; }
        public string LabMode { get; set; }
    }
}
