using GitHubWebhookTriggerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubWebhookTriggerApp.Services
{
    public interface IGithubEventProcessor
    {
        IEnumerable<AnimalImage> GetImages(string jsonFile);
    }
}
