using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FAP_57.SendPOSupplier.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Configuration
        var configuration = context.Configuration;

        // Services
        services.AddHttpClient<ILucyApiService, LucyApiService>();
        services.AddSingleton<IDataverseService, DataverseService>();

        // Logging
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddApplicationInsights();
            loggingBuilder.SetMinimumLevel(LogLevel.Information);
        });
    })
    .Build();

await host.RunAsync();
