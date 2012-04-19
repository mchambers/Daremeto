using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public enum EvidenceType
    {
        None,
        Picture,
        Video
    }

    public enum EvidenceStatus
    {
        None
    }

    public class Evidence
    {
        public long ID { get; set; }
        public long ChallengeID { get; set; }
        public long CustomerID { get; set; }
        public EvidenceType Type { get; set; }
        public String MediaURL { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public EvidenceStatus Status { get; set; }
    }
}