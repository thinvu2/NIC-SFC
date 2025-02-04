using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using Sfc.Core.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Sfc.Api
{
    //https://stackoverflow.com/questions/51568427/creating-custom-authorizeattribute-in-web-api-net-framework
    //https://dev.to/leading-edje/custom-authorization-filters-in-asp-net-web-api-3hnm
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        //private const string RESULT_KEY = "result";
        //private const string MESSAGE_KEY = "message";
        //private const string DATA_KEY = "data";
        //private const string RESULT = "FAIL";

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }
        protected override void HandleUnauthorizedRequest(HttpActionContext context)
        {

            //JObject jObject = new JObject();
            //jObject.Add(RESULT_KEY, RESULT);
            //jObject.Add(MESSAGE_KEY, "Authorization has been denied for this request.");

            //jObject.Add(DATA_KEY, "");

            //var res = jObject.ToString(Newtonsoft.Json.Formatting.None);
            var response = context.Request.CreateResponse(HttpStatusCode.Unauthorized);
            //var res = ResponseModel.CreateResponse(ResponseModel.ERROR, "Authorization has been denied for this request.", code: 401, "");
            //var content = new StringContent(res, Encoding.UTF8, "application/json");
            var responseModelSingle = new ResponseModelSingle(ResponseModelResult.ERROR, "Authorization has been denied for this request.", null);

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.Formatting = Formatting.Indented;
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var json = JsonConvert.SerializeObject(responseModelSingle, serializerSettings);
            var content = new StringContent(json, Encoding.UTF8, ResponseModelResult.JSONContentType);
            response.Content = content;
            context.Response = response;


        }
    }
}