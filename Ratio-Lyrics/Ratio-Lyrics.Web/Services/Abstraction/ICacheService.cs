using Ratio_Lyrics.Web.Constants;

namespace Ratio_Lyrics.Web.Services.Abstraction
{
    public interface ICacheService
    {        
        Task<T> GetOrExecute<T>(Lazy<Task<T>> lazyInitializeTask, string key, DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration);
        Task ClearCacheAsync(string pattern); 
    }
}
