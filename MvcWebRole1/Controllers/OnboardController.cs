﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TweetSharp;
using DareyaAPI.Models;
using Facebook;

namespace DareyaAPI.Controllers
{
    public class OnboardController : Controller
    {
        FacebookClient _fb;
        OnboardManager _obm;
        DareManager _dmgr;

        public OnboardController()
        {
            _fb = new FacebookClient();
            _obm = new OnboardManager();
            _dmgr = new DareManager();
        }

        //
        // GET: /Onboard/
        private const string _twitterAuthKey = "zb8BkscTtBjr5oyALIUHig";
        private const string _twitterConsumerSecret = "bqsUpg4JyCM3a9pW9Q5m8SY9eGjtySwOrIfSyEipY";

        private const string _fbAppID = "309260905814607";
        private const string _fbAppSecret = "c8c000c0ef5e4adc6a760662cd37638b";
        private const string _fbRedirectUrl = "http://www.dareme.to/Onboard/CompleteFacebook/";

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

        [HttpGet]
        public ActionResult AuthorizeFacebook()
        {
            var redirectUri=_fb.GetLoginUrl(new { 
                client_id=_fbAppID,
                client_secret=_fbAppSecret,
                redirect_uri=_fbRedirectUrl,
                response_type="code",
                scope="",
                state=System.Guid.NewGuid().ToString()
            });

            return Redirect(redirectUri.AbsoluteUri);
        }

        [HttpGet]
        public ActionResult AuthorizeTwitter()
        {
            TwitterService service = new TwitterService(_twitterAuthKey, _twitterConsumerSecret);

            string redirectUri;

            redirectUri = "http://www.dareme.to/Onboard/CompleteTwitter/";
            
            OAuthRequestToken requestToken = service.GetRequestToken(redirectUri);

            Uri uri = service.GetAuthorizationUri(requestToken);
            return new RedirectResult(uri.ToString(), false);
        }

        public ActionResult CompleteFacebook(string code, string state)
        {
            dynamic result = _fb.Post("oauth/access_token",
                                        new { 
                                            client_id=_fbAppID,
                                            client_secret=_fbAppSecret,
                                            redirect_uri = _fbRedirectUrl,
                                            code=code
                                        });

            string fbAccessToken = result.access_token;

            _fb.AccessToken = result.access_token;
            _fb.AppId = _fbAppID;
            _fb.AppSecret = _fbAppSecret;

            dynamic meResponse=_fb.Get("me");
            string fbID = meResponse.id;

            OnboardResult r = _obm.CompleteFirstStepForeignUserOnboard(fbID, Customer.ForeignUserTypes.Facebook, _fb.AccessToken);

            if (r == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);

            if (r.Customer.Type != (int)Customer.TypeCodes.Unclaimed)
            {
                return View("AlreadyACustomer");
            }
            else
            {
                r.OnboardToken.FirstName = meResponse.first_name;
                r.OnboardToken.LastName = meResponse.last_name;

                r.OnboardToken.Secret = null;
                return View("CompleteOnboard", r.OnboardToken);
            }
        }

        /*
        private OnboardResult CoreCompleteFirstStepForeignUserOnboard(string handle, Customer.ForeignUserTypes type, string token=null, string tokenSecret=null)
        {
            OnboardResult r = new OnboardResult();

            Customer c = RepoFactory.GetCustomerRepo().GetWithForeignUserID(handle, type);

            if (c == null)
            {
                Customer newCust = new Customer()
                {
                    ForeignUserID = handle,
                    ForeignUserType = (int)type,
                    Type = (int)Customer.TypeCodes.Unclaimed,
                    BillingType = (int)BillingSystem.BillingProcessorFactory.SupportedBillingProcessor.Stripe
                };

                newCust.ID = RepoFactory.GetCustomerRepo().Add(newCust);

                RepoFactory.GetCustomerRepo().AddForeignNetworkForCustomer(newCust.ID, handle, type);

                r.Customer = newCust;
            }
            else
            {
                // we don't put this in the repo, we're just
                // using it as a DTO to get the token and secret
                // back to the controller
                r.Customer = c;
            }

            OnboardToken newToken = new OnboardToken()
            {
                CustomerID = r.Customer.ID,
                VerificationString = System.Guid.NewGuid().ToString(),
                Token = token,
                Secret = tokenSecret,
                AccountType = (int)type,
                ForeignUserID = handle
            };

            RepoFactory.GetOnboardTokenRepo().Add(newToken);

            r.OnboardToken = newToken;

            return r;
        }
        */

        private OnboardResult CoreCompleteTwitter(string oauth_token, string oauth_verifier)
        {
            OnboardResult r=new OnboardResult();

            var requestToken = new OAuthRequestToken { Token = oauth_token };

            TwitterService service = new TwitterService(_twitterAuthKey, _twitterConsumerSecret);
            OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);
            
            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
            TwitterUser user = service.VerifyCredentials();

            string handle = "@"+user.ScreenName;

            if (handle == null || handle.Equals(""))
                return null;

            return _obm.CompleteFirstStepForeignUserOnboard(handle, Customer.ForeignUserTypes.Twitter, accessToken.Token, accessToken.TokenSecret);
        }

        public ActionResult CompleteTwitter(string oauth_token, string oauth_verifier)
        {
            OnboardResult r = CoreCompleteTwitter(oauth_token, oauth_verifier);

            if (r == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);

            if (r.Customer.Type != (int)Customer.TypeCodes.Unclaimed)
            {
                return View("AlreadyACustomer");
            }
            else
            {
                r.OnboardToken.Secret = null;
                return View("CompleteOnboard", r.OnboardToken);
            }
        }

        [HttpPost]
        public ActionResult Signup(SignupCustomer newCustomer)
        {
            CustomerController custCtl = new CustomerController();

            CustomerController.CustomerSignupResult result = custCtl.HandleCredentialSignup(newCustomer);

            if (result.Result != CustomerController.CustomerSignupResult.ResultCode.Success)
                return View("SignupFailed");
            
            return View("SignupComplete", result);
        }

        [HttpPost]
        public ActionResult Complete(OnboardToken t)
        {
            OnboardToken origToken = RepoFactory.GetOnboardTokenRepo().Get(t.VerificationString);
            if (origToken == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);

            origToken.EmailAddress = t.EmailAddress;
            origToken.Password = t.Password;
            origToken.FirstName = t.FirstName;
            origToken.LastName = t.LastName;

            if (_obm.Complete(origToken))
            {
                return View();
            }
            else
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
        }
    }
}
