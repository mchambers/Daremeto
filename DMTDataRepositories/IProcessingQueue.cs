using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace DareyaAPI.ProcessingQueue
{
    public enum MessageType
    {
        Notify,
        Billing,
        Maintenance
    }

    public class ProcessingQueueItem
    {
        public Dictionary<string, long> Data { get; set; }
        public MessageType Type { get; set; }
    }

    public interface IProcessingQueue
    {
        void PutQueueMessage(MessageType type, Dictionary<string, long> data);
        ProcessingQueueItem GetNextMessage();
    }
}