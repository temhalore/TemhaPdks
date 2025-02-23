
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using LorePdks.COMMON.Aspects.Interceptors;
using LorePdks.COMMON.Helpers;
using LorePdks.COMMON.Logging;
using Microsoft.Extensions.Configuration;
using LorePdks.COMMON.Logging.Serilog.LogUsingModel;

namespace LorePdks.COMMON.Aspects
{
    //tüm metodlar ilk giriş anında loglanır ve giriş parametreleri yazılır loga
    public class LogAspect : MethodInterception
    {
        
        private readonly Serilog.ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LogAspect()
        {

            _logger = LoggingConfiguration.Configuration(ServiceProviderHelper.ServiceProvider.GetService<IConfiguration>().GetSection("LogConfig"), $"{"LogAspect"}").CreateLogger();
            _httpContextAccessor = ServiceProviderHelper.ServiceProvider.GetService<IHttpContextAccessor>();
        }
        protected override void OnBefore(IInvocation invocation)
        {
            _logger?.Information(GetLogDetail(invocation));
       }

        private string GetLogDetail(IInvocation invocation)
        {
            var logParameters = new List<LogParameter>();
            for (var i = 0; i < invocation.Arguments.Length; i++)
            {
                string type11 = invocation.Arguments[i]?.GetType().Name;
                if (type11.Contains("Stream"))
                {
                    continue;
                }

                logParameters.Add(new LogParameter
                {
                    Name = invocation.GetConcreteMethod().GetParameters()[i].Name,
                    Value = invocation.GetConcreteMethod().GetParameters()[i].Name == "Password" ? "***" : invocation.Arguments[i],
                    Type = invocation.Arguments[i].GetType().Name,
                });
            }
            var logDetail = new LogDetail
            {
                MethodName = invocation.Method.DeclaringType.Name + "." + invocation.Method.Name,
                Parameters = logParameters
            };

            return JsonConvert.SerializeObject(logDetail);
        }
    }
}
