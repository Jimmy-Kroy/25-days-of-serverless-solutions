using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubWebhookHandlerApp.Models
{
    public class Issue
    {
        public string Sender { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public Uri? ReplyUri { get; set; } = null;
    }
}
