using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace DareyaAPI.Models
{
    public class Authorization : TableServiceEntity
    {
        public Authorization()
        {
        }

        public Authorization(String Token)
        {
            this.PartitionKey = Token;
            this.RowKey = System.DateTime.Now.ToString().Replace("/", "").Replace(":", "");
        }

        public long CustomerID { get; set; }
        public bool Valid { get; set; }
        public string EmailAddress { get; set; }
    }
}