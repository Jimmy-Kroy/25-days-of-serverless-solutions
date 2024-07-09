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

        [Function("GitHubWebhookTrigger")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            IEnumerable<AnimalImage> images = _githubEventProcessor.GetImages(requestBody);

            if(images.Any())
            {
                foreach (AnimalImage image in images)
                {
                    _logger.LogInformation($"image.Author: {image.Author}");
                    _logger.LogInformation($"image.ImageUrl: {image.ImageUrl}");
                    _logger.LogInformation($"image.CreationTime: {image.CreationTime}");
                    _logger.LogInformation($"image.Id: {image.Id}");
                    await _cosmosDbService.AddAsync(image);
                }


            }
            else
            {
                _logger.LogInformation($"#IEnumerable<AnimalImage> images IS EMPTY");
            }





            //var data = JObject.Parse(requestBody);


            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}