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
    public class EvidenceRepository : IEvidenceRepository
    {
        CloudStorageAccount storage;
        CloudTableClient client;
        TableServiceContextV2 context;

        private const string TableName = "Evidence";

        public EvidenceRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();
            client.CreateTableIfNotExist(TableName);
            //context = client.GetDataServiceContext();
            context = new TableServiceContextV2(client.BaseUri.ToString(), client.Credentials);
        }
        
        private string DbPartKey(long ChallengeID, long CustomerID)
        {
            return "Chal" + ChallengeID.ToString() + "Cust" + CustomerID.ToString();
        }

        private Evidence DbEvidenceToEvidence(EvidenceDb item)
        {
            Evidence e = new Evidence();

            e.ChallengeID = item.ChallengeID;
            e.CustomerID = item.CustomerID;
            e.MediaURL = item.MediaURL;
            e.Type = item.Type;
            e.UniqueID = item.UniqueID;
            e.Content = item.Content;

            return e;
        }

        private EvidenceDb EvidenceToDbEvidence(Evidence item)
        {
            EvidenceDb d = new EvidenceDb();

            d.CustomerID = item.CustomerID;
            d.Content = item.Content;
            d.MediaURL = item.MediaURL;
            d.UniqueID = item.UniqueID;
            d.Type = item.Type;
            d.ChallengeID = item.ChallengeID;

            return d;
        }

        public List<Evidence> GetAllForChallengeStatus(ChallengeStatus status)
        {
            CloudTableQuery<EvidenceDb> b = (from e in context.CreateQuery<EvidenceDb>(TableName) where e.PartitionKey == DbPartKey(status.ChallengeID, status.CustomerID) select e).AsTableServiceQuery<EvidenceDb>();
            List<Evidence> items = new List<Evidence>();

            foreach (EvidenceDb item in b)
            {
                items.Add(DbEvidenceToEvidence(item));
                context.Detach(item);
            }

            return items;
        }

        public void Add(Evidence e)
        {
            EvidenceDb d = EvidenceToDbEvidence(e);

            d.PartitionKey = DbPartKey(e.ChallengeID, e.CustomerID);
            d.RowKey = e.UniqueID;

            context.AttachTo(TableName, d, null);
            context.UpdateObject(d);
            context.SaveChangesWithRetries();
            context.Detach(d);
        }
    }
}