using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AF.DesignPatterns.Singleton;

namespace CHECK_SYS
{
    [Serializable]
    public class DBConnect : Singleton<DBConnect>
    {
        public string link_api { get; set; }
        public string db_connect { get; set; }
    }
}
