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
    public class OnboardToken
    {
        public int AccountType { get; set; }
        public string VerificationString { get; set; }
        public string Token { get; set; }
        public string Secret { get; set; }
        public string ForeignUserID { get; set; }
        public long CustomerID { get; set; }
        public long ChallengeID { get; set; }
        public string ChallengeStatusUniqueKey { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string AvatarURL { get; set; }
        public string BillingID { get; set; }
    }

    public class OnboardTokenDb : TableServiceEntity
    {
        public OnboardTokenDb()
        {
        }

        public OnboardTokenDb(OnboardToken t)
        {
            this.PartitionKey = t.VerificationString;
            this.RowKey = "Cust" + t.CustomerID;

            this.AccountType = t.AccountType;
            this.CustomerID = t.CustomerID;
            this.VerificationString = t.VerificationString;
            this.ForeignUserID = t.ForeignUserID;
            this.Token = t.Token;
            this.Secret = t.Secret;
            this.ChallengeID = t.ChallengeID;
            this.ChallengeStatusUniqueKey = t.ChallengeStatusUniqueKey;
            this.EmailAddress = t.EmailAddress;
            this.FirstName = t.FirstName;
            this.LastName = t.LastName;
            this.Password = t.Password;
            this.AvatarURL = t.AvatarURL;
        }

        public int AccountType { get; set; }
        public string VerificationString { get; set; }
        public string Token { get; set; }
        public string Secret { get; set; }
        public string ForeignUserID { get; set; }
        public long CustomerID { get; set; }
        public long ChallengeID { get; set; }
        public string ChallengeStatusUniqueKey { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string AvatarURL { get; set; }
    }
}
