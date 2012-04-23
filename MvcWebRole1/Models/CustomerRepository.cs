using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class CustomerRepository : ICustomerRepository
    {
        daremetoEntities db;

        public CustomerRepository()
        {
            db = new daremetoEntities();
        }

        public Customer GetWithEmailAddress(string EmailAddress)
        {
            return db.Customer.FirstOrDefault(cust => cust.EmailAddress.ToLower().Equals(EmailAddress));
        }

        public Customer GetWithID(long ID)
        {
            return db.Customer.FirstOrDefault(cust => cust.ID == ID);
        }

        public void Add(Customer c)
        {
            c.EmailAddress = c.EmailAddress.Trim().ToLower(); // store email addresses trimmed and in lowercase
            db.Customer.AddObject(c);
        }

        public void Update(Customer c)
        {
            db.SaveChanges();
        }
    }
}