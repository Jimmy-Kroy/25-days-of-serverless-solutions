using GitHubWebhookHandlerApp.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubWebhookHandlerApp.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly ILogger<GitHubService> _logger;
        private readonly HttpClient _httpClient;
        public GitHubService(ILogger<GitHubService> logger, IHttpClientFactory httpClientFactory, string token, string userAgent)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            /* Set Github token and User Agent in Authorization header. */
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Token {token}");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", $"{userAgent}");
        }

        public bool TryParse(string jsonRequest, ref Issue issue)
        {
            bool IsSuccess = false;
            dynamic? data = null;
            string sender = string.Empty;
            string action = string.Empty;
            string replyUrl = string.Empty;

            IsSuccess = IsValid(jsonRequest);

            if (IsSuccess)
            {
                data = JsonConvert.DeserializeObject(jsonRequest);
                IsSuccess = (data != null) &&
                    (data.sender != null) &&
                    (data.issue != null);
            }

            if (IsSuccess)
            {
                sender = data.sender.login;
                action = data.action;
                replyUrl = data.issue.comments_url;
                IsSuccess = !string.IsNullOrEmpty(sender) &&
                    !string.IsNullOrEmpty(action) &&
                    !string.IsNullOrEmpty(replyUrl);
            }

            if (IsSuccess)
            {
                issue.Sender = sender;
                issue.ReplyUri = new Uri(replyUrl);
                issue.Action = action;
            }

            return IsSuccess;
        }

        public async Task<bool> SendThankYouAsync(Issue issue)
        {
            bool IsSuccess = false;

            string message = $"Ho Ho Merry Christmas! Dear {issue.Sender} :), thanks a lot for opening this issue. 🚀";

            using (_httpClient)
            using (var request = new HttpRequestMessage())
            {
                request.RequestUri = issue.ReplyUri;
                request.Method = HttpMethod.Post;

                string requestBody = JsonConvert.SerializeObject(new
                {
                    body = message
                });

                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                _logger.LogInformation($"StatusCode[{response.StatusCode}], IsSuccessStatusCode[{response.IsSuccessStatusCode}]");
                IsSuccess = response.IsSuccessStatusCode;
                string content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Http response content: {content}");
            }

            return IsSuccess;
        }

        private bool IsValid(string jsonString)
        {
            try
            {
                JsonDocument.Parse(jsonString);
                return true;
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogError($"Error while parsing GitHub Json request: {ex.ToString()}");
                return false;
            }
        }
    }

}
