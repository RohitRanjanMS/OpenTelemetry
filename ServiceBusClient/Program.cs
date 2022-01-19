using System.Diagnostics;
using Azure.Messaging.ServiceBus;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Azure.Monitor.OpenTelemetry.Exporter;

// Define some important constants and the activity source
var serviceName = "SB.Publisher";
var serviceVersion = "1.0.0";

// Configure important OpenTelemetry settings and the console exporter
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(serviceName)
    .AddSource("Azure.Messaging.ServiceBus")
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .AddOtlpExporter(options => options.Endpoint = new Uri("<collector endpoint>"))
    .AddAzureMonitorTraceExporter(o => {
        o.ConnectionString = "<AI Connectionstring>";
    })
    .AddConsoleExporter()
    .Build();

var MyActivitySource = new ActivitySource(serviceName);
ServiceBusClient client = new ServiceBusClient("<SB Connectionstring>");
var sender = client.CreateSender("Q1");
for (int i = 0; i < 1000; i++)
{
    Console.WriteLine(i);
    using var activity = MyActivitySource.StartActivity("SB.SendMessage");
    activity?.SetTag("counter", i);    
    await sender.SendMessageAsync(new ServiceBusMessage("{\"Name\":\"" + i + "\"}"));
    activity?.Stop();
    Thread.Sleep(30000);
}

