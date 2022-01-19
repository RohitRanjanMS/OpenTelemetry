using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


app.Run();

builder.Services.AddLogging((loggingBuilder) =>
{
    // Configure logging filter here in code
    // or in host.json
    // loggingBuilder.AddFilter<OpenTelemetryLoggerProvider>("*", LogLevel.Warning);

    loggingBuilder.AddOpenTelemetry(
        (otelOptions) =>
        {
            otelOptions.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("testservice"));
            otelOptions.IncludeFormattedMessage = true;
            otelOptions.ParseStateValues = true;
            //otelOptions.AddConsoleExporter();
        });
});
builder.Services.AddOpenTelemetryTracing(builder =>
{
    builder.AddAspNetCoreInstrumentation();
    builder.AddHttpClientInstrumentation();
    builder.AddSource("MyApplicationActivitySource");
    //builder.AddConsoleExporter();
    builder.AddAzureMonitorTraceExporter(o =>
    {
        o.ConnectionString = "<AI Connectionstring>";
    });
    builder.AddOtlpExporter(options => options.Endpoint = new Uri("<collector endpoint>"));
});