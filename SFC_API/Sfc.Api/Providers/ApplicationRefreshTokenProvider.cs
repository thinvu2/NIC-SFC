using Microsoft.Owin.Security.Infrastructure;
using Sfc.Core.Entities;
using Sfc.Core.Helpers;
using Sfc.OracleDatabase.Repository;
using System;
using System.Globalization;
using System.Threading.Tasks;


namespace Sfc.Api.Providers
{
    //https://www.c-sharpcorner.com/UploadFile/ff2f08/angularjs-enable-owin-refresh-tokens-using-asp-net-web-api/
    public class ApplicationRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public void Create(AuthenticationTokenCreateContext context)
        {

        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");
            string db_key = context.OwinContext.Get<string>("db_key");
            using (AuthRepository _repo = new AuthRepository(db_key))
            {
                //var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

                var token = new RefreshTokenEntity()
                {
                    Id = HashHelper.GetHash(refreshTokenId),
                    ClientId = clientid,
                    Subject = context.Ticket.Identity.Name,
                    //Issued = DateTime.Now,
                    //Expires = DateTime.Now.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))

                    Issued = DateTime.Parse(context.Ticket.Properties.IssuedUtc.Value.ToString("o", (IFormatProvider)CultureInfo.InvariantCulture)),
                    Expires = DateTime.Parse(context.Ticket.Properties.ExpiresUtc.Value.ToString("o", (IFormatProvider)CultureInfo.InvariantCulture)),
                };

                //context.Ticket.Properties.IssuedUtc = token.Issued;
                //context.Ticket.Properties.ExpiresUtc = token.Expires;

                token.ProtectedTicket = context.SerializeTicket();

                var result = await _repo.AddRefreshTokenAsync(token);

                if (result)
                {
                    context.SetToken(refreshTokenId);
                }

            }
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
           
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            string db_key = context.OwinContext.Get<string>("db_key");
            string hashedTokenId = HashHelper.GetHash(context.Token);

            using (AuthRepository _repo = new AuthRepository(db_key))
            {
                var refreshToken = await _repo.FindRefreshTokenAsync(hashedTokenId);
                if (refreshToken != null)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    await _repo.RemoveRefreshTokenAsync(hashedTokenId);
                } 
                
            }
        }
    }
}