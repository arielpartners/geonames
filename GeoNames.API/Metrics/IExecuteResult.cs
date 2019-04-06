using System;

namespace GeoNames.API
{
    public interface IExecuteResult
    {
        DateTime CreatedTime { get; set; }
        TimeSpan TimeToCreate { get; set; }
    }

    public interface IExecuteResult<T> : IExecuteResult
    {
        T Value { get; set; }
    }
}
