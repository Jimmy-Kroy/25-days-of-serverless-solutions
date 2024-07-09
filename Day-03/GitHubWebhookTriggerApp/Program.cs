using GitHubWebhookTriggerApp.Configurations;
using GitHubWebhookTriggerApp.Models;
using GitHubWebhookTriggerApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddOptions<CosmosDbSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("CosmosDB").Bind(settings);
        });

        services.AddSingleton<ICosmosDbService<AnimalImage>, CosmosDbService<AnimalImage>>();
        services.AddScoped<IGithubEventProcessor, GithubEventProcessor>();
    })
    .Build();

host.Run();