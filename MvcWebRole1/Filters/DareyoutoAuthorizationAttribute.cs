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
    public enum DYAuthorizationRoles
    {
        Public,
        Users,
        Private,
        Friends
    }

    public enum DYAuthorizationResourceType
    {
        None,
        Customer,
        Challenge
    }

    public class DYAuthorizationAttribute : System.Web.Http.AuthorizeAttribute
    {
        private DYAuthorizationRoles AuthLevel = DYAuthorizationRoles.Private;
        private DYAuthorizationResourceType AuthResource;
        private long AuthResourceID;

        public DYAuthorizationAttribute(DYAuthorizationRoles level)
        {
            AuthLevel = level;
        }

        public DYAuthorizationAttribute(DYAuthorizationRoles level, DYAuthorizationResourceType type, long ID)
        {
            AuthResource = type;
            AuthResourceID = ID;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            IAuthorizationRepository ar = RepoFactory.GetAuthorizationRepo();

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