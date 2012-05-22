using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace DareyaAPI.Models
{
    public class ChallengeStatusVote
    {
        public long ChallengeID { get; set; }
        public string ChallengeStatusUniqueID { get; set; }
        public bool Accepted { get; set; }
        public string UniqueID { get; set; }
    }

    public class ChallengeStatusVoteDb : TableServiceEntity
    {
        public ChallengeStatusVoteDb()
        {

        }

        public long ChallengeID { get; set; }
        public string ChallengeStatusUniqueID { get; set; }
        public bool Accepted { get; set; }
        public string UniqueID { get; set; }
    }
}