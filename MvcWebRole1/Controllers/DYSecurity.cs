using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DareyaAPI.Models;

namespace DareyaAPI.Controllers
{
    public class Security
    {
        public enum Audience
        {
            Anybody,
            Users,
            Friends,
            Owner
        }

        /* DetermineAudience reports the audience the current user falls into with regards to the specified content.
         * 
         *      Example: 
         *          if they aren't logged in at all, return Anybody
         *          if the content is owned by a friend of theirs, return Friends
         *          if the content is owned by them, return Owner
         *          if they are logged in but the content doesn't fit any of these groups, return Users
        */
        public Audience DetermineAudience(Customer c)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated && c.ID == ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                return Audience.Owner;

            return Audience.Anybody;
        }

        public Audience DetermineAudience(Challenge c)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated && c.CustomerID == ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                return Audience.Owner;

            return Audience.Anybody;
        }

        public Authorization AuthorizeCustomer(Login l)
        {
            ICustomerRepository repo = new CustomerRepository();

            Customer c = repo.GetWithEmailAddress(l.EmailAddress);
            if (c == null)
                return null;

            if (!l.Password.Equals(c.Password))
                return null;

            Authorization a = new Authorization("test" + System.DateTime.Now.Ticks.ToString());
            a.CustomerID = c.ID;
            a.EmailAddress = l.EmailAddress;

            IAuthorizationRepository authRepo = new AuthorizationRepository();
            authRepo.Add(a); // store the auth token in the repo

            return a;
        }
    }
}