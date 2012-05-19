using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.BillingSystem
{
    public class StripeBillingProcessor : IBillingProcessor
    {
        public decimal GetProcessingFeesForAmount(decimal Amount)
        {
            return (Amount * 0.028m) + 0.30m;
        }

        public BillingProcessorResult Charge(string ForeignCustomerID, decimal Amount)
        {
            throw new NotImplementedException();
        }
    }
}