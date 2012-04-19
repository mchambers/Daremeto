using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace DareyaAPI.Controllers
{
    public class ChallengeController : ApiController
    {
        // GET /api/challenge
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET /api/challenge/5
        public string Get(int id)
        {
            return "value";
        }

        // POST /api/challenge
        public void Post(string value)
        {
        }

        // PUT /api/challenge/5
        public void Put(int id, string value)
        {
        }
        
        // DELETE /api/challenge/5
        public void Delete(int id)
        {
        }
    }
}
