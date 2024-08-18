using ChristmasCardApp.Configurations;
using ChristmasCardApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddOptions<RedisCacheSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("RedisCache").Bind(settings);
        });

        services.AddOptions<GithubSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("Github").Bind(settings);
        });

        services.AddHttpClient();
        services.AddScoped<IGitHubService, GitHubService>();
        services.AddScoped<IChristmasCardService, ChristmasCardService>();

        RedisCacheSettings redisCacheSettings =
        services.BuildServiceProvider().GetService<IOptions<RedisCacheSettings>>().Value;

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisCacheSettings.ConnectionString;
            options.InstanceName = redisCacheSettings.InstanceName;
        });

        services.AddSingleton<ICacheService, CacheService>();

    })
    .Build();

host.Run();
