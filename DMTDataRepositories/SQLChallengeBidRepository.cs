using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class ChallengeBidRepository : IChallengeBidRepository
    {
        private DMTDataRepositories.Database.DYDbEntities db;

        public ChallengeBidRepository()
        {
            db = new DMTDataRepositories.Database.DYDbEntities();
        }

        private DMTDataRepositories.Database.Bid BidToDbBid(ChallengeBid b)
        {
            DMTDataRepositories.Database.Bid d = new DMTDataRepositories.Database.Bid();

            if (b.ID > 0)
                d.ID = b.ID;
            d.CustomerID = b.CustomerID;
            d.Amount = b.Amount;
            d.ChallengeID = b.ChallengeID;
            d.ComputedFees = b.ComputedFees;
            return d;
        }

        private ChallengeBid DbBidToBid(DMTDataRepositories.Database.Bid d)
        {
            ChallengeBid b = new ChallengeBid();

            b.Amount = d.Amount;
            b.ChallengeID = (long)d.ChallengeID;
            b.CustomerID = d.CustomerID;
            b.ID = d.ID;

            if (d.ComputedFees != null)
                b.ComputedFees = (decimal)d.ComputedFees;
            else
                b.ComputedFees = 0m;

            return b;
        }

        public void Add(ChallengeBid bid)
        {
            DMTDataRepositories.Database.Bid dbBid = BidToDbBid(bid);
            db.Bid.AddObject(dbBid);
            db.SaveChanges();
        }

        public List<ChallengeBid> Get(long ChallengeID)
        {
            IEnumerable<DMTDataRepositories.Database.Bid> bids = (from a in db.Bid where a.ChallengeID == ChallengeID select a).AsEnumerable<DMTDataRepositories.Database.Bid>();
            List<ChallengeBid> outBids = new List<ChallengeBid>();

            foreach (DMTDataRepositories.Database.Bid b in bids)
            {
                outBids.Add(DbBidToBid(b));
            }

            return outBids;
        }

        public List<ChallengeBid> GetForCustomer(long CustomerID)
        {
            IEnumerable<DMTDataRepositories.Database.Bid> bids = (from a in db.Bid where a.CustomerID == CustomerID select a).AsEnumerable<DMTDataRepositories.Database.Bid>();
            List<ChallengeBid> outBids = new List<ChallengeBid>();

            foreach (DMTDataRepositories.Database.Bid b in bids)
            {
                outBids.Add(DbBidToBid(b));
            }

            return outBids;
        }

        public ChallengeBid CustomerDidBidOnChallenge(long CustomerID, long ChallengeID)
        {
            try
            {
                DMTDataRepositories.Database.Bid bid = (from a in db.Bid where a.CustomerID == CustomerID && a.ChallengeID == ChallengeID select a).SingleOrDefault<DMTDataRepositories.Database.Bid>();
                if (bid != null && bid.CustomerID == CustomerID && bid.ChallengeID == ChallengeID)
                    return DbBidToBid(bid);
            }
            catch (Exception e)
            {
                return null;
            }

            return null;
        }

        public void Update(ChallengeBid bid)
        {
            db.Bid.Attach(BidToDbBid(bid));

            /*
            Database.Bid dbBid=db.Bid.SingleOrDefault<Database.Bid>(b => b.ID == bid.ID);

            dbBid.Amount = bid.Amount;
            dbBid.ChallengeID = bid.ChallengeID;
            dbBid.CustomerID = bid.CustomerID;
            dbBid.Status = bid.Status;
            */

            db.SaveChanges();
        }

        public int GetBidCountForChallenge(long ChallengeID)
        {
            IEnumerable<DMTDataRepositories.Database.Bid> bids = (from a in db.Bid where a.ChallengeID == ChallengeID select a).AsEnumerable<DMTDataRepositories.Database.Bid>();
            return bids.Count();
        }

        public List<ChallengeBid> GetVotePendingBidsForCustomer(long CustomerID)
        {
            throw new NotImplementedException();
        }

        public List<ChallengeBid> GetActiveBidsForCustomer(long CustomerID)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatusForBidsOnChallenge(long ChallengeID, ChallengeBid.BidStatusCodes NewStatus)
        {
            throw new NotImplementedException();
        }
    }
}