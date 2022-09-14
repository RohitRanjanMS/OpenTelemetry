using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
//using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using System.Diagnostics;
using Azure.Monitor.OpenTelemetry.Exporter;

[assembly: FunctionsStartup(typeof(HttpTrigger2.Startup))]
namespace HttpTrigger2
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var serviceName = "Func.HttpTrigger2";
            var serviceVersion = "1.0.0";

            var openTelemetry = Sdk.CreateTracerProviderBuilder()
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                //.AddConsoleExporter()
                .AddAzureMonitorTraceExporter(o =>
                {
                    o.ConnectionString = "<>";
                })
                .Build();
            builder.Services.AddSingleton(openTelemetry);
        }
    }
}