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
        private IChallengeRepository ChalRepo;
        private IChallengeBidRepository BidRepo;

        public ChallengeController()
        {
            ChalRepo = new ChallengeRepository();
            BidRepo = new ChallengeBidRepository();
        }

        // GET /api/challenge
        public List<Challenge> Get()
        {
            return ChalRepo.GetNewest(0, 10);
        }

        // GET /api/challenge/5
        public Challenge Get(int id)
        {
            Challenge c = ChalRepo.Get(id);
            c.Bids = BidRepo.Get(c.ID);
            return c;
        }

        // PUT /api/challenge/bid/5
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void PutBid(int id, ChallengeBid value)
        {
            // validate the data first prolly.
            BidRepo.Add(value);
        }

        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void PostAccept(int id)
        {

        }

        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void PostReject(int id)
        {

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

            ChalRepo.Add(value);
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
