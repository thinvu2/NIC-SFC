using System.Runtime.Serialization;

namespace Sfc.Library.HttpClient.V2.Models
{
    [DataContract]
    public class BaseResponseModel
    {
        [DataMember(Name = "result")]
        public string Result { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

    }
}
