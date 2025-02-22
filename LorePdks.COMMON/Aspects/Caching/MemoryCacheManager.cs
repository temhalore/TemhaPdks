using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LorePdks.COMMON.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace LorePdks.COMMON.Aspects.Caching
{
    public class MemoryCacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;
        public MemoryCacheManager() : this(ServiceProviderHelper.ServiceProvider.GetService<IMemoryCache>())
        {
        }
        public MemoryCacheManager(IMemoryCache cache)
        {
            _cache = cache;
        }
        public void Add(string key, object data, int duration)
        {
            _cache.Set(key, Serialize(data), TimeSpan.FromMinutes(duration));
        }

        public void Add(string key, object data)
        {
            _cache.Set(key, Serialize(data));
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }
        public string GetStr(string key)
        {
            return _cache.Get(key).ToString();
        }

        public object Get(string key)
        {
            return Deserialize(_cache.Get(key).ToString());
        }

        public Task<T> GetValue<T>(string key)
        {
            throw new NotImplementedException();
        }

        public bool IsAdd(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void RemoveByPattern(string pattern)
        {
            var cacheEntriesCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var cacheEntriesCollection = cacheEntriesCollectionDefinition.GetValue(_cache) as dynamic;

            var cacheCollectionValues = new List<ICacheEntry>();

            foreach (var cacheItem in cacheEntriesCollection)
            {
                ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);
                cacheCollectionValues.Add(cacheItemValue);
            }

            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = cacheCollectionValues.Where(d => regex.IsMatch(d.Key.ToString())).Select(d => d.Key).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        private string Serialize<T>(T obj)
        {
            var data = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //  ContractResolver = new CustomConstructorResolver(),
                //  //SerializationBinder = new CustomSerializationBinder()
            });

            return data;
        }

        private object Deserialize(string data)
        {
            var obj = JsonConvert.DeserializeObject(data, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //  ContractResolver = new CustomConstructorResolver(),
                //  SerializationBinder = new CustomSerializationBinder()
            });

            return obj;
        }
    }
}
