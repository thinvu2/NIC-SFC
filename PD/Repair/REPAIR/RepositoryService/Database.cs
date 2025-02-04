using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPAIR
{
    public class Database
    {
        public readonly SfcHttpClient _sfcHttpClient;
        public Database(SfcHttpClient sfcHttpClient)
        {
            _sfcHttpClient = sfcHttpClient;
        }

        public async Task<int> ExecuteNoneQuery(string command, IEnumerable<SfcParameter> SfcParameters = null, SfcCommandType commandType = SfcCommandType.Text)
        {
            await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = command,
                SfcCommandType = commandType,
                SfcParameters = SfcParameters
            });
            return 0;
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetList(string command, IEnumerable<SfcParameter> SfcParameters = null, SfcCommandType commandType = SfcCommandType.Text)
        {
            var result = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = command,
                SfcCommandType = commandType,
                SfcParameters = SfcParameters
            });
            return result.Data;
        }

        public async Task<IDictionary<string, object>> GetObject(string command, IEnumerable<SfcParameter> SfcParameters = null, SfcCommandType commandType = SfcCommandType.Text)
        {
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = command,
                SfcCommandType = commandType,
                SfcParameters = SfcParameters
            });
            return result.Data;
        }
    }
}
