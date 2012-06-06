using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DareyaAPI.Models;
using UrbanBlimp;
using UrbanBlimp.Apple;
using System.Diagnostics;
using SendGridMail;
using System.IO;
using RazorEngine;

namespace DaremetoWorker
{
    class EmailNotifyModel
    {
        public Challenge Challenge { get; set; }
        public Customer SourceCustomer { get; set; }
        public Customer TargetCustomer { get; set; }
    }

    class NotifyQueueItemProcessor : IQueueItemProcessor
    {
        private RequestBuilder GetIOSUASandboxCredentials()
        {
            return new RequestBuilder { NetworkCredential=new System.Net.NetworkCredential("g-AoI2jQSGaAfXJZBZT9GQ", "GMNCIs-WSDKDghWcqw6-Ng") };
        }

        private RequestBuilder GetIOSUAProductionCredentials()
        {
            return new RequestBuilder { NetworkCredential = new System.Net.NetworkCredential("m-3W3fEkS52DUwrGMhqQ-w", "-hOx6911T06Kt9G5Ff7BrA") };
        }
        
        private void PushToCustomer(long CustomerID, string Text)
        {
            IPushServiceTokenRepository tokenRepo = RepoFactory.GetPushServiceTokenRepo();
            
            AddRegistrationService reg = new AddRegistrationService { RequestBuilder = GetIOSUAProductionCredentials() };
            PushService service = new PushService { RequestBuilder = GetIOSUAProductionCredentials() };

            List<string> pushTokens = new List<string>();

            foreach (PushServiceToken t in tokenRepo.TokensForCustomer(CustomerID))
            {
                reg.Execute(t.Token, new Registration());
                pushTokens.Add(t.Token);
                Trace.WriteLine("PUSH: Registering device token "+t.Token+" for customer "+CustomerID);
            }

            PushPayload payload = new PushPayload();
            payload.Alert = Text;
            payload.Badge = "0";

            Trace.WriteLine("PUSH: Pushing \"" + Text + "\" to " + pushTokens.Count.ToString()+" clients - "+pushTokens.ToString());

            try
            {
                PushNotification notification = new PushNotification { DeviceTokens = pushTokens, Payload = payload };
                service.Execute(notification);
            }
            catch (Exception e)
            {
                Trace.WriteLine("PUSH: Exception encountered, " + e.ToString());
            }
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
            Customer sourceCust;
            Customer targetCust;
            Challenge chal;

            targetCust = RepoFactory.GetCustomerRepo().GetWithID(TargetCustomerID);
            sourceCust = RepoFactory.GetCustomerRepo().GetWithID(SourceCustomerID);
            chal = RepoFactory.GetChallengeRepo().Get(ChallengeID);

            EmailNotifyModel m = new EmailNotifyModel { Challenge = chal, SourceCustomer = sourceCust, TargetCustomer = targetCust };

            string template = null;
            string output = null;
            string email = null;
            string subject = null;

            switch (Type)
            {
                case CustomerNotifier.NotifyType.ChallengeAccepted:
                    break;
                case CustomerNotifier.NotifyType.ChallengeAwardedToYou:
                    break;
                case CustomerNotifier.NotifyType.ChallengeBacked:
                    break;
                case CustomerNotifier.NotifyType.ChallengeClaimed:
                    break;
                case CustomerNotifier.NotifyType.ChallengeRejected:
                    break;
                case CustomerNotifier.NotifyType.ChallengeYouBackedAwardedAssented:
                    break;
                case CustomerNotifier.NotifyType.ChallengeYouBackedAwardedDissented:
                    break;
                case CustomerNotifier.NotifyType.NewChallenge:
                    template = File.OpenText("NewChallenge.email").ReadToEnd();
                    output = Razor.Parse(template, m);
                    email = targetCust.EmailAddress;
                    subject = "You've been dared by " + sourceCust.FirstName;
                    break;
            }

            if (output != null && !output.Equals(""))
            {
                var message = SendGrid.GenerateInstance();
                message.AddTo(email);
                message.From = new System.Net.Mail.MailAddress("support@dareme.to");
                message.Html = output;
                message.Subject = subject;
                var transport = SendGridMail.Transport.SMTP.GenerateInstance(new System.Net.NetworkCredential("daremeto", "3f!margarita"));
                transport.Deliver(message);
            }

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
