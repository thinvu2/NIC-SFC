using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sfc.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sfc.Api.Middlewares
{
    public class InvalidAuthenticationMiddleware : OwinMiddleware
    {


        public InvalidAuthenticationMiddleware(OwinMiddleware next) : base(next)
        {

        }

        //public override async Task Invoke(IOwinContext context)
        //{

        //    //context.Response.OnSendingHeaders(state =>
        //    //{
        //    //    var response = (OwinResponse)state;

        //    //    if (!response.Headers.ContainsKey("AuthorizationResponse") && response.StatusCode != 400) return;

        //    //    response.Headers.Remove("AuthorizationResponse");
        //    //    response.StatusCode = 401;

        //    //}, context.Response);

        //    //await Next.Invoke(context);

        //    //var owinResponse = context.Response;
        //    //var owinResponseStream = owinResponse.Body;
        //    //var responseBuffer = new MemoryStream();
        //    //owinResponse.Body = responseBuffer;

        //    //await Next.Invoke(context);



        //    /*if (context.Response.StatusCode == 400 && context.Response.Headers.ContainsKey("AuthorizationResponse"))
        //    {
        //        context.Response.Headers.Remove("AuthorizationResponse");
        //        context.Response.StatusCode = 200;
        //        var result = new Error
        //        {
        //            error = "unsupported_grant_type",
        //            description = "The 'grant_type' parameter is missing or unsupported",
        //            url = context.Request.Uri.ToString()
        //        };

        //        var customResponseBody = new StringContent(JsonConvert.SerializeObject(result));
        //        var customResponseStream = await customResponseBody.ReadAsStreamAsync();
        //        await customResponseStream.CopyToAsync(owinResponseStream);

        //        owinResponse.ContentType = "application/json";
        //        owinResponse.ContentLength = customResponseStream.Length;
        //        owinResponse.Body = owinResponseStream;
        //    }*/

        //    //if (context.Response.StatusCode == 400 && context.Response.Headers.ContainsKey("AuthorizationResponse"))
        //    //{
        //    //    context.Response.StatusCode = 200;
        //    //    JObject jObject = new JObject();
        //    //    jObject.Add(RESULT_KEY, RESULT);
        //    //    jObject.Add(MESSAGE_KEY, "Authorization has been denied for this request.");
        //    //    jObject.Add(DATA_KEY, "");

        //    //    var res = jObject.ToString(Newtonsoft.Json.Formatting.None);

        //    //    var content = new StringContent(res, Encoding.UTF8, "application/json");

        //    //    var customResponseStream = await content.ReadAsStreamAsync();
        //    //    await customResponseStream.CopyToAsync(owinResponseStream);

        //    //    owinResponse.ContentType = "application/json";
        //    //    owinResponse.ContentLength = customResponseStream.Length;
        //    //    owinResponse.Body = owinResponseStream;
        //    //} 

        //    context.Response.OnSendingHeaders(state =>
        //    {
        //        var response = (OwinResponse)state;

        //        if (!response.Headers.ContainsKey("AuthorizationResponse") && response.StatusCode != 400) return;

        //        response.Headers.Remove("AuthorizationResponse");
        //        response.StatusCode = 401;


        //    }, context.Response);

        //    await Next.Invoke(context);

        //}


        public async override Task Invoke(IOwinContext context)
        {
            //await Next.Invoke(context);
            if (context.Request.Path.HasValue && context.Request.Method == "POST" && context.Request.Path.Value.Contains("auth"))
            {
                //context.Response.Headers.Remove("AuthorizationResponse");
                //context.Response.Headers.Remove("Access-Control-Allow-Origin");

                StringContent customResponseBody = null;


                // hold a reference to what will be the outbound/processed response stream object
                var stream = context.Response.Body;
                string res = string.Empty;
                try
                {

            
                    // create a stream that will be sent to the response stream before processing
                    using (var buffer = new MemoryStream())
                    {
                        // set the response stream to the buffer to hold the unaltered response
                        context.Response.Body = buffer;

                        // allow other middleware to respond
                        await this.Next.Invoke(context);

                        // we have the unaltered response, go to start
                        buffer.Seek(0, SeekOrigin.Begin);

                        // read the stream
                        var reader = new StreamReader(buffer);
                        string responseBody = reader.ReadToEnd();


                        byte[] byteArray = null;

                        if (context.Response.StatusCode == 400)
                        {
                            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);
                            //JObject jNewObject = JObject.Parse(responseBody);
                            //res = ResponseModel.CreateResponse(ResponseModel.ERROR, jNewObject.GetValue("error_description") != null ? jNewObject.GetValue("error_description")?.ToString() : jNewObject.GetValue("error")?.ToString(), code: 401, "");
                            //customResponseBody = new StringContent(res, Encoding.UTF8, "application/json");
                            //string message = values.Keys.Contains("error_description") ? values["error_description"]?.ToString() : values.Keys.Contains("error") ? values["error"]?.ToString() : "Unauthorized not found {error_description,error} key";
                            string message = values.Keys.Contains("error_description") ? values["error_description"]?.ToString() :  values["error"]?.ToString();
                            var responseModelSingle = new ResponseModelSingle(ResponseModelResult.ERROR, message, null);

                            var serializerSettings = new JsonSerializerSettings();
                            serializerSettings.Formatting = Formatting.Indented;
                            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                            var json = JsonConvert.SerializeObject(responseModelSingle, serializerSettings);
                            customResponseBody = new StringContent(json, Encoding.UTF8, ResponseModelResult.JSONContentType);
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                        }
                        else if (context.Response.StatusCode == 200)
                        {
                            
                            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);
                            var responseModelSingle = new ResponseModelSingle(ResponseModelResult.OK, ResponseModelResult.OK, values);

                            var serializerSettings = new JsonSerializerSettings();
                            serializerSettings.Formatting = Formatting.Indented;
                            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                            var json = JsonConvert.SerializeObject(responseModelSingle, serializerSettings);
                            customResponseBody = new StringContent(json, Encoding.UTF8, ResponseModelResult.JSONContentType);
                            context.Response.StatusCode = (int)HttpStatusCode.OK;

                        }

                        else
                        {
                            return;
                        }

                        //byteArray = Encoding.ASCII.GetBytes(jObject.ToString());
                        byteArray = await customResponseBody.ReadAsByteArrayAsync();

                        context.Response.ContentType = ResponseModelResult.JSONContentType;
                        context.Response.ContentLength = byteArray.Length;

                        buffer.SetLength(0);
                        buffer.Write(byteArray, 0, byteArray.Length);
                        buffer.Seek(0, SeekOrigin.Begin);
                        buffer.CopyTo(stream);
                    }
                }
                catch (Exception ex)
                {

                    using (var buffer = new MemoryStream())
                    {
                        context.Response.Body = buffer;

                        var responseModelSingle = new ResponseModelSingle(ResponseModelResult.ERROR, ex.Message, null);

                        var serializerSettings = new JsonSerializerSettings();
                        serializerSettings.Formatting = Formatting.Indented;
                        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        var json = JsonConvert.SerializeObject(responseModelSingle, serializerSettings);
                        customResponseBody = new StringContent(json, Encoding.UTF8, ResponseModelResult.JSONContentType);

                        byte[] byteArray = null;
                        byteArray = await customResponseBody.ReadAsByteArrayAsync();
                        context.Response.ContentType = ResponseModelResult.JSONContentType;
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentLength = byteArray.Length;

                        buffer.SetLength(0);
                        buffer.Write(byteArray, 0, byteArray.Length);
                        buffer.Seek(0, SeekOrigin.Begin);
                        buffer.CopyTo(stream);
                    }
                }

            }
            else
            {
                await this.Next.Invoke(context);
            }
        }
    }

}