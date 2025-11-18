using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BlobTriggerFunctionAppV2;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function(nameof(Function1))]
    public async Task Run([BlobTrigger("my-jokes/{name}", Connection = "AzureWebJobsStorage")] byte[] byteArray,
        [BlobInput("processed-jokes/{name}", Connection = "AzureWebJobsStorage")] BlobClient outputBlob,
        string name)
    {
        string content = Encoding.UTF8.GetString(byteArray);

        _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}", name, content);
        outputBlob.Upload(BinaryData.FromString(content), overwrite: true);
    }
}