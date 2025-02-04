using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FG_CHECK
{
    class fDal
    {
        public async Task<DataTable> ExcuteSelectSQL(string sql, SfcHttpClient sfcHttpClient)
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
                throw new Exception(ex.ToString());
            }
            return data;
        }
        public async Task<bool> ExcuteNonQuerySQL(string sql, SfcHttpClient sfcHttpClient)
        {
            try
            {
                await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return true;
        }
        public async Task<DataTable> ExcuteSP(string procename, List<SfcParameter> ListPara, SfcHttpClient sfcHttpClient)
        {
            DataTable data = new DataTable();
            try
            {
                var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = procename,
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = ListPara
                });
                dynamic ads = result.Data;
                var vardatatabel = JsonConvert.SerializeObject(result.Data);
                data = JsonConvert.DeserializeObject<DataTable>(vardatatabel);
            }
            catch (Exception ex)
            {
                return null;
            }
            return data;
        }
    }
}
