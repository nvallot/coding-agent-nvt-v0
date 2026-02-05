#nullable enable

using Azure.Data.Tables;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using FluentValidation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CsvProcessor.Functions.Configuration;
using CsvProcessor.Functions.Models;
using CsvProcessor.Functions.Services;
using CsvProcessor.Functions.Validators;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Configuration
        services.Configure<AppSettings>(options =>
        {
            options.SourceContainerName = configuration["SourceContainerName"] ?? "csv-input";
            options.ServiceBusTopicName = configuration["ServiceBusTopicName"] ?? "orders-topic";
            options.IdempotencyTableName = configuration["IdempotencyTableName"] ?? "ProcessedFiles";
        });

        // Azure Clients with Managed Identity
        services.AddSingleton(_ =>
        {
            var tableServiceUri = configuration["TableStorageConnection__tableServiceUri"];
            if (string.IsNullOrEmpty(tableServiceUri))
            {
                // Local development with Azurite
                return new TableServiceClient("UseDevelopmentStorage=true");
            }
            return new TableServiceClient(new Uri(tableServiceUri), new DefaultAzureCredential());
        });

        services.AddSingleton(_ =>
        {
            var fullyQualifiedNamespace = configuration["ServiceBusConnection__fullyQualifiedNamespace"];
            if (string.IsNullOrEmpty(fullyQualifiedNamespace))
            {
                throw new InvalidOperationException(
                    "ServiceBusConnection__fullyQualifiedNamespace is required");
            }
            return new ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential());
        });

        // TimeProvider for testability
        services.AddSingleton(TimeProvider.System);

        // Validators
        services.AddScoped<IValidator<CsvOrderLine>, CsvOrderLineValidator>();

        // Services
        services.AddScoped<IIdempotencyService, IdempotencyService>();
        services.AddScoped<ICsvParserService, CsvParserService>();
        services.AddScoped<IJsonTransformerService, JsonTransformerService>();
        services.AddScoped<IServiceBusPublisher, ServiceBusPublisher>();

        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

await host.RunAsync();
