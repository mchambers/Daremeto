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
            FriendshipDb f=null;

            try
            {
                f = (from e in context.CreateQuery<FriendshipDb>(TableName) where e.PartitionKey == "Cust" + Customer1ID.ToString() && e.RowKey == "Cust" + Customer2ID.ToString() select e).FirstOrDefault();
            }
            catch (DataServiceQueryException e)
            {
                f = null;
            }

            if (f != null) return true;
            return false;
        }

        public void Add(long Customer1ID, long Customer2ID)
        {
            FriendshipDb f1 = new FriendshipDb();
            FriendshipDb f2 = new FriendshipDb();

            f1.PartitionKey = "Cust" + Customer1ID.ToString();
            f1.RowKey = "Cust" + Customer2ID.ToString();

            f1.CustomerID = Customer1ID;
            f1.FriendCustomerID = Customer2ID;

            f2.PartitionKey = f1.RowKey;
            f2.RowKey = f1.PartitionKey;

            f2.CustomerID = Customer2ID;
            f2.FriendCustomerID = Customer1ID;
            
            try
            {
                context.AddObject(TableName, f1);
                context.SaveChangesWithRetries();
            }
            catch (Exception e)
            {
            }

            try
            {
                context.AddObject(TableName, f2);
                context.SaveChangesWithRetries();
            }
            catch (Exception e)
            {
            }
        }
    }
}