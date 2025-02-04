using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRCM_B2B
{
    class DBApi
    {
        public static SfcHttpClient _sfcHttpClient;
        private const string baseUrl = "http://10.224.81.51/sfcwebapi";//web API

        public DBApi()
        {

        }

        public static SfcHttpClient sfcClient(string db_name, string api_link)
        {
            return _sfcHttpClient = new SfcHttpClient("http://10.220.96.223/sfcapi", db_name, "helloApp", "123456");
        }
        //public static SfcHttpClient sfcClient(string db_name)
        //{
        //    return _sfcHttpClient = new SfcHttpClient(baseUrl, db_name, "helloApp", "123456");//tai khoan luu csdl, khong thay doi
        //    //trien khai tren BD moi (NIC) thi phai them 2 bang SFIS1.C_API_REFRESHTOKEN_T, SFIS1.C_API_CLIENT_T
        //}
    }
}
