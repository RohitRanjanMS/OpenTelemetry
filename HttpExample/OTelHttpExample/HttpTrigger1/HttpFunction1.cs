using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;

namespace HttpTrigger1
{
    public static class HttpFunction1
    {
        [FunctionName("HttpFunction1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP 1 trigger function processed a request.");
            Console.WriteLine("Current ActivityId: " + Activity.Current?.Id);
            Console.WriteLine("Sending request to another Http trigger");

            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://localhost:7051/api/HttpFunction2");
            Console.WriteLine("Sent Http request with traceparent : " + Activity.Current?.TraceId + ", response: " + response.StatusCode);

            return new OkObjectResult("Hello1");
        }
    }
}
