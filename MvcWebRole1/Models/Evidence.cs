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
            this.PartitionKey = e.ChallengeStatusID;
            this.RowKey = e.UniqueID;

            this.Type = e.Type;
            this.MediaURL = e.MediaURL;
            this.UniqueID = e.UniqueID;
            this.Content = e.Content;
        }

        public long Type { get; set; }
        public string MediaURL { get; set; }
        public string UniqueID { get; set; }
        public string ChallengeStatusID { get; set; }
        public string Content { get; set; }
    }

    public class Evidence
    {
        public enum Types
        {
            Text,
            Photo,
            Video
        }

        public string ChallengeStatusID { get; set; }
        public string UniqueID { get; set; }
        public long Type { get; set; }
        public string MediaURL { get; set; }
        public string Content { get; set; }
    }
}