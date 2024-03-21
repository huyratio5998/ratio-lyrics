using Microsoft.Extensions.Caching.Distributed;
using Ratio_Lyrics.Web.Services.Abstraction;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;        
        private readonly ILogger _logger;

        public CacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer, ILogger<CacheService> logger)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }

        private async IAsyncEnumerable<string> GetCacheKeys(string pattern)
        {            
            foreach (var endPoint in _connectionMultiplexer.GetEndPoints())
            {
                var server = _connectionMultiplexer.GetServer(endPoint);
                foreach (var key in server.Keys(pattern: pattern))
                {
                    yield return key.ToString();
                }
            }
        }
        public async Task ClearCacheAsync(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return;

            await foreach (var key in GetCacheKeys(pattern))
            {
                await _distributedCache.RemoveAsync(key);
            }
            
            _logger.LogInformation($"Clear cache with pattern: {pattern}");
        }

        public async Task<T> GetOrExecute<T>(Task<T> task, string key, DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration)
        {
            try
            {
                _logger.LogInformation($"Start cache service. Key: {key}");
                T? result;
                _logger.LogInformation($"Start get cache value from key");
                var cacheResult = await _distributedCache.GetAsync(key);
                _logger.LogInformation($"End get cache value from key");
                if (cacheResult != null)
                {
                    _logger.LogInformation("Get cache data");
                    result = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(cacheResult));
                }
                else
                {
                    result = await task;
                    var option = new DistributedCacheEntryOptions()
                                .SetAbsoluteExpiration(absoluteExpiration)
                                .SetSlidingExpiration(slidingExpiration);

                    await _distributedCache.SetAsync(key, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result)), option);
                    _logger.LogInformation($"Run task, get data. Save data to cache with Key: {key}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception get data: " + ex);
                throw ex;
            }
        }
    }
}
