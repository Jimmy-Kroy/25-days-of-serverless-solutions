using Azure;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using Azure.Identity;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace GetJokeAppV2;

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
        _logger.LogInformation("GetJokeV2 C# HTTP trigger function processed a request.");
        AzureOpenAIClient azureClient = null;
        string joke = "In a twist of humor, today's joke is that there isn't one.";

        // Read from configuration
        string? endpoint = _configuration["JokeEndpoint"];
        string? apiKey = _configuration["JokeApiKey"];
        string? deploymentName = _configuration["DeploymentName"];
        bool useApiKey = _configuration.GetValue<bool>("UseApiKey", false);

        if (useApiKey)
        {
            azureClient = new AzureOpenAIClient(
                new Uri(endpoint),
                new AzureKeyCredential(apiKey));
        }
        else
        {
            azureClient = new AzureOpenAIClient(
                new Uri(endpoint),
                //new DefaultAzureCredential(),
                //"Cognitive Services OpenAI User" Added to foundry ai resource instead of foundry ai project.
                new ManagedIdentityCredential()); //If you only want to use MSI
        }

        ChatClient chatClient = azureClient.GetChatClient(deploymentName);

        // Support for this recently-launched model with MaxOutputTokenCount parameter requires
        // Azure.AI.OpenAI 2.2.0-beta.4 and SetNewMaxCompletionTokensPropertyEnabled
        var requestOptions = new ChatCompletionOptions()
        {
            MaxOutputTokenCount = 10000,
        };

        // The SetNewMaxCompletionTokensPropertyEnabled() method is an [Experimental] opt-in to use
        // the new max_completion_tokens JSON property instead of the legacy max_tokens property.
        // This extension method will be removed and unnecessary in a future service API version;
        // please disable the [Experimental] warning to acknowledge.
#pragma warning disable AOAI001
        requestOptions.SetNewMaxCompletionTokensPropertyEnabled(true);
#pragma warning restore AOAI001

        List<ChatMessage> messages = new List<ChatMessage>()
        {
            new SystemChatMessage("You are a funny comedian."),
            new UserChatMessage("Tell me a joke.")
        };

        var response = chatClient.CompleteChat(messages, requestOptions);

        joke = response.Value.Content[0].Text;

        _logger.LogInformation($"Response: {joke}");

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