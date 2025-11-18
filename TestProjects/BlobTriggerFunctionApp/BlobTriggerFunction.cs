using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace BlobTriggerFunctionApp;

public class BlobTriggerFunction
{
    private readonly ILogger<BlobTriggerFunction> _logger;

    public BlobTriggerFunction(ILogger<BlobTriggerFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(BlobTriggerFunction))]
    [BlobOutput("processed-jokes/{name}", Connection = "AzureWebJobsStorage")]
    public async Task<string> Run([BlobTrigger("my-jokes/{name}", Connection = "AzureWebJobsStorage")] string content,
        string name)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}", name, content);

        // Return the content to be written to the output blob
        return content;
    }
}