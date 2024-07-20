using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SentimentAnalyzerApp.Services;

namespace SentimentAnalyzerApp
{
    public class SentimentAnalyzer
    {
        private readonly ILogger<SentimentAnalyzer> _logger;
        private readonly ITextTranslationService _textTranslationService;

        public SentimentAnalyzer(ILogger<SentimentAnalyzer> logger, ITextTranslationService textTranslationService)
        {
            _logger = logger;
            _textTranslationService = textTranslationService;
        }

        [Function("AnalyseLetter")]
        public async Task<IActionResult> AnalyseLetter([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string text = await _textTranslationService.TranslateTextToEnglish("mina föräldrar är verkligen inte bra på tekniska saker");

            object[] body = new object[] { new { Response = text } };
            var response = JsonConvert.SerializeObject(body);


            _logger.LogInformation($"Translation: {text}");

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
