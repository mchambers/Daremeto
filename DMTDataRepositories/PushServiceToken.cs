using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace DareyaAPI.Models
{
    public enum PushServiceProviders
    {
        UrbanAirshipAppleSandbox,
        UrbanAirshipAppleProduction,
        UrbanAirshipAndroidSandbox,
        UrbanAirshipAndroidProduction,
        MicrosoftPNSSandbox,
        MicrosoftPNSProduction
    }
    
    public class PushServiceToken
    {
        public long CustomerID { get; set; }
        public string Token { get; set; }
        public int Provider { get; set; }

        public PushServiceToken()
        {
        }
    }

    public class PushServiceTokenDb : TableServiceEntity
    {
        public long CustomerID { get; set; }
        public string Token { get; set; }
        public int Provider { get; set; }
        public string UniqueID { get; set; }

        public PushServiceTokenDb()
        {
        }

        public PushServiceTokenDb(PushServiceToken token)
        {
            this.PartitionKey = "Cust" + token.CustomerID;
            this.RowKey = token.Token;

            this.CustomerID = token.CustomerID;
            this.Provider = token.Provider;
            this.Token = token.Token;
        }
    }
}