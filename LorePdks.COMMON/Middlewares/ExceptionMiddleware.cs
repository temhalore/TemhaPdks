using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using LorePdks.COMMON.Helpers;
using LorePdks.COMMON.Logging;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;
namespace LorePdks.COMMON.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ILogger _logger { get; set; }

        public ExceptionMiddleware(RequestDelegate next)
        {
           
            _next = next;
          
            _logger = LoggingConfiguration.Configuration(ServiceProviderHelper.ServiceProvider.GetService<IConfiguration>().GetSection("LogConfig"), $"{"ExceptionMiddleware"}").CreateLogger();
            // _logger = LoggingConfiguration.Configuration(ServiceProviderHelper.ServiceProvider.GetService<IConfiguration>().GetSection("LogConfig")).CreateLogger();

        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (AppException e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception e)
        {
            httpContext.Response.ContentType = "application/json";

            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

            var response = new ServiceResponse<object>();

            if (e.GetType() == typeof(ApplicationException))
            {
                response.message = e.Message;
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (e.GetType() == typeof(UnauthorizedAccessException))
            {
                response.message = e.Message;
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else if (e.GetType() == typeof(SecurityException))
            {
                response.message = e.Message;
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else if (e.GetType() == typeof(AppException))
            {
                response.messageHeader = "Uygulama İç Hata Gönderdi";
                response = new ServiceResponse<object>((AppException)e);

                httpContext.Response.StatusCode = StatusCodes.Status200OK;
            }
            else
            {

                response.message = "Internal Server Error";
            }
            if (string.IsNullOrWhiteSpace(response.messageHeader)) response.messageHeader = "Bir Hata Oluştu";


            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

    }
}
