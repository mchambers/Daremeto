using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DareyaAPI.Models;
using UrbanBlimp;
using UrbanBlimp.Apple;

namespace DaremetoWorker
{
    class NotifyQueueItemProcessor : IQueueItemProcessor
    {
        private RequestBuilder GetIOSUASandboxCredentials()
        {
            return new RequestBuilder { NetworkCredential=new System.Net.NetworkCredential("g-AoI2jQSGaAfXJZBZT9GQ", "GMNCIs-WSDKDghWcqw6-Ng") };
        }
        
        private void PushToCustomer(long CustomerID, string Text)
        {
            IPushServiceTokenRepository tokenRepo = RepoFactory.GetPushServiceTokenRepo();
            
            AddRegistrationService reg = new AddRegistrationService { RequestBuilder = GetIOSUASandboxCredentials() };
            PushService service = new PushService { RequestBuilder = GetIOSUASandboxCredentials() };

            List<string> pushTokens = new List<string>();

            foreach (PushServiceToken t in tokenRepo.TokensForCustomer(CustomerID))
            {
                reg.Execute(t.Token, new Registration());
                pushTokens.Add(t.Token);
            }

            PushNotification notification = new PushNotification { DeviceTokens = pushTokens };
            service.Execute(notification);
        }

        private void HandlePushServiceNotifications(long SourceCustomerID, long TargetCustomerID, long ChallengeID, CustomerNotifier.NotifyType Type)
        {
            Customer c;

            switch (Type)
            {
                case CustomerNotifier.NotifyType.ChallengeAccepted:         // source, target
                    c = RepoFactory.GetCustomerRepo().GetWithID(TargetCustomerID);
                    PushToCustomer(SourceCustomerID,  c.FirstName+" has accepted your dare!");
                    break;
                case CustomerNotifier.NotifyType.ChallengeAwardedToYou:     // target
                    PushToCustomer(TargetCustomerID, "A dare you've attempted has been awarded to you!");
                    break;
                case CustomerNotifier.NotifyType.ChallengeBacked:           // source, target
                    c = RepoFactory.GetCustomerRepo().GetWithID(TargetCustomerID);
                    PushToCustomer(SourceCustomerID, c.FirstName+" has backed one of your dares!");
                    break;
                case CustomerNotifier.NotifyType.ChallengeClaimed:          // source, target
                    c = RepoFactory.GetCustomerRepo().GetWithID(TargetCustomerID);
                    PushToCustomer(SourceCustomerID, c.FirstName+" claims to have completed a dare you issued");
                    break;
                case CustomerNotifier.NotifyType.ChallengeRejected:         // target
                    
                    break;
                case CustomerNotifier.NotifyType.ChallengeYouBackedAwardedAssented: // target

                    break;
                case CustomerNotifier.NotifyType.ChallengeYouBackedAwardedDissented: // target

                    break;
                case CustomerNotifier.NotifyType.NewChallenge:              // source, target
                    c = RepoFactory.GetCustomerRepo().GetWithID(SourceCustomerID);
                    PushToCustomer(TargetCustomerID, c.FirstName+" has dared you to do something!");
                    break;
            }
        }

        private void HandleEmailNotifications(long SourceCustomerID, long TargetCustomerID, long ChallengeID, CustomerNotifier.NotifyType Type)
        {

        }

        public void HandleQueueItem(DareyaAPI.ProcessingQueue.ProcessingQueueItem item)
        {
            ICustomerRepository custRepo = RepoFactory.GetCustomerRepo();

            long SourceCustomerID;
            long TargetCustomerID;
            long ChallengeID;
            long NotificationType;

            item.Data.TryGetValue("SrcID", out SourceCustomerID);
            item.Data.TryGetValue("TgtID", out TargetCustomerID);
            item.Data.TryGetValue("nType", out NotificationType);
            item.Data.TryGetValue("ChaID", out ChallengeID);

            HandlePushServiceNotifications(SourceCustomerID, TargetCustomerID, ChallengeID, (CustomerNotifier.NotifyType)NotificationType);
            HandleEmailNotifications(SourceCustomerID, TargetCustomerID, ChallengeID, (CustomerNotifier.NotifyType)NotificationType);
        }
    }
}
