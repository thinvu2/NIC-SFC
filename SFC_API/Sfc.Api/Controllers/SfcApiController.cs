using Sfc.Api.Helpers;
using Sfc.Core.Parameters;
using Sfc.OracleDatabase.Repository;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sfc.Api.Controllers
{
    [RoutePrefix("api")]
    [CustomAuthorize]
    public class SfcApiController : BaseApiController
    {
        public SfcApiController() : base()
        {

        }
       
        [CustomAuthorize]
        [Route("execute")]
        [HttpPost]
        public async Task<IHttpActionResult> ExecuteAsync([FromBody] QuerySingleParameterModel model)
        {
            try
            {
                var dbKey = IdentityHelper.CurrentDb;
                using (var sfcRepository = new SfcRepository(dbKey))
                {
                    //var jsonContent = await Request.Content.ReadAsStringAsync();
                    //var jsonContent = await Request.Content.ReadAsAsync<QuerySingleParameterModel>();

                    //JObject jObject = JObject.Parse(jsonContent);
                    //jObject.Add(EMP_KEY, IdentityHelper.CurrentUserName);
                    //jObject.Add(IP_KEY, GetClientIpAddress());
                    //string jsonString = jObject.ToString(Newtonsoft.Json.Formatting.None);

                    //var res = sfcRepository.QuerySingle(jsonString);

                    //var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    var results = await sfcRepository.ExecuteAsync(model);
                    return Ok(results);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [CustomAuthorize]
        [Route("querylist")]
        [HttpPost]
        public async Task<IHttpActionResult> QueryListAsync([FromBody] QuerySingleParameterModel model)
        {
            try
            {
                var dbKey = IdentityHelper.CurrentDb;
                using (var sfcRepository = new SfcRepository(dbKey))
                {
                    //var jsonContent = await Request.Content.ReadAsStringAsync();
                    //var jsonContent = await Request.Content.ReadAsAsync<QuerySingleParameterModel>();

                    //JObject jObject = JObject.Parse(jsonContent);
                    //jObject.Add(EMP_KEY, IdentityHelper.CurrentUserName);
                    //jObject.Add(IP_KEY, GetClientIpAddress());
                    //string jsonString = jObject.ToString(Newtonsoft.Json.Formatting.None);

                    //var res = sfcRepository.QuerySingle(jsonString);

                    //var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    var results = await sfcRepository.QueryListAsync(model);
                    return Ok(results);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [CustomAuthorize]
        [Route("querysingle")]
        [HttpPost]
        public async Task<IHttpActionResult> QuerySingleAsync([FromBody] QuerySingleParameterModel model)
        {
            try
            {
                var dbKey = IdentityHelper.CurrentDb;
                using (var sfcRepository = new SfcRepository(dbKey))
                {
                    //var jsonContent = await Request.Content.ReadAsStringAsync();
                    //var jsonContent = await Request.Content.ReadAsAsync<QuerySingleParameterModel>();

                    //JObject jObject = JObject.Parse(jsonContent);
                    //jObject.Add(EMP_KEY, IdentityHelper.CurrentUserName);
                    //jObject.Add(IP_KEY, GetClientIpAddress());
                    //string jsonString = jObject.ToString(Newtonsoft.Json.Formatting.None);

                    //var res = sfcRepository.QuerySingle(jsonString);

                    //var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    var result = await sfcRepository.QuerySingleAsync(model);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
       

    }
}
