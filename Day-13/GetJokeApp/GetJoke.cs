using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json.Serialization;

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
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
        // --- COSMOS DB INPUT BINDING CONFIGURATION ---
        [CosmosDBInput(
        databaseName: "AiContextDB",
        containerName: "AiContextContainer",
        Connection = "CosmosDbConnectionString",
        SqlQuery = "SELECT TOP 1 * FROM c ORDER BY c.timestamp DESC")] IEnumerable<JokeConversationContext> records)
    {
        string joke = "In a twist of humor, today's joke is that there isn't one.";
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var record = records.FirstOrDefault();
        if (record == null)
        {
            _logger.LogWarning("No records found by the Cosmos DB query.");
        }
        else
        {
            _logger.LogInformation($"Found record with ID: {record.Id} and Timestamp: {record.Timestamp}");
            ChatMessage[] chatMessages = GetChatCompletion(record);
            foreach (ChatMessage chatMessage in chatMessages) 
            {
                _logger.LogInformation($"{chatMessage.GetType().ToString()}, {chatMessage.Content[0].Text}");
            }
        }

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