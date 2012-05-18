using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DareyaAPI.ProcessingQueue;

namespace DaremetoWorker
{
    public class QueueItemProcessorFactory
    {
        private static NotifyQueueItemProcessor _email = new NotifyQueueItemProcessor();
        private static MaintenanceQueueItemProcessor _maintenance = new MaintenanceQueueItemProcessor();
        private static BillingQueueItemProcessor _billing = new BillingQueueItemProcessor();
        private static PushServiceQueueItemProcessor _push = new PushServiceQueueItemProcessor();

        public static IQueueItemProcessor GetQueueItemProcessor(DareyaAPI.ProcessingQueue.MessageType Type)
        {
            switch (Type)
            {
                case MessageType.Billing:
                    return _billing;
                case MessageType.Notify:
                    return _email;
                case MessageType.Maintenance:
                    return _maintenance;
                default:
                    return null;
            }
        }
    }
}
