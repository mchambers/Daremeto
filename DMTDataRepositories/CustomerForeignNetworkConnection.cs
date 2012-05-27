using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;

namespace DareyaAPI.Models
{
    public class CustomerForeignNetworkConnection
    {
        public int Type { get; set; }
        public string UserID { get; set; }
        public string AccessToken { get; set; }
        public string AccessSecret { get; set; }
        public long CustomerID { get; set; }
    }

    public class CustomerForeignNetworkConnectionDb : TableServiceEntity
    {
        public CustomerForeignNetworkConnectionDb()
        {   
        }

        public CustomerForeignNetworkConnectionDb(CustomerForeignNetworkConnection c, bool PartitionWithForeignKey)
        {
            if (PartitionWithForeignKey) // search by Type+ID
            {
                this.PartitionKey = c.Type.ToString() + "+" + c.UserID;
                this.RowKey = "Cust"+c.CustomerID.ToString();
            }
            else // search by CustomerID
            {
                this.PartitionKey = "Cust" + c.CustomerID.ToString();
                this.RowKey = c.Type.ToString() + "+" + c.UserID;
            }
            
            this.Type = c.Type;
            this.UserID = c.UserID;
            this.AccessToken = c.AccessToken;
            this.AccessSecret = c.AccessSecret;
            this.CustomerID = c.CustomerID;
        }

        public int Type { get; set; }
        public string UserID { get; set; }
        public string AccessToken { get; set; }
        public string AccessSecret { get; set; }
        public long CustomerID { get; set; }
    }
}
