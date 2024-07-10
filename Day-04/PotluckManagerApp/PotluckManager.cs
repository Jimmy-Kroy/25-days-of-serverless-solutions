using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace PotluckManagerApp
{
    public class PotluckManager
    {
        private readonly ILogger<PotluckManager> _logger;

        public PotluckManager(ILogger<PotluckManager> logger)
        {
            _logger = logger;
        }

        [Function("PotluckManager")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
