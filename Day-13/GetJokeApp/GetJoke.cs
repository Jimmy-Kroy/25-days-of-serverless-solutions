using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GetJokeApp;

public class GetJoke
{
    private readonly ILogger<GetJoke> _logger;

    public GetJoke(ILogger<GetJoke> logger)
    {
        _logger = logger;
    }

    [Function("GetJoke")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}