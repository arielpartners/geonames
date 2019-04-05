namespace GeoNames.Entities
{
    public class Region : Toponym
    {
        public string regionName
        {
            get => adminName1;
            set => adminName1 = value;
        }

        public string regionCode
        {
            get => adminCode1;
            set => adminCode1 = value;
        }
    }
}
