using ChristmasCardApp.Configurations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasCardApp.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisCacheSettings _redisCacheSettings;

        public CacheService(IDistributedCache distributedCache, IOptions<RedisCacheSettings> redisCacheSettings)
        {
            _distributedCache = distributedCache;
            _redisCacheSettings = redisCacheSettings.Value;
        }

        public async Task SetStringAsync(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                await this._distributedCache
                    .SetStringAsync(key, value,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _redisCacheSettings.CachingTime
                    });
            }
        }

        public async Task<string?> GetStringAsync(string key)
        {
            return await _distributedCache.GetStringAsync(key);
        }
    }

}
