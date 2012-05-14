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

        private Challenge PrepOutboundChallenge(Challenge c)
        {
            Customer tempCust = CustRepo.GetWithID(c.CustomerID);
            c.Customer = Customer.Filter(tempCust);

            tempCust = null;

            if (c.TargetCustomerID > 0)
            {
                Customer tempTargetCust = CustRepo.GetWithID(c.TargetCustomerID);
                if (tempTargetCust.FirstName != null && !tempTargetCust.FirstName.Equals(""))
                {
                    c.TargetCustomer = Customer.Filter(tempTargetCust);
                }
                tempTargetCust = null;
            }

            c.NumberOfBidders = BidRepo.Get(c.ID).Count;
            c.NumberOfTakers = StatusRepo.GetActiveStatusesForChallenge(c.ID).Count;

            return c;
        }

        [HttpGet]
        public List<Challenge> Featured(int StartAt = 0, int Limit = 10)
        {
            throw new NotImplementedException();
        }

        // GET /api/challenge
        [HttpGet]
        public List<Challenge> Get(int StartAt=0, int Limit=10)
        {
            List<Challenge> chals = ChalRepo.GetNewest(0, 10);
            List<Challenge> outChals = new List<Challenge>(chals.Count);

            foreach (Challenge c in chals)
            {
                outChals.Add(PrepOutboundChallenge(c));
            }

            return outChals;
        }

        [HttpGet]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public List<Challenge> ActiveForCustomer(long id)
        {
            if (id == 0) id = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;

            Customer c = CustRepo.GetWithID(id);
            Security.Audience audience = Security.DetermineAudience(c);
            if ((audience != Security.Audience.Owner) && (audience != Security.Audience.Friends))
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            /*
             * 
             * array of challenge objects with a Status object in them
             * 
             * */
            List<ChallengeStatus> statuses = StatusRepo.GetActiveChallengesForCustomer(id);
            List<Challenge> challenges = new List<Challenge>();

            foreach (ChallengeStatus s in statuses)
            {
                Challenge chal = PrepOutboundChallenge(ChalRepo.Get(s.ChallengeID));
                chal.Status = s;
                challenges.Add(chal);
            }

            return challenges;
        }

        // GET /api/challenge/5
        [HttpGet]
        public Challenge Get(long id)
        {
            Challenge c = PrepOutboundChallenge(ChalRepo.Get(id));

            //c.Bids = BidRepo.Get(c.ID);

            return c;
        }

        [HttpGet]
        public List<ChallengeStatus> Status(long id)
        {
            return StatusRepo.GetActiveStatusesForChallenge(id);
        }

        [HttpGet]
        public List<ChallengeBid> ActiveBids()
        {
            List<ChallengeBid> bids=BidRepo.GetForCustomer(((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID);

            foreach (ChallengeBid b in bids)
            {
                b.Challenge = PrepOutboundChallenge(ChalRepo.Get(b.ChallengeID));
            }

            return bids;
        }

        // PUT /api/challenge/bid/5
        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void Bid(ChallengeBid value)
        {
            Challenge c = ChalRepo.Get(value.ChallengeID);

            if (c == null)
                throw new HttpResponseException("The requested Challenge resource doesn't exist.", System.Net.HttpStatusCode.NotFound);

            if (c.Privacy == (int)Challenge.ChallengePrivacy.FriendsOnly)
            {
                if (Security.DetermineAudience(c) < Security.Audience.Friends)
                    throw new HttpResponseException("This item is friends-only.", System.Net.HttpStatusCode.Forbidden);
            }

            /*
            c.CurrentBid += value.Amount;
            ChalRepo.Update(c);

            value.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            value.Status = (int)ChallengeBid.BidStatusCodes.Default;

            BidRepo.Add(value);*/

            // transactional stored procedure bitches!
            ChalRepo.AddBidToChallenge(c, ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID, value.Amount);
        }

        // POST /api/challenge
        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void New(NewChallenge value)
        {
            if (value.Description.Equals(""))
            {
                throw new HttpResponseException("You have to specify a description.", System.Net.HttpStatusCode.InternalServerError);
            }
            
            value.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            int firstBid = value.CurrentBid;
            value.CurrentBid = 0; // IMPORTANT
            value.ID=ChalRepo.Add(value);
            ChalRepo.AddBidToChallenge(value, value.CustomerID, firstBid);

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
            
            ChalRepo.Update(value);

            if (createTargetStatus)
            {
                ChallengeStatus s = new ChallengeStatus();
                s.ChallengeID = value.ID;
                s.ChallengeOriginatorCustomerID = value.CustomerID;
                s.CustomerID = value.TargetCustomerID;
                s.UniqueID = System.Guid.NewGuid().ToString();
                s.Status = (int)ChallengeStatus.StatusCodes.None;
                StatusRepo.Add(s);

                // notify the receipient of the new challenge.
            }

        }

        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public ChallengeStatus Take(long id)
        {
            Challenge c = ChalRepo.Get(id);

            if (c == null)
                throw new HttpResponseException("The requested Challenge resource doesn't exist.", System.Net.HttpStatusCode.NotFound);

            if (!Security.CanManipulateContent(c))
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            if (c.TargetCustomerID == ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                throw new HttpResponseException("This Challenge was sent to the current user; call /challengestatus/accept instead", System.Net.HttpStatusCode.Conflict);

            ChallengeStatus s = new ChallengeStatus();
            s.ChallengeID = c.ID;
            s.ChallengeOriginatorCustomerID = c.CustomerID;
            s.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            s.UniqueID = System.Guid.NewGuid().ToString();
            s.Status = (int)ChallengeStatus.StatusCodes.Accepted;
            StatusRepo.Add(s);

            return s;
        }

        // PUT /api/challenge/5
        [HttpPut]
        public void Put(int id, string value)
        {
        }
        
        [HttpDelete]
        // DELETE /api/challenge/5
        public void Delete(int id)
        {
        }
    }
}
