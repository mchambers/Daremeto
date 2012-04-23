using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http;
using System.Security.Principal;
using DareyaAPI.Models;

namespace DareyaAPI.Filters
{
    public class DareyoutoAuthorizationAttribute : System.Web.Http.AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            Models.AuthorizationRepository ar = new Models.AuthorizationRepository();

            if (!HttpContext.Current.Request.Headers.AllKeys.Contains("DYAuthToken"))
            {
                HandleUnauthorizedRequest(actionContext);
            }
            else
            {
                // valid token?
                Authorization a=ar.GetWithToken(HttpContext.Current.Request.Headers["DYAuthToken"]);

                if (a == null)
                {
                    HandleUnauthorizedRequest(actionContext);
                }
                else
                {
                    String[] roles = { "Standard" };
                    HttpContext.Current.User=new GenericPrincipal(new DareyaIdentity(a.EmailAddress, a.CustomerID), roles);
                }
            }
            
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var challengeMessage = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            actionContext.Response = challengeMessage;
            throw new HttpResponseException(challengeMessage);
        }
    }
}