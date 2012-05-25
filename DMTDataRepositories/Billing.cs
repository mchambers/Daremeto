using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DareyaAPI.Models;

namespace DareyaAPI.BillingSystem
{
    public class Billing
    {
        public static decimal ComputeVigForAmount(decimal Amount)
        {
            return Amount * 0.05m;
        }
    }
}