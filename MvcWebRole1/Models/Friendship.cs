using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class Friendship
    {
        public long SourceCustomerID;
        public long TargetCustomerID;
        public int Status;
    }
}