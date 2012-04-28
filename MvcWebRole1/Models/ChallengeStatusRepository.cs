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
    public class ChallengeStatusRepository : IChallengeStatusRepository
    {
        CloudStorageAccount storage;
        CloudTableClient client;
        TableServiceContext context;
        
        private const string TableName = "ChalStatus";

        public ChallengeStatusRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();

            client.CreateTableIfNotExist(TableName);

            context = client.GetDataServiceContext();
        }

        private List<ChallengeStatus> CoreGetStatuses(long ID, string Prefix)
        {
            CloudTableQuery<ChallengeStatusDb> b = (from e in context.CreateQuery<ChallengeStatusDb>(TableName) where e.PartitionKey == Prefix + ID.ToString() select e).AsTableServiceQuery<ChallengeStatusDb>();
            List<ChallengeStatus> items = new List<ChallengeStatus>();

            foreach (ChallengeStatusDb item in b)
            {
                items.Add(new ChallengeStatus(item));
            }

            return items;
        }

        public List<ChallengeStatus> GetChallengesBySourceCustomer(long CustomerID)
        {
            return CoreGetStatuses(CustomerID, "SourceCust");
        }

        public List<ChallengeStatus> GetActiveChallengesForCustomer(long CustomerID)
        {
            return CoreGetStatuses(CustomerID, "Cust");
        }

        public List<ChallengeStatus> GetActiveStatusesForChallenge(long ChallengeID)
        {
            return CoreGetStatuses(ChallengeID, "Chal");
        }

        public void Add(ChallengeStatus value)
        {
            CoreAddOrUpdate(value);
        }

        private void CoreAddOrUpdate(ChallengeStatus value)
        {
            ChallengeStatusDb sourceCust = new ChallengeStatusDb(value);
            ChallengeStatusDb targetCust = new ChallengeStatusDb(value);
            ChallengeStatusDb chal = new ChallengeStatusDb(value);

            targetCust.PartitionKey = "Cust" + value.CustomerID;
            sourceCust.PartitionKey = "SourceCust" + value.ChallengeOriginatorCustomerID;
            chal.PartitionKey = "Chal" + value.ChallengeID;

            context.AttachTo(TableName, sourceCust);
            context.AttachTo(TableName, targetCust);
            context.AttachTo(TableName, chal);

            context.SaveChangesWithRetries(SaveChangesOptions.ContinueOnError);   
        }

        public void Update(ChallengeStatus value)
        {
            if (value.UniqueID == null || value.UniqueID.Equals(""))
                throw new InvalidOperationException("UniqueID is a required parameter");

            CoreAddOrUpdate(value);

            // update all three partitions using the rowkey
        }

        public ChallengeStatus Get(long id, string key)
        {
            ChallengeStatusDb s = (from e in context.CreateQuery<ChallengeStatusDb>(TableName) where e.PartitionKey == "Chal"+id.ToString() && e.RowKey == key select e).FirstOrDefault();
            return new ChallengeStatus(s);
        }
    }
}