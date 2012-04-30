using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using DareyaAPI.Models;
using System.Web;

namespace DareyaAPI.Controllers
{
    public class ChallengeStatusController : ApiController
    {
        private IChallengeStatusRepository StatusRepo;
        private IChallengeRepository ChalRepo;
        private Security Security;

        public ChallengeStatusController()
        {
            StatusRepo = new ChallengeStatusRepository();
            ChalRepo = new ChallengeRepository();
            Security = new Security();
        }

        // GET /api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void PostAccept(ChallengeStatus status)
        {
            Challenge c = ChalRepo.Get(status.ChallengeID);
            ChallengeStatus s = StatusRepo.Get(status.ChallengeID, status.UniqueID);

            if (!Security.CanManipulateContent(c))
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            s.ChallengeID = c.ID;
            s.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            s.Status = (int)ChallengeStatus.StatusCodes.Accepted;
            s.UniqueID = System.Guid.NewGuid().ToString();

            StatusRepo.Update(s);
        }

        // Reject is specifically for when a Challenge has been sent directly to you
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void PostReject(ChallengeStatus status)
        {
            Challenge c = ChalRepo.Get((int)status.ChallengeID);

            if (c.Privacy != (int)Challenge.ChallengePrivacy.SinglePerson || c.TargetCustomerID != ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotImplemented);

            ChallengeStatus s = StatusRepo.Get(status.ChallengeID, status.UniqueID);

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
    }
}