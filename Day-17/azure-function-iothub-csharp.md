# Procedure: Connecting Azure Function App (C#) to Azure IoT Hub

## Overview

This procedure covers two primary integration patterns between Azure Functions and Azure IoT Hub:

1. **Receiving messages FROM IoT Hub** (Device-to-Cloud): Using Event Hub Trigger
2. **Sending messages TO IoT Hub devices** (Cloud-to-Device): Using IoT Hub Service SDK

---

## Prerequisites

Before you begin, ensure you have:

1. **An active Azure subscription**
2. **An IoT Hub instance** in your Azure subscription
3. **Visual Studio 2022** or later (or Visual Studio Code with Azure Functions extension)
4. **Azure Functions Core Tools** (for local development)
5. **.NET 6.0 or later** installed
6. **At least one registered device** in your IoT Hub (for testing)

---

## Part 1: Receiving Messages from IoT Hub (Device-to-Cloud)

Azure Functions can process messages sent from IoT devices using the Event Hub trigger, as IoT Hub exposes an Event Hub-compatible endpoint.

### Step 1: Create a New Azure Function Project

1. Open **Visual Studio**
2. Create a new project
3. Select **Azure Functions** template
4. Configure the project:
   - **Project name**: IoTHubFunctionApp (or your preferred name)
   - **Location**: Choose your desired location
5. Click **Create**

### Step 2: Configure the Function

1. In the Azure Functions template dialog:
   - **Functions worker**: .NET 8.0 Isolated (Long Term Support)
   - **Function template**: Event Hub trigger
   - **Connection string setting name**: `EventHubConnection` (or your preferred name)
   - **Event Hub name**: Leave as `%EventHubName%` for now

2. Click **Create**

### Step 3: Install Required NuGet Packages

1. Right-click on the project in Solution Explorer
2. Select **Manage NuGet Packages**
3. Install the following packages:
   - `Microsoft.Azure.Functions.Worker` (should already be installed)
   - `Microsoft.Azure.Functions.Worker.Extensions.EventHubs` (version 5.x or later)

### Step 4: Obtain IoT Hub Event Hub-Compatible Connection String

1. Log into the [Azure Portal](https://portal.azure.com)
2. Navigate to your **IoT Hub**
3. In the left menu, select **Built-in endpoints** under the "Hub settings" section
4. Copy the following information:
   - **Event Hub-compatible endpoint** (connection string)
   - **Event Hub-compatible name**
5. Keep these values ready for configuration

### Step 5: Configure Local Settings

1. Open the `local.settings.json` file in your project
2. Add your connection settings:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "EventHubConnection": "Endpoint=sb://your-iothub.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=your-key;EntityPath=your-event-hub-name",
    "EventHubName": "your-event-hub-compatible-name"
  }
}
```

**Important**: Remove the `EntityPath=...` portion from the connection string if it's included, as the Event Hub name is specified separately.

### Step 6: Implement the Function Code

Replace the generated function code with the following:

```csharp
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace IoTHubFunctionApp
{
    public class IoTHubTriggerFunction
    {
        private readonly ILogger<IoTHubTriggerFunction> _logger;

        public IoTHubTriggerFunction(ILogger<IoTHubTriggerFunction> logger)
        {
            _logger = logger;
        }

        [Function("IoTHubTriggerFunction")]
        public void Run(
            [EventHubTrigger("%EventHubName%", Connection = "EventHubConnection")] 
            string[] messages)
        {
            foreach (string message in messages)
            {
                _logger.LogInformation($"IoT Hub message received: {message}");
                
                // Process your message here
                // You can deserialize JSON, store in database, etc.
            }
        }
    }
}
```

### Step 7: Test Locally

1. Press **F5** to start debugging
2. Send test messages from your IoT device or use Azure IoT Explorer
3. Verify messages appear in the console output

### Step 8: Deploy to Azure

1. Right-click on the project in Solution Explorer
2. Select **Publish**
3. Choose **Azure** as the target
4. Select **Azure Function App (Windows)** or (Linux)
5. Select your existing Function App or create a new one
6. Click **Publish**

### Step 9: Configure Application Settings in Azure

1. In the Azure Portal, navigate to your Function App
2. Select **Configuration** under "Settings"
3. Add the following application settings:
   - **Name**: `EventHubConnection`
   - **Value**: Your Event Hub-compatible connection string
   - **Name**: `EventHubName`
   - **Value**: Your Event Hub-compatible name
4. Click **Save**

---

## Part 2: Sending Messages to IoT Hub Devices (Cloud-to-Device)

Azure Functions can send cloud-to-device messages using the IoT Hub Service SDK.

### Step 1: Create HTTP Trigger Function

1. Right-click on your project
2. Select **Add > New Azure Function**
3. Name it `SendMessageToDevice`
4. Select **HTTP trigger** template
5. Set **Authorization level** to **Function** or **Anonymous** (for testing)
6. Click **Add**

### Step 2: Install IoT Hub Service SDK

1. Right-click on the project
2. Select **Manage NuGet Packages**
3. Search for and install:
   - `Microsoft.Azure.Devices` (latest version)

### Step 3: Obtain IoT Hub Service Connection String

1. In the Azure Portal, navigate to your **IoT Hub**
2. Select **Shared access policies** under "Security settings"
3. Click on **iothubowner** (or **service** for production)
4. Copy the **Primary connection string**

### Step 4: Add Connection String to Settings

Add to your `local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "IoTHubConnectionString": "HostName=your-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=your-key"
  }
}
```

### Step 5: Implement Cloud-to-Device Function

Replace the generated code with:

```csharp
using System.Net;
using System.Text;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IoTHubFunctionApp
{
    public class SendMessageToDevice
    {
        private readonly ILogger<SendMessageToDevice> _logger;
        private static ServiceClient? _serviceClient;

        public SendMessageToDevice(ILogger<SendMessageToDevice> logger)
        {
            _logger = logger;
        }

        [Function("SendMessageToDevice")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Processing cloud-to-device message request");

            // Initialize ServiceClient (reuse instance)
            if (_serviceClient == null)
            {
                string connectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString");
                _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            }

            // Parse request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            
            string deviceId = data?.deviceId;
            string messageText = data?.message;

            if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(messageText))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Please provide both deviceId and message in the request body");
                return badResponse;
            }

            try
            {
                // Create and send message
                var commandMessage = new Message(Encoding.UTF8.GetBytes(messageText));
                commandMessage.MessageId = Guid.NewGuid().ToString();
                commandMessage.Ack = DeliveryAcknowledgement.Full;

                // Optional: Add custom properties
                commandMessage.Properties.Add("MessageType", "Command");

                await _serviceClient.SendAsync(deviceId, commandMessage);

                _logger.LogInformation($"Message sent to device {deviceId}");

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Message sent successfully to device {deviceId}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message: {ex.Message}");
                
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error: {ex.Message}");
                return errorResponse;
            }
        }
    }
}
```

### Step 6: Create Request Model (Optional)

For better structure, create a models folder with a request class:

```csharp
namespace IoTHubFunctionApp.Models
{
    public class CloudToDeviceRequest
    {
        public string DeviceId { get; set; }
        public string Message { get; set; }
    }
}
```

### Step 7: Test the Function Locally

1. Press **F5** to start the Function App
2. Note the HTTP endpoint URL (e.g., `http://localhost:7071/api/SendMessageToDevice`)
3. Use a tool like Postman or curl to send a POST request:

```json
POST http://localhost:7071/api/SendMessageToDevice
Content-Type: application/json

{
  "deviceId": "your-device-id",
  "message": "Hello from Azure Function"
}
```

4. Verify the message is received on your IoT device

### Step 8: Deploy and Configure in Azure

1. Deploy the function using the **Publish** option
2. In the Azure Portal, add the `IoTHubConnectionString` to Application Settings
3. Test using the Azure Function's URL: `https://your-function-app.azurewebsites.net/api/SendMessageToDevice`

---

## Part 3: Advanced Scenarios

### Scenario A: Using Device Registry Manager

To check if a device exists or get device information:

```csharp
using Microsoft.Azure.Devices;

public class DeviceManagementFunction
{
    private static RegistryManager? _registryManager;

    [Function("GetDeviceInfo")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        if (_registryManager == null)
        {
            string connectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString");
            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        string deviceId = req.Query["deviceId"];

        try
        {
            var device = await _registryManager.GetDeviceAsync(deviceId);
            
            if (device != null)
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new 
                { 
                    deviceId = device.Id,
                    connectionState = device.ConnectionState.ToString(),
                    status = device.Status.ToString()
                });
                return response;
            }
            else
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync($"Device {deviceId} not found");
                return notFound;
            }
        }
        catch (Exception ex)
        {
            var error = req.CreateResponse(HttpStatusCode.InternalServerError);
            await error.WriteStringAsync($"Error: {ex.Message}");
            return error;
        }
    }
}
```

### Scenario B: Processing Messages with Custom Logic

Enhance the Event Hub trigger to parse and route messages:

```csharp
[Function("ProcessIoTMessages")]
public async Task Run(
    [EventHubTrigger("%EventHubName%", Connection = "EventHubConnection")] 
    string[] messages,
    FunctionContext context)
{
    foreach (string message in messages)
    {
        try
        {
            // Deserialize message
            var telemetry = JsonConvert.DeserializeObject<DeviceTelemetry>(message);
            
            // Process based on message type or device
            if (telemetry.Temperature > 30)
            {
                _logger.LogWarning($"High temperature alert: {telemetry.Temperature}°C from device {telemetry.DeviceId}");
                // Trigger alert, send notification, etc.
            }
            
            // Store in database, send to another service, etc.
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing message: {ex.Message}");
        }
    }
}

public class DeviceTelemetry
{
    public string DeviceId { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public DateTime Timestamp { get; set; }
}
```

---

## Troubleshooting

### Issue: Function not receiving messages

**Solution**:
- Verify the Event Hub-compatible connection string is correct
- Ensure the Event Hub name matches your IoT Hub's built-in endpoint
- Check that devices are sending messages to IoT Hub
- Verify the function app has proper permissions

### Issue: Cannot send messages to devices

**Solution**:
- Verify IoT Hub service connection string is correct
- Ensure the device ID exists in IoT Hub
- Check that the device is connected and listening for messages
- Verify the connection string has appropriate permissions (service or iothubowner)

### Issue: Connection string format errors

**Solution**:
- For Event Hub trigger: Do NOT include `EntityPath` in the connection string
- For Service Client: Use the full IoT Hub connection string
- Ensure no extra spaces or characters in the connection strings

### Issue: High latency or missed messages

**Solution**:
- Consider using a consumer group for your function (default is `$Default`)
- Increase function app scaling settings
- Monitor Event Hub metrics for throttling

---

## Security Best Practices

1. **Use Managed Identity** (for production):
   - Enable managed identity on your Function App
   - Grant appropriate RBAC roles to IoT Hub
   - Use identity-based connections instead of connection strings

2. **Store Secrets Securely**:
   - Use Azure Key Vault for connection strings
   - Reference Key Vault secrets in application settings
   - Never commit connection strings to source control

3. **Least Privilege Access**:
   - Use **service** policy instead of **iothubowner** for cloud-to-device messages
   - Use separate connection strings for different operations
   - Limit function authorization levels appropriately

4. **Monitor and Log**:
   - Enable Application Insights
   - Monitor function execution and failures
   - Set up alerts for anomalies

---

## Additional Resources

- [Azure IoT Hub Documentation](https://learn.microsoft.com/en-us/azure/iot-hub/)
- [Azure Functions Event Hub Trigger](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-event-iot-trigger)
- [IoT Hub Service SDK for .NET](https://learn.microsoft.com/en-us/dotnet/api/microsoft.azure.devices)
- [Cloud-to-Device Messaging Guide](https://learn.microsoft.com/en-us/azure/iot-hub/how-to-cloud-to-device-messaging)