using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;

namespace Make_Weight
{
    public class Database : IDatabase
    {
        private readonly SfcHttpClient _sfcHttpClient;
        public Database(SfcHttpClient sfcHttpClient)
        {
            _sfcHttpClient = sfcHttpClient;
        }

        public virtual void ExecuteNoneQuery(string command, IEnumerable<SfcParameter> SfcParameters = null)
        {
            _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = command,
                SfcParameters = SfcParameters
            });
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ExecuteProcAsync(string command, SfcCommandType type, IEnumerable<SfcParameter> SfcParameters = null)
        {
            var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = command,
                SfcParameters = SfcParameters,
                SfcCommandType = type,
            });
            return result.Data;
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetList(string command, IEnumerable<SfcParameter> SfcParameters = null)
        {
            var result = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = command,
                SfcParameters = SfcParameters
            });
            return result.Data;
        }

        public async Task<IDictionary<string, object>> GetObject(string command, IEnumerable<SfcParameter> SfcParameters = null)
        {
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = command,
                SfcParameters = SfcParameters
            });
            return result.Data;
        }
    }
}
