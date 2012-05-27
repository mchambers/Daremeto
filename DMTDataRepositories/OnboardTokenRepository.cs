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
    class OnboardTokenRepository : IOnboardTokenRepository
    {
        CloudStorageAccount storage;
        CloudTableClient client;
        TableServiceContextV2 context;

        private const string TableName = "OnboardToken";

        public OnboardTokenRepository()
        {
            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();
            client.CreateTableIfNotExist(TableName);

            context = new TableServiceContextV2(client.BaseUri.ToString(), client.Credentials);
        }

        public OnboardToken Get(string VerificationString)
        {
            OnboardTokenDb d=(from e in context.CreateQuery<OnboardTokenDb>(TableName) where e.PartitionKey==VerificationString select e).FirstOrDefault();

            OnboardToken t = new OnboardToken();
            t.AccountType = d.AccountType;
            t.ChallengeID = d.ChallengeID;
            t.ChallengeStatusUniqueKey = d.ChallengeStatusUniqueKey;
            t.CustomerID = d.CustomerID;
            t.EmailAddress = d.EmailAddress;
            t.Token = d.Token;
            t.Secret = d.Secret;
            t.VerificationString = d.VerificationString;
            t.FirstName = d.FirstName;
            t.LastName = d.LastName;
            t.Password = d.Password;
            t.AvatarURL = d.AvatarURL;
            t.ForeignUserID = d.ForeignUserID;
            
            return t;
        }

        public void Add(OnboardToken token)
        {
            OnboardTokenDb d = new OnboardTokenDb(token);
            context.AttachTo(TableName, d, null);
            context.UpdateObject(d);
            context.SaveChangesWithRetries();
        }

        public void Remove(OnboardToken token)
        {
            OnboardTokenDb d = new OnboardTokenDb(token);
            context.AttachTo(TableName, d);
            context.DeleteObject(d);
            context.SaveChangesWithRetries();
        }

        public void Update(OnboardToken token)
        {
            OnboardTokenDb d = new OnboardTokenDb(token);
            context.AttachTo(TableName, d, null);
            context.UpdateObject(d);
            context.SaveChangesWithRetries();
        }
    }
}
