using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SentimentAnalyzerApp.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentAnalyzerApp.Services
{
    public class TextTranslationService : ITextTranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TextTranslationService> _logger;
        private readonly TextTranslationClientSettings _settings;
        const string route = "/translate?api-version=3.0&to=en"; //translate everything to english

        public TextTranslationService(IOptions<TextTranslationClientSettings> settings, ILogger<TextTranslationService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _settings = settings.Value;
            _httpClient = httpClient;
        }

        public async Task<string> TranslateTextToEnglish(string text)
        {
            string result = string.Empty;

            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            //using (var client = new HttpClient())
            
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(_settings.EndpointUrl + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _settings.Key);
                // location required if you're using a multi-service or regional (not global) resource.
                request.Headers.Add("Ocp-Apim-Subscription-Region", _settings.AzureRegion);

                // Send the request and get response.
                HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                result = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(result);
            }

            return result;
        }
    }
}
