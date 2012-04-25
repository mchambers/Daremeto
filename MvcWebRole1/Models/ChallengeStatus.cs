using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class ChallengeStatus
    {
        public string UniqueID { get; set; }
        public long CustomerID { get; set; }
        public int Status { get; set; }
        public long ChallengeID { get; set; }
    }

    public class ChallengeStatusDb
    {

    }
}