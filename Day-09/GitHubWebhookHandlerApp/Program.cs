using GitHubWebhookHandlerApp.Configurations;
using GitHubWebhookHandlerApp.Helpers;
using GitHubWebhookHandlerApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddOptions<GitHubSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("GitHub").Bind(settings);
        });

        services.AddHttpClient();
        services.AddSingleton<IKeyVaultHelper>(factory =>
        {
            IOptions<GitHubSettings> settings = factory.GetService<IOptions<GitHubSettings>>();
            ILogger<KeyVaultHelper> logger = factory.GetService<ILogger<KeyVaultHelper>>();
            return new KeyVaultHelper(logger, settings.Value.KeyVaultName);
        });

        services.AddScoped<IGitHubService>(factory =>
        {
            IOptions<GitHubSettings> settings = factory.GetService<IOptions<GitHubSettings>>();
            ILogger<GitHubService> logger = factory.GetService<ILogger<GitHubService>>();
            IKeyVaultHelper keyVaultHelper = factory.GetService<IKeyVaultHelper>();
            IHttpClientFactory httpClientFactory = factory.GetService<IHttpClientFactory>();
            /* Retrieve the Token and UserAgent from the Azure KeyVault and configure the Github service object.*/
            string token = keyVaultHelper.GetCachedSecret(settings.Value.TokenName);
            string userAgent = keyVaultHelper.GetCachedSecret(settings.Value.UserAgentName);
            return new GitHubService(logger, httpClientFactory, token, userAgent);
        });
    })
    .Build();

host.Run();
