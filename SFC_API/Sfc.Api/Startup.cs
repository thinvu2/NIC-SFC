using System;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Sfc.Api.Helpers;
using Sfc.Api.Middlewares;
using Sfc.Api.Providers;

[assembly: OwinStartup(typeof(Sfc.Api.Startup))]

namespace Sfc.Api
{
    public class Startup
    {
        #region Public /Protected Properties.  

        /// <summary>  
        /// OAUTH options property.  
        /// </summary>  
        //public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        /// <summary>  
        /// Public client ID property.  
        /// </summary>  
        public static string PublicClientId { get; private set; }

        #endregion


        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            app.Use<InvalidAuthenticationMiddleware>();
            this.ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);

        }
        private void ConfigureOAuth(IAppBuilder app)
        {
            //// Enable the application to use a cookie to store information for the signed in user  
            //// and to use a cookie to temporarily store information about a user logging in with a third party login provider  
            //// Configure the sign in cookie  
            //app.UseCookieAuthentication(new CookieAuthenticationOptions());


            //// Configure the application for OAuth based flow  
            //PublicClientId = "self";
            //OAuthOptions = new OAuthAuthorizationServerOptions
            //{
            //    TokenEndpointPath = new PathString("/api/auth/token"),
            //    Provider = new ApplicationOAuthProvider(PublicClientId),
            //    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(1),
            //    AllowInsecureHttp = true, //Don't do this in production ONLY FOR DEVELOPING: ALLOW INSECURE HTTP!
            //    RefreshTokenProvider = new ApplicationRefreshTokenProvider()
            //};

            //// Enable the application to use bearer tokens to authenticate users  
            //app.UseOAuthBearerTokens(OAuthOptions);

            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {

                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/auth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
                Provider = new ApplicationOAuthProvider(),
                RefreshTokenProvider = new ApplicationRefreshTokenProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);




        }


    }
}
