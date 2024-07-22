using Azure.AI.TextAnalytics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentAnalyzerApp.Models
{
    public class Letter
    {
        [JsonProperty(PropertyName = "sender")]
        public required string Sender { get; set; }

        [JsonProperty(PropertyName = "text")]
        public required string Text { get; set; }

        public TranslationResult? EnglishTranslation { get; set; } = null;

        public DocumentSentiment? Sentiment { get; set; } = null;
    }
}
