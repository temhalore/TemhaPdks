using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using LorePdks.COMMON.Aspects.Interceptors;
using LorePdks.COMMON.Extensions;
using LorePdks.COMMON.Helpers;
using LorePdks.COMMON.Logging;
using LorePdks.COMMON.Logging.Serilog.LogUsingModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace LorePdks.COMMON.Aspects
{
    /// <summary>
    /// tüm metodlar giriş ve çıkış anı oalarak loglanır
    /// </summary>
    public class PerformanceAspect : MethodInterception
    {
        private readonly Serilog.ILogger _logger;

        private int _interval;
        private Stopwatch _stopwatch;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public PerformanceAspect(int interval)
        {
            _interval = interval;
            _stopwatch = ServiceProviderHelper.ServiceProvider.GetService<Stopwatch>();
            _logger = LoggingConfiguration.Configuration(ServiceProviderHelper.ServiceProvider.GetService<IConfiguration>().GetSection("LogConfig"), "PerformanceAspect").CreateLogger();
           _httpContextAccessor = ServiceProviderHelper.ServiceProvider.GetService<IHttpContextAccessor>();
        }

        protected override void OnBefore(IInvocation invocation)
        {
            _stopwatch.Start();
        }

        protected override void OnAfter(IInvocation invocation)
        {
            if (_stopwatch.Elapsed.TotalSeconds > _interval)
            {
                //_loggerServiceBase?.Warn(GetLogDetail(invocation, $"Performance : {_stopwatch.Elapsed.TotalSeconds}"));
                _logger?.Warning(GetLogDetail(invocation, $"Performance : {_stopwatch.Elapsed.TotalSeconds}"));
                //Debug.WriteLine($"Performance : {invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}-->{_stopwatch.Elapsed.TotalSeconds}");
            }
            _stopwatch.Reset();
        }

        private string GetLogDetail(IInvocation invocation, string performance)
        {
            var logParameters = new List<LogParameter>();
            for (var i = 0; i < invocation.Arguments.Length; i++)
            {
                // memorystreamler çok büyük oluyor json dönüşümü patlıyor bu nedenle geçildi 
                string type11 = invocation.Arguments[i]?.GetType().Name;
                if (type11.Contains("Stream"))
                {
                    continue;
                }

                logParameters.Add(new LogParameter
                {
                    Name = invocation.GetConcreteMethod().GetParameters()[i].Name,
                    Value = invocation.Arguments[i],
                    Type = invocation.Arguments[i]?.GetType().Name
                });
            }
            var logDetail = new LogDetailWithPerformance
            {
                MethodName = invocation.Method.DeclaringType.Name + "." + invocation.Method.Name,
                Parameters = logParameters,
                Performance = performance
            };

            return JsonConvert.SerializeObject(logDetail)
                              .RemoveMessageBase64ImageData();
        }
    }
}
