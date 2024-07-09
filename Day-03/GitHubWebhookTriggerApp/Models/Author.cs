using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubWebhookTriggerApp.Models
{
    public class Author
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
    }
}
