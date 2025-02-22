
using LorePdks.COMMON.Aspects.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;

namespace LorePdks.COMMON.Helpers
{
    public class RequestHelper : MethodInterception
    {
        public static string GetIPAddress()
        {
            var httpContext = ServiceProviderHelper.ServiceProvider.GetService<IHttpContextAccessor>().HttpContext;
            if (httpContext == null) return "127.0.0.1";
            string adres = "";
            IPAddress ip;
            var headers = httpContext.Request.Headers.ToList();
            if (headers.Exists((kvp) => kvp.Key == "X-Forwarded-For"))
            {
                // when running behind a load balancer you can expect this header
                var header = headers.First((kvp) => kvp.Key == "X-Forwarded-For").Value.ToString();
                ip = IPAddress.Parse(header);
            }
            else
            {
                // this will always have a value (running locally in development won't have the header)
                ip = httpContext.Request.HttpContext.Connection.RemoteIpAddress;
            }

            adres = ip.ToString();
            adres = adres.Replace("::1", "127.0.0.1");

            return adres;


        }

    }
}
