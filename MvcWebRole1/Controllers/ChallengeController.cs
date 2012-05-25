using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using DareyaAPI.Models;
using System.Diagnostics;

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
            ChalRepo = RepoFactory.GetChallengeRepo();
            BidRepo = RepoFactory.GetChallengeBidRepo();
            StatusRepo = RepoFactory.GetChallengeStatusRepo();
            CustRepo = RepoFactory.GetCustomerRepo();
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
            List<Challenge> chals = ChalRepo.GetNewest(0, 10).ToList<Challenge>();
            List<Challenge> outChals = new List<Challenge>(chals.Count);

            foreach (Challenge c in chals)
            {
                outChals.Add(PrepOutboundChallenge(c));
            }

            return outChals;
        }

        [HttpGet]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public List<Challenge> ActiveOutboundForCustomer(long id)
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
            List<ChallengeStatus> statuses = StatusRepo.GetChallengesBySourceCustomer(id);
            List<Challenge> challenges = new List<Challenge>();

            foreach (ChallengeStatus s in statuses)
            {
                Challenge chal = PrepOutboundChallenge(ChalRepo.Get(s.ChallengeID));
                chal.Status = s;
                challenges.Add(chal);
            }

            return challenges;
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
            Customer cust=CustRepo.GetWithID(((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID);

            if (c == null)
                throw new HttpResponseException("The requested Challenge resource doesn't exist.", System.Net.HttpStatusCode.NotFound);

            if (c.Privacy == (int)Challenge.ChallengePrivacy.FriendsOnly)
            {
                if (Security.DetermineAudience(c) < Security.Audience.Friends)
                    throw new HttpResponseException("This item is friends-only.", System.Net.HttpStatusCode.Forbidden);
            }

            decimal approximateFees;

            approximateFees=BillingSystem.BillingProcessorFactory.
                GetBillingProcessor((BillingSystem.BillingProcessorFactory.SupportedBillingProcessor)cust.BillingType).GetProcessingFeesForAmount(value.Amount);
            
            ChalRepo.AddBidToChallenge(c, ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID, value.Amount, approximateFees);
        }

        // POST /api/challenge
        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public Challenge New(NewChallenge value)
        {
            if (value.Description.Equals(""))
            {
                Trace.WriteLine("EXCEPTION: You must specify a description for a challenge", "ChallengeController::New");

                throw new HttpResponseException("You have to specify a description.", System.Net.HttpStatusCode.InternalServerError);
            }

            Trace.WriteLine("Creating a new challenge for customer " + ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID.ToString(), "ChallengeController::New");

            value.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            
            decimal firstBid = (decimal)value.CurrentBid;
            
            value.ID=ChalRepo.Add(value);

            Customer cust=CustRepo.GetWithID(((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID);

            decimal approxFees;
            try
            {
                approxFees = BillingSystem.BillingProcessorFactory.GetBillingProcessor((BillingSystem.BillingProcessorFactory.SupportedBillingProcessor)cust.BillingType).GetProcessingFeesForAmount(firstBid);
            }
            catch (Exception e)
            {
                Trace.WriteLine("NOTE: Caught customer " + ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID.ToString() + " with no billing proc., assuming Stripe", "ChallengeController::New");
                //throw new HttpResponseException("There was a fatal error generating the bid for this challenge.", System.Net.HttpStatusCode.InternalServerError);
                approxFees = BillingSystem.BillingProcessorFactory.GetBillingProcessor(BillingSystem.BillingProcessorFactory.SupportedBillingProcessor.Stripe).GetProcessingFeesForAmount(firstBid);
            }

            approxFees += BillingSystem.Billing.ComputeVigForAmount(firstBid); // we gotta get our taste.

            Trace.WriteLine("Adding a bid of " + firstBid.ToString() + " to challenge ID " + value.ID.ToString(), "ChallengeController::New");
            ChalRepo.AddBidToChallenge(value, value.CustomerID, firstBid, approxFees);

            value.CurrentBid = (firstBid - approxFees);

            bool createTargetStatus=false;

            Trace.WriteLine("The target customer type for challenge " + value.ID.ToString() + " is " + value.ForeignNetworkType.ToString());

            // Try using a supplied email address to target the challenge first...
            if (value.EmailAddress != null && !value.EmailAddress.Equals(""))
            {
                value.EmailAddress = value.EmailAddress.ToLower().Trim();

                Customer tryEMail = CustRepo.GetWithEmailAddress(value.EmailAddress);
                if (tryEMail != null && tryEMail.EmailAddress.Equals(value.EmailAddress))
                {
                    Trace.WriteLine("Found the email customer " + value.EmailAddress + " for new challenge " + value.ID.ToString(), "ChallengeController::New");
                    value.TargetCustomerID = tryEMail.ID;
                }
                else
                {
                    Trace.WriteLine("Couldn't find the email customer " + value.EmailAddress + " for new challenge " + value.ID.ToString(), "ChallengeController::New");
                    Customer unclaimedEmail = new Customer();
                    unclaimedEmail.EmailAddress = value.EmailAddress;
                    unclaimedEmail.Type = (int)Customer.TypeCodes.Unclaimed;
                    value.TargetCustomerID = CustRepo.Add(unclaimedEmail);
                }
                createTargetStatus = true;
            }
            else // then try using a supplied foreign network connection...
            {
                if (value.ForeignNetworkType != Customer.ForeignUserTypes.Undefined)
                {
                    Customer tryFN = CustRepo.GetWithForeignUserID(value.ForeignNetworkUserID, value.ForeignNetworkType);
                    if (tryFN != null && tryFN.ID > 0)
                    {
                        Trace.WriteLine("Found the foreign network customer " + value.ForeignNetworkUserID + " as customer ID " + tryFN.ID.ToString());
                        value.TargetCustomerID = tryFN.ID;
                    }
                    else
                    {
                        Customer unclaimedCust = new Customer
                        {
                            FirstName = value.FirstName,
                            LastName = value.LastName,
                            Type = (int)Customer.TypeCodes.Unclaimed
                        };

                        value.TargetCustomerID = CustRepo.Add(unclaimedCust);

                        Trace.WriteLine("Created a new foreign network customer " + value.ForeignNetworkUserID + " as customer ID " + value.TargetCustomerID.ToString());

                        CustRepo.AddForeignNetworkForCustomer(value.TargetCustomerID, value.ForeignNetworkUserID, value.ForeignNetworkType);
                    }

                    createTargetStatus = true;
                }
            }
            
            ChalRepo.Update(value);

            if (createTargetStatus)
            {
                ChallengeStatus s = new ChallengeStatus()
                {
                    ChallengeID=value.ID,
                    ChallengeOriginatorCustomerID=value.CustomerID,
                    CustomerID=value.TargetCustomerID,
                    UniqueID=System.Guid.NewGuid().ToString(),
                    Status=(int)ChallengeStatus.StatusCodes.None
                };

                StatusRepo.Add(s);

                // notify the receipient of the new challenge.
                CustomerNotifier.NotifyNewChallenge(s.ChallengeOriginatorCustomerID, s.CustomerID, s.ChallengeID);
            }

            return PrepOutboundChallenge(value);
        }

        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public ChallengeStatus Take(long id)
        {
            Trace.WriteLine("Customer " + ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID.ToString() + " wants to take challenge "+id.ToString(), "ChallengeController::Take");

            Challenge c = ChalRepo.Get(id);

            if (c == null)
                throw new HttpResponseException("The requested Challenge resource doesn't exist.", System.Net.HttpStatusCode.NotFound);

            if (!Security.CanManipulateContent(c))
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            if (c.TargetCustomerID == ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                throw new HttpResponseException("This Challenge was sent to the current user; call /challengestatus/accept instead", System.Net.HttpStatusCode.Conflict);

            if (c.CustomerID == ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                throw new HttpResponseException("This Challenge originated from the current user; you can't take your own dare", System.Net.HttpStatusCode.Conflict);
            
            ChallengeStatus s = new ChallengeStatus();
            s.ChallengeID = c.ID;
            s.ChallengeOriginatorCustomerID = c.CustomerID;
            s.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            s.UniqueID = System.Guid.NewGuid().ToString();
            s.Status = (int)ChallengeStatus.StatusCodes.Accepted;

            Trace.WriteLine("Adding 'taking this dare' status for customer " + ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID.ToString() + " and challenge " + id.ToString(), "ChallengeController::Take");
            StatusRepo.Add(s);

            CustomerNotifier.NotifyChallengeAccepted(s.ChallengeOriginatorCustomerID, s.CustomerID, c.ID);

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
