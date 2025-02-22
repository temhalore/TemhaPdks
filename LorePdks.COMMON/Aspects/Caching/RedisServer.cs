using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace LorePdks.COMMON.Aspects.Caching
{
    public class RedisServer
    {
        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _database;
        private int _currentDatabaseId = 0;

        public RedisServer(IConfiguration configuration)
        {
            string host = configuration.GetSection("Redis:Host").Value;
            //int port = configuration.GetSection("Redis:Port").Value.ToInt32();
            int port = Convert.ToInt32(configuration.GetSection("Redis:Port").Value);
            string username = configuration.GetSection("Redis:Username").Value;
            string password = configuration.GetSection("Redis:Password").Value;
            var opt = new ConfigurationOptions()
            {
                User = username,
                //Password = password,
                EndPoints = { { host, port } },
                AbortOnConnectFail = false
            };

            _connectionMultiplexer = ConnectionMultiplexer.Connect(opt);
            _database = _connectionMultiplexer.GetDatabase(_currentDatabaseId);
        }

        public IDatabase Database => _database;

        public void FlushDatabase()
        {
            if (_connectionMultiplexer.GetServers().Length > 0)
                _connectionMultiplexer.GetServers()[0].FlushDatabase(_currentDatabaseId);
        }

    }
}
