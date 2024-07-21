using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SentimentAnalyzerApp.Models;
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

        [Function("DetermineSentimentOfLetter")]
        public async Task<IActionResult> DetermineSentimentOfLetter([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            bool IsSuccess = false;

            _logger.LogInformation("C# HTTP trigger function processed a request.");

            TranslationResult? translationResult  = await _textTranslationService.TranslateTextToEnglish("mina föräldrar är verkligen inte bra på tekniska saker");
            IsSuccess = (translationResult != null) && (translationResult.Translations.FirstOrDefault() != null) && 
                !string.IsNullOrEmpty(translationResult.Translations.FirstOrDefault().Text);

            if(IsSuccess)
            {
                Console.WriteLine(translationResult.Translations.FirstOrDefault().Text);
            }




            //object[] body = new object[] { new { Response = text } };
            //var response = JsonConvert.SerializeObject(body);
            //_logger.LogInformation($"Translation: {text}");


            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
