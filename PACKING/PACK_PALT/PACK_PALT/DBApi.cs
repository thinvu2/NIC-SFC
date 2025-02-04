using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfc.Library.HttpClient;

namespace PACK_PALT
{
    class DBApi
    {
        public static SfcHttpClient _sfcHttpClient;
        //private const string baseUrl = "http://10.224.81.51/sfcwebapi";

        public DBApi()
        {

        }
        public static SfcHttpClient sfcClient(string api_link, string db_name)
        {
            return _sfcHttpClient = new SfcHttpClient(api_link, db_name, "helloApp", "123456");
        }
    }
}
