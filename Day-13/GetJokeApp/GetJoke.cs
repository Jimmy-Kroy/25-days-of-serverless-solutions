using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ClientModel;
using OpenAI;
using OpenAI.Chat;

namespace GetJokeApp;

public class GetJoke
{
    private readonly ILogger<GetJoke> _logger;
    private readonly IConfiguration _configuration;

    public GetJoke(ILogger<GetJoke> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Function("GetJoke")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        string joke = "In a twist of humor, today's joke is that there isn't one.";

        // Read from configuration
        string? endpoint = _configuration["JokeEndpoint"];
        string? apiKey = _configuration["JokeApiKey"];
        string? deploymentName = _configuration["DeploymentName"];

        ChatClient client = new(
            credential: new ApiKeyCredential(apiKey),
            model: deploymentName,
            options: new OpenAIClientOptions()
            {
                Endpoint = new($"{endpoint}"),
            });

        ChatCompletion completion = client.CompleteChat(GetChatCompletion());

        _logger.LogInformation($"Model={completion.Model}");
        foreach (ChatMessageContentPart contentPart in completion.Content)
        {
            joke = contentPart.Text;
            _logger.LogInformation($"Chat Role: {completion.Role}");
            _logger.LogInformation("Joke:");
            _logger.LogInformation(joke);
        }

        return new OkObjectResult($"The joke of the day: {joke}");
    }

    private ChatMessage[] GetChatCompletion()
    {
        List<ChatMessage> messages = new List<ChatMessage>();

        messages.Add(new SystemChatMessage("You are a comedian who tells short, clever dad jokes."));
        messages.Add(new UserChatMessage("Tell me a joke."));
        messages.Add(new AssistantChatMessage("I'm tired of following my dreams. I'm just going to ask them where they are going and meet up with them later."));
        messages.Add(new UserChatMessage("Tell me another joke."));
        messages.Add(new AssistantChatMessage("Did you hear about the guy whose whole left side was cut off? He's all right now."));
        messages.Add(new UserChatMessage("Give me a joke."));
        messages.Add(new AssistantChatMessage("Why didn't the skeleton cross the road? Because he had no guts."));
        messages.Add(new UserChatMessage("Tell me a joke."));
        messages.Add(new AssistantChatMessage("Chances are if you've seen one shopping center, you've seen a mall."));
        messages.Add(new UserChatMessage("Tell me another joke."));

        return messages.ToArray();
    }
}