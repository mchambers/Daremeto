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
    public class PushServiceTokenRepository : IPushServiceTokenRepository
    {
        CloudStorageAccount storage;
        CloudTableClient client;
        TableServiceContext context;

        private const string TableName = "PushServiceToken";

        public PushServiceTokenRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();
            client.CreateTableIfNotExist(TableName);
            context = client.GetDataServiceContext();
        }

        public void Add(PushServiceToken t)
        {
            PushServiceTokenDb d = new PushServiceTokenDb(t);
            context.AddObject(TableName, d);
            context.SaveChangesWithRetries();
        }

        public List<PushServiceToken> TokensForCustomer(long CustomerID)
        {
            CloudTableQuery<PushServiceTokenDb> b = (from e in context.CreateQuery<PushServiceTokenDb>(TableName) where e.PartitionKey == "Cust" + CustomerID.ToString() select e).AsTableServiceQuery<PushServiceTokenDb>();
            List<PushServiceToken> items = new List<PushServiceToken>();

            foreach (PushServiceTokenDb item in b)
            {
                PushServiceToken t = new PushServiceToken();
                t.CustomerID = item.CustomerID;
                t.Provider = item.Provider;
                t.Token = item.Token;
                t.UniqueID = item.UniqueID;

                items.Add(t);
            }

            return items;
        }

        public void Delete(PushServiceToken t)
        {
            throw new NotImplementedException();
        }
    }
}