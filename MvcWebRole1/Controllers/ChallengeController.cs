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
        [HttpGet]
        public List<Challenge> Get(int StartAt=0, int Limit=10)
        {
            List<Challenge> chals = ChalRepo.GetNewest(0, 10);

            foreach (Challenge c in chals)
            {
                Customer tempCust =  CustRepo.GetWithID(c.CustomerID);
                c.Customer = new Customer();
                c.Customer.FirstName = tempCust.FirstName;
                c.Customer.LastName = tempCust.LastName;
                c.Customer.AvatarURL = tempCust.AvatarURL;
                tempCust = null;

                if (c.TargetCustomerID > 0)
                {
                    Customer tempTargetCust = CustRepo.GetWithID(c.TargetCustomerID);
                    if (tempTargetCust.FirstName != null && !tempTargetCust.FirstName.Equals(""))
                    {
                        c.TargetCustomer = new Customer();
                        c.TargetCustomer.FirstName = tempTargetCust.FirstName;
                        c.TargetCustomer.LastName = tempTargetCust.LastName;
                        c.TargetCustomer.AvatarURL = tempTargetCust.AvatarURL;
                    }
                    tempTargetCust = null;
                }

                c.NumberOfTakers = StatusRepo.GetActiveStatusesForChallenge(c.ID).Count;
            }

            return chals;
        }

        // GET /api/challenge/5
        [HttpGet]
        public Challenge Get(long id)
        {
            Challenge c = ChalRepo.Get(id);

            Customer tempCust = CustRepo.GetWithID(c.CustomerID);
            c.Customer = new Customer();
            c.Customer.FirstName = tempCust.FirstName;
            c.Customer.LastName = tempCust.LastName;
            c.Customer.AvatarURL = tempCust.AvatarURL;
            tempCust = null;

            if (c.TargetCustomerID > 0)
            {
                Customer tempTargetCust = CustRepo.GetWithID(c.TargetCustomerID);
                if (tempTargetCust.FirstName != null && !tempTargetCust.FirstName.Equals(""))
                {
                    c.TargetCustomer = new Customer();
                    c.TargetCustomer.FirstName = tempTargetCust.FirstName;
                    c.TargetCustomer.LastName = tempTargetCust.LastName;
                    c.TargetCustomer.AvatarURL = tempTargetCust.AvatarURL;
                }
                tempTargetCust = null;
            }

            c.NumberOfTakers = StatusRepo.GetActiveStatusesForChallenge(c.ID).Count;
            c.Bids = BidRepo.Get(c.ID);

            return c;
        }

        [HttpGet]
        public List<ChallengeStatus> Status(long id)
        {
            return StatusRepo.GetActiveStatusesForChallenge(id);
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
