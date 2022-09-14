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

namespace HttpTrigger2
{
    public static class HttpFunction2
    {
        [FunctionName("HttpFunction2")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP 2 trigger function processed a request.");
            Console.WriteLine("Current ActivityId: " + Activity.Current?.Id);

            return new OkObjectResult("Hello2");
        }
    }
}
