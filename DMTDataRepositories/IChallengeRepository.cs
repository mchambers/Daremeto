﻿using System;
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

        List<Challenge> GetNewest(int startAt, int amount);
        List<Challenge> GetListForUser(long userID);
        List<Challenge> GetListForUser(long userID, int status);
    }
}