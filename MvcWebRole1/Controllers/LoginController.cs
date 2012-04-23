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
            CustomerRepository repo=new CustomerRepository();

            Customer c = repo.GetWithEmailAddress(value.EmailAddress);
            if (c == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            if (!value.Password.Equals(c.Password))
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            Authorization a = new Authorization("test"+System.DateTime.Now.Ticks.ToString());
            a.CustomerID = c.ID;
            a.EmailAddress = value.EmailAddress;

            AuthorizationRepository authRepo = new AuthorizationRepository();
            authRepo.Add(a); // store the auth token in the repo

            return a;
        }

        // PUT /api/<controller>/5
        public void Put(int id, string value)
        {
        }

        // DELETE /api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}