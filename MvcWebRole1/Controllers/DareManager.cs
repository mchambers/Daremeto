using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DareyaAPI.Models;
using System.Diagnostics;

namespace DareyaAPI.Controllers
{
    public class DareManager
    {
        /*
        public void MoveChallengeStatusesToNewCustomer(long OriginalCustomerID, long NewCustomerID)
        {
            IChallengeStatusRepository statusRepo = RepoFactory.GetChallengeStatusRepo();

            List<ChallengeStatus> statuses = RepoFactory.GetChallengeStatusRepo().GetActiveChallengesForCustomer(OriginalCustomerID);
            foreach (ChallengeStatus s in statuses)
            {
                s.CustomerID = NewCustomerID;
                statusRepo.Add(s);
            }
        }
        */

        public ChallengeStatus Take(long ChallengeID, long CustomerID)
        {
            Challenge c = RepoFactory.GetChallengeRepo().Get(ChallengeID);

            if (c == null)
                throw new Exception("Challenge not found");

            ChallengeStatus s = new ChallengeStatus();
            s.ChallengeID = c.ID;
            s.ChallengeOriginatorCustomerID = c.CustomerID;
            s.CustomerID = ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID;
            s.Status = (int)ChallengeStatus.StatusCodes.Accepted;

            //Trace.WriteLine("Adding 'taking this dare' status for customer " + ((DareyaIdentity)HttpContext.Current.User.Identity).CustomerID.ToString() + " and challenge " + id.ToString(), "ChallengeController::Take");
            RepoFactory.GetChallengeStatusRepo().Add(s);

            CustomerNotifier.NotifyChallengeAccepted(s.ChallengeOriginatorCustomerID, s.CustomerID, c.ID);

            return s;
        }

        public void Accept(long ChallengeID, long CustomerID)
        {

        }
    }
}