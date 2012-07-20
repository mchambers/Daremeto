using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Data.Services.Client;

namespace DareyaAPI.Models
{
    public class ActivityRepository : IActivityRepository
    {
        CloudStorageAccount storage;
        CloudTableClient client;
        TableServiceContextV2 context;

        private const string TableName = "Activity";

        public ActivityRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();

            client.CreateTableIfNotExist(TableName);

            context = new TableServiceContextV2(client.BaseUri.ToString(), client.Credentials);
            context.IgnoreResourceNotFoundException = true;
        }

        public IEnumerable<Activity> GetActivityForChallenge(long ChallengeID)
        {
            CloudTableQuery<Activity> query = (from e in context.CreateQuery<Activity>(TableName) where e.PartitionKey == "Chal" + ChallengeID select e).AsTableServiceQuery<Activity>();
            return query.AsEnumerable<Activity>();
        }

        public void Add(Activity value)
        {
            if (value.Created == null || value.ChallengeID == 0)
                return;

            if (value.RowKey == null)
                value.RowKey = value.Created.ToString();

            if (value.PartitionKey == null)
                value.PartitionKey = "Chal" + value.ChallengeID;

            context.AttachTo(TableName, value);
            context.UpdateObject(value);
            context.SaveChangesWithRetries();
            context.Detach(value);
        }
    }
}
