using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using DareyaAPI.Models;

namespace DareyaAPI.Controllers
{
    public class CustomerController : ApiController
    {
        // GET /api/customer
        public IEnumerable<Customer> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET /api/customer/5
        public string Get(int id)
        {
            return "value";
        }

        // POST /api/customer
        public void Post(string value)
        {
        }

        // PUT /api/customer/5
        public void Put(int id, string value)
        {
        }

        // DELETE /api/customer/5
        public void Delete(int id)
        {
        }
    }
}
