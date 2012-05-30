﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DareyaAPI.Models
{
    public interface IChallengeStatusRepository
    {
        List<ChallengeStatus> GetActiveChallengesForCustomer(long CustomerID);
        List<ChallengeStatus> GetActiveStatusesForChallenge(long ChallengeID);
        List<ChallengeStatus> GetChallengesBySourceCustomer(long CustomerID);
        List<ChallengeStatus> GetActiveChallengesBySourceCustomer(long CustomerID);

        void Add(ChallengeStatus value);
        ChallengeStatus Get(long CustomerID, long ChallengeID);
        void Update(ChallengeStatus value);

        bool CustomerTookChallenge(long CustomerID, long ChallengeID);
    }
}
