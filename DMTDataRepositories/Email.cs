using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
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

        public static Dictionary<string, long> GetNotifyQueueMessageData(NotifyType emailType, Nullable<long> SourceCustomerID, Nullable<long> TargetCustomerID, Nullable<long> ChallengeID)
        {
            Dictionary<string, long> data = new Dictionary<string, long>();

            data.Add("nType", (long)emailType);
            if (SourceCustomerID != null)
                data.Add("SrcID", SourceCustomerID.GetValueOrDefault());
            if(TargetCustomerID!=null)
                data.Add("TgtID", TargetCustomerID.GetValueOrDefault());
            if(ChallengeID!=null)
                data.Add("ChaID", ChallengeID.GetValueOrDefault());

            return data;
        }

        public static void NotifyNewChallenge(long Source, long Target, long Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.NewChallenge, Source, Target, Challenge));
        }

        public static void NotifyChallengeAccepted(long Source, long Target, long Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeAccepted, Source, Target, Challenge));
        }

        public static void NotifyChallengeRejected(long Target, long Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeRejected, null, Target, Challenge));
        }

        public static void NotifyChallengeClaimed(long Source, long Target, long Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeClaimed, Source, Target, Challenge));
        }

        public static void NotifyChallengeBacked(long Source, long Target, long Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeBacked, Source, Target, Challenge));
        }

        public static void NotifyChallengeYouBackedAwardedAssented(long Target, long Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeYouBackedAwardedAssented, null, Target, Challenge));
        }

        public static void NotifyChallengeYouBackedAwardedDissented(long Target, long Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeYouBackedAwardedDissented, null, Target, Challenge));
        }

        public static void NotifyChallengeAwardedToYou(long Target, long Challenge)
        {
            RepoFactory.GetProcessingQueue().PutQueueMessage(ProcessingQueue.MessageType.Notify, 
                GetNotifyQueueMessageData(NotifyType.ChallengeAwardedToYou, null, Target, Challenge));
        }
    }
}