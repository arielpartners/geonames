using System;

namespace GeoNames.API
{
    /// <summary>
    /// CachedValue&lt;TResult&gt; represents a value that may or may not have 
    /// originated from a cache, and timing information about when it was created
    /// and how long it took to create.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class CachedValue<TValue>
    {
        /// <summary>
        /// Create a CachedValue object.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="cached"></param>
        public CachedValue(ExecuteResult<TValue> result, bool cached)
        {
            Value = result.Value;
            Cached = cached;
            CreatedTime = result.CreatedTime;
            TimeToCreate = result.TimeToCreate;
            LifeTime = result.LifeTime();
        }

        /// <summary>
        /// Was this result already cached?
        /// </summary>
        public bool Cached { get; }

        /// <summary>
        /// When was this object instantiated?
        /// </summary>
        public DateTime CreatedTime { get; }

        /// <summary>
        /// Timespan it took to instantiate this value 
        /// </summary>
        public TimeSpan TimeToCreate { get; }

        /// <summary>
        /// The elapsed time from when the object was instantiated, NOT the instantiation elapsed time.
        /// </summary>
        public TimeSpan LifeTime { get; }

        /// <summary>
        /// The value of the object.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// ToString overload for friendly debugger displays.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            $"{Value}, Cached: {Cached}, TimeToCreate: {TimeToCreate.TotalMilliseconds}, LifeTime: {LifeTime.TotalMilliseconds}";
    }
}
