using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public interface IChallengeBidRepository
    {
        void Add(ChallengeBid bid);
        List<ChallengeBid> Get(long ChallengeID);
    }
}