/*
    Reference: https://learn.microsoft.com/en-us/agent-framework/user-guide/hosting/agent-to-agent-integration?tabs=dotnet-cli%2Cuser-secrets

    
    The swagger service can be accessed via the url: https://localhost:7114/swagger/index.html
    The weatherforcast API can be accessed via the url: https://localhost:7114/weatherforecast
 */



using A2A.AspNetCore;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

string endpoint = builder.Configuration["AZURE_OPENAI_ENDPOINT"]
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
string deploymentName = builder.Configuration["AZURE_OPENAI_DEPLOYMENT_NAME"]
    ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME is not set.");

Console.WriteLine($"endpoint: {endpoint}, deploymentName: {deploymentName}");

AzureOpenAIClient azureClient = new(
    new Uri(endpoint),
    new AzureCliCredential());

IChatClient chatClient = azureClient.GetChatClient(deploymentName).AsIChatClient();

builder.Services.AddSingleton(chatClient);

// Register an agent
var pirateAgent = builder.AddAIAgent("pirate", instructions: "You are a pirate. Speak like a pirate.");
//Exposing Multiple Agents
var mathAgent = builder.AddAIAgent("math", instructions: "You are a math expert.");
var scienceAgent = builder.AddAIAgent("science", instructions: "You are a science expert.");

//Echo client
var echoAgent = builder.AddAIAgent("Echo", instructions: "You are an agent that echoes back any message you send to it.");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

// Expose the agent via A2A protocol. You can also customize the agentCard
app.MapA2A(pirateAgent, path: "/a2a/pirate", agentCard: new()
{
    Name = "Pirate Agent",
    Description = "An agent that speaks like a pirate.",
    Version = "1.0"
});

app.MapA2A(mathAgent, path: "/a2a/math", agentCard: new()
{
    Name = "Math Agent",
    Description = "An agent that is a Math expert.",
    Version = "1.0"
});

app.MapA2A(scienceAgent, path: "/a2a/science", agentCard: new()
{
    Name = "Science Agent",
    Description = "An agent that is a science expert.",
    Version = "1.0"
});

app.MapA2A(echoAgent, path: "/a2a/echo", agentCard: new()
{
    Name = "Echo Agent",
    Description = "A basic agent that echoes back any message you send to it. Perfect for testing A2A communication.",
    Version = "1.0"
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
