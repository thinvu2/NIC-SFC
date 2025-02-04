using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfc.Library.HttpClient;

namespace PACKINGBOXID_CFG.Model
{
    class DBApi
    {
        public static SfcHttpClient _sfcHttpClient;

        public DBApi()
        {

        }
        public static SfcHttpClient sfcClient(string db_name, string api_link)
        {
            return _sfcHttpClient = new SfcHttpClient(api_link, db_name, "helloApp", "123456");
        }

    }
}
