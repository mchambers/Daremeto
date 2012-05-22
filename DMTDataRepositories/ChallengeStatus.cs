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
            New,
            Accepted,
            TargetRejected,
            ClaimSubmitted,
            NeedMoreEvidence,
            SourceRejected,
            Paid,
            PartialPaid
        }

        public ChallengeStatus()
        {
            this.Customer = null;
        }

        public ChallengeStatus(ChallengeStatusDb d)
        {
            this.UniqueID = d.RowKey;
            this.CustomerID = d.CustomerID;
            this.ChallengeID = d.ChallengeID;
            this.Status = d.Status;
            this.ChallengeOriginatorCustomerID = d.ChallengeOriginatorCustomerID;

            this.Customer = null;
        }

        public string UniqueID { get; set; }
        public long CustomerID { get; set; }
        public int Status { get; set; }
        public long ChallengeID { get; set; }
        public long ChallengeOriginatorCustomerID { get; set; }

        public Customer Customer;
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
            this.ChallengeID = s.ChallengeID;
            this.ChallengeOriginatorCustomerID = s.ChallengeOriginatorCustomerID;
            this.Status = s.Status;
            this.CustomerID = s.CustomerID;
        }

        public long CustomerID { get; set; }
        public int Status { get; set; }
        public long ChallengeID { get; set; }
        public long ChallengeOriginatorCustomerID { get; set; }
    }
}