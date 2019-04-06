using System;

namespace GeoNames.API
{
    /// <summary>
    /// The Profiler class executes arbitrary methods while running a stopwatch to time them.
    /// </summary>
    public static class Profiler
    {
        /// <summary>
        /// This is the public entry point to use Profiler.  It executes the method and returns the result and timing.
        /// </summary>
        /// <remarks>
        /// On purpose, exception handling is not implemented at this level, but is left to the implementor.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static ExecuteResult<T> Execute<T>(Func<T> method) => new ExecuteResult<T>(method);
    }
}
