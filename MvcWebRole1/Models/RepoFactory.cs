using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class RepoFactory
    {
        private static IAuthorizationRepository authRepo;
        private static IChallengeBidRepository bidRepo;
        private static IChallengeRepository chalRepo;
        private static IChallengeStatusRepository chalStatusRepo;
        private static IChallengeStatusVoteRepository chalStatusVoteRepo;
        private static IEvidenceRepository evidenceRepo;
        private static IFriendshipRepository friendshipRepo;
        private static ICustomerRepository customerRepo;
        private static IPushServiceTokenRepository tokenRepo;

        public static IAuthorizationRepository GetAuthorizationRepo()
        {
            if (authRepo == null)
                authRepo = new AuthorizationRepository();

            return authRepo;
        }

        public static IChallengeBidRepository GetChallengeBidRepo()
        {
            if (bidRepo == null)
                bidRepo = new ChallengeBidRepository();

            return bidRepo;
        }

        public static IChallengeRepository GetChallengeRepo()
        {
            if (chalRepo == null)
                chalRepo = new ChallengeRepository();

            return chalRepo;
        }

        public static IChallengeStatusRepository GetChallengeStatusRepo()
        {
            if (chalStatusRepo == null)
                chalStatusRepo = new ChallengeStatusRepository();

            return chalStatusRepo;
        }

        public static IChallengeStatusVoteRepository GetChallengeStatusVoteRepo()
        {
            if (chalStatusVoteRepo == null)
                chalStatusVoteRepo = new ChallengeStatusVoteRepository();

            return chalStatusVoteRepo;
        }

        public static IEvidenceRepository GetEvidenceRepo()
        {
            if (evidenceRepo == null)
                evidenceRepo = new EvidenceRepository();

            return evidenceRepo;
        }

        public static IFriendshipRepository GetFriendshipRepo()
        {
            if (friendshipRepo == null)
                friendshipRepo = new FriendshipRepository();

            return friendshipRepo;
        }

        public static ICustomerRepository GetCustomerRepo()
        {
            if (customerRepo == null)
                customerRepo = new CustomerRepository();

            return customerRepo;
        }

        public static IPushServiceTokenRepository GetPushServiceTokenRepo()
        {
            if (tokenRepo == null)
                tokenRepo = new PushServiceTokenRepository();

            return tokenRepo;
        }
    }
}