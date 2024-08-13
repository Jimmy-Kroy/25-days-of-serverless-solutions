using GitHubWebhookHandlerApp.Models;
using GitHubWebhookHandlerApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GitHubWebhookHandlerApp
{
    public class GitHubWebhookHandler
    {
        private readonly ILogger<GitHubWebhookHandler> _logger;
        private readonly IGitHubService _gitHubService;

        public GitHubWebhookHandler(ILogger<GitHubWebhookHandler> logger, IGitHubService gitHubService)
        {
            _logger = logger;
            _gitHubService = gitHubService;
        }

        [Function("GitHubWebhookHandler")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            bool IsSuccess = false;
            Issue issue = new Issue();

            _logger.LogInformation("GitHubWebhookHandler HTTP trigger function processed a request.");

            string jsonRequest = await new StreamReader(req.Body).ReadToEndAsync();

            IsSuccess = _gitHubService.TryParse(jsonRequest, ref issue);

            if (IsSuccess)
            {
                _logger.LogInformation($"Sender: {issue.Sender}.");
                _logger.LogInformation($"Action: {issue.Action}.");
                _logger.LogInformation($"ReplyUri: {issue.ReplyUri}.");
                if (string.Compare(issue.Action, "opened", true) == 0 ||
                    string.Compare(issue.Action, "created", true) == 0)
                {
                    IsSuccess = await _gitHubService.SendThankYouAsync(issue);
                }
            }

            return new OkObjectResult("Ok");
        }
    }
}
