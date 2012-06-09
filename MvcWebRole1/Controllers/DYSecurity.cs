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

        public enum Disposition
        {
            None,
            Originator,
            Backer,
            Taker
        }

        public Disposition DetermineDisposition(Challenge c)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return Disposition.None;

            long curCustID=((Models.DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;

            if(c.CustomerID==curCustID)
                return Disposition.Originator;

            if(c.TargetCustomerID==curCustID)
                return Disposition.Taker;

            if(RepoFactory.GetChallengeBidRepo().CustomerDidBidOnChallenge(curCustID, c.ID)!=null)
                return Disposition.Backer;

            return Disposition.None;
        }

        public Disposition DetermineDisposition(ChallengeStatus s)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return Disposition.None;

            long curCustID=((Models.DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;

            if (s.CustomerID == curCustID)
                return Disposition.Taker;

            if (s.ChallengeOriginatorCustomerID == curCustID)
                return Disposition.Originator;

            if (RepoFactory.GetChallengeBidRepo().CustomerDidBidOnChallenge(curCustID, s.ChallengeID)!=null)
                return Disposition.Backer;

            return Disposition.None;
        }

        public Audience DetermineVisibility(Customer c)
        {
            return Audience.Users;
        }

        public Audience DetermineVisibility(Challenge c)
        {
            return Audience.Users;
        }

        public bool CanManipulateContent(Challenge c)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return false;

            if (c.Privacy == (int)Challenge.ChallengePrivacy.SinglePerson)
            {
                if (c.TargetCustomerID == ((Models.DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                    return true;
                else
                    return false;
            }

            Audience a = CoreDetermineAudience(((Models.DareyaIdentity)HttpContext.Current.User.Identity).CustomerID);
            switch (a)
            {
                case Audience.Users:
                    //if (c.Privacy == (int)Challenge.ChallengePrivacy.FriendsOnly)
                    //    return false;

                    return true;

                case Audience.Owner:
                    return true;

                case Audience.Friends:
                    return true;

                case Audience.Anybody:
                default:
                    return false;
            }

        }

        /* DetermineAudience reports the audience the current user falls into with regards to the specified content.
         * 
         *      Example: 
         *          if they aren't logged in at all, return Anybody
         *          if the content is owned by a friend of theirs, return Friends
         *          if the content is owned by them, return Owner
         *          if they are logged in but the content doesn't fit any of these groups, return Users
        */
        private Audience CoreDetermineAudience(long CustomerID)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated && CustomerID == ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID)
                return Audience.Owner;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
                return Audience.Users;

            return Audience.Anybody;
        }

        public Audience DetermineAudience(Customer c)
        {
            return CoreDetermineAudience(c.ID);
        }

        public Audience DetermineAudience(Challenge c)
        {
            return CoreDetermineAudience(c.CustomerID);
        }

        public Authorization AuthorizeCustomer(Login l)
        {
            ICustomerRepository repo = Models.RepoFactory.GetCustomerRepo();

            Customer c=null;

            if (!l.EmailAddress.Equals(""))
            {
                c = repo.GetWithEmailAddress(l.EmailAddress);
                if (c == null)
                    return null;

                if (!l.Password.Equals(c.Password))
                    return null;
            }
            else
            {
                Facebook.FacebookClient fb = new Facebook.FacebookClient();

                c = repo.GetWithFacebookID(l.FacebookID);
                if (c == null)
                    return null;

                fb.AccessToken = l.FacebookToken;

                try
                {
                    dynamic me = fb.Get("me");

                    if (me == null || me.first_name.Equals(""))
                        return null;
                }
                catch (Exception e)
                {
                    return null;
                }

                c.FacebookAccessToken = l.FacebookToken;
                repo.Update(c); // store the newest Facebook access token since it may have changed
            }
            
            Authorization a = new Authorization("test" + System.DateTime.Now.Ticks.ToString());
            a.CustomerID = c.ID;
            a.EmailAddress = c.EmailAddress;

            IAuthorizationRepository authRepo = new AuthorizationRepository();
            authRepo.Add(a); // store the auth token in the repo

            return a;
        }
    }
}