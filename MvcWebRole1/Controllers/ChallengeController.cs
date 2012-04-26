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
        private IChallengeStatusRepository StatusRepo;
        private Security Security;

        public ChallengeController()
        {
            ChalRepo = new ChallengeRepository();
            BidRepo = new ChallengeBidRepository();
            Security = new Security();
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
            Challenge c = ChalRepo.Get(id);
            ChallengeStatus s = new ChallengeStatus();

            if (!Security.CanManipulateContent(c))
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            s.ChallengeID = c.ID;
            s.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            s.Status = (int)ChallengeStatus.StatusCodes.Accepted;
            s.UniqueID = System.Guid.NewGuid().ToString();

            StatusRepo.Add(s);
        }

        // Reject is specifically for when a Challenge has been sent directly to you
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void PostReject(ChallengeStatus status)
        {
            Challenge c = ChalRepo.Get((int)status.ChallengeID);

            if (c.Privacy != (int)Challenge.ChallengePrivacy.SinglePerson || c.TargetCustomerID != ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotImplemented);

            ChallengeStatus s=StatusRepo.Get(status.ChallengeID, status.UniqueID);

            if (!Security.CanManipulateContent(c))
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            s.ChallengeID = c.ID;
            s.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            s.Status = (int)ChallengeStatus.StatusCodes.TargetRejected;
            s.UniqueID = System.Guid.NewGuid().ToString();

            StatusRepo.Add(s);

            c.State = (int)Challenge.ChallengeState.Rejected;
            ChalRepo.Update(c);
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
