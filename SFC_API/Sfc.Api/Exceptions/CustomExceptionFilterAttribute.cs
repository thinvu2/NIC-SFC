using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using Sfc.Core.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;

namespace Sfc.Api
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        public override void OnException(HttpActionExecutedContext context)
        {

            var exception = context.Exception.InnerException ?? context.Exception;

            if (exception is SfcException)
            {
                SfcException sfcException = exception as SfcException;
          
            }
            var response = context.Request.CreateResponse(HttpStatusCode.InternalServerError);

            var responseModelSingle = new ResponseModelSingle(ResponseModelResult.ERROR, exception.Message, null);

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