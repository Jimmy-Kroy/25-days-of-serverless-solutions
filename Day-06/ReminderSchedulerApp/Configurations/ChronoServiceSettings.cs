using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderSchedulerApp.Configurations
{
    public class ChronoServiceSettings
    {
        /// <summary>
        ///     Service endpoint
        /// </summary>
        public required string EndpointUrl { get; set; }

        public string Key { get; set; } = string.Empty;
    }
}
