using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DareyaAPI.Models
{
    public interface IActivityRepository
    {
        IEnumerable<Activity> GetActivityForChallenge(long ChallengeID);
        void Add(Activity value);
    }
}
