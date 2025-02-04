using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sfc.Library.HttpClient.V2.Models
{
    public class ResponseModelCommandList : BaseResponseModel
    {
        [DataMember(Name = "data")]
        public IEnumerable<ResponseModelList> Data { get; set; }
    }
}
