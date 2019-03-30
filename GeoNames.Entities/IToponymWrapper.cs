namespace GeoNames.Entities
{
    public interface IToponymWrapper<TToponym>
    {
        TToponym item { get; set; }
    }
}
