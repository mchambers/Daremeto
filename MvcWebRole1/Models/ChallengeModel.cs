using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public enum ChallengePrivacy
    {
        None,
        JustOnePerson,
        Friends,
        Location,
        Everybody
    }

    public enum ChallengeType
    {
        None,
        Standard
    }

    public class Challenge
    {
        public long ID { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public ChallengePrivacy Privacy { get; set; }
        public ChallengeType Type { get; set; }
        public long CustomerID { get; set; }
        public DateTime Created { get; set; }
    }
}