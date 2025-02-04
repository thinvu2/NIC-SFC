using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPAIR
{
   public class Oracle :Database
    {
        public Oracle(SfcHttpClient sfcHttpClient) : base(sfcHttpClient)
        {

        }
        public async Task<T> GetObj<T>(string command, IEnumerable<SfcParameter> SfcParameters = null, SfcCommandType commandType = SfcCommandType.Text) where T : class, new()
        {
            var result = await GetObject(command, SfcParameters, commandType);
            if (result == null) return default(T);
            return result.ToObject<T>();
        }
        public async Task<List<T>> GetList<T>(string command, IEnumerable<SfcParameter> SfcParameters = null, SfcCommandType commandType = SfcCommandType.Text) where T : class, new()
        {
            var result = await GetList(command, SfcParameters, commandType);
            if (result == null) return new List<T>();
            return result.ToListObject<T>().ToList();
        }
    }
}
