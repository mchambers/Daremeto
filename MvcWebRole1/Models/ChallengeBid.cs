using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace DareyaAPI.Models
{
    public class ChallengeBid
    {
        public enum BidStatusCodes
        {
            Default,
            BidderAccepts,
            BidderRejects
        }

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
        public long ID { get; set; }
        public int Status { get; set; }
    }

    public class ChallengeBidDb : TableServiceEntity
    {
        public ChallengeBidDb()
        {
        }

        public ChallengeBidDb(ChallengeBid bid)
        {
            //this.PartitionKey = bid.ChallengeID.ToString();

            if (bid.UniqueID.Equals(""))
                this.RowKey = System.Guid.NewGuid().ToString();
            else
                this.RowKey = bid.UniqueID;

            this.Active = bid.Active;
            this.Amount = bid.Amount;
            this.ChallengeID = bid.ChallengeID;
            this.CustomerID = bid.CustomerID;
        }

        public ChallengeBidDb(long ChallengeID)
        {
            this.PartitionKey = ChallengeID.ToString();
            this.RowKey = System.Guid.NewGuid().ToString();
        }

        public int Amount { get; set; }
        public long ChallengeID { get; set; }
        public long CustomerID { get; set; }
        public bool Active { get; set; }
    }
}