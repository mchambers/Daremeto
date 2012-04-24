using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using DareyaAPI.Models;

namespace DareyaAPI.Controllers.Controllers
{
    public class LoginController : ApiController
    {
        // POST /api/<controller>
        public Authorization Post(Login value)
        {
            Security s=new Security();

            Authorization a = s.AuthorizeCustomer(value);
            if (a == null)
                throw new HttpResponseException("Invalid credentials", System.Net.HttpStatusCode.Forbidden);

            return a;
        }
    }
}