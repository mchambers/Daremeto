using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace DareyaAPI.ProcessingQueue
{
    public class ProcessingQueue : IProcessingQueue
    {
        private BinaryFormatter _formatter;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue queue;

        private string TableName = "processingqueue";

        public ProcessingQueue()
        {
            _formatter = new BinaryFormatter();

            storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            queueClient = storageAccount.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference(TableName);
            queue.CreateIfNotExist();
        }

        public void PutQueueMessage(MessageType type, Dictionary<string, long> data)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream(1024 * 64);
            _formatter.Serialize(stream, data);

            data.Add("Type", (long)type);

            CloudQueueMessage message = new CloudQueueMessage(stream.ToArray());
            queue.AddMessage(message);
        }

        public ProcessingQueueItem GetNextMessage()
        {
            ProcessingQueueItem item = new ProcessingQueueItem();

            CloudQueueMessage retrievedMessage = queue.GetMessage();

            if (retrievedMessage == null)
                return null;

            item.Data = (Dictionary<string, long>)_formatter.Deserialize(new System.IO.MemoryStream(retrievedMessage.AsBytes));
            item.Type = (MessageType)item.Data["Type"];

            queue.DeleteMessage(retrievedMessage);

            return item;
        }
    }
}