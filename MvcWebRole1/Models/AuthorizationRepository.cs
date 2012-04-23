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
    public class AuthorizationRepository : IAuthorizationRepository
    {
        CloudStorageAccount storage;
        CloudTableClient client;
        TableServiceContext context;

        public AuthorizationRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();
            client.CreateTableIfNotExist("Authorization");
            context = client.GetDataServiceContext();
        }

        public Authorization GetWithToken(string Token)
        {
            Authorization a = (from e in context.CreateQuery<Authorization>("Authorization") where e.PartitionKey == Token select e).FirstOrDefault();
            return a;
        }

        public void Add(Authorization a)
        {
            a.Valid = true;
            context.AddObject("Authorization", a);
            context.SaveChangesWithRetries();
        }

        public void Remove(string Token)
        {
            throw new NotImplementedException();
        }
    }
}