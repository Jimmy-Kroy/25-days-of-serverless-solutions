using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WishListApp.Configurations;
using WishListApp.Models;
using WishListApp.Services;

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

        services.AddOptions<SlackSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("Slack").Bind(settings);
        });

        services.AddHttpClient<ISlackClient<Wish>, SlackClient<Wish>>();
        services.AddScoped<ISlackClient<Wish>, SlackClient<Wish>>();
        services.AddScoped<ICosmosDbService<Wish>, CosmosDbService<Wish>>();
    })
    .Build();

host.Run();
