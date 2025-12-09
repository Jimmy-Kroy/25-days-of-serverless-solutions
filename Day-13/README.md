# Challenge 13: The Yule Lads

![Challenge 13: The Yule Lads](https://res.cloudinary.com/jen-looper/image/upload/v1575988577/images/challenge-13_o3hrxg.jpg)

## The Challenge

Here in Iceland, the holidays are full of tricksy traditions. For the thirteen days leading up to Christmas, it's said that children are visited each night by one of the thirteen Yule Lads (Jólasveinar), a motley crew of trolls with their own distinct personalities. Kertasníkir, the Candle-Stealer, follows children and steals their candles, whereas Þvörusleikir, the Spoon-Licker, loves to steal wooden spoons and lick the food off of them. And the big scary Christmas cat Jólakötturinn will devour any children who haven't gotten a new piece of clothing as a Christmas gift!

Each night, children put out their shoes by the window. When that night's visiting Yule Lad shows up, they leave gifts in the shoes of nice girls and boys, and rotting potatoes for the naughty ones.

This year, the thirteen tricksters are ready to get a bit more tech-savvy! They'd like to use machine learning to generate jokes they can say to children. They have a big book of jokes to train a machine learning model to generate a new joke for them every time they need one. Use this [sample dataset](https://raw.githubusercontent.com/simonaco/25daysofserverless/master/jokes.txt) extracted from [icanhazdadjoke](https://icanhazdadjoke.com/api) api as a blob trigger to create a troll joke generator for our tricksters.

## The Solution

# 🎭 JokeContextCreatorApp

> An Azure Function that transforms joke datasets into AI-ready conversation contexts for generating authentic jokes

[![Azure Functions](https://img.shields.io/badge/Azure-Functions-0078D4?style=flat&logo=azurefunctions&logoColor=white)](https://azure.microsoft.com/en-us/services/functions/)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Cosmos DB](https://img.shields.io/badge/Cosmos-DB-0078D4?style=flat&logo=microsoftazure&logoColor=white)](https://azure.microsoft.com/en-us/services/cosmos-db/)

## 📖 Background

This project was created for a festive assignment inspired by Icelandic Yule Lad traditions. The thirteen trickster trolls wanted to modernize their joke-telling by using machine learning to generate new jokes based on their big book of traditional material.

## 🎯 Overview

**JokeContextCreatorApp** is a blob-triggered Azure Function that automatically converts joke datasets into structured conversation contexts suitable for AI model training. When you upload a `jokes.txt` file to Azure Blob Storage, the function processes it and stores a formatted conversation context in Cosmos DB.

### How It Works

1. **Upload** → Upload `jokes.txt` to the `jokes-container` in Azure Blob Storage
2. **Process** → Blob trigger activates the function
3. **Transform** → Jokes are converted into a conversation format with system prompts
4. **Store** → The structured context is saved to Cosmos DB via output binding

## 🏗️ Architecture

```
Azure Blob Storage (jokes-container)
        ↓ (trigger)
JokeContextCreatorApp Function
        ↓ (output binding)
Cosmos DB (AiContextDB/AiContextContainer)
```

## 📋 Prerequisites

- Azure Subscription
- Visual Studio 2022
- .NET 8.0 SDK
- Azure Storage Account
- Azure Cosmos DB Account
- Azure Functions Core Tools

## 🚀 Getting Started

### 1. Create Azure Resources

#### Storage Account Setup
```bash
# Create storage account named 'jokesstorage'
# Create container named 'jokes-container'
```

Navigate to your storage account → **Access keys** → Copy the connection string

#### Cosmos DB Setup
```bash
# Create Cosmos DB account named 'jokes-context-storage'
# Create database named 'AiContextDB'
# Create container named 'AiContextContainer'
```

Navigate to your Cosmos DB account → **Keys** → Copy the connection string

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

> 💡 **Tip**: Find your Application Insights Instrumentation Key in the Azure Portal → Application Insights → Overview tab

## 📤 Deployment to Azure

### 1. Create Function App

- Create an Azure Function App on the **Consumption plan**
- Enable **Basic authentication** for direct publishing from Visual Studio

### 2. Publish from Visual Studio

1. Right-click the project → **Publish**
2. Download publish profile from Azure Portal (**Get publish profile**)
3. Import the profile in Visual Studio
4. Publish the application

### 3. Configure Environment Variables

Add the following to your Function App → **Configuration** → **Application settings**:

```
AzureWebJobsStorage: <your_storage_connection_string>
CosmosDbConnectionString: <your_cosmos_connection_string>
```

## 🔐 Managed Identity Configuration

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

### Verify Role Assignment

```bash
az cosmosdb sql role assignment list \
  --resource-group DAY13-RG \
  --account-name jokes-context-storage
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

## 📊 Output Format

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

## 🔍 Monitoring

View logs in Application Insights:

1. Navigate to your Application Insights resource
2. Go to **Logs** or **Transaction search**
3. Monitor function executions and troubleshoot issues

## 🛠️ Key Features

- ✅ **Blob Trigger**: Automatically processes uploaded joke files
- ✅ **Output Binding**: Seamlessly stores data in Cosmos DB
- ✅ **Managed Identity**: Secure authentication without connection strings
- ✅ **Application Insights**: Full logging and monitoring
- ✅ **Structured Output**: AI-ready conversation format

## 📝 Usage Example

1. Create a `jokes.txt` file with jokes (one per line):
```
Why don't scientists trust atoms? Because they make up everything!
Parallel lines have so much in common. It's a shame they'll never meet.
Why did the scarecrow win an award? He was outstanding in his field!
```

2. Upload to the `jokes-container` in your storage account

3. The function automatically processes the file and stores the context in Cosmos DB

4. Use the stored context to train or prompt an AI model for joke generation

## 🎄 Fun Fact

This project helps the thirteen Icelandic Yule Lads (Jólasveinar) modernize their holiday mischief with AI-generated jokes! From Kertasníkir (Candle-Stealer) to Þvörusleikir (Spoon-Licker), these trickster trolls can now delight children with both traditional and newly generated jokes during the thirteen nights before Christmas.

## 📚 Resources

- [Azure Functions Documentation](https://docs.microsoft.com/azure/azure-functions/)
- [Cosmos DB Output Binding](https://docs.microsoft.com/azure/azure-functions/functions-bindings-cosmosdb-v2-output)
- [Managed Identity Configuration](https://docs.microsoft.com/azure/app-service/overview-managed-identity)
- [Application Insights Logging](https://docs.microsoft.com/azure/azure-monitor/app/worker-service)

## 📄 License

This project was created as part of a learning assignment.

---

Made with 🎭 for the Icelandic Yule Lads

---

# 🎭 JokeOfTheDayApp - Azure Function Documentation

> *An AI-powered joke generator for Iceland's Yule Lads, bringing machine learning magic to holiday traditions*

---

## 📖 Overview

The **JokeOfTheDayApp** is an Azure Function that generates jokes using Azure AI Foundry. Inspired by Iceland's thirteen Yule Lads (Jólasveinar), this application combines traditional holiday folklore with modern AI technology to create unique jokes on demand.

### 🎯 How It Works

1. **HTTP Trigger** - The function is invoked via HTTP GET or POST request
2. **Context Retrieval** - Fetches the most recent joke context from Azure Cosmos DB using an input binding
3. **AI Generation** - Passes the context to an OpenAI model in Azure AI Foundry
4. **Response** - Returns a freshly generated joke based on the learned style and context

---

## 🏗️ Architecture

```
HTTP Request → Azure Function → Cosmos DB (Context) → Azure AI Foundry → Generated Joke
```

### Core Components

- **Azure Function App** - HTTP-triggered serverless function
- **Azure Cosmos DB** - NoSQL database storing joke conversation contexts
- **Azure AI Foundry** - OpenAI model deployment (gpt-5-nano)
- **Managed Identity** - Secure authentication without API keys

---

## 🔧 Local Development Setup

### Prerequisites

- Visual Studio 2022
- Azure Functions Core Tools
- .NET (dotnet-isolated runtime)
- Access to Azure subscription

### Required NuGet Packages

```xml
<PackageReference Include="Azure.AI.OpenAI" Version="2.2.0-beta.4" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.CosmosDB" />
```

### Configuration Steps

#### 1️⃣ **Configure Local Settings**

Create a `local.settings.json` file in your project root:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "JokeEndpoint": "https://day13foundry.cognitiveservices.azure.com/",
    "JokeApiKey": "YOUR_API_KEY_HERE",
    "DeploymentName": "gpt-5-nano",
    "UseApiKey": "true",
    "CosmosDbConnectionString": "YOUR_COSMOS_CONNECTION_STRING"
  }
}
```

#### 2️⃣ **Set Up Cosmos DB Input Binding**

The function uses a Cosmos DB input binding to retrieve joke context:

```csharp
[CosmosDBInput(
    databaseName: "AiContextDB",
    containerName: "AiContextContainer",
    Connection = "CosmosDbConnectionString",
    SqlQuery = "SELECT TOP 1 * FROM c ORDER BY c.timestamp DESC")]
IEnumerable<JokeConversationContext> records
```

#### 3️⃣ **Test Locally**

- Press F5 in Visual Studio 2022
- Navigate to the local endpoint (typically `http://localhost:7071/api/JokeOfTheDay`)
- Verify joke generation works correctly

---

## ☁️ Azure Deployment

### Step 1: Create Azure Resources

#### **Create Function App**

- **Plan Type**: Consumption Plan
- **Runtime**: .NET (dotnet-isolated)
- **Authentication**: Enable Basic Authentication for publishing from Visual Studio

#### **Download Publish Profile**

1. Navigate to your Function App in Azure Portal
2. Click **"Get publish profile"**
3. Save the `.publishsettings` file to your local machine

### Step 2: Publish from Visual Studio

1. Right-click your project in Visual Studio
2. Select **"Publish"**
3. Import the publish profile you downloaded
4. Click **"Publish"** to deploy

### Step 3: Configure Environment Variables

Add the following application settings in Azure Portal:

| Setting Name | Value |
|-------------|-------|
| `JokeEndpoint` | `https://day13foundry.cognitiveservices.azure.com/` |
| `JokeApiKey` | Your Azure AI Foundry API key |
| `DeploymentName` | `gpt-5-nano` |
| `UseApiKey` | `true` |
| `CosmosDbConnectionString` | Your Cosmos DB connection string |

---

## 🔐 Migration to Managed Identity (MSI)

For production deployments, replace API keys with System Managed Identity for enhanced security.

### Azure AI Foundry Configuration

#### **1️⃣ Enable System Assigned Managed Identity**

1. Go to your Function App in Azure Portal
2. Navigate to **Identity** tab
3. Turn **System assigned** status to **On**
4. Save and copy the **Object (principal) ID**

#### **2️⃣ Assign AI Role**

1. Navigate to your **Azure AI Foundry resource** (not the project)
2. Go to **Access Control (IAM)**
3. Click **"Add role assignment"**
4. Search for **"Cognitive Services OpenAI User"**
5. Assign to **Managed Identity** → Select your Function App
6. Click **"Review + assign"**

#### **3️⃣ Update Environment Variable**

Change `UseApiKey` to `false` in Function App settings

### Cosmos DB Configuration

#### **1️⃣ Get Cosmos DB Scope**

Open Azure Cloud Shell and run:

```bash
az cosmosdb show \
  --name jokes-context-storage \
  --resource-group DAY13-RG \
  --query id \
  --output tsv
```

**Output:**
```
/subscriptions/4c136ca6-b580-4aa7-9d63-97bc9a3da353/resourceGroups/DAY13-RG/providers/Microsoft.DocumentDB/databaseAccounts/jokes-context-storage
```

#### **2️⃣ Assign Cosmos DB Reader Role**

```bash
az cosmosdb sql role assignment create \
  --resource-group DAY13-RG \
  --account-name jokes-context-storage \
  --role-definition-name "Cosmos DB Built-in Data Reader" \
  --principal-id 554b53c4-fc2c-4a46-ba80-dd89b63657c7 \
  --scope /subscriptions/4c9b636-ba80-4717-9b63-97bc9a3da353/resourceGroups/DAY13-RG/providers/Microsoft.DocumentDB/databaseAccounts/jokes-context-storage
```

#### **3️⃣ Verify Role Assignment**

```bash
az cosmosdb sql role assignment list \
  --resource-group DAY13-RG \
  --account-name jokes-context-storage
```

#### **4️⃣ Update Connection String Settings**

Replace the `CosmosDbConnectionString` with two new settings:

| Setting Name | Value |
|-------------|-------|
| `CosmosDbConnectionString__accountEndpoint` | `https://jokes-context-storage.documents.azure.com:443/` |
| `CosmosDbConnectionString__credential` | `managedidentity` |

---

## 📊 Data Model

### JokeConversationContext

The Cosmos DB stores conversation contexts with the following structure:

```json
{
  "id": "unique-identifier",
  "timestamp": "2024-12-09T10:30:00Z",
  "messages": [
    {
      "role": "system",
      "content": "You are a funny comedian."
    },
    {
      "role": "user",
      "content": "Tell me a joke."
    },
    {
      "role": "assistant",
      "content": "Why did the Yule Lad cross the road?..."
    }
  ]
}
```

---

## 🎄 The Yule Lads Assignment

This project was created as part of a holiday-themed assignment to modernize Iceland's Yule Lad traditions. The thirteen Yule Lads, each with distinct personalities, wanted to use machine learning to generate jokes for children they visit during the thirteen nights before Christmas.

### The Tradition

- **Kertasníkir** (Candle-Stealer) - Follows children to steal their candles
- **Þvörusleikir** (Spoon-Licker) - Steals wooden spoons to lick food off them
- **Jólakötturinn** (Christmas Cat) - The scary Christmas cat who devours children without new clothes

Each night, children place shoes by the window. Nice children receive gifts, while naughty ones get rotting potatoes!

---

## 🔍 Key Features

✨ **AI-Powered Joke Generation** - Uses Azure OpenAI models for creative content  
🔒 **Secure Authentication** - Supports both API keys and Managed Identity  
📦 **Context-Aware** - Learns from historical joke patterns in Cosmos DB  
⚡ **Serverless Architecture** - Scales automatically with Azure Functions  
🎯 **Simple HTTP API** - Easy integration with any client application  

---

## 🚀 Testing Your Deployment

### Local Testing
```bash
curl http://localhost:7071/api/JokeOfTheDay
```

### Azure Testing
```bash
curl https://your-function-app.azurewebsites.net/api/JokeOfTheDay
```

**Expected Response:**
```json
"The joke of the day: [Your AI-generated joke here]"
```

---

## 📝 Code Reference

The main function signature:

```csharp
[Function("JokeOfTheDay")]
public IActionResult Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
    [CosmosDBInput(
        databaseName: "AiContextDB",
        containerName: "AiContextContainer",
        Connection = "CosmosDbConnectionString",
        SqlQuery = "SELECT TOP 1 * FROM c ORDER BY c.timestamp DESC")] 
    IEnumerable<JokeConversationContext> records)
```

---

## 🎁 Summary

The JokeOfTheDayApp successfully brings Iceland's Yule Lad traditions into the modern age by leveraging Azure's cloud services and AI capabilities. By combining Azure Functions, Cosmos DB, and Azure AI Foundry with secure Managed Identity authentication, we've created a scalable, secure, and festive joke generation service.

**Happy Holidays! 🎄✨**

---

*Created with ❤️ for the thirteen Yule Lads of Iceland*
