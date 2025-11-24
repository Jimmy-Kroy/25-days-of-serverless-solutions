using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace CosmosDBInputBindingApp;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("Function1")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
        // --- COSMOS DB INPUT BINDING CONFIGURATION ---
        [CosmosDBInput(
            databaseName: "AiContextDB",
            containerName: "AiContextContainer",
            Connection = "CosmosDbConnectionString",
            SqlQuery = "SELECT TOP 1 * FROM c ORDER BY c.timestamp DESC")] IEnumerable<JokeConversationContext> records)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var record = records.FirstOrDefault();

        if (record != null)
        {
            _logger.LogInformation($"Found record with ID: {record.Id} and Timestamp: {record.Timestamp}");
        }
        else
        {
            _logger.LogWarning("No records found by the Cosmos DB query.");
        }

        return new OkObjectResult("Welcome to Azure Functions!");
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