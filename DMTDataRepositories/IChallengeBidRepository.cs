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
        List<ChallengeBid> GetForCustomer(long CustomerID);
        void Update(ChallengeBid bid);
        ChallengeBid CustomerDidBidOnChallenge(long CustomerID, long ChallengeID);
        int GetBidCountForChallenge(long ChallengeID);
        void UpdateStatusForBidsOnChallenge(long ChallengeID, string UniqueID, ChallengeBid.BidStatusCodes NewStatus);
        List<ChallengeBid> GetVotePendingBidsForCustomer(long CustomerID);
        List<ChallengeBid> GetActiveBidsForCustomer(long CustomerID);
    }
}