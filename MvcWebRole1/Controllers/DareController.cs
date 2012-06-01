using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DareyaAPI.Models;

namespace DareyaAPI.Controllers
{
    public class DareController : Controller
    {
        //
        // GET: /Dare/

        public ActionResult Index()
        {
            return Latest();
        }

        public ActionResult Show(long id)
        {
            Challenge c = RepoFactory.GetChallengeRepo().Get(id);

            c.Customer = RepoFactory.GetCustomerRepo().GetWithID(c.CustomerID);
            
            if(c.TargetCustomerID!=0)
                c.TargetCustomer = RepoFactory.GetCustomerRepo().GetWithID(c.TargetCustomerID);
            
            c.Bids = RepoFactory.GetChallengeBidRepo().Get(c.ID);

            return View(c);
        }

        public ActionResult Latest()
        {
            IEnumerable<Challenge> c = RepoFactory.GetChallengeRepo().GetNewest(0, 10);
            return View(c);
        }
    }
}
