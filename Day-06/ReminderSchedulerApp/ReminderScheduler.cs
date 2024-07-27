using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReminderSchedulerApp.Models;
using ReminderSchedulerApp.Services;

namespace ReminderSchedulerApp
{
    public class ReminderScheduler
    {
        private readonly ILogger<ReminderScheduler> _logger;
        private readonly ISlackClient _slackClient;
        private readonly IChronoService _chronoService;
        public ReminderScheduler(ILogger<ReminderScheduler> logger, ISlackClient slackClient, IChronoService chronoService)
        {
            _logger = logger;
            _slackClient = slackClient;
            _chronoService = chronoService;
        }

        [Function(nameof(ReminderScheduler))]
        /* Orchestrations do NOT have direct access to DI. https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-isolated-overview */
        public async Task<bool> RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context) 
        {
            bool IsSuccess = true;
            DateTime? timestamp = null;
            Reminder? reminder = null;

            ILogger logger = context.CreateReplaySafeLogger(nameof(ReminderScheduler));
            _logger.LogInformation("SchedulerOrchestrator received a request.");

            reminder = context.GetInput<Reminder>();

            IsSuccess = (reminder != null);

            if (IsSuccess)
            {
                _logger.LogInformation($"Reminder obj: Text[{reminder.Text}], IconEmoji[{reminder.IconEmoji}], Timezone[{reminder.Timezone}], " +
                    $"Status[{reminder.Status}], Time[{reminder.Time}]");

                timestamp = await context.CallActivityAsync<DateTime>(nameof(ExtractTimestamp), reminder);

                IsSuccess = (timestamp != null);
            }

            if (IsSuccess)
            {
                reminder.Time = timestamp ?? default;
                reminder.Status = Reminder.eStatus.Created;

                IsSuccess = await context.CallActivityAsync<bool>(nameof(NotifyUser), reminder);
            }

            if (IsSuccess)
            {
                reminder.Status = Reminder.eStatus.Activated;
                await context.CreateTimer(reminder.Time, CancellationToken.None);
                IsSuccess = await context.CallActivityAsync<bool>(nameof(NotifyUser), reminder);
            }

            return IsSuccess;
        }

        [Function(nameof(ExtractTimestamp))]
        public async Task<DateTime?> ExtractTimestamp([ActivityTrigger] Reminder reminder, FunctionContext executionContext)
        {
            _logger.LogInformation("ExtractTimestamp received a request.");

            DateTime? timestamp = await _chronoService.ExtractTimeStamp(reminder.Text, reminder.Timezone);

            _logger.LogInformation($"Timestamp value from chronoService: [{timestamp}], now[{DateTime.Now}].");

            return timestamp;
        }

        [Function(nameof(NotifyUser))]
        public async Task<bool?> NotifyUser([ActivityTrigger] Reminder reminder, FunctionContext executionContext)
        {
            bool IsSuccess = false;

            _logger.LogInformation("NotifyUser received a request.");

            _logger.LogInformation($"NotifyUser reminder obj rcvd: Text[{reminder.Text}], IconEmoji[{reminder.IconEmoji}], Timezone[{reminder.Timezone}], " +
                $"Status[{reminder.Status}], Time[{reminder.Time}]");

            IsSuccess = await _slackClient.PostAsync(reminder);

            _logger.LogInformation($"_slackClient returned IsSuccess[{IsSuccess}].");

            return IsSuccess;
        }

        [Function(nameof(ScheduleReminder))]
        public async Task<HttpResponseData> ScheduleReminder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            _logger.LogInformation("ScheduleReminder HTTP post trigger function received a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Reminder reminder = JsonConvert.DeserializeObject<Reminder>(requestBody)!;

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(ReminderScheduler), reminder);

            _logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration

            return await client.CreateCheckStatusResponseAsync(req, instanceId);
        }
    }
}
