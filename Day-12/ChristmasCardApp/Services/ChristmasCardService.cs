using Markdig;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasCardApp.Services
{
    public class ChristmasCardService : IChristmasCardService
    {
        private readonly ILogger<ChristmasCardService> _logger;
        public ChristmasCardService(ILogger<ChristmasCardService> logger)
        {
            _logger = logger;
        }

        public string GetChristmasCard(string markdownContent)
        {
            StringBuilder stringBuilder = new StringBuilder();

            string htmlContent = Markdown.ToHtml(markdownContent);

            stringBuilder.AppendLine("<!DOCTYPE html>");
            stringBuilder.AppendLine("<html lang=\"en\">");
            stringBuilder.AppendLine("<head>");
            stringBuilder.AppendLine("</head>");
            stringBuilder.AppendLine("<body>");
            stringBuilder.AppendLine(htmlContent);
            stringBuilder.AppendLine("</body>");
            stringBuilder.AppendLine("</html>");

            return stringBuilder.ToString();
        }
    }
}
