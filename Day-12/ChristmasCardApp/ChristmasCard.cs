using ChristmasCardApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ChristmasCardApp
{
    public class ChristmasCard
    {
        private readonly ILogger<ChristmasCard> _logger;
        private readonly ICacheService _cacheService;
        private readonly IGitHubService _gitHubService;
        private readonly IChristmasCardService _christmasCardService;
        public ChristmasCard(ILogger<ChristmasCard> logger, ICacheService cacheService,
            IGitHubService gitHubService, IChristmasCardService christmasCardService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _gitHubService = gitHubService;
            _christmasCardService = christmasCardService;
        }

        [Function("ChristmasCard")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            bool isSuccess = false;
            string name = string.Empty;
            string? christmasCard = string.Empty;
            string? markdownContent = string.Empty;

            _logger.LogInformation("ChristmasCard HTTP trigger function processed a request.");

            name = req.Query["name"];
            _logger.LogInformation($"Query name rcvd: {name}");
            isSuccess = !string.IsNullOrEmpty(name);

            if (isSuccess)
            {
                /* Check if name present in cache. */
                christmasCard = await _cacheService.GetStringAsync(name);
            }

            if (isSuccess && string.IsNullOrEmpty(christmasCard))
            {
                _logger.LogInformation($"Name: {name} NOT found in cache, retrieving content. ");
                markdownContent = await _gitHubService.GetContentAsync(name);
                isSuccess = !string.IsNullOrEmpty(markdownContent);
            }

            if (isSuccess && string.IsNullOrEmpty(christmasCard))
            {
                christmasCard = _christmasCardService.GetChristmasCard(markdownContent);
                isSuccess = !string.IsNullOrEmpty(christmasCard);

                if (isSuccess) //Update cache
                {
                    await _cacheService.SetStringAsync(name, christmasCard);
                }
            }

            if (isSuccess)
            {
                return new ContentResult()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Content = christmasCard,
                    ContentType = "text/html"
                };
            }
            if (!isSuccess && string.IsNullOrEmpty(name))
            {
                return new ContentResult()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Please pass a name in the query string!",
                    ContentType = "text/html"
                };
            }
            else if (!isSuccess && string.IsNullOrEmpty(markdownContent))
            {
                return new ContentResult()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Content = $"No markdown file found for name: {name}.",
                    ContentType = "text/html"
                };
            }
            else if (!isSuccess && string.IsNullOrEmpty(christmasCard))
            {
                return new ContentResult()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = $"Unable to convert markdownContent to html.",
                    ContentType = "text/html"
                };
            }
            else
            {
                return new ContentResult()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Something went wrong please consult with your administrator!",
                    ContentType = "text/html"
                };
            }
        }
    }
}
