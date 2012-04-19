using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class ChallengeBid
    {
        public long ID { get; set; }
        public long ChallengeID { get; set; }
        public long CustomerID { get; set; }
        public int Amount { get; set; }
    }
}