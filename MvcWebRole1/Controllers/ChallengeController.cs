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
        private ICustomerRepository CustRepo;
        private Security Security;

        public ChallengeController()
        {
            ChalRepo = new ChallengeRepository();
            BidRepo = new ChallengeBidRepository();
            StatusRepo = new ChallengeStatusRepository();
            CustRepo = new CustomerRepository();
            Security = new Security();
        }

        // GET /api/challenge
        public List<Challenge> Get()
        {
            return ChalRepo.GetNewest(0, 10);
        }

        // GET /api/challenge/5
        public Challenge Get(long id)
        {
            Challenge c = ChalRepo.Get(id);
            c.Bids = BidRepo.Get(c.ID);
            return c;
        }

        public List<ChallengeStatus> GetStatus(long id)
        {
            return StatusRepo.GetActiveStatusesForChallenge(id);
        }

        // PUT /api/challenge/bid/5
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void PutBid(int id, ChallengeBid value)
        {
            Challenge c = ChalRepo.Get(value.ChallengeID);

            if (c == null)
                throw new HttpResponseException("The requested Challenge resource doesn't exist.", System.Net.HttpStatusCode.NotFound);

            if (c.Privacy == (int)Challenge.ChallengePrivacy.FriendsOnly)
            {
                if (Security.DetermineAudience(c) < Security.Audience.Friends)
                    throw new HttpResponseException("This item is friends-only.", System.Net.HttpStatusCode.Forbidden);
            }
            
            c.CurrentBid += value.Amount;
            ChalRepo.Update(c);

            BidRepo.Add(value);
        }

        // POST /api/challenge
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void PostNew(NewChallenge value)
        {
            if (value.Description.Equals(""))
            {
                throw new HttpResponseException("You have to specify a description.", System.Net.HttpStatusCode.InternalServerError);
            }
            
            value.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;

            long newID=ChalRepo.Add(value);
            bool createTargetStatus=false;

            switch (value.TargetType)
            {
                case (int)NewChallenge.TargetCustomerType.Default:
                    if (value.TargetCustomerID > 0)
                    {
                        createTargetStatus = true;


                    }
                    else
                        createTargetStatus = false;
                    break;
                case (int)NewChallenge.TargetCustomerType.Facebook:
                    if (value.FacebookUID != null && !value.FacebookUID.Equals(""))
                    {
                        Customer tryFB = CustRepo.GetWithFacebookID(value.FacebookUID);
                        if (tryFB != null && tryFB.ID>0)
                        {
                            value.TargetCustomerID = tryFB.ID;
                        }
                        else
                        {
                            Customer unclaimedFB = new Customer();
                            unclaimedFB.FacebookUserID = value.FacebookUID;
                            unclaimedFB.Type = (int)Customer.TypeCodes.Unclaimed;
                            value.TargetCustomerID=CustRepo.Add(unclaimedFB);
                        }
                        createTargetStatus = true;
                    }
                    break;
                case (int)NewChallenge.TargetCustomerType.PhoneNumber:
                    if (value.PhoneNumber != null && !value.PhoneNumber.Equals(""))
                    {
                        createTargetStatus = true;
                    }
                    break;
                case (int)NewChallenge.TargetCustomerType.EmailAddress:
                    if (value.EmailAddress != null && !value.EmailAddress.Equals(""))
                    {
                        value.EmailAddress=value.EmailAddress.ToLower();

                        Customer tryEMail = CustRepo.GetWithEmailAddress(value.EmailAddress);
                        if (tryEMail != null && tryEMail.EmailAddress.Equals(value.EmailAddress))
                        {
                            value.TargetCustomerID = tryEMail.ID;
                        }
                        else
                        {
                            Customer unclaimedEmail = new Customer();
                            unclaimedEmail.EmailAddress = value.EmailAddress;
                            unclaimedEmail.Type = (int)Customer.TypeCodes.Unclaimed;
                            value.TargetCustomerID = CustRepo.Add(unclaimedEmail);
                        }
                        createTargetStatus = true;
                    }
                    break;
            }

            if (createTargetStatus)
            {
                ChallengeStatus s = new ChallengeStatus();
                s.ChallengeID = newID;
                s.ChallengeOriginatorCustomerID = value.CustomerID;
                s.CustomerID = value.TargetCustomerID;
                s.UniqueID = System.Guid.NewGuid().ToString();
                s.Status = (int)ChallengeStatus.StatusCodes.None;
                StatusRepo.Add(s);

                // notify the receipient of the new challenge.
            }

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
