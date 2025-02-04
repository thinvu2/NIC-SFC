using Sfc.Api.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sfc.Api.Controllers
{
    public class BaseApiController : ApiController
    {
        public BaseApiController(): base()
        {

        }

        public string GetClientIpAddress()
        {
            try
            {
                return Request.GetClientIpAddress();
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }
}
