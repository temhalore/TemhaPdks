using Castle.DynamicProxy;
using LorePdks.COMMON.Aspects.Interceptors;
using LorePdks.COMMON.Helpers;
using LorePdks.COMMON.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LorePdks.COMMON.Aspects
{
    /// <summary>
    /// yetkisel loglama yada işlem gerekirse yapılabilir
    /// </summary>
    public class PermissionAspect : MethodInterception
    {
        private readonly Serilog.ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAspect()
        {
            _logger = LoggingConfiguration.Configuration(ServiceProviderHelper.ServiceProvider.GetService<IConfiguration>().GetSection("LogConfig"), "PermissionAspect").CreateLogger();
            _httpContextAccessor = ServiceProviderHelper.ServiceProvider.GetService<IHttpContextAccessor>();
        }


        protected override void OnBefore(IInvocation invocation)
        {
            string methodName = invocation.Method.DeclaringType.Name + "." + invocation.Method.Name;

        }

    }
}
