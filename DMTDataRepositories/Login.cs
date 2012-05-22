using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class Login
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string FacebookToken { get; set; }
        public string FacebookID { get; set; }
    }
}