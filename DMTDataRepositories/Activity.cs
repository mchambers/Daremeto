using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;

namespace DareyaAPI.Models
{
    public class Activity : TableServiceEntity
    {
        public enum ActivityType
        {
            ActivityCreateDare,
            ActivityTakeDare,
            ActivityAddEvidence,
            ActivityComment,
            ActivityLikeDare,
            ActivityBackDare,
            ActivityVoteYes,
            ActivityVoteNo,
            ActivityComplete
        }

        public Activity()
        {
        }

        public Activity(long ChallengeID, DateTime Created)
        {
            this.PartitionKey = "Chal" + ChallengeID.ToString();
            this.RowKey = Created.ToString();

            this.ChallengeID = ChallengeID;
            this.Created = Created;
        }

        public int Type { get; set; }
        public string Content { get; set; }
        public long CustomerID { get; set; }
        public long ChallengeID { get; set; }
        public DateTime Created { get; set; }
    }
}
