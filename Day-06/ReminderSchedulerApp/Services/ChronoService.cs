using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ReminderSchedulerApp.Configurations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderSchedulerApp.Services
{
    public class ChronoService : IChronoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChronoService> _logger;
        private readonly ChronoServiceSettings _chronoServiceSettings;

        public ChronoService(HttpClient httpClient, IOptions<ChronoServiceSettings> chronoServiceSettings,
            ILogger<ChronoService> logger)
        {
            _httpClient = httpClient;
            _chronoServiceSettings = chronoServiceSettings.Value;
            _logger = logger;
        }

        public async Task<DateTime?> ExtractTimeStamp(string text, string timezone)
        {
            bool IsSuccess = false;
            DateTime zuluDateTime = default;
            string jsonString = string.Empty;
            string zuluTimeString = string.Empty;
            JObject? data = null;
            JToken? jToken = null;

            jsonString = JsonConvert.SerializeObject(new
            {
                text,
                timezone
            });

            _logger.LogInformation($"Json to ChronoServices: {jsonString}");

            StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            if (!string.IsNullOrEmpty(_chronoServiceSettings.Key))
                _httpClient.DefaultRequestHeaders.Add("x-functions-key", _chronoServiceSettings.Key);

            HttpResponseMessage response =
                await _httpClient.PostAsync(_chronoServiceSettings.EndpointUrl, content);

            _logger.LogInformation($"ChronoServices http Post request: response.IsSuccessStatusCode[{response.IsSuccessStatusCode}], " +
                $"response.StatusCode[{response.StatusCode}]");

            IsSuccess = response.IsSuccessStatusCode;

            if (IsSuccess)
            {
                jsonString = await response.Content.ReadAsStringAsync();
                /* Format returned:
                {
                    "text": "Please schedule to make coffee in 2 days.",
                    "timestamp": "2024-07-19T12:43:58.417Z"
                }*/

                _logger.LogInformation($"Json from ChronoServices: {jsonString}");

                data = JObject.Parse(jsonString);
                jToken = data?.SelectToken("timestamp");

                IsSuccess = (jToken != null);
            }

            if (IsSuccess)
            {
                zuluTimeString = jToken.ToString(); //"2024-07-17T21:02:41Z"; example Zulu time string
                _logger.LogInformation($"Zulu time string from ChronoServices: {zuluTimeString}");
                zuluDateTime = DateTime.Parse(zuluTimeString, null, DateTimeStyles.RoundtripKind);
                _logger.LogInformation($"Zulu DateTime obj from ChronoServices: zuluDateTime: [{zuluDateTime}], " +
                    $"zuluDateTime.ToLocalTime()}}: [{zuluDateTime.ToLocalTime()}], Now: [{DateTime.Now}]");
            }

            return IsSuccess == true ? zuluDateTime.ToLocalTime() : null;
        }
    }
}
