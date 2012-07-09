using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class Challenge
    {
        public enum ChallengePrivacy
        {
            Public=0,
            FriendsOnly=1,
            Geotargeted=2,
            SinglePerson=3
        }

        public enum ChallengeState
        {
            Open,
            Accepted,
            BidsClosed,
            Completed,
            Failed,
            Expired,
            Rejected,
            Paid,
            PartialPaid
        }

        public enum ChallengeVisibility
        {
            Public,
            Private
        }

        public long ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Privacy { get; set; }
        public int Visibility { get; set; }
        public decimal CurrentBid { get; set; }
        public int State { get; set; }
        public int Anonymous { get; set; }
        public long CustomerID { get; set; }
        public long TargetCustomerID { get; set; }
        public ChallengeStatus Status { get; set; }
        public Customer Customer { get; set; }
        public Customer TargetCustomer { get; set; }
        public int NumberOfTakers { get; set; }
        public int NumberOfBidders { get; set; }
        public int Disposition { get; set; }
        public ChallengeBid Bid { get; set; }
        public List<ChallengeBid> Bids { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }

        public Challenge()
        {
            ID = 0;
            Title = "";
            Description = "";
            Privacy = 0;
            Visibility = 0;
            CurrentBid = 0;
            State = 0;
            Anonymous = 0;
            CustomerID = 0;
            TargetCustomerID = 0;
            NumberOfTakers = 0;
            Status = null;
            Customer = null;
        }
    }

    public class NewChallenge : Challenge
    {
        // for inbound challenge create requests,
        // add some fields so we can target non-users
        // and create unclaimed customer records for them.
        public string EmailAddress { get; set; }
        public Customer.ForeignUserTypes ForeignNetworkType { get; set; }
        public string ForeignNetworkUserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public NewChallenge()
            : base()
        {
            EmailAddress = "";
            ForeignNetworkType = Models.Customer.ForeignUserTypes.Undefined;
            FirstName = "";
            LastName = "";
            ForeignNetworkUserID = "";
        }
    }
}