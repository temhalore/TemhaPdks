using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using LorePdks.COMMON.Aspects.Interceptors;
using LorePdks.COMMON.Aspects.Logging;
using LorePdks.COMMON.Aspects.Logging.Serilog;
using LorePdks.COMMON.Extensions;
using LorePdks.COMMON.Helpers;
using LorePdks.COMMON.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace LorePdks.COMMON.Aspects
{
    public class ExceptionLogAspect : MethodInterception
    {
        private readonly LoggerServiceBase _loggerServiceBase;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ExceptionLogAspect(Type loggerService)
        {
            if (loggerService.BaseType != typeof(LoggerServiceBase))
            {
                throw new ArgumentException("Wrong Logger Type");
            }

            _loggerServiceBase = (LoggerServiceBase)Activator.CreateInstance(loggerService);
            _httpContextAccessor = ServiceProviderHelper.ServiceProvider.GetService<IHttpContextAccessor>();
        }

        protected override void OnException(IInvocation invocation, Exception e)
        {
            var logDetailWithException = GetLogDetail(invocation);

            //if (e is AggregateException)
            //    logDetailWithException.ExceptionMessage = string.Join(Environment.NewLine, (e as AggregateException).InnerExceptions.Select(x => x.Message));
            //else
            //    logDetailWithException.ExceptionMessage = e.Message;
            if (e.GetType() != typeof(AppException))
            {
                _loggerServiceBase.Error(e, JsonConvert.SerializeObject(logDetailWithException).RemoveMessageBase64ImageData());
            }

        }

        private LogDetail GetLogDetail(IInvocation invocation)
        {
            var logParameters = new List<LogParameter>();
            for (var i = 0; i < invocation.Arguments.Length; i++)
            {
                string type11 = invocation.Arguments[i]?.GetType()?.Name ?? "";
                if (type11.Contains("Stream"))
                {
                    continue;
                }
                logParameters.Add(new LogParameter
                {
                    Name = invocation.GetConcreteMethod()?.GetParameters()[i]?.Name ?? "Name empty",
                    Value = invocation.Arguments[i],
                    Type = invocation.Arguments[i]?.GetType()?.Name
                });
            }
            var logDetail = new LogDetail
            {
                MethodName = invocation.Method.DeclaringType.Name + "." + invocation.Method.Name,
                Parameters = logParameters
            };
            return logDetail;
        }
    }
}
