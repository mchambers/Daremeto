using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TweetSharp;
using DareyaAPI.Models;

namespace DareyaAPI.Controllers
{
    public class OnboardController : Controller
    {
        public class OnboardResult
        {
            public OnboardToken OnboardToken { get; set; }
            public Customer Customer { get; set; }
        }

        //
        // GET: /Onboard/
        private const string _twitterAuthKey = "zb8BkscTtBjr5oyALIUHig";
        private const string _twitterConsumerSecret = "bqsUpg4JyCM3a9pW9Q5m8SY9eGjtySwOrIfSyEipY";

        public ActionResult Index()
        {
            return View();
        }

        private void SafeUpdateOnboardToken(OnboardToken tok)
        {
            // only allows the updating of the email address, first name and last name fields

            OnboardToken updateToken = new OnboardToken();
            updateToken.CustomerID = tok.CustomerID;
            updateToken.VerificationString = tok.VerificationString;

            updateToken.FirstName = tok.FirstName;
            updateToken.LastName = tok.LastName;
            updateToken.EmailAddress = tok.EmailAddress;

            RepoFactory.GetOnboardTokenRepo().Update(updateToken);
        }

        private ActionResult CoreAuthorizeTwitter(string verifyString=null)
        {
            TwitterService service = new TwitterService(_twitterAuthKey, _twitterConsumerSecret);

            string redirectUri;

            //if (verifyString == null)
                redirectUri = "http://dareme.to/Onboard/CompleteTwitter/";
            //else
            //    redirectUri = "http://dareme.to/Onboard/CompleteTwitter/" + verifyString;

            OAuthRequestToken requestToken = service.GetRequestToken(redirectUri);

            Uri uri = service.GetAuthorizationUri(requestToken);
            return new RedirectResult(uri.ToString(), false);
        }

        public ActionResult AuthorizeTwitter(string id)
        {
            return CoreAuthorizeTwitter(id);
        }

        [HttpGet]
        public ActionResult AuthorizeTwitter()
        {
            return CoreAuthorizeTwitter();
        }

        private OnboardResult CoreCompleteTwitter(string oauth_token, string oauth_verifier)
        {
            OnboardResult r=new OnboardResult();

            var requestToken = new OAuthRequestToken { Token = oauth_token };

            TwitterService service = new TwitterService(_twitterAuthKey, _twitterConsumerSecret);
            OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);
            
            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
            TwitterUser user = service.VerifyCredentials();

            string handle = user.ScreenName;

            if (handle == null || handle.Equals(""))
                return null;

            Customer c = RepoFactory.GetCustomerRepo().GetWithForeignUserID(handle, Customer.ForeignUserTypes.Twitter);

            if (c == null)
            {
                Customer newCust = new Customer()
                {
                    ForeignUserID = handle,
                    ForeignUserType = (int)Customer.ForeignUserTypes.Twitter,
                    Type = (int)Customer.TypeCodes.Unclaimed,
                    BillingType = (int)BillingSystem.BillingProcessorFactory.SupportedBillingProcessor.Stripe
                };

                newCust.ID = RepoFactory.GetCustomerRepo().Add(newCust);

                OnboardToken newToken = new OnboardToken()
                {
                    CustomerID = newCust.ID,
                    VerificationString = System.Guid.NewGuid().ToString(),
                    Token=accessToken.Token,
                    Secret=accessToken.TokenSecret
                };

                RepoFactory.GetOnboardTokenRepo().Add(newToken);

                r.OnboardToken = newToken;
                r.Customer = newCust;
            }
            else
            {
                // we don't put this in the repo, we're just
                // using it as a DTO to get the token and secret
                // back to the controller
                r.OnboardToken = new OnboardToken()
                {
                    Token=accessToken.Token,
                    Secret=accessToken.TokenSecret
                };
                r.Customer=c;
            }

            return r;
        }

        public ActionResult CompleteTwitter(string id, string oauth_token, string oauth_verifier)
        {
            OnboardResult r = CoreCompleteTwitter(oauth_token, oauth_verifier);

            if (r == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);

            OnboardToken t = RepoFactory.GetOnboardTokenRepo().Get(id);
            t.Token = r.OnboardToken.Token;
            t.Secret = r.OnboardToken.Secret;
            RepoFactory.GetOnboardTokenRepo().Update(t);

            r.OnboardToken = t;
            r.OnboardToken.Secret = null;

            return View(r);
        }
        
        public ActionResult CompleteTwitter(string oauth_token, string oauth_verifier)
        {
            OnboardResult r = CoreCompleteTwitter(oauth_token, oauth_verifier);

            if (r == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);

            r.OnboardToken.Secret = null;

            return View(r);
        }

        [HttpPost]
        public ActionResult Complete(OnboardToken t)
        {
            ICustomerRepository custRepo = RepoFactory.GetCustomerRepo();

            OnboardToken origToken = RepoFactory.GetOnboardTokenRepo().Get(t.VerificationString);
            if (origToken == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound);

            Customer c = custRepo.GetWithID(origToken.CustomerID);
            
            Customer custCheck = custRepo.GetWithEmailAddress(t.EmailAddress.ToLower().Trim());

            c.Type = (int)Customer.TypeCodes.Default;

            // check for email address in use
            if (custCheck != null && custCheck.Type != (int)Customer.TypeCodes.Unclaimed)
            {
                Security s=new Security();

                Authorization a = s.AuthorizeCustomer(new Login { EmailAddress = t.EmailAddress, Password = t.Password });
                if (a != null && a.Valid)
                {
                    // zomg u're real
                    custRepo.AddForeignNetworkForCustomer(a.CustomerID, t.Token, t.AccountType);

                    // we need to collapse the unclaimed account into the one we just found.
                    RepoFactory.GetChallengeRepo().MoveChallengesToCustomer(c.ID, a.CustomerID);

                    // now that we've moved the challenges, delete the original customer
                    custRepo.Remove(c.ID);

                    return View();
                }
                else
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
            else if (custCheck != null && custCheck.Type == (int)Customer.TypeCodes.Unclaimed)
            {
                c = custCheck;
                c.Type = (int)Customer.TypeCodes.Unverified;
            }
            
            c.EmailAddress = t.EmailAddress.ToLower().Trim();
            c.FirstName = t.FirstName;
            c.LastName = t.LastName;
            c.Password = t.Password;

            custRepo.Update(c);
            custRepo.AddForeignNetworkForCustomer(c.ID, t.Token, t.AccountType);

            return View();
        }
    }
}
