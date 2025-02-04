using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Sfc.Api.Helpers
{
    public static class AuthContextHelper
    {
        public static void SetCustomError(this OAuthGrantResourceOwnerCredentialsContext context, string error, string errorMessage)
        {
            var json = new ResponseMessage
            { Data = errorMessage, Message = error, IsError = true }.ToJsonString();
            context.SetError(json);

            context.Response.Write(json);
            Invoke(context);
        }
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        static async Task Invoke(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var owinResponseStream = new MemoryStream();
            var customResponseBody = new System.Net.Http.StringContent(JsonConvert.SerializeObject(new ResponseMessage()));
            var customResponseStream = await customResponseBody.ReadAsStreamAsync();
            await customResponseStream.CopyToAsync(owinResponseStream);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            context.Response.ContentLength = customResponseStream.Length;
            context.Response.Body = owinResponseStream;
        }
    }

    public class ResponseMessage
    {
        public bool IsError { get; set; }
        public string Data { get; set; }
        public string Message { get; set; }
    }
}