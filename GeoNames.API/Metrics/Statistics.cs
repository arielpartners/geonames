using System;
using System.Configuration;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace GeoNames.API
{
    public class Statistics
    {
        static readonly object @lock = new object();

        public string Key { get; set; }
        public DateTime? CreateUtcDate { get; set; }
        public DateTime? RemoveUtcDate { get; set; }
        public int HitCount { get; set; }
        public CacheEntryRemovedReason? RemoveReason { get; set; }

        Statistics(string key)
        {
            Key = key;
            CreateUtcDate = DateTime.UtcNow;
        }

        static Dictionary<string, Statistics> _activeStats = new Dictionary<string, Statistics>();
        static Dictionary<Guid, Statistics> _expiredStats = new Dictionary<Guid, Statistics>();

        static bool? _enableStatistics;
        public static bool EnableStatistics
        {
            get
            {
                if (!_enableStatistics.HasValue)
                {
                    _enableStatistics = bool.Parse(ConfigurationManager.AppSettings["EnableStatistics"] ?? "true");
                }
                return _enableStatistics.Value;
            }
            set
            {
                _enableStatistics = value;
            }
        }

        public static IDictionary<string, Statistics> Get()
        {
            return _activeStats;
        }

        internal static void OnCreate(string key)
        {
            if (EnableStatistics)
            {
                lock (@lock)
                {
                    var stat = new Statistics(key);
                    _activeStats[key] = stat;
                    // _activeStats.Add(key, stat) ' was sometimes causing dupe key errors
                }
            }
        }

        internal static void OnGet(string key)
        {
            if (EnableStatistics)
            {
                lock (@lock)
                {
                    Statistics stat = null;
                    if (_activeStats.TryGetValue(key, out stat))
                    {
                        stat.HitCount += 1;
                    }
                }
            }
        }

        internal static void OnRemove(string key, CacheEntryRemovedReason reason)
        {
            if (EnableStatistics)
            {
                lock (@lock)
                {
                    Statistics stat = null;
                    if (_activeStats.TryGetValue(key, out stat))
                    {
                        _activeStats.Remove(key);

                        stat.RemoveUtcDate = DateTime.UtcNow;
                        stat.RemoveReason = reason;

                        _expiredStats.Add(Guid.NewGuid(), stat);
                    }
                }
            }
        }
    }
}
