using AspNetCoreHero.ThrowR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
namespace Application.Extensions.Caching
{
    public static class Extensions
    {
        public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string cacheKey, CancellationToken token = default)
        {
            Throw.Exception.IfNull(distributedCache, nameof(distributedCache));
            Throw.Exception.IfNull(cacheKey, nameof(cacheKey));
            byte[] utf8Bytes = await distributedCache.GetAsync(cacheKey, token).ConfigureAwait(false);
            if (utf8Bytes != null)
            {
                return JsonSerializer.Deserialize<T>(utf8Bytes);
            }
            return default;
        }
        public static async Task RemoveAsync(this IDistributedCache distributedCache, string cacheKey, CancellationToken token = default)
        {
            Throw.Exception.IfNull(distributedCache, nameof(distributedCache));
            Throw.Exception.IfNull(cacheKey, nameof(cacheKey));
            await distributedCache.RemoveAsync(cacheKey, token).ConfigureAwait(false);
        }
        public static async Task SetAsync<T>(this IDistributedCache distributedCache, string cacheKey, T obj, int cacheExpirationInMinutes = 30, CancellationToken token = default)
        {
            Throw.Exception.IfNull(distributedCache, nameof(distributedCache));
            Throw.Exception.IfNull(cacheKey, nameof(cacheKey));
            Throw.Exception.IfNull(obj, nameof(obj));
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
             .SetSlidingExpiration(TimeSpan.FromMinutes(cacheExpirationInMinutes));
            byte[] utf8Bytes = JsonSerializer.SerializeToUtf8Bytes<T>(obj);
            await distributedCache.SetAsync(cacheKey, utf8Bytes, options, token).ConfigureAwait(false);
        }

        public static T Get<T>(this IDistributedCache distributedCache, string cacheKey, CancellationToken token = default)
        {
            Throw.Exception.IfNull(distributedCache, nameof(distributedCache));
            Throw.Exception.IfNull(cacheKey, nameof(cacheKey));
            byte[] utf8Bytes = distributedCache.Get(cacheKey);
            if (utf8Bytes != null)
            {
                return JsonSerializer.Deserialize<T>(utf8Bytes);
            }
            return default;
        }
        public static void Remove(this IDistributedCache distributedCache, string cacheKey, CancellationToken token = default)
        {
            Throw.Exception.IfNull(distributedCache, nameof(distributedCache));
            Throw.Exception.IfNull(cacheKey, nameof(cacheKey));
            distributedCache.Remove(cacheKey);
        }
        public static void Set<T>(this IDistributedCache distributedCache, string cacheKey, T obj, int cacheExpirationInMinutes = 30, CancellationToken token = default)
        {
            Throw.Exception.IfNull(distributedCache, nameof(distributedCache));
            Throw.Exception.IfNull(cacheKey, nameof(cacheKey));
            Throw.Exception.IfNull(obj, nameof(obj));
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
             .SetSlidingExpiration(TimeSpan.FromMinutes(cacheExpirationInMinutes));
            byte[] utf8Bytes = JsonSerializer.SerializeToUtf8Bytes<T>(obj);
            distributedCache.Set(cacheKey, utf8Bytes, options);
        }
    }
}
