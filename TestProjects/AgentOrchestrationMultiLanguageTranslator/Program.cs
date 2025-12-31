// See https://aka.ms/new-console-template for more information

using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.ClientModel.Primitives;

/*

REFERENCES:
https://poornasoysa.tech/build-ai-agents-workflow-dotnet-microsoft-agent-framework/
https://youtu.be/QlEeIrt00zw

OUTPUT:

App started.
https://ai-3026-exercise-resource.cognitiveservices.azure.com, gpt-4o

FrenchAgent:
Bienvenue dans notre application ! Veuillez vérifier votre adresse e-mail avant de continuer.

SpanishAgent:
¡Bienvenido a nuestra aplicación! Por favor, verifica tu correo electrónico antes de continuar.

QualityReviewerAgent:
Quality: Excellent
Feedback: Both French and Spanish translations accurately convey the meaning of the original text. The tone remains consistent and friendly, suitable for an application interface. No grammatical or cultural issues detected.

SummaryAgent:
=== Localization Summary ===
French: Excellent (tone consistent, no corrections needed)
Spanish: Excellent (tone consistent, no corrections needed)

All translations reviewed successfully; both convey the original intent clearly and are user-friendly.

App finished.
*/
public class Program
{
    public static async Task<int> Main()
    {
        Console.WriteLine("App started.");

        // Quality Reviewer Agent
        const string qualityReviewerAgentInstructions =
            """
            
            You are a multilingual translation quality reviewer.
            Check the translations for grammar accuracy, tone consistency, and cultural fit
            compared to the original English text.

            Give a brief summary with a quality rating (Excellent / Good / Needs Review).

            Example output:
            Quality: Excellent
            Feedback: Accurate translation, friendly tone preserved, minor punctuation tweaks only.
            
            """;

        // Summary Agent
        const string summaryAgentInstructions =
            """
            
            You are a localization summary assistant.
            Summarize the translation results below.
            For each language, list:
            - Translation quality
            - Tone feedback
            - Any corrections made
            
            Then, provide an overall summary in 3–5 lines.
            
            Example output:
            === Localization Summary ===
            French: Excellent (minor punctuation fixes)
            Spanish: Good (tone consistent)
            All translations reviewed successfully.
            
            """;

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        // --- Config & Auth
        //var endpoint = new Uri("https://ai-3026-exercise-resource.cognitiveservices.azure.com");
        //var model = "gpt-4o";
        var endpoint = config["endpoint"];
        var model = config["model"];

        Console.WriteLine($"{endpoint}, {model} ");

        AzureOpenAIClient azureClient = new(
            new Uri(endpoint),
            new AzureCliCredential());
            //new AzureKeyCredential(apiKey));

        IChatClient chatClient = azureClient.GetChatClient(model).AsIChatClient();

        //Creating the Agents
        // French Translator Agent
        AIAgent frenchAgent = new ChatClientAgent(
            chatClient,
            new ChatClientAgentOptions
            {
                Name = "FrenchAgent",
                Instructions = "You are a translation assistant that translates the provided text to French."
            });

        // Spanish Translator Agent
        AIAgent spanishAgent = new ChatClientAgent(
            chatClient,
            new ChatClientAgentOptions
            {
                Name = "SpanishAgent",
                Instructions = "You are a translation assistant that translates the provided text to Spanish."
            });

        AIAgent qualityReviewerAgent = new ChatClientAgent(
            chatClient,
            new ChatClientAgentOptions
            {
                Name = "QualityReviewerAgent",
                Instructions = qualityReviewerAgentInstructions
            });

        AIAgent summaryAgent = new ChatClientAgent(
            chatClient,
            new ChatClientAgentOptions
            {
                Name = "SummaryAgent",
                Instructions = summaryAgentInstructions
            });

        AIAgent workflowAgent = await AgentWorkflowBuilder
            .BuildSequential(frenchAgent, spanishAgent, qualityReviewerAgent, summaryAgent)
            .AsAgentAsync();

        //Console.Write("\nYou: ");
        //string userInput = Console.ReadLine() ?? string.Empty;

        string prompt = "Welcome to our application! Please verify your email before continuing.";

        AgentRunResponse response = await workflowAgent.RunAsync(prompt);

        Console.WriteLine();

        foreach (var message in response.Messages)
        {
            Console.WriteLine($"{message.AuthorName}: ");

            Console.WriteLine(message.Text);
            Console.WriteLine();
        }

        Console.WriteLine("App finished.");

        return 0;
    }
}