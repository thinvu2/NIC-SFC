using Microsoft.Owin.Security;
using Sfc.Api.Helpers;
using Sfc.OracleDatabase.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Sfc.Api.Controllers
{
    [CustomAuthorize]
    //[Authorize]
    [RoutePrefix("api/values")]
    public class ValuesController : ApiController
    {
        
        public ValuesController()
        {

        }
        // GET api/values
        //[CustomAuthorize]
        public IEnumerable<string> Get()
        {

           // var claimsIdentity = this.User?.Identity as ClaimsIdentity;
            var db = IdentityHelper.CurrentDb;

            using (var repo = new SfcRepository(db))
            {

            }

                return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
