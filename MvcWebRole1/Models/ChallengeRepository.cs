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

            return c;
        }

        private Database.Challenge ChallengeToDbChallenge(Challenge c)
        {
            Database.Challenge dc = Database.Challenge.CreateChallenge(c.ID, c.Title, c.Description, c.CustomerID);

            dc.Privacy = (byte)c.Privacy;
            dc.State = c.State;
            dc.TargetCustomerID = c.TargetCustomerID;

            return dc;
        }

        public Challenge Get(int id)
        {
            Database.Challenge dc = repo.Challenge.FirstOrDefault(chal => chal.ID == id);
            return DbChallengeToChallenge(dc);
        }

        public void Add(Challenge item)
        {
            repo.Challenge.AddObject(ChallengeToDbChallenge(item));
            repo.SaveChanges();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Challenge item)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}