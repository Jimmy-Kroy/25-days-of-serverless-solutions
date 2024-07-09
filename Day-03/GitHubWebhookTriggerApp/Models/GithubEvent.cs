using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace GitHubWebhookTriggerApp.Models
{
    public class GithubEvent
    {

        [JsonProperty(PropertyName = "ref")]
        public string @ref { get; set; }

        [JsonProperty(PropertyName = "repository")]
        public Repository repository { get; set; }

        [JsonProperty(PropertyName = "commits")]
        public List<CommitItem> commits { get; set; }
    }
}
