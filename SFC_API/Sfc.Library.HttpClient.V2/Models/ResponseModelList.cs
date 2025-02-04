using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sfc.Library.HttpClient.V2.Models
{
    [DataContract]
    public class ResponseModelList : BaseResponseModel
    {

        [DataMember(Name = "data")]
        public IEnumerable<IDictionary<string, object>> Data { get; set; }
    }
}
