
using Microsoft.Extensions.Caching.Memory;

namespace ProudctManagementDashboard.Api.Cache
{
    public class MemoryCacheService : IMemeoryCacheService
    {
        private readonly IMemoryCache _memoryCache;
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T Get<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T value);
            return value;
        }

        public void Set<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            if (absoluteExpireTime.HasValue)
            {
                options.SetAbsoluteExpiration(absoluteExpireTime.Value);
            }
            if (slidingExpireTime.HasValue)
            {
                options.SetSlidingExpiration(slidingExpireTime.Value);
            }
            _memoryCache.Set(key, value, options);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
