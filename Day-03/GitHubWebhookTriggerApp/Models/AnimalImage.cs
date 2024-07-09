using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubWebhookTriggerApp.Models
{
    public class AnimalImage
    {
        public AnimalImage(string imageUrl, string author)
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
            ImageUrl = imageUrl;
            Author = author;
        }

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "creation_time")]
        public DateTime CreationTime { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }
    }

}
