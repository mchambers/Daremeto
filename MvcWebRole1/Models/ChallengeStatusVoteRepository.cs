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
        TableServiceContext context;

        private const string TableName = "ChalStatusVotes";

        public ChallengeStatusVoteRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();

            client.CreateTableIfNotExist(TableName);

            context = client.GetDataServiceContext();
        }

        private ChallengeStatusVoteDb VoteToDbVote(ChallengeStatusVote v)
        {
            ChallengeStatusVoteDb d = new ChallengeStatusVoteDb();

            d.Accepted = v.Accepted;
            d.ChallengeID = v.ChallengeID;
            d.ChallengeStatusUniqueID = v.ChallengeStatusUniqueID;
            d.UniqueID = v.UniqueID;

            d.PartitionKey = v.ChallengeID + "_" + v.ChallengeStatusUniqueID;
            d.RowKey = v.UniqueID;

            return d;
        }

        private ChallengeStatusVote DbVoteToVote(ChallengeStatusVoteDb d)
        {
            ChallengeStatusVote v = new ChallengeStatusVote();

            v.Accepted = d.Accepted;
            v.ChallengeID = d.ChallengeID;
            v.ChallengeStatusUniqueID = d.ChallengeStatusUniqueID;
            v.UniqueID = d.UniqueID;

            return v;
        }

        public void Add(ChallengeStatusVote vote)
        {
            ChallengeStatusVoteDb d = VoteToDbVote(vote);
            context.AttachTo(TableName, d);
            context.SaveChangesWithRetries();
        }

        public int GetCount(ChallengeStatus status)
        {
            int count=(from e in context.CreateQuery<ChallengeStatusVoteDb>(TableName) where e.PartitionKey==status.ChallengeID+"_"+status.UniqueID select e).Count();
            return count;
        }

        public int GetYesVotes(ChallengeStatus status)
        {
            IEnumerable<ChallengeStatusVoteDb> votes = (from e in context.CreateQuery<ChallengeStatusVoteDb>(TableName) where e.PartitionKey == status.ChallengeID + "_" + status.UniqueID select e).AsEnumerable<ChallengeStatusVoteDb>();
            return votes.Count(v => v.Accepted == true);
        }

        public int GetNoVotes(ChallengeStatus status)
        {
            IEnumerable<ChallengeStatusVoteDb> votes = (from e in context.CreateQuery<ChallengeStatusVoteDb>(TableName) where e.PartitionKey == status.ChallengeID + "_" + status.UniqueID select e).AsEnumerable<ChallengeStatusVoteDb>();
            return votes.Count(v => v.Accepted == false);
        }
    }
}