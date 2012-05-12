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
    public class FriendshipRepository : IFriendshipRepository
    {
        CloudStorageAccount storage;
        CloudTableClient client;
        TableServiceContext context;

        private const string TableName = "Friendship";

        public FriendshipRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();
            client.CreateTableIfNotExist(TableName);
            context = client.GetDataServiceContext();
        }

        public bool CustomersAreFriends(long Customer1ID, long Customer2ID)
        {
            FriendshipDb f = (from e in context.CreateQuery<FriendshipDb>(TableName) where e.PartitionKey == "Cust" + Customer1ID.ToString() && e.RowKey == "Cust" + Customer2ID.ToString() select e).FirstOrDefault();
            if (f != null) return true;
            return false;
        }

        public void Add(long Customer1ID, long Customer2ID)
        {
            FriendshipDb f1 = new FriendshipDb();
            FriendshipDb f2 = new FriendshipDb();

            f1.PartitionKey = "Cust" + Customer1ID.ToString();
            f1.RowKey = "Cust" + Customer2ID.ToString();
            
            f2.PartitionKey = f1.RowKey;
            f2.RowKey = f1.PartitionKey;

            context.AddObject(TableName, f1);
            context.SaveChangesWithRetries();

            context.AddObject(TableName, f2);
            context.SaveChangesWithRetries();
        }
    }
}