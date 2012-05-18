using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DareyaAPI.Models;

namespace DareyaAPI.Controllers
{
    public class CustomerNotifier
    {
        public enum NotifyType
        {
            NewChallenge,
            ChallengeAccepted,
            ChallengeRejected,
            ChallengeClaimed,
            ChallengeBacked,
            ChallengeYouBackedAwardedAssented,
            ChallengeYouBackedAwardedDissented,
            ChallengeAwardedToYou
        }

        public static Dictionary<string, long> GetNotifyQueueMessageData(NotifyType emailType, Customer Source, Customer Target, Challenge Challenge)
        {
            Dictionary<string, long> data = new Dictionary<string, long>();

            data.Add("nType", (long)emailType);
            if(Source!=null)
                data.Add("SrcID", Source.ID);
            if(Target!=null)
                data.Add("TgtID", Target.ID);
            if(Challenge!=null)
                data.Add("ChaID", Challenge.ID);

            return data;
        }

        public static void NotifyNewChallenge(Customer Source, Customer Target, Challenge Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.NewChallenge, Source, Target, Challenge));
        }

        public static void NotifyChallengeAccepted(Customer Source, Customer Target, Challenge Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeAccepted, Source, Target, Challenge));
        }

        public static void NotifyChallengeRejected(Customer Target, Challenge Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeRejected, null, Target, Challenge));
        }

        public static void NotifyChallengeClaimed(Customer Source, Customer Target, Challenge Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeClaimed, Source, Target, Challenge));
        }

        public static void NotifyChallengeBacked(Customer Source, Customer Target, Challenge Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeBacked, Source, Target, Challenge));
        }

        public static void NotifyChallengeYouBackedAwardedAssented(Customer Target, Challenge Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeYouBackedAwardedAssented, null, Target, Challenge));
        }

        public static void NotifyChallengeYouBackedAwardedDissented(Customer Target, Challenge Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeYouBackedAwardedDissented, null, Target, Challenge));
        }

        public static void NotifyChallengeAwardedToYou(Customer Target, Challenge Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeAwardedToYou, null, Target, Challenge));
        }
    }
}