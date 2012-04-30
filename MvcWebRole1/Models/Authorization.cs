using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace DareyaAPI.Models
{
    public class AuthorizationDb : TableServiceEntity
    {
        public AuthorizationDb()
        {
        }

        public AuthorizationDb(Authorization a)
        {
            this.PartitionKey = a.Token;
            this.RowKey = a.UniqueID;
        }

        public long CustomerID { get; set; }
        public bool Valid { get; set; }
        public string EmailAddress { get; set; }
    }

    public class Authorization
    {
        public Authorization()
        {
        }

        public Authorization(String Token)
        {
            this.Token = Token;
            this.UniqueID = System.DateTime.Now.ToString().Replace("/", "").Replace(":", "");
        }

        public String Token { get; set; }
        public String UniqueID { get; set; }
        public long CustomerID { get; set; }
        public bool Valid { get; set; }
        public string EmailAddress { get; set; }
    }
}