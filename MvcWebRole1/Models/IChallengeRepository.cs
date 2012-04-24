using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DareyaAPI.Models
{
    interface IChallengeRepository
    {
        ChallengeModel Get(int id);
        ChallengeModel Add(ChallengeModel item);
        void Remove(int id);
        bool Update(ChallengeModel item);

        IQueryable<ChallengeModel> GetListForUser(long userID);
        IQueryable<ChallengeModel> GetListForUser(long userID, int status);

    }
}
