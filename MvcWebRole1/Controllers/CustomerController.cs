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
        public class CustomerWithdrawRequest
        {
            public string Address { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZIPCode { get; set; }
        }

        public class CustomerSignupResult
        {
            public enum ResultCode
            {
                Success,
                EmailInUse,
                NoPasswordSupplied,
                Failed
            }

            public Customer Customer { get; set; }
            public ResultCode Result { get; set; }
        }

        public class BillingDTO
        {
            public string BillingID { get; set; }
            public int BillingType { get; set; }
        }

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

        private Customer FilterForAudience(Customer c, Security.Audience Audience)
        {
            Customer filtered = new Customer();

            switch (Audience)
            {
                case Security.Audience.Anybody:
                    filtered.FirstName = c.FirstName;
                    filtered.LastName = c.LastName;
                    filtered.AvatarURL = c.AvatarURL;
                    filtered.ID = c.ID;
                    break;
                case Security.Audience.Friends:
                    filtered.FirstName = c.FirstName;
                    filtered.LastName = c.LastName;
                    filtered.AvatarURL = c.AvatarURL;
                    filtered.ID = c.ID;
                    break;
                case Security.Audience.Owner:
                    filtered = c;
                    filtered.Password = null;
                    break;
                case Security.Audience.Users:
                    filtered.FirstName = c.FirstName;
                    filtered.LastName = c.LastName;
                    filtered.AvatarURL = c.AvatarURL;
                    filtered.ID = c.ID;
                    break;
            }

            return filtered;
        }

        [HttpGet]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public Account Balance(long id)
        {
            Account a = new Account();

            if (id != ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            a.Balance = RepoFactory.GetAccountRepo().BalanceForCustomerAccount(id);
            a.CustomerID = id;

            return a;
        }

        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void RequestWithdraw(CustomerWithdrawRequest request)
        {
            long id=((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;


        }

        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void Update(Customer profile)
        {
            Customer c = Repo.GetWithID(profile.ID);

            if (Security.DetermineAudience(c) != Security.Audience.Owner)
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            if(profile.FirstName!=null && !profile.FirstName.Equals(""))
                c.FirstName = profile.FirstName;

            if (profile.LastName != null && !profile.LastName.Equals(""))
                c.LastName = profile.LastName;

            if (profile.AvatarURL != null && !profile.AvatarURL.Equals(""))
                c.AvatarURL = profile.AvatarURL;

            if (profile.Address != null && !profile.Address.Equals(""))
                c.Address = profile.Address;

            if (profile.Address2 != null && !profile.Address2.Equals(""))
                c.Address2 = profile.Address2;

            if (profile.City != null && !profile.City.Equals(""))
                c.City = profile.City;

            if (profile.State != null && !profile.State.Equals(""))
                c.State = profile.State;

            if (profile.ZIPCode != null && !profile.ZIPCode.Equals(""))
                c.ZIPCode = profile.ZIPCode;

            Repo.Update(c);
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

        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void ForeignNetwork(CustomerForeignNetworkConnection d)
        {
            long curCustID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            if (d.CustomerID != curCustID)
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);

            OnboardManager mgr=new OnboardManager();

            try
            {
                bool res = mgr.LinkForeignUserToCustomer(Repo.GetWithID(curCustID), d.UserID, (Customer.ForeignUserTypes)d.Type);
                if (!res)
                    throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void Billing(BillingDTO billingInfo)
        {
            Customer c = Repo.GetWithID(((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID);

            c.BillingID = billingInfo.BillingID;
            c.BillingType = billingInfo.BillingType;

            Repo.Update(c);
        }

        //[HttpPost]
        //public void Claim(

        /*
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

        }*/

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
        
        public CustomerSignupResult HandleCredentialSignup(Customer newCustomer)
        {
            newCustomer.EmailAddress = newCustomer.EmailAddress.ToLower().Trim();

            Customer tryEmail = Repo.GetWithEmailAddress(newCustomer.EmailAddress);
            if (tryEmail != null && tryEmail.EmailAddress.Equals(newCustomer.EmailAddress))
            {
                if (tryEmail.Type != (int)Customer.TypeCodes.Unclaimed)
                    return new CustomerSignupResult { Customer = null, Result = CustomerSignupResult.ResultCode.EmailInUse };
            }

            newCustomer.Type = (int)Customer.TypeCodes.Unverified;
            
            try
            {
                CoreCreateSendVerificationEmail(newCustomer);
                if (tryEmail == null)
                    newCustomer.ID=Repo.Add(newCustomer);
                else
                    Repo.Update(tryEmail);
            }
            catch (Exception e)
            {
                return new CustomerSignupResult { Result = CustomerSignupResult.ResultCode.Failed, Customer=null };
            }

            return new CustomerSignupResult { Result = CustomerSignupResult.ResultCode.Success, Customer = newCustomer };
        }

        [HttpPost]
        public void Verify(Authorization verify)
        {
            
        }
        
        // POST /api/customer/signup
        [HttpPost]
        public void Signup(Customer newCustomer)
        {
            if (newCustomer.AvatarURL == null || newCustomer.AvatarURL.Equals(""))
                newCustomer.AvatarURL = "http://images.dareme.to/avatars/default.jpg";

            if(newCustomer.Password==null || newCustomer.Password.Equals(""))
            {
                // no authentication details provided for this customer
                throw new HttpResponseException("No credentials were supplied -- a password are required.", System.Net.HttpStatusCode.InternalServerError);
            }

            //newCustomer.BillingType = (int)BillingSystem.BillingProcessorFactory.SupportedBillingProcessor.Stripe;
            newCustomer.BillingType = (int)BillingSystem.BillingProcessorFactory.SupportedBillingProcessor.None;

            if (newCustomer.FirstName == null || newCustomer.FirstName.Equals("") || newCustomer.LastName == null || newCustomer.LastName.Equals(""))
                throw new HttpResponseException("New customers must supply a first and last name.", System.Net.HttpStatusCode.InternalServerError);

            CustomerSignupResult c=this.HandleCredentialSignup(newCustomer);
            
            if (c.Result != CustomerSignupResult.ResultCode.Success)
                throw new HttpResponseException("Couldn't sign up the new customer: " + c.Result.ToString(), System.Net.HttpStatusCode.Forbidden);
            
            // send back a blank 200; the client should now try to authorize
        }
    }
}
