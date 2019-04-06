using System;

namespace GeoNames.API
{
    public class ApiMetricResult<T> : ExecuteResult<T>, IApiMetricResult<T>
    {
        public ApiMetricResult()
        {
        }

        public ApiMetricResult(string source, Func<T> method) : base(method)
        {
            ResultSource = source;
        }

        public string ResultSource { get; set; }

        /// <summary>
        /// ToString overload for friendly debugger displays.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{ResultSource}: {TimeToCreate}";
    }
}
