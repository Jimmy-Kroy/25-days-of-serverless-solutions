// See https://aka.ms/new-console-template for more information


using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;

//Get environment values
//Set environment values, enter in search box "edit the system environment variable" 
//Add system environment variable to the UI
//After setting the environment variable, restart VS 
var endpoint = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_FOUNDRY_PROJECT_ENDPOINT is not set.");
var model = "gpt-4o"; // "gpt-5-mini"; // Environment.GetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_MODEL_ID") ?? "gpt-4.1-mini";

Console.WriteLine("App started!");
Console.WriteLine($"endpoint: {endpoint}");
Console.WriteLine($"model: {model}");

const string AgentName = "MicrosoftLearnAgent";
const string AgentInstructions = "You answer questions by searching the Microsoft Learn content only.";

var mcpTool = new MCPToolDefinition(
    serverLabel: "microsoft_learn",
    serverUrl: "https://learn.microsoft.com/api/mcp");
mcpTool.AllowedTools.Add("microsoft_docs_search");


var persistentAgentsClient = new PersistentAgentsClient(endpoint, new AzureCliCredential());

var agentMetadata = await persistentAgentsClient.Administration.CreateAgentAsync(
    model: model,
    name: AgentName,
    instructions: AgentInstructions,
    tools: [mcpTool]);

AIAgent agent = await persistentAgentsClient.GetAIAgentAsync(agentMetadata.Value.Id);


var runOptions = new ChatClientAgentRunOptions()
{
    ChatOptions = new()
    {
        RawRepresentationFactory = (_) => new ThreadAndRunOptions()
        {
            ToolResources = new MCPToolResource(serverLabel: "microsoft_learn")
            {
                RequireApproval = new MCPApproval("never"),
            }.ToToolResources()
        }
    }
};

AgentThread thread = agent.GetNewThread();
var response = await agent.RunAsync(
    "Please summarize the Azure AI Agent documentation related to MCP Tool calling?",
    thread,
    runOptions);
Console.WriteLine(response);

Console.WriteLine("Finished!");


/* The resulting output
App started!
endpoint: https://ai-3026-mcp-example-resource.services.ai.azure.com/api/projects/ai-3026-MCP-example
model: gpt-4o
The Azure AI Agent documentation regarding MCP Tool calling outlines how Azure AI agents can dynamically access Model Context Protocol (MCP)-hosted tools, enhancing their capabilities in applications. Below is a summary of the key points:

1. **Overview of MCP Integration**:
   - MCP (Model Context Protocol) enables applications and large language models to share context and leverage tools dynamically.
   - MCP supports remote servers for cloud-scale management of AI tools.
   - Tools integrated with MCP protocol can be accessed seamlessly during runtime, enhancing AI agents' functionalities, such as invoking APIs or interacting with external databases.

2. **Features and Objectives of MCP Tool Calling**:
   - Agents can use MCP tools to perform tasks like real-time decision-making, connecting with data sources, pulling external information, and executing functions.
   - Tool calling expands an agent's capacity beyond generating textual responses, allowing interaction with external services like APIs or databases.
   - Agents use dynamic configuration to identify and utilize appropriate tools during operations.

3. **Technical Implementation**:
   - Tools are wrapped as asynchronous functions and registered with the AI agent.
   - Hosting MCP tools involves setting up an MCP server, with options for self-hosting or using Azure Functions.
   - Supported programming languages for MCP setup include Python, C#, PowerShell, and JavaScript, enabling tool management and integration with Azure AI Foundry.

4. **Agent Tool Configuration**:
   - In Azure AI Foundry, agents can add MCP tools by specifying a server endpoint, choosing authentication methods, and defining project metadata.
   - Testing the tools involves queries that leverage server capabilities to validate integration.

5. **Prototyping with AI Playground**:
   - Azure AI offers an interactive Playground to prototype tool-calling agents.
   - Various tools, including vector search indices and Unity Catalog functions, can be selected and tested with the AI model to ensure the tools work effectively.

6. **Hosting an MCP Server**:
   - Azure Functions can be used to host MCP servers, offering scalability and ease of deployment.
   - This allows agents to access tools remotely over MCP protocol for enhanced flexibility.

For further learning about integrating MCP tools with Azure AI agents, visit the [official documentation](https://learn.microsoft.com/en-us/training/modules/connect-agent-to-mcp-tools/).
Finished!
 */


