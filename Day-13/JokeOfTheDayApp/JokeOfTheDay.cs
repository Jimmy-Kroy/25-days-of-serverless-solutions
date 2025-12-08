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
using OpenAI.Chat;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JokeOfTheDayApp;

public class JokeOfTheDay
{
    private readonly ILogger<JokeOfTheDay> _logger;
    private readonly IConfiguration _configuration;

    public JokeOfTheDay(ILogger<JokeOfTheDay> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Function("JokeOfTheDay")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
        // --- COSMOS DB INPUT BINDING CONFIGURATION ---
        [CosmosDBInput(
            databaseName: "AiContextDB",
            containerName: "AiContextContainer",
            Connection = "CosmosDbConnectionString",
            SqlQuery = "SELECT TOP 1 * FROM c ORDER BY c.timestamp DESC")] IEnumerable<JokeConversationContext> records)
    {
        _logger.LogInformation("GetJokeV2 C# HTTP trigger function processed a request.");
        AzureOpenAIClient azureClient = null;
        ChatMessage[] chatMessages = Array.Empty<ChatMessage>();
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


        //Retrieve Joke Context from Cosmos DB    
        var record = records.FirstOrDefault();

        if (record != null)
        {
            _logger.LogInformation($"Found record with ID: {record.Id} and Timestamp: {record.Timestamp}");
            //Serialize record to Json file
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,                                    // Pretty print
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,      // Use camelCase
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull  // Ignore null values
            };

            string json = JsonSerializer.Serialize(record, options);
            _logger.LogInformation($"\n\n{json}\n\n");

            _logger.LogInformation($"Converting joke context to ChatMessage objects.");
            chatMessages = GetChatCompletion(record);
            LogChatMessages(chatMessages);

            //json = JsonSerializer.Serialize(chatMessages, options);
            //_logger.LogInformation($"\n\n{json}\n\n");
        }
        else
        {
            _logger.LogWarning("No records found by the Cosmos DB query.");
        }

        //List<ChatMessage> messages = new List<ChatMessage>()
        //{
        //    new SystemChatMessage("You are a funny comedian."),
        //    new UserChatMessage("Tell me a joke.")
        //};

        var response = chatClient.CompleteChat(chatMessages, requestOptions);

        joke = response.Value.Content[0].Text;

        _logger.LogInformation($"Response: {joke}");

        return new OkObjectResult($"The joke of the day: {joke}");
    }

    private ChatMessage[] GetChatCompletion(JokeConversationContext context)
    {
        if (context == null || context.Messages == null || context.Messages.Count == 0)
        {
            _logger.LogWarning("No messages found in the conversation context.");
            return Array.Empty<ChatMessage>();
        }

        List<ChatMessage> chatMessages = new List<ChatMessage>();

        foreach (var message in context.Messages)
        {
            ChatMessage chatMessage = message.Role.ToLower() switch
            {
                "system" => new SystemChatMessage(message.Content),
                "user" => new UserChatMessage(message.Content),
                "assistant" => new AssistantChatMessage(message.Content),
                _ => throw new ArgumentException($"Unknown role: {message.Role}")
            };

            chatMessages.Add(chatMessage);
        }

        return chatMessages.ToArray();
    }

    private void LogChatMessages(ChatMessage[] chatMessages)
    {
        //Serialize record to Json file
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,                                    // Pretty print
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,      // Use camelCase
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull  // Ignore null values
        };


        if (chatMessages == null || chatMessages.Length == 0)
        {
            _logger.LogInformation("ChatMessages array is null or empty.");
            return;
        }

        var messagesJson = JsonSerializer.Serialize(
            chatMessages.Select((msg, i) => new
            {
                Index = i + 1,
                Role = msg.GetType().ToString(),
                Content = msg.Content
            })
        , options);

        _logger.LogInformation("ChatMessages: {Messages}", messagesJson);
    }
}

// Represents the full chat record document stored in Cosmos DB
public class JokeConversationContext
{
    // Note: The 'id' field is crucial for Cosmos DB documents
    [JsonPropertyName("id")]
    public string Id { get; set; }

    // This is the field we use for sorting to find the most recent record
    // The timestamp format matches the example provided
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; }

    // Optional Cosmos DB system properties, useful for debugging/validation
    [JsonPropertyName("_ts")]
    public long? TimestampSeconds { get; set; }
}

// Represents a single message part within the chat history
public class Message
{
    // Use JsonPropertyName to map to the lowercase JSON field
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}