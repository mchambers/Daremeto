using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public interface IEvidenceRepository
    {
        public List<Evidence> GetAllForChallengeStatus(ChallengeStatus s);
        public Evidence Get(string ChallengeStatusID, string EvidenceUniqueID);
    }
}