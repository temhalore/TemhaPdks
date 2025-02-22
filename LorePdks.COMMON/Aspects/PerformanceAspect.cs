using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using LorePdks.COMMON.Aspects.Interceptors;
using LorePdks.COMMON.Aspects.Logging;
using LorePdks.COMMON.Aspects.Logging.Serilog;
using LorePdks.COMMON.Aspects.Logging.Serilog.Logger;
using LorePdks.COMMON.Extensions;
using LorePdks.COMMON.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace LorePdks.COMMON.Aspects
{
    public class PerformanceAspect : MethodInterception
    {
        private int _interval;
        private Stopwatch _stopwatch;

        private readonly LoggerServiceBase _loggerServiceBase;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PerformanceAspect(int interval)
        {
            _interval = interval;
            _stopwatch = ServiceProviderHelper.ServiceProvider.GetService<Stopwatch>();

            _loggerServiceBase = (LoggerServiceBase)Activator.CreateInstance(typeof(PerformanceLogger));
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
                _loggerServiceBase?.Warn(GetLogDetail(invocation, $"Performance : {_stopwatch.Elapsed.TotalSeconds}"));
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
