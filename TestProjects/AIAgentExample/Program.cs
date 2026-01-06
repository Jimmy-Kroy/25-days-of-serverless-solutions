// See https://aka.ms/new-console-template for more information

/*


References:
https://learn.microsoft.com/en-us/agent-framework/tutorials/agents/run-agent?pivots=programming-language-csharp
https://learn.microsoft.com/en-us/agent-framework/user-guide/agents/agent-types/?pivots=programming-language-csharp
*/

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("App started!");
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var endpoint = config["endpoint"];
        var model = config["model"];

        //endpoint: https://ai-3026-exercise-resource.cognitiveservices.azure.com, model: gpt-4o
        Console.WriteLine($"endpoint: {endpoint}, model: {model}");

        //Using the Azure OpenAI SDK
        AzureOpenAIClient azureClient = new(
            new Uri(endpoint),
            new AzureCliCredential());

        IChatClient chatClient = azureClient.GetChatClient(model).AsIChatClient();

        //Create Joker agent
        AIAgent jokerAgent = new ChatClientAgent(
            chatClient,
            new ChatClientAgentOptions
            {
                Name = "Joker",
                Instructions = "You are good at telling jokes."
            });

        Console.WriteLine(await jokerAgent.RunAsync("Tell me a joke about a pirate."));
        /* Sure! Here's a pirate joke for you:
        Why did the pirate go to the Apple Store?
        To buy an **iPatch**! ????? */

        //Running the agent with streaming
        await foreach (var update in jokerAgent.RunStreamingAsync("Tell me a joke about a pirate."))
        {
            Console.WriteLine(update);
        }

        ChatMessage message = new(ChatRole.User, [
            new TextContent("Tell me a joke about this image?"),
            new UriContent("https://upload.wikimedia.org/wikipedia/commons/1/11/Joseph_Grimaldi.jpg", "image/jpeg")
        ]);

        Console.WriteLine(await jokerAgent.RunAsync(message));
        //Why don't clowns ever get stressed?
        //Because they know how to juggle all their problems.and their wine! Cheers to multitasking! ????

        //Running the agent with ChatMessages
        ChatMessage systemMessage = new(ChatRole.System,
            """
            
            If the user asks you to tell a joke, refuse to do so, explaining that you are not a clown.
            Offer the user an interesting fact instead.
            
            """);
        ChatMessage userMessage = new(ChatRole.User, "Tell me a joke about a pirate.");

        Console.WriteLine(await jokerAgent.RunAsync([systemMessage, userMessage]));
        //I'm not a clown, so I'll have to pass on the joke! But here's an interesting fact instead:
        //Pirates often wore eye patches, not necessarily because they had lost an eye,
        //but to help their vision adjust more quickly when moving between the bright deck and the dark below deck.

        Console.WriteLine("App finished!");
    }
}