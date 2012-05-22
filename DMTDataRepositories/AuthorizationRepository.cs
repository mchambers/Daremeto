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

        private AuthorizationDb AuthorizationToDbAuthorization(Authorization a)
        {
            AuthorizationDb d = new AuthorizationDb();

            d.CustomerID = a.CustomerID;
            d.EmailAddress = a.EmailAddress;
            d.PartitionKey = a.Token;
            d.RowKey = a.UniqueID;
            d.Valid = a.Valid;

            return d;
        }

        private Authorization DbAuthorizationToAuthorization(AuthorizationDb d)
        {
            Authorization a = new Authorization();

            a.UniqueID = d.RowKey;
            a.Token = d.PartitionKey;
            a.EmailAddress = d.EmailAddress;
            a.CustomerID = d.CustomerID;
            a.Valid = d.Valid;

            return a;
        }

        public AuthorizationRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();
            client.CreateTableIfNotExist("Authorization");
            context = client.GetDataServiceContext();
        }

        public Authorization GetWithToken(string Token)
        {
            AuthorizationDb a = (from e in context.CreateQuery<AuthorizationDb>("Authorization") where e.PartitionKey == Token select e).FirstOrDefault();
            return DbAuthorizationToAuthorization(a);
        }

        public void Add(Authorization a)
        {
            a.Valid = true;
            context.AddObject("Authorization", AuthorizationToDbAuthorization(a));
            context.SaveChangesWithRetries();
        }

        public void Remove(string Token)
        {
            throw new NotImplementedException();
        }
    }
}