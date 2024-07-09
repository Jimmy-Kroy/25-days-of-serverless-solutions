using GitHubWebhookTriggerApp.Models;
using Google.Protobuf.Compiler;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubWebhookTriggerApp.Services
{
    public class GithubEventProcessor : IGithubEventProcessor
    {
        private readonly ILogger<GithubEventProcessor> _logger;
        const string fileExtension = "png";

        public GithubEventProcessor(ILogger<GithubEventProcessor> logger)
        {
            _logger = logger;
        }

        public IEnumerable<AnimalImage> GetImages(string jsonString)
        {
            List<AnimalImage> images = new List<AnimalImage>();
            string repositoryUrl;
            string author;
            string branch;

            if (!IsValid(jsonString))
            {
                return Enumerable.Empty<AnimalImage>();
            }

            GithubEvent? githubEvent = JsonConvert.DeserializeObject<GithubEvent>(jsonString);

            if (githubEvent is null || githubEvent.commits is null || 
                githubEvent.repository is null || string.IsNullOrEmpty(githubEvent.@ref))
            {
                return Enumerable.Empty<AnimalImage>();
            }

            /* Get info needed to create AnimalImage object. */
            repositoryUrl = githubEvent.repository.HtmlUrl;
            branch = githubEvent.@ref.Replace("refs/heads", "");

            foreach (CommitItem commit in githubEvent.commits)
            {
                author = commit.Author.Username;
                foreach (string filename in commit.Added)
                {
                    if(filename.EndsWith(fileExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        string url = string.Concat(repositoryUrl, "/blob", branch, "/", filename);
                        AnimalImage image = new AnimalImage(url, author);
                        images.Add(image);
                    }
                }
            }

            return images;

            /* You can also use JObject to retrieve all the needed fields from the json file 
            JObject data = JObject.Parse(jsonFile);
            string refs = data.SelectToken("ref").ToString().Replace("refs/heads", "");
            string htmlUrl = data.SelectToken("repository.html_url").ToString();
            JToken added = data.SelectToken("head_commit.added");
            string added = data.SelectToken("head_commit.added").ToString();
            JToken items = data.SelectToken("head_commit.added");
            foreach(dynamic item in items)
            {
                string file = item.Value.ToString();
            }*/
        }

        private bool IsValid(string jsonString)
        {
            try
            {
                JsonDocument.Parse(jsonString);
                return true;
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogError($"Error in IsValid(string jsonString): {ex.ToString()}");
                return false;
            }
        }

    }
}
