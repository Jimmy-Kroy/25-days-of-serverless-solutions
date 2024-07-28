using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindImageApp.Models
{
    public class Image
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "regular_url")]
        public Uri? RegularUrl { get; set; } = null;

        [JsonProperty(PropertyName = "download_url")]
        public Uri? DownloadUrl { get; set; } = null;

        [JsonProperty(PropertyName = "alt_description")]
        public string AltDescription { get; set; } = string.Empty;

        public string FileName
        {
            get
            {
                return (AltDescription + ".png").Replace(' ', '_');
            }
        }
    }
}
