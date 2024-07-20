using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SentimentAnalyzerApp.Configurations;
using SentimentAnalyzerApp.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddHttpClient<ITextTranslationService, TextTranslationService>();

        services.AddTransient<ITextTranslationService, TextTranslationService>();


        services.AddOptions<TextTranslationClientSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("TextTranslationClient").Bind(settings);
        });

        services.AddOptions<TextAnalyticsClientSettings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("TextAnalyticsClient").Bind(settings);
        });

    })
    .Build();

host.Run();
