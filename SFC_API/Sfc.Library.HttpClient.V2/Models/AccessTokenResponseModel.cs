
using System.Runtime.Serialization;

namespace Sfc.Library.HttpClient.V2.Models
{
    [DataContract]
    public class AccessTokenResponseModel : BaseResponseModel
    {
        [DataMember(Name = "data")]
        public AccessTokenResponse Data { get; set; }

    }
}
