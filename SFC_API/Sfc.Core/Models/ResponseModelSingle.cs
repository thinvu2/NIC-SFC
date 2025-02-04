using System.Collections.Generic;
using System.Linq;

namespace Sfc.Core.Models
{
    public class ResponseModelSingle
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public IDictionary<string, object> Data { get; set; }

        public ResponseModelSingle(string result, string message, IDictionary<string, object> data)
        {

            this.Result = result;
            this.Message = message;
            this.Data = data != null ? data.ToDictionary(d=>d.Key.ToLower(), d => d.Value) : null;
        }
    }
}
