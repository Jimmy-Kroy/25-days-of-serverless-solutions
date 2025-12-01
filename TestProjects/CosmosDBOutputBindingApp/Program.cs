using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .Configure<LoggerFilterOptions>(options =>
    {
        //https://stackoverflow.com/questions/77565541/logs-not-appearing-in-application-insights-for-azure-functions-v4-with-net-8
        //https://stackoverflow.com/questions/78673187/application-insights-logging-configuration-ignored-on-azure-functions-v4-net-8
        // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs.
        // Application Insights requires an explicit override.
        // Log levels can also be configured using appsettings.json.
        // For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs
        LoggerFilterRule defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
            == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

        if (defaultRule is not null)
        {
            options.Rules.Remove(defaultRule);
        }
    });

builder.Build().Run();
