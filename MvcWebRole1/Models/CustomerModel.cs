using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class Customer
    {
        public long ID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZIPCode { get; set; }

        public string FacebookAccessToken { get; set; }
        public string FacebookExpires { get; set; }
        public string FacebookUserID { get; set; }

        public string Password { get; set; }

        public int BillingType { get; set; }
        public string BillingID { get; set; }
    }
}