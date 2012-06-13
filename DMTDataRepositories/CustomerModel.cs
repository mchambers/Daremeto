﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class SignupCustomer : Customer
    {
        public long ChallengeID { get; set; }
    }

    public class MiniCustomer
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarURL { get; set; }
    }

    public class Customer
    {
        public enum TypeCodes
        {
            Default,
            Unclaimed,
            Unverified
        }

        public enum ForeignUserTypes
        {
            Undefined,
            Facebook,
            Twitter,
            PhoneNumber,
            EmailAddress
        }

        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarURL { get; set; }

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

        public int Type { get; set; }

        public string ForeignUserID { get; set; }
        public int ForeignUserType { get; set; }
        
        public static Customer Filter(Customer c)
        {
            Customer filtered = new Customer();

            filtered.ID = c.ID;
            filtered.FirstName = c.FirstName;
            filtered.LastName = c.LastName;
            filtered.AvatarURL = c.AvatarURL;

            return filtered;
        }

        public Customer()
        {
            FirstName = "";
            LastName = "";
            EmailAddress = "";
            Address = "";
            Address2 = "";
            City = "";
            State = "";
            ZIPCode = "";
            FacebookAccessToken = "";
            FacebookExpires = "";
            FacebookUserID = "";
            Password = "";
            BillingType = 0;
            BillingID = "";
            Type = 0;
            AvatarURL = "";
            ForeignUserType = 0;
            ForeignUserID = "";
        }
    }
}