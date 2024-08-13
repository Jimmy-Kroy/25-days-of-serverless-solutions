using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealSeekerApp.Models
{
    public class DealTableEntity : BaseTableEntity
    {
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
        [JsonProperty("price")]
        public double Price { get; set; } = default;
        [JsonProperty("link")]
        public string Url { get; set; } = string.Empty;
    }
}
