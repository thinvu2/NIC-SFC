using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace Sfc.Api.Helpers
{
    //https://gist.github.com/imeanitworks/90b06118e5b0797ff5baf26a5a2f788e
    public static class IdentityHelper
    {
        private const string RoleKey = "role";
        private const string DbKey = "db_key";
        private const string TransactionKey = "trasaction_key";
        private static ClaimsIdentity User { get; set; }

        private static ClaimsIdentity Get()
        {
            try
            {
                return (Thread.CurrentPrincipal.Identity as ClaimsIdentity);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Unable to cast the authenticated user to ClaimsIdentity");
            }

        }

        private static void SetUser()
        {
            User = Get();
        }

        //public static List<string> GetRoles()
        //{
        //    if (User != null)
        //    {
        //        try
        //        {
        //            return User.FindAll(RoleKey).ToList().ConvertAll(x => x.Value);
        //        }
        //        catch (Exception e)
        //        {
        //            throw new ArgumentOutOfRangeException("Unable to find user roles in claims identity for authenticated user.");
        //        }
        //    }
        //    return null;
        //}

        public static string CurrentUserName
        {
            get
            {
                SetUser();
                if (User != null)
                {
                    try
                    {
                        return User.FindFirst(ClaimTypes.Name).Value;
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentOutOfRangeException("Unable to find 'nt_name' in claims identity for authenticated user.");
                    }
                }
                return null;
            }
        
        }

        public static string CurrentDb
        {
            get
            {
                SetUser();
                if (User != null)
                {
                    try
                    {
                        return User.FindFirst(DbKey).Value;
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentOutOfRangeException("Unable to find 'nt_name' in claims identity for authenticated user.");
                    }
                }
                return null;
            }
        }

    }
}