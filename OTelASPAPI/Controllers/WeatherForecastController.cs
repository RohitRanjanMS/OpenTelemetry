using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace OTelASPAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public static ActivitySource MyActivitySource = new ActivitySource("MyActivitySourceName");
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            _logger.LogInformation("Inside controller");
            //await DoSomething(_logger);
            await HTTPCallAsync();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
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
                WasteTime();
            }
            LastTime();
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