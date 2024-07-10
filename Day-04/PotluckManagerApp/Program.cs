using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PotluckManagerApp.Configurations;
using PotluckManagerApp.Models;
using PotluckManagerApp.Services;
using System.Diagnostics;

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

        services.AddSingleton<ICosmosDbService<FoodDish>, CosmosDbService<FoodDish>>();
    })
    .Build();

host.Run();
