using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderSchedulerApp.Models
{
    public class Reminder
    {
        public enum eStatus
        {
            None,
            Created,
            Activated
        };

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "iconEmoji")]
        public string IconEmoji { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "timezone")]
        public string Timezone { get; set; } = string.Empty;
        public DateTime Time { get; set; } = default;
        public eStatus Status { get; set; } = eStatus.None;

    }
}
