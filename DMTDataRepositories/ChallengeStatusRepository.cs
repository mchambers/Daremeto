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
        TableServiceContextV2 context;
        
        private const string TableName = "ChalStatus";

        public ChallengeStatusRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();

            client.CreateTableIfNotExist(TableName);

            //context = client.GetDataServiceContext();
            context = new TableServiceContextV2(client.BaseUri.ToString(), client.Credentials);
            context.IgnoreResourceNotFoundException = true;
        }

        private string DbCustKey(long CustomerID)
        {
            return "Cust" + CustomerID.ToString();
        }

        private string DbChalKey(long ChallengeID)
        {
            return "Chal" + ChallengeID.ToString();
        }

        private List<ChallengeStatus> CoreGetStatuses(long ID, string Prefix)
        {
            CloudTableQuery<ChallengeStatusDb> b = (from e in context.CreateQuery<ChallengeStatusDb>(TableName) where e.PartitionKey == Prefix + ID.ToString() select e).AsTableServiceQuery<ChallengeStatusDb>();
            List<ChallengeStatus> items = new List<ChallengeStatus>();

            foreach (ChallengeStatusDb item in b)
            {
                items.Add(new ChallengeStatus(item));
                context.Detach(item);
            }

            return items;
        }

        public bool CustomerTookChallenge(long CustomerID, long ChallengeID)
        {
            ChallengeStatusDb b = (from e in context.CreateQuery<ChallengeStatusDb>(TableName) where e.PartitionKey == DbCustKey(CustomerID) && e.RowKey == DbChalKey(ChallengeID) select e).FirstOrDefault<ChallengeStatusDb>();
            if (b != null)
            {
                context.Detach(b);
                return true;
            }
            return false;
        }

        public List<ChallengeStatus> GetChallengesBySourceCustomer(long CustomerID)
        {
            throw new NotImplementedException();
        }

        public List<ChallengeStatus> GetActiveChallengesForCustomer(long CustomerID)
        {
            CloudTableQuery<ChallengeStatusDb> c = (from e in context.CreateQuery<ChallengeStatusDb>(TableName) where e.PartitionKey == DbCustKey(CustomerID) select e).AsTableServiceQuery<ChallengeStatusDb>();

            List<ChallengeStatus> l = new List<ChallengeStatus>();
            //foreach (ChallengeStatusDb d in c.TakeWhile<ChallengeStatusDb>(i => i.Status!=(int)ChallengeStatus.StatusCodes.Paid))
            foreach (ChallengeStatusDb d in c)
            {
                l.Add(new ChallengeStatus(d));
                context.Detach(d);
            }

            return l;
        }

        public List<ChallengeStatus> GetActiveStatusesForChallenge(long ChallengeID)
        {
            CloudTableQuery<ChallengeStatusDb> c = (from e in context.CreateQuery<ChallengeStatusDb>(TableName) where e.PartitionKey == DbChalKey(ChallengeID) select e).AsTableServiceQuery<ChallengeStatusDb>();

            List<ChallengeStatus> l = new List<ChallengeStatus>();
            foreach (ChallengeStatusDb d in c)
            {
                l.Add(new ChallengeStatus(d));
                context.Detach(d);
            }

            return l;
        }

        public void Add(ChallengeStatus value)
        {
            CoreAddOrUpdate(value, true);
        }

        private void CoreAddOrUpdate(ChallengeStatus value, bool Add=false)
        {
            ChallengeStatusDb targetCust = new ChallengeStatusDb(value);
            ChallengeStatusDb chal = new ChallengeStatusDb(value);

            string targetCustKey = DbCustKey(value.CustomerID);
            string chalKey = DbChalKey(value.ChallengeID);

            targetCust.PartitionKey = targetCustKey;
            targetCust.RowKey=chalKey;

            chal.PartitionKey = chalKey;
            chal.RowKey = targetCustKey;
            
            context.AttachTo(TableName, chal, null);
            context.UpdateObject(chal);

            context.AttachTo(TableName, targetCust, null);
            context.UpdateObject(targetCust);

            context.SaveChangesWithRetries();

            context.Detach(targetCust);
            context.Detach(chal);
        }
        
        public void Update(ChallengeStatus value)
        {
            CoreAddOrUpdate(value);
        }

        public List<ChallengeStatus> GetActiveChallengesBySourceCustomer(long CustomerID)
        {
            throw new NotImplementedException();
        }

        public ChallengeStatus Get(long CustomerID, long ChallengeID)
        {
            ChallengeStatusDb d = (from e in context.CreateQuery<ChallengeStatusDb>(TableName) where e.PartitionKey == DbCustKey(CustomerID) && e.RowKey == DbChalKey(ChallengeID) select e).FirstOrDefault<ChallengeStatusDb>();

            if (d == null) return null;
            ChallengeStatus s = new ChallengeStatus(d);
            context.Detach(d);
            return s;
        }

        // TODO: We should just start a partition ClaimedChalXXXX instead of doing this search,
        //       it could get costly.
        public ChallengeStatus GetNextVotePendingStatusForChallenge(long ChallengeID)
        {
            ChallengeStatusDb d = (from e in context.CreateQuery<ChallengeStatusDb>(TableName) where e.PartitionKey == DbChalKey(ChallengeID) && e.Status == (int)ChallengeStatus.StatusCodes.ClaimSubmitted select e).FirstOrDefault<ChallengeStatusDb>();
            if (d == null) return null;
            ChallengeStatus s = new ChallengeStatus(d);
            context.Detach(d);
            return s;
        }

        public void MoveStatusesToNewCustomer(long OriginalCustomerID, long NewCustomerID)
        {
            List<ChallengeStatus> statuses = GetActiveChallengesForCustomer(OriginalCustomerID);
            foreach (ChallengeStatus s in statuses)
            {
                s.CustomerID = NewCustomerID;
                Add(s);
            }
        }

        public void ClearAll(long ChallengeID)
        {
            TableServiceContextV2 clearChalContext = new TableServiceContextV2(client.BaseUri.ToString(), client.Credentials);
            TableServiceContextV2 clearCustContext = new TableServiceContextV2(client.BaseUri.ToString(), client.Credentials);

            CloudTableQuery<ChallengeStatusDb> c = (from e in clearChalContext.CreateQuery<ChallengeStatusDb>(TableName) where e.PartitionKey == DbChalKey(ChallengeID) select e).AsTableServiceQuery<ChallengeStatusDb>();

            List<ChallengeStatusDb> custDeletes = new List<ChallengeStatusDb>();

            foreach (ChallengeStatusDb d in c)
            {
                custDeletes.Add(new ChallengeStatusDb { PartitionKey = DbCustKey(d.CustomerID), RowKey = DbChalKey(d.ChallengeID) });
                clearChalContext.DeleteObject(d);
            }
            clearChalContext.SaveChangesWithRetries(SaveChangesOptions.Batch);
            
            foreach (ChallengeStatusDb d in custDeletes)
            {
                clearCustContext.AttachTo(TableName, d);
                clearCustContext.DeleteObject(d);
            }
            clearCustContext.SaveChangesWithRetries(SaveChangesOptions.Batch);

            clearChalContext = null;
            clearCustContext = null;
        }

        public void MoveToLocker(long CustomerID, long ChallengeID)
        {
            throw new NotImplementedException();
        }
    }
}