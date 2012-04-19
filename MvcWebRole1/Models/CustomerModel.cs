using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public enum CustomerBillingType
    {
        None,
        Stripe,
        CardIO,
        PayPal
    }

    public class Customer
    {
        public long ID { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Address { get; set; }
        public String Address2 { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String ZIPCode { get; set; }
        public CustomerBillingType BillingType { get; set; }
        public String BillingID { get; set; }
    }
}