using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubWebhookTriggerApp.Configurations
{
    public class ContainerInfo
    {
        /// <summary>
        ///     Container Name
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        ///     Container partition Key
        /// </summary>
        public required string PartitionKey { get; set; }
    }
}
