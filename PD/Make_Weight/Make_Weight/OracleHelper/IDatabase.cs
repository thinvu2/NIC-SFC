using Sfc.Core.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make_Weight
{
    public interface IDatabase
    {
        Task<IEnumerable<IDictionary<string, object>>> GetList(string command, IEnumerable<SfcParameter> SfcParameters = null);

        Task<IDictionary<string, object>> GetObject(string command, IEnumerable<SfcParameter> SfcParameters = null);

        Task<IEnumerable<IDictionary<string, object>>> ExecuteProcAsync(string command, SfcCommandType type, IEnumerable<SfcParameter> SfcParameters = null);

        void ExecuteNoneQuery(string command, IEnumerable<SfcParameter> SfcParameters = null);
    }
}
