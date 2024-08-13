using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubWebhookHandlerApp.Configurations
{
    public class GitHubSettings
    {
        public required string KeyVaultName { get; set; }
        public required string TokenName { get; set; }
        public required string UserAgentName { get; set; }
    }
}
