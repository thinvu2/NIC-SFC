using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfc.Core.Entities
{
    public class EmployeeEntity
    {
        public string EMP_NO { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_RANK { get; set; }
        public string CLASS_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public DateTime QUIT_DATE { get; set; }
        public string EMP_PASS { get; set; }
        public string EMP_BC { get; set; }
        public string EMP_PWD_PASS { get; set; }
        public string EMAIL { get; set; }
    }
}
