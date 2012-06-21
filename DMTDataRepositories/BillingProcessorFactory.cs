using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.BillingSystem
{
    public class BillingProcessorFactory
    {
        private static Dictionary<SupportedBillingProcessor, IBillingProcessor> _processors;

        public enum SupportedBillingProcessor
        {
            None,
            Stripe,
            Amazon,
            PayPalMerchant
        }

        public static IBillingProcessor GetBillingProcessor(SupportedBillingProcessor processor)
        {
            if(_processors==null)
                _processors=new Dictionary<SupportedBillingProcessor,IBillingProcessor>();

            if (!_processors.ContainsKey(processor))
            {
                switch (processor)
                {
                    case SupportedBillingProcessor.Stripe:
                        _processors.Add(processor, new StripeBillingProcessor());
                        break;
                    default:
                        throw new Exception("No billing processor exists for the specified type.");
                }
            }

            return _processors[processor];
        }
    }
}