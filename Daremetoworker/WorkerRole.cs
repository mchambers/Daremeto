using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using DareyaAPI.Controllers;
using DareyaAPI.Models;
using DareyaAPI.ProcessingQueue;

namespace DaremetoWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            IProcessingQueue queue;
            IQueueItemProcessor processor;

            queue = new ProcessingQueue();

            while (true)
            {
                ProcessingQueueItem item;

                item = queue.GetNextMessage();
                while (item!=null)
                {
                    processor = QueueItemProcessorFactory.GetQueueItemProcessor(item.Type);
                    processor.HandleQueueItem(item);
                    item = queue.GetNextMessage();
                }

                Thread.Sleep(10000);
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
