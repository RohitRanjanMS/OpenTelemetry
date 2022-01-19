using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Azure.Communication;
using Azure.Communication.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using OpenTelemetry;

[assembly: FunctionsStartup(typeof(Function.ServiceBusTrigger.Startup))]
namespace Function.ServiceBusTrigger
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var serviceName = "Func.Processor";
            var serviceVersion = "1.0.0";
            builder.Services.AddLogging((loggingBuilder) =>
            {
                // Configure logging filter here in code
                // or in host.json
                // loggingBuilder.AddFilter<OpenTelemetryLoggerProvider>("*", LogLevel.Warning);

                loggingBuilder.AddOpenTelemetry(
                    (otelOptions) =>
                    {
                        otelOptions.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: serviceVersion));
                        otelOptions.IncludeFormattedMessage = true;
                        otelOptions.ParseStateValues = true;
                        //otelOptions.AddConsoleExporter();
                    });
            }
            );
            var openTelemetry = Sdk.CreateTracerProviderBuilder().
                AddSource("SBTrigger")
                
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                .SetSampler(new AlwaysOnSampler())
                .AddConsoleExporter()
                .AddOtlpExporter(options => options.Endpoint = new Uri("<collector endpoint>"))
                .AddAzureMonitorTraceExporter(o => {
                    o.ConnectionString = "<AI Connectionstring>";
                })
                .Build();
            builder.Services.AddSingleton(openTelemetry);
        }
    }
}