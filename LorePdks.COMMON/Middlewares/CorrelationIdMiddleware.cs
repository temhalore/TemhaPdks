using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.Middlewares
{
    public class CorrelationIdMiddleware
    {
        public const string CorrelationIdHeaderKey = "X-Correlation-ID";


        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));

        }
        /// <summary>
        /// Header da X-Correlation-ID parametresi yoksa oluşturmak için
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            string correlationId = null;
            if (!httpContext.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out StringValues correlationIds))
            {
                correlationId = Guid.NewGuid().ToString();
                httpContext.Request.Headers.Add(CorrelationIdHeaderKey, correlationId);
            }

            await _next.Invoke(httpContext);
        }
    }
}
