using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SentimentAnalyzerApp.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SentimentAnalyzerApp.Services
{
    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        private readonly ILogger<SentimentAnalysisService> _logger;
        private readonly TextAnalyticsClientSettings _settings;
        private readonly TextAnalyticsClient _textAnalyticsClient;

        public SentimentAnalysisService(IOptions<TextAnalyticsClientSettings> settings, ILogger<SentimentAnalysisService> logger)
        {
            _logger = logger;
            _settings = settings.Value;
            _textAnalyticsClient = new TextAnalyticsClient(new Uri(_settings.EndpointUrl), new AzureKeyCredential(_settings.Key));
        }

        public async Task<DocumentSentiment?> AnalyzeSentimentAsync(string text)
        {
            /* Note we can also use Batch analysis to reduce the number of calls to the analyze service: 
            List<string> documents = new List<string>
            {
                "The food and service were unacceptable. The concierge was nice, however.",
                text
            };

            AnalyzeSentimentResultCollection analyzeSentimentResults =
                await _textAnalyticsClient.AnalyzeSentimentBatchAsync(documents); */

            DocumentSentiment? documentSentiment = await _textAnalyticsClient.AnalyzeSentimentAsync(text);
   
            SentimentConfidenceScores scores = documentSentiment.ConfidenceScores;
            _logger.LogInformation($"\tPositive score: {scores.Positive:0.00}");
            _logger.LogInformation($"\tNegative score: {scores.Negative:0.00}");
            _logger.LogInformation($"\tNeutral score: {scores.Neutral:0.00}\n");

            return documentSentiment;
        }
    }
}
