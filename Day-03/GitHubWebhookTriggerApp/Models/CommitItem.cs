using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubWebhookTriggerApp.Models
{
    public class CommitItem
    {
        [JsonProperty(PropertyName = "added")]
        public IEnumerable<string> Added { get; set; }

        [JsonProperty(PropertyName = "author")]
        public Author Author { get; set; }
    }
}
