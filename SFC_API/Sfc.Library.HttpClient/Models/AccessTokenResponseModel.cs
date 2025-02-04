
using System.Runtime.Serialization;

namespace Sfc.Library.HttpClient.Models
{
    [DataContract]
    public class AccessTokenResponseModel : BaseResponseModel
    {
        [DataMember(Name = "data")]
        public AccessTokenResponse Data { get; set; }

    }
}
