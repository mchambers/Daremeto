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
        TableServiceContext context;

        private const string TableName = "Evidence";

        public EvidenceRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();
            client.CreateTableIfNotExist(TableName);
            context = client.GetDataServiceContext();
        }
        
        private Evidence DbEvidenceToEvidence(EvidenceDb item)
        {
            Evidence e = new Evidence();

            e.ChallengeStatusID = item.ChallengeStatusID;
            e.MediaURL = item.MediaURL;
            e.Type = item.Type;
            e.UniqueID = item.UniqueID;
            e.Content = item.Content;

            return e;
        }

        public List<Evidence> GetAllForChallengeStatus(string ChallengeStatusUniqueID)
        {
            CloudTableQuery<EvidenceDb> b = (from e in context.CreateQuery<EvidenceDb>(TableName) where e.PartitionKey == ChallengeStatusUniqueID select e).AsTableServiceQuery<EvidenceDb>();
            List<Evidence> items = new List<Evidence>();

            foreach (EvidenceDb item in b)
            {
                items.Add(DbEvidenceToEvidence(item));
            }

            return items;
        }

        public void Add(Evidence e)
        {
            throw new NotImplementedException();
        }
    }
}