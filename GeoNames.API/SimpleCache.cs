using NLog;
using System;
using System.Linq;
using System.Runtime.Caching;
using static GeoNames.API.Profiler;

namespace GeoNames.API
{
    public static class SimpleCache
    {
        readonly static Logger _logger = LogManager.GetCurrentClassLogger();

        public static CachedResult<R> Get<R>(Func<R> getter, int cacheExpirationSeconds, params object[] parms) where R : class
        {
            var key = GenerateCacheKey(parms);
            var startTime = DateTime.UtcNow;
            bool? isCached = null;
            var cachedExecuteResult = (ExecuteResult<R>)MemoryCache.Default[key];

            if (cachedExecuteResult == null)
            {
                isCached = false;

                var profiledResult = Execute(getter);
                cachedExecuteResult = profiledResult;

                Statistics.OnCreate(key);
                Add(key, cachedExecuteResult, DateTime.UtcNow.AddSeconds(cacheExpirationSeconds));
            }
            else
            {
                isCached = true;
                //logger.Trace($"Cache({key}) requested and found in cache.")
            }

            return new CachedResult<R>(cachedExecuteResult.Result, isCached.Value, startTime);
        }

        static string GenerateCacheKey(params object[] parms)
        {
            if (parms == null)
            {
                throw new Exception("Cannot generate Cache Key with null parameter list!");
            }
            return string.Join(":", parms.Select(parm => (parm ?? string.Empty).ToString()));
        }

        static void Add<R>(string key, R obj, DateTime absoluteExpiration) where R : class
        {
            if (obj != null)
            {
                var cache = MemoryCache.Default;

                cache.Set(key, obj, new CacheItemPolicy
                {
                    AbsoluteExpiration = absoluteExpiration,
                    Priority = CacheItemPriority.Default,
                    RemovedCallback = cera =>
                    {
                        var cacheItem = cera.CacheItem;
                        var execResult = (ExecuteResult)cacheItem.Value;
                        _logger.Trace($"SimpleCache({cacheItem.Key}) [{execResult}] Expired after {execResult.LifeTime()}. Reason: {cera.RemovedReason.ToString()}");
                        Statistics.OnRemove(cacheItem.Key, cera.RemovedReason);
                    }
                });
            }
        }

        public static void Reset(params object[] parms) => MemoryCache.Default.Remove(GenerateCacheKey(parms));
    }
}
