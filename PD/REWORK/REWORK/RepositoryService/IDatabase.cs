using Sfc.Core.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REWORK
{
    interface IDatabase
    {
        Task<IEnumerable<IDictionary<string, object>>> GetList(string command, IEnumerable<SfcParameter> SfcParameters = null, SfcCommandType commandType = SfcCommandType.Text);

        Task<IDictionary<string, object>> GetObject(string command, IEnumerable<SfcParameter> SfcParameters = null, SfcCommandType commandType = SfcCommandType.Text);

        Task<int> ExecuteNoneQuery(string command, IEnumerable<SfcParameter> SfcParameters = null, SfcCommandType commandType = SfcCommandType.Text);
    }
}
