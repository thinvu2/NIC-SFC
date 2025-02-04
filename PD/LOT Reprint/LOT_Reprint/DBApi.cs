using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOT_REPRINT
{
    class DBApi
    {
        public static SfcHttpClient _sfcHttpClient;
        public static SfcHttpClient sfcClient(string db_name, string api_link)
        {
            return _sfcHttpClient = new SfcHttpClient(api_link, db_name, "LOT_Reprint", "1234567");
        }
    }
}
