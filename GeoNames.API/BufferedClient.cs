using GeoNames.Entities;

namespace GeoNames.API
{
    public class BufferedClient : MemCacheClient
    {
        public BufferedClient(string connectionString, string userName) : 
            base(Verbosity.medium, new DBCacheClient(connectionString, new GeoNamesClient(userName)))
        {
        }

        public BufferedClient() : this(GeoNamesManager.connectionString, GeoNamesManager.userName)
        {
        }
    }
}
