using Application.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;
using NStandard.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class MemoryCacheRepository: IMemoryCacheRepository
    {
        private IMemoryCache _cache;
        public MemoryCacheRepository(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        public T CacheTryGetValue<T>(string key)
        {
            T cacheEntry;
            _cache.TryGetValue(key, out cacheEntry);
            return cacheEntry;
        } 
        public void CacheRemoce(string key)
        {
            _cache.Remove(key);
        }
        public List<T> CacheTrySetValue<T>(string key, List<T> value, double FromSeconds = 30)
        {
            List<T> cacheEntry;

            // Look for cache key.
            if (!_cache.TryGetValue(key, out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = value;

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(FromSeconds));

                // Save data in cache.
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
        public T CacheTrySetValue<T>(string key, T value, double FromSeconds = 30)
        {
            T cacheEntry;

            // Look for cache key.
            if (!_cache.TryGetValue(key, out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = value;

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(FromSeconds));

                // Save data in cache.
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
        public List<T> CacheTryGetValueSet<T>(string key, List<T> value, double FromSeconds = 30)
        {
            List<T> cacheEntry;

            // Look for cache key.
            if (!_cache.TryGetValue(key, out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = value;

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(FromSeconds));

                // Save data in cache.
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}
