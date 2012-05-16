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
            CoreAddOrUpdate(value, true);
        }

        private void CoreAddOrUpdate(ChallengeStatus value, bool Add=false)
        {
            ChallengeStatusDb sourceCust = new ChallengeStatusDb(value);
            ChallengeStatusDb targetCust = new ChallengeStatusDb(value);
            ChallengeStatusDb chal = new ChallengeStatusDb(value);

            targetCust.PartitionKey = "Cust" + value.CustomerID;
            sourceCust.PartitionKey = "SourceCust" + value.ChallengeOriginatorCustomerID;
            chal.PartitionKey = "Chal" + value.ChallengeID;

            if (Add)
                context.AddObject(TableName, sourceCust);
            else
            {
                context.AttachTo(TableName, sourceCust, null);
                context.UpdateObject(sourceCust);
            }

            if (Add)
                context.AddObject(TableName, targetCust);
            else
            {
                context.AttachTo(TableName, targetCust, null);
                context.UpdateObject(targetCust);
            }

            if (Add)
                context.AddObject(TableName, chal);
            else
            {
                context.AttachTo(TableName, chal, null);
                context.UpdateObject(chal);
            }

            context.SaveChangesWithRetries();

            context.Detach(sourceCust);
            context.Detach(targetCust);
            context.Detach(chal);
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
            ChallengeStatus outS = new ChallengeStatus(s);
            context.Detach(s);
            return outS;
        }

        public List<ChallengeStatus> GetActiveChallengesBySourceCustomer(long CustomerID)
        {
            CloudTableQuery<ChallengeStatusDb> b = (from e in context.CreateQuery<ChallengeStatusDb>(TableName) where e.PartitionKey == "SourceCust" + CustomerID.ToString() select e).AsTableServiceQuery<ChallengeStatusDb>();
            List<ChallengeStatus> items = new List<ChallengeStatus>();

            foreach (ChallengeStatusDb item in b)
            {
                items.Add(new ChallengeStatus(item));
                context.Detach(item);
            }

            return items;
        }
    }
}