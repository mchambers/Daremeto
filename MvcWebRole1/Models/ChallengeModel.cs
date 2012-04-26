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
            Rejected
        }

        public long ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Privacy { get; set; }
        public int CurrentBid { get; set; }
        public int State { get; set; }

        public long CustomerID { get; set; }
        public long TargetCustomerID { get; set; }

        public List<ChallengeBid> Bids { get; set; }
    }
}