using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class ChallengeBidRepository : IChallengeBidRepository
    {
        private Database.DYDbEntities db;

        public ChallengeBidRepository()
        {
            db = new Database.DYDbEntities();
        }

        private Database.Bid BidToDbBid(ChallengeBid b)
        {
            Database.Bid d = new Database.Bid();

            if (b.ID > 0)
                d.ID = b.ID;
            d.CustomerID = b.CustomerID;
            d.Amount = b.Amount;
            d.ChallengeID = b.ChallengeID;

            return d;
        }

        private ChallengeBid DbBidToBid(Database.Bid d)
        {
            ChallengeBid b = new ChallengeBid();

            b.Amount = d.Amount;
            b.ChallengeID = (long)d.ChallengeID;
            b.CustomerID = d.CustomerID;
            b.ID = d.ID;

            return b;
        }

        public void Add(ChallengeBid bid)
        {
            Database.Bid dbBid = BidToDbBid(bid);
            db.Bid.AddObject(dbBid);
            db.SaveChanges();
        }

        public List<ChallengeBid> Get(long ChallengeID)
        {
            IEnumerable<Database.Bid> bids=(from a in db.Bid where a.ChallengeID==ChallengeID select a).AsEnumerable<Database.Bid>();
            List<ChallengeBid> outBids = new List<ChallengeBid>();

            foreach (Database.Bid b in bids)
            {
                outBids.Add(DbBidToBid(b));
            }

            return outBids;
        }

        public List<ChallengeBid> GetForCustomer(long CustomerID)
        {
            IEnumerable<Database.Bid> bids = (from a in db.Bid where a.CustomerID == CustomerID select a).AsEnumerable<Database.Bid>();
            List<ChallengeBid> outBids = new List<ChallengeBid>();

            foreach (Database.Bid b in bids)
            {
                outBids.Add(DbBidToBid(b));
            }

            return outBids;
        }

        public ChallengeBid CustomerDidBidOnChallenge(long CustomerID, long ChallengeID)
        {
            try
            {
                Database.Bid bid = (from a in db.Bid where a.CustomerID == CustomerID && a.ChallengeID == ChallengeID select a).SingleOrDefault<Database.Bid>();
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
            IEnumerable<Database.Bid> bids = (from a in db.Bid where a.ChallengeID == ChallengeID select a).AsEnumerable<Database.Bid>();
            return bids.Count();
        }
    }
}