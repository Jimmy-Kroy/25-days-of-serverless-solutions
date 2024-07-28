using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindImageApp.Configurations
{
    public class UnsplashClientSettings
    {
        public required string EndpointUrl { get; set; }
        public required string KeyVaultName { get; set; }
        public required string SecretName { get; set; }
    }
}
