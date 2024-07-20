using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SentimentAnalyzerApp.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentAnalyzerApp.Services
{
    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        private readonly ILogger<SentimentAnalysisService> _logger;
        private readonly TextAnalyticsClientSettings _settings;

        public SentimentAnalysisService(IOptions<TextAnalyticsClientSettings> settings, ILogger<SentimentAnalysisService> logger)
        {
            _logger = logger;
            _settings = settings.Value;
        }

    }
}
