using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DareyaAPI.Models
{
    public interface IChallengeRepository
    {
        Challenge Get(long id);
        long Add(Challenge item);
        void Remove(int id);
        bool Update(Challenge item);

        void AddBidToChallenge(Challenge item, long CustomerID, decimal BidAmount, decimal ComputedFees);
        int MoveChallengesToCustomer(long SourceCustomerID, long NewTargetCustomerID);

        IEnumerable<Challenge> GetNewest(int startAt, int amount);
        IEnumerable<Challenge> GetOpen(int startAt, int amount);

        IEnumerable<Challenge> GetListForUser(long userID);
        IEnumerable<Challenge> GetListForUser(long userID, int status);

        IEnumerable<Challenge> GetOpenForCustomer(long CustomerID);

        IEnumerable<Challenge> GetUnbilledChallenges();
    }
}