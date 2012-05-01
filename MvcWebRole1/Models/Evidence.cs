using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace DareyaAPI.Models
{
    public class EvidenceDb : TableServiceEntity
    {
        public EvidenceDb()
        {
        }

        public EvidenceDb(Evidence e)
        {
            this.PartitionKey = "Evi" + e.ChallengeID;
            this.RowKey = e.UniqueID;

            this.Type = e.Type;
            this.MediaURL = e.MediaURL;
            this.UniqueID = e.UniqueID;
            this.ChallengeID = e.ChallengeID;
        }

        public long Type { get; set; }
        public string MediaURL { get; set; }
        public long ChallengeID { get; set; }
        public string UniqueID { get; set; }
    }

    public class Evidence
    {
        public enum Types
        {
            Text,
            Photo,
            Video
        }

        public long ChallengeID { get; set; }
        public string UniqueID { get; set; }
        public long Type { get; set; }
        public string MediaURL { get; set; }
    }
}