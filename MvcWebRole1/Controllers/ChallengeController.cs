using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using DareyaAPI.Models;

using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Security.Principal;

namespace DareyaAPI.Controllers
{
    public class ChallengeController : ApiController
    {
        private daremetoEntities repo;

        public ChallengeController()
        {
            repo = new daremetoEntities();
        }

        // GET /api/challenge
        public IEnumerable<Challenge> Get()
        {
            return repo.Challenge.OrderByDescending(c=>c.ID).Take(10);
        }

        // GET /api/challenge/5
        public Challenge Get(int id)
        {
            Challenge c = repo.Challenge.FirstOrDefault(chal => chal.ID == id);
            return c;
        }

        // POST /api/challenge
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void PostNew(Challenge value)
        {
            if (value.Description.Equals(""))
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }

            value.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            repo.Challenge.AddObject(value);
            repo.SaveChanges();
        }

        // PUT /api/challenge/5
        public void Put(int id, string value)
        {
        }
        
        // DELETE /api/challenge/5
        public void Delete(int id)
        {
        }
    }
}
