using System.Collections.Generic;
using System.Linq;


namespace Sfc.Core.Models
{
    public class ResponseModelList
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public IEnumerable<IDictionary<string, object>> Data { get; set; }

        public ResponseModelList(string result, string message, IEnumerable<IDictionary<string, object>> data)
        {

            this.Result = result;
            this.Message = message;
            this.Data = data != null ? data.Select(r => r.Distinct().ToDictionary(d => d.Key.ToLower(), d => d.Value)) : null;
        }
    }
}
