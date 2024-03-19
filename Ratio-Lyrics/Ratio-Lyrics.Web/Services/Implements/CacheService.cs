using Microsoft.Extensions.Caching.Distributed;
using Ratio_Lyrics.Web.Services.Abstraction;
using System.Text;
using System.Text.Json;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger _logger;        

        public CacheService(IDistributedCache distributedCache, ILogger<CacheService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<T> GetOrExecute<T>(Task<T> task, string key, DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration)
        {
            try
            {
                _logger.LogInformation($"Start cache service. Key: {key}");
                T? result;
                var cacheResult = await _distributedCache.GetAsync(key);
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
