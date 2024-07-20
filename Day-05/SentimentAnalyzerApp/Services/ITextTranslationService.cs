using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentAnalyzerApp.Services
{
    public interface ITextTranslationService
    {
        Task<string> TranslateTextToEnglish(string text);
    }
}
