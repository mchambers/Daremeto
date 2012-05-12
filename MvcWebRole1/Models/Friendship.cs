using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace DareyaAPI.Models
{
    public class FriendshipDb : TableServiceEntity
    {
        public long CustomerID { get; set; }
        public long FriendCustomerID { get; set; }
        public int Status { get; set; }
    }
}