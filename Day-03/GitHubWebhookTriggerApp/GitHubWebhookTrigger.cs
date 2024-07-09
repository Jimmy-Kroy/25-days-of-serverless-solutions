using GitHubWebhookTriggerApp.Models;
using GitHubWebhookTriggerApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Text.Json;

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

        [Function("GetImages")]
        public async Task<IActionResult> GetImages([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            const string query = "select* from c";
            string jsonString;

            _logger.LogInformation("GetImages received a request.");

            IEnumerable<AnimalImage> images = await _cosmosDbService.GetMultipleAsync(query);

            if (images.Any())
            {
                _logger.LogInformation($"{images.Count()} images retrieved from the database.");
                jsonString = JsonSerializer.Serialize(images);
            }
            else
            {
                _logger.LogInformation("No images found in the database.");
                jsonString = "No images found in the database.";
            }

            return new OkObjectResult(jsonString);
        }
    }
}