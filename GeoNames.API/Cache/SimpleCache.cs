using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using static GeoNames.API.Profiler;

namespace GeoNames.API
{
    /// <summary>
    /// Implements a Simple Cache.
    /// </summary>
    public static class SimpleCache
    {
        /// <summary>
        /// Log cache activity.
        /// </summary>
        readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Public entry point.  Looks for existing cached value, if not found,
        /// executed provided method and returns result.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="getter"></param>
        /// <param name="cacheExpirationSeconds"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public static CachedValue<R> Get<R>(
            Func<R> getter,                 // Method that gets the value if not ofund in the cache
            int cacheExpirationSeconds,     // # of seconds to cache the value
            params object[] parms           // Values used to generate unique cache key
            )
        {
            // Default to not expecting the value to be cached.
            bool isCached = false;

            // generate cache key.
            var key = GenerateCacheKey(parms);

            // Use cache key to try and existing cached value
            var cachedExecuteResult = (ExecuteResult<R>)GetCache()[key];

            if (cachedExecuteResult == null)
            {
                // The cache did not contain the value.

                // Execute the getter method to get the value.
                cachedExecuteResult = Execute(getter);

                // Update the cache statistics.
                Statistics.OnCreate(key);

                // Add this value to the cache.
                Add(key, cachedExecuteResult, DateTime.UtcNow.AddSeconds(cacheExpirationSeconds));
            }
            else
            {
                // The value was in the cache.
                isCached = true;
                _logger.Trace($"Cache({key}) requested and found in cache.");
            }

            // Return the value.
            return new CachedValue<R>(cachedExecuteResult, isCached);
        }

        /// <summary>
        /// Generate the cache key.
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        static string GenerateCacheKey(params object[] parms)
        {
            if (parms == null)
            {
                throw new Exception("Cannot generate Cache Key with null parameter list!");
            }
            return string.Join(":", parms.Select(parm => (parm ?? string.Empty).ToString()));
        }

        /// <summary>
        /// Add the object to the cache.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="absoluteExpiration"></param>
        static void Add<R>(string key, R obj, DateTime absoluteExpiration)
        {
            // If the object being set is non-null...
            if (!EqualityComparer<R>.Default.Equals(obj, default(R))) // long-hand / generic type-safe version of obj != null
            {
                // Get the cache.
                var cache = GetCache();

                // Set the cache entry.
                cache.Set(key, obj, new CacheItemPolicy
                {
                    AbsoluteExpiration = absoluteExpiration,
                    Priority = CacheItemPriority.Default,

                    // When cache item is being removed
                    RemovedCallback = cera =>
                    {
                        // Get the cache entry.
                        var cacheItem = cera.CacheItem;

                        // Get the cache entry value.
                        var value = (ExecuteResult)cacheItem.Value;

                        // Log the removal.
                        _logger.Trace($"SimpleCache({cacheItem.Key}) [{value}] Expired after {value.LifeTime()}. Reason: {cera.RemovedReason.ToString()}");

                        // And update cache statistics.
                        Statistics.OnRemove(cacheItem.Key, cera.RemovedReason);
                    }
                });
            }
        }

        // Clear cache entry.
        public static void Reset(params object[] parms) =>
            GetCache().Remove(GenerateCacheKey(parms));

        /// <summary>
        /// TODO: Generalize cache source to use configurable cache.
        /// </summary>
        /// <returns></returns>
        static ObjectCache GetCache() => MemoryCache.Default;
    }
}
