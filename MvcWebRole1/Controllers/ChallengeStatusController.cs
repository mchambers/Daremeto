﻿using System;
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
        private IFriendshipRepository FriendRepo;

        public ChallengeStatusController()
        {
            StatusRepo = RepoFactory.GetChallengeStatusRepo();
            ChalRepo = RepoFactory.GetChallengeRepo();
            Security = new Security();
            BidRepo = RepoFactory.GetChallengeBidRepo();
            VoteRepo = RepoFactory.GetChallengeStatusVoteRepo();
            CustRepo = RepoFactory.GetCustomerRepo();
            FriendRepo = RepoFactory.GetFriendshipRepo();
        }

        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void AcceptClaim(ChallengeStatus status)
        {
            ChallengeStatus s = StatusRepo.Get(status.CustomerID, status.ChallengeID);

            ChallengeBid bid = BidRepo.CustomerDidBidOnChallenge(((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID, s.ChallengeID);
            if (bid==null)
                throw new HttpResponseException("You can't deny a claim on a challenge you don't have a stake in.", System.Net.HttpStatusCode.Forbidden);

            ChallengeStatusVote vote = new ChallengeStatusVote();
            vote.ChallengeID = s.ChallengeID;
            vote.CustomerID = s.CustomerID;
            vote.BidderCustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            vote.Accepted = true;

            VoteRepo.Add(vote);

            int yesVotes = VoteRepo.GetYesVotes(s);
            if (yesVotes > (BidRepo.GetBidCountForChallenge(status.ChallengeID) * 0.33))
            {
                //bid.Status = (int)ChallengeBid.BidStatusCodes.BidderAccepts;
                //BidRepo.Update(bid);
                s.Status = (int)ChallengeStatus.StatusCodes.Accepted;
                StatusRepo.Update(s);

                // challenge completed! award the money! DO IT DO IT!
                CustomerNotifier.NotifyChallengeAwardedToYou(s.CustomerID, s.ChallengeID);
            }
            else
            {
                // keep waitin'.
                

            }
        }
        
        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void RejectClaim(ChallengeStatus status)
        {
            ChallengeStatus s = StatusRepo.Get(status.CustomerID, status.ChallengeID);
            
            ChallengeBid bid=BidRepo.CustomerDidBidOnChallenge(((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID, s.ChallengeID);
            if (bid==null)
                throw new HttpResponseException("You can't deny a claim on a challenge you don't have a stake in.", System.Net.HttpStatusCode.Forbidden);

            // add the "no" vote
            ChallengeStatusVote vote = new ChallengeStatusVote();
            vote.ChallengeID = s.ChallengeID;
            vote.CustomerID = s.CustomerID;
            vote.BidderCustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            vote.Accepted = false;

            VoteRepo.Add(vote);

            int noVotes=VoteRepo.GetNoVotes(s);
            if(noVotes > (BidRepo.GetBidCountForChallenge(status.ChallengeID)*0.66))
            {
                //bid.Status=(int)ChallengeBid.BidStatusCodes.BidderRejects;
                //BidRepo.Update(bid);
                s.Status = (int)ChallengeStatus.StatusCodes.SourceRejected;
                StatusRepo.Update(s);

                // you've failed this challenge my friend.
                CustomerNotifier.NotifyChallengeRejected(s.CustomerID, s.ChallengeID);
            }
            else
            {

            }

        }

        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void Claim(ChallengeStatus status)
        {
            ChallengeStatus s = StatusRepo.Get(status.CustomerID, status.ChallengeID);
            Challenge c = ChalRepo.Get(status.ChallengeID);

            if (s.CustomerID != ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                throw new HttpResponseException("This challenge status doesn't belong to the current user.", System.Net.HttpStatusCode.Forbidden);

            s.Status = (int)ChallengeStatus.StatusCodes.ClaimSubmitted;
            StatusRepo.Update(s);

            // close the bidding on this challenge until the claim can be verified.
            c.State = (int)Challenge.ChallengeState.BidsClosed;
            ChalRepo.Update(c);
            
            // set all of the bids for this challenge to "VotePending"
            // so the bidders can see what they need to vote on
            BidRepo.UpdateStatusForBidsOnChallenge(status.ChallengeID, ChallengeBid.BidStatusCodes.VotePending);

            CustomerNotifier.NotifyChallengeClaimed(s.ChallengeOriginatorCustomerID, s.CustomerID, c.ID);
        }

        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void Accept(ChallengeStatus status)
        {
            Challenge c = ChalRepo.Get(status.ChallengeID);
            ChallengeStatus s = StatusRepo.Get(((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID, status.ChallengeID);

            if (s == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            if (!Security.CanManipulateContent(c))
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            s.Status = (int)ChallengeStatus.StatusCodes.Accepted;

            StatusRepo.Update(s);

            // add a friendship between these folk if one doesn't exist.
            if (!FriendRepo.CustomersAreFriends(s.ChallengeOriginatorCustomerID, ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID))
                FriendRepo.Add(s.ChallengeOriginatorCustomerID, ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID);

            c.State = (int)Challenge.ChallengeState.Accepted;
            ChalRepo.Update(c);

            CustomerNotifier.NotifyChallengeAccepted(s.ChallengeOriginatorCustomerID, s.CustomerID, c.ID);
        }

        // Reject is specifically for when a Challenge has been sent directly to you
        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void Reject(ChallengeStatus status)
        {
            Challenge c = ChalRepo.Get((int)status.ChallengeID);

            if (c.Privacy != (int)Challenge.ChallengePrivacy.SinglePerson || c.TargetCustomerID != ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotImplemented);

            ChallengeStatus s = StatusRepo.Get(status.CustomerID, status.ChallengeID);

            if (s == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            if (!Security.CanManipulateContent(c))
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            s.Status = (int)ChallengeStatus.StatusCodes.TargetRejected;

            StatusRepo.Update(s);

            // the recipient has rejected this challenge. it dies here.
            // the creator will have to make a new one to re-issue it.
            c.State = (int)Challenge.ChallengeState.Rejected;
            ChalRepo.Update(c);
        }
    }
}