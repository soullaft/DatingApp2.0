using Microsoft.Extensions.Caching.Memory;

namespace API.Data.Cache
{
    public class CacheManager(IMemoryCache memoryCache)
    {
        public bool TryGetValue<T>(object key, out T? value) where T: class
        {
            if (memoryCache.TryGetValue(key, out T? result))
            {
                if (result == null)
                {
                    value = default;
                    return true;
                }

                if (result is T item)
                {
                    value = item;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public void SetValue<T>(object key, T value)
        {
            memoryCache.Set(key, value, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                .SetSize(100));
        }
    }
}
