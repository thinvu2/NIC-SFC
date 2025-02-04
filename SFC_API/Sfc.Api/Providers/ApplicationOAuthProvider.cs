using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Sfc.Core.Entities;
using Sfc.Core.Helpers;
using Sfc.OracleDatabase.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sfc.Api.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {


        public ApplicationOAuthProvider()
        {

        }


        public  override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //https://stackoverflow.com/questions/31442364/owin-oauth-send-additional-parameters

            string db_key = string.Empty;
            string[] keyParams = context.Parameters.Where(k => k.Key == "db_key").Select(k => k.Value).FirstOrDefault();

            if (keyParams != null && keyParams.Length > 0)
            {
                db_key = keyParams[0];

                context.OwinContext.Set<string>("db_key", db_key);
            } else
            {
                context.SetError("db_key", "{db_key} not provided.");
                return Task.FromResult<object>(null);
            }
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            ClientEntity client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            
            AuthRepository authRepository = new AuthRepository(db_key);

            client = authRepository.FindClient(clientId);


            if (client == null)
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }

            if (client.ApplicationType == ApplicationTypes.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    if (client.Secret != HashHelper.GetHash(clientSecret))
                    {
                        context.SetError("invalid_clientId", "Client secret is invalid.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<object>("", 10);

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            // http://www.codeproject.com/Articles/742532/Using-Web-API-Individual-User-Account-plus-CORS-En
            // This article helped me track down the issue that even though CORS is enabled application-wide, 
            // it still doesn't affect this OWIN component, so we have to enable it here also.
            //string origins = AppSettingsConfig.CorsPolicyOrigins;
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            if (allowedOrigin == null) allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { allowedOrigin });


            string username = context.UserName;
            string password = context.Password;
            string db_key = context.OwinContext.Get<string>("db_key");

            if(string.IsNullOrEmpty(db_key))
            {
                context.SetError("db_key", "The key {db_key} don't exist.");
                return;
            }
            using(var authRepository = new AuthRepository(db_key))
            {

                var employeeEntity = await authRepository.FindEmployeeAsync(username, password);


                if (employeeEntity == null)
                {
                    context.SetError("invalid_grant", "Username or Password not match!");
                    //context.Response.Headers.Add("AuthorizationResponse", new[] { "Ok" });
                    return;
                }



                //ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                //   OAuthDefaults.AuthenticationType);
                //ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                //    CookieAuthenticationDefaults.AuthenticationType);

                //AuthenticationProperties properties = CreateProperties(user);
                //AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                //context.Validated(ticket);
                //context.Request.Context.Authentication.SignIn(cookiesIdentity);

                // Initialization.  
                var claims = new List<Claim>();

                // Setting  
                claims.Add(new Claim(ClaimTypes.Name, username));
                claims.Add(new Claim("db_key", db_key));

                // Setting Claim Identities for OAUTH 2 protocol.  
                ClaimsIdentity oAuthClaimIdentity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookiesClaimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);

                // Setting user authentication.  

                IDictionary<string,string> props = new Dictionary<string, string>()
                {

                    {"username", username },
                    {
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {"db_key", db_key }
                };
                AuthenticationProperties properties = new AuthenticationProperties(props);
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthClaimIdentity, properties);

                // Grant access to authorize user.  
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(cookiesClaimIdentity);
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                //context.AdditionalResponseParameters.Add(property.Key, property.Value);
                if (property.Key == ".issued")
                {
                    
                    context.AdditionalResponseParameters.Add(property.Key, context.Properties.IssuedUtc.Value.ToString("o", (IFormatProvider)CultureInfo.InvariantCulture));
                }
                else if (property.Key == ".expires")
                {
                    context.AdditionalResponseParameters.Add(property.Key, context.Properties.ExpiresUtc.Value.ToString("o", (IFormatProvider)CultureInfo.InvariantCulture));
                }
                else
                {
                    context.AdditionalResponseParameters.Add(property.Key, property.Value);
                }
            }

            return Task.FromResult<object>(null);
        }


        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }
        public static AuthenticationProperties CreateProperties(string username)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "username", username }
            };
            return new AuthenticationProperties(data);
        }
    }
}