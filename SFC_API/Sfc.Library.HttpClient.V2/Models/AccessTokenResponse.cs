using System;
using System.Runtime.Serialization;


namespace Sfc.Library.HttpClient.V2.Models
{
    [DataContract]
    public class AccessTokenResponse
    {
        /*
         * {
        "access_token": "XmWFffnTNtc1n1NhrFutiQt6CBmr_zv160GUzhy1g9egZCVfCYuiLwxEp0CRitGEqHj3c-AF6Fz5Im7W5QZjMZg7OVHANOmnGK7jDwh8TwyrPMoXIdDSvi7EdlClIz407CA9c76JdyjyiBIIeRT0EW2dU78fgNK2ubBasJuILMqdX9OF8IMZvERJW4-t3t8Y_yQ16TAxAeXBPJ2_DPWfMuECYGsEWwUUAbU1eeS7PuAfYTbO7MBcdr4DQJBUcj9vc7v5NLJRe236Xt4wDCMqrVKRZKfE4VP7G-TbWx5uti8",
        "token_type": "bearer",
        "expires_in": 1799,
        "refresh_token": "09fb01f4660e4049a6b00911958fe0ea",
        "username": "V0916141",
        "as:client_id": "helloApp",
        "db_key": "TEST",
        ".issued": "Fri, 26 Feb 2021 02:47:50 GMT",
        ".expires": "Fri, 26 Feb 2021 03:17:50 GMT"
        }
         */
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "as:client_id")]
        public string ClientId { get; set; }

        [DataMember(Name = "db_key")]
        public string DbKey { get; set; }

        [DataMember(Name = ".issued")]
        public DateTime Issued { get; set; }

        [DataMember(Name = ".expires")]
        public DateTime Expires { get; set; }


    }
}
