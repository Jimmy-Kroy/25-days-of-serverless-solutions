using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindImageApp.Helpers
{
    public class KeyVaultHelper : IKeyVaultHelper
    {
        private readonly Uri _endpointUrl;
        private SecretClient _secretClient;
        private readonly ILogger<KeyVaultHelper> _logger;
        private static Dictionary<string, string> _secretsCache = new Dictionary<string, string>();
        private readonly SecretClientOptions _options = new SecretClientOptions()
        {
            Retry =
            {
                Delay= TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(16),
                MaxRetries = 5,
                Mode = RetryMode.Exponential
            }
        };

        public KeyVaultHelper(ILogger<KeyVaultHelper> logger, string keyVaultName)
        {
            _logger = logger;
            _endpointUrl = new Uri($"https://{keyVaultName}.vault.azure.net");
            _secretClient = new SecretClient(_endpointUrl, new DefaultAzureCredential(), _options);
        }

        public string GetCachedSecret(string secretName)
        {
            string secret = string.Empty;

            if (_secretsCache.ContainsKey(secretName))
            {
                secret = _secretsCache[secretName];
                _logger.LogInformation($"Retrieved secret from cache, secretName[{secretName}], secret[{secret}]");
            }
            else
            {
                secret = _secretClient.GetSecret(secretName).Value.Value;
                _secretsCache.Add(secretName, secret);
                _logger.LogInformation($"Retrieved secret from Azure Key Vault, secretName[{secretName}], secret[{secret}]");
            }

            return secret;
        }
    }

}
