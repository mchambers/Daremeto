using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using DareyaAPI.Models;
using System.Web;

namespace DareyaAPI.Controllers
{
    public class CustomerController : ApiController
    {
        private ICustomerRepository Repo;
        private Security Security;
        private IChallengeStatusRepository StatusRepo;
        private IChallengeRepository ChalRepo;
        private IPushServiceTokenRepository TokenRepo;

        private enum SignupType
        {
            Credentials,
            Facebook
        }

        public CustomerController()
        {
            Repo = RepoFactory.GetCustomerRepo();
            Security = new Security();
            StatusRepo = RepoFactory.GetChallengeStatusRepo();
            ChalRepo = RepoFactory.GetChallengeRepo();
            TokenRepo = RepoFactory.GetPushServiceTokenRepo();
        }

        // GET /api/customer
        /*public Database.Customer Get()
        {
            //return Database.Customer.CreateCustomer(0, "Shouldbe", "You");
        }*/

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
        [HttpGet]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public Customer Get(int id)
        {
            Customer c=Repo.GetWithID(id);
            return FilterForAudience(c, Security.DetermineAudience(c));
        }

        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void PushServiceToken(PushServiceToken t)
        {
            t.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            TokenRepo.Add(t);
        }

        private void CoreHandleFacebookSignup(Customer newCustomer)
        {
            Customer tryFB = Repo.GetWithFacebookID(newCustomer.FacebookUserID);

            // if we have an unclaimed FB user, claim them now
            // rather than making a new account.
            if (tryFB != null && tryFB.FacebookUserID == newCustomer.FacebookUserID)
            {
                tryFB.Type = (int)Customer.TypeCodes.Default;
                tryFB.FacebookAccessToken = newCustomer.FacebookAccessToken;
                tryFB.FacebookExpires = newCustomer.FacebookExpires;

                Repo.Update(tryFB);
            }
            else
            {
                Repo.Add(newCustomer);
            }

        }

        private void CoreCreateSendVerificationEmail(Customer newCustomer)
        {
            AuthorizationRepository authRepo = new AuthorizationRepository();
            
            Authorization a = new Authorization("verify-" + Guid.NewGuid().ToString());
            a.Valid = false;
            a.EmailAddress = newCustomer.EmailAddress;
            a.CustomerID = newCustomer.ID;

            authRepo.Add(a);

            String authUrl = "http://dareme.to/verify/" + a.Token;
        }

        private void CoreHandleCredentialSignup(Customer newCustomer)
        {
            newCustomer.EmailAddress = newCustomer.EmailAddress.ToLower();

            Customer tryEmail = Repo.GetWithEmailAddress(newCustomer.EmailAddress);
            if (tryEmail != null && tryEmail.EmailAddress.Equals(newCustomer.EmailAddress))
            {
                if (tryEmail.Type != (int)Customer.TypeCodes.Unclaimed)
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
                }
            }

            newCustomer.Type = (int)Customer.TypeCodes.Unverified;
            
            try
            {
                CoreCreateSendVerificationEmail(newCustomer);
                if (tryEmail == null)
                    Repo.Add(newCustomer);
                else
                    Repo.Update(tryEmail);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(e.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public void Verify(Authorization verify)
        {
            
        }

        // POST /api/customer/signup
        [HttpPost]
        public void Signup(Customer newCustomer)
        {
            SignupType signupType = SignupType.Credentials;

            if (newCustomer.AvatarURL == null || newCustomer.AvatarURL.Equals(""))
            {
                newCustomer.AvatarURL = "http://images.dareme.to/avatars/default.jpg";
            }

            if (newCustomer.FacebookUserID == null || newCustomer.FacebookUserID.Equals(""))
            {
                if (newCustomer.EmailAddress==null || newCustomer.EmailAddress.Equals(""))
                {
                    // invalid
                    throw new HttpResponseException("No Facebook Connect information was found and no email address specified.", System.Net.HttpStatusCode.InternalServerError);
                }
                else
                    signupType = SignupType.Credentials;
            }
            else
                signupType = SignupType.Facebook;

            if((newCustomer.FacebookAccessToken==null || newCustomer.FacebookAccessToken.Equals("")) && (newCustomer.Password==null || newCustomer.Password.Equals("")))
            {
                // no authentication details provided for this customer
                throw new HttpResponseException("No credentials were supplied -- either a Facebook access token or a password are required.", System.Net.HttpStatusCode.InternalServerError);
            }

            // force all customers onto Stripe for now.
            newCustomer.BillingType = (int)BillingSystem.BillingProcessorFactory.SupportedBillingProcessor.Stripe;

            switch (signupType)
            {
                case SignupType.Credentials:
                    CoreHandleCredentialSignup(newCustomer);
                    break;
                case SignupType.Facebook:
                    CoreHandleFacebookSignup(newCustomer);
                    break;
            }

            // queue an email to be sent welcoming the user

            // send back a blank 200; the client should now try to authorize
            
        }
    }
}
