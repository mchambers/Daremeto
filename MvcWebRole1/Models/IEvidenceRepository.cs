using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public interface IEvidenceRepository
    {
        public List<Evidence> GetAllForChallengeStatus(string ChallengeStatusUniqueID);
        public void Add(Evidence e);
    }
}