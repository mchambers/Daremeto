using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using DareyaAPI.Models;

namespace DareyaAPI.Controllers
{
    public class EvidenceController : ApiController
    {
        private EvidenceRepository EvidenceRepo;

        public EvidenceController()
        {
            EvidenceRepo = new EvidenceRepository();
        }

        // GET /api/<controller>/5
        [HttpGet]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public List<Evidence> Get(long id, string key)
        {
            ChallengeStatus s = new ChallengeStatus();
            s.ChallengeID = id;
            s.UniqueID = key;
            return EvidenceRepo.GetAllForChallengeStatus(s);
        }

        // POST /api/<controller>
        [HttpPost]
        [DareyaAPI.Filters.DYAuthorization(Filters.DYAuthorizationRoles.Users)]
        public void Post(Evidence value)
        {
            if (value.ChallengeStatusID == null || value.ChallengeStatusID.Equals(""))
                throw new HttpResponseException("You have to supply a challenge status ID.", System.Net.HttpStatusCode.Forbidden);

            if ((value.Content == null || value.Content.Equals("")) || (value.MediaURL == null || value.MediaURL.Equals("")))
                throw new HttpResponseException("No content or media URL supplied.", System.Net.HttpStatusCode.Forbidden);

            value.UniqueID = System.Guid.NewGuid().ToString();

            EvidenceRepo.Add(value);
        }
    }
}