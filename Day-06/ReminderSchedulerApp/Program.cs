using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReminderSchedulerApp.Configurations;
using ReminderSchedulerApp.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddOptions<SlackSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("Slack").Bind(settings);
        });

        services.AddOptions<ChronoServiceSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("ChronoService").Bind(settings);
        });

        services.AddHttpClient<ISlackClient, SlackClient>();
        services.AddHttpClient<IChronoService, ChronoService>();
        services.AddTransient<ISlackClient, SlackClient>();
        services.AddTransient<IChronoService, ChronoService>();
    })
    .Build();

host.Run();
