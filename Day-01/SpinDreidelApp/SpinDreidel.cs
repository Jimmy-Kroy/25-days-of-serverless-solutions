using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SpinDreidelApp.Models;
using SpinDreidelApp.Services;
using System.Text.Json;

namespace SpinDreidelApp
{
    public class SpinDreidel
    {
        private readonly ILogger<SpinDreidel> _logger;

        private readonly IDreidelService _dreidelService;

        public SpinDreidel(ILogger<SpinDreidel> logger, IDreidelService dreidelService)
        {
            _logger = logger;
            _dreidelService = dreidelService;
        }

        [Function("SpinDreidel")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("SpinDreidel HTTP trigger function processed a request.");
            DreidelSide dreidelSide = _dreidelService.Spin();

            string jsonString = JsonSerializer.Serialize(dreidelSide);
            _logger.LogInformation(jsonString);

            return new OkObjectResult(jsonString);
        }
    }
}
