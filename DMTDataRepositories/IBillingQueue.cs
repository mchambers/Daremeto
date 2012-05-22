using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public interface IBillingQueue
    {
        void ProcessBilling(ChallengeStatus s);
    }
}