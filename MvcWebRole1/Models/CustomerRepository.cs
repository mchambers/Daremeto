﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class CustomerRepository : ICustomerRepository
    {
        Database.DYDbEntities db;

        public CustomerRepository()
        {
            db = new Database.DYDbEntities();
        }

        private Customer DbCustomerToCustomer(Database.Customer dbCust)
        {
            if (dbCust == null)
                return null;

            Customer c = new Customer();

            c.ID = dbCust.ID;
            c.FirstName = dbCust.FirstName;
            c.LastName = dbCust.LastName;
            c.Address = dbCust.Address1;
            c.Address2 = dbCust.Address2;
            c.City = dbCust.City;
            c.State = dbCust.State;
            c.ZIPCode = dbCust.ZIPCode;
            c.FacebookAccessToken = dbCust.FacebookAccessToken;
            c.FacebookExpires = dbCust.FacebookExpires;
            c.BillingID = dbCust.BillingID;
            c.BillingType = (int)dbCust.BillingType;

            return c;
        }

        private Database.Customer CustomerToDbCustomer(Customer cust)
        {
            if (cust == null)
                return null;

            Database.Customer dbCust = Database.Customer.CreateCustomer(cust.ID, cust.FirstName, cust.LastName);

            dbCust.Address1 = cust.Address;
            dbCust.Address2 = cust.Address2;
            dbCust.City = cust.City;
            dbCust.State = cust.State;
            dbCust.ZIPCode = cust.ZIPCode;
            dbCust.FacebookAccessToken = cust.FacebookAccessToken;
            dbCust.FacebookExpires = cust.FacebookExpires;
            dbCust.FacebookUserID = cust.FacebookUserID;
            dbCust.BillingID = cust.BillingID;
            dbCust.BillingType = cust.BillingType;

            return dbCust;
        }

        public Customer GetWithEmailAddress(string EmailAddress)
        {
            Database.Customer c=db.Customer.FirstOrDefault(cust => cust.EmailAddress.ToLower().Equals(EmailAddress));
            return DbCustomerToCustomer(c);
        }

        public Customer GetWithID(long ID)
        {
            return DbCustomerToCustomer(db.Customer.FirstOrDefault(cust => cust.ID == ID));
        }

        public void Add(Customer c)
        {
            c.EmailAddress = c.EmailAddress.Trim().ToLower(); // store email addresses trimmed and in lowercase
            db.Customer.AddObject(CustomerToDbCustomer(c));
            db.SaveChanges();
        }

        public void Update(Customer c)
        {
            db.SaveChanges();
        }
    }
}