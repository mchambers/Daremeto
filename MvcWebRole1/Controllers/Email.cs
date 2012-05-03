using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DareyaAPI.Models;

namespace DareyaAPI.Controllers
{
    public class Email
    {
        public static void SendNewChallengeEmail(Customer Source, Customer Target, Challenge Challenge)
        {
        }

        public static void SendChallengeAcceptedEmail(Customer Source, Customer Target, Challenge Challenge)
        {
        }

        public static void SendChallengeRejectedEmail(Customer Target, Challenge Challenge)
        {
        }

        public static void SendChallengeClaimedEmail(Customer Source, Customer Target, Challenge Challenge)
        {
        }

        public static void SendChallengeBackedEmail(Customer Source, Customer Target, Challenge Challenge)
        {
        }

        public static void SendChallengeYouBackedAwardedAssentedEmail(Customer Target, Challenge Challenge)
        {
        }

        public static void SendChallengeYouBackedAwardedDissentedEmail(Customer Target, Challenge Challenge)
        {
        }

        public static void SendChallengeAwardedToYouEmail(Customer Target, Challenge Challenge)
        {
        }
    }
}