using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public interface ICustomerRepository
    {
        Customer GetWithID(long ID);
        long Add(Customer c);
        void Update(Customer c);
        Customer GetWithEmailAddress(string EmailAddress);
        Customer GetWithFacebookID(string FBID);
        Customer GetWithForeignUserID(string ID, Customer.ForeignUserTypes type);
        long GetIDForForeignUserID(string ID, Customer.ForeignUserTypes type);
        void AddForeignNetworkForCustomer(long ID, string ForeignID, Customer.ForeignUserTypes type);
        void ClearForeignNetworkLinks(string ForeignID, Customer.ForeignUserTypes type);
        void RemoveForeignNetworkForCustomer(long ID, string ForeignID, Customer.ForeignUserTypes type);
        void Remove(long CustomerID);
    }
}