using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace DareyaAPI.Models
{
    public class DareyaIdentity : GenericIdentity
    {
        public DareyaIdentity(string EmailAddress, long CustomerID) : base(EmailAddress)
        {
            this.CustomerID = CustomerID;
        }

        public long CustomerID { get; set; }
    }
}