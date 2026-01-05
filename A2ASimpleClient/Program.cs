// See https://aka.ms/new-console-template for more information
/// <summary>
/// A simple client that demonstrates how to communicate with A2A agents.
/// This shows the basic patterns for agent discovery and communication.
/// </summary>

/*

References:
https://github.com/mikeas1/a2a-samples/tree/a4479105d4e5f396951640ddeb36f8917b128772/samples/dotnet/BasicA2ADemo

*/

using A2A;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("🤖 Basic A2A .NET Demo Client");
        Console.WriteLine("==============================");
        Console.WriteLine();

        try
        {
            // Demonstrate agent discovery and communication
            await DemonstrateAgentCommunication();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ An error occurred: {ex.Message}");
            Console.WriteLine("💡 Make sure both agent servers are running:");
            Console.WriteLine("   - Echo Agent: http://localhost:5001");
            Console.WriteLine("   - Calculator Agent: http://localhost:5002");
        }

        Console.WriteLine();
        Console.WriteLine("Demo completed! Press any key to exit.");
        Console.ReadKey();
    }

    /// <summary>
    /// Demonstrates the complete workflow of discovering and communicating with agents.
    /// </summary>
    static async Task DemonstrateAgentCommunication()
    {
        // Define our agent endpoints
        var agents = new[]
        {
            //new { Name = "Echo Agent", Url = "http://localhost:5001/" },
            //new { Name = "Calculator Agent", Url = "http://localhost:5002/" }
            new { Name = "My Echo Agent", Url = "http://localhost:5000/" }
        };

        foreach (var agentInfo in agents)
        {
            Console.WriteLine($"🔍 Discovering {agentInfo.Name}...");

            try
            {
                // Step 1: Discover the agent and get its capabilities
                var agentCard = await DiscoverAgent(agentInfo.Url);

                // Step 2: Communicate with the agent
                await CommunicateWithAgent(agentInfo.Name, agentInfo.Url, agentCard);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to communicate with {agentInfo.Name}: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    /// <summary>
    /// Discovers an agent by fetching its agent card.
    /// </summary>
    static async Task<AgentCard> DiscoverAgent(string agentUrl)
    {
        var cardResolver = new A2ACardResolver(new Uri(agentUrl));
        var agentCard = await cardResolver.GetAgentCardAsync();

        Console.WriteLine($"✅ Found agent: {agentCard.Name}");
        Console.WriteLine($"   📝 Description: {agentCard.Description}");
        Console.WriteLine($"   🔢 Version: {agentCard.Version}");
        Console.WriteLine($"   🎯 Capabilities: Streaming = {agentCard.Capabilities?.Streaming}");

        // Show additional agent information
        if (agentCard.DefaultInputModes?.Count > 0)
        {
            Console.WriteLine($"   📥 Input modes: {string.Join(", ", agentCard.DefaultInputModes)}");
        }

        if (agentCard.DefaultOutputModes?.Count > 0)
        {
            Console.WriteLine($"   📤 Output modes: {string.Join(", ", agentCard.DefaultOutputModes)}");
        }

        return agentCard;
    }

    /// <summary>
    /// Demonstrates communication with a discovered agent.
    /// </summary>
    static async Task CommunicateWithAgent(string agentName, string agentUrl, AgentCard agentCard)
    {
        Console.WriteLine($"💬 Communicating with {agentName}...");

        // Create an A2A client for this agent
        var client = new A2AClient(new Uri(agentUrl));

        // Send appropriate messages based on the agent type
        if (agentName.Contains("Echo"))
        {
            await SendEchoMessages(client);
        }
        else if (agentName.Contains("Calculator"))
        {
            await SendCalculatorMessages(client);
        }
    }

    /// <summary>
    /// Sends test messages to the Echo Agent.
    /// </summary>
    static async Task SendEchoMessages(A2AClient client)
    {
        var testMessages = new[]
        {
            "Hello, Echo Agent!",
            "Can you repeat this message?",
            "Testing A2A communication 🚀"
        };

        foreach (var testMessage in testMessages)
        {
            var message = CreateMessage(testMessage);
            Console.WriteLine($"   📤 Sending: {testMessage}");

            var response = await client.SendMessageAsync(new MessageSendParams { Message = message });
            var responseText = GetTextFromMessage((Message)response);

            Console.WriteLine($"   📥 Received: {responseText}");
        }
    }

    /// <summary>
    /// Sends math expressions to the Calculator Agent.
    /// </summary>
    static async Task SendCalculatorMessages(A2AClient client)
    {
        var mathExpressions = new[]
        {
            "5 + 3",
            "10 * 7",
            "15 / 3",
            "20.5 - 5.3"
        };

        foreach (var expression in mathExpressions)
        {
            var message = CreateMessage(expression);
            Console.WriteLine($"   📤 Calculating: {expression}");

            var response = await client.SendMessageAsync(new MessageSendParams { Message = message });
            var responseText = GetTextFromMessage((Message)response);

            Console.WriteLine($"   📥 Result: {responseText}");
        }
    }

    /// <summary>
    /// Creates a message with the specified text.
    /// </summary>
    static Message CreateMessage(string text)
    {
        return new Message
        {
            Role = MessageRole.User,
            MessageId = Guid.NewGuid().ToString(),
            Parts = [new TextPart { Text = text }]
        };
    }

    /// <summary>
    /// Extracts text from a message response.
    /// </summary>
    static string GetTextFromMessage(Message message)
    {
        var textPart = message.Parts.OfType<TextPart>().FirstOrDefault();
        return textPart?.Text ?? "[No text content]";
    }
}