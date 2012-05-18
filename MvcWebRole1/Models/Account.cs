using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class Account
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public long CustomerID { get; set; }
    }
}