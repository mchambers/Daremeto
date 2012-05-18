using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DareyaAPI.ProcessingQueue;

namespace DaremetoWorker
{
    public interface IQueueItemProcessor
    {
        void HandleQueueItem(ProcessingQueueItem item);
    }
}
