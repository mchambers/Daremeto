using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class ChallengeRepository : IChallengeRepository
    {
        private Database.DYDbEntities repo;

        public ChallengeRepository()
        {
            repo = new Database.DYDbEntities();
        }

        private Challenge DbChallengeToChallenge(Database.Challenge dc)
        {
            Challenge c = new Challenge();

            c.ID = dc.ID;
            c.Title = dc.Title;
            c.Description = dc.Description;
            c.CurrentBid = (int)dc.CurrentBid;
            c.Privacy = (int)dc.Privacy;
            c.State = (int)dc.State;
            c.TargetCustomerID = (int)dc.TargetCustomerID;
            c.CustomerID = dc.CustomerID;

            try
            {
                c.Visibility = Convert.ToInt32(dc.Visibility);
            }
            catch (Exception ex)
            {
                c.Visibility = 0;
            }

            return c;
        }

        private Database.Challenge ChallengeToDbChallenge(Challenge c, bool Attach=true)
        {
            Database.Challenge dc = new Database.Challenge();

            dc.ID = c.ID;

            if(Attach) repo.Challenge.Attach(dc);

            dc.Title = c.Title;
            dc.Description = c.Description;
            dc.CustomerID = c.CustomerID;
            dc.Privacy = (byte)c.Privacy;
            dc.State = c.State;
            dc.TargetCustomerID = c.TargetCustomerID;
            dc.CurrentBid = c.CurrentBid;
            dc.Visibility = c.Visibility;

            return dc;
        }

        public Challenge Get(long id)
        {
            Database.Challenge dc = repo.Challenge.FirstOrDefault(chal => chal.ID == id);
            repo.Challenge.Detach(dc);
            return DbChallengeToChallenge(dc);
        }

        public long Add(Challenge item)
        {
            Database.Challenge c = ChallengeToDbChallenge(item, false);
            repo.Challenge.AddObject(c);
            repo.SaveChanges();
            repo.Refresh(System.Data.Objects.RefreshMode.StoreWins, c);
            repo.Detach(c);
            return c.ID;
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Challenge item)
        {
            Database.Challenge c = ChallengeToDbChallenge(item);
            /*
            Database.Challenge c = repo.Challenge.FirstOrDefault(d => d.ID == item.ID);

            //c.Anonymous = item.Anonymous;
            c.CurrentBid = item.CurrentBid;
            c.CustomerID = item.CustomerID;
            c.Description = item.Description;
            c.Privacy = (byte)item.Privacy;
            c.State = item.State;
            c.Title = item.Title;
            c.TargetCustomerID = item.TargetCustomerID;
            */
            repo.SaveChanges();
            return true;
        }

        public List<Challenge> GetListForUser(long userID)
        {
            throw new NotImplementedException();
        }

        public List<Challenge> GetListForUser(long userID, int status)
        {
            throw new NotImplementedException();
        }

        public List<Challenge> GetNewest(int startAt, int amount)
        {
            IEnumerable<Database.Challenge> chals = (from c in repo.Challenge where c.Visibility==null || c.Visibility==(int)Challenge.ChallengeVisibility.Public select c).OrderByDescending(c=>c.ID).Skip(startAt).Take(amount);

            List<Challenge> listChals = new List<Challenge>();
            foreach (Database.Challenge c in chals)
            {
                listChals.Add(DbChallengeToChallenge(c));
            }

            return listChals;
        }

        public void AddBidToChallenge(Challenge item, long CustomerID, decimal BidAmount, decimal ComputedFees)
        {
            repo.AddBidToChallenge(item.ID, BidAmount, CustomerID, ComputedFees);
        }
    }
}