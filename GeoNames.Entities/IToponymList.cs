using System.Collections.Generic;

namespace GeoNames.Entities
{
    public interface IToponymList<TToponym>
    {
        IEnumerable<TToponym> items { get; set; }
    }
}
