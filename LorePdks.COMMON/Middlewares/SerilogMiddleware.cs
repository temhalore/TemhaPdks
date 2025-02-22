using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LorePdks.COMMON.Configuration;
using LorePdks.COMMON.Extensions;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace LorePdks.COMMON.Middlewares
{
    public class SerilogMiddleware
    {
        private readonly RequestDelegate next;

        public SerilogMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {

            string headerdanAlinanToken = context == null ? "?" : context.Request.Headers[CoreConfig.TokenKeyName].ToString();
            LogContext.PushProperty(CoreConfig.TokenKeyName, headerdanAlinanToken);
            LogContext.PushProperty("RemoteIPAddress", context.GetRemoteIPAddress());//
            return next(context);
        }
    }
}
