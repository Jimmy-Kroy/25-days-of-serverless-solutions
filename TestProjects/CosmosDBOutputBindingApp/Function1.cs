using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;


namespace CosmosDBOutputBindingApp;


// Model class for the data to be written to Cosmos DB
public class CosmosDocument
{
    public string? id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
}


// Output binding class
public class HttpAndCosmosOutput
{
    [CosmosDBOutput(
        databaseName: "AiContextDB",
        containerName: "AiContextContainer",
        Connection = "CosmosDbConnectionString",
        CreateIfNotExists = true)]
    public CosmosDocument? CosmosDocument { get; set; }

    [HttpResult]
    public HttpResponseData? HttpResponse { get; set; }
}


public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("Function1")]
    public async Task<HttpAndCosmosOutput> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        // Create the document to be written to Cosmos DB
        CosmosDocument cosmosDocument = new CosmosDocument
        {
            id = Guid.NewGuid().ToString(),
            Name = "Sample Document",
            Description = "This document was created by an Azure Function",
            Timestamp = DateTime.UtcNow
        };

        // Create the HTTP response
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        await response.WriteStringAsync("Document successfully queued for Cosmos DB!");

        // Return both HTTP response and Cosmos DB document
        return new HttpAndCosmosOutput
        {
            HttpResponse = response,
            CosmosDocument = cosmosDocument
        };

    }
}
