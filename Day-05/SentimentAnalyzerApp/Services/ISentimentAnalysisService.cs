﻿using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentAnalyzerApp.Services
{
    public interface ISentimentAnalysisService
    {
        Task<DocumentSentiment?> AnalyzeSentimentAsync(string text);
    }
}
