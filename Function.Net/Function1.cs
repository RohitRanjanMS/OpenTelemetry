using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;

namespace Function.Net
{
    public static class Function1
    {

        public static ActivitySource MyActivitySource = new ActivitySource("MyActivitySourceName");
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";
            DoSomething(log);
            return new OkObjectResult(responseMessage);
        }

        private static async Task DoSomething(ILogger log)
        {

            using (Activity activity = MyActivitySource.StartActivity("DoingSomethingActivity"))
            {
                activity?.SetTag("foo", 1);
                activity?.SetTag("bar", "Hello, World!");
                activity?.SetTag("baz", new int[] { 1, 2, 3 });
                activity.AddEvent(new ActivityEvent("Before HttpCall"));
                await HTTPCallAsync();
                activity.AddEvent(new ActivityEvent("After HttpCall"));
                await WasteTime();
            }
            await LastTime();
            log.LogInformation("I am doing something.");
        }

        private static async Task HTTPCallAsync()
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://www.bing.com");
            Console.WriteLine(response.StatusCode);
        }

        private static async Task WasteTime()
        {
            using (Activity activity = MyActivitySource.StartActivity("TimeWasteActivity"))
            {
                await Task.Delay(1000);
            }
        }
        private static async Task LastTime()
        {
            using (Activity activity = MyActivitySource.StartActivity("LastActivity"))
            {
                await Task.Delay(1000);
            }
        }
    }
}
