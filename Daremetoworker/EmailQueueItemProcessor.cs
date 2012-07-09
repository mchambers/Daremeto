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
    public class EmailNotifyModel
    {
        public Challenge Challenge { get; set; }
        public Customer SourceCustomer { get; set; }
        public Customer TargetCustomer { get; set; }
    }

    public class NotifyQueueItemProcessor : IQueueItemProcessor
    {
        private RequestBuilder GetIOSUASandboxCredentials()
        {
            return new RequestBuilder { NetworkCredential=new System.Net.NetworkCredential("g-AoI2jQSGaAfXJZBZT9GQ", "GMNCIs-WSDKDghWcqw6-Ng") };
        }

        private RequestBuilder GetIOSUAProductionCredentials()
        {
            return new RequestBuilder { NetworkCredential = new System.Net.NetworkCredential("m-3W3fEkS52DUwrGMhqQ-w", "-hOx6911T06Kt9G5Ff7BrA") };
        }
        
        private void PushToCustomer(long CustomerID, string Text, long ChallengeID=0)
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

            if (pushTokens.Count == 0)
                return;

            PushPayload payload = new PushPayload();
            payload.Alert = Text;
            payload.Badge = "0";

            Trace.WriteLine("PUSH: Pushing \"" + Text + "\" to " + pushTokens.Count.ToString()+" clients - "+pushTokens.ToString());

            Dictionary<string, string> customData=null;

            if (ChallengeID > 0)
            {
                customData = new Dictionary<string, string>();
                customData.Add("dareid", ChallengeID.ToString());
            }
            try
            {
                PushNotification notification = new PushNotification { DeviceTokens = pushTokens, Payload = payload, CustomData=customData };
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
                    PushToCustomer(SourceCustomerID,  c.FirstName+" has taken your dare!", ChallengeID);
                    break;
                case CustomerNotifier.NotifyType.ChallengeAwardedToYou:     // target
                    PushToCustomer(TargetCustomerID, "A dare you've attempted has been awarded to you!", ChallengeID);
                    break;
                case CustomerNotifier.NotifyType.ChallengeBacked:           // source, target
                    c = RepoFactory.GetCustomerRepo().GetWithID(SourceCustomerID);
                    PushToCustomer(TargetCustomerID, c.FirstName+" has backed one of your dares!", ChallengeID);
                    break;
                case CustomerNotifier.NotifyType.ChallengeClaimed:          // source, target
                    c = RepoFactory.GetCustomerRepo().GetWithID(TargetCustomerID);
                    PushToCustomer(SourceCustomerID, c.FirstName+" claims to have completed a dare you issued", ChallengeID);
                    break;
                case CustomerNotifier.NotifyType.ChallengeRejected:         // target
                    
                    break;
                case CustomerNotifier.NotifyType.ChallengeYouBackedAwardedAssented: // target

                    break;
                case CustomerNotifier.NotifyType.ChallengeYouBackedAwardedDissented: // target

                    break;
                case CustomerNotifier.NotifyType.NewChallenge:              // source, target
                    c = RepoFactory.GetCustomerRepo().GetWithID(SourceCustomerID);
                    PushToCustomer(TargetCustomerID, c.FirstName+" has dared you to do something!", ChallengeID);
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

            try
            {
                switch (Type)
                {
                    case CustomerNotifier.NotifyType.ChallengeAccepted:
                        template = File.OpenText("ChallengeAccepted.email").ReadToEnd();
                        output = Razor.Parse(template, m);
                        email = sourceCust.EmailAddress;
                        subject = targetCust.FirstName+" has taken your dare";
                        break;
                    case CustomerNotifier.NotifyType.ChallengeAwardedToYou:
                        template = File.OpenText("ChallengeAwardedToYou.email").ReadToEnd();
                        output = Razor.Parse(template, m);
                        email = sourceCust.EmailAddress;
                        subject = "You've been awarded a bounty!";
                        break;
                    case CustomerNotifier.NotifyType.ChallengeBacked:
                        template = File.OpenText("ChallengeBacked.email").ReadToEnd();
                        output = Razor.Parse(template, m);
                        email = targetCust.EmailAddress;
                        subject = sourceCust.FirstName + " has backed one of your dares";
                        break;
                    case CustomerNotifier.NotifyType.ChallengeClaimed:
                        template = File.OpenText("ChallengeClaimed.email").ReadToEnd();
                        output = Razor.Parse(template, m);
                        email = sourceCust.EmailAddress;
                        subject = targetCust.FirstName + " thinks he's completed your dare";
                        break;
                    case CustomerNotifier.NotifyType.ChallengeRejected:
                        template = File.OpenText("ChallengeRejected.email").ReadToEnd();
                        output = Razor.Parse(template, m);
                        email = sourceCust.EmailAddress;
                        subject = targetCust.FirstName + " has rejected your dare";
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
            }
            catch (Exception e)
            {
                Trace.WriteLine("EMAIL PARSE EXCEPTION: " + e.ToString());
            }

            try
            {
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
            catch (Exception e)
            {
                Trace.WriteLine("EMAIL SEND EXCEPTION: " + e.ToString());
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
