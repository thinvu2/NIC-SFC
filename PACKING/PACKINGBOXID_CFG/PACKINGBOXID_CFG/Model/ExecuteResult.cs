using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Newtonsoft.Json;

namespace PACKINGBOXID_CFG.Model
{
    class ExecuteResult
    {
        private string message = "";
        private int type = 0;
        private bool status = true;
        private object anything;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public Object Anything
        {
            get { return anything; }
            set { anything = value; }
        }

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool Status
        {
            get { return status; }
            set { status = value; }
        }
        public static async Task<DataTable> ExecuteSQL(string sql, SfcHttpClient sfcHttpClient)
        {
            DataTable data;
            data = null;
            try
            {
                var datacust = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });


                if (datacust.Data != null)
                {
                    var vardatatabel = JsonConvert.SerializeObject(datacust.Data);
                    data = JsonConvert.DeserializeObject<DataTable>(vardatatabel);
                }
            }
            catch (Exception ex)
            {

            }
            return data;
        }
    }
}
