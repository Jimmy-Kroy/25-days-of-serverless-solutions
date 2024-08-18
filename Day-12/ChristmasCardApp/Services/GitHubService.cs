using ChristmasCardApp.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasCardApp.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly ILogger<GitHubService> _logger;
        private readonly HttpClient _httpClient;
        private readonly GithubSettings _githubSettings;

        public GitHubService(ILogger<GitHubService> logger, IHttpClientFactory httpClientFactory, IOptions<GithubSettings> githubSettings)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _githubSettings = githubSettings.Value;
        }

        public async Task<string?> GetContentAsync(string name, bool convertToHtml)
        {
            bool isSuccess = false;
            string stringResponse = string.Empty;

            string mediaType = convertToHtml ? "application/vnd.github.html+json" : "application/json";
            Uri requestUri = new Uri(_githubSettings.EndpointUrl + name + ".md");

            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.Headers.UserAgent.Add(new ProductInfoHeaderValue("ChristmasCardApp", "1.0"));
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                request.RequestUri = requestUri;

                HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                isSuccess = response.IsSuccessStatusCode;
                _logger.LogInformation($"IsSuccessStatusCode: {response.IsSuccessStatusCode}");

                if (isSuccess)
                {
                    stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    isSuccess = !string.IsNullOrEmpty(stringResponse);
                }

                //Return md file
                if (isSuccess && !convertToHtml)
                {
                    JObject resultObject = JObject.Parse(stringResponse);
                    string contentBase64 = resultObject["content"]?.ToString() ?? string.Empty;
                    stringResponse = Encoding.UTF8.GetString(Convert.FromBase64String(contentBase64));
                }
            }

            return isSuccess ? stringResponse : string.Empty;
        }
    }
}
