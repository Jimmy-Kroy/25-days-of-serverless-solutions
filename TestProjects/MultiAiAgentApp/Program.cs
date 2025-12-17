// See https://aka.ms/new-console-template for more information

using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using Azure.AI.Projects;

/*


References:
https://learn.microsoft.com/en-us/azure/ai-foundry/agents/how-to/connected-agents?view=foundry-classic&pivots=csharp

The resulting output is:
2025-12-17 16:04:11 -       user: Agent response: What is the stock price of Microsoft?
2025-12-17 16:04:20 -  assistant: Agent response: The last known closing stock price of Microsoft (MSFT) was approximately **$328.37** as of October 31, 2023. For the most up-to-date stock price, you can check reliable financial platforms such as Google Finance or Bloomberg.

*/

class Program
{
    static int Main()
    {
        //If secrets.json present, appsettings.json will be ignored.
        //If you want to use secrets instead of the plain JSON file:
        //Right - click your project and select Manage User Secrets.
        //Add your data there in the same JSON format.
        //The ConfigurationBuilder will automatically prioritize the secrets over the appsettings.json file.         
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<Program>(optional: true)
            .Build();

        // --- Config & Auth
        var endpoint = config["endpoint"];
        var model = config["model"];
        var apikey = config["APIKEY"]; //From secrets.json, right click project -> Manage User Secrets   

        Console.WriteLine($"{endpoint}, {model}, {apikey} ");

        // 2. Initialize the Project Client with the Key
        //AIProjectClient projectClient = new(new Uri(endpoint), new ApiKeyCredential(apikey) //AzureKeyCredential(apikey));


        PersistentAgentsClient client = new(endpoint, new DefaultAzureCredential());

        PersistentAgent stockAgent = client.Administration.CreateAgent(
                model: model,
                name: "stock_price_bot",
                instructions: "Your job is to get the stock price of a company. If you don't know the realtime stock price, return the last known stock price."
            // tools: [...] tools that would be used to get stock prices
            );
        ConnectedAgentToolDefinition connectedAgentDefinition = new(new ConnectedAgentDetails(stockAgent.Id, stockAgent.Name, "Gets the stock price of a company"));

        PersistentAgent mainAgent = client.Administration.CreateAgent(
                model: model,
                name: "main_stock_price_bot",
                instructions: "Your job is to get the stock price of a company, using the available tools.",
                tools: [connectedAgentDefinition]
            );

        PersistentAgentThread thread = client.Threads.CreateThread();

        // Create message to thread
        PersistentThreadMessage message = client.Messages.CreateMessage(
            thread.Id,
            MessageRole.User,
            "What is the stock price of Microsoft?");

        // Run the agent
        ThreadRun run = client.Runs.CreateRun(thread, mainAgent);
        do
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            run = client.Runs.GetRun(thread.Id, run.Id);
        }
        while (run.Status == RunStatus.Queued
            || run.Status == RunStatus.InProgress);

        // Confirm that the run completed successfully
        if (run.Status != RunStatus.Completed)
        {
            throw new Exception("Run did not complete successfully, error: " + run.LastError?.Message);
        }

        Pageable<PersistentThreadMessage> messages = client.Messages.GetMessages(
            threadId: thread.Id,
            order: ListSortOrder.Ascending
        );

        foreach (PersistentThreadMessage threadMessage in messages)
        {
            Console.Write($"{threadMessage.CreatedAt:yyyy-MM-dd HH:mm:ss} - {threadMessage.Role,10}: ");
            foreach (MessageContent contentItem in threadMessage.ContentItems)
            {
                if (contentItem is MessageTextContent textItem)
                {
                    string response = textItem.Text;
                    if (textItem.Annotations != null)
                    {
                        foreach (MessageTextAnnotation annotation in textItem.Annotations)
                        {
                            if (annotation is MessageTextUriCitationAnnotation urlAnnotation)
                            {
                                response = response.Replace(urlAnnotation.Text, $" [{urlAnnotation.UriCitation.Title}]({urlAnnotation.UriCitation.Uri})");
                            }
                        }
                    }
                    Console.Write($"Agent response: {response}");
                }
                else if (contentItem is MessageImageFileContent imageFileItem)
                {
                    Console.Write($"<image from ID: {imageFileItem.FileId}");
                }
                Console.WriteLine();
            }
        }

        Console.WriteLine($"App finished");


        return 0;
    }
}
