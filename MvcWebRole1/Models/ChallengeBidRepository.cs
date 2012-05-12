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
    public class AZTChallengeBidRepository : IChallengeBidRepository
    {
        CloudStorageAccount storage;
        CloudTableClient client;
        TableServiceContext context;

        private const string TableName = "ChallengeBid";

        public AZTChallengeBidRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();
            client.CreateTableIfNotExist(TableName);
            context = client.GetDataServiceContext();
        }

        public List<ChallengeBid> Get(long ChallengeID)
        {
            CloudTableQuery<ChallengeBidDb> b = (from e in context.CreateQuery<ChallengeBidDb>(TableName) where e.PartitionKey == "CBids"+ChallengeID.ToString() select e).AsTableServiceQuery<ChallengeBidDb>();
            List<ChallengeBid> items=new List<ChallengeBid>();

            foreach (ChallengeBidDb item in b)
            {
                items.Add(new ChallengeBid(item));
            }

            return items;
        }

        public void Add(ChallengeBid b)
        {
            context.AddObject(TableName, new ChallengeBidDb(b));
            context.SaveChangesWithRetries();
        }

        public List<ChallengeBid> GetForCustomer(long CustomerID)
        {
            throw new NotImplementedException();
        }


        public void Update(ChallengeBid bid)
        {
            throw new NotImplementedException();
        }

        public ChallengeBid CustomerDidBidOnChallenge(long CustomerID, long ChallengeID)
        {
            throw new NotImplementedException();
        }

        public int GetBidCountForChallenge(long ChallengeID)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatusForBidsOnChallenge(long ChallengeID, string UniqueID, ChallengeBid.BidStatusCodes NewStatus)
        {
            throw new NotImplementedException();
        }


        public List<ChallengeBid> GetVotePendingBidsForCustomer(long CustomerID)
        {
            throw new NotImplementedException();
        }
    }
}