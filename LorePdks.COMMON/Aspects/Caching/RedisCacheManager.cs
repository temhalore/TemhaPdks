using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LorePdks.COMMON.Aspects.Caching
{
    public class RedisCacheManager : ICacheManager
    {
        private RedisServer _redisServer;
        private static string _redisKeyParam = "SanalPos_";
        private readonly ILogger<RedisCacheManager> _logger;

        public RedisCacheManager(RedisServer redisServer, ILogger<RedisCacheManager> logger)
        {
            _redisServer = redisServer;
            _logger = logger;
        }

        //private readonly RedisEndpoint _redisEndpoint;

        //private void RedisInvoker(Action<RedisClient> redisAction)
        //{
        //    using (var client = new RedisClient(_redisEndpoint))
        //    {
        //        redisAction.Invoke(client);
        //    }
        //}

        //public RedisCacheManager()
        //{
        //    var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        //    var ip = configuration.GetSection("Redis:Host").Value;
        //    var port = Convert.ToInt32(configuration.GetSection("Redis:Port").Value);
        //    var password = configuration.GetSection("Redis:Password").Value;

        //    _redisEndpoint = new RedisEndpoint(ip, port, password);

        //}

        public T Get<T>(string key)
        {
            key = _redisKeyParam + key;
            if (IsAdd(key))
            {
                string jsonData = _redisServer.Database.StringGet(key);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }

            return default;

            //var result = default(T);
            //RedisInvoker(x => { result = x.Get<T>(key); });
            //return result;
        }
        public string GetStr(string key)
        {
            key = _redisKeyParam + key;
            if (IsAdd(key))
            {
                string jsonData = _redisServer.Database.StringGet(key);
                return jsonData;
            }

            return default;
            //var result = default(string);
            //RedisInvoker(x => { result = x.Get<string>(key); });
            //return Deserialize(result);
        }
        public object Get(string key)
        {
            key = _redisKeyParam + key;
            if (IsAdd(key))
            {
                string jsonData = _redisServer.Database.StringGet(key);
                return Deserialize(jsonData);
            }

            return default;
            //var result = default(string);
            //RedisInvoker(x => { result = x.Get<string>(key); });
            //return Deserialize(result);
        }

        public void Add(string key, object data, int duration)
        {
            key = _redisKeyParam + key;
            try
            {
                if (duration <= 0)
                    duration = 10;
                if (_redisServer.Database.IsConnected(key))
                    _redisServer.Database.StringSet(key, Serialize(data), TimeSpan.FromMinutes(duration));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message, null);
            }
            // RedisInvoker(x => x.Add(key, Serialize(data), TimeSpan.FromMinutes(duration)));
        }

        public void Add(string key, object data)
        {
            key = _redisKeyParam + key;
            if (_redisServer.Database.IsConnected(key))
                _redisServer.Database.StringSet(key, Serialize(data));
            //  RedisInvoker(x => x.Add(key, Serialize(data)));
        }

        public bool IsAdd(string key)
        {

            return _redisServer.Database.IsConnected(key) ? _redisServer.Database.KeyExists(key) : false;
            //var isAdded = false;
            //RedisInvoker(x => isAdded = x.ContainsKey(key));
            //return isAdded;
        }

        public void Remove(string key)
        {
            key = _redisKeyParam + key;
            if (_redisServer.Database.IsConnected(key))
                _redisServer.Database.KeyDelete(key);
            //RedisInvoker(x => x.Remove(key));
        }

        public void RemoveByPattern(string pattern)
        {

            _redisServer.Database.KeyDelete(pattern);
            //RedisInvoker(x => x.RemoveByPattern(pattern));
        }

        public void Clear()
        {
            _redisServer.FlushDatabase();
            //RedisInvoker(x => x.FlushAll());
        }

        private string Serialize<T>(T obj)
        {
            var data = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                // ContractResolver = new CustomConstructorResolver(),
                //SerializationBinder = new CustomSerializationBinder()
            });

            return data;
        }

        private object Deserialize(string data)
        {
            var obj = JsonConvert.DeserializeObject(data, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                // ContractResolver = new CustomConstructorResolver(),
                // SerializationBinder = new CustomSerializationBinder()
            });

            return obj;
        }
    }
}
