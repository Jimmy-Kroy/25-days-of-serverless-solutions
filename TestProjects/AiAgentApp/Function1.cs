using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace AiAgentApp;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private readonly IConfiguration _configuration;

    public Function1(ILogger<Function1> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Function("Function1")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        // Read from configuration
        string? endpoint = _configuration["Endpoint"];
        string? apiKey = _configuration["ApiKey"];
        string? deploymentName = _configuration["DeploymentName"];

        AzureOpenAIClient azureClient = new(
            new Uri(endpoint),
            new AzureKeyCredential(apiKey));
        
        ChatClient chatClient = azureClient.GetChatClient(deploymentName);

        var requestOptions = new ChatCompletionOptions()
        {
            MaxOutputTokenCount = 4096,
            Temperature = 1.0f,
            TopP = 1.0f,

        };

        List<ChatMessage> messages = new List<ChatMessage>()
        {
            new SystemChatMessage("You are a helpful assistant."),
            new UserChatMessage("I am going to Paris, what should I see?"),
        };

        var response = chatClient.CompleteChat(messages, requestOptions);
        _logger.LogInformation("Response: " + response.Value.Content[0].Text);

        return new OkObjectResult("Welcome to Azure Functions!");
    }
}