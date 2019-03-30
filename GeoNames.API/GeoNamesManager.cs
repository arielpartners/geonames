namespace GeoNames.API
{
    public static class GeoNamesManager
    {
        internal static string connectionString;
        internal static string userName;

        public static void Initialize(string connectionString, string userName)
        {
            GeoNamesManager.connectionString = connectionString;
            GeoNamesManager.userName = userName;
        }
    }
}
