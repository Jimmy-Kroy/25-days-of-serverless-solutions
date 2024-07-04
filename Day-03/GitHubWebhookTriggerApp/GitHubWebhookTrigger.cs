using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GitHubWebhookTriggerApp
{
    public class GitHubWebhookTrigger
    {
        private readonly ILogger<GitHubWebhookTrigger> _logger;

        public GitHubWebhookTrigger(ILogger<GitHubWebhookTrigger> logger)
        {
            _logger = logger;
        }

        [Function("GitHubWebhookTrigger")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
