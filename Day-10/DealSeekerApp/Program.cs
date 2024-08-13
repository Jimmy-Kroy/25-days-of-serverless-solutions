using DealSeekerApp.Configurations;
using DealSeekerApp.Models;
using DealSeekerApp.Services;
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

        services.AddOptions<TableStorageClientSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("TableStorageClient").Bind(settings);
        });

        services.AddOptions<BlobStorageClientSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("BlobStorageClient").Bind(settings);
        });

        /* https://stackoverflow.com/questions/38138100/addtransient-addscoped-and-addsingleton-services-differences
        Transient objects are always different; a new instance is provided to every controller and every service.
        Scoped objects are the same within a request, but different across different requests.
        Singleton objects are the same for every object and every request. */
        services.AddScoped<IDealsSeeker, TwitterService>();
        services.AddScoped<IHtmlGeneratorService, HtmlGeneratorService>();
        services.AddScoped<IBlobStorageClient, BlobStorageClient>();
        services.AddScoped<TableStorageClient<DealTableEntity>>();
    })
    .Build();

host.Run();
