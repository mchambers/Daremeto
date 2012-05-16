using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Controllers
{
    public class BillingProcessorFactory
    {
        public enum SupportedBillingProcessor
        {
            InHouse,
            Stripe,
            Amazon,
            PayPalMerchant
        }

        public static IBillingProcessor GetBillingProcessor(SupportedBillingProcessor processor)
        {
            return null;
        }
    }
}