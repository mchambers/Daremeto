using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DareyaAPI.Models;
using Facebook;
using TweetSharp;

namespace DareyaAPI.Controllers
{
    public class OnboardResult
    {
        public OnboardToken OnboardToken { get; set; }
        public Customer Customer { get; set; }
    }

    public class OnboardManager
    {
        private const string _twitterAuthKey = "zb8BkscTtBjr5oyALIUHig";
        private const string _twitterConsumerSecret = "bqsUpg4JyCM3a9pW9Q5m8SY9eGjtySwOrIfSyEipY";

        private const string _fbAppID = "309260905814607";
        private const string _fbAppSecret = "c8c000c0ef5e4adc6a760662cd37638b";

        public bool LinkForeignUserToCustomer(Customer custToLink, string handle, Customer.ForeignUserTypes type)
        {
            ICustomerRepository custRepo=RepoFactory.GetCustomerRepo();
            OnboardResult res = new OnboardResult();

            Customer c = custRepo.GetWithForeignUserID(handle, type);
            if (c != null)
            {
                if (c.ID == custToLink.ID)
                    return true;

                if (c.Type != (int)Customer.TypeCodes.Unclaimed)
                    return false;

                RepoFactory.GetChallengeRepo().MoveChallengesToCustomer(c.ID, custToLink.ID);
                custRepo.Remove(c.ID);
            }

            custRepo.AddForeignNetworkForCustomer(custToLink.ID, handle, type);
            return true;
        }

        public OnboardResult CompleteFirstStepForeignUserOnboard(string handle, Customer.ForeignUserTypes type, string token = null, string tokenSecret = null)
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
                    BillingType = (int)BillingSystem.BillingProcessorFactory.SupportedBillingProcessor.None
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

        private OnboardResult CoreCompleteTwitter(string oauth_token, string oauth_verifier)
        {
            OnboardResult r = new OnboardResult();

            var requestToken = new OAuthRequestToken { Token = oauth_token };

            TwitterService service = new TwitterService(_twitterAuthKey, _twitterConsumerSecret);
            OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);

            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
            TwitterUser user = service.VerifyCredentials();

            string handle = "@" + user.ScreenName;

            if (handle == null || handle.Equals(""))
                return null;

            return CompleteFirstStepForeignUserOnboard(handle, Customer.ForeignUserTypes.Twitter, accessToken.Token, accessToken.TokenSecret);
        }

        public bool Complete(OnboardToken t)
        {
            ICustomerRepository custRepo = RepoFactory.GetCustomerRepo();

            Customer c = custRepo.GetWithID(t.CustomerID);

            Customer custCheck = custRepo.GetWithEmailAddress(t.EmailAddress.ToLower().Trim());

            c.Type = (int)Customer.TypeCodes.Default;

            // check for email address in use
            if (custCheck != null && custCheck.Type != (int)Customer.TypeCodes.Unclaimed)
            {
                Security s = new Security();

                Authorization a = s.AuthorizeCustomer(new Login { EmailAddress = t.EmailAddress, Password = t.Password });
                if (a != null && a.Valid)
                {
                    System.Diagnostics.Trace.WriteLine("Moving challenges from " + c.ID.ToString() + " to " + a.CustomerID.ToString());

                    // zomg u're real
                    custRepo.AddForeignNetworkForCustomer(a.CustomerID, t.ForeignUserID, (Customer.ForeignUserTypes)t.AccountType);

                    // we need to collapse the unclaimed account into the one we just found.
                    RepoFactory.GetChallengeRepo().MoveChallengesToCustomer(c.ID, a.CustomerID);
                    RepoFactory.GetChallengeStatusRepo().MoveStatusesToNewCustomer(c.ID, a.CustomerID);

                    // now that we've moved the challenges, delete the original customer
                    custRepo.Remove(c.ID);

                    return true;
                }
                else
                    return false;
            }
            else if (custCheck != null && custCheck.Type == (int)Customer.TypeCodes.Unclaimed)
            {
                c = custCheck;
                c.Type = (int)Customer.TypeCodes.IncompleteOnboard;
            }

            c.EmailAddress = t.EmailAddress.ToLower().Trim();
            c.FirstName = t.FirstName;
            c.LastName = t.LastName;
            c.Password = t.Password;

            custRepo.Update(c);
            custRepo.AddForeignNetworkForCustomer(c.ID, t.ForeignUserID, (Customer.ForeignUserTypes)t.AccountType);

            if (t.ChallengeID != 0)
            {
                DareManager dmgr = new DareManager();
                dmgr.Accept(t.ChallengeID, c.ID);
            }

            return true;
        }
    }
}