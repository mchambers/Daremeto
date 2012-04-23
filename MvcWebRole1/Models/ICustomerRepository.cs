using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public interface ICustomerRepository
    {
        Customer GetWithID(long ID);
        void Add(Customer c);
        void Update(Customer c);
        Customer GetWithEmailAddress(string EmailAddress);
    }
}