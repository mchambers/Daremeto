using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.Configuration;

namespace DareyaAPI.Models
{
    class BidRepoDapper : IChallengeBidRepository
    {
        private string connStr;

        public BidRepoDapper()
        {
            connStr = ConfigurationManager.ConnectionStrings["DMTPrimary"].ConnectionString;
        }

        public void Add(ChallengeBid bid)
        {
            throw new NotImplementedException();
        }

        public List<ChallengeBid> Get(long chalID)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                var bids=db.Query<ChallengeBid>("spBidGetForChallenge", new { ChallengeID = chalID }, commandType: CommandType.StoredProcedure).AsEnumerable();
                return bids.ToList<ChallengeBid>();
            }
        }

        public List<ChallengeBid> GetForCustomer(long custID)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                var bids = db.Query<ChallengeBid>("spBidGetForCustomer", new { CustomerID = custID }, commandType: CommandType.StoredProcedure).AsEnumerable();
                return bids.ToList<ChallengeBid>();
            }
        }

        public void Update(ChallengeBid bid)
        {
            throw new NotImplementedException();
        }

        public ChallengeBid CustomerDidBidOnChallenge(long custID, long chalID)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                var bid = db.Query<ChallengeBid>("spBidCustomerBidForChallenge", new { CustomerID = custID, ChallengeID = chalID }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return bid;
            }
        }

        public int GetBidCountForChallenge(long ChallengeID)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();

                DynamicParameters p = new DynamicParameters();
                p.Add("@ChallengeID", ChallengeID, DbType.Int64, ParameterDirection.Input);
                p.Add("@BidCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                db.Execute("spBidCountForChallenge", p, commandType: CommandType.StoredProcedure);
                return p.Get<int>("@BidCount");
            }
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
