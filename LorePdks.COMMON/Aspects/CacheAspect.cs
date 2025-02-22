using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using LorePdks.COMMON.Aspects.Caching;
using LorePdks.COMMON.Aspects.Interceptors;
using LorePdks.COMMON.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace LorePdks.COMMON.Aspects
{
    public class CacheAspect : MethodInterception
    {
        private int _duration;
        private ICacheManager _cacheManager;

        public CacheAspect(int duration = 60)
        {
            _duration = duration;
            _cacheManager = ServiceProviderHelper.ServiceProvider.GetService<ICacheManager>();
        }

        public override void Intercept(IInvocation invocation)
        {
            var methodName = string.Format($"{invocation.Method.ReflectedType.FullName}.{invocation.Method.Name}");
            var arguments = invocation.Arguments.ToList();
            var key = $"{methodName}({string.Join(",", arguments.Select(x => x?.ToString() ?? "<Null>"))})";

            if (_cacheManager.IsAdd(key))
            {
                string data = _cacheManager.GetStr(key);
                invocation.ReturnValue = JsonConvert.DeserializeObject(data, invocation.Method.ReturnType);
                //invocation.ReturnValue =  Task.FromResult((List<BroadcastMessageDTO>)data);
                return;
            }
            invocation.Proceed();
            if (invocation.ReturnValue != null)
                _cacheManager.Add(key, invocation.ReturnValue, _duration);
            //var task = (Task)invocation.ReturnValue;
            //task.ContinueWith((item) =>
            //{
            //    var result = item.GetType().GetProperty("Result").GetValue(item, null);
            //    _cacheManager.Add(key, result, _duration);
            //}).Wait();
        }

        string BuildKey(object[] args)
        {
            var sb = new StringBuilder();
            foreach (var arg in args)
            {
                var paramValues = arg.GetType().GetProperties().Select(p => p.GetValue(arg)?.ToString() ?? string.Empty);
                sb.Append(string.Join('_', paramValues));

            }
            return sb.ToString();
        }
    }
}
