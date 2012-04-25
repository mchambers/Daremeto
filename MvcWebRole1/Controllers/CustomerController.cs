using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using DareyaAPI.Models;

namespace DareyaAPI.Controllers
{
    public class CustomerController : ApiController
    {
        private ICustomerRepository Repo;
        private Security Security;

        public CustomerController()
        {
            Repo = new CustomerRepository();
            Security = new Security();
        }

        // GET /api/customer
        public Database.Customer Get()
        {
            return Database.Customer.CreateCustomer(0, "Shouldbe", "You");
        }

        private Customer FilterForAudience(Customer c, Security.Audience Audience)
        {
            Customer filtered = new Customer();

            switch (Audience)
            {
                case Security.Audience.Anybody:
                    filtered.FirstName = c.FirstName;
                    filtered.LastName = c.LastName;
                    break;
                case Security.Audience.Friends:
                    filtered.FirstName = c.FirstName;
                    filtered.LastName = c.LastName;
                    break;
                case Security.Audience.Owner:
                    filtered = c;
                    break;
                case Security.Audience.Users:
                    filtered.FirstName = c.FirstName;
                    filtered.LastName = c.LastName;
                    break;
            }

            return filtered;
        }

        // GET /api/customer/5
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public Customer Get(int id)
        {
            Customer c=Repo.GetWithID(id);
            return FilterForAudience(c, Security.DetermineAudience(c));
        }

        // POST /api/customer/signup
        public void PostSignup(Customer newCustomer)
        {
            if (newCustomer.FirstName.Equals("") ||
                newCustomer.LastName.Equals("") ||
                newCustomer.EmailAddress.Equals(""))
            {
                // invalid
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }

            if(newCustomer.FacebookAccessToken.Equals("") && newCustomer.Password.Equals(""))
            {
                // no authentication details provided for this customer
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }

            try
            {
                Repo.Add(newCustomer);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }

            // queue an email to be sent welcoming the user

            // send back a blank 200; the client should now try to authorize
        }

        // POST /api/customer
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Private)]
        public void Post(string value)
        {
        }

        // PUT /api/customer/5
        public void Put(int id, string value)
        {
        }

        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Private)]
        // DELETE /api/customer/5
        public void Delete(int id)
        {
        }
    }
}
