using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Sfc.Api
{
    public static class HttpConfigurationFilterExtensions
    {
        public static HttpConfiguration RegisterExceptionFilters(this HttpConfiguration httpConfig)
        {

            httpConfig.Filters.Add(new CustomExceptionFilterAttribute());
            return httpConfig;

        }
    }
}