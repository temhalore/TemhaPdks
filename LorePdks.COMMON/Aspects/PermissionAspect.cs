using Castle.DynamicProxy;
using LorePdks.COMMON.Aspects.Interceptors;
using LorePdks.COMMON.Aspects.Logging.Serilog;
using LorePdks.COMMON.Aspects.Logging.Serilog.Logger;
using LorePdks.COMMON.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LorePdks.COMMON.Aspects
{
    public class PermissionAspect : MethodInterception
    {

        private readonly LoggerServiceBase _loggerServiceBase;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAspect()
        {
            _loggerServiceBase = (LoggerServiceBase)Activator.CreateInstance(typeof(PerformanceLogger));
            _httpContextAccessor = ServiceProviderHelper.ServiceProvider.GetService<IHttpContextAccessor>();
        }


        protected override void OnBefore(IInvocation invocation)
        {
            string methodName = invocation.Method.DeclaringType.Name + "." + invocation.Method.Name;

        }

    }
}
