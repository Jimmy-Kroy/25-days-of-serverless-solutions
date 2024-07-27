using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReminderSchedulerApp.Configurations;
using ReminderSchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderSchedulerApp.Services
{
    public class SlackClient : ISlackClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SlackClient> _logger;
        private readonly SlackSettings _slackSettings;

        public SlackClient(HttpClient httpClient, IOptions<SlackSettings> slackSettings, ILogger<SlackClient> logger)
        {
            _httpClient = httpClient;
            _slackSettings = slackSettings.Value;
            _logger = logger;
        }

        public async Task<bool> PostAsync(Reminder reminder)
        {
            bool IsSuccess = false;

            StringContent? content = FormatMessage(reminder);

            IsSuccess = (content != null);

            if (IsSuccess)
            {
                HttpResponseMessage response =
                    await _httpClient.PostAsync(_slackSettings.EndpointUrl, content);

                IsSuccess = response.IsSuccessStatusCode;
            }

            return IsSuccess;
        }

        private StringContent? FormatMessage(Reminder reminder)
        {
            bool IsSuccess = false;
            StringContent? content = null;
            string msg = string.Empty;

            if (reminder.Status == Reminder.eStatus.Created)
            {
                msg = $"{reminder.Text} has been scheduled";
            }
            else if (reminder.Status == Reminder.eStatus.Activated)
            {
                msg = $"Your Scheduled {reminder.Text} to happen now";
            }

            IsSuccess = (msg != string.Empty);

            if (IsSuccess)
            {
                string jsonString = JsonConvert.SerializeObject(new
                {
                    text = msg + " " + reminder.IconEmoji
                });

                content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            }

            return IsSuccess == true ? content : null;
        }
    }

}
