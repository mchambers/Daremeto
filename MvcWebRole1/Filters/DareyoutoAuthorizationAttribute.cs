using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Filters
{
    public class DareyoutoAuthorizationAttribute : System.Web.Http.AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return false;
        }
    }
}