using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Sfc.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Web API configuration and services

            //This handles CORS requests globally.
            //config.SetCorsPolicyProviderFactory(new CorsPolicyFactory());
            //config.EnableCors();

            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.MapHttpAttributeRoutes();
            config.RegisterExceptionFilters();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //WebAPI when dealing with json & JavaScript!
            //Setup json serialization to serialize classes to camel (std. Json format)
            var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
           
            formatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            formatter.SerializerSettings.DateFormatString = "yyyy/MM/dd hh:mm:ss";
            formatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            //Adding JSON type web api fomatting
            config.Formatters.Clear();
            config.Formatters.Add(formatter);
        }
    }
}
