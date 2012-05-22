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
using DareyaAPI.Models;
using DareyaAPI.ProcessingQueue;

namespace DaremetoWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private static CloudBlobClient blobStorage;
        private static bool storageInitialized = false;
        private static object gate = new Object();

        public override void Run()
        {
            IProcessingQueue queue;
            IQueueItemProcessor processor;

            queue = new ProcessingQueue();

            Trace.WriteLine("DaremetoWorker is running", "Startup");

            while (true)
            {
                ProcessingQueueItem item;

                item = queue.GetNextMessage();
                while (item!=null)
                {
                    try
                    {
                        Trace.WriteLine("Handling queue item of type "+item.Type.ToString(), "Run");
                        processor = QueueItemProcessorFactory.GetQueueItemProcessor(item.Type);
                        processor.HandleQueueItem(item);
                    }
                    catch (Exception ex)
                    {
                    }
                    
                    item = queue.GetNextMessage();
                }

                Thread.Sleep(5000);
            }
        }

        public override bool OnStart()
        {
            Trace.WriteLine("DaremetoWorker is starting", "Startup");

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            AppDomain appDomain = AppDomain.CurrentDomain;
            appDomain.UnhandledException += new UnhandledExceptionEventHandler(appDomain_UnhandledException);

            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();

            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", dmc);

            return base.OnStart();
        }

        private void InitStorage()
        {
            Trace.WriteLine("Init the diagnostic storage", "Startup");

            if (storageInitialized)
                return;

            lock (gate)
            {
                if (storageInitialized)
                    return;

                var storageAccount = CloudStorageAccount.FromConfigurationSetting("StorageConnectionString");

                blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference("WorkerExceptions");

                container.CreateIfNotExist();

                storageInitialized = true;
            }
        }

        void appDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.WriteLine("Unhandled exception in DaremetoWorker " + (e.ExceptionObject as Exception).ToString(), "DMTWorker::General");

            InitStorage();

            var container = blobStorage.GetContainerReference("WorkerExceptions");

            if (container != null)
            {
                Exception ex = e.ExceptionObject as Exception;

                if (ex != null)
                {
                    container.GetBlobReference(String.Format("DaremetoWorker-{0}-{1}",
                                                            RoleEnvironment.CurrentRoleInstance.Id,
                                                            DateTime.UtcNow.Ticks)).UploadText(ex.ToString());
                }
            }
            else
            {
                Trace.WriteLine("Couldn't get the unhandled exception storage container", "DMTWorker::General");
            }

            Thread.Sleep(60000);
        }
    }
}
