using DealSeekerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealSeekerApp.Services
{
    public class HtmlGeneratorService : IHtmlGeneratorService
    {
        private readonly string pageTemplate =
    @"<!DOCTYPE html>
<html>
<head>
    <title>Deals of today!</title>
    <link rel=""stylesheet"" href=""deals_style.css"">
</head>
<body>
    <h1 id=""heading"">Deals of today {0}!</h1>
    <table id=""data-table"">
        <thead>
            <tr>
                <th>Description</th>
                <th>Price</th>
                <th>Link</th>
            </tr>
        </thead>
        <tbody>
{1}
        </tbody>
    </table>
</body>
</html>";

        public string GenerateHtmlPage(IEnumerable<DealTableEntity> deals)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DealTableEntity deal in deals)
            {
                sb.Append("\t\t<tr>\n");
                sb.Append($"\t\t\t<td>{deal.Description}</td>\n");
                sb.Append($"\t\t\t<td>${deal.Price}</td>\n");
                sb.Append($"\t\t\t<td><a href=\"{deal.Url}\" target=\"_blank\">Visit</a></td>\n");
                sb.Append("\t\t</tr>\n");
            }

            string pageHtml = string.Format(pageTemplate, CurrentDateTimeInAmsterdam().ToString("dd-MM-yyyy HH:mm:ss"), sb.ToString());

            return pageHtml;
        }

        private DateTime CurrentDateTimeInAmsterdam()
        {
            TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localZone);

            return localTime;
        }
    }

}
