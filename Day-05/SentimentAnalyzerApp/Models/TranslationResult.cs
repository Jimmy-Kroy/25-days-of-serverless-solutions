using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SentimentAnalyzerApp.Models
{
    public class TranslationResult
    {
        [JsonProperty(PropertyName = "detectedLanguage")]
        public DetectedLanguage DetectedLanguage { get; set; }
        [JsonProperty(PropertyName = "translations")]
        public List<Translation> Translations { get; set; }
    }
}
