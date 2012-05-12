using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using DareyaAPI.Models;

namespace DareyaAPI.Controllers
{
    public class BidController : ApiController
    {
        IChallengeBidRepository BidRepo;

        public BidController()
        {
            BidRepo = new ChallengeBidRepository();
        }

        // GET /api/bid
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET /api/bid/5
        public string Get(int id)
        {
            return "value";
        }

        // POST /api/bid
        public void Post(string value)
        {
        }

        // PUT /api/bid/5
        public void Put(int id, string value)
        {
        }

        // DELETE /api/bid/5
        public void Delete(int id)
        {
        }
    }
}
