using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DareyaAPI.Models
{
    interface IChallengeRepository
    {
        Challenge Get(int id);
        void Add(Challenge item);
        void Remove(int id);
        bool Update(Challenge item);

        List<Challenge> GetNewest(int startAt, int amount);
        List<Challenge> GetListForUser(long userID);
        List<Challenge> GetListForUser(long userID, int status);
    }
}