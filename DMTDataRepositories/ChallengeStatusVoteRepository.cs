using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Data.Services.Client;

namespace DareyaAPI.Models
{
    public class ChallengeStatusVoteRepository : IChallengeStatusVoteRepository
    {
        CloudStorageAccount storage;
        CloudTableClient client;
        TableServiceContextV2 context;

        private const string TableName = "ChalStatusVotes";

        public ChallengeStatusVoteRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();

            client.CreateTableIfNotExist(TableName);

            context = new TableServiceContextV2(client.BaseUri.ToString(), client.Credentials);
            context.MergeOption = MergeOption.NoTracking;
        }

        private string DbPartitionForTaker(long ChallengeID, long CustomerID)
        {
            return "Chal" + ChallengeID.ToString() + "Cust" + CustomerID.ToString();
        }

        private string DbRowForBidder(long CustomerID)
        {
            return "BidderCust" + CustomerID.ToString();
        }

        private ChallengeStatusVoteDb VoteToDbVote(ChallengeStatusVote v)
        {
            ChallengeStatusVoteDb d = new ChallengeStatusVoteDb();

            d.Accepted = v.Accepted;
            d.ChallengeID = v.ChallengeID;
            d.CustomerID = v.CustomerID;
            d.BidderCustomerID = v.BidderCustomerID;

            d.PartitionKey = DbPartitionForTaker(d.ChallengeID, d.CustomerID);
            d.RowKey = DbRowForBidder(d.BidderCustomerID);

            return d;
        }

        private ChallengeStatusVote DbVoteToVote(ChallengeStatusVoteDb d)
        {
            ChallengeStatusVote v = new ChallengeStatusVote();

            v.Accepted = d.Accepted;
            v.ChallengeID = d.ChallengeID;
            v.CustomerID = d.CustomerID;
            v.BidderCustomerID = d.BidderCustomerID;
            
            return v;
        }

        public void Add(ChallengeStatusVote vote)
        {
            ChallengeStatusVoteDb d = VoteToDbVote(vote);
            context.AddObject(TableName, d);
            try
            {
                context.SaveChangesWithRetries();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Status repo vote exception: Couldn't add vote " + e.ToString());
            }
        }

        public int GetCount(ChallengeStatus status)
        {
            int count=(from e in context.CreateQuery<ChallengeStatusVoteDb>(TableName) where e.PartitionKey==DbPartitionForTaker(status.ChallengeID, status.CustomerID) select e).Count();
            return count;
        }

        public int GetYesVotes(ChallengeStatus status)
        {
            IEnumerable<ChallengeStatusVoteDb> votes = (from e in context.CreateQuery<ChallengeStatusVoteDb>(TableName) where e.PartitionKey == DbPartitionForTaker(status.ChallengeID, status.CustomerID) select e).AsEnumerable<ChallengeStatusVoteDb>();
            return votes.Count(v => v.Accepted == true);
        }

        public int GetNoVotes(ChallengeStatus status)
        {
            IEnumerable<ChallengeStatusVoteDb> votes = (from e in context.CreateQuery<ChallengeStatusVoteDb>(TableName) where e.PartitionKey == DbPartitionForTaker(status.ChallengeID, status.CustomerID) select e).AsEnumerable<ChallengeStatusVoteDb>();
            return votes.Count(v => v.Accepted == false);
        }
    }
}