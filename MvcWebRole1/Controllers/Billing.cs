using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DareyaAPI.Models;

namespace DareyaAPI.Controllers
{
    public class Billing
    {
        public IBillingProcessor BillingProcessor
        {
            get;
            set;
        }

        // We always need to make this.
        public decimal TheVig()
        {
            return 0.05m;
        }

        public decimal ComputeActualBountyForChallenge(Challenge c)
        {
            // break even vig = 
            decimal contributedBounty;

            /*
             * 
             * total up RealizedBidAmount (this is the ActualBid-FeesPaid)
             * then
             * TotalRealizedBidAmount=(RealizedBidAmount*TheVig())
             * 
             * */

            return 0;
        }
    }
}