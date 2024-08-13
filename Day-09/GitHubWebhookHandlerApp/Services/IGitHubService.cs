using GitHubWebhookHandlerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubWebhookHandlerApp.Services
{
    public interface IGitHubService
    {
        bool TryParse(string jsonRequest, ref Issue issue);
        Task<bool> SendThankYouAsync(Issue issue);
    }
}
