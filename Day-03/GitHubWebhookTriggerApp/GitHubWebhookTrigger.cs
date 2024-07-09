using GitHubWebhookTriggerApp.Models;
using GitHubWebhookTriggerApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace GitHubWebhookTriggerApp
{
    public class GitHubWebhookTrigger
    {
        private readonly ILogger<GitHubWebhookTrigger> _logger;
        private readonly IGithubEventProcessor _githubEventProcessor;
        private readonly ICosmosDbService<AnimalImage> _cosmosDbService;

        public GitHubWebhookTrigger(ILogger<GitHubWebhookTrigger> logger, IGithubEventProcessor githubEventProcessor,
            ICosmosDbService<AnimalImage> cosmosDbService)
        {
            _logger = logger;
            _githubEventProcessor = githubEventProcessor;
            _cosmosDbService = cosmosDbService;
        }

        [Function("GitHubPushEventHandler")]
        public async Task<IActionResult> GitHubPushEventHandler([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("GitHubPushEventHandler received a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            IEnumerable<AnimalImage> images = _githubEventProcessor.GetImages(requestBody);

            if (images.Any())
            {
                _logger.LogInformation($"Writing {images.Count()} images to the database.");
                await _cosmosDbService.BulkInsertAsync(images);

                int cnt = 0;
                foreach (AnimalImage image in images)
                {
                    _logger.LogInformation($"Image[{cnt}] Id: {image.Id}");
                    _logger.LogInformation($"Image[{cnt}] CreationTime: {image.CreationTime}");
                    _logger.LogInformation($"Image[{cnt}] Author: {image.Author}");
                    _logger.LogInformation($"Image[{cnt}] ImageUrl: {image.ImageUrl}");
                    cnt++;
                }
            }
            else
            {
                _logger.LogInformation($"No images found in request.");
            }

            return new OkObjectResult("Successfully processed Github push event!");
        }
    }
}