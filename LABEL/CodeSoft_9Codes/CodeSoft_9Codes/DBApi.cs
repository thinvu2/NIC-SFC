using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSoft_9Codes
{
    class DBApi
    {
        public static SfcHttpClient  _sfcHttpClient;
        public SfcHttpClient sfcClient(string db_name, string api_link)
        {
            return _sfcHttpClient = new SfcHttpClient(api_link, db_name, "helloApp", "123456");
        }
    }
}
