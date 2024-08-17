using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ChristmasCardApp
{
    public class ChristmasCard
    {
        private readonly ILogger<ChristmasCard> _logger;

        public ChristmasCard(ILogger<ChristmasCard> logger)
        {
            _logger = logger;
        }

        [Function("ChristmasCard")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
