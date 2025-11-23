using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace CosmosDBOutputBindingApp;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("Function1")]
    public async Task<HttpCosmosBindingResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        //Example JSON payload(you can also parse from req.Body)
        ConversationContext conversation = new ConversationContext
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Messages = new List<Message>
            {
                    new Message { Role = "system", Content = "You are a comedian who tells really funny jokes." },
                    new Message { Role = "user", Content = "Tell me a joke." },
                    new Message { Role = "assistant", Content = "I'm tired of following my dreams. I'm just going to ask them where they are going and meet up with them later." },
                    new Message { Role = "user", Content = "Tell me a joke." }
            }
        };

        // Create the HTTP response
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        await response.WriteStringAsync("Document successfully queued for Cosmos DB!");

        //string jsonContent = System.Text.Json.JsonSerializer.Serialize(conversation);
        //_logger.LogInformation("jsonContent: " + jsonContent);

        // Return both HTTP response and Cosmos DB document
        return new HttpCosmosBindingResult
        {
            HttpResponse = response,
            conversationContext = conversation
        };
    }
}

// Output binding class
public class HttpCosmosBindingResult
{
    [CosmosDBOutput(
        databaseName: "AiContextDB",
        containerName: "AiContextContainer",
        Connection = "CosmosDbConnectionString",
        PartitionKey = "/id",
        CreateIfNotExists = true)]
    public ConversationContext? conversationContext { get; set; }

    [HttpResult]
    public HttpResponseData? HttpResponse { get; set; }
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