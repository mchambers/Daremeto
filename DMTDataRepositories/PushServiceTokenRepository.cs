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
        TableServiceContextV2 context;

        private const string TableName = "PushServiceToken";

        public PushServiceTokenRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();
            client.CreateTableIfNotExist(TableName);
            context = new TableServiceContextV2(client.BaseUri.ToString(), client.Credentials);
        }

        public void Add(PushServiceToken t)
        {
            PushServiceTokenDb d = new PushServiceTokenDb(t);
            context.AttachTo(TableName, d, null);
            context.UpdateObject(d);
            context.SaveChangesWithRetries();
            context.Detach(d);
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
                context.Detach(item);
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