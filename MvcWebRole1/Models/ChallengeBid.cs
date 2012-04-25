using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace DareyaAPI.Models
{
    public class ChallengeBid
    {
        public ChallengeBid()
        {
        }

        public ChallengeBid(ChallengeBidDb dbItem)
        {
            this.ChallengeID = dbItem.ChallengeID;
            this.CustomerID = dbItem.CustomerID;
            this.Active = dbItem.Active;
            this.Amount = dbItem.Amount;
            this.UniqueID = dbItem.RowKey;
        }

        public long ChallengeID { get; set; }
        public long CustomerID { get; set; }
        public bool Active { get; set; }
        public int Amount { get; set; }
        public string UniqueID { get; set; }
    }

    public class ChallengeBidDb : TableServiceEntity
    {
        public ChallengeBidDb()
        {
        }

        public ChallengeBidDb(ChallengeBid bid)
        {
            this.PartitionKey = "CBids" + bid.ChallengeID.ToString();

            if (bid.UniqueID.Equals(""))
                this.RowKey = System.DateTime.Now.ToString().Replace("/", "").Replace(":", "");
            else
                this.RowKey = bid.UniqueID;

            this.Active = bid.Active;
            this.Amount = bid.Amount;
            this.ChallengeID = bid.ChallengeID;
            this.CustomerID = bid.CustomerID;
        }

        public ChallengeBidDb(long ChallengeID)
        {
            this.PartitionKey = "CBids"+ChallengeID.ToString();
            this.RowKey = System.DateTime.Now.ToString().Replace("/", "").Replace(":", "");
        }

        public int Amount { get; set; }
        public long ChallengeID { get; set; }
        public long CustomerID { get; set; }
        public bool Active { get; set; }
    }
}