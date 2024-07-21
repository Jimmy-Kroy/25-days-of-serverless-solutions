using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentAnalyzerApp.Configurations
{
    public class TextTranslationClientSettings
    {
        public required string EndpointUrl { get; set; }
        public required string AzureRegion { get; set; }
        public required string Key { get; set; }
    }
}
