using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishListApp.Configurations
{
    public class SlackSettings
    {
        /// <summary>
        ///     Slack webhook
        /// </summary>
        public required string EndpointUrl { get; set; }
    }
}
