using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class CustomerRepository : ICustomerRepository
    {
        DMTDataRepositories.Database.DYDbEntities db;
         
        public CustomerRepository()
        {
            db = new DMTDataRepositories.Database.DYDbEntities();
        }

        private Customer DbCustomerToCustomer(DMTDataRepositories.Database.Customer dbCust)
        {
            if (dbCust == null)
                return null;

            Customer c = new Customer();

            c.ID = dbCust.ID;
            c.FirstName = dbCust.FirstName;
            c.LastName = dbCust.LastName;
            c.EmailAddress = dbCust.EmailAddress;
            c.Address = dbCust.Address1;
            c.Address2 = dbCust.Address2;
            c.City = dbCust.City;
            c.State = dbCust.State;
            c.ZIPCode = dbCust.ZIPCode;
            c.FacebookUserID = dbCust.FacebookUserID;
            c.FacebookAccessToken = dbCust.FacebookAccessToken;
            c.FacebookExpires = dbCust.FacebookExpires;
            c.BillingID = dbCust.BillingID;
            c.Password = dbCust.Password;
            c.AvatarURL = dbCust.AvatarURL;

            if(dbCust.Type!=null) c.Type = (int)dbCust.Type;

            if(dbCust.BillingType!=null)
                c.BillingType = (int)dbCust.BillingType;

            return c;
        }

        private DMTDataRepositories.Database.Customer CustomerToDbCustomer(Customer cust)
        {
            if (cust == null)
                return null;

            DMTDataRepositories.Database.Customer dbCust = new DMTDataRepositories.Database.Customer(); // Database.Customer.CreateCustomer(cust.ID, cust.FirstName, cust.LastName);

            if (cust.FirstName == null) cust.FirstName = "";
            if (cust.LastName == null) cust.LastName = "";
            if (cust.EmailAddress == null) cust.EmailAddress = "";
            if (cust.Password == null) cust.Password = "";
            if (cust.Address == null) cust.Address = "";
            if (cust.Address2 == null) cust.Address2 = "";
            if (cust.City == null) cust.City = "";
            if (cust.State == null) cust.State = "";
            if (cust.ZIPCode == null) cust.ZIPCode = "";
            if (cust.FacebookAccessToken == null) cust.FacebookAccessToken = "";
            if (cust.FacebookExpires == null) cust.FacebookExpires = "";
            if (cust.FacebookUserID == null) cust.FacebookUserID = "";
            if (cust.BillingID == null) cust.BillingID = "";
            if (cust.BillingType == null) cust.BillingType = 0;
            if (cust.AvatarURL == null) cust.AvatarURL = "";

            dbCust.FirstName = cust.FirstName;
            dbCust.LastName = cust.LastName;
            dbCust.EmailAddress = cust.EmailAddress;
            dbCust.Password = cust.Password;
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
            dbCust.AvatarURL = cust.AvatarURL;
            dbCust.Type = cust.Type;

            return dbCust;
        }

        public Customer GetWithEmailAddress(string EmailAddress)
        {
            DMTDataRepositories.Database.Customer c = db.Customer.FirstOrDefault(cust => cust.EmailAddress.ToLower().Equals(EmailAddress));
            return DbCustomerToCustomer(c);
        }

        public Customer GetWithID(long ID)
        {
            return DbCustomerToCustomer(db.Customer.FirstOrDefault(cust => cust.ID == ID));
        }

        public long Add(Customer c)
        {
            c.EmailAddress = c.EmailAddress.Trim().ToLower(); // store email addresses trimmed and in lowercase
            DMTDataRepositories.Database.Customer dbc = CustomerToDbCustomer(c);
            db.Customer.AddObject(dbc);
            db.SaveChanges();
            db.Refresh(System.Data.Objects.RefreshMode.StoreWins, dbc);
            return dbc.ID;
        }

        public void Update(Customer c)
        {
            DMTDataRepositories.Database.Customer dbc = CustomerToDbCustomer(c);
            db.Customer.Attach(dbc);
            db.SaveChanges();
        }

        public Customer GetWithFacebookID(string FBID)
        {
            return DbCustomerToCustomer(db.Customer.FirstOrDefault(cust => cust.FacebookUserID.Equals(FBID)));
        }


        public Customer GetWithForeignUserID(string ID, Customer.ForeignUserTypes type)
        {
            throw new NotImplementedException();
        }
    }
}