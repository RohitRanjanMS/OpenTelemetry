using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Function.ServiceBusTrigger
{
    public static class Function1
    {
        public static ActivitySource MyActivitySource = new ActivitySource("SBTrigger");
        [FunctionName("Function1")]
        public static void Run([ServiceBusTrigger("Q1", Connection = "constring")] Message myQueueItem, ILogger log)
        {
            if(myQueueItem.UserProperties.ContainsKey("Diagnostic-Id"))
            {
                using (Activity activity = MyActivitySource.StartActivity("Func.Process", ActivityKind.Internal,
                    myQueueItem.UserProperties["Diagnostic-Id"].ToString()))
                {
                    log.LogInformation($"C# ServiceBus queue trigger function processed message with DiagId: {Encoding.Default.GetString(myQueueItem.Body)}");
                    Thread.Sleep(500);
                }
            }
            else
            {
                using (Activity activity = MyActivitySource.StartActivity("Func.Process", ActivityKind.Internal))
                {
                    log.LogInformation($"C# ServiceBus queue trigger function processed message: { Encoding.Default.GetString(myQueueItem.Body)}");
                    Thread.Sleep(500);
                }
            }           

        }
    }
}
