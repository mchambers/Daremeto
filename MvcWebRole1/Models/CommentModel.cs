using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class Comment
    {
        public long ID { get; set; }
        public long CustomerID { get; set; }
        public long ChallengeID { get; set; }
        public String Text { get; set; }
        public DateTime Created { get; set; }
    }
}