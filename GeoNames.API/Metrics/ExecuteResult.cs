using System;
using System.Diagnostics;

namespace GeoNames.API
{
    /// <summary>
    /// ExecuteResult is the method / return type-agnostic base class which handles the method timing.
    /// </summary>
    public class ExecuteResult : IExecuteResult
    {
        /// <summary>
        /// ExecuteResult is instantiated and the method executed in a single line of code,
        /// so the method StartTime is when the ExecuteResult is instantiated.
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timespan it took to instantiate this value 
        /// </summary>
        public TimeSpan TimeToCreate { get; set; }

        /// <summary>
        /// LifeTime is the elapsed time from when the method was executed, NOT the method elapsed time.
        /// This timespan is useful to determine length of time in a cache.
        /// </summary>
        /// <returns></returns>
        public TimeSpan LifeTime() => DateTime.UtcNow.Subtract(CreatedTime);

        /// <summary>
        /// The StopWatch that times the method execution.
        /// </summary>
        public Stopwatch StopWatch { get; set; }
    }

    /// <summary>
    /// ExecuteResult&lt;T&gt; is the class actually used to execute and time the method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExecuteResult<T> : ExecuteResult, IExecuteResult<T>
    {
        public ExecuteResult()
        {
        }

        /// <summary>
        /// The ExecuteResult&lt;T&gt; constructor executes and times the method.
        /// </summary>
        /// <param name="method"></param>
        public ExecuteResult(Func<T> method)
        {
            StopWatch = new Stopwatch();
            StopWatch.Start();
            Value = method();
            StopWatch.Stop();
            TimeToCreate = StopWatch.Elapsed;
        }

        /// <summary>
        /// The result of executing the method.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// ToString overload for friendly debugger displays.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value.ToString();
    }
}
