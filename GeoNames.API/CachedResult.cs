using System;

namespace GeoNames.API
{
    public class CachedResult<TResult>
    {
        public CachedResult(TResult result, bool cached, DateTime startTime)
        {
            Result = result;
            Cached = cached;
            StartTime = startTime;
            Elapsed = DateTime.UtcNow.Subtract(startTime);
        }

        public bool Cached { get; }
        public DateTime StartTime { get; }
        public TimeSpan Elapsed { get; }
        public TResult Result { get; }

        public override string ToString()
        {
            return $"{Result}, Cached: {Cached}, Elapsed: {Elapsed.TotalMilliseconds}";
        }
    }
}
