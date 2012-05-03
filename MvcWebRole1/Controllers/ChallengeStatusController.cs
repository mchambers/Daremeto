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
        private IChallengeBidRepository BidRepo;
        private IChallengeStatusVoteRepository VoteRepo;
        private ICustomerRepository CustRepo;

        public ChallengeStatusController()
        {
            StatusRepo = new ChallengeStatusRepository();
            ChalRepo = new ChallengeRepository();
            Security = new Security();
            BidRepo = new ChallengeBidRepository();
            VoteRepo = new ChallengeStatusVoteRepository();
            CustRepo = new CustomerRepository();
        }

        public void PostAcceptClaim(ChallengeStatus status)
        {
            ChallengeStatus s = StatusRepo.Get(status.ChallengeID, status.UniqueID);

            ChallengeBid bid = BidRepo.CustomerDidBidOnChallenge(((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID, s.ChallengeID);
            if (bid==null)
                throw new HttpResponseException("You can't deny a claim on a challenge you don't have a stake in.", System.Net.HttpStatusCode.Forbidden);

            ChallengeStatusVote vote = new ChallengeStatusVote();
            vote.ChallengeID = s.ChallengeID;
            vote.ChallengeStatusUniqueID = s.UniqueID;
            vote.Accepted = true;

            VoteRepo.Add(vote);

            int yesVotes = VoteRepo.GetYesVotes(s);
            if (yesVotes > (BidRepo.GetBidCountForChallenge(status.ChallengeID) * 0.33))
            {
                bid.Status = (int)ChallengeBid.BidStatusCodes.BidderAccepts;
                BidRepo.Update(bid);

                // challenge completed! award the money! DO IT DO IT!
                Email.SendChallengeAwardedToYouEmail(CustRepo.GetWithID(s.CustomerID), ChalRepo.Get(s.ChallengeID));
            }
            else
            {
                // keep waitin'.
                

            }
        }

        public void PostRejectClaim(ChallengeStatus status)
        {
            ChallengeStatus s = StatusRepo.Get(status.ChallengeID, status.UniqueID);
            
            ChallengeBid bid=BidRepo.CustomerDidBidOnChallenge(((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID, s.ChallengeID);
            if (bid==null)
                throw new HttpResponseException("You can't deny a claim on a challenge you don't have a stake in.", System.Net.HttpStatusCode.Forbidden);

            // add the "no" vote
            ChallengeStatusVote vote = new ChallengeStatusVote();
            vote.ChallengeID = s.ChallengeID;
            vote.ChallengeStatusUniqueID = s.UniqueID;
            vote.Accepted = false;

            VoteRepo.Add(vote);

            int noVotes=VoteRepo.GetNoVotes(s);
            if(noVotes > (BidRepo.GetBidCountForChallenge(status.ChallengeID)*0.66))
            {
                bid.Status=(int)ChallengeBid.BidStatusCodes.BidderRejects;
                BidRepo.Update(bid);

                // you've failed this challenge my friend.
                Email.SendChallengeRejectedEmail(CustRepo.GetWithID(s.CustomerID), ChalRepo.Get(s.ChallengeID));
            }
            else
            {

            }

        }

        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void PostClaim(ChallengeStatus status)
        {
            ChallengeStatus s = StatusRepo.Get(status.ChallengeID, status.UniqueID);
            Challenge c = ChalRepo.Get(status.ChallengeID);

            if (s.CustomerID != ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                throw new HttpResponseException("This challenge status doesn't belong to the current user.", System.Net.HttpStatusCode.Forbidden);

            s.Status = (int)ChallengeStatus.StatusCodes.ClaimSubmitted;
            StatusRepo.Update(s);

            // close the bidding on this challenge until the claim can be verified.
            c.State = (int)Challenge.ChallengeState.BidsClosed;
            ChalRepo.Update(c);

            Email.SendChallengeClaimedEmail(CustRepo.GetWithID(s.ChallengeOriginatorCustomerID), CustRepo.GetWithID(s.CustomerID), c);
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

            // transition this to the Bids Closed state
            // no more bids can be put in while the request is being reviewed.
            c.State = (int)Challenge.ChallengeState.BidsClosed;
            ChalRepo.Update(c);

            Email.SendChallengeAcceptedEmail(CustRepo.GetWithID(s.ChallengeOriginatorCustomerID), CustRepo.GetWithID(s.CustomerID), c);
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

            // the recipient has rejected this challenge. it dies here.
            // the creator will have to make a new one to re-issue it.
            c.State = (int)Challenge.ChallengeState.Rejected;
            ChalRepo.Update(c);
        }
    }
}