using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Stripe;

namespace DareyaAPI.BillingSystem
{
    public class StripeBillingProcessor : IBillingProcessor
    {
        public decimal GetProcessingFeesForAmount(decimal Amount)
        {
            return (Amount * 0.028m) + 0.30m;
        }

        public BillingProcessorResult Charge(string ForeignCustomerID, decimal Amount, string Reason=null)
        {
            StripeClient client = new StripeClient("LB3kUwdhiUlPlNl1UYW52NLn4q88QsFT");

            dynamic resp=client.CreateCharge(Amount, "usd", ForeignCustomerID, Reason);

            BillingProcessorResult result = new BillingProcessorResult();

            result.ForeignTransactionID = resp.id;
            if (resp.Paid)
                result.Result = BillingProcessorResult.BillingProcessorResultCode.Paid;
            else
                result.Result = BillingProcessorResult.BillingProcessorResultCode.Declined;

            return result;
        }
    }
}