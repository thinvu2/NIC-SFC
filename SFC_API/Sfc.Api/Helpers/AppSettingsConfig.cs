using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Sfc.Api.Helpers
{
    public static class AppSettingsConfig
    {
        public static string CorsPolicyOrigins { get { return ConfigurationManager.AppSettings["CorsPolicyOrigins"]; } }
    }
}