namespace GeoNames.Entities
{
    public class City : Region
    {
        public string cityName
        {
            get => toponymName;
            set => toponymName = value;
        }
    }
}
