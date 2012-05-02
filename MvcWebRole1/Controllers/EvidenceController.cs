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

        // GET /api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET /api/<controller>/5
        public List<Evidence> Get(string id)
        {
            return EvidenceRepo.GetAllForChallengeStatus(id);
        }

        // POST /api/<controller>
        public void Post(Evidence value)
        {
            if (value.ChallengeStatusID == null || value.ChallengeStatusID.Equals(""))
                throw new HttpResponseException("You have to supply a challenge status ID.", System.Net.HttpStatusCode.Forbidden);

            if ((value.Content == null || value.Content.Equals("")) || (value.MediaURL == null || value.MediaURL.Equals("")))
                throw new HttpResponseException("No content or media URL supplied.", System.Net.HttpStatusCode.Forbidden);

            value.UniqueID = System.Guid.NewGuid().ToString();

            EvidenceRepo.Add(value);
        }

        // PUT /api/<controller>/5
        public void Put(int id, string value)
        {
        }

        // DELETE /api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}