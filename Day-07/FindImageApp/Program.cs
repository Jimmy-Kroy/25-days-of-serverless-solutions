using FindImageApp.Configurations;
using FindImageApp.Helpers;
using FindImageApp.Services;
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

        services.AddOptions<UnsplashClientSettings>()
 .Configure<IConfiguration>((settings, configuration) =>
 {
     configuration.GetSection("UnsplashClient").Bind(settings);
 });

        services.AddHttpClient();

        services.AddSingleton<IKeyVaultHelper>(factory =>
        {
            IOptions<UnsplashClientSettings> settings = factory.GetService<IOptions<UnsplashClientSettings>>();
            ILogger<KeyVaultHelper> logger = factory.GetService<ILogger<KeyVaultHelper>>();
            return new KeyVaultHelper(logger, settings.Value.KeyVaultName);
        });

        /* Retrieve the Unsplash Access Key from the Azure KeyVault and configure the UnsplashClient.*/
        services.AddScoped<IUnsplashClient>(factory =>
        {
            IOptions<UnsplashClientSettings> settings = factory.GetService<IOptions<UnsplashClientSettings>>();
            ILogger<UnsplashClient> logger = factory.GetService<ILogger<UnsplashClient>>();
            IKeyVaultHelper keyVaultHelper = factory.GetService<IKeyVaultHelper>();
            IHttpClientFactory httpClientFactory = factory.GetService<IHttpClientFactory>();
            /* Retrieve access key from Azure Key Vault. */
            string accessKey = keyVaultHelper.GetCachedSecret(settings.Value.SecretName);

            return new UnsplashClient(new Uri(settings.Value.EndpointUrl), accessKey, logger, httpClientFactory);
        });
    })
    .Build();

host.Run();
