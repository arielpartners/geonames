namespace GeoNames.API
{
    public interface IApiMetricResult<T> : IExecuteResult<T>
    {
        string ResultSource { get; set; }
    }
}
