using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DareyaAPI.Models
{
    class ChallengeRepoDapper : IChallengeRepository
    {
        private string connStr;

        public ChallengeRepoDapper()
        {
            connStr = ConfigurationManager.ConnectionStrings["DMTPrimary"].ConnectionString;
        }

        private DynamicParameters ChalToDynParm(Challenge c, bool inclID = false)
        {
            var p = new DynamicParameters();

            p.Add("@Description", c.Description, DbType.String, ParameterDirection.Input);
            p.Add("@Privacy", c.Privacy, DbType.Int32, ParameterDirection.Input);
            p.Add("@Visibility", c.Visibility, DbType.Int32, ParameterDirection.Input);
            p.Add("@State", c.State, DbType.Int32, ParameterDirection.Input);
            p.Add("@Anonymous", c.Anonymous, DbType.Int32, ParameterDirection.Input);
            p.Add("@CustomerID", c.CustomerID, DbType.Int64, ParameterDirection.Input);
            p.Add("@TargetCustomerID", c.TargetCustomerID, DbType.Int64, ParameterDirection.Input);
            if (inclID)
                p.Add("@ID", c.ID, DbType.Int64, ParameterDirection.Input);

            return p;
        }

        public Challenge Get(long id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var c = conn.Query<Challenge>("spChallengeGet", new { ID = id }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return c;
            }
        }

        public long Add(Challenge item)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                DynamicParameters p = ChalToDynParm(item);
                p.Add("@InsertID", dbType: DbType.Int64, direction: ParameterDirection.Output);
                conn.Execute("spChallengeAdd", p, commandType: CommandType.StoredProcedure);
                return p.Get<long>("InsertID");
            }
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Challenge item)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                DynamicParameters p = ChalToDynParm(item, true);
                return (conn.Execute("spChallengeUpdate", p, commandType: CommandType.StoredProcedure) != 0);                
            }
        }

        public void AddBidToChallenge(Challenge item, long lCustomerID, decimal dBidAmount, decimal dComputedFees)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                db.Execute("AddBidToChallenge", new { ChallengeID = item.ID, BidAmount = dBidAmount, CustomerID = lCustomerID, FeesAmount = dComputedFees }, commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<Challenge> GetNewest(int startAt, int amount)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                var chals = db.Query<Challenge>("spChallengeGetNewest", new { StartAt = startAt, Amount = amount }, commandType: CommandType.StoredProcedure).AsEnumerable();
                return chals;
            }
        }

        public IEnumerable<Challenge> GetListForUser(long userID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Challenge> GetListForUser(long userID, int status)
        {
            throw new NotImplementedException();
        }

        public int MoveChallengesToCustomer(long SourceCustomerID, long NewTargetCustomerID)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                int numAffected = db.Execute("spCollapseCustomerIntoCustomer", new { OriginalCustomerID = SourceCustomerID, NewCustomerID = NewTargetCustomerID }, commandType: CommandType.StoredProcedure);
                return numAffected;
            }
        }
        
        public IEnumerable<Challenge> GetOpen(int startAt, int amount)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                var chals = db.Query<Challenge>("spChallengeGetOpen", new { StartAt = startAt, Amount = amount }, commandType: CommandType.StoredProcedure).AsEnumerable();
                return chals;
            }
        }

        public IEnumerable<Challenge> GetOpenForCustomer(long CustomerID)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                var chals = db.Query<Challenge>("spChallengeGetOpenForCustomer", new { CustomerID = CustomerID }, commandType: CommandType.StoredProcedure).AsEnumerable();
                return chals;
            }
        }
    }
}
