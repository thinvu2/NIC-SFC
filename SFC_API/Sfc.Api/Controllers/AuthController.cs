using Microsoft.Owin;
using Microsoft.Owin.Security;
using Sfc.Core.Helpers;
using Sfc.Core.Models;
using Sfc.OracleDatabase.Repository;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sfc.Api.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : BaseApiController
    {
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IOwinContext OwinContext
        {
            get
            {
                return Request.GetOwinContext();
            }
        }

        public AuthController() : base()
        {
        }
        //https://stackoverflow.com/questions/38518457/cant-sign-out-web-api-using-owin
        //No logout
       /* [Route("signout")]
        public IHttpActionResult Logout()
        {
            string username =  Authentication.User.Identity.Name;
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }
       */
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
