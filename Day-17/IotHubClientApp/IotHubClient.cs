using System;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace IotHubClientApp;
public class IotHubClient
{
    private readonly ILogger<IotHubClient> _logger;

    public IotHubClient(ILogger<IotHubClient> logger)
    {
        _logger = logger;
    }

    /*
    Notes about the EventHubConnection connection string.
    if your Event Hub is part of an IoT Hub, use the IoT Hub's Event Hub-compatible connection string
    (found in Azure Portal under IoT Hub => Built-in endpoints => Event Hub-compatible endpoint).
    Device id in Iot Hub is calledbeach-temp-station01
    */

    [Function(nameof(IotHubClient))]
    //public void Run([EventHubTrigger("messages/events", Connection = "EventHubConnection")] EventData[] events)
    public void Run([EventHubTrigger("beach-temp-station01", Connection = "EventHubConnection")] EventData[] events)
    {
        foreach (EventData @event in events)
        {
            _logger.LogInformation("Event Body: {body}", @event.Body);
            _logger.LogInformation("Event Content-Type: {contentType}", @event.ContentType);
        }

        /*
        [2025-12-22T19:36:08.609Z] Trigger Details: PartionId: 0, Offset: 4294967296-4294967296, EnqueueTimeUtc: 2025-12-22T19:36:07.9100000+00:00-2025-12-22T19:36:07.9100000+00:00, SequenceNumber: 142-142, Count: 1
        [2025-12-22T19:36:08.763Z] Event Body: System.ReadOnlyMemory<Byte>[114]
        [2025-12-22T19:36:08.766Z] Event Content-Type: (null)
        */

    }
}