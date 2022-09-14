using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TimerTrigger
{
    public class TimerFunction
    {
        private static ActivitySource activitySource = new ActivitySource("TimerTrigger");
        [FunctionName("TimerFunction")]
        public async Task RunAsync([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            using (activitySource.StartActivity("HttpGet"))
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync("http://localhost:7223/api/HttpFunction1");
                Console.WriteLine("Sent Http request with traceparent : " + Activity.Current?.TraceId + ", response: " + response.StatusCode);
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
