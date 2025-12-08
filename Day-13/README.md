# Challenge 13: The Yule Lads

![Challenge 13: The Yule Lads](https://res.cloudinary.com/jen-looper/image/upload/v1575988577/images/challenge-13_o3hrxg.jpg)

## The Challenge

Here in Iceland, the holidays are full of tricksy traditions. For the thirteen days leading up to Christmas, it's said that children are visited each night by one of the thirteen Yule Lads (Jólasveinar), a motley crew of trolls with their own distinct personalities. Kertasníkir, the Candle-Stealer, follows children and steals their candles, whereas Þvörusleikir, the Spoon-Licker, loves to steal wooden spoons and lick the food off of them. And the big scary Christmas cat Jólakötturinn will devour any children who haven't gotten a new piece of clothing as a Christmas gift!

Each night, children put out their shoes by the window. When that night's visiting Yule Lad shows up, they leave gifts in the shoes of nice girls and boys, and rotting potatoes for the naughty ones.

This year, the thirteen tricksters are ready to get a bit more tech-savvy! They'd like to use machine learning to generate jokes they can say to children. They have a big book of jokes to train a machine learning model to generate a new joke for them every time they need one. Use this [sample dataset](https://raw.githubusercontent.com/simonaco/25daysofserverless/master/jokes.txt) extracted from [icanhazdadjoke](https://icanhazdadjoke.com/api) api as a blob trigger to create a troll joke generator for our tricksters.

## The Solution

# ?? JokeContextCreatorApp

> An Azure Function that transforms joke datasets into AI-ready conversation contexts for generating authentic jokes

## ?? Background

This project was created for a festive assignment inspired by Icelandic Yule Lad traditions. The thirteen trickster trolls wanted to modernize their joke-telling by using machine learning to generate new jokes based on their big book of traditional material.

## ?? Overview

**JokeContextCreatorApp** is a blob-triggered Azure Function that automatically converts joke datasets into structured conversation contexts suitable for AI model training. When you upload a `jokes.txt` file to Azure Blob Storage, the function processes it and stores a formatted conversation context in Cosmos DB.

### How It Works

1. **Upload** ? Upload `jokes.txt` to the `jokes-container` in Azure Blob Storage
2. **Process** ? Blob trigger activates the function
3. **Transform** ? Jokes are converted into a conversation format with system prompts
4. **Store** ? The structured context is saved to Cosmos DB via output binding

## ??? Architecture

```
Azure Blob Storage (jokes-container)
        ? (trigger)
JokeContextCreatorApp Function
        ? (output binding)
Cosmos DB (AiContextDB/AiContextContainer)
```

## ?? Prerequisites

- Azure Subscription
- Visual Studio 2022
- .NET 8.0 SDK
- Azure Storage Account
- Azure Cosmos DB Account
- Azure Functions Core Tools

## ?? Getting Started

### 1. Create Azure Resources

#### Storage Account Setup
```bash
# Create storage account named 'jokesstorage'
# Create container named 'jokes-container'
```

Navigate to your storage account ? **Access keys** ? Copy the connection string

#### Cosmos DB Setup
```bash
# Create Cosmos DB account named 'jokes-context-storage'
# Create database named 'AiContextDB'
# Create container named 'AiContextContainer'
```

Navigate to your Cosmos DB account ? **Keys** ? Copy the connection string

### 2. Local Development Configuration

Create a `local.settings.json` file:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "DefaultEndpointsProtocol=https;AccountName=jokesstorage;AccountKey=<YOUR_KEY>;EndpointSuffix=core.windows.net",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDbConnectionString": "AccountEndpoint=https://jokes-context-storage.documents.azure.com:443/;AccountKey=<YOUR_KEY>;",
    "APPINSIGHTS_INSTRUMENTATIONKEY": "<YOUR_KEY>"
  }
}
```

### 3. Install NuGet Package

```bash
dotnet add package Microsoft.Azure.Functions.Worker.Extensions.CosmosDB
```

### 4. Configure Application Insights Logging

Update your `Program.cs` to enable all log levels:

```csharp
.ConfigureFunctionsApplicationInsights()
.Configure<LoggerFilterOptions>(options =>
{
    // Remove default Application Insights filter
    LoggerFilterRule defaultRule = options.Rules.FirstOrDefault(rule => 
        rule.ProviderName == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
    
    if (defaultRule is not null)
    {
        options.Rules.Remove(defaultRule);
    }
});
```

> ?? **Tip**: Find your Application Insights Instrumentation Key in the Azure Portal ? Application Insights ? Overview tab

## ?? Deployment to Azure

### 1. Create Function App

- Create an Azure Function App on the **Consumption plan**
- Enable **Basic authentication** for direct publishing from Visual Studio

### 2. Publish from Visual Studio

1. Right-click the project ? **Publish**
2. Download publish profile from Azure Portal (**Get publish profile**)
3. Import the profile in Visual Studio
4. Publish the application

### 3. Configure Environment Variables

Add the following to your Function App ? **Configuration** ? **Application settings**:

```
AzureWebJobsStorage: <your_storage_connection_string>
CosmosDbConnectionString: <your_cosmos_connection_string>
```

## ?? Managed Identity Configuration

For production environments, use Managed Identity instead of connection strings:

### Configure Cosmos DB

```bash
# Enable System Managed Identity in Function App (Identity tab)

# Get Cosmos DB scope
az cosmosdb show --name jokes-context-storage \
  --resource-group DAY13-RG \
  --query id --output tsv

# Assign Cosmos DB Built-in Data Contributor role
az cosmosdb sql role assignment create \
  --resource-group DAY13-RG \
  --account-name jokes-context-storage \
  --role-definition-name "Cosmos DB Built-in Data Contributor" \
  --principal-id <function-app-managed-identity-object-id> \
  --scope <cosmos-db-scope>
```

Update environment variables:
```
CosmosDbConnectionString__accountEndpoint: https://jokes-context-storage.documents.azure.com:443/
CosmosDbConnectionString__credential: managedidentity
```

### Configure Storage Account

Assign roles to the managed identity:
- Storage Blob Data Owner
- Storage Queue Data Contributor
- Storage Account Contributor

Update environment variables:
```
AzureWebJobsStorage__accountName: jokesstorage
AzureWebJobsStorage__blobServiceUri: https://jokesstorage.blob.core.windows.net/
AzureWebJobsStorage__credential: managedIdentity
```

### Verify Role Assignment

```bash
az cosmosdb sql role assignment list \
  --resource-group DAY13-RG \
  --account-name jokes-context-storage
```

## ?? Output Format

The function generates a conversation context structured for AI model consumption:

```json
{
  "id": "b7d9c168-f1b8-4548-bdf7-4ae89341ae9f",
  "timestamp": "2025-12-07T13:58:53.9176558Z",
  "messages": [
    {
      "role": "system",
      "content": "You are a comedian who tells authentic jokes that are funny. You don't repeat the jokes you already have told!"
    },
    {
      "role": "user",
      "content": "Tell me a joke."
    },
    {
      "role": "assistant",
      "content": "Parallel lines have so much in common. It's a shame they'll never meet."
    },
    {
      "role": "user",
      "content": "Tell me a joke."
    }
  ]
}
```

## ?? Monitoring

View logs in Application Insights:

1. Navigate to your Application Insights resource
2. Go to **Logs** or **Transaction search**
3. Monitor function executions and troubleshoot issues

## ??? Key Features

- ? **Blob Trigger**: Automatically processes uploaded joke files
- ? **Output Binding**: Seamlessly stores data in Cosmos DB
- ? **Managed Identity**: Secure authentication without connection strings
- ? **Application Insights**: Full logging and monitoring
- ? **Structured Output**: AI-ready conversation format

## ?? Usage Example

1. Create a `jokes.txt` file with jokes (one per line):
```
Why don't scientists trust atoms? Because they make up everything!
Parallel lines have so much in common. It's a shame they'll never meet.
Why did the scarecrow win an award? He was outstanding in his field!
```

2. Upload to the `jokes-container` in your storage account

3. The function automatically processes the file and stores the context in Cosmos DB

4. Use the stored context to train or prompt an AI model for joke generation

## ?? Fun Fact

This project helps the thirteen Icelandic Yule Lads (Jólasveinar) modernize their holiday mischief with AI-generated jokes! From Kertasníkir (Candle-Stealer) to Þvörusleikir (Spoon-Licker), these trickster trolls can now delight children with both traditional and newly generated jokes during the thirteen nights before Christmas.

## ?? Resources

- [Azure Functions Documentation](https://docs.microsoft.com/azure/azure-functions/)
- [Cosmos DB Output Binding](https://docs.microsoft.com/azure/azure-functions/functions-bindings-cosmosdb-v2-output)
- [Managed Identity Configuration](https://docs.microsoft.com/azure/app-service/overview-managed-identity)
- [Application Insights Logging](https://docs.microsoft.com/azure/azure-monitor/app/worker-service)

## ?? License

This project was created as part of a learning assignment.

---

Made with ?? for the Icelandic Yule Lads