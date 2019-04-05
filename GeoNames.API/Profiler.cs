using System;
using System.Diagnostics;

namespace GeoNames.API
{
    public static class Profiler
    {
        public class ExecuteResult
        {
            public DateTime StartTime { get; } = DateTime.UtcNow;
            public TimeSpan LifeTime() => DateTime.UtcNow.Subtract(StartTime);
            public Stopwatch StopWatch { get; set; }
        }

        public class ExecuteResult<T> : ExecuteResult
        {
            public ExecuteResult(Func<T> method) 
            {
                StopWatch = new Stopwatch();
                StopWatch.Start();
                Result = method();
                StopWatch.Stop();
            }

            public T Result { get; set; }

            public override string ToString() => Result.ToString();
        }

        public static ExecuteResult<T> Execute<T>(Func<T> method) => new ExecuteResult<T>(method);
    }
}
