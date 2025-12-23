// See https://aka.ms/new-console-template for more information

/*

References:
https://medium.com/microsoft-azure-in-practice/azure-ai-foundry-agent-service-c-build-production-ready-ai-agents-f1d3afb0f90a
https://github.com/divyeshg94/TravelPlanner.AI/tree/master



Resulting Output:

To: expenses@contoso.com
Subject: Expense Claim
Here are the details of the expenses:

1. Date: 07-Mar-2025, Description: Taxi, Amount: $24.00
2. Date: 07-Mar-2025, Description: Dinner, Amount: $65.50
3. Date: 07-Mar-2025, Description: Hotel, Amount: $125.90

Total: $215.40


Assistant: I have submitted your expense claim to expenses@contoso.com with the itemized details and the total amount of $215.40.
*/

using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text.Json;


class Program
{
    // Track which call_ids we've already submitted per run
    public static readonly ConcurrentDictionary<string, HashSet<string>> SubmittedByRun =
        new ConcurrentDictionary<string, HashSet<string>>();

    static async Task Main()
    {
        string model = "gpt-4o";
        string endpoint = @"https://ai-3026-maf-example-resource.services.ai.azure.com/api/projects/ai-3026-MAF-example";

        const string AgentName = "expenses_agent";
        const string AgentInstructions = "You are an AI assistant for expense claim submission. When a user submits expenses data and requests an expense claim, use the plug-in function to send an email to expenses@contoso.com with the subject 'Expense Claim`and a body that contains itemized expenses with a total. Then confirm to the user that you've done so.";

        string prompt = "I requests an expense claim. Here is the expenses data: " +
            "date, description, amount " +
            "07-Mar-2025, taxi,24.00 " +
            "07-Mar-2025, dinner,65.50" +
            "07-Mar-2025, hotel,125.90";

        Console.WriteLine("APP started!");

        PersistentAgentsClient persistentAgentsClient = new PersistentAgentsClient(endpoint, new AzureCliCredential());

        // Define the tool using the metadata of your SendEmail function
        var sendEmailTool = new FunctionToolDefinition(
            name: nameof(SendEmail),
            description: "Sends an email with itemized expenses",
            parameters: BinaryData.FromObjectAsJson(new
            {
                type = "object",
                properties = new
                {
                    to = new { type = "string", description = "Who to send the email to" },
                    subject = new { type = "string", description = "The subject of the email" },
                    body = new { type = "string", description = "The text body of the email" }
                },
                required = new[] { "to", "subject", "body" }
            })
        );

        PersistentAgent agent = await persistentAgentsClient.Administration.CreateAgentAsync(
            model: model,
            name: AgentName,
            instructions: AgentInstructions,
            tools: new List<ToolDefinition> { sendEmailTool });


        // --- Create single thread (preserve context across messages)
        PersistentAgentThread thread = persistentAgentsClient.Threads.CreateThread();

        Console.WriteLine("Expenses claim submission Agent (type 'exit' to quit)");
        Console.WriteLine("------------------------------------");

        var printedMessageIds = new HashSet<string>();

        while (true)
        {
            Console.WriteLine("Process prompt: ");
            Console.ReadLine();

            // Add user message to thread
            persistentAgentsClient.Messages.CreateMessage(
                threadId: thread.Id,
                role: MessageRole.User,
                content: prompt
            );

            // Run the agent (object overload avoids assistantId/agentId naming differences)
            ThreadRun threadRun = persistentAgentsClient.Runs.CreateRun(
                thread: thread,
                agent: agent
            );

            // Poll and handle tool-calls (with de-duplication)
            WaitForRunCompletion(persistentAgentsClient, thread, ref threadRun);

            // Print only new assistant messages
            foreach (var m in persistentAgentsClient.Messages.GetMessages(thread.Id, order: ListSortOrder.Ascending))
            {
                if (m.Role != MessageRole.Agent || printedMessageIds.Contains(m.Id)) continue;

                foreach (var part in m.ContentItems)
                {
                    if (part is MessageTextContent t) Console.WriteLine($"\nAssistant: {t.Text}");
                    else Console.WriteLine($"\nAssistant: {part}");
                }
                printedMessageIds.Add(m.Id);
            }
        }

        Console.WriteLine("App finished!");
    }


    static void WaitForRunCompletion(PersistentAgentsClient client, PersistentAgentThread thread, ref ThreadRun run)
    {
        while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress || run.Status == RunStatus.RequiresAction)
        {
            if (run.Status == RunStatus.RequiresAction)
            {
                var pending = GetPendingFunctionCalls(run)
                    .Where(c => !AlreadySubmitted(c.Id, c.Id))
                    .ToList();

                if (pending.Count > 0)
                {
                    var outputs = new List<ToolOutput>(pending.Count);
                    foreach (var call in pending)
                    {
                        var resultJson = HandleFunction(call.Name, call.Arguments.ToString());
                        outputs.Add(new ToolOutput(call.Id, resultJson));
                    }

                    client.Runs.SubmitToolOutputsToRun(thread.Id, run.Id, outputs);
                    MarkSubmitted(run.Id, pending.Select(c => c.Id));
                }
            }

            Thread.Sleep(300);
            run = client.Runs.GetRun(thread.Id, run.Id);
        }

        if (run.Status != RunStatus.Completed && run.LastError is not null)
            Console.WriteLine($"\n[Run Error] {run.LastError.Message}");
    }


    static IEnumerable<RequiredFunctionToolCall> GetPendingFunctionCalls(ThreadRun run)
    {
        // Normalize current + legacy SDK shapes into one unique list
        var seen = new HashSet<string>();

        if (run.RequiredAction is SubmitToolOutputsAction submit && submit.ToolCalls is not null)
        {
            foreach (var call in submit.ToolCalls.OfType<RequiredFunctionToolCall>())
                if (seen.Add(call.Id)) yield return call;
        }

        if (run.RequiredActions is not null)
        {
            foreach (var call in run.RequiredActions) // older previews
                if (seen.Add(call.Id)) yield return call;
        }
    }

    static bool AlreadySubmitted(string runId, string callId)
        => SubmittedByRun.TryGetValue(runId, out var set) && set.Contains(callId);

    static void MarkSubmitted(string runId, IEnumerable<string> callIds)
    {
        var set = SubmittedByRun.GetOrAdd(runId, _ => new HashSet<string>());
        foreach (var id in callIds) set.Add(id);
    }

    static string HandleFunction(string name, string argsJson)
    {
        string response = string.Empty;

        switch (name)
        {
            case nameof(SendEmail):
                var args = JsonDocument.Parse(argsJson).RootElement;
                string to = args.GetProperty("to").GetString()!;
                string subject = args.TryGetProperty("subject", out var c) ? c.GetString() ?? "" : "";
                string body = args.TryGetProperty("body", out var p) ? p.GetString() ?? "" : "";
                response  = SendEmail(to, subject, body);
                break;
            default:
                response = JsonSerializer.Serialize(new { error = $"Unknown tool: {name}" });
                break;
        }

        return response;
    }

    /// <summary>
    /// Sends an email with the specified parameters
    /// </summary>
    /// <param name="to">Who to send the email to</param>
    /// <param name="subject">The subject of the email</param>
    /// <param name="body">The text body of the email</param>
    [Description("Sends an email to a recipient")]
    static string SendEmail(
        [Description("Who to send the email to")] string to,
        [Description("The subject of the email")] string subject,
        [Description("The text body of the email")] string body)
    {
        string response = $"\nTo: {to} Subject: {subject} {body}\n";

        Console.WriteLine($"\nTo: {to}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"{body}\n");

        return response;
    }
}