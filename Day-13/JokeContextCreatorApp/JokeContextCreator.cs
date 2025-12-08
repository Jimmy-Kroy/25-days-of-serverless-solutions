using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace JokeContextCreatorApp;

public class JokeContextCreator
{
    private readonly ILogger<JokeContextCreator> _logger;

    public JokeContextCreator(ILogger<JokeContextCreator> logger)
    {
        _logger = logger;
    }

    [Function(nameof(JokeContextCreator))]
    [CosmosDBOutput(
        databaseName: "AiContextDB",
        containerName: "AiContextContainer",
        Connection = "CosmosDbConnectionString",
        PartitionKey = "/id",
        CreateIfNotExists = true)]
    public async Task<ConversationContext> Run([BlobTrigger("jokes-container/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name)
    {
        using var blobStreamReader = new StreamReader(stream);
        var jokesContent = await blobStreamReader.ReadToEndAsync();
        _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {name} \n Data: {jokesContent}", name, jokesContent);

        ConversationContext conversationContext = await CreateJokesContextAsync(jokesContent);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,                                    // Pretty print
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,      // Use camelCase
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull  // Ignore null values
        };

        string json = JsonSerializer.Serialize(conversationContext, options);
        _logger.LogInformation($"\n\n{json}\n\n");

        return conversationContext;
    }

    public static async Task<ConversationContext> CreateJokesContextAsync(string jokesContent)
    {
        return await Task.Run(() =>
        {
            // Split the jokes content by newlines and filter out empty lines
            var jokes = jokesContent
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            // Create the messages list with system prompt and joke exchanges
            var messages = new List<Message>
                {
                    new Message
                    {
                        Role = "system",
                        Content = "You are a comedian who tells authentic jokes that are funny. You don't repeat the jokes you already have told!"
                    }
                };

            // Add each joke as a user request followed by assistant response
            foreach (var joke in jokes)
            {
                messages.Add(new Message
                {
                    Role = "user",
                    Content = "Tell me a joke."
                });

                messages.Add(new Message
                {
                    Role = "assistant",
                    Content = joke.Trim()
                });
            }

            //Finally add one more message so that the model tells a joke.
            messages.Add(new Message
            {
                Role = "user",
                Content = "Tell me a joke."
            });

            // Create and return the ConversationContext
            var conversation = new ConversationContext
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Messages = messages
            };

            return conversation;
        });
    }
}

// POCOs for Cosmos DB
public class ConversationContext
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; }
}

public class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}