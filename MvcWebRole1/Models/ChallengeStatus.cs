using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace DareyaAPI.Models
{
    public class ChallengeStatus
    {
        public enum StatusCodes
        {
            None,
            Accepted,
            TargetRejected,
            Submitted,
            NeedMoreEvidence,
            SourceRejected
        }

        public ChallengeStatus()
        {
        }

        public ChallengeStatus(ChallengeStatusDb d)
        {
            this.UniqueID = d.RowKey;
            this.CustomerID = d.CustomerID;
            this.ChallengeID = d.ChallengeID;
            this.Status = d.Status;
        }

        public string UniqueID { get; set; }
        public long CustomerID { get; set; }
        public int Status { get; set; }
        public long ChallengeID { get; set; }
        public long ChallengeOriginatorCustomerID { get; set; }
    }

    public class ChallengeStatusDb : TableServiceEntity
    {
        public ChallengeStatusDb()
        {
        }

        public ChallengeStatusDb(ChallengeStatus s)
        {
            //this.PartitionKey = "Chal" + s.ChallengeID;
            
            if (s.UniqueID == null || s.UniqueID.Equals(""))
                s.UniqueID = System.Guid.NewGuid().ToString();

            this.RowKey = s.UniqueID;
        }

        public long CustomerID { get; set; }
        public int Status { get; set; }
        public long ChallengeID { get; set; }
        public long ChallengeOriginatorCustomerID { get; set; }
    }
}