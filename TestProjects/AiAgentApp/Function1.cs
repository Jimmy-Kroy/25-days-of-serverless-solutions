using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using OpenAI.Assistants;
using OpenAI.Chat;
using OpenAI.Files;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

/*

Content file data.txt: 
Category,Cost
Accommodation, 674.56
Transportation, 2301.00
Meals, 267.89
Misc., 34.50

References:
https://github.com/Yewo-Devs/BoilerPlate-AngularMongoS3/blob/2e64a20db820687c5c8fc5e0105490686465f67e/API/Infrastructure/Services/ChatGptService.cs#L74


To send file via Postman to the Rest function
1) Select body, 
2) Add for example key file or myfile
3) Set the type to file
4) Select the file you want to upload. 

*/

namespace AiAgentApp;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private readonly IConfiguration _configuration;

    public Function1(ILogger<Function1> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Function("Function1")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        // Read from configuration
        string? endpoint = _configuration["Endpoint"];
        string? apiKey = _configuration["ApiKey"];
        string? deploymentName = _configuration["DeploymentName"];

        AzureOpenAIClient azureClient = new(
            new Uri(endpoint),
            new AzureKeyCredential(apiKey));

        // Get the file client for uploading files
        OpenAIFileClient fileClient = azureClient.GetOpenAIFileClient();

        // Check if a file was uploaded in the request
        IFormFile? uploadedFile = req.Form.Files.Count > 0 ? req.Form.Files[0] : null;

        if (uploadedFile == null)
        {
            return new BadRequestObjectResult("Please upload a file.");
        }

        _logger.LogInformation($"Req.Form.Files.Count:  {req.Form.Files.Count}");
        _logger.LogInformation($"Uploading file: {uploadedFile.FileName}");

        // Upload the file to Azure OpenAI
        OpenAIFile file;
        using (Stream fileStream = uploadedFile.OpenReadStream())
        {
            file = await fileClient.UploadFileAsync(
                fileStream,
                uploadedFile.FileName,
                FileUploadPurpose.Assistants);
        }

        _logger.LogInformation($"File uploaded successfully. File ID: {file.Id}");
        
        // Get the Assistants client
        AssistantClient client = azureClient.GetAssistantClient();

        // Create code interpreter tool resources (if you have file IDs to attach)
        AssistantCreationOptions agentOptions = new AssistantCreationOptions()
        {
            Name = "data-agent",
            Instructions = "You are an AI agent that analyzes the data in the file that has been uploaded. Use Python to calculate statistical metrics as necessary.",
            Tools = { new CodeInterpreterToolDefinition() }
        };

        //If you have uploaded files, add them to tool resources
        agentOptions.ToolResources = new()
        {
            CodeInterpreter = new()
            {
                FileIds = { file.Id }
            }
        };

        Assistant agent = await client.CreateAssistantAsync(deploymentName, agentOptions);

        _logger.LogInformation($"Using agent: {agent.Name}");

        //string prompt = "What's the category with the highest cost?";
        //Response agent: Let me first inspect the uploaded file to understand its structure and identify the relevant columns for the analysis. I'll then determine the category with the highest cost.
        //The file contains two columns: "Category" and "Cost". To identify the category with the highest cost, we need to find the category with the maximum value in the "Cost" column. Let me calculate this for you.
        //The category with the highest cost is **Transportation**, with a cost of 2301.0.


        //string prompt = "Create a text - based bar chart showing cost by category";
        //Response agent: To create a text-based bar chart of cost by category, I'll first need to load and inspect the data in the uploaded file from `/mnt/data/assistant-6rWHdkdDXbM7i8GxGCB9qT` to find the relevant fields. Let me begin by examining the file's content.
        //The data consists of two columns:
        //1. * *Category * *: Represents different expense categories.
        //2. * *Cost * *: Represents the cost associated with each category.
        //Let me now create a text-based bar chart showing the cost.
        //Here is the text - based bar chart showing cost by category:
        //```
        //Accommodation | ############## (674.56)
        //Transportation | ################################################## (2301.00)
        //Meals | ##### (267.89)
        //Misc.           | (34.50)
        //```
        //Each `#` represents a normalized amount of cost on a scale of 50 units. Categories with higher costs have longer bars. Let me know if you'd like to adjust the scale or format further!

        string prompt = "What's the standard deviation of cost?";
        //Response agent: Let me first examine the content of the uploaded file so I can identify the column containing the "cost" values. I will then calculate the standard deviation for that column.
        //The dataset contains two columns: "Category" and "Cost." I will calculate the standard deviation of the "Cost" values.
        //The standard deviation of the "Cost" column is approximately **1022.47**.

        var threadRun = await CreateThreadAndRunAsync(client, agent.Id, prompt);

        // Check back to see when the run is done
        do
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            threadRun = await client.GetRunAsync(threadRun.ThreadId, threadRun.Id);
        } while (!threadRun.Status.IsTerminal);

        var allMessages = await RetrieveMessagesAsync(client, threadRun.ThreadId);

        _logger.LogInformation($"Response agent: {allMessages}");
        return new OkObjectResult("Welcome to Azure Functions!");
    }

    private async Task<ThreadRun> CreateThreadAndRunAsync(AssistantClient assistantClient, string assistantId, string initialMessage)
    {
        var threadCreationOptions = new ThreadCreationOptions
        {
            InitialMessages = { initialMessage }
        };

        var threadRun = await assistantClient.CreateThreadAndRunAsync(assistantId, threadCreationOptions);
        return threadRun;
    }

    private async Task<string> RetrieveMessagesAsync(AssistantClient assistantClient, string threadId)
    {
        var messages = assistantClient.GetMessagesAsync(threadId, new MessageCollectionOptions { Order = MessageCollectionOrder.Ascending });

        var result = new StringBuilder();

        await foreach (var message in messages)
        {
            if (message.Role != MessageRole.User)
            {
                foreach (var contentItem in message.Content)
                {
                    if (!string.IsNullOrEmpty(contentItem.Text))
                    {
                        result.AppendLine(contentItem.Text);

                        if (contentItem.TextAnnotations.Count > 0)
                        {
                            result.AppendLine();
                        }

                        // Include annotations, if any.
                        foreach (var annotation in contentItem.TextAnnotations)
                        {
                            if (!string.IsNullOrEmpty(annotation.InputFileId))
                            {
                                result.AppendLine($"* File citation, file ID: {annotation.InputFileId}");
                            }
                            if (!string.IsNullOrEmpty(annotation.OutputFileId))
                            {
                                result.AppendLine($"* File output, new file ID: {annotation.OutputFileId}");
                            }
                        }
                    }
                }
                result.AppendLine();
            }
        }

        return result.ToString();
    }
}