using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SentimentAnalyzerApp.Models;
using SentimentAnalyzerApp.Services;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace SentimentAnalyzerApp
{
    public class SentimentAnalyzer
    {
        private readonly ILogger<SentimentAnalyzer> _logger;
        private readonly ITextTranslationService _textTranslationService;
        private readonly ISentimentAnalysisService _sentimentAnalysisService;
        public SentimentAnalyzer(ILogger<SentimentAnalyzer> logger, ITextTranslationService textTranslationService, 
            ISentimentAnalysisService sentimentAnalysisService)
        {
            _logger = logger;
            _textTranslationService = textTranslationService;
            _sentimentAnalysisService = sentimentAnalysisService;
        }

        [Function("DetermineSentimentOfLetter")]
        public async Task<IActionResult> DetermineSentimentOfLetter([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            bool IsSuccess = false;
            // Create a list of tasks to perform parallel translations.
            List<Task<TranslationResult?>> translationTasks = new List<Task<TranslationResult?>>();
            // Create a list of tasks to perform parallel sentiment analysis.
            List<Task<DocumentSentiment?>> sentimentAnalysisTasks = new List<Task<DocumentSentiment?>> ();


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            IEnumerable<Letter> letters = JsonConvert.DeserializeObject<IEnumerable<Letter>>(requestBody);

            foreach(Letter letter in letters)
            {
                _logger.LogInformation($"\tSender: {letter.Sender}\tText: {letter.Text}\n");

                /* Translate all letters to english. */
                translationTasks.Add(_textTranslationService.TranslateTextToEnglishAsync(letter.Text));
            }

            // Run the tasks in parallel, and wait until all have been run
            IEnumerable<TranslationResult?> translations = await Task.WhenAll(translationTasks);

            IsSuccess = (letters.Count() == translations.Count());

            if(IsSuccess)
            {
                for (int i = 0; i < letters.Count(); i++)
                {
                    _logger.LogInformation($"\tSender: {letters.ElementAt(i).Sender}\tText: {letters.ElementAt(i).Text}\tTranslation: {translations.ElementAt(i).Translations.FirstOrDefault().Text}");
                    letters.ElementAt(i).EnglishTranslation = translations.ElementAt(i);
                    /* Determine sentiment of each letter. */
                    sentimentAnalysisTasks.Add(_sentimentAnalysisService.AnalyzeSentimentAsync(letters.ElementAt(i).EnglishTranslation.Translations.FirstOrDefault().Text));
                }
            }

            // Run the tasks in parallel, and wait until all have been run
            IEnumerable<DocumentSentiment?> sentiments = await Task.WhenAll(sentimentAnalysisTasks);

            IsSuccess = (letters.Count() == sentiments.Count());

            if (IsSuccess)
            {
                for (int i = 0; i < letters.Count(); i++)
                {
                    _logger.LogInformation($"Text: [{sentiments.ElementAt(i).Sentences.FirstOrDefault().Text}]" +
                        $" Positive score: [{sentiments.ElementAt(i).ConfidenceScores.Positive:0.00}]" +
                        $" Negative score: [{sentiments.ElementAt(i).ConfidenceScores.Negative:0.00}]" +
                        $" Neutral score: [{sentiments.ElementAt(i).ConfidenceScores.Neutral:0.00}]");
                    letters.ElementAt(i).Sentiment = sentiments.ElementAt(i);
                }
            }

            string santaReport = CreateSantaReport(letters);


            return new OkObjectResult(santaReport);
        }

        public static string CreateSantaReport(IEnumerable<Letter> letters)
        {
            List<object> SantaReport = new List<object>();

            foreach(Letter letter in letters)
            {
                string overallSentimentScore = letter.Sentiment.ConfidenceScores.Positive > 0.5 ? "Nice" :
                    letter.Sentiment.ConfidenceScores.Negative > 0.5 ? "Naughty" : "Neutral";

                object reportItem = new
                {
                    sender = letter.Sender,
                    text = letter.Text,
                    language = letter.EnglishTranslation.DetectedLanguage.Language,
                    translation = letter.EnglishTranslation.Translations[0].Text,
                    overall_sentiment_score = overallSentimentScore,
                    positive_sentiment = letter.Sentiment.ConfidenceScores.Positive,
                    negative_sentiment = letter.Sentiment.ConfidenceScores.Negative,
                    neutral_sentiment = letter.Sentiment.ConfidenceScores.Neutral
                };

                SantaReport.Add(reportItem);
            }

            string jsonString = JsonConvert.SerializeObject(SantaReport);

            return jsonString;
        }
    }
}
