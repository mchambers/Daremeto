using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public interface IChallengeStatusVoteRepository
    {
        void Add(ChallengeStatusVote vote);
        int GetCount(ChallengeStatus status);
        int GetYesVotes(ChallengeStatus status);
        int GetNoVotes(ChallengeStatus status);
    }
}