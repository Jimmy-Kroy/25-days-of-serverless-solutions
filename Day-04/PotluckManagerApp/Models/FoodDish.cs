using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PotluckManagerApp.Models
{
    public class FoodDish
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonProperty(PropertyName = "CreationTime")]
        public DateTime CreationTime { get; set; } = DateTime.Now;

        [JsonProperty(PropertyName = "GuestName")]
        public string GuestName { get; set; }

        [JsonProperty(PropertyName = "Dish")]
        public string Dish { get; set; }

        [JsonProperty(PropertyName = "Amount")]
        public string Amount { get; set; }

        [JsonProperty(PropertyName = "IsVegan")]
        public bool IsVegan { get; set; }
    }
}
